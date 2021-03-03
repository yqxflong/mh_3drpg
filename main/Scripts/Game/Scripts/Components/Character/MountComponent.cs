using UnityEngine;
using System.Collections;

public class MountComponent : MonoBehaviour {
	private Animator _animator=null;
	private PartitionObject _mountPartition=null;
	private GameObject _mountObject = null;

	//Mount
	public bool Mount(string name, string assetName)
	{
		if(_mountPartition!=null && _mountPartition.Name== name && _mountPartition.AssetName == assetName)
		{
			return false;
		}
		if(_mountPartition==null)	
		{
			_mountPartition = CreatePartitionObject(name, assetName);	
        }
		else
		{
			if(_mountObject!=null)
			{
				transform.parent = _mountObject.transform.parent;
				_mountPartition.UnregisterObject(_mountObject);
				_mountObject = null;
				_animator = null;
            }
			_mountPartition.Name = name;
			_mountPartition.AssetName = assetName;
        }
		_mountPartition.LoadAsset(OnAssetLoaded, gameObject);

		return true;
	}

	void OnAssetLoaded(string name, string assetName, GameObject partitionObj, bool isSuccess, bool isLinkObj)
	{
		if (!isSuccess)
		{
			EB.Debug.LogError(string.Format("[MountComponent]Failed to load asset {0}", assetName));
			return;
		}

		if (partitionObj == null)
		{
			EB.Debug.LogError(string.Format("[MountComponent]Load asset {0}  to null instance.", assetName));
			return;
		}

		InternalMount(name, assetName, partitionObj);

		// GM.AssetManager.UnRefAsset(assetName, false);
	}

	public void InternalMount(string name, string assetName, GameObject partitionObj)
	{
		if (_mountPartition == null)
		{
			return;
		}
		//maybe when Async load  ther Name Partition Mount
		if(name!= _mountPartition.Name || assetName!= _mountPartition.AssetName)
		{
			_mountPartition.UnregisterObject(partitionObj);
			return;
		}
		_mountObject = partitionObj;
        transform.parent = _mountObject.transform.parent;
		_animator = _mountObject.GetComponent<Animator>();
    }

	PartitionObject CreatePartitionObject(string name, string assetName)
	{
		PartitionObject partition = new PartitionObject();
		partition.Name = name;
		partition.AssetName = assetName;

		return partition;
	}

	//Move idle state
	public void SetDesiredState(MoveController.CombatantMoveState stateCommand)
	{
		if (_animator != null)
		{
			_animator.SetInteger("State", (int)stateCommand);
		}
	}
}
