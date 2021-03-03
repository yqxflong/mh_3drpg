using UnityEngine;
using System.Collections;

public class Formations : MonoBehaviour
{
    // Singleton
    static Formations singleInstance;

    public static Formations Instance
    {
        get
        {
            return singleInstance;
        }
    }

    // Initialization
    void Start()
    {

    }

    void OnEnable()
    {
        singleInstance = this;
    }

    void OnDisable()
    {
        singleInstance = null;
    }

    //____________________________
    // Get a Position transform within a Formation
    //____________________________
    public Transform GetPositionTransform(CombatLogic.FormationSide side, string formation_name, string position_name)
    {
        Transform formation_transform = GetFormationTransform(side, formation_name);
        Transform position_transform = formation_transform.Find(position_name);
        //if (position_transform == null)
        //{
        //    EB.Debug.LogError("Failed to find position " + side.ToString() + " - " + formation_name + " - " + position_name);
        //}
        return position_transform;
    }

    //____________________________
    // Get a Formation transform within a side
    //____________________________
    public Transform GetFormationTransform(CombatLogic.FormationSide side, string formation_name)
    {
        Transform team_transform = GetTeamTransform(side);
        Transform formation_transform = team_transform.Find(formation_name);
        //if (formation_transform == null)
        //{
        //    EB.Debug.LogError("Failed to find formation " + side.ToString() + " - " + formation_name);
        //}
        return formation_transform;
    }

    //___________________________
    // Get a Team Transform
    //___________________________
    // Stashed for Effciency
    private Transform m_PlayerSide = null;
    private Transform m_OpponentSide = null;

    public Transform GetTeamTransform(CombatLogic.FormationSide side)
    {
        Transform team_transform = null;
        switch (side)
        {
            case CombatLogic.FormationSide.PlayerOrChallenger:
                {
                    if (m_PlayerSide == null)
                    {
                        m_PlayerSide = gameObject.transform.Find("PlayerOrChallengerSide");
                        if (m_PlayerSide == null)
                        {
                            EB.Debug.LogError("Failed to find formations team transform PlayerSide");
                        }
                    }
                    team_transform = m_PlayerSide;
                    break;

                }
            case CombatLogic.FormationSide.Opponent:
                {
                    if (m_OpponentSide == null)
                    {
                        m_OpponentSide = gameObject.transform.Find("OpponentSide");
                        if (m_OpponentSide == null)
                        {
                            EB.Debug.LogError("Failed to find formations team transform OpponentSide");
                        }
                    }
                    team_transform = m_OpponentSide;
                    break;
                }
        }

        return team_transform;
    }
}
