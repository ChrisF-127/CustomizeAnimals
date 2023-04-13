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
			if (animalSettings.IsHumanLike)
				return 0f;

			var setting = (NullableFloatSetting)animalSettings.GeneralSettings["CaravanRidingSpeed"];
			var value = CreateNullableNumeric(
				offsetY,
				viewWidth,
				"SY_CA.CaravanRidingSpeed".Translate(),
				"SY_CA.CaravanRidingSpeedDisabled".Translate(),
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

		private string GlobalModifierBuffer;
		public override float CreateSettingGlobal(float offsetY, float viewWidth)
		{
			(var use, var value) = CreateNumericGlobal(
				offsetY,
				viewWidth,
				"SY_CA.CaravanRidingSpeedGlobal".Translate(),
				"SY_CA.TooltipCaravanRidingSpeedGlobal".Translate(),
				SettingCaravanRidingSpeed.UseGlobalModifier,
				SettingCaravanRidingSpeed.GlobalModifier,
				SettingCaravanRidingSpeed.GlobalModifierDefault,
				ref GlobalModifierBuffer,
				min: SettingCaravanRidingSpeed.MinimumModifier,
				max: SettingCaravanRidingSpeed.MaximumModifier);
			SettingCaravanRidingSpeed.UseGlobalModifier = use;
			SettingCaravanRidingSpeed.GlobalModifier = value;

			return SettingsThinRowHeight;
		}
	}
}
