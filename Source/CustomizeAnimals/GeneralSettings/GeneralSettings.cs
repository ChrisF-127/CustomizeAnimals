using CustomizeAnimals.Controls;
using CustomizeAnimals.Settings;
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
	public class GeneralSettings : IExposable
	{
		#region PROPERTIES
		private TrainableDef RescueDef { get; set; }
		private float DefaultRescueMinBodySize { get; set; } = 0f;
		public bool DisableRescueSizeLimit { get; set; }

		private TrainableDef HaulDef { get; set; }
		private float DefaultHaulMinBodySize { get; set; } = 0f;
		public bool DisableHaulSizeLimit { get; set; }
		#endregion

		#region FIELDS
		#endregion

		#region PUBLIC METHODS
		public void Intialize()
		{
			RescueDef = DefDatabase<TrainableDef>.AllDefs.First((d) => d.defName == "Rescue");
			if (RescueDef != null)
				DefaultRescueMinBodySize = RescueDef.minBodySize;
			else
				Log.Error($"{nameof(CustomizeAnimals)}: TrainableDef 'Rescue' not found!");

			HaulDef = DefDatabase<TrainableDef>.AllDefs.First((d) => d.defName == "Haul");
			if (RescueDef != null)
				DefaultHaulMinBodySize = HaulDef.minBodySize;
			else
				Log.Error($"{nameof(CustomizeAnimals)}: TrainableDef 'Haul' not found!");
		}

		public void ApplySettings()
		{
			RescueDef.minBodySize = DisableRescueSizeLimit ? 0f : DefaultRescueMinBodySize;
			HaulDef.minBodySize = DisableHaulSizeLimit ? 0f : DefaultHaulMinBodySize;
		}

		public void Reset()
		{
			DisableRescueSizeLimit = false;
			DisableHaulSizeLimit = false;
		}

		public bool IsModified()
		{
			if (DisableRescueSizeLimit)
				return true;
			if (DisableHaulSizeLimit)
				return true;
			return false;
		}
		#endregion

		#region INTERFACES
		public void ExposeData()
		{
			var boolValue = DisableRescueSizeLimit;
			Scribe_Values.Look(ref boolValue, "DisableRescueSizeLimit", false);
			DisableHaulSizeLimit = boolValue;

			boolValue = DisableHaulSizeLimit;
			Scribe_Values.Look(ref boolValue, "DisableHaulSizeLimit", false);
			DisableRescueSizeLimit = boolValue;
		}
		#endregion
	}
}
