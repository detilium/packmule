using System.Text.Json;
using System.Text.Json.Serialization;

namespace Packmule.Api.Documents;

public record PackageDocument
{
	[JsonPropertyName("_id")]
    public string Id { get; set; }
    
	public string Name { get; set; }
	
	[JsonPropertyName("dist-tags")]
    public Dictionary<string, string> DistTags { get; set; }

    public Dictionary<string, JsonDocument> Versions { get; set; }

    public Dictionary<string, DateTimeOffset> Time { get; set; }
}