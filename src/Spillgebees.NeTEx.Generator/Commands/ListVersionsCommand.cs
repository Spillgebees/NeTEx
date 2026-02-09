using System.CommandLine;
using Spillgebees.NeTEx.Generator.Services;

namespace Spillgebees.NeTEx.Generator.Commands;

public static class ListVersionsCommand
{
    public static Command Create()
    {
        var command = new Command("list-versions", "List available NeTEx schema versions from the official repository");

        command.SetAction(async (_, cancellationToken) =>
        {
            try
            {
                Console.WriteLine("Fetching available NeTEx schema versions...");
                Console.WriteLine();

                var versions = await SchemaDownloader.ListVersionsAsync(cancellationToken);

                if (versions.Count == 0)
                {
                    Console.WriteLine("No versions found.");
                    return;
                }

                Console.WriteLine("Available versions:");
                foreach (var version in versions)
                {
                    Console.WriteLine($"  {version}");
                }

                Console.WriteLine();
                Console.WriteLine($"Total: {versions.Count} version(s)");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error fetching versions: {ex.Message}");
                Environment.ExitCode = 1;
            }
        });

        return command;
    }
}
