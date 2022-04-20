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
	internal class SettingMaxTemperature : NullableFloatSetting
	{
		#region PROPERTIES
		public static bool UseMaxTempLimits { get; set; }
		public static float MinimumMaxTemp { get; set; }
		public static float MaximumMaxTemp { get; set; }
		#endregion

		#region CONSTRUCTORS
		public SettingMaxTemperature(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override void GetValue() =>
			Value = GetStat(StatDefOf.ComfyTemperatureMax, true);
		public override void SetValue() =>
			SetStat(StatDefOf.ComfyTemperatureMax, UseMaxTempLimits, MinimumMaxTemp, MaximumMaxTemp);

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "MaxTemperature", DefaultValue);
			Value = value;
		}

		public override void ResetGlobal()
		{
			UseMaxTempLimits = false;
			MinimumMaxTemp = TemperatureTuning.MinimumTemperature;
			MaximumMaxTemp = TemperatureTuning.MaximumTemperature;
		}

		public override void ExposeGlobal()
		{
			var useGlobal = UseMaxTempLimits;
			Scribe_Values.Look(ref useGlobal, "UseMaxTempLimits");
			UseMaxTempLimits = useGlobal;

			var minMaxTemp = MinimumMaxTemp;
			Scribe_Values.Look(ref minMaxTemp, "MinimumMaxTemp", TemperatureTuning.MinimumTemperature);
			MinimumMaxTemp = minMaxTemp;
			var maxMaxTemp = MaximumMaxTemp;
			Scribe_Values.Look(ref maxMaxTemp, "MaximumMaxTemp", TemperatureTuning.MaximumTemperature);
			MaximumMaxTemp = maxMaxTemp;
		}

		public override bool IsGlobalUsed() =>
			UseMaxTempLimits;
		#endregion
	}


	internal class SettingMinTemperature : NullableFloatSetting
	{
		#region PROPERTIES
		public static bool UseMinTempLimits { get; set; }
		public static float MinimumMinTemp { get; set; }
		public static float MaximumMinTemp { get; set; }
		#endregion

		#region CONSTRUCTORS
		public SettingMinTemperature(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		public override void ResetGlobal()
		{
			UseMinTempLimits = false;
			MinimumMinTemp = TemperatureTuning.MinimumTemperature;
			MaximumMinTemp = TemperatureTuning.MaximumTemperature;
		}

		public override void ExposeGlobal()
		{
			var useGlobal = UseMinTempLimits;
			Scribe_Values.Look(ref useGlobal, "UseMinTempLimits");
			UseMinTempLimits = useGlobal;

			var minMinTemp = MinimumMinTemp;
			Scribe_Values.Look(ref minMinTemp, "MinimumMinTemp", TemperatureTuning.MinimumTemperature);
			MinimumMinTemp = minMinTemp;
			var maxMinTemp = MaximumMinTemp;
			Scribe_Values.Look(ref maxMinTemp, "MaximumMinTemp", TemperatureTuning.MaximumTemperature);
			MaximumMinTemp = maxMinTemp;
		}
		#endregion

		#region INTERFACES
		public override void GetValue() =>
			Value = GetStat(StatDefOf.ComfyTemperatureMin, true);
		public override void SetValue() =>
			SetStat(StatDefOf.ComfyTemperatureMin, UseMinTempLimits, MinimumMinTemp, MaximumMinTemp);

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "MinTemperature", DefaultValue);
			Value = value;
		}

		public override bool IsGlobalUsed() =>
			UseMinTempLimits;
		#endregion
	}
}
