using UnityEngine;
using System.Collections;
using System.Collections.Generic;
    
namespace Hotfix_LT.UI
{
    public class LTSliderBtnList : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            if (mDMono.ObjectParamList != null && mDMono.ObjectParamList.Count > 0)
            {
                int count = mDMono.ObjectParamList.Count;
                TweenList = new List<TweenPosition>();
                for (int i = 0; i < count; i++)
                {
                    TweenList.Add((mDMono.ObjectParamList[i] as GameObject).GetComponent<TweenPosition>());
                }
            }
            if (mDMono.BoolParamList != null && mDMono.BoolParamList.Count > 0)
            {
                ChangeY = mDMono.BoolParamList[0];
            }
            if (mDMono.FloatParamList != null && mDMono.FloatParamList.Count > 0)
            {
                int count = mDMono.FloatParamList.Count;
                if (count > 0)
                {
                    CellYSpace = mDMono.FloatParamList[0];
                }                
                if(count > 1)
                {
                    LastYSpace = mDMono.FloatParamList[1];
                }
            }
            for (var i = 0; i < TweenList.Count; i++)
            {
                enableList.Add(true);
            }

        }


    
        public List<TweenPosition> TweenList;
        public List<bool> enableList = new List<bool>();
    
    
        private int index = 0;
        public float CellYSpace = 0f;
        public float LastYSpace = 0f;
    	public bool ChangeY=true;
    	public bool PlayOver { private set; get; }
        private System.Action callback;
    
        public void OnStateChange(bool state, System.Action callback = null)
        {
    		//curState = !curState;
    		PlayOver = false;
    		this.callback = callback;
            if (state)
            {
                index = 0;
                float startY = 0;
                for (int i = 0; i < TweenList.Count; i++)
                {
                    if(enableList[i])
                    {
                        TweenList[i].SetOnFinished(Empty);
                        TweenList[i].gameObject.SetActive(false);
    					if (ChangeY)
    					{
    						if (i == 0)
    						{
    							TweenList[i].from.y = startY;
    							TweenList[i].to.y = startY;
    						}
    						else if (i == TweenList.Count - 1)
    						{
    							TweenList[i].from.y = startY;
    							startY += CellYSpace + LastYSpace;
    							TweenList[i].to.y = startY;
    						}
    						else
    						{
    							TweenList[i].from.y = startY;
    							startY += CellYSpace;
    							TweenList[i].to.y = startY;
    						}
    					}
                    }
                }
                /*for(int i=0;i<TweenList.Count;i++)
                {
                    if(enableList[i])
                    {
    
                    }
                }*/
                Open();
            }
            else
            {
                index = TweenList.Count - 1;
                for (int i = 0; i < TweenList.Count; i++)
                {
                    if(enableList[i])
                    {
                        TweenList[i].SetOnFinished(Empty);
                        //TweenList[i].gameObject.SetActive(true);
                    }
                }
                Close();
            }
        }
    
        private void Open()
        {
            while(!enableList[index])
            {
                index++; 
                if (index >= TweenList.Count)
                {
    				if (callback != null)
    				{
    					callback();
    					callback = null;
    				}
    				PlayOver = true;
    				return;
                }
            }
            TweenPosition curTween = TweenList[index];
            curTween.gameObject.SetActive(true);
            curTween.Play(true);
            index++;
            if (index < TweenList.Count)
            {
                curTween.SetOnFinished(Open);
            }
            else
            {
                if (callback != null)
                {
                    callback();
                    callback = null;
                }
    			curTween.SetOnFinished(delegate() {
    				PlayOver = true;
    			});			
    		}
        }
    
        private void Close()
        {
            while(!enableList[index])
            {
                if (index < TweenList.Count - 1)
                {
                    TweenList[index + 1].gameObject.SetActive(false);
                }
                index--;
    
                if (index < 0)
                {
    				PlayOver = true;
    				return;
                }
            }
    
            if (index < TweenList.Count - 1)
            {
                TweenList[index + 1].gameObject.SetActive(false);
            }
    
            TweenPosition curTween = TweenList[index];
            curTween.Play(false);
            index--;
            if (index >= 0)
            {
                curTween.SetOnFinished(Close);
            }
            else
            {
    			PlayOver = true;
    			curTween.gameObject.SetActive(false);
    		}
        }
    
        private void Empty()
        {
    
        }
    }
}
