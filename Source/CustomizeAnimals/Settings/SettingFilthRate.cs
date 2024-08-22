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
	internal class SettingFilthRate : NullableFloatSetting
	{
		#region PROPERTIES
		public static bool UseMaximumFilthRate { get; set; } = false;
		public static float MaximumFilthRate { get; set; } = DefaultMaximum;

		public const float DefaultMaximum = 120f;
		#endregion

		#region CONSTRUCTORS
		public SettingFilthRate(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{
			if (DefaultValue == null)
				DefaultValue = StatDefOf.FilthRate.defaultBaseValue;
		}
		#endregion

		#region INTERFACES
		public override void GetValue() =>
			Value = GetStat(StatDefOf.FilthRate, false);
		public override void SetValue() =>
			SetStat(StatDefOf.FilthRate, Value ?? StatDefOf.FilthRate.defaultBaseValue, Animal.IsAnimal() && UseMaximumFilthRate, 0f, MaximumFilthRate);

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "FilthRate", DefaultValue);
			Value = value;
		}

		public override void ResetGlobal()
		{
			UseMaximumFilthRate = false;
			MaximumFilthRate = DefaultMaximum;
		}

		public override void ExposeGlobal()
		{
			var useGlobal = UseMaximumFilthRate;
			Scribe_Values.Look(ref useGlobal, "UseMaximumFilthRate");
			UseMaximumFilthRate = useGlobal;

			var maxValue = MaximumFilthRate;
			Scribe_Values.Look(ref maxValue, "MaximumFilthRate", DefaultMaximum);
			MaximumFilthRate = maxValue;
		}

		public override bool IsModified() =>
			!(DefaultValue?.Equals(Value ?? StatDefOf.FilthRate.defaultBaseValue) == true);

		public override bool IsGlobalUsed() =>
			UseMaximumFilthRate;
		#endregion
	}
}
