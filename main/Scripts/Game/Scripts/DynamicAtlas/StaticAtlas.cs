using UnityEngine;

public class StaticAtlas
{
    private const string HeroHeadAtlas = "Partner_Head";
    private const string HeadFrameAtlas = "Install_Head_Frame";
    public string HeroHeadAtlasPath = "_GameAssets/Res/Textures/UI/Atlas/Partnet_Head/";
    public string HeadFrameAtlasPath = "_GameAssets/Res/Textures/UI/Atlas/Partnet_HeadFrame/";

    public bool IsHeadAtlas(string spriteName)
    {
        return IsHeroHeadAtlas(spriteName) || IsHeadFrameAtlas(spriteName);
    }
    private bool IsHeroHeadAtlas(string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName))
        {
            return false;
        }

        if (spriteName.IndexOf(HeroHeadAtlas) == -1)
        {
            return false;
        }
        return true;
    }
    private bool IsHeadFrameAtlas(string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName))
        {
            return false;
        }

        if (spriteName.IndexOf(HeadFrameAtlas) == -1)
        {
            return false;
        }
        return true;
    }

    private bool _isLoadingHeroHead =false;
    private bool _isLoadingHeadFrame = false;
    
    private UIAtlas _altasHead = null, _atlasFrame = null;

    private System.Action<bool> _fnHead = null, _fnFrame = null;

    public void LoadStaticAtlas(string spriteName, System.Action<bool> fn = null)
    {
        if (IsHeroHeadAtlas(spriteName))
        {
            if(_altasHead)
            {
               fn(true);
            }
            else if(!_isLoadingHeroHead){
                _isLoadingHeroHead = true;
                _fnHead += fn;
                EB.Assets.LoadAsync($"{HeroHeadAtlasPath}{HeroHeadAtlas}", typeof(GameObject), o=>
                {
                    var go = o as GameObject;
                    _altasHead = go.GetComponent<UIAtlas>();
                    _fnHead?.Invoke(true);
                    _fnHead = null;
                });
            }
            else{//loading
                _fnHead += fn;
            }
        }
        else if(IsHeadFrameAtlas(spriteName))
        {
            if(_atlasFrame)
            {
                fn(true);
            }
            else if(!_isLoadingHeadFrame){
                _isLoadingHeadFrame = true;
                _fnFrame += fn;
                EB.Assets.LoadAsync($"{HeadFrameAtlasPath}{HeadFrameAtlas}", typeof(GameObject), o=>
                {
                    var go = o as GameObject;
                    _atlasFrame = go.GetComponent<UIAtlas>();
                    _fnFrame?.Invoke(true);
                    _fnFrame = null;
                });
            }
            else{//loading
                _fnFrame += fn;
            }
        }
    }

    /// <summary>
    /// 没加载完时取，自己判空!!
    /// </summary>
    /// <param name="spriteName"></param>
    /// <returns></returns>
    public UIAtlas GetStaticAtlas(string spriteName)
    {
        if (IsHeroHeadAtlas(spriteName))
        {
            return _altasHead;
        }
        else if (IsHeadFrameAtlas(spriteName))
        {
            return _atlasFrame;
        }
        else
        {
            return null;
        }
    }
}
