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
	internal class SettingNuzzleMtbHours : BaseSetting<float>
	{
		#region PROPERTIES
		public static bool UseNuzzleMtbHoursLimits { get; set; }
		public static float MinimumNuzzleMtbHours { get; set; }
		public static float MaximumNuzzleMtbHours { get; set; }

		public const float DefaultMinimum = -1f; // -1 = disabled
		public const float DefaultMaximum = 1440f; // 60d * 24h = 1 year

		public const float DefaultMinimumGlobal = 1f; // 1h
		public const float DefaultMaximumGlobal = -1f; // -1 = disabled
		#endregion

		#region CONSTRUCTORS
		public SettingNuzzleMtbHours(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override void GetValue()
		{
			var race = Animal?.race;
			if (race != null)
				Value = race.nuzzleMtbHours;
			else if (!IsGlobal)
				Log.Warning($"{nameof(CustomizeAnimals)}.{nameof(SettingNuzzleMtbHours)}: {Animal?.defName} race is null, value cannot be set!");
		}
		public override void SetValue()
		{
			var race = Animal?.race;
			if (race != null)
			{
				var value = Value;
				if (UseNuzzleMtbHoursLimits)
				{
					// If Minimum is -1, disable nuzzling for all animals
					if (MinimumNuzzleMtbHours < 0f)
						value = -1f;
					else
					{
						// If Maximum is higher than 0, enable nuzzling for all animals
						if (MaximumNuzzleMtbHours >= 0f && (value < 0f || value > MaximumNuzzleMtbHours))
							value = MaximumNuzzleMtbHours;
						// Enforce lower limit
						if (value >= 0f && value < MinimumNuzzleMtbHours)
							value = MinimumNuzzleMtbHours;
					}
				}
				race.nuzzleMtbHours = value;
			}
		}

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "NuzzleMtbHours", DefaultValue);
			Value = value;
		}

		public override void ResetGlobal()
		{
			UseNuzzleMtbHoursLimits = false;
			MinimumNuzzleMtbHours = DefaultMinimumGlobal;
			MaximumNuzzleMtbHours = DefaultMaximumGlobal;
		}

		public override void ExposeGlobal()
		{
			var useGlobal = UseNuzzleMtbHoursLimits;
			Scribe_Values.Look(ref useGlobal, "UseNuzzleMtbHoursLimits");
			UseNuzzleMtbHoursLimits = useGlobal;

			var minValue = MinimumNuzzleMtbHours;
			Scribe_Values.Look(ref minValue, "MinimumNuzzleMtbHours", DefaultMinimumGlobal);
			MinimumNuzzleMtbHours = minValue;

			var maxValue = MaximumNuzzleMtbHours;
			Scribe_Values.Look(ref maxValue, "MaximumNuzzleMtbHours", DefaultMaximumGlobal);
			MaximumNuzzleMtbHours = maxValue;
		}

		public override bool IsGlobalUsed() =>
			UseNuzzleMtbHoursLimits;
		#endregion
	}
}
