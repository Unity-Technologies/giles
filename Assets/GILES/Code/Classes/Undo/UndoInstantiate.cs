using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GILES
{
	/**
	 * Undo support for creating new gameObjects in scene.  Call after instantiating an object.
	 */
	public class UndoInstantiate : IUndo
	{
		public GameObject gameObject;
		[SerializeField] bool initialized = false;

		public UndoInstantiate(GameObject go)
		{
			this.gameObject = go;
			initialized = false;
		}

		public Hashtable RecordState()
		{
			Hashtable hash = new Hashtable();
			hash.Add(gameObject, initialized ? gameObject.activeSelf : false);
			initialized = true;
			return hash;
		}

		public void ApplyState(Hashtable hash)
		{
			foreach(DictionaryEntry kvp in hash)
				((GameObject)kvp.Key).SetActive((bool)kvp.Value);
		}

		public void OnExitScope()
		{
			if( gameObject != null && !gameObject.activeSelf )
				pb_ObjectUtility.Destroy(gameObject);
		}
	}
}