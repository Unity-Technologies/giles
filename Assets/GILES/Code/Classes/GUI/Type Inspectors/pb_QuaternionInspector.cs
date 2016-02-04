using UnityEngine;
using System.Collections;
using System.Reflection;

namespace GILES.Interface
{
	/**
	 * Field editor for Quaternion types.
	 */
	[pb_TypeInspector(typeof(Quaternion))]
	public class pb_QuaternionInspector : pb_TypeInspector
	{
		Quaternion quaternion;

		public UnityEngine.UI.Text title;

		public UnityEngine.UI.InputField
			input_x,
			input_y,
			input_z,
			input_w;

		void OnGUIChanged()
		{
			SetValue(quaternion);
		}

		public override void InitializeGUI()
		{
			title.text = GetName().SplitCamelCase();

#if UNITY_5_2
			input_x.onValueChange.AddListener( OnValueChange_X );
			input_y.onValueChange.AddListener( OnValueChange_Y );
			input_z.onValueChange.AddListener( OnValueChange_Z );
			input_w.onValueChange.AddListener( OnValueChange_W );
#else
			input_x.onValueChanged.AddListener( OnValueChange_X );
			input_y.onValueChanged.AddListener( OnValueChange_Y );
			input_z.onValueChanged.AddListener( OnValueChange_Z );
			input_w.onValueChanged.AddListener( OnValueChange_W );
#endif
		}

		protected override void OnUpdateGUI()
		{
			quaternion = GetValue<Quaternion>();
			input_x.text = quaternion.x.ToString();
			input_y.text = quaternion.y.ToString();
			input_z.text = quaternion.z.ToString();
			input_w.text = quaternion.w.ToString();
		}

		public void OnValueChange_X(string val)
		{
			float v;

			if(float.TryParse(val, out v))
			{
				quaternion.x = v;
				OnGUIChanged();
			}
		}

		public void OnValueChange_Y(string val)
		{
			float v;

			if(float.TryParse(val, out v))
			{
				quaternion.y = v;
				OnGUIChanged();
			}
		}

		public void OnValueChange_Z(string val)
		{
			float v;

			if(float.TryParse(val, out v))
			{
				quaternion.z = v;
				OnGUIChanged();
			}
		}

		public void OnValueChange_W(string val)
		{
			float v;

			if(float.TryParse(val, out v))
			{
				quaternion.w = v;
				OnGUIChanged();
			}
		}
	}
}
