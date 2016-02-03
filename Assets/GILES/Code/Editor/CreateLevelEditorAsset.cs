using UnityEngine;
using UnityEditor;
using System.Collections;
using GILES;
using System;

namespace GILES.UnityEditor
{

	/**
	 * Create a new level editor asset.
	 */
	public class CreateLevelEditorAsset : Editor
	{
		[MenuItem("Tools/Level Editor/Create Edit Mode Asset")]
		public static void MenuCreateLevelEditMode()
		{
			foreach(MonoScript ms in Selection.objects)
			{
				Type t = ms.GetClass();

				if( t.BaseType == typeof(pb_SceneEditor) )
				{
					pb_SceneEditor editor = (pb_SceneEditor) ScriptableObject.CreateInstance(t);

					AssetDatabase.CreateAsset(editor, AssetDatabase.GenerateUniqueAssetPath("Assets/LevelEditor/Edit Modes/" + t.ToString() + ".asset"));
				}
			}
		}
	}
}