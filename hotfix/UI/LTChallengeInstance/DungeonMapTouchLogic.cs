using UnityEngine;

namespace Hotfix_LT.UI
{
    public class DungeonMapTouchLogic : DynamicMonoHotfix
    {
        public Transform MapTran;
        public float MoveSpeed = 1;
        public Vector3 lastMapPos = Vector3.zero;
        public bool NeedLimit = true;
        public Vector2 LimitMaxPos = new Vector3(1365, 768);

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            MapTran = t.parent.GetComponent<Transform>("Cull/RowContainer");
            MoveSpeed = 1f;
            NeedLimit = false;

            t.GetComponentEx<UIEventTrigger>().onDrag.Add(new EventDelegate(mDMono, "OnDrag"));
        }

        public override void OnDrag(Vector2 delta)
        {
            MoveMap(delta);
        }
        
        private void MoveMap(Vector2 delta)
        {
            Vector3 mapTargetPos = lastMapPos + new Vector3(delta.x, delta.y, 0) * MoveSpeed;
            Move(mapTargetPos);
        }
    
        private void Move(Vector2 target)
        {
            float x = target.x;
    
            float y = target.y;
    
            if (NeedLimit)
            {
                x = Mathf.Clamp(target.x, -LimitMaxPos.x, LimitMaxPos.x);
                y = Mathf.Clamp(target.y, -LimitMaxPos.y, LimitMaxPos.y);
            }
    
            if (MapTran != null)
            {
                MapTran.localPosition = new Vector3(x, y, MapTran.localPosition.z);
                lastMapPos = MapTran.localPosition;
            }
        }
    
        public void MoveToNode(Vector2 nodePos)
        {
            Vector2 mapTargetPos = Vector2.zero - nodePos;//使用节点相对屏幕中心的坐标减去地图相对屏幕中心的坐标，就能得到地图需要移动多少使得节点位于正中央
            Move(mapTargetPos);
        }
    }
}
