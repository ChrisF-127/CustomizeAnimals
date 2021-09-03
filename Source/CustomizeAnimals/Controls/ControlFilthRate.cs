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
	internal class ControlFilthRate : BaseControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (NullableFloatSetting)animalSettings.Settings["FilthRate"];
			var value = CreateNullableNumeric(
				offsetY,
				viewWidth,
				"SY_CA.FilthRate".Translate(),
				"SY_CA.FilthRateDisabled".Translate(),
				"SY_CA.TooltipFilthRate".Translate(),
				"SY_CA.TooltipFilthRateChk".Translate(),
				setting.IsModified(),
				setting.Value,
				setting.DefaultValue,
				min: 1f);

			setting.Value = value;

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth)
		{
			(var use, var value) = CreateNullableNumericGlobal(
				offsetY,
				viewWidth,
				"SY_CA.MinimumFilthRate".Translate(),
				"SY_CA.FilthRateDisabled".Translate(),
				"SY_CA.TooltipMinimumFilthRate".Translate(),
				"SY_CA.TooltipMinimumFilthRateChk".Translate(),
				SettingFilthRate.UseMaximumFilthRate,
				SettingFilthRate.MaximumFilthRate,
				SettingFilthRate.DefaultMaximum,
				min: 1f);

			SettingFilthRate.UseMaximumFilthRate = use;
			SettingFilthRate.MaximumFilthRate = value;

			return SettingsRowHeight;
		}
	}
}
