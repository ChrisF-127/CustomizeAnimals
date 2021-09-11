using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;
using RimWorld;
using UnityEngine;

namespace CustomizeAnimals
{
	[StaticConstructorOnStartup]
	public static class HarmonyPatches
	{
		static HarmonyPatches()
		{
			Harmony harmony = new Harmony("syrus.customize_animals");

			harmony.Patch(
				AccessTools.Method(typeof(TrainableUtility), nameof(TrainableUtility.DegradationPeriodTicks)),
				postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.TrainableUtility_DegradationPeriodTicks_PostFix)));

			harmony.Patch(
				AccessTools.Method(typeof(MassUtility), nameof(MassUtility.Capacity)),
				postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.MassUtility_Capacity_PostFix)));
		}

		public static void TrainableUtility_DegradationPeriodTicks_PostFix(ref int __result)
		{
			if (GlobalSettings.GeneralSettings.TrainingDecayFactor != 1f)
				__result = Mathf.RoundToInt(Mathf.Clamp(__result / GlobalSettings.GeneralSettings.TrainingDecayFactor, 1f, 1e9f));
		}

		public static void MassUtility_Capacity_PostFix(ref Pawn p, ref float __result)
		{
			if (GlobalSettings.GeneralSettings.CarryingCapacityAffectsMassCapacity && AnimalSettings.IsValidAnimal(p.def))
				__result *= p.def.statBases.GetStatValueFromList(StatDefOf.CarryingCapacity, StatDefOf.CarryingCapacity.defaultBaseValue) / StatDefOf.CarryingCapacity.defaultBaseValue;
		}
	}
}
