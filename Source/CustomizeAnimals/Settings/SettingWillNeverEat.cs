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
	internal class SettingWillNeverEat : ThingDefListSetting
	{
		#region PROPERTIES
		public static bool UseGlobalList { get; set; } = false;
		public static List<ThingDef> GlobalList { get; set; } = new List<ThingDef>();

		protected override string ScribeLabel => 
			"WillNeverEat";
		#endregion

		#region CONSTRUCTORS
		public SettingWillNeverEat(ThingDef animal, bool isGlobal = false) : 
			base(animal, isGlobal)
		{ }
		#endregion

		#region OVERRIDES
		public override void GetValue()
		{
			if (!IsGlobal)
				Value = Animal.race?.willNeverEat ?? new List<ThingDef>();
		}
		public override void SetValue()
		{
			var race = Animal?.race;
			if (race != null)
			{
				var output = new List<ThingDef>();
				if (Value.Count > 0)
					foreach (var value in Value)
						output.Add(value);
				if (Animal.IsAnimal() && UseGlobalList && GlobalList.Count > 0)
					foreach (var global in GlobalList)
						if (!output.Contains(global))
							output.Add(global);
				race.willNeverEat = output;
			}
		}

		public override void ResetGlobal()
		{
			UseGlobalList = false;
			GlobalList.Clear();
		}
		public override void ExposeGlobal()
		{
			var useGlobal = UseGlobalList;
			Scribe_Values.Look(ref useGlobal, "UseWillNeverEatGlobalList");
			UseGlobalList = useGlobal;

			var globalList = GlobalList;
			Scribe_Collections.Look(ref globalList, "WillNeverEatGlobalList");
			GlobalList = globalList ?? new List<ThingDef>();
		}
		public override bool IsGlobalUsed() =>
			UseGlobalList;
		#endregion
	}
}
