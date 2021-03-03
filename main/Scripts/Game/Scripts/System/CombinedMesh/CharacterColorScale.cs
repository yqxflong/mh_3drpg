using UnityEngine;
using System.Collections;

public class CharacterColorScale : MonoBehaviour
{
    public float _ColorScale = 1.0f;
    private bool _IsInCombat = false;
    private bool _IsIrrelevance = false;

    public void ForceUpdateColorScale()
    {
        UpdateColorScale(true);
    }

    void Update()
    {
        UpdateColorScale(false);
    }

    void UpdateColorScale(bool forceUpdate)
    {
        float OrigColorScale = _ColorScale;

        RenderSettings rs = (RenderSettings)RenderSettingsManager.Instance.GetCurrentRenderSettings();
        if (_IsIrrelevance)
        {
            _ColorScale = rs.IrrelevanceCombatantScale;
        }
        else if (_IsInCombat)
        {
            _ColorScale = rs.CombatantScale;
        }
        else
        {
            _ColorScale = rs.NonCombatantScale;
        }

        if ((OrigColorScale != _ColorScale || forceUpdate) && Application.isPlaying)
        {
            SetColorScale(_ColorScale);
        }
    }

    private void SetColorScale(float colorScale)
    {
        SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            for (int j = 0; j < renderers[i].materials.Length; j++)
            {
                renderers[i].materials[j].SetFloat("_ColorScale", colorScale);
            }
        }
    }

    public void SetInCombat(bool inCombat)
    {
        _IsInCombat = inCombat;
    }

    public void SetIrrelevance(bool irrelevance)
    {
        _IsIrrelevance = irrelevance;
    }
}
