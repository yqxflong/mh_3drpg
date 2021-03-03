using UnityEngine;
using System.Collections;

public class SelectionDropRotateUpdate : MonoBehaviour
{
    private GameObject m_cameraObj;

    void Update()
    {
        if (null == m_cameraObj)
        {
            if (Camera.main != null)
            {
                m_cameraObj = Camera.main.gameObject;
            }
        }

        if (null != m_cameraObj)
        {
            Vector3 targetPos = m_cameraObj.transform.position;
            transform.LookAt(targetPos, m_cameraObj.transform.up);
            //transform.Rotate(Vector3.up * 180);
        }
    }
}
