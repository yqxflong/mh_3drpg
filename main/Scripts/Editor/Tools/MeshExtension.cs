using UnityEditor;
using UnityEngine;

public static class MeshExtension {
	public static ModelImporter GetModelImporter(string path) {
		if (!string.IsNullOrEmpty(path)) {
			var modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;
			return modelImporter;
		}

		return null;
	}

	public static Mesh GetMesh(string path)
	{
		if (!string.IsNullOrEmpty(path))
		{
			var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
			return mesh;
		}

		return null;
	}

	public static bool IsReadable(this Mesh mesh) {
		string path = AssetDatabase.GetAssetPath(mesh);
		return IsReadable(path);
	}

	public static bool IsReadable(string path) {
		var modelImporter = GetModelImporter(path);

		if (modelImporter != null) {
			return modelImporter.isReadable;
		}

		return false;
	}

	public static void SetReadable(this Mesh mesh, bool isReadable = false) {
		string path = AssetDatabase.GetAssetPath(mesh);
		SetReadable(path, isReadable);
	}

	public static void SetReadable(string path, bool isReadable = false) {
		var modelImporter = GetModelImporter(path);

		if (modelImporter != null && modelImporter.isReadable != isReadable) {
			modelImporter.isReadable = isReadable;
			AssetDatabase.ImportAsset(path);
		}
	}

	public static bool HasColors(this Mesh mesh) {
		if (mesh.colors32 == null || mesh.colors32.Length < 1) {
			return false;
		}

		return true;
	}

	public static bool HasColors(string path) {
		if (!string.IsNullOrEmpty(path)) {
			var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);

			if (mesh != null) {
				return mesh.HasColors();
			}
		}

