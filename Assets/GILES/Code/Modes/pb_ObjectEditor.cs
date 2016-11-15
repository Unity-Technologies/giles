using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using GILES.Interface;

namespace GILES
{
	/**
	 * A generic editor that allows selecting and moving around GameObjects.
	 */
	public class pb_ObjectEditor : pb_SceneEditor
	{
#region Member

		/// If true, handles will operate in global space.
		private bool global = true;
		
		/// Used to simplify handle transformations for multiple selection movements
		private Transform _mTransform;

		/// Create or retreive the parent transform for handle movements.
		private Transform mTransform
		{
			get
			{
				if (_mTransform == null)
				{
					_mTransform = new GameObject().transform;
					_mTransform.name = "Handle Transform Parent";
				}
				return _mTransform;
			}
		}

		/// Where the handle was before the latest interaction began.  In OnHandleMove `transform - handleTransformOrigin` is the delta to move objects.
		pb_Transform handleTransformOrigin;

		/// Cache of each selected object's transform at the beginning of handle interaction.
		pb_Transform[] selectionTransformOrigin;
#endregion

#region Initialization

		public override void OnEnable()
		{
			dragRectStyle.normal.background = pb_BuiltinResource.GetResource<Texture2D>(pb_BuiltinResource.img_WhiteTexture);
		}
#endregion

#region Handles

		public override void OnHandleBegin(pb_Transform transform)
		{
			handleTransformOrigin = transform;
			CacheSelectionTransforms();

			List<IUndo> undoGroup = pb_Selection.gameObjects.Select(x => (IUndo) new UndoTransform(x.transform)).ToList();
			Undo.RegisterStates(undoGroup, handle.GetTool().ToString() + " Tool");
		}

		public override void OnHandleMove(pb_Transform transform)
		{
			pb_Transform delta = (transform - handleTransformOrigin);

			if( global )
			{				
				mTransform.SetTRS(handleTransformOrigin);

				Transform[] parents = new Transform[pb_Selection.gameObjects.Count];

				for(int i = 0; i < pb_Selection.gameObjects.Count; i++)
				{
					GameObject go = pb_Selection.gameObjects[i];
					go.transform.SetTRS(selectionTransformOrigin[i]);
					parents[i] = go.transform.parent;
					go.transform.parent = mTransform;
				}

				mTransform.SetTRS(transform);

				for(int i = 0; i < pb_Selection.gameObjects.Count; i++)
				{
					pb_Selection.gameObjects[i].transform.parent = parents[i];
				}
			}
			else
			{
				for(int i = 0; i < pb_Selection.gameObjects.Count; i++)
				{
					GameObject go = pb_Selection.gameObjects[i];
					go.transform.SetTRS( selectionTransformOrigin[i] + delta );
				}
			}
		}

		/**
		 * Store the transform of each selected gameObject in selectionTransformOrigin array.
		 */
		void CacheSelectionTransforms()
		{
			int count = pb_Selection.gameObjects.Count;
			selectionTransformOrigin = new pb_Transform[count];
			for(int i = 0; i < count; i++)
			{
				selectionTransformOrigin[i] = new pb_Transform(pb_Selection.gameObjects[i].transform);
			}
		}
#endregion

#region Mouse Events

		private Vector2 mMouseOrigin = Vector2.zero;
		private bool 	mMouseDragging = false,
						mMouseIsDown = false,
						mDragCanceled = false;
		const float MOUSE_DRAG_DELTA = .2f;
		private Rect dragRect = new Rect(0,0,0,0);
		private GUIStyle dragRectStyle = new GUIStyle();
		public Color dragRectColor = new Color(0f, .75f, 1f, .6f);

		public override void OnGUI()
		{
			/// draw a selection rect
			if(mMouseDragging)
			{
				GUI.color = dragRectColor;
				GUI.Box(dragRect, "", dragRectStyle);
				GUI.color = Color.white;
			}
		}

