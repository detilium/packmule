using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Packmule.Core.Exceptions;
using Packmule.Core.Interfaces;
using Packmule.Core.Models;
using Packmule.Infrastructure.PostgreSQL.Entities;

namespace Packmule.Infrastructure.PostgreSQL;

public class PackageRepository : IPackageRepository
{
	private readonly DatabaseContext context;

	public PackageRepository(DatabaseContext context)
	{
		this.context = context;
	}

	public async Task<Package> CreatePackageAsync(string packageName, CancellationToken cancellationToken = default)
	{
		var package = new PackageEntity()
		{
			Name = packageName,
			CreatedAt = DateTime.UtcNow,
			ModifiedAt = DateTime.UtcNow
		};

		await context.Packages.AddAsync(package);

		var result = new Package()
		{
			Id = package.Id,
			Name = package.Name,
			CreatedAt = package.CreatedAt,
			ModifiedAt = package.ModifiedAt
		};

		return result;
	}

	public async Task<PackageVersion> CreatePackageVersionAsync(Guid packageId, string version, string manifestJson, string tarballUrl, string integrity, CancellationToken cancellationToken = default)
	{
		var packageVersion = new PackageVersionEntity()
		{
			Version = version,
			Manifest = manifestJson,
			TarballUri = tarballUrl,
			Integrity = integrity,
			CreatedAt = DateTime.UtcNow,
			PackageId = packageId
		};

		await context.PackageVersions.AddAsync(packageVersion);

		var result = new PackageVersion()
		{
			Id = packageVersion.Id,
			Version = packageVersion.Version,
			Manifest = packageVersion.Manifest,
			TarballUri = packageVersion.TarballUri,
			Integrity = packageVersion.Integrity,
			CreatedAt = packageVersion.CreatedAt,
			PackageId = packageVersion.PackageId
		};

		return result;
	}

	public async Task DeprecatePackageVersionAsync(Guid packageVersionId, string? deprecation, CancellationToken cancellationToken = default)
	{
		var packageVersion = await context.PackageVersions.FirstOrDefaultAsync(x => x.Id == packageVersionId);
		if (packageVersion is null)
		{
			throw new NotFoundException("Package version could not be found.");
		}

		packageVersion.Deprecation = deprecation;
	}

	public async Task<Package?> GetPackageAsync(string name, CancellationToken cancellationToken = default)
	{
		var normalizedName = name.ToLowerInvariant();

		return await context.Packages
			.AsNoTracking()
			.Where(x => x.NameLower == normalizedName)
			.Select(x => new Package()
			{
				Id = x.Id,
				Name = x.Name,
				CreatedAt = x.CreatedAt,
				ModifiedAt = x.ModifiedAt,
				Versions = x.PackageVersions
					.OrderBy(v => v.CreatedAt)
					.Select(v => new PackageVersion()
					{
						Id = v.Id,
						PackageId = v.PackageId,
						Version = v.Version,
						Manifest = v.Manifest,
						TarballUri = v.TarballUri,
						Integrity = v.Integrity,
						Deprecation = v.Deprecation,
						CreatedAt = v.CreatedAt
					})
					.ToList(),
				DistTags = x.DistTags
					.Select(d => new DistTag()
					{
						PackageId = d.PackageId,
						Tag = d.Tag,
						Version = d.Version
					})
					.ToList()
			})
			.SingleOrDefaultAsync(cancellationToken);
	}

