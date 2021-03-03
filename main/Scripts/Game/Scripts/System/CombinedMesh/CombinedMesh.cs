using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterMeshUpdateEvent : GameEvent
{
    private GameObject _target = null;

    public GameObject Target
    {
        get { return _target; }
    }

    public CharacterMeshUpdateEvent(GameObject target)
    {
        _target = target;
    }
}

public class CombinedMesh : MonoBehaviour {
    [HideInInspector]
    public bool _CombineTo1Draw;
    
    [System.Serializable]
    public struct ToCombineRow {
        public string Name;
        public GameObject GameObject;
        public Material[] OverrideMaterial;
        public GameObject CombinedMesh;
    }
    
    public ToCombineRow[] _toCombine;
    public string _toDelete;
    //private CharacterMeshUpdateEvent _meshUpdateEvent = null;

    void Awake ()
    {
        Refresh(); // refresh on Awake (in place of Start) cause some other component, such as Combatant.cs need this ready on their Start()
    }
    
    private void Refresh(int toRefreshIndex = -1, string equipmentDataId = null) {
        //Step -1
        int[] refreshIndices = new int[_toCombine.Length];
        for (int i = 0; i < refreshIndices.Length; i++) {
            refreshIndices [i] = toRefreshIndex == -1 || i == toRefreshIndex ? 1 : 0;
        }

        for(int ii = 0; ii < _toCombine.Length; ++ii)
        {
            if(_toCombine[ii].GameObject != null)
            {
                CombinedMeshExtender cme = _toCombine[ii].GameObject.GetComponent<CombinedMeshExtender>();
                if(cme != null)
                {
                    for(int jj = 0; jj < cme.rowEntries.Length; ++jj)
                    {
                        int rowIndex = System.Array.FindIndex(_toCombine, arg => arg.Name == cme.rowEntries[jj].combineRowName);
                        _toCombine[rowIndex].GameObject = cme.rowEntries[jj].mesh;
                        refreshIndices [rowIndex] = 1;
                    }
                }
            }
        }
        //Step 0
        //Remove any preview nodes
        if(_toDelete != null)
        {
            Transform deleteTransform = gameObject.transform.Find(_toDelete);
            deleteTransform.gameObject.SetActive(false);
        }
        
        //Step 1
        // Remove old mesh components
        GameObject skeleton = gameObject;
        SkinnedMeshRenderer old_smr = skeleton.GetComponent ("SkinnedMeshRenderer") as SkinnedMeshRenderer;
        if (old_smr != null) {
            DestroyImmediate (old_smr);
        }
        
        //Step 2
        //first we need to try and download each of the Assetbundles for this dude
        
        //Step 3
        //Grab each piece and reskin it to our skeleton
        //Vector3 position = skeleton.transform.position;
        
        //List<CombineInstance> combineInstances = new List<CombineInstance> ();
        //List<Material> materials = new List<Material> ();
        List<Transform> bones = new List<Transform> ();
        Transform[] transforms = skeleton.GetComponentsInChildren<Transform> ();
        
        //bool should_unload_unused_items = false;
        for (int i = 0; i < _toCombine.Length; ++i) {
            bones = new List<Transform> ();

            if(refreshIndices[i] == 0) continue;
            if(_toCombine[i].GameObject == null) continue;
            if(_toCombine[i].CombinedMesh != null) Destroy(_toCombine[i].CombinedMesh);
            
            GameObject customization_object = (GameObject)GameObject.Instantiate(_toCombine[i].GameObject);
            customization_object.transform.parent = gameObject.transform;
            customization_object.SetActive(true);
            
            if (customization_object == null) {
                EB.Debug.LogWarning ("CombineMesh: customization_object is null. type = {0}", _toCombine[i].Name);
                continue;
            }
            
            SkinnedMeshRenderer[] smrs = customization_object.GetComponentsInChildren<SkinnedMeshRenderer> ();
            
            if (smrs == null || smrs.Length == 0) {
                continue;
            }


            for(int j = 0; j < smrs.Length; j++)
            {
                if(_toCombine[i].OverrideMaterial.Length > 0)
                {
                    if(smrs[j].materials.Length == _toCombine[i].OverrideMaterial.Length)
                    {
                        smrs[j].materials = _toCombine[i].OverrideMaterial;
                    }
                    else
                    {
                        smrs[j].material = _toCombine[i].OverrideMaterial[0];
                    }
                }

                smrs[j].receiveShadows = false;
            }
            
            //set customizatioin color;
            ColorCustomization customization = customization_object.GetComponent<ColorCustomization>();


            if(customization != null)
            {
                //now we use this component to check whether current gameobject is hero instance, not a good way.
                if(GetComponent<HeroEquipmentDataLookup>() != null)
                {
                    customization.m_UseHeroColorPreset = true;
                    int skinColorIndex = 0;
                    DataLookupsCache.Instance.SearchIntByID("heroStats.skinColorIndex", out skinColorIndex);
                    customization.m_SkinColorIndex = skinColorIndex - 1;
                    
                    int hairColorIndex = 0;
                    DataLookupsCache.Instance.SearchIntByID("heroStats.hairColorIndex", out hairColorIndex);
                    customization.m_HairColorIndex = hairColorIndex - 1;
                    
                    int eyeColorIndex = 0;
                    DataLookupsCache.Instance.SearchIntByID("heroStats.eyesColorIndex", out eyeColorIndex);
                    customization.m_EyeColorIndex = eyeColorIndex - 1;

                    if(!string.IsNullOrEmpty(equipmentDataId))
                    {
                        int colorIndex = 0;
                        DataLookupsCache.Instance.SearchIntByID(string.Format("{0}.equipmentColorIndex", equipmentDataId), out colorIndex);
                        customization.m_EquipmentColorIndex = colorIndex - 1;
                    }
                }
                customization.ApplyColor();
            }

            SkinnedMeshRenderer smr = smrs [0];
            Transform root = null;
            
            //reskin this thing to the new skeleton
            foreach (Transform bone in smr.bones) {
                foreach (Transform transform in transforms) {
                    if(transform.name == smr.rootBone.name)
                        root = transform;
                    
                    if (transform.name != bone.name)
                        continue;
                    bones.Add (transform);
                    break;
                }
            }
            smr.bones = bones.ToArray();
            smr.rootBone = root;
            smr.transform.parent = gameObject.transform;
            _toCombine[i].CombinedMesh = smr.gameObject;
            GameObject.Destroy (customization_object);
        }
        
        //Step 4
        //Combine meshes using the same material/shader combo into one
        if (_CombineTo1Draw) {
            CombineMeshes1Draw (meshes);
        }
        else
        {
            //CombineMeshesMultiDraw(meshes);
        }
        gameObject.SendMessage("ForceUpdateColorScale", SendMessageOptions.DontRequireReceiver);

        /*
        //Raise character mesh update event.
        if(_meshUpdateEvent == null)
        {
            _meshUpdateEvent = new CharacterMeshUpdateEvent(gameObject);
        }
        EventManager.instance.Raise(_meshUpdateEvent);
        */
    }
    
