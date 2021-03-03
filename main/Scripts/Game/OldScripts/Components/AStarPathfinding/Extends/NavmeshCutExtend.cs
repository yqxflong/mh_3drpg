using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

#if APP_UPDATED

namespace Pathfinding
{
	public partial class NavmeshCut
	{
		public bool lockRotation = true; // if true, rotation of the gameobject will be ignored and not effect the nav mesh cut orientation
		public bool isActive = false; // do we want to cut the nav mesh using this object right now

#if BNICKSON_UPDATED
		// for simplicity of adding new nav mesh edges, made meshtype a rectangle and private (so should not be changed)
		private MeshType type = MeshType.Rectangle; // DON'T CHANGE - TALK TO BNICKSON IF YOU THINK YOU NEED A DIFFERENT TYPE
		/** Custom mesh to use.
		 * The contour(s) of the mesh will be extracted.
		 * If you get the "max perturbations" error when cutting with this, check the normals on the mesh.
		 * They should all point in the same direction. Try flipping them if that does not help.
		 */
		private Mesh mesh = null; // BNICKSON, changed to private, as only Rectangle Meshtype is an option

		// made private to remove from inspector as only rectangle MeshType is supported
		/** Radius of the circle */
		private float circleRadius = 1;

		// made private to remove from inspector as only rectangle MeshType is supported
		/** Number of vertices on the circle */
		private int circleResolution = 6;

		// changed to private, as not used
		private float meshScale = 1;
#endif

		private bool _wasActive = false;
		private bool _UpdateIsRequired = false;

		public void OnPopulateZones(PopulateZonesPlayingEvent evt)
		{
			if (isActive != _wasActive) // if our active state has changed, it means we are awaiting for the fact this has hapenned to be picked up in the TileHandlerHelper Update
			{
				lastPosition = tr.position; // set this here so TileHandlerHelper picks up the activity change, and not the movement, so NotifyUpdated() will not flag
											// this cuts obstacles as dynamic
			}
			RVOContourObstacle cutObstacle = GetRVOContourObstacle();
			cutObstacle.OnLatePostScan(AStarPathfindingUtils.GetSimulator());
		}

		// make sure everything is correct on level start
		public void OnLevelStart(LevelStartEvent e)
		{
			RVOContourObstacle cutObstacle = GetRVOContourObstacle();
			cutObstacle.SetObstaclesActive(AStarPathfindingUtils.GetSimulator(), isActive);
		}

		public void OnDisable()
		{
			Pathfinding.RVO.Simulator sim = AStarPathfindingUtils.GetSimulator();
			RVOContourObstacle cutObstacle = GetRVOContourObstacle();
			if (cutObstacle)
			{
				cutObstacle.RemoveDynamicObstacles(sim);
			}

			if (_wasActive)
			{
				if (cutObstacle)
				{
					cutObstacle.SetObstaclesActive(sim, false);
				}

				if (null != TileHandlerHelper.instance)
				{
					TileHandlerHelper.instance.OnNavMeshCutDisabled(lastBounds);
				}
			}
			_UpdateIsRequired = false;
			EventManager.instance.RemoveListener<PopulateZonesPlayingEvent>(OnPopulateZones);
			EventManager.instance.RemoveListener<LevelStartEvent>(OnLevelStart);
		}

		public void PreUpdate()
		{
			if (!isActive && _UpdateIsRequired)
			{
				_UpdateIsRequired = false; // we don't need to update anything if isActive is false
			}
		}

		// calculate the points on the ground plane for a rectangle
		public void CalculateRectangleContourPoints(ref Vector3 leftBottom, ref Vector3 rightBottom, ref Vector3 rightTop, ref Vector3 leftTop)
		{
			Vector3 forward;
			if (lockRotation)
			{
				forward = new Vector3(0f, 0f, 1f);
			}
			else
			{
				forward = tr.transform.forward;
				if (!GameUtils.NormalizeXZ(ref forward)) // if the normalize fails (because forward has a y component of +/-1)
				{
					forward = new Vector3(0f, 0f, 1f);
				}
			}
			Vector3 right = new Vector3(forward.z, 0f, -forward.x);

			const float HalfWidth = 0.5f;
			leftBottom = (-rectangleSize.x * right) + (-rectangleSize.y * forward);
			rightBottom = (rectangleSize.x * right) + (-rectangleSize.y * forward);
			rightTop = (rectangleSize.x * right) + (rectangleSize.y * forward);
			leftTop = (-rectangleSize.x * right) + (rectangleSize.y * forward);

			Vector3 woffset = tr.position + center;
			leftBottom = (woffset + leftBottom * HalfWidth);
			rightBottom = (woffset + rightBottom * HalfWidth);
			rightTop = (woffset + rightTop * HalfWidth);
			leftTop = (woffset + leftTop * HalfWidth);
		}

