using System.Collections;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class UIFuncConditionTabAdapt:UIConditionTabAdapt
    {
        public int mFuncId=0;
        private int needlevel = 0;

        public override void Awake()
        {
            base.Awake();
            if (mDMono.IntParamList.Count>0)
            {
                mFuncId = mDMono.IntParamList[0];
            }
        }

        public override bool IsConditionOk()
        {
            if(mFuncId<=0)
            {
                return true;
            }
            else
            {
                var func=FuncTemplateManager.Instance.GetFunc(mFuncId);  
                if(func==null)
                {
                	EB.Debug.LogWarning("Func not found ",mFuncId);
                	return true;
                }
                if (func.IsConditionOK())
                {
                	return true;
                }
                else
                {
                	return false;
                }
            }
        }

        public override bool ShowConditionMessage()
        {
            if(!IsConditionOk())
            {
                var func = FuncTemplateManager.Instance.GetFunc(mFuncId);
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, func.GetConditionStrSpecial());
                return false;
            }
            return true;
        }
    }
}