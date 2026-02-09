using System.Xml.Serialization;
using AwesomeAssertions;
using Spillgebees.NeTEx.Models.V1_3_1.GML;

namespace Spillgebees.NeTEx.Models.Tests.Serialization;

public class GmlSerializationTests
{
    [Test]
    public void Should_serialize_and_deserialize_direct_position_type()
    {
        // arrange
        var serializer = new XmlSerializer(typeof(DirectPositionType));
        var position = new DirectPositionType
        {
            Value = "59.911491 10.752245",
            SrsName = "urn:ogc:def:crs:EPSG::4326",
        };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, position);
        var xml = writer.ToString();

        using var reader = new StringReader(xml);
        var deserialized = serializer.Deserialize(reader) as DirectPositionType;

        // assert
        deserialized.Should().NotBeNull();
        deserialized!.Value.Should().Be("59.911491 10.752245");
        deserialized.SrsName.Should().Be("urn:ogc:def:crs:EPSG::4326");
    }

    [Test]
    public void Should_have_gml_xml_type_namespace_on_direct_position_type()
    {
        // act
        var xmlTypeAttr = typeof(DirectPositionType)
            .GetCustomAttributes(typeof(XmlTypeAttribute), false)
            .Cast<XmlTypeAttribute>()
            .FirstOrDefault();

        // assert
        xmlTypeAttr.Should().NotBeNull();
        xmlTypeAttr!.Namespace.Should().Be("http://www.opengis.net/gml/3.2");
    }
}
