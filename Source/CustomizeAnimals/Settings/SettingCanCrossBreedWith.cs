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
	/// Cross-breeding depends on the "can cross-breed with" setting of the male only
	/// </summary>
	internal class SettingCanCrossBreedWith : BaseSpecialSetting, ISettingWithGlobal
	{
		#region PROPERTIES
		public static IEnumerable<ThingDef> AllCrossBreedables => 
			DefDatabase<ThingDef>.AllDefs.Where(d => d.IsAnimal());

		public List<ThingDef> DefaultCanCrossBreedWith { get; }
		public List<ThingDef> CanCrossBreedWith { get; } = new List<ThingDef>();
		#endregion

		#region CONSTRUCTORS
		public SettingCanCrossBreedWith(ThingDef animal) : 
			base(animal)
		{
			GetValue();
			DefaultCanCrossBreedWith = Animal?.race?.canCrossBreedWith != null ? new List<ThingDef>(Animal.race.canCrossBreedWith) : null;
		}
		#endregion

		#region INTERFACES
		public override void GetValue()
		{
			if (Animal?.race?.canCrossBreedWith != null)
			{
				CanCrossBreedWith.Clear();
				CanCrossBreedWith.AddRange(Animal.race.canCrossBreedWith);
			}
		}
		public override void SetValue()
		{
			if (Animal?.race != null && Animal.IsAnimal())
				ApplyValues(CanCrossBreedWith);
		}

		public override void Reset() => 
			ApplyValues(DefaultCanCrossBreedWith);

		public override bool IsModified() =>
			!(CanCrossBreedWith.Count == 0 && DefaultCanCrossBreedWith == null)
			&& (CanCrossBreedWith.Count != DefaultCanCrossBreedWith?.Count || CanCrossBreedWith.Any(d => !DefaultCanCrossBreedWith.Contains(d)));

		public override void ExposeData()
		{
			if (Scribe.mode != LoadSaveMode.Saving || IsModified())
			{
				// convert defs to defNames
				var defNames = CanCrossBreedWith.Select(d => d.Def2String()).ToList();
				// save/load
				Scribe_Collections.Look(ref defNames, "CanCrossBreedWith");

				// convert defNames to defs
				List<ThingDef> crossBreedables = null;
				if (defNames != null)
				{
					crossBreedables = new List<ThingDef>();
					foreach (var defName in defNames)
					{
						var crossBreedable = defName != null && defName != "null" ? DefDatabase<ThingDef>.GetNamed(defName) : null;
						if (crossBreedable != null)
							crossBreedables.Add(crossBreedable);
					}
				}
				// apply
				ApplyValues(crossBreedables);
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

			// remove all crossbreedables
			if (values == null)
				Animal.race.canCrossBreedWith = null;
			// add all crossbreedables
			else if (Animal.race.canCrossBreedWith == null)
				Animal.race.canCrossBreedWith = new List<ThingDef>(values);
			// add / remove crossbreedables
			else
			{
				var crossBreedables = Animal.race.canCrossBreedWith;
				// check existing vs values for removed
				for (int i = crossBreedables.Count - 1; i >= 0; i--)
				{
					var crossBreedable = crossBreedables[i];
					if (!values.Contains(crossBreedable))
						crossBreedables.RemoveAt(i);
				}
				// check values vs existing for added
				for (int i = 0; i < values.Count; i++)
				{
					var crossBreedable = values[i];
					if (!crossBreedables.Contains(crossBreedable))
						crossBreedables.Add(crossBreedable);
				}
			}

			// update local storage
			if (values != CanCrossBreedWith)
			{
				CanCrossBreedWith.Clear();
				if (Animal.race.canCrossBreedWith?.Count > 0)
					CanCrossBreedWith.AddRange(Animal.race.canCrossBreedWith);
			}
		}
		#endregion
	}
}