		private void CalculateRectangleContourPoints(ref List<Vector3> outPoints)
		{
			Vector3 leftBottom = new Vector3();
			Vector3 rightBottom = new Vector3();
			Vector3 rightTop =  new Vector3();
			Vector3 leftTop =  new Vector3();
			CalculateRectangleContourPoints(ref leftBottom, ref rightBottom, ref rightTop, ref leftTop);

			outPoints.Add(leftBottom);
			outPoints.Add(rightBottom);
			outPoints.Add(rightTop);
			outPoints.Add(leftTop);
		}

		public RVOContourObstacle GetRVOContourObstacle()
		{
			RVOContourObstacle cutObstacle = gameObject.GetComponent<RVOContourObstacle>();
			if (!cutObstacle)
			{
				cutObstacle = gameObject.AddComponent<RVOContourObstacle>();
			}
			return cutObstacle;
		}

		// has the nav mesh cut moved
		private bool HasMoved()
		{
			return (tr.position - lastPosition).sqrMagnitude > updateDistance * updateDistance;
		}

		// used for building list of contour points for oriented rectangles 
		/** Cached variable, do avoid allocations */
		static private int NumRectangleContourPoints = 4;
		static List<Vector3> rectangleContourPoints = new List<Vector3>(NumRectangleContourPoints);

		#region override functions

		/** Forces this navmesh cut to update the navmesh.
		 *
		 * \note Dynamic updating requires a Tile Handler Helper somewhere in the scene.
		 * This update is not instant, it is done the next time the TileHandlerHelper checks this instance for
		 * if it needs updating.
		 *
		 * \see TileHandlerHelper.ForceUpdate()
		 */
		public void ForceUpdate()
		{
#if BNICKSON_UPDATED
			if (isActive)
			{
				_UpdateIsRequired = true;
			}
#else
			lastPosition = new Vector3(float.PositiveInfinity,float.PositiveInfinity,float.PositiveInfinity);
#endif
		}

		public void OnEnable()
		{
			tr = transform;
			lastPosition = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
			lastRotation = tr.rotation;

#if BNICKSON_UPDATED
			EventManager.instance.AddListener<PopulateZonesPlayingEvent>(OnPopulateZones);
			EventManager.instance.AddListener<LevelStartEvent>(OnLevelStart);
#endif
		}

		/** Returns true if this object has moved so much that it requires an update.
		 * When an update to the navmesh has been done, call NotifyUpdated to be able to get
		 * relavant output from this method again.
		 */
		public bool RequiresUpdate()
		{
#if BNICKSON_UPDATED
			// added check for isActive, check for _UpdateIsRequired, and moved check for movment into a function
			return HasMoved() || isActive != _wasActive || _UpdateIsRequired;
#else
			return wasEnabled != enabled || (wasEnabled && ((tr.position-lastPosition).sqrMagnitude > updateDistance*updateDistance || (useRotation && (Quaternion.Angle(lastRotation, tr.rotation) > updateRotationDistance))));
#endif
		}

		public void NotifyUpdated()
		{
#if BNICKSON_UPDATED
			Pathfinding.RVO.Simulator sim = AStarPathfindingUtils.GetSimulator();
			RVOContourObstacle cutObstacle = GetRVOContourObstacle();
			if (cutObstacle)
			{
				cutObstacle.SetObstaclesActive(sim, isActive);
				if (HasMoved())
				{
					cutObstacle.Move(sim);
				}
				else if (_UpdateIsRequired && isActive)
				{
					cutObstacle.CreateDynamicEdgesIfNoStaticEdgesExist(sim);
				}
			}

			_wasActive = isActive;
			_UpdateIsRequired = false;
#endif
			wasEnabled = enabled;

			if (wasEnabled)
			{
				lastPosition = tr.position;
				lastBounds = GetBounds();

				if (useRotation)
				{
					lastRotation = tr.rotation;
				}
			}
		}

