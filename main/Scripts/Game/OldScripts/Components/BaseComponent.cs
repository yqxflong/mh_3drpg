using UnityEngine;

public abstract class BaseComponent : MonoBehaviour
{
	protected ReplicationView FindReplicationViewForComponent<ComponentType>()
	{
		ReplicationView view = null;
		ReplicationView[] replicationViews = GetComponents<ReplicationView>();

		foreach ( ReplicationView replicationView in replicationViews )
		{
			if ( replicationView.observed is ComponentType )
			{
				view = replicationView;
				break;
			}
		}

		return view;
	}
}
 
