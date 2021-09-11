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
		protected static float SettingsDoubleRowHeight => CustomizeAnimals.SettingsDoubleRowHeight;
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
		public static bool DrawUseGlobalCheckBox(float offsetY, float viewWidth, bool useGlobal, float height)
		{
			var checkboxSize = SettingsRowHeight - 8;
			var offsetX = viewWidth + 1 - (SettingsRowHeight + checkboxSize / 2);

			Widgets.Checkbox(offsetX, offsetY + (height - checkboxSize) / 2, ref useGlobal, checkboxSize);
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
		public static void DrawTextFieldUnit(Rect rect, float? value, string unit)
		{
			Text.Anchor = TextAnchor.MiddleRight;
			Widgets.Label(new Rect(rect.x + 4, rect.y + 1, rect.width - 8, rect.height), $"{value?.ToString() ?? ""} {unit ?? ""}");
			Text.Anchor = TextAnchor.MiddleLeft;
		}

		public static float GetControlWidth(float viewWidth) =>
			viewWidth / 2 - SettingsRowHeight - 4;

		public static float ConvertToPercent(float value) =>
			value * 100f;
		public static float FromPercent(float value) =>
			value / 100f;

		protected float? Convert(ConvertDelegate convert, float? value) =>
			convert != null && value != null ? (float?)convert((float)value) : null;
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
		protected void CreateText(
			float offsetY,
			float viewWidth,
			string label,
			string text)
		{
			var controlWidth = GetControlWidth(viewWidth);

			// Label
			Widgets.Label(new Rect(0, offsetY, controlWidth, SettingsRowHeight), label);

			// Not applicable
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(new Rect(controlWidth + 2, offsetY, controlWidth - 4, SettingsRowHeight), text);
			Text.Anchor = TextAnchor.MiddleLeft;
		}
		

		protected bool CreateCheckbox(
			float offsetY,
			float viewWidth,
			string label,
			string tooltip,
			bool value,
			bool defaultValue,
			string text = null)
		{
			var controlWidth = GetControlWidth(viewWidth);
			var isModified = value != defaultValue;

			// Label
			if (isModified)
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(0, offsetY, controlWidth, SettingsRowHeight), label);
			GUI.color = OriColor;

			// Setting
			var checkboxSize = SettingsRowHeight - 8;
			Widgets.Checkbox(controlWidth, offsetY + (SettingsRowHeight - checkboxSize) / 2, ref value, checkboxSize);
			DrawTooltip(new Rect(controlWidth, offsetY, checkboxSize, checkboxSize), tooltip);

			// Text
			if (text != null)
			{
				Text.Anchor = TextAnchor.MiddleCenter;
				Widgets.Label(new Rect(controlWidth + checkboxSize + 4, offsetY + 4, controlWidth - checkboxSize - 6, SettingsRowHeight - 8), $"({text})");
				Text.Anchor = TextAnchor.MiddleLeft;
			}

			// Reset button
			if (isModified && DrawResetButton(offsetY, viewWidth, defaultValue.ToString()))
				value = defaultValue;

			return value;
		}


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
			ConvertDelegate convert = null,
			string unit = null)
		{
			var controlWidth = GetControlWidth(viewWidth);

			// Label
			if (isModified)
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(0, offsetY, controlWidth, SettingsRowHeight), label);
			GUI.color = OriColor;

			// Setting
			var textFieldRect = new Rect(controlWidth + 2, offsetY + 6, controlWidth - 4, SettingsRowHeight - 12);
			Widgets.TextFieldNumeric(textFieldRect, ref value, ref ValueBuffer, min, max);
			DrawTooltip(textFieldRect, tooltip);

			// Unit
			DrawTextFieldUnit(textFieldRect, Convert(convert, value), unit);

			// Reset button
			if (isModified && DrawResetButton(offsetY, viewWidth, defaultValue.ToString()))
			{
				value = defaultValue;
				ValueBuffer = null;
			}

			return value;
		}

		protected (bool, float) CreateNumericWithCheckbox(
			float offsetY,
			float viewWidth,
			string label,
			string labelDisabled,
			string tooltip,
			string tooltipCheckbox,
			bool isModified,
			float value,
			float defaultValue,
			bool checkboxValue,
			bool defaultCheckboxValue,
			float min = 0f,
			float max = 1e+9f,
			ConvertDelegate convert = null,
			string unit = null)
		{
			var controlWidth = GetControlWidth(viewWidth);

			// Label
			if (isModified)
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(0, offsetY, controlWidth, SettingsRowHeight), label);
			GUI.color = OriColor;

			// Setting
			var checkboxSize = SettingsRowHeight - 8;
			Widgets.Checkbox(controlWidth, offsetY + (SettingsRowHeight - checkboxSize) / 2, ref checkboxValue, checkboxSize);
			DrawTooltip(new Rect(controlWidth, offsetY, checkboxSize, checkboxSize), tooltipCheckbox);

			// Value
			float offsetX = controlWidth + checkboxSize + 4;
			float width = controlWidth - checkboxSize - 6;
			if (checkboxValue)
			{
				var textFieldRect = new Rect(offsetX, offsetY + 6, width, SettingsRowHeight - 12);
				Widgets.TextFieldNumeric(textFieldRect, ref value, ref ValueBuffer, min, max);
				DrawTooltip(textFieldRect, tooltip);

				// Unit
				DrawTextFieldUnit(textFieldRect, Convert(convert, value), unit);
			}
			else
			{
				// Label when disabled
				if (labelDisabled != null)
				{
					Text.Anchor = TextAnchor.MiddleCenter;
					Widgets.Label(new Rect(offsetX, offsetY + 4, width, SettingsRowHeight - 8), $"({labelDisabled})");
					Text.Anchor = TextAnchor.MiddleLeft;
				}
			}

			// Reset button
			if (isModified && DrawResetButton(offsetY, viewWidth, defaultCheckboxValue + "/" + defaultValue.ToString()))
			{
				value = defaultValue;
				checkboxValue = defaultCheckboxValue;
				ValueBuffer = null;
			}

			return (checkboxValue, value);
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
			ConvertDelegate convert = null,
			string unit = null)
		{
			var controlWidth = GetControlWidth(viewWidth);

			// Label
			if (isModified)
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(0, offsetY, controlWidth, SettingsRowHeight), label);
			GUI.color = OriColor;

			// Setting
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
				value = val;

				// Unit
				DrawTextFieldUnit(textFieldRect, Convert(convert, value), unit);
			}
			else
			{
				// Label when disabled
				if (labelDisabled != null)
				{
					Text.Anchor = TextAnchor.MiddleCenter;
					Widgets.Label(new Rect(offsetX, offsetY + 4, width, SettingsRowHeight - 8), $"({labelDisabled})");
					Text.Anchor = TextAnchor.MiddleLeft;
				}

				value = null;
			}

			// Reset button
			if (isModified && DrawResetButton(offsetY, viewWidth, defaultValue?.ToString() ?? "null"))
			{
				value = defaultValue;
				ValueBuffer = null;
			}

			// Output
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
			float max = 1e+9f,
			ConvertDelegate convert = null,
			string unit = null)
		{
			var controlWidth = GetControlWidth(viewWidth);
			var oriOffsetY = offsetY;

			// Label
			if (use)
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(0, offsetY, controlWidth, SettingsDoubleRowHeight), label);
			GUI.color = OriColor;

			var textFieldLabelWidth = 28;
			var textFieldWidth = controlWidth - textFieldLabelWidth - 2;
			var offsetX = controlWidth;
			offsetY += 6;
			var height = SettingsRowHeight - 12;

			// Minimum setting
			Widgets.Label(new Rect(offsetX, offsetY, textFieldLabelWidth, height), "min");
			offsetX += textFieldLabelWidth;
			var textFieldRect = new Rect(offsetX + 2, offsetY, textFieldWidth - 4, height);
			Widgets.TextFieldNumeric(textFieldRect, ref minValue, ref MinValueBuffer, min, max);
			DrawTooltip(textFieldRect, tooltipMin);

			// Unit
			DrawTextFieldUnit(textFieldRect, Convert(convert, minValue), unit);
			
			offsetX = controlWidth;
			offsetY += SettingsDoubleRowHeight - SettingsRowHeight;

			// Maximum setting
			Widgets.Label(new Rect(offsetX, offsetY, textFieldLabelWidth, height), "max");
			offsetX += textFieldLabelWidth;
			textFieldRect = new Rect(offsetX + 2, offsetY, textFieldWidth - 4, height);
			Widgets.TextFieldNumeric(textFieldRect, ref maxValue, ref MaxValueBuffer, min, max);
			DrawTooltip(textFieldRect, tooltipMax);

			// Unit
			DrawTextFieldUnit(textFieldRect, Convert(convert, maxValue), unit);

			// "Apply" checkbox
			use = DrawUseGlobalCheckBox(oriOffsetY, viewWidth, use, SettingsDoubleRowHeight);

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
			ConvertDelegate convert = null,
			string unit = null)
		{
			var controlWidth = GetControlWidth(viewWidth);

			// Label
			// Switch color if modified
			if (use)
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(0, offsetY, controlWidth, SettingsRowHeight), label);
			GUI.color = OriColor;

			// Setting
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
				DrawTextFieldUnit(textFieldRect, Convert(convert, value), unit);

				value = val;
			}
			else
			{
				// Label when disabled
				if (labelDisabled != null)
				{
					Text.Anchor = TextAnchor.MiddleCenter;
					Widgets.Label(new Rect(offsetX, offsetY + 4, width, SettingsRowHeight - 8), $"{labelDisabled}");
					Text.Anchor = TextAnchor.MiddleLeft;
				}

				value = null;
			}

			// "Apply" checkbox
			use = DrawUseGlobalCheckBox(offsetY, viewWidth, use, SettingsRowHeight);

			// Output
			return (use, value);
		}
		#endregion
	}
}
