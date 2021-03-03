using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.UI;
using UnityEngine;

namespace Hotfix_LT.Instance
{
    public class LTInstanceOptimizeManager : DynamicMonoHotfix
    {
        public static string PreBGName;
        public static LTInstanceOptimizeManager Instance = null;
        public GameObject RowObj;
        public GameObject RowObjContainer;
        public GameObject PlayerObj;
        public GameObject PlayerModelObj;

        public GameObject SelectObj;
        private SpriteRenderer SelectObjSprite;
        public GameObject FastBattleFxObj;

        private string bgName;
        public SpriteRenderer BGTexture;
        
        private GameObject EnvStaticPosObj;
        private GameObject EnvDynamicPosObj;

        public bool isPlayerReady;

        //playerroot下的Boom节点
        private GameObject PlayerRootBoomNode;
        private Transform PoolPath;
        private GameObject ControllerObj;
        private Queue<GameObject> ControllerObjQueue = new Queue<GameObject>();
        private GameObject NoticeObj;
        private Queue<GameObject> NoticeObjQueue = new Queue<GameObject>();
        private GameObject LobbyEliteFxObj;
        private Queue<GameObject> LobbyEliteFxObjQueue = new Queue<GameObject>();
        private GameObject BoomObj;
        private Queue<GameObject> BoomObjQueue = new Queue<GameObject>();

        public Instance3DLobby Lobby = null;
        private GM.AssetLoader<GameObject> Loader = null;
        public List<GameObject> RoleItemList = new List<GameObject>();
        private List<GameObject> FlyItemList=new List<GameObject>();
        private string ModelName = null;
        public override void Awake()
        {
            base.Awake();
            LoadingLogic.AddCustomProgress(10);
            Transform t = mDMono.transform;
            RowObj = t.FindEx("TempPrefab/RowItem").gameObject;
            RowObjContainer = t.FindEx("FloorRoot").gameObject;
            //player
            PlayerObj = t.FindEx("PlayerRoot").gameObject;
            PlayerModelObj = PlayerObj.FindEx("Model").gameObject;
            PlayerRootBoomNode = PlayerObj.FindEx("Boom").gameObject;

            EnvDynamicPosObj = PlayerObj.FindEx("Main Camera/EnvDynamicPos");
            EnvStaticPosObj = t.gameObject.FindEx("EnvStaticPos");

            BGTexture = PlayerObj.GetComponent<SpriteRenderer>("Main Camera/BG");
            float scale = Mathf.Max((float)Screen.width / (float)Screen.height + 0.1f, 16f / 9f);
            BGTexture.transform.localScale = new Vector3(scale, scale);
            if (!string.IsNullOrEmpty(PreBGName))
            {
                SetBGTexture(PreBGName);
                PreBGName = null;
            }
            //
            SelectObj = t.FindEx("TempPrefab/SelectItem").gameObject;
            SelectObjSprite = t.GetComponent <SpriteRenderer>("TempPrefab/SelectItem/Sprite");
            FastBattleFxObj = t.FindEx("TempPrefab/FastBattleItem").gameObject;

            Transform ItemRoot = t.Find("TempPrefab/RoleItem");
            int count = ItemRoot.childCount;
            for (int i = 0; i < count; ++i)
            {
                RoleItemList.Add(ItemRoot.GetChild(i).gameObject);
            }
            ItemRoot = t.Find("TempPrefab/FlyItem");
            count = ItemRoot.childCount;
            for (int i = 0; i < count; ++i)
            {
                FlyItemList.Add(ItemRoot.GetChild(i).gameObject);
            }

            ControllerObj = t.FindEx("TempPrefab/ObjPrefab/ControllerItem").gameObject;
            NoticeObj = t.FindEx("TempPrefab/ObjPrefab/NoticeItem").gameObject;
            LobbyEliteFxObj = t.FindEx("TempPrefab/ObjPrefab/LobbyEliteFxItem").gameObject;
            BoomObj = t.FindEx("TempPrefab/ObjPrefab/BoomItem").gameObject;

            PoolPath = t.FindEx("TempPrefab/ObjPool");

            StartCoroutine(CreateBuddyModel());

            Instance = this;
        }

        public override void OnDestroy()
        {
            #region 环境特效销毁
            _envEffectAct?.Stop();
            _envEffectAct = null;

            _envEffectAct2?.Stop();
            _envEffectAct2 = null;

            _envMapStyleId = 0;
            #endregion

            ReleaseBGTexture();
            StopAllCoroutines();
            base.OnDestroy();
            Instance = null;
        }

        public void SetBGTexture(string name)
        {
            if (string.IsNullOrEmpty(bgName) || !bgName.Equals(name))
            {
                ReleaseBGTexture();
                bgName = name;
                ReferencedTextureManager.GetTexture2DAsync(bgName, SetTextureAction, mDMono.gameObject);
            }
        }

        public void SetPlayerState(bool state)
        {
            PlayerModelObj.CustomSetActive(state);
        }

        void SetTextureAction(string texName, Texture2D tex, bool bSuccessed)
        {
            if (bSuccessed)
            {
                if (BGTexture != null)
                {
                    Sprite sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    BGTexture.sprite = sp;
                }
            }
            else
            {
                if (BGTexture != null)
                {
                    EB.Debug.LogWarning("SetTextureAction: failed to load texture {0}, use default texture instead", texName);
                    Sprite sp = Sprite.Create(GM.TextureManager.DefaultTexture2D,new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    BGTexture.sprite = sp;
                }
            }
        }

        private void ReleaseBGTexture()
        {
            if(!string.IsNullOrEmpty(bgName))
            {
                ReferencedTextureManager.ReleaseTexture(bgName);
                bgName = null;
            }
        }

        private void CreatItem()
        {
            InitObjQueue(ControllerObj, ControllerObjQueue, 12);
            InitObjQueue(NoticeObj, NoticeObjQueue, 3);
            InitObjQueue(LobbyEliteFxObj, LobbyEliteFxObjQueue, 1);
            InitObjQueue(BoomObj, BoomObjQueue, 2);
        }
        private void InitObjQueue(GameObject obj, Queue<GameObject> queue, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                GameObject Obj = GameObject.Instantiate(obj, PoolPath);
                queue.Enqueue(Obj);
            }
        }

        private void SetRoleSprite(string path,string spriteName)
        {
            mDMono.transform.GetComponent<SpriteRenderer>(path).sprite = LTInstanceSpriteTool.GetRole(spriteName);
        }
        
        public Johny.Action.ActionGeneralParticle HoldPlayerEffect(string effectName)
        {
            if(PlayerRootBoomNode)
            {
                return new Johny.Action.ActionGeneralParticle(0.0f, PlayerRootBoomNode, effectName, Vector3.zero, Vector3.zero);
            }

            return null;
        }

        #region 环境特效
        ///环境特效字典
        private int _envMapStyleId = 0;
        private readonly Dictionary<int, string> EnvEffectDic = new Dictionary<int, string>
        {
            {1, "fx_fb_caodi_csat"},  //草地
            {2, ""},  //圣城
            {3, ""},  //宫殿
            {4, "fx_fb_caodi_csat"},  //平原
            {5, "fx_fb_xuedi_cast"},   //雪山
            {6, "fx_fb_haidi_cast"},  //海洋
            {7, "fx_fb_shamo_cast"},     //沙漠
            {8, ""},  //金字塔
            {9, "fx_fb_manzupingyuan_cast"},  //蛮族平原
            {10, "fx_fb_renyudao_cast"},  //人鱼岛
            {11, "fx_fb_renyudao_cast"},  //天空城
            {12, ""}
        };
        private Johny.Action.ActionGeneralParticle _envEffectAct = null;
        private readonly Dictionary<int, string> EnvEffectDic2 = new Dictionary<int, string>
        {
            {1, ""},  //草地
            {2, ""},  //圣城
            {3, ""},  //宫殿
            {4, ""},  //平原
            {5, ""},  //雪山
            {6, "fx_fb_haidi_cast2"},  //海洋
            {7, ""},  //沙漠
            {8, ""},  //金字塔
            {9, ""},  //蛮族平原
            {10, ""},  //人鱼岛
            {11, ""},  //天空城
            {12, ""}
        };
        private Johny.Action.ActionGeneralParticle _envEffectAct2 = null;
        public void GenEnvironmentEffect(int mapStyleId)
        {
            if (mapStyleId > 0)
            {
                if (_envMapStyleId != mapStyleId)
                {
                    if (EnvEffectDic.TryGetValue(mapStyleId, out string effectName))
                    {
                        _envEffectAct?.Stop();
                        if (!string.IsNullOrEmpty(effectName))
                        {
                            _envEffectAct = new Johny.Action.ActionGeneralParticle(0.0f, EnvDynamicPosObj, effectName, Vector3.zero, Vector3.zero);
                        }
                    }
                    if (EnvEffectDic2.TryGetValue(mapStyleId, out string effectName2))
                    {
                        _envEffectAct2?.Stop();
                        if (!string.IsNullOrEmpty(effectName2))
                        {
                            _envEffectAct2 = new Johny.Action.ActionGeneralParticle(0.0f, EnvStaticPosObj, effectName2, Vector3.zero, Vector3.zero);
                        }
                    }
                    _envMapStyleId = mapStyleId;
                }
            }
        }
        #endregion

