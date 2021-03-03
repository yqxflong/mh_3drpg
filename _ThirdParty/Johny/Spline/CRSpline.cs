//JohnySpline
//Author: Johny
//用于实现动态物体的平滑曲线运动
//===翻版必究===

using UnityEngine;
using System;

namespace Johny{
    /// <summary>
    /// CR样条线
    /// </summary>
    public class CRSpline
    {
        public CRSpline(params Vector3[] points)
        {
            path = PathControlPointGenerator(points);
        }

        public void Reset(params Vector3[] points)
        {
            cachePathLength = 0.0f;
            path = PathControlPointGenerator(points);
        }

        private Vector3[] PathControlPointGenerator(Vector3[] path)
        {
            Vector3[] suppliedPath;
            Vector3[] vector3s;

            //create and store path points:
            suppliedPath = path;

            //populate calculate path;
            int offset = 2;
            vector3s = new Vector3[suppliedPath.Length + offset];
            Array.Copy(suppliedPath, 0, vector3s, 1, suppliedPath.Length);

            //populate start and end control points:
            //vector3s[0] = vector3s[1] - vector3s[2];
            vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
            vector3s[vector3s.Length - 1] = vector3s[vector3s.Length - 2] + (vector3s[vector3s.Length - 2] - vector3s[vector3s.Length - 3]);

            //is this a closed, continuous loop? yes? well then so let's make a continuous Catmull-Rom spline!
            if (vector3s[1] == vector3s[vector3s.Length - 2])
            {
                Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
                Array.Copy(vector3s, tmpLoopSpline, vector3s.Length);
                tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];
                tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];
                vector3s = new Vector3[tmpLoopSpline.Length];
                Array.Copy(tmpLoopSpline, vector3s, tmpLoopSpline.Length);
            }

            return (vector3s);
        }

        /// <summary>
        /// 根据插值系数，在曲线上插值出某一个点的位置
        /// </summary>
        /// <param name="t">插值系数，取值0到1</param>
        /// <returns></returns>
        public Vector3 Interp(float t)
        {
            int numSections = path.Length - 3;
            int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
            float u = t * (float)numSections - (float)currPt;

            Vector3 a = path[currPt];
            Vector3 b = path[currPt + 1];
            Vector3 c = path[currPt + 2];
            Vector3 d = path[currPt + 3];

            return .5f * (
                (-a + 3f * b - 3f * c + d) * (u * u * u)
                + (2f * a - 5f * b + 4f * c - d) * (u * u)
                + (-a + c) * u
                + 2f * b
            );
        }

        /// <summary>
        /// 获得曲线长度
        /// </summary>
        public float Length
        {
            get
            {
                if (cachePathLength == 0.0f)
                {
                    cachePathLength = 0.0f;
                    Vector3 prevPt = Interp(0);
                    int SmoothAmount = path.Length * 20;
                    for (int i = 1; i <= SmoothAmount; i++)
                    {
                        float pm = (float)i / SmoothAmount;
                        Vector3 currPt = Interp(pm);
                        cachePathLength += Vector3.Distance(prevPt, currPt);
                        prevPt = currPt;
                    }
                }

                return cachePathLength;
            }
        }

        Vector3[] path;
        float cachePathLength = 0.0f;
    }
}