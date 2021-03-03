///////////////////////////////////////////////////////////////////////
//
//  OpeningAndClosingSequence.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using _HotfixScripts.Utils;

namespace Hotfix_LT.UI
{
	public class OpeningAndClosingSequence : DynamicMonoHotfix, IHotfixUpdate
	{
		public delegate void ClosingSequenceBeginDelegate();
		public delegate void ClosingSequenceEndDelegate();

		public bool debugEnd = false;
		public bool debugStart = false;

		public float OpeningAutoPinchTime = 1.25f;
		public float ClosingAutoPinchTime = 1f;

		private static OpeningAndClosingSequence _instance = null;

		private enum eClosingSequenceStage
		{
			autoPinch,
			none,
		}
		private eClosingSequenceStage _closingSequenceStage = eClosingSequenceStage.none;
		private float _autoPinchStartTime = -1f;
		private float _autoPinchStartZoom = -1f;

		private ClosingSequenceBeginDelegate _onClosingSequenceBegin = null; // a callback for when the closing sequence begins
		private ClosingSequenceEndDelegate _onClosingSequenceEnd = null; // a callback for when the closing sequence ends

		private enum eOpeningSequenceStage
		{
			cutToCloseUp,
			autoPinch,
			none,
		}
		private eOpeningSequenceStage _openingSequenceStage = eOpeningSequenceStage.none;
		private Vector3 _spawnerFacingDirection = Vector3.right;

		private CameraBase _cameraComponent;
		private GameObject player;

		public static OpeningAndClosingSequence instance
		{
			get
			{
				return _instance;
			}
		}

		// start the end sequence (callback will only be called if closing sequence plays)
		public bool PlayClosingSequence()
		{
			bool IsInHostileDungeon = false;
			if (IsInHostileDungeon) // only play the outro in a hostile dungeon
			{
				if (!_cameraComponent)
				{
					_cameraComponent = mDMono.transform.GetComponent<CameraBase>();
				}

				PlayerController.LocalPlayerDisableNavigation();

				if (null == player)
				{
					player = PlayerManager.LocalPlayerGameObject();
				}

				Vector3 objectToCamera = Vector3.zero;
				Transform playerTransform = player.transform;
				if (_cameraComponent.CalculateObjectToCameraXZDir(playerTransform, ref objectToCamera))
				{
					StartSwivel(objectToCamera);
				}

				_closingSequenceStage = eClosingSequenceStage.autoPinch;
				_autoPinchStartTime = Time.time;
				_autoPinchStartZoom = _cameraComponent.GetZoomDistance();

				if (null != _onClosingSequenceBegin)
				{
					_onClosingSequenceBegin();
				}
				return true;
			}
			return false;
		}

		// do we want a callback for when the closing sequence begins
		public void AddClosingSequenceBeginCallback(ClosingSequenceBeginDelegate callback)
		{
			RemoveClosingSequenceBeginCallback(callback); // avoid duplicate addition
			_onClosingSequenceBegin += callback;
		}

		// do we no longer want a callback for when the closing sequence begins
		public void RemoveClosingSequenceBeginCallback(ClosingSequenceBeginDelegate callback)
		{
			_onClosingSequenceBegin -= callback;
		}

		// do we want a callback for when the closing sequence end
		public void AddClosingSequenceEndCallback(ClosingSequenceEndDelegate callback)
		{
			RemoveClosingSequenceEndCallback(callback); // avoid duplicate addition
			_onClosingSequenceEnd += callback;
		}

		// do we no longer want a callback for when the closing sequence begins
		public void RemoveClosingSequenceEndCallback(ClosingSequenceEndDelegate callback)
		{
			_onClosingSequenceEnd -= callback;
		}

		// start the opening sequence
		public void PlayOpeningSequence()
		{
			bool IsInHostileDungeon = false;
			if (IsInHostileDungeon) // only play the intro in a hostile dungeon
			{
				if (!_cameraComponent)
				{
					_cameraComponent = mDMono.transform.GetComponent<CameraBase>();
				}

				_cameraComponent.SetZoomDistance(0f); // fully pinched in
				_cameraComponent.SetInitialCameraPosition(); // to ensure we cut to fully zoomed in, rather than lerping to it

				PlayerController.LocalPlayerDisableNavigation();

				if (null == player)
				{
					player = PlayerManager.LocalPlayerGameObject();
				}
				Transform playerTransform = player.transform;
				_spawnerFacingDirection = playerTransform.forward; // save off what the player's facing direction from the spawner was, as we'll swivel back to that later

				Vector3 objectToCamera = Vector3.zero;
				if (_cameraComponent.CalculateObjectToCameraXZDir(playerTransform, ref objectToCamera))
				{
					player.transform.rotation = Quaternion.LookRotation(objectToCamera); // make the player look at the camera
				}
				_openingSequenceStage = eOpeningSequenceStage.cutToCloseUp;
			}
		}

