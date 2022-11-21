using CustomizeAnimals.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizeAnimals
{
	public class GlobalSettings : IExposable
	{
		#region PROPERTIES
		public static GeneralSettings GlobalGeneralSettings { get; } = new GeneralSettings();
		public static Dictionary<string, ISettingWithGlobal> GeneralSettings { get; } = new Dictionary<string, ISettingWithGlobal>();
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
			GeneralSettings.Add("MateMtbHours", new SettingMateMtbHours(null, true));
			GeneralSettings.Add("GestationPeriodDays", new SettingGestationPeriodDays(null, true));

			// Check
			foreach (var pair in GeneralSettings)
			{
				var count = GeneralSettings.Count((p) => p.Value.GetType() == pair.Value.GetType());
				if (count != 1)
				{
					Log.Error($"{nameof(CustomizeAnimals)}.{nameof(GlobalSettings)}.{nameof(Initialize)}: " +
						$"duplicate: '{pair.Key}'/'{pair.Value?.GetType()}' ({count})");
				}
			}
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
}
