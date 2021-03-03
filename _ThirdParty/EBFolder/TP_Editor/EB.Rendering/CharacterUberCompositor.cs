#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace EB.Rendering
{
    public class CharacterUberCompositor  : UberShaderCompositor
    {
        private static CharacterUberCompositor instance=null;
        public new static CharacterUberCompositor Instance
        {
            get
            {
                if(instance==null)
                {
                    instance = new CharacterUberCompositor();
                }
                return instance;
            }
        }

        protected override Shader CompositeUberShader(Material material)
        {
            bool isCutout = material.shader.name.Contains("UberCutout");

            var include = Application.dataPath + "/Scripts/Rendering/Shaders/Character/Lib/CharUber.cginc";

            List<ShaderCompositor.ShaderProperty> properties = new List<ShaderCompositor.ShaderProperty>() {
                new ShaderCompositor.ShaderProperty("_MainTex", "Diffuse Tex", ShaderCompositor.ePROPERTY_TYPE.Texture2D, ShaderCompositor.eDEFAULT_VALUE.Black),
                new ShaderCompositor.ShaderProperty("_NDotLWrap", "N.L Wrap", ShaderCompositor.ePROPERTY_TYPE.Float, ShaderCompositor.eDEFAULT_VALUE.Zero),
                new ShaderCompositor.ShaderProperty("_NDotLWrap1", "N.L Wrap 2", ShaderCompositor.ePROPERTY_TYPE.Float, ShaderCompositor.eDEFAULT_VALUE.Zero),
                new ShaderCompositor.ShaderProperty("_NormalMap", "Normal Map", ShaderCompositor.ePROPERTY_TYPE.Texture2D, ShaderCompositor.eDEFAULT_VALUE.Bump),
                new ShaderCompositor.ShaderProperty("_SpecTex", "Spec Map (A - Gloss)", ShaderCompositor.ePROPERTY_TYPE.Texture2D, ShaderCompositor.eDEFAULT_VALUE.Black),
                new ShaderCompositor.ShaderProperty("_SpecularIntensity", "Specular Intensity", ShaderCompositor.ePROPERTY_TYPE.Float, ShaderCompositor.eDEFAULT_VALUE.One),
                new ShaderCompositor.ShaderProperty("_SpecularGlossModulation", "Specular Gloss Modulation", ShaderCompositor.ePROPERTY_TYPE.Float, ShaderCompositor.eDEFAULT_VALUE.One),
                new ShaderCompositor.ShaderProperty("_EmissiveTex", "Emissive Map", ShaderCompositor.ePROPERTY_TYPE.Texture2D, ShaderCompositor.eDEFAULT_VALUE.Black),
                new ShaderCompositor.ShaderProperty("_EmissiveColor", "Emissive Color", ShaderCompositor.ePROPERTY_TYPE.Color, ShaderCompositor.eDEFAULT_VALUE.Black),
                new ShaderCompositor.ShaderProperty("_ReflectionColor", "Reflection Color", ShaderCompositor.ePROPERTY_TYPE.Color, ShaderCompositor.eDEFAULT_VALUE.Black),
                new ShaderCompositor.ShaderProperty("_ReflectionHDR", "Reflection HDR", ShaderCompositor.ePROPERTY_TYPE.Float, ShaderCompositor.eDEFAULT_VALUE.Zero),
                new ShaderCompositor.ShaderProperty("_ReflectionFresnelIntensity", "Reflection Fresnel Intensity", ShaderCompositor.ePROPERTY_TYPE.Float, ShaderCompositor.eDEFAULT_VALUE.Zero),
                new ShaderCompositor.ShaderProperty("_ReflectionFresnelPower", "Reflection Fresnel Power", ShaderCompositor.ePROPERTY_TYPE.Float, ShaderCompositor.eDEFAULT_VALUE.One),
                new ShaderCompositor.ShaderProperty("_FinalColor", "FinalColor", ShaderCompositor.ePROPERTY_TYPE.Color, ShaderCompositor.eDEFAULT_VALUE.White),
                new ShaderCompositor.ShaderProperty("_ContrastIntansity", "ContrastIntansity", ShaderCompositor.ePROPERTY_TYPE.Float, ShaderCompositor.eDEFAULT_VALUE.One),
                new ShaderCompositor.ShaderProperty("_Brightness", "Brightness", ShaderCompositor.ePROPERTY_TYPE.Float, ShaderCompositor.eDEFAULT_VALUE.Zero),
                new ShaderCompositor.ShaderProperty("_GrayScale", "GrayScale", ShaderCompositor.ePROPERTY_TYPE.Float, ShaderCompositor.eDEFAULT_VALUE.Zero),
                new ShaderCompositor.ShaderProperty("_ColorScale", "Color Scale", ShaderCompositor.ePROPERTY_TYPE.Float, ShaderCompositor.eDEFAULT_VALUE.One),
                new ShaderCompositor.ShaderProperty("_FresnelIntensity", "Intensity", ShaderCompositor.ePROPERTY_TYPE.Float, ShaderCompositor.eDEFAULT_VALUE.Zero),
                new ShaderCompositor.ShaderProperty("_FresnelPower", "Power", ShaderCompositor.ePROPERTY_TYPE.Float, ShaderCompositor.eDEFAULT_VALUE.Zero),
                new ShaderCompositor.ShaderProperty("_FresnelColor", "Color", ShaderCompositor.ePROPERTY_TYPE.Color, ShaderCompositor.eDEFAULT_VALUE.White),
                new ShaderCompositor.ShaderProperty("_Tint1", "Tint1", ShaderCompositor.ePROPERTY_TYPE.Color, ShaderCompositor.eDEFAULT_VALUE.Black),
                new ShaderCompositor.ShaderProperty("_Tint2", "Tint2", ShaderCompositor.ePROPERTY_TYPE.Color, ShaderCompositor.eDEFAULT_VALUE.Black),
                new ShaderCompositor.ShaderProperty("_Tint3", "Tint3", ShaderCompositor.ePROPERTY_TYPE.Color, ShaderCompositor.eDEFAULT_VALUE.Black),
                new ShaderCompositor.ShaderProperty("_Tint4", "Tint4", ShaderCompositor.ePROPERTY_TYPE.Color, ShaderCompositor.eDEFAULT_VALUE.Black),
                new ShaderCompositor.ShaderProperty("_TintTex", "Tint (RGBA)", ShaderCompositor.ePROPERTY_TYPE.Texture2D, ShaderCompositor.eDEFAULT_VALUE.Black),
            };

            if (isCutout)
            {
                properties.Add(new ShaderCompositor.ShaderProperty("_Cutoff", "Alpha Cutoff", ShaderCompositor.ePROPERTY_TYPE.Float, ShaderCompositor.eDEFAULT_VALUE.One));
            }

            ShaderCompositor.CategoryBlock categoryBlock = new ShaderCompositor.CategoryBlock();
            categoryBlock.Tags = new List<string>() { "\"Queue\"=\"Geometry\"", "\"RenderType\"=\"Character\"", "\"LightMode\"=\"ForwardBase\"" };
            categoryBlock.CullMode = UnityEngine.Rendering.CullMode.Back;
            categoryBlock.ZWrite = true;
            categoryBlock.ZTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            // Blend Off
            categoryBlock.SrcBlendMode = UnityEngine.Rendering.BlendMode.One;
            categoryBlock.DstBlendMode = UnityEngine.Rendering.BlendMode.Zero;

            List<string> cg100 = new List<string>()
            {
                "#include \"UnityCG.cginc\"",
                "#include \"Lighting.cginc\"",
                "#include \"AutoLight.cginc\"",
                "#include \"Assets/_GameAssets/Scripts/Game/OldScripts/Rendering/Shaders/Lib/EBG_Globals.cginc\"",
                "#pragma target 3.0",
                "#pragma vertex vertex_shader",
                "#pragma fragment fragment_shader",
                "#pragma multi_compile_fwdbase"
            };

            List<string> cg50 = new List<string>()
            {
                "#include \"UnityCG.cginc\"",
                "#include \"Assets/_GameAssets/Scripts/Game/OldScripts/Rendering/Shaders/Lib/EBG_Globals.cginc\"",
                "#pragma target 3.0",
                "#pragma vertex vertex_shader",
                "#pragma fragment fragment_shader"
            };

            List<string> stencils = new List<string>()
            {
                "Stencil {",
                "	 Ref 4",
                "    Comp always",
                "    Pass replace",
                "}"
            };

            //we only need this for LODs with normal maps

            List <ShaderCompositor.LOD> lods = null;
            if (isCutout)
            {
                lods = new List<ShaderCompositor.LOD>() {
                    new ShaderCompositor.LOD(
                        300,
                        new List<string>() { "EBG_DYNAMIC_SHADOWS_ON", "EBG_POINT_LIGHT", "EBG_TRANSPARENT", "EBG_FOG_ON", "EBG_RIM_ON" },
                        new List<string>() { "EBG_DIFFUSE_OFF", "EBG_DETAIL_OFF", "EBG_HIGHLIGHTS_IGNORE_ALPHA_OFF" },
                        cg100,
                        stencils
                    ),
                    new ShaderCompositor.LOD(
                        //Normal Maps Off
                        200,
                        new List<string>() { "EBG_POINT_LIGHT", "EBG_TRANSPARENT", "EBG_FOG_ON" },
                        new List<string>() { "EBG_DYNAMIC_SHADOWS_OFF", "EBG_RIM_OFF", "EBG_DIFFUSE_OFF", "EBG_DETAIL_OFF", "EBG_SPEC_OFF", "EBG_NORMAL_MAP_OFF", "EBG_FRESNEL_OFF", "EBG_REFLECTIONS_OFF" },
                        cg50
                    ),
                };
            }
            else
            {
                lods = new List<ShaderCompositor.LOD>() {
                    new ShaderCompositor.LOD(
                        300,
                        new List<string>() { "EBG_DYNAMIC_SHADOWS_ON", "EBG_POINT_LIGHT", "EBG_FOG_ON", "EBG_RIM_ON" },
                        new List<string>() { "EBG_DIFFUSE_OFF", "EBG_DETAIL_OFF", "EBG_ALPHA_CUTOFF_OFF" },
                        cg100,
                        stencils
                    ),
                    new ShaderCompositor.LOD(
                        //Normal Maps Off
                        200,
                        new List<string>() { "EBG_POINT_LIGHT", "EBG_FOG_ON" },
                        new List<string>() { "EBG_DYNAMIC_SHADOWS_OFF", "EBG_RIM_OFF", "EBG_DIFFUSE_OFF", "EBG_DETAIL_OFF", "EBG_ALPHA_CUTOFF_OFF", "EBG_SPEC_OFF", "EBG_NORMAL_MAP_OFF", "EBG_FRESNEL_OFF", "EBG_REFLECTIONS_OFF" },
                        cg50
                    ),
                };
            }

            char slash = System.IO.Path.DirectorySeparatorChar;
            string dir = Application.dataPath + slash + "Merge" + slash + "Shaders" + slash;
            System.IO.Directory.CreateDirectory(dir);

            string shaderName = ShaderCompositor.GenerateShaderName(material);
            string filePath = string.Empty;

            Shader composited = null;
            composited = ShaderCompositor.Composite(shaderName, material, dir, include, "EBG_", properties, categoryBlock, lods, out filePath);

            AssetDatabase.Refresh();

            return Shader.Find(composited.name);
        }
    }
}
#endif