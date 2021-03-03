using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WaittingLogic : MonoBehaviour, IStackableUI
{
    [ResourcesReference(typeof(Texture2D))]
    public ResourcesReference[] TextureResources;

    public List<string> Models;

    public UILabel selfLabel;
    public UILabel enemyLabel;

    public UITexture selfTex;
    public UITexture enemyTex;

    public UILabel LoadLabel;
    public UILabel WaitLabel;

    private string[] LoadingStr = { ".  ", " . ", "  ." };

    private WaittingState curState;

    private enum WaittingState
    {
        Loading,
        Waitting,
    }

    //private bool _onRemove = false;
    private float _waitOutTimer = 0;
    private float _waitTimer = 0;

    private float _loadTimer = 0;
    private float _loadTimerFactor = 2f;

    public float BackgroundUIFadeTime
    {
        get { return 0.0f; }
    }

    public bool EnstackOnCreate
    {
        get { return false; }
    }

    public bool ShowUIBlocker
    {
        get { return false; }
    }

    public bool Visibility
    {
        get { return gameObject.activeSelf; }
    }

    public bool CanAutoBackstack()
    {
        return false;
    }

    public bool IsFullscreen()
    {
        return true;
    }

    public bool IsRenderingWorldWhileFullscreen()
    {
        return false;
    }

    public IEnumerator OnAddToStack()
    {
        curState = WaittingState.Loading;

        _waitOutTimer = 45f;

        string loadingText = EB.Localizer.GetString("ID_LOADING");
        for (int i = 0, len = LoadingStr.Length; i < len; ++i)
        {
            LoadingStr[i] = string.Format("{0} {1}", loadingText, LoadingStr[i]);
        }

        yield break;
    }

    public IEnumerator OnRemoveFromStack()
    {
        Destroy(gameObject);
        yield break;
    }

    public virtual void Show(bool isShowing)
    {
        gameObject.SetActive(isShowing);
    }

    public virtual void OnFocus()
    {

    }

    public virtual void OnBlur()
    {

    }

    void Update()
    {
        if (curState == WaittingState.Loading)
        {
            LoadLabel.text = LoadingStr[Mathf.FloorToInt(_loadTimer % 3)];
            _loadTimer += Time.smoothDeltaTime * _loadTimerFactor;
        }
        else if (curState == WaittingState.Waitting)
        {
            _waitTimer += Time.deltaTime;
            if (_waitTimer < _waitOutTimer)
            {
                WaitLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_WaittingLogic_2604"), (int)_waitTimer);
            }
            else
            {
                _waitTimer = 0;
                EB.Debug.LogError("WaittingLogic.Update: waitting timedout");
                SparxHub.Instance.Disconnect(true);
            }
        }
    }

    public void SetWaittingScreen(object payload)
    {
        if (payload == null)
        {
            EB.Debug.LogError("payload is null!!!");
            return;
        }

        Hashtable data = payload as Hashtable;
        Hashtable combat_data = data["combat"] as Hashtable;
        ArrayList challengerTeams = EB.Dot.Array("challengerTeams", combat_data, null);
        ArrayList defenderBatches = EB.Dot.Array("defenderBatches", combat_data, null);

        ArrayList challenger = EB.Dot.Array("team", challengerTeams[0], null);
        ArrayList defender = EB.Dot.Array("team", defenderBatches[0], null);

        SetPlayerInfo(challenger[0], selfTex, selfLabel);
        SetPlayerInfo(defender[0], enemyTex, enemyLabel);

        curState = WaittingState.Loading;
        LoadLabel.gameObject.SetActive(curState == WaittingState.Loading);
        WaitLabel.gameObject.SetActive(curState == WaittingState.Waitting);
    }

    private void SetPlayerInfo(object data, UITexture texture, UILabel label)
    {
        string name = EB.Dot.String("name", data, string.Empty);
        string model = EB.Dot.String("model", data, string.Empty);
        label.text = name;
        texture.mainTexture = TextureResources[Models.IndexOf(model)].Value as Texture2D;
    }

    public void OnCombatViewLoaded()
    {
        curState = WaittingState.Waitting;
        LoadLabel.gameObject.SetActive(curState == WaittingState.Loading);
        WaitLabel.gameObject.SetActive(curState == WaittingState.Waitting);
        _waitTimer = 0;
    }

    public IEnumerator OnPrepareAddToStack()
    {
        yield break;
    }

    public void ClearData()
    {
    }
}
