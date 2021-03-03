using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.Data;
using UnityEngine;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// UR伙伴降临
    /// </summary>
    public class LTActivityBodyItem_URPartner : LTActivityBodyItem
    {
        private CommonRankGridScroll gridscroll;
        private List<CommonRankItemData> rankdatalist;
        private UISprite urPartnerIcon;
        private UISprite urPartnerType,urspTpl;
        private string UrPartnerInfoid;
        private string selfrankStr,redstr = "ff6699", greenstr = "ffffff";
        private UILabel selfRank;
        private UILabel selfScore,actEndtip, actEndtipshadow, countdowntip,countdowntipshaow,
            onecostlabel, onecostlabelshadow,tencostlabel,tencostlabelshadow, desctemp;
        private long selfuid;
        private int activityId,freedrawtimer,scoreredtimer,activityendtimer,NextFreeTime,oneDrawcardcost, tenDrawcardcost;
        private int MaxRankValue;
        private ParticleSystemUIComponent charFx;
        private EffectClip efClip;
        private GameObject endtipobj, drawcard1, drawcard10, DrawCardrp1, DrawCardrp10,scoreRewardRp, freeobj,costobj;
        private Transform content;
        private bool isCreatRichText;
        private Vector3 descpos;

        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.transform;
            urPartnerIcon = t.GetComponent<UISprite>("HeroInfoList/InfoItem/Icon");
            urPartnerType = t.GetComponent<UISprite>("HeroInfoList/InfoItem/Role");
            gridscroll = t.GetMonoILRComponent<CommonRankGridScroll>("Rankpanel/ScrollView/Placeholder/Grid");
            selfRank = t.GetComponent<UILabel>("Rankpanel/SelfRank");
            selfScore = t.GetComponent<UILabel>("Rankpanel/SelfScore");
            actEndtip = t.GetComponent<UILabel>("URDrawCard/Endtip");
            actEndtipshadow = t.GetComponent<UILabel>("URDrawCard/Endtip/Label (1)");
            countdowntip = t.GetComponent<UILabel>("URDrawCard/DrawCard1/descDr1");
            countdowntipshaow = t.GetComponent<UILabel>("URDrawCard/DrawCard1/descDr1/Label (1)");

            t.GetComponent<UILabel>("URDrawCard/DrawCard1/Draw1Btn/name").text = t.GetComponent<UILabel>("URDrawCard/DrawCard1/Draw1Btn/name/Label (1)").text =string .Format ( EB.Localizer .GetString("ID_DRAWCARD_UR"),1);
            t.GetComponent<UILabel>("URDrawCard/DrawCard10/Draw10Btn/name").text = t.GetComponent<UILabel>("URDrawCard/DrawCard10/Draw10Btn/name/Label (1)").text = string.Format(EB.Localizer.GetString("ID_DRAWCARD_UR"), 10);

            onecostlabel = t.GetComponent<UILabel>("URDrawCard/DrawCard1/Draw1Btn/cost");
            onecostlabelshadow = t.GetComponent<UILabel>("URDrawCard/DrawCard1/Draw1Btn/cost/Label (1)");

            tencostlabel = t.GetComponent<UILabel>("URDrawCard/DrawCard10/Draw10Btn/cost");
            tencostlabelshadow = t.GetComponent<UILabel>("URDrawCard/DrawCard10/Draw10Btn/cost/Label (1)");
            desctemp = t.GetComponent<UILabel>("CONTENT/DESC (1)");
            content = t.GetComponent<Transform>("CONTENT");
            urspTpl = t.GetComponent<UISprite>("CONTENT/Sprite");
            freeobj = t.GetComponent<Transform>("URDrawCard/DrawCard1/Draw1Btn/free").gameObject;
            costobj = t.GetComponent<Transform>("URDrawCard/DrawCard1/Draw1Btn/cost").gameObject;
            endtipobj = t.GetComponent<Transform>("URDrawCard/Endtip").gameObject;
            drawcard1 = t.GetComponent<Transform>("URDrawCard/DrawCard1").gameObject;
            drawcard10 = t.GetComponent<Transform>("URDrawCard/DrawCard10").gameObject;
            DrawCardrp1 = t.GetComponent<Transform>("URDrawCard/DrawCard1/Draw1Btn/redpoint").gameObject;
            DrawCardrp10 = t.GetComponent<Transform>("URDrawCard/DrawCard10/Draw10Btn/redpoint").gameObject;
            scoreRewardRp = t.GetComponent<Transform>("NavButton (1)/redpoint").gameObject;
            t.GetComponent<ConsecutiveClickCoolTrigger>("HeroInfoList/InfoItem").clickEvent.Add(new EventDelegate(OnClickPartnerIcon));
            t.GetComponent<ConsecutiveClickCoolTrigger>("URDrawCard/DrawCard1/Draw1Btn").clickEvent.Add(new EventDelegate(delegate { OnClickURDrawCardBtn(1); }));
            t.GetComponent<ConsecutiveClickCoolTrigger>("URDrawCard/DrawCard10/Draw10Btn").clickEvent.Add(new EventDelegate(delegate { OnClickURDrawCardBtn(10); }));
            t.GetComponent<UIButton>("URDrawCard/Priview").onClick.Add(new EventDelegate(OnLookUpPartnerBtnClick));
            t.GetComponent<UIButton>("NavButton (1)").onClick.Add(new EventDelegate(OpenScoreRewardView));
            t.GetComponent<UIButton>("NavButton (2)").onClick.Add(new EventDelegate(OpenRankRewardView));
            selfuid = LoginManager.Instance.LocalUserId.Value;
            oneDrawcardcost = (int)NewGameConfigTemplateManager.Instance.GetGameConfigValue("hcLotteryOneTime");
            tenDrawcardcost = (int)NewGameConfigTemplateManager.Instance.GetGameConfigValue("hcLotteryTenTimes");
            isCouldClick = true;
            descpos = new Vector3(-618, 371, 0);
        }
        public override void OnDestroy()
        {
            isCreatRichText = false;
            base.OnDestroy();
        }
        public override void OnEnable()
        {
            Messenger.AddListener(EventName.OnURScoreRewardRecieve, delegate { SetScoreRewardRPState(); });
        }

        public override void OnDisable()
        {
            if (freedrawtimer != 0)
            {
                ILRTimerManager.instance.RemoveTimer(freedrawtimer);
                freedrawtimer = 0;
            }
            if(scoreredtimer != 0)
            {
                ILRTimerManager.instance.RemoveTimer(scoreredtimer);
                scoreredtimer = 0;
            }
            if(activityendtimer == 0)
            {
                ILRTimerManager.instance.RemoveTimer(activityendtimer);
                activityendtimer = 0;
            }
            Messenger.RemoveListener(EventName.OnURScoreRewardRecieve, delegate { SetScoreRewardRPState(); });
        }
        public override void SetData(object data)
        {
            base.SetData(data);                    
            activityId = EB.Dot.Integer("activity_id", data, 0);
            string desctext = EB.Dot.String("desc", data, "");     
            SetTextShow(RichTextParser.ParseRichText(desctext), descpos, descpos);
            SetDrawCardState();
            int timetoend = fintime - EB.Time.Now;
            if (activityendtimer == 0 && timetoend > 0) ILRTimerManager.instance.AddTimer(timetoend*1000, 1, OnactivityEnd);
            var activity = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivity(activityId);            
            UrPartnerInfoid = activity.parameter1;
            int.TryParse(UrPartnerInfoid, out int infoid);          
            HeroInfoTemplate infotemp = CharacterTemplateManager.Instance.GetHeroInfo(infoid);
            MaxRankValue = EventTemplateManager.Instance.GetURPartnerEventRewardMaxNum(infoid);
            if (infotemp != null)
            {
                urPartnerIcon.spriteName = infotemp.icon;
                urPartnerType.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[infotemp.char_type];
                HotfixCreateFX.ShowCharTypeFX(charFx, efClip, urPartnerType.transform, (PartnerGrade)infotemp.role_grade, infotemp.char_type);
            }
            ActivityUtil.RequestRankData((int)ActivityUtil.ActivityRankType.URPartnerRank, SetRankData);          
        }
        private void SetTextShow(List<RichTextParser.Data> datas,Vector3 originpos, Vector3 curpos,int index = 0)
        {
            if (isCreatRichText||this == null||mDMono==null) return;
            int spacey = -65;
            for (; index < datas.Count; index++)
            {
                var data = datas[index];
                switch (data.Type)
                {
                    case RichTextParser.DataType.tString:
                        UILabel label = GameObject.Instantiate(desctemp);
                        label.transform.SetParent(content);
                        label.transform.localPosition = curpos;
                        label.transform.localScale = Vector3.one;
                        label.text = data.Text;
                        label.transform.gameObject.CustomSetActive(true);
                        curpos.x += label.width;
                        break;
                    case RichTextParser.DataType.tAtlas:
                        UISprite ursp = GameObject.Instantiate(urspTpl, content);                
                        EB.Assets.LoadAsyncAndInit<GameObject>("LTGeneral_Atlas", (assetName, o, succ) =>
                        {
                            if (succ)
                            {
                                ursp.atlas = o.GetComponent<UIAtlas>();
                                ursp.spriteName = data.SpriteNameInAtlas;
                                ursp.pivot = UIWidget.Pivot.TopLeft;
                                ursp.transform.localPosition = curpos;
                                ursp.transform.gameObject.CustomSetActive(true);
                                ursp.transform.localScale = Vector3.one;
                                ursp.MakePixelPerfect();
                                float rate = (float)50 / ursp.height;
                                ursp.height = 50;
                                ursp.width = (int)(ursp.width * rate);
                                ursp.depth = 10;
                                ursp.transform.gameObject.CustomSetActive(true);
                                curpos.x += ursp.width;
                                SetTextShow(datas, originpos, curpos, index+1);
                            }
                        }, ursp, null, false);
                        return;//终止解析
                    case RichTextParser.DataType.tBreak:
                        curpos.x = originpos.x;
                        curpos.y += spacey;
                        break;
                    case RichTextParser.DataType.tError:
                    case RichTextParser.DataType.tTexture:
                    default:
                        break;
                }
            }
            isCreatRichText = true;
        }

        private void OnactivityEnd(int seq)
        {
            SetDrawCardState();
            activityendtimer = 0;
            title.UpdateRedPoint();
        }
        private void SetDrawCardState()
        {
            string tipstr = "";
            bool isshowtip = false;
            if (starttime > EB.Time.Now)
            {
                tipstr = string.Format("[ff6699]{0}[-]",EB.Localizer.GetString("ID_ACTIVITY_NOTSTARTED"));
                isshowtip = true;
            }else if(fintime < EB.Time.Now)
            {
                tipstr = EB.Localizer.GetString("ID_uifont_in_LTLegionWarQualify_End_4");
                isshowtip = true;
            }
            if (!isshowtip)
            {
                AddCountDownTimer();
                DrawCardrp10.CustomSetActive(BalanceResourceUtil.GetUserDiamond() >= tenDrawcardcost);
            }
            actEndtip.text = actEndtipshadow.text = tipstr;
            endtipobj.CustomSetActive(isshowtip);
            drawcard1.CustomSetActive(!isshowtip);
            drawcard10.CustomSetActive(!isshowtip);
            SetScoreRewardRPState();
            SetDiamondCostLabel();
        }
        private void SetScoreRewardRPState(int seq = 0)
        {
            ActivityUtil.ResetRankRefreshRecord((int)ActivityUtil.ActivityRankType.URPartnerRank);
            ActivityUtil.RequestRankData((int)ActivityUtil.ActivityRankType.URPartnerRank, SetRankData);
            scoreRewardRp.CustomSetActive(GetScoreRewardRPState());
            scoreredtimer = 0;
            title.UpdateRedPoint();
        }
        private void SetDiamondCostLabel()
        {
            string colorstr;
            colorstr = BalanceResourceUtil.GetUserDiamond() >= oneDrawcardcost ? greenstr : redstr;
            onecostlabel.text = onecostlabelshadow.text = string.Format("[{0}]{1}[-]", colorstr, oneDrawcardcost);
            colorstr = BalanceResourceUtil.GetUserDiamond() >= tenDrawcardcost ? greenstr : redstr;
            tencostlabel.text = tencostlabelshadow.text = string.Format("[{0}]{1}[-]", colorstr, tenDrawcardcost);
        }
        private bool GetScoreRewardRPState()
        {
            if(starttime > EB.Time.Now|| state.Equals("pending"))
            {               
                return false;
            }
       
            var stages = EventTemplateManager.Instance.GetTimeLimitActivityStages(activityId);
            DataLookupsCache.Instance.SearchDataByID(string.Format("tl_acs.{0}", activityId), out Hashtable data);
            int score = EB.Dot.Integer("current", data, 0);
            if (score > 0)
            {
                for (int i = 0; i < stages.Count; i++)
                {
                    var tempstage = stages[i];
                    if (score>= tempstage.stage && EB.Dot.Integer(string.Format("stages.{0}", tempstage.id),data,0) == 0)
                    {
                        return true;
                    }else if(score< tempstage.stage)
                    {
                        return false;
                    }
                }
            }
            return false;
        }
        private void AddCountDownTimer()
        {
            DataLookupsCache.Instance.SearchIntByID("lottery.gold.next_refresh_time", out NextFreeTime);
            
            if(!DataLookupsCache.Instance.SearchDataByID<bool>(string.Format("lottery.urbuddy.{0}.isFree", activityId), out bool isfree))
            {
                isfree = true;
            }
            int timetamp = NextFreeTime - EB.Time.Now -1;
            if (timetamp <= 1|| isfree)
            {
                countdowntip.text = countdowntipshaow.text = EB.Localizer.GetString("ID_DRAWCAR_URFreeTIP");
                costobj.CustomSetActive(false);
                freeobj.CustomSetActive(true);
                DrawCardrp1.CustomSetActive(true);
            }
            else
            {
                RefreshFreeTime(0);
                if (freedrawtimer == 0)
                {
                    freedrawtimer = ILRTimerManager.instance.AddTimer(1000, timetamp, RefreshFreeTime);
                }
                DrawCardrp1.CustomSetActive(false);
                costobj.CustomSetActive(true);
                freeobj.CustomSetActive(false);
            }
        }

        private void RefreshFreeTime(int seq)
        {
            int timetamp = NextFreeTime - EB.Time.Now - 1;
            if (timetamp <= 0)
            {
                timetamp = 0;
                freedrawtimer = 0;
                AddCountDownTimer();
            }
            countdowntip.text = countdowntipshaow.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTDrawCardConfig_1139"), timetamp / 3600, timetamp % 3600 / 60, timetamp % 60);           
        }
        private void SetRankData()
        {
            if (this == null|| this. mDMono == null) return;
            Hashtable activityData = null;
            DataLookupsCache.Instance.SearchDataByID("tl_acs." + activityId, out activityData);
            ArrayList list = EB.Dot.Array("leaderboard", activityData, null);
            if (rankdatalist == null)
                rankdatalist = new List<CommonRankItemData>();
            else rankdatalist.Clear();
            if (list != null)
            {
                //设置自己数据
                if (list.Count > 0)
                {
                    var selfdata = list[0];
                    if (EB.Dot.Long("id", selfdata, 0) == selfuid)
                    {
                        int selfrank = EB.Dot.Integer("rank", selfdata, -1);
                        if (selfrank == -1)
                        {

                            selfrankStr = EB.Localizer.GetString("ID_ARENA_RANK_OUT_OF_RANGE");
                        }
                        else if (selfrank > MaxRankValue)
                        {
                            selfrankStr = string.Format("{0}+", MaxRankValue);
                        }
                        else
                        {
                            selfrankStr = selfrank.ToString();
                        }
                        selfRank.text = selfRank.transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}[42fe79]{1}[-]", EB.Localizer.GetString("ID_ARENA_LOCAL_RANK"), selfrankStr);
                        int selfscore = EB.Dot.Integer("score", selfdata, 0);
                        selfScore.text = selfScore.transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}[42fe79]{1}[-]", EB.Localizer.GetString("ID_ACTIVITY_UR_MYSCORE"), selfscore.ToString());
                    }
                }
                //设置他人数据
                object temp;
                int infoid,skin,index = 0;
                if (list.Count > 1)
                {
                    for (int i = 1; i < list.Count; i++)
                    {
                        temp = list[i];
                        if (temp == null) continue;
                        CommonRankItemData tempdata = new CommonRankItemData();
                        tempdata.m_Rank = EB.Dot.Integer("rank", temp, 0) - 1;
                        infoid = EB.Dot.Integer("template_id", temp, 15011)-1;
                        skin = EB.Dot.Integer("skin", temp, 0);
                        tempdata.m_Icon = CharacterTemplateManager.Instance.GetHeroInfo(infoid, skin).icon;
                        tempdata.m_Frame = EB.Dot.String("headFrame", temp, "");
                        tempdata.m_Name = EB.Dot.String("name", temp, "Null");
                        tempdata.m_Parm = EB.Dot.String("score", temp, "0");
                        tempdata.m_Uid = EB.Dot.Long("id", temp, 0);
                        tempdata.m_DrawLevel = EB.Dot.String("level", temp, "1");
                        rankdatalist.Add(tempdata);
                        index = i;
                    }
                    if (index < 3)
                    {
                        SetDefaultInfo(index);
                    }
                }
                else
                {
                    SetDefaultInfo();
                }
            }
            else
            {
                selfrankStr = EB.Localizer.GetString("ID_ARENA_RANK_OUT_OF_RANGE");
                selfRank.text = selfRank.transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}[42fe79]{1}[-]", EB.Localizer.GetString("ID_ARENA_LOCAL_RANK"), selfrankStr);
                int selfscore = 0;
                selfScore.text = selfScore.transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}[42fe79]{1}[-]", EB.Localizer.GetString("ID_ACTIVITY_UR_MYSCORE"), selfscore.ToString());
                SetDefaultInfo();
            }
            gridscroll.SetItemDatas(rankdatalist.ToArray());
        }

        private void SetDefaultInfo(int index = 0)
        {
            for (int i = index; i < 3; i++)//构造无玩家时显示
            {
                CommonRankItemData tempdata = new CommonRankItemData();
                tempdata.m_Rank = i;
                //tempdata.m_Icon = "";
                tempdata.m_Uid = 0;
                //tempdata.m_Frame = "";
                tempdata.m_Name = EB.Localizer.GetString("ID_UR_DEFAULT_PLAYER");
                tempdata.m_Parm = "0";
                rankdatalist.Add(tempdata);
            }
        }

        private void OnClickPartnerIcon()
        {
            if (string.IsNullOrEmpty(UrPartnerInfoid)) return;
            var ht = Johny.HashtablePool.Claim();
            ht.Add("id", UrPartnerInfoid);
            ht.Add("screenPos", new Vector2(Screen.width*0.53f, Screen.height*0.3f));
            GlobalMenuManager.Instance.Open("LTHeroToolTipUI", ht);
        }

        /// <summary>
        /// 查看抽奖概率信息
        /// </summary>
        private void OnLookUpPartnerBtnClick()
        {
            Hashtable data = new Hashtable();
            data.Add("type", DrawCardType.ur);
            int infoid;
            if (int.TryParse(UrPartnerInfoid, out infoid))
            {
                data.Add("chipdata", new string[1] { (infoid+1).ToString() });
            }
            GlobalMenuManager.Instance.Open("LTDrawCardLookupUI", data);
        }
        private void OpenRankRewardView()
        {
            int infoid;
            if (int.TryParse(UrPartnerInfoid, out infoid))
            {
                Hashtable table = new Hashtable();
                table.Add("itemDatas", EventTemplateManager.Instance.GetURPartnerEventRewardList(infoid));
                table.Add("tip", EB.Localizer.GetString("ID_uifont_in_LTWorldBossRewardPreviewUI_Label_0"));//活动结束后通过邮件发放奖励
                GlobalMenuManager.Instance.Open("LTGeneralRankAwardUI", table);
            }
        }
        private void OpenScoreRewardView()
        {
            GlobalMenuManager.Instance.Open("LTActivityScoreRewardHud", activityId);
        }
        bool isCouldClick;
        private void OnClickURDrawCardBtn(int times)
        {
            if (!isCouldClick) return;
            isCouldClick = false;
            LTHotfixApi.GetInstance().ExceptionFunc = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObjects = (string)response.error;
                    string[] strObject = strObjects.Split(",".ToCharArray(), 2);
                    switch (strObject[0])
                    {
                        case "event is not running":
                        {
                            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_uifont_in_LTLegionWarQualify_End_4"));//活动已结束
                            return true;
                        }
                        case "nsf":
                        {

                            MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_4, EB.Localizer.GetString("ID_codefont_in_NationBattleSelectTeamController_8317"), delegate (int result)
                            {
                                if (result == 0)
                                {
                                    InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                                    GlobalMenuManager.Instance.Open("LTChargeStoreHud", null);
                                }
                            });
                            return true;
                        }
                    }
                }
                return false;
            };
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/specialactivity/urbuddylottery");
            request.AddData("activityId", activityId);
            request.AddData("times", times);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                DataLookupsCache.Instance.CacheData(data);
                DrawCardrp10.CustomSetActive(BalanceResourceUtil.GetUserDiamond() >= tenDrawcardcost);//跳过抽卡动画时，不会走setdata(),需要此处刷新
                SetDiamondCostLabel();
                if (scoreredtimer == 0)
                {
                    scoreredtimer = ILRTimerManager.instance.AddTimer(1000, 1, SetScoreRewardRPState);
                }
                if (freedrawtimer == 0)
                {
                    AddCountDownTimer();
                }
                LTDrawCardDataManager.Instance.InitAllDrawPartner(data);
                object[] i = { (int)DrawCardType.ur, 1, "" };
                GlobalMenuManager.Instance.Open("LTGetItemUI", i);
                isCouldClick = true;
            });
        }

    }
}