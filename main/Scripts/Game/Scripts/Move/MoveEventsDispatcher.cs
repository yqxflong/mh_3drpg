using MoveEditor;
using System.Collections.Generic;
using UnityEngine;

public class MoveEventsDispatcher
{
	MoveController moveController = null;
	MoveEditor.Move currentMove = null;
	class MoveEventUpdateList
	{
		public MoveEvent[] moveArray;
		public float lastUpdatedFrame;
		public int currentEventIndex;
		
		public void Reset(MoveEvent[] moveArr, float startingFrame)
		{
			moveArray = moveArr;
			lastUpdatedFrame = startingFrame;
			currentEventIndex = 0;
		}
	}
	MoveEventUpdateList requiredEvents;
	MoveEventUpdateList regularEvents;

	Animator myAnimator;
	bool moveChanged;
	bool isCrossFading;

    FXHelper m_FXHelper;

    public bool MoveChanged
    {
        set
        {
            moveChanged = value;
        }
        get
        {
            return moveChanged;
        }
    }


    private int[] mCurrentPlayerMoveNum;
    private void SetPlayerMoves()
    {
        mCurrentPlayerMoveNum = new int[moveController.m_Moves.Length];
    }

    public MoveEventsDispatcher(MoveController moveController)
    {
        this.moveController = moveController;
    }

    public MoveEditor.Move GetMove()
	{
		return currentMove;
	}
	
	public void Awake()
	{
		myAnimator = moveController.GetComponent<Animator>();
        m_FXHelper = moveController.GetComponent<FXHelper>();
        SetPlayerMoves();
        requiredEvents = new MoveEventUpdateList();
		regularEvents = new MoveEventUpdateList();
		isCrossFading = false;
	}
	
	public void SetMove(MoveEditor.Move move, float startingFrame)
	{
		currentMove = move;
		regularEvents.Reset(move != null ? move.CompileMoveEvents(false) : null, startingFrame);
		requiredEvents.Reset(move != null ? move.CompileMoveEvents(true) : null, 0.0f);
        ResetPlayedNum(move != null ? move.CompileMoveEvents(false) : null);
        ResetPlayedNum(move != null ? move.CompileMoveEvents(true) : null);
        moveChanged = true;
        if (m_FXHelper != null && (move._moveState == MoveController.CombatantMoveState.kIdle) || move._moveState == MoveController.CombatantMoveState.kLocomotion)//by hzh: to prevent yabiao pathfinding bug
        {
            m_FXHelper.StopAll(true);
        }
	}
	
	public void Update()
    {
        if (!GameEngine.Instance.IsTimeToRootScene)
        {
            return;
        }

        if (moveChanged)
		{
			moveChanged = false;
		}

		if (moveController == null) 
		{
			return;
		}
		
		currentMove = moveController.CurrentMove;

		if(currentMove != null)
		{
			AnimatorStateInfo stateInfo = moveController.GetCurrentStateInfo();

			if(stateInfo.fullPathHash == moveController.GetCurrentAnimHash())
			{
				if (isCrossFading)
				{
					isCrossFading = false;
				}

				float currentFrame = regularEvents.lastUpdatedFrame; //isCrossFading ? regularEvents.lastUpdatedFrame : moveController.NormalizedTimeToFrame_Wrapped(stateInfo.normalizedTime);
				float nextFrame = moveController.NormalizedTimeToFrame_Wrapped(stateInfo.normalizedTime + (Time.deltaTime * myAnimator.speed / currentMove._animationClip.length));
				UpdateRequiredEvents(currentFrame, nextFrame);
				UpdateRegularEvents(currentFrame, nextFrame);
			}
		}
	}

	private void UpdateRequiredEvents(float currentFrame, float nextFrame)
	{
		if(requiredEvents.moveArray != null)
		{
			UpdateEventArray(requiredEvents, requiredEvents.lastUpdatedFrame, nextFrame);
		}
	}

	private void UpdateRegularEvents(float currentFrame, float nextFrame)
	{
		if(regularEvents.moveArray != null)
		{
			UpdateEventArray(regularEvents, currentFrame, nextFrame);
		}
	}

