//RichTextParser
//富文本解析器
//Johny

using System.Collections.Generic;

public class RichTextParser
{
    public enum DataType
    {
        tError = -1,
        tString,
        tBreak,
        tAtlas,
        tTexture,
    }

    public struct Data
    {
        public DataType Type;

        #region Only Error
        public string ErrorMsg;
        #endregion

        #region Only String
        public string Text;
        #endregion

        #region Only Atlas
        public string AtlasName;
        public string SpriteNameInAtlas;
        #endregion

        #region  Only Texture
        public string TextureName;
        #endregion
    }

    private static readonly Dictionary<string, DataType> DIC_FEATRURE = new Dictionary<string, DataType>
    {
        {"br", DataType.tBreak},
        {"atlas", DataType.tAtlas},
        {"tex", DataType.tTexture}
    };

    ///<xxxx>尖括号以内的部分
    private static Data ParseFeatureString(string feature)
    {
        var ll = feature.Split(' ');
        if(ll.Length > 0)
        {
            if(DIC_FEATRURE.TryGetValue(ll[0], out DataType dt))
            {
                switch(dt)
                {
                    case DataType.tBreak:
                        return new Data{Type = DataType.tBreak};
                    case DataType.tAtlas:
                        if(ll.Length == 3)
                        {
                            var arr_name = ll[1].Trim().Split('=');
                            var arr_sprite = ll[2].Trim().Split('=');
                            if(arr_name.Length == 2 && arr_sprite.Length == 2)
                            {
                                return new Data{Type = DataType.tAtlas, AtlasName = arr_name[1], SpriteNameInAtlas = arr_sprite[1]};
                            }
                        }
                        break;
                    case DataType.tTexture:
                        if(ll.Length == 2)
                        {
                            var arr_name = ll[1].Trim().Split('=');
                            if(arr_name.Length == 2)
                            {
                                return new Data{Type = DataType.tTexture, TextureName = arr_name[1]};
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        return new Data{Type = DataType.tError, ErrorMsg = $"feature: <{feature}>, 格式错误?!"};
    }

    private static void ParseRichText_Recursive(string text, List<Data> ret)
    {
        int beginIdx = text.IndexOf('<');
        if(beginIdx != -1)
        {
            if (beginIdx > 0)
            {//开始的尖括号不是第一个字符,将尖括号前面的string装入
                string before = text.Substring(0, beginIdx);
                ret.Add(new Data { Type = DataType.tString, Text = before });
            }

            int endIdx = text.IndexOf('>');
            if(endIdx - beginIdx == 1)
            {
                EB.Debug.LogError("未配置任何特性的尖括号!!");
            }
            else
            {//装入尖括号中的特性
                string feature = text.Substring(beginIdx + 1, endIdx - beginIdx - 1);
                ret.Add(ParseFeatureString(feature));
            }

            if(endIdx < text.Length - 1)
            {//结束的尖括号不是最后一个字符,继续递归
                string leftString = text.Substring(endIdx + 1);
                ParseRichText_Recursive(leftString, ret);
            }
        }
        else
        {//余下字符串没有特性标识,直接装入整个string
            ret.Add(new Data{Type = DataType.tString, Text = text});
        }
    }

    ///解析给定的富文本String
    ///return 连续按顺序的文本结构
    public static List<Data> ParseRichText(string text)
    {
        List<Data> ret = new List<Data>();
        ParseRichText_Recursive(text, ret);

        return ret;
    }

    #region Unit Test
    public const string TEST_STRING = "每次召唤，都会获得1积分<br>还有机会获得<atlas name=LTGeneral_Atlas sprite=Ty_Strive_Icon_UR>阿尔忒弥斯";
    #endregion
}