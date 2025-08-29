using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CustomizeAnimals
{
	internal static class CustomizeAnimalsUtility
	{
		public static bool AddIfNotContains<T>(this List<T> list, T item)
		{
			if (list.Contains(item)) 
				return false;
			list.Add(item);
			return true;
		}

		public static void SetFrom<T>(this T[] to, T[] from, int toOffset = 0, int fromOffset = 0)
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

		public static bool Compare<T>(this T[] a, T[] b, Func<T, T, bool> comp)
		{
			if (a != null && b != null)
				for (int i = 0; i < a.Length && i < b.Length; i++)
					if (comp(a[i], b[i]))
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

		public static void ExposeByteRangeArray(string nodeName, ByteRange[] values, ByteRange[] defaultValues, string[] names = null)
		{
			if (Scribe.mode != LoadSaveMode.Saving || isModified())
			{
				if (Scribe.EnterNode(nodeName))
				{
					for (int i = 0; i < values.Length; i++)
					{
						var value = values[i];
						var defaultValue = i < defaultValues?.Length ? defaultValues[i] : default;
						var name = i < names?.Length ? names[i] : $"Value_{i}";

						Scribe_Values.Look(ref value.min, name + "_min", defaultValue.min);
						Scribe_Values.Look(ref value.max, name + "_max", defaultValue.max);

						values[i] = value;
					}
					Scribe.ExitNode();
				}
			}
			bool isModified()
			{
				if (values.Length != defaultValues.Length)
					return true;
				for (int i = 0; i < values.Length; i++)
				{
					if (values[i].min != defaultValues[i].min
						|| values[i].max != defaultValues[i].max)
						return true;
				}
				return false;
			}
		}

		public static bool IsAnimal(this ThingDef thingDef) =>
			!thingDef.IsHumanLike()
			&& thingDef.thingCategories?.Contains(ThingCategoryDefOf.Animals) == true   // ANIMALS should have thing category "Animals"
			&& thingDef.race?.trainability != null;                                     // all ANIMALS have trainability, assuming that everything else is NOT an ANIMAL
		public static bool IsHumanLike(this ThingDef animal) =>
			animal?.race?.Humanlike == true;


		public static string Def2String(this Def def) =>
			def?.defName ?? "null";
	}
}
