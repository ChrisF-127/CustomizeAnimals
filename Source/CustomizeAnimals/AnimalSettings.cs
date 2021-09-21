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
		public static GeneralSettings GeneralSettings { get; } = new GeneralSettings();
		public static Dictionary<string, ISetting> Settings { get; } = new Dictionary<string, ISetting>();
		#endregion

		#region CONSTRUCTORS
		#endregion

		#region PUBLIC METHODS
		public static void Initialize()
		{
			GeneralSettings.Initialize();

			//Settings.Add("MarketValue", new SettingMarketValue(null, true));
			//Settings.Add("MeatAmount", new SettingMeatAmount(null, true));
			//Settings.Add("LeatherAmount", new SettingLeatherAmount(null, true));
			Settings.Add("ToxicSensitivity", new SettingToxicSensitivity(null, true));
			//Settings.Add("BodySize", new SettingBodySize(null, true));
			//Settings.Add("HealthScale", new SettingHealthScale(null, true));
			//Settings.Add("MoveSpeed", new SettingMoveSpeed(null, true)); 
			Settings.Add("Trainability", new SettingTrainability(null, true));
			Settings.Add("FilthRate", new SettingFilthRate(null, true));
			//Settings.Add("CaravanRidingSpeed", new SettingCaravanRidingSpeed(null, true));
			Settings.Add("CarryingCapacity", new SettingCarryingCapacity(null, true));
			//Settings.Add("PackAnimal", new SettingPackAnimal(null, true));
			Settings.Add("RoamMtbDays", new SettingRoamMtbDays(null, true));
			//Settings.Add("Wildness", new SettingWildness(null, true)); 
			//Settings.Add("LifeExpectancy", new SettingLifeExpectancy(null, true)); 
			Settings.Add("MaxTemperature", new SettingMaxTemperature(null, true));
			Settings.Add("MinTemperature", new SettingMinTemperature(null, true));
			Settings.Add("HungerRate", new SettingHungerRate(null, true));
			//Settings.Add("FoodType", new SettingFoodType(null, true));
			Settings.Add("WillNeverEat", new SettingWillNeverEat(null, true));
			Settings.Add("ManhunterOnTameFail", new SettingManhunterOnTameFail(null, true));
			Settings.Add("ManhunterOnDamage", new SettingManhunterOnDamage(null, true));
			//Settings.Add("Predator", new SettingPredator(null, true));
			//Settings.Add("MaxPreyBodySize", new SettingMaxPreyBodySize(null, true));
			Settings.Add("NuzzleMtbHours", new SettingNuzzleMtbHours(null, true));

			//Settings.Add("ArmorRating_Sharp", new SettingArmorRating_Sharp(null, true));
			//Settings.Add("ArmorRating_Blunt", new SettingArmorRating_Blunt(null, true));
			//Settings.Add("ArmorRating_Heat", new SettingArmorRating_Heat(null, true));

			Settings.Add("AttackPowerModifier", new SettingAttackPowerModifier(null, true));
			Settings.Add("AttackCooldownModifier", new SettingAttackCooldownModifier(null, true));
		}

		public void ApplySettings()
		{
			GeneralSettings.ApplySettings();
			foreach (var animal in CustomizeAnimals.Animals)
				animal.ApplySettings();
		}
		public void Reset()
		{
			GeneralSettings.Reset();
			foreach (var item in Settings.Values)
				item.ResetGlobal();
		}

		public bool IsGlobalUsed()
		{
			if (GeneralSettings.IsModified())
				return true;
			foreach (var item in Settings.Values)
				if (item.IsGlobalUsed())
					return true;
			return false;
		}
		#endregion

		#region INTERFACES
		public void ExposeData()
		{
			GeneralSettings.ExposeData();
			foreach (var item in Settings.Values)
				item.ExposeGlobal();

			ApplySettings();
		}
		#endregion
	}

	internal class AnimalSettings : IExposable
	{
		#region PROPERTIES
		public ThingDef Animal { get; }
		public Dictionary<string, ISetting> Settings { get; } = new Dictionary<string, ISetting>();
		public Dictionary<string, BaseSpecialSetting> SpecialSettings { get; } = new Dictionary<string, BaseSpecialSetting>();
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

			foreach (var item in animalSettings.Settings)
				Settings.Add(item.Key, item.Value);
			foreach (var item in animalSettings.SpecialSettings)
				SpecialSettings.Add(item.Key, item.Value);
		}
		#endregion

		#region PUBLIC METHODS
		public void Initialize()
		{
			Settings.Add("MarketValue", new SettingMarketValue(Animal));
			Settings.Add("MeatAmount", new SettingMeatAmount(Animal));
			Settings.Add("LeatherAmount", new SettingLeatherAmount(Animal));
			Settings.Add("ToxicSensitivity", new SettingToxicSensitivity(Animal));
			Settings.Add("BodySize", new SettingBodySize(Animal));
			Settings.Add("HealthScale", new SettingHealthScale(Animal));
			Settings.Add("MoveSpeed", new SettingMoveSpeed(Animal));
			Settings.Add("Trainability", new SettingTrainability(Animal));
			Settings.Add("FilthRate", new SettingFilthRate(Animal));
			Settings.Add("CaravanRidingSpeed", new SettingCaravanRidingSpeed(Animal));
			Settings.Add("CarryingCapacity", new SettingCarryingCapacity(Animal));
			Settings.Add("PackAnimal", new SettingPackAnimal(Animal));
			Settings.Add("RoamMtbDays", new SettingRoamMtbDays(Animal));
			Settings.Add("Wildness", new SettingWildness(Animal));
			Settings.Add("LifeExpectancy", new SettingLifeExpectancy(Animal));
			Settings.Add("MaxTemperature", new SettingMaxTemperature(Animal));
			Settings.Add("MinTemperature", new SettingMinTemperature(Animal));
			Settings.Add("HungerRate", new SettingHungerRate(Animal));
			Settings.Add("FoodType", new SettingFoodType(Animal));
			Settings.Add("WillNeverEat", new SettingWillNeverEat(Animal));
			Settings.Add("ManhunterOnTameFail", new SettingManhunterOnTameFail(Animal));
			Settings.Add("ManhunterOnDamage", new SettingManhunterOnDamage(Animal));
			Settings.Add("Predator", new SettingPredator(Animal));
			Settings.Add("MaxPreyBodySize", new SettingMaxPreyBodySize(Animal));
			Settings.Add("NuzzleMtbHours", new SettingNuzzleMtbHours(Animal));

			Settings.Add("ArmorRating_Sharp", new SettingArmorRating_Sharp(Animal));
			Settings.Add("ArmorRating_Blunt", new SettingArmorRating_Blunt(Animal));
			Settings.Add("ArmorRating_Heat", new SettingArmorRating_Heat(Animal));

			Settings.Add("AttackPowerModifier", new SettingAttackPowerModifier(Animal));
			Settings.Add("AttackCooldownModifier", new SettingAttackCooldownModifier(Animal));

			SpecialSettings.Add("LifeStageAges", new SpecialSettingLifeStageAges(Animal));

			ApplySettings();
		}

		public void ApplySettings()
		{
			foreach (var item in Settings.Values)
				item.SetValue();
			foreach (var item in SpecialSettings.Values)
				item.SetValue();
		}
		public void Reset()
		{
			foreach (var item in Settings.Values)
				item.Reset();
			foreach (var item in SpecialSettings.Values)
				item.Reset();
		}

		public bool IsModified()
		{
			foreach (var item in Settings.Values)
				if (item.IsModified())
					return true;
			foreach (var item in SpecialSettings.Values)
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
			foreach (var item in Settings.Values)
				item.ExposeData();
			foreach (var item in SpecialSettings.Values)
				item.ExposeData();

			ApplySettings();
		}
		#endregion
	}
}
