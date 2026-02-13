using System.Xml.Serialization;
using AwesomeAssertions;
using Spillgebees.SIRI.Models.V2_2.ACSB;
using Spillgebees.SIRI.Models.V2_2.GML;
using Spillgebees.SIRI.Models.V2_2.IFOPT;

namespace Spillgebees.SIRI.Models.Tests.Serialization;

public class IfoptSerializationTests
{
    [Test]
    public void Should_serialize_and_deserialize_stop_place_ref_structure()
    {
        // arrange
        var serializer = new XmlSerializer(typeof(StopPlaceRefStructure));
        var original = new StopPlaceRefStructure
        {
            Value = "NSR:StopPlace:1234",
        };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, original);
        var xml = writer.ToString();

        using var reader = new StringReader(xml);
        var deserialized = serializer.Deserialize(reader) as StopPlaceRefStructure;

        // assert
        deserialized.Should().NotBeNull();
        deserialized.Value.Should().Be("NSR:StopPlace:1234");
    }

    // -- CountryRefStructure (enum base: CountryCodeType) --
    // XmlSerializer cannot handle Nullable<enum> with [XmlText], so the generator
    // must emit a non-nullable enum property. These tests verify that.

    [Test]
    public void Should_construct_xml_serializer_for_country_ref_structure()
    {
        var act = () => new XmlSerializer(typeof(CountryRefStructure));

        act.Should().NotThrow();
    }

    [Test]
    public void Should_serialize_country_ref_structure_with_value()
    {
        // arrange
        var serializer = new XmlSerializer(typeof(CountryRefStructure));
        var original = new CountryRefStructure
        {
            Value = CountryCodeType.De,
        };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, original);
        var xml = writer.ToString();

        // assert — [XmlEnum("de")] maps the enum to lowercase XML representation
        xml.Should().Contain(">de<");
    }

    [Test]
    public void Should_round_trip_country_ref_structure()
    {
        // arrange
        var serializer = new XmlSerializer(typeof(CountryRefStructure));
        var original = new CountryRefStructure
        {
            Value = CountryCodeType.De,
        };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, original);
        var xml = writer.ToString();

        using var reader = new StringReader(xml);
        var deserialized = serializer.Deserialize(reader) as CountryRefStructure;

        // assert
        deserialized.Should().NotBeNull();
        deserialized.Value.Should().Be(CountryCodeType.De);
    }

    // -- AccessibilityStructure (enum base: AccessibilityEnumeration) --

    [Test]
    public void Should_construct_xml_serializer_for_accessibility_structure()
    {
        var act = () => new XmlSerializer(typeof(AccessibilityStructure));

        act.Should().NotThrow();
    }

    [Test]
    public void Should_round_trip_accessibility_structure()
    {
        // arrange
        var serializer = new XmlSerializer(typeof(AccessibilityStructure));
        var original = new AccessibilityStructure
        {
            Value = AccessibilityEnumeration.True,
        };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, original);
        var xml = writer.ToString();

        using var reader = new StringReader(xml);
        var deserialized = serializer.Deserialize(reader) as AccessibilityStructure;

        // assert
        deserialized.Should().NotBeNull();
        deserialized.Value.Should().Be(AccessibilityEnumeration.True);
        xml.Should().Contain(">true<");
    }

    // -- MeasureType (double base + required uom attribute) --

    [Test]
    public void Should_construct_xml_serializer_for_measure_type()
    {
        var act = () => new XmlSerializer(typeof(MeasureType));

        act.Should().NotThrow();
    }

    [Test]
    public void Should_round_trip_measure_type()
    {
        // arrange
        var serializer = new XmlSerializer(typeof(MeasureType));
        var original = new MeasureType
        {
            Value = 42.5,
            Uom = "m",
        };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, original);
        var xml = writer.ToString();

        using var reader = new StringReader(xml);
        var deserialized = serializer.Deserialize(reader) as MeasureType;

        // assert
        deserialized.Should().NotBeNull();
        deserialized.Value.Should().Be(42.5);
        deserialized.Uom.Should().Be("m");
    }

    [Test]
    public void Should_have_correct_xml_type_namespace_on_ifopt_types()
    {
        // act
        var xmlTypeAttr = typeof(StopPlaceRefStructure)
            .GetCustomAttributes(typeof(XmlTypeAttribute), false)
            .Cast<XmlTypeAttribute>()
            .FirstOrDefault();

        // assert
        xmlTypeAttr.Should().NotBeNull();
        xmlTypeAttr.Namespace.Should().Be("http://www.ifopt.org.uk/ifopt");
    }
}
