using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GILES
{
	public class pb_DeleteButton : pb_ToolbarButton
	{
		protected override void Start()
		{
			base.Start();

			pb_Selection.AddOnSelectionChangeListener( OnSelectionChanged );
			OnSelectionChanged(null);
		}

		public override string tooltip { get { return "Delete Selection"; } }

		public void DoDelete()
		{
			Undo.RegisterStates( new IUndo[] { new UndoDelete(pb_Selection.gameObjects), new UndoSelection() }, "Delete Selection" );
			pb_Selection.Clear();
		}

		private void OnSelectionChanged(IEnumerable<GameObject> go)
		{
			interactable = pb_Selection.activeGameObject != null;
		}
	}
}