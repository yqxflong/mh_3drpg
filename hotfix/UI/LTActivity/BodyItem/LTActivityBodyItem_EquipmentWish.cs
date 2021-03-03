using System.Collections;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 套装许愿
    /// </summary>
    public class LTActivityBodyItem_EquipmentWish : LTActivityBodyItem
    {
        private UILabel _titleLabel;
        private DynamicUISprite _wishButtonIcon;

        public override void Awake()
        {
            base.Awake();
        }

        public override void SetData(object data)
        {
            base.SetData(data);

            var btn = mDMono.transform.GetComponent<UIButton>("WishButton");

            if (btn != null)
            {
                if (state.Equals("pending"))
                {
                    btn.gameObject.SetActive(false);
                    return;
                }

                btn.gameObject.SetActive(true);
                btn.onClick.Add(new EventDelegate(OnWishButtonClicked));
            }

            if (!state.Equals("running") || EB.Time.Now > fintime)
            {
                return;
            }

            int activityId = EB.Dot.Integer("activity_id", data, 0);
            int itemId;

            if (DataLookupsCache.Instance.SearchIntByID(string.Format("tl_acs.{0}.current", activityId), out itemId))
            {
                RefreshWishIcon(itemId);
            }

            ArrayList arrayList;
            int count = 0;

            if (DataLookupsCache.Instance.SearchDataByID(string.Format("tl_acs.{0}.wish_reward", activityId), out arrayList) && arrayList != null)
            {
                for (var i = 0; i < arrayList.Count; i++)
                {
                    string idStr = System.Convert.ToInt32(arrayList[i]).ToString();
                    
                    if (idStr.Length == 4)
                    {
                        count += 6;
                    }
                    else if (idStr.Length == 6)
                    {
                        count += 1;
                    }
                }
            }

            SetTitleLabel(count);

            if (NavLabel != null)
            {
                NavLabel.text = EB.Localizer.GetString("ID_DIALOG_BUTTON_GO");
            }

            if (NavButton != null)
            {
                NavButton.onClick.Clear();
                NavButton.onClick.Add(new EventDelegate(() => GlobalMenuManager.Instance.Open("LTChallengeInstanceSelectHud")));
            }
        }

        public void RefreshWishIcon(int itemId)
        {
            string iconName = string.Empty;
            int suit = itemId % 100;
            SuitTypeInfo suitInfo = EconemyTemplateManager.Instance.GetSuitTypeInfoByEcidSuitType(suit);

            if (suitInfo != null)
            {
                iconName = suitInfo.SuitIcon;
            }

            SetWishButtonIcon(iconName);

            if (title != null)
            {
                title.UpdateRedPoint();
            }
        }

        private void SetTitleLabel(int num)
        {
            if (_titleLabel == null)
            {
                _titleLabel = mDMono.transform.GetComponent<UILabel>("TitleLabel");
            }

            if (_titleLabel != null)
            {
                _titleLabel.text = string.Format(EB.Localizer.GetString("ID_EQUIPMENT_WISH_GET"), num);
            }
        }

        private void SetWishButtonIcon(string spriteName)
        {
            if (_wishButtonIcon == null)
            {
                _wishButtonIcon = mDMono.transform.GetComponent<DynamicUISprite>("WishButton/Icon");
            }

            if (_wishButtonIcon != null)
            {
                _wishButtonIcon.spriteName = spriteName;
            }
        }

        private void OnWishButtonClicked()
        {
            if (!state.Equals("running") || EB.Time.Now > fintime)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_uifont_in_LTLegionWarQualify_End_4"));
                return;
            }

            //副本进行中不能更换许愿装备
            //int curLevel = 0;
            //DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.curLevel", out curLevel);

            //if (curLevel > 0)
            //{
            //    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_EQUIPMENT_WISH_NOT_REPLACEMENT_TIPS"));
            //    return;
            //}

            System.Action<int> callback = RefreshWishIcon;
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTEquipmentWishUI", callback);
        }
    }
}