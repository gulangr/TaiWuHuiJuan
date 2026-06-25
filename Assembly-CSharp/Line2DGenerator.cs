using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000075 RID: 117
public class Line2DGenerator : BaseMeshEffect
{
	// Token: 0x0600042E RID: 1070 RVA: 0x00019EBC File Offset: 0x000180BC
	public override void ModifyMesh(VertexHelper vh)
	{
		bool flag = !base.enabled;
		if (!flag)
		{
			vh.Clear();
			List<Vector2> vertices = new List<Vector2>();
			vertices.AddRange(this.OverrideVertices ?? this.Vertices);
			bool flag2 = vertices.Count == 0;
			if (!flag2)
			{
				bool enabled = this.RoundedCorners.Enabled;
				if (enabled)
				{
					vertices = Line2DGenerator.ApplyRoundedCorners(vertices, this.RoundedCorners.Quality, this.RoundedCorners.Radius);
				}
				List<Line2DGenerator.VertexSite> sites = Line2DGenerator.GenerateVertexSites(vertices, this.Width);
				bool enabled2 = this.Dashed.Enabled;
				if (enabled2)
				{
					sites = Line2DGenerator.ApplyDashed(sites, this.Dashed, this.Width);
				}
				for (int i = 0; i < sites.Count; i++)
				{
					Line2DGenerator.VertexSite site = sites[i];
					site.Index = i;
					sites[i] = site;
				}
				Line2DGenerator.GenerateMesh(sites, vh, this.FullMesh, this.UVDirection, this.Dashed.Enabled && this.UVDashed, this.Colored);
			}
		}
	}

	// Token: 0x0600042F RID: 1071 RVA: 0x00019FDC File Offset: 0x000181DC
	private static int Comparison(Line2DGenerator.VertexSite a, Line2DGenerator.VertexSite b)
	{
		return a.Index.CompareTo(b.Index);
	}

	// Token: 0x06000430 RID: 1072 RVA: 0x00019FF0 File Offset: 0x000181F0
	private static void GenerateSitesCache(IReadOnlyList<Line2DGenerator.VertexSite> sites)
	{
		Line2DGenerator.CachedBreakSites.Clear();
		Line2DGenerator.CachedNotBreakSites.Clear();
		foreach (Line2DGenerator.VertexSite site in sites)
		{
			(site.Break ? Line2DGenerator.CachedBreakSites : Line2DGenerator.CachedNotBreakSites).Add(site);
		}
		Line2DGenerator.CachedBreakSites.Sort(new Comparison<Line2DGenerator.VertexSite>(Line2DGenerator.Comparison));
		Line2DGenerator.CachedNotBreakSites.Sort(new Comparison<Line2DGenerator.VertexSite>(Line2DGenerator.Comparison));
	}

