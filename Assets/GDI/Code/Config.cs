using UnityEngine;

namespace Assets.GDI.Code
{
	/// <summary>
	/// A class that contains static members to configure GDI.
	/// </summary>
	public static class Config
	{
		public static string Version = "1";

		[SerializeField] public static int LogLevel = 0;

		public static int SocketSize = 15;
		public static int SocketOffsetTop = 20;
		public static int SocketMargin = 5;
		public static int EdgeTangent = 50;
		public static int PathPointSize = 8;

		[SerializeField] public static bool ShowEdgeHover = true;
		[SerializeField] public static bool ShowSocketHover = true;
		[SerializeField] public static string GameObjectName = "GDI";

		public static string DefaultGraphName = "default";
		public const string WindowName = "GDI";

		public const string TextFieldControlName = "FloatTextField";
		public const string NullControlName = "NullControl";

		public static string DocuPath = "/GDI/Resources/Documentation";
		public static string ExamplesPath = "/GDI/Resources/Examples";

		public static Color SelectedNodeColor = new Color(55f / 255, 55f / 255, 55f / 255, 1);
		public static Color NodeColor = new Color(33f / 255, 33f / 255, 33f / 255, 1);

		public static Color SelectedTabColor = new Color(99 / 255f, 99 / 255f, 99 / 255f, 1);
		public static Color TabColor = new Color(66 / 255f, 66 / 255f, 66 / 255f, 1);

		public static Color ArrowX = new Color(251 / 255f, 73 / 255f, 31 / 255f, 1);
		public static Color ArrowZ = new Color(44 / 255f, 88 / 255f, 206 / 255f, 1);
		public static Color ArrowY = new Color(140 / 255f, 238 / 255f, 62 / 255f, 1);

	}
}


