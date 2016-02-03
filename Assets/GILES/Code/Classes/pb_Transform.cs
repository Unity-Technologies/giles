using UnityEngine;
using System.Runtime.Serialization;
using GILES.Serialization;

namespace GILES
{
	[System.Serializable]
	public class pb_Transform : System.IEquatable<pb_Transform>, ISerializable
	{
		// If the matrix needs rebuilt, this will be true.  Used to delay expensive
		// matrix construction til necessary (since t/r/s can change a lot before a
		// matrix is needed).
		private bool dirty = true;

		[SerializeField] private Vector3 _position;
		[SerializeField] private Quaternion _rotation;
		[SerializeField] private Vector3 _scale;

		private Matrix4x4 matrix;

		public Vector3 position 		{ get { return _position; } set { dirty = true; _position = value; } }
		public Quaternion rotation 		{ get { return _rotation; } set { dirty = true; _rotation = value; } }
		public Vector3 scale 			{ get { return _scale; } set { dirty = true; _scale = value; } }

		public static readonly pb_Transform identity = new pb_Transform(Vector3.zero, Quaternion.identity, Vector3.one);

		public pb_Transform()
		{
			this.position = Vector3.zero;
			this.rotation = Quaternion.identity;
			this.scale = Vector3.one;
			this.matrix = Matrix4x4.identity;
			this.dirty = false;
		}

		public pb_Transform(Vector3 position, Quaternion rotation, Vector3 scale)
		{
			this.position 	= position;
			this.rotation 	= rotation;
			this.scale		= scale;

			this.matrix 	= Matrix4x4.TRS(position, rotation, scale);
			this.dirty 	= false;
		}

		public pb_Transform(Transform transform)
		{
			this.position 	= transform.position;
			this.rotation 	= transform.localRotation;
			this.scale		= transform.localScale;

			this.matrix 	= Matrix4x4.TRS(position, rotation, scale);
			this.dirty 	= false;
		}

		public pb_Transform(pb_Transform transform)
		{
			this.position 	= transform.position;
			this.rotation 	= transform.rotation;
			this.scale		= transform.scale;

			this.matrix 	= Matrix4x4.TRS(position, rotation, scale);
			this.dirty 	= false;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("position", (Vector3)_position, typeof(Vector3));
			info.AddValue("rotation", (Quaternion)_rotation, typeof(Quaternion));
			info.AddValue("scale", (Vector3)_scale, typeof(Vector3));
		}

		public pb_Transform(SerializationInfo info, StreamingContext context)
		{
			this._position = (Vector3) info.GetValue("position", typeof(Vector3));
			this._rotation = (Quaternion) info.GetValue("rotation", typeof(Quaternion));
			this._scale = (Vector3) info.GetValue("scale", typeof(Vector3));
			this.dirty = true;
		}

		public void SetTRS(Transform trs)
		{
			this.position 	= trs.position;
			this.rotation 	= trs.localRotation;
			this.scale		= trs.localScale;
			this.dirty 		= true;
		}

		bool Approx(Vector3 lhs, Vector3 rhs)
		{
			return 	Mathf.Abs(lhs.x - rhs.x) < Mathf.Epsilon &&
					Mathf.Abs(lhs.y - rhs.y) < Mathf.Epsilon &&
					Mathf.Abs(lhs.z - rhs.z) < Mathf.Epsilon;
		}

		bool Approx(Quaternion lhs, Quaternion rhs)
		{
			return 	Mathf.Abs(lhs.x - rhs.x) < Mathf.Epsilon &&
					Mathf.Abs(lhs.y - rhs.y) < Mathf.Epsilon &&
					Mathf.Abs(lhs.z - rhs.z) < Mathf.Epsilon &&
					Mathf.Abs(lhs.w - rhs.w) < Mathf.Epsilon;
		}

		public bool Equals(pb_Transform rhs)
		{
			return 	Approx(this.position, rhs.position) &&
					Approx(this.rotation, rhs.rotation) &&
					Approx(this.scale, rhs.scale);
		}

		public override bool Equals(System.Object rhs)
		{
			return rhs is pb_Transform && this.Equals( (pb_Transform) rhs );
		}

		public override int GetHashCode()
		{
			return position.GetHashCode() ^ rotation.GetHashCode() ^ scale.GetHashCode();
		}

		public Matrix4x4 GetMatrix()
		{
			if( !this.dirty )
			{
				return matrix;
			}
			else
			{
				this.dirty = false;
				matrix = Matrix4x4.TRS(position, rotation, scale);
				return matrix;
			}
		}

		public static pb_Transform operator - (pb_Transform lhs, pb_Transform rhs)
		{
			pb_Transform t = new pb_Transform();

			t.position = lhs.position - rhs.position;
			t.rotation = Quaternion.Inverse(rhs.rotation) * lhs.rotation;
			t.scale = new Vector3(	lhs.scale.x / rhs.scale.x,
									lhs.scale.y / rhs.scale.y,
									lhs.scale.z / rhs.scale.z);

			return t;
		}

		public static pb_Transform operator + (pb_Transform lhs, pb_Transform rhs)
		{
			pb_Transform t = new pb_Transform();

			t.position = lhs.position + rhs.position;
			t.rotation = lhs.rotation * rhs.rotation;
			t.scale = new Vector3(	lhs.scale.x * rhs.scale.x,
									lhs.scale.y * rhs.scale.y,
									lhs.scale.z * rhs.scale.z);

			return t;
		}

		public static pb_Transform operator + (Transform lhs, pb_Transform rhs)
		{
			pb_Transform t = new pb_Transform();

			t.position = lhs.position + rhs.position;
			t.rotation = lhs.localRotation * rhs.rotation;
			t.scale = new Vector3(	lhs.localScale.x * rhs.scale.x,
									lhs.localScale.y * rhs.scale.y,
									lhs.localScale.z * rhs.scale.z);

			return t;
		}

		public static bool operator == (pb_Transform lhs, pb_Transform rhs)
		{
			return System.Object.ReferenceEquals(lhs, rhs) || lhs.Equals(rhs);
		}

		public static bool operator != (pb_Transform lhs, pb_Transform rhs)
		{
			return !(lhs == rhs);
		}

		public Vector3 up { get { return rotation * Vector3.up; }	}
		public Vector3 forward { get { return rotation * Vector3.forward; }	}
		public Vector3 right { get { return rotation * Vector3.right; }	}

		public override string ToString()
		{
			return position.ToString("F2") + "\n" + rotation.ToString("F2") + "\n" + scale.ToString("F2");
		}
	}
}