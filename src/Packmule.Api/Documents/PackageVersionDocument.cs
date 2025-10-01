namespace Packmule.Api.Documents;

public record class PackageVersionDocument(
	string Version,
	string Manifest,
	string TarballUri,
	string Integrity,
	string? Deprecation,
	DateTimeOffset CreatedAt
);
