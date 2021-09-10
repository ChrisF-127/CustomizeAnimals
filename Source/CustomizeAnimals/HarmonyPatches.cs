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
		}


		public static void TrainableUtility_DegradationPeriodTicks_PostFix(ref int __result)
		{
			__result = Mathf.RoundToInt(Mathf.Clamp(__result / GlobalSettings.GeneralSettings.TrainingDecayFactor, 1f, 1e9f));
		}
	}
}
