using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTShowBoxCtrl : UIControllerHotfix
    {
        public override bool ShowUIBlocker { get { return true; } }
        private List<LTShowItemData> mItemDataList;
        public UIDynamicShowItem m_UIDynamicShowItem;
        public GameObject FxObj;
        public GameObject TitleObj;
        public System.Action mCallback;
        public string Title;

        private bool isReady = false;

        private bool isShowBooxFx = true;
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            m_UIDynamicShowItem = t.GetMonoILRComponent<UIDynamicShowItem>("ShowReward/RewardGrid");
            FxObj = t.FindEx("FxObj").gameObject;
            TitleObj= t.FindEx("Title").gameObject;
        }

        public override void SetMenuData(object param)
        {
            isReady = true;
            isShowBooxFx = true;
            mItemDataList = param as List<LTShowItemData>;
            if (mItemDataList == null)
            {
                isShowBooxFx = false;
                Hashtable ht = param as Hashtable;
                mItemDataList = ht["reward"] as List<LTShowItemData>;
                mCallback = ht["callback"] as System.Action;
                Title = ht["title"] as string;
            }
        }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();

            yield return new WaitUntil(() => !UIStack.Instance.IsLoadingScreenUp);
            
            InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 1.5f);
            isReady = false;
            if (isShowBooxFx)
            {
                FxObj.CustomSetActive(true);
            }
            else if (mCallback != null)
            {
                TitleObj.CustomSetActive(true);
                TitleObj.GetComponent<UILabel>().text = Title;
            }
            FusionAudio.PostEvent("UI/ShowReward");
            if (m_UIDynamicShowItem != null)
            {
                m_UIDynamicShowItem.Clear();

                yield return new WaitForSeconds(isShowBooxFx ? 1f : 0.3f);

                if (controller == null)
                {
                    yield break;
                }

                if (this.controller.gameObject != null)
                {
                    m_UIDynamicShowItem.needItemVoice = true;

                    var datas = new List<LTShowItemData>();
                    for (var i = 0; i < mItemDataList.Count; i++)
                    {
                        var item = mItemDataList[i];
                        datas.Add(new LTShowItemData(item.id, item.count, item.type, false));
                    }
                    m_UIDynamicShowItem.ShowItems(datas);
                }
            }
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            if (mCallback != null)
            {
                mCallback();
                mCallback = null;
                Title = string.Empty;
            }
            m_UIDynamicShowItem.Clear();
            FxObj.CustomSetActive(false);
            TitleObj.CustomSetActive(false);
            StopAllCoroutines();
            DestroySelf();
            yield break;
        }

        public override void OnCancelButtonClick()
        {
            if (isReady) return;
            base.OnCancelButtonClick();
        }
    }
}
