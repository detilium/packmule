namespace Packmule.Api.Documents;

public record class PackageDocument(
	string Name,
	IReadOnlyDictionary<string, string> DistTags,
	IReadOnlyDictionary<string, PackageVersionDocument> Versions,
	IReadOnlyDictionary<string, DateTimeOffset> Time,
	DateTimeOffset CreatedAt,
	DateTimeOffset ModifiedAt
);
