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
	internal class SettingLeatherAmount : NullableFloatSetting
	{
		#region PROPERTIES
		#endregion

		#region CONSTRUCTORS
		public SettingLeatherAmount(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override float? GetValue() =>
			GetStat(StatDefOf.LeatherAmount, true);
		public override void SetValue() =>
			SetStat(StatDefOf.LeatherAmount);

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "LeatherAmount", DefaultValue);
			Value = value;
		}
		#endregion
	}
}
