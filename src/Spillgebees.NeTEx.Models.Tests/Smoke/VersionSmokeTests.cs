using System.Xml.Serialization;
using AwesomeAssertions;
using V1_2_NeTEx = Spillgebees.NeTEx.Models.V1_2.NeTEx;
using V1_2_SIRI = Spillgebees.NeTEx.Models.V1_2.SIRI;
using V1_2_GML = Spillgebees.NeTEx.Models.V1_2.GML;
using V1_2_2_NeTEx = Spillgebees.NeTEx.Models.V1_2_2.NeTEx;
using V1_2_2_SIRI = Spillgebees.NeTEx.Models.V1_2_2.SIRI;
using V1_2_2_GML = Spillgebees.NeTEx.Models.V1_2_2.GML;
using V1_2_3_NeTEx = Spillgebees.NeTEx.Models.V1_2_3.NeTEx;
using V1_2_3_SIRI = Spillgebees.NeTEx.Models.V1_2_3.SIRI;
using V1_2_3_GML = Spillgebees.NeTEx.Models.V1_2_3.GML;
using V1_3_0_NeTEx = Spillgebees.NeTEx.Models.V1_3_0.NeTEx;
using V1_3_0_SIRI = Spillgebees.NeTEx.Models.V1_3_0.SIRI;
using V1_3_0_GML = Spillgebees.NeTEx.Models.V1_3_0.GML;
using V1_3_1_NeTEx = Spillgebees.NeTEx.Models.V1_3_1.NeTEx;
using V1_3_1_SIRI = Spillgebees.NeTEx.Models.V1_3_1.SIRI;
using V1_3_1_GML = Spillgebees.NeTEx.Models.V1_3_1.GML;

namespace Spillgebees.NeTEx.Models.Tests.Smoke;

/// <summary>
/// Smoke tests verifying that core types exist and have correct XML namespace metadata
/// for each supported NeTEx version (tags from the NeTEx-CEN/NeTEx GitHub repository).
/// </summary>
public class VersionSmokeTests
{
    [Test]
    public void Should_have_core_netex_types_for_v1_2()
    {
        // assert
        typeof(V1_2_NeTEx.PublicationDeliveryStructure).IsClass.Should().BeTrue();
        typeof(V1_2_NeTEx.StopPlace).IsClass.Should().BeTrue();
        typeof(V1_2_NeTEx.Line).IsClass.Should().BeTrue();
        typeof(V1_2_NeTEx.MultilingualString).IsClass.Should().BeTrue();

        GetXmlTypeNamespace(typeof(V1_2_NeTEx.StopPlace))
            .Should().Be("http://www.netex.org.uk/netex");
    }

    [Test]
    public void Should_have_siri_and_gml_types_for_v1_2()
    {
        // assert
        typeof(V1_2_SIRI.LocationStructure).IsClass.Should().BeTrue();
        typeof(V1_2_GML.DirectPositionType).IsClass.Should().BeTrue();

        GetXmlTypeNamespace(typeof(V1_2_SIRI.LocationStructure))
            .Should().Be("http://www.siri.org.uk/siri");
        GetXmlTypeNamespace(typeof(V1_2_GML.DirectPositionType))
            .Should().Be("http://www.opengis.net/gml/3.2");
    }

    [Test]
    public void Should_have_core_netex_types_for_v1_2_2()
    {
        // assert
        typeof(V1_2_2_NeTEx.PublicationDeliveryStructure).IsClass.Should().BeTrue();
        typeof(V1_2_2_NeTEx.StopPlace).IsClass.Should().BeTrue();
        typeof(V1_2_2_NeTEx.Line).IsClass.Should().BeTrue();
        typeof(V1_2_2_NeTEx.MultilingualString).IsClass.Should().BeTrue();

        GetXmlTypeNamespace(typeof(V1_2_2_NeTEx.StopPlace))
            .Should().Be("http://www.netex.org.uk/netex");
    }

    [Test]
    public void Should_have_siri_and_gml_types_for_v1_2_2()
    {
        // assert
        typeof(V1_2_2_SIRI.LocationStructure).IsClass.Should().BeTrue();
        typeof(V1_2_2_GML.DirectPositionType).IsClass.Should().BeTrue();

        GetXmlTypeNamespace(typeof(V1_2_2_SIRI.LocationStructure))
            .Should().Be("http://www.siri.org.uk/siri");
        GetXmlTypeNamespace(typeof(V1_2_2_GML.DirectPositionType))
            .Should().Be("http://www.opengis.net/gml/3.2");
    }

