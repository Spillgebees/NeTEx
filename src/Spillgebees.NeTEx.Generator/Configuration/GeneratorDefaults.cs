namespace Spillgebees.NeTEx.Generator.Configuration;

public static class GeneratorDefaults
{
    public const string DefaultVersion = "v1.3.1";
    public const string DefaultOutputDirectory = "./Generated";
    public const string DefaultNamespace = "NeTEx.Models";

    public const string NetexXmlNamespace = "http://www.netex.org.uk/netex";
    public const string SiriXmlNamespace = "http://www.siri.org.uk/siri";
    public const string GmlXmlNamespace = "http://www.opengis.net/gml/3.2";
    public const string W3XmlNamespace = "http://www.w3.org/XML/1998/namespace";

    public const string NetexSubNamespace = "NeTEx";
    public const string SiriSubNamespace = "SIRI";
    public const string GmlSubNamespace = "GML";
    public const string W3SubNamespace = "W3";

    public const string GitHubArchiveUrlTemplate =
        "https://github.com/NeTEx-CEN/NeTEx/archive/refs/tags/{0}.tar.gz";

    public const string GitHubArchiveRefUrlTemplate =
        "https://github.com/NeTEx-CEN/NeTEx/archive/{0}.tar.gz";

    public const string GitHubApiTagsUrl =
        "https://api.github.com/repos/NeTEx-CEN/NeTEx/tags";

    public const string PublicationSchemaFileName = "NeTEx_publication.xsd";
    public const string SiriSchemaFileName = "NeTEx_siri.xsd";
}
