using UnityEngine;
using System.Collections;
using Boo.Lang;

public class ParticleSystemUIComponent : MonoBehaviour
{
    public ParticleSystem fx;
    public ParticleSystemScalingMode ScaleMode = ParticleSystemScalingMode.Local;
    public bool needFXScaleMode = false;
    public bool playOnAwake;
	public bool playOnStart;
	public bool playOnEnable;
	public bool playOnVisible;
	public bool stopOnDisable;  //disable的時是否Stop，ondestory必stop
	public bool destroyOnStop;  //stop時是否刪除自身
	public bool stopOnDiaphanous; //panel.alpha = 0,时是否stop
	public int sortingOrderOffset;
	public UIPanel panel;
	public UIPanel VisibleSyncPanel;
    /// <summary>
    /// 是否根据屏幕缩放，与needFXScaleMode配合使用
    /// </summary>
	public bool scaleByAspect = false;
	public Vector3 position = Vector3.zero;
	public Vector3 rotation = Vector3.zero;
    public float scale = 1;
	public bool isPlaceOutside;
	public float playTime;
	public int WaitFrame;

	private ParticleSystem mFX = null;
	private int mOriginSortingOrder = 0;
	private ParticleSystemRenderer[] _particleSystemRenderers;
	private ParticleSystem[] _particleSystems;

	private int mWaitFrame = -1;

	public bool IsPlaying()
	{
		return mFX != null && mFX.isPlaying;
	}

	public void Play(bool clear = true)
	{
		if (mFX != null && clear)
		{
			Stop();
		}

		if (fx == null)
		{
			EB.Debug.LogWarning("ParticleSystemUIComponent's fx is null,Please check it name——{0}",this.gameObject .name);
			return;
		}

		if (mFX == null)
        {
			ClearAllMats();
			mFX = Instantiate(fx);
			_particleSystemRenderers = mFX.gameObject.GetComponentsInChildren<ParticleSystemRenderer>(true);
			_particleSystems = mFX.gameObject.GetComponentsInChildren<ParticleSystem>(true);
		}

		if (mFX != null)
		{
			ParticleSystem[] ps = mFX.GetComponentsInChildren<ParticleSystem>();

			if (needFXScaleMode && ps != null)
			{
				for (int i = 0; i < ps.Length; ++i)
				{
					var main = ps[i].main;
					main.scalingMode = ScaleMode;
				}
			}

			if (!isPlaceOutside)
			{
				mFX.transform.SetParent(transform);
				mFX.transform.localScale = fx.transform.localScale;
			}

			if (scaleByAspect)
			{
				Vector2 screenSize = NGUITools.screenSize;
				float screenScale = 1f;
				if ((screenSize.x / screenSize.y) < (16f / 9f))
				{
					screenScale = screenSize.y / screenSize.x * (16.0f / 9.0f); // base aspect is iphone 16 : 9
				}
				else if ((screenSize.x / screenSize.y) > (16f / 9f))
				{
					screenScale = screenSize.x / screenSize.y * (9.0f / 16.0f); // base aspect is iphone 16 : 9
				}
				if (needFXScaleMode)
				{
					mFX.transform.localScale = screenScale * fx.transform.localScale;
				}
				else
				{
					for (int i = 0; i < ps.Length; ++i)
					{
						if (ps[i] != mFX)
						{
							ps[i].transform.localScale *= screenScale;
						}
					}
				}
			}

			if (isPlaceOutside)
			{
				mFX.transform.position = transform.position;
			}
			else
			{
				mFX.transform.localPosition = position;
			}

			mFX.transform.localEulerAngles = rotation;
			SetScale();
			NGUITools.SetLayer(mFX.gameObject, gameObject.layer);
			SetSortingOrder sso = mFX.gameObject.GetComponent<SetSortingOrder>();

			if (sso == null)
			{
				sso = mFX.gameObject.AddComponent<SetSortingOrder>();
			}

			mOriginSortingOrder = sso.OriginSortingOrder;
			sso.SortingOrder = panel.sortingOrder + sortingOrderOffset;

			//mFX.EnableEmission(true);
			//mFX.Simulate(0.0001f, true, true);

			if (WaitFrame == 0)
			{
				mFX.gameObject.CustomSetActive(true);
				PlayFx();// mFX.Play(true);
			}
			else
			{
				mFX.Stop(true);

				//if (mTimer == 0)
				//{
				//	mTimer = TimerManager.instance.AddFramer(WaitFrame, 1, FxPlayTimer);
				//}
				mWaitFrame = WaitFrame;
			}

			if (playTime > 0)
			{
				Invoke("OnPlayEnd", playTime);
			}
		}
		else
		{
			EB.Debug.LogError("ParticleSystemReferenceComponent.Play: fx not found, {0}", fx.name);
		}
	}

