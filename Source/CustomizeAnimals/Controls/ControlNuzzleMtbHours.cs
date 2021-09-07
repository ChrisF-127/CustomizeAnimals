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
	internal class ControlNuzzleMtbHours : BaseSettingControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (BaseSetting<float>)animalSettings.Settings["NuzzleMtbHours"];
			var value = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.NuzzleMtbHours".Translate(),
				"SY_CA.TooltipNuzzleMtbHours".Translate(),
				setting.IsModified(),
				setting.Value,
				setting.DefaultValue,
				SettingNuzzleMtbHours.DefaultMinimum,
				SettingNuzzleMtbHours.DefaultMaximum,
				unit: "h");

			setting.Value = value;

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth)
		{
			(var use, var min, var max) = CreateNumericGlobalMinMax(
				offsetY,
				viewWidth,
				"SY_CA.NuzzleMtbHoursRange".Translate(),
				"SY_CA.TooltipMinNuzzleMtbHours".Translate(),
				"SY_CA.TooltipMaxNuzzleMtbHours".Translate(),
				SettingNuzzleMtbHours.UseNuzzleMtbHoursLimits,
				SettingNuzzleMtbHours.MinimumNuzzleMtbHours,
				SettingNuzzleMtbHours.MaximumNuzzleMtbHours,
				SettingNuzzleMtbHours.DefaultMinimum,
				SettingNuzzleMtbHours.DefaultMaximum,
				unit: "h");

			SettingNuzzleMtbHours.UseNuzzleMtbHoursLimits = use;
			SettingNuzzleMtbHours.MinimumNuzzleMtbHours = min;
			SettingNuzzleMtbHours.MaximumNuzzleMtbHours = max;

			return SettingsDoubleRowHeight;
		}
	}
}
