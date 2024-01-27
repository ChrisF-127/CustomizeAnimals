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
	internal class SettingAttackPowerModifier : BaseSetting<float>
	{
		#region PROPERTIES
		public static bool UseGlobal { get; set; } = false;
		public static float Global { get; set; } = GlobalDefault;

		public const float GlobalDefault = 1f;
		public const float Minimum = 1e-3f;
		public const float Maximum = 1e3f;
		#endregion

		#region FIELDS
		private float _previousValue = GlobalDefault;
		private float _previousGlobalValue = GlobalDefault;
		#endregion

		#region CONSTRUCTORS
		public SettingAttackPowerModifier(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region INTERFACES
		public override void GetValue() =>
			Value = 1f;
		public override void SetValue()
		{
			var tools = Animal?.tools;
			if (tools?.Count > 0)
			{
				var global = Animal.IsAnimal() && UseGlobal ? Global : 1f;

				foreach (var tool in tools)
					tool.power *= (Value / _previousValue) * (global / _previousGlobalValue);

				_previousValue = Value;
				_previousGlobalValue = global;
			}
		}

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "AttackPowerModifier", DefaultValue);
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
			Scribe_Values.Look(ref useGlobal, "UseGlobalAttackPowerModifier");
			UseGlobal = useGlobal;

			var value = Global;
			Scribe_Values.Look(ref value, "GlobalAttackPowerModifier", GlobalDefault);
			Global = value;
		}

		public override bool IsGlobalUsed() =>
			UseGlobal;
		#endregion
	}

	internal class SettingAttackCooldownModifier : BaseSetting<float>
	{
		#region PROPERTIES
		public static bool UseGlobal { get; set; } = false;
		public static float Global { get; set; } = GlobalDefault;

		public const float GlobalDefault = 1f;
		public const float Minimum = 1e-3f;
		public const float Maximum = 1e3f;
		#endregion

		#region FIELDS
		private float _previousValue = GlobalDefault;
		private float _previousGlobalValue = GlobalDefault;
		#endregion

		#region CONSTRUCTORS
		public SettingAttackCooldownModifier(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region INTERFACES
		public override void GetValue() => 
			Value = 1f;
		public override void SetValue()
		{
			var tools = Animal?.tools;
			if (tools?.Count > 0)
			{
				var global = Animal.IsAnimal() && UseGlobal ? Global : 1f;

				foreach (var tool in tools)
					tool.cooldownTime *= (Value / _previousValue) * (global / _previousGlobalValue);

				_previousValue = Value;
				_previousGlobalValue = global;
			}
		}

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "AttackCooldownModifier", DefaultValue);
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
			Scribe_Values.Look(ref useGlobal, "UseGlobalAttackCooldownModifier");
			UseGlobal = useGlobal;

			var value = Global;
			Scribe_Values.Look(ref value, "GlobalAttackCooldownModifier", GlobalDefault);
			Global = value;
		}

		public override bool IsGlobalUsed() =>
			UseGlobal;
		#endregion
	}
}
