using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PropsController : MonoBehaviour
{
	[System.Serializable]
	public class Prop
	{
		public string 				_name 				= "prop";
		public bool					_enabledByDefault	= false;
		public Transform 			_propRoot 			= null;
		public string				_propRootPath		= "";
		public bool					_attachToBone		= false;
		public MoveEditor.BodyPart 	_attachment			= MoveEditor.BodyPart.Root;
		public AnimationClip		_animClip			= null;

		public void Init(Animator animator)
		{
			_renderers = _propRoot.GetComponentsInChildren<Renderer>();

			if (_animClip != null)
			{
				_animation = _propRoot.gameObject.GetComponent<Animation>();
				if (_animation == null)
				{
					_animation = _propRoot.gameObject.AddComponent<Animation>();
				}

				if (!_animation.GetClip(_animClip.name))
				{
					_animation.AddClip(_animClip, _animClip.name);
				}
			}

			if (_attachToBone)
			{
				Transform attachment = MoveEditor.MoveUtils.GetBodyPartTransform(animator, _attachment);
				_propRoot.position = attachment.position;
				_propRoot.rotation = attachment.rotation;
				AttachTransform.Attach(_propRoot.gameObject, attachment);
			}
		}

		public void Toggle(bool enable)
		{
			for (int i = 0; i < _renderers.Length; i++)
			{
				_renderers[i].enabled = enable;
			}

			if (enable)
			{
				if (_animation != null)
				{
					_animation.Stop();
					_animation.Play(_animClip.name);
				}
			}
		}

		private Renderer[] 	_renderers = null;
		private Animation	_animation	= null;
	}

	public Prop[] _props = new Prop[0];

	private void Awake()
	{
		Init();
	}

	public void Init()
	{
		Animator animator = GetComponent<Animator>();
		for (int i = 0; i < _props.Length; i++)
		{
			_props[i].Init(animator);
		}
		
		ResetPropsToDefault();
	}

	public void ResetPropsToDefault()
	{
		for (int i = 0; i < _props.Length; i++)
			_props[i].Toggle(_props[i]._enabledByDefault);
	}

	public void EnableProp(string name)
	{
		ToggleProp(name, true);
	}

	public void DisableProp(string name)
	{
		ToggleProp(name, false);
	}

	public void ToggleProp(string name, bool enable)
	{
		// TODO: optimize this...
		Prop prop = System.Array.Find<Prop>(_props, delegate(Prop obj) { return obj._name == name; });
		if (prop != null)
		{
			prop.Toggle(enable);
		}
	}
}
