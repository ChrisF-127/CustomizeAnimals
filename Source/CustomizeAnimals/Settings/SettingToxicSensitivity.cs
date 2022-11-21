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
	internal class SettingToxicResistance : NullableFloatSetting
	{
		#region PROPERTIES
		public static bool UseLimits { get; set; } = false;
		public static float Minimum { get; set; } = StatDefOf.ToxicResistance.minValue;
		public static float Maximum { get; set; } = StatDefOf.ToxicResistance.maxValue;
		#endregion

		#region CONSTRUCTORS
		public SettingToxicResistance(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override void GetValue() =>
			Value = GetStat(StatDefOf.ToxicResistance, true);
		public override void SetValue() =>
			SetStat(StatDefOf.ToxicResistance, UseLimits, Minimum, Maximum);

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "ToxicResistance", DefaultValue);
			Value = value;
		}

		public override void ResetGlobal()
		{
			UseLimits = false;
			Minimum = StatDefOf.ToxicResistance.minValue;
			Maximum = StatDefOf.ToxicResistance.maxValue;
		}

		public override void ExposeGlobal()
		{
			var useGlobal = UseLimits;
			Scribe_Values.Look(ref useGlobal, "UseToxicResistanceLimits");
			UseLimits = useGlobal;

			var minValue = Minimum;
			Scribe_Values.Look(ref minValue, "MinimumToxicResistance", StatDefOf.ToxicResistance.minValue);
			Minimum = minValue;
			var maxValue = Maximum;
			Scribe_Values.Look(ref maxValue, "MaximumToxicResistance", StatDefOf.ToxicResistance.maxValue);
			Maximum = maxValue;
		}

		public override bool IsGlobalUsed() =>
			UseLimits;
		#endregion
	}
}
