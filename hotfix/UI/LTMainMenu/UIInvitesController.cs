using UnityEngine;
using System.Collections;
using _HotfixScripts.Utils;

/// <summary>
/// intact.invite
/// {
///  c:xxxx
///  expiry:xxx
///  id:xxxx
///  fromuid:xxx
///  fromname:“xxxx”
///  touids:[xxxx,xxxx]
///  tonames:["xxx",xxx]
/// }
/// </summary>
namespace Hotfix_LT.UI
{
    public class InvitesMessage
    {
    	public const string INVITE_PK = "social_pvp";
    	public const string INVITE_FRIEDS = "socail_frieds";
    	public const string ESCORT_HELP= "escort_help";
    	public const string FRIEND_OPERATE = "friend_operate";
    	protected System.Action m_callback;
    	public string mInviteType="";
    	public long mRecvTime;
    	public long mExpireTime;
    	public string mInviteId;
    	public long mFromUid;
    	public string mFromName;
    	public ArrayList mToNammes;

    	public InvitesMessage()//invite  answer：accept  answer：reject
    	{
    		mInviteType="";
    		mRecvTime = 0;
    		mExpireTime = 0;
    		mInviteId = "";
    		mFromUid = 0;
    		mFromName = "";
    		mToNammes =Johny.ArrayListPool.Claim();
    	}    
    
    	public InvitesMessage(Hashtable data)
    	{
    		mInviteType = EB.Dot.String("intact.invite.c",data,"");
    		mRecvTime = EB.Dot.Long("intact.invite.c", data, 0);
    		mExpireTime = EB.Dot.Long("intact.invite.e", data, 0);
    		mInviteId = EB.Dot.String("intact.invite._id", data, "");
    		mFromUid = EB.Dot.Long("intact.invite.s_uid", data, 0);
    		mFromName = EB.Dot.String("intact.invite.s_name", data, "");
    		//mToNammes = Hotfix_LT.EBCore.Dot.Array("intact.invite.t_names", data, null);
    
    		mToNammes = Johny.ArrayListPool.Claim();
    		ArrayList t_uidArray = Hotfix_LT.EBCore.Dot.Array("intact.invite.t_uids", data, null);

    		if (t_uidArray != null)
    		{
    			Hashtable infos = EB.Dot.Object("intact.invite.t_infos", data, Johny.HashtablePool.Claim());
    			for (int i = 0; i < t_uidArray.Count; ++i)
    			{
    				string receiveName = EB.Dot.String(t_uidArray[i] + ".name", infos, string.Empty);
    				if (receiveName != string.Empty)
    				{
    					mToNammes.Add(receiveName);
    				}
    				else
    				{
    					EB.Debug.LogError("receiveName==null uid={0}",t_uidArray[i]);
    				}
    			}
    		}
    		//else
    		//{
    		//	EB.Debug.LogError("t_uidArray==null");
    		//}
    	}

    	//判定是否超时
    	public bool IsExpire()
    	{
    		//DateTime dt = new DateTime(1970, 1, 1);
    		//TimeSpan d = System.DateTime.Now - dt;
    		//long seconddiff = d.Ticks / 10000000;
    		//if (seconddiff > mExpireTime) return true;
    		return false;
    	}
    
    	public void Play(System.Action callback)
    	{
    		m_callback = callback;
    		if (IsExpire())
    		{
    			if (m_callback != null) m_callback();
    			return;
    		}
    		RealPlay();
    	}
    
    	public virtual void RealPlay()
    	{
    		if (m_callback != null) m_callback();
    	}
    }
    
    public class PKInvitesMessage : InvitesMessage
    {
    	public PKInvitesMessage(Hashtable data):base(data)
    	{
    
    	}
    
    	public override void RealPlay()
    	{
    		var pc = PlayerManager.LocalPlayerController();

    		if (pc!=null && mFromUid == pc.playerUid)
    		{
    			if (m_callback != null)
    				m_callback();
    			return;
    		}
    
    		if (!SceneLogicManager.isMainlands() || SceneLogicManager.isLCCampaign())
    		{
    			return;
    		}
    
            if (LTLegionWarFinalController .Instance !=null)
            {
                return;
            }
    
    		System.Action<int> callBack = delegate (int value)
    		{
    			CallBack(value);
    		};
    
    		//MessageTemplateManager.ShowMessage(902068,Johny.HashtablePool.Claim() { { "0", mFromName } },CallBack);
    		long expireTs=mExpireTime - (long)EB.Time.Now;
    
    		Hashtable param = Johny.HashtablePool.Claim();
    		param["fromName"] = mFromName;
    		param["callBack"] = callBack;
    		param["expireTs"] = expireTs;
    
    		GlobalMenuManager.Instance.Open("PkReceiveRequestUI", param);
    		//PkReceiveRequestController.Open(param);
    		//UIStack.Instance.ExitStack(false);
    
    		////GlobalMenuManager.Instance.CloseMenu("ChatHudView");
    	}
    
