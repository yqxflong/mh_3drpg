///////////////////////////////////////////////////////////////////////
//
//  CharacterComponent.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class CharacterTargetingComponent : BaseComponent 
{
	private const float MinDistForNewMovementTargetSq = 1.5f * 1.5f;

	private GameObject _attackTarget;
	private Vector3? _movementTarget;

	private CharacterComponent _characterComponent;
	private NetworkOwnershipComponent _network;

	private ReplicationView _viewRPC;
	private EB.Collections.Queue<Vector3> _movePosQue=new EB.Collections.Queue<Vector3>();
	private System.Action _onMoveEndActionByQue;

	public delegate void MovementTargetChangeRequestEventHandler(Vector3 requestedTarget,bool isNull);
	public event MovementTargetChangeRequestEventHandler OnMovementTargetChangeRequest;

	public delegate void MovementTargetChangedEventHandler(Vector3 newMovementTarget, bool isNull);
	public event MovementTargetChangedEventHandler OnMovementTargetChanged;

	public delegate void AttackTargetChangedEventHandler(GameObject newAttackTarget);
	public event AttackTargetChangedEventHandler OnAttackTargetChanged;

	public delegate void AttackTargetDeathEventHandler(GameObject killer, bool isForcedKill);
	public event AttackTargetDeathEventHandler OnAttackTargetDeath;	

	public GameObject AttackTarget 
	{
		get 
		{
			return _attackTarget;
		}
		set 
		{
			SetAttackTarget(value);
		}
	}

	public Vector3? MovementTarget
	{
		get 
		{
			return _movementTarget;
		}
		set 
		{
            bool isNull = (value == null);
            Vector3 v3 = isNull ? Vector3.zero : value.Value;
			SetMovementTarget(v3, isNull);
		}
	}

    public bool HasMovementTarget()
    {
        return _movementTarget != null;
    }
    public Vector3 GetMovementTarget()
    {
        return _movementTarget.Value;
    }


    public void Awake() 
	{
		_characterComponent = GetComponent<CharacterComponent>();
		_network = GetComponent<NetworkOwnershipComponent>();

		_viewRPC = FindReplicationViewForComponent<CharacterTargetingComponent>();
	}

	public void SetAttackTarget(GameObject target, bool shouldIssueCommand = true) 
	{
		if (/*(GameStateManager.Instance.State != eGameState.Overworld) &&*/ !NetworkOwnershipComponent.IsGameObjectLocallyOwned(gameObject, _network))
			return;

		if (target != null)
		{
            SetMovementTargetNoRPC(Vector3.zero, true);
            SetAttackTargetFromRPC(target, shouldIssueCommand);
        }
		else
		{	
			ClearAttackTargetRPC();

			if (_viewRPC != null)
			{
				_viewRPC.RPC("ClearAttackTargetRPC", EB.RPCMode.Others);
			}
		}
	}


    public void SetMovementTargetNoRPC(Vector3 target,bool isNull=false , bool shouldIssueCommand = true)
    {
        //if ( !NetworkOwnershipComponent.IsGameObjectLocallyOwned(gameObject, _network))
        //    return;

        if (!isNull)
        {
            bool isValidNewTarget = !_movementTarget.HasValue;
            if (_movementTarget.HasValue)
            {
                Vector3 toTarget = target - _movementTarget.Value;
                isValidNewTarget = toTarget.sqrMagnitude > MinDistForNewMovementTargetSq;
            }

            if (isValidNewTarget)
            {
                SetMoveTargetRPC(target, shouldIssueCommand);


                if (_viewRPC != null)
                {
                    _viewRPC.RPC("SetMoveTargetRPC", EB.RPCMode.Others, target, shouldIssueCommand);
                }
            }
        }
        else
        {
            ClearMoveTargetRPC();
            if (_viewRPC != null)
            {
                _viewRPC.RPC("ClearMoveTargetRPC", EB.RPCMode.Others);
            }
        }

        if (OnMovementTargetChangeRequest != null)
        {
            OnMovementTargetChangeRequest(target,isNull);
        }
    }

	public void SetMovementTargetQueue(EB.Collections.Queue<Vector3> movePosQue,System.Action onEndCallback)
	{
		_movePosQue = movePosQue;
		_onMoveEndActionByQue = onEndCallback;
		GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.AlliancesManager", "RecordTransferPointFromILRWithCallback", _movePosQue.Count - 1);
		SetMovementTarget(_movePosQue.Dequeue(),false, true, false,true);
	}

	public void ClearMovementQueue()
	{
		_movePosQue.Clear();
		_onMoveEndActionByQue = null;
		StopMoveInDestination();
		_characterComponent.Stop();
	}

	/// <summary>
	/// �������ڵ��Ѿ�����Ŀ�ĵ�ֹͣ�ƶ� ����Ҫ����������Ϣ
	/// </summary>
	/// <param name="shouldIssueCommand"></param>
	public void StopMoveInDestination(bool shouldIssueCommand = true)
    {
		if (!NetworkOwnershipComponent.IsGameObjectLocallyOwned(gameObject, _network))
            return;

        ClearMoveTargetRPC();
        if (_viewRPC != null)
        {
            _viewRPC.RPC("ClearMoveTargetRPC", EB.RPCMode.Others);
        }

        if (OnMovementTargetChangeRequest != null)
        {
            OnMovementTargetChangeRequest(Vector3 .zero, true);
        }	
    }

	public bool MoveToNextPos()
	{
		if (_movePosQue.Count > 0)
		{
			GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.AlliancesManager", "RecordTransferPointFromILRWithCallback", _movePosQue.Count - 1);
			SetMovementTarget(_movePosQue.Dequeue(),false, true, false,true);
			return true;
		}
		else if (_onMoveEndActionByQue != null)
		{
			_onMoveEndActionByQue();
			_onMoveEndActionByQue = null;
		}
		return false;
	}

    public void SetMovementTarget(Vector3 target, bool isNull=false , bool shouldIssueCommand = true, bool isNeedShowReticle = true,bool force=false)
	{
		if (!force)
		{
			if (!NetworkOwnershipComponent.IsGameObjectLocallyOwned(gameObject, _network))
				return;
		}
		if (force)
		{
			EB.Debug.Log("Start SetMovementTarget by transfer dart");
		}

		if (!isNull)
		{
			bool isValidNewTarget = !_movementTarget.HasValue;
			if (_movementTarget.HasValue)
			{
				Vector3 toTarget = target - _movementTarget.Value;
				isValidNewTarget = toTarget.sqrMagnitude > MinDistForNewMovementTargetSq;
			}

			if (isValidNewTarget)
			{
                SetMoveTargetRPC(target, shouldIssueCommand);
				// Hotfix_LT.Messenger.Raise<Vector3>(Hotfix_LT.EventName.PlayerMoveSyncManagerMove, target);
				//GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "PlayerMoveSyncManagerMove", target);
				if (_viewRPC != null)
				{
					_viewRPC.RPC("SetMoveTargetRPC", EB.RPCMode.Others, target, shouldIssueCommand);
				}
			}
		}
		else
		{
			ClearMoveTargetRPC();
			// Hotfix_LT.Messenger.Raise<Vector3>(Hotfix_LT.EventName.PlayerMoveSyncManagerMove, gameObject.transform.position);
			//GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "PlayerMoveSyncManagerMove", gameObject.transform.position);
			if (_viewRPC != null)
			{
				_viewRPC.RPC("ClearMoveTargetRPC", EB.RPCMode.Others);
			}
		}

		if (isNeedShowReticle && OnMovementTargetChangeRequest != null)
		{
			OnMovementTargetChangeRequest(target,isNull);
		}
	}

	public void ClearMovementTarget() 
	{
		_movementTarget = null;

		if (OnMovementTargetChanged != null)
		{
			OnMovementTargetChanged(Vector3.zero,true);
		}
	}
    
	private void SetAttackTargetFromRPC(GameObject target, bool shouldIssueCommand)
	{
		if (_attackTarget != null)
		{
			CombatController combat = _attackTarget.GetComponent<CombatController>();
			if (combat != null) 
			{
				combat.OnStopAttack(gameObject);
			}
		}

		_attackTarget = target;
		if (_attackTarget != null)
		{

			CombatController combat = _attackTarget.GetComponent<CombatController>();
			if (combat != null)
			{
				combat.OnStartAttack(gameObject);
			}

			if (shouldIssueCommand)
			{
				_characterComponent.IssueAttackCommand(target);
			}
		}

		if (OnAttackTargetChanged != null)
		{
			OnAttackTargetChanged(_attackTarget);
		}
	}

	//[RPC]
	private void SetAttackTargetRPC(EB.Replication.ViewId viewId, bool shouldIssueCommand)
	{
		GameObject target = Replication.GetObjectFromViewId(viewId);
		SetAttackTargetFromRPC(target, shouldIssueCommand);
	}

	//[RPC]
	private void ClearAttackTargetRPC()
	{
		SetAttackTargetFromRPC(null, false);
	}

	//[RPC]
	private void SetMoveTargetRPC(Vector3 target, bool shouldIssueCommand)
	{
		_movementTarget = target;
		
		if (shouldIssueCommand && _movementTarget != null) 
		{
			_characterComponent.IssueMoveCommand(_movementTarget.Value);
		}

		if (OnMovementTargetChanged != null)
		{
			OnMovementTargetChanged(target, false);
		}
	}

	//[RPC]
	private void ClearMoveTargetRPC()
	{
		_movementTarget = null;

		if (_characterComponent.State == eCampaignCharacterState.Move)
		{
			_characterComponent.Stop();
		}
		
		if (OnMovementTargetChanged != null)
		{
			OnMovementTargetChanged(Vector3 .zero,true);
		}
	}
}
