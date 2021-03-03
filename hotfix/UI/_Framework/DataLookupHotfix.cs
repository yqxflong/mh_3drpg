using UnityEngine;
using EB.Sparx;
using System.Collections.Generic;
using System.Collections;

namespace Hotfix_LT.UI
{
    public class DataLookupHotfix : DataLookILRObject
    {

        public DataLookupILR mDL;

        public override void SetDataLookup(DataLookupILR dataLookup)
        {
            mDL = dataLookup;
        }

        static public GameObject InstantiateEx(GameObject target, Transform parent, string name = "Clone")
        {
            GameObject go = GameObject.Instantiate(target);
            go.name = name;
            go.transform.SetParent(parent, false);
            if (false == go.activeSelf) go.gameObject.SetActive(true);
            return go;
        }
        static public T InstantiateEx<T>(T target, Transform parent, string name = "Clone") where T : DynamicMonoHotfix
        {
            GameObject obj = target.mDMono.gameObject;
            GameObject go = InstantiateEx(obj, parent, name);
            return go.GetComponent<DynamicMonoILR>()._ilrObject as T;
        }

        static public T InstantiateExOrig<T>(T target, Transform parent, string name = "Clone") where T : MonoBehaviour
        {
            GameObject obj = target.gameObject;
            GameObject go = InstantiateEx(obj, parent, name);
            return go.GetComponent<T>();
        }

        #region Coroutine
        protected List<Coroutine> coroutines = new List<Coroutine>();

        protected void AddToCoroutines(Coroutine c)
        {
            if (coroutines.Contains(c))
            {
                return;
            }

            coroutines.Add(c);
        }

        protected Coroutine StartCoroutine(IEnumerator routine)
        {
            var c = EB.Coroutines.Run(routine);
            AddToCoroutines(c);
            return c;
        }

        protected void StopCoroutine(Coroutine c)
        {
            EB.Coroutines.Stop(c);

            if (coroutines.Contains(c))
            {
                coroutines.Remove(c);
            }
        }

        protected void StopCoroutine(IEnumerator c)
        {
            EB.Coroutines.Stop(c);
        }

        protected void StopAllCoroutines()
        {
            for (var i = 0; i < coroutines.Count; i++)
            {
                var c = coroutines[i];
                EB.Coroutines.Stop(c);
            }
            coroutines.Clear();
        }
        #endregion

        #region UISeverRequest Call
        public override void OnFetchData(EB.Sparx.Response res, int reqInstanceID)
        {
        }
        #endregion
    }
}