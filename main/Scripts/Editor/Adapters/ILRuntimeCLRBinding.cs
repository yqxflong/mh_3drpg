#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using ILRuntime.Runtime;
using Sirenix.Utilities;

[System.Reflection.Obfuscation(Exclude = true)]
public class ILRuntimeCLRBinding
{
   // [MenuItem("ILRuntime/Generate CLR Binding Code")]
    public static void GenerateCLRBinding()
    {
        List<Type> types = new List<Type>();
        types.Add(typeof(int));
        types.Add(typeof(float));
        types.Add(typeof(long));
        types.Add(typeof(object));
        types.Add(typeof(string));
        types.Add(typeof(Array));
        types.Add(typeof(Vector2));
        types.Add(typeof(Vector3));
        types.Add(typeof(Quaternion));
        types.Add(typeof(GameObject));
        types.Add(typeof(UnityEngine.Object));
        types.Add(typeof(Transform));
        types.Add(typeof(RectTransform));
        types.Add(typeof(Component));
        types.Add(typeof(UIController));
        types.Add(typeof(UIControllerILR));
        types.Add(typeof(UIControllerILRObject));
        types.Add(typeof(LogicILRObject));
        types.Add(typeof(HotfixTool));
        types.Add(typeof(LTHotfixApi));
        types.Add(typeof(EB.Coroutines));
        types.Add(typeof(EB.Dot));
        types.Add(typeof(UIStack));
        //types.Add(typeof(SceneTemplateManager));  TODOX
        //types.Add(typeof(EventTemplateManager));

        //types.Add(typeof(CLRBindingTestClass));  不属于TODOX
        types.Add(typeof(Time));
        types.Add(typeof(Debug));
        //所有DLL内的类型的真实C#类型都是ILTypeInstance
        types.Add(typeof(List<ILRuntime.Runtime.Intepreter.ILTypeInstance>));

        
        ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(types, "Assets/_GameAssets/Scripts/Game/ILR/Generated");

    }

    [MenuItem("ILRuntime/Generate CLR Binding Code by Analysis")]
    static void GenerateCLRBindingByAnalysis()
    {
        //用新的分析热更dll调用引用来生成绑定代码
        ILRuntime.Runtime.Enviorment.AppDomain domain = new ILRuntime.Runtime.Enviorment.AppDomain();
        using (System.IO.FileStream fs = new System.IO.FileStream("Library/ScriptAssemblies/Unity_Hotfix.dll", System.IO.FileMode.Open, System.IO.FileAccess.Read))
        {
            domain.LoadAssembly(fs);
            // InitILRuntime(domain);
            HotfixILRManager.InitializeILRuntime(domain);
            
            // List<Type> types = new List<Type>();
            // types.Add(typeof(Quaternion));
            // types.Add(typeof(Vector2));
            // types.Add(typeof(Vector3));
            
            ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(domain, "Assets/_GameAssets/Scripts/Game/ILR/Generated");
        }
        //Crossbind Adapter is needed to generate the correct binding code
    }
    
