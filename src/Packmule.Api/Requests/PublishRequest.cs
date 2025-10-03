using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Packmule.Api.Requests;

public record PublishRequest
{
	[JsonPropertyName("_id")]
	public required string Id { get; init; }
	public required string Name { get; init; }
	public required string Description { get; init; }
	public Dictionary<string, string>? DistTags { get; init; }
	public Dictionary<string, PublishRequestVersionManifest> Versions { get; set; }
	public object? Access { get; set; } // Unknown what this is.
	[JsonPropertyName("_attachments")]
	public Dictionary<string, PublishRequestAttachment> Attachments { get; set; }
}

public record PublishRequestAttachment
{
	public string ContentType { get; set; }
	public byte[] Data { get; set; }
	public int Length { get; set; }
}

public record PublishRequestVersionManifest
{
	public string Name { get; set; }
	public string Version { get; set; }
	public PublishRequestVersionDist Dist { get; set; }
	public string? Deprecated { get; set; }

	[JsonExtensionData]
	public Dictionary<string, JsonElement> Extra { get; set; }
}

public record PublishRequestVersionDist
{
	public string? Integrity { get; set; }
	[JsonPropertyName("shasum")]
	public string ShaSum { get; set; }
	public string? Tarball { get; set; }
}
