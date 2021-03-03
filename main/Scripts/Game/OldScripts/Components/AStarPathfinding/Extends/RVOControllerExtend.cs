using UnityEngine;
using System.Collections.Generic;
using Pathfinding.RVO.Sampled;

#if APP_UPDATED

namespace Pathfinding.RVO
{
    public partial class RVOController
    {
        public static System.Action<ZoneDescriptor> ZoneChangedEvent;
        public static System.Action<GameObject, string> GroundTypeChangedEvent;

        private EnvironmentHelper.eEnvironmentType _lastEnvironmentType = EnvironmentHelper.eEnvironmentType.Unknown;
        private string _lastGroundType = "Unknown";

        public delegate void PreUpdateDelegate();
        public delegate void OnPositionChangedDelegate();

        public const float smoothDownTime = 12.8f;
        public const float smoothUpTime = 19.8f;

        /** Layer mask for which types of other agents should be visible to us (enemy, player etc) and what type we are*/
        public LayerMask agentsToCollideWith = -1;
        public LayerMask agentCollisionType = -1; // collision type just for RVO purposes

        // added radiusAgainstObstacles
        /* allow agent to stay a different distance to obstacles than agents */
        public float radiusAgainstObstacles = 5;

        /* a callback for when the rvocontroller updates the character position */
        private OnPositionChangedDelegate _onPositionChanged;

        /* a callback for before the rvo controller update function is called */
        private PreUpdateDelegate _preUpdate;

        // has teleport just been called
        private bool didTeleport = false;

        public Vector3 velocity
        {
#if BNICKSON_UPDATED
            get { return null != rvoAgent ? rvoAgent.Velocity : Vector3.zero; }
#else
            get { return rvoAgent.Velocity; }
#endif
        }

        /* this is the velocity which has been computed, but won't be applied until next frame (could be though of as the on-deck velocity) */
        public Vector3 newVelocity
        {
            get { return RvoAgent.NewVelocity; }
        }

        public bool ShouldSnapToGround
        {
            get;
            set;
        }

        /** Get the rvoAgent **/
        public Agent RvoAgent
        {
            get { return rvoAgent as Agent; }
        }

        public void RemoveAgentFromSimulation()
        {
            //Remove the agent from the simulation but keep the reference
            //this component might get enabled and then we can simply
            //add it to the simulation again
            Agent agentReal = rvoAgent as Agent;
            if (null != agentReal && agentReal.simulator == simulator)
            {
                simulator.RemoveAgent(rvoAgent);
            }
        }

        // add a callback to be called before the update
        public void AddPreUpdateCallback(PreUpdateDelegate callback)
        {
            _preUpdate += callback;
        }

        // remove a callback
        public void RemovePreUpdateCallback(PreUpdateDelegate callback)
        {
            _preUpdate -= callback;
        }

        // add a callback for when the position is updated by this class
        public void AddPositionChangedCallback(OnPositionChangedDelegate callback)
        {
            _onPositionChanged += callback;
        }

        // remove a position changed callback
        public void RemovePositionChangedCallback(OnPositionChangedDelegate callback)
        {
            _onPositionChanged -= callback;
        }

        // place the passed position on the ground
        public Vector3 PlaceGameObjectOnTheGround(Vector3 pos)
        {
            pos.y = CalculateGroundHeight(pos);
            return pos;
        }

        private int _groundLayer = -1;
        private LevelDescriptor _levelRoot;
        private GameObject _lastGroundGO;
        private float CalculateGroundHeight(Vector3 pos)
        {
            if (_groundLayer == -1)
            {
                _groundLayer = LayerMask.NameToLayer("Ground");
            }

            float result = 0f;
            RaycastHit hit;
            if (GameUtils.CalculateGroundHeight(pos, mask, out hit, ref result))
            {
                GameObject hitobj = hit.transform.gameObject;
                if (hitobj.layer == _groundLayer)
                {
                    if (PlayerManager.IsLocalPlayer(gameObject))
                    {

                        if (_levelRoot == null)
                        {
                            GameObject levelGO = GameObject.Find("Level Root");
                            _levelRoot = levelGO.GetComponent<LevelDescriptor>();
                        }

                        if (_levelRoot != null)
                        {
                            ZoneDescriptor zoneDescriptor = _levelRoot.GetZoneForPosition(pos);
                            if (zoneDescriptor != null)
                            {
                                // prevent event spamming
                                if (zoneDescriptor.environmentType != _lastEnvironmentType)
                                {
                                    _lastEnvironmentType = zoneDescriptor.environmentType;
                                    if (ZoneChangedEvent != null)
                                    {
                                        ZoneChangedEvent(zoneDescriptor);
                                    }
                                }
                            }
                        }
                    }

                    // Only compare strings when we know we are looking at a new gameobject
                    if (_lastGroundGO != hitobj)
                    {
                        _lastGroundGO = hitobj;
                        if (_lastGroundType != hitobj.tag)
                        {
                            if (GroundTypeChangedEvent != null)
                            {
                                GroundTypeChangedEvent(gameObject, hitobj.tag);
                            }
                            _lastGroundType = hitobj.tag;
                        }
                    }
                }
                return result;
            }
            return 0f;
        }

        // miscellaneous updates in Teleport function
        public void Teleport(Vector3 pos, bool doResetVelocity)
        {
            if (ShouldSnapToGround)
            {
                pos.y = CalculateGroundHeight(pos);
            }

            tr.position = pos;
            lastPosition = pos;
            RvoAgent.Teleport(pos, doResetVelocity);
            adjustedY = pos.y;

            didTeleport = true;
        }

        #region override functions

