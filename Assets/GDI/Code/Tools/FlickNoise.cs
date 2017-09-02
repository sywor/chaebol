using UnityEngine;

namespace Assets.GDI.Code.Tools
{
	public static class FlickNoise {


		private static int[] hash = {
			151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
			140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
			247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
			57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
			74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
			60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
			65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
			200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
			52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
			207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
			119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
			129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
			218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
			81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
			184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
			222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180,

			151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
			140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
			247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
			57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
			74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
			60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
			65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
			200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
			52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
			207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
			119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
			129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
			218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
			81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
			184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
			222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180
		};

		private const int HashMask = 255;

		private static float[] _gradients1D = {
			1f, -1f
		};

		private const int GradientsMask1D = 1;

		private static Vector2[] _gradients2D = {
			new Vector2( 1f, 0f),
			new Vector2(-1f, 0f),
			new Vector2( 0f, 1f),
			new Vector2( 0f,-1f),
			new Vector2( 1f, 1f).normalized,
			new Vector2(-1f, 1f).normalized,
			new Vector2( 1f,-1f).normalized,
			new Vector2(-1f,-1f).normalized
		};

		private const int GradientsMask2D = 7;

		private static Vector3[] _gradients3D = {
			new Vector3( 1f, 1f, 0f),
			new Vector3(-1f, 1f, 0f),
			new Vector3( 1f,-1f, 0f),
			new Vector3(-1f,-1f, 0f),
			new Vector3( 1f, 0f, 1f),
			new Vector3(-1f, 0f, 1f),
			new Vector3( 1f, 0f,-1f),
			new Vector3(-1f, 0f,-1f),
			new Vector3( 0f, 1f, 1f),
			new Vector3( 0f,-1f, 1f),
			new Vector3( 0f, 1f,-1f),
			new Vector3( 0f,-1f,-1f),

			new Vector3( 1f, 1f, 0f),
			new Vector3(-1f, 1f, 0f),
			new Vector3( 0f,-1f, 1f),
			new Vector3( 0f,-1f,-1f)
		};

		/*private static Vector3[] simplexGradients3D = {
		new Vector3( 1f, 1f, 0f).normalized,
		new Vector3(-1f, 1f, 0f).normalized,
		new Vector3( 1f,-1f, 0f).normalized,
		new Vector3(-1f,-1f, 0f).normalized,
		new Vector3( 1f, 0f, 1f).normalized,
		new Vector3(-1f, 0f, 1f).normalized,
		new Vector3( 1f, 0f,-1f).normalized,
		new Vector3(-1f, 0f,-1f).normalized,
		new Vector3( 0f, 1f, 1f).normalized,
		new Vector3( 0f,-1f, 1f).normalized,
		new Vector3( 0f, 1f,-1f).normalized,
		new Vector3( 0f,-1f,-1f).normalized,

		new Vector3( 1f, 1f, 0f).normalized,
		new Vector3(-1f, 1f, 0f).normalized,
		new Vector3( 1f,-1f, 0f).normalized,
		new Vector3(-1f,-1f, 0f).normalized,
		new Vector3( 1f, 0f, 1f).normalized,
		new Vector3(-1f, 0f, 1f).normalized,
		new Vector3( 1f, 0f,-1f).normalized,
		new Vector3(-1f, 0f,-1f).normalized,
		new Vector3( 0f, 1f, 1f).normalized,
		new Vector3( 0f,-1f, 1f).normalized,
		new Vector3( 0f, 1f,-1f).normalized,
		new Vector3( 0f,-1f,-1f).normalized,

		new Vector3( 1f, 1f, 1f).normalized,
		new Vector3(-1f, 1f, 1f).normalized,
		new Vector3( 1f,-1f, 1f).normalized,
		new Vector3(-1f,-1f, 1f).normalized,
		new Vector3( 1f, 1f,-1f).normalized,
		new Vector3(-1f, 1f,-1f).normalized,
		new Vector3( 1f,-1f,-1f).normalized,
		new Vector3(-1f,-1f,-1f).normalized
	};*/

		//private const int simplexGradientsMask3D = 31;

		private const int GradientsMask3D = 15;

		private static float Dot (Vector2 g, float x, float y) {
			return g.x * x + g.y * y;
		}

		private static float Dot (Vector3 g, float x, float y, float z) {
			return g.x * x + g.y * y + g.z * z;
		}

