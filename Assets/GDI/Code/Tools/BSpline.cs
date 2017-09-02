using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.GDI.Code.Tools
{
	public class BSpline
	{
		private float[] _x;
		private float[] _y;

		private float[] _v;
		private float[] _t;

		public float LowestX;
		public float HighestX;


		public BSpline(List<Vector2> points)
		{
			if (points == null || points.Count < 2)
			{
				throw new Exception("Invalid parameter.");
			}
			int pointCount = points.Count;

			_x = new float[pointCount];
			_y = new float[pointCount];

			_v = new float[pointCount];
			_t = new float[pointCount];
			UpdatePoints(points);
		}

		public void UpdatePoints(List<Vector2> points)
		{
			if (points.Count != _x.Length)
			{
				Log.Error("Can not update points. The counts do not match.");
				return;
			}
			for (int i = 0; i < points.Count; i++)
			{
				_x[i] = points[i].x;
				_y[i] = points[i].y;
			}
			Update();
		}

		private void Update()
		{
			LowestX = float.MaxValue;
			HighestX = float.MinValue;

			int pointCount = _x.Length;

			for (int i = 0; i < pointCount; i++)
			{
				if (_x[i] < LowestX) LowestX = _x[i];
				if (_x[i] > HighestX) HighestX = _x[i];
				if (i >= 1) _v[i] = _x[i] - _x[i - 1];
			}

			if (pointCount > 2)
			{
				var s = new float[pointCount - 1];
				var s1 = new float[pointCount - 1];
				var d = new float[pointCount - 1];

				for (int i = 1; i <= pointCount - 2; i++)
				{
					d[i] = (_v[i] + _v[i + 1]) / 3;
					s[i] = _v[i] / 6;
					s1[i] = _v[i + 1] / 6;
					_t[i] = (_y[i + 1] - _y[i]) / _v[i + 1] - (_y[i] - _y[i - 1]) / _v[i];
				}
				Calc(s, d, s1, ref _t, pointCount - 2);
			}
		}

		public float GetValue(float x)
		{
			if (x >= HighestX) return _y[_y.Length - 1];
			if (x <= LowestX) return _y[0];

			//if (x > HighestX) return float.NaN;
			//if (x < LowestX) return float.NaN;

			int dist = 0;
			var lastX = float.MinValue;
			for (int i = 0; i < _x.Length; i++)
			{
				if (_x[i] < x && _x[i] > lastX)
				{
					lastX = _x[i];
					dist = i + 1;
				}
			}

			var x1 = x - lastX;
			var x2 = _v[dist] - x1;

			return ((-_t[dist - 1] / 6 * (x2 + _v[dist]) * x1 + _y[dist - 1]) * x2 +
			        (-_t[dist] / 6 * (x1 + _v[dist]) * x2 + _y[dist]) * x1) / _v[dist];
		}

		private static void Calc(float[] s, float[] d, float[] s1, ref float[] b, int n)
		{
			int i;
			for (i = 2; i <= n; i++)
			{
				s[i] = s[i] / d[i - 1];
				d[i] = d[i] - s[i] * s1[i - 1];
				b[i] = b[i] - s[i] * b[i - 1];
			}
			b[n] = b[n] / d[n];
			for (i = n - 1; i >= 1; i--) b[i] = (b[i] - s1[i] * b[i + 1]) / d[i];
		}
	}
}
