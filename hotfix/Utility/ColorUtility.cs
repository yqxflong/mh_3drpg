using System.Collections.Generic;
using UnityEngine;

namespace LT.Hotfix.Utility {
    public static class ColorUtility {
        //白色
        public static readonly string WhiteColorHexadecimal = "ffffff";
        public static readonly Color WhiteColor = Color.white;

        //绿色
        public static readonly string GreenColorHexadecimal = "42fe79";         
        public static readonly Color GreenColor = new Color(66 / 255f, 254 / 255f, 121 / 255f);

        //蓝色
        public static readonly string BlueColorHexadecimal = "33b2ff";          
        public static readonly Color BlueColor = new Color(51 / 255f, 178 / 255f, 255 / 255f);

        //紫色
        public static readonly string PurpleColorHexadecimal = "cc66ff";        
        public static readonly Color PurpleColor = new Color(204 / 255f, 102 / 255f, 255 / 255f);

        //黄色
        public static readonly string YellowColorHexadecimal = "fff348";        
        public static readonly Color YellowColor = new Color(255 / 255f, 243 / 255f, 72 / 255f);

        //红色
        public static readonly string RedColorHexadecimal = "ff6699";           
        public static readonly Color RedColor = new Color(255 / 255f, 102 / 255f, 153 / 255f);

        //淡红色
        public static readonly string LightRedColorHexadecimal = "fe3e3e";
        public static readonly Color LightRedColor = new Color(254 / 255f, 62 / 255f, 62 / 255f);

        //褐色
        public static readonly string BrownColorHexadecimal = "230c00";     
        public static readonly Color BrownColor = new Color(35 / 255f, 12 / 255f, 0 / 255f);

        //彩色
        public static readonly string ColorfulColorHexadecimal = "ff1c54";           
        public static readonly Color ColorfulColor = new Color(255 / 255f, 28 / 255f, 84 / 255f);

        //灰色
        public static readonly string GrayColorHexadecimal = "9f9f9f";          
        public static readonly Color GrayColor = new Color(159 / 255f, 159 / 255f, 159 / 255f);

        //渐变色
        public static readonly string GradientTopColorHexadecimal = "ffff58";           
        public static readonly Color GradientTopColor = new Color(255 / 255f, 255 / 255f, 88 / 255f);

        public static readonly string GradientBottomColorHexadecimal = "ff40ff";
        public static readonly Color GradientBottomColor = new Color(255 / 255f, 64 / 255f, 255 / 255f);

        //品质框-白色
        public static readonly string FrameWhiteColorHexadecimal = "3f545d";    
        public static readonly Color FrameWhiteColor = new Color(63 / 255f, 84 / 255f, 93 / 255f);

        //品质框-绿色
        public static readonly string FrameGreenColorHexadecimal = "135c3f";    
        public static readonly Color FrameGreenColor = new Color(19 / 255f, 92 / 255f, 63 / 255f);

        //品质框-蓝色
        public static readonly string FrameBlueColorHexadecimal = "1d1b90";          
        public static readonly Color FrameBlueColor = new Color(29 / 255f, 27 / 255f, 144 / 255f);

        //品质框-紫色
        public static readonly string FramePurpleColorHexadecimal = "6b008c";   
        public static readonly Color FramePurpleColor = new Color(107 / 255f, 0 / 255f, 140 / 255f);

        //品质框-黄色
        public static readonly string FrameYellowColorHexadecimal = "7a3a14";     
        public static readonly Color FrameYellowColor = new Color(122 / 255f, 58 / 255f, 20 / 255f);

        //品质框-红色
        public static readonly string FrameRedColorHexadecimal = "671119";          
        public static readonly Color FrameRedColor = new Color(103 / 255f, 17 / 255f, 25 / 255f);

        public static readonly string ColorStringFormat = "[{0}]{1}[-]";

        public static string QualityToGradientTopColorHexadecimal(int quality) {
            switch (quality) {
                case 2:
                    return GreenColorHexadecimal;
                case 3:
                    return BlueColorHexadecimal;
                case 4:
                    return PurpleColorHexadecimal;
                case 5:
                    return YellowColorHexadecimal;
                case 6:
                    return RedColorHexadecimal;
                case 7:
                    return GradientTopColorHexadecimal;
                default:
                    return WhiteColorHexadecimal;
            }
        }

        public static Color QualityToGradientTopColor(int quality) {
            switch (quality) {
                case 2:
                    return GreenColor;
                case 3:
                    return BlueColor;
                case 4:
                    return PurpleColor;
                case 5:
                    return YellowColor;
                case 6:
                    return RedColor;
                case 7:
                    return GradientTopColor;
                default:
                    return WhiteColor;
            }
        }

        public static string QualityToGradientBottomColorHexadecimal(int quality) {
            switch (quality) {
                case 2:
                    return GreenColorHexadecimal;
                case 3:
                    return BlueColorHexadecimal;
                case 4:
                    return PurpleColorHexadecimal;
                case 5:
                    return YellowColorHexadecimal;
                case 6:
                    return RedColorHexadecimal;
                case 7:
                    return GradientBottomColorHexadecimal;
                default:
                    return WhiteColorHexadecimal;
            }
        }

