using UnityEngine;
using System.Collections;

public class GameInfoLogic : MonoBehaviour 
{
	private UILabel         _uiLabel;

	private struct TimerCoroutineInfo
	{
		public float time;
		public System.Action<float> updateCallback;
		public System.Action endCallback;
	}
	
	void Start()
	{
		_uiLabel = GameObject.Find ("GameInfoLabel").GetComponent<UILabel>();
        _uiLabel.text = "";	
	}
	
	public void OnGameInfoChange( string text, float timeDisplayed = 1.0f, float fadeTime = 1.0f)
	{
		StopCoroutine("StartTimer");

		_uiLabel.text = text;
		_uiLabel.alpha = 1.0f;

		TimerCoroutineInfo info = new TimerCoroutineInfo();
		info.time = timeDisplayed;
		info.updateCallback = null;
		info.endCallback = () =>
			{
				TimerCoroutineInfo endInfo = new TimerCoroutineInfo();
				endInfo .time = fadeTime;
				endInfo .updateCallback = timeRemaining =>
					{
						_uiLabel.alpha = timeRemaining / fadeTime;
					};
				endInfo .endCallback = 
					() =>
					{
						_uiLabel.text = "";
						_uiLabel.alpha = 0.0f;
					};

				StartCoroutine("StartTimer", endInfo);
			};

		StartCoroutine("StartTimer", info);
	}

	private IEnumerator StartTimer(TimerCoroutineInfo info)
	{
		float currentTime = info.time;
		while (currentTime > 0.0f)
		{
			yield return null;
			currentTime -= Time.deltaTime;

			if (info.updateCallback != null)
			{
				info.updateCallback(currentTime);
			}
		}

		if (info.endCallback != null)
		{
			info.endCallback();
		}
	}
}
