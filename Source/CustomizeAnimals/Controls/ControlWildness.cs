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
	internal class ControlWildness : BaseSettingControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (BaseSetting<float>)animalSettings.Settings["Wildness"];
			var value = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.Wildness".Translate(),
				"SY_CA.TooltipWildness".Translate(),
				setting.IsModified(),
				setting.Value,
				setting.DefaultValue,
				min: SettingWildness.DefaultMinimum,
				max: SettingWildness.DefaultMaximum,
				convert: ConvertToPercent,
				unit: "%");

			setting.Value = value;

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth) => 0f;
	}
}
