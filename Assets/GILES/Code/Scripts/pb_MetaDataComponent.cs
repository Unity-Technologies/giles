using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using System.Collections;
using GILES.Serialization;

namespace GILES
{
	/**
	 * Metadata components are used to serialize and deserialize prefabs pointing
	 * to AssetBundle or Resource folder objects.  Can also mark an object as an
	 * instance asset, meaning the serializer will write all the components and
	 * information necessary to rebuild the object on deserialization.  If asset 
	 * is type AssetBundle or Resource it will be loaded from it's respective 
	 * location.
	 */
	[DisallowMultipleComponent]
	[pb_JsonIgnore]
	public class pb_MetaDataComponent : MonoBehaviour
	{
		/// Reference metadata information about a prefab or gameObject.  Used to
		/// serialize and deserialize prefabs/instance objects.
		public pb_MetaData metadata = new pb_MetaData();

		/**
		 * Set the name and asset path that this object can be found with.
		 */
		public void SetAssetBundleData(string bundleName, string assetPath)
		{
			metadata.SetAssetBundleData(bundleName, assetPath);
#if UNITY_EDITOR
			EditorUtility.SetDirty(this);
#endif
		}

		/**
		 * Set the fileId field if this asset is in the resources folder.
		 */
		public bool UpdateFileId()
		{
			bool modified = false;

#if UNITY_EDITOR
			if( PrefabUtility.GetPrefabType(this.gameObject) == PrefabType.Prefab && metadata.assetType != AssetType.Bundle )
			{
				string path = AssetDatabase.GetAssetPath(this.gameObject);
				string guid = AssetDatabase.AssetPathToGUID(path);

				if( !string.IsNullOrEmpty(metadata.fileId) && !guid.Equals(metadata.fileId) )
				{
					Debug.Log("Level Editor: Resource fileId changed -> " + this.gameObject.name + " (" + metadata.fileId + " -> " + guid + ")");
					modified = true;
				}

				metadata.SetFileId( guid );

				EditorUtility.SetDirty(this);
			}
#endif
			return modified;
		}

		public string GetFileId()
		{
			return metadata.fileId;
		}
	}
}
