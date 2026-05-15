using System.Collections;

using Xunit.Sdk;

namespace Gobo.Tests;

/// <summary>
/// These tests ensure that the formatter does not throw errors
/// on large files and does not change its output on a second pass.
/// </summary>
public class SampleTests
{
    public const string TestFileExtension = ".test";
    public const string ActualFileExtension = ".actual";

    [Theory]
    [ClassData(typeof(SampleFileProvider))]
    public async Task FormatSamples(TestFile test)
    {
        string filePath = test.FilePath;

        string input = await File.ReadAllTextAsync(filePath);

        FormatResult firstPass = GmlFormatter.Format(input, test.Options);

        FormatResult secondPass = GmlFormatter.Format(firstPass.Output, test.Options);

        string secondDiff = StringDiffer.PrintFirstDifference(firstPass.Output, secondPass.Output);
        if (secondDiff != string.Empty)
        {
            await File.WriteAllTextAsync(
                filePath.Replace(TestFileExtension, ActualFileExtension),
                firstPass.Output
            );
            throw new XunitException($"Second pass instability for '{test.Name}':\n{secondDiff}");
        }
    }
}

public class SampleFileProvider : IEnumerable<object[]>
{
    private readonly DirectoryInfo rootDirectory = DirectoryFinder.FindParent("Gobo.Tests");

    public SampleFileProvider() { }

    public IEnumerator<object[]> GetEnumerator()
    {
        string directoryPath = Path.Combine(rootDirectory.FullName, "Gml", "Samples");
        FormatOptions options = ConfigFileHandler.FindConfigOrDefault(directoryPath);
        IEnumerable<string> files = Directory.EnumerateFiles(directoryPath, $"*{SampleTests.TestFileExtension}");
        return files.Select(fp => new object[] { new TestFile(fp, options) }).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