		private static float Smooth (float t) {
			return t * t * t * (t * (t * 6f - 15f) + 10f);
		}

		private static float _sqr2 = Mathf.Sqrt(2f);

		private static float _squaresToTriangles = (3f - Mathf.Sqrt(3f)) / 6f;
		private static float _trianglesToSquares = (Mathf.Sqrt(3f) - 1f) / 2f;

		private static float _simplexScale2D = 2916f * _sqr2 / 125f;
		//private static float simplexScale3D = 8192f * Mathf.Sqrt(3f) / 375f;

		public static float Value1D (float x, bool smooth) {
			int i0 = Mathf.FloorToInt(x);
			float t = x - i0;
			i0 &= HashMask;
			int i1 = i0 + 1;
			int h0 = hash[i0];
			int h1 = hash[i1];
			if (smooth) t = Smooth(t);
			float a = h0;
			float b = h1 - h0;
			return (a + b * t) * (2f / HashMask) - 1f;
		}

		public static float Value2D (float x, float y, bool smooth) {
			int ix0 = Mathf.FloorToInt(x);
			int iy0 = Mathf.FloorToInt(y);
			float tx = x - ix0;
			float ty = y - iy0;
			ix0 &= HashMask;
			iy0 &= HashMask;
			int ix1 = ix0 + 1;
			int iy1 = iy0 + 1;

			int h0 = hash[ix0];
			int h1 = hash[ix1];
			int h00 = hash[h0 + iy0];
			int h10 = hash[h1 + iy0];
			int h01 = hash[h0 + iy1];
			int h11 = hash[h1 + iy1];

			if (smooth)
			{
				tx = Smooth(tx);
				ty = Smooth(ty);
			}

			float a = h00;
			float b = h10 - h00;
			float c = h01 - h00;
			float d = h11 - h01 - h10 + h00;

			return (a + b * tx + (c + d * tx) * ty) * (2f / HashMask) - 1f;
		}

		public static float Value3D (float x, float y, float z, bool smooth) {
			int ix0 = Mathf.FloorToInt(x);
			int iy0 = Mathf.FloorToInt(y);
			int iz0 = Mathf.FloorToInt(z);
			float tx = x - ix0;
			float ty = y - iy0;
			float tz = z - iz0;
			ix0 &= HashMask;
			iy0 &= HashMask;
			iz0 &= HashMask;
			int ix1 = ix0 + 1;
			int iy1 = iy0 + 1;
			int iz1 = iz0 + 1;

			int h0 = hash[ix0];
			int h1 = hash[ix1];
			int h00 = hash[h0 + iy0];
			int h10 = hash[h1 + iy0];
			int h01 = hash[h0 + iy1];
			int h11 = hash[h1 + iy1];
			int h000 = hash[h00 + iz0];
			int h100 = hash[h10 + iz0];
			int h010 = hash[h01 + iz0];
			int h110 = hash[h11 + iz0];
			int h001 = hash[h00 + iz1];
			int h101 = hash[h10 + iz1];
			int h011 = hash[h01 + iz1];
			int h111 = hash[h11 + iz1];

			if (smooth)
			{
				tx = Smooth(tx);
				ty = Smooth(ty);
				tz = Smooth(tz);
			}

			float a = h000;
			float b = h100 - h000;
			float c = h010 - h000;
			float d = h001 - h000;
			float e = h110 - h010 - h100 + h000;
			float f = h101 - h001 - h100 + h000;
			float g = h011 - h001 - h010 + h000;
			float h = h111 - h011 - h101 + h001 - h110 + h010 + h100 - h000;

			float sample = a + b * tx + (c + e * tx) * ty + (d + f * tx + (g + h * tx) * ty) * tz;
			return sample * (2f / HashMask) - 1f;
		}

		public static float Perlin1D (float x, bool smooth) {
			int i0 = Mathf.FloorToInt(x);
			float t0 = x - i0;
			float t1 = t0 - 1f;
			i0 &= HashMask;
			int i1 = i0 + 1;
			float g0 = _gradients1D[hash[i0] & GradientsMask1D];
			float g1 = _gradients1D[hash[i1] & GradientsMask1D];
			float v0 = g0 * t0;
			float v1 = g1 * t1;

			float t = t0;
			if (smooth)
			{
				t = Smooth(t0);
			}

			float a = v0;
			float b = v1 - v0;
			return (a + b * t) * 2f;
		}

