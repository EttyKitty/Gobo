using System.Collections;

using Xunit.Sdk;

namespace Gobo.Tests;

/// <summary>
/// General purpose formatting tests to avoid regressions
/// </summary>
public class FormattingTests
{
    public const string TestFileExtension = ".test";
    public const string ExpectedFileExtension = ".expected";
    public const string ActualFileExtension = ".actual";

    [Theory]
    [ClassData(typeof(FormattingTestProvider))]
    public async Task FormatTests(TestFile test)
    {
        string testFilePath = test.FilePath;
        string expectedFilePath = testFilePath.Replace(TestFileExtension, ExpectedFileExtension);
        string actualFilePath = testFilePath.Replace(TestFileExtension, ActualFileExtension);

        if (!Path.Exists(testFilePath))
        {
            throw new XunitException($"Test file {testFilePath} does not exist!");
        }

        if (!Path.Exists(expectedFilePath))
        {
            throw new XunitException($"Expected test file {expectedFilePath} does not exist!");
        }

        string input = await File.ReadAllTextAsync(testFilePath);

        FormatResult firstPass = GmlFormatter.Format(input, test.Options);

        string expectedOutput = (await File.ReadAllTextAsync(expectedFilePath)).ReplaceLineEndings(
            "\n"
        );

        string firstDiff = StringDiffer.PrintFirstDifference(expectedOutput, firstPass.Output);
        if (firstDiff != string.Empty)
        {
            await File.WriteAllTextAsync(actualFilePath, firstPass.Output);
            throw new XunitException($"Formatting error on first pass for '{test.Name}':\n{firstDiff}");
        }

        FormatResult secondPass = GmlFormatter.Format(firstPass.Output, test.Options);

        string secondDiff = StringDiffer.PrintFirstDifference(expectedOutput, secondPass.Output);
        if (secondDiff != string.Empty)
        {
            await File.WriteAllTextAsync(actualFilePath, firstPass.Output);
            throw new XunitException($"Formatting error on second pass for '{test.Name}':\n{secondDiff}");
        }
    }
}

public class FormattingTestProvider : IEnumerable<object[]>
{
    private readonly DirectoryInfo rootDirectory = DirectoryFinder.FindParent("Gobo.Tests");

    public FormattingTestProvider() { }

    public IEnumerator<object[]> GetEnumerator()
    {
        string directoryPath = Path.Combine(rootDirectory.FullName, "Gml", "FormattingTests");
        FormatOptions options = ConfigFileHandler.FindConfigOrDefault(directoryPath);
        IEnumerable<string> files = Directory.EnumerateFiles(directoryPath, $"*{SampleTests.TestFileExtension}");
        return files.Select(fp => new object[] { new TestFile(fp, options) }).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
