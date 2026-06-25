using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000076 RID: 118
public class LineRenderer2D : BaseMeshEffect
{
	// Token: 0x0600043E RID: 1086 RVA: 0x0001AED8 File Offset: 0x000190D8
	public override void ModifyMesh(VertexHelper verts)
	{
		bool flag = !base.enabled;
		if (!flag)
		{
			verts.Clear();
			IList<Vector2> vertices2;
			if (this.VerticesProvider == null)
			{
				IList<Vector2> vertices = this.Vertices;
				vertices2 = vertices;
			}
			else
			{
				vertices2 = this.VerticesProvider();
			}
			this.GenerateLineMesh(vertices2, verts);
		}
	}

	// Token: 0x0600043F RID: 1087 RVA: 0x0001AF24 File Offset: 0x00019124
	protected void GenerateLineMesh(IList<Vector2> vertices, VertexHelper verts)
	{
		LineRenderer2D.<>c__DisplayClass16_0 CS$<>8__locals1;
		CS$<>8__locals1.verts = verts;
		CS$<>8__locals1.<>4__this = this;
		bool flag = this.Width <= 0f || vertices == null || vertices.Count < 2;
		if (!flag)
		{
			int cornerSubsteps = (int)Math.Ceiling((double)(this.CornerRadius * this.CornerQuality));
			int subLineSubsteps = this.SubLineAmount + 1;
			List<Vector2> positions = new List<Vector2>();
			int i = 0;
			int len = vertices.Count;
			while (i < len)
			{
				Vector2 vertex = vertices[i];
				bool flag2 = !this.Dashed && i != 0 && subLineSubsteps > 1;
				if (flag2)
				{
					Vector2 prev = vertices[i - 1];
					Vector2 arrow = vertex - prev;
					float subLength = arrow.magnitude / (float)(subLineSubsteps + 1);
					arrow = arrow.normalized;
					for (int j = 1; j < subLineSubsteps - 1; j++)
					{
						Vector2 target = prev + arrow * subLength * (float)j;
						positions.Add(target);
					}
				}
				positions.Add(vertex);
				i++;
			}
			List<Vector2> points = new List<Vector2>();
			int k = 0;
			int len2 = positions.Count;
			while (k < len2)
			{
				Vector2 angularPoint = positions[k];
				bool flag3 = k > 0 && k < len2 - 1;
				if (flag3)
				{
					Vector2 p = positions[k - 1];
					Vector2 p2 = positions[k + 1];
					Vector2 d = new Vector2(angularPoint.x - p.x, angularPoint.y - p.y);
					Vector2 d2 = new Vector2(angularPoint.x - p2.x, angularPoint.y - p2.y);
					float angle = (Mathf.Atan2(d.y, d.x) - Mathf.Atan2(d2.y, d2.x)) / 2f;
					float tan = Mathf.Abs(Mathf.Tan(angle));
					float segment = this.CornerRadius / tan;
					float length = d.magnitude;
					float length2 = d2.magnitude;
					float radius = this.CornerRadius;
					float length3 = Mathf.Min(length, length2);
					bool flag4 = segment > length3;
					if (flag4)
					{
						segment = length3;
					}
					Vector2 p1Cross = LineRenderer2D.<GenerateLineMesh>g__GetProportionPoint|16_1(angularPoint, segment, length, d);
					Vector2 p2Cross = LineRenderer2D.<GenerateLineMesh>g__GetProportionPoint|16_1(angularPoint, segment, length2, d2);
					Vector2 dv = new Vector2(angularPoint.x * 2f - p1Cross.x - p2Cross.x, angularPoint.y * 2f - p1Cross.y - p2Cross.y);
					float l = dv.magnitude;
					float d3 = new Vector2(segment, radius).magnitude;
					Vector2 circlePoint = LineRenderer2D.<GenerateLineMesh>g__GetProportionPoint|16_1(angularPoint, d3, l, dv);
					float startAngle = Mathf.Atan2(p1Cross.y - circlePoint.y, p1Cross.x - circlePoint.x);
					float endAngle = Mathf.Atan2(p2Cross.y - circlePoint.y, p2Cross.x - circlePoint.x);
					float sweepAngle = endAngle - startAngle;
					bool flag5 = float.IsNaN(sweepAngle);
					if (flag5)
					{
						points.Add(angularPoint);
					}
					else
					{
						startAngle = endAngle;
						for (sweepAngle = -sweepAngle; sweepAngle > 3.1415927f; sweepAngle = 3.1415927f - sweepAngle)
						{
						}
						int pointsCount = (int)Mathf.Abs(sweepAngle * this.CornerQuality);
						float sign = Mathf.Sign(sweepAngle);
						for (int m = pointsCount - 1; m >= 0; m--)
						{
							points.Add(new Vector2(circlePoint.x + Mathf.Cos(startAngle + sign * (float)m / this.CornerQuality) * radius, circlePoint.y + Mathf.Sin(startAngle + sign * (float)m / this.CornerQuality) * radius));
						}
					}
				}
				else
				{
					points.Add(angularPoint);
				}
				IL_3EF:
				k++;
				continue;
				goto IL_3EF;
			}
			int vertsLength = points.Count * 2;
			Vector2 uv = Vector2.zero;
			Vector2[] result = new Vector2[vertsLength];
			float thetaPrev = Mathf.Atan2(points[1].y - points[0].y, points[1].x - points[0].x);
			float sinThetaPrev = Mathf.Sin(thetaPrev);
			float cosThetaPrev = Mathf.Cos(thetaPrev);
			Vector2[] upon = LineRenderer2D.CalculateUpDownPoints(points[0], sinThetaPrev, cosThetaPrev, this.Width);
			result[0] = upon[0];
			result[1] = upon[1];
			int n = 1;
			int len3 = points.Count - 1;
			float theta;
			float sinTheta;
			float cosTheta;
			Vector2[] upon2;
			while (n < len3)
			{
				theta = Mathf.Atan2(points[n + 1].y - points[n].y, points[n + 1].x - points[n].x);
				sinTheta = Mathf.Sin(theta);
				cosTheta = Mathf.Cos(theta);
				upon2 = LineRenderer2D.CalculateUpDownPoints(points[n], sinTheta, cosTheta, this.Width);
				Vector2[] mediumPoints = LineRenderer2D.CalculateMediumPoint(upon, upon2, sinThetaPrev, cosThetaPrev, sinTheta, cosTheta);
				int i2 = n * 2;
				result[i2] = mediumPoints[0];
				result[i2 + 1] = mediumPoints[1];
				sinThetaPrev = sinTheta;
				cosThetaPrev = cosTheta;
				upon = upon2;
				n++;
			}
			int length4 = points.Count;
			theta = Mathf.Atan2(points[length4 - 1].y - points[length4 - 2].y, points[length4 - 1].x - points[length4 - 2].x);
			sinTheta = Mathf.Sin(theta);
			cosTheta = Mathf.Cos(theta);
			upon2 = LineRenderer2D.CalculateUpDownPoints(points[length4 - 1], sinTheta, cosTheta, this.Width);
			result[vertsLength - 2] = upon2[0];
			result[vertsLength - 1] = upon2[1];
			float allLength = 0f;
			int i3 = 1;
			int len4 = points.Count;
			while (i3 < len4)
			{
				allLength += (points[i3] - points[i3 - 1]).magnitude;
				i3++;
			}
			bool flag6 = this.Dashed && this.DashedSolidLength * this.DashedSpaceLength > 0f;
			if (flag6)
			{
				LineRenderer2D.<>c__DisplayClass16_1 CS$<>8__locals2;
				CS$<>8__locals2.isSpacing = false;
				CS$<>8__locals2.dashedLength = 0.0;
				Vector2 dashedAnchor = result[0];
				Vector2 dashedAnchor2 = result[1];
				List<ValueTuple<Vector2, bool>> dashedResult = new List<ValueTuple<Vector2, bool>>
				{
					new ValueTuple<Vector2, bool>(dashedAnchor, false),
					new ValueTuple<Vector2, bool>(dashedAnchor2, false)
				};
				List<Vector2> dashedVertex = new List<Vector2>();
				int i4 = 1;
				int len5 = points.Count;
				while (i4 < len5)
				{
					Vector2 target2 = result[i4 * 2];
					Vector2 target3 = result[i4 * 2 + 1];
					Vector2 arrow2 = target2 - dashedAnchor;
					Vector2 arrow3 = target3 - dashedAnchor2;
					double diff;
					float arrowLength;
					float arrowLength2;
					while ((double)arrow2.magnitude > (diff = this.<GenerateLineMesh>g__DashedMax|16_2(ref CS$<>8__locals1, ref CS$<>8__locals2) - CS$<>8__locals2.dashedLength))
					{
						arrow2 = arrow2.normalized;
						arrow3 = arrow3.normalized;
						arrowLength = (arrow2 * (float)diff).magnitude;
						arrowLength2 = (arrow3 * (float)diff).magnitude;
						dashedAnchor += arrow2 * arrowLength;
						dashedAnchor2 += arrow3 * arrowLength2;
						bool flag7 = diff > (double)this.MinDiffLineValue;
						if (flag7)
						{
							dashedResult.Add(new ValueTuple<Vector2, bool>(dashedAnchor, CS$<>8__locals2.isSpacing));
							dashedResult.Add(new ValueTuple<Vector2, bool>(dashedAnchor2, CS$<>8__locals2.isSpacing));
						}
						LineRenderer2D.<GenerateLineMesh>g__SwitchDash|16_3(ref CS$<>8__locals2);
						arrow2 = target2 - dashedAnchor;
						arrow3 = target3 - dashedAnchor2;
					}
					arrowLength = arrow2.magnitude;
					arrowLength2 = arrow3.magnitude;
					float lengthDiff = (arrowLength + arrowLength2) * 0.5f;
					bool flag8 = lengthDiff > this.MinDiffLineValue;
					if (flag8)
					{
						dashedResult.Add(new ValueTuple<Vector2, bool>(target2, CS$<>8__locals2.isSpacing));
						dashedResult.Add(new ValueTuple<Vector2, bool>(target3, CS$<>8__locals2.isSpacing));
					}
					CS$<>8__locals2.dashedLength += (double)lengthDiff;
					dashedAnchor = target2;
					dashedAnchor2 = target3;
					i4++;
				}
				double step = 0.0;
				int i5 = 0;
				int len6 = dashedResult.Count;
				while (i5 < len6)
				{
					Color32 color = Color32.Lerp(this.StartColor, this.EndColor, (float)(step / (double)allLength));
					Vector2 v = dashedResult[i5].Item1;
					Vector2 v2 = dashedResult[i5 + 1].Item1;
					dashedVertex.Add(v);
					dashedVertex.Add(v2);
					CS$<>8__locals1.verts.AddVert(v, color, uv);
					CS$<>8__locals1.verts.AddVert(v2, color, uv);
					bool flag9 = this.<GenerateLineMesh>g__CheckOverFlow|16_0(ref CS$<>8__locals1);
					if (flag9)
					{
						return;
					}
					bool flag10 = i5 >= 2;
					if (flag10)
					{
						step += (double)(dashedResult[i5].Item1 - dashedResult[i5 - 2].Item1).magnitude;
					}
					i5 += 2;
				}
				CS$<>8__locals1.verts.AddVert(dashedResult[dashedResult.Count - 2].Item1, this.EndColor, uv);
				CS$<>8__locals1.verts.AddVert(dashedResult[dashedResult.Count - 1].Item1, this.EndColor, uv);
				bool shadowed = this.Shadowed;
				if (shadowed)
				{
					int vertexCount = CS$<>8__locals1.verts.currentVertCount;
					Vector2 nextArrow = (dashedResult[0].Item1 - dashedResult[2].Item1).normalized;
					nextArrow.Set(-nextArrow.y, nextArrow.x);
					CS$<>8__locals1.verts.AddVert(dashedResult[0].Item1 + nextArrow * this.ShadowWidth, this.ShadowTint, uv);
					CS$<>8__locals1.verts.AddVert(dashedResult[1].Item1 - nextArrow * this.ShadowWidth, this.ShadowTint, uv);
					int i6 = 2;
					int len7 = dashedResult.Count;
					while (i6 < len7)
					{
						Vector2 v3 = dashedResult[i6].Item1;
						Vector2 v4 = dashedResult[i6 + 1].Item1;
						Vector2 prev2 = dashedResult[i6 - 2].Item1;
						Vector2 prev3 = dashedResult[i6 - 2 + 1].Item1;
						Vector2 arrow4 = (prev2 - v3).normalized;
						arrow4.Set(-arrow4.y, arrow4.x);
						CS$<>8__locals1.verts.AddVert(v3 + arrow4 * this.ShadowWidth, this.ShadowTint, uv);
						CS$<>8__locals1.verts.AddVert(v4 - arrow4 * this.ShadowWidth, this.ShadowTint, uv);
						bool flag11 = this.<GenerateLineMesh>g__CheckOverFlow|16_0(ref CS$<>8__locals1);
						if (flag11)
						{
							return;
						}
						i6 += 2;
					}
					CS$<>8__locals1.verts.AddVert(dashedResult[dashedResult.Count - 2].Item1, this.EndColor, uv);
					CS$<>8__locals1.verts.AddVert(dashedResult[dashedResult.Count - 1].Item1, this.EndColor, uv);
					int i7 = 0;
					int len8 = dashedVertex.Count - 2;
					while (i7 < len8)
					{
						bool flag12 = !dashedResult[i7 + 2].Item2;
						if (flag12)
						{
							int vertexIndex = i7 + vertexCount;
							CS$<>8__locals1.verts.AddTriangle(vertexIndex, vertexIndex + 1, vertexIndex + 2);
							CS$<>8__locals1.verts.AddTriangle(vertexIndex + 2, vertexIndex + 1, vertexIndex + 3);
						}
						bool flag13 = this.<GenerateLineMesh>g__CheckOverFlow|16_0(ref CS$<>8__locals1);
						if (flag13)
						{
							return;
						}
						i7 += 2;
					}
				}
				int i8 = 0;
				int len9 = dashedVertex.Count - 2;
				while (i8 < len9)
				{
					bool flag14 = !dashedResult[i8 + 2].Item2;
					if (flag14)
					{
						CS$<>8__locals1.verts.AddTriangle(i8, i8 + 1, i8 + 2);
						CS$<>8__locals1.verts.AddTriangle(i8 + 2, i8 + 1, i8 + 3);
					}
					bool flag15 = this.<GenerateLineMesh>g__CheckOverFlow|16_0(ref CS$<>8__locals1);
					if (flag15)
					{
						return;
					}
					i8 += 2;
				}
			}
			else
			{
				float step2 = 0f;
				int i9 = 0;
				int len10 = points.Count;
				while (i9 < len10)
				{
					Color32 color2 = Color32.LerpUnclamped(this.StartColor, this.EndColor, step2 / allLength);
					CS$<>8__locals1.verts.AddVert(result[i9 * 2], color2, uv);
					CS$<>8__locals1.verts.AddVert(result[i9 * 2 + 1], color2, uv);
					bool flag16 = i9 != 0;
					if (flag16)
					{
						step2 += (points[i9] - points[i9 - 1]).magnitude;
					}
					bool flag17 = this.<GenerateLineMesh>g__CheckOverFlow|16_0(ref CS$<>8__locals1);
					if (flag17)
					{
						return;
					}
					i9++;
				}
				bool shadowed2 = this.Shadowed;
				if (shadowed2)
				{
					int vertexCount2 = CS$<>8__locals1.verts.currentVertCount;
					Vector2 nextArrow2 = (result[0] - result[2]).normalized;
					nextArrow2.Set(-nextArrow2.y, nextArrow2.x);
					CS$<>8__locals1.verts.AddVert(result[0] + nextArrow2 * this.ShadowWidth, this.ShadowTint, uv);
					CS$<>8__locals1.verts.AddVert(result[1] - nextArrow2 * this.ShadowWidth, this.ShadowTint, uv);
					int i10 = 1;
					int len11 = points.Count;
					while (i10 < len11)
					{
						Vector2 v5 = result[i10 * 2];
						Vector2 v6 = result[i10 * 2 + 1];
						Vector2 prev4 = result[(i10 - 1) * 2];
						Vector2 prev5 = result[(i10 - 1) * 2 + 1];
						Vector2 arrow5 = (prev4 - v5).normalized;
						arrow5.Set(-arrow5.y, arrow5.x);
						CS$<>8__locals1.verts.AddVert(v5 + arrow5 * this.ShadowWidth, this.ShadowTint, uv);
						CS$<>8__locals1.verts.AddVert(v6 - arrow5 * this.ShadowWidth, this.ShadowTint, uv);
						bool flag18 = this.<GenerateLineMesh>g__CheckOverFlow|16_0(ref CS$<>8__locals1);
						if (flag18)
						{
							return;
						}
						i10++;
					}
					int i11 = 0;
					int len12 = points.Count - 1;
					while (i11 < len12)
					{
						int vertexIndex2 = i11 * 2 + vertexCount2;
						CS$<>8__locals1.verts.AddTriangle(vertexIndex2, vertexIndex2 + 1, vertexIndex2 + 2);
						CS$<>8__locals1.verts.AddTriangle(vertexIndex2 + 2, vertexIndex2 + 1, vertexIndex2 + 3);
						bool flag19 = this.<GenerateLineMesh>g__CheckOverFlow|16_0(ref CS$<>8__locals1);
						if (flag19)
						{
							return;
						}
						i11++;
					}
				}
				int i12 = 0;
				int len13 = points.Count - 1;
				while (i12 < len13)
				{
					int vertexIndex3 = i12 * 2;
					CS$<>8__locals1.verts.AddTriangle(vertexIndex3, vertexIndex3 + 1, vertexIndex3 + 2);
					CS$<>8__locals1.verts.AddTriangle(vertexIndex3 + 2, vertexIndex3 + 1, vertexIndex3 + 3);
					bool flag20 = this.<GenerateLineMesh>g__CheckOverFlow|16_0(ref CS$<>8__locals1);
					if (flag20)
					{
						return;
					}
					i12++;
				}
			}
			this.<GenerateLineMesh>g__CheckOverFlow|16_0(ref CS$<>8__locals1);
		}
	}

