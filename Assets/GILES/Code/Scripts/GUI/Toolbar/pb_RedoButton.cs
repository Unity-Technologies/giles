using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GILES
{
	public class pb_RedoButton : pb_ToolbarButton
	{
		public override string tooltip
		{
			get
			{
				string cur = Undo.GetCurrentRedo();
				
				if(string.IsNullOrEmpty(cur))
					return "Nothing to Redo";
				else
					return "Redo: " + cur;
			}
		}

		protected override void Start()
		{
			base.Start();

			if(Undo.instance.undoStackModified != null)	
				Undo.instance.undoStackModified += RedoStackModified;
			else
				Undo.instance.undoStackModified = RedoStackModified;

			if(Undo.instance.redoStackModified != null)	
				Undo.instance.redoStackModified += RedoStackModified;
			else
				Undo.instance.redoStackModified = RedoStackModified;

			pb_Scene.AddOnLevelLoadedListener( () => { interactable = false; } );
			pb_Scene.AddOnLevelClearedListener( () => { interactable = false; } );

			RedoStackModified();
		}

		public void DoRedo()
		{
			Undo.PerformRedo();
		}

		private void RedoStackModified()
		{
			interactable = !string.IsNullOrEmpty(Undo.GetCurrentRedo());
		}
	}
}