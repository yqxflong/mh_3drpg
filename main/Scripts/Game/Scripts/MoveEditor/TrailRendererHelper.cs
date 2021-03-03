using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TrailRendererHelper : MonoBehaviour
{
    protected UnityEngine.TrailRenderer mTrail;

    void Awake()
    {
        mTrail = gameObject.GetComponent<UnityEngine.TrailRenderer>();
        if (null == mTrail)
        {
            EB.Debug.LogError("[TrailRendererHelper.Awake] invalid TrailRenderer.");
            return;
        }
    }

    void OnEnable()
    {
        if (null == mTrail)
        {
            return;
        }

        Cleanup();
    }

    public void Cleanup()
    {
        StartCoroutine(ResetTrails());
    }

    IEnumerator ResetTrails()
    {
        if (mTrail.time <= 0)
        {
            yield break;
        }

        mTrail.time = -mTrail.time;

        yield return new WaitForEndOfFrame();

        mTrail.time = -mTrail.time;
    }
}
