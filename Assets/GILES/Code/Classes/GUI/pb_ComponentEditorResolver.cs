using System;
using UnityEngine;
using System.Collections.Generic;

namespace GILES.Interface
{
	/**
	 * Static class responsible for returning the pb_ComponentEditor most appropriate to
	 * the component.  If the inspected component implements pb_ICustomEditor, the editor
	 * returned by pb_ICustomEditor.IntantiateInspector will be used.  This class should 
	 * be primarily used for overriding Unity component interfaces.
	 */
	public static class pb_ComponentEditorResolver
	{
		/**
		 * Defines the overriden component editors for built-in Unity components.
		 */
		readonly static Dictionary<Type, Type> builtInComponentEditors = new Dictionary<Type, Type>()
		{
			{ typeof(Transform), typeof(pb_TransformEditor) },
			{ typeof(MeshRenderer), typeof(pb_MeshRendererEditor) },
			{ typeof(Camera), typeof(pb_CameraEditor) }
		};

		/**
		 * Find the best matching pb_ComponentEditor for a type.
		 */
		public static pb_ComponentEditor GetEditor(Component component)
		{
			GameObject go = new GameObject();

			Type editorType = null;

			if( !builtInComponentEditors.TryGetValue(component.GetType(), out editorType) )
			{
				foreach(KeyValuePair<Type, Type> kvp in builtInComponentEditors)
				{
					if(kvp.Key.IsAssignableFrom(component.GetType()))
					{
						editorType = kvp.Value;
						break;
					}
				}
			}

			go.name = component.name;
			pb_ComponentEditor editor = (pb_ComponentEditor) go.AddComponent( editorType ?? typeof(pb_ComponentEditor) );
			editor.SetComponent(component);

			return editor;
		}
	}
}