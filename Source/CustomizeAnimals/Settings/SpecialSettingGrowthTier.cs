using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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
		public static int[] DefaultPassionGainsPerTier { get; private set; }
		public static int[] PassionGainsPerTier { get; set; }

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
		public SpecialSettingGrowthTier(ThingDef animal) : base(animal)
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

			GrowthTierPointsRequirements = GrowthUtility.GrowthTierPointsRequirements.ToArray();
			PassionGainsPerTier = GrowthUtility.PassionGainsPerTier.ToArray();
			PassionChoicesPerTier = GrowthUtility.PassionChoicesPerTier.ToArray();
			TraitGainsPerTier = GrowthUtility.TraitGainsPerTier.ToArray();
			TraitChoicesPerTier = GrowthUtility.TraitChoicesPerTier.ToArray();

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
			GrowthUtility.GrowthTierPointsRequirements.SetFrom(GrowthTierPointsRequirements);
			GrowthUtility.PassionGainsPerTier.SetFrom(PassionGainsPerTier);
			GrowthUtility.PassionChoicesPerTier.SetFrom(PassionChoicesPerTier);
			GrowthUtility.TraitGainsPerTier.SetFrom(TraitGainsPerTier);
			GrowthUtility.TraitChoicesPerTier.SetFrom(TraitChoicesPerTier);
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
				|| PassionGainsPerTier.IsDifferent(DefaultPassionGainsPerTier)
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

			CustomizeAnimalsUtility.ExposeArray(
				"PassionGainsPerTier",
				() => PassionGainsPerTier.IsDifferent(DefaultPassionGainsPerTier),
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
		private void ApplyGrowthMomentAges(int[] moments)
		{
			GrowthUtility.GrowthMomentAges.SetFrom(GrowthMomentAges, fromOffset: 1); // skip first (Baby)!

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
