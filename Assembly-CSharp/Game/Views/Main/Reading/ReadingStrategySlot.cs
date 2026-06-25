using System;
using TMPro;
using UnityEngine;

namespace Game.Views.Main.Reading
{
	// Token: 0x02000972 RID: 2418
	public class ReadingStrategySlot : MonoBehaviour
	{
		// Token: 0x0600739E RID: 29598 RVA: 0x0035BA8B File Offset: 0x00359C8B
		private void OnDisable()
		{
			this._isStrategyEnabled = null;
		}

		// Token: 0x0600739F RID: 29599 RVA: 0x0035BA9A File Offset: 0x00359C9A
		public void ResetStrategyState()
		{
			this.strategyBack.SetActive(false);
			this._isStrategyEnabled = null;
		}

		// Token: 0x060073A0 RID: 29600 RVA: 0x0035BAB8 File Offset: 0x00359CB8
		public void SetStrategyEnable(bool enable)
		{
			this.strategyBack.SetActive(enable);
			if (enable)
			{
				bool? isStrategyEnabled = this._isStrategyEnabled;
				bool flag = false;
				bool flag2 = isStrategyEnabled.GetValueOrDefault() == flag & isStrategyEnabled != null;
				if (flag2)
				{
					this.eff_strategyEnableHint.gameObject.SetActive(true);
					this.eff_strategyEnableHint.Play();
				}
				this._isStrategyEnabled = new bool?(true);
			}
			else
			{
				this._isStrategyEnabled = new bool?(false);
				this.eff_strategyEnableHint.gameObject.SetActive(false);
			}
		}

		// Token: 0x04005620 RID: 22048
		public GameObject highlight;

		// Token: 0x04005621 RID: 22049
		public TextMeshProUGUI strategyName;

		// Token: 0x04005622 RID: 22050
		public GameObject strategyBack;

		// Token: 0x04005623 RID: 22051
		public TextMeshProUGUI leftTime;

		// Token: 0x04005624 RID: 22052
		public GameObject leftTimeHolder;

		// Token: 0x04005625 RID: 22053
		public GameObject previewMarker;

		// Token: 0x04005626 RID: 22054
		public TextMeshProUGUI previewMarkerText;

		// Token: 0x04005627 RID: 22055
		public GameObject clearMarker;

		// Token: 0x04005628 RID: 22056
		public ParticleSystem eff_strategyEnableHint;

		// Token: 0x04005629 RID: 22057
		private bool? _isStrategyEnabled = null;
	}
}
