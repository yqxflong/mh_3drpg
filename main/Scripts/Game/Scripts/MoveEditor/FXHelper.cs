using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class PSExtensionMethods
{
	public static void EnableEmission(this ParticleSystem ps, bool enable)
	{
		if(ps == null)
		{
			return;
		}
		
		ParticleSystem[] particleSystems = ps.GetComponentsInChildren<ParticleSystem>(true);
		for (int i = 0; i < particleSystems.Length; i++)
		{            
			//particleSystems[i].enableEmission = enable ? !ParticlePal.DisabledByParticlePal(particleSystems[i].gameObject) : false;
			var em = particleSystems[i].emission;

			bool isvisible = enable && !ParticlePal.DisabledByParticlePal(particleSystems[i].gameObject);
            if (em.enabled != isvisible)
            {
                em.enabled = isvisible;
            }

        }
	}
	
	public static bool IsEmissionEnabled(this ParticleSystem ps)
	{
		ParticleSystem[] particleSystems = ps.GetComponentsInChildren<ParticleSystem>(true);
		for (int i = 0; i < particleSystems.Length; i++)
		{
			if (!particleSystems[i].emission.enabled)
				return false;
		}
		return true;
	}

	public static void StopAll(this ParticleSystem ps, bool clear)
	{
        ParticleSystem[] particleSystems = ps.GetComponentsInChildren<ParticleSystem>(true);
		for (int i = 0; i < particleSystems.Length; i++)
		{
            particleSystems[i].Stop(false);
            if (clear)
			{
				particleSystems[i].Clear(false);
			}
		}
	}

	public static void FadeOut(this ParticleSystem ps, float timer)
	{
		ps.Stop();
		ParticleSystem[] psList = ps.GetComponentsInChildren<ParticleSystem>(true);

		for(int i = 0; i < psList.Length; i++)
		{
			ParticleSystem psChild = psList[i];
			ParticleSystem.Particle[] particles = new ParticleSystem.Particle[psChild.particleCount];
			
			//get the particles
			psChild.GetParticles(particles);
			
			//then iterate through the array changing the color 1 by 1
			for(int p = 0; p < particles.Length; p++)
			{
				if(particles[p].remainingLifetime > timer)
				{
					particles[p].remainingLifetime = timer;
				}
			}
			
			//set the particles back to the system
			psChild.SetParticles(particles, particles.Length);
		}
	}

	public static void TintColor(this ParticleSystem psObject, Color newColor)
	{
		ParticleSystem[] psList = psObject.GetComponentsInChildren<ParticleSystem>(true);
		
		for(int i = 0; i < psList.Length; i++)
		{
			ParticleSystem psChild = psList[i];
			psChild.startColor = newColor;
		}
	}
}

namespace MoveEditor
{
	public class FXHelper : MonoBehaviour
	{
		private class ParticleInfo
		{
			public ParticleSystem _particleSystem;
			public string _name;
			public bool _interruptable;
			public bool _stopOnOverride;
			public bool _stopOnExitAnim;
			public bool _stopOnEndTurn;
			public bool _stopOnDuration;
			public float _duration;
			public bool _stopAfterTurns;
			public int _turns;
			public float _activeTime;
			public float _activeTurn;
			public int _clipHash;
            public ParticleEventProperties properties;

        }

		private struct TrailInfo
		{
			public GameObject _trail;
			public TrailRendererInstance _trailInstance;
			public bool _interruptable;
			public int _clipHash;
			public string _name;
		}

		private struct LightInfo
		{
			public GameObject _light;
			public bool _interruptable;
			public bool _stopOnExitAnim;
			public int _clipHash;
			public string _name;
		}

		public class PersistentAudioInfo
		{
			public string _eventId;
			public bool _stopOnOverride;
			public bool _stopOnExitAnim;
			public int _clipHash;
			public bool _stopOnDuration;
			public float _duration;
			public float _activeTime;
			public bool _stopOnEndTurn;
			public bool _stopAfterTurns;
			public int _turns;
			public int _activeTurn;
			public bool _interruptable;
		}

		private class EnvLightingInfo
		{
			public EnvLightingInfo(EnvironmentLightingProperties properties, float duration)
			{
				_properties = properties;
				_duration = duration;
				_time = 0.0f;
			}

			public bool Update(float deltaTime)
			{
				_time += deltaTime * _properties._speed;

				if (_properties != null)
				{
					Color multiply = _properties._multiplyGradient.Evaluate(_time / _properties._multiplyDuration);
					Color add = _properties._addGradient.Evaluate(_time / _properties._addDuration);

					RenderGlobals.SetEnvironmentAdjustments(multiply, add);
				}

				return _time >= _duration;
			}

			private EnvironmentLightingProperties _properties = null;
			private float _duration = 0.0f;
			private float _time = 0.0f;
		}

		public Transform m_DefaultFXAttachment;
		public Transform m_HeadFXAttachment;
		public Transform m_ChestFXAttachment;
		public Transform m_FootFXAttahment;
		public Transform m_HealthBarFXAttachment;
		bool m_CanPlayParticle=true;
		public bool CanPlayParticle { set { m_CanPlayParticle = value; } }

        public System.Action<ParticleSystem> PlayParticleAction;
        
        private void Awake()
		{
			_animator = GetComponent<Animator>();
		}

