using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RenderSettingsManager : MonoBehaviour
{
    public class ActiveRenderSettingsItem
    {
        public string name;
        public float startTime;
        public float duration;
    }

    private static bool _created = false;
    private static RenderSettingsManager _Instance = null;
    public static RenderSettingsManager Instance
    {
        get
        {
            if (_Instance == null && !_created)
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    Setup();
                }
                else
                {
                    FixRenderSetting();
                }
#else
                Setup();
#endif
            }
            return _Instance;
        }
    }

    private Dictionary<string, RenderSettings> _renderSettings = new Dictionary<string, RenderSettings>();
    private List<ActiveRenderSettingsItem> _activeRenderSettings = new List<ActiveRenderSettingsItem>();
    private RenderSettings _toBlendRenderSettings = null;
    public string defaultSettings = string.Empty;
    public string recoverSettings = "RecoverInTimeRenderSetting";

    public Dictionary<string, RenderSettings> RenderSettingDic()
    {
        return _renderSettings;
    }

    private static void Setup()
    {
        GameObject renderSettings = new GameObject("RenderSettingsManager");
        GameObject lastSettings = new GameObject("LastRenderSettings");
        GameObject currentSettings = new GameObject("CurrentRenderSettings");
        lastSettings.transform.parent = renderSettings.transform;
        currentSettings.transform.parent = renderSettings.transform;

        _Instance = renderSettings.AddComponent<RenderSettingsManager>();
        _Instance._toBlendRenderSettings = renderSettings.AddComponent<RenderSettings>();
        _Instance._lastRenderSettings = lastSettings.AddComponent<RenderSettings>();
        _Instance._currentRenderSettings = currentSettings.AddComponent<RenderSettings>();

        if (Application.isPlaying)
        {
            GameObject.DontDestroyOnLoad(renderSettings);
        }
        else
        {
            renderSettings.hideFlags = HideFlags.DontSave;
        }
    }

    private RenderSettings _lastRenderSettings = null;
    private RenderSettings _currentRenderSettings = null;
    private float _blendTime = 0;

    public RenderSettings GetCurrentRenderSettings()
    {
        return _toBlendRenderSettings;
    }

    public RenderSettings GetLastRenderSettings()
    {
        return _lastRenderSettings;
    }

    public static bool Ignore(RenderSettings rs)
    {
        return _Instance == null || rs.transform.root == _Instance.transform;
    }

    public void Register(RenderSettings renderSettings)
    {
        if (Ignore(renderSettings))
        {
            return;
        }

#if UNITY_EDITOR
        if (_renderSettings.ContainsKey(renderSettings.name))
        {
            EB.Debug.LogWarning("Duplicate RenderSetting with id {0}", renderSettings.name);
        }
#endif
        _renderSettings[renderSettings.name] = renderSettings;

        if (renderSettings.StartActive)
        {
            defaultSettings = renderSettings.name;

            //ActivateRenderSettings(defaultSettings, -1.0f);
        }
    }

    public void UnRegister(RenderSettings renderSettings)
    {
        if (Ignore(renderSettings))
        {
            return;
        }

        if (_renderSettings.ContainsKey(renderSettings.name))
        {
            _renderSettings.Remove(renderSettings.name);
        }
    
        if (defaultSettings == renderSettings.name)
        {
            defaultSettings = string.Empty;
        }

        if (RemoveActiveItems(renderSettings.name))
        {
            ActivateTopItem();
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (_activeRenderSettings.Count == 0 && _renderSettings.Count == 0)
            {
                DestroyImmediate(_Instance.gameObject);
                RenderSettings.forceUpdate = false;

                var cameras = UnityEditor.SceneView.GetAllSceneCameras();
                foreach (var camera in cameras)
                {
                    PostFXManagerTrigger trigger = camera.GetComponent<PostFXManagerTrigger>();
                    if (trigger != null)
                    {
                        DestroyImmediate(trigger);
                    }
                }

                //PostFXManager.DestroyInstance();
                //PlanarReflectionManager.DestroyInstance();
            }
        }
#endif
    }

    public void SetActiveRenderSettings(string name, RenderSettings rs =null)
    {
        ActivateRenderSettings(name, -1.0f,rs);
    }

    public void ActivateRenderSettings(string name, float duration, RenderSettings addRs =null)
    {
        if (duration == 0.0f)
        {
            if (RemoveActiveItems(name))
            {
                ActivateTopItem();
            }

            return;
        }

        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        if (!_renderSettings.ContainsKey(name))
        {
            if(addRs!=null)
            {
                _renderSettings.Add(name, addRs); //因为有同名的注销问题 所以添加这种方式
            }
            else
            {
                EB.Debug.LogWarning(string.Format("RenderSettingsManager.ActivateRenderSettings: RenderSettings {0} not exists", name));
                return;
            }
        }
        //EB.Debug.Log("RenderSettingsManager: Enabling " + name);

        if (_toBlendRenderSettings == null)
        {
            EB.Debug.LogWarning("RenderSettingsManager.ActivateRenderSettings: _toBlendRenderSettings is null");
            return;
        }

        RenderSettings rs = _renderSettings[name];
        if (rs.StartActive || rs.BlendInTime <= 0.0f)
        {
            _lastRenderSettings.Clone(rs);
            _currentRenderSettings.Clone(rs);
            _toBlendRenderSettings.Clone(rs);

            _toBlendRenderSettings.ApplyAtSceneLoad();
            _toBlendRenderSettings.ApplyEveryFrame();

            _blendTime = rs.BlendInTime + 1.0f;
        }
        else
        {
            _lastRenderSettings.Clone(_toBlendRenderSettings);
            _currentRenderSettings.Clone(rs);

            _blendTime = 0.0f;
        }

        if (duration < 0.0f)
        {
            _activeRenderSettings.Clear();
        }

        ActiveRenderSettingsItem push = new ActiveRenderSettingsItem();
        push.startTime = Time.time;
        push.duration = duration;
        push.name = name;

        _activeRenderSettings.Add(push);
    }

    private void ActivateTopItem()
    {
        if (_activeRenderSettings.Count == 0)
        {
            EB.Debug.Log("RenderSettingsManager.ActivateTopItem: active queue empty");
            return;
        }

        var top = _activeRenderSettings[_activeRenderSettings.Count - 1];
        if (!_renderSettings.ContainsKey(top.name))
        {
            EB.Debug.LogError(string.Format("RenderSettingsManager.ActivateTopItem: RenderSettings {0} not exists", top.name));
            return;
        }
        EB.Debug.Log("RenderSettingsManager: Enabling {0}", top.name);

        RenderSettings rs = _renderSettings[top.name];
        if (rs.StartActive || rs.BlendInTime <= 0.0f)
        {
            _lastRenderSettings.Clone(rs);
            _currentRenderSettings.Clone(rs);
            _toBlendRenderSettings.Clone(rs);

            _toBlendRenderSettings.ApplyAtSceneLoad();
            _toBlendRenderSettings.ApplyEveryFrame();

            _blendTime = rs.BlendInTime + 1.0f;
        }
        else
        {
            _lastRenderSettings.Clone(_toBlendRenderSettings);
            _currentRenderSettings.Clone(rs);

            _blendTime = 0.0f;
        }
    }

    private bool RemoveTimeoutActiveItems()
    {
        bool removed = false;

        for (int i = _activeRenderSettings.Count - 1; i >= 0; --i)
        {
            var item = _activeRenderSettings[i];
            if (item.duration > 0 && Time.time - item.startTime > item.duration)
            {
                _activeRenderSettings.RemoveAt(i);
                removed = true;
            }
            else
            {
                break;
            }
        }

        return removed;
    }

    public void ResetRecoverSetting()
    {
        if (_renderSettings.ContainsKey(recoverSettings))
        {
            GameObject.DestroyImmediate(_renderSettings[recoverSettings]);
        }

        _renderSettings.Remove(recoverSettings);
    }

    private bool RemoveActiveItems(string name)
    {
        bool top_removed = _activeRenderSettings.Count > 0 && _activeRenderSettings[_activeRenderSettings.Count - 1].name == name;
        _activeRenderSettings.RemoveAll(rs => rs.name == name);
        return top_removed;
    }

