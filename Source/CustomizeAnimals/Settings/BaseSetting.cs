using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizeAnimals.Settings
{
	public abstract class BaseSetting<T> : ISetting, IExposable
	{
		#region PROPERTIES
		public ThingDef Animal { get; }
		public T Value { get; set; }
		public T DefaultValue { get; protected set; }
		#endregion

		#region CONSTRUCTORS
		public BaseSetting(ThingDef animal)
		{
			Animal = animal;
			Value = DefaultValue = GetValue();
		}
		#endregion

		#region METHODS
		public abstract T GetValue();
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

	public interface ISetting
	{
		void SetValue();
		void Reset();
		void ExposeData();

		void ResetGlobal();
		void ExposeGlobal();

		bool IsModified();
		bool IsGlobalUsed();
	}
}
