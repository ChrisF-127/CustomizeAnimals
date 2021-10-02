using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CustomizeAnimals.Settings
{
	public interface ISetting
	{
		void GetValue();
		void SetValue();
		void Reset();
		void ExposeData();
		bool IsModified();
	}

	public interface ISettingWithGlobal : ISetting
	{
		void ResetGlobal();
		void ExposeGlobal();
		bool IsGlobalUsed();
	}

	internal abstract class BaseSetting<T> : ISettingWithGlobal, IExposable
	{
		#region PROPERTIES
		public ThingDef Animal { get; }
		public T Value { get; set; }
		public T DefaultValue { get; protected set; }

		public bool IsGlobal { get; } = false;
		#endregion

		#region CONSTRUCTORS
		public BaseSetting(ThingDef animal, bool isGlobal = false)
		{
			IsGlobal = isGlobal;

			Animal = animal;

			GetValue();
			DefaultValue = Value;
		}
		#endregion

		#region METHODS
		public abstract void GetValue();
		public abstract void SetValue();
		public virtual void Reset() =>
			Value = DefaultValue;
		public abstract void ExposeData();

		public virtual void ResetGlobal()
		{ }
		public virtual void ExposeGlobal()
		{ }

		public virtual bool IsModified() =>
			!(DefaultValue == null && Value == null || DefaultValue?.Equals(Value) == true);
		public virtual bool IsGlobalUsed() =>
			false;

		protected static string Def2String(Def def) =>
			def?.defName ?? "null";
		#endregion
	}

	internal abstract class NullableFloatSetting : BaseSetting<float?>
	{
		#region CONSTRUCTORS
		public NullableFloatSetting(ThingDef animal, bool isGlobal) : base(animal, isGlobal)
		{ }
		#endregion

		#region METHODS
		protected virtual float? GetStat(StatDef stat, bool useDefaultValueIfNull)
		{
			var statBases = Animal?.statBases;
			if (statBases != null)
			{
				var value = statBases.FirstOrDefault((s) => s.stat == stat)?.value;
				if (useDefaultValueIfNull && value == null)
					value = stat.defaultBaseValue;
				return value;
			}

			if (!IsGlobal)
				Log.Warning($"{nameof(CustomizeAnimals)}.{GetType()}: {Animal?.defName} statBases is null, value cannot be set!");
			return null;
		}
		protected virtual void SetStat(StatDef stat, bool useLimits = false, float min = 0f, float? max = 1e9f)
		{
			if (Animal == null || stat == null)
			{
				Log.Error($"{nameof(CustomizeAnimals)}.{GetType()}.{nameof(SetStat)}: invalid parameters: animal: {Animal} stat: {stat}");
				return;
			}

			var statBases = Animal?.statBases;
			if (statBases != null)
			{
				var local = Value;
				if (useLimits)
				{
					if (max == null)
						local = null;
					else if (local < min)
						local = min;
					else if (local > max)
						local = max;
				}

				var statModifier = statBases.FirstOrDefault((s) => s.stat == stat);
				if (local is float statValue)
				{
					if (statModifier != null)
						statModifier.value = statValue;
					else
						statBases.Add(new StatModifier { stat = stat, value = statValue });
				}
				else if (statModifier != null)
					statBases.Remove(statModifier);
			}
		}
		#endregion
	}

	internal abstract class BaseSpecialSetting : ISetting, IExposable
	{
		public ThingDef Animal { get; }

		public BaseSpecialSetting(ThingDef animal)
		{
			Animal = animal;
		}

		public abstract void GetValue();
		public abstract void SetValue();
		public abstract void Reset();
		public abstract bool IsModified();
		public abstract void ExposeData();
	}
}