    [MenuItem("ILRuntime/Generate Cross Binding Code")]
    static void GenerateCrossBindingCode()
    {
        // fout = new FileStream(zipFilePath, FileMode.CreateNew, FileAccess.Write);

        List<Type> types = new List<Type>();
        // types.Add(typeof(EB.Sparx.SparxAPI));
        types.Add(typeof(System.Collections.IEnumerable));
        types.Add(typeof(DataLookILRObject));
        // types.Add(typeof(DynamicMonoILRObject));
        types.Add(typeof(GameEvent));
        types.Add(typeof(LogicILRObject));
        types.Add(typeof(FlatBuffers.Table));
        types.Add(typeof(UIControllerILRObject));
        types.Add(typeof(IComparer<String>));
        types.Add(typeof(IComparer<ILRuntime.Runtime.Intepreter.ILTypeInstance>));
        types.Add(typeof(IComparer<DynamicMonoILRObjectAdaptor.Adaptor>));
        types.Add(typeof(IComparer<System.Collections.Generic.KeyValuePair<ILRuntime.Runtime.GeneratedAdapter.IComparable_1_ILTypeInstanceAdapter.Adapter, ILRuntime.Runtime.Intepreter.ILTypeInstance>>));
        types.Add(typeof(IComparable));
        types.Add(typeof(IComparable<ILRuntime.Runtime.Intepreter.ILTypeInstance>));
        types.Add(typeof(IEqualityComparer<ILRuntime.Runtime.GeneratedAdapter.IComparableAdapter.Adapter>));
        types.Add(typeof(IEqualityComparer));
        types.Add(typeof(IEqualityComparer<ILRuntime.Runtime.Intepreter.ILTypeInstance>));
        types.Add(typeof(IEqualityComparer<System.Int32>));
        types.Add(typeof(IEqualityComparer<ILRuntime.Runtime.GeneratedAdapter.IComparable_1_ILTypeInstanceAdapter.Adapter>));

        // List<Type> types1 = new List<Type>();
        // types1.Add(typeof(IEnumerable<ILRuntime.Runtime.Intepreter.ILTypeInstance>));

        string path = "Assets/_GameAssets/Scripts/Game/ILR/GeneratedAdapter";
        if (!System.IO.Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);

        foreach (Type type in types)
        {
            string msg = ILRuntimeCLRBinding.GenerateCrossBindingAdapterCode(type, "ILRuntime.Runtime.GeneratedAdapter");
            
            string clsName, realClsName;
            bool isByRef;
            type.GetClassName(out clsName, out realClsName, out isByRef, true);
            
            string fileName = path + "/" + clsName + "Adapter.cs";
            if(System.IO.File.Exists(fileName)){
                System.IO.File.Delete(fileName);
            }
            FileStream fs = File.OpenWrite(fileName);
            byte[] bytes = System.Text.Encoding.Default.GetBytes(Regex.Replace(msg, "(?<!\r)\n", "\r\n"));
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
        }


        GenerateBindingInitializeScript(types.Select(t=>
        {
            string clsName, realClsName;
            bool isByRef;
            t.GetClassName(out clsName, out realClsName, out isByRef, true);
            return clsName;
        }).ToList(), path);
    }
    
