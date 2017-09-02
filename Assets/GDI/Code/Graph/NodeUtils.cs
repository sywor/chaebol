using System.Text.RegularExpressions;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEditor;
using UnityEngine;

namespace Assets.GDI.Code.Graph
{
	/// <summary>
	/// This class contains static helper functions for GDI.
	/// </summary>
	public class NodeUtils
	{

		public const string NotConnectedMessage = "not connected";
		private static Texture2D _staticRectTexture;
		private static GUIStyle _staticRectStyle;

		private static Vector2 _tmpVec01;
		private static Vector2 _tmpVec02;

		/// <summary>
		/// Draws a textfield that accepts float inputs only.
		/// Returns true if the value has changed.
		/// </summary>
		/// <param name="area">The area to draw.</param>
		/// <param name="number">The number to display.</param>
		/// <returns>True if the value has changed.</returns>
		public static bool FloatTextField(Rect area, ref string number)
		{
			if (number == null) return false;
			GUI.SetNextControlName(Config.TextFieldControlName);
			var textFieldValue = GUI.TextField(area, number);
			var newTextFieldValue = GetValidNumberString(textFieldValue, number);
			var numberChanged = !IsEqualFloatValue(newTextFieldValue, number);
			number = newTextFieldValue;
			if (numberChanged) Event.current.Use();
			return numberChanged;
		}

		/// <summary>
		/// Returns a string that represents a valid float number.
		/// </summary>
		/// <param name="text">The text to get the number string from.</param>
		/// <param name="defaultNumber">A default value to return if the text is not parsable.</param>
		/// <returns>The number string or the default number.</returns>
		public static string GetValidNumberString(string text, string defaultNumber)
		{
			if (text == "-") return text;
			if (text == "") text = "0";
			if (Regex.Match(text, @"^-?[0-9]*(?:\.[0-9]*)?$").Success) return text;
			return defaultNumber;
		}

		/// <summary>
		/// Returns true if the assigned strings are equal float values.
		/// </summary>
		/// <param name="number01">First number string.</param>
		/// <param name="number02">Second number string.</param>
		/// <returns>True if the numbers are equal.</returns>
		public static bool IsEqualFloatValue(string number01, string number02)
		{
			return !(Mathf.Abs(Parse(number01) - Parse(number02)) > 0);
		}

		public static Color GetGreyscaleColor(float value)
		{
			if (float.IsNaN(value)) return Color.magenta;
			if (value > 1f) return Color.red;
			if (value < -1f) return Color.blue;
			Color c = Color.white * (value + 1f) / 2f;
			c.a = 1;
			return c;
		}

