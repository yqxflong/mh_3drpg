using EB.Sparx;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
	public class ChatUIMessage
	{
		private const string AudioColor = "FFF348FF";
		private const string TextColor = "FFFFFFFF";

		public string ChannelSpriteName
		{
			get; set;
		}

		public ChatRule.CHAT_CHANNEL Channel
		{
			get; set;
		}

		public EB.Sparx.ChatMessage Message
		{
			get; set;
		}

		public ChatUIMessage(EB.Sparx.ChatMessage msg)
		{
			Message = msg;
		}

		public string GetPreviewString()
		{
			string nameString = GetNameString();

			// nameString为空串则表示是系统消息，系统消息不需要使用冒号
			string str1 = "[{0}]{1}";
			if (Message.isAudio)
			{
				if (Message.uid == LoginManager.Instance.LocalUserId.Value)
				{
					return string.Format(str1, AudioColor, EB.Localizer.GetString("ID_codefont_in_ChatController_832"));
				}
				else
				{
					return string.Format(str1, AudioColor, EB.Localizer.GetString("ID_codefont_in_ChatController_872"));
				}
			}
			else
			{
				return string.Format("[{0}]{1}", TextColor, Message.text);
			}
		}

		public string GetNameString()
		{
			string nameString = string.Empty;
			if (!string.IsNullOrEmpty(Message.name))
			{
				nameString = string.Format("[00000000][{0}][\r{1}\r][-][-]", Message.uid > 0 ? "42C2F3FF" : "ff0000ff", Message.name);
			}
			return nameString;
		}

		static Dictionary<char, string> sVipNumberDict = new Dictionary<char, string>()
		{
			{'0', "${Maininterface_Font_Chat_Ling}" },
			{'1', "${Maininterface_Font_Chat_Yi}" },
			{'2', "${Maininterface_Font_Chat_Er}" },
			{'3', "${Maininterface_Font_Chat_San}" },
			{'4', "${Maininterface_Font_Chat_Si}" },
			{'5', "${Maininterface_Font_Chat_Wu}" },
			{'6', "${Maininterface_Font_Chat_Liu}" },
			{'7', "${Maininterface_Font_Chat_Qi}" },
			{'8', "${Maininterface_Font_Chat_Ba}" },
			{'9', "${Maininterface_Font_Chat_Jiu}" },
		};

		static string ConvertVipLevel(int level)
		{
			if (level < 0)
			{
				level = 0;
			}


			string lvl = level.ToString();
			string result = "";
			for (int i = 0; i < lvl.Length; ++i)
			{
				result += sVipNumberDict[lvl[i]];
			}
			return result;
		}

		public string GetVipLevelString()
		{
			return ConvertVipLevel(Message.vipLevel);
		}

		public int GetLevel()
		{
			return Message.level;
		}

		public string GetLeftAudioString()
		{
			float length = Message.isAudio ? Message.audioClip.length : 0f;
			length = length * 2;
			float maxLength = 10f;
			float minLength = 2f;
			if (length < minLength)
			{
				length = minLength;
			}
			if (length > maxLength)
			{
				length = maxLength;
			}
			return string.Format("[FFFFFF00]{0}", new string('0', Mathf.CeilToInt(length)));
		}

		public string GetRightAudioString()
		{
			float length = Message.isAudio ? Message.audioClip.length : 0f;
			length *= 2;
			float maxLength = 10f;
			float minLength = 2f;
			if (length < minLength)
			{
				length = minLength;
			}
			if (length > maxLength)
			{
				length = maxLength;
			}
			return string.Format("[FFFFFF00]{0}[-]", new string('0', Mathf.CeilToInt(length)));
		}

		public string GetAudioDurationString()
		{
			return string.Format(EB.Localizer.GetString("ID_codefont_in_ChatController_3281"), Message.isAudio ? Mathf.CeilToInt(Message.audioClip.length) : 0);
		}

		public string GetText()
		{
			return string.Format("{0}", Message.text);
		}
	}

	public class ChatController : InstanceBase<ChatController>
	{
		#region chat data
		public EB.Sparx.ChatMessage[] GetChatItemDataList()
		{
			return SparxHub.Instance.ChatManager.Messages;
		}

		// public EB.Sparx.ChatMessage[] GetChatItemDataList(ChatRule.CHAT_CHANNEL _channelType)
		// {
		// 	string channel = ChatRule.CHANNEL2STR[_channelType];
		// 	return SparxHub.Instance.ChatManager.GetMessages(channel);
		// }
		#endregion

		#region channel
		public long TargetPrivateUid { get; set; }
		public string TargetPrivateName { get; set; }
		// public string TargetPrivateTag
		// {
		// 	get
		// 	{
		// 		EB.Uri uri = new EB.Uri();
		// 		uri.SetComponent(EB.Uri.Component.Protocol, "private");
		// 		uri.SetComponent(EB.Uri.Component.Host, TargetPrivateUid.ToString());

		// 		return string.Format("[url={1}][00ff00ff]@{0}[-][/url]", TargetPrivateName, uri.ToString());
		// 	}
		// }

		public bool IsShowChannelWorld
		{
			get { return SparxHub.Instance.ChatManager.IsJoined(ChatRule.CHANNEL2STR[ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_WORLD]); }
			set
			{
				if (value)
				{
					RequestJoinChat(ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_WORLD);
				}
				else
				{
					RequestLeaveChat(ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_WORLD);
				}
			}
		}

		public bool IsShowChannelTeam
		{
			get { return SparxHub.Instance.ChatManager.IsJoined(ChatRule.CHANNEL2STR[ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_TEAM]); }
			set
			{
				if (value)
				{
					RequestJoinChat(ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_TEAM);
				}
				else
				{
					RequestLeaveChat(ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_TEAM);
				}
			}
		}

		public bool IsShowChannelAlliance
		{
			get { return SparxHub.Instance.ChatManager.IsJoined(ChatRule.CHANNEL2STR[ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCE]); }
			set
			{
				if (value)
				{
					RequestJoinChat(ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCE);
				}
				else
				{
					RequestLeaveChat(ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCE);
				}
			}
		}

		public bool IsShowChannelNation
		{
			get { return SparxHub.Instance.ChatManager.IsJoined(ChatRule.CHANNEL2STR[ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_NATION]); }
			set
			{
				if (value)
				{
					RequestJoinChat(ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_NATION);
				}
				else
				{
					RequestLeaveChat(ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_NATION);
				}
			}
		}

		public bool IsShowChannelPrivate
		{
			get { return SparxHub.Instance.ChatManager.IsJoined(ChatRule.CHANNEL2STR[ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_PRIVATE]); }
			set
			{
				if (value)
				{
					RequestJoinChat(ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_PRIVATE);
				}
				else
				{
					RequestLeaveChat(ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_PRIVATE);
				}
			}
		}

		// public bool IsShowChannelAllianceWar
		// {
		// 	get { return SparxHub.Instance.ChatManager.IsJoined(ChatRule.CHANNEL2STR[ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCEWAR]); }
		// 	set
		// 	{
		// 		if (value)
		// 		{
		// 			RequestJoinChat(ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCEWAR);
		// 		}
		// 		else
		// 		{
		// 			RequestLeaveChat(ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCEWAR);
		// 		}
		// 	}
		// }

		#endregion

		#region server request

		private string FullChannelStringForJoin(ChatRule.CHAT_CHANNEL _eChannel)
		{
			string channel = "";
			if (ChatRule.CHANNEL2STR.ContainsKey(_eChannel))
				channel = ChatRule.CHANNEL2STR[_eChannel];
			else
				EB.Debug.LogError("ChatRule.CHANNEL2STR not ContainsKey _eChannel ={0}", _eChannel);

			if (_eChannel == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_TEAM)
			{

			}
			else if (_eChannel == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_WORLD)
			{
				if (Hub.Instance == null || LoginManager.Instance == null || LoginManager.Instance.LocalUser == null)
				{
					return channel;
				}
				int id = LoginManager.Instance.LocalUser.RealmId;
				if (id <= 0)
				{
					id = LoginManager.Instance.LocalUser.WorldId;
				}
				channel += string.Format("_{0}", id);
			}
			else if (_eChannel == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_PRIVATE)
			{
				if (Hub.Instance == null || LoginManager.Instance == null || LoginManager.Instance.LocalUser == null)
				{
					return channel;
				}
				channel += string.Format("_{0}", LoginManager.Instance.LocalUser.Id.Value);
			}
			else if (_eChannel == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCE)
			{
				if (NationManager.Instance == null || NationManager.Instance.Account == null)
				{
					return channel;
				}
				channel += string.Format("_{0}", AlliancesManager.Instance.Account.AllianceId);
			}
			else if (_eChannel == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_NATION)
			{
				if (NationManager.Instance == null || NationManager.Instance.Account == null)
				{
					return channel;
				}
				channel += string.Format("_{0}", NationManager.Instance.Account.NationName);
			}
			else if (_eChannel == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCEWAR)
			{
				if (LTLegionWarManager.Instance == null)
				{
					return channel;
				}
				channel += string.Format("_{0}_{1}", LTLegionWarManager.Instance.SemiFinalField, LTLegionWarManager.Instance.FieldType);
			}

			return channel;
		}

		private string FullChannelStringForSend(ChatRule.CHAT_CHANNEL _eChannel)
		{
			string channel = "";
			if (ChatRule.CHANNEL2STR.ContainsKey(_eChannel))
				channel = ChatRule.CHANNEL2STR[_eChannel];
			else
				EB.Debug.LogError("ChatRule.CHANNEL2STR not ContainsKey _eChannel ={0}" , _eChannel);
			if (_eChannel == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_TEAM)
			{

			}
			else if (_eChannel == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_WORLD)
			{
				int id = LoginManager.Instance.LocalUser.RealmId;
				if (id <= 0)
					id = LoginManager.Instance.LocalUser.WorldId;
				channel += string.Format("_{0}", id);
			}
			else if (_eChannel == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_PRIVATE)
			{
				channel += string.Format("_{0}", ChatController.instance.TargetPrivateUid);
			}
			else if (_eChannel == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCE)
			{
				channel += string.Format("_{0}", AlliancesManager.Instance.Account.AllianceId);
			}
			else if (_eChannel == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_NATION)
			{
				channel += string.Format("_{0}", NationManager.Instance.Account.NationName);
			}
			else if (_eChannel == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCEWAR)
			{
				channel += string.Format("_{0}_{1}", LTLegionWarManager.Instance.SemiFinalField, LTLegionWarManager.Instance.FieldType);
			}

			return channel;
		}

		private void OnAllianceAccount(string path, INodeData data)
		{
			AllianceAccount account = AlliancesManager.Instance.Account;// data as AllianceAccount;
			IsShowChannelAlliance = account.State == eAllianceState.Joined;
		}

		private void OnNationAccount(string path, INodeData data)
		{
			NationAccount account = NationManager.Instance.Account;// data as NationAccount;
			IsShowChannelNation = !string.IsNullOrEmpty(account.NationName);
		}

		public void RequestJoinChat(ChatRule.CHAT_CHANNEL _eChannel)
		{
			var cm = SparxHub.Instance.ChatManager;
			string channelType = ChatRule.CHANNEL2STR[_eChannel];
			if (cm.IsJoined(channelType))
			{
				return;
			}

			string channel = FullChannelStringForJoin(_eChannel);
			cm.Join(channel, null,
				delegate (string error, object param)
				{
					if (!string.IsNullOrEmpty(error))
					{
						EB.Debug.LogWarning("RequestJoinChat:{0}", error);
						return;
					}

					if (_eChannel == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_PRIVATE)
						return;
					if (ChatRule.IS_NEED_REQUEST_HISTORY[_eChannel])
					{
						RequestChatHistory(_eChannel);
					}
				});
		}

		public void RequestLeaveChat(ChatRule.CHAT_CHANNEL _eChannel)
		{
			var cm = SparxHub.Instance.ChatManager;
			string channelType = ChatRule.CHANNEL2STR[_eChannel];
			if (!cm.IsJoined(channelType))
			{
				return;
			}

			string channel = FullChannelStringForJoin(_eChannel);
			cm.Leave(channel, null,
				delegate (string error, object param)
				{
					if (!string.IsNullOrEmpty(error))
					{
						EB.Debug.LogWarning("RequestLeaveChat:{0}", error);
						return;
					}
				});
		}

		public void RequestSendChat(ChatRule.CHAT_CHANNEL _eChannel, string _content)
		{
			var msg = NewChatMessage(_eChannel, _content);
			var ht = Johny.HashtablePool.Claim();
			ht.Add("name", msg.name);
			ht.Add("uid", msg.uid);
			ht.Add("level", msg.level);
			ht.Add("vip_level", msg.vipLevel);
			ht.Add("head_icon", msg.icon);
			ht.Add("head_frame", msg.frame);
			ht.Add("quality", msg.quality);
			ht.Add("battle_rating", msg.battleRating);
			ht.Add("alliance_name", msg.allianceName);
			ht.Add("month_vip_type", msg.monthVipType);
			ht.Add("worldId", LoginManager.Instance.LocalUser.WorldId);
			ht.Add("server_name", LoginManager.Instance.GetDefaultGameWorld(LoginManager.Instance.LocalUser.WorldId).Name);

			SparxHub.Instance.ChatManager.SendText(msg.channel, msg.text, ht,
				delegate (string error, object param)
				{
					if (!string.IsNullOrEmpty(error))
					{
						EB.Debug.LogError("RequestSendChat:{0}", error);
					}
				});
			TaskSystem.RequestChatTaskFinish(_eChannel);
		}

		public void RequestSendChat(ChatRule.CHAT_CHANNEL _eChannel, string _content, long privateUid, string privateName)
		{
			var msg = NewChatMessage(_eChannel, _content);
			var ht = Johny.HashtablePool.Claim();
			ht.Add("name", msg.name);
			ht.Add("uid", msg.uid);
			ht.Add("level", msg.level);
			ht.Add("vip_level", msg.vipLevel);
			ht.Add("head_icon", msg.icon);
			ht.Add("head_frame", msg.frame);
			ht.Add("quality", msg.quality);
			ht.Add("battle_rating", msg.battleRating);
			ht.Add("alliance_name", msg.allianceName);
			ht.Add("private_uid", privateUid);
			ht.Add("private_name", privateName);
			ht.Add("month_vip_type", msg.monthVipType);
			ht.Add("worldId", LoginManager.Instance.LocalUser.WorldId);
			ht.Add("server_name", LoginManager.Instance.GetDefaultGameWorld(LoginManager.Instance.LocalUser.WorldId).Name);

			SparxHub.Instance.ChatManager.SendText(msg.channel, msg.text, ht,
				delegate (string error, object param)
				{
					if (!string.IsNullOrEmpty(error))
					{
						//eResponseCodeUIExtensions.ShowErrorDialogue(EB.Localizer.GetString("ID_SPARX_NETWORK_ERROR"));
						EB.Debug.LogError("RequestSendChat:{0}", error);
					}
				});
			TaskSystem.RequestChatTaskFinish(_eChannel);
		}

		public void RequestSendChat(ChatRule.CHAT_CHANNEL _eChannel, AudioClip _audio, int samplePos)
		{
			var msg = NewChatMessage(_eChannel, string.Empty);
			var ht = Johny.HashtablePool.Claim();
            ht.Add("name", msg.name);
            ht.Add("uid", msg.uid);
            ht.Add("level", msg.level);
            ht.Add("vip_level", msg.vipLevel);
            ht.Add("head_icon", msg.icon);
            ht.Add("head_frame", msg.frame);
            ht.Add("quality", msg.quality);
            ht.Add("battle_rating", msg.battleRating);
            ht.Add("alliance_name", msg.allianceName);
            ht.Add("month_vip_type", msg.monthVipType);
			ht.Add("worldId", LoginManager.Instance.LocalUser.WorldId);
			ht.Add("server_name", LoginManager.Instance.GetDefaultGameWorld(LoginManager.Instance.LocalUser.WorldId).Name);

			SparxHub.Instance.ChatManager.SendAudio(msg.channel, _audio, samplePos, ht,
				delegate (string error, object param)
				{
					if (!string.IsNullOrEmpty(error))
					{
						//eResponseCodeUIExtensions.ShowErrorDialogue(EB.Localizer.GetString("ID_SPARX_NETWORK_ERROR"));
						EB.Debug.LogError("RequestSendChat:{0}", error);
					}
				});
			TaskSystem.RequestChatTaskFinish(_eChannel);
		}

		public void RequestSendChat(ChatRule.CHAT_CHANNEL _eChannel, AudioClip _audio, int samplePos, long privateUid, string privateName)
		{
			var msg = NewChatMessage(_eChannel, string.Empty);
			var ht = Johny.HashtablePool.Claim();
			ht.Add("name", msg.name);
			ht.Add("uid", msg.uid);
			ht.Add("level", msg.level);
			ht.Add("vip_level", msg.vipLevel);
			ht.Add("head_icon", msg.icon);
			ht.Add("head_frame", msg.frame);
			ht.Add("quality", msg.quality);
			ht.Add("battle_rating", msg.battleRating);
			ht.Add("alliance_name", msg.allianceName );
			ht.Add("private_uid", privateUid );
			ht.Add("private_name", privateName );
			ht.Add("month_vip_type", msg.monthVipType);
			ht.Add("worldId", LoginManager.Instance.LocalUser.WorldId);
			ht.Add("server_name", LoginManager.Instance.GetDefaultGameWorld(LoginManager.Instance.LocalUser.WorldId).Name);

			SparxHub.Instance.ChatManager.SendAudio(msg.channel, _audio, samplePos, ht,
				delegate (string error, object param)
				{
					if (!string.IsNullOrEmpty(error))
					{
						//eResponseCodeUIExtensions.ShowErrorDialogue(EB.Localizer.GetString("ID_SPARX_NETWORK_ERROR"));
						EB.Debug.LogError("RequestSendChat:{0}", error);
					}
				});
			TaskSystem.RequestChatTaskFinish(_eChannel);
		}

		public void RequestChatHistory(ChatRule.CHAT_CHANNEL _eChannel)
		{
			string channel = FullChannelStringForJoin(_eChannel);
			SparxHub.Instance.ChatManager.History(channel, null,
				delegate (string error, object param)
				{
					if (!string.IsNullOrEmpty(error))
					{
						EB.Debug.LogWarning("RequestChatHistory:{0}", error);
					}
				});
		}
		#endregion

		public bool CanSend(string _content)
		{
			if (string.IsNullOrEmpty(_content))
				return false;
			return true;
		}

		public EB.Sparx.ChatMessage NewChatMessage(ChatRule.CHAT_CHANNEL _eChannel, string _content)
		{
			EB.Sparx.ChatMessage msg = new EB.Sparx.ChatMessage();

			msg.channel = FullChannelStringForSend(_eChannel);
			msg.channelType = ChatRule.CHANNEL2STR[_eChannel];
			msg.name = LoginManager.Instance.LocalUser.Name;
			msg.vipLevel = LoginManager.Instance.LocalUser.Vip;
			msg.icon = LTMainHudManager.Instance.UserHeadIcon;// LoginManager.Instance.LocalUser.Icon;
            msg.frame = LTMainHudManager.Instance.UserLeaderHeadFrameStr;
            msg.uid = LoginManager.Instance.LocalUser.Id.Value;

			msg.text = _content;

			msg.monthVipType = 0;
			bool isSilverVip = LTChargeManager.Instance.IsSilverVIP();
			bool isGoldVip = LTChargeManager.Instance.IsGoldVIP();
			if (isSilverVip && !isGoldVip)
			{
				msg.monthVipType = 1;
			}
			else if (!isSilverVip && isGoldVip)
			{
				msg.monthVipType = 2;
			}
			else if (isSilverVip && isGoldVip)
			{
				msg.monthVipType = 3;
			}

			string templateId;
			DataLookupsCache.Instance.SearchDataByID("playstate.MainLand.template_id", out templateId);
			//Hotfix_LT.Data.HeroStatTemplate charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(templateId);

			msg.quality = 0;//charTpl.quality_level;
			msg.allianceName = AlliancesManager.Instance.Detail.Name;

            //string teamIdDataId = string.Format("userTeam.current_team");
            //string teamId = string.Empty;
            //if (DataLookupsCache.Instance.SearchDataByID(teamIdDataId, out teamId))
            //{
            //	msg.battleRating = AttributesManager.CalcTeamFightWithEquipedEquipment(teamId);
            //}
            msg.battleRating = 0;

            DataLookupsCache.Instance.SearchDataByID("level", out msg.level);

			return msg;
		}

		public static void InitFromILR()
		{
			instance.Init();
		}

		public void Init()
		{
			RefreshChannel();

			GameDataSparxManager.Instance.RegisterListener(AlliancesManager.accountDataId, OnAllianceAccount);
			GameDataSparxManager.Instance.RegisterListener(NationManager.AccountDataId, OnNationAccount);
		}

		public static void CleanFromILR()
		{
			instance.Clean();
		}

		public void Clean()
		{
			GameDataSparxManager.Instance.UnRegisterListener(AlliancesManager.accountDataId, OnAllianceAccount);
			GameDataSparxManager.Instance.UnRegisterListener(NationManager.AccountDataId, OnNationAccount);

			IsShowChannelWorld = false;
			IsShowChannelPrivate = false;
			IsShowChannelTeam = false;
			IsShowChannelAlliance = false;
			IsShowChannelNation = false;
		}

		public static void RefreshChannelFromILR(bool force = false)
		{
			instance.RefreshChannel(force);
		}

		public void RefreshChannel(bool force = false)
		{
			if (force)
			{
				IsShowChannelWorld = false;
				IsShowChannelPrivate = false;
				IsShowChannelTeam = false;
				IsShowChannelAlliance = false;
				IsShowChannelNation = false;
			}

			IsShowChannelWorld = true;
			IsShowChannelPrivate = true;
			// no team
			IsShowChannelTeam = false;
			IsShowChannelAlliance = AlliancesManager.Instance.Account.State == eAllianceState.Joined;
			IsShowChannelNation = !string.IsNullOrEmpty(NationManager.Instance.Account.NationName);
		}
	}
}