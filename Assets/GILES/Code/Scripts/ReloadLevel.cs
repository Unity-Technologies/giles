using UnityEngine;
using System.Collections;

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
				Debug.Log("Restarting level!");
				Application.LoadLevel(Application.loadedLevel);
			}
		}
	}
}
