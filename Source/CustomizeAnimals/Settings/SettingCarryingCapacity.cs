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
		public static bool UseLimits { get; set; } = false;
		public static float Minimum { get; set; } = StatDefOf.CarryingCapacity.minValue;
		public static float Maximum { get; set; } = StatDefOf.CarryingCapacity.maxValue;
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
			Scribe_Values.Look(ref useGlobal, "UseCarryingCapacityLimits");
			UseLimits = useGlobal;

			var minValue = Minimum;
			Scribe_Values.Look(ref minValue, "MinimumCarryingCapacity", StatDefOf.CarryingCapacity.minValue);
			Minimum = minValue;
			var maxValue = Maximum;
			Scribe_Values.Look(ref maxValue, "MaximumCarryingCapacity", StatDefOf.CarryingCapacity.maxValue);
			Maximum = maxValue;
		}

		public override bool IsGlobalUsed() =>
			UseLimits;
		#endregion
	}
}
