using System.Reflection;
using AwesomeAssertions;
using Spillgebees.NeTEx.Models.V1_3_1;
using Spillgebees.NeTEx.Models.V1_3_1.NeTEx;

namespace Spillgebees.NeTEx.Models.Tests;

/// <summary>
/// Tests verifying that the generated NeTEx v1.3.1 models carry correct
/// <see cref="XmlChoiceGroupAttribute"/> annotations on properties that
/// originate from <c>xs:choice</c> groups in the NeTEx XSD schemas.
/// </summary>
public class ChoiceGroupAttributeTests
{
    // -- helpers ------------------------------------------------------------------

    private static XmlChoiceGroupAttribute? GetChoiceGroupAttribute<T>(string propertyName) =>
        typeof(T).GetProperty(propertyName)?
            .GetCustomAttribute<XmlChoiceGroupAttribute>();

    private static XmlChoiceGroupAttribute GetRequiredChoiceGroupAttribute<T>(string propertyName)
    {
        var attr = GetChoiceGroupAttribute<T>(propertyName);
        attr.Should().NotBeNull($"property {typeof(T).Name}.{propertyName} should have [XmlChoiceGroupAttribute]");
        return attr!;
    }

    // -- DistanceMatrixElementDerivedViewStructure: two parallel choice groups -----

    [Test]
    public void Should_have_choice_group_on_start_stop_point_ref()
    {
        var attr = GetRequiredChoiceGroupAttribute<DistanceMatrixElementDerivedViewStructure>(
            nameof(DistanceMatrixElementDerivedViewStructure.StartStopPointRef));

        attr.ArmId.Should().Be(0);
    }

    [Test]
    public void Should_have_choice_group_on_start_tariff_zone_ref()
    {
        var attr = GetRequiredChoiceGroupAttribute<DistanceMatrixElementDerivedViewStructure>(
            nameof(DistanceMatrixElementDerivedViewStructure.StartTariffZoneRef));

        attr.ArmId.Should().Be(1);
    }

    [Test]
    public void Should_have_same_group_id_for_start_choice()
    {
        var stopRef = GetRequiredChoiceGroupAttribute<DistanceMatrixElementDerivedViewStructure>(
            nameof(DistanceMatrixElementDerivedViewStructure.StartStopPointRef));
        var zoneRef = GetRequiredChoiceGroupAttribute<DistanceMatrixElementDerivedViewStructure>(
            nameof(DistanceMatrixElementDerivedViewStructure.StartTariffZoneRef));

        stopRef.GroupId.Should().Be(zoneRef.GroupId);
    }

    [Test]
    public void Should_have_choice_group_on_end_stop_point_ref()
    {
        var attr = GetRequiredChoiceGroupAttribute<DistanceMatrixElementDerivedViewStructure>(
            nameof(DistanceMatrixElementDerivedViewStructure.EndStopPointRef));

        attr.ArmId.Should().Be(0);
    }

    [Test]
    public void Should_have_choice_group_on_end_tariff_zone_ref()
    {
        var attr = GetRequiredChoiceGroupAttribute<DistanceMatrixElementDerivedViewStructure>(
            nameof(DistanceMatrixElementDerivedViewStructure.EndTariffZoneRef));

        attr.ArmId.Should().Be(1);
    }

    [Test]
    public void Should_have_same_group_id_for_end_choice()
    {
        var stopRef = GetRequiredChoiceGroupAttribute<DistanceMatrixElementDerivedViewStructure>(
            nameof(DistanceMatrixElementDerivedViewStructure.EndStopPointRef));
        var zoneRef = GetRequiredChoiceGroupAttribute<DistanceMatrixElementDerivedViewStructure>(
            nameof(DistanceMatrixElementDerivedViewStructure.EndTariffZoneRef));

        stopRef.GroupId.Should().Be(zoneRef.GroupId);
    }

    [Test]
    public void Should_have_different_group_ids_for_start_and_end_choices()
    {
        var startAttr = GetRequiredChoiceGroupAttribute<DistanceMatrixElementDerivedViewStructure>(
            nameof(DistanceMatrixElementDerivedViewStructure.StartStopPointRef));
        var endAttr = GetRequiredChoiceGroupAttribute<DistanceMatrixElementDerivedViewStructure>(
            nameof(DistanceMatrixElementDerivedViewStructure.EndStopPointRef));

        startAttr.GroupId.Should().NotBe(endAttr.GroupId);
    }

    [Test]
    public void Should_not_have_choice_group_on_non_choice_property()
    {
        // StartName is not part of any choice group
        var attr = GetChoiceGroupAttribute<DistanceMatrixElementDerivedViewStructure>("StartName");

        attr.Should().BeNull();
    }

    // -- FareStructureElementVersionStructure: multiple choice groups in NeTEx -----

    [Test]
    public void Should_have_multiple_distinct_choice_groups_on_fare_structure_element()
    {
        var properties = typeof(FareStructureElementVersionStructure).GetProperties();
        var choiceGroupIds = properties
            .Select(p => p.GetCustomAttribute<XmlChoiceGroupAttribute>())
            .Where(a => a is not null)
            .Select(a => a!.GroupId)
            .Distinct()
            .ToList();

        choiceGroupIds.Count.Should().BeGreaterThanOrEqualTo(2);
    }
}
