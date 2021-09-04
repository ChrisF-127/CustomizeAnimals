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
		public Dictionary<string, ISetting> Settings { get; } = new Dictionary<string, ISetting>();
		#endregion

		#region CONSTRUCTORS
		public GlobalSettings()
		{
			//Settings.Add("BodySize", new SettingBodySize(null, true)); // no global settings
			Settings.Add("Trainability", new SettingTrainability(null, true));
			Settings.Add("FilthRate", new SettingFilthRate(null, true));
			Settings.Add("RoamMtbDays", new SettingRoamMtbDays(null, true));
			//Settings.Add("Wildness", new SettingWildness(null, true)); // no global settings
			Settings.Add("MaxTemperature", new SettingMaxTemperature(null, true));
			Settings.Add("MinTemperature", new SettingMinTemperature(null, true));
			Settings.Add("HungerRate", new SettingHungerRate(null, true));
		}
		#endregion

		#region PUBLIC METHODS
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

	public class AnimalSettings : IExposable
	{
		#region PROPERTIES
		public ThingDef Animal { get; }
		public Dictionary<string, ISetting> Settings { get; } = new Dictionary<string, ISetting>();
		#endregion

		#region CONSTRUCTORS
		public AnimalSettings(ThingDef animal)
		{
			Animal = animal ?? throw new Exception($"{nameof(CustomizeAnimals)}.{nameof(AnimalSettings)}: 'Animal' should not be null!");

			Settings.Add("BodySize", new SettingBodySize(Animal));
			Settings.Add("Trainability", new SettingTrainability(Animal));
			Settings.Add("FilthRate", new SettingFilthRate(Animal));
			Settings.Add("RoamMtbDays", new SettingRoamMtbDays(Animal));
			Settings.Add("Wildness", new SettingWildness(Animal));
			Settings.Add("MaxTemperature", new SettingMaxTemperature(Animal));
			Settings.Add("MinTemperature", new SettingMinTemperature(Animal));
			Settings.Add("HungerRate", new SettingHungerRate(Animal));

			ApplySettings();
		}
		public AnimalSettings(AnimalSettings animalSettings)
		{
			Animal = animalSettings.Animal;

			foreach (var item in animalSettings.Settings)
				Settings.Add(item.Key, item.Value);
		}
		#endregion

		#region PUBLIC METHODS
		public void ApplySettings()
		{
			foreach (var item in Settings.Values)
				item.SetValue();
		}
		public void Reset()
		{
			foreach (var item in Settings.Values)
				item.Reset();
		}

		public bool IsModified()
		{
			if (Animal != null)
				foreach (var item in Settings.Values)
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
			foreach (var item in Settings)
				item.Value.ExposeData();

			ApplySettings();
		}
		#endregion
	}
}
