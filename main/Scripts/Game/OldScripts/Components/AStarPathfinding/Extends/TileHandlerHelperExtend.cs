using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.Util;

#if APP_UPDATED

namespace Pathfinding
{
	public partial class TileHandlerHelper
	{
		/** Returns the active TileHandlerHelper object in the scene.*/
		public static TileHandlerHelper instance = null;

		private List<Bounds> _disabledCuts = new List<Bounds>();

		public void CreateTileHandlerAndRemoveListener(NavMeshScanEvent evt)
		{
			CreateTileHandler();
			EventManager.instance.RemoveListener<NavMeshScanEvent>(CreateTileHandlerAndRemoveListener);
		}

		public void OnNavMeshCutDisabled(Bounds disabledBounds)
		{
			_disabledCuts.Add(disabledBounds);
		}

		private void CreateTileHandler()
		{
			if (null == handler)
			{
				// changed checks around and called UpdateShortcuts to get recastGraph
				if (AstarPath.active == null || AstarPath.active.astarData == null)
				{
					EB.Debug.LogWarning("No AstarPath object in the scene or no astarData on that AstarPath object");
					EventManager.instance.AddListener<NavMeshScanEvent>(CreateTileHandlerAndRemoveListener);
					return;
				}

				if (AstarPath.active.astarData.recastGraph == null)
				{
					Debug.Log("No AstarPath object in the scene or no RecastGraph on that AstarPath object, UpdateShortcuts");
					AstarPath.active.astarData.UpdateShortcuts();
				}

				if (AstarPath.active.astarData.recastGraph == null)
				{
					EB.Debug.LogWarning("No RecastGraph on that AstarPath object");
					EventManager.instance.AddListener<NavMeshScanEvent>(CreateTileHandlerAndRemoveListener);
					return;
				}

				if (null == AstarPath.active.astarData.recastGraph.GetTiles())
				{
					EB.Debug.LogWarning("No NavmeshTile in RecastGraph");
					EventManager.instance.AddListener<NavMeshScanEvent>(CreateTileHandlerAndRemoveListener);
					return;
				}

				if (null == AstarPath.active.astarData.recastGraph.GetTiles())
				{
					EB.Debug.LogWarning("CreateTileHandler() No RecastGraph in the scene");
					return;
				}

				handler = new TileHandler(AstarPath.active.astarData.recastGraph);

				if (null == handler)
				{
					EB.Debug.LogWarning("CreateTileHandler() No TileHandler object");
					return;
				}

				handler.CreateTileTypesFromGraph();
			}
		}

		#region override functions

		void Start()
		{
			if (FindObjectsOfType(typeof(TileHandlerHelper)).Length > 1)
			{
				EB.Debug.LogError("There should only be one TileHandlerHelper per scene. Destroying.");
				Destroy(this);
				return;
			}

#if BNICKSON_UPDATED
			CreateTileHandler();

			instance = this;
#else
			if (handler == null)
			{
				if (AstarPath.active == null || AstarPath.active.astarData.recastGraph == null)
				{
					EB.Debug.LogWarning("No AstarPath object in the scene or no RecastGraph on that AstarPath object");
				}

				handler = new TileHandler(AstarPath.active.astarData.recastGraph);
				handler.CreateTileTypesFromGraph();
			}
#endif
		}

		public void ForceUpdate()
		{
#if BNICKSON_UPDATED
			instance = this;
#endif

			if (handler == null)
			{
				throw new System.Exception("Cannot update graphs. No TileHandler. Do not call this method in Awake.");
			}

			lastUpdateTime = Time.realtimeSinceStartup;

			List<NavmeshCut> cuts = NavmeshCut.GetAll();

			if (forcedReloadBounds.Count == 0)
			{
				int any = 0;

				for (int i = 0; i < cuts.Count; i++)
				{
#if BNICKSON_UPDATED
					// added PreUpdate call
					cuts[i].PreUpdate();
#endif

					if (cuts[i].RequiresUpdate())
					{
						any++;
						break;
					}
				}

#if BNICKSON_UPDATED
				// added _disabledCuts.Count == 0
				if (any == 0 && _disabledCuts.Count == 0) return;
#else
				// Nothing needs to be done for now
				if (any == 0) return;
#endif
			}

			bool end = handler.StartBatchLoad();

			//Debug.Log ("Updating...");

			for (int i = 0; i < forcedReloadBounds.Count; i++)
			{
				handler.ReloadInBounds(forcedReloadBounds[i]);
			}
			forcedReloadBounds.Clear();

			for (int i = 0; i < cuts.Count; i++)
			{
				if (cuts[i].enabled)
				{
					if (cuts[i].RequiresUpdate())
					{
						handler.ReloadInBounds(cuts[i].LastBounds);
						handler.ReloadInBounds(cuts[i].GetBounds());
					}
				}
				else if (cuts[i].RequiresUpdate())
				{
					handler.ReloadInBounds(cuts[i].LastBounds);
				}
			}

			for (int i = 0; i < cuts.Count; i++)
			{
				if (cuts[i].RequiresUpdate())
				{
					cuts[i].NotifyUpdated();
				}
			}

#if BNICKSON_UPDATED
			// a cut has been disabled
			for (int i = 0; i < _disabledCuts.Count; i++)
			{
				handler.ReloadInBounds(_disabledCuts[i]);
			}
			_disabledCuts.Clear();
#endif

			if (end) handler.EndBatchLoad();
		}

		#endregion
	}
}

#endif