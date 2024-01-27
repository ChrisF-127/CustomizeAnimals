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
	internal class SettingWildness : BaseSetting<float>
	{
		#region PROPERTIES
		public const float DefaultMinimum = 0f;
		public const float DefaultMaximum = 10f;
		#endregion

		#region CONSTRUCTORS
		public SettingWildness(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region INTERFACES
		public override void GetValue()
		{
			var race = Animal?.race;
			if (race != null)
				Value = race.wildness;
			else if (!IsGlobal)
				Log.Warning($"{nameof(CustomizeAnimals)}.{nameof(SettingWildness)}: {Animal?.defName} race is null, value cannot be set!");
		}
		public override void SetValue()
		{
			var race = Animal?.race;
			if (race != null)
				race.wildness = Value;
		}

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "Wildness", DefaultValue);
			Value = value;
		}
		#endregion
	}
}
