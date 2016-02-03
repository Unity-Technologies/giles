using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GILES.Interface;

namespace GILES
{
	/**
	 * Responsible for handling mouse and keyboard input and feeding events to scene editor modes.
	 * When extending the runtime editor, you will register pb_SceneEditor objects with this class,
	 * which handles disabling the current editor and setting up the new.
	 */
	public class pb_InputManager : pb_MonoBehaviourSingleton<pb_InputManager> 
	{
		// The editor mode that is used on initialization.  To set a new mode or swap between modes,
		// use pb_InputManager.SetCurrentMode().  See pb_SceneEditor for information about implementing
		// custom editor modes.
		public pb_SceneEditor currentEditor;

		/// Reference to the current EventSystem.
		private EventSystem eventSystem;

		// The mode that is currently in use.  This is the only pb_SceneEditor that will receive
		// events from the input manager.
		[HideInInspector] [SerializeField] public pb_SceneEditor _editor;

		/**
		 * Register a function to be called before mouse input is sent to SceneEditor base, which if return
		 * is true prevents mouse input from going to scene editor.
		 */
		public static void AddMouseInUseDelegate(MouseInUse del)
		{
			if(instance.mouseUsedDelegate == null)
				instance.mouseUsedDelegate = del;
			else
				instance.mouseUsedDelegate += del;
		}

		/**
		 * Remove a mouseInUseDelegate fuction.
		 */
		public static void RemoveMouseInUseDelegate(MouseInUse del)
		{
			instance.mouseUsedDelegate -= del;
		}

		/**
		 * Register a function to be called before keyboard input is sent to SceneEditor base, which if return
		 * is true prevents keyboard input from going to scene editor.
		 */
		public static void AddKeyInUseDelegate(KeyInUse del)
		{
			if(instance.keyUsedDelegate == null)
				instance.keyUsedDelegate = del;
			else
				instance.keyUsedDelegate += del;
		}

		/**
		 * Remove a KeyInUseDelegate fuction.
		 */
		public static void RemoveKeyInUseDelegate(KeyInUse del)
		{
			instance.keyUsedDelegate -= del;
		}

		/**
		 * Multi-cast delegate to be invoked when checking if mouse input should be forwarded to the currently
		 * in-use scene editor.
		 */
		public MouseInUse mouseUsedDelegate = null;

		/**
		 * Multi-cast delegate to be invoked when checking if keyboard input should be forwarded to the currently
		 * in-use scene editor.
		 */
		public KeyInUse keyUsedDelegate = null;

		/**
		 * Set the editor mode to `editor`.  Classes inheriting pb_SceneEditor are the brains
		 * behind level editor interaction - they are fed events and input from pb_InputManager
		 * and can decide what to do with the pb_Scene from there.
		 */
		public void SetEditor(pb_SceneEditor editor)
		{
			if(	_editor != null )
				_editor.Disable();

			_editor = editor;

			if(_editor != null)
				_editor.Enable();
		}

		/**
		 * Return the scene editor that is currently in use.
		 */
		public static pb_SceneEditor GetCurrentEditor()
		{
			if(instance._editor == null)
				Debug.Log("NULL! e r est");

			return instance._editor;
		}

		/**
		 * On initialization, check that this is not a duplicate and set the instance value.
		 */
		protected override void Awake()
		{
			base.Awake();
			Cursor.lockState = CursorLockMode.None;
		}

		/**
		 * Load the camera reference and enable the default editor.
		 */
		protected virtual void Start()
		{
			eventSystem = EventSystem.current;

			pb_Scene.AddOnLevelLoadedListener(OnLevelReset);
			pb_Scene.AddOnLevelClearedListener(OnLevelReset);

			SetEditor(currentEditor);
		}

		/**
		 * The Update function is responsible for intercepting input and deciding where to 
		 * send it.  Some keyboard shortcuts are reserved and will be "used" (ex, Ctrl-Z / Ctrl-Y 
		 * for Undo and Redo).  
		 */
		private void Update ()
		{
			if(_editor == null)
				return;

			if( Input.anyKeyDown && !IsKeyInUse() && !ProcessKeyInput() )
				_editor.OnKeyDownBase();

			_editor.UpdateBase();
		}

		/**
		 * Forward OnGUI to SceneEditor.
		 */
		private void OnGUI()
		{
			_editor.OnGUI();
		}

		private void OnLevelReset()
		{
			// reset the undo queue
			if( Undo.nullableInstance != null )
				Destroy(Undo.nullableInstance);
		}

		/**
		 * If another script wants priority of mouse, this will return true.
		 */
		public static bool IsMouseInUse()
		{
			bool inuse = instance.mouseUsedDelegate != null && instance.mouseUsedDelegate.GetInvocationList().Any(x => ((MouseInUse)x)(Input.mousePosition));
			return inuse;
		}

		/**
		 * If another script wants priority of keyboard presses, this will return true.  Ex: If any UGUI control has focus.
		 */
		public static bool IsKeyInUse()
		{
			return instance.keyUsedDelegate != null && instance.keyUsedDelegate.GetInvocationList().Any(x => ((KeyInUse)x)());
		}

		/**
		 * Checks for a reserved shortcut.  A reserved shortcut is one that is universal
		 * across all editor modes - eg, Undo, redo, delete, etc.
		 */
		private bool ProcessKeyInput()
		{
			/// the event system has a control focused - check for input shortcuts then bail
			if(eventSystem != null && eventSystem.currentSelectedGameObject != null)
			{
				if( Input.GetKey(KeyCode.Tab) )
				{
					Selectable next = pb_GUIUtility.GetNextSelectable(eventSystem.currentSelectedGameObject.GetComponent<Selectable>());

					if (next != null)
					{
						InputField inputfield = next.GetComponent<InputField>();

						if (inputfield != null)
							inputfield.OnPointerClick(new PointerEventData(eventSystem));
					}
				}


				return true;
			}

#if !UNITY_EDITOR
			if( Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.LeftControl) )
#endif
			{
				if( Input.GetKey(KeyCode.Z) )
				{
					Undo.PerformUndo();
					return true;
				}

				if( Input.GetKey(KeyCode.Y) )
				{
					Undo.PerformRedo();
					return true;
				}
			}

			if( Input.GetKey(KeyCode.Backspace))
			{
				Undo.RegisterStates( new IUndo[] { new UndoDelete(pb_Selection.gameObjects), new UndoSelection() }, "Delete Selection" );

				pb_Selection.Clear();
			}

			return false;
		}
	}
}
