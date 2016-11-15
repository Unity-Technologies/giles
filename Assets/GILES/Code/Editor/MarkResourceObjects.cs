using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.Callbacks;

namespace GILES.UnityEditor
{
	/**
	 * This editor script runs any time the project window is modified, checking that all 
	 * objects in the Resources/Level Editor Prefabs folder have the pb_MetaData script
	 * attached and generated properly.
	 */
	[InitializeOnLoad]
	public class MarkResourceObjects
	{
		static MarkResourceObjects()
		{
			EditorApplication.projectWindowChanged += ProjectWindowChanged;
		}

		~MarkResourceObjects()
		{
			EditorApplication.projectWindowChanged -= ProjectWindowChanged;
		}

		[MenuItem("Tools/Level Editor/Rebuild Resource IDs")]
		public static void ProjectWindowChanged()
		{
			double timer = Time.realtimeSinceStartup;
			foreach(string path in pb_Config.Resource_Folder_Paths)
			{
				foreach(GameObject go in (UnityEngine.Object[]) Resources.LoadAll(path, typeof(GameObject)))
				{
					pb_MetaDataComponent md = go.GetComponent<pb_MetaDataComponent>();
					if(!md) md = go.AddComponent<pb_MetaDataComponent>();
					
					if( md.UpdateFileId() )
						EditorUtility.SetDirty(go);
				}
			}
			if( (Time.realtimeSinceStartup - timer) > 3f )
				Debug.Log("Marking resources took: " + (Time.realtimeSinceStartup - timer) + " seconds");
		}
	}
}