		public Bounds GetBounds()
		{
			var bounds = new Bounds();
			switch (type)
			{
				case MeshType.Rectangle:
#if BNICKSON_UPDATED
					rectangleContourPoints.Clear();
					CalculateRectangleContourPoints(ref rectangleContourPoints);
					bounds = GameUtils.CalculateBounds(rectangleContourPoints); // calculate the bounding box to encapsulate all points we need to consider
					bounds.size = new Vector3(bounds.size.x, height, bounds.size.z);
#else
					if (useRotation) {
						Matrix4x4 m = tr.localToWorldMatrix;
						// Calculate the bounds by encapsulating each of the 8 corners in a bounds object
						bounds = new Bounds(m.MultiplyPoint3x4(center + new Vector3(-rectangleSize.x, -height, -rectangleSize.y)*0.5f), Vector3.zero);
						bounds.Encapsulate(m.MultiplyPoint3x4(center + new Vector3(rectangleSize.x, -height, -rectangleSize.y)*0.5f));
						bounds.Encapsulate(m.MultiplyPoint3x4(center + new Vector3(rectangleSize.x, -height, rectangleSize.y)*0.5f));
						bounds.Encapsulate(m.MultiplyPoint3x4(center + new Vector3(-rectangleSize.x, -height, rectangleSize.y)*0.5f));

						bounds.Encapsulate(m.MultiplyPoint3x4(center + new Vector3(-rectangleSize.x, height, -rectangleSize.y)*0.5f));
						bounds.Encapsulate(m.MultiplyPoint3x4(center + new Vector3(rectangleSize.x, height, -rectangleSize.y)*0.5f));
						bounds.Encapsulate(m.MultiplyPoint3x4(center + new Vector3(rectangleSize.x, height, rectangleSize.y)*0.5f));
						bounds.Encapsulate(m.MultiplyPoint3x4(center + new Vector3(-rectangleSize.x, height, rectangleSize.y)*0.5f));
					} else {
						bounds = new Bounds(tr.position+center, new Vector3(rectangleSize.x, height, rectangleSize.y));
					}
#endif
					break;
				case MeshType.Circle:
					if (useRotation)
					{
						Matrix4x4 m = tr.localToWorldMatrix;
						bounds = new Bounds(m.MultiplyPoint3x4(center), new Vector3(circleRadius * 2, height, circleRadius * 2));
					}
					else
					{
						bounds = new Bounds(transform.position + center, new Vector3(circleRadius * 2, height, circleRadius * 2));
					}
					break;
				case MeshType.CustomMesh:
					if (mesh == null) break;

					Bounds b = mesh.bounds;
					if (useRotation)
					{
						Matrix4x4 m = tr.localToWorldMatrix;
						b.center *= meshScale;
						b.size *= meshScale;

						bounds = new Bounds(m.MultiplyPoint3x4(center + b.center), Vector3.zero);

						Vector3 mx = b.max;
						Vector3 mn = b.min;

						bounds.Encapsulate(m.MultiplyPoint3x4(center + new Vector3(mx.x, mx.y, mx.z)));
						bounds.Encapsulate(m.MultiplyPoint3x4(center + new Vector3(mn.x, mx.y, mx.z)));
						bounds.Encapsulate(m.MultiplyPoint3x4(center + new Vector3(mn.x, mx.y, mn.z)));
						bounds.Encapsulate(m.MultiplyPoint3x4(center + new Vector3(mx.x, mx.y, mn.z)));

						bounds.Encapsulate(m.MultiplyPoint3x4(center + new Vector3(mx.x, mn.y, mx.z)));
						bounds.Encapsulate(m.MultiplyPoint3x4(center + new Vector3(mn.x, mn.y, mx.z)));
						bounds.Encapsulate(m.MultiplyPoint3x4(center + new Vector3(mn.x, mn.y, mn.z)));
						bounds.Encapsulate(m.MultiplyPoint3x4(center + new Vector3(mx.x, mn.y, mn.z)));

						Vector3 size = bounds.size;
						size.y = Mathf.Max(size.y, height * tr.lossyScale.y);
						bounds.size = size;
					}
					else
					{
						Vector3 size = b.size*meshScale;
						size.y = Mathf.Max(size.y, height);
						bounds = new Bounds(transform.position + center + b.center * meshScale, size);
					}
					break;
				default:
					throw new System.Exception("Invalid mesh type");
			}
			return bounds;
		}

