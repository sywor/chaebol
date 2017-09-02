using UnityEditor;

namespace Assets.GDI.Code.Chunk
{
	[CustomEditor(typeof(Launcher))]
	public class LauncherInspector : Editor {

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
		}

	}
}