		private void OnDisable()
		{
			if (_activeParticles.Count > 0)
			{
				foreach (var ps_info in _activeParticles)
				{
					EB.Debug.LogWarning("FXHelper.OnDisable: active particle = " + (ps_info._particleSystem != null ? ps_info._particleSystem.name : ps_info._name));
				}
			}
		}

		private void Start()
		{
			_currentAnimHash = GetCurrentAnimHash();
		}

		private void Update()
        {
            if (!GameEngine.Instance.IsTimeToRootScene)
            {
                return;
            }
            if (_activeEnvLightingInfo != null)
			{
				//if (!_player.IsFrozen())
				{
					if (_activeEnvLightingInfo.Update(Time.deltaTime))
					{
						_activeEnvLightingInfo = null;
					}
				}
			}

			if (_activePostFxEvents.Count > 0)
			{
				for (int i = _activePostFxEvents.Count - 1; i >= 0; i--)
				{
					if (_activePostFxEvents[i].Update(Time.deltaTime))
					{
						_activePostFxEvents.RemoveAt(i);
					}
				}
			}

			for (int i = _activeParticles.Count - 1; i >= 0; i--)
			{
				if (_activeParticles[i]._stopOnDuration &&
				   Time.realtimeSinceStartup - _activeParticles[i]._activeTime > _activeParticles[i]._duration)
				{
					StopParticleInternal(_activeParticles[i]);
				}
				else if (_activeParticles[i]._stopAfterTurns && GetCurrentTurn() - _activeParticles[i]._activeTurn >= _activeParticles[i]._turns)
				{
					StopParticleInternal(_activeParticles[i]);
				}
				else if (_activeParticles[i]._stopOnEndTurn && _activeParticles[i]._activeTurn != GetCurrentTurn())
				{
					StopParticleInternal(_activeParticles[i]);
				}
			}

			for (int i = _activePersistentAudioEvents.Count - 1; i >= 0; i--)
			{
				PersistentAudioInfo audio = _activePersistentAudioEvents[i];
				if (audio._stopOnDuration && Time.realtimeSinceStartup - audio._activeTime > audio._duration)
				{
					StopPersistentAudioInternal(audio);
				}
				else if (audio._stopAfterTurns && GetCurrentTurn() - audio._activeTurn >= audio._turns)
				{
					StopPersistentAudioInternal(audio);
				}
				else if (audio._stopOnEndTurn && audio._activeTurn != GetCurrentTurn())
				{
					StopPersistentAudioInternal(audio);
				}
			}
		}

		private void LateUpdate()
		{

            if (!GameEngine.Instance.IsTimeToRootScene)
            {
                return;
            }
            // If we've moved to a new animation state, check if we should interrupt particles
            HandleAnimationChange();

			for (int i = _activeParticles.Count - 1; i >= 0; i--)
			{
                if(_activeParticles[i]==null) //by pj ���ӶԿն�����Ƴ� ���ⱨ��Ӱ��������Ϊ
                {
                    _activeParticles.RemoveAt(i);
                }
                else if(_activeParticles[i]._particleSystem ==null)//by pj ���ӶԿն�����Ƴ� ���ⱨ��Ӱ��������Ϊ
                {
                    _activeParticles.RemoveAt(i);
                }
				else if (!_activeParticles[i]._particleSystem.isPlaying)
				{
					//EB.Debug.Log(string.Format("FXHelper.LateUpdate: {0} is not playing, remove", _activeParticles[i]._particleSystem.name));
					_activeParticles.RemoveAt(i);
				}
				else
				{
					//EB.Debug.Log(string.Format("FXHelper.LateUpdate: {0} is playing", _activeParticles[i]._particleSystem.name));
				}
			}

			for (int i = _activeTrailRenderers.Count - 1; i >= 0; i--)
			{
                if( _activeTrailRenderers[i]._trailInstance == null)
                {
                    _activeTrailRenderers.RemoveAt(i);
                }
				else if (!_activeTrailRenderers[i]._trailInstance.IsPlaying)
				{
					_activeTrailRenderers.RemoveAt(i);
				}
			}
		}

		private void HandleAnimationChange()
		{
            if (_activeParticles.Count > 0 || _activeDynamicLights.Count > 0 || _activePersistentAudioEvents.Count > 0)
            {
                int hash = GetCurrentAnimHash();

                if (hash != _currentAnimHash)
                {
                    _currentAnimHash = hash;


                    //by libin---这里实际不应该在动作结束后调用这句话，因为有可能特效还没播放，会显得不自然
                    for (int i = _activeParticles.Count - 1; i >= 0; i--)
                    {
                        //循环动作需要销毁，因为循环动作的特效就是靠动作结束来销毁的
                        if (_activeParticles[i]._particleSystem.main.loop)
                        {
                            if (_activeParticles[i]._stopOnExitAnim && _activeParticles[i]._clipHash != hash)
                            {
                                StopParticleInternal(_activeParticles[i], false, true);
                            }
                        }
                    }

                    for (int i = _activeDynamicLights.Count - 1; i >= 0; i--)
                    {
                        if (_activeDynamicLights[i]._stopOnExitAnim && _activeDynamicLights[i]._clipHash != hash)
                        {
                            StopDynamicLightInternal(_activeDynamicLights[i]);
                        }
                    }

                    for (int i = _activePersistentAudioEvents.Count - 1; i >= 0; i--)
                    {
                        if (_activePersistentAudioEvents[i]._stopOnExitAnim && _activePersistentAudioEvents[i]._clipHash != hash)
                        {
                            StopPersistentAudio(_activePersistentAudioEvents[i]._eventId);
                        }
                    }
                }
            }
        }

