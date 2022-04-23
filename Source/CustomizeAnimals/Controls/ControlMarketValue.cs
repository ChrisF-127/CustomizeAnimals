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
			var setting = (NullableFloatSetting)animalSettings.GeneralSettings["MarketValue"];
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

		private string GlobalModifierBuffer;
		public override float CreateSettingGlobal(float offsetY, float viewWidth)
		{
			(var use, var value) = CreateNumericGlobal(
				offsetY,
				viewWidth,
				"SY_CA.MarketValueGlobal".Translate(),
				"SY_CA.TooltipMarketValueGlobal".Translate(),
				SettingMarketValue.UseGlobalModifier,
				SettingMarketValue.GlobalModifier,
				SettingMarketValue.GlobalModifierDefault,
				ref GlobalModifierBuffer,
				min: SettingMarketValue.MinimumModifier,
				max: SettingMarketValue.MaximumModifier);
			SettingMarketValue.UseGlobalModifier = use;
			SettingMarketValue.GlobalModifier = value;

			return SettingsThinRowHeight;
		}
	}
}
