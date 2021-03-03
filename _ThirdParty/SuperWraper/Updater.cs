using UnityEngine;
using System.Collections.Generic;

namespace Unity.Standard.ScriptsWarp
{
    public class Updater : Delegater
    {
        void Update()
        {
            OnDelegater();
        }

        public static Updater Create(GameObject receiver, TheDelegater eventMethod, bool require = true)
        {
            Updater runner = require ? RequireTComponent<Updater>(receiver) : receiver.AddComponent<Updater>();
            runner.SetDelegater(eventMethod);
            return runner;
        }
    }
}