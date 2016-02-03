using UnityEngine;
using System.Collections;

namespace GILES.Interface
{
	/**
	 * Static GUI methods for working with user input.
	 */
	public class pb_GUI
	{
		/// Used to relay the current width of a rendering view.  Do not use this for anything unless
		/// you set it explicitly prior to use.
		internal static int viewWidth;

		/**
		 * Defines a standard style for header labels in inspectors.
		 */
		public static void HeaderLabel(string label)
		{
			GUILayout.Label("==== " + label + " ====");
		}

		/**
		 * Defines a standard style for horizontal inline labels in inspectors.
		 */
		public static void InspectorLabel(string label)
		{
			GUILayout.Label(label, GUILayout.MaxWidth(120));
		}

		/**
		 * A float field that accepts numeric input.  May result in precision loss.
		 */
		public static float FloatField(float value, params GUILayoutOption[] layoutOptions)
		{
			string str = value.ToString("G");

			str = GUILayout.TextField(str, layoutOptions);

			float val;

			if(float.TryParse(str, out val) && !Mathf.Approximately(value, val))
				return val;
			else
				return value;
		}

		/**
		 * An integer field that accepts numeric input.
		 */
		public static int IntField(int value)
		{
			string str = value.ToString();

			str = GUILayout.TextField(str);

			int val;

			if(int.TryParse(str, out val))
				return val;
			else
				return value;
		}

		/**
		 * A field for editing enum values
		 */
		public static int EnumField(System.Enum value)
		{
			var values = System.Enum.GetValues(value.GetType());

			float v = (float) System.Convert.ToInt32(value);
			v = GUILayout.HorizontalSlider(v, 0, values.Length);

			return (int) v;
		}

		/**
		 * A field for editing Vector2 values.
		 */
		public static Vector2 Vector2Field(Vector2 value)
		{
			GUILayout.BeginHorizontal();

				value.x = pb_GUI.FloatField(value.x);
				value.y = pb_GUI.FloatField(value.y);

			GUILayout.EndHorizontal();

			return value;
		}

		/**
		 * A field for editing Vector3 values.
		 */
		public static Vector3 Vector3Field(Vector3 value, int width = 0)
		{		
			GUILayout.BeginHorizontal();

			if(width > 0)
			{
				width = width / 3 - 1;

				value.x = pb_GUI.FloatField(value.x, GUILayout.MinWidth(width), GUILayout.MaxWidth(width));
				value.y = pb_GUI.FloatField(value.y, GUILayout.MinWidth(width), GUILayout.MaxWidth(width));
				value.z = pb_GUI.FloatField(value.z, GUILayout.MinWidth(width), GUILayout.MaxWidth(width));

			}
			else
			{
				value.x = pb_GUI.FloatField(value.x);
				value.y = pb_GUI.FloatField(value.y);
				value.z = pb_GUI.FloatField(value.z);
			}
			GUILayout.EndHorizontal();

			return value;
		}

		/**
		 * A field for editing Vector4 values.
		 */
		public static Vector4 Vector4Field(Vector4 value)
		{
			GUILayout.BeginHorizontal();

				value.x = pb_GUI.FloatField(value.x);
				value.y = pb_GUI.FloatField(value.y);
				value.z = pb_GUI.FloatField(value.z);
				value.w = pb_GUI.FloatField(value.w);

			GUILayout.EndHorizontal();

			return value;
		}

		/**
		 * A field for editing Quaternion values.
		 */
		public static Quaternion QuaternionField(Quaternion value)
		{
			GUILayout.BeginHorizontal();

				value.x = pb_GUI.FloatField(value.x);
				value.y = pb_GUI.FloatField(value.y);
				value.z = pb_GUI.FloatField(value.z);
				value.w = pb_GUI.FloatField(value.w);

			GUILayout.EndHorizontal();

			return value;
		}
	}
}