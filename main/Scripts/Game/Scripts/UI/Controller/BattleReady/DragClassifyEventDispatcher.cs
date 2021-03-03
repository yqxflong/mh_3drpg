using UnityEngine;
using System.Collections.Generic;

public class DragClassifyEventDispatcher : MonoBehaviour
{
    public enum DRAG_TYPE
    {
        NONE,
        DRAGE_MODEL,
        OTHER,
    };

    public enum DragDirection
    {
        Left, Right, Up, Down
    }
    public DragDirection eDragDirection = DragDirection.Left;

    protected DRAG_TYPE eDragType = DRAG_TYPE.NONE;
    public List<EventDelegate> onDragFunc = new List<EventDelegate>();
    public List<EventDelegate> onDragStartFunc = new List<EventDelegate>();
    public List<EventDelegate> onDragEndFunc = new List<EventDelegate>();

    public float MIN_DISTANCE = 0.1f;
    public float MIN_ANGLE = 15f;
    private Vector3 startClickPosition;
    protected bool isPress;    

    public virtual void Start()
    {
        isPress = false;
        eDragType = DRAG_TYPE.NONE;
    }

    private void OnStartPress()
    {
        startClickPosition = UICamera.lastWorldPosition;
    }

    protected virtual void OnThisDragStart()
    {
        eDragType = DRAG_TYPE.DRAGE_MODEL;
        EventDelegate.Execute(onDragStartFunc);
    }

    protected virtual void OnThisDrag()
    {
        EventDelegate.Execute(onDragFunc);
    }

    protected virtual void OnThisDragEnd()
    {
        eDragType = DRAG_TYPE.NONE;
        EventDelegate.Execute(onDragEndFunc);
    }

    void OnPress(bool ispressed)
    {
        if(ispressed)
        {
            isPress = true;
            OnStartPress();
        }
        else
        {
            OnThisDragEnd();
            isPress = false;
        }
    }

    void Update()
    {
        if (isPress)
        {
            if (eDragType == DRAG_TYPE.NONE && Vector3.Distance(UICamera.lastWorldPosition, startClickPosition) > MIN_DISTANCE)
            {
                Vector3 dir=Vector3.left;
                switch (eDragDirection)
                {
                    case DragDirection.Left:
                        dir = Vector3.left;break;
                    case DragDirection.Right:
                        dir = Vector3.right;break;
                    case DragDirection.Down:
                        dir = Vector3.down;break;
                    case DragDirection.Up:
                        dir = Vector3.up;break;
                }

                if (Vector3.Angle(dir, UICamera.lastWorldPosition- startClickPosition) < MIN_ANGLE)
                {
                    OnThisDragStart();
                }
                else
                {
                    eDragType = DRAG_TYPE.OTHER;
                }
            }
            if(eDragType == DRAG_TYPE.DRAGE_MODEL )
            {
                OnThisDrag();
            }
        }
    }
}