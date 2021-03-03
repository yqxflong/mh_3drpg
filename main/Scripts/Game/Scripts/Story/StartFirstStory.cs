using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;

public class StartFirstStory : MonoBehaviour {

    // Use this for initialization
    private static StartFirstStory m_Instance;
    public static StartFirstStory Instance {
        get {
            if (m_Instance == null) {
                EB.Debug.LogError("StartFirstStory didnt Init");
            }
            return m_Instance;
        }
    }

    public  bool FirstStoryEnterIsShowing=false;
    public  bool FirstStoryExitIsShowing = false;
    private System.Action CombatCallback;

    void Awake() {
		if (m_Instance != null)
		{
			EB.Debug.LogError("StartFirstStory has Init");
		}
		m_Instance = this;
    }

	void OnDestory()
	{
		m_Instance = null;
	}

    public void Enter(System.Action callback)
    {
        StartFirstStoryEnterMethod(callback);
    }

    public void Exit(System.Action callback) {
        StartFirstStoryExitMethod(callback);
    }

    private void StartFirstStoryEnterMethod(System.Action callback)
    {
        if (!DialogueManager.IsConversationActive)
        {
            FirstStoryEnterIsShowing = true;
            DialogueManager.StartConversation("Story1Test");			
            CombatCallback = callback;
        }            
    }

    private void StartFirstStoryExitMethod(System.Action callback) {
        if (!DialogueManager.IsConversationActive) {
            FirstStoryExitIsShowing = true;
            DialogueManager.StartConversation("Story2Test");			
            CombatCallback = callback;
        }
    }

    private void Update() {
        if (FirstStoryEnterIsShowing)
        {
            if (!DialogueManager.IsConversationActive)
            {
                FirstStoryEnterIsShowing = false;
                CombatCallback();
            }
        }
        if (FirstStoryExitIsShowing)
        {
            if (!DialogueManager.IsConversationActive) {
                FirstStoryExitIsShowing = false;
                CombatCallback();
            }
        }
    }
}
