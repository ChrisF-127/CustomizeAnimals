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
	internal class SpecialSettingShearable : BaseSpecialSetting
	{
		#region PROPERTIES
		public CompProperties_Shearable DefaultShearable { get; private set; }
		public CompProperties_Shearable Shearable { get; private set; }

		public bool DefaultIsShearable { get; private set; }
		public bool IsShearable { get; set; }

		public ThingDef DefaultWoolDef { get; private set; }
		public ThingDef WoolDef { get; set; }
		public int DefaultIntervalDays { get; private set; }
		public int IntervalDays { get; set; }
		public int DefaultAmount { get; private set; }
		public int Amount { get; set; }

		public static ThingDef StandardWoolDef { get; private set; } = null;
		#endregion

		#region CONSTRUCTORS
		public SpecialSettingShearable(ThingDef animal) : base(animal)
		{
			GetValue();

			DefaultShearable = Shearable;
			DefaultIsShearable = IsShearable;

			DefaultWoolDef = WoolDef;
			DefaultIntervalDays = IntervalDays;
			DefaultAmount = Amount;
		}
		#endregion

		#region OVERRIDES
		public override void GetValue()
		{
			Shearable = Animal.comps?.Find((comp) => comp is CompProperties_Shearable) as CompProperties_Shearable;
			IsShearable = Shearable != null;

			if (StandardWoolDef == null)
				StandardWoolDef = DefDatabase<ThingDef>.GetNamedSilentFail("WoolSheep") ?? DefDatabase<ThingDef>.GetNamed("Cloth");

			if (IsShearable)
			{
				WoolDef = Shearable.woolDef;
				IntervalDays = Shearable.shearIntervalDays;
				Amount = Shearable.woolAmount;
			}
			else
			{
				WoolDef = StandardWoolDef ?? throw new Exception($"{nameof(CustomizeAnimals)}.{nameof(SpecialSettingShearable)}: Standard wool should not be null");
				IntervalDays = 10;
				Amount = 45;
			}
		}
		public override void SetValue()
		{
			if (IsShearable)
			{
				if (Shearable == null)
				{
					Shearable = DefaultShearable ?? new CompProperties_Shearable();

					var comps = Animal.comps;
					if (comps == null)
						Animal.comps = new List<CompProperties>();
					if (!comps.Contains(Shearable))
						comps.Add(Shearable);
				}

				Shearable.woolDef = WoolDef ?? DefaultWoolDef;
				Shearable.shearIntervalDays = IntervalDays;
				Shearable.woolAmount = Amount;
			}
			else
			{
				if (Shearable != null)
				{
					Animal.comps?.Remove(Shearable);
					Shearable = null;
				}
			}
		}

		public override void Reset()
		{
			Shearable = DefaultShearable;
			IsShearable = DefaultIsShearable;

			WoolDef = DefaultWoolDef;
			IntervalDays = DefaultIntervalDays;
			Amount = DefaultAmount;
		}
		public override bool IsModified()
		{
			if (Shearable != DefaultShearable 
				|| IsShearable != DefaultIsShearable 
				|| IsShearable && (WoolDef != DefaultWoolDef || IntervalDays != DefaultIntervalDays || Amount != DefaultAmount))
				return true;
			return false;
		}

		public override void ExposeData()
		{
			if (Scribe.mode != LoadSaveMode.Saving || IsShearable || IsShearable != DefaultIsShearable)
			{
				if (Scribe.EnterNode("Shearable"))
				{
					bool boolValue = IsShearable;
					Scribe_Values.Look(ref boolValue, "IsShearable", DefaultIsShearable);
					IsShearable = boolValue;

					if (IsShearable)
					{
						if (Scribe.mode != LoadSaveMode.Saving || WoolDef != DefaultWoolDef)
						{
							ThingDef defValue = WoolDef;
							Scribe_Defs.Look(ref defValue, "WoolDef");
							WoolDef = defValue ?? DefaultWoolDef ?? StandardWoolDef;
						}

						int intValue = IntervalDays;
						Scribe_Values.Look(ref intValue, "IntervalDays", DefaultIntervalDays);
						IntervalDays = intValue;

						intValue = Amount;
						Scribe_Values.Look(ref intValue, "Amount", DefaultAmount);
						Amount = intValue;
					}

					Scribe.ExitNode();
				}
			}
		}
		#endregion
	}
}