        public void Awake()
        {
            tr = transform;

			// Find the RVOSimulator in this scene
#if BNICKSON_UPDATED
			//cachedSimulator = cachedSimulator != null ? cachedSimulator : FindObjectOfType(typeof(RVOSimulator)) as RVOSimulator;
			if (AstarPath.active!=null)
			{
				cachedSimulator = AstarPath.active.GetComponent<RVOSimulator>();
			} 
#else
            cachedSimulator = cachedSimulator ?? FindObjectOfType(typeof(RVOSimulator)) as RVOSimulator;
#endif
            if (cachedSimulator == null)
            {
                EB.Debug.LogError("No RVOSimulator component found in the scene. Please add one.");
            }
            else {
                simulator = cachedSimulator.GetSimulator();
            }

#if BNICKSON_UPDATED
            ShouldSnapToGround = true;
            // don't use cache because no where to release
            cachedSimulator = null;
#endif
        }

        protected void UpdateAgentProperties()
        {
#if BNICKSON_UPDATED
            if (null == rvoAgent)
            {
                return;
            }

            // added agentCollisionMask, agentCollisionType, radiusAgainstObstacles
            RvoAgent.AgentsToCollideWith = agentsToCollideWith;
            RvoAgent.AgentCollisionType = agentCollisionType;
            RvoAgent.RadiusAgainstObstacles = radiusAgainstObstacles;
#endif
            rvoAgent.Radius = radius;
            rvoAgent.MaxSpeed = maxSpeed;
            rvoAgent.Height = height;
            rvoAgent.AgentTimeHorizon = agentTimeHorizon;
            rvoAgent.ObstacleTimeHorizon = obstacleTimeHorizon;
            rvoAgent.Locked = locked;
            rvoAgent.MaxNeighbours = maxNeighbours;
            rvoAgent.DebugDraw = debug;
            rvoAgent.NeighbourDist = neighbourDist;
            rvoAgent.Layer = layer;
            rvoAgent.CollidesWith = collidesWith;
        }

        public void Teleport(Vector3 pos)
        {
#if BNICKSON_UPDATED
            Teleport(pos, true);
#else
            tr.position = pos;
            lastPosition = pos;
            rvoAgent.Teleport(pos);
            adjustedY = pos.y;
#endif
        }

        public void Update()
        {
            if (!GameEngine.Instance.IsTimeToRootScene)
            {
                return;
            }
            //如果是副本中则不能进入rvo
            //ToDo:暂时屏蔽，方便解耦
            //if (LTInstanceMapModel.Instance.IsInstanceMap())
            //{
            //    return;
            //}
            if ((bool)GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.LTInstanceMapModel", "IsInstanceMapFromILR"))
            {
                return;
            }

            if (rvoAgent == null) return;
#if BNICKSON_UPDATED
            didTeleport = false;

            if (null != _preUpdate)
            {
                _preUpdate();
            }
#endif

            if (lastPosition != tr.position)
            {
                Teleport(tr.position);
            }

            if (lockWhenNotMoving)
            {
                locked = desiredVelocity == Vector3.zero;
            }

            UpdateAgentProperties();

#if !BNICKSON_UPDATED
            RaycastHit hit;
#endif

            //The non-interpolated position
            Vector3 realPos = rvoAgent.InterpolatedPosition;

#if BNICKSON_UPDATED
            // Adding a flag so we can turn this ground snapping off for leap
            if (ShouldSnapToGround && !locked)
            {
                realPos.y = adjustedY;

                if (!didTeleport) // if we did teleport, adjustedY would've been set correctly already
                {
                    // adjustedY = CalculateGroundHeight(realPos);
                    float calculatedHeight = CalculateGroundHeight(realPos);
                    float diff = calculatedHeight - adjustedY;
                    adjustedY = Mathf.Lerp(adjustedY, calculatedHeight, Time.deltaTime * (diff >= 0.0f ? smoothUpTime : smoothDownTime));
                }

                // smooth out position for stairs, etc.
                realPos.y = adjustedY;
                rvoAgent.SetYPosition(adjustedY);
            }
#else
            realPos.y = adjustedY;

            if (mask != 0 && Physics.Raycast(realPos + Vector3.up*height*0.5f, Vector3.down, out hit, float.PositiveInfinity, mask)) {
                adjustedY = hit.point.y;
            } else {
                adjustedY = 0;
            }
            realPos.y = adjustedY;

            rvoAgent.SetYPosition(adjustedY);
#endif

            Vector3 force = Vector3.zero;

            if (wallAvoidFalloff > 0 && wallAvoidForce > 0)
            {
                List<ObstacleVertex> obst = rvoAgent.NeighbourObstacles;

                if (obst != null) for (int i = 0; i < obst.Count; i++)
                    {
                        Vector3 a = obst[i].position;
                        Vector3 b = obst[i].next.position;

                        Vector3 closest = position - VectorMath.ClosestPointOnSegment(a, b, position);

                        if (closest == a || closest == b) continue;

                        float dist = closest.sqrMagnitude;
                        closest /= dist * wallAvoidFalloff;
                        force += closest;
                    }
            }

#if ASTARDEBUG
            Debug.DrawRay(position, desiredVelocity + force*wallAvoidForce);
#endif
            rvoAgent.DesiredVelocity = desiredVelocity + force * wallAvoidForce;

            tr.position = realPos + Vector3.up * height * 0.5f - center;
            lastPosition = tr.position;

            if (enableRotation && velocity != Vector3.zero) transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(velocity), Time.deltaTime * rotationSpeed * Mathf.Min(velocity.magnitude, 0.2f));

#if BNICKSON_UPDATED
            if (null != _onPositionChanged)
            {
                _onPositionChanged();
            }
#endif
        }

#endregion
    }
}

#endif