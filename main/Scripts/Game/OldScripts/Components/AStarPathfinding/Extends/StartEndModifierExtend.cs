using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

#if APP_UPDATED

namespace Pathfinding
{
    public partial class StartEndModifier
    {
        public enum Exactness
        {
            SnapToNode,     /**< The point is snapped to the first/last node in the path*/
            Original,       /**< The point is set to the exact point which was passed when calling the pathfinding */
            Interpolate,    /**< The point is set to the closest point on the line between either the two first points or the two last points */
            ClosestOnNode,   /**< The point is set to the closest point on the node. Note that for some node types (point nodes) the "closest point" is the node's position which makes this identical to Exactness.SnapToNode */
#if BNICKSON_UPDATED
            VisibilityCheck, /**< The point is set to the exact point which was passed when calling the pathfinding as long as that point can be reached */
#endif
            NodeConnection,/**< The point is set to the closest point on one of the connections from the start/end node */
        }

        #region override functions

        Vector3 Snap(ABPath path, Exactness mode, bool start, out bool forceAddPoint)
        {
            var index = start ? 0 : path.path.Count - 1;
            var node = path.path[index];
            var nodePos = (Vector3)node.position;

            forceAddPoint = false;

            switch (mode)
            {
                case Exactness.ClosestOnNode:
                    return GetClampedPoint(nodePos, start ? path.startPoint : path.endPoint, node);
                case Exactness.SnapToNode:
                    return nodePos;
                case Exactness.Original:
                case Exactness.Interpolate:
                case Exactness.NodeConnection:
                    Vector3 relevantPoint;
                    if (start)
                    {
                        relevantPoint = adjustStartPoint != null ? adjustStartPoint() : path.originalStartPoint;
                    }
                    else {
                        relevantPoint = path.originalEndPoint;
                    }

                    switch (mode)
                    {
                        case Exactness.Original:
                            return GetClampedPoint(nodePos, relevantPoint, node);
                        case Exactness.Interpolate:
                            var clamped = GetClampedPoint(nodePos, relevantPoint, node);
                            // Adjacent node to either the start node or the end node in the path
                            var adjacentNode = path.path[Mathf.Clamp(index + (start ? 1 : -1), 0, path.path.Count - 1)];
                            return VectorMath.ClosestPointOnSegment(nodePos, (Vector3)adjacentNode.position, clamped);
                        case Exactness.NodeConnection:
                            // This code uses some tricks to avoid allocations
                            // even though it uses delegates heavily
                            // The connectionBufferAddDelegate delegate simply adds whatever node
                            // it is called with to the connectionBuffer
                            connectionBuffer = connectionBuffer ?? new List<GraphNode>();
                            connectionBufferAddDelegate = connectionBufferAddDelegate ?? (GraphNodeDelegate)connectionBuffer.Add;

                            // Adjacent node to either the start node or the end node in the path
                            adjacentNode = path.path[Mathf.Clamp(index + (start ? 1 : -1), 0, path.path.Count - 1)];

                            // Add all neighbours of #node to the connectionBuffer
                            node.GetConnections(connectionBufferAddDelegate);
                            var bestPos = nodePos;
                            var bestDist = float.PositiveInfinity;

                            // Loop through all neighbours
                            // Do it in reverse order because the length of the connectionBuffer
                            // will change during iteration
                            for (int i = connectionBuffer.Count - 1; i >= 0; i--)
                            {
                                var neighbour = connectionBuffer[i];

                                // Find the closest point on the connection between the nodes
                                // and check if the distance to that point is lower than the previous best
                                var closest = VectorMath.ClosestPointOnSegment(nodePos, (Vector3)neighbour.position, relevantPoint);

                                var dist = (closest - relevantPoint).sqrMagnitude;
                                if (dist < bestDist)
                                {
                                    bestPos = closest;
                                    bestDist = dist;

                                    // If this node is not the adjacent node
                                    // then the path should go through the start node as well
                                    forceAddPoint = neighbour != adjacentNode;
                                }
                            }

                            connectionBuffer.Clear();
                            return bestPos;
                        default:
                            throw new System.ArgumentException("Cannot reach this point, but the compiler is not smart enough to realize that.");
                    }
#if BNICKSON_UPDATED
                case Exactness.VisibilityCheck:
                    if (start)
                        return GetClampedPoint((Vector3)path.path[0].position, path.originalStartPoint, path.path[0], true);
                    else 
                        return GetClampedPoint((Vector3)path.path[path.path.Count - 1].position, path.originalEndPoint, path.path[path.path.Count - 1], true);
#endif
                default:
                    throw new System.ArgumentException("Invalid mode");
            }
        }

        // removed option useGraphRaycasting, so it could be specified on the start/end individually
        public Vector3 GetClampedPoint(Vector3 from, Vector3 to, GraphNode hint, bool useGraphRaycasting)
        {
            bool bak = this.useGraphRaycasting;
            this.useGraphRaycasting = useGraphRaycasting;
            Vector3 ret = GetClampedPoint(from, to, hint);
            this.useGraphRaycasting = bak;

            return ret;
        }

        #endregion
    }
}

#endif
