using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizeAnimals.Settings;
using RimWorld;
using Verse;

namespace CustomizeAnimals.Settings
{
	public class GlobalSettings : IExposable
	{
		#region PROPERTIES
		public Dictionary<string, ISetting> Settings { get; } = new Dictionary<string, ISetting>();
		#endregion

		#region CONSTRUCTORS
		public GlobalSettings()
		{
			Settings.Add("Trainability", new SettingTrainability(null));
			Settings.Add("RoamMtbDays", new SettingRoamMtbDays(null));
		}
		#endregion

		#region PUBLIC METHODS
		public void ApplySettings()
		{
			foreach (var animal in CustomizeAnimals.Animals)
				animal.ApplySettings();
		}
		public void Reset()
		{
			foreach (var item in Settings.Values)
				item.ResetGlobal();
		}

		public bool IsGlobalUsed()
		{
			foreach (var item in Settings.Values)
				if (item.IsGlobalUsed())
					return true;
			return false;
		}
		#endregion

		#region INTERFACES
		public void ExposeData()
		{
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
			Animal = animal ?? throw new Exception($"CustomizeAnimals.{nameof(AnimalSettings)}: 'Animal' should not be null!");

			Settings.Add("Trainability", new SettingTrainability(Animal));
			Settings.Add("RoamMtbDays", new SettingRoamMtbDays(Animal));

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
