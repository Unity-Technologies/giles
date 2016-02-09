using UnityEngine;
using System.Collections;
#if !UNITY_5_2
using UnityEngine.SceneManagement;
#endif

namespace GILES
{
	/**
	 *	A simple script to load Unity scenes.
	 */
	public class pb_LoadLevel : MonoBehaviour
	{
		// The name of the level to load.  Must be added to the build settings.
		public string levelName;

		public void Load()
		{
#if UNITY_5_2
			Application.LoadLevel(levelName);
#else
			SceneManager.LoadScene(levelName);
#endif
		}
	}
}
