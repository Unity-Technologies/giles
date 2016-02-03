using UnityEngine;
using System;

namespace GILES
{
	/**
	 * Components marked with this attribute will be ignored when copying or duplicating scene objects through
	 * pb_Scene.Instantiate().
	 *
	 * \sa pb_Gizmo, pb_Highlight
	 */
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class pb_EditorComponentAttribute : Attribute
	{
		public static void StripEditorComponents(GameObject target)
		{
			foreach(Component component in target.GetComponents<Component>())
			{
				if(component != null && Attribute.GetCustomAttribute(component.GetType(), typeof(pb_EditorComponentAttribute)) != null )
					pb_ObjectUtility.Destroy(component);
			}
		}
	}
}