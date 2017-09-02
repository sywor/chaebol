using System;
using System.Collections.Generic;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.ColorMap
{
	[Serializable]
	[GraphContextMenuItem("ColorMap", "Scatter")]

	/*[NodeHelpText("Use this node to add texture height maps to your landscape.\n"
				  + "The <b>ScatterColorMapNode</b> scatters the gray scale of a color map\n"
				  + "to the positions assigned by the vector input. Overlapping values\n"
				  + "are added to each other. A mask can be used to mask the scattering.\n"
				  + "You can fade the color maps according to the mask or cut them.\n \n"

	              + "<b><color=#FF3333ff>Performance:</color></b>\n"
	              + "The node requests neighbouring vector positions for every requested value.\n"
				  + "This can impact performance significantly. Therefore use a mask to avoid \n"
				  + "those requests for areas where you need no scattering.\n"
				  + "Keep the color map size low.\n"
				  + "Performance can also be improved by using the fade option.\n \n"

				  + "<b><color=#0000ffff>Output:</color></b>\n"
				  + "The gray scale value or float.NaN if no color map is scattered at.\n"
				  + "the position. \n \n"

				  + "<b><color=#005500ff>Input:</color></b>\n"
				  + "<b>map:</b> The color map to scatter.\n"
				  + "<b>vec:</b> The positions to scatter to (x, z).\n"
				  + "<b>mask:</b> A mask for scattering (mask < 0 is not scattered). \n \n"

				  + "<b><color=#00AAAAff>Option:</color></b>\n"
				  + "<b>fade:</b> Fades the gray scale values if checked (faster).\n"
	              + "          Cuts a color map if it is overlaping\n"
	              + "          mask value lower than 0 (slower).\n \n"

	              + "<b><color=#800080ff>Internal Request:</color></b>\n"
	              + "Uses <i>x</i> and <i>z</i> for the positions.\n"
	)]*/
	public class ScatterColorMapNode : AbstractNumberNode
	{

		private InputSocket _inputSocketColorMap;
		private InputSocket _inputSocketPoints;
		private InputSocket _inputSocketMask;

		[SerializeField] private bool _fadeMask = true;

		private Rect _tmpRect;
		private UnityEngine.Color[,] _colorMap;

		public ScatterColorMapNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketColorMap = new InputSocket(this, typeof(IColorMapConnection));
			_inputSocketPoints = new InputSocket(this, typeof(IVectorConnection));
			_inputSocketMask = new InputSocket(this, typeof(INumberConnection));
			_inputSocketMask.SetDirectInputNumber(1, false);
			Sockets.Add(_inputSocketColorMap);
			Sockets.Add(_inputSocketPoints);
			Sockets.Add(_inputSocketMask);
			Sockets.Add(new OutputSocket(this, typeof(INumberConnection)));
			Height = 80;
			Width = 110;
		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRect.Set(3, 0, 80, 20);
			GUI.Label(_tmpRect, "map");
			_tmpRect.Set(3, 20, 80, 20);
			GUI.Label(_tmpRect, "vec");
			_tmpRect.Set(3, 40, 80, 20);
			GUI.Label(_tmpRect, "mask");

			_tmpRect.Set(60, 40, 80, 20);
			var currentFadeMask = GUI.Toggle(_tmpRect, _fadeMask, "fade");

			if (currentFadeMask != _fadeMask)
			{
				_fadeMask = currentFadeMask;
				TriggerChangeEvent();
			}

			GUI.skin.label.alignment = TextAnchor.MiddleRight;
			_tmpRect.Set(Width - 80, 0, 80, 20);
			GUI.Label(_tmpRect, "height map");

			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{
			_colorMap = AbstractColorMapNode.GetInputColorMap(_inputSocketColorMap, new Request());
		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{

			if (_colorMap == null) return float.NaN;

			float mask = GetInputNumber(_inputSocketMask, request);
			if (float.IsNaN(mask)) return float.NaN;
			if (mask < 0) return float.NaN;

			int width = _colorMap.GetLength(0);
			int height = _colorMap.GetLength(1);

			float halfWidth = Mathf.Ceil(width / 2f);
			float halfHeight = Mathf.Ceil(height / 2f);

			Request r = new Request();
			r.X = request.X - width * 1.5f - 1;
			r.Y = request.Y;
			r.Z = request.Z - height * 1.5f - 1;
			r.SizeX = width * 3 + 1;
			r.SizeZ = height * 3 + 1;
			r.Seed = request.Seed;

			List<UnityEngine.Vector3> points = AbstractVector3Node.GetInputVector3List(_inputSocketPoints, request);

			if (points == null) return float.NaN;

			float value = float.NaN;
			bool initialized = false;
			for (int index = 0; index < points.Count; index++)
			{
				var point = points[index];
				if (!_fadeMask && !AllPositionsCoveredByMask(
					    (int) (point.x - halfWidth),
					    (int) (point.z - halfHeight), width, height, request.Y, request.Seed)) continue;


				int mapRelativeX = (int) (halfWidth - (point.x - request.X));
				int mapRelativeZ = (int) (halfHeight - (point.z - request.Z));
				if (mapRelativeX < width && mapRelativeX > 0 && mapRelativeZ < height && mapRelativeZ > 0)
				{
					if (!initialized)
					{
						value = 0;
						initialized = true;
					}
					value += _colorMap[mapRelativeX, mapRelativeZ].grayscale;
				}
			}

			if (_fadeMask) value = value * mask;
			return value;
		}

		private bool AllPositionsCoveredByMask(int x, int z, int width, int height, float y, float seed)
		{
			Request request = new Request();
			for (var maskX = x; maskX < x + width; maskX++)
			{
				for (var maskZ = z; maskZ < z + height; maskZ++)
				{
					request.X = maskX;
					request.Y = y;
					request.Z = maskZ;
					request.Seed = seed;
					float mask = GetInputNumber(_inputSocketMask, request);
					if (float.IsNaN(mask) || mask < 0) return false;
				}
			}
			return true;
		}
	}
}
