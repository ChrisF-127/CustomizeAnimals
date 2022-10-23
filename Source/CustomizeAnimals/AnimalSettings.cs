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
	public class GlobalSettings : IExposable
	{
		#region PROPERTIES
		public static GeneralSettings GlobalGeneralSettings { get; } = new GeneralSettings();
		public static Dictionary<string, ISettingWithGlobal> GeneralSettings { get; } = new Dictionary<string, ISettingWithGlobal>();
		#endregion

		#region CONSTRUCTORS
		#endregion

		#region PUBLIC METHODS
		public static void Initialize()
		{
			GlobalGeneralSettings.Initialize();

			GeneralSettings.Add("DrawSize", new SettingDrawSize(null, true));
			GeneralSettings.Add("MarketValue", new SettingMarketValue(null, true));
			GeneralSettings.Add("MeatAmount", new SettingMeatAmount(null, true));
			GeneralSettings.Add("LeatherAmount", new SettingLeatherAmount(null, true));
			GeneralSettings.Add("ToxicResistance", new SettingToxicResistance(null, true));
			GeneralSettings.Add("BodySize", new SettingBodySize(null, true));
			GeneralSettings.Add("HealthScale", new SettingHealthScale(null, true));
			GeneralSettings.Add("MoveSpeed", new SettingMoveSpeed(null, true)); 
			GeneralSettings.Add("Trainability", new SettingTrainability(null, true));
			GeneralSettings.Add("FilthRate", new SettingFilthRate(null, true));
			GeneralSettings.Add("CaravanRidingSpeed", new SettingCaravanRidingSpeed(null, true));
			GeneralSettings.Add("CarryingCapacity", new SettingCarryingCapacity(null, true));
			//GeneralSettings.Add("PackAnimal", new SettingPackAnimal(null, true));
			GeneralSettings.Add("RoamMtbDays", new SettingRoamMtbDays(null, true));
			//GeneralSettings.Add("Wildness", new SettingWildness(null, true)); 
			GeneralSettings.Add("LifeExpectancy", new SettingLifeExpectancy(null, true)); 
			GeneralSettings.Add("MaxTemperature", new SettingMaxTemperature(null, true));
			GeneralSettings.Add("MinTemperature", new SettingMinTemperature(null, true));
			GeneralSettings.Add("HungerRate", new SettingHungerRate(null, true));
			//GeneralSettings.Add("FoodType", new SettingFoodType(null, true));
			GeneralSettings.Add("WillNeverEat", new SettingWillNeverEat(null, true));
			GeneralSettings.Add("ManhunterOnTameFail", new SettingManhunterOnTameFail(null, true));
			GeneralSettings.Add("ManhunterOnDamage", new SettingManhunterOnDamage(null, true));
			//GeneralSettings.Add("Predator", new SettingPredator(null, true));
			//GeneralSettings.Add("MaxPreyBodySize", new SettingMaxPreyBodySize(null, true));
			GeneralSettings.Add("NuzzleMtbHours", new SettingNuzzleMtbHours(null, true));

			// Combat
			//GeneralSettings.Add("ArmorRating_Sharp", new SettingArmorRating_Sharp(null, true));
			//GeneralSettings.Add("ArmorRating_Blunt", new SettingArmorRating_Blunt(null, true));
			//GeneralSettings.Add("ArmorRating_Heat", new SettingArmorRating_Heat(null, true));
			GeneralSettings.Add("AttackPowerModifier", new SettingAttackPowerModifier(null, true));
			GeneralSettings.Add("AttackCooldownModifier", new SettingAttackCooldownModifier(null, true));

			// Reproduction
			GeneralSettings.Add("MateMtbHours", new SettingGestationPeriodDays(null, true));
			GeneralSettings.Add("GestationPeriodDays", new SettingGestationPeriodDays(null, true));
		}

		public void ApplySettings()
		{
			GlobalGeneralSettings.ApplySettings();
			foreach (var animal in CustomizeAnimals.Animals)
				animal.ApplySettings();
		}
		public void Reset()
		{
			GlobalGeneralSettings.Reset();
			foreach (var item in GeneralSettings.Values)
				item.ResetGlobal();
		}

		public bool IsGlobalUsed()
		{
			if (GlobalGeneralSettings.IsModified())
				return true;
			foreach (var item in GeneralSettings.Values)
				if (item.IsGlobalUsed())
					return true;
			return false;
		}
		#endregion

		#region INTERFACES
		public void ExposeData()
		{
			GlobalGeneralSettings.ExposeData();
			foreach (var item in GeneralSettings.Values)
				item.ExposeGlobal();

			ApplySettings();
		}
		#endregion
	}

	internal class AnimalSettings : IExposable
	{
		#region PROPERTIES
		public bool IsHuman => Animal == ThingDefOf.Human;
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
			GeneralSettings.Add("DrawSize", new SettingDrawSize(Animal));
			GeneralSettings.Add("MarketValue", new SettingMarketValue(Animal));
			GeneralSettings.Add("MeatAmount", new SettingMeatAmount(Animal));
			GeneralSettings.Add("LeatherAmount", new SettingLeatherAmount(Animal));
			if (!IsHuman)
			{
				GeneralSettings.Add("ToxicResistance", new SettingToxicResistance(Animal));
				GeneralSettings.Add("BodySize", new SettingBodySize(Animal));
			}
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

			if (!IsHuman)
			{
				GeneralSettings.Add("Trainability", new SettingTrainability(Animal));
				GeneralSettings.Add("FilthRate", new SettingFilthRate(Animal));
				GeneralSettings.Add("CaravanRidingSpeed", new SettingCaravanRidingSpeed(Animal));
				GeneralSettings.Add("PackAnimal", new SettingPackAnimal(Animal));
				GeneralSettings.Add("RoamMtbDays", new SettingRoamMtbDays(Animal));
				GeneralSettings.Add("NuzzleMtbHours", new SettingNuzzleMtbHours(Animal));
			}

			// Reproduction
			ReproductionSettings.Add("GestationPeriodDays", new SettingGestationPeriodDays(Animal));
			ReproductionSettings.Add("LitterSizeCurve", new SettingLitterSizeCurve(Animal));
			ReproductionSettings.Add("LifeStageAges", new SpecialSettingLifeStageAges(Animal));
			if (!IsHuman)
			{
				ReproductionSettings.Add("MateMtbHours", new SettingMateMtbHours(Animal));
				ReproductionSettings.Add("EggLayer", new SpecialSettingEggLayer(Animal));
			}

			// Productivity
			if (!IsHuman)
			{
				ProductivitySettings.Add("Milkable", new SpecialSettingMilkable(Animal));
				ProductivitySettings.Add("Shearable", new SpecialSettingShearable(Animal));
			}

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
