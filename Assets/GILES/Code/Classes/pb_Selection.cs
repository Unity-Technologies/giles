using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GILES
{
	/**
	 * Manages the current selection.
	 */
	public class pb_Selection : pb_MonoBehaviourSingleton<pb_Selection>
	{
		protected override void Awake()
		{
			base.Awake();

			Undo.AddUndoPerformedListener( UndoRedoPerformed );
			Undo.AddRedoPerformedListener( UndoRedoPerformed );
		}

		///< Delegate called when a new GameObject is selected.
		/// \sa AddOnSelectionChangeListener
		public event Callback<IEnumerable<GameObject>> OnSelectionChange;

		/**
		 * Event called on objects that were previously selected but now are not.
		 */
		public event Callback<IEnumerable<GameObject>> OnRemovedFromSelection;

		/**
		 * Add a listener for selection changes.
		 */
		public static void AddOnSelectionChangeListener(Callback<IEnumerable<GameObject>> del)
		{
			if(instance.OnSelectionChange != null)
				instance.OnSelectionChange += del;
			else
				instance.OnSelectionChange = del;
		}

		/**
		 * Add a listener for selection removal.
		 */
		public static void AddOnRemovedFromSelectionListener(Callback<IEnumerable<GameObject>> del)
		{
			if(instance.OnRemovedFromSelection != null)
				instance.OnRemovedFromSelection += del;
			else
				instance.OnRemovedFromSelection = del;
		}

		private List<GameObject> _gameObjects = new List<GameObject>();

		/// A list of the currently selected GameObjects.
		public static List<GameObject> gameObjects { get { return instance._gameObjects; } }

		/**
		 * Clear all objects in the current selection.
		 */
		public static void Clear()
		{			
			if(instance._gameObjects.Count > 0)
			{
				if(	instance.OnRemovedFromSelection != null)
					instance.OnRemovedFromSelection(instance._gameObjects);
			}

			int cleared = instance._Clear();

			if(cleared > 0)
			{
				if( instance.OnSelectionChange != null )
					instance.OnSelectionChange(null);
			}
		}

		/**
		 * Returns the first gameObject in the selection list.
		 */
		public static GameObject activeGameObject
		{
			get
			{
				return instance._gameObjects.Count > 0 ? instance._gameObjects[0] : null;
			}
		}

		/**
		 * Clear the selection lists and set them to `selection`.
		 */
		public static void SetSelection(IEnumerable<GameObject> selection)
		{
			if(	instance.OnRemovedFromSelection != null)
				instance.OnRemovedFromSelection(instance._gameObjects);

			instance._Clear();

			foreach(GameObject go in selection)
				instance._AddToSelection(go);

			if( instance.OnSelectionChange != null )
				instance.OnSelectionChange(selection);
		}

		/**
		 * Clear the selection lists and set them to `selection`.
		 */
		public static void SetSelection(GameObject selection)
		{
			SetSelection(new List<GameObject>() { selection });
		}

		/**
		 * Append `go` to the current selection (doesn't add in the event that it is already in the selection list).
		 */
		public static void AddToSelection(GameObject go)
		{
			instance._AddToSelection(go);

			if(instance.OnSelectionChange != null)
				instance.OnSelectionChange(new List<GameObject>(){ go });
		}

		/**
		 * Remove an object from the current selection.
		 */
		public static void RemoveFromSelection(GameObject go)
		{
			if(instance._RemoveFromSelection(go))
			{
				if( instance.OnRemovedFromSelection != null )
					instance.OnRemovedFromSelection(new List<GameObject>() { go } );

				if( instance.OnSelectionChange != null )	
					instance.OnSelectionChange(null);
			}
		}

		private static void UndoRedoPerformed()
		{
			if( instance.OnSelectionChange != null )
				instance.OnSelectionChange(null);
		}

		/**
		 * Called by code changes to the current selection that should update the pb_SceneEditor in use. 
		 * For example, this is used by the Inspector to update the handles and other scene gizmos when 
		 * changing transform values.
		 */
		public static void OnExternalUpdate()
		{
			if(	instance.OnSelectionChange != null )
				instance.OnSelectionChange(null);
		}

#region Implementation

		private void _InitializeSelected(GameObject go)
		{
			go.AddComponent<pb_SelectionHighlight>();
		}

		private void _DeinitializeSelected(GameObject go)
		{
			pb_SelectionHighlight highlight = go.GetComponent<pb_SelectionHighlight>();

			if(highlight != null)
				pb_ObjectUtility.Destroy(highlight);
		}

		private bool _AddToSelection(GameObject go)
		{
			if(go != null && !_gameObjects.Contains(go))
			{
				_InitializeSelected(go);
				_gameObjects.Add(go);

				return true;
			}
			return false;
		}

		private bool _RemoveFromSelection(GameObject go)
		{
			if(go != null && _gameObjects.Contains(go) )
			{
				pb_ObjectUtility.Destroy(go.GetComponent<pb_SelectionHighlight>());
				_gameObjects.Remove(go);

				return true;
			}

			return false;
		}

		private int _Clear()
		{
			int count = _gameObjects.Count;

			for(int i = 0; i < count; i++)
				_DeinitializeSelected(_gameObjects[i]);
			
			_gameObjects.Clear();

			return count;
		}
#endregion
	}
}
