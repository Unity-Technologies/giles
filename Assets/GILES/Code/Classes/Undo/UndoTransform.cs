using UnityEngine;
using System.Collections;

namespace GILES
{
	public class UndoTransform : IUndo
	{
		public Transform target;

		public UndoTransform(Transform target)
		{
			this.target = target;
		}

		public Hashtable RecordState()
		{
			Hashtable hash = new Hashtable();
			hash.Add("target", target);
			hash.Add("transform", new pb_Transform(target));
			return hash;
		}

		public void ApplyState(Hashtable values)
		{
			this.target = (Transform) values["target"];
			pb_Transform ser = (pb_Transform) values["transform"];
			this.target.SetTRS(ser);
		}

		public void OnExitScope() {}
	}
}