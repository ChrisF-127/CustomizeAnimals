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
	internal class ControlFoodType : BaseSettingControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (BaseSetting<FoodTypeFlags>)animalSettings.Settings["FoodType"];

			CreateSpecialControl(
				offsetY,
				viewWidth,
				"SY_CA.FoodType".Translate(),
				setting);

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth) => 0f;

		#region PRIVATE METHODS
		protected void CreateSpecialControl(
			float offsetY,
			float viewWidth,
			string label,
			BaseSetting<FoodTypeFlags> setting)
		{
			if (setting == null)
				return;

			var controlWidth = GetControlWidth(viewWidth);
			var buttonDim = SettingsRowHeight - 4;
			var textDisplayWidth = controlWidth - buttonDim * 2 - 6;
			var isModified = setting.IsModified();

			// Label
			if (isModified)
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(0, offsetY, controlWidth, SettingsRowHeight), label);
			GUI.color = OriColor;

			// Types
			if (setting.Value != FoodTypeFlags.None)
			{
				Text.Font = GameFont.Tiny;
				var textRect = new Rect(controlWidth, offsetY + 2, textDisplayWidth, SettingsRowHeight - 4);
				Widgets.Label(textRect, setting.Value.ToString());
				DrawTooltip(textRect, setting.Value.ToHumanString().CapitalizeFirst());
				Text.Font = OriTextFont;
			}

			// Add
			var rect = new Rect(controlWidth + textDisplayWidth + 2, offsetY + 2, buttonDim, buttonDim);
			if (!AllTypesSet(setting.Value))
			{
				Widgets.Dropdown(
					rect,
					setting,
					null,
					MenuGeneratorAdd,
					"+");
				DrawTooltip(rect, "SY_CA.TooltipFoodTypeAdd".Translate());
			}

			// Remove
			if (setting.Value != FoodTypeFlags.None)
			{
				rect = new Rect(controlWidth + textDisplayWidth + 4 + buttonDim, offsetY + 2, buttonDim, buttonDim);
				Widgets.Dropdown(
					rect,
					setting,
					null,
					MenuGeneratorRemove,
					"-");
				DrawTooltip(rect, "SY_CA.TooltipFoodTypeRemove".Translate());
			}

			// Reset button
			if (isModified && DrawResetButton(offsetY, viewWidth, setting.DefaultValue.ToString()))
				setting.Value = setting.DefaultValue;
		}

		private bool AllTypesSet(FoodTypeFlags foodTypes)
		{
			foreach (var e in Enum.GetValues(typeof(FoodTypeFlags)).Cast<FoodTypeFlags>())
				if (!foodTypes.HasFlag(e))
					return false;
			return true;
		}

		IEnumerable<Widgets.DropdownMenuElement<FoodTypeFlags>> MenuGeneratorAdd(BaseSetting<FoodTypeFlags> target)
		{
			foreach (var e in Enum.GetValues(typeof(FoodTypeFlags)).Cast<FoodTypeFlags>())
			{
				if (e != FoodTypeFlags.None && !target.Value.HasFlag(e))
				{
					yield return new Widgets.DropdownMenuElement<FoodTypeFlags>
					{
						option = new FloatMenuOption(e.ToString(), () => target.Value |= e)
						{
							tooltip = new TipSignal(e.ToHumanString().CapitalizeFirst())
						},
						payload = e,
					};
				}
			}
		}
		IEnumerable<Widgets.DropdownMenuElement<FoodTypeFlags>> MenuGeneratorRemove(BaseSetting<FoodTypeFlags> target)
		{
			foreach (var e in Enum.GetValues(typeof(FoodTypeFlags)).Cast<FoodTypeFlags>())
			{
				if (e != FoodTypeFlags.None && target.Value.HasFlag(e))
				{
					yield return new Widgets.DropdownMenuElement<FoodTypeFlags>
					{
						option = new FloatMenuOption(e.ToString(), 
						() => 
						{
							target.Value &= ~e;
							if (!target.Value.HasFlag(FoodTypeFlags.VegetableOrFruit))
								target.Value &= ~FoodTypeFlags.Fungus; // Fungus uses the "4096"-bit which has no Enum-flag of its own
						})
						{
							tooltip = new TipSignal(e.ToHumanString().CapitalizeFirst())
						},
						payload = e,
					};
				}
			}
		}
		#endregion
	}
}
