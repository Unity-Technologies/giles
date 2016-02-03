using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GILES
{
	/**
	 * Singleton responsible for enabling gizmo renderers when working in the level editor.
	 */
	[System.Serializable]
	public class pb_GizmoManager : pb_MonoBehaviourSingleton<pb_GizmoManager>
	{
		readonly static System.Type[] BuiltinGizmos = new System.Type[]
		{
			typeof(pb_Gizmo_Light),
			typeof(pb_Gizmo_Camera)
		};

		private static Dictionary<Type, Type> gizmoLookup = null;

		private static void RebuildGizmoLookup()
		{
			gizmoLookup = new Dictionary<Type, Type>();

			foreach(Type t in BuiltinGizmos)
			{
				 pb_GizmoAttribute attrib = (pb_GizmoAttribute) ((IEnumerable<Attribute>) t.GetCustomAttributes(true)).FirstOrDefault(x => x is pb_GizmoAttribute);

				 if(attrib != null)
					gizmoLookup.Add(attrib.type, t);
			}
		}

		/**
		 * Register with pb_Scene delegates so that this script can apply gizmos when new
		 * objects are added.
		 */
		protected override void Awake()
		{
			base.Awake();

			pb_Scene.AddOnObjectInstantiatedListener( OnObjectInstantiated );
			pb_Scene.AddOnLevelLoadedListener( OnLevelLoaded );
			pb_Selection.AddOnSelectionChangeListener( OnSelectionChange );
			pb_Selection.AddOnRemovedFromSelectionListener( OnRemovedFromSelection );
		}

		/**
		 * When a new object is added to the pb_Scene, check to see if it matches any of the gizmo
		 * associations.
		 */
		void OnObjectInstantiated(GameObject go)
		{
			AssociateGizmos(go);
		}

		void OnSelectionChange(IEnumerable<GameObject> selection)
		{
			SetIsSelected(selection, true);
		}

		void OnRemovedFromSelection(IEnumerable<GameObject> selection)
		{
			SetIsSelected(selection, false);
		}

		void SetIsSelected(IEnumerable<GameObject> selection, bool isSelected)
		{
			if(selection != null)
			{
				foreach(GameObject go in selection)
				{
					pb_Gizmo[] gizmos = go.GetComponentsInChildren<pb_Gizmo>();

					foreach(pb_Gizmo g in gizmos)
						g.isSelected = isSelected;
				}
			}
		}

		/**
		 * When a level is loaded, this iterates the scene objects and applies gizmo billboards where
		 * appropriate.
		 */
		void OnLevelLoaded()
		{
			foreach(GameObject go in pb_Scene.Children())
			{
				pb_Gizmo gizmo = AssociateGizmos(go);

				if(gizmo != null && pb_Selection.gameObjects != null && pb_Selection.gameObjects.Contains(go))
				{
					gizmo.isSelected = true;
				}
			}
		}

		pb_Gizmo AssociateGizmos(GameObject go)
		{
			pb_Gizmo[] existing = go.GetComponents<pb_Gizmo>();

			foreach(pb_Gizmo g in existing)
				pb_ObjectUtility.Destroy(g);

			foreach(Component component in go.GetComponentsInChildren<Component>())
			{
				if(component == null)
					continue;
					
				System.Type gizmo = FindGizmoForType(component.GetType());

				if(gizmo != null)
				{
					return (pb_Gizmo) go.AddComponent(gizmo);
				}
			}

			return null;
		}

		public static System.Type FindGizmoForType(Type type)
		{
			if(gizmoLookup == null)
				RebuildGizmoLookup();

			Type result = null;

			gizmoLookup.TryGetValue(type, out result);

			return result;
		}
	}
}
