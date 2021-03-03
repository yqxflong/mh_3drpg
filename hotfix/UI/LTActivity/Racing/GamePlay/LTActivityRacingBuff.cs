//LTActivityRacingBuff
//赛跑中的buff
//Johny

using UnityEngine;

namespace Hotfix_LT.UI
{
    public class BaseActivityRacingBuff
    {
        public enum BuffTYPE
        {
            tNone = 0,
            tConfused = 1,
            tTired = 2,
            tNormal = 3,
            tFast = 4,
            tDash = 5,
            tLocalReady = 100,
            tLocalVictory = 101,
            tLocalLose = 102,
        }

        #region getter
        public int Order{get;private set;}
        public BuffTYPE BuffType{get; private set;}

        public float During{get; protected set;} = 1.0f;

        public float MovePercent
        {
            get
            {
                return _normalizedMove / 60.0f;
            }
        }

        public MoveController.CombatantMoveState AniState{get; protected set;}
        #endregion

        protected float _normalizedMove;

        protected Johny.Action.ActionModelMove _actionMove;

        protected ActivityRacingPlayer _player;

        protected GameObject _holder;

        protected MoveController _mc;

        protected Animator _animator;

        protected string _broadcastText;

        public static BaseActivityRacingBuff CreateBuff(ActivityRacingPlayer player, int order, int type, float move)
        {
            BaseActivityRacingBuff buff = null;
            var buffType = (BuffTYPE)type;
            switch(buffType)
            {
                case BuffTYPE.tConfused:
                    buff = new ActivityRacingBuffConfused(player.pd, order, move);
                    buff.SetRacingPlayer(player);
                    return buff;
                case BuffTYPE.tTired:
                    buff = new ActivityRacingBuffTired(player.pd, order, move);
                    buff.SetRacingPlayer(player);
                    return buff;
                case BuffTYPE.tNormal:
                    buff = new ActivityRacingBuffNormal(player.pd, order, move);
                    buff.SetRacingPlayer(player);
                    return buff;
                case BuffTYPE.tFast:
                    buff = new ActivityRacingBuffFast(player.pd, order, move);
                    buff.SetRacingPlayer(player);
                    return buff;
                case BuffTYPE.tDash:
                    buff = new ActivityRacingBuffDash(player.pd, order, move);
                    buff.SetRacingPlayer(player);
                    return buff;
                case BuffTYPE.tLocalLose:
                    buff = new ActivityRacingBuffLose(player.pd, order);
                    buff.SetRacingPlayer(player);
                    return buff;
                case BuffTYPE.tLocalVictory:
                    buff = new ActivityRacingBuffVictory(player.pd, order);
                    buff.SetRacingPlayer(player);
                    return buff;
            }

            EB.Debug.LogError("[Racing]CreateBuff===>order: {0}, type: {1}", order, type);
            return buff;
        }

        public BaseActivityRacingBuff(int order, BuffTYPE buffid, float move)
        {
            Order = order;
            BuffType = buffid;
            _normalizedMove = move;
        }

        public void SetRacingPlayer(ActivityRacingPlayer player)
        {
            _player = player;
        }

        public void SetHolder(GameObject holder, MoveController mc)
        {
            _holder = holder;
            _animator = mc.gameObject.GetComponent<Animator>();
            _mc = mc;
        }

        public void Destroy()
        {
            _actionMove?.Stop();
            _actionMove = null;
        }

        public virtual void Start(Vector3 from, Vector3 to)
        {
            _actionMove = new Johny.Action.ActionModelMove(0.0f, During, _holder, from, to);
            _mc.TransitionTo(AniState);
            if(_broadcastText != null)
            {
                _player.BroadCastInRacing(_broadcastText);
            }
        }

        //跳过保留位移
        public virtual void SkipKeepDis(Vector3 from, Vector3 to)
        {
            _holder.transform.localPosition = to;
        }

        public virtual bool Update(float dt)
        {
            During -= dt;
            return During <= 0.0f && _actionMove.IsFinished;
        }

        public virtual void ChangeSpeed(float speed)
        {
            //NONE
        }

        public virtual bool IsVictory()
        {
            return false;
        }
    }