        #region About MyRole
        //记录我上次的朝向
        public static Vector3 MyLastDir{get; private set;} = UI.LTInstanceConfig.LEFT_MODEL_ROTATION;
        private IEnumerator CreateBuddyModel()
        {
            isPlayerReady = false;
            var statTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(UI.LTMainHudManager.Instance.UserLeaderTID);
            if (statTpl == null)
            {
                isPlayerReady = true;
                yield break;
            }

            var infoTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(statTpl.character_id, UI.LTMainHudManager.Instance.UserLeaderSkin);
            if (infoTpl == null)
            {
                isPlayerReady = true;
                yield break;
            }

            string modelName = infoTpl.model_name;//需添加皮肤
            if (Lobby == null && Loader == null)
            {
                Loader = new GM.AssetLoader<GameObject>("Instance3DLobby", mDMono.gameObject);
                UI.UI3DLobby.Preload(modelName);
                yield return Loader;
                if (Loader.Success)
                {
                    Loader.Instance.transform.SetParent(PlayerObj.transform.Find("Model"));
                    Loader.Instance.transform.localPosition = Vector3.zero;
                    Loader.Instance.transform.localScale = new Vector3(UI.LTInstanceConfig.MAP_MODLE_SCALE, UI.LTInstanceConfig.MAP_MODLE_SCALE, UI.LTInstanceConfig.MAP_MODLE_SCALE);
                    Lobby = Loader.Instance.GetMonoILRComponent<Instance3DLobby>();
                }
                LoadingLogic.AddCustomProgress(5);
            }

            if (Lobby != null)
            {
                Lobby.VariantName = modelName;
                while (Lobby.Current == null || Lobby.Current.character == null)
                {
                    yield return null;
                }

                while (Lobby.Current.character == null)
                {
                    yield return null;
                }

                Lobby.SetCharRotation(MyLastDir);
            }

            isPlayerReady = true;
        }

        public void SetMoveDir(UI.LTInstanceNode start,Vector2 end)
        {
            if (Lobby != null)
            {
                UI.LTInstanceNode.DirType dir = start.GetDirByPos((int)end.x, (int)end.y);
                if (dir == UI.LTInstanceNode.DirType.UP)
                {
                    Lobby.SetCharRotation(UI.LTInstanceConfig.TOP_MODEL_ROTATION);
                }
                else if (dir == UI.LTInstanceNode.DirType.Down)
                {
                    Lobby.SetCharRotation(UI.LTInstanceConfig.BOTTOM_MODEL_ROTATION);
                }
                else if (dir == UI.LTInstanceNode.DirType.Right)
                {
                    Lobby.SetCharRotation(UI.LTInstanceConfig.RIGHT_MODEL_ROTATION);
                }
                else if (dir == UI.LTInstanceNode.DirType.Left)
                {
                    Lobby.SetCharRotation(UI.LTInstanceConfig.LEFT_MODEL_ROTATION);
                }
            }
        }

        public bool MyRoleMoveDir(UI.LTInstanceNode start, UI.LTInstanceNode end)
        {
            if (Lobby != null)
            {
                UI.LTInstanceNode.DirType dir = start.GetDirByPos(end.x, end.y);
                if (dir == UI.LTInstanceNode.DirType.UP)
                {
                    Lobby.SetCharRotation(UI.LTInstanceConfig.TOP_MODEL_ROTATION);
                    MyLastDir = UI.LTInstanceConfig.TOP_MODEL_ROTATION;
                }
                else if (dir == UI.LTInstanceNode.DirType.Down)
                {
                    Lobby.SetCharRotation(UI.LTInstanceConfig.BOTTOM_MODEL_ROTATION);
                    MyLastDir = UI.LTInstanceConfig.BOTTOM_MODEL_ROTATION;
                }
                else if (dir == UI.LTInstanceNode.DirType.Right)
                {
                    Lobby.SetCharRotation(UI.LTInstanceConfig.RIGHT_MODEL_ROTATION);
                    MyLastDir = UI.LTInstanceConfig.RIGHT_MODEL_ROTATION;
                }
                else if (dir == UI.LTInstanceNode.DirType.Left)
                {
                    Lobby.SetCharRotation(UI.LTInstanceConfig.LEFT_MODEL_ROTATION);
                    MyLastDir = UI.LTInstanceConfig.LEFT_MODEL_ROTATION;
                }
                else
                {
                    return false;
                }
    
                Lobby.SetCharMoveState(MoveController.CombatantMoveState.kLocomotion);
            }
            return true;
        }

        public GameObject GetRoleItemByImgName(string roleType)
        {
            for (int i = 0; i < RoleItemList.Count; ++i)
            {
                if (RoleItemList[i].name.CompareTo(roleType) == 0)
                {
                    return RoleItemList[i];
                }
            }
            return RoleItemList[0];
        }

        public GameObject GetFlyItem(string roleType = "FlyBase")
        {
            for (int i = 0; i < FlyItemList.Count; ++i)
            {
                if (FlyItemList[i].name.CompareTo(roleType) == 0)
                {
                    return FlyItemList[i];
                }
            }
            return FlyItemList[0];
        }
        #endregion

        #region 快速战斗特效
        public void ShowFastBattleFx(Transform t)
        {
            FastBattleFxObj.transform.SetParent(t);
            FastBattleFxObj.transform.localPosition = Vector3.zero;
            FastBattleFxObj.transform.localScale = Vector3.one;
            FastBattleFxObj.CustomSetActive(true);
        }
        public void HideFastBattleFx()
        {
            FastBattleFxObj.transform.SetParent(PoolPath);
            FastBattleFxObj.CustomSetActive(false);
        }
        #endregion

        #region Item队列相关
        private GameObject GetItem(Transform parent, GameObject mObj, Queue<GameObject> mQueue)
        {
            GameObject obj = null;
            if (mQueue != null && mQueue.Count > 0)
            {
                obj = mQueue.Dequeue();
            }
            else
            {
                obj = GameObject.Instantiate(mObj);
            }
            if (obj != null)
            {
                obj.CustomSetActive(true);
                if (parent != null)
                {
                    obj.transform.SetParent(parent);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localScale = Vector3.one;
                }
            }
            return obj;
        }
        private void SetItem(GameObject obj, Queue<GameObject> mQueue)
        {
            if (obj == null)
            {
                return;
            }
            obj.transform.SetParent(PoolPath);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.CustomSetActive(false);
            if (mQueue != null) mQueue.Enqueue(obj);
        }
        #endregion

        #region 禁止
        bool initControllerSprite = true;
        public GameObject GetControllerItem(Transform parent = null)
        {
            if (initControllerSprite)
            {
                SetRoleSprite("TempPrefab/ObjPrefab/ControllerItem/Sprite", "Copy_Icon_Jinzhi");
                initControllerSprite = false;
            }
            return GetItem(parent, ControllerObj, ControllerObjQueue);
        }
        public void SetControllerItem(GameObject obj)
        {
            SetItem(obj, ControllerObjQueue);
        }
        #endregion

        #region 引导或提示
        bool initNoticeSprite = true;
        public GameObject GetNoticeItem(Transform parent = null)
        {
            if (initNoticeSprite)
            {
                SetRoleSprite("TempPrefab/ObjPrefab/NoticeItem/Sprite", "Copy_Icon_Tishi");
                initNoticeSprite = false;
            }
            return GetItem(parent, NoticeObj, NoticeObjQueue);
        }
        public void SetNoticeItem(GameObject obj)
        {
            SetItem(obj, NoticeObjQueue);
        }
        #endregion

