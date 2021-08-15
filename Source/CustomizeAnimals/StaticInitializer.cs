using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizeAnimals
{
	[StaticConstructorOnStartup]
	internal static class StaticInitializer
	{
		static StaticInitializer()
		{
			CustomizeAnimals_Settings.Initialize();
		}
	}

}
