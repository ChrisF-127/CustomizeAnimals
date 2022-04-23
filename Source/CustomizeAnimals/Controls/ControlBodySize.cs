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
			var setting = (BaseSetting<float>)animalSettings.GeneralSettings["BodySize"];
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

		private string GlobalModifierBuffer;
		public override float CreateSettingGlobal(float offsetY, float viewWidth)
		{
			(var use, var value) = CreateNumericGlobal(
				offsetY,
				viewWidth,
				"SY_CA.BodySizeGlobal".Translate(),
				"SY_CA.TooltipBodySizeGlobal".Translate(),
				SettingBodySize.UseGlobalModifier,
				SettingBodySize.GlobalModifier,
				SettingBodySize.GlobalModifierDefault,
				ref GlobalModifierBuffer,
				min: SettingBodySize.MinimumModifier,
				max: SettingBodySize.MaximumModifier);
			SettingBodySize.UseGlobalModifier = use;
			SettingBodySize.GlobalModifier = value;

			return SettingsThinRowHeight;
		}
	}
}
