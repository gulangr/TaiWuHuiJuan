using System;

namespace UnityEngine.UI
{
	// Token: 0x02000FAC RID: 4012
	[AddComponentMenu("UI/Loop Vertical Scroll Rect", 51)]
	[DisallowMultipleComponent]
	public class LoopVerticalScrollRect : LoopScrollRect
	{
		// Token: 0x0600B87D RID: 47229 RVA: 0x00541D0C File Offset: 0x0053FF0C
		protected LoopVerticalScrollRect()
		{
			this.direction = LoopScrollRectBase.LoopScrollRectDirection.Vertical;
		}

		// Token: 0x0600B87E RID: 47230 RVA: 0x00541D20 File Offset: 0x0053FF20
		protected override float GetSize(RectTransform item, bool includeSpacing)
		{
			float size = includeSpacing ? base.contentSpacing : 0f;
			bool flag = this.m_GridLayout != null;
			if (flag)
			{
				size += this.m_GridLayout.cellSize.y;
			}
			else
			{
				size += LoopScrollSizeUtils.GetPreferredHeight(item);
			}
			return size * this.m_Content.localScale.y;
		}

		// Token: 0x0600B87F RID: 47231 RVA: 0x00541D8C File Offset: 0x0053FF8C
		protected override float GetDimension(Vector2 vector)
		{
			return vector.y;
		}

		// Token: 0x0600B880 RID: 47232 RVA: 0x00541DA4 File Offset: 0x0053FFA4
		protected override float GetAbsDimension(Vector2 vector)
		{
			return vector.y;
		}

		// Token: 0x0600B881 RID: 47233 RVA: 0x00541DBC File Offset: 0x0053FFBC
		protected override Vector2 GetVector(float value)
		{
			return new Vector2(0f, value);
		}

		// Token: 0x0600B882 RID: 47234 RVA: 0x00541DDC File Offset: 0x0053FFDC
		protected override void Awake()
		{
			base.Awake();
			bool flag = this.m_Content;
			if (flag)
			{
				GridLayoutGroup layout = this.m_Content.GetComponent<GridLayoutGroup>();
				bool flag2 = layout != null && layout.constraint != GridLayoutGroup.Constraint.FixedColumnCount;
				if (flag2)
				{
					Debug.LogError("[LoopScrollRect] unsupported GridLayoutGroup constraint");
				}
			}
		}

		// Token: 0x0600B883 RID: 47235 RVA: 0x00541E38 File Offset: 0x00540038
		protected override bool UpdateItems(ref Bounds viewBounds, ref Bounds contentBounds)
		{
			bool changed = false;
			bool flag = viewBounds.size.y < contentBounds.min.y - viewBounds.max.y && this.itemTypeEnd > this.itemTypeStart;
			if (flag)
			{
				int maxItemTypeStart = -1;
				bool flag2 = this.totalCount >= 0;
				if (flag2)
				{
					maxItemTypeStart = Mathf.Max(0, this.totalCount - (this.itemTypeEnd - this.itemTypeStart));
				}
				float currentSize = contentBounds.size.y;
				float elementSize = base.EstimiateElementSize();
				this.ReturnToTempPool(true, this.itemTypeEnd - this.itemTypeStart);
				this.itemTypeStart = this.itemTypeEnd;
				int offsetCount = Mathf.FloorToInt((contentBounds.min.y - viewBounds.max.y) / (elementSize + base.contentSpacing));
				bool flag3 = maxItemTypeStart >= 0 && this.itemTypeStart + offsetCount * base.contentConstraintCount > maxItemTypeStart;
				if (flag3)
				{
					offsetCount = Mathf.FloorToInt((float)(maxItemTypeStart - this.itemTypeStart) / (float)base.contentConstraintCount);
				}
				this.itemTypeStart += offsetCount * base.contentConstraintCount;
				bool flag4 = this.totalCount >= 0;
				if (flag4)
				{
					this.itemTypeStart = Mathf.Max(this.itemTypeStart, 0);
				}
				this.itemTypeEnd = this.itemTypeStart;
				this.itemTypeSize = 0f;
				float offset = (float)offsetCount * (elementSize + base.contentSpacing);
				this.m_Content.anchoredPosition -= new Vector2(0f, offset + (this.reverseDirection ? 0f : currentSize));
				contentBounds.center -= new Vector3(0f, offset + currentSize / 2f, 0f);
				contentBounds.size = Vector3.zero;
				changed = true;
			}
			bool flag5 = viewBounds.min.y - contentBounds.max.y > viewBounds.size.y && this.itemTypeEnd > this.itemTypeStart;
			if (flag5)
			{
				float currentSize2 = contentBounds.size.y;
				float elementSize2 = base.EstimiateElementSize();
				this.ReturnToTempPool(false, this.itemTypeEnd - this.itemTypeStart);
				this.itemTypeEnd = this.itemTypeStart;
				int offsetCount2 = Mathf.FloorToInt((viewBounds.min.y - contentBounds.max.y) / (elementSize2 + base.contentSpacing));
				bool flag6 = this.totalCount >= 0 && this.itemTypeStart - offsetCount2 * base.contentConstraintCount < 0;
				if (flag6)
				{
					offsetCount2 = Mathf.FloorToInt((float)this.itemTypeStart / (float)base.contentConstraintCount);
				}
				this.itemTypeStart -= offsetCount2 * base.contentConstraintCount;
				bool flag7 = this.totalCount >= 0;
				if (flag7)
				{
					this.itemTypeStart = Mathf.Max(this.itemTypeStart, 0);
				}
				this.itemTypeEnd = this.itemTypeStart;
				this.itemTypeSize = 0f;
				float offset2 = (float)offsetCount2 * (elementSize2 + base.contentSpacing);
				this.m_Content.anchoredPosition += new Vector2(0f, offset2 + (this.reverseDirection ? currentSize2 : 0f));
				contentBounds.center += new Vector3(0f, offset2 + currentSize2 / 2f, 0f);
				contentBounds.size = Vector3.zero;
				changed = true;
			}
			bool flag8 = viewBounds.min.y < contentBounds.min.y + this.m_ContentBottomPadding;
			if (flag8)
			{
				float size = base.NewItemAtEnd();
				float totalSize = size;
				while (size > 0f && viewBounds.min.y < contentBounds.min.y + this.m_ContentBottomPadding - totalSize)
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
			bool flag11 = viewBounds.max.y > contentBounds.max.y - this.m_ContentTopPadding;
			if (flag11)
			{
				float size2 = base.NewItemAtStart();
				float totalSize2 = size2;
				while (size2 > 0f && viewBounds.max.y > contentBounds.max.y - this.m_ContentTopPadding + totalSize2)
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
			bool flag13 = viewBounds.min.y > contentBounds.min.y + this.threshold + this.m_ContentBottomPadding && viewBounds.size.y < contentBounds.size.y - this.threshold;
			if (flag13)
			{
				float size3 = base.DeleteItemAtEnd();
				float totalSize3 = size3;
				while (size3 > 0f && viewBounds.min.y > contentBounds.min.y + this.threshold + this.m_ContentBottomPadding + totalSize3)
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
			bool flag15 = viewBounds.max.y < contentBounds.max.y - this.threshold - this.m_ContentTopPadding && viewBounds.size.y < contentBounds.size.y - this.threshold;
			if (flag15)
			{
				float size4 = base.DeleteItemAtStart();
				float totalSize4 = size4;
				while (size4 > 0f && viewBounds.max.y < contentBounds.max.y - this.threshold - this.m_ContentTopPadding - totalSize4)
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
