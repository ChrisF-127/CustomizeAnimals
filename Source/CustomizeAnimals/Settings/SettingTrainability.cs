using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizeAnimals.Settings
{
	public class SettingTrainability : BaseSetting<TrainabilityDef>
	{
		public static bool UseMinimumTrainability { get; set; }
		public static TrainabilityDef MinimumTrainability { get; set; }

		public SettingTrainability(ThingDef animal) : base(animal)
		{ }

		public override TrainabilityDef Get()
		{
			return Animal?.race?.trainability;
		}
		public override void Set()
		{
			if (Animal?.race != null)
				Animal.race.trainability = UseMinimumTrainability && ToInt(MinimumTrainability) > ToInt(Value) ? MinimumTrainability : Value;
			Log.Message($"SettingTrainability.Set: {Animal.defName} {Animal.race.trainability}");
		}

		public override void ExposeData()
		{
			var trainability = Def2String(Value);
			Scribe_Values.Look(ref trainability, "Trainability", Def2String(DefaultValue));
			Value = trainability != null && trainability != "null" ? DefDatabase<TrainabilityDef>.GetNamed(trainability) : null;
		}
		public static void ExposeGlobal()
		{
			var useGlobal = UseMinimumTrainability;
			Scribe_Values.Look(ref useGlobal, "UseMinimumTrainability");
			UseMinimumTrainability = useGlobal;

			var trainability = Def2String(MinimumTrainability);
			Scribe_Values.Look(ref trainability, "MinimumTrainability", "null");
			MinimumTrainability = trainability != null && trainability != "null" ? DefDatabase<TrainabilityDef>.GetNamed(trainability) : null;
		}

		public static int ToInt(TrainabilityDef trainability)
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
}
