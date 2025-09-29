namespace Packmule.Core.Models;

public class DistTag
{
	public Guid PackageId { get; set; }
	public required string Tag { get; set; }
	public required string Version { get; set; }
}
