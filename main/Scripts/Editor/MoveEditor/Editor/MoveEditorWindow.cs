using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace MoveEditor
{
	public class MoveEditorWindow : EditorWindow
	{
		void OnEnable()
		{
			minSize = new Vector2(350, 250);

			InitCameraShakePresets(_cameraShakePresets);

			EditorPrefs.SetBool("MoveEdApplicationPlaying", Application.isPlaying);

			if (!Application.isPlaying)
			{
				InitMoveList();
				InitCharacterList();
			}

			SceneView.onSceneGUIDelegate -= OnSceneGUI;
			SceneView.onSceneGUIDelegate += OnSceneGUI;

			EditorApplication.playmodeStateChanged -= CheckApplicationState;
			EditorApplication.playmodeStateChanged += CheckApplicationState;

			SwitchPreviewMode(PreviewMode.Edit);
		}

		void OnSceneGUI(SceneView view)
		{
			if (_previewMode == PreviewMode.PlaybackPreview && !Application.isPlaying)
			{
				if (DynamicPointLightManager.Instance != null)
				{
					DynamicPointLightManager.Instance.Sim();
				}
			}
		}

		void OnDestroy()
		{
			SceneView.onSceneGUIDelegate -= OnSceneGUI;
			EditorApplication.playmodeStateChanged -= CheckApplicationState;

			CleanUpPreviewParticle();
			CleanUpDynamicLightPreview();
			CleanUpTrailLocator();
			CleanUpProjectileSpawnLocator();
			CleanUpHitOffsetLocator();
			CleanUpParticlePreviewTracks();
			CleanUpPropPreviewTracks();
			CleanUpTrailPreviewObjects();
			CleanUpDynamicLightPreviewObjects();
			CleanUpProjectilePreviewTracks();
			CleanUpEnvLightingPrewiewObjects();
			CleanUpPostFxPreviewTracks();
			CleanUpCameraShakePreviewTracks();
            CleanUpHitPreviewTracks();
			CleanUpCameraMotionPreviews();
			SetPreviewObject(null);
            SetHitPreviewObject(null);
		}

		void OnProjectChange()
		{
			MoveEditorUtils.CompileMoveDatabase();
			InitCharacterList();
			InitMoveList();

			if (_move == null)
			{
				SetAnimationClip(null);
				Repaint();
			}

			InitCameraShakePresets(_cameraShakePresets);
			InitCharacterList();
		}

		void OnHierarchyChange()
		{
			if (_previewGameObject == null)
			{
				if (_previewMode != PreviewMode.Edit)
				{
					SwitchPreviewMode(PreviewMode.Edit);
					Repaint();
				}
			}
		}

		void Update()
		{
			if (_previewMode == PreviewMode.PlaybackPreview)
			{
				DoPreviewPlayback();
			}

			if (_updateCameraPosition && _previewGameObject != null)
			{
				UpdateCamera();
			}

			// TJ: this is gross...we need to clean a bunch of stuff up if we're going into playmode, so we have to check on update
			if (EditorApplication.isPlayingOrWillChangePlaymode)
			{
				if (!Application.isPlaying)
				{
					ApplicationStateChanged();
				}
			}
		}

		private void OnGUI()
		{
			DrawMain();

			if (_showHitEventMetrics)
			{
				GUILayout.Space(5.0f);
				DrawHitEventMetrics();
			}

			GUILayout.Space(10.0f);

			if (_move != null && GetAnimationClip() != null)
			{
				_scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
				{
					DrawAnimationSpeedEvents();
					GUILayout.Space(10.0f);
					DrawCombatEvents();
					GUILayout.Space(10.0f);
					DrawGenericEvents();
					GUILayout.Space(10.0f);
					DrawHitEvents();
					GUILayout.Space(10.0f);
					DrawProjectileEvents();
					GUILayout.Space(10.0f);
					DrawParticleEvents();
					GUILayout.Space(10.0f);
					DrawTrailRendererEvents();
					GUILayout.Space(10.0f);
					DrawDynamicLightEvents();
					GUILayout.Space(10.0f);
					DrawCameraShakeEvents();
					//GUILayout.Space(10.0f);
					//DrawCameraSwingEvents();
					GUILayout.Space(10.0f);
					DrawCameraMotionEvents();
					GUILayout.Space(10.0f);
					DrawAudioEvents();
					//GUILayout.Space(10.0f);
					//DrawEnvironmentLightingEvents();
					//GUILayout.Space(10.0f);
					//DrawPostBloomEvents();
					//GUILayout.Space(10.0f);
					//DrawPostVignetteEvents();
					//GUILayout.Space(10.0f);
					//DrawPostWarpEvents();
					GUILayout.Space(10.0f);
					DrawRenderSettingsEvents();
					GUILayout.Space(10.0f);
				}
				GUILayout.EndScrollView();
			}

			if (_move != null)
				_move.SetPendingChanges();
		}

		private void DrawMain()
		{
			if (NGUIEditorTools.DrawHeader("Main Info", "move_editor_main"))
			{
				NGUIEditorTools.BeginContents();
				{
					// SELECT MOVE FROM LIST ----
					EditorGUILayout.BeginHorizontal();
					{
						int index = _move != null ? _moveNames.IndexOf(_move._path) : 0;
						index = Mathf.Max(index, 0);

						int testValue = EditorGUILayout.Popup("Move (List)", index, _moveShowNames);

						if (index != testValue)
						{
							index = testValue;
							if (index != 0)
							{
								GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(_movePaths[index], typeof(GameObject));
								//SetMove(prefab.GetComponent<Move>(), _movePaths[index], false);
								Move theMove = prefab.GetComponent<Move>();
								SetMove(theMove, _movePaths[index], false);
								if (theMove != null && theMove._animationObject.Value != null)
								{
									// auto-select dude
									OnCharacterSelected(theMove._animationObject.Value.name);
								}
							}
							else
							{
								SetMove(null, null, false);
							}
						}

						if (GUILayout.Button("O", GUILayout.Width(25)))
						{
							ListViewWindow.Open("Moves", _moveNames.ToArray(), OnMoveSelected);
						}

						if (GUILayout.Button("X", GUILayout.Width(25)))
						{
							SetMove(null, null, false);
						}
					}
					EditorGUILayout.EndHorizontal();

					// SELECT CHARACTER FROM LIST -----
					EditorGUILayout.BeginHorizontal();
					{
						//if (_previewGameObject == null && _move != null && _move._animationObject != null)
						//{
						//    OnCharacterSelected(_move._animationObject.name);
						//}

						int index = _previewGameObject != null ? _characterNames.IndexOf(_previewGameObject.name) : 0;
						index = Mathf.Max(index, 0);

						int testValue = EditorGUILayout.Popup("Character (List)", index, _characterShowNames);
						if (index != testValue)
						{
							index = testValue;
							if (index != 0)
							{
								GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(_characterPaths[index], typeof(GameObject));
								SetPreviewObject(prefab);
							}
							else
							{
								SetPreviewObject(null);
							}
						}
                        if (GUILayout.Button("O", GUILayout.Width(25)))
						{
							ListViewWindow.Open("Characters", _characterNames.ToArray(), OnCharacterSelected);
						}

						if (GUILayout.Button("X", GUILayout.Width(25)))
						{
							SetPreviewObject(null);
						}
                    }
					EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    {
                        int hitIndex = _previewHitGameObject != null ? _characterNames.IndexOf(_previewHitGameObject.name) : 0;
                        hitIndex = Mathf.Max(hitIndex, 0);

                        int testHitValue = EditorGUILayout.Popup("Hit Character (List)", hitIndex, _characterShowNames);
                        if (hitIndex != testHitValue)
                        {
                            hitIndex = testHitValue;
                            if (hitIndex != 0)
                            {
                                GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(_characterPaths[hitIndex], typeof(GameObject));
                                SetHitPreviewObject(prefab);
                            }
                            else
                            {
                                SetHitPreviewObject(null);
                            }
                        }

                        if (GUILayout.Button("O", GUILayout.Width(25)))
                        {
                            ListViewWindow.Open("Characters", _characterNames.ToArray(), OnHitCharacterSelected);
                        }

                        if (GUILayout.Button("X", GUILayout.Width(25)))
                        {
                            SetHitPreviewObject(null);
                        }
                    }
                    EditorGUILayout.EndHorizontal();


					if (_move != null)
					{
						EditorGUILayout.BeginHorizontal();
						_move._animationClip = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip", _move._animationClip, typeof(AnimationClip), false);
						_move._animationObject.Value = (GameObject)EditorGUILayout.ObjectField("Animation Object", _move._animationObject.Value, typeof(GameObject), false);
						EditorGUILayout.EndHorizontal();

						EditorGUILayout.BeginHorizontal();
						int selectedIndex = (int)_move._moveState;
						selectedIndex = EditorGUILayout.Popup("Move State", selectedIndex, System.Enum.GetNames(typeof(MoveController.CombatantMoveState)));
						_move._moveState = (MoveController.CombatantMoveState)selectedIndex;

						_move._motionTransform = EditorGUILayout.TextField("Rig Root Node", _move._motionTransform);
						EditorGUILayout.EndHorizontal();
					}

					if (GetAnimationClip() != null)
					{
						// ANIMATION SPEED -----
						EditorGUILayout.BeginHorizontal();
						{
							_move._animationSpeedEvent._floatParameter = EditorGUILayout.FloatField("Animation Speed", _move._animationSpeedEvent._floatParameter);
							_move._animationSpeedEvent.asep._plusMinus = EditorGUILayout.FloatField("Animation Speed +/-", _move._animationSpeedEvent.asep._plusMinus);
							//_move._animationSpeedEvent._slideToOpponent = EditorGUILayout.Toggle("Slide to Opponent", _move._animationSpeedEvent._slideToOpponent);
						}
						EditorGUILayout.EndHorizontal();

						if(_move.name.Contains("SKILL_") || _move._moveState == MoveController.CombatantMoveState.kAttackTarget)
						{
							EditorGUILayout.BeginVertical();
							{
								_move._actionPosition = (Move.eSkillActionPositionType)EditorGUILayout.EnumPopup("Skill Action Point", _move._actionPosition);

								EditorGUILayout.BeginHorizontal();
								switch (_move._actionPosition)
								{
									case Move.eSkillActionPositionType.TARGET:
										_move._targetPosition = (Move.eSkillTargetPositionType)EditorGUILayout.EnumPopup("Skill Target Point", _move._targetPosition);
										switch (_move._targetPosition)
										{
											case Move.eSkillTargetPositionType.TRANSFORM:
												_move._TargetLocationNode = EditorGUILayout.TextField("Target Node", _move._TargetLocationNode);
												break;
											default:
												_move._TargetLocationNode = "";
												break;
										}
										_move._attackRadius = EditorGUILayout.FloatField("Target Distance", _move._attackRadius);
										break;
									case Move.eSkillActionPositionType.MIDLINE:
										_move._targetPosition = (Move.eSkillTargetPositionType)EditorGUILayout.EnumPopup("Skill Target Point", _move._targetPosition);
										switch (_move._targetPosition)
										{
											case Move.eSkillTargetPositionType.TRANSFORM:
												_move._TargetLocationNode = EditorGUILayout.TextField("Target Node", _move._TargetLocationNode);
												break;
											default:
												_move._TargetLocationNode = "";
												break;
										}
										_move._attackRadius = EditorGUILayout.FloatField("Z Offset", _move._attackRadius);
										break;
									case Move.eSkillActionPositionType.MIDPOINT:
										_move._targetPosition = Move.eSkillTargetPositionType.NONE;
										_move._TargetLocationNode = "";
										_move._attackRadius = EditorGUILayout.FloatField("Z Offset", _move._attackRadius);
										_move._AttackLocationNode = "";
										break;
									case Move.eSkillActionPositionType.TRANSFORM:
										_move._targetPosition = Move.eSkillTargetPositionType.NONE;
										_move._TargetLocationNode = "";
										_move._AttackLocationNode = EditorGUILayout.TextField("Attack Node", _move._AttackLocationNode);
										_move._attackRadius = EditorGUILayout.FloatField("Z Offset", _move._attackRadius);
										break;
									case Move.eSkillActionPositionType.ORIGINAL:
										_move._targetPosition = Move.eSkillTargetPositionType.NONE;
										_move._TargetLocationNode = "";
										_move._attackRadius = 0f;
										_move._AttackLocationNode = "";
										break;
								}
								EditorGUILayout.EndHorizontal();

								_move._cinematicSkill = EditorGUILayout.Toggle("Cinematic Skill", _move._cinematicSkill);
							}
							EditorGUILayout.EndVertical();
						}

						// FRAME SLIDER -----
						EditorGUILayout.BeginHorizontal();
						{
							float frame;

							if (_smoothFrames)
								frame = EditorGUILayout.Slider("Frame", _currentFrame, 0, _numFrames);
							else
								frame = (int)EditorGUILayout.Slider("Frame", _currentFrame, 0, _numFrames);

							if (!Mathf.Approximately(frame, _currentFrame))
							{
								if (_previewMode != PreviewMode.PlaybackPreview)
								{
									_currentFrame = frame;
									ScrubAnimation();
								}
							}

							if (GUILayout.Button(_smoothFrames ? "S" : "K", GUILayout.Width(20)))
							{
								_smoothFrames = !_smoothFrames;
							}

                            _addPreviewTime = EditorGUILayout.FloatField("AddPlayBackPreviewTime", _addPreviewTime);
                        }
						EditorGUILayout.EndHorizontal();
                        //EditorGUILayout.BeginHorizontal();
                        //{
                        //    //_addPreviewTime = EditorGUILayout.FloatField("AddPlayBackPreviewTime",_addPreviewTime);
                        //}
                        //EditorGUILayout.EndHorizontal();
                        // PLAYBACK SPEED -----
                        if (_previewMode == PreviewMode.PlaybackPreview)
						{
							float speed = EditorGUILayout.Slider("Playback Speed", _playbackSpeed, 0, 2.0f);
							if (!Mathf.Approximately(speed, _playbackSpeed))
							{
								speed *= 10.0f;
								speed = Mathf.Round(speed);
								speed /= 10.0f;
								_playbackSpeed = speed;
							}
						}

						NGUIEditorTools.BeginContents();
						{
							bool showSettings = EditorGUILayout.Foldout(EditorPrefs.GetBool("mew_settings", false), "Settings");
							EditorPrefs.SetBool("mew_settings", showSettings);

							if (showSettings)
							{
								// FLIPPED TOGGLE -----
								if (_previewGameObject != null)
								{
									if (_previewMode == PreviewMode.Edit)
									{
										bool flipped = EditorGUILayout.Toggle("Flip Character", _flipped);

										if (flipped != _flipped)
										{
											FlipCharacter(_previewGameObject, flipped);
										}
									}
								}

								_showHitEventMetrics = EditorGUILayout.Toggle("Show Hit Metrics", _showHitEventMetrics);
								_updateCameraPosition = EditorGUILayout.Toggle("Update Camera", _updateCameraPosition);
								if (_updateCameraPosition)
								{
									_previewCameraOffset = EditorGUILayout.Vector3Field("Camera Offset", _previewCameraOffset);
								}
							}
						}
						NGUIEditorTools.EndContents();

						// SET EVENTS BUTTON -----
						GUI.color = Color.yellow;
						if (GUILayout.Button("Build Move"))
						{
							SwitchPreviewMode(PreviewMode.Edit);

							MoveEditorUtils.BuildMove(_move);
							ResetAnimatorComponent();
						}
						GUI.color = Color.white;

						if (_previewGameObject != null)
						{
							// CHANGE PREVIEW MODE (EDIT, SCRUB, OR PLAYBACK) -----
							switch (_previewMode)
							{
								case PreviewMode.Edit:
									EditorGUILayout.BeginHorizontal();
									{
										if (GUILayout.Button("Scrub Preview"))
											SwitchPreviewMode(PreviewMode.ScrubPreview);

										if (GUILayout.Button("Playback Preview"))
											SwitchPreviewMode(PreviewMode.PlaybackPreview);
									}
									EditorGUILayout.EndHorizontal();
									break;
								default:
									GUI.color = Color.red;
								    if (GUILayout.Button("Stop Preview"))
								    {
								        FusionAudio.ClearAudioPreviewers();
								        SwitchPreviewMode(PreviewMode.Edit);
								    }

								    GUI.color = Color.white;
									break;
							}
						}
					}
					NGUIEditorTools.EndContents();
				}
			}
		}

		private void OnMoveSelected(string name)
		{
			int index = _moveNames.IndexOf(name);
			GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(_movePaths[index], typeof(GameObject));
			Move theMove = prefab.GetComponent<Move>();
			SetMove(theMove, null, false);
			if (theMove != null && theMove._animationObject.Value != null && Application.isPlaying == false)
			{
				// auto-select dude
				OnCharacterSelected(theMove._animationObject.Value.name);
			}
		}

		private void OnCharacterSelected(string name)
		{
			int index = _characterNames.IndexOf(name);
			GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(_characterPaths[index], typeof(GameObject));
			SetPreviewObject(prefab);
		}

        private void OnHitCharacterSelected(string name)
        {
            int index = _characterNames.IndexOf(name);
            GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(_characterPaths[index], typeof(GameObject));
            SetHitPreviewObject(prefab);
        }

        private void ShowFrameStats()
		{
			Debug.Log("Frame Stats:");

			float fps = 30.0f * _move._animationSpeedEvent._floatParameter;

			GenericEventInfo openAttackWindowEvent = _move._genericEvents.Find(o => o._functionName == "OnOpenAttackInputWindow");
			float recoveryTime = openAttackWindowEvent._frame / fps;

			//for (int i = 0; i < _move._hitEvents.Count; ++i)
			//{
				//HitEventInfo hitEventInfo = _move._hitEvents[i];
				//Debug.Log("Hit Event :" + i);

				//float hitTime = hitEventInfo._frame / fps;
				//float opponentHitRecoveryTime = hitTime + hitEventInfo._damageProperties._hitStun;
				//float opponentBlockRecoveryTime = hitTime + hitEventInfo._damageProperties._blockStun;
				//Debug.Log("Hit Frame: "+ hitTime );
				//Debug.Log("Frame Adv (hit): "+ (opponentHitRecoveryTime - recoveryTime));
				//Debug.Log("Frame Adv (block) : "+ (opponentBlockRecoveryTime - recoveryTime));
			//}
			Debug.Log("Recovery: " + recoveryTime);
		}

		private void DrawHitEventMetrics()
		{
			if (_move != null && GetAnimationClip() != null)
			{
				if (NGUIEditorTools.DrawHeader("Hit Event Metrics"))
				{
					NGUIEditorTools.BeginContents();
					{
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.LabelField("Anim Length (s)", GUILayout.Width(100));
							EditorGUILayout.LabelField((_move.AdjustedLength).ToString("F2"), GUILayout.Width(50));
						}
						EditorGUILayout.EndHorizontal();

						if (_move._hitEvents.Count > 0)
						{
							// OrderBy in Editor is ok
							HitEventInfo[] sortedHitEvents = _move._hitEvents.OrderBy(evt => evt._frame).ToArray();

							float startupTime = MoveEditorUtils.GetRealTimeFromFrame(sortedHitEvents[0]._frame, _move.NumFrames, GetAnimationClip().length, _move.Speed);
							float lastHitTime = MoveEditorUtils.GetRealTimeFromFrame(sortedHitEvents.Last()._frame, _move.NumFrames, GetAnimationClip().length, _move.Speed);
							float recoveryFrameTime = -1;
							float recoveryTime = -1;
							float windowTime = -1;
							float frameAdvantageOnHit = -1.0f;
							float frameAdvantageOnBlock = -1.0f;

							GenericEventInfo windowOpenEvent = _move._genericEvents.Find(evt => evt._functionName == "OnOpenAttackInputWindow");
							if (windowOpenEvent != null)
							{
								recoveryFrameTime = MoveEditorUtils.GetRealTimeFromFrame(windowOpenEvent._frame, _move.NumFrames, GetAnimationClip().length, _move.Speed);
							}
							else
							{
								recoveryFrameTime = _move.AdjustedLength;
							}

							windowTime = _move.AdjustedLength - recoveryFrameTime;

							recoveryTime = recoveryFrameTime - lastHitTime;
							//frameAdvantageOnHit = sortedHitEvents.Last()._damageProperties._hitStun - recoveryTime;
							//frameAdvantageOnBlock = sortedHitEvents.Last()._damageProperties._blockStun - recoveryTime;

							EditorGUILayout.BeginHorizontal();
							{
								EditorGUILayout.LabelField("Startup (s)", GUILayout.Width(100));
								EditorGUILayout.LabelField(startupTime.ToString("F3"), GUILayout.Width(50));
								//EditorGUILayout.LabelField("|", GUILayout.Width(10));
								EditorGUILayout.LabelField("Recovery (s)", GUILayout.Width(100));
								EditorGUILayout.LabelField(recoveryTime.ToString("F3"), GUILayout.Width(50));
							}
							EditorGUILayout.EndHorizontal();

							EditorGUILayout.BeginHorizontal();
							{
								EditorGUILayout.LabelField("Window (s)", GUILayout.Width(100));
								EditorGUILayout.LabelField(windowTime.ToString("F3"), GUILayout.Width(50));
							}
							EditorGUILayout.EndHorizontal();

							EditorGUILayout.BeginHorizontal();
							{
								EditorGUILayout.LabelField("Hit Adv (s)", GUILayout.Width(100));
								EditorGUILayout.LabelField(frameAdvantageOnHit.ToString("F3"), GUILayout.Width(50));
								//EditorGUILayout.LabelField("|", GUILayout.Width(10));
								EditorGUILayout.LabelField("Block Adv (s)", GUILayout.Width(100));
								EditorGUILayout.LabelField(frameAdvantageOnBlock.ToString("F3"), GUILayout.Width(50));
							}
							EditorGUILayout.EndHorizontal();

							float normalizedTotalDamage = 0.0f;
							float normalizedTotalMana = 0.0f;
							//foreach (HitEventInfo hitEventInfo in sortedHitEvents)
							//{
								//normalizedTotalDamage += hitEventInfo._damageProperties._damage;
								//normalizedTotalMana += hitEventInfo._damageProperties._mana;
							//}

							EditorGUILayout.BeginHorizontal();
							{
								GUI.color = Mathf.Approximately(normalizedTotalDamage, 1) ? Color.white : Color.red;
								EditorGUILayout.LabelField("Total Dmg (%)", GUILayout.Width(100));
								EditorGUILayout.LabelField(normalizedTotalDamage.ToString("F3"), GUILayout.Width(50));
								GUI.color = Mathf.Approximately(normalizedTotalMana, 1) ? Color.white : Color.red;
								EditorGUILayout.LabelField("Total Mana (%)", GUILayout.Width(100));
								EditorGUILayout.LabelField(normalizedTotalMana.ToString("F3"), GUILayout.Width(50));
								GUI.color = Color.white;
							}
							EditorGUILayout.EndHorizontal();
						}
						NGUIEditorTools.EndContents();
					}
				}
			}
		}

		#region Draw Event Group Functions

		private void DrawAnimationSpeedEvents()
		{
			if (NGUIEditorTools.DrawHeader(string.Format("Animation Speed Events ({0})", _move._animationSpeedEvents.Count), "move_editor_speedss"))
			{
				NGUIEditorTools.BeginContents();
				{
					bool delete = false;

					for (int i = 0; i < _move._animationSpeedEvents.Count; i++)
					{
						if (i > 0)
							GUILayout.Space(5);

						NGUIEditorTools.BeginContents();
						{
							DrawAnimationSpeedEvent(i, ref delete);
						}
						NGUIEditorTools.EndContents();

						if (delete)
						{
							_move._animationSpeedEvents.RemoveAt(i);
							break;
						}
					}

					GUILayout.Space(5);

					if (GUILayout.Button("Add Animation Speed Event"))
					{
						_move._animationSpeedEvents.Add(new AnimationSpeedEventInfo());
					}
				}
				NGUIEditorTools.EndContents();
			}
		}

		private void DrawCombatEvents()
		{
			if (NGUIEditorTools.DrawHeader(string.Format("Combat Events ({0})", _move._combatEvents.Count), "move_editor_combats"))
			{
				NGUIEditorTools.BeginContents();
				{
					bool delete = false;

					for (int i = 0; i < _move._combatEvents.Count; i++)
					{
						if (i > 0)
							GUILayout.Space(5);

						NGUIEditorTools.BeginContents();
						{
							DrawCombatEvent(i, ref delete);
						}
						NGUIEditorTools.EndContents();

						if (delete)
						{
							_move._combatEvents.RemoveAt(i);
							break;
						}
					}

					GUILayout.Space(5);

					if (GUILayout.Button("Add Combat Event"))
					{
						_move._combatEvents.Add(new CombatEventInfo());
					}
				}
				NGUIEditorTools.EndContents();
			}
		}
		private void DrawGenericEvents()
		{
			if (NGUIEditorTools.DrawHeader(string.Format("Generic Events ({0})", _move._genericEvents.Count), "move_editor_generics"))
			{
				NGUIEditorTools.BeginContents();
				{
					bool delete = false;

					for (int i = 0; i < _move._genericEvents.Count; i++)
					{
						if (i > 0)
							GUILayout.Space(5);

						NGUIEditorTools.BeginContents();
						{
							DrawGenericEvent(i, ref delete);
						}
						NGUIEditorTools.EndContents();

						if (delete)
						{
							_move._genericEvents.RemoveAt(i);
							break;
						}
					}

					GUILayout.Space(5);

					if (GUILayout.Button("Add Generic Event"))
					{
						_move._genericEvents.Add(new GenericEventInfo());
					}
				}
				NGUIEditorTools.EndContents();
			}
		}

		private void DrawHitEvents()
		{
			if (NGUIEditorTools.DrawHeader(string.Format("Hit Events ({0})", _move._hitEvents.Count), "move_editor_hits"))
			{
				NGUIEditorTools.BeginContents();
				{
					bool delete = false;
					for (int i = 0; i < _move._hitEvents.Count; i++)
					{
						if (i > 0)
							GUILayout.Space(5);

						NGUIEditorTools.BeginContents();
						{
							DrawHitEvent(i, ref delete);
						}
						NGUIEditorTools.EndContents();

						if (delete)
						{
							_move._hitEvents.RemoveAt(i);
							break;
						}
					}

					GUILayout.Space(5);

					if (GUILayout.Button("Add Hit Event"))
					{
						_move._hitEvents.Add(new HitEventInfo());
					}
				}
				NGUIEditorTools.EndContents();
			}
		}

		private void DrawParticleEvents()
		{
			if (NGUIEditorTools.DrawHeader(string.Format("Particle Events ({0})", _move._particleEvents.Count), "move_editor_particles"))
			{
				NGUIEditorTools.BeginContents();
				{
					bool delete = false;

					for (int i = 0; i < _move._particleEvents.Count; i++)
					{
						if (i > 0)
							GUILayout.Space(5);

						NGUIEditorTools.BeginContents();
						{
							DrawParticleEvent(i, ref delete);
						}
						NGUIEditorTools.EndContents();

						if (delete)
						{
							_move._particleEvents.RemoveAt(i);
							break;
						}
					}

					GUILayout.Space(5);

					if (GUILayout.Button("Add Particle Event"))
					{
						_move._particleEvents.Add(new ParticleEventInfo());
					}
				}
				NGUIEditorTools.EndContents();
			}
		}

		private void DrawTrailRendererEvents()
		{
			if (NGUIEditorTools.DrawHeader(string.Format("Trail Renderer Events ({0})", _move._trailRendererEvents.Count), "move_editor_trails"))
			{
				NGUIEditorTools.BeginContents();
				{
					bool delete = false;

					for (int i = 0; i < _move._trailRendererEvents.Count; i++)
					{
						if (i > 0)
							GUILayout.Space(5);

						NGUIEditorTools.BeginContents();
						{
							DrawTrailRendererEvent(i, ref delete);
						}
						NGUIEditorTools.EndContents();

						if (delete)
						{
							_move._trailRendererEvents.RemoveAt(i);
							break;
						}
					}

					GUILayout.Space(5);

					if (GUILayout.Button("Add Trail Renderer Event"))
					{
						_move._trailRendererEvents.Add(new TrailRendererEventInfo());
					}
				}
				NGUIEditorTools.EndContents();
			}
		}

		private void DrawDynamicLightEvents()
		{
			if (NGUIEditorTools.DrawHeader(string.Format("Dynamic Light Events ({0})", _move._dynamicLightEvents.Count), "move_editor_dynamiclights"))
			{
				NGUIEditorTools.BeginContents();
				{
					bool delete = false;

					for (int i = 0; i < _move._dynamicLightEvents.Count; i++)
					{
						if (i > 0)
							GUILayout.Space(5);

						NGUIEditorTools.BeginContents();
						{
							DrawDynamicLightEvent(i, ref delete);
						}
						NGUIEditorTools.EndContents();

						if (delete)
						{
							_move._dynamicLightEvents.RemoveAt(i);
							break;
						}
					}

					GUILayout.Space(5);

					if (GUILayout.Button("Add Dynamic Light Event"))
					{
						_move._dynamicLightEvents.Add(new DynamicLightEventInfo());
					}
				}
				NGUIEditorTools.EndContents();
			}
		}

        float sliderNum = 0.5f;
        private void DrawCameraShakeEvents()
		{
			if (NGUIEditorTools.DrawHeader(string.Format("Camera Shake Events ({0})", _move._cameraShakeEvents.Count), "move_editor_camshakes"))
			{
				NGUIEditorTools.BeginContents();
				{
					bool delete = false;

					for (int i = 0; i < _move._cameraShakeEvents.Count; i++)
					{
						if (i > 0)
							GUILayout.Space(5);

						NGUIEditorTools.BeginContents();
						{
							DrawCameraShakeEvent(i, ref delete);
						}
						NGUIEditorTools.EndContents();

						if (delete)
						{
							_move._cameraShakeEvents.RemoveAt(i);
							break;
						}
					}

                    GUILayout.Space(5);

					if (GUILayout.Button("Add Camera Shake Event"))
					{
						_move._cameraShakeEvents.Add(new CameraShakeEventInfo());
					}
				}
				NGUIEditorTools.EndContents();
			}
		}

		private void DrawCameraSwingEvents()
		{
			if (NGUIEditorTools.DrawHeader(string.Format("Camera Swing Events ({0})", _move._cameraSwingEvents.Count), "move_editor_camswings"))
			{
				NGUIEditorTools.BeginContents();
				{
					bool delete = false;

					for (int i = 0; i < _move._cameraSwingEvents.Count; i++)
					{
						if (i > 0)
							GUILayout.Space(5);

						NGUIEditorTools.BeginContents();
						{
							DrawCameraSwingEvent(i, ref delete);
						}
						NGUIEditorTools.EndContents();

						if (delete)
						{
							_move._cameraSwingEvents.RemoveAt(i);
							break;
						}
					}

					GUILayout.Space(5);

					if (GUILayout.Button("Add Camera Swing Event"))
					{
						_move._cameraSwingEvents.Add(new CameraSwingEventInfo());
					}
				}
				NGUIEditorTools.EndContents();
			}
		}

		private void DrawCameraMotionEvents()
		{
			if (NGUIEditorTools.DrawHeader(string.Format("Camera Motion Events ({0})", _move._cameraMotionEvents.Count), "move_editor_cameramotions"))
			{
				NGUIEditorTools.BeginContents();
				{
					bool delete = false;
					for (int i = 0; i < _move._cameraMotionEvents.Count; i++)
					{
						if (i > 0)
							GUILayout.Space(5);

						NGUIEditorTools.BeginContents();
						{
							DrawCameraMotionEvent(i, ref delete);
						}
						NGUIEditorTools.EndContents();

						if (delete)
						{
							_move._cameraMotionEvents.RemoveAt(i);
							break;
						}
					}

					GUILayout.Space(5);

					if (GUILayout.Button("Add Camera Motion Event"))
					{
						_move._cameraMotionEvents.Add(new CameraMotionEventInfo());
					}
				}
				NGUIEditorTools.EndContents();
			}
		}

		private void DrawAudioEvents()
		{
			if (NGUIEditorTools.DrawHeader(string.Format("Audio Events ({0})", _move._audioEvents.Count), "move_editor_audio"))
			{
				NGUIEditorTools.BeginContents();
				{
					bool delete = false;

					for (int i = 0; i < _move._audioEvents.Count; i++)
					{
						if (i > 0)
							GUILayout.Space(5);

						NGUIEditorTools.BeginContents();
						{
							DrawAudioEvent(i, ref delete);
						}
						NGUIEditorTools.EndContents();

						if (delete)
						{
							_move._audioEvents.RemoveAt(i);
							break;
						}
					}

					GUILayout.Space(5);

					if (GUILayout.Button("Add Audio Event"))
					{
						_move._audioEvents.Add(new AudioEventInfo());
					}
				}
				NGUIEditorTools.EndContents();
			}
		}

		private void DrawEnvironmentLightingEvents()
		{
			if (NGUIEditorTools.DrawHeader(string.Format("Environment Lighting Events ({0})", _move._environmentLightingEvents.Count), "move_editor_envlighting"))
			{
				NGUIEditorTools.BeginContents();
				{
					bool delete = false;

					for (int i = 0; i < _move._environmentLightingEvents.Count; i++)
					{
						if (i > 0)
							GUILayout.Space(5);

						NGUIEditorTools.BeginContents();
						{
							DrawEnvironmentLightingEvent(i, ref delete);
						}
						NGUIEditorTools.EndContents();

						if (delete)
						{
							_move._environmentLightingEvents.RemoveAt(i);
							break;
						}
					}

					GUILayout.Space(5);

					if (GUILayout.Button("Add Environment Lighting Event"))
					{
						_move._environmentLightingEvents.Add(new EnvironmentLightingEventInfo());
					}
				}
				NGUIEditorTools.EndContents();
			}
		}

		private void DrawProjectileEvents()
		{
			if (NGUIEditorTools.DrawHeader(string.Format("Projectile Events ({0})", _move._projectileEvents.Count), "move_editor_projectiles"))
			{
				NGUIEditorTools.BeginContents();
				{
					bool delete = false;

					for (int i = 0; i < _move._projectileEvents.Count; ++i)
					{
						if (i > 0)
						{
							GUILayout.Space(5);
						}

						NGUIEditorTools.BeginContents();
						{
							DrawProjectileEvent(i, ref delete);
						}
						NGUIEditorTools.EndContents();

						if (delete)
						{
							_move._projectileEvents.RemoveAt(i);
							break;
						}
					}

					GUILayout.Space(5);

					if (GUILayout.Button("Add Projectile Event"))
					{
						_move._projectileEvents.Add(new ProjectileEventInfo());
					}
				}
				NGUIEditorTools.EndContents();
			}
		}

		private void DrawPostBloomEvents()
		{
			if (NGUIEditorTools.DrawHeader(string.Format("Post FX: Bloom Events ({0})", _move._postBloomEvents.Count), "move_editor_bloom"))
			{
				NGUIEditorTools.BeginContents();
				{
					bool delete = false;

					for (int i = 0; i < _move._postBloomEvents.Count; ++i)
					{
						if (i > 0)
						{
							GUILayout.Space(5);
						}

						NGUIEditorTools.BeginContents();
						{
							DrawPostBloomEvent(i, ref delete);
						}
						NGUIEditorTools.EndContents();

						if (delete)
						{
							_move._postBloomEvents.RemoveAt(i);
							break;
						}
					}

					GUILayout.Space(5);

					if (GUILayout.Button("Add Bloom Event"))
					{
						_move._postBloomEvents.Add(new PostBloomEventInfo());
					}
				}
				NGUIEditorTools.EndContents();
			}
		}

		private void DrawPostVignetteEvents()
		{
			if (NGUIEditorTools.DrawHeader(string.Format("Post FX: Vignette Events ({0})", _move._postVignetteEvents.Count), "move_editor_vig"))
			{
				NGUIEditorTools.BeginContents();
				{
					bool delete = false;

					for (int i = 0; i < _move._postVignetteEvents.Count; ++i)
					{
						if (i > 0)
						{
							GUILayout.Space(5);
						}

						NGUIEditorTools.BeginContents();
						{
							DrawPostVignetteEvent(i, ref delete);
						}
						NGUIEditorTools.EndContents();

						if (delete)
						{
							_move._postVignetteEvents.RemoveAt(i);
							break;
						}
					}

					GUILayout.Space(5);

					if (GUILayout.Button("Add Vignette Event"))
					{
						_move._postVignetteEvents.Add(new PostVignetteEventInfo());
					}
				}
				NGUIEditorTools.EndContents();
			}
		}

		private void DrawPostWarpEvents()
		{
			if (NGUIEditorTools.DrawHeader(string.Format("Post FX: Warp Events ({0})", _move._postWarpEvents.Count), "move_editor_warp"))
			{
				NGUIEditorTools.BeginContents();
				{
					bool delete = false;

					for (int i = 0; i < _move._postWarpEvents.Count; ++i)
					{
						if (i > 0)
						{
							GUILayout.Space(5);
						}

						NGUIEditorTools.BeginContents();
						{
							DrawPostWarpEvent(i, ref delete);
						}
						NGUIEditorTools.EndContents();

						if (delete)
						{
							_move._postWarpEvents.RemoveAt(i);
							break;
						}
					}

					GUILayout.Space(5);

					if (GUILayout.Button("Add Warp Event"))
					{
						_move._postWarpEvents.Add(new PostWarpEventInfo());
					}
				}
				NGUIEditorTools.EndContents();
			}
		}

		private void DrawRenderSettingsEvents()
		{
			if (NGUIEditorTools.DrawHeader(string.Format("Post FX: Render Settings ({0})", _move._switchRenderSettingsEvents.Count), "move_editor_warp"))
			{
				NGUIEditorTools.BeginContents();
				{
					bool delete = false;

					for (int i = 0; i < _move._switchRenderSettingsEvents.Count; ++i)
					{
						if (i > 0)
						{
							GUILayout.Space(5);
						}

						NGUIEditorTools.BeginContents();
						{
							DrawRenderSettingsEvent(i, ref delete);
						}
						NGUIEditorTools.EndContents();

						if (delete)
						{
							_move._switchRenderSettingsEvents.RemoveAt(i);
							break;
						}
					}

					GUILayout.Space(5);

					if (GUILayout.Button("Add Render Setting Event"))
					{
						_move._switchRenderSettingsEvents.Add(new SwitchRenderSettingsEventInfo());
					}
				}
				NGUIEditorTools.EndContents();
			}
		}
		#endregion

		#region Draw Individual Event Methods
		private void DrawAnimationSpeedEvent(int index, ref bool delete)
		{
			if (DrawTitleField("Animation Speed Event", index, "mew_ce_" + index.ToString(), ref delete))
			{
				AnimationSpeedEventInfo e = _move._animationSpeedEvents[index];

				DrawBaseFields(ref e._frame, ref e._required, ref e._playOnce, ref e._persistent, false);

				e._floatParameter = EditorGUILayout.FloatField("Animation Speed", e._floatParameter);
				e.asep._plusMinus = EditorGUILayout.FloatField("Animation Speed +/-", e.asep._plusMinus);

				_move._animationSpeedEvents[index] = e;
			}
		}

		private void DrawCombatEvent(int index, ref bool delete)
		{
			if (DrawTitleField("Combat Event", index, "mew_ce_" + index.ToString(), ref delete))
			{
				CombatEventInfo e = _move._combatEvents[index];

				DrawBaseFields(ref e._frame, ref e._required, ref e._playOnce, ref e._persistent, false);

				int selectedIndex = (int)e.eventType;
				//EB.Debug.Log (e.eventType.ToString() + " - " + selectedIndex);
				if (DrawPopupField(ref selectedIndex, "Event Type", System.Enum.GetName(typeof(CombatEventInfo.CombatEventType), e.eventType), System.Enum.GetNames(typeof(CombatEventInfo.CombatEventType))))
				{
				}
				e.eventType = (CombatEventInfo.CombatEventType)(System.Enum.GetValues(typeof(CombatEventInfo.CombatEventType)).GetValue(selectedIndex));
				if (e.eventType == CombatEventInfo.CombatEventType.PlayTargetHitReaction)
				{
					EditorGUILayout.TextArea("0=right, 90=forward, 180=left, left to right or right to left are both supported");
					e.hitRxnProps.framesToPlay = EditorGUILayout.FloatField("Duration (frames)", e.hitRxnProps.framesToPlay);
					e.hitRxnProps.startAngle = EditorGUILayout.FloatField("Starting Angle (degrees)", e.hitRxnProps.startAngle);
					e.hitRxnProps.endAngle = EditorGUILayout.FloatField("Ending Angle (degrees)", e.hitRxnProps.endAngle);

					int selectedIndex3 = (int)e.eventType;
					DrawPopupField(ref selectedIndex3, "hitActionType", System.Enum.GetName(typeof(PlayHitReactionProperties.eReactionType), e.hitRxnProps.defaultReaction), System.Enum.GetNames(typeof(PlayHitReactionProperties.eReactionType)));
					e.hitRxnProps.defaultReaction = (PlayHitReactionProperties.eReactionType)(System.Enum.GetValues(typeof(PlayHitReactionProperties.eReactionType)).GetValue(selectedIndex3));
					DrawPopupField(ref selectedIndex3, "hitActionPlayType", System.Enum.GetName(typeof(PlayHitReactionProperties.ePlayPriorityType), e.hitRxnProps.playPriority), System.Enum.GetNames(typeof(PlayHitReactionProperties.ePlayPriorityType)));
					e.hitRxnProps.playPriority = (PlayHitReactionProperties.ePlayPriorityType)(System.Enum.GetValues(typeof(PlayHitReactionProperties.ePlayPriorityType)).GetValue(selectedIndex3));
					e.hitRxnProps.weight = EditorGUILayout.IntField("weight", e.hitRxnProps.weight);
				}
				if (e.eventType == CombatEventInfo.CombatEventType.TargetBeenStonedReaction)
				{
					e.buffEventProps._lastFrame = EditorGUILayout.FloatField("Duration (seconds)", e.buffEventProps._lastFrame);
				}
				if (e.eventType == CombatEventInfo.CombatEventType.TimeScale)
				{
					e.timeScaleProps.timeScaleCurve = EditorGUILayout.CurveField("Time Scale Curve", e.timeScaleProps.timeScaleCurve);
				}
				if (e.eventType == CombatEventInfo.CombatEventType.Pause)
				{
					int selectedIndex2 = (int)e.eventType;
					e.pauseProps.fPauseTime = EditorGUILayout.FloatField("Pause Time", e.pauseProps.fPauseTime);
					DrawPopupField(ref selectedIndex2, "Type", System.Enum.GetName(typeof(PauseProperties.PAUSE_TYPE), e.pauseProps.type), System.Enum.GetNames(typeof(PauseProperties.PAUSE_TYPE)));
					e.pauseProps.type = (PauseProperties.PAUSE_TYPE)(System.Enum.GetValues(typeof(PauseProperties.PAUSE_TYPE)).GetValue(selectedIndex2));
				}

				_move._combatEvents[index] = e;
			}
		}
		private void DrawGenericEvent(int index, ref bool delete)
		{
			if (DrawTitleField("Generic Event", index, "mew_ge_" + index.ToString(), ref delete))
			{
				GenericEventInfo e = _move._genericEvents[index];

				DrawBaseFields(ref e._frame, ref e._required, ref e._playOnce, ref e._persistent, false);

				int selectedIndex = 0;
				if (DrawPopupField(ref selectedIndex, "Function", e._functionName, MoveEditorUtils.GetGenericEventPresetNames()))
				{

					if (selectedIndex == 0)
						e._functionName = "NewEvent";
					else
						e._functionName = MoveEditorUtils.GenericEventPresets[selectedIndex]._eventName;
				}

				MoveEditorUtils.GenericEventPreset preset = MoveEditorUtils.GenericEventPresets[selectedIndex];

				if (selectedIndex == 0)
					e._functionName = EditorGUILayout.TextField("Function Name", e._functionName);

				if (preset._useFloatParameter) e._floatParameter = EditorGUILayout.FloatField("Float", e._floatParameter);
				if (preset._useIntParameter) e._intParameter = EditorGUILayout.IntField("Int", e._intParameter);
				if (preset._useStringParameter) e._stringParameter = EditorGUILayout.TextField("String", e._stringParameter);
				if (preset._useObjectParameter) e._objectParameter = EditorGUILayout.ObjectField("Object", e._objectParameter, typeof(Object), false);

				_move._genericEvents[index] = e;
			}
		}

		private void DrawHitEvent(int index, ref bool delete)
		{
			if (DrawTitleField("Hit Event", index, "mew_he_" + index.ToString(), ref delete))
			{
				HitEventInfo e = _move._hitEvents[index];

				DrawBaseFields(ref e._frame, ref e._required, ref e._playOnce, ref e._persistent, false);

				//EditorGUILayout.LabelField("[[Damage Properties]]");
				//DrawDamageProperties(ref e._damageProperties);

				EditorGUILayout.LabelField("[[Reaction Properties]]");
				EditorGUILayout.TextArea("Rotate Hit: 0=right, 90=forward, 180=left, left to right or right to left are both supported");
				e._hitRxnProps.framesToPlay = EditorGUILayout.FloatField("Duration (frames)", e._hitRxnProps.framesToPlay);
				e._hitRxnProps.startAngle = EditorGUILayout.FloatField("Starting Angle (degrees)", e._hitRxnProps.startAngle);
				e._hitRxnProps.endAngle = EditorGUILayout.FloatField("Ending Angle (degrees)", e._hitRxnProps.endAngle);

				int selectedIndex3 = 0;
				DrawPopupField(ref selectedIndex3, "Action", System.Enum.GetName(typeof(PlayHitReactionProperties.eReactionType), e._hitRxnProps.defaultReaction), System.Enum.GetNames(typeof(PlayHitReactionProperties.eReactionType)));
				e._hitRxnProps.defaultReaction = (PlayHitReactionProperties.eReactionType)(System.Enum.GetValues(typeof(PlayHitReactionProperties.eReactionType)).GetValue(selectedIndex3));
				DrawPopupField(ref selectedIndex3, "PlayType", System.Enum.GetName(typeof(PlayHitReactionProperties.ePlayPriorityType), e._hitRxnProps.playPriority), System.Enum.GetNames(typeof(PlayHitReactionProperties.ePlayPriorityType)));
				e._hitRxnProps.playPriority = (PlayHitReactionProperties.ePlayPriorityType)(System.Enum.GetValues(typeof(PlayHitReactionProperties.ePlayPriorityType)).GetValue(selectedIndex3));
				e._hitRxnProps.weight = EditorGUILayout.IntField("Damage Weight", e._hitRxnProps.weight);
                e._hitRxnProps.isOnlyPlayEffect = EditorGUILayout.Toggle("isOnlyPlayEffect?", e._hitRxnProps.isOnlyPlayEffect);

                EditorGUILayout.LabelField("[[Particle Properties]]");
				DrawParticleProperties(ref e._particleProperties, OnParticleFieldAdd, OnParticleFieldSet, _previewGameObject, _flipped, _previewMode == PreviewMode.Edit);

				EditorGUILayout.LabelField("[[Audio Properties]]");
				DrawAudioProperties(ref e._audioProperties);

				_move._hitEvents[index] = e;
			}
		}

		private bool DrawParticleEvent(int index, ref bool delete)
		{
			if (DrawTitleField("Particle Event", index, "mew_pe_" + index.ToString(), ref delete))
			{
				ParticleEventInfo e = _move._particleEvents[index];

				DrawBaseFields(ref e._frame, ref e._required, ref e._playOnce, ref e._persistent, true);
				DrawParticleProperties(ref e._particleProperties, OnParticleFieldAdd, OnParticleFieldSet, _previewGameObject, _flipped, _previewMode == PreviewMode.Edit);

				_move._particleEvents[index] = e;
			}
			return delete;
		}

		private bool DrawTrailRendererEvent(int index, ref bool delete)
		{
			if (DrawTitleField("Trail Renderer Event", index, "mew_tre_" + index.ToString(), ref delete))
			{
				TrailRendererEventInfo e = _move._trailRendererEvents[index];

				DrawBaseFields(ref e._frame, ref e._required, ref e._playOnce, ref e._persistent, true);
				e._isEditCrit = EditorGUILayout.Toggle("Edit Crit?", e._isEditCrit);
				DrawTrailRendererProperties(ref e._trailRendererProperties, e._isEditCrit, OnTrailAddLocator, OnTrailSelectorLocator, _previewGameObject, _flipped, _previewMode == PreviewMode.Edit);

				_move._trailRendererEvents[index] = e;
			}
			return delete;
		}

		private bool DrawDynamicLightEvent(int index, ref bool delete)
		{
			if (DrawTitleField("Dynamic Light Event", index, "mew_dle_" + index.ToString(), ref delete))
			{
				DynamicLightEventInfo e = _move._dynamicLightEvents[index];

				DrawBaseFields(ref e._frame, ref e._required, ref e._playOnce, ref e._persistent, true);
				DrawDynamicLightProperties(ref e._dynamicLightProperties, OnDynamicLightAdd, OnDynamicLightSelect, _previewGameObject, _flipped, _previewMode == PreviewMode.Edit);

				_move._dynamicLightEvents[index] = e;
			}
			return delete;
		}

		private bool DrawCameraShakeEvent(int index, ref bool delete)
		{
			if (DrawTitleField("Camera Shake Event", index, "mew_cse_" + index.ToString(), ref delete))
			{
				CameraShakeEventInfo e = _move._cameraShakeEvents[index];

				DrawBaseFields(ref e._frame, ref e._required, ref e._playOnce, ref e._persistent, false);
				DrawCameraShakeProperties(ref e._cameraShakeProperties, _cameraShakePresets);

				_move._cameraShakeEvents[index] = e;
			}
			return delete;
		}

		private bool DrawCameraSwingEvent(int index, ref bool delete)
		{
			if (DrawTitleField("Camera Swing Event", index, "mew_cswe_" + index.ToString(), ref delete))
			{
				CameraSwingEventInfo e = _move._cameraSwingEvents[index];

				DrawBaseFields(ref e._frame, ref e._required, ref e._playOnce, ref e._persistent, false);
				DrawCameraSwingProperties(ref e._cameraSwingProperties);

				_move._cameraSwingEvents[index] = e;
			}
			return delete;
		}

		private bool DrawCameraMotionEvent(int index, ref bool delete)
		{
			if (DrawTitleField("Camera Motion Event", index, "new_cme_" + index.ToString(), ref delete))
			{
				CameraMotionEventInfo e = _move._cameraMotionEvents[index];
				DrawBaseFields(ref e._frame, ref e._required, ref e._playOnce, ref e._persistent, false);
				DrawCameraMotionProperties(ref e._cameraMotionProperties);
				_move._cameraMotionEvents[index] = e;
			}
			return delete;
		}

		private bool DrawAudioEvent(int index, ref bool delete)
		{
			if (DrawTitleField("Audio Event", index, "mew_ae_" + index.ToString(), ref delete))
			{
				AudioEventInfo e = _move._audioEvents[index];

				DrawBaseFields(ref e._frame, ref e._required, ref e._playOnce, ref e._persistent, true);

				DrawAudioProperties(ref e._audioProperties);

				_move._audioEvents[index] = e;
			}
			return delete;
		}

		private bool DrawEnvironmentLightingEvent(int index, ref bool delete)
		{
			if (DrawTitleField("Environment Lighting Event", index, "mew_ele_" + index.ToString(), ref delete))
			{
				EnvironmentLightingEventInfo e = _move._environmentLightingEvents[index];

				DrawBaseFields(ref e._frame, ref e._required, ref e._playOnce, ref e._persistent, true);
				DrawEnvironmentLightingProperties(ref e._lightingProperties, index);

				_move._environmentLightingEvents[index] = e;
			}
			return delete;
		}

		private bool DrawProjectileEvent(int index, ref bool delete)
		{
			if (DrawTitleField("Projectile Event", index, "mew_pe_" + index.ToString(), ref delete))
			{
				ProjectileEventInfo e = _move._projectileEvents[index];

				DrawBaseFields(ref e._frame, ref e._required, ref e._playOnce, ref e._persistent, false);
				DrawProjectileProperties(ref e._projectileProperties);

				//EditorGUILayout.LabelField("[[Damage Properties]]");
				//DrawDamageProperties(ref e._hitEvent._damageProperties);

				//EditorGUILayout.LabelField("[[Particle Properties]]");
				//DrawParticleProperties(ref e._hitEvent._particleProperties, OnParticleFieldAdd, OnParticleFieldSet, _previewGameObject, _flipped, _previewMode == PreviewMode.Edit);

				//EditorGUILayout.LabelField("[[Audio Properties]]");
				//DrawAudioProperties(ref e._hitEvent._audioProperties);

				_move._projectileEvents[index] = e;
			}
			return delete;
		}

		private bool DrawPostBloomEvent(int index, ref bool delete)
		{
			if (DrawTitleField("Bloom Event", index, "mew_pbe_" + index.ToString(), ref delete))
			{
				PostBloomEventInfo e = _move._postBloomEvents[index];

				DrawBaseFields(ref e._frame, ref e._required, ref e._playOnce, ref e._persistent, true);

				e._animationCurve = DrawAnimationCurveField(e._animationCurve);
				e._color = EditorGUILayout.ColorField("Color", e._color);
				e._mix = Mathf.Clamp(EditorGUILayout.FloatField("Mix [0, 1]", e._mix), 0, 1);
				e._intensity = Mathf.Clamp(EditorGUILayout.FloatField("Intensity [0, 255]", e._intensity), 0, 255);
				e._blur = Mathf.Clamp(EditorGUILayout.FloatField("Blur [0, 2]", e._blur), 0, 2);
				e._ramp = Mathf.Clamp(EditorGUILayout.FloatField("Ramp [0, 10]", e._ramp), 0, 10);

				_move._postBloomEvents[index] = e;
			}

			return delete;
		}

		private bool DrawPostVignetteEvent(int index, ref bool delete)
		{
			if (DrawTitleField("Vignette Event", index, "mew_pve_" + index.ToString(), ref delete))
			{
				PostVignetteEventInfo e = _move._postVignetteEvents[index];

				DrawBaseFields(ref e._frame, ref e._required, ref e._playOnce, ref e._persistent, true);

				e._animationCurve = DrawAnimationCurveField(e._animationCurve);
				e._mix = Mathf.Clamp(EditorGUILayout.FloatField("Mix [0, 1]", e._mix), 0, 1);
				e._intensity = Mathf.Clamp(EditorGUILayout.FloatField("Intensity [0, 1]", e._intensity), 0, 1);

				_move._postVignetteEvents[index] = e;
			}

			return delete;
		}

		private bool DrawPostWarpEvent(int index, ref bool delete)
		{
			if (DrawTitleField("Warp Event", index, "mew_pwe_" + index.ToString(), ref delete))
			{
				PostWarpEventInfo e = _move._postWarpEvents[index];

				DrawBaseFields(ref e._frame, ref e._required, ref e._playOnce, ref e._persistent, false);

				e._animationCurve = DrawAnimationCurveField(e._animationCurve);
				e._mix = Mathf.Clamp(EditorGUILayout.FloatField("Mix [0, 1]", e._mix), 0, 1);
				e._intensity = EditorGUILayout.Vector2Field("Intensity [(0,0), (1,1)]", e._intensity);

				e._intensity.x = Mathf.Clamp(e._intensity.x, 0, 1);
				e._intensity.y = Mathf.Clamp(e._intensity.y, 0, 1);

				_move._postWarpEvents[index] = e;
			}

			return delete;
		}

		private bool DrawRenderSettingsEvent(int index, ref bool delete)
		{
			if (DrawTitleField("Render Setting Event", index, "mew_pwe_" + index.ToString(), ref delete))
			{
				SwitchRenderSettingsEventInfo e = _move._switchRenderSettingsEvents[index];

				DrawBaseFields(ref e._frame, ref e._required, ref e._playOnce, ref e._persistent, false);

				e._name = EditorGUILayout.TextField("Name: ", e._name);
				_move._switchRenderSettingsEvents[index] = e;
			}

			return delete;
		}

		#endregion

		#region Draw Property Group Methods

		private void DrawDamageProperties(ref DamageEventProperties properties)
		{
			properties._damage = EditorGUILayout.FloatField("Damage", properties._damage);
			properties._mana = EditorGUILayout.FloatField("Mana", properties._mana);
			properties._range = EditorGUILayout.FloatField("Range", properties._range);
			properties._angle = EditorGUILayout.FloatField("Angle", properties._angle);

			string dummyPath = "";
			bool showCopyButton = _previewMode == PreviewMode.Edit;
			DrawBodyPartField(ref properties._pointOfOrigin, ref dummyPath, _previewGameObject, "Point Of Origin", false);
			DrawPositionField(ref properties._offset, _flipped, showCopyButton);

			if (_previewGameObject != null)
			{
				EditorGUILayout.BeginHorizontal();
				{
					if (GUILayout.Button("Add Locator"))
					{
						CleanUpHitOffsetLocator();
						_hitOffsetLocator = new GameObject("Locator");
						_hitOffsetLocator.transform.parent = MoveUtils.GetBodyPartTransform(GetAnimator(), properties._pointOfOrigin);
						_hitOffsetLocator.transform.localPosition = properties._offset;
						Selection.activeTransform = _hitOffsetLocator.transform;
					}

					if (_hitOffsetLocator != null)
					{
						if (GUILayout.Button("Select Locator"))
						{
							Selection.activeTransform = _hitOffsetLocator.transform;
						}
					}
				}
				EditorGUILayout.EndHorizontal();
			}

			properties._knockback = EditorGUILayout.FloatField("Knockback (Hit)", properties._knockback);
			properties._blockKnockback = EditorGUILayout.FloatField("Knockback (Block)", properties._blockKnockback);
			properties._hitStun = EditorGUILayout.FloatField("Hit Stun", properties._hitStun);
			properties._blockStun = EditorGUILayout.FloatField("Block Stun", properties._blockStun);
			properties._freezeTime = EditorGUILayout.FloatField("Freeze Time (Hit)", properties._freezeTime);
			properties._freezeTimeBlocking = EditorGUILayout.FloatField("Freeze Time (Block)", properties._freezeTimeBlocking);
			properties._unblockable = EditorGUILayout.Toggle("Unblockable", properties._unblockable);

			int index = 0;
			if (DrawPopupField(ref index, "Hit Reaction", properties._hitReaction, MoveEditorUtils.HitReactionStates))
			{
				if (index == 0)
					properties._hitReaction = "UNASSIGNED";
				else
					properties._hitReaction = MoveEditorUtils.HitReactionStates[index];
			}

			if (index == 0)
				properties._hitReaction = EditorGUILayout.TextField("Hit Reaction State", properties._hitReaction);

		}

		private void OnParticleFieldAdd(ParticleEventProperties properties, ParticleSystem ps, bool flipped)
		{
			CleanUpPreviewParticle();

			if (ps != null && _previewGameObject != null)
			{
				ResetAnimatorComponent();

				_previewParticleSystem = MoveUtils.InstantiateParticle(this,properties, GetAnimator(), flipped, true);
				if (_previewParticleSystem != null)
				{
					AttachTransform.Detach(_previewParticleSystem.gameObject);
					_previewParticleSystem.transform.parent = MoveUtils.GetBodyPartTransform(GetAnimator(), properties._bodyPart, properties._attachmentPath);
					Selection.activeTransform = _previewParticleSystem.transform;
				}
			}
		}

		private void OnParticleFieldSet()
		{
			switch (_previewMode)
			{
				case PreviewMode.Edit:
					if (_previewParticleSystem != null)
						Selection.activeTransform = _previewParticleSystem.transform;
					break;
			}
		}

		public static void DrawParticleProperties(ref ParticleEventProperties properties, ParticleFieldAddCallback addCallback, ParticleFieldSetCallback setCallback, GameObject previewGameObject, bool flipped, bool showCopyButton)
		{
			properties._eventName = EditorGUILayout.TextField("Event Name", properties._eventName);

			properties._particleReference.Value = DrawParticleField(properties._particleReference.Value, properties, true, addCallback, setCallback, "Left Particle", flipped);
			properties._flippedParticleReference.Value = DrawParticleField(properties._flippedParticleReference.Value, properties, true, addCallback, setCallback, "Right Particle", flipped);

			string patitionString = string.IsNullOrEmpty(properties._partition) ? "None" : properties._partition;
			ParticleEventProperties.ePartition parttion = (ParticleEventProperties.ePartition)EditorGUILayout.EnumPopup("Partition", (ParticleEventProperties.ePartition)System.Enum.Parse(typeof(ParticleEventProperties.ePartition), patitionString));
			properties._partition = parttion != ParticleEventProperties.ePartition.None ? parttion.ToString() : string.Empty;
			properties._interruptable = EditorGUILayout.Toggle("Interrupt On Hit?", properties._interruptable);
			properties._stopOnOverride = EditorGUILayout.Toggle("Stop On Override?", properties._stopOnOverride);
			properties._stopOnExit = EditorGUILayout.Toggle("Stop On Exit Anim?", properties._stopOnExit);
			properties._stopOnEndTurn = EditorGUILayout.Toggle("Stop On End Turn?", properties._stopOnEndTurn);
			EditorGUILayout.BeginHorizontal();
			properties._stopOnDuration = EditorGUILayout.Toggle("Stop On Duration?", properties._stopOnDuration);
			if (properties._stopOnDuration)
			{
				properties._duration = EditorGUILayout.FloatField("Particle Duration", properties._duration);
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			properties._stopAfterTurns = EditorGUILayout.Toggle("Stop After Turns?", properties._stopAfterTurns);
			if (properties._stopAfterTurns)
			{
				properties._turns = EditorGUILayout.IntField("Particle Turns", properties._turns);
			}
			EditorGUILayout.EndHorizontal();

			properties._cancelIfHit = EditorGUILayout.Toggle("Cancel If Hit?", properties._cancelIfHit);
			properties._killIfBlocked = EditorGUILayout.Toggle("Cancel If Blocked?", properties._killIfBlocked);
			properties._cancelIfMissed = EditorGUILayout.Toggle("Cancel If Missed?", properties._cancelIfMissed);

			properties._applyOnTargetList = EditorGUILayout.Toggle("Apply on Target List?", properties._applyOnTargetList);
			if (!properties._applyOnTargetList)
			{
				properties._spawnAtOpponent = EditorGUILayout.Toggle("Spawn At Target?", properties._spawnAtOpponent);
				if (properties._spawnAtOpponent)
				{
					properties._attachToOpponent = EditorGUILayout.Toggle("Attach To Opponent?", properties._attachToOpponent);
					if (!properties._attachToOpponent)
					{
						properties._spawnAtHitPoint = EditorGUILayout.Toggle("Spawn At Target Hit Point?", properties._spawnAtHitPoint);
						properties._spawnAtTargetBase = EditorGUILayout.Toggle("Spawn At Target Base?", properties._spawnAtTargetBase);
					}
				}
				else
				{// keep clean
					properties._attachToOpponent = false;
					properties._spawnAtHitPoint = false;
					properties._spawnAtTargetBase = false;
				}
			}
			else
			{// keep clean
				properties._spawnAtOpponent = false;
				properties._attachToOpponent = false;
				properties._spawnAtHitPoint = false;
				properties._spawnAtTargetBase = false;
			}

			if (properties._applyOnTargetList || !properties._spawnAtOpponent || properties._attachToOpponent)
			{
				DrawBodyPartField(ref properties._bodyPart, ref properties._attachmentPath, previewGameObject);
				properties._parent = EditorGUILayout.Toggle("Parent", properties._parent);
				if (properties._parent)
				{
					properties._lockRotation = EditorGUILayout.Toggle("Lock Rotation", properties._lockRotation);
				}
			}
			else
			{// keep clean
				properties._parent = false;
				properties._lockRotation = false;
			}

			DrawPositionField(ref properties._offset, flipped, showCopyButton);
			DrawOffsetLocksField(ref properties._lockXOffset, ref properties._lockYOffset, ref properties._lockZOffset);
			DrawAnglesField(ref properties._angles, properties._worldSpace, flipped, showCopyButton);

			if (!properties._spawnAtOpponent)
			{
				properties._worldSpace = EditorGUILayout.Toggle("World Space", properties._worldSpace);
			}
			else
			{// keep clean
				properties._worldSpace = false;
			}

			DrawScaleField(ref properties._scale, showCopyButton);
		}

		private void OnTrailAddLocator(TrailRendererEventProperties properties)
		{
			CleanUpTrailLocator();

			if (_previewGameObject != null)
			{
				_trailLocator = new GameObject("trail_locator");
				_trailLocator.transform.parent = MoveUtils.GetBodyPartTransform(GetAnimator(), properties._attachment, properties._attachmentPath);
				_trailLocator.transform.localPosition = properties._rigOffset;
				Selection.activeTransform = _trailLocator.transform;
			}
		}

		private void OnTrailSelectorLocator()
		{
			if (_trailLocator != null)
				Selection.activeTransform = _trailLocator.transform;
		}

		public static void DrawTrailRendererProperties(ref TrailRendererEventProperties properties, bool isEditCrit, TrailAddLocatorCallback addCallback, TrailSelectLocatorCallback selectCallback, GameObject previewGameObject, bool flipped, bool inEditMode)
		{
			TrailRendererInstance trailSettings = null;

			properties._isInterruptible = EditorGUILayout.Toggle("Interruptable?", properties._isInterruptible);
			properties._trailRenderer = (GameObject)EditorGUILayout.ObjectField("Trail Renderer", properties._trailRenderer, typeof(GameObject), false);
			properties._trailRendererCrit = (GameObject)EditorGUILayout.ObjectField("Trail Renderer Crit", properties._trailRendererCrit, typeof(GameObject), false);

			if (isEditCrit)
			{
				if (properties._trailRendererCrit != null)
				{
					trailSettings = properties._trailRendererCrit.GetComponent<TrailRendererInstance>();
				}
			}
			else
			{
				if (properties._trailRenderer != null)
				{
					trailSettings = properties._trailRenderer.GetComponent<TrailRendererInstance>();
				}
			}

			properties._eventName = EditorGUILayout.TextField("Event Name", properties._eventName);
			DrawBodyPartField(ref properties._attachment, ref properties._attachmentPath, previewGameObject);
			properties._parent = EditorGUILayout.Toggle("Parent", properties._parent);
			properties._rigAnimClip = (AnimationClip)EditorGUILayout.ObjectField("Rig Animation", properties._rigAnimClip, typeof(AnimationClip), false);

			if (inEditMode)
			{
				EditorGUILayout.BeginHorizontal();
				{
					if (GUILayout.Button("Add Locator"))
					{
						if (addCallback != null)
						{
							addCallback(properties);
						}
					}
					if (GUILayout.Button("Select Locator"))
					{
						if (selectCallback != null)
						{
							selectCallback();
						}
					}
				}
				EditorGUILayout.EndHorizontal();
			}

			bool showCopyButton = inEditMode;
			DrawPositionField(ref properties._rigOffset, flipped, showCopyButton, "Rig Offset");
			DrawOffsetLocksField(ref properties._lockXOffset, ref properties._lockYOffset, ref properties._lockZOffset);
			DrawAnglesField(ref properties._angles, false, false, showCopyButton);
			properties._lockRotation = EditorGUILayout.Toggle("Lock Rotation", properties._lockRotation);
			properties._worldSpace = EditorGUILayout.Toggle("World Space", properties._worldSpace);
			properties._useOffsets = EditorGUILayout.Toggle("Use Custom Offsets", properties._useOffsets);

			if (properties._useOffsets)
			{
				DrawPositionField(ref properties._offset1, flipped, showCopyButton, "Offset 1");
				DrawPositionField(ref properties._offset2, flipped, showCopyButton, "Offset 2");
				properties._useLocalOffsets = EditorGUILayout.Toggle("Use Local Offsets", properties._useLocalOffsets);
			}

			// TJ: eeesh...this next bit is copied over from "TrailRendererInspector" so that James can edit this stuff straight away in the move editor...
			if (trailSettings != null)
			{
				GameObject tempObject = GameObject.Find("GradientContainter");
				if (tempObject == null)
				{
					tempObject = new GameObject();
					tempObject.name = "GradientContainter";
				}
				//tempObject.hideFlags = HideFlags.DontSave;
				GradientContainer gradientContainer = (GradientContainer)tempObject.GetComponent<GradientContainer>();
				if (gradientContainer == null)
				{
					gradientContainer = (GradientContainer)tempObject.AddComponent<GradientContainer>();
				}

				gradientContainer.ColorGradient = trailSettings._ColorGradient;
				gradientContainer.LifeGradient = trailSettings._LifeGradient;
				SerializedObject serializedGradient = new SerializedObject(gradientContainer);
				SerializedProperty colorGradient = serializedGradient.FindProperty("ColorGradient");
				SerializedProperty lifeGradient = serializedGradient.FindProperty("LifeGradient");


				if (NGUIEditorTools.DrawHeader("Global Settings"))
				{
					NGUIEditorTools.BeginContents();
					trailSettings._AutoPlay = EditorGUILayout.Toggle("Auto Play", trailSettings._AutoPlay);
					trailSettings._IgnoreZ = EditorGUILayout.Toggle("Ignore Z", trailSettings._IgnoreZ);
					trailSettings._TrailLength = (TrailRendererManager.eTRAIL_LENGTH)EditorGUILayout.EnumPopup("Trail Size", trailSettings._TrailLength);
					trailSettings._DistanceThreshold = EditorGUILayout.FloatField("Distance Threshold)", trailSettings._DistanceThreshold);
					trailSettings._TimeUnits = (TrailRenderer.eTIME_UNITS)EditorGUILayout.EnumPopup("Time Units", trailSettings._TimeUnits);

					switch (trailSettings._TimeUnits)
					{
						case TrailRenderer.eTIME_UNITS.Seconds:
							trailSettings._TrailTime = EditorGUILayout.FloatField("Trail Length (seconds)", trailSettings._TrailTime);
							break;
						case TrailRenderer.eTIME_UNITS.Frames:
							trailSettings._TrailTimeInFrames = EditorGUILayout.IntField("Trail Length (frames)", trailSettings._TrailTimeInFrames);
							break;
					}

					trailSettings._WidthCurve = EditorGUILayout.CurveField("Width Curve", trailSettings._WidthCurve);
					trailSettings._Point1 = EditorGUILayout.ObjectField("Point One", trailSettings._Point1, typeof(GameObject), true) as GameObject;
					trailSettings._Point2 = EditorGUILayout.ObjectField("Point Two", trailSettings._Point2, typeof(GameObject), true) as GameObject;
					trailSettings._TextureRepeat = EditorGUILayout.FloatField("Texture length (meters)", trailSettings._TextureRepeat);
					trailSettings._TextureMetersSecond = EditorGUILayout.FloatField("Texture animate speed (meters/seconds)", trailSettings._TextureMetersSecond);
					trailSettings._TextureYSplit = EditorGUILayout.IntField("Vertical texture frames", trailSettings._TextureYSplit);
					trailSettings._SpanOverTrail = EditorGUILayout.Toggle("Color Gradient Over Trail", trailSettings._SpanOverTrail);
					trailSettings._AddColor = EditorGUILayout.Toggle("Life Gradient Add Color", trailSettings._AddColor);
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField(colorGradient, true, null);
					EditorGUILayout.PropertyField(lifeGradient, true, null);
					if (EditorGUI.EndChangeCheck())
					{
						serializedGradient.ApplyModifiedProperties();
					}
					trailSettings._Material = EditorGUILayout.ObjectField("Material", trailSettings._Material, typeof(Material), false) as Material;

					NGUIEditorTools.EndContents();
				}

				trailSettings._TrailType = (TrailRenderer.eTRAIL_TYPE)EditorGUILayout.EnumPopup("Trail Type", trailSettings._TrailType);

				switch (trailSettings._TrailType)
				{
					case TrailRenderer.eTRAIL_TYPE.Uniform:
						{
							if (NGUIEditorTools.DrawHeader("Uniform Fade Settings"))
							{
								NGUIEditorTools.BeginContents();

								switch (trailSettings._TimeUnits)
								{
									case TrailRenderer.eTIME_UNITS.Seconds:
										trailSettings._FadeStartTime = EditorGUILayout.FloatField("Fade Start Time (seconds)", trailSettings._FadeStartTime);
										trailSettings._FadeDuration = EditorGUILayout.FloatField("Fade Duration (seconds)", trailSettings._FadeDuration);
										break;
									case TrailRenderer.eTIME_UNITS.Frames:
										trailSettings._FadeStartTimeInFrames = EditorGUILayout.IntField("Fade Start Time (frames)", trailSettings._FadeStartTimeInFrames);
										trailSettings._FadeDurationInFrames = EditorGUILayout.IntField("Fade Duration (frames)", trailSettings._FadeDurationInFrames);
										break;
								}

								NGUIEditorTools.EndContents();
							}
							break;
						}

					case TrailRenderer.eTRAIL_TYPE.Catchup:
						{
							if (NGUIEditorTools.DrawHeader("Catchup Fade Settings"))
							{
								NGUIEditorTools.BeginContents();

								switch (trailSettings._TimeUnits)
								{
									case TrailRenderer.eTIME_UNITS.Seconds:
										trailSettings._FadeStartTime = EditorGUILayout.FloatField("Catchup Start Time (seconds)", trailSettings._FadeStartTime);
										trailSettings._FadeDuration = EditorGUILayout.FloatField("Catchup Duration (seconds)", trailSettings._FadeDuration);
										break;
									case TrailRenderer.eTIME_UNITS.Frames:
										trailSettings._FadeStartTimeInFrames = EditorGUILayout.IntField("Catchup Start Time (frames)", trailSettings._FadeStartTimeInFrames);
										trailSettings._FadeDurationInFrames = EditorGUILayout.IntField("Catchup Duration (frames)", trailSettings._FadeDurationInFrames);
										break;
								}

								NGUIEditorTools.EndContents();
							}
							break;
						}
					case TrailRenderer.eTRAIL_TYPE.Drag:
						{
							if (NGUIEditorTools.DrawHeader("Catchup Fade Settings"))
							{
								NGUIEditorTools.BeginContents();

								switch (trailSettings._TimeUnits)
								{
									case TrailRenderer.eTIME_UNITS.Seconds:
										trailSettings._FadeDuration = EditorGUILayout.FloatField("FadeIn Duration (seconds)", trailSettings._FadeDuration);
										break;
									case TrailRenderer.eTIME_UNITS.Frames:
										trailSettings._FadeDurationInFrames = EditorGUILayout.IntField("FadeIn Duration (frames)", trailSettings._FadeDurationInFrames);
										break;
								}

								NGUIEditorTools.EndContents();
							}
							break;
						}
					default:
						{
							EB.Debug.LogError("Bad trail type in inspector");
							break;
						}
				}

				trailSettings._ColorGradient = gradientContainer.ColorGradient;
				trailSettings._LifeGradient = gradientContainer.LifeGradient;
				EditorUtility.SetDirty(properties._trailRenderer);
			}
		}

		public static void DrawDynamicLightProperties(ref DynamicLightEventProperties properties, DynamicLightFieldAddCallback addCallback, DynamicLightFieldSelectCallback selectCallback, GameObject previewGameObject, bool flipped, bool inEditMode)
		{
			DynamicPointLightInstance lightSettings = null;

			properties._interruptable = EditorGUILayout.Toggle("Interruptable?", properties._interruptable);
			properties._stopOnExit = EditorGUILayout.Toggle("Stop On Exit Anim?", properties._stopOnExit);

			DrawDynamicLightField(ref properties._dynamicLight, properties, addCallback, selectCallback, "Dynamic Light", flipped, inEditMode);

			if (properties._dynamicLight != null)
				lightSettings = properties._dynamicLight.GetComponent<DynamicPointLightInstance>();

			properties._eventName = EditorGUILayout.TextField("Event Name", properties._eventName);

			DrawBodyPartField(ref properties._attachment, ref properties._attachmentPath, previewGameObject);

			properties._parent = EditorGUILayout.Toggle("Parent", properties._parent);

			bool showCopyButton = inEditMode;
			DrawPositionField(ref properties._offset, false, showCopyButton);

			if (lightSettings != null)
			{
				lightSettings.Intensity = EditorGUILayout.CurveField("Intensity", lightSettings.Intensity);
				lightSettings.IntensityMultiplier = EditorGUILayout.FloatField("Intensity Multiplier", lightSettings.IntensityMultiplier);

				GameObject tempObject = GameObject.Find("GradientContainter");
				if (tempObject == null)
				{
					tempObject = new GameObject();
					tempObject.name = "GradientContainter";
				}
				//tempObject.hideFlags = HideFlags.DontSave;
				GradientContainer gradientContainer = (GradientContainer)tempObject.GetComponent<GradientContainer>();
				if (gradientContainer == null)
				{
					gradientContainer = (GradientContainer)tempObject.AddComponent<GradientContainer>();
				}

				gradientContainer.DynamicLightGradient = lightSettings.Gradient;
				SerializedObject serializedGradient = new SerializedObject(gradientContainer);
				SerializedProperty dynamicLightGradient = serializedGradient.FindProperty("DynamicLightGradient");

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(dynamicLightGradient, true, null);
				if (EditorGUI.EndChangeCheck())
				{
					serializedGradient.ApplyModifiedProperties();
				}

				lightSettings.Gradient = gradientContainer.DynamicLightGradient;
				lightSettings.CycleTime = EditorGUILayout.FloatField("Cycle Time", lightSettings.CycleTime);
				lightSettings.AutoPlay = EditorGUILayout.Toggle("Auto Play", lightSettings.AutoPlay);
				lightSettings.OneShot = EditorGUILayout.Toggle("One Shot", lightSettings.OneShot);
				EditorUtility.SetDirty(properties._dynamicLight);
			}
		}

		public static void DrawCameraShakeProperties(ref CameraShakeEventProperties properties, SortedDictionary<string, CameraShakePreset> cameraShakePresets)
		{
			int index = 0;
			if (cameraShakePresets != null)
			{
				EditorGUILayout.BeginHorizontal();
				{
					string[] cameraShakePresetKeys = cameraShakePresets.Keys.ToArray();
					if (DrawPopupField(ref index, "Preset", properties._presetName, cameraShakePresetKeys))
					{
						properties._presetName = cameraShakePresetKeys[index];
						CameraShakePreset preset = LoadCameraShakePreset(properties._presetName, cameraShakePresets);

						if (preset != null)
							SetCameraShakePropertiesFromPreset(preset, ref properties);

						if (index == 0)
							properties._presetName = string.Empty;
					}

					if (GUILayout.Button("Save", GUILayout.Width(75)))
					{
						SaveCameraShakePreset(LoadCameraShakePreset(properties._presetName, cameraShakePresets), properties, cameraShakePresets);
					}
				}
				EditorGUILayout.EndHorizontal();
			}

			if (index == 0)
				properties._presetName = EditorGUILayout.TextField("Name", properties._presetName);
			NGUIEditorTools.BeginContents();
			{
				GUILayout.Label("[Shake Params for Normal Attack]");
				properties._numberOfShakes = EditorGUILayout.IntField("Number of Shakes", properties._numberOfShakes);
				properties._shakeAmount = EditorGUILayout.Vector3Field("Shake Amount", properties._shakeAmount);
				properties._rotationAmount = EditorGUILayout.Vector3Field("Rotation Amount", properties._rotationAmount);
				properties._distance = EditorGUILayout.FloatField("Distance", properties._distance);
				properties._speed = EditorGUILayout.FloatField("Speed", properties._speed);
				properties._decay = EditorGUILayout.FloatField("Decay", properties._decay);
			}
			NGUIEditorTools.EndContents();

			NGUIEditorTools.BeginContents();
			{
				GUILayout.Label("[Shake Params for Critical Attack]");
				properties._numberOfShakesCrit = EditorGUILayout.IntField("Number of Shakes Crit", properties._numberOfShakesCrit);
				properties._shakeAmountCrit = EditorGUILayout.Vector3Field("Shake Amount Crit", properties._shakeAmountCrit);
				properties._rotationAmountCrit = EditorGUILayout.Vector3Field("Rotation Amount Crit", properties._rotationAmountCrit);
				properties._distanceCrit = EditorGUILayout.FloatField("Distance Crit", properties._distanceCrit);
				properties._speedCrit = EditorGUILayout.FloatField("Speed Crit", properties._speedCrit);
				properties._decayCrit = EditorGUILayout.FloatField("Decay Crit", properties._decayCrit);
			}
			NGUIEditorTools.EndContents();

			properties._multiplyByTimeScale = EditorGUILayout.Toggle("Multiply By Time Scale", properties._multiplyByTimeScale);
		}

		private void DrawCameraSwingProperties(ref CameraSwingEventProperties properties)
		{
			properties._degrees = EditorGUILayout.FloatField("Degrees", properties._degrees);
			properties._lerpAmount = EditorGUILayout.FloatField("Lerp Amount", properties._lerpAmount);
		}

		private void DrawCameraMotionProperties(ref CameraMotionEventProperties properties)
		{
			//properties._lerpCurve = EditorGUILayout.CurveField("Lerp Curve", properties._lerpCurve);
			properties._lerpFrames = EditorGUILayout.IntField("Lerp Frames", properties._lerpFrames);
			properties._lerpDuration = (float)properties._lerpFrames / _move._animationClip.frameRate;
			properties._hangonFrames = EditorGUILayout.IntField("Hangon Frames", properties._hangonFrames);
			properties._hangonDuration = (float)properties._hangonFrames / _move._animationClip.frameRate;
			properties._motionTrigger = (CameraMotionTrigger)EditorGUILayout.EnumPopup("Motion Trigger", properties._motionTrigger);
			properties._motionTarget = (CameraMotionTarget)EditorGUILayout.EnumPopup("Motion Target", properties._motionTarget);
			properties._blendCurrentCamera = EditorGUILayout.Toggle("Blend Current Camera?", properties._blendCurrentCamera);
			if (properties._blendCurrentCamera)
			{
                properties._onlyLookAtTarget = EditorGUILayout.Toggle("Only Look At Target?", properties._onlyLookAtTarget);
                if (!properties._onlyLookAtTarget) //仅朝向目标不需要位移摄像机
                {
                    EditorGUILayout.LabelField("[Blend Camera Offset Params]");
                    properties._blendLerpSmoothing = (CameraLerp.LerpSmoothing)EditorGUILayout.EnumPopup("Blend Lerp Smoothing", properties._blendLerpSmoothing);
                    if (properties._blendLerpSmoothing == CameraLerp.LerpSmoothing.custom)
                    {
                        properties._blendLerpCurve = EditorGUILayout.CurveField("Blend Pos Lerp Curve", properties._blendLerpCurve);
                    }
                    properties._blendPitchLerpSmoothing = (CameraLerp.LerpSmoothing)EditorGUILayout.EnumPopup("Blend Pitch Lerp Smoothing", properties._blendPitchLerpSmoothing);
                    if (properties._blendPitchLerpSmoothing == CameraLerp.LerpSmoothing.custom)
                    {
                        properties._blendPitchLerpCurve = EditorGUILayout.CurveField("Blend Pitch Lerp Curve", properties._blendPitchLerpCurve);
                    }
                    properties._blendYawLerpSmoothing = (CameraLerp.LerpSmoothing)EditorGUILayout.EnumPopup("Blend Yaw Lerp Smoothing", properties._blendYawLerpSmoothing);
                    if (properties._blendYawLerpSmoothing == CameraLerp.LerpSmoothing.custom)
                    {
                        properties._blendYawLerpCurve = EditorGUILayout.CurveField("Blend Yaw Lerp Curve", properties._blendYawLerpCurve);
                    }
                    properties._blendDistanceOffset = EditorGUILayout.FloatField("Blend Distance Offset", properties._blendDistanceOffset);
                    properties._blendPitchOffset = EditorGUILayout.FloatField("Blend Pitch Offset", properties._blendPitchOffset);
                    properties._blendYawOffset = EditorGUILayout.FloatField("Blend Yaw Offset", properties._blendYawOffset);
                    properties._blendHeightOffset = EditorGUILayout.FloatField("Blend Height Offset", properties._blendHeightOffset);
                }
			}
			else
			{
				EditorGUILayout.LabelField("[Camera Motion Options]");

				List<CameraMotionOption> options = GM.JSON.ToObject<List<CameraMotionOption>>(properties._motionOptions);
				if (GUILayout.Button("Add Camera Motion Option"))
				{
					CameraMotionOption option = new CameraMotionOption();
					options.Add(option);
				}
				for (int i = 0; i < options.Count; i++)
				{
					if (i > 0)
					{
						GUILayout.Space(5.0f);
					}
					bool delete = false;
					NGUIEditorTools.BeginContents();
					{
						if (DrawTitleField("Camera Motion Option", i, "new_cmo_" + i.ToString(), ref delete))
						{
							CameraMotionOption option = options[i];
							DrawCameraMotionOptionProperties(ref option);
							options[i] = option;
						}
					}
					NGUIEditorTools.EndContents();

					if (delete)
					{
						options.RemoveAt(i);
						break;
					}
				}
				properties._motionOptions = GM.JSON.ToJson(options);
			}
			GUILayout.Space(5.0f);
		}
		private void DrawCameraMotionOptionProperties(ref CameraMotionOption option)
		{
			List<string> motionNames = new List<string>();
			int current_selected = -1;
			for (int i = 0; i < GlobalCameraMotionData.Instance._motions.Count; i++)
			{
				motionNames.Add(GlobalCameraMotionData.Instance._motions[i].name);
				if (string.Compare(GlobalCameraMotionData.Instance._motions[i].name, option._motionName) == 0)
				{
					current_selected = i;
				}
			}
			motionNames.Add("(NONE)");
			if (current_selected == -1)
			{
				current_selected = motionNames.Count - 1;
			}

			current_selected = EditorGUILayout.Popup("Camera Motion Data", current_selected, motionNames.ToArray());
			option._motionName = motionNames[current_selected];
			option._probability = EditorGUILayout.FloatField("Probability", option._probability);
			option._isCritical = EditorGUILayout.Toggle("Is Critical", option._isCritical);
		}

		private void DrawAudioProperties(ref AudioEventProperties properties)
		{
			// Directly edit the string of the event. Plac the fabric event name in there.
			properties._event = EditorGUILayout.TextField("Audio Event", properties._event);
            // MaxG: add more properties to handle persistent sounds
            properties._isPersistent = EditorGUILayout.Toggle("Is Persistent", properties._isPersistent);
			if (properties._isPersistent)
			{
				properties._stopOnOverride = EditorGUILayout.Toggle("Stop On Override?", properties._stopOnOverride);
				properties._interruptable = EditorGUILayout.Toggle("Interrupt On Hit?", properties._interruptable);
				properties._stopOnExitAnim = EditorGUILayout.Toggle("Stop on Exit Anim", properties._stopOnExitAnim);
				properties._stopOnEndTurn = EditorGUILayout.Toggle("Stop On End Turn?", properties._stopOnEndTurn);
				EditorGUILayout.BeginHorizontal();
				properties._stopOnDuration = EditorGUILayout.Toggle("Stop On Duration?", properties._stopOnDuration);
				if (properties._stopOnDuration)
				{
					properties._duration = EditorGUILayout.FloatField("Audio Duration", properties._duration);
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				properties._stopAfterTurns = EditorGUILayout.Toggle("Stop After Turns?", properties._stopAfterTurns);
				if (properties._stopAfterTurns)
				{
					properties._turns = EditorGUILayout.IntField("Audio Turns", properties._turns);
				}
				EditorGUILayout.EndHorizontal();
			}

			properties._cancelIfHit = EditorGUILayout.Toggle("Cancel If Hit?", properties._cancelIfHit);
			properties._cancelIfBlocked = EditorGUILayout.Toggle("Cancel If Blocked?", properties._cancelIfBlocked);
			properties._cancelIfMissed = EditorGUILayout.Toggle("Cancel If Missed?", properties._cancelIfMissed);

			properties._applyOnTargetList = EditorGUILayout.Toggle("Apply on Target List?", properties._applyOnTargetList);
		}

		private void DrawEnvironmentLightingProperties(ref EnvironmentLightingProperties properties, int arrayIndex)
		{
			GameObject tempObject = GameObject.Find("GradientContainter");
			if (tempObject == null)
			{
				tempObject = new GameObject();
				tempObject.name = "GradientContainter";
			}
			//tempObject.hideFlags = HideFlags.DontSave;
			GradientContainer gradientContainer = (GradientContainer)tempObject.GetComponent<GradientContainer>();
			if (gradientContainer == null)
			{
				gradientContainer = (GradientContainer)tempObject.AddComponent<GradientContainer>();
			}

			while (gradientContainer.EnvMultiplyGradientList.Count < arrayIndex + 1)
				gradientContainer.EnvMultiplyGradientList.Add(new Gradient());

			while (gradientContainer.EnvAddGradientList.Count < arrayIndex + 1)
				gradientContainer.EnvAddGradientList.Add(new Gradient());

			gradientContainer.EnvMultiplyGradientList[arrayIndex] = properties._multiplyGradient;
			gradientContainer.EnvAddGradientList[arrayIndex] = properties._addGradient;

			SerializedObject serializedGradient = new SerializedObject(gradientContainer);
			SerializedProperty multiplyGradientList = serializedGradient.FindProperty("EnvMultiplyGradientList");
			SerializedProperty addGradientList = serializedGradient.FindProperty("EnvAddGradientList");

			EditorGUILayout.PropertyField(multiplyGradientList.GetArrayElementAtIndex(arrayIndex), true, null);
			properties._multiplyDuration = EditorGUILayout.FloatField("Multiply Duration", properties._multiplyDuration);
			EditorGUILayout.PropertyField(addGradientList.GetArrayElementAtIndex(arrayIndex), true, null);
			properties._addDuration = EditorGUILayout.FloatField("Add Duration", properties._addDuration);

			serializedGradient.ApplyModifiedProperties();

		}

		private void DrawProjectileProperties(ref ProjectileEventProperties properties)
		{
			properties._prefab = (GameObject)EditorGUILayout.ObjectField("Projectile Prefab", properties._prefab, typeof(GameObject), false);
            properties._changeEffect = (GameObject)EditorGUILayout.ObjectField("Prefab Change Effect", properties._changeEffect, typeof(GameObject), false);
            bool inEditMode = _previewMode == PreviewMode.Edit;
			if (inEditMode)
			{
				EditorGUILayout.BeginHorizontal();
				{
					if (GUILayout.Button("Add"))
					{
						CleanUpProjectileSpawnLocator();

						if (_previewGameObject != null)
						{
							if (properties._prefab != null)
							{
								_projectileSpawnLocator = GameObject.Instantiate(properties._prefab) as GameObject;
							}
							else
							{
								_projectileSpawnLocator = new GameObject("projectile_spawn_locator");
							}
							_projectileSpawnLocator.transform.parent = MoveUtils.GetBodyPartTransform(GetAnimator(), properties._spawnAttachment, properties._spawnAttachmentPath);
							_projectileSpawnLocator.transform.localPosition = properties._spawnOffset;

							if (properties._worldSpace)
							{
								_projectileSpawnLocator.transform.eulerAngles = properties._spawnAngles;
							}
							else
							{
								_projectileSpawnLocator.transform.localEulerAngles = properties._spawnAngles;
							}

							Selection.activeTransform = _projectileSpawnLocator.transform;
						}
					}
					if (GUILayout.Button("Select"))
					{
						if (_projectileSpawnLocator != null)
						{
							Selection.activeTransform = _projectileSpawnLocator.transform;
						}
					}
				}
				EditorGUILayout.EndHorizontal();
			}
            properties._flyTime = EditorGUILayout.FloatField("Fly Time  (s)", properties._flyTime);
            properties._fadeOutTime = EditorGUILayout.FloatField("FadeOutTime (m/s)", properties._fadeOutTime);

            DrawBodyPartField(ref properties._spawnAttachment, ref properties._spawnAttachmentPath, _previewGameObject);
			DrawPositionField(ref properties._spawnOffset, _flipped, inEditMode);
			DrawAnglesField(ref properties._spawnAngles, properties._worldSpace, _flipped, inEditMode);

			#region [GM] Projectile.
			properties._autoVelocity = EditorGUILayout.Toggle("Auto-Velocity [GM]", properties._autoVelocity);
			#endregion

			properties._worldSpace = EditorGUILayout.Toggle("World Space", properties._worldSpace);

			properties._initialVelocity = EditorGUILayout.FloatField("Velocity (m/s)", properties._initialVelocity);

            properties._isTarget = EditorGUILayout.Toggle("Is Target", properties._isTarget);
            properties._isLookAtTarget = EditorGUILayout.Toggle("IsLookAtTarget", properties._isLookAtTarget);
            properties._isOnly = EditorGUILayout.Toggle("Is Only", properties._isOnly);

            properties._isBoomerang = EditorGUILayout.Toggle("Is Boomerang?", properties._isBoomerang);
			if (properties._isBoomerang)
			{
				properties._reattachmentVelocity = EditorGUILayout.FloatField("Reattachment Velocity (m/s)", properties._reattachmentVelocity);
				properties._reattachmentAngularVelocity = EditorGUILayout.FloatField("Angular Velocity", properties._reattachmentAngularVelocity);
				//properties._reattachmentPropName = EditorGUILayout.TextField("Respawn Prop Name", properties._reattachmentPropName);

				EditorGUILayout.LabelField("[[Reattachment]]");
				if (inEditMode)
				{
					EditorGUILayout.BeginHorizontal();
					{
						if (GUILayout.Button("Add"))
						{
							CleanUpProjectileSpawnLocator();

							if (_previewGameObject != null)
							{
								if (properties._prefab != null)
								{
									_projectileSpawnLocator = GameObject.Instantiate(properties._prefab) as GameObject;
								}
								else
								{
									_projectileSpawnLocator = new GameObject("projectile_spawn_locator");
								}
								_projectileSpawnLocator.transform.parent = MoveUtils.GetBodyPartTransform(GetAnimator(), properties._reattachment, properties._reattachmentPath);
								_projectileSpawnLocator.transform.localPosition = properties._reattachmentOffset;

								if (properties._reattachmentIsWorldSpace)
								{
									_projectileSpawnLocator.transform.eulerAngles = properties._reattachmentAngles;
								}
								else
								{
									_projectileSpawnLocator.transform.localEulerAngles = properties._reattachmentAngles;
								}

								Selection.activeTransform = _projectileSpawnLocator.transform;
							}
						}
						if (GUILayout.Button("Select"))
						{
							if (_projectileSpawnLocator != null)
							{
								Selection.activeTransform = _projectileSpawnLocator.transform;
							}
						}
					}
					EditorGUILayout.EndHorizontal();
				}
				DrawBodyPartField(ref properties._reattachment, ref properties._reattachmentPath, _previewGameObject);
				DrawPositionField(ref properties._reattachmentOffset, _flipped, inEditMode);
				DrawAnglesField(ref properties._reattachmentAngles, properties._reattachmentIsWorldSpace, _flipped, inEditMode);
				properties._reattachmentIsWorldSpace = EditorGUILayout.Toggle("World Space", properties._reattachmentIsWorldSpace);
			}
		}

		#endregion

		#region Draw Field Methods

		// callbacks for the fields
		public delegate void ParticleFieldAddCallback(ParticleEventProperties properties, ParticleSystem ps, bool flipped);
		public delegate void ParticleFieldSetCallback();
		public delegate void TrailAddLocatorCallback(TrailRendererEventProperties properties);
		public delegate void TrailSelectLocatorCallback();
		public delegate void DynamicLightFieldAddCallback(DynamicLightEventProperties properties, bool flipped);
		public delegate void DynamicLightFieldSelectCallback();

		private bool DrawTitleField(string title, int index, string prefsKey, ref bool delete)
		{
			bool view = EditorPrefs.GetBool(prefsKey, true);
			EditorGUILayout.BeginHorizontal();
			{
				view = EditorGUILayout.Foldout(view, string.Format("{0} {1}", title, index));
				if (GUILayout.Button("X", GUILayout.Width(20)))
					delete = true;
			}
			EditorGUILayout.EndHorizontal();
			EditorPrefs.SetBool(prefsKey, view);
			return view;
		}

		private void DrawBaseFields(ref float frame, ref bool required, ref bool playOnce, ref bool persistent, bool showPersistence)
		{
			if (GetAnimationClip() != null)
			{
				EditorGUILayout.BeginHorizontal();
				{
					frame = Mathf.Clamp(EditorGUILayout.FloatField("Frame", frame), 0, _numFrames);
					if (GUILayout.Button("Set", GUILayout.Width(35)))
					{
						frame = _currentFrame;
					}
					if (GUILayout.Button("Go", GUILayout.Width(35)))
					{
						_currentFrame = frame;
						ScrubAnimation();
					}
				}
				EditorGUILayout.EndHorizontal();

				required = EditorGUILayout.Toggle("Required", required);
				playOnce = EditorGUILayout.Toggle("Play Once", playOnce);
				if (showPersistence)
				{
					persistent = EditorGUILayout.Toggle("Persistent", persistent);
				}
			}
		}


		private static ParticleSystem DrawParticleField(ParticleSystem value, ParticleEventProperties properties, bool enableSetAndAdd, ParticleFieldAddCallback addCallback, ParticleFieldSetCallback setCallback, string title = "Particle", bool flipped = false)
		{
			ParticleSystem result = null;

			EditorGUILayout.BeginHorizontal();
			{
				result = (ParticleSystem)EditorGUILayout.ObjectField(title, value, typeof(ParticleSystem), false);

				if (enableSetAndAdd)
				{
					if (GUILayout.Button("Add", GUILayout.Width(35)))
					{
						if (addCallback != null)
						{
							addCallback(properties, value, flipped);
						}
					}
					if (GUILayout.Button("Sel", GUILayout.Width(35)))
					{
						if (setCallback != null)
						{
							setCallback();
						}
					}
				}
			}
			EditorGUILayout.EndHorizontal();

			return result;
		}

		private void OnDynamicLightAdd(DynamicLightEventProperties properties, bool flipped)
		{
			CleanUpDynamicLightPreview();

			if (_previewGameObject != null)
			{
				ResetAnimatorComponent();

				_previewDynamicLight = MoveUtils.InstantiateDynamicLight(properties, GetAnimator(), flipped, true);
				if (_previewDynamicLight != null)
				{
					_previewDynamicLight.transform.parent = MoveUtils.GetBodyPartTransform(GetAnimator(), properties._attachment, properties._attachmentPath);
					Selection.activeTransform = _previewDynamicLight.transform;
				}
			}
		}

		private void OnDynamicLightSelect()
		{
			if (_previewDynamicLight != null)
				Selection.activeTransform = _previewDynamicLight.transform;
		}

		private static void DrawDynamicLightField(ref GameObject value, DynamicLightEventProperties properties, DynamicLightFieldAddCallback addCallback, DynamicLightFieldSelectCallback selectCallback, string title = "Dynamic Light", bool flipped = false, bool inEditMode = false)
		{
			EditorGUILayout.BeginHorizontal();
			{
				value = (GameObject)EditorGUILayout.ObjectField(title, value, typeof(GameObject), false);

				if (inEditMode)
				{
					if (GUILayout.Button("Add", GUILayout.Width(35)))
					{
						if (value != null && addCallback != null)
						{
							addCallback(properties, flipped);
						}
					}
					if (GUILayout.Button("Sel", GUILayout.Width(35)))
					{
						if (selectCallback != null)
						{
							selectCallback();
						}
					}
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		private static void DrawBodyPartField(ref BodyPart value, ref string path, GameObject previewGameObject, string title = "Attachment", bool canUseCustomPath = true)
		{
			EditorGUILayout.BeginHorizontal();
			{
				value = (BodyPart)EditorGUILayout.EnumPopup(title, value);
				if (GUILayout.Button("Select", GUILayout.Width(75)))
				{
					if (previewGameObject != null)
					{
						Animator animator = previewGameObject != null ? previewGameObject.GetComponent<Animator>() : null;
						Transform t = MoveUtils.GetBodyPartTransform(animator, value, path);
						if (t != null)
							Selection.activeTransform = t;
					}
				}
			}
			EditorGUILayout.EndHorizontal();

			if (value == BodyPart.Custom && canUseCustomPath)
			{
				EditorGUILayout.BeginHorizontal();
				{
					path = EditorGUILayout.TextField("Custom Path", path);

					if (previewGameObject != null)
					{
						if (GUILayout.Button("Copy", GUILayout.Width(75.0f)))
						{
							string testValue;
							if (TryGetTransformPathFromSelection(previewGameObject.transform, out testValue))
							{
								path = testValue;
							}
						}
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				//path = "";
			}
		}

		private static bool TryGetTransformPathFromSelection(Transform root, out string value)
		{
			if (Selection.activeTransform != root)
			{
				if (Selection.activeTransform.IsChildOf(root))
				{
					Transform transform = Selection.activeTransform;
					string path = string.Empty;
					while (transform != null && transform != root)
					{
						path = "/" + transform.name + path;
						transform = transform.parent;
					}

					value = path.Substring(1);
					return true;
				}
			}

			value = "";
			return false;
		}

		private static void DrawPositionField(ref Vector3 value, bool flipped, bool showCopyButton, string title = "Offset")
		{
			Vector3 local = Selection.activeTransform != null ? Selection.activeTransform.localPosition : Vector3.zero;
			DrawVector3Field(ref value, title, local, Vector3.zero, false, Vector3.zero, flipped, showCopyButton);
		}

		private static void DrawAnglesField(ref Vector3 value, bool worldSpace, bool flipped, bool showCopyButton)
		{
			Vector3 local = Selection.activeTransform != null ? Selection.activeTransform.localEulerAngles : Vector3.zero;
			Vector3 world = Selection.activeTransform != null ? Selection.activeTransform.eulerAngles : Vector3.zero;
			DrawVector3Field(ref value, "Angles", local, world, worldSpace, Vector3.zero, flipped, showCopyButton);
		}

		private static void DrawScaleField(ref Vector3 value, bool showCopyButton)
		{
			Vector3 world = Selection.activeTransform != null ? Selection.activeTransform.lossyScale : Vector3.zero;
			DrawVector3Field(ref value, "Scale", Vector3.one, world, true, Vector3.one, false, showCopyButton);
		}

		private static void DrawVector3Field(ref Vector3 value, string title, Vector3 local, Vector3 world, bool worldSpace, Vector3 defaultValue, bool flipped, bool showCopyButton)
		{
			EditorGUILayout.BeginHorizontal();
			{
				value = EditorGUILayout.Vector3Field(title, value);

				if (showCopyButton && !flipped)
				{
					if (GUILayout.Button("Copy", GUILayout.Width(50)))
					{
						if (Selection.activeTransform != null) // && Selection.activeTransform.particleSystem != null)
						{
							if (worldSpace)
							{
								value = world;
							}
							else
							{
								value = local;
							}
						}
					}
				}

				if (GUILayout.Button("X", GUILayout.Width(20)))
				{
					value = defaultValue;
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		private static void DrawOffsetLocksField(ref bool lockX, ref HeightLock lockY, ref bool lockZ)
		{
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField("Lock", GUILayout.Width(50));

				EditorGUILayout.LabelField("X", GUILayout.Width(10));
				lockX = EditorGUILayout.Toggle(lockX, GUILayout.Width(20));

				GUILayout.FlexibleSpace();

				EditorGUILayout.LabelField("Y", GUILayout.Width(10));
				lockY = (HeightLock)EditorGUILayout.EnumPopup(lockY, GUILayout.Width(105));

				GUILayout.FlexibleSpace();

				EditorGUILayout.LabelField("Z", GUILayout.Width(10));
				lockZ = EditorGUILayout.Toggle(lockZ, GUILayout.Width(20));

				GUILayout.FlexibleSpace();
			}
			EditorGUILayout.EndHorizontal();
		}

		// returns true if value changed
		private static bool DrawPopupField(ref int index, string label, string current, string[] options)
		{
			int selected = string.IsNullOrEmpty(current) ? 0 : System.Array.IndexOf(options, current);
			selected = Mathf.Max(selected, 0);
			index = EditorGUILayout.Popup(label, selected, options);
			return selected != index;
		}

		private static AnimationCurve DrawAnimationCurveField(AnimationCurve curve, float defaultDuration = 1.0f)
		{
			if (curve.keys.Length == 0)
			{
				curve.AddKey(0, 0);
			}

			if (curve.keys.Length == 1)
			{
				curve.AddKey(defaultDuration, 0.0f);
			}

			float duration 	= curve.keys[curve.keys.Length - 1].time;
			duration = EditorGUILayout.FloatField("Duration", duration);
			curve = EditorGUILayout.CurveField("Animation Curve", curve);

			Keyframe[] keys 			= curve.keys;
			keys[0].time = 0.0f;
			keys[keys.Length - 1].time = duration;
			keys[keys.Length - 1].value = 0.0f;

			for (int i = 0; i < keys.Length; i++)
			{
				keys[i].value = Mathf.Clamp01(keys[i].value);
			}

			curve.keys = keys;

			return curve;
		}


		#endregion

		public void SetMove(Move move, string path, bool resetCharacter = false)
		{
			if (move != null && !string.IsNullOrEmpty(path))
			{
				string move_dir_path = MoveEditorUtils.GetMovesDirectoryPath();
				move_dir_path = move_dir_path.Substring(move_dir_path.IndexOf("Assets"));
				move._path = path.Substring(move_dir_path.Length);
				move._path = System.IO.Path.GetDirectoryName(move._path) + '/' + System.IO.Path.GetFileNameWithoutExtension(move._path);

				//string[] lines = path.Split(new char[] { '/' });
				//string[] noPrefab = lines[lines.Length - 1].Split(new char[] { '.' });
				//move._path = lines[lines.Length - 2] + "/" + noPrefab[0];
			}
			_move = move;
			SetAnimationClip(GetAnimationClip());

			if (move != null)
			{
				//EditorPrefs.SetString("MoveEdLastMove", move._path);
				move.SetPendingChanges();
			}

			_currentFrame = 0;

			if (resetCharacter)
				SetPreviewObject(null);

			InitCharacterList();
			InitMoveList();
		}

		private AnimationClip GetAnimationClip()
		{
			return _move != null ? _move._animationClip : null;
		}
        private Animator GetAnimator()
        {
            return _previewGameObject != null ? _previewGameObject.GetComponent<Animator>() : null;
        }

        private Animator GetHitAnimator()
		{
			return _previewHitGameObject != null ? _previewHitGameObject.GetComponent<Animator>() : null;
		}

		private void SetAnimationClip(AnimationClip clip)
		{
			if (_move != null)
			{
				_move._animationClip = clip;
				_numFrames = (clip != null) ? MoveEditorUtils.GetFrameCount(clip) : 1;
				_currentFrame = 0;

				EditorUtility.SetDirty(_move);
				ScrubAnimation();
			}
		}

		private void SetPreviewObject(GameObject preview)
		{
			if (IsValidPreviewObject(preview))
			{
				if (_previewGameObject != null)
				{
					DestroyImmediate(_previewGameObject);
				}

				_previewGameObject = (GameObject)Instantiate(preview);
                _previewGameObject.transform.position = new Vector3(0, 0.25f, -3f);//by pj 适配新的场景模型移动 因为不想增加上面信息数量就不写编辑面板设置offset
                _previewGameObject.name = preview.name;

				if (_previewGameObject != null)
				{
					EditorPrefs.SetString("MoveEdLastCharacter", preview.name);

					Component[] components = _previewGameObject.GetComponents<Component>();
					for (int i = 0; i < components.Length; i++)
					{
						if (components[i] is Transform || components[i] is Animator || components[i] is FXHelper)
							continue;

						if (components[i] is PropsController)
						{
							PropsController pc = (PropsController)components[i];
							pc.Init();

							continue;
						}

						DestroyImmediate(components[i]);
					}

					GetAnimator().enabled = false;

					FlipCharacter(_previewGameObject,_flipped);

					_previewGameObject.AddComponent<HitEventPreview>();
				}
			}
			else if (preview == null)
			{
				if (_previewGameObject != null)
					DestroyImmediate(_previewGameObject);

				_previewGameObject = null;
			}

			InitCharacterList();
			InitMoveList();

			ScrubAnimation();
		}

        private void SetHitPreviewObject(GameObject preview)
        {
            if (IsValidPreviewObject(preview))
            {
                if (_previewHitGameObject != null)
                {
                    DestroyImmediate(_previewHitGameObject);
                }

                _previewHitGameObject = (GameObject)Instantiate(preview);
                _previewHitGameObject.transform.position = new Vector3(0, 0.25f, 3f);//by pj 适配新的场景模型移动 因为不想增加上面信息数量就不写编辑面板设置offset
                _previewHitGameObject.name = preview.name;
                if (_previewHitGameObject != null)
                {
                    EditorPrefs.SetString("MoveEdLastCharacter", preview.name);

                    Component[] components = _previewHitGameObject.GetComponents<Component>();
                    for (int i = 0; i < components.Length; i++)
                    {
                        if (components[i] is Transform || components[i] is Animator || components[i] is FXHelper)
                            continue;

                        if (components[i] is PropsController)
                        {
                            PropsController pc = (PropsController)components[i];
                            pc.Init();

                            continue;
                        }

                        DestroyImmediate(components[i]);
                    }

                    GetHitAnimator().enabled = false;

                    FlipCharacter(_previewHitGameObject,!_flipped);

                    _previewHitGameObject.AddComponent<HitEventPreview>();
                }
            }
            else if (preview == null)
            {
                if (_previewHitGameObject != null)
                    DestroyImmediate(_previewHitGameObject);

                _previewHitGameObject = null;
            }

            //InitCharacterList();
            //InitMoveList();

            //ScrubAnimation();
        }

        // TJ: this is grooooooooss hack to get around a bug in Animator.GetBoneTransform (it returns null pretty often)
        private void ResetAnimatorComponent()
		{
			if (_previewGameObject != null && GetAnimator())
			{
				UnityEditorInternal.ComponentUtility.CopyComponent(GetAnimator());
				DestroyImmediate(GetAnimator());
				UnityEditorInternal.ComponentUtility.PasteComponentAsNew(_previewGameObject);
			}
		}

		private bool IsValidMoveInfoObject(GameObject go)
		{
			if (go != null)
			{
				Move info = go.GetComponent<Move>();
				return info != null;
			}

			return false;
		}

		private bool IsValidPreviewObject(GameObject go)
		{
			if (go != null)
			{
				Animator animator = go.GetComponent<Animator>();
				return animator != null;// fixme(troy) where do we set isHuman? && animator.isHuman;
			}

			return false;
		}

		private void ScrubAnimation()
		{
			if (GetAnimationClip() != null)
			{
				if (_previewGameObject != null)
				{
					if (_previewMode == PreviewMode.ScrubPreview)
					{
						float time = MoveEditorUtils.GetTimeFromFrame(_currentFrame, _numFrames, GetAnimationClip().length);
						PreviewParticleEvents(time);
						PostFxPreviewTrackManager.UpdateTracks(time);
					}

					GetAnimationClip().SampleAnimation(_previewGameObject, MoveEditorUtils.GetTimeFromFrame(_currentFrame, _numFrames, GetAnimationClip().length));

					//Transform root = MoveUtils.GetBodyPartTransform(GetAnimator(), BodyPart.Root);
					//if (root != null)
					//{
					//    FXRootNode fxRoot = root.GetComponent<FXRootNode>();
					//    if (fxRoot != null)
					//    {
					//        fxRoot.UpdatePosition();
					//    }
					//}

					MoveEditor.HitEventInfo hitEvent = _move._hitEvents.Find(x => x._frame == _currentFrame);

					HitEventPreview hitEventPreview = _previewGameObject.GetComponent<HitEventPreview>();
					if (hitEventPreview != null)
					{
						hitEventPreview.Animator = GetAnimator();
						hitEventPreview.HitEvent = hitEvent;
					}
				}
			}
		}


		private void InitPreviewPlayback()
		{
			if (GetAnimationClip() != null)
			{
				_lastPreviewRealTime = Time.realtimeSinceStartup;
				_previewTime = MoveEditorUtils.GetTimeFromFrame(_currentFrame, _numFrames, GetAnimationClip().length);
			}
		}

        private float _previewRealTime;
	    private bool isAudioManagerInit = false;
        private void DoPreviewPlayback()
		{
			if (GetAnimationClip() != null)
			{
				float deltaTime = Time.realtimeSinceStartup - _lastPreviewRealTime;
				_lastPreviewRealTime = Time.realtimeSinceStartup;

				float speedMod = _playbackSpeed * _move._animationSpeedEvent._floatParameter;
				_previewTime += deltaTime * speedMod;

                if (!Application.isPlaying)
                {
                    _previewRealTime += deltaTime * speedMod;
                    MoveCameraShakeHelper.UpdateShakes(_previewRealTime); //by pj为了能预览震屏
                }

				if ((_previewTime > GetAnimationClip().length+_addPreviewTime && ParticleTracksAreFinished()) || (_previewTime > GetAnimationClip().length +1.0f+_addPreviewTime/* CameraShakePreviewTrackManager.GetTracksMaxLengh()*/))
				{
					if (_particlePreviewTracks != null)
					{
						for (int i = 0; i < _particlePreviewTracks.Count; i++)
							_particlePreviewTracks[i].Reset();
					}

					ResetTrailEvents();
					ResetDynamicLightEvents();
					ResetPropEvents();
					ResetProjectileEvents();
                    RestHitEvents();
					CameraShakePreviewTrackManager.ResetAll();

					//CameraMotionPreviewManager.Instance.Reset();

					RenderGlobals.SetEnvironmentAdjustments(Color.gray, Color.black);

					if (_previewGameObject != null)
					{
						PropsController propsController = _previewGameObject.GetComponent<PropsController>();
						if (propsController != null)
							propsController.ResetPropsToDefault();
					}

					_previewTime = 0;
				}

				GetAnimationClip().SampleAnimation(_previewGameObject, _previewTime);

				//Transform root = MoveUtils.GetBodyPartTransform(GetAnimator(), BodyPart.Root);
				//if (root != null)
				//{
				//    FXRootNode fxRoot = root.GetComponent<FXRootNode>();
				//    if (fxRoot != null)
				//    {
				//        fxRoot.UpdatePosition();
				//    }
				//}

				PreviewParticleEvents(_previewTime);
				PreviewTrailEvents(_previewTime);
				PreviewDynamicLightEvents(_previewTime);
				PreviewPropEvents(_previewTime);
				PreviewProjectileEvents(_previewTime);
				PreviewEnvLightingEvents(_previewTime);
                PreviewHitEvents(_previewTime);
                CameraShakePreviewTrackManager.UpdateTracks(_previewTime);
				CameraMotionPreviewManager.Instance.Update(_previewTime, deltaTime * speedMod);
				PostFxPreviewTrackManager.UpdateTracks(_previewTime);

				float previous_frame = _currentFrame;
				if (_smoothFrames)
					_currentFrame = MoveEditorUtils.GetFrameFromTime(_previewTime, _numFrames, GetAnimationClip().length);
				else
					_currentFrame = MoveEditorUtils.GetFrameFromTimeToInt(_previewTime, _numFrames, GetAnimationClip().length);

				// wrap previous frame
				if (previous_frame > _currentFrame)
				{
					previous_frame -= _numFrames;
				}
				// Trigger Audio Events
                AudioManager.Initialize();
				for (int i = 0; i < _move._audioEvents.Count; ++i)
				{
					AudioEventInfo  audio_event_info = _move._audioEvents[i];
					if (audio_event_info._frame > previous_frame && audio_event_info._frame <= _currentFrame)
					{
						//FusionAudio.PostEvent(audio_event_info._audioProperties._event, _previewGameObject, true);
                        Debug.Log("AudioName is : "+ audio_event_info._audioProperties._event);
					    FusionAudio.PlayEventAudioSource(audio_event_info._audioProperties._event, audio_event_info._audioProperties._volumn, true);

					}
				}
			}
		}

		private void UpdateCamera()
		{
			if (_previewGameObject != null)
			{
				Transform mainCamera = Camera.main.transform;
				if (mainCamera != null)
				{
					Vector3 position = MoveUtils.GetBodyPartTransform(GetAnimator(), BodyPart.Root).position;
					position += _previewCameraOffset;

					mainCamera.position = position;
					mainCamera.transform.rotation = Quaternion.identity;
				}
			}
		}

		private bool ParticleTracksAreFinished()
		{
			if (_particlePreviewTracks != null)
			{
				for (int i = 0; i < _particlePreviewTracks.Count; i++)
				{
					if (!_particlePreviewTracks[i].IsFinished())
					{
						return false;
					}
				}
			}

			return true;
		}
        

        private void ResetTrailEvents()
		{
			if (_move != null && _trailPreviewTracks != null)
			{
				for (int i = 0; i < _trailPreviewTracks.Count; i++)
					_trailPreviewTracks[i].Reset();
			}
		}

		private void ResetDynamicLightEvents()
		{
			if (_move != null && _dynamicPreviewTracks != null)
			{
				for (int i = 0; i < _dynamicPreviewTracks.Count; i++)
					_dynamicPreviewTracks[i].Reset();
			}
		}

		private void ResetPropEvents()
		{
			if (_propPreviewTracks != null)
			{
				for (int i = 0; i < _propPreviewTracks.Count; i++)
					_propPreviewTracks[i].Reset();
			}
		}

		private void ResetProjectileEvents()
		{
			if (_projectilePreviewTracks != null)
			{
				for (int i = 0; i < _projectilePreviewTracks.Count; ++i)
				{
					_projectilePreviewTracks[i].Reset();
				}
			}
		}

        private void RestHitEvents()
        {
            if (_hitPreviewTracks != null)
            {
                for (int i = 0; i < _hitPreviewTracks.Count; ++i)
                {
                    _hitPreviewTracks[i].Reset();
                }
            }
        }

		private void PreviewParticleEvents(float time)
		{
			if (_move != null && GetAnimationClip() != null && _previewGameObject != null && _particlePreviewTracks != null)
			{
				Transform selection = Selection.activeTransform;

				for (int i = 0; i < _particlePreviewTracks.Count; i++)
					_particlePreviewTracks[i].Update(time);

				Selection.activeTransform = selection;
			}
		}

		private void PreviewTrailEvents(float time)
		{
			if (_move != null && _trailPreviewTracks != null)
			{
				for (int i = 0; i < _trailPreviewTracks.Count; i++)
					_trailPreviewTracks[i].Update(time);
			}
		}

		private void PreviewDynamicLightEvents(float time)
		{
			if (_move != null && _dynamicPreviewTracks != null)
			{
				for (int i = 0; i < _dynamicPreviewTracks.Count; i++)
				{
					_dynamicPreviewTracks[i].Update(time);
				}
			}
		}

		private void PreviewPropEvents(float time)
		{
			if (_propPreviewTracks != null)
			{
				for (int i = 0; i < _propPreviewTracks.Count; i++)
				{
					_propPreviewTracks[i].Update(time);
				}
			}
		}

		private void PreviewEnvLightingEvents(float time)
		{
			if (_envLightingPreviewTracks != null)
			{
				for (int i = 0; i < _envLightingPreviewTracks.Count; i++)
				{
					_envLightingPreviewTracks[i].Update(time);
				}
			}
		}

		private void PreviewProjectileEvents(float time)
		{
			if (_projectilePreviewTracks != null)
			{
				for (int i = 0; i < _projectilePreviewTracks.Count; i++)
				{
					_projectilePreviewTracks[i].Update(time);
				}
			}
		}
        private void PreviewHitEvents(float time)
        {
            if (_hitPreviewTracks != null)
            {
                for (int i = 0; i < _hitPreviewTracks.Count; i++)
                {
                    _hitPreviewTracks[i].Update(time);
                }
            }
        }


		private void InitTrailPreviewTracks()
		{
			CleanUpTrailPreviewObjects();

			if (_move != null)
			{
				_trailPreviewTracks = new List<TrailPreview>();

				foreach (TrailRendererEventInfo e in _move._trailRendererEvents)
				{
					TrailPreview preview = new TrailPreview();
					float startTime =  MoveEditorUtils.GetTimeFromFrame(e._frame, _numFrames, GetAnimationClip().length);
					preview.Init(e._trailRendererProperties, GetAnimator(), startTime, _flipped, e._isEditCrit);
					_trailPreviewTracks.Add(preview);
				}
			}
		}

		private void InitDynamicLightPreviewTracks()
		{
			CleanUpDynamicLightPreviewObjects();
			if (_move != null)
			{
				_dynamicPreviewTracks = new List<DynamicLightPreview>();
				foreach (DynamicLightEventInfo e in _move._dynamicLightEvents)
				{
					DynamicLightPreview preview = new DynamicLightPreview();
					float startTime =  MoveEditorUtils.GetTimeFromFrame(e._frame, _numFrames, GetAnimationClip().length);
					float stopTime = -1;
					for (int i = 0; i < _move._genericEvents.Count; i++)
					{
						if (_move._genericEvents[i]._functionName == "OnStopDynamicLight" && _move._genericEvents[i]._stringParameter == e._dynamicLightProperties._eventName)
							stopTime = MoveEditorUtils.GetTimeFromFrame(_move._genericEvents[i]._frame, _numFrames, GetAnimationClip().length);
					}

					preview.Init(e._dynamicLightProperties, GetAnimator(), startTime, stopTime);
					_dynamicPreviewTracks.Add(preview);
				}
			}
		}
		private void InitParticlePreviewTracks()
		{
			CleanUpParticlePreviewTracks();

			if (_move != null && GetAnimationClip() != null && _previewGameObject != null)
			{
				_particlePreviewTracks = new List<ParticlePreview>();
				foreach (ParticleEventInfo e in _move._particleEvents)
				{
					if (e._particleProperties._particleReference.Valiad && string.IsNullOrEmpty(e._particleProperties._partition))
					{
						ParticlePreview preview = InitParticlePreviewTrack(e._particleProperties, e._frame);
						if (e._particleProperties._spawnAtHitPoint)
						{
							preview.SetAtHitPoint(GetHitPoint(e._frame));
						}
						// fixme(troy) - add support for properties._spawnAtTargetBase
						_particlePreviewTracks.Add(preview);
					}
				}

                //foreach (HitEventInfo e in _move._hitEvents)   //by pj 不再需要再编辑演示的时候让受击粒子生成
                //{
                //    if (e._particleProperties._particleReference.Valiad)
                //    {
                //        ParticlePreview preview = InitParticlePreviewTrack(e._particleProperties, e._frame);
                //        if (e._particleProperties._spawnAtHitPoint)
                //        {
                //            preview.SetAtHitPoint(GetHitPoint(e._frame));
                //        }
                //        _particlePreviewTracks.Add(preview);
                //    }
                //}

                ScrubAnimation();
			}
		}

		private ParticlePreview InitParticlePreviewTrack(ParticleEventProperties properties, float frame)
		{
			ParticlePreview preview = new ParticlePreview();
			if (_previewGameObject != null)
			{
				float time = MoveEditorUtils.GetTimeFromFrame(frame, _numFrames, GetAnimationClip().length);
				GetAnimationClip().SampleAnimation(_previewGameObject, time);

				float stopTime = -1;
				bool clear = false;

				for (int i = 0; i < _move._genericEvents.Count; i++)
				{
					if (_move._genericEvents[i]._functionName == "OnStopParticle" && _move._genericEvents[i]._stringParameter == properties._eventName)
					{
						stopTime = MoveEditorUtils.GetTimeFromFrame(_move._genericEvents[i]._frame, _numFrames, GetAnimationClip().length);
					}
					else if (_move._genericEvents[i]._functionName == "OnClearParticle" && _move._genericEvents[i]._stringParameter == properties._eventName)
					{
						stopTime = MoveEditorUtils.GetTimeFromFrame(_move._genericEvents[i]._frame, _numFrames, GetAnimationClip().length);
						clear = true;
					}
				}

				preview.Init(properties, GetAnimator(), time, _flipped, stopTime, clear);
			}

			return preview;
		}

		private void InitPropPreviewTracks()
		{
			CleanUpPropPreviewTracks();

			if (_move != null && GetAnimationClip() != null && _previewGameObject != null)
			{
				PropsController propsController = _previewGameObject.GetComponent<PropsController>();
				if (propsController != null)
					propsController.Init();

				_propPreviewTracks = new List<PropPreview>();
				PropPreview preview;

				for (int i = 0; i < _move._genericEvents.Count; i++)
				{
					if (_move._genericEvents[i]._functionName == "EnableProp")
					{
						preview = new PropPreview();
						float time = MoveEditorUtils.GetTimeFromFrame(_move._genericEvents[i]._frame, _numFrames, GetAnimationClip().length);
						preview.Init(_previewGameObject.GetComponent<PropsController>(), _move._genericEvents[i]._stringParameter, true, time);
						_propPreviewTracks.Add(preview);
					}
					else if (_move._genericEvents[i]._functionName == "DisableProp")
					{
						preview = new PropPreview();
						float time = MoveEditorUtils.GetTimeFromFrame(_move._genericEvents[i]._frame, _numFrames, GetAnimationClip().length);
						preview.Init(_previewGameObject.GetComponent<PropsController>(), _move._genericEvents[i]._stringParameter, false, time);
						_propPreviewTracks.Add(preview);
					}
				}
			}
		}

		private void InitProjectilePreviewTracks()
		{
			CleanUpProjectilePreviewTracks();

			_projectilePreviewTracks = new List<ProjectilePreview>();

			if (_move != null && GetAnimationClip() != null && _previewGameObject != null)
			{
				for (int i = 0; i < _move._projectileEvents.Count; ++i)
				{
					ProjectilePreview preview = new ProjectilePreview();
					float time = MoveEditorUtils.GetTimeFromFrame(_move._projectileEvents[i]._frame, _numFrames, GetAnimationClip().length);
					preview.Init(time, _move._projectileEvents[i], _previewGameObject,_previewHitGameObject, _flipped);
					_projectilePreviewTracks.Add(preview);
				}
			}
		}

        private void InitHitPreviewTracks()
        {
            CleanUpHitPreviewTracks();

            _hitPreviewTracks = new List<HitPreview>();

            if (_move != null && GetAnimationClip() != null && _previewGameObject != null)
            {
                for (int i = 0; i < _move._hitEvents.Count; ++i)
                {
                    HitPreview preview = new HitPreview();
                    float time = MoveEditorUtils.GetTimeFromFrame(_move._hitEvents[i]._frame, _numFrames, GetAnimationClip().length);
                    preview.Init(time, _move._hitEvents[i], _previewGameObject, _previewHitGameObject, _flipped);
                    _hitPreviewTracks.Add(preview);
                }
            }
        }

		private void InitEnvLightingPreviewTracks()
		{
			if (_move != null && GetAnimationClip() != null)
			{
				_envLightingPreviewTracks = new List<EnvLightingPreview>();
				for (int i = 0; i < _move._environmentLightingEvents.Count; i++)
				{
					EnvLightingPreview preview = new EnvLightingPreview();
					preview.Init(_move._environmentLightingEvents[i]._lightingProperties, MoveEditorUtils.GetTimeFromFrame(_move._environmentLightingEvents[i]._frame, _move.NumFrames, GetAnimationClip().length));
					_envLightingPreviewTracks.Add(preview);
				}
			}
		}

		private void InitPostFxPreviewTracks()
		{
			PostFxPreviewTrackManager.Init();

			for (int i = 0; i < _move._postBloomEvents.Count; i++)
			{
				float time = MoveEditorUtils.GetTimeFromFrame(_move._postBloomEvents[i]._frame, _move.NumFrames, _move._animationClip.length);
				PostFxPreviewTrackManager.RegisterTrack<PostBloomEventInfo>(_move._postBloomEvents[i], time);
			}

			for (int i = 0; i < _move._postVignetteEvents.Count; i++)
			{
				float time = MoveEditorUtils.GetTimeFromFrame(_move._postVignetteEvents[i]._frame, _move.NumFrames, _move._animationClip.length);
				PostFxPreviewTrackManager.RegisterTrack<PostVignetteEventInfo>(_move._postVignetteEvents[i], time);
			}

			for (int i = 0; i < _move._postWarpEvents.Count; i++)
			{
				float time = MoveEditorUtils.GetTimeFromFrame(_move._postWarpEvents[i]._frame, _move.NumFrames, _move._animationClip.length);
				PostFxPreviewTrackManager.RegisterTrack<PostWarpEventInfo>(_move._postWarpEvents[i], time);
			}
		}

		private void InitCameraShakePreviewTracks()
		{
			CameraShakePreviewTrackManager.Init();
			for (int i =0; i < _move._cameraShakeEvents.Count; i++)
			{
				float time = MoveEditorUtils.GetTimeFromFrame(_move._cameraShakeEvents[i]._frame, _move.NumFrames, _move._animationClip.length);
				CameraShakePreviewTrackManager.RegisterTrack(_move._cameraShakeEvents[i], time);
			}
		}

		private void InitCameraMotionPreviews()
		{
			CameraMotionPreviewManager.Instance.Init(_previewGameObject,_previewHitGameObject);
			for (int i = 0; i < _move._cameraMotionEvents.Count; i++)
			{
				float time = MoveEditorUtils.GetTimeFromFrame(_move._cameraMotionEvents[i]._frame, _move.NumFrames, _move._animationClip.length);
				CameraMotionPreviewManager.Instance.RegisterPreview(_move._cameraMotionEvents[i], time);
			}
		}

		private void CleanUpProjectilePreviewTracks()
		{
			if (_projectilePreviewTracks != null)
			{
				foreach (ProjectilePreview p in _projectilePreviewTracks)
					p.Cleanup();

				_projectilePreviewTracks.Clear();
			}
		}

        private void CleanUpHitPreviewTracks()
        {
            if (_hitPreviewTracks != null)
            {
                foreach (HitPreview p in _hitPreviewTracks)
                    p.Cleanup();

                _hitPreviewTracks.Clear();
            }
        }

		private void CleanUpPreviewParticle()
		{
			if (_previewParticleSystem != null)
			{
				DestroyImmediate(_previewParticleSystem.gameObject);
			}
		}

		private void CleanUpDynamicLightPreview()
		{
			if (_previewDynamicLight != null)
			{
				DestroyImmediate(_previewDynamicLight.gameObject);
			}
		}

		private void CleanUpHitOffsetLocator()
		{
			if (_hitOffsetLocator != null)
				DestroyImmediate(_hitOffsetLocator);
		}

		private void CleanUpTrailLocator()
		{
			if (_trailLocator != null)
				DestroyImmediate(_trailLocator);
		}

		private void CleanUpProjectileSpawnLocator()
		{
			if (_projectileSpawnLocator != null)
				DestroyImmediate(_projectileSpawnLocator);
		}

		private void CleanUpParticlePreviewTracks()
		{
			if (_particlePreviewTracks != null)
			{
				foreach (ParticlePreview p in _particlePreviewTracks)
					p.Cleanup();

				_particlePreviewTracks.Clear();
			}
		}

		private void CleanUpTrailPreviewObjects()
		{
			if (_trailPreviewTracks != null)
			{
				foreach (TrailPreview tp in _trailPreviewTracks)
					tp.Cleanup();

				_trailPreviewTracks.Clear();
			}
		}

		private void CleanUpDynamicLightPreviewObjects()
		{
			if (_dynamicPreviewTracks != null)
			{
				foreach (DynamicLightPreview dlp in _dynamicPreviewTracks)
					dlp.Cleanup();

				_dynamicPreviewTracks.Clear();
			}

			if (DynamicPointLightManager.Instance != null)
			{
				DynamicPointLightManager.Instance.DeRegisterAll();
			}
		}

		private void CleanUpEnvLightingPrewiewObjects()
		{
			if (_envLightingPreviewTracks != null)
			{
				_dynamicPreviewTracks.Clear();
			}
			RenderGlobals.SetEnvironmentAdjustments(Color.gray, Color.black);
		}

		private void CleanUpPropPreviewTracks()
		{
			if (_propPreviewTracks != null)
			{
				_propPreviewTracks.Clear();
			}
		}

		private void CleanUpPostFxPreviewTracks()
		{
			PostFxPreviewTrackManager.DeregisterAll();
		}

		private void CleanUpCameraShakePreviewTracks()
		{
			CameraShakePreviewTrackManager.DeregisterAll();
		}

		private void CleanUpCameraMotionPreviews()
		{
            CameraMotionPreviewManager.Instance.DeregisterAll();
        }

        private ParticlePreview GetParticlePreview(ParticleEventProperties properties)
		{
			if (_particlePreviewTracks != null)
			{
				return _particlePreviewTracks.Find(delegate(ParticlePreview p) { return p.ParticleProperties == properties; }); ;
			}

			return null;
		}

		private void SwitchPreviewMode(PreviewMode mode)
		{
			OnExitPreviewMode(_previewMode);
			OnEnterPreviewMove(mode);

			if (_previewGameObject != null)
			{
				PropsController propsController = _previewGameObject.GetComponent<PropsController>();
				if (propsController != null)
					propsController.Init();
			}

			_previewMode = mode;
		}

		private void OnExitPreviewMode(PreviewMode mode)
		{
			switch (mode)
			{
				case PreviewMode.Edit:
					CleanUpPreviewParticle();
					CleanUpDynamicLightPreview();
					CleanUpTrailLocator();
					CleanUpHitOffsetLocator();
					CleanUpProjectileSpawnLocator();
					CleanUpTrailPreviewObjects();
					CleanUpDynamicLightPreviewObjects();
					CleanUpProjectilePreviewTracks();
                    CleanUpHitPreviewTracks();
					break;
				default:
					CleanUpParticlePreviewTracks();
					CleanUpTrailPreviewObjects();
					CleanUpDynamicLightPreviewObjects();
					CleanUpPropPreviewTracks();
					CleanUpProjectilePreviewTracks();
					CleanUpEnvLightingPrewiewObjects();
					CleanUpPostFxPreviewTracks();
					CleanUpCameraShakePreviewTracks();
                    CleanUpHitPreviewTracks();
                    CleanUpCameraMotionPreviews();
                    if (DynamicPointLightManager.Instance != null)
					{
						DynamicPointLightManager.Instance.DeRegisterAll();
						DynamicPointLightManager.Instance.Sim();
					}
                    if (!Application.isPlaying) //by pj 预览模式增加震屏 需要手动注销
                    {
                        if (Camera.main != null)
                        {
                            Thinksquirrel.CShake.CameraShake[] shakes = Camera.main.GetComponents<Thinksquirrel.CShake.CameraShake>();
                            for (int i = 0; i < shakes.Length; i++)
                            {
                                shakes[i].OnDisable();
                                for(int j=0;j< shakes[i].listDoShake.Count;j++)
                                {
                                    shakes[i].listDoShake[j].Clear();
                                }
                                shakes[i].listDoShake.Clear();
                            }
                        }
                    }
                    break;
			}
		}

        private void IdleCamera()
        {
            if (Camera.main != null)
            {
                CombatCamera combat_camera = Camera.main.GetComponent<CombatCamera>();

                if (combat_camera != null)
                {
                    combat_camera.OnIdleCamera();
                }
            }
        }

		private void OnEnterPreviewMove(PreviewMode mode)
		{
			switch (mode)
			{
				case PreviewMode.ScrubPreview:
					InitParticlePreviewTracks();
					InitPostFxPreviewTracks();
                    InitCameraMotionPreviews();
                    ScrubAnimation();
                    if(!Application.isPlaying) //因为镜头改为自由视角 所以在编辑模式需要手动调用镜头归位函数
                    {
                        IdleCamera();
                    }
					break;
				case PreviewMode.PlaybackPreview:
					_currentFrame = 0;
					DynamicPointLightManager.InitializeInstance();
					InitPreviewPlayback();
					InitParticlePreviewTracks();
					InitTrailPreviewTracks();
					InitEnvLightingPreviewTracks();
					InitPostFxPreviewTracks();
					InitDynamicLightPreviewTracks();
					InitPropPreviewTracks();
					InitProjectilePreviewTracks();
					InitCameraShakePreviewTracks();
					InitCameraMotionPreviews();
                    InitHitPreviewTracks();

                    if (!Application.isPlaying)
					{
						// Simon's code to get at the game view window, so that we can do the environment lighting shit
						System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
						System.Type type = assembly.GetType("UnityEditor.GameView");
						EditorWindow gameview = EditorWindow.GetWindow(type);
						gameview.autoRepaintOnSceneChange = true;
                        if(Camera.main !=null) //by pj 预览模式增加震屏
                        {
                            Thinksquirrel.CShake.CameraShake[] shakes = Camera.main.GetComponents<Thinksquirrel.CShake.CameraShake>();
                            for(int i=0;i< shakes.Length;i++)
                            {
                                shakes[i].OnEnable();
                            }
                        }

                        IdleCamera();
                    }
					break;
			}
		}

		public static void InitCameraShakePresets(SortedDictionary<string, CameraShakePreset> outPresets)
		{
			outPresets.Clear();
			outPresets.Add("(New)", null);

			if (System.IO.Directory.Exists(MoveEditorUtils.GetCameraShakePresetsDirectoryPath()))
			{
				string[] files = System.IO.Directory.GetFiles(MoveEditorUtils.GetCameraShakePresetsDirectoryPath(), "*.prefab");
				for (int i = 0; i < files.Length; i++)
				{
					string file = files[i].Substring(files[i].IndexOf("Assets"));
					CameraShakePreset preset = (CameraShakePreset)AssetDatabase.LoadAssetAtPath(file, typeof(CameraShakePreset));
					if (preset != null)
						outPresets.Add(preset.name, preset);
				}
			}
		}

		private void InitMoveList()
		{
			_moveNames.Clear();
			_movePaths.Clear();
			_moveShowNames = null;

			_moveNames.Add("[null]");
			_movePaths.Add("");

			string[] names = null;

			if (_previewGameObject != null)
				names = MoveEditorUtils.GetMoveListFromCharacter(_previewGameObject.name);

			if (names == null || names.Length == 0)
				names = MoveEditorUtils.GetAllMoveNames();

			if (names != null)
			{
				System.Array.Sort<string>(names);

				_moveNames.AddRange(names);

				string path = MoveEditorUtils.GetMovesDirectoryPath();
				path = path.Substring(path.IndexOf("Assets"));

				for (int i = 0; i < names.Length; i++)
					_movePaths.Add(string.Format("{0}{1}.prefab", path, names[i]));
			}

			_moveShowNames = _moveNames.Select(n => n == "[null]" ? n : n.Substring(0, 1) + "/" + n).ToArray();
		}

		private void InitCharacterList()
		{
			_characterNames.Clear();
			_characterPaths.Clear();
			_characterShowNames = null;

			_characterNames.Add("[null]");
			_characterPaths.Add("");

			string[] names = null;

			if (_move != null)
				names = MoveEditorUtils.GetCharacterListFromMove(_move.name);

			if (names == null || names.Length == 0)
				names = MoveEditorUtils.GetAllCharacterNames();

			if (names != null)
			{
				System.Array.Sort<string>(names);

				_characterNames.AddRange(names);

				string path = MoveEditorUtils.GetCharacterDirectoryPath();
				path = path.Substring(path.IndexOf("Assets"));

				for (int i = 0; i < names.Length; i++)
					_characterPaths.Add(string.Format("{0}{1}.prefab", path, names[i]));
			}
			_characterShowNames = _characterNames.Select(c => c == "[null]" ? c : c.Substring(0, 1) + "/" + c).ToArray();
		}

		private static CameraShakePreset LoadCameraShakePreset(string name, SortedDictionary<string, CameraShakePreset> cameraShakePresets)
		{
			if (cameraShakePresets.ContainsKey(name))
			{
				return cameraShakePresets[name];
			}
			else
			{
				return null;
			}
		}

		private static void SetCameraShakePropertiesFromPreset(CameraShakePreset preset, ref CameraShakeEventProperties properties)
		{
			if (preset != null && properties != null)
			{
				properties._numberOfShakes = preset._numberOfShakes;
				properties._shakeAmount = preset._shakeAmount;
				properties._rotationAmount = preset._rotationAmount;
				properties._distance = preset._distance;
				properties._speed = preset._speed;
				properties._decay = preset._decay;
				properties._multiplyByTimeScale = preset._multiplyByTimeScale;
			}
		}

		private static void SaveCameraShakePreset(CameraShakePreset preset, CameraShakeEventProperties properties, SortedDictionary<string, CameraShakePreset> cameraShakePresets)
		{
			if (preset == null)
			{
				if (string.IsNullOrEmpty(properties._presetName))
				{
					Debug.LogWarning("MoveEditorWindow: You must set a name for a new Camera Shake Preset before saving it.");
					return;
				}

				GameObject go = new GameObject(properties._presetName);
				go.AddComponent<CameraShakePreset>();

				if (!System.IO.Directory.Exists(MoveEditorUtils.GetCameraShakePresetsDirectoryPath()))
					System.IO.Directory.CreateDirectory(MoveEditorUtils.GetCameraShakePresetsDirectoryPath());

				GameObject prefab = PrefabUtility.CreatePrefab(MoveEditorUtils.GetCameraShakePresetPrefabPath(properties._presetName), go);
				DestroyImmediate(go);

				preset = prefab.GetComponent<CameraShakePreset>();
			}

			preset._numberOfShakes = properties._numberOfShakes;
			preset._shakeAmount = properties._shakeAmount;
			preset._rotationAmount = properties._rotationAmount;
			preset._distance = properties._distance;
			preset._speed = properties._speed;
			preset._decay = properties._decay;
			preset._multiplyByTimeScale = properties._multiplyByTimeScale;

			EditorUtility.SetDirty(preset);
			InitCameraShakePresets(cameraShakePresets);
		}

		private void CheckApplicationState()
		{
			if (Application.isPlaying != EditorPrefs.GetBool("MoveEdApplicationPlaying"))
				ApplicationStateChanged();
		}

		private void ApplicationStateChanged()
		{
			if (DynamicPointLightManager.IsInitialized())
			{
				DynamicPointLightManager.Instance.Clear();
			}

			_updateCameraPosition = false;

			EditorPrefs.SetBool("MoveEdApplicationPlaying", Application.isPlaying);
			SwitchPreviewMode(PreviewMode.Edit);

			Repaint();
		}

		private void FlipCharacter(GameObject preview, bool flipped)
		{
			if (preview != null)
			{
				_flipped = flipped;

                preview.transform.rotation = Quaternion.identity;
				GameObject objRef = EB.Util.GetObjectExactMatch(preview, "Reference");

				if (_flipped)
				{
                    preview.transform.rotation = Quaternion.Euler(0, -180, 0);

					if (objRef != null)
					{
						objRef.transform.localScale = new Vector3(-1, 1, 1);
					}
				}
				else
				{
                    preview.transform.rotation = Quaternion.Euler(0, 0, 0);

					if (objRef != null)
					{
						objRef.transform.localScale = new Vector3(1, 1, 1);
					}
				}
			}
		}

		private Vector3 GetHitPoint(float frame)
		{
			HitEventInfo closestEvent = null;
			for (int i = 0; i < _move._hitEvents.Count; i++)
			{
				float hitFrame = _move._hitEvents[i]._frame;

				if (Mathf.Approximately(hitFrame, frame))
				{
					closestEvent = _move._hitEvents[i];
					break;
				}
				else if (hitFrame < frame)
				{
					if (closestEvent == null || hitFrame > closestEvent._frame)
					{
						closestEvent = _move._hitEvents[i];
					}
				}
			}

			if (closestEvent != null)
			{
				DamageEventProperties properties = closestEvent._damageProperties;
				Transform bodyPartTransform = MoveEditor.MoveUtils.GetBodyPartTransform(GetAnimator(), properties._pointOfOrigin);

				if (bodyPartTransform != null)
				{
					_move._animationClip.SampleAnimation(_previewGameObject, MoveEditorUtils.GetTimeFromFrame(frame, _move.NumFrames, _move._animationClip.length));

					float rotAngle = _previewGameObject.transform.forward.x > 0.0f ? properties._angle : properties._angle * -1.0f;
					Vector3 origin = bodyPartTransform.TransformPoint(properties._offset);
					Vector3 direction = Quaternion.Euler(0.0f, 0.0f, rotAngle) * _previewGameObject.transform.forward;
					float range = properties._range;

					//LayerMask mask = LayerMask.NameToLayer("Character");

					RaycastHit hit = new RaycastHit();
					Ray ray = new Ray(origin, direction);

					if (Physics.Raycast(ray, out hit, range))
					{
						return hit.point;
					}
					else
					{
						return origin + direction * range;
					}
				}
			}

			return Vector3.zero;
		}

		private enum PreviewMode
		{
			Edit,
			ScrubPreview,
			PlaybackPreview,
		}

		// Core Properties
		private Move 						_move						= null;
		private GameObject					_previewGameObject			= null;
        private GameObject                  _previewHitGameObject       = null;
        private ParticleSystem				_previewParticleSystem		= null;
		private GameObject					_previewDynamicLight		= null;
		private GameObject					_hitOffsetLocator			= null;
		private GameObject					_trailLocator				= null;
		private GameObject					_projectileSpawnLocator		= null;
		private int							_numFrames					= 1;
		private float 						_currentFrame				= 0;
        private float                       _addPreviewTime             = 0;

		// Display Properties
		private static bool					_smoothFrames				= false;
		private static Vector2				_scrollPosition				= Vector2.zero;
		private static float				_playbackSpeed 				= 1.0f;

		// Preview Properties
		private bool						_flipped					= false;
		private PreviewMode					_previewMode				= PreviewMode.Edit;
		private List<ParticlePreview>		_particlePreviewTracks		= null;
		private List<TrailPreview>			_trailPreviewTracks			= null;
		private List<DynamicLightPreview>	_dynamicPreviewTracks		= null;
		private List<PropPreview>			_propPreviewTracks			= null;
		private List<ProjectilePreview> 	_projectilePreviewTracks 	= null;
		private List<EnvLightingPreview>	_envLightingPreviewTracks	= null;
        private List<HitPreview>            _hitPreviewTracks           = null;
        private float						_lastPreviewRealTime		= 0;
		private float						_previewTime				= 0;

		// our camera shake presets
		private SortedDictionary<string, CameraShakePreset> _cameraShakePresets = new SortedDictionary<string, CameraShakePreset>();

		private bool 						_showHitEventMetrics		= false;
		private bool						_updateCameraPosition		= false;
		private Vector3						_previewCameraOffset		= new Vector3(0, 1, -5);
		//private bool						_showRangeHelper			= false;
		//private BodyPart					_rangeHelperBodyPart		= BodyPart.RightHand;

		// move info - to load moves
		private List<string>				_moveNames					= new List<string>();
		private List<string>				_movePaths					= new List<string>();
		private string[]					_moveShowNames              = null;

		// character info - to load characters
		private List<string>				_characterNames 			= new List<string>();
		private string[]	                _characterShowNames         = null;
		private List<string>				_characterPaths 			= new List<string>();
	}
}
