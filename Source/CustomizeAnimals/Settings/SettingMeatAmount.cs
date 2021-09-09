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
	internal class SettingMeatAmount : NullableFloatSetting
	{
		#region PROPERTIES
		#endregion

		#region CONSTRUCTORS
		public SettingMeatAmount(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		public bool HasMeatDef() => Animal?.race?.meatDef != null;
		#endregion

		#region INTERFACES
		public override float? GetValue() =>
			GetStat(StatDefOf.MeatAmount, true);
		public override void SetValue() =>
			SetStat(StatDefOf.MeatAmount);

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "MeatAmount", DefaultValue);
			Value = value;
		}
		#endregion
	}
}
