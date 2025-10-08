using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CustomizeAnimals.Settings
{
	/// <summary>
	/// Animals on this list will join in on herd manhunter events, like hunting revenge 
	/// </summary>
	internal class SettingCrossAggroWith : BaseSpecialSetting, ISettingWithGlobal
	{
		#region PROPERTIES
		public static IEnumerable<ThingDef> AllCrossAggroable => 
			DefDatabase<ThingDef>.AllDefs.Where(d => d.IsAnimal());

		public List<ThingDef> DefaultCrossAggroWith { get; }
		public List<ThingDef> CrossAggroWith { get; } = new List<ThingDef>();
		#endregion

		#region CONSTRUCTORS
		public SettingCrossAggroWith(ThingDef animal) : 
			base(animal)
		{
			GetValue();
			DefaultCrossAggroWith = Animal?.race?.crossAggroWith != null ? new List<ThingDef>(Animal.race.crossAggroWith) : null;
		}
		#endregion

		#region INTERFACES
		public override void GetValue()
		{
			if (Animal?.race?.crossAggroWith != null)
			{
				CrossAggroWith.Clear();
				CrossAggroWith.AddRange(Animal.race.crossAggroWith);
			}
		}
		public override void SetValue()
		{
			if (Animal?.race != null && Animal.IsAnimal())
				ApplyValues(CrossAggroWith);
		}

		public override void Reset() => 
			ApplyValues(DefaultCrossAggroWith);

		public override bool IsModified() =>
			!(CrossAggroWith.Count == 0 && DefaultCrossAggroWith == null)
			&& (CrossAggroWith.Count != DefaultCrossAggroWith?.Count || CrossAggroWith.Any(d => !DefaultCrossAggroWith.Contains(d)));

		public override void ExposeData()
		{
			if (Scribe.mode != LoadSaveMode.Saving || IsModified())
			{
				// convert defs to defNames
				var defNames = CrossAggroWith.Select(d => d.Def2String()).ToList();
				// save/load
				Scribe_Collections.Look(ref defNames, "CrossAggroWith");

				// convert defNames to defs
				List<ThingDef> crossAggroables = null;
				if (defNames != null)
				{
					crossAggroables = new List<ThingDef>();
					foreach (var defName in defNames)
					{
						var crossAggroable = defName != null && defName != "null" ? DefDatabase<ThingDef>.GetNamed(defName) : null;
						if (crossAggroable != null)
							crossAggroables.Add(crossAggroable);
					}
				}
				// apply
				ApplyValues(crossAggroables ?? DefaultCrossAggroWith);
			}
		}

		public void ResetGlobal()
		{ }
		public void ExposeGlobal()
		{ }
		public bool IsGlobalUsed() =>
			false;
		#endregion

		#region PRIVATE METHODS
		private void ApplyValues(List<ThingDef> values)
		{
			if (Animal?.race == null)
				return;

			// remove all crossaggroables
			if (values == null)
				Animal.race.crossAggroWith = null;
			// add all crossaggroables
			else if (Animal.race.crossAggroWith == null)
				Animal.race.crossAggroWith = new List<ThingDef>(values);
			// add / remove crossaggroables
			else
			{
				var crossAggroables = Animal.race.crossAggroWith;
				// check existing vs values for removed
				for (int i = crossAggroables.Count - 1; i >= 0; i--)
				{
					var crossAggroable = crossAggroables[i];
					if (!values.Contains(crossAggroable))
						crossAggroables.RemoveAt(i);
				}
				// check values vs existing for added
				for (int i = 0; i < values.Count; i++)
				{
					var crossAggroable = values[i];
					if (!crossAggroables.Contains(crossAggroable))
						crossAggroables.Add(crossAggroable);
				}
			}

			// update local storage
			if (values != CrossAggroWith)
			{
				CrossAggroWith.Clear();
				if (Animal.race.crossAggroWith?.Count > 0)
					CrossAggroWith.AddRange(Animal.race.crossAggroWith);
			}
		}
		#endregion
	}
}
