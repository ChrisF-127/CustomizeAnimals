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
	internal class ControlMaxTemperature : BaseControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			return CreateNumeric(
				offsetY,
				viewWidth,
				animalSettings,
				(BaseSetting<float?>)animalSettings.Settings["MaxTemperature"],
				"SY_CA.MaxTemperature".Translate(),
				"SY_CA.TooltipMaxTemperature".Translate(),
				TemperatureTuning.MinimumTemperature,
				TemperatureTuning.MaximumTemperature);
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth)
		{
			(var use, var min, var max) = CreateNumericGlobalMinMax(
				offsetY,
				viewWidth,
				"SY_CA.MaxTemperatureRange".Translate(),
				"SY_CA.TooltipMinMaxTemperature".Translate(),
				"SY_CA.TooltipMaxMaxTemperature".Translate(),
				SettingMaxTemperature.UseMaxTempLimits,
				SettingMaxTemperature.MinimumMaxTemp,
				SettingMaxTemperature.MaximumMaxTemp,
				TemperatureTuning.MinimumTemperature,
				TemperatureTuning.MaximumTemperature);

			SettingMaxTemperature.UseMaxTempLimits = use;
			SettingMaxTemperature.MinimumMaxTemp = min;
			SettingMaxTemperature.MaximumMaxTemp = max;

			return SettingsRowHeight;
		}
	}


	internal class ControlMinTemperature : BaseControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			return CreateNumeric(
				offsetY,
				viewWidth,
				animalSettings,
				(BaseSetting<float?>)animalSettings.Settings["MinTemperature"],
				"SY_CA.MinTemperature".Translate(),
				"SY_CA.TooltipMinTemperature".Translate(),
				TemperatureTuning.MinimumTemperature,
				TemperatureTuning.MaximumTemperature);
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth)
		{
			(var use, var min, var max) = CreateNumericGlobalMinMax(
				offsetY,
				viewWidth,
				"SY_CA.MinTemperatureRange".Translate(),
				"SY_CA.TooltipMinMinTemperature".Translate(),
				"SY_CA.TooltipMaxMinTemperature".Translate(),
				SettingMinTemperature.UseMinTempLimits,
				SettingMinTemperature.MinimumMinTemp,
				SettingMinTemperature.MaximumMinTemp,
				TemperatureTuning.MinimumTemperature,
				TemperatureTuning.MaximumTemperature);

			SettingMinTemperature.UseMinTempLimits = use;
			SettingMinTemperature.MinimumMinTemp = min;
			SettingMinTemperature.MaximumMinTemp = max;

			return SettingsRowHeight;
		}
	}
}
