using UnityEngine;
using System.Collections;
using Hotfix_LT.UI;
using System.Collections.Generic;

namespace Hotfix_LT.Data
{
	public class DialogueTemplateManager
	{

		private Dictionary<int, DialogueData> m_Dialogues = new Dictionary<int, DialogueData>();
		public Dictionary<int, DialogueData> Dialogues
		{
			get
			{
				return m_Dialogues;
			}
		}
		GM.DataCache.ConditionGuide conditionSet;
		private static DialogueTemplateManager m_Instance;
		public static DialogueTemplateManager Instance
		{
			get
			{
				if (m_Instance == null)
				{
					m_Instance = new DialogueTemplateManager();
				}
				return m_Instance;
			}
		}

		public static void ClearUp()
		{
			if (m_Instance != null)
			{
				m_Instance.m_Dialogues.Clear();
			}
		}

		public void InitDialogueData(GM.DataCache.ConditionGuide data)
		{
			if (data == null) return;

			conditionSet = data;
			//for (int i = 0; i < conditionSet.DialogueLength; i++)
			//{
			//	var step_h = conditionSet.GetDialogue(i);
			//	int dialogueid = step_h.DialogueId;
			//	int stepnum = step_h.StepNum;
   //             DialogueData dialogue;
   //             if (!m_Dialogues.ContainsKey(dialogueid))
   //             {
   //                 dialogue = new DialogueData(dialogueid, stepnum);
   //                 m_Dialogues.Add(dialogueid, dialogue);
   //             }
   //             else
   //             {
   //                 dialogue = m_Dialogues[dialogueid] as DialogueData;
   //             }

   //             dialogue.AddStep(step_h);
   //         }
		}

		public DialogueData GetDialogueData(int id)
		{
            InitDialogueData();
            
            m_Dialogues.TryGetValue(id, out DialogueData result);
			return result;
		}

        public void InitDialogueData()
        {
            if (m_Dialogues.Count < 1)
            {
                for (int i = 0; i < conditionSet.DialogueLength; i++)
                {
                    var step_h = conditionSet.GetDialogue(i);
                    int dialogueid = step_h.DialogueId;
                    int stepnum = step_h.StepNum;
                    if (m_Dialogues.TryGetValue(dialogueid, out DialogueData info) == false)
                    {
                        info = new DialogueData(dialogueid, stepnum);
                        m_Dialogues.Add(dialogueid, info);
                    }
                    info.AddStep(step_h);
                }
            }
        }

		public DialogueStepData GetDialogueStepDataByCharcterID(int char_id)
		{
			DialogueStepData defaultStepData = null;

			var enumerator = m_Dialogues.Values.GetEnumerator();
			while(enumerator.MoveNext())
			{
				DialogueData dialogueData = enumerator.Current;

				int len = dialogueData.Steps.Length;
				for (int i = 0; i < len; i++)
                {
					var step = dialogueData.Steps[i];
					if (step.Icon == char_id.ToString())
						return step;
					else if (step.Layout == 1)
						defaultStepData = step;
				}
			}

			EB.Debug.LogWarning("GetDialogueStepDataByCharcterID Fail so use other DialogueStepData char_id:{0}", char_id);
			return defaultStepData;
		}

	}
}