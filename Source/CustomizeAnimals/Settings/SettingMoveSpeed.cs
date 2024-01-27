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
	internal class SettingMoveSpeed : NullableFloatSetting
	{
		#region PROPERTIES
		public static bool UseGlobalModifier { get; set; } = false;
		public static float GlobalModifier { get; set; } = GlobalModifierDefault;

		public const float GlobalModifierDefault = 1f;
		public const float MinimumModifier = 1e-3f;
		public const float MaximumModifier = 1e3f;
		#endregion

		#region CONSTRUCTORS
		public SettingMoveSpeed(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region INTERFACES
		public override void GetValue() =>
			Value = GetStat(StatDefOf.MoveSpeed, true);
		public override void SetValue() => 
			SetStat(StatDefOf.MoveSpeed, modifier: Animal.IsAnimal() && UseGlobalModifier ? GlobalModifier : 1f);

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "MoveSpeed", DefaultValue);
			Value = value;
		}

		public override void ResetGlobal()
		{
			UseGlobalModifier = false;
			GlobalModifier = GlobalModifierDefault;
		}

		public override void ExposeGlobal()
		{
			var useGlobal = UseGlobalModifier;
			Scribe_Values.Look(ref useGlobal, "UseGlobalMoveSpeedModifier");
			UseGlobalModifier = useGlobal;

			var value = GlobalModifier;
			Scribe_Values.Look(ref value, "GlobalMoveSpeedModifier", GlobalModifierDefault);
			GlobalModifier = value;
		}

		public override bool IsGlobalUsed() =>
			UseGlobalModifier;
		#endregion
	}
}
