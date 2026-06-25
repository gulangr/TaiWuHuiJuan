using System;
using UnityEngine;

namespace Game.Views.Exchange
{
	// Token: 0x02000A25 RID: 2597
	public class ExchangeAdvantageBar : MonoBehaviour
	{
		// Token: 0x06007F65 RID: 32613 RVA: 0x003B5728 File Offset: 0x003B3928
		public void Set(long currValue, long maxValueP1)
		{
			float fillRatio = Mathf.Min(1f, Mathf.Log(1f + Mathf.Abs((float)currValue), (float)maxValueP1));
			bool flag = currValue > 0L;
			if (flag)
			{
				this.redFill.fillAmount = fillRatio;
				this.redDot.gameObject.SetActive(true);
				this.redFill.gameObject.SetActive(true);
				this.greenDot.gameObject.SetActive(false);
				this.greenFill.gameObject.SetActive(false);
			}
			else
			{
				this.greenFill.fillAmount = fillRatio;
				this.redDot.gameObject.SetActive(false);
				this.redFill.gameObject.SetActive(false);
				this.greenDot.gameObject.SetActive(true);
				this.greenFill.gameObject.SetActive(true);
			}
		}

		// Token: 0x04006181 RID: 24961
		[SerializeField]
		private CImage redDot;

		// Token: 0x04006182 RID: 24962
		[SerializeField]
		private CImage greenDot;

		// Token: 0x04006183 RID: 24963
		[SerializeField]
		private CImage redFill;

		// Token: 0x04006184 RID: 24964
		[SerializeField]
		private CImage greenFill;
	}
}
