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

		public List<ThingDef> AuxiliaryList { get; } = new List<ThingDef>();
		#endregion

		#region CONSTRUCTORS
		public SettingWillNeverEat(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
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

		public void Value2Aux()
		{
			if (Value?.Count > 0)
				foreach (var def in Value)
					if (!AuxiliaryList.Contains(def))
						AuxiliaryList.Add(def);
		}
		public void Aux2Value()
		{
			if (AuxiliaryList.Count > 0)
			{
				if (Value == null)
					Value = new List<ThingDef>();
				foreach (var def in AuxiliaryList)
					if (!Value.Contains(def))
						Value.Add(def);
			}
			else
				Value = null;
		}
		#endregion

		#region INTERFACES
		public override void GetValue()
		{
			var race = Animal?.race;
			if (race != null)
				Value = race.willNeverEat;
			else if (!IsGlobal)
				Log.Warning($"{nameof(CustomizeAnimals)}.{nameof(SettingWillNeverEat)}: {Animal?.defName} race is null, value cannot be set!");

			Value2Aux();
		}
		public override void SetValue()
		{
			var race = Animal?.race;
			if (race != null)
			{
				var value = Value?.Count > 0 ? Value : null;
				if (UseGlobalList && GlobalList.Count > 0)
				{
					if (value == null)
						value = new List<ThingDef>();
					foreach (var def in GlobalList)
						if (!value.Contains(def))
							value.Add(def);
				}
				race.willNeverEat = value;
			}
		}

		public override void Reset()
		{
			if (DefaultValue == null)
				Value = null;
			else
			{
				if (Value == null)
					Value = new List<ThingDef>(DefaultValue);
				else
				{
					Value.Clear();
					foreach (var def in DefaultValue)
						Value.Add(def);
				}
			}

			AuxiliaryList.Clear();
			Value2Aux();
		}
		public override bool IsModified() =>
			IsModified(Value, DefaultValue);

		public override void ExposeData()
		{
			if (Scribe.mode != LoadSaveMode.Saving || IsModified())
			{
				var value = Value;
				Scribe_Collections.Look(ref value, "WillNeverEat");
				Value = value;
			}

			Value2Aux();
		}
		#endregion

		#region PRIVATE METHODES
		#endregion
	}
}
