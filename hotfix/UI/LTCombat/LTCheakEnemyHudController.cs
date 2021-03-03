using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTCheakEnemyHudController : UIControllerHotfix
    {
        public override bool ShowUIBlocker { get { return true; } }
        private int[] BGHeight = new int[3] { 460, 860, 960 };
        public UIWidget BG;
        public LTCheakEnemyScroll Scroll;
        public static bool m_Open;
        private List<int> enemyList;
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            BG = t.GetComponent<UIWidget>("BG");
            Scroll = t.GetMonoILRComponent<LTCheakEnemyScroll>("SlotsContainer/Placeholder/Grid");
            m_Open = false;
            controller.backButton = t.GetComponent<UIButton>("BG/Top/CloseButton");
        }
    
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            string enemyLayout = (string)param;
            enemyList = Hotfix_LT.Data.SceneTemplateManager.Instance.GetNewNewLayoutList(enemyLayout);
    
            if (enemyList.Count <= 6) BG.height = BGHeight[0];
            else if (enemyList.Count <= 12) BG.height = BGHeight[1];
            else  BG.height = BGHeight[2];
        }
        public override IEnumerator OnAddToStack()
        {
            m_Open = true;
            Scroll.SetItemDatas(enemyList);
            yield return null;
            yield return base.OnAddToStack();
        }
        public override IEnumerator OnRemoveFromStack()
        {
            m_Open = false;
            DestroySelf();
            yield break;
        }
    }
}
