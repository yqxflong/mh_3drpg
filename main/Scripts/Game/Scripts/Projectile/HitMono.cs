using UnityEngine;
using System.Collections;
using MoveEditor;
using System.Collections.Generic;

public class HitMono : MonoBehaviour
{
    public delegate void HitDamage();
    public Animator animator;
    public Transform[] targets;

    //private Combatant _combatant;


    private float _time;

    public class ParticleCell
    {
        public float startTime;
        public ParticleSystem ps;
        public float duration;
        public bool isPlayed;
        public HitDamage onHitDamage;
        public ParticleCell(float startTime, ParticleSystem ps,float duration)
        {
            this.startTime = startTime;
            this.ps = ps;
            this.duration = duration;
        }
    }

    public class AudioCell
    {
        public AudioEventProperties audio_info;
        public float startTime;
        public AudioCell(float startTime,AudioEventProperties audio_info)
        {
            this.audio_info = audio_info;
            this.startTime = startTime;
        }
    }
    public List<ParticleCell> listPC = new List<ParticleCell>();
    public List<AudioCell> listAC = new List<AudioCell>();
    
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void EditorAwake()
    {
        Awake();
    }

    void Update()
    {
        _time += Time.deltaTime;

        CommonUpdate(_time);

    }

    float[] lastTime;
    /// <summary>
    /// 可以编辑器界面传时间或者自己传
    /// </summary>
    /// <param name="time"></param>
    public void CommonUpdate(float time)
    {
        bool isApplicationPlaying = false;

        if (Application.isPlaying)
        {
            isApplicationPlaying = true;
        }
        for (int i = 0; i < listPC.Count; i++)
        {
            float tempTime = time - listPC[i].startTime;
            float useTime = tempTime - lastTime[i];
            if (useTime <= 0)
            {
                continue;
            }
            lastTime[i] = tempTime;

            if (tempTime < 0) //没到使用时间就跳过
            {
                continue;
            }
            if (listPC[i].isPlayed && listPC[i].duration < tempTime)
            {
                if (listPC[i].ps != null)
                {
                    listPC[i].ps.Stop(true);

                    if (isApplicationPlaying)
                    {
                        //GameObject.Destroy(listPC[i].ps.gameObject);
                    }
                    else
                    {
                        GameObject.DestroyImmediate(listPC[i].ps.gameObject);
                    }
                }
                listPC.RemoveAt(i);
                i--;
                if (listPC.Count == 0) //假如没有了 再运行时间就将时间重新计时
                {
                    _time = 0;
                }
            }
            else
            {
                if(!listPC[i].isPlayed)// 补充判断 以防没有运行的
                {
                    listPC[i].isPlayed = true;
                    if (isApplicationPlaying) //运行时态时间是不一致 所以要单独再刷新一次
                    {
                        listPC[i].startTime = time;
                    }
                    PlayParticle(listPC[i].ps);
                }

                if (!isApplicationPlaying)
                {
                    listPC[i].ps.Simulate(useTime, true, false, true);
                }
            }
        }

        for (int i = 0; i < listAC.Count; i++)
        {
            if (!isApplicationPlaying) //运行时直接启动 因为生成即调用 
            {
                float useTime = time - listAC[i].startTime;

                if (useTime < 0) //没到使用时间就跳过
                {
                    continue;
                }
            }

            PlayAudio(listAC[i].audio_info);

            listAC.RemoveAt(i);
            i--;
        }
    }
     void OnDestroy()
    {
        Cleanup();
    }

    public void Cleanup()
    {
        if(Application.isPlaying)
        {
            for (int i = 0; i < listPC.Count; i++)
            {
                if (listPC[i].ps != null)
                {
                    listPC[i].ps.Stop(true);
                }
                //GameObject.Destroy(listPC[i].ps);
            }
        }else
        {
            for (int i = 0; i < listPC.Count; i++)
            {
                listPC[i].ps.Stop(true);
                GameObject.DestroyImmediate(listPC[i].ps.gameObject);
            }
        }

        listPC.Clear();
        listAC.Clear();

    }
    //单独放特效
    public void OnInflictHitEx(MoveAnimationEvent ee)
    {
        HitEventInfo event_info = ee.EventRef as MoveEditor.HitEventInfo;
        ParticleEventProperties particle_info = event_info._particleProperties;
        SpawnParitcle(particle_info, false, 0);
    }

    public void OnInflictAudioEx(MoveAnimationEvent ee)
    {
        HitEventInfo event_info = ee.EventRef as MoveEditor.HitEventInfo;
        AudioEventProperties audio_info = event_info._audioProperties;
        SpawnAudioClass arg = new SpawnAudioClass();
        arg.audio_info = audio_info;
        arg.startTime = 0;

        SpawnAudio(arg.audio_info, false, arg.startTime);
    }

    public void OnInflictHit(MoveAnimationEvent ee)
    {
        HitEventInfo event_info = ee.EventRef as MoveEditor.HitEventInfo;
        //PlayHitReactionProperties reaction_info = event_info._hitRxnProps;
        ParticleEventProperties particle_info = event_info._particleProperties;
        AudioEventProperties audio_info = event_info._audioProperties;

        if(particle_info._cancelIfMissed)
        {

        }
        SpawnHit(particle_info, audio_info,0);
    }

    

