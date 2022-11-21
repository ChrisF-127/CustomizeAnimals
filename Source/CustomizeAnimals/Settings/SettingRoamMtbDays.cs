using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizeAnimals.Settings
{
	internal class SettingRoamMtbDays : NullableFloatSetting
	{
		#region PROPERTIES
		public static bool UseMinimumRoamMtbDays { get; set; } = false;
		public static float? MinimumRoamMtbDays { get; set; } = null;

		public const float DefaultMinimum = 1f;
		public const float DefaultMaximum = 120f;

		public const float DefaultMinimumGlobal = DefaultMaximum;
		#endregion

		#region CONSTRUCTORS
		public SettingRoamMtbDays(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override void GetValue()
		{
			Value = Animal?.race?.roamMtbDays;
		}
		public override void SetValue()
		{
			if (Animal?.race != null)
			{
				var oldValue = Animal.race.roamMtbDays;
				var newValue = UseMinimumRoamMtbDays ? MinimumRoamMtbDays > 0 ? MinimumRoamMtbDays > Value ? MinimumRoamMtbDays : Value : null : Value > 0 ? Value : null; // don't ask.
				if (oldValue == null && newValue != null || oldValue != null && newValue == null)
					AnimalPenUtility.ResetStaticData();
				Animal.race.roamMtbDays = newValue;
			}
		}

		public override void ExposeData()
		{
			var roamMtbDays = Value;
			Scribe_Values.Look(ref roamMtbDays, "RoamMtbDays", DefaultValue);
			Value = roamMtbDays;
		}

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

		public override bool IsGlobalUsed() =>
			UseMinimumRoamMtbDays;
		#endregion
	}
}
