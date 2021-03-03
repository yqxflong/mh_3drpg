using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.Effect
{
    public class PraticleLevelHelper : MonoBehaviour
    {
        public void Awake()
        {
            //base.Awake();

            //var t = mDMono.transform;

            //levelObjs = new GameObject[4];
            //levelObjs[0] = t.FindEx("yugu").gameObject;
            //levelObjs[1] = t.FindEx("yugu (1)").gameObject;
            //levelObjs[2] = t.FindEx("yugu (2)").gameObject;
            //levelObjs[3] = t.FindEx("yugu (3)").gameObject;

            //isOnlyHideRender = true;
            //isOnlyShowLevel = false;
            //destroyWaitTime = 0f;
        }

        /// <summary> 0层级为消亡特效</summary>
        public GameObject[] levelObjs;
        /// <summary> 是否只隐藏渲染</summary>
        public bool isOnlyHideRender;
        /// <summary> 是否仅显示对应的层级</summary>
        public bool isOnlyShowLevel;
    
        public float destroyWaitTime;
    
        private System.Action _destroyDo;
        private Coroutine _cWaitShowZeroLevel;
    
        public void SetLevel(int level,System.Action destroyDo)
        {
            _destroyDo = destroyDo;
            for (int i=0;i< levelObjs.Length;i++)
            {
                if (levelObjs[i] == null)
                {
                    continue;
                }
                if (isOnlyHideRender)
                {
                    if((isOnlyShowLevel&& i== level)||(!isOnlyShowLevel &&i <= level))
                    {
                        Renderer[] renders = levelObjs[i].GetComponentsInChildren<Renderer>();
                        for (int j = 0; j < renders.Length; j++)
                        {
                            renders[j].enabled = true;
                        }
                    }
                    else
                    {
                        Renderer[] renders = levelObjs[i].GetComponentsInChildren<Renderer>();
                        for (int j = 0; j < renders.Length; j++)
                        {
                            renders[j].enabled = false;
                        }
                    }
                }
                else
                {
                    if ((isOnlyShowLevel && i == level) || (!isOnlyShowLevel && i <= level))
                    {
                        levelObjs[i].SetActive(true);
                    }
                    else
                    {
                        levelObjs[i].SetActive(false);
                    }
                }
            }
    
            if(level ==0)
            {
                if (_cWaitShowZeroLevel != null)
                {
                    StopCoroutine(_cWaitShowZeroLevel);
                }
    
                _cWaitShowZeroLevel = StartCoroutine(WaitShowZeroLevel());
            }
        }
    
        IEnumerator WaitShowZeroLevel()
        {
            yield return new WaitForSeconds(destroyWaitTime);
    
            if(_destroyDo !=null)
            {
                _destroyDo();
            }
        }
    }
}
