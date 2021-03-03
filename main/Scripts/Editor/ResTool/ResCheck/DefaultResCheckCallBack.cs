
using UnityEditor;

public class DefaultResCheckCallBack : ResCheckerCallBack
{
    public void BeginCheck(string checkName, int totalCheckCount)
    {
        EditorUtility.DisplayProgressBar(string.Format("check res for {0}", checkName), string.Format("check count {0}", totalCheckCount.ToString()), 0);
    }

    public void OnCheckEnd()
    {
        EditorUtility.ClearProgressBar();
    }

    public void OnCheckProgress(string itemName, int currentCheckCount, int totalCheckCount)
    {
        EditorUtility.DisplayProgressBar("scanning", itemName, (float)currentCheckCount / totalCheckCount);
    }
}
