using UnityEngine;
using System.Collections;
using GILES;
#if !UNITY_5_2
using UnityEngine.SceneManagement;
#endif

namespace GILES.Example
{
	/**
	 * Simple example of a scene loading script.
	 */
	public class pb_SceneLoader : pb_MonoBehaviourSingleton<pb_SceneLoader>
	{
		/// Make this object persistent between scene loads.
		public override bool dontDestroyOnLoad { get { return true; } }

		/// The scene that will be opened and loaded into.
		public string sceneToLoadLevelInto = "Empty Scene";

		[HideInInspector] [SerializeField] private string json = null;

		/**
		 * Call this to load level.
		 */
		public static void LoadScene(string path)
		{
	 		string san = pb_FileUtility.SanitizePath(path, ".json");

			if(!pb_FileUtility.IsValidPath(san, ".json"))
			{
				Debug.LogWarning(san + " not found, or file is not a JSON scene.");
				return;
			}
			else
			{
				instance.json = pb_FileUtility.ReadFile(san);
			}

			SceneManager.LoadScene(instance.sceneToLoadLevelInto);
		}

		private void OnLevelWasLoaded(int i)
		{
			if( SceneManager.GetActiveScene().name == sceneToLoadLevelInto && !string.IsNullOrEmpty(json))
				pb_Scene.LoadLevel(json);
		}
	}
}
