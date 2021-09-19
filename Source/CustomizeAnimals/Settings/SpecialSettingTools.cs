using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizeAnimals.Settings
{
	internal class SpecialSettingAttackTools : BaseSpecialSetting
	{
		public List<AttackTool> Tools { get; set; } = new List<AttackTool>();

		public SpecialSettingAttackTools(ThingDef animal) : base(animal)
		{
			GetValue();
		}

		public override void GetValue()
		{
			var tools = Animal?.tools;
			if (tools != null)
			{
				if (Tools == null)
					Tools = new List<AttackTool>();
				else if (Tools.Count > 0)
				{
					Tools.Clear();
					Log.Message($"{Animal.defName}: Tools list cleared! (shouldn't happen)");
				}
				foreach (var tool in tools)
					Tools.Add(new AttackTool(tool));
			}
			else
				Log.Warning($"{nameof(CustomizeAnimals)}.{nameof(SpecialSettingAttackTools)}: {Animal?.defName} tools is null!");
		}
		public override void SetValue()
		{
			foreach (var tool in Tools)
				tool.SetValue();
		}

		public override void Reset()
		{
			foreach (var tool in Tools)
				tool.Reset();
		}
		public override bool IsModified()
		{
			foreach (var tool in Tools)
				if (tool.IsModified())
					return true;
			return false;
		}

		public override void ExposeData()
		{
			Scribe.EnterNode("Tools");
			for (int i = 0; i < Tools.Count; i++)
			{
				Scribe.EnterNode("li" + i);
				Tools[i].ExposeData();
				Scribe.ExitNode();
			}
			Scribe.ExitNode();
		}
	}

	internal class AttackTool : IExposable
	{
		public Tool Tool { get; }
		public string Label => Tool?.label?.CapitalizeFirst();
		public BodyPartGroupDef LinkedBodyPartsGroup => Tool?.linkedBodyPartsGroup;

		public float DefaultPower { get; private set; }
		public float Power { get; set; }
		public float DefaultCooldown { get; private set; }
		public float Cooldown { get; set; }
		public float DefaultArmorPenetration { get; private set; }
		public float ArmorPenetration { get; set; }
		public float DefaultChanceFactor { get; private set; }
		public float ChanceFactor { get; set; }

		public List<ToolCapacityDef> Capacities { get; } = new List<ToolCapacityDef>();

		//public List<ExtraDamage> ExtraDamages { get; } = new List<ExtraDamage>();
		//public List<ExtraDamage> SurpriseAttack_ExtraDamages { get; } = new List<ExtraDamage>();

		public AttackTool(Tool tool)
		{
			Tool = tool;
			if (Tool == null)
			{
				Log.Error($"{nameof(CustomizeAnimals)}.{nameof(AttackTool)}: tool-parameter was null!");
				return;
			}

			GetValue();

			DefaultPower = Power;
			DefaultCooldown = Cooldown;
			DefaultArmorPenetration = ArmorPenetration;
			DefaultChanceFactor = ChanceFactor;

			foreach (var capacity in Tool.capacities)
				Capacities.Add(capacity);

			//foreach (var extraDamage in Tool.extraMeleeDamages)
			//	ExtraDamages.Add(extraDamage);
			//foreach (var extraDamage in Tool.surpriseAttack.extraMeleeDamages)
			//	SurpriseAttack_ExtraDamages.Add(extraDamage);
		}
		public AttackTool(AttackTool attackTool)
		{
			DefaultPower = attackTool.DefaultPower;
			DefaultCooldown = attackTool.DefaultCooldown;
			DefaultArmorPenetration = attackTool.DefaultArmorPenetration;
			DefaultChanceFactor = attackTool.DefaultChanceFactor;

			foreach (var capacity in attackTool.Capacities)
				Capacities.Add(capacity);

			//foreach (var extraDamage in attackTool.ExtraDamages)
			//	ExtraDamages.Add(extraDamage);
			//foreach (var extraDamage in attackTool.SurpriseAttack_ExtraDamages)
			//	SurpriseAttack_ExtraDamages.Add(extraDamage);
		}

		#region PUBLIC METHODS
		public void GetValue()
		{
			if (Tool == null)
				return;
			Power = Tool.power;
			Cooldown = Tool.cooldownTime;
			ArmorPenetration = Tool.armorPenetration;
			ChanceFactor = Tool.chanceFactor;
		}
		public void SetValue()
		{
			if (Tool == null)
				return;
			Tool.power = Power;
			Tool.cooldownTime = Cooldown;
			Tool.armorPenetration = ArmorPenetration;
			Tool.chanceFactor = ChanceFactor;
		}

		public void Reset()
		{
			Power = DefaultPower;
			Cooldown = DefaultCooldown;
			ArmorPenetration = DefaultArmorPenetration;
			ChanceFactor = DefaultChanceFactor;
		}
		public bool IsModified()
		{
			if (Power != DefaultPower
				|| Cooldown != DefaultCooldown
				|| ArmorPenetration != DefaultArmorPenetration
				|| ChanceFactor != DefaultChanceFactor)
				return true;
			return false;
		}

		public void ExposeData()
		{
			var value = Power;
			Scribe_Values.Look(ref value, nameof(Power), DefaultPower);
			Power = value;

			value = Cooldown;
			Scribe_Values.Look(ref value, nameof(Cooldown), DefaultCooldown);
			Cooldown = value;

			value = ArmorPenetration;
			Scribe_Values.Look(ref value, nameof(ArmorPenetration), DefaultArmorPenetration);
			ArmorPenetration = value;

			value = ChanceFactor;
			Scribe_Values.Look(ref value, nameof(ChanceFactor), DefaultChanceFactor);
			ChanceFactor = value;
		}
		#endregion
	}
}
