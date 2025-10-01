using Packmule.Core.Models;

namespace Packmule.Core.Interfaces;

public interface IPackageRepository
{
	Task<IReadOnlyList<Package>> GetPackagesAsync(int pageNumber = 0, int pageSize = 20, CancellationToken cancellationToken = default);
	Task<Package?> GetPackageAsync(string name, CancellationToken cancellationToken = default);
	Task<Package?> GetPackageAsync(Guid packageId, CancellationToken cancellationToken = default);
	Task<Guid> CreatePackageAsync(string packageName, CancellationToken cancellationToken = default);

	Task<IReadOnlyList<PackageVersion>> GetPackageVersionsAsync(Guid packageId, CancellationToken cancellationToken = default);
	Task<PackageVersion?> GetPackageVersionAsync(Guid packageVersionId, CancellationToken cancellationToken = default);
	Task<Guid> CreatePackageVersionAsync(Guid packageId, string version, string manifestJson, string tarballUrl, string integrity, CancellationToken cancellationToken = default);
	Task DeprecatePackageVersionAsync(Guid packageVersionId, string? deprecation, CancellationToken cancellationToken = default);

	Task<IReadOnlyList<DistTag>> GetPackageDistTagsAsync(Guid packageId, CancellationToken cancellationToken = default);
	Task UpsertDistTagAsync(Guid packageId, string tag, string version, CancellationToken cancellationToken = default);
	Task RemoveDistTagAsync(Guid packageId, string tag, CancellationToken cancellationToken = default);
}
