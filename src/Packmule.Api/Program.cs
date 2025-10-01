using Packmule.Api.Documents;
using Packmule.Core.Interfaces;
using Packmule.Infrastructure.PostgreSQL.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddPostgreSQL(builder.Configuration.GetConnectionString("DatabaseContext"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/{packageName}", async (string packageName, IUnitOfWork unitOfWork, CancellationToken cancellationToken) =>
{
    var package = await unitOfWork.PackageRepository.GetPackageAsync(packageName, cancellationToken);
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

app.Run();
