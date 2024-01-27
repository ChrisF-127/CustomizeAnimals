using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Mono.Cecil.Cil;
using System.Reflection.Emit;
using System.Reflection;
using CustomizeAnimals.Settings;

namespace CustomizeAnimals
{
	[StaticConstructorOnStartup]
	public static class HarmonyPatches
	{
		static HarmonyPatches()
		{
			var harmony = new Harmony("syrus.customize_animals");

			harmony.Patch(
				AccessTools.Method(typeof(TrainableUtility), nameof(TrainableUtility.DegradationPeriodTicks)),
				postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.TrainableUtility_DegradationPeriodTicks_PostFix)));
			harmony.Patch(
				AccessTools.Method(typeof(MassUtility), nameof(MassUtility.Capacity)),
				postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.MassUtility_Capacity_PostFix)));
			harmony.Patch(
				AccessTools.Method(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.BirthdayBiological)),
				transpiler: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.Pawn_AgeTracker_BirthdayBiological_Transpiler)));
			harmony.Patch(
				AccessTools.Method(typeof(Graphic_Multi), nameof(Graphic_Multi.Init)),
				transpiler: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.Graphic_Multi_Init_Transpiler)));
		}

		public static void TrainableUtility_DegradationPeriodTicks_PostFix(ref int __result)
		{
			if (GlobalSettings.GlobalGeneralSettings.TrainingDecayFactor != 1f)
				__result = Mathf.RoundToInt(Mathf.Clamp(__result / GlobalSettings.GlobalGeneralSettings.TrainingDecayFactor, 1f, 1e9f));
		}

		public static void MassUtility_Capacity_PostFix(Pawn p, ref float __result)
		{
			if (GlobalSettings.GlobalGeneralSettings.CarryingCapacityAffectsMassCapacity && (p.def.IsAnimal() || p.def.IsHumanLike()))
				__result *= p.def.statBases.GetStatValueFromList(StatDefOf.CarryingCapacity, StatDefOf.CarryingCapacity.defaultBaseValue) / StatDefOf.CarryingCapacity.defaultBaseValue;
		}

		public static IEnumerable<CodeInstruction> Pawn_AgeTracker_BirthdayBiological_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var patched = false;
			var list = instructions.ToList();
			for (int i = 0; i < list.Count - 5; i++)
			{
				//  0 -- ldc.i4.1 NULL
				//    ++ call static System.Int32[] CustomizeAnimals.Settings.SpecialSettingGrowthTier::get_TraitGainsPerTier()
				//    ++ ldarg.0 NULL
				//    ++ call System.Int32 Verse.Pawn_AgeTracker::get_GrowthTier()
				//    ++ ldelem.i4 NULL
				//  1    ldloca.s 15 (System.Nullable`1[Verse.PawnGenerationRequest])
				//  2    initobj System.Nullable`1[Verse.PawnGenerationRequest]
				//  3    ldloc.s 15 (System.Nullable`1[Verse.PawnGenerationRequest])
				//  4    ldc.i4.1 NULL
				//  5    call static System.Collections.Generic.List`1<RimWorld.Trait> Verse.PawnGenerator::GenerateTraitsFor(...)
				if (   list[i + 0].opcode == OpCodes.Ldc_I4_1
					&& list[i + 1].opcode == OpCodes.Ldloca_S 
					&& list[i + 2].opcode == OpCodes.Initobj 
					&& list[i + 3].opcode == OpCodes.Ldloc_S 
					&& list[i + 4].opcode == OpCodes.Ldc_I4_1
					&& list[i + 5].opcode == OpCodes.Call
					&& list[i + 5].operand is MethodInfo methodInfo
					&& methodInfo == AccessTools.Method(typeof(PawnGenerator), nameof(PawnGenerator.GenerateTraitsFor)))
				{
					list.Insert(i++, new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(SpecialSettingGrowthTier), nameof(SpecialSettingGrowthTier.TraitGainsPerTier))));
					list.Insert(i++, new CodeInstruction(OpCodes.Ldarg_0));
					list.Insert(i++, new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.GrowthTier))));
					list[i].opcode = OpCodes.Ldelem_I4;
					patched = true;
					break;
				}
			}

			if (!patched)
				Log.Error($"{nameof(CustomizeAnimals)}: failed to apply '{nameof(Pawn_AgeTracker_BirthdayBiological_Transpiler)}'-patch");
			return list;
		}

		public static IEnumerable<CodeInstruction> Graphic_Multi_Init_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var patched = false;
			var list = instructions.ToList();
			for (int i = 4; i < list.Count; i++)
			{
				// 4  ldfld System.String Verse.GraphicRequest::path
				// 3  ldc.i4.0 NULL
				// 2  call static UnityEngine.Texture2D Verse.ContentFinder`1<UnityEngine.Texture2D>::Get(System.String itemPath, System.Boolean reportFailure)
				// 1  stelem.ref NULL
				//  + ldloc.0 NULL
				//  + ldc.i4.0 NULL
				//  + ldelema UnityEngine.Texture2D
				//  + ldarg.1 NULL
				//  + ldfld System.String Verse.GraphicRequest::path
				//  + call static System.Void CustomizeAnimals.HarmonyPatches::Patchy(UnityEngine.Texture2D& texture2D, System.String path)
				// 0  ldloc.0 NULL [Label0, Label2, Label4, Label6]
				if (   list[i - 4].opcode == OpCodes.Ldfld
					&& list[i - 4].operand is FieldInfo fieldInfo
					&& fieldInfo.DeclaringType == typeof(GraphicRequest)
					&& fieldInfo.Name == nameof(GraphicRequest.path)
					&& list[i - 3].opcode == OpCodes.Ldc_I4_0
					&& list[i - 2].opcode == OpCodes.Call
					&& list[i - 2].operand is MethodInfo methodInfo
					&& methodInfo.IsStatic
					&& methodInfo.DeclaringType == typeof(ContentFinder<Texture2D>)
					&& methodInfo.Name == nameof(ContentFinder<Texture2D>.Get)
					&& list[i - 1].opcode == OpCodes.Stelem_Ref
					&& list[i].opcode == OpCodes.Ldloc_0
					&& list[i].labels.Count == 4)
				{
					list.InsertRange(i, new CodeInstruction[]
					{
						new CodeInstruction(OpCodes.Ldloc_0),
						new CodeInstruction(OpCodes.Ldc_I4_0),
						new CodeInstruction(OpCodes.Ldelema, typeof(Texture2D)),
						new CodeInstruction(OpCodes.Ldarg_1),
						new CodeInstruction(OpCodes.Ldfld, typeof(GraphicRequest).GetField(nameof(GraphicRequest.path))),
						new CodeInstruction(OpCodes.Call, typeof(HarmonyPatches).GetMethod(nameof(HarmonyPatches.Graphic_Multi_Init_PackTexture_Patch), BindingFlags.Static | BindingFlags.NonPublic)),
					});
					patched = true;
					break;
				}
			}

			if (!patched)
				Log.Error($"{nameof(CustomizeAnimals)}: failed to apply '{nameof(Graphic_Multi_Init_Transpiler)}'-patch");
			return list;
		}

		private static void Graphic_Multi_Init_PackTexture_Patch(ref Texture2D texture2D, string path)
		{
			const string dummy = "CA_pack_dummy";
			if (texture2D == null && path.EndsWith("Pack"))
			{
				texture2D = CustomizeAnimals.Instance.Content.GetContentHolder<Texture2D>().Get(dummy);
				Log.Warning($"{nameof(CustomizeAnimals)}: pack animal texture not found: {path} - replacing with dummy: {dummy} - success: {texture2D != null}");
			}
		}
	}
}
