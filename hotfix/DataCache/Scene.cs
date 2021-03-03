// automatically generated, do not modify

namespace GM.DataCache
{

using System;
using FlatBuffers;

public sealed class LayoutInfo : Table {
  public static LayoutInfo GetRootAsLayoutInfo(ByteBuffer _bb) { return GetRootAsLayoutInfo(_bb, new LayoutInfo()); }
  public static LayoutInfo GetRootAsLayoutInfo(ByteBuffer _bb, LayoutInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public LayoutInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string CombatLayoutName { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetCombatLayoutNameBytes() { return __vector_as_arraysegment(4); }
  public int Wave { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Layout { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetLayoutBytes() { return __vector_as_arraysegment(8); }
  public string F1 { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetF1Bytes() { return __vector_as_arraysegment(10); }
  public string F2 { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetF2Bytes() { return __vector_as_arraysegment(12); }
  public string F3 { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetF3Bytes() { return __vector_as_arraysegment(14); }
  public string M1 { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetM1Bytes() { return __vector_as_arraysegment(16); }
  public string M2 { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetM2Bytes() { return __vector_as_arraysegment(18); }
  public string M3 { get { int o = __offset(20); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetM3Bytes() { return __vector_as_arraysegment(20); }
  public string B1 { get { int o = __offset(22); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetB1Bytes() { return __vector_as_arraysegment(22); }
  public string B2 { get { int o = __offset(24); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetB2Bytes() { return __vector_as_arraysegment(24); }
  public string B3 { get { int o = __offset(26); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetB3Bytes() { return __vector_as_arraysegment(26); }
  public string IconTemplateId { get { int o = __offset(28); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIconTemplateIdBytes() { return __vector_as_arraysegment(28); }
  public string CombatAI { get { int o = __offset(30); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetCombatAIBytes() { return __vector_as_arraysegment(30); }
  public string Model { get { int o = __offset(32); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetModelBytes() { return __vector_as_arraysegment(32); }

  public static Offset<LayoutInfo> CreateLayoutInfo(FlatBufferBuilder builder,
      StringOffset combat_layout_nameOffset = default(StringOffset),
      int wave = 0,
      StringOffset layoutOffset = default(StringOffset),
      StringOffset f1Offset = default(StringOffset),
      StringOffset f2Offset = default(StringOffset),
      StringOffset f3Offset = default(StringOffset),
      StringOffset m1Offset = default(StringOffset),
      StringOffset m2Offset = default(StringOffset),
      StringOffset m3Offset = default(StringOffset),
      StringOffset b1Offset = default(StringOffset),
      StringOffset b2Offset = default(StringOffset),
      StringOffset b3Offset = default(StringOffset),
      StringOffset icon_template_idOffset = default(StringOffset),
      StringOffset combat_aIOffset = default(StringOffset),
      StringOffset modelOffset = default(StringOffset)) {
    builder.StartObject(15);
    LayoutInfo.AddModel(builder, modelOffset);
    LayoutInfo.AddCombatAI(builder, combat_aIOffset);
    LayoutInfo.AddIconTemplateId(builder, icon_template_idOffset);
    LayoutInfo.AddB3(builder, b3Offset);
    LayoutInfo.AddB2(builder, b2Offset);
    LayoutInfo.AddB1(builder, b1Offset);
    LayoutInfo.AddM3(builder, m3Offset);
    LayoutInfo.AddM2(builder, m2Offset);
    LayoutInfo.AddM1(builder, m1Offset);
    LayoutInfo.AddF3(builder, f3Offset);
    LayoutInfo.AddF2(builder, f2Offset);
    LayoutInfo.AddF1(builder, f1Offset);
    LayoutInfo.AddLayout(builder, layoutOffset);
    LayoutInfo.AddWave(builder, wave);
    LayoutInfo.AddCombatLayoutName(builder, combat_layout_nameOffset);
    return LayoutInfo.EndLayoutInfo(builder);
  }

  public static void StartLayoutInfo(FlatBufferBuilder builder) { builder.StartObject(15); }
  public static void AddCombatLayoutName(FlatBufferBuilder builder, StringOffset combatLayoutNameOffset) { builder.AddOffset(0, combatLayoutNameOffset.Value, 0); }
  public static void AddWave(FlatBufferBuilder builder, int wave) { builder.AddInt(1, wave, 0); }
  public static void AddLayout(FlatBufferBuilder builder, StringOffset layoutOffset) { builder.AddOffset(2, layoutOffset.Value, 0); }
  public static void AddF1(FlatBufferBuilder builder, StringOffset f1Offset) { builder.AddOffset(3, f1Offset.Value, 0); }
  public static void AddF2(FlatBufferBuilder builder, StringOffset f2Offset) { builder.AddOffset(4, f2Offset.Value, 0); }
  public static void AddF3(FlatBufferBuilder builder, StringOffset f3Offset) { builder.AddOffset(5, f3Offset.Value, 0); }
  public static void AddM1(FlatBufferBuilder builder, StringOffset m1Offset) { builder.AddOffset(6, m1Offset.Value, 0); }
  public static void AddM2(FlatBufferBuilder builder, StringOffset m2Offset) { builder.AddOffset(7, m2Offset.Value, 0); }
  public static void AddM3(FlatBufferBuilder builder, StringOffset m3Offset) { builder.AddOffset(8, m3Offset.Value, 0); }
  public static void AddB1(FlatBufferBuilder builder, StringOffset b1Offset) { builder.AddOffset(9, b1Offset.Value, 0); }
  public static void AddB2(FlatBufferBuilder builder, StringOffset b2Offset) { builder.AddOffset(10, b2Offset.Value, 0); }
  public static void AddB3(FlatBufferBuilder builder, StringOffset b3Offset) { builder.AddOffset(11, b3Offset.Value, 0); }
  public static void AddIconTemplateId(FlatBufferBuilder builder, StringOffset iconTemplateIdOffset) { builder.AddOffset(12, iconTemplateIdOffset.Value, 0); }
  public static void AddCombatAI(FlatBufferBuilder builder, StringOffset combatAIOffset) { builder.AddOffset(13, combatAIOffset.Value, 0); }
  public static void AddModel(FlatBufferBuilder builder, StringOffset modelOffset) { builder.AddOffset(14, modelOffset.Value, 0); }
  public static Offset<LayoutInfo> EndLayoutInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<LayoutInfo>(o);
  }
};

public sealed class MainlandEncounter : Table {
  public static MainlandEncounter GetRootAsMainlandEncounter(ByteBuffer _bb) { return GetRootAsMainlandEncounter(_bb, new MainlandEncounter()); }
  public static MainlandEncounter GetRootAsMainlandEncounter(ByteBuffer _bb, MainlandEncounter obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public MainlandEncounter __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string MainlandName { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMainlandNameBytes() { return __vector_as_arraysegment(4); }
  public string Locator { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetLocatorBytes() { return __vector_as_arraysegment(6); }
  public string RandomLocators { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRandomLocatorsBytes() { return __vector_as_arraysegment(8); }
  public string CombatLayoutName { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetCombatLayoutNameBytes() { return __vector_as_arraysegment(10); }
  public string DisplayName { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDisplayNameBytes() { return __vector_as_arraysegment(12); }
  public string EncounterPrefab { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetEncounterPrefabBytes() { return __vector_as_arraysegment(14); }
  public string Script { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetScriptBytes() { return __vector_as_arraysegment(16); }
  public string Role { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRoleBytes() { return __vector_as_arraysegment(18); }
  public string EncounterAppearingWay { get { int o = __offset(20); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetEncounterAppearingWayBytes() { return __vector_as_arraysegment(20); }
  public string IsAppearing { get { int o = __offset(22); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIsAppearingBytes() { return __vector_as_arraysegment(22); }
  public int IsLoad { get { int o = __offset(24); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int FuncId1 { get { int o = __offset(26); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int FuncId2 { get { int o = __offset(28); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int FuncId3 { get { int o = __offset(30); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string HeadIcon { get { int o = __offset(32); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetHeadIconBytes() { return __vector_as_arraysegment(32); }
  public int DialogueId { get { int o = __offset(34); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<MainlandEncounter> CreateMainlandEncounter(FlatBufferBuilder builder,
      StringOffset mainland_nameOffset = default(StringOffset),
      StringOffset locatorOffset = default(StringOffset),
      StringOffset random_locatorsOffset = default(StringOffset),
      StringOffset combat_layout_nameOffset = default(StringOffset),
      StringOffset display_nameOffset = default(StringOffset),
      StringOffset encounter_prefabOffset = default(StringOffset),
      StringOffset scriptOffset = default(StringOffset),
      StringOffset roleOffset = default(StringOffset),
      StringOffset encounter_appearing_wayOffset = default(StringOffset),
      StringOffset is_appearingOffset = default(StringOffset),
      int is_load = 0,
      int func_id_1 = 0,
      int func_id_2 = 0,
      int func_id_3 = 0,
      StringOffset head_iconOffset = default(StringOffset),
      int dialogue_id = 0) {
    builder.StartObject(16);
    MainlandEncounter.AddDialogueId(builder, dialogue_id);
    MainlandEncounter.AddHeadIcon(builder, head_iconOffset);
    MainlandEncounter.AddFuncId3(builder, func_id_3);
    MainlandEncounter.AddFuncId2(builder, func_id_2);
    MainlandEncounter.AddFuncId1(builder, func_id_1);
    MainlandEncounter.AddIsLoad(builder, is_load);
    MainlandEncounter.AddIsAppearing(builder, is_appearingOffset);
    MainlandEncounter.AddEncounterAppearingWay(builder, encounter_appearing_wayOffset);
    MainlandEncounter.AddRole(builder, roleOffset);
    MainlandEncounter.AddScript(builder, scriptOffset);
    MainlandEncounter.AddEncounterPrefab(builder, encounter_prefabOffset);
    MainlandEncounter.AddDisplayName(builder, display_nameOffset);
    MainlandEncounter.AddCombatLayoutName(builder, combat_layout_nameOffset);
    MainlandEncounter.AddRandomLocators(builder, random_locatorsOffset);
    MainlandEncounter.AddLocator(builder, locatorOffset);
    MainlandEncounter.AddMainlandName(builder, mainland_nameOffset);
    return MainlandEncounter.EndMainlandEncounter(builder);
  }

  public static void StartMainlandEncounter(FlatBufferBuilder builder) { builder.StartObject(16); }
  public static void AddMainlandName(FlatBufferBuilder builder, StringOffset mainlandNameOffset) { builder.AddOffset(0, mainlandNameOffset.Value, 0); }
  public static void AddLocator(FlatBufferBuilder builder, StringOffset locatorOffset) { builder.AddOffset(1, locatorOffset.Value, 0); }
  public static void AddRandomLocators(FlatBufferBuilder builder, StringOffset randomLocatorsOffset) { builder.AddOffset(2, randomLocatorsOffset.Value, 0); }
  public static void AddCombatLayoutName(FlatBufferBuilder builder, StringOffset combatLayoutNameOffset) { builder.AddOffset(3, combatLayoutNameOffset.Value, 0); }
  public static void AddDisplayName(FlatBufferBuilder builder, StringOffset displayNameOffset) { builder.AddOffset(4, displayNameOffset.Value, 0); }
  public static void AddEncounterPrefab(FlatBufferBuilder builder, StringOffset encounterPrefabOffset) { builder.AddOffset(5, encounterPrefabOffset.Value, 0); }
  public static void AddScript(FlatBufferBuilder builder, StringOffset scriptOffset) { builder.AddOffset(6, scriptOffset.Value, 0); }
  public static void AddRole(FlatBufferBuilder builder, StringOffset roleOffset) { builder.AddOffset(7, roleOffset.Value, 0); }
  public static void AddEncounterAppearingWay(FlatBufferBuilder builder, StringOffset encounterAppearingWayOffset) { builder.AddOffset(8, encounterAppearingWayOffset.Value, 0); }
  public static void AddIsAppearing(FlatBufferBuilder builder, StringOffset isAppearingOffset) { builder.AddOffset(9, isAppearingOffset.Value, 0); }
  public static void AddIsLoad(FlatBufferBuilder builder, int isLoad) { builder.AddInt(10, isLoad, 0); }
  public static void AddFuncId1(FlatBufferBuilder builder, int funcId1) { builder.AddInt(11, funcId1, 0); }
  public static void AddFuncId2(FlatBufferBuilder builder, int funcId2) { builder.AddInt(12, funcId2, 0); }
  public static void AddFuncId3(FlatBufferBuilder builder, int funcId3) { builder.AddInt(13, funcId3, 0); }
  public static void AddHeadIcon(FlatBufferBuilder builder, StringOffset headIconOffset) { builder.AddOffset(14, headIconOffset.Value, 0); }
  public static void AddDialogueId(FlatBufferBuilder builder, int dialogueId) { builder.AddInt(15, dialogueId, 0); }
  public static Offset<MainlandEncounter> EndMainlandEncounter(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<MainlandEncounter>(o);
  }
};

public sealed class Mainland : Table {
  public static Mainland GetRootAsMainland(ByteBuffer _bb) { return GetRootAsMainland(_bb, new Mainland()); }
  public static Mainland GetRootAsMainland(ByteBuffer _bb, Mainland obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Mainland __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string MainlandName { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMainlandNameBytes() { return __vector_as_arraysegment(4); }
  public string Locator { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetLocatorBytes() { return __vector_as_arraysegment(6); }
  public string Icon { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIconBytes() { return __vector_as_arraysegment(8); }
  public string DisplayName { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDisplayNameBytes() { return __vector_as_arraysegment(10); }
  public string MapName { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMapNameBytes() { return __vector_as_arraysegment(12); }
  public string BornLocator { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBornLocatorBytes() { return __vector_as_arraysegment(14); }
  public string CombatMap { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetCombatMapBytes() { return __vector_as_arraysegment(16); }
  public string TriggerDialogue { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTriggerDialogueBytes() { return __vector_as_arraysegment(18); }
  public string MapDesc { get { int o = __offset(20); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMapDescBytes() { return __vector_as_arraysegment(20); }
  public string EncounterGroup { get { int o = __offset(22); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetEncounterGroupBytes() { return __vector_as_arraysegment(22); }
  public string TransferPoints { get { int o = __offset(24); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTransferPointsBytes() { return __vector_as_arraysegment(24); }
  public string InteractGroup { get { int o = __offset(26); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetInteractGroupBytes() { return __vector_as_arraysegment(26); }
  public string BackGroundMusic { get { int o = __offset(28); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBackGroundMusicBytes() { return __vector_as_arraysegment(28); }
  public string BossBattleMusic { get { int o = __offset(30); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBossBattleMusicBytes() { return __vector_as_arraysegment(30); }

  public static Offset<Mainland> CreateMainland(FlatBufferBuilder builder,
      StringOffset mainland_nameOffset = default(StringOffset),
      StringOffset locatorOffset = default(StringOffset),
      StringOffset iconOffset = default(StringOffset),
      StringOffset display_nameOffset = default(StringOffset),
      StringOffset map_nameOffset = default(StringOffset),
      StringOffset born_locatorOffset = default(StringOffset),
      StringOffset combat_mapOffset = default(StringOffset),
      StringOffset trigger_dialogueOffset = default(StringOffset),
      StringOffset map_descOffset = default(StringOffset),
      StringOffset encounter_groupOffset = default(StringOffset),
      StringOffset transfer_pointsOffset = default(StringOffset),
      StringOffset interact_groupOffset = default(StringOffset),
      StringOffset back_ground_musicOffset = default(StringOffset),
      StringOffset boss_battle_musicOffset = default(StringOffset)) {
    builder.StartObject(14);
    Mainland.AddBossBattleMusic(builder, boss_battle_musicOffset);
    Mainland.AddBackGroundMusic(builder, back_ground_musicOffset);
    Mainland.AddInteractGroup(builder, interact_groupOffset);
    Mainland.AddTransferPoints(builder, transfer_pointsOffset);
    Mainland.AddEncounterGroup(builder, encounter_groupOffset);
    Mainland.AddMapDesc(builder, map_descOffset);
    Mainland.AddTriggerDialogue(builder, trigger_dialogueOffset);
    Mainland.AddCombatMap(builder, combat_mapOffset);
    Mainland.AddBornLocator(builder, born_locatorOffset);
    Mainland.AddMapName(builder, map_nameOffset);
    Mainland.AddDisplayName(builder, display_nameOffset);
    Mainland.AddIcon(builder, iconOffset);
    Mainland.AddLocator(builder, locatorOffset);
    Mainland.AddMainlandName(builder, mainland_nameOffset);
    return Mainland.EndMainland(builder);
  }

  public static void StartMainland(FlatBufferBuilder builder) { builder.StartObject(14); }
  public static void AddMainlandName(FlatBufferBuilder builder, StringOffset mainlandNameOffset) { builder.AddOffset(0, mainlandNameOffset.Value, 0); }
  public static void AddLocator(FlatBufferBuilder builder, StringOffset locatorOffset) { builder.AddOffset(1, locatorOffset.Value, 0); }
  public static void AddIcon(FlatBufferBuilder builder, StringOffset iconOffset) { builder.AddOffset(2, iconOffset.Value, 0); }
  public static void AddDisplayName(FlatBufferBuilder builder, StringOffset displayNameOffset) { builder.AddOffset(3, displayNameOffset.Value, 0); }
  public static void AddMapName(FlatBufferBuilder builder, StringOffset mapNameOffset) { builder.AddOffset(4, mapNameOffset.Value, 0); }
  public static void AddBornLocator(FlatBufferBuilder builder, StringOffset bornLocatorOffset) { builder.AddOffset(5, bornLocatorOffset.Value, 0); }
  public static void AddCombatMap(FlatBufferBuilder builder, StringOffset combatMapOffset) { builder.AddOffset(6, combatMapOffset.Value, 0); }
  public static void AddTriggerDialogue(FlatBufferBuilder builder, StringOffset triggerDialogueOffset) { builder.AddOffset(7, triggerDialogueOffset.Value, 0); }
  public static void AddMapDesc(FlatBufferBuilder builder, StringOffset mapDescOffset) { builder.AddOffset(8, mapDescOffset.Value, 0); }
  public static void AddEncounterGroup(FlatBufferBuilder builder, StringOffset encounterGroupOffset) { builder.AddOffset(9, encounterGroupOffset.Value, 0); }
  public static void AddTransferPoints(FlatBufferBuilder builder, StringOffset transferPointsOffset) { builder.AddOffset(10, transferPointsOffset.Value, 0); }
  public static void AddInteractGroup(FlatBufferBuilder builder, StringOffset interactGroupOffset) { builder.AddOffset(11, interactGroupOffset.Value, 0); }
  public static void AddBackGroundMusic(FlatBufferBuilder builder, StringOffset backGroundMusicOffset) { builder.AddOffset(12, backGroundMusicOffset.Value, 0); }
  public static void AddBossBattleMusic(FlatBufferBuilder builder, StringOffset bossBattleMusicOffset) { builder.AddOffset(13, bossBattleMusicOffset.Value, 0); }
  public static Offset<Mainland> EndMainland(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Mainland>(o);
  }
};

public sealed class LostMainLand : Table {
  public static LostMainLand GetRootAsLostMainLand(ByteBuffer _bb) { return GetRootAsLostMainLand(_bb, new LostMainLand()); }
  public static LostMainLand GetRootAsLostMainLand(ByteBuffer _bb, LostMainLand obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public LostMainLand __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Id { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIdBytes() { return __vector_as_arraysegment(4); }
  public string Name { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(6); }
  public string LandPos { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetLandPosBytes() { return __vector_as_arraysegment(8); }
  public string LineName { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetLineNameBytes() { return __vector_as_arraysegment(10); }
  public string GetChapterList(int j) { int o = __offset(12); return o != 0 ? __string(__vector(o) + j * 4) : null; }
  public int ChapterListLength { get { int o = __offset(12); return o != 0 ? __vector_len(o) : 0; } }
  public string LandSize { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetLandSizeBytes() { return __vector_as_arraysegment(14); }

  public static Offset<LostMainLand> CreateLostMainLand(FlatBufferBuilder builder,
      StringOffset idOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      StringOffset land_posOffset = default(StringOffset),
      StringOffset line_nameOffset = default(StringOffset),
      VectorOffset chapter_listOffset = default(VectorOffset),
      StringOffset land_sizeOffset = default(StringOffset)) {
    builder.StartObject(6);
    LostMainLand.AddLandSize(builder, land_sizeOffset);
    LostMainLand.AddChapterList(builder, chapter_listOffset);
    LostMainLand.AddLineName(builder, line_nameOffset);
    LostMainLand.AddLandPos(builder, land_posOffset);
    LostMainLand.AddName(builder, nameOffset);
    LostMainLand.AddId(builder, idOffset);
    return LostMainLand.EndLostMainLand(builder);
  }

  public static void StartLostMainLand(FlatBufferBuilder builder) { builder.StartObject(6); }
  public static void AddId(FlatBufferBuilder builder, StringOffset idOffset) { builder.AddOffset(0, idOffset.Value, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(1, nameOffset.Value, 0); }
  public static void AddLandPos(FlatBufferBuilder builder, StringOffset landPosOffset) { builder.AddOffset(2, landPosOffset.Value, 0); }
  public static void AddLineName(FlatBufferBuilder builder, StringOffset lineNameOffset) { builder.AddOffset(3, lineNameOffset.Value, 0); }
  public static void AddChapterList(FlatBufferBuilder builder, VectorOffset chapterListOffset) { builder.AddOffset(4, chapterListOffset.Value, 0); }
  public static VectorOffset CreateChapterListVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartChapterListVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddLandSize(FlatBufferBuilder builder, StringOffset landSizeOffset) { builder.AddOffset(5, landSizeOffset.Value, 0); }
  public static Offset<LostMainLand> EndLostMainLand(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<LostMainLand>(o);
  }
};

public sealed class LostMainChapter : Table {
  public static LostMainChapter GetRootAsLostMainChapter(ByteBuffer _bb) { return GetRootAsLostMainChapter(_bb, new LostMainChapter()); }
  public static LostMainChapter GetRootAsLostMainChapter(ByteBuffer _bb, LostMainChapter obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public LostMainChapter __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Id { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIdBytes() { return __vector_as_arraysegment(4); }
  public int LevelLimit { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ForwardChapterId { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetForwardChapterIdBytes() { return __vector_as_arraysegment(8); }
  public string Name { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(10); }
  public string LandId { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetLandIdBytes() { return __vector_as_arraysegment(12); }
  public string ChapterPos { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetChapterPosBytes() { return __vector_as_arraysegment(14); }
  public string ChapterBg { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetChapterBgBytes() { return __vector_as_arraysegment(16); }
  public string GetCampaignList(int j) { int o = __offset(18); return o != 0 ? __string(__vector(o) + j * 4) : null; }
  public int CampaignListLength { get { int o = __offset(18); return o != 0 ? __vector_len(o) : 0; } }
  public string BoxList { get { int o = __offset(20); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBoxListBytes() { return __vector_as_arraysegment(20); }
  public string BallPosList { get { int o = __offset(22); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBallPosListBytes() { return __vector_as_arraysegment(22); }
  public string BallRewardDesc { get { int o = __offset(24); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBallRewardDescBytes() { return __vector_as_arraysegment(24); }
  public string PasswordPos { get { int o = __offset(26); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetPasswordPosBytes() { return __vector_as_arraysegment(26); }
  public string Password { get { int o = __offset(28); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetPasswordBytes() { return __vector_as_arraysegment(28); }
  public string AwardIcon { get { int o = __offset(30); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetAwardIconBytes() { return __vector_as_arraysegment(30); }
  public string MaskBg { get { int o = __offset(32); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMaskBgBytes() { return __vector_as_arraysegment(32); }
  public string Icon { get { int o = __offset(34); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIconBytes() { return __vector_as_arraysegment(34); }
  public string BeforeChapter { get { int o = __offset(36); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBeforeChapterBytes() { return __vector_as_arraysegment(36); }
  public string AfterChapter { get { int o = __offset(38); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetAfterChapterBytes() { return __vector_as_arraysegment(38); }
  public string AwardIcon1 { get { int o = __offset(40); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetAwardIcon1Bytes() { return __vector_as_arraysegment(40); }
  public string AwardIcon2 { get { int o = __offset(42); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetAwardIcon2Bytes() { return __vector_as_arraysegment(42); }
  public string AwardIcon3 { get { int o = __offset(44); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetAwardIcon3Bytes() { return __vector_as_arraysegment(44); }
  public int LimitParam1 { get { int o = __offset(46); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int LimitParam2 { get { int o = __offset(48); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<LostMainChapter> CreateLostMainChapter(FlatBufferBuilder builder,
      StringOffset idOffset = default(StringOffset),
      int level_limit = 0,
      StringOffset forward_chapter_idOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      StringOffset land_idOffset = default(StringOffset),
      StringOffset chapter_posOffset = default(StringOffset),
      StringOffset chapter_bgOffset = default(StringOffset),
      VectorOffset campaign_listOffset = default(VectorOffset),
      StringOffset box_listOffset = default(StringOffset),
      StringOffset ball_pos_listOffset = default(StringOffset),
      StringOffset ball_reward_descOffset = default(StringOffset),
      StringOffset password_posOffset = default(StringOffset),
      StringOffset passwordOffset = default(StringOffset),
      StringOffset award_iconOffset = default(StringOffset),
      StringOffset mask_bgOffset = default(StringOffset),
      StringOffset iconOffset = default(StringOffset),
      StringOffset before_chapterOffset = default(StringOffset),
      StringOffset after_chapterOffset = default(StringOffset),
      StringOffset award_icon_1Offset = default(StringOffset),
      StringOffset award_icon_2Offset = default(StringOffset),
      StringOffset award_icon_3Offset = default(StringOffset),
      int limit_param_1 = 0,
      int limit_param_2 = 0) {
    builder.StartObject(23);
    LostMainChapter.AddLimitParam2(builder, limit_param_2);
    LostMainChapter.AddLimitParam1(builder, limit_param_1);
    LostMainChapter.AddAwardIcon3(builder, award_icon_3Offset);
    LostMainChapter.AddAwardIcon2(builder, award_icon_2Offset);
    LostMainChapter.AddAwardIcon1(builder, award_icon_1Offset);
    LostMainChapter.AddAfterChapter(builder, after_chapterOffset);
    LostMainChapter.AddBeforeChapter(builder, before_chapterOffset);
    LostMainChapter.AddIcon(builder, iconOffset);
    LostMainChapter.AddMaskBg(builder, mask_bgOffset);
    LostMainChapter.AddAwardIcon(builder, award_iconOffset);
    LostMainChapter.AddPassword(builder, passwordOffset);
    LostMainChapter.AddPasswordPos(builder, password_posOffset);
    LostMainChapter.AddBallRewardDesc(builder, ball_reward_descOffset);
    LostMainChapter.AddBallPosList(builder, ball_pos_listOffset);
    LostMainChapter.AddBoxList(builder, box_listOffset);
    LostMainChapter.AddCampaignList(builder, campaign_listOffset);
    LostMainChapter.AddChapterBg(builder, chapter_bgOffset);
    LostMainChapter.AddChapterPos(builder, chapter_posOffset);
    LostMainChapter.AddLandId(builder, land_idOffset);
    LostMainChapter.AddName(builder, nameOffset);
    LostMainChapter.AddForwardChapterId(builder, forward_chapter_idOffset);
    LostMainChapter.AddLevelLimit(builder, level_limit);
    LostMainChapter.AddId(builder, idOffset);
    return LostMainChapter.EndLostMainChapter(builder);
  }

  public static void StartLostMainChapter(FlatBufferBuilder builder) { builder.StartObject(23); }
  public static void AddId(FlatBufferBuilder builder, StringOffset idOffset) { builder.AddOffset(0, idOffset.Value, 0); }
  public static void AddLevelLimit(FlatBufferBuilder builder, int levelLimit) { builder.AddInt(1, levelLimit, 0); }
  public static void AddForwardChapterId(FlatBufferBuilder builder, StringOffset forwardChapterIdOffset) { builder.AddOffset(2, forwardChapterIdOffset.Value, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(3, nameOffset.Value, 0); }
  public static void AddLandId(FlatBufferBuilder builder, StringOffset landIdOffset) { builder.AddOffset(4, landIdOffset.Value, 0); }
  public static void AddChapterPos(FlatBufferBuilder builder, StringOffset chapterPosOffset) { builder.AddOffset(5, chapterPosOffset.Value, 0); }
  public static void AddChapterBg(FlatBufferBuilder builder, StringOffset chapterBgOffset) { builder.AddOffset(6, chapterBgOffset.Value, 0); }
  public static void AddCampaignList(FlatBufferBuilder builder, VectorOffset campaignListOffset) { builder.AddOffset(7, campaignListOffset.Value, 0); }
  public static VectorOffset CreateCampaignListVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartCampaignListVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddBoxList(FlatBufferBuilder builder, StringOffset boxListOffset) { builder.AddOffset(8, boxListOffset.Value, 0); }
  public static void AddBallPosList(FlatBufferBuilder builder, StringOffset ballPosListOffset) { builder.AddOffset(9, ballPosListOffset.Value, 0); }
  public static void AddBallRewardDesc(FlatBufferBuilder builder, StringOffset ballRewardDescOffset) { builder.AddOffset(10, ballRewardDescOffset.Value, 0); }
  public static void AddPasswordPos(FlatBufferBuilder builder, StringOffset passwordPosOffset) { builder.AddOffset(11, passwordPosOffset.Value, 0); }
  public static void AddPassword(FlatBufferBuilder builder, StringOffset passwordOffset) { builder.AddOffset(12, passwordOffset.Value, 0); }
  public static void AddAwardIcon(FlatBufferBuilder builder, StringOffset awardIconOffset) { builder.AddOffset(13, awardIconOffset.Value, 0); }
  public static void AddMaskBg(FlatBufferBuilder builder, StringOffset maskBgOffset) { builder.AddOffset(14, maskBgOffset.Value, 0); }
  public static void AddIcon(FlatBufferBuilder builder, StringOffset iconOffset) { builder.AddOffset(15, iconOffset.Value, 0); }
  public static void AddBeforeChapter(FlatBufferBuilder builder, StringOffset beforeChapterOffset) { builder.AddOffset(16, beforeChapterOffset.Value, 0); }
  public static void AddAfterChapter(FlatBufferBuilder builder, StringOffset afterChapterOffset) { builder.AddOffset(17, afterChapterOffset.Value, 0); }
  public static void AddAwardIcon1(FlatBufferBuilder builder, StringOffset awardIcon1Offset) { builder.AddOffset(18, awardIcon1Offset.Value, 0); }
  public static void AddAwardIcon2(FlatBufferBuilder builder, StringOffset awardIcon2Offset) { builder.AddOffset(19, awardIcon2Offset.Value, 0); }
  public static void AddAwardIcon3(FlatBufferBuilder builder, StringOffset awardIcon3Offset) { builder.AddOffset(20, awardIcon3Offset.Value, 0); }
  public static void AddLimitParam1(FlatBufferBuilder builder, int limitParam1) { builder.AddInt(21, limitParam1, 0); }
  public static void AddLimitParam2(FlatBufferBuilder builder, int limitParam2) { builder.AddInt(22, limitParam2, 0); }
  public static Offset<LostMainChapter> EndLostMainChapter(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<LostMainChapter>(o);
  }
};

public sealed class LostMainCampaigns : Table {
  public static LostMainCampaigns GetRootAsLostMainCampaigns(ByteBuffer _bb) { return GetRootAsLostMainCampaigns(_bb, new LostMainCampaigns()); }
  public static LostMainCampaigns GetRootAsLostMainCampaigns(ByteBuffer _bb, LostMainCampaigns obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public LostMainCampaigns __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Chapter { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetChapterBytes() { return __vector_as_arraysegment(4); }
  public string Id { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIdBytes() { return __vector_as_arraysegment(6); }
  public string Name { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(8); }
  public int RecommendLevel { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int PartnerExp { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int BlitzPartnerExp { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int HeroExp { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int CostVigor { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int AwardGold { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string MapName { get { int o = __offset(22); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMapNameBytes() { return __vector_as_arraysegment(22); }
  public string Desc { get { int o = __offset(24); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDescBytes() { return __vector_as_arraysegment(24); }
  public int Type { get { int o = __offset(26); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ModelName { get { int o = __offset(28); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetModelNameBytes() { return __vector_as_arraysegment(28); }
  public string GetPreCampaigns(int j) { int o = __offset(30); return o != 0 ? __string(__vector(o) + j * 4) : null; }
  public int PreCampaignsLength { get { int o = __offset(30); return o != 0 ? __vector_len(o) : 0; } }
  public string GetNextCampaigns(int j) { int o = __offset(32); return o != 0 ? __string(__vector(o) + j * 4) : null; }
  public int NextCampaignsLength { get { int o = __offset(32); return o != 0 ? __vector_len(o) : 0; } }
  public string EncounterGroupId { get { int o = __offset(34); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetEncounterGroupIdBytes() { return __vector_as_arraysegment(34); }
  public string StoryId { get { int o = __offset(36); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetStoryIdBytes() { return __vector_as_arraysegment(36); }
  public string RewardId { get { int o = __offset(38); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRewardIdBytes() { return __vector_as_arraysegment(38); }
  public int GetCampaignPos(int j) { int o = __offset(40); return o != 0 ? bb.GetInt(__vector(o) + j * 4) : (int)0; }
  public int CampaignPosLength { get { int o = __offset(40); return o != 0 ? __vector_len(o) : 0; } }
  public ArraySegment<byte>? GetCampaignPosBytes() { return __vector_as_arraysegment(40); }
  public string AwardIcon { get { int o = __offset(42); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetAwardIconBytes() { return __vector_as_arraysegment(42); }

  public static Offset<LostMainCampaigns> CreateLostMainCampaigns(FlatBufferBuilder builder,
      StringOffset chapterOffset = default(StringOffset),
      StringOffset idOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      int recommend_level = 0,
      int partner_exp = 0,
      int blitz_partner_exp = 0,
      int hero_exp = 0,
      int cost_vigor = 0,
      int award_gold = 0,
      StringOffset map_nameOffset = default(StringOffset),
      StringOffset descOffset = default(StringOffset),
      int type = 0,
      StringOffset model_nameOffset = default(StringOffset),
      VectorOffset pre_campaignsOffset = default(VectorOffset),
      VectorOffset next_campaignsOffset = default(VectorOffset),
      StringOffset encounter_group_idOffset = default(StringOffset),
      StringOffset story_idOffset = default(StringOffset),
      StringOffset reward_idOffset = default(StringOffset),
      VectorOffset campaign_posOffset = default(VectorOffset),
      StringOffset award_iconOffset = default(StringOffset)) {
    builder.StartObject(20);
    LostMainCampaigns.AddAwardIcon(builder, award_iconOffset);
    LostMainCampaigns.AddCampaignPos(builder, campaign_posOffset);
    LostMainCampaigns.AddRewardId(builder, reward_idOffset);
    LostMainCampaigns.AddStoryId(builder, story_idOffset);
    LostMainCampaigns.AddEncounterGroupId(builder, encounter_group_idOffset);
    LostMainCampaigns.AddNextCampaigns(builder, next_campaignsOffset);
    LostMainCampaigns.AddPreCampaigns(builder, pre_campaignsOffset);
    LostMainCampaigns.AddModelName(builder, model_nameOffset);
    LostMainCampaigns.AddType(builder, type);
    LostMainCampaigns.AddDesc(builder, descOffset);
    LostMainCampaigns.AddMapName(builder, map_nameOffset);
    LostMainCampaigns.AddAwardGold(builder, award_gold);
    LostMainCampaigns.AddCostVigor(builder, cost_vigor);
    LostMainCampaigns.AddHeroExp(builder, hero_exp);
    LostMainCampaigns.AddBlitzPartnerExp(builder, blitz_partner_exp);
    LostMainCampaigns.AddPartnerExp(builder, partner_exp);
    LostMainCampaigns.AddRecommendLevel(builder, recommend_level);
    LostMainCampaigns.AddName(builder, nameOffset);
    LostMainCampaigns.AddId(builder, idOffset);
    LostMainCampaigns.AddChapter(builder, chapterOffset);
    return LostMainCampaigns.EndLostMainCampaigns(builder);
  }

  public static void StartLostMainCampaigns(FlatBufferBuilder builder) { builder.StartObject(20); }
  public static void AddChapter(FlatBufferBuilder builder, StringOffset chapterOffset) { builder.AddOffset(0, chapterOffset.Value, 0); }
  public static void AddId(FlatBufferBuilder builder, StringOffset idOffset) { builder.AddOffset(1, idOffset.Value, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(2, nameOffset.Value, 0); }
  public static void AddRecommendLevel(FlatBufferBuilder builder, int recommendLevel) { builder.AddInt(3, recommendLevel, 0); }
  public static void AddPartnerExp(FlatBufferBuilder builder, int partnerExp) { builder.AddInt(4, partnerExp, 0); }
  public static void AddBlitzPartnerExp(FlatBufferBuilder builder, int blitzPartnerExp) { builder.AddInt(5, blitzPartnerExp, 0); }
  public static void AddHeroExp(FlatBufferBuilder builder, int heroExp) { builder.AddInt(6, heroExp, 0); }
  public static void AddCostVigor(FlatBufferBuilder builder, int costVigor) { builder.AddInt(7, costVigor, 0); }
  public static void AddAwardGold(FlatBufferBuilder builder, int awardGold) { builder.AddInt(8, awardGold, 0); }
  public static void AddMapName(FlatBufferBuilder builder, StringOffset mapNameOffset) { builder.AddOffset(9, mapNameOffset.Value, 0); }
  public static void AddDesc(FlatBufferBuilder builder, StringOffset descOffset) { builder.AddOffset(10, descOffset.Value, 0); }
  public static void AddType(FlatBufferBuilder builder, int type) { builder.AddInt(11, type, 0); }
  public static void AddModelName(FlatBufferBuilder builder, StringOffset modelNameOffset) { builder.AddOffset(12, modelNameOffset.Value, 0); }
  public static void AddPreCampaigns(FlatBufferBuilder builder, VectorOffset preCampaignsOffset) { builder.AddOffset(13, preCampaignsOffset.Value, 0); }
  public static VectorOffset CreatePreCampaignsVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartPreCampaignsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddNextCampaigns(FlatBufferBuilder builder, VectorOffset nextCampaignsOffset) { builder.AddOffset(14, nextCampaignsOffset.Value, 0); }
  public static VectorOffset CreateNextCampaignsVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartNextCampaignsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddEncounterGroupId(FlatBufferBuilder builder, StringOffset encounterGroupIdOffset) { builder.AddOffset(15, encounterGroupIdOffset.Value, 0); }
  public static void AddStoryId(FlatBufferBuilder builder, StringOffset storyIdOffset) { builder.AddOffset(16, storyIdOffset.Value, 0); }
  public static void AddRewardId(FlatBufferBuilder builder, StringOffset rewardIdOffset) { builder.AddOffset(17, rewardIdOffset.Value, 0); }
  public static void AddCampaignPos(FlatBufferBuilder builder, VectorOffset campaignPosOffset) { builder.AddOffset(18, campaignPosOffset.Value, 0); }
  public static VectorOffset CreateCampaignPosVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartCampaignPosVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddAwardIcon(FlatBufferBuilder builder, StringOffset awardIconOffset) { builder.AddOffset(19, awardIconOffset.Value, 0); }
  public static Offset<LostMainCampaigns> EndLostMainCampaigns(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<LostMainCampaigns>(o);
  }
};

public sealed class LostChallengeChapter : Table {
  public static LostChallengeChapter GetRootAsLostChallengeChapter(ByteBuffer _bb) { return GetRootAsLostChallengeChapter(_bb, new LostChallengeChapter()); }
  public static LostChallengeChapter GetRootAsLostChallengeChapter(ByteBuffer _bb, LostChallengeChapter obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public LostChallengeChapter __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Level { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public bool IsCheckPoint { get { int o = __offset(6); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public string FixMap { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetFixMapBytes() { return __vector_as_arraysegment(8); }
  public int RecommendLevel { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int BigFloor { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SmallFloor { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<LostChallengeChapter> CreateLostChallengeChapter(FlatBufferBuilder builder,
      int level = 0,
      bool is_check_point = false,
      StringOffset fixMapOffset = default(StringOffset),
      int recommend_level = 0,
      int big_floor = 0,
      int small_floor = 0) {
    builder.StartObject(6);
    LostChallengeChapter.AddSmallFloor(builder, small_floor);
    LostChallengeChapter.AddBigFloor(builder, big_floor);
    LostChallengeChapter.AddRecommendLevel(builder, recommend_level);
    LostChallengeChapter.AddFixMap(builder, fixMapOffset);
    LostChallengeChapter.AddLevel(builder, level);
    LostChallengeChapter.AddIsCheckPoint(builder, is_check_point);
    return LostChallengeChapter.EndLostChallengeChapter(builder);
  }

  public static void StartLostChallengeChapter(FlatBufferBuilder builder) { builder.StartObject(6); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(0, level, 0); }
  public static void AddIsCheckPoint(FlatBufferBuilder builder, bool isCheckPoint) { builder.AddBool(1, isCheckPoint, false); }
  public static void AddFixMap(FlatBufferBuilder builder, StringOffset fixMapOffset) { builder.AddOffset(2, fixMapOffset.Value, 0); }
  public static void AddRecommendLevel(FlatBufferBuilder builder, int recommendLevel) { builder.AddInt(3, recommendLevel, 0); }
  public static void AddBigFloor(FlatBufferBuilder builder, int bigFloor) { builder.AddInt(4, bigFloor, 0); }
  public static void AddSmallFloor(FlatBufferBuilder builder, int smallFloor) { builder.AddInt(5, smallFloor, 0); }
  public static Offset<LostChallengeChapter> EndLostChallengeChapter(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<LostChallengeChapter>(o);
  }
};

public sealed class LostChallengeWalls : Table {
  public static LostChallengeWalls GetRootAsLostChallengeWalls(ByteBuffer _bb) { return GetRootAsLostChallengeWalls(_bb, new LostChallengeWalls()); }
  public static LostChallengeWalls GetRootAsLostChallengeWalls(ByteBuffer _bb, LostChallengeWalls obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public LostChallengeWalls __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Type { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTypeBytes() { return __vector_as_arraysegment(6); }
  public string Pos { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetPosBytes() { return __vector_as_arraysegment(8); }
  public string Img { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetImgBytes() { return __vector_as_arraysegment(10); }
  public string Rotation { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRotationBytes() { return __vector_as_arraysegment(12); }

  public static Offset<LostChallengeWalls> CreateLostChallengeWalls(FlatBufferBuilder builder,
      int id = 0,
      StringOffset typeOffset = default(StringOffset),
      StringOffset posOffset = default(StringOffset),
      StringOffset imgOffset = default(StringOffset),
      StringOffset rotationOffset = default(StringOffset)) {
    builder.StartObject(5);
    LostChallengeWalls.AddRotation(builder, rotationOffset);
    LostChallengeWalls.AddImg(builder, imgOffset);
    LostChallengeWalls.AddPos(builder, posOffset);
    LostChallengeWalls.AddType(builder, typeOffset);
    LostChallengeWalls.AddId(builder, id);
    return LostChallengeWalls.EndLostChallengeWalls(builder);
  }

  public static void StartLostChallengeWalls(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddType(FlatBufferBuilder builder, StringOffset typeOffset) { builder.AddOffset(1, typeOffset.Value, 0); }
  public static void AddPos(FlatBufferBuilder builder, StringOffset posOffset) { builder.AddOffset(2, posOffset.Value, 0); }
  public static void AddImg(FlatBufferBuilder builder, StringOffset imgOffset) { builder.AddOffset(3, imgOffset.Value, 0); }
  public static void AddRotation(FlatBufferBuilder builder, StringOffset rotationOffset) { builder.AddOffset(4, rotationOffset.Value, 0); }
  public static Offset<LostChallengeWalls> EndLostChallengeWalls(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<LostChallengeWalls>(o);
  }
};

public sealed class LostCombatMap : Table {
  public static LostCombatMap GetRootAsLostCombatMap(ByteBuffer _bb) { return GetRootAsLostCombatMap(_bb, new LostCombatMap()); }
  public static LostCombatMap GetRootAsLostCombatMap(ByteBuffer _bb, LostCombatMap obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public LostCombatMap __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int BattleType { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Scene { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetSceneBytes() { return __vector_as_arraysegment(6); }

  public static Offset<LostCombatMap> CreateLostCombatMap(FlatBufferBuilder builder,
      int battle_type = 0,
      StringOffset sceneOffset = default(StringOffset)) {
    builder.StartObject(2);
    LostCombatMap.AddScene(builder, sceneOffset);
    LostCombatMap.AddBattleType(builder, battle_type);
    return LostCombatMap.EndLostCombatMap(builder);
  }

  public static void StartLostCombatMap(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddBattleType(FlatBufferBuilder builder, int battleType) { builder.AddInt(0, battleType, 0); }
  public static void AddScene(FlatBufferBuilder builder, StringOffset sceneOffset) { builder.AddOffset(1, sceneOffset.Value, 0); }
  public static Offset<LostCombatMap> EndLostCombatMap(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<LostCombatMap>(o);
  }
};

public sealed class MainLandsGhost : Table {
  public static MainLandsGhost GetRootAsMainLandsGhost(ByteBuffer _bb) { return GetRootAsMainLandsGhost(_bb, new MainLandsGhost()); }
  public static MainLandsGhost GetRootAsMainLandsGhost(ByteBuffer _bb, MainLandsGhost obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public MainLandsGhost __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string MainlandName { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMainlandNameBytes() { return __vector_as_arraysegment(4); }
  public string EncounterName { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetEncounterNameBytes() { return __vector_as_arraysegment(6); }
  public string Locator { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetLocatorBytes() { return __vector_as_arraysegment(8); }
  public string CombatLayoutName { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetCombatLayoutNameBytes() { return __vector_as_arraysegment(10); }
  public string DisplayName { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDisplayNameBytes() { return __vector_as_arraysegment(12); }
  public string EncounterPrefab { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetEncounterPrefabBytes() { return __vector_as_arraysegment(14); }
  public string Script { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetScriptBytes() { return __vector_as_arraysegment(16); }
  public string Role { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRoleBytes() { return __vector_as_arraysegment(18); }
  public string EncounterAppearingWay { get { int o = __offset(20); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetEncounterAppearingWayBytes() { return __vector_as_arraysegment(20); }

  public static Offset<MainLandsGhost> CreateMainLandsGhost(FlatBufferBuilder builder,
      StringOffset mainland_nameOffset = default(StringOffset),
      StringOffset encounter_nameOffset = default(StringOffset),
      StringOffset locatorOffset = default(StringOffset),
      StringOffset combat_layout_nameOffset = default(StringOffset),
      StringOffset display_nameOffset = default(StringOffset),
      StringOffset encounter_prefabOffset = default(StringOffset),
      StringOffset scriptOffset = default(StringOffset),
      StringOffset roleOffset = default(StringOffset),
      StringOffset encounter_appearing_wayOffset = default(StringOffset)) {
    builder.StartObject(9);
    MainLandsGhost.AddEncounterAppearingWay(builder, encounter_appearing_wayOffset);
    MainLandsGhost.AddRole(builder, roleOffset);
    MainLandsGhost.AddScript(builder, scriptOffset);
    MainLandsGhost.AddEncounterPrefab(builder, encounter_prefabOffset);
    MainLandsGhost.AddDisplayName(builder, display_nameOffset);
    MainLandsGhost.AddCombatLayoutName(builder, combat_layout_nameOffset);
    MainLandsGhost.AddLocator(builder, locatorOffset);
    MainLandsGhost.AddEncounterName(builder, encounter_nameOffset);
    MainLandsGhost.AddMainlandName(builder, mainland_nameOffset);
    return MainLandsGhost.EndMainLandsGhost(builder);
  }

  public static void StartMainLandsGhost(FlatBufferBuilder builder) { builder.StartObject(9); }
  public static void AddMainlandName(FlatBufferBuilder builder, StringOffset mainlandNameOffset) { builder.AddOffset(0, mainlandNameOffset.Value, 0); }
  public static void AddEncounterName(FlatBufferBuilder builder, StringOffset encounterNameOffset) { builder.AddOffset(1, encounterNameOffset.Value, 0); }
  public static void AddLocator(FlatBufferBuilder builder, StringOffset locatorOffset) { builder.AddOffset(2, locatorOffset.Value, 0); }
  public static void AddCombatLayoutName(FlatBufferBuilder builder, StringOffset combatLayoutNameOffset) { builder.AddOffset(3, combatLayoutNameOffset.Value, 0); }
  public static void AddDisplayName(FlatBufferBuilder builder, StringOffset displayNameOffset) { builder.AddOffset(4, displayNameOffset.Value, 0); }
  public static void AddEncounterPrefab(FlatBufferBuilder builder, StringOffset encounterPrefabOffset) { builder.AddOffset(5, encounterPrefabOffset.Value, 0); }
  public static void AddScript(FlatBufferBuilder builder, StringOffset scriptOffset) { builder.AddOffset(6, scriptOffset.Value, 0); }
  public static void AddRole(FlatBufferBuilder builder, StringOffset roleOffset) { builder.AddOffset(7, roleOffset.Value, 0); }
  public static void AddEncounterAppearingWay(FlatBufferBuilder builder, StringOffset encounterAppearingWayOffset) { builder.AddOffset(8, encounterAppearingWayOffset.Value, 0); }
  public static Offset<MainLandsGhost> EndMainLandsGhost(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<MainLandsGhost>(o);
  }
};

public sealed class LostChallengeStyle : Table {
  public static LostChallengeStyle GetRootAsLostChallengeStyle(ByteBuffer _bb) { return GetRootAsLostChallengeStyle(_bb, new LostChallengeStyle()); }
  public static LostChallengeStyle GetRootAsLostChallengeStyle(ByteBuffer _bb, LostChallengeStyle obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public LostChallengeStyle __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string MapBg { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMapBgBytes() { return __vector_as_arraysegment(6); }
  public string MapMask { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMapMaskBytes() { return __vector_as_arraysegment(8); }
  public string CombatScene { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetCombatSceneBytes() { return __vector_as_arraysegment(10); }
  public int NormalTerra { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int TopWell { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<LostChallengeStyle> CreateLostChallengeStyle(FlatBufferBuilder builder,
      int id = 0,
      StringOffset map_bgOffset = default(StringOffset),
      StringOffset map_maskOffset = default(StringOffset),
      StringOffset combat_sceneOffset = default(StringOffset),
      int normalTerra = 0,
      int topWell = 0) {
    builder.StartObject(6);
    LostChallengeStyle.AddTopWell(builder, topWell);
    LostChallengeStyle.AddNormalTerra(builder, normalTerra);
    LostChallengeStyle.AddCombatScene(builder, combat_sceneOffset);
    LostChallengeStyle.AddMapMask(builder, map_maskOffset);
    LostChallengeStyle.AddMapBg(builder, map_bgOffset);
    LostChallengeStyle.AddId(builder, id);
    return LostChallengeStyle.EndLostChallengeStyle(builder);
  }

  public static void StartLostChallengeStyle(FlatBufferBuilder builder) { builder.StartObject(6); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddMapBg(FlatBufferBuilder builder, StringOffset mapBgOffset) { builder.AddOffset(1, mapBgOffset.Value, 0); }
  public static void AddMapMask(FlatBufferBuilder builder, StringOffset mapMaskOffset) { builder.AddOffset(2, mapMaskOffset.Value, 0); }
  public static void AddCombatScene(FlatBufferBuilder builder, StringOffset combatSceneOffset) { builder.AddOffset(3, combatSceneOffset.Value, 0); }
  public static void AddNormalTerra(FlatBufferBuilder builder, int normalTerra) { builder.AddInt(4, normalTerra, 0); }
  public static void AddTopWell(FlatBufferBuilder builder, int topWell) { builder.AddInt(5, topWell, 0); }
  public static Offset<LostChallengeStyle> EndLostChallengeStyle(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<LostChallengeStyle>(o);
  }
};

public sealed class MainLandsGhostReward : Table {
  public static MainLandsGhostReward GetRootAsMainLandsGhostReward(ByteBuffer _bb) { return GetRootAsMainLandsGhostReward(_bb, new MainLandsGhostReward()); }
  public static MainLandsGhostReward GetRootAsMainLandsGhostReward(ByteBuffer _bb, MainLandsGhostReward obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public MainLandsGhostReward __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Type { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTypeBytes() { return __vector_as_arraysegment(6); }
  public string Reward { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRewardBytes() { return __vector_as_arraysegment(8); }

  public static Offset<MainLandsGhostReward> CreateMainLandsGhostReward(FlatBufferBuilder builder,
      int id = 0,
      StringOffset typeOffset = default(StringOffset),
      StringOffset rewardOffset = default(StringOffset)) {
    builder.StartObject(3);
    MainLandsGhostReward.AddReward(builder, rewardOffset);
    MainLandsGhostReward.AddType(builder, typeOffset);
    MainLandsGhostReward.AddId(builder, id);
    return MainLandsGhostReward.EndMainLandsGhostReward(builder);
  }

  public static void StartMainLandsGhostReward(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddType(FlatBufferBuilder builder, StringOffset typeOffset) { builder.AddOffset(1, typeOffset.Value, 0); }
  public static void AddReward(FlatBufferBuilder builder, StringOffset rewardOffset) { builder.AddOffset(2, rewardOffset.Value, 0); }
  public static Offset<MainLandsGhostReward> EndMainLandsGhostReward(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<MainLandsGhostReward>(o);
  }
};

public sealed class LostChallengeReward : Table {
  public static LostChallengeReward GetRootAsLostChallengeReward(ByteBuffer _bb) { return GetRootAsLostChallengeReward(_bb, new LostChallengeReward()); }
  public static LostChallengeReward GetRootAsLostChallengeReward(ByteBuffer _bb, LostChallengeReward obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public LostChallengeReward __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Week { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Drop1 { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDrop1Bytes() { return __vector_as_arraysegment(6); }
  public string Drop2 { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDrop2Bytes() { return __vector_as_arraysegment(8); }
  public string Drop3 { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDrop3Bytes() { return __vector_as_arraysegment(10); }
  public string Drop4 { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDrop4Bytes() { return __vector_as_arraysegment(12); }
  public int Floor { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float DropRate { get { int o = __offset(16); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<LostChallengeReward> CreateLostChallengeReward(FlatBufferBuilder builder,
      int week = 0,
      StringOffset drop_1Offset = default(StringOffset),
      StringOffset drop_2Offset = default(StringOffset),
      StringOffset drop_3Offset = default(StringOffset),
      StringOffset drop_4Offset = default(StringOffset),
      int floor = 0,
      float drop_rate = 0) {
    builder.StartObject(7);
    LostChallengeReward.AddDropRate(builder, drop_rate);
    LostChallengeReward.AddFloor(builder, floor);
    LostChallengeReward.AddDrop4(builder, drop_4Offset);
    LostChallengeReward.AddDrop3(builder, drop_3Offset);
    LostChallengeReward.AddDrop2(builder, drop_2Offset);
    LostChallengeReward.AddDrop1(builder, drop_1Offset);
    LostChallengeReward.AddWeek(builder, week);
    return LostChallengeReward.EndLostChallengeReward(builder);
  }

  public static void StartLostChallengeReward(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddWeek(FlatBufferBuilder builder, int week) { builder.AddInt(0, week, 0); }
  public static void AddDrop1(FlatBufferBuilder builder, StringOffset drop1Offset) { builder.AddOffset(1, drop1Offset.Value, 0); }
  public static void AddDrop2(FlatBufferBuilder builder, StringOffset drop2Offset) { builder.AddOffset(2, drop2Offset.Value, 0); }
  public static void AddDrop3(FlatBufferBuilder builder, StringOffset drop3Offset) { builder.AddOffset(3, drop3Offset.Value, 0); }
  public static void AddDrop4(FlatBufferBuilder builder, StringOffset drop4Offset) { builder.AddOffset(4, drop4Offset.Value, 0); }
  public static void AddFloor(FlatBufferBuilder builder, int floor) { builder.AddInt(5, floor, 0); }
  public static void AddDropRate(FlatBufferBuilder builder, float dropRate) { builder.AddFloat(6, dropRate, 0); }
  public static Offset<LostChallengeReward> EndLostChallengeReward(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<LostChallengeReward>(o);
  }
};

public sealed class LostChallengeChapterRole : Table {
  public static LostChallengeChapterRole GetRootAsLostChallengeChapterRole(ByteBuffer _bb) { return GetRootAsLostChallengeChapterRole(_bb, new LostChallengeChapterRole()); }
  public static LostChallengeChapterRole GetRootAsLostChallengeChapterRole(ByteBuffer _bb, LostChallengeChapterRole obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public LostChallengeChapterRole __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Desc { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDescBytes() { return __vector_as_arraysegment(6); }
  public int Type { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Img { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetImgBytes() { return __vector_as_arraysegment(10); }
  public string Model { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetModelBytes() { return __vector_as_arraysegment(12); }
  public string Order { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetOrderBytes() { return __vector_as_arraysegment(14); }
  public string GetParam(int j) { int o = __offset(16); return o != 0 ? __string(__vector(o) + j * 4) : null; }
  public int ParamLength { get { int o = __offset(16); return o != 0 ? __vector_len(o) : 0; } }
  public string GetTag(int j) { int o = __offset(18); return o != 0 ? __string(__vector(o) + j * 4) : null; }
  public int TagLength { get { int o = __offset(18); return o != 0 ? __vector_len(o) : 0; } }
  public string Group { get { int o = __offset(20); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetGroupBytes() { return __vector_as_arraysegment(20); }
  public int Keyid { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Rotation { get { int o = __offset(24); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRotationBytes() { return __vector_as_arraysegment(24); }
  public string Span { get { int o = __offset(26); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetSpanBytes() { return __vector_as_arraysegment(26); }
  public string Offset { get { int o = __offset(28); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetOffsetBytes() { return __vector_as_arraysegment(28); }
  public string Guide { get { int o = __offset(30); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetGuideBytes() { return __vector_as_arraysegment(30); }
  public float ZOffset { get { int o = __offset(32); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public string Correlation { get { int o = __offset(34); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetCorrelationBytes() { return __vector_as_arraysegment(34); }

  public static Offset<LostChallengeChapterRole> CreateLostChallengeChapterRole(FlatBufferBuilder builder,
      int id = 0,
      StringOffset descOffset = default(StringOffset),
      int type = 0,
      StringOffset imgOffset = default(StringOffset),
      StringOffset modelOffset = default(StringOffset),
      StringOffset orderOffset = default(StringOffset),
      VectorOffset paramOffset = default(VectorOffset),
      VectorOffset tagOffset = default(VectorOffset),
      StringOffset groupOffset = default(StringOffset),
      int keyid = 0,
      StringOffset rotationOffset = default(StringOffset),
      StringOffset spanOffset = default(StringOffset),
      StringOffset offsetOffset = default(StringOffset),
      StringOffset guideOffset = default(StringOffset),
      float z_offset = 0,
      StringOffset correlationOffset = default(StringOffset)) {
    builder.StartObject(16);
    LostChallengeChapterRole.AddCorrelation(builder, correlationOffset);
    LostChallengeChapterRole.AddZOffset(builder, z_offset);
    LostChallengeChapterRole.AddGuide(builder, guideOffset);
    LostChallengeChapterRole.AddOffset(builder, offsetOffset);
    LostChallengeChapterRole.AddSpan(builder, spanOffset);
    LostChallengeChapterRole.AddRotation(builder, rotationOffset);
    LostChallengeChapterRole.AddKeyid(builder, keyid);
    LostChallengeChapterRole.AddGroup(builder, groupOffset);
    LostChallengeChapterRole.AddTag(builder, tagOffset);
    LostChallengeChapterRole.AddParam(builder, paramOffset);
    LostChallengeChapterRole.AddOrder(builder, orderOffset);
    LostChallengeChapterRole.AddModel(builder, modelOffset);
    LostChallengeChapterRole.AddImg(builder, imgOffset);
    LostChallengeChapterRole.AddType(builder, type);
    LostChallengeChapterRole.AddDesc(builder, descOffset);
    LostChallengeChapterRole.AddId(builder, id);
    return LostChallengeChapterRole.EndLostChallengeChapterRole(builder);
  }

  public static void StartLostChallengeChapterRole(FlatBufferBuilder builder) { builder.StartObject(16); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddDesc(FlatBufferBuilder builder, StringOffset descOffset) { builder.AddOffset(1, descOffset.Value, 0); }
  public static void AddType(FlatBufferBuilder builder, int type) { builder.AddInt(2, type, 0); }
  public static void AddImg(FlatBufferBuilder builder, StringOffset imgOffset) { builder.AddOffset(3, imgOffset.Value, 0); }
  public static void AddModel(FlatBufferBuilder builder, StringOffset modelOffset) { builder.AddOffset(4, modelOffset.Value, 0); }
  public static void AddOrder(FlatBufferBuilder builder, StringOffset orderOffset) { builder.AddOffset(5, orderOffset.Value, 0); }
  public static void AddParam(FlatBufferBuilder builder, VectorOffset paramOffset) { builder.AddOffset(6, paramOffset.Value, 0); }
  public static VectorOffset CreateParamVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartParamVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddTag(FlatBufferBuilder builder, VectorOffset tagOffset) { builder.AddOffset(7, tagOffset.Value, 0); }
  public static VectorOffset CreateTagVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartTagVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddGroup(FlatBufferBuilder builder, StringOffset groupOffset) { builder.AddOffset(8, groupOffset.Value, 0); }
  public static void AddKeyid(FlatBufferBuilder builder, int keyid) { builder.AddInt(9, keyid, 0); }
  public static void AddRotation(FlatBufferBuilder builder, StringOffset rotationOffset) { builder.AddOffset(10, rotationOffset.Value, 0); }
  public static void AddSpan(FlatBufferBuilder builder, StringOffset spanOffset) { builder.AddOffset(11, spanOffset.Value, 0); }
  public static void AddOffset(FlatBufferBuilder builder, StringOffset offsetOffset) { builder.AddOffset(12, offsetOffset.Value, 0); }
  public static void AddGuide(FlatBufferBuilder builder, StringOffset guideOffset) { builder.AddOffset(13, guideOffset.Value, 0); }
  public static void AddZOffset(FlatBufferBuilder builder, float zOffset) { builder.AddFloat(14, zOffset, 0); }
  public static void AddCorrelation(FlatBufferBuilder builder, StringOffset correlationOffset) { builder.AddOffset(15, correlationOffset.Value, 0); }
  public static Offset<LostChallengeChapterRole> EndLostChallengeChapterRole(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<LostChallengeChapterRole>(o);
  }
};

public sealed class LostChallengeChapterElement : Table {
  public static LostChallengeChapterElement GetRootAsLostChallengeChapterElement(ByteBuffer _bb) { return GetRootAsLostChallengeChapterElement(_bb, new LostChallengeChapterElement()); }
  public static LostChallengeChapterElement GetRootAsLostChallengeChapterElement(ByteBuffer _bb, LostChallengeChapterElement obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public LostChallengeChapterElement __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Desc { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDescBytes() { return __vector_as_arraysegment(6); }
  public int Type { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Color { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Img { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetImgBytes() { return __vector_as_arraysegment(12); }
  public int CanPass { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Group { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetGroupBytes() { return __vector_as_arraysegment(16); }
  public int RandomGroup { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float RP { get { int o = __offset(20); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public string Rotation { get { int o = __offset(22); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRotationBytes() { return __vector_as_arraysegment(22); }

  public static Offset<LostChallengeChapterElement> CreateLostChallengeChapterElement(FlatBufferBuilder builder,
      int id = 0,
      StringOffset descOffset = default(StringOffset),
      int type = 0,
      int color = 0,
      StringOffset imgOffset = default(StringOffset),
      int canPass = 0,
      StringOffset groupOffset = default(StringOffset),
      int randomGroup = 0,
      float RP = 0,
      StringOffset rotationOffset = default(StringOffset)) {
    builder.StartObject(10);
    LostChallengeChapterElement.AddRotation(builder, rotationOffset);
    LostChallengeChapterElement.AddRP(builder, RP);
    LostChallengeChapterElement.AddRandomGroup(builder, randomGroup);
    LostChallengeChapterElement.AddGroup(builder, groupOffset);
    LostChallengeChapterElement.AddCanPass(builder, canPass);
    LostChallengeChapterElement.AddImg(builder, imgOffset);
    LostChallengeChapterElement.AddColor(builder, color);
    LostChallengeChapterElement.AddType(builder, type);
    LostChallengeChapterElement.AddDesc(builder, descOffset);
    LostChallengeChapterElement.AddId(builder, id);
    return LostChallengeChapterElement.EndLostChallengeChapterElement(builder);
  }

  public static void StartLostChallengeChapterElement(FlatBufferBuilder builder) { builder.StartObject(10); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddDesc(FlatBufferBuilder builder, StringOffset descOffset) { builder.AddOffset(1, descOffset.Value, 0); }
  public static void AddType(FlatBufferBuilder builder, int type) { builder.AddInt(2, type, 0); }
  public static void AddColor(FlatBufferBuilder builder, int color) { builder.AddInt(3, color, 0); }
  public static void AddImg(FlatBufferBuilder builder, StringOffset imgOffset) { builder.AddOffset(4, imgOffset.Value, 0); }
  public static void AddCanPass(FlatBufferBuilder builder, int canPass) { builder.AddInt(5, canPass, 0); }
  public static void AddGroup(FlatBufferBuilder builder, StringOffset groupOffset) { builder.AddOffset(6, groupOffset.Value, 0); }
  public static void AddRandomGroup(FlatBufferBuilder builder, int randomGroup) { builder.AddInt(7, randomGroup, 0); }
  public static void AddRP(FlatBufferBuilder builder, float RP) { builder.AddFloat(8, RP, 0); }
  public static void AddRotation(FlatBufferBuilder builder, StringOffset rotationOffset) { builder.AddOffset(9, rotationOffset.Value, 0); }
  public static Offset<LostChallengeChapterElement> EndLostChallengeChapterElement(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<LostChallengeChapterElement>(o);
  }
};

public sealed class LostChallengeEnv : Table {
  public static LostChallengeEnv GetRootAsLostChallengeEnv(ByteBuffer _bb) { return GetRootAsLostChallengeEnv(_bb, new LostChallengeEnv()); }
  public static LostChallengeEnv GetRootAsLostChallengeEnv(ByteBuffer _bb, LostChallengeEnv obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public LostChallengeEnv __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Desc { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDescBytes() { return __vector_as_arraysegment(6); }
  public string Pic { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetPicBytes() { return __vector_as_arraysegment(8); }
  public string Icon { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIconBytes() { return __vector_as_arraysegment(10); }
  public string Name { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(12); }
  public string EnvLogic { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetEnvLogicBytes() { return __vector_as_arraysegment(14); }

  public static Offset<LostChallengeEnv> CreateLostChallengeEnv(FlatBufferBuilder builder,
      int id = 0,
      StringOffset descOffset = default(StringOffset),
      StringOffset picOffset = default(StringOffset),
      StringOffset iconOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      StringOffset env_logicOffset = default(StringOffset)) {
    builder.StartObject(6);
    LostChallengeEnv.AddEnvLogic(builder, env_logicOffset);
    LostChallengeEnv.AddName(builder, nameOffset);
    LostChallengeEnv.AddIcon(builder, iconOffset);
    LostChallengeEnv.AddPic(builder, picOffset);
    LostChallengeEnv.AddDesc(builder, descOffset);
    LostChallengeEnv.AddId(builder, id);
    return LostChallengeEnv.EndLostChallengeEnv(builder);
  }

  public static void StartLostChallengeEnv(FlatBufferBuilder builder) { builder.StartObject(6); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddDesc(FlatBufferBuilder builder, StringOffset descOffset) { builder.AddOffset(1, descOffset.Value, 0); }
  public static void AddPic(FlatBufferBuilder builder, StringOffset picOffset) { builder.AddOffset(2, picOffset.Value, 0); }
  public static void AddIcon(FlatBufferBuilder builder, StringOffset iconOffset) { builder.AddOffset(3, iconOffset.Value, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(4, nameOffset.Value, 0); }
  public static void AddEnvLogic(FlatBufferBuilder builder, StringOffset envLogicOffset) { builder.AddOffset(5, envLogicOffset.Value, 0); }
  public static Offset<LostChallengeEnv> EndLostChallengeEnv(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<LostChallengeEnv>(o);
  }
};

public sealed class LostInstanceMessage : Table {
  public static LostInstanceMessage GetRootAsLostInstanceMessage(ByteBuffer _bb) { return GetRootAsLostInstanceMessage(_bb, new LostInstanceMessage()); }
  public static LostInstanceMessage GetRootAsLostInstanceMessage(ByteBuffer _bb, LostInstanceMessage obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public LostInstanceMessage __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int MessageId { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Icon { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIconBytes() { return __vector_as_arraysegment(8); }
  public string Name { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(10); }
  public string Desc { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDescBytes() { return __vector_as_arraysegment(12); }

  public static Offset<LostInstanceMessage> CreateLostInstanceMessage(FlatBufferBuilder builder,
      int id = 0,
      int message_id = 0,
      StringOffset iconOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      StringOffset descOffset = default(StringOffset)) {
    builder.StartObject(5);
    LostInstanceMessage.AddDesc(builder, descOffset);
    LostInstanceMessage.AddName(builder, nameOffset);
    LostInstanceMessage.AddIcon(builder, iconOffset);
    LostInstanceMessage.AddMessageId(builder, message_id);
    LostInstanceMessage.AddId(builder, id);
    return LostInstanceMessage.EndLostInstanceMessage(builder);
  }

  public static void StartLostInstanceMessage(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddMessageId(FlatBufferBuilder builder, int messageId) { builder.AddInt(1, messageId, 0); }
  public static void AddIcon(FlatBufferBuilder builder, StringOffset iconOffset) { builder.AddOffset(2, iconOffset.Value, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(3, nameOffset.Value, 0); }
  public static void AddDesc(FlatBufferBuilder builder, StringOffset descOffset) { builder.AddOffset(4, descOffset.Value, 0); }
  public static Offset<LostInstanceMessage> EndLostInstanceMessage(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<LostInstanceMessage>(o);
  }
};

public sealed class AlienMaze : Table {
  public static AlienMaze GetRootAsAlienMaze(ByteBuffer _bb) { return GetRootAsAlienMaze(_bb, new AlienMaze()); }
  public static AlienMaze GetRootAsAlienMaze(ByteBuffer _bb, AlienMaze obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public AlienMaze __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Name { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(6); }
  public string Icon { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIconBytes() { return __vector_as_arraysegment(8); }
  public string Limit { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetLimitBytes() { return __vector_as_arraysegment(10); }
  public string Reward { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRewardBytes() { return __vector_as_arraysegment(12); }
  public int Env { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string FixMap { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetFixMapBytes() { return __vector_as_arraysegment(16); }
  public string MapStyle { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMapStyleBytes() { return __vector_as_arraysegment(18); }

  public static Offset<AlienMaze> CreateAlienMaze(FlatBufferBuilder builder,
      int id = 0,
      StringOffset nameOffset = default(StringOffset),
      StringOffset iconOffset = default(StringOffset),
      StringOffset limitOffset = default(StringOffset),
      StringOffset rewardOffset = default(StringOffset),
      int env = 0,
      StringOffset fixMapOffset = default(StringOffset),
      StringOffset mapStyleOffset = default(StringOffset)) {
    builder.StartObject(8);
    AlienMaze.AddMapStyle(builder, mapStyleOffset);
    AlienMaze.AddFixMap(builder, fixMapOffset);
    AlienMaze.AddEnv(builder, env);
    AlienMaze.AddReward(builder, rewardOffset);
    AlienMaze.AddLimit(builder, limitOffset);
    AlienMaze.AddIcon(builder, iconOffset);
    AlienMaze.AddName(builder, nameOffset);
    AlienMaze.AddId(builder, id);
    return AlienMaze.EndAlienMaze(builder);
  }

  public static void StartAlienMaze(FlatBufferBuilder builder) { builder.StartObject(8); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(1, nameOffset.Value, 0); }
  public static void AddIcon(FlatBufferBuilder builder, StringOffset iconOffset) { builder.AddOffset(2, iconOffset.Value, 0); }
  public static void AddLimit(FlatBufferBuilder builder, StringOffset limitOffset) { builder.AddOffset(3, limitOffset.Value, 0); }
  public static void AddReward(FlatBufferBuilder builder, StringOffset rewardOffset) { builder.AddOffset(4, rewardOffset.Value, 0); }
  public static void AddEnv(FlatBufferBuilder builder, int env) { builder.AddInt(5, env, 0); }
  public static void AddFixMap(FlatBufferBuilder builder, StringOffset fixMapOffset) { builder.AddOffset(6, fixMapOffset.Value, 0); }
  public static void AddMapStyle(FlatBufferBuilder builder, StringOffset mapStyleOffset) { builder.AddOffset(7, mapStyleOffset.Value, 0); }
  public static Offset<AlienMaze> EndAlienMaze(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<AlienMaze>(o);
  }
};

public sealed class Monopoly : Table {
  public static Monopoly GetRootAsMonopoly(ByteBuffer _bb) { return GetRootAsMonopoly(_bb, new Monopoly()); }
  public static Monopoly GetRootAsMonopoly(ByteBuffer _bb, Monopoly obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Monopoly __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string FixMap { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetFixMapBytes() { return __vector_as_arraysegment(6); }
  public string MapStyle { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMapStyleBytes() { return __vector_as_arraysegment(8); }
  public string Reward { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRewardBytes() { return __vector_as_arraysegment(10); }

  public static Offset<Monopoly> CreateMonopoly(FlatBufferBuilder builder,
      int id = 0,
      StringOffset fixMapOffset = default(StringOffset),
      StringOffset mapStyleOffset = default(StringOffset),
      StringOffset rewardOffset = default(StringOffset)) {
    builder.StartObject(4);
    Monopoly.AddReward(builder, rewardOffset);
    Monopoly.AddMapStyle(builder, mapStyleOffset);
    Monopoly.AddFixMap(builder, fixMapOffset);
    Monopoly.AddId(builder, id);
    return Monopoly.EndMonopoly(builder);
  }

  public static void StartMonopoly(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddFixMap(FlatBufferBuilder builder, StringOffset fixMapOffset) { builder.AddOffset(1, fixMapOffset.Value, 0); }
  public static void AddMapStyle(FlatBufferBuilder builder, StringOffset mapStyleOffset) { builder.AddOffset(2, mapStyleOffset.Value, 0); }
  public static void AddReward(FlatBufferBuilder builder, StringOffset rewardOffset) { builder.AddOffset(3, rewardOffset.Value, 0); }
  public static Offset<Monopoly> EndMonopoly(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Monopoly>(o);
  }
};

public sealed class ConditionScene : Table {
  public static ConditionScene GetRootAsConditionScene(ByteBuffer _bb) { return GetRootAsConditionScene(_bb, new ConditionScene()); }
  public static ConditionScene GetRootAsConditionScene(ByteBuffer _bb, ConditionScene obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ConditionScene __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

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
  public LayoutInfo GetCombatLayout(int j) { return GetCombatLayout(new LayoutInfo(), j); }
  public LayoutInfo GetCombatLayout(LayoutInfo obj, int j) { int o = __offset(18); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int CombatLayoutLength { get { int o = __offset(18); return o != 0 ? __vector_len(o) : 0; } }
  public Mainland GetMainlands(int j) { return GetMainlands(new Mainland(), j); }
  public Mainland GetMainlands(Mainland obj, int j) { int o = __offset(20); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int MainlandsLength { get { int o = __offset(20); return o != 0 ? __vector_len(o) : 0; } }
  public MainlandEncounter GetMainlandsEncounters(int j) { return GetMainlandsEncounters(new MainlandEncounter(), j); }
  public MainlandEncounter GetMainlandsEncounters(MainlandEncounter obj, int j) { int o = __offset(22); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int MainlandsEncountersLength { get { int o = __offset(22); return o != 0 ? __vector_len(o) : 0; } }
  public LostMainLand GetLostMainLand(int j) { return GetLostMainLand(new LostMainLand(), j); }
  public LostMainLand GetLostMainLand(LostMainLand obj, int j) { int o = __offset(24); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int LostMainLandLength { get { int o = __offset(24); return o != 0 ? __vector_len(o) : 0; } }
  public LostMainChapter GetLostMainChapter(int j) { return GetLostMainChapter(new LostMainChapter(), j); }
  public LostMainChapter GetLostMainChapter(LostMainChapter obj, int j) { int o = __offset(26); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int LostMainChapterLength { get { int o = __offset(26); return o != 0 ? __vector_len(o) : 0; } }
  public LostMainCampaigns GetLostMainCampaigns(int j) { return GetLostMainCampaigns(new LostMainCampaigns(), j); }
  public LostMainCampaigns GetLostMainCampaigns(LostMainCampaigns obj, int j) { int o = __offset(28); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int LostMainCampaignsLength { get { int o = __offset(28); return o != 0 ? __vector_len(o) : 0; } }
  public LostChallengeChapter GetLostChallengeChapter(int j) { return GetLostChallengeChapter(new LostChallengeChapter(), j); }
  public LostChallengeChapter GetLostChallengeChapter(LostChallengeChapter obj, int j) { int o = __offset(30); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int LostChallengeChapterLength { get { int o = __offset(30); return o != 0 ? __vector_len(o) : 0; } }
  public LostChallengeWalls GetLostChallengeWalls(int j) { return GetLostChallengeWalls(new LostChallengeWalls(), j); }
  public LostChallengeWalls GetLostChallengeWalls(LostChallengeWalls obj, int j) { int o = __offset(32); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int LostChallengeWallsLength { get { int o = __offset(32); return o != 0 ? __vector_len(o) : 0; } }
  public LostCombatMap GetLostCombatMap(int j) { return GetLostCombatMap(new LostCombatMap(), j); }
  public LostCombatMap GetLostCombatMap(LostCombatMap obj, int j) { int o = __offset(34); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int LostCombatMapLength { get { int o = __offset(34); return o != 0 ? __vector_len(o) : 0; } }
  public MainLandsGhost GetMainlandsGhost(int j) { return GetMainlandsGhost(new MainLandsGhost(), j); }
  public MainLandsGhost GetMainlandsGhost(MainLandsGhost obj, int j) { int o = __offset(36); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int MainlandsGhostLength { get { int o = __offset(36); return o != 0 ? __vector_len(o) : 0; } }
  public LostChallengeStyle GetLostChallengeStyle(int j) { return GetLostChallengeStyle(new LostChallengeStyle(), j); }
  public LostChallengeStyle GetLostChallengeStyle(LostChallengeStyle obj, int j) { int o = __offset(38); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int LostChallengeStyleLength { get { int o = __offset(38); return o != 0 ? __vector_len(o) : 0; } }
  public MainLandsGhostReward GetMainlandsGhostReward(int j) { return GetMainlandsGhostReward(new MainLandsGhostReward(), j); }
  public MainLandsGhostReward GetMainlandsGhostReward(MainLandsGhostReward obj, int j) { int o = __offset(40); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int MainlandsGhostRewardLength { get { int o = __offset(40); return o != 0 ? __vector_len(o) : 0; } }
  public LostChallengeReward GetLostChallengeReward(int j) { return GetLostChallengeReward(new LostChallengeReward(), j); }
  public LostChallengeReward GetLostChallengeReward(LostChallengeReward obj, int j) { int o = __offset(42); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int LostChallengeRewardLength { get { int o = __offset(42); return o != 0 ? __vector_len(o) : 0; } }
  public LostChallengeChapterRole GetLostChallengeChapterRole(int j) { return GetLostChallengeChapterRole(new LostChallengeChapterRole(), j); }
  public LostChallengeChapterRole GetLostChallengeChapterRole(LostChallengeChapterRole obj, int j) { int o = __offset(44); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int LostChallengeChapterRoleLength { get { int o = __offset(44); return o != 0 ? __vector_len(o) : 0; } }
  public LostChallengeChapterElement GetLostChallengeChapterElement(int j) { return GetLostChallengeChapterElement(new LostChallengeChapterElement(), j); }
  public LostChallengeChapterElement GetLostChallengeChapterElement(LostChallengeChapterElement obj, int j) { int o = __offset(46); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int LostChallengeChapterElementLength { get { int o = __offset(46); return o != 0 ? __vector_len(o) : 0; } }
  public LostChallengeEnv GetLostChallengeEnv(int j) { return GetLostChallengeEnv(new LostChallengeEnv(), j); }
  public LostChallengeEnv GetLostChallengeEnv(LostChallengeEnv obj, int j) { int o = __offset(48); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int LostChallengeEnvLength { get { int o = __offset(48); return o != 0 ? __vector_len(o) : 0; } }
  public LostInstanceMessage GetLostInstanceMessage(int j) { return GetLostInstanceMessage(new LostInstanceMessage(), j); }
  public LostInstanceMessage GetLostInstanceMessage(LostInstanceMessage obj, int j) { int o = __offset(50); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int LostInstanceMessageLength { get { int o = __offset(50); return o != 0 ? __vector_len(o) : 0; } }
  public AlienMaze GetAlienMaze(int j) { return GetAlienMaze(new AlienMaze(), j); }
  public AlienMaze GetAlienMaze(AlienMaze obj, int j) { int o = __offset(52); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int AlienMazeLength { get { int o = __offset(52); return o != 0 ? __vector_len(o) : 0; } }
  public Monopoly GetMonopoly(int j) { return GetMonopoly(new Monopoly(), j); }
  public Monopoly GetMonopoly(Monopoly obj, int j) { int o = __offset(54); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int MonopolyLength { get { int o = __offset(54); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<ConditionScene> CreateConditionScene(FlatBufferBuilder builder,
      StringOffset _idOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      bool enabled = false,
      int priority = 0,
      VectorOffset date_conditionsOffset = default(VectorOffset),
      VectorOffset user_conditionsOffset = default(VectorOffset),
      Offset<GM.DataCache.Options> optionsOffset = default(Offset<GM.DataCache.Options>),
      VectorOffset combat_layoutOffset = default(VectorOffset),
      VectorOffset mainlandsOffset = default(VectorOffset),
      VectorOffset mainlands_encountersOffset = default(VectorOffset),
      VectorOffset lost_main_landOffset = default(VectorOffset),
      VectorOffset lost_main_chapterOffset = default(VectorOffset),
      VectorOffset lost_main_campaignsOffset = default(VectorOffset),
      VectorOffset lost_challenge_chapterOffset = default(VectorOffset),
      VectorOffset lost_challenge_wallsOffset = default(VectorOffset),
      VectorOffset lost_combat_mapOffset = default(VectorOffset),
      VectorOffset mainlands_ghostOffset = default(VectorOffset),
      VectorOffset lost_challenge_styleOffset = default(VectorOffset),
      VectorOffset mainlands_ghost_rewardOffset = default(VectorOffset),
      VectorOffset lost_challenge_rewardOffset = default(VectorOffset),
      VectorOffset lost_challenge_chapter_roleOffset = default(VectorOffset),
      VectorOffset lost_challenge_chapter_elementOffset = default(VectorOffset),
      VectorOffset lost_challenge_envOffset = default(VectorOffset),
      VectorOffset lost_instance_messageOffset = default(VectorOffset),
      VectorOffset alien_mazeOffset = default(VectorOffset),
      VectorOffset monopolyOffset = default(VectorOffset)) {
    builder.StartObject(26);
    ConditionScene.AddMonopoly(builder, monopolyOffset);
    ConditionScene.AddAlienMaze(builder, alien_mazeOffset);
    ConditionScene.AddLostInstanceMessage(builder, lost_instance_messageOffset);
    ConditionScene.AddLostChallengeEnv(builder, lost_challenge_envOffset);
    ConditionScene.AddLostChallengeChapterElement(builder, lost_challenge_chapter_elementOffset);
    ConditionScene.AddLostChallengeChapterRole(builder, lost_challenge_chapter_roleOffset);
    ConditionScene.AddLostChallengeReward(builder, lost_challenge_rewardOffset);
    ConditionScene.AddMainlandsGhostReward(builder, mainlands_ghost_rewardOffset);
    ConditionScene.AddLostChallengeStyle(builder, lost_challenge_styleOffset);
    ConditionScene.AddMainlandsGhost(builder, mainlands_ghostOffset);
    ConditionScene.AddLostCombatMap(builder, lost_combat_mapOffset);
    ConditionScene.AddLostChallengeWalls(builder, lost_challenge_wallsOffset);
    ConditionScene.AddLostChallengeChapter(builder, lost_challenge_chapterOffset);
    ConditionScene.AddLostMainCampaigns(builder, lost_main_campaignsOffset);
    ConditionScene.AddLostMainChapter(builder, lost_main_chapterOffset);
    ConditionScene.AddLostMainLand(builder, lost_main_landOffset);
    ConditionScene.AddMainlandsEncounters(builder, mainlands_encountersOffset);
    ConditionScene.AddMainlands(builder, mainlandsOffset);
    ConditionScene.AddCombatLayout(builder, combat_layoutOffset);
    ConditionScene.AddOptions(builder, optionsOffset);
    ConditionScene.AddUserConditions(builder, user_conditionsOffset);
    ConditionScene.AddDateConditions(builder, date_conditionsOffset);
    ConditionScene.AddPriority(builder, priority);
    ConditionScene.AddName(builder, nameOffset);
    ConditionScene.Add_id(builder, _idOffset);
    ConditionScene.AddEnabled(builder, enabled);
    return ConditionScene.EndConditionScene(builder);
  }

  public static void StartConditionScene(FlatBufferBuilder builder) { builder.StartObject(26); }
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
  public static void AddCombatLayout(FlatBufferBuilder builder, VectorOffset combatLayoutOffset) { builder.AddOffset(7, combatLayoutOffset.Value, 0); }
  public static VectorOffset CreateCombatLayoutVector(FlatBufferBuilder builder, Offset<LayoutInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartCombatLayoutVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddMainlands(FlatBufferBuilder builder, VectorOffset mainlandsOffset) { builder.AddOffset(8, mainlandsOffset.Value, 0); }
  public static VectorOffset CreateMainlandsVector(FlatBufferBuilder builder, Offset<Mainland>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartMainlandsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddMainlandsEncounters(FlatBufferBuilder builder, VectorOffset mainlandsEncountersOffset) { builder.AddOffset(9, mainlandsEncountersOffset.Value, 0); }
  public static VectorOffset CreateMainlandsEncountersVector(FlatBufferBuilder builder, Offset<MainlandEncounter>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartMainlandsEncountersVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddLostMainLand(FlatBufferBuilder builder, VectorOffset lostMainLandOffset) { builder.AddOffset(10, lostMainLandOffset.Value, 0); }
  public static VectorOffset CreateLostMainLandVector(FlatBufferBuilder builder, Offset<LostMainLand>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartLostMainLandVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddLostMainChapter(FlatBufferBuilder builder, VectorOffset lostMainChapterOffset) { builder.AddOffset(11, lostMainChapterOffset.Value, 0); }
  public static VectorOffset CreateLostMainChapterVector(FlatBufferBuilder builder, Offset<LostMainChapter>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartLostMainChapterVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddLostMainCampaigns(FlatBufferBuilder builder, VectorOffset lostMainCampaignsOffset) { builder.AddOffset(12, lostMainCampaignsOffset.Value, 0); }
  public static VectorOffset CreateLostMainCampaignsVector(FlatBufferBuilder builder, Offset<LostMainCampaigns>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartLostMainCampaignsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddLostChallengeChapter(FlatBufferBuilder builder, VectorOffset lostChallengeChapterOffset) { builder.AddOffset(13, lostChallengeChapterOffset.Value, 0); }
  public static VectorOffset CreateLostChallengeChapterVector(FlatBufferBuilder builder, Offset<LostChallengeChapter>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartLostChallengeChapterVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddLostChallengeWalls(FlatBufferBuilder builder, VectorOffset lostChallengeWallsOffset) { builder.AddOffset(14, lostChallengeWallsOffset.Value, 0); }
  public static VectorOffset CreateLostChallengeWallsVector(FlatBufferBuilder builder, Offset<LostChallengeWalls>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartLostChallengeWallsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddLostCombatMap(FlatBufferBuilder builder, VectorOffset lostCombatMapOffset) { builder.AddOffset(15, lostCombatMapOffset.Value, 0); }
  public static VectorOffset CreateLostCombatMapVector(FlatBufferBuilder builder, Offset<LostCombatMap>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartLostCombatMapVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddMainlandsGhost(FlatBufferBuilder builder, VectorOffset mainlandsGhostOffset) { builder.AddOffset(16, mainlandsGhostOffset.Value, 0); }
  public static VectorOffset CreateMainlandsGhostVector(FlatBufferBuilder builder, Offset<MainLandsGhost>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartMainlandsGhostVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddLostChallengeStyle(FlatBufferBuilder builder, VectorOffset lostChallengeStyleOffset) { builder.AddOffset(17, lostChallengeStyleOffset.Value, 0); }
  public static VectorOffset CreateLostChallengeStyleVector(FlatBufferBuilder builder, Offset<LostChallengeStyle>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartLostChallengeStyleVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddMainlandsGhostReward(FlatBufferBuilder builder, VectorOffset mainlandsGhostRewardOffset) { builder.AddOffset(18, mainlandsGhostRewardOffset.Value, 0); }
  public static VectorOffset CreateMainlandsGhostRewardVector(FlatBufferBuilder builder, Offset<MainLandsGhostReward>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartMainlandsGhostRewardVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddLostChallengeReward(FlatBufferBuilder builder, VectorOffset lostChallengeRewardOffset) { builder.AddOffset(19, lostChallengeRewardOffset.Value, 0); }
  public static VectorOffset CreateLostChallengeRewardVector(FlatBufferBuilder builder, Offset<LostChallengeReward>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartLostChallengeRewardVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddLostChallengeChapterRole(FlatBufferBuilder builder, VectorOffset lostChallengeChapterRoleOffset) { builder.AddOffset(20, lostChallengeChapterRoleOffset.Value, 0); }
  public static VectorOffset CreateLostChallengeChapterRoleVector(FlatBufferBuilder builder, Offset<LostChallengeChapterRole>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartLostChallengeChapterRoleVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddLostChallengeChapterElement(FlatBufferBuilder builder, VectorOffset lostChallengeChapterElementOffset) { builder.AddOffset(21, lostChallengeChapterElementOffset.Value, 0); }
  public static VectorOffset CreateLostChallengeChapterElementVector(FlatBufferBuilder builder, Offset<LostChallengeChapterElement>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartLostChallengeChapterElementVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddLostChallengeEnv(FlatBufferBuilder builder, VectorOffset lostChallengeEnvOffset) { builder.AddOffset(22, lostChallengeEnvOffset.Value, 0); }
  public static VectorOffset CreateLostChallengeEnvVector(FlatBufferBuilder builder, Offset<LostChallengeEnv>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartLostChallengeEnvVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddLostInstanceMessage(FlatBufferBuilder builder, VectorOffset lostInstanceMessageOffset) { builder.AddOffset(23, lostInstanceMessageOffset.Value, 0); }
  public static VectorOffset CreateLostInstanceMessageVector(FlatBufferBuilder builder, Offset<LostInstanceMessage>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartLostInstanceMessageVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddAlienMaze(FlatBufferBuilder builder, VectorOffset alienMazeOffset) { builder.AddOffset(24, alienMazeOffset.Value, 0); }
  public static VectorOffset CreateAlienMazeVector(FlatBufferBuilder builder, Offset<AlienMaze>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartAlienMazeVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddMonopoly(FlatBufferBuilder builder, VectorOffset monopolyOffset) { builder.AddOffset(25, monopolyOffset.Value, 0); }
  public static VectorOffset CreateMonopolyVector(FlatBufferBuilder builder, Offset<Monopoly>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartMonopolyVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<ConditionScene> EndConditionScene(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ConditionScene>(o);
  }
};

public sealed class Scene : Table {
  public static Scene GetRootAsScene(ByteBuffer _bb) { return GetRootAsScene(_bb, new Scene()); }
  public static Scene GetRootAsScene(ByteBuffer _bb, Scene obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Scene __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public ConditionScene GetArray(int j) { return GetArray(new ConditionScene(), j); }
  public ConditionScene GetArray(ConditionScene obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ArrayLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<Scene> CreateScene(FlatBufferBuilder builder,
      VectorOffset arrayOffset = default(VectorOffset)) {
    builder.StartObject(1);
    Scene.AddArray(builder, arrayOffset);
    return Scene.EndScene(builder);
  }

  public static void StartScene(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddArray(FlatBufferBuilder builder, VectorOffset arrayOffset) { builder.AddOffset(0, arrayOffset.Value, 0); }
  public static VectorOffset CreateArrayVector(FlatBufferBuilder builder, Offset<ConditionScene>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartArrayVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<Scene> EndScene(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Scene>(o);
  }
  public static void FinishSceneBuffer(FlatBufferBuilder builder, Offset<Scene> offset) { builder.Finish(offset.Value); }
};


}
