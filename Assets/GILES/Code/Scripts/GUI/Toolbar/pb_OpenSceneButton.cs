using UnityEngine;
using System.Collections;
#if !UNITY_5_2
using UnityEngine.SceneManagement;
#endif

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
#if UNITY_5_2
			Application.LoadLevel(scene);
#else
			SceneManager.LoadScene(scene);
#endif
		}
	}
}
