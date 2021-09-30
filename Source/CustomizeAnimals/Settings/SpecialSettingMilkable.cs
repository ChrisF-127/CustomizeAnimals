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
	internal class SpecialSettingMilkable : BaseSpecialSetting
	{
		#region PROPERTIES
		public CompProperties_Milkable Milkable { get; private set; }
		public bool IsMilkable => Milkable != null;

		public ThingDef DefaultDef { get; private set; }
		public ThingDef Def { get; set; }
		public int DefaultIntervalDays { get; private set; }
		public int IntervalDays { get; set; }
		public int DefaultAmount { get; private set; }
		public int Amount { get; set; }
		public bool DefaultFemaleOnly { get; private set; }
		public bool FemaleOnly { get; set; }

		public ThingDef MilkDef => Milkable?.milkDef;

		public static ThingDef StandardMilkDef { get; }
		#endregion

		#region CONSTRUCTORS
		public SpecialSettingMilkable(ThingDef animal) : base(animal)
		{
			GetValue();

			DefaultDef = Def;
			DefaultIntervalDays = IntervalDays;
			DefaultAmount = Amount;
			DefaultFemaleOnly = FemaleOnly;
		}
		#endregion

		#region OVERRIDES
		public override void GetValue()
		{
			Milkable = Animal?.comps?.Find((comp) => comp is CompProperties_Milkable) as CompProperties_Milkable;
			if (!IsMilkable)
				return;

			Def = Milkable.milkDef;
			IntervalDays = Milkable.milkIntervalDays;
			Amount = Milkable.milkAmount;
			FemaleOnly = Milkable.milkFemaleOnly;
		}
		public override void SetValue()
		{
			if (!IsMilkable)
				return;

			Milkable.milkDef = Def;
			Milkable.milkIntervalDays = IntervalDays;
			Milkable.milkAmount = Amount;
			Milkable.milkFemaleOnly = FemaleOnly;
		}

		public override void Reset()
		{
			Def = DefaultDef;
			IntervalDays = DefaultIntervalDays;
			Amount = DefaultAmount;
			FemaleOnly = DefaultFemaleOnly;
		}
		public override bool IsModified()
		{
			if (Def != DefaultDef
				|| IntervalDays != DefaultIntervalDays
				|| Amount != DefaultAmount
				|| FemaleOnly != DefaultFemaleOnly)
				return true;
			return false;
		}

		public override void ExposeData()
		{
			bool boolValue = FemaleOnly;
			Scribe_Values.Look(ref boolValue, nameof(FemaleOnly), DefaultFemaleOnly);
			FemaleOnly = boolValue;

			int intValue = IntervalDays;
			Scribe_Values.Look(ref intValue, nameof(IntervalDays), DefaultIntervalDays);
			IntervalDays = intValue;

			intValue = Amount;
			Scribe_Values.Look(ref intValue, nameof(Amount), DefaultAmount);
			Amount = intValue;
		}
		#endregion
	}
}
