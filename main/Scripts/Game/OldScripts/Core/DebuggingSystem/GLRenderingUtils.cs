///////////////////////////////////////////////////////////////////////
//
//  RenderingUtils.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////
#if DEBUG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class GLRenderingUtils : IDebuggable
{
    private class Sphere
    {
        public Vector3 position;
        public float radius;
        public Color col;
        public int segments;
        public int stacks;

        public float time;
    }

    private class Line
    {
        public Vector3 start;
        public Vector4 end;
        public Color col;

        public float time;
    }

    private class Axis
    {
        public Matrix4x4 mat;

        public float time;
    }

	private class Box
	{
		public Vector3 position;
		public Vector3 halfSize; // the distance from position to the edge of the box (NOT edge to edge, which would be full size)
		public Color col;

		public float time;
	}

    public delegate void OnDrawDebugDelegate();

    public bool isDebugModeOn = true;

    static private List<Sphere> _spheres = new List<Sphere>();
    static private List<Line> _lines = new List<Line>();
    static private List<Axis> _axes = new List<Axis>();
	static private List<Box> _boxes = new List<Box>();

    static private bool _isDrawing = false;

    static public void ForceClear()
    {
        _spheres.Clear();
        _lines.Clear();
        _axes.Clear();
    }

    static public void DoDrawScaledAxes(Quaternion quat, Vector3 pos, float scale, float time = -1f)
    {
        Matrix4x4 temp = Matrix4x4.identity;
        temp.SetTRS(pos, quat, new Vector3(scale, scale, scale));
        DoDrawAxes(temp, time);
    }

    static public void DoDrawScaledAxes(Matrix4x4 mat, float scale, float time = -1)
    {
        DoDrawAxes(mat * Matrix4x4.Scale(new Vector3(scale, scale, scale)), time);
    }

    static public void DoDrawAxes(Matrix4x4 mat, float time = -1f)
    {
        if (!_isDrawing || time > 0f) // if time is greater than zero, it needs to be added to the  draw over time list
        {
            Axis axis = new Axis();
            axis.mat = mat;
            axis.time = time;
            _axes.Add(axis);
            return;
        }

        GL.Begin(GL.LINES);
        Vector3 pos = new Vector3(mat.GetColumn(3).x, mat.GetColumn(3).y, mat.GetColumn(3).z);

        GL.Color(Color.red);
        GL.Vertex(pos);
        GL.Vertex(pos + new Vector3(mat.GetColumn(0).x, mat.GetColumn(0).y, mat.GetColumn(0).z));

        GL.Color(Color.green);
        GL.Vertex(pos);
        GL.Vertex(pos + new Vector3(mat.GetColumn(1).x, mat.GetColumn(1).y, mat.GetColumn(1).z));

        GL.Color(Color.blue);
        GL.Vertex(pos);
        GL.Vertex(pos + new Vector3(mat.GetColumn(2).x, mat.GetColumn(2).y, mat.GetColumn(2).z));
        GL.End();
    }

	// halfSize is the distance from position to the edge of the box (NOT edge to edge, which would be full size)
	static public void DoDrawBox(Vector3 position, Vector3 halfSize, Color col, float time = -1f)
	{
		if (!_isDrawing || time > 0f) // if time is greater than zero, it needs to be added to the  draw over time list
		{
			Box box = new Box();
			box.position = position;
			box.halfSize = halfSize;
			box.col = col;
			box.time = time;
			_boxes.Add(box);
			return;
		}

		// box top
		Vector3 topRightForward = position + halfSize;
		Vector3 topLeftForward = position + new Vector3(-halfSize.x, halfSize.y, halfSize.z);
		Vector3 topLeftBackward = position + new Vector3(-halfSize.x, halfSize.y, -halfSize.z);
		Vector3 topRightBackward = position + new Vector3(halfSize.x, halfSize.y, -halfSize.z);

		// box bottom
		Vector3 bottomRightForward = position + new Vector3(halfSize.x, -halfSize.y, halfSize.z);
		Vector3 bottomLeftForward = position + new Vector3(-halfSize.x, -halfSize.y, halfSize.z);
		Vector3 bottomLeftBackward = position + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
		Vector3 bottomRightBackward = position + new Vector3(halfSize.x, -halfSize.y, -halfSize.z);

		GL.Begin(GL.LINES);
			GL.Color(col);
			// draw top
			GL.Vertex(topRightForward); GL.Vertex(topLeftForward);
			GL.Vertex(topLeftForward); GL.Vertex(topLeftBackward);
			GL.Vertex(topLeftBackward); GL.Vertex(topRightBackward);
			GL.Vertex(topRightBackward); GL.Vertex(topRightForward);

			// draw bottom
			GL.Vertex(bottomRightForward); GL.Vertex(bottomLeftForward);
			GL.Vertex(bottomLeftForward); GL.Vertex(bottomLeftBackward);
			GL.Vertex(bottomLeftBackward); GL.Vertex(bottomRightBackward);
			GL.Vertex(bottomRightBackward); GL.Vertex(bottomRightForward);

			// draw bottom to top
			GL.Vertex(bottomRightForward); GL.Vertex(topRightForward);
			GL.Vertex(bottomLeftForward); GL.Vertex(topLeftForward);
			GL.Vertex(bottomLeftBackward); GL.Vertex(topLeftBackward);
			GL.Vertex(bottomRightBackward); GL.Vertex(topRightBackward);
		GL.End();
	}

    static public void DoDrawSphere(Vector3 position, float radius, Color col, int segments = 8, int stacks = 4, float time = -1f)
    {
        if (segments < 1 || stacks < 1)
        {
            return;
        }

        if (!_isDrawing || time > 0f) // if time is greater than zero, it needs to be added to the  draw over time list
        {
            Sphere sphere = new Sphere();
            sphere.position = position;
            sphere.radius = radius;
            sphere.col = col;
            sphere.segments = segments;
            sphere.stacks = stacks;
            sphere.time = time;
            _spheres.Add(sphere);
            return;
        }

        GL.Begin(GL.LINES);
        GL.Color(col);
        float oneStackAngle = Mathf.PI / stacks;
        float oneSegmentAngle = (2f * Mathf.PI) / segments;

        for (int stackCount = 0; stackCount < stacks; ++stackCount)
        {
            float stackAngle = stackCount * oneStackAngle;
            float groundPlaneLen = Mathf.Sin(stackAngle) * radius;
            float y = Mathf.Cos(stackAngle) * radius;

            float stackAngleNextStack = (stackCount + 1) * oneStackAngle;
            float groundPlaneLenNextStack = Mathf.Sin(stackAngleNextStack) * radius;
            float yNextStack = Mathf.Cos(stackAngleNextStack) * radius;

            for (int segmentCount = 0; segmentCount < segments; ++segmentCount)
            {
                float segmentAngle = segmentCount * oneSegmentAngle;
                Vector3 vert = position + new Vector3(Mathf.Sin(segmentAngle) * groundPlaneLen, y, Mathf.Cos(segmentAngle) * groundPlaneLen);
                GL.Vertex(vert); // drawing line to next stack
                GL.Vertex(position + new Vector3(Mathf.Sin(segmentAngle) * groundPlaneLenNextStack, yNextStack, Mathf.Cos(segmentAngle) * groundPlaneLenNextStack));

                if (0 != stackCount) // the line vertices would all be in the same place, so no need to draw
                { // drawing line to next segment
                    GL.Vertex(vert);
                    float segmentAnglePlusOne = (1 + segmentCount) * oneSegmentAngle;
                    GL.Vertex(position + new Vector3(Mathf.Sin(segmentAnglePlusOne) * groundPlaneLen, y, Mathf.Cos(segmentAnglePlusOne) * groundPlaneLen));
                }
            }
        }
        GL.End();
    }

    static public void DoDrawLine(Vector3 from, Vector3 to, Color col, float time = -1f)
    {
        if (!_isDrawing || time > 0f) // if time is greater than zero, it needs to be added to the  draw over time list
        {
            Line line = new Line();
            line.start = from;
            line.end = to;
            line.col = col;
            line.time = time;
            _lines.Add(line);
            return;
        }

        GL.Begin(GL.LINES);
        GL.Color(col);
        GL.Vertex(from);
        GL.Vertex(to);
        GL.End();
    }

    public void OnDrawDebug()
    {
        GL.PushMatrix();

        _isDrawing = true;

        for (int obj = 0; obj < _spheres.Count; ++obj)
        {
            DoDrawSphere(_spheres[obj].position, _spheres[obj].radius, _spheres[obj].col, _spheres[obj].segments, _spheres[obj].stacks);
            _spheres[obj].time -= Time.deltaTime;
            if (_spheres[obj].time <= 0f)
            {
                _spheres.RemoveAt(obj--);
            }
        }

        for (int obj = 0; obj < _lines.Count; ++obj)
        {
            DoDrawLine(_lines[obj].start, _lines[obj].end, _lines[obj].col);
            _lines[obj].time -= Time.deltaTime;
            if (_lines[obj].time <= 0f)
            {
                _lines.RemoveAt(obj--);
            }
        }

        for (int obj = 0; obj < _axes.Count; ++obj)
        {
            DoDrawAxes(_axes[obj].mat);
            _axes[obj].time -= Time.deltaTime;
            if (_axes[obj].time <= 0f)
            {
                _axes.RemoveAt(obj--);
            }
        }

		for (int obj = 0; obj < _boxes.Count; ++obj)
		{
			DoDrawBox(_boxes[obj].position, _boxes[obj].halfSize, _boxes[obj].col);
			_boxes[obj].time -= Time.deltaTime;
			if (_boxes[obj].time <= 0f)
			{
				_boxes.RemoveAt(obj--);
			}
		}

        _isDrawing = false;
        GL.PopMatrix();
    }


    public void OnDebugGUI()
    {
    }

    public void OnDebugPanelGUI()
    {
    }
}

#endif