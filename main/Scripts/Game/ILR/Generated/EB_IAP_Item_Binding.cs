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
    unsafe class EB_IAP_Item_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.IAP.Item);

            field = type.GetField("cost", flag);
            app.RegisterCLRFieldGetter(field, get_cost_0);
            app.RegisterCLRFieldSetter(field, set_cost_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_cost_0, AssignFromStack_cost_0);
            field = type.GetField("value", flag);
            app.RegisterCLRFieldGetter(field, get_value_1);
            app.RegisterCLRFieldSetter(field, set_value_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_value_1, AssignFromStack_value_1);
            field = type.GetField("payoutId", flag);
            app.RegisterCLRFieldGetter(field, get_payoutId_2);
            app.RegisterCLRFieldSetter(field, set_payoutId_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_payoutId_2, AssignFromStack_payoutId_2);
            field = type.GetField("localizedCost", flag);
            app.RegisterCLRFieldGetter(field, get_localizedCost_3);
            app.RegisterCLRFieldSetter(field, set_localizedCost_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_localizedCost_3, AssignFromStack_localizedCost_3);
            field = type.GetField("redeemers", flag);
            app.RegisterCLRFieldGetter(field, get_redeemers_4);
            app.RegisterCLRFieldSetter(field, set_redeemers_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_redeemers_4, AssignFromStack_redeemers_4);
            field = type.GetField("buyLimit", flag);
            app.RegisterCLRFieldGetter(field, get_buyLimit_5);
            app.RegisterCLRFieldSetter(field, set_buyLimit_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_buyLimit_5, AssignFromStack_buyLimit_5);
            field = type.GetField("icon", flag);
            app.RegisterCLRFieldGetter(field, get_icon_6);
            app.RegisterCLRFieldSetter(field, set_icon_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_icon_6, AssignFromStack_icon_6);
            field = type.GetField("longName", flag);
            app.RegisterCLRFieldGetter(field, get_longName_7);
            app.RegisterCLRFieldSetter(field, set_longName_7);
            app.RegisterCLRFieldBinding(field, CopyToStack_longName_7, AssignFromStack_longName_7);
            field = type.GetField("discount", flag);
            app.RegisterCLRFieldGetter(field, get_discount_8);
            app.RegisterCLRFieldSetter(field, set_discount_8);
            app.RegisterCLRFieldBinding(field, CopyToStack_discount_8, AssignFromStack_discount_8);
            field = type.GetField("dayBuyLimit", flag);
            app.RegisterCLRFieldGetter(field, get_dayBuyLimit_9);
            app.RegisterCLRFieldSetter(field, set_dayBuyLimit_9);
            app.RegisterCLRFieldBinding(field, CopyToStack_dayBuyLimit_9, AssignFromStack_dayBuyLimit_9);
            field = type.GetField("weeklyBuyLimit", flag);
            app.RegisterCLRFieldGetter(field, get_weeklyBuyLimit_10);
            app.RegisterCLRFieldSetter(field, set_weeklyBuyLimit_10);
            app.RegisterCLRFieldBinding(field, CopyToStack_weeklyBuyLimit_10, AssignFromStack_weeklyBuyLimit_10);
            field = type.GetField("monthlyBuyLimit", flag);
            app.RegisterCLRFieldGetter(field, get_monthlyBuyLimit_11);
            app.RegisterCLRFieldSetter(field, set_monthlyBuyLimit_11);
            app.RegisterCLRFieldBinding(field, CopyToStack_monthlyBuyLimit_11, AssignFromStack_monthlyBuyLimit_11);
            field = type.GetField("LimitedTimeGiftId", flag);
            app.RegisterCLRFieldGetter(field, get_LimitedTimeGiftId_12);
            app.RegisterCLRFieldSetter(field, set_LimitedTimeGiftId_12);
            app.RegisterCLRFieldBinding(field, CopyToStack_LimitedTimeGiftId_12, AssignFromStack_LimitedTimeGiftId_12);
            field = type.GetField("localizedDesc", flag);
            app.RegisterCLRFieldGetter(field, get_localizedDesc_13);
            app.RegisterCLRFieldSetter(field, set_localizedDesc_13);
            app.RegisterCLRFieldBinding(field, CopyToStack_localizedDesc_13, AssignFromStack_localizedDesc_13);
            field = type.GetField("order", flag);
            app.RegisterCLRFieldGetter(field, get_order_14);
            app.RegisterCLRFieldSetter(field, set_order_14);
            app.RegisterCLRFieldBinding(field, CopyToStack_order_14, AssignFromStack_order_14);
            field = type.GetField("categoryValue", flag);
            app.RegisterCLRFieldGetter(field, get_categoryValue_15);
            app.RegisterCLRFieldSetter(field, set_categoryValue_15);
            app.RegisterCLRFieldBinding(field, CopyToStack_categoryValue_15, AssignFromStack_categoryValue_15);
            field = type.GetField("twoMultiple", flag);
            app.RegisterCLRFieldGetter(field, get_twoMultiple_16);
            app.RegisterCLRFieldSetter(field, set_twoMultiple_16);
            app.RegisterCLRFieldBinding(field, CopyToStack_twoMultiple_16, AssignFromStack_twoMultiple_16);
            field = type.GetField("show", flag);
            app.RegisterCLRFieldGetter(field, get_show_17);
            app.RegisterCLRFieldSetter(field, set_show_17);
            app.RegisterCLRFieldBinding(field, CopyToStack_show_17, AssignFromStack_show_17);
            field = type.GetField("limitNum", flag);
            app.RegisterCLRFieldGetter(field, get_limitNum_18);
            app.RegisterCLRFieldSetter(field, set_limitNum_18);
            app.RegisterCLRFieldBinding(field, CopyToStack_limitNum_18, AssignFromStack_limitNum_18);
            field = type.GetField("cents", flag);
            app.RegisterCLRFieldGetter(field, get_cents_19);
            app.RegisterCLRFieldSetter(field, set_cents_19);
            app.RegisterCLRFieldBinding(field, CopyToStack_cents_19, AssignFromStack_cents_19);
            field = type.GetField("category", flag);
            app.RegisterCLRFieldGetter(field, get_category_20);
            app.RegisterCLRFieldSetter(field, set_category_20);
            app.RegisterCLRFieldBinding(field, CopyToStack_category_20, AssignFromStack_category_20);
            field = type.GetField("currencyCode", flag);
            app.RegisterCLRFieldGetter(field, get_currencyCode_21);
            app.RegisterCLRFieldSetter(field, set_currencyCode_21);
            app.RegisterCLRFieldBinding(field, CopyToStack_currencyCode_21, AssignFromStack_currencyCode_21);
            field = type.GetField("productId", flag);
            app.RegisterCLRFieldGetter(field, get_productId_22);
            app.RegisterCLRFieldSetter(field, set_productId_22);
            app.RegisterCLRFieldBinding(field, CopyToStack_productId_22, AssignFromStack_productId_22);

            app.RegisterCLRCreateArrayInstance(type, s => new EB.IAP.Item[s]);

            args = new Type[]{typeof(System.Object)};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }



        static object get_cost_0(ref object o)
        {
            return ((EB.IAP.Item)o).cost;
        }

        static StackObject* CopyToStack_cost_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.IAP.Item)o).cost;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_cost_0(ref object o, object v)
        {
            ((EB.IAP.Item)o).cost = (System.Single)v;
        }

        static StackObject* AssignFromStack_cost_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @cost = *(float*)&ptr_of_this_method->Value;
            ((EB.IAP.Item)o).cost = @cost;
            return ptr_of_this_method;
        }

        static object get_value_1(ref object o)
        {
            return ((EB.IAP.Item)o).value;
        }

        static StackObject* CopyToStack_value_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.IAP.Item)o).value;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_value_1(ref object o, object v)
        {
            ((EB.IAP.Item)o).value = (System.Int32)v;
        }

        static StackObject* AssignFromStack_value_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @value = ptr_of_this_method->Value;
            ((EB.IAP.Item)o).value = @value;
            return ptr_of_this_method;
        }

        static object get_payoutId_2(ref object o)
        {
            return ((EB.IAP.Item)o).payoutId;
        }

        static StackObject* CopyToStack_payoutId_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.IAP.Item)o).payoutId;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_payoutId_2(ref object o, object v)
        {
            ((EB.IAP.Item)o).payoutId = (System.Int32)v;
        }

        static StackObject* AssignFromStack_payoutId_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @payoutId = ptr_of_this_method->Value;
            ((EB.IAP.Item)o).payoutId = @payoutId;
            return ptr_of_this_method;
        }

        static object get_localizedCost_3(ref object o)
        {
            return ((EB.IAP.Item)o).localizedCost;
        }

        static StackObject* CopyToStack_localizedCost_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.IAP.Item)o).localizedCost;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_localizedCost_3(ref object o, object v)
        {
            ((EB.IAP.Item)o).localizedCost = (System.String)v;
        }

        static StackObject* AssignFromStack_localizedCost_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @localizedCost = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.IAP.Item)o).localizedCost = @localizedCost;
            return ptr_of_this_method;
        }

        static object get_redeemers_4(ref object o)
        {
            return ((EB.IAP.Item)o).redeemers;
        }

        static StackObject* CopyToStack_redeemers_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.IAP.Item)o).redeemers;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_redeemers_4(ref object o, object v)
        {
            ((EB.IAP.Item)o).redeemers = (System.Collections.Generic.List<EB.Sparx.RedeemerItem>)v;
        }

        static StackObject* AssignFromStack_redeemers_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<EB.Sparx.RedeemerItem> @redeemers = (System.Collections.Generic.List<EB.Sparx.RedeemerItem>)typeof(System.Collections.Generic.List<EB.Sparx.RedeemerItem>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.IAP.Item)o).redeemers = @redeemers;
            return ptr_of_this_method;
        }

        static object get_buyLimit_5(ref object o)
        {
            return ((EB.IAP.Item)o).buyLimit;
        }

        static StackObject* CopyToStack_buyLimit_5(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.IAP.Item)o).buyLimit;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_buyLimit_5(ref object o, object v)
        {
            ((EB.IAP.Item)o).buyLimit = (System.Int32)v;
        }

        static StackObject* AssignFromStack_buyLimit_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @buyLimit = ptr_of_this_method->Value;
            ((EB.IAP.Item)o).buyLimit = @buyLimit;
            return ptr_of_this_method;
        }

        static object get_icon_6(ref object o)
        {
            return ((EB.IAP.Item)o).icon;
        }

        static StackObject* CopyToStack_icon_6(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.IAP.Item)o).icon;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_icon_6(ref object o, object v)
        {
            ((EB.IAP.Item)o).icon = (System.String)v;
        }

        static StackObject* AssignFromStack_icon_6(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @icon = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.IAP.Item)o).icon = @icon;
            return ptr_of_this_method;
        }

        static object get_longName_7(ref object o)
        {
            return ((EB.IAP.Item)o).longName;
        }

        static StackObject* CopyToStack_longName_7(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.IAP.Item)o).longName;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_longName_7(ref object o, object v)
        {
            ((EB.IAP.Item)o).longName = (System.String)v;
        }

        static StackObject* AssignFromStack_longName_7(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @longName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.IAP.Item)o).longName = @longName;
            return ptr_of_this_method;
        }

        static object get_discount_8(ref object o)
        {
            return ((EB.IAP.Item)o).discount;
        }

        static StackObject* CopyToStack_discount_8(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.IAP.Item)o).discount;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_discount_8(ref object o, object v)
        {
            ((EB.IAP.Item)o).discount = (System.Single)v;
        }

        static StackObject* AssignFromStack_discount_8(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @discount = *(float*)&ptr_of_this_method->Value;
            ((EB.IAP.Item)o).discount = @discount;
            return ptr_of_this_method;
        }

        static object get_dayBuyLimit_9(ref object o)
        {
            return ((EB.IAP.Item)o).dayBuyLimit;
        }

        static StackObject* CopyToStack_dayBuyLimit_9(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.IAP.Item)o).dayBuyLimit;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_dayBuyLimit_9(ref object o, object v)
        {
            ((EB.IAP.Item)o).dayBuyLimit = (System.Int32)v;
        }

        static StackObject* AssignFromStack_dayBuyLimit_9(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @dayBuyLimit = ptr_of_this_method->Value;
            ((EB.IAP.Item)o).dayBuyLimit = @dayBuyLimit;
            return ptr_of_this_method;
        }

        static object get_weeklyBuyLimit_10(ref object o)
        {
            return ((EB.IAP.Item)o).weeklyBuyLimit;
        }

        static StackObject* CopyToStack_weeklyBuyLimit_10(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.IAP.Item)o).weeklyBuyLimit;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_weeklyBuyLimit_10(ref object o, object v)
        {
            ((EB.IAP.Item)o).weeklyBuyLimit = (System.Int32)v;
        }

        static StackObject* AssignFromStack_weeklyBuyLimit_10(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @weeklyBuyLimit = ptr_of_this_method->Value;
            ((EB.IAP.Item)o).weeklyBuyLimit = @weeklyBuyLimit;
            return ptr_of_this_method;
        }

        static object get_monthlyBuyLimit_11(ref object o)
        {
            return ((EB.IAP.Item)o).monthlyBuyLimit;
        }

        static StackObject* CopyToStack_monthlyBuyLimit_11(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.IAP.Item)o).monthlyBuyLimit;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_monthlyBuyLimit_11(ref object o, object v)
        {
            ((EB.IAP.Item)o).monthlyBuyLimit = (System.Int32)v;
        }

        static StackObject* AssignFromStack_monthlyBuyLimit_11(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @monthlyBuyLimit = ptr_of_this_method->Value;
            ((EB.IAP.Item)o).monthlyBuyLimit = @monthlyBuyLimit;
            return ptr_of_this_method;
        }

        static object get_LimitedTimeGiftId_12(ref object o)
        {
            return ((EB.IAP.Item)o).LimitedTimeGiftId;
        }

        static StackObject* CopyToStack_LimitedTimeGiftId_12(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.IAP.Item)o).LimitedTimeGiftId;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_LimitedTimeGiftId_12(ref object o, object v)
        {
            ((EB.IAP.Item)o).LimitedTimeGiftId = (System.String)v;
        }

        static StackObject* AssignFromStack_LimitedTimeGiftId_12(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @LimitedTimeGiftId = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.IAP.Item)o).LimitedTimeGiftId = @LimitedTimeGiftId;
            return ptr_of_this_method;
        }

        static object get_localizedDesc_13(ref object o)
        {
            return ((EB.IAP.Item)o).localizedDesc;
        }

        static StackObject* CopyToStack_localizedDesc_13(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.IAP.Item)o).localizedDesc;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_localizedDesc_13(ref object o, object v)
        {
            ((EB.IAP.Item)o).localizedDesc = (System.String)v;
        }

        static StackObject* AssignFromStack_localizedDesc_13(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @localizedDesc = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.IAP.Item)o).localizedDesc = @localizedDesc;
            return ptr_of_this_method;
        }

        static object get_order_14(ref object o)
        {
            return ((EB.IAP.Item)o).order;
        }

        static StackObject* CopyToStack_order_14(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.IAP.Item)o).order;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_order_14(ref object o, object v)
        {
            ((EB.IAP.Item)o).order = (System.Int32)v;
        }

        static StackObject* AssignFromStack_order_14(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @order = ptr_of_this_method->Value;
            ((EB.IAP.Item)o).order = @order;
            return ptr_of_this_method;
        }

        static object get_categoryValue_15(ref object o)
        {
            return ((EB.IAP.Item)o).categoryValue;
        }

        static StackObject* CopyToStack_categoryValue_15(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.IAP.Item)o).categoryValue;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_categoryValue_15(ref object o, object v)
        {
            ((EB.IAP.Item)o).categoryValue = (System.Int32)v;
        }

        static StackObject* AssignFromStack_categoryValue_15(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @categoryValue = ptr_of_this_method->Value;
            ((EB.IAP.Item)o).categoryValue = @categoryValue;
            return ptr_of_this_method;
        }

        static object get_twoMultiple_16(ref object o)
        {
            return ((EB.IAP.Item)o).twoMultiple;
        }

        static StackObject* CopyToStack_twoMultiple_16(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.IAP.Item)o).twoMultiple;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_twoMultiple_16(ref object o, object v)
        {
            ((EB.IAP.Item)o).twoMultiple = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_twoMultiple_16(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @twoMultiple = ptr_of_this_method->Value == 1;
            ((EB.IAP.Item)o).twoMultiple = @twoMultiple;
            return ptr_of_this_method;
        }

        static object get_show_17(ref object o)
        {
            return ((EB.IAP.Item)o).show;
        }

        static StackObject* CopyToStack_show_17(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.IAP.Item)o).show;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_show_17(ref object o, object v)
        {
            ((EB.IAP.Item)o).show = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_show_17(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @show = ptr_of_this_method->Value == 1;
            ((EB.IAP.Item)o).show = @show;
            return ptr_of_this_method;
        }

        static object get_limitNum_18(ref object o)
        {
            return ((EB.IAP.Item)o).limitNum;
        }

        static StackObject* CopyToStack_limitNum_18(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.IAP.Item)o).limitNum;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_limitNum_18(ref object o, object v)
        {
            ((EB.IAP.Item)o).limitNum = (System.Int32)v;
        }

        static StackObject* AssignFromStack_limitNum_18(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @limitNum = ptr_of_this_method->Value;
            ((EB.IAP.Item)o).limitNum = @limitNum;
            return ptr_of_this_method;
        }

        static object get_cents_19(ref object o)
        {
            return ((EB.IAP.Item)o).cents;
        }

        static StackObject* CopyToStack_cents_19(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.IAP.Item)o).cents;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_cents_19(ref object o, object v)
        {
            ((EB.IAP.Item)o).cents = (System.Int32)v;
        }

        static StackObject* AssignFromStack_cents_19(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @cents = ptr_of_this_method->Value;
            ((EB.IAP.Item)o).cents = @cents;
            return ptr_of_this_method;
        }

        static object get_category_20(ref object o)
        {
            return ((EB.IAP.Item)o).category;
        }

        static StackObject* CopyToStack_category_20(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.IAP.Item)o).category;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_category_20(ref object o, object v)
        {
            ((EB.IAP.Item)o).category = (System.String)v;
        }

        static StackObject* AssignFromStack_category_20(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @category = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.IAP.Item)o).category = @category;
            return ptr_of_this_method;
        }

        static object get_currencyCode_21(ref object o)
        {
            return ((EB.IAP.Item)o).currencyCode;
        }

        static StackObject* CopyToStack_currencyCode_21(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.IAP.Item)o).currencyCode;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_currencyCode_21(ref object o, object v)
        {
            ((EB.IAP.Item)o).currencyCode = (System.String)v;
        }

        static StackObject* AssignFromStack_currencyCode_21(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @currencyCode = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.IAP.Item)o).currencyCode = @currencyCode;
            return ptr_of_this_method;
        }

        static object get_productId_22(ref object o)
        {
            return ((EB.IAP.Item)o).productId;
        }

        static StackObject* CopyToStack_productId_22(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.IAP.Item)o).productId;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_productId_22(ref object o, object v)
        {
            ((EB.IAP.Item)o).productId = (System.String)v;
        }

        static StackObject* AssignFromStack_productId_22(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @productId = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.IAP.Item)o).productId = @productId;
            return ptr_of_this_method;
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object @freeGiftItem = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = new EB.IAP.Item(@freeGiftItem);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
