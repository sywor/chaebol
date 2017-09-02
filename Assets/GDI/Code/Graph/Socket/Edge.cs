using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Socket
{

	public class Edge
	{
		public InputSocket Input;
		public OutputSocket Output;

		private Vector2 _tmpStartPos;
		private Vector2 _tmpEndPos;

		private List<Vector2> _path;

		private Rect _tmpRect;

		private Rect _bounds;

		public Rect Bounds
		{
			get { return _bounds; }
		}

		public List<Vector2> Path
		{
			set { _path = value; }
		}

		public Edge(OutputSocket outputSocket, InputSocket inputSocket)
		{
			Input = inputSocket;
			Output = outputSocket;
			_path = new List<Vector2>();
			_tmpRect = new Rect();
			UpdateBounds();
		}

		public AbstractSocket GetOtherSocket(AbstractSocket socket)
		{
			if (socket == Input) return Output;
			return Input;
		}

		public void Draw(bool highlight, bool hover)
		{
			if (Input != null && Output != null)
			{
				for (int i = 0; i <= _path.Count; i++)
				{
					DrawEdgeSegment(
						GetSegmentStartPosition(i),
						GetSegmentTangent01(i),
						GetSegmentEndPosition(i),
						GetSegmentTangent02(i), Output.Type, highlight, hover);
				}

				if (hover)
				{
					for (int i = 0; i < _path.Count; i++)
					{
						_tmpRect.Set(_path[i].x - Config.PathPointSize / 2f, _path[i].y - Config.PathPointSize / 2f,
							Config.PathPointSize, Config.PathPointSize);
						EditorGUI.DrawRect(_tmpRect, Color.white);
					}
				}
			}
		}

		private Vector2 GetSegmentStartPosition(int segmentIndex)
		{
			if (segmentIndex == 0) return GetEdgeSocketPosition(Output, _tmpStartPos);
			return _path[segmentIndex - 1];
		}


		private Vector2 GetSegmentEndPosition(int segmentIndex)
		{
			if (segmentIndex == _path.Count) return GetEdgeSocketPosition(Input, _tmpEndPos);
			return _path[segmentIndex];
		}

		private Vector2 GetSegmentTangent01(int segmentIndex)
		{
			if (segmentIndex == 0) return GetSocketTangentPosition(Output, GetSegmentStartPosition(segmentIndex));

			Vector2 prev = GetSegmentStartPosition(segmentIndex - 1);
			Vector2 current = GetSegmentStartPosition(segmentIndex);
			Vector2 direction = current - prev;
			direction.Normalize();
			direction.x *= Config.EdgeTangent / 3f;
			direction.y *= Config.EdgeTangent / 3f;
			direction = current + direction;
			return direction;
		}

		private Vector2 GetSegmentTangent02(int segmentIndex)
		{
			if (segmentIndex == _path.Count) return GetSocketTangentPosition(Input, GetSegmentEndPosition(segmentIndex));
			Vector2 current = GetSegmentEndPosition(segmentIndex);
			Vector2 next = GetSegmentEndPosition(segmentIndex + 1);
			Vector2 direction = current - next;
			direction.Normalize();
			direction.x *= Config.EdgeTangent / 3f;
			direction.y *= Config.EdgeTangent / 3f;
			direction = current + direction;
			return direction;
		}

		public void AddPathPointAt(Vector2 windowPosition, int segmentIndex)
		{
			_path.Insert(segmentIndex, windowPosition);
			UpdateBounds();
		}

		public void RemovePathPointAt(Vector2 windowPosition)
		{
			for (int index = 0; index < _path.Count; index++)
			{
				var p = _path[index];
				_tmpRect.Set(windowPosition.x - Config.PathPointSize / 2f, windowPosition.y - Config.PathPointSize / 2f,
					Config.PathPointSize, Config.PathPointSize);
				if (_tmpRect.Contains(p))
				{
					_path.Remove(p);
					UpdateBounds();
					return;
				}
			}
		}

		public void MoveAllPathPoints(float x, float y)
		{
			for (var i = 0; i < _path.Count; i++)
			{
				_path[i] = new Vector2(_path[i].x + x, _path[i].y + y);
			}
			UpdateBounds();
		}

		public int GetPathPointIndex(Vector2 windowPosition)
		{
			for (var i = 0; i < _path.Count; i++)
			{
				Vector2 point = _path[i];
				_tmpRect.Set(point.x - Config.PathPointSize / 2f, point.y - Config.PathPointSize / 2f,
					Config.PathPointSize, Config.PathPointSize);
				if (_tmpRect.Contains(windowPosition)) return i;
			}
			return -1;
		}

		public bool SetPathPointAtIndex(int index, Vector2 point)
		{
			if (index < 0 || index >= _path.Count) return false;
			_path[index] = point;
			UpdateBounds();
			return true;
		}

		public void UpdateBounds()
		{
			float lowestX = float.MaxValue;
			float lowestY = float.MaxValue;

			float heigestX = float.MinValue;
			float heigestY = float.MinValue;
			for (int i = 0; i < _path.Count; i++)
			{
				lowestX = Mathf.Min(lowestX, _path[i].x);
				lowestY = Mathf.Min(lowestY, _path[i].y);
				heigestX = Mathf.Max(heigestX, _path[i].x);
				heigestY = Mathf.Max(heigestY, _path[i].y);
			}

			_tmpStartPos = GetEdgeSocketPosition(Input, _tmpStartPos);
			_tmpEndPos = GetEdgeSocketPosition(Output, _tmpEndPos);

			lowestX = Mathf.Min(_tmpStartPos.x, lowestX);
			lowestX = Mathf.Min(_tmpEndPos.x, 	lowestX);
			lowestY = Mathf.Min(_tmpStartPos.y, lowestY);
			lowestY = Mathf.Min(_tmpEndPos.y, 	lowestY);

			heigestX = Mathf.Max(_tmpStartPos.x, heigestX);
			heigestX = Mathf.Max(_tmpEndPos.x, heigestX);
			heigestY = Mathf.Max(_tmpStartPos.y, heigestY);
			heigestY = Mathf.Max(_tmpEndPos.y, heigestY);

			_bounds.x = lowestX - 20;
			_bounds.y = lowestY - 20;
			_bounds.width = heigestX - lowestX + 40;
			_bounds.height = heigestY - lowestY + 40;
		}

		public int IntersectsPathSegment(Vector2 windowPosition)
		{
			for (int i = 0; i <= _path.Count; i++)
			{
				float distance = HandleUtility.DistancePointBezier(windowPosition,
					GetSegmentStartPosition(i), GetSegmentEndPosition(i),
					GetSegmentTangent01(i), GetSegmentTangent02(i));
				if (distance <= Config.PathPointSize) return i;
			}
			return -1;
		}

		public static void DrawEdgeSegment(Vector2 position01, Vector2 tangent01, Vector2 position02,
			Vector2 tangent02, Type type, bool highlight, bool hover)
		{
			Color borderColor = Color.black;
			float borderSize = 3;

			if (highlight)
			{
				borderColor = Color.white;
				borderSize = 5;
			}
			if (hover)
			{
				borderColor = Color.black;
				borderSize = 11;
			}
			Handles.DrawBezier(
				position01, position02,
				tangent01, tangent02, borderColor, null, 7);

			Handles.DrawBezier(
				position01, position02,
				tangent01, tangent02, Node.GetEdgeColor(type), null, borderSize);
		}

		public static Vector2 GetEdgeSocketPosition(AbstractSocket socket, Vector2 position)
		{
			if (socket.Parent.Collapsed)
			{
				float width = Config.SocketSize;
				if (socket.IsOutput()) width = 0;
				position.Set(socket.X + width, socket.Parent.WindowRect.y + 8);
			}
			else
			{
				float width = 0;
				if (socket.IsOutput()) width = Config.SocketSize;
				position.Set(socket.X + width, socket.Y + Config.SocketSize / 2f);
			}
			return position;
		}

		public static Vector2 GetSocketTangentPosition(AbstractSocket socket, Vector2 position)
		{
			if (socket.IsInput()) return position + Vector2.left * Config.EdgeTangent;
			return position + Vector2.right * Config.EdgeTangent;
		}

		///<summary>Creates a serializable version of this edge.</summary>
		/// <returns>A serializable version of this edge.</returns>
		public SerializableEdge ToSerializedEgde()
		{
			SerializableEdge s = new SerializableEdge();
			s.InputNodeId = Input.Parent.Id;
			s.InputSocketIndex = Input.Parent.Sockets.IndexOf(Input);
			s.OutputNodeId = Output.Parent.Id;
			s.OutputSocketIndex = Output.Parent.Sockets.IndexOf(Output);
			s.Path = _path;
			return s;
		}
	}


	[Serializable] public class SerializableEdge
	{
		[SerializeField] public int OutputNodeId = -1;
		[SerializeField] public int OutputSocketIndex = -1;
		[SerializeField] public int InputNodeId = -1;
		[SerializeField] public int InputSocketIndex = -1;
		[SerializeField] public List<Vector2> Path;
	}
}


