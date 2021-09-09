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
	internal class SettingArmorRating_Sharp : SettingArmorRating
	{
		#region PROPERTIES
		#endregion

		#region CONSTRUCTORS
		public SettingArmorRating_Sharp(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override float? GetValue() =>
			GetStat(StatDefOf.ArmorRating_Sharp, true);
		public override void SetValue() =>
			SetStat(StatDefOf.ArmorRating_Sharp);

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "ArmorRating_Sharp", DefaultValue);
			Value = value;
		}
		#endregion
	}

	internal class SettingArmorRating_Blunt : SettingArmorRating
	{
		#region PROPERTIES
		#endregion

		#region CONSTRUCTORS
		public SettingArmorRating_Blunt(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override float? GetValue() =>
			GetStat(StatDefOf.ArmorRating_Blunt, true);
		public override void SetValue() =>
			SetStat(StatDefOf.ArmorRating_Blunt);

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "ArmorRating_Blunt", DefaultValue);
			Value = value;
		}
		#endregion
	}

	internal class SettingArmorRating_Heat : SettingArmorRating
	{
		#region PROPERTIES
		#endregion

		#region CONSTRUCTORS
		public SettingArmorRating_Heat(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override float? GetValue() =>
			GetStat(StatDefOf.ArmorRating_Heat, true);
		public override void SetValue() =>
			SetStat(StatDefOf.ArmorRating_Heat);

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "ArmorRating_Heat", DefaultValue);
			Value = value;
		}
		#endregion
	}


	internal abstract class SettingArmorRating : NullableFloatSetting
	{
		#region PROPERTIES
		public const float DefaultMinimum = -1e3f;
		public const float DefaultMaximum = 1e3f;
		#endregion

		#region CONSTRUCTORS
		public SettingArmorRating(ThingDef animal, bool isGlobal) : base(animal, isGlobal)
		{ }
		#endregion
	}
}