    //迷茫 buff
    public class ActivityRacingBuffConfused : BaseActivityRacingBuff
    {
        private const float DURING = 1.0f;
        private MoveController.CombatantMoveState _curState;

        public ActivityRacingBuffConfused(RacingPlayerData pd, int order, float move)
        :base(order, BuffTYPE.tConfused, move)
        {
            AniState = MoveController.CombatantMoveState.kIdle;
            _curState = AniState;
            _broadcastText = EB.Localizer.Format("ID_RACING_BUFF_CONFUSED", pd.Num, pd.Name);
        }

        public override void SkipKeepDis(Vector3 from, Vector3 to)
        {
            //NONE
        }

        public override void ChangeSpeed(float speed)
        {
            During = DURING / speed;
            _animator.speed = speed;
        }

        public override bool Update(float dt)
        { 
            bool finished = During <= 0.0f;
            if(finished)
            {
                if(_curState == MoveController.CombatantMoveState.kHitReaction)
                {
                    _curState = MoveController.CombatantMoveState.kReady;
                    _mc.TransitionTo(_curState);
                }
                else if(_curState == MoveController.CombatantMoveState.kReady)
                {
                    _curState = MoveController.CombatantMoveState.kIdle;
                    _mc.TransitionTo(_curState);
                }
                else if(_curState == MoveController.CombatantMoveState.kIdle)
                {
                    _curState = MoveController.CombatantMoveState.kLocomotion;
                    _mc.TransitionTo(_curState);
                    return true;
                }
                return false;
            }
            else
            {
                if(_curState == MoveController.CombatantMoveState.kIdle)
                {
                    _curState = MoveController.CombatantMoveState.kReady;
                    _mc.TransitionTo(_curState);
                }
                else if(_curState == MoveController.CombatantMoveState.kReady)
                {
                    _curState = MoveController.CombatantMoveState.kHitReaction;
                    _mc.TransitionTo(_curState);
                }
                else
                {
                    During -= dt;
                }

                return false;
            }
        }
    }

    //疲惫 buff
    public class ActivityRacingBuffTired : BaseActivityRacingBuff
    {
        private const float DURING = 1.0f;

        public ActivityRacingBuffTired(RacingPlayerData pd, int order, float move)
        :base(order, BuffTYPE.tTired, move)
        {
            AniState = MoveController.CombatantMoveState.kLocomotion;
            _broadcastText = EB.Localizer.Format("ID_RACING_BUFF_TIRED", pd.Num, pd.Name);
        }

        public override void ChangeSpeed(float speed)
        {
            During = DURING / speed;
            _animator.speed = speed;
        }

        public override void SkipKeepDis(Vector3 from, Vector3 to)
        {
            //NONE
        }
    }

    //正常 buff
    public class ActivityRacingBuffNormal : BaseActivityRacingBuff
    {
        private const float DURING = 1.0f;

        public ActivityRacingBuffNormal(RacingPlayerData pd, int order, float move)
        :base(order, BuffTYPE.tNormal, move)
        {
            AniState = MoveController.CombatantMoveState.kLocomotion;
        }

        public override void ChangeSpeed(float speed)
        {
            During = DURING / speed;
            _animator.speed = speed;
        }
    }

    //快跑 buff
    public class ActivityRacingBuffFast : BaseActivityRacingBuff
    {
        private const float DURING = 1.0f;
        public ActivityRacingBuffFast(RacingPlayerData pd, int order, float move)
        :base(order, BuffTYPE.tFast, move)
        {
            AniState = MoveController.CombatantMoveState.kLocomotion;
            _broadcastText = EB.Localizer.Format("ID_RACING_BUFF_FAST", pd.Num, pd.Name);
        }

        public override void ChangeSpeed(float speed)
        {
            During = DURING / speed;
            _animator.speed = speed;
        }
    }

    //冲刺 buff
    public class ActivityRacingBuffDash : BaseActivityRacingBuff
    {
        private const float DURING = 1.0f;
        private MoveController.CombatantMoveState _curState;

