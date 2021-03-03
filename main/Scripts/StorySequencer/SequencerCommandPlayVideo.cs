using UnityEngine;
using System.Collections;
namespace PixelCrushers.DialogueSystem.SequencerCommands {
    public class SequencerCommandPlayVideo : SequencerCommand {
        public void Start() {
            //string name = GetParameter(0);

            StartCoroutine(PlayVideoUtil.Instance.PlayVideo(delegate()
            {
                Stop();
            }));
            
        }
    }
}