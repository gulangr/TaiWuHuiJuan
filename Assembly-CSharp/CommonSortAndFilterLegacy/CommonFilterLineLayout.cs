using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000424 RID: 1060
	public class CommonFilterLineLayout : MonoBehaviour
	{
		// Token: 0x17000664 RID: 1636
		// (get) Token: 0x06003EEA RID: 16106 RVA: 0x001F784B File Offset: 0x001F5A4B
		public int ChildCount
		{
			get
			{
				return this.childList.Count;
			}
		}

		// Token: 0x06003EEB RID: 16107 RVA: 0x001F7858 File Offset: 0x001F5A58
		public void Init(float totalWidth)
		{
			bool inited = this._inited;
			if (!inited)
			{
				bool flag = this.Content == null;
				if (flag)
				{
					this.Content = base.GetComponent<HorizontalLayoutGroup>();
				}
				this.ContentRect = this.Content.GetComponent<RectTransform>();
				this.ContentRect.SetWidth(totalWidth);
				this._inited = true;
			}
		}

		// Token: 0x06003EEC RID: 16108 RVA: 0x001F78B8 File Offset: 0x001F5AB8
		public void AddChild(RectTransform child, bool headElement)
		{
			child.SetParent(this.Content.transform, false);
			this.childList.Add(child);
			if (headElement)
			{
				this.headWidth = child.rect.width;
			}
			else
			{
				child.SetWidth(this.ContentRect.rect.width - this.headWidth - this.Content.spacing);
			}
		}

		// Token: 0x04002D2E RID: 11566
		public HorizontalLayoutGroup Content;

		// Token: 0x04002D2F RID: 11567
		private RectTransform ContentRect;

		// Token: 0x04002D30 RID: 11568
		private List<RectTransform> childList = new List<RectTransform>();

		// Token: 0x04002D31 RID: 11569
		private bool _inited = false;

		// Token: 0x04002D32 RID: 11570
		private float headWidth = 0f;
	}
}
