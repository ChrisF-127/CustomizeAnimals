using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace CustomizeAnimals
{
	public class CustomizeAnimals_Settings : ModSettings
	{
		public override void ExposeData()
		{
			base.ExposeData();
			
			var animals = CustomizeAnimals.Animals;
			if (Scribe.mode == LoadSaveMode.Saving)
			{
				for (int i = 0; i < animals.Count; i++)
				{
					var animal = animals[i];
					if (animal.IsModified())
						Scribe_Deep.Look(ref animal, animal.Animal.defName, animal.Animal);
				}
			}
			else
			{
				for (int i = 0; i < animals.Count; i++)
				{
					var animal = animals[i];
					if (animal.Animal != null)
					{
						Scribe_Deep.Look(ref animal, animal.Animal.defName, animal.Animal);
						if (animal != null)
							animals[i] = animal;
					}
					else
						Log.Error($"{nameof(CustomizeAnimals_Settings)}.{nameof(ExposeData)}: animal.Animal should not be null !");
				}
			}
		}
	}
}
