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
	internal class ControlWildness : BaseControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (NullableFloatSetting)animalSettings.Settings["Wildness"];
			var value = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.Wildness".Translate(),
				"SY_CA.TooltipWildness".Translate(),
				setting.IsModified(),
				setting.Value ?? 1f,
				setting.DefaultValue ?? 1f,
				max: 10f,
				to: ToPercent,
				back: FromPercent,
				unit: "%");

			setting.Value = value;

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth)
		{
#if TRUE
			return 0;
#else
			(var use, var min, var max) = CreateNumericGlobalMinMax(
				offsetY,
				viewWidth,
				"SY_CA.WildnessRange".Translate(),
				"SY_CA.TooltipMinWildness".Translate(),
				"SY_CA.TooltipMaxWildness".Translate(),
				SettingWildness.UseWildnessLimits,
				SettingWildness.MinimumWildness,
				SettingWildness.MaximumWildness,
				max: 10f,
				to: ToPercent,
				back: FromPercent,
				unit: "%");

			SettingWildness.UseWildnessLimits = use;
			SettingWildness.MinimumWildness = min;
			SettingWildness.MaximumWildness = max;

			return SettingsRowHeight;
#endif
		}
	}
}
