using UnityEngine;

public class MyRenderSetting : MonoBehaviour
{
    public float LightProbeScale = 1.0f;

    public Vector3 GlobalLightRotation = new Vector3(1.0f, 1.0f, 1.0f);

    private void Start()
    {
        Shader.SetGlobalFloat("_EBGCharLightProbeScale", LightProbeScale);
        Vector3 charLightDirection = Quaternion.Euler(GlobalLightRotation) * Vector3.forward;
        Shader.SetGlobalVector("_EBGCharDirectionToLight0", Vector3.zero - charLightDirection.normalized);
    }
}