        public static Color QualityToGradientBottomColor(int quality) {
            switch (quality) {
                case 2:
                    return GreenColor;
                case 3:
                    return BlueColor;
                case 4:
                    return PurpleColor;
                case 5:
                    return YellowColor;
                case 6:
                    return RedColor;
                case 7:
                    return GradientBottomColor;
                default:
                    return WhiteColor; 
            }
        }

        public static Color QualityToColor(int quality) {
            switch (quality) {
                case 2:
                    return GreenColor;
                case 3:
                    return BlueColor;
                case 4:
                    return PurpleColor;
                case 5:
                    return YellowColor;
                case 6:
                    return RedColor;
                case 7:
                    return ColorfulColor;
                default:
                    return WhiteColor;
            }
        }

        public static Color QualityToFrameColor(int quality) {
            switch (quality) {
                case 1:
                    return FrameWhiteColor;
                case 2:
                    return FrameGreenColor;
                case 3:
                    return FrameBlueColor;
                case 4:
                    return FramePurpleColor;
                case 5:
                    return FrameYellowColor;
                case 6:
                    return FrameRedColor;
                case 7:
                    return WhiteColor;
                default:
                    return FrameWhiteColor;
            }
        }

        private static Dictionary<int, string> _decimalDict = new Dictionary<int, string> { 
            { 0, "0"},
            { 1, "0.0"},
            { 2, "0.00"},
            { 3, "0.000"} 
        };

        public static string FormatLeftSideColor(int curCount, int needCount) {
            if (curCount >= needCount) {
                return string.Format("[{0}]{1}[-]/{2}", GreenColorHexadecimal, curCount, needCount);
            } else {
                return string.Format("[{0}]{1}[-]/{2}", RedColorHexadecimal, curCount, needCount);
            }
        }

        public static string FormatColor(int curCount, int needCount) {
            if (curCount >= needCount) {
                return string.Format("[{0}]{1}/{2}", GreenColorHexadecimal, curCount, needCount);
            } else {
                return string.Format("[{0}]{1}/{2}", RedColorHexadecimal, curCount, needCount);
            }
        }

        public static string FormatColorIncrement(float before, float after, int index) {
            var value = after - before;
            if (value >= 0) {
                return string.Format("[{0}]+{1}", GreenColorHexadecimal, value.ToString(_decimalDict[index]));
            } else {
                return string.Format("[{0}]{1}", RedColorHexadecimal, value.ToString(_decimalDict[index]));
            }
        }

        public static string FormatColorPercentIncrement(float before, float after,int Decimal)
        {
            var value = after - before;

            if (value >= 0)
            {
                return string.Format("[{0}]+{1}%[-]", GreenColorHexadecimal, value.ToString(_decimalDict[Decimal]));
            }
            else
            {
                return string.Format("[{0}]{1}%[-]", RedColorHexadecimal, value.ToString(_decimalDict[Decimal]));
            }
        }


        public static string FormatColorByGreen(float curCount, float needCount, string sign,int Decimal) {
            if (curCount >= needCount) {
                return string.Format("[{0}]{1}{2}[-]", GreenColorHexadecimal, curCount.ToString(_decimalDict[Decimal]), sign);
            } else {
                return string.Format("[{0}]{1}{2}[-]", WhiteColorHexadecimal, curCount.ToString(_decimalDict[Decimal]), sign);
            }
        }

        public static string FormatColorFullLevel(int curCount, int needCount) {
            if (curCount >= needCount) {
                return string.Format("[{0}]{1}/{2}", GreenColorHexadecimal, curCount, needCount);
            } else {
                return string.Format("[{0}]{1}/{2}", WhiteColorHexadecimal, curCount, needCount);
            }
        }


        public static string GetFormatColorStr(string colorStr, string text) {
            return string.Format(ColorStringFormat, colorStr, text);
        }

        static public string FormatResidueStr(int residue, int total) {
            if (residue <= 0)
                return string.Format("[{0}]{1}/{2}[-]", RedColorHexadecimal, residue, total);
            else
                return string.Format("[{0}]{1}/{2}[-]", GreenColorHexadecimal, residue, total);
        }

        public static string FormatEnoughStr(int curCount, int needCount) {
            if (curCount >= needCount) {
                return string.Format("[{0}]{1}/{2}[-]", GreenColorHexadecimal, curCount, needCount);
            } else {
                return string.Format("[{0}]{1}/{2}[-]", RedColorHexadecimal, curCount, needCount);
            }
        }

        public static string TaskFormatEnoughStr(int curCount, int needCount) {
            if (curCount >= needCount) {
                return string.Format("[{0}]{1}/{2}[-]", GreenColorHexadecimal, curCount, needCount);
            } else {
                return string.Format("[{0}]{1}/{2}[-]", WhiteColorHexadecimal, curCount, needCount);
            }
        }
    }
}