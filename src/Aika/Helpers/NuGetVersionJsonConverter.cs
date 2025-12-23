using NuGet.Versioning;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Aika.Helpers;

internal class NuGetVersionJsonConverter : JsonConverter<NuGetVersion>
{
    public override NuGetVersion? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var versionString = reader.GetString();
        if (versionString == null)
            return null;

        if (NuGetVersion.TryParse(versionString, out var version))
            return version;

        throw new JsonException($"Invalid semantic version format: {versionString}");
    }

    public override void Write(Utf8JsonWriter writer, NuGetVersion value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