#if UNITY_EDITOR
    public void SetActiveRenderSettings(RenderSettings rs)
    {
        RenderSettings lastRenderSettings = _toBlendRenderSettings.transform.Find("LastRenderSettings").GetComponent<RenderSettings>();
        RenderSettings currentRenderSettings = _toBlendRenderSettings.transform.Find("CurrentRenderSettings").GetComponent<RenderSettings>();
        if (_toBlendRenderSettings == rs)
        {
            _lastRenderSettings = lastRenderSettings;
            _currentRenderSettings = currentRenderSettings;
            _blendTime = _currentRenderSettings.BlendInTime + 1.0f;
        }
        else if (lastRenderSettings == rs)
        {
            _lastRenderSettings = lastRenderSettings;
            _currentRenderSettings = currentRenderSettings;
            _blendTime = 0;
        }
        else if (currentRenderSettings == rs)
        {
            _lastRenderSettings = lastRenderSettings;
            _currentRenderSettings = currentRenderSettings;
            _blendTime = currentRenderSettings.BlendInTime;
        }
        else
        {
            _blendTime = 0;
            _lastRenderSettings = rs;
            _currentRenderSettings = rs;
        }

        // indicate current active render settings
        defaultSettings = rs.name;
    }

    private static void FixRenderSetting()
    {
        GameObject renderSettings = GameObject.Find("RenderSettingsManager");
        if (renderSettings == null)
        {
            renderSettings = new GameObject("RenderSettingsManager");
            _Instance = renderSettings.AddComponent<RenderSettingsManager>();
            _Instance._toBlendRenderSettings = renderSettings.AddComponent<RenderSettings>();
        }
        else
        {
            _Instance = renderSettings.GetComponent<RenderSettingsManager>() ?? renderSettings.AddComponent<RenderSettingsManager>();
            _Instance._toBlendRenderSettings = renderSettings.GetComponent<RenderSettings>();
        }

        Transform lastSettings = renderSettings.transform.Find("LastRenderSettings");
        if (lastSettings == null)
        {
            lastSettings = new GameObject("LastRenderSettings").transform;
            lastSettings.transform.parent = renderSettings.transform;
            _Instance._lastRenderSettings = lastSettings.gameObject.AddComponent<RenderSettings>();
        }
        else
        {
            _Instance._lastRenderSettings = lastSettings.gameObject.GetComponent<RenderSettings>() ?? lastSettings.gameObject.AddComponent<RenderSettings>();
        }

        Transform currentSettings = renderSettings.transform.Find("CurrentRenderSettings");
        if (currentSettings == null)
        {
            currentSettings = new GameObject("CurrentRenderSettings").transform;
            currentSettings.transform.parent = renderSettings.transform;
            _Instance._currentRenderSettings = currentSettings.gameObject.AddComponent<RenderSettings>();
        }
        else
        {
            _Instance._currentRenderSettings = currentSettings.gameObject.GetComponent<RenderSettings>() ?? currentSettings.gameObject.AddComponent<RenderSettings>();
        }

        if (Application.isPlaying)
        {
            GameObject.DontDestroyOnLoad(renderSettings);
        }
        else
        {
            renderSettings.hideFlags = HideFlags.DontSave;
        }
    }
