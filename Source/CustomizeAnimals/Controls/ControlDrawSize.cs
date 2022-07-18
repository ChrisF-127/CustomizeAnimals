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
	internal class ControlDrawSize: BaseSettingControl
	{
		#region OVERRIDES
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (BaseSetting<float>)animalSettings.GeneralSettings["DrawSize"];
			var value = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.DrawSize".Translate(),
				"SY_CA.TooltipDrawSize".Translate(),
				setting.IsModified(),
				setting.Value,
				setting.DefaultValue);

			setting.Value = value;

			return SettingsRowHeight;
		}

		private string GlobalModifierBuffer;
		public override float CreateSettingGlobal(float offsetY, float viewWidth)
		{
			(var use, var value) = CreateNumericGlobal(
				offsetY,
				viewWidth,
				"SY_CA.DrawSizeGlobal".Translate(),
				"SY_CA.TooltipHealthScaleGlobal".Translate(),
				SettingDrawSize.UseGlobal,
				SettingDrawSize.Global,
				SettingDrawSize.GlobalDefault,
				ref GlobalModifierBuffer,
				min: SettingDrawSize.Minimum,
				max: SettingDrawSize.Maximum);
			SettingDrawSize.UseGlobal = use;
			SettingDrawSize.Global = value;

			return SettingsThinRowHeight;
		}
		#endregion
	}
}