    	public void CallBack(int value)
    	{
    		if (value == 0)
    		{
    			if (IsExpire()) return;
    			string[] invites = new string[] { mInviteId };
    			LTHotfixManager.GetManager<InvitesManager>().Accept(invites, OnAccept);
    		}
    		else
    		{
    			if (IsExpire()) return;
    			string[] invites = new string[] { mInviteId };
                LTHotfixManager.GetManager<InvitesManager>().Reject(invites, OnReject);
    		}
    		if(m_callback!=null)m_callback();
    	}
    
    	public void OnAccept(EB.Sparx.Response result)
    	{
    		EB.Debug.Log("OnAccept: {0}", result.text);
    		if (result.sucessful)
    		{
    			DataLookupsCache.Instance.CacheData(result.hashtable);
    		}
    		else
    		{
    			MessageTemplateManager.ShowMessage(902069);
    		}
    	}
    
    	public void OnReject(EB.Sparx.Response result)
    	{
    		EB.Debug.Log("OnReject: {0}", result.text);
    		if (result.sucessful)
    		{
    
    		}
    		else
    		{
    			MessageTemplateManager.ShowMessage(902069);
    		}
    	}
    }
    
    public class PKCancelMessage : InvitesMessage
    {
    	public PKCancelMessage(Hashtable data) : base(data)
    	{
    
    	}
    
    	public override void RealPlay()
    	{
    		if (m_callback != null)
    			m_callback();
    		
            Hotfix_LT.Messenger.Raise(EventName.PKCancelEvent);
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_UIInvitesController_4951"));
    	}
    }
    
    public class PKAcceptMessage : InvitesMessage
    {
    	public PKAcceptMessage(Hashtable data):base(data)
    	{
    
    	}
    	public override void RealPlay()
    	{
    		if (m_callback != null) m_callback();
    		EB.Debug.Log("PKAcceptMessage======================");
    	}
    }
    
    public class PKRejectMessage : InvitesMessage
    {
    	static public bool IsPkReject;
    	public PKRejectMessage(Hashtable data):base(data)
    	{
    
    	}
    	public override void RealPlay()
    	{
    		if (mFromUid != PlayerManager.LocalPlayerController().playerUid)
    		{
    			if (m_callback != null) m_callback();
    			return;
    		}
    		//Hashtable data = Johny.HashtablePool.Claim();
    		//string text;
    		//string localstr = EB.Localizer.GetString("ID_PK_REJECT");
    		//if (mToNammes != null && mToNammes.Count > 0) text = string.Format(localstr, mToNammes[0]);
    		//else
    		//{
    		//	text = string.Format(localstr, "");
    		//	EB.Debug.LogError("PKRejectMessage mToNames is empty");
    		//}
    
    		//EventManager.instance.Raise(new PkRejectEvent());
    		//data.Add("text", text);
    		//UIFlyingMessageManager.Instance.Play(data, "InviteReject", m_callback);
    		//if (m_callback != null) m_callback();
    		IsPkReject = true;
            Hotfix_LT.Messenger.Raise(EventName.PkRejectEvent);
    		MessageTemplateManager.ShowMessage(902146);
    	}
    }
    
    public class UIInvitesController : DynamicMonoHotfix, IHotfixUpdate
    {
    	private static UIInvitesController m_Instance;
    	public static UIInvitesController Instance { get{ return m_Instance; } }
    
    	public UIButton HelpOtherBtn;
    	public GameObject TransferDartingFlag;
    	public UILabel TranferDartCountdownLabel;
    	bool mTransferState;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            HelpOtherBtn = t.GetComponent<UIButton>("Escort/HelpOtherBtn");
            TransferDartingFlag = t.FindEx("Escort/EscortingFlag").gameObject;
            TranferDartCountdownLabel = t.GetComponent<UILabel>("Escort/EscortingFlag/Countdown");
            QueueSize = 50;

