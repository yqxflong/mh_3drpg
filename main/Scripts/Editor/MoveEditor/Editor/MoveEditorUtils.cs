using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace MoveEditor
{
	public static class MoveEditorUtils 
	{
#if UNITY_EDITOR
		[MenuItem("G&M/Moves/Create New Move")]
		public static NewMoveEditorWindow ShowNewMoveEditorWindow()
		{
			NewMoveEditorWindow window = EditorWindow.GetWindow<NewMoveEditorWindow>();
			window.titleContent.text = "Create Move";
			return window;
		}
		
		[MenuItem("G&M/Moves/Move Editor")]
		public static MoveEditorWindow ShowMoveEditorWindow()
		{
			MoveEditorWindow window = EditorWindow.GetWindow<MoveEditorWindow>("Move Editor",true);
			return window;
		}

		[MenuItem("G&M/Moves/Projectile Editor")]
		public static ProjectileEditorWindow ShowProjectileEditorWindow()
		{
			ProjectileEditorWindow window = EditorWindow.GetWindow<ProjectileEditorWindow>("Projectile Editor", true);
			return window;
		}
		
		[MenuItem("G&M/Moves/Build/Build Changed Moves")]
		public static void BuildChangedMoves()
		{
			BuildMoves(false);
			WriteDatabase();
			EditorUtility.DisplayDialog("Complete", "Finished building moves.", "OK");
		}

		[MenuItem("G&M/Moves/Build/Build All Moves")]
		public static void BuildAllMoves()
		{
			if (EditorUtility.DisplayDialog("Build All Moves?", "Are you sure you want to build ALL moves? This may take a few minutes.", "Yes", "FUCK NO! ABORT!"))
			{
				BuildMoves(true);
				WriteDatabase();
				EditorUtility.DisplayDialog("Complete", "Finished building moves.", "OK");
			}
		}

		[MenuItem("G&M/Moves/Utils/Compile Move Database")]
		public static void CompileMoveDatabase()
		{
			WriteDatabase();
			WriteCharacterMoveDB();
		}

		[MenuItem("G&M/Moves/Utils/Find Animations Without Moves")]
		public static void PrintAnimationsWithoutMoves()
		{
			AnimationClip[] clips = GetAnimationsWithoutMoves();
			if (clips.Length == 0)
			{
				Debug.LogWarning("GOOD NEWS, EVERYONE!! Moves have been created for ALL ANIMATIONS used in gameplay!");
			}
			else
			{
				for (int i = 0; i < clips.Length; i++)
				{
					Debug.LogWarning("No Move Found For Anim: " + clips[i].name, clips[i]);
				}
			}
		}

		[MenuItem("G&M/Moves/Utils/Find Unused Moves")]
		public static void PrintUnusedMoves()
		{
			Move[] moves = GetUnusedMoves();
			if (moves.Length == 0)
			{
				Debug.LogWarning("GOOD NEWS, EVERYONE!! ALL MOVES are being used in gameplay!");
			}
			else
			{
				for (int i = 0; i < moves.Length; i++)
				{
					Debug.LogWarning("Move is unused: " + moves[i].name, moves[i]);
				}
			}
		}

		[MenuItem("G&M/Moves/Utils/Compile Individual FX Lib")]
		public static void DoCompileFxLibForCharacter()
		{
			ListViewWindow.Open("Characters", GetAllCharacterNames(), CompileFxLibForCharacter, false);
		}

		[MenuItem("G&M/Moves/Utils/Compile ALL FX Libs")]
		public static void DoCompileAllFxLibs()
		{
			CompileAllFxLibs();
		}
		
		private static bool _hackMetaData = true;
		private static bool _writeAnimationEventsToMove = true;
		
		public static void BuildMove(Move move)
		{
			BuildSingleMove(move);

			AssetDatabase.SaveAssets();
		}
		
		public static void BuildMoves(bool buildUnchanged)
		{
			Move[] moves = GetAllMoves();
			for (int i = 0; i < moves.Length; i++)
			{
				EditorUtility.DisplayProgressBar("Building Moves", string.Format("Please wait while moves are built ({0}/{1}).", i + 1, moves.Length), (float)i / (moves.Length - 1));

				if (buildUnchanged || moves[i].HasChangesPending())
				{
					BuildSingleMove(moves[i]);
				}
			}

			EditorUtility.ClearProgressBar();
			AssetDatabase.SaveAssets();
		}
		
		private static void BuildSingleMove(Move move)
		{
			move._animationEvents = null;
			if(_writeAnimationEventsToMove)
			{
				RemoveAllAnimationEvents(move);
				WriteAnimationEventsToMove(move);
			}
			else
			{
				if (_hackMetaData)
					WriteAllAnimationEvents(move);
				else
					SetAllAnimationEvents(move);
			}
			//move.isLooping = move._animationClip != null && move._animationClip.isLooping;
			BuildAnimationCurves(move);
		}

		public static void BuildAnimationCurves(Move move)
		{
			GameObject movePrefab = move._animationObject.Value as GameObject;
			if (movePrefab)
			{
				GameObject dude = (GameObject)GameObject.Instantiate(movePrefab, Vector3.zero, Quaternion.identity);
				if (string.IsNullOrEmpty(move._motionTransform))
				{
					move._follow = dude.transform.GetChild(0);
				}
				else
				{
					move._follow = dude.transform.Find(move._motionTransform);
				}

				if (move._follow == null)
				{
					move._follow = dude.transform.Find("Root");
				}
				if (move._follow == null)
				{
					EB.Debug.LogError("Can't find root motion transform");
				}

				move._toAttackDist = null;
				move._toReturnDist = null;
				move._toEndDist = null;
				move._toAttack = null;
				move._toReturn = null;
				move._toFrameVsDistanceToTarget = null;
				move._zOffsetCurve = null;
				move._yOffsetCurve = null;
				move.pos = null;
				move.rot = null;

				CalculateSkillAnimCurve(move, dude);
				CalculateReactionAnimCurve(move, dude);
				CalculateMoveAnimCurve(move, dude);

				move._follow = null;
				GameObject.DestroyImmediate(dude);
			}
		}

		private static void CalculateMoveAnimCurve(Move move, GameObject go)
		{
			if (move == null || go == null)
			{
				return;
			}

			if (move._moveState != MoveController.CombatantMoveState.kForward 
				&& move._moveState != MoveController.CombatantMoveState.kBackward)
			{
				return;
			}

			move._zOffsetCurve = new AnimationCurve();

			AnimationClip moveAnim = move._animationClip;
			float totalFrames = moveAnim.length * moveAnim.frameRate;

			move._animationClip.SampleAnimation(go, 0);
			Vector3 originalPosition = move._follow.position;
			Vector3 positionOffset = go.transform.position - originalPosition;
			Quaternion rotationOffset = Quaternion.Inverse(move._follow.rotation) * go.transform.rotation;

			int frames = (int)System.Math.Floor(move._animationClip.length * move._animationClip.frameRate);
			Debug.Log("Frame Count " + frames);
			move.rot = new Quaternion[frames + 1];
			move.pos = new Vector3[frames + 1];
			for (int i = 0; i <= frames; i++)
			{
				move._animationClip.SampleAnimation(go, i / totalFrames * moveAnim.length);

				move._zOffsetCurve.AddKey(i, move._follow.position.z - originalPosition.z);

				Debug.Log("Add Frame " + i);
				move.rot[i] = move._follow.rotation * rotationOffset;
				move.pos[i] = move._follow.position + positionOffset;
			}
		}

		private static void CalculateReactionAnimCurve(Move move, GameObject go)
		{
			if (move == null || go == null)
			{
				return;
			}

			if (move._moveState != MoveController.CombatantMoveState.kHitReaction)
			{
				return;
			}

			move._zOffsetCurve = new AnimationCurve();
			move._yOffsetCurve = new AnimationCurve();

			AnimationClip moveAnim = move._animationClip;
			float totalFrames = moveAnim.length * moveAnim.frameRate;

			move._animationClip.SampleAnimation(go, 0);
			Vector3 originalPosition = move._follow.position;
			Vector3 positionOffset = go.transform.position - originalPosition;
			Quaternion rotationOffset = Quaternion.Inverse(move._follow.rotation) * go.transform.rotation;

			int frames = (int)System.Math.Floor(move._animationClip.length * move._animationClip.frameRate);
			Debug.Log("Frame Count " + frames);
			move.rot = new Quaternion[frames + 1];
			move.pos = new Vector3[frames + 1];
			for (int i = 0; i <= frames; i++)
			{
				move._animationClip.SampleAnimation(go, i / totalFrames * moveAnim.length);

				move._zOffsetCurve.AddKey(i, move._follow.position.z - originalPosition.z);
				move._yOffsetCurve.AddKey(i, move._follow.position.y - originalPosition.y);

				Debug.Log("Add Frame " + i);
				move.rot[i] = move._follow.rotation * rotationOffset;
				move.pos[i] = move._follow.position + positionOffset;
			}
		}

		private static void CalculateSkillAnimCurve(Move move, GameObject go)
		{
			if (move == null || go == null)
			{
				return;
			}

			if (move._moveState != MoveController.CombatantMoveState.kAttackTarget)
			{
				return;
			}

			int[] frames = move.GetCombatSkillTriggerFrames();
			if (frames != null)
			{
				CalculateAnimCurve(frames[0], frames[1], frames[2], frames[3], move, go);
			}
		}

		private static void CalculateAnimCurve(int startRunFrame, int endRunFrame, int startReturnFrame, int endReturnFrame, Move move, GameObject go)
		{
			if (go == null) {
				return;
			}

			move._toAttackDist = new AnimationCurve();
			move._toReturnDist = new AnimationCurve();
			move._toAttack = new AnimationCurve();
			move._toReturn = new AnimationCurve();
			move._toEndDist = new AnimationCurve();
			move._toFrameVsDistanceToTarget = new AnimationCurve();
			move._zOffsetCurve = new AnimationCurve();

			AnimationClip moveAnim = move._animationClip;
			//Animator animator = go.GetComponent<Animator>();
			//AnimatorStateInfo state_info = animator.GetCurrentAnimatorStateInfo(0);
			float totalFrames = moveAnim.length * moveAnim.frameRate; 
			//float framesPerSecond = 1.0f / moveAnim.frameRate;

			move._animationClip.SampleAnimation(go, 0);
			float originalZ = move._follow.position.z;

			move._animationClip.SampleAnimation(go, startRunFrame / totalFrames * moveAnim.length);
			float fromStartZ = move._follow.position.z;

			int frames = (int)System.Math.Floor (move._animationClip.length * move._animationClip.frameRate);
			Debug.Log("Frame Count " + frames);
			move.rot = new Quaternion[frames];
			move.pos = new Vector3[frames];
			for (int i = 0; i < frames; i++) {
				move._animationClip.SampleAnimation(go, i / totalFrames * moveAnim.length);

				move._zOffsetCurve.AddKey(i, move._follow.position.z - originalZ);

				Debug.Log("Add Frame " + (i-startRunFrame));
				if(i-startRunFrame > frames || i-startRunFrame < 0)
				{
					continue;
				}
				move.rot[i-startRunFrame] = move._follow.rotation;
				move.pos[i-startRunFrame] = move._follow.position;
			}

			move._animationClip.SampleAnimation(go, endRunFrame / totalFrames * moveAnim.length);
			float toDestZ = move._follow.position.z;

			for(int i = startRunFrame; i <= endRunFrame; i++)
			{
				move._animationClip.SampleAnimation(go, i / totalFrames * moveAnim.length);
				
				float toDestDist = toDestZ - move._follow.position.z;
				float fromDestDist = move._follow.position.z - fromStartZ;

				move._toAttack.AddKey(toDestDist, i);
				move._toAttackDist.AddKey(i, fromDestDist);
				move._toFrameVsDistanceToTarget.AddKey(i, toDestDist);
			}
			
			move._animationClip.SampleAnimation(go, startReturnFrame / totalFrames * moveAnim.length);
			float origZ = move._follow.position.z;
			int jj = endReturnFrame;
			move._animationClip.SampleAnimation(go, 0.99f * moveAnim.length);
			float endPositionZ = move._follow.position.z;
			for(int i = startReturnFrame; i <= endReturnFrame; i++, --jj)
			{
				move._animationClip.SampleAnimation(go, i / totalFrames * moveAnim.length);
				
				float dist = System.Math.Abs(move._follow.position.z - origZ);
				float toEndDist = System.Math.Abs(move._follow.position.z - endPositionZ);
				move._toReturn.AddKey(dist, i);
				move._toReturnDist.AddKey(jj, dist);
				move._toEndDist.AddKey(i, toEndDist);
			}
		}


		public static AnimationClip[] GetAnimationsWithoutMoves()
		{
			//CompileMoveDatabase();
			//return GetAllUsedAnimationClips().Where(clip => !_dbClipsToMoves.ContainsKey(clip.name)).ToArray();

			return null;
		}

		public static Move[] GetUnusedMoves()
		{
			CompileMoveDatabase();
			AnimationClip[] usedClips = GetAllUsedAnimationClips();

			return GetAllMoves().Where(move => move._animationClip == null || !usedClips.Contains(move._animationClip)).ToArray();
		}

		public static void CompileAllFxLibs()
		{
			if (EditorUtility.DisplayDialog("Compile All FX Libs", "This may take a while. Are you sure you want to build ALL FX Libs?", "Yes", "No"))
			{
				string[] characterNames = GetAllCharacterNames();
				FXLib standardFx = Resources.Load<FXLib>("Prefabs/StandardGameplayFX");
				try
				{
					for (int i = 0; i < characterNames.Length; i++)
					{
						EditorUtility.DisplayProgressBar("Compiling FX Libs", string.Format("{0} ({1}/{2})", characterNames[i], (i + 1), characterNames.Length), (float)i/characterNames.Length);
						CompileFxLibForCharacter(characterNames[i], false, standardFx);
					}
				}
				catch
				{
					EditorUtility.ClearProgressBar();
				}
									  
				EditorUtility.ClearProgressBar();
				AssetDatabase.SaveAssets();
			}
		}
		
		public static bool IsClipShared(Move move)
		{
			if (move != null)
			{
				if (move._animationClip != null)
				{
					Move[] moves = GetAllMoves();
					for (int i = 0; i < moves.Length; i++)
					{
						if (moves[i] != move)
						{
							if (moves[i]._animationClip == move._animationClip)
							{
								Debug.LogWarning(string.Format("WARNING: Animation Clip '{0}' is shared by multiple moves: {1}, {2}", move._animationClip.name, move.name, moves[i].name));
								return true;
							}
						}
					}
				}
			}
			
			return false;
		}

		public static int GetFrameCount(AnimationClip clip)
		{
			return Mathf.FloorToInt(clip.length * clip.frameRate);
		}
		
		public static int GetFrameFromTimeToInt(float time, float numFrames, float length)
		{
			return Mathf.FloorToInt(GetFrameFromTime(time, numFrames, length));
		}
		
		public static float GetFrameFromTime(float time, float numFrames, float length)
		{
			return time * numFrames / length;
		}

		// TJ: this will get the time for a given frame of an animation without factoring in any playback speed modification
		public static float GetTimeFromFrame(float frame, float numFrames, float length)
		{
			return 	GetRealTimeFromFrame(frame, numFrames, length, 1.0f);
		}

		// TJ: this will get the actual time for a given frame of an animation, factoring in the specified playback speed of the animation
		public static float GetRealTimeFromFrame(float frame, float numFrames, float length, float speed)
		{
			return ((frame / numFrames) * length) / speed;
		}
		
		public static string GetMoveFilePath(string name)
		{
			return string.Format("{0}/Moves/{1}.prefab", Application.dataPath, name);
		}
		
		public static string GetMovePrefabPath(string name)
		{
			return string.Format("Assets/Moves/{0}.prefab", name);
		}

        public static string GetCommonMovePrefabPath(string name)
        {
            return string.Format("Assets/Moves/Common/{0}.prefab", name);
        }

        public static string GetMovesDirectoryPath()
		{
			return string.Format("{0}/Moves/", Application.dataPath);
		}

		public static string GetCharacterDirectoryPath()
		{
			//return string.Format("{0}/Art/CharacterPrefabs/", Application.dataPath);
			return string.Format("{0}/Bundles/Player/", Application.dataPath);
		}

		public static string GetMergedCharacterDirectoryPath()
		{
			return string.Format("{0}/Bundles/Characters/Merged", Application.dataPath);
		}
		
		public static string GetCameraShakePresetPrefabPath(string name)
		{
			return string.Format("Assets/MoveAssets/CameraShakePresets/{0}.prefab", name);
		}
		
		public static string GetCameraShakePresetsDirectoryPath()
		{
			return string.Format("{0}/MoveAssets/CameraShakePresets/", Application.dataPath);
		}

		public static Move[] GetAllMoves()
		{
			List<Move> moves = new List<Move>();
			string[] files = System.IO.Directory.GetFiles(GetMovesDirectoryPath(), "*.prefab", System.IO.SearchOption.AllDirectories);
			for (int i = 0; i < files.Length; i++)
			{
				string file = files[i].Substring(files[i].IndexOf("Assets"));
				Move move = (Move)AssetDatabase.LoadAssetAtPath(file, typeof(Move));
				if (move != null)
					moves.Add(move);
			}
			
			return moves.ToArray();
		}

		public static AnimationClip[] GetAllUsedAnimationClips()
		{
			HashSet<AnimationClip> clips = new HashSet<AnimationClip>();
			string[] files = System.IO.Directory.GetFiles(Application.dataPath + "/Art/Characters/Controllers/", "*.overrideController");
			for (int i = 0; i < files.Length; i++)
			{
				string assetPath = files[i].Substring(files[i].IndexOf("Assets"));
				UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(AnimatorOverrideController));
				if (asset != null)
				{
					AnimatorOverrideController controller = (AnimatorOverrideController)asset;
					for (int j = 0; j < controller.clips.Length; j++)
					{
						AnimationClipPair pair = controller.clips[j];
						AnimationClip clip = pair.overrideClip != null ? pair.overrideClip : pair.originalClip;
						clips.Add(clip);
					}
				}
			}
			
			return clips.ToArray();
		}

		public static string[] GetAllMoveNames()
		{
			string[] files = System.IO.Directory.GetFiles(GetMovesDirectoryPath(), "*.prefab", System.IO.SearchOption.AllDirectories);
			for (int i = 0; i < files.Length; i++)
			{
				int startIndex = GetMovesDirectoryPath().Length;
				int length = files[i].IndexOf(".prefab") - startIndex;
				files[i] = files[i].Substring(startIndex, length);
				// convert windows path
				files[i] = files[i].Replace("\\", "/");
			}

			return files;
		}

		public static GameObject[] GetAllCharacters()
		{
			List<GameObject> prefabs = new List<GameObject>();
			string[] files = System.IO.Directory.GetFiles(GetCharacterDirectoryPath(), "*.prefab");

			for (int i = 0; i < files.Length; i++)
			{
				string file = files[i].Substring(files[i].IndexOf("Assets"));
				GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(file, typeof(GameObject));
				if (prefab != null)
				{
					prefabs.Add(prefab);
				}
			}

			return prefabs.ToArray();
		}

		public static GameObject[] GetAllMergedCharacters()
		{
			List<GameObject> prefabs = new List<GameObject>();
			string[] files = System.IO.Directory.GetFiles(GetMergedCharacterDirectoryPath(), "*.prefab");
			
			for (int i = 0; i < files.Length; i++)
			{
				string file = files[i].Substring(files[i].IndexOf("Assets"));
				GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(file, typeof(GameObject));
				if (prefab != null)
				{
					prefabs.Add(prefab);
				}
			}
			
			return prefabs.ToArray();
		}

		public static string[] GetAllCharacterNames()
		{
			List<string> names = new List<string>();
			string[] files = System.IO.Directory.GetFiles(GetCharacterDirectoryPath(), "*.prefab");
			for (int i = 0; i < files.Length; i++)
			{
				int startIndex = GetCharacterDirectoryPath().Length;
				int length = files[i].IndexOf(".prefab") - startIndex;
				files[i] = files[i].Substring(startIndex, length);
				names.Add(files[i]);
			}

			return names.ToArray();
		}

		public static string[] GetAllVariantNames(string characterName)
		{
			string baseFolder = "Assets/Bundles/Player/Variants";

			List<string> names = new List<string> ();

			GameObject[] variantsObjs = DebugHandler.GetAtPath<GameObject>(baseFolder);
			for (int i = 0; i < variantsObjs.Length; i++) 
			{
				CharacterVariant variant_component = variantsObjs[i].GetComponent<CharacterVariant>();
				if (variant_component != null && variant_component.MoveSetPrefab != null && variant_component.MoveSetPrefab.name == characterName)
				{
					names.Add(variantsObjs[i].name);
				}
			}

			return names.ToArray();
		}

		public static GameObject[] GetAllVariants()
		{
			string baseFolder = "Assets/Bundles/Player/Variants";

			return DebugHandler.GetAtPath<GameObject>(baseFolder);
		}

		//public static string GetDatabasePath()
		//{
		//    return Application.dataPath + "/Moves/movedb.txt";
		//}

		//public static string GetCharacterMoveDatabasePath()
		//{
		//    return Application.dataPath + "/Moves/charmovedb.txt";
		//}
		
		public static string GetMoveFromClip(string clipName)
		{
			//if (_dbClipsToMoves.Count == 0)
			//    CompileMoveDatabase();

			//if (_dbClipsToMoves.ContainsKey(clipName))
			//    return _dbClipsToMoves[clipName];
			
			return string.Empty;
		}
		
		public static string GetClipFromMove(string moveName)
		{
			//if (_dbMovesToClips.Count == 0)
			//    CompileMoveDatabase();

			//if (_dbMovesToClips.ContainsKey(moveName))
			//    return _dbMovesToClips[moveName];
			
			return string.Empty;
		}

		public static string[] GetMoveListFromCharacter(string characterName)
		{
			//if (_dbCharacterToClips.Count == 0)
			//    CompileMoveDatabase();

			//if (_dbCharacterToClips.ContainsKey(characterName))
			//{
			//    List<string> moves = new List<string>();
			//    for (int i = 0; i < _dbCharacterToClips[characterName].Count; i++)
			//    {
			//        string move = GetMoveFromClip(_dbCharacterToClips[characterName][i]);
			//        if (!string.IsNullOrEmpty(move))
			//            moves.Add(move);
			//    }

			//    return moves.ToArray();
			//}

			return null;
		}

		public static string[] GetCharacterListFromMove(string moveName)
		{
			//if (_dbClipToCharacters.Count == 0)
			//    CompileMoveDatabase();

			//string clipName = GetClipFromMove(moveName);
			//if (!string.IsNullOrEmpty(clipName))
			//{
			//    if (_dbClipToCharacters.ContainsKey(clipName))
			//        return _dbClipToCharacters[clipName].ToArray();
			//}

			return null;
		}
		
		public static void WriteDatabase()
		{
			//Move[] moves = GetAllMoves();

			//UnityEditor.VersionControl.Task task = UnityEditor.VersionControl.Provider.Checkout(GetDatabasePath(), UnityEditor.VersionControl.CheckoutMode.Both);
			//task.Wait();

			//if (task.success)
			//{
			//    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(GetDatabasePath()))
			//    {
			//        string line = "";
			//        for (int i = 0; i < moves.Length; i++)
			//        {
			//            if (moves[i]._animationClip != null)
			//            {
			//                line = string.Format("{0},{1}", moves[i].name, moves[i]._animationClip.name);
			//                writer.WriteLine(line);
			//            }
			//        }
			//    }
			//}
			//else
			//{
			//	EB.Debug.LogWarning("WARNING: CHECK OUT MOVE DB FILE FAILED");
			//}

			//ReadDatabase();
		}

		public static void ReadDatabase()
		{
			//_dbMovesToClips = new Dictionary<string, string> ();
			//_dbClipsToMoves = new Dictionary<string, string> ();

			//using (System.IO.StreamReader reader = new System.IO.StreamReader(GetDatabasePath()))
			//{
			//    string line = "";
			//    while ((line = reader.ReadLine()) != null)
			//    {
			//        string[] values = line.Split(',');
			//        if (!_dbMovesToClips.ContainsKey(values[0]))
			//            _dbMovesToClips.Add(values[0], values[1]);

			//        if (!_dbClipsToMoves.ContainsKey(values[1]))
			//            _dbClipsToMoves.Add(values[1], values[0]);
			//    }
			//}
		}

		public static void WriteCharacterMoveDB()
		{
			//GameObject[] characters = GetAllCharacters();

			//UnityEditor.VersionControl.Task task = UnityEditor.VersionControl.Provider.Checkout(GetCharacterMoveDatabasePath(), UnityEditor.VersionControl.CheckoutMode.Both);
			//task.Wait();

			//if (task.success)
			//{
			//    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(GetCharacterMoveDatabasePath()))
			//    {
			//        System.Text.StringBuilder sb = new System.Text.StringBuilder();
			//        UltimatePlayerController controller = null;
			//        for (int i = 0; i < characters.Length; i++)
			//        {
			//            controller = characters[i].GetComponent<UltimatePlayerController>();

			//            if (controller != null)
			//            {
			//                sb = new System.Text.StringBuilder();
			//                sb.Append(characters[i].name);
			//                sb.Append(",");

			//                if (controller._overrideController != null)
			//                {
			//                    AnimationClipPair[] clips = controller._overrideController.clips;

			//                    for (int j = 0; j < clips.Length; j++)
			//                    {
			//                        string name = clips[j].overrideClip != null ? clips[j].overrideClip.name : clips[j].originalClip.name;

			//                        sb.Append(name);
									
			//                        if (j < clips.Length - 1)
			//                            sb.Append(",");
			//                    }
			//                }

			//                writer.WriteLine(sb.ToString());
			//            }
			//        }
			//    }
			//}
			//else
			//{
			//    EB.Debug.LogWarning("WARNING: CHECKING OUT CHARACTER MOVE DB FILE FAILED");
			//}

			//ReadCharacterMoveDB();
		}

		public static void ReadCharacterMoveDB()
		{
			//_dbCharacterToClips = new Dictionary<string, List<string>>();
			//_dbClipToCharacters = new Dictionary<string, List<string>>();

			//using (System.IO.StreamReader reader = new System.IO.StreamReader(GetCharacterMoveDatabasePath()))
			//{
			//    string line = "";
			//    while ((line = reader.ReadLine()) != null)
			//    {
			//        string[] values = line.Split(',');

			//        if (!_dbCharacterToClips.ContainsKey(values[0]))
			//        {
			//            _dbCharacterToClips.Add(values[0], new List<string>());

			//            for (int i = 1; i < values.Length; i++)
			//            {
			//                _dbCharacterToClips[values[0]].Add(values[i]);

			//                if (!_dbClipToCharacters.ContainsKey(values[i]))
			//                    _dbClipToCharacters.Add(values[i], new List<string>());

			//                _dbClipToCharacters[values[i]].Add(values[0]);
			//            }
			//        }
			//    }
			//}
		}
		public static void CompileFxLibForCharacter(string name) { CompileFxLibForCharacter(name, false, null); }
		public static void CompileFxLibForCharacter(string name, bool saveOnComplete, FXLib standardFx)
		{
			GameObject character = GetCharacter(name);
			if (character != null)
			{
				if (standardFx == null)
				{
					standardFx = Resources.Load<FXLib>("Prefabs/StandardGameplayFX");
				}

				string[] moveList = GetMoveListFromCharacter(name);
				
				List<GameObject> particles 		= new List<GameObject>();
				List<GameObject> trails			= new List<GameObject>();
				List<GameObject> lights			= new List<GameObject>();
				List<GameObject> projectiles	= new List<GameObject>();
				
				for (int i = 0; i < moveList.Length; i++)
				{
					Move move = GetMove(moveList[i]);
					
					for (int j = 0; j < move._hitEvents.Count; j++)
					{
						ParticleSystem ps = move._hitEvents[j]._particleProperties._particleReference.Value;
						ParticleSystem fps = move._hitEvents[j]._particleProperties._flippedParticleReference.Value;
						
						if (ps != null && !standardFx._fxParticleList.Contains(ps.gameObject) && !particles.Contains(ps.gameObject))
						{
							particles.Add(ps.gameObject);
						}
						
						if (fps != null && !standardFx._fxParticleList.Contains(fps.gameObject)	&& !particles.Contains(fps.gameObject))
						{
							particles.Add(fps.gameObject);
						}
					}
					
					for (int j = 0; j < move._particleEvents.Count; j++)
					{
						ParticleSystem ps = move._particleEvents[j]._particleProperties._particleReference.Value;
						ParticleSystem fps = move._particleEvents[j]._particleProperties._flippedParticleReference.Value;
						
						if (ps != null && !standardFx._fxParticleList.Contains(ps.gameObject) && !particles.Contains(ps.gameObject))
						{
							particles.Add(ps.gameObject);
						}
						
						if (fps != null && !standardFx._fxParticleList.Contains(fps.gameObject)	&& !particles.Contains(fps.gameObject))
						{
							particles.Add(fps.gameObject);
						}
					}
					
					for (int j = 0; j < move._trailRendererEvents.Count; j++)
					{
						GameObject trail = move._trailRendererEvents[j]._trailRendererProperties._trailRenderer;
						
						if (trail != null && !standardFx._fxTrailList.Contains(trail) && !trails.Contains(trail))
						{
							trails.Add(trail);
						}
					}
					
					for (int j = 0; j < move._dynamicLightEvents.Count; j++)
					{
						GameObject light = move._dynamicLightEvents[j]._dynamicLightProperties._dynamicLight;
						
						if (light != null && !standardFx._fxLightList.Contains(light) && !lights.Contains(light))
						{
							lights.Add(light);
						}
					}
					
					for (int j = 0; j < move._projectileEvents.Count; j++)
					{
						GameObject projectile = move._projectileEvents[j]._projectileProperties._prefab;
						
						if (projectile != null && !projectiles.Contains(projectile))
						{
							projectiles.Add(projectile);
						}
					}
				}

				//UnityEditor.VersionControl.Task task = UnityEditor.VersionControl.Provider.Checkout(character, UnityEditor.VersionControl.CheckoutMode.Both);
				//task.Wait();

				//if (task.success)
				{
					FXLib lib = character.GetComponent<FXLib>();
					if (lib == null)
					{
						lib = character.AddComponent<FXLib>();
					}
					
					lib._fxParticleList			= particles;
					lib._fxTrailList			= trails.ToArray();
					lib._fxLightList			= lights.ToArray();
					lib._projectilePrefabList	= projectiles.ToArray();

					EditorUtility.SetDirty(character);

					GameObject mergedCharacter = GetMergedCharacter(name);
					if (mergedCharacter != null)
					{
						//task = UnityEditor.VersionControl.Provider.Checkout(mergedCharacter, UnityEditor.VersionControl.CheckoutMode.Both);
						//task.Wait();

						//if (task.success)
						{
							lib = mergedCharacter.GetComponent<FXLib>();
							if (lib == null)
							{
								lib = mergedCharacter.AddComponent<FXLib>();
							}

							lib._fxParticleList			= particles;
							lib._fxTrailList			= trails.ToArray();
							lib._fxLightList			= lights.ToArray();
							lib._projectilePrefabList	= projectiles.ToArray();
							
							EditorUtility.SetDirty(mergedCharacter);
						}
						//else
						//{
						//	EB.Debug.LogWarning("WARNING! Could not check out MERGED character: " + name);
						//}
					}

					if (saveOnComplete)
					{
						AssetDatabase.SaveAssets();
					}
				}
				//else
				//{
				//	EB.Debug.LogWarning("WARNING! Could not check out character: " + name);
				//}
			}

		}

		public static Move GetMove(string name)
		{
			return System.Array.Find<Move>(GetAllMoves(), delegate(Move move) { return move.name == name; });
		}

		public static GameObject GetCharacter(string name)
		{
			return System.Array.Find<GameObject>(GetAllCharacters(), delegate(GameObject go) { return go.name.Equals(name); });
		}

		public static GameObject GetMergedCharacter(string name)
		{
			return System.Array.Find<GameObject>(GetAllMergedCharacters(), delegate(GameObject go) { return go.name.Equals(name); });
		}
		
		// TJ: this is how unity exposes the means of writing animation events to clips via script; HOWEVER, this does not change the clip's metadata
		private static void SetAllAnimationEvents(Move move)
		{
			if (move == null)
				return;
			
			if (move._animationClip == null)
				return;
			
			if (IsClipShared(move))
				return;
			
			AnimationUtility.SetAnimationEvents(move._animationClip, move.CompileEvents());
		}
		
		// TJ: This is a HACK; I overwrite the metadata file for the fbx with the events I want to appear in the clip
		private static void WriteAllAnimationEvents(Move move)
		{
			if (move == null)
				return;
			
			if (move._animationClip == null)
				return;
			
			if (IsClipShared(move))
				return;
			
			AnimationEvent[] events = move.CompileEvents();

			string metaDataPath = AssetDatabase.GetTextMetaFilePathFromAssetPath(AssetDatabase.GetAssetPath(move._animationClip));
			string temp = System.IO.Path.GetTempFileName();

			System.IO.StreamReader reader = new System.IO.StreamReader(metaDataPath);
			System.IO.StreamWriter writer = new System.IO.StreamWriter(temp);

			string line = string.Empty;

			// 1) Read down to our animation clip; write every line up to and including that
			while ((line = reader.ReadLine()) != null)
			{
				writer.WriteLine(line);
				if (line.Contains("name: " + move._animationClip.name))
					break;
			}

			// 2) Read down to the events subsection of our animation clip; write every line up to and including that
			while ((line = reader.ReadLine()) != null)
			{
				if (line.Contains("events:"))
					break;
				writer.WriteLine(line);
			}

			// 3) Write our new events (if we have any to write)
			if (events == null || events.Length == 0)
			{
				writer.WriteLine("      events: []");
			}
			else
			{
				writer.WriteLine("      events:");
				for (int i = 0; i < events.Length; i++)
					WriteEventMetadataString(events[i], ref writer);
			}

			// 4) Continue reading until we hit the next subsection ('transformMask')
			while ((line = reader.ReadLine()) != null)
			{
				if (line.Contains("transformMask:"))
				{
					writer.WriteLine(line);
					break;
				}
			}

			// 5) Finish reading the file, and write all remaining lines
			while ((line = reader.ReadLine()) != null)
			{
				writer.WriteLine(line);
			}

			reader.Close();
			writer.Close();

			// 6) Overwrite the existing metadata file
			//System.IO.File.Delete(metaDataPath);
			//System.IO.File.Move(temp, metaDataPath);

			UnityEditor.VersionControl.Task task = UnityEditor.VersionControl.Provider.Checkout(AssetDatabase.GetAssetPath(move._animationClip), UnityEditor.VersionControl.CheckoutMode.Both);
			task.Wait();

			try
			{
				System.IO.File.Copy(temp, metaDataPath, true);
			}
			catch(System.UnauthorizedAccessException e)
			{
				Debug.LogException(e);
				Debug.LogWarningFormat("Could not build move '{0}' because the associated metadata file is not checked out", move.gameObject.name);
			}

			// 7) Reimport the fbx file
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(move._animationClip), ImportAssetOptions.ForceUpdate);
		}
		
		private static void RemoveAllAnimationEvents(Move move)
		{
			if (move == null)
				return;
			
			if (move._animationClip == null)
				return;
			
			AnimationEvent[] events = new AnimationEvent[0];
			
			string metaDataPath = AssetDatabase.GetTextMetaFilePathFromAssetPath(AssetDatabase.GetAssetPath(move._animationClip));
			string temp = System.IO.Path.GetTempFileName();
			
			System.IO.StreamReader reader = new System.IO.StreamReader(metaDataPath);
			System.IO.StreamWriter writer = new System.IO.StreamWriter(temp);
			
			string line = string.Empty;
			
			// 1) Read down to our animation clip; write every line up to and including that
			while ((line = reader.ReadLine()) != null)
			{
				writer.WriteLine(line);
				if (line.Contains("name: " + move._animationClip.name))
					break;
			}
			
			// 2) Read down to the events subsection of our animation clip; write every line up to and including that
			while ((line = reader.ReadLine()) != null)
			{
				if (line.Contains("events:"))
					break;
				writer.WriteLine(line);
			}
			
			// 3) Write our new events (if we have any to write)
			if (events == null || events.Length == 0)
			{
				writer.WriteLine("      events: []");
			}
			else
			{
				writer.WriteLine("      events:");
				for (int i = 0; i < events.Length; i++)
					WriteEventMetadataString(events[i], ref writer);
			}
			
			// 4) Continue reading until we hit the next subsection ('transformMask')
			while ((line = reader.ReadLine()) != null)
			{
				if (line.Contains("transformMask:"))
				{
					writer.WriteLine(line);
					break;
				}
			}
			
			// 5) Finish reading the file, and write all remaining lines
			while ((line = reader.ReadLine()) != null)
			{
				writer.WriteLine(line);
			}
			
			reader.Close();
			writer.Close();
			
			// 6) Overwrite the existing metadata file
			//System.IO.File.Delete(metaDataPath);
			//System.IO.File.Move(temp, metaDataPath);
			
			//UnityEditor.VersionControl.Task task = UnityEditor.VersionControl.Provider.Checkout(AssetDatabase.GetAssetPath(move._animationClip), UnityEditor.VersionControl.CheckoutMode.Both);
			//task.Wait();
			
			try
			{
				System.IO.File.Copy(temp, metaDataPath, true);
			}
			catch(System.UnauthorizedAccessException e)
			{
				Debug.LogException(e);
				Debug.LogWarningFormat("Could not build move '{0}' because the associated metadata file is not checked out", move.gameObject.name);
			}
			
			// 7) Reimport the fbx file
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(move._animationClip), ImportAssetOptions.ForceUpdate);
		}

		private static void WriteAnimationEventsToMove(Move move)
		{
			if (move == null)
				return;
			
			if (move._animationClip == null)
				return;
			
			move._animationEvents = move.CompileEvents();
		}
		
		private static void WriteEventMetadataString(AnimationEvent e, ref System.IO.StreamWriter writer)
		{
			writer.WriteLine(string.Format("        - time: {0}", e.time.ToString()));
			writer.WriteLine(string.Format("          functionName: {0}", e.functionName));
			writer.WriteLine(string.Format("          data: {0}", e.stringParameter));
			writer.WriteLine(string.Format("          objectReferenceParameter: {{instanceID: 0}}"));
			writer.WriteLine(string.Format("          floatParameter: {0}", e.floatParameter.ToString()));
			writer.WriteLine(string.Format("          intParameter: {0}", e.intParameter.ToString()));
			writer.WriteLine(string.Format("          messageOptions: {0}", ((int)(e.messageOptions)).ToString()));
		}

		public static string[] GetGenericEventPresetNames()
		{
			return _genericEventPresets.Select(preset => preset._eventName).ToArray();
		}

		public static GenericEventPreset[] GenericEventPresets { get { return _genericEventPresets; } }
		public static string[] HitReactionStates { get { return _hitReactionStates; } }

		public class GenericEventPreset
		{
			public string 	_eventName			= "New Event";
			public bool 	_useFloatParameter 	= false;
			public bool 	_useIntParameter 	= false;
			public bool 	_useObjectParameter = false;
			public bool 	_useStringParameter = false;

			public GenericEventPreset(string eventName, bool useFloatParameter, bool useIntParameter, bool useObjectParameter, bool useStringParameter)
			{
				_eventName = eventName;
				_useFloatParameter = useFloatParameter;
				_useIntParameter = useIntParameter;
				_useObjectParameter = useObjectParameter;
				_useStringParameter = useStringParameter;
			}
		}

		// a list of common events
		private static GenericEventPreset[] _genericEventPresets = 
		{
			new GenericEventPreset("(New)", true, true, true, true),
			new GenericEventPreset("OnOpenAttackInputWindow", false, false, false, false),
			new GenericEventPreset("OnCloseAttackInputWindow", false, false, false, false),
			new GenericEventPreset("OnEnableInvulnerability", false, false, false, false),
			new GenericEventPreset("OnDisableInvulnerability", false, false, false, false),
			new GenericEventPreset("OnDisableRun", true, false, false, false),
			new GenericEventPreset("OnSetArmorLevel", false, true, false, false),
			new GenericEventPreset("OnStopParticle", false, false, false, true),
			new GenericEventPreset("OnClearParticle", false, false, false, true),
			new GenericEventPreset("OnStopTrailRenderer", false, false, false, true),
			new GenericEventPreset("OnStopDynamicLight", false, false, false, true),
			new GenericEventPreset("ReleaseWeaponToAnimation", false, false, false, true),
			new GenericEventPreset("ResetToIdle", false, false, false, false),
			new GenericEventPreset("BlockReset", false, false, false, false),
			new GenericEventPreset("EnableProp", false, false, false, true),
			new GenericEventPreset("DisableProp", false, false, false, true),
			new GenericEventPreset("SlideToOpponent", true, false, false, false),
		};

		// a list of hit reaction states
		private static string[] _hitReactionStates = { "(Other)", "recoil_light_left", "recoil_light_right", "recoil_medium_left", "recoil_medium_right", "recoil_medium_back", "recoil_heavy" };
		 
		//private static Dictionary<string, string> 			_dbMovesToClips 	= new Dictionary<string, string>();
		//private static Dictionary<string, string> 			_dbClipsToMoves 	= new Dictionary<string, string>();
		//private static Dictionary<string, List<string>> 	_dbCharacterToClips = new Dictionary<string, List<string>>();
		//private static Dictionary<string, List<string>> 	_dbClipToCharacters	= new Dictionary<string, List<string>>();
#endif
	}
}
