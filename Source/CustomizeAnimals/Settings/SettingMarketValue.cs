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
	internal class SettingMarketValue : NullableFloatSetting
	{
		#region PROPERTIES
		#endregion

		#region CONSTRUCTORS
		public SettingMarketValue(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override void GetValue() =>
			Value = GetStat(StatDefOf.MarketValue, true);
		public override void SetValue() =>
			SetStat(StatDefOf.MarketValue);

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "MarketValue", DefaultValue);
			Value = value;
		}
		#endregion
	}
}
