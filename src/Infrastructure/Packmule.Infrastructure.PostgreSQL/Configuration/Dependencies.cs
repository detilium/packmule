using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Packmule.Core.Interfaces;

namespace Packmule.Infrastructure.PostgreSQL.Configuration;

public static class Dependencies
{
	public static IServiceCollection AddPostgreSQL(this IServiceCollection services, string connectionString)
	{
		services.AddDbContextPool<DatabaseContext>(options => options
			.UseNpgsql(connectionString));

		services.AddScoped<IPackageRepository, PackageRepository>();
		services.AddScoped<IUnitOfWork, UnitOfWork>();

		return services;
	}
}