		public void GetContour(List<List<Pathfinding.ClipperLib.IntPoint>> buffer)
		{

			if (circleResolution < 3) circleResolution = 3;

			Vector3 woffset = tr.position;
			switch (type)
			{
				case MeshType.Rectangle:
#if BNICKSON_UPDATED
					List<ClipperLib.IntPoint> buffer0 = Pathfinding.Util.ListPool<ClipperLib.IntPoint>.Claim();

					// oriented rectangles
					rectangleContourPoints.Clear();
					CalculateRectangleContourPoints(ref rectangleContourPoints);

					for (int contourPoint = 0; contourPoint < rectangleContourPoints.Count; ++contourPoint)
					{
						buffer0.Add(V3ToIntPoint(rectangleContourPoints[contourPoint]));
					}

					buffer.Add(buffer0);
#else
					List<Pathfinding.ClipperLib.IntPoint> buffer0 = Pathfinding.Util.ListPool<Pathfinding.ClipperLib.IntPoint>.Claim();
					if (useRotation) {
						Matrix4x4 m = tr.localToWorldMatrix;
						buffer0.Add(V3ToIntPoint(m.MultiplyPoint3x4(center + new Vector3(-rectangleSize.x, 0, -rectangleSize.y)*0.5f)));
						buffer0.Add(V3ToIntPoint(m.MultiplyPoint3x4(center + new Vector3(rectangleSize.x, 0, -rectangleSize.y)*0.5f)));
						buffer0.Add(V3ToIntPoint(m.MultiplyPoint3x4(center + new Vector3(rectangleSize.x, 0, rectangleSize.y)*0.5f)));
						buffer0.Add(V3ToIntPoint(m.MultiplyPoint3x4(center + new Vector3(-rectangleSize.x, 0, rectangleSize.y)*0.5f)));
					} else {
						woffset += center;
						buffer0.Add(V3ToIntPoint(woffset + new Vector3(-rectangleSize.x, 0, -rectangleSize.y)*0.5f));
						buffer0.Add(V3ToIntPoint(woffset + new Vector3(rectangleSize.x, 0, -rectangleSize.y)*0.5f));
						buffer0.Add(V3ToIntPoint(woffset + new Vector3(rectangleSize.x, 0, rectangleSize.y)*0.5f));
						buffer0.Add(V3ToIntPoint(woffset + new Vector3(-rectangleSize.x, 0, rectangleSize.y)*0.5f));
					}
					buffer.Add(buffer0);
#endif
					break;
				case MeshType.Circle:
					buffer0 = Pathfinding.Util.ListPool<Pathfinding.ClipperLib.IntPoint>.Claim(circleResolution);
					if (useRotation)
					{
						Matrix4x4 m = tr.localToWorldMatrix;
						for (int i = 0; i < circleResolution; i++)
						{
							buffer0.Add(V3ToIntPoint(m.MultiplyPoint3x4(center + new Vector3(Mathf.Cos((i * 2 * Mathf.PI) / circleResolution), 0, Mathf.Sin((i * 2 * Mathf.PI) / circleResolution)) * circleRadius)));
						}
					}
					else
					{
						woffset += center;
						for (int i = 0; i < circleResolution; i++)
						{
							buffer0.Add(V3ToIntPoint(woffset + new Vector3(Mathf.Cos((i * 2 * Mathf.PI) / circleResolution), 0, Mathf.Sin((i * 2 * Mathf.PI) / circleResolution)) * circleRadius));
						}
					}
					buffer.Add(buffer0);
					break;
				case MeshType.CustomMesh:
					if (mesh != lastMesh || contours == null)
					{
						CalculateMeshContour();
						lastMesh = mesh;
					}

					if (contours != null)
					{
						woffset += center;

						bool reverse = Vector3.Dot ( tr.up, Vector3.up ) < 0;

						for (int i = 0; i < contours.Length; i++)
						{
							Vector3[] contour = contours[i];

							buffer0 = Pathfinding.Util.ListPool<Pathfinding.ClipperLib.IntPoint>.Claim(contour.Length);
							if (useRotation)
							{
								Matrix4x4 m = tr.localToWorldMatrix;
								for (int x = 0; x < contour.Length; x++)
								{
									buffer0.Add(V3ToIntPoint(m.MultiplyPoint3x4(center + contour[x] * meshScale)));
								}
							}
							else
							{
								for (int x = 0; x < contour.Length; x++)
								{
									buffer0.Add(V3ToIntPoint(woffset + contour[x] * meshScale));
								}
							}

							if (reverse) buffer0.Reverse();

							buffer.Add(buffer0);
						}
					}
					break;
			}
		}

#endregion
	}
}

#endif
