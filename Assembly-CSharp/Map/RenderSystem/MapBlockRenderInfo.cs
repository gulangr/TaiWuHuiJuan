using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameData.Domains.Map;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Map.RenderSystem
{
	// Token: 0x020006CE RID: 1742
	public class MapBlockRenderInfo
	{
		// Token: 0x17000A45 RID: 2629
		// (get) Token: 0x060052E1 RID: 21217 RVA: 0x00263A5B File Offset: 0x00261C5B
		// (set) Token: 0x060052E2 RID: 21218 RVA: 0x00263A63 File Offset: 0x00261C63
		public short BlockIndex { get; private set; }

		// Token: 0x060052E3 RID: 21219 RVA: 0x00263A6C File Offset: 0x00261C6C
		public static Color32 GetStateColor(MapBlockRenderInfo.EMapBlockState state)
		{
			if (!true)
			{
			}
			Color32 result;
			switch (state)
			{
			case MapBlockRenderInfo.EMapBlockState.DarkVisible:
				result = MapBlockRenderInfo.InVisibleColor.BrightnessDelta(MapBlockRenderInfo.DarkVisibleBrightnessDelta);
				goto IL_63;
			case MapBlockRenderInfo.EMapBlockState.Visible:
				result = MapBlockRenderInfo.VisibleColor;
				goto IL_63;
			case MapBlockRenderInfo.EMapBlockState.InSight:
				result = MapBlockRenderInfo.InSightColor;
				goto IL_63;
			case MapBlockRenderInfo.EMapBlockState.InFlame:
				result = Colors.Instance["sectfulongflame"];
				goto IL_63;
			}
			result = MapBlockRenderInfo.InVisibleColor;
			IL_63:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x060052E4 RID: 21220 RVA: 0x00263AE1 File Offset: 0x00261CE1
		public static bool IsColoredState(MapBlockRenderInfo.EMapBlockState state)
		{
			return state == MapBlockRenderInfo.EMapBlockState.Visible || state == MapBlockRenderInfo.EMapBlockState.InSight || state == MapBlockRenderInfo.EMapBlockState.InFlame;
		}

		// Token: 0x17000A46 RID: 2630
		// (get) Token: 0x060052E5 RID: 21221 RVA: 0x00263AF2 File Offset: 0x00261CF2
		// (set) Token: 0x060052E6 RID: 21222 RVA: 0x00263AFA File Offset: 0x00261CFA
		public MapBlockRenderInfo.EMapBlockState State { get; private set; }

		// Token: 0x17000A47 RID: 2631
		// (get) Token: 0x060052E7 RID: 21223 RVA: 0x00263B03 File Offset: 0x00261D03
		// (set) Token: 0x060052E8 RID: 21224 RVA: 0x00263B0B File Offset: 0x00261D0B
		public bool ShowAsLeftEdge { get; private set; }

		// Token: 0x17000A48 RID: 2632
		// (get) Token: 0x060052E9 RID: 21225 RVA: 0x00263B14 File Offset: 0x00261D14
		// (set) Token: 0x060052EA RID: 21226 RVA: 0x00263B1C File Offset: 0x00261D1C
		public bool ShowAsRightEdge { get; private set; }

		// Token: 0x17000A49 RID: 2633
		// (get) Token: 0x060052EB RID: 21227 RVA: 0x00263B25 File Offset: 0x00261D25
		// (set) Token: 0x060052EC RID: 21228 RVA: 0x00263B2D File Offset: 0x00261D2D
		public bool ShowAsDownEdge { get; private set; }

		// Token: 0x17000A4A RID: 2634
		// (get) Token: 0x060052ED RID: 21229 RVA: 0x00263B36 File Offset: 0x00261D36
		// (set) Token: 0x060052EE RID: 21230 RVA: 0x00263B3E File Offset: 0x00261D3E
		public bool ShowAsUpEdge { get; private set; }

		// Token: 0x060052EF RID: 21231 RVA: 0x00263B47 File Offset: 0x00261D47
		public void SetBlockBottomCenter(Vector2 bottomCenter, short blockIndex)
		{
			this._blockBottomCenter = bottomCenter;
			this.BlockIndex = blockIndex;
			this.Clear();
		}

		// Token: 0x060052F0 RID: 21232 RVA: 0x00263B60 File Offset: 0x00261D60
		private void GenerateSpriteMeshFromBottomCenter(Vector2 bottomCenter, [TupleElementNames(new string[]
		{
			"sprite",
			"texIndex"
		})] ValueTuple<Sprite, float> spriteInfo, UIVertex[] vertexData, Color32 color, int vertexOffset = 0, bool flip = false)
		{
			bool flag = null == spriteInfo.Item1;
			if (!flag)
			{
				float spriteWidth = spriteInfo.Item1.rect.width;
				float spriteHeight = spriteInfo.Item1.rect.height;
				float xMin = bottomCenter.x - spriteWidth * 0.5f;
				float xMax = bottomCenter.x + spriteWidth * 0.5f;
				float yMin = bottomCenter.y;
				float yMax = bottomCenter.y + spriteHeight;
				vertexData[vertexOffset].position.x = xMin;
				vertexData[vertexOffset].position.y = yMax;
				vertexData[vertexOffset].position.z = 0f;
				vertexData[vertexOffset].color = color;
				vertexData[vertexOffset].uv0 = spriteInfo.Item1.uv[flip ? 2 : 0];
				vertexData[vertexOffset].uv1.x = spriteInfo.Item2;
				vertexData[vertexOffset].uv1.y = 0f;
				vertexData[vertexOffset + 1].position.x = xMax;
				vertexData[vertexOffset + 1].position.y = yMax;
				vertexData[vertexOffset + 1].position.z = 0f;
				vertexData[vertexOffset + 1].color = color;
				vertexData[vertexOffset + 1].uv0 = spriteInfo.Item1.uv[flip ? 3 : 1];
				vertexData[vertexOffset + 1].uv1.x = spriteInfo.Item2;
				vertexData[vertexOffset + 1].uv1.y = 0f;
				vertexData[vertexOffset + 2].position.x = xMin;
				vertexData[vertexOffset + 2].position.y = yMin;
				vertexData[vertexOffset + 2].position.z = 0f;
				vertexData[vertexOffset + 2].color = color;
				vertexData[vertexOffset + 2].uv0 = spriteInfo.Item1.uv[flip ? 0 : 2];
				vertexData[vertexOffset + 2].uv1.x = spriteInfo.Item2;
				vertexData[vertexOffset + 2].uv1.y = 0f;
				vertexData[vertexOffset + 3].position = vertexData[vertexOffset + 2].position;
				vertexData[vertexOffset + 3].color = color;
				vertexData[vertexOffset + 3].uv0 = spriteInfo.Item1.uv[flip ? 0 : 2];
				vertexData[vertexOffset + 3].uv1.x = spriteInfo.Item2;
				vertexData[vertexOffset + 3].uv1.y = 0f;
				vertexData[vertexOffset + 4].position = vertexData[vertexOffset + 1].position;
				vertexData[vertexOffset + 4].color = color;
				vertexData[vertexOffset + 4].uv0 = spriteInfo.Item1.uv[flip ? 3 : 1];
				vertexData[vertexOffset + 4].uv1.x = spriteInfo.Item2;
				vertexData[vertexOffset + 4].uv1.y = 0f;
				vertexData[vertexOffset + 5].position.x = xMax;
				vertexData[vertexOffset + 5].position.y = yMin;
				vertexData[vertexOffset + 5].position.z = 0f;
				vertexData[vertexOffset + 5].color = color;
				vertexData[vertexOffset + 5].uv0 = spriteInfo.Item1.uv[flip ? 1 : 3];
				vertexData[vertexOffset + 5].uv1.x = spriteInfo.Item2;
				vertexData[vertexOffset + 5].uv1.y = 0f;
			}
		}

		// Token: 0x060052F1 RID: 21233 RVA: 0x00263FC0 File Offset: 0x002621C0
		private void GenerateSpriteMeshFromBottomLeft(Vector2 bottomLeft, Sprite sprite, bool flip, UIVertex[] vertexData, int vertexOffset, Color32 color)
		{
			float spriteWidth = sprite.rect.width;
			float spriteHeight = sprite.rect.height;
			float xMin = bottomLeft.x;
			float xMax = bottomLeft.x + spriteWidth;
			float yMin = bottomLeft.y;
			float yMax = bottomLeft.y + spriteHeight;
			vertexData[vertexOffset].position.x = xMin;
			vertexData[vertexOffset].position.y = yMax;
			vertexData[vertexOffset].position.z = 0f;
			vertexData[vertexOffset].color = color;
			vertexData[vertexOffset].uv0 = sprite.uv[flip ? 1 : 0];
			vertexData[vertexOffset].uv1.x = 0f;
			vertexData[vertexOffset].uv1.y = 0f;
			vertexData[vertexOffset + 1].position.x = xMax;
			vertexData[vertexOffset + 1].position.y = yMax;
			vertexData[vertexOffset + 1].position.z = 0f;
			vertexData[vertexOffset + 1].color = color;
			vertexData[vertexOffset + 1].uv0 = sprite.uv[flip ? 0 : 1];
			vertexData[vertexOffset + 1].uv1.x = 0f;
			vertexData[vertexOffset + 1].uv1.y = 0f;
			vertexData[vertexOffset + 2].position.x = xMin;
			vertexData[vertexOffset + 2].position.y = yMin;
			vertexData[vertexOffset + 2].position.z = 0f;
			vertexData[vertexOffset + 2].color = color;
			vertexData[vertexOffset + 2].uv0 = sprite.uv[flip ? 3 : 2];
			vertexData[vertexOffset + 2].uv1.x = 0f;
			vertexData[vertexOffset + 2].uv1.y = 0f;
			vertexData[vertexOffset + 3].position = vertexData[vertexOffset + 2].position;
			vertexData[vertexOffset + 3].color = color;
			vertexData[vertexOffset + 3].uv0 = sprite.uv[flip ? 3 : 2];
			vertexData[vertexOffset + 3].uv1.x = 0f;
			vertexData[vertexOffset + 3].uv1.y = 0f;
			vertexData[vertexOffset + 4].position = vertexData[vertexOffset + 1].position;
			vertexData[vertexOffset + 4].color = color;
			vertexData[vertexOffset + 4].uv0 = sprite.uv[flip ? 0 : 1];
			vertexData[vertexOffset + 4].uv1.x = 0f;
			vertexData[vertexOffset + 4].uv1.y = 0f;
			vertexData[vertexOffset + 5].position.x = xMax;
			vertexData[vertexOffset + 5].position.y = yMin;
			vertexData[vertexOffset + 5].position.z = 0f;
			vertexData[vertexOffset + 5].color = color;
			vertexData[vertexOffset + 5].uv0 = sprite.uv[flip ? 2 : 3];
			vertexData[vertexOffset + 5].uv1.x = 0f;
			vertexData[vertexOffset + 5].uv1.y = 0f;
		}

		// Token: 0x060052F2 RID: 21234 RVA: 0x002643EC File Offset: 0x002625EC
		private void GenerateSpriteMeshFromBottomRight(Vector2 bottomRight, Sprite sprite, bool flip, UIVertex[] vertexData, int vertexOffset, Color32 color)
		{
			float spriteWidth = sprite.rect.width;
			float spriteHeight = sprite.rect.height;
			float xMin = bottomRight.x - spriteWidth;
			float xMax = bottomRight.x;
			float yMin = bottomRight.y;
			float yMax = bottomRight.y + spriteHeight;
			vertexData[vertexOffset].position.x = xMin;
			vertexData[vertexOffset].position.y = yMax;
			vertexData[vertexOffset].position.z = 0f;
			vertexData[vertexOffset].color = color;
			vertexData[vertexOffset].uv0 = sprite.uv[flip ? 1 : 0];
			vertexData[vertexOffset].uv1.x = 0f;
			vertexData[vertexOffset].uv1.y = 0f;
			vertexData[vertexOffset + 1].position.x = xMax;
			vertexData[vertexOffset + 1].position.y = yMax;
			vertexData[vertexOffset + 1].position.z = 0f;
			vertexData[vertexOffset + 1].color = color;
			vertexData[vertexOffset + 1].uv0 = sprite.uv[flip ? 0 : 1];
			vertexData[vertexOffset + 1].uv1.x = 0f;
			vertexData[vertexOffset + 1].uv1.y = 0f;
			vertexData[vertexOffset + 2].position.x = xMin;
			vertexData[vertexOffset + 2].position.y = yMin;
			vertexData[vertexOffset + 2].position.z = 0f;
			vertexData[vertexOffset + 2].color = color;
			vertexData[vertexOffset + 2].uv0 = sprite.uv[flip ? 3 : 2];
			vertexData[vertexOffset + 2].uv1.x = 0f;
			vertexData[vertexOffset + 2].uv1.y = 0f;
			vertexData[vertexOffset + 3].position = vertexData[vertexOffset + 2].position;
			vertexData[vertexOffset + 3].color = color;
			vertexData[vertexOffset + 3].uv0 = sprite.uv[flip ? 3 : 2];
			vertexData[vertexOffset + 3].uv1.x = 0f;
			vertexData[vertexOffset + 3].uv1.y = 0f;
			vertexData[vertexOffset + 4].position = vertexData[vertexOffset + 1].position;
			vertexData[vertexOffset + 4].color = color;
			vertexData[vertexOffset + 4].uv0 = sprite.uv[flip ? 0 : 1];
			vertexData[vertexOffset + 4].uv1.x = 0f;
			vertexData[vertexOffset + 4].uv1.y = 0f;
			vertexData[vertexOffset + 5].position.x = xMax;
			vertexData[vertexOffset + 5].position.y = yMin;
			vertexData[vertexOffset + 5].position.z = 0f;
			vertexData[vertexOffset + 5].color = color;
			vertexData[vertexOffset + 5].uv0 = sprite.uv[flip ? 2 : 3];
			vertexData[vertexOffset + 5].uv1.x = 0f;
			vertexData[vertexOffset + 5].uv1.y = 0f;
		}

		// Token: 0x060052F3 RID: 21235 RVA: 0x00264818 File Offset: 0x00262A18
		private void GenerateSpriteMeshFromTopLeft(Vector2 topLeft, Sprite sprite, bool flip, UIVertex[] vertexData, int vertexOffset, Color32 color)
		{
			float spriteWidth = sprite.rect.width;
			float spriteHeight = sprite.rect.height;
			float xMin = topLeft.x;
			float xMax = topLeft.x + spriteWidth;
			float yMin = topLeft.y - spriteHeight;
			float yMax = topLeft.y;
			vertexData[vertexOffset].position.x = xMin;
			vertexData[vertexOffset].position.y = yMax;
			vertexData[vertexOffset].position.z = 0f;
			vertexData[vertexOffset].color = color;
			vertexData[vertexOffset].uv0 = sprite.uv[flip ? 1 : 0];
			vertexData[vertexOffset].uv1.x = 0f;
			vertexData[vertexOffset].uv1.y = 0f;
			vertexData[vertexOffset + 1].position.x = xMax;
			vertexData[vertexOffset + 1].position.y = yMax;
			vertexData[vertexOffset + 1].position.z = 0f;
			vertexData[vertexOffset + 1].color = color;
			vertexData[vertexOffset + 1].uv0 = sprite.uv[flip ? 0 : 1];
			vertexData[vertexOffset + 1].uv1.x = 0f;
			vertexData[vertexOffset + 1].uv1.y = 0f;
			vertexData[vertexOffset + 2].position.x = xMin;
			vertexData[vertexOffset + 2].position.y = yMin;
			vertexData[vertexOffset + 2].position.z = 0f;
			vertexData[vertexOffset + 2].color = color;
			vertexData[vertexOffset + 2].uv0 = sprite.uv[flip ? 3 : 2];
			vertexData[vertexOffset + 2].uv1.x = 0f;
			vertexData[vertexOffset + 2].uv1.y = 0f;
			vertexData[vertexOffset + 3].position = vertexData[vertexOffset + 2].position;
			vertexData[vertexOffset + 3].color = color;
			vertexData[vertexOffset + 3].uv0 = sprite.uv[flip ? 3 : 2];
			vertexData[vertexOffset + 3].uv1.x = 0f;
			vertexData[vertexOffset + 3].uv1.y = 0f;
			vertexData[vertexOffset + 4].position = vertexData[vertexOffset + 1].position;
			vertexData[vertexOffset + 4].color = color;
			vertexData[vertexOffset + 4].uv0 = sprite.uv[flip ? 0 : 1];
			vertexData[vertexOffset + 4].uv1.x = 0f;
			vertexData[vertexOffset + 4].uv1.y = 0f;
			vertexData[vertexOffset + 5].position.x = xMax;
			vertexData[vertexOffset + 5].position.y = yMin;
			vertexData[vertexOffset + 5].position.z = 0f;
			vertexData[vertexOffset + 5].color = color;
			vertexData[vertexOffset + 5].uv0 = sprite.uv[flip ? 2 : 3];
			vertexData[vertexOffset + 5].uv1.x = 0f;
			vertexData[vertexOffset + 5].uv1.y = 0f;
		}

		// Token: 0x060052F4 RID: 21236 RVA: 0x00264C44 File Offset: 0x00262E44
		private void GenerateSpriteMeshFromTopRight(Vector2 topRight, Sprite sprite, bool flip, UIVertex[] vertexData, int vertexOffset, Color32 color)
		{
			float spriteWidth = sprite.rect.width;
			float spriteHeight = sprite.rect.height;
			float xMin = topRight.x - spriteWidth;
			float xMax = topRight.x;
			float yMin = topRight.y - spriteHeight;
			float yMax = topRight.y;
			vertexData[vertexOffset].position.x = xMin;
			vertexData[vertexOffset].position.y = yMax;
			vertexData[vertexOffset].position.z = 0f;
			vertexData[vertexOffset].color = color;
			vertexData[vertexOffset].uv0 = sprite.uv[flip ? 1 : 0];
			vertexData[vertexOffset].uv1.x = 0f;
			vertexData[vertexOffset].uv1.y = 0f;
			vertexData[vertexOffset + 1].position.x = xMax;
			vertexData[vertexOffset + 1].position.y = yMax;
			vertexData[vertexOffset + 1].position.z = 0f;
			vertexData[vertexOffset + 1].color = color;
			vertexData[vertexOffset + 1].uv0 = sprite.uv[flip ? 0 : 1];
			vertexData[vertexOffset + 1].uv1.x = 0f;
			vertexData[vertexOffset + 1].uv1.y = 0f;
			vertexData[vertexOffset + 2].position.x = xMin;
			vertexData[vertexOffset + 2].position.y = yMin;
			vertexData[vertexOffset + 2].position.z = 0f;
			vertexData[vertexOffset + 2].color = color;
			vertexData[vertexOffset + 2].uv0 = sprite.uv[flip ? 3 : 2];
			vertexData[vertexOffset + 2].uv1.x = 0f;
			vertexData[vertexOffset + 2].uv1.y = 0f;
			vertexData[vertexOffset + 3].position = vertexData[vertexOffset + 2].position;
			vertexData[vertexOffset + 3].color = color;
			vertexData[vertexOffset + 3].uv0 = sprite.uv[flip ? 3 : 2];
			vertexData[vertexOffset + 3].uv1.x = 0f;
			vertexData[vertexOffset + 3].uv1.y = 0f;
			vertexData[vertexOffset + 4].position = vertexData[vertexOffset + 1].position;
			vertexData[vertexOffset + 4].color = color;
			vertexData[vertexOffset + 4].uv0 = sprite.uv[flip ? 0 : 1];
			vertexData[vertexOffset + 4].uv1.x = 0f;
			vertexData[vertexOffset + 4].uv1.y = 0f;
			vertexData[vertexOffset + 5].position.x = xMax;
			vertexData[vertexOffset + 5].position.y = yMin;
			vertexData[vertexOffset + 5].position.z = 0f;
			vertexData[vertexOffset + 5].color = color;
			vertexData[vertexOffset + 5].uv0 = sprite.uv[flip ? 2 : 3];
			vertexData[vertexOffset + 5].uv1.x = 0f;
			vertexData[vertexOffset + 5].uv1.y = 0f;
		}

		// Token: 0x060052F5 RID: 21237 RVA: 0x00265070 File Offset: 0x00263270
		private void GenerateBorderVertexDataForVer921()
		{
			int vertexCount = 0;
			bool showAsLeftEdge = this.ShowAsLeftEdge;
			if (showAsLeftEdge)
			{
				vertexCount += 6;
			}
			bool showAsRightEdge = this.ShowAsRightEdge;
			if (showAsRightEdge)
			{
				vertexCount += 6;
			}
			bool showAsUpEdge = this.ShowAsUpEdge;
			if (showAsUpEdge)
			{
				vertexCount += 6;
			}
			bool showAsDownEdge = this.ShowAsDownEdge;
			if (showAsDownEdge)
			{
				vertexCount += 6;
			}
			bool flag = this._areaBorderVertexData == null || this._areaBorderVertexData.Length != vertexCount;
			if (flag)
			{
				this._areaBorderVertexData = new UIVertex[vertexCount];
			}
			Vector2[] points = this.GetTopBorderPoints();
			Vector2 top = points[0];
			Vector2 top2 = points[1];
			points = this.GetBottomBorderPoints();
			Vector2 bottom = points[0];
			Vector2 bottom2 = points[1];
			points = this.GetLeftBorderPoints();
			Vector2 left = points[0];
			Vector2 left2 = points[1];
			points = this.GetRightBorderPoints();
			Vector2 right = points[0];
			Vector2 right2 = points[1];
			int dataOffset = 0;
			bool showAsUpEdge2 = this.ShowAsUpEdge;
			if (showAsUpEdge2)
			{
				this.<GenerateBorderVertexDataForVer921>g__SetCornerDataToVertexArray|58_0(new Vector2[]
				{
					top,
					top2,
					right,
					right2
				}, dataOffset);
				dataOffset += 6;
			}
			bool showAsRightEdge2 = this.ShowAsRightEdge;
			if (showAsRightEdge2)
			{
				this.<GenerateBorderVertexDataForVer921>g__SetCornerDataToVertexArray|58_0(new Vector2[]
				{
					right,
					right2,
					bottom,
					bottom2
				}, dataOffset);
				dataOffset += 6;
			}
			bool showAsDownEdge2 = this.ShowAsDownEdge;
			if (showAsDownEdge2)
			{
				this.<GenerateBorderVertexDataForVer921>g__SetCornerDataToVertexArray|58_0(new Vector2[]
				{
					bottom,
					bottom2,
					left,
					left2
				}, dataOffset);
				dataOffset += 6;
			}
			bool showAsLeftEdge2 = this.ShowAsLeftEdge;
			if (showAsLeftEdge2)
			{
				this.<GenerateBorderVertexDataForVer921>g__SetCornerDataToVertexArray|58_0(new Vector2[]
				{
					left,
					left2,
					top,
					top2
				}, dataOffset);
			}
		}

		// Token: 0x060052F6 RID: 21238 RVA: 0x00265264 File Offset: 0x00263464
		public Vector2[] GetTopBorderPoints()
		{
			MapBlockRenderInfo target = null;
			sbyte code = MapBlockRenderInfo.System.GetBlockCornerCodeAndLinkTarget(this, MoveDirection.Up, ref target);
			bool flag = code == 10;
			Vector2[] result;
			if (flag)
			{
				result = target.GetRightBorderPoints();
			}
			else
			{
				bool flag2 = code == 11;
				if (flag2)
				{
					result = target.GetLeftBorderPoints();
				}
				else
				{
					Vector2 top = this._blockBottomCenter + Vector2.up * (MapRenderSystem.BlockBaseHeight + MapBlockRenderInfo.BlockBorderOffsetVertical);
					Vector2 top2 = top + Vector2.up * MapBlockRenderInfo.BlockBorderWidth;
					result = new Vector2[]
					{
						top,
						top2
					};
				}
			}
			return result;
		}

		// Token: 0x060052F7 RID: 21239 RVA: 0x00265300 File Offset: 0x00263500
		public Vector2[] GetBottomBorderPoints()
		{
			MapBlockRenderInfo target = null;
			sbyte code = MapBlockRenderInfo.System.GetBlockCornerCodeAndLinkTarget(this, MoveDirection.Down, ref target);
			bool flag = code == 20;
			Vector2[] result;
			if (flag)
			{
				result = target.GetLeftBorderPoints();
			}
			else
			{
				bool flag2 = code == 21;
				if (flag2)
				{
					result = target.GetRightBorderPoints();
				}
				else
				{
					Vector2 bottom = this._blockBottomCenter + Vector2.down * MapBlockRenderInfo.BlockBorderOffsetVertical;
					Vector2 bottom2 = bottom + Vector2.down * MapBlockRenderInfo.BlockBorderWidth;
					result = new Vector2[]
					{
						bottom,
						bottom2
					};
				}
			}
			return result;
		}

		// Token: 0x060052F8 RID: 21240 RVA: 0x00265398 File Offset: 0x00263598
		public Vector2[] GetLeftBorderPoints()
		{
			MapBlockRenderInfo target = null;
			sbyte code = MapBlockRenderInfo.System.GetBlockCornerCodeAndLinkTarget(this, MoveDirection.Left, ref target);
			bool flag = code == 30;
			Vector2[] result;
			if (flag)
			{
				result = target.GetTopBorderPoints();
			}
			else
			{
				bool flag2 = code == 31;
				if (flag2)
				{
					result = target.GetBottomBorderPoints();
				}
				else
				{
					Vector2 left = this._blockBottomCenter + new Vector2((float)(-(float)MapRenderSystem.BlockRenderSpace.x) * 0.5f, MapRenderSystem.BlockBaseHeight * 0.5f) + Vector2.left * MapBlockRenderInfo.BlockBorderOffsetHorizontal;
					Vector2 left2 = left + Vector2.left * (MapBlockRenderInfo.BlockBorderWidth * MapBlockRenderInfo.BlockBorderWidthRate);
					result = new Vector2[]
					{
						left,
						left2
					};
				}
			}
			return result;
		}

		// Token: 0x060052F9 RID: 21241 RVA: 0x00265464 File Offset: 0x00263664
		public Vector2[] GetRightBorderPoints()
		{
			MapBlockRenderInfo target = null;
			sbyte code = MapBlockRenderInfo.System.GetBlockCornerCodeAndLinkTarget(this, MoveDirection.Right, ref target);
			bool flag = code == 40;
			Vector2[] result;
			if (flag)
			{
				result = target.GetBottomBorderPoints();
			}
			else
			{
				bool flag2 = code == 41;
				if (flag2)
				{
					result = target.GetTopBorderPoints();
				}
				else
				{
					Vector2 right = this._blockBottomCenter + new Vector2((float)MapRenderSystem.BlockRenderSpace.x * 0.5f, MapRenderSystem.BlockBaseHeight * 0.5f) + Vector2.right * MapBlockRenderInfo.BlockBorderOffsetHorizontal;
					Vector2 right2 = right + Vector2.right * (MapBlockRenderInfo.BlockBorderWidth * MapBlockRenderInfo.BlockBorderWidthRate);
					result = new Vector2[]
					{
						right,
						right2
					};
				}
			}
			return result;
		}

		// Token: 0x060052FA RID: 21242 RVA: 0x00265530 File Offset: 0x00263730
		public void TryGenerateBlockVertexData()
		{
			Color32 color = MapBlockRenderInfo.GetStateColor(this.State);
			if (this._blockVertexData == null)
			{
				this._blockVertexData = new UIVertex[6];
			}
			ValueTuple<Sprite, float> spriteInfo = new ValueTuple<Sprite, float>(this._showingSpriteInfo.Item1, (float)this._showingSpriteInfo.Item2 / 7f);
			this.GenerateSpriteMeshFromBottomCenter(this._blockBottomCenter, spriteInfo, this._blockVertexData, color, 0, false);
			this._vertexDirtyFlags[0] = false;
		}

		// Token: 0x060052FB RID: 21243 RVA: 0x002655A4 File Offset: 0x002637A4
		public UIVertex[] GetBlockVertexData(bool generateIfNotExist = false)
		{
			bool flag = this._vertexDirtyFlags[0];
			if (flag)
			{
				this.TryGenerateBlockVertexData();
			}
			bool flag2 = this.State != MapBlockRenderInfo.EMapBlockState.Empty && this.State != MapBlockRenderInfo.EMapBlockState.Invisible;
			UIVertex[] result;
			if (flag2)
			{
				result = this._blockVertexData;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060052FC RID: 21244 RVA: 0x002655F0 File Offset: 0x002637F0
		public UIVertex[] GetBlockShadowVertexData()
		{
			bool flag = this.State != MapBlockRenderInfo.EMapBlockState.Empty && this.State != MapBlockRenderInfo.EMapBlockState.Invisible;
			UIVertex[] result;
			if (flag)
			{
				bool flag2 = this._vertexDirtyFlags[1] || this._blockShadowVertexData == null;
				if (flag2)
				{
					if (this._blockShadowVertexData == null)
					{
						this._blockShadowVertexData = new UIVertex[6];
					}
					Vector2 bottomCenter = this._blockBottomCenter + MapBlockRenderInfo.BlockShadowOffset;
					this.GenerateSpriteMeshFromBottomCenter(bottomCenter, new ValueTuple<Sprite, float>(MapRenderSystem.BlockShadowSprite, 0f), this._blockShadowVertexData, Color.white, 0, false);
					this._vertexDirtyFlags[1] = false;
				}
				result = this._blockShadowVertexData;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060052FD RID: 21245 RVA: 0x0026569C File Offset: 0x0026389C
		public UIVertex[] GetBlockPreparationVertexData()
		{
			bool flag = this.State == MapBlockRenderInfo.EMapBlockState.Preparation;
			UIVertex[] result;
			if (flag)
			{
				bool flag2 = this._vertexDirtyFlags[2] || this._blockPreparationVertexData == null;
				if (flag2)
				{
					if (this._blockPreparationVertexData == null)
					{
						this._blockPreparationVertexData = new UIVertex[6];
					}
					this.GenerateSpriteMeshFromBottomCenter(this._blockBottomCenter, new ValueTuple<Sprite, float>(MapRenderSystem.BlockPreparationSprite, 0f), this._blockPreparationVertexData, Color.white, 0, false);
					this._vertexDirtyFlags[2] = false;
				}
				result = this._blockPreparationVertexData;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060052FE RID: 21246 RVA: 0x00265730 File Offset: 0x00263930
		public UIVertex[] GetAreaBorderVertexDataFor921()
		{
			bool flag = this._vertexDirtyFlags[3];
			if (flag)
			{
				this.GenerateBorderVertexDataForVer921();
				this._vertexDirtyFlags[3] = false;
			}
			return this._areaBorderVertexData;
		}

		// Token: 0x060052FF RID: 21247 RVA: 0x00265768 File Offset: 0x00263968
		public UIVertex[] GetAreaBorderVertexData()
		{
			bool flag = this._vertexDirtyFlags[3] || this._areaBorderVertexData == null;
			if (flag)
			{
				int vertexCount = 0;
				bool showAsLeftEdge = this.ShowAsLeftEdge;
				if (showAsLeftEdge)
				{
					vertexCount += 6;
				}
				bool showAsUpEdge = this.ShowAsUpEdge;
				if (showAsUpEdge)
				{
					vertexCount += 6;
				}
				bool showAsRightEdge = this.ShowAsRightEdge;
				if (showAsRightEdge)
				{
					vertexCount += 6;
					bool showAsUpEdge2 = this.ShowAsUpEdge;
					if (showAsUpEdge2)
					{
						vertexCount += 6;
					}
				}
				bool showAsDownEdge = this.ShowAsDownEdge;
				if (showAsDownEdge)
				{
					vertexCount += 6;
					bool showAsLeftEdge2 = this.ShowAsLeftEdge;
					if (showAsLeftEdge2)
					{
						vertexCount += 6;
					}
				}
				bool flag2 = this._areaBorderVertexData == null || this._areaBorderVertexData.Length != vertexCount;
				if (flag2)
				{
					this._areaBorderVertexData = new UIVertex[vertexCount];
				}
				int vertexOffset = 0;
				bool showAsLeftEdge3 = this.ShowAsLeftEdge;
				if (showAsLeftEdge3)
				{
					Vector2 anchorPoint = this._blockBottomCenter + MapBlockRenderInfo.BlockBaseOffset + new Vector2(-0.5f * MapRenderSystem.BlockBaseSprite.rect.width, 0.5f * MapRenderSystem.BlockBaseSprite.rect.height - 2f);
					this.GenerateSpriteMeshFromBottomLeft(anchorPoint, MapRenderSystem.UpAreaEdgeSprite, true, this._areaBorderVertexData, vertexOffset, Color.white);
					vertexOffset += 6;
				}
				bool showAsUpEdge3 = this.ShowAsUpEdge;
				if (showAsUpEdge3)
				{
					Vector2 anchorPoint2 = this._blockBottomCenter + MapBlockRenderInfo.BlockBaseOffset + new Vector2(2f, MapRenderSystem.BlockBaseSprite.rect.height + 3f);
					this.GenerateSpriteMeshFromTopLeft(anchorPoint2, MapRenderSystem.UpAreaEdgeSprite, false, this._areaBorderVertexData, vertexOffset, Color.white);
					vertexOffset += 6;
				}
				bool showAsRightEdge2 = this.ShowAsRightEdge;
				if (showAsRightEdge2)
				{
					Vector2 anchorPoint3 = this._blockBottomCenter + MapBlockRenderInfo.BlockBaseOffset;
					anchorPoint3.y -= 98f;
					this.GenerateSpriteMeshFromBottomLeft(anchorPoint3, MapRenderSystem.DownAreaEdgeSprite, true, this._areaBorderVertexData, vertexOffset, Color.white);
					vertexOffset += 6;
					bool showAsUpEdge4 = this.ShowAsUpEdge;
					if (showAsUpEdge4)
					{
						anchorPoint3.x += MapRenderSystem.DownAreaEdgeSprite.rect.width;
						anchorPoint3.y += MapRenderSystem.DownAreaEdgeSprite.rect.height + 9f;
						this.GenerateSpriteMeshFromTopLeft(anchorPoint3, MapRenderSystem.AreaTableCornerSprite, true, this._areaBorderVertexData, vertexOffset, Color.white);
						vertexOffset += 6;
					}
				}
				bool showAsDownEdge2 = this.ShowAsDownEdge;
				if (showAsDownEdge2)
				{
					Vector2 anchorPoint4 = this._blockBottomCenter + MapBlockRenderInfo.BlockBaseOffset;
					anchorPoint4.y -= 98f;
					this.GenerateSpriteMeshFromBottomRight(anchorPoint4, MapRenderSystem.DownAreaEdgeSprite, false, this._areaBorderVertexData, vertexOffset, Color.white);
					vertexOffset += 6;
					bool showAsLeftEdge4 = this.ShowAsLeftEdge;
					if (showAsLeftEdge4)
					{
						anchorPoint4.x -= MapRenderSystem.DownAreaEdgeSprite.rect.width;
						anchorPoint4.y += MapRenderSystem.DownAreaEdgeSprite.rect.height + 9f;
						this.GenerateSpriteMeshFromTopRight(anchorPoint4, MapRenderSystem.AreaTableCornerSprite, false, this._areaBorderVertexData, vertexOffset, Color.white);
					}
				}
			}
			return this._areaBorderVertexData;
		}

		// Token: 0x06005300 RID: 21248 RVA: 0x00265AB8 File Offset: 0x00263CB8
		public UIVertex[] GetAreaBorderCornerVertexData()
		{
			bool flag = this._vertexDirtyFlags[3];
			if (flag)
			{
				WorldMapModel model = SingletonObject.getInstance<WorldMapModel>();
				MapBlockData selfBlock = model.GetBlockData(new Location(model.ShowingAreaId, this.BlockIndex));
				MapBlockData upBlock = model.GetNeighbor(selfBlock, MoveDirection.Up, true);
				MapBlockData downBlock = model.GetNeighbor(selfBlock, MoveDirection.Down, true);
				MapBlockData leftBlock = model.GetNeighbor(selfBlock, MoveDirection.Left, true);
				MapBlockData rightBlock = model.GetNeighbor(selfBlock, MoveDirection.Right, true);
				MapBlockRenderInfo upBlockRenderInfo = (upBlock == null) ? null : MapBlockRenderInfo.System.GetBlockRenderInfo((int)upBlock.BlockId);
				MapBlockRenderInfo downBlockRenderInfo = (downBlock == null) ? null : MapBlockRenderInfo.System.GetBlockRenderInfo((int)downBlock.BlockId);
				MapBlockRenderInfo leftBlockRenderInfo = (leftBlock == null) ? null : MapBlockRenderInfo.System.GetBlockRenderInfo((int)leftBlock.BlockId);
				MapBlockRenderInfo rightBlockRenderInfo = (rightBlock == null) ? null : MapBlockRenderInfo.System.GetBlockRenderInfo((int)rightBlock.BlockId);
				bool upCornerFlag = !this.ShowAsLeftEdge && !this.ShowAsUpEdge && leftBlockRenderInfo != null && leftBlockRenderInfo.ShowAsUpEdge && upBlockRenderInfo != null && upBlockRenderInfo.ShowAsLeftEdge;
				bool downCornerFlag = !this.ShowAsDownEdge && !this.ShowAsRightEdge && downBlockRenderInfo != null && downBlockRenderInfo.ShowAsRightEdge && rightBlockRenderInfo != null && rightBlockRenderInfo.ShowAsDownEdge;
				bool leftCornerFlag = !this.ShowAsLeftEdge && !this.ShowAsDownEdge && leftBlockRenderInfo != null && leftBlockRenderInfo.ShowAsDownEdge && downBlockRenderInfo != null && downBlockRenderInfo.ShowAsLeftEdge;
				bool rightCornerFlag = !this.ShowAsUpEdge && !this.ShowAsRightEdge && upBlockRenderInfo != null && upBlockRenderInfo.ShowAsRightEdge && rightBlockRenderInfo != null && rightBlockRenderInfo.ShowAsUpEdge;
				int vertexCount = 0;
				bool flag2 = upCornerFlag;
				if (flag2)
				{
					vertexCount += 6;
				}
				bool flag3 = downCornerFlag;
				if (flag3)
				{
					vertexCount += 6;
				}
				bool flag4 = leftCornerFlag;
				if (flag4)
				{
					vertexCount += 6;
				}
				bool flag5 = rightCornerFlag;
				if (flag5)
				{
					vertexCount += 6;
				}
				bool flag6 = vertexCount <= 0;
				if (flag6)
				{
					return null;
				}
				bool flag7 = this._areaBorderCornerVertexData == null || this._areaBorderCornerVertexData.Length != vertexCount;
				if (flag7)
				{
					this._areaBorderCornerVertexData = new UIVertex[vertexCount];
				}
				int vertexOffset = 0;
				bool flag8 = upCornerFlag;
				if (flag8)
				{
					Vector2 anchorPoint = this._blockBottomCenter + MapBlockRenderInfo.BlockBaseOffset + new Vector2(0f, MapRenderSystem.BlockBaseSprite.rect.height + 2f);
					this.GenerateSpriteMeshFromBottomCenter(anchorPoint, new ValueTuple<Sprite, float>(MapRenderSystem.VerticalAreaCornerSprite, 0f), this._areaBorderCornerVertexData, Color.white, vertexOffset, true);
					vertexOffset += 6;
				}
				bool flag9 = downCornerFlag;
				if (flag9)
				{
					Vector2 spriteSize = MapRenderSystem.VerticalAreaCornerSprite.rect.size;
					Vector2 anchorPoint2 = this._blockBottomCenter + MapBlockRenderInfo.BlockBaseOffset + new Vector2(-spriteSize.x * 0.5f, -2f);
					this.GenerateSpriteMeshFromTopLeft(anchorPoint2, MapRenderSystem.VerticalAreaCornerSprite, false, this._areaBorderCornerVertexData, vertexOffset, Color.white);
					vertexOffset += 6;
				}
				bool flag10 = leftCornerFlag;
				if (flag10)
				{
					Vector2 anchorPoint3 = this._blockBottomCenter + MapBlockRenderInfo.BlockBaseOffset + new Vector2(-MapRenderSystem.BlockBaseSprite.rect.width * 0.5f - 5f, MapRenderSystem.BlockBaseSprite.rect.height * 0.5f - MapRenderSystem.HorizontalAreaCornerSprite.rect.height * 0.5f);
					this.GenerateSpriteMeshFromBottomRight(anchorPoint3, MapRenderSystem.HorizontalAreaCornerSprite, true, this._areaBorderCornerVertexData, vertexOffset, Color.white);
					vertexOffset += 6;
				}
				bool flag11 = rightCornerFlag;
				if (flag11)
				{
					Vector2 anchorPoint4 = this._blockBottomCenter + MapBlockRenderInfo.BlockBaseOffset + new Vector2(MapRenderSystem.BlockBaseSprite.rect.width * 0.5f + 5f, MapRenderSystem.BlockBaseSprite.rect.height * 0.5f - MapRenderSystem.HorizontalAreaCornerSprite.rect.height * 0.5f);
					this.GenerateSpriteMeshFromBottomLeft(anchorPoint4, MapRenderSystem.HorizontalAreaCornerSprite, false, this._areaBorderCornerVertexData, vertexOffset, Color.white);
				}
				this._vertexDirtyFlags[3] = false;
			}
			return this._areaBorderCornerVertexData;
		}

		// Token: 0x06005301 RID: 21249 RVA: 0x00265F08 File Offset: 0x00264108
		public UIVertex[] GetBlockBaseVertexData()
		{
			bool flag = this._vertexDirtyFlags[3];
			if (flag)
			{
				if (this._blockBaseVertexData == null)
				{
					this._blockBaseVertexData = new UIVertex[6];
				}
				Vector2 bottomCenter = this._blockBottomCenter + MapBlockRenderInfo.BlockBaseOffset;
				this.GenerateSpriteMeshFromBottomCenter(bottomCenter, new ValueTuple<Sprite, float>(MapRenderSystem.BlockBaseSprite, 0f), this._blockBaseVertexData, Color.white, 0, false);
				this._vertexDirtyFlags[3] = false;
			}
			return this._blockBaseVertexData;
		}

		// Token: 0x06005302 RID: 21250 RVA: 0x00265F88 File Offset: 0x00264188
		public UIVertex[] GetAreaBlockShadowVertexData()
		{
			bool flag = this._vertexDirtyFlags[4];
			if (flag)
			{
				if (this._areaBlockBaseShadowVertexData == null)
				{
					this._areaBlockBaseShadowVertexData = new UIVertex[6];
				}
				Vector2 bottomCenter = this._blockBottomCenter + MapBlockRenderInfo.BlockBaseShadowOffset;
				this.GenerateSpriteMeshFromBottomCenter(bottomCenter, new ValueTuple<Sprite, float>(MapRenderSystem.BlockBaseShadowSprite, 0f), this._areaBlockBaseShadowVertexData, Color.white, 0, false);
				this._vertexDirtyFlags[4] = false;
			}
			return this._areaBlockBaseShadowVertexData;
		}

		// Token: 0x06005303 RID: 21251 RVA: 0x00266005 File Offset: 0x00264205
		public void SetShowAsLeftEdge(bool flag)
		{
			this.ShowAsLeftEdge = flag;
			this._vertexDirtyFlags[3] = true;
		}

		// Token: 0x06005304 RID: 21252 RVA: 0x00266019 File Offset: 0x00264219
		public void SetShowAsRightEdge(bool flag)
		{
			this.ShowAsRightEdge = flag;
			this._vertexDirtyFlags[3] = true;
		}

		// Token: 0x06005305 RID: 21253 RVA: 0x0026602D File Offset: 0x0026422D
		public void SetShowAsDownEdge(bool flag)
		{
			this.ShowAsDownEdge = flag;
			this._vertexDirtyFlags[3] = true;
		}

		// Token: 0x06005306 RID: 21254 RVA: 0x00266041 File Offset: 0x00264241
		public void SetShowAsUpEdge(bool flag)
		{
			this.ShowAsUpEdge = flag;
			this._vertexDirtyFlags[3] = true;
		}

		// Token: 0x06005307 RID: 21255 RVA: 0x00266055 File Offset: 0x00264255
		public void SetSprite(Sprite sprite, sbyte textureIndex)
		{
			this._showingSpriteInfo = new ValueTuple<Sprite, sbyte>(sprite, textureIndex);
			this._vertexDirtyFlags[0] = true;
		}

		// Token: 0x06005308 RID: 21256 RVA: 0x00266070 File Offset: 0x00264270
		public Sprite GetSprite()
		{
			return this._showingSpriteInfo.Item1;
		}

		// Token: 0x06005309 RID: 21257 RVA: 0x00266090 File Offset: 0x00264290
		public void SetBlockState(MapBlockRenderInfo.EMapBlockState newState)
		{
			bool flag = this.State == newState;
			if (!flag)
			{
				this._previousState = this.State;
				this.State = newState;
				bool flag2 = newState != MapBlockRenderInfo.EMapBlockState.Invisible;
				if (flag2)
				{
					bool flag3 = this._blockVertexData == null;
					if (flag3)
					{
						this.TryGenerateBlockVertexData();
					}
					this.GetBlockShadowVertexData();
				}
				bool flag4 = newState == MapBlockRenderInfo.EMapBlockState.Invisible && this.BlockEffectList != null;
				if (flag4)
				{
					bool flag5 = this.BlockEffectList != null;
					if (flag5)
					{
						this.BlockEffectList.ForEach(new Action<MapBlockEffect>(MapBlockRenderInfo.System.ReturnMapBlockEffect));
						this.BlockEffectList.Clear();
					}
				}
				bool flag6 = newState != MapBlockRenderInfo.EMapBlockState.NegativeFilm && this.BlockEffectList != null;
				if (flag6)
				{
					foreach (MapBlockEffect blockEffect in this.BlockEffectList)
					{
						blockEffect.SetNegativeFilmEffectValue(0f);
					}
				}
			}
		}

		// Token: 0x0600530A RID: 21258 RVA: 0x002661A8 File Offset: 0x002643A8
		public void SetNegativeFilmBaseColor(Color32 color)
		{
			bool flag = this._blockVertexData == null;
			if (!flag)
			{
				for (int i = 0; i < this._blockVertexData.Length; i++)
				{
					this._blockVertexData[i].color = color;
				}
			}
		}

		// Token: 0x0600530B RID: 21259 RVA: 0x002661F0 File Offset: 0x002643F0
		public void Clear()
		{
			this.State = MapBlockRenderInfo.EMapBlockState.Empty;
			this._vertexDirtyFlags = new bool[]
			{
				true,
				true,
				true,
				true,
				true
			};
			this._blockVertexData = null;
			this._blockShadowVertexData = null;
			this._blockPreparationVertexData = null;
			this._areaBorderVertexData = null;
			this._areaBorderCornerVertexData = null;
			this._blockBaseVertexData = null;
			this._areaBlockBaseShadowVertexData = null;
			this.SetShowAsLeftEdge(false);
			this.SetShowAsRightEdge(false);
			this.SetShowAsDownEdge(false);
			this.SetShowAsUpEdge(false);
			this._showingSpriteInfo = new ValueTuple<Sprite, sbyte>(null, 0);
			bool flag = this.BlockEffectList != null;
			if (flag)
			{
				this.BlockEffectList.ForEach(new Action<MapBlockEffect>(MapBlockRenderInfo.System.ReturnMapBlockEffect));
				this.BlockEffectList.Clear();
			}
			this.SetBlockState(MapBlockRenderInfo.EMapBlockState.Invisible);
		}

		// Token: 0x0600530C RID: 21260 RVA: 0x002662BC File Offset: 0x002644BC
		public Vector2 GetBlockCenter()
		{
			return this._blockBottomCenter + Vector2.up * (MapRenderSystem.BlockBaseHeight * 0.5f);
		}

		// Token: 0x0600530D RID: 21261 RVA: 0x002662F0 File Offset: 0x002644F0
		public Vector3[] GetBlockSpriteVertexPositions()
		{
			UIVertex[] vertexArray = new UIVertex[6];
			ValueTuple<Sprite, float> spriteInfo = new ValueTuple<Sprite, float>(this._showingSpriteInfo.Item1, (float)this._showingSpriteInfo.Item2 / 7f);
			this.GenerateSpriteMeshFromBottomCenter(this._blockBottomCenter, spriteInfo, vertexArray, Color.white, 0, false);
			Vector3[] positionsArray = new Vector3[6];
			for (int i = 0; i < 6; i++)
			{
				positionsArray[i] = vertexArray[i].position;
			}
			return positionsArray;
		}

		// Token: 0x0600530E RID: 21262 RVA: 0x0026637C File Offset: 0x0026457C
		public Color32 GetCurrentVertexColor()
		{
			return (this._blockVertexData != null) ? this._blockVertexData[0].color : MapBlockRenderInfo.InVisibleColor;
		}

		// Token: 0x0600530F RID: 21263 RVA: 0x002663B0 File Offset: 0x002645B0
		public void ApplyAnimBlockHide(float progress, Color32 colorFrom)
		{
			bool flag = this._blockVertexData == null || this._blockShadowVertexData == null;
			if (!flag)
			{
				float realProgress = MapRenderSystem.BlockAppearAnimCurve.Evaluate(progress);
				for (int i = 0; i < 6; i++)
				{
					this._blockVertexData[i].color = Color.Lerp(colorFrom, MapBlockRenderInfo.InVisibleColor, realProgress);
					this._blockShadowVertexData[i].color.a = (byte)Mathf.Lerp(255f, 0f, realProgress);
				}
			}
		}

		// Token: 0x06005310 RID: 21264 RVA: 0x0026644C File Offset: 0x0026464C
		public void ApplyAnimBlockAppear(float progress, Vector3[] finalVertexPositions)
		{
			float realProgress = MapRenderSystem.BlockAppearAnimCurve.Evaluate(progress);
			Color32 targetColor = MapBlockRenderInfo.GetStateColor(this.State);
			for (int i = 0; i < 6; i++)
			{
				this._blockVertexData[i].color = Color.Lerp(MapBlockRenderInfo.InVisibleColor, targetColor, realProgress);
				this._blockVertexData[i].position.y = Mathf.Lerp(finalVertexPositions[i].y + MapBlockRenderInfo.BlockShadowOffset.y * 2f, finalVertexPositions[i].y, realProgress);
				this._blockShadowVertexData[i].color.a = (byte)Mathf.Lerp(0f, 255f, realProgress);
			}
		}

		// Token: 0x06005311 RID: 21265 RVA: 0x00266524 File Offset: 0x00264724
		public void ApplyAnimBlockColorChanged(float progress, Color32 colorFrom, Color32 colorTo)
		{
			float realProgress = MapRenderSystem.BlockAppearAnimCurve.Evaluate(progress);
			for (int i = 0; i < 6; i++)
			{
				this._blockVertexData[i].color = Color.Lerp(colorFrom, colorTo, realProgress);
			}
		}

		// Token: 0x06005312 RID: 21266 RVA: 0x00266578 File Offset: 0x00264778
		public void ApplyAnimBlockNegativeFilm(float progress)
		{
			float realProgress = MapRenderSystem.BlockAppearAnimCurve.Evaluate(progress);
			float fromValue = (float)((this.State == MapBlockRenderInfo.EMapBlockState.NegativeFilm) ? 0 : 1);
			float toValue = (float)((this.State == MapBlockRenderInfo.EMapBlockState.NegativeFilm) ? 1 : 0);
			for (int i = 0; i < 6; i++)
			{
				bool flag = this._previousState == MapBlockRenderInfo.EMapBlockState.Invisible;
				if (flag)
				{
					this._blockVertexData[i].color = Color.Lerp(MapBlockRenderInfo.InVisibleColor, MapBlockRenderInfo.InSightColor, progress);
				}
				else
				{
					bool flag2 = this._previousState == MapBlockRenderInfo.EMapBlockState.NegativeFilm;
					if (flag2)
					{
						this._blockVertexData[i].color = MapBlockRenderInfo.GetStateColor(this.State);
					}
				}
				this._blockVertexData[i].uv1.y = Mathf.Lerp(fromValue, toValue, realProgress);
			}
			float finallyProgress = Mathf.Lerp(fromValue, toValue, realProgress);
			bool flag3 = this.BlockEffectList != null;
			if (flag3)
			{
				foreach (MapBlockEffect mapBlockEffect in this.BlockEffectList)
				{
					mapBlockEffect.SetNegativeFilmEffectValue(finallyProgress);
				}
			}
		}

		// Token: 0x06005313 RID: 21267 RVA: 0x002666C4 File Offset: 0x002648C4
		public void ApplyAnimBlockDarkVisible(float progress)
		{
			float realProgress = MapRenderSystem.BlockAppearAnimCurve.Evaluate(progress);
			int fromValue = (this.State == MapBlockRenderInfo.EMapBlockState.DarkVisible) ? 0 : MapBlockRenderInfo.DarkVisibleBrightnessDelta;
			int toValue = (this.State == MapBlockRenderInfo.EMapBlockState.DarkVisible) ? MapBlockRenderInfo.DarkVisibleBrightnessDelta : 0;
			Color32 finalColor = MapBlockRenderInfo.InSightColor;
			bool flag = this.State == MapBlockRenderInfo.EMapBlockState.Visible && fromValue != 0;
			if (flag)
			{
				finalColor = MapBlockRenderInfo.VisibleColor;
			}
			for (int i = 0; i < 6; i++)
			{
				int delta = (int)Mathf.Lerp((float)fromValue, (float)toValue, realProgress);
				this._blockVertexData[i].color = finalColor.BrightnessDelta(delta);
			}
		}

		// Token: 0x06005316 RID: 21270 RVA: 0x00266838 File Offset: 0x00264A38
		[CompilerGenerated]
		private void <GenerateBorderVertexDataForVer921>g__SetCornerDataToVertexArray|58_0(Vector2[] corners, int offset)
		{
			this._areaBorderVertexData[offset].position.x = corners[0].x;
			this._areaBorderVertexData[offset].position.y = corners[0].y;
			this._areaBorderVertexData[offset].position.z = 0f;
			this._areaBorderVertexData[offset].color = MapRenderSystem.EdgeColor;
			this._areaBorderVertexData[offset + 1].position.x = corners[1].x;
			this._areaBorderVertexData[offset + 1].position.y = corners[1].y;
			this._areaBorderVertexData[offset + 1].position.z = 0f;
			this._areaBorderVertexData[offset + 1].color = MapRenderSystem.EdgeColor;
			this._areaBorderVertexData[offset + 2].position.x = corners[2].x;
			this._areaBorderVertexData[offset + 2].position.y = corners[2].y;
			this._areaBorderVertexData[offset + 2].position.z = 0f;
			this._areaBorderVertexData[offset + 2].color = MapRenderSystem.EdgeColor;
			this._areaBorderVertexData[offset + 3].position = this._areaBorderVertexData[offset + 2].position;
			this._areaBorderVertexData[offset + 3].color = MapRenderSystem.EdgeColor;
			this._areaBorderVertexData[offset + 4].position = this._areaBorderVertexData[offset + 1].position;
			this._areaBorderVertexData[offset + 4].color = MapRenderSystem.EdgeColor;
			this._areaBorderVertexData[offset + 5].position.x = corners[3].x;
			this._areaBorderVertexData[offset + 5].position.y = corners[3].y;
			this._areaBorderVertexData[offset + 5].position.z = 0f;
			this._areaBorderVertexData[offset + 5].color = MapRenderSystem.EdgeColor;
		}

		// Token: 0x04003822 RID: 14370
		public static MapRenderSystem System;

		// Token: 0x04003823 RID: 14371
		private Vector2 _blockBottomCenter;

		// Token: 0x04003825 RID: 14373
		public int ControlCode;

		// Token: 0x04003826 RID: 14374
		private static readonly Vector2 BlockShadowOffset = new Vector2(0f, -10f);

		// Token: 0x04003827 RID: 14375
		private static readonly Vector2 BlockBaseOffset = new Vector2(0f, -15f);

		// Token: 0x04003828 RID: 14376
		private static readonly Vector2 BlockBaseShadowOffset = new Vector2(0f, -214f);

		// Token: 0x04003829 RID: 14377
		public static readonly float BlockBorderOffsetVertical = 30f;

		// Token: 0x0400382A RID: 14378
		public static readonly float BlockBorderOffsetHorizontal = 58f;

		// Token: 0x0400382B RID: 14379
		public static readonly float BlockBorderWidth = 5f;

		// Token: 0x0400382C RID: 14380
		public static readonly float BlockBorderWidthRate = 1.93f;

		// Token: 0x0400382D RID: 14381
		private static readonly Color32 VisibleColor = new Color32(90, 90, 90, byte.MaxValue);

		// Token: 0x0400382E RID: 14382
		private static readonly Color32 InVisibleColor = Color.clear;

		// Token: 0x0400382F RID: 14383
		private static readonly Color32 InSightColor = new Color32(217, 217, 217, byte.MaxValue);

		// Token: 0x04003830 RID: 14384
		private static readonly int DarkVisibleBrightnessDelta = -150;

		// Token: 0x04003832 RID: 14386
		private MapBlockRenderInfo.EMapBlockState _previousState;

		// Token: 0x04003837 RID: 14391
		public List<MapBlockEffect> BlockEffectList;

		// Token: 0x04003838 RID: 14392
		private bool[] _vertexDirtyFlags = new bool[5];

		// Token: 0x04003839 RID: 14393
		private ValueTuple<Sprite, sbyte> _showingSpriteInfo;

		// Token: 0x0400383A RID: 14394
		private UIVertex[] _blockVertexData;

		// Token: 0x0400383B RID: 14395
		private UIVertex[] _blockShadowVertexData;

		// Token: 0x0400383C RID: 14396
		private UIVertex[] _blockPreparationVertexData;

		// Token: 0x0400383D RID: 14397
		private UIVertex[] _areaBorderVertexData;

		// Token: 0x0400383E RID: 14398
		private UIVertex[] _areaBorderCornerVertexData;

		// Token: 0x0400383F RID: 14399
		private UIVertex[] _blockBaseVertexData;

		// Token: 0x04003840 RID: 14400
		private UIVertex[] _areaBlockBaseShadowVertexData;

		// Token: 0x02001B09 RID: 6921
		public enum EMapBlockState
		{
			// Token: 0x0400B7D1 RID: 47057
			Empty,
			// Token: 0x0400B7D2 RID: 47058
			Invisible,
			// Token: 0x0400B7D3 RID: 47059
			Preparation,
			// Token: 0x0400B7D4 RID: 47060
			DarkVisible,
			// Token: 0x0400B7D5 RID: 47061
			NegativeFilm,
			// Token: 0x0400B7D6 RID: 47062
			Visible,
			// Token: 0x0400B7D7 RID: 47063
			InSight,
			// Token: 0x0400B7D8 RID: 47064
			InFlame
		}
	}
}
