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
		public CompProperties_Milkable DefaultMilkable { get; private set; }
		public CompProperties_Milkable Milkable { get; private set; }

		public bool DefaultIsMilkable { get; private set; }
		public bool IsMilkable { get; set; }

		public ThingDef DefaultMilkDef { get; private set; }
		public ThingDef MilkDef { get; set; }
		public int DefaultIntervalDays { get; private set; }
		public int IntervalDays { get; set; }
		public int DefaultAmount { get; private set; }
		public int Amount { get; set; }
		public bool DefaultFemaleOnly { get; private set; }
		public bool FemaleOnly { get; set; }

		public static ThingDef StandardMilkDef { get; private set; } = null;
		#endregion

		#region CONSTRUCTORS
		public SpecialSettingMilkable(ThingDef animal) : base(animal)
		{
			GetValue();

			DefaultMilkable = Milkable;
			DefaultIsMilkable = IsMilkable;

			DefaultMilkDef = MilkDef;
			DefaultIntervalDays = IntervalDays;
			DefaultAmount = Amount;
			DefaultFemaleOnly = FemaleOnly;
		}
		#endregion

		#region OVERRIDES
		public override void GetValue()
		{
			Milkable = Animal.comps?.Find((comp) => comp is CompProperties_Milkable) as CompProperties_Milkable;
			IsMilkable = Milkable != null;

			if (StandardMilkDef == null)
				StandardMilkDef = DefDatabase<ThingDef>.GetNamed("Milk");

			if (IsMilkable)
			{
				MilkDef = Milkable.milkDef;
				IntervalDays = Milkable.milkIntervalDays;
				Amount = Milkable.milkAmount;
				FemaleOnly = Milkable.milkFemaleOnly;
			}
			else
			{
				MilkDef = StandardMilkDef ?? throw new Exception($"{nameof(CustomizeAnimals)}.{nameof(SpecialSettingMilkable)}: Standard milk should not be null");
				IntervalDays = 2;
				Amount = 10;
				FemaleOnly = true;
			}
		}
		public override void SetValue()
		{
			if (IsMilkable)
			{
				if (Milkable == null)
				{
					Milkable = DefaultMilkable ?? new CompProperties_Milkable();

					var comps = Animal.comps;
					if (comps == null)
						Animal.comps = new List<CompProperties>();
					if (!comps.Contains(Milkable))
						comps.Add(Milkable);
				}

				Milkable.milkDef = MilkDef ?? DefaultMilkDef;
				Milkable.milkIntervalDays = IntervalDays;
				Milkable.milkAmount = Amount;
				Milkable.milkFemaleOnly = FemaleOnly;
			}
			else
			{
				if (Milkable != null)
				{
					Animal.comps?.Remove(Milkable);
					Milkable = null;
				}
			}
		}

		public override void Reset()
		{
			Milkable = DefaultMilkable;
			IsMilkable = DefaultIsMilkable;

			MilkDef = DefaultMilkDef;
			IntervalDays = DefaultIntervalDays;
			Amount = DefaultAmount;
			FemaleOnly = DefaultFemaleOnly;
		}
		public override bool IsModified()
		{
			if (Milkable != DefaultMilkable 
				|| IsMilkable != DefaultIsMilkable 
				|| IsMilkable && (MilkDef != DefaultMilkDef || IntervalDays != DefaultIntervalDays || Amount != DefaultAmount || FemaleOnly != DefaultFemaleOnly))
				return true;
			return false;
		}

		public override void ExposeData()
		{
			if (Scribe.mode != LoadSaveMode.Saving || IsMilkable || IsMilkable != DefaultIsMilkable)
			{
				if (Scribe.EnterNode("Milkable"))
				{
					bool boolValue = IsMilkable;
					Scribe_Values.Look(ref boolValue, "IsMilkable", DefaultIsMilkable);
					IsMilkable = boolValue;

					if (IsMilkable)
					{
						if (Scribe.mode != LoadSaveMode.Saving || MilkDef != DefaultMilkDef)
						{
							ThingDef defValue = MilkDef;
							Scribe_Defs.Look(ref defValue, "MilkDef");
							MilkDef = defValue ?? DefaultMilkDef ?? StandardMilkDef;
						}

						int intValue = IntervalDays;
						Scribe_Values.Look(ref intValue, "IntervalDays", DefaultIntervalDays);
						IntervalDays = intValue;

						intValue = Amount;
						Scribe_Values.Look(ref intValue, "Amount", DefaultAmount);
						Amount = intValue;

						boolValue = FemaleOnly;
						Scribe_Values.Look(ref boolValue, "FemaleOnly", DefaultFemaleOnly);
						FemaleOnly = boolValue;
					}

					Scribe.ExitNode();
				}
			}
		}
		#endregion
	}
}
