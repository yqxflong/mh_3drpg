using UnityEngine;
using System.Collections;

public class DialogueTextureCmp : MonoBehaviour {
    private string m_TextureName;
	public bool m_UseObjectNameAsTexture=false;
	public bool m_IsRelease = true;
	public string spriteName
	{
		set
		{
			if (value != m_TextureName)
			{
				ReleaseTexture();
				m_TextureName = value;
				UpdateTexture();
			}
		}
	}
    
	public UITexture target;

	void UpdateTexture()
	{
		target.material.SetTexture("_MainTex", null);
		target.material.SetTexture("_MaskTex", null);
		target.mainTexture = null;

		if (!string.IsNullOrEmpty(m_TextureName))
		{
			ReferencedTextureManager.GetTexture2DAsync(m_TextureName, SetTextureAction, gameObject);
		}
	}

	void ReleaseTexture()
	{
		if (!string.IsNullOrEmpty(m_TextureName))
		{
			ReferencedTextureManager.ReleaseTexture(m_TextureName);
		}
	}

	public void OnEnable()
	{
		if(m_UseObjectNameAsTexture)
		{
			spriteName = gameObject.name;
		}
	}

	public void OnDisable()
	{
		if (target != null && target.material != null)
		{
			target.material.SetTexture("_MainTex", null);
			target.material.SetTexture("_MaskTex", null);
		}

		if(m_IsRelease)ReleaseTexture();
		m_TextureName = string.Empty;
	}

	#region action
	void SetTextureAction(string texName, Texture2D tex, bool bSuccessed)
	{
		if (bSuccessed)
		{
			if (target != null && target.material != null)
			{
				target.mainTexture = tex;
				target.material.SetTexture("_MainTex", tex);
				target.MarkAsChanged();
			}
		}
		else
		{
			EB.Debug.LogWarning("SetTextureAction: failed to load texture {0}, use default texture instead", texName);
			if (target != null && target.material != null)
			{
				target.mainTexture = GM.TextureManager.DefaultTexture2D;
				target.material.SetTexture("_MainTex", GM.TextureManager.DefaultTexture2D);
				target.MarkAsChanged();
			}
		}
	}

	void SetTongDaoTextureAction(string texName, Texture2D tex, bool bSuccessed)
	{
		if (bSuccessed)
		{
			if (target != null && target.material != null)
			{
				target.material.SetTexture("_MaskTex", tex);
				target.MarkAsChanged();
			}
		}
		else
		{
			EB.Debug.Log("SetTongDaoTextureAction: failed to load texture {0}, use default texture instead", texName);
		}
	}
	#endregion action
}
