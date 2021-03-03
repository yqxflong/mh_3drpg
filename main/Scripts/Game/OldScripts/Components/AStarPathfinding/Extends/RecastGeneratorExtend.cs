using UnityEngine;
using System.Collections.Generic;
using Pathfinding.Serialization.JsonFx;
using Pathfinding.Voxels;
using System;

#if APP_UPDATED

namespace Pathfinding
{
	public partial class RecastGraph
	{
		[JsonMember]
		public bool generateFromInputMesh = false;

		[JsonMember]
		public bool isSingleInputMesh = false;

		[JsonMember]
		public float heightZoneLinkTolerance = 3f;

		/// <summary>
		/// this parameter is used to stop nav mesh being built on obstacles
		/// specify a layer which nav mesh should not be built on (the layer should be one from those specified in Layer Mask)
		/// </summary>
		[JsonMember]
		public LayerMask excludeMask = 0;

		/// <summary>
		/// when generating a single zone nav mesh, we may choose to generate nine zones but only use the center zone, this can give better results
		/// than using one zone which is the size of the nav mesh bounds
		/// </summary>
		[HideInInspector]
		public bool useCenterTileOnly = false;

		protected void GenerateTileFromInputMesh(AStarPathfindingWalkableArea walkArea)
		{            
			if (null == walkArea)
			{
				EB.Debug.LogWarning("GenerateTileFromInputMesh: walkArea is empty");
				return;
			}

			Mesh inputMesh = walkArea.gameObject.GetComponent<MeshFilter>().sharedMesh;
			if (null == inputMesh)
			{
				EB.Debug.LogWarning("GenerateTileFromInputMesh: inputMesh is empty");
				return;
			}

			List<Vector3> allPoints = new List<Vector3>(inputMesh.vertices);
			Bounds bounds = GameUtils.CalculateBounds(allPoints);

			forcedBoundsCenter = walkArea.transform.TransformPoint(bounds.center);
			forcedBoundsSize = bounds.size;

			// this section changes the size of the bounds and tile, so that we always get one single tile
			SetUpNavMeshToFitOnOneTile();
			CalculateNumberOfTiles(ref tileXCount, ref tileZCount);
			Debug.Assert(tileXCount == 1 && tileZCount == 1, "Their should be only one tile when using a prebuilt nav mesh");

			tiles = new NavmeshTile[tileXCount * tileZCount]; // only one tile

			// ignore this setting
			scanEmptyGraph = false;

			// the Vector3 vertices in the mesh need to be converted to the APP Int3 format
			Int3[] Int3Verts = new Int3[inputMesh.vertices.Length];
			for (int i = 0; i < Int3Verts.Length; ++i)
			{
				Vector3 tempVert = inputMesh.vertices[i];
				tempVert = walkArea.transform.TransformPoint(tempVert); // get the world space position, rather than local space

				Int3Verts[i] = (Int3)tempVert;
			}
			tiles[0] = CreateTile(inputMesh.triangles, Int3Verts, 0, 0); // our single tile

			//Assign graph index to nodes
			uint graphIndex = (uint)AstarPath.active.astarData.GetGraphIndex(this);
			GraphNodeDelegateCancelable del = delegate(GraphNode n)
			{
				n.GraphIndex = graphIndex;
				return true;
			};
			GetNodes(del);
		}

