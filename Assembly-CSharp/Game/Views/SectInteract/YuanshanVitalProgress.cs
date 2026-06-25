using System;
using FrameWork;
using GameData.Domains.Extra;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x020009C0 RID: 2496
	public class YuanshanVitalProgress : MonoBehaviour
	{
		// Token: 0x06007930 RID: 31024 RVA: 0x00385508 File Offset: 0x00383708
		public void Set(int reverseThreshold, int maxThreshold)
		{
			this._reverseThreshold = reverseThreshold;
			this._maxThreshold = maxThreshold;
			float percent = (float)this._reverseThreshold / (float)this._maxThreshold;
			this.bottom.fillAmount = 1f - percent;
			this.rectSign.anchoredPosition = this.rectSign.anchoredPosition.SetX(this.bottom.GetComponent<RectTransform>().rect.width * percent);
		}

		// Token: 0x06007931 RID: 31025 RVA: 0x0038557D File Offset: 0x0038377D
		public void SetTips(bool isGoodEnd, SectStoryThreeVitalsCharacter data)
		{
			this.tips.Type = TipType.ThreeVitals;
			this.tips.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("isGoodEnd", isGoodEnd).SetObject("vitalData", data);
		}

		// Token: 0x06007932 RID: 31026 RVA: 0x003855B8 File Offset: 0x003837B8
		public void SetProgress(int value)
		{
			bool inRange = value <= this._reverseThreshold;
			string color = (value == 0) ? "grey" : (inRange ? "brightblue" : "brightred");
			this.progress.fillAmount = (float)value / (float)this._maxThreshold;
			this.progressReversed.fillAmount = (inRange ? 0f : (1f - (float)value / (float)this._maxThreshold));
			this.progressValue.text = (this.showMax ? string.Format("{0}/{1}", value.ToString().SetColor(color), this._maxThreshold) : string.Format("{0}%", value).SetColor(color));
		}

		// Token: 0x04005BD0 RID: 23504
		public CImage bottom;

		// Token: 0x04005BD1 RID: 23505
		public RectTransform rectSign;

		// Token: 0x04005BD2 RID: 23506
		public CImage progress;

		// Token: 0x04005BD3 RID: 23507
		public CImage progressReversed;

		// Token: 0x04005BD4 RID: 23508
		public TextMeshProUGUI progressValue;

		// Token: 0x04005BD5 RID: 23509
		public TooltipInvoker tips;

		// Token: 0x04005BD6 RID: 23510
		public bool showMax = false;

		// Token: 0x04005BD7 RID: 23511
		private int _reverseThreshold;

		// Token: 0x04005BD8 RID: 23512
		private int _maxThreshold;
	}
}
