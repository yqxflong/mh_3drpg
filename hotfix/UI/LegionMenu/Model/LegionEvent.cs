using System;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 军团事件集合
    /// </summary>
    public class LegionEvent
    {
        /// <summary>
        /// 显示军团面板
        /// </summary>
        public static Action LegionShowUI;
        /// <summary>
        /// 搜索界面快速加入军团
        /// </summary>
        public static Action SearchQuickJoinLegion;
        /// <summary>
        /// 搜索查询军团
        /// </summary>
        public static Action<string> SearchLegion;

        /// <summary>
        /// 发送创建军团
        /// </summary>
        public static Action<string, int> SendCreateLegionMsg;

        /// <summary>
        /// 关闭军团相关UI
        /// </summary>
        public static Action CloseLegionHudUI;

        /// <summary>
        /// 打开管理面板
        /// </summary>
        public static Action OpenManagerMenu;

        /// <summary>
        /// 发送邮件
        /// </summary>
        public static Action<string, string> SendLegionMail;

        /// <summary>
        /// 发送拒绝全部入会申请
        /// </summary>
        public static Action SendRejectTotalRequestJoin;

        /// <summary>
        /// 发送拒绝入会申请
        /// </summary>
        public static Action<long> SendRejectRequestJoin;

        /// <summary>
        /// 发送同意入会申请
        /// </summary>
        public static Action<long> SendConsentRequestJoin;

        /// <summary>
        /// 发送保存限制条件
        /// </summary>
        public static Action<int, bool> SendSaveLimit;

        /// <summary>
        /// 发送申请加入军团
        /// </summary>
        public static Action<int> SendApplyJoinLegion;

        /// <summary>
        /// 发送取消申请加入军团
        /// </summary>
        public static Action<int> SendCancelApplyJoinLegion;

        /// <summary>
        /// 发送军团公告
        /// </summary>
        public static Action<string> SendSaveLegionNotice;

        /// <summary>
        /// 发送退出军团
        /// </summary>
        public static Action SendLeaveLegion;

        /// <summary>
        /// 发送成员升职
        /// </summary>
        public static Action<long> SendMemberPromote;

        /// <summary>
        /// 发送成员降职
        /// </summary>
        public static Action<long> SendMemberDemote;

        /// <summary>
        /// 军团成员职位变动回调
        /// </summary>
        public static Action MemberPostChangeCallBack;

        /// <summary>
        /// 提示回调
        /// </summary>
        public static Action MessageCallBack;

        /// <summary>
        ///发送移交军团长
        /// </summary>
        public static Action<long> SendMemberGiveOwner;

        /// <summary>
        /// 发送被踢出军团
        /// </summary>
        public static Action<long> SendMemberKickOut;

        /// <summary>
        /// 发送添加好友
        /// </summary>
        public static Action<long> SendMemberAddFriend;

        /// <summary>
        /// 发送聊天
        /// </summary>
        public static Action<long> OpenMemberTalk;

        /// <summary>
        ///  发送金币捐献
        /// </summary>
        /// <returns></returns>
        public static Action SendGoldDonate;

        /// <summary>
        /// 发送钻石捐献
        /// </summary>
        public static Action SendDiamandDonate;

        /// <summary>
        /// 发送至尊捐献
        /// </summary>
        public static Action SendLuxuryDonate;

        /// <summary>
        /// 发送获取军团事件的消息
        /// </summary>
        public static Action SendGetLegionMessages;

        /// <summary>
        /// 发送给予军团月卡
        /// </summary>
        public static Action<long, Action<long>> SendGiveMonthCard;

        /// <summary>
        /// 通知军团改变
        /// </summary>
        public static Action<AllianceAccount> NotifyLegionAccount;

        /// <summary>
        /// 通知更新军团数据
        /// </summary>
        public static Action<LegionData> NotifyUpdateLegionData;

        /// <summary>
        /// 通知更新军团事件
        /// </summary>
        public static Action<List<MessageItemData>> NotifyUpdateLegionMessages;

        /// <summary>
        /// 通知更新军团列表
        /// </summary>
        public static Action<SearchItemData[]> NotifyUpdateSearchItemDatas;

        /// <summary>
        /// 通知被踢出军团
        /// </summary>
        public static Action NotifyByKickOut;

        /// <summary>
        /// 点击军团成员
        /// </summary>
        public static Action<long> OnClickMember;


    }
}