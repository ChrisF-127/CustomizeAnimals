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
	internal class SettingCaravanRidingSpeed : NullableFloatSetting
	{
		#region PROPERTIES
		#endregion

		#region CONSTRUCTORS
		public SettingCaravanRidingSpeed(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override void GetValue() =>
			Value = GetStat(StatDefOf.CaravanRidingSpeedFactor, false);
		public override void SetValue() =>
			SetStat(StatDefOf.CaravanRidingSpeedFactor);

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "RidingSpeed", DefaultValue);
			Value = value;
		}
		#endregion
	}
}
