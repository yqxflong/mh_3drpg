using UnityEngine;
using System.Collections;
using _HotfixScripts.Utils;
using PixelCrushers.DialogueSystem;
    
namespace Hotfix_LT.UI
{
    public class MainPlot : DynamicMonoHotfix, IHotfixUpdate
    {
        private bool StoryIsShowing = false;
        private System.Action CombatCallback;

        public override void OnEnable()
		{
			RegisterMonoUpdater();
		}

        public override void OnDisable()
        {
            ErasureMonoUpdater();
        }
        
        public void Enter(System.Action callback,string StoryData)
        {
            MainPlotEnterMethod(callback,StoryData);
        }
    
        private void MainPlotEnterMethod(System.Action callback, string StoryData)
        {
            if (!DialogueManager.IsConversationActive) {
                DialogueManager.StartConversation(StoryData);
                StoryIsShowing = true;
                CombatCallback = callback;
            }
        }
    
        public void Update ()
        {
            if (StoryIsShowing) {
                if (!DialogueManager.IsConversationActive) {
                    StoryIsShowing = false;
                    if(CombatCallback!=null)
                        CombatCallback();
                }
            }
        }
    }
}
