using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GILES
{
	/**
	 * The Undo class is responsible for managing the undo state and stack.  It is implemented as a singleton with
	 * a static interface.  Register IUndo inheriting actions with Undo.RegisterState or Undo.RegisterStates, and
	 * use Undo.PerformUndo and Undo.PerformRedo to step through the stack.
	 * \sa IUndo, UndoDelete, UndoInstantiate, UndoTransform
	 */
	public class Undo : pb_ScriptableObjectSingleton<Undo>
	{	
		/**
		 * Stores a summary of the action, the target (IUndo inheriting object), and a hashtable of values representing
		 * object state at time of undo snapshot.
		 */
		protected class UndoState
		{
			// A summary of the undo action.
			public string message;

			// The object that is targeted by this action.
			public IUndo target;

			// A collection of values representing the state of `target`.  Will by passed to IUndo::ApplyState.
			public Hashtable values;

			/**
			 * Initialize a new UndoState object with an IUndo object and summary of the undo-able action.
			 */
			public UndoState(IUndo target, string msg)
			{
				this.target = target;
				this.message = msg;
				this.values = target.RecordState();
			}

			/**
			 * Reverts the IUndo state by calling IUndo::ApplyState()
			 */
			public void Apply()
			{
				target.ApplyState(values);
			}

			/**
			 * Returns the summary of this undo action.
			 */
			public override string ToString()
			{
				return message;
			}
		}

		public Callback undoPerformed = null;
		public Callback redoPerformed = null;
		public Callback undoStackModified = null;
		public Callback redoStackModified = null;

		/**
		 * Add a callback when an Undo action is performed.
		 */
		public static void AddUndoPerformedListener(Callback callback)
		{
			if(instance.undoPerformed != null)
				instance.undoPerformed += callback;
			else
				instance.undoPerformed = callback;
		}

		/**
		 * Add a callback when an Redo action is performed.
		 */
		public static void AddRedoPerformedListener(Callback callback)
		{
			if(instance.redoPerformed != null)
				instance.redoPerformed += callback;
			else
				instance.redoPerformed = callback;
		}

		[SerializeField] private Stack<List<UndoState>> undoStack = new Stack<List<UndoState>>();
		[SerializeField] private Stack<List<UndoState>> redoStack = new Stack<List<UndoState>>();

		/**
		 * Return a formatted string with a summary of every undo-able action in the undo stack.
		 */
		public static string PrintUndoStack()
		{
			return instance.PrintStack(instance.undoStack);
		}

		/**
		 * Return a formatted string with a summary of every redo-able action in the redo stack.
		 */
		public static string PrintRedoStack()
		{
			return instance.PrintStack(instance.redoStack);
		}

		/**
		 * Create a nicely formatted string with a stack.
		 */
		private string PrintStack(Stack<List<UndoState>> stack)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			foreach(List<UndoState> collection in stack)
			{
				foreach(UndoState state in collection)
				{
					sb.AppendLine(state.ToString());
					break;
				}

				sb.AppendLine("-----");
			}

			return sb.ToString();
		}

		[SerializeField] UndoState currentUndo, currentRedo;

		private void PushUndo(List<UndoState> state)
		{
			currentUndo = state[0];
			undoStack.Push(state);

			if(undoStackModified != null)
				undoStackModified();
		}

		private void PushRedo(List<UndoState> state)
		{
			currentRedo = state[0];
			redoStack.Push(state);

			if(redoStackModified != null)
				redoStackModified();
		}

		private List<UndoState> PopUndo()
		{
			List<UndoState> states = Pop(undoStack);

			if(states == null || undoStack.Count < 1)
				currentUndo = null;
			else
				currentUndo = ((List<UndoState>)undoStack.Peek())[0];

			if(undoStackModified != null)
				undoStackModified();

			return states;
		}

		private List<UndoState> PopRedo()
		{
			List<UndoState> states = Pop(redoStack);

			if(states == null || redoStack.Count < 1)
				currentRedo = null;
			else
				currentRedo = ((List<UndoState>)redoStack.Peek())[0];

			if(redoStackModified != null)
				redoStackModified();

			return states;
		}

		private List<UndoState> Pop(Stack<List<UndoState>> stack)
		{
			if(stack.Count > 0)
				return (List<UndoState>) stack.Pop();
			else
				return null;
		}

		private static void ClearStack(Stack<List<UndoState>> stack)
		{
			foreach(List<UndoState> commands in stack)
				foreach(UndoState state in commands)
					state.target.OnExitScope();

			stack.Clear();
		}

		/**
		 * Get the message from the last registered IUndo, or if PerformRedo was called more recently,
		 * this will be the currently queued undo action.
		 */
		public static string GetCurrentUndo()
		{
			return instance.currentUndo == null ? "" : instance.currentUndo.message;
		}

		/**
		 * Get the message accompanying the next queued redo action.
		 */
		public static string GetCurrentRedo()
		{
			return instance.currentRedo == null ? "" : instance.currentRedo.message;
		}

		/**
		 * Register a new undoable state with message.  Message should describe the action that will be
		 * undone.
		 * \sa IUndo
		 */
		public static void RegisterState(IUndo target, string message)
		{
			ClearStack(instance.redoStack);
			instance.currentRedo = null;
			instance.PushUndo(new List<UndoState>() { new UndoState(target, message) });
		}

		/**
		 * Register a collection of undoable states with message.  Message should describe the action that
		 * will be undone.
		 * \sa IUndo
		 */
		public static void RegisterStates(IEnumerable<IUndo> targets, string message)
		{
			ClearStack(instance.redoStack);
			instance.currentRedo = null;
			List<UndoState> states = targets.Select(x => new UndoState(x, message)).ToList();
			instance.PushUndo(states);
		}

		/**
		 * Applies the currently queued Undo state.
		 */
		public static void PerformUndo()
		{
			List<UndoState> states = instance.PopUndo();

			if(states == null)
				return;

			instance.PushRedo(states.Select(x => new UndoState(x.target, x.message)).ToList());

			foreach(UndoState state in states)
			{
				state.Apply();
			}

			if( instance.undoPerformed != null )
				instance.undoPerformed();
		}

		/**
		 * If the Redo stack exists, this applies the queued Redo action.  Redo is cleared on Undo.RegisterState 
		 * or Undo.RegisterStates calls.
		 */
		public static void PerformRedo()
		{
			List<UndoState> states = instance.PopRedo();

			if( states == null )
				return;

			instance.PushUndo(states.Select(x => new UndoState(x.target, x.message)).ToList());

			foreach(UndoState state in states)
				state.Apply();

			if(	instance.redoPerformed != null )
				instance.redoPerformed();
		}	
	}
}