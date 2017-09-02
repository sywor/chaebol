using System.Collections.Generic;
using Assets.GDI.Code.Tools.Thread;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes
{
	public abstract class AbstractNoiseNode : AbstractNumberNode
	{
		protected List<GUIThreadedTexture> _textures;
		private Rect _errorMessageLabel;

		protected AbstractNoiseNode(int id, Graph parent) : base(id, parent)
		{
			_errorMessageLabel = new Rect(3, 0, 100, 15);
			_textures = new List<GUIThreadedTexture>();
		}


		protected void DrawTextures()
		{
			for (var i = 0; i < _textures.Count; i++) _textures[i].OnGUI();
			if (IsUpdatingTexture()) GUI.Label(_errorMessageLabel, "updating data..");
		}

		protected bool IsUpdatingTexture()
		{
			for (int index = 0; index < _textures.Count; index++)
			{
				GUIThreadedTexture t = _textures[index];
				if (t.IsUpdating) return true;
			}
			return false;
		}

	}
}
