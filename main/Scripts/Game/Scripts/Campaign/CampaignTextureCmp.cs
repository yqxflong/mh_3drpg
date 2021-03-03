using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CampaignTextureCmp : MonoBehaviour
{
	public List<EventDelegate> onLoadingOver = new List<EventDelegate>();

	private string m_TextureName = string.Empty;

    private string m_LastTextureName =string.Empty;

    public string spriteName
    {
        set
        {
            if (!value.Equals(m_TextureName))
            {
                m_TextureName = value;
                UpdateTexture();
            }
        }
    }

    public UITexture target;
    
    void UpdateTexture()
    {
        if (string.IsNullOrEmpty(m_TextureName))
        {
            target.mainTexture = null;
            if (!string.IsNullOrEmpty(m_LastTextureName))
            {
                ReferencedTextureManager.ReleaseTexture(m_LastTextureName);
            }
            m_LastTextureName = m_TextureName;
        }
        if (!string.IsNullOrEmpty(m_TextureName) && !m_TextureName.Equals(m_LastTextureName))
        {
            if (!string.IsNullOrEmpty(m_LastTextureName))
            {
                ReferencedTextureManager.ReleaseTexture(m_LastTextureName);
            }

			ReferencedTextureManager.GetTexture2DAsync(m_TextureName, SetTextureAction, gameObject);
            m_LastTextureName = m_TextureName;
        }
    }

    void ReleaseTexture()
    {
        if (!string.IsNullOrEmpty(m_TextureName))
        {
			ReferencedTextureManager.ReleaseTexture(m_TextureName);
		}
    }

    public void OnDestroy()
    {
        ReleaseTexture();
    }

    #region action
    void SetTextureAction(string texName, Texture2D tex, bool bSuccessed)
    {
        if (bSuccessed)
        {
            if (target != null)
            {
                target.mainTexture = tex;
                EventDelegate.Execute(onLoadingOver);
			}
        }
        else
        {
            if (target != null)
            {
                EB.Debug.LogWarning("SetTextureAction: failed to load texture {0}, use default texture instead", texName);
                target.mainTexture = GM.TextureManager.DefaultTexture2D;
            }
        }
    }
    #endregion action
}
