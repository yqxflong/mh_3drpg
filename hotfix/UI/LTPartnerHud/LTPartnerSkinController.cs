using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTPartnerSkinController : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            mScroll = t.GetMonoILRComponent<LTPartnerSkinScroll>("Scroll/PlaceHolder/Grid");
            mBtn = t.GetComponent<UIButton>("Btn");
            mBtnSprite = t.GetComponent<UISprite>("Btn/BtnSprite");
            mBtnLabel = t.GetComponent<UILabel>("Btn/Label");
            CenterOnChild = t.GetComponent<UICenterOnChild>("Scroll/PlaceHolder/Grid");

            t.Find("Scroll").GetComponent<UIPanel>().sortingOrder = t.parent.GetComponent<UIPanel>().sortingOrder + 1;
            t.Find("Scroll").GetComponent<UIPanel>().depth = t.parent.GetComponent<UIPanel>().depth + 1;
            if (mBtn.onClick .Count == 0)
            {
                mBtn.onClick.Add(new EventDelegate (OnSkinBtnClick));
            }
            t.GetComponent<ConsecutiveClickCoolTrigger>("Scroll/PlaceHolder/Grid/Item").clickEvent.Add(new EventDelegate(t.GetComponent<UICenterOnClick>("Scroll/PlaceHolder/Grid/Item").OnClick));
        }


    
        public LTPartnerSkinScroll mScroll;
        public UIButton mBtn;
        public UISprite mBtnSprite;
        public UILabel mBtnLabel;
        public UICenterOnChild CenterOnChild;
    
        private int heroId;
    
    
        public void Show(LTPartnerData data,bool isshowAwaken = false)
        {
            CenterOnChild.onCenter += OnCenter;
    
            var temp = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroAwakeInfoByInfoID(data.HeroInfo.id);
            if(temp==null||string.IsNullOrEmpty (temp.awakeSkin))//data.HeroInfo .role_grade != (int)PartnerGrade.SSR)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText,EB .Localizer .GetString("ID_PARTNER_SKIN_AWAKEN_TIP2"));
                return;
            }
            mDMono.gameObject.CustomSetActive(true);
    
            heroId = data.HeroId;
            var list =CreakSkinDataList(data);
            mScroll.SetItemDatas(list.ToArray());
            curObj = null;
            if (isshowAwaken) StartCoroutine(InitSelectIE(1));
            else StartCoroutine(InitSelectIE(data.CurSkin));
        }
    
        IEnumerator InitSelectIE(int curSkin)
        {
            yield return null;
            if(mDMono.gameObject .activeSelf)InitSelect(curSkin);
        }
    
        public void Hide()
        {
            CenterOnChild.onCenter -= OnCenter;
            mDMono.gameObject.CustomSetActive(false);
        }
    
        private List<LTLTPartnerSkinItemData> CreakSkinDataList(LTPartnerData hero)
        {
            List<LTLTPartnerSkinItemData> list = new List<LTLTPartnerSkinItemData>();
            var heroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(hero.InfoId);
            //原皮
            list.Add(new LTLTPartnerSkinItemData(hero.HeroId,0,EB.Localizer .GetString("ID_PARTNER_SKIN_DEFAULT"), heroInfo.skin));
            //觉醒皮
            list.Add(new LTLTPartnerSkinItemData(hero.HeroId, 1, EB.Localizer.GetString("ID_PARTNER_SKIN_AWAKEN"), string.Format ("{0}_1", heroInfo.skin)));
            //氪金皮
            //list.AddRange();
            return list;
        }
    
        private void InitSelect(int index)
        {
            if (index < mScroll.activates.Count)
            {
                CenterOnChild.CenterOn(mScroll.activates[index].transform);
            }
        }
    
        private void SetGetTipLabel(LTPartnerData partner, int index)
        {
            if(partner.CurSkin == index)
            {
                mBtn.enabled = false;
                mBtnSprite.color = Color.magenta;
                mBtnLabel.text = EB.Localizer.GetString("ID_PARTNER_AWAKEN_BTN_3");//当前穿戴
            }
            else if (index == 0)
            {
                mBtn.enabled = true;
                mBtnSprite.color = Color.white;
                mBtnLabel.text = EB.Localizer.GetString("ID_PARTNER_AWAKEN_BTN_2");//穿戴
            }
            else if (index == 1)
            {
                if (partner.IsAwaken > 0)
                {
                    mBtn.enabled = true;
                    mBtnSprite.color = Color.white;
                    mBtnLabel.text = EB.Localizer.GetString("ID_PARTNER_AWAKEN_BTN_2");//穿戴
                }
                else
                {
                    mBtn.enabled = false;
                    mBtnSprite.color = Color.magenta;
                    mBtnLabel.text =  EB.Localizer.GetString("ID_PARTNER_AWAKEN_BTN_1");//未拥有
                }
            }
            else
            {
                //待处理
            }
        }
        
        private GameObject curObj;
        private void OnCenter(GameObject obj)
        {
            if (curObj == obj)
            {
                return;
            }
            curObj = obj;
            OnItemSelect();
            //InitSelect(curObj.GetComponent<LTPartnerSkinItem>().GetSkinIndex());
        }
    
        private void OnItemSelect()
        {
            int index = mScroll.activates.IndexOf(curObj);
            for(int i=0;i<mScroll.activates.Count;++i)
            {
                int depth = mScroll.activates.Count - Mathf.Abs(index - i)*10;
    
                mScroll.activates[i].GetComponent<LTPartnerSkinItem>().SetDepth(depth);
                mScroll.activates[i].transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                mScroll.activates[i].GetComponent<UIWidget>().alpha = 0.75f;
            }
            curObj.GetComponent<UIWidget>().alpha = 1f;
            curObj.transform.localScale = new Vector3(1f, 1f, 1f);
    
            var itemTemp=curObj.GetComponent<LTPartnerSkinItem>();
            SetGetTipLabel(itemTemp.GetSkinPartnerData(), itemTemp.GetSkinIndex());            
            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerSkinSelect, itemTemp.GetSkinIndex());
        }
    
        public void OnSkinBtnClick()
        {
            int skinIndex = curObj.GetComponent<LTPartnerSkinItem>().GetSkinIndex();
            int sceneId = MainLandLogic.GetInstance().SceneId;
            LTPartnerDataManager.Instance.PartnerUseAwakeSkin(heroId, skinIndex, sceneId, new System.Action<bool>(delegate {
                var partner =LTPartnerDataManager.Instance.RefreshSkinData(heroId);
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerAwakenSucc, partner);
                SetGetTipLabel(partner, skinIndex);
                if (partner.StatId == LTMainHudManager.Instance.UserLeaderTID) {
                    if (!AllianceUtil.IsInTransferDart)
                    {
                        Hotfix_LT.Messenger.Raise("SetLeaderEvent");
                    }
                }
            }));
        }
    }
}
