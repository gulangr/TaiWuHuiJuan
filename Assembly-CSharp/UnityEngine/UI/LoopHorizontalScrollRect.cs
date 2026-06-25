using System;

namespace UnityEngine.UI
{
	// Token: 0x02000FA5 RID: 4005
	[AddComponentMenu("UI/Loop Horizontal Scroll Rect", 50)]
	[DisallowMultipleComponent]
	public class LoopHorizontalScrollRect : LoopScrollRect
	{
		// Token: 0x0600B7CD RID: 47053 RVA: 0x0053C868 File Offset: 0x0053AA68
		protected LoopHorizontalScrollRect()
		{
			this.direction = LoopScrollRectBase.LoopScrollRectDirection.Horizontal;
		}

		// Token: 0x0600B7CE RID: 47054 RVA: 0x0053C87C File Offset: 0x0053AA7C
		protected override float GetSize(RectTransform item, bool includeSpacing)
		{
			float size = includeSpacing ? base.contentSpacing : 0f;
			bool flag = this.m_GridLayout != null;
			if (flag)
			{
				size += this.m_GridLayout.cellSize.x;
			}
			else
			{
				size += LoopScrollSizeUtils.GetPreferredWidth(item);
			}
			return size * this.m_Content.localScale.x;
		}

		// Token: 0x0600B7CF RID: 47055 RVA: 0x0053C8E8 File Offset: 0x0053AAE8
		protected override float GetDimension(Vector2 vector)
		{
			return -vector.x;
		}

		// Token: 0x0600B7D0 RID: 47056 RVA: 0x0053C904 File Offset: 0x0053AB04
		protected override float GetAbsDimension(Vector2 vector)
		{
			return vector.x;
		}

		// Token: 0x0600B7D1 RID: 47057 RVA: 0x0053C91C File Offset: 0x0053AB1C
		protected override Vector2 GetVector(float value)
		{
			return new Vector2(-value, 0f);
		}

		// Token: 0x0600B7D2 RID: 47058 RVA: 0x0053C93C File Offset: 0x0053AB3C
		protected override void Awake()
		{
			base.Awake();
			bool flag = this.m_Content;
			if (flag)
			{
				GridLayoutGroup layout = this.m_Content.GetComponent<GridLayoutGroup>();
				bool flag2 = layout != null && layout.constraint != GridLayoutGroup.Constraint.FixedRowCount;
				if (flag2)
				{
					Debug.LogError("[LoopScrollRect] unsupported GridLayoutGroup constraint");
				}
			}
		}

