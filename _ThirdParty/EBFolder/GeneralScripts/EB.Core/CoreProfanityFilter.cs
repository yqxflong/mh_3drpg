using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EB
{	
    public static class ProfanityFilter 
    {
        private static List<string> _naughtyList = new List<string>();
        
        public static void Init( string words ) 
        {
            foreach( var word in words.Split(new char[]{'\n','\r'}, System.StringSplitOptions.RemoveEmptyEntries) )
            {
                _naughtyList.Add(word);	
            }
        }
        
        public static bool Test( string text )
        {
            foreach (string word in _naughtyList)
            {
                string wordNoSpaces = word.Replace(" ", "");
                if (text.IndexOf(word, System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                     text.IndexOf(wordNoSpaces, System.StringComparison.OrdinalIgnoreCase) >= 0
                    )
                {
                    return false;
                }
            }
            return true;
        }
        
        public static string Filter( string text )
        {
            string censoredText = text;

            foreach (string word in _naughtyList)
            {
                string replacement = new string('*', word.Length);
                int index = 0;

                // TODO: handle the space's... ie F U C K;

                while ( (index = censoredText.IndexOf(word, System.StringComparison.OrdinalIgnoreCase)) >= 0 )
                {
                    censoredText = censoredText.Substring(0, index) + replacement + censoredText.Substring(index + replacement.Length);
                }
            }

            return FilterContinueFigure(censoredText,5);
        }


		public static string FilterContinueFigure(string text, int continueNumber)
		{
			string[] figureChs = new string[] {
				EB.Localizer.GetString("ID_0"),
				EB.Localizer.GetString("ID_1"),
				EB.Localizer.GetString("ID_2"),
				EB.Localizer.GetString("ID_3"),
				EB.Localizer.GetString("ID_4"),
				EB.Localizer.GetString("ID_5"),
				EB.Localizer.GetString("ID_6"),
				EB.Localizer.GetString("ID_7"),
				EB.Localizer.GetString("ID_8"),
				EB.Localizer.GetString("ID_9"),
				"0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

			int arrayIndex = 0;
			int figureCounter = 0;
			bool isMeetCondition = false;
			while (arrayIndex < text.Length)
			{
				if (figureChs.Contains<string>(text[arrayIndex].ToString()))
				{
					figureCounter++;
					if (figureCounter >= continueNumber)
					{
						isMeetCondition = true;
						if (arrayIndex < text.Length)
							text = text.Substring(0, arrayIndex) + "*" + text.Substring(arrayIndex + 1);
						else
							text = text.Substring(0, arrayIndex) + "*";
					}
					else if (isMeetCondition)
					{
						isMeetCondition = false;
						figureCounter = 0;
					}
				}
				else
				{
					figureCounter = 0;
				}
				arrayIndex++;
			}

			return text;
		}
	}
}


