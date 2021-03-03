using UnityEngine;

namespace Hotfix_LT.UI
{
    public class FunctionOpenUnit : DynamicMonoHotfix
    {
        public int FunctionID;
        public UILabel FuncName;
        public UISprite FuncIcon;
        public bool ActionInAwake = false;

        public override void Awake()
        {
            base.Awake();

            if (mDMono.IntParamList != null)
            {
                var count = mDMono.IntParamList.Count;

                if (count > 0)
                {
                    FunctionID = mDMono.IntParamList[0];
                }
            }

            if (mDMono.ObjectParamList != null)
            {
                var count = mDMono.ObjectParamList.Count;

                if (count > 0 && mDMono.ObjectParamList[0] != null)
                {
                    FuncName = ((GameObject)mDMono.ObjectParamList[0]).GetComponentEx<UILabel>();
                }
                if (count > 1 && mDMono.ObjectParamList[1] != null)
                {
                    FuncIcon= ((GameObject)mDMono.ObjectParamList[1]).GetComponentEx<UISprite>();
                }
            }

            if (mDMono.BoolParamList != null)
            {
                var count = mDMono.BoolParamList.Count;

                if (count > 0)
                {
                    ActionInAwake = mDMono.BoolParamList[0];
                }
            }

            if (ActionInAwake)
            {
                Init();
            }
        }
    
        public void Init()
        {
            var func = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(FunctionID);
            mDMono.gameObject.SetActive(func.IsConditionOK());

            if (FuncName != null)
            {
                FuncName.text = func.display_name;
            }

            if (FuncIcon != null)
            {
                FuncIcon.spriteName = func.iconName;
            }
        }
        
        public void OnFuncOpenBtnClick()
        {
            var func = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(FunctionID);

            if (func.ui_model != null)
            {
                GlobalMenuManager.Instance.Open(func.ui_model);
            }
        }
    }
}
