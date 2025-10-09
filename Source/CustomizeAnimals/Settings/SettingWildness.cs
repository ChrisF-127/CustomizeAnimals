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
	internal class SettingWildness : NullableFloatSetting
	{
		#region PROPERTIES
		public const float DefaultMinimum = 0f;
		public const float DefaultMaximum = 10f;

		protected override string ScribeLabel =>
			"Wildness";
		#endregion

		#region CONSTRUCTORS
		public SettingWildness(ThingDef animal, bool isGlobal = false) : 
			base(animal, isGlobal)
		{
			if (DefaultValue == null)
				DefaultValue = StatDefOf.Wildness.defaultBaseValue;
		}
		#endregion

		#region INTERFACES
		public override void GetValue() =>
			Value = GetStat(StatDefOf.Wildness, false);
		public override void SetValue() =>
			SetStat(StatDefOf.Wildness, Value ?? StatDefOf.Wildness.defaultBaseValue, Animal.IsAnimal(), DefaultMinimum, DefaultMaximum);
		public override bool IsModified() =>
			!(DefaultValue?.Equals(Value ?? StatDefOf.Wildness.defaultBaseValue) == true);
		#endregion
	}
}
