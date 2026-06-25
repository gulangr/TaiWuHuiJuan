using System;
using Config;
using FrameWork;
using GameData.Domains.Taiwu.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.Looping
{
	// Token: 0x0200097E RID: 2430
	public class CurrentStrategyItem : MonoBehaviour
	{
		// Token: 0x060074A4 RID: 29860 RVA: 0x00365324 File Offset: 0x00363524
		private void Awake()
		{
			PointerTrigger pointerTrigger = base.GetComponent<PointerTrigger>();
			pointerTrigger.EnterEvent.RemoveAllListeners();
			pointerTrigger.EnterEvent.AddListener(delegate()
			{
				bool isInEditingMode = this._isInEditingMode;
				if (!isInEditingMode)
				{
					this.highlight.SetActive(true);
				}
			});
			pointerTrigger.ExitEvent.RemoveAllListeners();
			pointerTrigger.ExitEvent.AddListener(delegate()
			{
				bool isInEditingMode = this._isInEditingMode;
				if (!isInEditingMode)
				{
					this.highlight.SetActive(false);
				}
			});
		}

		// Token: 0x060074A5 RID: 29861 RVA: 0x00365384 File Offset: 0x00363584
		public void Set(QiArtStrategyDisplayData qiArtStrategyDisplayData)
		{
			this.empty.SetActive(false);
			this.highlight.SetActive(false);
			this.content.SetActive(qiArtStrategyDisplayData != null && qiArtStrategyDisplayData.TemplateId != -1);
			this.SetStrategyEnable(this.content.activeSelf);
			bool flag = qiArtStrategyDisplayData != null && qiArtStrategyDisplayData.TemplateId != -1;
			if (flag)
			{
				QiArtStrategyItem config = QiArtStrategy.Instance[qiArtStrategyDisplayData.TemplateId];
				this.strategyName.text = config.Name;
				int currentDate = SingletonObject.getInstance<BasicGameData>().CurrDate;
				this.duration.text = (qiArtStrategyDisplayData.ExpireTime - currentDate).ToString();
				TooltipInvoker tooltipInvoker = this.mouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				this.mouseTip.RuntimeParam.Set("arg0", config.Name);
				this.mouseTip.RuntimeParam.Set("arg1", config.Desc);
			}
		}

		// Token: 0x060074A6 RID: 29862 RVA: 0x0036549C File Offset: 0x0036369C
		public void SetIsInEditingMode(bool isinEditingMode)
		{
			this._isInEditingMode = isinEditingMode;
			bool flag = !isinEditingMode;
			if (flag)
			{
				this.empty.SetActive(false);
				this.highlight.SetActive(false);
			}
		}

		// Token: 0x060074A7 RID: 29863 RVA: 0x003654D5 File Offset: 0x003636D5
		public void SetEmptyActive()
		{
			this.empty.SetActive(true);
			this.highlight.SetActive(false);
			this.content.SetActive(false);
		}

		// Token: 0x060074A8 RID: 29864 RVA: 0x003654FF File Offset: 0x003636FF
		public void SetHighlightActive()
		{
			this.empty.SetActive(false);
			this.highlight.SetActive(true);
			this.content.SetActive(false);
		}

		// Token: 0x060074A9 RID: 29865 RVA: 0x00365529 File Offset: 0x00363729
		private void OnDisable()
		{
			this.ResetStrategyState();
		}

		// Token: 0x060074AA RID: 29866 RVA: 0x00365533 File Offset: 0x00363733
		public void ResetStrategyState()
		{
			this._isStrategyEnabled = null;
		}

		// Token: 0x060074AB RID: 29867 RVA: 0x00365544 File Offset: 0x00363744
		public void SetStrategyEnable(bool enable)
		{
			bool flag = this.eff_strategyEnableHint == null;
			if (!flag)
			{
				if (enable)
				{
					bool? isStrategyEnabled = this._isStrategyEnabled;
					bool flag2 = false;
					bool flag3 = isStrategyEnabled.GetValueOrDefault() == flag2 & isStrategyEnabled != null;
					if (flag3)
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
		}

		// Token: 0x0400572F RID: 22319
		[SerializeField]
		private CImage icon;

		// Token: 0x04005730 RID: 22320
		[SerializeField]
		private TextMeshProUGUI strategyName;

		// Token: 0x04005731 RID: 22321
		[SerializeField]
		private TextMeshProUGUI duration;

		// Token: 0x04005732 RID: 22322
		[SerializeField]
		private TooltipInvoker mouseTip;

		// Token: 0x04005733 RID: 22323
		[SerializeField]
		private GameObject empty;

		// Token: 0x04005734 RID: 22324
		[SerializeField]
		private GameObject highlight;

		// Token: 0x04005735 RID: 22325
		[SerializeField]
		private GameObject content;

		// Token: 0x04005736 RID: 22326
		[SerializeField]
		private ParticleSystem eff_strategyEnableHint;

		// Token: 0x04005737 RID: 22327
		public int Index;

		// Token: 0x04005738 RID: 22328
		private bool _isInEditingMode;

		// Token: 0x04005739 RID: 22329
		private bool? _isStrategyEnabled = null;
	}
}
