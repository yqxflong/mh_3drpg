using UnityEngine;

public class ParticleAnimationPal : MonoBehaviour
{
	private ParticleSystem _fx;

	void OnAwake()
	{
		_fx = GetComponent<ParticleSystem>();
	}

	void OnEnable()
	{
		if (_fx != null)
		{
			EnableParticleSystemAnimations(_fx);
		}
	}

	void OnDisable()
	{
		if (_fx != null)
		{
			DisableParticleSystemAnimations(_fx);
		}
	}

	private void DisableParticleSystemAnimations(ParticleSystem ps)
	{
		foreach (Animator animator in ps.GetComponentsInChildren<Animator>(true))
		{
			if (animator.gameObject.activeInHierarchy)
			{
				animator.gameObject.SetActive(false);
			}
		}

		foreach (Animation animation in ps.GetComponentsInChildren<Animation>(true))
		{
			if (animation.gameObject.activeInHierarchy)
			{
				animation.gameObject.SetActive(false);
			}
		}

		foreach (MeshRenderer mesh_renderer in ps.GetComponentsInChildren<MeshRenderer>())
		{
			if (mesh_renderer.gameObject.activeInHierarchy)
			{
				mesh_renderer.gameObject.SetActive(false);
			}
		}

		foreach (SkinnedMeshRenderer mesh_renderer in ps.GetComponentsInChildren<SkinnedMeshRenderer>(true))
		{
			if (mesh_renderer.gameObject.activeInHierarchy)
			{
				mesh_renderer.gameObject.SetActive(false);
			}
		}
	}

	private void EnableParticleSystemAnimations(ParticleSystem ps)
	{
		foreach (MeshRenderer mesh_renderer in ps.GetComponentsInChildren<MeshRenderer>(true))
		{
			mesh_renderer.gameObject.SetActive(true);
		}

		foreach (SkinnedMeshRenderer mesh_renderer in ps.GetComponentsInChildren<SkinnedMeshRenderer>(true))
		{
			mesh_renderer.gameObject.SetActive(true);
		}

		foreach (Animator animator in ps.GetComponentsInChildren<Animator>(true))
		{
			animator.gameObject.SetActive(true);
			animator.Rebind();
		}

		foreach (Animation animation in ps.GetComponentsInChildren<Animation>(true))
		{
			animation.gameObject.SetActive(true);
			if (animation.clip != null)
			{
				animation.Play();
			}
		}
	}

	public static void Bind(GameObject go)
	{
		if (go.GetComponentInChildren<Animator>() != null || go.GetComponentInChildren<Animation>() != null)
		{
			if (go.GetComponent<ParticleAnimationPal>() == null)
			{
				go.AddComponent<ParticleAnimationPal>();
			}
		}
	}
}
