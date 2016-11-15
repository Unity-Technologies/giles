using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GILES
{
	/**
	 * Undo support for deleting gameObjects from scene.  When deleting an object, you simply need to 
	 * create the UndoDelete instance and send it to Undo.RegisterState.  This action performs the delete
	 * for you.
	 */
	public class UndoDelete : IUndo
	{
		public IEnumerable<GameObject> gameObjects;

		public UndoDelete(IEnumerable<GameObject> selection)
		{
			this.gameObjects = selection;
		}

		public Hashtable RecordState()
		{
			Hashtable hash = new Hashtable();

			int n = 0;

			foreach(GameObject go in gameObjects)
			{
				go.SetActive(false);
				hash.Add(n++, go);
			}

			return hash;
		}

		public void ApplyState(Hashtable hash)
		{
			foreach(GameObject go in hash.Values)
				go.SetActive(true);
		}

		public void OnExitScope()
		{
			foreach(GameObject go in gameObjects)
				if(go != null && !go.activeSelf)
					pb_ObjectUtility.Destroy(go);
		}
	}
}