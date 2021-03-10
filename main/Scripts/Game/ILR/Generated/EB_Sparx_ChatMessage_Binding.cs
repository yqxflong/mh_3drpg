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
    unsafe class EB_Sparx_ChatMessage_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.Sparx.ChatMessage);
            args = new Type[]{typeof(System.Object)};
            method = type.GetMethod("Parse", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Parse_0);

            field = type.GetField("json", flag);
            app.RegisterCLRFieldGetter(field, get_json_0);
            app.RegisterCLRFieldSetter(field, set_json_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_json_0, AssignFromStack_json_0);
            field = type.GetField("ts", flag);
            app.RegisterCLRFieldGetter(field, get_ts_1);
            app.RegisterCLRFieldSetter(field, set_ts_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_ts_1, AssignFromStack_ts_1);
            field = type.GetField("vipLevel", flag);
            app.RegisterCLRFieldGetter(field, get_vipLevel_2);
            app.RegisterCLRFieldSetter(field, set_vipLevel_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_vipLevel_2, AssignFromStack_vipLevel_2);
            field = type.GetField("quality", flag);
            app.RegisterCLRFieldGetter(field, get_quality_3);
            app.RegisterCLRFieldSetter(field, set_quality_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_quality_3, AssignFromStack_quality_3);
            field = type.GetField("isAudio", flag);
            app.RegisterCLRFieldGetter(field, get_isAudio_4);
            app.RegisterCLRFieldSetter(field, set_isAudio_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_isAudio_4, AssignFromStack_isAudio_4);
            field = type.GetField("name", flag);
            app.RegisterCLRFieldGetter(field, get_name_5);
            app.RegisterCLRFieldSetter(field, set_name_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_name_5, AssignFromStack_name_5);
            field = type.GetField("icon", flag);
            app.RegisterCLRFieldGetter(field, get_icon_6);
            app.RegisterCLRFieldSetter(field, set_icon_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_icon_6, AssignFromStack_icon_6);
            field = type.GetField("frame", flag);
            app.RegisterCLRFieldGetter(field, get_frame_7);
            app.RegisterCLRFieldSetter(field, set_frame_7);
            app.RegisterCLRFieldBinding(field, CopyToStack_frame_7, AssignFromStack_frame_7);
            field = type.GetField("battleRating", flag);
            app.RegisterCLRFieldGetter(field, get_battleRating_8);
            app.RegisterCLRFieldSetter(field, set_battleRating_8);
            app.RegisterCLRFieldBinding(field, CopyToStack_battleRating_8, AssignFromStack_battleRating_8);
            field = type.GetField("uid", flag);
            app.RegisterCLRFieldGetter(field, get_uid_9);
            app.RegisterCLRFieldSetter(field, set_uid_9);
            app.RegisterCLRFieldBinding(field, CopyToStack_uid_9, AssignFromStack_uid_9);
            field = type.GetField("level", flag);
            app.RegisterCLRFieldGetter(field, get_level_10);
            app.RegisterCLRFieldSetter(field, set_level_10);
            app.RegisterCLRFieldBinding(field, CopyToStack_level_10, AssignFromStack_level_10);
            field = type.GetField("channelType", flag);
            app.RegisterCLRFieldGetter(field, get_channelType_11);
            app.RegisterCLRFieldSetter(field, set_channelType_11);
            app.RegisterCLRFieldBinding(field, CopyToStack_channelType_11, AssignFromStack_channelType_11);
            field = type.GetField("audioClip", flag);
            app.RegisterCLRFieldGetter(field, get_audioClip_12);
            app.RegisterCLRFieldSetter(field, set_audioClip_12);
            app.RegisterCLRFieldBinding(field, CopyToStack_audioClip_12, AssignFromStack_audioClip_12);
            field = type.GetField("allianceName", flag);
            app.RegisterCLRFieldGetter(field, get_allianceName_13);
            app.RegisterCLRFieldSetter(field, set_allianceName_13);
            app.RegisterCLRFieldBinding(field, CopyToStack_allianceName_13, AssignFromStack_allianceName_13);
            field = type.GetField("lower", flag);
            app.RegisterCLRFieldGetter(field, get_lower_14);
            app.RegisterCLRFieldSetter(field, set_lower_14);
            app.RegisterCLRFieldBinding(field, CopyToStack_lower_14, AssignFromStack_lower_14);
            field = type.GetField("channel", flag);
            app.RegisterCLRFieldGetter(field, get_channel_15);
            app.RegisterCLRFieldSetter(field, set_channel_15);
            app.RegisterCLRFieldBinding(field, CopyToStack_channel_15, AssignFromStack_channel_15);
            field = type.GetField("ip", flag);
            app.RegisterCLRFieldGetter(field, get_ip_16);
            app.RegisterCLRFieldSetter(field, set_ip_16);
            app.RegisterCLRFieldBinding(field, CopyToStack_ip_16, AssignFromStack_ip_16);
            field = type.GetField("text", flag);
            app.RegisterCLRFieldGetter(field, get_text_17);
            app.RegisterCLRFieldSetter(field, set_text_17);
            app.RegisterCLRFieldBinding(field, CopyToStack_text_17, AssignFromStack_text_17);
            field = type.GetField("id", flag);
            app.RegisterCLRFieldGetter(field, get_id_18);
            app.RegisterCLRFieldSetter(field, set_id_18);
            app.RegisterCLRFieldBinding(field, CopyToStack_id_18, AssignFromStack_id_18);
            field = type.GetField("language", flag);
            app.RegisterCLRFieldGetter(field, get_language_19);
            app.RegisterCLRFieldSetter(field, set_language_19);
            app.RegisterCLRFieldBinding(field, CopyToStack_language_19, AssignFromStack_language_19);
            field = type.GetField("monthVipType", flag);
            app.RegisterCLRFieldGetter(field, get_monthVipType_20);
            app.RegisterCLRFieldSetter(field, set_monthVipType_20);
            app.RegisterCLRFieldBinding(field, CopyToStack_monthVipType_20, AssignFromStack_monthVipType_20);
            field = type.GetField("achievementType", flag);
            app.RegisterCLRFieldGetter(field, get_achievementType_21);
            app.RegisterCLRFieldSetter(field, set_achievementType_21);
            app.RegisterCLRFieldBinding(field, CopyToStack_achievementType_21, AssignFromStack_achievementType_21);
            field = type.GetField("privateUid", flag);
            app.RegisterCLRFieldGetter(field, get_privateUid_22);
            app.RegisterCLRFieldSetter(field, set_privateUid_22);
            app.RegisterCLRFieldBinding(field, CopyToStack_privateUid_22, AssignFromStack_privateUid_22);
            field = type.GetField("isRead", flag);
            app.RegisterCLRFieldGetter(field, get_isRead_23);
            app.RegisterCLRFieldSetter(field, set_isRead_23);
            app.RegisterCLRFieldBinding(field, CopyToStack_isRead_23, AssignFromStack_isRead_23);
            field = type.GetField("privateName", flag);
            app.RegisterCLRFieldGetter(field, get_privateName_24);
            app.RegisterCLRFieldSetter(field, set_privateName_24);
            app.RegisterCLRFieldBinding(field, CopyToStack_privateName_24, AssignFromStack_privateName_24);

            app.RegisterCLRCreateArrayInstance(type, s => new EB.Sparx.ChatMessage[s]);

            args = new Type[]{};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }


        static StackObject* Parse_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object @json = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = EB.Sparx.ChatMessage.Parse(@json);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_json_0(ref object o)
        {
            return ((EB.Sparx.ChatMessage)o).json;
        }

        static StackObject* CopyToStack_json_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatMessage)o).json;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance, true);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method, true);
        }

        static void set_json_0(ref object o, object v)
        {
            ((EB.Sparx.ChatMessage)o).json = (System.Object)v;
        }

        static StackObject* AssignFromStack_json_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Object @json = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.ChatMessage)o).json = @json;
            return ptr_of_this_method;
        }

        static object get_ts_1(ref object o)
        {
            return ((EB.Sparx.ChatMessage)o).ts;
        }

        static StackObject* CopyToStack_ts_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatMessage)o).ts;
            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_ts_1(ref object o, object v)
        {
            ((EB.Sparx.ChatMessage)o).ts = (System.Int64)v;
        }

        static StackObject* AssignFromStack_ts_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int64 @ts = *(long*)&ptr_of_this_method->Value;
            ((EB.Sparx.ChatMessage)o).ts = @ts;
            return ptr_of_this_method;
        }

        static object get_vipLevel_2(ref object o)
        {
            return ((EB.Sparx.ChatMessage)o).vipLevel;
        }

        static StackObject* CopyToStack_vipLevel_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatMessage)o).vipLevel;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_vipLevel_2(ref object o, object v)
        {
            ((EB.Sparx.ChatMessage)o).vipLevel = (System.Int32)v;
        }

        static StackObject* AssignFromStack_vipLevel_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @vipLevel = ptr_of_this_method->Value;
            ((EB.Sparx.ChatMessage)o).vipLevel = @vipLevel;
            return ptr_of_this_method;
        }

        static object get_quality_3(ref object o)
        {
            return ((EB.Sparx.ChatMessage)o).quality;
        }

        static StackObject* CopyToStack_quality_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatMessage)o).quality;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_quality_3(ref object o, object v)
        {
            ((EB.Sparx.ChatMessage)o).quality = (System.Int32)v;
        }

        static StackObject* AssignFromStack_quality_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @quality = ptr_of_this_method->Value;
            ((EB.Sparx.ChatMessage)o).quality = @quality;
            return ptr_of_this_method;
        }

        static object get_isAudio_4(ref object o)
        {
            return ((EB.Sparx.ChatMessage)o).isAudio;
        }

        static StackObject* CopyToStack_isAudio_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatMessage)o).isAudio;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_isAudio_4(ref object o, object v)
        {
            ((EB.Sparx.ChatMessage)o).isAudio = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_isAudio_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @isAudio = ptr_of_this_method->Value == 1;
            ((EB.Sparx.ChatMessage)o).isAudio = @isAudio;
            return ptr_of_this_method;
        }

        static object get_name_5(ref object o)
        {
            return ((EB.Sparx.ChatMessage)o).name;
        }

        static StackObject* CopyToStack_name_5(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatMessage)o).name;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_name_5(ref object o, object v)
        {
            ((EB.Sparx.ChatMessage)o).name = (System.String)v;
        }

        static StackObject* AssignFromStack_name_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @name = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.ChatMessage)o).name = @name;
            return ptr_of_this_method;
        }

        static object get_icon_6(ref object o)
        {
            return ((EB.Sparx.ChatMessage)o).icon;
        }

        static StackObject* CopyToStack_icon_6(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatMessage)o).icon;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_icon_6(ref object o, object v)
        {
            ((EB.Sparx.ChatMessage)o).icon = (System.String)v;
        }

        static StackObject* AssignFromStack_icon_6(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @icon = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.ChatMessage)o).icon = @icon;
            return ptr_of_this_method;
        }

        static object get_frame_7(ref object o)
        {
            return ((EB.Sparx.ChatMessage)o).frame;
        }

        static StackObject* CopyToStack_frame_7(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatMessage)o).frame;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_frame_7(ref object o, object v)
        {
            ((EB.Sparx.ChatMessage)o).frame = (System.String)v;
        }

        static StackObject* AssignFromStack_frame_7(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @frame = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.ChatMessage)o).frame = @frame;
            return ptr_of_this_method;
        }

        static object get_battleRating_8(ref object o)
        {
            return ((EB.Sparx.ChatMessage)o).battleRating;
        }

        static StackObject* CopyToStack_battleRating_8(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatMessage)o).battleRating;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_battleRating_8(ref object o, object v)
        {
            ((EB.Sparx.ChatMessage)o).battleRating = (System.Int32)v;
        }

        static StackObject* AssignFromStack_battleRating_8(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @battleRating = ptr_of_this_method->Value;
            ((EB.Sparx.ChatMessage)o).battleRating = @battleRating;
            return ptr_of_this_method;
        }

        static object get_uid_9(ref object o)
        {
            return ((EB.Sparx.ChatMessage)o).uid;
        }

        static StackObject* CopyToStack_uid_9(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatMessage)o).uid;
            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_uid_9(ref object o, object v)
        {
            ((EB.Sparx.ChatMessage)o).uid = (System.Int64)v;
        }

        static StackObject* AssignFromStack_uid_9(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int64 @uid = *(long*)&ptr_of_this_method->Value;
            ((EB.Sparx.ChatMessage)o).uid = @uid;
            return ptr_of_this_method;
        }

        static object get_level_10(ref object o)
        {
            return ((EB.Sparx.ChatMessage)o).level;
        }

        static StackObject* CopyToStack_level_10(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatMessage)o).level;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_level_10(ref object o, object v)
        {
            ((EB.Sparx.ChatMessage)o).level = (System.Int32)v;
        }

        static StackObject* AssignFromStack_level_10(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @level = ptr_of_this_method->Value;
            ((EB.Sparx.ChatMessage)o).level = @level;
            return ptr_of_this_method;
        }

        static object get_channelType_11(ref object o)
        {
            return ((EB.Sparx.ChatMessage)o).channelType;
        }

        static StackObject* CopyToStack_channelType_11(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatMessage)o).channelType;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_channelType_11(ref object o, object v)
        {
            ((EB.Sparx.ChatMessage)o).channelType = (System.String)v;
        }

        static StackObject* AssignFromStack_channelType_11(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @channelType = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.ChatMessage)o).channelType = @channelType;
            return ptr_of_this_method;
        }

        static object get_audioClip_12(ref object o)
        {
            return ((EB.Sparx.ChatMessage)o).audioClip;
        }

        static StackObject* CopyToStack_audioClip_12(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatMessage)o).audioClip;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_audioClip_12(ref object o, object v)
        {
            ((EB.Sparx.ChatMessage)o).audioClip = (UnityEngine.AudioClip)v;
        }

        static StackObject* AssignFromStack_audioClip_12(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.AudioClip @audioClip = (UnityEngine.AudioClip)typeof(UnityEngine.AudioClip).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.ChatMessage)o).audioClip = @audioClip;
            return ptr_of_this_method;
        }

        static object get_allianceName_13(ref object o)
        {
            return ((EB.Sparx.ChatMessage)o).allianceName;
        }

        static StackObject* CopyToStack_allianceName_13(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatMessage)o).allianceName;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_allianceName_13(ref object o, object v)
        {
            ((EB.Sparx.ChatMessage)o).allianceName = (System.String)v;
        }

        static StackObject* AssignFromStack_allianceName_13(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @allianceName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.ChatMessage)o).allianceName = @allianceName;
            return ptr_of_this_method;
        }

        static object get_lower_14(ref object o)
        {
            return ((EB.Sparx.ChatMessage)o).lower;
        }

        static StackObject* CopyToStack_lower_14(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatMessage)o).lower;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_lower_14(ref object o, object v)
        {
            ((EB.Sparx.ChatMessage)o).lower = (System.String)v;
        }

        static StackObject* AssignFromStack_lower_14(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @lower = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.ChatMessage)o).lower = @lower;
            return ptr_of_this_method;
        }

        static object get_channel_15(ref object o)
        {
            return ((EB.Sparx.ChatMessage)o).channel;
        }

        static StackObject* CopyToStack_channel_15(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatMessage)o).channel;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_channel_15(ref object o, object v)
        {
            ((EB.Sparx.ChatMessage)o).channel = (System.String)v;
        }

        static StackObject* AssignFromStack_channel_15(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @channel = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.ChatMessage)o).channel = @channel;
            return ptr_of_this_method;
        }

        static object get_ip_16(ref object o)
        {
            return ((EB.Sparx.ChatMessage)o).ip;
        }

        static StackObject* CopyToStack_ip_16(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatMessage)o).ip;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_ip_16(ref object o, object v)
        {
            ((EB.Sparx.ChatMessage)o).ip = (System.Net.IPAddress)v;
        }

        static StackObject* AssignFromStack_ip_16(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Net.IPAddress @ip = (System.Net.IPAddress)typeof(System.Net.IPAddress).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.ChatMessage)o).ip = @ip;
            return ptr_of_this_method;
        }

        static object get_text_17(ref object o)
        {
            return ((EB.Sparx.ChatMessage)o).text;
        }

        static StackObject* CopyToStack_text_17(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatMessage)o).text;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_text_17(ref object o, object v)
        {
            ((EB.Sparx.ChatMessage)o).text = (System.String)v;
        }

        static StackObject* AssignFromStack_text_17(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @text = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.ChatMessage)o).text = @text;
            return ptr_of_this_method;
        }

        static object get_id_18(ref object o)
        {
            return ((EB.Sparx.ChatMessage)o).id;
        }

        static StackObject* CopyToStack_id_18(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatMessage)o).id;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_id_18(ref object o, object v)
        {
            ((EB.Sparx.ChatMessage)o).id = (System.String)v;
        }

        static StackObject* AssignFromStack_id_18(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @id = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.ChatMessage)o).id = @id;
            return ptr_of_this_method;
        }

        static object get_language_19(ref object o)
        {
            return ((EB.Sparx.ChatMessage)o).language;
        }

        static StackObject* CopyToStack_language_19(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatMessage)o).language;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_language_19(ref object o, object v)
        {
            ((EB.Sparx.ChatMessage)o).language = (EB.Language)v;
        }

        static StackObject* AssignFromStack_language_19(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            EB.Language @language = (EB.Language)typeof(EB.Language).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.ChatMessage)o).language = @language;
            return ptr_of_this_method;
        }

        static object get_monthVipType_20(ref object o)
        {
            return ((EB.Sparx.ChatMessage)o).monthVipType;
        }

        static StackObject* CopyToStack_monthVipType_20(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatMessage)o).monthVipType;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_monthVipType_20(ref object o, object v)
        {
            ((EB.Sparx.ChatMessage)o).monthVipType = (System.Int32)v;
        }

        static StackObject* AssignFromStack_monthVipType_20(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @monthVipType = ptr_of_this_method->Value;
            ((EB.Sparx.ChatMessage)o).monthVipType = @monthVipType;
            return ptr_of_this_method;
        }

        static object get_achievementType_21(ref object o)
        {
            return ((EB.Sparx.ChatMessage)o).achievementType;
        }

        static StackObject* CopyToStack_achievementType_21(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatMessage)o).achievementType;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_achievementType_21(ref object o, object v)
        {
            ((EB.Sparx.ChatMessage)o).achievementType = (System.String)v;
        }

        static StackObject* AssignFromStack_achievementType_21(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @achievementType = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.ChatMessage)o).achievementType = @achievementType;
            return ptr_of_this_method;
        }

        static object get_privateUid_22(ref object o)
        {
            return ((EB.Sparx.ChatMessage)o).privateUid;
        }

        static StackObject* CopyToStack_privateUid_22(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatMessage)o).privateUid;
            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_privateUid_22(ref object o, object v)
        {
            ((EB.Sparx.ChatMessage)o).privateUid = (System.Int64)v;
        }

        static StackObject* AssignFromStack_privateUid_22(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int64 @privateUid = *(long*)&ptr_of_this_method->Value;
            ((EB.Sparx.ChatMessage)o).privateUid = @privateUid;
            return ptr_of_this_method;
        }

        static object get_isRead_23(ref object o)
        {
            return ((EB.Sparx.ChatMessage)o).isRead;
        }

        static StackObject* CopyToStack_isRead_23(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatMessage)o).isRead;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_isRead_23(ref object o, object v)
        {
            ((EB.Sparx.ChatMessage)o).isRead = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_isRead_23(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @isRead = ptr_of_this_method->Value == 1;
            ((EB.Sparx.ChatMessage)o).isRead = @isRead;
            return ptr_of_this_method;
        }

        static object get_privateName_24(ref object o)
        {
            return ((EB.Sparx.ChatMessage)o).privateName;
        }

        static StackObject* CopyToStack_privateName_24(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatMessage)o).privateName;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_privateName_24(ref object o, object v)
        {
            ((EB.Sparx.ChatMessage)o).privateName = (System.String)v;
        }

        static StackObject* AssignFromStack_privateName_24(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @privateName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.ChatMessage)o).privateName = @privateName;
            return ptr_of_this_method;
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);

            var result_of_this_method = new EB.Sparx.ChatMessage();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
