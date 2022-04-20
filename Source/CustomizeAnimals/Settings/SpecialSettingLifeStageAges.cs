using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizeAnimals.Settings
{
	internal class SpecialSettingLifeStageAges : BaseSpecialSetting
	{
		public List<LifeStageAgeSetting> LifeStageAges { get; set; } = new List<LifeStageAgeSetting>();

		public SpecialSettingLifeStageAges(ThingDef animal) : base(animal)
		{
			GetValue();
		}

		public override void GetValue()
		{
			var lifeStages = Animal?.race?.lifeStageAges;
			if (lifeStages != null)
			{
				if (LifeStageAges == null)
					LifeStageAges = new List<LifeStageAgeSetting>();
				else if (LifeStageAges.Count > 0)
				{
					LifeStageAges.Clear();
					Log.Message($"{Animal.defName}: LifeStages list cleared! (shouldn't happen)");
				}
				foreach (var lifeStage in lifeStages)
					LifeStageAges.Add(new LifeStageAgeSetting(lifeStage));
			}
			else
				Log.Warning($"{nameof(CustomizeAnimals)}.{nameof(SpecialSettingLifeStageAges)}: {Animal?.defName} lifeStages is null!");
		}
		public override void SetValue()
		{
			foreach (var lifeStage in LifeStageAges)
				lifeStage.SetValue();
		}

		public override void Reset()
		{
			foreach (var lifeStage in LifeStageAges)
				lifeStage.Reset();
		}
		public override bool IsModified()
		{
			foreach (var lifeStage in LifeStageAges)
				if (lifeStage.IsModified())
					return true;
			return false;
		}

		public override void ExposeData()
		{
			if (Scribe.mode != LoadSaveMode.Saving || IsModified())
			{
				if (Scribe.EnterNode("LifeStageAges"))
				{
					foreach (var lifeStageAge in LifeStageAges)
						lifeStageAge.ExposeData();
					Scribe.ExitNode();
				}
			}
		}
	}

	internal class LifeStageAgeSetting : IExposable
	{
		public LifeStageAge LifeStageAge { get; }

		public float DefaultMinAge { get; private set; }
		public float MinAge { get; set; }

		public LifeStageAgeSetting(LifeStageAge lifeStageAge)
		{
			LifeStageAge = lifeStageAge;

			GetValue();

			DefaultMinAge = MinAge;
		}

		#region PUBLIC METHODS
		public void GetValue()
		{
			if (LifeStageAge == null)
				return;
			MinAge = LifeStageAge.minAge;
		}
		public void SetValue()
		{
			if (LifeStageAge == null)
				return;
			LifeStageAge.minAge = MinAge;
		}

		public void Reset() =>
			MinAge = DefaultMinAge;
		public bool IsModified() =>
			MinAge != DefaultMinAge;

		public void ExposeData()
		{
			if (LifeStageAge?.def?.defName == null)
				return;
			
			if (Scribe.mode != LoadSaveMode.Saving || IsModified())
			{
				if (Scribe.EnterNode(LifeStageAge.def.defName))
				{
					var value = MinAge;
					Scribe_Values.Look(ref value, "MinAge", DefaultMinAge);
					MinAge = value;

					Scribe.ExitNode();
				}
			}
		}
		#endregion

	}
}
