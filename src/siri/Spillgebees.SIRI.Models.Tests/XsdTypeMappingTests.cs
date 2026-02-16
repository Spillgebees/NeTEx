using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;
using AwesomeAssertions;
using Spillgebees.SIRI.Models.V2_2.SIRI;

namespace Spillgebees.SIRI.Models.Tests;

/// <summary>
/// Tests verifying that the xscg fork correctly maps XSD types to C# types,
/// generates default values for XSD fixed/default attributes, applies the
/// ShouldSerialize pattern, uses init setters on collections, and handles
/// xs:list enum collection serialization for SIRI models.
/// </summary>
public class XsdTypeMappingTests
{
    [Test]
    public void Should_map_xs_date_time_to_date_time_offset_for_required_property()
    {
        // arrange & act — HalfOpenTimestampOutputRangeStructure.StartTime is xs:dateTime
        // with minOccurs=1, mapped to DateTimeOffset with the required modifier.
        var property = typeof(HalfOpenTimestampOutputRangeStructure)
            .GetProperty(nameof(HalfOpenTimestampOutputRangeStructure.StartTime))!;

        // assert
        property.PropertyType.Should().Be(typeof(DateTimeOffset));
    }

    [Test]
    public void Should_map_xs_date_time_to_nullable_date_time_offset_for_optional_property()
    {
        // arrange & act — HalfOpenTimestampOutputRangeStructure.EndTime is an optional
        // xs:dateTime, mapped to Nullable<DateTimeOffset>.
        var property = typeof(HalfOpenTimestampOutputRangeStructure)
            .GetProperty(nameof(HalfOpenTimestampOutputRangeStructure.EndTime))!;

        // assert
        property.PropertyType.Should().Be(typeof(DateTimeOffset?));
    }

    [Test]
    public void Should_round_trip_half_open_timestamp_range_via_xml_serializer()
    {
        // arrange
        var serializer = new XmlSerializer(typeof(HalfOpenTimestampOutputRangeStructure));
        var original = new HalfOpenTimestampOutputRangeStructure
        {
            StartTime = new DateTimeOffset(2026, 3, 15, 9, 0, 0, TimeSpan.FromHours(1)),
            EndTime = new DateTimeOffset(2026, 3, 15, 17, 30, 0, TimeSpan.FromHours(1)),
        };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, original);
        var xml = writer.ToString();

        using var reader = new StringReader(xml);
        var deserialized = serializer.Deserialize(reader) as HalfOpenTimestampOutputRangeStructure;

