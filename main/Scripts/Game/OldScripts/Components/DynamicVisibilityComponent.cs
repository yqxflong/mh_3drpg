///////////////////////////////////////////////////////////////////////
//
//  DynamicVisibilityComponent.cs
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

public class DynamicVisibilityComponent : MonoBehaviour 
{
	private const float _minAlpha = 0.4f;
	private const float _fadeTime = 0.3f;
	private float _currentFadeTime = _fadeTime;
	private bool _isUsingTransparency = false;
	
	private int _fadeOutFrameCount = 0;
	public int FadeOutFrameCount
	{
		get { return _fadeOutFrameCount; }
	}

	private Shader _transparentShader;
	private Material[] _sharedMaterialsOriginal = null;
	private Material[] _sharedMaterialsTransparent = null;
	
	private void Awake()
	{
		if (GetComponent<Renderer>() == null)
		{
			return;
		}
		
		_sharedMaterialsOriginal = GetComponent<Renderer>().sharedMaterials;		
		if (_sharedMaterialsOriginal.Length <= 0)
		{
			return;
		}
		
		// look up the shader with transparency support
		_transparentShader = Shader.Find("Fusion/Unlit/VertexColorLM_Alpha");
		
		// we want to avoid creating a new instance of each material. By setting sharedMaterials back to the 
		//  original array when we're done fading, we keep batching from breaking
		_sharedMaterialsTransparent = new Material[_sharedMaterialsOriginal.Length];

		// cache original sharedMaterials array, and a copy with the modified shader (supports alpha)
		for(int i = 0; i < _sharedMaterialsOriginal.Length; i++)
		{			
			if (_sharedMaterialsOriginal[i] == null)
			{
				continue;
			}
			Material transparentMaterial = new Material(_sharedMaterialsOriginal[i]);
			transparentMaterial.shader = _transparentShader;
			
			_sharedMaterialsTransparent[i] = transparentMaterial;
		}
	}
	
	public void FadeOut()
	{
		_fadeOutFrameCount = Time.frameCount;
		if (_sharedMaterialsOriginal.Length <= 0)
		{
			return;
		}
		
		if (!_isUsingTransparency)
		{
			_isUsingTransparency = true;
			GetComponent<Renderer>().sharedMaterials = _sharedMaterialsTransparent;
		}
		
		float oldTime = _currentFadeTime;
		_currentFadeTime = Mathf.Max(0.0f, _currentFadeTime - Time.deltaTime);
		if (oldTime == _currentFadeTime)
		{
			return;
		}
		
		// find out the alpha percent base on _currentFadeTime
		float alphaPercent = _currentFadeTime / _fadeTime;
		alphaPercent = Mathf.Lerp(_minAlpha, 1.0f, alphaPercent);
		for (int i = 0; i < _sharedMaterialsTransparent.Length; ++i)
		{
			_sharedMaterialsTransparent[i].color = new Color(_sharedMaterialsOriginal[i].color.r, _sharedMaterialsOriginal[i].color.g,
				_sharedMaterialsOriginal[i].color.b, _sharedMaterialsOriginal[i].color.a * alphaPercent);
		}
	}
	
	// return true if is totally fade in
	public bool FadeIn()
	{
		if (_sharedMaterialsOriginal.Length <= 0)
		{
			return false;
		}
		
		float oldTime = _currentFadeTime;
		_currentFadeTime = Mathf.Min(_fadeTime, _currentFadeTime + Time.deltaTime);
		if (oldTime == _currentFadeTime)
		{
			return false;
		}
		
		if (_currentFadeTime == _fadeTime)
		{
			if (_isUsingTransparency)
			{
				_isUsingTransparency = false;
				GetComponent<Renderer>().sharedMaterials = _sharedMaterialsOriginal;
			}
			return true;
		}
		else
		{
			// find out the alpha percent base on _currentFadeTime
			float alphaPercent = _currentFadeTime / _fadeTime;
			alphaPercent = Mathf.Lerp(_minAlpha, 1.0f, alphaPercent);
			for (int i = 0; i < _sharedMaterialsTransparent.Length; ++i)
			{
				_sharedMaterialsTransparent[i].color = new Color(_sharedMaterialsOriginal[i].color.r, _sharedMaterialsOriginal[i].color.g,
					_sharedMaterialsOriginal[i].color.b, _sharedMaterialsOriginal[i].color.a * alphaPercent);
			}
			return false;
		}
	}
}
