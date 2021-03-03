﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCameraBlur : MonoBehaviour
{
	public delegate void Callback(RenderTexture tex);

	public Material Mat;
	public float Size;
	public int Sample;

	private static Callback callback;
	public static CombatCameraBlur Instance;
	public static void Capture(Callback func)
	{
		if (Instance)
		{
			callback = func;
			Instance.enabled = true;
		}
		else
		{
			EB.Debug.LogError("CombatCameraBlur.Capture: error caused by nil instance.");
		}
	}

	void Awake()
	{
		if (Mat == null)
		{
			EB.Debug.LogError("CombatCameraBlur.Awake: error caused by nil mat.");
		}
		Instance = this;
		enabled = false;
	}

	void OnDestroy()
	{
		Instance = null;
		callback = null;
		Mat = null;
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		if (Mat)
		{
			int rtW = src.width / Sample;
			int rtH = src.height / Sample;
			RenderTexture rtTempA = RenderTexture.GetTemporary(rtW, rtH, 0, src.format);
			rtTempA.filterMode = FilterMode.Bilinear;
			Graphics.Blit(src, rtTempA);
			for (int i = 0; i < Size; i++)
			{
				float iteraionOffs = i * 1.0f;
				Mat.SetFloat("_blurSize", Size + iteraionOffs);

				//vertical blur           
				RenderTexture rtTempB = RenderTexture.GetTemporary(rtW, rtH, 0, src.format);
				rtTempB.filterMode = FilterMode.Bilinear;
				Graphics.Blit(rtTempA, rtTempB, Mat, 0);
				RenderTexture.ReleaseTemporary(rtTempA);
				rtTempA = rtTempB;

				//horizontal blur             
				rtTempB = RenderTexture.GetTemporary(rtW, rtH, 0, src.format);
				rtTempB.filterMode = FilterMode.Bilinear;
				Graphics.Blit(rtTempA, rtTempB, Mat, 1);
				RenderTexture.ReleaseTemporary(rtTempA);
				rtTempA = rtTempB;
			}
			Graphics.Blit(src, dest);
			if (callback != null)
			{
				callback(rtTempA);
				callback = null;
				enabled = false;
			}
			else
			{
				RenderTexture.ReleaseTemporary(rtTempA);
			}
		}
	}
}
