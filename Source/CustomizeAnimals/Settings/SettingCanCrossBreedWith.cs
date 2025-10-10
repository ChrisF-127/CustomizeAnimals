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
	/// Cross-breeding depends on the "can cross-breed with" setting of the male only
	/// </summary>
	internal class SettingCanCrossBreedWith : ListSetting<ThingDef>
	{
		#region PROPERTIES
		protected override string ScribeLabel =>
			"CanCrossBreedWith";
		#endregion

		#region CONSTRUCTORS
		public SettingCanCrossBreedWith(ThingDef animal) : 
			base(animal)
		{ }
		#endregion

		#region INTERFACES
		public override void GetValue()
		{
			if (!IsGlobal)
				Value = Animal?.race?.canCrossBreedWith ?? new List<ThingDef>();
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
				race.canCrossBreedWith = output;
			}
		}
		#endregion
	}
}