        #region 精英怪特效
        public GameObject GetLobbyEliteFxItem(Transform parent = null)
        {
            return GetItem(parent, LobbyEliteFxObj, LobbyEliteFxObjQueue);
        }
        public void SetLobbyEliteFxItem(GameObject obj)
        {
            SetItem(obj, LobbyEliteFxObjQueue);
        }
        #endregion

        #region 炸弹
        private bool initBoomSprite = true;
        public GameObject GetBoomItem(Transform parent = null)
        {
            if (initBoomSprite)
            {
                SetRoleSprite("TempPrefab/ObjPrefab/BoomItem/Sprite", "Copy_Icon_Zhadan");
                initBoomSprite = false;
            }
            return GetItem(parent, BoomObj, BoomObjQueue);
        }
        public void SetBoomItem(GameObject obj)
        {
            SetItem(obj, BoomObjQueue);
        }
        #endregion

        #region 选中与无法选中
        public void ShowSelectObj(bool type, Transform t)
        {
            SelectObjSprite.sprite = LTInstanceSpriteTool.GetRole(type ? "Copy_Icon_Xuanzhong" : "Copy_Icon_WufaXuanzhong");// SetRoleSprite("TempPrefab/SelectItem/Sprite", "Copy_Icon_Xuanzhong");
            if(!type) UI.MessageTemplateManager.ShowMessage(UI.eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTInstanceFloorTemp_12239"));
            SelectObj.transform.SetParent(t);
            SelectObj.transform.localPosition = Vector3.zero;
            SelectObj.transform.localScale = Vector3.one;
            SelectObj.CustomSetActive(true);
        }
        public void HideSelectObj()
        {
            SelectObj.transform.SetParent(PoolPath);
            SelectObj.CustomSetActive(false);
        }

        #endregion
    }

    public class LTInstanceSpriteTool
    {
        public static Sprite GetMapStyleSprite(string name)
        {
            Sprite sp = null;
            if(UI.LTInstanceMapModel.Instance.InstanceMapStyleAtlas!=null) sp= UI.LTInstanceMapModel.Instance.InstanceMapStyleAtlas.GetSprite(name);
            return sp;
        }

        public static Sprite GetRole(string name)
        {
            Sprite sp = null;
            if (UI.LTInstanceMapModel.Instance.InstanceRoleAtlas != null) sp = UI.LTInstanceMapModel.Instance.InstanceRoleAtlas.GetSprite(name);
            return sp;
        }

        public static string GetFlyItemSpriteName(UI.LTShowItemData item)
        {
            if (item.type.Equals(LTShowItemType.TYPE_RES))//钻石
            {
                if (item.id.Equals(UI.LTResID.HcName)) return "Copy_Icon_Zuanshi";
                else if (item.id.Equals(UI.LTResID.GoldName)) return "Copy_Icon_Jinbi";
                else return "Copy_Icon_Jingyanyaoshui";
            }

            int id = 0;
            int.TryParse(item.id, out id);
            if (id > 0)
            {
                if (item.type.Equals(LTShowItemType.TYPE_HEROSHARD))
                {
                    var hero = Data.CharacterTemplateManager.Instance.GetHeroInfoByStatId(id);
                    if (hero != null)
                    {
                        switch (hero.role_grade)
                        {
                            case 1: return "Copy_Icon_Nsuipian";
                            case 2: return "Copy_Icon_Rsuipian";
                            case 3: return "Copy_Icon_SRsuipian";
                            default: return "Copy_Icon_SSRsuipian";
                        }
                    }
                }
                if (id >= 1021 && id <= 1025)//魔法书
                {
                    return "Copy_Icon_Mofashu";
                }
                else if (id >= 1031 && id <= 1033)//锻体液
                {
                    return "Copy_Icon_Duantiye";
                }
                else if (id / 1000 == 3)//装备箱
                {
                    int quality = id % 1000 / 100;
                    switch (quality)
                    {
                        case 1:
                        case 2: return "Copy_Icon_Baoxiang_Lv";
                        case 3: return "Copy_Icon_Baoxiang_Lan";
                        case 4: return "Copy_Icon_Baoxiang_Zi";
                        case 5: return "Copy_Icon_Baoxiang_Cheng";
                        default: return "Copy_Icon_Baoxiang_Hong";
                    }
                }
                else if (item.id.Equals(LTDrawCardConfig.LOTTERY_GOLD_ID))
                {
                    return "Copy_Icon_Jinbijuan";
                }
                else if (item.id.Equals(LTDrawCardConfig.LOTTERY_HC_ID))
                {
                    return "Copy_Icon_Zuanshijuan";
                }
                else if (id == 2014)
                {
                    return "Copy_Icon_Shenqicailiao_1";
                }
                else if (id == 2015)
                {
                    return "Copy_Icon_Shenqicailiao_2";
                }
                else if (id == 2016)
                {
                    return "Copy_Icon_Shenqicailiao_3";
                }
            }
            return "Copy_Icon_Baoxiang";//其他
        }

        private static readonly Dictionary<string, string> DecorationEffectDic = new Dictionary<string, string>
        {
            {"Map_2_Penquan_1", "fx_fb_Penquan_cast"},
            {"Map_11_Penquan_1", "fx_fb_Penquan_cast2"},
            {"Map_8_Huo_1", "fx_fb_huoba_cast"},
            {"Map_11_Shuijing_1", "fx_fb_shuijing_cast"},
            //{"Map_11_Deng_1", "fx_fb_shuijing_cast3"},
        };

        private static readonly Dictionary<string, string> TerraEffectDic = new Dictionary<string, string>
        {
            {"Map_5_Qiang_3", "fx_fb_bingmian_cast"},
            {"Map_11_Qiang_4", "fx_fb_shuijing_cast2"},
        };

        public static void SetTerraEffect(string name, Transform parent)
        {
            string effectName = null;
            if (TerraEffectDic.TryGetValue(name, out effectName))
            {
                EB.Assets.LoadAsyncAndInit<GameObject>(effectName, (assetName, o, succ) =>
                {
                    if (succ)
                    {
                        o.transform.localPosition = Vector3.zero;
                    }
                }, parent, parent.gameObject, true);
            }
        }

        public static void SetDecorationEffect(string name,Transform parent,bool changeOrder)
        {
            string effectName = null;
            if (DecorationEffectDic.TryGetValue(name,out effectName))
            {
                EB.Assets.LoadAsyncAndInit<GameObject>(effectName, (assetName, o, succ) =>
                {
                    if (succ)
                    {
                        o.transform.localPosition = Vector3.zero;
                        if (changeOrder)
                        {
                            var renderers= o.transform.GetComponentsInChildren<Renderer>();
                            for(int i=0;i< renderers.Length; ++i)
                            {
                                renderers[i].sortingOrder--;
                            }
                        }
                    }
                }, parent, parent.gameObject, true);
            }
        }
    }

    public class LTBoomItemTemp: DynamicMonoHotfix
    {
        public SpriteRenderer Num1Sprite;
        public SpriteRenderer Num2Sprite;

        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.transform;
            Num1Sprite =t.GetComponent<SpriteRenderer>("Sprite/Num1");
            Num2Sprite = t.GetComponent<SpriteRenderer>("Sprite/Num2");
        }

        public void SetNum(int num)
        {
            if (num > 0)
            {
                int num1 = num % 10;
                int num2 = num / 10;
                Num1Sprite.sprite = LTInstanceSpriteTool.GetRole(string.Format("Copy_Word_1_{0}",num1));
                if (num2 > 0) Num2Sprite.sprite = LTInstanceSpriteTool.GetRole(string.Format("Copy_Word_1_{0}", num2));
                else Num2Sprite.sprite = null;
                if (num <=2) FusionAudio.PostEvent("UI/New/DaoShuBu", true);
            }
            else
            {
                mDMono.transform.gameObject.CustomSetActive(false);
            }
        }

    }

    public class LTInstanceFloorTemp : UI.LTInstanceNodeTemp
    {
        public SpriteRenderer Terra;

        public SpriteRenderer Decoration;
        
        public Transform LobbyRoot;
        public GameObject LobbyShadow;
        public Transform Role;
        public List<GameObject> StarList;
        public Transform BoomRoot;

        private LTInstanceRoleBase roleItem;

        private int RoleId;
        private bool IsNotice = false;
        private GameObject NoticeObj;
        private bool isController = false;
        private GameObject ControllerObj;
        private GameObject LobbyEliteFxObj;
        private LTBoomItemTemp BoomObj;

