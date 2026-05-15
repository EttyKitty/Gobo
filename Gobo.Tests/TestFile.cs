namespace Gobo.Tests;

public class TestFile
{
    public string FilePath;
    public string Name;
    public FormatOptions Options;

    public TestFile(string filePath, FormatOptions? options = null)
    {
        FilePath = filePath;
        Name = Path.GetFileName(filePath);
        Options = options ?? FormatOptions.DefaultTestOptions;
    }

    public override string ToString() => Name;
}
