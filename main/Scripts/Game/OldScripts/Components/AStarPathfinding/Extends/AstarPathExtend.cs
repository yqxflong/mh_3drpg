using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.RVO;
using System.Threading;

#if APP_UPDATED

public partial class AstarPath
{
    public List<Pathfinding.RVO.ObstacleVertex> obstaclesScratchList;
    public HashSet<TriangleMeshNode> trianglesHashSet;

    const int NumObstacles = 100;

    public void StartUsingObstaclesScratchList()
    {
        if (null == obstaclesScratchList)
        {
            obstaclesScratchList = new List<Pathfinding.RVO.ObstacleVertex>(NumObstacles);
        }
        if (0 != obstaclesScratchList.Count)
        {
            throw new System.Exception("Using obstaclesScratchList should be between matching StartUsingObstaclesScratchList() and StopUsingObstaclesScratchList()");
        }
        obstaclesScratchList.Clear();
    }

    public void StopUsingObstaclesScratchList()
    {
        if (null == obstaclesScratchList)
        {
            throw new System.Exception("obstaclesScratchList is null");
        }
        obstaclesScratchList.Clear();
    }

    public void StartUsingTriangleScratchList()
    {
        if (null == trianglesHashSet)
        {
            trianglesHashSet = new HashSet<TriangleMeshNode>();
        }
        if (0 != trianglesHashSet.Count)
        {
            throw new System.Exception("Using trianglesHashSet should be between matching StartUsingTriangleScratchList() and StopUsingTriangleScratchList()");
        }
        trianglesHashSet.Clear();
    }

    public void StopUsingTriangleScratchList()
    {
        if (null == trianglesHashSet)
        {
            throw new System.Exception("trianglesHashSet is null");
        }
        trianglesHashSet.Clear();
    }

    public void OnEnable()
    {
        EventManager.instance.AddListener<PopulateZonesPlayingEvent>(OnPopulateZones);
    }

    public void OnPopulateZones(PopulateZonesPlayingEvent evt)
    {
        if (scanOnStartup && AStarPathfindingUtils.WillNavMeshBeBuiltFromInputMeshOnLevelBegin())
        {
            if (!astarData.cacheStartup || astarData.data_cachedStartup == null)
            {
                LevelHelper helper = GameObject.FindObjectOfType(typeof(LevelHelper)) as LevelHelper;
                if (helper != null)
                {
                    AStarPathfindingUtils.CreateRecastNavMeshForInputMeshes(helper.transform.Find(ZoneHelper.ZonesRootName), helper.isOverworldNavMeshRequired);
                    Scan();
                    EventManager.instance.Raise(new NavMeshScanEvent());
                }
            }
        }

        if (Application.isPlaying)
        {
            RecastGraph.DestroyWalkableAreaObjects(); // we don't need the instances of AStarPathfindingWalkableArea in the game after this point
            RecastGraph.DestroyRecastMeshObjComponents(); // RecastMeshObj's are only used for nav mesh generation, they are not needed after this point
            AStarPathfindingRecastCut.DestroyRecastCutObjects();
            AStarPathfindingGenerationTimeGeo.DestroyGenerationTimeGeoObjects();
        }
    }

    #region override functions

    /** Sets up all needed variables and scans the graphs.
     * Calls Initialize, starts the ReturnPaths coroutine and scans all graphs.
     * Also starts threads if using multithreading
     * \see #OnAwakeSettings
     */
    void Awake()
    {
        //Very important to set this. Ensures the singleton pattern holds
        active = this;

        if (FindObjectsOfType(typeof(AstarPath)).Length > 1)
        {
            EB.Debug.LogError("You should NOT have more than one AstarPath component in the scene at any time.\n" +
                "This can cause serious errors since the AstarPath component builds around a singleton pattern.");
        }

        //Disable GUILayout to gain some performance, it is not used in the OnGUI call
        useGUILayout = false;

        isEditor = Application.isEditor;

        // This class uses the [ExecuteInEditMode] attribute
        // So Awake is called even when not playing
        // Don't do anything when not in play mode
        if (!Application.isPlaying)
        {
            return;
        }

        if (OnAwakeSettings != null)
        {
            OnAwakeSettings();
        }

        //To make sure all graph modifiers have been enabled before scan (to avoid script run order issues)
        GraphModifier.FindAllModifiers();
        RelevantGraphSurface.FindAllGraphSurfaces();

        int numThreads = CalculateThreadCount(threadCount);


        threads = new Thread[numThreads];

        //Thread info, will contain at least one item since the coroutine "thread" is thought of as a real thread in this case
        threadInfos = new PathThreadInfo[System.Math.Max(numThreads, 1)];

        //Set up path queue with the specified number of receivers
        pathQueue = new ThreadControlQueue(threadInfos.Length);

        for (int i = 0; i < threadInfos.Length; i++)
        {
            threadInfos[i] = new PathThreadInfo(i, this, new PathHandler(i, threadInfos.Length));
        }

        //Start coroutine if not using multithreading
        if (numThreads == 0)
        {
            threadEnumerator = CalculatePaths(threadInfos[0]);
        }
        else {
            threadEnumerator = null;
        }

#if !UNITY_WEBGL
        for (int i = 0; i < threads.Length; i++)
        {
            threads[i] = new Thread(new ParameterizedThreadStart(CalculatePathsThreaded));
            threads[i].Name = "Pathfinding Thread " + i;
            threads[i].IsBackground = true;
        }

        //Start pathfinding threads
        for (int i = 0; i < threads.Length; i++)
        {
            if (logPathResults == PathLog.Heavy)
                EB.Debug.Log("Starting pathfinding thread {0}", i);
            threads[i].Start(threadInfos[i]);
        }

        if (numThreads != 0)
        {
            graphUpdateThread = new Thread(new ParameterizedThreadStart(ProcessGraphUpdatesAsync));
            graphUpdateThread.IsBackground = true;

            // Set the thread priority for graph updates
            // Unless compiling for windows store or windows phone which does not support it
#if !UNITY_WINRT
            graphUpdateThread.Priority = System.Threading.ThreadPriority.Lowest;
#endif
            graphUpdateThread.Start(this);
        }
#endif

        Initialize();

        // Flush work items, possibly added in initialize to load graph data
        FlushWorkItems();

        euclideanEmbedding.dirty = true;

#if BNICKSON_UPDATED
        // added !AStarPathfindingUtils.WillNavMeshBeBuiltFromInputMeshOnLevelBegin()
        if (scanOnStartup && !AStarPathfindingUtils.WillNavMeshBeBuiltFromInputMeshOnLevelBegin())
#else
        if (scanOnStartup)
#endif
        {
            if (!astarData.cacheStartup || astarData.file_cachedStartup == null)
            {
                Scan();

#if BNICKSON_UPDATED
                EventManager.instance.Raise(new NavMeshScanEvent());
#endif
            }
        }
    }

    void OnDisable()
    {
        EventManager.instance.RemoveListener<PopulateZonesPlayingEvent>(OnPopulateZones);
    }

    #endregion
}

#endif