	public async Task<Package?> GetPackageAsync(Guid packageId, CancellationToken cancellationToken = default)
	{
		return await context.Packages
			.AsNoTracking()
			.Where(x => x.Id == packageId)
			.Select(x => new Package()
			{
				Id = x.Id,
				Name = x.Name,
				CreatedAt = x.CreatedAt,
				ModifiedAt = x.ModifiedAt,
				Versions = x.PackageVersions
					.OrderBy(v => v.CreatedAt)
					.Select(v => new PackageVersion()
					{
						Id = v.Id,
						PackageId = v.PackageId,
						Version = v.Version,
						Manifest = v.Manifest,
						TarballUri = v.TarballUri,
						Integrity = v.Integrity,
						Deprecation = v.Deprecation,
						CreatedAt = v.CreatedAt
					})
					.ToList(),
				DistTags = x.DistTags
					.Select(d => new DistTag()
					{
						PackageId = d.PackageId,
						Tag = d.Tag,
						Version = d.Version
					})
					.ToList()
			})
			.SingleOrDefaultAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<DistTag>> GetPackageDistTagsAsync(Guid packageId, CancellationToken cancellationToken = default)
	{
		return await context.DistTags
			.AsNoTracking()
			.Where(x => x.PackageId == packageId)
			.Select(x => new DistTag()
			{
				PackageId = x.PackageId,
				Tag = x.Tag,
				Version = x.Version
			})
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<Package>> GetPackagesAsync(int pageNumber = 0, int pageSize = 20, CancellationToken cancellationToken = default)
	{
		return await context.Packages
			.AsNoTracking()
			.Skip(pageNumber * pageSize)
			.Take(pageSize)
			.Select(x => new Package()
			{
				Id = x.Id,
				Name = x.Name,
				CreatedAt = x.CreatedAt,
				ModifiedAt = x.ModifiedAt,
				Versions = x.PackageVersions
					.OrderBy(v => v.CreatedAt)
					.Select(v => new PackageVersion()
					{
						Id = v.Id,
						PackageId = v.PackageId,
						Version = v.Version,
						Manifest = v.Manifest,
						TarballUri = v.TarballUri,
						Integrity = v.Integrity,
						Deprecation = v.Deprecation,
						CreatedAt = v.CreatedAt
					})
					.ToList(),
				DistTags = x.DistTags
					.Select(d => new DistTag()
					{
						PackageId = d.PackageId,
						Tag = d.Tag,
						Version = d.Version
					})
					.ToList()
			})
			.ToListAsync(cancellationToken);
	}

	public async Task<PackageVersion?> GetPackageVersionAsync(Guid packageVersionId, CancellationToken cancellationToken = default)
	{
		return await context.PackageVersions
			.AsNoTracking()
			.Where(x => x.Id == packageVersionId)
			.Select(x => new PackageVersion()
			{
				Id = x.Id,
				PackageId = x.PackageId,
				Version = x.Version,
				Manifest = x.Manifest,
				TarballUri = x.TarballUri,
				Integrity = x.Integrity,
				Deprecation = x.Deprecation,
				CreatedAt = x.CreatedAt
			})
			.SingleOrDefaultAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<PackageVersion>> GetPackageVersionsAsync(Guid packageId, CancellationToken cancellationToken = default)
	{

		return await context.PackageVersions
			.AsNoTracking()
			.Where(x => x.PackageId == packageId)
			.Select(x => new PackageVersion()
			{
				Id = x.Id,
				PackageId = x.PackageId,
				Version = x.Version,
				Manifest = x.Manifest,
				TarballUri = x.TarballUri,
				Integrity = x.Integrity,
				Deprecation = x.Deprecation,
				CreatedAt = x.CreatedAt
			})
			.ToListAsync(cancellationToken);
	}

	public async Task RemoveDistTagAsync(Guid packageId, string tag, CancellationToken cancellationToken = default)
	{
		tag = tag.Trim();

		var entity = await context.DistTags
			.SingleOrDefaultAsync(x => x.PackageId == packageId && x.Tag == tag);

		if (entity is null)
			return;

		context.DistTags.Remove(entity);
	}

	public async Task<DistTag> UpsertDistTagAsync(Guid packageId, string tag, string version, CancellationToken cancellationToken = default)
	{
		tag = tag.Trim();

		var existingTag = await context.DistTags
			.SingleOrDefaultAsync(x => x.PackageId == packageId && x.Tag == tag);

		if (existingTag is null)
		{
			var newTag = new DistTagEntity()
			{
				PackageId = packageId,
				Tag = tag,
				Version = version
			};
			await context.DistTags.AddAsync(newTag);
		}
		else if (!version.Equals(existingTag.Version, StringComparison.InvariantCultureIgnoreCase))
		{
			existingTag.Version = version;
		}

		var result = new DistTag()
		{
			Tag = tag,
			Version = version
		};

		return result;
	}
}
