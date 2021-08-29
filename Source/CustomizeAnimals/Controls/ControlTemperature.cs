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
	internal class ControlMaxTemperature : BaseControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (BaseSetting<float?>)animalSettings.Settings["MaxTemperature"];
			var tempMode = Prefs.TemperatureMode;
			var temp = CreateNumeric(
				offsetY,
				viewWidth,
				animalSettings,
				"SY_CA.MaxTemperature".Translate(),
				"SY_CA.TooltipMaxTemperature".Translate(),
				setting.IsModified(),
				CelsiusTo(setting.Value ?? StatDefOf.ComfyTemperatureMax.defaultBaseValue, tempMode), // Value should never be null at this point
				CelsiusTo(setting.DefaultValue ?? StatDefOf.ComfyTemperatureMax.defaultBaseValue, tempMode), // DefaultValue should never be null at this point
				CelsiusTo(TemperatureTuning.MinimumTemperature, tempMode),
				CelsiusTo(TemperatureTuning.MaximumTemperature, tempMode));

			setting.Value = ToCelsius(temp, tempMode);

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth)
		{
			var tempMode = Prefs.TemperatureMode;
			(var use, var min, var max) = CreateNumericGlobalMinMax(
				offsetY,
				viewWidth,
				"SY_CA.MaxTemperatureRange".Translate(),
				"SY_CA.TooltipMinMaxTemperature".Translate(),
				"SY_CA.TooltipMaxMaxTemperature".Translate(),
				SettingMaxTemperature.UseMaxTempLimits,
				CelsiusTo(SettingMaxTemperature.MinimumMaxTemp, tempMode),
				CelsiusTo(SettingMaxTemperature.MaximumMaxTemp, tempMode),
				CelsiusTo(TemperatureTuning.MinimumTemperature, tempMode),
				CelsiusTo(TemperatureTuning.MaximumTemperature, tempMode));

			SettingMaxTemperature.UseMaxTempLimits = use;
			SettingMaxTemperature.MinimumMaxTemp = ToCelsius(min, tempMode);
			SettingMaxTemperature.MaximumMaxTemp = ToCelsius(max, tempMode);

			return SettingsRowHeight;
		}
	}


	internal class ControlMinTemperature : BaseControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (BaseSetting<float?>)animalSettings.Settings["MinTemperature"];
			var tempMode = Prefs.TemperatureMode;
			var temp = CreateNumeric(
				offsetY,
				viewWidth,
				animalSettings,
				"SY_CA.MinTemperature".Translate(),
				"SY_CA.TooltipMinTemperature".Translate(),
				setting.IsModified(),
				CelsiusTo(setting.Value ?? StatDefOf.ComfyTemperatureMin.defaultBaseValue, tempMode), // Value should never be null at this point
				CelsiusTo(setting.DefaultValue ?? StatDefOf.ComfyTemperatureMin.defaultBaseValue, tempMode), // DefaultValue should never be null at this point
				CelsiusTo(TemperatureTuning.MinimumTemperature, tempMode),
				CelsiusTo(TemperatureTuning.MaximumTemperature, tempMode));

			setting.Value = ToCelsius(temp, tempMode);

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth)
		{
			var tempMode = Prefs.TemperatureMode;
			(var use, var min, var max) = CreateNumericGlobalMinMax(
				offsetY,
				viewWidth,
				"SY_CA.MinTemperatureRange".Translate(),
				"SY_CA.TooltipMinMinTemperature".Translate(),
				"SY_CA.TooltipMaxMinTemperature".Translate(),
				SettingMinTemperature.UseMinTempLimits,
				CelsiusTo(SettingMinTemperature.MinimumMinTemp, tempMode),
				CelsiusTo(SettingMinTemperature.MaximumMinTemp, tempMode),
				CelsiusTo(TemperatureTuning.MinimumTemperature, tempMode),
				CelsiusTo(TemperatureTuning.MaximumTemperature, tempMode));

			SettingMinTemperature.UseMinTempLimits = use;
			SettingMinTemperature.MinimumMinTemp = ToCelsius(min, tempMode);
			SettingMinTemperature.MaximumMinTemp = ToCelsius(max, tempMode);

			return SettingsRowHeight;
		}
	}
}
