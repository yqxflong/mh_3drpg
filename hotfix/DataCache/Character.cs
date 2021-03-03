// automatically generated, do not modify

namespace GM.DataCache
{

using System;
using FlatBuffers;

public sealed class HeroStat : Table {
  public static HeroStat GetRootAsHeroStat(ByteBuffer _bb) { return GetRootAsHeroStat(_bb, new HeroStat()); }
  public static HeroStat GetRootAsHeroStat(ByteBuffer _bb, HeroStat obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public HeroStat __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Id { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIdBytes() { return __vector_as_arraysegment(4); }
  public string CharacterId { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetCharacterIdBytes() { return __vector_as_arraysegment(6); }
  public string Name { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(8); }
  public string Tips { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTipsBytes() { return __vector_as_arraysegment(10); }
  public int AttributeRating { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float BaseMaxHP { get { int o = __offset(14); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float BaseATK { get { int o = __offset(16); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float BaseDEF { get { int o = __offset(18); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float ChainAtk { get { int o = __offset(20); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float CritP { get { int o = __offset(22); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float CritV { get { int o = __offset(24); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float SpExtra { get { int o = __offset(26); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float SpRes { get { int o = __offset(28); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int ActiveSkill { get { int o = __offset(30); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int CommonSkill { get { int o = __offset(32); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int PassiveSkill { get { int o = __offset(34); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string HeroFetter1 { get { int o = __offset(36); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetHeroFetter1Bytes() { return __vector_as_arraysegment(36); }
  public string HeroFetter2 { get { int o = __offset(38); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetHeroFetter2Bytes() { return __vector_as_arraysegment(38); }
  public string HeroFetter3 { get { int o = __offset(40); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetHeroFetter3Bytes() { return __vector_as_arraysegment(40); }
  public int StarSkill5 { get { int o = __offset(42); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int StarSkill6 { get { int o = __offset(44); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<HeroStat> CreateHeroStat(FlatBufferBuilder builder,
      StringOffset idOffset = default(StringOffset),
      StringOffset character_idOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      StringOffset tipsOffset = default(StringOffset),
      int attribute_rating = 0,
      float base_MaxHP = 0,
      float base_ATK = 0,
      float base_DEF = 0,
      float ChainAtk = 0,
      float CritP = 0,
      float CritV = 0,
      float SpExtra = 0,
      float SpRes = 0,
      int active_skill = 0,
      int common_skill = 0,
      int passive_skill = 0,
      StringOffset hero_fetter_1Offset = default(StringOffset),
      StringOffset hero_fetter_2Offset = default(StringOffset),
      StringOffset hero_fetter_3Offset = default(StringOffset),
      int star_skill_5 = 0,
      int star_skill_6 = 0) {
    builder.StartObject(21);
    HeroStat.AddStarSkill6(builder, star_skill_6);
    HeroStat.AddStarSkill5(builder, star_skill_5);
    HeroStat.AddHeroFetter3(builder, hero_fetter_3Offset);
    HeroStat.AddHeroFetter2(builder, hero_fetter_2Offset);
    HeroStat.AddHeroFetter1(builder, hero_fetter_1Offset);
    HeroStat.AddPassiveSkill(builder, passive_skill);
    HeroStat.AddCommonSkill(builder, common_skill);
    HeroStat.AddActiveSkill(builder, active_skill);
    HeroStat.AddSpRes(builder, SpRes);
    HeroStat.AddSpExtra(builder, SpExtra);
    HeroStat.AddCritV(builder, CritV);
    HeroStat.AddCritP(builder, CritP);
    HeroStat.AddChainAtk(builder, ChainAtk);
    HeroStat.AddBaseDEF(builder, base_DEF);
    HeroStat.AddBaseATK(builder, base_ATK);
    HeroStat.AddBaseMaxHP(builder, base_MaxHP);
    HeroStat.AddAttributeRating(builder, attribute_rating);
    HeroStat.AddTips(builder, tipsOffset);
    HeroStat.AddName(builder, nameOffset);
    HeroStat.AddCharacterId(builder, character_idOffset);
    HeroStat.AddId(builder, idOffset);
    return HeroStat.EndHeroStat(builder);
  }

  public static void StartHeroStat(FlatBufferBuilder builder) { builder.StartObject(21); }
  public static void AddId(FlatBufferBuilder builder, StringOffset idOffset) { builder.AddOffset(0, idOffset.Value, 0); }
  public static void AddCharacterId(FlatBufferBuilder builder, StringOffset characterIdOffset) { builder.AddOffset(1, characterIdOffset.Value, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(2, nameOffset.Value, 0); }
  public static void AddTips(FlatBufferBuilder builder, StringOffset tipsOffset) { builder.AddOffset(3, tipsOffset.Value, 0); }
  public static void AddAttributeRating(FlatBufferBuilder builder, int attributeRating) { builder.AddInt(4, attributeRating, 0); }
  public static void AddBaseMaxHP(FlatBufferBuilder builder, float baseMaxHP) { builder.AddFloat(5, baseMaxHP, 0); }
  public static void AddBaseATK(FlatBufferBuilder builder, float baseATK) { builder.AddFloat(6, baseATK, 0); }
  public static void AddBaseDEF(FlatBufferBuilder builder, float baseDEF) { builder.AddFloat(7, baseDEF, 0); }
  public static void AddChainAtk(FlatBufferBuilder builder, float ChainAtk) { builder.AddFloat(8, ChainAtk, 0); }
  public static void AddCritP(FlatBufferBuilder builder, float CritP) { builder.AddFloat(9, CritP, 0); }
  public static void AddCritV(FlatBufferBuilder builder, float CritV) { builder.AddFloat(10, CritV, 0); }
  public static void AddSpExtra(FlatBufferBuilder builder, float SpExtra) { builder.AddFloat(11, SpExtra, 0); }
  public static void AddSpRes(FlatBufferBuilder builder, float SpRes) { builder.AddFloat(12, SpRes, 0); }
  public static void AddActiveSkill(FlatBufferBuilder builder, int activeSkill) { builder.AddInt(13, activeSkill, 0); }
  public static void AddCommonSkill(FlatBufferBuilder builder, int commonSkill) { builder.AddInt(14, commonSkill, 0); }
  public static void AddPassiveSkill(FlatBufferBuilder builder, int passiveSkill) { builder.AddInt(15, passiveSkill, 0); }
  public static void AddHeroFetter1(FlatBufferBuilder builder, StringOffset heroFetter1Offset) { builder.AddOffset(16, heroFetter1Offset.Value, 0); }
  public static void AddHeroFetter2(FlatBufferBuilder builder, StringOffset heroFetter2Offset) { builder.AddOffset(17, heroFetter2Offset.Value, 0); }
  public static void AddHeroFetter3(FlatBufferBuilder builder, StringOffset heroFetter3Offset) { builder.AddOffset(18, heroFetter3Offset.Value, 0); }
  public static void AddStarSkill5(FlatBufferBuilder builder, int starSkill5) { builder.AddInt(19, starSkill5, 0); }
  public static void AddStarSkill6(FlatBufferBuilder builder, int starSkill6) { builder.AddInt(20, starSkill6, 0); }
  public static Offset<HeroStat> EndHeroStat(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<HeroStat>(o);
  }
};

public sealed class HeroInfo : Table {
  public static HeroInfo GetRootAsHeroInfo(ByteBuffer _bb) { return GetRootAsHeroInfo(_bb, new HeroInfo()); }
  public static HeroInfo GetRootAsHeroInfo(ByteBuffer _bb, HeroInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public HeroInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Id { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIdBytes() { return __vector_as_arraysegment(4); }
  public string Name { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(6); }
  public string Title { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTitleBytes() { return __vector_as_arraysegment(8); }
  public string Memo { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMemoBytes() { return __vector_as_arraysegment(10); }
  public int CharType { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Tendency { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Race { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int LifetimeTurns { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string EnterEffect { get { int o = __offset(20); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetEnterEffectBytes() { return __vector_as_arraysegment(20); }
  public string LeaveEffect { get { int o = __offset(22); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetLeaveEffectBytes() { return __vector_as_arraysegment(22); }
  public int MaxHPBarType { get { int o = __offset(24); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int HideMaxHPBar { get { int o = __offset(26); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int MaxHPBarSize { get { int o = __offset(28); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Portrait { get { int o = __offset(30); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetPortraitBytes() { return __vector_as_arraysegment(30); }
  public string Icon { get { int o = __offset(32); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIconBytes() { return __vector_as_arraysegment(32); }
  public string ModelName { get { int o = __offset(34); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetModelNameBytes() { return __vector_as_arraysegment(34); }
  public string ModelId { get { int o = __offset(36); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetModelIdBytes() { return __vector_as_arraysegment(36); }
  public string Chat { get { int o = __offset(38); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetChatBytes() { return __vector_as_arraysegment(38); }
  public int Force { get { int o = __offset(40); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Gender { get { int o = __offset(42); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SummonShard { get { int o = __offset(44); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int InitStar { get { int o = __offset(46); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ExpType { get { int o = __offset(48); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string DropChickId1 { get { int o = __offset(50); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDropChickId1Bytes() { return __vector_as_arraysegment(50); }
  public string DropChickId2 { get { int o = __offset(52); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDropChickId2Bytes() { return __vector_as_arraysegment(52); }
  public string DropChickId3 { get { int o = __offset(54); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDropChickId3Bytes() { return __vector_as_arraysegment(54); }
  public string RoleProfileText { get { int o = __offset(56); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRoleProfileTextBytes() { return __vector_as_arraysegment(56); }
  public string RoleBgInfo { get { int o = __offset(58); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRoleBgInfoBytes() { return __vector_as_arraysegment(58); }
  public string RoleGrade { get { int o = __offset(60); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRoleGradeBytes() { return __vector_as_arraysegment(60); }
  public string RoleProfile { get { int o = __offset(62); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRoleProfileBytes() { return __vector_as_arraysegment(62); }
  public int Draw { get { int o = __offset(64); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string LobbyCamera { get { int o = __offset(66); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetLobbyCameraBytes() { return __vector_as_arraysegment(66); }
  public int ShowInClash { get { int o = __offset(68); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int CultivateGift { get { int o = __offset(70); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string RoleProfileIcon { get { int o = __offset(72); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRoleProfileIconBytes() { return __vector_as_arraysegment(72); }
  public int Reward { get { int o = __offset(74); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Skin { get { int o = __offset(76); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetSkinBytes() { return __vector_as_arraysegment(76); }
  public int SuitAttributeId1 { get { int o = __offset(78); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string SuitAttributeId2 { get { int o = __offset(80); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetSuitAttributeId2Bytes() { return __vector_as_arraysegment(80); }
  public int ShowInWiki { get { int o = __offset(82); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int IsNew { get { int o = __offset(84); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<HeroInfo> CreateHeroInfo(FlatBufferBuilder builder,
      StringOffset idOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      StringOffset titleOffset = default(StringOffset),
      StringOffset memoOffset = default(StringOffset),
      int CharType = 0,
      int tendency = 0,
      int race = 0,
      int lifetime_turns = 0,
      StringOffset enter_effectOffset = default(StringOffset),
      StringOffset leave_effectOffset = default(StringOffset),
      int MaxHP_bar_type = 0,
      int hide_MaxHP_bar = 0,
      int MaxHP_bar_size = 0,
      StringOffset portraitOffset = default(StringOffset),
      StringOffset iconOffset = default(StringOffset),
      StringOffset model_nameOffset = default(StringOffset),
      StringOffset model_idOffset = default(StringOffset),
      StringOffset chatOffset = default(StringOffset),
      int force = 0,
      int gender = 0,
      int summon_shard = 0,
      int init_star = 0,
      int exp_type = 0,
      StringOffset drop_Chick_Id_1Offset = default(StringOffset),
      StringOffset drop_Chick_Id_2Offset = default(StringOffset),
      StringOffset drop_Chick_Id_3Offset = default(StringOffset),
      StringOffset role_profile_textOffset = default(StringOffset),
      StringOffset role_bg_infoOffset = default(StringOffset),
      StringOffset role_gradeOffset = default(StringOffset),
      StringOffset role_profileOffset = default(StringOffset),
      int draw = 0,
      StringOffset lobby_cameraOffset = default(StringOffset),
      int show_in_clash = 0,
      int cultivate_gift = 0,
      StringOffset role_profile_iconOffset = default(StringOffset),
      int reward = 0,
      StringOffset skinOffset = default(StringOffset),
      int suit_attribute_Id_1 = 0,
      StringOffset suit_attribute_Id_2Offset = default(StringOffset),
      int show_in_wiki = 0,
      int is_new = 0) {
    builder.StartObject(41);
    HeroInfo.AddIsNew(builder, is_new);
    HeroInfo.AddShowInWiki(builder, show_in_wiki);
    HeroInfo.AddSuitAttributeId2(builder, suit_attribute_Id_2Offset);
    HeroInfo.AddSuitAttributeId1(builder, suit_attribute_Id_1);
    HeroInfo.AddSkin(builder, skinOffset);
    HeroInfo.AddReward(builder, reward);
    HeroInfo.AddRoleProfileIcon(builder, role_profile_iconOffset);
    HeroInfo.AddCultivateGift(builder, cultivate_gift);
    HeroInfo.AddShowInClash(builder, show_in_clash);
    HeroInfo.AddLobbyCamera(builder, lobby_cameraOffset);
    HeroInfo.AddDraw(builder, draw);
    HeroInfo.AddRoleProfile(builder, role_profileOffset);
    HeroInfo.AddRoleGrade(builder, role_gradeOffset);
    HeroInfo.AddRoleBgInfo(builder, role_bg_infoOffset);
    HeroInfo.AddRoleProfileText(builder, role_profile_textOffset);
    HeroInfo.AddDropChickId3(builder, drop_Chick_Id_3Offset);
    HeroInfo.AddDropChickId2(builder, drop_Chick_Id_2Offset);
    HeroInfo.AddDropChickId1(builder, drop_Chick_Id_1Offset);
    HeroInfo.AddExpType(builder, exp_type);
    HeroInfo.AddInitStar(builder, init_star);
    HeroInfo.AddSummonShard(builder, summon_shard);
    HeroInfo.AddGender(builder, gender);
    HeroInfo.AddForce(builder, force);
    HeroInfo.AddChat(builder, chatOffset);
    HeroInfo.AddModelId(builder, model_idOffset);
    HeroInfo.AddModelName(builder, model_nameOffset);
    HeroInfo.AddIcon(builder, iconOffset);
    HeroInfo.AddPortrait(builder, portraitOffset);
    HeroInfo.AddMaxHPBarSize(builder, MaxHP_bar_size);
    HeroInfo.AddHideMaxHPBar(builder, hide_MaxHP_bar);
    HeroInfo.AddMaxHPBarType(builder, MaxHP_bar_type);
    HeroInfo.AddLeaveEffect(builder, leave_effectOffset);
    HeroInfo.AddEnterEffect(builder, enter_effectOffset);
    HeroInfo.AddLifetimeTurns(builder, lifetime_turns);
    HeroInfo.AddRace(builder, race);
    HeroInfo.AddTendency(builder, tendency);
    HeroInfo.AddCharType(builder, CharType);
    HeroInfo.AddMemo(builder, memoOffset);
    HeroInfo.AddTitle(builder, titleOffset);
    HeroInfo.AddName(builder, nameOffset);
    HeroInfo.AddId(builder, idOffset);
    return HeroInfo.EndHeroInfo(builder);
  }

  public static void StartHeroInfo(FlatBufferBuilder builder) { builder.StartObject(41); }
  public static void AddId(FlatBufferBuilder builder, StringOffset idOffset) { builder.AddOffset(0, idOffset.Value, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(1, nameOffset.Value, 0); }
  public static void AddTitle(FlatBufferBuilder builder, StringOffset titleOffset) { builder.AddOffset(2, titleOffset.Value, 0); }
  public static void AddMemo(FlatBufferBuilder builder, StringOffset memoOffset) { builder.AddOffset(3, memoOffset.Value, 0); }
  public static void AddCharType(FlatBufferBuilder builder, int CharType) { builder.AddInt(4, CharType, 0); }
  public static void AddTendency(FlatBufferBuilder builder, int tendency) { builder.AddInt(5, tendency, 0); }
  public static void AddRace(FlatBufferBuilder builder, int race) { builder.AddInt(6, race, 0); }
  public static void AddLifetimeTurns(FlatBufferBuilder builder, int lifetimeTurns) { builder.AddInt(7, lifetimeTurns, 0); }
  public static void AddEnterEffect(FlatBufferBuilder builder, StringOffset enterEffectOffset) { builder.AddOffset(8, enterEffectOffset.Value, 0); }
  public static void AddLeaveEffect(FlatBufferBuilder builder, StringOffset leaveEffectOffset) { builder.AddOffset(9, leaveEffectOffset.Value, 0); }
  public static void AddMaxHPBarType(FlatBufferBuilder builder, int MaxHPBarType) { builder.AddInt(10, MaxHPBarType, 0); }
  public static void AddHideMaxHPBar(FlatBufferBuilder builder, int hideMaxHPBar) { builder.AddInt(11, hideMaxHPBar, 0); }
  public static void AddMaxHPBarSize(FlatBufferBuilder builder, int MaxHPBarSize) { builder.AddInt(12, MaxHPBarSize, 0); }
  public static void AddPortrait(FlatBufferBuilder builder, StringOffset portraitOffset) { builder.AddOffset(13, portraitOffset.Value, 0); }
  public static void AddIcon(FlatBufferBuilder builder, StringOffset iconOffset) { builder.AddOffset(14, iconOffset.Value, 0); }
  public static void AddModelName(FlatBufferBuilder builder, StringOffset modelNameOffset) { builder.AddOffset(15, modelNameOffset.Value, 0); }
  public static void AddModelId(FlatBufferBuilder builder, StringOffset modelIdOffset) { builder.AddOffset(16, modelIdOffset.Value, 0); }
  public static void AddChat(FlatBufferBuilder builder, StringOffset chatOffset) { builder.AddOffset(17, chatOffset.Value, 0); }
  public static void AddForce(FlatBufferBuilder builder, int force) { builder.AddInt(18, force, 0); }
  public static void AddGender(FlatBufferBuilder builder, int gender) { builder.AddInt(19, gender, 0); }
  public static void AddSummonShard(FlatBufferBuilder builder, int summonShard) { builder.AddInt(20, summonShard, 0); }
  public static void AddInitStar(FlatBufferBuilder builder, int initStar) { builder.AddInt(21, initStar, 0); }
  public static void AddExpType(FlatBufferBuilder builder, int expType) { builder.AddInt(22, expType, 0); }
  public static void AddDropChickId1(FlatBufferBuilder builder, StringOffset dropChickId1Offset) { builder.AddOffset(23, dropChickId1Offset.Value, 0); }
  public static void AddDropChickId2(FlatBufferBuilder builder, StringOffset dropChickId2Offset) { builder.AddOffset(24, dropChickId2Offset.Value, 0); }
  public static void AddDropChickId3(FlatBufferBuilder builder, StringOffset dropChickId3Offset) { builder.AddOffset(25, dropChickId3Offset.Value, 0); }
  public static void AddRoleProfileText(FlatBufferBuilder builder, StringOffset roleProfileTextOffset) { builder.AddOffset(26, roleProfileTextOffset.Value, 0); }
  public static void AddRoleBgInfo(FlatBufferBuilder builder, StringOffset roleBgInfoOffset) { builder.AddOffset(27, roleBgInfoOffset.Value, 0); }
  public static void AddRoleGrade(FlatBufferBuilder builder, StringOffset roleGradeOffset) { builder.AddOffset(28, roleGradeOffset.Value, 0); }
  public static void AddRoleProfile(FlatBufferBuilder builder, StringOffset roleProfileOffset) { builder.AddOffset(29, roleProfileOffset.Value, 0); }
  public static void AddDraw(FlatBufferBuilder builder, int draw) { builder.AddInt(30, draw, 0); }
  public static void AddLobbyCamera(FlatBufferBuilder builder, StringOffset lobbyCameraOffset) { builder.AddOffset(31, lobbyCameraOffset.Value, 0); }
  public static void AddShowInClash(FlatBufferBuilder builder, int showInClash) { builder.AddInt(32, showInClash, 0); }
  public static void AddCultivateGift(FlatBufferBuilder builder, int cultivateGift) { builder.AddInt(33, cultivateGift, 0); }
  public static void AddRoleProfileIcon(FlatBufferBuilder builder, StringOffset roleProfileIconOffset) { builder.AddOffset(34, roleProfileIconOffset.Value, 0); }
  public static void AddReward(FlatBufferBuilder builder, int reward) { builder.AddInt(35, reward, 0); }
  public static void AddSkin(FlatBufferBuilder builder, StringOffset skinOffset) { builder.AddOffset(36, skinOffset.Value, 0); }
  public static void AddSuitAttributeId1(FlatBufferBuilder builder, int suitAttributeId1) { builder.AddInt(37, suitAttributeId1, 0); }
  public static void AddSuitAttributeId2(FlatBufferBuilder builder, StringOffset suitAttributeId2Offset) { builder.AddOffset(38, suitAttributeId2Offset.Value, 0); }
  public static void AddShowInWiki(FlatBufferBuilder builder, int showInWiki) { builder.AddInt(39, showInWiki, 0); }
  public static void AddIsNew(FlatBufferBuilder builder, int isNew) { builder.AddInt(40, isNew, 0); }
  public static Offset<HeroInfo> EndHeroInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<HeroInfo>(o);
  }
};

public sealed class Monster : Table {
  public static Monster GetRootAsMonster(ByteBuffer _bb) { return GetRootAsMonster(_bb, new Monster()); }
  public static Monster GetRootAsMonster(ByteBuffer _bb, Monster obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Monster __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Id { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIdBytes() { return __vector_as_arraysegment(4); }
  public string CharacterId { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetCharacterIdBytes() { return __vector_as_arraysegment(6); }
  public string Name { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(8); }
  public int Tips { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float BaseMaxHP { get { int o = __offset(12); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float BaseATK { get { int o = __offset(14); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float BaseDEF { get { int o = __offset(16); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float ChainAtk { get { int o = __offset(18); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float CritP { get { int o = __offset(20); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float CritV { get { int o = __offset(22); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float SpExtra { get { int o = __offset(24); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float SpRes { get { int o = __offset(26); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float IncMaxHP { get { int o = __offset(28); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float IncATK { get { int o = __offset(30); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float IncDEF { get { int o = __offset(32); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int ActiveSkill1 { get { int o = __offset(34); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ActiveSkill2 { get { int o = __offset(36); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ActiveSkill3 { get { int o = __offset(38); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int CommonSkill { get { int o = __offset(40); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int PassiveSkill1 { get { int o = __offset(42); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int PassiveSkill2 { get { int o = __offset(44); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int PassiveSkill3 { get { int o = __offset(46); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int MonsterType { get { int o = __offset(48); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int HpNumber { get { int o = __offset(50); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float ScaleMultiple { get { int o = __offset(52); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int Level { get { int o = __offset(54); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Star { get { int o = __offset(56); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Upgrade { get { int o = __offset(58); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float Speed { get { int o = __offset(60); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public string SpawnCamera { get { int o = __offset(62); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetSpawnCameraBytes() { return __vector_as_arraysegment(62); }

  public static Offset<Monster> CreateMonster(FlatBufferBuilder builder,
      StringOffset idOffset = default(StringOffset),
      StringOffset character_idOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      int tips = 0,
      float base_MaxHP = 0,
      float base_ATK = 0,
      float base_DEF = 0,
      float ChainAtk = 0,
      float CritP = 0,
      float CritV = 0,
      float SpExtra = 0,
      float SpRes = 0,
      float inc_MaxHP = 0,
      float inc_ATK = 0,
      float inc_DEF = 0,
      int active_skill_1 = 0,
      int active_skill_2 = 0,
      int active_skill_3 = 0,
      int common_skill = 0,
      int passive_skill_1 = 0,
      int passive_skill_2 = 0,
      int passive_skill_3 = 0,
      int monster_type = 0,
      int hp_number = 0,
      float scale_multiple = 0,
      int level = 0,
      int star = 0,
      int upgrade = 0,
      float speed = 0,
      StringOffset spawn_cameraOffset = default(StringOffset)) {
    builder.StartObject(30);
    Monster.AddSpawnCamera(builder, spawn_cameraOffset);
    Monster.AddSpeed(builder, speed);
    Monster.AddUpgrade(builder, upgrade);
    Monster.AddStar(builder, star);
    Monster.AddLevel(builder, level);
    Monster.AddScaleMultiple(builder, scale_multiple);
    Monster.AddHpNumber(builder, hp_number);
    Monster.AddMonsterType(builder, monster_type);
    Monster.AddPassiveSkill3(builder, passive_skill_3);
    Monster.AddPassiveSkill2(builder, passive_skill_2);
    Monster.AddPassiveSkill1(builder, passive_skill_1);
    Monster.AddCommonSkill(builder, common_skill);
    Monster.AddActiveSkill3(builder, active_skill_3);
    Monster.AddActiveSkill2(builder, active_skill_2);
    Monster.AddActiveSkill1(builder, active_skill_1);
    Monster.AddIncDEF(builder, inc_DEF);
    Monster.AddIncATK(builder, inc_ATK);
    Monster.AddIncMaxHP(builder, inc_MaxHP);
    Monster.AddSpRes(builder, SpRes);
    Monster.AddSpExtra(builder, SpExtra);
    Monster.AddCritV(builder, CritV);
    Monster.AddCritP(builder, CritP);
    Monster.AddChainAtk(builder, ChainAtk);
    Monster.AddBaseDEF(builder, base_DEF);
    Monster.AddBaseATK(builder, base_ATK);
    Monster.AddBaseMaxHP(builder, base_MaxHP);
    Monster.AddTips(builder, tips);
    Monster.AddName(builder, nameOffset);
    Monster.AddCharacterId(builder, character_idOffset);
    Monster.AddId(builder, idOffset);
    return Monster.EndMonster(builder);
  }

  public static void StartMonster(FlatBufferBuilder builder) { builder.StartObject(30); }
  public static void AddId(FlatBufferBuilder builder, StringOffset idOffset) { builder.AddOffset(0, idOffset.Value, 0); }
  public static void AddCharacterId(FlatBufferBuilder builder, StringOffset characterIdOffset) { builder.AddOffset(1, characterIdOffset.Value, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(2, nameOffset.Value, 0); }
  public static void AddTips(FlatBufferBuilder builder, int tips) { builder.AddInt(3, tips, 0); }
  public static void AddBaseMaxHP(FlatBufferBuilder builder, float baseMaxHP) { builder.AddFloat(4, baseMaxHP, 0); }
  public static void AddBaseATK(FlatBufferBuilder builder, float baseATK) { builder.AddFloat(5, baseATK, 0); }
  public static void AddBaseDEF(FlatBufferBuilder builder, float baseDEF) { builder.AddFloat(6, baseDEF, 0); }
  public static void AddChainAtk(FlatBufferBuilder builder, float ChainAtk) { builder.AddFloat(7, ChainAtk, 0); }
  public static void AddCritP(FlatBufferBuilder builder, float CritP) { builder.AddFloat(8, CritP, 0); }
  public static void AddCritV(FlatBufferBuilder builder, float CritV) { builder.AddFloat(9, CritV, 0); }
  public static void AddSpExtra(FlatBufferBuilder builder, float SpExtra) { builder.AddFloat(10, SpExtra, 0); }
  public static void AddSpRes(FlatBufferBuilder builder, float SpRes) { builder.AddFloat(11, SpRes, 0); }
  public static void AddIncMaxHP(FlatBufferBuilder builder, float incMaxHP) { builder.AddFloat(12, incMaxHP, 0); }
  public static void AddIncATK(FlatBufferBuilder builder, float incATK) { builder.AddFloat(13, incATK, 0); }
  public static void AddIncDEF(FlatBufferBuilder builder, float incDEF) { builder.AddFloat(14, incDEF, 0); }
  public static void AddActiveSkill1(FlatBufferBuilder builder, int activeSkill1) { builder.AddInt(15, activeSkill1, 0); }
  public static void AddActiveSkill2(FlatBufferBuilder builder, int activeSkill2) { builder.AddInt(16, activeSkill2, 0); }
  public static void AddActiveSkill3(FlatBufferBuilder builder, int activeSkill3) { builder.AddInt(17, activeSkill3, 0); }
  public static void AddCommonSkill(FlatBufferBuilder builder, int commonSkill) { builder.AddInt(18, commonSkill, 0); }
  public static void AddPassiveSkill1(FlatBufferBuilder builder, int passiveSkill1) { builder.AddInt(19, passiveSkill1, 0); }
  public static void AddPassiveSkill2(FlatBufferBuilder builder, int passiveSkill2) { builder.AddInt(20, passiveSkill2, 0); }
  public static void AddPassiveSkill3(FlatBufferBuilder builder, int passiveSkill3) { builder.AddInt(21, passiveSkill3, 0); }
  public static void AddMonsterType(FlatBufferBuilder builder, int monsterType) { builder.AddInt(22, monsterType, 0); }
  public static void AddHpNumber(FlatBufferBuilder builder, int hpNumber) { builder.AddInt(23, hpNumber, 0); }
  public static void AddScaleMultiple(FlatBufferBuilder builder, float scaleMultiple) { builder.AddFloat(24, scaleMultiple, 0); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(25, level, 0); }
  public static void AddStar(FlatBufferBuilder builder, int star) { builder.AddInt(26, star, 0); }
  public static void AddUpgrade(FlatBufferBuilder builder, int upgrade) { builder.AddInt(27, upgrade, 0); }
  public static void AddSpeed(FlatBufferBuilder builder, float speed) { builder.AddFloat(28, speed, 0); }
  public static void AddSpawnCamera(FlatBufferBuilder builder, StringOffset spawnCameraOffset) { builder.AddOffset(29, spawnCameraOffset.Value, 0); }
  public static Offset<Monster> EndMonster(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Monster>(o);
  }
};

public sealed class Star : Table {
  public static Star GetRootAsStar(ByteBuffer _bb) { return GetRootAsStar(_bb, new Star()); }
  public static Star GetRootAsStar(ByteBuffer _bb, Star obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Star __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Id { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIdBytes() { return __vector_as_arraysegment(4); }
  public int StarLevel { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int HolePosition { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int CostShard { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Attr { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetAttrBytes() { return __vector_as_arraysegment(12); }
  public float Param { get { int o = __offset(14); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int Level { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<Star> CreateStar(FlatBufferBuilder builder,
      StringOffset idOffset = default(StringOffset),
      int star_level = 0,
      int hole_position = 0,
      int cost_shard = 0,
      StringOffset attrOffset = default(StringOffset),
      float param = 0,
      int level = 0) {
    builder.StartObject(7);
    Star.AddLevel(builder, level);
    Star.AddParam(builder, param);
    Star.AddAttr(builder, attrOffset);
    Star.AddCostShard(builder, cost_shard);
    Star.AddHolePosition(builder, hole_position);
    Star.AddStarLevel(builder, star_level);
    Star.AddId(builder, idOffset);
    return Star.EndStar(builder);
  }

  public static void StartStar(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddId(FlatBufferBuilder builder, StringOffset idOffset) { builder.AddOffset(0, idOffset.Value, 0); }
  public static void AddStarLevel(FlatBufferBuilder builder, int starLevel) { builder.AddInt(1, starLevel, 0); }
  public static void AddHolePosition(FlatBufferBuilder builder, int holePosition) { builder.AddInt(2, holePosition, 0); }
  public static void AddCostShard(FlatBufferBuilder builder, int costShard) { builder.AddInt(3, costShard, 0); }
  public static void AddAttr(FlatBufferBuilder builder, StringOffset attrOffset) { builder.AddOffset(4, attrOffset.Value, 0); }
  public static void AddParam(FlatBufferBuilder builder, float param) { builder.AddFloat(5, param, 0); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(6, level, 0); }
  public static Offset<Star> EndStar(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Star>(o);
  }
};

public sealed class StarUp : Table {
  public static StarUp GetRootAsStarUp(ByteBuffer _bb) { return GetRootAsStarUp(_bb, new StarUp()); }
  public static StarUp GetRootAsStarUp(ByteBuffer _bb, StarUp obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public StarUp __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Id { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIdBytes() { return __vector_as_arraysegment(4); }
  public string Name { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(6); }
  public int StarId { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float IncMaxHP { get { int o = __offset(10); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float IncATK { get { int o = __offset(12); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float IncDEF { get { int o = __offset(14); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<StarUp> CreateStarUp(FlatBufferBuilder builder,
      StringOffset idOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      int star_id = 0,
      float inc_MaxHP = 0,
      float inc_ATK = 0,
      float inc_DEF = 0) {
    builder.StartObject(6);
    StarUp.AddIncDEF(builder, inc_DEF);
    StarUp.AddIncATK(builder, inc_ATK);
    StarUp.AddIncMaxHP(builder, inc_MaxHP);
    StarUp.AddStarId(builder, star_id);
    StarUp.AddName(builder, nameOffset);
    StarUp.AddId(builder, idOffset);
    return StarUp.EndStarUp(builder);
  }

  public static void StartStarUp(FlatBufferBuilder builder) { builder.StartObject(6); }
  public static void AddId(FlatBufferBuilder builder, StringOffset idOffset) { builder.AddOffset(0, idOffset.Value, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(1, nameOffset.Value, 0); }
  public static void AddStarId(FlatBufferBuilder builder, int starId) { builder.AddInt(2, starId, 0); }
  public static void AddIncMaxHP(FlatBufferBuilder builder, float incMaxHP) { builder.AddFloat(3, incMaxHP, 0); }
  public static void AddIncATK(FlatBufferBuilder builder, float incATK) { builder.AddFloat(4, incATK, 0); }
  public static void AddIncDEF(FlatBufferBuilder builder, float incDEF) { builder.AddFloat(5, incDEF, 0); }
  public static Offset<StarUp> EndStarUp(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<StarUp>(o);
  }
};

public sealed class Upgrade : Table {
  public static Upgrade GetRootAsUpgrade(ByteBuffer _bb) { return GetRootAsUpgrade(_bb, new Upgrade()); }
  public static Upgrade GetRootAsUpgrade(ByteBuffer _bb, Upgrade obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Upgrade __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Id { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIdBytes() { return __vector_as_arraysegment(4); }
  public string Name { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(6); }
  public int UpgradeId { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float IncMaxHP { get { int o = __offset(10); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float IncATK { get { int o = __offset(12); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float IncDEF { get { int o = __offset(14); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int LevelLimit { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int CharType { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Material1 { get { int o = __offset(20); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMaterial1Bytes() { return __vector_as_arraysegment(20); }
  public int Quantity1 { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Material2 { get { int o = __offset(24); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMaterial2Bytes() { return __vector_as_arraysegment(24); }
  public int Quantity2 { get { int o = __offset(26); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Material3 { get { int o = __offset(28); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMaterial3Bytes() { return __vector_as_arraysegment(28); }
  public int Quantity3 { get { int o = __offset(30); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Material4 { get { int o = __offset(32); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMaterial4Bytes() { return __vector_as_arraysegment(32); }
  public int Quantity4 { get { int o = __offset(34); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Material5 { get { int o = __offset(36); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMaterial5Bytes() { return __vector_as_arraysegment(36); }
  public int Quantity5 { get { int o = __offset(38); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Material6 { get { int o = __offset(40); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMaterial6Bytes() { return __vector_as_arraysegment(40); }
  public int Quantity6 { get { int o = __offset(42); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float IncSpeed { get { int o = __offset(44); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int Level { get { int o = __offset(46); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Quality { get { int o = __offset(48); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Gold { get { int o = __offset(50); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Hc { get { int o = __offset(52); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<Upgrade> CreateUpgrade(FlatBufferBuilder builder,
      StringOffset idOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      int upgrade_id = 0,
      float inc_MaxHP = 0,
      float inc_ATK = 0,
      float inc_DEF = 0,
      int level_limit = 0,
      int char_type = 0,
      StringOffset material_1Offset = default(StringOffset),
      int quantity_1 = 0,
      StringOffset material_2Offset = default(StringOffset),
      int quantity_2 = 0,
      StringOffset material_3Offset = default(StringOffset),
      int quantity_3 = 0,
      StringOffset material_4Offset = default(StringOffset),
      int quantity_4 = 0,
      StringOffset material_5Offset = default(StringOffset),
      int quantity_5 = 0,
      StringOffset material_6Offset = default(StringOffset),
      int quantity_6 = 0,
      float inc_speed = 0,
      int level = 0,
      int quality = 0,
      int gold = 0,
      int hc = 0) {
    builder.StartObject(25);
    Upgrade.AddHc(builder, hc);
    Upgrade.AddGold(builder, gold);
    Upgrade.AddQuality(builder, quality);
    Upgrade.AddLevel(builder, level);
    Upgrade.AddIncSpeed(builder, inc_speed);
    Upgrade.AddQuantity6(builder, quantity_6);
    Upgrade.AddMaterial6(builder, material_6Offset);
    Upgrade.AddQuantity5(builder, quantity_5);
    Upgrade.AddMaterial5(builder, material_5Offset);
    Upgrade.AddQuantity4(builder, quantity_4);
    Upgrade.AddMaterial4(builder, material_4Offset);
    Upgrade.AddQuantity3(builder, quantity_3);
    Upgrade.AddMaterial3(builder, material_3Offset);
    Upgrade.AddQuantity2(builder, quantity_2);
    Upgrade.AddMaterial2(builder, material_2Offset);
    Upgrade.AddQuantity1(builder, quantity_1);
    Upgrade.AddMaterial1(builder, material_1Offset);
    Upgrade.AddCharType(builder, char_type);
    Upgrade.AddLevelLimit(builder, level_limit);
    Upgrade.AddIncDEF(builder, inc_DEF);
    Upgrade.AddIncATK(builder, inc_ATK);
    Upgrade.AddIncMaxHP(builder, inc_MaxHP);
    Upgrade.AddUpgradeId(builder, upgrade_id);
    Upgrade.AddName(builder, nameOffset);
    Upgrade.AddId(builder, idOffset);
    return Upgrade.EndUpgrade(builder);
  }

  public static void StartUpgrade(FlatBufferBuilder builder) { builder.StartObject(25); }
  public static void AddId(FlatBufferBuilder builder, StringOffset idOffset) { builder.AddOffset(0, idOffset.Value, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(1, nameOffset.Value, 0); }
  public static void AddUpgradeId(FlatBufferBuilder builder, int upgradeId) { builder.AddInt(2, upgradeId, 0); }
  public static void AddIncMaxHP(FlatBufferBuilder builder, float incMaxHP) { builder.AddFloat(3, incMaxHP, 0); }
  public static void AddIncATK(FlatBufferBuilder builder, float incATK) { builder.AddFloat(4, incATK, 0); }
  public static void AddIncDEF(FlatBufferBuilder builder, float incDEF) { builder.AddFloat(5, incDEF, 0); }
  public static void AddLevelLimit(FlatBufferBuilder builder, int levelLimit) { builder.AddInt(6, levelLimit, 0); }
  public static void AddCharType(FlatBufferBuilder builder, int charType) { builder.AddInt(7, charType, 0); }
  public static void AddMaterial1(FlatBufferBuilder builder, StringOffset material1Offset) { builder.AddOffset(8, material1Offset.Value, 0); }
  public static void AddQuantity1(FlatBufferBuilder builder, int quantity1) { builder.AddInt(9, quantity1, 0); }
  public static void AddMaterial2(FlatBufferBuilder builder, StringOffset material2Offset) { builder.AddOffset(10, material2Offset.Value, 0); }
  public static void AddQuantity2(FlatBufferBuilder builder, int quantity2) { builder.AddInt(11, quantity2, 0); }
  public static void AddMaterial3(FlatBufferBuilder builder, StringOffset material3Offset) { builder.AddOffset(12, material3Offset.Value, 0); }
  public static void AddQuantity3(FlatBufferBuilder builder, int quantity3) { builder.AddInt(13, quantity3, 0); }
  public static void AddMaterial4(FlatBufferBuilder builder, StringOffset material4Offset) { builder.AddOffset(14, material4Offset.Value, 0); }
  public static void AddQuantity4(FlatBufferBuilder builder, int quantity4) { builder.AddInt(15, quantity4, 0); }
  public static void AddMaterial5(FlatBufferBuilder builder, StringOffset material5Offset) { builder.AddOffset(16, material5Offset.Value, 0); }
  public static void AddQuantity5(FlatBufferBuilder builder, int quantity5) { builder.AddInt(17, quantity5, 0); }
  public static void AddMaterial6(FlatBufferBuilder builder, StringOffset material6Offset) { builder.AddOffset(18, material6Offset.Value, 0); }
  public static void AddQuantity6(FlatBufferBuilder builder, int quantity6) { builder.AddInt(19, quantity6, 0); }
  public static void AddIncSpeed(FlatBufferBuilder builder, float incSpeed) { builder.AddFloat(20, incSpeed, 0); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(21, level, 0); }
  public static void AddQuality(FlatBufferBuilder builder, int quality) { builder.AddInt(22, quality, 0); }
  public static void AddGold(FlatBufferBuilder builder, int gold) { builder.AddInt(23, gold, 0); }
  public static void AddHc(FlatBufferBuilder builder, int hc) { builder.AddInt(24, hc, 0); }
  public static Offset<Upgrade> EndUpgrade(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Upgrade>(o);
  }
};

public sealed class SkillLevel : Table {
  public static SkillLevel GetRootAsSkillLevel(ByteBuffer _bb) { return GetRootAsSkillLevel(_bb, new SkillLevel()); }
  public static SkillLevel GetRootAsSkillLevel(ByteBuffer _bb, SkillLevel obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public SkillLevel __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Level { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int StarLimit { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Exp { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<SkillLevel> CreateSkillLevel(FlatBufferBuilder builder,
      int level = 0,
      int star_limit = 0,
      int exp = 0) {
    builder.StartObject(3);
    SkillLevel.AddExp(builder, exp);
    SkillLevel.AddStarLimit(builder, star_limit);
    SkillLevel.AddLevel(builder, level);
    return SkillLevel.EndSkillLevel(builder);
  }

  public static void StartSkillLevel(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(0, level, 0); }
  public static void AddStarLimit(FlatBufferBuilder builder, int starLimit) { builder.AddInt(1, starLimit, 0); }
  public static void AddExp(FlatBufferBuilder builder, int exp) { builder.AddInt(2, exp, 0); }
  public static Offset<SkillLevel> EndSkillLevel(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<SkillLevel>(o);
  }
};

public sealed class Evolution : Table {
  public static Evolution GetRootAsEvolution(ByteBuffer _bb) { return GetRootAsEvolution(_bb, new Evolution()); }
  public static Evolution GetRootAsEvolution(ByteBuffer _bb, Evolution obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Evolution __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Level { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int LevelLimit { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int CharType { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Material1 { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMaterial1Bytes() { return __vector_as_arraysegment(10); }
  public int Quantity1 { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Material2 { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMaterial2Bytes() { return __vector_as_arraysegment(14); }
  public int Quantity2 { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Material3 { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMaterial3Bytes() { return __vector_as_arraysegment(18); }
  public int Quantity3 { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Material4 { get { int o = __offset(22); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMaterial4Bytes() { return __vector_as_arraysegment(22); }
  public int Quantity4 { get { int o = __offset(24); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<Evolution> CreateEvolution(FlatBufferBuilder builder,
      int level = 0,
      int level_limit = 0,
      int char_type = 0,
      StringOffset material1Offset = default(StringOffset),
      int quantity1 = 0,
      StringOffset material2Offset = default(StringOffset),
      int quantity2 = 0,
      StringOffset material3Offset = default(StringOffset),
      int quantity3 = 0,
      StringOffset material4Offset = default(StringOffset),
      int quantity4 = 0) {
    builder.StartObject(11);
    Evolution.AddQuantity4(builder, quantity4);
    Evolution.AddMaterial4(builder, material4Offset);
    Evolution.AddQuantity3(builder, quantity3);
    Evolution.AddMaterial3(builder, material3Offset);
    Evolution.AddQuantity2(builder, quantity2);
    Evolution.AddMaterial2(builder, material2Offset);
    Evolution.AddQuantity1(builder, quantity1);
    Evolution.AddMaterial1(builder, material1Offset);
    Evolution.AddCharType(builder, char_type);
    Evolution.AddLevelLimit(builder, level_limit);
    Evolution.AddLevel(builder, level);
    return Evolution.EndEvolution(builder);
  }

  public static void StartEvolution(FlatBufferBuilder builder) { builder.StartObject(11); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(0, level, 0); }
  public static void AddLevelLimit(FlatBufferBuilder builder, int levelLimit) { builder.AddInt(1, levelLimit, 0); }
  public static void AddCharType(FlatBufferBuilder builder, int charType) { builder.AddInt(2, charType, 0); }
  public static void AddMaterial1(FlatBufferBuilder builder, StringOffset material1Offset) { builder.AddOffset(3, material1Offset.Value, 0); }
  public static void AddQuantity1(FlatBufferBuilder builder, int quantity1) { builder.AddInt(4, quantity1, 0); }
  public static void AddMaterial2(FlatBufferBuilder builder, StringOffset material2Offset) { builder.AddOffset(5, material2Offset.Value, 0); }
  public static void AddQuantity2(FlatBufferBuilder builder, int quantity2) { builder.AddInt(6, quantity2, 0); }
  public static void AddMaterial3(FlatBufferBuilder builder, StringOffset material3Offset) { builder.AddOffset(7, material3Offset.Value, 0); }
  public static void AddQuantity3(FlatBufferBuilder builder, int quantity3) { builder.AddInt(8, quantity3, 0); }
  public static void AddMaterial4(FlatBufferBuilder builder, StringOffset material4Offset) { builder.AddOffset(9, material4Offset.Value, 0); }
  public static void AddQuantity4(FlatBufferBuilder builder, int quantity4) { builder.AddInt(10, quantity4, 0); }
  public static Offset<Evolution> EndEvolution(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Evolution>(o);
  }
};

public sealed class HeroLevel : Table {
  public static HeroLevel GetRootAsHeroLevel(ByteBuffer _bb) { return GetRootAsHeroLevel(_bb, new HeroLevel()); }
  public static HeroLevel GetRootAsHeroLevel(ByteBuffer _bb, HeroLevel obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public HeroLevel __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Level { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Exp { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int BuddyExp { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int MaxVigor { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int RecoverVigor { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ResType { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ResQuantity { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<HeroLevel> CreateHeroLevel(FlatBufferBuilder builder,
      int level = 0,
      int exp = 0,
      int buddy_exp = 0,
      int max_vigor = 0,
      int recover_vigor = 0,
      int res_type = 0,
      int res_quantity = 0) {
    builder.StartObject(7);
    HeroLevel.AddResQuantity(builder, res_quantity);
    HeroLevel.AddResType(builder, res_type);
    HeroLevel.AddRecoverVigor(builder, recover_vigor);
    HeroLevel.AddMaxVigor(builder, max_vigor);
    HeroLevel.AddBuddyExp(builder, buddy_exp);
    HeroLevel.AddExp(builder, exp);
    HeroLevel.AddLevel(builder, level);
    return HeroLevel.EndHeroLevel(builder);
  }

  public static void StartHeroLevel(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(0, level, 0); }
  public static void AddExp(FlatBufferBuilder builder, int exp) { builder.AddInt(1, exp, 0); }
  public static void AddBuddyExp(FlatBufferBuilder builder, int buddyExp) { builder.AddInt(2, buddyExp, 0); }
  public static void AddMaxVigor(FlatBufferBuilder builder, int maxVigor) { builder.AddInt(3, maxVigor, 0); }
  public static void AddRecoverVigor(FlatBufferBuilder builder, int recoverVigor) { builder.AddInt(4, recoverVigor, 0); }
  public static void AddResType(FlatBufferBuilder builder, int resType) { builder.AddInt(5, resType, 0); }
  public static void AddResQuantity(FlatBufferBuilder builder, int resQuantity) { builder.AddInt(6, resQuantity, 0); }
  public static Offset<HeroLevel> EndHeroLevel(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<HeroLevel>(o);
  }
};

public sealed class PlayerXPProgression : Table {
  public static PlayerXPProgression GetRootAsPlayerXPProgression(ByteBuffer _bb) { return GetRootAsPlayerXPProgression(_bb, new PlayerXPProgression()); }
  public static PlayerXPProgression GetRootAsPlayerXPProgression(ByteBuffer _bb, PlayerXPProgression obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public PlayerXPProgression __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Level { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ExpRequirement { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int HardCurrencyReward { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SoftCurrencyReward { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int BuddyExp { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ActionPower { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int VigorMax { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int VigorReward { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ResType { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ResQuantity { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<PlayerXPProgression> CreatePlayerXPProgression(FlatBufferBuilder builder,
      int level = 0,
      int exp_requirement = 0,
      int hard_currency_reward = 0,
      int soft_currency_reward = 0,
      int buddy_exp = 0,
      int action_power = 0,
      int vigor_max = 0,
      int vigor_reward = 0,
      int res_type = 0,
      int res_quantity = 0) {
    builder.StartObject(10);
    PlayerXPProgression.AddResQuantity(builder, res_quantity);
    PlayerXPProgression.AddResType(builder, res_type);
    PlayerXPProgression.AddVigorReward(builder, vigor_reward);
    PlayerXPProgression.AddVigorMax(builder, vigor_max);
    PlayerXPProgression.AddActionPower(builder, action_power);
    PlayerXPProgression.AddBuddyExp(builder, buddy_exp);
    PlayerXPProgression.AddSoftCurrencyReward(builder, soft_currency_reward);
    PlayerXPProgression.AddHardCurrencyReward(builder, hard_currency_reward);
    PlayerXPProgression.AddExpRequirement(builder, exp_requirement);
    PlayerXPProgression.AddLevel(builder, level);
    return PlayerXPProgression.EndPlayerXPProgression(builder);
  }

  public static void StartPlayerXPProgression(FlatBufferBuilder builder) { builder.StartObject(10); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(0, level, 0); }
  public static void AddExpRequirement(FlatBufferBuilder builder, int expRequirement) { builder.AddInt(1, expRequirement, 0); }
  public static void AddHardCurrencyReward(FlatBufferBuilder builder, int hardCurrencyReward) { builder.AddInt(2, hardCurrencyReward, 0); }
  public static void AddSoftCurrencyReward(FlatBufferBuilder builder, int softCurrencyReward) { builder.AddInt(3, softCurrencyReward, 0); }
  public static void AddBuddyExp(FlatBufferBuilder builder, int buddyExp) { builder.AddInt(4, buddyExp, 0); }
  public static void AddActionPower(FlatBufferBuilder builder, int actionPower) { builder.AddInt(5, actionPower, 0); }
  public static void AddVigorMax(FlatBufferBuilder builder, int vigorMax) { builder.AddInt(6, vigorMax, 0); }
  public static void AddVigorReward(FlatBufferBuilder builder, int vigorReward) { builder.AddInt(7, vigorReward, 0); }
  public static void AddResType(FlatBufferBuilder builder, int resType) { builder.AddInt(8, resType, 0); }
  public static void AddResQuantity(FlatBufferBuilder builder, int resQuantity) { builder.AddInt(9, resQuantity, 0); }
  public static Offset<PlayerXPProgression> EndPlayerXPProgression(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<PlayerXPProgression>(o);
  }
};

public sealed class MannualBreak : Table {
  public static MannualBreak GetRootAsMannualBreak(ByteBuffer _bb) { return GetRootAsMannualBreak(_bb, new MannualBreak()); }
  public static MannualBreak GetRootAsMannualBreak(ByteBuffer _bb, MannualBreak obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public MannualBreak __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Type { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Level { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Material1 { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMaterial1Bytes() { return __vector_as_arraysegment(10); }
  public int Quantity1 { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float ScorePromotion { get { int o = __offset(14); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float IncMaxHP { get { int o = __offset(16); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float IncATK { get { int o = __offset(18); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float IncDEF { get { int o = __offset(20); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<MannualBreak> CreateMannualBreak(FlatBufferBuilder builder,
      int id = 0,
      int type = 0,
      int level = 0,
      StringOffset material_1Offset = default(StringOffset),
      int quantity_1 = 0,
      float score_promotion = 0,
      float inc_MaxHP = 0,
      float inc_ATK = 0,
      float inc_DEF = 0) {
    builder.StartObject(9);
    MannualBreak.AddIncDEF(builder, inc_DEF);
    MannualBreak.AddIncATK(builder, inc_ATK);
    MannualBreak.AddIncMaxHP(builder, inc_MaxHP);
    MannualBreak.AddScorePromotion(builder, score_promotion);
    MannualBreak.AddQuantity1(builder, quantity_1);
    MannualBreak.AddMaterial1(builder, material_1Offset);
    MannualBreak.AddLevel(builder, level);
    MannualBreak.AddType(builder, type);
    MannualBreak.AddId(builder, id);
    return MannualBreak.EndMannualBreak(builder);
  }

  public static void StartMannualBreak(FlatBufferBuilder builder) { builder.StartObject(9); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddType(FlatBufferBuilder builder, int type) { builder.AddInt(1, type, 0); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(2, level, 0); }
  public static void AddMaterial1(FlatBufferBuilder builder, StringOffset material1Offset) { builder.AddOffset(3, material1Offset.Value, 0); }
  public static void AddQuantity1(FlatBufferBuilder builder, int quantity1) { builder.AddInt(4, quantity1, 0); }
  public static void AddScorePromotion(FlatBufferBuilder builder, float scorePromotion) { builder.AddFloat(5, scorePromotion, 0); }
  public static void AddIncMaxHP(FlatBufferBuilder builder, float incMaxHP) { builder.AddFloat(6, incMaxHP, 0); }
  public static void AddIncATK(FlatBufferBuilder builder, float incATK) { builder.AddFloat(7, incATK, 0); }
  public static void AddIncDEF(FlatBufferBuilder builder, float incDEF) { builder.AddFloat(8, incDEF, 0); }
  public static Offset<MannualBreak> EndMannualBreak(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<MannualBreak>(o);
  }
};

public sealed class MannualScore : Table {
  public static MannualScore GetRootAsMannualScore(ByteBuffer _bb) { return GetRootAsMannualScore(_bb, new MannualScore()); }
  public static MannualScore GetRootAsMannualScore(ByteBuffer _bb, MannualScore obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public MannualScore __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Score { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Evaluate { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float AttributeAddition { get { int o = __offset(10); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float ATK { get { int o = __offset(12); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float MaxHP { get { int o = __offset(14); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float DEF { get { int o = __offset(16); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<MannualScore> CreateMannualScore(FlatBufferBuilder builder,
      int id = 0,
      int score = 0,
      int evaluate = 0,
      float attribute_addition = 0,
      float ATK = 0,
      float MaxHP = 0,
      float DEF = 0) {
    builder.StartObject(7);
    MannualScore.AddDEF(builder, DEF);
    MannualScore.AddMaxHP(builder, MaxHP);
    MannualScore.AddATK(builder, ATK);
    MannualScore.AddAttributeAddition(builder, attribute_addition);
    MannualScore.AddEvaluate(builder, evaluate);
    MannualScore.AddScore(builder, score);
    MannualScore.AddId(builder, id);
    return MannualScore.EndMannualScore(builder);
  }

  public static void StartMannualScore(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddScore(FlatBufferBuilder builder, int score) { builder.AddInt(1, score, 0); }
  public static void AddEvaluate(FlatBufferBuilder builder, int evaluate) { builder.AddInt(2, evaluate, 0); }
  public static void AddAttributeAddition(FlatBufferBuilder builder, float attributeAddition) { builder.AddFloat(3, attributeAddition, 0); }
  public static void AddATK(FlatBufferBuilder builder, float ATK) { builder.AddFloat(4, ATK, 0); }
  public static void AddMaxHP(FlatBufferBuilder builder, float MaxHP) { builder.AddFloat(5, MaxHP, 0); }
  public static void AddDEF(FlatBufferBuilder builder, float DEF) { builder.AddFloat(6, DEF, 0); }
  public static Offset<MannualScore> EndMannualScore(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<MannualScore>(o);
  }
};

public sealed class MannualRoleGrade : Table {
  public static MannualRoleGrade GetRootAsMannualRoleGrade(ByteBuffer _bb) { return GetRootAsMannualRoleGrade(_bb, new MannualRoleGrade()); }
  public static MannualRoleGrade GetRootAsMannualRoleGrade(ByteBuffer _bb, MannualRoleGrade obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public MannualRoleGrade __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string RoleGrade { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRoleGradeBytes() { return __vector_as_arraysegment(6); }
  public string ScoreAddition { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetScoreAdditionBytes() { return __vector_as_arraysegment(8); }
  public float StarAddition { get { int o = __offset(10); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<MannualRoleGrade> CreateMannualRoleGrade(FlatBufferBuilder builder,
      int id = 0,
      StringOffset role_gradeOffset = default(StringOffset),
      StringOffset score_additionOffset = default(StringOffset),
      float star_addition = 0) {
    builder.StartObject(4);
    MannualRoleGrade.AddStarAddition(builder, star_addition);
    MannualRoleGrade.AddScoreAddition(builder, score_additionOffset);
    MannualRoleGrade.AddRoleGrade(builder, role_gradeOffset);
    MannualRoleGrade.AddId(builder, id);
    return MannualRoleGrade.EndMannualRoleGrade(builder);
  }

  public static void StartMannualRoleGrade(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddRoleGrade(FlatBufferBuilder builder, StringOffset roleGradeOffset) { builder.AddOffset(1, roleGradeOffset.Value, 0); }
  public static void AddScoreAddition(FlatBufferBuilder builder, StringOffset scoreAdditionOffset) { builder.AddOffset(2, scoreAdditionOffset.Value, 0); }
  public static void AddStarAddition(FlatBufferBuilder builder, float starAddition) { builder.AddFloat(3, starAddition, 0); }
  public static Offset<MannualRoleGrade> EndMannualRoleGrade(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<MannualRoleGrade>(o);
  }
};

public sealed class MannualUpgradeScore : Table {
  public static MannualUpgradeScore GetRootAsMannualUpgradeScore(ByteBuffer _bb) { return GetRootAsMannualUpgradeScore(_bb, new MannualUpgradeScore()); }
  public static MannualUpgradeScore GetRootAsMannualUpgradeScore(ByteBuffer _bb, MannualUpgradeScore obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public MannualUpgradeScore __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Upgrade { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetUpgradeBytes() { return __vector_as_arraysegment(6); }
  public int Score { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<MannualUpgradeScore> CreateMannualUpgradeScore(FlatBufferBuilder builder,
      int id = 0,
      StringOffset upgradeOffset = default(StringOffset),
      int score = 0) {
    builder.StartObject(3);
    MannualUpgradeScore.AddScore(builder, score);
    MannualUpgradeScore.AddUpgrade(builder, upgradeOffset);
    MannualUpgradeScore.AddId(builder, id);
    return MannualUpgradeScore.EndMannualUpgradeScore(builder);
  }

  public static void StartMannualUpgradeScore(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddUpgrade(FlatBufferBuilder builder, StringOffset upgradeOffset) { builder.AddOffset(1, upgradeOffset.Value, 0); }
  public static void AddScore(FlatBufferBuilder builder, int score) { builder.AddInt(2, score, 0); }
  public static Offset<MannualUpgradeScore> EndMannualUpgradeScore(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<MannualUpgradeScore>(o);
  }
};

public sealed class LevelUp : Table {
  public static LevelUp GetRootAsLevelUp(ByteBuffer _bb) { return GetRootAsLevelUp(_bb, new LevelUp()); }
  public static LevelUp GetRootAsLevelUp(ByteBuffer _bb, LevelUp obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public LevelUp __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Id { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIdBytes() { return __vector_as_arraysegment(4); }
  public int Level { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float MaxHP { get { int o = __offset(8); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float ATK { get { int o = __offset(10); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float DEF { get { int o = __offset(12); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float Speed { get { int o = __offset(14); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float CritP { get { int o = __offset(16); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float CritV { get { int o = __offset(18); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float SpExtra { get { int o = __offset(20); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float SpRes { get { int o = __offset(22); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<LevelUp> CreateLevelUp(FlatBufferBuilder builder,
      StringOffset idOffset = default(StringOffset),
      int level = 0,
      float MaxHP = 0,
      float ATK = 0,
      float DEF = 0,
      float speed = 0,
      float CritP = 0,
      float CritV = 0,
      float SpExtra = 0,
      float SpRes = 0) {
    builder.StartObject(10);
    LevelUp.AddSpRes(builder, SpRes);
    LevelUp.AddSpExtra(builder, SpExtra);
    LevelUp.AddCritV(builder, CritV);
    LevelUp.AddCritP(builder, CritP);
    LevelUp.AddSpeed(builder, speed);
    LevelUp.AddDEF(builder, DEF);
    LevelUp.AddATK(builder, ATK);
    LevelUp.AddMaxHP(builder, MaxHP);
    LevelUp.AddLevel(builder, level);
    LevelUp.AddId(builder, idOffset);
    return LevelUp.EndLevelUp(builder);
  }

  public static void StartLevelUp(FlatBufferBuilder builder) { builder.StartObject(10); }
  public static void AddId(FlatBufferBuilder builder, StringOffset idOffset) { builder.AddOffset(0, idOffset.Value, 0); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(1, level, 0); }
  public static void AddMaxHP(FlatBufferBuilder builder, float MaxHP) { builder.AddFloat(2, MaxHP, 0); }
  public static void AddATK(FlatBufferBuilder builder, float ATK) { builder.AddFloat(3, ATK, 0); }
  public static void AddDEF(FlatBufferBuilder builder, float DEF) { builder.AddFloat(4, DEF, 0); }
  public static void AddSpeed(FlatBufferBuilder builder, float speed) { builder.AddFloat(5, speed, 0); }
  public static void AddCritP(FlatBufferBuilder builder, float CritP) { builder.AddFloat(6, CritP, 0); }
  public static void AddCritV(FlatBufferBuilder builder, float CritV) { builder.AddFloat(7, CritV, 0); }
  public static void AddSpExtra(FlatBufferBuilder builder, float SpExtra) { builder.AddFloat(8, SpExtra, 0); }
  public static void AddSpRes(FlatBufferBuilder builder, float SpRes) { builder.AddFloat(9, SpRes, 0); }
  public static Offset<LevelUp> EndLevelUp(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<LevelUp>(o);
  }
};

public sealed class ProficiencyUp : Table {
  public static ProficiencyUp GetRootAsProficiencyUp(ByteBuffer _bb) { return GetRootAsProficiencyUp(_bb, new ProficiencyUp()); }
  public static ProficiencyUp GetRootAsProficiencyUp(ByteBuffer _bb, ProficiencyUp obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ProficiencyUp __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Type { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Level { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int GetChipCost(int j) { int o = __offset(10); return o != 0 ? bb.GetInt(__vector(o) + j * 4) : (int)0; }
  public int ChipCostLength { get { int o = __offset(10); return o != 0 ? __vector_len(o) : 0; } }
  public ArraySegment<byte>? GetChipCostBytes() { return __vector_as_arraysegment(10); }
  public int PotenCost { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int GetGoldCost(int j) { int o = __offset(14); return o != 0 ? bb.GetInt(__vector(o) + j * 4) : (int)0; }
  public int GoldCostLength { get { int o = __offset(14); return o != 0 ? __vector_len(o) : 0; } }
  public ArraySegment<byte>? GetGoldCostBytes() { return __vector_as_arraysegment(14); }
  public float ATK { get { int o = __offset(16); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float MaxHP { get { int o = __offset(18); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float DEF { get { int o = __offset(20); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float Speed { get { int o = __offset(22); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float CritP { get { int o = __offset(24); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float CritV { get { int o = __offset(26); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float AntiCritP { get { int o = __offset(28); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float SpExtra { get { int o = __offset(30); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float SpRes { get { int o = __offset(32); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float DmgMulti { get { int o = __offset(34); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float DmgRes { get { int o = __offset(36); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int Form { get { int o = __offset(38); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<ProficiencyUp> CreateProficiencyUp(FlatBufferBuilder builder,
      int id = 0,
      int type = 0,
      int level = 0,
      VectorOffset chipCostOffset = default(VectorOffset),
      int potenCost = 0,
      VectorOffset goldCostOffset = default(VectorOffset),
      float ATK = 0,
      float MaxHP = 0,
      float DEF = 0,
      float speed = 0,
      float CritP = 0,
      float CritV = 0,
      float AntiCritP = 0,
      float SpExtra = 0,
      float SpRes = 0,
      float DmgMulti = 0,
      float DmgRes = 0,
      int form = 0) {
    builder.StartObject(18);
    ProficiencyUp.AddForm(builder, form);
    ProficiencyUp.AddDmgRes(builder, DmgRes);
    ProficiencyUp.AddDmgMulti(builder, DmgMulti);
    ProficiencyUp.AddSpRes(builder, SpRes);
    ProficiencyUp.AddSpExtra(builder, SpExtra);
    ProficiencyUp.AddAntiCritP(builder, AntiCritP);
    ProficiencyUp.AddCritV(builder, CritV);
    ProficiencyUp.AddCritP(builder, CritP);
    ProficiencyUp.AddSpeed(builder, speed);
    ProficiencyUp.AddDEF(builder, DEF);
    ProficiencyUp.AddMaxHP(builder, MaxHP);
    ProficiencyUp.AddATK(builder, ATK);
    ProficiencyUp.AddGoldCost(builder, goldCostOffset);
    ProficiencyUp.AddPotenCost(builder, potenCost);
    ProficiencyUp.AddChipCost(builder, chipCostOffset);
    ProficiencyUp.AddLevel(builder, level);
    ProficiencyUp.AddType(builder, type);
    ProficiencyUp.AddId(builder, id);
    return ProficiencyUp.EndProficiencyUp(builder);
  }

  public static void StartProficiencyUp(FlatBufferBuilder builder) { builder.StartObject(18); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddType(FlatBufferBuilder builder, int type) { builder.AddInt(1, type, 0); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(2, level, 0); }
  public static void AddChipCost(FlatBufferBuilder builder, VectorOffset chipCostOffset) { builder.AddOffset(3, chipCostOffset.Value, 0); }
  public static VectorOffset CreateChipCostVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartChipCostVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddPotenCost(FlatBufferBuilder builder, int potenCost) { builder.AddInt(4, potenCost, 0); }
  public static void AddGoldCost(FlatBufferBuilder builder, VectorOffset goldCostOffset) { builder.AddOffset(5, goldCostOffset.Value, 0); }
  public static VectorOffset CreateGoldCostVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartGoldCostVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddATK(FlatBufferBuilder builder, float ATK) { builder.AddFloat(6, ATK, 0); }
  public static void AddMaxHP(FlatBufferBuilder builder, float MaxHP) { builder.AddFloat(7, MaxHP, 0); }
  public static void AddDEF(FlatBufferBuilder builder, float DEF) { builder.AddFloat(8, DEF, 0); }
  public static void AddSpeed(FlatBufferBuilder builder, float speed) { builder.AddFloat(9, speed, 0); }
  public static void AddCritP(FlatBufferBuilder builder, float CritP) { builder.AddFloat(10, CritP, 0); }
  public static void AddCritV(FlatBufferBuilder builder, float CritV) { builder.AddFloat(11, CritV, 0); }
  public static void AddAntiCritP(FlatBufferBuilder builder, float AntiCritP) { builder.AddFloat(12, AntiCritP, 0); }
  public static void AddSpExtra(FlatBufferBuilder builder, float SpExtra) { builder.AddFloat(13, SpExtra, 0); }
  public static void AddSpRes(FlatBufferBuilder builder, float SpRes) { builder.AddFloat(14, SpRes, 0); }
  public static void AddDmgMulti(FlatBufferBuilder builder, float DmgMulti) { builder.AddFloat(15, DmgMulti, 0); }
  public static void AddDmgRes(FlatBufferBuilder builder, float DmgRes) { builder.AddFloat(16, DmgRes, 0); }
  public static void AddForm(FlatBufferBuilder builder, int form) { builder.AddInt(17, form, 0); }
  public static Offset<ProficiencyUp> EndProficiencyUp(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ProficiencyUp>(o);
  }
};

public sealed class ProficiencyDescribe : Table {
  public static ProficiencyDescribe GetRootAsProficiencyDescribe(ByteBuffer _bb) { return GetRootAsProficiencyDescribe(_bb, new ProficiencyDescribe()); }
  public static ProficiencyDescribe GetRootAsProficiencyDescribe(ByteBuffer _bb, ProficiencyDescribe obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ProficiencyDescribe __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Name { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(6); }
  public string Describe { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDescribeBytes() { return __vector_as_arraysegment(8); }
  public string Icon { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIconBytes() { return __vector_as_arraysegment(10); }
  public int GetLimit(int j) { int o = __offset(12); return o != 0 ? bb.GetInt(__vector(o) + j * 4) : (int)0; }
  public int LimitLength { get { int o = __offset(12); return o != 0 ? __vector_len(o) : 0; } }
  public ArraySegment<byte>? GetLimitBytes() { return __vector_as_arraysegment(12); }

  public static Offset<ProficiencyDescribe> CreateProficiencyDescribe(FlatBufferBuilder builder,
      int id = 0,
      StringOffset nameOffset = default(StringOffset),
      StringOffset describeOffset = default(StringOffset),
      StringOffset iconOffset = default(StringOffset),
      VectorOffset limitOffset = default(VectorOffset)) {
    builder.StartObject(5);
    ProficiencyDescribe.AddLimit(builder, limitOffset);
    ProficiencyDescribe.AddIcon(builder, iconOffset);
    ProficiencyDescribe.AddDescribe(builder, describeOffset);
    ProficiencyDescribe.AddName(builder, nameOffset);
    ProficiencyDescribe.AddId(builder, id);
    return ProficiencyDescribe.EndProficiencyDescribe(builder);
  }

  public static void StartProficiencyDescribe(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(1, nameOffset.Value, 0); }
  public static void AddDescribe(FlatBufferBuilder builder, StringOffset describeOffset) { builder.AddOffset(2, describeOffset.Value, 0); }
  public static void AddIcon(FlatBufferBuilder builder, StringOffset iconOffset) { builder.AddOffset(3, iconOffset.Value, 0); }
  public static void AddLimit(FlatBufferBuilder builder, VectorOffset limitOffset) { builder.AddOffset(4, limitOffset.Value, 0); }
  public static VectorOffset CreateLimitVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartLimitVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<ProficiencyDescribe> EndProficiencyDescribe(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ProficiencyDescribe>(o);
  }
};

public sealed class SwitchCost : Table {
  public static SwitchCost GetRootAsSwitchCost(ByteBuffer _bb) { return GetRootAsSwitchCost(_bb, new SwitchCost()); }
  public static SwitchCost GetRootAsSwitchCost(ByteBuffer _bb, SwitchCost obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public SwitchCost __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Upgrade { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Cost { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<SwitchCost> CreateSwitchCost(FlatBufferBuilder builder,
      int upgrade = 0,
      int cost = 0) {
    builder.StartObject(2);
    SwitchCost.AddCost(builder, cost);
    SwitchCost.AddUpgrade(builder, upgrade);
    return SwitchCost.EndSwitchCost(builder);
  }

  public static void StartSwitchCost(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddUpgrade(FlatBufferBuilder builder, int upgrade) { builder.AddInt(0, upgrade, 0); }
  public static void AddCost(FlatBufferBuilder builder, int cost) { builder.AddInt(1, cost, 0); }
  public static Offset<SwitchCost> EndSwitchCost(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<SwitchCost>(o);
  }
};

public sealed class HeroFetter : Table {
  public static HeroFetter GetRootAsHeroFetter(ByteBuffer _bb) { return GetRootAsHeroFetter(_bb, new HeroFetter()); }
  public static HeroFetter GetRootAsHeroFetter(ByteBuffer _bb, HeroFetter obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public HeroFetter __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Index { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Condition { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Param { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetParamBytes() { return __vector_as_arraysegment(8); }
  public int Level { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float IncMaxHP { get { int o = __offset(12); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float IncATK { get { int o = __offset(14); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float IncDEF { get { int o = __offset(16); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<HeroFetter> CreateHeroFetter(FlatBufferBuilder builder,
      int index = 0,
      int condition = 0,
      StringOffset paramOffset = default(StringOffset),
      int level = 0,
      float inc_MaxHP = 0,
      float inc_ATK = 0,
      float inc_DEF = 0) {
    builder.StartObject(7);
    HeroFetter.AddIncDEF(builder, inc_DEF);
    HeroFetter.AddIncATK(builder, inc_ATK);
    HeroFetter.AddIncMaxHP(builder, inc_MaxHP);
    HeroFetter.AddLevel(builder, level);
    HeroFetter.AddParam(builder, paramOffset);
    HeroFetter.AddCondition(builder, condition);
    HeroFetter.AddIndex(builder, index);
    return HeroFetter.EndHeroFetter(builder);
  }

  public static void StartHeroFetter(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddIndex(FlatBufferBuilder builder, int index) { builder.AddInt(0, index, 0); }
  public static void AddCondition(FlatBufferBuilder builder, int condition) { builder.AddInt(1, condition, 0); }
  public static void AddParam(FlatBufferBuilder builder, StringOffset paramOffset) { builder.AddOffset(2, paramOffset.Value, 0); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(3, level, 0); }
  public static void AddIncMaxHP(FlatBufferBuilder builder, float incMaxHP) { builder.AddFloat(4, incMaxHP, 0); }
  public static void AddIncATK(FlatBufferBuilder builder, float incATK) { builder.AddFloat(5, incATK, 0); }
  public static void AddIncDEF(FlatBufferBuilder builder, float incDEF) { builder.AddFloat(6, incDEF, 0); }
  public static Offset<HeroFetter> EndHeroFetter(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<HeroFetter>(o);
  }
};

public sealed class HeroAwaken : Table {
  public static HeroAwaken GetRootAsHeroAwaken(ByteBuffer _bb) { return GetRootAsHeroAwaken(_bb, new HeroAwaken()); }
  public static HeroAwaken GetRootAsHeroAwaken(ByteBuffer _bb, HeroAwaken obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public HeroAwaken __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Id { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIdBytes() { return __vector_as_arraysegment(4); }
  public int Level { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int UpgradeId { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Star { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string AwakenIcon { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetAwakenIconBytes() { return __vector_as_arraysegment(12); }
  public string AwakenSkin { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetAwakenSkinBytes() { return __vector_as_arraysegment(14); }
  public int AwakeType { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string AttributeAdd { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetAttributeAddBytes() { return __vector_as_arraysegment(18); }
  public int AwakenBeforeSkill { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int AwakenLaterSkill { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Material1 { get { int o = __offset(24); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMaterial1Bytes() { return __vector_as_arraysegment(24); }
  public int Quantity1 { get { int o = __offset(26); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Material2 { get { int o = __offset(28); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMaterial2Bytes() { return __vector_as_arraysegment(28); }
  public int Quantity2 { get { int o = __offset(30); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Material3 { get { int o = __offset(32); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMaterial3Bytes() { return __vector_as_arraysegment(32); }
  public int Quantity3 { get { int o = __offset(34); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Material4 { get { int o = __offset(36); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMaterial4Bytes() { return __vector_as_arraysegment(36); }
  public int Quantity4 { get { int o = __offset(38); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float IncMaxHP { get { int o = __offset(40); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float IncATK { get { int o = __offset(42); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float IncDEF { get { int o = __offset(44); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float IncSpeed { get { int o = __offset(46); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<HeroAwaken> CreateHeroAwaken(FlatBufferBuilder builder,
      StringOffset idOffset = default(StringOffset),
      int level = 0,
      int upgrade_id = 0,
      int star = 0,
      StringOffset awaken_iconOffset = default(StringOffset),
      StringOffset awaken_skinOffset = default(StringOffset),
      int awake_type = 0,
      StringOffset attribute_addOffset = default(StringOffset),
      int awaken_before_skill = 0,
      int awaken_later_skill = 0,
      StringOffset material_1Offset = default(StringOffset),
      int quantity_1 = 0,
      StringOffset material_2Offset = default(StringOffset),
      int quantity_2 = 0,
      StringOffset material_3Offset = default(StringOffset),
      int quantity_3 = 0,
      StringOffset material_4Offset = default(StringOffset),
      int quantity_4 = 0,
      float inc_MaxHP = 0,
      float inc_ATK = 0,
      float inc_DEF = 0,
      float inc_speed = 0) {
    builder.StartObject(22);
    HeroAwaken.AddIncSpeed(builder, inc_speed);
    HeroAwaken.AddIncDEF(builder, inc_DEF);
    HeroAwaken.AddIncATK(builder, inc_ATK);
    HeroAwaken.AddIncMaxHP(builder, inc_MaxHP);
    HeroAwaken.AddQuantity4(builder, quantity_4);
    HeroAwaken.AddMaterial4(builder, material_4Offset);
    HeroAwaken.AddQuantity3(builder, quantity_3);
    HeroAwaken.AddMaterial3(builder, material_3Offset);
    HeroAwaken.AddQuantity2(builder, quantity_2);
    HeroAwaken.AddMaterial2(builder, material_2Offset);
    HeroAwaken.AddQuantity1(builder, quantity_1);
    HeroAwaken.AddMaterial1(builder, material_1Offset);
    HeroAwaken.AddAwakenLaterSkill(builder, awaken_later_skill);
    HeroAwaken.AddAwakenBeforeSkill(builder, awaken_before_skill);
    HeroAwaken.AddAttributeAdd(builder, attribute_addOffset);
    HeroAwaken.AddAwakeType(builder, awake_type);
    HeroAwaken.AddAwakenSkin(builder, awaken_skinOffset);
    HeroAwaken.AddAwakenIcon(builder, awaken_iconOffset);
    HeroAwaken.AddStar(builder, star);
    HeroAwaken.AddUpgradeId(builder, upgrade_id);
    HeroAwaken.AddLevel(builder, level);
    HeroAwaken.AddId(builder, idOffset);
    return HeroAwaken.EndHeroAwaken(builder);
  }

  public static void StartHeroAwaken(FlatBufferBuilder builder) { builder.StartObject(22); }
  public static void AddId(FlatBufferBuilder builder, StringOffset idOffset) { builder.AddOffset(0, idOffset.Value, 0); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(1, level, 0); }
  public static void AddUpgradeId(FlatBufferBuilder builder, int upgradeId) { builder.AddInt(2, upgradeId, 0); }
  public static void AddStar(FlatBufferBuilder builder, int star) { builder.AddInt(3, star, 0); }
  public static void AddAwakenIcon(FlatBufferBuilder builder, StringOffset awakenIconOffset) { builder.AddOffset(4, awakenIconOffset.Value, 0); }
  public static void AddAwakenSkin(FlatBufferBuilder builder, StringOffset awakenSkinOffset) { builder.AddOffset(5, awakenSkinOffset.Value, 0); }
  public static void AddAwakeType(FlatBufferBuilder builder, int awakeType) { builder.AddInt(6, awakeType, 0); }
  public static void AddAttributeAdd(FlatBufferBuilder builder, StringOffset attributeAddOffset) { builder.AddOffset(7, attributeAddOffset.Value, 0); }
  public static void AddAwakenBeforeSkill(FlatBufferBuilder builder, int awakenBeforeSkill) { builder.AddInt(8, awakenBeforeSkill, 0); }
  public static void AddAwakenLaterSkill(FlatBufferBuilder builder, int awakenLaterSkill) { builder.AddInt(9, awakenLaterSkill, 0); }
  public static void AddMaterial1(FlatBufferBuilder builder, StringOffset material1Offset) { builder.AddOffset(10, material1Offset.Value, 0); }
  public static void AddQuantity1(FlatBufferBuilder builder, int quantity1) { builder.AddInt(11, quantity1, 0); }
  public static void AddMaterial2(FlatBufferBuilder builder, StringOffset material2Offset) { builder.AddOffset(12, material2Offset.Value, 0); }
  public static void AddQuantity2(FlatBufferBuilder builder, int quantity2) { builder.AddInt(13, quantity2, 0); }
  public static void AddMaterial3(FlatBufferBuilder builder, StringOffset material3Offset) { builder.AddOffset(14, material3Offset.Value, 0); }
  public static void AddQuantity3(FlatBufferBuilder builder, int quantity3) { builder.AddInt(15, quantity3, 0); }
  public static void AddMaterial4(FlatBufferBuilder builder, StringOffset material4Offset) { builder.AddOffset(16, material4Offset.Value, 0); }
  public static void AddQuantity4(FlatBufferBuilder builder, int quantity4) { builder.AddInt(17, quantity4, 0); }
  public static void AddIncMaxHP(FlatBufferBuilder builder, float incMaxHP) { builder.AddFloat(18, incMaxHP, 0); }
  public static void AddIncATK(FlatBufferBuilder builder, float incATK) { builder.AddFloat(19, incATK, 0); }
  public static void AddIncDEF(FlatBufferBuilder builder, float incDEF) { builder.AddFloat(20, incDEF, 0); }
  public static void AddIncSpeed(FlatBufferBuilder builder, float incSpeed) { builder.AddFloat(21, incSpeed, 0); }
  public static Offset<HeroAwaken> EndHeroAwaken(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<HeroAwaken>(o);
  }
};

public sealed class HeroStrategy : Table {
  public static HeroStrategy GetRootAsHeroStrategy(ByteBuffer _bb) { return GetRootAsHeroStrategy(_bb, new HeroStrategy()); }
  public static HeroStrategy GetRootAsHeroStrategy(ByteBuffer _bb, HeroStrategy obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public HeroStrategy __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int InfoId { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Damage { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Existence { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Control { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Assist { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int RecommendedSuit { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string RecommendedAttr { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRecommendedAttrBytes() { return __vector_as_arraysegment(16); }
  public string RecommendedMatch { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRecommendedMatchBytes() { return __vector_as_arraysegment(18); }
  public string MatchDesc { get { int o = __offset(20); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMatchDescBytes() { return __vector_as_arraysegment(20); }

  public static Offset<HeroStrategy> CreateHeroStrategy(FlatBufferBuilder builder,
      int info_id = 0,
      int damage = 0,
      int existence = 0,
      int control = 0,
      int assist = 0,
      int recommended_suit = 0,
      StringOffset recommended_attrOffset = default(StringOffset),
      StringOffset recommended_matchOffset = default(StringOffset),
      StringOffset match_descOffset = default(StringOffset)) {
    builder.StartObject(9);
    HeroStrategy.AddMatchDesc(builder, match_descOffset);
    HeroStrategy.AddRecommendedMatch(builder, recommended_matchOffset);
    HeroStrategy.AddRecommendedAttr(builder, recommended_attrOffset);
    HeroStrategy.AddRecommendedSuit(builder, recommended_suit);
    HeroStrategy.AddAssist(builder, assist);
    HeroStrategy.AddControl(builder, control);
    HeroStrategy.AddExistence(builder, existence);
    HeroStrategy.AddDamage(builder, damage);
    HeroStrategy.AddInfoId(builder, info_id);
    return HeroStrategy.EndHeroStrategy(builder);
  }

  public static void StartHeroStrategy(FlatBufferBuilder builder) { builder.StartObject(9); }
  public static void AddInfoId(FlatBufferBuilder builder, int infoId) { builder.AddInt(0, infoId, 0); }
  public static void AddDamage(FlatBufferBuilder builder, int damage) { builder.AddInt(1, damage, 0); }
  public static void AddExistence(FlatBufferBuilder builder, int existence) { builder.AddInt(2, existence, 0); }
  public static void AddControl(FlatBufferBuilder builder, int control) { builder.AddInt(3, control, 0); }
  public static void AddAssist(FlatBufferBuilder builder, int assist) { builder.AddInt(4, assist, 0); }
  public static void AddRecommendedSuit(FlatBufferBuilder builder, int recommendedSuit) { builder.AddInt(5, recommendedSuit, 0); }
  public static void AddRecommendedAttr(FlatBufferBuilder builder, StringOffset recommendedAttrOffset) { builder.AddOffset(6, recommendedAttrOffset.Value, 0); }
  public static void AddRecommendedMatch(FlatBufferBuilder builder, StringOffset recommendedMatchOffset) { builder.AddOffset(7, recommendedMatchOffset.Value, 0); }
  public static void AddMatchDesc(FlatBufferBuilder builder, StringOffset matchDescOffset) { builder.AddOffset(8, matchDescOffset.Value, 0); }
  public static Offset<HeroStrategy> EndHeroStrategy(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<HeroStrategy>(o);
  }
};

public sealed class Promotion : Table {
  public static Promotion GetRootAsPromotion(ByteBuffer _bb) { return GetRootAsPromotion(_bb, new Promotion()); }
  public static Promotion GetRootAsPromotion(ByteBuffer _bb, Promotion obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Promotion __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Name { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(6); }
  public string BigIcon { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBigIconBytes() { return __vector_as_arraysegment(8); }
  public string SmallIcon { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetSmallIconBytes() { return __vector_as_arraysegment(10); }
  public int Level { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Star { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string TaskIds { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTaskIdsBytes() { return __vector_as_arraysegment(16); }
  public int ItemId { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Cost { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int AdditiveAttributeLevel { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int AttributeLevelUpperLimit { get { int o = __offset(24); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int QualityLevel { get { int o = __offset(26); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<Promotion> CreatePromotion(FlatBufferBuilder builder,
      int id = 0,
      StringOffset nameOffset = default(StringOffset),
      StringOffset big_iconOffset = default(StringOffset),
      StringOffset small_iconOffset = default(StringOffset),
      int level = 0,
      int star = 0,
      StringOffset task_idsOffset = default(StringOffset),
      int item_id = 0,
      int cost = 0,
      int additive_attribute_level = 0,
      int attribute_level_upper_limit = 0,
      int quality_level = 0) {
    builder.StartObject(12);
    Promotion.AddQualityLevel(builder, quality_level);
    Promotion.AddAttributeLevelUpperLimit(builder, attribute_level_upper_limit);
    Promotion.AddAdditiveAttributeLevel(builder, additive_attribute_level);
    Promotion.AddCost(builder, cost);
    Promotion.AddItemId(builder, item_id);
    Promotion.AddTaskIds(builder, task_idsOffset);
    Promotion.AddStar(builder, star);
    Promotion.AddLevel(builder, level);
    Promotion.AddSmallIcon(builder, small_iconOffset);
    Promotion.AddBigIcon(builder, big_iconOffset);
    Promotion.AddName(builder, nameOffset);
    Promotion.AddId(builder, id);
    return Promotion.EndPromotion(builder);
  }

  public static void StartPromotion(FlatBufferBuilder builder) { builder.StartObject(12); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(1, nameOffset.Value, 0); }
  public static void AddBigIcon(FlatBufferBuilder builder, StringOffset bigIconOffset) { builder.AddOffset(2, bigIconOffset.Value, 0); }
  public static void AddSmallIcon(FlatBufferBuilder builder, StringOffset smallIconOffset) { builder.AddOffset(3, smallIconOffset.Value, 0); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(4, level, 0); }
  public static void AddStar(FlatBufferBuilder builder, int star) { builder.AddInt(5, star, 0); }
  public static void AddTaskIds(FlatBufferBuilder builder, StringOffset taskIdsOffset) { builder.AddOffset(6, taskIdsOffset.Value, 0); }
  public static void AddItemId(FlatBufferBuilder builder, int itemId) { builder.AddInt(7, itemId, 0); }
  public static void AddCost(FlatBufferBuilder builder, int cost) { builder.AddInt(8, cost, 0); }
  public static void AddAdditiveAttributeLevel(FlatBufferBuilder builder, int additiveAttributeLevel) { builder.AddInt(9, additiveAttributeLevel, 0); }
  public static void AddAttributeLevelUpperLimit(FlatBufferBuilder builder, int attributeLevelUpperLimit) { builder.AddInt(10, attributeLevelUpperLimit, 0); }
  public static void AddQualityLevel(FlatBufferBuilder builder, int qualityLevel) { builder.AddInt(11, qualityLevel, 0); }
  public static Offset<Promotion> EndPromotion(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Promotion>(o);
  }
};

public sealed class PromotionTraining : Table {
  public static PromotionTraining GetRootAsPromotionTraining(ByteBuffer _bb) { return GetRootAsPromotionTraining(_bb, new PromotionTraining()); }
  public static PromotionTraining GetRootAsPromotionTraining(ByteBuffer _bb, PromotionTraining obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public PromotionTraining __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Name { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(6); }
  public string NegativeRegion { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNegativeRegionBytes() { return __vector_as_arraysegment(8); }
  public string PositiveRegion { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetPositiveRegionBytes() { return __vector_as_arraysegment(10); }
  public string RegionPercent { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRegionPercentBytes() { return __vector_as_arraysegment(12); }
  public string NegativeProbability { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNegativeProbabilityBytes() { return __vector_as_arraysegment(14); }
  public int UpperLimit { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Cost { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetCostBytes() { return __vector_as_arraysegment(18); }
  public int Count { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<PromotionTraining> CreatePromotionTraining(FlatBufferBuilder builder,
      int id = 0,
      StringOffset nameOffset = default(StringOffset),
      StringOffset negative_regionOffset = default(StringOffset),
      StringOffset positive_regionOffset = default(StringOffset),
      StringOffset region_percentOffset = default(StringOffset),
      StringOffset negative_probabilityOffset = default(StringOffset),
      int upper_limit = 0,
      StringOffset costOffset = default(StringOffset),
      int count = 0) {
    builder.StartObject(9);
    PromotionTraining.AddCount(builder, count);
    PromotionTraining.AddCost(builder, costOffset);
    PromotionTraining.AddUpperLimit(builder, upper_limit);
    PromotionTraining.AddNegativeProbability(builder, negative_probabilityOffset);
    PromotionTraining.AddRegionPercent(builder, region_percentOffset);
    PromotionTraining.AddPositiveRegion(builder, positive_regionOffset);
    PromotionTraining.AddNegativeRegion(builder, negative_regionOffset);
    PromotionTraining.AddName(builder, nameOffset);
    PromotionTraining.AddId(builder, id);
    return PromotionTraining.EndPromotionTraining(builder);
  }

  public static void StartPromotionTraining(FlatBufferBuilder builder) { builder.StartObject(9); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(1, nameOffset.Value, 0); }
  public static void AddNegativeRegion(FlatBufferBuilder builder, StringOffset negativeRegionOffset) { builder.AddOffset(2, negativeRegionOffset.Value, 0); }
  public static void AddPositiveRegion(FlatBufferBuilder builder, StringOffset positiveRegionOffset) { builder.AddOffset(3, positiveRegionOffset.Value, 0); }
  public static void AddRegionPercent(FlatBufferBuilder builder, StringOffset regionPercentOffset) { builder.AddOffset(4, regionPercentOffset.Value, 0); }
  public static void AddNegativeProbability(FlatBufferBuilder builder, StringOffset negativeProbabilityOffset) { builder.AddOffset(5, negativeProbabilityOffset.Value, 0); }
  public static void AddUpperLimit(FlatBufferBuilder builder, int upperLimit) { builder.AddInt(6, upperLimit, 0); }
  public static void AddCost(FlatBufferBuilder builder, StringOffset costOffset) { builder.AddOffset(7, costOffset.Value, 0); }
  public static void AddCount(FlatBufferBuilder builder, int count) { builder.AddInt(8, count, 0); }
  public static Offset<PromotionTraining> EndPromotionTraining(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<PromotionTraining>(o);
  }
};

public sealed class PromotionAttributeLevel : Table {
  public static PromotionAttributeLevel GetRootAsPromotionAttributeLevel(ByteBuffer _bb) { return GetRootAsPromotionAttributeLevel(_bb, new PromotionAttributeLevel()); }
  public static PromotionAttributeLevel GetRootAsPromotionAttributeLevel(ByteBuffer _bb, PromotionAttributeLevel obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public PromotionAttributeLevel __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Name { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(6); }
  public float AttrValue { get { int o = __offset(8); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int UnlockLevel { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<PromotionAttributeLevel> CreatePromotionAttributeLevel(FlatBufferBuilder builder,
      int id = 0,
      StringOffset nameOffset = default(StringOffset),
      float attr_value = 0,
      int unlock_level = 0) {
    builder.StartObject(4);
    PromotionAttributeLevel.AddUnlockLevel(builder, unlock_level);
    PromotionAttributeLevel.AddAttrValue(builder, attr_value);
    PromotionAttributeLevel.AddName(builder, nameOffset);
    PromotionAttributeLevel.AddId(builder, id);
    return PromotionAttributeLevel.EndPromotionAttributeLevel(builder);
  }

  public static void StartPromotionAttributeLevel(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(1, nameOffset.Value, 0); }
  public static void AddAttrValue(FlatBufferBuilder builder, float attrValue) { builder.AddFloat(2, attrValue, 0); }
  public static void AddUnlockLevel(FlatBufferBuilder builder, int unlockLevel) { builder.AddInt(3, unlockLevel, 0); }
  public static Offset<PromotionAttributeLevel> EndPromotionAttributeLevel(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<PromotionAttributeLevel>(o);
  }
};

public sealed class ArtifactEquipments : Table {
  public static ArtifactEquipments GetRootAsArtifactEquipments(ByteBuffer _bb) { return GetRootAsArtifactEquipments(_bb, new ArtifactEquipments()); }
  public static ArtifactEquipments GetRootAsArtifactEquipments(ByteBuffer _bb, ArtifactEquipments obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ArtifactEquipments __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string HeroId { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetHeroIdBytes() { return __vector_as_arraysegment(6); }
  public string Name { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(8); }
  public int EnhancementLevel { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int QualityLevel { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string IconId { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIconIdBytes() { return __vector_as_arraysegment(14); }
  public string AttributeAdd { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetAttributeAddBytes() { return __vector_as_arraysegment(16); }
  public string ItemCost { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemCostBytes() { return __vector_as_arraysegment(18); }
  public int SkillId { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string EnhancementSkillDescribe { get { int o = __offset(22); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetEnhancementSkillDescribeBytes() { return __vector_as_arraysegment(22); }

  public static Offset<ArtifactEquipments> CreateArtifactEquipments(FlatBufferBuilder builder,
      int id = 0,
      StringOffset hero_IdOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      int enhancement_level = 0,
      int quality_level = 0,
      StringOffset icon_idOffset = default(StringOffset),
      StringOffset attribute_addOffset = default(StringOffset),
      StringOffset item_costOffset = default(StringOffset),
      int skill_id = 0,
      StringOffset enhancement_skill_describeOffset = default(StringOffset)) {
    builder.StartObject(10);
    ArtifactEquipments.AddEnhancementSkillDescribe(builder, enhancement_skill_describeOffset);
    ArtifactEquipments.AddSkillId(builder, skill_id);
    ArtifactEquipments.AddItemCost(builder, item_costOffset);
    ArtifactEquipments.AddAttributeAdd(builder, attribute_addOffset);
    ArtifactEquipments.AddIconId(builder, icon_idOffset);
    ArtifactEquipments.AddQualityLevel(builder, quality_level);
    ArtifactEquipments.AddEnhancementLevel(builder, enhancement_level);
    ArtifactEquipments.AddName(builder, nameOffset);
    ArtifactEquipments.AddHeroId(builder, hero_IdOffset);
    ArtifactEquipments.AddId(builder, id);
    return ArtifactEquipments.EndArtifactEquipments(builder);
  }

  public static void StartArtifactEquipments(FlatBufferBuilder builder) { builder.StartObject(10); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddHeroId(FlatBufferBuilder builder, StringOffset heroIdOffset) { builder.AddOffset(1, heroIdOffset.Value, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(2, nameOffset.Value, 0); }
  public static void AddEnhancementLevel(FlatBufferBuilder builder, int enhancementLevel) { builder.AddInt(3, enhancementLevel, 0); }
  public static void AddQualityLevel(FlatBufferBuilder builder, int qualityLevel) { builder.AddInt(4, qualityLevel, 0); }
  public static void AddIconId(FlatBufferBuilder builder, StringOffset iconIdOffset) { builder.AddOffset(5, iconIdOffset.Value, 0); }
  public static void AddAttributeAdd(FlatBufferBuilder builder, StringOffset attributeAddOffset) { builder.AddOffset(6, attributeAddOffset.Value, 0); }
  public static void AddItemCost(FlatBufferBuilder builder, StringOffset itemCostOffset) { builder.AddOffset(7, itemCostOffset.Value, 0); }
  public static void AddSkillId(FlatBufferBuilder builder, int skillId) { builder.AddInt(8, skillId, 0); }
  public static void AddEnhancementSkillDescribe(FlatBufferBuilder builder, StringOffset enhancementSkillDescribeOffset) { builder.AddOffset(9, enhancementSkillDescribeOffset.Value, 0); }
  public static Offset<ArtifactEquipments> EndArtifactEquipments(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ArtifactEquipments>(o);
  }
};

public sealed class ConditionCharacter : Table {
  public static ConditionCharacter GetRootAsConditionCharacter(ByteBuffer _bb) { return GetRootAsConditionCharacter(_bb, new ConditionCharacter()); }
  public static ConditionCharacter GetRootAsConditionCharacter(ByteBuffer _bb, ConditionCharacter obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ConditionCharacter __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

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
  public HeroStat GetHeroStats(int j) { return GetHeroStats(new HeroStat(), j); }
  public HeroStat GetHeroStats(HeroStat obj, int j) { int o = __offset(18); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int HeroStatsLength { get { int o = __offset(18); return o != 0 ? __vector_len(o) : 0; } }
  public HeroInfo GetHeroInfos(int j) { return GetHeroInfos(new HeroInfo(), j); }
  public HeroInfo GetHeroInfos(HeroInfo obj, int j) { int o = __offset(20); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int HeroInfosLength { get { int o = __offset(20); return o != 0 ? __vector_len(o) : 0; } }
  public Monster GetMonster(int j) { return GetMonster(new Monster(), j); }
  public Monster GetMonster(Monster obj, int j) { int o = __offset(22); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int MonsterLength { get { int o = __offset(22); return o != 0 ? __vector_len(o) : 0; } }
  public Star GetStar(int j) { return GetStar(new Star(), j); }
  public Star GetStar(Star obj, int j) { int o = __offset(24); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int StarLength { get { int o = __offset(24); return o != 0 ? __vector_len(o) : 0; } }
  public Upgrade GetUpgrade(int j) { return GetUpgrade(new Upgrade(), j); }
  public Upgrade GetUpgrade(Upgrade obj, int j) { int o = __offset(26); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int UpgradeLength { get { int o = __offset(26); return o != 0 ? __vector_len(o) : 0; } }
  public Evolution GetEvolution(int j) { return GetEvolution(new Evolution(), j); }
  public Evolution GetEvolution(Evolution obj, int j) { int o = __offset(28); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int EvolutionLength { get { int o = __offset(28); return o != 0 ? __vector_len(o) : 0; } }
  public HeroLevel GetLevel(int j) { return GetLevel(new HeroLevel(), j); }
  public HeroLevel GetLevel(HeroLevel obj, int j) { int o = __offset(30); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int LevelLength { get { int o = __offset(30); return o != 0 ? __vector_len(o) : 0; } }
  public StarUp GetStarUp(int j) { return GetStarUp(new StarUp(), j); }
  public StarUp GetStarUp(StarUp obj, int j) { int o = __offset(32); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int StarUpLength { get { int o = __offset(32); return o != 0 ? __vector_len(o) : 0; } }
  public MannualBreak GetMannualBreak(int j) { return GetMannualBreak(new MannualBreak(), j); }
  public MannualBreak GetMannualBreak(MannualBreak obj, int j) { int o = __offset(34); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int MannualBreakLength { get { int o = __offset(34); return o != 0 ? __vector_len(o) : 0; } }
  public LevelUp GetLevelUp(int j) { return GetLevelUp(new LevelUp(), j); }
  public LevelUp GetLevelUp(LevelUp obj, int j) { int o = __offset(36); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int LevelUpLength { get { int o = __offset(36); return o != 0 ? __vector_len(o) : 0; } }
  public MannualScore GetMannualScore(int j) { return GetMannualScore(new MannualScore(), j); }
  public MannualScore GetMannualScore(MannualScore obj, int j) { int o = __offset(38); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int MannualScoreLength { get { int o = __offset(38); return o != 0 ? __vector_len(o) : 0; } }
  public MannualRoleGrade GetMannualRoleGrade(int j) { return GetMannualRoleGrade(new MannualRoleGrade(), j); }
  public MannualRoleGrade GetMannualRoleGrade(MannualRoleGrade obj, int j) { int o = __offset(40); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int MannualRoleGradeLength { get { int o = __offset(40); return o != 0 ? __vector_len(o) : 0; } }
  public MannualUpgradeScore GetMannualUpgradeScore(int j) { return GetMannualUpgradeScore(new MannualUpgradeScore(), j); }
  public MannualUpgradeScore GetMannualUpgradeScore(MannualUpgradeScore obj, int j) { int o = __offset(42); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int MannualUpgradeScoreLength { get { int o = __offset(42); return o != 0 ? __vector_len(o) : 0; } }
  public SkillLevel GetSkillLevel(int j) { return GetSkillLevel(new SkillLevel(), j); }
  public SkillLevel GetSkillLevel(SkillLevel obj, int j) { int o = __offset(44); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int SkillLevelLength { get { int o = __offset(44); return o != 0 ? __vector_len(o) : 0; } }
  public PlayerXPProgression GetPlayerXpProgression(int j) { return GetPlayerXpProgression(new PlayerXPProgression(), j); }
  public PlayerXPProgression GetPlayerXpProgression(PlayerXPProgression obj, int j) { int o = __offset(46); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int PlayerXpProgressionLength { get { int o = __offset(46); return o != 0 ? __vector_len(o) : 0; } }
  public ProficiencyUp GetProficiencyUp(int j) { return GetProficiencyUp(new ProficiencyUp(), j); }
  public ProficiencyUp GetProficiencyUp(ProficiencyUp obj, int j) { int o = __offset(48); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ProficiencyUpLength { get { int o = __offset(48); return o != 0 ? __vector_len(o) : 0; } }
  public ProficiencyDescribe GetProficiencyDescribe(int j) { return GetProficiencyDescribe(new ProficiencyDescribe(), j); }
  public ProficiencyDescribe GetProficiencyDescribe(ProficiencyDescribe obj, int j) { int o = __offset(50); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ProficiencyDescribeLength { get { int o = __offset(50); return o != 0 ? __vector_len(o) : 0; } }
  public SwitchCost GetSwitchCost(int j) { return GetSwitchCost(new SwitchCost(), j); }
  public SwitchCost GetSwitchCost(SwitchCost obj, int j) { int o = __offset(52); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int SwitchCostLength { get { int o = __offset(52); return o != 0 ? __vector_len(o) : 0; } }
  public HeroAwaken GetHeroAwaken(int j) { return GetHeroAwaken(new HeroAwaken(), j); }
  public HeroAwaken GetHeroAwaken(HeroAwaken obj, int j) { int o = __offset(54); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int HeroAwakenLength { get { int o = __offset(54); return o != 0 ? __vector_len(o) : 0; } }
  public HeroFetter GetHeroFetter(int j) { return GetHeroFetter(new HeroFetter(), j); }
  public HeroFetter GetHeroFetter(HeroFetter obj, int j) { int o = __offset(56); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int HeroFetterLength { get { int o = __offset(56); return o != 0 ? __vector_len(o) : 0; } }
  public HeroStrategy GetHeroStrategy(int j) { return GetHeroStrategy(new HeroStrategy(), j); }
  public HeroStrategy GetHeroStrategy(HeroStrategy obj, int j) { int o = __offset(58); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int HeroStrategyLength { get { int o = __offset(58); return o != 0 ? __vector_len(o) : 0; } }
  public Promotion GetPromotion(int j) { return GetPromotion(new Promotion(), j); }
  public Promotion GetPromotion(Promotion obj, int j) { int o = __offset(60); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int PromotionLength { get { int o = __offset(60); return o != 0 ? __vector_len(o) : 0; } }
  public PromotionTraining GetPromotionTraining(int j) { return GetPromotionTraining(new PromotionTraining(), j); }
  public PromotionTraining GetPromotionTraining(PromotionTraining obj, int j) { int o = __offset(62); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int PromotionTrainingLength { get { int o = __offset(62); return o != 0 ? __vector_len(o) : 0; } }
  public PromotionAttributeLevel GetPromotionAttributeLevel(int j) { return GetPromotionAttributeLevel(new PromotionAttributeLevel(), j); }
  public PromotionAttributeLevel GetPromotionAttributeLevel(PromotionAttributeLevel obj, int j) { int o = __offset(64); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int PromotionAttributeLevelLength { get { int o = __offset(64); return o != 0 ? __vector_len(o) : 0; } }
  public ArtifactEquipments GetArtifactEquipments(int j) { return GetArtifactEquipments(new ArtifactEquipments(), j); }
  public ArtifactEquipments GetArtifactEquipments(ArtifactEquipments obj, int j) { int o = __offset(66); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ArtifactEquipmentsLength { get { int o = __offset(66); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<ConditionCharacter> CreateConditionCharacter(FlatBufferBuilder builder,
      StringOffset _idOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      bool enabled = false,
      int priority = 0,
      VectorOffset date_conditionsOffset = default(VectorOffset),
      VectorOffset user_conditionsOffset = default(VectorOffset),
      Offset<GM.DataCache.Options> optionsOffset = default(Offset<GM.DataCache.Options>),
      VectorOffset hero_statsOffset = default(VectorOffset),
      VectorOffset hero_infosOffset = default(VectorOffset),
      VectorOffset monsterOffset = default(VectorOffset),
      VectorOffset starOffset = default(VectorOffset),
      VectorOffset upgradeOffset = default(VectorOffset),
      VectorOffset evolutionOffset = default(VectorOffset),
      VectorOffset levelOffset = default(VectorOffset),
      VectorOffset star_upOffset = default(VectorOffset),
      VectorOffset mannual_breakOffset = default(VectorOffset),
      VectorOffset level_upOffset = default(VectorOffset),
      VectorOffset mannual_scoreOffset = default(VectorOffset),
      VectorOffset mannual_role_gradeOffset = default(VectorOffset),
      VectorOffset mannual_upgrade_scoreOffset = default(VectorOffset),
      VectorOffset skill_levelOffset = default(VectorOffset),
      VectorOffset player_xp_progressionOffset = default(VectorOffset),
      VectorOffset proficiency_upOffset = default(VectorOffset),
      VectorOffset proficiency_describeOffset = default(VectorOffset),
      VectorOffset switch_costOffset = default(VectorOffset),
      VectorOffset hero_awakenOffset = default(VectorOffset),
      VectorOffset hero_fetterOffset = default(VectorOffset),
      VectorOffset hero_strategyOffset = default(VectorOffset),
      VectorOffset promotionOffset = default(VectorOffset),
      VectorOffset promotion_trainingOffset = default(VectorOffset),
      VectorOffset promotion_attribute_levelOffset = default(VectorOffset),
      VectorOffset artifact_equipmentsOffset = default(VectorOffset)) {
    builder.StartObject(32);
    ConditionCharacter.AddArtifactEquipments(builder, artifact_equipmentsOffset);
    ConditionCharacter.AddPromotionAttributeLevel(builder, promotion_attribute_levelOffset);
    ConditionCharacter.AddPromotionTraining(builder, promotion_trainingOffset);
    ConditionCharacter.AddPromotion(builder, promotionOffset);
    ConditionCharacter.AddHeroStrategy(builder, hero_strategyOffset);
    ConditionCharacter.AddHeroFetter(builder, hero_fetterOffset);
    ConditionCharacter.AddHeroAwaken(builder, hero_awakenOffset);
    ConditionCharacter.AddSwitchCost(builder, switch_costOffset);
    ConditionCharacter.AddProficiencyDescribe(builder, proficiency_describeOffset);
    ConditionCharacter.AddProficiencyUp(builder, proficiency_upOffset);
    ConditionCharacter.AddPlayerXpProgression(builder, player_xp_progressionOffset);
    ConditionCharacter.AddSkillLevel(builder, skill_levelOffset);
    ConditionCharacter.AddMannualUpgradeScore(builder, mannual_upgrade_scoreOffset);
    ConditionCharacter.AddMannualRoleGrade(builder, mannual_role_gradeOffset);
    ConditionCharacter.AddMannualScore(builder, mannual_scoreOffset);
    ConditionCharacter.AddLevelUp(builder, level_upOffset);
    ConditionCharacter.AddMannualBreak(builder, mannual_breakOffset);
    ConditionCharacter.AddStarUp(builder, star_upOffset);
    ConditionCharacter.AddLevel(builder, levelOffset);
    ConditionCharacter.AddEvolution(builder, evolutionOffset);
    ConditionCharacter.AddUpgrade(builder, upgradeOffset);
    ConditionCharacter.AddStar(builder, starOffset);
    ConditionCharacter.AddMonster(builder, monsterOffset);
    ConditionCharacter.AddHeroInfos(builder, hero_infosOffset);
    ConditionCharacter.AddHeroStats(builder, hero_statsOffset);
    ConditionCharacter.AddOptions(builder, optionsOffset);
    ConditionCharacter.AddUserConditions(builder, user_conditionsOffset);
    ConditionCharacter.AddDateConditions(builder, date_conditionsOffset);
    ConditionCharacter.AddPriority(builder, priority);
    ConditionCharacter.AddName(builder, nameOffset);
    ConditionCharacter.Add_id(builder, _idOffset);
    ConditionCharacter.AddEnabled(builder, enabled);
    return ConditionCharacter.EndConditionCharacter(builder);
  }

  public static void StartConditionCharacter(FlatBufferBuilder builder) { builder.StartObject(31); }
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
  public static void AddHeroStats(FlatBufferBuilder builder, VectorOffset heroStatsOffset) { builder.AddOffset(7, heroStatsOffset.Value, 0); }
  public static VectorOffset CreateHeroStatsVector(FlatBufferBuilder builder, Offset<HeroStat>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartHeroStatsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddHeroInfos(FlatBufferBuilder builder, VectorOffset heroInfosOffset) { builder.AddOffset(8, heroInfosOffset.Value, 0); }
  public static VectorOffset CreateHeroInfosVector(FlatBufferBuilder builder, Offset<HeroInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartHeroInfosVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddMonster(FlatBufferBuilder builder, VectorOffset monsterOffset) { builder.AddOffset(9, monsterOffset.Value, 0); }
  public static VectorOffset CreateMonsterVector(FlatBufferBuilder builder, Offset<Monster>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartMonsterVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddStar(FlatBufferBuilder builder, VectorOffset starOffset) { builder.AddOffset(10, starOffset.Value, 0); }
  public static VectorOffset CreateStarVector(FlatBufferBuilder builder, Offset<Star>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartStarVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddUpgrade(FlatBufferBuilder builder, VectorOffset upgradeOffset) { builder.AddOffset(11, upgradeOffset.Value, 0); }
  public static VectorOffset CreateUpgradeVector(FlatBufferBuilder builder, Offset<Upgrade>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartUpgradeVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddEvolution(FlatBufferBuilder builder, VectorOffset evolutionOffset) { builder.AddOffset(12, evolutionOffset.Value, 0); }
  public static VectorOffset CreateEvolutionVector(FlatBufferBuilder builder, Offset<Evolution>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartEvolutionVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddLevel(FlatBufferBuilder builder, VectorOffset levelOffset) { builder.AddOffset(13, levelOffset.Value, 0); }
  public static VectorOffset CreateLevelVector(FlatBufferBuilder builder, Offset<HeroLevel>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartLevelVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddStarUp(FlatBufferBuilder builder, VectorOffset starUpOffset) { builder.AddOffset(14, starUpOffset.Value, 0); }
  public static VectorOffset CreateStarUpVector(FlatBufferBuilder builder, Offset<StarUp>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartStarUpVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddMannualBreak(FlatBufferBuilder builder, VectorOffset mannualBreakOffset) { builder.AddOffset(15, mannualBreakOffset.Value, 0); }
  public static VectorOffset CreateMannualBreakVector(FlatBufferBuilder builder, Offset<MannualBreak>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartMannualBreakVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddLevelUp(FlatBufferBuilder builder, VectorOffset levelUpOffset) { builder.AddOffset(16, levelUpOffset.Value, 0); }
  public static VectorOffset CreateLevelUpVector(FlatBufferBuilder builder, Offset<LevelUp>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartLevelUpVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddMannualScore(FlatBufferBuilder builder, VectorOffset mannualScoreOffset) { builder.AddOffset(17, mannualScoreOffset.Value, 0); }
  public static VectorOffset CreateMannualScoreVector(FlatBufferBuilder builder, Offset<MannualScore>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartMannualScoreVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddMannualRoleGrade(FlatBufferBuilder builder, VectorOffset mannualRoleGradeOffset) { builder.AddOffset(18, mannualRoleGradeOffset.Value, 0); }
  public static VectorOffset CreateMannualRoleGradeVector(FlatBufferBuilder builder, Offset<MannualRoleGrade>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartMannualRoleGradeVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddMannualUpgradeScore(FlatBufferBuilder builder, VectorOffset mannualUpgradeScoreOffset) { builder.AddOffset(19, mannualUpgradeScoreOffset.Value, 0); }
  public static VectorOffset CreateMannualUpgradeScoreVector(FlatBufferBuilder builder, Offset<MannualUpgradeScore>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartMannualUpgradeScoreVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddSkillLevel(FlatBufferBuilder builder, VectorOffset skillLevelOffset) { builder.AddOffset(20, skillLevelOffset.Value, 0); }
  public static VectorOffset CreateSkillLevelVector(FlatBufferBuilder builder, Offset<SkillLevel>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartSkillLevelVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddPlayerXpProgression(FlatBufferBuilder builder, VectorOffset playerXpProgressionOffset) { builder.AddOffset(21, playerXpProgressionOffset.Value, 0); }
  public static VectorOffset CreatePlayerXpProgressionVector(FlatBufferBuilder builder, Offset<PlayerXPProgression>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartPlayerXpProgressionVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddProficiencyUp(FlatBufferBuilder builder, VectorOffset proficiencyUpOffset) { builder.AddOffset(22, proficiencyUpOffset.Value, 0); }
  public static VectorOffset CreateProficiencyUpVector(FlatBufferBuilder builder, Offset<ProficiencyUp>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartProficiencyUpVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddProficiencyDescribe(FlatBufferBuilder builder, VectorOffset proficiencyDescribeOffset) { builder.AddOffset(23, proficiencyDescribeOffset.Value, 0); }
  public static VectorOffset CreateProficiencyDescribeVector(FlatBufferBuilder builder, Offset<ProficiencyDescribe>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartProficiencyDescribeVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddSwitchCost(FlatBufferBuilder builder, VectorOffset switchCostOffset) { builder.AddOffset(24, switchCostOffset.Value, 0); }
  public static VectorOffset CreateSwitchCostVector(FlatBufferBuilder builder, Offset<SwitchCost>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartSwitchCostVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddHeroAwaken(FlatBufferBuilder builder, VectorOffset heroAwakenOffset) { builder.AddOffset(25, heroAwakenOffset.Value, 0); }
  public static VectorOffset CreateHeroAwakenVector(FlatBufferBuilder builder, Offset<HeroAwaken>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartHeroAwakenVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddHeroFetter(FlatBufferBuilder builder, VectorOffset heroFetterOffset) { builder.AddOffset(26, heroFetterOffset.Value, 0); }
  public static VectorOffset CreateHeroFetterVector(FlatBufferBuilder builder, Offset<HeroFetter>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartHeroFetterVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddHeroStrategy(FlatBufferBuilder builder, VectorOffset heroStrategyOffset) { builder.AddOffset(27, heroStrategyOffset.Value, 0); }
  public static VectorOffset CreateHeroStrategyVector(FlatBufferBuilder builder, Offset<HeroStrategy>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartHeroStrategyVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddPromotion(FlatBufferBuilder builder, VectorOffset promotionOffset) { builder.AddOffset(28, promotionOffset.Value, 0); }
  public static VectorOffset CreatePromotionVector(FlatBufferBuilder builder, Offset<Promotion>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartPromotionVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddPromotionTraining(FlatBufferBuilder builder, VectorOffset promotionTrainingOffset) { builder.AddOffset(29, promotionTrainingOffset.Value, 0); }
  public static VectorOffset CreatePromotionTrainingVector(FlatBufferBuilder builder, Offset<PromotionTraining>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartPromotionTrainingVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddPromotionAttributeLevel(FlatBufferBuilder builder, VectorOffset promotionAttributeLevelOffset) { builder.AddOffset(30, promotionAttributeLevelOffset.Value, 0); }
  public static VectorOffset CreatePromotionAttributeLevelVector(FlatBufferBuilder builder, Offset<PromotionAttributeLevel>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartPromotionAttributeLevelVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddArtifactEquipments(FlatBufferBuilder builder, VectorOffset artifactEquipmentsOffset) { builder.AddOffset(31, artifactEquipmentsOffset.Value, 0); }
  public static VectorOffset CreateArtifactEquipmentsVector(FlatBufferBuilder builder, Offset<ArtifactEquipments>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartArtifactEquipmentsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<ConditionCharacter> EndConditionCharacter(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ConditionCharacter>(o);
  }
};

public sealed class Character : Table {
  public static Character GetRootAsCharacter(ByteBuffer _bb) { return GetRootAsCharacter(_bb, new Character()); }
  public static Character GetRootAsCharacter(ByteBuffer _bb, Character obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Character __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public ConditionCharacter GetArray(int j) { return GetArray(new ConditionCharacter(), j); }
  public ConditionCharacter GetArray(ConditionCharacter obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ArrayLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<Character> CreateCharacter(FlatBufferBuilder builder,
      VectorOffset arrayOffset = default(VectorOffset)) {
    builder.StartObject(1);
    Character.AddArray(builder, arrayOffset);
    return Character.EndCharacter(builder);
  }

  public static void StartCharacter(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddArray(FlatBufferBuilder builder, VectorOffset arrayOffset) { builder.AddOffset(0, arrayOffset.Value, 0); }
  public static VectorOffset CreateArrayVector(FlatBufferBuilder builder, Offset<ConditionCharacter>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartArrayVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<Character> EndCharacter(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Character>(o);
  }
  public static void FinishCharacterBuffer(FlatBufferBuilder builder, Offset<Character> offset) { builder.Finish(offset.Value); }
};


}
