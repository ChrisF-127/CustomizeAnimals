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
	internal class SettingToxicEnvironmentResistance : NullableFloatSetting
	{
		#region PROPERTIES
		public static bool UseLimits { get; set; } = false;
		public static float Minimum { get; set; } = StatDefOf.ToxicEnvironmentResistance.minValue;
		public static float Maximum { get; set; } = StatDefOf.ToxicEnvironmentResistance.maxValue;

		protected override string ScribeLabel =>
			"ToxicEnvironmentResistance";
		#endregion

		#region CONSTRUCTORS
		public SettingToxicEnvironmentResistance(ThingDef animal, bool isGlobal = false) : 
			base(animal, isGlobal)
		{ }
		#endregion

		#region INTERFACES
		public override void GetValue() =>
			Value = GetStat(StatDefOf.ToxicEnvironmentResistance, true);
		public override void SetValue() =>
			SetStat(StatDefOf.ToxicEnvironmentResistance, Animal.IsAnimal() && UseLimits, Minimum, Maximum);

		public override void ResetGlobal()
		{
			UseLimits = false;
			Minimum = StatDefOf.ToxicEnvironmentResistance.minValue;
			Maximum = StatDefOf.ToxicEnvironmentResistance.maxValue;
		}
		public override void ExposeGlobal()
		{
			var useGlobal = UseLimits;
			Scribe_Values.Look(ref useGlobal, "UseToxicEnvironmentResistanceLimits");
			UseLimits = useGlobal;

			var minValue = Minimum;
			Scribe_Values.Look(ref minValue, "MinimumToxicEnvironmentResistance", StatDefOf.ToxicEnvironmentResistance.minValue);
			Minimum = minValue;
			var maxValue = Maximum;
			Scribe_Values.Look(ref maxValue, "MaximumToxicEnvironmentResistance", StatDefOf.ToxicEnvironmentResistance.maxValue);
			Maximum = maxValue;
		}
		public override bool IsGlobalUsed() =>
			UseLimits;
		#endregion
	}
}
