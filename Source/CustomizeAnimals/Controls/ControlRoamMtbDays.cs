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
	internal class ControlRoamMtbDays : BaseControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (NullableFloatSetting)animalSettings.Settings["RoamMtbDays"];
			var value = CreateNullableNumeric(
				offsetY,
				viewWidth,
				"SY_CA.RoamMtbDays".Translate(),
				"SY_CA.RoamMtbDaysDisabled".Translate(),
				"SY_CA.TooltipRoamMtbDays".Translate(),
				"SY_CA.TooltipRoamMtbDaysChk".Translate(),
				setting.IsModified(),
				setting.Value,
				setting.DefaultValue,
				min: 1f,
				unit: "d");

			setting.Value = value;

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth)
		{
			(var use, var value) = CreateNullableNumericGlobal(
				offsetY,
				viewWidth,
				"SY_CA.MinimumRoamMtbDays".Translate(),
				"SY_CA.RoamMtbDaysDisabled".Translate(),
				"SY_CA.TooltipMinimumRoamMtbDays".Translate(),
				"SY_CA.TooltipMinimumRoamMtbDaysChk".Translate(),
				SettingRoamMtbDays.UseMinimumRoamMtbDays,
				SettingRoamMtbDays.MinimumRoamMtbDays,
				SettingRoamMtbDays.DefaultMinimum,
				min: 1f,
				unit: "d");

			SettingRoamMtbDays.UseMinimumRoamMtbDays = use;
			SettingRoamMtbDays.MinimumRoamMtbDays = value;

			return SettingsRowHeight;
		}
	}
}
