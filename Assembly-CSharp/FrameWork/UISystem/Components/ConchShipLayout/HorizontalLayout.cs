using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.UISystem.Components.ConchShipLayout
{
	// Token: 0x0200102F RID: 4143
	public class HorizontalLayout : MonoBehaviour
	{
		// Token: 0x0600BD5E RID: 48478 RVA: 0x005607F9 File Offset: 0x0055E9F9
		private void Awake()
		{
			this._rectTransform = (base.transform as RectTransform);
		}

		// Token: 0x0600BD5F RID: 48479 RVA: 0x00560810 File Offset: 0x0055EA10
		private void Start()
		{
			bool flag = null != this._rectTransform;
			if (flag)
			{
				this._rectTransform.SetPivot(this._rectTransform.pivot.SetX(0f));
			}
		}

		// Token: 0x0600BD60 RID: 48480 RVA: 0x00560850 File Offset: 0x0055EA50
		public void LayoutHorizontal(bool anim = false)
		{
			float animDuration = 0.3f;
			bool flag = null == this.HeadItem;
			if (!flag)
			{
				this.CheckValidate();
				float horizontalSize = (float)this.Padding.left;
				LayoutItem item = this.HeadItem;
				while (item != null)
				{
					float x = horizontalSize + item.PivotWidth;
					if (anim)
					{
						item.AnimToX(x, animDuration, null);
					}
					else
					{
						item.SetX(x);
					}
					horizontalSize += item.LayoutWidth + this.Spacing;
					item = item.GetNextItemForLayout();
				}
				horizontalSize += (float)this.Padding.right;
				bool autoAdaptWidth = this.AutoAdaptWidth;
				if (autoAdaptWidth)
				{
					this._rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, horizontalSize);
				}
				bool flag2 = this._onLayoutFinished != null;
				if (flag2)
				{
					SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(animDuration, this._onLayoutFinished);
				}
			}
		}

		// Token: 0x0600BD61 RID: 48481 RVA: 0x0056092D File Offset: 0x0055EB2D
		public void SetLayoutFinishedEvent(Action action)
		{
			this._onLayoutFinished = action;
		}

		// Token: 0x0600BD62 RID: 48482 RVA: 0x00560938 File Offset: 0x0055EB38
		public void InsertItemAtIndex(LayoutItem item, int index)
		{
			LayoutItem prevItem = this.HeadItem;
			int indexForCount = 0;
			while (prevItem != null && indexForCount < index)
			{
				prevItem = prevItem.GetNextItemForLayout();
			}
			bool flag = null == prevItem;
			if (!flag)
			{
				item.NextItem = prevItem.NextItem;
				item.PrevItem = prevItem;
				prevItem.NextItem = item;
				item.AnimToX(this.GetItemProperX(item), 0.3f, null);
				item.NextItem.AnimToX(this.GetItemProperX(item.NextItem), 0.3f, delegate
				{
					this.SetGroupPosBaseOnItem(item.NextItem);
				});
			}
		}

		// Token: 0x0600BD63 RID: 48483 RVA: 0x00560A0C File Offset: 0x0055EC0C
		public void RemoveItemFromGroup(LayoutItem item, bool autoAlign)
		{
			bool flag = null == item;
			if (!flag)
			{
				LayoutItem prevItem = item.PrevItem;
				LayoutItem nextItem = item.NextItem;
				bool flag2 = null != prevItem;
				if (flag2)
				{
					prevItem.NextItem = nextItem;
				}
				bool flag3 = null != nextItem;
				if (flag3)
				{
					nextItem.PrevItem = prevItem;
				}
				item.PrevItem = null;
				item.NextItem = null;
				bool flag4 = autoAlign && null != nextItem;
				if (flag4)
				{
					nextItem.AnimToX(this.GetItemProperX(nextItem), 0.3f, delegate
					{
						this.SetGroupPosBaseOnItem(nextItem);
					});
				}
			}
		}

		// Token: 0x0600BD64 RID: 48484 RVA: 0x00560AD0 File Offset: 0x0055ECD0
		public float GetItemProperX(LayoutItem target)
		{
			bool flag = null == this.HeadItem;
			float result;
			if (flag)
			{
				result = -1f;
			}
			else
			{
				this.CheckValidate();
				float horizontalSize = (float)this.Padding.left;
				LayoutItem item = this.HeadItem;
				while (item != null)
				{
					float x = horizontalSize + item.PivotWidth;
					bool flag2 = item == target;
					if (flag2)
					{
						return x;
					}
					horizontalSize += item.LayoutWidth + this.Spacing;
					item = item.GetNextItemForLayout();
				}
				result = -1f;
			}
			return result;
		}

		// Token: 0x0600BD65 RID: 48485 RVA: 0x00560B60 File Offset: 0x0055ED60
		private void SetGroupPosBaseOnItem(LayoutItem item)
		{
			bool flag = item == null;
			if (!flag)
			{
				float x = item.RectTransform.anchoredPosition.x;
				x += item.LayoutWidth - item.PivotWidth + this.Spacing;
				item = item.GetNextItemForLayout();
				while (item != null)
				{
					item.SetX(x + item.PivotWidth);
					x += item.LayoutWidth + this.Spacing;
					item = item.GetNextItemForLayout();
				}
			}
		}

		// Token: 0x0600BD66 RID: 48486 RVA: 0x00560BE4 File Offset: 0x0055EDE4
		private void CheckValidate()
		{
			bool flag = null == this.HeadItem;
			if (!flag)
			{
				List<LayoutItem> itemList = new List<LayoutItem>(base.transform.childCount);
				LayoutItem item = this.HeadItem;
				while (null != item)
				{
					itemList.Add(item);
					item = item.GetNextItemForLayout();
					bool flag2 = itemList.Contains(item);
					if (flag2)
					{
						throw new Exception(base.name + ":HorizontalLayout error:loop layout");
					}
				}
			}
		}

		// Token: 0x040091C2 RID: 37314
		public RectOffset Padding;

		// Token: 0x040091C3 RID: 37315
		public float Spacing;

		// Token: 0x040091C4 RID: 37316
		public bool AutoAdaptWidth;

		// Token: 0x040091C5 RID: 37317
		public LayoutItem HeadItem;

		// Token: 0x040091C6 RID: 37318
		private RectTransform _rectTransform;

		// Token: 0x040091C7 RID: 37319
		private Action _onLayoutFinished;
	}
}
