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
	internal class SettingDrawSize : BaseSetting<float>
	{
		#region PROPERTIES
		public static bool UseGlobal { get; set; } = false;
		public static float Global { get; set; } = GlobalDefault;

		public const float GlobalDefault = 1f;
		public const float Minimum = 1e-3f;
		public const float Maximum = 1e3f;
		#endregion

		#region FIELDS
		private float _previousValue = GlobalDefault;
		private float _previousGlobalValue = GlobalDefault;
		#endregion

		#region CONSTRUCTORS
		public SettingDrawSize(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{ }
		#endregion

		#region INTERFACES
		public override void GetValue() =>
			Value = 1f;
		public override void SetValue()
		{
			var pawnKindDefs = DefDatabase<PawnKindDef>.AllDefs.Where(kd => kd.race == Animal);
			if (pawnKindDefs?.Count() > 0)
			{
				var global = Animal.IsAnimal() && UseGlobal ? Global : 1f;

				foreach (var pawnKindDef in pawnKindDefs)
				{
					foreach (var lifeStage in pawnKindDef.lifeStages)
					{
						if (lifeStage.bodyGraphicData != null)
							lifeStage.bodyGraphicData.drawSize *= (Value / _previousValue) * (global / _previousGlobalValue);
						if (lifeStage.dessicatedBodyGraphicData != null)
							lifeStage.dessicatedBodyGraphicData.drawSize *= (Value / _previousValue) * (global / _previousGlobalValue);
					}
				}

				_previousValue = Value;
				_previousGlobalValue = global;
			}
		}

		public override void ExposeData()
		{
			var value = Value;
			Scribe_Values.Look(ref value, "DrawSizeModifier", DefaultValue);
			Value = value;
		}

		public override void ResetGlobal()
		{
			UseGlobal = false;
			Global = GlobalDefault;
		}

		public override void ExposeGlobal()
		{
			var useGlobal = UseGlobal;
			Scribe_Values.Look(ref useGlobal, "UseGlobalDrawSizeModifier");
			UseGlobal = useGlobal;

			var value = Global;
			Scribe_Values.Look(ref value, "GlobalDrawSizeModifier", GlobalDefault);
			Global = value;
		}

		public override bool IsGlobalUsed() =>
			UseGlobal;
		#endregion
	}
}
