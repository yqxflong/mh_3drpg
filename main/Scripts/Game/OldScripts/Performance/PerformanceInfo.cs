using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// WARNING: THIS CLASS IS GENERATED
// Update it by modifying performance configuration on sparx, then using codegen functionality
/// <summary>
/// 性能信息配置信息
/// </summary>
public class PerformanceInfo
{
	public enum ePLATFORM
	{
		iOS = 0,
		Android = 1,
	}
	
	public const int ePLATFORM_COUNT = 2;
	
	public enum eENVIRONMENT
	{
		CityView = 0,
		MapView = 1,
		CampaignView = 2,
		CombatView = 3,
		InventoryView= 4,
		MainLandView=5,
        InstanceView=6,
	}
	
	public const int eENVIRONMENT_COUNT = 7;

	public enum eMSAA
	{
		x1 = 0,
		x2 = 2,
		x4 = 4,
		x8 = 8,
	}

	public const int eMSAA_COUNT = 4;

	public enum eANISOTROPIC
	{
		Disable = 0,
		Enable = 1,
		ForceEnable = 2,
	}

	public const int eANISOTROPIC_COUNT = 3;

	public enum ePARTICLE_QUALITY
	{
		//Off = -1,
		Low = 0,
		Med = 1,
		High = 2,
	}

	public const int ePARTICLE_QUALITY_COUNT = 3;

	public enum ePOSTFX_QUALITY
	{
		Off = -2,
		Low = -1,
		Medium = 0,
		High = 1,
	}

	public const int ePOSTFX_QUALITY_COUNT = 4;

	public enum eTRAIL_QUALITY
	{
		Off = -1,
		Low = 0,
		Medium = 1,
		High = 2,
	}

	public const int eTRAIL_QUALITY_COUNT = 4;

	public enum eSHADOW_QUALITY
	{
		Off = 0,
		On = 1,
	}

	public const int eSHADOW_QUALITY_COUNT = 2;

	[System.Flags]
	public enum ePOSTFX
	{
		// Don't remove these
		None1 = 1,
		None2 = 2,
		// End Don't remove these
		Bloom = 4,
		Vignette = 8,
		Vignette2 = 16,
		FakeVignette = 32,
		Warp = 64,
		ToneMap = 128,
		ColorGrade = 256,
		RadialBlur = 512,
	}
	
	public const int ePOSTFX_COUNT = 8;

	public enum eREFLECTION_QUALITY
	{
		Off = -1,
		Low = 0,
		High = 1,
	}

	public const int eREFLECTION_QUALITY_COUNT = 3;

	public enum eBLEND_WEIGHTS
	{
		OneBone = 1,
		TwoBones = 2,
		FourBones = 4,
	}

	public const int eBLEND_WEIGHTS_COUNT = 3;

	public enum eFLARE_QUALITY
	{
		Off = -1,
		Low = 0,
		High = 1,
	}

	public const int eFLARE_QUALITY_COUNT = 3;

	public enum eVFX_QUALITY
	{
		OFF = 0,
		SD = 1,
		HD = 2,
	}

	public const int eVFX_QUALITY_COUNT = 3;


	public string ProfileName = "unloaded";
    /// <summary>
    /// CPU的简况名称
    /// </summary>
	public string CpuProfileName = "unloaded";
	public string ProfileId = "unloaded";

	public class EnvironmentInfo
	{
		public eMSAA msaa;
		public eANISOTROPIC aniso;
		public int lod;
		public int hiddenLayers;
		public ePARTICLE_QUALITY particleQuality;
		public eREFLECTION_QUALITY reflectionQuality;
		public int reflectedLayers;
		public eTRAIL_QUALITY trailQuality;
		public eSHADOW_QUALITY shadowQuality;
		public ePOSTFX_QUALITY postFXQuality;
		public ePOSTFX[] postFX;
		public eBLEND_WEIGHTS blendWeights;
		public bool limitFEAnimation;
		public eFLARE_QUALITY flareQuality;
		public eVFX_QUALITY vfxQuality;
		public bool slowDevice;

		public void SetDefaults()
		{
			msaa = eMSAA.x4;
			aniso = eANISOTROPIC.Enable;
			lod = 1300;
			hiddenLayers = 0;
			particleQuality = ePARTICLE_QUALITY.High;
			reflectionQuality = eREFLECTION_QUALITY.High;
			reflectedLayers = 10240;
			trailQuality = eTRAIL_QUALITY.High;
			shadowQuality = eSHADOW_QUALITY.On;
			postFXQuality = ePOSTFX_QUALITY.High;
			postFX = new ePOSTFX[] { };
			blendWeights = eBLEND_WEIGHTS.FourBones;
			limitFEAnimation = false;
			flareQuality = eFLARE_QUALITY.Off;
			vfxQuality = eVFX_QUALITY.HD;
			slowDevice = false;
		}

		public void SetCombatSceneDefaults()
		{
			msaa = eMSAA.x4;
			aniso = eANISOTROPIC.Enable;
			lod = 500;
			shadowQuality = eSHADOW_QUALITY.Off;
			postFXQuality = ePOSTFX_QUALITY.Low;
			vfxQuality = eVFX_QUALITY.HD;
			trailQuality = eTRAIL_QUALITY.High;
			reflectionQuality = eREFLECTION_QUALITY.High;
			flareQuality = eFLARE_QUALITY.High;
			particleQuality = ePARTICLE_QUALITY.High;
			blendWeights = eBLEND_WEIGHTS.FourBones;
		}
	}
	
	public EnvironmentInfo[] Environments;
	
