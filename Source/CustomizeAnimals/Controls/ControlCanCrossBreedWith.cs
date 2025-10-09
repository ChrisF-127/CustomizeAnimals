using CustomizeAnimals.Settings;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CustomizeAnimals.Controls
{
	internal class ControlCanCrossBreedWith : BaseSettingControl
	{
		#region PROPERTIES
		public static List<ThingDef> AllCrossBreedables { get; private set; }
		#endregion

		#region OVERRIDE
		public override void Initialize()
		{
			var defs = DefDatabase<ThingDef>.AllDefs.Where(d => d.IsAnimal()).ToList();
			defs.Sort((a, b) => string.Compare(a.label, b.label, true));
			AllCrossBreedables = defs;
		}

		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			if (animalSettings.IsHumanLike)
				return 0f;

			var setting = (SettingCanCrossBreedWith)animalSettings.ReproductionSettings["CanCrossBreedWith"];

			CreateMultiSelector(
				offsetY,
				viewWidth,
				"SY_CA.CanCrossBreedWith".Translate(),
				"SY_CA.TooltipCanCrossBreedWithAdd".Translate(),
				"SY_CA.TooltipCanCrossBreedWithRemove".Translate(),
				setting,
				setting.Value,
				setting.DefaultValue,
				AllCrossBreedables.ToList(),
				MenuGeneratorAdd,
				MenuGeneratorRemove,
				ListToString);

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth) => 0f;
		#endregion

		#region PRIVATE METHODS
		private IEnumerable<Widgets.DropdownMenuElement<ThingDef>> MenuGeneratorAdd(SettingCanCrossBreedWith target)
		{
			foreach (var e in AllCrossBreedables)
			{
				if (target.Value.Contains(e))
					continue;

				yield return new Widgets.DropdownMenuElement<ThingDef>
				{
					option = new FloatMenuOption(e.LabelCap, () => target.Value.Add(e)),
					payload = e,
				};
			}
		}
		private IEnumerable<Widgets.DropdownMenuElement<ThingDef>> MenuGeneratorRemove(SettingCanCrossBreedWith target)
		{
			foreach (var e in target.Value)
			{
				yield return new Widgets.DropdownMenuElement<ThingDef>
				{
					option = new FloatMenuOption(e.LabelCap, () => target.Value.Remove(e)),
					payload = e,
				};
			}
		}
		private static string ListToString(IList<ThingDef> values)
		{
			if (!(values?.Count > 0))
				return "";

			var sb = new StringBuilder(values[0].LabelCap);
			for (int i = 1; i < values.Count; i++)
			{
				sb.Append(", ");
				sb.Append(values[i].LabelCap);
			}
			return sb.ToString();
		}
		#endregion
	}
}
