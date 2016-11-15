using System.Runtime.Serialization;

namespace GILES
{
	/**
	 * pb_AssetBundlePath contains information that helps pb_ResourceManager and pb_AssetBundles find
	 * and load assets from AssetBundles.
	 */
	[System.Serializable]
	public class pb_AssetBundlePath : ISerializable
	{
		/// The name of an asset bundle (as set in the Unity Editor).
		public string assetBundleName;
		
		/// The path to the asset within the AssetBundle.  Usually this will be set by the CreateAssetBundles 
		/// editor tool.
		public string filePath;

		/**
		 * Constructor
		 * @param InAssetBundleName Name of the asset bundle (should be name only - use with pb_Config AssetBundle_SearchDirectories).
		 * @param InFilePath The path to the asset within the bundle.
		 */
		public pb_AssetBundlePath(string InAssetBundleName, string InFilePath)
		{
			assetBundleName	= InAssetBundleName;
			filePath		= InFilePath;
		}

		/**
		 * Serialization constructor.
		 */
		public pb_AssetBundlePath(SerializationInfo info, StreamingContext context)
		{
			assetBundleName = (string) info.GetValue("assetBundleName", typeof(string));
			filePath = (string) info.GetValue("filePath", typeof(string));
		}

		/**
		 * Serialization override.
		 */
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("assetBundleName", assetBundleName, typeof(string));
			info.AddValue("filePath", filePath, typeof(string));
		}

		/**
		 * Returns a nicely formatted summary of this bundle information.
		 */
		public override string ToString()
		{
			return "Bundle: " + assetBundleName + "\nPath: " + filePath;
		}
	}
}