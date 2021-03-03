﻿//#define SHADER_COMPOSITOR_DEBUG

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace EB.Rendering
{
	public class ShaderCompositor 
	{
		public enum ePROPERTY_TYPE
		{
			Color,
			Float,
			Texture2D,
			TextureCube,
			Vector
		}
		
		public enum eDEFAULT_VALUE
		{
			Black,
			White,
			Bump,
			One,
			Zero
		}

		public struct ShaderProperty
		{
			public string id;
			public string name;
			public ePROPERTY_TYPE type;
			public eDEFAULT_VALUE defaultValue;

			public ShaderProperty(string _id, string _name, ePROPERTY_TYPE _type, eDEFAULT_VALUE _defaultValue)
			{
				id = _id;
				name = _name;
				type = _type;
				defaultValue = _defaultValue;
			}
		}

		public struct CategoryBlock
		{
			public List<string> Tags;
			public UnityEngine.Rendering.CullMode CullMode;
			public UnityEngine.Rendering.BlendMode SrcBlendMode;
			public UnityEngine.Rendering.BlendMode DstBlendMode;
			public UnityEngine.Rendering.BlendOp BlendOpMode;

			public bool ZWrite;
			public UnityEngine.Rendering.CompareFunction ZTest;
		}

		public struct LOD
		{
			public int lod;
			public List<string> defines;
			public List<string> features;
			public List<string> cgs;
			public List<string> stencils;

			public LOD(int _lod, List<string> _defines, List<string> _features, List<string> _cgs, List<string> _stencils = null)
			{
				lod = _lod;
				defines = _defines;
				features = _features;
				cgs = _cgs;
				stencils = _stencils;
			}
		}

		public static string GenerateShaderName(Material material)
		{
			string shaderName = material.shader.name;
			
			shaderName = shaderName.Replace("EBG/","Hidden/");
			string[] keywords = material.shaderKeywords.Where(keyword => !keyword.StartsWith("_")).ToArray();
			System.Array.Sort (keywords);

			if (keywords.Length == 0)
			{
				shaderName += "_BASE";
			}
			else
			{
				shaderName += "_" + string.Join("_", keywords);
			}
			
			if (material.HasProperty("_DepthTest"))
			{
				shaderName += "_" + material.GetFloat("_DepthTest");
			}
			
			if (material.HasProperty("_SrcFactor"))
			{
				shaderName += "_" + material.GetFloat("_SrcFactor");
			}
			
			if (material.HasProperty("_DstFactor"))
			{
				shaderName += "_" + material.GetFloat("_DstFactor");
			}

			if (material.HasProperty("_BlendOp"))
			{
				shaderName += "_" + material.GetFloat("_BlendOp");
			}

			return shaderName;
		}
		
		public static Shader Composite(Material material, string dir, string includeFile, string keywordPrefix, List<ShaderProperty> properties, CategoryBlock categoryBlock, List<LOD> lods)
		{
			string filePath = string.Empty;
			return Composite(material, dir, includeFile, keywordPrefix, properties, categoryBlock, lods, out filePath);
		}
		
		public static Shader Composite(string shaderName, Material material, string dir, string includeFile, string keywordPrefix, List<ShaderProperty> properties, CategoryBlock categoryBlock, List<LOD> lods)
		{
			string filePath = string.Empty;
			return Composite(shaderName, material, dir, includeFile, keywordPrefix, properties, categoryBlock, lods, out filePath);
		}
		
		public static Shader Composite(Material material, string dir, string includeFile, string keywordPrefix, List<ShaderProperty> properties, CategoryBlock categoryBlock, List<LOD> lods, out string filePath)
		{
			string shaderName = GenerateShaderName(material);
			return Composite(shaderName, material, dir, includeFile, keywordPrefix, properties, categoryBlock, lods, out filePath);
		}

		public static Shader Composite(string shaderName, Material material, string dir, string includeFile, string keywordPrefix, List<ShaderProperty> properties, CategoryBlock categoryBlock, List<LOD> lods, out string filePath)
		{
#if SHADER_COMPOSITOR_DEBUG
			Debug.LogWarning("Compositing shader " + shaderName + " for " + material.name);
#endif
			string shaderfilename = shaderName.Replace("/","_");
			filePath = dir + shaderfilename + ".shader";

			StreamWriter sw = File.CreateText(filePath);

			//SHADER NAME
			sw.WriteLine("Shader \"" + shaderName + "\"");
			sw.WriteLine("{");

			{
				//SHADER PROPERTIES
				CompositeProperty(sw, properties);

				//SHADER CATEGORY
				sw.WriteLine("\tCategory");
				sw.WriteLine("\t{");

				{
					//TAGS
					CompositeCategoryTag(sw, categoryBlock);

					//SUBSHADERS
					CompositeCategorySubshader(sw, material, includeFile, lods, false);
				}

				sw.WriteLine("\t}");

				//FALLBACK
				if ((categoryBlock.SrcBlendMode == UnityEngine.Rendering.BlendMode.One) && (categoryBlock.DstBlendMode == UnityEngine.Rendering.BlendMode.Zero))
				{
					sw.WriteLine("\tFallBack \"Diffuse\"");
				}
			}            

			sw.WriteLine("}");
			sw.Close();

			//trigger a refresh due to newly saved shader
			AssetDatabase.Refresh();

			Shader result = Shader.Find(shaderName);
			if (result == null)
			{
				UnityEngine.Debug.LogError("Something went wrong compositing " + shaderName);
			}

			return result;
		}

		public static Shader CompositeOcclusion(string shaderName, Material material, string dir, string includeFile, string keywordPrefix, List<ShaderProperty> properties, CategoryBlock categoryBlock, List<LOD> lods, out string filePath)
		{
#if SHADER_COMPOSITOR_DEBUG
			UnityEngine.Debug.LogWarning("Compositing shader " + shaderName + " for " + material.name);
#endif
			string shaderfilename = shaderName.Replace("/","_");
			filePath = dir + shaderfilename + ".shader";

			StreamWriter sw = File.CreateText(filePath);

			//SHADER NAME
			sw.WriteLine("Shader \"" + shaderName + "\"");
			sw.WriteLine("{");

			{
				//SHADER PROPERTIES
				CompositeProperty(sw, properties);

				//SHADER CATEGORY
				sw.WriteLine("\tCategory");
				sw.WriteLine("\t{");

				{
					//TAGS
					CompositeCategoryTag(sw, categoryBlock);

					//SUBSHADERS
					CompositeCategorySubshader(sw, material, includeFile, lods, true);
				}

				sw.WriteLine("\t}");

				//FALLBACK
				if ((categoryBlock.SrcBlendMode == UnityEngine.Rendering.BlendMode.One) && (categoryBlock.DstBlendMode == UnityEngine.Rendering.BlendMode.Zero))
				{
					sw.WriteLine("\tFallBack \"Diffuse\"");
				}
			}

			sw.WriteLine("}");
			sw.Close();

			//trigger a refresh due to newly saved shader
			AssetDatabase.Refresh();

			Shader result = Shader.Find(shaderName);
			if (result == null)
			{
				UnityEngine.Debug.LogError("Something went wrong compositing " + shaderName);
			}

			return result;
		}

		static void CompositeProperty(StreamWriter sw, List<ShaderProperty> properties)
		{
			sw.WriteLine("\tProperties");
			sw.WriteLine("\t{");
			foreach (ShaderProperty property in properties)
			{
				//_NDotLWrap("N.L Wrap", Float) = 0
				sw.Write("\t\t" + property.id + "(\"" + property.name + "\", ");
				switch (property.type)
				{
					case (ePROPERTY_TYPE.Color):
					case (ePROPERTY_TYPE.Float):
					case (ePROPERTY_TYPE.Vector):
						sw.Write(property.type.ToString());
						break;
					case (ePROPERTY_TYPE.Texture2D):
						sw.Write("2D");
						break;
					case (ePROPERTY_TYPE.TextureCube):
						sw.Write("Cube");
						break;
				}

				sw.Write(") = ");

				switch (property.type)
				{
					case (ePROPERTY_TYPE.Color):
						switch (property.defaultValue)
						{
							case (eDEFAULT_VALUE.Black):
								sw.WriteLine("(0, 0, 0, 0)");
								break;
							case (eDEFAULT_VALUE.White):
								sw.WriteLine("(1, 1, 1, 1)");
								break;
							default:
								UnityEngine.Debug.LogError("Invalid default value for " + property.name);
								break;
						}
						break;
					case (ePROPERTY_TYPE.Float):
						switch (property.defaultValue)
						{
							case (eDEFAULT_VALUE.Zero):
								sw.WriteLine("0");
								break;
							case (eDEFAULT_VALUE.One):
								sw.WriteLine("1");
								break;
							default:
								UnityEngine.Debug.LogError("Invalid default value for " + property.name);
								break;
						}
						break;
					case (ePROPERTY_TYPE.Vector):
						switch (property.defaultValue)
						{
							case (eDEFAULT_VALUE.Zero):
								sw.WriteLine("(0, 0, 0, 0)");
								break;
							case (eDEFAULT_VALUE.One):
								sw.WriteLine("(1, 1, 1, 1)");
								break;
							default:
								UnityEngine.Debug.LogError("Invalid default value for " + property.name);
								break;
						}
						break;
					case (ePROPERTY_TYPE.Texture2D):
					case (ePROPERTY_TYPE.TextureCube):
						switch (property.defaultValue)
						{
							case (eDEFAULT_VALUE.Black):
								sw.WriteLine("\"black\" {}");
								break;
							case (eDEFAULT_VALUE.White):
								sw.WriteLine("\"white\" {}");
								break;
							case (eDEFAULT_VALUE.Bump):
								sw.WriteLine("\"bump\" {}");
								break;
							default:
								UnityEngine.Debug.LogError("Invalid default value for " + property.name);
								break;
						}
						break;
				}
			}
			sw.WriteLine("\t}");
		}

		static void CompositeCategoryTag(StreamWriter sw, CategoryBlock categoryBlock)
		{
			sw.WriteLine("\t\tTags");
			sw.WriteLine("\t\t{");
			foreach (string tag in categoryBlock.Tags)
			{
				sw.WriteLine("\t\t\t" + tag);
			}
			sw.WriteLine("\t\t}");
			sw.WriteLine("\t\tLighting Off");
			sw.WriteLine("\t\tFog { Mode Off }");
			sw.WriteLine("\t\tCull " + categoryBlock.CullMode.ToString());
			sw.WriteLine("\t\tZWrite " + (categoryBlock.ZWrite ? "On" : "Off"));
			switch (categoryBlock.ZTest)
			{
				case (UnityEngine.Rendering.CompareFunction.Always):
				case (UnityEngine.Rendering.CompareFunction.Equal):
				case (UnityEngine.Rendering.CompareFunction.Greater):
				case (UnityEngine.Rendering.CompareFunction.Less):
				case (UnityEngine.Rendering.CompareFunction.Never):
				case (UnityEngine.Rendering.CompareFunction.NotEqual):
					sw.WriteLine("\t\tZTest " + categoryBlock.ZTest.ToString());
					break;
				case (UnityEngine.Rendering.CompareFunction.LessEqual):
					sw.WriteLine("\t\tZTest LEqual");
					break;
				case (UnityEngine.Rendering.CompareFunction.GreaterEqual):
					sw.WriteLine("\t\tZTest GEqual");
					break;
				case (UnityEngine.Rendering.CompareFunction.Disabled):
					sw.WriteLine("\t\tZTest Always");
					break;
			};
			if ((categoryBlock.SrcBlendMode == UnityEngine.Rendering.BlendMode.One) && (categoryBlock.DstBlendMode == UnityEngine.Rendering.BlendMode.Zero))
			{
				sw.WriteLine("\t\tBlend Off");
			}
			else
			{
				sw.WriteLine("\t\tBlend " + categoryBlock.SrcBlendMode + " " + categoryBlock.DstBlendMode);
				sw.WriteLine("\t\tBlendOp " + categoryBlock.BlendOpMode);
			}
		}

		static void CompositeCategorySubshader(StreamWriter sw, Material material, string includeFile, List<LOD> lods, bool occlusion)
		{
			lods.Sort((l1, l2) => l2.lod.CompareTo(l1.lod));

			for (int i = 0; i < lods.Count; ++i)
			{
				//try to skip any LODs that would compile the same thing as the next set
				if (i < lods.Count - 1)
				{
					bool identical = true;

					LOD current = lods[i];
					LOD next = lods[i+1];


					//make sure our defines are the same length
					identical &= (current.defines.Count == next.defines.Count);

					//make sure all defines in current are in next
					foreach (string define in current.defines)
					{
						identical &= next.defines.Contains(define);
					}

					foreach (string keyword in material.shaderKeywords)
					{
						if (keyword.EndsWith("_ON"))
						{
							string keywordOff = keyword.Replace("_ON", "_OFF");
							if (current.features.Contains(keywordOff))
							{
								if (!next.features.Contains(keywordOff))
								{
									identical = false;
								}
							}
							else if (next.features.Contains(keywordOff))
							{
								if (!current.features.Contains(keywordOff))
								{
									identical = false;
								}
							}
						}
					}

					if (identical)
					{
						continue;
					}
				}

				LOD lod = lods[i];
				sw.WriteLine("\t\tSubshader");
				sw.WriteLine("\t\t{");
				sw.WriteLine("\t\t\tLOD " + lod.lod);

				//OCCLUSION PASS
				if (occlusion)
				{
					sw.WriteLine("\t\t\tPass");
					sw.WriteLine("\t\t\t{");

					{
						sw.WriteLine("\t\t\t\tZTest Greater");
						sw.WriteLine("\t\t\t\tZWrite Off");
						sw.WriteLine("\t\t\t\tBlend SrcAlpha OneMinusSrcAlpha");
						sw.WriteLine("\t\t\t\tCGPROGRAM");

						foreach (string cg in lod.cgs)
						{
							sw.WriteLine("\t\t\t\t" + cg);
						}

						lod.defines.Add("EBG_OCCLUSION");
						var shader = CompositeMaterial(material, includeFile, "EBG_", lod.defines, lod.features);
						foreach (string line in shader.Split('\n'))
						{
							sw.WriteLine("\t\t\t\t" + line);
						}
						lod.defines.Remove("EBG_OCCLUSION");
						sw.WriteLine("\t\t\t\tENDCG");
					}

					sw.WriteLine("\t\t\t}");
				}

				//PASS
				{
					sw.WriteLine("\t\t\tPass");
					sw.WriteLine("\t\t\t{");

					{
						if (lod.stencils != null)
						{
							foreach (string stencil in lod.stencils)
							{
								sw.WriteLine("\t\t\t\t" + stencil);
							}
						}
						sw.WriteLine("\t\t\t\tCGPROGRAM");

						foreach (string cg in lod.cgs)
						{
							sw.WriteLine("\t\t\t\t" + cg);
						}

						var shader = CompositeMaterial(material, includeFile, "EBG_", lod.defines, lod.features);

						foreach (string line in shader.Split('\n'))
						{
							sw.WriteLine("\t\t\t\t" + line);
						}

						sw.WriteLine("\t\t\t\tENDCG");
					}

					sw.WriteLine("\t\t\t}");
				}

				sw.WriteLine("\t\t}");
			}
		}

		static string CompositeMaterial(Material material, string includeFile, string keywordPrefix, List<string> defines, List<string> features)
		{
			List<string> keywords = new List<string>();

			//add all the fixed defines
			foreach(string define in defines)
			{
				keywords.Add(define);
			}

			//add all the features that are explictely "OFF"; those that are "ON" we only want on if the material defines it so
			foreach(string feature in features)
			{
				if (feature.EndsWith("_OFF"))
				{
					keywords.Add(feature);
				}
			}

			//add all the material defines that we haven't forced
			string[] materialKeywords = material.shaderKeywords;
			foreach(string keyword in materialKeywords)
			{
				if (!keyword.StartsWith(keywordPrefix) || keywords.Contains(keyword))
				{
					continue;
				}

				if (keyword.EndsWith("_OFF") && !keywords.Contains(keyword.Replace("_OFF", "_ON")))
				{
					keywords.Add(keyword);
				}
				else if (keyword.EndsWith("_ON") && !keywords.Contains(keyword.Replace("_ON", "_OFF")))
				{
					keywords.Add(keyword);
				}
			}

			//add -D in front of all our defines
			List<string> keywordArguments = new List<string>();
			foreach(string keyword in keywords)
			{
				keywordArguments.Add("-D" + keyword);
			}

			string arguments = "-P -E " + string.Join(" ", keywordArguments.ToArray()) + " -undef " + includeFile;

#if SHADER_COMPOSITOR_DEBUG
			UnityEngine.Debug.Log(arguments);
#endif

			ShellResult result = ShellCommand("cpp", arguments);

#if SHADER_COMPOSITOR_DEBUG
				UnityEngine.Debug.Log(result.stdout);
				if (result.stderr.Length > 0) 
				{
					UnityEngine.Debug.LogError(result.stderr);
				}
#endif

			if (result.resultCode != 0)
			{
				UnityEngine.Debug.LogError(result.stderr);
			}
		
			return "//" + string.Join(" ", keywords.ToArray()) + "\n" + result.stdout;
		}
		
		public class ShellResult
		{
			public int resultCode = -1;
			public string stdout = "";
			public string stderr = "";	
		}

#if UNITY_EDITOR_WIN
		private static string FindCygwinRoot(string path)
		{
			string mintty = Path.Combine(path, "bin/mintty.exe");
			if (File.Exists(mintty))
			{
				return path;
			}

			foreach (string sub in Directory.GetDirectories(path))
			{
				if (Path.GetFileName(sub).ToLower().Contains("cygwin"))
				{
					string result = FindCygwinRoot(sub);
					if (!string.IsNullOrEmpty(result))
					{
						return result;
					}
				}
			}
			
			return string.Empty;
		}

		private static string s_cygwinRoot = string.Empty;
		private static string ScanCygwinRoot()
		{
			if (!string.IsNullOrEmpty(s_cygwinRoot))
			{
				return s_cygwinRoot;
			}

			// return [A:\\, B:\\]
			string[] drives = System.Environment.GetLogicalDrives();
			foreach (string drive in drives)
			{
				s_cygwinRoot = FindCygwinRoot(drive);
				if (!string.IsNullOrEmpty(s_cygwinRoot))
				{
					break;
				}
			}

			return s_cygwinRoot;
		}

		private static string s_extraSearch = string.Empty;
		private static void SetupCygwinSearchPath(string cygwin_root)
		{
			if (!string.IsNullOrEmpty(s_extraSearch))
			{
				return;
			}

			string origin = System.Environment.GetEnvironmentVariable("PATH");
			string[] originSearches = origin.Split(Path.PathSeparator).Select(x => x.ToLower()).ToArray();

			List<string> searches = new List<string>();
			searches.Add(Path.Combine(cygwin_root, "bin"));
			searches.Add(Path.Combine(cygwin_root, "sbin"));
			searches.Add(Path.Combine(cygwin_root, "usr\\bin"));
			searches.Add(Path.Combine(cygwin_root, "usr\\sbin"));
			searches.Add(Path.Combine(cygwin_root, "usr\\local\\bin"));

			foreach (string search in searches)
			{
				if (!originSearches.Contains(search.ToLower()))
				{
					s_extraSearch += Path.PathSeparator + search;
				}
			}

			System.Environment.SetEnvironmentVariable("PATH", origin + s_extraSearch);
		}

		private static void SetupCygwinEnvironment()
		{
			string cygwinRoot = ScanCygwinRoot();
			if (!string.IsNullOrEmpty(cygwinRoot))
			{
				SetupCygwinSearchPath(cygwinRoot);
			}
		}
#endif

		private static ShellResult ShellCommand(string command, string arguments)
		{
#if UNITY_EDITOR_WIN
			SetupCygwinEnvironment();
#endif
			ShellResult result = new ShellResult();
			
			try {	
				ProcessStartInfo processStartInfo = new ProcessStartInfo();
				processStartInfo.FileName = command;
				processStartInfo.Arguments = arguments;
				processStartInfo.RedirectStandardOutput = true;
				processStartInfo.RedirectStandardError = true;
				processStartInfo.UseShellExecute = false;
				processStartInfo.CreateNoWindow = true;
				processStartInfo.WorkingDirectory = System.IO.Directory.GetCurrentDirectory();
				Process proc = Process.Start(processStartInfo);
				
				while (true)
				{
					string outLine = proc.StandardOutput.ReadLine();
					string errLine = proc.StandardError.ReadLine();

					if (outLine == null && errLine == null)
					{
						break;
					}

					if (outLine != null && outLine.Trim() != "") 
					{
						result.stdout += outLine + "\n";
					}

					if (errLine != null && errLine.Trim() != "")
					{
						result.stderr += errLine + "\n";
					}
				}
				proc.WaitForExit();
				result.resultCode = proc.ExitCode;
			} 		
			catch(System.Exception e)
			{
				UnityEngine.Debug.LogError(e);
			}
			return result;
		}
	}
}

#endif
