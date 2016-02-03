using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using GILES;
using GILES.Serialization;

namespace GILES.UnityEditor
{
	[CustomEditor(typeof(pb_MetaDataComponent))]
	public class pb_MetaDataEditor : Editor
	{
		pb_MetaDataComponent data;
		pb_ComponentDiff diff;
		Dictionary<Component, bool> dropdowns = new Dictionary<Component, bool>();

		void OnEnable()
		{
			data = (pb_MetaDataComponent) target;
			diff = data.metadata.componentDiff;
		}

		public override void OnInspectorGUI()
		{
			GUILayout.Label("Asset Type: " + data.metadata.assetType);
			GUILayout.Label("File ID: " + data.metadata.fileId);
			GUILayout.Label("Asset Path: " + data.metadata.assetBundlePath);

			GUILayout.Label("Modified Values", EditorStyles.boldLabel);

			int labelWidth = (int) Mathf.Min(Screen.width/2, 100);

			foreach(KeyValuePair<Component, Dictionary<string, object>> kvp in diff.modifiedValues)
			{
				if(!dropdowns.ContainsKey(kvp.Key))
					dropdowns.Add(kvp.Key, false);

				dropdowns[kvp.Key] = EditorGUILayout.Foldout(dropdowns[kvp.Key], kvp.Key.ToString());

				if(dropdowns[kvp.Key])
				{
					foreach(KeyValuePair<string, object> changes in kvp.Value)
					{
						GUILayout.BeginHorizontal();

						GUILayout.Label(changes.Key, GUILayout.MinWidth(labelWidth), GUILayout.MaxWidth(labelWidth));

						GUILayout.Label(changes.Value.ToString().Truncate(128));

						GUILayout.EndHorizontal();
					}
				}
			}
		}

	}
}