    /// <summary>
    /// 
    /// </summary>
    /// <param name="particle_info"></param>
    /// <param name="audio_info"></param>
    /// <param name="startTime">这个时间主要是给编辑器用 因为编辑器时钟是一致 但是运行时态脚本初始化不一致所以无视这个时间</param>
    public void SpawnHit(ParticleEventProperties particle_info , AudioEventProperties audio_info,float startTime)
    {
        //ParticleEventProperties particle_info = ParticleEventProperties.Deserialize(particle_info_primal.Serialize());
        //particle_info._particleReference = particle_info_primal._particleReference;
        //particle_info._flippedParticleReference = particle_info_primal._flippedParticleReference;
        //AudioEventProperties audio_info = AudioEventProperties.Deserialize(audio_info_primal.Serialize());

        SpawnParitcle(particle_info, particle_info._applyOnTargetList, startTime);
        SpawnAudioClass arg = new SpawnAudioClass();
        arg.audio_info = audio_info;
        arg.startTime = startTime;
        TimerManager.instance.AddFramer(2, 1, OnFrameUpSpawnAudioHandler, arg);
    }

    class SpawnAudioClass
    {
        public AudioEventProperties audio_info;
        public float startTime;
    }

    private void OnFrameUpSpawnAudioHandler(int timerSequence, object arg)
    {
        TimerManager.instance.RemoveTimer(OnFrameUpSpawnAudioHandler);

        SpawnAudioClass obj = arg as SpawnAudioClass;
        if (obj != null && obj.audio_info != null)
            SpawnAudio(obj.audio_info, obj.audio_info._applyOnTargetList, obj.startTime);
    }

    public void SpawnParitcle(ParticleEventProperties properties,bool applyOnTargetList,float startTime)
    {
        if(properties == null)
        {
            return; 
        }
        if (applyOnTargetList)
        {
            SpawnParticleOnTargetList(properties, applyOnTargetList, startTime);
            return;
        }

        ParticleSystem ps = MoveUtils.InstantiateParticle(this,properties, animator);

        if(ps==null)
        {
            EB.Debug.LogWarning("ps==null ", properties.ParticleName);
            return;
        }

        ParticleCell pc = new ParticleCell(startTime, ps, properties._duration <= 0 ? ps.duration : properties._duration);
        listPC.Add(pc);
        lastTime = new float[listPC.Count];
        if(!Application.isPlaying) //不需要加载
        {
            ps.EnableEmission(false);
            ps.Stop(true);
        }
        else
        {
            pc.startTime = _time;
            pc.isPlayed = true;
            PlayParticle(pc.ps, 0);
        }
    }
    

    public void PlayParticle(ParticleSystem ps, float time = 0)
    {
        if (ps == null)
        {
            return;
        }
        ps.EnableEmission(true);

        if (time < 0.01f)
        {
            ps.Simulate(0.0001f, true, true);
        }
        else
        {
            ps.Simulate(time, true, true);
        }

        ps.Play(true);
    }

    public void SpawnParticleOnTargetList(ParticleEventProperties properties,bool applyOnTargetList,float startTime)
    {
        if (!applyOnTargetList)
        {
            return;
        }
        
        if (Application.isPlaying)  //运行模式是combatant里取targets
        {
            //if (_combatant == null)
            //{
            //    _combatant = GetComponent<Combatant>();
            //}

            //if (_combatant.LTTargets == null) return;
            //Combatant[] tcs = new Combatant[_combatant.LTTargets.Count];

            //if(tcs==null)
            //{
            //    return;
            //}
            //for(int i=0;i<tcs.Length;i++)
            //{
            //    tcs[i] = LTCombatEventReceiver.Instance.GetCombatant(_combatant.LTTargets[i]);
            //}

            //if (targets == null || targets.Length !=tcs.Length)
            //{
            //    targets = new Transform[tcs.Length];
            //}

            //for(int i =0;i< targets.Length;i++)
            //{
            //    if (tcs[i] != null)
            //    {
            //        targets[i] = tcs[i].transform;
            //    }
            //}
            
        }

        if (targets == null)
        {
            return;
        }
        
        for(int i=0;i<targets.Length;i++)
        {
            Transform target = targets[i];
            if (target != null)
            {
                HitMono hitMono = target.GetComponent<HitMono>();
                if(hitMono == null)
                {
                    hitMono = target.gameObject.AddComponent<HitMono>();
                }
                hitMono.SpawnParitcle(properties,!applyOnTargetList, startTime);
            }
        }
    }

    public void SpawnAudio(AudioEventProperties audio_info,bool applyOnTargetList,float startTime)
    {
        if (audio_info == null)
        {
            return;
        }
        if (applyOnTargetList)
        {
            SpawnAudioOnTargetList(audio_info, applyOnTargetList, startTime);
            return;
        }
        AudioCell ac= new AudioCell(startTime, audio_info);
        listAC.Add(ac);
    }

    public void SpawnAudioOnTargetList(AudioEventProperties audio_info,bool applyOnTargetList, float startTime)
    {
        if (!applyOnTargetList)
        {
            return;
        }
        if (targets == null)
        {
            return;
        }
        
        foreach (Transform target in targets)
        {
            if (target != null)
            {
                HitMono hitMono = target.GetComponent<HitMono>();
                if (hitMono == null)
                {
                    hitMono = target.gameObject.AddComponent<HitMono>();
                }
                hitMono.SpawnAudio(audio_info, !applyOnTargetList,startTime);
            }
        }
    }

    public void PlayAudio(AudioEventProperties audioPropes)
    {
        FusionAudio.PostEvent(audioPropes._event, gameObject, true);
    }

}
