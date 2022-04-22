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
	internal class SettingHealthScale : BaseSetting<float>
	{
		#region PROPERTIES
		public static bool UseGlobal { get; set; }
		public static float Global { get; set; }

		public const float GlobalDefault = 1f;
		public const float Minimum = 1e-3f;
		public const float Maximum = 1e3f;
		#endregion

		#region CONSTRUCTORS
		public SettingHealthScale(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override void GetValue()
		{
			var race = Animal?.race;
			if (race != null)
				Value = race.baseHealthScale;
			else if (!IsGlobal)
				Log.Warning($"{nameof(CustomizeAnimals)}.{nameof(SettingHealthScale)}: {Animal?.defName} race is null, value cannot be set!");
		}
		public override void SetValue()
		{
			var race = Animal?.race;
			if (race != null)
				race.baseHealthScale = Value * (UseGlobal ? Global : 1f);
		}

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "HealthScale", DefaultValue);
			Value = value;
		}

		public override void ResetGlobal()
		{
			UseGlobal = false;
			Global = GlobalDefault;
		}

		public override void ExposeGlobal()
		{
			var useGlobal = UseGlobal;
			Scribe_Values.Look(ref useGlobal, "UseHealthScaleModifier");
			UseGlobal = useGlobal;

			var value = Global;
			Scribe_Values.Look(ref value, "HealthScaleModifier", GlobalDefault);
			Global = value;
		}

		public override bool IsGlobalUsed() =>
			UseGlobal;
		#endregion
	}
}
