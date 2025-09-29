namespace Packmule.Core.Models;

public class PackageVersion
{
	public Guid Id { get; set; }
	public Guid PackageId { get; set; }
	public required string Version { get; set; }
	public required string Manifest { get; set; }
	public required string TarballUri { get; set; }
	public required string Integrity { get; set; }
	public DateTime CreatedAt { get; set; }
}
