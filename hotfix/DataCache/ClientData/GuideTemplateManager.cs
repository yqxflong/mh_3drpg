using System.Collections;
using System.Collections.Generic;
using GM.DataCache;
namespace Hotfix_LT.Data
{
	public class GuideTemplate
	{
		public int guide_id;
		public int rollback_id;
		public int next_id;
		public int fore_id;
		public int type;
		public string trigger_type;
		public string excute_type;
		public string view;
		public string target_path;
		public string parameter;
		public int level;
		public int hall_level;
		public string campaignid;
		public string task_id;
		public string items;
		public string town_id;
		public string tips;
		public int tips_anchor;
		public int shade;

		public static GuideTemplate Search = new GuideTemplate();
		public static GuideTemplateComparer Comparer = new GuideTemplateComparer();
	}

	public class GuideTemplateComparer : IComparer<GuideTemplate>
	{
		public int Compare(GuideTemplate x, GuideTemplate y)
		{
			return x.guide_id - y.guide_id;
		}
	}

	public class GuideAudioTemplate
	{
		public string event_name;
		public int dialogue_id;
		public int dialogue_step_id;
		public int guide_node_step_id;
		public string dialog_bgm;
	}

	public class DialogueTemplate
	{
		public int dialogue_id;
		public int step_id;
		public int step_num;
		public string icon;
		public string name;
		public int layout;
		public string context;
		public int shade;
		public float camera_shake;
		public float stay_time;

		public static DialogueTemplate Search = new DialogueTemplate();
		public static DialogueTemplateComparer Comparer = new DialogueTemplateComparer();
	}

	public class DialogueTemplateComparer : IComparer<DialogueTemplate>
	{
		public int Compare(DialogueTemplate x, DialogueTemplate y)
		{
			return x.dialogue_id - y.dialogue_id;
		}
	}

	public class WordTemplate
	{
		public int id;
		public string context;
		public int type;
		public static WordTemplate Search = new WordTemplate();
		public static WordTemplateComparer Comparer = new WordTemplateComparer();
	}

	public class WordTemplateComparer : IComparer<WordTemplate>
	{
		public int Compare(WordTemplate x, WordTemplate y)
		{
			return x.id - y.id;
		}
	}

	public class ChapterStory
	{
		public string id;
		public string chapterId;
		public string bgm;
		public string aside;
		public string asideSound;
	}

	public class GuideTemplateManager
	{
		private static GuideTemplateManager sInstance = null;
		/*private GuideTemplate[] mGuideTbl = null;
		private DialogueTemplate[] mDialogueTbl = null;
		private WordTemplate[] mWordTbl = null;
		private GuideAudioInfo[] mGuideAudioTbl = null;

		public Dictionary<string, string> mDLGAudio = new Dictionary<string, string>();
		public Dictionary<string, string> mGDEAudio = new Dictionary<string, string>();*/

		public Dictionary<string, ChapterStory> mChapterStory = new Dictionary<string, ChapterStory>();

		public static GuideTemplateManager Instance
		{
			get { return sInstance = sInstance ?? new GuideTemplateManager(); }
		}

		public static void ClearUp()
		{
			if (sInstance != null)
			{
				sInstance.mChapterStory.Clear();
			}
		}

		public bool InitFromDataCache(GM.DataCache.ConditionGuide guide)
		{
			if (guide == null)
			{
				EB.Debug.LogError("InitFromDataCache: guide is null");
				return false;
			}

			var conditionSet = guide;

			if (!InitChapterStory(conditionSet))
			{
				EB.Debug.LogError("InitFromDataCache: init chapterStory failed");
				return false;
			}

			return true;
		}

		private bool InitChapterStory(GM.DataCache.ConditionGuide story)
		{
			if (story == null)
			{
				EB.Debug.LogError("InitChapterStory: story is null");
				return false;
			}

			mChapterStory = new Dictionary<string, ChapterStory>();
			for (int i = 0; i < story.ChapterStoryLength; ++i)
			{
				var item = ParseChapterStory(story.GetChapterStory(i));
				if (mChapterStory.ContainsKey(item.chapterId))
				{
					EB.Debug.LogError("InitChapterStory: {0} exists", item.chapterId);
					mChapterStory.Remove(item.chapterId);
				}
				mChapterStory.Add(item.chapterId, item);
			}
			return true;
		}

		private ChapterStory ParseChapterStory(GM.DataCache.ChapterStory obj)
		{
			ChapterStory item = new ChapterStory();
			item.id = obj.Id;
			item.chapterId = obj.ChapterId;
			item.bgm = obj.Bgm;
			item.aside = obj.Aside;
			item.asideSound = obj.Aside;
			return item;
		}


