using UnityEngine;
using System.Reflection;
using System.Collections;

namespace GILES
{
	/**
	 * Undo setting a value with reflection.
	 */
	public class UndoReflection : IUndo
	{
		public object target;
		public string memberName;

		public UndoReflection(object target, string member)
		{
			this.target = target;
			this.memberName = member;
		}

		public UndoReflection(object target, MemberInfo info)
		{
			this.target = target;
			this.memberName = info.Name;
		}

		public Hashtable RecordState()
		{
			Hashtable hash = new Hashtable();

			hash.Add("value", pb_Reflection.GetValue<object>(target, memberName));

			return hash;
		}

		public void ApplyState(Hashtable hash)
		{
			pb_Reflection.SetValue(target, memberName, hash["value"]);
		}

		public void OnExitScope() {}
	}
}