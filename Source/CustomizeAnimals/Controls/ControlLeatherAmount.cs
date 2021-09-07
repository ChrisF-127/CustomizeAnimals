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
	internal class ControlLeatherAmount : BaseSettingControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (NullableFloatSetting)animalSettings.Settings["LeatherAmount"];

			var bodySize = animalSettings?.Animal?.race?.baseBodySize ?? 1f;
			float convert(float val) => Mathf.Round(StatDefOf.LeatherAmount.postProcessCurve.Evaluate(val * bodySize));

			var value = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.LeatherAmount".Translate(),
				"SY_CA.TooltipLeatherAmount".Translate(),
				setting.IsModified(),
				setting.Value ?? StatDefOf.LeatherAmount.defaultBaseValue, // Value should never be null at this point
				setting.DefaultValue ?? StatDefOf.LeatherAmount.defaultBaseValue, // DefaultValue should never be null at this point
				min: StatDefOf.LeatherAmount.minValue,
				max: StatDefOf.LeatherAmount.maxValue,
				convert: convert);

			setting.Value = value;

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth) => 0f;
	}
}
