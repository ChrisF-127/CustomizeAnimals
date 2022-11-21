using CustomizeAnimals.Controls;
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
	internal class GeneralSettingsControls : BaseControl
	{
		#region PROPERTIES
		private GeneralSettings Settings => GlobalSettings.GlobalGeneralSettings;
		#endregion

		#region FIELDS
		private string TrainingDecayFactorBuffer;
		private string EggMassFactorBuffer;
		private string EggNutritionFactorBuffer;
		#endregion

		#region PUBLIC METHODS
		public float CreateTrainabilityLimitsControls(float offsetY, float viewWidth)
		{
			var controlWidth = GetControlWidth(viewWidth);
			var halfWidth = viewWidth / 2;
			var quarterWidth = halfWidth / 2 - 2;
			var checkboxSize = SettingsRowHeight - 8;
			var checkboxOffset = (SettingsRowHeight - checkboxSize) / 2;

			float startOffsetY = offsetY;
			float offsetX = 0;
			float value = 0f;


			// Training Decay Limits Label
			if (Settings.IsTrainingDecayFactorModified)
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(offsetX, offsetY, controlWidth, SettingsRowHeight), "SY_CA.TrainingDecay".Translate());
			GUI.color = OriColor;

			offsetX += controlWidth;

			// Settings
			var textFieldRect = new Rect(controlWidth + 2, offsetY + 6, controlWidth - 4, SettingsRowHeight - 12);
			value = Settings.TrainingDecayFactor;
			Widgets.TextFieldNumeric(textFieldRect, ref value, ref TrainingDecayFactorBuffer, 0f, 1e3f);
			DrawTooltip(textFieldRect, "SY_CA.TooltipTrainingDecay".Translate());

			// Reset button
			if (Settings.IsTrainingDecayFactorModified && DrawResetButton(offsetY, viewWidth, Settings.DefaultTrainingDecayFactor.ToString()))
			{
				value = Settings.DefaultTrainingDecayFactor;
				TrainingDecayFactorBuffer = null;
			}
			Settings.TrainingDecayFactor = value;


			// Next row
			offsetX = 0;
			offsetY += SettingsRowHeight;


			// Trainable Limits Label
			if (Settings.DisableHaulSizeLimit || Settings.DisableRescueSizeLimit)
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(offsetX, offsetY, controlWidth, SettingsRowHeight), "SY_CA.TrainableLimits".Translate());
			GUI.color = OriColor;

			offsetX += controlWidth;

			// Rescue: BodySize Limit
			var selected = Settings.DisableHaulSizeLimit;
			Widgets.Checkbox(offsetX, offsetY + checkboxOffset, ref selected, checkboxSize);
			DrawTooltip(new Rect(offsetX, offsetY, quarterWidth - 12, SettingsRowHeight), "SY_CA.TooltipRescueLimit".Translate());
			offsetX += checkboxSize + 2;
			Widgets.Label(new Rect(offsetX, offsetY, quarterWidth - checkboxSize - 12, SettingsRowHeight), "SY_CA.RescueLimit".Translate());
			Settings.DisableHaulSizeLimit = selected;

			offsetX = controlWidth + quarterWidth + 2;

			// Haul: BodySize Limit
			selected = Settings.DisableRescueSizeLimit;
			Widgets.Checkbox(offsetX, offsetY + checkboxOffset, ref selected, checkboxSize);
			DrawTooltip(new Rect(offsetX, offsetY, quarterWidth - 12, SettingsRowHeight), "SY_CA.TooltipHaulLimit".Translate());
			offsetX += checkboxSize + 2;
			Widgets.Label(new Rect(offsetX, offsetY, quarterWidth - checkboxSize - 12, SettingsRowHeight), "SY_CA.HaulLimit".Translate());
			Settings.DisableRescueSizeLimit = selected;


			// Next row
			offsetX = 0;
			offsetY += SettingsRowHeight;


			// Trainable Limits Label
			if (Settings.CarryingCapacityAffectsMassCapacity)
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(offsetX, offsetY, controlWidth, SettingsRowHeight), "SY_CA.CarryingCapacity".Translate());
			GUI.color = OriColor;

			offsetX += controlWidth;

			// Carrying Capacity affects Mass Capacity
			selected = Settings.CarryingCapacityAffectsMassCapacity;
			Widgets.Checkbox(offsetX, offsetY + checkboxOffset, ref selected, checkboxSize);
			DrawTooltip(new Rect(offsetX, offsetY, controlWidth, SettingsRowHeight), "SY_CA.TooltipAffectsMassCapacity".Translate());
			offsetX += checkboxSize + 2;
			Widgets.Label(new Rect(offsetX, offsetY, controlWidth, SettingsRowHeight), "SY_CA.AffectsMassCapacity".Translate());
			Settings.CarryingCapacityAffectsMassCapacity = selected;


			// Next row
			offsetX = 0;
			offsetY += SettingsRowHeight;

			return offsetY - startOffsetY;
		}

		public float CreateEggSettings(float offsetY, float viewWidth)
		{
			float totalHeight = offsetY;

			// Egg mass affected by body size
			(var useMass, var valueMass) = CreateNumericGlobal(
				totalHeight,
				viewWidth,
				"SY_CA.EggBodySizeMass".Translate(),
				"SY_CA.TooltipEggBodySizeMass".Translate(),
				Settings.EggMassDependOnBodySize,
				Settings.EggMassFactor,
				GeneralSettings.DefaultEggMassFactor,
				ref EggMassFactorBuffer,
				min: 1e-2f,
				max: 1e2f);
			Settings.EggMassDependOnBodySize = useMass;
			Settings.EggMassFactor = valueMass;
			totalHeight += SettingsDoubleRowHeight - SettingsRowHeight;

			// Egg nutrition affected by body size
			(var useNutrition, var valueNutrition) = CreateNumericGlobal(
				totalHeight,
				viewWidth,
				"SY_CA.EggBodySizeNutrition".Translate(),
				"SY_CA.TooltipEggBodySizeNutrition".Translate(),
				Settings.EggNutritionDependOnBodySize,
				Settings.EggNutritionFactor,
				GeneralSettings.DefaultEggNutritionFactor,
				ref EggNutritionFactorBuffer,
				min: 1e-2f,
				max: 1e2f);
			Settings.EggNutritionDependOnBodySize = useNutrition;
			Settings.EggNutritionFactor = valueNutrition;

			return SettingsDoubleRowHeight;
		}
		#endregion
	}
}
