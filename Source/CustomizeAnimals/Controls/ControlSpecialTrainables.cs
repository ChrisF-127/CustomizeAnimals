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
	internal class ControlSpecialTrainables : BaseSettingControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (SettingSpecialTrainables)animalSettings.GeneralSettings["SpecialTrainables"];

			CreateMultiSelector(
				offsetY,
				viewWidth,
				"SY_CA.SpecialTrainables".Translate(),
				"SY_CA.TooltipSpecialTrainablesAdd".Translate(),
				"SY_CA.TooltipSpecialTrainablesRemove".Translate(),
				setting,
				setting.SpecialTrainables,
				SettingSpecialTrainables.AllTrainableDefs.ToList(),
				MenuGeneratorAdd,
				MenuGeneratorRemove,
				ListToString);

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth) => 0f;

		#region PRIVATE METHODS
		private IEnumerable<Widgets.DropdownMenuElement<TrainableDef>> MenuGeneratorAdd(SettingSpecialTrainables target)
		{
			foreach (var e in SettingSpecialTrainables.AllTrainableDefs)
			{
				if (target.SpecialTrainables.Contains(e))
					continue;

				yield return new Widgets.DropdownMenuElement<TrainableDef>
				{
					option = new FloatMenuOption(e.LabelCap, () => target.SpecialTrainables.Add(e)),
					payload = e,
				};
			}
		}
		private IEnumerable<Widgets.DropdownMenuElement<TrainableDef>> MenuGeneratorRemove(SettingSpecialTrainables target)
		{
			foreach (var e in target.SpecialTrainables)
			{
				yield return new Widgets.DropdownMenuElement<TrainableDef>
				{
					option = new FloatMenuOption(e.LabelCap, () => target.SpecialTrainables.Remove(e)),
					payload = e,
				};
			}
		}
		private static string ListToString(IList<TrainableDef> values)
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