		return true;
	}

	public static void SetColors32(this Mesh mesh, Color32[] colors32 = null) {
		string path = AssetDatabase.GetAssetPath(mesh);
		SetColors32(path, colors32);
	}

	public static void SetColors32(string path, Color32[] colors32 = null) {
		if (!string.IsNullOrEmpty(path)) {
			var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);

			if (mesh != null) {
				mesh.colors32 = colors32;
				AssetDatabase.CreateAsset(mesh, path);
				AssetDatabase.ImportAsset(path);
			}
		}
	}

	public static void SetColors(this Mesh mesh, Color[] colors = null) {
		string path = AssetDatabase.GetAssetPath(mesh);
		SetColors(path, colors);
	}

	public static void SetColors(string path, Color[] colors = null) {
		if (!string.IsNullOrEmpty(path)) {
			var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);

			if (mesh != null) {
				mesh.colors = colors;
				AssetDatabase.CreateAsset(mesh, path);
				AssetDatabase.ImportAsset(path);
			}
		}
	}

	public static bool IsOptimize(this Mesh mesh) {
		string path = AssetDatabase.GetAssetPath(mesh);
		return IsOptimize(path);
	}

	public static bool IsOptimize(string path) {
		var modelImporter = GetModelImporter(path);

		if (modelImporter != null) {
			return modelImporter.optimizeMesh;
		}

		return false;
	}

	public static void SetOptimize(this Mesh mesh, bool isOptimize = true) {
		string path = AssetDatabase.GetAssetPath(mesh);
		SetOptimize(path, isOptimize);
	}

	public static void SetOptimize(string path, bool isOptimize = true) {
		var modelImporter = GetModelImporter(path);

		if (modelImporter != null && modelImporter.optimizeMesh != isOptimize) {
			modelImporter.optimizeMesh = isOptimize;
			AssetDatabase.ImportAsset(path);
		}
	}

	public static bool HasNormals(this Mesh mesh) {
		if (mesh.normals == null || mesh.normals.Length < 1) {
			return false;
		}

		return true;
	}

	public static bool HasNormals(string path) {
		var modelImporter = GetModelImporter(path);

		if (modelImporter != null) {
			return modelImporter.importNormals != ModelImporterNormals.None;
		}

		return false;
	}

	public static ModelImporterNormals GetModelImporterNormals(this Mesh mesh) {
		string path = AssetDatabase.GetAssetPath(mesh);
		return GetModelImporterNormals(path);
	}

	public static ModelImporterNormals GetModelImporterNormals(string path) {
		var modelImporter = GetModelImporter(path);

		if (modelImporter != null) {
			return modelImporter.importNormals;
		}

		return ModelImporterNormals.None;
	}

	public static void SetModelImporterNormals(this Mesh mesh, ModelImporterNormals importNormals = ModelImporterNormals.None) {
		string path = AssetDatabase.GetAssetPath(mesh);
		SetModelImporterNormals(path, importNormals);
	}

	public static void SetModelImporterNormals(string path, ModelImporterNormals importNormals = ModelImporterNormals.None) {
		var modelImporter = GetModelImporter(path);

		if (modelImporter != null && modelImporter.importNormals != importNormals) {
			modelImporter.importNormals = importNormals;
			AssetDatabase.ImportAsset(path);
		}
	}

	public static bool HasTangents(this Mesh mesh) {
		if (mesh.tangents == null || mesh.tangents.Length < 1) {
			return false;
		}

		return true;
	}

	public static bool HasTangents(string path) {
		var modelImporter = GetModelImporter(path);

		if (modelImporter != null) {
			return modelImporter.importTangents != ModelImporterTangents.None;
		}

		return false;
	}

	public static ModelImporterTangents GetModelImporterTangents(this Mesh mesh) {
		string path = AssetDatabase.GetAssetPath(mesh);
		return GetModelImporterTangents(path);
	}

	public static ModelImporterTangents GetModelImporterTangents(string path) {
		var modelImporter = GetModelImporter(path);

		if (modelImporter != null) {
			return modelImporter.importTangents;
		}

		return ModelImporterTangents.None;
	}

	public static void SetModelImporterTangents(this Mesh mesh, ModelImporterTangents importTangents = ModelImporterTangents.None) {
		string path = AssetDatabase.GetAssetPath(mesh);
		SetModelImporterTangents(path, importTangents);
	}

	public static void SetModelImporterTangents(string path, ModelImporterTangents importTangents = ModelImporterTangents.None) {
		var modelImporter = GetModelImporter(path);

		if (modelImporter != null && modelImporter.importTangents != importTangents) {
			modelImporter.importTangents = importTangents;
			AssetDatabase.ImportAsset(path);
		}
	}

	public static bool IsImportVisibility(this Mesh mesh) {
		string path = AssetDatabase.GetAssetPath(mesh);
		return IsImportVisibility(path);
	}

	public static bool IsImportVisibility(string path) {
		var modelImporter = GetModelImporter(path);

		if (modelImporter != null) {
			return modelImporter.importVisibility;
		}

		return false;
	}

	public static void SetImportVisibility(this Mesh mesh, bool importVisibility = false) {
		string path = AssetDatabase.GetAssetPath(mesh);
		SetImportVisibility(path, importVisibility);
	}

	public static void SetImportVisibility(string path, bool importVisibility = false) {
		var modelImporter = GetModelImporter(path);

		if (modelImporter != null && modelImporter.importVisibility != importVisibility) {
			modelImporter.importVisibility = importVisibility;
			AssetDatabase.ImportAsset(path);
		}
	}

	public static bool IsImportCameras(this Mesh mesh) {
		string path = AssetDatabase.GetAssetPath(mesh);
		return IsImportCameras(path);
	}

	public static bool IsImportCameras(string path) {
		var modelImporter = GetModelImporter(path);

		if (modelImporter != null) {
			return modelImporter.importCameras;
		}

		return false;
	}

	public static void SetImportCameras(this Mesh mesh, bool importCameras = false) {
		string path = AssetDatabase.GetAssetPath(mesh);
		SetImportCameras(path, importCameras);
	}

	public static void SetImportCameras(string path, bool importCameras = false) {
		var modelImporter = GetModelImporter(path);

		if (modelImporter != null && modelImporter.importCameras != importCameras) {
			modelImporter.importCameras = importCameras;
			AssetDatabase.ImportAsset(path);
		}
	}

	public static bool IsImportLights(this Mesh mesh) {
		string path = AssetDatabase.GetAssetPath(mesh);
		return IsImportLights(path);
	}

	public static bool IsImportLights(string path) {
		var modelImporter = GetModelImporter(path);

		if (modelImporter != null) {
			return modelImporter.importLights;
		}

		return false;
	}

	public static void SetImportLights(this Mesh mesh, bool importLights = false) {
		string path = AssetDatabase.GetAssetPath(mesh);
		SetImportLights(path, importLights);
	}

	public static void SetImportLights(string path, bool importLights = false) {
		var modelImporter = GetModelImporter(path);

		if (modelImporter != null && modelImporter.importLights != importLights) {
			modelImporter.importLights = importLights;
			AssetDatabase.ImportAsset(path);
		}
	}

	public static bool IsImportBlendShapes(this Mesh mesh) {
		string path = AssetDatabase.GetAssetPath(mesh);
		return IsImportBlendShapes(path);
	}

	public static bool IsImportBlendShapes(string path) {
		var modelImporter = GetModelImporter(path);

		if (modelImporter != null) {
			return modelImporter.importBlendShapes;
		}

		return false;
	}

	public static void SetImportBlendShapes(this Mesh mesh, bool importBlendShapes = false) {
		string path = AssetDatabase.GetAssetPath(mesh);
		SetImportBlendShapes(path, importBlendShapes);
	}

	public static void SetImportBlendShapes(string path, bool importBlendShapes = false) {
		var modelImporter = GetModelImporter(path);

		if (modelImporter != null && modelImporter.importBlendShapes != importBlendShapes) {
			modelImporter.importBlendShapes = importBlendShapes;
			AssetDatabase.ImportAsset(path);
		}
	}
}
