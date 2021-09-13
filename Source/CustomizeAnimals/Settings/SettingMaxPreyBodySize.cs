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
	internal class SettingMaxPreyBodySize : BaseSetting<float>
	{
		#region PROPERTIES
		public const float DefaultMinimum = 1e-3f;
		public const float DefaultMaximum = 99999f;

		public bool IsPredator => Animal?.race?.predator ?? false;
		#endregion

		#region CONSTRUCTORS
		public SettingMaxPreyBodySize(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override void GetValue()
		{
			var race = Animal?.race;
			if (race != null)
				Value = race.maxPreyBodySize;
			else if (!IsGlobal)
				Log.Warning($"{nameof(CustomizeAnimals)}.{nameof(SettingMaxPreyBodySize)}: {Animal?.defName} race is null, value cannot be set!");
		}
		public override void SetValue()
		{
			var race = Animal?.race;
			if (race != null)
				race.maxPreyBodySize = Value;
		}

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "MaxPreyBodySize", DefaultValue);
			Value = value;
		}
		#endregion
	}
}
