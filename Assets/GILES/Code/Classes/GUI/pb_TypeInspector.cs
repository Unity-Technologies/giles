using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Reflection;
using GILES;
using GILES.Serialization;

namespace GILES.Interface
{
	/**
	 * Base class from which type inspectors should inherit.  Type inspectors may be implemented one of two ways:
	 * Using reflection, or using delegates.  The usage depends on the Initialize() constructor used.  If initialized
	 * with an object and PropertyInfo or FieldInfo parameter, the value will be updated and set using reflection.  If
	 * initialized with get/set delegates, the type inspector will query `updateValue` to get the stored value, and on
	 * a change via GUI will call `onValueChanged` to update the stored value.
	 */
	public abstract class pb_TypeInspector : MonoBehaviour
	{
		/// The variable name to display (if null or empty, will attempt to check propertyInfo or fieldInfo for a name).
		public string memberName = null;

		/// The object this value was reflected from.  If inspector uses delegates to set and get value, this may be null.
		protected object target = null;

		/// The PropertyInfo if this type member is a property.  One or both propertyInfo and fieldInfo may be null.
		protected PropertyInfo propertyInfo = null;
		
		/// The FieldInfo if this type member is a field.  One or both propertyInfo and fieldInfo may be null.
		protected FieldInfo fieldInfo = null;

		/// UGUI input fields will fire OnValueChange events when setting values programatically, which can result in 
		/// infinite loops during initialization.  This tells the inspector to ignore the next SetValue call.
		private bool ignoreSetValue = false;

		/// If this value is being inspected as part of a collection, this is the value's index in that collection.
		private int index = -1;

		/// The type that was fed to GetInspector or AddTypeInspector that produced this instance.
		public System.Type declaringType { get; private set; }

		/**
		 * Internal function used to set declaringType (since inheriting classes cannot access protected set methods)
		 */
		internal void SetDeclaringType(System.Type type) { declaringType = type; }

		/// If this type inspector was instantiated by an encompassing object inspector, this may be set to point to the 
		/// parent inspector so that SetValue and GetValue calls can be propagated.
		public pb_TypeInspector parent;

		/**
		 * Initialize this type inspector using reflection to get and set the stored value.
		 */
		public void Initialize(object target, PropertyInfo prop)
		{
			Initialize(null, target, prop);
		}

		/**
		 * Initialize this type inspector with a variable name and using reflection to get and set the stored value.
		 */
		public void Initialize(string name, object target, PropertyInfo prop)
		{
			if(!string.IsNullOrEmpty(name))
				this.memberName = name;
				
			this.target = target;
			this.propertyInfo = prop;
			this.fieldInfo = null;
			this.declaringType = propertyInfo.PropertyType;

			Initialize_INTERNAL();
		}

		/**
		 * Initialize this type inspector using reflection to get and set the stored value.
		 */
		public void Initialize(object target, FieldInfo field)
		{
			Initialize(null, target, field);
		}

		/**
		 * Initialize this type inspector with a variable name and using reflection to get and set the stored value.
		 */
		public void Initialize(string name, object target, FieldInfo field)
		{
			if(!string.IsNullOrEmpty(name))
				this.memberName = name;
				
			this.target = target;
			this.propertyInfo = null;
			this.fieldInfo = field;
			this.declaringType = fieldInfo.FieldType;

			Initialize_INTERNAL();
		}

		/**
		 * Initialize this type inspector using delegates to get and set the stored value.
		 */
		public void Initialize(string name, UpdateValue getStoredValueDelegate, Callback<object> onValueChangedDelegate)
		{
			Initialize(name, getStoredValueDelegate, onValueChangedDelegate, null, null);
		}

		/**
		 * Type inspector initializer when parent object is an array.
		 */
		public void Initialize(string name, UpdateValueWithIndex getStoredValueDelegate, Callback<int, object> onValueChangedDelegate)
		{
			Initialize(name,  null, null, getStoredValueDelegate, onValueChangedDelegate);
		}

