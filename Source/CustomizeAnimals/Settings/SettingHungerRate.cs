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
	internal class SettingHungerRate : NullableFloatSetting
	{
		#region PROPERTIES
		public static bool UseHungerRateLimits { get; set; }
		public static float MinimumHungerRate { get; set; }
		public static float MaximumHungerRate { get; set; }

		public const float DefaultMinimum = 0f;
		public const float DefaultMaximum = 25f;
		#endregion

		#region CONSTRUCTORS
		public SettingHungerRate(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override float? GetValue()
		{
			var race = Animal?.race;
			if (race != null)
				return race.baseHungerRate;

			if (!IsGlobal)
				Log.Warning($"{nameof(CustomizeAnimals)}.{nameof(SettingHungerRate)}: {Animal?.defName} race is null, value cannot be set!");
			return null;
		}
		public override void SetValue()
		{
			var race = Animal?.race;
			if (race != null)
			{
				if (Value is float value)
				{
					if (UseHungerRateLimits)
						value = Mathf.Clamp(value, MinimumHungerRate, MaximumHungerRate);
					race.baseHungerRate = value;
				}
				else
					Log.Warning($"{Animal?.defName}: hunger rate value is null");
			}
		}

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "HungerRate", DefaultValue);
			Value = value;
		}

		public override void ResetGlobal()
		{
			UseHungerRateLimits = false;
			MinimumHungerRate = DefaultMinimum;
			MaximumHungerRate = DefaultMaximum;
		}

		public override void ExposeGlobal()
		{
			var useGlobal = UseHungerRateLimits;
			Scribe_Values.Look(ref useGlobal, nameof(UseHungerRateLimits));
			UseHungerRateLimits = useGlobal;

			var minValue = MinimumHungerRate;
			Scribe_Values.Look(ref minValue, nameof(MinimumHungerRate), DefaultMinimum);
			MinimumHungerRate = minValue;
			var maxValue = MaximumHungerRate;
			Scribe_Values.Look(ref maxValue, nameof(MaximumHungerRate), DefaultMaximum);
			MaximumHungerRate = maxValue;
		}

		public override bool IsGlobalUsed() =>
			UseHungerRateLimits;
		#endregion
	}
}
