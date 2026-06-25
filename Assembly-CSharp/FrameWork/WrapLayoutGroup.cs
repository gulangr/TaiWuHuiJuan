using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork
{
	// Token: 0x02000FEA RID: 4074
	public class WrapLayoutGroup : LayoutGroup
	{
		// Token: 0x170014F6 RID: 5366
		// (get) Token: 0x0600B9FA RID: 47610 RVA: 0x0054B7E8 File Offset: 0x005499E8
		// (set) Token: 0x0600B9FB RID: 47611 RVA: 0x0054B800 File Offset: 0x00549A00
		public bool UseFixedCellWidth
		{
			get
			{
				return this.useFixedCellWidth;
			}
			set
			{
				base.SetProperty<bool>(ref this.useFixedCellWidth, value);
			}
		}

		// Token: 0x170014F7 RID: 5367
		// (get) Token: 0x0600B9FC RID: 47612 RVA: 0x0054B814 File Offset: 0x00549A14
		// (set) Token: 0x0600B9FD RID: 47613 RVA: 0x0054B82C File Offset: 0x00549A2C
		public float CellWidth
		{
			get
			{
				return this.cellWidth;
			}
			set
			{
				base.SetProperty<float>(ref this.cellWidth, value);
			}
		}

		// Token: 0x170014F8 RID: 5368
		// (get) Token: 0x0600B9FE RID: 47614 RVA: 0x0054B840 File Offset: 0x00549A40
		// (set) Token: 0x0600B9FF RID: 47615 RVA: 0x0054B858 File Offset: 0x00549A58
		public float CellHeight
		{
			get
			{
				return this.cellHeight;
			}
			set
			{
				base.SetProperty<float>(ref this.cellHeight, value);
			}
		}

		// Token: 0x170014F9 RID: 5369
		// (get) Token: 0x0600BA00 RID: 47616 RVA: 0x0054B86C File Offset: 0x00549A6C
		// (set) Token: 0x0600BA01 RID: 47617 RVA: 0x0054B884 File Offset: 0x00549A84
		public float MinCellWidth
		{
			get
			{
				return this.minCellWidth;
			}
			set
			{
				base.SetProperty<float>(ref this.minCellWidth, value);
			}
		}

		// Token: 0x170014FA RID: 5370
		// (get) Token: 0x0600BA02 RID: 47618 RVA: 0x0054B898 File Offset: 0x00549A98
		// (set) Token: 0x0600BA03 RID: 47619 RVA: 0x0054B8B0 File Offset: 0x00549AB0
		public float MaxCellWidth
		{
			get
			{
				return this.maxCellWidth;
			}
			set
			{
				base.SetProperty<float>(ref this.maxCellWidth, value);
			}
		}

		// Token: 0x170014FB RID: 5371
		// (get) Token: 0x0600BA04 RID: 47620 RVA: 0x0054B8C4 File Offset: 0x00549AC4
		// (set) Token: 0x0600BA05 RID: 47621 RVA: 0x0054B8DC File Offset: 0x00549ADC
		public float SpacingX
		{
			get
			{
				return this.spacingX;
			}
			set
			{
				base.SetProperty<float>(ref this.spacingX, value);
			}
		}

		// Token: 0x170014FC RID: 5372
		// (get) Token: 0x0600BA06 RID: 47622 RVA: 0x0054B8F0 File Offset: 0x00549AF0
		// (set) Token: 0x0600BA07 RID: 47623 RVA: 0x0054B908 File Offset: 0x00549B08
		public float SpacingY
		{
			get
			{
				return this.spacingY;
			}
			set
			{
				base.SetProperty<float>(ref this.spacingY, value);
			}
		}

		// Token: 0x0600BA08 RID: 47624 RVA: 0x0054B91C File Offset: 0x00549B1C
		public override void CalculateLayoutInputHorizontal()
		{
			base.CalculateLayoutInputHorizontal();
			float totalWidth = this.CalculateLayout();
			base.SetLayoutInputForAxis(totalWidth, totalWidth, -1f, 0);
		}

		// Token: 0x0600BA09 RID: 47625 RVA: 0x0054B948 File Offset: 0x00549B48
		public override void CalculateLayoutInputVertical()
		{
			int rowCount = this.rowStartIndices.Count;
			float totalHeight = (float)(base.padding.top + base.padding.bottom) - this.spacingY + (float)rowCount * (this.cellHeight + this.spacingY);
			base.SetLayoutInputForAxis(totalHeight, totalHeight, -1f, 1);
		}

		// Token: 0x0600BA0A RID: 47626 RVA: 0x0054B9A2 File Offset: 0x00549BA2
		public override void SetLayoutHorizontal()
		{
			this.LayoutChildren();
		}

		// Token: 0x0600BA0B RID: 47627 RVA: 0x0054B9AC File Offset: 0x00549BAC
		public override void SetLayoutVertical()
		{
		}

		// Token: 0x0600BA0C RID: 47628 RVA: 0x0054B9B0 File Offset: 0x00549BB0
		private float CalculateLayout()
		{
			this.rowStartIndices.Clear();
			bool flag = base.rectChildren.Count == 0;
			float result;
			if (flag)
			{
				result = (float)(base.padding.left + base.padding.right);
			}
			else
			{
				this.rowStartIndices.Add(0);
				float containerWidth = base.rectTransform.rect.width - (float)base.padding.left - (float)base.padding.right;
				float currentRowWidth = 0f;
				for (int i = 0; i < base.rectChildren.Count; i++)
				{
					RectTransform child = base.rectChildren[i];
					float cellW = this.GetCellWidth(child);
					float neededWidth = (currentRowWidth > 0f) ? (this.spacingX + cellW) : cellW;
					bool flag2 = currentRowWidth + neededWidth > containerWidth && currentRowWidth > 0f;
					if (flag2)
					{
						this.rowStartIndices.Add(i);
						currentRowWidth = cellW;
					}
					else
					{
						currentRowWidth += neededWidth;
					}
				}
				result = (float)(base.padding.left + base.padding.right) + currentRowWidth;
			}
			return result;
		}

		// Token: 0x0600BA0D RID: 47629 RVA: 0x0054BAE4 File Offset: 0x00549CE4
		private float GetCellWidth(RectTransform child)
		{
			bool flag = this.useFixedCellWidth;
			float result;
			if (flag)
			{
				result = this.cellWidth;
			}
			else
			{
				float preferredWidth = LayoutUtility.GetPreferredWidth(child);
				result = Mathf.Clamp(preferredWidth, this.minCellWidth, this.maxCellWidth);
			}
			return result;
		}

		// Token: 0x0600BA0E RID: 47630 RVA: 0x0054BB24 File Offset: 0x00549D24
		private void LayoutChildren()
		{
			bool flag = this.rowStartIndices.Count == 0;
			if (!flag)
			{
				float containerWidth = base.rectTransform.rect.width - (float)base.padding.left - (float)base.padding.right;
				float y = (float)base.padding.top;
				for (int rowIdx = 0; rowIdx < this.rowStartIndices.Count; rowIdx++)
				{
					int startIndex = this.rowStartIndices[rowIdx];
					int endIndex = (rowIdx + 1 < this.rowStartIndices.Count) ? this.rowStartIndices[rowIdx + 1] : base.rectChildren.Count;
					float rowWidth = 0f;
					for (int i = startIndex; i < endIndex; i++)
					{
						bool flag2 = i > startIndex;
						if (flag2)
						{
							rowWidth += this.spacingX;
						}
						rowWidth += this.GetCellWidth(base.rectChildren[i]);
					}
					float alignmentOffset = this.GetAlignmentOffset(containerWidth, rowWidth);
					float x = (float)base.padding.left + alignmentOffset;
					for (int j = startIndex; j < endIndex; j++)
					{
						RectTransform child = base.rectChildren[j];
						float cellW = this.GetCellWidth(child);
						child.anchoredPosition = new Vector2(x + cellW * 0.5f, -y - this.cellHeight * 0.5f);
						child.sizeDelta = new Vector2(cellW, this.cellHeight);
						child.anchorMin = new Vector2(0f, 1f);
						child.anchorMax = new Vector2(0f, 1f);
						child.pivot = new Vector2(0.5f, 0.5f);
						x += cellW + this.spacingX;
					}
					y += this.cellHeight + this.spacingY;
				}
			}
		}

		// Token: 0x0600BA0F RID: 47631 RVA: 0x0054BD34 File Offset: 0x00549F34
		private float GetAlignmentOffset(float containerWidth, float rowWidth)
		{
			switch (base.childAlignment)
			{
			case TextAnchor.UpperCenter:
			case TextAnchor.MiddleCenter:
			case TextAnchor.LowerCenter:
				return (containerWidth - rowWidth) * 0.5f;
			case TextAnchor.UpperRight:
			case TextAnchor.MiddleRight:
			case TextAnchor.LowerRight:
				return containerWidth - rowWidth;
			}
			return 0f;
		}

		// Token: 0x04008FD3 RID: 36819
		[Header("Cell Settings")]
		[SerializeField]
		private bool useFixedCellWidth;

		// Token: 0x04008FD4 RID: 36820
		[SerializeField]
		private float cellWidth = 100f;

		// Token: 0x04008FD5 RID: 36821
		[SerializeField]
		private float cellHeight = 30f;

		// Token: 0x04008FD6 RID: 36822
		[SerializeField]
		private float minCellWidth = 50f;

		// Token: 0x04008FD7 RID: 36823
		[SerializeField]
		private float maxCellWidth = 200f;

		// Token: 0x04008FD8 RID: 36824
		[Header("Layout Settings")]
		[SerializeField]
		private float spacingX = 4f;

		// Token: 0x04008FD9 RID: 36825
		[SerializeField]
		private float spacingY = 4f;

		// Token: 0x04008FDA RID: 36826
		private readonly List<int> rowStartIndices = new List<int>();
	}
}
