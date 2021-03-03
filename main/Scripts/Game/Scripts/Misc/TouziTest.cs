using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouziTest : MonoBehaviour {

    public GameObject TouziObj;

    public float holdtime = 1;

    public List<Vector3> list;

    public ParticleSystem FX;

    private System.Action m_Callback;

    public void InitDice(int index, System.Action callback)
    {
        TouziObj.SetActive(true);
        FX.Play();
        StopAllCoroutines();
        TouziObj.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        StartCoroutine(Scroll(index));
        m_Callback += callback;
    }

    private IEnumerator Scroll(int index)
    {
        Vector3 target = list[index];
        float secondTime = 0;

        while (secondTime < holdtime)
        {
            secondTime += Time.deltaTime;
            TouziObj.transform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, target, secondTime / holdtime));
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        m_Callback();
        m_Callback = null;
        TouziObj.SetActive(false);
    }
}
