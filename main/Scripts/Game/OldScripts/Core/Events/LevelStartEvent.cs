using UnityEngine;
using System.Collections;

[VisibleAtDesignLevel]
public class LevelStartEvent : ReplicatedEvent
{
    public LevelStartEvent() : base(null, null) { }
}