    [Test]
    public void Should_have_core_netex_types_for_v1_2_3()
    {
        // assert
        typeof(V1_2_3_NeTEx.PublicationDeliveryStructure).IsClass.Should().BeTrue();
        typeof(V1_2_3_NeTEx.StopPlace).IsClass.Should().BeTrue();
        typeof(V1_2_3_NeTEx.Line).IsClass.Should().BeTrue();
        typeof(V1_2_3_NeTEx.MultilingualString).IsClass.Should().BeTrue();

        GetXmlTypeNamespace(typeof(V1_2_3_NeTEx.StopPlace))
            .Should().Be("http://www.netex.org.uk/netex");
    }

    [Test]
    public void Should_have_siri_and_gml_types_for_v1_2_3()
    {
        // assert
        typeof(V1_2_3_SIRI.LocationStructure).IsClass.Should().BeTrue();
        typeof(V1_2_3_GML.DirectPositionType).IsClass.Should().BeTrue();

        GetXmlTypeNamespace(typeof(V1_2_3_SIRI.LocationStructure))
            .Should().Be("http://www.siri.org.uk/siri");
        GetXmlTypeNamespace(typeof(V1_2_3_GML.DirectPositionType))
            .Should().Be("http://www.opengis.net/gml/3.2");
    }

    [Test]
    public void Should_have_core_netex_types_for_v1_3_0()
    {
        // assert
        typeof(V1_3_0_NeTEx.PublicationDeliveryStructure).IsClass.Should().BeTrue();
        typeof(V1_3_0_NeTEx.StopPlace).IsClass.Should().BeTrue();
        typeof(V1_3_0_NeTEx.Line).IsClass.Should().BeTrue();
        typeof(V1_3_0_NeTEx.MultilingualString).IsClass.Should().BeTrue();

        GetXmlTypeNamespace(typeof(V1_3_0_NeTEx.StopPlace))
            .Should().Be("http://www.netex.org.uk/netex");
    }

    [Test]
    public void Should_have_siri_and_gml_types_for_v1_3_0()
    {
        // assert
        typeof(V1_3_0_SIRI.LocationStructure).IsClass.Should().BeTrue();
        typeof(V1_3_0_GML.DirectPositionType).IsClass.Should().BeTrue();

        GetXmlTypeNamespace(typeof(V1_3_0_SIRI.LocationStructure))
            .Should().Be("http://www.siri.org.uk/siri");
        GetXmlTypeNamespace(typeof(V1_3_0_GML.DirectPositionType))
            .Should().Be("http://www.opengis.net/gml/3.2");
    }

    [Test]
    public void Should_have_core_netex_types_for_v1_3_1()
    {
        // assert
        typeof(V1_3_1_NeTEx.PublicationDeliveryStructure).IsClass.Should().BeTrue();
        typeof(V1_3_1_NeTEx.StopPlace).IsClass.Should().BeTrue();
        typeof(V1_3_1_NeTEx.Line).IsClass.Should().BeTrue();
        typeof(V1_3_1_NeTEx.MultilingualString).IsClass.Should().BeTrue();

        GetXmlTypeNamespace(typeof(V1_3_1_NeTEx.StopPlace))
            .Should().Be("http://www.netex.org.uk/netex");
    }

    [Test]
    public void Should_have_siri_and_gml_types_for_v1_3_1()
    {
        // assert
        typeof(V1_3_1_SIRI.LocationStructure).IsClass.Should().BeTrue();
        typeof(V1_3_1_GML.DirectPositionType).IsClass.Should().BeTrue();

        GetXmlTypeNamespace(typeof(V1_3_1_SIRI.LocationStructure))
            .Should().Be("http://www.siri.org.uk/siri");
        GetXmlTypeNamespace(typeof(V1_3_1_GML.DirectPositionType))
            .Should().Be("http://www.opengis.net/gml/3.2");
    }

    // Note: NeTEx XSD uses dummy elements with trailing underscores (e.g. StopPlace_) as
    // substitution group heads. Our NetexNamingProvider renames these to a Base suffix
    // (e.g. StopPlaceBase), so the concrete types get the clean names (e.g. StopPlace).

    private static string? GetXmlTypeNamespace(Type type) =>
        type.GetCustomAttributes(typeof(XmlTypeAttribute), false)
            .Cast<XmlTypeAttribute>()
            .FirstOrDefault()
            ?.Namespace;
}
