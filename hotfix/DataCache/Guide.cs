// automatically generated, do not modify

namespace GM.DataCache
{

using System;
using FlatBuffers;

public sealed class GuideInfo : Table {
  public static GuideInfo GetRootAsGuideInfo(ByteBuffer _bb) { return GetRootAsGuideInfo(_bb, new GuideInfo()); }
  public static GuideInfo GetRootAsGuideInfo(ByteBuffer _bb, GuideInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public GuideInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int GuideId { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int RollbackId { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int NextId { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ForeId { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Type { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string TriggerType { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTriggerTypeBytes() { return __vector_as_arraysegment(14); }
  public string ExcuteType { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetExcuteTypeBytes() { return __vector_as_arraysegment(16); }
  public string View { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetViewBytes() { return __vector_as_arraysegment(18); }
  public string TargetPath { get { int o = __offset(20); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTargetPathBytes() { return __vector_as_arraysegment(20); }
  public string Parameter { get { int o = __offset(22); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetParameterBytes() { return __vector_as_arraysegment(22); }
  public int Level { get { int o = __offset(24); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int HallLevel { get { int o = __offset(26); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Campaignid { get { int o = __offset(28); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetCampaignidBytes() { return __vector_as_arraysegment(28); }
  public string TaskId { get { int o = __offset(30); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTaskIdBytes() { return __vector_as_arraysegment(30); }
  public string Items { get { int o = __offset(32); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemsBytes() { return __vector_as_arraysegment(32); }
  public string TownId { get { int o = __offset(34); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTownIdBytes() { return __vector_as_arraysegment(34); }
  public string Tips { get { int o = __offset(36); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTipsBytes() { return __vector_as_arraysegment(36); }
  public int TipsAnchor { get { int o = __offset(38); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Shade { get { int o = __offset(40); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<GuideInfo> CreateGuideInfo(FlatBufferBuilder builder,
      int guide_id = 0,
      int rollback_id = 0,
      int next_id = 0,
      int fore_id = 0,
      int type = 0,
      StringOffset trigger_typeOffset = default(StringOffset),
      StringOffset excute_typeOffset = default(StringOffset),
      StringOffset viewOffset = default(StringOffset),
      StringOffset target_pathOffset = default(StringOffset),
      StringOffset parameterOffset = default(StringOffset),
      int level = 0,
      int hall_level = 0,
      StringOffset campaignidOffset = default(StringOffset),
      StringOffset task_idOffset = default(StringOffset),
      StringOffset itemsOffset = default(StringOffset),
      StringOffset town_idOffset = default(StringOffset),
      StringOffset tipsOffset = default(StringOffset),
      int tips_anchor = 0,
      int shade = 0) {
    builder.StartObject(19);
    GuideInfo.AddShade(builder, shade);
    GuideInfo.AddTipsAnchor(builder, tips_anchor);
    GuideInfo.AddTips(builder, tipsOffset);
    GuideInfo.AddTownId(builder, town_idOffset);
    GuideInfo.AddItems(builder, itemsOffset);
    GuideInfo.AddTaskId(builder, task_idOffset);
    GuideInfo.AddCampaignid(builder, campaignidOffset);
    GuideInfo.AddHallLevel(builder, hall_level);
    GuideInfo.AddLevel(builder, level);
    GuideInfo.AddParameter(builder, parameterOffset);
    GuideInfo.AddTargetPath(builder, target_pathOffset);
    GuideInfo.AddView(builder, viewOffset);
    GuideInfo.AddExcuteType(builder, excute_typeOffset);
    GuideInfo.AddTriggerType(builder, trigger_typeOffset);
    GuideInfo.AddType(builder, type);
    GuideInfo.AddForeId(builder, fore_id);
    GuideInfo.AddNextId(builder, next_id);
    GuideInfo.AddRollbackId(builder, rollback_id);
    GuideInfo.AddGuideId(builder, guide_id);
    return GuideInfo.EndGuideInfo(builder);
  }

  public static void StartGuideInfo(FlatBufferBuilder builder) { builder.StartObject(19); }
  public static void AddGuideId(FlatBufferBuilder builder, int guideId) { builder.AddInt(0, guideId, 0); }
  public static void AddRollbackId(FlatBufferBuilder builder, int rollbackId) { builder.AddInt(1, rollbackId, 0); }
  public static void AddNextId(FlatBufferBuilder builder, int nextId) { builder.AddInt(2, nextId, 0); }
  public static void AddForeId(FlatBufferBuilder builder, int foreId) { builder.AddInt(3, foreId, 0); }
  public static void AddType(FlatBufferBuilder builder, int type) { builder.AddInt(4, type, 0); }
  public static void AddTriggerType(FlatBufferBuilder builder, StringOffset triggerTypeOffset) { builder.AddOffset(5, triggerTypeOffset.Value, 0); }
  public static void AddExcuteType(FlatBufferBuilder builder, StringOffset excuteTypeOffset) { builder.AddOffset(6, excuteTypeOffset.Value, 0); }
  public static void AddView(FlatBufferBuilder builder, StringOffset viewOffset) { builder.AddOffset(7, viewOffset.Value, 0); }
  public static void AddTargetPath(FlatBufferBuilder builder, StringOffset targetPathOffset) { builder.AddOffset(8, targetPathOffset.Value, 0); }
  public static void AddParameter(FlatBufferBuilder builder, StringOffset parameterOffset) { builder.AddOffset(9, parameterOffset.Value, 0); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(10, level, 0); }
  public static void AddHallLevel(FlatBufferBuilder builder, int hallLevel) { builder.AddInt(11, hallLevel, 0); }
  public static void AddCampaignid(FlatBufferBuilder builder, StringOffset campaignidOffset) { builder.AddOffset(12, campaignidOffset.Value, 0); }
  public static void AddTaskId(FlatBufferBuilder builder, StringOffset taskIdOffset) { builder.AddOffset(13, taskIdOffset.Value, 0); }
  public static void AddItems(FlatBufferBuilder builder, StringOffset itemsOffset) { builder.AddOffset(14, itemsOffset.Value, 0); }
  public static void AddTownId(FlatBufferBuilder builder, StringOffset townIdOffset) { builder.AddOffset(15, townIdOffset.Value, 0); }
  public static void AddTips(FlatBufferBuilder builder, StringOffset tipsOffset) { builder.AddOffset(16, tipsOffset.Value, 0); }
  public static void AddTipsAnchor(FlatBufferBuilder builder, int tipsAnchor) { builder.AddInt(17, tipsAnchor, 0); }
  public static void AddShade(FlatBufferBuilder builder, int shade) { builder.AddInt(18, shade, 0); }
  public static Offset<GuideInfo> EndGuideInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<GuideInfo>(o);
  }
};

public sealed class DialogueInfo : Table {
  public static DialogueInfo GetRootAsDialogueInfo(ByteBuffer _bb) { return GetRootAsDialogueInfo(_bb, new DialogueInfo()); }
  public static DialogueInfo GetRootAsDialogueInfo(ByteBuffer _bb, DialogueInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DialogueInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int DialogueId { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int StepId { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int StepNum { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Icon { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIconBytes() { return __vector_as_arraysegment(10); }
  public string Name { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(12); }
  public int Layout { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Context { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetContextBytes() { return __vector_as_arraysegment(16); }
  public int Shade { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float CameraShake { get { int o = __offset(20); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float StayTime { get { int o = __offset(22); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int FontSize { get { int o = __offset(24); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string LobbyCamera { get { int o = __offset(26); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetLobbyCameraBytes() { return __vector_as_arraysegment(26); }

  public static Offset<DialogueInfo> CreateDialogueInfo(FlatBufferBuilder builder,
      int dialogue_id = 0,
      int step_id = 0,
      int step_num = 0,
      StringOffset iconOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      int layout = 0,
      StringOffset contextOffset = default(StringOffset),
      int shade = 0,
      float camera_shake = 0,
      float stay_time = 0,
      int font_size = 0,
      StringOffset lobby_cameraOffset = default(StringOffset)) {
    builder.StartObject(12);
    DialogueInfo.AddLobbyCamera(builder, lobby_cameraOffset);
    DialogueInfo.AddFontSize(builder, font_size);
    DialogueInfo.AddStayTime(builder, stay_time);
    DialogueInfo.AddCameraShake(builder, camera_shake);
    DialogueInfo.AddShade(builder, shade);
    DialogueInfo.AddContext(builder, contextOffset);
    DialogueInfo.AddLayout(builder, layout);
    DialogueInfo.AddName(builder, nameOffset);
    DialogueInfo.AddIcon(builder, iconOffset);
    DialogueInfo.AddStepNum(builder, step_num);
    DialogueInfo.AddStepId(builder, step_id);
    DialogueInfo.AddDialogueId(builder, dialogue_id);
    return DialogueInfo.EndDialogueInfo(builder);
  }

  public static void StartDialogueInfo(FlatBufferBuilder builder) { builder.StartObject(12); }
  public static void AddDialogueId(FlatBufferBuilder builder, int dialogueId) { builder.AddInt(0, dialogueId, 0); }
  public static void AddStepId(FlatBufferBuilder builder, int stepId) { builder.AddInt(1, stepId, 0); }
  public static void AddStepNum(FlatBufferBuilder builder, int stepNum) { builder.AddInt(2, stepNum, 0); }
  public static void AddIcon(FlatBufferBuilder builder, StringOffset iconOffset) { builder.AddOffset(3, iconOffset.Value, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(4, nameOffset.Value, 0); }
  public static void AddLayout(FlatBufferBuilder builder, int layout) { builder.AddInt(5, layout, 0); }
  public static void AddContext(FlatBufferBuilder builder, StringOffset contextOffset) { builder.AddOffset(6, contextOffset.Value, 0); }
  public static void AddShade(FlatBufferBuilder builder, int shade) { builder.AddInt(7, shade, 0); }
  public static void AddCameraShake(FlatBufferBuilder builder, float cameraShake) { builder.AddFloat(8, cameraShake, 0); }
  public static void AddStayTime(FlatBufferBuilder builder, float stayTime) { builder.AddFloat(9, stayTime, 0); }
  public static void AddFontSize(FlatBufferBuilder builder, int fontSize) { builder.AddInt(10, fontSize, 0); }
  public static void AddLobbyCamera(FlatBufferBuilder builder, StringOffset lobbyCameraOffset) { builder.AddOffset(11, lobbyCameraOffset.Value, 0); }
  public static Offset<DialogueInfo> EndDialogueInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DialogueInfo>(o);
  }
};

public sealed class WordInfo : Table {
  public static WordInfo GetRootAsWordInfo(ByteBuffer _bb) { return GetRootAsWordInfo(_bb, new WordInfo()); }
  public static WordInfo GetRootAsWordInfo(ByteBuffer _bb, WordInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public WordInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Context { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetContextBytes() { return __vector_as_arraysegment(6); }
  public string Type { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTypeBytes() { return __vector_as_arraysegment(8); }

  public static Offset<WordInfo> CreateWordInfo(FlatBufferBuilder builder,
      int id = 0,
      StringOffset contextOffset = default(StringOffset),
      StringOffset typeOffset = default(StringOffset)) {
    builder.StartObject(3);
    WordInfo.AddType(builder, typeOffset);
    WordInfo.AddContext(builder, contextOffset);
    WordInfo.AddId(builder, id);
    return WordInfo.EndWordInfo(builder);
  }

  public static void StartWordInfo(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddContext(FlatBufferBuilder builder, StringOffset contextOffset) { builder.AddOffset(1, contextOffset.Value, 0); }
  public static void AddType(FlatBufferBuilder builder, StringOffset typeOffset) { builder.AddOffset(2, typeOffset.Value, 0); }
  public static Offset<WordInfo> EndWordInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<WordInfo>(o);
  }
};

public sealed class FunctionInfo : Table {
  public static FunctionInfo GetRootAsFunctionInfo(ByteBuffer _bb) { return GetRootAsFunctionInfo(_bb, new FunctionInfo()); }
  public static FunctionInfo GetRootAsFunctionInfo(ByteBuffer _bb, FunctionInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public FunctionInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string UiModel { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetUiModelBytes() { return __vector_as_arraysegment(6); }
  public string DisplayName { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDisplayNameBytes() { return __vector_as_arraysegment(8); }
  public string Condition { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetConditionBytes() { return __vector_as_arraysegment(10); }
  public string Parameter { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetParameterBytes() { return __vector_as_arraysegment(12); }
  public string Icon { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIconBytes() { return __vector_as_arraysegment(14); }
  public string Discript { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDiscriptBytes() { return __vector_as_arraysegment(16); }
  public int Notice { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<FunctionInfo> CreateFunctionInfo(FlatBufferBuilder builder,
      int id = 0,
      StringOffset ui_modelOffset = default(StringOffset),
      StringOffset display_nameOffset = default(StringOffset),
      StringOffset conditionOffset = default(StringOffset),
      StringOffset parameterOffset = default(StringOffset),
      StringOffset iconOffset = default(StringOffset),
      StringOffset discriptOffset = default(StringOffset),
      int notice = 0) {
    builder.StartObject(8);
    FunctionInfo.AddNotice(builder, notice);
    FunctionInfo.AddDiscript(builder, discriptOffset);
    FunctionInfo.AddIcon(builder, iconOffset);
    FunctionInfo.AddParameter(builder, parameterOffset);
    FunctionInfo.AddCondition(builder, conditionOffset);
    FunctionInfo.AddDisplayName(builder, display_nameOffset);
    FunctionInfo.AddUiModel(builder, ui_modelOffset);
    FunctionInfo.AddId(builder, id);
    return FunctionInfo.EndFunctionInfo(builder);
  }

  public static void StartFunctionInfo(FlatBufferBuilder builder) { builder.StartObject(8); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddUiModel(FlatBufferBuilder builder, StringOffset uiModelOffset) { builder.AddOffset(1, uiModelOffset.Value, 0); }
  public static void AddDisplayName(FlatBufferBuilder builder, StringOffset displayNameOffset) { builder.AddOffset(2, displayNameOffset.Value, 0); }
  public static void AddCondition(FlatBufferBuilder builder, StringOffset conditionOffset) { builder.AddOffset(3, conditionOffset.Value, 0); }
  public static void AddParameter(FlatBufferBuilder builder, StringOffset parameterOffset) { builder.AddOffset(4, parameterOffset.Value, 0); }
  public static void AddIcon(FlatBufferBuilder builder, StringOffset iconOffset) { builder.AddOffset(5, iconOffset.Value, 0); }
  public static void AddDiscript(FlatBufferBuilder builder, StringOffset discriptOffset) { builder.AddOffset(6, discriptOffset.Value, 0); }
  public static void AddNotice(FlatBufferBuilder builder, int notice) { builder.AddInt(7, notice, 0); }
  public static Offset<FunctionInfo> EndFunctionInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<FunctionInfo>(o);
  }
};

public sealed class PreviewInfo : Table {
  public static PreviewInfo GetRootAsPreviewInfo(ByteBuffer _bb) { return GetRootAsPreviewInfo(_bb, new PreviewInfo()); }
  public static PreviewInfo GetRootAsPreviewInfo(ByteBuffer _bb, PreviewInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public PreviewInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Data { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDataBytes() { return __vector_as_arraysegment(6); }

  public static Offset<PreviewInfo> CreatePreviewInfo(FlatBufferBuilder builder,
      int id = 0,
      StringOffset dataOffset = default(StringOffset)) {
    builder.StartObject(2);
    PreviewInfo.AddData(builder, dataOffset);
    PreviewInfo.AddId(builder, id);
    return PreviewInfo.EndPreviewInfo(builder);
  }

  public static void StartPreviewInfo(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddData(FlatBufferBuilder builder, StringOffset dataOffset) { builder.AddOffset(1, dataOffset.Value, 0); }
  public static Offset<PreviewInfo> EndPreviewInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<PreviewInfo>(o);
  }
};

public sealed class ChapterStory : Table {
  public static ChapterStory GetRootAsChapterStory(ByteBuffer _bb) { return GetRootAsChapterStory(_bb, new ChapterStory()); }
  public static ChapterStory GetRootAsChapterStory(ByteBuffer _bb, ChapterStory obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ChapterStory __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Id { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIdBytes() { return __vector_as_arraysegment(4); }
  public string ChapterId { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetChapterIdBytes() { return __vector_as_arraysegment(6); }
  public string Bgm { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBgmBytes() { return __vector_as_arraysegment(8); }
  public string Aside { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetAsideBytes() { return __vector_as_arraysegment(10); }
  public string AsideSound { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetAsideSoundBytes() { return __vector_as_arraysegment(12); }

  public static Offset<ChapterStory> CreateChapterStory(FlatBufferBuilder builder,
      StringOffset idOffset = default(StringOffset),
      StringOffset chapter_idOffset = default(StringOffset),
      StringOffset bgmOffset = default(StringOffset),
      StringOffset asideOffset = default(StringOffset),
      StringOffset aside_soundOffset = default(StringOffset)) {
    builder.StartObject(5);
    ChapterStory.AddAsideSound(builder, aside_soundOffset);
    ChapterStory.AddAside(builder, asideOffset);
    ChapterStory.AddBgm(builder, bgmOffset);
    ChapterStory.AddChapterId(builder, chapter_idOffset);
    ChapterStory.AddId(builder, idOffset);
    return ChapterStory.EndChapterStory(builder);
  }

  public static void StartChapterStory(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddId(FlatBufferBuilder builder, StringOffset idOffset) { builder.AddOffset(0, idOffset.Value, 0); }
  public static void AddChapterId(FlatBufferBuilder builder, StringOffset chapterIdOffset) { builder.AddOffset(1, chapterIdOffset.Value, 0); }
  public static void AddBgm(FlatBufferBuilder builder, StringOffset bgmOffset) { builder.AddOffset(2, bgmOffset.Value, 0); }
  public static void AddAside(FlatBufferBuilder builder, StringOffset asideOffset) { builder.AddOffset(3, asideOffset.Value, 0); }
  public static void AddAsideSound(FlatBufferBuilder builder, StringOffset asideSoundOffset) { builder.AddOffset(4, asideSoundOffset.Value, 0); }
  public static Offset<ChapterStory> EndChapterStory(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ChapterStory>(o);
  }
};

public sealed class ConditionGuide : Table {
  public static ConditionGuide GetRootAsConditionGuide(ByteBuffer _bb) { return GetRootAsConditionGuide(_bb, new ConditionGuide()); }
  public static ConditionGuide GetRootAsConditionGuide(ByteBuffer _bb, ConditionGuide obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ConditionGuide __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

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
  public GuideInfo GetGuide(int j) { return GetGuide(new GuideInfo(), j); }
  public GuideInfo GetGuide(GuideInfo obj, int j) { int o = __offset(18); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int GuideLength { get { int o = __offset(18); return o != 0 ? __vector_len(o) : 0; } }
  public DialogueInfo GetDialogue(int j) { return GetDialogue(new DialogueInfo(), j); }
  public DialogueInfo GetDialogue(DialogueInfo obj, int j) { int o = __offset(20); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int DialogueLength { get { int o = __offset(20); return o != 0 ? __vector_len(o) : 0; } }
  public WordInfo GetWords(int j) { return GetWords(new WordInfo(), j); }
  public WordInfo GetWords(WordInfo obj, int j) { int o = __offset(22); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int WordsLength { get { int o = __offset(22); return o != 0 ? __vector_len(o) : 0; } }
  public FunctionInfo GetFunctions(int j) { return GetFunctions(new FunctionInfo(), j); }
  public FunctionInfo GetFunctions(FunctionInfo obj, int j) { int o = __offset(24); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int FunctionsLength { get { int o = __offset(24); return o != 0 ? __vector_len(o) : 0; } }
  public PreviewInfo GetPreview(int j) { return GetPreview(new PreviewInfo(), j); }
  public PreviewInfo GetPreview(PreviewInfo obj, int j) { int o = __offset(26); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int PreviewLength { get { int o = __offset(26); return o != 0 ? __vector_len(o) : 0; } }
  public FunctionsActivedInfo GetFunctionsActived(int j) { return GetFunctionsActived(new FunctionsActivedInfo(), j); }
  public FunctionsActivedInfo GetFunctionsActived(FunctionsActivedInfo obj, int j) { int o = __offset(28); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int FunctionsActivedLength { get { int o = __offset(28); return o != 0 ? __vector_len(o) : 0; } }
  public GuideNodeInfo GetGuideNodes(int j) { return GetGuideNodes(new GuideNodeInfo(), j); }
  public GuideNodeInfo GetGuideNodes(GuideNodeInfo obj, int j) { int o = __offset(30); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int GuideNodesLength { get { int o = __offset(30); return o != 0 ? __vector_len(o) : 0; } }
  public GuideAudioInfo GetGuideAudio(int j) { return GetGuideAudio(new GuideAudioInfo(), j); }
  public GuideAudioInfo GetGuideAudio(GuideAudioInfo obj, int j) { int o = __offset(32); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int GuideAudioLength { get { int o = __offset(32); return o != 0 ? __vector_len(o) : 0; } }
  public ChapterStory GetChapterStory(int j) { return GetChapterStory(new ChapterStory(), j); }
  public ChapterStory GetChapterStory(ChapterStory obj, int j) { int o = __offset(34); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ChapterStoryLength { get { int o = __offset(34); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<ConditionGuide> CreateConditionGuide(FlatBufferBuilder builder,
      StringOffset _idOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      bool enabled = false,
      int priority = 0,
      VectorOffset date_conditionsOffset = default(VectorOffset),
      VectorOffset user_conditionsOffset = default(VectorOffset),
      Offset<GM.DataCache.Options> optionsOffset = default(Offset<GM.DataCache.Options>),
      VectorOffset guideOffset = default(VectorOffset),
      VectorOffset dialogueOffset = default(VectorOffset),
      VectorOffset wordsOffset = default(VectorOffset),
      VectorOffset functionsOffset = default(VectorOffset),
      VectorOffset previewOffset = default(VectorOffset),
      VectorOffset functionsActivedOffset = default(VectorOffset),
      VectorOffset guideNodesOffset = default(VectorOffset),
      VectorOffset guideAudioOffset = default(VectorOffset),
      VectorOffset chapter_storyOffset = default(VectorOffset)) {
    builder.StartObject(16);
    ConditionGuide.AddChapterStory(builder, chapter_storyOffset);
    ConditionGuide.AddGuideAudio(builder, guideAudioOffset);
    ConditionGuide.AddGuideNodes(builder, guideNodesOffset);
    ConditionGuide.AddFunctionsActived(builder, functionsActivedOffset);
    ConditionGuide.AddPreview(builder, previewOffset);
    ConditionGuide.AddFunctions(builder, functionsOffset);
    ConditionGuide.AddWords(builder, wordsOffset);
    ConditionGuide.AddDialogue(builder, dialogueOffset);
    ConditionGuide.AddGuide(builder, guideOffset);
    ConditionGuide.AddOptions(builder, optionsOffset);
    ConditionGuide.AddUserConditions(builder, user_conditionsOffset);
    ConditionGuide.AddDateConditions(builder, date_conditionsOffset);
    ConditionGuide.AddPriority(builder, priority);
    ConditionGuide.AddName(builder, nameOffset);
    ConditionGuide.Add_id(builder, _idOffset);
    ConditionGuide.AddEnabled(builder, enabled);
    return ConditionGuide.EndConditionGuide(builder);
  }

  public static void StartConditionGuide(FlatBufferBuilder builder) { builder.StartObject(16); }
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
  public static void AddGuide(FlatBufferBuilder builder, VectorOffset guideOffset) { builder.AddOffset(7, guideOffset.Value, 0); }
  public static VectorOffset CreateGuideVector(FlatBufferBuilder builder, Offset<GuideInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartGuideVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddDialogue(FlatBufferBuilder builder, VectorOffset dialogueOffset) { builder.AddOffset(8, dialogueOffset.Value, 0); }
  public static VectorOffset CreateDialogueVector(FlatBufferBuilder builder, Offset<DialogueInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartDialogueVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddWords(FlatBufferBuilder builder, VectorOffset wordsOffset) { builder.AddOffset(9, wordsOffset.Value, 0); }
  public static VectorOffset CreateWordsVector(FlatBufferBuilder builder, Offset<WordInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartWordsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddFunctions(FlatBufferBuilder builder, VectorOffset functionsOffset) { builder.AddOffset(10, functionsOffset.Value, 0); }
  public static VectorOffset CreateFunctionsVector(FlatBufferBuilder builder, Offset<FunctionInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartFunctionsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddPreview(FlatBufferBuilder builder, VectorOffset previewOffset) { builder.AddOffset(11, previewOffset.Value, 0); }
  public static VectorOffset CreatePreviewVector(FlatBufferBuilder builder, Offset<PreviewInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartPreviewVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddFunctionsActived(FlatBufferBuilder builder, VectorOffset functionsActivedOffset) { builder.AddOffset(12, functionsActivedOffset.Value, 0); }
  public static VectorOffset CreateFunctionsActivedVector(FlatBufferBuilder builder, Offset<FunctionsActivedInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartFunctionsActivedVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddGuideNodes(FlatBufferBuilder builder, VectorOffset guideNodesOffset) { builder.AddOffset(13, guideNodesOffset.Value, 0); }
  public static VectorOffset CreateGuideNodesVector(FlatBufferBuilder builder, Offset<GuideNodeInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartGuideNodesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddGuideAudio(FlatBufferBuilder builder, VectorOffset guideAudioOffset) { builder.AddOffset(14, guideAudioOffset.Value, 0); }
  public static VectorOffset CreateGuideAudioVector(FlatBufferBuilder builder, Offset<GuideAudioInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartGuideAudioVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddChapterStory(FlatBufferBuilder builder, VectorOffset chapterStoryOffset) { builder.AddOffset(15, chapterStoryOffset.Value, 0); }
  public static VectorOffset CreateChapterStoryVector(FlatBufferBuilder builder, Offset<ChapterStory>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartChapterStoryVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<ConditionGuide> EndConditionGuide(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ConditionGuide>(o);
  }
};

public sealed class Guide : Table {
  public static Guide GetRootAsGuide(ByteBuffer _bb) { return GetRootAsGuide(_bb, new Guide()); }
  public static Guide GetRootAsGuide(ByteBuffer _bb, Guide obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Guide __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public ConditionGuide GetArray(int j) { return GetArray(new ConditionGuide(), j); }
  public ConditionGuide GetArray(ConditionGuide obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ArrayLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<Guide> CreateGuide(FlatBufferBuilder builder,
      VectorOffset arrayOffset = default(VectorOffset)) {
    builder.StartObject(1);
    Guide.AddArray(builder, arrayOffset);
    return Guide.EndGuide(builder);
  }

  public static void StartGuide(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddArray(FlatBufferBuilder builder, VectorOffset arrayOffset) { builder.AddOffset(0, arrayOffset.Value, 0); }
  public static VectorOffset CreateArrayVector(FlatBufferBuilder builder, Offset<ConditionGuide>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartArrayVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<Guide> EndGuide(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Guide>(o);
  }
  public static void FinishGuideBuffer(FlatBufferBuilder builder, Offset<Guide> offset) { builder.Finish(offset.Value); }
};

public sealed class FunctionsActivedInfo : Table {
  public static FunctionsActivedInfo GetRootAsFunctionsActivedInfo(ByteBuffer _bb) { return GetRootAsFunctionsActivedInfo(_bb, new FunctionsActivedInfo()); }
  public static FunctionsActivedInfo GetRootAsFunctionsActivedInfo(ByteBuffer _bb, FunctionsActivedInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public FunctionsActivedInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Name { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(6); }
  public int Level { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Dialogue { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDialogueBytes() { return __vector_as_arraysegment(10); }
  public string Describe { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDescribeBytes() { return __vector_as_arraysegment(12); }

  public static Offset<FunctionsActivedInfo> CreateFunctionsActivedInfo(FlatBufferBuilder builder,
      int id = 0,
      StringOffset nameOffset = default(StringOffset),
      int level = 0,
      StringOffset dialogueOffset = default(StringOffset),
      StringOffset describeOffset = default(StringOffset)) {
    builder.StartObject(5);
    FunctionsActivedInfo.AddDescribe(builder, describeOffset);
    FunctionsActivedInfo.AddDialogue(builder, dialogueOffset);
    FunctionsActivedInfo.AddLevel(builder, level);
    FunctionsActivedInfo.AddName(builder, nameOffset);
    FunctionsActivedInfo.AddId(builder, id);
    return FunctionsActivedInfo.EndFunctionsActivedInfo(builder);
  }

  public static void StartFunctionsActivedInfo(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(1, nameOffset.Value, 0); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(2, level, 0); }
  public static void AddDialogue(FlatBufferBuilder builder, StringOffset dialogueOffset) { builder.AddOffset(3, dialogueOffset.Value, 0); }
  public static void AddDescribe(FlatBufferBuilder builder, StringOffset describeOffset) { builder.AddOffset(4, describeOffset.Value, 0); }
  public static Offset<FunctionsActivedInfo> EndFunctionsActivedInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<FunctionsActivedInfo>(o);
  }
};

public sealed class GuideNodeInfo : Table {
  public static GuideNodeInfo GetRootAsGuideNodeInfo(ByteBuffer _bb) { return GetRootAsGuideNodeInfo(_bb, new GuideNodeInfo()); }
  public static GuideNodeInfo GetRootAsGuideNodeInfo(ByteBuffer _bb, GuideNodeInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public GuideNodeInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int GroupId { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int StepId { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int NextId { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ForeId { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string StepType { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetStepTypeBytes() { return __vector_as_arraysegment(12); }
  public int RollBackId { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SkipToId { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ConditionCmd { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetConditionCmdBytes() { return __vector_as_arraysegment(18); }
  public string CParameter { get { int o = __offset(20); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetCParameterBytes() { return __vector_as_arraysegment(20); }
  public string CReceiptType { get { int o = __offset(22); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetCReceiptTypeBytes() { return __vector_as_arraysegment(22); }
  public string CNeedParameter { get { int o = __offset(24); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetCNeedParameterBytes() { return __vector_as_arraysegment(24); }
  public string ExecuteCmd { get { int o = __offset(26); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetExecuteCmdBytes() { return __vector_as_arraysegment(26); }
  public string EParameter { get { int o = __offset(28); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetEParameterBytes() { return __vector_as_arraysegment(28); }
  public int EFailType { get { int o = __offset(30); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string UmengId { get { int o = __offset(32); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetUmengIdBytes() { return __vector_as_arraysegment(32); }

  public static Offset<GuideNodeInfo> CreateGuideNodeInfo(FlatBufferBuilder builder,
      int group_id = 0,
      int step_id = 0,
      int next_id = 0,
      int fore_id = 0,
      StringOffset step_typeOffset = default(StringOffset),
      int roll_back_id = 0,
      int skip_to_id = 0,
      StringOffset condition_cmdOffset = default(StringOffset),
      StringOffset c_parameterOffset = default(StringOffset),
      StringOffset c_receipt_typeOffset = default(StringOffset),
      StringOffset c_need_parameterOffset = default(StringOffset),
      StringOffset execute_cmdOffset = default(StringOffset),
      StringOffset e_parameterOffset = default(StringOffset),
      int e_fail_type = 0,
      StringOffset umeng_idOffset = default(StringOffset)) {
    builder.StartObject(15);
    GuideNodeInfo.AddUmengId(builder, umeng_idOffset);
    GuideNodeInfo.AddEFailType(builder, e_fail_type);
    GuideNodeInfo.AddEParameter(builder, e_parameterOffset);
    GuideNodeInfo.AddExecuteCmd(builder, execute_cmdOffset);
    GuideNodeInfo.AddCNeedParameter(builder, c_need_parameterOffset);
    GuideNodeInfo.AddCReceiptType(builder, c_receipt_typeOffset);
    GuideNodeInfo.AddCParameter(builder, c_parameterOffset);
    GuideNodeInfo.AddConditionCmd(builder, condition_cmdOffset);
    GuideNodeInfo.AddSkipToId(builder, skip_to_id);
    GuideNodeInfo.AddRollBackId(builder, roll_back_id);
    GuideNodeInfo.AddStepType(builder, step_typeOffset);
    GuideNodeInfo.AddForeId(builder, fore_id);
    GuideNodeInfo.AddNextId(builder, next_id);
    GuideNodeInfo.AddStepId(builder, step_id);
    GuideNodeInfo.AddGroupId(builder, group_id);
    return GuideNodeInfo.EndGuideNodeInfo(builder);
  }

  public static void StartGuideNodeInfo(FlatBufferBuilder builder) { builder.StartObject(15); }
  public static void AddGroupId(FlatBufferBuilder builder, int groupId) { builder.AddInt(0, groupId, 0); }
  public static void AddStepId(FlatBufferBuilder builder, int stepId) { builder.AddInt(1, stepId, 0); }
  public static void AddNextId(FlatBufferBuilder builder, int nextId) { builder.AddInt(2, nextId, 0); }
  public static void AddForeId(FlatBufferBuilder builder, int foreId) { builder.AddInt(3, foreId, 0); }
  public static void AddStepType(FlatBufferBuilder builder, StringOffset stepTypeOffset) { builder.AddOffset(4, stepTypeOffset.Value, 0); }
  public static void AddRollBackId(FlatBufferBuilder builder, int rollBackId) { builder.AddInt(5, rollBackId, 0); }
  public static void AddSkipToId(FlatBufferBuilder builder, int skipToId) { builder.AddInt(6, skipToId, 0); }
  public static void AddConditionCmd(FlatBufferBuilder builder, StringOffset conditionCmdOffset) { builder.AddOffset(7, conditionCmdOffset.Value, 0); }
  public static void AddCParameter(FlatBufferBuilder builder, StringOffset cParameterOffset) { builder.AddOffset(8, cParameterOffset.Value, 0); }
  public static void AddCReceiptType(FlatBufferBuilder builder, StringOffset cReceiptTypeOffset) { builder.AddOffset(9, cReceiptTypeOffset.Value, 0); }
  public static void AddCNeedParameter(FlatBufferBuilder builder, StringOffset cNeedParameterOffset) { builder.AddOffset(10, cNeedParameterOffset.Value, 0); }
  public static void AddExecuteCmd(FlatBufferBuilder builder, StringOffset executeCmdOffset) { builder.AddOffset(11, executeCmdOffset.Value, 0); }
  public static void AddEParameter(FlatBufferBuilder builder, StringOffset eParameterOffset) { builder.AddOffset(12, eParameterOffset.Value, 0); }
  public static void AddEFailType(FlatBufferBuilder builder, int eFailType) { builder.AddInt(13, eFailType, 0); }
  public static void AddUmengId(FlatBufferBuilder builder, StringOffset umengIdOffset) { builder.AddOffset(14, umengIdOffset.Value, 0); }
  public static Offset<GuideNodeInfo> EndGuideNodeInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<GuideNodeInfo>(o);
  }
};

public sealed class GuideAudioInfo : Table {
  public static GuideAudioInfo GetRootAsGuideAudioInfo(ByteBuffer _bb) { return GetRootAsGuideAudioInfo(_bb, new GuideAudioInfo()); }
  public static GuideAudioInfo GetRootAsGuideAudioInfo(ByteBuffer _bb, GuideAudioInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public GuideAudioInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string EventName { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetEventNameBytes() { return __vector_as_arraysegment(4); }
  public int DialogId { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int DialogStepId { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int GuideId { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string DialogBgm { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDialogBgmBytes() { return __vector_as_arraysegment(12); }

  public static Offset<GuideAudioInfo> CreateGuideAudioInfo(FlatBufferBuilder builder,
      StringOffset event_nameOffset = default(StringOffset),
      int dialog_id = 0,
      int dialog_step_id = 0,
      int guide_id = 0,
      StringOffset dialog_bgmOffset = default(StringOffset)) {
    builder.StartObject(5);
    GuideAudioInfo.AddDialogBgm(builder, dialog_bgmOffset);
    GuideAudioInfo.AddGuideId(builder, guide_id);
    GuideAudioInfo.AddDialogStepId(builder, dialog_step_id);
    GuideAudioInfo.AddDialogId(builder, dialog_id);
    GuideAudioInfo.AddEventName(builder, event_nameOffset);
    return GuideAudioInfo.EndGuideAudioInfo(builder);
  }

  public static void StartGuideAudioInfo(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddEventName(FlatBufferBuilder builder, StringOffset eventNameOffset) { builder.AddOffset(0, eventNameOffset.Value, 0); }
  public static void AddDialogId(FlatBufferBuilder builder, int dialogId) { builder.AddInt(1, dialogId, 0); }
  public static void AddDialogStepId(FlatBufferBuilder builder, int dialogStepId) { builder.AddInt(2, dialogStepId, 0); }
  public static void AddGuideId(FlatBufferBuilder builder, int guideId) { builder.AddInt(3, guideId, 0); }
  public static void AddDialogBgm(FlatBufferBuilder builder, StringOffset dialogBgmOffset) { builder.AddOffset(4, dialogBgmOffset.Value, 0); }
  public static Offset<GuideAudioInfo> EndGuideAudioInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<GuideAudioInfo>(o);
  }
};


}
