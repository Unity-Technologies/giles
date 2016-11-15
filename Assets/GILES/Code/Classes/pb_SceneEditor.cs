using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace GILES
{
	/**
	 * Scripts wanting to take control of the scene editor must inherit this class.
	 * To activate the editor, set it as the current editor in the pb_InputManager
	 * class.
	 */
	[CreateAssetMenuAttribute(menuName = "Scene Editor", fileName = "RT Scene Editor", order = pb_Config.ASSET_MENU_ORDER)]
	[System.Serializable]
	public abstract class pb_SceneEditor : ScriptableObject
	{
#region Internal

		readonly KeyCode SHORTCUT_TRANSLATE = KeyCode.W;
		readonly KeyCode SHORTCUT_ROTATE = KeyCode.E;
		readonly KeyCode SHORTCUT_SCALE = KeyCode.R;

		internal pb_SelectionHandle handle { get { return pb_SelectionHandle.instance; } }

		// If the mouse down occurred and AcceptMouseInput() returned false, don't send any further mouse events.
		private bool ignoreMouse = false;

		// Mouse position from last OnMouseDownEvent.
		[SerializeField] Vector2 _mouseOrigin;

		// Current mousePosition.
		[SerializeField] Vector2 _mousePosition;

		/**
		 * Called by pb_InputManager when an editor is loaded.
		 */
		internal void Enable()
		{
			handle.OnHandleBegin += OnHandleBegin;
			handle.OnHandleMove += OnHandleMove;
			handle.OnHandleFinish += OnHandleFinish;
			pb_Selection.AddOnSelectionChangeListener( OnSelectionChange );

			pb_InputManager.AddMouseInUseDelegate(
				(Vector2 mpos) => {
					return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
				});

			OnEnable();
		}

		/**
		 * Called by pb_InputManager when an editor is unloaded.
		 */
		internal void Disable()
		{
			handle.OnHandleBegin -= OnHandleBegin;
			handle.OnHandleMove -= OnHandleMove;
			handle.SetIsHidden(true);
			OnDisabled();
		}

		internal void UpdateBase()
		{
			if(Input.GetMouseButtonUp(0))
			{
				if( !ignoreMouse ) // && !IsMouseInUse() )
				{
					OnMouseUp();
				}

				ignoreMouse = false;
			}
			else if (Input.GetMouseButtonDown(0))
			{
				if( IsMouseInUse() )
				{
					ignoreMouse = true;
				}
				else
				{
					ignoreMouse = false;
					OnMouseDown();
				}
			} 
			else if (Input.GetMouseButton(0) && !ignoreMouse)
			{
				// if( !IsMouseInUse() )
				OnMouseMove();
			}

			Update();
		}

		public void OnKeyDownBase()
		{
			if( Input.GetKey(SHORTCUT_TRANSLATE) )
				handle.SetTool(Tool.Position);
			else if( Input.GetKey(SHORTCUT_ROTATE) )
				handle.SetTool(Tool.Rotate);
			else if( Input.GetKey(SHORTCUT_SCALE) )
				handle.SetTool(Tool.Scale);
			else if( Input.GetKey(KeyCode.F))
				OnFrameSelection();

			OnKeyDown();
		}
#endregion

#region Virtual

		/**
		 * Called once per frame.
		 */
		public virtual void Update() {}

		/**
		 * OnGUI forwarded from MonoBehaviour.
		 */
		public virtual void OnGUI() {}

		/**
		 * Called when a valid mouse down event is raised.  This is passed down from pb_InputManager,
		 * which first checks that the interface doesn't need to intercept this click.
		 */
		public virtual void OnMouseDown() {}

		/**
		 * Called on any mouse movement.  If OnMouseDown was intercepted by pb_InputManager, no movement
		 * events will be called until the next valid mouse down event.
		 */
		public virtual void OnMouseMove() {}

		/**
		 * Called when a valid mouse up event is raised.  This is passed down from pb_InputManager,
		 * which first checks that the interface doesn't need to intercept this click.
		 */
		public virtual void OnMouseUp() {}

		/**
		 *	`OnHandleBegin` is called at the start of a handle interaction.  `transform` is the handle's transform
		 *	prior to any movement.
		 */
		public virtual void OnHandleBegin(pb_Transform transform) {}

		/**
		 * Called any time a handle is moved.  OnHandleBegin will always be called prior, and OnHandleFinish will be called
		 * post.
		 */
		public virtual void OnHandleMove(pb_Transform transform) {}

		/**
		 * Called the handle relinquishes control, either due to a mouse up event or canceled action.
		 */
		public virtual void OnHandleFinish() {}

		/**
		 * Called when a user presses a key.  Fired once.
		 */
		public virtual void OnKeyDown() {}

		/**
		 * Called when the user presses the `F` key to frame the current selection.  pb_ObjectEditor forwards this to the 
		 * scene camera to enable zooming to the current selection.
		 */
		public virtual void OnFrameSelection() {}

		/**
		 * Called when an editor is first enabled in the scene.  Initialize temporary resources here.
		 */
		public virtual void OnEnable() {}

		/**
		 * Called when an editor is unloaded from the scene.  Use this to clean up any temporary resources.
		 */
		public virtual void OnDisabled() {}

		/**
		 *  Called when the current selection is modified, either through a new selection or a change in the inspector.
		 *	This is the same as subscribing to pb_Selection.onSelectionChange.
		 */
		public virtual void OnSelectionChange(IEnumerable<GameObject> added) {}

		/**
		 * Override this to determine when the camera receives full control.
		 */
		public virtual bool EnableCameraControls()
		{
			return Input.GetKey(KeyCode.LeftAlt);
		}

		/**
		 * Determines if the mouse should send events to the editor mode.  Base
		 * checks against any registered functions in pb_InputManager and also
		 * against any uGUI functions.  If this returns true, no mouse events 
		 * will be sent to inheriting objects.
		 */
		public virtual bool IsMouseInUse()
		{
			return pb_InputManager.IsMouseInUse();
		}
#endregion

	}
}
