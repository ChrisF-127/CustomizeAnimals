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
	internal class SettingSpecialTrainables : BaseSpecialSetting, ISettingWithGlobal
	{
		#region PROPERTIES
		public static IEnumerable<TrainableDef> AllTrainableDefs => 
			DefDatabase<TrainableDef>.AllDefs.Where(d => d.specialTrainable);

		public List<TrainableDef> DefaultSpecialTrainables { get; }
		public List<TrainableDef> SpecialTrainables { get; } = new List<TrainableDef>();
		#endregion

		#region CONSTRUCTORS
		public SettingSpecialTrainables(ThingDef animal) : 
			base(animal)
		{
			GetValue();
			DefaultSpecialTrainables = Animal?.race?.specialTrainables != null ? new List<TrainableDef>(Animal.race.specialTrainables) : null;
		}
		#endregion

		#region INTERFACES
		public override void GetValue()
		{
			if (Animal?.race?.specialTrainables != null)
			{
				SpecialTrainables.Clear();
				SpecialTrainables.AddRange(Animal.race.specialTrainables);
			}
		}
		public override void SetValue()
		{
			if (Animal?.race != null && Animal.IsAnimal())
				ApplyValues(SpecialTrainables);
		}

		public override void Reset() => 
			ApplyValues(DefaultSpecialTrainables);

		public override bool IsModified() =>
			!(SpecialTrainables.Count == 0 && DefaultSpecialTrainables == null)
			&& (SpecialTrainables.Count != DefaultSpecialTrainables?.Count || SpecialTrainables.Any(d => !DefaultSpecialTrainables.Contains(d)));

		public override void ExposeData()
		{
			if (Scribe.mode != LoadSaveMode.Saving || IsModified())
			{
				// convert defs to defNames
				var defNames = SpecialTrainables.Select(d => d.Def2String()).ToList();
				// save/load
				Scribe_Collections.Look(ref defNames, "SpecialTrainables");

				// convert defNames to defs
				List<TrainableDef> trainables = null;
				if (defNames != null)
				{
					trainables = new List<TrainableDef>();
					foreach (var defName in defNames)
					{
						var trainable = defName != null && defName != "null" ? DefDatabase<TrainableDef>.GetNamed(defName) : null;
						if (trainable != null)
							trainables.Add(trainable);
					}
				}
				// apply
				ApplyValues(trainables);
			}
		}

		public void ResetGlobal()
		{ }
		public void ExposeGlobal()
		{ }
		public bool IsGlobalUsed() =>
			false;
		#endregion

		#region PUBLIC METHODS
		public void UpdateAbilitiesAfterInit(List<Pawn> pawns = null, List<TrainableDef> allTrainableDefs = null)
		{
			if (pawns == null)
				pawns = PawnsFinder.All_AliveOrDead;
			if (allTrainableDefs == null)
				allTrainableDefs = AllTrainableDefs.Where(td => td.enablesAbility != null).ToList();

			// check for missing abilities according to trainables
			var add = new List<AbilityDef>();
			foreach (var trainableDef in SpecialTrainables)
			{
				// filter out abilities
				if (trainableDef.enablesAbility == null)
					continue;
				// mark selected ability for adding
				add.Add(trainableDef.enablesAbility);
			}

			// iterate over all available trainable defs to find applied abilities without selected trainables (s. SepcialTrainables)
			var remove = new List<AbilityDef>();
			foreach (var trainableDef in allTrainableDefs)
			{
				// filter out abilities
				if (trainableDef.enablesAbility == null)
					continue;
				// mark non-selected ability for removal
				if (!SpecialTrainables.Contains(trainableDef))
					remove.Add(trainableDef.enablesAbility);
			}

			// update
			UpdateAbilities(pawns, add, remove);
		}
		#endregion

		#region PRIVATE METHODS
		private void ApplyValues(List<TrainableDef> values)
		{
			if (Animal?.race == null)
				return;

			var addedAbilities = new List<AbilityDef>();
			var removedAbilities = new List<AbilityDef>();

			// remove all trainables
			if (values == null)
			{
				if (Animal.race.specialTrainables != null)
				{
					foreach (var trainable in Animal.race.specialTrainables)
						if (trainable.enablesAbility != null && !removedAbilities.Contains(trainable.enablesAbility))
							removedAbilities.Add(trainable.enablesAbility);
					Animal.race.specialTrainables = null;
				}
			}
			// add all trainables
			else if (Animal.race.specialTrainables == null)
			{
				foreach (var trainable in values)
					if (trainable.enablesAbility != null && !addedAbilities.Contains(trainable.enablesAbility))
						addedAbilities.Add(trainable.enablesAbility);
				Animal.race.specialTrainables = new List<TrainableDef>(values);
			}
			// add / remove trainables
			else
			{
				var specialTrainables = Animal.race.specialTrainables;
				// check existing vs values for removed
				for (int i = specialTrainables.Count - 1; i >= 0; i--)
				{
					var trainable = specialTrainables[i];
					if (!values.Contains(trainable))
					{
						if (trainable.enablesAbility != null && !removedAbilities.Contains(trainable.enablesAbility))
							removedAbilities.Add(trainable.enablesAbility);
						specialTrainables.RemoveAt(i);
					}
				}
				// check values vs existing for added
				for (int i = 0; i < values.Count; i++)
				{
					var trainable = values[i];
					if (!specialTrainables.Contains(trainable))
					{
						if (trainable.enablesAbility != null && !addedAbilities.Contains(trainable.enablesAbility))
							addedAbilities.Add(trainable.enablesAbility);
						specialTrainables.Add(trainable);
					}
				}
			}

			// add / remove abilities
			if (removedAbilities.Count > 0 || addedAbilities.Count > 0)
			{
				var pawnKindDef = DefDatabase<PawnKindDef>.AllDefs.FirstOrDefault(d => d.race == Animal);
				if (pawnKindDef == null)
					Log.Error($"No PawnKindDef found for '{Animal.defName}'!");
				else
				{
					// add to / remove from PawnKindDef
					foreach (var abilityDef in removedAbilities)
						pawnKindDef.abilities.Remove(abilityDef);
					foreach (var abilityDef in addedAbilities)
						pawnKindDef.abilities.Add(abilityDef);

					// add to / remove from pawns
					if (Current.Game != null)
						UpdateAbilities(PawnsFinder.All_AliveOrDead, addedAbilities, removedAbilities);
				}
			}

			// update local storage
			if (values != SpecialTrainables)
			{
				SpecialTrainables.Clear();
				if (Animal.race.specialTrainables?.Count > 0)
					SpecialTrainables.AddRange(Animal.race.specialTrainables);
			}
		}

		private void UpdateAbilities(List<Pawn> pawns, List<AbilityDef> addedAbilities, List<AbilityDef> removedAbilities)
		{
			var adds = new int[addedAbilities?.Count ?? 0];
			var totalAdds = 0;
			var removes = new int[removedAbilities?.Count ?? 0];
			var totalRemoves = 0;
			Parallel.ForEach(pawns, pawn =>
			{
				if (pawn?.def != Animal)
					return;

				// remove abilities
				var first = true;
				if (removedAbilities?.Count > 0 && pawn.abilities != null)
				{
					for (int i = 0; i < removedAbilities.Count; i++)
					{
						var abilityDef = removedAbilities[i];
						if (pawn.abilities.abilities.Any(a => a.def == abilityDef))
						{
							pawn.abilities.RemoveAbility(abilityDef);

							lock (this)
							{
								removes[i]++;
								if (first)
								{
									totalRemoves++;
									first = false;
								}
							}
						}
					}
				}

				// add abilities
				first = true;
				if (addedAbilities?.Count > 0)
				{
					if (pawn.abilities == null)
						pawn.abilities = new Pawn_AbilityTracker(pawn);
					for (int i = 0; i < addedAbilities.Count; i++)
					{
						var abilityDef = addedAbilities[i];
						if (!pawn.abilities.abilities.Any(a => a.def == abilityDef))
						{
							pawn.abilities.GainAbility(abilityDef);

							lock (this)
							{
								adds[i]++;
								if (first)
								{
									totalAdds++;
									first = false;
								}
							}
						}
					}
				}

				// remove ability tracker when empty
				if (pawn.abilities?.abilities?.Count == 0)
					pawn.abilities = null;
			});

			if (totalAdds > 0)
				Log.Message($"{nameof(CustomizeAnimals)}.{nameof(UpdateAbilities)}: '{Animal.LabelCap}' ({Animal.defName}): {totalAdds} modified, added: {toString(addedAbilities, adds)}");
			if (totalRemoves > 0)
				Log.Message($"{nameof(CustomizeAnimals)}.{nameof(UpdateAbilities)}: '{Animal.LabelCap}' ({Animal.defName}): {totalRemoves} modified, removed: {toString(removedAbilities, removes)}");

			string toString(IList<AbilityDef> values, int[] count)
			{
				if (!(values?.Count > 0))
					return "";

				var sb = new StringBuilder();
				for (int i = 0; i < values.Count; i++)
				{
					if (count[i] == 0)
						continue;

					sb.Append(count[i]);
					sb.Append(" ");
					sb.Append(values[i].LabelCap);

					if (i < values.Count - 1)
						sb.Append(", ");
				}
				return sb.ToString();
			}
		}
		#endregion
	}
}
