using System.CommandLine;
using Spillgebees.NeTEx.Generator.Configuration;
using Spillgebees.NeTEx.Generator.Services;

namespace Spillgebees.NeTEx.Generator.Commands;

public static class GenerateCommand
{
    public static Command Create()
    {
        var versionOption = new Option<string>("--version", "-v")
        {
            Description = "NeTEx schema version tag (e.g., v1.3.1, v1.2)",
            DefaultValueFactory = _ => GeneratorDefaults.DefaultVersion,
        };

        var refOption = new Option<string?>("--ref")
        {
            Description = "Git ref (branch name or commit SHA). Mutually exclusive with --version.",
        };

        var outputOption = new Option<string>("--output", "-o")
        {
            Description = "Output directory for generated C# files",
            DefaultValueFactory = _ => GeneratorDefaults.DefaultOutputDirectory,
        };

        var namespaceOption = new Option<string>("--namespace", "-n")
        {
            Description = "Root C# namespace. Sub-namespaces .NeTEx, .SIRI, .GML are appended automatically.",
            DefaultValueFactory = _ => GeneratorDefaults.DefaultNamespace,
        };

        var cleanOption = new Option<bool>("--clean")
        {
            Description = "Delete output directory before generating",
            DefaultValueFactory = _ => false,
        };

        var verboseOption = new Option<bool>("--verbose")
        {
            Description = "Enable verbose logging",
            DefaultValueFactory = _ => false,
        };

        var command = new Command("generate", "Generate C# model classes from NeTEx XSD schemas");
        command.Options.Add(versionOption);
        command.Options.Add(refOption);
        command.Options.Add(outputOption);
        command.Options.Add(namespaceOption);
        command.Options.Add(cleanOption);
        command.Options.Add(verboseOption);

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            var version = parseResult.GetValue(versionOption)!;
            var gitRef = parseResult.GetValue(refOption);
            var output = parseResult.GetValue(outputOption)!;
            var rootNamespace = parseResult.GetValue(namespaceOption)!;
            var clean = parseResult.GetValue(cleanOption);
            var verbose = parseResult.GetValue(verboseOption);

            if (gitRef is not null && parseResult.GetResult(versionOption) is { Tokens.Count: > 0 })
            {
                await Console.Error.WriteLineAsync("Error: --version and --ref are mutually exclusive. Specify only one.");
                Environment.ExitCode = 1;
                return;
            }

            Action<string>? logVerboseLine = verbose ? Console.WriteLine : null;

            var isTag = gitRef is null;
            var versionOrRef = gitRef ?? version;

            try
            {
                if (clean && Directory.Exists(output))
                {
                    logVerboseLine?.Invoke($"Cleaning output directory: {output}");

                    try
                    {
                        Directory.Delete(output, recursive: true);
                    }
                    catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
                    {
                        throw new InvalidOperationException(
                            $"Failed to clean output directory '{output}': {ex.Message}", ex);
                    }
                }

                using var schemaDir = await SchemaDownloader.DownloadAndExtractAsync(
                    versionOrRef,
                    isTag,
                    logVerboseLine,
                    cancellationToken);

                logVerboseLine?.Invoke("Generating C# models...");
                logVerboseLine?.Invoke($"  Output:     {Path.GetFullPath(output)}");
                logVerboseLine?.Invoke($"  Namespace:  {rootNamespace}");
                logVerboseLine?.Invoke("  Sub-namespaces: .NeTEx, .SIRI, .GML");

                var result = CodeGenerator.Generate(schemaDir.XsdPath, output, rootNamespace, verbose);

                Console.WriteLine($"Generated NeTEx {versionOrRef} models in {Path.GetFullPath(output)}");
                logVerboseLine?.Invoke($"  NeTEx: {rootNamespace}.{GeneratorDefaults.NetexSubNamespace}");
                if (result.SiriGenerated)
                {
                    logVerboseLine?.Invoke($"  SIRI:  {rootNamespace}.{GeneratorDefaults.SiriSubNamespace}");
                }
                logVerboseLine?.Invoke($"  GML:   {rootNamespace}.{GeneratorDefaults.GmlSubNamespace}");
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync($"Error: {ex.Message}");
                if (verbose)
                {
                    await Console.Error.WriteLineAsync(ex.StackTrace);
                }

                Environment.ExitCode = 1;
            }
        });

        return command;
    }
}
