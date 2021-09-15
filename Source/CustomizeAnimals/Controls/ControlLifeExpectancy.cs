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
	internal class ControlLifeExpectancy : BaseSettingControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (BaseSetting<float>)animalSettings.Settings["LifeExpectancy"];
			var value = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.LifeExpectancy".Translate(),
				"SY_CA.TooltipLifeExpectancy".Translate(),
				setting.IsModified(),
				setting.Value,
				setting.DefaultValue,
				min: SettingLifeExpectancy.DefaultMinimum,
				max: SettingLifeExpectancy.DefaultMaximum,
				unit: "y");

			setting.Value = value;

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth) => 0f;
	}
}
