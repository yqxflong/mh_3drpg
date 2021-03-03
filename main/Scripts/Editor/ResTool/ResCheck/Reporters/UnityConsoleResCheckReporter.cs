using System.IO;
using UnityEngine;

public class UnityConsoleResCheckReporter : ResCheckReporter
{
    public void Report(ResCheckResult result)
    {
        if (result.hasError)
        {
            Debug.LogError(string.Format("ErrorOut: {0} error nums: {1}", result.checkName, result.errorMessages.Count.ToString()));
            result.errorMessages.ForEach(s => Debug.LogError(string.Format("ErrorOut:{0}",s)));
        }
        else
        {
           EB.Debug.Log(string.Format("{0} passed", result.checkName));
        }
    }
}

public class ExportTxtResCheckReporter : ResCheckReporter
{
    public void Report(ResCheckResult result)
    {
        if (result.hasError)
        {
            Debug.LogError(string.Format("ErrorOut: {0} error nums: {1}", result.checkName, result.errorMessages.Count.ToString()));
            string contents = string.Empty;
            result.errorMessages.ForEach(s => contents += (s + "\r\n"));
            File.WriteAllText(Application.dataPath + "/output.txt", contents, System.Text.Encoding.Unicode);
        }
        else
        {
           EB.Debug.Log(string.Format("{0} passed", result.checkName));
        }
    }
}

public class GUICheckerErrorOutResCheckReporter : ResCheckReporter
{
    public void Report(ResCheckResult result)
    {
        if (result.hasError)
        {
            Debug.LogError(string.Format("GUICheckerErrorOut: {0} error nums: {1}", result.checkName, result.errorMessages.Count.ToString()));
            result.errorMessages.ForEach(s => Debug.LogError(string.Format("GUICheckerErrorOut:{0}", s)));
        }
        else
        {
           EB.Debug.Log(string.Format("{0} passed", result.checkName));
        }
    }
}

public class PrefabMissMatCheckerErrorOutResCheckReporter : ResCheckReporter
{
    public void Report(ResCheckResult result)
    {
        if (result.hasError)
        {
            Debug.LogError(string.Format("PrefabMissMatCheckerErrorOut: {0} error nums: {1}", result.checkName, result.errorMessages.Count.ToString()));
            result.errorMessages.ForEach(s => Debug.LogError(string.Format("PrefabMissMatCheckerErrorOut:{0}", s)));
        }
        else
        {
           EB.Debug.Log(string.Format("{0} passed", result.checkName));
        }
    }
}

public class PrefabsMissCheckerErrorOutResCheckReporter : ResCheckReporter
{
    public void Report(ResCheckResult result)
    {
        if (result.hasError)
        {
            Debug.LogError(string.Format("PrefabsMissCheckerErrorOut: {0} error nums: {1}", result.checkName, result.errorMessages.Count.ToString()));
            result.errorMessages.ForEach(s => Debug.LogError(string.Format("PrefabsMissCheckerErrorOut:{0}", s)));
        }
        else
        {
           EB.Debug.Log(string.Format("{0} passed", result.checkName));
        }
    }
}