		// start the opening sequence animation
		public void PlayOpeningSequenceAnimation()
		{
			/*
			if (GameStateManager.Instance && GameStateManager.Instance.IsInHostileDungeon && eOpeningSequenceStage.cutToCloseUp == _openingSequenceStage)
			{
				// play the opening animation
				CharacterComponent charTarget = player.GetComponent<CharacterComponent>();
				if (null != charTarget)
				{
					PlayStartOrEndAnimationSound(true);
					PlaySpecialAnimation(eSpecialAnimationVariety.DungeonStart, charTarget, false);
				}
			}
			*/
		}

		// start the auto pinch out stage of the opening sequence
		public void PlayOpeningSequenceAutoPinchOut()
		{
			if (eOpeningSequenceStage.cutToCloseUp == _openingSequenceStage)
			{
				_openingSequenceStage = eOpeningSequenceStage.autoPinch;
				_autoPinchStartTime = Time.time;
			}
		}

		public override void OnEnable()
		{
			_instance = this;
			RegisterMonoUpdater();

			EventManager.instance.AddListener<LevelStartEvent>(OnLevelStart);

			if (!_cameraComponent)
			{
				_cameraComponent = mDMono.transform.GetComponent<CameraBase>();
			}
		}

		public override void OnDisable()
		{
            ErasureMonoUpdater();
			EventManager.instance.RemoveListener<LevelStartEvent>(OnLevelStart);
			_instance = null;
		}

		// perform any logic on level start
		private void OnLevelStart(LevelStartEvent e)
		{
		}

		public void Update()
		{
			if (!GameEngine.Instance.IsTimeToRootScene)
			{
				return;
			}
			if (debugEnd)
			{
				debugEnd = false;
				PlayClosingSequence();
			}

			if (debugStart)
			{
				debugStart = false;
				PlayOpeningSequence();
				PlayOpeningSequenceAutoPinchOut();
			}

			OpeningSequenceUpdate();
			ClosingSequenceUpdate();
		}

		private void OpeningSequenceUpdate()
		{
			switch (_openingSequenceStage)
			{
				case eOpeningSequenceStage.autoPinch:
					float mult = (Time.time - _autoPinchStartTime) / OpeningAutoPinchTime;
					if (mult >= 1f) // finished auto pinch
					{
						_cameraComponent.SetZoomDistance(1f);
						_openingSequenceStage = eOpeningSequenceStage.none;

						if (UIHierarchyHelper.Instance)
						{
							UIHierarchyHelper.Instance.ShowRegularHUD(true); // we're going back to the normal HUD
						}
						//EventManager.instance.Raise(new IntroCompleteEvent()); // the intro sequence is over
						PlayerController.LocalPlayerEnableNavigation(); // normal player control

						StartWalk(_spawnerFacingDirection);
					}
					else
					{
						mult = Cinematic.Smooth(mult, CameraLerp.LerpSmoothing.slowFastSlow);
						_cameraComponent.SetZoomDistance(mult);
					}
					break;
				case eOpeningSequenceStage.none:
					break;
				default: break;
			}
		}

		// start the swivel
		private void StartSwivel(Vector3 dir)
		{
			LocomotionComponentAPP loco = player.GetComponent<LocomotionComponentAPP>();
			if (null != loco)
			{
				loco.LookAtPosition(player.transform.position + dir, true);
			}
		}

		// start the walk
		private void StartWalk(Vector3 dir)
		{
			CharacterTargetingComponent character = player.GetComponent<CharacterTargetingComponent>();
			if (null != character)
			{
				const float MoveOffset = 2f;
				character.SetMovementTarget(player.transform.position + dir * MoveOffset);
			}
		}

