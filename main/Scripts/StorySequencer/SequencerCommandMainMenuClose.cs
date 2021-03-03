using UnityEngine;
using System.Collections;
namespace PixelCrushers.DialogueSystem.SequencerCommands
{
	public class SequencerCommandMainMenuClose : SequencerCommand
	{

		public void Start()
		{
			//ToDo:暂时屏蔽，方便解耦
			//LTMainMenuHudController.Instance.gameObject.SetActive(false);
			GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.LTMainMenuHudController", "SetActiveFromILR", false);
			Stop();
		}
	}
}
