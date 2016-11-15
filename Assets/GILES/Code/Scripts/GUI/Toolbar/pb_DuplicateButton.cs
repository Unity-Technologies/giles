using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GILES
{
	public class pb_DuplicateButton : pb_ToolbarButton
	{
		protected override void Start()
		{
			base.Start();

			pb_Selection.AddOnSelectionChangeListener( OnSelectionChanged );
			OnSelectionChanged(null);
		}

		public override string tooltip { get { return "Duplicate Selection"; } }

		public void DoDuplicate()
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

		private void OnSelectionChanged(IEnumerable<GameObject> go)
		{
			interactable = pb_Selection.activeGameObject != null;
		}
	}
}