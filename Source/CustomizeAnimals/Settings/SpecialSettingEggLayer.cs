using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CustomizeAnimals.Settings
{
	internal class SpecialSettingEggLayer : BaseSpecialSetting
	{
		#region PROPERTIES
		private GeneralSettings GeneralSettings => GlobalSettings.GlobalGeneralSettings;

		public CompProperties_EggLayer EggLayer { get; private set; }
		public bool IsEggLayer => EggLayer != null;

		public ThingDef FertilizedDef => EggLayer?.eggFertilizedDef;
		public float DefaultFertilizedEggMass { get; private set; }
		public float DefaultFertilizedEggNutrition { get; private set; }
		public ThingDef UnfertilizedDef => EggLayer?.eggUnfertilizedDef;
		public float DefaultUnfertilizedEggMass { get; private set; }
		public float DefaultUnfertilizedEggNutrition { get; private set; }

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
			if (!IsEggLayer)
				return;
			
			FemaleOnly = EggLayer.eggLayFemaleOnly;
			IntervalDays = EggLayer.eggLayIntervalDays;
			CountRangeMin = EggLayer.eggCountRange.min;
			CountRangeMax = EggLayer.eggCountRange.max;
			FertilizationCountMax = EggLayer.eggFertilizationCountMax;
			ProgressUnfertilizedMax = EggLayer.eggProgressUnfertilizedMax;

			Hatcher = FertilizedDef?.comps?.Find((comp) => comp is CompProperties_Hatcher) as CompProperties_Hatcher;
			if (Hatcher != null) 
				DaysToHatch = Hatcher.hatcherDaystoHatch;

			DefaultFertilizedEggMass = FertilizedDef?.GetStatValueAbstract(StatDefOf.Mass) ?? 0f;
			DefaultFertilizedEggNutrition = FertilizedDef?.GetStatValueAbstract(StatDefOf.Nutrition) ?? 0f;
			DefaultUnfertilizedEggMass = UnfertilizedDef?.GetStatValueAbstract(StatDefOf.Mass) ?? 0f;
			DefaultUnfertilizedEggNutrition = UnfertilizedDef?.GetStatValueAbstract(StatDefOf.Nutrition) ?? 0f;
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

			if (Hatcher != null) 
				Hatcher.hatcherDaystoHatch = DaysToHatch;


			var bodySize = Animal?.race?.baseBodySize ?? 1f;
			// Egg Mass
			if (GeneralSettings.EggMassDependOnBodySize)
			{
				var mass = bodySize * GeneralSettings.EggMassFactor;
				FertilizedDef?.SetStatBaseValue(StatDefOf.Mass, mass);
				UnfertilizedDef?.SetStatBaseValue(StatDefOf.Mass, mass);
			}
			else
			{
				FertilizedDef?.SetStatBaseValue(StatDefOf.Mass, DefaultFertilizedEggMass);
				UnfertilizedDef?.SetStatBaseValue(StatDefOf.Mass, DefaultUnfertilizedEggMass);
			}
			// Egg Nutrition
			if (GeneralSettings.EggNutritionDependOnBodySize)
			{
				var nutrition = bodySize * GeneralSettings.EggNutritionFactor;
				FertilizedDef?.SetStatBaseValue(StatDefOf.Nutrition, nutrition);
				UnfertilizedDef?.SetStatBaseValue(StatDefOf.Nutrition, nutrition);
			}
			else
			{
				FertilizedDef?.SetStatBaseValue(StatDefOf.Nutrition, DefaultFertilizedEggNutrition);
				UnfertilizedDef?.SetStatBaseValue(StatDefOf.Nutrition, DefaultUnfertilizedEggNutrition);
			}
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
			Scribe_Values.Look(ref boolValue, "FemaleOnly", DefaultFemaleOnly);
			FemaleOnly = boolValue;

			float floatValue = IntervalDays;
			Scribe_Values.Look(ref floatValue, "IntervalDays", DefaultIntervalDays);
			IntervalDays = floatValue;

			int intValue = CountRangeMin;
			Scribe_Values.Look(ref intValue, "CountRangeMin", DefaultCountRangeMin);
			CountRangeMin = intValue;

			intValue = CountRangeMax;
			Scribe_Values.Look(ref intValue, "CountRangeMax", DefaultCountRangeMax);
			CountRangeMax = intValue;

			intValue = FertilizationCountMax;
			Scribe_Values.Look(ref intValue, "FertilizationCountMax", DefaultFertilizationCountMax);
			FertilizationCountMax = intValue;

			floatValue = ProgressUnfertilizedMax;
			Scribe_Values.Look(ref floatValue, "ProgressUnfertilizedMax", DefaultProgressUnfertilizedMax);
			ProgressUnfertilizedMax = floatValue;

			floatValue = DaysToHatch;
			Scribe_Values.Look(ref floatValue, "DaysToHatch", DefaultDaysToHatch);
			DaysToHatch = floatValue;
		}
		#endregion
	}
}
