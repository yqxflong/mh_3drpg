using UnityEngine;
using System.Collections;
using MoveEditor;

/// <summary>
/// 特效播放的时候 此挂载对象是不动的 动的是effect对象
/// </summary>
public class ProjectileMono : MonoBehaviour
{
    /// <summary>
    /// 飞行时长
    /// </summary>
    public float flyTime;
    /// <summary>
    /// 特效消失延迟
    /// </summary>
    public float fadeOutTime;
    public AnimationCurve motionCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 0.0f);
    /// <summary>
    /// 根节点非ParticleSystem 但是所有ParticleSystem必须其中一个ParticleSystem节点之下
    /// </summary>
    public GameObject effectPrefab;
    public bool isTarget;
    public float speed; //速度 当没有目标时就使用速度
    public BodyPart bodyPart = BodyPart.Root; //初始化挂载部位
    public string spawnCustomPath;
    public Vector3 offset;
    public bool isLookAtTarget; //是否要foward朝向目标
    public delegate void ProjectileCallback();

    public ProjectileCallback finishCallBack;
    public ProjectileCallback hitCallBack;

    /// <summary>
    /// 不管目标有多少 仅初始化一个对第一个目标追踪 
    /// 如果为false 就会复制多份投向不同目标
    /// </summary>
    public bool isOnly;
    public BodyPart targetBodyPart = BodyPart.Root; //追踪目标的部位
    public string targetCustomPath;
    public Transform characterTran; //该技能角色的transform
    public Transform mainTarget; //主要目标
    public Transform[] targets; //包含mainTarget的受击

    private Vector3[] _tagetPoints;
    private GameObject[] _effects;
    private ParticleSystem[] _particleSystems;
    private bool[] _playeds;

    private float _time;
    private bool _isInit;
    private bool _isFinish;
    private bool _played;
    private bool _isFadeOut;
    private bool _isStop;
    private bool _isDestroy;
    private Vector3 _originalPosition;//原始位置

    private Transform attchTran;  //实际初始化的
    private Transform hitMainTarget; //实际受击部位
    private Transform[] hitTargets; //实际受击部位

    /// <summary>
    /// 构造必要属性 优先级最高
    /// </summary>
    /// <param name="characterTran"></param>
    public void Spawn(Transform characterTran, Vector3 attachOffset, BodyPart bodyPart,string spawnCustomPath, BodyPart targetBodyPart,string targetCustomPath, float velocity, float flyTime, float fadeOutTime,bool isTarget,bool isLookAtTarget,bool isOnly)
    {
        this.characterTran = characterTran;
        offset = attachOffset;
        this.bodyPart = bodyPart;
        speed = velocity;
        this.flyTime = flyTime;
        this.fadeOutTime = fadeOutTime;
        this.targetBodyPart = targetBodyPart;
        this.spawnCustomPath = spawnCustomPath;
        this.targetCustomPath = targetCustomPath;
        this.isTarget = isTarget;
        this.isLookAtTarget = isLookAtTarget;
        this.isOnly = isOnly;
    }

    /// <summary>
    /// 初始化 优先级低于Spawn
    /// </summary>
    public void Init(Transform[] tars = null)
    {
        if (characterTran != null && characterTran.GetComponent<Animator>() != null)
        {
            transform.parent = MoveUtils.GetBodyPartTransform(characterTran.GetComponent<Animator>(), bodyPart, spawnCustomPath);
        }
        else
        {
            EB.Debug.LogError("[NullReferenceException] ProjectileMono.Init: line 88");
        }

        transform.localPosition = Vector3.zero;
        transform.position += offset; //非常省事  
        transform.parent = null;
        Reset();

        if (isTarget)
        {
            if (Application.isPlaying)
            {
                //Combatant cb = characterTran.GetComponent<Combatant>();
                //if (cb != null&& cb.LTTargets!=null)
                //{
                //    Combatant[] tcbs = new Combatant[cb.LTTargets.Count];
                //    for (int i = 0; i < tcbs.Length; i++)
                //    {
                //        tcbs[i] = LTCombatEventReceiver.Instance.GetCombatant(cb.LTTargets[i]);
                //    }
                //    if (targets == null || targets.Length != tcbs.Length)
                //    {
                //        targets = new Transform[tcbs.Length];
                //    }
                //    for (int i = 0; i < tcbs.Length; i++)
                //    {
                //        targets[i] = tcbs[i].transform;
                //    }

                //    mainTarget = targets[0];//运行时 默认暂定为第0个下标的 为主要点选对象
                //}

                if (tars != null && tars.Length > 0)
                {
                    targets = tars;
                    mainTarget = targets[0];//运行时 默认暂定为第0个下标的 为主要点选对象
                }
            }
        }

        if (targets != null)
        {
            if (targetBodyPart == BodyPart.Root)
            {
                hitTargets = targets;
            }
            else
            {
                hitTargets = new Transform[targets.Length];
                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i] != null && targets[i].GetComponent<Animator>() != null)
                    {
                        hitTargets[i] = MoveUtils.GetBodyPartTransform(targets[i].GetComponent<Animator>(), targetBodyPart, targetCustomPath);
                    }
                    else
                    {
                        EB.Debug.LogError("[NullReferenceException] ProjectileMono.Init: line 145");
                    }
                }
            }
        }

        if (isOnly)
        {
            if (targetBodyPart == BodyPart.Root)
            {
                hitMainTarget = mainTarget;
            }
            else
            {
                if (mainTarget != null && mainTarget.GetComponent<Animator>() != null)
                {
                    hitMainTarget = MoveUtils.GetBodyPartTransform(mainTarget.GetComponent<Animator>(), targetBodyPart, targetCustomPath);
                }
                else
                {
                    EB.Debug.LogError("[NullReferenceException] ProjectileMono.Init: line 165");
                }
            }
        }

        if (isTarget && targets != null)
        {
            if (isOnly)
            {
                _tagetPoints = new Vector3[1];
                _tagetPoints[0] = hitMainTarget.position;
                _effects = new GameObject[1];
                _playeds = new bool[1];
                //从对象池里拿
                ParticleSystem ps = PSPoolManager.Instance.Use(this,effectPrefab.name);
                if (ps != null)
                {
                    _effects[0] = ps.gameObject;
                    ps.gameObject.transform.parent = this.transform;
                }
                if (_effects[0] == null)
                {
                    _effects[0] = Instantiate(effectPrefab);
                }
                
                _particleSystems = new ParticleSystem[1];
                effectTimes = new float[_particleSystems.Length];
                _particleSystems[0] = _effects[0].GetComponent<ParticleSystem>();
                if (_particleSystems[0] == null)
                {
                    _particleSystems[0] = _effects[0].GetComponentInChildren<ParticleSystem>();
                }
                if (_particleSystems[0] != null)
                {
                    _particleSystems[0].transform.position = transform.position;
                }
            }
            else
            {
                _tagetPoints = new Vector3[hitTargets.Length];

                _effects = new GameObject[hitTargets.Length];
                _playeds = new bool[hitTargets.Length];
                _particleSystems = new ParticleSystem[hitTargets.Length];
                effectTimes = new float[_particleSystems.Length];

                for (int i = 0; i < _tagetPoints.Length; i++)
                {
                    if (hitTargets[i] != null)
                    {
                        _tagetPoints[i] = hitTargets[i].transform.position;
                    }
                    else
                    {
                        EB.Debug.LogError("[NullReferenceException] ProjectileMono.Init: line 219");
                    }
                    
                    //从对象池里拿
                    ParticleSystem ps = PSPoolManager.Instance.Use(this, effectPrefab.name);
                    //ParticleSystem ps = Instantiate(effectPrefab).GetComponent<ParticleSystem>();
                    if (ps != null)
                    {
                        _effects[i] = ps.gameObject;
                        ps.gameObject.transform.parent = this.transform;
                    }
                    if (_effects[i] == null)
                    {
                        _effects[i] = Instantiate(effectPrefab);
                    }
                    _particleSystems[i] = _effects[i].GetComponent<ParticleSystem>();
                    if (_particleSystems[i] == null)
                    {
                        _particleSystems[i] = _effects[i].GetComponentInChildren<ParticleSystem>();
                    }
                    if (_particleSystems[i] != null)
                    {
                        _particleSystems[i].transform.position = transform.position;
                    }
                }
            }
        }
        else //演示为无目标
        {
            _tagetPoints = new Vector3[1];
            _tagetPoints[0] = characterTran.forward * speed * flyTime + transform.position;
            _effects = new GameObject[1];
            _playeds = new bool[1];

            //从对象池里拿
            ParticleSystem ps = PSPoolManager.Instance.Use(this,effectPrefab.name);
            if (ps != null)
            {
                _effects[0] = ps.gameObject;
                ps.gameObject.transform.parent = this.transform;
            }
            if (_effects[0] == null)
            {
                _effects[0] = Instantiate(effectPrefab);
            }
            //
            _particleSystems = new ParticleSystem[1];
            effectTimes = new float[_particleSystems.Length];
            _particleSystems[0] = _effects[0].GetComponent<ParticleSystem>();
            if (_particleSystems[0] == null)
            {
                _particleSystems[0] = _effects[0].GetComponentInChildren<ParticleSystem>();
            }
            if (_particleSystems[0] != null)
            {
                _particleSystems[0].transform.position = transform.position;
            }
        }
    }

    public void Reset()
    {
        _played = false;
        _isInit = true;
        _isFinish = false;
        _isFadeOut = false;
        _isStop = false;
        _time = 0;

        if (!Application.isPlaying)
        {
            if (_effects != null)
            {
                for (int i = 0; i < _effects.Length; i++)
                {
                    ShowOtherRenderer(_effects[i].transform);
                }
            }
        }
    }

    private float[] effectTimes;
    /// <summary>
    /// 通用Update
    /// </summary>
    /// <param name="time"></param>
    public void CommonUpdate(float time)
    {
        if (!_isInit || _isFinish)
        {
            return;
        }
        float t = time / flyTime;
        if (t > 1)
        {
            t = 1;
            if (!_isFadeOut)
            {
                _isFadeOut = true;
                if (_particleSystems != null)
                {
                    for (int i = 0; i < _particleSystems.Length; i++)
                    {
                        _particleSystems[i].EnableEmission(false);
                    }
                }
            }
            if (hitCallBack != null)
            {
                hitCallBack();
                hitCallBack = null;//仅需回掉一次
            }
        }
        if (_particleSystems != null)
        {
            for (int i = 0; i < _particleSystems.Length; i++)
            {
                PlaySimulate(_particleSystems[i], _effects[i], time, time - effectTimes[i], i);
                effectTimes[i] = time;
            }
        }
        
        float y = (motionCurve.Evaluate(System.Math.Min(0, 1.0f)) - motionCurve.Evaluate(System.Math.Min(t, 1.0f))) * 10;

        if (t > 0.8 && isTarget) // 如果以后需要刷新目标位置可以使用
        {
            if (isOnly && mainTarget != null)
            {

            }
            else
            {

            }
        }

        if(_effects!=null) for (int i = 0; i < _effects.Length; i++)
        {
            if (_effects[i] == null)
            {
                continue;
            }
            Vector3 behind = (_tagetPoints[i] - transform.position);
            _effects[i].transform.position = transform.position + t * behind + new Vector3(0f, y, 0f);
            if (isLookAtTarget)
            {
                _effects[i].transform.LookAt(_tagetPoints[i]);
            }
        }

        if (time > flyTime + fadeOutTime)
        {
            OnFinish();
        }
    }

    //完成
    void OnFinish()
    {
        if (!Application.isPlaying)
        {
            for (int i = 0; i < _particleSystems.Length; i++)
            {
                StopSimulate(_particleSystems[i]);
            }

            for (int i = 0; i < _effects.Length; i++)
            {
                HideOtherRenderer(_effects[i].transform);
            }
        }
        else
        {
            Stop();
        }

        if (finishCallBack != null)
        {
            finishCallBack();
            finishCallBack = null; //仅需回掉一次
        }

        _time = 0;
        _isFinish = true;
    }

    //结束 运行状态会主动调用  编辑状态随Destroy调用
    public void Stop() 
    {
        if(_isStop)
        {
            return;
        }

        _isStop = true;

        if (_effects != null)
        {
            if (Application.isPlaying)
            {
                for (int i = 0; i < _effects.Length; i++)
                {
                    //回收特效
                    PSPoolManager.Instance.Recycle(_effects[i]);
                    //Destroy(_effects[i]);
                }
            }
            else
            {
                for (int i = 0; i < _effects.Length; i++)
                {
                    DestroyImmediate(_effects[i]);
                }
            }
        }
        _effects = null;
        _playeds = null;

        if(!_isDestroy)
        {
            if (Application.isPlaying)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnDestroy()
    {
        _isDestroy = true;

        if (!_isStop)
        {
            Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;

        CommonUpdate(_time);
    }

    /// <summary>
    /// 播放
    /// </summary>
    /// <param name="particleSystem">首个粒子系统</param>
    /// <param name="effect"></param>
    /// <param name="timeSinceTrigger">触发后的计时</param>
    protected void PlaySimulate(ParticleSystem particleSystem, GameObject effect, float timeSinceTrigger, float effectTime, int index)
    {
        if (Application.isPlaying)
        {
            if (!_playeds[index])
            {
                particleSystem.EnableEmission(true);
                particleSystem.Simulate(0.0001f, true, true);
                particleSystem.Play(true);
                _playeds[index] = true;
            }
        }
        else
        {
            if (!_played)
            {
                particleSystem.Play(true);
                particleSystem.EnableEmission(true);
            }
            _played = true;
            particleSystem.Simulate(effectTime, true, false, true);

            var ani = effect.GetComponent<Animation>();
            if (ani != null)
            {
                ani.clip.SampleAnimation(ani.gameObject, timeSinceTrigger);
            }

            var ant = effect.GetComponent<Animator>();
            if (ant != null)
            {
                ant.Update(timeSinceTrigger);
            }

            foreach (var anim in effect.GetComponentsInChildren<Animation>())
            {
                anim.clip.SampleAnimation(anim.gameObject, timeSinceTrigger);
            }

            foreach (var anim in effect.GetComponentsInChildren<Animator>())
            {
                anim.Update(timeSinceTrigger);
            }
        }
    }

    protected void StopSimulate(ParticleSystem particleSystem)
    {
        _played = false;

        particleSystem.Stop(true);
        particleSystem.EnableEmission(false);
        if (!Application.isPlaying)
        {
            particleSystem.Clear(true);
        }
    }
    
    protected void HideOtherRenderer(Transform tran)
    {
        Renderer renderer = tran.GetComponent<Renderer>();

        if (renderer is MeshRenderer)
        {
            renderer.enabled = false;
            //if (particleTransform.gameObject.GetComponent<NcSpriteAnimation>() != null)
            //{
            //    particleTransform.gameObject.GetComponent<NcSpriteAnimation>().ResetAnimation();
            //}
        }

        else if (renderer is SkinnedMeshRenderer)
        {
            renderer.enabled = false;
        }
        else if (renderer is UnityEngine.TrailRenderer)
        {
            renderer.enabled = false;
        }

        int transChildCount = tran.childCount;

        if (transChildCount != 0)
        {
            for (int i = 0; i < transChildCount; i++)
            {
                HideOtherRenderer(tran.GetChild(i));
            }
        }
    }

    protected void ShowOtherRenderer(Transform tran)
    {
        Renderer renderer = tran.GetComponent<Renderer>();

        if (renderer is MeshRenderer)
        {
            renderer.enabled = true;
            //if (particleTransform.gameObject.GetComponent<NcSpriteAnimation>() != null)
            //{
            //    particleTransform.gameObject.GetComponent<NcSpriteAnimation>().ResetAnimation();
            //}
        }

        else if (renderer is SkinnedMeshRenderer)
        {
            renderer.enabled = true;
        }

        else if (renderer is UnityEngine.TrailRenderer)
        {
            renderer.enabled = true;
        }

        int transChildCount = tran.childCount;

        if (transChildCount != 0)
        {
            for (int i = 0; i < transChildCount; i++)
            {
                HideOtherRenderer(tran.GetChild(i));
            }
        }
    }
}
