namespace Packmule.Core.Models;

public class Package
{
	public Guid Id { get; set; }
	public required string Name { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime ModifiedAt { get; set; }
}