#endif

    void Update()
    {
        if (_toBlendRenderSettings == null)
        {
            return;
        }

        if (RemoveTimeoutActiveItems())
        {
            ActivateTopItem();
        }

        if ((_lastRenderSettings == null) || (_currentRenderSettings == null) || (_currentRenderSettings.BlendInTime <= 0.0f) || (_blendTime > _currentRenderSettings.BlendInTime))
        {
            //not blending
            if (_currentRenderSettings != null)
            {
#if UNITY_EDITOR
                _toBlendRenderSettings.ApplyAtSceneLoad();
#endif
                _toBlendRenderSettings.ApplyEveryFrame();
            }

            return;
        }

        _blendTime += Time.deltaTime;

        float t = CosLerp (0.0f, 1.0f, _blendTime / _currentRenderSettings.BlendInTime);

        _toBlendRenderSettings.ApplyBlend(_lastRenderSettings, _currentRenderSettings, t);
        _toBlendRenderSettings.ApplyAtSceneLoad();
        _toBlendRenderSettings.ApplyEveryFrame();
    }

    void OnDestroy()
    {
        if (!Application.isPlaying)
        {
            _created = false;
        }
    }

    float CosLerp(float from, float to, float x)
    {
        float f = (1.0f - Mathf.Cos(x * Mathf.PI * .5f));
        return f * (to - from) + from;
    }
    
    public void ReplaceGlobalCharacterOutline(float scale)
    {
        RenderSettings.GlobalCharacterOutlineScale = scale;
        RenderSettingsBase rsb = RenderSettingsManager.Instance.GetCurrentRenderSettings();
        RenderSettings rs = rsb as RenderSettings;
        if (rs != null)
        {
            float use = scale * rs.CharactorOutlineScale;
            Shader.SetGlobalFloat("_EBGCharOutlineScale", use);
        }
    }
}
