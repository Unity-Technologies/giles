using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace GILES.Interface
{
	/**
	 * Used by pb_TypeInspector to find the appropriate inspector subclasses to draw
	 * the inspector GUI.
	 */
	public static class pb_InspectorResolver
	{
		public const string TYPE_INSPECTOR_PATH = "Required/GUI/TypeInspector";

		/// Link types to their respective best available pb_TypeInspector.  This can include types that do not have an exact
		/// inspector match, unlike inspectorLookup which only contains 1:1 matches.
		private static Dictionary<Type, GameObject> inspectorPool = new Dictionary<Type, GameObject>();

		/// Store a lookup table of pb_TypeInspector prefabs.
		private static Dictionary<IEnumerable<pb_TypeInspectorAttribute>, GameObject> inspectorLookup = null;

		private static void InitializeLookup()
		{
			inspectorPool = new Dictionary<Type, GameObject>();
			inspectorLookup = new Dictionary<IEnumerable<pb_TypeInspectorAttribute>, GameObject>();

			foreach(GameObject go in Resources.LoadAll(TYPE_INSPECTOR_PATH, typeof(GameObject)))
			{
				pb_TypeInspector typeInspector = go.GetComponent<pb_TypeInspector>();

				if(typeInspector == null)
					continue;

				IEnumerable<Attribute> typeAttribs = (IEnumerable<Attribute>) typeInspector.GetType().GetCustomAttributes(true);
				inspectorLookup.Add(typeAttribs.Where(x => x != null && x is pb_TypeInspectorAttribute).Cast<pb_TypeInspectorAttribute>(), go);
			}
		}

		/**
		 *	Get a type inspector that most closely matches type.
		 *	If an exact type match is found, that inspector is returned.  If no exact match is found,
		 *  the first base class match found.  If no base class is found, a generic object inspector
		 *  is returned, which in turn will reflect the properties and fields contained and attempt to
		 *	build inspectors for each.
		 */
		public static pb_TypeInspector GetInspector(Type type)
		{
			if( inspectorLookup == null )
				InitializeLookup();

			GameObject inspectorObject;

			if(inspectorPool.TryGetValue(type, out inspectorObject))
				return GameObject.Instantiate(inspectorObject).GetComponent<pb_TypeInspector>();

			List<GameObject> inspectors = new List<GameObject>();

			foreach(KeyValuePair<IEnumerable<pb_TypeInspectorAttribute>, GameObject> kvp in inspectorLookup)
			{
				foreach(pb_TypeInspectorAttribute attrib in kvp.Key)
				{
					if( attrib.CanEditType(type) )
					{
						if(attrib.type == type)
						{
							inspectors.Insert(0, kvp.Value);
							goto EXACT_TYPE_INSPECTOR_FOUND;
						}
						else
						{
							inspectors.Add(kvp.Value);
						}
					}
				}
			}

EXACT_TYPE_INSPECTOR_FOUND:

			if(inspectors.Count > 0)
			{
				inspectorPool.Add(type, inspectors[0]);
				inspectorObject = GameObject.Instantiate(inspectors[0]);
				pb_TypeInspector typeInspector = inspectorObject.GetComponent<pb_TypeInspector>();
				typeInspector.SetDeclaringType(type);
				return typeInspector;
			}
			else
			{
				GameObject go = new GameObject();
				go.name = "Generic Object Inspector: " + type;
				pb_TypeInspector typeInspector = go.AddComponent<pb_ObjectInspector>();
				typeInspector.SetDeclaringType(type);
				return typeInspector;
			}
		}

		/**
		 * Add a new type inspector to parentTransform with a target and reflected property or field.
		 */
		public static pb_TypeInspector AddTypeInspector(object target, Transform parentTransform, PropertyInfo property = null, FieldInfo field = null)
		{
			pb_TypeInspector inspector = null;
			System.Type type = property != null ? property.PropertyType : field.FieldType;

			inspector = pb_InspectorResolver.GetInspector(type);

			if(inspector != null)
			{
				if(property != null)
					inspector.Initialize(target, property);
				else
					inspector.Initialize(target, field);

				inspector.transform.SetParent(parentTransform);
			}
			else
			{
				Debug.LogError("No inspector found!  Is `pb_ObjectInspector.cs` missing?");
			}

			return inspector;
		}
	}
}