		public static float Perlin2D (float x, float y, bool smooth) {
			int ix0 = Mathf.FloorToInt(x);
			int iy0 = Mathf.FloorToInt(y);
			float tx0 = x - ix0;
			float ty0 = y - iy0;
			float tx1 = tx0 - 1f;
			float ty1 = ty0 - 1f;
			ix0 &= HashMask;
			iy0 &= HashMask;
			int ix1 = ix0 + 1;
			int iy1 = iy0 + 1;

			int h0 = hash[ix0];
			int h1 = hash[ix1];
			Vector2 g00 = _gradients2D[hash[h0 + iy0] & GradientsMask2D];
			Vector2 g10 = _gradients2D[hash[h1 + iy0] & GradientsMask2D];
			Vector2 g01 = _gradients2D[hash[h0 + iy1] & GradientsMask2D];
			Vector2 g11 = _gradients2D[hash[h1 + iy1] & GradientsMask2D];

			float v00 = Dot(g00, tx0, ty0);
			float v10 = Dot(g10, tx1, ty0);
			float v01 = Dot(g01, tx0, ty1);
			float v11 = Dot(g11, tx1, ty1);

			float tx = tx0;
			float ty = ty0;

			if (smooth)
			{
				tx = Smooth(tx0);
				ty = Smooth(ty0);
			}

			float a = v00;
			float b = v10 - v00;
			float c = v01 - v00;
			float d = v11 - v01 - v10 + v00;
			return (a + b * tx + (c + d * tx) * ty) * _sqr2;
		}

		public static float Perlin3D (float x, float y, float z, bool smooth) {
			int ix0 = Mathf.FloorToInt(x);
			int iy0 = Mathf.FloorToInt(y);
			int iz0 = Mathf.FloorToInt(z);
			float tx0 = x - ix0;
			float ty0 = y - iy0;
			float tz0 = z - iz0;
			float tx1 = tx0 - 1f;
			float ty1 = ty0 - 1f;
			float tz1 = tz0 - 1f;
			ix0 &= HashMask;
			iy0 &= HashMask;
			iz0 &= HashMask;
			int ix1 = ix0 + 1;
			int iy1 = iy0 + 1;
			int iz1 = iz0 + 1;

			int h0 = hash[ix0];
			int h1 = hash[ix1];
			int h00 = hash[h0 + iy0];
			int h10 = hash[h1 + iy0];
			int h01 = hash[h0 + iy1];
			int h11 = hash[h1 + iy1];
			Vector3 g000 = _gradients3D[hash[h00 + iz0] & GradientsMask3D];
			Vector3 g100 = _gradients3D[hash[h10 + iz0] & GradientsMask3D];
			Vector3 g010 = _gradients3D[hash[h01 + iz0] & GradientsMask3D];
			Vector3 g110 = _gradients3D[hash[h11 + iz0] & GradientsMask3D];
			Vector3 g001 = _gradients3D[hash[h00 + iz1] & GradientsMask3D];
			Vector3 g101 = _gradients3D[hash[h10 + iz1] & GradientsMask3D];
			Vector3 g011 = _gradients3D[hash[h01 + iz1] & GradientsMask3D];
			Vector3 g111 = _gradients3D[hash[h11 + iz1] & GradientsMask3D];

			float v000 = Dot(g000, tx0, ty0, tz0);
			float v100 = Dot(g100, tx1, ty0, tz0);
			float v010 = Dot(g010, tx0, ty1, tz0);
			float v110 = Dot(g110, tx1, ty1, tz0);
			float v001 = Dot(g001, tx0, ty0, tz1);
			float v101 = Dot(g101, tx1, ty0, tz1);
			float v011 = Dot(g011, tx0, ty1, tz1);
			float v111 = Dot(g111, tx1, ty1, tz1);

			float tx = tx0;
			float ty = ty0;
			float tz = tz0;

			if (smooth)
			{
				tx = Smooth(tx0);
				ty = Smooth(ty0);
				tz = Smooth(tz0);
			}

			float a = v000;
			float b = v100 - v000;
			float c = v010 - v000;
			float d = v001 - v000;
			float e = v110 - v010 - v100 + v000;
			float f = v101 - v001 - v100 + v000;
			float g = v011 - v001 - v010 + v000;
			float h = v111 - v011 - v101 + v001 - v110 + v010 + v100 - v000;

			return a + b * tx + (c + e * tx) * ty + (d + f * tx + (g + h * tx) * ty) * tz;
		}

