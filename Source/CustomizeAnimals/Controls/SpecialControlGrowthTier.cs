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
			CreateArraySetting(
				ref totalHeight,
				ref viewWidth,
				"SY_CA.GrowthMomentAges".Translate(),
				"SY_CA.TooltipGrowthMomentAges".Translate(),
				SpecialSettingGrowthTier.GrowthMomentAges,
				SpecialSettingGrowthTier.DefaultGrowthMomentAges,
				ref GrowthMomentAgesBuffers,
				sublabels: new string[]
				{
					"SY_CA.GrowthMomentAgesBaby".Translate(),
					"SY_CA.GrowthMomentAgesChild".Translate(),
					"SY_CA.GrowthMomentAgesTeen".Translate(),
					"SY_CA.GrowthMomentAgesAdult".Translate(),
				},
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
		#endregion
	}
}
