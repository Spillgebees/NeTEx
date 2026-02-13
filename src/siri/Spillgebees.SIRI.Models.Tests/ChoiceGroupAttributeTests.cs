using System.Reflection;
using AwesomeAssertions;
using Spillgebees.SIRI.Models.V2_2;
using Spillgebees.SIRI.Models.V2_2.SIRI;

namespace Spillgebees.SIRI.Models.Tests;

/// <summary>
/// Tests verifying that the generated SIRI v2.2 models carry correct
/// <see cref="XmlChoiceGroupAttribute"/> annotations on properties that
/// originate from <c>xs:choice</c> groups in the SIRI XSD schemas.
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

    // -- MonitoredCountingStructure: simple choice (Count vs Percentage) -----------

    [Test]
    public void Should_have_choice_group_attribute_on_monitored_counting_count()
    {
        var attr = GetRequiredChoiceGroupAttribute<MonitoredCountingStructure>(
            nameof(MonitoredCountingStructure.Count));

        attr.ArmId.Should().Be(0);
    }

    [Test]
    public void Should_have_choice_group_attribute_on_monitored_counting_percentage()
    {
        var attr = GetRequiredChoiceGroupAttribute<MonitoredCountingStructure>(
            nameof(MonitoredCountingStructure.Percentage));

        attr.ArmId.Should().Be(1);
    }

    [Test]
    public void Should_have_same_group_id_for_monitored_counting_choice()
    {
        var countAttr = GetRequiredChoiceGroupAttribute<MonitoredCountingStructure>(
            nameof(MonitoredCountingStructure.Count));
        var percentageAttr = GetRequiredChoiceGroupAttribute<MonitoredCountingStructure>(
            nameof(MonitoredCountingStructure.Percentage));

        countAttr.GroupId.Should().Be(percentageAttr.GroupId);
    }

    // -- LocationStructure: sequence arm (Lon/Lat/Alt share arm, Coordinates is other) --

    [Test]
    public void Should_have_choice_group_attribute_on_location_longitude()
    {
        var attr = GetRequiredChoiceGroupAttribute<LocationStructure>(
            nameof(LocationStructure.Longitude));

        attr.ArmId.Should().Be(0);
    }

    [Test]
    public void Should_have_choice_group_attribute_on_location_latitude()
    {
        var attr = GetRequiredChoiceGroupAttribute<LocationStructure>(
            nameof(LocationStructure.Latitude));

        attr.ArmId.Should().Be(0);
    }

    [Test]
    public void Should_have_choice_group_attribute_on_location_altitude()
    {
        var attr = GetRequiredChoiceGroupAttribute<LocationStructure>(
            nameof(LocationStructure.Altitude));

        attr.ArmId.Should().Be(0);
    }

    [Test]
    public void Should_have_choice_group_attribute_on_location_coordinates()
    {
        var attr = GetRequiredChoiceGroupAttribute<LocationStructure>(
            nameof(LocationStructure.Coordinates));

        attr.ArmId.Should().Be(1);
    }

    [Test]
    public void Should_have_same_group_id_for_location_choice()
    {
        var lonAttr = GetRequiredChoiceGroupAttribute<LocationStructure>(
            nameof(LocationStructure.Longitude));
        var latAttr = GetRequiredChoiceGroupAttribute<LocationStructure>(
            nameof(LocationStructure.Latitude));
        var altAttr = GetRequiredChoiceGroupAttribute<LocationStructure>(
            nameof(LocationStructure.Altitude));
        var coordAttr = GetRequiredChoiceGroupAttribute<LocationStructure>(
            nameof(LocationStructure.Coordinates));

        // All belong to the same choice group
        lonAttr.GroupId.Should().Be(latAttr.GroupId);
        latAttr.GroupId.Should().Be(altAttr.GroupId);
        altAttr.GroupId.Should().Be(coordAttr.GroupId);

        // Lon/Lat/Alt share arm 0, Coordinates is arm 1
        lonAttr.ArmId.Should().Be(latAttr.ArmId);
        latAttr.ArmId.Should().Be(altAttr.ArmId);
        coordAttr.ArmId.Should().NotBe(lonAttr.ArmId);
    }

    // -- PlannedStopAssignmentStructure: both arms are sequences -------------------

    [Test]
    public void Should_have_choice_group_attribute_on_planned_stop_assignment_quay_arm()
    {
        var quayRef = GetRequiredChoiceGroupAttribute<PlannedStopAssignmentStructure>(
            nameof(PlannedStopAssignmentStructure.AimedQuayRef));
        var quayName = GetRequiredChoiceGroupAttribute<PlannedStopAssignmentStructure>(
            nameof(PlannedStopAssignmentStructure.AimedQuayName));
        var quayType = GetRequiredChoiceGroupAttribute<PlannedStopAssignmentStructure>(
            nameof(PlannedStopAssignmentStructure.QuayType));

        // All three share the same groupId and armId
        quayRef.GroupId.Should().Be(quayName.GroupId);
        quayName.GroupId.Should().Be(quayType.GroupId);
        quayRef.ArmId.Should().Be(0);
        quayName.ArmId.Should().Be(0);
        quayType.ArmId.Should().Be(0);
    }

    [Test]
    public void Should_have_choice_group_attribute_on_planned_stop_assignment_boarding_position_arm()
    {
        var bpRef = GetRequiredChoiceGroupAttribute<PlannedStopAssignmentStructure>(
            nameof(PlannedStopAssignmentStructure.AimedBoardingPositionRef));
        var bpName = GetRequiredChoiceGroupAttribute<PlannedStopAssignmentStructure>(
            nameof(PlannedStopAssignmentStructure.AimedBoardingPositionName));

        // Both share the same groupId and armId
        bpRef.GroupId.Should().Be(bpName.GroupId);
        bpRef.ArmId.Should().Be(1);
        bpName.ArmId.Should().Be(1);
    }

    [Test]
    public void Should_have_same_group_id_across_planned_stop_assignment_choice()
    {
        var quayRef = GetRequiredChoiceGroupAttribute<PlannedStopAssignmentStructure>(
            nameof(PlannedStopAssignmentStructure.AimedQuayRef));
        var bpRef = GetRequiredChoiceGroupAttribute<PlannedStopAssignmentStructure>(
            nameof(PlannedStopAssignmentStructure.AimedBoardingPositionRef));

        quayRef.GroupId.Should().Be(bpRef.GroupId);
    }

    // -- non-choice properties should NOT have the attribute -----------------------

    [Test]
    public void Should_not_have_choice_group_attribute_on_non_choice_property()
    {
        // MonitoredCountingStructure.CountingType is not part of any choice
        var attr = GetChoiceGroupAttribute<MonitoredCountingStructure>("CountingType");

        attr.Should().BeNull();
    }

    [Test]
    public void Should_not_have_choice_group_attribute_on_location_id()
    {
        // LocationStructure.Id is an attribute, not part of the choice
        var attr = GetChoiceGroupAttribute<LocationStructure>("Id");

        attr.Should().BeNull();
    }

    // -- ControlActionStructure: multiple choice groups in same type ---------------

    [Test]
    public void Should_have_multiple_distinct_choice_groups_on_control_action_structure()
    {
        // ControlActionStructure has 5 choice groups — verify at least 2 are distinct
        var properties = typeof(ControlActionStructure).GetProperties();
        var choiceGroupIds = properties
            .Select(p => p.GetCustomAttribute<XmlChoiceGroupAttribute>())
            .Where(a => a is not null)
            .Select(a => a!.GroupId)
            .Distinct()
            .ToList();
        choiceGroupIds.Count.Should().BeGreaterThanOrEqualTo(2);
    }
}
