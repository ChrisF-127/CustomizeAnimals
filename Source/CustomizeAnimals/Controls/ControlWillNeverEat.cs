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
	internal class ControlWillNeverEat : BaseSettingControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (SettingWillNeverEat)animalSettings.Settings["WillNeverEat"];

			var list = setting.AuxiliaryList;
			var isModified = setting.IsModified();

			// Selector control
			CreateSpecialControl(
				offsetY,
				viewWidth,
				"SY_CA.WillNeverEat".Translate(),
				"SY_CA.TooltipWillNeverEatAdd".Translate(),
				"SY_CA.TooltipWillNeverEatRemove".Translate(),
				ListToString(list),
				isModified,
				list);

			// Reset button
			if (isModified)
			{
				var defaultValue = setting.DefaultValue;
				if (DrawResetButton(offsetY, viewWidth, "\n" + ListToString(defaultValue)))
				{
					list.Clear();
					if (defaultValue != null)
						foreach (var def in defaultValue)
							list.Add(def);
				}
			}

			// Set value
			setting.Aux2Value();

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth)
		{
			var list = SettingWillNeverEat.GlobalList;
			var use = SettingWillNeverEat.UseGlobalList;

			// Selector control
			CreateSpecialControl(
				offsetY,
				viewWidth,
				"SY_CA.WillNeverEat".Translate(),
				"SY_CA.TooltipWillNeverEatAdd".Translate(),
				"SY_CA.TooltipWillNeverEatRemove".Translate(),
				ListToString(list),
				use,
				list);

			// "Apply" checkbox
			SettingWillNeverEat.UseGlobalList = DrawUseGlobalCheckBox(offsetY, viewWidth, use, SettingsRowHeight);

			return SettingsRowHeight;
		}

		#region PRIVATE METHODS
		private void CreateSpecialControl(
			float offsetY,
			float viewWidth,
			string label,
			string tooltipAdd,
			string tooltipRemove,
			string tooltipThings,
			bool isModified,
			List<ThingDef> list)
		{
			if (list == null)
				return;

			var controlWidth = GetControlWidth(viewWidth);
			var iconDisplayWidth = controlWidth * 0.667f;
			var dropdownWidth = controlWidth * 0.333f / 2f;

			// Label
			if (isModified)
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(0, offsetY, controlWidth, SettingsRowHeight), label);
			GUI.color = OriColor;

			// Icons
			if (list.Count > 0)
			{
				var dim = SettingsRowHeight - 4;
				for (int i = 0; i < list.Count; i++)
				{
					var x = (iconDisplayWidth - dim) / (list.Count + 1) * (i + 1);
					Widgets.DefIcon(new Rect(controlWidth + x, offsetY + 2, dim, dim), list[i]);
				}
			}
			DrawTooltip(new Rect(controlWidth, offsetY + 2, iconDisplayWidth, SettingsRowHeight - 4), tooltipThings);

			// Add
			var rect = new Rect(controlWidth + iconDisplayWidth + 2, offsetY + 2, dropdownWidth - 2, SettingsRowHeight - 4);
			Widgets.Dropdown(
				rect,
				list,
				null,
				MenuGeneratorAdd,
				"+");
			DrawTooltip(rect, tooltipAdd);

			// Remove
			if (list.Count > 0)
			{
				rect = new Rect(controlWidth + iconDisplayWidth + dropdownWidth, offsetY + 2, dropdownWidth - 2, SettingsRowHeight - 4);
				Widgets.Dropdown(
					rect,
					list,
					null,
					MenuGeneratorRemove,
					"-");
				DrawTooltip(rect, tooltipRemove);
			}

			// Sort
			list.SortBy((def) => def.label);
		}

		private string ListToString(List<ThingDef> list)
		{
			if (list != null)
			{
				string output = "";
				for (int i = 0; i < list.Count; i++)
				{
					if (i > 0)
						output += "\n";
					output += list[i].label.CapitalizeFirst();
				}
				return output;
			}
			return "None";
		}

		private IEnumerable<Widgets.DropdownMenuElement<ThingDef>> MenuGeneratorAdd(List<ThingDef> target)
		{
			var filteredDefs = new List<ThingDef>();
			foreach (var def in DefDatabase<ThingDef>.AllDefs)
			{
				if (def.ingestible != null
					&& def.plant != null // technically other things could also be set as "never eat", too much work to do that for now though
					//&& !def.thingCategories.Contains(ThingCategoryDefOf.MeatRaw)
					//&& !def.thingCategories.Contains(ThingCategoryDefOf.CorpsesAnimal)
					//&& !def.thingCategories.Contains(ThingCategoryDefOf.CorpsesHumanlike)
					//&& !def.thingCategories.Contains(ThingCategoryDefOf.CorpsesInsect)
					//&& !def.thingCategories.Contains(ThingCategoryDefOf.CorpsesMechanoid)
					&& !target.Contains(def))
					filteredDefs.Add(def);
			}
			filteredDefs.SortBy((def) => def.label);

			foreach (var def in filteredDefs)
			{
				yield return new Widgets.DropdownMenuElement<ThingDef>
				{
					option = new FloatMenuOption(def.label.CapitalizeFirst(), () => target.Add(def)),
					payload = def,
				};
			}
		}

		private IEnumerable<Widgets.DropdownMenuElement<ThingDef>> MenuGeneratorRemove(List<ThingDef> target)
		{
			for (int i = target.Count - 1; i >= 0; i--)
			{
				var def = target[i];
				yield return new Widgets.DropdownMenuElement<ThingDef>
				{
					option = new FloatMenuOption(def.label.CapitalizeFirst(), () => target.Remove(def)),
					payload = def,
				};
			}
		}
		#endregion
	}
}
