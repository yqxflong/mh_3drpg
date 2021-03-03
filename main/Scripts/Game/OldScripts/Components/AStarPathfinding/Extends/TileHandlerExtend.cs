using System;
using UnityEngine;

#if APP_UPDATED

namespace Pathfinding.Util
{
	#region override functions

	public partial class TileHandler
	{
		/** Load a tile at tile coordinate \a x, \a z.
		 *
		 * \param tile Tile type to load
		 * \param x Tile x coordinate (first tile is at (0,0), second at (1,0) etc.. ).
		 * \param z Tile z coordinate.
		 * \param rotation Rotate tile by 90 degrees * value.
		 * \param yoffset Offset Y coordinates by this amount. In Int3 space, so if you have a world space
		 * offset, multiply by Int3.Precision and round to the nearest integer before calling this function.
		 */
		public void LoadTile(TileType tile, int x, int z, int rotation, int yoffset)
		{
			if (tile == null) throw new ArgumentNullException("tile");

			if (AstarPath.active == null) return;

			int index = x + z * graph.tileXCount;
			rotation = rotation % 4;

			// If loaded during this batch with the same settings, skip it
			if (isBatching && reloadedInBatch[index] && activeTileOffsets[index] == yoffset && activeTileRotations[index] == rotation && activeTileTypes[index] == tile)
			{
				return;
			}

			reloadedInBatch[index] |= isBatching;

			activeTileOffsets[index] = yoffset;
			activeTileRotations[index] = rotation;
			activeTileTypes[index] = tile;

			//Add a work item
			//This will pause pathfinding as soon as possible
			//and call the delegate when it is safe to update graphs
			AstarPath.active.AddWorkItem(new AstarPath.AstarWorkItem(delegate (bool force) {
				// If this was not the correct settings to load with, ignore
				if (!(activeTileOffsets[index] == yoffset && activeTileRotations[index] == rotation && activeTileTypes[index] == tile)) return true;

				GraphModifier.TriggerEvent(GraphModifier.EventType.PreUpdate);

				Int3[] verts;
				int[] tris;

				tile.Load(out verts, out tris, rotation, yoffset);

				//Calculate tile bounds so that the correct cutting offset can be used
				//The tile will be cut in local space (i.e it is at the world origin) so cuts need to be translated
				//to that point from their world space coordinates
				Bounds r = graph.GetTileBounds(x, z, tile.Width, tile.Depth);
				var cutOffset = (Int3)r.min;
				cutOffset = -cutOffset;

				Int3[] outVerts = null;
				int[] outTris = null;
				int vCount, tCount;

				//Cut the polygon
				CutPoly(verts, tris, ref outVerts, ref outTris, out vCount, out tCount, null, cutOffset, r);

				//Refine to remove bad triangles
				DelaunayRefinement(outVerts, outTris, ref vCount, ref tCount, true, false, -cutOffset);

				if (tCount != outTris.Length) outTris = ShrinkArray(outTris, tCount);
				if (vCount != outVerts.Length) outVerts = ShrinkArray(outVerts, vCount);

				// Rotate the mask correctly
				// and update width and depth to match rotation
				// (width and depth will swap if rotated 90 or 270 degrees )
				int newWidth = rotation % 2 == 0 ? tile.Width : tile.Depth;
				int newDepth = rotation % 2 == 0 ? tile.Depth : tile.Width;

				//Replace the tile using the final vertices and triangles
				//The vertices are still in local space
				graph.ReplaceTile(x, z, newWidth, newDepth, outVerts, outTris, false);

				//Trigger post update event
				//This can trigger for example recalculation of navmesh links
				GraphModifier.TriggerEvent(GraphModifier.EventType.PostUpdate);

#if BNICKSON_UPDATED
				AStarPathfindingAbilityBridge.NavMeshHasBeenUpdated();
#endif

				//Flood fill everything to make sure graph areas are still valid
				//This tends to take more than 50% of the calculation time
				AstarPath.active.QueueWorkItemFloodFill();

				return true;
			}));
		}

		#endregion
	}
}

#endif