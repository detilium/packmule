using System;

namespace Packmule.Core.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
	IPackageRepository PackageRepository { get; }

	Task<int> SaveAsync(CancellationToken cancellationToken = default);
}
