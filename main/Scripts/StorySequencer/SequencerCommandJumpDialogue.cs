using UnityEngine;
using System.Collections;
namespace PixelCrushers.DialogueSystem.SequencerCommands
{
	public class SequencerCommandJumpDialogue : SequencerCommand
	{
		
		public void Start()
		{
			GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.DialogueFrame", "Instance", "OnClick");
            Stop();
		}
	}
}