	// Token: 0x06000440 RID: 1088 RVA: 0x0001C06C File Offset: 0x0001A26C
	private static Vector2[] CalculateUpDownPoints(Vector2 p, float sinTheta, float cosTheta, float width)
	{
		float widthCosTheta = width * cosTheta;
		float widthSinTheta = width * sinTheta;
		Vector2 pUp = new Vector2(p.x + widthSinTheta, p.y - widthCosTheta);
		Vector2 pDown = new Vector2(p.x - widthSinTheta, p.y + widthCosTheta);
		return new Vector2[]
		{
			pUp,
			pDown
		};
	}

	// Token: 0x06000441 RID: 1089 RVA: 0x0001C0CC File Offset: 0x0001A2CC
	private static Vector2[] CalculateMediumPoint(IReadOnlyList<Vector2> upon1, IReadOnlyList<Vector2> upon2, float sinTheta1, float cosTheta1, float sinTheta2, float cosTheta2)
	{
		float a1B2MinusA2B = -sinTheta1 * cosTheta2 + sinTheta2 * cosTheta1;
		bool flag = Mathf.Abs(a1B2MinusA2B) <= 0.01f;
		Vector2[] result2;
		if (flag)
		{
			result2 = new Vector2[]
			{
				(upon1[0] + upon2[0]) * 0.5f,
				(upon1[1] + upon2[1]) * 0.5f
			};
		}
		else
		{
			Vector2 p1Up = upon1[0];
			Vector2 p1Down = upon1[1];
			Vector2 p2Up = upon2[0];
			Vector2 p2Down = upon2[1];
			float c1B2C2B1Up = (p1Up.x * sinTheta1 - p1Up.y * cosTheta1) * cosTheta2 - (p2Up.x * sinTheta2 - p2Up.y * cosTheta2) * cosTheta1;
			float c1A2C2A1Up = (p1Up.x * sinTheta1 - p1Up.y * cosTheta1) * -sinTheta2 + (p2Up.x * sinTheta2 - p2Up.y * cosTheta2) * sinTheta1;
			float c1B2C2B1Down = (p1Down.x * sinTheta1 - p1Down.y * cosTheta1) * cosTheta2 - (p2Down.x * sinTheta2 - p2Down.y * cosTheta2) * cosTheta1;
			float c1A2C2A1Down = (p1Down.x * sinTheta1 - p1Down.y * cosTheta1) * -sinTheta2 + (p2Down.x * sinTheta2 - p2Down.y * cosTheta2) * sinTheta1;
			Vector2[] result = new Vector2[]
			{
				new Vector2(c1B2C2B1Up / -a1B2MinusA2B, c1A2C2A1Up / a1B2MinusA2B),
				new Vector2(c1B2C2B1Down / -a1B2MinusA2B, c1A2C2A1Down / a1B2MinusA2B)
			};
			result2 = result;
		}
		return result2;
	}

