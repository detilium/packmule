using System;

namespace Packmule.Infrastructure.PostgreSQL.Entities;

public class PackageEntity
{
	public Guid Id { get; set; }
	public required string Name { get; set; }
	public string NameLower { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime ModifiedAt { get; set; }

	public ICollection<PackageVersionEntity> PackageVersions { get; set; }
	public ICollection<DistTagEntity> DistTags { get; set; }
}
