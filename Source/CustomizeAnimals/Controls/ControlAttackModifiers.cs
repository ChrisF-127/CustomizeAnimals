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
	internal class ControlAttackModifier : BaseControl
	{
		#region FIELDS
		private string PowerBuffer;
		private string CooldownBuffer;

		private float TotalDamage = float.NaN;
		private float TotalCooldown = float.NaN;
		#endregion

		#region PRIVATE METHODS
		private (float, string) CalculateDPS(ThingDef animal)
		{
			if (animal?.tools == null)
				return (float.NaN, null);

			List<Tool> tools = animal.tools;

			float highestInitialWeight = 0f;
			int catMidCount = 0, catBestCount = 0;
			List<DamageCalculation> allDPSList = new List<DamageCalculation>();

			// find highest initial weight
			foreach (var tool in tools)
			{
				float damage = tool.power;
				float cooldown = tool.cooldownTime;
				float armorPenetration = tool.armorPenetration;
				float chanceFactor = tool.chanceFactor;
				float initialWeight = damage * (1f + (armorPenetration < 0f ? damage * 0.015f : armorPenetration)) / cooldown * chanceFactor;
				highestInitialWeight = Math.Max(initialWeight, highestInitialWeight);
			}

			// save each tools details
			foreach (var tool in tools)
			{
				float damage = tool.power;
				float cooldown = tool.cooldownTime;
				float armorPenetration = tool.armorPenetration;
				float chanceFactor = tool.chanceFactor;
				float initialWeight = damage * (1f + (armorPenetration < 0f ? damage * 0.015f : armorPenetration)) / cooldown * chanceFactor;

				int cat;
				// worst
				if (initialWeight < highestInitialWeight * 0.25f)
					continue;
				// mid
				else if (initialWeight < highestInitialWeight * 0.95f)
				{
					cat = 1;
					catMidCount++;
				}
				// best
				else
				{
					cat = 2;
					catBestCount++;
				}

				allDPSList.Add(new DamageCalculation
				{
					Damage = damage,
					Cooldown = cooldown,
					ArmorPenetration = armorPenetration,
					ChanceFactor = chanceFactor,
					InitialWeight = initialWeight,
					Cat = cat,
				});
			}

			// calculate weighting factor
			float factorCatMid = 1f / catMidCount * 0.25f;
			float factorCatBest = 1f / catBestCount * 0.75f;
			float factorCatTotal = 0f;
			foreach (var dps in allDPSList)
			{
				switch (dps.Cat)
				{
					case 1:
						dps.FactorCat = factorCatMid;
						break;
					case 2:
						dps.FactorCat = factorCatBest;
						break;
					default:
						continue;
				}
				factorCatTotal += dps.FactorCat;
			}

			float totalDamage = 0, totalCooldown = 0;
			foreach (var dps in allDPSList)
			{
				totalDamage += dps.FactorCat / factorCatTotal * dps.Damage;
				totalCooldown += dps.FactorCat / factorCatTotal * dps.Cooldown;
			}

			TotalDamage = totalDamage;
			TotalCooldown = totalCooldown;

			var manipulation = animal?.race?.body?.HasPartWithTag(BodyPartTagDefOf.ManipulationLimbCore) == true;
			var output = totalDamage / totalCooldown * StatDefOf.MeleeHitChance.postProcessCurve.Evaluate(
				4f + (manipulation ? 0f : -12f)); // animal melee skill is always +4, animals without "manipulation" body parts have a malus of -12 for DPS calculation
			return (output, manipulation ? null : "SY_CA.TooltipAttackModifierDPSNoManipulation".Translate());
		}
		#endregion

		#region OVERRIDES
		public override void Reset()
		{
			PowerBuffer = null;
			CooldownBuffer = null;
			
			TotalDamage = float.NaN;
			TotalCooldown = float.NaN;
		}

		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			// Power
			var settingPower = (SettingAttackPowerModifier)animalSettings.GeneralSettings["AttackPowerModifier"];
			settingPower.Value = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.AttackPowerModifier".Translate(),
				"SY_CA.TooltipAttackPowerModifier".Translate(),
				settingPower.IsModified(),
				settingPower.Value,
				settingPower.DefaultValue,
				ref PowerBuffer,
				min: SettingAttackPowerModifier.Minimum,
				max: SettingAttackPowerModifier.Maximum,
				unit: TotalDamage.ToString("0.00"));

			offsetY += SettingsDoubleRowHeight - SettingsRowHeight;

			// Cooldown
			var settingCooldown = (SettingAttackCooldownModifier)animalSettings.GeneralSettings["AttackCooldownModifier"];
			settingCooldown.Value = CreateNumeric(
				offsetY,
				viewWidth,
				"SY_CA.AttackCooldownModifier".Translate(),
				"SY_CA.TooltipAttackCooldownModifier".Translate(),
				settingCooldown.IsModified(),
				settingCooldown.Value,
				settingCooldown.DefaultValue,
				ref CooldownBuffer,
				min: SettingAttackCooldownModifier.Minimum,
				max: SettingAttackCooldownModifier.Maximum,
				unit: TotalCooldown.ToString("0.00"));

			offsetY += SettingsTripleRowHeight - SettingsDoubleRowHeight;

			// DPS Calculation
			(var dps, var dpsTooltip) = CalculateDPS(animalSettings.Animal);
			CreateText(offsetY, viewWidth, "SY_CA.AttackModifierDPS".Translate(), dps.ToString("0.00"), dpsTooltip);

			return SettingsTripleRowHeight;
		}

		public override float CreateSettingGlobal(float offsetY, float viewWidth)
		{
			// Power
			(var usePower, var valuePower) = CreateNumericGlobal(
				offsetY,
				viewWidth,
				"SY_CA.AttackPowerModifierGlobal".Translate(),
				"SY_CA.TooltipAttackPowerModifierGlobal".Translate(),
				SettingAttackPowerModifier.UseGlobal,
				SettingAttackPowerModifier.Global,
				SettingAttackPowerModifier.GlobalDefault,
				ref PowerBuffer,
				min: SettingAttackPowerModifier.Minimum,
				max: SettingAttackPowerModifier.Maximum);
			SettingAttackPowerModifier.UseGlobal = usePower;
			SettingAttackPowerModifier.Global = valuePower;

			offsetY += SettingsDoubleRowHeight - SettingsRowHeight;

			// Cooldown
			(var useCooldown, var valueCooldown) = CreateNumericGlobal(
				offsetY,
				viewWidth,
				"SY_CA.AttackCooldownModifierGlobal".Translate(),
				"SY_CA.TooltipAttackCooldownModifierGlobal".Translate(),
				SettingAttackCooldownModifier.UseGlobal,
				SettingAttackCooldownModifier.Global,
				SettingAttackCooldownModifier.GlobalDefault,
				ref CooldownBuffer,
				min: SettingAttackCooldownModifier.Minimum,
				max: SettingAttackCooldownModifier.Maximum);
			SettingAttackCooldownModifier.UseGlobal = useCooldown;
			SettingAttackCooldownModifier.Global = valueCooldown;

			return SettingsDoubleRowHeight;
		}
		#endregion

		#region CLASSES
		private class DamageCalculation
		{
			public float Damage;
			public float Cooldown;
			public float ArmorPenetration;
			public float ChanceFactor;
			public float InitialWeight;
			public int Cat;
			public float FactorCat;
			public float DPS => Damage / Cooldown;
		}
		#endregion
	}
}
