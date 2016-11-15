using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.IO;
using System.Linq;
using GILES;

namespace GILES.UnityEditor
{
	/**
	 * Editor class to aid in the creation of Asset Bundles.
	 */
	public class CreateAssetBundle : Editor
	{
		[MenuItem("Tools/Level Editor/Build Asset Bundles")]
		static void BuildAssetBundle()
		{
			if(!Directory.Exists("Assets/AssetBundles"))
				Directory.CreateDirectory("Assets/AssetBundles");

			// Make sure all assets in level editor bundles are marked with pb_MetaData
			foreach(string bundle_name in AssetDatabase.GetAllAssetBundleNames())
			{
				if(pb_Config.AssetBundle_Names.Any(x => x.IndexOf(bundle_name, StringComparison.OrdinalIgnoreCase) >= 0))
				{
					foreach(string asset_path in AssetDatabase.GetAssetPathsFromAssetBundle(bundle_name))
					{
						SetMetadata(asset_path, bundle_name);
					}
				}
			}

			BuildPipeline.BuildAssetBundles("Assets/AssetBundles",
				BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle,
				EditorUserBuildSettings.activeBuildTarget );
		}

		private static void SetMetadata(string path, string name)
		{
			switch( pb_FileUtility.GetPathType(path) )
			{
				case PathType.File:
				{
					UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(path);

					GameObject go = obj as GameObject;

					if(go != null && PrefabUtility.GetPrefabType(go) == PrefabType.Prefab)
					{
						pb_MetaDataComponent metadata = go.GetComponent<pb_MetaDataComponent>();

						if(!metadata)
							metadata = go.AddComponent<pb_MetaDataComponent>();

						metadata.SetAssetBundleData(name, path);
					}

					break;
				}

				case PathType.Directory:
				{
					foreach(string subdir in Directory.GetDirectories(path))
						SetMetadata(subdir, name);

					foreach(string asset in Directory.GetFiles(path))
						SetMetadata(asset, name);

					break;
				}

				default:
					break;
			}
		}

	}

}