    public void SetToCombineGameObjectByRowName(string rowName, GameObject gameObject, string equipmentDataId = null) {
        int rowIndex = System.Array.FindIndex(_toCombine, arg => arg.Name == rowName);
        _toCombine[rowIndex].GameObject = gameObject;
        Refresh(rowIndex, equipmentDataId);
    }

    public void UpdateEquipmentColor(string rowName, int colorIndex)
    {
        if(_toCombine == null)
        {
            return;
        }

        for(int i = _toCombine.Length - 1; i >= 0; i--)
        {
            if(_toCombine[i].Name == rowName)
            {
                ColorCustomization.ApplyEquipmentColor(_toCombine[i].CombinedMesh, colorIndex);
                break;
            }
        }
    }
    
    private GameObject[] meshes {
        get { return _toCombine.Select<ToCombineRow, GameObject>(arg => arg.CombinedMesh).ToArray(); }
    }

    void CombineMeshesMultiDraw(GameObject[] meshes)
    {
        GameObject skeleton = gameObject;
        //Vector3 position = skeleton.transform.position;
        
        //________________________________________
        // Instantiate Meshes and Collect Pieces
        //________________________________________
        List<CombineInstance> combineInstances = new List<CombineInstance> ();
        List<Material> materials = new List<Material> ();
        List<Transform> bones = new List<Transform> ();
        Transform[] transforms = skeleton.GetComponentsInChildren<Transform> ();
        
        //bool should_unload_unused_items = false;
        int to_combine_len = _toCombine.Length;
        for (int i = 0; i < to_combine_len; ++i) {

            SkinnedMeshRenderer[] smrs = _toCombine[i].CombinedMesh.GetComponents<SkinnedMeshRenderer> ();
            
            if (smrs == null) {
                continue;
            }
            
            SkinnedMeshRenderer smr = smrs [0];
            
            if (smr == null) {
                continue;
            }
            
            materials.Add (smr.materials [0]);
            
            // Add mesh if needed
            if(smr.sharedMesh != null) {
                for (int sub = 0; sub < smr.sharedMesh.subMeshCount; sub++) {
                    CombineInstance ci = new CombineInstance ();
                    ci.mesh = smr.sharedMesh;
                    ci.subMeshIndex = sub;
                    combineInstances.Add (ci);
                }
                
                foreach (Transform bone in smr.bones) {
                    foreach (Transform transform in transforms) {
                        if (transform.name != bone.name)
                            continue;
                        bones.Add (transform);
                        break;
                    }
                }
            }
            
            GameObject.Destroy (_toCombine[i].CombinedMesh);
        }
        
        // Combine all meshes into one
        SkinnedMeshRenderer combined_mesh = skeleton.AddComponent<SkinnedMeshRenderer> ();
        SetRequiredCustomizationMeshProperties (combined_mesh);
        combined_mesh.sharedMesh = new Mesh ();
        combined_mesh.sharedMesh.CombineMeshes (combineInstances.ToArray (), false, false);
        combined_mesh.bones = bones.ToArray ();
        combined_mesh.rootBone = gameObject.transform;
        combined_mesh.materials = materials.ToArray ();


        //bomber.m_customizations.m_combined_mesh = combined_mesh;
        /*if (combined_mesh.materials.Length > 2) {
            bomber.m_customizations.m_body_material = combined_mesh.materials [0];
            SetDefaultMaterialLightingCubeMap (bomber.m_customizations.m_body_material);
            
            bomber.m_customizations.m_face_material = combined_mesh.materials [1];
            SetDefaultMaterialLightingCubeMap (bomber.m_customizations.m_face_material);
            
            bomber.m_customizations.m_hair_material = combined_mesh.materials [2];
            SetDefaultMaterialLightingCubeMap (bomber.m_customizations.m_hair_material);
        }  else if (combined_mesh.materials.Length > 1) {
            bomber.m_customizations.m_body_material = combined_mesh.materials [0];
            SetDefaultMaterialLightingCubeMap (bomber.m_customizations.m_body_material);
            
            bomber.m_customizations.m_face_material = combined_mesh.materials [1];
            SetDefaultMaterialLightingCubeMap (bomber.m_customizations.m_face_material);
            
            bomber.m_customizations.m_hair_material = combined_mesh.materials [1];
            SetDefaultMaterialLightingCubeMap (bomber.m_customizations.m_hair_material);
        }  else if (combined_mesh.materials.Length > 0) {
            bomber.m_customizations.m_body_material = combined_mesh.materials [0];
            SetDefaultMaterialLightingCubeMap (bomber.m_customizations.m_body_material);
            
            bomber.m_customizations.m_face_material = combined_mesh.materials [0];
            SetDefaultMaterialLightingCubeMap (bomber.m_customizations.m_face_material);
            
            bomber.m_customizations.m_hair_material = combined_mesh.materials [0];
            SetDefaultMaterialLightingCubeMap (bomber.m_customizations.m_hair_material);
        }
        bomber.m_customizations.m_combined_material = bomber.m_customizations.m_body_material;*/
        
        
        // If necessary remove unused items from memory
        //if(should_unload_unused_items)
        //{
        //	RequestUnloadUnusedItem();
        //}
    }
    
