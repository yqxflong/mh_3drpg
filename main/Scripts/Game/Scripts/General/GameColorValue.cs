using UnityEngine;
using System.Collections;

public static class GameColorValue
{
	//游戏字体通用9种颜色	
	public static readonly string Color_Str_Format="[{0}]{1}[-]";

	public static readonly string Write_Str = "ffffff";			//白色
	// public static readonly Color Write_Color = new Color(255 / 255.0f, 255 / 255.0f, 255 / 255.0f);

	// public static readonly string Yellow_Light_Str = "FFF4B6";  //淡黄色
	public static readonly Color Yellow_Light_Color = new Color(255 / 255.0f, 244 / 255.0f, 182 / 255.0f);

	// public static readonly string Yellow_Str = "FFD200FF";        //黄色
	// public static readonly Color Yellow_Color = new Color(255 / 255.0f, 210 / 255.0f, 0 / 255.0f);

	// public static readonly string Gray_Str = "9f9f9f";          //灰色
	public static readonly Color Gray_Color = new Color(159 / 255.0f, 159 / 255.0f, 159 / 255.0f);
    
    //LT游戏字体通用颜色
    public static readonly string Green_Str_LT = "42fe79";         //绿色
    // public static readonly Color Green_Color_LT = new Color(66 / 255.0f, 254 / 255.0f, 121 / 255.0f);

    // public static readonly string Blue_Str_LT = "33b2ff";          //蓝色
    // public static readonly Color Blue_Color_LT = new Color(51 / 255.0f, 178 / 255.0f, 255 / 255.0f);

    // public static readonly string Violet_Str_LT = "cc66ff";        //紫色
    // public static readonly Color Violet_Color_LT = new Color(204 / 255.0f, 102 / 255.0f, 255 / 255.0f);

    // public static readonly string Yellow_Str_LT = "fff348";        //黄色
    // public static readonly Color Yellow_Color_LT = new Color(255 / 255.0f, 243 / 255.0f, 72 / 255.0f);

    public static readonly string Red_Str_LT = "ff6699";           //红色
    // public static readonly Color Red_Color_LT = new Color(255 / 255.0f, 102 / 255.0f, 153 / 255.0f);

    // public static readonly string Brownness_Str_LT = "230c00";     //褐色
    // public static readonly Color Brownness_Color_LT = new Color(35 / 255.0f, 12 / 255.0f, 0 / 255.0f);

    // public static readonly string Colorful_Str_LT = "ff1c54";           //彩色
    // public static readonly Color Colorful_Color_LT = new Color(255 / 255.0f, 28 / 255.0f, 84 / 255.0f);

    //------------------------------------------------------------------
    //public static readonly string        Write_Str = "ffffff";         //白色
    //public static readonly Color         Write_Color = new Color(255 / 255.0f, 255 / 255.0f, 255 / 255.0f);

    //public static readonly string        Yellow_Light_Str = "FFF4B6";  //淡黄色
    //public static readonly Color         Yellow_Light_Color = new Color(255 / 255.0f, 244 / 255.0f, 182 / 255.0f);

    //public static readonly string        Yellow_Str = "FFD200FF";        //黄色
    //public static readonly Color         Yellow_Color = new Color(255 / 255.0f, 210 / 255.0f, 0 / 255.0f);

    //public static readonly string        Gray_Str = "9f9f9f";          //灰色
    //public static readonly Color         Gray_Color = new Color(159 / 255.0f, 159 / 255.0f, 159 / 255.0f);
    //-----------------------------------------------------------------                                    
    //LT游戏字体通用颜色
    // public static readonly string        TopWrite_Str = "ffffff";         //白色
    // public static readonly Color         TopWrite_Color = new Color(255 / 255.0f, 255 / 255.0f, 255 / 255.0f);

    // public static readonly string        TopGreen_Str_LT = "42fe79";         //绿色
    // public static readonly Color         TopGreen_Color_LT = new Color(66 / 255.0f, 254 / 255.0f, 121 / 255.0f);
                                        
    // public static readonly string        TopBlue_Str_LT = "33b2ff";          //蓝色
    // public static readonly Color         TopBlue_Color_LT = new Color(51 / 255.0f, 178 / 255.0f, 255 / 255.0f);
                                         
    // public static readonly string        TopViolet_Str_LT = "cc66ff";        //紫色
    // public static readonly Color         TopViolet_Color_LT = new Color(204 / 255.0f, 102 / 255.0f, 255 / 255.0f);
                                         
    // public static readonly string        TopYellow_Str_LT = "fff348";        //黄色
    // public static readonly Color         TopYellow_Color_LT = new Color(255 / 255.0f, 243 / 255.0f, 72 / 255.0f);
                                         
    // public static readonly string        TopRed_Str_LT = "ff6699";           //红色
    // public static readonly Color         TopRed_Color_LT = new Color(255 / 255.0f, 102 / 255.0f, 153 / 255.0f);
                                         
