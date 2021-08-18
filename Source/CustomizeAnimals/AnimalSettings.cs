using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace CustomizeAnimals
{
	public class AnimalSettings : IExposable
	{
		#region PROPERTIES
		public ThingDef Animal { get; private set; }

		public SettingTrainability Trainability { get; }
		public SettingRoamMtbDays RoamMtbDays { get; }
		#endregion

		#region CONSTRUCTORS
		public AnimalSettings(ThingDef animal)
		{
			if (!IsValidAnimal(animal))
				throw new Exception(($"CustomizeAnimal.{nameof(AnimalSettings)}: invalid thing added as {nameof(AnimalSettings)}: {animal.label.CapitalizeFirst()} ({animal.defName})"));

			Animal = animal;

			Trainability = new SettingTrainability(Animal);
			RoamMtbDays = new SettingRoamMtbDays(Animal);
		}
		public AnimalSettings(AnimalSettings animalSettings)
		{
			Animal = animalSettings.Animal;

			Trainability = animalSettings.Trainability;
			RoamMtbDays = animalSettings.RoamMtbDays;
		}
		#endregion

		#region PUBLIC METHODS
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
			var trainability = Def2String(Trainability.Value);
			Scribe_Values.Look(ref trainability, nameof(Trainability), Def2String(Trainability.DefaultValue));
			Trainability.Value = trainability != null ? DefDatabase<TrainabilityDef>.GetNamed(trainability) : null;

			var roamMtbDays = RoamMtbDays.Value;
			Scribe_Values.Look(ref roamMtbDays, nameof(RoamMtbDays), RoamMtbDays.DefaultValue);
			RoamMtbDays.Value = roamMtbDays;
		}
		#endregion

		#region PRIVATE METHODS
		private string Def2String(Def def) =>
			def?.defName ?? "null";
		#endregion
	}

	#region SUB SETTINGS
	public class SettingTrainability : Setting<TrainabilityDef>
	{
		public override TrainabilityDef Value
		{
			get => Animal?.race?.trainability;
			set { if (Animal?.race != null) Animal.race.trainability = value; }
		}

		public SettingTrainability(ThingDef animal) : base(animal)
		{
			DefaultValue = animal?.race?.trainability;
		}
	}
	public class SettingRoamMtbDays : Setting<float?>
	{
		public override float? Value
		{
			get => Animal?.race?.roamMtbDays;
			set { if (Animal?.race != null) Animal.race.roamMtbDays = value; }
		}

		public SettingRoamMtbDays(ThingDef animal) : base(animal)
		{
			DefaultValue = animal?.race?.roamMtbDays;
		}
	}

	public abstract class Setting<T>
	{
		public ThingDef Animal { get; }
		public T DefaultValue { get; protected set; }
		public abstract T Value { get; set; }
		
		public Setting(ThingDef animal)
		{
			Animal = animal;
		}

		public bool IsModified() =>
			!(DefaultValue == null && Value == null || DefaultValue?.Equals(Value) == true);
		public void Reset() =>
			Value = DefaultValue;
	}
	#endregion
}
