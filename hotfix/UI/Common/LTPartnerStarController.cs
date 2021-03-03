using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTPartnerStarController : DynamicMonoHotfix
    {
        public GameObject[] StarObjList;

        private float SixStarValue = 25;
        private float FiveStarValue = 30;
        private float OtherStarValue = 35;
        private float posOffset = 12;

        private Vector3 vecHigh = Vector3.zero;
        private Vector3 vecLow = Vector3.zero;
        private UIGrid mUIGrid;

        public override void Awake()
        {
            var t = mDMono.transform;
            var childCount = t.childCount;
            StarObjList = new GameObject[childCount];

            for (int i = 0; i < childCount; i++)
            {
                StarObjList[i] = t.GetChild(i).gameObject;
            }
            
            if (mDMono.FloatParamList != null)
            {
                var count = mDMono.FloatParamList.Count;

                if (count > 0)
                {
                    SixStarValue = mDMono.FloatParamList[0];
                }
                if (count > 1)
                {
                    FiveStarValue = mDMono.FloatParamList[1];
                }
                if (count > 2)
                {
                    OtherStarValue = mDMono.FloatParamList[2];
                }
                if (count > 3)
                {
                    posOffset = mDMono.FloatParamList[3];
                }
            }
        }

        public void SetSrarList(int starNum, int awakenLevel)
        {
            if (vecHigh == Vector3.zero || vecLow == Vector3.zero)
            {
                vecHigh = mDMono.transform.localPosition;
                vecLow = new Vector3(vecHigh.x - posOffset, vecHigh.y, vecHigh.z);
            }

            if (mUIGrid == null)
            {
                mUIGrid = mDMono.gameObject.GetComponent<UIGrid>();
            }

            switch (starNum)
            {
                case 6:
                    mUIGrid.cellWidth = SixStarValue;
                    mUIGrid.transform.localPosition = vecHigh;
                    break;
                case 5:
                    mUIGrid.cellWidth = FiveStarValue;
                    mUIGrid.transform.localPosition = vecHigh;
                    break;
                default:
                    mUIGrid.cellWidth = OtherStarValue;
                    mUIGrid.transform.localPosition = vecLow;
                    break;
            }
            for (int i = 0; i < StarObjList.Length; i++)
            {
                if (i < starNum)
                {
                    StarObjList[i].SetActive(true);
                    SetNormal(StarObjList[i], awakenLevel);
                }
                else
                {
                    StarObjList[i].SetActive(false);
                }
            }
            mUIGrid.repositionNow = true;
        }

        public void SetStarAlpha(int starNum, int awakenLevel)
        {
            for (int i = 0; i < StarObjList.Length; i++)
            {
                if (i < starNum)
                {
                    SetNormal(StarObjList[i], awakenLevel);

                }
                else
                {
                    SetGray(StarObjList[i]);
                }
            }
        }

        private void SetNormal(GameObject obj, int awakenLevel)
        {
            obj.GetComponent<UISprite>().spriteName = LTPartnerConfig.PARTNER_AWAKN_STAR_DIC[awakenLevel];
            obj.GetComponent<UISprite>().color = new Color(1, 1, 1, 1);
        }

        private void SetGray(GameObject obj)
        {
            obj.GetComponent<UISprite>().color = new Color(0, 0, 0, 0.5f);
        }
    }
}
