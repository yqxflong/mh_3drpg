using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTChallengeInstanceThemeHudController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            ItemTmp = t.GetMonoILRComponent<LTChallengeInstanceThemeItemTemp>("Item");
            DescLabel = t.GetComponent<UILabel>("Bottom/UIPanel/Desc");
            Grid = t.GetComponent<UIGrid>("Scroll/Grid");
            Scroll = t.GetComponent<UIScrollView>("Scroll");
            TipUIObj = t.FindEx("Bottom/UIPanel/Tip").gameObject;
            LeftTipBGSprite = t.GetComponent<UISprite>("Bottom/UIPanel/Left");
            RightTipBGSprite = t.GetComponent<UISprite>("Bottom/UIPanel/Right");

            t.GetComponent<UIButton>("BG").onClick.Add(new EventDelegate(OnCancelButtonClick));

            InitUI();
        }
        
        public LTChallengeInstanceThemeItemTemp ItemTmp;
    
        public UILabel DescLabel;
    
        public float FullDistance = 1365;
    
        public float Scale = 0.2f;
    
        public UIGrid Grid;
    
        public UIScrollView Scroll;
    
        public GameObject TipUIObj;
    
        public UISprite LeftTipBGSprite, RightTipBGSprite;
    
        private float mFrom = 250, mTo = 1160;
    
        public override bool IsFullscreen()
        {
            return true;
        }
    
        private bool IsAlienMazeTheme=false;
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            IsAlienMazeTheme = param != null;
            LeftTipBGSprite.width = RightTipBGSprite.width = (int)mFrom;
        }
    
        private bool m_guideToolState = false;
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            if (MengBanController.Instance.controller.gameObject.activeSelf)
            {
                m_guideToolState = true;
                MengBanController.Instance.controller.transform .parent .gameObject.CustomSetActive(false);
            }
    
            if (IsAlienMazeTheme)
            {
                Scroll.gameObject.CustomSetActive(false);
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                FusionAudio.PostEvent("UI/New/Style", true);
                yield return new WaitForSeconds(2);
            }
            yield return InitSelect(LTInstanceMapModel.Instance.ChallengeThemeId);
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            if (m_guideToolState)
            {
                m_guideToolState = false;
                MengBanController.Instance.controller.transform.parent.gameObject.CustomSetActive(true);
            }
            TipUIObj.CustomSetActive(false);
            Hotfix_LT.Messenger.Raise(EventName.OnShowDoorEvent);
            DestroySelf();
            yield break;
        }
    
        private void InitUI( )
        {
            InitScroll();
        }
    
        private void InitScroll()
        {
            foreach (var pair in Hotfix_LT.Data.SceneTemplateManager.Instance.GetAllLostChallengeEnv())
            {
                if(pair.Value.Icon!= pair.Value.Pic) CreateItem(pair);
            }
            Grid.Reposition();
        }
    
        private void CreateItem(KeyValuePair<int, Hotfix_LT.Data.LostChallengeEnv> pair)
        {
            GameObject itemObj = GameEngine.Instantiate(ItemTmp.mDMono.gameObject);
            itemObj.transform.SetParent(Grid.transform);
            itemObj.transform.localPosition = Vector3.zero;
            itemObj.transform.localScale = Vector3.one;
            itemObj.GetMonoILRComponent<LTChallengeInstanceThemeItemTemp>().InitData(pair.Value, IsAlienMazeTheme);
            itemObj.name = pair.Value.Name;
            itemObj.CustomSetActive(true);
        }
    
        private bool mIsSlect = false;
    
        private IEnumerator InitSelect(int id)
        {
            if (!IsAlienMazeTheme)
            {
                float time = 0;
                while (time < 0.5f)
                {
                    float target = Mathf.Lerp(6 * -750, (6 + 1) * -750, (0.5f - time) / 0.5f);
                    Scroll.MoveRelative(new Vector3(target, 0, 0));
                    time += Time.deltaTime;
                    yield return null;
                }
    
                time = 0;
                while (time < 2f)
                {
                    float target = Mathf.Lerp(6 * -750, (6 + 1) * -750, time / 2f);
                    Scroll.MoveRelative(new Vector3(target, 0, 0));
                    time += Time.deltaTime;
                    yield return null;
                }
                Scroll.gameObject.CustomSetActive(false);
            }
            var tpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostChallengeEnvById(id);
            if (tpl != null)
            {
                DescLabel.gameObject.CustomSetActive(false);
                DescLabel.text = tpl.Desc;
                ItemTmp.InitData(tpl, IsAlienMazeTheme);
                ItemTmp.mDMono.gameObject.CustomSetActive(true);
                if (IsAlienMazeTheme)
                {
                    ItemTmp.ShowTween();
                }
                else
                {
                    ItemTmp.ShowFx();
                }
            }
            yield return new WaitForSeconds(0.4f);
            float value = 0;
            while (value<=1.0f)
            {
                value += Time.deltaTime*3;
                LeftTipBGSprite.width = RightTipBGSprite.width = (int)(Mathf.Clamp(mTo * value, mFrom, mTo));
                if (value > 1)
                {
                    DescLabel.gameObject.CustomSetActive(true);
                    if (IsAlienMazeTheme)
                    {
                        FusionAudio.PostEvent("UI/New/N", true);
                    }
                    yield return new WaitForSeconds(0.8f);
                    TipUIObj.CustomSetActive(true);
                    mIsSlect = true;
                    FusionAudio.PostEvent("UI/New/Style", false);
                    yield break;
                }
                yield return  null;
            }
    
        }
    
        public override void OnCancelButtonClick()
        {
            if (mIsSlect)
            {
                FusionAudio.PostEvent("UI/New/Style", false);
                FusionAudio.PostEvent("UI/New/N", false);
                controller.Close();
            }
        }
    }
}