    private void FxPlayTimer()
    {
	    if (mFX == null)
	    {
		    StopTimer();
		    return;
	    }

		mWaitFrame = -1;

		if (!mFX.gameObject.activeSelf)
		{
			mFX.gameObject.CustomSetActive(true);
		}

		PlayFx();// mFX.Play(true);

		if (transform.GetComponent<EffectClip>() != null)
		{
			transform.GetComponent<EffectClip>().Init();
		}
	}

    private void StopTimer()
    {
		//if (mTimer > 0)
		//{
		//    TimerManager.instance.RemoveTimer(mTimer);
		//    mTimer = 0;
		//}
		mWaitFrame = -1;
	}

    private void SetScale()
    {
        if (needFXScaleMode)
        {
            mFX.transform.localScale *= scale;
        }
        else if (_particleSystems != null)
        {
			var len = _particleSystems.Length;

			for (int i = 0; i < len; i++)
            {
				var t = _particleSystems[i].transform;
				t.localScale = scale * t.localScale;
            }
        }
    }

	public Coroutine Wait()
	{
		Play();
		return StartCoroutine(WaitCoroutine());
	}

	public Coroutine WaitDuration(float inc)
	{
		Play();
		return StartCoroutine(WaitDurationCoroutin(inc));
	}

	private IEnumerator WaitCoroutine()
	{
		bool finish = mFX == null || !mFX.isPlaying || mFX.isStopped || !mFX.IsAlive(true);

		while (!finish)
		{
			yield return null;
			finish = mFX == null || !mFX.isPlaying || mFX.isStopped || !mFX.IsAlive(true);
		}
	}

	private IEnumerator WaitDurationCoroutin(float inc)
	{
		float duration = mFX != null ? mFX.duration + inc : 0.0f;
		yield return new WaitForSeconds(duration);
	}

	private void PlayFx()
	{
		mFX.Play(true);

		if (_particleSystems != null)
		{
			int len = _particleSystems.Length;
			ParticleSystem ps;
			ParticleSystemRenderer psr;

			for (var i = 0; i < len; i++)
			{
				ps = _particleSystems[i];
				psr = ps.gameObject.GetComponent<ParticleSystemRenderer>();

				if (ps.emission.enabled == false)
				{
					ClearMat(psr);
				}
				else
				{
					ClearMat(psr, true);
				}
			}
		}
	}

	private void ClearMat(ParticleSystemRenderer psr, bool ignoreFirstMaterial = false)
	{
		if (psr == null)
		{
			return;
		}

		for (var i = 0; i < psr.materials.Length; i++)
		{ 
			if (ignoreFirstMaterial && i == 0)
			{
				continue;
			}

			Destroy(psr.materials[i]); 
		}
	}

	private void ClearAllMats()
	{
		if (_particleSystemRenderers != null)
		{
			int len = _particleSystemRenderers.Length;

			for (var i = len - 1; i >= 0; i--)
			{
				ClearMat(_particleSystemRenderers[i]);
			}

			_particleSystemRenderers = null;
		}
	}

