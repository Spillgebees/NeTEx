using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using AwesomeAssertions;
using Spillgebees.NeTEx.Models.V1_3_1.NeTEx;
using Spillgebees.NeTEx.Models.V1_3_1.SIRI;

namespace Spillgebees.NeTEx.Models.Tests;

/// <summary>
/// Tests verifying that the generated NeTEx v1.3.1 models correctly use
/// C# 11 <c>required</c> modifiers and nullable reference type annotations.
///
/// The behavioral tests exercise the generated API surface directly (no reflection).
/// The metadata tests use reflection to verify the compiler emitted the expected attributes.
/// </summary>
public class NullabilityAndRequiredTests
{
    // -- behavioral: required properties ------------------------------------------

    [Test]
    public void Should_construct_publication_delivery_with_required_properties()
    {
        // arrange & act — this compiles only because PublicationTimestamp and
        // ParticipantRef are marked 'required'; omitting either would be a compile error.
        var timestamp = DateTimeOffset.UtcNow;
        var delivery = new PublicationDeliveryStructure
        {
            PublicationTimestamp = timestamp,
            ParticipantRef = "test-participant",
        };

        // assert
        delivery.PublicationTimestamp.Should().Be(timestamp);
        delivery.ParticipantRef.Should().Be("test-participant");
    }

    [Test]
    public void Should_construct_multilingual_string_with_nullable_value()
    {
        // arrange & act — MultilingualString.Value is a nullable [XmlText] property
        // because xs:string has no minLength restriction; absent text content
        // produces a clean self-closing element.
        var str = new MultilingualString
        {
            Value = "hello",
        };

        // assert
        str.Value.Should().Be("hello");
    }

    [Test]
    public void Should_construct_natural_language_string_with_required_value()
    {
        // arrange & act — NaturalLanguageStringStructure.Value is a required [XmlText]
        // property because it extends PopulatedStringType which has minLength=1.
        var str = new NaturalLanguageStringStructure
        {
            Value = "hello",
        };

        // assert
        str.Value.Should().Be("hello");
    }

    // -- behavioral: optional nullable properties ---------------------------------

    [Test]
    public void Should_allow_null_on_optional_reference_type_properties()
    {
        // arrange
        var delivery = new PublicationDeliveryStructure
        {
            PublicationTimestamp = DateTimeOffset.UtcNow,
            ParticipantRef = "test",
        };

        // act — optional properties accept null without warnings under #nullable enable
        delivery.Description = null;
        delivery.PublicationRequest = null;
        delivery.DataObjects = null;

        // assert
        delivery.Description.Should().BeNull();
        delivery.PublicationRequest.Should().BeNull();
        delivery.DataObjects.Should().BeNull();
    }

    [Test]
    public void Should_allow_null_on_optional_string_attribute()
    {
        // arrange
        var str = new MultilingualString { Value = "test" };

        // act
        str.Lang = null;

        // assert
        str.Lang.Should().BeNull();
    }

    // -- behavioral: XmlSerializer bypasses required ------------------------------

    [Test]
    public void Should_round_trip_required_properties_via_xml_serializer()
    {
        // arrange — XmlSerializer uses a parameterless constructor and ignores 'required',
        // so serialization round-trips work transparently despite the modifier.
        var serializer = new XmlSerializer(typeof(MultilingualString));
        var original = new MultilingualString { Value = "hello" };

        // act
        using var writer = new StringWriter();
        serializer.Serialize(writer, original);
        var xml = writer.ToString();

        using var reader = new StringReader(xml);
        var result = serializer.Deserialize(reader) as MultilingualString;

        // assert
        result.Should().NotBeNull();
        result!.Value.Should().Be("hello");
        result.Lang.Should().BeNull();
    }

    // -- behavioral: collections are not required ---------------------------------

    [Test]
    public void Should_allow_creating_organisation_without_setting_collection()
    {
        // arrange & act — OrganisationType is a List<> (not required), so
        // we can construct without setting it in the initializer
        var org = new Organisation
        {
            Id = "ORG:1",
            Version = "1",
        };

        // assert — collection is initialized by the constructor
        org.OrganisationType.Should().NotBeNull();
    }

    // -- metadata: required modifier via reflection -------------------------------

    [Test]
    public void Should_have_required_modifier_on_publication_timestamp()
    {
        // arrange
        var property = typeof(PublicationDeliveryStructure)
            .GetProperty(nameof(PublicationDeliveryStructure.PublicationTimestamp))!;

        // act
        var isRequired = property.IsDefined(typeof(RequiredMemberAttribute), inherit: false);

        // assert
        isRequired.Should().BeTrue();
    }

    [Test]
    public void Should_have_required_modifier_on_participant_ref()
    {
        // arrange
        var property = typeof(PublicationDeliveryStructure)
            .GetProperty(nameof(PublicationDeliveryStructure.ParticipantRef))!;

        // act
        var isRequired = property.IsDefined(typeof(RequiredMemberAttribute), inherit: false);

        // assert
        isRequired.Should().BeTrue();
    }

    [Test]
    public void Should_not_have_required_modifier_on_optional_description()
    {
        // arrange
        var property = typeof(PublicationDeliveryStructure)
            .GetProperty(nameof(PublicationDeliveryStructure.Description))!;

        // act
        var isRequired = property.IsDefined(typeof(RequiredMemberAttribute), inherit: false);

        // assert
        isRequired.Should().BeFalse();
    }

