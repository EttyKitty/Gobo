using System.Collections.Concurrent;
using System.Diagnostics;

using DocoptNet;

using Gobo;
using Gobo.Text;

const string usage =
    @"Usage:
  gobo [options] <file-or-directory>

Options:
  -h --help       Show this screen.
  -v --version    Show version.
  --check         Check that the files are formatted. Will not write any changes.
  --fast          Skip validation of formatted syntax tree and comments.
  --write-stdout  Write the results of formatting any files to stdout.
  --skip-write    Skip writing changes. Used for testing to ensure Gobo doesn't throw any errors.
";

return await Docopt
    .CreateParser(usage)
    .WithVersion("Gobo 0.3.0")
    .Parse(args)
    .Match(
        Run,
        result => ShowHelp(result.Help),
        result => ShowVersion(result.Version),
        result => OnError(result.Usage)
    );

static Task<int> ShowHelp(string help)
{
    Console.WriteLine(help);
    return Task.FromResult(0);
}

static Task<int> ShowVersion(string version)
{
    Console.WriteLine(version);
    return Task.FromResult(0);
}

static Task<int> OnError(string usage)
{
    Console.Error.WriteLine(usage);
    return Task.FromResult(1);
}

static async Task<int> Run(IDictionary<string, ArgValue> arguments)
{
    string filePath = arguments["<file-or-directory>"].ToString();

    if (!Path.Exists(filePath))
    {
        Console.Error.WriteLine($"{filePath} is not a valid path.");
        return 1;
    }

    bool isCheckMode = arguments["--check"].IsTrue;
    FormatOptions options = ConfigFileHandler.FindConfigOrDefault(filePath);
    const string GmlExtension = ".gml";

    int failureCount = 0;

    if (File.Exists(filePath))
    {
        if (!Path.GetExtension(filePath).Equals(GmlExtension, StringComparison.OrdinalIgnoreCase))
        {
            Console.Error.WriteLine($"{filePath} is not a *.gml file.");
            return 1;
        }

        Console.WriteLine($"{(isCheckMode ? "Checking" : "Formatting")} {Path.GetFileName(filePath)}...");

        var stopWatch = Stopwatch.StartNew();
        bool success = await ProcessFile(filePath, options, arguments, isCheckMode);
        if (!success)
        {
            failureCount++;
        }

        Console.WriteLine($"Done in {stopWatch.ElapsedMilliseconds} ms");
    }
    else if (Directory.Exists(filePath))
    {
        string[] ignorePatterns = ["node_modules", "extensions", ".git", ".svn", "prefabs", "bin", "obj"];
        var files = new DirectoryInfo(filePath)
            .EnumerateFiles($"*{GmlExtension}", SearchOption.AllDirectories)
            .Where(f => !Path.GetRelativePath(filePath, f.FullName)
                .Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar])
                .Any(segment => ignorePatterns.Contains(segment, StringComparer.OrdinalIgnoreCase)))
            .ToList();

        if (files.Count == 0)
        {
            Console.WriteLine($"No *{GmlExtension} files found in {filePath}");
            return 0;
        }

        Console.WriteLine($"{(isCheckMode ? "Checking" : "Formatting")} {files.Count} files...");
        var stopWatch = Stopwatch.StartNew();

        var concurrentFailures = new ConcurrentBag<string>();

        await Parallel.ForEachAsync(files, async (file, ct) =>
        {
            bool success = await ProcessFile(file.FullName, options, arguments, isCheckMode);
            if (!success)
            {
                concurrentFailures.Add(file.FullName);
            }
        });

        stopWatch.Stop();
        failureCount = concurrentFailures.Count;

        Console.WriteLine($"{(isCheckMode ? "Checked" : "Formatted")} {files.Count} files in {stopWatch.Elapsed:s\\.fff} seconds");
    }

    return (isCheckMode && failureCount > 0) ? 1 : 0;
}

static async Task<bool> ProcessFile(
    string filePath,
    FormatOptions options,
    IDictionary<string, ArgValue> arguments,
    bool isCheckMode)
{
    if (isCheckMode)
    {
        return await CheckFile(filePath, options, arguments);
    }
    else
    {
        await FormatFile(filePath, options, arguments);
        return true;
    }
}

static async Task FormatFile(
    string filePath,
    FormatOptions options,
    IDictionary<string, ArgValue> arguments)
{
    try
    {
        string input = await File.ReadAllTextAsync(filePath);
        options.ValidateOutput = arguments["--fast"].IsFalse;

        FormatResult result = GmlFormatter.Format(input, options);

        if (arguments["--skip-write"].IsFalse)
        {
            await File.WriteAllTextAsync(filePath, result.Output);
        }

        if (arguments["--write-stdout"].IsTrue)
        {
            Console.WriteLine(result.Output);
        }
    }
    catch (Exception e)
    {
        Console.Error.WriteLine($"[Error] {filePath}: {e.Message}");
    }
}

static async Task<bool> CheckFile(
    string filePath,
    FormatOptions options,
    IDictionary<string, ArgValue> arguments)
{
    try
    {
        string text = await File.ReadAllTextAsync(filePath);
        var input = SourceText.From(text);
        bool isFormatted = GmlFormatter.Check(input, options);

        if (!isFormatted)
        {
            Console.WriteLine($"[Warn] {filePath} is not formatted.");
            return false;
        }

        return true;
    }
    catch (Exception e)
    {
        Console.Error.WriteLine($"[Error] {filePath}: {e.Message}");
        return false;
    }
}
