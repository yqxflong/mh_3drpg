﻿using UnityEngine;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.Util;

#if APP_UPDATED

namespace Pathfinding
{
    public partial class FunnelModifier
    {
        /* how far the paths generated by the funnel modifier should stay from the edge of the nav mesh */
        [HideInInspector] // Hides var below
        public float obstaclePadding = 0f;

        // this function keeps the path generated away from the edge of the nav mesh, replacing funnel path points which are on a triangle vertex
        // with new points which are offset into the triangle away from the vertex
        private void PadFunnelPath(ref List<Vector3> funnelPath, ref List<GraphNode> path, Path _p)
        {
            ABPath abPath = _p as ABPath;
            const float SamePositionSqr = 0.001f * 0.001f;

            bool skipFirstPoint = false;
            bool skipLastPoint = false;
            if (null != abPath && funnelPath.Count > 0)
            {   // don't want to mess with the start point
                if (VectorMath.SqrDistanceXZ(funnelPath[0], abPath.originalStartPoint) < SamePositionSqr)
                {
                    skipFirstPoint = true;
                }
                // don't want to mess with the end point
                if (VectorMath.SqrDistanceXZ(funnelPath[funnelPath.Count - 1], abPath.originalEndPoint) < SamePositionSqr)
                {
                    skipLastPoint = true;
                }
            }

            int startNode = 0;
            for (int point = 0; point < funnelPath.Count; ++point)
            {
                Vector3 funnelPoint = funnelPath[point];

                if (0 == point && skipFirstPoint)
                {
                    continue;
                }

                if (funnelPath.Count - 1 == point && skipLastPoint)
                {
                    continue;
                }

                bool hasPointBeenRemoved = false; // we will remove the funnel path point if it is found to be on a triangle vertex (it'll be replaced with offset points)

                ++startNode;
                // go over all points apart from the end node, as we don't want to add new path points in the end node as that node contains the target position
                for (int node = startNode; node < path.Count - 1; ++node)
                {
                    if (node < 0 || node >= path.Count)
                    {
                        continue;
                    }
                    TriangleMeshNode theNode = path[node] as TriangleMeshNode;

                    Vector3[] vertices = new Vector3[3] { (Vector3)theNode.GetVertex(0), (Vector3)theNode.GetVertex(1), (Vector3)theNode.GetVertex(2) };

                    for (int vert = 0; vert < 3; ++vert)
                    {
                        if (VectorMath.SqrDistanceXZ(funnelPoint, vertices[vert]) < SamePositionSqr) // the triangle has a vertex on the funnel path
                        {
                            bool isNodeOnTheLine = false;
                            // in this section we test if the triangle has an edge on the funnel path
                            if (point < funnelPath.Count - 1) // we need another point after point
                            {
                                Vector3 nextFunnelPoint = funnelPath[point + 1];
                                Vector3 vertNotOnTheLine = new Vector3(); // if two of our triangle verts are on the funnel path, this is the other vert
                                if (VectorMath.SqrDistanceXZ(vertices[(vert + 2) % 3], nextFunnelPoint) < SamePositionSqr)
                                {
                                    isNodeOnTheLine = true;
                                    vertNotOnTheLine = vertices[(vert + 1) % 3]; // other point is next
                                }
                                else if (VectorMath.SqrDistanceXZ(vertices[(vert + 1) % 3], nextFunnelPoint) < SamePositionSqr)
                                {
                                    isNodeOnTheLine = true;
                                    vertNotOnTheLine = vertices[(vert + 2) % 3]; // other point is prev
                                }

                                if (isNodeOnTheLine)
                                {
                                    Vector3 pushAwayDirNorm = vertNotOnTheLine - VectorMath.ClosestPointOnLine(funnelPoint, nextFunnelPoint, vertNotOnTheLine);
                                    pushAwayDirNorm.y = 0f;
                                    pushAwayDirNorm.Normalize();

                                    // add a new point to the funnel path which is offset at a right angle from triangle line which is coincident with the funnel path
                                    Vector3 lineStart = funnelPoint + ((nextFunnelPoint - funnelPoint) * 0.25f);
                                    Vector3 firstPointToAdd = lineStart + pushAwayDirNorm * obstaclePadding;
                                    float maxLineLen = CalculateMaxLineLengthXZ(lineStart, firstPointToAdd, nextFunnelPoint, vertNotOnTheLine, vertices[vert]);
                                    firstPointToAdd = lineStart + pushAwayDirNorm * maxLineLen;
                                    AddPadPointToFunnelPath(ref funnelPath, ref point, ref hasPointBeenRemoved, firstPointToAdd);

                                    // add another point to the funnel path which is offset at a right angle from triangle line which is coincident with the funnel path
                                    lineStart = funnelPoint + ((nextFunnelPoint - funnelPoint) * 0.75f);
                                    Vector3 secondPointToAdd = lineStart + pushAwayDirNorm * obstaclePadding;
                                    maxLineLen = CalculateMaxLineLengthXZ(lineStart, secondPointToAdd, nextFunnelPoint, vertNotOnTheLine, vertices[vert]);
                                    secondPointToAdd = lineStart + pushAwayDirNorm * maxLineLen;
                                    AddPadPointToFunnelPath(ref funnelPath, ref point, ref hasPointBeenRemoved, secondPointToAdd);
                                }
                            }

                            // if the node did not have an edge on the funnel path, let's just offset from the single triangle vertex on the funnel path
                            if (!isNodeOnTheLine)
                            {
                                Vector3 toPrevVert = (vertices[(vert + 2) % 3] - vertices[vert]);
                                Vector3 toNextVert = (vertices[(vert + 1) % 3] - vertices[vert]);

                                // calculate a line between the two triangle edges and offset a point on that line
                                Vector3 newNavPoint = Vector3.Lerp(vertices[vert] + toPrevVert.normalized, vertices[vert] + toNextVert.normalized, 0.5f);
                                float maxLength = CalculateMaxLineLengthXZ(vertices[vert], newNavPoint, vertices[(vert + 1) % 3], vertices[(vert + 2) % 3]);
                                Vector3 dirNorm = (newNavPoint - vertices[vert]);
                                dirNorm.y = 0f;
                                newNavPoint = vertices[vert] + dirNorm.normalized * Mathf.Min(obstaclePadding, maxLength);
                                AddPadPointToFunnelPath(ref funnelPath, ref point, ref hasPointBeenRemoved, newNavPoint);
                            }
                            startNode = node;
                            break;
                        }
                    }
                }
            }
        }

