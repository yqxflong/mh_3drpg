using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SetSortingOrder : MonoBehaviour
{
    public int SortingOrder;
    public bool isParticle = true;

    public int OriginSortingOrder
    {
        get { return originSortingOrder == int.MinValue ? GetLayer() : originSortingOrder; }
    }

    [SerializeField]
    private int originSortingOrder = int.MinValue;
    private int currentSortingOrder;

    void Awake()
    {
        if (originSortingOrder == int.MinValue)
        {
            originSortingOrder = GetLayer();
        }
    }

    void Start()
    {
        currentSortingOrder = SortingOrder;
        SetLayer();
    }

    void Update()
    {
        if (SortingOrder != currentSortingOrder)
        {
            currentSortingOrder = SortingOrder;
            SetLayer();            
        }
    }

    int GetLayer()
    {
        if (isParticle)
        {
            var ps = GetComponentInChildren<ParticleSystem>();
            if (ps != null)
            {
                Renderer rs = ps.GetComponent<Renderer>();
                return rs.sortingOrder;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                return renderer.sortingOrder;
            }
            else
            {
                return 0;
            }
        }
    }

    void SetLayer()
    {
        if (isParticle)
        {
            ParticleSystem[] sys = GetComponentsInChildren<ParticleSystem>(true);

            for (int i = 0; i < sys.Length; i++)
            {
                if (sys[i] != null && sys[i].GetComponent<Renderer>() != null)
                {
                    Renderer[] rens = sys[i].GetComponentsInChildren<Renderer>(true);
                    for (int j = 0; j < rens.Length; j++)//此处是为了防止粒子特效嵌套mesh by hzh
                    {
                        rens[j].sortingOrder = SortingOrder;
                    }
                }
            }
            
        }
        else
        {
            Renderer ren = GetComponent<Renderer>();

            if (ren != null)
            {
                ren.sortingOrder = SortingOrder;
            }
        }
    }

    public void SetLayer(int sortingOrder)
    {
        SortingOrder = sortingOrder;
        if (currentSortingOrder != SortingOrder)
        {
            currentSortingOrder = SortingOrder;
            SetLayer();
        }
    }

    void Reset()
    {
        var ps = GetComponent<ParticleSystem>();
        if (ps != null)
        {
            isParticle = true;
            SortingOrder = ps.GetComponent<Renderer>().sortingOrder;
            return;
        }

        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            isParticle = false;
            SortingOrder = renderer.sortingOrder;
            return;
        }

        isParticle = true;
        var subps = GetComponentInChildren<ParticleSystem>();
        if (subps != null)
        {
            SortingOrder = subps.GetComponent<Renderer>().sortingOrder;
        }
    }
}
