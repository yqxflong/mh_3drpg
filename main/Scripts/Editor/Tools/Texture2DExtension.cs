using Sirenix.OdinInspector.Editor.Drawers;
using UnityEditor;
using UnityEngine;

public static class Texture2DExtension {
	public static bool IsReadable(this Texture2D tex) {
		string path = AssetDatabase.GetAssetPath(tex);
		return IsReadable(path);
	}

	public static bool IsReadable(string path) {
		if (!string.IsNullOrEmpty(path)) {
			var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

			if (textureImporter != null) {
				return textureImporter.isReadable;
			}
		}

		return false;
	}

	public static string SetReadable(this Texture2D tex, bool isReadable = false) {
		string path = AssetDatabase.GetAssetPath(tex);
		SetReadable(path, isReadable);
		return path;
	}

	public static void SetReadable(string path, bool isReadable = false) {
		if (!string.IsNullOrEmpty(path)) {
			var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

			if (textureImporter != null && textureImporter.isReadable != isReadable) {
				textureImporter.isReadable = isReadable;
				AssetDatabase.ImportAsset(path);
			}
		}
	}

	public static bool WrapModeIsRepeat(this Texture2D tex)
	{
		string path = AssetDatabase.GetAssetPath(tex);
		return WrapModeIsRepeat(path);
	}

	public static bool WrapModeIsRepeat(string path)
	{
		if (!string.IsNullOrEmpty(path))
		{
			var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

			if (textureImporter != null)
			{
				// 这里之所以不直接使用枚举值判等，原因是有些纹理的wrapMode值在Inspector面板中是repeat，但是打印的值是-1
				return textureImporter.wrapMode <= 0;// TextureWrapMode.Repeat;
			}
		}

		return false;
	}

	public static string SetWrapMode(this Texture2D tex, TextureWrapMode wrapMode = TextureWrapMode.Clamp)
	{
		string path = AssetDatabase.GetAssetPath(tex);
		SetWrapMode(path, wrapMode);
		return path;
	}

	public static void SetWrapMode(string path, TextureWrapMode wrapMode = TextureWrapMode.Clamp)
	{
		if (!string.IsNullOrEmpty(path))
		{
			var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

			if (textureImporter != null && textureImporter.wrapMode != wrapMode)
			{
				textureImporter.wrapMode = wrapMode;
				AssetDatabase.ImportAsset(path);
			}
		}
	}

	public static bool IsCompression(this Texture2D tex)
	{
		string path = AssetDatabase.GetAssetPath(tex);
		return IsCompression(path);
	}

	public static bool IsCompression(string path)
	{
		if (!string.IsNullOrEmpty(path))
		{
			var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
			
			if (textureImporter != null)
			{
#if UNITY_ANDROID
				var textureImporterPlatformSetting = textureImporter.GetPlatformTextureSettings("Android");

				if (textureImporterPlatformSetting.overridden)
				{
					return textureImporterPlatformSetting.format.ToString().Contains("ETC");
				}
				else
				{
					return textureImporter.textureCompression != TextureImporterCompression.Uncompressed;
				}
#elif UNITY_IOS
				var textureImporterPlatformSetting = textureImporter.GetPlatformTextureSettings("iPhone");
				return textureImporterPlatformSetting.format.ToString().Contains("ASTC");
#else
				return textureImporter.textureCompression != TextureImporterCompression.Uncompressed;
#endif
			}
		}

		return false;
	}

	public static string SetCompression(this Texture2D tex, TextureImporterCompression compression = TextureImporterCompression.Compressed)
	{
		string path = AssetDatabase.GetAssetPath(tex);
		SetCompression(path, compression);
		return path;
	}

	public static void SetCompression(string path, TextureImporterCompression compression = TextureImporterCompression.Compressed)
	{
		if (!string.IsNullOrEmpty(path))
		{
			var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

			if (textureImporter != null)
			{
				
#if UNITY_ANDROID
				var textureImporterPlatformSetting = textureImporter.GetPlatformTextureSettings("Android");
				textureImporterPlatformSetting.format = textureImporter.DoesSourceTextureHaveAlpha() ? TextureImporterFormat.ETC2_RGBA8 : TextureImporterFormat.ETC2_RGB4;
				textureImporter.SetPlatformTextureSettings(textureImporterPlatformSetting);
				textureImporter.SaveAndReimport();
#elif UNITY_IOS
				var textureImporterPlatformSetting = textureImporter.GetPlatformTextureSettings("iPhone");
				textureImporterPlatformSetting.format = textureImporter.DoesSourceTextureHaveAlpha() ? TextureImporterFormat.ASTC_RGBA_4x4 : TextureImporterFormat.ASTC_RGB_4x4;
				textureImporter.SetPlatformTextureSettings(textureImporterPlatformSetting);
				textureImporter.SaveAndReimport();
#else
				textureImporter.textureCompression = compression;
				textureImporter.SaveAndReimport();
#endif
			}
		}
	}

	public static bool FilterModeIsTrilinear(this Texture2D tex)
	{
		string path = AssetDatabase.GetAssetPath(tex);
		return FilterModeIsTrilinear(path);
	}

	public static bool FilterModeIsTrilinear(string path)
	{
		if (!string.IsNullOrEmpty(path))
		{
			var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

			if (textureImporter != null)
			{
				return textureImporter.filterMode == FilterMode.Trilinear;
			}
		}

		return false;
	}

	public static string SetFilterMode(this Texture2D tex, FilterMode filterMode = FilterMode.Bilinear)
	{
		string path = AssetDatabase.GetAssetPath(tex);
		SetFilterMode(path, filterMode);
		return path;
	}

	public static void SetFilterMode(string path, FilterMode filterMode = FilterMode.Bilinear)
	{
		if (!string.IsNullOrEmpty(path))
		{
			var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

			if (textureImporter != null && textureImporter.filterMode != filterMode)
			{
				textureImporter.filterMode = filterMode;
				AssetDatabase.ImportAsset(path);
			}
		}
	}
}
