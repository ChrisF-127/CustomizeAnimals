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
	internal class SpecialControlAttackTools : BaseSpecialSettingControl
	{
		#region PROPERTIES
		private List<ControlAttackTool> AttackToolControls { get; } = new List<ControlAttackTool>();
		#endregion

		#region METHODS
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (SpecialSettingAttackTools)animalSettings.SpecialSettings["AttackTools"];

			if (AttackToolControls.Count == 0)
				foreach (var tool in setting.Tools)
					AttackToolControls.Add(new ControlAttackTool(tool));

			var totalHeight = offsetY;
			foreach (var control in AttackToolControls)
			{
				var height = control.CreateSetting(offsetY, viewWidth);
				offsetY += height;
				totalHeight += height;
			}
			return totalHeight;
		}

		public override void Reset()
		{
			AttackToolControls.Clear();
		}
		#endregion

		#region CLASSES
		private class ControlAttackTool : BaseControl
		{
			#region PROPERTIES
			public AttackTool Tool { get; }
			#endregion

			#region FIELDS
			protected string PowerBuffer;
			protected string CooldownBuffer;
			protected string ArmorPenetrationBuffer;
			protected string ChanceFactorBuffer;
			#endregion

			#region CONSTRUCTORS
			public ControlAttackTool(AttackTool tool)
			{
				Tool = tool;
			}
			#endregion

			#region METHODS
			public float CreateSetting(float offsetY, float viewWidth)
			{
				if (Tool == null)
					return 0f;

				var oriOffsetY = offsetY;

				// Separator
				Widgets.ListSeparator(ref offsetY, viewWidth - 16, Tool.Label?.CapitalizeFirst() ?? "[no_label]");
				offsetY += 2;
				Text.Anchor = TextAnchor.MiddleLeft;

				// Power
				Tool.Power = CreateNumeric(
					offsetY, 
					viewWidth, 
					"SY_CA.AttackToolsPower".Translate(), 
					"SY_CA.TooltipAttackToolsPower".Translate(), 
					Tool.Power != Tool.DefaultPower, 
					Tool.Power,
					Tool.DefaultPower,
					ref PowerBuffer,
					min: -1e9f);
				offsetY += SettingsRowHeight;
				// Cooldown
				Tool.Cooldown = CreateNumeric(
					offsetY,
					viewWidth,
					"SY_CA.AttackToolsCooldown".Translate(),
					"SY_CA.TooltipAttackToolsCooldown".Translate(),
					Tool.Cooldown != Tool.DefaultCooldown,
					Tool.Cooldown,
					Tool.DefaultCooldown,
					ref CooldownBuffer);
				offsetY += SettingsRowHeight;
				// Armor Penetration
				Tool.ArmorPenetration = CreateNumeric(
					offsetY,
					viewWidth,
					"SY_CA.AttackToolsArmorPenetration".Translate(),
					"SY_CA.TooltipAttackToolsArmorPenetration".Translate(),
					Tool.ArmorPenetration != Tool.DefaultArmorPenetration,
					Tool.ArmorPenetration,
					Tool.DefaultArmorPenetration,
					ref ArmorPenetrationBuffer,
					min: -1f);
				offsetY += SettingsRowHeight;
				// Cooldown
				Tool.ChanceFactor = CreateNumeric(
					offsetY,
					viewWidth,
					"SY_CA.AttackToolsChanceFactor".Translate(),
					"SY_CA.TooltipAttackToolsChanceFactor".Translate(),
					Tool.ChanceFactor != Tool.DefaultChanceFactor,
					Tool.ChanceFactor,
					Tool.DefaultChanceFactor,
					ref ChanceFactorBuffer);
				offsetY += SettingsRowHeight;

				// Linked Body Parts Group 
				CreateText(offsetY, viewWidth, "SY_CA.AttackToolsLinkedBodyPartsGroup".Translate(), Tool.LinkedBodyPartsGroup?.label?.CapitalizeFirst());
				offsetY += SettingsRowHeight;

				// Capacities
				CreateText(offsetY, viewWidth, "SY_CA.AttackToolsCapacities".Translate(), ListToString(", ", Tool.Capacities, (cap) => cap?.label?.CapitalizeFirst()));
				offsetY += SettingsRowHeight;

				return offsetY - oriOffsetY;
			}
			#endregion
		}
		#endregion
	}
}
