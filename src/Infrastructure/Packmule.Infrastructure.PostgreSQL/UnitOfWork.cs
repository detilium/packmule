using System;
using Packmule.Core.Interfaces;

namespace Packmule.Infrastructure.PostgreSQL;

public class UnitOfWork : IUnitOfWork
{
	private readonly DatabaseContext context;

	public IPackageRepository PackageRepository { get; }

	public UnitOfWork(
		DatabaseContext context,
		IPackageRepository packageRepository)
	{
		this.context = context;
		PackageRepository = packageRepository;
	}

	public async Task<int> SaveAsync(CancellationToken cancellationToken)
		=> await context.SaveChangesAsync(cancellationToken);

	async ValueTask IAsyncDisposable.DisposeAsync()
	{
		await context.DisposeAsync();
	}
}
