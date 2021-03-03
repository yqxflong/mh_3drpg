using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleRecycleInTime : MonoBehaviour
{

    public float RecycleTime;
    public string poolPsName;
    public ParticleSystem Particle;
	// Use this for initialization
	void OnEnable ()
	{
	    StartCoroutine(RecycleParticleInTime());
	}   

    IEnumerator RecycleParticleInTime()
    {
//        while(string.IsNullOrEmpty(poolPsName))
//        {
//            yield return null;
//        }
        /*
        PSPool particlePool = PSPoolManager.Instance.Find(poolPsName);
        EB.Debug.LogPSPoolAsset("<color=#00ff00>回收记时计到点</color>name:" + name + ",缓存有没有:" + (particlePool != null));
        yield return new WaitForSeconds(RecycleTime);
        EB.Debug.LogPSPoolAsset("<color=#FF00BA>回收的时间到了</color>name:" + name + ",缓存有没有:" + (particlePool != null));
        particlePool.PullToPool(Particle);
        */
        
        yield return new WaitForSeconds(RecycleTime);
        //EB.Debug.LogPSPoolAsset("<color=#FF00BA>回收的时间到了</color>name:" + name);
        PSPoolManager.Instance.Recycle(Particle);
        yield break;
    }
}
