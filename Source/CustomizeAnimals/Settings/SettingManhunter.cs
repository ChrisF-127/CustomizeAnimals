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
	internal class SettingManhunterOnTameFail : SettingManhunter
	{
		#region PROPERTIES
		public static bool UseLimits { get; set; }
		public static float Minimum { get; set; }
		public static float Maximum { get; set; }
		#endregion

		#region CONSTRUCTORS
		public SettingManhunterOnTameFail(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override float GetValue()
		{
			var race = Animal?.race;
			if (race != null)
				return race.manhunterOnTameFailChance;

			if (!IsGlobal)
				Log.Warning($"{nameof(CustomizeAnimals)}.{nameof(SettingManhunterOnDamage)}: {Animal?.defName} race is null, value cannot be set!");
			return 0f;
		}
		public override void SetValue()
		{
			var race = Animal?.race;
			if (race != null)
				race.manhunterOnTameFailChance = UseLimits ? Mathf.Clamp(Value, Minimum, Maximum) : Value;
		}

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "ManhunterOnTameFail", DefaultValue);
			Value = value;
		}

		public override void ResetGlobal()
		{
			UseLimits = false;
			Minimum = DefaultMinimum;
			Maximum = DefaultMaximum;
		}

		public override void ExposeGlobal()
		{
			var useGlobal = UseLimits;
			Scribe_Values.Look(ref useGlobal, nameof(UseLimits));
			UseLimits = useGlobal;

			var minMaxTemp = Minimum;
			Scribe_Values.Look(ref minMaxTemp, nameof(Minimum), DefaultMinimum);
			Minimum = minMaxTemp;
			var maxMaxTemp = Maximum;
			Scribe_Values.Look(ref maxMaxTemp, nameof(Maximum), DefaultMaximum);
			Maximum = maxMaxTemp;
		}

		public override bool IsGlobalUsed() =>
			UseLimits;
		#endregion
	}
	internal class SettingManhunterOnDamage : SettingManhunter
	{
		#region PROPERTIES
		public static bool UseLimits { get; set; }
		public static float Minimum { get; set; }
		public static float Maximum { get; set; }
		#endregion

		#region CONSTRUCTORS
		public SettingManhunterOnDamage(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override float GetValue()
		{
			var race = Animal?.race;
			if (race != null)
				return race.manhunterOnDamageChance;

			if (!IsGlobal)
				Log.Warning($"{nameof(CustomizeAnimals)}.{nameof(SettingManhunterOnDamage)}: {Animal?.defName} race is null, value cannot be set!");
			return 0f;
		}
		public override void SetValue()
		{
			var race = Animal?.race;
			if (race != null)
				race.manhunterOnDamageChance = UseLimits ? Mathf.Clamp(Value, Minimum, Maximum) : Value;
		}

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "ManhunterOnDamage", DefaultValue);
			Value = value;
		}

		public override void ResetGlobal()
		{
			UseLimits = false;
			Minimum = DefaultMinimum;
			Maximum = DefaultMaximum;
		}

		public override void ExposeGlobal()
		{
			var useGlobal = UseLimits;
			Scribe_Values.Look(ref useGlobal, nameof(UseLimits));
			UseLimits = useGlobal;

			var minMaxTemp = Minimum;
			Scribe_Values.Look(ref minMaxTemp, nameof(Minimum), DefaultMinimum);
			Minimum = minMaxTemp;
			var maxMaxTemp = Maximum;
			Scribe_Values.Look(ref maxMaxTemp, nameof(Maximum), DefaultMaximum);
			Maximum = maxMaxTemp;
		}

		public override bool IsGlobalUsed() =>
			UseLimits;
		#endregion
	}



	internal abstract class SettingManhunter : BaseSetting<float>
	{
		#region PROPERTIES
		public const float DefaultMinimum = 0f;
		public const float DefaultMaximum = 1f;
		#endregion

		#region CONSTRUCTORS
		public SettingManhunter(ThingDef animal, bool isGlobal) : base(animal, isGlobal)
		{ }
		#endregion
	}
}
