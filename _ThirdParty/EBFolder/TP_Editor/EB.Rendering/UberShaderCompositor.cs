#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace EB.Rendering
{
	public class UberShaderCompositor  
	{
		private static UberShaderCompositor instance=null;
		public static UberShaderCompositor Instance
		{
			get
			{
				if(instance==null)
				{
					instance = new UberShaderCompositor();
				}
				return instance;
			}
		}


		public void  Convert(Material material)
		{
			int lod = material.shader.maximumLOD;     
			Shader shader = Shader.Find (ShaderCompositor.GenerateShaderName(material)); 
			if (shader == null)
			{
				shader = CompositeUberShader (material);
				shader.maximumLOD = lod;
			} 
		
			material.shader = shader;
		
		}
		
		protected virtual  Shader CompositeUberShader(Material material) 
		{
			return null;
		}
		
	
	}
	
}
#endif