using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizeAnimals.Settings
{
	internal class SettingLitterSizeCurve : BaseSetting<SimpleCurve>
	{
		private enum HasCurvePointResult
		{
			NotFound = -1,
			NotEqual = 0,
			Equal = 1,
		}

		#region PROPERTIES
		#endregion

		#region CONSTRUCTORS
		public SettingLitterSizeCurve(ThingDef animal, bool isGlobal = false) : base(animal, isGlobal)
		{
			if (!isGlobal)
				DefaultValue = new SimpleCurve(Value.Points);
		}
		#endregion

		#region PUBLIC METHODS
		public static bool IsModified(SimpleCurve one, SimpleCurve two)
		{
			if (one?.PointsCount != two?.PointsCount)
				return true;
			if (one != null)
				for (int i = 0; i < one.PointsCount; i++)
					if (!one[i].Equals(two[i]))
						return true;
			return false;
		}
		#endregion

		#region OVERRIDES
		public override void GetValue()
		{
			if (!IsGlobal)
				Value = Animal.race?.litterSizeCurve ?? new SimpleCurve();
		}
		public override void SetValue()
		{
			var race = Animal?.race;
			if (race != null)
				race.litterSizeCurve = Value.PointsCount >= 3 && Value.First().y == 0f && Value.Last().y == 0f ? new SimpleCurve(Value.Points) : null;
		}

		public override void Reset()
		{
			Value.Points.Clear();
			if (DefaultValue.PointsCount > 0)
				foreach (var point in DefaultValue)
					Value.Add(point);
		}
		public override bool IsModified() =>
			IsModified(Value, DefaultValue);

		public override void ExposeData()
		{
			if (Scribe.mode != LoadSaveMode.Saving || IsModified())
			{
				var points = Value.Points;
				Scribe_Collections.Look(ref points, "LitterSizeCurve");

				if (points == null)
					Value = new SimpleCurve(DefaultValue);
				else if (points != Value.Points)
					Value.SetPoints(points);
			}
		}
		#endregion
	}
}
