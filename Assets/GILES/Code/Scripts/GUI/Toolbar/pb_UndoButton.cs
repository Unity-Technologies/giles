using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GILES
{
	public class pb_UndoButton : pb_ToolbarButton
	{
		public override string tooltip
		{
			get
			{
				string cur = Undo.GetCurrentUndo();
				
				if(string.IsNullOrEmpty(cur))
					return "Nothing to Undo";
				else
					return "Undo: " + cur;
			}
		}

		protected override void Start()
		{
			base.Start();

			if(Undo.instance.undoStackModified != null)	
				Undo.instance.undoStackModified += UndoStackModified;
			else
				Undo.instance.undoStackModified = UndoStackModified;

			pb_Scene.AddOnLevelLoadedListener( () => { interactable = false; } );
			pb_Scene.AddOnLevelClearedListener( () => { interactable = false; } );

			UndoStackModified();
		}

		public void DoUndo()
		{
			Undo.PerformUndo();
		}

		private void UndoStackModified()
		{
			interactable = !string.IsNullOrEmpty(Undo.GetCurrentUndo());
		}
	}
}