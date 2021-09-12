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
	internal class ControlFoodType : BaseSettingControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (BaseSetting<FoodTypeFlags>)animalSettings.Settings["FoodType"];
			CreateDropDown(
				offsetY,
				viewWidth,
				"SY_CA.FoodType".Translate(),
				setting.Value.ToHumanString().CapitalizeFirst(),
				setting,
				(e) => e.ToHumanString().CapitalizeFirst());

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth) => 0f;
	}
}
