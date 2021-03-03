using UnityEngine;
using System.Collections;

public class EdgeDetectNormalsAndDepth : PostEffectsBase {

	public Shader edgeDetectShader;
    [SerializeField]
	private Material edgeDetectMaterial = null;
	public Material material {  
		get {
			edgeDetectMaterial = CheckShaderAndCreateMaterial(edgeDetectShader, edgeDetectMaterial);
			return edgeDetectMaterial;
		}  
	}

    public Shader DepthShader;
    private Material DepthMaterial = null;
    public Material DMaterial
    {
        get
        {
            DepthMaterial = CheckShaderAndCreateMaterial(DepthShader, DepthMaterial);
            return DepthMaterial;
        }
    }

    public RenderTexture depthRT;

	[Range(0.0f, 1.0f)]
	public float edgesOnly = 0.0f;

	public Color edgeColor = Color.black;

	public Color backgroundColor = Color.white;

	public float sampleDistance = 1.0f;

	public float sensitivityDepth = 1.0f;

	public float sensitivityNormals = 1.0f;

    public float DepthTexture;

	void OnEnable() {
		GetComponent<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;
	    depthRT = new RenderTexture(2730,1536,0);
    }

    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material != null)
        {
            material.SetFloat("_EdgeOnly", edgesOnly);
            material.SetColor("_EdgeColor", edgeColor);
            material.SetColor("_BackgroundColor", backgroundColor);
            material.SetFloat("_SampleDistance", sampleDistance);
            material.SetVector("_Sensitivity", new Vector4(sensitivityNormals, sensitivityDepth, 0.0f, 0.0f));

            Graphics.Blit(src, dest, material);
            Graphics.Blit(src, depthRT, DMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }

    //[ImageEffectOpaque]
    //void OnRenderImage(RenderTexture src, RenderTexture dest)
    //{
    //    if (DepthShader != null)
    //    {
    //        depthRT = new RenderTexture(2530,1680,1);
    //        Graphics.Blit(src, depthRT, material);
    //    }
    //    else
    //    {
    //        Graphics.Blit(src, dest);
    //    }
    //}
}
