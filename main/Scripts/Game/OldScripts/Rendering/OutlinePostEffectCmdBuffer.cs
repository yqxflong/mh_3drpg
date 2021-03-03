using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class OutlinePostEffectCmdBuffer : MonoBehaviour
{
    public RenderTexture renderTexture;

    private CommandBuffer commandBuffer = null;
    private Material outlineMaterial = null;

    public Shader shader = null;
    //描边prepass shader（渲染纯色贴图的shader）  
    public Shader outlineShader = null;
    //采样率  
    public float samplerScale = 2;
    //降采样  
    public int downSample = 2;
    //迭代次数  
    public int iteration = 1;
    //描边颜色  
    public Color outLineColor = Color.green;
    //描边强度  
    [Range(0.0f, 10.0f)]
    public float outLineStrength = 3.0f;
    //目标对象  
    private GameObject myTargetObject = null;
    private GameObject otherTargetObject = null;

    private Material _Material;


    void OnEnable()
    {
        if (outlineShader == null) return;
        if (shader == null) return;
        RefreshTargetObject(myTargetObject, otherTargetObject);
    }

    public void RefreshTargetObject(GameObject myTarget, GameObject OtherTarget)
    {
        myTargetObject = myTarget;
        otherTargetObject = OtherTarget;
        if (myTargetObject == null && otherTargetObject == null)
        {
            Clear();
            return;
        }
        if (outlineMaterial == null) outlineMaterial = new Material(outlineShader);
        if (_Material == null) _Material = new Material(shader);
        
        if (renderTexture == null)
            renderTexture = RenderTexture.GetTemporary(Screen.width >> downSample, Screen.height >> downSample, 0);
        //showRenderTexture = renderTexture;
        //创建描边prepass的command buffer  
        if (commandBuffer != null)
        {
            commandBuffer.Release();
        }
        else
        {
            commandBuffer = new CommandBuffer();
            commandBuffer.SetRenderTarget(renderTexture);
        }
        commandBuffer.ClearRenderTarget(true, true, Color.black);

        if (myTargetObject != null)
        {
            Renderer[] myRenderers = myTargetObject.GetComponentsInChildren<Renderer>();
            commandBufferHander(myRenderers);
        }
        if (otherTargetObject != null)
        {
            Renderer[] otherRenderers = otherTargetObject.GetComponentsInChildren<Renderer>();
            commandBufferHander(otherRenderers);
        }
    }

    private void commandBufferHander(Renderer[] renderers)
    {
        foreach (Renderer r in renderers)
        {
            if(r.gameObject.GetComponent<ParticleSystem>() != null)
            {
                continue;
            }
            SkinnedMeshRenderer sm = r as SkinnedMeshRenderer;
            if (sm != null && sm.sharedMesh != null)
            {
                for (int i = 0; i < sm.sharedMesh.subMeshCount; i++)
                {

                    commandBuffer.DrawRenderer(r, outlineMaterial, i);
                }
            }
            else
            {
                commandBuffer.DrawRenderer(r, outlineMaterial);
            }
        }
    }

    void OnDisable()
    {
        Clear();
    }

    public void OnlyClearRT()
    {
        if (renderTexture)
        {
            RenderTexture.ReleaseTemporary(renderTexture);
            renderTexture = null;
        }
    }

    public void SetDir(float dir)
    {
        if (_Material != null)
        {
            _Material.SetFloat("_Dir", dir);
        }
    }

    void Clear()
    {
        OnlyClearRT();
        if (outlineMaterial)
        {
            DestroyImmediate(outlineMaterial);
            outlineMaterial = null;
        }

        if (_Material)
        {
            DestroyImmediate(_Material);
            _Material = null;
        }
        if (commandBuffer != null)
        {
            commandBuffer.Release();
            commandBuffer = null;
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_Material && outlineMaterial && renderTexture != null && commandBuffer != null)
        {
            //通过Command Buffer可以设置自定义材质的颜色  
            outlineMaterial.SetColor("_OutlineCol", outLineColor);
            //直接通过Graphic执行Command Buffer  
            if(commandBuffer != null) Graphics.ExecuteCommandBuffer(commandBuffer);

            //对RT进行Blur处理  
            RenderTexture temp1 = RenderTexture.GetTemporary(source.width >> downSample, source.height >> downSample, 0);
            RenderTexture temp2 = RenderTexture.GetTemporary(source.width >> downSample, source.height >> downSample, 0);

            //高斯模糊，两次模糊，横向纵向，使用pass0进行高斯模糊  
            _Material.SetVector("_offsets", new Vector4(0, samplerScale, 0, 0));
            Graphics.Blit(renderTexture, temp1, _Material, 0);
            _Material.SetVector("_offsets", new Vector4(samplerScale, 0, 0, 0));
            Graphics.Blit(temp1, temp2, _Material, 0);

            //如果有叠加再进行迭代模糊处理  
            for (int i = 0; i < iteration; i++)
            {
                _Material.SetVector("_offsets", new Vector4(0, samplerScale, 0, 0));
                Graphics.Blit(temp2, temp1, _Material, 0);
                _Material.SetVector("_offsets", new Vector4(samplerScale, 0, 0, 0));
                Graphics.Blit(temp1, temp2, _Material, 0);
            }
            //用模糊图和原始图计算出轮廓图  
            _Material.SetTexture("_BlurTex", temp2);
            Graphics.Blit(renderTexture, temp1, _Material, 1);

            //轮廓图和场景图叠加  
            _Material.SetTexture("_BlurTex", temp1);
            _Material.SetFloat("_OutlineStrength", outLineStrength);
            Graphics.Blit(renderTexture, destination, _Material, 2);

            RenderTexture.ReleaseTemporary(temp1);
            RenderTexture.ReleaseTemporary(temp2);
        }
		else
		{
#if UNITY_EDITOR
			Graphics.Blit(source, destination);
#endif
		}
	}
}