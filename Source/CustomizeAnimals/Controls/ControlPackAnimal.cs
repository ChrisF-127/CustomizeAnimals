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
	internal class ControlPackAnimal : BaseSettingControl
	{
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			if (animalSettings.IsHuman)
				return 0f;

			var setting = (SettingPackAnimal)animalSettings.GeneralSettings["PackAnimal"];
			var value = CreateCheckbox(
				offsetY,
				viewWidth,
				"SY_CA.PackAnimal".Translate(),
				"SY_CA.TooltipPackAnimal".Translate(),
				setting.Value,
				setting.DefaultValue,
				$"{"SY_CA.PackAnimalMassCapacity".Translate()}: {setting.GetCaravanMassCapacity()} kg");

			setting.Value = value;

			return SettingsRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth) => 0f;
	}
}
