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
    unsafe class CampaignTextureCmp_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::CampaignTextureCmp);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("set_spriteName", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_spriteName_0);

            field = type.GetField("target", flag);
            app.RegisterCLRFieldGetter(field, get_target_0);
            app.RegisterCLRFieldSetter(field, set_target_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_target_0, AssignFromStack_target_0);
            field = type.GetField("onLoadingOver", flag);
            app.RegisterCLRFieldGetter(field, get_onLoadingOver_1);
            app.RegisterCLRFieldSetter(field, set_onLoadingOver_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_onLoadingOver_1, AssignFromStack_onLoadingOver_1);


        }


        static StackObject* set_spriteName_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @value = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::CampaignTextureCmp instance_of_this_method = (global::CampaignTextureCmp)typeof(global::CampaignTextureCmp).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.spriteName = value;

            return __ret;
        }


        static object get_target_0(ref object o)
        {
            return ((global::CampaignTextureCmp)o).target;
        }

        static StackObject* CopyToStack_target_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::CampaignTextureCmp)o).target;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_target_0(ref object o, object v)
        {
            ((global::CampaignTextureCmp)o).target = (global::UITexture)v;
        }

        static StackObject* AssignFromStack_target_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UITexture @target = (global::UITexture)typeof(global::UITexture).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::CampaignTextureCmp)o).target = @target;
            return ptr_of_this_method;
        }

        static object get_onLoadingOver_1(ref object o)
        {
            return ((global::CampaignTextureCmp)o).onLoadingOver;
        }

        static StackObject* CopyToStack_onLoadingOver_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::CampaignTextureCmp)o).onLoadingOver;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onLoadingOver_1(ref object o, object v)
        {
            ((global::CampaignTextureCmp)o).onLoadingOver = (System.Collections.Generic.List<global::EventDelegate>)v;
        }

        static StackObject* AssignFromStack_onLoadingOver_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::EventDelegate> @onLoadingOver = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::CampaignTextureCmp)o).onLoadingOver = @onLoadingOver;
            return ptr_of_this_method;
        }



    }
}