	private Dictionary<string, eENVIRONMENT> scenesToEnvironment = new Dictionary<string, eENVIRONMENT>();
	
	public eENVIRONMENT EnvironmentForScene(string sceneName)
	{
		if (scenesToEnvironment.ContainsKey(sceneName))
		{
			return scenesToEnvironment[sceneName];
		}
		EB.Debug.Log("PeformanceInfo:EnvironmentForScene scene not found: {0}", sceneName);
		return (eENVIRONMENT)0;
	}
	
	public EnvironmentInfo EnvironmentInfoForScene(string sceneName)
	{
        eENVIRONMENT environment = eENVIRONMENT.MainLandView;
        if (scenesToEnvironment.ContainsKey(sceneName))
        {
            environment = scenesToEnvironment[sceneName];
        }
		return Environments[(int)environment];
	}
	
	public PerformanceInfo(object data)
	{
		// parse the profiles
		object profile = EB.Dot.Object("profile", data, null);

		ProfileName = EB.Dot.String("name", profile, "no name");//机型等级
		ProfileId = EB.Dot.String("_id", profile, "no id");//机器唯一id
		CpuProfileName = EB.Dot.String("cpu", profile, "High");//cpu等级
        EB.Sparx.PerformanceManager.PerformanceCurSetting = CpuProfileName;
        UserData.UserQualitySet = CpuProfileName;
        UserData.SerializePrefs();

        Environments = new EnvironmentInfo[eENVIRONMENT_COUNT];
		foreach (var e in System.Enum.GetValues(typeof(eENVIRONMENT)))//设置环境
		{
			object entry = EB.Dot.Object(e.ToString(), profile, null);
			if (entry == null)
			{
				Environments[(int)e] = new EnvironmentInfo();
				Environments[(int)e].SetDefaults();
				continue;
			}
			Environments[(int)e] = new EnvironmentInfo();
			Environments[(int)e].msaa = (eMSAA)EB.Dot.Integer("aa", entry, (int)eMSAA.x4);//多重采样抗锯齿
            Environments[(int)e].aniso = (eANISOTROPIC)EB.Dot.Integer("aniso", entry, (int)eANISOTROPIC.Enable);//各向异性
            Environments[(int)e].lod = EB.Dot.Integer("l", entry, 1300);//多细节层次
            Environments[(int)e].hiddenLayers = EB.Dot.Integer("hl", entry, 0);//隐藏层
			Environments[(int)e].particleQuality = (ePARTICLE_QUALITY)EB.Dot.Integer("pq", entry, (int)ePARTICLE_QUALITY.High);//粒子质量
			Environments[(int)e].reflectionQuality = (eREFLECTION_QUALITY)EB.Dot.Integer("rlq", entry, (int)eREFLECTION_QUALITY.High);//反射质量
			Environments[(int)e].reflectedLayers = EB.Dot.Integer("rl", entry, 10240);//反射层次
			Environments[(int)e].trailQuality = (eTRAIL_QUALITY)EB.Dot.Integer("tr", entry, (int)eTRAIL_QUALITY.High);//细节质量
			Environments[(int)e].shadowQuality = (eSHADOW_QUALITY)EB.Dot.Integer("sh", entry, (int)eSHADOW_QUALITY.On);//阴影质量
			Environments[(int)e].postFXQuality = (ePOSTFX_QUALITY)EB.Dot.Integer("pfxq", entry, (int)ePOSTFX_QUALITY.High);//后期效果质量
			var pfxArray = EB.Dot.Array("pfx", entry, null);//后期效果
			if (pfxArray != null)
			{
				Environments[(int)e].postFX = new ePOSTFX[pfxArray.Count];
				for (var j = 0; j < pfxArray.Count; ++j)
				{
					Environments[(int)e].postFX[j] = (ePOSTFX)System.Convert.ToInt32(pfxArray[j]);
				}
			}
			else
			{
				Environments[(int)e].postFX = new ePOSTFX[] { };
			}

			//强制调整骨骼融合权重为4
			Environments[(int)e].blendWeights = eBLEND_WEIGHTS.FourBones;  //(eBLEND_WEIGHTS)EB.Dot.Integer("bw", entry, (int)eBLEND_WEIGHTS.FourBones);//混合重量
			Environments[(int)e].limitFEAnimation = EB.Dot.Bool("fe", entry, false);//限制FE动画
			Environments[(int)e].flareQuality = (eFLARE_QUALITY)EB.Dot.Integer("lfq", entry, (int)eFLARE_QUALITY.Off);//闪光质量
			Environments[(int)e].vfxQuality = (eVFX_QUALITY)EB.Dot.Integer("vfx", entry, (int)eVFX_QUALITY.HD);//视觉特效
			Environments[(int)e].slowDevice = EB.Dot.Bool("slow", entry, false);//缓存

            //服务器hud不知道为何字段——EB.Dot.Integer("hud", entry, 2);

        }

        // parse the scenes

        scenesToEnvironment.Clear();

		var scenes = EB.Dot.Array("scenes", data, null);

		if (scenes != null)
		{
			foreach (object scene in scenes)
			{
				string sceneName = EB.Dot.String("name", scene, string.Empty);
				int sceneEnvironment = EB.Dot.Integer("env", scene, -1);
				if (sceneName != string.Empty && sceneEnvironment != -1 && sceneEnvironment < eENVIRONMENT_COUNT)
				{
					scenesToEnvironment[sceneName] = (eENVIRONMENT)sceneEnvironment;
				}
			}

            if (!scenesToEnvironment.ContainsKey("InstanceView"))
            {
                scenesToEnvironment["InstanceView"] = scenesToEnvironment["MainLandView"];
            }
		}
	}
}
