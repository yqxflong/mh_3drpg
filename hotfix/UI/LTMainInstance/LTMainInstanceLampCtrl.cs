using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTMainInstanceLampCtrl : UIControllerHotfix
    {
        public LTMainInstanceLampItem[] items;
        public TweenAlpha TextureTween;

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            LobbyTexture = t.GetComponent<UITexture>("Lamp/God");
            items = new LTMainInstanceLampItem[3];
            items[0]= t.GetMonoILRComponent<LTMainInstanceLampItem>("Wish/1");
            items[1] = t.GetMonoILRComponent<LTMainInstanceLampItem>("Wish/2");
            items[2] = t.GetMonoILRComponent<LTMainInstanceLampItem>("Wish/3");

            TextureTween = t.GetComponent<TweenAlpha>("Lamp/God");
        }

        public override bool IsFullscreen()
        {
            return false;
        }
    
        public override bool ShowUIBlocker
        {
            get
            {
                return true;
            }
        }
    
        public override void SetMenuData(object param)
        {
            isInitLobby = true;
            InitUI();
            TextureTween.ResetToBeginning();
        }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            StartCoroutine(CreateGod());
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            StopAllCoroutines();
    
            if (Lobby != null)
            {
                GameObject.Destroy(Lobby.mDMono.gameObject);
                Lobby = null;
            }
    
            if (Loader != null)
            {
                EB.Assets.UnloadAssetByName("UI3DLobby", false);
                Loader = null;
            }
    
            DestroySelf();
            yield break;
        }
    
        private void InitUI()
        {
            List<LTShowItemData> datas = new List<LTShowItemData>();
            datas.Add(GetItemData(Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("prayPointOption1")));
            datas.Add(GetItemData(Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("prayPointOption2")));
            datas.Add(GetItemData(Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("prayPointOption3")));
    
            for (int i = 0; i < items.Length; i++)
            {
                items[i].SetItem(datas[i]);
            }
        }
    
        private LTShowItemData GetItemData(string strValue)
        {
            object obj = EB.JSON.Parse(strValue);
            return new LTShowItemData(obj);
        }
    
        public UITexture LobbyTexture;
        private UI3DLobby Lobby = null;
        private GM.AssetLoader<GameObject> Loader = null;
        private bool isInitLobby;
    
        private IEnumerator CreateGod()
        {
            string modelName = "P027-Variant";
            if (Lobby == null && Loader == null)
            {
                Loader = new GM.AssetLoader<GameObject>("UI3DLobby", controller.gameObject);
                UI3DLobby.Preload(modelName);
                yield return Loader;
                if (Loader.Success)
                {
                    Loader.Instance.transform.parent = LobbyTexture.transform;
                    Lobby = Loader.Instance.GetMonoILRComponent<UI3DLobby>();
                    Lobby.ConnectorTexture = LobbyTexture;
                    Lobby.SetCameraMode(1, true);
                    Lobby.VariantName = modelName;
                }
            }
            TextureTween.PlayForward();
            isInitLobby = false;
        }
    
        public override void OnCancelButtonClick()
        {
            if (isInitLobby)
            {
                // 防止在加载模型的时候按了esc退出，导致摄像机看不见主城；
                return;
            }
            base.OnCancelButtonClick();
        }
    }
}