        private GameObject OtherModel;
        private Instance3DLobby Lobby = null;
        private GM.AssetLoader<GameObject> Loader = null;
        private string ModelName = null;

        private bool isActFinish = false;
        private Johny.Action.ActionCellBornMove act1;
        private Johny.Action.ActionAlphaChange act2;
        private Johny.Action.ActionAlphaChange act3;
        private Johny.Action.ActionCellStampDown act4;
        private Johny.Action.ActionModelRotation act5;
        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.transform;
            Terra = t.GetComponent<SpriteRenderer>("Terra");

            Decoration = t.GetComponent<SpriteRenderer>("Terra/Decoration");
            
            LobbyRoot = t.Find("Terra/Lobby");
            LobbyShadow = LobbyRoot.Find("Shadow").gameObject;
            Role = t.Find("Terra/Role");
            BoomRoot = t.Find("Terra/Boom");

            StarList = new List<GameObject>();
            StarList.Add(t.Find("Terra/StarList/Star").gameObject);
            StarList.Add(t.Find("Terra/StarList/Star (1)").gameObject);
            StarList.Add(t.Find("Terra/StarList/Star (2)").gameObject);

            isActFinish = false;
            Decoration.gameObject.CustomSetActive(false);
        }
        
        public override void OnDestroy()
        {
            act1.Stop();
            act2.Stop();
            if (act3 != null) act3.Stop();
            if (act4 != null) act4.Stop();
            if (act5 != null) act5.Stop();
            base.OnDestroy();
        }

        public override void SetData(int num, UI.LTInstanceNode data)
        {
            base.SetData(num, data);
            //Born Move
            {
                var bornPos = new Vector3(0, -1f, 0);
                var middlePos = bornPos + new Vector3(0, 1.3f, 0);
                var finalPos = middlePos + new Vector3(0, -0.3f, 0);
                float delay = LTInstanceConfigManager.GetFloatValue("CellBornMove_Delay", 0.0f);
                float during = LTInstanceConfigManager.GetFloatValue("CellBornMove_During", 0.5f);
                act1 = new Johny.Action.ActionCellBornMove(delay, during, Terra.gameObject, bornPos, middlePos, finalPos);
                act1.SetFinishHandler(delegate (Johny.Action.ActionCellBornMove.FinishStatus e) {
                    if (e == Johny.Action.ActionCellBornMove.FinishStatus.Complete)
                    {
                        if (mDMono != null)
                        {
                            isActFinish = true;
                        }
                    }
                });
            }
            //Born FadeIn
            {
                float delay = LTInstanceConfigManager.GetFloatValue("CellBornFadeIn_Delay", 0.0f);
                float during = LTInstanceConfigManager.GetFloatValue("CellBornFadeIn_During", 0.45f);
                float from = LTInstanceConfigManager.GetFloatValue("CellBornFadeIn_From", 0.0f);
                float to = LTInstanceConfigManager.GetFloatValue("CellBornFadeIn_To", 1.0f);
                act2 = new Johny.Action.ActionAlphaChange(delay, during, Terra, from, to);
                act2.SetFinishHandler(delegate (Johny.Action.ActionAlphaChange.FinishStatus e) {
                    if (e == Johny.Action.ActionAlphaChange.FinishStatus.Complete)
                    {
                        if (mDMono != null)
                        {
                            if (Decoration.sprite != null)
                            {
                                act3 = new Johny.Action.ActionAlphaChange(delay, during, Decoration, from, to);
                                Decoration.gameObject.CustomSetActive(true);
                            }
                        }
                    }
                });
            }

            AwakeTerra();
            CommonUpdate();
            AwakeRole();
        }

        public override void UpdateData(UI.LTInstanceNode data)
        {
            base.UpdateData(data);
            CommonUpdate();
            UpdateRole();
        }

        private void CommonUpdate()
        {
            bool isControllered = nodeData.IsControllered && !nodeData.IsSight;
            if (isControllered && nodeData.CanPass)
            {
                if (!isController)
                {
                    ControllerObj = LTInstanceOptimizeManager.Instance.GetControllerItem(mDMono.transform);
                    isController = true;
                }
            }
            else 
            {
                if (isController)
                {
                    AwakeTerra();//重新设置图片
                    GameObject obj = ControllerObj;
                    LTInstanceOptimizeManager.Instance.SetControllerItem(obj);
                    ControllerObj = null;
                    isController = false;
                }
            }

            bool isBomb = nodeData.RoleData.CampaignData.Bomb > 0 && nodeData.RoleData.Id != 33;
            int BombTimer = nodeData.RoleData.CampaignData.Bomb;
            if (isBomb && BombTimer > 0)
            {
                if (BoomObj == null)
                {
                    GameObject obj=LTInstanceOptimizeManager.Instance.GetBoomItem(BoomRoot);
                    BoomObj = obj.GetMonoILRComponent<LTBoomItemTemp>();
                }
                BoomObj.SetNum(BombTimer);

            }
            else
            {
                if (BoomObj != null)
                {
                    LTInstanceOptimizeManager.Instance.SetBoomItem(BoomObj.mDMono .gameObject);
                }
            }
        }

        protected void AwakeTerra()
        {
            Terra.sprite = LTInstanceSpriteTool.GetMapStyleSprite(nodeData.Img);
        }

        protected void AwakeRole()
        {
            if (nodeData.RoleData.Id > 0 && nodeData.RoleData.Type ==1)
            {
                CreateDecorationItem();
                return;
            }
            
            if (!string.IsNullOrEmpty(nodeData.RoleData.OtherModel))
            {
                CreateOtherModelFunc();
                return;
            }

            bool needShowRole = nodeData.IsSight && nodeData.RoleData.Id > 0 || UI.LTInstanceMapModel.Instance.IsAlwayShowRole(nodeData.RoleData.Id);
            if (!string.IsNullOrEmpty(nodeData.RoleData.Img) && ((!needShowRole && !string.IsNullOrEmpty(nodeData.RoleData.Model) && UI.LTInstanceMapModel.Instance.IsHunter()) || !nodeData.CanPass))
            {
                if (!nodeData.IsControllered)
                {
                    CreateRoleItem();
                }
                return;
            }

            bool isCompleteMainCampaign = nodeData.RoleData.CampaignData.CampaignId > 0 && nodeData.RoleData.CampaignData.Star > 0;
            if (!needShowRole && !isCompleteMainCampaign)
            {
                return;
            }

            bool hasModel = !string.IsNullOrEmpty(nodeData.RoleData.Model) && !isCompleteMainCampaign;

            bool needShowImgRole = !string.IsNullOrEmpty(nodeData.RoleData.Img) && !hasModel;

            if (needShowImgRole)
            {
                CreateRoleItem();

                if (nodeData.RoleData.CampaignData.IsDoorOpen)
                {
                    roleItem.SetOpenData();
                }
            }

            if (hasModel)
            {
                CreateLobbyFunc();
            }
            else
            {
                ClearLobbyFunc();
            }

            if (isCompleteMainCampaign)
            {
                InitStarList();
            }
        }
        
