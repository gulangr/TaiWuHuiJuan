using System;
using System.Collections.Generic;

namespace Game.Views.SectInteract
{
	// Token: 0x020009AA RID: 2474
	public class MaxRectangleFinder
	{
		// Token: 0x0600777F RID: 30591 RVA: 0x00379B4C File Offset: 0x00377D4C
		public static MaxRectangleFinder.RectangleInfo CalculateMaxRectangleArea(EBoardGridState[,] matrix)
		{
			int rows = matrix.GetLength(0);
			int cols = matrix.GetLength(1);
			int[] heights = new int[cols];
			MaxRectangleFinder.RectangleInfo maxRect = new MaxRectangleFinder.RectangleInfo();
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < cols; j++)
				{
					heights[j] = ((matrix[i, j] == EBoardGridState.All) ? (heights[j] + 1) : 0);
				}
				MaxRectangleFinder.RectangleInfo histResult = MaxRectangleFinder.LargestRectangleInHistogram(heights);
				bool flag = histResult.Area > maxRect.Area;
				if (flag)
				{
					maxRect.Area = histResult.Area;
					maxRect.Top = i - histResult.Height + 1;
					maxRect.Left = histResult.Left;
					maxRect.Width = histResult.Width;
					maxRect.Height = histResult.Height;
				}
			}
			return maxRect;
		}

		// Token: 0x06007780 RID: 30592 RVA: 0x00379C34 File Offset: 0x00377E34
		private static MaxRectangleFinder.RectangleInfo LargestRectangleInHistogram(int[] heights)
		{
			Stack<int> stack = new Stack<int>();
			stack.Push(-1);
			MaxRectangleFinder.RectangleInfo maxInfo = new MaxRectangleFinder.RectangleInfo();
			for (int i = 0; i < heights.Length; i++)
			{
				while (stack.Peek() != -1 && heights[stack.Peek()] >= heights[i])
				{
					int height = heights[stack.Pop()];
					int width = i - stack.Peek() - 1;
					int area = height * width;
					bool flag = area > maxInfo.Area;
					if (flag)
					{
						maxInfo.Area = area;
						maxInfo.Left = stack.Peek() + 1;
						maxInfo.Width = width;
						maxInfo.Height = height;
					}
				}
				stack.Push(i);
			}
			while (stack.Peek() != -1)
			{
				int height2 = heights[stack.Pop()];
				int width2 = heights.Length - stack.Peek() - 1;
				int area2 = height2 * width2;
				bool flag2 = area2 > maxInfo.Area;
				if (flag2)
				{
					maxInfo.Area = area2;
					maxInfo.Left = stack.Peek() + 1;
					maxInfo.Width = width2;
					maxInfo.Height = height2;
				}
			}
			return maxInfo;
		}

		// Token: 0x02001EDA RID: 7898
		public class RectangleInfo
		{
			// Token: 0x0600F1F3 RID: 61939 RVA: 0x00617D50 File Offset: 0x00615F50
			public override string ToString()
			{
				return string.Format("Area: {0}, Top: {1}, Left: {2}, Width: {3}, Height: {4}", new object[]
				{
					this.Area,
					this.Top,
					this.Left,
					this.Width,
					this.Height
				});
			}

			// Token: 0x0400CB46 RID: 52038
			public int Area;

			// Token: 0x0400CB47 RID: 52039
			public int Top;

			// Token: 0x0400CB48 RID: 52040
			public int Left;

			// Token: 0x0400CB49 RID: 52041
			public int Width;

			// Token: 0x0400CB4A RID: 52042
			public int Height;
		}
	}
}
