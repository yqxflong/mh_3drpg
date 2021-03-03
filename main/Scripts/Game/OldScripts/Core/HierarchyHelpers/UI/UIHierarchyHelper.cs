///////////////////////////////////////////////////////////////////////
//
//  UIHierarchyHelper.cs
//
//  Copyright (c) 2006-2014 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class UIHierarchyHelper : MonoBehaviour
{
    public static UIHierarchyHelper Instance
    {
        get;
        private set;
    }

    public enum eUIType
    {
        HUD_Static,
        NewPanel,
        None,
        HUD_Dynamic,
    }

    [System.Flags]
    public enum eContainers
    {
        HUD_Static = 1 << 0,
        HUD_Dynamic = 1 << 1,
    }

    public int rootMinimumHeight = 1356;
    public int rootMaximumHeight = 1536;

    public GameObject hudStaticContainer;
    public GameObject hudDynamicContainer;

    public UICamera MainUICamera;

    private Dictionary<eContainers, bool> _containersShown = new Dictionary<eContainers, bool>(); // whether the container was shown regardless of stacking

    private Dictionary<int, Transform> _staticHudAnchorMap = new Dictionary<int, Transform>();
    private Dictionary<int, Transform> _dynamicHudAnchorMap = new Dictionary<int, Transform>();

    //private Dictionary<eScreens, GameObject> _screenToGO = new Dictionary<eScreens, GameObject>();

    private Transform _staticHudNoAnchor;
    private Transform _dynamicHudNoAnchor;
    private BlockPanelController _blockPanel;

    public void ShowContainers(eContainers containerTypes, bool show, float alphaFadeTime)
    {
        if (show && UIStack.Instance.IsUIBlockingScreen)
        {
            return;
        }

        List<GameObject> containers = new List<GameObject>();

        if ((containerTypes & eContainers.HUD_Dynamic) != 0)
        {
            containers.Add(hudDynamicContainer);
        }
        if ((containerTypes & eContainers.HUD_Static) != 0)
        {
            containers.Add(hudStaticContainer);
        }

        for (int i = 0; i < containers.Count; i++)
        {
            if (alphaFadeTime > 0.0f)
            {
                UITweener tweener = containers[i].GetComponent<UITweener>();
                tweener.duration = alphaFadeTime;

                if (show)
                {
                    tweener.PlayReverse();
                }
                else
                {
                    tweener.PlayForward();
                }
            }
            else
            {
                UIPanel panel = containers[i].GetComponent<UIPanel>();
                panel.alpha = show ? 1.0f : 0.0f;
            }
        }
    }

    public void ShowRegularHUD(bool show, float alphaFadeTime = 1.0f)
    {
        _containersShown[eContainers.HUD_Dynamic] = show;
        _containersShown[eContainers.HUD_Static] = show;
        ShowContainers(eContainers.HUD_Static | eContainers.HUD_Dynamic, show, alphaFadeTime);
    }

    #region LoadAndPlace
    [System.Obsolete("同步接口，无法读取AB资源，请尽早使用异步！")]
    public GameObject LoadAndPlace(string resourcePath, eUIType type, UIAnchor.Side? side, bool forced = false)
    {
        if (!forced && UIStack.Instance.IsLoadingScreenUp || !forced && UIStack.Instance.IsWaittingScreenUp)
        {
            return null;
        }

        return LoadAndPlace(resourcePath, ResolvePosition(type, side));
    }

    private GameObject LoadAndPlace(string resourcePath, Transform parent)
    {
        GameObject prefab = EB.Assets.Load(resourcePath) as GameObject;

        if (prefab == null)
        {
            DebugSystem.LogWarning("No such prefab in Resources.");
            return null;
        }

        return JustPlace(prefab, parent);
    }

    public void LoadAndPlaceAsync(System.Action<GameObject> fn, string resourcePath, eUIType type, UIAnchor.Side? side, bool forced = false)
    {
        if (!forced && UIStack.Instance.IsLoadingScreenUp || !forced && UIStack.Instance.IsWaittingScreenUp)
        {
            fn?.Invoke(null);
        }
        else
        {
            _LoadAndPlaceAsync(resourcePath, ResolvePosition(type, side), fn);
        }
    }

    private void _LoadAndPlaceAsync(string resourcePath, Transform parent, System.Action<GameObject> fn)
    {
        EB.Assets.LoadAsync(resourcePath, typeof(GameObject), o=>
        {
            if(o){
                GameObject go = o as GameObject;
                if (go == null)
                {
                    DebugSystem.LogWarning("No such prefab in Resources.");
                    fn?.Invoke(null);
                }
                else
                {
                    fn?.Invoke(JustPlace(go, parent));
                }
            }
        });
    }
    #endregion

    #region Place
    private GameObject JustPlace(GameObject prefab, Transform parent)
    {
        if (prefab != null)
        {
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            Place(go, parent);
            return go;
        }
        return null;
    }

    public void Place(GameObject instance, eUIType type, UIAnchor.Side? side)
    {
        Place(instance, ResolvePosition(type, side));
    }

    public void Place(GameObject instance, eUIType type, UIAnchor.Side side)
    {
        Place(instance, ResolvePosition(type, side));
    }

    private void Place(GameObject instance, Transform parent)
    {
        if (parent != null && instance != null)
        {
            AttachTo(instance.transform, parent);
        }

        if (instance != null)
        {
            OnLoadAndPlace(instance);
        }
    }
    #endregion

    private void AttachTo(Transform toAttach, Transform parent)
    {
        Vector3 localPosition = toAttach.localPosition;
        Vector3 localScale = toAttach.localScale;

        toAttach.SetParent(parent);

        toAttach.localPosition = localPosition;
        toAttach.localScale = localScale;
    }

    public void SetBlockPanel(bool isPanelOn)
    {
        if (isPanelOn)
        {
            InputBlockerManager.Instance.Block(InputBlockReason.SET_BLOCK_PANEL_ON, 0.3f);
        }
        else
        {
            InputBlockerManager.Instance.UnBlock(InputBlockReason.SET_BLOCK_PANEL_ON);
        }

        CheckBlockPanel();

        if (null != _blockPanel)
        {
            if (isPanelOn)
            {
                _blockPanel.GetComponent<BlockPanelController>().Open();
            }
            else
            {
                _blockPanel.GetComponent<BlockPanelController>().Close();
            }
        }
    }

    private void CheckBlockPanel()
    {
        if (_blockPanel == null)
        {
            _blockPanel = transform.GetComponentInChildren<BlockPanelController>();
        }
    }

    public bool IsBlockPanelOn()
    {
        CheckBlockPanel();

        if (null != _blockPanel)
        {
            return _blockPanel.gameObject.activeSelf;
        }

        return false;
    }

    protected void OnLoadAndPlace(GameObject placed)
    {
        if (placed == null)
        {
            return;
        }

        IStackableUI stackable = (IStackableUI)placed.GetComponent(typeof(IStackableUI));
        if (stackable != null && stackable.EnstackOnCreate)
        {
            if (UIStack.Instance.IsLoadingScreenUp || UIStack.Instance.IsWaittingScreenUp)
            {
                UIStack.Queue(stackable);
            }
            else
            {
                UIStack.Open(stackable);
            }
        }
        SetHudShow();
    }

    public static void ReloadAllHierarchyHelpers()
    {
        if (Instance != null && Instance.enabled)
        {
            Instance.OnAboutToReload();
            Instance.OnReload();
        }
    }

    protected void OnAboutToReload()
    {
        UIStack.Instance.ClearEnstackQueue();
        UIStack.Instance.ExitStack(true);
    }

    private UIPanel mhudDynamicPanel = null;
    private UIPanel mhudStaticPanel = null;

    protected void OnReload()
    {
        if (mhudDynamicPanel == null)
        {
            mhudDynamicPanel = hudDynamicContainer.GetComponent<UIPanel>();
        }
        if (mhudStaticPanel == null)
        {
            mhudStaticPanel = hudStaticContainer.GetComponent<UIPanel>();
        }
        mhudDynamicPanel.RebuildAllDrawCalls();
        mhudStaticPanel.RebuildAllDrawCalls();

        mhudDynamicPanel.alpha = 1.0f;
        mhudStaticPanel.alpha = 1.0f;

        // the alpha of these panels, these values will be returned to after the HUD re-appears in the modal system
        _containersShown.Clear();
        _containersShown[eContainers.HUD_Dynamic] = true;
        _containersShown[eContainers.HUD_Static] = true;

        SetHudShow();
    }

    private void SetHudShow()
    {
        //�鿴�Ƿ�������ؽڵ�
        var item = _staticHudAnchorMap.GetEnumerator();
        while (item.MoveNext())
        {
            var monos = item.Current.Value.GetComponentsInChildren<MonoBehaviour>(true);
            bool isShow = monos != null && monos.Length > 1;
            item.Current.Value.gameObject.CustomSetActive(isShow);
        }
        var item2 = _dynamicHudAnchorMap.GetEnumerator();
        while (item2.MoveNext())
        {
            var monos = item2.Current.Value.GetComponentsInChildren<MonoBehaviour>(true);
            bool isShow = monos != null && monos.Length > 1;
            item2.Current.Value.gameObject.CustomSetActive(isShow);
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        float aspectRatio = Screen.width / (float)Screen.height;
        if (aspectRatio > 1.35f) //If using a phone, let's scale up the UI for fat fingers...
        {
            UIRoot root = GetComponent<UIRoot>();
            root.minimumHeight = rootMinimumHeight;
            root.maximumHeight = rootMaximumHeight;
        }

        DontDestroyOnLoad(gameObject);

        LoadAnchors(hudStaticContainer, _staticHudAnchorMap, ref _staticHudNoAnchor);
        LoadAnchors(hudDynamicContainer, _dynamicHudAnchorMap, ref _dynamicHudNoAnchor);

        SetHudShow();
        EventManager.instance.AddListener<CinematicEvent>(OnCinematicEvent);
    }

    private Transform ResolvePosition(eUIType uiType, UIAnchor.Side? side)
    {
        Transform result = null;
        switch (uiType)
        {
            case eUIType.HUD_Static:
                result = side.HasValue ? _staticHudAnchorMap[(int)side.Value] : _staticHudNoAnchor;
                break;
            case eUIType.NewPanel:
                AutoClosingContainer container = NGUITools.AddChild<AutoClosingContainer>(hudDynamicContainer.transform.parent.gameObject);
                if (side == null)
                {
                    result = container.transform;
                }
                else
                {
                    UIAnchor tempAnchor = NGUITools.AddChild<UIAnchor>(container.gameObject);
                    tempAnchor.side = side.Value;
                    tempAnchor.gameObject.AddComponent<AutoClosingContainer>();
                    tempAnchor.uiCamera = UICamera.mainCamera;
                    result = tempAnchor.transform;
                }
                break;
            case eUIType.None:
                if (hudDynamicContainer != null && hudDynamicContainer.transform != null) // Max_G: fix a crash on exit in combat
                {
                    result = hudDynamicContainer.transform.parent;
                }
                break;
            case eUIType.HUD_Dynamic:
                result = side.HasValue ? _dynamicHudAnchorMap[(int)side.Value] : _dynamicHudNoAnchor;
                break;
        }
        return result;
    }

    private void LoadAnchors(GameObject container, Dictionary<int, Transform> dic, ref Transform noAnchorGo)
    {
        if (container != null)
        {
            dic[(int)UIAnchor.Side.Bottom] = container.transform.Find("Anchor_Bottom");
            dic[(int)UIAnchor.Side.BottomLeft] = container.transform.Find("Anchor_BottomLeft");
            dic[(int)UIAnchor.Side.BottomRight] = container.transform.Find("Anchor_BottomRight");
            dic[(int)UIAnchor.Side.Center] = container.transform.Find("Anchor_Center");
            dic[(int)UIAnchor.Side.Left] = container.transform.Find("Anchor_SideLeft");
            dic[(int)UIAnchor.Side.Right] = container.transform.Find("Anchor_SideRight");
            dic[(int)UIAnchor.Side.Top] = container.transform.Find("Anchor_Top");
            dic[(int)UIAnchor.Side.TopLeft] = container.transform.Find("Anchor_TopLeft");
            dic[(int)UIAnchor.Side.TopRight] = container.transform.Find("Anchor_TopRight");

            noAnchorGo = container.transform.Find("NoAnchorHuds");
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        EventManager.instance.RemoveListener<CinematicEvent>(OnCinematicEvent);
    }

    private void OnCinematicEvent(CinematicEvent e)
    {
        if (e.GetSwitches().ShouldHideUIDuringCinematic())
        {
            if (e.GetCinematicEventType() == CinematicEvent.CinematicEventType.starting)
            {
                // If any UI screen is open, close it
                UIStack.Instance.ExitStack(true);
                ShowRegularHUD(false);
            }
            else if (e.GetCinematicEventType() == CinematicEvent.CinematicEventType.ending)
            {
                ShowRegularHUD(true);
            }
        }
    }
}