        protected void UpdateRole()
        {
            if (nodeData.RoleData.Id > 0 && nodeData.RoleData.Type == 1)
            {
                return;
            }

            if (!string.IsNullOrEmpty(nodeData.RoleData.OtherModel))
            {
                if(OtherModel == null)
                {
                    CreateOtherModelFunc();
                }
                return;
            }

            bool needShowRole = nodeData.IsSight && nodeData.RoleData.Id > 0 ||UI.LTInstanceMapModel.Instance.IsAlwayShowRole(nodeData.RoleData.Id);
            if (!string.IsNullOrEmpty(nodeData.RoleData.Img) && ((!needShowRole && !string.IsNullOrEmpty(nodeData.RoleData.Model) && UI.LTInstanceMapModel.Instance.IsHunter()) || !nodeData.CanPass))
            {
                if (roleItem == null)
                {
                    if (!nodeData.IsControllered)
                    {
                        CreateRoleItem();
                    }
                }
                return;
            }

            bool isCompleteMainCampaign = nodeData.RoleData.CampaignData.CampaignId > 0 && nodeData.RoleData.CampaignData.Star > 0;
            if (!needShowRole && !isCompleteMainCampaign)
            {
                if (roleItem != null)
                {
                    GameObject.Destroy(roleItem.mDMono.gameObject);
                    roleItem = null;
                }
                ClearLobbyFunc();
                return;
            }

            bool hasModel = !string.IsNullOrEmpty(nodeData.RoleData.Model) && !isCompleteMainCampaign;
            bool needShowImgRole = !string.IsNullOrEmpty(nodeData.RoleData.Img) && !hasModel;

            if (!needShowImgRole)
            {
                if (roleItem != null)
                {
                    GameObject.Destroy(roleItem.mDMono.gameObject);
                    roleItem = null;
                }
            }
            else
            {
                if (roleItem != null)
                {
                    string strType = (nodeData.RoleData.IsDynImg) ? "RoleShake" : "RoleBase";
                    if (UI.LTInstanceConfig.RoleItemNameDic.ContainsKey(nodeData.RoleData.Img))
                    {
                        strType = UI.LTInstanceConfig.RoleItemNameDic[nodeData.RoleData.Img];
                    }
                    if (roleItem.mDMono.gameObject.name != string.Format("{0}(Clone)", strType))
                    {
                        GameObject.Destroy(roleItem.mDMono.gameObject);
                        roleItem = null;

                        CreateRoleItem();
                    }

                    if (nodeData.RoleData.CampaignData.IsDoorOpen)
                    {
                        roleItem.SetOpenData();
                    }
                }
                else
                {
                    CreateRoleItem();

                    if (nodeData.RoleData.CampaignData.IsDoorOpen)
                    {
                        roleItem.SetOpenData();
                    }
                }
            }

            if (hasModel)
            {
                CreateLobbyFunc();
            }
            else
            {
                ClearLobbyFunc();
            }



            if (Lobby != null)
            {
                UpdateMonsterRotation(false);
            }

            if (isCompleteMainCampaign)
            {
                InitStarList();
            }
        }

        private void InitStarList()
        {
            for (int i = 0; i < StarList.Count; i++)
            {
                if (i < nodeData.RoleData.CampaignData.Star)
                {
                    StarList[i].gameObject.CustomSetActive(true);
                }
                else
                {
                    StarList[i].gameObject.CustomSetActive(false);
                }
            }
        }

        private void CreateRoleItem()
        {
            string strType = (nodeData.RoleData.IsDynImg) ? "RoleShake" : "RoleBase";
            if (UI.LTInstanceConfig.RoleItemNameDic.ContainsKey(nodeData.RoleData.Img))
            {
                strType = UI.LTInstanceConfig.RoleItemNameDic[nodeData.RoleData.Img];
            }

            GameObject tempObj = LTInstanceOptimizeManager.Instance.GetRoleItemByImgName(strType);
            GameObject roleItemObj = GameObject.Instantiate(tempObj, Role.transform);
            //位置配置
            roleItemObj.transform.localEulerAngles = nodeData.RoleData.Rotation;
            roleItemObj.transform.localScale = nodeData.RoleData.Span;
            Vector3 offset = nodeData.RoleData.Offset;
            roleItemObj.transform.localPosition += offset;

            roleItem = roleItemObj.GetMonoILRComponent<LTInstanceRoleBase>();
            roleItem.Init(nodeData);
        }

        private void CreateDecorationItem()
        {
            Decoration.sprite = LTInstanceSpriteTool.GetMapStyleSprite(nodeData.RoleData.Img);
            //位置配置
            Decoration.transform.localEulerAngles = nodeData.RoleData.Rotation;
            Decoration.transform.localScale = nodeData.RoleData.Span;
            Vector3 offset = nodeData.RoleData.Offset+ Decoration.transform.localPosition;
            Decoration.transform.localPosition = offset;
        }

        private void CreateLobbyFunc()
        {
            StartCoroutine(CreateMonster(nodeData.RoleData.Model, UI.LTInstanceMapModel.Instance.isFirstCreatFloor));
            if (nodeData.RoleData.IsElite && LobbyEliteFxObj == null)
            {
                LobbyEliteFxObj = LTInstanceOptimizeManager.Instance.GetLobbyEliteFxItem(LobbyRoot);
            }
            LobbyShadow.CustomSetActive(true);
        }

        private void ClearLobbyFunc()
        {
            if (Lobby != null)
            {
                GameObject.Destroy(Lobby.mDMono.gameObject);
                if (LobbyEliteFxObj != null) LTInstanceOptimizeManager.Instance.SetLobbyEliteFxItem(LobbyEliteFxObj);
                Lobby = null;
                Loader = null;
                ModelName = string.Empty;
                if (act5 != null)
                {
                    act5.Stop();
                    act5 = null;
                }
            }
            LobbyShadow.CustomSetActive(false);
        }

        private IEnumerator CreateMonster(string modelName,bool isFirstCreat)
        {
            if (string.IsNullOrEmpty(modelName))
            {
                yield break;
            }

            if (modelName == ModelName)
            {
                yield break;
            }

            ModelName = modelName;
            if (Lobby == null && Loader == null)
            {
                Loader = new GM.AssetLoader<GameObject>("Instance3DLobby", mDMono.gameObject);
                UI.UI3DLobby.Preload(modelName);
                yield return Loader;
                if (Loader.Success)
                {
                    Loader.Instance.transform.SetParent(LobbyRoot);
                    Loader.Instance.transform.localPosition = Vector3.zero;

                    if (nodeData.RoleData.Order == "Boss")
                    {
                        //UI.LTInstanceConfig.MAP_BOSS_SCALE
                        Loader.Instance.transform.localScale = new Vector3(nodeData.RoleData.ModelScale, nodeData.RoleData.ModelScale, nodeData.RoleData.ModelScale);

                        if (!isFirstCreat)
                        {
                            FusionAudio.PostEvent("UI/New/Warn", true);
                            UI.GlobalMenuManager.Instance.Open("LTChallengeInstanceBossView");
                        }
                        LobbyShadow.transform.localScale = new Vector3(4.2f,3f);
                    }
                    else
                    {
                        Loader.Instance.transform.localScale = new Vector3(UI.LTInstanceConfig.MAP_MODLE_SCALE, UI.LTInstanceConfig.MAP_MODLE_SCALE, UI.LTInstanceConfig.MAP_MODLE_SCALE);
                        LobbyShadow.transform.localScale = new Vector3(1f, 0.8f);
                    }

                    Lobby = Loader.Instance.GetMonoILRComponent<Instance3DLobby>();
                }
            }

            if (Lobby == null)
            {
                yield break;
            }

            Lobby.VariantName = modelName;
            while (Lobby == null || Lobby.Current == null || Lobby.Current.character == null)
            {
                if (Lobby == null)
                {
                    yield break;
                }
                else
                {
                    yield return null;
                }
            }

            Lobby.SetCharMoveState(MoveController.CombatantMoveState.kReady);
            UpdateMonsterRotation();
            if (nodeData.RoleData.Order == "Boss")
            {
                Renderer[] renders = Lobby.Current.mDMono.transform.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renders.Length; i++)
                {
                    Material[] mats = renders[i].materials;
                    for (int j = 0; j < mats.Length; j++)
                    {
                        Material mat = new Material(mats[j]);
                        mat.SetFloat("_SpecularPower", 50f);
                        mats[j] = mat;
                    }
                    renders[i].materials = mats;
                }
            }

