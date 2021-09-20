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
	internal class ControlArmorRating_Sharp : BaseControl
	{
		#region FIELDS
		private string SharpBuffer;
		private string BluntBuffer;
		private string HeatBuffer;
		#endregion

		#region OVERRIDES
		public override void Reset()
		{
			SharpBuffer = null;
			BluntBuffer = null;
			HeatBuffer = null;
		}

		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var settingSharp = (NullableFloatSetting)animalSettings.Settings["ArmorRating_Sharp"];
			settingSharp.Value = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.ArmorRating_Sharp".Translate(),
				"SY_CA.TooltipArmorRating_Sharp".Translate(),
				settingSharp.IsModified(),
				settingSharp.Value ?? StatDefOf.ArmorRating_Sharp.defaultBaseValue, // Value should never be null at this point
				settingSharp.DefaultValue ?? StatDefOf.ArmorRating_Sharp.defaultBaseValue, // DefaultValue should never be null at this point
				ref SharpBuffer,
				min: SettingArmorRating.DefaultMinimum,
				max: SettingArmorRating.DefaultMaximum,
				convert: ConvertToPercent,
				unit: "%");

			offsetY += SettingsDoubleRowHeight - SettingsRowHeight;

			var settingBlunt = (NullableFloatSetting)animalSettings.Settings["ArmorRating_Blunt"];
			settingBlunt.Value = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.ArmorRating_Blunt".Translate(),
				"SY_CA.TooltipArmorRating_Blunt".Translate(),
				settingBlunt.IsModified(),
				settingBlunt.Value ?? StatDefOf.ArmorRating_Blunt.defaultBaseValue, // Value should never be null at this point
				settingBlunt.DefaultValue ?? StatDefOf.ArmorRating_Blunt.defaultBaseValue, // DefaultValue should never be null at this point
				ref BluntBuffer,
				min: SettingArmorRating.DefaultMinimum,
				max: SettingArmorRating.DefaultMaximum,
				convert: ConvertToPercent,
				unit: "%");

			offsetY += SettingsTripleRowHeight - SettingsDoubleRowHeight;

			var settingHeat = (NullableFloatSetting)animalSettings.Settings["ArmorRating_Heat"];
			settingHeat.Value = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.ArmorRating_Heat".Translate(),
				"SY_CA.TooltipArmorRating_Heat".Translate(),
				settingHeat.IsModified(),
				settingHeat.Value ?? StatDefOf.ArmorRating_Heat.defaultBaseValue, // Value should never be null at this point
				settingHeat.DefaultValue ?? StatDefOf.ArmorRating_Heat.defaultBaseValue, // DefaultValue should never be null at this point
				ref HeatBuffer,
				min: SettingArmorRating.DefaultMinimum,
				max: SettingArmorRating.DefaultMaximum,
				convert: ConvertToPercent,
				unit: "%");

			return SettingsTripleRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth) => 0f;
		#endregion
	}
}
