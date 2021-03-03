///////////////////////////////////////////////////////////////////////
//
//  GameStateManager.cs
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
using System.Reflection;
using System.IO;

public class GameStateManager : MonoBehaviour
{
	public static void Initialize()
	{
		if (null == sInstance)
		{
            GameObject sGameObject = new GameObject("GameStateManager");
			DontDestroyOnLoad(sGameObject);
			sInstance = sGameObject.AddComponent<GameStateManager>();
		}
	}
    
	private static GameStateManager sInstance = null;
	public static GameStateManager Instance
	{
		get
        {
            if (sInstance == null)
            {
                Initialize();
            }
            return sInstance;
        }
	}
    
	public CodeObjectManager ObjectManager
	{
		get; set;
	}

	private Dictionary<eGameState, System.Type> _gameStateTypes;

    private eGameState _eState;
    public eGameState State
	{
		get { return _eState; }
	}

    private GameState _state;
    public T GetGameState<T>() where T : GameState
	{
		return _state as T;
	}

    private List<GameState> mNewStateObjs = new List<GameState>();
    private List<GameState> moldStateObjs = new List<GameState>();
    private int mCurrentRunStateTime = 1;
    private int mTotalRunStateTime = 1;

    private float timer = 0;

    public void SetGameState<T>(System.Action<T> transition = null) where T : GameState
	{
		T newStateObj = typeof(T).GetConstructor (System.Type.EmptyTypes).Invoke (null) as T;
		GameStateAttribute attr = typeof(T).GetCustomAttributes (typeof(GameStateAttribute), false) [0] as GameStateAttribute;
		eGameState newState = attr.State;

		StopAllCoroutines();

		if (transition != null)
		{
			transition(newStateObj);
		}

		GameState oldStateObj = _state;
		if (null != oldStateObj)
		{
			oldStateObj.End(newStateObj);
		}
		eGameState previousState = _eState;

		_eState = newState;
		_state = newStateObj;

		PerformanceManager.Instance.OnGameStateTransition(newState);
		if (ObjectManager != null)
		{
			ObjectManager.End();
		}
		ObjectManager = new CodeObjectManager();
		ObjectManager.Start();

        mNewStateObjs.Add(newStateObj);
        moldStateObjs.Add(oldStateObj);
        EventManager.instance.Raise(new GameStateChangedEvent(previousState, newState));
	}

	private void Awake()
	{
        //音频内存管理器
        //LTAudio.ClearAudio.v_Instance.F_Start();
        //相机自适应
        //CameraUtils.F_Adapt();

        if (null == _gameStateTypes)
		{
			_gameStateTypes = new Dictionary<eGameState, System.Type>();
			Assembly assembly = Assembly.GetAssembly (typeof(GameState));
			foreach (System.Type t in assembly.GetTypes())
			{
				if (t.IsSubclassOf(typeof(GameState)))
				{
					GameStateAttribute attr = (GameStateAttribute)t.GetCustomAttributes (typeof(GameStateAttribute), false) [0];
					_gameStateTypes.Add(attr.State, t);
				}
			}
		}

#if !DEBUG || NO_DEBUG_SCREEN
		SetGameState<GameStateDownload>();
#else
		SetGameState<GameStateDebugStartScreen> ();		
#endif
	}

    private void Update()
    {
        if (_state!=null)
        {
            _state.Update();
            timer = 0;
        }
        else
        {
            if (timer < 3)
            {
                timer += Time.deltaTime;
            }
            else
            {
                Debug.LogError("_state == null");
                timer = 0;
            }
        }

        if (mNewStateObjs.Count > 0)
        {
            mCurrentRunStateTime += 1;
            if (mCurrentRunStateTime >= mTotalRunStateTime)
            {
                StartCoroutine(mNewStateObjs[0].Start(moldStateObjs[0]));
                mNewStateObjs.RemoveAt(0);
                moldStateObjs.RemoveAt(0);
                mCurrentRunStateTime = 0;
            }
        }
    }
    
	public void OnPause(bool pauseStatus)
	{
		_state.OnPause(pauseStatus);
	}

	public void OnFocus(bool focusStatus)
	{
		_state.OnFocus(focusStatus);
	}
}