		public static Color[] ToColorMap(float[,] values, IColorConnection colorSampler = null, OutputSocket colorRequestSocket = null)
		{
			int width = values.GetLength(0);
			int height = values.GetLength(1);
			Color[] colorMap = new Color[width * height];
			Request request = new Request();
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					Color c;
					if (colorSampler == null) c = GetGreyscaleColor(values[x, y]);
					else
					{
						request.X = x;
						request.Y = (values[x, y] + 1f) / 2f;
						request.Z = y;
						c = colorSampler.GetColor(colorRequestSocket, request);
					}
					colorMap[y * width + x] = c;
				}
			}
			return colorMap;
		}

		public static Color[] ToTextureColorMap(Color[,] colors)
		{
			int width = colors.GetLength(0);
			int height = colors.GetLength(1);
			Color[] colorMap = new Color[width * height];
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					colorMap[y * width + x] = colors[x, y];
				}
			}
			return colorMap;
		}


		public static void GUIDrawRect(Rect position, Color color)
		{
			if (_staticRectTexture == null) _staticRectTexture = new Texture2D(1, 1);
			if (_staticRectStyle == null) _staticRectStyle = new GUIStyle();
			_staticRectTexture.SetPixel(0, 0, color);
			_staticRectTexture.Apply();
			_staticRectStyle.normal.background = _staticRectTexture;
			GUI.Box(position, GUIContent.none, _staticRectStyle);
		}

		public static float ModifySeed(float baseSeed, float modifierSeed)
		{
			if (modifierSeed == 0) return baseSeed;
			if (baseSeed == 0) return modifierSeed;
			return baseSeed * modifierSeed % float.MaxValue;
		}


		public static bool LineIntersectsLine(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
		{
			float s1_x, s1_y, s2_x, s2_y;
			s1_x = p1.x - p0.x;
			s1_y = p1.y - p0.y;
			s2_x = p3.x - p2.x;
			s2_y = p3.y - p2.y;
			float s, t;
			s = (-s1_y * (p0.x - p2.x) + s1_x * (p0.y - p2.y)) / (-s2_x * s1_y + s1_x * s2_y);
			t = (s2_x * (p0.y - p2.y) - s2_y * (p0.x - p2.x)) / (-s2_x * s1_y + s1_x * s2_y);

			if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
			{
				//return new Vector2(p0.x + (t * s1_x), p0.y + (t * s1_y));
				return true;
			}
			return false;
		}

		public static bool LineIntersectsRect(Vector2 lineP0, Vector2 lineP1, Rect rect)
		{
			_tmpVec01.Set(rect.x, rect.y);
			_tmpVec02.Set(rect.x, rect.y + rect.height);
			if (LineIntersectsLine(lineP0, lineP1, _tmpVec01, _tmpVec02)) return true;

			_tmpVec01.Set(rect.x, rect.y);
			_tmpVec02.Set(rect.x + rect.width, rect.y);
			if (LineIntersectsLine(lineP0, lineP1, _tmpVec01, _tmpVec02)) return true;

			_tmpVec01.Set(rect.x + rect.width, rect.y);
			_tmpVec02.Set(rect.x + rect.width, rect.y + rect.height);
			if (LineIntersectsLine(lineP0, lineP1, _tmpVec01, _tmpVec02)) return true;

			_tmpVec01.Set(rect.x, rect.y + rect.height);
			_tmpVec02.Set(rect.x + rect.width, rect.y + rect.height);
			if (LineIntersectsLine(lineP0, lineP1, _tmpVec01, _tmpVec02)) return true;
			return false;
		}

		public static void DrawVerticalLine(Vector2 start, Vector2 end, Color color, string endText, string startCoordinate,
			string endCoordinate)
		{
			EditorGUI.DrawRect(new Rect(start.x, start.y, end.x - start.x + 2, end.y - start.y), color);
			EditorGUI.DrawRect(new Rect(end.x - 6, end.y, 6, 2), color);
			EditorGUI.DrawRect(new Rect(start.x - 6, start.y, 6, 2), color);

			GUI.Label(new Rect(end.x - 10, end.y - 17, 20, 20), endText);

			GUI.skin.label.alignment = TextAnchor.MiddleRight;
			GUI.Label(new Rect(end.x - 50, end.y - 10, 44, 20), endCoordinate);
			GUI.Label(new Rect(start.x - 50, start.y - 10, 44, 20), startCoordinate);
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public static void DrawHorizontalLine(Vector2 start, Vector2 end, Color color, string endText, string startCoordinate,
			string endCoordinate)
		{
			EditorGUI.DrawRect(new Rect(start.x, start.y, end.x - start.x, end.y - start.y + 2), color);
			EditorGUI.DrawRect(new Rect(start.x - 2, start.y, 2, 6), color);
			EditorGUI.DrawRect(new Rect(end.x - 2, end.y, 2, 6), color);

			GUI.Label(new Rect(end.x - 3, end.y - 10, 20, 20), endText);

			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.Label(new Rect(end.x - 22, end.y + 4, 44, 20), endCoordinate);
			GUI.Label(new Rect(start.x - 22, start.y + 4, 44, 20), startCoordinate);
		}

		public static float Parse(string text)
		{
			if (text == "-") return 0;
			return float.Parse(text);
		}

		public static Texture2D MakeTex(int width, int height, Color col)
		{
			Color[] pix = new Color[width * height];

			for (int i = 0; i < pix.Length; i++)
			{
				pix[i] = col;
			}


			Texture2D result = new Texture2D(width, height);
			result.SetPixels(pix);
			result.Apply();

			return result;
		}
	}
}
