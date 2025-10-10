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
	internal class SettingSpecialTrainables : ListSetting<TrainableDef>
	{
		#region PROPERTIES
		public static Dictionary<TrainableDef, AbilityDef> AbilityDefDict { get; } = new Dictionary<TrainableDef, AbilityDef>();

		protected override string ScribeLabel =>
			"SpecialTrainables";
		#endregion

		#region CONSTRUCTORS
		public SettingSpecialTrainables(ThingDef animal) : 
			base(animal)
		{
			GetValue();
			DefaultValue = Animal?.race?.specialTrainables != null ? new List<TrainableDef>(Animal.race.specialTrainables) : null;
		}
		#endregion

		#region INTERFACES
		public override void GetValue()
		{
			if (Value == null)
				Value = new List<TrainableDef>();
			if (Animal?.race?.specialTrainables != null)
			{
				Value.Clear();
				Value.AddRange(Animal.race.specialTrainables);
			}
		}
		public override void SetValue()
		{
			if (Animal?.race != null && Animal.IsAnimal())
				ApplyValues(Value);
		}

		public override void Reset() => 
			ApplyValues(DefaultValue);

		public override void ExposeData()
		{
			if (Scribe.mode != LoadSaveMode.Saving || IsModified())
			{
				// convert defs to defNames
				var value = Value;
				// save/load
				Scribe_Collections.Look(ref value, ScribeLabel);
				// apply
				ApplyValues(value ?? DefaultValue);
			}
		}

		public override bool IsModified() =>
			!(Value.Count == 0 && DefaultValue == null)
			&& (Value.Count != DefaultValue?.Count || Value.Any(d => !DefaultValue.Contains(d)));
		#endregion

		#region PUBLIC METHODS
		public void UpdateAbilitiesAfterInit(List<Pawn> pawns = null, List<TrainableDef> trainablesWithAbilities = null)
		{
			if (pawns == null)
				pawns = PawnsFinder.All_AliveOrDead;
			if (trainablesWithAbilities == null)
				trainablesWithAbilities = AbilityDefDict.Keys.ToList();

			// check for missing abilities according to trainables
			var add = new List<AbilityDef>();
			foreach (var trainableDef in Value)
			{
				// filter out abilities
				if (!AbilityDefDict.TryGetValue(trainableDef, out var abilityDef))
					continue;
				// mark selected ability for adding
				add.AddIfNotContains(abilityDef);
			}

			// iterate over all available trainable defs to find applied abilities without selected trainables (s. SepcialTrainables)
			var remove = new List<AbilityDef>();
			foreach (var trainableDef in trainablesWithAbilities)
			{
				// filter out abilities
				if (!AbilityDefDict.TryGetValue(trainableDef, out var abilityDef))
					continue;
				// mark non-selected ability for removal
				if (!Value.Contains(trainableDef))
					remove.AddIfNotContains(abilityDef);
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
					foreach (var trainableDef in Animal.race.specialTrainables)
						if (AbilityDefDict.TryGetValue(trainableDef, out var abilityDef) && !removedAbilities.Contains(abilityDef))
							removedAbilities.Add(abilityDef);
					Animal.race.specialTrainables = null;
				}
			}
			// add all trainables
			else if (Animal.race.specialTrainables == null)
			{
				foreach (var trainableDef in values)
					if (AbilityDefDict.TryGetValue(trainableDef, out var abilityDef) && !addedAbilities.Contains(abilityDef))
						addedAbilities.Add(abilityDef);
				Animal.race.specialTrainables = new List<TrainableDef>(values);
			}
			// add / remove trainables
			else
			{
				var specialTrainables = Animal.race.specialTrainables;
				// check existing vs values for removed
				for (int i = specialTrainables.Count - 1; i >= 0; i--)
				{
					var trainableDef = specialTrainables[i];
					if (!values.Contains(trainableDef))
					{
						if (AbilityDefDict.TryGetValue(trainableDef, out var abilityDef) && !removedAbilities.Contains(abilityDef))
							removedAbilities.Add(abilityDef);
						specialTrainables.RemoveAt(i);
					}
				}
				// check values vs existing for added
				for (int i = 0; i < values.Count; i++)
				{
					var trainableDef = values[i];
					if (!specialTrainables.Contains(trainableDef))
					{
						if (AbilityDefDict.TryGetValue(trainableDef, out var abilityDef) && !addedAbilities.Contains(abilityDef))
							addedAbilities.Add(abilityDef);
						specialTrainables.Add(trainableDef);
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
			if (values != Value)
			{
				Value.Clear();
				if (Animal.race.specialTrainables?.Count > 0)
					Value.AddRange(Animal.race.specialTrainables);
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
