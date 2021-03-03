using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    //from: https://blog.uwa4d.com/archives/USparkle_RedPoint.html

    public class RedPointNode
    {
        public string name;
        public int num = 0;
        public LTRedPointSystem.OnNumChange numChangeFunc;

        public RedPointNode parent = null;
        public Dictionary<string, RedPointNode> childs = new Dictionary<string, RedPointNode>();

        public void SetNum(int num)
        {
            if (childs.Count > 0)
            {
                EB.Debug.LogError("RedPointNode SetNum Error!No Leaf Node.name = {0}", name);
                return;
            }
            if (this.num != num)
            {
                this.num = num;
                NotifyNumChange();
                if (parent != null)
                {
                    parent.RefreshNum();
                }
            }
        }

        private void RefreshNum()
        {
            int newNum = 0;
            var enumerator = childs.GetEnumerator();
            while (enumerator.MoveNext())
            {
                newNum+=enumerator.Current.Value.num;
            }
            if (newNum != num)
            {
                num = newNum;
                NotifyNumChange();
                if (parent != null)//递归设置所有父节点，不然只有三级节点时,根节点不刷新
                {
                    parent.RefreshNum();
                }
            }            
        }

        private void NotifyNumChange()
        {
            numChangeFunc?.Invoke(this);
        }
    }
}
