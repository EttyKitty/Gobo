namespace Gobo.Printer;

internal class DocPrinterOptions
{
    public bool UseTabs { get; init; } = false;
    public int TabWidth { get; init; } = 4;
    public int MaxLineWidth { get; init; } = -1;
    public bool TrimInitialLines { get; init; } = true;

    public const int WidthUsedByTests = 100;
}
