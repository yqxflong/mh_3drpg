using System.Collections;
using EB.Sparx;

public class LTHotfixManagerLogic : LogicILR
{
    protected override IEnumerator InitalizeWait(Config config)
    {
        yield return base.InitalizeWait(config);
        string Core = "Hotfix_LT.UI.LTHotfixManager";
#if ILRuntime
        logicObject = HotfixILRManager.GetInstance().appdomain.Instantiate<LogicILRObject>(Core);
#else
        var type = HotfixILRManager.GetInstance().assembly.GetType(Core);
        logicObject = System.Activator.CreateInstance(type) as LogicILRObject;
#endif

        logicObject.Initialize(config);
    }
}
