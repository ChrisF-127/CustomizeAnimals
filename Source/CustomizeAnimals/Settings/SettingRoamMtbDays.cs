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
		public static bool UseMinimumRoamMtbDays { get; set; } = false;
		public static float? MinimumRoamMtbDays { get; set; } = null;

		public SettingRoamMtbDays(ThingDef animal) : base(animal)
		{ }

		public override float? Get()
		{
			return Animal?.race?.roamMtbDays;
		}
		public override void Set()
		{
			if (Animal?.race != null)
				Animal.race.roamMtbDays = UseMinimumRoamMtbDays ? MinimumRoamMtbDays > 0 ? Value > MinimumRoamMtbDays ? Value : MinimumRoamMtbDays : null : Value > 0 ? Value : null; // don't ask.
		}

		public override void ExposeData()
		{
			var roamMtbDays = Value;
			Scribe_Values.Look(ref roamMtbDays, "RoamMtbDays", DefaultValue);
			Value = roamMtbDays;
		}
		public static void ExposeGlobal()
		{
			var useGlobal = UseMinimumRoamMtbDays;
			Scribe_Values.Look(ref useGlobal, "UseMinimumRoamMtbDays");
			UseMinimumRoamMtbDays = useGlobal;

			var roamMtbDays = MinimumRoamMtbDays;
			Scribe_Values.Look(ref roamMtbDays, "MinimumRoamMtbDays");
			MinimumRoamMtbDays = roamMtbDays > 0 ? roamMtbDays : null;
		}
	}
}
