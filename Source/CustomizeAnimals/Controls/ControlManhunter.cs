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
	internal class ControlManhunterOnTameFail : ControlManhunter
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (SettingManhunter)animalSettings.Settings["ManhunterOnTameFail"];
			var temp = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.ManhunterOnTameFail".Translate(),
				"SY_CA.TooltipManhunterOnTameFail".Translate(),
				setting.IsModified(),
				setting.Value,
				setting.DefaultValue,
				SettingManhunter.DefaultMinimum,
				SettingManhunter.DefaultMaximum,
				convert: ToPercent,
				unit: "%");

			setting.Value = temp;

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth)
		{
			(var use, var min, var max) = CreateNumericGlobalMinMax(
				offsetY,
				viewWidth,
				"SY_CA.ManhunterOnTameFailRange".Translate(),
				"SY_CA.TooltipMinManhunterOnTameFail".Translate(),
				"SY_CA.TooltipMaxManhunterOnTameFail".Translate(),
				SettingManhunterOnTameFail.UseLimits,
				SettingManhunterOnTameFail.Minimum,
				SettingManhunterOnTameFail.Maximum,
				SettingManhunter.DefaultMinimum,
				SettingManhunter.DefaultMaximum,
				convert: ToPercent,
				unit: "%");

			SettingManhunterOnTameFail.UseLimits = use;
			SettingManhunterOnTameFail.Minimum = min;
			SettingManhunterOnTameFail.Maximum = max;

			return SettingsDoubleRowHeight;
		}
	}
	internal class ControlManhunterOnDamage : ControlManhunter
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (SettingManhunter)animalSettings.Settings["ManhunterOnDamage"];
			var temp = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.ManhunterOnDamage".Translate(),
				"SY_CA.TooltipManhunterOnDamage".Translate(),
				setting.IsModified(),
				setting.Value,
				setting.DefaultValue,
				SettingManhunter.DefaultMinimum,
				SettingManhunter.DefaultMaximum,
				convert: ToPercent,
				unit: "%");

			setting.Value = temp;

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth)
		{
			(var use, var min, var max) = CreateNumericGlobalMinMax(
				offsetY,
				viewWidth,
				"SY_CA.ManhunterOnDamageRange".Translate(),
				"SY_CA.TooltipMinManhunterOnDamage".Translate(),
				"SY_CA.TooltipMaxManhunterOnDamage".Translate(),
				SettingManhunterOnDamage.UseLimits,
				SettingManhunterOnDamage.Minimum,
				SettingManhunterOnDamage.Maximum,
				SettingManhunter.DefaultMinimum,
				SettingManhunter.DefaultMaximum,
				convert: ToPercent,
				unit: "%");

			SettingManhunterOnDamage.UseLimits = use;
			SettingManhunterOnDamage.Minimum = min;
			SettingManhunterOnDamage.Maximum = max;

			return SettingsDoubleRowHeight;
		}
	}



	internal abstract class ControlManhunter : BaseSettingControl
	{ }
}
