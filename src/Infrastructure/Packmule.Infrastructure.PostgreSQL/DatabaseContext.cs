using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Packmule.Core.Models;
using Packmule.Infrastructure.PostgreSQL.Entities;

namespace Packmule.Infrastructure.PostgreSQL;

public class DatabaseContext : DbContext
{
	public DbSet<PackageEntity> Packages { get; set; }
	public DbSet<PackageVersionEntity> PackageVersions { get; set; }
	public DbSet<DistTagEntity> DistTags { get; set; }

	public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<PackageEntity>(p =>
		{
			p.HasKey(x => x.Id);
			p.HasIndex(x => x.NameLower).IsUnique();

			p.Property(x => x.Name)
				.HasMaxLength(250)
				.IsRequired();

			p.Property(x => x.NameLower)
				.HasConversion(v => v.ToLowerInvariant(), v => v)
				.HasMaxLength(250)
				.IsRequired();

			p.Property(x => x.CreatedAt)
				.ValueGeneratedOnAdd()
				.IsRequired();

			p.HasMany(p => p.PackageVersions)
				.WithOne(pv => pv.Package)
				.HasForeignKey(pv => pv.PackageId)
				.OnDelete(DeleteBehavior.Cascade);

			p.HasMany(p => p.DistTags)
				.WithOne(dt => dt.Package)
				.HasForeignKey(dt => dt.PackageId)
				.OnDelete(DeleteBehavior.Cascade);
		});

		modelBuilder.Entity<PackageVersionEntity>(pv =>
		{
			pv.HasKey(x => x.Id);

			pv.Property(x => x.Version)
				.HasMaxLength(25)
				.IsRequired();

			pv.Property(x => x.Manifest)
				.HasMaxLength(10000)
				.IsRequired();

			pv.Property(x => x.TarballUri)
				.HasMaxLength(500)
				.IsRequired();

			pv.Property(x => x.Integrity)
				.HasMaxLength(150)
				.IsRequired();

			pv.Property(x => x.Deprecation)
				.HasMaxLength(500)
				.IsRequired(false);

			pv.Property(x => x.CreatedAt)
				.ValueGeneratedOnAdd()
				.IsRequired();
		});

		modelBuilder.Entity<DistTagEntity>(dt =>
		{
			dt.HasKey(x => new { x.PackageId, x.Tag });

			dt.Property(x => x.Version)
				.HasMaxLength(25)
				.IsRequired();
		});
	}

	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		foreach (var entry in ChangeTracker.Entries<PackageEntity>())
		{
			if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
			{
				entry.Entity.NameLower = entry.Entity.Name.ToLowerInvariant();
			}
		}

		return base.SaveChangesAsync(cancellationToken);
	}
}
