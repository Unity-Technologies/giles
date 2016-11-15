using UnityEngine;
using System.Collections;
using System.Reflection;

namespace GILES.Interface
{
	/**
	 * Field editor for Component types.
	 */
	[pb_TypeInspector(typeof(Component))]
	public class pb_ComponentInspector : pb_TypeInspector
	{
		public override void InitializeGUI()
		{}
			
		protected override void OnUpdateGUI()
		{}
	}
}