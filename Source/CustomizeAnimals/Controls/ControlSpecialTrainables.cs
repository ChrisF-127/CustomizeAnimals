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

			CreateSpecialControl(
				offsetY,
				viewWidth,
				"SY_CA.SpecialTrainables".Translate(),
				"SY_CA.TooltipSpecialTrainablesAdd".Translate(),
				"SY_CA.TooltipSpecialTrainablesRemove".Translate(),
				setting);

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth) => 0f;

		#region PRIVATE METHODS
		protected void CreateSpecialControl(
			float offsetY,
			float viewWidth,
			string label,
			string tooltipAdd,
			string tooltipRemove,
			SettingSpecialTrainables setting)
		{
			if (setting == null)
				return;

			var controlWidth = GetControlWidth(viewWidth);
			var buttonDim = SettingsRowHeight - 4;
			var textDisplayWidth = controlWidth - buttonDim * 2 - 6;
			var isModified = setting.IsModified();

			var allTrainables = SettingSpecialTrainables.AllTrainableDefs.ToList();

			// Label
			if (isModified)
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(0, offsetY, controlWidth, SettingsRowHeight), label);
			GUI.color = OriColor;

			// Types
			if (setting.SpecialTrainables.Count > 0)
			{
				Text.Font = GameFont.Tiny;
				var textRect = new Rect(controlWidth, offsetY + 2, textDisplayWidth, SettingsRowHeight - 4);
				var text = ToString(setting.SpecialTrainables);
				Widgets.Label(textRect, text);
				DrawTooltip(textRect, text);
				Text.Font = OriTextFont;
			}

			// Add
			var rect = new Rect(controlWidth + textDisplayWidth + 2, offsetY + 2, buttonDim, buttonDim);
			if (!AllSet(setting.SpecialTrainables, allTrainables))
			{
				Widgets.Dropdown(
					rect,
					setting,
					null,
					MenuGeneratorAdd,
					"+");
				DrawTooltip(rect, tooltipAdd);
			}

			// Remove
			if (setting.SpecialTrainables.Count > 0)
			{
				rect = new Rect(controlWidth + textDisplayWidth + 4 + buttonDim, offsetY + 2, buttonDim, buttonDim);
				Widgets.Dropdown(
					rect,
					setting,
					null,
					MenuGeneratorRemove,
					"-");
				DrawTooltip(rect, tooltipRemove);
			}

			// Reset button
			if (isModified && DrawResetButton(offsetY, viewWidth, ToString(setting.DefaultSpecialTrainables)))
				setting.Reset();
		}

		private bool AllSet(List<TrainableDef> current, List<TrainableDef> all) =>
			current.Count == all.Count && current.All(d => all.Contains(d));

		private static string ToString(List<TrainableDef> values)
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

		private IEnumerable<Widgets.DropdownMenuElement<TrainableDef>> MenuGeneratorAdd(SettingSpecialTrainables target)
		{
			foreach (var e in SettingSpecialTrainables.AllTrainableDefs)
			{
				if (!target.SpecialTrainables.Contains(e))
				{
					yield return new Widgets.DropdownMenuElement<TrainableDef>
					{
						option = new FloatMenuOption(e.LabelCap, () => target.SpecialTrainables.Add(e)),
						payload = e,
					};
				}
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
		#endregion
	}
}
