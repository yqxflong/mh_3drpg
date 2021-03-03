///////////////////////////////////////////////////////////////////////
//
//  FXPoolable.cs
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
using System.Collections.Generic;

public class FXPoolable : MonoBehaviour, IPoolable
{
	private ParticleSystem[] _particleSystems;
	private Animation[] _animations;
	private AudioSource[] _audioSources;
	private Projector[] _projectors;

	public bool isTransparent = true;
	
	void Awake()
	{
		if (isTransparent)
		{
			if (GameEngine.Instance == null)
			{
				EB.Debug.LogWarning("Could not find game engine.");
			}
			else
			{
				gameObject.layer = GameEngine.Instance.transparentFXLayer;
				foreach (Transform t in transform)
				{
					t.gameObject.layer = GameEngine.Instance.transparentFXLayer;
				}
			}
		}
	}
	
	void OnEnable()
	{
        if (_particleSystems == null)
        {
            _particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
        }
		//foreach(ParticleSystem ps in _particleSystems)
		//{
			//ps.playOnAwake = false;
		//}
		
        if (_animations == null)
        {
            _animations = gameObject.GetComponentsInChildren<Animation>();
        }
		foreach(Animation animation in _animations) 
		{
			animation.Rewind();
			animation.playAutomatically = false;
		}
		
        if (_audioSources == null)
        {
            _audioSources = gameObject.GetComponentsInChildren<AudioSource>();
        }
		foreach(AudioSource audioSource in _audioSources)
		{
			audioSource.playOnAwake = false;
			audioSource.Stop();
		}

        if (_projectors == null)
        {
            _projectors = gameObject.GetComponentsInChildren<Projector>();
        }
	}
	
	public void OnPoolActivate()
	{
        if (_particleSystems != null)
        {
            for (int i = 0; i < _particleSystems.Length; i++)
            {
                ParticleSystem ps = _particleSystems[i];
                if (ps != null)
                {
                    ps.Clear();
                    ps.Simulate(0.005f);
                    ps.Play();
                }
            }
        }

        if (_animations != null)
        {
            for (int i = 0; i < _animations.Length; i++)
            {
                Animation animation = _animations[i];
                animation.Play();
            }
        }

        if (_audioSources != null)
        {
            for (int i = 0; i < _audioSources.Length; i++)
            {
                AudioSource audioSource = _audioSources[i];
                audioSource.Play();
            }
        }
	}
	
	public void OnPoolDeactivate()
    {
        if (_particleSystems != null)
        {
            for (int i = 0; i < _particleSystems.Length; i++)
            {
                ParticleSystem ps = _particleSystems[i];
                if (ps != null)
                {
                    ps.Clear();
                }
            }
        }
    }

	void OnDestroy()
	{
		if (_projectors != null)
		{
			for (int i = 0; i < _projectors.Length; i++)
			{
				Projector p = _projectors[i];
				if (p != null && p.material != null && p.material.name.EndsWith("-Instance"))
				{
                    EB.Debug.LogCoreAsset("<color=#ff0000>XXX清除掉对象:{0}</color>", (p.material == null) ? "空的引用" : p.material.name);
                    Destroy(p.material);
				}
			}
		}
	}
}
