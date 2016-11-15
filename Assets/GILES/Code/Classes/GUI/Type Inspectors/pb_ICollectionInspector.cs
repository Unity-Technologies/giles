using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Linq;

namespace GILES.Interface
{
	/**
	 * Field editor for collections.
	 */
	[pb_TypeInspector(typeof(ICollection))]
	public class pb_ICollectionInspector : pb_TypeInspector
	{
		/// If a collection count is greater than this value, it won't be expanded into editable fields.
		const int MAX_COLLECTION_LENGTH = 32;

		ICollection value;
		object[] array;

		public UnityEngine.UI.Text title;
		public Transform collection;

		void OnGUIChanged()
		{
			SetValue(value);
		}

		public override void InitializeGUI()
		{
			title.text = GetName().SplitCamelCase();
			
			value = GetValue<ICollection>();

			if(value != null)
			{
				// much boxing
				array = value.Cast<object>().ToArray();

				if(array.Length < 1 || array.Length > 32)
					return;

				if(declaringType == null || declaringType.GetElementType() == null)
					return;

				System.Type elementType = declaringType.GetElementType();

				string typeName = elementType.ToString().Substring(elementType.ToString().LastIndexOf('.') + 1);

				for(int i = 0; i < array.Length; i++)
				{
					pb_TypeInspector inspector = pb_InspectorResolver.GetInspector(elementType);
					inspector.SetIndexInCollection(i);
					inspector.Initialize(	typeName,
											(int index) => { return array[index]; },
											SetValueAtIndex );
					inspector.transform.SetParent(collection);
				}
			}
		}

		private void SetValueAtIndex(int index, object val)
		{
			/// @todo
			Debug.LogWarning("Setting values in a collection is not supported yet!");
			array[index] = val;
		}

		protected override void OnUpdateGUI()
		{
			value = GetValue<ICollection>();

			if(value == null)
				return;

			int prev_length = array.Length;

			array = value.Cast<object>().ToArray();
			
			if(array.Length < 1 || array.Length > 32)
				return;

			if(prev_length != array.Length)	
			{
				foreach(Transform t in transform)
					pb_ObjectUtility.Destroy(t.gameObject);

				InitializeGUI();
			}
		}
	}
}