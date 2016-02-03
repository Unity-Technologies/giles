using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GILES.Interface
{
	/**
	 * A LayoutElement that calculates width based on a text component's width.
	 */
	public class pb_LayoutElementText : LayoutElement
	{
		public bool expandWidth = true, expandHeight = false;
		public Text text;
		public float paddingWidth = 4f, paddingHeight = 4f;

		public override float minWidth
		{
			get { return GetTextWidth(); }
		}

		public override float preferredWidth
		{
			get { return GetTextWidth(); }
		}

		public override float minHeight
		{
			get { return GetTextHeight(); }
		}

		public override float preferredHeight
		{
			get { return GetTextHeight(); }
		}

		float GetTextWidth()
		{
			if(text != null && expandWidth)
				return text.preferredWidth + (paddingWidth * 2f);
			else
				return -1f;
		}

		float GetTextHeight()
		{
			if(text != null && expandHeight)
				return text.preferredHeight + (paddingHeight * 2f);
			else
				return -1f;
		}
	}
}