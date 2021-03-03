using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class StoneThrowerTemplate : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            TimerLabel = t.GetComponent<UILabel>("InfoBar/Label");

            HpGOs = new GameObject[3];
            HpGOs[0] = t.FindEx("InfoBar/HPGrid/Sprite").gameObject;
            HpGOs[1] = t.FindEx("InfoBar/HPGrid/Sprite (1)").gameObject;
            HpGOs[2] = t.FindEx("InfoBar/HPGrid/Sprite (2)").gameObject;

            ModelGO = t.FindEx("Model").gameObject;
            InfoBar = t.FindEx("InfoBar").gameObject;
            BrokenGO = t.FindEx("BrokenModel").gameObject;
            BrokenSpriteDelayShowTime = 0f;
            BrokenStayTime = 3f;

        }


    
    	public UILabel TimerLabel;
    	public GameObject[] HpGOs;
    	public GameObject ModelGO;
    	public GameObject InfoBar;
    	public GameObject BrokenGO;
    	public float BrokenSpriteDelayShowTime;
    	public float BrokenStayTime;
    	public StoneThrowerData Data { get; private set; }
    	private WaitForSeconds Wait1s = new WaitForSeconds(1f);
    	private Coroutine CountdownC;
    
    	public void Init(StoneThrowerData data)
    	{		
    		Data = data;
    		if (data.dir == eBattleDirection.UpDefend || data.dir == eBattleDirection.MiddleDefend || data.dir == eBattleDirection.DownDefend)
    		{
    			ModelGO.transform.localScale = new Vector3(-1, 1, 1);
    			BrokenGO.transform.localScale = new Vector3(-1, 1, 1);
    		}
    		else
    		{
    			ModelGO.transform.localScale = Vector3.one;
    			BrokenGO.transform.localScale = Vector3.one;
    		}
    		UpdateHP(data.hp);
    		if (CountdownC != null)
    			StopCoroutine(CountdownC);
    		CountdownC = StartCoroutine(Countdown());
    	}
    
    	public void UpdateHP(int hp)
    	{
    		int index = 0;
    		for(index = 0; index < hp;++index)
    		{
    			HpGOs[index].gameObject.CustomSetActive(true);
    		}
    		for (index = hp; index < HpGOs.Length; ++index)
    		{
    			HpGOs[index].gameObject.CustomSetActive(false);
    		}
    	}
    
    	IEnumerator Countdown()
    	{
    		while (true)
    		{
    			long leftTime = Data.endTime- EB.Time.Now;
    			if (leftTime > 0)
    			{
    				TimerLabel.text = leftTime.ToString();
    			}
    			yield return Wait1s;
    		}
    	}
    
    	public void SetDeath(bool isFire)
    	{
    		ModelGO.SetActive(false);
    		InfoBar.SetActive(false);
    		if (isFire)
    			Object.Destroy(mDMono.gameObject);
    		else
    			StartCoroutine(PlayBrokenSprite(isFire));
    	}
    
    	IEnumerator PlayBrokenSprite(bool isFire)
    	{
    		if (isFire)
    			yield return new WaitForSeconds(BrokenSpriteDelayShowTime);
    		BrokenGO.SetActive(true);
    		yield return new WaitForSeconds(BrokenStayTime);
    		UITweener uit= BrokenGO.GetComponent<UITweener>();
    		uit.ResetToBeginning();
    		uit.PlayForward();
    		uit.SetOnFinished(delegate ()
    		{
    			Object.Destroy(mDMono.gameObject);
    		});
    	}
    }
}
