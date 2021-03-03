///////////////////////////////////////////////////////////////////////
//
//  AStarPathfindingUtils.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.RVO;

public class AStarPathfindingUtils
{
    public class NavMeshQueryInput
    {
        public Vector3 from;
        public Vector3 to;
        public bool clampFromInNavMesh = false;

        public Vector3 fromNodePos;
        public NNInfo fromInfo;
        public bool isInputValid = false;

        public NavMeshQueryInput(Vector3 from, Vector3 to)
        {
            Reset(from, to);
        }

        public NavMeshQueryInput()
        {
        }

        public void Reset(Vector3 from, Vector3 to)
        {
            this.from = from;
            this.to = to;
            clampFromInNavMesh = false;

            fromInfo.node = null;
            fromInfo.constrainedNode = null;
            isInputValid = false;
        }
    }
    
    public const float NavMeshCharacterRadius = 0.3f;

    // checks to see if their is an obstacle between the point and the center of the closest node on the nav mesh
    public static bool IsPointOnNavMesh(Vector3 point, Simulator sim, NNInfo? nearestInfo = null, List<ObstacleVertex> obstaclesToTest = null)
    {
        if (null != AstarPath.active)
        {
            NNInfo info = nearestInfo ?? AstarPath.active.GetNearest(point);
            if (null != info.node)
            {
                Vector3 nodePos = (Vector3)info.node.position;
                if (null == obstaclesToTest)
                {
                    AstarPath.active.StartUsingObstaclesScratchList();
                    // sim.GetObstacles() is an alternative - this may be faster
                    Vector3 center = Vector3.Lerp(nodePos, point, 0.5f);
                    const float SearchPadding = 0.1f;
                    sim.GetStaticAndDynamicObstacles(AstarPath.active.obstaclesScratchList, center, (nodePos - center).sqrMagnitude + SearchPadding);

                    for (int i = 0; i < AstarPath.active.obstaclesScratchList.Count; i++)
                    { // do a 2d line intersection check to see if the line goes off the nav mesh
                        if (VectorMath.SegmentsIntersectXZ(nodePos, point, AstarPath.active.obstaclesScratchList[i].position, AstarPath.active.obstaclesScratchList[i].next.position))
                        {
                            AstarPath.active.StopUsingObstaclesScratchList();
                            return false; // the point is off the nav mesh                    
                        }
                    }
                    AstarPath.active.StopUsingObstaclesScratchList();
                }
                else // null != obstaclesToTest
                {
                    for (int i = 0; i < obstaclesToTest.Count; i++)
                    { // do a 2d line intersection check to see if the line goes off the nav mesh
                        if (VectorMath.SegmentsIntersectXZ(nodePos, point, obstaclesToTest[i].position, obstaclesToTest[i].next.position))
                        {
                            return false; // the point is off the nav mesh                    
                        }
                    }
                }
                return true; // the point is on the nav mesh   				
            }
        }
        return false;
    }

    // this function returns back the nearest position on the nav mesh to the passed point
    // if the point is not on the nav mesh, this function will return a position on the perimeter of the nav mesh.
    // increase 'NudgeIntoNavMesh' in order to return a point offset into the nav mesh away from the perimeter ('NudgeIntoNavMesh' can be useful for 
    // floating point inacuracies in subsequent nav mesh visibility tests)
    public static Vector3 CalculatePointOnNavMesh(Vector3 point, Simulator sim, float NudgeIntoNavMesh = 0f, NNInfo? nearestInfo = null, List<ObstacleVertex> obstaclesToTest = null)
    {
        if (null != AstarPath.active)
        {
            NNInfo info = nearestInfo ?? AstarPath.active.GetNearest(point);
            if (IsPointOnNavMesh(point, sim, info, obstaclesToTest))
            {
                return point;
            }
            else if (null != info.node)
            {
                if (NudgeIntoNavMesh != 0f)
                {
                    Vector3 diffToTriangleCenter = ((Vector3)info.node.position) - info.clampedPosition;

                    if (diffToTriangleCenter.sqrMagnitude <= NudgeIntoNavMesh * NudgeIntoNavMesh) // check this to avoid exiting the closest node, which may cause an exit of the nav mesh
                    {
                        return info.clampedPosition + diffToTriangleCenter; // this will put the returned position at the center of the closest node 
                    }
                    else
                    {	// offset into the closest node by the amount of NudgeIntoNavMesh
                        return info.clampedPosition + (diffToTriangleCenter.normalized * NudgeIntoNavMesh);
                    }
                }
                else
                {
                    return info.clampedPosition;
                }
            }
        }
        return point;
    }

    public static TriangleMeshNode GetNearestNodeOnNavMesh(Vector3 point)
    {
        NNInfo nearestInfo = AstarPath.active.GetNearest(point);
        return nearestInfo.node as TriangleMeshNode;
    }

    public static Simulator GetSimulator()
    {
        if (null == AstarPath.active)
        {
            EB.Debug.LogError("No AstarPath component found in the scene.");
            return null;
        }

        RVOSimulator sim = AstarPath.active.gameObject.GetComponent<RVOSimulator>();
        if (sim == null)
        {
            EB.Debug.LogError("No RVOSimulator component found in the scene. Please add one.");
            return null;
        }
        Simulator simulator = sim.GetSimulator();
        if (simulator == null)
        {
            EB.Debug.LogError("No Simulator component found in the scene. Please add one.");
            return null;
        }
        return simulator;
    }

