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
	internal class SettingFenceBlocked : BaseSetting<bool?>
	{
		#region PROPERTIES
		public static Dictionary<ThingDef, bool?> Cache { get; } = new Dictionary<ThingDef, bool?>();

		public static bool Always { get; set; }
		public static bool NotWhenFollowing { get; set; }

		protected override string ScribeLabel =>
			"FenceBlocked";
		#endregion

		#region CONSTRUCTORS
		public SettingFenceBlocked(ThingDef animal, bool isGlobal = false) : 
			base(animal, isGlobal)
		{ }
		#endregion

		#region INTERFACES
		public override void GetValue()
		{ }
		public override void SetValue()
		{
			// reset cache
			Cache.Clear();
		}

		public override void ResetGlobal()
		{
			Always = false;
			NotWhenFollowing = false;
		}
		public override void ExposeGlobal()
		{
			var value = Always;
			Scribe_Values.Look(ref value, "FenceBlockedAlways", false);
			Always = value;

			value = NotWhenFollowing;
			Scribe_Values.Look(ref value, "FenceBlockedNotWhenFollowing", false);
			NotWhenFollowing = value;
		}
		public override bool IsGlobalUsed() =>
			Always || NotWhenFollowing;
		#endregion
	}
}
