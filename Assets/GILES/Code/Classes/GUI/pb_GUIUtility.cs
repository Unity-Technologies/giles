using UnityEngine;
using UnityEngine.UI;

namespace GILES.Interface
{
	/**
	 * Static helper functions for common GUI operations.
	 */
	public static class pb_GUIUtility
	{
		/// The default color to use as the background for panels.
		public static readonly Color PANEL_COLOR = new Color(.27f, .27f, .27f, .5f);

		/// The default color to use as the background for items in panels.
		public static readonly Color ITEM_BACKGROUND_COLOR = new Color(.15f, .15f, .15f, 1f);

		/// The default padding for UI elements.
		public const int PADDING = 4;

		/// Default padding for panel UI elements.
		/// \sa PADDING
		public static readonly RectOffset PADDING_RECT_OFFSET = new RectOffset(PADDING, PADDING, PADDING, PADDING);

		/// Cache of default font.
		/// \sa DefaultFont()
		private static Font _defaultFont;

		/**
		 * Replacement for UnityEngine.GUIUtility.ScreenToGUIPoint() which seems not to work.
		 */
		public static Vector2 ScreenToGUIPoint(Vector2 v)
		{
			v.y = Screen.height - v.y;
			return v;
		}

		/**
		 * Get default font.
		 */
		public static Font DefaultFont()
		{
			if(_defaultFont == null)
				_defaultFont = (Font) Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

			return _defaultFont;
		}

		/**
		 * Get a named font from the `Resources/Required/Font` folder.
		 */
		public static Font GetFont(string fontName)
		{
			return Resources.Load<Font>("Required/Font/" + fontName);
		}

		/**
		 * Returns a new gameObject a vertical layoutgroup and text component child.
		 */
		public static GameObject CreateLabeledVerticalPanel(string label)
		{
			GameObject go = new GameObject();
			go.name = label;

			go.AddComponent<Image>().color = PANEL_COLOR;

			AddVerticalLayoutGroup(go);

			CreateLabel( label ).transform.SetParent(go.transform);

			return go;
		}

		/**
		 * Create a new gameObject with a `HorizontalLayoutGroup` and default settings.
		 */
		public static GameObject CreateHorizontalGroup()
		{
			GameObject go = new GameObject();

			HorizontalLayoutGroup group = go.AddComponent<HorizontalLayoutGroup>();
			group.padding = new RectOffset(2,2,2,2);
			group.childForceExpandWidth = true;
			group.childForceExpandHeight = false;

			return go;
		}

		/**
		 * Create a new GameObject with `text` and a default font.
		 */
		public static GameObject CreateLabel(string text)
		{
			GameObject go = new GameObject();
			go.name = "Label Field";
			Text field = go.AddComponent<Text>();
			string temp = text.Replace("UnityEngine.","");
			field.text = temp; //This removes the UnityEngine. Prefix from the Inspector.
			//field.text = text;
			field.font = pb_GUIUtility.DefaultFont();
			go.AddComponent<LayoutElement>().minHeight = 24;
			field.alignment = TextAnchor.MiddleLeft;
			return go;
		}

		/**
		 * Add a `VerticalLayoutGroup` component to `go`, using the default parameters.
		 */
		public static void AddVerticalLayoutGroup(GameObject go)
		{
			AddVerticalLayoutGroup(
				go,
				pb_GUIUtility.PADDING_RECT_OFFSET,
				pb_GUIUtility.PADDING,
				true,
				false);
		}

		/**
		 * Add a `VerticalLayoutGroup` component to `go`, using the specified parameters.
		 */
		public static void AddVerticalLayoutGroup(GameObject go, RectOffset padding, int spacing, bool childForceExpandWidth, bool childForceExpandHeight)
		{
			VerticalLayoutGroup group = go.AddComponent<VerticalLayoutGroup>();
			group.padding = padding;
			group.spacing = spacing;
			group.childForceExpandWidth = childForceExpandWidth;
			group.childForceExpandHeight = childForceExpandHeight;
		}

		/**
		 * Get the next selectable to the right, or if null, the next selectable down and left-most.
		 */
		public static Selectable GetNextSelectable(Selectable current)
		{
			if(current == null)
				return null;

			Selectable next = current.FindSelectableOnRight();

			if(next != null)
			{
				return next;
			}
			else
			{
				next = current.FindSelectableOnDown();

				if(next == null)
					return null;

				Selectable left = next;

				while(next != null)
				{
					left = left.FindSelectableOnLeft();

					if(left == null)
						return next;

					next = left;
				}
			}

			return null;
		}

		/**
		 * Return a rect in screen coordinates.
		 */
		public static Rect GetScreenRect(this RectTransform rectTransform)
		{
			Vector3[] world = new Vector3[4];
			rectTransform.GetWorldCorners(world);
			Vector2 min = Vector3.Min(Vector3.Min(Vector3.Min(world[0], world[1]), world[2]), world[3]);
			Vector2 max = Vector3.Max(Vector3.Max(Vector3.Max(world[0], world[1]), world[2]), world[3]);
			return new Rect(min.x, max.y, max.x - min.x, max.y - min.y);
		}
	}
}
