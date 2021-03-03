///////////////////////////////////////////////////////////////////////
//
//  UILoadingSpinner.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class UILoadingSpinner : MonoBehaviour
{
    public WaitForSeconds delayTime = new WaitForSeconds(0.25f);
    public bool hasOverTime = true;
    public float overTimer = 10;//ÉèÖÃ³¬Ê±·À¿¨ËÀ
    private bool _hasStarted = false;
    private bool _shouldFadeIn = false;

    public void Show()
    {
        StartCoroutine(OnAddToStack());
    }

    private float timer = 0;
    public void Update()
    {
        if (_hasStarted&& hasOverTime)
        {
            timer += Time.deltaTime;
            if(timer> overTimer)
            {
                EB.Debug.LogError("UILoadingSpinner is TimeOut.Hide!");
                LoadingSpinner.Hide();
                timer = 0;
            }
        }
    }

    public void Hide()
    {
        StartCoroutine(OnRemoveFromStack());
    }

	private IEnumerator OnAddToStack()
	{
		_shouldFadeIn = true;
        yield return delayTime;
        if (_shouldFadeIn)
        {
            UITweenActivator tweener = new UITweenActivator(gameObject, 1);
            tweener.PlayForward();

            _shouldFadeIn = false;
            _hasStarted = true;
            timer = 0;
        }
	}



	private IEnumerator OnRemoveFromStack()
	{
		_shouldFadeIn = false;
		if (_hasStarted)
		{
			UITweenActivator tweener = new UITweenActivator(gameObject, 1);
			tweener.PlayReverse();
			_hasStarted = false;

			bool isFinished = false;
			tweener.onFinished += delegate ()
			{
				isFinished = true;
			};

			while (!isFinished)
			{
				yield return null;
			}
		}
        StopAllCoroutines();
        GameObject.Destroy(gameObject);
    }
    
}

public static class LoadingSpinner
{
	private static int _numLoadingSpinners = 0;
	private static UILoadingSpinner _ui = null;
    private static GameObject loadingWindow;

	public static void Init()
	{
		EB.Sparx.HttpEndPoint.RegisterHandler("ClearQueueAndBackUpDeep", Destroy);
		UnInit();
		UnInit();
	}

	public static void UnInit()
	{
		EB.Sparx.HttpEndPoint.UnRegisterHandler("ClearQueueAndBackUpDeep", Destroy);
	}

    public static void Show()
	{
		if (_numLoadingSpinners == 0 && _ui == null)
		{
            if (loadingWindow == null)
            {
                string windowPrefab = "_GameAssets/Res/Prefabs/UIPrefabs/LTLoading/UI_LoadingSpinner";
                UIHierarchyHelper.Instance.LoadAndPlaceAsync(go=>
				{
					loadingWindow = go;
					_ui = loadingWindow.GetComponent<UILoadingSpinner>();
                    _ui.Show();
                }, windowPrefab, UIHierarchyHelper.eUIType.None, null, true);
            }
		}
		_numLoadingSpinners++;
	}

	public static void Hide()
	{
		_numLoadingSpinners--;

		if (_numLoadingSpinners < 0)
		{
			EB.Debug.LogWarning("LoadingSpinner Hide too many times");
			_numLoadingSpinners = 0;
			return;
		}

		if (_numLoadingSpinners == 0 && _ui != null)
		{
            _ui.Hide();
        }
	}

	public static void Destroy()
	{
		if (_numLoadingSpinners > 0)
		{
			_numLoadingSpinners = 0;
			if (_ui != null)
			{
                _ui.Hide();
            }
		}
	}
}