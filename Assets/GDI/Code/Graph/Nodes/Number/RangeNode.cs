using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Number
{

	[Serializable]
	[GraphContextMenuItem("Number", "Range")]
	public class RangeNode : AbstractNumberNode
	{


		[SerializeField] private int _selectedMode;

		[NonSerialized] private InputSocket _inputSocket01;
		[NonSerialized] public static string[] Modes = {"[-1:1] to [0:1]", "[-1:1] to [-1:0]", "[0:1] to [-1:1]"};

		public RangeNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocket01 = new InputSocket(this, typeof(INumberConnection));
			Sockets.Add(_inputSocket01);
			Sockets.Add(new OutputSocket(this, typeof(INumberConnection)));
			Height = 80;
			Width = 100;
		}

		protected override void OnGUI()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			int newMode = GUILayout.SelectionGrid(_selectedMode, Modes, 1, "toggle");
			if (newMode != _selectedMode)
			{
				_selectedMode = newMode;
				TriggerChangeEvent();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		public override void Update()
		{

		}


		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			float value = GetInputNumber(_inputSocket01, request);
			if (float.IsNaN(value)) return float.NaN;

			if (_selectedMode == 0) return (value + 1f) / 2f;
			if (_selectedMode == 2) return value * 2f - 1f;
			return (value + 1f) / 2f - 1;
		}
	}
}
