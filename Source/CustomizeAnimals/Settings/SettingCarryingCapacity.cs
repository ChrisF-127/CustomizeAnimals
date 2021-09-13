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
	internal class SettingCarryingCapacity : NullableFloatSetting
	{
		#region PROPERTIES
		public static bool UseLimits { get; set; }
		public static float Minimum { get; set; }
		public static float Maximum { get; set; }
		#endregion

		#region CONSTRUCTORS
		public SettingCarryingCapacity(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override void GetValue() =>
			Value = GetStat(StatDefOf.CarryingCapacity, true);
		public override void SetValue() =>
			SetStat(StatDefOf.CarryingCapacity, UseLimits, Minimum, Maximum);

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "CarryingCapacity", DefaultValue);
			Value = value;
		}

		public override void ResetGlobal()
		{
			UseLimits = false;
			Minimum = StatDefOf.CarryingCapacity.minValue;
			Maximum = StatDefOf.CarryingCapacity.maxValue;
		}

		public override void ExposeGlobal()
		{
			var useGlobal = UseLimits;
			Scribe_Values.Look(ref useGlobal, nameof(UseLimits));
			UseLimits = useGlobal;

			var minValue = Minimum;
			Scribe_Values.Look(ref minValue, nameof(Minimum), StatDefOf.CarryingCapacity.minValue);
			Minimum = minValue;
			var maxValue = Maximum;
			Scribe_Values.Look(ref maxValue, nameof(Maximum), StatDefOf.CarryingCapacity.maxValue);
			Maximum = maxValue;
		}

		public override bool IsGlobalUsed() =>
			UseLimits;
		#endregion
	}
}
