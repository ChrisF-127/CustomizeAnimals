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
	internal delegate T ConvertDelegate<T>(T value);

	internal abstract class BaseControl
	{
		#region PROPERTIES
		protected static float SettingsViewHeight => CustomizeAnimals.SettingsViewHeight;
		protected static float SettingsRowHeight => CustomizeAnimals.SettingsRowHeight;
		protected static float SettingsThinRowHeight => CustomizeAnimals.SettingsThinRowHeight;
		protected static float SettingsDoubleRowHeight => CustomizeAnimals.SettingsDoubleRowHeight;
		protected static float SettingsTripleRowHeight => CustomizeAnimals.SettingsTripleRowHeight;
		protected static float SettingsIconSize => CustomizeAnimals.SettingsIconSize;
		protected static float SettingsOffsetY => CustomizeAnimals.SettingsOffsetY;

		protected static GameFont OriTextFont => CustomizeAnimals.OriTextFont;
		protected static TextAnchor OriTextAnchor => CustomizeAnimals.OriTextAnchor;
		protected static Color OriColor => CustomizeAnimals.OriColor;
		protected static Color ModifiedColor => CustomizeAnimals.ModifiedColor;
		#endregion

		#region CLASS METHODS
		public virtual float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings) =>
			0f;
		public virtual float CreateSettingGlobal(float offsetY, float viewWidth) =>
			0f;
		public virtual void Reset() 
		{ }
		#endregion

		#region HELPER METHODS
		public static float GetControlWidth(float viewWidth) =>
			viewWidth / 2 - SettingsRowHeight - 4;

		public static float? ConvertToPercent(float? value) =>
			value * 100f;
		public static float ConvertToPercent(float value) =>
			value * 100f;

		public static float ConvertYearToDays(float value) =>
			value * 60f;

		public static string ListToString<T>(string seperator, IEnumerable<T> list, Func<T, string> converter)
		{
			string output = "";
			for (int i = 0; i < list.Count(); i++)
			{
				if (i > 0)
					output += seperator;
				output += converter(list.ElementAt(i));
			}
			return output;
		}
		#endregion

		#region CONTROLS METHODS
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
			DrawTooltip(new Rect(offsetX, offsetY + (height - checkboxSize) / 2, checkboxSize, checkboxSize), "SY_CA.TooltipUseGlobal".Translate());

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
		public static void DrawTextFieldUnit<T>(Rect rect, T value, string unit)
		{
			Text.Anchor = TextAnchor.MiddleRight;
			Widgets.Label(new Rect(rect.x + 4, rect.y + 1, rect.width - 8, rect.height), $"{value?.ToString() ?? ""} {unit ?? ""}");
			Text.Anchor = TextAnchor.MiddleLeft;
		}


		protected static void CreateText(
			float offsetY,
			float viewWidth,
			string label,
			string text,
			string textTooltip = null)
		{
			var controlWidth = GetControlWidth(viewWidth);

			// Label
			Widgets.Label(new Rect(0, offsetY, controlWidth, SettingsRowHeight), label);

			// Text
			var rect = new Rect(controlWidth + 2, offsetY, controlWidth - 4, SettingsRowHeight);
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(rect, text);
			Text.Anchor = TextAnchor.MiddleLeft;

			// Tooltip
			if (textTooltip != null)
				DrawTooltip(rect, textTooltip);
		}

		protected static bool CreateCheckbox(
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

		protected static void CreateDropDownEnum<T>(
			float offsetY,
			float viewWidth,
			string label,
			string tooltip,
			BaseSetting<T> setting,
			Func<T, string> menuTooltipGenerator = null)
			where T : Enum
		{
			if (setting.Value == null || setting.DefaultValue == null)
				return;

			var controlWidth = GetControlWidth(viewWidth);
			var isModified = !setting.Value.Equals(setting.DefaultValue);

			T getPayload(BaseSetting<T> target) => target.Value;
			IEnumerable<Widgets.DropdownMenuElement<T>> menuGenerator(BaseSetting<T> target)
			{
				foreach (var e in Enum.GetValues(typeof(T)).Cast<T>())
				{
					yield return new Widgets.DropdownMenuElement<T>
					{
						option = new FloatMenuOption(e.ToString(), () => target.Value = e) { tooltip = menuTooltipGenerator != null ? new TipSignal(menuTooltipGenerator(e)) : null },
						payload = e,
					};
				}
			}

			// Label
			if (isModified)
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(0, offsetY, controlWidth, SettingsRowHeight), label);
			GUI.color = OriColor;

			// Setting
			var rect = new Rect(controlWidth + 2, offsetY + 2, controlWidth - 4, SettingsRowHeight - 4);
			Widgets.Dropdown(rect, setting, getPayload, menuGenerator, setting.Value.ToString());
			DrawTooltip(rect, tooltip);

			// Reset button
			if (isModified && DrawResetButton(offsetY, viewWidth, setting.DefaultValue.ToString()))
				setting.Value = setting.DefaultValue;
		}

		protected void CreateDropdownSelectorControl<T>(
			float offsetY,
			float viewWidth,
			string label,
			string tooltip,
			bool isModified,
			TargetWrapper<T> valueWrapper,
			T DefaultValue,
			IEnumerable<T> list,
			Func<T, string> itemToString)
		{
			var controlWidth = GetControlWidth(viewWidth);

			// Label
			if (isModified)
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(0, offsetY, controlWidth, SettingsRowHeight), label);
			GUI.color = OriColor;

			// Menu Generator
			IEnumerable<Widgets.DropdownMenuElement<T>> menuGenerator(TargetWrapper<T> target)
			{
				foreach (var item in list)
				{
					yield return new Widgets.DropdownMenuElement<T>
					{
						option = new FloatMenuOption(itemToString(item), () => target.Item = item),
						payload = item,
					};
				}
			}

			// Dropdown
			var rect = new Rect(controlWidth + 2, offsetY + 2, controlWidth - 4, SettingsRowHeight - 4);
			Widgets.Dropdown(
				rect,
				valueWrapper,
				null,
				menuGenerator,
				itemToString(valueWrapper.Item));
			DrawTooltip(rect, tooltip);

			// Reset
			if (isModified && DrawResetButton(offsetY, viewWidth, itemToString(DefaultValue)))
				valueWrapper.Item = DefaultValue;
		}

		protected T CreateNumeric<T>(
			float offsetY,
			float viewWidth,
			string label,
			string tooltip,
			bool isModified,
			T value,
			T defaultValue,
			ref string valueBuffer,
			float min = 0f,
			float max = 1e+9f,
			ConvertDelegate<T> convert = null,
			string unit = null) 
			where T : struct
		{
			var controlWidth = GetControlWidth(viewWidth);

			// Label
			if (isModified)
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(0, offsetY, controlWidth - 8, SettingsRowHeight), label);
			GUI.color = OriColor;

			// Setting
			var textFieldRect = new Rect(controlWidth + 2, offsetY + 6, controlWidth - 4, SettingsRowHeight - 12);
			Widgets.TextFieldNumeric(textFieldRect, ref value, ref valueBuffer, min, max);
			if (!string.IsNullOrWhiteSpace(tooltip))
				DrawTooltip(textFieldRect, tooltip);

			// Unit
			DrawTextFieldUnit(textFieldRect, convert != null ? (T?)convert(value) : null, unit);

			// Reset button
			if (isModified && DrawResetButton(offsetY, viewWidth, defaultValue.ToString()))
			{
				value = defaultValue;
				valueBuffer = null;
			}

			return value;
		}

		protected (bool, T) CreateNumericWithCheckbox<T>(
			float offsetY,
			float viewWidth,
			string label,
			string labelDisabled,
			string tooltip,
			string tooltipCheckbox,
			bool isModified,
			T value,
			T defaultValue,
			bool checkboxValue,
			bool defaultCheckboxValue,
			ref string valueBuffer,
			float min = 0f,
			float max = 1e+9f,
			ConvertDelegate<T> convert = null,
			string unit = null)
			where T : struct
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
				Widgets.TextFieldNumeric(textFieldRect, ref value, ref valueBuffer, min, max);
				DrawTooltip(textFieldRect, tooltip);

				// Unit
				DrawTextFieldUnit(textFieldRect, convert != null ? (T?)convert(value) : null, unit);
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
				valueBuffer = null;
			}

			return (checkboxValue, value);
		}

		protected T? CreateNullableNumeric<T>(
			float offsetY,
			float viewWidth,
			string label,
			string labelDisabled,
			string tooltip,
			string tooltipCheckbox,
			bool isModified,
			T? value,
			T? defaultValue,
			ref string valueBuffer,
			float min = 0f,
			float max = 1e+9f,
			ConvertDelegate<T?> convert = null,
			string unit = null)
			where T : struct
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
				var val = value ?? defaultValue ?? default;
				Widgets.TextFieldNumeric(textFieldRect, ref val, ref valueBuffer, min, max);
				DrawTooltip(textFieldRect, tooltip);
				value = val;

				// Unit
				DrawTextFieldUnit(textFieldRect, convert != null && value != null ? convert(value) : null, unit);
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
				valueBuffer = null;
			}

			// Output
			return value;
		}

		protected void CreateArraySetting<T>(
			ref float totalHeight,
			ref float viewWidth,
			string label,
			string tooltip,
			T[] array,
			T[] defaultArray,
			ref string[] buffer,
			int startingIndex = 0,
			string[] sublabels = null,
			ConvertDelegate<T> convert = null,
			string unit = null)
			where T : struct, IComparable
		{
			Widgets.Label(new Rect(16f, totalHeight, viewWidth, SettingsRowHeight), label);
			totalHeight += SettingsRowHeight;

			var length = Mathf.Min(array.Length, defaultArray.Length);
			if (buffer == null)
				buffer = new string[length];

			for (int i = startingIndex; i < length; i++)
			{
				Text.Anchor = TextAnchor.MiddleRight;
				array[i] = CreateNumeric(
					totalHeight,
					viewWidth,
					i < sublabels?.Length ? sublabels[i] : i.ToString(),
					tooltip,
					!array[i].Equals(defaultArray[i]),
					array[i],
					defaultArray[i],
					ref buffer[i],
					convert: convert,
					unit: unit);
				totalHeight += SettingsRowHeight;
			}
			Text.Anchor = TextAnchor.MiddleLeft;
		}


		protected (bool, T) CreateNumericGlobal<T>(
			float offsetY,
			float viewWidth,
			string label,
			string tooltip,
			bool use,
			T value,
			T defaultValue,
			ref string valueBuffer,
			float min = 0f,
			float max = 1e+9f,
			ConvertDelegate<T> convert = null,
			string unit = null)
			where T : struct
		{
			var controlWidth = GetControlWidth(viewWidth);

			// Label
			if (use)
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(0, offsetY, controlWidth, SettingsRowHeight), label);
			GUI.color = OriColor;

			// Setting
			var textFieldRect = new Rect(controlWidth + 2, offsetY + 6, controlWidth - 4, SettingsRowHeight - 12);
			Widgets.TextFieldNumeric(textFieldRect, ref value, ref valueBuffer, min, max);
			DrawTooltip(textFieldRect, tooltip);

			// Unit
			DrawTextFieldUnit(textFieldRect, convert != null ? (T?)convert(value) : null, unit);

			// "Apply" checkbox
			use = DrawUseGlobalCheckBox(offsetY, viewWidth, use, SettingsRowHeight);

			// Output
			return (use, value);
		}

		protected (bool, T?) CreateNullableNumericGlobal<T>(
			float offsetY,
			float viewWidth,
			string label,
			string labelDisabled,
			string tooltip,
			string tooltipCheckbox,
			bool use,
			T? value,
			T defaultValue,
			ref string valueBuffer,
			float min = 0f,
			float max = 1e+9f,
			ConvertDelegate<T?> convert = null,
			string unit = null)
			where T : struct
		{
			var controlWidth = GetControlWidth(viewWidth);

			// Label
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
				var val = value ?? defaultValue;
				Widgets.TextFieldNumeric(textFieldRect, ref val, ref valueBuffer, min, max);
				DrawTooltip(textFieldRect, tooltip);

				// Unit
				DrawTextFieldUnit(textFieldRect, convert != null && value != null ? convert(value) : null, unit);

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

		protected (bool, T, T) CreateNumericGlobalMinMax<T>(
			float offsetY,
			float viewWidth,
			string label,
			string tooltipMin,
			string tooltipMax,
			bool use,
			T minValue,
			T maxValue,
			ref string minValueBuffer,
			ref string maxValueBuffer,
			float min = 0f,
			float max = 1e+9f,
			ConvertDelegate<T> convert = null,
			string unit = null)
			where T : struct
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
			Widgets.TextFieldNumeric(textFieldRect, ref minValue, ref minValueBuffer, min, max);
			DrawTooltip(textFieldRect, tooltipMin);

			// Unit
			DrawTextFieldUnit(textFieldRect, convert != null ? (T?)convert(minValue) : null, unit);

			offsetX = controlWidth;
			offsetY += SettingsDoubleRowHeight - SettingsRowHeight;

			// Maximum setting
			Widgets.Label(new Rect(offsetX, offsetY, textFieldLabelWidth, height), "max");
			offsetX += textFieldLabelWidth;
			textFieldRect = new Rect(offsetX + 2, offsetY, textFieldWidth - 4, height);
			Widgets.TextFieldNumeric(textFieldRect, ref maxValue, ref maxValueBuffer, min, max);
			DrawTooltip(textFieldRect, tooltipMax);

			// Unit
			DrawTextFieldUnit(textFieldRect, convert != null ? (T?)convert(maxValue) : null, unit);

			// "Apply" checkbox
			use = DrawUseGlobalCheckBox(oriOffsetY, viewWidth, use, SettingsDoubleRowHeight);

			// Output
			return (use, minValue, maxValue);
		}
		#endregion
	}


	internal abstract class BaseSettingControl : BaseControl
	{
		#region FIELDS
		protected string ValueBuffer = null;
		protected string MinValueBuffer = null;
		protected string MaxValueBuffer = null;
		#endregion

		#region METHODS
		public override void Reset()
		{
			ValueBuffer = null;
			MinValueBuffer = null;
			MaxValueBuffer = null;
		}


		protected T CreateNumeric<T>(
			float offsetY,
			float viewWidth,
			string label,
			string tooltip,
			bool isModified,
			T value,
			T defaultValue,
			float min = 0f,
			float max = 1e+9f,
			ConvertDelegate<T> convert = null,
			string unit = null)
			where T : struct =>
			CreateNumeric(
				offsetY, viewWidth, label, tooltip, 
				isModified, value, defaultValue, ref ValueBuffer, min, max, convert, unit);

		protected (bool, T) CreateNumericWithCheckbox<T>(
			float offsetY,
			float viewWidth,
			string label,
			string labelDisabled,
			string tooltip,
			string tooltipCheckbox,
			bool isModified,
			T value,
			T defaultValue,
			bool checkboxValue,
			bool defaultCheckboxValue,
			float min = 0f,
			float max = 1e+9f,
			ConvertDelegate<T> convert = null,
			string unit = null)
			where T : struct =>
			CreateNumericWithCheckbox(
				offsetY, viewWidth, label, labelDisabled, tooltip, tooltipCheckbox, 
				isModified, value, defaultValue, checkboxValue, defaultCheckboxValue, ref ValueBuffer, min, max, convert, unit);

		protected T? CreateNullableNumeric<T>(
			float offsetY,
			float viewWidth,
			string label,
			string labelDisabled,
			string tooltip,
			string tooltipCheckbox,
			bool isModified,
			T? value,
			T? defaultValue,
			float min = 0f,
			float max = 1e+9f,
			ConvertDelegate<T?> convert = null,
			string unit = null)
			where T : struct =>
			CreateNullableNumeric(
				offsetY, viewWidth, label, labelDisabled, tooltip, tooltipCheckbox,
				isModified, value, defaultValue, ref ValueBuffer, min, max, convert, unit);


		protected (bool, T) CreateNumericGlobal<T>(
			float offsetY,
			float viewWidth,
			string label,
			string tooltip,
			bool use,
			T value,
			T defaultValue,
			float min = 0f,
			float max = 1e+9f,
			ConvertDelegate<T> convert = null,
			string unit = null)
			where T : struct =>
			CreateNumericGlobal(
				offsetY, viewWidth, label, tooltip, 
				use, value, defaultValue, ref ValueBuffer, min, max, convert, unit);

		protected (bool, T?) CreateNullableNumericGlobal<T>(
			float offsetY,
			float viewWidth,
			string label,
			string labelDisabled,
			string tooltip,
			string tooltipCheckbox,
			bool use,
			T? value,
			T defaultValue,
			float min = 0f,
			float max = 1e+9f,
			ConvertDelegate<T?> convert = null,
			string unit = null)
			where T : struct =>
			CreateNullableNumericGlobal(
				offsetY, viewWidth, label, labelDisabled, tooltip, tooltipCheckbox,
				use, value, defaultValue, ref ValueBuffer, min, max, convert, unit);

		protected (bool, T, T) CreateNumericGlobalMinMax<T>(
			float offsetY,
			float viewWidth,
			string label,
			string tooltipMin,
			string tooltipMax,
			bool use,
			T minValue,
			T maxValue,
			float min = 0f,
			float max = 1e+9f,
			ConvertDelegate<T> convert = null,
			string unit = null)
			where T : struct =>
			CreateNumericGlobalMinMax(
				offsetY, viewWidth, label, tooltipMin, tooltipMax,
				use, minValue, maxValue, ref MinValueBuffer, ref MaxValueBuffer, min, max, convert, unit);
		#endregion
	}

	internal abstract class BaseSpecialSettingControl : BaseControl
	{
	}

	internal class TargetWrapper<T>
	{
		public T Item { get; set; }

		public TargetWrapper(T item)
		{
			Item = item;
		}
	}
}
