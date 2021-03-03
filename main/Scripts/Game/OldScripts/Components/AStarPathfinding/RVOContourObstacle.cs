using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.RVO;

/**
 * Contour Obstacle for RVO Simulation.
 * 
 * \astarpro 
 */
[AddComponentMenu("Pathfinding/Local Avoidance/Contour Obstacle")]
public class RVOContourObstacle : MonoBehaviour
{
	private bool _isActive = false;
	// All obstacles added
	private List<ObstacleVertex> _staticObstacles = null;
	private List<ObstacleVertex> _dynamicObstacles = null; // will only be used if the RVOContourObstacle moves

	public void OnLatePostScan(Pathfinding.RVO.Simulator sim)
	{
		RemoveDynamicObstacles(sim);
		RemoveStaticObstaclesAndCreateNew(sim);
	}

	// turn obstacles on/off
	public void SetObstaclesActive(Pathfinding.RVO.Simulator sim, bool isActive)
	{
		_isActive = isActive;

		if (null != sim)
		{
			if (null != _staticObstacles)
			{
				for (int i = 0; i < _staticObstacles.Count; i++)
				{
					sim.SetObstacleActive(_staticObstacles[i], _isActive);
				}
			}

			if (null != _dynamicObstacles)
			{
				for (int i = 0; i < _dynamicObstacles.Count; i++)
				{
					sim.SetObstacleActive(_dynamicObstacles[i], _isActive);
				}
			}
		}
	}

	// the obstacle has move
	public void Move(Pathfinding.RVO.Simulator sim)
	{
		if (null != _staticObstacles)
		{
			SetObstaclesActive(sim, false);
			_staticObstacles = null;
		}

		RemoveDynamicObstaclesAndCreateNew(sim);
	}

	// we want dynamic obstacles, but only if we do not have any static obstacles
	public void CreateDynamicEdgesIfNoStaticEdgesExist(Pathfinding.RVO.Simulator sim)
	{
		if (null != _staticObstacles && _staticObstacles.Count > 0)
		{
			return;
		}
		_staticObstacles = null;
		RemoveDynamicObstaclesAndCreateNew(sim);
	}

	// destroy the dynamic obstacles we already have
	public void RemoveDynamicObstacles(Pathfinding.RVO.Simulator sim)
	{
		if (null != _dynamicObstacles)
		{
			if (null != sim)
			{
				for (int i = 0; i < _dynamicObstacles.Count; i++)
				{
					sim.RemoveDynamicObstacle(_dynamicObstacles[i]);
				}
			}
			_dynamicObstacles.Clear();
		}
	}

	// remove old abstacles and create new ones
	private void RemoveStaticObstaclesAndCreateNew(Pathfinding.RVO.Simulator sim)
	{
		RemoveStaticObstacles(sim);
		CreateObstacles(sim, ref _staticObstacles);

		if (null != _staticObstacles)
		{
			for (int i = 0; i < _staticObstacles.Count; i++)
			{
				sim.AddObstacle(_staticObstacles[i]);
			}
		}
	}

	// destroy the dynamic obstacles we already have and create new ones
	private void RemoveDynamicObstaclesAndCreateNew(Pathfinding.RVO.Simulator sim)
	{
		RemoveDynamicObstacles(sim);

		CreateObstacles(sim, ref _dynamicObstacles);
		if (null != _dynamicObstacles)
		{
			for (int i = 0; i < _dynamicObstacles.Count; i++)
			{
				sim.AddDynamicObstacle(_dynamicObstacles[i]);
			}
		}
	}

	// destroy the static obstacles we already have
	private void RemoveStaticObstacles(Pathfinding.RVO.Simulator sim)
	{
		if (null != _staticObstacles)
		{
			for (int i = 0; i < _staticObstacles.Count; i++)
			{
				sim.RemoveObstacle(_staticObstacles[i]);
			}
			_staticObstacles.Clear();
		}
	}

	// create the obstacles we need
	private void CreateObstacles(Pathfinding.RVO.Simulator sim, ref List<ObstacleVertex> obstacles)
	{
		NavmeshCut cut = gameObject.GetComponent<NavmeshCut>();
		if (null == cut)
		{
			return;
		}

		Vector3 leftFront = new Vector3();
		Vector3 rightFront = new Vector3();
		Vector3 rightBack = new Vector3();
		Vector3 leftBack = new Vector3();
		cut.CalculateRectangleContourPoints(ref leftFront, ref rightFront, ref rightBack, ref leftBack);

		Bounds bounds = cut.GetBounds();

		Vector3 cutTopCenter = Vector3.Lerp(leftFront, rightBack, 0.5f); // calculate the top center of the nav mesh cut
		cutTopCenter.y = bounds.max.y;

		float edgeHeight = bounds.min.y;

		// do a sphere cast downwards to see where the ground is beneath the nav mesh cut in order to put the obstacles on the ground 
		RaycastHit hitInfo;
		const float SphereCastRadius = 0.35f; // this value ensures the sphere cast is large enough to not go through seems
		if (Physics.SphereCast(cutTopCenter, SphereCastRadius, Vector3.down, out hitInfo, bounds.max.y - bounds.min.y, 1 << LayerMask.NameToLayer("Ground")))
		{
			edgeHeight = hitInfo.point.y;
		}

		leftFront.y = rightFront.y = rightBack.y = leftBack.y = edgeHeight; // set the obstacle height

		Vector3[] verts = new Vector3[] {rightFront, rightBack, leftBack, leftFront};
		AddObstacle(verts, Mathf.Max(0f, bounds.max.y - edgeHeight), sim, ref obstacles);
	}

	/** Adds an obstacle with the specified vertices.
	  * The vertices array might be changed by this function. */
	private void AddObstacle(Vector3[] vertices, float height, Pathfinding.RVO.Simulator sim, ref List<ObstacleVertex> obstacles)
	{
		if (vertices == null) throw new System.ArgumentNullException("Vertices Must Not Be Null");
		if (height < 0) throw new System.ArgumentOutOfRangeException("Height must be non-negative");
		if (vertices.Length < 2) throw new System.ArgumentException("An obstacle must have at least two vertices");

		if (null == obstacles)
		{
			obstacles = new List<ObstacleVertex>();
		}
		obstacles.Clear();

		if (vertices.Length > 2)
		{
			WindCorrectly(vertices);
		}

		for (int vert = 0; vert < vertices.Length; ++vert)
		{
			Vector3 v1 = vertices[vert];
			Vector3 v2 = vertices[(vert + 1) % vertices.Length];
			Vector3 dir = v2 - v1;
			Vector3 pushAwayDir = new Vector3(dir.z, 0f, -dir.x);
			obstacles.Add(sim.CreateObstacle(v1, v2, pushAwayDir, height, _isActive));
		}
	}


	/** Winds the vertices correctly.
	 * Winding order is determined from #obstacleMode.
	 */
	private void WindCorrectly(Vector3[] vertices)
	{
		int leftmost = 0;
		float leftmostX = float.PositiveInfinity;
		for (int i = 0; i < vertices.Length; i++)
		{
			if (vertices[i].x < leftmostX)
			{
				leftmost = i;
				leftmostX = vertices[i].x;
			}
		}

		if (VectorMath.IsClockwiseXZ(vertices[(leftmost - 1 + vertices.Length) % vertices.Length], vertices[leftmost], vertices[(leftmost + 1) % vertices.Length]))
		{
			System.Array.Reverse(vertices);
		}
	}
}