		private static float SimplexValue1DPart (float ax, int ix) {
			float x = ax - ix;
			float f = 1f - x * x;
			float f2 = f * f;
			float f3 = f * f2;
			float h = hash[ix & HashMask];
			return h * f3;
		}

		public static float SimplexValue1D (float x) {
			int ix = Mathf.FloorToInt(x);
			float sample = SimplexValue1DPart(x, ix);
			sample += SimplexValue1DPart(x, ix + 1);
			return sample * (2f / HashMask) - 1f;
		}

		private static float SimplexValue2DPart (float ax, float ay, int ix, int iy) {
			float unskew = (ix + iy) * _squaresToTriangles;
			float x = ax - ix + unskew;
			float y = ay - iy + unskew;
			float f = 0.5f - x * x - y * y;
			if (f > 0f) {
				float f2 = f * f;
				float f3 = f * f2;
				float h = hash[hash[ix & HashMask] + iy & HashMask];
				return h * f3;
			}
			return 0f;
		}

		public static float SimplexValue2D (float x, float y) {
			float skew = (x + y) * _trianglesToSquares;
			float sx = x + skew;
			float sy = y + skew;
			int ix = Mathf.FloorToInt(sx);
			int iy = Mathf.FloorToInt(sy);
			float sample = SimplexValue2DPart(x, y, ix, iy);
			sample += SimplexValue2DPart(x, y, ix + 1, iy + 1);
			if (sx - ix >= sy - iy) {
				sample += SimplexValue2DPart(x, y, ix + 1, iy);
			}
			else {
				sample += SimplexValue2DPart(x, y, ix, iy + 1);
			}
			return sample * (8f * 2f / HashMask) - 1f;
		}

		private static float SimplexValue3DPart (float ax, float ay, float az, int ix, int iy, int iz) {
			float unskew = (ix + iy + iz) * (1f / 6f);
			float x = ax - ix + unskew;
			float y = ay - iy + unskew;
			float z = az - iz + unskew;
			float f = 0.5f - x * x - y * y - z * z;
			if (f > 0f) {
				float f2 = f * f;
				float f3 = f * f2;
				float h = hash[hash[hash[ix & HashMask] + iy & HashMask] + iz & HashMask];
				return h * f3;
			}
			return 0;
		}

		public static float SimplexValue3D (float ax, float ay, float az) {
			float skew = (ax + ay + az) * (1f / 3f);
			float sx = ax + skew;
			float sy = ay + skew;
			float sz = az + skew;
			int ix = Mathf.FloorToInt(sx);
			int iy = Mathf.FloorToInt(sy);
			int iz = Mathf.FloorToInt(sz);
			float sample = SimplexValue3DPart(ax, ay, az, ix, iy, iz);
			sample += SimplexValue3DPart(ax, ay, az, ix + 1, iy + 1, iz + 1);
			float x = sx - ix;
			float y = sy - iy;
			float z = sz - iz;
			if (x >= y) {
				if (x >= z) {
					sample += SimplexValue3DPart(ax, ay, az, ix + 1, iy, iz);
					if (y >= z) {
						sample += SimplexValue3DPart(ax, ay, az, ix + 1, iy + 1, iz);
					}
					else {
						sample += SimplexValue3DPart(ax, ay, az, ix + 1, iy, iz + 1);
					}
				}
				else {
					sample += SimplexValue3DPart(ax, ay, az, ix, iy, iz + 1);
					sample += SimplexValue3DPart(ax, ay, az, ix + 1, iy, iz + 1);
				}
			}
			else {
				if (y >= z) {
					sample += SimplexValue3DPart(ax, ay, az, ix, iy + 1, iz);
					if (x >= z) {
						sample += SimplexValue3DPart(ax, ay, az, ix + 1, iy + 1, iz);
					}
					else {
						sample += SimplexValue3DPart(ax, ay, az, ix, iy + 1, iz + 1);
					}
				}
				else {
					sample += SimplexValue3DPart(ax, ay, az, ix, iy, iz + 1);
					sample += SimplexValue3DPart(ax, ay, az, ix, iy + 1, iz + 1);
				}
			}
			return sample * (8f * 2f / HashMask) - 1f;
		}

