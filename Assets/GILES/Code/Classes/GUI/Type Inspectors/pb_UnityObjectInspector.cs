using UnityEngine;
using System.Collections;
using System.Reflection;

namespace GILES.Interface
{
	/**
	 * Field editor for UnityEngine.Object types.
	 */
	[pb_TypeInspector(typeof(UnityEngine.Object))]
	public class pb_UnityObjectInspector : pb_TypeInspector
	{
		UnityEngine.Object value;

		public UnityEngine.UI.Text title;
		public UnityEngine.UI.InputField dropbox;

		void OnGUIChanged()
		{
			SetValue(value);
		}

		public override void InitializeGUI()
		{
			title.text = GetName().SplitCamelCase();
		}

		protected override void OnUpdateGUI()
		{
			value = GetValue<UnityEngine.Object>();
			dropbox.text = (value == null ? "null" : value.ToString());
		}

		public void OnValueChange(UnityEngine.Object val)
		{
			value = val;
			OnGUIChanged();
		}
	}
}