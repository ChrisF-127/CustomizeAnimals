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
	internal class SettingMaxTemperature : BaseSetting<float?>
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
		public override void ResetGlobal()
		{
			UseMaxTempLimits = false;
			MinimumMaxTemp = TemperatureTuning.MinimumTemperature;
			MaximumMaxTemp = TemperatureTuning.MaximumTemperature;
		}

		public override void ExposeGlobal()
		{
			var useGlobal = UseMaxTempLimits;
			Scribe_Values.Look(ref useGlobal, nameof(UseMaxTempLimits));
			UseMaxTempLimits = useGlobal;

			var minMaxTemp = MinimumMaxTemp;
			Scribe_Values.Look(ref minMaxTemp, nameof(MinimumMaxTemp), TemperatureTuning.MinimumTemperature);
			MinimumMaxTemp = minMaxTemp;
			var maxMaxTemp = MaximumMaxTemp;
			Scribe_Values.Look(ref maxMaxTemp, nameof(MaximumMaxTemp), TemperatureTuning.MaximumTemperature);
			MaximumMaxTemp = maxMaxTemp;
		}
		#endregion

		#region INTERFACES
		public override float? GetValue()
		{
			var statBases = Animal?.statBases;
			if (statBases != null)
				return statBases.FirstOrDefault((s) => s.stat == StatDefOf.ComfyTemperatureMax)?.value ?? StatDefOf.ComfyTemperatureMax.defaultBaseValue;

			if (!IsGlobal)
				Log.Warning($"{nameof(CustomizeAnimals)}.{nameof(SettingMaxTemperature)}: {Animal?.defName} statBases is null, value cannot be set!");
			return null;
		}
		public override void SetValue()
		{
			var statBases = Animal?.statBases;
			if (statBases != null)
			{
				var stat = statBases.FirstOrDefault((s) => s.stat == StatDefOf.ComfyTemperatureMax);
				if (Value != null)
				{
					var value = (float)Value;
					if (UseMaxTempLimits)
						value = Mathf.Clamp(value, MinimumMaxTemp, MaximumMaxTemp);

					if (stat != null)
						stat.value = value;
					else
						statBases.Add(new StatModifier { stat = StatDefOf.ComfyTemperatureMax, value = value });
				}
				else if (stat != null)
					statBases.Remove(stat);
			}
		}

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "MaxTemperature", DefaultValue);
			Value = value;
		}

		public override bool IsGlobalUsed() =>
			UseMaxTempLimits;
		#endregion
	}


	internal class SettingMinTemperature : BaseSetting<float?>
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
			Scribe_Values.Look(ref useGlobal, nameof(UseMinTempLimits));
			UseMinTempLimits = useGlobal;

			var minMinTemp = MinimumMinTemp;
			Scribe_Values.Look(ref minMinTemp, nameof(MinimumMinTemp), TemperatureTuning.MinimumTemperature);
			MinimumMinTemp = minMinTemp;
			var maxMinTemp = MaximumMinTemp;
			Scribe_Values.Look(ref maxMinTemp, nameof(MaximumMinTemp), TemperatureTuning.MaximumTemperature);
			MaximumMinTemp = maxMinTemp;
		}
		#endregion

		#region INTERFACES
		public override float? GetValue()
		{
			var statBases = Animal?.statBases;
			if (statBases != null)
				return statBases.FirstOrDefault((s) => s.stat == StatDefOf.ComfyTemperatureMin)?.value ?? StatDefOf.ComfyTemperatureMin.defaultBaseValue;

			if (!IsGlobal)
				Log.Warning($"{nameof(CustomizeAnimals)}.{nameof(SettingMinTemperature)}: {Animal?.defName} statBases is null, value cannot be set!");
			return null;
		}
		public override void SetValue()
		{
			var statBases = Animal?.statBases;
			if (statBases != null)
			{
				var stat = statBases.FirstOrDefault((s) => s.stat == StatDefOf.ComfyTemperatureMin);
				if (Value != null)
				{
					var value = (float)Value;
					if (UseMinTempLimits)
						value = Mathf.Clamp(value, MinimumMinTemp, MaximumMinTemp);

					if (stat != null)
						stat.value = value;
					else
						statBases.Add(new StatModifier { stat = StatDefOf.ComfyTemperatureMin, value = value });
				}
				else if (stat != null)
					statBases.Remove(stat);
			}
		}

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
