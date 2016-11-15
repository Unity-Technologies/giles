using UnityEngine;
using System.Collections;
using System.Linq;

namespace GILES
{
	/**
	 * Undo support for changing the current selection.
	 */
	public class UndoSelection : IUndo
	{
		public UndoSelection() {}

		public Hashtable RecordState()
		{
			Hashtable hash = new Hashtable();
			int n = 0;

			foreach(GameObject go in pb_Selection.gameObjects)
				hash.Add(n++, go);

			return hash;
		}

		public void ApplyState(Hashtable hash)
		{
			pb_Selection.SetSelection(hash.Values.Cast<GameObject>().ToList());
		}

		public void OnExitScope() {}
	}
}