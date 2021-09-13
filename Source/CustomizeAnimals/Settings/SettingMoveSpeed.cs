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
			SetStat(StatDefOf.MoveSpeed);

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "MoveSpeed", DefaultValue);
			Value = value;
		}
		#endregion
	}
}
