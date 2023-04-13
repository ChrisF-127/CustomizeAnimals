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
	internal class SpecialControlMilkable : BaseSpecialSettingControl
	{
		#region FIELDS
		public string IntervalDaysBuffer;
		public string AmountBuffer;

		public TargetWrapper<ThingDef> ValueWrapper;
		#endregion

		#region METHODS
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			if (animalSettings.IsHumanLike)
				return 0f;

			var setting = (SpecialSettingMilkable)animalSettings.ProductivitySettings["Milkable"];
			if (ValueWrapper == null)
				ValueWrapper = new TargetWrapper<ThingDef>(setting.MilkDef);

			var totalHeight = offsetY;

			// Separator
			Widgets.ListSeparator(ref totalHeight, viewWidth - 16, "SY_CA.SeparatorMilkable".Translate());
			totalHeight += 2;
			Text.Anchor = TextAnchor.MiddleLeft;

			// Is Milkable
			setting.IsMilkable = CreateCheckbox(
				totalHeight,
				viewWidth,
				"SY_CA.MilkableIsMilkable".Translate(),
				"SY_CA.TooltipMilkableIsMilkable".Translate(),
				setting.IsMilkable,
				setting.DefaultIsMilkable,
				"SY_CA.MilkableIsMilkableText".Translate());
			totalHeight += SettingsRowHeight;

			if (!setting.IsMilkable)
				return totalHeight - offsetY;

			// Milk Def
			CreateDropdownSelectorControl(
				totalHeight,
				viewWidth,
				"SY_CA.MilkableDef".Translate(),
				"SY_CA.TooltipMilkableDef".Translate(),
				setting.MilkDef != setting.DefaultMilkDef,
				ValueWrapper,
				setting.DefaultMilkDef,
				GetAllowedMilkDefs(setting.MilkDef),
				(def) => def?.label?.CapitalizeFirst() ?? "SY_CA.MilkableNoDef".Translate());
			setting.MilkDef = ValueWrapper.Item;
			totalHeight += SettingsRowHeight;

			// Interval Days
			setting.IntervalDays = CreateNumeric(
				totalHeight,
				viewWidth,
				"SY_CA.MilkableIntervalDays".Translate(),
				"SY_CA.TooltipMilkableIntervalDays".Translate(),
				setting.IntervalDays != setting.DefaultIntervalDays,
				setting.IntervalDays,
				setting.DefaultIntervalDays,
				ref IntervalDaysBuffer,
				unit: "d");
			totalHeight += SettingsRowHeight;

			// Amount
			setting.Amount = CreateNumeric(
				totalHeight,
				viewWidth,
				"SY_CA.MilkableAmount".Translate(),
				"SY_CA.TooltipMilkableAmount".Translate(),
				setting.Amount != setting.DefaultAmount,
				setting.Amount,
				setting.DefaultAmount,
				ref AmountBuffer);
			totalHeight += SettingsRowHeight;
			
			// Female Only
			setting.FemaleOnly = CreateCheckbox(
				totalHeight,
				viewWidth,
				"SY_CA.MilkableFemaleOnly".Translate(),
				"SY_CA.TooltipMilkableFemaleOnly".Translate(),
				setting.FemaleOnly,
				setting.DefaultFemaleOnly,
				"SY_CA.MilkableFemaleOnlyText".Translate());
			totalHeight += SettingsRowHeight;

			return totalHeight - offsetY;
		}

		public override void Reset()
		{
			IntervalDaysBuffer = null;
			AmountBuffer = null;

			ValueWrapper = null;
		}
		#endregion

		#region PRIVATE METHODS
		private IEnumerable<ThingDef> GetAllowedMilkDefs(params ThingDef[] excludes)
		{
			var list = DefDatabase<ThingDef>.AllDefs.Where(
				(def) => 
				def.category == ThingCategory.Item
				&& def.CountAsResource == true
				&& def.MadeFromStuff == false
				).ToList();
			list.SortBy((def) => def.label ?? "");
			return list;
		}
		#endregion
	}
}
