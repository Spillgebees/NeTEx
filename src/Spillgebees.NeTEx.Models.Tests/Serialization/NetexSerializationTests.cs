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
        deserialized.Value.Should().Be("Central Station");
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
        deserialized.StopPlaceType.Should().Be(StopTypeEnumeration.RailStation);
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
        deserialized.ParticipantRef.Should().Be("test-participant");
    }

    [Test]
    public void Should_serialize_and_deserialize_organisation_with_typed_enum_list()
    {
        // arrange
        var serializer = new XmlSerializer(typeof(Organisation));
        var organisation = new Organisation
        {
            Id = "ORG:Operator:1",
            Version = "1",
            OrganisationType =
            [
                OrganisationTypeEnumeration.Operator,
                OrganisationTypeEnumeration.Authority
            ]
        };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, organisation);
        var xml = writer.ToString();

        using var reader = new StringReader(xml);
        var deserialized = serializer.Deserialize(reader) as Organisation;

        // assert
        // xsd:list types serialize as a single element with space-separated values
        // instead of multiple elements of the same name
        xml.Should().Contain("<OrganisationType>operator authority</OrganisationType>");
        xml.Should().NotContain("<OrganisationType>operator</OrganisationType>");
        xml.Should().NotContain("<OrganisationType>authority</OrganisationType>");

        deserialized.Should().NotBeNull();
        deserialized.OrganisationType.Should().HaveCount(2);
        deserialized.OrganisationType[0].Should().Be(OrganisationTypeEnumeration.Operator);
        deserialized.OrganisationType[1].Should().Be(OrganisationTypeEnumeration.Authority);
    }

    [Test]
    public void Should_serialize_and_deserialize_parking_with_typed_enum_list()
    {
        // arrange
        var serializer = new XmlSerializer(typeof(Parking));
        var parking = new Parking
        {
            Id = "PRK:Parking:1",
            Version = "1",
            ParkingPaymentProcess =
            [
                ParkingPaymentProcessEnumeration.PayAndDisplay,
                ParkingPaymentProcessEnumeration.PayByMobileDevice
            ]
        };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, parking);
        var xml = writer.ToString();

        using var reader = new StringReader(xml);
        var deserialized = serializer.Deserialize(reader) as Parking;

        // assert
        // xsd:list types serialize as a single element with space-separated values
        // instead of multiple elements of the same name
        xml.Should().Contain("<ParkingPaymentProcess>payAndDisplay payByMobileDevice</ParkingPaymentProcess>");
        xml.Should().NotContain("<ParkingPaymentProcess>payAndDisplay</ParkingPaymentProcess>");
        xml.Should().NotContain("<ParkingPaymentProcess>payByMobileDevice</ParkingPaymentProcess>");

        deserialized.Should().NotBeNull();
        deserialized.ParkingPaymentProcess.Should().HaveCount(2);
        deserialized.ParkingPaymentProcess[0].Should().Be(ParkingPaymentProcessEnumeration.PayAndDisplay);
        deserialized.ParkingPaymentProcess[1].Should().Be(ParkingPaymentProcessEnumeration.PayByMobileDevice);
    }



    [Test]
    public void Should_serialize_and_deserialize_optional_element_with_default_value_to_nullable_property_with_fallback_default()
    {
        // arrange
        // accessVehicleEquipment is a subclass of EquipmentVersionStructure,
        // which has an optional Note with a default value of "false." in the xsd
        var serializer = new XmlSerializer(typeof(AccessVehicleEquipment));
        var accessVehicleEquipment = new AccessVehicleEquipment
        {
            Id = "NSR:AccessVehicleEquipment:1",
            Version = "1"
        };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, accessVehicleEquipment);
        var xml = writer.ToString();

        using var reader = new StringReader(xml);
        var deserialized = serializer.Deserialize(reader) as AccessVehicleEquipment;

        // assert
        xml.Should().Contain("<AccessVehicleEquipment ");
        xml.Should().Contain("<Note>false.</Note>");
        deserialized.Should().NotBeNull();
        deserialized.Note.Should().NotBeNull();
        deserialized.Note.Value.Should().Be("false.");
    }

    [Test]
    public void Should_serialize_simple_content_ref_as_self_closing_element_when_value_is_null()
    {
        // arrange
        // CodespaceRefStructure has a simpleContent text body (Value) and a required ref attribute.
        // When Value is null the element should serialize as self-closing: <CodespaceRef ref="..." />
        var serializer = new XmlSerializer(typeof(CodespaceRefStructure));
        var codespaceRef = new CodespaceRefStructure { Ref = "nsr" };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, codespaceRef);
        var xml = writer.ToString();

        using var reader = new StringReader(xml);
        var deserialized = serializer.Deserialize(reader) as CodespaceRefStructure;

        // assert — self-closing element (no text body)
        xml.Should().Contain("ref=\"nsr\"");
        xml.Should().NotContain(">nsr</");
        xml.Should().Contain("/>");

        deserialized.Should().NotBeNull();
        deserialized.Ref.Should().Be("nsr");
        deserialized.Value.Should().BeNullOrEmpty();
    }

    [Test]
    public void Should_serialize_simple_content_ref_with_text_body_when_value_is_set()
    {
        // arrange
        var serializer = new XmlSerializer(typeof(CodespaceRefStructure));
        var codespaceRef = new CodespaceRefStructure { Ref = "nsr", Value = "NSR" };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, codespaceRef);
        var xml = writer.ToString();

        using var reader = new StringReader(xml);
        var deserialized = serializer.Deserialize(reader) as CodespaceRefStructure;

        // assert — element has text content
        xml.Should().Contain("ref=\"nsr\"");
        xml.Should().Contain(">NSR</CodespaceRef>");

        deserialized.Should().NotBeNull();
        deserialized.Ref.Should().Be("nsr");
        deserialized.Value.Should().Be("NSR");
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
        xmlTypeAttr.Namespace.Should().Be("http://www.netex.org.uk/netex");
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
        xmlRootAttr.ElementName.Should().Be("PublicationDelivery");
        xmlRootAttr.Namespace.Should().Be("http://www.netex.org.uk/netex");
    }
}
