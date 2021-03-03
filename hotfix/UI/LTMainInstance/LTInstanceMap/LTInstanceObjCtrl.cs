using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class InstanceObj
    {
        public GameObject Obj;
        public bool IsUsed = false;
    
        public int x = 0;
        public int y = 0;
    
        public bool IsFx = false;
    
        public void HideSelf()
        {
            if (Obj != null)
            {
                Obj.transform.localPosition = Vector3.zero;
            }
            IsUsed = false;
            x = 0;
            y = 0;
        }
    }
    
    /// <summary>
    /// 副本地图格子上动态物件控制类
    /// </summary>
    public class LTInstanceObjCtrl : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            Panel = t.GetComponent<UIPanel>();
            ContainerObj = t.FindEx("Center/ObjectContainer/DynamicObjContainer").gameObject;

            // FanzhuanObj = (GameObject)mDMono.ObjectParamList[0];
            // ShengjiFanzhuanObj = (GameObject)mDMono.ObjectParamList[1];
            // WenyiFanzhuanObj = (GameObject)mDMono.ObjectParamList[2];
            // QuickBattleObj = (GameObject)mDMono.ObjectParamList[3];
            ControllerObj = (GameObject)mDMono.ObjectParamList[4];
            GuideNoticeObj = (GameObject)mDMono.ObjectParamList[5];
            HeroNoticeObj = (GameObject)mDMono.ObjectParamList[6];
        }
        
        public UIPanel Panel;
    
        public GameObject ContainerObj;

        #region Particle Effect
        // public GameObject FanzhuanObj;

        // public GameObject ShengjiFanzhuanObj;

        // public GameObject WenyiFanzhuanObj;

        // private List<InstanceObj> FanzhuanObjList = new List<InstanceObj>();

        // private List<InstanceObj> ShengjiFanzhuanObjList = new List<InstanceObj>();
    
        // private List<InstanceObj> WenyiFanzhuanObjList = new List<InstanceObj>();

        // public GameObject QuickBattleObj;
        // private List<InstanceObj> QuickBattleObjList = new List<InstanceObj>();
        #endregion

        public GameObject ControllerObj;
        private List<InstanceObj> ControllerObjList = new List<InstanceObj>();
    
        public GameObject GuideNoticeObj;
        public List<InstanceObj> GuideNoticeObjList = new List<InstanceObj>();
    
        public GameObject HeroNoticeObj;
        public List<InstanceObj> HeroNoticeObjList = new List<InstanceObj>();

        private void PrepareFanzhuanEffects()
        {
            // for(int i = 0; i < FanzhuanObjList.Count; i++)
            // {
            //     var fanzhuan = FanzhuanObjList[i];
            //     ParticleSystem ps = fanzhuan.Obj.GetComponent<ParticleSystem>();
            //     ps.Play();
            //     fanzhuan.HideSelf();
            // }
        }

        public override void Start()
        {
            #region Particle Effect
            // InitObjList(FanzhuanObj, FanzhuanObjList, 5, true);
            // InitObjList(ShengjiFanzhuanObj, ShengjiFanzhuanObjList, 5, true);
            // InitObjList(WenyiFanzhuanObj, WenyiFanzhuanObjList, 5, true);
            // PrepareFanzhuanEffects();
            // InitObjList(QuickBattleObj, QuickBattleObjList, 3, true);
            #endregion

            InitObjList(ControllerObj, ControllerObjList, 30);
            InitObjList(GuideNoticeObj, GuideNoticeObjList, 10);
            InitObjList(HeroNoticeObj, HeroNoticeObjList, 3);
        }
    
        private void InitObjList(GameObject obj, List<InstanceObj> list, int count, bool isFx = false)
        {
            for (int i = 0; i < count; i++)
            {
                InstanceObj instanceObj = new InstanceObj();
                instanceObj.Obj = CreateObj(obj, isFx);
                instanceObj.IsUsed = false;
                instanceObj.x = 0;
                instanceObj.y = 0;
                instanceObj.IsFx = true;
                list.Add(instanceObj);
            }
        }
    
        private GameObject CreateObj(GameObject obj, bool isFx)
        {
            if (obj == null)
            {
                return null;
            }
            GameObject tempObj = GameObject.Instantiate(obj);
            tempObj.transform.SetParent(ContainerObj.transform);
            tempObj.transform.localPosition = Vector3.zero;
            tempObj.transform.localScale = Vector3.one;
            if (isFx)
            {
                SetSortingOrder sso = tempObj.GetComponent<SetSortingOrder>();
                if (sso == null)
                {
                    sso = tempObj.AddComponent<SetSortingOrder>();
                }
                sso.SortingOrder = Panel.sortingOrder + 1;
            }
            return tempObj;
        }
    
        public void ShowFanzhuanFX(Vector3 pos)
        {
            if (LTInstanceMapModel.Instance.ChallengeThemeEnvStr.Equals("D"))//瘟疫//LTInstanceMapModel.Instance.ChallengeThemeId == 2|| LTInstanceMapModel.Instance.ChallengeThemeId==103)
            {
                // for (int i = 0; i < WenyiFanzhuanObjList.Count; i++)
                // {
                //     if (!WenyiFanzhuanObjList[i].IsUsed)
                //     {
                //         var wenyi = WenyiFanzhuanObjList[i];
                //         ParticleSystem ps = wenyi.Obj.GetComponent<ParticleSystem>();
                //         ps.Play();
                //         wenyi.Obj.transform.position = pos;
                //         wenyi.IsUsed = true;
                //         ILRTimerManager.instance.AddTimer(1200, 1, delegate
                //         {
                //             if (wenyi != null)
                //             {
                //                 wenyi.HideSelf();
                //             }
                //         });
                //         break;
                //     }
                // }
                //var act = Hotfix_LT.Instance.LTInstanceOptimizeManager.Instance.HoldPlayerEffect("fx_fb_fanzhuan_wenyi");
                //act.SetForceFinishDuring(1200);
            }
            else if(LTInstanceMapModel.Instance.ChallengeThemeEnvStr.Equals("H"))//生机//LTInstanceMapModel.Instance.ChallengeThemeId == 4)
            {
                // for (int i = 0; i < ShengjiFanzhuanObjList.Count; i++)
                // {
                //     if (!ShengjiFanzhuanObjList[i].IsUsed)
                //     {
                //         var shengji = ShengjiFanzhuanObjList[i];
                //         ParticleSystem ps = shengji.Obj.GetComponent<ParticleSystem>();
                //         ps.Play();
                //         shengji.Obj.transform.position = pos;
                //         shengji.IsUsed = true;
                //         ILRTimerManager.instance.AddTimer(1200, 1, delegate
                //         {
                //             if (shengji != null)
                //             {
                //                 shengji.HideSelf();
                //             }
                //         });
                //         break;
                //     }
                // }
                //var act = Hotfix_LT.Instance.LTInstanceOptimizeManager.Instance.HoldPlayerEffect("fx_fb_fanzhuan_jiaxue");
                //act.SetForceFinishDuring(1200);
            }
            else
            {
                // for (int i = 0; i < FanzhuanObjList.Count; i++)
                // {
                //     if (!FanzhuanObjList[i].IsUsed)
                //     {
                //         var fanzhuan = FanzhuanObjList[i];
                //         ParticleSystem ps = fanzhuan.Obj.GetComponent<ParticleSystem>();
                //         ps.Play();
                //         fanzhuan.Obj.transform.position = pos;
                //         fanzhuan.IsUsed = true;
                //         ILRTimerManager.instance.AddTimer(1200, 1, delegate
                //         {
                //             if (fanzhuan != null)
                //             {
                //                 fanzhuan.HideSelf();
                //             }
                //         });
                //         break;
                //     }
                // }
                //var act = Hotfix_LT.Instance.LTInstanceOptimizeManager.Instance.HoldPlayerEffect("fx_fb_fanzhuan");
                //act.SetForceFinishDuring(1200);
            }
        }
    
        public void ShowQuickFX(Vector3 pos)
        {
            // for (int i = 0; i < QuickBattleObjList.Count; i++)
            // {
            //     if (!QuickBattleObjList[i].IsUsed)
            //     {
            //         var quickbattle = QuickBattleObjList[i];
            //         quickbattle.Obj.transform.position = pos;
            //         quickbattle.IsUsed = true;
            //         ILRTimerManager.instance.AddTimer(1200, 1, delegate
            //         {
            //             if (quickbattle != null)
            //             {
            //                 quickbattle.HideSelf();
            //             }
            //         });
            //         break;
            //     }
            // }
            var act = Hotfix_LT.Instance.LTInstanceOptimizeManager.Instance.HoldPlayerEffect("fx_ui_KSZD");
            act.SetForceFinishDuring(1200);
        }
    
        private Dictionary<Vector2, bool> ControllerObjUsedDic = new Dictionary<Vector2, bool>(); 
    
        public void ShowControllerObj(Transform tran, int x, int y)
        {
            if (ControllerObjUsedDic.ContainsKey(new Vector2(x, y)))
            {
                return;
            }
            for (int i = 0; i < ControllerObjList.Count; i++)
            {
                if (!ControllerObjList[i].IsUsed)
                {
                    ControllerObjList[i].Obj.transform.SetParent(tran);
                    ControllerObjList[i].Obj.transform.localPosition = Vector3.zero;
                    ControllerObjList[i].IsUsed = true;
                    ControllerObjList[i].x = x;
                    ControllerObjList[i].y = y;
                    ControllerObjUsedDic.Add(new Vector2(x, y), true);
                    break;
                }
            }
        }
    
        public void HideControllerObj(int x, int y)
        {
            for (int i = 0; i < ControllerObjList.Count; i++)
            {
                if (ControllerObjList[i].x == x && ControllerObjList[i].y == y)
                {
                    if (ControllerObjList[i].Obj != null)
                    {
                        ControllerObjList[i].Obj.transform.SetParent(ContainerObj.transform);
                        ControllerObjList[i].HideSelf();
                        ControllerObjUsedDic.Remove(new Vector2(x, y));
                    }
                    break;
                }
            }
        }
    
        public void ShowGuideNoticeObj(Transform tran, int x, int y)
        {
            for (int i = 0; i < GuideNoticeObjList.Count; i++)
            {
                if (!GuideNoticeObjList[i].IsUsed)
                {
                    GuideNoticeObjList[i].Obj.GetComponent<UITweener>().enabled = true;
                    GuideNoticeObjList[i].Obj.transform.SetParent(tran);
                    GuideNoticeObjList[i].Obj.transform.localPosition = LTInstanceConfig.GuideNoticePos;
                    GuideNoticeObjList[i].IsUsed = true;
                    GuideNoticeObjList[i].x = x;
                    GuideNoticeObjList[i].y = y;
                    break;
                }
            }
        }
    
        public void HideGuideNoticeObj(int x, int y)
        {
            for (int i = 0; i < GuideNoticeObjList.Count; i++)
            {
                if (GuideNoticeObjList[i].x == x && GuideNoticeObjList[i].y == y)
                {
                    GuideNoticeObjList[i].Obj.GetComponent<UITweener>().enabled = false;
                    GuideNoticeObjList[i].Obj.transform.SetParent(ContainerObj.transform);
                    GuideNoticeObjList[i].HideSelf();
                    break;
                }
            }
        }
    
        public void ShowHeroNoticeObj(Transform tran, int x, int y)
        {
            for (int i = 0; i < HeroNoticeObjList.Count; ++i)
            {
                if (!HeroNoticeObjList[i].IsUsed)
                {
                    HeroNoticeObjList[i].Obj.GetComponent<UITweener>().enabled = true;
                    HeroNoticeObjList[i].Obj.transform.SetParent(tran);
                    HeroNoticeObjList[i].Obj.transform.localPosition = LTInstanceConfig.HeroNoticePos;
                    HeroNoticeObjList[i].IsUsed = true;
                    HeroNoticeObjList[i].x = x;
                    HeroNoticeObjList[i].y = y;
                    break;
                }
            }
        }
    
        public void HideHeroNoticeObj(int x, int y)
        {
            for (int i = 0; i < HeroNoticeObjList.Count; ++i)
            {
                if (HeroNoticeObjList[i].IsUsed && HeroNoticeObjList[i].x == x && HeroNoticeObjList[i].y == y)
                {
                    HeroNoticeObjList[i].Obj.GetComponent<UITweener>().enabled = false;
                    HeroNoticeObjList[i].Obj.transform.SetParent(ContainerObj.transform);
                    HeroNoticeObjList[i].HideSelf();
                    break;
                }
            }
        }
    }
}
