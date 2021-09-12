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
	internal class SettingFoodType : BaseSetting<FoodTypeFlags>
	{
		#region PROPERTIES
		#endregion

		#region CONSTRUCTORS
		public SettingFoodType(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override FoodTypeFlags GetValue()
		{
			var race = Animal?.race;
			if (race != null)
				return race.foodType;

			if (!IsGlobal)
				Log.Warning($"{nameof(CustomizeAnimals)}.{nameof(SettingFoodType)}: {Animal?.defName} race is null, value cannot be set!");
			return FoodTypeFlags.None;
		}
		public override void SetValue()
		{
			var race = Animal?.race;
			if (race != null)
				race.foodType = Value;
		}

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "FoodType", DefaultValue);
			Value = value;
		}
		#endregion
	}
}
