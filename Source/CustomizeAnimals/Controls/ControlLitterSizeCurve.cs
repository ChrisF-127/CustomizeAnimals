using CustomizeAnimals.Settings;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CustomizeAnimals.Controls
{
	internal class ControlLitterSizeCurve : BaseSettingControl
	{
		private const float PointControlRowHeight = 26f;
		private readonly List<CurvePointStringBuffer> Buffers = new List<CurvePointStringBuffer>();

		#region OVERRIDES
		public override float CreateSetting(float offsetY, float viewWidth, AnimalSettings animalSettings)
		{
			var setting = (SettingLitterSizeCurve)animalSettings.Settings["LitterSizeCurve"];

			var controlWidth = GetControlWidth(viewWidth);
			var halfWidth = controlWidth / 2;
			var buttonDim = PointControlRowHeight - 4;
			var totalHeight = offsetY;

			var value = setting.Value;
			var isModified = setting.IsModified();

			
			// Label
			if (isModified)
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(0, totalHeight, controlWidth, PointControlRowHeight + 3), "SY_CA.LitterSizeCurve".Translate());
			GUI.color = OriColor;

			// Range
			var rect = new Rect(controlWidth + 4, totalHeight, halfWidth - 5, PointControlRowHeight + 3);
			int min = value.PointsCount > 0 ? Mathf.Max(Mathf.RoundToInt(value.First().x), 1) : 1;
			int max = value.PointsCount > 0 ? Mathf.Max(Mathf.RoundToInt(value.Last().x), 1) : 1;
			Widgets.Label(rect, min + "~" + max);
			DrawTooltip(rect, "SY_CA.TooltipLitterSizeCurveRange".Translate());

			// Average
			rect = new Rect(controlWidth + 5 + halfWidth, totalHeight, halfWidth - 5, PointControlRowHeight + 3);
			var avg = value.PointsCount > 1 ? Rand.ByCurveAverage(value) : value.PointsCount == 1 ? value[0].x : 1f;
			Widgets.Label(rect, (!float.IsNaN(avg) ? avg : 1f).ToString("0.000"));
			DrawTooltip(rect, "SY_CA.TooltipLitterSizeCurveAverage".Translate());

			// Reset button
			if (isModified && DrawResetButton(totalHeight, viewWidth, "\n" + CurveToString(setting.DefaultValue)))
				setting.Reset();
			totalHeight += PointControlRowHeight + 3;


			// Label "Count"
			rect = new Rect(controlWidth + 4, totalHeight, halfWidth - 5, PointControlRowHeight + 3);
			Widgets.Label(rect, "SY_CA.LitterSizeCurveCount".Translate());
			DrawTooltip(rect, "SY_CA.TooltipLitterSizeCurveCount".Translate());

			// Label "Probability"
			rect = new Rect(controlWidth + 5 + halfWidth, totalHeight, halfWidth - 5, PointControlRowHeight + 3);
			Widgets.Label(rect, "SY_CA.LitterSizeCurveProbability".Translate());
			DrawTooltip(rect, "SY_CA.TooltipLitterSizeCurveProbability".Translate());

			// Add button
			rect = new Rect(viewWidth + 1 - (SettingsRowHeight * 2), totalHeight + 2, buttonDim, buttonDim);
			DrawTooltip(rect, "SY_CA.TooltipLitterSizeCurveAdd".Translate());
			if (Widgets.ButtonText(rect, "+"))
				value.Add(value.PointsCount > 0 ? value.Last().x + 1 : 1, 0);

			totalHeight += PointControlRowHeight;


			// Curve points
			totalHeight += CreateCurveControl(totalHeight, viewWidth, controlWidth, halfWidth, buttonDim, value);
			totalHeight += 3;

			// Sort curve points
			var points = value.Points.ToArray();
			var buffers = Buffers.ToArray();
			Array.Sort(points, buffers, new CurvePointComparer());
			value.SetPoints(points);
			Buffers.Clear();
			Buffers.AddRange(buffers);

			return totalHeight - offsetY;
		}

		public override void Reset()
		{
			base.Reset();
			Buffers.Clear();
		}
		#endregion

		#region PRIVATE METHODS
		private float CreateCurveControl(
			float offsetY,
			float viewWidth,
			float controlWidth,
			float halfWidth,
			float buttonDim,
			SimpleCurve value)
		{
			if (!(value?.PointsCount > 0))
				return 0f;

			var totalHeight = offsetY;

			while (Buffers.Count < value.PointsCount)
				Buffers.Add(new CurvePointStringBuffer());
			while (Buffers.Count > value.PointsCount)
				Buffers.RemoveLast();

			for (int i = 0; i < value.PointsCount;)
			{
				var point = value[i];

				// Count
				var buffer = Buffers[i];
				var x = point.x;
				var rect = new Rect(controlWidth + 2, totalHeight + 2, halfWidth - 3, PointControlRowHeight - 4);
				Widgets.TextFieldNumeric(rect, ref x, ref buffer.x);
				DrawTooltip(rect, "SY_CA.TooltipLitterSizeCurveCount".Translate());

				// Probability
				var y = point.y;
				rect = new Rect(controlWidth + 3 + halfWidth, totalHeight + 2, halfWidth - 3, PointControlRowHeight - 4);
				Widgets.TextFieldNumeric(rect, ref y, ref buffer.y);
				DrawTooltip(rect, "SY_CA.TooltipLitterSizeCurveProbability".Translate());

				// Remove button
				rect = new Rect(viewWidth + 1 - (SettingsRowHeight * 2), totalHeight + 2, buttonDim, buttonDim);
				DrawTooltip(rect, "SY_CA.TooltipLitterSizeCurveRemove".Translate());
				if (Widgets.ButtonText(rect, "-"))
				{
					value.Points.RemoveAt(i);
					Buffers.RemoveAt(i);
					continue;
				}

				// Replace changed curve point
				if (point.x != x || point.y != y)
					value[i] = new CurvePoint(x, y);

				totalHeight += PointControlRowHeight;
				i++;
			}

			return totalHeight - offsetY;
		}

		private string CurveToString(SimpleCurve curve)
		{
			if (curve != null)
			{
				string output = "";
				for (int i = 0; i < curve.PointsCount; i++)
				{
					if (i > 0)
						output += "\n";
					output += $"({curve[i].x},{curve[i].y})";
				}
				return output;
			}
			return "None";
		}

		private class CurvePointStringBuffer
		{
			public string x;
			public string y;
		}
		#endregion

		#region PRIVATE CLASSES
		private class CurvePointComparer : IComparer<CurvePoint>
		{
			int IComparer<CurvePoint>.Compare(CurvePoint x, CurvePoint y)
			{
				var v = x.x - y.x;
				if (v > 0)
					return 1;
				if (v < 0)
					return -1;
				return 0;
			}
		}
		#endregion
	}
}
