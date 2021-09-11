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
	internal class ControlArmorRating_Sharp : ControlArmorRating
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (NullableFloatSetting)animalSettings.Settings["ArmorRating_Sharp"];
			var value = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.ArmorRating_Sharp".Translate(),
				"SY_CA.TooltipArmorRating_Sharp".Translate(),
				setting.IsModified(),
				setting.Value ?? StatDefOf.ArmorRating_Sharp.defaultBaseValue, // Value should never be null at this point
				setting.DefaultValue ?? StatDefOf.ArmorRating_Sharp.defaultBaseValue, // DefaultValue should never be null at this point
				min: SettingArmorRating.DefaultMinimum,
				max: SettingArmorRating.DefaultMaximum,
				convert: ConvertToPercent,
				unit: "%");

			setting.Value = value;

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth) => 0f;
	}

	internal class ControlArmorRating_Blunt : ControlArmorRating
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (NullableFloatSetting)animalSettings.Settings["ArmorRating_Blunt"];
			var value = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.ArmorRating_Blunt".Translate(),
				"SY_CA.TooltipArmorRating_Blunt".Translate(),
				setting.IsModified(),
				setting.Value ?? StatDefOf.ArmorRating_Blunt.defaultBaseValue, // Value should never be null at this point
				setting.DefaultValue ?? StatDefOf.ArmorRating_Blunt.defaultBaseValue, // DefaultValue should never be null at this point
				min: SettingArmorRating.DefaultMinimum,
				max: SettingArmorRating.DefaultMaximum,
				convert: ConvertToPercent,
				unit: "%");

			setting.Value = value;

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth) => 0f;
	}

	internal class ControlArmorRating_Heat : ControlArmorRating
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (NullableFloatSetting)animalSettings.Settings["ArmorRating_Heat"];
			var value = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.ArmorRating_Heat".Translate(),
				"SY_CA.TooltipArmorRating_Heat".Translate(),
				setting.IsModified(),
				setting.Value ?? StatDefOf.ArmorRating_Heat.defaultBaseValue, // Value should never be null at this point
				setting.DefaultValue ?? StatDefOf.ArmorRating_Heat.defaultBaseValue, // DefaultValue should never be null at this point
				min: SettingArmorRating.DefaultMinimum,
				max: SettingArmorRating.DefaultMaximum,
				convert: ConvertToPercent,
				unit: "%");

			setting.Value = value;

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth) => 0f;
	}


	internal abstract class ControlArmorRating : BaseSettingControl
	{ }
}