		// generate a nav mesh with one tile per zone
		protected void GenerateTilesForZones(AStarPathfindingWalkableArea[] walkAreas)
		{
			if (null == walkAreas || 0 == walkAreas.Length)
			{
				EB.Debug.LogWarning("GenerateTilesForZones: walkAreas is empty");
				return;
			}

			LevelHelper levelHelper = GameObject.FindObjectOfType(typeof(LevelHelper)) as LevelHelper;
			if (null == levelHelper)
			{
				EB.Debug.LogWarning("GenerateTilesForZones: LevelHelper not found");
				return;
			}

			Vector3 NavMeshBoundingBoxMin;
			Vector3 NavMeshBoundingBoxMax;
			levelHelper.CalculateAllZonesMinMax(out NavMeshBoundingBoxMin, out NavMeshBoundingBoxMax);

			// set the entire bounds and center of the recast graph
			forcedBoundsSize = NavMeshBoundingBoxMax - NavMeshBoundingBoxMin;
			forcedBoundsCenter = NavMeshBoundingBoxMin + (forcedBoundsSize * 0.5f);

			tileSizeX = tileSizeZ = (int)(EditorVars.GridSize / cellSize); // this line sets the tile size so that each tile will hold the size of a zone
			CalculateNumberOfTiles(ref tileXCount, ref tileZCount);

#if DEBUG
			int correctTilesXCount = (Mathf.CeilToInt(forcedBoundsSize.x / EditorVars.GridSize)); // this would fail if GridSize is less than 1f
			int correctTilesZCount = (Mathf.CeilToInt(forcedBoundsSize.z / EditorVars.GridSize)); // this would fail if GridSize is less than 1f
			if (correctTilesXCount != tileXCount || correctTilesZCount != tileZCount)
			{
				EB.Debug.LogError("Incorrect number of tiles generated");
			}
#endif
			tiles = new NavmeshTile[tileXCount * tileZCount];

			// ignore this setting
			scanEmptyGraph = false;

			Vector3 tempZoneMin = new Vector3();
			Vector3 tempZoneMax = new Vector3();

			// go over all the walkable areas and put there mesh into a tile
			foreach (AStarPathfindingWalkableArea walk in walkAreas)
			{
				ZoneDescriptor zoneDescriptor = (ZoneDescriptor)GameUtils.FindFirstComponentUpwards<ZoneDescriptor>(walk.transform);
				if (zoneDescriptor != null)
				{
					ZoneDescriptor.CalculateZoneMinAndMax(ref tempZoneMin, ref tempZoneMax, zoneDescriptor.gameObject.transform);

					// 1f is added to avoid floating point inacuracy (avoids 63.999/64 = 0, 64.99/64=1 which is correct)
					int z = (int)(((tempZoneMin.z + 1f) - NavMeshBoundingBoxMin.z) / EditorVars.GridSize);
					int x = (int)(((tempZoneMin.x + 1f) - NavMeshBoundingBoxMin.x) / EditorVars.GridSize);

					MeshFilter meshFilter = walk.gameObject.GetComponent<MeshFilter>();
					if (null != meshFilter.sharedMesh)
					{
						// the Vector3 vertices in the mesh need to be converted to the APP Int3 format
						Int3[] Int3Verts = new Int3[meshFilter.sharedMesh.vertices.Length];
						for (int i = 0; i < Int3Verts.Length; ++i)
						{
							Vector3 tempVert = new Vector3(meshFilter.sharedMesh.vertices[i].x, meshFilter.sharedMesh.vertices[i].y, meshFilter.sharedMesh.vertices[i].z);
							tempVert = walk.transform.TransformPoint(tempVert); // get the world space position, rather than local space

							// clamp the verts to the edges of the zone boundaries if they are close, this is so that the different tiles are linked together accurately
							const float Tol = 0.01f;
							tempVert.x = (tempVert.x <= tempZoneMin.x + Tol) ? tempZoneMin.x : tempVert.x;
							tempVert.x = (tempVert.x >= tempZoneMax.x - Tol) ? tempZoneMax.x : tempVert.x;

							tempVert.z = (tempVert.z <= tempZoneMin.z + Tol) ? tempZoneMin.z : tempVert.z;
							tempVert.z = (tempVert.z >= tempZoneMax.z - Tol) ? tempZoneMax.z : tempVert.z;

							Int3Verts[i] = (Int3)tempVert;

						}
						NavmeshTile tile = CreateTile(meshFilter.sharedMesh.triangles, Int3Verts, x, z);
						tiles[Convert2DArrayCoordTo1DArrayCoord(x, z, tileXCount)] = tile;
					}
				}
			}

			//Assign graph index to nodes
			uint graphIndex = (uint)AstarPath.active.astarData.GetGraphIndex(this);
			GraphNodeDelegateCancelable del = delegate(GraphNode n)
			{
				n.GraphIndex = graphIndex;
				return true;
			};
			GetNodes(del);

			// connect each tile to one and other
			for (int z = 0; z < tileZCount; z++)
			{
				for (int x = 0; x < tileXCount; x++)
				{
					// make sure all the tiles which might be considered, have tiles created
					CreateAndAddEmptyTileIfNonExists(x, z);
					CreateAndAddEmptyTileIfNonExists(x + 1, z);
					CreateAndAddEmptyTileIfNonExists(x, z + 1);

					if (x < tileXCount - 1) ConnectTiles(tiles[Convert2DArrayCoordTo1DArrayCoord(x, z, tileXCount)], tiles[Convert2DArrayCoordTo1DArrayCoord(x + 1, z, tileXCount)]);
					if (z < tileZCount - 1) ConnectTiles(tiles[Convert2DArrayCoordTo1DArrayCoord(x, z, tileXCount)], tiles[Convert2DArrayCoordTo1DArrayCoord(x, z + 1, tileXCount)]);
				}
			}
		}