		/**
		 * Full initializer with parameters for array elements and a type param to force the element type (which can be necessary in the 
		 * event that UpdateValue returns null and no declaring type is found).
		 */
		public void Initialize(	string name,
								UpdateValue getStoredValueDelegate,
								Callback<object> onValueChangedDelegate,
								UpdateValueWithIndex getStoredValueDelegateIndexed,
								Callback<int, object> onValueChangedDelegateIndexed )
		{
			if(!string.IsNullOrEmpty(name))
				this.memberName = name;
				
			this.updateValue = getStoredValueDelegate;
			this.onValueChanged = onValueChangedDelegate;
			this.updateValueWithIndex = getStoredValueDelegateIndexed;
			this.onValueChangedAtIndex = onValueChangedDelegateIndexed;

			if(declaringType == null)
			{
				object o = updateValue != null ? updateValue() : (updateValueWithIndex != null ? updateValueWithIndex(index) : null);

				if(o != null)
					declaringType = o.GetType();
				else
					declaringType = null;
			}

			Initialize_INTERNAL();
		}

		private void Initialize_INTERNAL()
		{
			gameObject.name = GetName();

			InitializeGUI();
			if(useDefaultSkin) ApplyDefaultSkin();
			UpdateGUI();
		}

		/**
		 * If the type inspector is part of a collection, this relays the collection index to the Get/Set value methods.
		 */
		public void SetIndexInCollection(int index)
		{
			this.index = index;
		}

		public static readonly Color InputField_BackgroundColor = new Color(.32f, .32f, .32f, .8f);
		public static readonly Color InputField_TextColor = Color.white;
		public const int InputField_MinHeight = 30;

		virtual public bool useDefaultSkin { get { return true; } }

		/**
		 * Iterates the type inspector hierarchy and applies default skin stuff (like image, font colors, etc)
		 */
		public void ApplyDefaultSkin()
		{
			foreach(InputField inputField in gameObject.GetComponentsInChildren<InputField>())
			{
				LayoutElement layoutElement = inputField.gameObject.DemandComponent<LayoutElement>();
				layoutElement.minHeight = 30f;
				inputField.textComponent.color = InputField_TextColor;
			}

			foreach(Button button in gameObject.GetComponentsInChildren<Button>())
			{
				LayoutElement layoutElement = button.gameObject.DemandComponent<LayoutElement>();
				layoutElement.minHeight = 30f;
				button.GetComponentInChildren<Text>().color = InputField_TextColor;
			}
		}

		/// Delegate to be invoked by type inspector when updating the internally stored value.  Passes the index corresponding
		/// to a collection that this object belongs to.
		public delegate object UpdateValueWithIndex(int index);

		/// Delegate to be invoked by type inspector when updating the internally stored value.
		public delegate object UpdateValue();

		/// Delegate to be invoked by type inspector when updating the internally stored value.  If this 
		/// is set, GetValue will invoke this delegate instead of calling the reflection values.
		public UpdateValue updateValue;
				
		/// Delegate will be invoked by type inspector when the value has changed internally.  Passes the index
		/// of the changed object as the first parameter.
		// public delegate void OnValueChangedAtIndex(int index, object newValue);	

		/// Delegate to be invoked by type inspector when updating the internally stored value.  If this 
		/// is set, GetValue will invoke this delegate instead of calling the reflection values.
		public UpdateValueWithIndex updateValueWithIndex;

		/// Delegate called when the value is first modified.
		public Callback onValueBeginChange = null;

		/// Delegate called when the value is first modified.
		public Callback<int> onValueBeginChangeAtIndex = null;


		/// Delegate called when the value is changed via GUI.  If this is non-null, reflection setters will not be invoked.
		public Callback<object> onValueChanged = null;

		/// Delegate called when the value is changed via GUI.  If this is non-null, reflection setters will not be invoked.
		public Callback<int, object> onValueChangedAtIndex = null;

		/**
		 * Called after the target is set.  Use this to initialize GUI.
		 */
		public abstract void InitializeGUI();

		/**
		 * Update the displayed values by querying GetValue.
		 */
		public void UpdateGUI()
		{
			ignoreSetValue = true;
			OnUpdateGUI();
			ignoreSetValue = false;
		}

		/**
		 * Called by parent type inspector or component editor when the GUI is repainting with new information.  Usually
		 * the body of this function looks like this:
		 * \code
		 * 	storedValue = GetValue<T>();
		 *	valueLabel.text = storedValue.ToString();
		 */
		protected abstract void OnUpdateGUI();

