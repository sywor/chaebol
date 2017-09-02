using Assets.GDI.Code;
using UnityEditor;
using UnityEngine;

namespace Assets.GDI.Editor.GDI
{
	public static class PreferencesPanel {

		public static bool Show;

		private static GUIStyle _headline1;
		private static GUIStyle _headline2;

		private static GUIContent[] _logOptions;
		private static GUIStyle _richtText;

		public static void Draw(float widht, float height)
		{
			if (!Show) return;
			Rect backButton = new Rect(1, 1, 100, 30);
			EditorGUI.DrawRect(backButton, Color.gray);
			GUI.Label(backButton, "back");
			if (backButton.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp)
			{
				Event.current.Use();
				Show = false;
				return;
			}
			InitStyles();



			GUILayout.BeginArea(new Rect(0, 40, widht, height - 40));
			GUILayout.BeginVertical(GUILayout.Width(400));

			GUILayout.BeginHorizontal();
			GUILayout.Label("Controls", _headline1);
			GUILayout.EndHorizontal();
			GUILayout.Space(15);

			GUILayout.BeginHorizontal();
			GUILayout.Label("<b>Middle mouse button + drag:</b> Drag the canvas. ", _richtText);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("<b>Scroll :</b> Zoom the canvas.", _richtText);
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			GUILayout.Label("<b>Right click on canvas:</b> Open node adding conext menu. ", _richtText);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("<b>Right click on node:</b> Open node conext menu. ", _richtText);
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			GUILayout.Label("<b>Left click on node:</b> Select node.", _richtText);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("<b>Left click on node + drag:</b> Move node.", _richtText);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("<b>Left click on canvas + drag:</b> Selection box.", _richtText);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("<b>Left click on node + shift:</b> Add node to selection.", _richtText);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("<b>Press 0:</b> Reset zoom.", _richtText);
			GUILayout.EndHorizontal();


			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			GUILayout.Label("<b>Left click on edge:</b> Add edge path point.", _richtText);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("<b>Drag edge path point:</b> Move egde path point.", _richtText);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("<b>Right click on edge path point:</b> Remove point.", _richtText);
			GUILayout.EndHorizontal();



			GUILayout.Space(30);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Settings", _headline1);
			GUILayout.EndHorizontal();
			GUILayout.Space(15);

			GUILayout.BeginHorizontal();
			GUILayout.Label("Logging");
			int selectedLogOption = GUILayout.SelectionGrid(Config.LogLevel, _logOptions, 2);
			if (selectedLogOption != Config.LogLevel)
			{
				Config.LogLevel = selectedLogOption;
				SaveConfig();
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			bool showEdgetooltip = GUILayout.Toggle(Config.ShowEdgeHover, "show edge tooltip");
			if (showEdgetooltip != Config.ShowEdgeHover)
			{
				Config.ShowEdgeHover = showEdgetooltip;
				SaveConfig();
			}
			GUILayout.EndHorizontal();


			GUILayout.BeginHorizontal();
			bool showSockettooltip = GUILayout.Toggle(Config.ShowSocketHover, "show socket tooltip");
			if (showSockettooltip != Config.ShowSocketHover)
			{
				Config.ShowSocketHover = showSockettooltip;
				SaveConfig();
			}
			GUILayout.EndHorizontal();


			GUILayout.EndVertical();
			GUILayout.EndArea();
		}

		private static void SaveConfig()
		{

		}

		private static void InitStyles()
		{
			if (_headline1 == null)
			{
				_headline1 = new GUIStyle();
				_headline1.fontSize = 18;
				_headline1.alignment = TextAnchor.MiddleCenter;
				_headline1.fontStyle = FontStyle.Bold;
				_headline1.normal.textColor = Color.white;

				_headline2 = new GUIStyle(_headline1);
				_headline2.fontSize = 16;

				_richtText = new GUIStyle(_headline1);
				_richtText.richText = true;
				_richtText.fontSize = 12;
				_richtText.fontStyle = FontStyle.Normal;
				_richtText.normal.textColor = GUI.skin.label.normal.textColor;

				var logOptionError = new GUIContent("error");
				var logOptionInfo = new GUIContent("info");
				_logOptions = new GUIContent[] {logOptionError, logOptionInfo};


			}


		}
	}
}