    public static string GenerateCrossBindingAdapterCode(Type baseType, string nameSpace)
        {
            StringBuilder sb = new StringBuilder();
            
            ////////////////////////////////////////////////////
            /// 
            MethodInfo[] methods = baseType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            // var tmpList = new List<MethodInfo>(methods);
            // var baseTypes = baseType.GetBaseTypes();
            //
            // foreach (Type type in baseTypes)
            // {
            //     tmpList.AddRange(type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
            // }
            
            
            /////////////////////////////////////////////////////
            List<MethodInfo> virtMethods = new List<MethodInfo>();
            // foreach (var i in methods)
            foreach (var i in methods)
            {
                if ((i.IsVirtual && !i.IsFinal) || i.IsAbstract || baseType.IsInterface)
                    virtMethods.Add(i);
            }
            string clsName, realClsName;
            bool isByRef;
            baseType.GetClassName(out clsName, out realClsName, out isByRef, true);
            sb.Append(@"using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ");
            sb.AppendLine(nameSpace);
            sb.Append(@"{   
    public class ");
            sb.Append(clsName);
            sb.AppendLine(@"Adapter : CrossBindingAdaptor
    {");
           

            sb.Append(@"        public override Type BaseCLRType
        {
            get
            {
                return typeof(");
            sb.Append(realClsName);
            sb.AppendLine(@");
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }
");

            sb.AppendLine(string.Format("        public class Adapter : {0}, CrossBindingAdaptorType", realClsName));
            sb.AppendLine(@"        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;
");
    
            GenerateCrossBindingMethodInfo(sb, virtMethods);
    
    sb.AppendLine(@"
            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }
");
            GenerateCrossBindingMethodBody(sb, virtMethods);
            sb.Append(@"            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("); sb.AppendLine("\"ToString\", 0);");
            sb.AppendLine(@"                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return instance.ToString();
                }
                else
                    return instance.Type.FullName;");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }
    
    public static void GenerateCrossBindingMethodInfo(StringBuilder sb, List<MethodInfo> virtMethods)
    {
        int index = 0;
        foreach (var i in virtMethods)
        {
            if (ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.ShouldSkip(i))
                continue;
            var param = i.GetParameters();
            if (ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.NeedGenerateCrossBindingMethodClass(param))
            {
                ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GenerateCrossBindingMethodClass(sb, i.Name, index, param, i.ReturnType);
                sb.AppendLine(string.Format("            {0}_{1}Info m{0}_{1} = new {0}_{1}Info();", i.Name, index));
            }
            else
            {
                // if (i.ReturnType != typeof(void))
                // {
                //     sb.AppendLine(string.Format("            CrossBindingFunctionInfo<{0}> m{1}_{2} = new CrossBindingFunctionInfo<{0}>(\"{1}\");", ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GetParametersString(param, i.ReturnType), i.Name, index));
                // }
                // else
                // {
                //     if (param.Length > 0)
                //         sb.AppendLine(string.Format("            CrossBindingMethodInfo<{0}> m{1}_{2} = new CrossBindingMethodInfo<{0}>(\"{1}\");", ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GetParametersString(param, i.ReturnType), i.Name, index));
                //     else
                //         sb.AppendLine(string.Format("            CrossBindingMethodInfo m{0}_{1} = new CrossBindingMethodInfo(\"{0}\");", i.Name, index));
                // }
                sb.AppendLine(string.Format("            IMethod m{0}_{1};", i.Name, index));
                sb.AppendLine(string.Format("            bool m{0}_{1}_Got;", i.Name, index));
                sb.AppendLine(string.Format("            bool m{0}_{1}_Invoking;", i.Name, index));
            }
            index++;
        }
    }
    
    public static void GenerateCrossBindingMethodBody(StringBuilder sb, List<MethodInfo> virtMethods)
        {
            int index = 0;
            Dictionary<string, ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.PropertyGenerateInfo> pendingProperties = new Dictionary<string, ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.PropertyGenerateInfo>();
            foreach (var i in virtMethods)
            {
                if (ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.ShouldSkip(i))
                    continue;
                bool isProperty = i.IsSpecialName && (i.Name.StartsWith("get_") || i.Name.StartsWith("set_"));
                ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.PropertyGenerateInfo pInfo = null;
                bool isGetter = false;
                StringBuilder oriBuilder = null;
                if (isProperty)
                {
                    string pName = i.Name.Substring(4);
                    isGetter = i.Name.StartsWith("get_");
                    oriBuilder = sb;
                    sb = new StringBuilder();
                    if (!pendingProperties.TryGetValue(pName, out pInfo))
                    {
                        pInfo = new ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.PropertyGenerateInfo();
                        pInfo.Name = pName;
                        pendingProperties[pName] = pInfo;
                    }
                    if (pInfo.ReturnType == null)
                    {
                        if (isGetter)
                        {
                            pInfo.ReturnType = i.ReturnType;
                        }
                        else
                        {
                            pInfo.ReturnType = i.GetParameters()[0].ParameterType;
                        }
                    }
                }
                var param = i.GetParameters();
                string modifier = i.IsFamily ? "protected" : "public";
                string overrideStr = i.DeclaringType.IsInterface ? "" : "override ";
                string clsName, realClsName;
                string returnString = "";
                bool isByRef;
                if (i.ReturnType != typeof(void))
                {
                    i.ReturnType.GetClassName(out clsName, out realClsName, out isByRef, true);
                    returnString = "return ";
                }
                else
                    realClsName = "void";
                if (!isProperty)
                {
                    sb.Append(string.Format("            {0} {3}{1} {2}(", modifier, realClsName, i.Name, overrideStr));
                    ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GetParameterDefinition(sb, param, true);
                    sb.AppendLine(@")
            {");
                }
                else
                {
                    pInfo.Modifier = modifier;
                    pInfo.OverrideString = overrideStr;
                }
                sb.AppendLine(string.Format("                if (!m{0}_{1}_Got)", i.Name, index));
                sb.AppendLine("                {");
                sb.AppendLine(string.Format("                    m{0}_{1} = instance.Type.GetMethod(\"{2}\", {3});", i.Name, index, i.Name, param.Length));
                sb.AppendLine(string.Format("                    m{0}_{1}_Got = true;", i.Name, index));
                sb.AppendLine("                }");
                
                
                if (!i.IsAbstract)
                {
                    sb.AppendLine(string.Format("                if (m{0}_{1} != null && !m{0}_{1}_Invoking)", i.Name, index, i.Name, index));
                    sb.AppendLine("                {");
                    sb.AppendLine(string.Format("                    m{0}_{1}_Invoking = true;", i.Name, index));
                    
                    // sb.AppendLine("                else");
                    sb.AppendLine(string.Format("                    {3}appdomain.Invoke(m{0}_{1}, this.instance{2});", i.Name, index, ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GetParameterName(param, false), string.IsNullOrEmpty(returnString) ? "": "var returnValue = "));
                    sb.AppendLine(string.Format("                    m{0}_{1}_Invoking = false;", i.Name, index));
                    if (!string.IsNullOrEmpty(returnString))
                    {
                        sb.AppendLine(string.Format("                    {0}({1})returnValue;", returnString, realClsName));
                    }
                    sb.AppendLine("                }");
                    sb.AppendLine("                else");
                    sb.AppendLine("                {");
                    if (isProperty)
                    {
                        if (isGetter)
                        {
                            sb.AppendLine(string.Format("                    return base.{0};", i.Name.Substring(4)));
                        }
                        else
                        {
                            sb.AppendLine(string.Format("                    base.{0} = value;", i.Name.Substring(4)));
                        }
                    }
                    else
                        sb.AppendLine(string.Format("                    {2}base.{0}({1});", i.Name, ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GetParameterName(param, true), returnString));
                    sb.AppendLine("                }");
                }
                else
                {
                    sb.AppendLine(string.Format("                {3}({4})appdomain.Invoke(m{0}_{1}, this.instance{2});", i.Name, index, ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GetParameterName(param, false), returnString, realClsName));
                }
               
                if (isProperty)
                {
                    if (isGetter)
                    {
                        pInfo.GetterBody = sb.ToString();
                    }
                    else
                    {
                        pInfo.SettingBody = sb.ToString();
                    }
                    sb = oriBuilder;
                }
                else
                {
                    sb.AppendLine("            }");
                    sb.AppendLine();
                }
                index++;
            }

            foreach (var i in pendingProperties)
            {
                var pInfo = i.Value;
                string clsName, realClsName;
                bool isByRef;
                pInfo.ReturnType.GetClassName(out clsName, out realClsName, out isByRef, true);
                sb.AppendLine(string.Format("            {0} {3}{1} {2}", pInfo.Modifier, realClsName, pInfo.Name, pInfo.OverrideString));
                sb.AppendLine("            {");
                if (!string.IsNullOrEmpty(pInfo.GetterBody))
                {
                    sb.AppendLine("            get");
                    sb.AppendLine("            {");
                    sb.AppendLine(pInfo.GetterBody);
                    sb.AppendLine("            }");

                }
                if (!string.IsNullOrEmpty(pInfo.SettingBody))
                {
                    sb.AppendLine("            set");
                    sb.AppendLine("            {");
                    sb.AppendLine(pInfo.SettingBody);
                    sb.AppendLine("            }");

                }
                sb.AppendLine("            }");
                sb.AppendLine();
            }
        }
    
    internal static void GenerateBindingInitializeScript(List<string> clsNames, string outputPath)
        {
            if (!System.IO.Directory.Exists(outputPath))
                System.IO.Directory.CreateDirectory(outputPath);
            
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outputPath + "/CrossBindings.cs", false, new UTF8Encoding(false)))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"using System;
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
        {");
                if (clsNames != null)
                {
                    foreach (var i in clsNames)
                    {
                        sb.Append("            app.RegisterCrossBindingAdaptor(new ");
                        sb.Append(i + "Adapter");
                        sb.AppendLine("());");
                    }
                }

                sb.AppendLine("        }");
                sb.AppendLine(@"    }
}");
                sw.Write(Regex.Replace(sb.ToString(), "(?<!\r)\n", "\r\n"));
            }
        }

    static void InitILRuntime(ILRuntime.Runtime.Enviorment.AppDomain domain)
    {
        //这里需要注册所有热更DLL中用到的跨域继承Adapter，否则无法正确抓取引用
        //domain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
        //domain.RegisterCrossBindingAdaptor(new DynamicMonoILRObjectAdaptor());
        //domain.RegisterCrossBindingAdaptor(new LogicILRObjectAdaptor());
        //domain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
        //domain.RegisterCrossBindingAdaptor(new NodeDataILRObjectAdaptor());
        //domain.RegisterCrossBindingAdaptor(new UIControllerILRObjectAdapter());
        //domain.RegisterCrossBindingAdaptor(new DataLookupILRObjectAdapter());
        //domain.RegisterCrossBindingAdaptor(new SparxAPIAdapter());
        
        
        HotfixILRManager.RegisterCrossBindingAdaptor(domain);
        // domain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
        // domain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
        // domain.RegisterCrossBindingAdaptor(new UIControllerILRObjectAdapter());
        // domain.RegisterCrossBindingAdaptor(new LogicILRObjectAdaptor());
        // domain.RegisterCrossBindingAdaptor(new DynamicMonoILRObjectAdaptor());
        // domain.RegisterCrossBindingAdaptor(new DataLookupILRObjectAdapter());
        // domain.RegisterCrossBindingAdaptor(new SparxAPIAdapter());
        // domain.RegisterCrossBindingAdaptor(new GameEventAdapter());
        // domain.RegisterCrossBindingAdaptor(new IComparableAdapter());
        // domain.RegisterCrossBindingAdaptor(new IComparerAdapter());
        // domain.RegisterCrossBindingAdaptor(new IEqualityComparerAdapter());
        // domain.RegisterCrossBindingAdaptor(new TableAdapter());
        // domain.RegisterCrossBindingAdaptor(new EventNameAdapter());
    }


    private static void CommonCheckSymbolMenu(string title, string symbol)
    {
        Menu.SetChecked(title, ContainSymbolStr(symbol));
    }
    

    private static void CommonSetSymbolMenu(string title,string symbol)
    {
        if (!ContainSymbolStr(symbol))
        {
            _AddDefineSymbols(symbol);
            Menu.SetChecked(title, true);
        } else{
            _RemoveDefineSymbols(symbol);
            Menu.SetChecked(title, false);
        }
    }


    public static bool ContainSymbolStr(string key)
    {
        string symbolStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        var symbols = symbolStr.Split(';');
        foreach (var item in symbols)
        {
            if (item == key)
            {
                return true;
            }
        }
        return false;
    }

    private static void _AddDefineSymbols(string theSysmbol){
        var curPlatform = EditorUserBuildSettings.selectedBuildTargetGroup;
        string defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(curPlatform);
        if(!defineSymbols.Contains(theSysmbol)){
            if(defineSymbols.LastIndexOf(';') == defineSymbols.Length - 1){
                string finalSymbol = $"{defineSymbols}{theSysmbol}";
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, finalSymbol);   
            }
            else{
                string finalSymbol = $"{defineSymbols};{theSysmbol}";
                PlayerSettings.SetScriptingDefineSymbolsForGroup(curPlatform, finalSymbol);   
            }
        }
    }

    private static void _RemoveDefineSymbols(string theSysmbol){
        var curPlatform = EditorUserBuildSettings.selectedBuildTargetGroup;
        string defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(curPlatform);
        if(defineSymbols.Contains(theSysmbol)){
            string finalSymbol = string.Empty;
            List<string> ll = defineSymbols.Split(';').ToList();
            foreach(string ss in ll ){
                if(!ss.Trim().Equals(theSysmbol)
                    && !ss.Trim().Equals(string.Empty)){
                    finalSymbol += $"{ss};";
                }
            }
            if(finalSymbol.Length > 0){
                finalSymbol.Remove(finalSymbol.Length - 1, 1);
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(curPlatform, finalSymbol);   
        }
    }


    #region UseAssetBundleInEditor
    private const string TitleUseAssetBundleInEditor = "ILRuntime/UseAssetBundleInEditor";
    private const string SymbolUseAssetBundleInEditor = "USE_ASSETBUNDLE_IN_EDITOR";
    [MenuItem(TitleUseAssetBundleInEditor,false)]
    public static void SetUseAssetBundleInEditor(){
        CommonSetSymbolMenu(TitleUseAssetBundleInEditor,SymbolUseAssetBundleInEditor);
    }

    [MenuItem(TitleUseAssetBundleInEditor, true)]
    public static bool CheckUseAssetBundleInEditor()
    {
        CommonCheckSymbolMenu(TitleUseAssetBundleInEditor,SymbolUseAssetBundleInEditor);
        return true;
    }
    #endregion
    
    #region UseDebug
    private const string TitleUseDebug = "ILRuntime/UseDebug";
    private const string SymbolUseDebug = "USE_DEBUG";
    [MenuItem(TitleUseDebug,false)]
    public static void SetUseDebug()
    {
        CommonSetSymbolMenu(TitleUseDebug,SymbolUseDebug);
    }
    
    [MenuItem(TitleUseDebug, true)]
    public static bool CheckUseDebug()
    {
        CommonCheckSymbolMenu(TitleUseDebug,SymbolUseDebug);
        return true;
    }
    #endregion
   
    #region DisableILRuntimeDebug
    private const string TitleDisableILRuntimeDebug = "ILRuntime/DisableILRuntimeDebug";
    private const string SymbolDisableILRuntimeDebug = "DISABLE_ILRUNTIME_DEBUG";
    [MenuItem(TitleDisableILRuntimeDebug,false)]
    public static void SetDisableILRuntimeDebug()
    {
        CommonSetSymbolMenu(TitleDisableILRuntimeDebug,SymbolDisableILRuntimeDebug);
    }
    
    [MenuItem(TitleDisableILRuntimeDebug, true)]
    public static bool CheckDisableILRuntimeDebug()
    {
        CommonCheckSymbolMenu(TitleDisableILRuntimeDebug,SymbolDisableILRuntimeDebug);
        return true;
    }
    #endregion

    #region CheckBundleDataRepeatName
    [MenuItem("ILRuntime/CheckBundleDataRepeatName")]
    public static void CheckBundleDataRepeatName()
    {
        Dictionary<string, string> dic_name_path = new Dictionary<string, string>();
        string bdpath = "Assets/_ThirdParty/BundleManager/BundleData.txt";
        var ta = AssetDatabase.LoadAssetAtPath<TextAsset>(bdpath);
        var jnode = Johny.JSONNode.Parse(ta.text);
        if(jnode.IsArray)
        {
            var jarr = jnode.AsArray;
            for(int i = 0; i < jarr.Count; i++)
            {
                var jobj = jarr[i].AsObject;
                var jarr_inc = jobj["includs"].AsArray;
                var jarr_dep = jobj["dependAssets"].AsArray;
                if(jarr_inc.Count > 0)
                {
                    for(int j = 0; j < jarr_inc.Count; j++)
                    {
                        string path = jarr_inc[j].ToString();
                        string name =  System.IO.Path.GetFileNameWithoutExtension(path);
                        string ext = System.IO.Path.GetExtension(path);
                        if((path.Contains("materials") && ext.Equals(".mat")) || (path.Contains("particles") && ext.Equals(".prefab")))
                        {
                            if(dic_name_path.TryGetValue(name, out string outPath)
                            && !path.Equals(outPath)
                            && !(path.Contains("particles") && outPath.Contains("particles")))
                            {
                                UnityEngine.Debug.LogErrorFormat("检测BundleData===>AssetName重复，Path1: {0}, Path2: {1}", outPath, path);
                            }
                            else
                            {
                                dic_name_path[name] = path;
                            }
                        }
                    }
                    
                    for(int j = 0; j < jarr_dep.Count; j++)
                    {
                        string path = jarr_dep[j].ToString();
                        string name =  System.IO.Path.GetFileNameWithoutExtension(path);
                        string ext = System.IO.Path.GetExtension(path);
                        if((path.Contains("materials") && ext.Equals(".mat")) || (path.Contains("particles") && ext.Equals(".prefab")))
                        {
                            if(dic_name_path.TryGetValue(name, out string outPath)
                            && !path.Equals(outPath)
                            && !(path.Contains("particles") && outPath.Contains("particles")))
                            {
                                UnityEngine.Debug.LogErrorFormat("检测BundleData===>AssetName重复，Path1: {0}, Path2: {1}", outPath, path);
                            }
                            else
                            {
                                dic_name_path[name] = path;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            throw new Exception("jnode is not Array!");
        }
    }
    #endregion
}
#endif
