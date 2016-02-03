using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GILES.Interface
{
	/**
	 * Component attached to pb_TypeInspectors to guarantee a uniform label width
	 * for int, enum, toggle, float, and string fields.
	 */
	public class pb_InspectorLabelLayoutElement : LayoutElement
	{
		const int LABEL_WIDTH = 64;

		public override float minWidth
		{
			get { return LABEL_WIDTH; }
		}

		public override float preferredWidth
		{
			get { return LABEL_WIDTH; }
		}
	}
}