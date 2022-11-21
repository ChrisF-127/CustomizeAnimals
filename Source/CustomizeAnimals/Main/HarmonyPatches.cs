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

			harmony.Patch(
				AccessTools.Method(typeof(ContentFinder<Texture2D>), nameof(ContentFinder<Texture2D>.Get)),
				postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.ContentFinder_Texture2D_Get_PostFix)));
		}

		public static void TrainableUtility_DegradationPeriodTicks_PostFix(ref int __result)
		{
			if (GlobalSettings.GlobalGeneralSettings.TrainingDecayFactor != 1f)
				__result = Mathf.RoundToInt(Mathf.Clamp(__result / GlobalSettings.GlobalGeneralSettings.TrainingDecayFactor, 1f, 1e9f));
		}

		public static void MassUtility_Capacity_PostFix(Pawn p, ref float __result)
		{
			if (GlobalSettings.GlobalGeneralSettings.CarryingCapacityAffectsMassCapacity && AnimalSettings.IsValidAnimal(p.def))
				__result *= p.def.statBases.GetStatValueFromList(StatDefOf.CarryingCapacity, StatDefOf.CarryingCapacity.defaultBaseValue) / StatDefOf.CarryingCapacity.defaultBaseValue;
		}

		public static void ContentFinder_Texture2D_Get_PostFix(string itemPath, ref Texture2D __result)
		{
			const string dummy = "CA_pack_dummy";
			if (__result == null && itemPath.EndsWith("Pack"))
			{
				__result = CustomizeAnimals.Instance.Content.GetContentHolder<Texture2D>().Get(dummy);
				Log.Warning($"{nameof(CustomizeAnimals)}: Pack animal texture not found: {itemPath} - replacing with dummy: {dummy} - success: {__result != null}");
			}
		}
	}
}
