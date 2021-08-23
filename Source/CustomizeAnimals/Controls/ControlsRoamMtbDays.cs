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
	internal class ControlsRoamMtbDays : BaseControls
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var controlWidth = GetControlWidth(viewWidth);
			var roamMtbDaysSetting = (SettingRoamMtbDays)animalSettings.Settings["RoamMtbDays"];

			// Label
			// Switch color if modified
			if (roamMtbDaysSetting.IsModified())
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(0, offsetY, controlWidth, SettingsRowHeight), "SY_CA.RoamMtbDays".Translate());
			GUI.color = OriColor;

			// RoamMtbDays Settings
			float? roamMtbDays;
			var roamSelected = roamMtbDaysSetting.Value != null;
			var checkboxSize = SettingsRowHeight - 8;
			Widgets.Checkbox(controlWidth, offsetY + (SettingsRowHeight - checkboxSize) / 2, ref roamSelected, checkboxSize);
			DrawTooltip(new Rect(controlWidth, offsetY, checkboxSize, checkboxSize), "SY_CA.TooltipRoamMtbDaysChk".Translate());
			// RoamMtbDays active = roamer (requires pen!)
			if (roamSelected)
			{
				var roamValue = roamMtbDaysSetting.Value ?? roamMtbDaysSetting.DefaultValue ?? 2;
				var roamBuffer = roamValue.ToString();
				var textFieldRect = new Rect(controlWidth + checkboxSize + 4, offsetY + (SettingsRowHeight - 20) / 2, controlWidth - checkboxSize - 4, 20);
				Widgets.TextFieldNumeric(textFieldRect, ref roamValue, ref roamBuffer, 1);
				DrawTooltip(textFieldRect, "SY_CA.TooltipRoamMtbDays".Translate());
				roamMtbDays = roamValue;
			}
			else
				roamMtbDays = null;

			// Reset button
			if (roamMtbDaysSetting.IsModified() && DrawResetButton(offsetY, viewWidth, (roamMtbDaysSetting.DefaultValue ?? 0).ToString()))
				roamMtbDaysSetting.Reset();
			// Set value
			else
				roamMtbDaysSetting.Value = roamMtbDays;

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth)
		{
			var controlWidth = GetControlWidth(viewWidth);

			// Label
			// Switch color if modified
			if (SettingRoamMtbDays.UseMinimumRoamMtbDays)
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(0, offsetY, controlWidth, SettingsRowHeight), "SY_CA.MinimumRoamMtbDays".Translate());
			GUI.color = OriColor;

			// RoamMtbDays Settings
			float? roamMtbDays;
			var roamSelected = SettingRoamMtbDays.MinimumRoamMtbDays != null;
			var checkboxSize = SettingsRowHeight - 8;
			Widgets.Checkbox(controlWidth, offsetY + (SettingsRowHeight - checkboxSize) / 2, ref roamSelected, checkboxSize);
			DrawTooltip(new Rect(controlWidth, offsetY, checkboxSize, checkboxSize), "SY_CA.TooltipMinimumRoamMtbDaysChk".Translate());
			// RoamMtbDays active = roamer (requires pen!)
			if (roamSelected)
			{
				var roamValue = SettingRoamMtbDays.MinimumRoamMtbDays ?? 2;
				var roamBuffer = roamValue.ToString();
				var textFieldRect = new Rect(controlWidth + checkboxSize + 4, offsetY + (SettingsRowHeight - 20) / 2, controlWidth - checkboxSize - 4, 20);
				Widgets.TextFieldNumeric(textFieldRect, ref roamValue, ref roamBuffer, 1);
				DrawTooltip(textFieldRect, "SY_CA.TooltipMinimumRoamMtbDays".Translate());
				roamMtbDays = roamValue;
			}
			else
				roamMtbDays = null;

			// Set global
			SettingRoamMtbDays.UseMinimumRoamMtbDays = DrawUseGlobalCheckBox(offsetY, viewWidth, SettingRoamMtbDays.UseMinimumRoamMtbDays);
			SettingRoamMtbDays.MinimumRoamMtbDays = roamMtbDays;

			return SettingsRowHeight;
		}
	}
}
