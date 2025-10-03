using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Packmule.Api.Documents;
using Packmule.Api.Requests;
using Packmule.Core.Interfaces;
using Packmule.Infrastructure.PostgreSQL.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpLogging(o => { });

builder.Services.AddPostgreSQL(builder.Configuration.GetConnectionString("DatabaseContext"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpLogging();

app.UseHttpsRedirection();

/* LOGIN AND USER CONTROLS */

app.MapPost("/-/v1/login", async (HttpRequest request) =>
{
    var doc = await JsonDocument.ParseAsync(request.Body);
    var name = doc.RootElement.GetProperty("name").GetString() ?? "user";

    var token = $"packmule-{Guid.NewGuid():N}";
    return Results.Created($"/-/user/{name}", new { token, username = name, ok = true });
});

app.MapPut("/-/user/org.couchdb.user:{username}", (string username) =>
{
    return Results.Created($"/-/user/org.couchdb.user:{username}", new { ok = true, _id = $"org.couchdb.user:{username}", rev = "1-1", token = $"packmule-{Guid.NewGuid()}" });
});

app.MapGet("/-/user/org.couchdb.user:{username}", (string username) =>
{
    return Results.Ok(new
    {
        _id = $"org.couchdb.user:{username}",
        name = username,
        roles = Array.Empty<string>(),
        type = "user"
    });
});

app.MapGet("/-/whoami", (HttpRequest request) =>
{
    var auth = request.Headers.Authorization.ToString();
    if (string.IsNullOrWhiteSpace(auth))
        return Results.Unauthorized();

    return Results.Ok(new { username = "dev" });
});

/* PACKAGE ENDPOINTS */

/// <summary>
/// Get package metadata.
/// </summary>
/// <param name="packageName"></param>
app.MapGet("/{packageName}", async (string packageName, IUnitOfWork unitOfWork, CancellationToken cancellationToken) =>
{
    var package = await unitOfWork.PackageRepository.GetPackageAsync(Uri.UnescapeDataString(packageName), cancellationToken);
    if (package is null)
        return Results.NotFound(new { error = "Package not found." });

    var tags = package.DistTags.ToDictionary(d => d.Tag, d => d.Version, StringComparer.Ordinal);
    var versions = package.Versions.ToDictionary(
        v => v.Version,
        v => new PackageVersionDocument(
            v.Version,
            v.Manifest,
            v.TarballUri,
            v.Integrity,
            v.Deprecation,
            v.CreatedAt
        ),
        StringComparer.Ordinal);

    var time = new Dictionary<string, DateTimeOffset>(StringComparer.Ordinal)
    {
        ["created"] = package.CreatedAt,
        ["modified"] = package.ModifiedAt
    };

    foreach (var v in package.Versions)
        time[v.Version] = v.CreatedAt;

    var document = new PackageDocument(
        package.Name,
        tags,
        versions,
        time,
        package.CreatedAt,
        package.ModifiedAt
    );

    return Results.Ok(document);
})
.WithName("GetPackage")
.WithOpenApi();

/// <summary>
/// Get package version tarball.
/// </summary>
/// <param name="package"></param>
/// <param name="file"></param>
app.MapGet("/{package}/-/{file}", async (string package, string file, HttpContext httpContext) =>
{
    var rootDir = "";
    var safePackageDir = Uri.EscapeDataString(Uri.UnescapeDataString(package));
    var dir = Path.Combine(rootDir, safePackageDir);
    var path = Path.Combine(dir, file);

    var fullPath = Path.GetFullPath(path);

    var fileInfo = new FileInfo(fullPath);
    var etag = $"W/\"{fileInfo.Length:x}-{fileInfo.LastWriteTime.Ticks:x}\"";
    httpContext.Response.Headers.ETag = etag;
    httpContext.Response.Headers.CacheControl = "public; max-age=31536000, immutable";
    httpContext.Response.ContentType = "application/octet-stream";

    return Results.File(fullPath, "application/octet-stream",
        fileDownloadName: file,
        enableRangeProcessing: true);
});

/// <summary>
/// Upload new package version.
/// </summary>
/// <param name="pkg"></param>
/// <param name="request"></param>
app.MapPut("/{pkg}", async (string pkg, PublishRequest request, IHostEnvironment env, IUnitOfWork unitOfWork, CancellationToken cancellationToken) =>
{
    var pathName = Uri.UnescapeDataString(pkg); // @packmule/packmule-custom-package
    var tarballRoot = Path.Combine(env.ContentRootPath, "tarballs");
    if (!Directory.Exists(tarballRoot))
        Directory.CreateDirectory(tarballRoot);

    var packagePath = Path.Combine(tarballRoot, pkg); // tarballs/%40packmule%2Fpackagemle-custom-package
    if (!Directory.Exists(packagePath))
        Directory.CreateDirectory(packagePath);

    var versionPath = request.Attachments.First().Key;
    if (versionPath.Contains("/"))
        versionPath = versionPath[(versionPath.LastIndexOf('/') + 1)..];

    var fullPath = Path.Combine(packagePath, versionPath); // tarballs/%40packmule%2Fpackmule-custom-package/packmule-custom-package-1.0.0.tgz
    var tarballData = request.Attachments.First().Value.Data;

    await File.WriteAllBytesAsync(fullPath, tarballData);
    var tarballUrl = $"http://localhost:5182/{Uri.EscapeDataString(pkg)}/-/{versionPath}";

    var package = await unitOfWork.PackageRepository.GetPackageAsync(pathName);
    if (package is null)
    {
        package = await unitOfWork.PackageRepository.CreatePackageAsync(pathName, cancellationToken);
    }

    var version = request.Versions.First();

    using var sha = SHA512.Create();
    using var stream = new MemoryStream(tarballData);
    var hash = await sha.ComputeHashAsync(stream);
    var integrity = "sha512-" + Convert.ToBase64String(hash);

    version.Value.Dist.Integrity = integrity;
    version.Value.Dist.Tarball = tarballUrl;

    var versionManifest = JsonSerializer.Serialize(version, options: new(JsonSerializerDefaults.Web));

    var packageVersion = await unitOfWork.PackageRepository.CreatePackageVersionAsync(
        package.Id,
        version.Key,
        versionManifest,
        tarballUrl,
        integrity,
        cancellationToken);

    await unitOfWork.PackageRepository.UpsertDistTagAsync(package.Id, "latest", version.Key, cancellationToken);

    await unitOfWork.SaveAsync(cancellationToken);
});

app.Run();
