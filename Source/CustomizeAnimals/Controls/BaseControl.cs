using CustomizeAnimals.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CustomizeAnimals.Controls
{
	internal abstract class BaseControl
	{
		#region PROPERTIES
		protected static float SettingsViewHeight => CustomizeAnimals.SettingsViewHeight;
		protected static float SettingsRowHeight => CustomizeAnimals.SettingsRowHeight;
		protected static float SettingsIconSize => CustomizeAnimals.SettingsIconSize;
		protected static float SettingsOffsetY => CustomizeAnimals.SettingsOffsetY;

		protected static GameFont OriTextFont => CustomizeAnimals.OriTextFont;
		protected static TextAnchor OriTextAnchor => CustomizeAnimals.OriTextAnchor;
		protected static Color OriColor => CustomizeAnimals.OriColor;
		protected static Color ModifiedColor => CustomizeAnimals.ModifiedColor;
		#endregion

		#region METHODS
		public abstract float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings);
		public abstract float CreateSettingGlobal(float offsetY, float viewWidth);

		public static bool DrawResetButton(float offsetY, float viewWidth, string tooltip)
		{
			var buttonRect = new Rect(viewWidth + 2 - (SettingsRowHeight * 2), offsetY + 2, SettingsRowHeight * 2 - 2, SettingsRowHeight - 4);
			DrawTooltip(buttonRect, "SY_CA.TooltipDefaultValue".Translate() + " " + tooltip);
			return Widgets.ButtonText(buttonRect, "SY_CA.Reset".Translate());
		}
		public static bool DrawUseGlobalCheckBox(float offsetY, float viewWidth, bool useGlobal)
		{
			var checkboxSize = SettingsRowHeight - 8;
			var offsetX = viewWidth + 1 - (SettingsRowHeight + checkboxSize / 2);

			Widgets.Checkbox(offsetX, offsetY + (SettingsRowHeight - checkboxSize) / 2, ref useGlobal, checkboxSize);
			DrawTooltip(new Rect(offsetX, offsetY, checkboxSize, checkboxSize), "SY_CA.TooltipUseGlobal".Translate());

			return useGlobal;
		}
		public static void DrawTooltip(Rect rect, string tooltip)
		{
			if (Mouse.IsOver(rect))
			{
				ActiveTip activeTip = new ActiveTip(tooltip);
				activeTip.DrawTooltip(GenUI.GetMouseAttachedWindowPos(activeTip.TipRect.width, activeTip.TipRect.height) + (UI.MousePositionOnUIInverted - Event.current.mousePosition));
			}
		}

		public static float GetControlWidth(float viewWidth) =>
			viewWidth / 2 - SettingsRowHeight - 4;

		public static float ToCelsius(float temp, TemperatureDisplayMode fromMode)
		{
			switch (fromMode)
			{
				case TemperatureDisplayMode.Celsius:
					return temp;
				case TemperatureDisplayMode.Fahrenheit:
					return (temp - 32f) / 1.8f;
				case TemperatureDisplayMode.Kelvin:
					return temp - 273.15f;
				default:
					throw new InvalidOperationException();
			};
		}
		public static float CelsiusTo(float temp, TemperatureDisplayMode toMode)
		{
			switch (toMode)
			{
				case TemperatureDisplayMode.Celsius:
					return temp;
				case TemperatureDisplayMode.Fahrenheit:
					return temp * 1.8f + 32f;
				case TemperatureDisplayMode.Kelvin:
					return temp + 273.15f;
				default:
					throw new InvalidOperationException();
			};
		}
		#endregion

		#region STANDARD CONTROLS
		protected float CreateNumeric(
			float offsetY,
			float viewWidth,
			AnimalSettings animalSettings,
			string label,
			string tooltip,
			bool isModified,
			float value,
			float defaulValue,
			float min = 0f,
			float max = 1e+9f)
		{
			// check stuff
			if (animalSettings == null)
			{
				Log.Error($"{nameof(CustomizeAnimals)}: create numeric control failed: {nameof(animalSettings)} is null");
				return 0;
			}

			var controlWidth = GetControlWidth(viewWidth);

			// Label
			// Switch color if modified
			if (isModified)
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(0, offsetY, controlWidth, SettingsRowHeight), label);
			GUI.color = OriColor;

			// Settings
			var buffer = value.ToString();
			var textFieldRect = new Rect(controlWidth, offsetY, controlWidth, SettingsRowHeight).ContractedBy(2, 6);
			Widgets.TextFieldNumeric(textFieldRect, ref value, ref buffer, min, max);
			DrawTooltip(textFieldRect, tooltip);

			// Reset button
			if (isModified && DrawResetButton(offsetY, viewWidth, defaulValue.ToString()))
				value = defaulValue;

			return value;
		}

		protected (bool, float, float) CreateNumericGlobalMinMax(
			float offsetY,
			float viewWidth,
			string label,
			string tooltipMin,
			string tooltipMax,
			bool use,
			float minValue,
			float maxValue,
			float min = 0f,
			float max = 1e+9f)
		{
			var controlWidth = GetControlWidth(viewWidth);

			// Label
			// Switch color if modified
			if (use)
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(0, offsetY, controlWidth, SettingsRowHeight), label);
			GUI.color = OriColor;

			var textFieldLabelWidth = 32;
			var textFieldWidth = controlWidth / 2 - textFieldLabelWidth - 2;
			var offsetX = controlWidth;

			// Minimum setting
			Widgets.Label(new Rect(offsetX, offsetY, textFieldLabelWidth, SettingsRowHeight), "min");
			offsetX += textFieldLabelWidth;
			var buffer = minValue.ToString();
			var textFieldRect = new Rect(offsetX, offsetY, textFieldWidth, SettingsRowHeight).ContractedBy(2, 6);
			Widgets.TextFieldNumeric(textFieldRect, ref minValue, ref buffer, min, max);
			DrawTooltip(textFieldRect, tooltipMin);
			offsetX += textFieldWidth + 4;

			// Maximum setting
			Widgets.Label(new Rect(offsetX, offsetY, textFieldLabelWidth, SettingsRowHeight), "max");
			offsetX += textFieldLabelWidth;
			buffer = maxValue.ToString();
			textFieldRect = new Rect(offsetX, offsetY, textFieldWidth, SettingsRowHeight).ContractedBy(2, 6);
			Widgets.TextFieldNumeric(textFieldRect, ref maxValue, ref buffer, min, max);
			DrawTooltip(textFieldRect, tooltipMax);

			// "Apply" checkbox
			use = DrawUseGlobalCheckBox(offsetY, viewWidth, use);

			// Output
			return (use, minValue, maxValue);
		}
		#endregion
	}
}
