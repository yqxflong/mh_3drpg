using Hotfix_LT.Data;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class FunctionOpenBtnControl : DataLookupHotfix
    {
        public int FunCId = 10077;

        private FuncTemplate funTemplate;
        private UILabel NameLabel;
        private UILabel TipLabel;

        public override void Awake()
        {
            base.Awake();

            var t = mDL.transform;
            NameLabel = t.GetComponent<UILabel>("Label");
            TipLabel = t.GetComponent<UILabel>("Label (1)");
            funTemplate = FuncTemplateManager.Instance.GetFunc(FunCId);

            if (funTemplate != null && !funTemplate.IsConditionOK())
            {
                if (!mDL.DataIDList.Contains("level"))
                {
                    mDL.DataIDList.Add("level");
                }

                SetBtnCondition();
            }

        }
        public override void OnLookupUpdate(string dataID, object value)
        {
            base.OnLookupUpdate(dataID, value);
            SetBtnCondition();
        }
        private void SetBtnCondition()
        {
            if (funTemplate.IsConditionOK())
            {
                NameLabel.transform.localPosition = new Vector3(0, 19, 0);
                TipLabel.gameObject.CustomSetActive(false);
                mDL.transform.GetComponent<BoxCollider>().enabled = true;
                mDL.transform.GetComponent<UISprite>().color = new Color(1, 1, 1, 1);
            }
            else
            {
                NameLabel.transform.localPosition = new Vector3(0, 38, 0);
                TipLabel.gameObject.CustomSetActive(true);
                TipLabel.text = TipLabel.transform.GetChild(0).GetComponent<UILabel>().text = funTemplate.GetConditionStr();
                mDL.transform.GetComponent<BoxCollider>().enabled = false;
                mDL.transform.GetComponent<UISprite>().color = new Color(1, 0, 1, 1);
            }
        }
    }
}