using System;
using Game.Views.World;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x0200099F RID: 2463
	public class JieQingAreaHelper : MonoBehaviour
	{
		// Token: 0x17000D4F RID: 3407
		// (get) Token: 0x06007678 RID: 30328 RVA: 0x00373765 File Offset: 0x00371965
		// (set) Token: 0x06007679 RID: 30329 RVA: 0x0037376D File Offset: 0x0037196D
		public Area Area { get; private set; }

		// Token: 0x17000D50 RID: 3408
		// (get) Token: 0x0600767A RID: 30330 RVA: 0x00373776 File Offset: 0x00371976
		// (set) Token: 0x0600767B RID: 30331 RVA: 0x0037377E File Offset: 0x0037197E
		public int AreaId { get; private set; }

		// Token: 0x17000D51 RID: 3409
		// (get) Token: 0x0600767C RID: 30332 RVA: 0x00373787 File Offset: 0x00371987
		public RectTransform EffHolder
		{
			get
			{
				return this.effHolder ? this.effHolder : (base.transform as RectTransform);
			}
		}

		// Token: 0x0600767D RID: 30333 RVA: 0x003737AC File Offset: 0x003719AC
		public void Init(Area area, short areaId)
		{
			this.Area = area;
			this.AreaId = (int)areaId;
			bool flag = !this.effHolder;
			if (flag)
			{
				this.effHolder = (base.transform as RectTransform);
			}
		}

		// Token: 0x0600767E RID: 30334 RVA: 0x003737F0 File Offset: 0x003719F0
		public void UpdateFixedScaleItems(Vector3 scale)
		{
			bool flag = this.fixedScaleSigns == null;
			if (!flag)
			{
				foreach (Transform item in this.fixedScaleSigns)
				{
					bool flag2 = item;
					if (flag2)
					{
						item.localScale = Vector3.one / Mathf.Max(0.01f, scale.x);
					}
				}
			}
		}

		// Token: 0x0600767F RID: 30335 RVA: 0x0037385C File Offset: 0x00371A5C
		public void SetMurderSignCount(int count)
		{
			bool visible = count > 0;
			bool flag = this.murderSign;
			if (flag)
			{
				this.murderSign.SetActive(visible);
			}
			bool flag2 = visible && this.murderSignCount;
			if (flag2)
			{
				this.murderSignCount.text = count.ToString();
			}
		}

		// Token: 0x06007680 RID: 30336 RVA: 0x003738B4 File Offset: 0x00371AB4
		public void HideMurderSign()
		{
			bool flag = this.murderSign;
			if (flag)
			{
				this.murderSign.SetActive(false);
			}
		}

		// Token: 0x04005960 RID: 22880
		[SerializeField]
		private RectTransform effHolder;

		// Token: 0x04005961 RID: 22881
		[SerializeField]
		private GameObject murderSign;

		// Token: 0x04005962 RID: 22882
		[SerializeField]
		private TMP_Text murderSignCount;

		// Token: 0x04005963 RID: 22883
		[SerializeField]
		private Transform[] fixedScaleSigns;
	}
}
