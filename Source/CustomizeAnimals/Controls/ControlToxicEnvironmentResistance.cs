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
	internal class ControlToxicEnvironmentResistance : BaseSettingControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (NullableFloatSetting)animalSettings.GeneralSettings["ToxicEnvironmentResistance"];
			var value = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.ToxicEnvironmentResistance".Translate(),
				"SY_CA.TooltipToxicEnvironmentResistance".Translate(),
				setting.IsModified(),
				setting.Value ?? StatDefOf.ToxicEnvironmentResistance.defaultBaseValue,
				setting.DefaultValue ?? StatDefOf.ToxicEnvironmentResistance.defaultBaseValue, 
				StatDefOf.ToxicEnvironmentResistance.minValue,
				StatDefOf.ToxicEnvironmentResistance.maxValue,
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
				"SY_CA.ToxicEnvironmentResistanceRange".Translate(),
				"SY_CA.TooltipMinToxicEnvironmentResistance".Translate(),
				"SY_CA.TooltipMaxToxicEnvironmentResistance".Translate(),
				SettingToxicEnvironmentResistance.UseLimits,
				SettingToxicEnvironmentResistance.Minimum,
				SettingToxicEnvironmentResistance.Maximum,
				StatDefOf.ToxicEnvironmentResistance.minValue,
				StatDefOf.ToxicEnvironmentResistance.maxValue,
				convert: ConvertToPercent,
				unit: "%");

			SettingToxicEnvironmentResistance.UseLimits = use;
			SettingToxicEnvironmentResistance.Minimum = min;
			SettingToxicEnvironmentResistance.Maximum = max;

			return SettingsDoubleRowHeight;
		}
	}
}
