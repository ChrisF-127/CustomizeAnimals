using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizeAnimals.Settings
{
	internal class SpecialSettingEggLayer : BaseSpecialSetting
	{
		#region PROPERTIES
		public CompProperties_EggLayer EggLayer { get; private set; }
		public bool IsEggLayer => EggLayer != null;

		public ThingDef FertilizedDef => EggLayer?.eggFertilizedDef;
		public ThingDef UnfertilizedDef => EggLayer?.eggUnfertilizedDef;
		public bool IsDefaultUnfertilizedDefNull { get; private set; }

		public bool DefaultFemaleOnly { get; private set; }
		public bool FemaleOnly { get; set; }
		public float DefaultIntervalDays { get; private set; }
		public float IntervalDays { get; set; }
		public int DefaultCountRangeMin { get; private set; }
		public int CountRangeMin { get; set; }
		public int DefaultCountRangeMax { get; private set; }
		public int CountRangeMax { get; set; }
		public int DefaultFertilizationCountMax { get; private set; }
		public int FertilizationCountMax { get; set; }
		public float DefaultProgressUnfertilizedMax { get; private set; }
		public float ProgressUnfertilizedMax { get; set; }

		public CompProperties_Hatcher Hatcher { get; private set; }

		public float DefaultDaysToHatch { get; private set; }
		public float DaysToHatch { get; set; }
		#endregion

		#region CONSTRUCTORS
		public SpecialSettingEggLayer(ThingDef animal) : base(animal)
		{
			GetValue();

			DefaultFemaleOnly = FemaleOnly;
			DefaultIntervalDays = IntervalDays;
			DefaultCountRangeMin = CountRangeMin;
			DefaultCountRangeMax = CountRangeMax;
			DefaultFertilizationCountMax = FertilizationCountMax;
			DefaultProgressUnfertilizedMax = ProgressUnfertilizedMax;

			DefaultDaysToHatch = DaysToHatch;
		}
		#endregion

		#region OVERRIDES
		public override void GetValue()
		{
			EggLayer = Animal?.comps?.Find((comp) => comp is CompProperties_EggLayer) as CompProperties_EggLayer;
			if (IsEggLayer)
			{
				FemaleOnly = EggLayer.eggLayFemaleOnly;
				IntervalDays = EggLayer.eggLayIntervalDays;
				CountRangeMin = EggLayer.eggCountRange.min;
				CountRangeMax = EggLayer.eggCountRange.max;
				FertilizationCountMax = EggLayer.eggFertilizationCountMax;
				ProgressUnfertilizedMax = EggLayer.eggProgressUnfertilizedMax;

				IsDefaultUnfertilizedDefNull = UnfertilizedDef == null;

				Hatcher = FertilizedDef?.comps?.Find((comp) => comp is CompProperties_Hatcher) as CompProperties_Hatcher;
				if (Hatcher != null) // should never be null
					DaysToHatch = Hatcher.hatcherDaystoHatch;
			}
		}
		public override void SetValue()
		{
			if (!IsEggLayer)
				return;

			EggLayer.eggLayFemaleOnly = FemaleOnly;
			EggLayer.eggLayIntervalDays = IntervalDays;
			EggLayer.eggCountRange.min = CountRangeMin;
			EggLayer.eggCountRange.max = CountRangeMax;
			EggLayer.eggFertilizationCountMax = FertilizationCountMax;
			EggLayer.eggProgressUnfertilizedMax = ProgressUnfertilizedMax;

			if (ProgressUnfertilizedMax >= 1f)
			{
				if (EggLayer.eggUnfertilizedDef == null) // unfertilized def must not be null if the animal can lay unfertilized eggs; just use chicken eggs as all eggs are equal anyway
					EggLayer.eggUnfertilizedDef = DefDatabase<ThingDef>.AllDefs.FirstOrDefault((def) => def.defName == "EggChickenUnfertilized"); 
			}
			else if (ProgressUnfertilizedMax < 1f)
			{
				if (IsDefaultUnfertilizedDefNull && EggLayer.eggUnfertilizedDef != null)
					EggLayer.eggUnfertilizedDef = null;
			}

			if (Hatcher != null)
				Hatcher.hatcherDaystoHatch = DaysToHatch;
		}

		public override void Reset()
		{
			FemaleOnly = DefaultFemaleOnly;
			IntervalDays = DefaultIntervalDays;
			CountRangeMin = DefaultCountRangeMin;
			CountRangeMax = DefaultCountRangeMax;
			FertilizationCountMax = DefaultFertilizationCountMax;
			ProgressUnfertilizedMax = DefaultProgressUnfertilizedMax;

			DaysToHatch = DefaultDaysToHatch;
		}
		public override bool IsModified()
		{
			if (FemaleOnly != DefaultFemaleOnly
				|| IntervalDays != DefaultIntervalDays
				|| CountRangeMin != DefaultCountRangeMin
				|| CountRangeMax != DefaultCountRangeMax
				|| FertilizationCountMax != DefaultFertilizationCountMax
				|| ProgressUnfertilizedMax != DefaultProgressUnfertilizedMax
				|| DaysToHatch != DefaultDaysToHatch)
				return true;
			return false;
		}

		public override void ExposeData()
		{
			bool boolValue = FemaleOnly;
			Scribe_Values.Look(ref boolValue, nameof(FemaleOnly), DefaultFemaleOnly);
			FemaleOnly = boolValue;

			float floatValue = IntervalDays;
			Scribe_Values.Look(ref floatValue, nameof(IntervalDays), DefaultIntervalDays);
			IntervalDays = floatValue;

			int intValue = CountRangeMin;
			Scribe_Values.Look(ref intValue, nameof(CountRangeMin), DefaultCountRangeMin);
			CountRangeMin = intValue;

			intValue = CountRangeMax;
			Scribe_Values.Look(ref intValue, nameof(CountRangeMax), DefaultCountRangeMax);
			CountRangeMax = intValue;

			intValue = FertilizationCountMax;
			Scribe_Values.Look(ref intValue, nameof(FertilizationCountMax), DefaultFertilizationCountMax);
			FertilizationCountMax = intValue;

			floatValue = ProgressUnfertilizedMax;
			Scribe_Values.Look(ref floatValue, nameof(ProgressUnfertilizedMax), DefaultProgressUnfertilizedMax);
			ProgressUnfertilizedMax = floatValue;

			floatValue = DaysToHatch;
			Scribe_Values.Look(ref floatValue, nameof(DaysToHatch), DefaultDaysToHatch);
			DaysToHatch = floatValue;
		}
		#endregion
	}
}
