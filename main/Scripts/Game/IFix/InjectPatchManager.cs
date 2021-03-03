using System.Collections;
using UnityEngine;
using Game.Tool;
using System.IO;
using IFix.Core;

namespace IFix
{
	public class InjectPatchManager : Singleton<InjectPatchManager>
	{
		public System.Action OnPatchLoaded { private get; set; }

		public bool HasInitialized { get; private set; }

		public Coroutine LoadScriptPatch()
		{
            string platformName = Application.platform == RuntimePlatform.IPhonePlayer ? "iOS" : "Android";
			string assetName = $"{platformName}Unity_Main.patch";
			if(!GM.AssetManager.HasTheAssetInBundle(assetName))
			{
				return null;
			}

			string path = GM.AssetManager.GetAssetFullPathByAssetName(assetName);
			return EB.Assets.LoadAsync(path, typeof(TextAsset), o=>
			{
				if(o != null)
				{
					var asset = o as TextAsset;
					PatchManager.Load(new MemoryStream(asset.bytes));
					if (OnPatchLoaded != null)
					{
						OnPatchLoaded();
						OnPatchLoaded = null;
					}
					HasInitialized = true;
				}
			});
		}

		public void ResetState()
		{
			HasInitialized = false;
		}
	}
}