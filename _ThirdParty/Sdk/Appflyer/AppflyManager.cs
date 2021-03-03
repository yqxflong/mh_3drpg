//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace Appsflyer
//{
//    public class AppflyManager: IAppsFlyerConversionData
//    {
//        //const string packagename = "org.manhuang.android.AppflyManager";
//        //static AndroidJavaClass _appflyerclass;
//        //static AndroidJavaClass Appflyerclass 
//        //{ 
//        //    get             
//        //    { 
//        //        if(_appflyerclass == null)
//        //        {
//        //            _appflyerclass = new AndroidJavaClass(packagename);
//        //        }
//        //        return _appflyerclass;
//        //    } 
//        //}
////        public static void InitAppsFlyer()
////        {
////#if UNITY_ANDROID
////            EB.Debug.Log("AppflyManager.InitAppsFlyer");
////            Appflyerclass.CallStatic("InitAppsflyer");
////#elif UNITY_UNITY_IPHONE
////#endif
////        }

////        public static void SendEventToAppflyer(string eventname,Dictionary<string,object> eventValue)
////        {
////#if UNITY_ANDROID
////            EB.Debug.Log("AppflyManager.SendEventToAppflyer eventname:{0}", eventname);
////            AndroidJavaObject javamap = Umeng.Analytics.ToJavaHashMap(eventValue);
////            Appflyerclass.CallStatic("SendEventWithListener", Umeng.Analytics.ToJavaObject(eventname), javamap);
////#elif UNITY_UNITY_IPHONE
////#endif
////        }
//        public static void InitAppsFlyer()
//        {

//        }
//        public static void SendEventToAppflyer(string eventname, Dictionary<string, object> eventValue)
//        {

//        }

//    }

//    //public class AFInAppEventType
//    //{
//    //    public const String LEVEL_ACHIEVED = "af_level_achieved";
//    //    public const String ADD_PAYMENT_INFO = "af_add_payment_info";
//    //    public const String ADD_TO_CART = "af_add_to_cart";
//    //    public const String ADD_TO_WISH_LIST = "af_add_to_wishlist";
//    //    public const String COMPLETE_REGISTRATION = "af_complete_registration";
//    //    public const String TUTORIAL_COMPLETION = "af_tutorial_completion";
//    //    public const String INITIATED_CHECKOUT = "af_initiated_checkout";
//    //    public const String PURCHASE = "af_purchase";
//    //    public const String RATE = "af_rate";
//    //    public const String SEARCH = "af_search";
//    //    public const String SPENT_CREDIT = "af_spent_credits";
//    //    public const String ACHIEVEMENT_UNLOCKED = "af_achievement_unlocked";
//    //    public const String CONTENT_VIEW = "af_content_view";
//    //    public const String TRAVEL_BOOKING = "af_travel_booking";
//    //    public const String SHARE = "af_share";
//    //    public const String INVITE = "af_invite";
//    //    public const String LOGIN = "af_login";
//    //    public const String RE_ENGAGE = "af_re_engage";
//    //    public const String UPDATE = "af_update";
//    //    public const String OPENED_FROM_PUSH_NOTIFICATION = "af_opened_from_push_notification";
//    //    public const String LOCATION_CHANGED = "af_location_changed";
//    //    public const String LOCATION_COORDINATES = "af_location_coordinates";
//    //    public const String ORDER_ID = "af_order_id";
//    //    public const String CUSTOMER_SEGMENT = "af_customer_segment";
//    //    public const String SUBSCRIBE = "af_subscribe";
//    //    public const String START_TRIAL = "af_start_trial";
//    //    public const String AD_CLICK = "af_ad_click";
//    //    public const String AD_VIEW = "af_ad_view";
//    //}

