using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Fabric;
using UnityEngine;
using Object = UnityEngine.Object;

public class EventManagerEx : Fabric.EventManager {

    public static EventManagerEx Instance
    {
        get
        {
            if (EventManagerEx._instance == null)
            {
                if (FabricManager.Instance != null)
                {
                    EventManagerEx._instance = FabricManager.Instance.GetComponent<EventManagerEx>();
                    if (EventManagerEx._instance == null)
                    {
                        EventManagerEx._instance = (EventManagerEx)FabricManager.Instance.gameObject.AddComponent(typeof(EventManagerEx));
                    }
                }
                if (EventManagerEx._instance == null)
                {
                    EventManagerEx._instance = (EventManagerEx)Object.FindObjectOfType(typeof(EventManagerEx));
                }
            }
            return EventManagerEx._instance;
        }
    }

    public GroupComponent GetGroupComponent(string GroupName)
    {
        GroupComponent group =  GameObject.Find(GroupName).GetComponent<GroupComponent>();
        if (group)
        {
            return group;
        }
        EB.Debug.LogError("can not find group : {0}", GroupName);
        return null;
    }

    public Dictionary<string, AudioComponent> AudioComponentDic = new Dictionary<string, AudioComponent>();

    public AudioComponent SetAudioComponentFirstSibling(string eventName)
    {
        AudioComponent audio;
        string component = eventName.Replace("/", "_");
        EB.Debug.LogError("component : {0}",component);
        if (!AudioComponentDic.ContainsKey(component))
        {
            audio = GameObject.Find(component).GetComponent<AudioComponent>();
            AudioComponentDic.Add(component, audio);

        }
        audio = AudioComponentDic[component];
        if (audio)
        {
            audio.transform.SetAsFirstSibling();
            return audio;
        }
        EB.Debug.LogError("can not find group : {0}" , component);
        return null;
    }

    public AudioComponent GetAudioComponent(string component)
    {
        AudioComponent audio = GameObject.Find(component).GetComponent<AudioComponent>();
        if (audio)
        {
            return audio;
        }
        EB.Debug.LogError("can not find group : {0}", component);
        return null;
    }

    private static EventManagerEx _instance;
}
