using UnityEngine;
using System.Collections.Generic;
    
namespace Hotfix_LT.UI
{
    public class AutoCombatItem : DynamicMonoHotfix
    {
        private float _originY;
        private Vector3 _moveUpDistance;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            CommonLabel = t.FindEx("Icon/CommonLabel").gameObject;
            SpecialLabel = t.FindEx("Icon/SpecialLabel").gameObject;
            Icon = t.FindEx("Icon").gameObject;

            GraySpriteList = new List<UISprite>();
            GraySpriteList.Add(t.GetComponent<UISprite>("Icon/Frame"));

            BGSprite = t.GetComponent<UISprite>("Icon/Frame/Sprite");
            GrayBGSprite = t.GetComponent<UISprite>("Icon/Frame/Sprite (1)");
            Position = 0;

            t.GetComponent<UIButton>("Icon").onClick.Add(new EventDelegate(OnItemClick));
        }

        public override void Start()
        {
            _originY = mDMono.transform.localPosition.y;
            _moveUpDistance = new Vector3(0, 72, 0);
        }

        public GameObject CommonLabel,SpecialLabel,Icon;
    	public List<UISprite> GraySpriteList;
    	public UISprite BGSprite;
    	public UISprite GrayBGSprite;
    	public Hotfix_LT.Combat.CombatCharacterSyncData ItemCharSyncData;
    	bool _NormalSkill = false;
    	public bool IsNormalSkill { get { return _NormalSkill; } }
    	public int Position;
    
        public void OnItemClick() {
            if(IsNormalSkill)
    		{
                SetSpecialSkill();
            }
            else
    		{
                SetCommonSkill();
            }
        }
    
        public void SetSpecialSkill() {
            CommonLabel.SetActive(false);
            SpecialLabel.SetActive(true);
    		_NormalSkill = false;
            LTCombatHudController.Instance.AutoSkillString[Position] = 's';
            string sc = new string(LTCombatHudController.Instance.AutoSkillString);
            PlayerPrefs.SetString(LoginManager.Instance.LocalUserId.Value.ToString()+"AutoCombatSkill", sc);
        }
    
        public void SetCommonSkill() {
    		_NormalSkill = true;
            CommonLabel.SetActive(true);
            SpecialLabel.SetActive(false);
            LTCombatHudController.Instance.AutoSkillString[Position] = 'n';
            string sc = new string(LTCombatHudController.Instance.AutoSkillString);
            PlayerPrefs.SetString(LoginManager.Instance.LocalUserId.Value.ToString()+"AutoCombatSkill", sc);
        }
    
        public void DoAction() {
            this.mDMono.transform.localPosition = mDMono.transform.localPosition + _moveUpDistance;
        }
    
        public void FinishAction() {
            var pos = mDMono.transform.localPosition;
            this.mDMono.transform.localPosition = new Vector3(pos.x, _originY, pos.z);
        }
    
        public void SetDeath(bool isTrue) {
            if (ItemCharSyncData.Hp <= 0||isTrue) {
                Icon.GetComponent<BoxCollider>().enabled = false;
                Icon.GetComponent<UIButton>().enabled = false;
                Icon.GetComponentInChildren<UISprite>().color = Color.magenta;
    			GraySpriteList.ForEach(gs => gs.color = Color.magenta);
    			BGSprite.gameObject.SetActive(false);
    			GrayBGSprite.gameObject.SetActive(true);
    			ParticleSystemUIComponent aoyiFX;
    			if ((aoyiFX = mDMono.gameObject.GetComponentInChildren<ParticleSystemUIComponent>()) != null)
    			{
    				aoyiFX.gameObject.SetActive(false);
    			}
    		}
            else {
                Icon.GetComponent<BoxCollider>().enabled = true;
                Icon.GetComponent<UIButton>().enabled = true;
                Icon.GetComponentInChildren<UISprite>().color = Color.white;
    			GraySpriteList.ForEach(gs => gs.color = Color.white);
    			BGSprite.gameObject.SetActive(true);
    			GrayBGSprite.gameObject.SetActive(false);
    		}
        }
    }
}
