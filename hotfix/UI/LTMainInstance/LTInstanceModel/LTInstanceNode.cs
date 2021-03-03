using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTInstanceNodeCampaignData
    {
        public int CampaignId = 0;
        public int Star = 0;
        public string Password = string.Empty;
        public bool IsDoorOpen = false;
        public int Bomb = 0;
        public int SkillId = 0;
        public string Layout = null;
        public bool ControlNearby = false;
        public int Kill = 0;
        public List<LTShowItemData> Bonus = new List<LTShowItemData>();

        public LTInstanceNodeCampaignData DeepCopy(){
            var cd = new LTInstanceNodeCampaignData();
            cd.CampaignId = CampaignId;
            cd.Star = Star;
            cd.Password = Password;
            cd.IsDoorOpen = IsDoorOpen;
            cd.Bomb = Bomb;
            cd.SkillId = SkillId;
            cd.Layout = Layout;
            cd.ControlNearby = ControlNearby;
            cd.Kill = Kill;
            cd.Bonus = Bonus;
            return cd;
        }
    }

    public class LTInstanceNodeRoleData
    {
        public int Id = 0;
        public string Img = string.Empty;
        public bool IsDynImg = false;
        public string Model = string.Empty;
        public string OtherModel = string.Empty;
        public bool IsElite = false;
        public float ModelScale = 1;
        public Vector3 Rotation = Vector3.zero;
        public Vector3 Span = Vector3.one;
        public Vector3 Offset = Vector3.zero;
        public bool IsCorrelation = false;
        public string Order = string.Empty;
        public int Type = 0;
        public List<string> Param = new List<string>();
        public LTInstanceNodeCampaignData CampaignData = new LTInstanceNodeCampaignData();

        public LTInstanceNodeRoleData DeepCopy(){
            var rd = new LTInstanceNodeRoleData();
            rd.Id = Id;
            rd.Img = Img;
            rd.IsDynImg = IsDynImg;
            rd.Model = Model;
            rd.OtherModel = OtherModel;
            rd.IsElite = IsElite;
            rd.ModelScale = ModelScale;
            rd.Rotation = Rotation;
            rd.Span = Span;
            rd.Offset = Offset;
            rd.Order = Order;
            rd.Type = Type;
            rd.Param = Param;
            rd.CampaignData = CampaignData.DeepCopy();
            rd.IsCorrelation = IsCorrelation;
            return rd;
        }
    }

    public class LTInstanceNode
    {
        public enum DirType
        {
            NONE = 0,
            UP = 8,
            Down = 2,
            Right = 6,
            Left = 4,
        }
        public enum NodeType
        {
            WALL = 0,
            Floor = 1,
        }

        #region terra data
        /// <summary>行 </summary>
        public int y;
        /// <summary>列</summary>
        public int x;
        public int OriginTerra;
        public NodeType Type;//格子类型，墙还是地板
        public bool IsSight = false;//是否可见的
        public bool IsControllered = false;//是否被拦截无法通行的
        public bool CanPass;//是否可通行的
        public string Img;
        public Vector3 Rotation;
        public string Layout = string.Empty;
        public int HireId = 0;
        public Hashtable HireCost = null;
        public List<LTShowItemData> WheelData = null;
        public int WheelCount = 0;

        public bool isSimulate = false;//前端模拟修改的数据
                                       //public bool isBossArea = false;
        #endregion

        #region role data
        public object AddData{get;set;}
        public LTInstanceNodeRoleData RoleData{get;set;}
        #endregion

        public LTInstanceNode(int x, int y, int originTerra, bool sight, bool controllered, Hotfix_LT.Data.LostChallengeChapterElement terraDataTemplate)
        {
            this.x = x;
            this.y = y;
            OriginTerra = originTerra > 0 ? originTerra : terraDataTemplate.Id;
            IsSight = sight;
            IsControllered = controllered;

            Img = terraDataTemplate.Img;
            Rotation = terraDataTemplate.Rotation;
            CanPass = terraDataTemplate.CanPass;
            Type = terraDataTemplate.CanPass ? NodeType.Floor : NodeType.WALL;
            RoleData = new LTInstanceNodeRoleData();
        }

        public LTInstanceNode(int x, int y, int originTerra, bool sight, bool controllered, 
                                string img, Vector3 rotaion, bool canpass, NodeType type, 
                                LTInstanceNodeRoleData rd){
            this.x = x;
            this.y = y;
            OriginTerra = originTerra;
            IsSight = sight;
            IsControllered = controllered;

            Img = img;
            Rotation = rotaion;
            CanPass = canpass;
            Type = type;
            RoleData = rd.DeepCopy();
        }
   
        public LTInstanceNode DeepCopy(){
            return new LTInstanceNode(x,y,OriginTerra, IsSight, IsControllered, Img, Rotation, CanPass, Type, RoleData);
        }

        public override int GetHashCode(){
            return y * 100 + x;
        }

        private LTInstanceNode UpNode;
        private LTInstanceNode DownNode;
        private LTInstanceNode LeftNode;
        private LTInstanceNode RightNode;

        public LTInstanceNode GetNeightbourNodeByDir(DirType dir)
        {
            switch (dir)
            {
                case DirType.UP: return UpNode;
                case DirType.Down: return DownNode;
                case DirType.Left: return LeftNode;
                case DirType.Right: return RightNode;
                default:return null;
            }
        }

        public List<int> GetNeighbourhoodFloorHash()
        {
            List<int> list = new List<int>();
            if (UpNode != null && UpNode.Type == NodeType.Floor) list.Add(UpNode.GetHashCode());
            if (DownNode != null && DownNode.Type == NodeType.Floor) list.Add(DownNode.GetHashCode());
            if (LeftNode != null && LeftNode.Type == NodeType.Floor) list.Add(LeftNode.GetHashCode());
            if (RightNode != null && RightNode.Type == NodeType.Floor) list.Add(RightNode.GetHashCode());
            return list;
        }

        public bool IsShowHightWell()
        {
            return (RightNode==null|| UpNode==null);
        }

        /// <summary>
        /// 获取邻近能走的格子
        /// </summary>
        /// <returns></returns>
        public List<LTInstanceNode> GetNeighbourhood()
        {
            List<LTInstanceNode> list = new List<LTInstanceNode>();
            if (UpNode != null) list.Add(UpNode);
            if (DownNode != null) list.Add(DownNode);
            if (LeftNode != null) list.Add(LeftNode);
            if (RightNode != null) list.Add(RightNode);
            return list;
        }

        public void SetUp(LTInstanceNode node)
        {
            UpNode = node;
        }

        public void SetDown(LTInstanceNode node)
        {
            DownNode = node;
        }

        public void SetLeft(LTInstanceNode node)
        {
            LeftNode = node;
        }

        public void SetRight(LTInstanceNode node)
        {
            RightNode = node;
        }

        //是否为相同点
        public bool IsSameNodePos(LTInstanceNode node)
        {
            if (node == null)
            {
                return false;
            }

            if (this.x == node.x && this.y == node.y)
            {
                return true;
            }
            return false;
        }

        public bool IsNeighbourPos(LTInstanceNode node)
        {
            if ((Math.Abs(this.x - node.x) == 1 && this.y == node.y) ||
                Math.Abs(this.y - node.y) == 1 && this.x == node.x)
            {
                return true;
            }
            return false;
        }

        public bool IsNearSightFloor()
        {
            if (LeftNode != null && LeftNode.Type == NodeType.Floor && LeftNode.IsSight ||
                RightNode != null && RightNode.Type == NodeType.Floor && RightNode.IsSight ||
                UpNode != null && UpNode.Type == NodeType.Floor && UpNode.IsSight ||
                DownNode != null && DownNode.Type == NodeType.Floor && DownNode.IsSight )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsCanShowWell()
        {
            bool canShow = false;
            if (LeftNode != null)
            {
                if (LeftNode.Type == NodeType.Floor && LeftNode.IsSight) return true;
                canShow = LeftNode.Type == NodeType.WALL;
            }
            else
            {
                canShow = true;
            }
            if (canShow)
            {
                if(RightNode != null)
                {
                    if (RightNode.Type == NodeType.Floor && RightNode.IsSight || RightNode.Type == NodeType.WALL) return true;
                    else return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsDirFloor(DirType dir)
        {
            if (dir == DirType.UP)
            {
                return UpNode != null && UpNode.Type == NodeType.Floor;
            }
            else if (dir == DirType.Down)
            {
                return DownNode != null && DownNode.Type == NodeType.Floor;
            }
            else if (dir == DirType.Left)
            {
                return LeftNode != null && LeftNode.Type == NodeType.Floor;
            }
            else if (dir == DirType.Right)
            {
                return RightNode != null && RightNode.Type == NodeType.Floor;
            }
            return false;
        }

        public bool IsDirWall(DirType dir)
        {
            if (dir == DirType.UP)
            {
                return UpNode != null && UpNode.Type == NodeType.WALL;
            }
            else if (dir == DirType.Down)
            {
                return DownNode != null && DownNode.Type == NodeType.WALL;
            }
            else if (dir == DirType.Left)
            {
                return LeftNode != null && LeftNode.Type == NodeType.WALL;
            }
            else if (dir == DirType.Right)
            {
                return RightNode != null && RightNode.Type == NodeType.WALL;
            }
            return false;
        }

        public bool IsLeftWall()
        {
            return LeftNode != null && LeftNode.Type == NodeType.WALL;
        }

        public bool IsRightWall()
        {
            return RightNode != null && RightNode.Type == NodeType.WALL;
        }

        public bool IsUpWall()
        {
            return UpNode != null && UpNode.Type == NodeType.WALL;
        }

        public bool IsDownWall()
        {
            return DownNode != null && DownNode.Type == NodeType.WALL;
        }
        public bool IsLeftCorrelation()
        {
            return LeftNode != null && LeftNode.RoleData.IsCorrelation;
        }

        public bool IsRightCorrelation()
        {
            return RightNode != null && RightNode.RoleData.IsCorrelation;
        }

        public bool IsUpCorrelation()
        {
            return UpNode != null && UpNode.RoleData.IsCorrelation;
        }

        public bool IsDownCorrelation()
        {
            return DownNode != null && DownNode.RoleData.IsCorrelation;
        }
        /// <summary>
        /// 通过坐标获取朝向
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public DirType GetDirByPos(int x, int y)
        {
            DirType dir = DirType.NONE;
            if (UpNode != null && UpNode.x == x && UpNode.y == y)
            {
                dir = DirType.UP;
            }
            else if (DownNode != null && DownNode.x == x && DownNode.y == y)
            {
                dir = DirType.Down;
            }
            else if (LeftNode != null && LeftNode.x == x && LeftNode.y == y)
            {
                dir = DirType.Left;
            }
            else if (RightNode != null && RightNode.x == x && RightNode.y == y)
            {
                dir = DirType.Right;
            }
            return dir;
        }

        public DirType GetProbablyDirByPos(int x, int y, bool yPrior = false)
        {
            if (yPrior)
            {
                if (this.y > y) return DirType.UP;
                if (this.y < y) return DirType.Down;
            }
            if (this.x < x) return DirType.Right;
            if (this.x > x) return DirType.Left;
            if (!yPrior)
            {
                if (this.y > y) return DirType.UP;
                if (this.y < y) return DirType.Down;
            }
            return DirType.NONE;
        }

        public LTInstanceNode GetNodeByDir(int dir)
        {
            switch (dir)
            {
                case (int)DirType.UP:
                    return UpNode;

                case (int)DirType.Down:
                    return DownNode;

                case (int)DirType.Left:
                    return LeftNode;

                case (int)DirType.Right:
                    return RightNode;
            }
            return null;
        }

        /// <summary>
        /// 重写输出格式
        /// </summary>
        public override string ToString()
        {
            return string.Format("[{0},{1}]Type:{2}", x, y, Type);
        }

        #region PathFinding

        /// <summary>寻路保存的节点</summary>
        public LTInstanceNode Parent;

        public int gCost;

        public int hCost;

        public int fCost
        {
            get
            {
                return gCost + hCost;
            }
        }

        public bool HasMonster()
        {
            return false;
        }

        #endregion

        #region 前端翻格子逻辑
        public bool Open(int terra, out bool isModel)
        {
            isModel = false;
            if (!IsSight && CanPass && !IsControllered)
            {
                isSimulate = true;

                IsSight = true;
                var terraDataTemplate = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostChallengeChapterElement(terra);
                Img = terraDataTemplate.Img;
                Rotation = terraDataTemplate.Rotation;
                CanPass = terraDataTemplate.CanPass;
                if (!string.IsNullOrEmpty(RoleData.Model))
                {
                    isModel = true;
                }
                return true;
            }
            return false;
        }

        public void Controller()
        {
            if (!IsSight && CanPass && !IsControllered)
            {
                IsControllered = true;
            }
        }

        //public void BossArea()
        //{
        //    if (!IsSight && CanPass && !IsControllered)
        //    {
        //        isBossArea = true;
        //    }
        //}

        #endregion
    }

    public class LTInstanceEvent
    {
        //主线与挑战副本
        public const string EVENT_TYPE_OPEN_BOX = "OpenBox";//Main|Challenge 触发宝箱
        public const string EVENT_TYPE_HIDDEN = "HiddenPath";//Main|Challenge 密道事件
        public const string EVENT_TYPE_DOOR_OPEN = "DoorOpen";//Main|Challenge 开门，开机关

        //主线副本
        public const string EVENT_TYPE_DIALOG = "Dialog";//Main 触发对话
        public const string EVENT_TYPE_MOVIE = "Movie";//Main 剧情
        public const string EVENT_TYPE_PRAY_POINT = "PrayPoint";//Main 神灯事件
        public const string EVENT_TYPE_PRAYPOINTFULL = "PrayPointFull";//Main 神灯已满事件
        public const string EVENT_TYPE_PASSWORD = "Password";//Main 显示密码
        public const string EVENT_TYPE_HERO = "Hero";//Main 主线副本获得英雄
        public const string EVENT_TYPE_MAIN_CAMP_OVER = "MainCampOver";//Main 主线副本章节通关，返回大地图
        public const string EVENT_TYPE_MAIN_CAMP_OVER_NORETURN = "MainCampOverNoReturn";//Main 主线副本章节通关，不返回大地图

        //挑战副本
        public const string EVENT_TYPE_HEAL_TRIGGER = "HealTrigger";//Challenge 触发治疗
        public const string EVENT_TYPE_TRAP_TRIGGER = "TrapTrigger";//Challenge 触发陷阱
        public const string EVENT_TYPE_EXIT_POINT = "ExitPoint";//Challenge 挑战副本传送门
        public const string EVENT_TYPE_CHALLENGE_FAIL = "Fail";//Challenge 挑战副本失败
        public const string EVENT_TYPE_MANA_REGEN = "ManaRegen";//Challenge 获得挑战副本魔法药水
        public const string EVENT_TYPE_DICE = "Dice";//Challenge 显示挑战副本骰子
        public const string EVENT_TYPE_SHOP = "Shop";//Challenge 打开挑战副本商店
        public const string EVENT_TYPE_SHOPREFRESH = "ShopRefresh";//Challenge 商店刷新事件
        public const string EVENT_TYPE_GETSCROLL = "GetScroll";//Challenge获得技能卷轴
        public const string EVENT_TYPE_MANA_REGEN_FACTOR = "ManaRegenFactor";//Challenge 魔法泉水
        public const string EVENT_TYPE_MANA_TRAP_TRIGGER = "ManaTrapTrigger";//Challenge 魔法陷阱
        public const string EVENT_TYPE_HUNTERMARK = "HunterMark";//Challenge 拾取猎人标记事件
        public const string EVENT_TYPE_EXITLOCKED = "ExitLocked";//Challenge 点击被封印的传送门事件
        public const string EVENT_TYPE_EXITLOCKED2 = "ExitLocked2";//Challenge 点击传送门未解锁钥匙事件
        public const string EVENT_TYPE_GUIDE = "Guide";//Challenge 新元素事件
        public const string EVENT_TYPE_LUCK = "Lucky";//Challenge 幸运骰子
        public const string EVENT_TYPE_WHEEL = "Wheel";//Challenge 转盘
                                                       //需前端提前处理的事件，服务器只做校对
        public const string EVENT_TYPE_DAMAGE = "DamageEvt";//Challenge 瘟疫
        public const string EVENT_TYPE_HEAL = "HealEvt";//Challenge 生机
        public const string EVENT_TYPE_BOMB = "Bomb";//Challenge 炸弹怪物爆炸

        public string Type;
        public int x;
        public int y;
        public object Param;//宝箱凋落物

        public LTInstanceEvent(string type, int x, int y)
        {
            this.Type = type;
            this.x = x;
            this.y = y;
        }

        public bool HasHPInfoData;
    }
}