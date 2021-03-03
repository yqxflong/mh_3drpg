#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace EB.Rendering
{
    public class EnviroUberCompositor : UberShaderCompositor
    {
        private static EnviroUberCompositor instance=null;
        public new static EnviroUberCompositor Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EnviroUberCompositor();
                }
                return instance;
            }
        }

        protected override Shader CompositeUberShader(Material material)
        {
            bool isCutout = material.shader.name.Contains("UberCutout");
            bool isFade = material.shader.name.Contains("UberFade");
            bool isOwnLightMap = material.shader.name.Contains("UberOwnLightMap");
            bool isT4M = material.shader.name.Contains("UberT4M");

            var include = Application.dataPath + "/_GameAssets/Scripts/Game/OldScripts/Rendering/Shaders/Environment/Uber/Lib/EnviroUber.cginc";

            List<ShaderCompositor.ShaderProperty> properties = new List<ShaderCompositor.ShaderProperty>() {
                new ShaderCompositor.ShaderProperty("_MainTex", "Diffuse Tex", ShaderCompositor.ePROPERTY_TYPE.Texture2D, ShaderCompositor.eDEFAULT_VALUE.Black),
                new ShaderCompositor.ShaderProperty("_NDotLWrap", "N.L Wrap", ShaderCompositor.ePROPERTY_TYPE.Float, ShaderCompositor.eDEFAULT_VALUE.Zero),
                new ShaderCompositor.ShaderProperty("_NDotLWrap1", "N.L Wrap 2", ShaderCompositor.ePROPERTY_TYPE.Float, ShaderCompositor.eDEFAULT_VALUE.Zero),
                new ShaderCompositor.ShaderProperty("_NormalMap", "Normal Map", ShaderCompositor.ePROPERTY_TYPE.Texture2D, ShaderCompositor.eDEFAULT_VALUE.Bump),

                new ShaderCompositor.ShaderProperty("_NormalMapIntensity", "Normal Map Itensity", ShaderCompositor.ePROPERTY_TYPE.Float, ShaderCompositor.eDEFAULT_VALUE.Zero),
                new ShaderCompositor.ShaderProperty("_NormalMapDamp", "Normal Map Dampening", ShaderCompositor.ePROPERTY_TYPE.Float, ShaderCompositor.eDEFAULT_VALUE.Zero),

                new ShaderCompositor.ShaderProperty("_SpecEmissiveTex", "Spec/Emissive Map (R - Spec Mask, G - Gloss, B - Emissive)", ShaderCompositor.ePROPERTY_TYPE.Texture2D, ShaderCompositor.eDEFAULT_VALUE.Black),
                new ShaderCompositor.ShaderProperty("_SpecularIntensity", "Specular Intensity", ShaderCompositor.ePROPERTY_TYPE.Float, ShaderCompositor.eDEFAULT_VALUE.One),
                new ShaderCompositor.ShaderProperty("_SpecularGlossModulation", "Specular Gloss Modulation", ShaderCompositor.ePROPERTY_TYPE.Float, ShaderCompositor.eDEFAULT_VALUE.One),
                new ShaderCompositor.ShaderProperty("_EmissiveColor", "Emissive Color", ShaderCompositor.ePROPERTY_TYPE.Color, ShaderCompositor.eDEFAULT_VALUE.Black),
            };

            if (isCutout)
            {
                if(!isFade)
                {
                    properties.Add(new ShaderCompositor.ShaderProperty("_Cutoff", "Alpha Cutoff", ShaderCompositor.ePROPERTY_TYPE.Float, ShaderCompositor.eDEFAULT_VALUE.One));
                }
                properties.Add(new ShaderCompositor.ShaderProperty("_ReflectionColor", "Reflection Color", ShaderCompositor.ePROPERTY_TYPE.Color, ShaderCompositor.eDEFAULT_VALUE.Black));
                properties.Add(new ShaderCompositor.ShaderProperty("_ReflectionHDR", "Reflection HDR", ShaderCompositor.ePROPERTY_TYPE.Float, ShaderCompositor.eDEFAULT_VALUE.Zero));
                properties.Add(new ShaderCompositor.ShaderProperty("_FresnelScale", "Fresnel Intensity", ShaderCompositor.ePROPERTY_TYPE.Float, ShaderCompositor.eDEFAULT_VALUE.Zero));
                properties.Add(new ShaderCompositor.ShaderProperty("_FresnelPower", "FresnelPower", ShaderCompositor.ePROPERTY_TYPE.Float, ShaderCompositor.eDEFAULT_VALUE.One));
                properties.Add(new ShaderCompositor.ShaderProperty("_FresnelColor", "FresnelColor", ShaderCompositor.ePROPERTY_TYPE.Color, ShaderCompositor.eDEFAULT_VALUE.White));
            }

            if (isOwnLightMap)
            {
                properties.Add(new ShaderCompositor.ShaderProperty("_Lightmap", "Lightmap Tex", ShaderCompositor.ePROPERTY_TYPE.Texture2D, ShaderCompositor.eDEFAULT_VALUE.Black));
                properties.Add(new ShaderCompositor.ShaderProperty("_LightmapInd", "LightmapInd Tex", ShaderCompositor.ePROPERTY_TYPE.Texture2D, ShaderCompositor.eDEFAULT_VALUE.Black));
                properties.Add(new ShaderCompositor.ShaderProperty("_Color", "Color Scale", ShaderCompositor.ePROPERTY_TYPE.Color, ShaderCompositor.eDEFAULT_VALUE.White));
            }

            if(isT4M)
            {
                properties.Clear();
                properties.Add(new ShaderCompositor.ShaderProperty("_MainTex", "Layer 1 (R)", ShaderCompositor.ePROPERTY_TYPE.Texture2D, ShaderCompositor.eDEFAULT_VALUE.Black));
                properties.Add(new ShaderCompositor.ShaderProperty("_Splat1", "Layer 2 (G)", ShaderCompositor.ePROPERTY_TYPE.Texture2D, ShaderCompositor.eDEFAULT_VALUE.Black));
                properties.Add(new ShaderCompositor.ShaderProperty("_Splat2", "Layer 3 (B)", ShaderCompositor.ePROPERTY_TYPE.Texture2D, ShaderCompositor.eDEFAULT_VALUE.Black));
                properties.Add(new ShaderCompositor.ShaderProperty("_Splat3", "Layer 4 (A)", ShaderCompositor.ePROPERTY_TYPE.Texture2D, ShaderCompositor.eDEFAULT_VALUE.Black));
                properties.Add(new ShaderCompositor.ShaderProperty("_T4MControl", "Control (RGBA)", ShaderCompositor.ePROPERTY_TYPE.Texture2D, ShaderCompositor.eDEFAULT_VALUE.Black));
                properties.Add(new ShaderCompositor.ShaderProperty("_NDotLWrap", "N.L Wrap", ShaderCompositor.ePROPERTY_TYPE.Float, ShaderCompositor.eDEFAULT_VALUE.Zero));
            }

            ShaderCompositor.CategoryBlock categoryBlock = new ShaderCompositor.CategoryBlock();
            if(isFade)
            {
                categoryBlock.Tags = new List<string>() { "\"Queue\"=\"Transparent-1\"", "\"RenderType\"=\"EnvironmentTransparent\"", "\"LightMode\"=\"ForwardBase\"" };
                categoryBlock.CullMode = UnityEngine.Rendering.CullMode.Back;
                categoryBlock.ZWrite = false;
                categoryBlock.ZTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                // Blend Off
                categoryBlock.SrcBlendMode = UnityEngine.Rendering.BlendMode.SrcAlpha;
                categoryBlock.DstBlendMode = UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
            }
            else if (isT4M)
            {
                categoryBlock.Tags = new List<string>() { "\"Queue\"=\"Geometry\"", "\"RenderType\"=\"EnvironmentT4M\"", "\"LightMode\"=\"ForwardBase\"" };
                categoryBlock.CullMode = UnityEngine.Rendering.CullMode.Back;
                categoryBlock.ZWrite = true;
                categoryBlock.ZTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                // Blend Off
                categoryBlock.SrcBlendMode = UnityEngine.Rendering.BlendMode.One;
                categoryBlock.DstBlendMode = UnityEngine.Rendering.BlendMode.Zero;
            }
            else
            {
                categoryBlock.Tags = new List<string>() { "\"Queue\"=\"Geometry\"", "\"RenderType\"=\"Environment\"", "\"LightMode\"=\"ForwardBase\"" };
                categoryBlock.CullMode = UnityEngine.Rendering.CullMode.Back;
                categoryBlock.ZWrite = true;
                categoryBlock.ZTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                // Blend Off
                categoryBlock.SrcBlendMode = UnityEngine.Rendering.BlendMode.One;
                categoryBlock.DstBlendMode = UnityEngine.Rendering.BlendMode.Zero;
            }

            List<string> cg100 = new List<string>()
            {
                //"#define SHADOWS_SCREEN",
                "#include \"UnityCG.cginc\"",
                "#include \"Lighting.cginc\"",
                "#include \"AutoLight.cginc\"",
                "#include \"Assets/_GameAssets/Scripts/Game/OldScripts/Rendering/Shaders/Lib/EBG_Globals.cginc\"",
                "#pragma target 3.0",
                "#pragma vertex vertex_shader",
                "#pragma fragment fragment_shader",
                "#pragma multi_compile_fwdbase",
            };
            List<string> cg50 = new List<string>()
            {
                //"#include \"UnityCG.cginc\"",
                "#include \"Lighting.cginc\"",
                "#include \"AutoLight.cginc\"",
                "#include \"Assets/_GameAssets/Scripts/Game/OldScripts/Rendering/Shaders/Lib/EBG_Globals.cginc\"",
                "#pragma target 3.0",
                "#pragma vertex vertex_shader",
                "#pragma fragment fragment_shader",
            };
            //we only need this for LODs with normal maps


            List<ShaderCompositor.LOD> lods = null;
            if (isCutout)
            {
                lods = new List<ShaderCompositor.LOD>() {
                    new ShaderCompositor.LOD(
                        400,
                        new List<string>() { "EBG_TRANSPARENT", "EBG_FOG_ON", "EBG_DYNAMIC_SHADOWS_ON", "EBG_LIGHTMAP_ON" },
                        new List<string>() { "EBG_VERTEX_LIGHTING_OFF", "EBG_PLANAR_REFLECTIONS_OFF" },
                        cg100
                    ),
                    new ShaderCompositor.LOD(
                        200,
                        new List<string>() { "EBG_TRANSPARENT", "EBG_FOG_ON", "EBG_LIGHTMAP_ON" },
                        new List<string>() { "EBG_DYNAMIC_SHADOWS_OFF", "EBG_VERTEX_LIGHTING_OFF", "EBG_PLANAR_REFLECTIONS_OFF" },
                        cg50
                    ),
                };
            }
            else if (isOwnLightMap)
            {
                lods = new List<ShaderCompositor.LOD>() {
                    new ShaderCompositor.LOD(
                        400,
                        new List<string>() { "EBG_FOG_ON", "EBG_DYNAMIC_SHADOWS_ON", "EBG_LIGHTMAP_ON", "EBG_LIGHTMAP_OWN_ON", "EBG_DIFFUSE_COLOR" },
                        new List<string>() { "EBG_VERTEX_LIGHTING_OFF", "EBG_PLANAR_REFLECTIONS_OFF", "EBG_DISABLE_LIGHTMAP_OFF" },
                        cg100
                    ),
                    new ShaderCompositor.LOD(
                        200,
                        new List<string>() { "EBG_FOG_ON", "EBG_LIGHTMAP_ON", "EBG_LIGHTMAP_OWN_ON", "EBG_DIFFUSE_COLOR" },
                        new List<string>() { "EBG_DYNAMIC_SHADOWS_OFF", "EBG_VERTEX_LIGHTING_OFF", "EBG_PLANAR_REFLECTIONS_OFF", "EBG_DISABLE_LIGHTMAP_OFF" },
                        cg50
                    ),
                };
            }
            else if (isT4M)
            {
                lods = new List<ShaderCompositor.LOD>() {
                    new ShaderCompositor.LOD(
                        400,
                        new List<string>() { "EBG_FOG_ON", "EBG_DYNAMIC_SHADOWS_ON", "EBG_LIGHTMAP_ON", "EBG_T4M_ON" },
                        new List<string>() { "EBG_VERTEX_LIGHTING_OFF", "EBG_ALPHA_CUTOFF_OFF", "EBG_PLANAR_REFLECTIONS_OFF", "EBG_DISABLE_LIGHTMAP_OFF", "EBG_HIGHLIGHTS_IGNORE_ALPHA_OFF", "EBG_NORMAL_MAP_OFF", "EBG_SPEC_OFF", "EBG_EMISSIVE_OFF", "EBG_REFLECTIONS_OFF", "EBG_PLANAR_REFLECTIONS_OFF", "EBG_FRESNEL_OFF", "EBG_DISABLE_LIGHTMAP_OFF"},
                        cg100
                    ),
                    new ShaderCompositor.LOD(
                        200,
                        new List<string>() { "EBG_FOG_ON", "EBG_LIGHTMAP_ON", "EBG_T4M_ON" },
                        new List<string>() { "EBG_DYNAMIC_SHADOWS_OFF","EBG_VERTEX_LIGHTING_OFF", "EBG_ALPHA_CUTOFF_OFF", "EBG_PLANAR_REFLECTIONS_OFF", "EBG_DISABLE_LIGHTMAP_OFF", "EBG_HIGHLIGHTS_IGNORE_ALPHA_OFF", "EBG_NORMAL_MAP_OFF", "EBG_SPEC_OFF", "EBG_EMISSIVE_OFF", "EBG_REFLECTIONS_OFF", "EBG_PLANAR_REFLECTIONS_OFF", "EBG_FRESNEL_OFF", "EBG_DISABLE_LIGHTMAP_OFF"},
                        cg50
                    ),
                };
            }
            else
            {
                lods = new List<ShaderCompositor.LOD>() {
                    new ShaderCompositor.LOD(
                        400,
                        new List<string>() { "EBG_FOG_ON", "EBG_DYNAMIC_SHADOWS_ON", "EBG_LIGHTMAP_ON" },
                        new List<string>() { "EBG_VERTEX_LIGHTING_OFF", "EBG_ALPHA_CUTOFF_OFF", "EBG_HIGHLIGHTS_IGNORE_ALPHA_OFF", "EBG_REFLECTIONS_OFF", "EBG_PLANAR_REFLECTIONS_OFF", "EBG_FRESNEL_OFF", "EBG_DISABLE_LIGHTMAP_OFF" },
                        cg100
                    ),
                    new ShaderCompositor.LOD(
                        200,
                        new List<string>() { "EBG_FOG_ON", "EBG_LIGHTMAP_ON" },
                        new List<string>() { "EBG_DYNAMIC_SHADOWS_OFF", "EBG_VERTEX_LIGHTING_OFF", "EBG_ALPHA_CUTOFF_OFF", "EBG_HIGHLIGHTS_IGNORE_ALPHA_OFF", "EBG_REFLECTIONS_OFF", "EBG_PLANAR_REFLECTIONS_OFF", "EBG_FRESNEL_OFF", "EBG_DISABLE_LIGHTMAP_OFF" },
                        cg50
                    ),
                };
            }

            char slash = System.IO.Path.DirectorySeparatorChar;
            string dir = Application.dataPath + slash + "Merge" + slash + "Shaders" + slash;
            System.IO.Directory.CreateDirectory(dir);

            string shaderName = ShaderCompositor.GenerateShaderName(material);
            string filePath = string.Empty;
            Shader composited = ShaderCompositor.Composite(shaderName, material, dir, include, "EBG_", properties, categoryBlock, lods, out filePath);

            AssetDatabase.Refresh();

            return Shader.Find(composited.name);
        }
    }
}
#endif