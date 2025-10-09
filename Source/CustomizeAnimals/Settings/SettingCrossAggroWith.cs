using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CustomizeAnimals.Settings
{
	/// <summary>
	/// Animals on this list will join in on herd manhunter events, like hunting revenge 
	/// </summary>
	internal class SettingCrossAggroWith : ThingDefListSetting
	{
		#region PROPERTIES
		protected override string ScribeLabel =>
			"CrossAggroWith";
		#endregion

		#region CONSTRUCTORS
		public SettingCrossAggroWith(ThingDef animal) : 
			base(animal)
		{ }
		#endregion

		#region INTERFACES
		public override void GetValue()
		{
			if (!IsGlobal)
				Value = Animal?.race?.crossAggroWith ?? new List<ThingDef>();
		}
		public override void SetValue()
		{
			var race = Animal?.race;
			if (race != null && Animal.IsAnimal())
			{
				var output = new List<ThingDef>();
				if (Value.Count > 0)
					foreach (var value in Value)
						output.Add(value);
				race.crossAggroWith = output;
			}
		}
		#endregion
	}
}
