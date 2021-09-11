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
	internal class ControlCarryingCapacity : BaseSettingControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (NullableFloatSetting)animalSettings.Settings["CarryingCapacity"];

			var bodySize = animalSettings?.Animal?.race?.baseBodySize ?? 1f;
			float convert(float val) => Mathf.Round(val * bodySize);

			var value = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.CarryingCapacity".Translate(),
				"SY_CA.TooltipCarryingCapacity".Translate(),
				setting.IsModified(),
				setting.Value ?? StatDefOf.CarryingCapacity.defaultBaseValue,
				setting.DefaultValue ?? StatDefOf.CarryingCapacity.defaultBaseValue, 
				StatDefOf.CarryingCapacity.minValue,
				StatDefOf.CarryingCapacity.maxValue,
				convert: convert);

			setting.Value = value;

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth)
		{
			(var use, var min, var max) = CreateNumericGlobalMinMax(
				offsetY,
				viewWidth,
				"SY_CA.CarryingCapacityRange".Translate(),
				"SY_CA.TooltipMinCarryingCapacity".Translate(),
				"SY_CA.TooltipMaxCarryingCapacity".Translate(),
				SettingCarryingCapacity.UseLimits,
				SettingCarryingCapacity.Minimum,
				SettingCarryingCapacity.Maximum,
				StatDefOf.CarryingCapacity.minValue,
				StatDefOf.CarryingCapacity.maxValue);

			SettingCarryingCapacity.UseLimits = use;
			SettingCarryingCapacity.Minimum = min;
			SettingCarryingCapacity.Maximum = max;

			return SettingsDoubleRowHeight;
		}
	}
}