            t.GetComponent<UIButton>("Escort/HelpOtherBtn").onClick.Add(new EventDelegate(OnHandleOtherHelpReqBtnClick));
            t.GetComponent<UIEventTrigger>("Escort/EscortingFlag").onClick.Add(new EventDelegate(OnRequestHelpBtnClick));

    		m_Instance = this;
    		m_MessageQueue = new EB.Collections.Queue<InvitesMessage>(QueueSize);
    
    		var im = LTHotfixManager.GetManager<InvitesManager>();
    		im.OnAcceptListener += OnAcceptListener;
    		im.OnRejectListener += OnRejectListener;
    		im.OnInviteListener += OnInviteListener;
    		im.OnRequestListener += OnRequestListener;
    		im.OnRemoveTargetListener += OnRemoveTargetListener;
    		im.OnRemoveInviteListener += OnRemoveInviteListener;
    	}

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}
        public override void OnDisable()
        {
            base.OnDisable();
            ErasureMonoUpdater();
        }
        public override void OnDestroy()
        {
    		m_Instance = null;
    
    		var im = LTHotfixManager.GetManager<InvitesManager>();
    		im.OnAcceptListener -= OnAcceptListener;
    		im.OnRejectListener -= OnRejectListener;
    		im.OnInviteListener -= OnInviteListener;
    		im.OnRequestListener -= OnRequestListener;
    		im.OnRemoveTargetListener -= OnRemoveTargetListener;
    		im.OnRemoveInviteListener -= OnRemoveInviteListener;
    	}
    
    	public void UpdateBtnState()
    	{
    		bool helpOtherCondition = AlliancesManager.Instance.HelpApplyInfo.HelpApplyList.Count != 0 && HelpApplyController.GetResidueHelpApplyCount() > 0 && Hotfix_LT.Data.EventTemplateManager.Instance.IsTimeOK("escort_start", "escort_stop");
    		HelpOtherBtn.gameObject.SetActive(helpOtherCondition);
    
    		mTransferState = AllianceUtil.IsInTransferDart;
    		TransferDartingFlag.SetActive(mTransferState);
    	}

        public void Update()
    	{
    		if (!mTransferState)
    			return;
    
    		System.TimeSpan transferCountdownTs = System.TimeSpan.FromSeconds(AlliancesManager.Instance.TransferDartInfo.TransferEndTs - EB.Time.Now);
    		if (transferCountdownTs.TotalSeconds > 0f)
    		{
    			if(transferCountdownTs.TotalSeconds<3*60)
    				LTUIUtil.SetText(TranferDartCountdownLabel,LT.Hotfix.Utility.ColorUtility.GetFormatColorStr(LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal,string.Format("{0:00}:{1:00}", transferCountdownTs.Minutes, transferCountdownTs.Seconds)));
    			else
    				LTUIUtil.SetText(TranferDartCountdownLabel, LT.Hotfix.Utility.ColorUtility.GetFormatColorStr(LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal, string.Format("{0:00}:{1:00}", transferCountdownTs.Minutes, transferCountdownTs.Seconds)));
    		}
    		else
    			LTUIUtil.SetText(TranferDartCountdownLabel,LT.Hotfix.Utility.ColorUtility.GetFormatColorStr(LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal, "00:00"));
    	}
    
    	public void OnRequestHelpBtnClick()
    	{
    	    FusionAudio.PostEvent("UI/General/ButtonClick");
    		GlobalMenuManager.Instance.Open("LTApplyHelpUI");
    	}
    
    	public void OnHandleOtherHelpReqBtnClick()
    	{
    		GlobalMenuManager.Instance.Open("LTAppliesUI");
    	}
    
    	private void OnAcceptListener(InviteData payload)
    	{
    		if (payload.Catalog != InvitesMessage.INVITE_PK && payload.Catalog != InvitesMessage.INVITE_FRIEDS)
    			return;
    		Hashtable data = Johny.HashtablePool.Claim();;
    		Hashtable intact = Johny.HashtablePool.Claim();;
    		intact["invite"] = payload.ToJson();
    		data["intact"] = intact;
    		PutOneMessage(new PKAcceptMessage(data));
    	}
    
    	private void OnRejectListener(InviteData payload)
    	{
    		if (payload.Catalog != InvitesMessage.INVITE_PK && payload.Catalog != InvitesMessage.INVITE_FRIEDS)
    			return;
    		Hashtable data = Johny.HashtablePool.Claim();
    		Hashtable intact = Johny.HashtablePool.Claim();
    		intact["invite"] = payload.ToJson();
    		data["intact"] = intact;
    		PutOneMessage(new PKRejectMessage(data));
    	}
    
    	private void OnInviteListener(InviteData payload)
    	{
    		if (payload.Catalog != InvitesMessage.INVITE_PK && payload.Catalog != InvitesMessage.INVITE_FRIEDS) return;
    		Hashtable data = Johny.HashtablePool.Claim();
    		Hashtable intact = Johny.HashtablePool.Claim();
    		intact["invite"] = payload.ToJson();
    		data["intact"] = intact;
    		PutOneMessage(new PKInvitesMessage(data));
    	}
    
    	private void OnCancelListener(InviteData payload)  //处理对方取消切磋消息
    	{
    		if (payload.Catalog != InvitesMessage.INVITE_PK && payload.Catalog != InvitesMessage.INVITE_FRIEDS) return;
    		Hashtable data = Johny.HashtablePool.Claim();
    		Hashtable intact = Johny.HashtablePool.Claim();
    		intact["invite"] = payload.ToJson();
    		data["intact"] = intact;
    		PutOneMessage(new PKCancelMessage(data));
    	}
    
    	private void OnRequestListener(InviteData payload)
    	{
    		if (payload.Catalog == InvitesMessage.ESCORT_HELP)
    		{
    			long localUid =LoginManager.Instance.LocalUserId.Value;
    			for (var i = 0; i < payload.ReceiverUids.Length; i++)
    			{
                    var receiver = payload.ReceiverUids[i];

                    if (receiver.Equals(localUid))
    				{
    					AlliancesManager.Instance.GetHelpApplyInfo(delegate (Hashtable result) {
    						Hashtable list = EB.Dot.Object("escortAndRob.helpApply.list", result, null);
    						HelpOtherBtn.gameObject.SetActive(list.Count>0);				
    					});
    					break;
    				}
    			}
    		}
    	}
    
    	private void OnRemoveTargetListener(RemoveData payload)
    	{
    		if (payload.Catalog == InvitesMessage.ESCORT_HELP)
    		{
    			if (payload.TargetUid != AllianceUtil.GetLocalUid().ToString())
    				return;
    			AlliancesManager.Instance.GetHelpApplyInfo(delegate(Hashtable result) {
    				Hashtable list=EB.Dot.Object("escortAndRob.helpApply.list", result, null);
    				if (list != null && list.Count == 0)
    				{
    					HelpOtherBtn.gameObject.SetActive(false);
    				}
    			});
    		}
    	}
    
    	private void OnRemoveInviteListener()  //处理对方取消切磋消息
    	{
    		PutOneMessage(new PKCancelMessage(Johny.HashtablePool.Claim()));
    	}
    
    	//消息队列
    	private EB.Collections.Queue<InvitesMessage> m_MessageQueue;
    	public int QueueSize = 50;
    	//private bool m_IsPlaying = false;
    
    	public void PutOneMessage(InvitesMessage message)
    	{
    		if (m_MessageQueue.Count < QueueSize)
    		{
    			m_MessageQueue.Enqueue(message);
    			PlayOneMessage();
    		}
    	}
    
    	void PlayOneMessage()
    	{
    		if (SceneLogicManager.getSceneType()!="mainlands") return;
    		//判定当前是否正在播放
    		//if (!m_IsPlaying)
    		//{
    			//从队列里面取出一条，播放
    			if (m_MessageQueue.Count >= 1)
    			{
                //m_IsPlaying = true;
                mDMono.gameObject.SetActive(true);
    				if (mDMono.gameObject.activeInHierarchy)
    				{
    					StartCoroutine(Play(m_MessageQueue.Dequeue()));
    				}
    				else
    					m_MessageQueue.Dequeue();
    			}
    			else//说明没有需要播放的 就隐掉
    			{
                mDMono.gameObject.SetActive(false);
    			}
    		//}
    		//else
    		//{
    
    		//}
    	}
    
    	IEnumerator Play(InvitesMessage message)
    	{
    		//
    		yield return new WaitForEndOfFrame();
    		message.Play(OnPlayEnd);
    		yield break;
    	}
    
    	void OnPlayEnd()
    	{
    		//m_IsPlaying = false;
    		PlayOneMessage();//取下一条
    	}
    }
}
