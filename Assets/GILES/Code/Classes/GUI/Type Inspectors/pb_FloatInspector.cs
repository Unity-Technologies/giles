using UnityEngine;
using System.Collections;
using System.Reflection;

namespace GILES.Interface
{
	/**
	 * Field editor for float types.
	 */
	[pb_TypeInspector(typeof(float))]
	public class pb_FloatInspector : pb_TypeInspector
	{
		float value;

		public UnityEngine.UI.Text title;
		public UnityEngine.UI.InputField input;

		void OnGUIChanged()
		{
			SetValue(value);
		}

		public override void InitializeGUI()
		{
			title.text = GetName().SplitCamelCase();
			input.onValueChange.AddListener( OnValueChange );
		}


		protected override void OnUpdateGUI()
		{
			value = GetValue<float>();
			input.text = value.ToString();
		}

		public void OnValueChange(string val)
		{
			float v;

			if(float.TryParse(val, out v))	
			{
				value = v;
				OnGUIChanged();
			}
		}
	}
}