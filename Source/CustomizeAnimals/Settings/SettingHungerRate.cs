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
		public static bool UseHungerRateLimits { get; set; } = false;
		public static float MinimumHungerRate { get; set; } = DefaultMinimumGlobal;
		public static float MaximumHungerRate { get; set; } = DefaultMaximumGlobal;

		public const float DefaultMinimumGlobal = 0f;
		public const float DefaultMaximumGlobal = 25f;
		#endregion

		#region CONSTRUCTORS
		public SettingHungerRate(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region INTERFACES
		public override void GetValue()
		{
			var race = Animal?.race;
			if (race != null)
				Value = race.baseHungerRate;
			else if (!IsGlobal)
				Log.Warning($"{nameof(CustomizeAnimals)}.{nameof(SettingHungerRate)}: {Animal?.defName} race is null, value cannot be set!");
		}
		public override void SetValue()
		{
			var race = Animal?.race;
			if (race != null)
			{
				if (Value is float value)
				{
					if (Animal.IsAnimal() && UseHungerRateLimits)
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
			MinimumHungerRate = DefaultMinimumGlobal;
			MaximumHungerRate = DefaultMaximumGlobal;
		}

		public override void ExposeGlobal()
		{
			var useGlobal = UseHungerRateLimits;
			Scribe_Values.Look(ref useGlobal, "UseHungerRateLimits");
			UseHungerRateLimits = useGlobal;

			var minValue = MinimumHungerRate;
			Scribe_Values.Look(ref minValue, "MinimumHungerRate", DefaultMinimumGlobal);
			MinimumHungerRate = minValue;
			var maxValue = MaximumHungerRate;
			Scribe_Values.Look(ref maxValue, "MaximumHungerRate", DefaultMaximumGlobal);
			MaximumHungerRate = maxValue;
		}

		public override bool IsGlobalUsed() =>
			UseHungerRateLimits;
		#endregion
	}
}
