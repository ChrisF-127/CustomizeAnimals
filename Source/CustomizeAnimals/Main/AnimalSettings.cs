using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizeAnimals.Settings;
using RimWorld;
using Verse;

namespace CustomizeAnimals
{
	internal class AnimalSettings : IExposable
	{
		#region PROPERTIES
		public bool IsHumanLike => Animal.IsHumanLike();
		public ThingDef Animal { get; }
		public Dictionary<string, ISettingWithGlobal> GeneralSettings { get; } = new Dictionary<string, ISettingWithGlobal>();
		public Dictionary<string, ISetting> ReproductionSettings { get; } = new Dictionary<string, ISetting>();
		public Dictionary<string, ISetting> ProductivitySettings { get; } = new Dictionary<string, ISetting>();
		#endregion

		#region CONSTRUCTORS
		public AnimalSettings(ThingDef animal)
		{
			Animal = animal ?? throw new Exception($"{nameof(CustomizeAnimals)}.{nameof(AnimalSettings)}: 'Animal' should not be null!");

			Initialize();
		}
		public AnimalSettings(AnimalSettings animalSettings)
		{
			Animal = animalSettings.Animal;

			foreach (var item in animalSettings.GeneralSettings)
				GeneralSettings.Add(item.Key, item.Value);
			foreach (var item in animalSettings.ReproductionSettings)
				ReproductionSettings.Add(item.Key, item.Value);
			foreach (var item in animalSettings.ProductivitySettings)
				ProductivitySettings.Add(item.Key, item.Value);
		}
		#endregion

		#region PUBLIC METHODS
		public void Initialize()
		{
			// General
			if (!IsHumanLike)
				GeneralSettings.Add("DrawSize", new SettingDrawSize(Animal));
			GeneralSettings.Add("MarketValue", new SettingMarketValue(Animal));
			GeneralSettings.Add("MeatAmount", new SettingMeatAmount(Animal));
			GeneralSettings.Add("LeatherAmount", new SettingLeatherAmount(Animal));
			if (!IsHumanLike)
				GeneralSettings.Add("ToxicResistance", new SettingToxicResistance(Animal));
			GeneralSettings.Add("BodySize", new SettingBodySize(Animal));
			GeneralSettings.Add("HealthScale", new SettingHealthScale(Animal));
			GeneralSettings.Add("MoveSpeed", new SettingMoveSpeed(Animal));
			GeneralSettings.Add("CarryingCapacity", new SettingCarryingCapacity(Animal));
			GeneralSettings.Add("Wildness", new SettingWildness(Animal));
			GeneralSettings.Add("LifeExpectancy", new SettingLifeExpectancy(Animal));
			GeneralSettings.Add("MaxTemperature", new SettingMaxTemperature(Animal));
			GeneralSettings.Add("MinTemperature", new SettingMinTemperature(Animal));
			GeneralSettings.Add("HungerRate", new SettingHungerRate(Animal));
			GeneralSettings.Add("FoodType", new SettingFoodType(Animal));
			GeneralSettings.Add("WillNeverEat", new SettingWillNeverEat(Animal));
			GeneralSettings.Add("ManhunterOnTameFail", new SettingManhunterOnTameFail(Animal));
			GeneralSettings.Add("ManhunterOnDamage", new SettingManhunterOnDamage(Animal));
			GeneralSettings.Add("Predator", new SettingPredator(Animal));
			GeneralSettings.Add("MaxPreyBodySize", new SettingMaxPreyBodySize(Animal));

			GeneralSettings.Add("ArmorRating_Sharp", new SettingArmorRating_Sharp(Animal));
			GeneralSettings.Add("ArmorRating_Blunt", new SettingArmorRating_Blunt(Animal));
			GeneralSettings.Add("ArmorRating_Heat", new SettingArmorRating_Heat(Animal));

			GeneralSettings.Add("AttackPowerModifier", new SettingAttackPowerModifier(Animal));
			GeneralSettings.Add("AttackCooldownModifier", new SettingAttackCooldownModifier(Animal));

			if (!IsHumanLike)
			{
				GeneralSettings.Add("Trainability", new SettingTrainability(Animal));
				GeneralSettings.Add("FilthRate", new SettingFilthRate(Animal));
				GeneralSettings.Add("CaravanRidingSpeed", new SettingCaravanRidingSpeed(Animal));
				GeneralSettings.Add("PackAnimal", new SettingPackAnimal(Animal));
				GeneralSettings.Add("RoamMtbDays", new SettingRoamMtbDays(Animal));
				GeneralSettings.Add("NuzzleMtbHours", new SettingNuzzleMtbHours(Animal));
			}

			// Check General
			foreach (var pair in GeneralSettings)
			{
				var count = GeneralSettings.Count((p) => p.Value.GetType() == pair.Value.GetType());
				if (count != 1)
				{
					Log.Error($"{nameof(CustomizeAnimals)}.{nameof(AnimalSettings)}.{nameof(Initialize)}: " +
						$"({nameof(GeneralSettings)}) duplicate: '{pair.Key}'/'{pair.Value?.GetType()}' ({count})");
				}
			}

			// Reproduction
			ReproductionSettings.Add("GestationPeriodDays", new SettingGestationPeriodDays(Animal));
			ReproductionSettings.Add("LitterSizeCurve", new SettingLitterSizeCurve(Animal));
			ReproductionSettings.Add("LifeStageAges", new SpecialSettingLifeStageAges(Animal));
			if (!IsHumanLike)
			{
				ReproductionSettings.Add("MateMtbHours", new SettingMateMtbHours(Animal));
				ReproductionSettings.Add("EggLayer", new SpecialSettingEggLayer(Animal));
			}
			else
			{
				ReproductionSettings.Add("GrowthTier", new SpecialSettingGrowthTier(Animal));
			}

			// Check Reproduction
			foreach (var pair in ReproductionSettings)
			{
				var count = ReproductionSettings.Count((p) => p.Value.GetType() == pair.Value.GetType());
				if (count != 1)
				{
					Log.Error($"{nameof(CustomizeAnimals)}.{nameof(AnimalSettings)}.{nameof(Initialize)}: " +
						$"({nameof(ReproductionSettings)}) duplicate: '{pair.Key}'/'{pair.Value?.GetType()}' ({count})");
				}
			}

			// Productivity
			if (!IsHumanLike)
			{
				ProductivitySettings.Add("Milkable", new SpecialSettingMilkable(Animal));
				ProductivitySettings.Add("Shearable", new SpecialSettingShearable(Animal));
			}

			// Check Productivity
			foreach (var pair in ProductivitySettings)
			{
				var count = ProductivitySettings.Count((p) => p.Value.GetType() == pair.Value.GetType());
				if (count != 1)
				{
					Log.Error($"{nameof(CustomizeAnimals)}.{nameof(AnimalSettings)}.{nameof(Initialize)}: " +
						$"({nameof(ProductivitySettings)}) duplicate: '{pair.Key}'/'{pair.Value?.GetType()}' ({count})");
				}
			}

			// Preset values
			ApplySettings();
		}