	// Token: 0x06000443 RID: 1091 RVA: 0x0001C278 File Offset: 0x0001A478
	[CompilerGenerated]
	private bool <GenerateLineMesh>g__CheckOverFlow|16_0(ref LineRenderer2D.<>c__DisplayClass16_0 A_1)
	{
		bool flag = A_1.verts.currentVertCount > 52575 || A_1.verts.currentIndexCount > 52575;
		bool result;
		if (flag)
		{
			AdaptableLog.TagWarning("LineRenderer2D", string.Format("overflow! vCount {0}, iCount {1}", A_1.verts.currentVertCount, A_1.verts.currentIndexCount), false);
			A_1.verts.Clear();
			result = true;
		}
		else
		{
			result = false;
		}
		return result;
	}

	// Token: 0x06000444 RID: 1092 RVA: 0x0001C300 File Offset: 0x0001A500
	[CompilerGenerated]
	internal static Vector2 <GenerateLineMesh>g__GetProportionPoint|16_1(Vector2 point, float seg, float segLength, Vector2 dvc)
	{
		float factor = seg / segLength;
		return new Vector2(point.x - dvc.x * factor, point.y - dvc.y * factor);
	}

	// Token: 0x06000445 RID: 1093 RVA: 0x0001C33C File Offset: 0x0001A53C
	[CompilerGenerated]
	private double <GenerateLineMesh>g__DashedMax|16_2(ref LineRenderer2D.<>c__DisplayClass16_0 A_1, ref LineRenderer2D.<>c__DisplayClass16_1 A_2)
	{
		return (double)(A_2.isSpacing ? this.DashedSpaceLength : this.DashedSolidLength);
	}

