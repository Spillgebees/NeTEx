using Spillgebees.Transmodel.Generator.Configuration;
using XmlSchemaClassGenerator;
using XscGenerator = XmlSchemaClassGenerator.Generator;

namespace Spillgebees.Transmodel.Generator.Services;

public static class CodeGenerator
{
    public static GenerationResult GenerateNetex(
        string xsdDirectory,
        string outputDirectory,
        string rootNamespace,
        bool verbose = false)
    {
        var netexNamespace = $"{rootNamespace}.{NetexDefaults.NetexSubNamespace}";
        var siriNamespace = $"{rootNamespace}.{NetexDefaults.SiriSubNamespace}";
        var gmlNamespace = $"{rootNamespace}.{SharedDefaults.GmlSubNamespace}";
        var w3Namespace = $"{rootNamespace}.{SharedDefaults.W3SubNamespace}";

        var namespaceProvider = new NamespaceProvider
        {
            GenerateNamespace = key =>
                CodeUtilities.GenerateNamespace(key.XmlSchemaNamespace, rootNamespace),
            [new NamespaceKey(NetexDefaults.NetexXmlNamespace)] = netexNamespace,
            [new NamespaceKey(NetexDefaults.SiriXmlNamespace)] = siriNamespace,
            [new NamespaceKey(SharedDefaults.GmlXmlNamespace)] = gmlNamespace,
            [new NamespaceKey(SharedDefaults.W3XmlNamespace)] = w3Namespace
        };

        var generator = CreateBaseGenerator(outputDirectory, rootNamespace, namespaceProvider, verbose);
        // Set NamingProvider after the object initializer to avoid being overwritten
        // by any NamingScheme setter. This renames abstract dummy elements (e.g. 'StopPlace_')
        // so that concrete elements get the clean C# class name (e.g. 'StopPlace').
        generator.NamingProvider = new NetexNamingProvider(NamingScheme.PascalCase);

        var publicationSchema = Path.Combine(xsdDirectory, NetexDefaults.PublicationSchemaFileName);
        var siriSchema = Path.Combine(xsdDirectory, NetexDefaults.SiriSchemaFileName);

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

    public static void GenerateSiri(
        string xsdDirectory,
        string outputDirectory,
        string rootNamespace,
        bool verbose = false)
    {
        var siriNamespace = $"{rootNamespace}.{SiriDefaults.SiriSubNamespace}";
        var ifoptNamespace = $"{rootNamespace}.{SiriDefaults.IfoptSubNamespace}";
        var acsbNamespace = $"{rootNamespace}.{SiriDefaults.AcsbSubNamespace}";
        var datex2Namespace = $"{rootNamespace}.{SiriDefaults.Datex2SubNamespace}";
        var wsdlNamespace = $"{rootNamespace}.{SiriDefaults.WsdlSubNamespace}";
        var gmlNamespace = $"{rootNamespace}.{SharedDefaults.GmlSubNamespace}";
        var w3Namespace = $"{rootNamespace}.{SharedDefaults.W3SubNamespace}";

        var namespaceProvider = new NamespaceProvider
        {
            GenerateNamespace = key =>
                CodeUtilities.GenerateNamespace(key.XmlSchemaNamespace, rootNamespace),
            [new NamespaceKey(SiriDefaults.SiriXmlNamespace)] = siriNamespace,
            [new NamespaceKey(SiriDefaults.IfoptXmlNamespace)] = ifoptNamespace,
            [new NamespaceKey(SiriDefaults.AcsbXmlNamespace)] = acsbNamespace,
            [new NamespaceKey(SiriDefaults.Datex2XmlNamespace)] = datex2Namespace,
            [new NamespaceKey(SiriDefaults.WsdlXmlNamespace)] = wsdlNamespace,
            [new NamespaceKey(SharedDefaults.GmlXmlNamespace)] = gmlNamespace,
            [new NamespaceKey(SharedDefaults.W3XmlNamespace)] = w3Namespace
        };

        var generator = CreateBaseGenerator(outputDirectory, rootNamespace, namespaceProvider, verbose);
        // SIRI doesn't have the underscore naming convention, use default PascalCase
        generator.NamingProvider = new NamingProvider(NamingScheme.PascalCase);

        var mainSchema = Path.Combine(xsdDirectory, SiriDefaults.MainSchemaFileName);
        if (!File.Exists(mainSchema))
        {
            throw new FileNotFoundException($"SIRI main schema not found: {mainSchema}");
        }

        var schemas = new List<string> { mainSchema };

        // Add WSDL model schemas if present
        var wsdlDir = Path.Combine(xsdDirectory, "wsdl_model");
        if (Directory.Exists(wsdlDir))
        {
            schemas.AddRange(Directory.GetFiles(wsdlDir, "*.xsd"));
        }

        generator.Generate(schemas);
    }

    private static XscGenerator CreateBaseGenerator(
        string outputDirectory,
        string rootNamespace,
        NamespaceProvider namespaceProvider,
        bool verbose)
    {
        return new XscGenerator
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

            // Nullable, required, and choice groups
            EnableNullableDirective = true,
            GenerateRequiredModifier = true,
            UseShouldSerializePattern = true,
            GenerateChoiceGroupAttributes = true,
            ChoiceGroupAttributeNamespace = rootNamespace,

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
        };
    }
}

public sealed record GenerationResult(bool SiriGenerated);
