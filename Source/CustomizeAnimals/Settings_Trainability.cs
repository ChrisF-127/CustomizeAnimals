using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CustomizeAnimals
{
	internal class Settings_Trainability : Settings_Base
	{
		public override float Create(float offsetY, float viewWidth, AnimalSettings animal)
		{
			float controlWidth = GetControlWidth(viewWidth);
			var trainabilitySetting = animal.Trainability;
			var roamMtbDaysSetting = animal.RoamMtbDays;

			// Switch color if modified
			var labelRect = new Rect(0, offsetY, controlWidth, SettingsRowHeight);
			if (trainabilitySetting.Value != TrainabilityDefOf.None && roamMtbDaysSetting.Value != null)
			{
				GUI.color = Color.red;
				DrawTooltip(labelRect, "SY_CA.TrainabilityRoamingWarning".Translate());
			}
			else if (trainabilitySetting.IsModified())
				GUI.color = ModifiedColor;

			// Label
			Widgets.Label(labelRect, "SY_CA.Trainability".Translate());
			GUI.color = OriColor;

			var trainability = trainabilitySetting.Value;
			var trainabilityOptionWidth = controlWidth / 3;
			var trainabilityOffsetY = offsetY + (SettingsRowHeight - 24) / 2;

			// None
			var trainabilityOffsetX = controlWidth + trainabilityOptionWidth * 0;
			if (Widgets.RadioButton(trainabilityOffsetX, trainabilityOffsetY, trainability == TrainabilityDefOf.None))
				trainability = TrainabilityDefOf.None;
			Widgets.Label(new Rect(trainabilityOffsetX + 30, offsetY, trainabilityOptionWidth, SettingsRowHeight), "SY_CA.TrainabilityNone".Translate());
			DrawTooltip(new Rect(trainabilityOffsetX, offsetY, trainabilityOptionWidth, SettingsRowHeight), "SY_CA.TooltipTrainabilityNone".Translate());

			// Intermediate
			trainabilityOffsetX = controlWidth + trainabilityOptionWidth * 1;
			if (Widgets.RadioButton(trainabilityOffsetX, trainabilityOffsetY, trainability == TrainabilityDefOf.Intermediate))
				trainability = TrainabilityDefOf.Intermediate;
			Widgets.Label(new Rect(trainabilityOffsetX + 30, offsetY, trainabilityOptionWidth, SettingsRowHeight), "SY_CA.TrainabilityIntermediate".Translate());
			DrawTooltip(new Rect(trainabilityOffsetX, offsetY, trainabilityOptionWidth, SettingsRowHeight), "SY_CA.TooltipTrainabilityIntermediate".Translate());

			// Advanced
			trainabilityOffsetX = controlWidth + trainabilityOptionWidth * 2;
			if (Widgets.RadioButton(trainabilityOffsetX, trainabilityOffsetY, trainability == TrainabilityDefOf.Advanced))
				trainability = TrainabilityDefOf.Advanced;
			Widgets.Label(new Rect(trainabilityOffsetX + 30, offsetY, trainabilityOptionWidth, SettingsRowHeight), "SY_CA.TrainabilityAdvanced".Translate());
			DrawTooltip(new Rect(trainabilityOffsetX, offsetY, trainabilityOptionWidth, SettingsRowHeight), "SY_CA.TooltipTrainabilityAdvanced".Translate());

			// Reset button
			if (trainabilitySetting.IsModified() && DrawResetButton(offsetY, viewWidth, trainabilitySetting.DefaultValue.ToString()))
				trainabilitySetting.Reset();
			// Set Trainability
			else
				trainabilitySetting.Value = trainability;

			return SettingsRowHeight;
		}

	}
}
