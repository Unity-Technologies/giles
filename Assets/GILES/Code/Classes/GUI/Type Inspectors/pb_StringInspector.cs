using UnityEngine;
using System.Collections;
using System.Reflection;

namespace GILES.Interface
{
	/**
	 * Field editor for string types.
	 */
	[pb_TypeInspector(typeof(string))]
	public class pb_StringInspector : pb_TypeInspector
	{
		string value;

		public UnityEngine.UI.Text title;
		public UnityEngine.UI.InputField input;

		public override void InitializeGUI()
		{
			title.text = GetName().SplitCamelCase();
			input.onValueChange.AddListener( OnValueChange );
		}

		protected override void OnUpdateGUI()
		{
			value = GetValue<string>();
			input.text = value != null ? value.ToString() : "null";
		}

		public void OnValueChange(string val)
		{
			SetValue(val);
		}
	}
}