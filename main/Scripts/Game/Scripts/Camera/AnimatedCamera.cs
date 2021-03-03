using UnityEngine;
using System.Collections;

public class AnimatedCamera : MonoBehaviour {

	public GameObject[] m_look_at;
	public float m_camera_decel = 2.0f;

	public AnimationClip m_clip;

	Vector3 m_press_pos;
	float m_t = 0.0f;

	const int SPEED_HISTORY_MAX = 12;
	float m_actual_speed;
	float[] m_t_speed = new float[SPEED_HISTORY_MAX];
	int m_speed_index = 0;

	// Use this for initialization
	void Start () 
	{
	}

	float GetFastestSpeed(float sign)
	{
		float fastest = 0.0f;
		for(int i = 0; i < SPEED_HISTORY_MAX; i++)
		{
			if(sign > 0.0f)
			{
				fastest = fastest < m_t_speed[i] ? m_t_speed[i] : fastest;
			}
			else
			{
				fastest = fastest > m_t_speed[i] ? m_t_speed[i] : fastest;
			}
		}

		return fastest;
	}

	// Update is called once per frame
	void Update () 
	{
		if(m_clip != null && !GetComponent<Animation>().IsPlaying(m_clip.name))
		{
			GetComponent<Animation>().Play(m_clip.name);
		}

		bool press = false;
#if UNITY_EDITOR
		if(Input.GetMouseButton(0))
		{
			press = true;
		}
#endif
		if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
		{
			press = true;
		}

		if(press)
		{
			Vector3 pos = Input.mousePosition; 
			Vector3 diff = pos - m_press_pos;
			diff.x = Mathf.Clamp(diff.x, -15.5f, 15.5f);
			diff.y = Mathf.Clamp(diff.y, -15.5f, 15.5f);
		
			float frame_motion = diff.y * Time.deltaTime * 0.25f;
			m_t += frame_motion;
			m_t = Mathf.Clamp(m_t, 0.0f, 1.0f);

			m_t_speed[m_speed_index] = frame_motion;
			m_speed_index = (m_speed_index+1) % SPEED_HISTORY_MAX;

			m_actual_speed = GetFastestSpeed(Mathf.Sign(frame_motion));
		}
		else if(m_actual_speed != 0.0f)
		{
			for(int i = 0; i < SPEED_HISTORY_MAX; i++)
			{
				m_t_speed[i] = 0.0f;
			}
			m_speed_index = 0;

			float sign = Mathf.Sign(m_actual_speed);

			float decel = -sign * m_camera_decel * Time.deltaTime;
			m_actual_speed += decel;

			if(sign != Mathf.Sign(m_actual_speed))
			{
				m_actual_speed = 0.0f;
			}

			EB.Debug.Log(m_actual_speed);
			m_t += m_actual_speed;
			m_t = Mathf.Clamp(m_t, 0.0f, 1.0f);
		}

		#if UNITY_EDITOR
		m_press_pos = Input.mousePosition;
		#endif
		if(Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved))
		{
			m_press_pos = Input.GetTouch(0).position;
		}

		if(!m_clip)
		{
			Vector3 new_pos = Vector3.Lerp(m_look_at[0].transform.localPosition, m_look_at[1].transform.localPosition, m_t);
			gameObject.transform.position = new_pos;

			Quaternion new_quat = Quaternion.Lerp(m_look_at[0].transform.localRotation, m_look_at[1].transform.localRotation, m_t);
			gameObject.transform.rotation = new_quat;
		}
		else
		{
			GetComponent<Animation>()[m_clip.name].speed = 0.0f;
			GetComponent<Animation>()[m_clip.name].time = m_clip.length * m_t;
		}
	}
}
