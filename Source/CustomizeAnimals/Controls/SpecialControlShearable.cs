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
	internal class SpecialControlShearable : BaseSpecialSettingControl
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

			var setting = (SpecialSettingShearable)animalSettings.ProductivitySettings["Shearable"];
			if (ValueWrapper == null)
				ValueWrapper = new TargetWrapper<ThingDef>(setting.WoolDef);

			var totalHeight = offsetY;

			// Separator
			Widgets.ListSeparator(ref totalHeight, viewWidth - 16, "SY_CA.SeparatorShearable".Translate());
			totalHeight += 2;
			Text.Anchor = TextAnchor.MiddleLeft;

			// Is Shearable
			setting.IsShearable = CreateCheckbox(
				totalHeight,
				viewWidth,
				"SY_CA.ShearableIsShearable".Translate(),
				"SY_CA.TooltipShearableIsShearable".Translate(),
				setting.IsShearable,
				setting.DefaultIsShearable,
				"SY_CA.ShearableIsShearableText".Translate());
			totalHeight += SettingsRowHeight;

			if (!setting.IsShearable)
				return totalHeight - offsetY;

			// Wool Def
			CreateDropdownSelectorControl(
				totalHeight,
				viewWidth,
				"SY_CA.ShearableDef".Translate(),
				"SY_CA.TooltipShearableDef".Translate(),
				setting.WoolDef != setting.DefaultWoolDef,
				ValueWrapper,
				setting.DefaultWoolDef,
				GetAllowedWoolDefs(setting.WoolDef),
				(def) => def?.label?.CapitalizeFirst() ?? "SY_CA.ShearableNoDef".Translate());
			setting.WoolDef = ValueWrapper.Item;
			totalHeight += SettingsRowHeight;

			// Interval Days
			setting.IntervalDays = CreateNumeric(
				totalHeight,
				viewWidth,
				"SY_CA.ShearableIntervalDays".Translate(),
				"SY_CA.TooltipShearableIntervalDays".Translate(),
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
				"SY_CA.ShearableAmount".Translate(),
				"SY_CA.TooltipShearableAmount".Translate(),
				setting.Amount != setting.DefaultAmount,
				setting.Amount,
				setting.DefaultAmount,
				ref AmountBuffer);
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
		private IEnumerable<ThingDef> GetAllowedWoolDefs(params ThingDef[] excludes)
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
