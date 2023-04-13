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
	internal class ControlMateMtbHours : BaseSettingControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			if (animalSettings.IsHumanLike)
				return 0f;

			var setting = (BaseSetting<float>)animalSettings.ReproductionSettings["MateMtbHours"];
			var value = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.MateMtbHours".Translate(),
				"SY_CA.TooltipMateMtbHours".Translate(),
				setting.IsModified(),
				setting.Value,
				setting.DefaultValue,
				unit: "h");

			setting.Value = value;

			return SettingsRowHeight;
		}

		private string GlobalModifierBuffer;
		public override float CreateSettingGlobal(float offsetY, float viewWidth)
		{
			(var use, var value) = CreateNumericGlobal(
				offsetY,
				viewWidth,
				"SY_CA.MateMtbHoursGlobal".Translate(),
				"SY_CA.TooltipMateMtbHoursGlobal".Translate(),
				SettingMateMtbHours.UseGlobalModifier,
				SettingMateMtbHours.GlobalModifier,
				SettingMateMtbHours.GlobalModifierDefault,
				ref GlobalModifierBuffer,
				min: SettingMateMtbHours.MinimumModifier,
				max: SettingMateMtbHours.MaximumModifier);
			SettingMateMtbHours.UseGlobalModifier = use;
			SettingMateMtbHours.GlobalModifier = value;

			return SettingsThinRowHeight;
		}
	}
}