        // add a new point to the funnel path as a result of padding the path away from the edge of the nav mesh
        // handles removal of existing point
        private void AddPadPointToFunnelPath(ref List<Vector3> funnelPath, ref int point, ref bool hasOriginalFunnelPointBeenRemoved, Vector3 newFunnelPathPoint)
        {
            if (!hasOriginalFunnelPointBeenRemoved) // only want to remove the original point once, this variable is rest to false as every point is evaluated in PadFunnelPath()
            {
                funnelPath.RemoveAt(point);
            }
            if (hasOriginalFunnelPointBeenRemoved) // we only increment number of point if we have NOT just removed the original point
            {
                ++point;
            }
            funnelPath.Insert(point, newFunnelPathPoint);
            hasOriginalFunnelPointBeenRemoved = true;
        }

        // see when the line (lineStart to lineEnd) intersects the barrier lines (barrierVertZero to barrierVertOne and the line barrierVertOne to barrierVertTwo)
        // return back the min length to these lines, or return the current length of the line if both barrier lines are u
        private static float CalculateMaxLineLengthXZ(Vector3 lineStart, Vector3 lineEnd, Vector3 barrierVertZero, Vector3 barrierVertOne, Vector3? barrierVertTwo = null)
        {
            lineStart.y = lineEnd.y = barrierVertZero.y = barrierVertOne.y = 0f;

            float maxLineLength = (lineStart - lineEnd).magnitude;
            bool doesIntersect = false;
            Vector3 intersectionPoint = VectorMath.SegmentIntersectionPointXZ(lineStart, lineEnd, barrierVertZero, barrierVertOne, out doesIntersect);
            if (doesIntersect)
            {
                maxLineLength = (intersectionPoint - lineStart).magnitude;
            }

            if (barrierVertTwo.HasValue) // if we want to test two barrier lines
            {
                Vector3 barrierVertTwoVal = barrierVertTwo.GetValueOrDefault();
                barrierVertTwoVal.y = 0f;
                intersectionPoint = VectorMath.SegmentIntersectionPointXZ(lineStart, lineEnd, barrierVertOne, barrierVertTwoVal, out doesIntersect);
                if (doesIntersect)
                {
                    maxLineLength = Mathf.Min(maxLineLength, (intersectionPoint - lineStart).magnitude); // see if this barier is nearer than the previous
                }
            }
            return maxLineLength;
        }

        #region override functions

        public override void Apply(Path p)
        {
            List<GraphNode> path = p.path;
            List<Vector3> vectorPath = p.vectorPath;

            if (path == null || path.Count == 0 || vectorPath == null || vectorPath.Count != path.Count)
            {
                return;
            }

            List<Vector3> funnelPath = ListPool<Vector3>.Claim();

            // Claim temporary lists and try to find lists with a high capacity
            List<Vector3> left = ListPool<Vector3>.Claim(path.Count + 1);
            List<Vector3> right = ListPool<Vector3>.Claim(path.Count + 1);

            AstarProfiler.StartProfile("Construct Funnel");

            // Add start point
            left.Add(vectorPath[0]);
            right.Add(vectorPath[0]);

            // Loop through all nodes in the path (except the last one)
            for (int i = 0; i < path.Count - 1; i++)
            {
                // Get the portal between path[i] and path[i+1] and add it to the left and right lists
                bool portalWasAdded = path[i].GetPortal(path[i + 1], left, right, false);

                if (!portalWasAdded)
                {
                    // Fallback, just use the positions of the nodes
                    left.Add((Vector3)path[i].position);
                    right.Add((Vector3)path[i].position);

                    left.Add((Vector3)path[i + 1].position);
                    right.Add((Vector3)path[i + 1].position);
                }
            }

            // Add end point
            left.Add(vectorPath[vectorPath.Count - 1]);
            right.Add(vectorPath[vectorPath.Count - 1]);

            if (!RunFunnel(left, right, funnelPath))
            {
                // If funnel algorithm failed, degrade to simple line
                funnelPath.Add(vectorPath[0]);
                funnelPath.Add(vectorPath[vectorPath.Count - 1]);
            }

#if BNICKSON_UPDATED
            if (0f != obstaclePadding)
            {
                PadFunnelPath(ref funnelPath, ref path, p);
            }
#endif

            // Release lists back to the pool
            ListPool<Vector3>.Release(p.vectorPath);
            p.vectorPath = funnelPath;

            ListPool<Vector3>.Release(left);
            ListPool<Vector3>.Release(right);
        }

        #endregion
    }
}

#endif