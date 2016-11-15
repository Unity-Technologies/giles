using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace GILES.Interface
{
	/**
	 * Base class for Component editors.  This implements a generic component editor when 
	 * no suitable override is found.
	 * 
	 * To have a component use your custom editor, either append the pb_ComponentEditorResolver.builtinComponentEditors
	 * array or inherit pb_ICustomEditor from your script and instantiate a prefab with the editor.
	 *
	 * Note: If subclassing and making use of pb_TypeInspector delegates to set and get values,
	 * you will need to manually notify pb_ComponentDiff of changes to prefabs.  See pb_TransformEditor
	 * for an example.  If using pb_TypeInspector with reflection, this is not necesssary as it is 
	 * done automatically.
	 */
	public class pb_ComponentEditor : MonoBehaviour
	{
		/// The UnityEngine.Component being edited.
		protected Component target;

		public static readonly HashSet<string> ignoreProperties = new HashSet<string>()
		{
			"tag",
			"name",
			"hideFlags",
			"useGUILayout"
		};

		/**
		 * Initialize this editor with a component target.  This rebuilds the GUI automatically.
		 */
		public void SetComponent(Component target)
		{
			foreach(Transform t in transform)
				pb_ObjectUtility.Destroy(t.gameObject);

			this.target = target;

			InitializeGUI();
		}

		/**
		 * Update the inspector GUI.
		 */
		public virtual void UpdateGUI()
		{
			foreach(pb_TypeInspector inspector in gameObject.GetComponentsInChildren<pb_TypeInspector>(false))
				inspector.UpdateGUI();
		}

		protected virtual void InitializeGUI()
		{
			pb_GUIUtility.AddVerticalLayoutGroup(gameObject);

			foreach(PropertyInfo prop in pb_Reflection.GetSerializableProperties(target.GetType(), BindingFlags.Instance | BindingFlags.Public))
			{
				if( ignoreProperties.Contains(prop.Name) || System.Attribute.GetCustomAttribute(prop, typeof(pb_InspectorIgnoreAttribute)) != null)
					continue;

				pb_TypeInspector typeInspector = pb_InspectorResolver.AddTypeInspector(target, transform, property : prop);
				typeInspector.onTypeInspectorSetValue = this.OnTypeInspectorSetValue;
			}

			foreach(FieldInfo fil in target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
			{
				if(System.Attribute.GetCustomAttribute(fil, typeof(pb_InspectorIgnoreAttribute)) != null)
					continue;

				pb_TypeInspector typeInspector = pb_InspectorResolver.AddTypeInspector(target, transform, field : fil);
				typeInspector.onTypeInspectorSetValue = this.OnTypeInspectorSetValue;
			}
		}

		/**
		 * Called by a type inspector when a value has been set.
		 */
		void OnTypeInspectorSetValue()
		{
			foreach(pb_Gizmo gizmo in target.gameObject.GetComponents<pb_Gizmo>())
			{
				if ( gizmo.CanEditType(target.GetType()) )
				{
					gizmo.OnComponentModified();
				}
			}
		}
	}
}