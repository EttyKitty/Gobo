using System.Text.Json.Serialization;

namespace Gobo;

public enum BraceStyle
{
    SameLine,
    NewLine,
}

public class FormatOptions
{
    public bool UseTabs { get; set; } = false;
    public int TabWidth { get; set; } = 4;
    public int MaxLineWidth { get; set; } = -1;
    public bool FlatExpressions { get; set; } = false;
    public bool VerticalStructs { get; set; } = true;

    [JsonIgnore]
    public BraceStyle BraceStyle { get; set; } = BraceStyle.SameLine;

    [JsonIgnore]
    public bool ValidateOutput { get; set; } = true;

    [JsonIgnore]
    public bool RemoveSyntaxExtensions { get; set; } = false;

    [JsonIgnore]
    public bool GetDebugInfo { get; set; } = false;

    public static FormatOptions DefaultTestOptions { get; } = new() { GetDebugInfo = true };

    public static FormatOptions Default { get; } = new();
}

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(FormatOptions))]
public partial class FormatOptionsSerializer : JsonSerializerContext { }
