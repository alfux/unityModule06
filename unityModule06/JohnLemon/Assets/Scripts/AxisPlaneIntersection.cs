using UnityEngine;

namespace AlfuxMath
{
	class AxisPlaneIntersection
	{
		private Matrix3x3 matrix;
		private Vector3 solution;
		private Vector3 target;

		public AxisPlaneIntersection()
		{
			this.matrix = null;
			this.solution = new Vector3(float.NaN, float.NaN, float.NaN);
			this.target = new Vector3(float.NaN, float.NaN, float.NaN);
		}

		public AxisPlaneIntersection(
			Vector3 axisPoint, Vector3 axis,
			Vector3 planePoint, Vector3 planeNormal
		) {
			if (Mathf.Abs(axis.x) > 0.01)
			{
				this.XAxisNonZero(axisPoint, axis, planePoint, planeNormal);
			}
			else if (Mathf.Abs(axis.y) > 0.01)
			{
				this.YAxisNonZero(axisPoint, axis, planePoint, planeNormal);
			}
			else if (Mathf.Abs(axis.z) > 0.01)
			{
				this.ZAxisNonZero(axisPoint, axis, planePoint, planeNormal);
			}
			else
			{
				this.matrix = null;
				this.solution = new Vector3(float.NaN, float.NaN, float.NaN);
				this.target = new Vector3(float.NaN, float.NaN, float.NaN);
			}
		}

		void XAxisNonZero(
			Vector3 axisPoint, Vector3 axis,
			Vector3 planePoint, Vector3 planeNormal
		) {
			this.matrix = new Matrix3x3(
				-axis.y      , axis.x       , 0           ,
				-axis.z      , 0            , axis.x      ,
				planeNormal.x, planeNormal.y, planeNormal.z
			);
			this.target = new Vector3(
				axisPoint.y * axis.x - axisPoint.x * axis.y,
				axisPoint.z * axis.x - axisPoint.x * axis.z,
				Vector3.Dot(planePoint, planeNormal)
			);
			this.solution = this.matrix.Inv.Dot(this.target);
		}

		void YAxisNonZero(
			Vector3 axisPoint, Vector3 axis,
			Vector3 planePoint, Vector3 planeNormal
		) {
			this.matrix = new Matrix3x3(
				axis.y       , -axis.x		, 0			   ,
				0            , -axis.z		, axis.y	   ,
				planeNormal.x, planeNormal.y, planeNormal.z
			);
			this.target = new Vector3(
				axisPoint.x * axis.y - axisPoint.y * axis.x,
				axisPoint.z * axis.y - axisPoint.y * axis.z,
				Vector3.Dot(planePoint, planeNormal)
			);
			this.solution = this.matrix.Inv.Dot(this.target);
		}

		void ZAxisNonZero(
			Vector3 axisPoint, Vector3 axis,
			Vector3 planePoint, Vector3 planeNormal
		) {
			this.matrix = new Matrix3x3(
				axis.z		 , 0			, -axis.x	   ,
				0			 , axis.z		, -axis.y	   ,
				planeNormal.x, planeNormal.y, planeNormal.z
			);
			this.target = new Vector3(
				axisPoint.x * axis.z - axisPoint.z * axis.x,
				axisPoint.y * axis.z - axisPoint.z * axis.y,
				Vector3.Dot(planePoint, planeNormal)
			);
			this.solution = this.matrix.Inv.Dot(this.target);
		}

		public Vector3 Solution
		{
			get => this.solution;
		}
	}

}