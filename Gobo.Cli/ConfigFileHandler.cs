using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Gobo.Cli;

public static class ConfigFileHandler
{
    private const string ConfigFileName = ".goborc.json";

    public static bool TryFindConfigFile(string filePath, [NotNullWhen(true)] out string? configFilePath)
    {
        string absolutePath = Path.GetFullPath(filePath);
        string? current = File.Exists(absolutePath) ? Path.GetDirectoryName(absolutePath) : absolutePath;

        while (current is not null)
        {
            string potentialConfigPath = Path.Combine(current, ConfigFileName);

            if (File.Exists(potentialConfigPath))
            {
                configFilePath = potentialConfigPath;
                return true;
            }

            current = Path.GetDirectoryName(current);
        }

        configFilePath = null;
        return false;
    }

    public static FormatOptions FindConfigOrDefault(string filePath)
    {
        if (!TryFindConfigFile(filePath, out string? configPath))
        {
            return FormatOptions.Default;
        }

        try
        {
            ReadOnlySpan<byte> data = File.ReadAllBytes(configPath);

            return JsonSerializer.Deserialize(data, FormatOptionsSerializer.Default.FormatOptions) ?? FormatOptions.Default;
        }
        catch (JsonException ex)
        {
            Console.Error.WriteLine(
                $"[Error] Failed to parse config at: {configPath}\n" +
                $"Reason: {ex.Message}\n" +
                "Falling back to default settings."
            );
            return FormatOptions.Default;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[Critical] Unexpected error reading config: {ex.Message}");
            throw;
        }
    }
}
