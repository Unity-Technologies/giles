using UnityEngine;
using System.Collections;

namespace GILES
{
	/**
	 * Open a Unity scene.
	 */
	public class pb_OpenSceneButton : pb_ToolbarButton
	{
		public string scene;

		public override string tooltip { get { return "Open " + scene; } }

		public void OpenScene()
		{
			Application.LoadLevel(scene);
		}
	}
}