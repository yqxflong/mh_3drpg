// automatically generated, do not modify

namespace GM.DataCache
{

using System;
using FlatBuffers;

public sealed class SkillTemplate : Table {
  public static SkillTemplate GetRootAsSkillTemplate(ByteBuffer _bb) { return GetRootAsSkillTemplate(_bb, new SkillTemplate()); }
  public static SkillTemplate GetRootAsSkillTemplate(ByteBuffer _bb, SkillTemplate obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public SkillTemplate __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Name { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(6); }
  public string Description { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDescriptionBytes() { return __vector_as_arraysegment(8); }
  public sbyte SkillType { get { int o = __offset(10); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public string Icon { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIconBytes() { return __vector_as_arraysegment(12); }
  public sbyte Target { get { int o = __offset(14); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public int HPCost { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SPGen { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SPGenProb { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SPCost { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SPCostProb { get { int o = __offset(24); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public sbyte CombatInitCooldown { get { int o = __offset(26); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public sbyte CooldownTurnNum { get { int o = __offset(28); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public bool TriggerChain { get { int o = __offset(30); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool IsMustHit { get { int o = __offset(32); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int SkillImpact1 { get { int o = __offset(34); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SkillImpact2 { get { int o = __offset(36); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SkillImpact3 { get { int o = __offset(38); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SkillImpact4 { get { int o = __offset(40); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SkillImpact5 { get { int o = __offset(42); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public bool HideOthers { get { int o = __offset(44); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public sbyte FadeEnvironment { get { int o = __offset(46); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public string Action { get { int o = __offset(48); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetActionBytes() { return __vector_as_arraysegment(48); }
  public sbyte SkillPosition { get { int o = __offset(50); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public int TargetDistance { get { int o = __offset(52); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string AutoStrategy { get { int o = __offset(54); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetAutoStrategyBytes() { return __vector_as_arraysegment(54); }
  public string BuffDescribleId { get { int o = __offset(56); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBuffDescribleIdBytes() { return __vector_as_arraysegment(56); }
  public float BattleRating { get { int o = __offset(58); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<SkillTemplate> CreateSkillTemplate(FlatBufferBuilder builder,
      int id = 0,
      StringOffset nameOffset = default(StringOffset),
      StringOffset descriptionOffset = default(StringOffset),
      sbyte skill_type = 0,
      StringOffset iconOffset = default(StringOffset),
      sbyte target = 0,
      int HP_cost = 0,
      int SP_gen = 0,
      int SP_gen_prob = 0,
      int SP_cost = 0,
      int SP_cost_prob = 0,
      sbyte combat_init_cooldown = 0,
      sbyte cooldown_turn_num = 0,
      bool trigger_chain = false,
      bool is_must_hit = false,
      int skill_impact_1 = 0,
      int skill_impact_2 = 0,
      int skill_impact_3 = 0,
      int skill_impact_4 = 0,
      int skill_impact_5 = 0,
      bool hide_others = false,
      sbyte fade_environment = 0,
      StringOffset actionOffset = default(StringOffset),
      sbyte skill_position = 0,
      int target_distance = 0,
      StringOffset auto_strategyOffset = default(StringOffset),
      StringOffset buff_describle_idOffset = default(StringOffset),
      float battle_rating = 0) {
    builder.StartObject(28);
    SkillTemplate.AddBattleRating(builder, battle_rating);
    SkillTemplate.AddBuffDescribleId(builder, buff_describle_idOffset);
    SkillTemplate.AddAutoStrategy(builder, auto_strategyOffset);
    SkillTemplate.AddTargetDistance(builder, target_distance);
    SkillTemplate.AddAction(builder, actionOffset);
    SkillTemplate.AddSkillImpact5(builder, skill_impact_5);
    SkillTemplate.AddSkillImpact4(builder, skill_impact_4);
    SkillTemplate.AddSkillImpact3(builder, skill_impact_3);
    SkillTemplate.AddSkillImpact2(builder, skill_impact_2);
    SkillTemplate.AddSkillImpact1(builder, skill_impact_1);
    SkillTemplate.AddSPCostProb(builder, SP_cost_prob);
    SkillTemplate.AddSPCost(builder, SP_cost);
    SkillTemplate.AddSPGenProb(builder, SP_gen_prob);
    SkillTemplate.AddSPGen(builder, SP_gen);
    SkillTemplate.AddHPCost(builder, HP_cost);
    SkillTemplate.AddIcon(builder, iconOffset);
    SkillTemplate.AddDescription(builder, descriptionOffset);
    SkillTemplate.AddName(builder, nameOffset);
    SkillTemplate.AddId(builder, id);
    SkillTemplate.AddSkillPosition(builder, skill_position);
    SkillTemplate.AddFadeEnvironment(builder, fade_environment);
    SkillTemplate.AddHideOthers(builder, hide_others);
    SkillTemplate.AddIsMustHit(builder, is_must_hit);
    SkillTemplate.AddTriggerChain(builder, trigger_chain);
    SkillTemplate.AddCooldownTurnNum(builder, cooldown_turn_num);
    SkillTemplate.AddCombatInitCooldown(builder, combat_init_cooldown);
    SkillTemplate.AddTarget(builder, target);
    SkillTemplate.AddSkillType(builder, skill_type);
    return SkillTemplate.EndSkillTemplate(builder);
  }

  public static void StartSkillTemplate(FlatBufferBuilder builder) { builder.StartObject(28); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(1, nameOffset.Value, 0); }
  public static void AddDescription(FlatBufferBuilder builder, StringOffset descriptionOffset) { builder.AddOffset(2, descriptionOffset.Value, 0); }
  public static void AddSkillType(FlatBufferBuilder builder, sbyte skillType) { builder.AddSbyte(3, skillType, 0); }
  public static void AddIcon(FlatBufferBuilder builder, StringOffset iconOffset) { builder.AddOffset(4, iconOffset.Value, 0); }
  public static void AddTarget(FlatBufferBuilder builder, sbyte target) { builder.AddSbyte(5, target, 0); }
  public static void AddHPCost(FlatBufferBuilder builder, int HPCost) { builder.AddInt(6, HPCost, 0); }
  public static void AddSPGen(FlatBufferBuilder builder, int SPGen) { builder.AddInt(7, SPGen, 0); }
  public static void AddSPGenProb(FlatBufferBuilder builder, int SPGenProb) { builder.AddInt(8, SPGenProb, 0); }
  public static void AddSPCost(FlatBufferBuilder builder, int SPCost) { builder.AddInt(9, SPCost, 0); }
  public static void AddSPCostProb(FlatBufferBuilder builder, int SPCostProb) { builder.AddInt(10, SPCostProb, 0); }
  public static void AddCombatInitCooldown(FlatBufferBuilder builder, sbyte combatInitCooldown) { builder.AddSbyte(11, combatInitCooldown, 0); }
  public static void AddCooldownTurnNum(FlatBufferBuilder builder, sbyte cooldownTurnNum) { builder.AddSbyte(12, cooldownTurnNum, 0); }
  public static void AddTriggerChain(FlatBufferBuilder builder, bool triggerChain) { builder.AddBool(13, triggerChain, false); }
  public static void AddIsMustHit(FlatBufferBuilder builder, bool isMustHit) { builder.AddBool(14, isMustHit, false); }
  public static void AddSkillImpact1(FlatBufferBuilder builder, int skillImpact1) { builder.AddInt(15, skillImpact1, 0); }
  public static void AddSkillImpact2(FlatBufferBuilder builder, int skillImpact2) { builder.AddInt(16, skillImpact2, 0); }
  public static void AddSkillImpact3(FlatBufferBuilder builder, int skillImpact3) { builder.AddInt(17, skillImpact3, 0); }
  public static void AddSkillImpact4(FlatBufferBuilder builder, int skillImpact4) { builder.AddInt(18, skillImpact4, 0); }
  public static void AddSkillImpact5(FlatBufferBuilder builder, int skillImpact5) { builder.AddInt(19, skillImpact5, 0); }
  public static void AddHideOthers(FlatBufferBuilder builder, bool hideOthers) { builder.AddBool(20, hideOthers, false); }
  public static void AddFadeEnvironment(FlatBufferBuilder builder, sbyte fadeEnvironment) { builder.AddSbyte(21, fadeEnvironment, 0); }
  public static void AddAction(FlatBufferBuilder builder, StringOffset actionOffset) { builder.AddOffset(22, actionOffset.Value, 0); }
  public static void AddSkillPosition(FlatBufferBuilder builder, sbyte skillPosition) { builder.AddSbyte(23, skillPosition, 0); }
  public static void AddTargetDistance(FlatBufferBuilder builder, int targetDistance) { builder.AddInt(24, targetDistance, 0); }
  public static void AddAutoStrategy(FlatBufferBuilder builder, StringOffset autoStrategyOffset) { builder.AddOffset(25, autoStrategyOffset.Value, 0); }
  public static void AddBuffDescribleId(FlatBufferBuilder builder, StringOffset buffDescribleIdOffset) { builder.AddOffset(26, buffDescribleIdOffset.Value, 0); }
  public static void AddBattleRating(FlatBufferBuilder builder, float battleRating) { builder.AddFloat(27, battleRating, 0); }
  public static Offset<SkillTemplate> EndSkillTemplate(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<SkillTemplate>(o);
  }
};

public sealed class ImpactsTemplate : Table {
  public static ImpactsTemplate GetRootAsImpactsTemplate(ByteBuffer _bb) { return GetRootAsImpactsTemplate(_bb, new ImpactsTemplate()); }
  public static ImpactsTemplate GetRootAsImpactsTemplate(ByteBuffer _bb, ImpactsTemplate obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ImpactsTemplate __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int ImpactId { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int LogicId { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float Param1 { get { int o = __offset(8); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float Param2 { get { int o = __offset(10); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float Param3 { get { int o = __offset(12); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float Param4 { get { int o = __offset(14); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float Param5 { get { int o = __offset(16); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float Upgrade1 { get { int o = __offset(18); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float Upgrade2 { get { int o = __offset(20); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float Upgrade3 { get { int o = __offset(22); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float Upgrade4 { get { int o = __offset(24); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float Upgrade5 { get { int o = __offset(26); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<ImpactsTemplate> CreateImpactsTemplate(FlatBufferBuilder builder,
      int impact_id = 0,
      int logic_id = 0,
      float param_1 = 0,
      float param_2 = 0,
      float param_3 = 0,
      float param_4 = 0,
      float param_5 = 0,
      float upgrade_1 = 0,
      float upgrade_2 = 0,
      float upgrade_3 = 0,
      float upgrade_4 = 0,
      float upgrade_5 = 0) {
    builder.StartObject(12);
    ImpactsTemplate.AddUpgrade5(builder, upgrade_5);
    ImpactsTemplate.AddUpgrade4(builder, upgrade_4);
    ImpactsTemplate.AddUpgrade3(builder, upgrade_3);
    ImpactsTemplate.AddUpgrade2(builder, upgrade_2);
    ImpactsTemplate.AddUpgrade1(builder, upgrade_1);
    ImpactsTemplate.AddParam5(builder, param_5);
    ImpactsTemplate.AddParam4(builder, param_4);
    ImpactsTemplate.AddParam3(builder, param_3);
    ImpactsTemplate.AddParam2(builder, param_2);
    ImpactsTemplate.AddParam1(builder, param_1);
    ImpactsTemplate.AddLogicId(builder, logic_id);
    ImpactsTemplate.AddImpactId(builder, impact_id);
    return ImpactsTemplate.EndImpactsTemplate(builder);
  }

  public static void StartImpactsTemplate(FlatBufferBuilder builder) { builder.StartObject(12); }
  public static void AddImpactId(FlatBufferBuilder builder, int impactId) { builder.AddInt(0, impactId, 0); }
  public static void AddLogicId(FlatBufferBuilder builder, int logicId) { builder.AddInt(1, logicId, 0); }
  public static void AddParam1(FlatBufferBuilder builder, float param1) { builder.AddFloat(2, param1, 0); }
  public static void AddParam2(FlatBufferBuilder builder, float param2) { builder.AddFloat(3, param2, 0); }
  public static void AddParam3(FlatBufferBuilder builder, float param3) { builder.AddFloat(4, param3, 0); }
  public static void AddParam4(FlatBufferBuilder builder, float param4) { builder.AddFloat(5, param4, 0); }
  public static void AddParam5(FlatBufferBuilder builder, float param5) { builder.AddFloat(6, param5, 0); }
  public static void AddUpgrade1(FlatBufferBuilder builder, float upgrade1) { builder.AddFloat(7, upgrade1, 0); }
  public static void AddUpgrade2(FlatBufferBuilder builder, float upgrade2) { builder.AddFloat(8, upgrade2, 0); }
  public static void AddUpgrade3(FlatBufferBuilder builder, float upgrade3) { builder.AddFloat(9, upgrade3, 0); }
  public static void AddUpgrade4(FlatBufferBuilder builder, float upgrade4) { builder.AddFloat(10, upgrade4, 0); }
  public static void AddUpgrade5(FlatBufferBuilder builder, float upgrade5) { builder.AddFloat(11, upgrade5, 0); }
  public static Offset<ImpactsTemplate> EndImpactsTemplate(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ImpactsTemplate>(o);
  }
};

public sealed class BuffTemplate : Table {
  public static BuffTemplate GetRootAsBuffTemplate(ByteBuffer _bb) { return GetRootAsBuffTemplate(_bb, new BuffTemplate()); }
  public static BuffTemplate GetRootAsBuffTemplate(ByteBuffer _bb, BuffTemplate obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public BuffTemplate __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Name { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(6); }
  public string Description { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDescriptionBytes() { return __vector_as_arraysegment(8); }
  public int BuffType { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int EffectTurns { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int OtherInterruptCondition { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)3; } }
  public int CanOverlap { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)1; } }
  public int StackNum { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)1; } }
  public int Priority { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SkillImpact1 { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SkillImpact2 { get { int o = __offset(24); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SkillImpact3 { get { int o = __offset(26); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SkillImpact4 { get { int o = __offset(28); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SkillImpact5 { get { int o = __offset(30); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Buff { get { int o = __offset(32); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBuffBytes() { return __vector_as_arraysegment(32); }
  public string OnceFX { get { int o = __offset(34); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetOnceFXBytes() { return __vector_as_arraysegment(34); }
  public int OnceFXAttachment { get { int o = __offset(36); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string LoopFX { get { int o = __offset(38); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetLoopFXBytes() { return __vector_as_arraysegment(38); }
  public int LoopFXAttachment { get { int o = __offset(40); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SpecialState { get { int o = __offset(42); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string BuffFloatFont { get { int o = __offset(44); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBuffFloatFontBytes() { return __vector_as_arraysegment(44); }
  public int BuffShow { get { int o = __offset(46); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<BuffTemplate> CreateBuffTemplate(FlatBufferBuilder builder,
      int id = 0,
      StringOffset nameOffset = default(StringOffset),
      StringOffset descriptionOffset = default(StringOffset),
      int buff_type = 0,
      int effect_turns = 0,
      int other_interrupt_condition = 3,
      int can_overlap = 1,
      int stack_num = 1,
      int priority = 0,
      int skill_impact_1 = 0,
      int skill_impact_2 = 0,
      int skill_impact_3 = 0,
      int skill_impact_4 = 0,
      int skill_impact_5 = 0,
      StringOffset buffOffset = default(StringOffset),
      StringOffset once_FXOffset = default(StringOffset),
      int once_FX_attachment = 0,
      StringOffset loop_FXOffset = default(StringOffset),
      int loop_FX_attachment = 0,
      int special_state = 0,
      StringOffset buff_float_fontOffset = default(StringOffset),
      int buff_show = 0) {
    builder.StartObject(22);
    BuffTemplate.AddBuffShow(builder, buff_show);
    BuffTemplate.AddBuffFloatFont(builder, buff_float_fontOffset);
    BuffTemplate.AddSpecialState(builder, special_state);
    BuffTemplate.AddLoopFXAttachment(builder, loop_FX_attachment);
    BuffTemplate.AddLoopFX(builder, loop_FXOffset);
    BuffTemplate.AddOnceFXAttachment(builder, once_FX_attachment);
    BuffTemplate.AddOnceFX(builder, once_FXOffset);
    BuffTemplate.AddBuff(builder, buffOffset);
    BuffTemplate.AddSkillImpact5(builder, skill_impact_5);
    BuffTemplate.AddSkillImpact4(builder, skill_impact_4);
    BuffTemplate.AddSkillImpact3(builder, skill_impact_3);
    BuffTemplate.AddSkillImpact2(builder, skill_impact_2);
    BuffTemplate.AddSkillImpact1(builder, skill_impact_1);
    BuffTemplate.AddPriority(builder, priority);
    BuffTemplate.AddStackNum(builder, stack_num);
    BuffTemplate.AddCanOverlap(builder, can_overlap);
    BuffTemplate.AddOtherInterruptCondition(builder, other_interrupt_condition);
    BuffTemplate.AddEffectTurns(builder, effect_turns);
    BuffTemplate.AddBuffType(builder, buff_type);
    BuffTemplate.AddDescription(builder, descriptionOffset);
    BuffTemplate.AddName(builder, nameOffset);
    BuffTemplate.AddId(builder, id);
    return BuffTemplate.EndBuffTemplate(builder);
  }

  public static void StartBuffTemplate(FlatBufferBuilder builder) { builder.StartObject(22); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(1, nameOffset.Value, 0); }
  public static void AddDescription(FlatBufferBuilder builder, StringOffset descriptionOffset) { builder.AddOffset(2, descriptionOffset.Value, 0); }
  public static void AddBuffType(FlatBufferBuilder builder, int buffType) { builder.AddInt(3, buffType, 0); }
  public static void AddEffectTurns(FlatBufferBuilder builder, int effectTurns) { builder.AddInt(4, effectTurns, 0); }
  public static void AddOtherInterruptCondition(FlatBufferBuilder builder, int otherInterruptCondition) { builder.AddInt(5, otherInterruptCondition, 3); }
  public static void AddCanOverlap(FlatBufferBuilder builder, int canOverlap) { builder.AddInt(6, canOverlap, 1); }
  public static void AddStackNum(FlatBufferBuilder builder, int stackNum) { builder.AddInt(7, stackNum, 1); }
  public static void AddPriority(FlatBufferBuilder builder, int priority) { builder.AddInt(8, priority, 0); }
  public static void AddSkillImpact1(FlatBufferBuilder builder, int skillImpact1) { builder.AddInt(9, skillImpact1, 0); }
  public static void AddSkillImpact2(FlatBufferBuilder builder, int skillImpact2) { builder.AddInt(10, skillImpact2, 0); }
  public static void AddSkillImpact3(FlatBufferBuilder builder, int skillImpact3) { builder.AddInt(11, skillImpact3, 0); }
  public static void AddSkillImpact4(FlatBufferBuilder builder, int skillImpact4) { builder.AddInt(12, skillImpact4, 0); }
  public static void AddSkillImpact5(FlatBufferBuilder builder, int skillImpact5) { builder.AddInt(13, skillImpact5, 0); }
  public static void AddBuff(FlatBufferBuilder builder, StringOffset buffOffset) { builder.AddOffset(14, buffOffset.Value, 0); }
  public static void AddOnceFX(FlatBufferBuilder builder, StringOffset onceFXOffset) { builder.AddOffset(15, onceFXOffset.Value, 0); }
  public static void AddOnceFXAttachment(FlatBufferBuilder builder, int onceFXAttachment) { builder.AddInt(16, onceFXAttachment, 0); }
  public static void AddLoopFX(FlatBufferBuilder builder, StringOffset loopFXOffset) { builder.AddOffset(17, loopFXOffset.Value, 0); }
  public static void AddLoopFXAttachment(FlatBufferBuilder builder, int loopFXAttachment) { builder.AddInt(18, loopFXAttachment, 0); }
  public static void AddSpecialState(FlatBufferBuilder builder, int specialState) { builder.AddInt(19, specialState, 0); }
  public static void AddBuffFloatFont(FlatBufferBuilder builder, StringOffset buffFloatFontOffset) { builder.AddOffset(20, buffFloatFontOffset.Value, 0); }
  public static void AddBuffShow(FlatBufferBuilder builder, int buffShow) { builder.AddInt(21, buffShow, 0); }
  public static Offset<BuffTemplate> EndBuffTemplate(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<BuffTemplate>(o);
  }
};

public sealed class SkillLevelUp : Table {
  public static SkillLevelUp GetRootAsSkillLevelUp(ByteBuffer _bb) { return GetRootAsSkillLevelUp(_bb, new SkillLevelUp()); }
  public static SkillLevelUp GetRootAsSkillLevelUp(ByteBuffer _bb, SkillLevelUp obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public SkillLevelUp __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ReduceCdRating { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetReduceCdRatingBytes() { return __vector_as_arraysegment(6); }
  public float Rating { get { int o = __offset(8); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float DamageIncPercent { get { int o = __offset(10); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float ATKIncPercent { get { int o = __offset(12); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float DEFIncPercent { get { int o = __offset(14); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float MaxHPIncPercent { get { int o = __offset(16); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<SkillLevelUp> CreateSkillLevelUp(FlatBufferBuilder builder,
      int id = 0,
      StringOffset reduce_cd_ratingOffset = default(StringOffset),
      float rating = 0,
      float damage_inc_percent = 0,
      float ATK_inc_percent = 0,
      float DEF_inc_percent = 0,
      float MaxHP_inc_percent = 0) {
    builder.StartObject(7);
    SkillLevelUp.AddMaxHPIncPercent(builder, MaxHP_inc_percent);
    SkillLevelUp.AddDEFIncPercent(builder, DEF_inc_percent);
    SkillLevelUp.AddATKIncPercent(builder, ATK_inc_percent);
    SkillLevelUp.AddDamageIncPercent(builder, damage_inc_percent);
    SkillLevelUp.AddRating(builder, rating);
    SkillLevelUp.AddReduceCdRating(builder, reduce_cd_ratingOffset);
    SkillLevelUp.AddId(builder, id);
    return SkillLevelUp.EndSkillLevelUp(builder);
  }

  public static void StartSkillLevelUp(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddReduceCdRating(FlatBufferBuilder builder, StringOffset reduceCdRatingOffset) { builder.AddOffset(1, reduceCdRatingOffset.Value, 0); }
  public static void AddRating(FlatBufferBuilder builder, float rating) { builder.AddFloat(2, rating, 0); }
  public static void AddDamageIncPercent(FlatBufferBuilder builder, float damageIncPercent) { builder.AddFloat(3, damageIncPercent, 0); }
  public static void AddATKIncPercent(FlatBufferBuilder builder, float ATKIncPercent) { builder.AddFloat(4, ATKIncPercent, 0); }
  public static void AddDEFIncPercent(FlatBufferBuilder builder, float DEFIncPercent) { builder.AddFloat(5, DEFIncPercent, 0); }
  public static void AddMaxHPIncPercent(FlatBufferBuilder builder, float MaxHPIncPercent) { builder.AddFloat(6, MaxHPIncPercent, 0); }
  public static Offset<SkillLevelUp> EndSkillLevelUp(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<SkillLevelUp>(o);
  }
};

public sealed class ConditionCombat : Table {
  public static ConditionCombat GetRootAsConditionCombat(ByteBuffer _bb) { return GetRootAsConditionCombat(_bb, new ConditionCombat()); }
  public static ConditionCombat GetRootAsConditionCombat(ByteBuffer _bb, ConditionCombat obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ConditionCombat __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string _id { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? Get_idBytes() { return __vector_as_arraysegment(4); }
  public string Name { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(6); }
  public bool Enabled { get { int o = __offset(8); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)true; } }
  public int Priority { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)1; } }
  public GM.DataCache.DateCondition GetDateConditions(int j) { return GetDateConditions(new GM.DataCache.DateCondition(), j); }
  public GM.DataCache.DateCondition GetDateConditions(GM.DataCache.DateCondition obj, int j) { int o = __offset(12); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int DateConditionsLength { get { int o = __offset(12); return o != 0 ? __vector_len(o) : 0; } }
  public GM.DataCache.UserCondition GetUserConditions(int j) { return GetUserConditions(new GM.DataCache.UserCondition(), j); }
  public GM.DataCache.UserCondition GetUserConditions(GM.DataCache.UserCondition obj, int j) { int o = __offset(14); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int UserConditionsLength { get { int o = __offset(14); return o != 0 ? __vector_len(o) : 0; } }
  public GM.DataCache.Options Options { get { return GetOptions(new GM.DataCache.Options()); } }
  public GM.DataCache.Options GetOptions(GM.DataCache.Options obj) { int o = __offset(16); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public SkillTemplate GetSkills(int j) { return GetSkills(new SkillTemplate(), j); }
  public SkillTemplate GetSkills(SkillTemplate obj, int j) { int o = __offset(18); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int SkillsLength { get { int o = __offset(18); return o != 0 ? __vector_len(o) : 0; } }
  public BuffTemplate GetBuffs(int j) { return GetBuffs(new BuffTemplate(), j); }
  public BuffTemplate GetBuffs(BuffTemplate obj, int j) { int o = __offset(20); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int BuffsLength { get { int o = __offset(20); return o != 0 ? __vector_len(o) : 0; } }
  public ImpactsTemplate GetImpacts(int j) { return GetImpacts(new ImpactsTemplate(), j); }
  public ImpactsTemplate GetImpacts(ImpactsTemplate obj, int j) { int o = __offset(22); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ImpactsLength { get { int o = __offset(22); return o != 0 ? __vector_len(o) : 0; } }
  public SkillLevelUp GetSkillLevelUp(int j) { return GetSkillLevelUp(new SkillLevelUp(), j); }
  public SkillLevelUp GetSkillLevelUp(SkillLevelUp obj, int j) { int o = __offset(24); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int SkillLevelUpLength { get { int o = __offset(24); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<ConditionCombat> CreateConditionCombat(FlatBufferBuilder builder,
      StringOffset _idOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      bool enabled = true,
      int priority = 1,
      VectorOffset date_conditionsOffset = default(VectorOffset),
      VectorOffset user_conditionsOffset = default(VectorOffset),
      Offset<GM.DataCache.Options> optionsOffset = default(Offset<GM.DataCache.Options>),
      VectorOffset skillsOffset = default(VectorOffset),
      VectorOffset buffsOffset = default(VectorOffset),
      VectorOffset impactsOffset = default(VectorOffset),
      VectorOffset skill_level_upOffset = default(VectorOffset)) {
    builder.StartObject(11);
    ConditionCombat.AddSkillLevelUp(builder, skill_level_upOffset);
    ConditionCombat.AddImpacts(builder, impactsOffset);
    ConditionCombat.AddBuffs(builder, buffsOffset);
    ConditionCombat.AddSkills(builder, skillsOffset);
    ConditionCombat.AddOptions(builder, optionsOffset);
    ConditionCombat.AddUserConditions(builder, user_conditionsOffset);
    ConditionCombat.AddDateConditions(builder, date_conditionsOffset);
    ConditionCombat.AddPriority(builder, priority);
    ConditionCombat.AddName(builder, nameOffset);
    ConditionCombat.Add_id(builder, _idOffset);
    ConditionCombat.AddEnabled(builder, enabled);
    return ConditionCombat.EndConditionCombat(builder);
  }

  public static void StartConditionCombat(FlatBufferBuilder builder) { builder.StartObject(11); }
  public static void Add_id(FlatBufferBuilder builder, StringOffset IdOffset) { builder.AddOffset(0, IdOffset.Value, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(1, nameOffset.Value, 0); }
  public static void AddEnabled(FlatBufferBuilder builder, bool enabled) { builder.AddBool(2, enabled, true); }
  public static void AddPriority(FlatBufferBuilder builder, int priority) { builder.AddInt(3, priority, 1); }
  public static void AddDateConditions(FlatBufferBuilder builder, VectorOffset dateConditionsOffset) { builder.AddOffset(4, dateConditionsOffset.Value, 0); }
  public static VectorOffset CreateDateConditionsVector(FlatBufferBuilder builder, Offset<GM.DataCache.DateCondition>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartDateConditionsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddUserConditions(FlatBufferBuilder builder, VectorOffset userConditionsOffset) { builder.AddOffset(5, userConditionsOffset.Value, 0); }
  public static VectorOffset CreateUserConditionsVector(FlatBufferBuilder builder, Offset<GM.DataCache.UserCondition>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartUserConditionsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddOptions(FlatBufferBuilder builder, Offset<GM.DataCache.Options> optionsOffset) { builder.AddOffset(6, optionsOffset.Value, 0); }
  public static void AddSkills(FlatBufferBuilder builder, VectorOffset skillsOffset) { builder.AddOffset(7, skillsOffset.Value, 0); }
  public static VectorOffset CreateSkillsVector(FlatBufferBuilder builder, Offset<SkillTemplate>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartSkillsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddBuffs(FlatBufferBuilder builder, VectorOffset buffsOffset) { builder.AddOffset(8, buffsOffset.Value, 0); }
  public static VectorOffset CreateBuffsVector(FlatBufferBuilder builder, Offset<BuffTemplate>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartBuffsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddImpacts(FlatBufferBuilder builder, VectorOffset impactsOffset) { builder.AddOffset(9, impactsOffset.Value, 0); }
  public static VectorOffset CreateImpactsVector(FlatBufferBuilder builder, Offset<ImpactsTemplate>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartImpactsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddSkillLevelUp(FlatBufferBuilder builder, VectorOffset skillLevelUpOffset) { builder.AddOffset(10, skillLevelUpOffset.Value, 0); }
  public static VectorOffset CreateSkillLevelUpVector(FlatBufferBuilder builder, Offset<SkillLevelUp>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartSkillLevelUpVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<ConditionCombat> EndConditionCombat(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ConditionCombat>(o);
  }
};

public sealed class Combat : Table {
  public static Combat GetRootAsCombat(ByteBuffer _bb) { return GetRootAsCombat(_bb, new Combat()); }
  public static Combat GetRootAsCombat(ByteBuffer _bb, Combat obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Combat __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public ConditionCombat GetArray(int j) { return GetArray(new ConditionCombat(), j); }
  public ConditionCombat GetArray(ConditionCombat obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ArrayLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<Combat> CreateCombat(FlatBufferBuilder builder,
      VectorOffset arrayOffset = default(VectorOffset)) {
    builder.StartObject(1);
    Combat.AddArray(builder, arrayOffset);
    return Combat.EndCombat(builder);
  }

  public static void StartCombat(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddArray(FlatBufferBuilder builder, VectorOffset arrayOffset) { builder.AddOffset(0, arrayOffset.Value, 0); }
  public static VectorOffset CreateArrayVector(FlatBufferBuilder builder, Offset<ConditionCombat>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartArrayVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<Combat> EndCombat(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Combat>(o);
  }
  public static void FinishCombatBuffer(FlatBufferBuilder builder, Offset<Combat> offset) { builder.Finish(offset.Value); }
};


}
