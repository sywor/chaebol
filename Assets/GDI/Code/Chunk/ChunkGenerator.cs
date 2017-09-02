using UnityEditor;
using UnityEngine;

namespace Assets.GDI.Code.Chunk
{
	[ExecuteInEditMode]
	public class ChunkGenerator : MonoBehaviour
	{
		void OnRenderObject ()
		{
			UpdateJob();
		}

		void OnEnable()
		{
			EditorApplication.update += UpdateJob;
		}

		public void Update()
		{
			UpdateJob();
		}

		private void EditorUpdate()
		{
			// if (!Application.isPlaying) { }
			UpdateJob();
		}

		private void UpdateJob()
		{
			//_jobQueue.Update();
		}

		void OnDrawGizmos()
		{

		}

		void OnDrawGizmosSelected()
		{
			/*Gizmos.color = Color.yellow;
			_gizmoVector.Set(transform.position.x, 10000, transform.position.z);
			Gizmos.DrawLine(transform.position, _gizmoVector);*/
		}
	}
}
