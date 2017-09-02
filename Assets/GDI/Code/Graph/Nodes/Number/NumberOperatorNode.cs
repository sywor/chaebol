﻿using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Number
{

	[Serializable]
	[GraphContextMenuItem("Number", "Operator")]
	public class NumberOperatorNode : AbstractNumberNode
	{

		[SerializeField] private int _selectedMode;

		[NonSerialized]  public static  string[] Operations = { "add", "sub", "mul", "div" };
		[NonSerialized] private InputSocket _inputSocket01;
		[NonSerialized] private InputSocket _inputSocket02;

		public NumberOperatorNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocket01 = new InputSocket(this, typeof(INumberConnection));
			Sockets.Add(_inputSocket01);
			_inputSocket02 = new InputSocket(this, typeof(INumberConnection));
			Sockets.Add(_inputSocket02);
			Sockets.Add(new OutputSocket(this, typeof(INumberConnection)));
			Height = 95;
			Width = 65;
		}

		protected override void OnGUI()
		{

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			int newMode = GUILayout.SelectionGrid(_selectedMode,Operations,1,"toggle");
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
			var value01 = GetInputNumber(_inputSocket01, request);
			var value02 = GetInputNumber(_inputSocket02, request);
			if (!float.IsNaN(value01) && !float.IsNaN(value02)) return Calculate(value01, value02);
			return float.NaN;
		}

		public float Calculate(float value01, float value02)
		{
			if (_selectedMode == 0) return value01 + value02;
			if (_selectedMode == 1) return value01 - value02;
			if (_selectedMode == 2) return value01 * value02;
			if (_selectedMode == 3)
			{
				if (value02.Equals(0)) return float.NaN;
				return value01 / value02;
			}
			return float.NaN;
		}

		public void SetMode(Operator o)
		{
			if (o == Operator.Add) _selectedMode = 0;
			if (o == Operator.Substract) _selectedMode = 1;
			if (o == Operator.Multiply) _selectedMode = 2;
			if (o == Operator.Divide) _selectedMode = 3;
		}
	}

	public enum Operator
	{
		Add,
		Substract,
		Multiply,
		Divide
	}
}
