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
	internal class ControlMeatAmount : BaseSettingControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (SettingMeatAmount)animalSettings.GeneralSettings["MeatAmount"];

			if (setting.HasMeatDef())
			{
				var bodySize = animalSettings?.Animal?.race?.baseBodySize ?? 1f;
				float convert(float val) => Mathf.Round(StatDefOf.MeatAmount.postProcessCurve.Evaluate(val * bodySize));

				var value = CreateNumeric(
					offsetY,
					viewWidth,
					"SY_CA.MeatAmount".Translate(),
					"SY_CA.TooltipMeatAmount".Translate(),
					setting.IsModified(),
					setting.Value ?? StatDefOf.MeatAmount.defaultBaseValue, // Value should never be null at this point
					setting.DefaultValue ?? StatDefOf.MeatAmount.defaultBaseValue, // DefaultValue should never be null at this point
					min: StatDefOf.MeatAmount.minValue,
					max: StatDefOf.MeatAmount.maxValue,
					convert: convert);

				setting.Value = value;
			}
			else
			{
				CreateText(
					offsetY,
					viewWidth,
					"SY_CA.MeatAmount".Translate(),
					$"({"SY_CA.MeatAmountNoDef".Translate()})");
			}

			return SettingsRowHeight;
		}

		private string GlobalModifierBuffer;
		public override float CreateSettingGlobal(float offsetY, float viewWidth)
		{
			(var use, var value) = CreateNumericGlobal(
				offsetY,
				viewWidth,
				"SY_CA.MeatAmountGlobal".Translate(),
				"SY_CA.TooltipMeatAmountGlobal".Translate(),
				SettingMeatAmount.UseGlobalModifier,
				SettingMeatAmount.GlobalModifier,
				SettingMeatAmount.GlobalModifierDefault,
				ref GlobalModifierBuffer,
				min: SettingMeatAmount.MinimumModifier,
				max: SettingMeatAmount.MaximumModifier);
			SettingMeatAmount.UseGlobalModifier = use;
			SettingMeatAmount.GlobalModifier = value;

			return SettingsThinRowHeight;
		}
	}
}
