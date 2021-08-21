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
				var animal = CustomizeAnimals.Global;
				Scribe_Deep.Look(ref animal, "Global", animal);

				for (int i = 0; i < animals.Count; i++)
				{
					animal = animals[i];
					if (animal?.IsModified() == true)
						Scribe_Deep.Look(ref animal, animal.Animal.defName, animal);
				}
			}
			else
			{
				var animal = CustomizeAnimals.Global;
				Scribe_Deep.Look(ref animal, "Global", animal);
				if (animal != null)
					CustomizeAnimals.Global = animal;

				for (int i = 0; i < animals.Count; i++)
				{
					animal = animals[i];
					if (animal?.Animal != null)
					{
						Scribe_Deep.Look(ref animal, animal.Animal.defName, animal);
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
