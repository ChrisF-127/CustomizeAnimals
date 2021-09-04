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
	internal class SettingBodySize : BaseSetting<float>
	{
		#region PROPERTIES
		#endregion

		#region CONSTRUCTORS
		public SettingBodySize(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override float GetValue()
		{
			var race = Animal?.race;
			if (race != null)
				return race.baseBodySize;

			if (!IsGlobal)
				Log.Warning($"{nameof(CustomizeAnimals)}.{nameof(SettingBodySize)}: {Animal?.defName} race is null, value cannot be set!");
			return 1f;
		}
		public override void SetValue()
		{
			var race = Animal?.race;
			if (race != null)
				race.baseBodySize = Value;
		}

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "BodySize", DefaultValue);
			Value = value;
		}
		#endregion
	}
}
