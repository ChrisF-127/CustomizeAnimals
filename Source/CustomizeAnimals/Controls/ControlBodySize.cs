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
	internal class ControlBodySize : BaseSettingControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (BaseSetting<float>)animalSettings.Settings["BodySize"];
			var value = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.BodySize".Translate(),
				"SY_CA.TooltipBodySize".Translate(),
				setting.IsModified(),
				setting.Value,
				setting.DefaultValue,
				min: SettingBodySize.DefaultMinimum,
				max: SettingBodySize.DefaultMaximum);

			setting.Value = value;

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth) => 0f;
	}
}
