//LTInstanceUtils
//副本相关工具类
//Johny

using UnityEngine;

namespace Hotfix_LT.Instance
{
    public static class LTInstanceUtils
    {
        ///<Summary>
        ///获取格子坐标的Hash Value
        ///</Summary>
        public static int GridPosHash(Vector2Int pos)
        {
            return pos.y * 100 + pos.x;
        }

        ///<Summary>
        ///格子必须为左上角1,1的二象限坐标
        ///格子的中心点为上方菱形的中心点
        ///超出左上角点击坐标一律返回0,0
        ///<param=clickWolrdPos>点击的世界坐标
        ///<param=topLeftCellWorldPos>格子左上角的坐标
        ///<param=cellWidth>格子菱形的宽
        ///<param=cellHeight>格子菱形的高
        ///</Summary>
        public static Vector2Int ClickPos2GridPos(Vector2 clickWolrdPos, Vector2 topLeftCellWorldPos, float cellWidth, float cellHeight)
        {
            float halfW = cellWidth * 0.5f;
            Vector2 oriPos = new Vector2(topLeftCellWorldPos.x - halfW, topLeftCellWorldPos.y);
            Vector2 rePos = clickWolrdPos - oriPos;
            int M = Mathf.RoundToInt(rePos.x/cellWidth + rePos.y/cellHeight) - 1;
            int N = Mathf.RoundToInt(rePos.x/cellWidth - rePos.y/cellHeight);
            var ret = new Vector2Int(Mathf.Max(0, M), Mathf.Max(0, N));
            return ret;
        }

        ///点击坐标直接转成格子坐标的Hash值
        public static int ClickPos2GridPosHash(Vector2 clickWolrdPos, Vector2 topLeftCellWorldPos, float cellWidth, float cellHeight)
        {
            var gridPos = ClickPos2GridPos(clickWolrdPos, topLeftCellWorldPos, cellWidth, cellHeight);
            return GridPosHash(gridPos);
        }
    }
}

