using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTInstanceRoleBase : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();
            LimitOpen = mDMono.BoolParamList[0];
            RoleSprite = mDMono.transform.GetComponent<UISprite>("Role");
            Anis = mDMono.transform.GetComponentsInChildren<UITweener>(true);
        }

        public bool LimitOpen ;


        public UISprite RoleSprite;

        public UITweener[] Anis;

        protected string imgName;

        protected LTInstanceNode nodeData;

        public virtual void Init(LTInstanceNode nodeData)
        {
            imgName = GetImgName(nodeData.RoleData.Img);
            this.nodeData = nodeData;
            RoleSprite.spriteName = imgName;
            PlayAni();
        }

        public virtual void SetParam(object param)
        {

        }

        public virtual void PlayAni()
        {
            if (Anis == null) return;
            for (int i = 0; i < Anis.Length; i++)
            {
                Anis[i].ResetToBeginning();
                Anis[i].PlayForward();
            }
        }

        public virtual void SetOpenData()
        {
            if (!LimitOpen) RoleSprite.spriteName = string.Format("{0}1", imgName);
        }

        private string GetImgName(string strName)
        {
            string[] str = strName.Split('#');
            return str[str.Length - 1];
        }

        protected string GetParentPath(string path, Transform t)
        {
            if (t != null)
            {
                path = t.name + "/" + path;
                path = GetParentPath(path, t.parent);
            }
            return path;
        }
    }

    public class LTInstanceRoleEffect : LTInstanceRoleBase
    {
        public override void Awake()
        {
            base.Awake();
            Effect = mDMono.transform.GetComponent<ParticleSystemUIComponent>("Fx");
        }
        public ParticleSystemUIComponent Effect;

        public override void Init(LTInstanceNode nodeData)
        {
            base.Init(nodeData);
            Effect.gameObject.CustomSetActive(true);
        }

        public override void SetParam(object param)
        {
            Effect.panel = param as UIPanel;
        }
    }

    public class LTInstanceRoleEffect1 : LTInstanceRoleBase
    {
        public override void Awake()
        {
            base.Awake();
            Effect = mDMono.transform.GetComponent<ParticleSystemUIComponent>("Fx");
        }

        public ParticleSystemUIComponent Effect;

        public override void Init(LTInstanceNode nodeData)
        {
            base.Init(nodeData);
            Effect.gameObject.CustomSetActive(false);
            if (mDMono .gameObject.activeInHierarchy)
            {
                Transform go = mDMono.transform.parent.parent;
                if (go != null)
                {
                    GuideNodeManager.GateString = GetParentPath(go.name, go.parent);
                }
            }
        }

        public override void SetParam(object param)
        {
            Effect.panel = param as UIPanel;
        }

        public override void SetOpenData()
        {
            if (LTInstanceConfig.RoleEffectNameDic.ContainsKey(imgName))
            {
                ParticleSystem par = LTInstanceEffect.Instance.GetParticleByEffectName(LTInstanceConfig.RoleEffectNameDic[imgName]);
                if (par != null)
                {
                    Effect.fx = par;
                    Effect.gameObject.CustomSetActive(true);
                }
            }
        }
    }

    public class LTInstanceRoleEffect2 : LTInstanceRoleBase
    {
        public override void Awake()
        {
            base.Awake();
            Effect = mDMono.transform.GetComponent<ParticleSystemUIComponent>("Fx");
        }
        public ParticleSystemUIComponent Effect;

        public override void Init(LTInstanceNode nodeData)
        {
            base.Init(nodeData);
            if (LTInstanceConfig.RoleEffectNameDic.ContainsKey(imgName))
            {
                ParticleSystem par = LTInstanceEffect.Instance.GetParticleByEffectName(LTInstanceConfig.RoleEffectNameDic[imgName]);
                if (par != null)
                {
                    Effect.fx = par;
                    Effect.gameObject.CustomSetActive(true);
                }
            }
            if (mDMono.gameObject.activeInHierarchy)
            {
                Transform go = mDMono.transform.parent.parent;
                if (go != null)
                {
                    GuideNodeManager.GateString = GetParentPath(go.name, go.parent);
                }
            }
        }

        public override void SetParam(object param)
        {
            Effect.panel = param as UIPanel;
        }

        public override void SetOpenData()
        {
            base.SetOpenData();
            Effect.gameObject.CustomSetActive(false);
        }
    }
    
    public class LTInstanceRoleShake : LTInstanceRoleBase
    {
        public override void SetOpenData()
        {
            base.SetOpenData();
            mDMono.gameObject.CustomSetActive(false);
        }
    }

    public class LTInstanceRoleSkill : LTInstanceRoleBase
    {
        public override void Awake()
        {
            base.Awake();
            skillSprite = mDMono.transform.GetComponent<DynamicUISprite>("Role/SkillImg");
        }

        public DynamicUISprite skillSprite;

        private int skillId;

        public override void Init(LTInstanceNode nodeData)
        {
            base.Init(nodeData);
            skillId = nodeData.RoleData.CampaignData.SkillId;

            Hotfix_LT.Data.SkillTemplate skillTpl = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(skillId);
            if (skillTpl != null)
            {
                skillSprite.spriteName = skillTpl.Icon;
            }
        }
    }
}
