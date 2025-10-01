using System;

namespace Packmule.Infrastructure.PostgreSQL.Entities;

public class DistTagEntity
{
	public Guid PackageId { get; set; }
	public required string Tag { get; set; }
	public required string Version { get; set; }

	public PackageEntity Package { get; set; }
}
