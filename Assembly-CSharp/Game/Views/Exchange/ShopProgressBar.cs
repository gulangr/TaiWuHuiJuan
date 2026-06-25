using System;
using UnityEngine;

namespace Game.Views.Exchange
{
	// Token: 0x02000A2A RID: 2602
	public class ShopProgressBar : MonoBehaviour
	{
		// Token: 0x06007F7A RID: 32634 RVA: 0x003B5E94 File Offset: 0x003B4094
		public void Set(int merchantFavorability, int deltaFavorability = 0)
		{
			int num;
			int worldProgressLimitedFavor;
			CommonUtils.IsMerchantFavorabilityReachProgressLimit(merchantFavorability, out num, out worldProgressLimitedFavor);
			bool flag = worldProgressLimitedFavor >= 100;
			if (flag)
			{
				this.limitArea.gameObject.SetActive(false);
				this.limitLine.gameObject.SetActive(false);
			}
			else
			{
				this.limitLine.position = this.childPoints[worldProgressLimitedFavor / 10].rectTransform.position;
				this.limitArea.offsetMin = new Vector2(this.limitLine.anchoredPosition.x, this.limitArea.offsetMin.y);
				this.limitArea.gameObject.SetActive(true);
				this.limitLine.gameObject.SetActive(true);
			}
			this.barBase.gameObject.SetActive(deltaFavorability > 0);
			bool flag2 = deltaFavorability > 0;
			if (flag2)
			{
				this.barBase.fillAmount = (float)deltaFavorability / 100f;
			}
			bool activeSelf = this.limitLine.gameObject.activeSelf;
			if (activeSelf)
			{
				this.barFill.fillAmount = (float)Math.Min(worldProgressLimitedFavor, merchantFavorability) / 100f;
			}
			else
			{
				this.barFill.fillAmount = (float)merchantFavorability / 100f;
			}
			merchantFavorability = Math.Min(merchantFavorability, worldProgressLimitedFavor);
			foreach (ShopProgressBarPointBase child in this.childPoints)
			{
				child.Set(merchantFavorability);
			}
		}

		// Token: 0x06007F7B RID: 32635 RVA: 0x003B6008 File Offset: 0x003B4208
		private void LateUpdate()
		{
			bool flag = !this.barBase.gameObject.activeSelf;
			if (!flag)
			{
				this.BreathEffect(Mathf.Abs(Mathf.Sin(Time.time)));
			}
		}

		// Token: 0x06007F7C RID: 32636 RVA: 0x003B6045 File Offset: 0x003B4245
		private void BreathEffect(float alpha)
		{
			this.barBase.SetAlpha(alpha);
		}

		// Token: 0x040061CE RID: 25038
		private const int Diff = 10;

		// Token: 0x040061CF RID: 25039
		[SerializeField]
		private ShopProgressBarPointBase[] childPoints;

		// Token: 0x040061D0 RID: 25040
		[SerializeField]
		private RectTransform limitLine;

		// Token: 0x040061D1 RID: 25041
		[SerializeField]
		private RectTransform limitArea;

		// Token: 0x040061D2 RID: 25042
		[SerializeField]
		private CImage barBase;

		// Token: 0x040061D3 RID: 25043
		[SerializeField]
		private CImage barFill;
	}
}
