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
    unsafe class CharacterModel_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::CharacterModel);
            args = new Type[]{};
            method = type.GetMethod("get_ResourcePrefabNameMain", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ResourcePrefabNameMain_0);
            args = new Type[]{typeof(global::eGender)};
            method = type.GetMethod("PrefabNameFromGenderMain", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, PrefabNameFromGenderMain_1);

            field = type.GetField("heightOffset", flag);
            app.RegisterCLRFieldGetter(field, get_heightOffset_0);
            app.RegisterCLRFieldSetter(field, set_heightOffset_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_heightOffset_0, AssignFromStack_heightOffset_0);
            field = type.GetField("resourceDirectory", flag);
            app.RegisterCLRFieldGetter(field, get_resourceDirectory_1);
            app.RegisterCLRFieldSetter(field, set_resourceDirectory_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_resourceDirectory_1, AssignFromStack_resourceDirectory_1);
            field = type.GetField("prefabName", flag);
            app.RegisterCLRFieldGetter(field, get_prefabName_2);
            app.RegisterCLRFieldSetter(field, set_prefabName_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_prefabName_2, AssignFromStack_prefabName_2);
            field = type.GetField("team", flag);
            app.RegisterCLRFieldGetter(field, get_team_3);
            app.RegisterCLRFieldSetter(field, set_team_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_team_3, AssignFromStack_team_3);
            field = type.GetField("NPC_Template", flag);
            app.RegisterCLRFieldGetter(field, get_NPC_Template_4);
            app.RegisterCLRFieldSetter(field, set_NPC_Template_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_NPC_Template_4, AssignFromStack_NPC_Template_4);
            field = type.GetField("audioName", flag);
            app.RegisterCLRFieldGetter(field, get_audioName_5);
            app.RegisterCLRFieldSetter(field, set_audioName_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_audioName_5, AssignFromStack_audioName_5);


        }


        static StackObject* get_ResourcePrefabNameMain_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::CharacterModel instance_of_this_method = (global::CharacterModel)typeof(global::CharacterModel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ResourcePrefabNameMain;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* PrefabNameFromGenderMain_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::eGender @gender = (global::eGender)typeof(global::eGender).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::CharacterModel instance_of_this_method = (global::CharacterModel)typeof(global::CharacterModel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.PrefabNameFromGenderMain(@gender);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_heightOffset_0(ref object o)
        {
            return ((global::CharacterModel)o).heightOffset;
        }

        static StackObject* CopyToStack_heightOffset_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::CharacterModel)o).heightOffset;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_heightOffset_0(ref object o, object v)
        {
            ((global::CharacterModel)o).heightOffset = (System.Single)v;
        }

        static StackObject* AssignFromStack_heightOffset_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @heightOffset = *(float*)&ptr_of_this_method->Value;
            ((global::CharacterModel)o).heightOffset = @heightOffset;
            return ptr_of_this_method;
        }

        static object get_resourceDirectory_1(ref object o)
        {
            return ((global::CharacterModel)o).resourceDirectory;
        }

        static StackObject* CopyToStack_resourceDirectory_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::CharacterModel)o).resourceDirectory;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_resourceDirectory_1(ref object o, object v)
        {
            ((global::CharacterModel)o).resourceDirectory = (global::eResourceDirectory)v;
        }

        static StackObject* AssignFromStack_resourceDirectory_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::eResourceDirectory @resourceDirectory = (global::eResourceDirectory)typeof(global::eResourceDirectory).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::CharacterModel)o).resourceDirectory = @resourceDirectory;
            return ptr_of_this_method;
        }

        static object get_prefabName_2(ref object o)
        {
            return ((global::CharacterModel)o).prefabName;
        }

        static StackObject* CopyToStack_prefabName_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::CharacterModel)o).prefabName;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_prefabName_2(ref object o, object v)
        {
            ((global::CharacterModel)o).prefabName = (System.String)v;
        }

        static StackObject* AssignFromStack_prefabName_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @prefabName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::CharacterModel)o).prefabName = @prefabName;
            return ptr_of_this_method;
        }

        static object get_team_3(ref object o)
        {
            return ((global::CharacterModel)o).team;
        }

        static StackObject* CopyToStack_team_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::CharacterModel)o).team;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_team_3(ref object o, object v)
        {
            ((global::CharacterModel)o).team = (global::eTeamId)v;
        }

        static StackObject* AssignFromStack_team_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::eTeamId @team = (global::eTeamId)typeof(global::eTeamId).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::CharacterModel)o).team = @team;
            return ptr_of_this_method;
        }

        static object get_NPC_Template_4(ref object o)
        {
            return ((global::CharacterModel)o).NPC_Template;
        }

        static StackObject* CopyToStack_NPC_Template_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::CharacterModel)o).NPC_Template;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_NPC_Template_4(ref object o, object v)
        {
            ((global::CharacterModel)o).NPC_Template = (System.String)v;
        }

        static StackObject* AssignFromStack_NPC_Template_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @NPC_Template = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::CharacterModel)o).NPC_Template = @NPC_Template;
            return ptr_of_this_method;
        }

        static object get_audioName_5(ref object o)
        {
            return ((global::CharacterModel)o).audioName;
        }

        static StackObject* CopyToStack_audioName_5(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::CharacterModel)o).audioName;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_audioName_5(ref object o, object v)
        {
            ((global::CharacterModel)o).audioName = (System.String)v;
        }

        static StackObject* AssignFromStack_audioName_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @audioName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::CharacterModel)o).audioName = @audioName;
            return ptr_of_this_method;
        }



    }
}
