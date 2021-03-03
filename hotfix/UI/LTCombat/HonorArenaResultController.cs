using UnityEngine;
using System.Collections;
using ILRuntime.Runtime;

namespace Hotfix_LT.UI
{
    public class HonorArenaResultController : DynamicMonoHotfix
    {
        public System.Action onShownAnimCompleted;

        private bool _isWon;
        private int _rounds = 0;
        private const int _MAX_PARTNER_NUM = 6;
        private GameObject _victory;
        private GameObject _defeat;
        private GameObject _panel1;
        private GameObject _panel2;
        private UISprite _avatarLeft;
        private UISprite _avatarRight;
        private UISprite _frameLeft;
        private UISprite _frameRight;
        private UILabel _nameLeft;
        private UILabel _nameRight;
        private UILabel _score;
        private UILabel _pointsLeft;
        private UILabel _pointsRight;
        private HonorArenaResultInfo _vs1Info;
        private HonorArenaResultInfo _vs2Info;
        private HonorArenaResultInfo _vs3Info;
        private HonorArenaResultInfo[] _infos;
        private Color _winScoreColor = new Color32(65, 254, 119, 255);
        private Color _loseScoreColor = new Color32(252, 80, 80, 255);

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            _victory = t.parent.FindEx("VictoryObj").gameObject;
            _defeat = t.parent.FindEx("DefeatObj").gameObject;
            _panel1 = t.FindEx("Container/Panel_1").gameObject;
            _panel2 = t.FindEx("Container/Panel_2").gameObject;
            _avatarLeft = _panel1.GetComponent<UISprite>("Avatar_Left");
            _avatarRight = _panel1.GetComponent<UISprite>("Avatar_Right");
            _frameLeft = _panel1.GetComponent<UISprite>("Avatar_Left/Frame");
            _frameRight = _panel1.GetComponent<UISprite>("Avatar_Right/Frame");
            _nameLeft = _panel1.GetComponent<UILabel>("Avatar_Left/Name");
            _nameRight = _panel1.GetComponent<UILabel>("Avatar_Right/Name");
            _score = _panel1.GetComponent<UILabel>("Score");               //【格式】2:1
            _pointsLeft = _panel1.GetComponent<UILabel>("Points/Left");    //【格式】积分: 1900
            _pointsRight = _panel1.GetComponent<UILabel>("Points/Right");  //【格式】2850
            _vs1Info = _panel2.GetMonoILRComponent<HonorArenaResultInfo>("VS1");
            _vs2Info = _panel2.GetMonoILRComponent<HonorArenaResultInfo>("VS2");
            _vs3Info = _panel2.GetMonoILRComponent<HonorArenaResultInfo>("VS3");
            _infos = new HonorArenaResultInfo[] { _vs1Info, _vs2Info, _vs3Info };

            SetData();
        }

        public override void OnEnable()
        {
            Reset();
    		StartCoroutine(ShowFxAnimation());
    	}

        public override void OnDisable()
        {
            StopAllCoroutines();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            DataLookupsCache.Instance.CacheData("honorBattleResult", null);
        }

        private void Reset()
        {
            _victory.SetActive(false);
    		_defeat.SetActive(false);
            _panel1.SetActive(false);
            _panel2.SetActive(false);
        }

