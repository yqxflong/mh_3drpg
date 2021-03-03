using UnityEngine;
using System.Collections;
using Thinksquirrel.CShake;

public class MoveCameraShakeHelper
{
    public static CameraShake[] shakes;
    public static void Shake(MoveEditor.CameraShakeEventInfo info, bool isCrit = false)
    {
        shakes = CameraShake.GetComponents();

        if (shakes.Length == 0)
        {
            EB.Debug.LogWarning("DoShake: CameraShake instance is null");
            return;
        }

        var props = info._cameraShakeProperties;
        if (isCrit)
        {
            for (int i = 0, len = shakes.Length; i < len; ++i)
            {
                CameraShake shake = shakes[i];
                shake.Shake(shake.shakeType, props._numberOfShakesCrit, props._shakeAmountCrit, props._rotationAmountCrit,
                                                props._distanceCrit, props._speedCrit, props._decayCrit, shake.uiShakeModifier,
                                                props._multiplyByTimeScale, null);
            }
        }
        else
        {
            for (int i = 0, len = shakes.Length; i < len; ++i)
            {
                CameraShake shake = shakes[i];
                shake.Shake(shake.shakeType, props._numberOfShakes, props._shakeAmount, props._rotationAmount,
                                                props._distance, props._speed, props._decay, shake.uiShakeModifier,
                                                props._multiplyByTimeScale, null);
            }
        }
    }

    public static void UpdateShakes(float time) //by pj 为了震屏效果能再编辑器使用 其原来采用的是协程
    {
        if(shakes==null)
        {
            return;
        }

        for(int i=0;i< shakes.Length;i++)
        {
            if(shakes[i]==null)
            {
                continue;
            }

            for(int j=0;j< shakes[i].listDoShake.Count;j++)
            {
                shakes[i].listDoShake[j].Update(time);
            }
        }
    }
}
