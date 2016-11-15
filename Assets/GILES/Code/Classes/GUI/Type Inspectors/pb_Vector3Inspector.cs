using UnityEngine;
using System.Collections;
using System.Reflection;

namespace GILES.Interface
{
	/**
	 * Field editor for Vector3 types.
	 */
	[pb_TypeInspector(typeof(Vector3))]
	public class pb_Vector3Inspector : pb_TypeInspector
	{
		Vector3 vector;

		public UnityEngine.UI.Text title;

		public UnityEngine.UI.InputField
			input_x,
			input_y,
			input_z;

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
			input_z.onValueChange.AddListener( OnValueChange_Z );
#else
			input_x.onValueChanged.AddListener( OnValueChange_X );
			input_y.onValueChanged.AddListener( OnValueChange_Y );
			input_z.onValueChanged.AddListener( OnValueChange_Z );
#endif
		}

		protected override void OnUpdateGUI()
		{
			vector = GetValue<Vector3>();

			input_x.text = vector.x.ToString();
			input_y.text = vector.y.ToString();
			input_z.text = vector.z.ToString();
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

		public void OnValueChange_Z(string val)
		{
			float v;

			if(float.TryParse(val, out v))
			{
				vector.z = v;
				OnGUIChanged();
			}
		}
	}
}
