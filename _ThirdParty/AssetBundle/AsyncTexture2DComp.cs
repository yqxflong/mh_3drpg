using System.Collections;
using UnityEngine;

public class AsyncTexture2DComp : MonoBehaviour
{
	public string textureName;
	public UITexture target;
    public MeshRenderer meshRenderer;
    public UIWidget ParentGO;
    public bool isGeometry = false;

    private Material mMaterial = null;

    void Awake()
	{
		if (target == null)
		{
			target = GetComponent<UITexture>();
		}
        if (meshRenderer != null)
        {
            mMaterial = meshRenderer.material;
            mMaterial.renderQueue = isGeometry ? 2000 : 3000;
        }
	}

	void OnEnable()
	{
		if (!string.IsNullOrEmpty(textureName))
		{
            if (target != null)
            {
                target.mainTexture = null;
            }
			GM.TextureManager.GetTexture2DAsync(textureName, SetTextureAction, gameObject);
		}
	}

	void OnDisable()
	{
		if (!string.IsNullOrEmpty(textureName))
		{
			GM.TextureManager.ReleaseTexture(textureName);
		}
	}

    IEnumerator SetMaterialRect(Texture2D tex, bool bSuccessed)
    {
        Vector3 parentWidgetVec = Vector3.zero;
        if (ParentGO != null)
        {
            while (ParentGO.width <= 10)
            {
                yield return null;
            }
            parentWidgetVec = new Vector3(ParentGO.width, ParentGO.height, 0);
        }
        if (bSuccessed)
        {
            if (mMaterial != null)
            {
                mMaterial.mainTexture = tex;
                meshRenderer.transform.localScale = parentWidgetVec;
            }
        }
        else
        {
            if (mMaterial != null)
            {
                mMaterial.mainTexture = GM.TextureManager.DefaultTexture2D;
                meshRenderer.transform.localScale = parentWidgetVec;
            }
        }
        yield break;
    }

    void SetTextureAction(string texName, Texture2D tex, bool bSuccessed)
	{
		if ((!gameObject.activeSelf) && bSuccessed)
		{
			GM.TextureManager.ReleaseTexture(texName);
			return;
		}

        if (mMaterial != null)
        {
            StartCoroutine(SetMaterialRect(tex, bSuccessed));
        }
        else
        {
            if (bSuccessed)
            {
                if (target != null)
                {
                    target.mainTexture = tex;
                }
            }
            else
            {
                EB.Debug.LogWarning("SetTextureAction: failed to load texture {0}, use default texture instead", texName);
                if (target != null)
                {
                    target.mainTexture = GM.TextureManager.DefaultTexture2D;
                }
            }
        }
        
	}
}