		private static float Simplex1DPart (float ax, int ix) {
			float x = ax - ix;
			float f = 1f - x * x;
			float f2 = f * f;
			float f3 = f * f2;
			float g = _gradients1D[hash[ix & HashMask] & GradientsMask1D];
			float v = g * x;
			return v * f3;
		}

		public static float Simplex1D (float x) {
			int ix = Mathf.FloorToInt(x);
			float sample = Simplex1DPart(x, ix);
			sample += Simplex1DPart(x, ix + 1);
			return sample * (64f / 27f);
		}

		private static float Simplex2DPart (float ax, float ay, int ix, int iy) {
			float unskew = (ix + iy) * _squaresToTriangles;
			float x = ax - ix + unskew;
			float y = ay - iy + unskew;
			float f = 0.5f - x * x - y * y;
			if (f > 0f) {
				float f2 = f * f;
				float f3 = f * f2;
				Vector2 g = _gradients2D[hash[hash[ix & HashMask] + iy & HashMask] & GradientsMask2D];
				float v = Dot(g, x, y);
				return v * f3;
			}
			return 0;
		}

		public static float Simplex2D (float x, float y) {
			float skew = (x + y) * _trianglesToSquares;
			float sx = x + skew;
			float sy = y + skew;
			int ix = Mathf.FloorToInt(sx);
			int iy = Mathf.FloorToInt(sy);
			float sample = Simplex2DPart(x, y, ix, iy);
			sample += Simplex2DPart(x, y, ix + 1, iy + 1);
			if (sx - ix >= sy - iy) {
				sample += Simplex2DPart(x, y, ix + 1, iy);
			}
			else {
				sample += Simplex2DPart(x, y, ix, iy + 1);
			}
			return sample * _simplexScale2D;
		}

		/*private static float Simplex3DPart (Vector3 point, int ix, int iy, int iz) {
		float unskew = (ix + iy + iz) * (1f / 6f);
		float x = point.x - ix + unskew;
		float y = point.y - iy + unskew;
		float z = point.z - iz + unskew;
		float f = 0.5f - x * x - y * y - z * z;
		if (f > 0f) {
			float f2 = f * f;
			float f3 = f * f2;
			Vector3 g = simplexGradients3D[hash[hash[hash[ix & hashMask] + iy & hashMask] + iz & hashMask] & simplexGradientsMask3D];
			float v = Dot(g, x, y, z);
			return v * f3;
		}
		return 0;
	}

	public static float Simplex3D (Vector3 point, float frequency) {
		point *= frequency;
		float skew = (point.x + point.y + point.z) * (1f / 3f);
		float sx = point.x + skew;
		float sy = point.y + skew;
		float sz = point.z + skew;
		int ix = Mathf.FloorToInt(sx);
		int iy = Mathf.FloorToInt(sy);
		int iz = Mathf.FloorToInt(sz);
		float sample = Simplex3DPart(point, ix, iy, iz);
		sample += Simplex3DPart(point, ix + 1, iy + 1, iz + 1);
		float x = sx - ix;
		float y = sy - iy;
		float z = sz - iz;
		if (x >= y) {
			if (x >= z) {
				sample += Simplex3DPart(point, ix + 1, iy, iz);
				if (y >= z) {
					sample += Simplex3DPart(point, ix + 1, iy + 1, iz);
				}
				else {
					sample += Simplex3DPart(point, ix + 1, iy, iz + 1);
				}
			}
			else {
				sample += Simplex3DPart(point, ix, iy, iz + 1);
				sample += Simplex3DPart(point, ix + 1, iy, iz + 1);
			}
		}
		else {
			if (y >= z) {
				sample += Simplex3DPart(point, ix, iy + 1, iz);
				if (x >= z) {
					sample += Simplex3DPart(point, ix + 1, iy + 1, iz);
				}
				else {
					sample += Simplex3DPart(point, ix, iy + 1, iz + 1);
				}
			}
			else {
				sample += Simplex3DPart(point, ix, iy, iz + 1);
				sample += Simplex3DPart(point, ix, iy + 1, iz + 1);
			}
		}
		return sample * simplexScale3D;
	}*/
	}
}