    // will return the point at which the line exits the nav mesh, or 'to' if the line does not exit the nav mesh
    // if 'clampFromInNavMesh' is true, we make sure the from position is on the nav mesh before doing checks (this is an optimization to avoid checks)
    // (done in 2d, does not currently support nav meshes on top of one and other)
    public static Vector3 CalculateExitPoint(ref Vector3 from, Vector3 to, Simulator sim, bool clampFromInNavMesh = false)
    {
        if (null == AstarPath.active)
        {
            return from;
        }

        AstarPath.active.StartUsingObstaclesScratchList();
        Vector3 fromNodePos = new Vector3();
        if (!CalculateNavMeshQueryParams(ref from, to, ref fromNodePos, ref AstarPath.active.obstaclesScratchList, clampFromInNavMesh, sim))
        {
            AstarPath.active.StopUsingObstaclesScratchList();
            return from;
        }

        float closestHitMagSqr = float.MaxValue;
        Vector3 exitPoint = to;

        if (clampFromInNavMesh)
        {
            for (int i = 0; i < AstarPath.active.obstaclesScratchList.Count; i++)
            {	// do a 2d line intersection check to see if the line extends off the nav mesh
                bool doesIntersect = false;
                Vector3 intersectionPoint = VectorMath.SegmentIntersectionPointXZ(from, to, AstarPath.active.obstaclesScratchList[i].position, AstarPath.active.obstaclesScratchList[i].next.position, out doesIntersect);
                if (doesIntersect)
                {
                    if ((intersectionPoint - from).sqrMagnitude < closestHitMagSqr)
                    {
                        closestHitMagSqr = (intersectionPoint - from).sqrMagnitude;
                        exitPoint = intersectionPoint;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < AstarPath.active.obstaclesScratchList.Count; i++)
            {   // do a 2d line intersection check to see if the start point is off the nav mesh
                if (VectorMath.SegmentsIntersectXZ(fromNodePos, from, AstarPath.active.obstaclesScratchList[i].position, AstarPath.active.obstaclesScratchList[i].next.position))
                {
                    AstarPath.active.StopUsingObstaclesScratchList();
                    return from; // the start of the line is off the nav mesh
                }
                // do a 2d line intersection check to see if the line extends off the nav mesh
                bool doesIntersect = false;
                Vector3 intersectionPoint = VectorMath.SegmentIntersectionPointXZ(from, to, AstarPath.active.obstaclesScratchList[i].position, AstarPath.active.obstaclesScratchList[i].next.position, out doesIntersect);
                if (doesIntersect)
                {
                    if ((intersectionPoint - from).sqrMagnitude < closestHitMagSqr)
                    {
                        closestHitMagSqr = (intersectionPoint - from).sqrMagnitude;
                        exitPoint = intersectionPoint;
                    }
                }
            }
        }
        AstarPath.active.StopUsingObstaclesScratchList();
        return exitPoint;
    }

    // will return false if any part of the line is off the nav mesh
    // if 'clampFromInNavMesh' is true, we make sure the from position is on the nav mesh before doing checks (this is an optimization to avoid checks)
    // (done in 2d, does not currently support nav meshes on top of one and other)
    public static bool IsVisible(Vector3 from, Vector3 to, Simulator sim, bool clampFromInNavMesh = false)
    {
        if (null == AstarPath.active)
        {
            return false;
        }

        AstarPath.active.StartUsingObstaclesScratchList();
        Vector3 fromNodePos = new Vector3();
        if (!CalculateNavMeshQueryParams(ref from, to, ref fromNodePos, ref AstarPath.active.obstaclesScratchList, clampFromInNavMesh, sim))
        {
            AstarPath.active.StopUsingObstaclesScratchList();
            return false;
        }

        if (clampFromInNavMesh)
        {
            for (int i = 0; i < AstarPath.active.obstaclesScratchList.Count; i++)
            {	// do a 2d line intersection check to see if the line extends off the nav mesh
                if (VectorMath.SegmentsIntersectXZ(from, to, AstarPath.active.obstaclesScratchList[i].position, AstarPath.active.obstaclesScratchList[i].next.position))
                {
                    AstarPath.active.StopUsingObstaclesScratchList();
                    return false; // the line extends off the nav mesh                    
                }
            }
        }
        else
        {
            for (int i = 0; i < AstarPath.active.obstaclesScratchList.Count; i++)
            {   // do a 2d line intersection check to see if the start point is off the nav mesh
                if (VectorMath.SegmentsIntersectXZ(fromNodePos, from, AstarPath.active.obstaclesScratchList[i].position, AstarPath.active.obstaclesScratchList[i].next.position))
                {
                    AstarPath.active.StopUsingObstaclesScratchList();
                    return false; // the start point is off the nav mesh                    
                }
                // do a 2d line intersection check to see if the line extends off the nav mesh
                if (VectorMath.SegmentsIntersectXZ(from, to, AstarPath.active.obstaclesScratchList[i].position, AstarPath.active.obstaclesScratchList[i].next.position))
                {
                    AstarPath.active.StopUsingObstaclesScratchList();
                    return false; // the line extends off the nav mesh                    
                }
            }
        }
        AstarPath.active.StopUsingObstaclesScratchList();
        return true; // the line is fully on the nav mesh                             
    }

    private static List<Vector3> _allPoints = null;

    private static void CreateAllPointsList()
    {
        if (null == _allPoints)
        {
            const int NumPointsCapacity = 6;
            _allPoints = new List<Vector3>(NumPointsCapacity);
        }

    }

    // calculate all the obstacles which will be used in multiple nav mesh IsVisible tests (optimization to avoid doing multiple sim.KDTree.GetObstacles())
    // if all the from points are the same in the 'theInput' list, set 'areFromPointsEqual' to true for an optimization
    public static bool CalculateNavMeshQueryObstacles(ref List<NavMeshQueryInput> theInput, ref List<ObstacleVertex> obstacles, Simulator sim, bool areFromPointsEqual)
    {
        if (null == AstarPath.active)
        {
            return false;
        }

        CreateAllPointsList();
        _allPoints.Clear();
        for (int inputIndex = 0; inputIndex < theInput.Count; ++inputIndex)
        {
            NavMeshQueryInput query = theInput[inputIndex];
            query.isInputValid = true;

            _allPoints.Add(query.from);
            _allPoints.Add(query.to);

            if (!areFromPointsEqual || 0 == inputIndex) // if from points are not equal or this is the first in the list
            {
                query.fromInfo = AstarPath.active.GetNearest(query.from);
            }
            else // (areFromPointsEqual && inputIndex > 0) // from points are equal and we've already calculated one GetNearest()
            {
                query.fromInfo = theInput[0].fromInfo;
            }

            if (null == query.fromInfo.node)
            {
                query.isInputValid = false;
                continue;
            }
            query.fromNodePos = (Vector3)query.fromInfo.node.position;
            _allPoints.Add(query.fromNodePos);
        }

        Bounds bounds = GameUtils.CalculateBounds(_allPoints); // calculate the bounding box to encapsulate all points we need to consider
        // sim.GetObstacles() is an alternative - this may be faster
        const float SearchPadding = 0.1f;
        sim.GetStaticAndDynamicObstacles(obstacles, bounds.center, (bounds.max - bounds.center).sqrMagnitude+SearchPadding); // get all obstacles near our line
        return true;
    }

    // will return false if any part of the line is off the nav mesh
    // if 'clampFromInNavMesh' is true, we make sure the from position is on the nav mesh before doing checks (this is an optimization to avoid checks)
    // (done in 2d, does not currently support nav meshes on top of one and other)
    public static bool IsVisible(NavMeshQueryInput input, List<ObstacleVertex> obstacles, Simulator sim)
    {
        if (!input.isInputValid)
        {
            return false;
        }

        if (input.clampFromInNavMesh)
        {
            input.from = CalculatePointOnNavMesh(input.from, sim, 0.1f, input.fromInfo, obstacles);	
            int obstaclesCount = obstacles.Count;
            for (int i = 0; i < obstaclesCount; i++)
            {	// do a 2d line intersection check to see if the line extends off the nav mesh
                if (VectorMath.SegmentsIntersectXZ(input.from, input.to, obstacles[i].position, obstacles[i].next.position))
                {
                    return false; // the line extends off the nav mesh                    
                }
            }
        }
        else
        {
            int obstaclesCount = obstacles.Count;
            for (int i = 0; i < obstaclesCount; i++)
            {   // do a 2d line intersection check to see if the start point is off the nav mesh
                if (VectorMath.SegmentsIntersectXZ(input.fromNodePos, input.from, obstacles[i].position, obstacles[i].next.position))
                {
                    return false; // the start point is off the nav mesh                    
                }
                // do a 2d line intersection check to see if the line extends off the nav mesh
                if (VectorMath.SegmentsIntersectXZ(input.from, input.to, obstacles[i].position, obstacles[i].next.position))
                {
                    return false; // the line extends off the nav mesh                    
                }
            }
        }
        return true; // the line is fully on the nav mesh                             
    }

    public static List<Vector3> GetRandomPointsOnNavmesh(Vector3 origin, int count, float maxRadius, float minRadius = 0.0f)
    {
        List<TriangleMeshNode> validNodes = GetAllNodesInRadius(origin, maxRadius);

        // remove nodes in smaller radius
        if (minRadius > 0.0f)
        {
            List<TriangleMeshNode> invalidNodes = GetAllNodesInRadius(origin, minRadius);
            for (int i = 0; i < invalidNodes.Count; i++)
            {
                validNodes.Remove(invalidNodes[i]);
            }
        }

        List<Vector3> points = new List<Vector3>();
        if (validNodes.Count == 0)
        {
            //EB.Debug.LogError("No valid navmesh nodes found!");
            return points;
        }
        for (int i = 0; i < count; i++)
        {
            TriangleMeshNode randomNode = validNodes[UnityEngine.Random.Range(0, validNodes.Count)];
            Vector3 randomPoint = RandomPointInTrangle((Vector3)randomNode.GetVertex(0), (Vector3)randomNode.GetVertex(1), (Vector3)randomNode.GetVertex(2));

            float dist = (randomPoint - origin).magnitude;
            if (dist <= maxRadius && dist > minRadius)
            {
                points.Add(randomPoint);
            }
            else 
            {
                points.Add(origin + (randomPoint - origin).normalized * (minRadius + maxRadius) / 2.0f);
            }
        }

        return points;
    }

    public static List<TriangleMeshNode> GetAllNodesInRadius(Vector3 origin, float radius)
    {
        //float radiusSquared = radius * radius;

        TriangleMeshNode root = AStarPathfindingUtils.GetNearestNodeOnNavMesh(origin);

        List<TriangleMeshNode> validNodes = new List<TriangleMeshNode>();
        EB.Collections.Queue<TriangleMeshNode> toCheck = new EB.Collections.Queue<TriangleMeshNode>();
        HashSet<TriangleMeshNode> visited = new HashSet<TriangleMeshNode>();

        toCheck.Enqueue(root);
        while (toCheck.Count > 0)
        {
            TriangleMeshNode curNode = toCheck.Dequeue();

            if (SphereXZTriangleIntersect(origin, radius, (Vector3)curNode.GetVertex(0), (Vector3)curNode.GetVertex(1), (Vector3)curNode.GetVertex(2)))
            {
                validNodes.Add(curNode);
                for (int i = 0; i < curNode.connections.Length; i++)
                {
                    TriangleMeshNode connection = curNode.connections[i] as TriangleMeshNode;
                    if (!visited.Contains(connection))
                    {
                        toCheck.Enqueue(connection);
                    }
                }
            }

            visited.Add(curNode);
        }

        return validNodes;
    }

    // checks to see if their is an obstacle between the point and the center of the closest node on the nav mesh
    public static bool IsPointOnRecastGraph(Vector3 point, NNInfo? nearestInfo = null)
    {
        NNInfo info = nearestInfo ?? AstarPath.active.GetNearest(point);
        NavGraph graph = AstarData.GetGraph(info.node);

        if (graph != null)
        {
            IRaycastableGraph rayGraph = graph as IRaycastableGraph;
            if (rayGraph != null)
            {
                GraphHitInfo hit;
                return !rayGraph.Linecast(((Vector3)info.node.position), point, info.node, out hit);
            }
            return false; // no recast graph
        }
        return false; // no nav mesh		
    }

    // this function returns back the nearest position on the nav mesh to the passed point
    // if the point is not on the nav mesh, this function will return a position on the perimeter of the nav mesh.
    // increase 'NudgeIntoNavMesh' in order to return a point offset into the nav mesh away from the perimeter ('NudgeIntoNavMesh' can be useful for 
    // floating point inacuracies in subsequent nav mesh visibility tests)
    public static Vector3 CalculatePointOnRecastGraph(Vector3 point, float NudgeIntoNavMesh = 0f, NNInfo? nearestInfo = null)
    {
        if (null != AstarPath.active)
        {
            NNInfo info = nearestInfo ?? AstarPath.active.GetNearest(point);
            if (IsPointOnRecastGraph(point, info))
            {
                return point;
            }
            else if (null != info.node)
            {
                if (NudgeIntoNavMesh != 0f)
                {
                    Vector3 diffToTriangleCenter = ((Vector3)info.node.position) - info.clampedPosition;

                    if (diffToTriangleCenter.sqrMagnitude <= NudgeIntoNavMesh * NudgeIntoNavMesh) // check this to avoid exiting the closest node, which may cause an exit of the nav mesh
                    {
                        return info.clampedPosition + diffToTriangleCenter; // this will put the returned position at the center of the closest node 
                    }
                    else
                    {	// offset into the closest node by the amount of NudgeIntoNavMesh
                        return info.clampedPosition + (diffToTriangleCenter.normalized * NudgeIntoNavMesh);
                    }
                }
                else
                {
                    return info.clampedPosition;
                }
            }
        }
        return point;
    }

    // will return the point at which the line exits the nav mesh, or 'to' if the line does not exit the nav mesh
    // if 'clampFromInNavMesh' is true, we make sure the from position is on the nav mesh before doing checks
    // (done in 2d, does not currently support nav meshes on top of one and other)
    public static Vector3 CalculateExitPointOfRecastGraph(ref Vector3 from, Vector3 to, bool clampFromInNavMesh = false)
    {
        NNInfo fromInfo = AstarPath.active.GetNearest(from);
        if (null == fromInfo.node)
        {
            return from;
        }

        if (clampFromInNavMesh)
        {
            from = CalculatePointOnRecastGraph(from, 0.1f, fromInfo);			
        }

        NavGraph graph = AstarData.GetGraph(fromInfo.node);

        if (graph != null)
        {
            IRaycastableGraph rayGraph = graph as IRaycastableGraph;
            if (rayGraph != null)
            {
                GraphHitInfo hit;
                if (rayGraph.Linecast(from, to, fromInfo.node, out hit))
                {
                    return hit.point;
                }
                return to; // no nav mesh exit
            }
            return from; // no recast graph
        }
        return from; // no nav mesh	
    }

    // will return false if any part of the line is off the nav mesh
    // if 'clampFromInNavMesh' is true, we make sure the from position is on the nav mesh before doing checks
    // (done in 2d, does not currently support nav meshes on top of one and other)
    public static bool IsVisibleOnRecastGraph(Vector3 from, Vector3 to, bool clampFromInNavMesh = false, NNInfo? nearestInfo = null)
    {
        if (null == AstarPath.active)
        {
            return false;
        }

        NNInfo fromInfo = nearestInfo ?? AstarPath.active.GetNearest(from);
        if (null == fromInfo.node)
        {
            return false;
        }

        if (clampFromInNavMesh)
        {
            from = CalculatePointOnRecastGraph(from, 0.1f, fromInfo);
        }

        NavGraph graph = AstarData.GetGraph(fromInfo.node);

        if (graph != null)
        {
            IRaycastableGraph rayGraph = graph as IRaycastableGraph;
            if (rayGraph != null)
            {
                GraphHitInfo hit;
                if (rayGraph.Linecast(from, to, fromInfo.node, out hit))
                {
                    return false; // hit an obstacle
                }
                return true; // no nav mesh exit
            }
            return false; // no recast graph
        }
        return false; // no nav mesh	
    }

    // checks to see if the point is actually inside the closest triangle on the nav mesh
    public static bool IsPointOnNavMeshOptimized(Vector3 point, NNInfo? nearestInfo = null)
    {
        if (null != AstarPath.active)
        {
            NNInfo pointInfo = nearestInfo ?? AstarPath.active.GetNearest(point);
            TriangleMeshNode closestTriangle = pointInfo.node as TriangleMeshNode;
            if (null != closestTriangle)
            {
                return closestTriangle.ContainsPoint((Int3)point);
            }			
        }
        return false;
    }

    // this function returns back the nearest position on the nav mesh to the passed point
    // if the point is not on the nav mesh, this function will return a position on the perimeter of the nav mesh.
    // increase 'NudgeIntoNavMesh' in order to return a point offset into the nav mesh away from the perimeter ('NudgeIntoNavMesh' can be useful for 
    // floating point inacuracies in subsequent nav mesh visibility tests)
    public static Vector3 CalculatePointOnNavMeshOptimized(Vector3 point, float NudgeIntoNavMesh = 0f, NNInfo? nearestInfo = null)
    {
        if (null != AstarPath.active)
        {
            NNInfo info = nearestInfo ?? AstarPath.active.GetNearest(point);
            if (IsPointOnNavMeshOptimized(point, info))
            {
                return point;
            }
            else if (null != info.node)
            {
                if (NudgeIntoNavMesh != 0f)
                {
                    Vector3 diffToTriangleCenter = ((Vector3)info.node.position) - info.clampedPosition;

                    if (diffToTriangleCenter.sqrMagnitude <= NudgeIntoNavMesh * NudgeIntoNavMesh) // check this to avoid exiting the closest node, which may cause an exit of the nav mesh
                    {
                        return info.clampedPosition + diffToTriangleCenter; // this will put the returned position at the center of the closest node 
                    }
                    else
                    {	// offset into the closest node by the amount of NudgeIntoNavMesh
                        return info.clampedPosition + (diffToTriangleCenter.normalized * NudgeIntoNavMesh);
                    }
                }
                else
                {
                    return info.clampedPosition;
                }
            }
        }
        return point;
    }

    // will return the point at which the line exits the nav mesh, or 'to' if the line does not exit the nav mesh
    // if 'clampFromInNavMesh' is true, we make sure the from position is on the nav mesh before doing checks
    // (done in 2d, does not currently support nav meshes on top of one and other)
    public static Vector3 CalculateExitPointOfNavMeshOptimized(ref Vector3 from, Vector3 to, bool clampFromInNavMesh, NNInfo? nearestInfo)
    {
        Vector3 exitPoint = Vector3.zero;
        IsVisibleOnNavMeshOptimized(ref from, to, clampFromInNavMesh, nearestInfo, ref exitPoint, null);
        return exitPoint;
    }

    // will return false if any part of the line is off the nav mesh
    // if 'clampFromInNavMesh' is true, we make sure the from position is on the nav mesh before doing checks
    // (done in 2d, does not currently support nav meshes on top of one and other)
    public static bool IsVisibleOnNavMeshOptimized(Vector3 from, Vector3 to)
    {
        Vector3 notUsed = Vector3.zero;
        return IsVisibleOnNavMeshOptimized(ref from, to, false, null, ref notUsed, null);
    }
    
    // will return false if any part of the line is off the nav mesh
    // if 'clampFromInNavMesh' is true, we make sure the from position is on the nav mesh before doing checks
    // (done in 2d, does not currently support nav meshes on top of one and other)
    public static bool IsVisibleOnNavMeshOptimized(ref Vector3 from, Vector3 to, bool clampFromInNavMesh, NNInfo? nearestInfo, ref Vector3 endPoint, List<TriangleMeshNode> outList)
    {
        endPoint = from;
        if (null == AstarPath.active)
        {
            return false; // no nav mesh
        }

        NNInfo fromInfo = nearestInfo ?? AstarPath.active.GetNearest(from);
        TriangleMeshNode currentTriangle = fromInfo.node as TriangleMeshNode;
        if (null == currentTriangle)
        {
            return false; // no nav mesh
        }

        if (clampFromInNavMesh)
        {
            endPoint = from = CalculatePointOnNavMeshOptimized(from, 0.1f, fromInfo);
        }
        else if (!currentTriangle.ContainsPoint((Int3)from))
        {
            return false; // start point is off the nav mesh
        }

        bool isVisible = false;
        AstarPath.active.StartUsingTriangleScratchList();
        while (null != currentTriangle)
        {
            if (null != outList)
            {
                outList.Add(currentTriangle);
            }

            TriangleMeshNode nextTriangle = null;
            if (DoesLineExitTriangle(from, to, currentTriangle, ref AstarPath.active.trianglesHashSet, ref nextTriangle, ref endPoint)) // the end point returned will be on the ground plane (y value zero)
            {
                if (null == nextTriangle) // no next triangle, so the line exits off the edge of the nav mesh ('while' loop will end)
                {
                    VirticalRayPlaneIntersection(new Vector2(endPoint.x, endPoint.z), currentTriangle, ref endPoint); // make sure the endPoint is on the plane of the triangle
                    isVisible = false; // exits the nav mesh
                }
            }
            else // does not exit triangle
            {
                VirticalRayPlaneIntersection(new Vector2(to.x, to.z), currentTriangle, ref endPoint); // make sure the endPoint is on the plane of the triangle
                isVisible = true; // the line cast ends inside this triangle, no nextTriangle, nextTriangle will be null ('while' loop will end)
            }
            AstarPath.active.trianglesHashSet.Add(currentTriangle);
            currentTriangle = nextTriangle;
        }
        AstarPath.active.StopUsingTriangleScratchList();
        return isVisible;  
    }

    // has the nav mesh already been generated from tagged geometry and the data cached for load on level begin
    public static bool HasNavMeshBeenCorrectlyAutoGenerated()
    {
        if (null != AstarPath.active && 1 == AstarPath.active.graphs.Length && AstarPath.active.graphs[0] is RecastGraph) // we need a single recast
        {
            RecastGraph recast = AstarPath.active.graphs[0] as RecastGraph;
            if (!recast.generateFromInputMesh) // it should NOT be marked for generation from a pre built input mesh
            {
                // we need data for the neav mesh
                if (AstarPath.active.astarData.cacheStartup && AstarPath.active.astarData.data_cachedStartup != null && AstarPath.active.astarData.data_cachedStartup.Length > 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // has a mesh been pre built which will be used to create the nav mesh on level begin
    public static bool WillNavMeshBeBuiltFromInputMeshOnLevelBegin()
    {
        if (null != AstarPath.active && (null == AstarPath.active.graphs || 0 == AstarPath.active.graphs.Length)) // should be no graphs
        {	// their should be no cached data
            if (!AstarPath.active.astarData.cacheStartup && (null == AstarPath.active.astarData.data_cachedStartup || 0 == AstarPath.active.astarData.data_cachedStartup.Length))
            {
                return true;
            }			
        }
        return false;
    }

    static public bool CreateRecastNavMeshForInputMeshes(Transform zonesRootTransform, bool singleInput)
    {
        if (CreateAPPNavMesh(false, null, zonesRootTransform, false, 0f, false))
        {
            SetAPPToUseInputMesh(singleInput);
            return true;
        }
        return false;
    }

    static public void SetAPPToUseInputMesh(bool singleInput)
    {
        if (AstarPath.active == null)
        {
            EB.Debug.LogError("LevelHelperEditor::SetAPPToUseInputMesh AStar object not found!");
            return;
        }

        AstarPath.active.astarData.data_cachedStartup = null;
        AstarPath.active.astarData.cacheStartup = false;
        AstarPath.active.showNavGraphs = false;
        RecastGraph recastGraph = (RecastGraph)AstarPath.active.astarData.FindGraphOfType(typeof(RecastGraph));
        if (null != recastGraph)
        {
            recastGraph.generateFromInputMesh = true;
            recastGraph.isSingleInputMesh = singleInput;
        }
    }

    // A star pathfinding pro (APP)
    static public bool CreateAPPNavMesh(bool multiTile, Transform singleZone, Transform zonesRootTransform, bool doActivateAndDeactivateGeoForGeneration, float navBoundsOffset, bool doScan)
    {
        if (AstarPath.active == null)
        {
            EB.Debug.LogError("LevelHelperEditor::ExportLevel AStar object not found!");
            return false;
        }

        AstarPath.active.graphs = new NavGraph[0]; // clear graphs

        GameObject zonesRoot = zonesRootTransform.gameObject;
        if (zonesRoot == null)
        {
            EB.Debug.LogError("LevelHelperEditor::Zones gameObject not found!");
            return false;
        }

        // re-init valid graph types in underlying AstarData object
        AstarPath.active.astarData.FindGraphTypes();

        const float kBoundingBoxHeight = 100.0f; // this is an arbitrary amount, if there is ever a tall level, this can be increased
        RecastGraph recastGraph = null;
        float gridSizeX = EditorVars.GridSize;
        float gridSizeY = EditorVars.GridSize;
        float gridSizeZ = EditorVars.GridSize;
        if (null != singleZone)
        {
            recastGraph = (RecastGraph)AstarPath.active.astarData.AddGraph(typeof(RecastGraph));

            GlobalNavHelper nav_helper = singleZone.GetComponent<GlobalNavHelper>();
            if(nav_helper != null)
            {
                gridSizeX = nav_helper.m_Range.x;
                gridSizeY = nav_helper.m_Range.y;
                gridSizeZ = nav_helper.m_Range.z;
            }

            Vector3 center = new Vector3(singleZone.position.x + gridSizeX / 2.0f, 0.0f, singleZone.position.z + gridSizeZ / 2.0f);
            Vector3 boundingBox = Vector3.zero;
            if (multiTile)
            { // if it's multi tile, we create a bounding box which is three times the size of our zone, so that our zone will be placed in the center				
                boundingBox = new Vector3(gridSizeX * 3f, gridSizeY * 3f, gridSizeZ * 3f);
                SetNavMeshDefaults(recastGraph, center, boundingBox, multiTile);
                recastGraph.tileSizeX = recastGraph.tileSizeZ = (int)(EditorVars.GridSize / recastGraph.cellSize); // this line sets the tile size so that each tile will hold the size of a zone
                recastGraph.useCenterTileOnly = true;
            }
            else
            {
                boundingBox = new Vector3(gridSizeX, gridSizeY, gridSizeZ);
                SetNavMeshDefaults(recastGraph, center, boundingBox, multiTile);
            }
        }
        else
        {
            // or geo can be tested to get the y extents
            Vector3 NavMeshBoundingBoxMin = new Vector3(float.MaxValue, -(kBoundingBoxHeight * 0.5f), float.MaxValue);
            Vector3 NavMeshBoundingBoxMax = new Vector3(-float.MaxValue, kBoundingBoxHeight * 0.5f, -float.MaxValue);

            // walk zones to generate the bounding box encompassing the entire level (all zones)
            foreach (Transform zone in zonesRoot.transform)
            {
                
                gridSizeX = EditorVars.GridSize;
                gridSizeY = EditorVars.GridSize;
                gridSizeZ = EditorVars.GridSize;
                GlobalNavHelper nav_helper = zone.GetComponent<GlobalNavHelper>();
                if(nav_helper != null)
                {
                    gridSizeX = nav_helper.m_Range.x;
                    gridSizeY = nav_helper.m_Range.y;
                    gridSizeZ = nav_helper.m_Range.z;
                }

                float zoneXMin = zone.position.x + navBoundsOffset;
                float zoneXMax = zone.position.x + gridSizeX + navBoundsOffset;

                float zoneZMin = zone.position.z + navBoundsOffset;
                float zoneZMax = zone.position.z + gridSizeZ + navBoundsOffset;

                NavMeshBoundingBoxMin.x = zoneXMin < NavMeshBoundingBoxMin.x ? zoneXMin : NavMeshBoundingBoxMin.x;
                NavMeshBoundingBoxMax.x = zoneXMax > NavMeshBoundingBoxMax.x ? zoneXMax : NavMeshBoundingBoxMax.x;

                NavMeshBoundingBoxMin.z = zoneZMin < NavMeshBoundingBoxMin.z ? zoneZMin : NavMeshBoundingBoxMin.z;
                NavMeshBoundingBoxMax.z = zoneZMax > NavMeshBoundingBoxMax.z ? zoneZMax : NavMeshBoundingBoxMax.z;
            }

            Vector3 NavMeshBoundingBoxCenter = Vector3.Lerp(NavMeshBoundingBoxMin, NavMeshBoundingBoxMax, 0.5f);
            Vector3 NavMeshBoundingBox = NavMeshBoundingBoxMax - NavMeshBoundingBoxMin;

            // one monolithic nav mesh to encompass all zones
            recastGraph = (RecastGraph)AstarPath.active.astarData.AddGraph(typeof(RecastGraph));
            SetNavMeshDefaults(recastGraph, NavMeshBoundingBoxCenter, NavMeshBoundingBox, multiTile);
            recastGraph.useCenterTileOnly = false;
        }

        AstarPath.active.astarData.data_cachedStartup = null;
        AstarPath.active.astarData.cacheStartup = false;
        recastGraph.generateFromInputMesh = false;
        AstarPath.active.showNavGraphs = true;

        if (doScan)
        {
            LevelHelper.FusionMenuScan(doActivateAndDeactivateGeoForGeneration); // build navmeshes from recast graph      
        }
        
        return true;
    }

    // set up the default values for a nav mesh
    static private void SetNavMeshDefaults(RecastGraph recastGraph, Vector3 center, Vector3 boundingBox, bool isMultiTileRequired)
    {
        recastGraph.characterRadius = NavMeshCharacterRadius; // the larger this is, the larger the borders around the nav mesh will be
        recastGraph.cellSize = 0.5f; // 0.2f; // a lower value for this generates a curved border around objects (which means character gets stuck less, as no sharp corners)
        recastGraph.walkableClimb = 1.0f;
        recastGraph.maxSlope = 45.0f;
        recastGraph.showMeshOutline = true;
        recastGraph.contourMaxError = 1f; // the smaller this is the closer the edge of the nav mesh will match the edge of geo, a large value
        // will mean the edge of the nav mesh will be straight even if their is a jagged edge to the geo
        recastGraph.mask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Obstacle"));
        recastGraph.excludeMask = (1 << LayerMask.NameToLayer("Obstacle"));
        recastGraph.forcedBoundsCenter = center;
        recastGraph.forcedBoundsSize = boundingBox;
        if (!isMultiTileRequired)
        {
            recastGraph.SetUpNavMeshToFitOnOneTile();
        }
    }

    private static bool SharedEdge(TriangleMeshNode triangle, GraphNode other, ref int sharedEdge, ref int otherSharedEdge)
    {
        triangle.GetPortal(other, null, null, false, out sharedEdge, out otherSharedEdge);
        return sharedEdge > -1;
    }

    // this is a utility function used by IsVisibleOnNavMeshOptimized, before calling this function is should be determined that the line 'from' -> 'to' starts outside 
    // the triangle, and enters the triangle, this function will then determine if the line exits the triangle
    // the exitPoint returned will be on the ground plane
    private static bool DoesLineExitTriangle(Vector3 from, Vector3 to, TriangleMeshNode currentTriangle, ref HashSet<TriangleMeshNode> previousTriangles, ref TriangleMeshNode nextTriangle, ref Vector3 exitPoint)
    {
        if (currentTriangle.ContainsPoint((Int3)to)) // does the line end in this triangle
        {
            return false; // the line does not exit this triangle
        }

        const int NumVerts = 3;
        if (null != currentTriangle.connections)
        {
            for (int connect = 0; connect < currentTriangle.connections.Length; connect++) // go through all our connections
            {
                TriangleMeshNode other = currentTriangle.connections[connect] as TriangleMeshNode;

                int edge = -1;
                int otherEdge = -1;
                if (other != null && !previousTriangles.Contains(other) && SharedEdge(currentTriangle, other, ref edge, ref otherEdge)) // get the connecting edge
                {
                    bool intersectsEdge = false;
                    bool intersectsOtherEdge = true;
                    // check to see if the line enters the other triangle through this edge
                    exitPoint = VectorMath.SegmentIntersectionPointXZ(from, to, (Vector3)currentTriangle.GetVertex(edge), (Vector3)currentTriangle.GetVertex((edge + 1) % NumVerts), out intersectsEdge);

                    int tileIndex = (currentTriangle.GetVertexIndex(0) >> RecastGraph.TileIndexOffset) & RecastGraph.TileIndexMask;
                    int otherTileIndex = (other.GetVertexIndex(0) >> RecastGraph.TileIndexOffset) & RecastGraph.TileIndexMask;
                    if (tileIndex != otherTileIndex) // if the edjoining triangles are on different tiles, the edges may not match exactly, so we need to be sure that both triangles edges are intersected
                    {
                        
                        exitPoint = VectorMath.SegmentIntersectionPointXZ(from, to, (Vector3)other.GetVertex(otherEdge), (Vector3)other.GetVertex((otherEdge + 1) % NumVerts), out intersectsOtherEdge);					
                    }
                    if (intersectsEdge && intersectsOtherEdge)
                    {
                        nextTriangle = other;
                        return true;
                    }
                }
            }
        }

        Vector3 prevVertPos = (Vector3)currentTriangle.GetVertex(0);
        float farthestIntersectionSqr = -1f;
        for (int vert = 1; vert <= NumVerts; ++vert) // go over all the edges, to see which one the line passes out of, so we can calculate the triangle exit point
        {
            Vector3 vertPos = (Vector3)currentTriangle.GetVertex(vert % NumVerts);

            bool intersects = false;
            Vector3 potentialExitPoint = VectorMath.SegmentIntersectionPointXZ(from, to, prevVertPos, vertPos, out intersects);
            if (intersects)
            {
                float intersectionSqr = (potentialExitPoint - from).sqrMagnitude;
                // we're looking for the farthest intersection, as the nearest intersection would have been the edge intersected when the line entered the triangle
                if (intersectionSqr > farthestIntersectionSqr)
                {
                    farthestIntersectionSqr = intersectionSqr;
                    exitPoint = potentialExitPoint;
                }
            }
            prevVertPos = vertPos;
        }
        return farthestIntersectionSqr > 0f;
    }

    // see if an infinite virtical ray intersects the plane of the triangle node
    public static bool VirticalRayPlaneIntersection(Vector2 rayFrom, TriangleMeshNode node, ref Vector3 intersectionPoint)
    {				
        Vector3 vertZero = (Vector3)node.GetVertex(0);
        Vector3 vertOne = (Vector3)node.GetVertex(1);
        Vector3 vertTwo = (Vector3)node.GetVertex(2);

        Vector3 nodeNormal = Vector3.Cross(vertOne - vertZero, vertTwo - vertZero).normalized;
        Plane plane = new Plane(nodeNormal, (Vector3)node.position); // create a plane from the triangle normal and a position on the triangle plane

        // make sure our ray starts below the triangle, so a virtical up ray will hit it
        float minTriangleY = Mathf.Min(vertZero.y, Mathf.Min(vertOne.y, vertTwo.y)) - 1f;
        Vector3 rayFrom3d = new Vector3(rayFrom.x, minTriangleY, rayFrom.y);
        Ray ray = new Ray(rayFrom3d, Vector3.up);

        float dist = 0f;
        if (plane.Raycast(ray, out dist))
        {
            intersectionPoint = rayFrom3d + Vector3.up * dist;
            return true;
        }
        return false;
    }

    private static bool CalculateNavMeshQueryParams(ref Vector3 from, Vector3 to, ref Vector3 fromNodePos, ref List<ObstacleVertex> obstacles, bool clampFromInNavMesh, Simulator sim)
    {
        if (null == AstarPath.active)
        {
            return false;
        }

        CreateAllPointsList();
        _allPoints.Clear();
        _allPoints.Add(from); _allPoints.Add(to);

        NNInfo fromInfo = AstarPath.active.GetNearest(from);
        if (null == fromInfo.node)
        {
            return false;
        }
        fromNodePos = (Vector3)fromInfo.node.position;
        _allPoints.Add(fromNodePos);

        Bounds bounds = GameUtils.CalculateBounds(_allPoints); // calculate the bounding box to encapsulate all points we need to consider
        // sim.GetObstacles() is an alternative - this may be faster
        const float SearchPadding = 0.1f;
        sim.GetStaticAndDynamicObstacles(obstacles, bounds.center, (bounds.max - bounds.center).sqrMagnitude+SearchPadding); // get all obstacles near our line

        if (clampFromInNavMesh)
        {
            from = CalculatePointOnNavMesh(from, sim, 0.1f, fromInfo, obstacles);
        }
        return true;
    }

    //http://mathworld.wolfram.com/TrianglePointPicking.html
    //Not using the most uniform method, but should be good enough for basic purposes
    private static Vector3 RandomPointInTrangle(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        v1 = v1 - v0;
        v2 = v2 - v0;

        float a1 = UnityEngine.Random.value;
        float a2 = UnityEngine.Random.value;

        return (a1 * v1) + ((1 - a1) * a2 * v2) + v0;
    }

    //Used to intersect sphere check navmesh triangles.
    //Would like to go back and use a less hacky method of testing if the center point of the sphere is inside the triangle
    private static bool SphereXZTriangleIntersect(Vector3 origin, float radius, Vector3 a, Vector3 b, Vector3 c)
    {
        if (IsPointInCircle(a, origin, radius)) 
            return true;
        if (IsPointInCircle(b, origin, radius))
            return true;
        if (IsPointInCircle(c, origin, radius))
            return true;
        if (IsPointInCircle(ClosestPointOnSegment(origin, a, b), origin, radius))
            return true;
        if (IsPointInCircle(ClosestPointOnSegment(origin, b, c), origin, radius))
            return true;
        if (IsPointInCircle(ClosestPointOnSegment(origin, a, c), origin, radius))
            return true;

        // Vector3.Angle always return the smallest angle between two vectors i.e. < 180 degrees.
        // So for a point inside the triangle, the sum of all angles will be 360 (no single angle is greater than 180 so the sum will work)
        // Otherwise, we have one angle that's bigger than 180 and Unity's function will give us the complimentary of that angle (360 - angle)
        // and the sum won't be 360 anymore (it will be less)
        float totalAngle = 0.0f;
        totalAngle += Vector3.Angle(a - origin, b - origin);
        totalAngle += Vector3.Angle(b - origin, c - origin);
        totalAngle += Vector3.Angle(c - origin, a - origin);

        if (Mathf.Approximately(totalAngle, 360.0f))
            return true;
        //if (!(new Plane(a, b, a + new Vector3(0, 1, 0))).GetSide(origin) && !(new Plane(b, c, b + new Vector3(0, 1, 0))).GetSide(origin) && !(new Plane(c, a, a + new Vector3(0, 1, 0))).GetSide(origin))
            //return true; //Origin is inside the triangle (on the same side of 3 vertical edge planes). I'm sure there's a better way to do this

        return false;
    }

    private static bool IsPointInCircle(Vector3 point, Vector3 origin, float radius)
    {
        return (origin - point).sqrMagnitude <= radius * radius;
    }

    private static Vector3 ClosestPointOnSegment(Vector3 p, Vector3 segStart, Vector3 segEnd)
    {
        Vector3 ab = segEnd - segStart;
        float t = Vector3.Dot(p - segStart, ab) / Vector3.Dot(ab, ab);
        if(t < 0.0f) t = 0.0f;
        if(t > 1.0f) t = 1.0f;
        return Vector3.Lerp(segStart, segEnd, t);
    }
}
