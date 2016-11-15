using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GILES.Interface
{
	/**
	 * Defines a set of colors and fonts / sizes.
	 */
	[System.Serializable]
	[CreateAssetMenuAttribute(menuName = "Level Editor GUI Style", fileName = "RT GUI Style", order = pb_Config.ASSET_MENU_ORDER)]
	public class pb_GUIStyle : ScriptableObject
	{
		[SerializeField] private Font _font;

		/// Background image tint.
		public Color color = Color.white;

		/// The default color of a button.
		public Color normalColor = new Color(.2f, .2f, .2f, .7f);

		/// Background color tint when hovering or highlighted.
		public Color highlightedColor = new Color(.27f, .27f, .27f, 1f);

		/// Background color tint when pressed.
		public Color pressedColor = new Color(.37f, .37f, .37f, 1f);

		/// Background color tint when disabled.
		public Color disabledColor = new Color(.7f, .7f, .7f, 1f);

		/// Image to use (if applicable).
		public Texture2D image;

		/// Sprite to use (if applicable).
		public Sprite sprite;

		/// Font to use.  If null the default Arial is used.
		public Font font
		{
			get
			{
				return _font == null ? (Font) Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") : _font;
			}

			set { _font = value; }
		}

		/// Text tint.
		public Color fontColor = Color.white;

		public static pb_GUIStyle Create(
			Color color,
			Color? normalColor = null,
			Color? highlightedColor = null,
			Color? pressedColor = null,
			Color? disabledColor = null,
			Texture2D image = null,
			Sprite sprite = null,
			Font font = null,
					Color? fontColor = null)
		{
			pb_GUIStyle style = ScriptableObject.CreateInstance<pb_GUIStyle>();

			style.color				= color;
			style.image 			= image;
			style.sprite 			= sprite;
			style.font				= font;
			
			if(normalColor != null) 	style.normalColor		= (Color) normalColor;
			if(highlightedColor != null) style.highlightedColor	= (Color) highlightedColor;
			if(pressedColor != null) 	style.pressedColor		= (Color) pressedColor;
			if(disabledColor != null) 	style.disabledColor 		= (Color) disabledColor;
			if(fontColor != null) 		style.fontColor 			= (Color) fontColor;

			return style;
		}

		public virtual void Apply(Graphic element)
		{
			element.color = element is Text ? fontColor : color;
			pb_Reflection.SetValue(element, "font", font);
			pb_Reflection.SetValue(element, "image", image);
			pb_Reflection.SetValue(element, "sprite", sprite);
		}

		public virtual void Apply(Selectable element)
		{
			ColorBlock block = element.colors;

			block.disabledColor = disabledColor;
			block.highlightedColor = highlightedColor;
			block.normalColor = normalColor;
			block.pressedColor = pressedColor;

			element.colors = block;
		}
	}
}