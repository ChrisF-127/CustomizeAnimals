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
		public float GetCaravanMassCapacity()
		{
			if (Animal?.race == null)
				return -1f;
			
			var massCapacity = Animal.race.baseBodySize * 35f;
			if (GlobalSettings.GlobalGeneralSettings.CarryingCapacityAffectsMassCapacity && Animal.statBases != null)
				massCapacity *= Animal.statBases.GetStatValueFromList(StatDefOf.CarryingCapacity, StatDefOf.CarryingCapacity.defaultBaseValue) / StatDefOf.CarryingCapacity.defaultBaseValue;
			return massCapacity;
		}
		#endregion

		#region INTERFACES
		public override void GetValue()
		{
			var race = Animal?.race;
			if (race != null)
				Value = race.packAnimal;
			else if (!IsGlobal)
				Log.Warning($"{nameof(CustomizeAnimals)}.{nameof(SettingPackAnimal)}: {Animal?.defName} race is null, value cannot be set!");
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
