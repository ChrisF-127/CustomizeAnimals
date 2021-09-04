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
	internal class ControlHealthScale : BaseSettingControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (BaseSetting<float>)animalSettings.Settings["HealthScale"];
			var value = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.HealthScale".Translate(),
				"SY_CA.TooltipHealthScale".Translate(),
				setting.IsModified(),
				setting.Value,
				setting.DefaultValue,
				min: 0.001f);

			setting.Value = value;

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth) => 0f;
	}
}
