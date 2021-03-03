using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MoveEditor
{
	public enum AttackLevel
	{
		kAttackLight,
		kAttackMedium,
		kAttackHeavy,
		kAttackSpecial0,
		kAttackSpecial1,
		kAttackSpecial2,
		kAttackInvalid
	}

	public enum AttackResult
	{
		None,
		Hit,
		Blocked,
		Missed,
	}

	public enum BodyPart
	{
		Hips,
		LeftUpperLeg,
		RightUpperLeg,
		LeftLowerLeg,
		RightLowerLeg,
		LeftFoot,
		RightFoot,
		Spine,
		Chest,
		Neck,
		Head,
		LeftShoulder,
		RightShoulder,
		LeftUpperArm,
		RightUpperArm,
		LeftLowerArm,
		RightLowerArm,
		LeftHand,
		RightHand,
		LeftToes,
		RightToes,
		LeftEye,
		RightEye,
		Jaw,
		LastBone,
		Root,
		LeftProp,
		RightProp,
		Custom,
		FXRoot,
		HeadNub,
		ChestNub,
		FootNub,
		HealthBar,
		HeadNubNotRot,
		EnemyCenter,
		FriendCenter,
		SceneRoot
	}
	
	public class CameraMotionOption
	{
		public string _motionName;
		public float _probability;
		public bool _isCritical;
	}

	public enum CameraMotionTarget
	{
		Attacker,
		Defenders,
		All,
		//OriginView
		DefendersCameraAnchor,
	}

	//Camera Motion Event执行条件，有Local Play Only(仅仅对玩家自身执行该事件)，Enemy Only(仅仅对敌人执行该事件)，All三种(对战场中所有人都执行)
	public enum CameraMotionTrigger
	{
		LocalPlayerOnly,
		EnemyOnly,
		All,
	}

	public static class MoveUtils 
	{
		public static Transform GetBodyPartTransform(Animator animator, BodyPart bodyPart, string customPath = "")
		{
			switch(bodyPart)
			{
				case BodyPart.Root:
					return animator.transform;
				case BodyPart.LeftProp:
					return animator.GetBoneTransform(HumanBodyBones.LeftHand).Find("LeftProp");
				case BodyPart.RightProp:
					return animator.GetBoneTransform(HumanBodyBones.RightHand).Find("RightProp");
				case BodyPart.Custom:
				{
					if (customPath.Length > 0)
					{
						Transform transform = animator.transform.Find(customPath);
						if( transform == null )
						{
							int index = customPath.LastIndexOf('/');
							index = System.Math.Min( customPath.Length - 1, System.Math.Max(0, index+1) );
							string path = customPath.Substring(index);

							GameObject tempgo=	EB.Util.GetObject(animator.gameObject, path);
							if (tempgo != null)
							{
								transform = tempgo.transform;
							}
							else
							{
								EB.Debug.LogError("Not GetBodyPartTransform For:customPath={0}", customPath);
								transform = animator.transform;
							}
						}

						return transform;
					}
					else
					{
						return animator.transform;
					}
				}
				case BodyPart.FXRoot:
					return animator.GetComponent<FXHelper>().FXRootTransform;
				case BodyPart.HeadNub:
					return animator.GetComponent<FXHelper>().HeadNubTransform;
				case BodyPart.ChestNub:
					return animator.GetComponent<FXHelper>().ChestNubTransform;
				case BodyPart.FootNub:
					return animator.GetComponent<FXHelper>().FootNubTransform;
				case BodyPart.HealthBar:
					return animator.GetComponent<FXHelper>().HealthBarTransform;
				case BodyPart.HeadNubNotRot:
					return animator.transform;
				case BodyPart.EnemyCenter:
					if (GameEngine.Instance!=null)
					{
						//if(animator.GetComponent<Combatant>().Data.TeamId== CombatLogic.Instance.LocalPlayerTeamIndex)
						//{
							CombatLogic.FormationSide side = CombatLogic.Instance.GetSide(1 - animator.GetComponent<Hotfix_LT.Combat.Combatant>().Data.TeamId);
							return Formations.Instance.GetPositionTransform(side, "1", "1");
						//}
						//else
						//{
						//	CombatLogic.FormationSide side = CombatLogic.Instance.GetSide(CombatLogic.Instance.LocalPlayerTeamIndex);
						//  return Formations.Instance.GetPositionTransform(side, "1", "1");
						//}
					}
					else
					{
						return animator.transform;
					}
				case BodyPart.FriendCenter:
					if (GameEngine.Instance != null)
					{
						//if (animator.GetComponent<Combatant>().Data.TeamId == CombatLogic.Instance.LocalPlayerTeamIndex)
						//{
							CombatLogic.FormationSide side = CombatLogic.Instance.GetSide(animator.GetComponent<Hotfix_LT.Combat.Combatant>().Data.TeamId);
							return Formations.Instance.GetPositionTransform(side, "1", "1");
						//}
						//else
						//{
						//	CombatLogic.FormationSide side = CombatLogic.Instance.GetSide(1-CombatLogic.Instance.LocalPlayerTeamIndex);
						//	return Formations.Instance.GetPositionTransform(side, "1", "1");
						//}
					}
					else
					{
						return animator.transform;
					}
				case BodyPart.SceneRoot:
					return null;
				default:
					HumanBodyBones bone = (HumanBodyBones)bodyPart;
					return animator.GetBoneTransform(bone);
			}
		}

        /// <summary>
        /// 获取特效实体
        /// </summary>
        /// <param name="properties">特效信息</param>
        /// <param name="flipped">是否翻转粒子系统</param>
        /// <param name="bypassPools">是否直接克隆特效</param>
        /// <returns></returns>
		private static ParticleSystem GetParticleInstance(Object obj, ParticleEventProperties properties, bool flipped = false, bool bypassPools = false )
		{
            //
            //EB.Debug.LogPSPoolAsset("<color=#00ff00>获取特效实体:" + properties + "</color>,是否翻转:" + flipped + ",是否直接克隆特效:" + bypassPools+",(false:从特效缓存池拿里)");
			ParticleSystem ps = null;
			
			if (Application.isPlaying)
			{
				if (!bypassPools)
				{
					ps = LoadParticleWithPool(obj,properties, flipped);
				}
				else
				{
					ps = LoadParticleWithoutPool(properties, flipped);
				}
			}
			else
			{
				ps = LoadParticleWithoutPool(properties, flipped);
			}

			return ps;
		}
        
        public static ParticleSystem InstantiateParticle(Object obj, ParticleEventProperties properties, Vector3 worldPosition, bool flipped = false, bool bypassPools = false )
		{
            ParticleSystem ps = GetParticleInstance(obj, properties, flipped, bypassPools);

			if( ps != null )
			{
				ps.transform.position = worldPosition;

				ps.transform.eulerAngles = properties._angles;
				Vector3 offset = properties._offset;

				if (flipped)
				{
					offset.x = -offset.x; // flip the x
					FlipWorldRotationByXYPlane(ps.transform);
				}

				ps.transform.position += offset;
			}

			return ps;
		}

		public static ParticleSystem InstantiateParticle(Object obj, ParticleEventProperties properties, Animator animator, bool flipped = false, bool bypassPools = false)
		{
			ParticleSystem ps = GetParticleInstance(obj, properties, flipped, bypassPools);
				
			if (ps != null)
			{
				Transform parent = GetBodyPartTransform(animator, properties._bodyPart, properties._attachmentPath);

                if (properties._parent)
                {
                    ps.transform.SetParent(parent);
					if (properties._bodyPart == BodyPart.HeadNubNotRot)
					{
						ps.transform.position = animator.GetComponent<FXHelper>().HeadNubTransform.position;
						ps.transform.localEulerAngles = Vector3.zero;
					}
					else
					{
						ps.transform.localPosition = properties._offset;
					}
                }
                else
                { 
					ps.transform.SetParent(null);
					ps.transform.position = parent.TransformPoint(properties._offset);
				}

				if (properties._lockXOffset || properties._lockYOffset != HeightLock.Unlocked || properties._lockZOffset)
				{
					AttachTransform.LockPosition(ps.gameObject, properties._lockXOffset, properties._lockYOffset, properties._lockZOffset);
				}

                if (properties._worldSpace)
                {
                    ps.transform.eulerAngles = properties._angles;

                    if (flipped)
                    {
                        // rotate this by 180
                        ps.transform.RotateAround(ps.transform.position, Vector3.up, 180.0f);
                    }
                }
                else
                {
                    if (properties._parent)
                    {
                        ps.transform.localEulerAngles = properties._angles; //感觉不好 没看明白 因为我们现在的粒子不挂角色身上
                    }
                    else
                    {
                        ps.transform.eulerAngles = parent.TransformDirection(properties._angles);//解决无父节点的旋转问题
                    }
                }

				// mirror the effect, for parented effects, this is done inside the attach transform
				if (flipped && !properties._parent)
				{
					FlipWorldRotationByXYPlane(ps.transform);
				}

				ps.transform.localScale = properties._scale;
			}
			
			return ps;
		}
        /// <summary>
        /// 从特效缓存池里获取特效粒子
        /// </summary>
        /// <param name="properties">特效信息</param>
        /// <param name="flipped">是否翻转</param>
        /// <returns></returns>
		private static ParticleSystem LoadParticleWithPool(Object obj, ParticleEventProperties properties, bool flipped)
		{
			if (PSPoolManager.Instance != null)
			{
				string name = string.Empty;
				
				if (flipped)
				{
					if (properties._flippedParticleReference.Valiad)
						name = properties._flippedParticleReference.Name;
					else
						name = properties.FlippedParticleName;
				}
				
				if (string.IsNullOrEmpty(name))
				{
					if (properties._particleReference.Valiad)
						name = properties._particleReference.Name;
					else
						name = properties.ParticleName;
				}
				
				return PSPoolManager.Instance.Use(obj,name);
			}

			return null;
		}

        public static string GetParticleName(ParticleEventProperties properties, bool flipped)
        {
            if (flipped)
            {
                if (properties._flippedParticleReference.Value != null)
                {
                    return properties._flippedParticleReference.Value.name;
                }
                else
                {
                    if (properties._particleReference.Value != null)
                    {
                        return properties._particleReference.Value.name;
                    }
                }
            }
            else
            {
                if (properties._particleReference.Value != null)
                {
                    return properties._particleReference.Value.name;
                }
            }
            return properties.ParticleName;
        }
        /// <summary>
        /// 直接克隆特效
        /// </summary>
        /// <param name="properties">特效信息</param>
        /// <param name="flipped">是否翻转粒子系统</param>
        /// <returns></returns>
		private static ParticleSystem LoadParticleWithoutPool(ParticleEventProperties properties, bool flipped)
		{
			ParticleSystem ps = null;

			if (flipped)
			{
				if (properties._flippedParticleReference.Value != null)
				{
					ps = (ParticleSystem)GameObject.Instantiate(properties._flippedParticleReference.Value);
				}
				else
				{
					if (properties._particleReference.Value != null)
					{
						ps = (ParticleSystem)GameObject.Instantiate(properties._particleReference.Value);
					}
				}
			}
			else
			{
				if (properties._particleReference.Value != null)
				{
					ps = (ParticleSystem)GameObject.Instantiate(properties._particleReference.Value);
				}
			}

            if (ps != null)
            {
                ps.name = GetParticleName(properties, flipped);
            }

			return ps;
		}

		private static GameObject LoadTrailWithPool(TrailRendererEventProperties properties, bool isCrit = false)
		{
			GameObject trail = null;
			if (GenericPoolSingleton.Instance.trailPool != null)
			{
				string name = string.Empty;
				if(isCrit)
				{
					if(properties._trailRendererCrit != null)
					{
						name = properties._trailRendererCrit.name;
					}
					else
					{
						name = properties.TrailRendererNameCrit;
					}
				}
				else
				{
					if (properties._trailRenderer != null)
					{
						name = properties._trailRenderer.name;
					}
					else
					{
						name = properties.TrailRendererName;
					}
				}
				trail = GenericPoolSingleton.Instance.trailPool.Use(name);
			}
			return trail;
		}

		private static GameObject LoadTrailWithoutPool(TrailRendererEventProperties properties, bool isCrit = false)
		{
			GameObject trail = null;
			if(isCrit)
			{
				trail =  (GameObject)GameObject.Instantiate(properties._trailRendererCrit);
			}
			else
			{
				trail = (GameObject)GameObject.Instantiate(properties._trailRenderer);
			}
			return trail;
		}

		private static GameObject LoadLightWithPool(DynamicLightEventProperties properties)
		{
			GameObject light = null;
			if (GenericPoolSingleton.Instance.lightPool != null)
			{
				string name = string.Empty;
				if (properties._dynamicLight != null)
				{
					name = properties._dynamicLight.name;
				}
				else
				{
					name = properties.DynamicLightName;
				}
				light = GenericPoolSingleton.Instance.lightPool.Use(name);
			}
			return light;
		}
		
		private static GameObject LoadLightWithoutPool(DynamicLightEventProperties properties)
		{
			GameObject light = (GameObject)GameObject.Instantiate(properties._dynamicLight); 
			return light;
		}


		public static GameObject InstantiateTrailInstance (TrailRendererEventProperties properties, Animator animator, bool flipped, bool isCrit, bool bypassPools = false)
		{
			GameObject trail = null;
			if (Application.isPlaying)
			{
				if (!bypassPools)
				{
					trail = LoadTrailWithPool(properties, isCrit);
				}
				else 
				{
					trail = LoadTrailWithoutPool(properties, isCrit);
				}
			}
			else
			{
				trail = LoadTrailWithoutPool(properties, isCrit);
			}

			if(trail != null)
			{
				TrailRendererInstance tri = trail.GetComponent<TrailRendererInstance>();
				Transform parent = null;

				if(tri != null)
				{
					Vector3 offset1 = properties._offset1;
					Vector3 offset2 = properties._offset2;

					tri.OffsetPoints(offset1, offset2, properties._useLocalOffsets);

					if (tri._TimeUnits == TrailRenderer.eTIME_UNITS.Frames)
					{
						tri.ConvertFramesToSeconds(properties._fps);
					}
				}

				if (animator != null)
				{
					parent = GetBodyPartTransform(animator, properties._attachment, properties._attachmentPath);
					trail.transform.parent = parent;

					trail.transform.localPosition = properties._rigOffset;

					if (properties._worldSpace)
					{
						trail.transform.eulerAngles = properties._angles;

						if (flipped)
						{
							// rotate this by 180
							trail.transform.RotateAround(trail.transform.position, Vector3.up, 180.0f);
						}
					}
					else
					{
						trail.transform.localEulerAngles = properties._angles;
					}

				}

				trail.transform.localScale = Vector3.one;

				if( !properties._parent )
				{
					trail.transform.parent = null;
				}
				else
				{
					AttachTransform at = AttachTransform.LockPosition(trail, properties._lockXOffset, properties._lockYOffset, properties._lockZOffset);
					tri.SetAttachTransform(at);
				}
			}
			
			return trail;
		}

		public static GameObject InstantiateDynamicLight (DynamicLightEventProperties properties, Animator animator, bool flipped, bool bypassPools = false)
		{
			GameObject light = null;

			if (Application.isPlaying)
			{

				if (!bypassPools)
				{
					light = LoadLightWithPool(properties);
				}
				else 
				{
					light = LoadLightWithoutPool(properties);
				}

			}
			else
			{
				light = LoadLightWithoutPool(properties);
			}

			
			if(light != null)
			{
				//DynamicPointLightInstance lightInstance = light.GetComponent<DynamicPointLightInstance>();
	
				if (animator != null)
				{
					light.transform.parent = GetBodyPartTransform(animator, properties._attachment, properties._attachmentPath);
					light.transform.localPosition = properties._offset;
				}

				if (properties._parent)
				{
					AttachTransform.Attach(light, light.transform.parent, flipped, false);
				}
			}
			
			return light;
		}

		public static Dictionary<string, string> ParseEventProperties(string propertyString)
		{
			Dictionary<string, string> dict = new Dictionary<string, string>();
			
			string[] items = propertyString.Split(';');
			for (int i = 0; i < items.Length; i++)
			{
				if (!string.IsNullOrEmpty(items[i]))
				{
					string[] item = items[i].Split('=');
					if (item.Length == 2)
					{
						dict.Add(item[0], item[1]);
					}
				}
			}
			
			return dict;
		}

		public static string StringifyGradientColorKeys(Gradient g)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			for (int i = 0; i < g.colorKeys.Length; i++)
			{
				sb.Append(g.colorKeys[i].color);
				sb.Append("&");
				sb.Append(g.colorKeys[i].time);
				
				if (i < g.colorKeys.Length - 1)
					sb.Append("|");
			}

			return sb.ToString();
		}

		public static string StringifyAnimationCurve(AnimationCurve ac)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for (int i = 0; i < ac.keys.Length; i++)
			{
				sb.Append(string.Format("{0},{1},{2},{3}", new object[]{ ac.keys[i].time, ac.keys[i].value, ac.keys[i].inTangent, ac.keys[i].outTangent }));
				if (i < ac.keys.Length - 1)
					sb.Append("|");
			}

			return sb.ToString();
		}

		public static AnimationCurve ParseAnimationCurve(string s)
		{
			List<Keyframe> keys = new List<Keyframe>();

			string[] items 	= s.Split('|');
			Vector4 key = Vector4.zero;

			for (int i = 0; i < items.Length; i++)
			{
				string[] values = items[i].Split(',');

				float.TryParse(values[0], out key.x);
				float.TryParse(values[1], out key.y);
				float.TryParse(values[2], out key.z);
				float.TryParse(values[3], out key.w);

				keys.Add(new Keyframe(key.x, key.y, key.z, key.w));
			}

			return new AnimationCurve(keys.ToArray());;
		}

		public static Vector2 ParseVector2(string s)
		{
			Vector2 v = Vector2.zero;
			
			int start = s.IndexOf('(') + 1;
			string sub = s.Substring(start, s.IndexOf(')') - start);
			string[] items = sub.Split(',');
			
			float.TryParse(items[0], out v.x);
			float.TryParse(items[1], out v.y);

			return v;
		}

		public static Vector3 ParseVector3(string s)
		{
			Vector3 v = Vector3.zero;

			int start = s.IndexOf('(') + 1;
			string sub = s.Substring(start, s.IndexOf(')') - start);
			string[] items = sub.Split(',');
			
			float.TryParse(items[0], out v.x);
			float.TryParse(items[1], out v.y);
			float.TryParse(items[2], out v.z);
			
			return v;
		}

		public static Color ParseColor(string s)
		{
			Color color = Color.white;

			int start = s.IndexOf('(') + 1;
			string sub = s.Substring(start, s.IndexOf(')') - start);
			string[] items = sub.Split(',');

			float.TryParse(items[0], out color.r);
			float.TryParse(items[1], out color.g);
			float.TryParse(items[2], out color.b);
			float.TryParse(items[3], out color.a);

			return color;
		}

		public static Gradient ParseGradient(string s)
		{
			Gradient gradient = new Gradient();
			List<GradientColorKey> colorKeys = new List<GradientColorKey>();
			string[] pairs = s.Split(new char[]{'|'});
			for (int i = 0; i < pairs.Length; i++)
			{
				GradientColorKey key = new GradientColorKey();
				string[] items = pairs[i].Split(new char[]{'&'});
				key.color = MoveEditor.MoveUtils.ParseColor(items[0]);
				key.time = float.Parse(items[1]);
				colorKeys.Add(key);
			}
			
			gradient.colorKeys = colorKeys.ToArray();
			return gradient;
		}

		public static string ParseStringParam(Dictionary<string, string> dict, string key, string defaultValue = null)
		{
			string value = string.Empty;
			if( dict.TryGetValue(key, out value) )
			{
				return value;
			}
			return defaultValue;
		}

		public static bool ParseBoolParam(Dictionary<string, string> dict, string key, bool defaultValue = false)
		{
			string s = ParseStringParam( dict, key );
			if( s != null )
			{
				bool value = false;
				bool.TryParse( s, out value );
				return value;
			}
			return defaultValue;
		}

		public static float ParseFloatParam(Dictionary<string, string> dict, string key, float defaultValue = 0.0f)
		{
			string s = ParseStringParam( dict, key );
			if( s != null )
			{
				float value = 0.0f;
				float.TryParse( s, out value );
				return value;
			}
			return defaultValue;
		}

		public static int ParseIntParam(Dictionary<string, string> dict, string key, int defaultValue = 0)
		{
			string s = ParseStringParam( dict, key );
			if( s != null )
			{
				int value = 0;
				int.TryParse( s, out value );
				return value;
			}
			return defaultValue;
		}

		public static T ParseEnumParam<T>(Dictionary<string, string> dict, string key, T defaultValue )
		{
			string s = ParseStringParam( dict, key );
			if( s != null )
			{
				T value = (T)System.Enum.Parse(typeof(T), s);
				return value;
			}
			return defaultValue;
		}

		public static Vector3 ParseVector2Param(Dictionary<string, string> dict, string key, Vector2 defaultValue)
		{
			string s = ParseStringParam( dict, key );
			if( s != null )
			{
				Vector3 value = ParseVector2(s);
				return value;
			}
			return defaultValue;
		}

		public static Vector3 ParseVector3Param(Dictionary<string, string> dict, string key, Vector3 defaultValue)
		{
			string s = ParseStringParam( dict, key );
			if( s != null )
			{
				Vector3 value = ParseVector3(s);
				return value;
			}
			return defaultValue;
		}

		public static Color ParseColorParam(Dictionary<string, string> dict, string key, Color defaultValue)
		{
			string s = ParseStringParam( dict, key );
			if(s != null)
			{
				return ParseColor(s);
			}
			return defaultValue;
		}

		public static AnimationCurve ParseAnimationCurveParam(Dictionary<string, string> dict, string key, AnimationCurve defaultValue)
		{
			string s = ParseStringParam(dict, key);
			if (s != null)
			{
				return ParseAnimationCurve(s);
			}
			return defaultValue;
		}

		public static Gradient ParseGradientParam(Dictionary<string, string> dict, string key, Gradient defaultValue)
		{
			string s = ParseStringParam(dict, key);
			if (s != null)
			{
				return ParseGradient(s);
			}
			return defaultValue;
		}

		public static T ParseJsonParam<T>(Dictionary<string, string> dict, string key, T defaultValue)
		{
			string s = ParseStringParam(dict, key);
			if(s != null)
			{
				return GM.JSON.ToObject<T>(s);
			}

			return defaultValue;
		}

		public static float FlipWorldAngle(float angle)
		{
			angle = NormalizeAngle(angle);

			if (angle <= 90 || angle >= 270)
				angle = 360 - angle;
			else
				angle = 180 + (180 - angle);

			return angle;
		}


		// this function assumes that the particle is symmetrical along the XY plane 
		public static void FlipWorldRotationByXYPlane(Transform transform)
		{
			Vector3 xAxis = transform.right;
			Vector3 yAxis = transform.up;
			Vector3 zAxis = transform.forward;
			
		//	EB.Debug.Log ( xAxis + " "+yAxis + " "+zAxis );
			
			Vector3 newXAxis = xAxis;
			Vector3 newYAxis = yAxis;
			Vector3 newZAxis = zAxis;
			
			float absDotX = Mathf.Abs(Vector3.Dot(xAxis, Vector3.right));
			float absDotY = Mathf.Abs(Vector3.Dot(yAxis, Vector3.right));
			float absDotZ = Mathf.Abs(Vector3.Dot(zAxis, Vector3.right));
		//	EB.Debug.Log ( absDotX + " "+ absDotY + " " +absDotZ);
			if( absDotX > absDotY && absDotX > absDotZ )
			{
				newXAxis = Vector3.Reflect(xAxis, Vector3.forward);
				
				// figure out what's up
				
				if( Mathf.Abs(Vector3.Dot(yAxis, Vector3.up)) > Mathf.Abs(Vector3.Dot(zAxis, Vector3.up)) )
				{
				//	EB.Debug.Log ( "x y z");
					newYAxis = Vector3.Reflect(yAxis, Vector3.forward);
					newZAxis = Vector3.Normalize(Vector3.Cross(Vector3.Normalize(newXAxis), Vector3.Normalize(newYAxis)));
				}
				else
				{
				//	EB.Debug.Log ( "x z y");
					newZAxis = Vector3.Reflect(zAxis, Vector3.forward);
					newYAxis = Vector3.Normalize(Vector3.Cross(Vector3.Normalize(newZAxis), Vector3.Normalize(newXAxis)));
				}
				
			}
			else if( absDotY > absDotZ && absDotY > absDotX )
			{
				
				newYAxis = Vector3.Reflect(yAxis, Vector3.forward);
				
				// figure out what's up
				if( Mathf.Abs(Vector3.Dot(xAxis, Vector3.up)) > Mathf.Abs(Vector3.Dot(zAxis, Vector3.up)) )
				{
				//	EB.Debug.Log ( "y x z");
					newXAxis = Vector3.Reflect(xAxis, Vector3.forward);
					newZAxis = Vector3.Normalize(Vector3.Cross(Vector3.Normalize(newXAxis), Vector3.Normalize(newYAxis)));
				}
				else
				{
				//	EB.Debug.Log ( "y z x");
					newZAxis = Vector3.Reflect(zAxis, Vector3.forward);
					newXAxis = Vector3.Normalize(Vector3.Cross(Vector3.Normalize(newYAxis), Vector3.Normalize(newZAxis)));
				}
			}
			else
			{
				newZAxis = Vector3.Reflect(zAxis, Vector3.forward);
				
				// figure out what's up
				if( Mathf.Abs(Vector3.Dot(xAxis, Vector3.up)) > Mathf.Abs(Vector3.Dot(yAxis, Vector3.up)) )
				{
				//	EB.Debug.Log ( "z1 x y");
					newXAxis = Vector3.Reflect(xAxis, Vector3.forward);
					newYAxis = Vector3.Normalize(Vector3.Cross(Vector3.Normalize(newZAxis), Vector3.Normalize(newXAxis)));
				}
				else
				{
				//	EB.Debug.Log ( "z y x");
					newYAxis = Vector3.Reflect(yAxis, Vector3.forward);
					newXAxis = Vector3.Normalize(Vector3.Cross(Vector3.Normalize(newYAxis), Vector3.Normalize(newZAxis)));
				}
			}
			
			//EB.Debug.Log ( newXAxis+" " + newYAxis + " "+newZAxis );
			Vector3.OrthoNormalize(ref newXAxis, ref newYAxis, ref newZAxis);
			//EB.Debug.Log ( newXAxis+" " + newYAxis + " "+newZAxis );
			transform.rotation = Quaternion.LookRotation( newZAxis, newYAxis );
		}

		public static float FlipLocalAngle(float angle)
		{
			angle = NormalizeAngle(angle);

			if (angle < 180)
				angle = 180 - angle;
			else
				angle = (360 - angle) + 180;

			return angle;
		}
		
		public static Vector3 NormalizeAngles(Vector3 angles)
		{
			return new Vector3(NormalizeAngle(angles.x), NormalizeAngle(angles.y), NormalizeAngle(angles.z));
		}

		public static float NormalizeAngle(float angle)
		{
			while (angle < 0)
				angle += 360;
			while (angle >= 360)
				angle -= 360;
			
			return angle;
		}

		public static string StringfyToJson(object obj)
		{
			if(obj == null)
			{
				return string.Empty;
			}
			return GM.JSON.ToJson(obj);
		}

		static List<CameraMotionOption> _camera_motion_lottery = new List<CameraMotionOption>();
		static List<float>				_camera_motion_cdf = new List<float>();
		public static CameraMotionOption GetCamermotionLottery(ref List<CameraMotionOption> options, bool isCritical)
		{
			_camera_motion_lottery.Clear();
			//int count = options.Count;
			for(int i = 0; i < options.Count; i++)
			{
				if(!(options[i]._isCritical ^ isCritical))  //相等
				{
					if(_camera_motion_lottery.Count == 0)
					{
						_camera_motion_lottery.Add(options[i]);
					}
					else //如果count大于1，则进行从小到大的排序
					{
						int index = 0;
						for(index = 0; index < _camera_motion_lottery.Count; index++)
						{
							if(options[i]._probability < _camera_motion_lottery[index]._probability)
							{
								break;
							}
						}
						if(index < _camera_motion_lottery.Count)
						{
							_camera_motion_lottery.Insert(index, options[i]);
						}
						else
						{
							_camera_motion_lottery.Add(options[i]);
						}
					}
				}
			}

			if(_camera_motion_lottery.Count == 0)
			{
				return null;
			}

			_camera_motion_cdf.Clear();
			int cdf_count = _camera_motion_lottery.Count;
			float sum = 0.0f;
			for(int i = 0; i < cdf_count; i++)
			{
				sum += _camera_motion_lottery[i]._probability;
				_camera_motion_cdf.Add(sum);
			}

			//Make sure we got 1.0f to avoid 0.999999f
			if(Mathf.Approximately(_camera_motion_cdf[cdf_count - 1], 1.0f))
			{
				_camera_motion_cdf[cdf_count - 1] = 1.0f;
			}

			float rnd = UnityEngine.Random.Range(0.0f, 1.0f);

			int cdf_index = 0;
			for(cdf_index = 0; cdf_index < cdf_count; cdf_index++)
			{
				if(rnd < _camera_motion_cdf[cdf_index])
				{
					break;
				}
			}
			if(cdf_index >= cdf_count)
			{
				return null;
			}
			else
			{
				return _camera_motion_lottery[cdf_index];
			}
		}	
	}
}
