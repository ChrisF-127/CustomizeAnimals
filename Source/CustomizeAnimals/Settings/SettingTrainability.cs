using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizeAnimals.Settings
{
	internal class SettingTrainability : BaseSetting<TrainabilityDef>
	{
		#region PROPERTIES
		public static bool UseMinimumTrainability { get; set; }
		public static TrainabilityDef MinimumTrainability { get; set; }
		#endregion

		#region CONSTRUCTORS
		public SettingTrainability(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
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
		#endregion

		#region INTERFACES
		public override TrainabilityDef GetValue()
		{
			return Animal?.race?.trainability;
		}
		public override void SetValue()
		{
			if (Animal?.race != null)
				Animal.race.trainability = UseMinimumTrainability && ToInt(MinimumTrainability) > ToInt(Value) ? MinimumTrainability : Value;
		}

		public override void ExposeData()
		{
			var trainability = Def2String(Value);
			Scribe_Values.Look(ref trainability, "Trainability", Def2String(DefaultValue));
			Value = trainability != null && trainability != "null" ? DefDatabase<TrainabilityDef>.GetNamed(trainability) : null;
		}

		public override void ResetGlobal()
		{
			UseMinimumTrainability = false;
			MinimumTrainability = TrainabilityDefOf.None;
		}

		public override void ExposeGlobal()
		{
			var useGlobal = UseMinimumTrainability;
			Scribe_Values.Look(ref useGlobal, "UseMinimumTrainability");
			UseMinimumTrainability = useGlobal;

			var trainability = Def2String(MinimumTrainability);
			Scribe_Values.Look(ref trainability, "MinimumTrainability", Def2String(TrainabilityDefOf.None));
			MinimumTrainability = trainability != null && trainability != "null" ? DefDatabase<TrainabilityDef>.GetNamed(trainability) : null;
		}

		public override bool IsGlobalUsed() =>
			UseMinimumTrainability;
		#endregion
	}
}