    // public static readonly string        TopBrownness_Str_LT = "230c00";     //褐色
    // public static readonly Color         TopBrownness_Color_LT = new Color(35 / 255.0f, 12 / 255.0f, 0 / 255.0f);
                                         
    // public static readonly string        TopColorful_Str_LT = "ffff58";           //彩色
    // public static readonly Color         TopColorful_Color_LT = new Color(255 / 255.0f, 255 / 255.0f, 88 / 255.0f);


    // public static readonly string        BottomWrite_Str = "ffffff";         //白色
    // public static readonly Color         BottomWrite_Color = new Color(255 / 255.0f, 255 / 255.0f, 255 / 255.0f);

    // public static readonly string        BottomGreen_Str_LT = "42fe79";         //绿色
    // public static readonly Color         BottomGreen_Color_LT = new Color(66 / 255.0f, 254 / 255.0f, 121 / 255.0f);
                                         
    // public static readonly string        BottomBlue_Str_LT = "33b2ff";          //蓝色
    // public static readonly Color         BottomBlue_Color_LT = new Color(51 / 255.0f, 178 / 255.0f, 255 / 255.0f);
                                         
    // public static readonly string        BottomViolet_Str_LT = "cc66ff";        //紫色
    // public static readonly Color         BottomViolet_Color_LT = new Color(204 / 255.0f, 102 / 255.0f, 255 / 255.0f);
                                         
    // public static readonly string        BottomYellow_Str_LT = "fff348";        //黄色
    // public static readonly Color         BottomYellow_Color_LT = new Color(255 / 255.0f, 243 / 255.0f, 72 / 255.0f);
                                         
    // public static readonly string        BottomRed_Str_LT = "ff6699";           //红色
    // public static readonly Color         BottomRed_Color_LT = new Color(255 / 255.0f, 102 / 255.0f, 153 / 255.0f);
                                         
    // public static readonly string        BottomBrownness_Str_LT = "230c00";     //褐色
    // public static readonly Color         BottomBrownness_Color_LT = new Color(35 / 255.0f, 12 / 255.0f, 0 / 255.0f);
                                         
    // public static readonly string        BottomColorful_Str_LT = "ff40ff";           //彩色
    // public static readonly Color         BottomColorful_Color_LT = new Color(255 / 255.0f, 64 / 255.0f, 255 / 255.0f);
    //------------------------------------------------------------------


    //游戏品质框底的颜色
    // public static readonly string Write_Str_Frame = "3f545d";         //白色
    // public static readonly Color Write_Color_Frame = new Color(63 / 255.0f, 84 / 255.0f, 93 / 255.0f);

    // public static readonly string Green_Str_Frame = "135c3f";         //绿色
    // public static readonly Color Green_Color_Frame = new Color(19 / 255.0f, 92 / 255.0f, 63 / 255.0f);

    // public static readonly string Blue_Str_Frame = "1d1b90";          //蓝色
    // public static readonly Color Blue_Color_Frame = new Color(29 / 255.0f, 27 / 255.0f, 144 / 255.0f);

    // public static readonly string Violet_Str_Frame = "6b008c";        //紫色
    // public static readonly Color Violet_Color_Frame = new Color(107 / 255.0f, 0 / 255.0f, 140 / 255.0f);

    // public static readonly string Yellow_Str_Frame = "7a3a14";        //黄色
    // public static readonly Color Yellow_Color_Frame = new Color(122 / 255.0f, 58 / 255.0f, 20 / 255.0f);

    // public static readonly string Red_Str_Frame = "671119";           //红色
    // public static readonly Color Red_Color_Frame = new Color(103 / 255.0f, 17 / 255.0f, 25 / 255.0f);

    // public static readonly string Colorful_Str_Frame = "ffffff";           //彩色
    // public static readonly Color Colorful_Color_Frame = new Color(255 / 255.0f, 255 / 255.0f, 255 / 255.0f);

    public static string GetFormatColorStr(string colorStr, string text)
	{
		return string.Format(Color_Str_Format, colorStr, text);
	}

	// static public string FormatResidueStr(int residue, int total)
	// {
	// 	if (residue <= 0)
	// 		return string.Format("[{0}]{1}/{2}[-]", Red_Str_LT, residue, total);
	// 	else
	// 		return string.Format("[{0}]{1}/{2}[-]", Green_Str_LT, residue, total);
	// }

    // public static string FormatEnoughStr(int curCount, int needCount)
    // {
    //     if (curCount >= needCount)
    //     {
    //         return string.Format("[{0}]{1}/{2}[-]", Green_Str_LT, curCount, needCount);
    //     }
    //     else
    //     {
    //         return string.Format("[{0}]{1}/{2}[-]", Red_Str_LT, curCount, needCount);
    //     }
    // }

    // public static string TaskFormatEnoughStr(int curCount, int needCount)
    // {
    //     if (curCount >= needCount)
    //     {
    //         return string.Format("[{0}]{1}/{2}[-]", Green_Str_LT, curCount, needCount);
    //     }
    //     else
    //     {
    //         return string.Format("[{0}]{1}/{2}[-]", Write_Str, curCount, needCount);
    //     }
    // }
}
