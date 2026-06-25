using System;
using UnityEngine;

namespace Game.Components.Common
{
	// Token: 0x02000F93 RID: 3987
	public class PageSwitchHorizontalNormalHeadController : PageSwitchHorizontalController
	{
		// Token: 0x0600B75F RID: 46943 RVA: 0x0053950F File Offset: 0x0053770F
		protected override void SetupHeadElementRect(RectTransform itemRectTrans, float width)
		{
			itemRectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, this.Padding, width);
			itemRectTrans.SetAnchor(Vector2.one * 0.5f, Vector2.one * 0.5f);
		}

		// Token: 0x0600B760 RID: 46944 RVA: 0x00539548 File Offset: 0x00537748
		protected override void Scroll(float deltaX)
		{
			Refers firstCell = base.GetPageItem(0);
			Refers lastCell = base.GetPageItem(base.PageCount - 1);
			bool ignoreReuse = null != firstCell && null != lastCell;
			for (int i = this._pageItemList.Count - 1; i >= 0; i--)
			{
				RectTransform itemRectTrans = this._pageItemList[i].transform as RectTransform;
				itemRectTrans.anchoredPosition = itemRectTrans.anchoredPosition.SetX(itemRectTrans.anchoredPosition.x + deltaX);
				bool flag = ignoreReuse;
				if (!flag)
				{
					bool flag2 = itemRectTrans.position.x < this.HideDotLeft.position.x || itemRectTrans.position.x > this.HideDotRight.position.x;
					if (flag2)
					{
						bool flag3 = this._pageItemList.Count <= 1;
						if (flag3)
						{
							break;
						}
						this._pageItemList.RemoveAt(i);
						this._itemPool.DestroyObject(itemRectTrans.gameObject);
						itemRectTrans.name = "poolItem";
					}
				}
			}
			bool flag4 = !ignoreReuse;
			if (flag4)
			{
				this.FillViewPort();
			}
		}
	}
}
