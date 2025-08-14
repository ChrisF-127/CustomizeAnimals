using CustomizeAnimals.Settings;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CustomizeAnimals.Controls
{
	internal class ControlToxicResistance : BaseSettingControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (NullableFloatSetting)animalSettings.GeneralSettings["ToxicResistance"];
			var value = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.ToxicResistance".Translate(),
				"SY_CA.TooltipToxicResistance".Translate(),
				setting.IsModified(),
				setting.Value ?? StatDefOf.ToxicResistance.defaultBaseValue,
				setting.DefaultValue ?? StatDefOf.ToxicResistance.defaultBaseValue, 
				StatDefOf.ToxicResistance.minValue,
				StatDefOf.ToxicResistance.maxValue,
				convert: ConvertToPercent,
				unit: "%");

			setting.Value = value;

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth)
		{
			(var use, var min, var max) = CreateNumericGlobalMinMax(
				offsetY,
				viewWidth,
				"SY_CA.ToxicResistanceRange".Translate(),
				"SY_CA.TooltipMinToxicResistance".Translate(),
				"SY_CA.TooltipMaxToxicResistance".Translate(),
				SettingToxicResistance.UseLimits,
				SettingToxicResistance.Minimum,
				SettingToxicResistance.Maximum,
				StatDefOf.ToxicResistance.minValue,
				StatDefOf.ToxicResistance.maxValue,
				convert: ConvertToPercent,
				unit: "%");

			SettingToxicResistance.UseLimits = use;
			SettingToxicResistance.Minimum = min;
			SettingToxicResistance.Maximum = max;

			return SettingsDoubleRowHeight;
		}
	}
}
