using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTInstanceSmallMapHudController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            RowObj = t.FindEx("TemplatePrefab/LTInstanceMapRowObj").gameObject;
            RowObjContainer = t.FindEx("Cull/RowContainer").gameObject;
            TouchLogic = t.GetMonoILRComponent<DungeonMapTouchLogic>("BG");
            controller.backButton = t.GetComponent<UIButton>("BG/CancelBtn");

            rowCtrlDic = new Dictionary<int, LTInstanceRowCtrl>();
        }
        
        private Dictionary<int, LTInstanceRowCtrl> rowCtrlDic;
    
        public GameObject RowObj;
    
        public GameObject RowObjContainer;
    
        public DungeonMapTouchLogic TouchLogic;
    
        private LTInstanceNode mCurNode;
    
        public override bool IsFullscreen()
        {
            return false;
        }
    
        public override bool ShowUIBlocker
        {
            get
            {
                return true;
            }
        }
    
        public override void SetMenuData(object param)
        {
            LTInstanceNode data = param as LTInstanceNode;
            if (data != null)
            {
                mCurNode = data;
            }
        }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            InitMap();
            InitPos();
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            StopAllCoroutines();
            rowCtrlDic.Clear();
            for (int i= RowObjContainer.transform.childCount-1;i>=0;i--)
            {
                GameObject.Destroy(RowObjContainer.transform.GetChild(i).gameObject);
            }
            DestroySelf();
            yield break;
        }
    
        private void InitMap()
        {
            Dictionary<int, LTInstanceNode> dataDic = LTInstanceMapModel.Instance.NodeDataHashDic;
            if (dataDic == null)
            {
                return;
            }
            List<LTInstanceNode> dataList = new List<LTInstanceNode>();
    
            foreach(var it in dataDic)
            {
                dataList.Add(it.Value);
            }
    
            if (dataList == null || dataList.Count < 0)
            {
                return;
            }

            for (var i = 0; i < dataList.Count; ++i)
            {
                var node = dataList[i];
                if (!node.CanPass) continue;
                LTInstanceRowCtrl row = null;
                if (rowCtrlDic.TryGetValue(node.y, out row))
                {
                    LTInstanceNodeTemp cell = null;
                    if (row.itemObjDic.TryGetValue(node.x, out cell))
                    {
                        cell.UpdateData(node);
                    }
                    else
                    {
                        row.CreateNodeFromCache(node);
                    }
                }
                else
                {
                    CreateRow(node);
                }
            }
        }
    
        private void InitPos()
        {
            if (TouchLogic != null&& mCurNode!=null)
            {
                Vector3 endPos = new Vector3((mCurNode.x + mCurNode.y) * LTInstanceConfig.SMALL_MAP_SCALE_X, (mCurNode.x-mCurNode.y) * LTInstanceConfig.SMALL_MAP_SCALE_Y, 0);// new Vector3(mCurNode.x * LTInstanceConfig.SMALL_MAP_SCALE, -mCurNode.y * LTInstanceConfig.SMALL_MAP_SCALE, 0);
                TouchLogic.MoveToNode(endPos);
            }
        }
    
        private void CreateRow(LTInstanceNode node)
        {
            GameObject row = GameObject.Instantiate(RowObj);
            row.CustomSetActive(true);
            row.transform.SetParent(RowObjContainer.transform);
            row.transform.localPosition = new Vector3(node.y * LTInstanceConfig.SMALL_MAP_SCALE_X, -node.y * LTInstanceConfig.SMALL_MAP_SCALE_Y);// new Vector3(0, -node.y, 0) * LTInstanceConfig.SMALL_MAP_SCALE;
            row.transform.localScale = Vector3.one;
            row.name = node.y.ToString();
            LTInstanceRowCtrl ctrl = row.GetMonoILRComponent<LTInstanceRowCtrl>();
            ctrl.CreateNodeFromCache(node);
            rowCtrlDic.Add(node.y, ctrl);
        }
    }
}
