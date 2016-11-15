using UnityEngine;
using UnityEditor;
using GILES.Interface;

namespace GILES.UnityEditor
{
	/**
	 * Custom editor implementation for pb_LayoutElementTextWidth.
	 * 
	 * \notes Despite a valiant effort, AVFoundation remains the undisputed king of super-long class names.
	 */
	[CanEditMultipleObjects, CustomEditor(typeof(pb_LayoutElementText))]
	public class pb_LayoutElementTextEditor : Editor
	{
		SerializedProperty textComponent;
		SerializedProperty expandWidth, expandHeight;
		SerializedProperty paddingWidth, paddingHeight;
		SerializedProperty flexibleWidth, flexibleHeight;

		void OnEnable()
		{
			textComponent = serializedObject.FindProperty("text");
			expandWidth = serializedObject.FindProperty("expandWidth");
			expandHeight = serializedObject.FindProperty("expandHeight");
			paddingWidth = serializedObject.FindProperty("paddingWidth");
			paddingHeight = serializedObject.FindProperty("paddingHeight");
			flexibleWidth = serializedObject.FindProperty("m_FlexibleWidth");
			flexibleHeight = serializedObject.FindProperty("m_FlexibleHeight");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(textComponent);

			GUILayout.BeginHorizontal();
			expandWidth.boolValue = EditorGUILayout.Toggle("Expand Width", expandWidth.boolValue);
			GUI.enabled = expandWidth.boolValue;
			EditorGUIUtility.labelWidth = 60f;
			paddingWidth.floatValue = EditorGUILayout.FloatField("Padding", paddingWidth.floatValue);
			EditorGUIUtility.labelWidth = 0f;
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUI.enabled = true;
			expandHeight.boolValue = EditorGUILayout.Toggle("Expand Height", expandHeight.boolValue);
			GUI.enabled = expandHeight.boolValue;
			EditorGUIUtility.labelWidth = 60f;
			paddingHeight.floatValue = EditorGUILayout.FloatField("Padding", paddingHeight.floatValue);
			GUILayout.EndHorizontal();

			bool flexWidth = flexibleWidth.floatValue > 0;
			bool flexHeight = flexibleHeight.floatValue > 0;

			GUILayout.BeginHorizontal();
				EditorGUIUtility.labelWidth = 0f;
				GUI.enabled = true;
				flexWidth = EditorGUILayout.Toggle("Flexible Width", flexWidth);

				if(flexWidth && flexibleWidth.floatValue < 0f) 
					flexibleWidth.floatValue = 1f;
				else if(!flexWidth && flexibleWidth.floatValue > 0f)
					flexibleWidth.floatValue = -1f;
				GUI.enabled = flexWidth;
				EditorGUIUtility.labelWidth = 60f;
				flexibleWidth.floatValue = EditorGUILayout.FloatField("Weight", flexibleWidth.floatValue);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
				GUI.enabled = true;
				EditorGUIUtility.labelWidth = 0f;
				flexHeight = EditorGUILayout.Toggle("Flexible Height", flexHeight);

				if(flexHeight && flexibleHeight.floatValue < 0f) 
					flexibleHeight.floatValue = 1f;
				else if(!flexHeight && flexibleHeight.floatValue > 0f)
					flexibleHeight.floatValue = -1f;
				GUI.enabled = flexHeight;
				EditorGUIUtility.labelWidth = 60f;
				flexibleHeight.floatValue = EditorGUILayout.FloatField("Weight", flexibleHeight.floatValue);
			GUILayout.EndHorizontal();

			GUI.enabled = true;
			
			serializedObject.ApplyModifiedProperties();
		}
	}
}