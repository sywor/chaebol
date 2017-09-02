using System;
using System.Collections.Generic;
using System.Linq;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using Assets.GDI.Code.Tools;
using UnityEditor;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Number
{

	[Serializable]
	[GraphContextMenuItem("Number", "Curve")]
	public class CurveNode : AbstractNumberNode
	{

		[SerializeField] private float _panelWidth = 200;
		[SerializeField] private float _panelHeight = 200;
		[SerializeField] private bool _clamp = true;
		[SerializeField] private int _selectedLimitOption;
		[SerializeField] private List<Vector2> _points;

		private const int PointSize = 6;

		private bool _startedDragingInPanel;

		private static UnityEngine.Color _panelColor = UnityEngine.Color.black;
		private static UnityEngine.Color _pointColor = UnityEngine.Color.white;
		private static UnityEngine.Color _pointHoverColor = UnityEngine.Color.gray;
		private static UnityEngine.Color _pointSelectColor = UnityEngine.Color.blue;
		private static UnityEngine.Color _limitColor = new UnityEngine.Color(1, 0, 1, 0.1f);

		private static UnityEngine.Color _coordinateColor01 = new UnityEngine.Color(0.4f, 0.4f, 0.4f);
		private static UnityEngine.Color _coordinateColor02 = new UnityEngine.Color(0.1f, 0.1f, 0.1f);

		private string[] _limitOptions = {"norm", "cut"};


		private UnityEngine.Vector3 _tmpVec0 = new UnityEngine.Vector3();
		private UnityEngine.Vector3 _tmpVec1 = new UnityEngine.Vector3();

		private Vector2 _tmpVec2 = new UnityEngine.Vector3();
		private Vector2 _tmpVec3 = new UnityEngine.Vector3();

		private float _panelMarginLeftRight = 20;
		private float _panelMarginTop = 14;

		private InputSocket _inputSocketValue;

		private int _dragPointIndex = -1;
		private int _hoverPointIndex = -1;
		private int _selectedPointIndex;

		private BSpline _spline;
		private Rect _tmpRect;
		private Rect _panelRect;

		private string _pointXInput = "0";
		private string _pointYInput = "0";


		public CurveNode(int id, Graph parent) : base(id, parent)
		{
			Resizable = true;
			_inputSocketValue = new InputSocket(this, typeof(INumberConnection));
			Sockets.Add(_inputSocketValue);
			Sockets.Add(new OutputSocket(this, typeof(INumberConnection)));

			_tmpRect = new Rect();
			_panelRect = new Rect();
			_points = new List<Vector2>();
			_points.Add(new Vector2(-1.0f, -1.0f));
			_points.Add(new Vector2(0f, 0f));
			_points.Add(new Vector2(1.0f, 1.0f));

			_spline = new BSpline(_points);
			_selectedPointIndex = 0;

			SocketTopOffsetInput = _panelHeight + +_panelMarginTop + 20;
		}

		protected override void OnGUI()
		{
			_hoverPointIndex = GetPointAt(Event.current.mousePosition);

			DrawUI();

			Width = _panelWidth + _panelMarginLeftRight * 2;
			Height = _panelHeight + _panelMarginTop + 90;
			_panelRect.Set(_panelMarginLeftRight, _panelMarginTop, _panelWidth, _panelHeight);
			EditorGUI.DrawRect(_panelRect, _panelColor);

			NodeUtils.DrawVerticalLine(
				new Vector2(_panelMarginLeftRight - 2, _panelHeight + _panelMarginTop),
				new Vector2(_panelMarginLeftRight - 2, _panelMarginTop),
					UnityEngine.Color.yellow, "out", "-1", "1");

			NodeUtils.DrawHorizontalLine(
				new Vector2(_panelMarginLeftRight, _panelHeight + _panelMarginTop),
				new Vector2(_panelMarginLeftRight + _panelWidth, _panelHeight + _panelMarginTop),
				UnityEngine.Color.cyan, "in", "-1", "1");

			DrawCoordinateSytem();
			DrawLine();
			for (int i = 0; i < _points.Count; i++)
			{
				DrawPoint(i);
			}

			HandleMouseEvents();
		}

		private void DrawUI()
		{
			float topOffset = _panelHeight + _panelMarginTop + 20;

			if (Event.current.type == EventType.MouseUp)
			{
				TriggerChangeEvent();
			}

			if (_selectedPointIndex >= _points.Count)
			{
				_selectedPointIndex = -1;
			}

			if (_selectedPointIndex > -1)
			{
				Vector2 v = _points[_selectedPointIndex];
				_pointXInput = v.x + "";
				_pointYInput = v.y + "";

				_tmpRect.Set(10, topOffset, 20, 18);
				GUI.Label(_tmpRect, "x");
				_tmpRect.Set(20, _panelHeight + _panelMarginTop + 20, 40, 18);
				if (NodeUtils.FloatTextField(_tmpRect, ref _pointXInput))
				{
					v.x = Mathf.Max(Mathf.Min(NodeUtils.Parse(_pointXInput), 1), -1);
					_points[_selectedPointIndex] = v;
					UpdatePoints();
					TriggerChangeEvent();
				}
				_tmpRect.Set(60, topOffset, 20, 18);
				GUI.Label(_tmpRect, "y");
				_tmpRect.Set(70, _panelHeight + _panelMarginTop + 20, 40, 18);
				if (NodeUtils.FloatTextField(_tmpRect, ref _pointYInput))
				{
					v.y = Mathf.Max(Mathf.Min(NodeUtils.Parse(_pointYInput), 1), -1);
					_points[_selectedPointIndex] = v;
					UpdatePoints();
					TriggerChangeEvent();
				}

				if (_points.Count > 2)
				{
					_tmpRect.Set(115, topOffset, 35, 18);
					if (GUI.Button(_tmpRect, "del"))
					{
						_points.RemoveAt(_selectedPointIndex);
						_spline = new BSpline(_points);
						UpdatePoints();
						TriggerChangeEvent();
					}
				}
			}

			_tmpRect.Set(150, topOffset, 35, 18);
			if (GUI.Button(_tmpRect, "add"))
			{
				Vector2 addPoint = new Vector2();
				if (!ContainsEqualPoint(addPoint))
				{
					_points.Add(new Vector2());
					_spline = new BSpline(_points);
					UpdatePoints();
					TriggerChangeEvent();
				}
			}

			_tmpRect.Set(190, topOffset, 20, 18);
			if (GUI.Button(_tmpRect, "+"))
			{
				_panelWidth += 50;
				_panelHeight += 50;
			}

			_tmpRect.Set(210, topOffset, 20, 18);
			if (_panelWidth > 200 && GUI.Button(_tmpRect, "-"))
			{
				_panelWidth -= 50;
				_panelHeight -= 50;
			}


			topOffset += 25;
			_tmpRect.Set(15, topOffset, 60, 20);
			var currentClamp = GUI.Toggle(_tmpRect, _clamp, "clamp");
			if (currentClamp != _clamp)
			{
				_clamp = currentClamp;
				TriggerChangeEvent();
			}


			if (_spline.LowestX > -1 || _spline.HighestX < 1)
			{
				_tmpRect.Set(80, topOffset - 2, 50, 20);
				GUI.Label(_tmpRect, "bounds:");
				_tmpRect.Set(130, topOffset, 100, 20);
				int newMode = GUI.SelectionGrid(_tmpRect, _selectedLimitOption, _limitOptions, 2, "toggle");
				if (newMode != _selectedLimitOption)
				{
					_selectedLimitOption = newMode;
					TriggerChangeEvent();
				}
			}
		}

		private bool ContainsEqualPoint(Vector2 point)
		{
			for (int index = 0; index < _points.Count; index++)
			{
				var p = _points[index];
				if (Mathf.Abs(p.x - point.x) < 0.01f && Mathf.Abs(p.y - point.y) < 0.01f) return true;
			}
			return false;
		}


		private void HandleMouseEvents()
		{

			_tmpRect.Set(_panelRect.x - 15, _panelRect.y - 15, _panelRect.width + 30, _panelRect.height + 20);

			if (!_tmpRect.Contains(Event.current.mousePosition))
			{
				_startedDragingInPanel = false;
			}

			if (Event.current.type == EventType.MouseMove) _dragPointIndex = -1;

			if (Event.current.type == EventType.MouseDown && _hoverPointIndex > -1)
			{
				_dragPointIndex = _hoverPointIndex;
				_selectedPointIndex = _hoverPointIndex;

				if (_tmpRect.Contains(Event.current.mousePosition))
				{
					_startedDragingInPanel = true;
				}
			}

			if (Event.current.type == EventType.MouseDrag && _startedDragingInPanel)
			{


				if (_tmpRect.Contains(Event.current.mousePosition))
				{
					Event.current.Use(); // avoid draging the node
				}

				if (_dragPointIndex > -1)
				{
					Vector2 nextP = ToPanelPosition(Event.current.mousePosition);
					if (nextP.x < -1) nextP.x = -1;
					if (nextP.x > 1) nextP.x = 1;
					if (nextP.y < -1) nextP.y = -1;
					if (nextP.y > 1) nextP.y = 1;
					_points[_dragPointIndex] = nextP;
					UpdatePoints();
					_hoverPointIndex = GetPointAt(Event.current.mousePosition);
					_dragPointIndex = _hoverPointIndex;
				}
			}

			if (Event.current.type == EventType.MouseUp)
			{
				if (_dragPointIndex != -1) TriggerChangeEvent();
				_dragPointIndex = -1;
			}
		}

		private void UpdatePoints()
		{
			_points = _points.OrderBy(v => v.x).ToList();
			_spline.UpdatePoints(_points);
			//c = 0;
		}

		private void DrawPoint(int index)
		{
			//_hoverPointIndex
			if (index > -1 && index < _points.Count)
			{
				Vector2 point = _points[index];
				Vector2 nodePosition = ToNodePotision(point);
				UnityEngine.Color c;
				if (_selectedPointIndex == index)
				{
					_tmpRect.Set(nodePosition.x - PointSize / 2 - 2, nodePosition.y - PointSize / 2 - 2, PointSize + 4, PointSize + 4);
					EditorGUI.DrawRect(_tmpRect, _pointSelectColor);
				}
				if (_hoverPointIndex == index) c = _pointHoverColor;
				else c = _pointColor;
				_tmpRect.Set(nodePosition.x - PointSize / 2, nodePosition.y - PointSize / 2, PointSize, PointSize);
				EditorGUI.DrawRect(_tmpRect, c);
			}
		}

		private void DrawLine()
		{
			DrawLimitAreas();


			float segments = _panelWidth / 5f;
			float start = _spline.LowestX;
			float end = _spline.HighestX;
			float step = (end - start) / segments;

			Handles.color = _pointColor;
			for (var i = start; i <= Mathf.Ceil(end - step); i += step)
			{
				float x0 = i;
				float x1 = Mathf.Min(i + step, end);
				float y0 = GetValueAt(x0);
				float y1 = GetValueAt(x1);
				_tmpVec0.Set(x0, y0, 0);
				_tmpVec1.Set(x1, y1, 0);
				UnityEngine.Vector3 v0 = ToNodePotision(_tmpVec0);
				UnityEngine.Vector3 v1 = ToNodePotision(_tmpVec1);
				Handles.DrawLine(v0, v1);
			}

		}

		private void DrawCoordinateSytem()
		{
			Handles.color = _coordinateColor01;

			_tmpVec2.Set(-1, 0);
			_tmpVec3.Set(1, 0);
			Handles.DrawLine(ToNodePotision(_tmpVec2), ToNodePotision(_tmpVec3));

			_tmpVec2.Set(0, 1);
			_tmpVec3.Set(0, -1);
			Handles.DrawLine(ToNodePotision(_tmpVec2), ToNodePotision(_tmpVec3));

			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			_tmpVec2.Set(-1, 0);
			Vector2 v = ToNodePotision(_tmpVec2);
			_tmpRect.Set(v.x - 20, v.y - 10, 20, 20);
			GUI.Label(_tmpRect, "0");

			_tmpVec2.Set(0, -1);
			v = ToNodePotision(_tmpVec2);
			_tmpRect.Set(v.x - 10, v.y, 20, 20);
			GUI.Label(_tmpRect, "0");

			Handles.color = _coordinateColor02;
			for (float x = -1; x <= 1; x+= 0.1f)
			{
				_tmpVec2.Set(x, 1);
				v = ToNodePotision(_tmpVec2);
				_tmpVec0.Set(v.x, v.y, 0);
				_tmpVec2.Set(x, -1);
				v = ToNodePotision(_tmpVec2);
				_tmpVec1.Set(v.x, v.y, 0);
				Handles.DrawLine(_tmpVec0, _tmpVec1);
			}

			for (float y = -1; y <= 1; y += 0.1f)
			{
				_tmpVec2.Set(1, y);
				v = ToNodePotision(_tmpVec2);
				_tmpVec0.Set(v.x, v.y, 0);
				_tmpVec2.Set(-1, y);
				v = ToNodePotision(_tmpVec2);
				_tmpVec1.Set(v.x, v.y, 0);
				Handles.DrawLine(_tmpVec0, _tmpVec1);
			}

			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		private void DrawLimitAreas()
		{
			// limit lines
			if (_points[0].x > -1)
			{
				_tmpVec0.Set(-1, -1, 0);
				_tmpVec1.Set(_points[0].x, 1, 0);
				UnityEngine.Vector3 v0 = ToNodePotision(_tmpVec0);
				UnityEngine.Vector3 v1 = ToNodePotision(_tmpVec1);
				_tmpRect.Set(v0.x, v0.y, v1.x - v0.x, v1.y - v0.y);
				EditorGUI.DrawRect(_tmpRect, _limitColor);
			}

			if (_points[_points.Count - 1].x < 1)
			{
				_tmpVec0.Set(_points[_points.Count - 1].x, 1, 0);
				_tmpVec1.Set(1, -1, 0);
				UnityEngine.Vector3 v0 = ToNodePotision(_tmpVec0);
				UnityEngine.Vector3 v1 = ToNodePotision(_tmpVec1);
				_tmpRect.Set(v0.x, v0.y, v1.x - v0.x, v1.y - v0.y);
				EditorGUI.DrawRect(_tmpRect, _limitColor);
			}
		}

		private float GetValueAt(float x)
		{
			float value = _spline.GetValue(x);
			if (_clamp)
			{
				if (value > 1) return 1;
				if (value < -1) return -1;
			}
			return value;
		}

		private Vector2 ToPanelPosition(Vector2 mousePosition)
		{
			mousePosition.x = ((mousePosition.x - _panelMarginLeftRight) / _panelWidth) * 2 - 1;
			mousePosition.y = ((_panelHeight + _panelMarginTop - mousePosition.y) / _panelHeight) * 2 - 1;
			return mousePosition;
		}

		private Vector2 ToNodePotision(Vector2 panelPostion)
		{
			float x = (panelPostion.x + 1) / 2;
			float y = (panelPostion.y + 1) / 2;
			panelPostion.x = (x * _panelWidth) + _panelMarginLeftRight;
			panelPostion.y = (_panelHeight - y  * _panelHeight) + _panelMarginTop;
			return panelPostion;
		}

		private int GetPointAt(Vector2 nodePosition)
		{
			for (var i = 0; i < _points.Count; i++)
			{
				Vector2 nodePositionPoint = ToNodePotision(_points[i]);
				_tmpRect.Set(nodePositionPoint.x - PointSize / 2, nodePositionPoint.y - PointSize / 2, PointSize, PointSize);
				if (_tmpRect.Contains(nodePosition)) return i;
			}
			return -1;
		}

		private bool IsInPanelBounds(Vector2 position)
		{
			return position.x >= 0 && position.x <= 1 && position.y >= 0 && position.y <= 1;
		}



		public override void Update()
		{
			_spline = new BSpline(_points);
			UpdatePoints();
		}

		private float ToInputScale(float value)
		{
			float currentSize = GetCurrentSize();
			if (currentSize == 0) return float.NaN;
			value = (value * currentSize) + (currentSize) + _spline.LowestX;
			return value;
		}

		private float GetCurrentSize()
		{
			return Mathf.Abs(_spline.HighestX - _spline.LowestX) / 2;
		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{

			float value;

			if (_selectedLimitOption == 0 && (_spline.HighestX < 1 || _spline.LowestX > -1))
			{
				value = GetInputNumber(_inputSocketValue, request);
				value = ToInputScale(value);
				float result = GetValueAt(value);
				return result * GetCurrentSize();
			}

			if (_selectedLimitOption == 1 && (_spline.HighestX < 1 || _spline.LowestX > -1))
			{
				value = GetInputNumber(_inputSocketValue, request);
				if (value > _spline.HighestX) value = _spline.HighestX;
				if (value < _spline.LowestX) value = _spline.LowestX;
				return GetValueAt(value);
			}

			value = GetInputNumber(_inputSocketValue, request);
			return GetValueAt(value);
		}
	}

}
