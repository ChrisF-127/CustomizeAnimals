using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizeAnimals.Settings
{
	public abstract class BaseSetting<T> : IExposable
	{
		public ThingDef Animal { get; }
		public T Value { get; set; }
		public T DefaultValue { get; protected set; }

		public BaseSetting(ThingDef animal)
		{
			Animal = animal;
			Value = DefaultValue = Get();
		}

		public abstract T Get();
		public abstract void Set();
		public abstract void ExposeData();

		public bool IsModified() =>
			!(DefaultValue == null && Value == null || DefaultValue?.Equals(Value) == true);
		public void Reset() =>
			Value = DefaultValue;

		protected static string Def2String(Def def) =>
			def?.defName ?? "null";
	}
}
