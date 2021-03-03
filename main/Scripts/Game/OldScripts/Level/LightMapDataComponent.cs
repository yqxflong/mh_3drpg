using UnityEngine;
using System.Collections;
using BE.Level;

/// <summary>
/// 光照贴图纹理
/// </summary>
[System.Serializable]
public class LightMapTextures
{
	public Texture2D lightmapLight;
	//public Texture2D lightmapDir;

	public LightMapTextures(Texture2D _lightmapLight/*, Texture2D _lightmapDir*/)
	{
		lightmapLight = _lightmapLight;
		//lightmapDir = _lightmapDir;
	}
}

/// <summary>
/// 光照贴图管理器(主要是为了防止异步加载的时候出现光照贴图引用发生丢失情况)
/// </summary>
public class LightMapDataComponent : MonoBehaviour
{
    /// <summary>
    /// 相应的光照贴图的索引
    /// </summary>
	public int lightmapIndex;
    /// <summary>
    /// 相应的光照贴图的大小与偏移量
    /// </summary>
	public Vector4 lightmapScaleOffset;

	void Start()
	{
        /*
        if (Application.isPlaying)
        {
            Destroy(this);
        }*/
        RestoreLightMapData();
	}

    /// <summary>
    /// 修复光照贴图的数据
    /// </summary>
	public void RestoreLightMapData()
	{
		Renderer objRenderer = gameObject.GetComponent<Renderer>();
		if (objRenderer != null && lightmapIndex >= 0)
		{
			objRenderer.lightmapIndex = lightmapIndex;
			objRenderer.lightmapScaleOffset = lightmapScaleOffset;

		}
	}
}
