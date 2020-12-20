using UnityEngine;
using System.Collections;

namespace GILES
{

	/**
	 * Helper methods for Input
	 */
	public static class pb_InputExtension
	{
		///<summary>
		/// Returns true if any shift button is pressed.
		///</summary>
		public static bool Shift()
		{
			return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		}
		///<summary>
		/// Returns true if any control button is pressed.
		///</summary>
		public static bool Control()
		{
			return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
		}
	}
}