        private void SetData()
        {
            ArrayList data;
            DataLookupsCache.Instance.SearchDataByID("honorBattleResult", out data);

            if (data != null)
            {
                object dataLeft = null;
                object dataRight = null;
                int playerWorldId = 0;
                int opponentWorldId = 0;

                for (var i = 0; i < data.Count; i++)
                {
                    var dt = data[i];
                    var characterId = Data.CharacterTemplateManager.Instance.TemplateidToCharacterid(EB.Dot.Integer("teamInfo.leader", dt, 0).ToString());
                    var heroInfo = Data.CharacterTemplateManager.Instance.GetHeroInfo(characterId, EB.Dot.Integer("teamInfo.skin", dt, 0));
                    var headFrame = Data.EconemyTemplateManager.Instance.GetHeadFrame(EB.Dot.String("teamInfo.headFrame", dt, "")).iconId;
                    var worldId = EB.Dot.Integer("teamInfo.worldId", dt, 0);
                    var name = EB.Dot.String("teamInfo.name", dt, "");

                    // 0代表玩家自己的队伍
                    if (EB.Dot.Integer("teamIndex", dt, 0) == 0)
                    {
                        if (heroInfo != null)
                        {
                            _avatarLeft.spriteName = heroInfo.icon;
                        }

                        _frameLeft.gameObject.SetActive(!string.IsNullOrEmpty(headFrame));
                        _frameLeft.spriteName = headFrame;
                        _nameLeft.text = string.Format("{0}【{1}{2}】", name, worldId, EB.Localizer.GetString("ID_LOGIN_SERVER_NAME"));
                        _pointsLeft.text = string.Format("{0}: {1}", EB.Localizer.GetString("ID_ID_ACTIVITY_UR_SCORE"), EB.Dot.Integer("teamInfo.scorePre", dt, 0));
                        _pointsRight.text = EB.Dot.Integer("teamInfo.scorePost", dt, 0).ToString();
                        dataLeft = dt;
                        playerWorldId = worldId;
                    }
                    else
                    {
                        if (heroInfo != null)
                        {
                            _avatarRight.spriteName = heroInfo.icon;
                        }

                        _frameRight.gameObject.SetActive(!string.IsNullOrEmpty(headFrame));
                        _frameRight.spriteName = headFrame;
                        _nameRight.text = name;
                        dataRight = dt;
                        opponentWorldId = worldId;
                    }
                }

                _nameRight.text += string.Format("【{0}{1}】", (opponentWorldId < 1) ? playerWorldId : opponentWorldId, EB.Localizer.GetString("ID_LOGIN_SERVER_NAME"));

                var roundResult = EB.Dot.Array("roundResult", dataLeft, null);
                var winNum = 0;
                var roundInfoLeft = EB.Dot.Array("roundInfo", dataLeft, null);
                var roundInfoRight = EB.Dot.Array("roundInfo", dataRight, null);
                    
                if (roundResult == null)
                {
                    _isWon = false;
                    _rounds = 0;
                }
                else
                {
                    for (var i = 0; i < roundResult.Count; i++)
                    {
                        OtherPlayerPartnerData[] arrleft = (roundInfoLeft != null && i < roundInfoLeft.Count) ? GetPartnerDataArray(roundInfoLeft[i] as ArrayList) : null;
                        OtherPlayerPartnerData[] arrRight = (roundInfoRight != null && i < roundInfoRight.Count) ? GetPartnerDataArray(roundInfoRight[i] as ArrayList) : null;
                        var leftDatas = new OtherPlayerPartnerData[_MAX_PARTNER_NUM];
                        var rightDatas = new OtherPlayerPartnerData[_MAX_PARTNER_NUM];

                        if (arrleft != null)
                        {
                            for (var j = 0; j < arrleft.Length; j++)
                            {
                                leftDatas[j] = arrleft[j]; 
                            }
                        }

                        if (arrRight != null)
                        {
                            for (var j = 0; j < arrRight.Length; j++)
                            {
                                rightDatas[j] = arrRight[j];
                            }
                        }

                        if (roundResult[i].ToInt32() == 1)
                        {
                            winNum += 1;
                            _infos[i].SetData(true, false, leftDatas, rightDatas);
                        }
                        else
                        {
                            _infos[i].SetData(false, true, leftDatas, rightDatas);
                        }
                    }

                    _score.text = string.Format("{0}:{1}", winNum, roundResult.Count - winNum);
                    _isWon = winNum > (roundResult.Count - winNum);
                    _rounds = roundResult.Count;
                    _pointsRight.color = _isWon ? _winScoreColor : _loseScoreColor;
                }
            }
        }

        private OtherPlayerPartnerData[] GetPartnerDataArray(ArrayList arrayList)
        {
            var result = new OtherPlayerPartnerData[arrayList.Count];

            for (var i = 0; i < arrayList.Count; i++)
            {
                result[i] = GetPartnerData(arrayList[i]);
            }

            return result;
        }

        private OtherPlayerPartnerData GetPartnerData(object data)
        {
            if (data == null)
            {
                return null;
            }

            var heroInfo = Data.CharacterTemplateManager.Instance.GetHeroInfo(EB.Dot.Integer("charId", data, 0), EB.Dot.Integer("skin", data, 0));
            var partnerData = new OtherPlayerPartnerData();
            partnerData.Name = heroInfo != null ? heroInfo.name : "";
            partnerData.Attr = heroInfo != null ? heroInfo.char_type : Data.eRoleAttr.None;
            partnerData.Icon = heroInfo != null ? heroInfo.icon : "";
            partnerData.QualityLevel = heroInfo != null ? heroInfo.role_grade : 0;
            partnerData.Level = EB.Dot.Integer("level", data, 0);
            partnerData.Star = EB.Dot.Integer("star", data, 0);
            partnerData.UpGradeId = EB.Dot.Integer("upgradeId", data, 0);
            partnerData.awakenLevel = EB.Dot.Integer("awakenLevel", data, 0);
            return partnerData;
        }

        private IEnumerator ShowFxAnimation()
        {
            yield return new WaitForEndOfFrame();
    		_defeat.CustomSetActive(!_isWon);
    		_victory.CustomSetActive(_isWon);
            yield return new WaitForSeconds(1f);
            OnFxAnimationFinished();
        }

        private void OnFxAnimationFinished()
        {
            _panel1.SetActive(true);
            onShownAnimCompleted?.Invoke();
        }

        public void ShowTeamInfo(System.Action callback)
        {
            _panel1.FindEx("Points").SetActive(false);
            _victory.SetActive(false);
            _defeat.SetActive(false);

            var tp = _panel1.GetComponentEx<TweenPosition>();
            tp.onFinished.Add(new EventDelegate(() => 
            { 
                StartCoroutine(ShowVsInfo());
                callback?.Invoke();
            }));
            tp.enabled = true;
        }

        WaitForSeconds _wait02 = new WaitForSeconds(0.2f);

        private IEnumerator ShowVsInfo()
        {
            _panel2.SetActive(true);

            for (var i = 0; i < _rounds; i++)
            {
                _infos[i].mDMono.gameObject.SetActive(true);
                yield return _wait02;
            }
        }
    }
}
