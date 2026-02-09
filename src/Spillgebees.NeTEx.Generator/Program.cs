using System.CommandLine;
using Spillgebees.NeTEx.Generator.Commands;

var rootCommand = new RootCommand(
    "Spillgebees NeTEx Generator - Generate C# model classes from NeTEx XSD schemas");

rootCommand.Subcommands.Add(GenerateCommand.Create());
rootCommand.Subcommands.Add(ListVersionsCommand.Create());

return await rootCommand.Parse(args).InvokeAsync();
