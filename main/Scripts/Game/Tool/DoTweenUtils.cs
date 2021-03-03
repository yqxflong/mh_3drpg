using UnityEngine;

public class DoTweenUtils
{
    public static float FloatUpdate(float currentValue, float targetValue, float speed){
        float diff = targetValue - currentValue;
        currentValue += (diff * speed) * Time.deltaTime;
        return (currentValue);
    }
    
    public static void DrawLine(Vector3[] line, Color color) {
        if(line.Length>0){
            DrawLineHelper(line,color,"gizmos");
        }
    }
    
    private static void DrawLineHelper(Vector3[] line, Color color, string method){
        Gizmos.color=color;
        for (int i = 0; i < line.Length-1; i++) {
            if(method == "gizmos"){
                Gizmos.DrawLine(line[i], line[i+1]);;
            }else if(method == "handles"){
                Debug.LogError("iTween Error: Drawing a line with Handles is temporarily disabled because of compatability issues with Unity 2.6!");
            }
        }
    }		
}