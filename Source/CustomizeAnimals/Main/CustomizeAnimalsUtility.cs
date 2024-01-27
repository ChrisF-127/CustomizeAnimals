using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizeAnimals
{
	internal static class CustomizeAnimalsUtility
	{
		public static void SetFrom<T>(this T[] to, T[] from, int toOffset = 0, int fromOffset = 0)
			where T : IComparable
		{
			if (from == null || to == null)
				return;
			for (int i = toOffset, j = fromOffset; i < to.Length && j < from.Length; i++, j++)
				to[i] = from[j];
		}

		public static bool IsDifferent<T>(this T[] a, T[] b)
			where T : IComparable
		{
			if (a != null && b != null)
				for (int i = 0; i < a.Length && i < b.Length; i++)
					if (a[i].CompareTo(b[i]) != 0)
						return true;
			return false;
		}

		public static void ExposeArray<T>(string nodeName, Func<bool> isModified, T[] values, T[] defaultValues, string[] names = null)
		{
			if (Scribe.mode != LoadSaveMode.Saving || isModified())
			{
				if (Scribe.EnterNode(nodeName))
				{
					for (int i = 0; i < values.Length; i++)
					{
						var value = values[i];
						var name = i < names?.Length ? names[i] : $"Value_{i}";
						var defaultValue = i < defaultValues?.Length ? defaultValues[i] : default;
						Scribe_Values.Look(ref value, name, defaultValue);
						values[i] = value;
					}
					Scribe.ExitNode();
				}
			}
		}

		public static bool IsAnimal(this ThingDef animal) =>
			!animal.IsHumanLike();
		public static bool IsHumanLike(this ThingDef animal) =>
			animal?.race?.Humanlike == true;
	}
}
