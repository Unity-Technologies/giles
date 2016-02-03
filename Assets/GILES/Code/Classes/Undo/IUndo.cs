using UnityEngine;
using System.Collections;

namespace GILES
{
	/**
	 * An interface that enables objects to store and load undo-able states.
	 */
	public interface IUndo
	{
		/**
		 * Record the current state of the target object. RecordState() should grab the
		 * current state of the object pointed to, not the current state as stored.
		 * 
		 * RecordState should return a hashtable that contains all the information
		 * necessary to reconstitute the object.  How the data is stored is up to the
		 * implementation, since it is also left to the implementing class to reconstruct
		 * itself using this same information in ApplyState.
		 */
		Hashtable RecordState();

		/**
		 * Apply the recorded state to the target object using the values stored by 
		 * RecordState.
		 */
		void ApplyState(Hashtable values);

		/**
		 * Called when a state is purged from the undo or redo stack.  Commonly used by 
		 * objects to release memory once no longer needed.
		 */
		void OnExitScope();
	}
}