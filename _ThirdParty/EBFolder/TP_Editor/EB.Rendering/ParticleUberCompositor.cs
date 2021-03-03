#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace EB.Rendering{
	public class ParticleUberCompositor: UberShaderCompositor{

		private static ParticleUberCompositor instance=null;
		public new static ParticleUberCompositor Instance
		{
			get
			{
				if(instance==null)
				{
					instance = new ParticleUberCompositor();
				}
				return instance;
			}
		}

		protected override Shader CompositeUberShader(Material material)
		{
			var include = Application.dataPath + "/Scripts/Rendering/Shaders/Particles/Lib/ParticleUber.cginc";
			
			List<ShaderCompositor.ShaderProperty> properties = new List<ShaderCompositor.ShaderProperty>() {
				new ShaderCompositor.ShaderProperty("_MainTex", 			"Diffuse Tex", 			ShaderCompositor.ePROPERTY_TYPE.Texture2D, 		ShaderCompositor.eDEFAULT_VALUE.Black),
				new ShaderCompositor.ShaderProperty("_HueShift", 			"Hue Shift", 			ShaderCompositor.ePROPERTY_TYPE.Float, 			ShaderCompositor.eDEFAULT_VALUE.Zero),
				new ShaderCompositor.ShaderProperty("_Sat", 			"Saturation", 			ShaderCompositor.ePROPERTY_TYPE.Float, 			ShaderCompositor.eDEFAULT_VALUE.Zero),
				new ShaderCompositor.ShaderProperty("_Value", 			"Value", 			ShaderCompositor.ePROPERTY_TYPE.Float, 		ShaderCompositor.eDEFAULT_VALUE.Zero),
				new ShaderCompositor.ShaderProperty("_PointLightIntensity", 			"Intensity", 	ShaderCompositor.ePROPERTY_TYPE.Float, 		ShaderCompositor.eDEFAULT_VALUE.Zero),
				new ShaderCompositor.ShaderProperty("_HeightFade", 		"Fade Out Start Height", 		ShaderCompositor.ePROPERTY_TYPE.Float, 			ShaderCompositor.eDEFAULT_VALUE.One),
				new ShaderCompositor.ShaderProperty("_GroundHeight", 	"Fade Out End Height", 			ShaderCompositor.ePROPERTY_TYPE.Float, 			ShaderCompositor.eDEFAULT_VALUE.One),
				new ShaderCompositor.ShaderProperty("_UVScroll", 			"X/Y Speed", 		ShaderCompositor.ePROPERTY_TYPE.Vector, 		ShaderCompositor.eDEFAULT_VALUE.Zero),
			
			};

			ShaderCompositor.CategoryBlock categoryBlock = new ShaderCompositor.CategoryBlock();
			categoryBlock.Tags = new List<string>() {"\"LightMode\"=\"ForwardBase\"","\"Queue\"=\"Transparent\"","\"RenderType\"=\"Transparent\""};
			categoryBlock.CullMode = UnityEngine.Rendering.CullMode.Back;
			categoryBlock.ZWrite = false;

			categoryBlock.ZTest = (UnityEngine.Rendering.CompareFunction)material.GetInt ("_DepthTest");
			categoryBlock.SrcBlendMode = (UnityEngine.Rendering.BlendMode)material.GetInt ("_SrcFactor");
			categoryBlock.DstBlendMode = (UnityEngine.Rendering.BlendMode)material.GetInt ("_DstFactor");
			categoryBlock.BlendOpMode = (UnityEngine.Rendering.BlendOp)material.GetInt ("_BlendOp");
			List<string> cgs = new List<string>() 
			{ 
				"#include \"UnityCG.cginc\"",
                "#include \"Assets/_GameAssets/Scripts/Game/OldScripts/Rendering/Shaders/Lib/EBG_Globals.cginc\"", 
				"#pragma target 3.0",
				"#pragma vertex vertex_lm", 
				"#pragma fragment fragment_lm" ,
			};
			
			//we only need this for LODs with normal maps
			
			
			List<ShaderCompositor.LOD> lods = new List<ShaderCompositor.LOD>() {
				new ShaderCompositor.LOD(
					200, 
					new List<string>() {},
					new List<string>() { },
					cgs
				),
				new ShaderCompositor.LOD(
					100, 
					new List<string>() {},
					new List<string>() { "EBG_NORMAL_MAP_OFF","EBG_FRESNEL_OFF" },
					cgs
				),
				
			};
			
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