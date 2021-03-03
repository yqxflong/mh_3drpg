using UnityEngine;

namespace Hotfix_LT.UI
{
    public class UnityHelper
    {
        public static Transform FindTheChildNode(GameObject goParent, string childName)
        {
            Transform searchTrans = goParent.transform.Find(childName);

            if (searchTrans == null)
            {
                for (var i = 0; i < goParent.transform.childCount; i++)
                {
                    searchTrans = FindTheChildNode(goParent.transform.GetChild(i).gameObject, childName);

                    if (searchTrans != null)
                    {
                        return searchTrans;
                    }
                }
            }

            return searchTrans;
        }

        public static T GetTheChildNodeComponetScripts<T>(GameObject goParent, string childName) where T : Component
        {
            Transform searchTranformNode = FindTheChildNode(goParent, childName);

            if (searchTranformNode != null)
            {
                return searchTranformNode.gameObject.GetComponent<T>();
            }
            else
            {
                return null;
            }
        }

        public static T AddChildNodeCompnent<T>(GameObject goParent, string childName) where T : Component
        {
            Transform searchTranform = FindTheChildNode(goParent, childName);

            //如果查找成功，则考虑如果已经有相同的脚本了，则先删除，否则直接添加。
            if (searchTranform != null)
            {
                //如果已经有相同的脚本了，则先删除
                T[] componentScriptsArray = searchTranform.GetComponents<T>();

                for (int i = 0; i < componentScriptsArray.Length; i++)
                {
                    if (componentScriptsArray[i] != null)
                    {
                        Object.Destroy(componentScriptsArray[i]);
                    }
                }

                return searchTranform.gameObject.AddComponent<T>();
            }
            else
            {
                return null;
            }
            //如果查找不成功，返回Null.
        }

        public static void AddChildNodeToParentNode(Transform parents, Transform child)
        {
            child.SetParent(parents, false);
            child.localPosition = Vector3.zero;
            child.localScale = Vector3.one;
            child.localEulerAngles = Vector3.zero;
        }
    }
}