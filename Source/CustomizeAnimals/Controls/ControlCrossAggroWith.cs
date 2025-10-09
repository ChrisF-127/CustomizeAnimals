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
	internal class ControlCrossAggroWith : BaseSettingControl
	{
		#region PROPERTIES
		public static List<ThingDef> AllCrossAggroable { get;  private set; }
		#endregion

		#region OVERRIDES
		public override void Initialize()
		{
			var defs = DefDatabase<ThingDef>.AllDefs.Where(d => d.IsAnimal()).ToList();
			defs.Sort((a, b) => string.Compare(a.label, b.label, true));
			AllCrossAggroable = defs;
		}

		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			if (animalSettings.IsHumanLike)
				return 0f;

			var setting = (SettingCrossAggroWith)animalSettings.GeneralSettings["CrossAggroWith"];

			CreateMultiSelector(
				offsetY,
				viewWidth,
				"SY_CA.CrossAggroWith".Translate(),
				"SY_CA.TooltipCrossAggroWithAdd".Translate(),
				"SY_CA.TooltipCrossAggroWithRemove".Translate(),
				setting,
				setting.Value,
				setting.DefaultValue,
				AllCrossAggroable,
				MenuGeneratorAdd,
				MenuGeneratorRemove,
				ListToString);

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth) => 0f;
		#endregion

		#region PRIVATE METHODS
		private IEnumerable<Widgets.DropdownMenuElement<ThingDef>> MenuGeneratorAdd(SettingCrossAggroWith target)
		{
			foreach (var e in AllCrossAggroable)
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
		private IEnumerable<Widgets.DropdownMenuElement<ThingDef>> MenuGeneratorRemove(SettingCrossAggroWith target)
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
