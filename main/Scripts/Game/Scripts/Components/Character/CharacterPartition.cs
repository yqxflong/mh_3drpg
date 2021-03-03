using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 部件数据组件
/// </summary>
public class CharacterPartition : MonoBehaviour
{
    [System.Serializable]
    public struct MeshMaterialInfo
    {
        public string MeshName;
        public int MaterialCount;
        public Material[] Materials;
    }
    /// <summary>
    /// 部件源文件
    /// </summary>
    [HideInInspector]
    [SerializeField]
    public GameObject m_FBXSource;
    /// <summary>
    /// 部件相应的材质
    /// </summary>
    [HideInInspector]
    [SerializeField]
    public List<MeshMaterialInfo> m_MaterialRegistry = new List<MeshMaterialInfo>();
    /// <summary>
    /// 部件对象
    /// </summary>
    private GameObject m_PartitionObj;

    /// <summary>
    /// 初始化
    /// </summary>
    public void Initialize()
    {
        if (m_FBXSource == null)
        {
            EB.Debug.LogError("部件引用的预置为空了:{0}", name);
            return;
        }
        
        EB.Debug.LogObjectMgrAsset("初始化部件<color=#ff0000>克隆一次，部件需要的:</color>{0},预置体:<color=#00ff00>{1}</color>", name, m_FBXSource.name);
        GameObject partitionObj = GameObject.Instantiate(m_FBXSource, Vector3.zero, Quaternion.identity) as GameObject;

        partitionObj.transform.parent = transform;
        SkinnedMeshRenderer[] skinnedMeshes = partitionObj.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = skinnedMeshes.Length - 1; i >= 0; i--)
        {
            SkinnedMeshRenderer mesh = skinnedMeshes[i];
            if (mesh == null)
            {
                continue;
            }
            for (int j = m_MaterialRegistry.Count - 1; j >= 0; j--)
            {
                MeshMaterialInfo info = m_MaterialRegistry[j];
                if (info.MeshName == mesh.name)
                {
                    mesh.sharedMaterials = info.Materials;
                    break;
                }
            }
        }

        GM.AssetUtils.FixShaderInEditor(partitionObj);
    }
    
    public bool IsMeshMaterialOverride(string meshName)
    {
        for (int i = m_MaterialRegistry.Count - 1; i >= 0; i--)
        {
            if (m_MaterialRegistry[i].MeshName == meshName)
            {
                return true;
            }
        }

        return false;
    }

    public void RegisterMesh(string meshName, Material[] materials)
    {
        for (int i = m_MaterialRegistry.Count - 1; i >= 0; i--)
        {
            if (m_MaterialRegistry[i].MeshName == meshName)
            {
                MeshMaterialInfo info = m_MaterialRegistry[i];
                info.Materials = materials;
                info.MaterialCount = materials == null ? 0 : materials.Length;
                m_MaterialRegistry[i] = info;
                return;
            }
        }
        MeshMaterialInfo newInfo = new MeshMaterialInfo();
        newInfo.MeshName = meshName;
        newInfo.Materials = materials;
        if (materials != null)
        {
            newInfo.MaterialCount = materials.Length;
        }
        else
        {
            newInfo.MaterialCount = 0;
        }

        m_MaterialRegistry.Add(newInfo);
    }

    public MeshMaterialInfo GetMeshMaterialInfo(string meshName)
    {
        for (int i = m_MaterialRegistry.Count - 1; i >= 0; i--)
        {
            if (m_MaterialRegistry[i].MeshName == meshName)
            {
                return m_MaterialRegistry[i];
            }
        }

        return new MeshMaterialInfo();
    }

    public void ClearMeshMaterialInfos()
    {
        m_MaterialRegistry.Clear();
    }

    /*
    private void OnDestroy()
    {
        EB.Debug.LogObjectMgrAsset("清除掉_人物部件上的引用_name:" + this.name);
        m_FBXSource = null;
        for (int i=0;i< m_MaterialRegistry.Count;i++)
        {
            for (int j=0;j< m_MaterialRegistry[i].Materials.Length;j++)
            {
                m_MaterialRegistry[i].Materials[j] = null;
            }
        }
    }
    */
}
