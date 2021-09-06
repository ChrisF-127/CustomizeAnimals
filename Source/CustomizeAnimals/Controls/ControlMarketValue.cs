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
	internal class ControlMarketValue : BaseSettingControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (NullableFloatSetting)animalSettings.Settings["MarketValue"];
			var value = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.MarketValue".Translate(),
				"SY_CA.TooltipMarketValue".Translate(),
				setting.IsModified(),
				setting.Value ?? StatDefOf.MarketValue.defaultBaseValue, // Value should never be null at this point
				setting.DefaultValue ?? StatDefOf.MarketValue.defaultBaseValue, // DefaultValue should never be null at this point
				min: StatDefOf.MarketValue.minValue,
				max: StatDefOf.MarketValue.maxValue,
				unit: "$");

			setting.Value = value;

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth) => 0f;
	}
}
