using System.CommandLine;
using Spillgebees.Transmodel.Generator.Commands;

var rootCommand = new RootCommand(
    "Spillgebees Transmodel Generator - Generate C# XML bindings from NeTEx and SIRI XSD schemas");

rootCommand.Subcommands.Add(GenerateNetexCommand.Create());
rootCommand.Subcommands.Add(GenerateSiriCommand.Create());
rootCommand.Subcommands.Add(ListNetexVersionsCommand.Create());
rootCommand.Subcommands.Add(ListSiriVersionsCommand.Create());

return await rootCommand.Parse(args).InvokeAsync();
