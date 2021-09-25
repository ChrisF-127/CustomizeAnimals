using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizeAnimals
{
	internal delegate void GiveShortHash(Def def, Type defType);

	internal static class Helper
	{
		internal static GiveShortHash GiveShortHashDelegate;

		static Helper()
		{
			var method = typeof(ShortHashGiver).GetMethod(
				"GiveShortHash",
				BindingFlags.Static | BindingFlags.NonPublic,
				null,
				new Type[2] { typeof(Def), typeof(Type) },
				null);
			if (method != null)
				GiveShortHashDelegate = (GiveShortHash)Delegate.CreateDelegate(typeof(GiveShortHash), method);
			else
				Log.Error($"{nameof(CustomizeAnimals)}: failed to initialize ShortHashGiver.GiveShortHash-delegate");
		}
	}
}
