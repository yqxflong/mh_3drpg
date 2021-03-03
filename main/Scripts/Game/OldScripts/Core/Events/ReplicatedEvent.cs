using UnityEngine;

public abstract class ReplicatedEvent : GameEvent, EB.Replication.ISerializable
{
    public GameObject instigator = null;
    public GameObject target = null;
    public bool hostOnly = true;

    private bool _shouldReplicate = true;
    public bool ShouldReplicate
    {
        get
        {
            if (!hostOnly)
            {
                return false;
            }
            return _shouldReplicate;
        }
        set
        {
            if (hostOnly)
            {
                _shouldReplicate = value;
            }
        }
    }

    public ReplicatedEvent(GameObject instigator, GameObject target)
    {
        this.instigator = instigator;
        this.target = target;
    }

	public virtual void Serialize(EB.BitStream bs)
	{
		if (bs.isReading)
		{
			bool hasInstigator = true;
			bs.Serialize(ref hasInstigator);

			if (hasInstigator)
			{
				EB.Replication.ViewId instigatorViewID = new EB.Replication.ViewId();
				bs.Serialize(ref instigatorViewID);
				instigator = Replication.GetObjectFromViewId(instigatorViewID);
			}
			else
			{
				instigator = null;
			}

			bool hasTarget = true;
			bs.Serialize(ref hasTarget);

			if (hasTarget)
			{
				EB.Replication.ViewId targetViewID = new EB.Replication.ViewId();
				bs.Serialize(ref targetViewID);
				target = Replication.GetObjectFromViewId(targetViewID);
			}
			else
			{
				target = null;
			}
		}
		else
		{
			bool hasInstigator = instigator != null;
			bs.Serialize(ref hasInstigator);

			if (hasInstigator)
			{
				EB.Replication.ViewId instigatorViewID = instigator.GetComponent<ReplicationView>().viewId;
				bs.Serialize(ref instigatorViewID);
			}

			bool hasTarget = target != null;
			bs.Serialize(ref hasTarget);

			if (hasTarget)
			{
				EB.Replication.ViewId targetViewID = target.GetComponent<ReplicationView>().viewId;
				bs.Serialize(ref targetViewID);
			}
		}
	}
}
