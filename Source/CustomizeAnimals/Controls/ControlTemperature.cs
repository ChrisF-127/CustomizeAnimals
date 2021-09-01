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
			var setting = (BaseSetting<float?>)animalSettings.Settings["MaxTemperature"];
			var tempMode = Prefs.TemperatureMode;
			var temp = CreateNumeric(
				offsetY,
				viewWidth,
				animalSettings,
				"SY_CA.MaxTemperature".Translate(),
				"SY_CA.TooltipMaxTemperature".Translate(),
				setting.IsModified(),
				setting.Value ?? StatDefOf.ComfyTemperatureMax.defaultBaseValue, // Value should never be null at this point
				setting.DefaultValue ?? StatDefOf.ComfyTemperatureMax.defaultBaseValue, // DefaultValue should never be null at this point
				TemperatureTuning.MinimumTemperature,
				TemperatureTuning.MaximumTemperature,
				to: FromCelsius,
				back: ToCelsius,
				unit: GetTemperatureUnit());

			setting.Value = temp;

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
				SettingMaxTemperature.MinimumMaxTemp,
				SettingMaxTemperature.MaximumMaxTemp,
				TemperatureTuning.MinimumTemperature,
				TemperatureTuning.MaximumTemperature,
				to: FromCelsius,
				back: ToCelsius,
				unit: GetTemperatureUnit());

			SettingMaxTemperature.UseMaxTempLimits = use;
			SettingMaxTemperature.MinimumMaxTemp = min;
			SettingMaxTemperature.MaximumMaxTemp = max;

			return SettingsRowHeight;
		}
	}


	internal class ControlMinTemperature : TemperatureBaseControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (BaseSetting<float?>)animalSettings.Settings["MinTemperature"];
			var temp = CreateNumeric(
				offsetY,
				viewWidth,
				animalSettings,
				"SY_CA.MinTemperature".Translate(),
				"SY_CA.TooltipMinTemperature".Translate(),
				setting.IsModified(),
				setting.Value ?? StatDefOf.ComfyTemperatureMin.defaultBaseValue, // Value should never be null at this point
				setting.DefaultValue ?? StatDefOf.ComfyTemperatureMin.defaultBaseValue, // DefaultValue should never be null at this point
				TemperatureTuning.MinimumTemperature,
				TemperatureTuning.MaximumTemperature,
				to: FromCelsius,
				back: ToCelsius,
				unit: GetTemperatureUnit());

			setting.Value = temp;

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
				SettingMinTemperature.MinimumMinTemp,
				SettingMinTemperature.MaximumMinTemp,
				TemperatureTuning.MinimumTemperature,
				TemperatureTuning.MaximumTemperature,
				to: FromCelsius,
				back: ToCelsius,
				unit: GetTemperatureUnit());

			SettingMinTemperature.UseMinTempLimits = use;
			SettingMinTemperature.MinimumMinTemp = min;
			SettingMinTemperature.MaximumMinTemp = max;

			return SettingsRowHeight;
		}
	}


	internal abstract class TemperatureBaseControl : BaseControl
	{
		public static float ToCelsius(float temp)
		{
			switch (Prefs.TemperatureMode)
			{
				case TemperatureDisplayMode.Celsius:
					return temp;
				case TemperatureDisplayMode.Fahrenheit:
					return (temp - 32f) / 1.8f;
				case TemperatureDisplayMode.Kelvin:
					return temp - 273.15f;
				default:
					throw new InvalidOperationException();
			};
		}
		public static float FromCelsius(float temp)
		{
			switch (Prefs.TemperatureMode)
			{
				case TemperatureDisplayMode.Celsius:
					return temp;
				case TemperatureDisplayMode.Fahrenheit:
					return temp * 1.8f + 32f;
				case TemperatureDisplayMode.Kelvin:
					return temp + 273.15f;
				default:
					throw new InvalidOperationException();
			};
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
