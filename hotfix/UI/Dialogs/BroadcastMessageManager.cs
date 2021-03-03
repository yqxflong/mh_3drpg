using Hotfix_LT.Data;
using System.Collections;

namespace Hotfix_LT.UI
{
	public class BroadcastMessageManager : ManagerUnit
	{
		public override void Initialize(EB.Sparx.Config config)
		{

		}

		public override void OnLoggedIn()
		{
			ProcessLoginRateMessage();
		}

		public override void Async(string message, object payload)
		{
			switch (message)
			{
				case "Words":
					OnWrodsMessage(payload);
					break;
				case "Normal":
					OnNormalMessage(payload);
					break;
				case "DeleteMessage":
					OnDeleteMessage(payload);
					break;
				case "NewTLEvent":
					OnNewTLEvent();
					break;
			}
		}

		/// <summary>
		/// tip_message
		/// {
		///	  t_id:2222222
		///   id:1222333
		///   param:[sss,ddf,fff,dd]
		/// }
		/// </summary>

		void OnNewTLEvent()
		{
			LTMainHudManager.Instance.UpdateTLEvent();
		}

		void OnWrodsMessage(object payload)
		{
			int id = EB.Dot.Integer("id", payload, 0);
			ArrayList formatParams = Hotfix_LT.EBCore.Dot.Array("param", payload, null);

			if (formatParams == null)
			{
				MessageTemplateManager.ShowMessage(id);
			}
			else
			{
	            if (id == 902087)//召唤伙伴
                {
					if (formatParams.Count < 2) return;
                    string str = (string)formatParams[1];
                    int heroid = 0;
                    if (int.TryParse(str, out heroid))
                    {
                        formatParams[1] = EB.Localizer.GetString(string.Format("ID_herostats_hero_infos_{0}_name", formatParams[1]));
                        switch (CharacterTemplateManager.Instance.GetHeroInfo(heroid).role_grade)
                        {
                            case 4:
                                formatParams.Add("SSR");
                                break;
                            case 5:
                                formatParams.Add("UR");
                                break;
                            default:
                                formatParams.Add("SSR");
                                break;
                        }
                    }
                }
                else
                {
					for (int i = 0; i < formatParams.Count; i++)
					{
						if (formatParams[i] != null)
						{
							formatParams[i] = GetString(formatParams[i].ToString());
						}
					}
				}
                MessageTemplateManager.ShowMessage(id, formatParams, null);
			}
		}

		/// <summary>
		/// tip_message
		/// {
		/// 	t_id:2222222
		///     type:[1,2]  显示方式 参考words表格
		///     localize:
		///     {
		///         id:xxxxxxx
		///         param:[ssss,dddf,f,f,dd]
		///     }
		/// 	s:222233333//开始时间
		///     e:222233333//结束时间
		///		d:10//间隔
		/// }
		/// </summary>
		void OnNormalMessage(object payload)
		{
			string t_id = string.Empty;
			t_id = EB.Dot.String("t_id", payload, t_id);

			if (string.IsNullOrEmpty(t_id))
			{
				ShowNormalMessage(payload);
			}
			else
			{
				int s = EB.Dot.Integer("s", payload, 0);
				int e = EB.Dot.Integer("e", payload, 0);
				int d = EB.Dot.Integer("d", payload, 0);
				DeltaActionExcuter action = new DeltaActionExcuter(t_id, s, e, d, "DeltaTipAction", payload);
				action.Register();
			}
		}

		void OnDeleteMessage(object payload)
		{
			string t_id = string.Empty;
			t_id = EB.Dot.String("t_id", payload, t_id);

			if (string.IsNullOrEmpty(t_id))
			{
				LTHotfixManager.GetManager<AutoRefreshingManager>().RemoveDeltaActionExcute(t_id);
			}
		}

		void ProcessLoginRateMessage()
		{
			ArrayList messages = null;
			DataLookupsCache.Instance.SearchDataByID<ArrayList>("RateMessage", out messages);

			if (messages != null)
			{
				for (int i = 0; i < messages.Count; i++)
				{
					string t_id = string.Empty;
					t_id = EB.Dot.String("t_id", messages[i], t_id);

					if (!string.IsNullOrEmpty(t_id))
					{
						int s = EB.Dot.Integer("s", messages[i], 0);
						int e = EB.Dot.Integer("e", messages[i], 0);
						int d = EB.Dot.Integer("d", messages[i], 0);
						DeltaActionExcuter action = new DeltaActionExcuter(t_id, s, e, d, "DeltaTipAction", messages[i]);
						action.Register();
					}
				}
			}
		}

		public static void ShowWordMessage(object payload)
		{
			int id = EB.Dot.Integer("id", payload, 0);
			ArrayList formatParams = Hotfix_LT.EBCore.Dot.Array("param", payload, null);

			if (formatParams == null)
			{
				MessageTemplateManager.ShowMessage(id);
			}
			else
			{
				for (int i = 0; i < formatParams.Count; i++)
				{
					formatParams[i] = GetString(formatParams[i].ToString());
				}

				MessageTemplateManager.ShowMessage(id, formatParams, null);
			}
		}

		public static void ShowNormalMessage(object payload)
		{
			ArrayList types = Hotfix_LT.EBCore.Dot.Array("type", payload, null);

			if (types == null)
			{
				return;
			}

			string content = LocalizeString(EB.Dot.Object("localize", payload, null));

			for (int j = 0; j < types.Count; j++)
			{
				eMessageUIType type = Hotfix_LT.EBCore.Dot.Enum<eMessageUIType>("", types[j], eMessageUIType.None);
				MessageTemplateManager.ShowMessage(type, content);
			}
		}

		public static string LocalizeString(object content)
		{
			string local = string.Empty;

			if (content == null)
			{
				return local;
			}

			string id = EB.Dot.String("id", content, string.Empty);
			ArrayList formatParams = Hotfix_LT.EBCore.Dot.Array("param", content, null);

			if (formatParams == null)
			{
				return GetString(id);
			}

			object[] translatedFotmatParams = new object[formatParams.Count];

			for (int i = 0; i < formatParams.Count; i++)
			{
				if (formatParams[i] != null)
				{
					string aa = formatParams[i].ToString();
					translatedFotmatParams[i] = GetString(aa);
				}
			}

			return string.Format(GetString(id), translatedFotmatParams);
		}

		public static string GetString(string id)
		{
			if (!string.IsNullOrEmpty(id))
			{
				if (id.StartsWith(EB.Symbols.LocIdPrefix))
				{
					return EB.Localizer.GetString(id);
				}
				else
				{
					int equipId;

					if (int.TryParse(id, out equipId))
					{
						EquipmentItemTemplate equip = EconemyTemplateManager.Instance.GetEquipment(equipId);

						if (equip != null)
						{
							return equip.Name;
						}
					}

					return id;
				}
			}
			else
			{
				return id;
			}
		}
	}

	public class DeltaTipAction : DeltaActionExcuter.DeltaAction
	{
		public override void Excute(object data)
		{
			int id = EB.Dot.Integer("id", data, 0);

			if (id > 0)
			{
				BroadcastMessageManager.ShowWordMessage(data);
			}
			else
			{
				BroadcastMessageManager.ShowNormalMessage(data);
			}
		}
	}
}