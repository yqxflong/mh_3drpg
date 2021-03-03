using System;
using UnityEngine;

public struct AttributeData
{
    public int Data;
    public bool IsAttackingRightSide;
    public bool IsCurrentAttackCritical;
    public int Turn;
    public long Hp;
    public Vector3 TargetPosition;
    public float TargetRadius;
    public Animator Animator;
    public MoveEditor.Move CurrentMove;
    public Vector3 HitPoint;
}
