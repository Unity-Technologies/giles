using UnityEngine;
using System.Collections;
using System.Reflection;

namespace GILES.Interface
{
	/**
	 * Field editor for Vector2 types.
	 */
	[pb_TypeInspector(typeof(Vector2))]
	public class pb_Vector2Inspector : pb_TypeInspector
	{
		Vector2 vector;

		public UnityEngine.UI.Text title;

		public UnityEngine.UI.InputField input_x, input_y;

		void OnGUIChanged()
		{
			SetValue(vector);
		}

		public override void InitializeGUI()
		{
			title.text = GetName().SplitCamelCase();
#if UNITY_5_2
			input_x.onValueChange.AddListener( OnValueChange_X );
			input_y.onValueChange.AddListener( OnValueChange_Y );
#else
			input_x.onValueChanged.AddListener( OnValueChange_X );
			input_y.onValueChanged.AddListener( OnValueChange_Y );
#endif
		}

		protected override void OnUpdateGUI()
		{
			vector = GetValue<Vector2>();
			input_x.text = vector.x.ToString();
			input_y.text = vector.y.ToString();
		}

		public void OnValueChange_X(string val)
		{
			float v;

			if(float.TryParse(val, out v))
			{
				vector.x = v;
				OnGUIChanged();
			}
		}

		public void OnValueChange_Y(string val)
		{
			float v;

			if(float.TryParse(val, out v))
			{
				vector.y = v;
				OnGUIChanged();
			}
		}
	}
}
