using System;
using GameData.Domains.Taiwu;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B97 RID: 2967
	public class SkillBreakTriangleGrid
	{
		// Token: 0x17000FCF RID: 4047
		// (get) Token: 0x06009258 RID: 37464 RVA: 0x00442FD8 File Offset: 0x004411D8
		// (set) Token: 0x06009259 RID: 37465 RVA: 0x00442FE0 File Offset: 0x004411E0
		public int Width { get; private set; }

		// Token: 0x17000FD0 RID: 4048
		// (get) Token: 0x0600925A RID: 37466 RVA: 0x00442FE9 File Offset: 0x004411E9
		// (set) Token: 0x0600925B RID: 37467 RVA: 0x00442FF1 File Offset: 0x004411F1
		public int Height { get; private set; }

		// Token: 0x17000FD1 RID: 4049
		// (get) Token: 0x0600925C RID: 37468 RVA: 0x00442FFA File Offset: 0x004411FA
		// (set) Token: 0x0600925D RID: 37469 RVA: 0x00443002 File Offset: 0x00441202
		public float TriangleBase { get; private set; }

		// Token: 0x17000FD2 RID: 4050
		// (get) Token: 0x0600925E RID: 37470 RVA: 0x0044300B File Offset: 0x0044120B
		// (set) Token: 0x0600925F RID: 37471 RVA: 0x00443013 File Offset: 0x00441213
		public float TriangleHeight { get; private set; }

		// Token: 0x06009260 RID: 37472 RVA: 0x0044301C File Offset: 0x0044121C
		public SkillBreakTriangleGrid(int width, int height, float triangleBase, float triangleHeight)
		{
			this.Width = width;
			this.Height = height;
			this.TriangleBase = triangleBase;
			this.TriangleHeight = triangleHeight;
		}

		// Token: 0x17000FD3 RID: 4051
		// (get) Token: 0x06009261 RID: 37473 RVA: 0x00443047 File Offset: 0x00441247
		public float AreaWidth
		{
			get
			{
				return (float)(this.Width - 1) * this.TriangleBase;
			}
		}

		// Token: 0x17000FD4 RID: 4052
		// (get) Token: 0x06009262 RID: 37474 RVA: 0x00443059 File Offset: 0x00441259
		public float AreaHeight
		{
			get
			{
				return (float)(this.Height - 1) * this.TriangleHeight;
			}
		}

		// Token: 0x17000FD5 RID: 4053
		// (get) Token: 0x06009263 RID: 37475 RVA: 0x0044306B File Offset: 0x0044126B
		public Vector2 AreaSize
		{
			get
			{
				return new Vector2(this.AreaWidth, this.AreaHeight);
			}
		}

		// Token: 0x06009264 RID: 37476 RVA: 0x00443080 File Offset: 0x00441280
		public bool IsValidPoint(int x, int y)
		{
			bool flag = x < 0 || x >= this.Width || y < 0 || y >= this.Height;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = y % 2 == 0 && x == this.Width - 1;
				result = !flag2;
			}
			return result;
		}

		// Token: 0x06009265 RID: 37477 RVA: 0x004430D8 File Offset: 0x004412D8
		public Vector2 GetPointPosition(int x, int y)
		{
			bool flag = !this.IsValidPoint(x, y);
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("x:{0}, y:{1} is not valid", x, y));
			}
			float xOffset = SkillBreakPlate.IsProtrusion(y) ? 0f : 0.5f;
			float posX = ((float)x + xOffset) * this.TriangleBase;
			float posY = (float)y * this.TriangleHeight;
			return new Vector2(posX, posY);
		}

		// Token: 0x06009266 RID: 37478 RVA: 0x0044314C File Offset: 0x0044134C
		public float GetRotationZ(int x0, int y0, int x1, int y1)
		{
			Vector2 pos0 = this.GetPointPosition(x0, y0);
			Vector2 pos = this.GetPointPosition(x1, y1);
			Vector2 dir = pos - pos0;
			return Vector2.SignedAngle(Vector2.right, dir);
		}
	}
}