		public void Init(Hotfix_LT.Combat.Combatant player)
		{
			_player = player;
		}

        public void Flip(bool flipped)
		{
			_flipped = flipped;
		}

		public bool IsFlipped()
		{
			return _flipped;
		}

		public void ReportAttackResult(MoveEditor.AttackResult result)
		{
			switch (result)
			{
				case MoveEditor.AttackResult.Blocked:
					for (int i = 0; i < _cachedConditionalParticles.Count; i++)
					{
						if (!_cachedConditionalParticles[i]._killIfBlocked)
						{
							PlayParticle(_cachedConditionalParticles[i], true);
						}
					}
					for (int i = 0; i < _cachedConditionalAudioes.Count; i++)
					{
						if (!_cachedConditionalAudioes[i]._cancelIfBlocked)
						{
							PlayAudio(_cachedConditionalAudioes[i], true);
						}
					}
					break;
				case MoveEditor.AttackResult.Missed:
					for (int i = 0; i < _cachedConditionalParticles.Count; i++)
					{
						if (!_cachedConditionalParticles[i]._cancelIfMissed)
						{
							PlayParticle(_cachedConditionalParticles[i], true);
						}
					}
					for (int i = 0; i < _cachedConditionalAudioes.Count; i++)
					{
						if (!_cachedConditionalAudioes[i]._cancelIfMissed)
						{
							PlayAudio(_cachedConditionalAudioes[i], true);
						}
					}
					break;
				case MoveEditor.AttackResult.Hit:
					for (int i = 0; i < _cachedConditionalParticles.Count; i++)
					{
						if (!_cachedConditionalParticles[i]._cancelIfHit)
						{
							PlayParticle(_cachedConditionalParticles[i], true);
						}
					}
					for (int i = 0; i < _cachedConditionalAudioes.Count; i++)
					{
						if (!_cachedConditionalAudioes[i]._cancelIfHit)
						{
							PlayAudio(_cachedConditionalAudioes[i], true);
						}
					}
					break;
			}

			_cachedConditionalParticles.Clear();
			_cachedConditionalAudioes.Clear();
		}

		// Particles are interrupted when the player is hit
		public void InterruptFX()
		{
			for (int i = _activeParticles.Count - 1; i >= 0; i--)
			{
				if (_activeParticles[i]._interruptable)
					StopParticleInternal(_activeParticles[i]);
			}

			for (int i = _activeTrailRenderers.Count - 1; i >= 0; i--)
			{
				if (_activeTrailRenderers[i]._interruptable)
					StopTrailRendererInternal(_activeTrailRenderers[i]);
			}

			for (int i = _activeDynamicLights.Count - 1; i >= 0; i--)
			{
				if (_activeDynamicLights[i]._interruptable)
					StopDynamicLightInternal(_activeDynamicLights[i]);
			}

			for (int i = _activePersistentAudioEvents.Count - 1; i >= 0; i--)
			{
				if (_activePersistentAudioEvents[i]._interruptable)
					StopPersistentAudioInternal(_activePersistentAudioEvents[i]);
			}
		}

		public void StopAll(bool clearParticles = false) // [GM] : added clearParticles argument
		{
			for (int i = _activeParticles.Count - 1; i >= 0; i--)
			{
				StopParticleInternal(_activeParticles[i], clearParticles); // [GM] clearParticles
			}

			for (int i = _activeTrailRenderers.Count - 1; i >= 0; i--)
			{
				StopTrailRendererInternal(_activeTrailRenderers[i]);
			}

			for (int i = _activeDynamicLights.Count - 1; i >= 0; i--)
			{
				StopDynamicLightInternal(_activeDynamicLights[i]);
			}

			for (int i = _activePersistentAudioEvents.Count - 1; i >= 0; i--)
			{
				StopPersistentAudioInternal(_activePersistentAudioEvents[i]);
			}          
		}

		public void FreezeFX(bool freeze)
		{
			for (int i = 0; i < _activeTrailRenderers.Count; ++i)
			{
				FreezeTrailRendererInternal(_activeTrailRenderers[i], freeze);
			}
		}

		public ParticleSystem PlayParticle(ParticleEventProperties properties, bool forcePlay)
		{
			return PlayParticle(properties, 0, forcePlay);
		}

