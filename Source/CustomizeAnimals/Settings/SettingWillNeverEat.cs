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
	internal class SettingWillNeverEat : BaseSetting<List<ThingDef>>
	{
		#region PROPERTIES
		public static bool UseGlobalList { get; set; } = false;
		public static List<ThingDef> GlobalList { get; } = new List<ThingDef>();
		#endregion

		#region CONSTRUCTORS
		public SettingWillNeverEat(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{
			if (!isGlobal)
				DefaultValue = new List<ThingDef>(Value);
		}
		#endregion

		#region PUBLIC METHODS
		public static bool IsModified(List<ThingDef> one, List<ThingDef> two)
		{
			if (one?.Count != two?.Count)
				return true;
			if (one != null)
				foreach (var def in one)
					if (!two.Contains(def))
						return true;
			return false;
		}
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
				if (UseGlobalList && GlobalList.Count > 0)
					foreach (var global in GlobalList)
						if (!output.Contains(global))
							output.Add(global);

				race.willNeverEat = output.Count > 0 ? output : null;
			}
		}

		public override void Reset()
		{
			Value.Clear();
			if (DefaultValue.Count > 0)
				foreach (var def in DefaultValue)
					Value.Add(def);
		}
		public override bool IsModified() =>
			IsModified(Value, DefaultValue);

		public override void ExposeData()
		{
			if (Scribe.mode != LoadSaveMode.Saving || IsModified())
			{
				var value = Value;
				Scribe_Collections.Look(ref value, "WillNeverEat");
				Value = value ?? new List<ThingDef>();
			}
		}
		#endregion
	}
}
