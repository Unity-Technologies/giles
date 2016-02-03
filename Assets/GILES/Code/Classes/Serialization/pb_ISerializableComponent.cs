using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GILES.Serialization
{
	/**
	 * pb_ISerializable interface allows components implementing it to override
	 * the data that is saved when serializing.
	 */
	public interface pb_ISerializableComponent
	{
		/**
		 * Called prior to serializing a component, store any data that should be
		 * serialized in a dictionary with a string key and object value.
		 */
		Dictionary<string, object> PopulateSerializableDictionary();

		/**
		 * Method will be called when rebuilding a component.  `values` contains
		 * the data stored by `PopulateSerializableDictionary()`.
		 */
		void ApplyDictionaryValues(Dictionary<string, object> values);
	}
}