        // assert
        deserialized.Should().NotBeNull();
        deserialized.StartTime.Should().Be(original.StartTime);
        deserialized.EndTime.Should().Be(original.EndTime);
    }

    [Test]
    public void Should_not_serialize_null_end_time_in_half_open_range()
    {
        // arrange — EndTime is optional; when null it should not appear in XML
        var serializer = new XmlSerializer(typeof(HalfOpenTimestampOutputRangeStructure));
        var original = new HalfOpenTimestampOutputRangeStructure
        {
            StartTime = new DateTimeOffset(2026, 3, 15, 9, 0, 0, TimeSpan.Zero),
        };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, original);
        var xml = writer.ToString();

        // assert — the ShouldSerialize pattern prevents serialization
        original.ShouldSerializeEndTime().Should().BeFalse();
        xml.Should().NotContain("<EndTime>");
    }

    [Test]
    public void Should_generate_default_value_for_xsd_default_version_attribute()
    {
        // arrange & act — Siri.Version has default="2.1" in the XSD, which the
        // generator emits as a backing field initialized to "2.1" and a
        // [DefaultValue("2.1")] attribute.
        var siri = new Siri();

        // assert — version is automatically set to the XSD default
        siri.Version.Should().Be("2.1");
    }

    [Test]
    public void Should_not_have_default_value_attribute_on_nullable_string_property_with_xsd_default()
    {
        // arrange
        var property = typeof(Siri)
            .GetProperty(nameof(Siri.Version))!;

        // act
        var attr = property.GetCustomAttribute<DefaultValueAttribute>();

        // assert
        attr.Should().BeNull();
    }

    [Test]
    public void Should_still_have_default_value_attribute_on_value_type_property_with_xsd_default()
    {
        // arrange
        var property = typeof(HalfOpenTimestampOutputRangeStructure)
            .GetProperty(nameof(HalfOpenTimestampOutputRangeStructure.EndTimeStatus))!;

        // act
        var attr = property.GetCustomAttribute<DefaultValueAttribute>();

        // assert
        attr.Should().NotBeNull();
        attr!.Value.Should().Be(EndTimeStatusEnumeration.Undefined);
    }

    [Test]
    public void Should_generate_default_value_for_xsd_default_enum_property()
    {
        // arrange & act — HalfOpenTimestampOutputRangeStructure.EndTimeStatus has
        // default="undefined" in the XSD, mapped to EndTimeStatusEnumeration.Undefined.
        var range = new HalfOpenTimestampOutputRangeStructure
        {
            StartTime = DateTimeOffset.UtcNow,
        };

        // assert
        range.EndTimeStatus.Should().Be(EndTimeStatusEnumeration.Undefined);
    }

    [Test]
    public void Should_use_init_setter_on_collection_properties()
    {
        // arrange — TrainElementStructure.MaximumPassengerCapacities is a List<>
        // with an init setter.
        var property = typeof(TrainElementStructure)
            .GetProperty(nameof(TrainElementStructure.MaximumPassengerCapacities))!;
        var setter = property.GetSetMethod();

        // assert — init-only setters have the IsExternalInit modreq
        setter.Should().NotBeNull();
        setter!.ReturnParameter.GetRequiredCustomModifiers()
            .Should().Contain(typeof(System.Runtime.CompilerServices.IsExternalInit));
    }

    [Test]
    public void Should_initialize_enum_collection_in_constructor()
    {
        // arrange & act — TrainElementStructure constructor initializes FareClasses
        var element = new TrainElementStructure
        {
            TrainElementCode = "TE:001",
        };

        // assert
        element.FareClasses.Should().NotBeNull();
        element.FareClasses.Should().HaveCount(0);
    }

    [Test]
    public void Should_serialize_enum_collection_as_space_separated_xsd_list()
    {
        // arrange — FareClasses is an xs:list of FareClassEnumeration values.
        // The xscg fork generates a proxy *Xml property that serializes as a
        // single element with space-separated values.
        var serializer = new XmlSerializer(typeof(TrainElementStructure));
        var element = new TrainElementStructure
        {
            TrainElementCode = "TE:001",
            FareClasses =
            [
                FareClassEnumeration.FirstClass,
                FareClassEnumeration.StandardClass
            ],
        };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, element);
        var xml = writer.ToString();

        // assert — xs:list serializes as space-separated values in one element
        xml.Should().Contain("<FareClasses>firstClass standardClass</FareClasses>");
    }

    [Test]
    public void Should_round_trip_enum_collection_via_xml_serializer()
    {
        // arrange
        var serializer = new XmlSerializer(typeof(TrainElementStructure));
        var original = new TrainElementStructure
        {
            TrainElementCode = "TE:002",
            FareClasses =
            [
                FareClassEnumeration.BusinessClass,
                FareClassEnumeration.EconomyClass
            ],
        };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, original);
        var xml = writer.ToString();

        using var reader = new StringReader(xml);
        var deserialized = serializer.Deserialize(reader) as TrainElementStructure;

        // assert
        deserialized.Should().NotBeNull();
        deserialized.FareClasses.Should().HaveCount(2);
        deserialized.FareClasses[0].Should().Be(FareClassEnumeration.BusinessClass);
        deserialized.FareClasses[1].Should().Be(FareClassEnumeration.EconomyClass);
    }

    [Test]
    public void Should_not_serialize_empty_enum_collection()
    {
        // arrange — empty FareClasses list should not appear in XML
        var serializer = new XmlSerializer(typeof(TrainElementStructure));
        var element = new TrainElementStructure
        {
            TrainElementCode = "TE:003",
        };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, element);
        var xml = writer.ToString();

        // assert
        element.ShouldSerializeFareClasses().Should().BeFalse();
        xml.Should().NotContain("<FareClasses>");
    }

    [Test]
    public void Should_generate_should_serialize_for_nullable_value_type()
    {
        // arrange — TrainElementStructure.TrainElementType is Nullable<TrainElementTypeEnumeration>
        var element = new TrainElementStructure
        {
            TrainElementCode = "TE:004",
        };

        // act & assert — null by default
        element.TrainElementType.Should().BeNull();
        element.ShouldSerializeTrainElementType().Should().BeFalse();

        // act — set the value
        element.TrainElementType = TrainElementTypeEnumeration.Carriage;

        // assert
        element.ShouldSerializeTrainElementType().Should().BeTrue();
    }

    [Test]
    public void Should_have_default_value_for_boolean_property_with_xsd_default()
    {
        // arrange & act — TrainElementStructure.ReversingDirection defaults to true
        // per the XSD default value.
        var element = new TrainElementStructure
        {
            TrainElementCode = "TE:005",
        };

        // assert
        element.ReversingDirection.Should().BeTrue();
        element.SelfPropelled.Should().BeTrue();
    }
}
