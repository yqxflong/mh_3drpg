using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Hotfix_LT.UI
{
    public class CombatTargetSelectController
    {
        public class SingleTarget
        {
            public int IngameId;
            public string name;
            public Vector3 screenPosition;
            public float radius;
        }

        public int TargetTeam
        {
            get
            {
                return targetTeam;
            }
        }
        bool setupDone;
        bool lastStatePressed;
        bool lastStateOverUI;
        public bool ignoreInput;
        int targetTeam = -1;
        Hotfix_LT.Combat.CombatantIndex skillSender;
        const float outlineWidth = 0.002f;
        Color combatOrange = new Color(1.0f, 102.0f / 255.0f, 0.0f, 1.0f);

        public bool NeedsTargets
        {
            get
            {
                return (myTargets == null || myTargets.Length == 0) && ignoreInput == false;
            }
        }
        SingleTarget[] myTargets;


        private static CombatTargetSelectController instance;
        public static CombatTargetSelectController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CombatTargetSelectController();
                }
                return instance;
            }
        }

        public void SetTargetingInfo(Hotfix_LT.Combat.CombatantIndex sender, int teamIndex, int maxTargetsForSkill, List<int> targetIndices)
        {
            setupDone = false;
            ignoreInput = false;

            skillSender = sender;
            targetTeam = teamIndex;
            myTargets = new SingleTarget[targetIndices.Count];
            TryDoSetup(targetIndices);
        }

        #region CPU耗时 2.94ms
        void TryDoSetup(List<int> targetIndicesCached)
        {
            if (setupDone)
                return;
            if (myTargets.Length == 0)
                return;

            setupDone = true;

            LTCombatEventReceiver.Instance.ForEach(combatant =>
            {
                if(combatant.Index == null){
                    return;
                }
                
                if (combatant.IsLaunch())
                {
                    combatant.colorScale.SetIrrelevance(false);
                }
                else if (combatant.Index.TeamIndex != targetTeam && !combatant.Index.Equals(skillSender))
                {
                    combatant.colorScale.SetIrrelevance(true);
                }
                else if (combatant.Index.TeamIndex == targetTeam && targetIndicesCached.IndexOf(combatant.Index.IndexOnTeam) < 0)
                {
                    combatant.colorScale.SetIrrelevance(true);
                }
                else
                {
                    combatant.colorScale.SetIrrelevance(false);
                }

                combatant.RemoveSelectionFX(false);
                if (combatant.Index.Equals(skillSender))
                {
                    if (!LTCombatHudController.Instance.AutoMode)
                    {
                        combatant.SetupSelectFX();
                    }
                }
                else if (targetTeam != skillSender.TeamIndex && combatant.Index.TeamIndex == targetTeam && targetIndicesCached.IndexOf(combatant.Index.IndexOnTeam) >= 0)
                {
                    combatant.SetupSelectableFX();
                }
            });

            for (int ii = 0; ii < myTargets.Length; ++ii)
            {
                Hotfix_LT.Combat.Combatant cc = LTCombatEventReceiver.Instance.GetCombatant(targetTeam, targetIndicesCached[ii]);
                myTargets[ii] = new SingleTarget();
                myTargets[ii].IngameId = cc.Data.IngameId;
                myTargets[ii].name = cc.myName;
            }
        }
        #endregion

        public void OnSkillSelectionDone()
        {
            myTargets = null;
            skillSender = null;
            targetTeam = -1;
        }

        /// <summary>
        /// 筛选技能选择的目标
        /// </summary>
        /// <param name="Pos"></param>
        /// <returns>目标</returns>
        private Hotfix_LT.Combat.Combatant SearchTargetByClickPos(Vector3 Pos)
        {
            if (LTCombatHudController.Instance.AutoMode)
            {
                targetTeam = 1 - CombatLogic.Instance.LocalPlayerTeamIndex;
                LTCombatHudController.Instance.CombatSkillCtrl.SetConvergeTargeting();
            }

            var closestTarget = FindClosestTarget(Pos);

            if (closestTarget != null)
            {
                if (MyFollowCamera.delTouchCharacter != null)
                {
                    MyFollowCamera.delTouchCharacter();
                }
            }

            if (closestTarget != null)
            {
                var pc = Camera.main.WorldToScreenPoint(closestTarget.transform.position);
                var pd = new Vector3(Pos.x, Pos.y, pc.z);
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(pd);
                closestTarget.PlaySelectedFX(worldPos);
            }
            return closestTarget;
        }

        Vector2 lasttouch;
        /// <summary>
        /// 更新释放技能的目标
        /// </summary>
        /// <returns>目标</returns>
        public Hotfix_LT.Combat.Combatant UpdateTargets()
        {
            if (myTargets == null || ignoreInput || !LTCombatEventReceiver.IsCombatInit() || !LTCombatEventReceiver.Instance.Ready)
            {
                return null;
            }

            if (LTCombatHudController.Instance != null && !LTCombatHudController.Instance.CombatHudVisible())
            {
                return null;
            }

            if (ChatHudController.sOpen || FriendHudController.sOpen)
            {
                return null;
            }

            if (setupDone == false)
            {
                if (LTCombatHudController.Instance.AutoMode)
                {
                    targetTeam = 1 - CombatLogic.Instance.LocalPlayerTeamIndex;
                    LTCombatHudController.Instance.CombatSkillCtrl.SetConvergeTargeting();
                }
                return null;
            }

            if (UICamera.currentScheme == UICamera.ControlScheme.Touch)
            {
                if (ILRDefine.UNITY_EDITOR)
                {
                    EB.Debug.LogError("UICamera.currentScheme == UICamera.ControlScheme.Touch & UNITY_EDITOR");
                    UICamera.currentScheme = UICamera.ControlScheme.Mouse;
                    return null;
                } else
                {
                    if(Input.touchCount == 0)
                    {
                        if(lastStatePressed)
                        {
                            lastStatePressed = false;
                            if(lastStateOverUI)
                            {
                                return null;
                            }
                            else
                            {
                                return SearchTargetByClickPos(lasttouch);
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                    lasttouch = TouchPos;
                }
            }
            else if (UICamera.currentScheme == UICamera.ControlScheme.Mouse)
            {
                if (Input.GetMouseButton(0) == false)
                {
                    if (lastStatePressed)
                    {
                        lastStatePressed = false;
                        if (lastStateOverUI)
                        {
                            return null;
                        }
                        else
                        {
                            return SearchTargetByClickPos(Input.mousePosition);

                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                lasttouch = Input.mousePosition;
            }
            else
            {
                return null;
            }

            if (!lastStatePressed)
            {
                lastStatePressed = true;
                lastStateOverUI = CurrentTouchOverUI();
            }

            return null;
        }

        Vector2 TouchPos { get { return Input.mousePosition; } }

        private bool CurrentTouchOverUI()
        {
            var t = new UICamera.MouseOrTouch();
            t.pos = lasttouch;
            UICamera.Raycast(t);
            if (t.current != null && t.current != UICamera.fallThrough && NGUITools.FindInParents<UIRoot>(t.current) != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 查找技能选择的目标
        /// </summary>
        /// <param name="location"></param>
        /// <returns>目标</returns>
        private Hotfix_LT.Combat.Combatant FindClosestTarget(Vector3 location)
        {
            try
            {
                Hotfix_LT.Combat.Combatant result = null;
                float minDistance = float.MaxValue;
                for (int ii = 0; ii < myTargets.Length; ++ii)
                {
                    Hotfix_LT.Combat.Combatant cc = LTCombatEventReceiver.Instance.GetCombatantByIngameId(myTargets[ii].IngameId);
                    if (cc == null)
                    {
                    EB.Debug.LogWarning("FindClosestTarget cc is null");
                        continue;
                    }

                    float teamMultiplier = targetTeam == 1 ? 1.0f : -1.0f;
                    Vector3 screenPositionTop, screenPositionBottom;
                    float scaleFactor = cc.transform.localScale.x;
                    if (cc.Collider.direction == 1 || cc.Collider.direction == 2)
                    {
                        float bottomFrontZ, topBackY, topBackZ;
                        if (cc.Collider.direction == 1)
                        {
                            bottomFrontZ = cc.Collider.radius * scaleFactor;
                            topBackY = cc.Collider.height * scaleFactor;
                            topBackZ = cc.Collider.radius * scaleFactor;
                        }
                        else
                        {
                            bottomFrontZ = cc.Collider.height / 2 * scaleFactor;
                            topBackY = cc.Collider.center.y + cc.Collider.radius * scaleFactor;
                            topBackZ = cc.Collider.height / 2 * scaleFactor;
                        }
                        Vector3 bottomFront = (cc.gameObject.transform.rotation * (new Vector3(0.0f, 0.0f, teamMultiplier * bottomFrontZ))) + cc.OriginPosition;
                        teamMultiplier *= -1.0f;
                        Vector3 topBack = (cc.gameObject.transform.rotation * (new Vector3(0.0f, topBackY, teamMultiplier * topBackZ))) + cc.OriginPosition;
                        screenPositionTop = Camera.main.WorldToScreenPoint(topBack);
                        screenPositionBottom = Camera.main.WorldToScreenPoint(bottomFront);
                    }
                    else
                    {
                        float bottomLeftX, topRightX, topRightY;
                        bottomLeftX = cc.Collider.height / 2 * scaleFactor;
                        topRightX = cc.Collider.height / 2 * scaleFactor;
                        topRightY = cc.Collider.center.y + cc.Collider.radius * scaleFactor;
                        teamMultiplier *= -1.0f;
                        Vector3 bottomLeft = (cc.gameObject.transform.rotation * (new Vector3(teamMultiplier * bottomLeftX, 0.0f, 0.0f))) + cc.OriginPosition;
                        teamMultiplier *= -1.0f;
                        Vector3 topRight = (cc.gameObject.transform.rotation * (new Vector3(teamMultiplier * topRightX, topRightY, 0.0f))) + cc.OriginPosition;
                        screenPositionTop = Camera.main.WorldToScreenPoint(topRight);
                        screenPositionBottom = Camera.main.WorldToScreenPoint(bottomLeft);
                    }

                    myTargets[ii].screenPosition = (screenPositionTop + screenPositionBottom) / 2.0f;
                    myTargets[ii].radius = Vector3.Magnitude(screenPositionTop - screenPositionBottom) / 2.0f;

                    float distance = Vector3.Magnitude(myTargets[ii].screenPosition - location);
                    if (distance < myTargets[ii].radius && distance < minDistance)
                    {
                        minDistance = distance;
                        result = cc;
                    }
                }

                return result;
            }
            catch(System.NullReferenceException e)
            {
                EB.Debug.LogError(e.ToString());
                return null;
            }
        }

        public void SetAllToColor(Hotfix_LT.Combat.Combatant cc, Color myColor)
        {
            if (cc != null && cc.redRing != null)
            {
                cc.redRing.gameObject.CustomSetActive(myColor == Color.red);
                cc.orangeRing.gameObject.CustomSetActive(myColor == combatOrange);
                cc.blackRing.gameObject.CustomSetActive(false);
                cc.greenRing.gameObject.CustomSetActive(myColor == Color.green);
            }
        }

        private string GetTargetName(int index)
        {
            if (LTCombatEventReceiver.IsCombatInit())
                return LTCombatEventReceiver.Instance.GetCombatant(targetTeam, index).myName;
            return "already deleted";
        }
    }

}