
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using System;
using System.Collections.Generic;
using System.Reflection;

public class GetHotfixMthod
{
    private static GetHotfixMthod instance;
    public static GetHotfixMthod Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GetHotfixMthod();

            }
            return instance;
        }
    }

    private Type type;
    private object obj;

#if ILRuntime
    private Dictionary<string, Dictionary<string, IMethod>> IMethodDic = new Dictionary<string, Dictionary<string, IMethod>>();
    /// <summary>
    /// 调用热更方法（可缓存方法）
    /// </summary>
    /// <param name="TypeName">类型名</param>
    /// <param name="MethodName">方法名</param>
    /// <param name="parmType">参数类型数组,无则为null</param>
    /// <param name="instance">实例，静态方法为null</param>
    /// <param name="param">参数数组，无参数null</param>
    /// <returns></returns>
    public object InvokeHotfixMethod(string TypeName, string InstanceName, string MethodName, params object[] param)
    {
        object instance = null;
        if(!string.IsNullOrEmpty(InstanceName)){
            instance = GetHotfixTypeAttribute(TypeName, InstanceName, null);
        }
        
        int parmNum = 0;
        IType itype = GetHotfixType(TypeName);
        if (itype == null) return null;

        Dictionary<string, IMethod> methodDic;
        IMethod Method;
        if (!IMethodDic.TryGetValue(TypeName, out methodDic)
            || !methodDic.TryGetValue(MethodName, out Method))
        {
            parmNum = param == null? 0 : param.Length;
            Method = itype.GetMethod(MethodName, parmNum);
            if (Method == null)
            {
                EB.Debug.LogError("Can not find Hotfix IMethod: {0}:{1}", TypeName, MethodName);
                return null;
            }

            if(methodDic == null)
            {
                methodDic = new Dictionary<string, IMethod>();
                IMethodDic[TypeName] = methodDic;
            }
            methodDic[MethodName] = Method;
        }
        obj = HotfixILRManager.GetInstance().appdomain.Invoke(Method, instance, param);
        return obj;
    }
#endif

    /// <summary>
    /// ILR下获得特定类的属性或字段
    /// </summary>
    /// <param name="TypeName">类型名称</param>
    /// <param name="propertyName">属性/字段名称</param>
    /// <param name="instance">实例，静态为null</param>
    /// <param name="isProper">是否属性，字段为false</param>
    /// <param name="isBaseType">是否为基类属性或字段</param>
    /// <returns></returns>
    public object GetHotfixTypeAttribute(string TypeName, string propertyName, object instance,bool isProper = true,bool isBaseType = false)
    {
#if ILRuntime
        IType itype = GetHotfixType(TypeName);
        if (itype == null) return null;
        type = itype.ReflectionType;
        if (isProper)
        {
            
            if (isBaseType)
            {
                obj = type.BaseType.GetProperty(propertyName).GetValue(instance);
            }
            else
            {
                obj = type.GetProperty(propertyName).GetValue(instance);
            }
          

        }
        else
        {
            if (isBaseType)
            {
                obj = type.BaseType.GetField(propertyName).GetValue(instance);
            }
            else
            {
                obj = type.GetField(propertyName).GetValue(instance);
            }
            
        }
#else
        type = GetHotfixType(TypeName);
        if(type == null) return null;
        if (isProper)
        {
            if (isBaseType)
            {
                obj = type.BaseType.GetProperty(propertyName).GetValue(instance, null);
            }
            else
            {
                obj = type.GetProperty(propertyName).GetValue(instance, null);
            }

        }
        else
        {
            if (isBaseType)
            {
                obj = type.BaseType.GetField(propertyName).GetValue(instance);
            }
            else
            {
                obj = type.GetField(propertyName).GetValue(instance);
            }

        }
#endif
        return obj;
    }

#if ILRuntime
    private Dictionary<string, IType> ITypeDic = new Dictionary<string, IType>();
    private IType GetHotfixType(string TypeName)
    {
        if (!HotfixILRManager.GetInstance().IsInit) return null;
        IType itype; 
        if (!ITypeDic.TryGetValue(TypeName, out itype))
        {
            itype = HotfixILRManager.GetInstance().appdomain.GetType(TypeName);
            if(itype == null)
            {
                EB.Debug.LogError("Can not find Hotfix Itype: {0}", TypeName);
                return null;
            }
            ITypeDic[TypeName] = itype;
        }

        return itype;
    }
#else
    Dictionary<string, Type> TypeDic = new Dictionary<string, Type>();
    public Type GetHotfixType(string TypeName)
    {
        if (!HotfixILRManager.GetInstance().IsInit) return null;

        if (TypeDic.ContainsKey(TypeName))
        {
            type = TypeDic[TypeName];
        }
        else
        {
            type = HotfixILRManager.GetInstance().assembly.GetType(TypeName);
            if (type == null)
            {
                EB.Debug.LogError("Can not find Hotfix type:" + TypeName);
                return null;
            }
            TypeDic.Add(TypeName, type);
        }
        return type; 
    }
#endif

}



