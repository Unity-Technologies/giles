using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using System.Collections.Generic;
using GILES.Interface;

namespace GILES.UnityEditor
{
	[CustomEditor(typeof(pb_GUIStyle))]
	public class pb_GUIStyleEditor : Editor
	{
		HashSet<string> ignoreProperties = new HashSet<string>()
		{
			"m_PrefabParentObject",
			"m_PrefabInternal",
			"m_GameObject",
			"m_Enabled",
			"m_EditorHideFlags",
			"m_Script",
			"m_Name",
			"m_EditorClassIdentifier"
		};

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			SerializedProperty iterator = serializedObject.GetIterator();

			/// why doesn't GetIterator() do this?
			iterator.Next(true);

			while(iterator.Next(false))
			{
				if(ignoreProperties.Contains(iterator.name))
					continue;

				EditorGUILayout.PropertyField(iterator);
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}