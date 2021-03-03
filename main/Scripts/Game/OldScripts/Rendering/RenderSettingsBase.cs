using UnityEngine;

public enum eLightDir
{
    MainLand,
    Instance,
    BattleReady,
    Partner,
    RankList,
    ChallengeInstance,
    nullData,
}
[ExecuteInEditMode]
public abstract class RenderSettingsBase : MonoBehaviour 
{
	//build-time light probles
	public float LightProbeOffset = 0.0f;
	public float LightProbeScale = 1.0f;
	public float LightProbeMax = 1.0f;

	public float BlendInTime = 0.5f;
	public string Name = "";

	public bool StartActive = false;

    [SerializeField]
    private eLightDir lightDir = eLightDir.MainLand;

    //主城
    public readonly Vector4 MainLandLightDir = new Vector4(45.04f, 11.92f, 11.1f, 1);
    //副本
    public readonly Vector4 InstanceLightDir = new Vector4(-243f, 263.1f, -335.6f, 1);
    //布阵
    public readonly Vector4 BattleReadyLightDir = new Vector4(-103.7f, 65.8f, -93.6f, 1);
    //伙伴
    public readonly Vector4 PartnerLightDir = new Vector4(-2f, 1.05f, -1.59f, 1);
    //public readonly Vector4 PartnerLightDir = new Vector4(-1.86f, 1.42f, -2f, 1);
    //天梯
    public readonly Vector4 RankListLightDir = new Vector4(-2.31f, 2.88f, -2.4f, 1);
    //挑战副本
    public readonly Vector4 ChallengeInstanceLightDir = new Vector4(-243f, 263.1f, -335.6f, 1);

    public eLightDir LightDir
    {
        get
        {
            return lightDir;
        }

        set
        {
            lightDir = value;
        }
    }

    public Vector4 GetLightDir()
    {
        switch((int)LightDir)
        {
            case (int)eLightDir.MainLand:
                {
                    return MainLandLightDir;
                }
            case (int)eLightDir.Instance:
                {
                    return InstanceLightDir;
                }
            case (int)eLightDir.BattleReady:
                {
                    return BattleReadyLightDir;
                }
            case (int)eLightDir.Partner:
                {
                    return PartnerLightDir;
                }
            case (int)eLightDir.RankList:
                {
                    return RankListLightDir;
                }
            case (int)eLightDir.ChallengeInstance:
                {
                    return ChallengeInstanceLightDir;
                }
            default:
                {
                    return MainLandLightDir;
                }
        }
    }

    public virtual void Clone(RenderSettingsBase toClone)
	{
		LightProbeOffset = toClone.LightProbeOffset;
		LightProbeScale = toClone.LightProbeScale;
		LightProbeMax = toClone.LightProbeMax;
	}

	public abstract void ApplyAtSceneLoad();
	public abstract void ApplyEveryFrame();
	public abstract void ApplyBlend (RenderSettingsBase source, RenderSettingsBase dest, float destFactor);

	public virtual void Start()
	{
		if (StartActive) {
			ApplyAtSceneLoad();
		}
	}
}
