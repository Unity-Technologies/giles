using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Reflection;

namespace GILES.Interface
{
    /**
	 * Field editor for enum types.
	 */
    [pb_TypeInspector(typeof(System.Enum))]
    public class pb_EnumInspector : pb_TypeInspector
    {
        object value;

        public Text title;
        public Dropdown dropdown;
        string[] enumNames;		/// the available names of this enum
		System.Array enumValues;    /// the available values of this enum

        void OnGUIChanged()
        {
            SetValue(value);
        }

        public override void InitializeGUI()
        {
            title.text = GetName().SplitCamelCase();
            dropdown.onValueChanged.AddListener(OnClick);

            enumNames = System.Enum.GetNames(declaringType);
            enumValues = System.Enum.GetValues(declaringType);
            RefreshDropdown();
        }

        void OnClick(int index)
        {
            // cycle enum value
            if (enumValues != null)
            {
                int len = enumValues.Length;
                value = enumValues.GetValue(index);

                OnGUIChanged();
            }
        }

        protected override void OnUpdateGUI()
        {
            value = GetValue<object>();

            RefreshDropdown();
        }

        void RefreshDropdown()
        {
            dropdown.ClearOptions();
            List<string> options = new List<string>();

            for (int i = 0; i < enumValues.Length; i++)
            {
                options.Add(enumNames[i].ToString());
            }
            dropdown.AddOptions(options);
        }


    }
}