    /*void SetBomberCustomizations_1DrawCall (Bomber bomber, int avatar_id, 	
                                                uint body_id, List<DataObjects.InventoryItemColor> body_color_map, 
                                                uint face_id, List<DataObjects.InventoryItemColor> face_color_map, 
                                                uint hair_id, List<DataObjects.InventoryItemColor> hair_color_map)*/
    void CombineMeshes1Draw(GameObject[] meshes)
    {
        UnityEngine.Profiling.Profiler.BeginSample ("SetBomberCustomizations_1DrawCall");
        
        //int customization_material_renderer_id = avatar_id;
        
        //if (bomber.m_customizations == null) {
        //	bomber.m_customizations = new CustomizationManager ();	
        //}
        
        //GameObject bomber_skeleton = bomber.m_body;
        GameObject bomber_skeleton = gameObject;
        //CustomizationLoader loader = GetCustomizationLoader (bomber.m_gender);
        //Vector3 position = bomber_skeleton.transform.position;
        
        // Remove old mesh components
        SkinnedMeshRenderer old_smr = bomber_skeleton.GetComponent ("SkinnedMeshRenderer") as SkinnedMeshRenderer;
        if (old_smr != null) {
            DestroyImmediate (old_smr);
        }
        
        //  Put Customization IDs into an array for easy iteration
        //uint[] customization_ids = new uint[3];
        //customization_ids [CustomizationLoader.CUSTOMIZATION_TYPE_BODY] = body_id;
        //customization_ids [CustomizationLoader.CUSTOMIZATION_TYPE_FACE] = face_id;
        //customization_ids [CustomizationLoader.CUSTOMIZATION_TYPE_HAIR] = hair_id;
        
        // Test whether you need a face or not
        //bool needs_face = (!BomberFactory.ShouldHairRemoveFace (hair_id) && !bomber.m_is_zombie) || bomber.m_is_king;
        //bool needs_face = true;
        
        //________________________________________
        // Instantiate Meshes and Collect Pieces
        //________________________________________
        //List<Material> materials = new List<Material>();
        Transform[] skeleton_bones = bomber_skeleton.GetComponentsInChildren<Transform> ();
        
        // Lists for all components of combined mesh
        UnityEngine.Profiling.Profiler.BeginSample ("SetBomberCustomizations_1DrawCall - List Alocation");
        //List<Vector3> combined_verts = new List<Vector3> ();
        //List<Vector3> combined_normals = new List<Vector3> ();
        //List<Vector2> combined_uvs = new List<Vector2> ();
        //List<int> combined_tris = new List<int> ();
        //List<Transform> combined_bones = new List<Transform> ();
        //List<BoneWeight> combined_bone_weights = new List<BoneWeight> ();
        //List<Matrix4x4> combined_bind_poses = new List<Matrix4x4> ();
        UnityEngine.Profiling.Profiler.EndSample ();
        
        // Load and stash the 3 customization pieces so we can know sizes of everything
        int combined_vert_count = 0;
        int combined_normal_count = 0;
        int combined_uv_count = 0;
        int combined_tri_count = 0;
        int combined_bone_count = 0;
        int combined_bone_weight_count = 0;
        int combined_bind_pose_count = 0;
        GameObject[] customization_objects = new GameObject[meshes.Length];
        bool should_unload_unused_items = false;
        for (int i = 0; i < meshes.Length; ++i) {
            UnityEngine.Profiling.Profiler.BeginSample ("SetBomberCustomizations_1DrawCall - Load and Count");
            
            // Instantiate
            //uint customization_id = customization_ids [i];
            bool customization_was_removed_from_cache = false;
            customization_objects [i] = meshes[i];
            should_unload_unused_items |= customization_was_removed_from_cache; // should free memory if customization was removed
            if (customization_objects [i] == null) {
                //Debug.LogWarning ("BomberFactory::SetBomberCustomizations: customization_object is null. type = " + i.ToString () + ", customization_id = " + customization_id.ToString ());
                continue;
            }
            
            // check if it should be added
            //if (i == CustomizationLoader.CUSTOMIZATION_TYPE_FACE && needs_face == false) {
            //	continue;
            //}
            
            // Get skinned mesh renderer
            SkinnedMeshRenderer[] smrs = customization_objects [i].GetComponentsInChildren<SkinnedMeshRenderer> ();
            if (smrs == null || smrs [0] == null || smrs [0].sharedMesh == null) {
                //GameObject.Destroy (customization_objects [i]);
                customization_objects [i] = null;
                continue;
            }
            SkinnedMeshRenderer smr = smrs [0];
            Mesh shared_mesh = smr.sharedMesh;
            
            // Count the mesh properties
            combined_vert_count += shared_mesh.vertices.Length;
            combined_normal_count += shared_mesh.normals.Length;
            combined_uv_count += shared_mesh.uv.Length;
            combined_tri_count += shared_mesh.triangles.Length;
            combined_bone_weight_count += shared_mesh.boneWeights.Length;
            combined_bind_pose_count += shared_mesh.bindposes.Length;
            
            //combined_bone_count += smr.bones.Length;
            Transform[] shared_mesh_bones = smr.bones;
            int bones_length = shared_mesh_bones.Length;
            int skeleton_bones_length = skeleton_bones.Length;
            for (int j = 0; j < bones_length; ++j) {
                Transform bone = shared_mesh_bones [j];
                for (int k = 0; k < skeleton_bones_length; ++k) {
                    Transform skeleton_bone = skeleton_bones [k];
                    if (skeleton_bone.name != bone.name) {
                        continue;
                    }
                    combined_bone_count++;
                    break;
                }
            }
            
            
            UnityEngine.Profiling.Profiler.EndSample ();
        }
        
        // Store materials for all customizations
        Material[] customization_materials = new Material[meshes.Length];
        
        // Adjust parameters as we loop through meshes
        //const float total_texture_size = 768.0f;
        //const float body_texture_size = 512.0f;
        //const float face_texture_size = 256.0f;
        //const float hair_texture_size = 256.0f;
        //Vector2[] uv_offsets = {
        //	new Vector2 (0, 0),
        //	new Vector2 (body_texture_size / total_texture_size, 0),
        //	new Vector2 (0, body_texture_size / total_texture_size)
        //} ;
        //float[] uv_scales = {
        //	body_texture_size / total_texture_size,
        //	face_texture_size / total_texture_size,
        //	hair_texture_size / total_texture_size
        //} ; 
        
        // Build combined mesh
        UnityEngine.Profiling.Profiler.BeginSample ("SetBomberCustomizations_1DrawCall - PreAlloc");
        Vector3[] combined_verts = new Vector3[combined_vert_count];
        Vector3[] combined_normals = new Vector3[combined_normal_count];
        Vector2[] combined_uvs = new Vector2[combined_uv_count];
        int[] combined_tris = new int[combined_tri_count];
        Transform[] combined_bones = new Transform[combined_bone_count];
        BoneWeight[] combined_bone_weights = new BoneWeight[combined_bone_weight_count];
        Matrix4x4[] combined_bind_poses = new Matrix4x4[combined_bind_pose_count];
        int normal_id_offset = 0;
        int vert_id_offset = 0;
        int uv_id_offset = 0;
        int tri_id_offset = 0;
        int bone_id_offset = 0;
        int bone_weight_id_offset = 0;
        int bind_pose_id_offset = 0;
        UnityEngine.Profiling.Profiler.EndSample ();
        for (int i = 0; i < customization_objects.Length; ++i) {
            //uint customization_id = customization_ids[i];
            
            if(customization_objects [i] == null)
            {
                continue;
            }
            
            SkinnedMeshRenderer[] smrs = customization_objects[i].GetComponentsInChildren<SkinnedMeshRenderer> ();
            SkinnedMeshRenderer smr = smrs [0];
            Mesh shared_mesh = smr.sharedMesh;
            
            UnityEngine.Profiling.Profiler.BeginSample ("SetBomberCustomizations_1DrawCall - Set Material");
            // Add the material to array to be put in avatar customization structure
            customization_materials[i] = smr.sharedMaterials[0];
            //mmcmanus
            //customization_materials [i] = BomberCustomizationMaterialRenderManager.m_Instance.m_Renderers[customization_material_renderer_id].m_RenderPlanes [i].sharedMaterial;
            // Send the material to the combined material renderer
            Material shared_mesh_material = smr.sharedMaterial;
            customization_materials [i].SetTexture ("_MainTex", shared_mesh_material.GetTexture ("_MainTex"));
            customization_materials [i].SetTexture ("_TintTex", shared_mesh_material.GetTexture ("_TintTex"));
            customization_materials [i].SetColor ("_Tint1", shared_mesh_material.GetColor ("_Tint1"));
            customization_materials [i].SetColor ("_Tint2", shared_mesh_material.GetColor ("_Tint2"));
            customization_materials [i].SetColor ("_Tint3", shared_mesh_material.GetColor ("_Tint3"));
            customization_materials [i].SetColor ("_Tint4", shared_mesh_material.GetColor ("_Tint4"));
            UnityEngine.Profiling.Profiler.EndSample ();
            
            // Store some data in custmization
            UnityEngine.Profiling.Profiler.BeginSample ("SetBomberCustomizations_1DrawCall - Loop End");
            /*switch (i) {
            case CustomizationLoader.CUSTOMIZATION_TYPE_BODY:
            {
                if (bomber.m_customizations.m_body_customization == null) {
                    bomber.m_customizations.m_body_customization = new CustomizationBodyData ();
                }
                CustomizationBody body_source = customization_objects [i].GetComponent<CustomizationBody> ();
                if (body_source != null) {
                    bomber.m_customizations.m_body_customization.Copy (body_source);
                    bomber.m_customizations.m_body_id = customization_id;
                }
                break;
            }
            case CustomizationLoader.CUSTOMIZATION_TYPE_FACE:
            {
                if (bomber.m_customizations.m_face_customization == null) {
                    bomber.m_customizations.m_face_customization = new CustomizationFaceData ();
                }
                CustomizationFace face_source = customization_objects [i].GetComponent<CustomizationFace> ();
                if (face_source != null) {
                    bomber.m_customizations.m_face_customization.Copy (face_source);
                    bomber.m_customizations.m_face_id = customization_id;
                }
                break;
            }
            case CustomizationLoader.CUSTOMIZATION_TYPE_HAIR:
            {
                if (bomber.m_customizations.m_hair_customization == null) {
                    bomber.m_customizations.m_hair_customization = new CustomizationHairData ();
                }
                CustomizationHair hair_source = customization_objects [i].GetComponent<CustomizationHair> ();
                if (hair_source != null) {
                    bomber.m_customizations.m_hair_customization.Copy (hair_source);
                    bomber.m_customizations.m_hair_id = customization_id;
                }
                break;
            }
            } */
            
            // check if it should be added
            // Note: This check must be done after the customization_materials and bomber.m_customizations are set up
            //       so that the ApplyColors call below has the information it needs to color the materials
            //if (i == CustomizationLoader.CUSTOMIZATION_TYPE_FACE && needs_face == false) {
            //	GameObject.Destroy (customization_objects [i]);
            //	customization_objects [i] = null;
            //	continue;
            //}
            
            // Collect Verts
            UnityEngine.Profiling.Profiler.BeginSample ("SetBomberCustomizations_1DrawCall - PreVerts");
            Vector3[] shared_mesh_verts = shared_mesh.vertices;
            int vert_length = shared_mesh_verts.Length;
            UnityEngine.Profiling.Profiler.EndSample ();
            UnityEngine.Profiling.Profiler.BeginSample ("SetBomberCustomizations_1DrawCall - Verts");
            for (int j = 0; j < vert_length; ++j) {
                combined_verts [j + vert_id_offset].Set (shared_mesh_verts [j].x, shared_mesh_verts [j].y, shared_mesh_verts [j].z);
            }
            UnityEngine.Profiling.Profiler.EndSample ();
            
            // Collect Normals
            UnityEngine.Profiling.Profiler.BeginSample ("SetBomberCustomizations_1DrawCall - PreNormals");
            Vector3[] shared_mesh_normals = shared_mesh.normals;
            int normal_length = shared_mesh_normals.Length;
            UnityEngine.Profiling.Profiler.EndSample ();
            UnityEngine.Profiling.Profiler.BeginSample ("SetBomberCustomizations_1DrawCall - Normals");
            for (int j = 0; j < normal_length; ++j) {
                combined_normals [j + normal_id_offset].Set (shared_mesh_normals [j].x, shared_mesh_normals [j].y, shared_mesh_normals [j].z);
            }
            UnityEngine.Profiling.Profiler.EndSample ();
            
            // Collect UVs
            UnityEngine.Profiling.Profiler.BeginSample ("SetBomberCustomizations_1DrawCall - PreUVs");
            Vector2[] shared_mesh_uvs = shared_mesh.uv;
            int uv_length = shared_mesh_uvs.Length;
            UnityEngine.Profiling.Profiler.EndSample ();
            UnityEngine.Profiling.Profiler.BeginSample ("SetBomberCustomizations_1DrawCall - UVs");
            for (int j = 0; j < uv_length; ++j) {
                //mmcmanus
                //combined_uvs [j + uv_id_offset].Set (shared_mesh_uvs [j].x * uv_scales [i] + uv_offsets [i].x,
                //                                     shared_mesh_uvs [j].y * uv_scales [i] + uv_offsets [i].y);
            }
            UnityEngine.Profiling.Profiler.EndSample ();
            
            // Collect Tris
            UnityEngine.Profiling.Profiler.BeginSample ("SetBomberCustomizations_1DrawCall - PreTris");
            int[] shared_mesh_tris = shared_mesh.triangles;
            int tris_length = shared_mesh_tris.Length;
            UnityEngine.Profiling.Profiler.EndSample ();
            UnityEngine.Profiling.Profiler.BeginSample ("SetBomberCustomizations_1DrawCall - Tris");
            for (int j = 0; j < tris_length; ++j) {
                combined_tris [j + tri_id_offset] = shared_mesh_tris [j] + vert_id_offset;
            }
            UnityEngine.Profiling.Profiler.EndSample ();
            
            // Collect Bones
            UnityEngine.Profiling.Profiler.BeginSample ("SetBomberCustomizations_1DrawCall - PreBones");
            Transform[] shared_mesh_bones = smr.bones;
            int bones_length = shared_mesh_bones.Length;
            int skeleton_bones_length = skeleton_bones.Length;
            UnityEngine.Profiling.Profiler.EndSample ();
            UnityEngine.Profiling.Profiler.BeginSample ("SetBomberCustomizations_1DrawCall - Bones");
            for (int j = 0; j < bones_length; ++j) {
                Transform bone = shared_mesh_bones [j];
                string bone_name = bone.name;
                for (int k = 0; k < skeleton_bones_length; ++k) {
                    Transform skeleton_bone = skeleton_bones [k];
                    if (skeleton_bone.name != bone_name) {
                        continue;
                    }
                    combined_bones [j + bone_id_offset] = skeleton_bone;
                    break;
                }
            }
            UnityEngine.Profiling.Profiler.EndSample ();
            
            // CollectionBase Bone Weights
            UnityEngine.Profiling.Profiler.BeginSample ("SetBomberCustomizations_1DrawCall - PreBoneWeights");
            BoneWeight[] shared_mesh_bone_weights = shared_mesh.boneWeights;
            int bone_weights_length = shared_mesh_bone_weights.Length;
            UnityEngine.Profiling.Profiler.EndSample ();
            UnityEngine.Profiling.Profiler.BeginSample ("SetBomberCustomizations_1DrawCall - Bone Weights");
            for (int j = 0; j < bone_weights_length; ++j) {
                
                combined_bone_weights [j + bone_weight_id_offset].boneIndex0 = shared_mesh_bone_weights [j].boneIndex0 + bone_id_offset;
                combined_bone_weights [j + bone_weight_id_offset].weight0 = shared_mesh_bone_weights [j].weight0;
                
                combined_bone_weights [j + bone_weight_id_offset].boneIndex1 = shared_mesh_bone_weights [j].boneIndex1 + bone_id_offset;
                combined_bone_weights [j + bone_weight_id_offset].weight1 = shared_mesh_bone_weights [j].weight1;
                
                combined_bone_weights [j + bone_weight_id_offset].boneIndex2 = shared_mesh_bone_weights [j].boneIndex2 + bone_id_offset;
                combined_bone_weights [j + bone_weight_id_offset].weight2 = shared_mesh_bone_weights [j].weight2;
                
                combined_bone_weights [j + bone_weight_id_offset].boneIndex3 = shared_mesh_bone_weights [j].boneIndex3 + bone_id_offset;
                combined_bone_weights [j + bone_weight_id_offset].weight3 = shared_mesh_bone_weights [j].weight3;
                
            }
            UnityEngine.Profiling.Profiler.EndSample ();
            
            // Collect Bind Poses
            UnityEngine.Profiling.Profiler.BeginSample ("SetBomberCustomizations_1DrawCall - PreBindPoses");
            Matrix4x4[] shared_mesh_bind_poses = shared_mesh.bindposes;
            int bind_poses_length = shared_mesh_bind_poses.Length;
            UnityEngine.Profiling.Profiler.EndSample ();
            UnityEngine.Profiling.Profiler.BeginSample ("SetBomberCustomizations_1DrawCall - BindPoses");
            for (int j = 0; j < bind_poses_length; ++j) {
                Matrix4x4 mm = shared_mesh_bind_poses [j];
                combined_bind_poses [j + bind_pose_id_offset].m00 = mm.m00;
                combined_bind_poses [j + bind_pose_id_offset].m01 = mm.m01;
                combined_bind_poses [j + bind_pose_id_offset].m02 = mm.m02;
                combined_bind_poses [j + bind_pose_id_offset].m03 = mm.m03;
                combined_bind_poses [j + bind_pose_id_offset].m10 = mm.m10;
                combined_bind_poses [j + bind_pose_id_offset].m11 = mm.m11;
                combined_bind_poses [j + bind_pose_id_offset].m12 = mm.m12;
                combined_bind_poses [j + bind_pose_id_offset].m13 = mm.m13;	
                combined_bind_poses [j + bind_pose_id_offset].m20 = mm.m20;
                combined_bind_poses [j + bind_pose_id_offset].m21 = mm.m21;
                combined_bind_poses [j + bind_pose_id_offset].m22 = mm.m22;
                combined_bind_poses [j + bind_pose_id_offset].m23 = mm.m23;		
                combined_bind_poses [j + bind_pose_id_offset].m30 = mm.m30;
                combined_bind_poses [j + bind_pose_id_offset].m31 = mm.m31;
                combined_bind_poses [j + bind_pose_id_offset].m32 = mm.m32;
                combined_bind_poses [j + bind_pose_id_offset].m33 = mm.m33;	
            }
            UnityEngine.Profiling.Profiler.EndSample ();
            
            //GameObject.Destroy (customization_objects [i]);
            customization_objects [i] = null;
            
            vert_id_offset += vert_length;
            normal_id_offset += normal_length;
            uv_id_offset += uv_length;
            tri_id_offset += tris_length;
            bone_id_offset += bones_length;
            bone_weight_id_offset += bone_weights_length;
            bind_pose_id_offset += bind_poses_length;
            UnityEngine.Profiling.Profiler.EndSample ();
        }
        
        // Create a combined mesh
        UnityEngine.Profiling.Profiler.BeginSample ("SetBomberCustomizations_1DrawCall - Combine Mesh");
        
        Mesh combined_mesh = new Mesh ();
        combined_mesh.vertices = combined_verts;
        combined_mesh.normals = combined_normals;
        combined_mesh.uv = combined_uvs;
        combined_mesh.triangles = combined_tris;
        combined_mesh.boneWeights = combined_bone_weights;
        combined_mesh.bindposes = combined_bind_poses;
        
        // Create a combined mesh renderer
        SkinnedMeshRenderer combined_mesh_renderer = bomber_skeleton.AddComponent<SkinnedMeshRenderer> ();
        combined_mesh_renderer.bones = combined_bones;
        //mmcmanus
        //combined_mesh_renderer.sharedMaterial = BomberCustomizationMaterialRenderManager.m_Instance.m_Renderers [customization_material_renderer_id].m_ResultingMaterial; 
        combined_mesh_renderer.sharedMesh = combined_mesh;
        //SetDefaultMaterialLightingCubeMap (combined_mesh_renderer.sharedMaterial);
        SetRequiredCustomizationMeshProperties (combined_mesh_renderer);
        //bomber.m_customizations.m_combined_material = combined_mesh_renderer.sharedMaterial;
        
        // Store customization materials
        //bomber.m_customizations.m_combined_mesh = combined_mesh_renderer;
        UnityEngine.Profiling.Profiler.EndSample ();
        
        /*if (customization_materials.Length > 2) {
            bomber.m_customizations.m_body_material = customization_materials [0];
            bomber.m_customizations.m_face_material = customization_materials [1];
            bomber.m_customizations.m_hair_material = customization_materials [2];
        }  else if (customization_materials.Length > 1) {
            bomber.m_customizations.m_body_material = customization_materials [0];
            bomber.m_customizations.m_face_material = customization_materials [1];
            bomber.m_customizations.m_hair_material = customization_materials [1];
        }  else if (customization_materials.Length > 0) {
            bomber.m_customizations.m_body_material = customization_materials [0];
            bomber.m_customizations.m_face_material = customization_materials [0];
            bomber.m_customizations.m_hair_material = customization_materials [0];
        } */
        
        // Apply Colors
        //bomber.m_customizations.m_body_color_map = body_color_map;
        //bomber.m_customizations.m_face_color_map = face_color_map;
        //bomber.m_customizations.m_hair_color_map = hair_color_map;
        //bomber.m_customizations.ApplyColors ();
        
        // Actvate rendering for 1 frame
        //mmcmanus
        //BomberCustomizationMaterialRenderManager.m_Instance.m_Renderers[customization_material_renderer_id].Activate ();
        
        for (int i = 0; i < meshes.Length; i++) {
            GameObject.Destroy(meshes[i]);
        }
        
        // If necessary remove unused items from memory
        if(should_unload_unused_items)
        {
            //RequestUnloadUnusedItem();
        }
        
        UnityEngine.Profiling.Profiler.EndSample ();
        
    }
    
    static void SetRequiredCustomizationMeshProperties (SkinnedMeshRenderer mesh)
    {
        //mesh.castShadows = false;
        mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mesh.receiveShadows = false;
        mesh.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        mesh.updateWhenOffscreen = true;
    }
}

