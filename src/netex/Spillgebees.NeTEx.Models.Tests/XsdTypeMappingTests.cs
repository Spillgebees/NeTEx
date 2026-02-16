using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;
using AwesomeAssertions;
using Spillgebees.NeTEx.Models.V1_3_1.NeTEx;

namespace Spillgebees.NeTEx.Models.Tests;

/// <summary>
/// Tests verifying that the xscg fork correctly maps XSD types to C# types,
/// generates default values for XSD fixed/default attributes, applies the
/// ShouldSerialize pattern for optional value types, uses init setters on
/// collections, and renames substitution group heads via NetexNamingProvider.
/// </summary>
public class XsdTypeMappingTests
{
    [Test]
    public void Should_map_xs_date_to_date_only_for_required_property()
    {
        // arrange & act — OperatingDayVersionStructure.CalendarDate is xs:date
        // with minOccurs=1, mapped to System.DateOnly with the required modifier.
        var property = typeof(OperatingDayVersionStructure)
            .GetProperty(nameof(OperatingDayVersionStructure.CalendarDate))!;

        // assert
        property.PropertyType.Should().Be(typeof(DateOnly));
    }

    [Test]
    public void Should_map_xs_date_to_nullable_date_only_for_optional_property()
    {
        // arrange & act — CustomerVersionStructure.DateOfBirth is an optional xs:date,
        // mapped to Nullable<DateOnly>.
        var property = typeof(CustomerVersionStructure)
            .GetProperty(nameof(CustomerVersionStructure.DateOfBirth))!;

        // assert
        property.PropertyType.Should().Be(typeof(DateOnly?));
    }

    [Test]
    public void Should_map_xs_date_time_to_date_time_offset_for_required_property()
    {
        // arrange & act — ClosedTimestampRangeStructure.StartTime is xs:dateTime
        // with minOccurs=1, mapped to DateTimeOffset with the required modifier.
        var property = typeof(ClosedTimestampRangeStructure)
            .GetProperty(nameof(ClosedTimestampRangeStructure.StartTime))!;

        // assert
        property.PropertyType.Should().Be(typeof(DateTimeOffset));
    }

    [Test]
    public void Should_map_xs_date_time_to_nullable_date_time_offset_for_optional_property()
    {
        // arrange & act — CustomerVersionStructure.EmailVerified is an optional
        // xs:dateTime, mapped to Nullable<DateTimeOffset>.
        var property = typeof(CustomerVersionStructure)
            .GetProperty(nameof(CustomerVersionStructure.EmailVerified))!;

        // assert
        property.PropertyType.Should().Be(typeof(DateTimeOffset?));
    }

    [Test]
    public void Should_round_trip_closed_timestamp_range_via_xml_serializer()
    {
        // arrange
        var serializer = new XmlSerializer(typeof(ClosedTimestampRangeStructure));
        var original = new ClosedTimestampRangeStructure
        {
            StartTime = new DateTimeOffset(2026, 6, 1, 8, 0, 0, TimeSpan.FromHours(2)),
            EndTime = new DateTimeOffset(2026, 6, 1, 18, 0, 0, TimeSpan.FromHours(2)),
        };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, original);
        var xml = writer.ToString();

        using var reader = new StringReader(xml);
        var deserialized = serializer.Deserialize(reader) as ClosedTimestampRangeStructure;