		private void ClosingSequenceUpdate()
		{
			switch (_closingSequenceStage)
			{
				case eClosingSequenceStage.autoPinch:
					float mult = (Time.time - _autoPinchStartTime) / ClosingAutoPinchTime;
					if (mult >= 1f) // finished auto pinch
					{
						_cameraComponent.SetZoomDistance(0f);

						// play the closing animation
						GameObject localPlayer = PlayerManager.LocalPlayerGameObject();
						CharacterComponent charTarget = null;
						if (null != localPlayer)
						{
							charTarget = localPlayer.GetComponent<CharacterComponent>();
							if (null != charTarget)
							{
								PlayStartOrEndAnimationSound(false);
								PlaySpecialAnimation(eSpecialAnimationVariety.DungeonEnd, charTarget, false);
							}
						}

						StartCoroutine(WaitForClosingSequenceEnd(charTarget));
					}
					else
					{
						mult = Cinematic.Smooth(mult, CameraLerp.LerpSmoothing.slowFastSlow);
						_cameraComponent.SetZoomDistance(_autoPinchStartZoom - mult * _autoPinchStartZoom);
					}
					break;
				case eClosingSequenceStage.none:
					break;
				default: break;
			}
		}

		private IEnumerator WaitForClosingSequenceEnd(CharacterComponent charTarget)
		{
			_closingSequenceStage = eClosingSequenceStage.none;
			const float HoldTime = 1f;
			yield return new WaitForSeconds(HoldTime); // regardless of how long the animation is, we'll wait at least the length of HoldTime

			while (IsSpecialAnimationPlaying(charTarget)) // wait for the special animation to end
			{
				yield return null;
			}

			ClosingSequenceEnd();
		}

		private void ClosingSequenceEnd()
		{
			PlayerController.LocalPlayerEnableNavigation();
			if (null != _onClosingSequenceEnd)
			{
				_onClosingSequenceEnd();
			}
		}

		private void PlayStartOrEndAnimationSound(bool isStart)
		{
			eGender gender = CharacterManager.Instance.CurrentCharacter.GeneralRecord.gender;
			//eClass characterClass = CharacterManager.Instance.CurrentCharacter.GeneralRecord.Class;

			FusionAudio.eEvent evt = FusionAudio.eEvent.None;

			//switch (characterClass)
			//{
			//	case eClass.Barbarian:
			//		if (gender == eGender.Female)
			//		{
			//			if (isStart)
			//			{
			//				evt = FusionAudio.eEvent.SFX_Barbarian_Female_Dungeon_Start;
			//			}
			//			else
			//			{
			//				evt = FusionAudio.eEvent.SFX_Barbarian_Female_Dungeon_End;
			//			}
			//		}
			//		else
			//		{
			//			if (isStart)
			//			{
			//				evt = FusionAudio.eEvent.SFX_Barbarian_Male_Dungeon_Start;
			//			}
			//			else
			//			{
			//				evt = FusionAudio.eEvent.SFX_Barbarian_Male_Dungeon_End;
			//			}
			//		}
			//		break;
			//	case eClass.Sorcerer:
			//		if (gender == eGender.Female)
			//		{
			//			if (isStart)
			//			{
			//				evt = FusionAudio.eEvent.SFX_Sorcerer_Female_Dungeon_Start;
			//			}
			//			else
			//			{
			//				evt = FusionAudio.eEvent.SFX_Sorcerer_Female_Dungeon_End;
			//			}
			//		}
			//		else
			//		{
			//			if (isStart)
			//			{
			//				evt = FusionAudio.eEvent.SFX_Sorcerer_Male_Dungeon_Start;
			//			}
			//			else
			//			{
			//				evt = FusionAudio.eEvent.SFX_Sorcerer_Male_Dungeon_End;
			//			}
			//		}
			//		break;
			//}

			FusionAudio.PostEvent(evt, PlayerManager.LocalPlayerGameObject(), true);
		}

		private void PlaySpecialAnimation(eSpecialAnimationVariety animationToPlay, CharacterComponent charTarget, bool doLoop)
		{
			/*
			if (eCharacterState.SpecialAnimation != charTarget.State)
			{
				charTarget.State = eCharacterState.SpecialAnimation;
			}
			SpecialAnimationStateHandler specialHandler = (charTarget.CurrentHandler as SpecialAnimationStateHandler);
			if (null != specialHandler)
			{
				specialHandler.SetVariety(animationToPlay, false);
			}
			*/
		}

		// check if we're still in the special animation state
		private bool IsSpecialAnimationPlaying(CharacterComponent charTarget)
		{
			return false; //null != charTarget && eCharacterState.SpecialAnimation == charTarget.State;
		}
	}
}