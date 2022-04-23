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
		private const float HitPointFactor = 30f;
		private const float Minimum = 1e0f / HitPointFactor;
		private const float Maximum = 1e6f / HitPointFactor;

		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (BaseSetting<float>)animalSettings.GeneralSettings["HealthScale"];
			var value = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.HealthScale".Translate(),
				"SY_CA.TooltipHealthScale".Translate(),
				setting.IsModified(),
				setting.Value,
				setting.DefaultValue,
				min: Minimum,
				max: Maximum,
				convert: ConvertToHitPoints);

			setting.Value = value;

			return SettingsRowHeight;
		}

		private string GlobalModifierBuffer;
		public override float CreateSettingGlobal(float offsetY, float viewWidth)
		{
			(var use, var value) = CreateNumericGlobal(
				offsetY,
				viewWidth,
				"SY_CA.HealthScaleGlobal".Translate(),
				"SY_CA.TooltipHealthScaleGlobal".Translate(),
				SettingHealthScale.UseGlobalModifier,
				SettingHealthScale.GlobalModifier,
				SettingHealthScale.GlobalModifierDefault,
				ref GlobalModifierBuffer,
				min: SettingHealthScale.MinimumModifier,
				max: SettingHealthScale.MaximumModifier);
			SettingHealthScale.UseGlobalModifier = use;
			SettingHealthScale.GlobalModifier = value;

			return SettingsThinRowHeight;
		}

		private float ConvertToHitPoints(float value) =>
			value * HitPointFactor;
	}
}
