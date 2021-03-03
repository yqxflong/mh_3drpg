using UnityEngine;
using System.Collections;
using _HotfixScripts.Utils;
using Unity.Jobs;
using UnityEngine.Jobs;
namespace Hotfix_LT.Combat
{
	public class HealthBar3D : Hotfix_LT.DynamicMonoHotfix, IHotfixUpdate
	{ 
		private GameObject healthBar = null;
		private GameObject m_cameraObj;

		private Material m_healthBarMat;
		private bool m_isLeft;

		private bool m_isHealthBarSet = false;
		private bool m_isSideSet = false;
		private bool m_isPosSet = false;

		//private float m_timeFactor = .05f;

		private int m_maxHP;
		public int MaxHP
		{
			get
			{
				return m_maxHP;
			}
		}

		private int m_currentHP;
		private float m_targetHpValue;

		[System.Serializable]
		public struct CutoffRangeValue
		{
			public float Min;
			public float Max;

			public CutoffRangeValue(float min, float max)
			{
				Min = min;
				Max = max;
			}
		}
		public CutoffRangeValue CutoffRange = new CutoffRangeValue(0.01f, 1);

		private float m_hpValue
		{
			get
			{
				if (null != m_healthBarMat)
				{
					return m_healthBarMat.GetFloat("_Cutoff");
				}
				else
				{
					EB.Debug.LogError("HealthBar not found!");
					InitHealthBar();
					return 0;
				}
			}
			set
			{
				if (null != m_healthBarMat)
				{
					value = CutoffRange.Min + (CutoffRange.Max - CutoffRange.Min) * value;
					m_healthBarMat.SetFloat("_Cutoff", value);
				}
				else
				{
					EB.Debug.LogError("HealthBar not found!");
					InitHealthBar();
				}
			}
		}

		// Use this for initialization
	    public override void Start()
		{
			healthBar = null;

			InitHealthBar();
		}

		public override void OnEnable()
		{
			//base.OnEnable();
			RegisterMonoUpdater();
		}
        public override void OnDisable()
        {
            base.OnDisable();
            ErasureMonoUpdater();
        }
        public override void OnDestroy()
		{
			if (healthBar != null)
			{
				GameObject.Destroy(healthBar);
			}
		}

		public void Update()
		{
			if (m_isHealthBarSet && m_isSideSet && !m_isPosSet)
			{
				SetHealthBarData();
				m_isPosSet = true;
			}

			if (null == m_cameraObj)
			{
				if (Camera.main != null)
				{
					m_cameraObj = Camera.main.gameObject;
				}
			}

			if (null != healthBar && null != m_cameraObj)
			{
				Vector3 targetPos = new Vector3(m_cameraObj.transform.position.x, healthBar.transform.position.y, m_cameraObj.transform.position.z);
				healthBar.transform.LookAt(targetPos);
				healthBar.transform.Rotate(Vector3.up * 180);
			}

			if (m_isHealthBarSet && 0 != m_maxHP)
			{
				SetHealthBar();
			}
		}

		void InitHealthBar()
		{
			// modify by zx
			//if(GameEngine.Instance != null) {
			//	GM.AssetManager.GetAsset<GameObject>("CircularHealthBar", HealthBarDownload, GameEngine.Instance.gameObject);
			//}
		}

		public void DestroyHealthBar()
		{
			if (healthBar != null)
			{
				GameObject.Destroy(healthBar);
			}
		}

		void HealthBarDownload(string assetname, GameObject go, bool bSuccessed)
		{
			healthBar = go;
			Transform locator = mDMono.gameObject.transform.parent.Find("HealthBarTarget");
			healthBar.transform.SetParent(locator == null ? mDMono.gameObject.transform.parent : locator);
			healthBar.transform.localPosition = Vector3.zero;
			healthBar.gameObject.transform.rotation = Quaternion.identity;

			Transform healthBarRenderer = healthBar.transform.Find("HealthBarRenderer");
			m_healthBarMat = healthBarRenderer.gameObject.GetComponent<Renderer>().material;
			m_hpValue = 0;

			// comment by huangzhijun
			//m_isHealthBarSet = true;
			m_isHealthBarSet = false;
			healthBarRenderer.gameObject.CustomSetActive(false);
		}

		public void SetHealthBarSide(bool isLeft)
		{
			if (healthBar == null)
			{
				InitHealthBar();
			}

			m_isLeft = isLeft;
			m_isSideSet = true;
		}

		void SetHealthBarData()
		{
			//float posFactor = m_isLeft ? 1 : -1;

			if (null != healthBar)
			{
				m_healthBarMat.color = m_isLeft ? Color.green : Color.red;
			}
		}

		public void SetMaxHp(int maxHp)
		{
			m_maxHP = maxHp;
		}

		public void SetCurrentHp(int currentHp)
		{
			m_currentHP = currentHp;
		}

		JobUpdateSetHealthBar mJobUpdateSetHealthBar;
		JobHandle mJobHandleSetHealthBar;

		void SetHealthBar()
		{
			//EB.Debug.Log(">>>>HealthBar: " + transform.parent + " : " + m_currentHP+" >>" + m_maxHP);
			if (m_currentHP > 0)
			{
				if (m_maxHP > 0)
				{
					healthBar.CustomSetActive(true);

					mJobUpdateSetHealthBar = new JobUpdateSetHealthBar()
					{
						m_currentHP = m_currentHP,
						m_maxHP = m_currentHP,
					};

					TransformAccessArray accessArray = new TransformAccessArray();
					mJobHandleSetHealthBar = mJobUpdateSetHealthBar.Schedule(accessArray);

					accessArray.Dispose();
					mJobHandleSetHealthBar.Complete();
					JobHandle.ScheduleBatchedJobs();

					//float hpPercentageValue = Mathf.Clamp01((float)m_currentHP / m_maxHP);
					//// anything under this minimum is not visible in our health bar
					//const float hpMin = 0.0175f;
					//hpPercentageValue = hpMin + hpPercentageValue * (1.0f - hpMin);
					////EB.Debug.Log("hpPercentageValue: " +hpPercentageValue);
					//m_targetHpValue = 1 - hpPercentageValue;
					// StartCoroutine(StartHealthBarAnimation(m_hpValue));
					//EB.Debug.Log("m_hpValue: " +m_hpValue);
				}
			}
			else
			{
				healthBar.CustomSetActive(false);
			}
		}

		private IEnumerator StartHealthBarAnimation(float previousValue)
		{
			float time = 0;
			// lerp DOWN
			if (previousValue < m_targetHpValue)
			{
				while (CutoffRange.Min < (m_targetHpValue - m_hpValue))
				{
					//EB.Debug.Log("m_hpValue: "+m_hpValue+" previousValue: "+previousValue+"m_targetHpValue: "+m_targetHpValue+" time: "+time);
					m_hpValue = Mathf.Lerp(previousValue, m_targetHpValue, 1.0f - Mathf.Cos(time * Mathf.PI * 0.5f));
					time += Time.deltaTime * 2.0f;
					yield return null;
				}
			}
			// lerp UP
			else if (previousValue > m_targetHpValue)
			{
				while (CutoffRange.Min < (m_hpValue - m_targetHpValue))
				{
					//EB.Debug.Log("m_hpValue: "+m_hpValue+" previousValue: "+previousValue+"m_targetHpValue: "+m_targetHpValue+" time: "+time);
					m_hpValue = Mathf.Lerp(previousValue, m_targetHpValue, 1.0f - Mathf.Cos(time * Mathf.PI * 0.5f));
					time += Time.deltaTime * 2.0f;
					yield return null;
				}
			}
			yield break;
		}
	}
}