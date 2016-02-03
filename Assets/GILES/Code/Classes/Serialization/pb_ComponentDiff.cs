using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GILES.Serialization
{
	/**
	 * Stores a dictionary of modified values and their corresponding component.  This is 
	 * used to serialize changes to prefabs without writing the entirity of their serialized
	 * data to disk.
	 */
	public class pb_ComponentDiff : ISerializable
	{
		public Dictionary<Component, Dictionary<string, object>> modifiedValues;

		public pb_ComponentDiff()
		{
			modifiedValues = new Dictionary<Component, Dictionary<string, object>>();
		}

		List<System.Type> keys;
		List<Dictionary<string, object>> values;

		/**
		 * Serialization override.
		 */
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if(modifiedValues == null)
				modifiedValues = new Dictionary<Component, Dictionary<string, object>>();
				
			List<System.Type> keys = modifiedValues.Keys.Select(x => x.GetType()).ToList();

			info.AddValue("components", keys, typeof(List<System.Type>));
			info.AddValue("values", modifiedValues.Values.ToList(), typeof(List<Dictionary<string, object>>));
		}

		/**
		 * Serialized constructor.
		 */
		public pb_ComponentDiff(SerializationInfo info, StreamingContext context)
		{
			modifiedValues = new Dictionary<Component, Dictionary<string, object>>();
			keys = (List<System.Type>) info.GetValue("components", typeof(List<System.Type>));
			values = (List<Dictionary<string, object>>) info.GetValue("values", typeof(List<Dictionary<string, object>>));
		}

		/**
		 * Add a diff entry for a component.  `component` points to the edited component, name is the variable name, and 
		 * value is the new value.
		 */
		public static void AddDiff(Component component, string name, object value)
		{
			GameObject go = component.gameObject;

			pb_MetaDataComponent md_component = go.GetComponent<pb_MetaDataComponent>();

			if(md_component == null)
				md_component = go.AddComponent<pb_MetaDataComponent>();

			pb_ComponentDiff diff = md_component.metadata.componentDiff;

			Dictionary<string, object> v;

			if(diff.modifiedValues.TryGetValue(component, out v))
			{
				if(v.ContainsKey(name))
					v[name] = value;
				else
					v.Add(name, value);
			}
			else
			{
				diff.modifiedValues.Add(component, new Dictionary<string, object>() { {name, value} } );
			}
		}


		/**
		 * Called after an object is deserialized.  This interates through components and sets the modified values,
		 * while simultaneously rebuilding modifiedValues so that the keys point to actual component objects.
		 */
		public void ApplyPatch(GameObject target)
		{
			if(keys.Count < 1 || values.Count != keys.Count)
				return;

			modifiedValues = new Dictionary<Component, Dictionary<string, object>>();

			int entries = keys.Count;

			/// if a component has multiple instances on an object, this will make sure that they remain distinct (probably)
			Dictionary<System.Type, int> dup_components = new Dictionary<System.Type, int>();

			for(int i = 0; i < entries; i++)
			{
				Component[] components = target.GetComponents(keys[i]);

				if( dup_components.ContainsKey(keys[i]) )
					dup_components[keys[i]]++;
				else
					dup_components.Add(keys[i], 0);

				int index = System.Math.Min(dup_components[keys[i]], components.Length-1);

				modifiedValues.Add(components[index], values[i]);

				foreach(KeyValuePair<string, object> kvp in values[i])
				{
					pb_Reflection.SetValue(components[index], kvp.Key, kvp.Value);
					// Debug.LogWarning("Failed patching property: " + kvp.Key + "  Value (" + kvp.Value.GetType() + "): " + kvp.Value.ToString());
				}
			}
		}
	}
}