using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.HeightMap
{
	[Serializable]
	[GraphContextMenuItem("HeightMap", "Gradient Step")]
	public class GradientStepNode : AbstractNumberNode {

		private InputSocket _inputSocketValue;
		private InputSocket _inputSocketSteps;
		private InputSocket _inputSocketGradientFactor;

		private Rect _tmpRect;

		public GradientStepNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketValue = 			new InputSocket(this, typeof(INumberConnection));
			_inputSocketSteps = 			new InputSocket(this, typeof(INumberConnection));
			_inputSocketGradientFactor = 	new InputSocket(this, typeof(INumberConnection));
			_inputSocketGradientFactor.SetDirectInputNumber(0, false);
			_inputSocketSteps.SetDirectInputNumber(2, false);

			Sockets.Add(_inputSocketValue);
			Sockets.Add(_inputSocketSteps);
			Sockets.Add(_inputSocketGradientFactor);
			Sockets.Add(new OutputSocket(this, typeof(INumberConnection)));
			Height = 80;
		}

		protected override void OnGUI()
		{
			_tmpRect.Set(3, 0, 50, 20);
			GUI.Label(_tmpRect, "value");
			_tmpRect.Set(3, 20, 50, 20);
			GUI.Label(_tmpRect, "steps");
			_tmpRect.Set(3, 40, 80, 20);
			GUI.Label(_tmpRect, "gradient fac.");
		}

		public override void Update()
		{

		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			float value = GetInputNumber(_inputSocketValue, request);
			float steps = GetInputNumber(_inputSocketSteps, request);
			float gradientFactor = GetInputNumber(_inputSocketGradientFactor, request);
			if (float.IsNaN(value) || float.IsNaN(steps) || steps < 1) return float.NaN;
			return Calc(value, steps, gradientFactor);
		}

		public static float Calc(float value, float steps, float gradientFactor)
		{
			if (float.IsNaN(gradientFactor)) gradientFactor = 0;
			if (steps == 1) return 0;

			float singleStep = 2f / (steps - 1);
			float halfStep = singleStep / 2f;
			float gradientSize = singleStep * gradientFactor;
			float halfGradientSize = gradientSize / 2f;

			for (int i = 0; i < steps; i++)
			{
				float currentStep = -1 + i * singleStep;
				float stepOffset = currentStep - value;

				if (gradientFactor > 0)
				{
					float currentPlusHalf = currentStep + halfStep;
					bool isToUpperGradient = value > currentPlusHalf - halfGradientSize && value < currentPlusHalf;
					bool isToLowerGradient = value < currentPlusHalf + halfGradientSize && value > currentPlusHalf;
					if (isToUpperGradient) return currentPlusHalf - Math.Abs(stepOffset + halfStep) / gradientFactor;
					if (isToLowerGradient) return currentPlusHalf + Math.Abs(stepOffset + halfStep) / gradientFactor;
				}

				if (Math.Abs(stepOffset) <= halfStep) return currentStep;
			}
			return float.NaN;
		}
	}
}
