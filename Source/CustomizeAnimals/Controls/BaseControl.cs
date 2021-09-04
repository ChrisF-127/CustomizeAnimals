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
	internal delegate float ConvertDelegate(float value);

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
		public static void DrawTextFieldUnit(Rect rect, string unit)
		{
			if (unit == null)
				return;
			Text.Anchor = TextAnchor.MiddleRight;
			Widgets.Label(new Rect(rect.x + 4, rect.y + 1, rect.width - 8, rect.height), unit);
			Text.Anchor = TextAnchor.MiddleLeft;
		}

		public static float GetControlWidth(float viewWidth) =>
			viewWidth / 2 - SettingsRowHeight - 4;

		public static float ToPercent(float value) =>
			(float)Math.Round(value * 100f, 5);
		public static float FromPercent(float value) =>
			(float)Math.Round(value /= 100f, 5);
		#endregion
	}

	internal abstract class BaseSettingControl : BaseControl
	{
		#region PROPERTIES
		#endregion

		#region FIELDS
		protected string ValueBuffer = null;
		protected string MinValueBuffer = null;
		protected string MaxValueBuffer = null;
		#endregion

		#region METHODS
		public abstract float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings);
		public abstract float CreateSettingGlobal(float offsetY, float viewWidth);

		public virtual void ResetTextBuffers()
		{
			ValueBuffer = null;
			MinValueBuffer = null;
			MaxValueBuffer = null;
		}
		#endregion
		
		#region STANDARD CONTROLS
		#region ANIMAL SETTING
		protected float CreateNumeric(
			float offsetY,
			float viewWidth,
			string label,
			string tooltip,
			bool isModified,
			float value,
			float defaultValue,
			float min = 0f,
			float max = 1e+9f,
			ConvertDelegate to = null,
			ConvertDelegate back = null,
			string unit = null)
		{
			var controlWidth = GetControlWidth(viewWidth);

			// Label
			// Switch color if modified
			if (isModified)
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(0, offsetY, controlWidth, SettingsRowHeight), label);
			GUI.color = OriColor;

			// Convert
			if (to != null)
			{
				value = to(value);
				min = to(min);
				max = to(max);
			}

			// Settings
			var textFieldRect = new Rect(controlWidth + 2, offsetY + 6, controlWidth - 4, SettingsRowHeight - 12);
			Widgets.TextFieldNumeric(textFieldRect, ref value, ref ValueBuffer, min, max);
			DrawTooltip(textFieldRect, tooltip);

			// Convert back
			if (back != null)
			{
				value = back(value);
			}

			// Unit
			DrawTextFieldUnit(textFieldRect, unit);

			// Reset button
			if (isModified && DrawResetButton(offsetY, viewWidth, defaultValue.ToString()))
			{
				value = defaultValue;
				ValueBuffer = null;
			}

			return value;
		}
		protected float? CreateNullableNumeric(
			float offsetY,
			float viewWidth,
			string label,
			string labelDisabled,
			string tooltip,
			string tooltipCheckbox,
			bool isModified,
			float? value,
			float? defaultValue,
			float min = 0f,
			float max = 1e+9f,
			ConvertDelegate to = null,
			ConvertDelegate back = null,
			string unit = null)
		{
			var controlWidth = GetControlWidth(viewWidth);

			// Label
			// Switch color if modified
			if (isModified)
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(0, offsetY, controlWidth, SettingsRowHeight), label);
			GUI.color = OriColor;

			// Convert
			if (to != null)
			{
				if (value is float v)
					value = to(v);
				min = to(min);
				max = to(max);
			}

			// Settings
			var selected = value != null;
			var checkboxSize = SettingsRowHeight - 8;
			Widgets.Checkbox(controlWidth, offsetY + (SettingsRowHeight - checkboxSize) / 2, ref selected, checkboxSize);
			DrawTooltip(new Rect(controlWidth, offsetY, checkboxSize, checkboxSize), tooltipCheckbox);

			// Value
			float offsetX = controlWidth + checkboxSize + 4;
			float width = controlWidth - checkboxSize - 6;
			if (selected)
			{
				var textFieldRect = new Rect(offsetX, offsetY + 6, width, SettingsRowHeight - 12);
				float val = value ?? defaultValue ?? min;
				Widgets.TextFieldNumeric(textFieldRect, ref val, ref ValueBuffer, min, max);
				DrawTooltip(textFieldRect, tooltip);

				// Unit
				DrawTextFieldUnit(textFieldRect, unit);

				value = val;
			}
			else
			{
				// Label when disabled
				if (labelDisabled != null)
				{
					Text.Anchor = TextAnchor.MiddleCenter;
					Widgets.Label(new Rect(offsetX, offsetY + 4, width, SettingsRowHeight - 8), labelDisabled);
					Text.Anchor = TextAnchor.MiddleLeft;
				}

				value = null;
			}

			// Convert back
			if (back != null)
			{
				if (value is float v)
					value = back(v);
			}

			// Reset button
			if (isModified && DrawResetButton(offsetY, viewWidth, (defaultValue ?? 0).ToString()))
			{
				value = defaultValue;
				ValueBuffer = null;
			}

			// Output
			return value;
		}
		#endregion

		#region GLOBAL SETTINGS
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
			float max = 1e+9f,
			ConvertDelegate to = null,
			ConvertDelegate back = null,
			string unit = null)
		{
			var controlWidth = GetControlWidth(viewWidth);

			// Label
			// Switch color if modified
			if (use)
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(0, offsetY, controlWidth, SettingsRowHeight), label);
			GUI.color = OriColor;

			var textFieldLabelWidth = 28;
			var textFieldWidth = controlWidth / 2 - textFieldLabelWidth - 2;
			var offsetX = controlWidth;

			// Convert
			if (to != null)
			{
				minValue = to(minValue);
				maxValue = to(maxValue);
				min = to(min);
				max = to(max);
			}

			// Minimum setting
			Widgets.Label(new Rect(offsetX, offsetY, textFieldLabelWidth, SettingsRowHeight), "min");
			offsetX += textFieldLabelWidth;
			var textFieldRect = new Rect(offsetX + 2, offsetY + 6, textFieldWidth - 4, SettingsRowHeight - 12);
			Widgets.TextFieldNumeric(textFieldRect, ref minValue, ref MinValueBuffer, min, max);
			DrawTooltip(textFieldRect, tooltipMin);

			DrawTextFieldUnit(textFieldRect, unit);

			offsetX += textFieldWidth + 4;

			// Maximum setting
			Widgets.Label(new Rect(offsetX, offsetY, textFieldLabelWidth, SettingsRowHeight), "max");
			offsetX += textFieldLabelWidth;
			textFieldRect = new Rect(offsetX + 2, offsetY + 6, textFieldWidth - 4, SettingsRowHeight - 12);
			Widgets.TextFieldNumeric(textFieldRect, ref maxValue, ref MaxValueBuffer, min, max);
			DrawTooltip(textFieldRect, tooltipMax);

			// Unit
			DrawTextFieldUnit(textFieldRect, unit);

			// Convert back
			if (back != null)
			{
				minValue = back(minValue);
				maxValue = back(maxValue);
			}

			// "Apply" checkbox
			use = DrawUseGlobalCheckBox(offsetY, viewWidth, use);

			// Output
			return (use, minValue, maxValue);
		}
		protected (bool, float?) CreateNullableNumericGlobal(
			float offsetY,
			float viewWidth,
			string label,
			string labelDisabled,
			string tooltip,
			string tooltipCheckbox,
			bool use,
			float? value,
			float defaultValue,
			float min = 0f,
			float max = 1e+9f,
			ConvertDelegate to = null,
			ConvertDelegate back = null,
			string unit = null)
		{
			var controlWidth = GetControlWidth(viewWidth);

			// Label
			// Switch color if modified
			if (use)
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(0, offsetY, controlWidth, SettingsRowHeight), label);
			GUI.color = OriColor;

			// Convert
			if (to != null)
			{
				if (value is float v)
					value = to(v);
				min = to(min);
				max = to(max);
			}

			// Settings
			var selected = value != null;
			var checkboxSize = SettingsRowHeight - 8;
			Widgets.Checkbox(controlWidth, offsetY + (SettingsRowHeight - checkboxSize) / 2, ref selected, checkboxSize);
			DrawTooltip(new Rect(controlWidth, offsetY, checkboxSize, checkboxSize), tooltipCheckbox);

			// Value
			float offsetX = controlWidth + checkboxSize + 4;
			float width = controlWidth - checkboxSize - 6;
			if (selected)
			{
				var textFieldRect = new Rect(offsetX, offsetY + 6, width, SettingsRowHeight - 12);
				float val = value ?? defaultValue;
				Widgets.TextFieldNumeric(textFieldRect, ref val, ref ValueBuffer, min, max);
				DrawTooltip(textFieldRect, tooltip);

				// Unit
				DrawTextFieldUnit(textFieldRect, unit);

				value = val;
			}
			else
			{
				// Label when disabled
				if (labelDisabled != null)
				{
					Text.Anchor = TextAnchor.MiddleCenter;
					Widgets.Label(new Rect(offsetX, offsetY + 4, width, SettingsRowHeight - 8), labelDisabled);
					Text.Anchor = TextAnchor.MiddleLeft;
				}

				value = null;
			}

			// Convert back
			if (back != null)
			{
				if (value is float v)
					value = back(v);
			}

			// "Apply" checkbox
			use = DrawUseGlobalCheckBox(offsetY, viewWidth, use);

			// Output
			return (use, value);
		}
		#endregion
		#endregion
	}
}
