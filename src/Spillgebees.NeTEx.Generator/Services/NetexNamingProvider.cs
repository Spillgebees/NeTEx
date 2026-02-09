using System.Xml;
using System.Xml.Schema;
using XmlSchemaClassGenerator;

namespace Spillgebees.NeTEx.Generator.Services;

/// <summary>
/// Custom naming provider that prevents substitution group head elements
/// (XSD elements ending with '_', e.g. 'StopPlace_', 'Line_') from stealing
/// the clean C# class name from their concrete counterparts.
///
/// The NeTEx XSD uses a convention where dummy elements have a trailing underscore
/// (e.g. 'StopPlace_') as substitution group heads, and concrete elements have
/// the clean name (e.g. 'StopPlace'). Without this provider, xscgen strips the
/// trailing '_' from the dummy element name, giving it the clean C# name and
/// forcing the concrete element to get a '2' suffix.
///
/// This provider appends 'Base' to all underscore-suffixed element and type names
/// (e.g. 'StopPlace_' becomes 'StopPlaceBase'), ensuring the concrete element
/// gets the clean C# class name consumers expect.
/// </summary>
public class NetexNamingProvider : NamingProvider
{
    public NetexNamingProvider(NamingScheme namingScheme)
        : base(namingScheme)
    {
    }

    public override string RootClassNameFromQualifiedName(
        XmlQualifiedName qualifiedName,
        XmlSchemaElement xmlElement)
    {
        if (qualifiedName.Name.EndsWith("_"))
        {
            var baseName = base.RootClassNameFromQualifiedName(qualifiedName, xmlElement);
            return baseName + "Base";
        }

        return base.RootClassNameFromQualifiedName(qualifiedName, xmlElement);
    }

    public override string ComplexTypeNameFromQualifiedName(
        XmlQualifiedName qualifiedName,
        XmlSchemaComplexType complexType)
    {
        // Handle anonymous complex types derived from underscore-suffixed elements
        // (e.g. the inline type of 'MediumAccessDevice_' gets qualified name 'MediumAccessDevice_')
        if (qualifiedName.Name.EndsWith("_"))
        {
            var baseName = base.ComplexTypeNameFromQualifiedName(qualifiedName, complexType);
            return baseName + "Base";
        }

        return base.ComplexTypeNameFromQualifiedName(qualifiedName, complexType);
    }
}
