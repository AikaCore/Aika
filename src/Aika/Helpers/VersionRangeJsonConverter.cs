using NuGet.Versioning;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Aika.Helpers;

internal class VersionRangeJsonConverter : JsonConverter<VersionRange>
{
    public override VersionRange? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var versionString = reader.GetString();
        
        if (versionString == null)
            return null;

        if (VersionRange.TryParse(versionString, out var range))
            return range;

        throw new JsonException($"Invalid version range format: {versionString}");
    }

    public override void Write(Utf8JsonWriter writer, VersionRange value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
