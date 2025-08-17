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
					var animalSettings = animals[i];
					if (animalSettings?.IsModified() == true)
						Scribe_Deep.Look(ref animalSettings, animalSettings.Animal.defName, animalSettings);
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
					var animalSettings = animals[i];
					if (animalSettings?.Animal != null)
					{
						Scribe_Deep.Look(ref animalSettings, animalSettings.Animal.defName, animalSettings);
						if (animalSettings != null)
							animals[i] = animalSettings;
					}
					else
						Log.Error($"{nameof(CustomizeAnimals_ModSettings)}.{nameof(ExposeData)}: 'animal.Animal' should not be null!");
				}
			}
		}
	}
}
