using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GILES
{
	/**
	 * pb_AssetBundles provides an interface to accessing AssetBundle objects.  It handles loading and unloading
	 * automatically.
	 */
	public class pb_AssetBundles : pb_MonoBehaviourSingleton<pb_AssetBundles>
	{
		public override bool dontDestroyOnLoad { get { return true; } }

		List<string> availableAssetBundles = new List<string>();
		Dictionary<string, AssetBundle> loadedAssetBundles = new Dictionary<string, AssetBundle>();

#region Interface

		/**
		 * Register an asset bundle as containing usable assets.  This means it will be searched
		 * when for object references when serailizing levels.
		 */
		public static void RegisterAssetBundle(string path)
		{
			pb_AssetBundles.instance._RegisterAssetBundle(pb_FileUtility.GetFullPath(path));
		}

		/**
		 * Load an asset bundle from a path (relative or absolute).
		 */
		public static AssetBundle LoadAssetBundle(string path)
		{
			string full_path = pb_FileUtility.GetFullPath(path);
			return pb_AssetBundles.instance._LoadAssetBundle(full_path);
		}

		/**
		 * Load an AssetBundle from it's name.  Bundle must have been registered using pb_AssetBundles.RegisterAssetBundle() or
		 * have been listed in pb_Config.AssetBundle_Paths.
		 */
		public static AssetBundle LoadAssetBundleWithName(string name)
		{
			return pb_AssetBundles.instance._LoadAssetBundleWithName(name);
		}

		/**
		 * Return the AssetBundle and path within that bundle to an object.
		 * May return a null or emptry string.
		 */
		public static bool GetAssetPath<T>(T asset, out pb_AssetBundlePath path) where T : UnityEngine.Object
		{
			return pb_AssetBundles.instance._GetAssetPath<T>(asset, out path);
		}

		/**
		 * Load an asset from a pb_AssetBundlePath.
		 */
		public static T LoadAsset<T>(pb_AssetBundlePath path) where T : UnityEngine.Object
		{
			return pb_AssetBundles.instance._LoadAsset<T>(path);
		}
#endregion

#region Implementation

		private void _RegisterAssetBundle(string full_path)
		{
			if(!availableAssetBundles.Contains(full_path))
				availableAssetBundles.Add(full_path);
		}

		private AssetBundle _LoadAssetBundle(string full_path)
		{
			AssetBundle bundle = null;

			if(!loadedAssetBundles.TryGetValue(full_path, out bundle))
			{
				_RegisterAssetBundle(full_path);

#if UNITY_5_2 || UNITY_5_1
				bundle = AssetBundle.CreateFromFile(full_path);
#else
				bundle = AssetBundle.LoadFromFile(full_path);
#endif
				loadedAssetBundles.Add(full_path, bundle);
			}

			return bundle;
		}

		private void _UnloadAssetBundle(string full_path, bool unloadAllLoadedObjects)
		{
			if(loadedAssetBundles.ContainsKey(full_path))
			{
				loadedAssetBundles[full_path].Unload(unloadAllLoadedObjects);
				loadedAssetBundles.Remove(full_path);
			}
		}

		private void _UnloadAssetBundle(AssetBundle bundle)
		{
			foreach(KeyValuePair<string, AssetBundle> bundles in loadedAssetBundles)
			{
				if(bundles.Value == bundle)
				{
					loadedAssetBundles.Remove(bundles.Key);
					return;
				}
			}
		}

		private bool _GetAssetPath<T>(T asset, out pb_AssetBundlePath path) where T : UnityEngine.Object
		{
			foreach(string bundle_path in availableAssetBundles)
			{
				try
				{
					AssetBundle bundle = _LoadAssetBundle(bundle_path);

					foreach(KeyValuePair<string, UnityEngine.Object> kvp in LoadBundleAssetsWithPaths(bundle))
					{
						if(kvp.Value.GetType() == typeof(T))
						{
							path = new pb_AssetBundlePath(bundle_path, kvp.Key);

							return true;
						}
					}

				}

				catch(System.Exception e)
				{
					Debug.LogWarning("Failed loading AssetBundle: " + bundle_path + "\n" + e.ToString());
				}
			}

			path = null;

			return false;
		}

		private Dictionary<string, UnityEngine.Object> LoadBundleAssetsWithPaths(AssetBundle bundle)
		{
			Dictionary<string, UnityEngine.Object> dic = new Dictionary<string, UnityEngine.Object>();

			foreach(string str in bundle.GetAllAssetNames())
			{
				dic.Add(str, bundle.LoadAsset(str));
			}

			return dic;
		}

		private AssetBundle _LoadAssetBundleWithName(string name)
		{
			// First check if bundle is already loaded
			foreach(string str in availableAssetBundles)
			{
				if( string.Compare( System.IO.Path.GetFileName(str), name, true) == 0 )
				{
					return _LoadAssetBundle(str);
				}
			}

			// if not, search known assetbundle directories for match (returns first matching bundle path)
			foreach(string dir in pb_Config.AssetBundle_SearchDirectories)
			{
				string[] paths = Directory.GetFiles(dir, name, SearchOption.AllDirectories);

				if(paths.Length > 0)
				{
					foreach(string path in paths)
					{
						try
						{
							AssetBundle bundle = _LoadAssetBundle(path);
							if(bundle != null)
								return bundle;
						} catch {}
					}
				}
			}

			return null;
		}

		private T _LoadAsset<T>(pb_AssetBundlePath path) where T : UnityEngine.Object
		{
			AssetBundle bundle = _LoadAssetBundleWithName(path.assetBundleName);

			if(bundle == null)
				return (T) null;

			if( bundle.Contains(path.filePath) )
			{
				T obj = bundle.LoadAsset<T>(path.filePath);

				return obj;
			}

			return (T) null;
		}
#endregion
	}
}
