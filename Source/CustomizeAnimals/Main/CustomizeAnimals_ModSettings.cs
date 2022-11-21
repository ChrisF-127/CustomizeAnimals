using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace CustomizeAnimals
{
	public class CustomizeAnimals_ModSettings : ModSettings
	{
		public override void ExposeData()
		{
			base.ExposeData();
			
			var animals = CustomizeAnimals.Animals;
			if (Scribe.mode == LoadSaveMode.Saving)
			{
				var global = CustomizeAnimals.Global;
				Scribe_Deep.Look(ref global, "Global");

				for (int i = 0; i < animals.Count; i++)
				{
					var animal = animals[i];
					if (animal?.IsModified() == true)
						Scribe_Deep.Look(ref animal, animal.Animal.defName, animal);
				}
			}
			else
			{
				var global = CustomizeAnimals.Global;
				Scribe_Deep.Look(ref global, "Global");
				if (global != null)
					CustomizeAnimals.Global = global;

				for (int i = 0; i < animals.Count; i++)
				{
					var animal = animals[i];
					if (animal?.Animal != null)
					{
						Scribe_Deep.Look(ref animal, animal.Animal.defName, animal);
						if (animal != null)
							animals[i] = animal;
					}
					else
						Log.Error($"{nameof(CustomizeAnimals_ModSettings)}.{nameof(ExposeData)}: 'animal.Animal' should not be null!");
				}
			}
		}
	}
}
