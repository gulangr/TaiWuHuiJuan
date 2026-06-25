using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Components.LineGenerator
{
	// Token: 0x02000EEF RID: 3823
	public class SmoothCurve
	{
		// Token: 0x0600AF64 RID: 44900 RVA: 0x004FEE20 File Offset: 0x004FD020
		public SmoothCurve(Vector3[] points)
		{
			bool flag = points == null || points.Length < 2;
			if (flag)
			{
				Debug.LogError("SmoothCurve 需要至少两个点。");
				this._points = new Vector3[]
				{
					Vector3.zero,
					Vector3.zero
				};
			}
			else
			{
				this._points = points;
			}
			this._pointCount = this._points.Length;
			this._cumulativeLengths = new float[this._pointCount];
			this._cumulativeLengths[0] = 0f;
			for (int i = 1; i < this._pointCount; i++)
			{
				float segLen = Vector2.Distance(this._points[i - 1], this._points[i]);
				this._cumulativeLengths[i] = this._cumulativeLengths[i - 1] + segLen;
			}
			this._totalLength = this._cumulativeLengths[this._pointCount - 1];
		}

		// Token: 0x0600AF65 RID: 44901 RVA: 0x004FEF18 File Offset: 0x004FD118
		public Vector3[] GetVertices(float invImputeLength, float dotProductThreshold = 0.005f)
		{
			float start = 0f;
			Vector3 prevPoint = this._points[0];
			Vector3 nearestPoint = prevPoint;
			Vector3 prevDirection = Vector3.zero;
			List<Vector3> vertices = new List<Vector3>
			{
				prevPoint
			};
			int skipCount = 0;
			while ((start += invImputeLength) < this._totalLength - invImputeLength / 4f)
			{
				skipCount++;
				Vector3 candidate = this.GetPoint(start);
				Vector3 newDirection = candidate - prevPoint;
				Vector3 nearestDirection = candidate - nearestPoint;
				float newDirMag = newDirection.magnitude;
				float prevDirMag = prevDirection.magnitude;
				float nearestDirMag = nearestDirection.magnitude;
				float normalThreshold = 1f - dotProductThreshold;
				bool flag = newDirMag * 4f < invImputeLength || prevDirMag * 4f < invImputeLength || nearestDirMag * 4f < invImputeLength || Vector3.Dot(newDirection, prevDirection) < newDirMag * prevDirMag * (1f - dotProductThreshold / (float)skipCount) || Vector3.Dot(nearestDirection, prevDirection) < nearestDirMag * prevDirMag * normalThreshold || Vector3.Dot(nearestDirection, newDirection) < nearestDirMag * newDirMag * normalThreshold;
				if (flag)
				{
					prevDirection = newDirection;
					vertices.Add(prevPoint = candidate);
					skipCount = 0;
				}
				nearestPoint = candidate;
			}
			IEnumerable<Vector3> source = vertices;
			Vector3[] points = this._points;
			return source.Append(points[points.Length - 1]).ToArray<Vector3>();
		}

		// Token: 0x0600AF66 RID: 44902 RVA: 0x004FF07C File Offset: 0x004FD27C
		public Vector3 GetPoint(float distance)
		{
			bool flag = distance <= 0f;
			Vector3 result;
			if (flag)
			{
				result = this._points[0];
			}
			else
			{
				bool flag2 = distance >= this._totalLength;
				if (flag2)
				{
					result = this._points[this._pointCount - 1];
				}
				else
				{
					int segIndex = this.FindSegmentIndex(distance);
					float t = (distance - this._cumulativeLengths[segIndex]) / (this._cumulativeLengths[segIndex + 1] - this._cumulativeLengths[segIndex]);
					Vector3 p0;
					Vector3 p;
					Vector3 p2;
					Vector3 p3;
					this.GetControlPoints(segIndex, out p0, out p, out p2, out p3);
					result = this.CatmullRom(p0, p, p2, p3, t);
				}
			}
			return result;
		}

		// Token: 0x0600AF67 RID: 44903 RVA: 0x004FF120 File Offset: 0x004FD320
		private int FindSegmentIndex(float distance)
		{
			int left = 0;
			int right = this._pointCount - 1;
			while (left < right)
			{
				int mid = (left + right) / 2;
				bool flag = this._cumulativeLengths[mid] <= distance;
				if (flag)
				{
					left = mid + 1;
				}
				else
				{
					right = mid;
				}
			}
			return left - 1;
		}

		// Token: 0x0600AF68 RID: 44904 RVA: 0x004FF170 File Offset: 0x004FD370
		private void GetControlPoints(int i, out Vector3 p0, out Vector3 p1, out Vector3 p2, out Vector3 p3)
		{
			p1 = this._points[i];
			p2 = this._points[i + 1];
			bool flag = i == 0;
			if (flag)
			{
				p0 = this._points[0];
			}
			else
			{
				p0 = this._points[i - 1];
			}
			bool flag2 = i + 2 >= this._pointCount;
			if (flag2)
			{
				p3 = this._points[this._pointCount - 1];
			}
			else
			{
				p3 = this._points[i + 2];
			}
		}

		// Token: 0x0600AF69 RID: 44905 RVA: 0x004FF218 File Offset: 0x004FD418
		private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			float t2 = t * t;
			float t3 = t2 * t;
			Vector3 res = 0.5f * (2f * p1 + (-p0 + p2) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 + (-p0 + 3f * p1 - 3f * p2 + p3) * t3);
			res.z = Math.Min(p1.z, p2.z);
			return res;
		}

		// Token: 0x170013D4 RID: 5076
		// (get) Token: 0x0600AF6A RID: 44906 RVA: 0x004FF2F1 File Offset: 0x004FD4F1
		public float TotalLength
		{
			get
			{
				return this._totalLength;
			}
		}

		// Token: 0x04008801 RID: 34817
		private readonly Vector3[] _points;

		// Token: 0x04008802 RID: 34818
		private readonly float[] _cumulativeLengths;

		// Token: 0x04008803 RID: 34819
		private readonly float _totalLength;

		// Token: 0x04008804 RID: 34820
		private readonly int _pointCount;
	}
}
