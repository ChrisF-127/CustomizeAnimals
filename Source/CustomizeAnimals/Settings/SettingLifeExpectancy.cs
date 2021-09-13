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
	internal class SettingLifeExpectancy : BaseSetting<float>
	{
		#region PROPERTIES
		public const float DefaultMinimum = 0f;
		public const float DefaultMaximum = 1e9f;
		#endregion

		#region CONSTRUCTORS
		public SettingLifeExpectancy(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override void GetValue()
		{
			var race = Animal?.race;
			if (race != null)
				Value = race.lifeExpectancy;
			else if (!IsGlobal)
				Log.Warning($"{nameof(CustomizeAnimals)}.{nameof(SettingLifeExpectancy)}: {Animal?.defName} race is null, value cannot be set!");
		}
		public override void SetValue()
		{
			var race = Animal?.race;
			if (race != null)
				race.lifeExpectancy = Value;
		}

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "LifeExpectancy", DefaultValue);
			Value = value;
		}
		#endregion
	}
}