		public override void OnMouseMove()
		{
			if(handle.InUse())
				return;

			if(mMouseDragging)
			{
				if(pb_InputManager.IsKeyInUse() || pb_InputManager.IsMouseInUse())
				{
					mDragCanceled = true;
					mMouseDragging = false;
					return;
				}

				dragRect.x = Mathf.Min(mMouseOrigin.x, Input.mousePosition.x);
				dragRect.y = Screen.height - Mathf.Max(mMouseOrigin.y, Input.mousePosition.y);
				dragRect.width = Mathf.Abs(mMouseOrigin.x - Input.mousePosition.x);
				dragRect.height = Mathf.Abs(mMouseOrigin.y - Input.mousePosition.y);

				UpdateDragPicker();
			}
			else
			if(mMouseIsDown && !mDragCanceled && Vector2.Distance(mMouseOrigin, Input.mousePosition) > MOUSE_DRAG_DELTA)
			{
				if(pb_Selection.activeGameObject != null)
					Undo.RegisterState(new UndoSelection(), "Change Selection");

				mMouseDragging = true;

				dragRect.x = mMouseOrigin.x;
				dragRect.y = Screen.height - mMouseOrigin.y;
				dragRect.width = 0f;
				dragRect.height = 0f;
			}
		}

		public override void OnMouseDown()
		{
			if(!handle.InUse())
			{
				mMouseOrigin = Input.mousePosition;
				mDragCanceled = false;
				mMouseIsDown = true;
			}
		}
		
		public override void OnMouseUp()
		{
			if( !handle.InUse() )
			{
				if(mMouseDragging || mDragCanceled)
				{
					mDragCanceled = false;
					mMouseDragging = false;
				}
				else if( !IsMouseInUse() )
				{
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

					GameObject hit = pb_HandleUtility.ObjectRaycast(ray, pb_Scene.Children());
					
					if(hit != null)
					{
						Undo.RegisterState(new UndoSelection(), "Change Selection");

						if(!pb_InputExtension.Shift() && !pb_InputExtension.Control())
							pb_Selection.SetSelection(hit);
						else
							pb_Selection.AddToSelection(hit);
					}
					else
					{
						if( pb_Selection.activeGameObject != null )
							Undo.RegisterState(new UndoSelection(), "Change Selection");

						if(!pb_InputExtension.Shift() && !pb_InputExtension.Control())
							pb_Selection.Clear();

						if(pb_Selection.gameObjects.Count < 1)
							handle.SetIsHidden(true);
					}
				}
			}
		}
#endregion

#region Key Events

		public override void OnKeyDown()
		{

#if !UNITY_EDITOR
			if( Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.LeftControl) )
#endif
			{
				/// duplicate
				if( Input.GetKey(KeyCode.D) )
				{
					List<GameObject> newObjects = new List<GameObject>();
					List<IUndo> undo = new List<IUndo>() { new UndoSelection() };

					foreach(GameObject go in pb_Selection.gameObjects)
					{
						GameObject inst = (GameObject) pb_Scene.Instantiate(go);
						newObjects.Add(inst);
						undo.Add(new UndoInstantiate(inst));
					}

					Undo.RegisterStates(undo, "Duplicate Object");

					pb_Selection.SetSelection(newObjects);
				}
			}
		}
#endregion

#region Event

		public override void OnSelectionChange(IEnumerable<GameObject> added)
		{
			if( pb_Selection.activeGameObject != null )
			{
				
				Transform t = pb_Selection.activeGameObject.transform;
				handle.SetTRS(t.position, t.localRotation, Vector3.one);
				handle.SetIsHidden(false);
			}
			else
			{
				handle.SetIsHidden(true);
			}

		}

		public override void OnFrameSelection()
		{
			if(pb_Selection.activeGameObject != null)
				pb_SceneCamera.Focus( pb_Selection.activeGameObject);
			else
				pb_SceneCamera.Focus( Vector3.zero );
		}

		public override bool EnableCameraControls()
		{
			return base.EnableCameraControls() || Input.GetKey(KeyCode.Q);
		}
#endregion

#region Drag Picking

		void UpdateDragPicker()
		{
			Rect screenRect = new Rect(dragRect.x, Screen.height - (dragRect.y + dragRect.height), dragRect.width, dragRect.height);

			List<GameObject> selected = new List<GameObject>();

			foreach(GameObject go in pb_Scene.Children())
			{
				Vector2 pos = Camera.main.WorldToScreenPoint(go.transform.position);

				if(screenRect.Contains(pos))
				{
					selected.Add(go);
				}
			}

			pb_Selection.SetSelection(selected);
		}
#endregion

	}

}
