using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum InputBlockReason
{
    UI_SERVER_REQUEST,
    SET_BLOCK_PANEL_ON,
    BINDABLE_ITEM_FLY_ANIM,
    SOCKET_ITEM_FLY_ANIM,
    CONVERT_FLY_ANIM,
    FUSION_BLOCK_UI_INTERACTION,
    SCREEN_TRANSITION_MASK,
    UI_GUIDE_FOBIDDEN,
}

public class InputBlockerManager : MonoBehaviour 
{
    static InputBlockerManager _instance = null;

    public static InputBlockerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("InputBlockerManager");
                go.hideFlags = HideFlags.HideAndDontSave;
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<InputBlockerManager>();
            }

            return _instance;
        }
    }

    public struct InputBlockInfo
    {
        public InputBlockReason reason;
        public float timer;
        public float triggerTime;
        
        public InputBlockInfo(InputBlockReason _reason, float _timer)
        {
            reason = _reason;
            timer = _timer;
            triggerTime = RealTime.time;
        }
        
        // Override the ToString method:
        public override string ToString()
        {
            return(string.Format("({0},{1})", reason.ToString(), timer));   
        }
    };

    bool _isBlocking = false;
    List<InputBlockInfo> blockList = new List<InputBlockInfo>();

    public string GetStatus()
    {
        int listCount = blockList.Count;

        if(listCount > 0)
        {
            var status = new System.Text.StringBuilder();

            for(int i = 0; i < blockList.Count; i++)
            {
                status.Append(blockList[i].ToString()).Append(",");
            }

            return status.ToString();
        }
        else
        {
            return "None";
        }
    }

    public bool isBlocking()
    {
        return _isBlocking;
    }
    
    /// <summary>
    /// Unblock user input.
    /// Note: Always unlock one specific block, unless you really mean to unlock everything!
    /// </summary>
    /// <param name="blockReason">If not set, it will force unblocking everything.</param>
    public void UnBlock(InputBlockReason blockReason)
    {
        //EB.Debug.Log("[InputBlockerManager]UnBlock: " + blockReason.ToString());
        //EB.Debug.Log(GetStatus());

        // No block in the list, just return.
        if(blockList.Count == 0)
        {
            _isBlocking = false;
            return;
        }
        
        int removedNum = blockList.RemoveAll(delegate(InputBlockInfo obj) {
            return blockReason == obj.reason;
       });

        if(removedNum == 0)
        {
            //EB.Debug.Log("[InputBlockerManager]UnBlock: Block reason " + blockReason + " not found. It's been expired or never get triggered!");
        }

        if(blockList.Count == 0)
        {
            _UnBlock();
        }
    }

    /// <summary>
    /// Forces unlock user input.
    /// NOTE: This will get rid of all the locking history, only use this when you really need it.
    /// </summary>
    public void ForceUnlockAll()
    {
        _UnBlock();
        blockList.Clear();
    }
    
    /// <summary>
    /// Block user's input.
    /// </summary>
    /// <param name="blockReason">For what reason you need to block user's input.</param>
    /// <param name="timer">Timer in seconds.</param>
    public void Block(InputBlockReason blockReason, float timer = 2f)
    {
        //EB.Debug.Log("[InputBlockerManager]Block: " + blockReason.ToString() + ", " + timer.ToString() + "secs");

        if(!_isBlocking)
        {
            _Block();
        }

        InputBlockInfo newBlock = new InputBlockInfo(blockReason, timer);
        blockList.Add(newBlock);
    }

    void Update()
    {
        if(blockList.Count == 0 && !_isBlocking) return;

        blockList.RemoveAll(delegate(InputBlockInfo obj)
        {
            return obj.triggerTime + obj.timer < RealTime.time;
        });

        if(blockList.Count > 0 && !_isBlocking)
        {
            _Block();
        }
        else if(blockList.Count == 0)
        {
            _UnBlock();
        }
    }

    void _Block()
    {
        //EB.Debug.Log("[InputBlockerManager]_Block");

        if(UICamera.mainCamera != null)
        {
            UICamera mainUICamera = UICamera.mainCamera.GetComponent<UICamera>();
            
            if(mainUICamera != null)
            {
                mainUICamera.useTouch = false;
                mainUICamera.useMouse = false;
                mainUICamera.useKeyboard = false;
                _isBlocking = true;
            }
        }

        if(!_isBlocking)
        {
            EB.Debug.LogWarning("[InputBlockerManager]_Block: Fail to block input!");
        }
    }
    
    void _UnBlock()
    {
        //EB.Debug.Log("[InputBlockerManager]_UnBlock");
        
        if(UICamera.mainCamera != null && _isBlocking)
        {
            UICamera mainUICamera = UICamera.mainCamera.GetComponent<UICamera>();
            
            if(mainUICamera != null)
            {
                mainUICamera.useTouch = true;
                mainUICamera.useMouse = true;
                mainUICamera.useKeyboard = true;
                _isBlocking = false;
            }
        }

        if(_isBlocking)
        {
            EB.Debug.LogWarning("[InputBlockerManager]_Block: Fail to unblock input!");
        }
    }
    
}
