using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEngine.Profiling;
using EB.Sequence.Runtime;

public class OptimizeEditorWindow : EditorWindow
{
    private bool _showFilterSetting;
    private bool _showContainSetting;
    private bool _showTextureSetting;
    private bool _showMeshSetting;
    private bool _showParticleSetting;
    private bool _showAnchorSetting;
	private GUIStyle _foldoutStyle;
    private string _dataPath;
    private float _indent = 20f;

    private static OptimizeEditorModel _model;
    private static OptimizeEditorWindow _window;
    private static readonly string _modelPath = "Assets/_GameAssets/Scripts/Editor/Tools/OptimizeEditorModel.asset";

    [MenuItem("Tools/Optimize", false, 200)]
    private static void Init()
    {
        _model = AssetDatabase.LoadAssetAtPath<OptimizeEditorModel>(_modelPath);

        if (_model == null)
        {
            Debug.LogError("OptimizeEditorModel.asset not found!");
            return;
        }

        _window = GetWindow<OptimizeEditorWindow>();
        _window.Show();
    }

    private void OnDestroy()
    {
        EditorUtility.SetDirty(_model);
    }

    private void Awake()
    {
        _foldoutStyle = EditorStyles.foldout;
        _foldoutStyle.fontStyle = FontStyle.Bold;
        _dataPath = Application.dataPath;

        if (_model.filterList == null)
        {
            _model.filterList = new List<string>();
        }
    }

    private void OnGUI()
    {
        _showFilterSetting = EditorGUILayout.Foldout(_showFilterSetting, "过滤目录【不检查这些目录中的内容】", true, _foldoutStyle);

        if (_showFilterSetting)
        {
            BeginContent();
            DrawFilterList();
            EndContent();
        }

        _showContainSetting = EditorGUILayout.Foldout(_showContainSetting, "包含目录【只检查这些目录中的内容，不填表示整个Assets目录】（如果内容也存在过滤目录中，则内容不会被检查）", true, _foldoutStyle);

        if (_showContainSetting)
        {
            BeginContent();
            DrawContainList();
            EndContent();
        }

        _showTextureSetting = EditorGUILayout.Foldout(_showTextureSetting, "纹理检测", true, _foldoutStyle);

        if (_showTextureSetting)
        {
            BeginContent();
            DrawTextureSetting();
            EndContent();
        }

        _showMeshSetting = EditorGUILayout.Foldout(_showMeshSetting, "网格检测", true, _foldoutStyle);

        if (_showMeshSetting)
        {
            BeginContent();
            DrawMeshSetting();
            EndContent();
        }

        _showParticleSetting = EditorGUILayout.Foldout(_showParticleSetting, "粒子系统检测", true, _foldoutStyle);

        if (_showParticleSetting)
        {
            BeginContent();
            DrawParticleSystemSetting();
            EndContent();
        }

        _showAnchorSetting = EditorGUILayout.Foldout(_showAnchorSetting, "UI锚点适配检测", true, _foldoutStyle);

        if (_showAnchorSetting)
        {
	        BeginContent();
	        DrawUiAnchorSetting();
	        EndContent();
        }
	}

