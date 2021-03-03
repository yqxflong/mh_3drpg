using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(UILabel))]

public class newUILabel:MonoBehaviour{

    public bool isUseUserDefinedShadowHeight = false;
    public float userDefinedShadowHeight;

    public UILabel parent;
    public UILabel son;
    private EventDelegate eventDel;

    private void Start()
    {
        parent = GetComponent<UILabel>();
        if (son == null)
        {
            Initialize();
        }
        SetAction();
    }

    private void Initialize()
    {
        if (parent.transform.childCount > 0 && parent.transform.GetChild(0).GetComponent<UILabel>() != null)
        {
            // 做这个处理的原因是，在item重用的时候，parent这个UILabel下面已经有了之前复制的UIlabel了，防止多重复制出现重影的情况
            son = parent.transform.GetChild(0).GetComponent<UILabel>();
            if (eventDel == null)
            {
                SetAction();
            }
            return;
        }

        parent.GetComponent<newUILabel>().enabled = false;
        GameObject sonGo = Instantiate(parent.gameObject, parent.transform);
        son = sonGo.GetComponent<UILabel>();
        son.depth = parent.depth - 1;
        son.color = Color.black;
        son.text = parent.text;
        Vector3 Position = transform.localPosition;
        if (isUseUserDefinedShadowHeight)
        {
            Position.y = -userDefinedShadowHeight;
        }
        else
        {
            Position.y = -4;
        }
        Position.x = 0;
        Position.z = 0;
        son.transform.localPosition = Position;
        parent.GetComponent<newUILabel>().enabled = true;
    }

    private void SetAction()
    {
        parent.OnCompleteFunc += OnTextUpdate;
        OnTextUpdate();
    }

    public void OnTextUpdate()
    {
        son.text = parent.text;
        son.fontSize = parent.fontSize;
    }
}

