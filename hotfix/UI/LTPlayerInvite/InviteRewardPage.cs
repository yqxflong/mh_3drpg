using EB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class InviteRewardPage : DynamicMonoHotfix
    {

        private eInvitePageType curPage;
        private eTaskTabType curTaskType;
        private UISprite typeSp0, typeSp1, typeSp2;
        private UIGrid TypeGrid;
        //private InviteTaskGridScroll taskgrid;
        //private InviteTaskItemData[] loginArray, levelUpArray, chargeArray;
        private InviteScrollDataLookUp inviteTaskScroll;
        private GameObject Inputobj, Invitedsucobj, InviteRewardobj,loginobj,levelupobj,chargeobj, loginred, levelupred, chargered;
        private UILabel /*InvitePlayers, InvitePlayersShadow, InviteHc,InviteHcShadow,*/ InputInviteCode;
        private inviteDataLookUp inviteplayer, inviteHc;
        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.transform;
            typeSp0 = t.GetComponent<UISprite>("TaskView/TopBtns/TypeBtn/Sprite");
            typeSp1 = t.GetComponent<UISprite>("TaskView/TopBtns/TypeBtn (1)/Sprite");
            typeSp2 = t.GetComponent<UISprite>("TaskView/TopBtns/TypeBtn (2)/Sprite");
            TypeGrid = t.GetComponent<UIGrid>("TaskView/TopBtns");
            inviteTaskScroll = t.GetDataLookupILRComponent<InviteScrollDataLookUp>("TaskView/ScrollView/Placeholder/Grid");
            //taskgrid = t.GetMonoILRComponent<InviteTaskGridScroll>("TaskView/ScrollView/Placeholder/Grid");
            Inputobj = t.GetComponent<Transform>("TaskView/TitleContent/InputInviteCode").gameObject;
            Invitedsucobj = t.GetComponent<Transform>("TaskView/TitleContent/InvatedScucess").gameObject;
            InviteRewardobj = t.GetComponent<Transform>("TaskView/TitleContent/InviteReward").gameObject;
            inviteplayer = t.GetDataLookupILRComponent<inviteDataLookUp>("TaskView/TitleContent/InviteReward/InviteNum");
            //InvitePlayersShadow = t.GetComponent<UILabel>("TaskView/TitleContent/InviteReward/InviteNum/Label(Clone)");
            inviteHc = t.GetDataLookupILRComponent<inviteDataLookUp>("TaskView/TitleContent/InviteReward/HcNum");
            //InviteHcShadow = t.GetComponent<UILabel>("TaskView/TitleContent/InviteReward/HcNum/Label(Clone)");
            InputInviteCode = t.GetComponent<UILabel>("TaskView/TitleContent/InputInviteCode/InputLabel");
            loginobj = t.GetComponent<Transform>("TaskView/TopBtns/TypeBtn").gameObject;
            levelupobj = t.GetComponent<Transform>("TaskView/TopBtns/TypeBtn (1)").gameObject;
            chargeobj = t.GetComponent<Transform>("TaskView/TopBtns/TypeBtn (2)").gameObject;
            loginred = t.GetComponent<Transform>("TaskView/TopBtns/TypeBtn/RedPoint").gameObject;
            levelupred = t.GetComponent<Transform>("TaskView/TopBtns/TypeBtn (1)/RedPoint").gameObject;
            chargered = t.GetComponent<Transform>("TaskView/TopBtns/TypeBtn (2)/RedPoint").gameObject;
            t.GetComponent<UIButton>("TaskView/TopBtns/TypeBtn").onClick.Add(new EventDelegate(delegate { OnTaskTypeBtnClick(eTaskTabType.logion,(int)curPage); }));
            t.GetComponent<UIButton>("TaskView/TopBtns/TypeBtn (1)").onClick.Add(new EventDelegate(delegate { OnTaskTypeBtnClick(eTaskTabType.levelup,(int)curPage); }));
            t.GetComponent<UIButton>("TaskView/TopBtns/TypeBtn (2)").onClick.Add(new EventDelegate(delegate { OnTaskTypeBtnClick(eTaskTabType.charge, (int)curPage); }));
            t.GetComponent<ConsecutiveClickCoolTrigger>("TaskView/TitleContent/InputInviteCode/SureButton").clickEvent.Add(new EventDelegate(OnBindInvitedCodeBtnClick));
        }

        public override void OnDestroy()
        {
            RemoveAllRP();
        }
        public override void OnEnable()
        {
            inviteplayer.SetDataID("inviteFriends.friendNum", "0");
            inviteHc.SetDataID("inviteFriends.friendHc", "0");
        }
        //public override void OnDisable()
        //{
        //    RemoveAllRP();
        //}
       
        public void Show(bool isshow,int pagetype)
        {
            mDMono.gameObject.CustomSetActive(isshow);
            RemoveAllRP();
            if (isshow)
            {
                SetCurPageData(pagetype);
                SetCaterageState();
            }           
        }

        public void SetCaterageState()
        {
            if(curPage == eInvitePageType.inviteOther)
            {
                loginobj.CustomSetActive(PlayerInviteManager.Instance.inviteloginnum>0);
                levelupobj.CustomSetActive(PlayerInviteManager.Instance.inviteuplevelnum > 0);
                chargeobj.CustomSetActive(PlayerInviteManager.Instance.invitechargenum > 0);
                LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.invitelogin, SetloginRP);
                LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.invitelevelup, SetlevelupRP);
                LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.invitecharge, SetchargeRP);
            }
            else if(curPage == eInvitePageType.invited)
            {
                loginobj.CustomSetActive(PlayerInviteManager.Instance.invitedloginnum > 0);
                levelupobj.CustomSetActive(PlayerInviteManager.Instance.inviteduplevelnum > 0);
                chargeobj.CustomSetActive(PlayerInviteManager.Instance.invitedchargenum > 0);
                LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.invitedlogin, SetloginRP);
                LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.invitedlevelup, SetlevelupRP);
                LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.invitedcharge, SetchargeRP);
            }
            TypeGrid.Reposition();
        }

        private void SetloginRP(RedPointNode node) { loginred.CustomSetActive(node.num > 0); }
        private void SetlevelupRP(RedPointNode node) { levelupred.CustomSetActive(node.num > 0); }
        private void SetchargeRP(RedPointNode node) { chargered.CustomSetActive(node.num > 0); }
        private void RemoveAllRP()
        {
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.invitedlogin, SetloginRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.invitedlevelup, SetlevelupRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.invitedcharge, SetchargeRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.invitedlogin, SetloginRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.invitedlevelup, SetlevelupRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.invitedcharge, SetchargeRP);
        }

        private eTaskTabType GetCurSeleteBtn()
        {
            if(curPage == eInvitePageType.inviteOther)
            {
                if(PlayerInviteManager.Instance.inviteloginnum > 0)
                {
                    return eTaskTabType.logion;
                }
                if (PlayerInviteManager.Instance.inviteuplevelnum > 0)
                {
                    return eTaskTabType.levelup;
                }
                if (PlayerInviteManager.Instance.invitechargenum > 0)
                {
                    return eTaskTabType.charge;
                }
            }else if(curPage == eInvitePageType.invited)
            {
                if (PlayerInviteManager.Instance.invitedloginnum > 0)
                {
                    return eTaskTabType.logion;
                }
                if (PlayerInviteManager.Instance.inviteduplevelnum > 0)
                {
                    return eTaskTabType.levelup;
                }
                if (PlayerInviteManager.Instance.invitedchargenum > 0)
                {
                    return eTaskTabType.charge;
                }
            }
            return eTaskTabType.logion;
        }
        private void SetCurPageData(int pagetype)
        {
            curPage = (eInvitePageType)pagetype;
            bool hasbind = false;
            hasbind = PlayerInviteManager.Instance.HasBindInviteCode;
            switch (curPage)
            {
                case eInvitePageType.none:
                    break;
                case eInvitePageType.inviteOther:
                    break;
                case eInvitePageType.invited:
                    break;
                default:
                    break;
            }
            InviteRewardobj.CustomSetActive(curPage == eInvitePageType.inviteOther);
            Invitedsucobj.CustomSetActive(curPage == eInvitePageType.invited && hasbind);
            Inputobj.CustomSetActive(curPage == eInvitePageType.invited && !hasbind);
            OnTaskTypeBtnClick(GetCurSeleteBtn(), pagetype);
        }


        private void OnTaskTypeBtnClick(eTaskTabType type, int pagetype)
        {
            //if ((int)curPage == pagetype && curTaskType == type)
            //{
            //    return;
            //}
            curTaskType = type;
            typeSp0.color = curTaskType == eTaskTabType.logion ? Color.white : Color.magenta;
            typeSp1.color = curTaskType == eTaskTabType.levelup ? Color.white : Color.magenta;
            typeSp2.color = curTaskType == eTaskTabType.charge ? Color.white : Color.magenta;
            //设置显示itemlist
            inviteTaskScroll.SetTaskTypeAndParam(eTaskType.Invite, curPage, curTaskType);
        }
        private void OnBindInvitedCodeBtnClick()
        {
            string code = InputInviteCode.text;
            if (string.IsNullOrEmpty(code))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_INVITE_23"));//邀请码为空
                return;
            }
            if (code.Equals(PlayerInviteManager.Instance.InviteCode))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_INVITE_21"));//不能输入自己的邀请码哦！
                return;
            }
            if (ShareToManager.IsEmulator())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_INVITE_22"));//模拟器无法输入邀请码，请在手机上使用
                return;
            }
            PlayerInviteManager.Instance.BindInvitePlayer(code, delegate
            {
                Invitedsucobj.CustomSetActive(true);
                Inputobj.CustomSetActive(false);
                PlayerInviteManager.Instance.ReflashRedPoint();
            }
            );
        }

    }

    public class inviteDataLookUp : DataLookupHotfix
    {
        private UILabel showtext,showtextlabel;
        private string defaultText;

        public override void Awake()
        {
            base.Awake();
            showtext = mDL.transform.GetComponent<UILabel>();
            showtextlabel = mDL.transform.GetComponent<UILabel>("Label(Clone)",false);
        }
        public void SetDataID(string DataId,string Defaultvalue)
        {
            mDL.DefaultDataID = DataId;
            defaultText = Defaultvalue;
        }
        public override void OnLookupUpdate(string dataID, object value)
        {          
            base.OnLookupUpdate(dataID, value);
            string showstr = value == null ? defaultText : value.ToString();
            showtext.text = showstr;
            if (showtextlabel != null)
            {
                showtextlabel.text = showstr;
            }
        }
    }
}
