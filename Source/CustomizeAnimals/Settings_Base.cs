using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CustomizeAnimals
{
	internal abstract class Settings_Base
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

		#region PUBLIC METHODS
		public abstract float Create(float offsetY, float viewWidt, AnimalSettings animal);
		
		public static bool DrawResetButton(float offsetY, float viewWidth, string tooltip)
		{
			var buttonRect = new Rect(viewWidth - SettingsRowHeight * 2 + 2, offsetY + 2, SettingsRowHeight * 2 - 4, SettingsRowHeight - 4);
			var output = Widgets.ButtonText(buttonRect, "SY_CA.Reset".Translate());

			DrawTooltip(buttonRect, "SY_CA.TooltipDefaultValue".Translate() + " " + tooltip);
			return output;
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
		#endregion
	}
}
