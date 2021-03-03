using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTLineupPresetController : UIControllerHotfix
    {
        public override bool ShowUIBlocker { get { return true; } }
        public override bool IsFullscreen() { return false; }

        private UIGrid _uiGrid;
        private GameObject _lineupGo;
        private List<LTLineupPresetInfo> _lineupPresetInfos;
        private bool _isInit = false;

        public override void Awake()
        {
            if (_isInit)
            {
                return;
            }

            base.Awake();

            var t = controller.transform;
            controller.backButton = t.GetComponent<UIButton>("BG/Top/CloseBtn");
            _uiGrid = t.GetComponent<UIGrid>("Scroll View/Grid");
            _lineupPresetInfos = new List<LTLineupPresetInfo>();

            _isInit = true;
        }

        private void InitPool()
        {
            var tGrid = _uiGrid.transform;
            var childCount = tGrid.childCount;
            _lineupPresetInfos.Clear();

            for (var i = 0; i < childCount; i++)
            {
                var child = tGrid.GetChild(i);
                child.gameObject.SetActive(false);

                if (i == 0)
                {
                    _lineupGo = child.gameObject;
                }
                else
                {
                    _lineupPresetInfos.Add(child.GetMonoILRComponent<LTLineupPresetInfo>());
                }
            }
        }

        private void InitData(bool showSavePanel, string lineupType, int battleType, System.Action callback)
        {
            ArrayList data;
            DataLookupsCache.Instance.SearchDataByID("lineup_preset", out data);

            for (var i = 0; i < data.Count; i++)
            {
                var type = EB.Dot.String("lineup_type", data[i], "");

                if (type.Equals(lineupType))
                {
                    var infos = EB.Dot.Array("lineup_infos", data[i], new ArrayList());

                    for (var j = 0; j < infos.Count; j++)
                    {
                        int[] ids;

                        if (infos[j] is int[])
                        {
                            ids = infos[j] as int[];
                        }
                        else
                        {
                            var arrayList = EB.Dot.Array(string.Format("lineup_infos[{0}]", j), data[i], new ArrayList());
                            ids = new int[arrayList.Count];

                            for (var k = 0; k < ids.Length; k++)
                            {
                                ids[k] = System.Convert.ToInt32(arrayList[k]);
                            }
                        }

                        LTLineupPresetInfo lineupPresetInfo;
                        
                        // 优先使用已存在的缓存对象，没有再创建新的
                        if (_lineupPresetInfos.Count > 0)
                        {
                            lineupPresetInfo = _lineupPresetInfos[0];
                            _lineupPresetInfos.RemoveAt(0);
                        }
                        else
                        {
                            lineupPresetInfo = Object.Instantiate(_lineupGo, _uiGrid.transform).GetMonoILRComponent<LTLineupPresetInfo>();
                        }
                        
                        lineupPresetInfo.mDMono.gameObject.SetActive(true);
                        lineupPresetInfo.SetData(j, ids, lineupType, showSavePanel, battleType, callback);
                    }
                    break;
                }
            }
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            
            Awake();
            InitPool();

            var ht = param as Hashtable;
            var showSavePanel = (bool)ht["ShowSavePanel"];
            var lineupType = ht["LineupType"].ToString();
            var callback = (System.Action)ht["Callback"];
            var battleType = (int)ht["BattleType"];
            InitData(showSavePanel, lineupType, battleType, callback);
        }

        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            yield return null;
        }

        public override IEnumerator OnRemoveFromStack()
        {
            yield return base.OnRemoveFromStack();
            DestroySelf();
            yield break;
        }
    }
}