		// Token: 0x0600B7D3 RID: 47059 RVA: 0x0053C998 File Offset: 0x0053AB98
		protected override bool UpdateItems(ref Bounds viewBounds, ref Bounds contentBounds)
		{
			bool changed = false;
			bool flag = viewBounds.size.x < contentBounds.min.x - viewBounds.max.x && this.itemTypeEnd > this.itemTypeStart;
			if (flag)
			{
				float currentSize = contentBounds.size.x;
				float elementSize = base.EstimiateElementSize();
				this.ReturnToTempPool(false, this.itemTypeEnd - this.itemTypeStart);
				this.itemTypeEnd = this.itemTypeStart;
				int offsetCount = Mathf.FloorToInt((contentBounds.min.x - viewBounds.max.x) / (elementSize + base.contentSpacing));
				bool flag2 = this.totalCount >= 0 && this.itemTypeStart - offsetCount * base.contentConstraintCount < 0;
				if (flag2)
				{
					offsetCount = Mathf.FloorToInt((float)this.itemTypeStart / (float)base.contentConstraintCount);
				}
				this.itemTypeStart -= offsetCount * base.contentConstraintCount;
				bool flag3 = this.totalCount >= 0;
				if (flag3)
				{
					this.itemTypeStart = Mathf.Max(this.itemTypeStart, 0);
				}
				this.itemTypeEnd = this.itemTypeStart;
				this.itemTypeSize = 0f;
				float offset = (float)offsetCount * (elementSize + base.contentSpacing);
				this.m_Content.anchoredPosition -= new Vector2(offset + (this.reverseDirection ? currentSize : 0f), 0f);
				contentBounds.center -= new Vector3(offset + currentSize / 2f, 0f, 0f);
				contentBounds.size = Vector3.zero;
				changed = true;
			}
			bool flag4 = viewBounds.min.x - contentBounds.max.x > viewBounds.size.x && this.itemTypeEnd > this.itemTypeStart;
			if (flag4)
			{
				int maxItemTypeStart = -1;
				bool flag5 = this.totalCount >= 0;
				if (flag5)
				{
					maxItemTypeStart = Mathf.Max(0, this.totalCount - (this.itemTypeEnd - this.itemTypeStart));
					maxItemTypeStart = maxItemTypeStart / base.contentConstraintCount * base.contentConstraintCount;
				}
				float currentSize2 = contentBounds.size.x;
				float elementSize2 = base.EstimiateElementSize();
				this.ReturnToTempPool(true, this.itemTypeEnd - this.itemTypeStart);
				this.itemTypeStart = this.itemTypeEnd;
				int offsetCount2 = Mathf.FloorToInt((viewBounds.min.x - contentBounds.max.x) / (elementSize2 + base.contentSpacing));
				bool flag6 = maxItemTypeStart >= 0 && this.itemTypeStart + offsetCount2 * base.contentConstraintCount > maxItemTypeStart;
				if (flag6)
				{
					offsetCount2 = Mathf.FloorToInt((float)(maxItemTypeStart - this.itemTypeStart) / (float)base.contentConstraintCount);
				}
				this.itemTypeStart += offsetCount2 * base.contentConstraintCount;
				bool flag7 = this.totalCount >= 0;
				if (flag7)
				{
					this.itemTypeStart = Mathf.Max(this.itemTypeStart, 0);
				}
				this.itemTypeEnd = this.itemTypeStart;
				this.itemTypeSize = 0f;
				float offset2 = (float)offsetCount2 * (elementSize2 + base.contentSpacing);
				this.m_Content.anchoredPosition += new Vector2(offset2 + (this.reverseDirection ? 0f : currentSize2), 0f);
				contentBounds.center += new Vector3(offset2 + currentSize2 / 2f, 0f, 0f);
				contentBounds.size = Vector3.zero;
				changed = true;
			}
			bool flag8 = viewBounds.max.x > contentBounds.max.x - this.m_ContentRightPadding;
			if (flag8)
			{
				float size = base.NewItemAtEnd();
				float totalSize = size;
				while (size > 0f && viewBounds.max.x > contentBounds.max.x - this.m_ContentRightPadding + totalSize)
				{
					size = base.NewItemAtEnd();
					totalSize += size;
				}
				bool flag9 = totalSize > 0f;
				if (flag9)
				{
					changed = true;
				}
			}
			else
			{
				bool flag10 = this.itemTypeEnd % base.contentConstraintCount != 0 && (this.itemTypeEnd < this.totalCount || this.totalCount < 0);
				if (flag10)
				{
					base.NewItemAtEnd();
				}
			}
			bool flag11 = viewBounds.min.x < contentBounds.min.x + this.m_ContentLeftPadding;
			if (flag11)
			{
				float size2 = base.NewItemAtStart();
				float totalSize2 = size2;
				while (size2 > 0f && viewBounds.min.x < contentBounds.min.x + this.m_ContentLeftPadding - totalSize2)
				{
					size2 = base.NewItemAtStart();
					totalSize2 += size2;
				}
				bool flag12 = totalSize2 > 0f;
				if (flag12)
				{
					changed = true;
				}
			}
			bool flag13 = viewBounds.max.x < contentBounds.max.x - this.threshold - this.m_ContentRightPadding && viewBounds.size.x < contentBounds.size.x - this.threshold;
			if (flag13)
			{
				float size3 = base.DeleteItemAtEnd();
				float totalSize3 = size3;
				while (size3 > 0f && viewBounds.max.x < contentBounds.max.x - this.threshold - this.m_ContentRightPadding - totalSize3)
				{
					size3 = base.DeleteItemAtEnd();
					totalSize3 += size3;
				}
				bool flag14 = totalSize3 > 0f;
				if (flag14)
				{
					changed = true;
				}
			}
			bool flag15 = viewBounds.min.x > contentBounds.min.x + this.threshold + this.m_ContentLeftPadding && viewBounds.size.x < contentBounds.size.x - this.threshold;
			if (flag15)
			{
				float size4 = base.DeleteItemAtStart();
				float totalSize4 = size4;
				while (size4 > 0f && viewBounds.min.x > contentBounds.min.x + this.threshold + this.m_ContentLeftPadding + totalSize4)
				{
					size4 = base.DeleteItemAtStart();
					totalSize4 += size4;
				}
				bool flag16 = totalSize4 > 0f;
				if (flag16)
				{
					changed = true;
				}
			}
			bool flag17 = changed;
			if (flag17)
			{
				this.ClearTempPool();
			}
			return changed;
		}
	}
}
