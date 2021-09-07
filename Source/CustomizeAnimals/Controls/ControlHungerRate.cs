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
	internal class ControlHungerRate : BaseSettingControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (NullableFloatSetting)animalSettings.Settings["HungerRate"];
			var value = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.HungerRate".Translate(),
				"SY_CA.TooltipHungerRate".Translate(),
				setting.IsModified(),
				setting.Value ?? 1f,
				setting.DefaultValue ?? 1f,
				convert: ToHungerRate);

			setting.Value = value;

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth)
		{
			(var use, var min, var max) = CreateNumericGlobalMinMax(
				offsetY,
				viewWidth,
				"SY_CA.HungerRateRange".Translate(),
				"SY_CA.TooltipMinHungerRate".Translate(),
				"SY_CA.TooltipMaxHungerRate".Translate(),
				SettingHungerRate.UseHungerRateLimits,
				SettingHungerRate.MinimumHungerRate,
				SettingHungerRate.MaximumHungerRate,
				convert: ToHungerRate);

			SettingHungerRate.UseHungerRateLimits = use;
			SettingHungerRate.MinimumHungerRate = min;
			SettingHungerRate.MaximumHungerRate = max;

			return SettingsDoubleRowHeight;
		}

		private static float ToHungerRate(float value) =>
			(float)Math.Round(value * 1.6f, 3);
	}
}
