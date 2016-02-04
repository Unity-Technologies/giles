using UnityEngine;
using System.Collections;
#if !UNITY_5_2
using UnityEngine.SceneManagement;
#endif

namespace GILES
{
	/**
	 * Reload this level when Alt-F5 is pressed.
	 */
	public class ReloadLevel : MonoBehaviour
	{
		void Update()
		{
			if(Input.GetKeyDown(KeyCode.F5) && Input.GetKey(KeyCode.LeftAlt))
			{
#if UNITY_5_2
				Application.LoadLevel( Application.loadedLevel );
#else
				SceneManager.LoadScene( SceneManager.GetActiveScene().name );
#endif
			}
		}
	}
}
