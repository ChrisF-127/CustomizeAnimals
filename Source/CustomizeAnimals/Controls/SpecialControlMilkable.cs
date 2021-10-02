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
	internal class SpecialControlMilkable : BaseSpecialSettingControl
	{
		#region PROPERTIES
		#endregion

		#region FIELDS
		public string IntervalDaysBuffer;
		public string AmountBuffer;
		#endregion

		#region METHODS
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			if (animalSettings.IsHuman)
				return 0f;

			var setting = (SpecialSettingMilkable)animalSettings.SpecialSettings["Milkable"];

			var totalHeight = offsetY;

			// Is Milkable
			setting.IsMilkable = CreateCheckbox(
				totalHeight,
				viewWidth,
				"SY_CA.MilkableIsMilkable".Translate(),
				"SY_CA.TooltipMilkableIsMilkable".Translate(),
				setting.IsMilkable,
				setting.DefaultIsMilkable,
				"SY_CA.MilkableIsMilkableText".Translate());
			totalHeight += SettingsRowHeight;

			if (!setting.IsMilkable)
				return totalHeight - offsetY;

#warning TODO selectable milk def
			// Milk Def
			CreateText(
				totalHeight,
				viewWidth,
				"SY_CA.MilkableDef".Translate(),
				setting?.MilkDef?.label?.CapitalizeFirst() ?? "SY_CA.MilkableNoDef".Translate());
			totalHeight += SettingsRowHeight;

			// Interval Days
			setting.IntervalDays = CreateNumeric(
				totalHeight,
				viewWidth,
				"SY_CA.MilkableIntervalDays".Translate(),
				"SY_CA.TooltipMilkableIntervalDays".Translate(),
				setting.IntervalDays != setting.DefaultIntervalDays,
				setting.IntervalDays,
				setting.DefaultIntervalDays,
				ref IntervalDaysBuffer,
				unit: "d");
			totalHeight += SettingsRowHeight;

			// Amount
			setting.Amount = CreateNumeric(
				totalHeight,
				viewWidth,
				"SY_CA.MilkableAmount".Translate(),
				"SY_CA.TooltipMilkableAmount".Translate(),
				setting.Amount != setting.DefaultAmount,
				setting.Amount,
				setting.DefaultAmount,
				ref AmountBuffer);
			totalHeight += SettingsRowHeight;
			
			// Female Only
			setting.FemaleOnly = CreateCheckbox(
				totalHeight,
				viewWidth,
				"SY_CA.MilkableFemaleOnly".Translate(),
				"SY_CA.TooltipMilkableFemaleOnly".Translate(),
				setting.FemaleOnly,
				setting.DefaultFemaleOnly,
				"SY_CA.MilkableFemaleOnlyText".Translate());
			totalHeight += SettingsRowHeight;

			return totalHeight - offsetY;
		}

		public override void Reset()
		{
			IntervalDaysBuffer = null;
			AmountBuffer = null;
		}
		#endregion
	}
}
