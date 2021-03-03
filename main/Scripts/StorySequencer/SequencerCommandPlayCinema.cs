using UnityEngine;
using System.Collections;
namespace PixelCrushers.DialogueSystem.SequencerCommands
{
	public class SequencerCommandPlayCinema : SequencerCommand
	{
		public void Start()
		{
			string name = GetParameter(0);
			GameObject moveObject = GameObject.Find(name);
			Cinematic CinemaComponent = moveObject.GetComponent<Cinematic>();
			Cinematic.Play(CinemaComponent, PlayerManager.LocalPlayerGameObject());
			Stop();
		}
	}
}
