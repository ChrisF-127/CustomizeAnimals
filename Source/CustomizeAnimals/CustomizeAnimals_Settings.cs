using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace CustomizeAnimals
{
	public class CustomizeAnimals_Settings : ModSettings
	{
		public static List<AnimalSettings> Animals { get; private set; } = new List<AnimalSettings>();

		public override void ExposeData()
		{
			base.ExposeData();
			Log.Message("CustomizeAnimals_Settings.ExposeData: " + Scribe.mode);
		}

		public static void Initialize()
		{
			foreach (var thingDef in DefDatabase<ThingDef>.AllDefs)
			{
				if (AnimalSettings.IsValidAnimal(thingDef))
					Animals.Add(new AnimalSettings(thingDef));
			}
			Animals.SortBy((a) => a.Animal.label);
		}
	}
}
