using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Hotfix_LT.UI
{
    public enum eMessageUIType
    {
        None = 0,
        MessageDialogue_1 = 1,
        MessageDialogue_2 = 2,
        MessageDialogue_3 = 3,
        MessageDialogue_4 = 4,

        FloatingText = 5,
        CenterRollingMessage = 6,
        IMPrivateMessage = 7,
        IMPublicMessage = 8,
        IMAllianceMessage = 9,
        IMTeamMessage = 10,
        IMSystemMessage = 11,
        CombatPowerText = 12,
    }

    public class MessageTemplate
    {
        public const string OkBtn = "ID_DIALOG_TITLE_CONFIRM";
        public const string CancelBtn = "ID_DIALOG_BUTTON_CANCEL";
        public const string Title = "ID_DIALOG_TITLE_TIPS";
        public delegate void OnClose(int result); // 0 = OK, 1 = Cancel, 2 = Close.
        public int id;
        public string context;

        public MessageTemplate()
        {
            id = 0;
            context = "";
        }

        public MessageTemplate(GM.DataCache.WordInfo data)
        {
            id = data.Id;
            context = EB.Localizer.GetTableString(string.Format("ID_guide_words_{0}_context", id), data.Context);//;
        }

        public virtual void ShowMessage(object data, OnClose callback)
        {

        }

        public string FormatMessage(object data)
        {
            if (data == null) return context;

            MatchEvaluator matchEval = delegate (Match match)
            {
                // replaces "{index}" by data
                string dataIDIndex = match.Value.Trim('{', '}');
                return EB.Dot.String(dataIDIndex, data, "");
            };
            Regex regex = new Regex("{[^{}]+}"); // matches "{index}", ex : "{22}"
            string processedString = regex.Replace(context, matchEval);
            return processedString;
        }
    }

    public class MessageListItem
    {
        public int id;
        public List<MessageTemplate> items;

        public MessageListItem()
        {
            id = 0;
            items = new List<MessageTemplate>();
        }
    }

    public class MessageListItemComparer : IComparer<MessageListItem>
    {
        public int Compare(MessageListItem x, MessageListItem y)
        {
            return x.id - y.id;
        }
    }

    public class MessageTemplateManager
    {
        static public int HcBuyMsgId = 901030;
        static public int GoldBuyMsgId = 901031;
        static public int VigorBuyMsgId = 902085;
        static public int NoOpenTipsId = 902013;

        private static MessageTemplateManager s_instance;
        private MessageListItem[] mMessageTemplates = new MessageListItem[0];

        public static MessageTemplateManager Instance
        {
            get { return s_instance = s_instance ?? new MessageTemplateManager(); }
        }

        private MessageTemplateManager()
        {

        }

        public static void ClearUp()
        {
            if (s_instance != null)
            {
                s_instance.mMessageTemplates = null;
            }
        }

        public bool InitFromDataCache(GM.DataCache.ConditionGuide messages)
        {
            if (messages == null)
            {
                EB.Debug.LogError("can not find messages data");
                return false;
            }

            var conditionSet = messages;
            mMessageTemplates = new MessageListItem[conditionSet.WordsLength];

            for (int i = 0; i < conditionSet.WordsLength; ++i)
            {
                var message = conditionSet.GetWords(i);
                mMessageTemplates[i] = new MessageListItem();
                string types = message.Type;
                int id = message.Id;
                mMessageTemplates[i].id = id;

                if (string.IsNullOrEmpty(types))
                { 
                    continue; 
                }

                string[] type_arry = types.Split(',');

                if (type_arry == null || type_arry.Length == 0)
                {
                    continue;
                }

                MessageTemplate mt = null;

                for (int j = 0; j < type_arry.Length; j++)
                {
                    //eMessageUIType type = Hotfix_LT.EBCore.Dot.Enum<eMessageUIType>("", type_arry[j], eMessageUIType.None);
                    eMessageUIType type;
                    try
                    { 
                        type = (eMessageUIType)int.Parse(type_arry[j]);
                    }
                    catch
                    {
                        type = eMessageUIType.None;
                    }
                    switch (type)
                    {
                        case eMessageUIType.MessageDialogue_1:
                            mt = new MessageDialogue_1(message);
                            break;
                        case eMessageUIType.MessageDialogue_2:
                            mt = new MessageDialogue_2(message);
                            break;
                        case eMessageUIType.MessageDialogue_3:
                            mt = new MessageDialogue_3(message);
                            break;
                        case eMessageUIType.MessageDialogue_4:
                            mt = new MessageDialogue_4(message);
                            break;
                        case eMessageUIType.FloatingText:
                            mt = new FloatingText(message);
                            break;
                        case eMessageUIType.CenterRollingMessage:
                            mt = new CenterRollingMessage(message);
                            break;
                        case eMessageUIType.IMPrivateMessage:
                            mt = new IMPrivateMessage(message);
                            break;
                        case eMessageUIType.IMPublicMessage:
                            mt = new IMPublicMessage(message);
                            break;
                        case eMessageUIType.IMAllianceMessage:
                            mt = new IMAllianceMessage(message);
                            break;
                        case eMessageUIType.IMTeamMessage:
                            mt = new IMTeamMessage(message);
                            break;
                        case eMessageUIType.IMSystemMessage:
                            mt = new IMSystemMessage(message);
                            break;
                        default:
                            EB.Debug.LogWarning("MessageTemplate type is elegal for {0}" , message.Id);
                            mt = new MessageTemplate(message);
                            break;
                    }

                    mMessageTemplates[i].items.Add(mt);
                }
            }

            System.Array.Sort(mMessageTemplates, new MessageListItemComparer());
            return true;
        }

        /// <summary>
        /// 获取消息处理方式列表
        /// </summary>
        /// <param name="tplId"></param>
        /// <returns></returns>
        public List<MessageTemplate> GetMessageTemplate(int tplId)
        {
            int i = 0, j = (mMessageTemplates != null) ? mMessageTemplates.Length - 1 : -1;

            while (i <= j)
            {
                int m = (i - j) / 2 + j; // (i + j) / 2

                if (mMessageTemplates[m].id > tplId)
                {
                    j = m - 1;
                }
                else if (mMessageTemplates[m].id < tplId)
                {
                    i = m + 1;
                }
                else
                {
                    return mMessageTemplates[m].items;
                }
            }

            return null;
        }

        /// <summary>
        /// 显示一个消息，具体这个消息的显示形式由配表决定，显示时机跟具体显示系统调度有关
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        public static void ShowMessage(int id, object data, MessageTemplate.OnClose callback)
        {
            if (s_instance == null)
            {
                return;
            }

            List<MessageTemplate> tmps = s_instance.GetMessageTemplate(id);

            if (tmps == null)
            {
                EB.Debug.LogError("No MessageTemplate for id={0}" , id);
                return;
            }

            for (int i = 0; i < tmps.Count; i++)
            {
                if (i == 0)
                {
                    tmps[i].ShowMessage(data, callback);
                }
                else
                {
                    tmps[i].ShowMessage(data, null);
                }
            }
        }

        public static void ShowMessage(int id)
        {
            ShowMessage(id, null, null);
        }

        public static void ShowMessageFromILR(string type, string content, bool hasCallback = false)
        {
            MessageTemplate.OnClose callback = null;

            if (hasCallback)
            {
                callback = (int r) =>
                {
                    if (r == 0)
                    {
                        LTGameSettingController.SetGameQualityLevel(UnityEngine.QualitySettings.GetQualityLevel() + 1);
                    }
                };
            }

            ShowMessage((eMessageUIType)Enum.Parse(typeof(eMessageUIType), type), content, callback);
        }

        public static void ShowMessage(eMessageUIType type, string content, MessageTemplate.OnClose callback = null)
        {
            switch (type)
            {
                case eMessageUIType.MessageDialogue_1:
                    if (SceneLogic.SceneState == SceneLogic.eSceneState.SceneLoop)
                    {
                        MessageDialog.HideCurrent();
                        MessageDialog.Show(EB.Localizer.GetString(MessageTemplate.Title), content, EB.Localizer.GetString(MessageTemplate.OkBtn), null, true, true, true, delegate (int result)
                        {
                            if (callback != null)
                            {
                                callback(result);
                            }
                        });
                    }
                    break;
                case eMessageUIType.MessageDialogue_2:
                    if (SceneLogic.SceneState == SceneLogic.eSceneState.SceneLoop)
                    {
                        MessageDialog.HideCurrent();
                        MessageDialog.Show(EB.Localizer.GetString(MessageTemplate.Title), content, EB.Localizer.GetString(MessageTemplate.OkBtn), EB.Localizer.GetString(MessageTemplate.CancelBtn), true, false, true, delegate (int result)
                        {
                            if (callback != null)
                            {
                                callback(result);
                            }
                        });
                    }
                    break;
                case eMessageUIType.MessageDialogue_3:
                    if (SceneLogic.SceneState == SceneLogic.eSceneState.SceneLoop)
                    {
                        MessageDialog.HideCurrent();
                        MessageDialog.Show(EB.Localizer.GetString(MessageTemplate.Title), content, EB.Localizer.GetString(MessageTemplate.OkBtn), null, false, true, false, delegate (int result)
                        {
                            if (callback != null)
                            {
                                callback(result);
                            }
                        });
                    }
                    break;
                case eMessageUIType.MessageDialogue_4:
                    if (SceneLogic.SceneState == SceneLogic.eSceneState.SceneLoop)
                    {
                        MessageDialog.HideCurrent();
                        MessageDialog.Show(EB.Localizer.GetString(MessageTemplate.Title), content, EB.Localizer.GetString(MessageTemplate.OkBtn), EB.Localizer.GetString(MessageTemplate.CancelBtn), false, false, false, delegate (int result)
                        {
                            if (callback != null)
                            {
                                callback(result);
                            }
                        });
                    }
                    break;
                case eMessageUIType.FloatingText:
                    FloatingUITextManager.ShowFloatingText(content);
                    break;
                case eMessageUIType.CombatPowerText:
                    FloatingCombatPowerTextManager.ShowFloatingText(content);
                    break;
                case eMessageUIType.CenterRollingMessage:
                    if (null != UIBroadCastMessageController.Instance)
                    {
                        UIBroadCastMessageController.Instance.PutOneMessage(content);
                    }
                    break;
                case eMessageUIType.IMPrivateMessage:
                    SparxHub.Instance.ChatManager.HandleSystemMessage(content);
                    break;
                case eMessageUIType.IMPublicMessage:
                    SparxHub.Instance.ChatManager.HandlePublicMessage(content);
                    break;
                case eMessageUIType.IMAllianceMessage:
                    SparxHub.Instance.ChatManager.HandleAllianceMessage(content); ;
                    break;
                case eMessageUIType.IMTeamMessage:
                    SparxHub.Instance.ChatManager.HandleTeamMessage(content);
                    break;
                case eMessageUIType.IMSystemMessage:
                    SparxHub.Instance.ChatManager.HandleSystemMessage(content);
                    break;
                default:
                    EB.Debug.LogWarning("MessageTemplate type is elegal for {0}" , type);
                    break;
            }
        }
    }

    /// <summary>
    /// okBtn
    /// </summary>
    public class MessageDialogue_1 : MessageTemplate
    {
        public MessageDialogue_1(GM.DataCache.WordInfo data) : base(data) { }

        public override void ShowMessage(object data, OnClose callback)
        {
            MessageDialog.HideCurrent();
            string content = FormatMessage(data);
            MessageDialog.Show(EB.Localizer.GetString(Title), content, EB.Localizer.GetString(OkBtn), null, true, true, true, delegate (int result)
            {
                if (callback != null)
                {
                    callback(result);
                }
            });
        }
    }

    /// <summary>
    /// okBtn+cancelBtn
    /// </summary>
    public class MessageDialogue_2 : MessageTemplate
    {
        public MessageDialogue_2(GM.DataCache.WordInfo data) : base(data) { }

        public override void ShowMessage(object data, OnClose callback)
        {
            MessageDialog.HideCurrent();
            string content = FormatMessage(data);
            MessageDialog.Show(EB.Localizer.GetString(Title), content, EB.Localizer.GetString(OkBtn), EB.Localizer.GetString(CancelBtn), true, false, true, delegate (int result)
            {
                if (callback != null)
                {
                    callback(result);
                }
            });
        }
    }

    /// <summary>
    /// title+closebtn+okBtn
    /// </summary>
    public class MessageDialogue_3 : MessageTemplate
    {
        public MessageDialogue_3(GM.DataCache.WordInfo data) : base(data) { }

        public override void ShowMessage(object data, OnClose callback)
        {
            MessageDialog.HideCurrent();
            string content = FormatMessage(data);
            MessageDialog.Show(EB.Localizer.GetString(Title), content, EB.Localizer.GetString(OkBtn), null, false, true, false, delegate (int result)
            {
                if (callback != null)
                {
                    callback(result);
                }
            });
        }
    }

    /// <summary>
    /// title+closebtn+okBtn+cancelBtn
    /// </summary>
    public class MessageDialogue_4 : MessageTemplate
    {
        public MessageDialogue_4(GM.DataCache.WordInfo data) : base(data) { }

        public override void ShowMessage(object data, OnClose callback)
        {
            MessageDialog.HideCurrent();
            string content = FormatMessage(data);
            MessageDialog.Show(EB.Localizer.GetString(Title), content, EB.Localizer.GetString(OkBtn), EB.Localizer.GetString(CancelBtn), false, false, false, delegate (int result)
            {
                if (callback != null)
                {
                    callback(result);
                }
            });
        }
    }


    public class FloatingText : MessageTemplate
    {
        public FloatingText(GM.DataCache.WordInfo data) : base(data) { }

        public override void ShowMessage(object data, OnClose callback)
        {
            string content = FormatMessage(data);
            FloatingUITextManager.ShowFloatingText(content);
        }
    }

    public class CenterRollingMessage : MessageTemplate
    {
        public CenterRollingMessage(GM.DataCache.WordInfo data) : base(data) { }

        public override void ShowMessage(object data, OnClose callback)
        {
            string content = FormatMessage(data);

            if (null != UIBroadCastMessageController.Instance)
            {
                UIBroadCastMessageController.Instance.PutOneMessage(content);
            }
        }
    }

    public class IMPrivateMessage : MessageTemplate
    {
        public IMPrivateMessage(GM.DataCache.WordInfo data) : base(data) { }

        public override void ShowMessage(object data, OnClose callback)
        {
            string content = FormatMessage(data);
            SparxHub.Instance.ChatManager.HandleSystemMessage(content);
        }
    }

    public class IMPublicMessage : MessageTemplate
    {
        public IMPublicMessage(GM.DataCache.WordInfo data) : base(data) { }

        public override void ShowMessage(object data, OnClose callback)
        {
            string content = FormatMessage(data);
            SparxHub.Instance.ChatManager.HandlePublicMessage(content);
        }
    }

    public class IMAllianceMessage : MessageTemplate
    {
        public IMAllianceMessage(GM.DataCache.WordInfo data) : base(data) { }

        public override void ShowMessage(object data, OnClose callback)
        {
            string content = FormatMessage(data);
            SparxHub.Instance.ChatManager.HandleAllianceMessage(content);
        }
    }

    public class IMTeamMessage : MessageTemplate
    {
        public IMTeamMessage(GM.DataCache.WordInfo data) : base(data) { }

        public override void ShowMessage(object data, OnClose callback)
        {
            string content = FormatMessage(data);
            SparxHub.Instance.ChatManager.HandleTeamMessage(content);
        }
    }

    public class IMSystemMessage : MessageTemplate
    {
        public IMSystemMessage(GM.DataCache.WordInfo data) : base(data) { }

        public override void ShowMessage(object data, OnClose callback)
        {
            string content = FormatMessage(data);
            SparxHub.Instance.GetManager<EB.Sparx.ChatManager>().HandleSystemMessage(content);
        }
    }
}