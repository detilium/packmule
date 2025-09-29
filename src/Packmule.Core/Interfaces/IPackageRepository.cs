using Packmule.Core.Models;

namespace Packmule.Core.Interfaces;

public interface IPackageRepository
{
	Task<List<Package>> GetPackagesAsync(int pageNumber = 0, int pageSize = 20);
	Task<Package> GetPackageAsync(Guid packageId);
	Task<Guid> CreatePackageAsync(string packageName);

	Task<List<PackageVersion>> GetPackageVersionsAsync(Guid packageId);
	Task<PackageVersion> GetPackageVersionAsync(Guid packageVersionId);
	Task<Guid> CreatePackageVersionAsync(Guid packageId, string version, string manifest, string tarballUrl, string integrity);
	Task DeprecatePackageVersionAsync(Guid packageVersionId);

	Task<List<DistTag>> GetPackageDistTagsAsync(Guid packageId);
	Task UpdateDistTagAsync(Guid packageId, string tag, string version);
}