		protected NavmeshTile CreateTile(int[] tris, Int3[] verts, int x, int z)
		{
#if BNICKSON_UPDATED
			if (tris == null) throw new System.ArgumentNullException("The mesh must be valid. tris is null.");
			if (verts == null) throw new System.ArgumentNullException("The mesh must be valid. verts is null.");

			//Create a new navmesh tile and assign its settings
			var tile = new NavmeshTile();

			tile.x = x;
			tile.z = z;
			tile.w = 1;
			tile.d = 1;
			tile.tris = tris;
			tile.verts = verts;
			tile.bbTree = new BBTree();
#endif

			if (tile.tris.Length % 3 != 0) throw new System.ArgumentException("Indices array's length must be a multiple of 3 (mesh.tris)");

			if (tile.verts.Length >= VertexIndexMask)
				throw new System.ArgumentException("Too many vertices per tile (more than " + VertexIndexMask + ")." +
"\nTry enabling ASTAR_RECAST_LARGER_TILES under the 'Optimizations' tab in the A* Inspector");

			//Dictionary<Int3, int> firstVerts = new Dictionary<Int3, int> ();
			Dictionary<Int3, int> firstVerts = cachedInt3_int_dict;
			firstVerts.Clear();

			var compressedPointers = new int[tile.verts.Length];

			int count = 0;
			for (int i = 0; i < tile.verts.Length; i++)
			{
				try
				{
					firstVerts.Add(tile.verts[i], count);
					compressedPointers[i] = count;
					tile.verts[count] = tile.verts[i];
					count++;
				}
				catch
				{
					//There are some cases, rare but still there, that vertices are identical
					compressedPointers[i] = firstVerts[tile.verts[i]];
				}
			}

			for (int i = 0; i < tile.tris.Length; i++)
			{
				tile.tris[i] = compressedPointers[tile.tris[i]];
			}

			var compressed = new Int3[count];
			for (int i = 0; i < count; i++) compressed[i] = tile.verts[i];

			tile.verts = compressed;

			var nodes = new TriangleMeshNode[tile.tris.Length/3];
			tile.nodes = nodes;

			//Here we are faking a new graph
			//The tile is not added to any graphs yet, but to get the position querys from the nodes
			//to work correctly (not throw exceptions because the tile is not calculated) we fake a new graph
			//and direct the position queries directly to the tile
			int graphIndex = AstarPath.active.astarData.graphs.Length;

			TriangleMeshNode.SetNavmeshHolder(graphIndex, tile);

			//This index will be ORed to the triangle indices
			int tileIndex = x + z*tileXCount;
			tileIndex <<= TileIndexOffset;

			//Create nodes and assign triangle indices
			for (int i = 0; i < nodes.Length; i++)
			{
				var node = new TriangleMeshNode(active);
				nodes[i] = node;
				node.GraphIndex = (uint)graphIndex;
				node.v0 = tile.tris[i * 3 + 0] | tileIndex;
				node.v1 = tile.tris[i * 3 + 1] | tileIndex;
				node.v2 = tile.tris[i * 3 + 2] | tileIndex;

				//Degenerate triangles might occur, but they will not cause any large troubles anymore
				//if (Polygon.IsColinear (node.GetVertex(0), node.GetVertex(1), node.GetVertex(2))) {
				//	Debug.Log ("COLINEAR!!!!!!");
				//}

				//Make sure the triangle is clockwise
				if (!VectorMath.IsClockwiseXZ(node.GetVertex(0), node.GetVertex(1), node.GetVertex(2)))
				{
					int tmp = node.v0;
					node.v0 = node.v2;
					node.v2 = tmp;
				}

				node.Walkable = true;
				node.Penalty = initialPenalty;
				node.UpdatePositionFromVertices();
				tile.bbTree.Insert(node);
			}

			CreateNodeConnections(tile.nodes);

			//Remove the fake graph
			TriangleMeshNode.SetNavmeshHolder(graphIndex, null);

			return tile;
		}

