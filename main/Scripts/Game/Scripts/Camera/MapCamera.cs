using UnityEngine;
using System.Collections;

public class MapCamera : CameraBase 
{
	public Collider m_camera_bounds;

	const float FLICK_TIMER = 1.0f;
	//float m_flick_timer;

	//private Vector3 m_TargetPosition = Vector3.zero;

	protected override void OnComponentStart ()
	{
		base.OnComponentStart ();

		//Waiting for data setup of city view camera.

		if(m_camera_bounds != null)
		{
			Ray ray = GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0.0f));
			RaycastHit ray_hit;
			if(m_camera_bounds.Raycast(ray, out ray_hit, Mathf.Infinity))
			{
				_gameCamera.SetGameCameraPositionAndRotation(ray_hit.point, this);
				_gameCamera.GetPosition(ref _gameCameraPosition);
				_gameCamera.GetRotation(ref _gameCameraRotation);
				transform.position = _gameCameraPosition;
				transform.rotation = _gameCameraRotation;
			}
		}


		//_gameCamera.SetPosition(transform.position);

		//_gameCameraPosition = transform.position;
													//_gameCameraRotation = transform.rotation;

	}

	protected override void OnComponentEnable ()
	{
		base.OnComponentEnable ();
		EventManager.instance.AddListener<TapEvent>(OnTapEvent);
		EventManager.instance.AddListener<TouchStartEvent>(OnTouchStartEvent);
		EventManager.instance.AddListener<TouchUpdateEvent>(OnTouchUpdateEvent);
		EventManager.instance.AddListener<TouchEndEvent>(OnTouchEndEvent);
		EventManager.instance.AddListener<FlickEvent>(OnFlickEvent);

		//Setup camera culling mask according to PerformanceManager's config data.
		if (null != gameObject.GetComponent<Camera>())
		{
			SceneLoadConfig config = SceneLoadManager.GetSceneLoadConfig(SceneLoadManager.CurrentStateName);
			if(config != null)
			{
				uint mask = config.GetHideLayerMask();
				gameObject.GetComponent<Camera>().cullingMask = gameObject.GetComponent<Camera>().cullingMask & (int)mask;
			}
		}
	}

	protected override void OnComponentDisable ()
	{
		base.OnComponentDisable ();

		EventManager.instance.RemoveListener<TouchStartEvent>(OnTouchStartEvent);
		EventManager.instance.RemoveListener<TouchUpdateEvent>(OnTouchUpdateEvent);
		EventManager.instance.RemoveListener<TouchEndEvent>(OnTouchEndEvent);
		EventManager.instance.RemoveListener<FlickEvent>(OnFlickEvent);
	}

	protected override void UpdateGameCamera (float deltaTime)
	{
		base.UpdateGameCamera (deltaTime);
		UpdateMotion (m_camera_velocity);
	}

	Vector3 m_initial_touch;
	Vector3 m_flick_velocity;
	Vector3 m_camera_velocity;

	void OnTapEvent(TapEvent evt)
	{
		if(evt.target != null)
		{
			MeshClick mesh_click = evt.target.gameObject.GetComponent<MeshClick>();
			if(mesh_click != null)
			{
				mesh_click.OnMeshClick(evt.target.gameObject);
			}
		}
	}

	void OnTouchStartEvent(TouchStartEvent evt)
	{
		//EB.Debug.Log ("START TOUCH");
		m_initial_touch = evt.screenPosition;
	}

	void OnTouchUpdateEvent(TouchUpdateEvent evt)
	{
		Vector3 pos_diff = m_initial_touch - evt.screenPosition;
		m_initial_touch = evt.screenPosition;
		m_camera_velocity = pos_diff;
		//m_flick_timer = 0.0f;
	}

	void OnTouchEndEvent(TouchEndEvent evt)
	{

	}

	void OnFlickEvent(FlickEvent evt)
	{
		EB.Debug.Log("FLICK");
		m_camera_velocity = Vector3.zero;
	}
	
	void UpdateMotion(Vector3 motion)
	{
		//project motion against camera facing
		motion.x *= 1.0f;
		motion.z = motion.y;
		motion.y = 0.0f;
		motion *= Time.deltaTime * 0.8f;

		if (motion.sqrMagnitude > 0.0f) {
			if (gameObject != null && gameObject.transform != null) {
					motion = (Vector3.forward * motion.z) + (Vector3.right * motion.x);
					transform.localPosition += motion;
			}
		}


		/*
		if(m_camera_bounds != null)
		{
			Vector3 direction = m_camera_bounds.transform.localPosition - transform.localPosition;
			float distance = direction.magnitude;
			direction.Normalize();
			
			Ray ray = new Ray(transform.localPosition, direction);
			RaycastHit hit = new RaycastHit();
			
			if(m_camera_bounds.Raycast(ray, out hit, distance))
			{
				Vector3 needs_projection = transform.localPosition - hit.point;
				
				Vector3 projected_motion = needs_projection - (Vector3.Dot(needs_projection, hit.normal) * hit.normal);

				Vector3 new_pos = hit.point + projected_motion;
				Vector3 new_vel = new_pos - hit.point;

				transform.localPosition = new_pos;

				m_camera_velocity = new_vel;
			}
		}
		*/
	}
}


