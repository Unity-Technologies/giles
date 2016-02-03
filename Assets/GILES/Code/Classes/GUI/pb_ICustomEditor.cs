using UnityEngine;

namespace GILES.Interface
{
	public interface pb_ICustomEditor
	{
		/**
		 * Instantiate a UI panel or layout group that will be added to the inspector window.
		 * \sa pb_TypeInspector, pb_Inspector, pb_ComponentEditor
		 */
		pb_ComponentEditor InstantiateInspector(Component component);
	}
}