    private void BeginContent()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(_indent);
        GUILayout.BeginVertical();
    }

    private void EndContent()
    {
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    #region Filter Setting
    private string _lastFilterPath = "Assets";
    private Vector2 _filterScrollPos;

    private void DrawFilterList()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Folder", GUILayout.Width(100)))
        {
            var path = EditorUtility.OpenFolderPanel("Select Filter Folder", _lastFilterPath, "");
            path = path.Replace(_dataPath, "Assets");

            if (!string.IsNullOrEmpty(path) && !_model.filterList.Contains(path))
            {
                _model.filterList.Add(path);
            }

            _lastFilterPath = path;
        }

        if (GUILayout.Button("Add File", GUILayout.Width(100)))
        {
            var path = EditorUtility.OpenFilePanel("Select Filter File", _lastFilterPath, "");
            path = path.Replace(_dataPath, "Assets");

            if (!string.IsNullOrEmpty(path) && !_model.filterList.Contains(path))
            {
                _model.filterList.Add(path);
            }

            _lastFilterPath = path;
        }

        GUILayout.EndHorizontal();

        if (_model.filterList.Count > 0)
        {
            _filterScrollPos = GUILayout.BeginScrollView(_filterScrollPos, EditorStyles.helpBox);

            for (var i = 0; i < _model.filterList.Count; i++)
            {
                GUILayout.BeginHorizontal(EditorStyles.textField, GUILayout.MinHeight(20f));
                var path = _model.filterList[i];

                if (GUILayout.Button(path, EditorStyles.label))
                {
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(path));
                }

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    _model.filterList.RemoveAt(i);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }
    }

    private bool InFilterList(string path)
    {
        for (var i = 0; i < _model.filterList.Count; i++)
        {
            if (path.StartsWith(_model.filterList[i]))
            {
                return true;
            }
        }

        return false;
    }
    #endregion

    #region Contain Setting
    private string _lastContainPath = "Assets";
    private Vector2 _containScrollPos;

    private void DrawContainList()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Folder", GUILayout.Width(100)))
        {
            var path = EditorUtility.OpenFolderPanel("Select Contain Folder", _lastContainPath, "");
            path = path.Replace(_dataPath, "Assets");

            if (!string.IsNullOrEmpty(path) && !_model.containList.Contains(path))
            {
                _model.containList.Add(path);
            }

            _lastContainPath = path;
        }

        if (GUILayout.Button("Add File", GUILayout.Width(100)))
        {
            var path = EditorUtility.OpenFilePanel("Select Contain File", _lastContainPath, "");
            path = path.Replace(_dataPath, "Assets");

            if (!string.IsNullOrEmpty(path) && !_model.containList.Contains(path))
            {
                _model.containList.Add(path);
            }

            _lastContainPath = path;
        }

        GUILayout.EndHorizontal();

        if (_model.containList.Count > 0)
        {
            _containScrollPos = GUILayout.BeginScrollView(_containScrollPos, EditorStyles.helpBox);

            for (var i = 0; i < _model.containList.Count; i++)
            {
                GUILayout.BeginHorizontal(EditorStyles.textField, GUILayout.MinHeight(20f));
                var path = _model.containList[i];

                if (GUILayout.Button(path, EditorStyles.label))
                {
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(path));
                }

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    _model.containList.RemoveAt(i);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }
    }

    private bool InContainList(string path)
    {
        if (_model.containList == null || _model.containList.Count < 1)
        {
            // 列表为空表示整个Assets目录
            return true;
        }

        for (var i = 0; i < _model.containList.Count; i++)
        {
            if (path.StartsWith(_model.containList[i]))
            {
                return true;
            }
        }

        return false;
    }
    #endregion

    #region Texture Setting
    private enum TextureSetting
    {
        None,
        MemorySize,
        ReadWrite,
        WrapMode,
        FilterMode,
        Compression,
    }

    private TextureSetting _textureSetting = TextureSetting.None;
    private List<string> _textureSettingPaths = new List<string>();
    private Vector2 _textureSettingScrollPos;
    private string _textureMemorySize = "0";
    private List<KeyValuePair<string, string>> _textureSizes = new List<KeyValuePair<string, string>>();

    private void DrawTextureSetting()
    {
        DrawTextureMemorySize();
        DrawTextureReadWrite();
        DrawTextureWrapMode();
        DrawTextureFilterMode();
        DrawTextureCompression();
    }

    private void DrawTextureMemorySize()
    {
        GUILayout.BeginHorizontal(EditorStyles.textField);
        GUILayout.Label("纹理内存占用");

        if (_textureSetting == TextureSetting.MemorySize)
        {
            GUILayout.Label(_textureMemorySize.ToString(), EditorStyles.textField, GUILayout.Width(100));
        }

        if (GUILayout.Button("扫描", GUILayout.Width(50)))
        {
            _textureSetting = TextureSetting.MemorySize;
            _textureSizes.Clear();
            _textureSettingPaths.Clear();
            var guids = AssetDatabase.FindAssets("t:texture");
            var len = guids.Length;
            var size = 0L;

            for (var i = 0; i < len; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);

                if (!InFilterList(path) && InContainList(path))
                {
                    var tex = AssetDatabase.LoadAssetAtPath<Texture>(path);
                    System.Type type = Assembly.Load("UnityEditor.dll").GetType("UnityEditor.TextureUtil");
                    MethodInfo methodInfo = type.GetMethod("GetStorageMemorySize", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
                    //Debug.LogFormat("{0}内存占用：{1}", tex.name, EditorUtility.FormatBytes(Profiler.GetRuntimeMemorySizeLong(tex)));
                    //Debug.LogFormat("{0}硬盘占用：{1}", tex.name, EditorUtility.FormatBytes((int)methodInfo.Invoke(null, new object[] { tex })));

                    if (tex == null)
                    {
                        _textureSizes.Add(new KeyValuePair<string, string>(path, "null"));
                    }
                    else
                    {
                        var bytes = (int)methodInfo.Invoke(null, new object[] { tex });
                        size += bytes;
                        _textureSizes.Add(new KeyValuePair<string, string>(path, EditorUtility.FormatBytes(bytes)));
                    }
                }

                EditorUtility.DisplayProgressBar("扫描中...", path, (float)(i + 1) / len);
            }

            _textureMemorySize = EditorUtility.FormatBytes(size);
            EditorUtility.ClearProgressBar();
        }

        if (_textureSetting == TextureSetting.MemorySize && _textureSizes.Count > 0)
        {
            if (GUILayout.Button("清除", GUILayout.Width(50)))
            {
                _textureMemorySize = "0";
                _textureSizes.Clear();
                _textureSetting = TextureSetting.None;
            }
        }

        GUILayout.EndHorizontal();

        if (_textureSizes.Count > 0)
        {
            _textureSettingScrollPos = GUILayout.BeginScrollView(_textureSettingScrollPos, EditorStyles.helpBox);

            for (var i = 0; i < _textureSizes.Count; i++)
            {
                var path = _textureSizes[i].Key;
                GUILayout.BeginHorizontal(EditorStyles.textField, GUILayout.MinHeight(20f));

                if (GUILayout.Button(path, EditorStyles.label))
                {
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(path));
                }

                if (GUILayout.Button(_textureSizes[i].Value.ToString(), GUILayout.Width(80)))
                {
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }
    }

    private void DrawTextureReadWrite()
    {
        GUILayout.BeginHorizontal(EditorStyles.textField);
        GUILayout.Label("开启Read/Write选项的纹理");

        DrawTextureProperties("t:texture", TextureSetting.ReadWrite,
            (path) =>
            {
                if (Texture2DExtension.IsReadable(path))
                {
                    _textureSettingPaths.Add(path);
                }
            },
            (path) =>
            {
                Texture2DExtension.SetReadable(path);
            }
        );

        GUILayout.EndHorizontal();

        DrawTextureScrollList(TextureSetting.ReadWrite, (path) =>
        {
            Texture2DExtension.SetReadable(path);
        });
    }

    private void DrawTextureWrapMode()
    {
        GUILayout.BeginHorizontal(EditorStyles.textField);
        GUILayout.Label("Wrap模式为Repeat的纹理");

        DrawTextureProperties("t:texture", TextureSetting.WrapMode,
            (path) =>
            {
                if (Texture2DExtension.WrapModeIsRepeat(path))
                {
                    _textureSettingPaths.Add(path);
                }
            },
            (path) =>
            {
                Texture2DExtension.SetWrapMode(path);
            }
        );

        GUILayout.EndHorizontal();

        DrawTextureScrollList(TextureSetting.WrapMode, (path) =>
        {
            Texture2DExtension.SetWrapMode(path);
        });
    }

    private void DrawTextureFilterMode()
    {
        GUILayout.BeginHorizontal(EditorStyles.textField);
        GUILayout.Label("过滤模式为Trilinear的纹理");

        DrawTextureProperties("t:texture", TextureSetting.FilterMode,
            (path) =>
            {
                if (Texture2DExtension.FilterModeIsTrilinear(path))
                {
                    _textureSettingPaths.Add(path);
                }
            },
            (path) =>
            {
                Texture2DExtension.SetFilterMode(path);
            }
        );

        GUILayout.EndHorizontal();

        DrawTextureScrollList(TextureSetting.FilterMode, (path) =>
        {
            Texture2DExtension.SetFilterMode(path);
        });
    }

    private void DrawTextureCompression()
    {
        GUILayout.BeginHorizontal(EditorStyles.textField);
        GUILayout.Label("使用非压缩格式的纹理");

        DrawTextureProperties("t:texture", TextureSetting.Compression,
            (path) =>
            {
                if (Texture2DExtension.IsCompression(path))
                {
                    _textureSettingPaths.Add(path);
                }
            },
            (path) =>
            {
                Texture2DExtension.SetCompression(path);
            }
        );

        GUILayout.EndHorizontal();

        DrawTextureScrollList(TextureSetting.Compression, (path) =>
        {
            Texture2DExtension.SetCompression(path);
        });
    }

    private void DrawTextureProperties(string filter, TextureSetting textureSetting, System.Action<string> addToPaths, System.Action<string> action)
    {
        if (GUILayout.Button("扫描", GUILayout.Width(50)))
        {
            _textureSetting = textureSetting;
            _textureSettingPaths.Clear();
            var guids = AssetDatabase.FindAssets(filter);
            var len = guids.Length;

            for (var i = 0; i < len; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);

                if (!InFilterList(path) && InContainList(path) && addToPaths != null)
                {
                    addToPaths(path);
                }

                EditorUtility.DisplayProgressBar("处理中...", path, (float)(i + 1) / len);
            }

            EditorUtility.ClearProgressBar();

            if (_textureSettingPaths.Count < 1)
            {
                _window.ShowNotification(new GUIContent("无需处理"));
            }
        }

        if (_textureSetting == textureSetting && _textureSettingPaths.Count > 0)
        {
            GUILayout.Label(_textureSettingPaths.Count.ToString(), EditorStyles.textField, GUILayout.Width(50));

            if (GUILayout.Button("全部处理", GUILayout.Width(80)))
            {
                var len = _textureSettingPaths.Count;

                for (var i = 0; i < len; i++)
                {
                    var path = _textureSettingPaths[i];

                    if (action != null)
                    {
                        action(path);
                    }

                    EditorUtility.DisplayProgressBar("处理中...", path, (float)(i + 1) / len);
                }

                EditorUtility.ClearProgressBar();
                _textureSettingPaths.Clear();
            }

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                _textureSettingPaths.Clear();
                _textureSetting = TextureSetting.None;
            }
        }
    }

    private void DrawTextureScrollList(TextureSetting textureSetting, System.Action<string> act)
    {
        if (_textureSetting == textureSetting && _textureSettingPaths.Count > 0)
        {
            _textureSettingScrollPos = GUILayout.BeginScrollView(_textureSettingScrollPos, EditorStyles.helpBox);

            for (var i = 0; i < _textureSettingPaths.Count; i++)
            {
                var path = _textureSettingPaths[i];
                GUILayout.BeginHorizontal(EditorStyles.textField, GUILayout.MinHeight(20f));

                if (GUILayout.Button(path, EditorStyles.label))
                {
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(path));
                }

                if (GUILayout.Button("处理", GUILayout.Width(50)))
                {
                    act?.Invoke(path);
                    _textureSettingPaths.RemoveAt(i);
                }

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    _textureSettingPaths.RemoveAt(i);

                    if (_textureSettingPaths.Count < 1)
                    {
                        _textureSetting = TextureSetting.None;
                    }
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }
    }
    #endregion

    #region Mesh Setting
    private enum MeshSetting
    {
        None,
        ReadWriteEnabled,
        OptimizeMesh,
        ImportBlendShapes,
        ImportVisibility,
        ImportCameras,
        ImportLights,
        ImportNormals,
        ImportTangents,
        ImportColors,
        SubMesh,
        Topology,
    }

    private MeshSetting _meshSetting = MeshSetting.None;
    private List<string> _meshSettingPaths = new List<string>();
    private Vector2 _meshSettingScrollPos;

    private void DrawMeshSetting()
    {
        DrawMeshReadWrite();
        DrawMeshOptimize();
        DrawMeshBlendShapes();
        DrawMeshVisibility();
        DrawMeshCameras();
        DrawMeshLights();
        DrawMeshNormals();
        DrawMeshTangents();
        DrawMeshColors();
        DrawMeshSubMesh();
        DrawMeshTopology();
    }

    private void DrawMeshReadWrite()
    {
        GUILayout.BeginHorizontal(EditorStyles.textField);
        GUILayout.Label("开启Read/Write选项的网格");

        DrawMeshProperties("t:model", MeshSetting.ReadWriteEnabled,
            (path) =>
            {
                if (MeshExtension.IsReadable(path))
                {
                    _meshSettingPaths.Add(path);
                }
            }, 
            (path) =>
            {
                MeshExtension.SetReadable(path);
            }
        );

        GUILayout.EndHorizontal();

        DrawMeshScrollList(MeshSetting.ReadWriteEnabled, (path) =>
        {
            MeshExtension.SetReadable(path);
        });
    }

    private void DrawMeshOptimize()
    {
        GUILayout.BeginHorizontal(EditorStyles.textField);
        GUILayout.Label("未开启OptimizeMesh选项的网格");

        DrawMeshProperties("t:model", MeshSetting.OptimizeMesh,
            (path) =>
            {
                if (!MeshExtension.IsOptimize(path))
                {
                    _meshSettingPaths.Add(path);
                }
            }, 
            (path) =>
            {
                MeshExtension.SetOptimize(path);
            }
        );

        GUILayout.EndHorizontal();

        DrawMeshScrollList(MeshSetting.OptimizeMesh, (path) =>
        {
            MeshExtension.SetOptimize(path);
        });
    }

    private void DrawMeshBlendShapes()
    {
        GUILayout.BeginHorizontal(EditorStyles.textField);
        GUILayout.Label("开启BlendShapes导入选项的网格");

        DrawMeshProperties("t:model", MeshSetting.ImportBlendShapes,
            (path) =>
            {
                if (MeshExtension.IsImportBlendShapes(path))
                {
                    _meshSettingPaths.Add(path);
                }
            }, (path) =>
            {
                MeshExtension.SetImportBlendShapes(path);
            }
        );

        GUILayout.EndHorizontal();

        DrawMeshScrollList(MeshSetting.ImportBlendShapes, (path) =>
        {
            MeshExtension.SetImportBlendShapes(path);
        });
    }

    private void DrawMeshVisibility()
    {
        GUILayout.BeginHorizontal(EditorStyles.textField);
        GUILayout.Label("开启Visibility导入选项的网格");

        DrawMeshProperties("t:model", MeshSetting.ImportVisibility,
            (path) =>
            {
                if (MeshExtension.IsImportVisibility(path))
                {
                    _meshSettingPaths.Add(path);
                }
            }, (path) =>
            {
                MeshExtension.SetImportVisibility(path);
            }
        );

        GUILayout.EndHorizontal();

        DrawMeshScrollList(MeshSetting.ImportVisibility, (path) =>
        {
            MeshExtension.SetImportVisibility(path);
        });
    }

    private void DrawMeshCameras()
    {
        GUILayout.BeginHorizontal(EditorStyles.textField);
        GUILayout.Label("开启Cameras导入选项的网格");

        DrawMeshProperties("t:model", MeshSetting.ImportCameras,
            (path) =>
            {
                if (MeshExtension.IsImportCameras(path))
                {
                    _meshSettingPaths.Add(path);
                }
            }, (path) =>
            {
                MeshExtension.SetImportCameras(path);
            }
        );

        GUILayout.EndHorizontal();

        DrawMeshScrollList(MeshSetting.ImportCameras, (path) =>
        {
            MeshExtension.SetImportCameras(path);
        });
    }

    private void DrawMeshLights()
    {
        GUILayout.BeginHorizontal(EditorStyles.textField);
        GUILayout.Label("开启Lights导入选项的网格");

        DrawMeshProperties("t:model", MeshSetting.ImportLights,
            (path) =>
            {
                if (MeshExtension.IsImportLights(path))
                {
                    _meshSettingPaths.Add(path);
                }
            }, (path) =>
            {
                MeshExtension.SetImportLights(path);
            }
        );

        GUILayout.EndHorizontal();

        DrawMeshScrollList(MeshSetting.ImportLights, (path) =>
        {
            MeshExtension.SetImportLights(path);
        });
    }

    private void DrawMeshNormals()
    {
        GUILayout.BeginHorizontal(EditorStyles.textField);
        GUILayout.Label("包含Normal属性的网格");

        DrawMeshProperties("t:model", MeshSetting.ImportNormals,
            (path) =>
            {
                if (MeshExtension.HasNormals(path))
                {
                    _meshSettingPaths.Add(path);
                }
            }, (path) =>
            {
                MeshExtension.SetModelImporterNormals(path);
            }
        );

        GUILayout.EndHorizontal();

        DrawMeshScrollList(MeshSetting.ImportNormals, (path) =>
        {
            MeshExtension.SetModelImporterNormals(path);
        });
    }

    private void DrawMeshTangents()
    {
        GUILayout.BeginHorizontal(EditorStyles.textField);
        GUILayout.Label("包含Tangent属性的网格");

        DrawMeshProperties("t:model", MeshSetting.ImportTangents,
            (path) =>
            {
                if (MeshExtension.HasTangents(path))
                {
                    _meshSettingPaths.Add(path);
                }
            }, (path) =>
            {
                MeshExtension.SetModelImporterTangents(path);
            }
        );

        GUILayout.EndHorizontal();

        DrawMeshScrollList(MeshSetting.ImportTangents, (path) =>
        {
            MeshExtension.SetModelImporterTangents(path);
        });
    }

    private void DrawMeshColors()
    {
        GUILayout.BeginHorizontal(EditorStyles.textField);
        GUILayout.Label("包含Color属性的网格");

        DrawMeshProperties("t:mesh", MeshSetting.ImportColors,
            (path) =>
            {
                if (MeshExtension.HasColors(path))
                {
                    _meshSettingPaths.Add(path);
                }
            }, (path) =>
            {
                MeshExtension.SetColors(path);
            }
        );

        GUILayout.EndHorizontal();

        DrawMeshScrollList(MeshSetting.ImportColors, (path) =>
        {
            MeshExtension.SetColors(path);
        });
    }

    private void DrawMeshSubMesh()
    {
        GUILayout.BeginHorizontal(EditorStyles.textField);
        GUILayout.Label("包含2个及以上SubMesh的网格");

        DrawMeshProperties("t:mesh", MeshSetting.SubMesh,
            (path) =>
            {
                if (MeshExtension.GetMesh(path).subMeshCount > 1)
                {
                    _meshSettingPaths.Add(path);
                }
            }, (path) =>
            {
                _window.ShowNotification(new GUIContent("暂不提供处理逻辑"));
            }
        );

        GUILayout.EndHorizontal();

        DrawMeshScrollList(MeshSetting.SubMesh, (path) =>
        {
            _window.ShowNotification(new GUIContent("暂不提供处理逻辑"));
        });
    }

    private void DrawMeshTopology()
    {
        GUILayout.BeginHorizontal(EditorStyles.textField);
        GUILayout.Label("包含非三角形的拓扑网格");

        DrawMeshProperties("t:mesh", MeshSetting.Topology,
            (path) =>
            {
                var mi = MeshExtension.GetModelImporter(path);
                
                if (mi != null)
                {
                    mi.keepQuads = true;
                    AssetDatabase.ImportAsset(path);
                }
                //else
                //{
                //    Debug.LogError("path: " + path);
                //}

                if (MeshExtension.GetMesh(path).GetTopology(0) != MeshTopology.Triangles)
                {
                    _meshSettingPaths.Add(path);
                }
                else
                {
                    if (mi != null)
                    {
                        mi.keepQuads = false;
                        AssetDatabase.ImportAsset(path);
                    }
                }
            }, (path) =>
            {
                _window.ShowNotification(new GUIContent("暂不提供处理逻辑"));
                MeshExtension.GetModelImporter(path).keepQuads = false;
                AssetDatabase.ImportAsset(path);
            }
        );

        GUILayout.EndHorizontal();

        DrawMeshScrollList(MeshSetting.Topology, (path) =>
        {
            _window.ShowNotification(new GUIContent("网格拓扑类型：" + MeshExtension.GetMesh(path).GetTopology(0).ToString()));
            MeshExtension.GetModelImporter(path).keepQuads = false;
            AssetDatabase.ImportAsset(path);
        });
    }

    private void DrawMeshProperties(string filter, MeshSetting meshSetting, System.Action<string> addToPaths, System.Action<string> action)
    {
        if (GUILayout.Button("扫描", GUILayout.Width(50)))
        {
            _meshSetting = meshSetting;
            _meshSettingPaths.Clear();
            var guids = AssetDatabase.FindAssets(filter);
            var len = guids.Length;

            for (var i = 0; i < len; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);

                if (!InFilterList(path) && InContainList(path) && addToPaths != null)
                {
                    addToPaths(path);
                }

                EditorUtility.DisplayProgressBar("处理中...", path, (float)(i + 1) / len);
            }

            EditorUtility.ClearProgressBar();

            if (_meshSettingPaths.Count < 1)
            {
                _window.ShowNotification(new GUIContent("无需处理"));
            }
        }

        if (_meshSetting == meshSetting && _meshSettingPaths.Count > 0)
        {
            GUILayout.Label(_meshSettingPaths.Count.ToString(), EditorStyles.textField, GUILayout.Width(50));

            if (GUILayout.Button("全部处理", GUILayout.Width(80)))
            {
                var len = _meshSettingPaths.Count;

                for (var i = 0; i < len; i++)
                {
                    var path = _meshSettingPaths[i];

                    if (action != null)
                    {
                        action(path);
                    }

                    EditorUtility.DisplayProgressBar("处理中...", path, (float)(i + 1) / len);
                }

                EditorUtility.ClearProgressBar();
                _meshSettingPaths.Clear();
            }

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                _meshSettingPaths.Clear();
                _meshSetting = MeshSetting.None;
            }
        }
    }

    private void DrawMeshScrollList(MeshSetting meshSetting, System.Action<string> act)
    {
        if (_meshSetting == meshSetting && _meshSettingPaths.Count > 0)
        {
            _meshSettingScrollPos = GUILayout.BeginScrollView(_meshSettingScrollPos, EditorStyles.helpBox);

            for (var i = 0; i < _meshSettingPaths.Count; i++)
            {
                var path = _meshSettingPaths[i];
                GUILayout.BeginHorizontal(EditorStyles.textField, GUILayout.MinHeight(20f));

                if (GUILayout.Button(path, EditorStyles.label))
                {
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(path));
                }

                if (GUILayout.Button("处理", GUILayout.Width(50)))
                {
                    act?.Invoke(path);
                    _meshSettingPaths.RemoveAt(i);
                }

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    _meshSettingPaths.RemoveAt(i);

                    if (_meshSettingPaths.Count < 1)
                    {
                        _meshSetting = MeshSetting.None;
                    }
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }
    }
    #endregion

    #region ParticleSystem Setting
    private enum ParticleSystemSetting
    {
        None,
        RemoveTrailMaterial,
    }

    private ParticleSystemSetting _particleSystemSetting = ParticleSystemSetting.None;
    private List<string> _particleSystemSettingPaths = new List<string>();
    private Vector2 _particleSystemSettingScrollPos;

    private void DrawParticleSystemSetting()
    {
        DrawParticleSystemRemoveTrailMaterial();
    }

    private void DrawParticleSystemRemoveTrailMaterial()
    {
        GUILayout.BeginHorizontal(EditorStyles.textField);
        GUILayout.Label("去除ParticleSystem中的Trail Material");

        DrawParticleSystemProperties("t:prefab", ParticleSystemSetting.RemoveTrailMaterial,
            (path) =>
            {
                if (HasTrailMaterial(path))
                {
                    _particleSystemSettingPaths.Add(path);
                }
            },
            (path) =>
            {
                RemoveTrailMaterial(path);
            }
        );

        GUILayout.EndHorizontal();

        DrawParticleSystemScrollList(ParticleSystemSetting.RemoveTrailMaterial, (path) =>
        {
            RemoveTrailMaterial(path);
        });
    }

    private bool HasTrailMaterial(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);

        if (go != null)
        {
            var psrs = go.GetComponentsInChildren<ParticleSystemRenderer>(true);

            if (psrs != null)
            {
                var len = psrs.Length;

                for (var i = 0; i < len; i++)
                {
                    if (psrs[i].trailMaterial != null)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void RemoveTrailMaterial(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);

        if (go != null)
        {
            var psrs = go.GetComponentsInChildren<ParticleSystemRenderer>(true);

            if (psrs != null)
            {
                var len = psrs.Length;

                for (var i = 0; i < len; i++)
                {
                    psrs[i].trailMaterial = null;
                }
            }
        }

        AssetDatabase.SaveAssets();
    }

    private void DrawParticleSystemProperties(string filter, ParticleSystemSetting setting, System.Action<string> addToPaths, System.Action<string> action)
    {
        if (GUILayout.Button("扫描", GUILayout.Width(50)))
        {
            _particleSystemSetting = setting;
            _particleSystemSettingPaths.Clear();
            var guids = AssetDatabase.FindAssets(filter);
            var len = guids.Length;

            for (var i = 0; i < len; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);

                if (!InFilterList(path) && InContainList(path) && addToPaths != null)
                {
                    addToPaths(path);
                }

                EditorUtility.DisplayProgressBar("处理中...", path, (float)(i + 1) / len);
            }

            EditorUtility.ClearProgressBar();

            if (_particleSystemSettingPaths.Count < 1)
            {
                _window.ShowNotification(new GUIContent("无需处理"));
            }
        }

        if (_particleSystemSetting == setting && _particleSystemSettingPaths.Count > 0)
        {
            GUILayout.Label(_particleSystemSettingPaths.Count.ToString(), EditorStyles.textField, GUILayout.Width(50));

            if (GUILayout.Button("全部处理", GUILayout.Width(80)))
            {
                var len = _particleSystemSettingPaths.Count;

                for (var i = 0; i < len; i++)
                {
                    var path = _particleSystemSettingPaths[i];

                    if (action != null)
                    {
                        action(path);
                    }

                    EditorUtility.DisplayProgressBar("处理中...", path, (float)(i + 1) / len);
                }

                EditorUtility.ClearProgressBar();
                _particleSystemSettingPaths.Clear();
            }

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                _particleSystemSettingPaths.Clear();
                _particleSystemSetting = ParticleSystemSetting.None;
            }
        }
    }

    private void DrawParticleSystemScrollList(ParticleSystemSetting setting, System.Action<string> act)
    {
        if (_particleSystemSetting == setting && _particleSystemSettingPaths.Count > 0)
        {
            _particleSystemSettingScrollPos = GUILayout.BeginScrollView(_particleSystemSettingScrollPos, EditorStyles.helpBox);

            for (var i = 0; i < _particleSystemSettingPaths.Count; i++)
            {
                var path = _particleSystemSettingPaths[i];
                GUILayout.BeginHorizontal(EditorStyles.textField, GUILayout.MinHeight(20f));

                if (GUILayout.Button(path, EditorStyles.label))
                {
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(path));
                }

                if (GUILayout.Button("处理", GUILayout.Width(50)))
                {
                    act?.Invoke(path);
                    _particleSystemSettingPaths.RemoveAt(i);
                }

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    _particleSystemSettingPaths.RemoveAt(i);

                    if (_particleSystemSettingPaths.Count < 1)
                    {
                        _particleSystemSetting = ParticleSystemSetting.None;
                    }
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }
    }
    #endregion

    #region UIAnchor Setting

    private enum UISetting
	{
		None,
		WidgetAnchorUpdate,
		WidgetAnchorStart,
		WidgetAnchorEnable,
	}

	private UISetting _uiSetting = UISetting.None;
	private List<string> _uiSettingPaths = new List<string>();
	private Vector2 _uiSettingScrollPos;

	private void DrawUiAnchorSetting()
	{
		DrawUiAnchorUpdate();
	}

	private void DrawUiAnchorUpdate()
	{
		GUILayout.BeginHorizontal(EditorStyles.textField);
		GUILayout.Label("开启Anchor.Update选项的UIWidget");

		DrawUiProperties("t:prefab", UISetting.WidgetAnchorUpdate,
			(path) =>
			{
				if (UIControllerExtension.IsReadable(path))
				{
					_uiSettingPaths.Add(path);
				}
			},
			(path, isStart) =>
			{
				UIControllerExtension.SetAvailable(path, isStart);
			}
		);

		GUILayout.EndHorizontal();

		DrawUiScrollList(UISetting.WidgetAnchorUpdate, (path,isStart) =>
		{
			UIControllerExtension.SetAvailable(path, isStart);
		});
	}

	private void DrawUiProperties(string filter, UISetting uiSetting, System.Action<string> addToPaths, System.Action<string, bool> action)
	{
		if (GUILayout.Button("扫描", GUILayout.Width(50)))
		{
			_uiSetting = uiSetting;
			_uiSettingPaths.Clear();
			string uiRootPath = "Assets/_GameAssets/Res/Prefabs/UIPrefabs";
			string[] guids = AssetDatabase.FindAssets( filter, new []{ uiRootPath } );
			int len = guids.Length;

			for (var i = 0; i < len; i++)
			{
				var path = AssetDatabase.GUIDToAssetPath(guids[i]);

				if (!InFilterList(path) && InContainList(path) && addToPaths != null)
				{
					addToPaths(path);
				}

				EditorUtility.DisplayProgressBar("处理中...", path, (float)(i + 1) / len);
			}

			EditorUtility.ClearProgressBar();

			if (_uiSettingPaths.Count < 1)
			{
				_window.ShowNotification(new GUIContent("无需处理"));
			}
		}

		if (_uiSetting == uiSetting && _uiSettingPaths.Count > 0)
		{
			GUILayout.Label(_uiSettingPaths.Count.ToString(), EditorStyles.textField, GUILayout.Width(50));

			int len = _uiSettingPaths.Count;

			if (GUILayout.Button("全改成Start", GUILayout.Width(100)))
			{
				OnUiWidgetAnchorSetting(len, action , true);
			}

			if (GUILayout.Button("全改成Enable", GUILayout.Width(100)))
			{
				OnUiWidgetAnchorSetting(len, action, false);
			}

			if (GUILayout.Button("X", GUILayout.Width(20)))
			{
				_uiSettingPaths.Clear();
				_uiSetting = UISetting.None;
			}
		}
	}

	private void OnUiWidgetAnchorSetting(int len, System.Action<string, bool> action, bool value)
	{
		for (int i = 0; i < len; i++)
		{
			var path = _uiSettingPaths[i];

			action?.Invoke(path, value);

			EditorUtility.DisplayProgressBar("处理中...", path, (float)(i + 1) / len);
		}

		EditorUtility.ClearProgressBar();
		_uiSettingPaths.Clear();

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	private void DrawUiScrollList(UISetting uiSetting, System.Action<string, bool> act)
	{
		if (_uiSetting == uiSetting && _uiSettingPaths.Count > 0)
		{
			_uiSettingScrollPos = GUILayout.BeginScrollView(_uiSettingScrollPos, EditorStyles.helpBox);

			for (var i = 0; i < _uiSettingPaths.Count; i++)
			{
				var path = _uiSettingPaths[i];
				GUILayout.BeginHorizontal(EditorStyles.textField, GUILayout.MinHeight(20f));

				if (GUILayout.Button(path, EditorStyles.label))
				{
					EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(path));
				}

				if (GUILayout.Button("改Start", GUILayout.Width(80)))
				{
					act?.Invoke(path,true);
					_uiSettingPaths.RemoveAt(i);

					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
				}

				if (GUILayout.Button("改Enable", GUILayout.Width(80)))
				{
					act?.Invoke(path, false);
					_uiSettingPaths.RemoveAt(i);

					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
				}

				if (GUILayout.Button("X", GUILayout.Width(20)))
				{
					_uiSettingPaths.RemoveAt(i);

					if (_uiSettingPaths.Count < 1)
					{
						_uiSetting = UISetting.None;
					}
				}

				GUILayout.EndHorizontal();
			}

			GUILayout.EndScrollView();
		}
	}
	#endregion
}