		private bool IsGameObjectInLayerMask(GameObject theGameObject, LayerMask theLayerMask)
		{
			return ((1 << theGameObject.layer) == ((1 << theGameObject.layer) & theLayerMask));
		}

		public void SetUpNavMeshToFitOnOneTile()
		{
			forcedBoundsSize.x = forcedBoundsSize.z = Mathf.Max(forcedBoundsSize.x, forcedBoundsSize.z);
			const int Padding = 10;
			tileSizeX = tileSizeZ = (int)(forcedBoundsSize.x / cellSize + Padding); // extra padding here ensures entire mesh will fit on a single tile
		}
		
		protected int Convert2DArrayCoordTo1DArrayCoord(int x, int z, int xMax)
		{
			return x + z * xMax;
		}

		// fill in placeholder tiles
		protected void CreateAndAddEmptyTileIfNonExists(int x, int z)
		{
			if (null != tiles && x < tileXCount && z < tileZCount && null == tiles[Convert2DArrayCoordTo1DArrayCoord(x, z, tileXCount)])
			{
				tiles[Convert2DArrayCoordTo1DArrayCoord(x, z, tileXCount)] = NewEmptyTile(x, z);
			}
		}

		protected void CalculateNumberOfTiles(ref int outX, ref int outZ)
		{
			//Voxel grid size
			int gw = (int)(forcedBounds.size.x / cellSize + 0.5f);
			int gd = (int)(forcedBounds.size.z / cellSize + 0.5f);

			// tileSizeX && tileSizeZ need set before
			useTiles = true;
			editorTileSize = (tileSizeX == tileSizeZ ? tileSizeX : editorTileSize);

			//Number of tiles
			outX = (gw + tileSizeX - 1) / tileSizeX;
			outZ = (gd + tileSizeZ - 1) / tileSizeZ;
		}

		//// find all the meshes marked with a AStarPathfindingWalkableArea component and combine into one mesh
		//protected static Mesh GenerateCombinedNavMesh(AStarPathfindingWalkableArea[] walkAreas)
		//{
		//    if (0 == walkAreas.Length)
		//    {
		//        return null;
		//    }
		//    else if (1 == walkAreas.Length)
		//    {
		//        return walkAreas[0].gameObject.GetComponent<MeshFilter>().sharedMesh;
		//    }

		//    CombineInstance[] combine = new CombineInstance[walkAreas.Length];
		//    for (int area = 0; area < walkAreas.Length; ++area)
		//    {
		//        MeshFilter meshFilter = walkAreas[area].gameObject.GetComponent<MeshFilter>();
		//        combine[area].mesh = meshFilter.sharedMesh;
		//        combine[area].transform = meshFilter.transform.localToWorldMatrix;
		//    }

		//    Mesh mesh = new Mesh();
		//    mesh.CombineMeshes(combine, true, true);
		//    GameUtils.WeldMeshVertices(mesh);
		//    return mesh;
		//}

		public static void DestroyWalkableAreaObjects()
		{
			AStarPathfindingWalkableArea[] walkAreas = GameObject.FindObjectsOfType(typeof(AStarPathfindingWalkableArea)) as AStarPathfindingWalkableArea[];
			foreach (AStarPathfindingWalkableArea walk in walkAreas)
			{
				UnityEngine.Object.DestroyImmediate(walk.gameObject);
			}
		}

		public static void DestroyRecastMeshObjComponents()
		{
			RecastMeshObj[] recastObjs = GameObject.FindObjectsOfType(typeof(RecastMeshObj)) as RecastMeshObj[];
			foreach (RecastMeshObj obj in recastObjs)
			{
				UnityEngine.Object.DestroyImmediate(obj);
			}
		}

		#region override functions

		protected void ScanTiledNavmesh(OnScanStatus statusCallback)
		{
#if BNICKSON_UPDATED
			if (generateFromInputMesh)
			{
				AStarPathfindingWalkableArea[] walkAreas = GameObject.FindObjectsOfType(typeof(AStarPathfindingWalkableArea)) as AStarPathfindingWalkableArea[];
				if (isSingleInputMesh)
				{
					if (1 == walkAreas.Length)
					{
						GenerateTileFromInputMesh(walkAreas[0]);
					}
				}
				else
				{
					GenerateTilesForZones(walkAreas);
				}
			}
			else
			{
				ScanAllTiles(statusCallback);
			}
#else
			ScanAllTiles(statusCallback);
#endif
		}

