///////////////////////////////////////////////////////////////////////
//
//  MeshUtils.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public static class MeshUtils
{
	public const int IndexInvalid = -1;
	public const int NumVertsPerTriangle = 3;

	public class EditedMesh
	{
		public List<Vector3> verts = new List<Vector3>();
		public List<int> triangleIndecis = new List<int>();

		public EditedMesh(Mesh mesh)
		{
			AddVerts(mesh.vertices);

			int[] triangles = mesh.triangles;
			int numMeshTriangles = triangles.Length;

			for (int index = 0; index < numMeshTriangles; ++index)
			{
				triangleIndecis.Add(triangles[index]);
			}
		}

		public EditedMesh()
		{
		}

		public void AddVerts(List<Vector3> newVerts)
		{
			for (int vert = 0; vert < newVerts.Count; ++vert)
			{
				verts.Add(newVerts[vert]);
			}
		}

		public void AddVerts(Vector3[] newVerts)
		{
			int newVertsLength = newVerts.Length;
			for (int vert = 0; vert < newVertsLength; ++vert)
			{
				verts.Add(newVerts[vert]);
			}
		}

		public void AddTriangles(List<int> newTriangleIndecis)
		{
			for (int index = 0; index < newTriangleIndecis.Count; ++index)
			{
				triangleIndecis.Add(newTriangleIndecis[index]);
			}
		}

		public void RemapTriangleIndecis(Mesh mesh, Dictionary<int, int> vertRemapping)
		{
			for (int triangleIndex = 0; triangleIndex < mesh.triangles.Length; ++triangleIndex)
			{
				triangleIndecis.Add(vertRemapping[mesh.triangles[triangleIndex]]);
			}
		}

		public void CopyTo(Mesh mesh)
		{
			List<Vector3> newNormals = new List<Vector3>();
			List<Vector2> newUVs = new List<Vector2>();

			for (int vert = 0; vert < verts.Count; ++vert)
			{
				newNormals.Add(Vector3.up);
				newUVs.Add(Vector2.zero);
			}

			if (verts.Count < mesh.vertices.Length)
			{ // if we are losing verts
				mesh.triangles = triangleIndecis.ToArray(); // update indecis first, so old indecis will not be referencing out of bounds
				mesh.vertices = verts.ToArray();
				mesh.normals = newNormals.ToArray();
				mesh.uv = newUVs.ToArray();
			}
			else
			{ // gaining verts
				mesh.vertices = verts.ToArray();
				mesh.normals = newNormals.ToArray();
				mesh.uv = newUVs.ToArray();
				mesh.triangles = triangleIndecis.ToArray(); // set this after we have our new verts availble, to avoid referencing out of bounds
			}
			mesh.RecalculateBounds();
		}
	}

	public static void AddUnitTriangle(Mesh mesh)
	{
		int currentNumVerts = mesh.vertices.Length;
		EditedMesh meshEdit = new EditedMesh(mesh);
		meshEdit.AddVerts(new List<Vector3>() { Vector3.zero, new Vector3(-1f, 0f, 0f), new Vector3(0f, 0f, 1f) });

		meshEdit.AddTriangles(new List<int>() { currentNumVerts, currentNumVerts + 1, currentNumVerts + 2 });
		meshEdit.CopyTo(mesh);
	}

	// will also remove any triangles which are using the verts
	public static void RemoveVerts(List<int> vertsToRemove, Mesh mesh)
	{
		EditedMesh meshEdit = new EditedMesh();

		Dictionary<int, int> vertRemapping = new Dictionary<int, int>();
		for (int vert = 0; vert < mesh.vertices.Length; ++vert)
		{
			if (!vertsToRemove.Contains(vert))
			{
				vertRemapping.Add(vert, meshEdit.verts.Count);
				meshEdit.verts.Add(mesh.vertices[vert]);
			}
		}

		for (int triangleIndex = 0; triangleIndex < mesh.triangles.Length; triangleIndex += NumVertsPerTriangle)
		{
			if (!vertsToRemove.Contains(mesh.triangles[triangleIndex]) &&
				!vertsToRemove.Contains(mesh.triangles[triangleIndex + 1]) &&
				!vertsToRemove.Contains(mesh.triangles[triangleIndex + 2])) // if none of this triangles verts are being removed, keep triangle
			{
				meshEdit.triangleIndecis.Add(vertRemapping[mesh.triangles[triangleIndex]]);
				meshEdit.triangleIndecis.Add(vertRemapping[mesh.triangles[triangleIndex + 1]]);
				meshEdit.triangleIndecis.Add(vertRemapping[mesh.triangles[triangleIndex + 2]]);
			}
		}
		meshEdit.CopyTo(mesh);
	}

	public static void MoveVerts(List<int> vertsToMove, Vector3 delta, Mesh mesh)
	{
		EditedMesh meshEdit = new EditedMesh(mesh);

		int vertsToMoveCount = vertsToMove.Count;
		for (int index = 0; index < vertsToMoveCount; ++index)
		{
			meshEdit.verts[vertsToMove[index]] += delta;
		}
		meshEdit.CopyTo(mesh);
	}

	// snap the vert to a length along the axis
	public static void SnapVertsOnAxis(List<int> vertsToMove, Vector3 snapAxis, float snapLength, Mesh mesh)
	{
		EditedMesh meshEdit = new EditedMesh(mesh);
		Vector3 snapAxisNormalized = snapAxis.normalized;
		for (int index = 0; index < vertsToMove.Count; ++index)
		{
			float fixUpLength = snapLength - Vector3.Dot(meshEdit.verts[vertsToMove[index]], snapAxisNormalized);
			meshEdit.verts[vertsToMove[index]] = meshEdit.verts[vertsToMove[index]] + snapAxisNormalized * fixUpLength;
		}
		meshEdit.CopyTo(mesh);
	}

	// all triangles will be updated 
	public static int FuseVerts(List<int> vertsToFuse, Mesh mesh)
	{
		if (vertsToFuse.Count > 1)
		{
			EditedMesh meshEdit = new EditedMesh();
			List<int> vertsToRemove = new List<int>();
			int vertToKeep = mesh.vertices.Length;
			for (int vert = 0; vert < vertsToFuse.Count; ++vert)
			{
				vertsToRemove.Add(vertsToFuse[vert]);
				if (vertsToFuse[vert] < vertToKeep)
				{
					vertToKeep = vertsToFuse[vert];
				}
			}
			vertsToRemove.Remove(vertToKeep);

			Dictionary<int, int> vertRemapping = new Dictionary<int, int>();
			for (int vert = 0; vert < mesh.vertices.Length; ++vert)
			{
				if (vertsToRemove.Contains(vert))
				{
					vertRemapping.Add(vert, vertToKeep);
				}
				else
				{
					vertRemapping.Add(vert, meshEdit.verts.Count);
					meshEdit.verts.Add(mesh.vertices[vert]);
				}
			}

			meshEdit.RemapTriangleIndecis(mesh, vertRemapping);
			meshEdit.CopyTo(mesh);
			return vertToKeep;
		}
		return IndexInvalid;
	}

	public static void FuseCloseVerts(List<int> vertsToFuse, Mesh mesh, float distSqr)
	{
		EditedMesh meshEdit = new EditedMesh();
		Dictionary<int, int> vertRemapping = new Dictionary<int, int>();

		for (int vert = 0; vert < mesh.vertices.Length; ++vert)
		{
			if (vertRemapping.ContainsKey(vert)) // already been remapped
			{
				continue;
			}
			vertRemapping.Add(vert, meshEdit.verts.Count);
			meshEdit.verts.Add(mesh.vertices[vert]);

			if (!vertsToFuse.Contains(vert))
			{
				continue; // not interested in fusing this vert
			}

			Vector3 vertPos = mesh.vertices[vert];
			for (int vertToCompare = vert + 1; vertToCompare < mesh.vertices.Length; ++vertToCompare)
			{
				if (!vertsToFuse.Contains(vertToCompare))
				{
					continue; // not interested in fusing this vert
				}

				if ((vertPos - mesh.vertices[vertToCompare]).sqrMagnitude < distSqr)
				{
					vertRemapping.Add(vertToCompare, vertRemapping[vert]);
				}
			}
		}
		meshEdit.RemapTriangleIndecis(mesh, vertRemapping);
		meshEdit.CopyTo(mesh);
	}

	public static void RemoveUnusedVerts(Mesh mesh)
	{
		EditedMesh meshEdit = new EditedMesh();
		Dictionary<int, int> vertRemapping = new Dictionary<int, int>();

		for (int triangleIndex = 0; triangleIndex < mesh.triangles.Length; ++triangleIndex)
		{
			int vert = mesh.triangles[triangleIndex];
			if (!vertRemapping.ContainsKey(vert))
			{
				vertRemapping.Add(vert, meshEdit.verts.Count);
				meshEdit.verts.Add(mesh.vertices[vert]);
			}
			meshEdit.triangleIndecis.Add(vertRemapping[vert]);
		}
		meshEdit.CopyTo(mesh);
	}

	public static void DuplicateVerts(List<int> vertsToDuplicate, Mesh mesh)
	{
		EditedMesh meshEdit = new EditedMesh(mesh);
		for (int index = 0; index < vertsToDuplicate.Count; ++index)
		{
			meshEdit.verts.Add(mesh.vertices[vertsToDuplicate[index]]);
		}
		meshEdit.CopyTo(mesh);
	}

	// triangleVertIndecis must be a multiple of three, each triangle must have three vert indecis
	public static void AddTriangles(List<int> triangleVertIndecis, Mesh mesh)
	{
		EditedMesh meshEdit = new EditedMesh(mesh);
		meshEdit.AddTriangles(triangleVertIndecis);
		meshEdit.CopyTo(mesh);
	}

	public static void DuplicateTriangles(List<int> trianglesToDuplicate, Mesh mesh)
	{
		EditedMesh meshEdit = new EditedMesh(mesh);
		// go over all the trianles, making copies of all the verts which are used
		Dictionary<int, int> vertRemapping = new Dictionary<int, int>();
		for (int index = 0; index < trianglesToDuplicate.Count; ++index)
		{
			int startingTriangleIndex = trianglesToDuplicate[index] * NumVertsPerTriangle;

			for (int triangleVert = 0; triangleVert < NumVertsPerTriangle; ++triangleVert)
			{
				int vertIndex = mesh.triangles[startingTriangleIndex + triangleVert];
				if (!vertRemapping.ContainsKey(vertIndex))
				{
					vertRemapping.Add(vertIndex, meshEdit.verts.Count);
					meshEdit.verts.Add(mesh.vertices[vertIndex]);
				}
				meshEdit.triangleIndecis.Add(vertRemapping[vertIndex]);
			}
		}
		meshEdit.CopyTo(mesh);
	}

	// reverse the winding order of the triangle
	public static void ChangeTrianglesWinding(List<int> trianglesToChange, Mesh mesh)
	{
		EditedMesh meshEdit = new EditedMesh(mesh);
		for (int triangle = 0; triangle < trianglesToChange.Count; ++triangle)
		{
			int firstTriangleIndex = trianglesToChange[triangle] * NumVertsPerTriangle;

			int temp = meshEdit.triangleIndecis[firstTriangleIndex];
			meshEdit.triangleIndecis[firstTriangleIndex] = meshEdit.triangleIndecis[firstTriangleIndex + 2];
			meshEdit.triangleIndecis[firstTriangleIndex + 2] = temp;
		}
		meshEdit.CopyTo(mesh);
	}

	// triangle zero, will mean the first triangle (three triangle indecis) are deleted, triangle index 1, the second triangle deleted
	public static void RemoveTriangles(List<int> trianglesToRemove, Mesh mesh)
	{
		EditedMesh meshEdit = new EditedMesh();
		meshEdit.AddVerts(mesh.vertices);
		for (int triangle = 0; triangle < CalculateNumTriangles(mesh); ++triangle)
		{
			if (!trianglesToRemove.Contains(triangle))
			{
				int startingTriangleIndex = triangle * NumVertsPerTriangle;
				meshEdit.triangleIndecis.Add(mesh.triangles[startingTriangleIndex]);
				meshEdit.triangleIndecis.Add(mesh.triangles[startingTriangleIndex + 1]);
				meshEdit.triangleIndecis.Add(mesh.triangles[startingTriangleIndex + 2]);
			}
		}
		meshEdit.CopyTo(mesh);
	}

	public static void MoveTriangles(List<int> trianglesToMove, Vector3 delta, Mesh mesh)
	{
		EditedMesh meshEdit = new EditedMesh(mesh);
		HashSet<int> editedVerts = new HashSet<int>();

		int trianglesToMoveCount = trianglesToMove.Count;
		for (int index = 0; index < trianglesToMoveCount; ++index)
		{
			int triangle = trianglesToMove[index];
			int startingTriangleIndex = triangle * NumVertsPerTriangle;

			for (int triangleIndex = 0; triangleIndex < NumVertsPerTriangle; ++triangleIndex)
			{
				int vertIndex = meshEdit.triangleIndecis[startingTriangleIndex + triangleIndex];
				if (!editedVerts.Contains(vertIndex))
				{
					editedVerts.Add(vertIndex);
					meshEdit.verts[vertIndex] += delta;
				}
			}
		}
		meshEdit.CopyTo(mesh);
	}

	public static Vector3 CalculateTriangleCenter(int triangle, Mesh mesh)
	{
		return CalculateTriangleCenter(mesh.vertices[mesh.triangles[triangle * 3]],
										mesh.vertices[mesh.triangles[triangle * 3 + 1]],
										mesh.vertices[mesh.triangles[triangle * 3 + 2]]);
	}

	public static Vector3 CalculateTriangleCenter(Vector3 vert0, Vector3 vert1, Vector3 vert2)
	{
		return (vert0 + vert1 + vert2) / 3f;
	}

	public static int CalculateNumTriangles(Mesh mesh)
	{
		return mesh.triangles.Length / MeshUtils.NumVertsPerTriangle;
	}

	public static int CalculateNumUnusedVerts(Mesh mesh)
	{
		HashSet<int> usedVerts = new HashSet<int>();
		int numTriangleIndecis = mesh.triangles.Length;
		int[] triangles = mesh.triangles;
		for (int triangleIndex = 0; triangleIndex < numTriangleIndecis; ++triangleIndex)
		{
			usedVerts.Add(triangles[triangleIndex]);
		}
		return mesh.vertices.Length - usedVerts.Count;
	}

	public static int CalculateSelectedVert(Ray ray, Mesh mesh)
	{
		int closestVert = IndexInvalid;
		float closestVertDistSqr = 0f;

		Vector3 rayDirectionNormalized = ray.direction.normalized;

		Vector3[] vertices = mesh.vertices;
		int numVerts = vertices.Length;

		Vector3 rayOriginToVert = Vector3.zero;
		Vector3 closestPointOnRay = Vector3.zero;
		float distSqr = 0f;

		for (int vert = 0; vert < numVerts; ++vert)
		{
			rayOriginToVert = vertices[vert] - ray.origin;
			closestPointOnRay = rayDirectionNormalized * Vector3.Dot(rayDirectionNormalized, rayOriginToVert);
			distSqr = (rayOriginToVert - closestPointOnRay).sqrMagnitude;

			if (IndexInvalid == closestVert || distSqr < closestVertDistSqr)
			{
				closestVert = vert;
				closestVertDistSqr = distSqr;
			}
		}
		return closestVert;
	}

	public static int CalculateSelectedTriangle(Ray ray, Mesh mesh)
	{
		int closestTriangle = IndexInvalid;
		float closestTriangleDist = 0f;

		float intersectionDist = 0f;
		Vector3 vert0 = Vector3.zero;
		Vector3 vert1 = Vector3.zero;
		Vector3 vert2 = Vector3.zero;

		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;

		Vector3[] threeVerts = new Vector3[NumVertsPerTriangle] { vert0, vert1, vert2 };

		int numMeshTriangles = triangles.Length;
		for (int triangleIndex = 0; triangleIndex < numMeshTriangles; triangleIndex += NumVertsPerTriangle)
		{
			threeVerts[0] = vertices[triangles[triangleIndex]];
			threeVerts[1] = vertices[triangles[triangleIndex + 1]];
			threeVerts[2] = vertices[triangles[triangleIndex + 2]];

			if (RayTriangleIntersect(ray, threeVerts, out intersectionDist) &&
				(IndexInvalid == closestTriangle || intersectionDist < closestTriangleDist))
			{
				closestTriangle = triangleIndex / 3;
				closestTriangleDist = intersectionDist;
			}
		}
		return closestTriangle;
	}

	static public void ForEachTriangle(Mesh mesh, System.Action<Vector3, Vector3, Vector3> Func)
	{
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;

		int numMeshTriangles = mesh.triangles.Length;
		for (int triangleIndex = 0; triangleIndex < numMeshTriangles; triangleIndex += NumVertsPerTriangle)
		{
			Func(vertices[triangles[triangleIndex]],
				 vertices[triangles[triangleIndex + 1]],
				 vertices[triangles[triangleIndex + 2]]);
		}
	}

	public static bool GetTriangle(int triangleIndex, Mesh mesh, ref Vector3 vert0, ref Vector3 vert1, ref Vector3 vert2)
	{
		if ((triangleIndex + 1) * NumVertsPerTriangle <= mesh.triangles.Length)
		{
			vert0 = mesh.vertices[mesh.triangles[triangleIndex * NumVertsPerTriangle]];
			vert1 = mesh.vertices[mesh.triangles[triangleIndex * NumVertsPerTriangle + 1]];
			vert2 = mesh.vertices[mesh.triangles[triangleIndex * NumVertsPerTriangle + 2]];
			return true;
		}
		return false;
	}

	private static bool RayTriangleIntersect(Ray ray, Vector3[] verts, out float intersectionDist)
	{
		Plane trianglePlane = new Plane();
		trianglePlane.Set3Points(verts[0], verts[1], verts[2]);

		if (trianglePlane.Raycast(ray, out intersectionDist)) // If the ray makes contact with the plane
		{
			return IsPointInsideTriangle(ray.GetPoint(intersectionDist), verts[0], verts[1], verts[2]);
		}
		return false;
	}

	// determines whether a point on a plane is inside a triangle
	private static bool IsPointInsideTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
	{
		return SameSide(p, a, b, c) && SameSide(p, b, a, c) && SameSide(p, c, a, b);
	}

	private static bool SameSide(Vector3 p1, Vector3 p2, Vector3 a, Vector3 b)
	{
		return (Vector3.Dot(Vector3.Cross(b - a, p1 - a), Vector3.Cross(b - a, p2 - a)) >= 0f);
	}
}
