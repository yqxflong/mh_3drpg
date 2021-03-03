using UnityEngine;
 using System.Collections.Generic;

/// <summary>
/// Use the provided editor action to put this component on all objects that use static light maps, in order to have their lightmap parameters exported to an asset bundle and later restored properly. 
/// The corresponding lightmaps will automatically be referenced and included in the bundle. At loading time you need to have this script in your hosting project and call the static method RestoreAll. 
/// </summary>
[ExecuteInEditMode]
public class LightmapParams : MonoBehaviour
{
    public Texture2D lightmapNear, lightmapFar;
    public int lightmapIndex;
    public Vector4 lightmapScaleOffset;

    /// <summary>This function is called from an editor action and creates or updates this component on all objects with renderers and static lightmap in the current scene. </summary>
    public static void StoreAll()
    {
        foreach (Renderer r in FindObjectsOfType<Renderer>())
            if (r.lightmapIndex != -1)
            {
                LightmapParams lmp = r.gameObject.GetComponent<LightmapParams>();
                if (!lmp)
                    lmp = r.gameObject.AddComponent<LightmapParams>();
                lmp.lightmapIndex = r.lightmapIndex;
                lmp.lightmapScaleOffset = r.lightmapScaleOffset;
                lmp.lightmapNear = LightmapSettings.lightmaps[lmp.lightmapIndex].lightmapDir;
                lmp.lightmapFar = LightmapSettings.lightmaps[lmp.lightmapIndex].lightmapColor;
            }
    }

    /// <summary>This function needs to be called after importing an assetbundle with exported lightmap data (see <see cref="StoreAll"/>), in order to restore it to the object. The collection it lightmaps will be set from the set of all lightmaps of all <see cref="LightmapParams"/> instances.</summary>
    public static void RestoreAll(LightmapsMode mode, bool removeComponentsAfterwards = false, bool setStatic = false)
    {
        List<LightmapData> newLightmaps = new List<LightmapData>();

        foreach (LightmapParams p in FindObjectsOfType<LightmapParams>())
        {
            Renderer r = p.gameObject.GetComponent<Renderer>();
            if (!r)
                continue;

            r.lightmapIndex = p.lightmapIndex;
            r.lightmapScaleOffset = p.lightmapScaleOffset;

            // collect lightmaps from references: 
            while (newLightmaps.Count <= p.lightmapIndex)
                newLightmaps.Add(new LightmapData());
            newLightmaps[p.lightmapIndex].lightmapDir = p.lightmapNear;
            newLightmaps[p.lightmapIndex].lightmapColor = p.lightmapFar;

            if (setStatic)
                r.gameObject.isStatic = true;

            if (removeComponentsAfterwards)
                Destroy(p);
        }

        // activate: 
        LightmapSettings.lightmaps = newLightmaps.ToArray();
        LightmapSettings.lightmapsMode = mode;
    }

    /// <summary>This function is called from an editor action to remove all <see cref="LightmapParams"/> components in the scene.</summary>
    public static void RemoveAll()
    {
        foreach (LightmapParams p in FindObjectsOfType<LightmapParams>())
            DestroyImmediate(p);
    }

    void Awake()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Renderer r = GetComponent<Renderer>();
            if (r != null)
            {
                lightmapIndex = r.lightmapIndex;
                lightmapScaleOffset = r.lightmapScaleOffset;
                if (lightmapIndex >= 0)
                {
                    lightmapNear = LightmapSettings.lightmaps[lightmapIndex].lightmapDir;
                    lightmapFar = LightmapSettings.lightmaps[lightmapIndex].lightmapColor;
                }
            }
        }
#endif
    }
}

