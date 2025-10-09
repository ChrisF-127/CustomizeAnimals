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
	internal class SettingVacuumResistance : NullableFloatSetting
	{
		#region PROPERTIES
		public static bool UseLimits { get; set; } = false;
		public static float Minimum { get; set; } = StatDefOf.VacuumResistance.minValue;
		public static float Maximum { get; set; } = StatDefOf.VacuumResistance.maxValue;

		protected override string ScribeLabel =>
			"VacuumResistance";
		#endregion

		#region CONSTRUCTORS
		public SettingVacuumResistance(ThingDef animal, bool isGlobal = false) : 
			base(animal, isGlobal)
		{ }
		#endregion

		#region INTERFACES
		public override void GetValue() =>
			Value = GetStat(StatDefOf.VacuumResistance, true);
		public override void SetValue() =>
			SetStat(StatDefOf.VacuumResistance, Animal.IsAnimal() && UseLimits, Minimum, Maximum);

		public override void ResetGlobal()
		{
			UseLimits = false;
			Minimum = StatDefOf.VacuumResistance.minValue;
			Maximum = StatDefOf.VacuumResistance.maxValue;
		}
		public override void ExposeGlobal()
		{
			var useGlobal = UseLimits;
			Scribe_Values.Look(ref useGlobal, "UseVacuumResistanceLimits");
			UseLimits = useGlobal;

			var minValue = Minimum;
			Scribe_Values.Look(ref minValue, "MinimumVacuumResistance", StatDefOf.VacuumResistance.minValue);
			Minimum = minValue;
			var maxValue = Maximum;
			Scribe_Values.Look(ref maxValue, "MaximumVacuumResistance", StatDefOf.VacuumResistance.maxValue);
			Maximum = maxValue;
		}
		public override bool IsGlobalUsed() =>
			UseLimits;
		#endregion
	}
}
