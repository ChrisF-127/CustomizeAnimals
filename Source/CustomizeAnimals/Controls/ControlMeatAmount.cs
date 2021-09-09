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
			var setting = (SettingMeatAmount)animalSettings.Settings["MeatAmount"];

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

		public override float CreateSettingGlobal(float offsetY, float viewWidth) => 0f;
	}
}
