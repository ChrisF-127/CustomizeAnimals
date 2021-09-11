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
	internal class SettingPackAnimal : BaseSetting<bool>
	{
		#region PROPERTIES
		#endregion

		#region CONSTRUCTORS
		public SettingPackAnimal(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override bool GetValue()
		{
			var race = Animal?.race;
			if (race != null)
				return race.packAnimal;

			if (!IsGlobal)
				Log.Warning($"{nameof(CustomizeAnimals)}.{nameof(SettingPackAnimal)}: {Animal?.defName} race is null, value cannot be set!");
			return false;
		}
		public override void SetValue()
		{
			var race = Animal?.race;
			if (race != null)
				race.packAnimal = Value;
		}

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "PackAnimal", DefaultValue);
			Value = value;
		}
		#endregion
	}
}