	// Token: 0x06000431 RID: 1073 RVA: 0x0001A094 File Offset: 0x00018294
	private static Line2DGenerator.VertexSite FindNextSite(IReadOnlyList<Line2DGenerator.VertexSite> sites, int index)
	{
		int count = sites.Count;
		bool flag = count == 0;
		Line2DGenerator.VertexSite result2;
		if (flag)
		{
			result2 = default(Line2DGenerator.VertexSite);
		}
		else
		{
			bool flag2 = count <= 64;
			if (flag2)
			{
				for (int i = 0; i < count; i++)
				{
					bool flag3 = sites[i].Index > index;
					if (flag3)
					{
						return sites[i];
					}
				}
				result2 = default(Line2DGenerator.VertexSite);
			}
			else
			{
				int left = 0;
				int right = count - 1;
				int result = -1;
				while (left <= right)
				{
					int mid = left + right >> 1;
					bool flag4 = sites[mid].Index > index;
					if (flag4)
					{
						result = mid;
						right = mid - 1;
					}
					else
					{
						left = mid + 1;
					}
				}
				result2 = ((result >= 0) ? sites[result] : default(Line2DGenerator.VertexSite));
			}
		}
		return result2;
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x0001A180 File Offset: 0x00018380
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Vector2 Arrow(Vector2 from, Vector2 to)
	{
		return to - from;
	}

	// Token: 0x06000433 RID: 1075 RVA: 0x0001A18C File Offset: 0x0001838C
	private static float RealHalfWidth(Vector2 direction, Vector2 normal, float halfWidth)
	{
		float angle = Vector2.Angle(direction, normal) * 0.017453292f;
		bool flag = Mathf.Approximately(0f, angle) || Mathf.Approximately(3.1415927f, angle);
		float result;
		if (flag)
		{
			result = halfWidth;
		}
		else
		{
			result = Math.Max(halfWidth / Mathf.Cos(angle), 0f);
		}
		return result;
	}

	// Token: 0x06000434 RID: 1076 RVA: 0x0001A1E4 File Offset: 0x000183E4
	private static List<Vector2> ApplyRoundedCorners(IReadOnlyList<Vector2> vertices, int quality, float radius)
	{
		int angleParts = quality + 1;
		List<Vector2> result = new List<Vector2>
		{
			vertices[0]
		};
		for (int i = 1; i < vertices.Count - 1; i++)
		{
			Vector2 curr = vertices[i];
			Vector2 prev = vertices[i - 1];
			Vector2 next = vertices[i + 1];
			Vector2 directionPrev = Line2DGenerator.Arrow(curr, prev).normalized;
			Vector2 directionNext = Line2DGenerator.Arrow(curr, next).normalized;
			Vector2 currentArrow = (directionPrev + directionNext).normalized;
			float r = Mathf.Min(radius, Mathf.Min(Vector2.Distance(curr, prev) * 0.25f, Vector2.Distance(curr, next) * 0.25f));
			float bisectorDistance = r / Mathf.Sin(Vector2.Angle(directionPrev, directionNext) * 0.017453292f / 2f);
			Vector2 bisector = currentArrow * bisectorDistance;
			Vector2 controlPoint = curr + bisector;
			float totalAngleSigned = Vector2.SignedAngle(directionPrev, directionNext);
			float totalAngleSign = Mathf.Sign(totalAngleSigned);
			bool flag = Mathf.Approximately(Mathf.Abs(totalAngleSigned), 180f);
			if (flag)
			{
				result.Add(curr);
			}
			else
			{
				float endPointDistance = r / Mathf.Cos(Vector2.Angle(directionPrev, directionNext) * 0.017453292f / 2f);
				Vector2 endPoint = curr + Line2DGenerator.Arrow(curr, prev).normalized * Mathf.Min(endPointDistance, Vector2.Distance(curr, prev) * 0.5f);
				result.Add(endPoint);
				float totalAngle = Vector2.Angle(directionPrev, directionNext);
				float partAngle = totalAngle / (float)angleParts * totalAngleSign;
				Vector2 baseLine = Line2DGenerator.Arrow(controlPoint, curr);
				float baseAngle = Mathf.Atan2(baseLine.y, baseLine.x) * 57.29578f + totalAngle * 0.5f * totalAngleSign;
				for (int j = 1; j < angleParts; j++)
				{
					float arcAngle = (baseAngle - partAngle * (float)j) * 0.017453292f;
					Vector2 arcPoint = controlPoint + new Vector2(Mathf.Cos(arcAngle), Mathf.Sin(arcAngle)).normalized * r;
					result.Add(arcPoint);
				}
				Vector2 endPoint2 = curr + Line2DGenerator.Arrow(curr, next).normalized * Mathf.Min(endPointDistance, Vector2.Distance(curr, next) * 0.5f);
				result.Add(endPoint2);
			}
		}
		result.Add(vertices[vertices.Count - 1]);
		return result;
	}

	// Token: 0x06000435 RID: 1077 RVA: 0x0001A478 File Offset: 0x00018678
	private static List<Line2DGenerator.VertexSite> ApplyDashed(IReadOnlyList<Line2DGenerator.VertexSite> sites, Line2DGenerator.DashedStyle dashed, float width)
	{
		Line2DGenerator.<>c__DisplayClass20_0 CS$<>8__locals1;
		CS$<>8__locals1.dashed = dashed;
		CS$<>8__locals1.ret = new List<Line2DGenerator.VertexSite>();
		float halfWidth = width * 0.5f;
		CS$<>8__locals1.currentSegmentLength = 0f;
		CS$<>8__locals1.currentSegmentSpacing = false;
		float remainOffset = CS$<>8__locals1.dashed.Offset;
		while (!Mathf.Approximately(remainOffset, 0f))
		{
			float offsetAbs = Mathf.Abs(remainOffset);
			float offsetSign = Mathf.Sign(remainOffset);
			float remain = Line2DGenerator.<ApplyDashed>g__GetCurrentSegmentRemainLength|20_0(ref CS$<>8__locals1);
			bool flag = offsetAbs > remain;
			if (!flag)
			{
				bool flag2 = offsetSign > 0f;
				if (flag2)
				{
					CS$<>8__locals1.currentSegmentLength += offsetAbs;
				}
				else
				{
					Line2DGenerator.<ApplyDashed>g__SwitchCurrentSegmentSpacing|20_2(ref CS$<>8__locals1);
					CS$<>8__locals1.currentSegmentLength += Line2DGenerator.<ApplyDashed>g__GetCurrentSegmentRemainLength|20_0(ref CS$<>8__locals1) - offsetAbs;
				}
				break;
			}
			Line2DGenerator.<ApplyDashed>g__SwitchCurrentSegmentSpacing|20_2(ref CS$<>8__locals1);
			remainOffset -= remain * offsetSign;
		}
		for (int i = 0; i < sites.Count - 1; i++)
		{
			Line2DGenerator.VertexSite curr = sites[i];
			Line2DGenerator.VertexSite next = sites[i + 1];
			float stepLength = Vector2.Distance(curr.Origin.Position, next.Origin.Position);
			bool flag3 = stepLength <= Line2DGenerator.<ApplyDashed>g__GetCurrentSegmentRemainLength|20_0(ref CS$<>8__locals1);
			if (flag3)
			{
				CS$<>8__locals1.currentSegmentLength += Line2DGenerator.<ApplyDashed>g__AddSegment|20_3(curr, next, ref CS$<>8__locals1);
			}
			else
			{
				Line2DGenerator.VertexSite p0 = curr;
				Line2DGenerator.VertexSite p = p0;
				while (stepLength > Line2DGenerator.<ApplyDashed>g__GetCurrentSegmentRemainLength|20_0(ref CS$<>8__locals1))
				{
					p.ReplaceOrigin(p0.Origin.Position + Line2DGenerator.Arrow(p0.Origin.Position, next.Origin.Position).normalized * Line2DGenerator.<ApplyDashed>g__GetCurrentSegmentRemainLength|20_0(ref CS$<>8__locals1));
					Vector2 normalToSideA = Vector2.Perpendicular(p0.Direction);
					p.Normal = (p.Direction = p0.Direction);
					p.SideA.Position = p.Origin.Position + normalToSideA * halfWidth;
					p.SideB.Position = p.Origin.Position - normalToSideA * halfWidth;
					float currentLength = Vector2.Distance(p0.Origin.Position, p.Origin.Position);
					bool flag4 = Mathf.Approximately(currentLength, 0f);
					if (flag4)
					{
						break;
					}
					Line2DGenerator.<ApplyDashed>g__AddSegment|20_3(p0, p, ref CS$<>8__locals1);
					stepLength -= currentLength;
					Line2DGenerator.<ApplyDashed>g__SwitchCurrentSegmentSpacing|20_2(ref CS$<>8__locals1);
					p0 = p;
				}
				p0 = p;
				p = next;
				Line2DGenerator.<ApplyDashed>g__AddSegment|20_3(p0, p, ref CS$<>8__locals1);
				CS$<>8__locals1.currentSegmentLength += stepLength;
			}
		}
		return CS$<>8__locals1.ret;
	}

	// Token: 0x06000436 RID: 1078 RVA: 0x0001A74C File Offset: 0x0001894C
	private static List<Line2DGenerator.VertexSite> GenerateVertexSites(IReadOnlyList<Vector2> vertices, float width)
	{
		List<Line2DGenerator.VertexSite> sites = new List<Line2DGenerator.VertexSite>();
		bool flag = width <= 0f || vertices == null || vertices.Count < 2;
		List<Line2DGenerator.VertexSite> result;
		if (flag)
		{
			result = sites;
		}
		else
		{
			float halfWidth = width * 0.5f;
			for (int i = 0; i < vertices.Count; i++)
			{
				Vector2 p0 = vertices[i];
				bool flag2 = i == vertices.Count - 1;
				Vector2 normal;
				Vector2 direction;
				if (flag2)
				{
					direction = (normal = Line2DGenerator.Arrow(vertices[i - 1], vertices[i]).normalized);
				}
				else
				{
					Vector2 p = vertices[i + 1];
					direction = Line2DGenerator.Arrow(p0, p).normalized;
					bool flag3 = i == 0;
					if (flag3)
					{
						normal = direction;
					}
					else
					{
						Vector2 pb = vertices[i - 1];
						Vector2 a = Line2DGenerator.Arrow(p0, pb).normalized;
						Vector2 a2 = Line2DGenerator.Arrow(p0, p).normalized;
						Vector2 ah = (a + a2) / 2f;
						normal = Vector2.Perpendicular(ah).normalized;
						bool flag4 = Vector2.SignedAngle(a, a2) < 0f;
						if (flag4)
						{
							normal *= -1f;
						}
					}
				}
				bool flag5 = Mathf.Approximately(0f, normal.sqrMagnitude) && i > 0;
				if (flag5)
				{
					normal = direction;
				}
				bool returning = Mathf.Abs(Vector2.SignedAngle(direction, normal)) > 90f;
				bool flag6 = returning;
				if (flag6)
				{
					normal *= -1f;
				}
				Vector2 normalToSideA = Vector2.Perpendicular(normal) * (float)(returning ? -1 : 1);
				Vector2 normalToSideB = -normalToSideA;
				float realHalfWidth = Line2DGenerator.RealHalfWidth(direction, normal, halfWidth);
				sites.Add(new Line2DGenerator.VertexSite
				{
					Origin = new Line2DGenerator.Vertex
					{
						Position = p0
					},
					SideA = new Line2DGenerator.Vertex
					{
						Position = p0 + normalToSideA * Mathf.Min(realHalfWidth, halfWidth * 1.732f)
					},
					SideB = new Line2DGenerator.Vertex
					{
						Position = p0 + normalToSideB * realHalfWidth
					},
					Normal = normal,
					Direction = direction
				});
			}
			result = sites;
		}
		return result;
	}

	// Token: 0x06000437 RID: 1079 RVA: 0x0001A9C8 File Offset: 0x00018BC8
	private static void GenerateMesh(IReadOnlyList<Line2DGenerator.VertexSite> sites, VertexHelper vh, bool fullMesh, bool uvDirection, bool uvDashed, Line2DGenerator.ColoredStyle colored)
	{
		float totalLength = delegate
		{
			float sum = 0f;
			int j = 1;
			int len = sites.Count;
			while (j < len)
			{
				sum += Vector2.Distance(sites[j].Origin.Position, sites[j - 1].Origin.Position);
				j++;
			}
			return sum;
		}();
		Line2DGenerator.GenerateSitesCache(sites);
		float currentLength = 0f;
		float currentDashProgress = 0f;
		float currentDashTotal = 0f;
		int i = 0;
		while (i < sites.Count)
		{
			Line2DGenerator.VertexSite curr = sites[i];
			bool flag = i > 0;
			if (flag)
			{
				float lengthDelta = Vector2.Distance(curr.Origin.Position, sites[i - 1].Origin.Position);
				currentDashProgress += lengthDelta;
				currentLength += lengthDelta;
			}
			int vertIndex = vh.currentVertCount;
			float progress = currentLength / totalLength;
			Color color = Color.Lerp(colored.StartColor, colored.EndColor, progress);
			float uvValue = uvDashed ? (currentDashProgress / currentDashTotal) : progress;
			vh.AddVert(curr.SideA.Position, color, uvDirection ? new Vector2(uvValue, 0f) : new Vector2(0f, uvValue), Vector4.one, curr.Normal, Vector2.zero);
			if (fullMesh)
			{
				vh.AddVert(curr.Origin.Position, color, uvDirection ? new Vector2(uvValue, 0.5f) : new Vector2(0.5f, uvValue), Vector4.one, curr.Normal, Vector2.zero);
				vh.AddVert(curr.SideB.Position, color, uvDirection ? new Vector2(uvValue, 1f) : new Vector2(1f, uvValue), Vector4.one, curr.Normal, Vector2.zero);
				bool flag2 = i <= 0 || curr.Break;
				if (flag2)
				{
					currentDashProgress = 0f;
					Line2DGenerator.VertexSite nextStart = Line2DGenerator.FindNextSite(Line2DGenerator.CachedNotBreakSites, curr.Index);
					Line2DGenerator.VertexSite nextBreak = Line2DGenerator.FindNextSite(Line2DGenerator.CachedBreakSites, nextStart.Index);
					currentDashTotal = Math.Clamp(Vector2.Distance(nextStart.Origin.Position, nextBreak.Origin.Position), 0.1f, Math.Max(totalLength, 0.1f));
				}
				else
				{
					vh.AddTriangle(vertIndex - 3, vertIndex, vertIndex - 2);
					vh.AddTriangle(vertIndex, vertIndex - 2, vertIndex + 1);
					vh.AddTriangle(vertIndex - 2, vertIndex + 1, vertIndex - 1);
					vh.AddTriangle(vertIndex + 1, vertIndex - 1, vertIndex + 2);
				}
			}
			else
			{
				vh.AddVert(curr.SideB.Position, color, uvDirection ? new Vector2(uvValue, 1f) : new Vector2(1f, uvValue), Vector4.one, curr.Normal, Vector2.zero);
				bool flag3 = i <= 0 || curr.Break;
				if (flag3)
				{
					currentDashProgress = 0f;
					Line2DGenerator.VertexSite nextStart2 = Line2DGenerator.FindNextSite(Line2DGenerator.CachedNotBreakSites, curr.Index);
					Line2DGenerator.VertexSite nextBreak2 = Line2DGenerator.FindNextSite(Line2DGenerator.CachedBreakSites, nextStart2.Index);
					currentDashTotal = Math.Clamp(Vector2.Distance(nextStart2.Origin.Position, nextBreak2.Origin.Position), 0.1f, Math.Max(totalLength, 0.1f));
				}
				else
				{
					vh.AddTriangle(vertIndex - 2, vertIndex, vertIndex - 1);
					vh.AddTriangle(vertIndex, vertIndex - 1, vertIndex + 1);
				}
			}
			IL_3AD:
			i++;
			continue;
			goto IL_3AD;
		}
	}

	// Token: 0x0600043A RID: 1082 RVA: 0x0001AE1C File Offset: 0x0001901C
	[CompilerGenerated]
	internal static float <ApplyDashed>g__GetCurrentSegmentRemainLength|20_0(ref Line2DGenerator.<>c__DisplayClass20_0 A_0)
	{
		return Mathf.Max(Line2DGenerator.<ApplyDashed>g__GetCurrentSegmentMaxLength|20_1(ref A_0) - A_0.currentSegmentLength, 0f);
	}

	// Token: 0x0600043B RID: 1083 RVA: 0x0001AE35 File Offset: 0x00019035
	[CompilerGenerated]
	internal static float <ApplyDashed>g__GetCurrentSegmentMaxLength|20_1(ref Line2DGenerator.<>c__DisplayClass20_0 A_0)
	{
		return A_0.currentSegmentSpacing ? A_0.dashed.SpaceLength : A_0.dashed.SolidLength;
	}

	// Token: 0x0600043C RID: 1084 RVA: 0x0001AE57 File Offset: 0x00019057
	[CompilerGenerated]
	internal static void <ApplyDashed>g__SwitchCurrentSegmentSpacing|20_2(ref Line2DGenerator.<>c__DisplayClass20_0 A_0)
	{
		A_0.currentSegmentLength = 0f;
		A_0.currentSegmentSpacing = !A_0.currentSegmentSpacing;
	}

	// Token: 0x0600043D RID: 1085 RVA: 0x0001AE74 File Offset: 0x00019074
	[CompilerGenerated]
	internal static float <ApplyDashed>g__AddSegment|20_3(Line2DGenerator.VertexSite p0, Line2DGenerator.VertexSite p1, ref Line2DGenerator.<>c__DisplayClass20_0 A_2)
	{
		p0.Break = A_2.currentSegmentSpacing;
		p1.Break = A_2.currentSegmentSpacing;
		A_2.ret.Add(p0);
		A_2.ret.Add(p1);
		return Vector2.Distance(p0.Origin.Position, p1.Origin.Position);
	}

	// Token: 0x04000297 RID: 663
	public Vector2[] Vertices;

	// Token: 0x04000298 RID: 664
	public IReadOnlyList<Vector2> OverrideVertices;

	// Token: 0x04000299 RID: 665
	[Range(0f, 16384f)]
	public float Width;

	// Token: 0x0400029A RID: 666
	public bool FullMesh;

	// Token: 0x0400029B RID: 667
	public bool UVDirection;

	// Token: 0x0400029C RID: 668
	public bool UVDashed;

	// Token: 0x0400029D RID: 669
	public Line2DGenerator.ColoredStyle Colored = new Line2DGenerator.ColoredStyle
	{
		StartColor = Color.white,
		EndColor = Color.white
	};

	// Token: 0x0400029E RID: 670
	public Line2DGenerator.RoundedCornersStyle RoundedCorners = new Line2DGenerator.RoundedCornersStyle
	{
		Enabled = false,
		Quality = 4,
		Radius = 4f
	};

	// Token: 0x0400029F RID: 671
	public Line2DGenerator.DashedStyle Dashed;

	// Token: 0x040002A0 RID: 672
	private static readonly List<Line2DGenerator.VertexSite> CachedBreakSites = new List<Line2DGenerator.VertexSite>();

	// Token: 0x040002A1 RID: 673
	private static readonly List<Line2DGenerator.VertexSite> CachedNotBreakSites = new List<Line2DGenerator.VertexSite>();

	// Token: 0x020010E7 RID: 4327
	private struct Vertex
	{
		// Token: 0x040094B7 RID: 38071
		public Vector2 Position;
	}

	// Token: 0x020010E8 RID: 4328
	private struct VertexSite
	{
		// Token: 0x0600C0ED RID: 49389 RVA: 0x0056C428 File Offset: 0x0056A628
		public void ReplaceOrigin(Vector2 newOrigin)
		{
			this.SideA.Position = newOrigin + (this.SideA.Position - this.Origin.Position);
			this.SideB.Position = newOrigin + (this.SideB.Position - this.Origin.Position);
			this.Origin.Position = newOrigin;
		}

		// Token: 0x040094B8 RID: 38072
		public Line2DGenerator.Vertex Origin;

		// Token: 0x040094B9 RID: 38073
		public Line2DGenerator.Vertex SideA;

		// Token: 0x040094BA RID: 38074
		public Line2DGenerator.Vertex SideB;

		// Token: 0x040094BB RID: 38075
		public Vector2 Normal;

		// Token: 0x040094BC RID: 38076
		public Vector2 Direction;

		// Token: 0x040094BD RID: 38077
		public bool Break;

		// Token: 0x040094BE RID: 38078
		public int Index;
	}

	// Token: 0x020010E9 RID: 4329
	[Serializable]
	public struct ColoredStyle
	{
		// Token: 0x040094BF RID: 38079
		public Color StartColor;

		// Token: 0x040094C0 RID: 38080
		public Color EndColor;
	}

	// Token: 0x020010EA RID: 4330
	[Serializable]
	public struct RoundedCornersStyle
	{
		// Token: 0x040094C1 RID: 38081
		public bool Enabled;

		// Token: 0x040094C2 RID: 38082
		[Range(0f, 128f)]
		public int Quality;

		// Token: 0x040094C3 RID: 38083
		[Range(0.1f, 16384f)]
		public float Radius;
	}

	// Token: 0x020010EB RID: 4331
	[Serializable]
	public struct DashedStyle
	{
		// Token: 0x040094C4 RID: 38084
		public bool Enabled;

		// Token: 0x040094C5 RID: 38085
		[Range(0f, 3.4028235E+38f)]
		public float SolidLength;

		// Token: 0x040094C6 RID: 38086
		[Range(0f, 3.4028235E+38f)]
		public float SpaceLength;

		// Token: 0x040094C7 RID: 38087
		public float Offset;
	}
}
