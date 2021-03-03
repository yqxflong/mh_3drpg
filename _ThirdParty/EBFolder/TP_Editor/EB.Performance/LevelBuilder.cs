using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LevelBuilder : EBWorldBuilder<RenderSettings>
{
		private char slash = System.IO.Path.DirectorySeparatorChar;
		private Dictionary<string, EBMeshUtils.CHANNEL[]> _channelMap = new Dictionary<string, EBMeshUtils.CHANNEL[]>{  	
		{ "EBG/Enviro/Lit/Diffuse", 				new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1 }															},
		{ "EBG/Enviro/Lit/DiffuseRim", 				new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1, EBMeshUtils.CHANNEL.NORMAL }								},
		{ "EBG/Enviro/Lit/DiffuseLightProbe",		new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1, EBMeshUtils.CHANNEL.NORMAL }								},
		
		{ "EBG/Enviro/LM/Base", 					new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1, EBMeshUtils.CHANNEL.UV2 }									},
		{ "EBG/Enviro/LM/Blend", 					new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1, EBMeshUtils.CHANNEL.UV2 }									},
		{ "EBG/Enviro/LM/Puddle", 					new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1, EBMeshUtils.CHANNEL.UV2 }									},
		{ "EBG/Enviro/LM/Road", 					new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1, EBMeshUtils.CHANNEL.UV2 }									},
		{ "EBG/Enviro/LM/RoadNew", 					new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1, EBMeshUtils.CHANNEL.UV2 }									},
		{ "EBG/Enviro/LM/RoadDecal",				new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1, EBMeshUtils.CHANNEL.UV2 }									},
		{ "EBG/Enviro/LM/Vegetation", 				new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1, EBMeshUtils.CHANNEL.UV2 }									},
		{ "EBG/Enviro/LM/VertexColor", 				new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1, EBMeshUtils.CHANNEL.UV2, EBMeshUtils.CHANNEL.COLOR }		},
		{ "EBG/Enviro/LM/Water", 					new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1, EBMeshUtils.CHANNEL.UV2 }									},
		
		{ "EBG/Enviro/Self-Illumin/Additive", 		new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1	}															},
		{ "EBG/Enviro/Self-Illumin/AnimatedMask", 	new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1	}															},
		{ "EBG/Enviro/Self-Illumin/AnimatedStrip", 	new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1, EBMeshUtils.CHANNEL.COLOR }								},
		{ "EBG/Enviro/Self-Illumin/AnimatedStripBlend", new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1, EBMeshUtils.CHANNEL.COLOR }							},
		{ "EBG/Enviro/Self-Illumin/AnimatedStripAdditive", new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1, EBMeshUtils.CHANNEL.COLOR }							},
		{ "EBG/Enviro/Self-Illumin/Base", 			new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1 }															},
		{ "EBG/Enviro/Self-Illumin/Blend", 			new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1	}															},
		{ "EBG/Enviro/Self-Illumin/GlassAlways",	new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.NORMAL }														},
		{ "EBG/Enviro/Self-Illumin/GlassWindow",	new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1, EBMeshUtils.CHANNEL.NORMAL }								},
		{ "EBG/Enviro/Self-Illumin/Interior", 		new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1, EBMeshUtils.CHANNEL.NORMAL, EBMeshUtils.CHANNEL.TANGENT }	},
		{ "EBG/Enviro/Self-Illumin/LM", 			new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1, EBMeshUtils.CHANNEL.UV2, EBMeshUtils.CHANNEL.COLOR }		},
		
		{ "EBG/Enviro/Unlit/Additive", 				new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1	}						 									},
		{ "EBG/Enviro/Unlit/Base", 					new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1	}						 									},
		{ "EBG/Enviro/Unlit/Blend", 				new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1	}					 										},
		{ "EBG/Enviro/Unlit/Multiply", 				new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1	}						 									},
		{ "EBG/Enviro/Unlit/VertexColor", 			new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1, EBMeshUtils.CHANNEL.COLOR	}						 		},
		{ "EBG/Enviro/Unlit/Skybox", 				new EBMeshUtils.CHANNEL[] { EBMeshUtils.CHANNEL.UV1 }						 									}
	};
	
		Dictionary<int, int> _layerMap = new Dictionary<int, int>{ 
		{ LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("environment") },
		{ LayerMask.NameToLayer("ground"), LayerMask.NameToLayer("environment") }
	};
	
		private Dictionary<string, BlockInfo> blockInfos;
		private Dictionary<string, TrackInfo> trackInfos;
		private Dictionary<string, GameObject> blockCodesMapping;
		//private Dictionary<string, GameObject> sceneInfos;
		private string scene;

		private WorldInfo m_world_info;

		public LevelBuilder (string worldName) : base(worldName)
		{
				//grab the world_info_list
				m_world_info = GenerateWorldInfoFromScene ();

				//blockInfos = LoadBlockInfo();
				//trackInfos = LoadTrackInfo();
				//sceneInfos = LoadSceneInfo();
		}
	
		public class BlockInfo
		{
				public string path;
				public GameObject prefab;
		
				public BlockInfo ()
				{
						path = string.Empty;
						prefab = null;
				}
		}
	
		public class TrackInfo
		{
				public string[] blockCodes;
		}
	
		private Dictionary<string, BlockInfo> LoadBlockInfo ()
		{
				return null;

				/*Dictionary<string, BlockInfo> blockInfos = new Dictionary<string, BlockInfo>();
		
		char slash = System.IO.Path.DirectorySeparatorChar;
		string basePath = "Assets" + slash + "Resources" + slash + "Bundles" + slash + "environments" + slash + "blocks";
		var paths 		= GeneralUtils.GetFilesRecursive(basePath, "*.prefab");
		
		foreach (var path in paths )
		{
			var pathSplit = path.Split('/'); //purposely doesn't split on 'slash', as this function always returns a forward slash
			var name = pathSplit[pathSplit.Length - 1].Split('.')[0];
			var go = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
			if (!go)
			{
				Debug.LogError("Nothing at path: " + path);
				continue;
			}
			var track = go.GetComponent<TrackPieceData>();
			if (!track)
			{
				Debug.LogWarning("Nothing track piece data for " + path);
				continue;
			}
			
			BlockInfo blockInfo = new BlockInfo();
			blockInfo.path = path;
			blockInfo.prefab = go;
			blockInfos.Add(track.TrackID, blockInfo);
		}
		return blockInfos;*/
		}

		/*public Dictionary<string, TrackInfo> LoadSceneInfo()
	{
	}*/

		public Dictionary<string, TrackInfo> LoadTrackInfo ()
		{
				return null;

				/*Dictionary<string, TrackInfo> trackInfos = new Dictionary<string, TrackInfo>();
		char slash = System.IO.Path.DirectorySeparatorChar;
		var lines = System.IO.File.ReadAllLines("Assets" + slash + "Editor" + slash + "track_codes.txt");
		foreach( var line in lines )
		{
			var p = line.Split(',');
			if (p.Length != 2 )
				continue;
			
			string name = p[0];
			string[] pieceCodes = p[1].Split('&');
			TrackInfo ti = new TrackInfo();
			ti.blockCodes = pieceCodes;
			trackInfos[name] = ti;
		}
		return trackInfos;*/
		}
	
		private void SetDefaultLightmapSettings ()
		{
				UnityEngine.LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
				UnityEditor.LightmapEditorSettings.realtimeResolution = 35;
				UnityEditor.LightmapEditorSettings.padding = 3;
				UnityEngine.RenderSettings.ambientLight = Color.black;
		}
	
		public override EBMeshUtils.Channels GetChannelMap (Shader shader)
		{
				if (shader == null) {
						Debug.LogError ("Null shader");
						return new EBMeshUtils.Channels (true);
				}
		
				if (!_channelMap.ContainsKey (shader.name)) {
						Debug.LogWarning ("No channel map for shader " + shader.name);
						return new EBMeshUtils.Channels (true);
				}
		
				return new EBMeshUtils.Channels (_channelMap [shader.name]);
		}
	
		public override int RemapLayer (int layer)
		{
				if (_layerMap.ContainsKey (layer))
						return _layerMap [layer];
				return layer;
		}
	
		public override GameObject GetPrefabForCode (string code)
		{
				return blockCodesMapping [code];
		}
	
		private void CreateAISplines ()
		{
				/*Debug.Log("Creating Ai Spline");
		
		var generator = GameObject.Find("TrackGenerator").GetComponent<TrackGenerator>();
		
		System.Collections.Generic.List<AiCubicBezSplineNode> _splineNodes = new System.Collections.Generic.List<AiCubicBezSplineNode>();
		
		int index = 0;
		
		System.Collections.Generic.Dictionary<int, float> frontTangentOfRemovedNode = new System.Collections.Generic.Dictionary<int, float>();
		
		TrafficManager savedTM = null;
		float[] laneOffsets = new float[0];
		
		foreach(TrackPieceData piece in generator._trackPieces)
		{
			if (piece._playerSplines != null)
			{
				if (index == 0)
				{
					savedTM = piece._playerSplines.GetComponent<TrafficManager>();
					
					laneOffsets = new float[piece._playerSplines.Lanes.Length];
					
					for (int i = 0; i < piece._playerSplines.Lanes.Length; i++)	
					{
						laneOffsets[i] = piece._playerSplines.Lanes[i].Offset;
					}
				}
				
				int node = 0;
				
				if (piece._playerSplines._cubicBezNodes == null || piece._playerSplines._cubicBezNodes.Count == 0)	
				{
					foreach(AiCubicBezSplineNode acbsn in piece._playerSplines.gameObject.GetComponentsInChildren<AiCubicBezSplineNode>())
					{
						piece._playerSplines._cubicBezNodes.Add(acbsn);
					}
				}
				
				foreach(AiCubicBezSplineNode splineNode in piece._playerSplines._cubicBezNodes)
				{
					if (index == 0)
					{
						_splineNodes.Add(splineNode);
						index++;
					}
					else if (node == 0)
					{     
						frontTangentOfRemovedNode.Add(index-1, splineNode.forwardTangent.transform.localPosition.z);
					}
					else
					{
						_splineNodes.Add(splineNode);
						index++;
					}
					
					node++;
				}
			}
		}	
		
		// Create master AiSpline for level and set offsets based on start block offsets
		AiSpline aiSpline = new GameObject("AiSpline").AddComponent<AiSpline>();
		
		aiSpline.Lanes = new AiSpline.Lane[laneOffsets.Length];
		
		aiSpline.MainLane = new AiSpline.Lane();
		aiSpline.MainLane.Draw = true;
		
		for(int i = 0; i < aiSpline.Lanes.Length; i++)
		{
			aiSpline.Lanes[i] = new AiSpline.Lane();
			aiSpline.Lanes[i].Draw = true;
			aiSpline.Lanes[i].Offset = laneOffsets[i];
		}
		
		aiSpline.ReplaceNodesExact(_splineNodes.ToArray());
		
		// Fix nodes for spline -> Remove duplicates and alter tangets to smooth curves
		foreach(int i in frontTangentOfRemovedNode.Keys)
		{
			GameObject waypoint = EB.Util.GetObjectExactMatch(aiSpline.gameObject, "AiWaypoint_" + i.ToString("00"));
			if (waypoint != null)
			{
				Transform frontTangent = waypoint.transform.FindChild("Front");
				
				if (frontTangent != null)
				{
					frontTangent.localPosition = new Vector3(0,0,frontTangentOfRemovedNode[i]);
				}
			}
		}
		
		// Add Traffic Manager and populate info from Start Block
		if (savedTM != null)
		{
			TrafficManager tm = aiSpline.gameObject.AddComponent<TrafficManager>();
			tm.AiTrafficVehiclePrefabs = savedTM.AiTrafficVehiclePrefabs;
			tm._maximumGapBetweenCars = savedTM._maximumGapBetweenCars;
			tm._minimumGapBetweenCars = savedTM._minimumGapBetweenCars;
			tm._minimumStartingDistanceFromPlayer = savedTM._minimumStartingDistanceFromPlayer;
			tm._trafficLevel = savedTM._trafficLevel;
		}
		
		// Kill all the old splines
		foreach(TrackPieceData piece in generator._trackPieces)
		{
			if (piece._playerSplines != null)
				DestroyImmediate(piece._playerSplines.gameObject);
			
			DestroyImmediate(piece._exitLocator); //we still use entry locators
			
			foreach(SetupSpline ss in piece.GetComponentsInChildren<SetupSpline>())
			{
				DestroyImmediate (ss.collider);
				DestroyImmediate (ss);
			}
		}
		
		// Fix all remaining splines!
		foreach(AiSpline spline in GameObject.FindObjectsOfType<AiSpline>())
		{
			Debug.Log("Fixing / Normalizing spline: " + spline.name);
			spline.FixSpline(true);
		}
		
		var track = GameObject.Find("z_track");
		
		if (track == null)
		{
			return;
		}
		
		// Setup exit points in splines
		foreach(SetSplineLaneTime sslt in track.GetComponentsInChildren<SetSplineLaneTime>())
		{	
			if (sslt._type == SetSplineLaneTime.Type.ExitMainSpline)
			{
				AiSpline.Exit newExit = new AiSpline.Exit();
				newExit.fromSpline = aiSpline;
				sslt._fromSpline = aiSpline;
				newExit.toSpline = sslt._toSpline;
				
				if (sslt._setAllLanes) newExit.setAllLanes = true;
				else
				{
					newExit.fromLane = sslt._fromLane;
					newExit.toLane = sslt._toLane;
				}
				
				if (sslt._affectedType == SetSplineLaneTime.CarType.All)
				{
					newExit.affectRacers = true;
					newExit.affectTraffic = true;
				}
				else if (sslt._affectedType == SetSplineLaneTime.CarType.Racer)
				{
					newExit.affectRacers = true;
					newExit.affectTraffic = false;
				}
				else if (sslt._affectedType == SetSplineLaneTime.CarType.Traffic)
				{
					newExit.affectRacers = false;
					newExit.affectTraffic = true;
				}
				
				if (aiSpline != null)
				{
					sslt._fromTime = aiSpline.GetTimeAtPointWithThreshold(sslt.transform.position, 25.0f);
				}
				if (newExit.toSpline != null)
				{
					sslt._toTime = newExit.toSpline.GetTimeAtPointWithThreshold(sslt.transform.position, 25.0f);
				}
				
				newExit.fromTime = sslt._fromTime;
				newExit.toTime = sslt._toTime;
				
				newExit.lastTrafficSpline = sslt._disableTrafficRepositioning;
				
				if (aiSpline._exits == null) aiSpline._exits = new List<AiSpline.Exit>();
				aiSpline._exits.Add(newExit);
			}
			else
			{
				AiSpline.Exit newExit = new AiSpline.Exit();
				newExit.fromSpline = sslt._fromSpline;
				newExit.toSpline = aiSpline;
				sslt._toSpline = aiSpline;
				
				if (sslt._setAllLanes) newExit.setAllLanes = true;
				else
				{
					newExit.fromLane = sslt._fromLane;
					newExit.toLane = sslt._toLane;
				}
				
				if (sslt._affectedType == SetSplineLaneTime.CarType.All)
				{
					newExit.affectRacers = true;
					newExit.affectTraffic = true;
				}
				else if (sslt._affectedType == SetSplineLaneTime.CarType.Racer)
				{
					newExit.affectRacers = true;
					newExit.affectTraffic = false;
				}
				else if (sslt._affectedType == SetSplineLaneTime.CarType.Traffic)
				{
					newExit.affectRacers = false;
					newExit.affectTraffic = true;
				}
				
				if (sslt._fromSpline != null)
				{
					sslt._fromTime = sslt._fromSpline.GetTimeAtPointWithThreshold(sslt.transform.position, 25.0f);
				}
				if (aiSpline != null)
				{
					sslt._toTime = aiSpline.GetTimeAtPointWithThreshold(sslt.transform.position, 25.0f);
				}
				
				newExit.fromTime = sslt._fromTime;
				newExit.toTime = sslt._toTime;
				
				if (sslt._useLaneOffset)
				{
					newExit.laneOffset = sslt._laneOffset;
				}
				else
				{
					newExit.laneOffset = 0;
				}
				
				
				if (newExit.fromSpline._exits == null) newExit.fromSpline._exits = new List<AiSpline.Exit>();
				newExit.fromSpline._exits.Add(newExit);
			}
		}
		
		// Calculate spline times for exits in Decision blocks
		foreach(TrackPieceData tpd in track.GetComponentsInChildren<TrackPieceData>())
		{
			if (tpd._pieceType == TrackPieceData.TRACK_PIECE_TYPE.GATE_DECISION)
			{
				foreach(GameObject go in tpd._decisionObjects)
				{
					go.SetActive(true);
					
					foreach(SetSplineLaneTime sslt in go.GetComponentsInChildren<SetSplineLaneTime>())
					{
						if (sslt._type == SetSplineLaneTime.Type.ExitMainSpline)
						{
							if (aiSpline != null)
							{
								sslt._fromTime = aiSpline.GetTimeAtPointWithThreshold(sslt.transform.position, 25.0f);
							}
							
							if (sslt._toSpline != null)
							{
								sslt._toTime = sslt._toSpline.GetTimeAtPointWithThreshold(sslt.transform.position, 25.0f);
							}
						}
						else
						{
							if (sslt._fromSpline != null)
							{
								sslt._fromTime = sslt._fromSpline.GetTimeAtPointWithThreshold(sslt.transform.position, 25.0f);
							}
							
							if (aiSpline != null)
							{
								sslt._toTime = aiSpline.GetTimeAtPointWithThreshold(sslt.transform.position, 25.0f);
							}
						}
					}
					
					go.SetActive(false);
				}
			}
		}
		
		aiSpline._updateLive = true;*/
		}
	
		private void AdjustDecalLayer ()
		{
				Vector3 decalOffset = new Vector3 (0.0f, 0.01f, 0.0f);
				int decalLayer = LayerMask.NameToLayer ("decal");
				GameObject[] gameObjects = FindObjectsOfType<GameObject> ();
				foreach (var gameObject in gameObjects) {
						if ((gameObject.layer == decalLayer) && (gameObject.transform.childCount == 0)) {
								gameObject.transform.position += decalOffset;
						}
				}
		}
	
		public override float GetLightProbeVolumeFactor ()
		{
				if (_worldName.Contains ("garage")) {
						return 0.5f;
				}
				return 0.15f;
		}
	
		private void KillLighting ()
		{
				foreach (Light light in Object.FindObjectsOfType(typeof(Light))) {
						if ((light.type != LightType.Directional) && (light.tag != "KeepOnBuild")) {
								if ((light.gameObject.transform.childCount == 0) && (light.gameObject.GetComponents<Component> ().Length == 2)) {
										//it's just a light with no children, kill the whole thing
										Object.DestroyImmediate (light.gameObject);
								} else {
										//has other things, just kill the lighting component
										Object.DestroyImmediate (light);
								}
						}
				}
		
				var track = GameObject.Find ("z_track");
		
				if (track == null) {
						return;
				}
		
				//List<MeshFilter> filtersList = new List<MeshFilter> ();
		
				foreach (KeyValuePair<string,GameObject> map in blockCodesMapping) {
						GameObject prefab = (GameObject)map.Value;
						if (!prefab) {
								Debug.LogError ("No prefab " + map.Key + "_" + prefab.name + " found.");
								continue;
						}
			
						var z_lighting = EB.Util.GetObjectExactMatch (prefab, "z_lighting"); 
						if (z_lighting) {
								DestroyImmediate (z_lighting);
						}
				}
		}
	
		private void KillExtraneousObjects ()
		{
				/*object[] allObjects = FindObjectsOfTypeAll(typeof(GameObject));
		
		System.Collections.Generic.List<GameObject> ObjectsToDestroy = new System.Collections.Generic.List<GameObject>();
		
		for (int i = 0; i < allObjects.Length; i++)
		{	
			GameObject obj = (GameObject)allObjects[i];
			
			if (obj.activeInHierarchy)
			{
				string name = obj.name;
				
				if (name.Contains("matNode") || name.Contains("matVert") || name.Contains("TangentForward") || name.Contains("TangentBackward") || name.Contains("bakeonly"))
				{
					ObjectsToDestroy.Add(obj);
				}
				else if (name.Equals("Nodes") && obj.GetComponentsInChildren<Collider>().Length == 0)
				{
					ObjectsToDestroy.Add(obj);
				}
				
				if (obj.GetComponent<RoadBuilder>() != null) DestroyImmediate (obj.GetComponent<RoadBuilder>());
				if (obj.GetComponent<RoadNode>() != null) 
				{
					DestroyImmediate (obj.GetComponent<RoadNode>());
					if (obj.GetComponent<BoxCollider>() != null)
					{
						DestroyImmediate(obj.GetComponent<BoxCollider>());
					}
				}
				if (obj.GetComponent<Building>() != null) DestroyImmediate (obj.GetComponent<Building>());
			}
		}
		
		for(int i = 0; i < ObjectsToDestroy.Count; i++)
		{
			if (ObjectsToDestroy[i] != null)
			{
				DestroyImmediate(ObjectsToDestroy[i]);
			}
		}*/
		}
	
		//CALLBACKS
	
		public override void WillBuildWorld ()
		{	
				//P4Connect.Config.PerforceEnabled = false;
		}

		public WorldInfo GenerateWorldInfoFromScene ()
		{
				string path = "Assets" + slash + "GM" + slash + "GameAssets" + slash + "Environment" + slash + _worldName + slash + "Prefabs";

				WorldInfo worldInfo = new WorldInfo ();
				List<string> pieceCodes = new List<string> ();
				blockCodesMapping = new Dictionary<string, GameObject> ();
		
				//loop through all gameobjects, if they are a prefab add them to worldinfo
				GameObject[] obj = (GameObject[])GameObject.FindObjectsOfType (typeof(GameObject));
				//List<GameObject> assetbundle_list = new List<GameObject> ();
				for (int i = 0; i < obj.Length; i++) {
						if (PrefabUtility.FindPrefabRoot (obj [i]) == obj [i] && PrefabUtility.GetPrefabType (obj [i]) == PrefabType.PrefabInstance) {
								pieceCodes.Add (obj [i].name);
								blockCodesMapping.Add (obj [i].name, obj [i]);

								string asset_path = path + slash + obj [i].name + ".prefab";
								Debug.Log (asset_path);
								GameObject go = AssetDatabase.LoadAssetAtPath (asset_path, typeof(GameObject)) as GameObject;
								if (!go) {
										Debug.LogError ("Nothing at path: " + asset_path);
										continue;
								}
						}
				}
		
				worldInfo.pieceCodes = pieceCodes.ToArray ();
		
				return worldInfo;
		}

		public override WorldInfo AssembleWorld (GameObject root)
		{ 
				/*if(root != null)
		{
			root.AddComponent<AssetBundleHelper>();
		}

		string path = "Assets" + slash + "GM" + slash + "GameAssets" + slash + "Environment" + slash + _worldName + slash + "Prefabs";
		for(int i = 0; i < m_world_info.pieceCodes.Length; i++)
		{
			string asset_path = path + slash + m_world_info.pieceCodes[i] + ".prefab";
			Debug.Log ("Instantiating: " + asset_path);
			GameObject to_instantiate = AssetDatabase.LoadAssetAtPath(asset_path, typeof(GameObject)) as GameObject;
			GameObject go = (GameObject)GameObject.Instantiate(to_instantiate);
			if(go.GetComponent<AssetBundleHelper>() == null)
			{
				go.AddComponent<AssetBundleHelper>();
			}

			go.name = m_world_info.pieceCodes[i] + "-Built";
			blockCodesMapping[m_world_info.pieceCodes[i]] = go;
		}*/
			                                     
				return m_world_info;
		}
	
		public override void MergeAdditionalCustom (string pieceCode)
		{
				/*GameObject prefab = GetPrefabForCode(pieceCode);
		TrackPieceData tpd = prefab.GetComponent<TrackPieceData>();
		if(tpd != null)
		{
			GameObject[] decisions = tpd._decisionObjects;
			foreach(GameObject d in decisions)
			{
				d.gameObject.SetActive(true);
				BreakableObject[] breakArray = d.GetComponentsInChildren<BreakableObject>();
				List<MeshFilter> meshFilters = new List<MeshFilter>();
				
				foreach(BreakableObject b in breakArray) 
				{
					MeshFilter[] decisionMeshFilters = b.GetComponentsInChildren<MeshFilter>();
					foreach(MeshFilter m in decisionMeshFilters)
					{
						m.gameObject.layer = LayerMask.NameToLayer("vehicle_collider");
						meshFilters.Add(m);
					}
				}
				
				
				GameObject mergedBreakableMeshes = new GameObject("MergedBreakableMeshes");
				mergedBreakableMeshes.transform.parent = d.transform;
				
				List<GameObject> gos = SkinnedMeshMerger.Merge(meshFilters.ToArray(), (_worldName+"_"+d.gameObject.name), _worldName);
				foreach(GameObject g in gos) 
				{
					g.layer = LayerMask.NameToLayer("environment");
					g.transform.parent = mergedBreakableMeshes.transform;
				}
				d.gameObject.SetActive(false);
				
			}
		}*/
		}
	
		public override MeshFilter[] GetSkinnedMeshsFor (string pieceCode)
		{
				/*GameObject go = GetPrefabForCode(pieceCode);
		BreakableObject[] breakables;
		List<MeshFilter> meshFilters = new List<MeshFilter>();
		
		if(!go) 
		{
			Debug.LogError("Prefab for code "+pieceCode+" is null");
			return meshFilters.ToArray();
		}
		
		breakables = go.GetComponentsInChildren<BreakableObject>();
		
		foreach(var breakable in breakables)
		{
			breakable.gameObject.layer = LayerMask.NameToLayer("vehicle_collider");
			Renderer[] ignoredRenderers = breakable._renderersToTurnOff;
			MeshFilter[] meshFiltersPerBreakable = breakable.gameObject.GetComponentsInChildren<MeshFilter>();
			
			if(meshFiltersPerBreakable != null && meshFiltersPerBreakable.Length > 0)
			{
				foreach(MeshFilter meshFilter in meshFiltersPerBreakable)
				{
					bool ignore = false;
					foreach(Renderer ignoredRenderer in ignoredRenderers) // To ignore is temporary until all smackables are fixed up to have no "whole" pieces
					{
						if(meshFilter != null && meshFilter.renderer == ignoredRenderer)
						{
							meshFilter.renderer.enabled = false;
							DestroyImmediate(meshFilter.renderer);
							DestroyImmediate(meshFilter);
							ignore = true;
							break;
						}
					}
					if(meshFilter == null)
					{
						continue;
					}
					if(!ignore)
					{
						meshFilters.Add(meshFilter);
					}
				}
			}
		}
		return meshFilters.ToArray();*/
				return null;
		}
	
		public override void DidMergeWorld ()
		{	
				EditorUtility.DisplayProgressBar ("Cleaning up", "Combining GameObjects", 0.0f);

				//put all of the global data under "Global level data"
				GameObject global_data = new GameObject ("Global Level Data");
				global_data.AddComponent<AssetBundleHelper> ();

				string[] objects_to_delete = { "Main Camera" };
				for (int i = 0; i < objects_to_delete.Length; i++) {
						GameObject to_delete = GameObject.Find (objects_to_delete [i]);
						if (to_delete != null) {
								GameObject.DestroyImmediate (to_delete);
						}
				}

				string[] objects_to_parent = { "WorldPainterData", "RenderSettings", "LightProbes" };
				for (int i = 0; i < objects_to_parent.Length; i++) {
						GameObject to_delete = GameObject.Find (objects_to_parent [i]);
						if (to_delete != null) {
								to_delete.transform.parent = global_data.transform;
						}
				}

				//Create Prefabs & Generate Assetbundles
				EditorUtility.DisplayProgressBar ("Cleaning up", "Creating Prefabs & Assetbundles", 0.6f);
				BuildHelper.CreateAssetBundlesFromScene ();

				//Deploy?

				/*EditorUtility.DisplayProgressBar("Cleaning up", "Removing Empty GameObjects and Empty Meshes", 0.0f);
		KillLighting();
		EditorUtility.DisplayProgressBar("Cleaning up", "Removing Empty GameObjects and Empty Meshes", 0.25f);
		KillExtraneousObjects();
		EditorUtility.DisplayProgressBar("Cleaning up", "Removing Empty GameObjects and Empty Meshes", 0.5f);
		RemoveEmptyAnims();
		EditorUtility.DisplayProgressBar("Cleaning up", "Removing Empty GameObjects and Empty Meshes", 0.75f);
		
		List<MeshFilter> filtersList = new List<MeshFilter>();
		List<Transform> transforms = new List<Transform>();
		List<GameObject> z_unmerged_gameObjects = new List<GameObject>();
		
		foreach(KeyValuePair<string,GameObject> piece in blockCodesMapping)
		{
			var prefab = piece.Value;
			if (!prefab)
			{
				Debug.LogError("No prefab for code " + prefab.name);
				continue;
			}
			
			var unmerged = EB.Util.GetObjectExactMatch(prefab,"z_unmerged"); 
			if (!unmerged)
			{
				Debug.LogError("No z_unmerged found in prefab " + prefab.name + ". Not merging this block.");
				continue;
			}
			
			z_unmerged_gameObjects.Add(unmerged as GameObject);
			
			var meshFilters = EB.Util.FindAllComponents<MeshFilter>(unmerged);
			
			filtersList.AddRange(meshFilters);
			transforms.AddRange(EB.Util.FindAllComponents<Transform>(unmerged));
			
		}	
		
		//skip hidden meshes
		for (int i = filtersList.Count - 1; i >= 0; --i)
		{
			MeshFilter filter = filtersList[i];
			if (!filter.gameObject.activeInHierarchy)
			{
				filtersList.RemoveAt(i);
			}
		}
		
		var filters = filtersList.ToArray();
		
		int childDepth = 5;
		for (int i = 0; i < childDepth; ++i)
		{
			foreach(var filter in filters)
			{
				if (filter == null)
					continue;
				
				int childCount = filter.gameObject.transform.childCount;
				Component[] components = filter.gameObject.GetComponents<Component>();
				
				if ((childCount == 0) && (components.Length == 4) && (filter.gameObject.GetComponents<Building>().Length == 1))
				{
					//a transform, or a transform, mesh filter, mesh renderer (which we merged), and a building script; so kill the parent node
					DestroyImmediate(filter.gameObject);
				}
			}
			
			foreach(var transform in transforms)
			{
				if (transform.gameObject.name == "Front" || transform.gameObject.name == "Back")
					continue;
			}
		}
		
		for (int i = 0; i < z_unmerged_gameObjects.Count; i++)
		{
			RemoveEmptyGameObjects(z_unmerged_gameObjects[i]);
		}
		
		Object.DestroyImmediate( GameObject.Find("Main Camera") ); 
		EditorUtility.ClearProgressBar();*/
		}
	
		private void RemoveEmptyAnims ()
		{
				GameObject[] objects = (GameObject[])Resources.FindObjectsOfTypeAll (typeof(GameObject));
		
				for (int i = 0; i < objects.Length; i++) {
						Animation anim = objects [i].GetComponent<Animation> ();
			
						if (anim != null) {
								if (anim.GetClipCount () == 0) {
										//Debug.Log("Removing Animation component with zero clips on object: " + objects[i].name);
										DestroyImmediate (anim, true);
								}
						}	
			
						Animator animator = objects [i].GetComponent<Animator> ();
			
						if (animator != null) {
								if (animator.avatar == null || animator.runtimeAnimatorController == null) {
										//Debug.Log("Removing Animator component with no avatar or controller: " + objects[i].name);
										DestroyImmediate (animator, true);
								}
						}
				}
		}
	
		private void RemoveEmptyGameObjects (GameObject go)
		{
				Transform[] transforms = go.GetComponentsInChildren<Transform> ();
				int index = 0;
		
				foreach (var transform in transforms) {
						if (transform == null)
								continue;
			
						CheckForDelete (transform);
			
						index++;
				}
		}
	
		private void CheckForDelete (Transform t)
		{
				bool destroy = false;
		
				if ((t.childCount == 0) && (t.gameObject.GetComponents<Component> ().Length == 1)) {
						//empty node
						//Debug.Log("Removing empty GameObject: " + t.gameObject.name);
						destroy = true;
				} else if ((t.childCount == 0) && (t.gameObject.GetComponents<Component> ().Length == 3)) {
						MeshFilter mf = t.GetComponent<MeshFilter> ();
			
						if (mf != null) {
								if (mf.sharedMesh == null) {
										//Debug.Log("Removing GameObject without mesh: " + t.gameObject.name);
										destroy = true;
								} else if (mf.sharedMesh.vertexCount == 0) {
										//Debug.Log ("Removing GameObject with mesh with zero vertices: " + t.gameObject.name);
										destroy = true;
								}
						}
				}
		
				if (destroy) {
						Transform transformToCheck = t.parent;
						DestroyImmediate (t.gameObject, true);
			
						if (transformToCheck != null) {
								CheckForDelete (transformToCheck);
						}
				}
		}
	
		public override void DidBuildWorld ()
		{	
				//P4Connect.Config.PerforceEnabled = true;
		}
	
		//PATHS
		public override string GetMergeOutputPath (string partitionName)
		{
				string outPath = "Assets" + slash + "Merge" + slash + _worldName + slash + "Lightmaps";
				Directory.CreateDirectory (outPath);
				return outPath + slash + "partition" + partitionName + ".png";
		}
	
		public override string GetMergeGeometryPath ()
		{
				return "Assets" + slash + "Merge" + slash + _worldName + slash + "Geometry";
		}
	
		public override string GetSaveScenePath (string add = "")
		{
				return "Assets" + slash + "GM" + slash + "GameAssets" + slash + "Environment" + slash + _worldName + slash + _worldName + add + ".unity";
		}
	
		public override string GetGlobalBakeLightmapPath ()
		{
				return "Assets" + slash + "GM" + slash + "GameAssets" + slash + "Environment" + slash + _worldName + slash + _worldName;
		}
	
		public override string GetGlobalBakeLightmapLocalPath ()
		{
				return "Scenes" + slash + "tracks" + slash + _worldName;
		}
	
		public override string GetScenePath (bool isMeta = false)
		{
				string path = Application.dataPath + slash + "Scenes" + slash + "tracks" + slash + _worldName + ".unity";
		
				if (isMeta) {
						path += ".meta";
				}
				return path;
		}
	
		public override string GetWorldPainterDataPath (bool isMeta = false)
		{
				string path = Application.dataPath + slash + "TrackData" + slash + "WorldPainter" + slash + _worldName + ".prefab";
		
				if (isMeta) {
						path += ".meta";
				}
				return path;
		}
	
		public override string GetSourceLightmapsPath ()
		{
				return Application.dataPath + slash + "Scenes" + slash + "tracks" + slash + _worldName + slash;
		}
	
		public override string GetRenderSettingsFolder ()
		{
				return  "Assets" + slash + "TrackData" + slash + "RenderSettings" + slash + _worldName + slash;
		}
	
		public override string GetMergedLightmapsPath ()
		{
				return Application.dataPath + slash + "Merge" + slash + _worldName + slash;
		}
	
		public override string GetLightmapPathForPieceCode (string code)
		{
				string baseLightmapPath = Application.dataPath + slash + "OriginalAssets" + slash + "LocationFiles" + slash;
				GameObject prefab = GetPrefabForCode (code);
				string originalName = prefab.name.Substring (prefab.name.IndexOf ('_') + 1, prefab.name.Length - (prefab.name.IndexOf ('_') + 1));
				string[] pieceNameArray = originalName.Split ('_');
				return baseLightmapPath + pieceNameArray [0] + slash + pieceNameArray [1] + slash + originalName;
		}	
}
