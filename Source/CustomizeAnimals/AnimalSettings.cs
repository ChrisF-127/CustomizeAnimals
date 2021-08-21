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
			Trainability.ExposeData();
			RoamMtbDays.ExposeData();
		}
		#endregion
	}

	#region SUB SETTINGS
	public class SettingTrainability : Setting<TrainabilityDef>
	{
		public SettingTrainability(ThingDef animal) : base(animal)
		{
			DefaultValue = animal?.race?.trainability;
		}

		public override TrainabilityDef Get()
		{
			return Animal?.race?.trainability;
		}
		public override void Set()
		{
			if (Animal?.race != null)
				Animal.race.trainability = UseGlobal && TrainabilityToInt(Global) > TrainabilityToInt(Value) ? Global : Value;
		}

		public override void ExposeData()
		{
			var trainability = Def2String(Value);
			Scribe_Values.Look(ref trainability, "Trainability", Def2String(DefaultValue));
			Value = trainability != null && trainability != "null" ? DefDatabase<TrainabilityDef>.GetNamed(trainability) : null;
			Set();
		}

		public int TrainabilityToInt(TrainabilityDef trainability)
		{
			if (trainability == TrainabilityDefOf.None)
				return 0;
			if (trainability == TrainabilityDefOf.Intermediate)
				return 1;
			if (trainability == TrainabilityDefOf.Advanced)
				return 2;
			return -1;
		}
	}
	public class SettingRoamMtbDays : Setting<float?>
	{
		public SettingRoamMtbDays(ThingDef animal) : base(animal)
		{
			DefaultValue = animal?.race?.roamMtbDays;
		}

		public override float? Get()
		{
			return Animal?.race?.roamMtbDays;
		}
		public override void Set()
		{
			if (Animal?.race != null)
				Animal.race.roamMtbDays = UseGlobal ? Global > 0 ? Value > Global ? Value : Global : null : Value > 0 ? Value : null; // don't ask.
		}

		public override void ExposeData()
		{
			Log.Message($"{Animal?.defName} {Scribe.mode}");
			var roamMtbDays = Value;
			Scribe_Values.Look(ref roamMtbDays, "RoamMtbDays", DefaultValue);
			Value = roamMtbDays;
			Set();
		}
	}

	public abstract class Setting<T> : IExposable
	{
		public static bool UseGlobal { get; set; }
		public static T Global { get; set; }

		public ThingDef Animal { get; }
		public T Value { get; set; }
		public T DefaultValue { get; protected set; }

		public Setting(ThingDef animal)
		{
			Animal = animal;
			Set();
		}

		public abstract T Get();
		public abstract void Set();
		public abstract void ExposeData();

		public bool IsModified() =>
			!(DefaultValue == null && Value == null || DefaultValue?.Equals(Value) == true);
		public void Reset() =>
			Value = DefaultValue;
		
		protected string Def2String(Def def) =>
			def?.defName ?? "null";
	}
	#endregion
}
