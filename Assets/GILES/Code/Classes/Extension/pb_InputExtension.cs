using UnityEngine;
using System.Collections;

namespace GILES
{

	/**
	 * Helper methods for Input
	 */
	public static class pb_InputExtension
	{

		public static bool Shift()
		{
			return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		}

		public static bool Control()
		{
			return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
		}
	}
}