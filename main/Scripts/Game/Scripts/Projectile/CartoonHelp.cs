using UnityEngine;
using System.Collections;

public class CartoonHelp
{
    public static void ScaleCartoonOutLine(GameObject go,float outLineValue)
    {
		//0.005
        Renderer[] renders =  go.GetComponentsInChildren<Renderer>(true);
        
        for (int i=0;i<renders.Length;i++)
        {
            if(renders[i].materials!=null)
            {
                for(int j=0;j<renders[i].materials.Length;j++)
                {
                    //float f = renders[i].materials[j].GetFloat("_Outline");
					if(renders[i].materials[j]!=null)
						renders[i].materials[j].SetFloat("_Outline", outLineValue);
                }
            }
        }

    }
    public static void SetCartoonBrightLight(GameObject go, float BrightValue)
    {
#if UNITY_EDITOR
        BrightValue = 0f;
#endif
        //0.005
        Renderer[] renders = go.GetComponentsInChildren<Renderer>(true);

        for (int i = 0; i < renders.Length; i++)
        {
            if (renders[i].materials != null)
            {
                Material[] mats = renders[i].materials;
                for (int j = 0; j < mats.Length; j++)
                {
                    //float f = renders[i].materials[j].GetFloat("_Outline");
                    if (mats[j] != null)
                        mats[j].SetFloat("_Brightness", BrightValue);
                }
            }
        }

    }
}