        private const int mReplayTime = 3;
        private int mCurrentReplayTime = 0;
        private int mTimer = -1;
        public ParticleSystem PlayParticle(ParticleEventProperties properties, float time = 0, bool forcePlay = false, bool target = false, Vector3 tarPos = new Vector3(), Animator tarAimator = null, Vector3 tarOrgPos = new Vector3(), Vector3 tarHitPos = new Vector3())
		{
			if(!m_CanPlayParticle)
			{
				return null;
			}

			//判断是否被禁止播放特效
            if(DisableFX)
            {
                return null;
            }

			if (properties._applyOnTargetList)
			{
				ParticleEventProperties clone = ParticleEventProperties.Deserialize(properties.Serialize());
				PlayParticleOnTargetList(clone);
				return null;
			}

			//在副本中，或者在赛跑活动中
			if ((GameFlowControlManager.IsInView("InstanceView") || GameFlowControlManager.Instance.InActivityRacing)
				&& properties._flippedParticleReference.Name.Contains("paobu"))
			{
                return null;
            }

			//如果是世界boss的封印特效，则必须要在主场景才会出现
			//ToDo:改调热更
			//if (properties._flippedParticleReference.Name.Contains("fx_f_M00") && (!_animator.name.Contains("-Variant-Normal-M") || LTWorldBossDataManager.Instance.IsOpenWorldBoss()))
			if (properties._flippedParticleReference.Name.Contains("fx_f_M00") && (!_animator.name.Contains("-Variant-Normal-M") || (bool)GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.LTWorldBossDataManager", "IsOpenWorldBossFromILR")))
			{
                return null;
            }

            //世界boss某个出场特效播放之后需要特殊处理场景状态
            if (properties._flippedParticleReference.Name.Equals("fx_m_m001_specialappear03") || properties._flippedParticleReference.Name.Equals("fx_m_m002_specialappear03") || properties._flippedParticleReference.Name.Equals("fx_m_m003_specialappear03"))
            {
                if (WorldBossCombatScene.Instance != null)
                {
                    WorldBossCombatScene.Instance.SetSceneStatus(2);//世界boss播放出场镜头的时候需要特殊设置场景状态
                }
            }

            if (!string.IsNullOrEmpty(properties._partition))
			{
				//AvatarComponent avatar = _player != null ? _player.GetComponent<AvatarComponent>() : GetComponent<AvatarComponent>();
				AvatarComponent avatar = GetComponent<AvatarComponent>();
				if (avatar == null)
				{
					return null;
				}

				if (!avatar.Partitions.ContainsKey(properties._partition))
				{
					return null;
				}

				/*string fxName = avatar.Partitions[properties._partition].AssetName;
				if (properties._particleReference.Valiad && !properties._particleReference.Name.EndsWith(fxName))
				{
					return null;
				}
				if (properties._flippedParticleReference.Valiad && !properties._flippedParticleReference.Name.EndsWith(fxName))
				{
					return null;
				}
				if (!string.IsNullOrEmpty(properties.ParticleName) && !properties.ParticleName.EndsWith(fxName))
				{
					return null;
				}
				if (!string.IsNullOrEmpty(properties.FlippedParticleName) && !properties.FlippedParticleName.EndsWith(fxName))
				{
					return null;
				}*/
			}

			bool canPlay = true;

			// TJ: Test conditions that would prevent the particle from playing
			if (!forcePlay)
			{
				// check if there are any particles playing using this event name
				if (properties._eventName.Length > 0 && _activeParticles.Count(pi => pi._name == properties._eventName) > 0)
				{
					ParticleInfo info = _activeParticles.Find(pi => pi._name == properties._eventName);
					if (!info._stopOnOverride)
					{
						canPlay = false;
					}
				}
				// check if we need to wait for attacks to be resolved
				else if (properties._killIfBlocked || properties._cancelIfMissed || properties._cancelIfHit)
				{
					_cachedConditionalParticles.Add(properties);
					canPlay = false;
				}
			}

			if (!canPlay)
			{
				return null;
			}

			ParticleInfo last = null;
			if (properties._eventName.Length > 0 && _activeParticles.Count(pi => pi._name == properties._eventName) > 0)
			{
				foreach (ParticleInfo info in _activeParticles.FindAll(pi => pi._name == properties._eventName))
				{
					if (info._stopOnOverride)
					{
						if (!info._particleSystem.loop || last != null)
						{
							StopParticle(info._name);
						}
						else
						{
							last = info;
						}
					}
				}
			}

			//EB.Debug.Log("PlayParticle: play {0} _activeParticles = {1}", properties._eventName, _activeParticles.Count);

			ParticleInfo new_pi = last;
			ParticleSystem ps = last != null ? last._particleSystem : null;
			if (new_pi == null)
			{
				ps = InstantiateParticle(properties, _flipped, target, tarPos, tarAimator, tarOrgPos, tarHitPos);
				if (ps == null)
                {
					//如果是开场动画，如果还没播放开场动画，还需要隔一段时间播放
					//if (CombatUtil.IsHeroStartEffect(MoveUtils.GetParticleName(properties, true)))
					if (IsHeroStartEffect(MoveUtils.GetParticleName(properties, true)))
					{
						mCurrentReplayTime++;
                        if (mCurrentReplayTime >= mReplayTime)
                        {
                            TimerManager.instance.RemoveTimer(mTimer);
                            mCurrentReplayTime = 0;
                            return null;
                        }
                        mTimer = TimerManager.instance.AddTimer(100, 1, delegate (int seq)
                        {
                            TimerManager.instance.RemoveTimer(mTimer);
                            PlayParticle(properties, time, forcePlay);
                        });
                    }
					return null;
				}

				new_pi = RegisterParticle(ps, properties._eventName, properties._interruptable, properties._stopOnExit, properties._stopOnDuration, properties._duration);
			}

			new_pi._stopOnOverride = properties._stopOnOverride;
			new_pi._stopOnEndTurn = properties._stopOnEndTurn;
			new_pi._stopAfterTurns = properties._stopAfterTurns;
			new_pi._turns = properties._turns;
            new_pi.properties = properties;
            new_pi._activeTurn = Mathf.Max(1, GetCurrentTurn());

			if (last == null)
			{
				ps.EnableEmission(true);

				if (time < 0.01f)
				{
					ps.Simulate(0.0001f, true, true);
				}
				else
				{
					ps.Simulate(time, true, true);
				}
                ps.gameObject.layer = gameObject.layer;
                ps.transform.SetChildLayer(gameObject.layer);

				ps.Play(true);
			}
            //EB.Debug.LogPSPoolAsset(string.Format(">>播放指定的特效:<color=#00ff00>{0}</color>", ps.name));
            if(PlayParticleAction!=null) PlayParticleAction(ps);

            return ps;
		}
		
		public  bool IsHeroStartEffect(string assetName)
		{
			if (string.IsNullOrEmpty(assetName))
			{
				return false;
			}
			//居然把出场写成chushang，我也没办法(之前的人留下注解)
			return assetName.Contains("chuchang") || assetName.Contains("chushang");
		}
        
		public void PlayParticleOnTargetList(ParticleEventProperties properties)
        {
            if (!properties._applyOnTargetList)
            {
                return;
            }
            if (_player == null)
            {
                return;
            }

			// calculate world position of the particle
			Hotfix_LT.Combat.Combatant[] targets = _player.GetCurrentTargets();
            if (targets == null)
            {
                return;
            }

            properties._applyOnTargetList = false;
            foreach (Hotfix_LT.Combat.Combatant target in targets)
            {
                if (target != null && target.FXHelper != null) target.FXHelper.PlayParticle(properties, false);
            }
        }

        public void StopParticle(string name, bool clear = false)
		{
			if (!string.IsNullOrEmpty(name))
			{
				ParticleInfo info = _activeParticles.Find(delegate(ParticleInfo p) { return p._name == name; });
				if (info != null)
					StopParticleInternal(info, clear, clear);
			}
		}

		public bool IsPlaying(string name)
		{
			return _activeParticles.Exists(delegate(ParticleInfo pi) { return pi._name == name; });
		}

		public void StopTrailRenderer(string name)
		{
			if (!string.IsNullOrEmpty(name))
			{
				TrailInfo ti = _activeTrailRenderers.Find(delegate(TrailInfo t) { return t._name == name; });

				if (ti._trail != null)
				{
					StopTrailRendererInternal(ti);
				}
				else
				{
					EB.Debug.LogWarning("Trying to stop a trail that does not exist: " + name);
				}

			}
		}

		public void StopDynamicLight(string name)
		{
			if (!string.IsNullOrEmpty(name))
			{
				LightInfo info = _activeDynamicLights.Find(delegate(LightInfo li) { return li._name == name; });
				StopDynamicLightInternal(info);
			}
		}

		private void StopParticleInternal(ParticleInfo particleInfo, bool clear = false, bool isRecycle = false)
		{
            if (particleInfo._particleSystem != null)
			{
                EB.Debug.LogPSPoolAsset(string.Format("停掉特效_时刻的处理=<color=#00ff00>{0}</color>,clear=<color=#00ff00>{1}</color>,isRecycle=<color=#00ff00>{2}</color>"
                    , particleInfo._particleSystem.name, clear, isRecycle));
                //
                particleInfo._particleSystem.EnableEmission(false);
				clear |= particleInfo._particleSystem.transform.parent != null;
				particleInfo._particleSystem.Stop();
				if (clear)
				{
					particleInfo._particleSystem.Clear(true);
				}
				particleInfo._particleSystem.StopAll(clear);

				Animator[] animators = particleInfo._particleSystem.GetComponentsInChildren<Animator>(true);
				foreach (var anim in animators)
				{
					anim.gameObject.CustomSetActive(false);
				}
				Animation[] animations = particleInfo._particleSystem.GetComponentsInChildren<Animation>(true);
				foreach (var anim in animations)
				{
					anim.gameObject.CustomSetActive(false);
				}

				if (clear && particleInfo._particleSystem.isPlaying)
				{
					EB.Debug.LogWarning(string.Format("FXHelper.StopParticleInternal: {0} isAlive={1} isPlaying={2} isStopped={3} isPaused={4} emission={5}",
						particleInfo._particleSystem.name,
						particleInfo._particleSystem.IsAlive(true),
						particleInfo._particleSystem.isPlaying,
						particleInfo._particleSystem.isStopped,
						particleInfo._particleSystem.isPaused,
						particleInfo._particleSystem.emission.enabled));
				}

                //只要停掉的就回收到特效池
                //if (isRecycle)
                {
                    PSPoolManager.Instance.Recycle(particleInfo._particleSystem);
                }
            }

			_activeParticles.Remove(particleInfo);
		}

		private void StopTrailRendererInternal(TrailInfo trailInfo)
		{
			trailInfo._trailInstance.Stop();

			if (GenericPoolSingleton.Instance.trailPool != null)
			{
				trailInfo._trail.transform.localScale = Vector3.one;
				GenericPoolSingleton.Instance.trailPool.Recycle(trailInfo._trail);
			}

			_activeTrailRenderers.Remove(trailInfo);
		}

		private void FreezeTrailRendererInternal(TrailInfo trailInfo, bool freeze)
		{
			trailInfo._trailInstance.Pause(freeze);
		}

		private void StopDynamicLightInternal(LightInfo lightInfo)
		{
			lightInfo._light.GetComponent<DynamicPointLightInstance>().Stop();
			lightInfo._light.CustomSetActive(false);
			_activeDynamicLights.Remove(lightInfo);
		}

		public void OnPlayAudio(MoveAnimationEvent e)
		{
			PlayAudio((e.EventRef as AudioEventInfo)._audioProperties, false);
		}
		public void OnPlayAudioEx(MoveEditor.AudioEventInfo e)
		{
			PlayAudio(e._audioProperties, false);
		}

		public void PlayAudio(AudioEventProperties audioPropes, bool forcePlay = false)
		{
			if (audioPropes._applyOnTargetList)
			{
				//AudioEventProperties clone = AudioEventProperties.Deserialize(audioPropes.Serialize());
				PlayAudioOnTargetList(audioPropes);
				return;
			}

			bool canPlay = true;

			if (!forcePlay)
			{
				if (audioPropes._event.Length > 0 && _activePersistentAudioEvents.Count(pi => pi._eventId == audioPropes._event) > 0)
				{
					PersistentAudioInfo info = _activePersistentAudioEvents.Find(pi => pi._eventId == audioPropes._event);
					if (!info._stopOnOverride)
					{
						canPlay = false;
					}
				}
				else if (audioPropes._cancelIfBlocked || audioPropes._cancelIfHit || audioPropes._cancelIfMissed)
				{                    
					_cachedConditionalAudioes.Add(audioPropes);
					canPlay = false;
				}
			}

			if (!canPlay)
			{
				return;
			}

			if (audioPropes._event.Length > 0 && _activePersistentAudioEvents.Count(pi => pi._eventId == audioPropes._event) > 0)
			{
				foreach (PersistentAudioInfo info in _activePersistentAudioEvents.FindAll(pi => pi._eventId == audioPropes._event))
				{
					if (info._stopOnOverride)
					{
						StopPersistentAudio(info._eventId);
					}
				}
			}

			if (FusionAudio.PostEvent(audioPropes._event, gameObject, true))
			{
				// Register the audio if its persistent
				if (audioPropes._isPersistent)
				{
					PersistentAudioInfo pai = RegisterPersistentAudio(audioPropes._event, audioPropes._stopOnExitAnim);
					pai._activeTime = Time.realtimeSinceStartup;
					pai._activeTurn = GetCurrentTurn();
					pai._activeTurn = Mathf.Max(1, pai._activeTurn);
					pai._stopOnDuration = audioPropes._stopOnDuration;
					pai._duration = audioPropes._duration;
					pai._stopOnEndTurn = audioPropes._stopOnEndTurn;
					pai._stopAfterTurns = audioPropes._stopAfterTurns;
					pai._turns = audioPropes._turns;
					pai._interruptable = audioPropes._interruptable;
					pai._stopOnOverride = audioPropes._stopOnOverride;
				}
			}
		}

		public void PlayAudioOnTargetList(AudioEventProperties audioPropes)
		{
			if (!audioPropes._applyOnTargetList)
			{
				return;
			}
			if (_player == null)
			{
				return;
			}

			Hotfix_LT.Combat.Combatant[] targets = _player.GetCurrentTargets();
			if (targets == null)
			{
				return;
			}

			audioPropes._applyOnTargetList = false;
			foreach (Hotfix_LT.Combat.Combatant target in targets)
			{
				target.FXHelper.PlayAudio(audioPropes, false);
			}
		}

		public void StopPersistentAudio(string eventId)
		{
			PersistentAudioInfo info = _activePersistentAudioEvents.Find(delegate(PersistentAudioInfo p) { return p._eventId == eventId; });
			if (info != null)
			{
				StopPersistentAudioInternal(info);
			}
		}

		private void StopPersistentAudioInternal(PersistentAudioInfo info)
		{
			FusionAudio.PostEvent(info._eventId, gameObject, false);
			_activePersistentAudioEvents.Remove(info);
		}

		public bool DisableFX { get; set; } = false;

		//此函数被反射调用的，不能删
		public void OnPlayParticle(MoveEditor.MoveAnimationEvent e)
		{
			//ParticleEventInfo evt = new MoveEditor.ParticleEventInfo(e.stringParameter);
			ParticleEventInfo evt = e.EventRef as ParticleEventInfo;
			PlayParticle(evt._particleProperties);
		}

		public void OnPlayParticleEx(ParticleEventInfo pe, bool target = false, Vector3 tarPos = new Vector3(), Animator tarAimator = null, Vector3 tarOrgPos = new Vector3(), Vector3 tarHitPos = new Vector3())
		{
			PlayParticle(pe._particleProperties, 0, false, target, tarPos, tarAimator, tarOrgPos, tarHitPos);
		}

		private void OnStopParticle(MoveEditor.MoveAnimationEvent e)
		{
			StopParticle(e.stringParameter);
		}

		private void OnClearParticle(MoveEditor.MoveAnimationEvent e)
		{
			StopParticle(e.stringParameter, true);
		}

        //public void OnPlayCameraShake(MoveEditor.MoveAnimationEvent e)
        //{
        //    //var info = new MoveEditor.CameraShakeEventInfo(e.stringParameter);
        //    var info = e.EventRef as CameraShakeEventInfo;
        //    bool isCrit = false;
        //    if (_player == null)
        //    {
        //        _player = GetComponent<Hotfix_LT.Combat.Combatant>();
        //    }
        //    if (_player != null)
        //    {
        //        isCrit = _player.IsCurrentAttackCritical();
        //    }
        //    MoveCameraShakeHelper.Shake(info, isCrit);
        //}

        public void OnPlayCameraShakeEx(MoveEditor.MoveAnimationEvent e, bool isCrit)
		{
			var info = e.EventRef as CameraShakeEventInfo;
			MoveCameraShakeHelper.Shake(info, isCrit);
		}

		public void OnPlayTrailRenderer(MoveAnimationEvent e)
		{
			TrailRendererEventInfo evt = e.EventRef as TrailRendererEventInfo;
			TrailRendererEventProperties properties = evt._trailRendererProperties;

            PlayTrailRenderer(properties);
        }

		public void PlayTrailRenderer(TrailRendererEventProperties properties)
		{
            bool isCrit = false;
			Hotfix_LT.Combat.Combatant combatant = gameObject.GetComponent<Hotfix_LT.Combat.Combatant>();
            if (combatant != null)
            {
                isCrit = combatant.IsCurrentAttackCritical();
            }
            GameObject trail = MoveUtils.InstantiateTrailInstance(properties, GetComponent<Animator>(), _flipped, isCrit);

            if (trail != null)
            {
                RegisterTrail(trail, properties._eventName, properties._isInterruptible);
                trail.GetComponent<TrailRendererInstance>().Play(Time.time);

                if (!string.IsNullOrEmpty(properties.RigAnimClipName))
                {
                    Animation animRoot = trail.transform.Find("anim_root").GetComponent<Animation>();
                    animRoot.Play(properties.RigAnimClipName);
                }
            }
        }

		public void OnPostFxBloomEvent(MoveEditor.MoveAnimationEvent e)
		{
			//PostBloomEventInfo evt = new PostBloomEventInfo(e.stringParameter);
			PostBloomEventInfo evt = e.EventRef as PostBloomEventInfo;
			_activePostFxEvents.Add(evt);
		}

		public void OnPostFxVignetteEvent(MoveEditor.MoveAnimationEvent e)
		{
			//PostVignetteEventInfo evt = new PostVignetteEventInfo(e.stringParameter);
			PostVignetteEventInfo evt = e.EventRef as PostVignetteEventInfo;
			_activePostFxEvents.Add(evt);
		}

		public void OnPostFxWarpEvent(MoveEditor.MoveAnimationEvent e)
		{
			//PostWarpEventInfo evt = new PostWarpEventInfo(e.stringParameter);
			PostWarpEventInfo evt = e.EventRef as PostWarpEventInfo;
			_activePostFxEvents.Add(evt);
		}

		private void OnStopTrailRenderer(MoveEditor.MoveAnimationEvent e)
		{
			StopTrailRenderer(e.stringParameter);
		}

		private void OnStopDynamicLight(MoveEditor.MoveAnimationEvent e)
		{
			StopDynamicLight(e.stringParameter);
		}

		public void OnPlayDynamicLight(MoveAnimationEvent e)
		{
			//DynamicLightEventInfo evt = new DynamicLightEventInfo(e);
			DynamicLightEventInfo evt = e.EventRef as DynamicLightEventInfo;
			DynamicLightEventProperties properties = evt._dynamicLightProperties;

			PlayDynamicLight(properties);
		}

		public void PlayDynamicLight(DynamicLightEventProperties properties)
		{
			// TJ: don't play this light if one with the same event name is already playing
			if (properties._eventName.Length > 0 && _activeDynamicLights.Count(pi => pi._name == properties._eventName) > 0)
			{
				return;
			}

			GameObject light = MoveUtils.InstantiateDynamicLight(properties, GetComponent<Animator>(), false);
			if (light != null)
			{
				RegisterLight(light, properties._eventName, properties._interruptable, properties._stopOnExit);
				light.GetComponent<DynamicPointLightInstance>().Play();
			}
		}

		public void OnModifyEnvironmentLighting(MoveEditor.MoveAnimationEvent e)
		{
			//EnvironmentLightingEventInfo evt = new EnvironmentLightingEventInfo(e.stringParameter);
			EnvironmentLightingEventInfo evt = e.EventRef as EnvironmentLightingEventInfo;
			EnvironmentLightingProperties properties = evt._lightingProperties;

			float duration = Mathf.Max(properties._multiplyDuration, properties._addDuration);

			_activeEnvLightingInfo = new EnvLightingInfo(properties, duration);
		}

		private ParticleSystem InstantiateParticle(ParticleEventProperties properties, bool flipped, bool target=false, Vector3 tarPos=new Vector3(), Animator tarAimator=null, Vector3 tarOrgPos = new Vector3(), Vector3 tarHitPos = new Vector3())
		{
			ParticleSystem ps = null;

			if (properties._spawnAtOpponent && _player != null)
			{
				// calculate world position of the particle
				//Vector3 position = _player.transform.position;
				//Combatant target = _player.GetAttackTarget();

				Vector3 position = this.gameObject.transform.position;

				if (!target)
				{
					ps = MoveUtils.InstantiateParticle(this,properties, position, flipped);
				}
				else
				{
					//position = target.transform.position;
					position = tarPos;
					if (properties._attachToOpponent)
					{
						flipped = !flipped;	// since this is now attached to the other player we need to reverse the flip

						if (tarAimator == null)
						{
							ps = MoveUtils.InstantiateParticle(this,properties, position, flipped);
						}
						else
						{
							ps = MoveUtils.InstantiateParticle(this,properties, tarAimator, flipped);
						}
					}
					else
					{
						if (properties._spawnAtTargetBase)
						{
							position = tarOrgPos;
						}
						else if (properties._spawnAtHitPoint)
						{
							position = tarHitPos;
						}

						ps = MoveUtils.InstantiateParticle(this,properties, position, flipped);
					}
				}
			}
			else
			{
				if (_animator == null)
				{
					ps = MoveUtils.InstantiateParticle(this,properties, transform.position, flipped);
				}
				else
				{
					ps = MoveUtils.InstantiateParticle(this,properties, _animator, flipped);
				}
			}

			return ps;
		}

		private ParticleInfo RegisterParticle(ParticleSystem ps, string name, bool interruptable, bool stopOnExitAnim, bool stopOnDuration, float duration)
		{
			if (ps != null)
			{
				ParticleInfo pi = new ParticleInfo();

				pi._particleSystem = ps;
				pi._name = name;
				pi._interruptable = interruptable;
				pi._stopOnExitAnim = stopOnExitAnim;
				pi._stopOnDuration = stopOnDuration;
				pi._duration = duration;
				pi._activeTime = Time.realtimeSinceStartup;
				pi._clipHash = GetCurrentAnimHash();

				_activeParticles.Add(pi);

				return pi;
			}

			return null;
		}

		private void RegisterTrail(GameObject t, string name, bool interruptable)
		{
			if (t != null)
			{
				TrailInfo ti = new TrailInfo();
				ti._trail = t;
				ti._trailInstance = t.GetComponent<TrailRendererInstance>();
				ti._interruptable = interruptable;
				ti._name = name;
				ti._clipHash = GetCurrentAnimHash();

				_activeTrailRenderers.Add(ti);
			}
		}

		public PersistentAudioInfo RegisterPersistentAudio(string eventId, bool stopOnExitAnim)
		{
			if (!string.IsNullOrEmpty(eventId))
			{
				PersistentAudioInfo pai = new PersistentAudioInfo();
				pai._eventId = eventId;
				pai._stopOnExitAnim = stopOnExitAnim;
				pai._clipHash = GetCurrentAnimHash();

				_activePersistentAudioEvents.Add(pai);

				return pai;
			}

			return null;
		}

		private void RegisterLight(GameObject t, string name, bool interruptable, bool stopOnExitAnim)
		{
			if (t != null)
			{
				LightInfo li = new LightInfo();
				t.SetActive(true);
				li._light = t;
				li._interruptable = interruptable;
				li._stopOnExitAnim = stopOnExitAnim;
				li._name = name;
				li._clipHash = GetCurrentAnimHash();

				_activeDynamicLights.Add(li);
			}
		}

		public Transform FXRootTransform
		{
			get { return m_DefaultFXAttachment == null ? transform : m_DefaultFXAttachment; }
		}

		public Transform HeadNubTransform
		{
			get { return m_HeadFXAttachment == null ? transform: m_HeadFXAttachment; }
		}

		public Transform ChestNubTransform
		{
			get { return m_ChestFXAttachment == null ? transform: m_ChestFXAttachment; }
		}

		public Transform FootNubTransform
		{
			get { return m_FootFXAttahment == null? transform: m_FootFXAttahment; }
		}

		public Transform HealthBarTransform
		{
			get { return m_HealthBarFXAttachment == null ? HeadNubTransform : m_HealthBarFXAttachment; }
		}

		private int GetCurrentAnimHash()
		{
			if (_player != null)
			{
				return _player.MoveController.GetCurrentAnimHash();
			}
			else
			{
				return _animator.IsInTransition(0) ? _animator.GetNextAnimatorStateInfo(0).fullPathHash : _animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
			}
		}

		private int GetCurrentTurn()
		{
			if (_player != null)
			{
				return _player.GetTurn();
			}
			else
			{
				return 0;
			}
		}

		private List<ParticleInfo>		_activeParticles			= new List<ParticleInfo>();
		private List<TrailInfo>			_activeTrailRenderers 		= new List<TrailInfo>();
		private List<LightInfo>			_activeDynamicLights 		= new List<LightInfo>();
		private List<PostFxEventInfo>	_activePostFxEvents			= new List<PostFxEventInfo>();
		private List<PersistentAudioInfo> _activePersistentAudioEvents = new List<PersistentAudioInfo>();
		private EnvLightingInfo			_activeEnvLightingInfo		= null;

		// TJ: this is a list of particles that MAY NOT PLAY based on certain conditions; save them until the end of the frame on which they are requested to resolve conditions
		private List<ParticleEventProperties> _cachedConditionalParticles = new List<ParticleEventProperties>();
		private List<AudioEventProperties> _cachedConditionalAudioes = new List<AudioEventProperties>();

		private Hotfix_LT.Combat.Combatant _player				= null;
		private Animator 					_animator			= null;
		private int 						_currentAnimHash	= -1;
		private bool 						_flipped 			= false;
	}
}