    [Test]
    public void Should_not_have_required_modifier_on_optional_publication_request()
    {
        // arrange
        var property = typeof(PublicationDeliveryStructure)
            .GetProperty(nameof(PublicationDeliveryStructure.PublicationRequest))!;

        // act
        var isRequired = property.IsDefined(typeof(RequiredMemberAttribute), inherit: false);

        // assert
        isRequired.Should().BeFalse();
    }

    [Test]
    public void Should_not_have_required_modifier_on_unconstrained_xml_text_value_property()
    {
        // arrange — MultilingualString.Value is an [XmlText] property whose base type
        // (xs:string) has no minLength restriction. It should be nullable, not required,
        // because absent text content produces a clean self-closing element.
        var property = typeof(MultilingualString)
            .GetProperty(nameof(MultilingualString.Value))!;

        // act
        var isRequired = property.IsDefined(typeof(RequiredMemberAttribute), inherit: false);

        // assert
        isRequired.Should().BeFalse();
    }

    [Test]
    public void Should_have_required_modifier_on_min_length_constrained_xml_text_value_property()
    {
        // arrange — NaturalLanguageStringStructure.Value is an [XmlText] property whose
        // base type (PopulatedStringType) has minLength=1. The text content is effectively
        // required, so the property gets the 'required' modifier.
        var property = typeof(NaturalLanguageStringStructure)
            .GetProperty(nameof(NaturalLanguageStringStructure.Value))!;

        // act
        var isRequired = property.IsDefined(typeof(RequiredMemberAttribute), inherit: false);

        // assert
        isRequired.Should().BeTrue();
    }

    [Test]
    public void Should_not_have_required_modifier_on_collection_property()
    {
        // arrange — Organisation.OrganisationType is a List<> (enumerable), never required
        var property = typeof(Organisation)
            .GetProperty(nameof(Organisation.OrganisationType))!;

        // act
        var isRequired = property.IsDefined(typeof(RequiredMemberAttribute), inherit: false);

        // assert
        isRequired.Should().BeFalse();
    }

    // -- metadata: nullable annotations via reflection ----------------------------

    [Test]
    public void Should_have_nullable_annotation_on_optional_reference_type_property()
    {
        // arrange
        var property = typeof(PublicationDeliveryStructure)
            .GetProperty(nameof(PublicationDeliveryStructure.Description))!;
        var context = new NullabilityInfoContext();

        // act
        var info = context.Create(property);

        // assert
        info.WriteState.Should().Be(NullabilityState.Nullable);
        info.ReadState.Should().Be(NullabilityState.Nullable);
    }

    [Test]
    public void Should_have_nullable_annotation_on_optional_publication_request()
    {
        // arrange
        var property = typeof(PublicationDeliveryStructure)
            .GetProperty(nameof(PublicationDeliveryStructure.PublicationRequest))!;
        var context = new NullabilityInfoContext();

        // act
        var info = context.Create(property);

        // assert
        info.WriteState.Should().Be(NullabilityState.Nullable);
    }

    [Test]
    public void Should_not_have_nullable_annotation_on_required_reference_type_property()
    {
        // arrange — ParticipantRef is required and non-nullable
        var property = typeof(PublicationDeliveryStructure)
            .GetProperty(nameof(PublicationDeliveryStructure.ParticipantRef))!;
        var context = new NullabilityInfoContext();

        // act
        var info = context.Create(property);

        // assert
        info.WriteState.Should().Be(NullabilityState.NotNull);
        info.ReadState.Should().Be(NullabilityState.NotNull);
    }

    [Test]
    public void Should_have_nullable_annotation_on_optional_string_attribute()
    {
        // arrange — MultilingualString.Lang is an optional xml attribute
        var property = typeof(MultilingualString)
            .GetProperty(nameof(MultilingualString.Lang))!;
        var context = new NullabilityInfoContext();

        // act
        var info = context.Create(property);

        // assert
        info.WriteState.Should().Be(NullabilityState.Nullable);
    }

    [Test]
    public void Should_have_nullable_annotation_on_unconstrained_xml_text_value_property()
    {
        // arrange — MultilingualString.Value has no minLength restriction,
        // so it should be string? (nullable).
        var property = typeof(MultilingualString)
            .GetProperty(nameof(MultilingualString.Value))!;
        var context = new NullabilityInfoContext();

        // act
        var info = context.Create(property);

        // assert
        info.WriteState.Should().Be(NullabilityState.Nullable);
        info.ReadState.Should().Be(NullabilityState.Nullable);
    }

    [Test]
    public void Should_not_have_nullable_annotation_on_min_length_constrained_xml_text_value_property()
    {
        // arrange — NaturalLanguageStringStructure.Value has minLength=1,
        // so it should be required string (not nullable).
        var property = typeof(NaturalLanguageStringStructure)
            .GetProperty(nameof(NaturalLanguageStringStructure.Value))!;
        var context = new NullabilityInfoContext();

        // act
        var info = context.Create(property);

        // assert
        info.WriteState.Should().Be(NullabilityState.NotNull);
        info.ReadState.Should().Be(NullabilityState.NotNull);
    }

    // -- metadata: #nullable enable directive -------------------------------------

    [Test]
    public void Should_have_nullable_context_enabled_on_generated_types()
    {
        // arrange — NullableContextAttribute is emitted by the compiler when
        // #nullable enable is active for the file
        var attr = typeof(PublicationDeliveryStructure)
            .GetCustomAttribute<NullableContextAttribute>();

        // assert — a non-null attribute means the nullable context was active
        attr.Should().NotBeNull();
    }
}
