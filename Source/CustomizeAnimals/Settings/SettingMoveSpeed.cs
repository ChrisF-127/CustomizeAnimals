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
		public static bool UseGlobal { get; set; }
		public static float GlobalModifier { get; set; }

		public const float GlobalDefault = 1f;
		public const float GlobalMinimum = 1e-3f;
		public const float GlobalMaximum = 1e3f;
		#endregion

		#region CONSTRUCTORS
		public SettingMoveSpeed(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override void GetValue() =>
			Value = GetStat(StatDefOf.MoveSpeed, true);
		public override void SetValue() => 
			SetStat(StatDefOf.MoveSpeed, modifier: UseGlobal ? GlobalModifier : 1f);

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "MoveSpeed", DefaultValue);
			Value = value;
		}

		public override void ResetGlobal()
		{
			UseGlobal = false;
			GlobalModifier = GlobalDefault;
		}

		public override void ExposeGlobal()
		{
			var useGlobal = UseGlobal;
			Scribe_Values.Look(ref useGlobal, "UseGlobalMoveSpeedModifier");
			UseGlobal = useGlobal;

			var value = GlobalModifier;
			Scribe_Values.Look(ref value, "GlobalMoveSpeedModifier", GlobalDefault);
			GlobalModifier = value;
		}

		public override bool IsGlobalUsed() =>
			UseGlobal;
		#endregion
	}
}
