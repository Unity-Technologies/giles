using UnityEngine;
using System.Runtime.Serialization;
using GILES.Serialization;

namespace GILES
{
	/**
	 * pb_Metadata stores information about how an asset should be stored and reconstructed.
	 */
	[System.Serializable()]
	public class pb_MetaData : ISerializable
	{

		public const string GUID_NOT_FOUND = "MetaData_NoGUIDPresent";
		public const string ASSET_BUNDLE = "MetaData_BundleAsset";
		public const string ASSET_INSTANCE = "MetaData_InstanceAsset";

		[SerializeField] private string _fileId = GUID_NOT_FOUND;
		[SerializeField] private pb_AssetBundlePath _assetBundlePath;
		[SerializeField] private AssetType _assetType = AssetType.Instance;

		// Tags can be used to organize objects in the resource browser.
		public string[] tags = new string[0];

		// If target is a prefab, this stores a diff of component values.
		public pb_ComponentDiff componentDiff;

		/**
		 * Serialization override.
		 */
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("_fileId", 			_fileId, 			typeof(string));
			info.AddValue("_assetBundlePath", 	_assetBundlePath, 	typeof(pb_AssetBundlePath));
			info.AddValue("_assetType", 		_assetType, 		typeof(AssetType));
			info.AddValue("componentDiff", 		componentDiff, 		typeof(pb_ComponentDiff));
		}

		/**
		 * Serialized constructor.
		 */
		public pb_MetaData(SerializationInfo info, StreamingContext context)
		{
			_fileId				= (string) info.GetValue("_fileId", typeof(string));
			_assetBundlePath	= (pb_AssetBundlePath) info.GetValue("_assetBundlePath", typeof(pb_AssetBundlePath));
			_assetType			= (AssetType) info.GetValue("_assetType", typeof(AssetType));
			componentDiff		= (pb_ComponentDiff) info.GetValue(	"componentDiff", typeof(pb_ComponentDiff));
		}

		/**
		 * Basic constructor (used on instance assets).
		 */
		public pb_MetaData()
		{
			_assetType = AssetType.Instance;
			_fileId = GUID_NOT_FOUND;
			_assetBundlePath = null;

			componentDiff = new pb_ComponentDiff();
		}

		/**
		 * The file id that can be used to look up this object (if this is a prefab stored in Resources folder).
		 */
		public string fileId
		{
			get { return _fileId; }
		}

		/**
		 * Return the asset bundle information (if assetType == AssetType.Bundle) - if not a bundle
		 * type this value will be junk.
		 */
		public pb_AssetBundlePath assetBundlePath
		{
			get { return _assetBundlePath; }
		}

		/**
		 * Return the type of asset pointed to.
		 */
		public AssetType assetType
		{
			get { return _assetType; }
		}
			
		/**
		 * Set the asset bundle information.
		 */
		public void SetAssetBundleData(string bundleName, string assetPath)
		{
			_fileId = ASSET_BUNDLE;
			_assetType = AssetType.Bundle;
			_assetBundlePath = new pb_AssetBundlePath(bundleName, assetPath);
		}

		/**
		 * Set the fileID field if this asset is in the resources folder.
		 */
		public void SetFileId(string id)
		{
			_assetType = AssetType.Resource;
			_assetBundlePath = null;
			_fileId = id;
		}
	}
}