		/**
		 * If onValueChanged delegate is non-null, the delegate will be invoked passing the new value as a parameter.
		 * Otherwise the value will be set using reflection.
		 */
		protected void SetValue(object value)
		{
			if( ignoreSetValue )	
				return;

			/// keeps track of how many times per-mouse down or key focus
			if(++onValueSetCount == 1 && !GetValue<object>().Equals(value) )
			{
				if( onValueBeginChange != null )
					onValueBeginChange();

				if(onValueBeginChangeAtIndex != null)
					onValueBeginChangeAtIndex(index);

				if(propertyInfo != null)
					Undo.RegisterState(new UndoReflection(target, propertyInfo), "Set " + propertyInfo.Name);

				if(fieldInfo != null)
					Undo.RegisterState(new UndoReflection(target, fieldInfo), "Set " + fieldInfo.Name);
			}

			if(onValueChanged != null)
			{
				onValueChanged(value);
			}
			else
			if(onValueChangedAtIndex != null)
			{
				onValueChangedAtIndex(index, value);
			}
			else
			if(propertyInfo != null)
			{
				try
				{
					if(target == null)
						target = System.Activator.CreateInstance(propertyInfo.PropertyType);

					propertyInfo.SetValue(target, value, null);
				}
				catch (System.Exception e)
				{
					Debug.LogError(e.ToString());
				}
			}
			else
			if(fieldInfo != null && target != null)
			{
				try
				{
					if(target == null)
						target = System.Activator.CreateInstance(fieldInfo.FieldType);

					fieldInfo.SetValue(target, value);
				}
				catch(System.Exception e)
				{
					Debug.LogError(e.ToString());
				}
			}

			OnInspectedValueSet();
		}

		/**
		 * If overriding, be sure to call base.Update().
		 */
		protected virtual void Update()
		{

			if(	Input.GetMouseButtonUp(0) ||
				Input.GetMouseButtonUp(1) ||
				Input.GetMouseButtonUp(2) ||
				Input.GetKeyUp(KeyCode.Tab) ||
				Input.GetKeyUp(KeyCode.Return))
			{
				onValueSetCount = 0;
			}
		}

		/**
		 * A delegate to be called by this type inspector when the value has been set through the GUI.
		 */
		public Callback onTypeInspectorSetValue = null;

		/// How many times OnInspectedValueSet() has been called during this mouse down event.
		private int onValueSetCount = 0;

		/**
		 * Walks upwards through an inspector tree until either parent is null, or the type inspector
		 * is a reflected value and is owned by a component, at which point a diff is added to pb_Metadata
		 * diffs list. 
		 */
		protected virtual void OnInspectedValueSet()
		{
			if( onTypeInspectorSetValue != null )
				onTypeInspectorSetValue();

			if(parent != null)	
			{
				parent.OnInspectedValueSet();
			}
			else if(target is Component)
			{
				if(propertyInfo != null)
					pb_ComponentDiff.AddDiff((Component)target, propertyInfo.Name, GetValue<object>());
				else if(fieldInfo != null)
					pb_ComponentDiff.AddDiff((Component)target, fieldInfo.Name, GetValue<object>());
			}
		}
		
		/**
		 * If propertyInfo or fieldInfo is non-null, this will attempt to GetValue on the target.
		 * If both are null, object is returned.
		 */
		public T GetValue<T>()
		{
			if(updateValue != null)
				return (T) updateValue();

			if(updateValueWithIndex != null)
				return (T) updateValueWithIndex(index);

			if(propertyInfo != null && target != null)
				return (T) propertyInfo.GetValue(target, null);

			if(fieldInfo != null && target != null)
				return (T) fieldInfo.GetValue(target);

			return default(T);
		}

		/**
		 * Return the name of the member being inspected.
		 */
		public virtual string GetName()
		{
			if( !string.IsNullOrEmpty(memberName) )
				return memberName;
			else
			if(propertyInfo != null)
				return propertyInfo.Name;
			else if(fieldInfo != null)
				return fieldInfo.Name;
			else
				return "Generic Type Inspector";
		}
	}	
}