using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILRuntime.Runtime.GeneratedAdapter
{
    class CrossBindings
    {
        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            app.RegisterCrossBindingAdaptor(new IEnumerableAdapter());
            app.RegisterCrossBindingAdaptor(new DataLookILRObjectAdapter());
            app.RegisterCrossBindingAdaptor(new GameEventAdapter());
            app.RegisterCrossBindingAdaptor(new LogicILRObjectAdapter());
            app.RegisterCrossBindingAdaptor(new TableAdapter());
            app.RegisterCrossBindingAdaptor(new UIControllerILRObjectAdapter());
            app.RegisterCrossBindingAdaptor(new IComparer_1_StringAdapter());
            app.RegisterCrossBindingAdaptor(new IComparer_1_ILTypeInstanceAdapter());
            app.RegisterCrossBindingAdaptor(new IComparer_1_DynamicMonoILRObjectAdaptor_Binding_AdaptorAdapter());
            app.RegisterCrossBindingAdaptor(new IComparer_1_KeyValuePair_2_ILRuntime_Runtime_GeneratedAdapter_IComparable_1_ILTypeInstanceAdapter_Binding_Adapter_ILTypeInstanceAdapter());
            app.RegisterCrossBindingAdaptor(new IComparableAdapter());
            app.RegisterCrossBindingAdaptor(new IComparable_1_ILTypeInstanceAdapter());
            app.RegisterCrossBindingAdaptor(new IEqualityComparer_1_ILRuntime_Runtime_GeneratedAdapter_IComparableAdapter_Binding_AdapterAdapter());
            app.RegisterCrossBindingAdaptor(new IEqualityComparerAdapter());
            app.RegisterCrossBindingAdaptor(new IEqualityComparer_1_ILTypeInstanceAdapter());
            app.RegisterCrossBindingAdaptor(new IEqualityComparer_1_Int32Adapter());
            app.RegisterCrossBindingAdaptor(new IEqualityComparer_1_ILRuntime_Runtime_GeneratedAdapter_IComparable_1_ILTypeInstanceAdapter_Binding_AdapterAdapter());
        }
    }
}