		protected void ScanAllTiles(OnScanStatus statusCallback)
		{
#if ASTARDEBUG
			System.Console.WriteLine ("Recast Graph -- Collecting Meshes");
#endif

#if BNICKSON_UPDATED
			editorTileSize = (int)(EditorVars.GridSize / cellSize);
#endif

			//----

			//Voxel grid size
			int gw = (int)(forcedBounds.size.x / cellSize + 0.5f);
			int gd = (int)(forcedBounds.size.z / cellSize + 0.5f);

			if (!useTiles)
			{
				tileSizeX = gw;
				tileSizeZ = gd;
			}
			else {
				tileSizeX = editorTileSize;
				tileSizeZ = editorTileSize;
			}

			//Number of tiles
			int tw = (gw + tileSizeX - 1) / tileSizeX;
			int td = (gd + tileSizeZ - 1) / tileSizeZ;

			tileXCount = tw;
			tileZCount = td;

			if (tileXCount * tileZCount > TileIndexMask + 1)
			{
				throw new System.Exception("Too many tiles (" + (tileXCount * tileZCount) + ") maximum is " + (TileIndexMask + 1) +
					"\nTry disabling ASTAR_RECAST_LARGER_TILES under the 'Optimizations' tab in the A* inspector.");
			}

			tiles = new NavmeshTile[tileXCount * tileZCount];

#if ASTARDEBUG
			System.Console.WriteLine("Recast Graph -- Creating Voxel Base");
#endif

			// If this is true, just fill the graph with empty tiles
			if (scanEmptyGraph)
			{
				for (int z = 0; z < td; z++)
				{
					for (int x = 0; x < tw; x++)
					{
						tiles[z * tileXCount + x] = NewEmptyTile(x, z);
					}
				}
				return;
			}

			AstarProfiler.StartProfile("Finding Meshes");
			List<ExtraMesh> extraMeshes;

#if !NETFX_CORE || UNITY_EDITOR
			System.Console.WriteLine("Collecting Meshes");
#endif
			CollectMeshes(out extraMeshes, forcedBounds);

			AstarProfiler.EndProfile("Finding Meshes");

			// A walkableClimb higher than walkableHeight can cause issues when generating the navmesh since then it can in some cases
			// Both be valid for a character to walk under an obstacle and climb up on top of it (and that cannot be handled with navmesh without links)
			// The editor scripts also enforce this but we enforce it here too just to be sure
			walkableClimb = Mathf.Min(walkableClimb, walkableHeight);

			//Create the voxelizer and set all settings
			var vox = new Voxelize(cellHeight, cellSize, walkableClimb, walkableHeight, maxSlope);
			vox.inputExtraMeshes = extraMeshes;

			vox.maxEdgeLength = maxEdgeLength;

			int lastInfoCallback = -1;
			var watch = System.Diagnostics.Stopwatch.StartNew();

			//Generate all tiles
			for (int z = 0; z < td; z++)
			{
				for (int x = 0; x < tw; x++)
				{
					int tileNum = z * tileXCount + x;
#if !NETFX_CORE || UNITY_EDITOR
					System.Console.WriteLine("Generating Tile #" + (tileNum) + " of " + td * tw);
#endif

					//Call statusCallback only 10 times since it is very slow in the editor
					if (statusCallback != null && (tileNum * 10 / tiles.Length > lastInfoCallback || watch.ElapsedMilliseconds > 2000))
					{
						lastInfoCallback = tileNum * 10 / tiles.Length;
						watch.Reset();
						watch.Start();

						statusCallback(new Progress(Mathf.Lerp(0.1f, 0.9f, tileNum / (float)tiles.Length), "Building Tile " + tileNum + "/" + tiles.Length));
					}

					BuildTileMesh(vox, x, z);
				}
			}

#if !NETFX_CORE
			System.Console.WriteLine("Assigning Graph Indices");
#endif

			if (statusCallback != null) statusCallback(new Progress(0.9f, "Connecting tiles"));

			//Assign graph index to nodes
			uint graphIndex = (uint)AstarPath.active.astarData.GetGraphIndex(this);

			GraphNodeDelegateCancelable del = delegate (GraphNode n) {
				n.GraphIndex = graphIndex;
				return true;
			};
			GetNodes(del);

#if BNICKSON_UPDATED
#if DEBUG
			if (useCenterTileOnly && (3 != tileXCount || 3 != tileZCount))
			{
				EB.Debug.LogError("RecastGenerator.ScanAllTiles() : Incorrect amount of tiles generated if ceneter tile is all that is required");
			}
#endif

			int centerXTile = (tileXCount / 2);
			int centerZTile = (tileZCount / 2);
#endif

			for (int z = 0; z < td; z++)
			{
				for (int x = 0; x < tw; x++)
				{
#if BNICKSON_UPDATED
					// if we're only using the center tile, and this is not the center tile
					if (useCenterTileOnly && !(centerZTile == z && centerXTile == x))
					{
						continue;
					}
#endif

#if !NETFX_CORE
					System.Console.WriteLine("Connecing Tile #" + (z * tileXCount + x) + " of " + td * tw);
#endif
					if (x < tw - 1) ConnectTiles(tiles[x + z * tileXCount], tiles[x + 1 + z * tileXCount]);
					if (z < td - 1) ConnectTiles(tiles[x + z * tileXCount], tiles[x + (z + 1) * tileXCount]);
				}
			}

			AstarProfiler.PrintResults();
		}