		/*  public bool InitFromDataCache(object tbls)
		  {
			  if (tbls == null)
			  {
				  EB.Debug.LogError("InitFromDataCache: tbls is null");
				  return false;
			  }

			  ArrayList guideTbl = Hotfix_LT.EBCore.Dot.Array("guide", tbls, null);
			  if (!InitGuideTbl(guideTbl))
			  {
				  EB.Debug.LogError("InitFromDataCache: init guide table failed");
				  return false;
			  }

			  ArrayList dialogueTbl = Hotfix_LT.EBCore.Dot.Array("dialogue", tbls, null);
			  if (!InitDialogueTbl(dialogueTbl))
			  {
				  EB.Debug.LogError("InitFromDataCache: init dialogue table failed");
				  return false;
			  }

			  ArrayList wordTbl = Hotfix_LT.EBCore.Dot.Array("words", tbls, null);
			  if (!InitWordTbl(wordTbl))
			  {
				  EB.Debug.LogError("InitFromDataCache: init word table failed");
				  return false;
			  }

			  ArrayList guideAudioTbl = Hotfix_LT.EBCore.Dot.Array("guide_audio", tbls, null);
			  if (!InitGuideAudioTbl(guideAudioTbl))
			  {
				  EB.Debug.LogError("InitFromDataCache: init guide audio table failed");
				  return false;
			  }
			  return true;
		  }

		  private bool InitGuideAudioTbl(ArrayList tbl)
		  {
			  if (tbl == null)
			  {
				  EB.Debug.LogError("InitGuideAudioTbl: guide audio tbl is null");
				  return false;
			  }

			  for (int i = 0; i < mGuideAudioTbl.Length; i++)
			  {
				  GuideAudioTemplate tpl = ParseGuideAudio(mGuideAudioTbl[i]);
				  if (tpl.dialogue_id != 0)
				  {
					  string DLGid = tpl.dialogue_id.ToString() + tpl.dialogue_step_id.ToString();
					  mDLGAudio.Add(DLGid, tpl.event_name);   
				  }
				  if (tpl.guide_node_step_id != 0)
				  {
					  mGDEAudio.Add(tpl.guide_node_step_id.ToString(), tpl.event_name);
				  }
			  }

			  return true;
		  }

		  private GuideAudioTemplate ParseGuideAudio(object obj)
		  {
			  GuideAudioTemplate tpl = new GuideAudioTemplate();
			  tpl.event_name = EB.Dot.String("event_name", obj, tpl.event_name);
			  tpl.dialogue_id = EB.Dot.Integer("dialog_id", obj, tpl.dialogue_id);
			  tpl.dialogue_step_id = EB.Dot.Integer("dialog_step_id", obj, tpl.dialogue_step_id);
			  tpl.guide_node_step_id = EB.Dot.Integer("guide_id", obj, tpl.guide_node_step_id);
			  return tpl;
		  }


		  private bool InitGuideTbl(ArrayList tbl)
		  {
			  if (tbl == null)
			  {
				  EB.Debug.LogError("InitGuideTbl: guide tbl is null");
				  return false;
			  }

			  mGuideTbl = new GuideTemplate[tbl.Count];
			  for (int i = 0; i < mGuideTbl.Length; ++i)
			  {
				  mGuideTbl[i] = ParseGuide(tbl[i]);
			  }

			  System.Array.Sort(mGuideTbl, GuideTemplate.Comparer);
			  return true;
		  }

		  private GuideTemplate ParseGuide(object obj)
		  {
			  GuideTemplate tpl = new GuideTemplate();
			  tpl.guide_id = EB.Dot.Integer("guide_id", obj, tpl.guide_id);
			  tpl.rollback_id = EB.Dot.Integer("rollback_id", obj, tpl.rollback_id);
			  tpl.next_id = EB.Dot.Integer("next_id", obj, tpl.next_id);
			  tpl.fore_id = EB.Dot.Integer("fore_id", obj, tpl.fore_id);
			  tpl.type = EB.Dot.Integer("type", obj, tpl.type);
			  tpl.trigger_type = EB.Dot.String("trigger_type", obj, tpl.trigger_type);
			  tpl.excute_type = EB.Dot.String("excute_type", obj, tpl.excute_type);
			  tpl.view = EB.Dot.String("view", obj, tpl.view);
			  tpl.target_path = EB.Dot.String("target_path", obj, tpl.target_path);
			  tpl.parameter = EB.Dot.String("parameter", obj, tpl.parameter);
			  tpl.level = EB.Dot.Integer("level", obj, tpl.level);
			  tpl.hall_level = EB.Dot.Integer("hall_level", obj, tpl.hall_level);
			  tpl.campaignid = EB.Dot.String("campaignid", obj, tpl.campaignid);
			  tpl.task_id = EB.Dot.String("task_id", obj, tpl.task_id);
			  tpl.items = EB.Dot.String("items", obj, tpl.items);
			  tpl.town_id = EB.Dot.String("town_id", obj, tpl.town_id);
			  tpl.tips = EB.Dot.String("tips", obj, tpl.tips);
			  tpl.tips_anchor = EB.Dot.Integer("tips_anchor", obj, tpl.tips_anchor);
			  tpl.shade = EB.Dot.Integer("shade", obj, tpl.shade);
			  return tpl;
		  }

		  private bool InitDialogueTbl(ArrayList tbl)
		  {
			  if (tbl == null)
			  {
				  EB.Debug.LogError("InitDialogue: dialogue tbl is null");
				  return false;
			  }

			  mDialogueTbl = new DialogueTemplate[tbl.Count];
			  for (int i = 0; i < mDialogueTbl.Length; ++i)
			  {
				  mDialogueTbl[i] = ParseDialogue(tbl[i]);
			  }

			  System.Array.Sort(mDialogueTbl, DialogueTemplate.Comparer);
			  return true;
		  }

		  private DialogueTemplate ParseDialogue(object obj)
		  {
			  DialogueTemplate tpl = new DialogueTemplate();
			  tpl.dialogue_id = EB.Dot.Integer("dialogue_id", obj, tpl.dialogue_id);
			  tpl.step_id = EB.Dot.Integer("step_id", obj, tpl.step_id);
			  tpl.step_num = EB.Dot.Integer("step_num", obj, tpl.step_num);
			  tpl.icon = EB.Dot.String("icon", obj, tpl.icon);
			  tpl.name = EB.Dot.String("name", obj, tpl.name);
			  tpl.layout = EB.Dot.Integer("layout", obj, tpl.layout);
			  tpl.context = EB.Dot.String("context", obj, tpl.context);
			  tpl.shade = EB.Dot.Integer("shade", obj, tpl.shade);
			  tpl.camera_shake = EB.Dot.Single("camera_shake", obj, tpl.camera_shake);
			  tpl.stay_time = EB.Dot.Single("stay_time", obj, tpl.stay_time);
			  return tpl;
		  }

		  private bool InitWordTbl(ArrayList tbl)
		  {
			  if (tbl == null)
			  {
				  EB.Debug.LogError("InitWordTbl: word tbl is null");
				  return false;
			  }

			  mWordTbl = new WordTemplate[tbl.Count];
			  for (int i = 0; i < mWordTbl.Length; ++i)
			  {
				  mWordTbl[i] = ParseWord(tbl[i]);
			  }

			  System.Array.Sort(mWordTbl, WordTemplate.Comparer);
			  return true;
		  }

		  private WordTemplate ParseWord(object obj)
		  {
			  WordTemplate tpl = new WordTemplate();
			  tpl.id = EB.Dot.Integer("id", obj, tpl.id);
			  tpl.context = EB.Dot.String("context", obj, tpl.context);
			  tpl.type = EB.Dot.Integer("type", obj, tpl.type);
			  return tpl;
		  }

		  /*public GuideTemplate GetGuide(int guide_id)
		  {
			  GuideTemplate.Search.guide_id = guide_id;
			  int index = System.Array.BinarySearch<GuideTemplate>(mGuideTbl, GuideTemplate.Search, GuideTemplate.Comparer);
			  if (index >= 0)
			  {
				  return mGuideTbl[index];
			  }
			  else
			  {
				  EB.Debug.LogWarning("GetGuide: guide not found, guide_id = {0}", guide_id);
				  return null;
			  }
		  }

		  public DialogueTemplate GetDialogue(int dialogue_id)
		  {
			  DialogueTemplate.Search.dialogue_id = dialogue_id;
			  int index = System.Array.BinarySearch<DialogueTemplate>(mDialogueTbl, DialogueTemplate.Search, DialogueTemplate.Comparer);
			  if (index >= 0)
			  {
				  return mDialogueTbl[index];
			  }
			  else
			  {
				  EB.Debug.LogWarning("GetDialogue: dialogue not found, dialogue_id = {0}", dialogue_id);
				  return null;
			  }
		  }

		  public WordTemplate GetWord(int word_id)
		  {
			  WordTemplate.Search.id = word_id;
			  int index = System.Array.BinarySearch<WordTemplate>(mWordTbl, WordTemplate.Search, WordTemplate.Comparer);
			  if (index >= 0)
			  {
				  return mWordTbl[index];
			  }
			  else
			  {
				  EB.Debug.LogWarning("GetWord: word not found, word_id = {0}", word_id);
				  return null;
			  }
		  }*/

		public ChapterStory GetChapterStoryByChpaterId(string chapterId)
		{
			ChapterStory data = null;
			mChapterStory.TryGetValue(chapterId, out data);
			return data;
		}
	}
}