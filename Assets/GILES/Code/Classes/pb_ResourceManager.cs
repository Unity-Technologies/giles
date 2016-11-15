using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GILES
{

	/**
	 * Singleton resource manager for efficiently finding prefabs and objects loaded in either AssetBundles or Resources.
	 */
	public class pb_ResourceManager : pb_ScriptableObjectSingleton<pb_ResourceManager>
	{
		/// A lookup table of available prefabs in the resources folder.
		Dictionary<string, GameObject> lookup = new Dictionary<string, GameObject>();

		/**
		 * Load all assets listed in pb_Config.Resource_Folder_Paths and populate a lookup table, then
		 * unload.
		 */
		protected override void OnEnable()
		{
			base.OnEnable();

			foreach(string resourceFolder in pb_Config.Resource_Folder_Paths)
			{
				GameObject[] prefabs = Resources.LoadAll(resourceFolder).Select(x => x is GameObject ? (GameObject)x : null).Where(y => y != null).ToArray();

#if ASSET_LOADER_DEBUG
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				sb.AppendLine("Loaded Resources: ");
#endif

				// Populate a dictionary to use as a lookup, then unload whatever isn't used
				for(int i = 0; i < prefabs.Length; i++)
				{
#if ASSET_LOADER_DEBUG
					sb.AppendLine(string.Format("{0,-50} : {1}", prefabs[i].name, prefabs[i].GetComponent<pb_MetaDataComponent>().metadata.fileId) );
#endif
					lookup.Add(prefabs[i].GetComponent<pb_MetaDataComponent>().metadata.fileId, prefabs[i]);
				}

#if ASSET_LOADER_DEBUG
				Debug.Log(sb.ToString());
#endif

				Resources.UnloadUnusedAssets();
			}
		}

		public static GameObject LoadPrefabWithId(string fileId)
		{
			if( string.IsNullOrEmpty(fileId) )
				return null;

			GameObject obj = null;

			if(pb_ResourceManager.instance.lookup.TryGetValue(fileId, out obj))
				return obj;

			return null;
		}

		/**
		 * Attempt to load a prefab from either Resources or AssetBundle using `metadata`.  Can return null
		 * if object is not found.
		 */
		public static GameObject LoadPrefabWithMetadata(pb_MetaData metadata)
		{
			if( metadata.assetType == AssetType.Instance )
			{
				Debug.LogWarning("Attempting to load instance asset through resource manager.");
				return null;
			}

			switch(metadata.assetType)
			{
				case AssetType.Resource:
				{
					if(instance.lookup.ContainsKey(metadata.fileId))
					{
						return instance.lookup[metadata.fileId];
					}
					else
					{
						Debug.LogWarning("Resource manager could not find \"" + metadata.fileId + "\" in loaded resources.");
						return null;
					}
				}

				case AssetType.Bundle:
				{
					return pb_AssetBundles.LoadAsset<GameObject>(metadata.assetBundlePath);
				}

				default:
				{
					Debug.LogError("File not found from metadata: " + metadata);
					return null;
				}
			}
		}

		/**
		 * Load all assets of type T and return.  Searches all directories listed in pb_Config (Resources and AssetBundle).
		 */
		public static IEnumerable<T> LoadAll<T>() where T : UnityEngine.Object
		{
			List<T> assets = new List<T>();

			foreach(string path in pb_Config.Resource_Folder_Paths)
			{
				assets.AddRange( Resources.LoadAll<T>(path) );
			}

			foreach(string bundleName in pb_Config.AssetBundle_Names)
			{
				try
				{
					AssetBundle bundle = pb_AssetBundles.LoadAssetBundleWithName(bundleName);
					assets.AddRange( bundle.LoadAllAssets<T>() );
				}
				catch {}
			}

			return assets;
		}
	}
}
