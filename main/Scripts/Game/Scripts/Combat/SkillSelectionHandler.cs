using UnityEngine;
using System.Collections;

public class SkillSelectionHandler : MonoBehaviour
{
	public class SkillUpdate
	{
		public int skillIndex;
		public bool showQTE;
		public bool qteActivated;

		public SkillUpdate (IDictionary data_dic)
		{
			skillIndex = int.Parse(data_dic["skillIndex"].ToString());
			showQTE = (bool)data_dic["showQTE"];
			qteActivated = (bool)data_dic["qteActivated"];
		}
	}

	GameObject m_qte;
	GameObject[] m_skills;
	UITexture[] m_uitextures;
	const int kMaxCombatSkills = 10;
	
	void Start()
	{
		m_qte = GameObject.Find ("QTE");
		m_skills = new GameObject[kMaxCombatSkills];
		m_uitextures = new UITexture[kMaxCombatSkills];
		for(int ii = 0; ii < kMaxCombatSkills; ++ii)
		{
			m_skills[ii] = GameObject.Find (string.Format("Skill{0}", ii));
			if(m_skills[ii] != null)
			{
				m_uitextures[ii] = FindUITexture(m_skills[ii]);
			}
		}
		HideQTE();
	}

	// public void TriggerSkill() {
	// 	FindObjectOfType<PlayMakerFSM>().Fsm.BroadcastEvent("UpdateSkillSelection");
	// }
	
	public void OnSkillSelected(SkillUpdate myUpdate)
	{
		Color selectedColor = myUpdate.qteActivated ? Color.green : Color.yellow;
		for(int ii = 0; ii < kMaxCombatSkills; ++ii)
		{
			if(m_uitextures[ii] != null)
			{
				m_uitextures[ii].color = (myUpdate.skillIndex == ii) ? selectedColor : Color.white;
			}
		}
		if(myUpdate.showQTE)
		{
			ShowQTE();
		}
	}
	
	public void OnLocalPlayerAttacks()
	{
		for(int ii = 0; ii < kMaxCombatSkills; ++ii)
		{
			if(m_uitextures[ii] != null)
			{
				m_uitextures[ii].color = Color.white;
			}
		}
		HideQTE();
	}

	public void OnPlayerDied()
	{
		for(int ii = 0; ii < kMaxCombatSkills; ++ii)
		{
			if(m_uitextures[ii] != null)
			{
				m_uitextures[ii].gameObject.transform.parent.gameObject.CustomSetActive(false);
			}
		}
		HideQTE();
	}

	public void OnQTEFinished()
	{
		HideQTE();
	}

	public void HideQTE()
	{
		if(m_qte != null)
		{
			m_qte.CustomSetActive(false);
		}
	}
	
	public void ShowQTE()
	{
		if(m_qte != null)
		{
			m_qte.CustomSetActive(true);
		}
	}
	
	UITexture FindUITexture(GameObject skillNode)
	{
		foreach(Transform tt in skillNode.transform)
		{
			if(tt.gameObject != null)
			{
				UITexture uit = tt.gameObject.GetComponent<UITexture>();
				if(uit != null)
				{
					return uit;
				}
			}
		}
		return null;
	}
}