//    //public class AFInAppEventParameterName
//    //{
//    //    public const string LEVEL = "af_level";
//    //    public const string SCORE = "af_score";
//    //    public const string SUCCESS = "af_success";
//    //    public const string PRICE = "af_price";
//    //    public const string CONTENT_TYPE = "af_content_type";
//    //    public const string CONTENT_ID = "af_content_id";
//    //    public const string CONTENT_LIST = "af_content_list";
//    //    public const string CURRENCY = "af_currency";
//    //    public const string QUANTITY = "af_quantity";
//    //    public const string REGSITRATION_METHOD = "af_registration_method";
//    //    public const string PAYMENT_INFO_AVAILIBLE = "af_payment_info_available";
//    //    public const string MAX_RATING_VALUE = "af_max_rating_value";
//    //    public const string RATING_VALUE = "af_rating_value";
//    //    public const string SEARCH_string = "af_search_string";
//    //    public const string DATE_A = "af_date_a";
//    //    public const string DATE_B = "af_date_b";
//    //    public const string DESTINATION_A = "af_destination_a";
//    //    public const string DESTINATION_B = "af_destination_b";
//    //    public const string DESCRIPTION = "af_description";
//    //    public const string CLASS = "af_class";
//    //    public const string EVENT_START = "af_event_start";
//    //    public const string EVENT_END = "af_event_end";
//    //    public const string LATITUDE = "af_lat";
//    //    public const string LONGTITUDE = "af_long";
//    //    public const string CUSTOMER_USER_ID = "af_customer_user_id";
//    //    public const string VALIDATED = "af_validated";
//    //    public const string REVENUE = "af_revenue";
//    //    public const string PROJECTED_REVENUE = "af_projected_revenue";
//    //    public const string RECEIPT_ID = "af_receipt_id";
//    //    public const string TUTORIAL_ID = "af_tutorial_id";
//    //    public const string ACHIEVEMENT_ID = "af_achievement_id";
//    //    public const string VIRTUAL_CURRENCY_NAME = "af_virtual_currency_name";
//    //    public const string DEEP_LINK = "af_deep_link";
//    //    public const string OLD_VERSION = "af_old_version";
//    //    public const string NEW_VERSION = "af_new_version";
//    //    public const string REVIEW_TEXT = "af_review_text";
//    //    public const string COUPON_CODE = "af_coupon_code";
//    //    public const string PARAM_1 = "af_param_1";
//    //    public const string PARAM_2 = "af_param_2";
//    //    public const string PARAM_3 = "af_param_3";
//    //    public const string PARAM_4 = "af_param_4";
//    //    public const string PARAM_5 = "af_param_5";
//    //    public const string PARAM_6 = "af_param_6";
//    //    public const string PARAM_7 = "af_param_7";
//    //    public const string PARAM_8 = "af_param_8";
//    //    public const string PARAM_9 = "af_param_9";
//    //    public const string PARAM_10 = "af_param_10";
//    //    public const string DEPARTING_DEPARTURE_DATE = "af_departing_departure_date";
//    //    public const string RETURNING_DEPARTURE_DATE = "af_returning_departure_date";
//    //    public const string DESTINATION_LIST = "af_destination_list";
//    //    public const string CITY = "af_city";
//    //    public const string REGION = "af_region";
//    //    public const string COUNTRY = "af_country";
//    //    public const string DEPARTING_ARRIVAL_DATE = "af_departing_arrival_date";
//    //    public const string RETURNING_ARRIVAL_DATE = "af_returning_arrival_date";
//    //    public const string SUGGESTED_DESTINATIONS = "af_suggested_destinations";
//    //    public const string TRAVEL_START = "af_travel_start";
//    //    public const string TRAVEL_END = "af_travel_end";
//    //    public const string NUM_ADULTS = "af_num_adults";
//    //    public const string NUM_CHILDREN = "af_num_children";
//    //    public const string NUM_INFANTS = "af_num_infants";
//    //    public const string SUGGESTED_HOTELS = "af_suggested_hotels";
//    //    public const string USER_SCORE = "af_user_score";
//    //    public const string HOTEL_SCORE = "af_hotel_score";
//    //    public const string PURCHASE_CURRENCY = "af_purchase_currency";
//    //    public const string PREFERRED_STAR_RATINGS = "af_preferred_star_ratings";
//    //    public const string PREFERRED_PRICE_RANGE = "af_preferred_price_range";
//    //    public const string PREFERRED_NEIGHBORHOODS = "af_preferred_neighborhoods";
//    //    public const string PREFERRED_NUM_STOPS = "af_preferred_num_stops";
//    //    public const string AF_CHANNEL = "af_channel";
//    //    public const string CONTENT = "af_content";
//    //    public const string AD_REVENUE_AD_TYPE = "af_adrev_ad_type";
//    //    public const string AD_REVENUE_NETWORK_NAME = "af_adrev_network_name";
//    //    public const string AD_REVENUE_PLACEMENT_ID = "af_adrev_placement_id";
//    //    public const string AD_REVENUE_AD_SIZE = "af_adrev_ad_size";
//    //    public const string AD_REVENUE_MEDIATED_NETWORK_NAME = "af_adrev_mediated_network_name";
//    //}


//}