            if (nodeData.RoleData.Order == "Hero" || nodeData.RoleData.Order == "Hire")
            {
                ShowNotice();
            }
        }

        private void UpdateMonsterRotation(bool IsInit=true)
        {
            if (MapCtrl.curNode != null)
            {
                Vector3 tar = new Vector3(MapCtrl.curNode.x - nodeData.x, 0, nodeData.y - MapCtrl.curNode.y);
                Quaternion baserot = Quaternion.Euler(UI.LTInstanceConfig.LEFT_MODEL_ROTATION);
                Quaternion rot = Quaternion.FromToRotation(Vector3.left, tar);
                if (IsInit)
                {
                    Lobby.SetCharRotation(baserot * rot);
                }
                else
                {
                    Transform t=Lobby.Current.mDMono.transform;
                    var rotateFrom = t.eulerAngles;
                    var rotateTo = (baserot * rot).eulerAngles;
                    if (act5 == null)
                    {
                        float delay = LTInstanceConfigManager.GetFloatValue("ModelRotation_Delay", 0.2f);
                        float during = LTInstanceConfigManager.GetFloatValue("ModelRotaion_During", 0.2f);
                        act5 = new Johny.Action.ActionModelRotation(delay, during, t.gameObject, rotateFrom, rotateTo);
                    }
                    else
                    {
                        act5.Restart(rotateFrom, rotateTo);
                    }
                }
            }
        }

        public override void OnFloorClick()
        {
            if (!nodeData.CanPass)
            {
                return;
            }

            if (!isActFinish)
            {
                return;
            }

            if (OnCheckGuideNotice())
            {
                return;
            }

            if (nodeData.IsSight)
            {
                UI.GlobalMenuManager.CurGridMap_OnFloorClicked(nodeData, mDMono.transform);
                FusionAudio.PostEvent("UI/Floor/Click");
            }
            else
            {
                UI.MessageTemplateManager.ShowMessage(UI.eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTInstanceFloorTemp_12239"));
            }
        }

        public override void OnFloorAni()
        {
            if (act4 == null)
            {
                float delay = LTInstanceConfigManager.GetFloatValue("CellStampDown_Delay", 0.2f);
                float during = LTInstanceConfigManager.GetFloatValue("CellStampDown_During", 0.3f);
                float toY = LTInstanceConfigManager.GetFloatValue("CellStampDown_ToY", -0.1f);
                act4 = new Johny.Action.ActionCellStampDown(delay, during,Terra.gameObject ,Vector3.zero, new Vector3(0, toY, 0));
            }
            else
            {
                act4.Restart();
            }
        }

        #region 快速战斗
        public void ShowQuickBattleFX()
        {
            UI.CommonConditionParse.SetFocusViewName("IsQuickBattle");
            LTInstanceOptimizeManager.Instance.ShowFastBattleFx(LobbyRoot);
            LTInstanceOptimizeManager.Instance.SetPlayerState(false);
            if (MapCtrl != null)
            {
                MapCtrl.SetPlayerState(false);
            }
            ILRTimerManager.instance.AddTimer(1000, 1, CloseQuickBattleFx);
        }

        private void CloseQuickBattleFx(int timer)
        {
            LTInstanceOptimizeManager.Instance.HideFastBattleFx();
            LTInstanceOptimizeManager.Instance.SetPlayerState(true);
            if (MapCtrl != null)
            {
                MapCtrl.SetPlayerState(true);
            }
        }
        #endregion
        
        public void ShowNotice(int roleId = -1)
        {
            if (roleId > 0)
            {
                var tpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetChallengeChapterRole(roleId);
                if (tpl == null || string.IsNullOrEmpty(tpl.Guide[0]) || tpl.Order == "Hire") return;
                RoleId = roleId;
                IsNotice = true;
            }

            NoticeObj = LTInstanceOptimizeManager.Instance.GetNoticeItem(mDMono.transform);
        }
        public void HideNotice()
        {
            if (RoleId > 0)
            {
                RoleId = 0;
                IsNotice = false;
            }

            LTInstanceOptimizeManager.Instance.SetNoticeItem(NoticeObj);
            NoticeObj = null;
        }
        public bool OnCheckGuideNotice()
        {
            if (IsNotice)
            {
                UI.GlobalMenuManager.Instance.Open("LTChallengeInstanceNoticeView", RoleId);
                HideNotice();
                return true;
            }
            else
            {
                return false;
            }
        }
    
        public override void OpenTheDoor()
        {
            if (nodeData.RoleData.CampaignData.IsDoorOpen)
            {
                roleItem.SetOpenData();
            }
        }

        public void ClearRoleItem()
        {
            if (roleItem != null)
            {
                GameObject.Destroy(roleItem.mDMono.gameObject);
                roleItem = null;
            }
        }

        #region 副本boss宝箱相关
        private void CreateOtherModelFunc()
        {
            StartCoroutine(CreateOtherModel(nodeData.RoleData.OtherModel));
            LTInstanceMapModel.Instance.SetBossRewardHash(nodeData.x, nodeData.y);
        }
        
        private void ClearOtherModelFunc()
        {
            if (OtherModel != null)
            {
                GameObject.Destroy(OtherModel);
                OtherModel = null;
                ModelName = string.Empty;
                LTInstanceMapModel.Instance.SetBossRewardHash();
            }
        }
        
        private IEnumerator CreateOtherModel(string modelName)
        {
            if (string.IsNullOrEmpty(modelName))
            {
                yield break;
            }

            if (modelName == ModelName)
            {
                yield break;
            }

            ModelName = modelName;

            if (OtherModel == null && Loader == null)
            {
                Loader = new GM.AssetLoader<GameObject>(modelName, mDMono.gameObject);
                yield return Loader;
                if (Loader.Success)
                {
                    Loader.Instance.transform.SetParent(LobbyRoot);
                    Loader.Instance.transform.localPosition = Vector3.zero;
                    Loader.Instance.transform.localScale = new Vector3(nodeData.RoleData.ModelScale, nodeData.RoleData.ModelScale, nodeData.RoleData.ModelScale);
                    Loader.Instance.transform.localRotation = Quaternion.Euler(UI.LTInstanceConfig.LEFT_MODEL_ROTATION);
                    OtherModel = Loader.Instance;
                }
            }
        }

        public bool hasOtherModel()
        {
            return OtherModel != null;
        }

        public void ShowBossRewardFX(List<UI.LTShowItemData> datas, Vector3 screenPos, System.Action callback = null)
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 10f);
            Vector3 flyPos = Camera.main.ScreenToWorldPoint(screenPos);
            StartCoroutine(PlayBoxRewardFX(datas, flyPos, callback));
        }

        private IEnumerator PlayBoxRewardFX(List<UI.LTShowItemData> datas, Vector3 flyPos, System.Action callback)
        {
            yield return EB.Assets.LoadAsyncAndInit<GameObject>("fx_fb_baoxiang_kaiqi", (assetName, o, succ) =>
            {
                if (succ)
                {
                    if (OtherModel != null)
                    {
                        o.transform.localScale = Vector3.one;
                        o.transform.localRotation = Quaternion.Euler(Vector3.zero);
                        o.transform.localPosition = Vector3.zero;
                        OtherModel.transform.Find("baoxiang_ok/Baoxiang1").gameObject.CustomSetActive(false);
                    }
                }
            }, OtherModel.transform, OtherModel, true);

            int max = datas.Count;
            for(int i=0;i< max; ++i)
            {
                int index = Random.Range(0, max);
                if (i == index) continue;
                UI.LTShowItemData temp = datas[index];
                datas[index] = datas[i];
                datas[i] = temp;
            }

            yield return new WaitForSeconds(2f);
            ClearOtherModelFunc();
            yield return new WaitForSeconds(0.1f);
            List<LTInstanceFlyBase> flyList = new List<LTInstanceFlyBase>();
            int x = nodeData.x + 2;
            int y = nodeData.y - 2;
            for (int i = 0; i < datas.Count; ++i)
            {
                GameObject tempObj = null;
                int id = 0;
                int.TryParse(datas[i].id, out id);
                if (id / 1000 == 3)//装备箱定义
                {
                    tempObj = LTInstanceOptimizeManager.Instance.GetFlyItem("FlyEquipBox");
                }
                else
                {
                    tempObj = LTInstanceOptimizeManager.Instance.GetFlyItem();
                }

                var temp = MapCtrl.GetNodeObjByPos(x, y);
                GameObject roleItemObj = GameObject.Instantiate(tempObj, temp.mDMono.transform);
                LTInstanceFlyBase flyItem = roleItemObj.GetMonoILRComponent<LTInstanceFlyBase>();
                flyItem.Show(datas[i]);//flyPos
                flyList.Add(flyItem);
                y++;
                if (y > nodeData.y + 2)
                {
                    y = nodeData.y - 2;
                    x--;
                }
                yield return new WaitForSeconds(0.23f);
            }
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < flyList.Count; ++i)
            {
                flyList[i].Fly(flyPos);
            }
            yield return new WaitForSeconds(1f);
            int count = flyList.Count;
            for (int i = count - 1; i >= 0; --i)
            {
                LTInstanceFlyBase obj = flyList[i];
                flyList.RemoveAt(i);
                if (obj != null) GameObject.Destroy(obj.mDMono.gameObject);
            }
            var ht = Johny.HashtablePool.Claim();
            ht["reward"] = datas;
            ht["callback"] = null;
            GlobalMenuManager.Instance.Open("LTShowBoxView", ht);
            InputBlockerManager.Instance.UnBlock(InputBlockReason.FUSION_BLOCK_UI_INTERACTION);
        }
        #endregion

    }

    public class LTInstanceWallTemp : UI.LTInstanceNodeTemp
    {
        private SpriteRenderer Terra;

        public SpriteRenderer Decoration;

        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.transform;
            Terra = t.GetComponent<SpriteRenderer>("Terra");

            Decoration = t.GetComponent<SpriteRenderer>("Decoration");
        }

        private Johny.Action.ActionCellBornMove act1;
        private Johny.Action.ActionAlphaChange act2;
        private Johny.Action.ActionAlphaChange act3;
        public override void OnDestroy()
        {
            act1.Stop();
            act2.Stop();
            if (act3 != null) act3.Stop();
            base.OnDestroy();
        }

        public override void SetData(int num, UI.LTInstanceNode data)
        {
            base.SetData(num, data);
            //Born Move
            {
                var bornPos = new Vector3(0, -1f, 0);
                var middlePos = bornPos + new Vector3(0, 1.3f, 0);
                var finalPos = middlePos + new Vector3(0, -0.3f, 0);
                float delay = LTInstanceConfigManager.GetFloatValue("CellBornMove_Delay", 0.0f);
                float during = LTInstanceConfigManager.GetFloatValue("CellBornMove_During", 0.5f);
                act1 = new Johny.Action.ActionCellBornMove(delay, during, Terra.gameObject, bornPos, middlePos, finalPos);
            }

            //Born FadeIn
            {
                float delay = LTInstanceConfigManager.GetFloatValue("CellBornFadeIn_Delay", 0.0f);
                float during = LTInstanceConfigManager.GetFloatValue("CellBornFadeIn_During", 0.45f);
                float from = LTInstanceConfigManager.GetFloatValue("CellBornFadeIn_From", 0.0f);
                float to = LTInstanceConfigManager.GetFloatValue("CellBornFadeIn_To", 1.0f);
                act2 = new Johny.Action.ActionAlphaChange(delay, during, Terra, from, to);
                act2.SetFinishHandler(delegate (Johny.Action.ActionAlphaChange.FinishStatus e) {
                    if (e == Johny.Action.ActionAlphaChange.FinishStatus.Complete)
                    {
                        if (mDMono != null)
                        {
                            if (Decoration.sprite != null)
                            {
                                act3 = new Johny.Action.ActionAlphaChange(delay, during, Decoration, from, to);
                                Decoration.gameObject.CustomSetActive(true);
                            }

                            if (nodeData.RoleData.IsCorrelation)
                            {
                                if (nodeData.IsLeftCorrelation())
                                {
                                    var nearNode = nodeData.GetNeightbourNodeByDir(UI.LTInstanceNode.DirType.Left);
                                    if (MapCtrl.GetNodeObjByPos(nearNode.x, nearNode.y) == null)
                                    {
                                        MapCtrl.MapDataUpdateFunc_EnqueueNode(nearNode, false);
                                    }
                                }
                                if (nodeData.IsRightCorrelation())
                                {
                                    var nearNode = nodeData.GetNeightbourNodeByDir(UI.LTInstanceNode.DirType.Right);
                                    if (MapCtrl.GetNodeObjByPos(nearNode.x, nearNode.y) == null)
                                    {
                                        MapCtrl.MapDataUpdateFunc_EnqueueNode(nearNode, false);
                                    }
                                }
                                if (nodeData.IsUpCorrelation())
                                {
                                    var nearNode = nodeData.GetNeightbourNodeByDir(UI.LTInstanceNode.DirType.UP);
                                    if (MapCtrl.GetNodeObjByPos(nearNode.x, nearNode.y) == null)
                                    {
                                        MapCtrl.MapDataUpdateFunc_EnqueueNode(nearNode, false);
                                    }
                                }
                                if (nodeData.IsDownCorrelation())
                                {
                                    var nearNode = nodeData.GetNeightbourNodeByDir(UI.LTInstanceNode.DirType.Down);
                                    if (MapCtrl.GetNodeObjByPos(nearNode.x, nearNode.y) == null)
                                    {
                                        MapCtrl.MapDataUpdateFunc_EnqueueNode(nearNode, false);
                                    }
                                }
                            }

                            if (nodeData.IsUpWall())
                            {
                                var nearNode = nodeData.GetNeightbourNodeByDir(UI.LTInstanceNode.DirType.UP);
                                if (nearNode.IsCanShowWell() && MapCtrl.GetNodeObjByPos(nearNode.x, nearNode.y) == null)
                                {
                                    MapCtrl.MapDataUpdateFunc_EnqueueNode(nearNode,false);
                                }
                            }
                            if (nodeData.IsDownWall())
                            {
                                var nearNode = nodeData.GetNeightbourNodeByDir(UI.LTInstanceNode.DirType.Down);
                                if (nearNode.IsCanShowWell() && MapCtrl.GetNodeObjByPos(nearNode.x, nearNode.y) == null)
                                {
                                    MapCtrl.MapDataUpdateFunc_EnqueueNode(nearNode, false);
                                }
                            }
                        }
                    }
                });
            }

            AwakeTerra();
            AwakeRole();
        }

        public override void UpdateData(UI.LTInstanceNode data)
        {
            base.UpdateData(data);
        }

        private void AwakeTerra()
        {
            string name = string.Empty;
            if (nodeData.IsShowHightWell() && UI.LTInstanceMapModel.Instance.IsSimulateWell())
            {
                name = UI.LTInstanceMapModel.Instance.GetSimulateWellName();
            }
            if (string.IsNullOrEmpty(name))
            {
                name = nodeData.Img;
            }
            Terra.sprite = LTInstanceSpriteTool.GetMapStyleSprite(name);
            LTInstanceSpriteTool.SetTerraEffect(nodeData.Img, Terra.transform);
        }

        protected void AwakeRole()
        {
            if (nodeData.RoleData.Id > 0 && nodeData.RoleData.Type == 1)
            {
                CreateDecorationItem();
                return;
            }
        }

        private void CreateDecorationItem()
        {
            Decoration.sprite = LTInstanceSpriteTool.GetMapStyleSprite(nodeData.RoleData.Img);
            //位置配置
            Decoration.transform.localEulerAngles = nodeData.RoleData.Rotation;
            Decoration.transform.localScale = nodeData.RoleData.Span ;
            Vector3 offset = nodeData.RoleData.Offset ;
            Decoration.transform.localPosition += offset;
            bool changeOrder = false;
            if(offset.z > 0)
            {
                Decoration.sortingOrder -= 1;
                changeOrder = true;
            }
            LTInstanceSpriteTool.SetDecorationEffect(nodeData.RoleData.Img, Decoration.transform, changeOrder);
        }
        
        public override void OnFloorClick()
        {
            if (UI.LTInstanceMapModel .Instance.IsMonopolyEntering()) return;
            Instance.LTInstanceOptimizeManager.Instance.ShowSelectObj(false, mDMono.transform);
        }
    }

    public class LTInstanceRoleBase:DynamicMonoHotfix
    {
        public SpriteRenderer RoleSprite;

        public bool LimitOpen;

        protected string imgName;

        protected UI.LTInstanceNode nodeData;
        
        public override void Awake()
        {
            base.Awake();
            LimitOpen = mDMono.BoolParamList[0];
            RoleSprite = mDMono.transform.GetComponent<SpriteRenderer>("Role");
        }

        public virtual void Init(UI.LTInstanceNode nodeData)
        {
            imgName = GetImgName(nodeData.RoleData.Img);
            this.nodeData = nodeData;
            RoleSprite.sprite = LTInstanceSpriteTool .GetRole(imgName);
            PlayAni();
        }
        
        public virtual void PlayAni()
        {

        }

        public virtual void SetOpenData()
        {
            if (!LimitOpen) RoleSprite.sprite = LTInstanceSpriteTool.GetRole(string.Format("{0}1", imgName));
        }

        private string GetImgName(string strName)
        {
            string[] str = strName.Split('#');
            return str[str.Length - 1];
        }
    }
    
    public class LTInstanceRoleShake : LTInstanceRoleBase
    {
        private Johny.Action.ActionCellUpAndDownLoop act3;
        public override void OnDestroy()
        {
            if (act3 != null) act3.Stop();
            base.OnDestroy();
        }

        public override void PlayAni()
        {
            float delay = LTInstanceConfigManager.GetFloatValue("ItemUpAndDown_Delay", 0.0f);
            float during = LTInstanceConfigManager.GetFloatValue("ItemUpAndDown_During", 1.0f);
            int minCnt = LTInstanceConfigManager.GetIntValue("ItemUpAndDown_MinCnt", 3);
            int maxCnt = LTInstanceConfigManager.GetIntValue("ItemUpAndDown_MaxCnt", 5);
            float minInter = LTInstanceConfigManager.GetFloatValue("ItemUpAndDown_MinInterval", 0.5f);
            float maxInter = LTInstanceConfigManager.GetFloatValue("ItemUpAndDown_MaxInterval", 1.5f);
            act3 = new Johny.Action.ActionCellUpAndDownLoop(delay, during, RoleSprite.gameObject, new Vector3(0, 0.6f, 0));
            act3.SetLoopInterval(minCnt, maxCnt, minInter, maxInter);
        }
    }

    public class LTInstanceRoleSkill : LTInstanceRoleShake
    {
        public SpriteRenderer OtherSprite;
        private int skillId;
        public override void Awake()
        {
            base.Awake();
            LimitOpen = mDMono.BoolParamList[0];
            OtherSprite = mDMono.transform.GetComponent<SpriteRenderer>("Role/Skill");
        }

        public override void Init(UI.LTInstanceNode nodeData)
        {
            base.Init(nodeData);
            skillId = nodeData.RoleData.CampaignData.SkillId;
            Hotfix_LT.Data.SkillTemplate skillTpl = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(skillId);
            if (skillTpl != null)
            {
                OtherSprite.sprite =LTInstanceSpriteTool .GetRole (skillTpl.Icon);
            }
        }
    }

    public class LTInstanceRoleXianJing: LTInstanceRoleBase
    {
        public SpriteRenderer OtherSprite;
        private Johny.Action.ActionAlphaChange act3;

        public override void Awake()
        {
            base.Awake();
            OtherSprite = mDMono.transform.GetComponent<SpriteRenderer>("Role/Guang");
        }

        public override void Init(UI.LTInstanceNode nodeData)
        {
            base.Init(nodeData);
            OtherSprite.sprite = LTInstanceSpriteTool.GetRole(string .Format("{0}_Guang", imgName));
        }

        public override void OnDestroy()
        {
            if (act3 != null) act3.Stop();
            base.OnDestroy();
        }

        public override void PlayAni()
        {
            float delay = LTInstanceConfigManager.GetFloatValue("Xianjing_AlphaChange_Delay", 0);
            float during = LTInstanceConfigManager.GetFloatValue("Xianjing_AlphaChange_During", 0.5f);
            float from = LTInstanceConfigManager.GetFloatValue("Xianjing_AlphaChange_From", 0);
            float to = LTInstanceConfigManager.GetFloatValue("Xianjing_AlphaChange_To", 1);
            act3 = new Johny.Action.ActionAlphaChange(delay, during, OtherSprite, from, to);
            act3.ForwardAndReverseLoop = true;
        }
    }

    public class LTInstanceRoleDoor : LTInstanceRoleBase
    {
        private GameObject FX;
        public override void Awake()
        {
            base.Awake();
            FX = mDMono.transform.Find("FX").gameObject;
        }

        public override void SetOpenData()
        {
            FX.CustomSetActive(true);
        }
    }

    public class LTInstanceRoleDoor2 : LTInstanceRoleBase
    {
        private GameObject FX;
        public override void Awake()
        {
            base.Awake();
            FX = mDMono.transform.Find("FX").gameObject;
        }

        public override void PlayAni()
        {
            FX.CustomSetActive(true);
        }
    }

    public class LTInstanceFlyBase : DynamicMonoHotfix
    {
        public SpriteRenderer RoleSprite;
        public Transform FxRoot;
        protected Johny.Action.ActionCellBornMove act1;
        protected Johny.Action.ActionCellUpAndDownLoop act2;
        public override void Awake()
        {
            base.Awake();
            RoleSprite = mDMono.transform.GetComponent<SpriteRenderer>("Root/Role");
            FxRoot = mDMono.transform.Find("Root/FX");
        }

        public override void OnDestroy()
        {
            if(act1!=null) act1.Stop();
            if (act2 != null) act2.Stop();
            base.OnDestroy();
        }

        public virtual void PlayAni()
        {
            float delay = LTInstanceConfigManager.GetFloatValue("ItemUpAndDown_Delay", 0.0f);
            float during = LTInstanceConfigManager.GetFloatValue("ItemUpAndDown_During", 1.0f);
            int minCnt = LTInstanceConfigManager.GetIntValue("ItemUpAndDown_MinCnt", 3);
            int maxCnt = LTInstanceConfigManager.GetIntValue("ItemUpAndDown_MaxCnt", 5);
            float minInter = LTInstanceConfigManager.GetFloatValue("ItemUpAndDown_MinInterval", 0.5f);
            float maxInter = LTInstanceConfigManager.GetFloatValue("ItemUpAndDown_MaxInterval", 1.5f);
            act2 = new Johny.Action.ActionCellUpAndDownLoop(delay, during, RoleSprite.gameObject, new Vector3(0, 0.6f, 0));
            act2.SetLoopInterval(minCnt, maxCnt, minInter, maxInter);
        }
        
        protected virtual bool HasFXLimit() { return true; }

        public virtual void Show(UI.LTShowItemData data)
        {
            string name = LTInstanceSpriteTool.GetFlyItemSpriteName(data);
            RoleSprite.sprite = LTInstanceSpriteTool.GetRole(name);

            LTIconNameQuality itemInfo = LTItemInfoTool.GetInfo(data.id, data.type, data.coloring);

            PlayAni();

            if (HasFXLimit())
            {
                int quality = 0;
                int.TryParse(itemInfo.quality,out quality);
                float limit= Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("ChallengeBossRewardLimit");
                if (limit >= quality) return;
            }

            string fxName = "fx_fb_wuping_lv";
            switch (itemInfo.quality)
            {
                case Player.EconomyConstants.Quality.POOR:
                    fxName= "fx_fb_wuping_lv"; break;
                case Player.EconomyConstants.Quality.COMMON:
                    fxName = "fx_fb_wuping_lv"; break;
                case Player.EconomyConstants.Quality.UNCOMMON:
                    fxName = "fx_fb_wuping_lan"; break;
                case Player.EconomyConstants.Quality.EPIC:
                    fxName = "fx_fb_wuping_zi"; break;
                case Player.EconomyConstants.Quality.LEGENDARY:
                    fxName = "fx_fb_wuping_cheng"; break;
                case Player.EconomyConstants.Quality.HALLOWS:
                    fxName = "fx_fb_wuping_hong"; break;
                default: fxName = "fx_fb_wuping_hong"; break;
            }

            EB.Assets.LoadAsyncAndInit<GameObject>(fxName, (assetName, o, succ) =>
            {
                if (succ)
                {
                    o.transform.localPosition = Vector3.zero;
                }
            }, FxRoot, FxRoot.gameObject, true);
        }

        public virtual void Fly(Vector3 flyPos)
        {
            {
                act1 = new Johny.Action.ActionCellBornMove(Random .Range(0,0.6f), 0.5f, mDMono.gameObject, mDMono.transform .position, flyPos, flyPos, false);
            }
        }
    }

    public class LTInstanceFlyEquipBox : LTInstanceFlyBase
    {
        public SpriteRenderer EquipSprite;

        protected override bool HasFXLimit() { return false; }

        public override void Awake()
        {
            EquipSprite = mDMono.transform.GetComponent<SpriteRenderer>("Root/Role/Equip");
            base.Awake();
        }

        public override void Show(LTShowItemData data)
        {
            base.Show(data);
            int id = int.Parse(data.id);
            EquipSprite.sprite  = LTInstanceSpriteTool.GetRole(string.Format ("Equip_Suit_{0}", id % 100));
        }

        public override void PlayAni()
        {
            float delay = LTInstanceConfigManager.GetFloatValue("ItemUpAndDown_Delay", 0.0f);
            float during = LTInstanceConfigManager.GetFloatValue("ItemUpAndDown_During", 1.0f);
            int minCnt = LTInstanceConfigManager.GetIntValue("ItemUpAndDown_MinCnt", 3);
            int maxCnt = LTInstanceConfigManager.GetIntValue("ItemUpAndDown_MaxCnt", 5);
            float minInter = LTInstanceConfigManager.GetFloatValue("ItemUpAndDown_MinInterval", 0.5f);
            float maxInter = LTInstanceConfigManager.GetFloatValue("ItemUpAndDown_MaxInterval", 1.5f);
            act2 = new Johny.Action.ActionCellUpAndDownLoop(delay, during, EquipSprite.gameObject, new Vector3(0, 0.78f, 0));
            act2.SetLoopInterval(minCnt, maxCnt, minInter, maxInter);
        }
    }
}