    private void UpdateEventArray(MoveEventUpdateList moveEventList, float startingFrame, float nextFrame)
	{
		float fixedNextFrame = nextFrame < startingFrame ? currentMove.NumFrames : nextFrame;
		while (moveEventList.currentEventIndex < moveEventList.moveArray.Length)
        {
            MoveEvent currentEvent = moveEventList.moveArray[moveEventList.currentEventIndex];
			float moveEventFrame = moveController.NormalizedTimeToFrame_Wrapped(currentEvent.myAnimEvent.time);

			if (startingFrame <= moveEventFrame && moveEventFrame <= fixedNextFrame)
            {
				TryTriggerEvent(currentEvent);
				if (moveChanged == true)
				{
					break;
				}
			}
			else if(moveEventFrame > fixedNextFrame)
			{
				break;
			}
			moveEventList.currentEventIndex++;
		}
		// if we wrap frames for a LOOPING animation, then play from 0 to the proper ending frame
		if (moveEventList.currentEventIndex == moveEventList.moveArray.Length && nextFrame < startingFrame)
		{
#if UNITY_ANDROID
			if (moveController.GetCurrentStateInfo().loop)
#else
			if (currentMove._animationClip != null && currentMove._animationClip.isLooping)
#endif
			{
				moveEventList.currentEventIndex = 0;
				if(moveEventList.moveArray.Length > 1 && nextFrame > 0.0f)
				{
					UpdateEventArray(moveEventList, 0.0f, nextFrame);
				}
			}
		}
		
		moveEventList.lastUpdatedFrame = nextFrame;
	}

	private Dictionary<int, int> mDicEventPlayedNums = new Dictionary<int, int>();
    private void ResetPlayedNum(MoveEvent[] mEvents)
    {
        if (mEvents == null)
        {
            return;
        }

		for (var i = 0; i < mEvents.Length; i++)
        {
            var mEvent = mEvents[i];
            int hashcode = mEvent.GetHashCode();

			if (mDicEventPlayedNums.ContainsKey(hashcode))
            {
                mDicEventPlayedNums[hashcode] = 0;
            }
            else
            {
                mDicEventPlayedNums.Add(hashcode, 0);
            }
        }
    }
    private int GetPlayedNum(MoveEvent mEvent)
    {
        if (mEvent == null)
        {
            return 0;
        }
        int num = 0;
        int hashcode = mEvent.GetHashCode();
        if (mDicEventPlayedNums.ContainsKey(hashcode))
        {
            mDicEventPlayedNums.TryGetValue(hashcode, out num);
        }
        else
        {
            mDicEventPlayedNums.Add(hashcode, 0);
        }
        return num;
    }
    private void AddPlayedNum(MoveEvent mEvent)
    {
        if (mEvent == null)
        {
            return;
        }
        
        int hashcode = mEvent.GetHashCode();
        if (mDicEventPlayedNums.ContainsKey(hashcode))
        {
            mDicEventPlayedNums[hashcode]++;
        }
        else
        {
            mDicEventPlayedNums.Add(hashcode, 0);
        }
    }

	private void TryTriggerEvent(MoveEvent moveEvent)
	{
        int num = GetPlayedNum(moveEvent);

		if (moveEvent.playOnce == false || num == 0)
        {
			TriggerEvent(moveEvent);
        }
    }

	private void TriggerEvent(MoveEvent moveEvent)
	{		
		AddPlayedNum(moveEvent);

		if (!moveEvent.myAnimEvent.functionName.Equals("OnMoveEvent"))
		{
			var ilrObj = moveController.transform.GetComponent<Hotfix_LT.Combat.Combatant>();

			if (ilrObj != null)
			{
				bool handle = ilrObj.OnHandleMessage(moveEvent.myAnimEvent.functionName, moveEvent.myAnimEvent);
				if (!handle)
				{
					moveController.gameObject.SendMessage(moveEvent.myAnimEvent.functionName, moveEvent.myAnimEvent, moveEvent.myAnimEvent.messageOptions);
				}
			}
			else
            {
				moveController.gameObject.SendMessage(moveEvent.myAnimEvent.functionName, moveEvent.myAnimEvent, moveEvent.myAnimEvent.messageOptions);
			}
		}
	}

	public void OnCrossFade(MoveEditor.Move move, float normalizedTime)
	{
		if(isCrossFading)
			return;

		isCrossFading = true;

		if(normalizedTime > 1.0f)
		{
			normalizedTime = normalizedTime - Mathf.Floor(normalizedTime);
		}
		float startingFrame = move == null ? 0.0f : normalizedTime * move._animationClip.length * move._animationClip.frameRate;

		if(currentMove != move)
		{
			regularEvents.Reset(move != null ? move.CompileMoveEvents(false) : null, startingFrame);
			requiredEvents.Reset(move != null ? move.CompileMoveEvents(true) : null, 0.0f);
			currentMove = move;
			moveChanged = true;
		}
		else if(currentMove != null && moveController != null)
		{
			AnimatorStateInfo stateInfo = moveController.GetCurrentStateInfo();
			if(stateInfo.fullPathHash == moveController.GetCurrentAnimHash())
			{
				float currentFrame = moveController.NormalizedTimeToFrame_Wrapped(stateInfo.normalizedTime);
				UpdateRegularEvents(regularEvents.lastUpdatedFrame, currentFrame);
			}
		}
		UpdateRequiredEvents(requiredEvents.lastUpdatedFrame, startingFrame);		
	}
}
