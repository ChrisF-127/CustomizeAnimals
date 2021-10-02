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
	internal class ControlToxicSensitivity : BaseSettingControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (NullableFloatSetting)animalSettings.GeneralSettings["ToxicSensitivity"];
			var value = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.ToxicSensitivity".Translate(),
				"SY_CA.TooltipToxicSensitivity".Translate(),
				setting.IsModified(),
				setting.Value ?? StatDefOf.ToxicSensitivity.defaultBaseValue,
				setting.DefaultValue ?? StatDefOf.ToxicSensitivity.defaultBaseValue, 
				StatDefOf.ToxicSensitivity.minValue,
				StatDefOf.ToxicSensitivity.maxValue,
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
				"SY_CA.ToxicSensitivityRange".Translate(),
				"SY_CA.TooltipMinToxicSensitivity".Translate(),
				"SY_CA.TooltipMaxToxicSensitivity".Translate(),
				SettingToxicSensitivity.UseLimits,
				SettingToxicSensitivity.Minimum,
				SettingToxicSensitivity.Maximum,
				StatDefOf.ToxicSensitivity.minValue,
				StatDefOf.ToxicSensitivity.maxValue,
				convert: ConvertToPercent,
				unit: "%");

			SettingToxicSensitivity.UseLimits = use;
			SettingToxicSensitivity.Minimum = min;
			SettingToxicSensitivity.Maximum = max;

			return SettingsDoubleRowHeight;
		}
	}
}
