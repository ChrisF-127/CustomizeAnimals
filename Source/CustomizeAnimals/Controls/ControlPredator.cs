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
	internal class ControlPredator : BaseSettingControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var predatorSetting = (BaseSetting<bool>)animalSettings.GeneralSettings["Predator"];
			var maxPreyBodySizeSetting = (SettingMaxPreyBodySize)animalSettings.GeneralSettings["MaxPreyBodySize"];

			(var use, var value) = CreateNumericWithCheckbox(
				offsetY,
				viewWidth,
				"SY_CA.Predator".Translate(),
				"SY_CA.MaxPreyBodySizeNotPredator".Translate(),
				"SY_CA.TooltipMaxPreyBodySize".Translate(),
				"SY_CA.TooltipPredator".Translate(),
				predatorSetting.IsModified() || maxPreyBodySizeSetting.IsModified(),
				maxPreyBodySizeSetting.Value,
				maxPreyBodySizeSetting.DefaultValue,
				predatorSetting.Value,
				predatorSetting.DefaultValue,
				min: SettingMaxPreyBodySize.DefaultMinimum,
				max: SettingMaxPreyBodySize.DefaultMaximum);

			predatorSetting.Value = use;
			maxPreyBodySizeSetting.Value = value;

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth) => 0f;
	}
}
