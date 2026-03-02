using System.Xml.Serialization;
using AwesomeAssertions;
using Spillgebees.NeTEx.Models.V2_0_0.NeTEx;

namespace Spillgebees.NeTEx.Models.Tests.Serialization.Netex.v2;

public class SerializationTests
{
    [Test]
    public void Should_serialize_and_deserialize_multilingual_string()
    {
        // arrange
        var serializer = new XmlSerializer(typeof(MultilingualString));
        List<TextType> texts =
        [
            new()
            {
                Value = "Central station",
                Lang = "en"
            },
            new()
            {
                Value = "Zentrale Station",
                Lang = "de"
            },
            new()
            {
                Value = "Station centrale",
                Lang = "fr"
            },
            new()
            {
                Value = "Zentral Statioun",
                Lang = "lb"
            }
        ];
        var original = new MultilingualString
        {
            Text = texts
        };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, original);
        var xml = writer.ToString();

        using var reader = new StringReader(xml);
        var deserialized = serializer.Deserialize(reader) as MultilingualString;

        // assert
        xml.Should().Contain("<Text lang=\"en\" xmlns=\"http://www.netex.org.uk/netex\">Central station</Text>");
        xml.Should().Contain("<Text lang=\"de\" xmlns=\"http://www.netex.org.uk/netex\">Zentrale Station</Text>");
        xml.Should().Contain("<Text lang=\"fr\" xmlns=\"http://www.netex.org.uk/netex\">Station centrale</Text>");
        xml.Should().Contain("<Text lang=\"lb\" xmlns=\"http://www.netex.org.uk/netex\">Zentral Statioun</Text>");
        deserialized.Should().NotBeNull();
        deserialized.Text.Should().BeEquivalentTo(texts);
    }
}
