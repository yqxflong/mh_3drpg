using System.Collections;
using System.Collections.Generic;
using Game.Tool;
using UnityEngine;

public class ClickFxPoolManager : MonoSingleton<ClickFxPoolManager> {

    public ParticleSystemUIComponent Fx;
    public int MaxFxCount = 3;
    private ParticleSystemUIComponent curFx;
    private int numTouches;

    private bool _shouldPlayFx = false;

    private void Awake()
    {
        MaxFxCount = Mathf.Clamp(MaxFxCount, 3, 5);
        for(int i = 0; i < MaxFxCount; i++)
        {
            ParticleSystemUIComponent newObj= GameObject.Instantiate(Fx, this.transform);
            newObj.gameObject.CustomSetActive(true);
            DisableFx(newObj);
            FxList.Add(newObj);
        }
    }

    private void DisableFx(ParticleSystemUIComponent fx)
    {
        fx.transform.position = Vector3.one * -999;
    }

    void OnDestroy()
    {
        for(int i = 0; i < FxList.Count; i++)
        {
            var obj = FxList[i];
            GameObject.Destroy(obj);
        }
        FxList.Clear();
    }

    private void Update()
    {
        if(_shouldPlayFx)
        {
            _shouldPlayFx = false;
            curFx.Play();
            return;
        }

        if (TouchController.Instance != null
            && UICamera.mainCamera != null)
        {
            numTouches = System.Math.Min(TouchController.Instance.ActiveTouches.Count, 2);
            for (int i = 0; i < numTouches; i++)
            {
                if (TouchController.Instance.ActiveTouches[i].phase == TouchPhase.Began)
                {
                    curFx = GetFxObj();
                    curFx.transform.position = UICamera.mainCamera.ScreenToWorldPoint(TouchController.Instance.ActiveTouches[i].position);
                    _shouldPlayFx = true;
                    break;
                }
            }
        }
    }
    
    private void OnTouchStartEvent(TouchStartEvent evt)
    {
        curFx = GetFxObj();
        if (UICamera.mainCamera != null)
        {
            curFx.transform.position = UICamera.mainCamera.ScreenToWorldPoint(evt.screenPosition);
            curFx.gameObject.CustomSetActive(true);
            curFx.Play();
        }
    }
    
    private List<ParticleSystemUIComponent> FxList = new List<ParticleSystemUIComponent>();
    private int index = 0;
    private ParticleSystemUIComponent GetFxObj()
    {
        index++;
        if (index >= MaxFxCount) index = 0;
        return FxList[index];
    }

    public void StopAll()
    {
		FxList.ForEach(fx => { if(fx != null)fx.Stop(); });
    }
}