        public ActivityRacingBuffDash(RacingPlayerData pd, int order, float move)
        :base(order, BuffTYPE.tDash, move)
        {
            AniState = MoveController.CombatantMoveState.kIdle;
            _curState = AniState;
            _broadcastText = EB.Localizer.Format("ID_RACING_BUFF_DASH", pd.Num, pd.Name);
        }

        public override void ChangeSpeed(float speed)
        {
            During = DURING / speed;
            _animator.speed = speed;
        }

        public override bool Update(float dt)
        {
            bool finished = During <= 0.0f && _actionMove.IsFinished;
            if(finished)
            {
                if(_curState == MoveController.CombatantMoveState.kForward)
                {
                    _curState = MoveController.CombatantMoveState.kReady;
                    _mc.TransitionTo(_curState);
                }
                else if(_curState == MoveController.CombatantMoveState.kReady)
                {
                    _curState = MoveController.CombatantMoveState.kIdle;
                    _mc.TransitionTo(_curState);
                }
                else if(_curState == MoveController.CombatantMoveState.kIdle)
                {
                     _curState = MoveController.CombatantMoveState.kLocomotion;
                    _mc.TransitionTo(_curState);
                    return true;
                }
                return false;
            }
            else
            {
                if(_curState == MoveController.CombatantMoveState.kIdle)
                {
                    _curState = MoveController.CombatantMoveState.kReady;
                    _mc.TransitionTo(_curState);
                }
                else if(_curState == MoveController.CombatantMoveState.kReady)
                {
                    _curState = MoveController.CombatantMoveState.kForward;
                    _mc.TransitionTo(_curState);
                }
                else
                {
                    During -= dt;
                    _mc.transform.localPosition = Vector3.zero;
                }
                return false;
            }
        }
    }

    //Victory buff
    public class ActivityRacingBuffVictory : BaseActivityRacingBuff
    {
        private MoveController.CombatantMoveState _curState;
        private const float IDLE_DURING = 5.0f;
        private float _idleDuring = 0.0f;

        public ActivityRacingBuffVictory(RacingPlayerData pd, int order)
        :base(order, BuffTYPE.tLocalVictory, 0)
        {
            _curState = MoveController.CombatantMoveState.kIdle;
            _broadcastText = EB.Localizer.Format("ID_RACING_FINISH", pd.Num, pd.Name, pd.Rank);
        }

        public override bool IsVictory()
        {
            return true;
        }

        public override void Start(Vector3 from, Vector3 to)
        {
            base.Start(from, to);
            FusionAudio.PostEvent("UI/81.FirstGoal");
        }

        public override void SkipKeepDis(Vector3 from, Vector3 to)
        {
            //NONE
        }

        public override void ChangeSpeed(float speed)
        {
            _animator.speed = speed;
        }

        public override bool Update(float dt)
        {
            if(_curState == MoveController.CombatantMoveState.kVictoryDance)
            {
                if(_mc.GetCurrentStateInfo().normalizedTime >= 1)
                {
                    _curState = MoveController.CombatantMoveState.kIdle;
                    _mc.TransitionTo(_curState);
                    _idleDuring = IDLE_DURING;
                }
            }
            else if(_curState == MoveController.CombatantMoveState.kIdle)
            {
                if(_idleDuring > 0.0f)
                {
                    _idleDuring -= dt;
                }
                if(_idleDuring <= 0.0f)
                {
                    _curState = MoveController.CombatantMoveState.kVictoryDance;
                    _mc.TransitionTo(_curState);
                }
            }

            return false;
        }
    }

    //Lose buff
    public class ActivityRacingBuffLose : BaseActivityRacingBuff
    {
        private const float DURING = 5.0f;

        public ActivityRacingBuffLose(RacingPlayerData pd, int order)
        :base(order, BuffTYPE.tLocalLose, 0)
        {
            During = DURING;
            AniState = MoveController.CombatantMoveState.kIdle;
            _broadcastText = EB.Localizer.Format("ID_RACING_FINISH", pd.Num, pd.Name, pd.Rank);
        }

        public override void SkipKeepDis(Vector3 from, Vector3 to)
        {
            //NONE
        }

        public override void ChangeSpeed(float speed)
        {
            During = DURING / speed;
        }

        public override bool Update(float dt)
        {
            During -= dt;
            return During <= 0.0f;
        }
    }
}