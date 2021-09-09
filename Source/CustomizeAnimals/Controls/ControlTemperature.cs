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
	internal class ControlMaxTemperature : TemperatureBaseControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (NullableFloatSetting)animalSettings.Settings["MaxTemperature"];
			var temp = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.MaxTemperature".Translate(),
				"SY_CA.TooltipMaxTemperature".Translate(),
				setting.IsModified(),
				setting.Value ?? StatDefOf.ComfyTemperatureMax.defaultBaseValue, // Value should never be null at this point
				setting.DefaultValue ?? StatDefOf.ComfyTemperatureMax.defaultBaseValue, // DefaultValue should never be null at this point
				TemperatureTuning.MinimumTemperature,
				TemperatureTuning.MaximumTemperature,
				convert: FromCelsius,
				unit: GetTemperatureUnit());

			setting.Value = temp;

			return SettingsRowHeight;
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
				TemperatureTuning.MaximumTemperature,
				convert: FromCelsius,
				unit: GetTemperatureUnit());

			SettingMaxTemperature.UseMaxTempLimits = use;
			SettingMaxTemperature.MinimumMaxTemp = min;
			SettingMaxTemperature.MaximumMaxTemp = max;

			return SettingsDoubleRowHeight;
		}
	}


	internal class ControlMinTemperature : TemperatureBaseControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (NullableFloatSetting)animalSettings.Settings["MinTemperature"];
			var temp = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.MinTemperature".Translate(),
				"SY_CA.TooltipMinTemperature".Translate(),
				setting.IsModified(),
				setting.Value ?? StatDefOf.ComfyTemperatureMin.defaultBaseValue, // Value should never be null at this point
				setting.DefaultValue ?? StatDefOf.ComfyTemperatureMin.defaultBaseValue, // DefaultValue should never be null at this point
				TemperatureTuning.MinimumTemperature,
				TemperatureTuning.MaximumTemperature,
				convert: FromCelsius,
				unit: GetTemperatureUnit());

			setting.Value = temp;

			return SettingsRowHeight;
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
				TemperatureTuning.MaximumTemperature,
				convert: FromCelsius,
				unit: GetTemperatureUnit());

			SettingMinTemperature.UseMinTempLimits = use;
			SettingMinTemperature.MinimumMinTemp = min;
			SettingMinTemperature.MaximumMinTemp = max;

			return SettingsDoubleRowHeight;
		}
	}


	internal abstract class TemperatureBaseControl : BaseSettingControl
	{
		public static float FromCelsius(float temp)
		{
			double output;
			switch (Prefs.TemperatureMode)
			{
				case TemperatureDisplayMode.Celsius:
					output = temp;
					break;
				case TemperatureDisplayMode.Fahrenheit:
					output = temp * 1.8 + 32.0;
					break;
				case TemperatureDisplayMode.Kelvin:
					output = temp + 273.15;
					break;
				default:
					throw new InvalidOperationException();
			};
			return (float)Math.Round(output, 2);
		}
		public static float ToCelsius(float temp)
		{
			double output;
			switch (Prefs.TemperatureMode)
			{
				case TemperatureDisplayMode.Celsius:
					output = temp;
					break;
				case TemperatureDisplayMode.Fahrenheit:
					output = (temp - 32.0) / 1.8;
					break;
				case TemperatureDisplayMode.Kelvin:
					output = temp - 273.15;
					break;
				default:
					throw new InvalidOperationException();
			};
			return (float)Math.Round(output, 2);
		}
		public static string GetTemperatureUnit()
		{
			switch (Prefs.TemperatureMode)
			{
				case TemperatureDisplayMode.Celsius:
					return "°C";
				case TemperatureDisplayMode.Fahrenheit:
					return "°F";
				case TemperatureDisplayMode.Kelvin:
					return "K";
			}
			return "";
		}
	}
}
