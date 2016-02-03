using UnityEngine;

namespace GILES.Interface
{
	/**
	 * Defines an interface for GUI items contained within a pb_Window that may be resized.
	 */
	public interface pb_IOnResizeHandler
	{
		void OnResize();
	}
}