	// Token: 0x06000446 RID: 1094 RVA: 0x0001C365 File Offset: 0x0001A565
	[CompilerGenerated]
	internal static void <GenerateLineMesh>g__SwitchDash|16_3(ref LineRenderer2D.<>c__DisplayClass16_1 A_0)
	{
		A_0.isSpacing = !A_0.isSpacing;
		A_0.dashedLength = 0.0;
	}

	// Token: 0x040002A2 RID: 674
	public Vector2[] Vertices;

	// Token: 0x040002A3 RID: 675
	public Func<IList<Vector2>> VerticesProvider;

	// Token: 0x040002A4 RID: 676
	public float Width;

	// Token: 0x040002A5 RID: 677
	public int SubLineAmount;

	// Token: 0x040002A6 RID: 678
	public float CornerRadius;

	// Token: 0x040002A7 RID: 679
	public float CornerQuality;

	// Token: 0x040002A8 RID: 680
	public Color32 StartColor;

	// Token: 0x040002A9 RID: 681
	public Color32 EndColor;

	// Token: 0x040002AA RID: 682
	public bool Dashed;

	// Token: 0x040002AB RID: 683
	public float DashedSolidLength;

	// Token: 0x040002AC RID: 684
	public float DashedSpaceLength;

	// Token: 0x040002AD RID: 685
	public bool Shadowed;

	// Token: 0x040002AE RID: 686
	public float ShadowWidth;

	// Token: 0x040002AF RID: 687
	public Color32 ShadowTint;

	// Token: 0x040002B0 RID: 688
	public float MinDiffLineValue = 2f;
}
