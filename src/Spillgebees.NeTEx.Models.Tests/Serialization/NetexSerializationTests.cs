using System.Xml.Serialization;
using AwesomeAssertions;
using Spillgebees.NeTEx.Models.V1_3_1.NeTEx;

namespace Spillgebees.NeTEx.Models.Tests.Serialization;

public class NetexSerializationTests
{
    [Test]
    public void Should_serialize_and_deserialize_multilingual_string()
    {
        // arrange
        var serializer = new XmlSerializer(typeof(MultilingualString));
        var original = new MultilingualString
        {
            Value = "Central Station",
            Lang = "en",
        };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, original);
        var xml = writer.ToString();

        using var reader = new StringReader(xml);
        var deserialized = serializer.Deserialize(reader) as MultilingualString;

        // assert
        deserialized.Should().NotBeNull();
        deserialized!.Value.Should().Be("Central Station");
        deserialized.Lang.Should().Be("en");
    }

    [Test]
    public void Should_serialize_and_deserialize_stop_place_with_stop_place_type()
    {
        // arrange
        var serializer = new XmlSerializer(typeof(StopPlace));
        var stopPlace = new StopPlace
        {
            Id = "NSR:StopPlace:1",
            Version = "1",
            StopPlaceType = StopTypeEnumeration.RailStation,
        };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, stopPlace);
        var xml = writer.ToString();

        using var reader = new StringReader(xml);
        var deserialized = serializer.Deserialize(reader) as StopPlace;

        // assert
        xml.Should().Contain("<StopPlaceType>railStation</StopPlaceType>");
        deserialized.Should().NotBeNull();
        deserialized!.StopPlaceType.Should().Be(StopTypeEnumeration.RailStation);
    }

    [Test]
    public void Should_serialize_and_deserialize_publication_delivery_structure()
    {
        // arrange
        var serializer = new XmlSerializer(typeof(PublicationDeliveryStructure));
        var delivery = new PublicationDeliveryStructure
        {
            PublicationTimestamp = DateTimeOffset.UtcNow,
            ParticipantRef = "test-participant",
            Version = "1.0",
        };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, delivery);
        var xml = writer.ToString();

        using var reader = new StringReader(xml);
        var deserialized = serializer.Deserialize(reader) as PublicationDeliveryStructure;

        // assert
        xml.Should().Contain("<ParticipantRef>test-participant</ParticipantRef>");
        deserialized.Should().NotBeNull();
        deserialized!.ParticipantRef.Should().Be("test-participant");
    }

    [Test]
    public void Should_have_correct_xml_type_namespace_on_stop_place()
    {
        // act
        var xmlTypeAttr = typeof(StopPlace)
            .GetCustomAttributes(typeof(XmlTypeAttribute), false)
            .Cast<XmlTypeAttribute>()
            .FirstOrDefault();

        // assert
        xmlTypeAttr.Should().NotBeNull();
        xmlTypeAttr!.Namespace.Should().Be("http://www.netex.org.uk/netex");
    }

    [Test]
    public void Should_have_xml_root_attribute_on_publication_delivery_structure()
    {
        // act
        var xmlRootAttr = typeof(PublicationDeliveryStructure)
            .GetCustomAttributes(typeof(XmlRootAttribute), false)
            .Cast<XmlRootAttribute>()
            .FirstOrDefault();

        // assert
        xmlRootAttr.Should().NotBeNull();
        xmlRootAttr!.ElementName.Should().Be("PublicationDelivery");
        xmlRootAttr.Namespace.Should().Be("http://www.netex.org.uk/netex");
    }
}
