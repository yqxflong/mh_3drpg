using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotfixTool
{
    //#泛型对象 IL2CPP不支持动态创建 所以热更代码里特殊使用的在此 也可以通过绑定Type泛型
    public static List<UIControllerILR> listILRController;
    public static List<UIControllerILRObject> listIRLObject;
    //public static List<MainlandsGhostRewardTemplate> listGhostReward; //TODOX


    /// <summary>
    /// 此方法不需要被调用 只是为了告诉IL2CPP哪些泛型是要预生成的 
    /// </summary>
    public static void InitIL2CPP()
    {
        //#泛型方法  告诉  不用调用 只是告诉L2CPP这些是要泛型函数需要衍生的类型
        GameObject gameObject = new GameObject();
        gameObject.GetComponent<GameObject>();
        gameObject.GetComponentInChildren<GameObject>();
        gameObject.GetComponentsInChildren<GameObject>();
        gameObject.GetComponent<Transform>();
        gameObject.GetComponentInChildren<Transform>();
        gameObject.GetComponentsInChildren<Transform>();
        gameObject.GetComponent<UIRoot>();
        gameObject.GetComponentInChildren<UIRoot>();
        gameObject.GetComponentsInChildren<UIRoot>();
        gameObject.GetComponent<UIRect>();
        gameObject.GetComponentInChildren<UIRect>();
        gameObject.GetComponentsInChildren<UIRect>();
        gameObject.GetComponent<UIWidget>();
        gameObject.GetComponentInChildren<UIWidget>();
        gameObject.GetComponentsInChildren<UIWidget>();
        gameObject.GetComponent<UILabel>();
        gameObject.GetComponentInChildren<UILabel>();
        gameObject.GetComponentsInChildren<UILabel>();
        gameObject.GetComponent<UISprite>();
        gameObject.GetComponentInChildren<UISprite>();
        gameObject.GetComponentsInChildren<UISprite>();
        gameObject.GetComponent<UIButton>();
        gameObject.GetComponentInChildren<UIButton>();
        gameObject.GetComponentsInChildren<UIButton>();
        gameObject.GetComponent<UITexture>();
        gameObject.GetComponentInChildren<UITexture>();
        gameObject.GetComponentsInChildren<UITexture>();
        gameObject.GetComponent<UIGrid>();
        gameObject.GetComponentInChildren<UIGrid>();
        gameObject.GetComponentsInChildren<UIGrid>();
        gameObject.GetComponent<UIScrollView>();
        gameObject.GetComponentInChildren<UIScrollView>();
        gameObject.GetComponentsInChildren<UIScrollView>();
        gameObject.GetComponent<UIScrollBar>();
        gameObject.GetComponentInChildren<UIScrollBar>();
        gameObject.GetComponentsInChildren<UIScrollBar>();
        gameObject.GetComponent<UIProgressBar>();
        gameObject.GetComponentInChildren<UIProgressBar>();
        gameObject.GetComponentsInChildren<UIProgressBar>();
        gameObject.GetComponent<UIToggle>();
        gameObject.GetComponentInChildren<UIToggle>();
        gameObject.GetComponentsInChildren<UIToggle>();
        gameObject.GetComponent<UIControllerILR>();
        gameObject.GetComponentInChildren<UIControllerILR>();
        gameObject.GetComponentsInChildren<UIControllerILR>();

        AddComponent<UIControllerILR>(gameObject);

        GlobalUtils.SetNumTemplate<Transform>(gameObject.transform, new List<Transform>(), 1, 1,false);
        GlobalUtils.SetNumTemplate<UIRect>(gameObject.AddComponent<UIRect>(), new List<UIRect>(), 1, 1, false);
        GlobalUtils.SetNumTemplate<UIWidget>(gameObject.AddComponent<UIWidget>(), new List<UIWidget>(), 1, 1, false);
        GlobalUtils.SetNumTemplate<UIButton>(gameObject.AddComponent<UIButton>(), new List<UIButton>(), 1, 1, false);
        GlobalUtils.SetNumTemplate<UILabel>(gameObject.AddComponent<UILabel>(), new List<UILabel>(), 1, 1, false);
        GlobalUtils.SetNumTemplate<UISprite>(gameObject.AddComponent<UISprite>(), new List<UISprite>(), 1, 1, false);
        GlobalUtils.SetNumTemplate<UITexture>(gameObject.AddComponent<UITexture>(), new List<UITexture>(), 1, 1, false);
        GlobalUtils.SetNumTemplate<UIGrid>(gameObject.AddComponent<UIGrid>(), new List<UIGrid>(), 1, 1, false);
        GlobalUtils.SetNumTemplate<UIScrollView>(gameObject.AddComponent<UIScrollView>(), new List<UIScrollView>(), 1, 1, false);
        GlobalUtils.SetNumTemplate<UIScrollBar>(gameObject.AddComponent<UIScrollBar>(), new List<UIScrollBar>(), 1, 1, false);
        GlobalUtils.SetNumTemplate<UIProgressBar>(gameObject.AddComponent<UIProgressBar>(), new List<UIProgressBar>(), 1, 1, false);
        GlobalUtils.SetNumTemplate<UIToggle>(gameObject.AddComponent<UIToggle>(), new List<UIToggle>(), 1, 1, false);

    }
	public static T AddComponent<T>(GameObject go) where T : Component
    {
        return go.AddComponent<T>();
    }
}
