using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTSpeedSnatchHudUI : DynamicMonoHotfix
    {
        LTSpeedSnatchActiveItem[] _items;

        UIButton _bxBtn;
        UIButton _infoBtn;
        UIProgressBar _progress;
        ParticleSystemUIComponent _boxFx;

        public override void Awake()
        {
            base.Awake();

            if(!LTSpeedSnatchILRModel.GetInstance().IsActive) mDMono.gameObject.CustomSetActive(false);

            _items = new LTSpeedSnatchActiveItem[3];

            Transform itemTran = mDMono.transform.Find("Container/SnatchActiveItem1");
            _items[0] = new LTSpeedSnatchActiveItem(itemTran, mDMono.GetComponent<UIPanel>());
            itemTran = mDMono.transform.Find("Container/SnatchActiveItem2");
            _items[1] = new LTSpeedSnatchActiveItem(itemTran, mDMono.GetComponent<UIPanel>());
            itemTran = mDMono.transform.Find("Container/SnatchActiveItem3");
            _items[2] = new LTSpeedSnatchActiveItem(itemTran, mDMono.GetComponent<UIPanel>());

            _bxBtn = mDMono.transform.Find("Container/BxButton").GetComponent<UIButton>();
            _infoBtn = mDMono.transform.Find("Container/InfoButton").GetComponent<UIButton>();
            _progress = mDMono.transform.Find("Container/SnatchProgress").GetComponent<UIProgressBar>();

            _boxFx = _bxBtn.GetComponent <ParticleSystemUIComponent>();
            if (_boxFx != null) _boxFx.panel = mDMono.GetComponent<UIPanel>();

            _bxBtn.onClick.Add(new EventDelegate(OnClickBx));
            _infoBtn.onClick.Add(new EventDelegate(OnClickInfo));

            LTSpeedSnatchHotfixEvent.SpeedSnatchBase += OnSpeedSnatchBase;
            LTSpeedSnatchHotfixEvent.SpeedSnatchActive += OnSpeedSnatchActive;
            Hotfix_LT.Messenger.AddListener("LTSpeedSnatchEvent.DropFunc", OnDropFunc);
        }

        public override void Start()
        {
            base.Start();
            if (LTSpeedSnatchHotfixEvent.RefreshModel != null)
            {
                LTSpeedSnatchHotfixEvent.RefreshModel();
            }
        }

        public override void OnDestroy()
        {
            LTSpeedSnatchHotfixEvent.SpeedSnatchBase -= OnSpeedSnatchBase;
            LTSpeedSnatchHotfixEvent.SpeedSnatchActive -= OnSpeedSnatchActive;
            Hotfix_LT.Messenger.RemoveListener("LTSpeedSnatchEvent.DropFunc", OnDropFunc);
            base.OnDestroy();
        }

        private Coroutine _progressChange;
        void OnSpeedSnatchBase(int[] attr)
        {
            if(attr ==null)
            {
                for(int i=0;i< _items.Length;i++)
                {
                    _items[i].attributeIcon.gameObject.SetActive(false);
                    SetFxObjAction(_items[i]);
                    _items[i].wenSpt.gameObject.SetActive(true);
                }

                _progress.value = 0;

                if (_progressChange != null)
                {
                    EB.Coroutines.Stop(_progressChange);
                }
            }
            else
            {
                if(_progressChange!=null)
                {
                    EB.Coroutines.Stop(_progressChange);
                }
                for (int i = 0; i < _items.Length; i++)
                {
                    if (i >= attr.Length)
                    {
                        _items[i].wenSpt.gameObject.SetActive(true);
                        _items[i].attributeIcon.gameObject.SetActive(false);
                        SetFxObjAction(_items[i]);

                    }
                    else
                    {
                        _items[i].attributeIcon.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[(Hotfix_LT.Data.eRoleAttr)attr[i]]; 
                        SetFxObjAction(_items[i], (Hotfix_LT.Data.eRoleAttr)attr[i]);
                        _items[i].wenSpt.gameObject.SetActive(false);
                        _items[i].attributeIcon.gameObject.SetActive(true);
                    }
                }
                if (LTSpeedSnatchILRModel.GetInstance().firstEnterMainLand)
                {
                    LTSpeedSnatchILRModel.GetInstance().firstEnterMainLand = false;
                    LTSpeedSnatchILRModel.GetInstance().AttrLength = attr.Length;
                }
                if (LTSpeedSnatchILRModel.GetInstance().AttrLength!= attr.Length)
                {
                    _progressChange = EB.Coroutines.Run(ProgressChange(((float)attr.Length) / 3,(Hotfix_LT.Data.eRoleAttr)attr[attr.Length-1]));
                    LTSpeedSnatchILRModel.GetInstance().AttrLength = attr.Length;
                }
                else
                {
                    _progress.value = attr.Length*0.33f;
                }
            }
        }

        private void SetFxObjAction(LTSpeedSnatchActiveItem item, Hotfix_LT.Data.eRoleAttr attr = Hotfix_LT.Data.eRoleAttr.None)
        {
            switch (attr)
            {
                case Hotfix_LT.Data.eRoleAttr.Feng:
                    {
                        item.FxFeng.CustomSetActive(true);
                    }
                    break;
                case Hotfix_LT.Data.eRoleAttr.Huo:
                    {
                        item.FxHuo.CustomSetActive(true);
                    }
                    break;
                case Hotfix_LT.Data.eRoleAttr.Shui:
                    {
                        item.FxShui.CustomSetActive(true);
                    }
                    break;
                default: {
                        item.FxFeng.CustomSetActive(false);
                        item.FxHuo.CustomSetActive(false);
                        item.FxShui.CustomSetActive(false);
                    } break;
            }
        }

        IEnumerator ProgressChange(float f, Data.eRoleAttr attr = Data.eRoleAttr.None)
        {
            while(!SceneLogicManager.isMainlands()) //等到主城中
            {
                yield return null;
            }

            int buffIndex = -1;

            if (_progress != null)
            {
                if (f > 0.67)
                {
                    _progress.value = 0.67f;
                }
                else if (f > 0.34)
                {
                    _progress.value = 0.34f;
                }
            }

            float pv = _progress != null ? _progress.value : 0f;

            while (pv != f)
            {
                if (pv > f)
                {
                    pv -= 0.004f;

                    if (pv < f)
                    {
                        pv = f;
                    }
                }
                else if (pv < f)
                {
                    pv += 0.004f;

                    if (pv > f)
                    {
                        pv = f;
                    }
                }

                if (_progress != null)
                {
                    _progress.value = pv;
                }

                int index = 0;
                
                if(pv > 0.67)
                {
                    index = 2;
                }
                else if (pv > 0.34)
                {
                    index = 1;
                }
                else if (pv > 0)
                {
                    index = 0;
                }

                if (buffIndex != index)
                {
                    buffIndex = index;

                    if (_items != null)
                    {
                        for (int i = 0; i < _items.Length; i++)
                        {
                            if (i > index)
                            {
                                _items[i].wenSpt.gameObject.SetActive(true);
                                _items[i].attributeIcon.gameObject.SetActive(false);
                                SetFxObjAction(_items[i]);
                            }
                            else
                            {
                                _items[i].wenSpt.gameObject.SetActive(false);
                                _items[i].attributeIcon.gameObject.SetActive(true);
                                SetFxObjAction(_items[i], attr);
                            }
                        }
                    }
                }

                yield return null;
            }

            if (pv > 0.99f)
            {
                if (_boxFx != null)
                {
                    _boxFx.enabled = true;
                }

                yield return OpenAward();
            }
        }

        IEnumerator OpenAward()
        {
            yield return new WaitForSeconds(0.6f);
            OnOpenAward();
            yield return new WaitForSeconds(2);

            if (_boxFx != null)
            {
                _boxFx.enabled = false;
            }

            Hotfix_LT.Messenger.Raise("GetSpeedSnatchBaseData");
        }

        void OnSpeedSnatchActive(bool isActive)
        {
            mDMono.gameObject.CustomSetActive(isActive);
        }

        void OnDropFunc()
        {
            OnClickBx();
        }

        void OnOpenAward()
        {
            if (LTSpeedSnatchILRModel.GetInstance().listRewardShowItemData.Count > 0)
            {
                GlobalMenuManager.Instance.Open("LTShowRewardView", LTSpeedSnatchILRModel.GetInstance().listRewardShowItemData);
            }
        }

        void OnClickBx()
        {
            GlobalMenuManager.Instance.Open("LTSpeedSnatchAwardInfoHudUI", LTSpeedSnatchILRModel.GetInstance().ListGhostReward);
        }

        void OnClickInfo()
        {
            string text = EB.Localizer.GetString("ID_SPEED_SNATCH_RULES");
            GlobalMenuManager.Instance.Open("LTRuleUIView", text);
        }

    }

    class LTSpeedSnatchActiveItem
    {
        public Transform Root;
        public UISprite attributeIcon;
        public UISprite wenSpt;
        public GameObject FxFeng;
        public GameObject FxHuo;
        public GameObject FxShui;

        public LTSpeedSnatchActiveItem(Transform root,UIPanel panel =null)
        {
            this.Root = root;
            attributeIcon = Root.Find("Spt001").GetComponent <UISprite>();
            wenSpt = Root.Find("WenSpt").GetComponent<UISprite>();

            FxFeng = attributeIcon.transform .Find("Fx_feng").gameObject;
            FxFeng.GetComponent<ParticleSystemUIComponent>().panel = panel;
            FxHuo = attributeIcon.transform.Find("Fx_huo").gameObject;
            FxHuo.GetComponent<ParticleSystemUIComponent>().panel = panel;
            FxShui = attributeIcon.transform.Find("Fx_shui").gameObject;
            FxShui.GetComponent<ParticleSystemUIComponent>().panel = panel;
        }
    }
}
