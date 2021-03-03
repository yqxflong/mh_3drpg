using UnityEngine;
using System.Collections;
namespace PixelCrushers.DialogueSystem.SequencerCommands
{
	public class SequencerCommandMainMenuOpen : SequencerCommand
	{

		public void Start()
		{
			//ToDo:暂时屏蔽，方便解耦
			//LTMainMenuHudController.Instance.gameObject.SetActive(true);
			GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.LTMainMenuHudController", "SetActiveFromILR", true);
			Stop();
        }
	}
}
