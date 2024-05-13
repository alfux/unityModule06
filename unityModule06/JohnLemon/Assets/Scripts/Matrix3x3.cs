using UnityEngine;

namespace AlfuxMath
{
	public class Matrix3x3
	{
		public readonly float
			m11, m12, m13,
			m21, m22, m23,
			m31, m32, m33;
		public readonly float det;

		public Matrix3x3() :
			this(
				0, 0, 0,
				0, 0, 0,
				0, 0, 0
			)
		{
			// Empty because everything is done by the third constructor call
		}

		public Matrix3x3(Vector3 c1, Vector3 c2, Vector3 c3) :
			this(
				c1.x, c2.x, c3.x,
				c1.y, c2.y, c3.y,
				c1.z, c2.z, c3.z
			)
		{
			// Empty because everything is done by the third constructor call
		}

		public Matrix3x3(
			float m11, float m12, float m13,
			float m21, float m22, float m23,
			float m31, float m32, float m33
		) {
			this.m11 = m11; this.m12 = m12; this.m13 = m13;
			this.m21 = m21; this.m22 = m22; this.m23 = m23;
			this.m31 = m31; this.m32 = m32; this.m33 = m33;
			this.det = (
				+ this.m11 * (this.m22 * this.m33 - this.m32 * this.m23)
				- this.m12 * (this.m21 * this.m33 - this.m31 * this.m23)
				+ this.m13 * (this.m21 * this.m32 - this.m31 * this.m22)
			);
		}

		public Vector3 Dot(Vector3 v)
		{
			return (
				new Vector3(
					this.m11 * v.x + this.m12 * v.y + this.m13 * v.z,
					this.m21 * v.x + this.m22 * v.y + this.m23 * v.z,
					this.m31 * v.x + this.m32 * v.y + this.m33 * v.z
				)
			);
		}

		public Matrix3x3 Dot(Matrix3x3 n)
		{
			return (
				new Matrix3x3(
					this.m11 * n.m11 + this.m12 * n.m21 + this.m13 * n.m31,
					this.m11 * n.m12 + this.m12 * n.m22 + this.m13 * n.m32,
					this.m11 * n.m13 + this.m12 * n.m23 + this.m13 * n.m33,
					this.m21 * n.m11 + this.m22 * n.m21 + this.m23 * n.m31,
					this.m21 * n.m12 + this.m22 * n.m22 + this.m23 * n.m32,
					this.m21 * n.m13 + this.m22 * n.m23 + this.m23 * n.m33,
					this.m31 * n.m11 + this.m32 * n.m21 + this.m33 * n.m31,
					this.m31 * n.m12 + this.m32 * n.m22 + this.m33 * n.m32,
					this.m31 * n.m13 + this.m32 * n.m23 + this.m33 * n.m33
				)
			);
		}

		public Matrix3x3 Inv
		{
			get
			{
				if (this.det == 0)
				{
					return (
						new Matrix3x3(
							float.NaN, float.NaN, float.NaN,
							float.NaN, float.NaN, float.NaN,
							float.NaN, float.NaN, float.NaN
						)
					);
				}
				return (
					new Matrix3x3(
						(this.m22 * this.m33 - this.m32 * this.m23) / this.det,
						(this.m32 * this.m13 - this.m12 * this.m33) / this.det,
						(this.m12 * this.m23 - this.m22 * this.m13) / this.det,
						(this.m31 * this.m23 - this.m21 * this.m33) / this.det,
						(this.m11 * this.m33 - this.m31 * this.m13) / this.det,
						(this.m21 * this.m13 - this.m11 * this.m23) / this.det,
						(this.m21 * this.m32 - this.m31 * this.m22) / this.det,
						(this.m31 * this.m12 - this.m11 * this.m32) / this.det,
						(this.m11 * this.m22 - this.m21 * this.m12) / this.det
					)
				);
			}
		}

		public override string ToString()
		{
			return (
				$"\n\t{this.m11} {this.m12} {this.m13}\n" +
				$"\t{this.m21} {this.m22} {this.m23}\n" +
				$"\t{this.m31} {this.m32} {this.m33}\n"
			);
		}
	}
}