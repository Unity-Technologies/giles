using UnityEngine;
using System.Collections;
using System.Reflection;

namespace GILES.Interface
{
	/**
	 * Field editor for Material types.
	 */
	[pb_TypeInspector(typeof(Material))]
	public class pb_MaterialInspector : pb_TypeInspector
	{
		Material value;

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
			value = GetValue<Material>();
			dropbox.text = (value == null ? "null" : value.ToString());
		}

		public void OnValueChange(Material val)
		{
			value = val;
			OnGUIChanged();
		}
	}
}