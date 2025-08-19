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
				setting.CrossAggroWith,
				SettingCrossAggroWith.AllCrossAggroable.ToList(),
				MenuGeneratorAdd,
				MenuGeneratorRemove,
				ListToString);

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth) => 0f;

		#region PRIVATE METHODS
		private IEnumerable<Widgets.DropdownMenuElement<ThingDef>> MenuGeneratorAdd(SettingCrossAggroWith target)
		{
			foreach (var e in SettingCanCrossBreedWith.AllCrossBreedables)
			{
				if (target.CrossAggroWith.Contains(e))
					continue;

				yield return new Widgets.DropdownMenuElement<ThingDef>
				{
					option = new FloatMenuOption(e.LabelCap, () => target.CrossAggroWith.Add(e)),
					payload = e,
				};
			}
		}
		private IEnumerable<Widgets.DropdownMenuElement<ThingDef>> MenuGeneratorRemove(SettingCrossAggroWith target)
		{
			foreach (var e in target.CrossAggroWith)
			{
				yield return new Widgets.DropdownMenuElement<ThingDef>
				{
					option = new FloatMenuOption(e.LabelCap, () => target.CrossAggroWith.Remove(e)),
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
