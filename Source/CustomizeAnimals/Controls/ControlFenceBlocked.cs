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
	internal class ControlFenceBlocked : BaseSettingControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			if (animalSettings.IsHumanLike)
				return 0f;

			var fenceBlocked = (BaseSetting<bool?>)animalSettings.GeneralSettings["FenceBlocked"];

			var value = CreateNullableCheckbox(
				offsetY,
				viewWidth,
				"SY_CA.FenceBlocked".Translate(),
				"SY_CA.TooltipFenceBlocked".Translate(),
				fenceBlocked.Value,
				fenceBlocked.DefaultValue);

			fenceBlocked.Value = value;

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth)
		{
			var always = SettingFenceBlocked.Always;
			var notWhenDrafted = SettingFenceBlocked.NotWhenFollowing;

			var controlWidth = GetControlWidth(viewWidth);
			var quarterWidth = viewWidth * 0.25f - 2;
			var checkboxSize = SettingsRowHeight - 8;
			var checkboxOffset = (SettingsRowHeight - checkboxSize) / 2;
			var offsetX = 0f;

			// Label
			if (always || notWhenDrafted)
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(offsetX, offsetY, controlWidth, SettingsRowHeight), "SY_CA.FenceBlocked".Translate());
			GUI.color = OriColor;

			offsetX += controlWidth;

			// Always
			Widgets.Checkbox(offsetX, offsetY + checkboxOffset, ref always, checkboxSize);
			DrawTooltip(new Rect(offsetX, offsetY, quarterWidth - 12, SettingsRowHeight), "SY_CA.TooltipFenceBlockedAlways".Translate());
			offsetX += checkboxSize + 2;
			Widgets.Label(new Rect(offsetX, offsetY, quarterWidth - checkboxSize - 12, SettingsRowHeight), "SY_CA.FenceBlockedAlways".Translate());
			SettingFenceBlocked.Always = always;

			offsetX = controlWidth + quarterWidth + 2;

			// Not when drafted
			Widgets.Checkbox(offsetX, offsetY + checkboxOffset, ref notWhenDrafted, checkboxSize);
			DrawTooltip(new Rect(offsetX, offsetY, quarterWidth - 12, SettingsRowHeight), "SY_CA.TooltipFenceBlockedNotWhenFollowing".Translate());
			offsetX += checkboxSize + 2;
			Widgets.Label(new Rect(offsetX, offsetY, quarterWidth - checkboxSize - 12, SettingsRowHeight), "SY_CA.FenceBlockedNotWhenFollowing".Translate());
			SettingFenceBlocked.NotWhenFollowing = notWhenDrafted;

			return SettingsRowHeight;
		}
	}
}
