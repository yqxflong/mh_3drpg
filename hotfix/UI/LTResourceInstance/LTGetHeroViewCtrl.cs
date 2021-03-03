using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTGetHeroViewCtrl : UIControllerHotfix
    {
        public LTpartnerInfoItem IconItem;
        public UILabel NameLabel;
        public UISprite RareSprite;
        public UISprite NotOwnSprite;
        public UISprite OwnSprite;
        public List<Transform> DropTranList;
        public UISprite BGSprite;

        private int[] mBGHighValue = new int[3] { 595, 845, 1095, };
        private IList DropDatas;
        private int mInfoId = 0;
        private Hotfix_LT.Data.HeroInfoTemplate mInfoTemp;
    
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

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            IconItem = t.GetMonoILRComponent<LTpartnerInfoItem>("InfoItem");
            NameLabel = t.GetComponent<UILabel>("Name");
            RareSprite = t.GetComponent<UISprite>("Rare");
            NotOwnSprite = t.GetComponent<UISprite>("NotOwn");
            OwnSprite = t.GetComponent<UISprite>("Own");

            DropTranList = new List<Transform>();
            DropTranList.Add(t.FindEx("Items/DropItem"));
            DropTranList.Add(t.FindEx("Items/DropItem (1)"));
            DropTranList.Add(t.FindEx("Items/DropItem (2)"));

            BGSprite = t.GetComponent<UISprite>("BG");

            DropTranList[0].GetComponent<UIButton>().onClick.Add(new EventDelegate(() => OnGoToDrop(DropTranList[0])));
            DropTranList[1].GetComponent<UIButton>().onClick.Add(new EventDelegate(() => OnGoToDrop(DropTranList[1])));
            DropTranList[2].GetComponent<UIButton>().onClick.Add(new EventDelegate(() => OnGoToDrop(DropTranList[2])));
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            mInfoId = (int)param;
            mInfoTemp = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(mInfoId);
        }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            if (mInfoTemp != null)
            {
                InitUI();
            }
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }

        private void InitUI()
        {
            IconItem.Fill(mInfoTemp);
            RareSprite.spriteName = LTPartnerConfig.PARTNER_GRADE_SPRITE_NAME_DIC[(PartnerGrade)mInfoTemp.role_grade];
            NameLabel.text = mInfoTemp.name;
            LTPartnerData data = LTPartnerDataManager.Instance.GetPartnerByInfoId(mInfoId);
            NotOwnSprite.gameObject.CustomSetActive(data==null||data.HeroId <= 0);
            OwnSprite.gameObject.CustomSetActive(data != null&&data.HeroId > 0);
            InitDrop();
        }

        private void InitDrop()
        {
            int statId = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStatByInfoId(mInfoId).id;
            var item = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(statId);
            DropDatas = item.DropDatas;
            int count = DropDatas.Count;
            BGSprite.height = mBGHighValue[count - 1];
            if (count > 0)
            {
                for (int i = 0; i < 3; ++i)
                {
                    if (i < count)
                    {
                        DropTranList[i].gameObject.SetActive(true);
                        item.DropDatas[i].ShowName(DropTranList[i].Find("Label").GetComponent<UILabel>());
                        item.DropDatas[i].ShowBG(DropTranList[i].Find("BG").GetComponent<UISprite>());
                        DropTranList[i].SetSiblingIndex(i);
                    }
                    else
                    {
                        DropTranList[i].gameObject.SetActive(false);
                    }
                }
    
                for (int i = 0; i < count; i++)
                {
                    if (!item.DropDatas[i].IsOpen)
                    {
                        DropTranList[i].SetSiblingIndex(count - 1);
                    }
                }
            }
            else
            {
                DropTranList[1].gameObject.SetActive(false);
                DropTranList[2].gameObject.SetActive(false);
                DropTranList[0].gameObject.SetActive(false);
            }
        }
    
        public void OnGoToDrop(Transform tran)
        {
            int index = DropTranList.IndexOf(tran);
            FusionAudio.PostEvent("UI/General/ButtonClick");
            Hotfix_LT.Data.DropDataBase data = (DropDatas[index] as Hotfix_LT.Data.DropDataBase);
            data.GotoDrop(controller);
        }
    }
}
