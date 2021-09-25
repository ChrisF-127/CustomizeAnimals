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
	internal class SettingGestationPeriodDays : BaseSetting<float>
	{
		#region PROPERTIES
		#endregion

		#region CONSTRUCTORS
		public SettingGestationPeriodDays(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override void GetValue()
		{
			var race = Animal?.race;
			if (race != null)
				Value = race.gestationPeriodDays;
			else if (!IsGlobal)
				Log.Warning($"{nameof(CustomizeAnimals)}.{nameof(SettingGestationPeriodDays)}: {Animal?.defName} race is null, value cannot be set!");
		}
		public override void SetValue()
		{
			var race = Animal?.race;
			if (race != null)
				race.gestationPeriodDays = Value;
		}

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "GestationPeriodDays", DefaultValue);
			Value = value;
		}
		#endregion
	}
}
