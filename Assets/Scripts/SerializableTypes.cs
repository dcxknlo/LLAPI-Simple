using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SerializableTypes : ScriptableObject {

	[System.Serializable]
	public struct QuaternionStruct
	{
		public byte header;
		public float x;
		public float y;
		public float z;
		public float w;
		private byte size;


		public QuaternionStruct(Quaternion inQuaternion)
		{
			header = (byte)HEADER_TYPES.QUATERNION;

			x = inQuaternion.x;
			y = inQuaternion.y;
			z = inQuaternion.z;
			w = inQuaternion.w;       

			size = 17; // 4 * 4 byte floats + 1 byte header
		}

		public byte[] ToArray()
		{
			MemoryStream stream = new MemoryStream();
			BinaryWriter bWriter = new BinaryWriter(stream);

			bWriter.Write(header);
			bWriter.Write(this.x);
			bWriter.Write(this.y);
			bWriter.Write(this.z);
			bWriter.Write(this.w);

			return stream.ToArray();
		}

		public static QuaternionStruct FromArray(byte[] bytes)
		{
			BinaryReader bReader = new BinaryReader(new MemoryStream(bytes));
			var s = default(QuaternionStruct);

			s.header = bReader.ReadByte();
			s.x = bReader.ReadSingle();
			s.y = bReader.ReadSingle();
			s.z = bReader.ReadSingle();
			s.w = bReader.ReadSingle();

			return s;
		}
		public Quaternion ToQuaternion()
		{
			return new Quaternion (this.x, this.y, this.z, this.w);

		}
	}

	[System.Serializable]
	public struct Vector3Struct
	{
		public byte header;
		public float x;
		public float y;
		public float z;
		private byte size;


		public Vector3Struct(Vector3 vec3)
		{
			header = (byte)HEADER_TYPES.VECTOR3;
			x = vec3.x;
			y = vec3.y;
			z = vec3.z;

			size = 13; // 3 * 4 byte floats  + 1 byte header
		}

		public byte[] ToArray()
		{
			MemoryStream stream = new MemoryStream();
			BinaryWriter bWriter = new BinaryWriter(stream);

			bWriter.Write(header);
			bWriter.Write(this.x);
			bWriter.Write(this.y);
			bWriter.Write(this.z);


			return stream.ToArray();
		}

		public static Vector3Struct FromArray(byte[] bytes)
		{
			BinaryReader bReader = new BinaryReader(new MemoryStream(bytes));
			var s = default(Vector3Struct);

			s.header = bReader.ReadByte();
			s.x = bReader.ReadSingle();
			s.y = bReader.ReadSingle();
			s.z = bReader.ReadSingle();

			return s;
		}
		public Vector3 ToVector3()
		{
			return new Vector3 (this.x, this.y, this.z);

		}

	}
	[System.Serializable]
	public struct floatStruct{
		float f;

		public floatStruct(float inputFloat)
		{
			f = inputFloat;
		}
		public byte[] ToArray()
		{
			MemoryStream stream = new MemoryStream ();
			BinaryWriter bWriter = new BinaryWriter (stream);

			bWriter.Write (this.f);
			return stream.ToArray ();
		}
		public static floatStruct FromArray(byte[] bytes)
		{
			BinaryReader bReader = new BinaryReader(new MemoryStream(bytes));

			var s = default(floatStruct);
			s.f = bReader.ReadSingle ();

			return s;
		}

	}
	[System.Serializable]
	public struct TransformStruct
	{
		byte header;

		public Vector3 pos;
		public Quaternion quat;

		public TransformStruct(Transform inTransform)
		{
			pos = inTransform.position;
			quat = inTransform.rotation;

			header = (byte)HEADER_TYPES.TRANSFORM;
		}
		public byte[] ToArray ()
		{
			MemoryStream stream = new MemoryStream ();
			BinaryWriter bWriter = new BinaryWriter (stream);

			bWriter.Write(header);
			bWriter.Write(this.pos.x);
			bWriter.Write(this.pos.y);
			bWriter.Write(this.pos.z);

			bWriter.Write (this.quat.x);
			bWriter.Write (this.quat.y);
			bWriter.Write(this.quat.z);
			bWriter.Write(this.quat.w);

			return stream.ToArray ();
		}

		public  static TransformStruct FromArray(byte[] bytes)
		{
			BinaryReader bReader = new BinaryReader(new MemoryStream(bytes));
			var s = default(TransformStruct);

			s.header = bReader.ReadByte();
			s.pos.x = bReader.ReadSingle();
			s.pos.y = bReader.ReadSingle();
			s.pos.z = bReader.ReadSingle();

			s.quat.x = bReader.ReadSingle();
			s.quat.y = bReader.ReadSingle();
			s.quat.z = bReader.ReadSingle();
			s.quat.w = bReader.ReadSingle ();

			return s;

		}
		public Vector3 ToPosition()
		{
			return pos;
		}
		public Quaternion ToRotation()
		{
			return quat;
		}
	}
}
