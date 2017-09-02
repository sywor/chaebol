using UnityEngine;

namespace Assets.GDI.Editor.GDI
{
	public class EditorZoomArea
	{

		private static Matrix4x4 _prevGuiMatrix;
		private static Vector3 _tmpScaleVector = new Vector3();
		private static Rect _tmpRect = new Rect();

		public static Rect Begin(float zoomScale, Rect screenCoordsArea)
		{
			GUI.EndGroup();
			Rect clippedArea = screenCoordsArea.ScaleSizeBy(1.0f / zoomScale, screenCoordsArea.TopLeft());
			clippedArea.y += GDIWindow.TopOffset;
			GUI.BeginGroup(clippedArea);
			_prevGuiMatrix = GUI.matrix;
			Matrix4x4 translation = Matrix4x4.TRS(clippedArea.TopLeft(), Quaternion.identity, Vector3.one);
			_tmpScaleVector.Set(zoomScale, zoomScale, 1.0f);
			Matrix4x4 scale = Matrix4x4.Scale(_tmpScaleVector);
			GUI.matrix = translation * scale * translation.inverse * GUI.matrix;
			return clippedArea;
		}

		public static void End()
		{
			GUI.matrix = _prevGuiMatrix;
			GUI.EndGroup();
			_tmpRect.Set(0, GDIWindow.TopOffset, Screen.width, Screen.height);
			GUI.BeginGroup(_tmpRect);
		}
	}
}