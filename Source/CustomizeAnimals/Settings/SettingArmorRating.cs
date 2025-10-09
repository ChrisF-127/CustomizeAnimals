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
		protected override string ScribeLabel =>
			"ArmorRating_Sharp";
		#endregion

		#region CONSTRUCTORS
		public SettingArmorRating_Sharp(ThingDef animal, bool isGlobal = false) : 
			base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override void GetValue() =>
			Value = GetStat(StatDefOf.ArmorRating_Sharp, true);
		public override void SetValue() =>
			SetStat(StatDefOf.ArmorRating_Sharp);
		#endregion
	}

	internal class SettingArmorRating_Blunt : SettingArmorRating
	{
		#region PROPERTIES
		protected override string ScribeLabel =>
			"ArmorRating_Blunt";
		#endregion

		#region CONSTRUCTORS
		public SettingArmorRating_Blunt(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override void GetValue() =>
			Value = GetStat(StatDefOf.ArmorRating_Blunt, true);
		public override void SetValue() =>
			SetStat(StatDefOf.ArmorRating_Blunt);
		#endregion
	}

	internal class SettingArmorRating_Heat : SettingArmorRating
	{
		#region PROPERTIES
		protected override string ScribeLabel =>
			"ArmorRating_Heat";
		#endregion

		#region CONSTRUCTORS
		public SettingArmorRating_Heat(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region PUBLIC METHODS
		#endregion

		#region INTERFACES
		public override void GetValue() =>
			Value = GetStat(StatDefOf.ArmorRating_Heat, true);
		public override void SetValue() =>
			SetStat(StatDefOf.ArmorRating_Heat);
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
