using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class FunctionOpenController : UIControllerHotfix
    {
        public override bool ShowUIBlocker { get { return false; } }
        public override bool CanAutoBackstack() { return false; }
    
        private static FunctionOpenController s_Instance;
        public static FunctionOpenController Instance
        {
            get { return s_Instance; }
        }

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            TopTitleLabel = t.GetComponent<UILabel>("FuncOpenView/OpenTitleLabel");
            FuncSprite = t.GetComponent<UISprite>("FuncOpenView/FuncIcon");
            FuncLabel = t.GetComponent<UILabel>("FuncOpenView/FuncIcon/FuncName");
            FxObj = t.FindEx("Fx").gameObject;

            TweenPos = new Transform[2];
            TweenPos[0] = t.GetComponent<Transform>("Pos/BottomRight/T0");
            TweenPos[1] = t.GetComponent<Transform>("Pos/TopRight/T1");

            t.GetComponent<UIButton>("Bg").onClick.Add(new EventDelegate(OncContinueClick));

            s_Instance = this;
            t.gameObject.CustomSetActive(false);
            Hide();
        }
    
        public override void OnFocus()
        {
            string focusName = CommonConditionParse.FocusViewName; //为了不干扰焦点
            base.OnFocus();
            CommonConditionParse.FocusViewName = focusName;
            EB.Debug.Log("FocusViewName <color=#800000ff>{0}</color>",CommonConditionParse.FocusViewName);
        }
    
        public UILabel TopTitleLabel;
        public UISprite FuncSprite;
        public UILabel FuncLabel;
        public GameObject FxObj;
        public Transform[] TweenPos;//目前写死，且只有一个
    
        private System.Action CallBack = null;
        private int posIndex = 0;//征战，活动
        private bool btnClick = false;
    
        public void ResetIndex()
        {
            posIndex = 0;
        }
    
        public void Show(int FuncId,int Pos ,System.Action callBack = null)
        {
            posIndex = Pos;
            Show(FuncId, callBack);
        }
    
        public void Show(int FuncId,System.Action callBack=null)
        {
            var Func= Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(FuncId);
            FuncSprite.spriteName = Func.iconName;
            FuncLabel.text = Func.display_name;
            CallBack = callBack;
            controller.Open();
            controller.GetComponent<TweenAlpha>().ResetToBeginning();
            controller.gameObject.CustomSetActive(true);
            controller.GetComponent<TweenAlpha>().PlayForward();
            StartCoroutine(Play());
        }
    
        IEnumerator Play()
        {
            UITweener[] ts = TopTitleLabel.transform.GetComponents<UITweener>();

            if (ts != null)
            {
                for (var i = 0; i < ts.Length; i++)
                {
                    ts[0].PlayForward();
                }
            }

            FxObj.CustomSetActive(true);
    
            FusionAudio.PostEvent("UI/New/JieSuo",true);
            yield return new WaitForSeconds(ts[0].duration + ts[0].delay);
    
            TweenScale TS =FuncSprite.GetComponent<TweenScale>();
            //TS.ResetToBeginning();
            TS.PlayForward();
    
            yield return new WaitForSeconds(TS.duration + TS.delay);
            btnClick = true;
            yield break;
        }
    
        public void OncContinueClick()
        {
            if (!btnClick) return;
            btnClick = false;
            StartCoroutine(ContinuePlay());
        }
    
        IEnumerator ContinuePlay()
        {
            UITweener[] ts = TopTitleLabel.transform.GetComponents<UITweener>();

            if (ts != null)
            {
                for(var i = 0; i < ts.Length; i++)
                {
                    ts[i].PlayReverse();
                }
            }

            TweenPosition TP = FuncSprite.GetComponent<TweenPosition>();
            TP.to = TweenPos[posIndex].parent.localPosition + TweenPos[posIndex].localPosition;//静态坐标
            TP.ResetToBeginning();
            TP.PlayForward();
    
            FxObj.CustomSetActive(false);
            yield return new WaitForSeconds(TP.duration + TP.delay);
            TweenScale TS = FuncSprite.GetComponent<TweenScale>();
            TS.PlayReverse();
            yield return new WaitForSeconds(TS.duration + TS.delay);
            Hide();
            yield break;
        }
    
        public void Hide()
        {
            controller.Close();
            StopAllCoroutines();
            FuncSprite.transform.localPosition = FuncSprite.transform.localScale = TopTitleLabel.transform.localScale = Vector3.zero;
            controller.gameObject.CustomSetActive(false);
            if (CallBack != null) CallBack();
            CallBack = null;
            btnClick = false;
        }
    }
}
