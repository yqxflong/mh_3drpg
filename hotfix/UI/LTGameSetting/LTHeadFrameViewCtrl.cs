using Hotfix_LT.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class HeadFrameEvent//主干临时用，热更改用新事件
    {
        public static System.Action<string, int, bool> SelectEvent;
        public static System.Action InitEvent;
    }

    public class LTHeadFrameViewCtrl : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.transform;
            mScroll = t.GetMonoILRComponent<LTHeadFrameViewScroll>("HeadFrameView/Placeholder/Grid");
            HeadIcon = t.transform.GetComponent<UISprite>("TopBG/Head/Icon");
            HeadFrameNameLabel = t.transform.GetComponent<UILabel>("TopBG/Head/Name");
            HeadFrameDescLabel = t.transform.GetComponent<UILabel>("TopBG/Head/Desc");
            HeadFrameIcon = t.transform.GetComponent<UISprite>("TopBG/Head/Icon/Frame");
            UseBtn = t.transform.GetComponent<UIButton>("TopBG/UseButton");
            UseBtn.onClick.Add(new EventDelegate ( OnUseBtnClick));
            UseBtnLabel = t.transform.GetComponent<UILabel>("TopBG/UseButton/Label");
        }

        public LTHeadFrameViewScroll mScroll;

        public UISprite HeadIcon;
        public UILabel HeadFrameNameLabel;
        public UILabel HeadFrameDescLabel;
        public UISprite HeadFrameIcon;
        public UIButton UseBtn;
        public UILabel UseBtnLabel;

        //到热更需要放到manager中
        public static string Id;//ui选择
        public static int Num;
        private string headFrameStr;
        public static string CurId;//角色目前
        public static int CurNum;

        public override void Start()
        {
            DataLookupsCache.Instance.SearchDataByID<string>("user.headFrame", out headFrameStr);
            if (string.IsNullOrEmpty(headFrameStr))
            {
                Id = "0";
                Num = 0;
            }
            else
            {
                string[] split = headFrameStr.Split('_');
                Id = split[0];
                Num = int.Parse(split[1]);
            }
            SelectEvent(Id, Num, false);

            HeadFrameEvent.SelectEvent += SelectEvent;
            HeadFrameEvent.InitEvent += InitItems;
        }

        public override void OnEnable()
        {
            HeadIcon.spriteName = LTMainHudManager.Instance.UserHeadIcon;
        }

        public override void OnDestroy()
        {
            HeadFrameEvent.SelectEvent -= SelectEvent;
            HeadFrameEvent.InitEvent -= InitItems;
            Id = CurId = headFrameStr = null;
            Num = CurNum = 0;
        }
        private Dictionary<string, int> HeadFrameDic = new Dictionary<string, int>();
        public void InitItems()
        {
            var temp = EconemyTemplateManager.Instance.GetAllHeadFrame();
            //排序
            Hashtable Data;
            DataLookupsCache.Instance.SearchDataByID<Hashtable>("userHeadFrame.head_frame", out Data);
            if (Data != null)
            {
                foreach (DictionaryEntry data in Data)
                {
                    string Id = data.Key.ToString();
                    if (!HeadFrameDic.ContainsKey(Id))
                    {
                        int Num = int.Parse(data.Value.ToString());// EB.Dot.Integer(Id, data, 0);
                        HeadFrameDic.Add(Id, Num);
                    }
                    else
                    {
                        int Num = int.Parse(data.Value.ToString());
                        HeadFrameDic[Id] = Num;
                    }
                }
            }
            temp.Sort((a, b) =>
            {
                if (a.id.Equals("0")) return -1;
                else if (b.id.Equals("0")) return 1;
                else if (HeadFrameDic.ContainsKey(a.id) && HeadFrameDic[a.id] >= a.num && (!HeadFrameDic.ContainsKey(b.id) || HeadFrameDic.ContainsKey(b.id) && HeadFrameDic[b.id] < b.num))
                    return -1;
                else if (HeadFrameDic.ContainsKey(b.id) && HeadFrameDic[b.id] >= b.num && (!HeadFrameDic.ContainsKey(a.id) || HeadFrameDic.ContainsKey(a.id) && HeadFrameDic[a.id] < a.num))
                    return 1;
                else if (int.Parse(a.id) < int.Parse(b.id))
                    return -1;
                else if (int.Parse(a.id) > int.Parse(b.id))
                    return 1;
                else if (a.num < b.num)
                    return -1;
                else if (a.num > b.num)
                    return 1;
                else
                    return 0;
            });
            mScroll.SetItemDatas(temp);
        }

        public void SelectEvent(string id, int num, bool isLock)
        {
            Id = id;
            Num = num;
            HeadFrame data = EconemyTemplateManager.Instance.GetHeadFrame(Id, Num);
            if (string.IsNullOrEmpty(headFrameStr) || string.IsNullOrEmpty(CurId))
            {
                DataLookupsCache.Instance.SearchDataByID<string>("user.headFrame", out headFrameStr);
                if (string.IsNullOrEmpty(headFrameStr)) headFrameStr = "0_0";
                CurId = data.id;
                CurNum = data.num;
            }
            HeadFrameNameLabel.text = data.name;
            HeadFrameDescLabel.text = data.desc;
            HeadFrameIcon.spriteName = data.iconId;
            if (isLock)
            {
                UseBtn.GetComponent<UISprite>().color = Color.magenta;
                UseBtn.GetComponent<BoxCollider>().enabled = false;
                UseBtnLabel.text = EB.Localizer.GetString("ID_PARTNER_AWAKEN_BTN_1");
            }
            else if (Id.Equals(CurId) && Num == CurNum)
            {
                UseBtn.GetComponent<UISprite>().color = Color.magenta;
                UseBtn.GetComponent<BoxCollider>().enabled = false;
                UseBtnLabel.text = EB.Localizer.GetString("ID_PARTNER_AWAKEN_BTN_2");
            }
            else
            {
                UseBtn.GetComponent<UISprite>().color = Color.white;
                UseBtn.GetComponent<BoxCollider>().enabled = true;
                UseBtnLabel.text = EB.Localizer.GetString("ID_SMALLPARTNER_USE");
            }

            InitItems();
        }

        public void OnUseBtnClick()
        {
            if (headFrameStr.Equals(string.Format("{0}_{1}", Id, Num))) return;

            int sceneId = SceneLogicManager.CurrentSceneLogic.SceneId;
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/headframe/useHeadFrame");
            request.AddData("id", Id);
            request.AddData("num", Num);
            request.AddData("sceneId", sceneId);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                DataLookupsCache.Instance.CacheData(data);
                headFrameStr = null;
                SelectEvent(Id, Num, false);
            });
        }
    }
}