		/** Find all relevant RecastMeshObj components and create ExtraMeshes for them */
		public void GetRecastMeshObjs(Bounds bounds, List<ExtraMesh> buffer)
		{
			List<RecastMeshObj> buffer2 = Util.ListPool<RecastMeshObj>.Claim();

			// Get all recast mesh objects inside the bounds
			RecastMeshObj.GetAllInBounds(buffer2, bounds);

			var cachedVertices = new Dictionary<Mesh, Vector3[]>();
			var cachedTris = new Dictionary<Mesh, int[]>();

			// Create an ExtraMesh object
			// for each RecastMeshObj
			for (int i = 0; i < buffer2.Count; i++)
			{
				MeshFilter filter = buffer2[i].GetMeshFilter();
				Renderer rend = filter != null ? filter.GetComponent<Renderer>() : null;

				if (filter != null && rend != null)
				{
					Mesh mesh = filter.sharedMesh;

#if BNICKSON_UPDATED
					// extra information about which game object is causing the problem
					if (null == mesh)
					{
						EB.Debug.LogError("RecastGenerator.GetRecastMeshObjs(): {0} has a RecastMeshObj, but does not have a MeshFilter", filter.name);
					}
#endif

					var smesh = new ExtraMesh();
					smesh.matrix = rend.localToWorldMatrix;
					smesh.original = filter;
					smesh.area = buffer2[i].area;

					// Don't read the vertices and triangles from the
					// mesh if we have seen the same mesh previously
					if (cachedVertices.ContainsKey(mesh))
					{
						smesh.vertices = cachedVertices[mesh];
						smesh.triangles = cachedTris[mesh];
					}
					else {
						smesh.vertices = mesh.vertices;
						smesh.triangles = mesh.triangles;
						cachedVertices[mesh] = smesh.vertices;
						cachedTris[mesh] = smesh.triangles;
					}

					smesh.bounds = rend.bounds;

					buffer.Add(smesh);
				}
				else {
					Collider coll = buffer2[i].GetCollider();

					if (coll == null)
					{
						EB.Debug.LogError("RecastMeshObject ({0}) didn't have a collider or MeshFilter+Renderer attached" , buffer2[i].gameObject.name);
						continue;
					}

					ExtraMesh smesh = RasterizeCollider(coll);
					smesh.area = buffer2[i].area;

					//Make sure a valid ExtraMesh was returned
					if (smesh.vertices != null) buffer.Add(smesh);
				}
			}

			//Clear cache to avoid memory leak
			capsuleCache.Clear();

			Util.ListPool<RecastMeshObj>.Release(buffer2);
		}

