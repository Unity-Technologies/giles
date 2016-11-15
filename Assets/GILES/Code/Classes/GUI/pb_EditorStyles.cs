using UnityEngine;
using UnityEngine.UI;

namespace GILES.Interface
{
	/**
	 * Collection of commonly used GUI styles.
	 */
	public static class pb_EditorStyles
	{

		/**
		 * pb_GUIStyle requires one parameter: color.  All others are optional.
		 *	Color color,
		 *	-Color? normalColor = null,
		 *	-Color? highlightedColor = null,
		 *	-Color? pressedColor = null,
		 *	-Color? disabledColor = null,
		 *	-Texture2D image = null,
		 *	-Sprite sprite = null,
		 *	-Font font = null,
		 *	-Color? fontColor = null)
		 */

		/// Button style.
		public static pb_GUIStyle ButtonStyle = pb_GUIStyle.Create(
			Color.white,
			new Color(.2f, .2f, .2f, .7f),
			new Color(.27f, .27f, .27f, 1f),
			new Color(.37f, .37f, .37f, 1f),
			new Color(.7f, .7f, .7f, 1f),
			null,
			null,
			null,
			Color.white );
		
		/// Panel style.
		public static pb_GUIStyle PanelStyle = pb_GUIStyle.Create(
			new Color(.17f, .17f, .17f, .5f),
			new Color(.17f, .17f, .17f, .17f),
			new Color(.17f, .17f, .17f, .17f),
			new Color(.17f, .17f, .17f, .17f),
			new Color(.17f, .17f, .17f, .17f),
			null,
			null,
			null,
			Color.white );

		/// A title bar in  window.
		public static pb_GUIStyle TitleBarStyle = pb_GUIStyle.Create(
			new Color(.27f, .27f, .27f, .7f),
			fontColor: Color.white );
	}
}