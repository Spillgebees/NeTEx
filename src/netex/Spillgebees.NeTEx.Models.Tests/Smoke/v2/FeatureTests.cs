using AwesomeAssertions;
using Spillgebees.NeTEx.Models.V2_0_0.NeTEx;

namespace Spillgebees.NeTEx.Models.Tests.Smoke.v2;

public class FeatureTests
{
    [Test]
    public void Should_have_propulsion_type_enumeration_with_expected_values()
    {
        // assert
        typeof(PropulsionTypeEnumeration).IsEnum.Should().BeTrue();

        Enum.IsDefined(PropulsionTypeEnumeration.Combustion)
            .Should().BeTrue();
        Enum.IsDefined(PropulsionTypeEnumeration.Electric)
            .Should().BeTrue();
        Enum.IsDefined(PropulsionTypeEnumeration.ElectricAssist)
            .Should().BeTrue();
        Enum.IsDefined(PropulsionTypeEnumeration.Hybrid)
            .Should().BeTrue();
        Enum.IsDefined(PropulsionTypeEnumeration.Human)
            .Should().BeTrue();
        Enum.IsDefined(PropulsionTypeEnumeration.Other)
            .Should().BeTrue();
    }

    [Test]
    public void Should_have_fuel_type_enumeration_with_expected_values()
    {
        // assert
        typeof(FuelTypeEnumeration).IsEnum.Should().BeTrue();

        Enum.IsDefined(FuelTypeEnumeration.Battery)
            .Should().BeTrue();
        Enum.IsDefined(FuelTypeEnumeration.Diesel)
            .Should().BeTrue();
        Enum.IsDefined(FuelTypeEnumeration.Hydrogen)
            .Should().BeTrue();
        Enum.IsDefined(FuelTypeEnumeration.Electricity)
            .Should().BeTrue();
        Enum.IsDefined(FuelTypeEnumeration.PetrolBatteryHybrid)
            .Should().BeTrue();
        Enum.IsDefined(FuelTypeEnumeration.None)
            .Should().BeTrue();
    }

    [Test]
    public void Should_have_deck_plan_types()
    {
        // assert
        typeof(DeckPlan).IsClass.Should().BeTrue();
        typeof(DeckPlanVersionStructure).IsClass.Should().BeTrue();
        typeof(DeckPlan).Should().BeDerivedFrom<DeckPlanVersionStructure>();
    }

    [Test]
    public void Should_have_multilingual_string_with_text_child_elements_and_mixed_content()
    {
        // arrange & act
        var textProperty = typeof(MultilingualString).GetProperty(nameof(MultilingualString.Text));
        var textMixedProperty = typeof(MultilingualString).GetProperty(nameof(MultilingualString.Text_1));

        // assert
        textProperty.Should().NotBeNull();
        textProperty.PropertyType.Should().Be<List<TextType>>();

        textMixedProperty.Should().NotBeNull();
        textMixedProperty.PropertyType.Should().Be<string[]>();
    }

    [Test]
    public void Should_have_private_codes_structure()
    {
        // assert
        typeof(PrivateCodesStructure).IsClass.Should().BeTrue();

        var privateCodeProperty = typeof(PrivateCodesStructure)
            .GetProperty(nameof(PrivateCodesStructure.PrivateCode));

        privateCodeProperty.Should().NotBeNull();
        privateCodeProperty.PropertyType.Should().Be<List<PrivateCodeStructure>>();
    }

    [Test]
    public void Should_have_new_quay_type_values()
    {
        // assert
        typeof(QuayTypeEnumeration).IsEnum.Should().BeTrue();

        Enum.IsDefined(QuayTypeEnumeration.BusStopWithinRoadwayBoarding)
            .Should().BeTrue();
        Enum.IsDefined(QuayTypeEnumeration.TramStopWithinRoadwayBoarding)
            .Should().BeTrue();
    }

    [Test]
    public void Should_have_purchase_moment_enumeration_with_expected_values()
    {
        // assert
        typeof(PurchaseMomentEnumeration).IsEnum.Should().BeTrue();

        Enum.IsDefined(PurchaseMomentEnumeration.InAdvance)
            .Should().BeTrue();
        Enum.IsDefined(PurchaseMomentEnumeration.BeforeBoarding)
            .Should().BeTrue();
        Enum.IsDefined(PurchaseMomentEnumeration.OnBoarding)
            .Should().BeTrue();
        Enum.IsDefined(PurchaseMomentEnumeration.SubscriptionOnly)
            .Should().BeTrue();
        Enum.IsDefined(PurchaseMomentEnumeration.NoTicketRequired)
            .Should().BeTrue();
    }
}
