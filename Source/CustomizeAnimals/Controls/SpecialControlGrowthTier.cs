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
	internal class SpecialControlGrowthTier : BaseSpecialSettingControl
	{
		#region PROPERTIES
		#endregion

		#region FIELDS
		// Growth Tier variables are static, therefore all controls and their buffers can also be static
		public static string[] GrowthTierPointsRequirementsBuffers;
		public static string[] PassionGainsPerTierBuffers;
		public static string[] PassionChoicesPerTierBuffers;
		public static string[] TraitGainsPerTierBuffers;
		public static string[] TraitChoicesPerTierBuffers;
		public static string[] GrowthMomentAgesBuffers;
		#endregion

		#region METHODS
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			if (!animalSettings.IsHumanLike)
				return 0f;

			var totalHeight = offsetY;

			// Separator
			Widgets.ListSeparator(ref totalHeight, viewWidth - 16, "SY_CA.GrowthTier".Translate());
			totalHeight += 2;
			Text.Anchor = TextAnchor.MiddleLeft;

			// Growth Tier Points Requirements
			CreateArraySetting(
				ref totalHeight,
				ref viewWidth,
				"SY_CA.GrowthTierPointsRequirements".Translate(),
				"SY_CA.TooltipGrowthTierPointsRequirements".Translate(),
				SpecialSettingGrowthTier.GrowthTierPointsRequirements,
				SpecialSettingGrowthTier.DefaultGrowthTierPointsRequirements,
				ref GrowthTierPointsRequirementsBuffers,
				startingIndex: 1);

			// Passion Gains Per Tier
			CreateArraySetting(
				ref totalHeight,
				ref viewWidth,
				"SY_CA.PassionGainsPerTier".Translate(),
				"SY_CA.TooltipPassionGainsPerTier".Translate(),
				SpecialSettingGrowthTier.PassionGainsPerTier,
				SpecialSettingGrowthTier.DefaultPassionGainsPerTier,
				ref PassionGainsPerTierBuffers);

			// Passion Choices Per Tier
			CreateArraySetting(
				ref totalHeight,
				ref viewWidth,
				"SY_CA.PassionChoicesPerTier".Translate(),
				"SY_CA.TooltipPassionChoicesPerTier".Translate(),
				SpecialSettingGrowthTier.PassionChoicesPerTier,
				SpecialSettingGrowthTier.DefaultPassionChoicesPerTier,
				ref PassionChoicesPerTierBuffers);

			// Trait Gains Per Tier
			CreateArraySetting(
				ref totalHeight,
				ref viewWidth,
				"SY_CA.TraitGainsPerTier".Translate(),
				"SY_CA.TooltipTraitGainsPerTier".Translate(),
				SpecialSettingGrowthTier.TraitGainsPerTier,
				SpecialSettingGrowthTier.DefaultTraitGainsPerTier,
				ref TraitGainsPerTierBuffers);

			// Trait Choices Per Tier
			CreateArraySetting(
				ref totalHeight,
				ref viewWidth,
				"SY_CA.TraitChoicesPerTier".Translate(),
				"SY_CA.TooltipTraitChoicesPerTier".Translate(),
				SpecialSettingGrowthTier.TraitChoicesPerTier,
				SpecialSettingGrowthTier.DefaultTraitChoicesPerTier,
				ref TraitChoicesPerTierBuffers);

			// Growth Moment Ages
			CreateGrowthMomentSetting(
				ref totalHeight,
				ref viewWidth,
				"SY_CA.GrowthMomentAges".Translate(),
				"SY_CA.TooltipGrowthMomentAges".Translate(),
				convert: y => y * 60,
				unit: "d");

			return totalHeight - offsetY;
		}

		public override void Reset()
		{
			GrowthTierPointsRequirementsBuffers = null;
			PassionGainsPerTierBuffers = null;
			PassionChoicesPerTierBuffers = null;
			TraitGainsPerTierBuffers = null;
			TraitChoicesPerTierBuffers = null;
			GrowthMomentAgesBuffers = null;
		}

		private void CreateGrowthMomentSetting(
			ref float totalHeight, 
			ref float viewWidth, 
			string label, 
			string tooltip, 
			ConvertDelegate<int> convert = null,
			string unit = null)
		{
			var array = SpecialSettingGrowthTier.GrowthMomentAges;
			var defaultArray = SpecialSettingGrowthTier.DefaultGrowthMomentAges;
			var length = Mathf.Min(array.Length, defaultArray.Length);

			var sublabels = new string[]
			{
				"SY_CA.GrowthMomentAgesBaby".Translate(),
				"SY_CA.GrowthMomentAgesChild".Translate(),
				"SY_CA.GrowthMomentAgesTeen".Translate(),
				"SY_CA.GrowthMomentAgesAdult".Translate(),
			};

			if (GrowthMomentAgesBuffers == null)
				GrowthMomentAgesBuffers = new string[length + 1];

			// Label
			Widgets.Label(new Rect(16f, totalHeight, viewWidth, SettingsRowHeight), label);
			totalHeight += SettingsRowHeight;

			// Baby
			Text.Anchor = TextAnchor.MiddleRight;
			SpecialSettingGrowthTier.GrowthMomentAgesBaby = CreateNumeric(
				totalHeight,
				viewWidth,
				sublabels[0],
				tooltip,
				SpecialSettingGrowthTier.GrowthMomentAgesBaby != SpecialSettingGrowthTier.DefaultGrowthMomentAgesBaby,
				SpecialSettingGrowthTier.GrowthMomentAgesBaby,
				SpecialSettingGrowthTier.DefaultGrowthMomentAgesBaby,
				ref GrowthMomentAgesBuffers[0],
				convert: convert,
				unit: unit);
			totalHeight += SettingsRowHeight;

			// Child, Teen & Adult
			for (int i = 0; i < length; i++)
			{
				Text.Anchor = TextAnchor.MiddleRight;
				array[i] = CreateNumeric(
					totalHeight,
					viewWidth,
					i < sublabels.Length ? sublabels[i + 1] : "ERROR: Unknown Growth Moment",
					tooltip,
					array[i] != defaultArray[i],
					array[i],
					defaultArray[i],
					ref GrowthMomentAgesBuffers[i + 1],
					convert: convert,
					unit: unit);
				totalHeight += SettingsRowHeight;
			}
			Text.Anchor = TextAnchor.MiddleLeft;
		}
		#endregion
	}
}
