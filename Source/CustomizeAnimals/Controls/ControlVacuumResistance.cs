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
	internal class ControlVacuumResistance : BaseSettingControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (NullableFloatSetting)animalSettings.GeneralSettings["VacuumResistance"];
			var value = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.VacuumResistance".Translate(),
				"SY_CA.TooltipVacuumResistance".Translate(),
				setting.IsModified(),
				setting.Value ?? StatDefOf.VacuumResistance.defaultBaseValue,
				setting.DefaultValue ?? StatDefOf.VacuumResistance.defaultBaseValue, 
				StatDefOf.VacuumResistance.minValue,
				StatDefOf.VacuumResistance.maxValue,
				convert: ConvertToPercent,
				unit: "%");

			setting.Value = value;

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth)
		{
			(var use, var min, var max) = CreateNumericGlobalMinMax(
				offsetY,
				viewWidth,
				"SY_CA.VacuumResistanceRange".Translate(),
				"SY_CA.TooltipMinVacuumResistance".Translate(),
				"SY_CA.TooltipMaxVacuumResistance".Translate(),
				SettingVacuumResistance.UseLimits,
				SettingVacuumResistance.Minimum,
				SettingVacuumResistance.Maximum,
				StatDefOf.VacuumResistance.minValue,
				StatDefOf.VacuumResistance.maxValue,
				convert: ConvertToPercent,
				unit: "%");

			SettingVacuumResistance.UseLimits = use;
			SettingVacuumResistance.Minimum = min;
			SettingVacuumResistance.Maximum = max;

			return SettingsDoubleRowHeight;
		}
	}
}
