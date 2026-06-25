using System;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B06 RID: 2822
	public class CombatDistanceMark : MonoBehaviour
	{
		// Token: 0x17000F51 RID: 3921
		// (get) Token: 0x06008AE1 RID: 35553 RVA: 0x00404428 File Offset: 0x00402628
		private int DistRange
		{
			get
			{
				return (int)(this.maxDist - this.minDist);
			}
		}

		// Token: 0x17000F52 RID: 3922
		// (get) Token: 0x06008AE2 RID: 35554 RVA: 0x00404438 File Offset: 0x00402638
		private RectTransform BarRect
		{
			get
			{
				return (null == this._rectTransform) ? (this._rectTransform = base.GetComponent<RectTransform>()) : this._rectTransform;
			}
		}

		// Token: 0x06008AE3 RID: 35555 RVA: 0x0040446C File Offset: 0x0040266C
		public void Refresh(float scale)
		{
			float pos = (float)(this.minDist * 5) * scale;
			pos = (this.reserve ? (-pos) : pos);
			float width = (float)(this.DistRange * 5) * scale;
			this.BarRect.anchoredPosition = this.BarRect.anchoredPosition.SetX(pos);
			this.BarRect.SetWidth(width);
			for (int i = 0; i < base.transform.childCount; i++)
			{
				RectTransform mark = (RectTransform)base.transform.GetChild(i);
				float markPos = (float)(i * 50) * scale * (float)(this.reserve ? -1 : 1);
				mark.anchoredPosition = mark.anchoredPosition.SetX(markPos);
			}
		}

		// Token: 0x04006A7F RID: 27263
		public short minDist;

		// Token: 0x04006A80 RID: 27264
		public short maxDist;

		// Token: 0x04006A81 RID: 27265
		public bool reserve;

		// Token: 0x04006A82 RID: 27266
		private RectTransform _rectTransform;
	}
}
