using UnityEngine;
using System.Collections;
namespace PixelCrushers.DialogueSystem.SequencerCommands
{
	public class SequencerCommandPlayDialogue : SequencerCommand
	{
		public void Start()
		{
			int dialogueid = GetParameterAsInt(0);
			GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.DialoguePlayUtil", "Instance", "Play", dialogueid, new System.Action(delegate ()
			{
				Stop();
			}));
		}
	}
}
