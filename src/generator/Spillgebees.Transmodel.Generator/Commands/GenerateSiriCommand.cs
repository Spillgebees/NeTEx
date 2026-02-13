using System.CommandLine;
using Spillgebees.Transmodel.Generator.Configuration;
using Spillgebees.Transmodel.Generator.Services;

namespace Spillgebees.Transmodel.Generator.Commands;

public static class GenerateSiriCommand
{
    public static Command Create()
    {
        var versionOption = new Option<string>("--version", "-v")
        {
            Description = "SIRI schema version tag (e.g., v2.2)",
            DefaultValueFactory = _ => SiriDefaults.DefaultVersion,
        };

        var refOption = new Option<string?>("--ref")
        {
            Description = "Git ref (branch name or commit SHA). Mutually exclusive with --version.",
        };

        var outputOption = new Option<string>("--output", "-o")
        {
            Description = "Output directory for generated C# files",
            DefaultValueFactory = _ => SharedDefaults.DefaultOutputDirectory,
        };

        var namespaceOption = new Option<string>("--namespace", "-n")
        {
            Description = "Root C# namespace. Sub-namespaces .SIRI, .IFOPT, .ACSB, .DATEX2, .WSDL, .GML are appended automatically.",
            DefaultValueFactory = _ => SiriDefaults.DefaultNamespace,
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

        var command = new Command("generate-siri", "Generate C# XML bindings from SIRI XSD schemas");
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
                    SiriDefaults.GitHubArchiveUrlTemplate,
                    SiriDefaults.GitHubArchiveRefUrlTemplate,
                    SiriDefaults.CacheDirectoryName,
                    logVerboseLine,
                    cancellationToken);

                logVerboseLine?.Invoke("Generating C# XML bindings...");
                logVerboseLine?.Invoke($"  Output:     {Path.GetFullPath(output)}");
                logVerboseLine?.Invoke($"  Namespace:  {rootNamespace}");
                logVerboseLine?.Invoke("  Sub-namespaces: .SIRI, .IFOPT, .ACSB, .DATEX2, .WSDL, .GML");

                CodeGenerator.GenerateSiri(schemaDir.XsdPath, output, rootNamespace, verbose);

                Console.WriteLine($"Generated SIRI {versionOrRef} models in {Path.GetFullPath(output)}");
                logVerboseLine?.Invoke($"  SIRI:   {rootNamespace}.{SiriDefaults.SiriSubNamespace}");
                logVerboseLine?.Invoke($"  IFOPT:  {rootNamespace}.{SiriDefaults.IfoptSubNamespace}");
                logVerboseLine?.Invoke($"  ACSB:   {rootNamespace}.{SiriDefaults.AcsbSubNamespace}");
                logVerboseLine?.Invoke($"  DATEX2: {rootNamespace}.{SiriDefaults.Datex2SubNamespace}");
                logVerboseLine?.Invoke($"  WSDL:   {rootNamespace}.{SiriDefaults.WsdlSubNamespace}");
                logVerboseLine?.Invoke($"  GML:    {rootNamespace}.{SharedDefaults.GmlSubNamespace}");
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
