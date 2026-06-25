using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000ADA RID: 2778
	public class CricketHealthBar : MonoBehaviour
	{
		// Token: 0x060088B3 RID: 34995 RVA: 0x003F54B8 File Offset: 0x003F36B8
		public void Clear()
		{
			this.temporary.DOKill(false);
			this.temporary.fillAmount = (this.points.fillAmount = 1f);
			this.processValue.text = string.Empty;
			this._lastCurrent = -1;
			this._lastMax = -1;
		}

		// Token: 0x060088B4 RID: 34996 RVA: 0x003F5514 File Offset: 0x003F3714
		public void Set(int current, int max, int injury)
		{
			this.processValue.text = CricketCombatKit.WrapProperty(current, injury);
			bool flag = this._lastCurrent == current && this._lastMax == max;
			if (!flag)
			{
				this._lastCurrent = current;
				this._lastMax = max;
				float fillAmount = Mathf.Clamp01((float)current / (float)Mathf.Max(max, 1));
				this.temporary.DOKill(false);
				bool flag2 = fillAmount > this.temporary.fillAmount;
				if (flag2)
				{
					this.temporary.fillAmount = fillAmount;
				}
				else
				{
					this.temporary.DOFillAmount(fillAmount, 0.5f).SetDelay(0.5f);
				}
				this.points.fillAmount = fillAmount;
			}
		}

		// Token: 0x040068A1 RID: 26785
		[SerializeField]
		private CImage temporary;

		// Token: 0x040068A2 RID: 26786
		[SerializeField]
		private CImage points;

		// Token: 0x040068A3 RID: 26787
		[SerializeField]
		private TextMeshProUGUI processValue;

		// Token: 0x040068A4 RID: 26788
		private int _lastCurrent;

		// Token: 0x040068A5 RID: 26789
		private int _lastMax;
	}
}