        // assert
        deserialized.Should().NotBeNull();
        deserialized.StartTime.Should().Be(original.StartTime);
        deserialized.EndTime.Should().Be(original.EndTime);
    }

    [Test]
    public void Should_generate_default_value_for_xsd_default_version_attribute()
    {
        // arrange & act — PublicationDeliveryStructure.Version has default="1.0"
        // in the XSD, which the generator emits as a backing field initialized
        // to "1.0" and a [DefaultValue("1.0")] attribute.
        var delivery = new PublicationDeliveryStructure
        {
            PublicationTimestamp = DateTimeOffset.UtcNow,
            ParticipantRef = "test",
        };

        // assert — version is automatically set to the XSD default
        delivery.Version.Should().Be("1.0");
    }

    [Test]
    public void Should_not_have_default_value_attribute_on_nullable_string_property_with_xsd_default()
    {
        // arrange
        var property = typeof(PublicationDeliveryStructure)
            .GetProperty(nameof(PublicationDeliveryStructure.Version))!;

        // act
        var attr = property.GetCustomAttribute<DefaultValueAttribute>();

        // assert
        attr.Should().BeNull();
    }

    [Test]
    public void Should_still_have_default_value_attribute_on_value_type_property_with_xsd_default()
    {
        // arrange
        var property = typeof(DayTypeAssignmentVersionStructure)
            .GetProperty(nameof(DayTypeAssignmentVersionStructure.IsAvailable))!;

        // act
        var attr = property.GetCustomAttribute<DefaultValueAttribute>();

        // assert
        attr.Should().NotBeNull();
        attr!.Value.Should().Be(true);
    }

    [Test]
    public void Should_generate_should_serialize_method_for_nullable_date_only()
    {
        // arrange — CustomerVersionStructure has a ShouldSerializeDateOfBirth() method
        // generated because DateOfBirth is Nullable<DateOnly>.
        var method = typeof(CustomerVersionStructure)
            .GetMethod("ShouldSerializeDateOfBirth");

        // assert
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(bool));
    }

    [Test]
    public void Should_return_false_from_should_serialize_when_nullable_date_only_is_null()
    {
        // arrange
        var customer = new CustomerVersionStructure();

        // act & assert
        customer.DateOfBirth.Should().BeNull();
        customer.ShouldSerializeDateOfBirth().Should().BeFalse();
    }

    [Test]
    public void Should_return_true_from_should_serialize_when_nullable_date_only_is_set()
    {
        // arrange
        var customer = new CustomerVersionStructure
        {
            DateOfBirth = new DateOnly(1990, 1, 15),
        };

        // act & assert
        customer.ShouldSerializeDateOfBirth().Should().BeTrue();
    }

    [Test]
    public void Should_use_init_setter_on_collection_properties()
    {
        // arrange — Organisation.OrganisationType is a List<> with an init setter,
        // meaning it can be set via object initializer but not reassigned afterwards.
        var property = typeof(Organisation)
            .GetProperty(nameof(Organisation.OrganisationType))!;
        var setter = property.GetSetMethod();

        // assert — init-only setters have the IsExternalInit modreq
        setter.Should().NotBeNull();
        setter!.ReturnParameter.GetRequiredCustomModifiers()
            .Should().Contain(typeof(System.Runtime.CompilerServices.IsExternalInit));
    }

    [Test]
    public void Should_rename_substitution_group_head_to_base_suffix()
    {
        // arrange — The NeTEx XSD defines StopPlace_ as an abstract dummy element
        // used as a substitution group head. The NetexNamingProvider renames it to
        // StopPlaceBase so the concrete StopPlace class gets the clean name.

        // act
        var baseType = typeof(StopPlaceBase);
        var xmlType = baseType.GetCustomAttribute<XmlTypeAttribute>();

        // assert — C# class is "StopPlaceBase", but XmlType still references "StopPlace_"
        baseType.Name.Should().Be("StopPlaceBase");
        xmlType.Should().NotBeNull();
        xmlType!.TypeName.Should().Be("StopPlace_");
    }

    [Test]
    public void Should_give_concrete_type_the_clean_name()
    {
        // arrange & act — StopPlace is the concrete type with the clean name
        var concreteType = typeof(StopPlace);
        var xmlType = concreteType.GetCustomAttribute<XmlTypeAttribute>();

        // assert
        concreteType.Name.Should().Be("StopPlace");
        xmlType.Should().NotBeNull();
        xmlType!.TypeName.Should().Be("StopPlace");
    }

    [Test]
    public void Should_generate_should_serialize_for_nullable_time_only()
    {
        // arrange — OperatingDayVersionStructure.EarliestTime is Nullable<TimeOnly>
        var operatingDay = new OperatingDayVersionStructure
        {
            CalendarDate = new DateOnly(2026, 6, 15),
        };

        // act & assert — null by default, ShouldSerialize returns false
        operatingDay.EarliestTime.Should().BeNull();
        operatingDay.ShouldSerializeEarliestTime().Should().BeFalse();

        // act — set the value
        operatingDay.EarliestTime = new TimeOnly(4, 30);

        // assert — ShouldSerialize now returns true
        operatingDay.ShouldSerializeEarliestTime().Should().BeTrue();
    }

    [Test]
    public void Should_generate_should_serialize_for_nullable_time_span()
    {
        // arrange — OperatingDayVersionStructure.DayLength is Nullable<TimeSpan>
        var operatingDay = new OperatingDayVersionStructure
        {
            CalendarDate = new DateOnly(2026, 6, 15),
        };

        // act & assert — null by default, ShouldSerialize returns false
        operatingDay.DayLength.Should().BeNull();
        operatingDay.ShouldSerializeDayLength().Should().BeFalse();

        // act — set the value
        operatingDay.DayLength = TimeSpan.FromHours(28);

        // assert — ShouldSerialize now returns true
        operatingDay.ShouldSerializeDayLength().Should().BeTrue();
    }
}
