using Assets.GDI.Code;
using UnityEditor;
using UnityEngine;

namespace Assets.GDI.Editor.GDI
{
	public static class AboutPanel
	{

		public static bool Show;

		private static GUIStyle _headline1;
		private static GUIStyle _headline2;

		private static Texture2D _logoTexture;

		public static void Draw(float widht, float height)
		{
			if (!Show) return;
			Rect backButton = new Rect(1, 1, 100, 30);
			EditorGUI.DrawRect(backButton, Color.gray);
			GUI.Label(backButton, "back");
			if (backButton.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp){
				Event.current.Use();
				Show = false;
				return;
			}
			InitStyles();

			GUILayout.BeginArea(new Rect(0, 40, widht, height - 40));

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Box(_logoTexture);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Graph Development Interface \n", _headline1);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Version " + Config.Version);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Made for Unity 5.4");
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Copyright  Luca Hofmann");
			GUILayout.EndHorizontal();

			GUILayout.Space(30);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Notice:", _headline2);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("This tool gives you the freedom to create complex graphs that can lead to heavy computations.");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("This can cause Unity to freeze or even crash. Make sure to frequently save your progress to avoid");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("loss of work. Read the node help text to learn more about performance optimizations of your graphs.");
			GUILayout.EndHorizontal();

			GUILayout.Space(30);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Used software:", _headline2);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Based on 'Brotherhood of Node' (MIT Licence).");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Some noise nodes are based on Jasper Flick tutorials.");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Unity vector color shader by @defaxer");
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		private static void InitStyles()
		{
			if (_headline1 == null)
			{
				_headline1 = new GUIStyle();
				_headline1.fontSize = 16;
				_headline1.alignment = TextAnchor.MiddleCenter;
				_headline1.fontStyle = FontStyle.Bold;
				_headline1.normal.textColor = Color.white;

				_headline2 = new GUIStyle(_headline1);
				_headline2.fontSize = 12;
			}
			if (_logoTexture == null)
			{
				_logoTexture = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/GDI/Resources/Textures/GDI_Logo.png", typeof(Texture2D));
			}
		}
	}
}
