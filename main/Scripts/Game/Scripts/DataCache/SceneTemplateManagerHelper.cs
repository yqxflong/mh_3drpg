//SceneTemplateManagerHelper
//帮助热更中同名Class减少跨域
//Johny
using System.Collections.Generic;
using Unity.Standard.ScriptsWarp;
using UnityEngine;

namespace Main.DataCache
{
    public static class SceneTemplateManagerHelper
    {
        public static Vector3 LostChallengeChapterRole_ParseRotation(int id, string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                string[] rotationSplit = str.Split(',');
                if (rotationSplit.Length >= 3)
                {
                    return new Vector3(float.Parse(rotationSplit[0]), float.Parse(rotationSplit[1]), float.Parse(rotationSplit[2]));
                }
                else
                {
                    EB.Debug.LogError("Error Role Config Rotation need Vector3, role id = {0}", id);
                }
            }

            return Vector3.zero;
        }

        public static Vector2 LostChallengeChapterRole_ParseSpan(int id, string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                string[] spanSplit = str.Split(',');
                if (spanSplit.Length >= 2)
                {
                    return new Vector2(float.Parse(spanSplit[0]), float.Parse(spanSplit[1]));
                }
                else
                {
                    EB.Debug.LogError("Error Role Config Span need Vector2, role id = {0}", id);
                }
            }

            return Vector2.one;
        }

        public static Vector2 LostChallengeChapterRole_ParseOffset(int id, string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                string[] offsetSplit = str.Split(',');
                if (offsetSplit.Length >= 2)
                {
                    return new Vector2(float.Parse(offsetSplit[0]), float.Parse(offsetSplit[1]));
                }
                else
                {
                    EB.Debug.LogError("Error Role Config Offset need Vector2, role id = {0}", id);
                }
            }

            return Vector2.zero;
        }

        public static string[] LostChallengeChapterRole_ParseGuide(int id, string str)
        {
            string[] ret;
            if (!string.IsNullOrEmpty(str))
            {
				List<StringView> views;
				using (ZString.Block())
				{
					ZString strID = ZString.Format("ID_scenes_lost_challenge_chapter_role_{0}_guide", id);
					string tmp = EB.Localizer.GetTableString(strID, str);
					StringView @string = new StringView(tmp);
					views = @string.Split2List('|');
				}
				ret = new string[views.Count];
				for (int i = 0; i < views.Count; i++) { ret[i] = views[i].ToString(); }

                if (ret.Length < 2)
                {
                    EB.Debug.LogError("Error Role Config Guide need two string, role id = {0}", id);
                }
                else
                {
                    return ret;
                }
            }

            return new string[2];
        }

        public static void LostChallengeChapterRole_ParseModel(string str, out string model, out float scale)
        {
            str = str ?? "";

			StringView view = new StringView(str);
			List<StringView> split = view.Split2List('*');
            model = split.Count > 0 ? split[0].ToString() : string.Empty;
            if (split.Count > 1)
            {
                float.TryParse(split[1].ToString(), out scale);
            }
            else
            {
                scale = 1;
            }
        }
    }
}
