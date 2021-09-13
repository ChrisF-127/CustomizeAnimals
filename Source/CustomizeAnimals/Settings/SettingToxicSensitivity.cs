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
	internal class SettingToxicSensitivity : NullableFloatSetting
	{
		#region PROPERTIES
		public static bool UseLimits { get; set; }
		public static float Minimum { get; set; }
		public static float Maximum { get; set; }
		#endregion

		#region CONSTRUCTORS
		public SettingToxicSensitivity(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override void GetValue() =>
			Value = GetStat(StatDefOf.ToxicSensitivity, true);
		public override void SetValue() =>
			SetStat(StatDefOf.ToxicSensitivity, UseLimits, Minimum, Maximum);

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "ToxicSensitivity", DefaultValue);
			Value = value;
		}

		public override void ResetGlobal()
		{
			UseLimits = false;
			Minimum = StatDefOf.ToxicSensitivity.minValue;
			Maximum = StatDefOf.ToxicSensitivity.maxValue;
		}

		public override void ExposeGlobal()
		{
			var useGlobal = UseLimits;
			Scribe_Values.Look(ref useGlobal, nameof(UseLimits));
			UseLimits = useGlobal;

			var minValue = Minimum;
			Scribe_Values.Look(ref minValue, nameof(Minimum), StatDefOf.ToxicSensitivity.minValue);
			Minimum = minValue;
			var maxValue = Maximum;
			Scribe_Values.Look(ref maxValue, nameof(Maximum), StatDefOf.ToxicSensitivity.maxValue);
			Maximum = maxValue;
		}

		public override bool IsGlobalUsed() =>
			UseLimits;
		#endregion
	}
}
