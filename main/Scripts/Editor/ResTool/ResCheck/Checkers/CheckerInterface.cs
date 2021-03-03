

public interface ResChecker
{
    ResCheckResult Check(ResCheckerCallBack callbacker);
    string Name();
}

public interface ResCheckerCallBack
{
    void BeginCheck(string checkName, int totalCheckCount);
    void OnCheckProgress(string itemName, int currentCheckCount, int totalCheckCount);
    void OnCheckEnd();
}

public interface ResCheckReporter
{
    void Report(ResCheckResult result);
}
