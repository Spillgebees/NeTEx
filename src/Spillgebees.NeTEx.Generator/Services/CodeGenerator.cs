using Spillgebees.NeTEx.Generator.Configuration;
using XmlSchemaClassGenerator;
using XscGenerator = XmlSchemaClassGenerator.Generator;

namespace Spillgebees.NeTEx.Generator.Services;

public static class CodeGenerator
{
    public static GenerationResult Generate(
        string xsdDirectory,
        string outputDirectory,
        string rootNamespace,
        bool verbose = false)
    {
        var netexNamespace = $"{rootNamespace}.{GeneratorDefaults.NetexSubNamespace}";
        var siriNamespace = $"{rootNamespace}.{GeneratorDefaults.SiriSubNamespace}";
        var gmlNamespace = $"{rootNamespace}.{GeneratorDefaults.GmlSubNamespace}";
        var w3Namespace = $"{rootNamespace}.{GeneratorDefaults.W3SubNamespace}";

        var namespaceProvider = new NamespaceProvider
        {
            GenerateNamespace = key =>
                CodeUtilities.GenerateNamespace(key.XmlSchemaNamespace, rootNamespace),
            [new NamespaceKey(GeneratorDefaults.NetexXmlNamespace)] = netexNamespace,
            [new NamespaceKey(GeneratorDefaults.SiriXmlNamespace)] = siriNamespace,
            [new NamespaceKey(GeneratorDefaults.GmlXmlNamespace)] = gmlNamespace,
            [new NamespaceKey(GeneratorDefaults.W3XmlNamespace)] = w3Namespace
        };

        var generator = new XscGenerator
        {
            OutputFolder = Path.GetFullPath(outputDirectory),
            NamespaceProvider = namespaceProvider,

            // .NET Core / Nullable
            NetCoreSpecificCode = true,
            EnableNullableReferenceAttributes = true,
            GenerateNullables = true,

            // Code style
            CompactTypeNames = true,
            SeparateClasses = true,
            SeparateNamespaceHierarchy = true,
            UseDateOnly = true,
            UseXElementForAny = true,
            DateTimeWithTimeZone = true,
            MapUnionToWidestCommonType = true,
            DoNotForceIsNullable = true,

            // Collections
            CollectionSettersMode = CollectionSettersMode.Init,
            CollectionType = typeof(List<>),
            EnumCollection = true,

            // Nullable and required
            EnableNullableDirective = true,
            GenerateRequiredModifier = true,

            // Minimal attribute noise
            GenerateSerializableAttribute = false,
            GenerateDesignerCategoryAttribute = false,
            GenerateDebuggerStepThroughAttribute = false,
            GenerateDescriptionAttribute = false,
            GenerateCommandLineArgumentsComment = false,
            CreateGeneratedCodeAttributeVersion = false,

            // Structure
            EmitOrder = true,
            GenerateInterfaces = false,
            SeparateSubstitutes = true,

            // Logging
            Log = verbose ? msg => Console.WriteLine($"  [xscgen] {msg}") : null,
            // Set NamingProvider after the object initializer to avoid being overwritten
            // by any NamingScheme setter. This renames abstract dummy elements (e.g. 'StopPlace_')
            // so that concrete elements get the clean C# class name (e.g. 'StopPlace').
            NamingProvider = new NetexNamingProvider(NamingScheme.PascalCase)
        };

        var publicationSchema = Path.Combine(xsdDirectory, GeneratorDefaults.PublicationSchemaFileName);
        var siriSchema = Path.Combine(xsdDirectory, GeneratorDefaults.SiriSchemaFileName);

        var schemas = new List<string>();

        if (File.Exists(publicationSchema))
        {
            schemas.Add(publicationSchema);
        }
        else
        {
            throw new FileNotFoundException(
                $"Publication schema not found: {publicationSchema}");
        }

        var siriGenerated = false;
        if (File.Exists(siriSchema))
        {
            schemas.Add(siriSchema);
            siriGenerated = true;
        }
        else
        {
            Console.WriteLine($"  [warn] SIRI schema not found at {siriSchema}, skipping SIRI types.");
        }

        generator.Generate(schemas);

        return new GenerationResult(siriGenerated);
    }
}

public sealed record GenerationResult(bool SiriGenerated);
