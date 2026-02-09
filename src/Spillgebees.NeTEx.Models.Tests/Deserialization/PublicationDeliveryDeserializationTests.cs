using System.Xml;
using System.Xml.Serialization;
using AwesomeAssertions;
using Spillgebees.NeTEx.Models.V1_3_1.NeTEx;

namespace Spillgebees.NeTEx.Models.Tests.Deserialization;

/// <summary>
/// Tests for deserializing NeTEx XML documents using XmlDocument + XPath.
///
/// These tests validate parsing of real-world NeTEx XML structures without constructing
/// full object graphs, complementing the XmlSerializer round-trip tests in the Serialization folder.
/// </summary>
public class PublicationDeliveryDeserializationTests
{
    [Test]
    public void Should_deserialize_stop_place_xml_via_xml_document()
    {
        // arrange
        var xml = File.ReadAllText("TestData/simple-stop-place.xml");
        var doc = new XmlDocument();
        var nsManager = new XmlNamespaceManager(doc.NameTable);
        nsManager.AddNamespace("netex", "http://www.netex.org.uk/netex");

        // act
        doc.LoadXml(xml);

        // assert
        var root = doc.SelectSingleNode("/netex:PublicationDelivery", nsManager);
        root.Should().NotBeNull();

        var participantRef = doc.SelectSingleNode(
            "/netex:PublicationDelivery/netex:ParticipantRef", nsManager);
        participantRef.Should().NotBeNull();
        participantRef!.InnerText.Should().Be("test-participant");

        var stopPlaceName = doc.SelectSingleNode("//netex:StopPlace/netex:Name", nsManager);
        stopPlaceName.Should().NotBeNull();
        stopPlaceName!.InnerText.Should().Be("Central Station");

        var stopPlaceType = doc.SelectSingleNode("//netex:StopPlace/netex:StopPlaceType", nsManager);
        stopPlaceType.Should().NotBeNull();
        stopPlaceType!.InnerText.Should().Be("railStation");
    }

    [Test]
    public void Should_deserialize_timetable_xml_via_xml_document()
    {
        // arrange
        var xml = File.ReadAllText("TestData/simple-timetable.xml");
        var doc = new XmlDocument();
        var nsManager = new XmlNamespaceManager(doc.NameTable);
        nsManager.AddNamespace("netex", "http://www.netex.org.uk/netex");

        // act
        doc.LoadXml(xml);

        // assert
        var lineName = doc.SelectSingleNode("//netex:Line/netex:Name", nsManager);
        lineName.Should().NotBeNull();
        lineName!.InnerText.Should().Be("Bus Line 42");

        var transportMode = doc.SelectSingleNode("//netex:Line/netex:TransportMode", nsManager);
        transportMode.Should().NotBeNull();
        transportMode!.InnerText.Should().Be("bus");
    }

    [Test]
    public void Should_deserialize_cross_namespace_xml_via_xml_document()
    {
        // arrange
        var xml = File.ReadAllText("TestData/stop-place-with-location.xml");
        var doc = new XmlDocument();
        var nsManager = new XmlNamespaceManager(doc.NameTable);
        nsManager.AddNamespace("netex", "http://www.netex.org.uk/netex");

        // act
        doc.LoadXml(xml);

        // assert
        var root = doc.SelectSingleNode("/netex:PublicationDelivery", nsManager);
        root.Should().NotBeNull();

        var participantRef = doc.SelectSingleNode(
            "/netex:PublicationDelivery/netex:ParticipantRef", nsManager);
        participantRef.Should().NotBeNull();
        participantRef!.InnerText.Should().Be("test-participant");

        var longitude = doc.SelectSingleNode("//netex:Location/netex:Longitude", nsManager);
        longitude.Should().NotBeNull();
        longitude!.InnerText.Should().Be("10.752245");

        var latitude = doc.SelectSingleNode("//netex:Location/netex:Latitude", nsManager);
        latitude.Should().NotBeNull();
        latitude!.InnerText.Should().Be("59.911491");

        var stopPlaceName = doc.SelectSingleNode("//netex:StopPlace/netex:Name", nsManager);
        stopPlaceName.Should().NotBeNull();
        stopPlaceName!.InnerText.Should().Be("Oslo S");
    }

    [Test]
    public void Should_serialize_and_deserialize_multilingual_string()
    {
        // arrange
        var serializer = new XmlSerializer(typeof(MultilingualString));
        var original = new MultilingualString
        {
            Value = "Round trip test",
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
        deserialized!.Value.Should().Be("Round trip test");
        deserialized.Lang.Should().Be("en");
    }
}
