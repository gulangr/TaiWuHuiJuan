using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.UISystem.Components.ConchShipLayout
{
	// Token: 0x0200102E RID: 4142
	public class DynamicCellSizeGridLayoutGroup : LayoutGroup
	{
		// Token: 0x0600BD58 RID: 48472 RVA: 0x00560544 File Offset: 0x0055E744
		public override void CalculateLayoutInputHorizontal()
		{
			RectTransform containerRectTrans = base.transform as RectTransform;
			if (this._cacheResultMap == null)
			{
				this._cacheResultMap = new Dictionary<int, ValueTuple<int, float, Vector2>>();
			}
			this._cacheResultMap.Clear();
			float lineWidth = (float)(base.padding.left + base.padding.right);
			this._lineHeight = 0f;
			int lineIndex = 0;
			int i = 0;
			int max = base.transform.childCount;
			while (i < max)
			{
				RectTransform child = base.transform.GetChild(i) as RectTransform;
				bool flag = null == child || !child.gameObject.activeSelf;
				if (!flag)
				{
					bool flag2 = lineWidth + child.rect.width > containerRectTrans.rect.width;
					if (flag2)
					{
						lineIndex++;
						lineWidth = (float)(base.padding.left + base.padding.right);
					}
					this._lineHeight = Mathf.Max(this._lineHeight, child.rect.height);
					Vector2 size = new Vector2(LayoutUtility.GetPreferredSize(child, 0), LayoutUtility.GetPreferredSize(child, 1));
					this._cacheResultMap.Add(child.GetInstanceID(), new ValueTuple<int, float, Vector2>(lineIndex, lineWidth, size));
					lineWidth += child.rect.width + this.Spacing.x;
				}
				i++;
			}
			float minSpace = (float)(base.padding.top + base.padding.bottom) + (float)(lineIndex + 1) * this._lineHeight + (float)lineIndex * this.Spacing.y;
			base.SetLayoutInputForAxis(minSpace, minSpace, -1f, 1);
		}

		// Token: 0x0600BD59 RID: 48473 RVA: 0x00560708 File Offset: 0x0055E908
		public override void CalculateLayoutInputVertical()
		{
		}

		// Token: 0x0600BD5A RID: 48474 RVA: 0x0056070B File Offset: 0x0055E90B
		public override void SetLayoutHorizontal()
		{
			this.LayoutAllChildren();
		}

		// Token: 0x0600BD5B RID: 48475 RVA: 0x00560715 File Offset: 0x0055E915
		public override void SetLayoutVertical()
		{
		}

		// Token: 0x0600BD5C RID: 48476 RVA: 0x00560718 File Offset: 0x0055E918
		private void LayoutAllChildren()
		{
			int i = 0;
			int max = base.transform.childCount;
			while (i < max)
			{
				RectTransform child = base.transform.GetChild(i) as RectTransform;
				bool flag = null == child || !child.gameObject.activeSelf;
				if (!flag)
				{
					ValueTuple<int, float, Vector2> data;
					bool flag2 = this._cacheResultMap.TryGetValue(child.GetInstanceID(), out data);
					if (flag2)
					{
						base.SetChildAlongAxis(child, 0, data.Item2, data.Item3.x);
						base.SetChildAlongAxis(child, 1, (float)base.padding.top + (float)data.Item1 * (this._lineHeight + this.Spacing.y), data.Item3.y);
					}
				}
				i++;
			}
		}

		// Token: 0x040091BF RID: 37311
		[TupleElementNames(new string[]
		{
			"line",
			"pos",
			"size"
		})]
		private Dictionary<int, ValueTuple<int, float, Vector2>> _cacheResultMap;

		// Token: 0x040091C0 RID: 37312
		private float _lineHeight;

		// Token: 0x040091C1 RID: 37313
		public Vector2 Spacing;
	}
}
