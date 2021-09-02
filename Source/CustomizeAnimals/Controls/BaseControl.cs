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
			float max = 1e+9f,
			ConvertDelegate to = null,
			ConvertDelegate back = null,
			string unit = null)
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

			DrawTextFieldUnit(textFieldRect, unit);

			// Reset button
			if (isModified && DrawResetButton(offsetY, viewWidth, defaulValue.ToString()))
			{
				value = defaulValue;
				ValueBuffer = null;
			}
			
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
			ConvertDelegate to = null,
			ConvertDelegate back = null,
			string unit = null)
		{
			var oriMinValue = minValue;
			var oriMaxValue = maxValue;
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
		#endregion
	}
}
