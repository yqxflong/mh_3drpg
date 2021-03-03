using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Hotfix_LT.Combat
{
	public class StoneBuffEffect : BuffEffect
	{
		public StoneBuffEffect(MoveEditor.BuffEventProperties props)
		{
			m_EventProps = props;
			m_Type = BuffEffectManager.BuffType.Stone;
			if (m_EventProps == null)
			{
				return;
			}
			m_EffectLerpInTotalTime = m_EventProps._lastFrame;
		}

		public override bool IsLerpingInEffect()
		{
			if (m_EffectLerpInStartTime == -1.0f)
			{
				return false;
			}
			return true;
		}

		public override bool IsLerpingOutEffect()
		{
			if (m_EffectLerpOutStartTime == -1.0f)
			{
				return false;
			}
			return true;
		}

		public override void Update(Combatant cb)
		{
			LerpIn(cb);
			LerpOut(cb);
			//no other effects need to change per frame now.
		}

		protected override void LerpIn(Combatant cb)
		{
			if (!IsLerpingInEffect())
			{
				return;
			}
			float frame = Time.time - m_EffectLerpInStartTime;
			float lerpValue = frame / m_EffectLerpInTotalTime;

			lerpValue = Mathf.Min(lerpValue, 1);

			LerpStoneEffect(lerpValue, cb);
			if (lerpValue >= 1)
			{
				m_EffectLerpInStartTime = -1.0f;
			}
		}

		protected override void LerpOut(Combatant cb)
		{
			if (!IsLerpingOutEffect())
			{
				return;
			}
			float frame = Time.time - m_EffectLerpOutStartTime;
			float lerpValue = frame / m_EffectLerpOutTotalTime;

			lerpValue = Mathf.Min(lerpValue, 1);

			LerpStoneEffect(1 - lerpValue, cb);
			if (lerpValue >= 1)
			{
				m_EffectLerpOutStartTime = -1.0f;
			}
		}

		protected override void StartLerpIn(Combatant cb)
		{
			m_EffectLerpOutStartTime = -1.0f;
			m_EffectLerpInStartTime = Time.time;

			SkinnedMeshRenderer[] renderers = cb.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
			if (renderers.Length == 0 || renderers[0].materials.Length == 0)
			{
				return;
			}

			m_animator_speed = cb.gameObject.GetComponent<Animator>().speed;

			m_grayscale = renderers[0].materials[0].GetFloat("_GrayScale");
			m_constract = renderers[0].materials[0].GetFloat("_ContrastIntansity");
			m_brightness = renderers[0].materials[0].GetFloat("_Brightness");
			m_finalcolor = renderers[0].materials[0].GetColor("_FinalColor");

			for (int i = 0; i < renderers.Length; i++)
			{
				if (m_specintensityarr.Count < i + 1)
				{
					m_specintensityarr.Add(new List<float>());
				}
				if (m_specglossarr.Count < i + 1)
				{
					m_specglossarr.Add(new List<float>());
				}


				for (int j = 0; j < renderers[i].materials.Length; j++)
				{
					if (m_specintensityarr[i].Count < j + 1)
					{
						m_specintensityarr[i].Add(renderers[i].materials[j].GetFloat("_SpecularIntensity"));
					}
					else
					{
						m_specintensityarr[i][j] = renderers[i].materials[j].GetFloat("_SpecularIntensity");
					}
					if (m_specglossarr[i].Count < j + 1)
					{
						m_specglossarr[i].Add(renderers[i].materials[j].GetFloat("_SpecularGlossModulation"));
					}
					else
					{

						m_specglossarr[i][j] = renderers[i].materials[j].GetFloat("_SpecularGlossModulation");
					}
				}
			}
		}

		protected override void StartLerpOut(Combatant cb)
		{
			m_EffectLerpInStartTime = -1.0f;
			m_EffectLerpOutStartTime = Time.time;
		}


		void LerpStoneEffect(float lerpValue, Combatant cb)
		{
			RenderSettings rs = (RenderSettings)RenderSettingsManager.Instance.GetCurrentRenderSettings();
			SkinnedMeshRenderer[] renderers = cb.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

			float gs = Mathf.Lerp(m_grayscale, rs.CharactorGrayScale, lerpValue);
			float ct = Mathf.Lerp(m_constract, rs.CharactorContrast, lerpValue);
			float bg = Mathf.Lerp(m_brightness, rs.CharactorBrightness, lerpValue);
			Color fc = Color.Lerp(m_finalcolor, rs.CharactorFinalColor, lerpValue);
			//float si = Mathf.Lerp (m_specinstance, rs.CharactorSpecularIntensity, lerpValue);

			float sg;
			float ni;


			for (int i = 0; i < renderers.Length; i++)
			{
				for (int j = 0; j < renderers[i].materials.Length; j++)
				{
					if (m_specglossarr.Count >= i + 1 && m_specglossarr[i].Count >= j + 1)
					{
						sg = Mathf.Lerp(m_specglossarr[i][j], rs.CharactorSpecularGloss, lerpValue);
						renderers[i].materials[j].SetFloat("_SpecularGlossModulation", sg);
					}

					if (m_specintensityarr.Count >= i + 1 && m_specintensityarr[i].Count >= j + 1)
					{
						ni = Mathf.Lerp(m_specintensityarr[i][j], rs.CharactorSpecularIntensity, lerpValue);
						renderers[i].materials[j].SetFloat("_SpecularIntensity", ni);
					}

					renderers[i].materials[j].SetFloat("_GrayScale", gs);
					renderers[i].materials[j].SetFloat("_ContrastIntansity", ct);
					renderers[i].materials[j].SetFloat("_Brightness", bg);
					renderers[i].materials[j].SetColor("_FinalColor", fc);
				}
			}
			cb.gameObject.GetComponent<Animator>().speed = Mathf.Lerp(m_animator_speed, 0, lerpValue);
		}

		private float m_grayscale = 0.0f;
		private float m_constract = 1.0f;
		private float m_brightness = 0.0f;
		private Color m_finalcolor = new Color(255, 255, 255, 0);

		//private float m_specinstance = 2.0f;
		//private float m_specgloss = 8.0f;
		private List<List<float>> m_specintensityarr = new List<List<float>>();
		private List<List<float>> m_specglossarr = new List<List<float>>();

		private float m_animator_speed = -1.0f;
	}

}