using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizeAnimals.Settings
{
	internal class SpecialSettingGrowthTier : BaseSpecialSetting
	{
		#region PROPERTIES
		// Applied directly via GrowthUtility.GrowthTierPointsRequirements
		public static float[] DefaultGrowthTierPointsRequirements { get; private set; }
		public static float[] GrowthTierPointsRequirements { get; set; }

		// Applied directly via GrowthUtility.PassionGainsPerTier
		public static ByteRange[] DefaultPassionGainsPerTier { get; private set; }
		public static ByteRange[] PassionGainsPerTier { get; set; }

		// Applied directly via GrowthUtility.PassionChoicesPerTier
		public static int[] DefaultPassionChoicesPerTier { get; private set; }
		public static int[] PassionChoicesPerTier { get; set; }

		// Applied via Pawn_AgeTracker.BirthdayBiological-Transpiler
		public static int[] DefaultTraitGainsPerTier { get; private set; }
		public static int[] TraitGainsPerTier { get; set; }

		// Applied directly via GrowthUtility.TraitChoicesPerTier
		public static int[] DefaultTraitChoicesPerTier { get; private set; }
		public static int[] TraitChoicesPerTier { get; set; }

		// Applied indirectly via Pawn_AgeTracker.growthMomentAges instead of GrowthUtility.GrowthMomentAges!
		public static int[] DefaultGrowthMomentAges { get; private set; }
		public static int[] GrowthMomentAges { get; set; }
		#endregion

		#region CONSTRUCTORS
		public SpecialSettingGrowthTier(ThingDef animal) : 
			base(animal)
		{
			GetValue();

			DefaultGrowthTierPointsRequirements = GrowthTierPointsRequirements.ToArray();
			DefaultPassionGainsPerTier = PassionGainsPerTier.ToArray();
			DefaultPassionChoicesPerTier= PassionChoicesPerTier.ToArray();
			DefaultTraitGainsPerTier = TraitGainsPerTier.ToArray();
			DefaultTraitChoicesPerTier = TraitChoicesPerTier.ToArray();
			DefaultGrowthMomentAges = GrowthMomentAges.ToArray();
		}
		#endregion

		#region OVERRIDES
		public override void GetValue()
		{
			if (Animal.IsAnimal())
				return;

			var growthTiers = GrowthUtility.GrowthTiers;

			GrowthTierPointsRequirements = growthTiers.Select(t => t.pointsRequirement).ToArray();
			PassionGainsPerTier = growthTiers.Select(t => t.passionGainsRange).ToArray();
			PassionChoicesPerTier = growthTiers.Select(t => (int)t.passionChoices).ToArray();
			TraitGainsPerTier = growthTiers.Select(t => (int)t.traitGains).ToArray();
			TraitChoicesPerTier = growthTiers.Select(t => (int)t.traitChoices).ToArray();

			GrowthMomentAges = new int[GrowthUtility.GrowthMomentAges.Length + 1];
			GrowthMomentAges[0] = 3; // this value is hardcoded (Pawn_AgeTracker.TrySimulateGrowthPoints)
			GrowthMomentAges.SetFrom(GrowthUtility.GrowthMomentAges, toOffset: 1); // skip first (Baby)!
		}
		public override void SetValue()
		{
			if (Animal.IsAnimal())
				return;

			if (GrowthTierPointsRequirements[0] != 0) // always has to be 0
			{
				Log.Error($"{nameof(CustomizeAnimals)}: Unexpected value for {nameof(GrowthTierPointsRequirements)}[0]: '{GrowthTierPointsRequirements[0]}', should be 0; correcting to 0 now");
				GrowthTierPointsRequirements[0] = 0;
			}

			var growthTiers = GrowthUtility.GrowthTiers;
			for (int i = 0; i < growthTiers.Length; i++)
			{
				var growthTier = growthTiers[i];
				var pointsRequirements = GrowthTierPointsRequirements[i];
				var passionGainsRange = PassionGainsPerTier[i];
				var passionChoices = (byte)PassionChoicesPerTier[i];
				var traitGains = (byte)TraitGainsPerTier[i];
				var traitChoices = (byte)TraitChoicesPerTier[i];

				// check for changes
				if (growthTier.pointsRequirement != pointsRequirements
					|| growthTier.passionGainsRange != passionGainsRange
					|| growthTier.passionChoices != passionChoices
					|| growthTier.traitGains != traitGains
					|| growthTier.traitChoices != traitChoices)
				{
					// apply changes
					growthTiers[i] = new GrowthUtility.GrowthTier(
						(byte)i,
						pointsRequirements,
						passionGainsRange,
						passionChoices,
						traitGains,
						traitChoices);
				}
			}

			ApplyGrowthMomentAges(GrowthMomentAges);
		}

		public override void Reset()
		{
			GrowthTierPointsRequirements.SetFrom(DefaultGrowthTierPointsRequirements);
			PassionGainsPerTier.SetFrom(DefaultPassionGainsPerTier);
			PassionChoicesPerTier.SetFrom(DefaultPassionChoicesPerTier);
			TraitGainsPerTier.SetFrom(DefaultTraitGainsPerTier);
			TraitChoicesPerTier.SetFrom(DefaultTraitChoicesPerTier);
			ApplyGrowthMomentAges(DefaultGrowthMomentAges);
		}
		public override bool IsModified()
		{
			if (GrowthTierPointsRequirements.IsDifferent(DefaultGrowthTierPointsRequirements)
				|| PassionGainsPerTier.Compare(DefaultPassionGainsPerTier, (a, b) => a != b)
				|| PassionChoicesPerTier.IsDifferent(DefaultPassionChoicesPerTier)
				|| TraitGainsPerTier.IsDifferent(DefaultTraitGainsPerTier)
				|| TraitChoicesPerTier.IsDifferent(DefaultTraitChoicesPerTier)
				|| GrowthMomentAges.IsDifferent(DefaultGrowthMomentAges))
				return true;
			return false;
		}

		public override void ExposeData()
		{
			CustomizeAnimalsUtility.ExposeArray(
				"GrowthTierPointsRequirements",
				() => GrowthTierPointsRequirements.IsDifferent(DefaultGrowthTierPointsRequirements),
				GrowthTierPointsRequirements,
				DefaultGrowthTierPointsRequirements);

			CustomizeAnimalsUtility.ExposeByteRangeArray(
				"PassionGainsPerTierMin",
				PassionGainsPerTier,
				DefaultPassionGainsPerTier);

			CustomizeAnimalsUtility.ExposeArray(
				"PassionChoicesPerTier",
				() => PassionChoicesPerTier.IsDifferent(DefaultPassionChoicesPerTier),
				PassionChoicesPerTier,
				DefaultPassionChoicesPerTier);

			CustomizeAnimalsUtility.ExposeArray(
				"TraitGainsPerTier",
				() => TraitGainsPerTier.IsDifferent(DefaultTraitGainsPerTier),
				TraitGainsPerTier,
				DefaultTraitGainsPerTier);

			CustomizeAnimalsUtility.ExposeArray(
				"TraitChoicesPerTier",
				() => TraitChoicesPerTier.IsDifferent(DefaultTraitChoicesPerTier),
				TraitChoicesPerTier,
				DefaultTraitChoicesPerTier);

			CustomizeAnimalsUtility.ExposeArray(
				"GrowthMomentAges",
				() => GrowthMomentAges.IsDifferent(DefaultGrowthMomentAges),
				GrowthMomentAges,
				DefaultGrowthMomentAges,
				new string[]
				{
					"Baby",
					"Child",
					"Teen",
					"Adult",
				});
		}
		#endregion

		#region PRIVATE METHODS
		// Growth Moments are not set via GrowthUtility.GrowthMomentAges,
		//  but use the Pawn_AgeTracker.growthMomentAges instead for some reason.
		//  So, that list needs to be adjusted to actually apply the changes.
		private static void ApplyGrowthMomentAges(int[] moments)
		{
			GrowthUtility.GrowthMomentAges.SetFrom(moments, fromOffset: 1); // skip first (Baby)

			if (Pawn_AgeTracker.growthMomentAges == null)
				Pawn_AgeTracker.growthMomentAges = new List<int>(moments);
			else
			{
				var growthMomentAges = Pawn_AgeTracker.growthMomentAges;
				for (int i = 0; i < moments.Length && i < growthMomentAges.Count; i++)
					growthMomentAges[i] = moments[i];
			}
		}
		#endregion
	}
}