	public void Stop()
	{		
        StopTimer();

		if (mFX != null)
		{
			mFX.EnableEmission(false);
			mFX.Stop(true);
			mFX.Clear(true);
			mFX.StopAll(true);
			Animator[] animators = mFX.GetComponentsInChildren<Animator>(true);

			if (animators != null)
			{
				for (int i = 0; i < animators.Length; i++)
				{
					Animator anim = animators[i];
					anim.gameObject.CustomSetActive(false);
				}
			}

			Animation[] animations = mFX.GetComponentsInChildren<Animation>(true);

			if (animators != null)
			{
				for (int i = 0; i < animations.Length; i++)
				{
					Animation anim = animations[i];
					anim.gameObject.CustomSetActive(false);
				}
			}

            SetSortingOrder sso = mFX.GetComponent<SetSortingOrder>();

            if (sso != null)
            {
                sso.SetLayer(mOriginSortingOrder);
                Destroy(sso);
            }

			ClearAllMats();
			Destroy(mFX.gameObject);
			mFX = null;
		}

		if (destroyOnStop)
		{
			Destroy(gameObject);
		}
	}

	public void Pause()
	{
		StopTimer();

		if (mFX != null)
		{
			mFX.gameObject.SetActive(false);
		}
	}

	void OnPlayEnd()
	{
		destroyOnStop = true;
		Stop();
	}

	void Awake()
	{
		if (playOnAwake && mFX == null)
		{
			Play();
		}
	}

	void Start()
	{
		if (playOnStart && mFX == null)
		{
			Play();
		}
	}

	void OnDestroy()
	{
		Stop();

		if (mFX != null)
		{
			Destroy(mFX);
			mFX = null;
		}
	}
    
	void OnEnable()
	{
		if (mFX != null && mFX.isPaused)
		{
			if (WaitFrame == 0)
			{
				PlayFx();// mFX.Play(true); 
			}
			else
			{
				//if (mTimer == 0) mTimer = TimerManager.instance.AddFramer(WaitFrame, 1, FxPlayTimer);
				if (mWaitFrame == -1) mWaitFrame = WaitFrame;
			}
		}
		else if (mFX == null && playOnEnable)
		{
			try
			{
				Play();
			}
			catch(System.NullReferenceException e)
			{
				EB.Debug.LogError(e.ToString());
			}

            if(transform.GetComponent<EffectClip>() != null)
            {
                transform.GetComponent<EffectClip>().Init();
            }
		}
	}

	void OnDisable()
	{
		if (mFX != null && stopOnDisable)
		{
			Stop();
		}
		else if (mFX != null && mFX.isPlaying)
		{
			mFX.Pause(true);
		}
	}

	void Update()
	{
		if ( mFX != null )
		{
			#region 延時播放特效
			if(mWaitFrame > 0)
            {
				mWaitFrame -= 1;
				if(mWaitFrame == 0)
                {
					FxPlayTimer();
					mWaitFrame = -1;
                }
			}
			else
            {
				if (!mFX.main.loop && !mFX.isPlaying || mFX.isStopped || !mFX.IsAlive(true))
				{
					ClearAllMats();
					Destroy(mFX.gameObject);
					mFX = null;
					return;
				}
			}
            #endregion


			if (stopOnDiaphanous && VisibleSyncPanel != null && IsPlaying() && !mFX.isStopped && mFX.IsAlive(true))
			{
				if (VisibleSyncPanel.alpha <= 0.001f)
				{
					Stop();
				}
			}
		}
		else
		{
			if (playOnVisible && VisibleSyncPanel != null && !IsPlaying())
			{
				if (VisibleSyncPanel.alpha >= 0.998f)
				{
					Play();
					EffectClip clip = transform.GetComponent<EffectClip>();

					if (clip != null && !clip.HasInitialized)
					{
						clip.Init();
					}
				}
			}
		}
	}
}