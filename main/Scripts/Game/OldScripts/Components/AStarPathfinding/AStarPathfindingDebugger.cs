///////////////////////////////////////////////////////////////////////
//
//  AStarPathfindingDebugger.cs
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

[ExecuteInEditMode]
public class AStarPathfindingDebugger : MonoBehaviour
#if DEBUG
	, IDebuggableEx 
#endif
{
	public bool doDrawLines = false;
	public bool doDrawMesh = false;
	public int closeDistanceEdges = -1;
	public bool doDrawEdgeTop = false;
	public bool doDrawEdgeNormals = false;
	public bool doDrawNodeCosts = false;
	public bool doPrintNodeCosts = false;
	public float navMeshAlpha = 0.5f;
	public bool debugNavMeshExistence = false;
	public bool tryLoadNavMeshIfNonExists = false;

#if DEBUG

	const int DisplayAllRegions = -1;
	public int desplaySpecifiedRegion = DisplayAllRegions;

	private Pathfinding.RVO.Simulator _sim = null;

	public void OnPreviousValuesLoaded()
	{
		TurnOffAllDebugging();
	}

	public void OnValueChanged(System.Reflection.FieldInfo field, object oldValue, object newValue)
	{
	}

	void OnDrawGizmos()
	{
		if (AstarPath.active)
		{
			OnDrawDebug();
		}
	}

	void OnEnable()
	{
		//DebugSystem.RegisterSystem("Pathfinding", this);
	}

	void OnDisable()
	{
		//DebugSystem.UnregisterSystem(this);
	}

	public void TurnOffAllDebugging()
	{
		doDrawLines = false;
		doDrawMesh = false;
		closeDistanceEdges = -1;
		doDrawEdgeTop = false;
		doDrawEdgeNormals = false;
		doDrawNodeCosts = false;
		doPrintNodeCosts = false;
		debugNavMeshExistence = false;
		tryLoadNavMeshIfNonExists = false;
	}

	// what is the debug color for the specified node
	public static Color CalculateNodeDebugColor(GraphNode node)
	{
		if (null != AstarPath.active && null != AstarPath.active.graphs && node.GraphIndex < AstarPath.active.graphs.Length)
		{
			NavGraph graph = AstarPath.active.graphs[node.GraphIndex];
			return graph.NodeColor(node, AstarPath.active.debugPathData);			
		}
		return Color.white;
	}

	public void Update()
	{
		if (tryLoadNavMeshIfNonExists)
		{
			if (null == AstarPath.active.graphs || 0 == AstarPath.active.graphs.Length || 0 == CalculateNumNavMeshTriangles())
			{
				if (AstarPath.active.astarData.cacheStartup && AstarPath.active.astarData.data_cachedStartup != null)
				{
					AstarPath.active.astarData.LoadFromCache();
				}
				else
				{
					AstarPath.active.astarData.DeserializeGraphs();
				}
			}
		}
	}

	static private int CalculateNumNavMeshTriangles()
	{
		int numTriangles = 0;
		for (int graphIndex = 0; graphIndex < AstarPath.active.graphs.Length; ++graphIndex)
		{
			INavmesh ng = AstarPath.active.graphs[graphIndex] as INavmesh;
			if (null != ng)
			{
				ng.GetNodes(delegate(GraphNode _node)
							{
					++numTriangles;
					return true;
				});
			}
		}
		return numTriangles;
	}
	
	public void OnDrawDebug()
	{
		if (AstarPath.active)
		{
			if (doDrawMesh)
			{
				DrawTriangles();
			}

			DrawEdges();			

			if (doDrawLines)
			{
				DrawTriangleLines();
			}

			DrawNodeCosts();
		}

		if (debugNavMeshExistence)
		{
			debugNavMeshExistence = false;

			GameObject player = PlayerManager.LocalPlayerGameObject();
			if (null != player)
			{
				if (null == AstarPath.active) // no AstarPath
				{
					DebugSystem.Log("AstarPath.active object does not exist", "Pathfinding");
				}
				else if (null == AstarPath.active.graphs) // no graphs array
				{
					DebugSystem.Log("AstarPath.active.graphs array does not exist", "Pathfinding");
				}
				else if (0 == AstarPath.active.graphs.Length) // empty graphs array
				{
					DebugSystem.Log("0 == AstarPath.active.graphs.Length", "Pathfinding");
				}			
				else if (0 == CalculateNumNavMeshTriangles()) // no triangles in the scene
				{
					DebugSystem.Log("There are no triangles in the scene", "Pathfinding");
				}
				else if (!AstarPath.active.astarData.cacheStartup || null == AstarPath.active.astarData.data_cachedStartup) // incorrectly set-up to scan on startup
				{
					DebugSystem.Log("The AStar object is incorrectly set-up to scan on start-up", "Pathfinding");
				}
				else // all good
				{
					DebugSystem.Log("A correctly set-up nav mesh exists", "Pathfinding");
				}
			}
		}
	}

	private void DrawNodeCosts()
	{
		if (doDrawNodeCosts)
		{
			PlayerController player = PlayerManager.LocalPlayerController();
			if (null != player)
			{
				NNInfo info = AstarPath.active.GetNearest(player.GetComponent<LocomotionComponentAPP>().Destination);
				TriangleMeshNode node = info.node as TriangleMeshNode;
				if (null != node)
				{
					for (int i = 0; i < 3; ++i)
					{
						int next = (i + 1) % 3;
						Vector3 one = (Vector3)node.GetVertex(i);
						Vector3 two = (Vector3)node.GetVertex(next);
						GLRenderingUtils.DoDrawLine(one, two, Color.white);
					}
					
					int highestConnectionCostIndex = 0;					
					for (int connect = 1; connect < node.connections.Length; ++connect)
					{
						if (node.connectionCosts[connect] > node.connectionCosts[highestConnectionCostIndex])
						{
							highestConnectionCostIndex = connect;
						}
					}

					Color[] colors = new Color[] { Color.red, Color.green, Color.blue };
					string[] colorNames = new string[] { "Color.red", "Color.green", "Color.blue" };
					const float HeightOffset = 2f;
					for (int connect = 0; connect < node.connections.Length; ++connect)
					{
						TriangleMeshNode nodeConnect = node.connections[connect] as TriangleMeshNode;
						GLRenderingUtils.DoDrawLine((Vector3)node.position, (Vector3)nodeConnect.position, colors[Mathf.Min(connect, colors.Length - 1)]);
						float dist = GameUtils.SubXZ((Vector3)node.position, (Vector3)nodeConnect.position).magnitude;
						Vector3 offset = new Vector3(0f, ((float)node.connectionCosts[connect] / (float)node.connectionCosts[highestConnectionCostIndex]) * HeightOffset, 0f);

						for (int i = 0; i < 3; ++i)
						{
							int next = (i + 1) % 3;
							Vector3 one = (Vector3)nodeConnect.GetVertex(i);
							Vector3 two = (Vector3)nodeConnect.GetVertex(next);

							Vector3 oneOffset = one + offset;
							Vector3 twoOffset = two + offset;

							Color col = colors[Mathf.Min(connect, colors.Length - 1)];
							GLRenderingUtils.DoDrawLine(oneOffset, twoOffset, col);
							GLRenderingUtils.DoDrawLine(one, oneOffset, col);
							GLRenderingUtils.DoDrawLine(two, twoOffset, col);
						}
						if (doPrintNodeCosts)
						{
							DebugSystem.Log(colorNames[Mathf.Min(connect, colorNames.Length - 1)] + ": Cost: " + node.connectionCosts[connect] + " Dist: " + dist, "Pathfinding");
						}
					}
				}
			}
		}
		doPrintNodeCosts = false;
	}

	private void DrawTriangles()
	{
		// draw all graph nodes
		GL.Begin(GL.TRIANGLES);            
			Vector3 offsetUp = new Vector3(0f, 0.1f, 0f); 
			for (int graphIndex = 0; graphIndex < AstarPath.active.graphs.Length; ++graphIndex)
			{
				NavGraph graph = AstarPath.active.graphs[graphIndex];				
				INavmesh ng = graph as INavmesh;
				if (null != ng)
				{
					ng.GetNodes(delegate(GraphNode _node)
					{
						// if we are only displaying one region, and this is not that region
						if (desplaySpecifiedRegion != DisplayAllRegions && desplaySpecifiedRegion != _node.Area)
						{
							return true;
						}

						Color theColor = graph.NodeColor(_node, AstarPath.active.debugPathData);
						theColor.a = navMeshAlpha;

						TriangleMeshNode node = _node as TriangleMeshNode;
						if (!node.Walkable)
						{
							theColor = Color.black;
						}

						GL.Color(theColor);

						Vector3 vert0 = (Vector3)node.GetVertex(0);
						Vector3 vert1 = (Vector3)node.GetVertex(1);
						Vector3 vert2 = (Vector3)node.GetVertex(2);

						GL.Vertex(vert0 + offsetUp);
						GL.Vertex(vert1 + offsetUp);
						GL.Vertex(vert2 + offsetUp);

						return true;
					});
				}
			}
		GL.End();
	}

	private void DrawTriangleLines()
	{
		GL.Begin(GL.LINES);
			for (int graphIndex = 0; graphIndex < AstarPath.active.graphs.Length; ++graphIndex)
			{
				NavGraph graph = AstarPath.active.graphs[graphIndex];
				INavmesh ng = graph as INavmesh;
				if (null != ng)
				{
					ng.GetNodes(delegate(GraphNode _node)
					{
						// if we are only displaying one region, and this is not that region
						if (desplaySpecifiedRegion != DisplayAllRegions && desplaySpecifiedRegion != _node.Area)
						{
							return true;
						}

						Color theColor = graph.NodeColor(_node, AstarPath.active.debugPathData);
						GL.Color(theColor);
						
						TriangleMeshNode node = _node as TriangleMeshNode;

						Vector3 vertZero = (Vector3)node.GetVertex(0);
						Vector3 vertOne = (Vector3)node.GetVertex(1);
						Vector3 vertTwo = (Vector3)node.GetVertex(2);

						GL.Vertex(vertZero);
						GL.Vertex(vertOne);

						GL.Vertex(vertOne);
						GL.Vertex(vertTwo);

						GL.Vertex(vertTwo);
						GL.Vertex(vertZero);	

						GL.Color(Color.blue);
						// draw a line to all of the connected nodes (only if a two way connection exists)
						for (int nodeConnect = 0; nodeConnect < node.connections.Length; nodeConnect++)
						{							
							TriangleMeshNode connectedNode = node.connections[nodeConnect] as TriangleMeshNode;
							if (null != connectedNode)
							{	// go over all the nodes that this connected node is connected to in order to check that one of them is the original node, thus a two way connection exists
								for (int connectedNodeConnect = 0; connectedNodeConnect < connectedNode.connections.Length; connectedNodeConnect++)
								{
									TriangleMeshNode originalNode = connectedNode.connections[connectedNodeConnect] as TriangleMeshNode;
									if (originalNode == node) // is this the original node, meaning a two way connection exists
									{
										GL.Vertex((Vector3)node.position);
										GL.Vertex((Vector3)connectedNode.position);
										break;
									}
								}								
							}							
						}

						return true;
					});					
				}
			}
		GL.End();
	}
 
	private void DrawEdges()
	{
		if (closeDistanceEdges > 0f || doDrawEdgeNormals)
		{
			PlayerController player = PlayerManager.LocalPlayerController();
			if (null != player)
			{
				if (closeDistanceEdges > 0f)
				{
					GLRenderingUtils.DoDrawSphere(player.transform.position, closeDistanceEdges, Color.red);
				}

				List<ObstacleVertex> obstacles = new List<ObstacleVertex>();
				GetSimulator().GetStaticAndDynamicObstacles(obstacles, player.transform.position, closeDistanceEdges * closeDistanceEdges);

				GL.Begin(GL.LINES);
				for (int i = 0; i < obstacles.Count; i++)
				{
					DrawObstacleAndNormal(obstacles[i], closeDistanceEdges > 0f, doDrawEdgeNormals, doDrawEdgeTop, Color.black);
				}
				GL.End();
			}
		}
	}

	//protected void Awake()
	//{
	//    DebugRenderingCameraComponent.AddCallback(OnDrawDebug);
	//}

	public void OnDebugGUI()
	{
	}

	public void OnDebugPanelGUI()
	{
	}

	// draw the list of obstacles
	private void DrawObstacleAndNormal(ObstacleVertex obstacle, bool drawObstacle, bool drawNormal, bool drawTop, Color col)
	{		
		Vector3 cposition = obstacle.position;
		Vector3 cnextposition = obstacle.next.position;

		if (drawObstacle)
		{
			GL.Color(col);
			GL.Vertex(cposition);
			GL.Vertex(cnextposition);

			if (drawTop)
			{
				Vector3 drawOffsetToTopOfObstacle = new Vector3(0f, obstacle.height, 0f);
				GL.Vertex(cposition + drawOffsetToTopOfObstacle);
				GL.Vertex(cnextposition + drawOffsetToTopOfObstacle);

				GL.Color(Color.white);	
				GL.Vertex(cposition);
				GL.Vertex(cposition + drawOffsetToTopOfObstacle);
				GL.Vertex(cnextposition);
				GL.Vertex(cnextposition + drawOffsetToTopOfObstacle);
			}
		}

		if (drawNormal)
		{
			GL.Color(Color.red);			
			Vector3 normalDrawStartPos = Vector3.Lerp(cposition, cnextposition, 0.5f);
			Vector3 normalDrawEndPos = normalDrawStartPos + obstacle.pushAwayDir;
			GL.Vertex(normalDrawStartPos);
			GL.Vertex(normalDrawEndPos);
		}
	}

	private Pathfinding.RVO.Simulator GetSimulator()
	{
		if (null == _sim)
		{
			_sim = AStarPathfindingUtils.GetSimulator();
		}
		return _sim;
	}
#endif
}

