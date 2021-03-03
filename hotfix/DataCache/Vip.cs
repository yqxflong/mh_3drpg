// automatically generated, do not modify

namespace GM.DataCache
{

using System;
using FlatBuffers;

public sealed class VipInfo : Table {
  public static VipInfo GetRootAsVipInfo(ByteBuffer _bb) { return GetRootAsVipInfo(_bb, new VipInfo()); }
  public static VipInfo GetRootAsVipInfo(ByteBuffer _bb, VipInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public VipInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Level { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Type { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int TotalHc { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int BuyVigorTimes { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int BuyBuddyExpTimes { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int BuyGoldTimes { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int BountyDoubleTimes { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int EscortDoubleTimes { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ArenaDoubleTimes { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int LuckyCat { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ActicketTimes { get { int o = __offset(24); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ArenaTimes { get { int o = __offset(26); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ResignInTimes { get { int o = __offset(28); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int EscortTimes { get { int o = __offset(30); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int InfiniteChallengeTimes { get { int o = __offset(32); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int BountyTaskTimes { get { int o = __offset(34); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float TreasureRating { get { int o = __offset(36); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float ExpSpringRating { get { int o = __offset(38); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int LostChallengeReviveTimes { get { int o = __offset(40); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float ShopDiscount { get { int o = __offset(42); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public string EatEquipmentGift { get { int o = __offset(44); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetEatEquipmentGiftBytes() { return __vector_as_arraysegment(44); }
  public int RobTimes { get { int o = __offset(46); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int EscortRefreshTimes { get { int o = __offset(48); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int EscortHelpTimes { get { int o = __offset(50); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int EscortReinforceTimes { get { int o = __offset(52); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int RedNameAttackTimes { get { int o = __offset(54); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int RedNameRemovalTimes { get { int o = __offset(56); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string GiftItemId { get { int o = __offset(58); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetGiftItemIdBytes() { return __vector_as_arraysegment(58); }
  public int InvEquipCount { get { int o = __offset(60); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public bool CanBlitzChallCampaign { get { int o = __offset(62); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool CanBlitzAlliCampaign { get { int o = __offset(64); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int BuyFreeVigorTimes { get { int o = __offset(66); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int InfiDiscountTimes { get { int o = __offset(68); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int AwakenCampDiscountTimes { get { int o = __offset(70); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int HonorArenaIdleRewardLimit { get { int o = __offset(72); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int AlliDonateTimes { get { int o = __offset(74); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public bool FirstDonateDouble { get { int o = __offset(76); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int DayBuyLimit { get { int o = __offset(78); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int WeeklyBuyLimit { get { int o = __offset(80); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int MonthlyBuyLimit { get { int o = __offset(82); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int InfiniteChallengeResetTimes { get { int o = __offset(84); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public bool IsFreeRevive { get { int o = __offset(86); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool IsMysteryShopShow { get { int o = __offset(88); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool CanBlitzTenTimes { get { int o = __offset(90); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int ExpeditionRating { get { int o = __offset(92); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int DailyHcReward { get { int o = __offset(94); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int GiftPrice { get { int o = __offset(96); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public bool IsCanQuickRefining { get { int o = __offset(98); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int EscortAndRobTimes { get { int o = __offset(100); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int FreeRefiningTimes { get { int o = __offset(102); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<VipInfo> CreateVipInfo(FlatBufferBuilder builder,
      int level = 0,
      int type = 0,
      int total_hc = 0,
      int buy_vigor_times = 0,
      int buy_buddy_exp_times = 0,
      int buy_gold_times = 0,
      int bounty_double_times = 0,
      int escort_double_times = 0,
      int arena_double_times = 0,
      int lucky_cat = 0,
      int acticket_times = 0,
      int arena_times = 0,
      int resign_in_times = 0,
      int escort_times = 0,
      int infinite_challenge_times = 0,
      int bounty_task_times = 0,
      float treasure_rating = 0,
      float exp_spring_rating = 0,
      int lost_challenge_revive_times = 0,
      float shop_discount = 0,
      StringOffset eat_equipment_giftOffset = default(StringOffset),
      int rob_times = 0,
      int escort_refresh_times = 0,
      int escort_help_times = 0,
      int escort_reinforce_times = 0,
      int red_name_attack_times = 0,
      int red_name_removal_times = 0,
      StringOffset gift_item_idOffset = default(StringOffset),
      int inv_equip_count = 0,
      bool can_blitz_chall_campaign = false,
      bool can_blitz_alli_campaign = false,
      int buy_free_vigor_times = 0,
      int infi_discount_times = 0,
      int awaken_camp_discount_times = 0,
      int honor_arena_idle_reward_limit = 0,
      int alli_donate_times = 0,
      bool first_donate_double = false,
      int day_buy_limit = 0,
      int weekly_buy_limit = 0,
      int monthly_buy_limit = 0,
      int infinite_challenge_reset_times = 0,
      bool is_free_revive = false,
      bool is_mystery_shop_show = false,
      bool can_blitz_ten_times = false,
      int expedition_rating = 0,
      int daily_hc_reward = 0,
      int gift_price = 0,
      bool is_can_quick_refining = false,
      int escort_and_rob_times = 0,
      int free_refining_times = 0) {
    builder.StartObject(50);
    VipInfo.AddFreeRefiningTimes(builder, free_refining_times);
    VipInfo.AddEscortAndRobTimes(builder, escort_and_rob_times);
    VipInfo.AddGiftPrice(builder, gift_price);
    VipInfo.AddDailyHcReward(builder, daily_hc_reward);
    VipInfo.AddExpeditionRating(builder, expedition_rating);
    VipInfo.AddInfiniteChallengeResetTimes(builder, infinite_challenge_reset_times);
    VipInfo.AddMonthlyBuyLimit(builder, monthly_buy_limit);
    VipInfo.AddWeeklyBuyLimit(builder, weekly_buy_limit);
    VipInfo.AddDayBuyLimit(builder, day_buy_limit);
    VipInfo.AddAlliDonateTimes(builder, alli_donate_times);
    VipInfo.AddHonorArenaIdleRewardLimit(builder, honor_arena_idle_reward_limit);
    VipInfo.AddAwakenCampDiscountTimes(builder, awaken_camp_discount_times);
    VipInfo.AddInfiDiscountTimes(builder, infi_discount_times);
    VipInfo.AddBuyFreeVigorTimes(builder, buy_free_vigor_times);
    VipInfo.AddInvEquipCount(builder, inv_equip_count);
    VipInfo.AddGiftItemId(builder, gift_item_idOffset);
    VipInfo.AddRedNameRemovalTimes(builder, red_name_removal_times);
    VipInfo.AddRedNameAttackTimes(builder, red_name_attack_times);
    VipInfo.AddEscortReinforceTimes(builder, escort_reinforce_times);
    VipInfo.AddEscortHelpTimes(builder, escort_help_times);
    VipInfo.AddEscortRefreshTimes(builder, escort_refresh_times);
    VipInfo.AddRobTimes(builder, rob_times);
    VipInfo.AddEatEquipmentGift(builder, eat_equipment_giftOffset);
    VipInfo.AddShopDiscount(builder, shop_discount);
    VipInfo.AddLostChallengeReviveTimes(builder, lost_challenge_revive_times);
    VipInfo.AddExpSpringRating(builder, exp_spring_rating);
    VipInfo.AddTreasureRating(builder, treasure_rating);
    VipInfo.AddBountyTaskTimes(builder, bounty_task_times);
    VipInfo.AddInfiniteChallengeTimes(builder, infinite_challenge_times);
    VipInfo.AddEscortTimes(builder, escort_times);
    VipInfo.AddResignInTimes(builder, resign_in_times);
    VipInfo.AddArenaTimes(builder, arena_times);
    VipInfo.AddActicketTimes(builder, acticket_times);
    VipInfo.AddLuckyCat(builder, lucky_cat);
    VipInfo.AddArenaDoubleTimes(builder, arena_double_times);
    VipInfo.AddEscortDoubleTimes(builder, escort_double_times);
    VipInfo.AddBountyDoubleTimes(builder, bounty_double_times);
    VipInfo.AddBuyGoldTimes(builder, buy_gold_times);
    VipInfo.AddBuyBuddyExpTimes(builder, buy_buddy_exp_times);
    VipInfo.AddBuyVigorTimes(builder, buy_vigor_times);
    VipInfo.AddTotalHc(builder, total_hc);
    VipInfo.AddType(builder, type);
    VipInfo.AddLevel(builder, level);
    VipInfo.AddIsCanQuickRefining(builder, is_can_quick_refining);
    VipInfo.AddCanBlitzTenTimes(builder, can_blitz_ten_times);
    VipInfo.AddIsMysteryShopShow(builder, is_mystery_shop_show);
    VipInfo.AddIsFreeRevive(builder, is_free_revive);
    VipInfo.AddFirstDonateDouble(builder, first_donate_double);
    VipInfo.AddCanBlitzAlliCampaign(builder, can_blitz_alli_campaign);
    VipInfo.AddCanBlitzChallCampaign(builder, can_blitz_chall_campaign);
    return VipInfo.EndVipInfo(builder);
  }

  public static void StartVipInfo(FlatBufferBuilder builder) { builder.StartObject(50); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(0, level, 0); }
  public static void AddType(FlatBufferBuilder builder, int type) { builder.AddInt(1, type, 0); }
  public static void AddTotalHc(FlatBufferBuilder builder, int totalHc) { builder.AddInt(2, totalHc, 0); }
  public static void AddBuyVigorTimes(FlatBufferBuilder builder, int buyVigorTimes) { builder.AddInt(3, buyVigorTimes, 0); }
  public static void AddBuyBuddyExpTimes(FlatBufferBuilder builder, int buyBuddyExpTimes) { builder.AddInt(4, buyBuddyExpTimes, 0); }
  public static void AddBuyGoldTimes(FlatBufferBuilder builder, int buyGoldTimes) { builder.AddInt(5, buyGoldTimes, 0); }
  public static void AddBountyDoubleTimes(FlatBufferBuilder builder, int bountyDoubleTimes) { builder.AddInt(6, bountyDoubleTimes, 0); }
  public static void AddEscortDoubleTimes(FlatBufferBuilder builder, int escortDoubleTimes) { builder.AddInt(7, escortDoubleTimes, 0); }
  public static void AddArenaDoubleTimes(FlatBufferBuilder builder, int arenaDoubleTimes) { builder.AddInt(8, arenaDoubleTimes, 0); }
  public static void AddLuckyCat(FlatBufferBuilder builder, int luckyCat) { builder.AddInt(9, luckyCat, 0); }
  public static void AddActicketTimes(FlatBufferBuilder builder, int acticketTimes) { builder.AddInt(10, acticketTimes, 0); }
  public static void AddArenaTimes(FlatBufferBuilder builder, int arenaTimes) { builder.AddInt(11, arenaTimes, 0); }
  public static void AddResignInTimes(FlatBufferBuilder builder, int resignInTimes) { builder.AddInt(12, resignInTimes, 0); }
  public static void AddEscortTimes(FlatBufferBuilder builder, int escortTimes) { builder.AddInt(13, escortTimes, 0); }
  public static void AddInfiniteChallengeTimes(FlatBufferBuilder builder, int infiniteChallengeTimes) { builder.AddInt(14, infiniteChallengeTimes, 0); }
  public static void AddBountyTaskTimes(FlatBufferBuilder builder, int bountyTaskTimes) { builder.AddInt(15, bountyTaskTimes, 0); }
  public static void AddTreasureRating(FlatBufferBuilder builder, float treasureRating) { builder.AddFloat(16, treasureRating, 0); }
  public static void AddExpSpringRating(FlatBufferBuilder builder, float expSpringRating) { builder.AddFloat(17, expSpringRating, 0); }
  public static void AddLostChallengeReviveTimes(FlatBufferBuilder builder, int lostChallengeReviveTimes) { builder.AddInt(18, lostChallengeReviveTimes, 0); }
  public static void AddShopDiscount(FlatBufferBuilder builder, float shopDiscount) { builder.AddFloat(19, shopDiscount, 0); }
  public static void AddEatEquipmentGift(FlatBufferBuilder builder, StringOffset eatEquipmentGiftOffset) { builder.AddOffset(20, eatEquipmentGiftOffset.Value, 0); }
  public static void AddRobTimes(FlatBufferBuilder builder, int robTimes) { builder.AddInt(21, robTimes, 0); }
  public static void AddEscortRefreshTimes(FlatBufferBuilder builder, int escortRefreshTimes) { builder.AddInt(22, escortRefreshTimes, 0); }
  public static void AddEscortHelpTimes(FlatBufferBuilder builder, int escortHelpTimes) { builder.AddInt(23, escortHelpTimes, 0); }
  public static void AddEscortReinforceTimes(FlatBufferBuilder builder, int escortReinforceTimes) { builder.AddInt(24, escortReinforceTimes, 0); }
  public static void AddRedNameAttackTimes(FlatBufferBuilder builder, int redNameAttackTimes) { builder.AddInt(25, redNameAttackTimes, 0); }
  public static void AddRedNameRemovalTimes(FlatBufferBuilder builder, int redNameRemovalTimes) { builder.AddInt(26, redNameRemovalTimes, 0); }
  public static void AddGiftItemId(FlatBufferBuilder builder, StringOffset giftItemIdOffset) { builder.AddOffset(27, giftItemIdOffset.Value, 0); }
  public static void AddInvEquipCount(FlatBufferBuilder builder, int invEquipCount) { builder.AddInt(28, invEquipCount, 0); }
  public static void AddCanBlitzChallCampaign(FlatBufferBuilder builder, bool canBlitzChallCampaign) { builder.AddBool(29, canBlitzChallCampaign, false); }
  public static void AddCanBlitzAlliCampaign(FlatBufferBuilder builder, bool canBlitzAlliCampaign) { builder.AddBool(30, canBlitzAlliCampaign, false); }
  public static void AddBuyFreeVigorTimes(FlatBufferBuilder builder, int buyFreeVigorTimes) { builder.AddInt(31, buyFreeVigorTimes, 0); }
  public static void AddInfiDiscountTimes(FlatBufferBuilder builder, int infiDiscountTimes) { builder.AddInt(32, infiDiscountTimes, 0); }
  public static void AddAwakenCampDiscountTimes(FlatBufferBuilder builder, int awakenCampDiscountTimes) { builder.AddInt(33, awakenCampDiscountTimes, 0); }
  public static void AddHonorArenaIdleRewardLimit(FlatBufferBuilder builder, int honorArenaIdleRewardLimit) { builder.AddInt(34, honorArenaIdleRewardLimit, 0); }
  public static void AddAlliDonateTimes(FlatBufferBuilder builder, int alliDonateTimes) { builder.AddInt(35, alliDonateTimes, 0); }
  public static void AddFirstDonateDouble(FlatBufferBuilder builder, bool firstDonateDouble) { builder.AddBool(36, firstDonateDouble, false); }
  public static void AddDayBuyLimit(FlatBufferBuilder builder, int dayBuyLimit) { builder.AddInt(37, dayBuyLimit, 0); }
  public static void AddWeeklyBuyLimit(FlatBufferBuilder builder, int weeklyBuyLimit) { builder.AddInt(38, weeklyBuyLimit, 0); }
  public static void AddMonthlyBuyLimit(FlatBufferBuilder builder, int monthlyBuyLimit) { builder.AddInt(39, monthlyBuyLimit, 0); }
  public static void AddInfiniteChallengeResetTimes(FlatBufferBuilder builder, int infiniteChallengeResetTimes) { builder.AddInt(40, infiniteChallengeResetTimes, 0); }
  public static void AddIsFreeRevive(FlatBufferBuilder builder, bool isFreeRevive) { builder.AddBool(41, isFreeRevive, false); }
  public static void AddIsMysteryShopShow(FlatBufferBuilder builder, bool isMysteryShopShow) { builder.AddBool(42, isMysteryShopShow, false); }
  public static void AddCanBlitzTenTimes(FlatBufferBuilder builder, bool canBlitzTenTimes) { builder.AddBool(43, canBlitzTenTimes, false); }
  public static void AddExpeditionRating(FlatBufferBuilder builder, int expeditionRating) { builder.AddInt(44, expeditionRating, 0); }
  public static void AddDailyHcReward(FlatBufferBuilder builder, int dailyHcReward) { builder.AddInt(45, dailyHcReward, 0); }
  public static void AddGiftPrice(FlatBufferBuilder builder, int giftPrice) { builder.AddInt(46, giftPrice, 0); }
  public static void AddIsCanQuickRefining(FlatBufferBuilder builder, bool isCanQuickRefining) { builder.AddBool(47, isCanQuickRefining, false); }
  public static void AddEscortAndRobTimes(FlatBufferBuilder builder, int escortAndRobTimes) { builder.AddInt(48, escortAndRobTimes, 0); }
  public static void AddFreeRefiningTimes(FlatBufferBuilder builder, int freeRefiningTimes) { builder.AddInt(49, freeRefiningTimes, 0); }
  public static Offset<VipInfo> EndVipInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<VipInfo>(o);
  }
};

public sealed class ConditionVip : Table {
  public static ConditionVip GetRootAsConditionVip(ByteBuffer _bb) { return GetRootAsConditionVip(_bb, new ConditionVip()); }
  public static ConditionVip GetRootAsConditionVip(ByteBuffer _bb, ConditionVip obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ConditionVip __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string _id { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? Get_idBytes() { return __vector_as_arraysegment(4); }
  public string Name { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(6); }
  public bool Enabled { get { int o = __offset(8); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int Priority { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public GM.DataCache.DateCondition GetDateConditions(int j) { return GetDateConditions(new GM.DataCache.DateCondition(), j); }
  public GM.DataCache.DateCondition GetDateConditions(GM.DataCache.DateCondition obj, int j) { int o = __offset(12); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int DateConditionsLength { get { int o = __offset(12); return o != 0 ? __vector_len(o) : 0; } }
  public GM.DataCache.UserCondition GetUserConditions(int j) { return GetUserConditions(new GM.DataCache.UserCondition(), j); }
  public GM.DataCache.UserCondition GetUserConditions(GM.DataCache.UserCondition obj, int j) { int o = __offset(14); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int UserConditionsLength { get { int o = __offset(14); return o != 0 ? __vector_len(o) : 0; } }
  public GM.DataCache.Options Options { get { return GetOptions(new GM.DataCache.Options()); } }
  public GM.DataCache.Options GetOptions(GM.DataCache.Options obj) { int o = __offset(16); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public VipInfo GetVip(int j) { return GetVip(new VipInfo(), j); }
  public VipInfo GetVip(VipInfo obj, int j) { int o = __offset(18); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int VipLength { get { int o = __offset(18); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<ConditionVip> CreateConditionVip(FlatBufferBuilder builder,
      StringOffset _idOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      bool enabled = false,
      int priority = 0,
      VectorOffset date_conditionsOffset = default(VectorOffset),
      VectorOffset user_conditionsOffset = default(VectorOffset),
      Offset<GM.DataCache.Options> optionsOffset = default(Offset<GM.DataCache.Options>),
      VectorOffset vipOffset = default(VectorOffset)) {
    builder.StartObject(8);
    ConditionVip.AddVip(builder, vipOffset);
    ConditionVip.AddOptions(builder, optionsOffset);
    ConditionVip.AddUserConditions(builder, user_conditionsOffset);
    ConditionVip.AddDateConditions(builder, date_conditionsOffset);
    ConditionVip.AddPriority(builder, priority);
    ConditionVip.AddName(builder, nameOffset);
    ConditionVip.Add_id(builder, _idOffset);
    ConditionVip.AddEnabled(builder, enabled);
    return ConditionVip.EndConditionVip(builder);
  }

  public static void StartConditionVip(FlatBufferBuilder builder) { builder.StartObject(8); }
  public static void Add_id(FlatBufferBuilder builder, StringOffset IdOffset) { builder.AddOffset(0, IdOffset.Value, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(1, nameOffset.Value, 0); }
  public static void AddEnabled(FlatBufferBuilder builder, bool enabled) { builder.AddBool(2, enabled, false); }
  public static void AddPriority(FlatBufferBuilder builder, int priority) { builder.AddInt(3, priority, 0); }
  public static void AddDateConditions(FlatBufferBuilder builder, VectorOffset dateConditionsOffset) { builder.AddOffset(4, dateConditionsOffset.Value, 0); }
  public static VectorOffset CreateDateConditionsVector(FlatBufferBuilder builder, Offset<GM.DataCache.DateCondition>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartDateConditionsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddUserConditions(FlatBufferBuilder builder, VectorOffset userConditionsOffset) { builder.AddOffset(5, userConditionsOffset.Value, 0); }
  public static VectorOffset CreateUserConditionsVector(FlatBufferBuilder builder, Offset<GM.DataCache.UserCondition>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartUserConditionsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddOptions(FlatBufferBuilder builder, Offset<GM.DataCache.Options> optionsOffset) { builder.AddOffset(6, optionsOffset.Value, 0); }
  public static void AddVip(FlatBufferBuilder builder, VectorOffset vipOffset) { builder.AddOffset(7, vipOffset.Value, 0); }
  public static VectorOffset CreateVipVector(FlatBufferBuilder builder, Offset<VipInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartVipVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<ConditionVip> EndConditionVip(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ConditionVip>(o);
  }
};

public sealed class Vip : Table {
  public static Vip GetRootAsVip(ByteBuffer _bb) { return GetRootAsVip(_bb, new Vip()); }
  public static Vip GetRootAsVip(ByteBuffer _bb, Vip obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Vip __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public ConditionVip GetArray(int j) { return GetArray(new ConditionVip(), j); }
  public ConditionVip GetArray(ConditionVip obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ArrayLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<Vip> CreateVip(FlatBufferBuilder builder,
      VectorOffset arrayOffset = default(VectorOffset)) {
    builder.StartObject(1);
    Vip.AddArray(builder, arrayOffset);
    return Vip.EndVip(builder);
  }

  public static void StartVip(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddArray(FlatBufferBuilder builder, VectorOffset arrayOffset) { builder.AddOffset(0, arrayOffset.Value, 0); }
  public static VectorOffset CreateArrayVector(FlatBufferBuilder builder, Offset<ConditionVip>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartArrayVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<Vip> EndVip(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Vip>(o);
  }
  public static void FinishVipBuffer(FlatBufferBuilder builder, Offset<Vip> offset) { builder.Finish(offset.Value); }
};


}
