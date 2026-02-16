using System.Xml.Serialization;
using AwesomeAssertions;
using Spillgebees.SIRI.Models.V2_2.SIRI;

namespace Spillgebees.SIRI.Models.Tests.Serialization;

public class SiriSerializationTests
{
    [Test]
    public void Should_serialize_and_deserialize_natural_language_string()
    {
        // arrange
        var serializer = new XmlSerializer(typeof(NaturalLanguageStringStructure));
        var original = new NaturalLanguageStringStructure
        {
            Value = "Central Station",
            Lang = "en"
        };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, original);
        var xml = writer.ToString();

        using var reader = new StringReader(xml);
        var deserialized = serializer.Deserialize(reader) as NaturalLanguageStringStructure;

        // assert
        deserialized.Should().NotBeNull();
        deserialized.Value.Should().Be("Central Station");
        deserialized.Lang.Should().Be("en");
    }

    [Test]
    public void Should_serialize_and_deserialize_key_value_structure()
    {
        // arrange
        var serializer = new XmlSerializer(typeof(KeyValueStructure));
        var original = new KeyValueStructure
        {
            Key = "mode",
            Value = "bus",
            TypeOfKey = "transportType"
        };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, original);
        var xml = writer.ToString();

        using var reader = new StringReader(xml);
        var deserialized = serializer.Deserialize(reader) as KeyValueStructure;

        // assert — elements include xmlns because the root element doesn't declare the SIRI namespace
        xml.Should().Contain(">mode</Key>");
        xml.Should().Contain(">bus</Value>");
        xml.Should().Contain(">transportType</TypeOfKey>");

        deserialized.Should().NotBeNull();
        deserialized.Key.Should().Be("mode");
        deserialized.Value.Should().Be("bus");
        deserialized.TypeOfKey.Should().Be("transportType");
    }

    [Test]
    public void Should_serialize_and_deserialize_info_link_structure()
    {
        // arrange
        var serializer = new XmlSerializer(typeof(InfoLinkStructure));
        var original = new InfoLinkStructure
        {
            Uri = "https://example.com/timetable.png",
            LinkContent = LinkContentEnumeration.Timetable
        };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, original);
        var xml = writer.ToString();

        using var reader = new StringReader(xml);
        var deserialized = serializer.Deserialize(reader) as InfoLinkStructure;

        // assert — elements include xmlns because the root element doesn't declare the SIRI namespace
        xml.Should().Contain(">https://example.com/timetable.png</Uri>");
        xml.Should().Contain(">timetable</LinkContent>");

        deserialized.Should().NotBeNull();
        deserialized.Uri.Should().Be("https://example.com/timetable.png");
        deserialized.LinkContent.Should().Be(LinkContentEnumeration.Timetable);
    }

    [Test]
    public void Should_not_serialize_null_nullable_value_type()
    {
        // arrange — InfoLinkStructure with no LinkContent set
        var serializer = new XmlSerializer(typeof(InfoLinkStructure));
        var original = new InfoLinkStructure
        {
            Uri = "https://example.com"
        };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, original);
        var xml = writer.ToString();

        // assert — absent nullable value type should not appear in XML
        xml.Should().NotContain("<LinkContent>");
    }

    [Test]
    public void Should_have_xml_root_attribute_on_siri_envelope()
    {
        // act
        var xmlRootAttr = typeof(Siri)
            .GetCustomAttributes(typeof(XmlRootAttribute), false)
            .Cast<XmlRootAttribute>()
            .FirstOrDefault();

        // assert
        xmlRootAttr.Should().NotBeNull();
        xmlRootAttr.ElementName.Should().Be("Siri");
        xmlRootAttr.Namespace.Should().Be("http://www.siri.org.uk/siri");
    }

    [Test]
    public void Should_have_xml_root_attribute_on_service_delivery()
    {
        // act
        var xmlRootAttr = typeof(ServiceDelivery)
            .GetCustomAttributes(typeof(XmlRootAttribute), false)
            .Cast<XmlRootAttribute>()
            .FirstOrDefault();

        // assert
        xmlRootAttr.Should().NotBeNull();
        xmlRootAttr.ElementName.Should().Be("ServiceDelivery");
        xmlRootAttr.Namespace.Should().Be("http://www.siri.org.uk/siri");
    }

    [Test]
    public void Should_have_correct_xml_type_namespace_on_siri_types()
    {
        // act
        var xmlTypeAttr = typeof(NaturalLanguageStringStructure)
            .GetCustomAttributes(typeof(XmlTypeAttribute), false)
            .Cast<XmlTypeAttribute>()
            .FirstOrDefault();

        // assert
        xmlTypeAttr.Should().NotBeNull();
        xmlTypeAttr.Namespace.Should().Be("http://www.siri.org.uk/siri");
    }

    /// <summary>
    /// Siri.Version defaults to "2.1" via backing field initialization.
    /// With <c>[DefaultValueAttribute]</c> suppressed on <c>string?</c> properties,
    /// <c>XmlSerializer</c> must include the default value in the serialized XML.
    /// </summary>
    [Test]
    public void Should_serialize_default_string_value_when_property_is_left_as_default()
    {
        // arrange
        var serializer = new XmlSerializer(typeof(Siri));
        var original = new Siri();

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, original);
        var xml = writer.ToString();

        using var reader = new StringReader(xml);
        var deserialized = serializer.Deserialize(reader) as Siri;

        // assert
        xml.Should().Contain("version=\"2.1\"");
        deserialized.Should().NotBeNull();
        deserialized.Version.Should().Be("2.1");
    }

    /// <summary>
    /// Even when Siri.Version is explicitly set to the default value of "2.1", it should still be serialized in the XML since the [DefaultValue] attribute is suppressed on nullable string properties to avoid unintended consequences of the serializer omitting elements with default values.
    /// </summary>
    [Test]
    public void Should_serialize_default_string_value_when_property_is_explicitly_set_to_default()
    {
        // arrange — explicitly setting Version to the same value as the XSD default
        var serializer = new XmlSerializer(typeof(Siri));
        var original = new Siri { Version = "2.1" };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, original);
        var xml = writer.ToString();

        using var reader = new StringReader(xml);
        var deserialized = serializer.Deserialize(reader) as Siri;

        // assert
        xml.Should().Contain("version=\"2.1\"");
        deserialized.Should().NotBeNull();
        deserialized.Version.Should().Be("2.1");
    }
}
