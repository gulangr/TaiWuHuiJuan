using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace FrameWork.UISystem
{
	// Token: 0x02000FF5 RID: 4085
	public static class PolygonGenerator
	{
		// Token: 0x0600BA4F RID: 47695 RVA: 0x0054D778 File Offset: 0x0054B978
		public static Vector2[] GeneratePolygon(Sprite referenceSprite, RectTransform refefenceRectTransform = null, float simplificationTolerance = 2f, float alphaThreshold = 0.1f)
		{
			bool flag = referenceSprite == null;
			Vector2[] result;
			if (flag)
			{
				Debug.LogError("SpriteRenderer or its Sprite is not set.");
				result = null;
			}
			else
			{
				Rect rect = referenceSprite.rect;
				Texture2D texture = referenceSprite.texture;
				Rect actualRect = referenceSprite.packed ? referenceSprite.textureRect : rect;
				int startX = (int)actualRect.x;
				int startY = (int)actualRect.y;
				int width = (int)actualRect.width;
				int height = (int)actualRect.height;
				Color[] pixels = PolygonGenerator.ExtractPixelsFromTexture(texture, startX, startY, width, height);
				int lowAlphaCount;
				int highAlphaCount;
				bool[,] alphaMap;
				PolygonGenerator.CreateAlphaMap(pixels, width, height, alphaThreshold, out lowAlphaCount, out highAlphaCount, out alphaMap);
				Debug.Log(string.Format("[PolygonGenerator] Sprite: {0}, Packed: {1}, ", referenceSprite.name, referenceSprite.packed) + string.Format("rect: {0}, textureRect: {1}", rect, referenceSprite.textureRect));
				Debug.Log(string.Format("[PolygonGenerator] 图片尺寸: {0}x{1}, 透明像素: {2}, 可见像素: {3}", new object[]
				{
					width,
					height,
					lowAlphaCount,
					highAlphaCount
				}));
				float cornerBL = pixels[0].a;
				float cornerBR = pixels[width - 1].a;
				float cornerTL = pixels[(height - 1) * width].a;
				float cornerTR = pixels[(height - 1) * width + width - 1].a;
				float center = pixels[height / 2 * width + width / 2].a;
				Debug.Log(string.Format("[PolygonGenerator] Alpha采样 - BL:{0:F3}, BR:{1:F3}, TL:{2:F3}, TR:{3:F3}, Center:{4:F3}", new object[]
				{
					cornerBL,
					cornerBR,
					cornerTL,
					cornerTR,
					center
				}));
				List<Vector2> vertices = PolygonGenerator.GeneratePolygonFromAlphaMap(alphaMap, width, height);
				bool flag2 = vertices == null || vertices.Count < 3;
				if (flag2)
				{
					Debug.LogError("Failed to generate polygon from alpha map.");
					result = null;
				}
				else
				{
					vertices = PolygonGenerator.SimplifyPolygon(vertices, simplificationTolerance);
					PolygonGenerator.TransformVerticesToRectTransform(vertices, refefenceRectTransform, width, height);
					result = vertices.ToArray();
				}
			}
			return result;
		}

		// Token: 0x0600BA50 RID: 47696 RVA: 0x0054D99C File Offset: 0x0054BB9C
		public static Vector2[] GeneratePolygon(Texture2D referenceTexture, RectTransform referenceRectTransform = null, float simplificationTolerance = 2f, float alphaThreshold = 0.1f)
		{
			bool flag = referenceTexture == null;
			Vector2[] result;
			if (flag)
			{
				Debug.LogError("Texture is not set.");
				result = null;
			}
			else
			{
				int width = referenceTexture.width;
				int height = referenceTexture.height;
				Color[] pixels = PolygonGenerator.ExtractPixelsFromTexture(referenceTexture, 0, 0, width, height);
				int lowAlphaCount;
				int highAlphaCount;
				bool[,] alphaMap;
				PolygonGenerator.CreateAlphaMap(pixels, width, height, alphaThreshold, out lowAlphaCount, out highAlphaCount, out alphaMap);
				Debug.Log(string.Format("[PolygonGenerator] Texture: {0}, 尺寸: {1}x{2}, 透明像素: {3}, 可见像素: {4}", new object[]
				{
					referenceTexture.name,
					width,
					height,
					lowAlphaCount,
					highAlphaCount
				}));
				List<Vector2> vertices = PolygonGenerator.GeneratePolygonFromAlphaMap(alphaMap, width, height);
				bool flag2 = vertices == null || vertices.Count < 3;
				if (flag2)
				{
					Debug.LogError("Failed to generate polygon from alpha map.");
					result = null;
				}
				else
				{
					vertices = PolygonGenerator.SimplifyPolygon(vertices, simplificationTolerance);
					PolygonGenerator.TransformVerticesToRectTransform(vertices, referenceRectTransform, width, height);
					result = vertices.ToArray();
				}
			}
			return result;
		}

		// Token: 0x0600BA51 RID: 47697 RVA: 0x0054DA90 File Offset: 0x0054BC90
		private static Color[] ExtractPixelsFromTexture(Texture2D texture, int startX, int startY, int width, int height)
		{
			RenderTexture rt = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.ARGB32);
			Graphics.Blit(texture, rt);
			RenderTexture.active = rt;
			Texture2D readableTex = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
			readableTex.ReadPixels(new Rect(0f, 0f, (float)texture.width, (float)texture.height), 0, 0);
			readableTex.Apply();
			RenderTexture.active = null;
			RenderTexture.ReleaseTemporary(rt);
			Color[] pixels = readableTex.GetPixels(startX, startY, width, height);
			bool isEditor = Application.isEditor;
			if (isEditor)
			{
				Object.DestroyImmediate(readableTex);
			}
			else
			{
				Object.Destroy(readableTex);
			}
			return pixels;
		}

		// Token: 0x0600BA52 RID: 47698 RVA: 0x0054DB3C File Offset: 0x0054BD3C
		private static void CreateAlphaMap(Color[] pixels, int width, int height, float alphaThreshold, out int lowAlphaCount, out int highAlphaCount, out bool[,] alphaMap)
		{
			alphaMap = new bool[width, height];
			lowAlphaCount = 0;
			highAlphaCount = 0;
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					float alpha = pixels[x + y * width].a;
					bool flag = alpha < alphaThreshold;
					if (flag)
					{
						lowAlphaCount++;
					}
					else
					{
						highAlphaCount++;
					}
					bool flag2 = alpha >= alphaThreshold;
					if (flag2)
					{
						alphaMap[x, y] = true;
					}
				}
			}
		}

		// Token: 0x0600BA53 RID: 47699 RVA: 0x0054DBCC File Offset: 0x0054BDCC
		private static void TransformVerticesToRectTransform(List<Vector2> vertices, RectTransform referenceRectTransform, int width, int height)
		{
			bool flag = referenceRectTransform == null;
			if (!flag)
			{
				float scaleX = referenceRectTransform.rect.width / (float)width;
				float scaleY = referenceRectTransform.rect.height / (float)height;
				float rectX = referenceRectTransform.rect.x;
				float rectY = referenceRectTransform.rect.y;
				for (int i = 0; i < vertices.Count; i++)
				{
					float px = (vertices[i].x + 0.5f) * scaleX + rectX;
					float py = (vertices[i].y + 0.5f) * scaleY + rectY;
					vertices[i] = new Vector2(px, py);
				}
			}
		}

		// Token: 0x0600BA54 RID: 47700 RVA: 0x0054DC94 File Offset: 0x0054BE94
		private static List<Vector2> GeneratePolygonFromAlphaMap(bool[,] alphaMap, int width, int height)
		{
			PolygonGenerator.<>c__DisplayClass5_0 CS$<>8__locals1;
			CS$<>8__locals1.width = width;
			CS$<>8__locals1.height = height;
			CS$<>8__locals1.alphaMap = alphaMap;
			List<ValueTuple<Vector2, Vector2>> edges = new List<ValueTuple<Vector2, Vector2>>();
			for (int y = -1; y < CS$<>8__locals1.height; y++)
			{
				for (int x = -1; x < CS$<>8__locals1.width; x++)
				{
					bool bl = PolygonGenerator.<GeneratePolygonFromAlphaMap>g__GetAlpha|5_0(x, y, ref CS$<>8__locals1);
					bool br = PolygonGenerator.<GeneratePolygonFromAlphaMap>g__GetAlpha|5_0(x + 1, y, ref CS$<>8__locals1);
					bool tr = PolygonGenerator.<GeneratePolygonFromAlphaMap>g__GetAlpha|5_0(x + 1, y + 1, ref CS$<>8__locals1);
					bool tl = PolygonGenerator.<GeneratePolygonFromAlphaMap>g__GetAlpha|5_0(x, y + 1, ref CS$<>8__locals1);
					int caseIndex = (bl ? 1 : 0) | (br ? 2 : 0) | (tr ? 4 : 0) | (tl ? 8 : 0);
					Vector2 bottom = new Vector2((float)x + 0.5f, (float)y);
					Vector2 right = new Vector2((float)(x + 1), (float)y + 0.5f);
					Vector2 top = new Vector2((float)x + 0.5f, (float)(y + 1));
					Vector2 left = new Vector2((float)x, (float)y + 0.5f);
					switch (caseIndex)
					{
					case 1:
						edges.Add(new ValueTuple<Vector2, Vector2>(left, bottom));
						break;
					case 2:
						edges.Add(new ValueTuple<Vector2, Vector2>(bottom, right));
						break;
					case 3:
						edges.Add(new ValueTuple<Vector2, Vector2>(left, right));
						break;
					case 4:
						edges.Add(new ValueTuple<Vector2, Vector2>(right, top));
						break;
					case 5:
						edges.Add(new ValueTuple<Vector2, Vector2>(left, bottom));
						edges.Add(new ValueTuple<Vector2, Vector2>(right, top));
						break;
					case 6:
						edges.Add(new ValueTuple<Vector2, Vector2>(bottom, top));
						break;
					case 7:
						edges.Add(new ValueTuple<Vector2, Vector2>(left, top));
						break;
					case 8:
						edges.Add(new ValueTuple<Vector2, Vector2>(top, left));
						break;
					case 9:
						edges.Add(new ValueTuple<Vector2, Vector2>(top, bottom));
						break;
					case 10:
						edges.Add(new ValueTuple<Vector2, Vector2>(bottom, right));
						edges.Add(new ValueTuple<Vector2, Vector2>(top, left));
						break;
					case 11:
						edges.Add(new ValueTuple<Vector2, Vector2>(top, right));
						break;
					case 12:
						edges.Add(new ValueTuple<Vector2, Vector2>(right, left));
						break;
					case 13:
						edges.Add(new ValueTuple<Vector2, Vector2>(right, bottom));
						break;
					case 14:
						edges.Add(new ValueTuple<Vector2, Vector2>(bottom, left));
						break;
					}
				}
			}
			bool flag = edges.Count == 0;
			List<Vector2> result;
			if (flag)
			{
				Debug.LogError("No edges found in alpha map.");
				result = null;
			}
			else
			{
				Dictionary<Vector2, List<ValueTuple<Vector2, Vector2>>> edgeDict = new Dictionary<Vector2, List<ValueTuple<Vector2, Vector2>>>();
				foreach (ValueTuple<Vector2, Vector2> edge in edges)
				{
					bool flag2 = !edgeDict.ContainsKey(edge.Item1);
					if (flag2)
					{
						edgeDict[edge.Item1] = new List<ValueTuple<Vector2, Vector2>>();
					}
					edgeDict[edge.Item1].Add(edge);
				}
				List<List<Vector2>> allContours = new List<List<Vector2>>();
				HashSet<ValueTuple<Vector2, Vector2>> usedEdges = new HashSet<ValueTuple<Vector2, Vector2>>();
				while (usedEdges.Count < edges.Count)
				{
					ValueTuple<Vector2, Vector2> firstEdge = default(ValueTuple<Vector2, Vector2>);
					bool found = false;
					foreach (ValueTuple<Vector2, Vector2> edge2 in edges)
					{
						bool flag3 = !usedEdges.Contains(edge2);
						if (flag3)
						{
							firstEdge = edge2;
							found = true;
							break;
						}
					}
					bool flag4 = !found;
					if (flag4)
					{
						break;
					}
					List<Vector2> contour = new List<Vector2>
					{
						firstEdge.Item1
					};
					Vector2 currentEnd = firstEdge.Item2;
					usedEdges.Add(firstEdge);
					contour.Add(currentEnd);
					Vector2 startPoint = firstEdge.Item1;
					int safetyCounter = edges.Count + 1;
					while (!PolygonGenerator.ApproximatelyEqual(currentEnd, startPoint, 0.0001f) && safetyCounter > 0)
					{
						safetyCounter--;
						bool foundNext = false;
						List<ValueTuple<Vector2, Vector2>> candidateEdges;
						bool flag5 = edgeDict.TryGetValue(currentEnd, out candidateEdges);
						if (flag5)
						{
							foreach (ValueTuple<Vector2, Vector2> candidateEdge in candidateEdges)
							{
								bool flag6 = !usedEdges.Contains(candidateEdge);
								if (flag6)
								{
									usedEdges.Add(candidateEdge);
									currentEnd = candidateEdge.Item2;
									bool flag7 = !PolygonGenerator.ApproximatelyEqual(currentEnd, startPoint, 0.0001f);
									if (flag7)
									{
										contour.Add(currentEnd);
									}
									foundNext = true;
									break;
								}
							}
						}
						bool flag8 = !foundNext;
						if (flag8)
						{
							foreach (ValueTuple<Vector2, Vector2> edge3 in edges)
							{
								bool flag9 = !usedEdges.Contains(edge3) && PolygonGenerator.ApproximatelyEqual(edge3.Item2, currentEnd, 0.0001f);
								if (flag9)
								{
									usedEdges.Add(edge3);
									currentEnd = edge3.Item1;
									bool flag10 = !PolygonGenerator.ApproximatelyEqual(currentEnd, startPoint, 0.0001f);
									if (flag10)
									{
										contour.Add(currentEnd);
									}
									foundNext = true;
									break;
								}
							}
						}
						bool flag11 = !foundNext;
						if (flag11)
						{
							break;
						}
					}
					bool flag12 = contour.Count >= 3;
					if (flag12)
					{
						allContours.Add(contour);
					}
				}
				bool flag13 = allContours.Count == 0;
				if (flag13)
				{
					Debug.LogError("Failed to build any contour from edges.");
					result = null;
				}
				else
				{
					List<Vector2> largestContour = (from c in allContours
					orderby PolygonGenerator.CalculatePolygonArea(c) descending
					select c).First<List<Vector2>>();
					result = largestContour;
				}
			}
			return result;
		}

		// Token: 0x0600BA55 RID: 47701 RVA: 0x0054E2C8 File Offset: 0x0054C4C8
		private static bool ApproximatelyEqual(Vector2 a, Vector2 b, float tolerance = 0.0001f)
		{
			return Mathf.Abs(a.x - b.x) < tolerance && Mathf.Abs(a.y - b.y) < tolerance;
		}

		// Token: 0x0600BA56 RID: 47702 RVA: 0x0054E308 File Offset: 0x0054C508
		private static float CalculatePolygonArea(List<Vector2> polygon)
		{
			float area = 0f;
			int i = polygon.Count;
			for (int j = 0; j < i; j++)
			{
				int k = (j + 1) % i;
				area += polygon[j].x * polygon[k].y;
				area -= polygon[k].x * polygon[j].y;
			}
			return Mathf.Abs(area) / 2f;
		}

		// Token: 0x0600BA57 RID: 47703 RVA: 0x0054E388 File Offset: 0x0054C588
		public static List<Vector2> SimplifyPolygon(List<Vector2> vertices, float epsilon)
		{
			bool flag = vertices == null || vertices.Count < 3;
			List<Vector2> result;
			if (flag)
			{
				result = vertices;
			}
			else
			{
				bool flag2 = epsilon < 0f;
				if (flag2)
				{
					epsilon = 0f;
				}
				float areaThreshold = epsilon * epsilon;
				bool flag3 = areaThreshold < 1E-06f;
				if (flag3)
				{
					areaThreshold = 1E-06f;
				}
				result = PolygonGenerator.VisvalingamWhyatt(vertices, areaThreshold);
			}
			return result;
		}

		// Token: 0x0600BA58 RID: 47704 RVA: 0x0054E3E8 File Offset: 0x0054C5E8
		private static List<Vector2> VisvalingamWhyatt(List<Vector2> points, float areaThreshold)
		{
			bool flag = points.Count <= 3;
			List<Vector2> result;
			if (flag)
			{
				result = new List<Vector2>(points);
			}
			else
			{
				PolygonGenerator.<>c__DisplayClass9_0 CS$<>8__locals1;
				CS$<>8__locals1.result = new LinkedList<Vector2>(points);
				while (CS$<>8__locals1.result.Count > 3)
				{
					float minArea = float.MaxValue;
					LinkedListNode<Vector2> minNode = null;
					for (LinkedListNode<Vector2> current = CS$<>8__locals1.result.First; current != null; current = current.Next)
					{
						float area = PolygonGenerator.<VisvalingamWhyatt>g__CalculateArea|9_0(current, ref CS$<>8__locals1);
						bool flag2 = area < minArea;
						if (flag2)
						{
							minArea = area;
							minNode = current;
						}
					}
					bool flag3 = minArea >= areaThreshold;
					if (flag3)
					{
						break;
					}
					CS$<>8__locals1.result.Remove(minNode);
				}
				result = CS$<>8__locals1.result.ToList<Vector2>();
			}
			return result;
		}

		// Token: 0x0600BA59 RID: 47705 RVA: 0x0054E4B4 File Offset: 0x0054C6B4
		private static float TriangleArea(Vector2 a, Vector2 b, Vector2 c)
		{
			return Mathf.Abs((b.x - a.x) * (c.y - a.y) - (c.x - a.x) * (b.y - a.y)) / 2f;
		}

		// Token: 0x0600BA5A RID: 47706 RVA: 0x0054E508 File Offset: 0x0054C708
		private static float PerpendicularDistance(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
		{
			bool flag = PolygonGenerator.ApproximatelyEqual(lineStart, lineEnd, 0.0001f);
			float result;
			if (flag)
			{
				result = Vector2.Distance(point, lineStart);
			}
			else
			{
				float num = Mathf.Abs((lineEnd.y - lineStart.y) * point.x - (lineEnd.x - lineStart.x) * point.y + lineEnd.x * lineStart.y - lineEnd.y * lineStart.x);
				float den = Mathf.Sqrt(Mathf.Pow(lineEnd.y - lineStart.y, 2f) + Mathf.Pow(lineEnd.x - lineStart.x, 2f));
				result = num / den;
			}
			return result;
		}

		// Token: 0x0600BA5B RID: 47707 RVA: 0x0054E5BC File Offset: 0x0054C7BC
		[CompilerGenerated]
		internal static bool <GeneratePolygonFromAlphaMap>g__GetAlpha|5_0(int x, int y, ref PolygonGenerator.<>c__DisplayClass5_0 A_2)
		{
			bool flag = x < 0 || x >= A_2.width || y < 0 || y >= A_2.height;
			return !flag && A_2.alphaMap[x, y];
		}

		// Token: 0x0600BA5C RID: 47708 RVA: 0x0054E604 File Offset: 0x0054C804
		[CompilerGenerated]
		internal static float <VisvalingamWhyatt>g__CalculateArea|9_0(LinkedListNode<Vector2> node, ref PolygonGenerator.<>c__DisplayClass9_0 A_1)
		{
			LinkedListNode<Vector2> prev = node.Previous ?? A_1.result.Last;
			LinkedListNode<Vector2> next = node.Next ?? A_1.result.First;
			return PolygonGenerator.TriangleArea(prev.Value, node.Value, next.Value);
		}
	}
}