		public void ApplySettings()
		{
			foreach (var item in GeneralSettings.Values)
				item.SetValue();
			foreach (var item in ReproductionSettings.Values)
				item.SetValue();
			foreach (var item in ProductivitySettings.Values)
				item.SetValue();
		}
		public void Reset()
		{
			foreach (var item in GeneralSettings.Values)
				item.Reset();
			foreach (var item in ReproductionSettings.Values)
				item.Reset();
			foreach (var item in ProductivitySettings.Values)
				item.Reset();
		}

		public bool IsModified()
		{
			foreach (var item in GeneralSettings.Values)
				if (item.IsModified())
					return true;
			foreach (var item in ReproductionSettings.Values)
				if (item.IsModified())
					return true;
			foreach (var item in ProductivitySettings.Values)
				if (item.IsModified())
					return true;
			return false;
		}

		public static bool IsValidAnimal(ThingDef thingDef) =>
			thingDef.thingCategories?.Contains(ThingCategoryDefOf.Animals) == true	// ANIMALS should have thing category "Animals"
			&& thingDef.race != null												// all ANIMALS should have a race
			&& thingDef.race.trainability != null;									// all ANIMALS have trainability, assuming that everything else is NOT an ANIMAL
		#endregion

		#region INTERFACES
		public void ExposeData()
		{
			foreach (var item in GeneralSettings.Values)
				item.ExposeData();
			foreach (var item in ReproductionSettings.Values)
				item.ExposeData();
			foreach (var item in ProductivitySettings.Values)
				item.ExposeData();

			ApplySettings();
		}
		#endregion
	}
}
