using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000C9A RID: 3226
	public class FilterLineLayout : MonoBehaviour
	{
		// Token: 0x17001125 RID: 4389
		// (get) Token: 0x0600A430 RID: 42032 RVA: 0x004CB344 File Offset: 0x004C9544
		public int ChildCount
		{
			get
			{
				return this._childList.Count;
			}
		}

		// Token: 0x0600A431 RID: 42033 RVA: 0x004CB354 File Offset: 0x004C9554
		public void Init(float totalWidth)
		{
			bool initialized = this._initialized;
			if (!initialized)
			{
				bool flag = this.content == null;
				if (flag)
				{
					this.content = base.GetComponent<HorizontalLayoutGroup>();
				}
				this._contentRect = this.content.GetComponent<RectTransform>();
				this._contentRect.SetWidth(totalWidth);
				this._initialized = true;
			}
		}

		// Token: 0x0600A432 RID: 42034 RVA: 0x004CB3B4 File Offset: 0x004C95B4
		public void AddChild(RectTransform child, bool headElement)
		{
			child.SetParent(this.content.transform, false);
			this._childList.Add(child);
			if (headElement)
			{
				this._headWidth = child.rect.width;
			}
			else
			{
				child.SetWidth(this._contentRect.rect.width - this._headWidth - this.content.spacing);
			}
		}

		// Token: 0x040081FC RID: 33276
		[SerializeField]
		private HorizontalLayoutGroup content;

		// Token: 0x040081FD RID: 33277
		private RectTransform _contentRect;

		// Token: 0x040081FE RID: 33278
		private readonly List<RectTransform> _childList = new List<RectTransform>();

		// Token: 0x040081FF RID: 33279
		private bool _initialized;

		// Token: 0x04008200 RID: 33280
		private float _headWidth;
	}
}
