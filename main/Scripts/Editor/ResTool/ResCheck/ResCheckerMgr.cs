using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class ResCheckerMgr
{

    /// <summary>
    /// 运行所有的检查
    /// </summary>
    /// <param name="reporter"></param>
    public static void Run(ResCheckReporter reporter)
    {
        List<ResCheckResult> errorResults = RunCheckers();
        errorResults.ForEach(r => reporter.Report(r));
    }
    static List<ResCheckResult> RunCheckers()
    {
        List<ResChecker> checker = new List<ResChecker>();

        checker.Add(new CharatersChecker());
        checker.Add(new PrefabMissChecker());
        checker.Add(new PrefabMissChecker());
        checker.Add(new PrefabMissMatChecker());
        checker.Add(new GUIChecker());

        var checkCallBack = new DefaultResCheckCallBack();
        List<ResCheckResult> errorResults = checker
        .Select(c => c.Check(checkCallBack))
        .ToList()
        .Where(r => r.hasError)
        .ToList();
        return errorResults;
    }

    /// <summary>
    /// 独立检查
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reporter"></param>
    public static void RunChecker<T>(ResCheckReporter reporter) where T : ResChecker, new()
    {
        var checker = new T();
        var r = checker.Check(new DefaultResCheckCallBack());
       EB.Debug.Log("reporter is null = " + (reporter == null).ToString());
       EB.Debug.Log("RunChecker reporter is " + reporter);
        reporter.Report(r);
    }
}
