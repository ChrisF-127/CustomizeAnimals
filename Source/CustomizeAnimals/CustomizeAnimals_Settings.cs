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

			// TODO? Can probably be optimized to only fill the exposable list if it is going to be saved & only apply the loaded list if it's being loaded?
			var animals = CustomizeAnimals.Animals;
			var animalsExposable = new List<AnimalSettingsExposable>();

			// Get all modified animals into a list of exposables in case of saving
			foreach (var animal in animals)
			{
				if (animal.IsModified())
					animalsExposable.Add(animal.ToExposable());
			}

			// Save/Load list
			Scribe_Collections.Look(ref animalsExposable, "AnimalSettings");

			// Apply list to Animals
			for (int i = 0; i < animals.Count; i++)
			{
				var animal = animalsExposable.Find((a) => a.Animal == animals[i].Animal);
				if (animal != null)
				{
					animals[i].FromExposable(animal);
					animalsExposable.Remove(animal);
				}
			}
		}
	}
}
