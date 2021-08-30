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
	internal class SettingWildness : BaseSetting<float?>
	{
		#region PROPERTIES
		public static bool UseWildnessLimits { get; set; }
		public static float MinimumWildness { get; set; }
		public static float MaximumWildness { get; set; }

		public const float DefaultMinimum = 0f;
		public const float DefaultMaximum = 10f;
		#endregion

		#region CONSTRUCTORS
		public SettingWildness(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		public override void ResetGlobal()
		{
			UseWildnessLimits = false;
			MinimumWildness = DefaultMinimum;
			MaximumWildness = DefaultMaximum;
		}

		public override void ExposeGlobal()
		{
			var useGlobal = UseWildnessLimits;
			Scribe_Values.Look(ref useGlobal, nameof(UseWildnessLimits));
			UseWildnessLimits = useGlobal;

			var minValue = MinimumWildness;
			Scribe_Values.Look(ref minValue, nameof(MinimumWildness), DefaultMinimum);
			MinimumWildness = minValue;
			var maxValue = MaximumWildness;
			Scribe_Values.Look(ref maxValue, nameof(MaximumWildness), DefaultMaximum);
			MaximumWildness = maxValue;
		}
		#endregion

		#region INTERFACES
		public override float? GetValue()
		{
			var race = Animal?.race;
			if (race != null)
				return race.wildness;

			if (!IsGlobal)
				Log.Warning($"{nameof(CustomizeAnimals)}.{nameof(SettingWildness)}: {Animal?.defName} race is null, value cannot be set!");
			return null;
		}
		public override void SetValue()
		{
			var race = Animal?.race;
			if (race != null)
			{
				if (Value is float value)
				{
					if (UseWildnessLimits)
						value = Mathf.Clamp(value, MinimumWildness, MaximumWildness);
					race.wildness = value;
				}
				else
					Log.Warning($"{Animal?.defName}: wildness value is null");
			}
		}

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "Wildness", DefaultValue);
			Value = value;
		}

		public override bool IsGlobalUsed() =>
			UseWildnessLimits;
		#endregion
	}
}
