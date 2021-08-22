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
		#region PUBLIC METHODS
		public void Set()
		{
			foreach (var animal in CustomizeAnimals.Animals)
				animal.Set();
		}

		public bool IsGlobalUsed() =>
			SettingTrainability.UseMinimumTrainability
			|| SettingRoamMtbDays.UseMinimumRoamMtbDays;
		#endregion

		#region INTERFACES
		public void ExposeData()
		{
			SettingTrainability.ExposeGlobal();
			SettingRoamMtbDays.ExposeGlobal();

			Set();
		}
		#endregion
	}

	public class AnimalSettings : IExposable
	{
		#region PROPERTIES
		public ThingDef Animal { get; }

		public SettingTrainability Trainability { get; }
		public SettingRoamMtbDays RoamMtbDays { get; }
		#endregion

		#region CONSTRUCTORS
		public AnimalSettings(ThingDef animal)
		{
			Animal = animal ?? throw new Exception($"CustomizeAnimals.{nameof(AnimalSettings)}: 'Animal' should not be null!");

			Trainability = new SettingTrainability(Animal);
			RoamMtbDays = new SettingRoamMtbDays(Animal);
			
			Set();
		}
		public AnimalSettings(AnimalSettings animalSettings)
		{
			Animal = animalSettings.Animal;

			Trainability = animalSettings.Trainability;
			RoamMtbDays = animalSettings.RoamMtbDays;
		}
		#endregion

		#region PUBLIC METHODS
		public void Set()
		{
			Trainability.Set();
			RoamMtbDays.Set();
		}

		public bool IsModified() =>
			Animal != null
			&& (Trainability.IsModified() 
			|| RoamMtbDays.IsModified());

		public static bool IsValidAnimal(ThingDef thingDef) =>
			thingDef.thingCategories?.Contains(ThingCategoryDefOf.Animals) == true
			&& thingDef.race != null
			&& thingDef.race.trainability != null;
		#endregion

		#region INTERFACES
		public void ExposeData()
		{
			Trainability.ExposeData();
			RoamMtbDays.ExposeData();

			Set();
		}
		#endregion
	}
}