		/** Generate connections between the two tiles.
		 * The tiles must be adjacent.
		 */
		protected void ConnectTiles(NavmeshTile tile1, NavmeshTile tile2)
		{
			if (tile1 == null) return;//throw new System.ArgumentNullException ("tile1");
			if (tile2 == null) return;//throw new System.ArgumentNullException ("tile2");

			if (tile1.nodes == null) throw new System.ArgumentException("tile1 does not contain any nodes");
			if (tile2.nodes == null) throw new System.ArgumentException("tile2 does not contain any nodes");

			int t1x = Mathf.Clamp(tile2.x, tile1.x, tile1.x + tile1.w - 1);
			int t2x = Mathf.Clamp(tile1.x, tile2.x, tile2.x + tile2.w - 1);
			int t1z = Mathf.Clamp(tile2.z, tile1.z, tile1.z + tile1.d - 1);
			int t2z = Mathf.Clamp(tile1.z, tile2.z, tile2.z + tile2.d - 1);

			int coord, altcoord;
			int t1coord, t2coord;

			float tcs;

			if (t1x == t2x)
			{
				coord = 2;
				altcoord = 0;
				t1coord = t1z;
				t2coord = t2z;
				tcs = tileSizeZ * cellSize;
			}
			else if (t1z == t2z)
			{
				coord = 0;
				altcoord = 2;
				t1coord = t1x;
				t2coord = t2x;
				tcs = tileSizeX * cellSize;
			}
			else {
				throw new System.ArgumentException("Tiles are not adjacent (neither x or z coordinates match)");
			}

			if (Math.Abs(t1coord - t2coord) != 1)
			{
				EB.Debug.Log("{0} {1} {2} {3}\n{5} {6} {7} {8}\n{9} {10} {11} {12}",tile1.x, tile1.z, tile1.w, tile1.d,
					tile2.x,tile2.z, tile2.w,tile2.d,t1x,t1z, t2x, t2z);
				throw new System.ArgumentException("Tiles are not adjacent (tile coordinates must differ by exactly 1. Got '" + t1coord + "' and '" + t2coord + "')");
			}

			//Midpoint between the two tiles
			int midpoint = (int)Math.Round((Math.Max(t1coord, t2coord) * tcs + forcedBounds.min[coord]) * Int3.Precision);

#if ASTARDEBUG
			Vector3 v1 = new Vector3(-100, 0, -100);
			Vector3 v2 = new Vector3(100, 0, 100);
			v1[coord] = midpoint*Int3.PrecisionFactor;
			v2[coord] = midpoint*Int3.PrecisionFactor;

			Debug.DrawLine(v1, v2, Color.magenta);
#endif

#if BNICKSON_UPDATED
			// different triangle link height tolerance based on whether we're linking tiles generated for random levels or not
			float heightToleranceSquared = generateFromInputMesh ? GameUtils.Square(heightZoneLinkTolerance) : GameUtils.Square(walkableClimb);
#endif
			TriangleMeshNode[] nodes1 = tile1.nodes;
			TriangleMeshNode[] nodes2 = tile2.nodes;

			//Find adjacent nodes on the border between the tiles
			for (int i = 0; i < nodes1.Length; i++)
			{
				TriangleMeshNode node = nodes1[i];
				int av = node.GetVertexCount();

				for (int a = 0; a < av; a++)
				{
					Int3 ap1 = node.GetVertex(a);
					Int3 ap2 = node.GetVertex((a + 1) % av);
#if BNICKSON_UPDATED
					if (ap1[coord] == midpoint && ap2[coord] == midpoint) // this could be given a little bit of tolerance
#else
					if (Math.Abs(ap1[coord] - midpoint) < 2 && Math.Abs(ap2[coord] - midpoint) < 2)
#endif
					{
#if ASTARDEBUG
						Debug.DrawLine((Vector3)ap1, (Vector3)ap2, Color.red);
#endif

						int minalt = Math.Min(ap1[altcoord], ap2[altcoord]);
						int maxalt = Math.Max(ap1[altcoord], ap2[altcoord]);

						//Degenerate edge
						if (minalt == maxalt) continue;

						for (int j = 0; j < nodes2.Length; j++)
						{
							TriangleMeshNode other = nodes2[j];
							int bv = other.GetVertexCount();
							for (int b = 0; b < bv; b++)
							{
								Int3 bp1 = other.GetVertex(b);
								Int3 bp2 = other.GetVertex((b + 1) % av);
#if BNICKSON_UPDATED
								if (bp1[coord] == midpoint && bp2[coord] == midpoint) // this could be given a little bit of tolerance
#else
								if (Math.Abs(bp1[coord] - midpoint) < 2 && Math.Abs(bp2[coord] - midpoint) < 2)
#endif
								{

									int minalt2 = Math.Min(bp1[altcoord], bp2[altcoord]);
									int maxalt2 = Math.Max(bp1[altcoord], bp2[altcoord]);

									//Degenerate edge
									if (minalt2 == maxalt2) continue;

									if (maxalt > minalt2 && minalt < maxalt2)
									{
										//Adjacent

										//Test shortest distance between the segments (first test if they are equal since that is much faster)
										if ((ap1 == bp1 && ap2 == bp2) || (ap1 == bp2 && ap2 == bp1) ||
#if BNICKSON_UPDATED
											VectorMath.SqrDistanceSegmentSegment((Vector3)ap1, (Vector3)ap2, (Vector3)bp1, (Vector3)bp2) < heightToleranceSquared) // different height tolerances based on generating from input mesh or not
#else
											VectorMath.SqrDistanceSegmentSegment((Vector3)ap1, (Vector3)ap2, (Vector3)bp1, (Vector3)bp2) < walkableClimb*walkableClimb)
#endif
										{

											uint cost = (uint)(node.position - other.position).costMagnitude;

											node.AddConnection(other, cost);
											other.AddConnection(node, cost);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		void GetSceneMeshes(Bounds bounds, List<string> tagMask, LayerMask layerMask, List<ExtraMesh> meshes)
		{
			if ((tagMask != null && tagMask.Count > 0) || layerMask != 0)
			{
				var filters = GameObject.FindObjectsOfType(typeof(MeshFilter)) as MeshFilter[];

				var filteredFilters = new List<MeshFilter>(filters.Length / 3);

				for (int i = 0; i < filters.Length; i++)
				{
					MeshFilter filter = filters[i];
					Renderer rend = filter.GetComponent<Renderer>();

					if (rend != null && filter.sharedMesh != null && rend.enabled && (((1 << filter.gameObject.layer) & layerMask) != 0 || tagMask.Contains(filter.tag)))
					{
						if (filter.GetComponent<RecastMeshObj>() == null)
						{
							filteredFilters.Add(filter);
						}
					}
				}

				var cachedVertices = new Dictionary<Mesh, Vector3[]>();
				var cachedTris = new Dictionary<Mesh, int[]>();

				bool containedStatic = false;

				for (int i = 0; i < filteredFilters.Count; i++)
				{
					MeshFilter filter = filteredFilters[i];

					// Note, guaranteed to have a renderer
					Renderer rend = filter.GetComponent<Renderer>();

					//Workaround for statically batched meshes
					if (rend.isPartOfStaticBatch)
					{
						containedStatic = true;
					}
					else {
						//Only include it if it intersects with the graph
						if (rend.bounds.Intersects(bounds))
						{
							Mesh mesh = filter.sharedMesh;
							var smesh = new ExtraMesh();
							smesh.matrix = rend.localToWorldMatrix;
							smesh.original = filter;

#if BNICKSON_UPDATED
							if (IsGameObjectInLayerMask(filter.gameObject, excludeMask))
							{// UnwalkableArea = 0 = NoNavMeshOnObject + 1
								const int NoNavMeshOnObject = -1;
								smesh.area = NoNavMeshOnObject;
							}
#endif

							if (cachedVertices.ContainsKey(mesh))
							{
								smesh.vertices = cachedVertices[mesh];
								smesh.triangles = cachedTris[mesh];
							}
							else {
								smesh.vertices = mesh.vertices;
								smesh.triangles = mesh.triangles;
								cachedVertices[mesh] = smesh.vertices;
								cachedTris[mesh] = smesh.triangles;
							}

							smesh.bounds = rend.bounds;

							meshes.Add(smesh);
						}
					}

					if (containedStatic)
						EB.Debug.LogWarning("Some meshes were statically batched. These meshes can not be used for navmesh calculation" +
							" due to technical constraints.\nDuring runtime scripts cannot access the data of meshes which have been statically batched.\n" +
							"One way to solve this problem is to use cached startup (Save & Load tab in the inspector) to only calculate the graph when the game is not playing.");
				}

#if ASTARDEBUG
				int y = 0;
				foreach (ExtraMesh smesh in meshes) {
					y++;
					Vector3[] vecs = smesh.vertices;
					int[] tris = smesh.triangles;

					for (int i = 0; i < tris.Length; i += 3) {
						Vector3 p1 = smesh.matrix.MultiplyPoint3x4(vecs[tris[i+0]]);
						Vector3 p2 = smesh.matrix.MultiplyPoint3x4(vecs[tris[i+1]]);
						Vector3 p3 = smesh.matrix.MultiplyPoint3x4(vecs[tris[i+2]]);

						Debug.DrawLine(p1, p2, Color.red, 1);
						Debug.DrawLine(p2, p3, Color.red, 1);
						Debug.DrawLine(p3, p1, Color.red, 1);
					}
				}
#endif
			}
		}

		#endregion
	}
}

#endif