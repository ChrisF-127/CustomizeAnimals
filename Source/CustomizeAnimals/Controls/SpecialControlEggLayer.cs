using CustomizeAnimals.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CustomizeAnimals.Controls
{
	internal class SpecialControlEggLayer : BaseSpecialSettingControl
	{
		#region PROPERTIES
		#endregion

		#region FIELDS
		public string IntervalDaysBuffer;
		public string DaysToHatchBuffer;
		public string CountRangeMinBuffer;
		public string CountRangeMaxBuffer;
		public string FertilizationCountMaxBuffer;
		public string ProgressUnfertilizedMaxBuffer;
		#endregion

		#region METHODS
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			if (animalSettings.IsHumanLike)
				return 0f;

			var setting = (SpecialSettingEggLayer)animalSettings.ReproductionSettings["EggLayer"];
			if (!setting.IsEggLayer)
				return 0f;

			var totalHeight = offsetY;

			// Separator
			Widgets.ListSeparator(ref totalHeight, viewWidth - 16, "SY_CA.EggLayer".Translate());
			totalHeight += 2;
			Text.Anchor = TextAnchor.MiddleLeft;

			// Fertilized Def
			CreateText(
				totalHeight,
				viewWidth,
				"SY_CA.EggLayerFertilizedDef".Translate(),
				setting?.EggLayer?.eggFertilizedDef?.label?.CapitalizeFirst() ?? "SY_CA.EggLayerNoDef".Translate());
			totalHeight += SettingsRowHeight;

			// Unfertilized Def
			CreateText(
				totalHeight,
				viewWidth,
				"SY_CA.EggLayerUnfertilizedDef".Translate(),
				setting?.EggLayer?.eggUnfertilizedDef?.label?.CapitalizeFirst() ?? "SY_CA.EggLayerNoDef".Translate());
			totalHeight += SettingsRowHeight;

			// Interval Days
			setting.IntervalDays = CreateNumeric(
				totalHeight,
				viewWidth,
				"SY_CA.EggLayerIntervalDays".Translate(),
				"SY_CA.TooltipEggLayerIntervalDays".Translate(),
				setting.IntervalDays != setting.DefaultIntervalDays,
				setting.IntervalDays,
				setting.DefaultIntervalDays,
				ref IntervalDaysBuffer,
				unit: "d");
			totalHeight += SettingsRowHeight;

			// Days To Hatch
			setting.DaysToHatch = CreateNumeric(
				totalHeight,
				viewWidth,
				"SY_CA.EggLayerDaysToHatch".Translate(),
				"SY_CA.TooltipEggLayerDaysToHatch".Translate(),
				setting.DaysToHatch != setting.DefaultDaysToHatch,
				setting.DaysToHatch,
				setting.DefaultDaysToHatch,
				ref DaysToHatchBuffer,
				unit: "d");
			totalHeight += SettingsRowHeight;

			// Count Range Min
			setting.CountRangeMin = CreateNumeric(
				totalHeight,
				viewWidth,
				"SY_CA.EggLayerCountRangeMin".Translate(),
				"SY_CA.TooltipEggLayerCountRangeMin".Translate(),
				setting.CountRangeMin != setting.DefaultCountRangeMin,
				setting.CountRangeMin,
				setting.DefaultCountRangeMin,
				ref CountRangeMinBuffer,
				max: 75);
			totalHeight += SettingsRowHeight;

			// Count Range Max
			setting.CountRangeMax = CreateNumeric(
				totalHeight,
				viewWidth,
				"SY_CA.EggLayerCountRangeMax".Translate(),
				"SY_CA.TooltipEggLayerCountRangeMax".Translate(),
				setting.CountRangeMax != setting.DefaultCountRangeMax,
				setting.CountRangeMax,
				setting.DefaultCountRangeMax,
				ref CountRangeMaxBuffer,
				min: setting.CountRangeMin,
				max: 75);
			totalHeight += SettingsRowHeight;

			// Fertilization Count Max
			setting.FertilizationCountMax = CreateNumeric(
				totalHeight,
				viewWidth,
				"SY_CA.EggLayerFertilizationCountMax".Translate(),
				"SY_CA.TooltipEggLayerFertilizationCountMax".Translate(),
				setting.FertilizationCountMax != setting.DefaultFertilizationCountMax,
				setting.FertilizationCountMax,
				setting.DefaultFertilizationCountMax,
				ref FertilizationCountMaxBuffer,
				min: 0f);
			totalHeight += SettingsRowHeight;

			// Progress Unfertilized Max
			//setting.ProgressUnfertilizedMax = CreateNumeric(
			//	totalHeight,
			//	viewWidth,
			//	"SY_CA.EggLayerProgressUnfertilizedMax".Translate(),
			//	"SY_CA.TooltipEggLayerProgressUnfertilizedMax".Translate(),
			//	setting.ProgressUnfertilizedMax != setting.DefaultProgressUnfertilizedMax,
			//	setting.ProgressUnfertilizedMax,
			//	setting.DefaultProgressUnfertilizedMax,
			//	ref ProgressUnfertilizedMaxBuffer,
			//	max: 1f,
			//	convert: ConvertToPercent,
			//	unit: "%");
			//totalHeight += SettingsRowHeight;

			if (setting.ProgressUnfertilizedMax == 1f && setting.UnfertilizedDef != null)
			{
				// Female Only
				setting.FemaleOnly = CreateCheckbox(
					totalHeight,
					viewWidth,
					"SY_CA.EggLayerFemaleOnly".Translate(),
					"SY_CA.TooltipEggLayerFemaleOnly".Translate(),
					setting.FemaleOnly,
					setting.DefaultFemaleOnly,
					"SY_CA.EggLayerFemaleOnlyText".Translate());
				totalHeight += SettingsRowHeight;
			}

			return totalHeight - offsetY;
		}

		public override void Reset()
		{
			IntervalDaysBuffer = null;
			DaysToHatchBuffer = null;
			CountRangeMinBuffer = null;
			CountRangeMaxBuffer = null;
			FertilizationCountMaxBuffer = null;
			ProgressUnfertilizedMaxBuffer = null;
		}
		#endregion
	}
}
