using System;

namespace Packmule.Infrastructure.PostgreSQL.Entities;

public class PackageVersionEntity
{
	public Guid Id { get; set; }
	public required string Version { get; set; }
	public required string Manifest { get; set; }
	public required string TarballUri { get; set; }
	public required string Integrity { get; set; }
	public string? Deprecation { get; set; }
	public DateTime CreatedAt { get; set; }

	public Guid PackageId { get; set; }
	public PackageEntity Package { get; set; }
}
