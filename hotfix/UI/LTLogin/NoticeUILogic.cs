using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
	public class NoticeUILogic : UIControllerHotfix
    {
        private GameObject container
        {
            get { return controller.transform.Find("Container").gameObject; }
        }

        private NoticeTitleController titleGameObj;
        private GameObject contentGameObj;
        private UITable titleUiTable;
        private UIScrollView titleScrollView;
        private UIScrollView contentScrollView;

        private List<NoticeItem> Notices;

        private List<NoticeTitleController> _titleControllers = new List<NoticeTitleController>();
        private bool isFirst = true;
        private object mParam;
        private int TitleWidth = 80;
        public override void Awake()
        {
            base.Awake();
            // InitHastable();

            titleGameObj = controller.transform
                .Find("Container/Content/LeftBtn/Scroll View/Placeholder/Container/TitleBtn")
                .GetMonoILRComponent<NoticeTitleController>();
            contentGameObj = controller.transform.Find("Container/Content/Content Scroll View/Content").gameObject;
            titleUiTable = titleGameObj.mDMono.transform.parent.GetComponent<UITable>();
            titleScrollView = controller.transform.Find("Container/Content/LeftBtn/Scroll View")
                .GetComponent<UIScrollView>();
            contentScrollView = controller.transform.Find("Container/Content/Content Scroll View")
                .GetComponent<UIScrollView>();
            controller.transform.GetComponent<UIButton>("Container/Content/Title/CloseBtn").onClick.Add(new EventDelegate(OnCancelButtonClick));
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            mParam = param;
        }

        public override IEnumerator OnAddToStack()
        {
            if (isFirst)
            {
                Notices = (List<NoticeItem>) mParam;
                for (int i = 0; i < Notices.Count; i++)
                {
                    GameObject titleInstance =
                        GameObject.Instantiate(titleGameObj.mDMono.gameObject, titleUiTable.transform);
                    titleInstance.GetMonoILRComponent<NoticeTitleController>().SetText(Notices[i].type);
                    _titleControllers.Add(titleInstance.GetMonoILRComponent<NoticeTitleController>());
                }

                titleUiTable.Reposition();
                isFirst = false;
            }

            yield return base.OnAddToStack();
            //进入默认设置第一条公告
            if (Notices.Count > 0) SetTitleAndContent(0);
            titleScrollView.enabled = (Notices.Count > 5);
        }

        public override bool ShowUIBlocker
        {
            get { return true; }
        }

        public override void Show(bool isShowing)
        {
            container.SetActive(isShowing);
        }

        public override bool Visibility
        {
            get { return container.activeSelf; }
        }

        public void OnClickTitleBtn(GameObject btn)
        {
            int index = btn.transform.GetSiblingIndex();
            SetTitleAndContent(index - 1);
        }

        private void SetTitleAndContent(int index)
        {
            //设置标题
            for (int i = 0; i < _titleControllers.Count; i++)
            {
                if (index == i)
                {
                    _titleControllers[i].ClickTitleBtn();
                }
                else
                {
                    _titleControllers[i].SetSpriteStateByNum();
                }
            }

            //设置内容
            SetContent(index);
        }

		public override void OnBlur()
		{
			base.OnBlur();

			#region 强制回收GC
			System.GC.Collect(System.GC.MaxGeneration, System.GCCollectionMode.Forced);
			System.GC.WaitForPendingFinalizers();
			System.GC.Collect();
			#endregion
		}

		private void SetContent(int index)
        {
            LTUIUtil.SetText(contentGameObj.GetComponent<UILabel>(), Notices[index].notice);
            LTUIUtil.SetText(contentGameObj.transform.Find("ContentTitle/Label").GetComponent<UILabel>(),
                Notices[index].type);

            float panelHeight = contentScrollView.GetComponent<UIPanel>().GetViewSize().y;
            contentScrollView.ResetPosition();
            contentScrollView.enabled = (contentGameObj.GetComponent<UILabel>().height+TitleWidth) > panelHeight;
        }
    }
}