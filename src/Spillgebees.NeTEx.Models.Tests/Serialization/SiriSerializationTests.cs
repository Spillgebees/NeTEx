using System.Xml.Serialization;
using AwesomeAssertions;
using Spillgebees.NeTEx.Models.V1_3_1.SIRI;

namespace Spillgebees.NeTEx.Models.Tests.Serialization;

public class SiriSerializationTests
{
    [Test]
    public void Should_serialize_and_deserialize_location_structure()
    {
        // arrange
        var serializer = new XmlSerializer(typeof(LocationStructure));
        var location = new LocationStructure
        {
            Longitude = 10.752245m,
            Latitude = 59.911491m,
        };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, location);
        var xml = writer.ToString();

        using var reader = new StringReader(xml);
        var deserialized = serializer.Deserialize(reader) as LocationStructure;

        // assert
        deserialized.Should().NotBeNull();
        deserialized.Longitude.Should().Be(10.752245m);
        deserialized.Latitude.Should().Be(59.911491m);
    }

    [Test]
    public void Should_serialize_and_deserialize_natural_language_string_structure()
    {
        // arrange
        var serializer = new XmlSerializer(typeof(NaturalLanguageStringStructure));
        var langString = new NaturalLanguageStringStructure
        {
            Value = "Test String",
            Lang = "en",
        };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, langString);
        var xml = writer.ToString();

        using var reader = new StringReader(xml);
        var deserialized = serializer.Deserialize(reader) as NaturalLanguageStringStructure;

        // assert
        deserialized.Should().NotBeNull();
        deserialized.Value.Should().Be("Test String");
        deserialized.Lang.Should().Be("en");
    }

    [Test]
    public void Should_have_siri_xml_type_namespace_on_location_structure()
    {
        // act
        var xmlTypeAttr = typeof(LocationStructure)
            .GetCustomAttributes(typeof(XmlTypeAttribute), false)
            .Cast<XmlTypeAttribute>()
            .FirstOrDefault();

        // assert
        xmlTypeAttr.Should().NotBeNull();
        xmlTypeAttr.Namespace.Should().Be("http://www.siri.org.uk/siri");
    }
}
