using UnityEngine;
using System.Collections.Generic;
using Pathfinding.Serialization.JsonFx;
using Pathfinding.Serialization;
using Pathfinding;

#if APP_UPDATED

namespace Pathfinding
{
    public partial class NavMeshGraph
    {
        #region override functions

        /** Returns if there is an obstacle between \a origin and \a end on the graph.
         * \param [in] graph The graph to perform the search on
         * \param [in] tmp_origin Point to start from
         * \param [in] tmp_end Point to linecast to
         * \param [out] hit Contains info on what was hit, see GraphHitInfo
         * \param [in] hint You need to pass the node closest to the start point, if null, a search for the closest node will be done
         * \param trace If a list is passed, then it will be filled with all nodes the linecast traverses
         * This is not the same as Physics.Linecast, this function traverses the \b graph and looks for collisions instead of checking for collider intersection.
         * \astarpro */
        public static bool Linecast(INavmesh graph, Vector3 tmp_origin, Vector3 tmp_end, GraphNode hint, out GraphHitInfo hit, List<GraphNode> trace)
        {
            var end = (Int3)tmp_end;
            var origin = (Int3)tmp_origin;

            hit = new GraphHitInfo();

            if (float.IsNaN(tmp_origin.x + tmp_origin.y + tmp_origin.z)) throw new System.ArgumentException("origin is NaN");
            if (float.IsNaN(tmp_end.x + tmp_end.y + tmp_end.z)) throw new System.ArgumentException("end is NaN");

            var node = hint as TriangleMeshNode;
            if (node == null)
            {
                node = (graph as NavGraph).GetNearest(tmp_origin, NNConstraint.None).node as TriangleMeshNode;

                if (node == null)
                {
                    EB.Debug.LogError("Could not find a valid node to start from");
                    hit.point = tmp_origin;
                    return true;
                }
            }

            if (origin == end)
            {
                hit.node = node;
                return false;
            }

            origin = (Int3)node.ClosestPointOnNode((Vector3)origin);
            hit.origin = (Vector3)origin;

            if (!node.Walkable)
            {
                hit.point = (Vector3)origin;
                hit.tangentOrigin = (Vector3)origin;
                return true;
            }


            List<Vector3> left = Pathfinding.Util.ListPool<Vector3>.Claim();//new List<Vector3>(1);
            List<Vector3> right = Pathfinding.Util.ListPool<Vector3>.Claim();//new List<Vector3>(1);

#if BNICKSON_UPDATED
            HashSet<GraphNode> visitedNodes = new HashSet<GraphNode>();
#endif    

            int counter = 0;
            while (true)
            {
#if BNICKSON_UPDATED
                if (visitedNodes.Contains(node)) // this can happen due to a badly formed nav mesh (a node/triangle should never be inspected twice in a straight line cast)
                {
                    EB.Debug.LogError("NavMeshGenerator.Linecast() : Badly formed nav mesh causing a potential infinate loop. Cast from " + tmp_origin + ", to " + tmp_end);
                    return false;
                }
                visitedNodes.Add(node);
#endif

                counter++;
                if (counter > 2000)
                {
                    EB.Debug.LogError("Linecast was stuck in infinite loop. Breaking.");
                    Pathfinding.Util.ListPool<Vector3>.Release(left);
                    Pathfinding.Util.ListPool<Vector3>.Release(right);
                    return true;
                }

                TriangleMeshNode newNode = null;

                if (trace != null) trace.Add(node);

                if (node.ContainsPoint(end))
                {
                    Pathfinding.Util.ListPool<Vector3>.Release(left);
                    Pathfinding.Util.ListPool<Vector3>.Release(right);
                    return false;
                }

                for (int i = 0; i < node.connections.Length; i++)
                {
                    //Nodes on other graphs should not be considered
                    //They might even be of other types (not MeshNode)
                    if (node.connections[i].GraphIndex != node.GraphIndex) continue;

                    left.Clear();
                    right.Clear();

                    if (!node.GetPortal(node.connections[i], left, right, false)) continue;

                    Vector3 a = left[0];
                    Vector3 b = right[0];

                    //i.e Left or colinear
                    if (!VectorMath.RightXZ(a, b, hit.origin))
                    {
                        if (VectorMath.RightXZ(a, b, tmp_end))
                        {
                            //Since polygons are laid out in clockwise order, the ray would intersect (if intersecting) this edge going in to the node, not going out from it
                            continue;
                        }
                    }

                    float factor1, factor2;

                    if (VectorMath.LineIntersectionFactorXZ(a, b, hit.origin, tmp_end, out factor1, out factor2))
                    {
                        //Intersection behind the start
                        if (factor2 < 0) continue;

                        if (factor1 >= 0 && factor1 <= 1)
                        {
                            newNode = node.connections[i] as TriangleMeshNode;
                            break;
                        }
                    }
                }

                if (newNode == null)
                {
                    //Possible edge hit
                    int vs = node.GetVertexCount();

                    for (int i = 0; i < vs; i++)
                    {
                        var a = (Vector3)node.GetVertex(i);
                        var b = (Vector3)node.GetVertex((i + 1) % vs);


                        //i.e left or colinear
                        if (!VectorMath.RightXZ(a, b, hit.origin))
                        {
                            //Since polygons are laid out in clockwise order, the ray would intersect (if intersecting) this edge going in to the node, not going out from it
                            if (VectorMath.RightXZ(a, b, tmp_end))
                            {
                                //Since polygons are laid out in clockwise order, the ray would intersect (if intersecting) this edge going in to the node, not going out from it
                                continue;
                            }
                        }

                        float factor1, factor2;
                        if (VectorMath.LineIntersectionFactorXZ(a, b, hit.origin, tmp_end, out factor1, out factor2))
                        {
                            if (factor2 < 0) continue;

                            if (factor1 >= 0 && factor1 <= 1)
                            {
                                Vector3 intersectionPoint = a + (b - a) * factor1;
                                hit.point = intersectionPoint;
                                hit.node = node;
                                hit.tangent = b - a;
                                hit.tangentOrigin = a;

                                Pathfinding.Util.ListPool<Vector3>.Release(left);
                                Pathfinding.Util.ListPool<Vector3>.Release(right);
                                return true;
                            }
                        }
                    }

                    //Ok, this is wrong...
                    EB.Debug.LogWarning("Linecast failing because point not inside node, and line does not hit any edges of it");

                    Pathfinding.Util.ListPool<Vector3>.Release(left);
                    Pathfinding.Util.ListPool<Vector3>.Release(right);
                    return false;
                }

                node = newNode;
            }
        }

        #endregion
    }
}

#endif