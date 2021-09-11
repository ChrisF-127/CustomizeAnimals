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
	internal class ControlCaravanRidingSpeed : BaseSettingControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (NullableFloatSetting)animalSettings.Settings["CaravanRidingSpeed"];
			var value = CreateNullableNumeric(
				offsetY,
				viewWidth,
				"SY_CA.CaravanRidingSpeed".Translate(),
				$"({"SY_CA.CaravanRidingSpeedDisabled".Translate()})",
				"SY_CA.TooltipCaravanRidingSpeed".Translate(),
				"SY_CA.TooltipCaravanRidingSpeedChk".Translate(),
				setting.IsModified(),
				setting.Value,
				setting.DefaultValue, 
				StatDefOf.CaravanRidingSpeedFactor.minValue,
				StatDefOf.CaravanRidingSpeedFactor.maxValue,
				convert: ConvertToPercent,
				unit: "%");

			setting.Value = value;

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth) => 0f;
	}
}
