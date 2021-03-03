using UnityEngine;
using System.Collections;
using Pathfinding.RVO;
public class ObstacleHelper : MonoBehaviour {
    public GameObject m_FxOb;
    private static ObstacleHelper m_Instance;
    public static ObstacleHelper Instance
    {
        get
        {
            if(m_Instance==null)
            {
                EB.Debug.LogError("ObstacleHelper is not awake now");
            }
            return m_Instance;
        }
    }

    void Awake()
    {
        m_Instance = this;
    }

    void OnDestroy()
    {
        m_Instance = null;
    }

    public void InitObstacle(string obs)
    {
        if (string.IsNullOrEmpty(obs)) return;
        RVOSquareObstacle[] childs = gameObject.transform.GetComponentsInChildren<RVOSquareObstacle>(true);
        if (childs == null) return;
        string[] splits = obs.Split(',');
        if (splits == null) return;
        for(int j=0;j<splits.Length;j++)
        {
            for (int i = 0; i < childs.Length; i++)
            {
            
                if (childs[i].name.Equals(splits[j]))
                {
                    Transform anchor = childs[i].transform.GetChild(0);
                    if (anchor == null) continue;
                    SpawnVFX(m_FxOb, anchor, GetFxLength(childs[i].gameObject));
                    childs[i].gameObject.SetActive(true);
                    break;
                }
            }
        }
    }

    public float GetFxLength(GameObject obstacle)
    {
        float length = 1f;
        if (obstacle == null) return length;
        SceneObstacleFxLength sofl= obstacle.GetComponent<SceneObstacleFxLength>();
        if (sofl == null) return length;
        return sofl.m_Length;
    }

    public void ShowObstacle(string obs)
    {
        if (string.IsNullOrEmpty(obs)) return;
        if (gameObject == null) return;
        RVOSquareObstacle [] childs = gameObject.transform.GetComponentsInChildren<RVOSquareObstacle>(true);
        if (childs == null) return;
        for (int i = 0; i < childs.Length; i++)
        {
            if (obs.Contains(childs[i].name))
            {
                Transform anchor = childs[i].transform.GetChild(0);
                if (anchor == null) continue;

                SpawnVFX(m_FxOb, anchor, GetFxLength(childs[i].gameObject));
                //InitObstacle("EmptyObstacle", childs[i].transform);
                childs[i].gameObject.SetActive(true);
            }
        }

    }

    public void DispearObstacle()
    {
        RVOSquareObstacle[] childs = gameObject.transform.GetComponentsInChildren<RVOSquareObstacle>(true);
        if (childs == null) return;
        for (int i = 0; i < childs.Length; i++)
        {
            Transform anchor = childs[i].transform.GetChild(0);
            if (anchor == null) continue;
            anchor.DestroyChildren();
            childs[i].gameObject.SetActive(false);
        }
    }

    public static GameObject SpawnVFX(GameObject _vfxObjPrefab, Transform parentTransform,float scale)
    {
        if (_vfxObjPrefab == null)
        {
            EB.Debug.LogWarning("[UIBindableItemDataLookupDragVFX]SpawnVFX: Can't spawn the given VFX, cause it's not defined!");
            return null;
        }

        GameObject vfxObj = GameObject.Instantiate(_vfxObjPrefab, parentTransform.position, Quaternion.identity) as GameObject;

        if (vfxObj != null)
        {
            vfxObj.transform.parent = parentTransform;
            vfxObj.transform.localScale = new Vector3(1f, 1f, 1f);
            vfxObj.transform.localPosition = new Vector3(0f, 0f, 0f);
            vfxObj.transform.localRotation = Quaternion.identity;

            Transform [] transforms=vfxObj.GetComponentsInChildren<Transform>();
            if (transforms == null) return vfxObj;
            for(int i=0;i<transforms.Length;i++)
            {
                Vector3 localscale = transforms[i].localScale;
                localscale.x = scale;
                transforms[i].localScale= localscale;
            }
        }
        return vfxObj;
    }
}
