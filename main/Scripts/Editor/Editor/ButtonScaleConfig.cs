using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ButtonScaleConfig  {

    [MenuItem("NGUI/Options/ButtonScaleConfig")]
    public static void CreatButtonScaleConfig()
    {
        ButtonScaleData asset = ScriptableObject.CreateInstance<ButtonScaleData>();

        asset.scale = 1.2f;
        asset.shrink = 0.9f;
        asset.scaleDuration = 0.1f;
        asset.shrinkDuration = 0.05f;
        AssetDatabase.CreateAsset(asset, "Assets/Resources/ButtonScaleData.asset");
    }
}
