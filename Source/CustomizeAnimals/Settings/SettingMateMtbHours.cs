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
	internal class SettingMateMtbHours : BaseSetting<float>
	{
		#region PROPERTIES
		public static bool UseGlobalModifier { get; set; } = false;
		public static float GlobalModifier { get; set; } = GlobalModifierDefault;

		public const float GlobalModifierDefault = 1f;
		public const float MinimumModifier = 1e-3f;
		public const float MaximumModifier = 1e3f;
		#endregion

		#region CONSTRUCTORS
		public SettingMateMtbHours(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override void GetValue()
		{
			var race = Animal?.race;
			if (race != null)
				Value = race.mateMtbHours;
			else if (!IsGlobal)
				Log.Warning($"{nameof(CustomizeAnimals)}.{nameof(SettingMateMtbHours)}: {Animal?.defName} race is null, value cannot be set!");
		}
		public override void SetValue()
		{
			var race = Animal?.race;
			if (race != null)
				race.mateMtbHours = Value * (UseGlobalModifier ? GlobalModifier : 1f);
		}

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "MateMtbHours", DefaultValue);
			Value = value;
		}

		public override void ResetGlobal()
		{
			UseGlobalModifier = false;
			GlobalModifier = GlobalModifierDefault;
		}

		public override void ExposeGlobal()
		{
			var useGlobal = UseGlobalModifier;
			Scribe_Values.Look(ref useGlobal, "UseMateMtbHoursModifier");
			UseGlobalModifier = useGlobal;

			var value = GlobalModifier;
			Scribe_Values.Look(ref value, "MateMtbHoursModifier", GlobalModifierDefault);
			GlobalModifier = value;
		}

		public override bool IsGlobalUsed() =>
			UseGlobalModifier;
		#endregion
	}
}
