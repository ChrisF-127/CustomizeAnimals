using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizeAnimals.Settings
{
	public class SettingRoamMtbDays : BaseSetting<float?>
	{
		#region PROPERTIES
		public static bool UseMinimumRoamMtbDays { get; set; } 
		public static float? MinimumRoamMtbDays { get; set; }
		#endregion

		#region CONSTRUCTORS
		public SettingRoamMtbDays(ThingDef animal) : base(animal)
		{ }
		#endregion

		#region PUBLIC METHODS
		public override void ResetGlobal()
		{
			UseMinimumRoamMtbDays = false;
			MinimumRoamMtbDays = null;
		}

		public override void ExposeGlobal()
		{
			var useGlobal = UseMinimumRoamMtbDays;
			Scribe_Values.Look(ref useGlobal, "UseMinimumRoamMtbDays");
			UseMinimumRoamMtbDays = useGlobal;

			var roamMtbDays = MinimumRoamMtbDays;
			Scribe_Values.Look(ref roamMtbDays, "MinimumRoamMtbDays");
			MinimumRoamMtbDays = roamMtbDays > 0 ? roamMtbDays : null;
		}
		#endregion

		#region INTERFACES
		public override float? GetValue()
		{
			return Animal?.race?.roamMtbDays;
		}
		public override void SetValue()
		{
			if (Animal?.race != null)
				Animal.race.roamMtbDays = UseMinimumRoamMtbDays ? MinimumRoamMtbDays > 0 ? MinimumRoamMtbDays > Value ? MinimumRoamMtbDays : Value : null : Value > 0 ? Value : null; // don't ask.
		}

		public override void ExposeData()
		{
			var roamMtbDays = Value;
			Scribe_Values.Look(ref roamMtbDays, "RoamMtbDays", DefaultValue);
			Value = roamMtbDays;
		}

		public override bool IsGlobalUsed() =>
			UseMinimumRoamMtbDays;
		#endregion
	}
}
