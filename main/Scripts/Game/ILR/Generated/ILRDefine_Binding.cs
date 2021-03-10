using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class ILRDefine_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::ILRDefine);

            field = type.GetField("USE_ASTTEST", flag);
            app.RegisterCLRFieldGetter(field, get_USE_ASTTEST_0);
            app.RegisterCLRFieldSetter(field, set_USE_ASTTEST_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_USE_ASTTEST_0, AssignFromStack_USE_ASTTEST_0);
            field = type.GetField("DEBUG", flag);
            app.RegisterCLRFieldGetter(field, get_DEBUG_1);
            app.RegisterCLRFieldSetter(field, set_DEBUG_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_DEBUG_1, AssignFromStack_DEBUG_1);
            field = type.GetField("IS_FX", flag);
            app.RegisterCLRFieldGetter(field, get_IS_FX_2);
            app.RegisterCLRFieldSetter(field, set_IS_FX_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_IS_FX_2, AssignFromStack_IS_FX_2);
            field = type.GetField("UNITY_IPHONE", flag);
            app.RegisterCLRFieldGetter(field, get_UNITY_IPHONE_3);
            app.RegisterCLRFieldSetter(field, set_UNITY_IPHONE_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_UNITY_IPHONE_3, AssignFromStack_UNITY_IPHONE_3);
            field = type.GetField("UNITY_EDITOR", flag);
            app.RegisterCLRFieldGetter(field, get_UNITY_EDITOR_4);
            app.RegisterCLRFieldSetter(field, set_UNITY_EDITOR_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_UNITY_EDITOR_4, AssignFromStack_UNITY_EDITOR_4);
            field = type.GetField("USE_UMENG", flag);
            app.RegisterCLRFieldGetter(field, get_USE_UMENG_5);
            app.RegisterCLRFieldSetter(field, set_USE_UMENG_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_USE_UMENG_5, AssignFromStack_USE_UMENG_5);
            field = type.GetField("UNITY_ANDROID", flag);
            app.RegisterCLRFieldGetter(field, get_UNITY_ANDROID_6);
            app.RegisterCLRFieldSetter(field, set_UNITY_ANDROID_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_UNITY_ANDROID_6, AssignFromStack_UNITY_ANDROID_6);
            field = type.GetField("USE_XINKUAISDK", flag);
            app.RegisterCLRFieldGetter(field, get_USE_XINKUAISDK_7);
            app.RegisterCLRFieldSetter(field, set_USE_XINKUAISDK_7);
            app.RegisterCLRFieldBinding(field, CopyToStack_USE_XINKUAISDK_7, AssignFromStack_USE_XINKUAISDK_7);
            field = type.GetField("USE_WECHATSDK", flag);
            app.RegisterCLRFieldGetter(field, get_USE_WECHATSDK_8);
            app.RegisterCLRFieldSetter(field, set_USE_WECHATSDK_8);
            app.RegisterCLRFieldBinding(field, CopyToStack_USE_WECHATSDK_8, AssignFromStack_USE_WECHATSDK_8);
            field = type.GetField("USE_APPSFLYER", flag);
            app.RegisterCLRFieldGetter(field, get_USE_APPSFLYER_9);
            app.RegisterCLRFieldSetter(field, set_USE_APPSFLYER_9);
            app.RegisterCLRFieldBinding(field, CopyToStack_USE_APPSFLYER_9, AssignFromStack_USE_APPSFLYER_9);
            field = type.GetField("USE_AOSHITANGSDK", flag);
            app.RegisterCLRFieldGetter(field, get_USE_AOSHITANGSDK_10);
            app.RegisterCLRFieldSetter(field, set_USE_AOSHITANGSDK_10);
            app.RegisterCLRFieldBinding(field, CopyToStack_USE_AOSHITANGSDK_10, AssignFromStack_USE_AOSHITANGSDK_10);
            field = type.GetField("USE_ALIPAYSDK", flag);
            app.RegisterCLRFieldGetter(field, get_USE_ALIPAYSDK_11);
            app.RegisterCLRFieldSetter(field, set_USE_ALIPAYSDK_11);
            app.RegisterCLRFieldBinding(field, CopyToStack_USE_ALIPAYSDK_11, AssignFromStack_USE_ALIPAYSDK_11);
            field = type.GetField("USE_AIHELP", flag);
            app.RegisterCLRFieldGetter(field, get_USE_AIHELP_12);
            app.RegisterCLRFieldSetter(field, set_USE_AIHELP_12);
            app.RegisterCLRFieldBinding(field, CopyToStack_USE_AIHELP_12, AssignFromStack_USE_AIHELP_12);
            field = type.GetField("USE_GM", flag);
            app.RegisterCLRFieldGetter(field, get_USE_GM_13);
            app.RegisterCLRFieldSetter(field, set_USE_GM_13);
            app.RegisterCLRFieldBinding(field, CopyToStack_USE_GM_13, AssignFromStack_USE_GM_13);
            field = type.GetField("USE_VFPKSDK", flag);
            app.RegisterCLRFieldGetter(field, get_USE_VFPKSDK_14);
            app.RegisterCLRFieldSetter(field, set_USE_VFPKSDK_14);
            app.RegisterCLRFieldBinding(field, CopyToStack_USE_VFPKSDK_14, AssignFromStack_USE_VFPKSDK_14);


        }



        static object get_USE_ASTTEST_0(ref object o)
        {
            return global::ILRDefine.USE_ASTTEST;
        }

        static StackObject* CopyToStack_USE_ASTTEST_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::ILRDefine.USE_ASTTEST;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_USE_ASTTEST_0(ref object o, object v)
        {
            global::ILRDefine.USE_ASTTEST = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_USE_ASTTEST_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @USE_ASTTEST = ptr_of_this_method->Value == 1;
            global::ILRDefine.USE_ASTTEST = @USE_ASTTEST;
            return ptr_of_this_method;
        }

        static object get_DEBUG_1(ref object o)
        {
            return global::ILRDefine.DEBUG;
        }

        static StackObject* CopyToStack_DEBUG_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::ILRDefine.DEBUG;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_DEBUG_1(ref object o, object v)
        {
            global::ILRDefine.DEBUG = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_DEBUG_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @DEBUG = ptr_of_this_method->Value == 1;
            global::ILRDefine.DEBUG = @DEBUG;
            return ptr_of_this_method;
        }

        static object get_IS_FX_2(ref object o)
        {
            return global::ILRDefine.IS_FX;
        }

        static StackObject* CopyToStack_IS_FX_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::ILRDefine.IS_FX;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_IS_FX_2(ref object o, object v)
        {
            global::ILRDefine.IS_FX = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_IS_FX_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @IS_FX = ptr_of_this_method->Value == 1;
            global::ILRDefine.IS_FX = @IS_FX;
            return ptr_of_this_method;
        }

        static object get_UNITY_IPHONE_3(ref object o)
        {
            return global::ILRDefine.UNITY_IPHONE;
        }

        static StackObject* CopyToStack_UNITY_IPHONE_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::ILRDefine.UNITY_IPHONE;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_UNITY_IPHONE_3(ref object o, object v)
        {
            global::ILRDefine.UNITY_IPHONE = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_UNITY_IPHONE_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @UNITY_IPHONE = ptr_of_this_method->Value == 1;
            global::ILRDefine.UNITY_IPHONE = @UNITY_IPHONE;
            return ptr_of_this_method;
        }

        static object get_UNITY_EDITOR_4(ref object o)
        {
            return global::ILRDefine.UNITY_EDITOR;
        }

        static StackObject* CopyToStack_UNITY_EDITOR_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::ILRDefine.UNITY_EDITOR;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_UNITY_EDITOR_4(ref object o, object v)
        {
            global::ILRDefine.UNITY_EDITOR = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_UNITY_EDITOR_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @UNITY_EDITOR = ptr_of_this_method->Value == 1;
            global::ILRDefine.UNITY_EDITOR = @UNITY_EDITOR;
            return ptr_of_this_method;
        }

        static object get_USE_UMENG_5(ref object o)
        {
            return global::ILRDefine.USE_UMENG;
        }

        static StackObject* CopyToStack_USE_UMENG_5(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::ILRDefine.USE_UMENG;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_USE_UMENG_5(ref object o, object v)
        {
            global::ILRDefine.USE_UMENG = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_USE_UMENG_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @USE_UMENG = ptr_of_this_method->Value == 1;
            global::ILRDefine.USE_UMENG = @USE_UMENG;
            return ptr_of_this_method;
        }

        static object get_UNITY_ANDROID_6(ref object o)
        {
            return global::ILRDefine.UNITY_ANDROID;
        }

        static StackObject* CopyToStack_UNITY_ANDROID_6(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::ILRDefine.UNITY_ANDROID;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_UNITY_ANDROID_6(ref object o, object v)
        {
            global::ILRDefine.UNITY_ANDROID = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_UNITY_ANDROID_6(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @UNITY_ANDROID = ptr_of_this_method->Value == 1;
            global::ILRDefine.UNITY_ANDROID = @UNITY_ANDROID;
            return ptr_of_this_method;
        }

        static object get_USE_XINKUAISDK_7(ref object o)
        {
            return global::ILRDefine.USE_XINKUAISDK;
        }

        static StackObject* CopyToStack_USE_XINKUAISDK_7(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::ILRDefine.USE_XINKUAISDK;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_USE_XINKUAISDK_7(ref object o, object v)
        {
            global::ILRDefine.USE_XINKUAISDK = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_USE_XINKUAISDK_7(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @USE_XINKUAISDK = ptr_of_this_method->Value == 1;
            global::ILRDefine.USE_XINKUAISDK = @USE_XINKUAISDK;
            return ptr_of_this_method;
        }

        static object get_USE_WECHATSDK_8(ref object o)
        {
            return global::ILRDefine.USE_WECHATSDK;
        }

        static StackObject* CopyToStack_USE_WECHATSDK_8(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::ILRDefine.USE_WECHATSDK;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_USE_WECHATSDK_8(ref object o, object v)
        {
            global::ILRDefine.USE_WECHATSDK = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_USE_WECHATSDK_8(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @USE_WECHATSDK = ptr_of_this_method->Value == 1;
            global::ILRDefine.USE_WECHATSDK = @USE_WECHATSDK;
            return ptr_of_this_method;
        }

        static object get_USE_APPSFLYER_9(ref object o)
        {
            return global::ILRDefine.USE_APPSFLYER;
        }

        static StackObject* CopyToStack_USE_APPSFLYER_9(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::ILRDefine.USE_APPSFLYER;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_USE_APPSFLYER_9(ref object o, object v)
        {
            global::ILRDefine.USE_APPSFLYER = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_USE_APPSFLYER_9(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @USE_APPSFLYER = ptr_of_this_method->Value == 1;
            global::ILRDefine.USE_APPSFLYER = @USE_APPSFLYER;
            return ptr_of_this_method;
        }

        static object get_USE_AOSHITANGSDK_10(ref object o)
        {
            return global::ILRDefine.USE_AOSHITANGSDK;
        }

        static StackObject* CopyToStack_USE_AOSHITANGSDK_10(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::ILRDefine.USE_AOSHITANGSDK;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_USE_AOSHITANGSDK_10(ref object o, object v)
        {
            global::ILRDefine.USE_AOSHITANGSDK = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_USE_AOSHITANGSDK_10(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @USE_AOSHITANGSDK = ptr_of_this_method->Value == 1;
            global::ILRDefine.USE_AOSHITANGSDK = @USE_AOSHITANGSDK;
            return ptr_of_this_method;
        }

        static object get_USE_ALIPAYSDK_11(ref object o)
        {
            return global::ILRDefine.USE_ALIPAYSDK;
        }

        static StackObject* CopyToStack_USE_ALIPAYSDK_11(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::ILRDefine.USE_ALIPAYSDK;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_USE_ALIPAYSDK_11(ref object o, object v)
        {
            global::ILRDefine.USE_ALIPAYSDK = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_USE_ALIPAYSDK_11(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @USE_ALIPAYSDK = ptr_of_this_method->Value == 1;
            global::ILRDefine.USE_ALIPAYSDK = @USE_ALIPAYSDK;
            return ptr_of_this_method;
        }

        static object get_USE_AIHELP_12(ref object o)
        {
            return global::ILRDefine.USE_AIHELP;
        }

        static StackObject* CopyToStack_USE_AIHELP_12(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::ILRDefine.USE_AIHELP;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_USE_AIHELP_12(ref object o, object v)
        {
            global::ILRDefine.USE_AIHELP = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_USE_AIHELP_12(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @USE_AIHELP = ptr_of_this_method->Value == 1;
            global::ILRDefine.USE_AIHELP = @USE_AIHELP;
            return ptr_of_this_method;
        }

        static object get_USE_GM_13(ref object o)
        {
            return global::ILRDefine.USE_GM;
        }

        static StackObject* CopyToStack_USE_GM_13(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::ILRDefine.USE_GM;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_USE_GM_13(ref object o, object v)
        {
            global::ILRDefine.USE_GM = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_USE_GM_13(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @USE_GM = ptr_of_this_method->Value == 1;
            global::ILRDefine.USE_GM = @USE_GM;
            return ptr_of_this_method;
        }

        static object get_USE_VFPKSDK_14(ref object o)
        {
            return global::ILRDefine.USE_VFPKSDK;
        }

        static StackObject* CopyToStack_USE_VFPKSDK_14(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::ILRDefine.USE_VFPKSDK;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_USE_VFPKSDK_14(ref object o, object v)
        {
            global::ILRDefine.USE_VFPKSDK = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_USE_VFPKSDK_14(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @USE_VFPKSDK = ptr_of_this_method->Value == 1;
            global::ILRDefine.USE_VFPKSDK = @USE_VFPKSDK;
            return ptr_of_this_method;
        }



    }
}
