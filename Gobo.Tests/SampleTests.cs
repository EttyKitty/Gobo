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
        var filePath = test.FilePath;

        var input = await File.ReadAllTextAsync(filePath);

        var firstPass = GmlFormatter.Format(input, test.Options);

        var secondPass = GmlFormatter.Format(firstPass.Output, test.Options);

        var secondDiff = StringDiffer.PrintFirstDifference(firstPass.Output, secondPass.Output);
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
        var directoryPath = Path.Combine(rootDirectory.FullName, "Gml", "Samples");
        var options = ConfigFileHandler.FindConfigOrDefault(directoryPath);
        var files = Directory.EnumerateFiles(directoryPath, $"*{SampleTests.TestFileExtension}");
        return files.Select(fp => new object[] { new TestFile(fp, options) }).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
