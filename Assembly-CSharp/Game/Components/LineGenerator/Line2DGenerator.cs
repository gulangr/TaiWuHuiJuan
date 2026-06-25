using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.LineGenerator
{
	// Token: 0x02000EEE RID: 3822
	public class Line2DGenerator : BaseMeshEffect
	{
		// Token: 0x170013D3 RID: 5075
		// (get) Token: 0x0600AF53 RID: 44883 RVA: 0x004FDE2D File Offset: 0x004FC02D
		// (set) Token: 0x0600AF54 RID: 44884 RVA: 0x004FDE38 File Offset: 0x004FC038
		public IEnumerable<Vector3> Vertices
		{
			get
			{
				return this.points;
			}
			set
			{
				bool flag = (this.points = value.ToArray<Vector3>()).Length < 2;
				if (flag)
				{
					this.vertices = Array.Empty<Vector3>();
					this._sc = null;
					this.cimage.SetVerticesDirty();
				}
				else
				{
					this._sc = new SmoothCurve(this.points);
					this.vertices = this._sc.GetVertices(this.minImputeLength, this.angleThreshold);
					this.cimage.SetVerticesDirty();
				}
			}
		}

		// Token: 0x0600AF55 RID: 44885 RVA: 0x004FDEBC File Offset: 0x004FC0BC
		public void SetWidth(float newWidth)
		{
			bool flag = Mathf.Approximately(this.width, newWidth);
			if (!flag)
			{
				this.width = newWidth;
				this.cimage.SetVerticesDirty();
			}
		}

		// Token: 0x0600AF56 RID: 44886 RVA: 0x004FDEF0 File Offset: 0x004FC0F0
		public override void ModifyMesh(VertexHelper vh)
		{
			bool flag = !base.enabled;
			if (!flag)
			{
				vh.Clear();
				this._sitesCache.Clear();
				int num;
				this._sitesCache.AddRange(this.<ModifyMesh>g__Rendering|24_0(this.vertices, out num));
				for (int i = 0; i < this._sitesCache.Count; i++)
				{
					Line2DGenerator.VertexSite site = this._sitesCache[i];
					site.Index = i;
					this._sitesCache[i] = site;
				}
				Line2DGenerator.GenerateMesh(this._sitesCache, vh, this.fullMesh, this.uVDirection, this.dashed.Enabled && this.uVDashed, this.color, this.colorDisabled, this.uVUseFixedLengthForY, this.uVLineYScale);
			}
		}

		// Token: 0x0600AF57 RID: 44887 RVA: 0x004FDFC6 File Offset: 0x004FC1C6
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector3 Arrow(Vector3 from, Vector3 to)
		{
			return to - from;
		}

		// Token: 0x0600AF58 RID: 44888 RVA: 0x004FDFD0 File Offset: 0x004FC1D0
		private static float RealHalfWidth(Vector3 direction, Vector3 normal, float halfWidth)
		{
			float angle = Vector3.Angle(direction, normal) * 0.017453292f;
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

		// Token: 0x0600AF59 RID: 44889 RVA: 0x004FE028 File Offset: 0x004FC228
		private List<Line2DGenerator.VertexSite> ApplyDashed(IReadOnlyList<Line2DGenerator.VertexSite> sites, Line2DGenerator.DashedStyle dashedStyle, float lineWidth)
		{
			Line2DGenerator.<>c__DisplayClass30_0 CS$<>8__locals1;
			CS$<>8__locals1.dashedStyle = dashedStyle;
			CS$<>8__locals1.<>4__this = this;
			float totalLength = Line2DGenerator.Distance(sites, 0, sites.Count);
			float currLength = 0f;
			CS$<>8__locals1.progress = 0f;
			this._cachedApplyDashedRet.Clear();
			float halfWidth = lineWidth * 0.5f;
			CS$<>8__locals1.currentSegmentLength = 0f;
			CS$<>8__locals1.currentSegmentSpacing = false;
			float remainOffset = CS$<>8__locals1.dashedStyle.Offset;
			while (!Mathf.Approximately(remainOffset, 0f))
			{
				float offsetAbs = Mathf.Abs(remainOffset);
				float offsetSign = Mathf.Sign(remainOffset);
				float remain = this.<ApplyDashed>g__GetCurrentSegmentRemainLength|30_0(ref CS$<>8__locals1);
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
						this.<ApplyDashed>g__SwitchCurrentSegmentSpacing|30_2(ref CS$<>8__locals1);
						CS$<>8__locals1.currentSegmentLength += this.<ApplyDashed>g__GetCurrentSegmentRemainLength|30_0(ref CS$<>8__locals1) - offsetAbs;
					}
					break;
				}
				this.<ApplyDashed>g__SwitchCurrentSegmentSpacing|30_2(ref CS$<>8__locals1);
				remainOffset -= remain * offsetSign;
			}
			for (int i = 0; i < sites.Count - 1; i++)
			{
				Line2DGenerator.VertexSite curr = sites[i];
				Line2DGenerator.VertexSite next = sites[i + 1];
				float stepLength = Vector3.Distance(curr.Origin.Position, next.Origin.Position);
				CS$<>8__locals1.progress = (currLength + stepLength / 2f) / totalLength;
				currLength += stepLength;
				bool flag3 = stepLength <= this.<ApplyDashed>g__GetCurrentSegmentRemainLength|30_0(ref CS$<>8__locals1);
				if (flag3)
				{
					CS$<>8__locals1.currentSegmentLength += this.<ApplyDashed>g__AddSegment|30_3(curr, next, ref CS$<>8__locals1);
				}
				else
				{
					Line2DGenerator.VertexSite p0 = curr;
					Line2DGenerator.VertexSite p = p0;
					while (stepLength > this.<ApplyDashed>g__GetCurrentSegmentRemainLength|30_0(ref CS$<>8__locals1))
					{
						p.ReplaceOrigin(p0.Origin.Position + Line2DGenerator.Arrow(p0.Origin.Position, next.Origin.Position).normalized * this.<ApplyDashed>g__GetCurrentSegmentRemainLength|30_0(ref CS$<>8__locals1));
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
						this.<ApplyDashed>g__AddSegment|30_3(p0, p, ref CS$<>8__locals1);
						stepLength -= currentLength;
						this.<ApplyDashed>g__SwitchCurrentSegmentSpacing|30_2(ref CS$<>8__locals1);
						p0 = p;
					}
					p0 = p;
					p = next;
					this.<ApplyDashed>g__AddSegment|30_3(p0, p, ref CS$<>8__locals1);
					CS$<>8__locals1.currentSegmentLength += stepLength;
				}
			}
			return this._cachedApplyDashedRet;
		}

		// Token: 0x0600AF5A RID: 44890 RVA: 0x004FE370 File Offset: 0x004FC570
		private static List<Line2DGenerator.VertexSite> GenerateVertexSites(IReadOnlyList<Vector3> vertices, float width)
		{
			Line2DGenerator._cachedGenerateVertexSites.Clear();
			bool flag = width <= 0f || vertices == null || vertices.Count < 2;
			List<Line2DGenerator.VertexSite> cachedGenerateVertexSites;
			if (flag)
			{
				cachedGenerateVertexSites = Line2DGenerator._cachedGenerateVertexSites;
			}
			else
			{
				float halfWidth = width * 0.5f;
				for (int i = 0; i < vertices.Count; i++)
				{
					Vector3 p0 = vertices[i];
					bool flag2 = i == vertices.Count - 1;
					Vector3 normal;
					Vector3 direction;
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
							Vector3 a = Line2DGenerator.Arrow(p0, pb).normalized;
							Vector3 a2 = Line2DGenerator.Arrow(p0, p).normalized;
							Vector3 ah = (a + a2) / 2f;
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
					Line2DGenerator._cachedGenerateVertexSites.Add(new Line2DGenerator.VertexSite
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
				cachedGenerateVertexSites = Line2DGenerator._cachedGenerateVertexSites;
			}
			return cachedGenerateVertexSites;
		}

		// Token: 0x0600AF5B RID: 44891 RVA: 0x004FE674 File Offset: 0x004FC874
		private static float Distance(IReadOnlyList<Line2DGenerator.VertexSite> sites, int start, int end)
		{
			float sum = 0f;
			for (int i = start + 1; i < end; i++)
			{
				sum += (sites[i].Origin.Position - sites[i - 1].Origin.Position).magnitude;
			}
			return sum;
		}

		// Token: 0x0600AF5C RID: 44892 RVA: 0x004FE6DC File Offset: 0x004FC8DC
		private static void GenerateMesh(IReadOnlyList<Line2DGenerator.VertexSite> sites, VertexHelper vh, bool fullMesh, bool uvDirection, bool uvDashed, Line2DGenerator.ColoredStyle color, Line2DGenerator.ColoredStyle colorDisabled, bool uVUseFixedLengthForY = false, float uvYScale = 1f)
		{
			float totalLength = Line2DGenerator.Distance(sites, 0, sites.Count);
			float currentLength = 0f;
			float currentDashProgress = 0f;
			float currentDashTotal = 0f;
			int i = 0;
			while (i < sites.Count)
			{
				Line2DGenerator.VertexSite curr = sites[i];
				Line2DGenerator.VertexSite prev = sites[Math.Max(i - 1, 0)];
				int vertIndex = vh.currentVertCount;
				float progress = (currentLength += ((curr.SideA.Position + curr.SideB.Position) / 2f - (prev.SideA.Position + prev.SideB.Position) / 2f).magnitude) * uvYScale;
				float normalizedProgress = currentLength / totalLength;
				Color usingColor = (curr.SideA.Position.z + curr.SideB.Position.z < 0f) ? Color.Lerp(colorDisabled.StartColor, colorDisabled.EndColor, normalizedProgress) : Color.Lerp(color.StartColor, color.EndColor, normalizedProgress);
				float uvValue = uvDashed ? (currentDashProgress / currentDashTotal) : (uVUseFixedLengthForY ? progress : normalizedProgress);
				vh.AddVert(curr.SideA.Position, usingColor, uvDirection ? new Vector3(uvValue, 0f) : new Vector3(0f, uvValue), Vector4.one, curr.Normal, Vector3.zero);
				if (fullMesh)
				{
					vh.AddVert(curr.Origin.Position, usingColor, uvDirection ? new Vector3(uvValue, 0.5f) : new Vector3(0.5f, uvValue), Vector4.one, curr.Normal, Vector3.zero);
					vh.AddVert(curr.SideB.Position, usingColor, uvDirection ? new Vector3(uvValue, 1f) : new Vector3(1f, uvValue), Vector4.one, curr.Normal, Vector3.zero);
					bool flag = i <= 0 || curr.Break;
					if (flag)
					{
						currentDashProgress = 0f;
						Line2DGenerator.VertexSite nextStart = sites.FirstOrDefault((Line2DGenerator.VertexSite site) => !site.Break && site.Index > curr.Index);
						Line2DGenerator.VertexSite nextBreak = sites.FirstOrDefault((Line2DGenerator.VertexSite site) => site.Break && site.Index > nextStart.Index);
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
					vh.AddVert(curr.SideB.Position, usingColor, uvDirection ? new Vector3(uvValue, 1f) : new Vector3(1f, uvValue), Vector4.one, curr.Normal, Vector3.zero);
					bool flag2 = i <= 0 || curr.Break;
					if (flag2)
					{
						currentDashProgress = 0f;
						Line2DGenerator.VertexSite nextStart = sites.FirstOrDefault((Line2DGenerator.VertexSite site) => !site.Break && site.Index > curr.Index);
						Line2DGenerator.VertexSite nextBreak2 = sites.FirstOrDefault((Line2DGenerator.VertexSite site) => site.Break && site.Index > nextStart.Index);
						currentDashTotal = Math.Clamp(Vector2.Distance(nextStart.Origin.Position, nextBreak2.Origin.Position), 0.1f, Math.Max(totalLength, 0.1f));
					}
					else
					{
						vh.AddTriangle(vertIndex - 2, vertIndex, vertIndex - 1);
						vh.AddTriangle(vertIndex, vertIndex - 1, vertIndex + 1);
					}
				}
				IL_49F:
				i++;
				continue;
				goto IL_49F;
			}
		}

		// Token: 0x0600AF5F RID: 44895 RVA: 0x004FECAC File Offset: 0x004FCEAC
		[CompilerGenerated]
		private IEnumerable<Line2DGenerator.VertexSite> <ModifyMesh>g__Rendering|24_0(Vector3[] verticesToProcess, out int length)
		{
			bool flag = verticesToProcess.Length != 0;
			IEnumerable<Line2DGenerator.VertexSite> result;
			if (flag)
			{
				this._calcCache.Clear();
				this._calcCache.AddRange(verticesToProcess);
				List<Line2DGenerator.VertexSite> generatedSites = Line2DGenerator.GenerateVertexSites(this._calcCache, this.width);
				bool enabled = this.dashed.Enabled;
				if (enabled)
				{
					generatedSites = this.ApplyDashed(generatedSites, this.dashed, this.width);
				}
				length = generatedSites.Count;
				result = generatedSites;
			}
			else
			{
				length = 0;
				result = Enumerable.Empty<Line2DGenerator.VertexSite>();
			}
			return result;
		}

		// Token: 0x0600AF60 RID: 44896 RVA: 0x004FED2A File Offset: 0x004FCF2A
		[CompilerGenerated]
		private float <ApplyDashed>g__GetCurrentSegmentRemainLength|30_0(ref Line2DGenerator.<>c__DisplayClass30_0 A_1)
		{
			return Mathf.Max(this.<ApplyDashed>g__GetCurrentSegmentMaxLength|30_1(ref A_1) - A_1.currentSegmentLength, 0f);
		}

		// Token: 0x0600AF61 RID: 44897 RVA: 0x004FED44 File Offset: 0x004FCF44
		[CompilerGenerated]
		private float <ApplyDashed>g__GetCurrentSegmentMaxLength|30_1(ref Line2DGenerator.<>c__DisplayClass30_0 A_1)
		{
			return A_1.currentSegmentSpacing ? (A_1.dashedStyle.SpaceLength * this.SpaceLengthModifier(A_1.progress)) : (A_1.dashedStyle.SolidLength * this.SolidLengthModifier(A_1.progress));
		}

		// Token: 0x0600AF62 RID: 44898 RVA: 0x004FED95 File Offset: 0x004FCF95
		[CompilerGenerated]
		private void <ApplyDashed>g__SwitchCurrentSegmentSpacing|30_2(ref Line2DGenerator.<>c__DisplayClass30_0 A_1)
		{
			A_1.currentSegmentLength = 0f;
			A_1.currentSegmentSpacing = !A_1.currentSegmentSpacing;
		}

		// Token: 0x0600AF63 RID: 44899 RVA: 0x004FEDB4 File Offset: 0x004FCFB4
		[CompilerGenerated]
		private float <ApplyDashed>g__AddSegment|30_3(Line2DGenerator.VertexSite p0, Line2DGenerator.VertexSite p1, ref Line2DGenerator.<>c__DisplayClass30_0 A_3)
		{
			p0.Break = A_3.currentSegmentSpacing;
			p1.Break = A_3.currentSegmentSpacing;
			this._cachedApplyDashedRet.Add(p0);
			this._cachedApplyDashedRet.Add(p1);
			return Vector2.Distance(p0.Origin.Position, p1.Origin.Position);
		}

		// Token: 0x040087EB RID: 34795
		[SerializeField]
		public Vector3[] points;

		// Token: 0x040087EC RID: 34796
		[SerializeField]
		public Vector3[] vertices;

		// Token: 0x040087ED RID: 34797
		[SerializeField]
		private float minImputeLength = 2f;

		// Token: 0x040087EE RID: 34798
		[SerializeField]
		private float angleThreshold = 0.005f;

		// Token: 0x040087EF RID: 34799
		public Func<float, float> SpaceLengthModifier = (float _) => 1f;

		// Token: 0x040087F0 RID: 34800
		public Func<float, float> SolidLengthModifier = (float _) => 1f;

		// Token: 0x040087F1 RID: 34801
		private SmoothCurve _sc;

		// Token: 0x040087F2 RID: 34802
		[SerializeField]
		[Range(0f, 16f)]
		private float width;

		// Token: 0x040087F3 RID: 34803
		public bool fullMesh;

		// Token: 0x040087F4 RID: 34804
		public bool uVDirection;

		// Token: 0x040087F5 RID: 34805
		public bool uVDashed;

		// Token: 0x040087F6 RID: 34806
		public Line2DGenerator.ColoredStyle color = new Line2DGenerator.ColoredStyle
		{
			StartColor = Color.yellow,
			EndColor = Color.yellow
		};

		// Token: 0x040087F7 RID: 34807
		public Line2DGenerator.ColoredStyle colorDisabled = new Line2DGenerator.ColoredStyle
		{
			StartColor = Color.blue,
			EndColor = Color.blue
		};

		// Token: 0x040087F8 RID: 34808
		public Line2DGenerator.DashedStyle dashed;

		// Token: 0x040087F9 RID: 34809
		[SerializeField]
		private CRawImage cimage;

		// Token: 0x040087FA RID: 34810
		[Tooltip("mesh移动速度")]
		[Range(0f, 16f)]
		public float moveSpeed = 4f;

		// Token: 0x040087FB RID: 34811
		[Tooltip("使用真实长度作为uv的y值，否则使用比例（原逻辑）")]
		public bool uVUseFixedLengthForY;

		// Token: 0x040087FC RID: 34812
		[Tooltip("uv的y值缩放")]
		public float uVLineYScale = 1f;

		// Token: 0x040087FD RID: 34813
		private List<Line2DGenerator.VertexSite> _sitesCache = new List<Line2DGenerator.VertexSite>();

		// Token: 0x040087FE RID: 34814
		private List<Vector3> _calcCache = new List<Vector3>();

		// Token: 0x040087FF RID: 34815
		private List<Line2DGenerator.VertexSite> _cachedApplyDashedRet = new List<Line2DGenerator.VertexSite>();

		// Token: 0x04008800 RID: 34816
		private static readonly List<Line2DGenerator.VertexSite> _cachedGenerateVertexSites = new List<Line2DGenerator.VertexSite>();

		// Token: 0x02002531 RID: 9521
		private struct Vertex
		{
			// Token: 0x0400E74B RID: 59211
			public Vector3 Position;
		}

		// Token: 0x02002532 RID: 9522
		private struct VertexSite
		{
			// Token: 0x06010B27 RID: 68391 RVA: 0x0066B7A0 File Offset: 0x006699A0
			public void ReplaceOrigin(Vector3 newOrigin)
			{
				this.SideA.Position = newOrigin + (this.SideA.Position - this.Origin.Position);
				this.SideB.Position = newOrigin + (this.SideB.Position - this.Origin.Position);
				this.Origin.Position = newOrigin;
			}

			// Token: 0x0400E74C RID: 59212
			public Line2DGenerator.Vertex Origin;

			// Token: 0x0400E74D RID: 59213
			public Line2DGenerator.Vertex SideA;

			// Token: 0x0400E74E RID: 59214
			public Line2DGenerator.Vertex SideB;

			// Token: 0x0400E74F RID: 59215
			public Vector3 Normal;

			// Token: 0x0400E750 RID: 59216
			public Vector3 Direction;

			// Token: 0x0400E751 RID: 59217
			public bool Break;

			// Token: 0x0400E752 RID: 59218
			public int Index;
		}

		// Token: 0x02002533 RID: 9523
		[Serializable]
		public struct ColoredStyle
		{
			// Token: 0x0400E753 RID: 59219
			public Color StartColor;

			// Token: 0x0400E754 RID: 59220
			public Color EndColor;
		}

		// Token: 0x02002534 RID: 9524
		[Serializable]
		public struct RoundedCornersStyle
		{
			// Token: 0x0400E755 RID: 59221
			public bool Enabled;

			// Token: 0x0400E756 RID: 59222
			[Range(0f, 128f)]
			public int Quality;

			// Token: 0x0400E757 RID: 59223
			[Range(0.1f, 16384f)]
			public float Radius;
		}

		// Token: 0x02002535 RID: 9525
		[Serializable]
		public struct DashedStyle
		{
			// Token: 0x0400E758 RID: 59224
			public bool Enabled;

			// Token: 0x0400E759 RID: 59225
			[Range(0f, 3.4028235E+38f)]
			public float SolidLength;

			// Token: 0x0400E75A RID: 59226
			[Range(0f, 3.4028235E+38f)]
			public float SpaceLength;

			// Token: 0x0400E75B RID: 59227
			public float Offset;
		}
	}
}
