using System;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.Looping
{
	// Token: 0x0200097D RID: 2429
	public class AvailableStrategyItem : MonoBehaviour
	{
		// Token: 0x17000D29 RID: 3369
		// (get) Token: 0x0600749F RID: 29855 RVA: 0x00365138 File Offset: 0x00363338
		private ViewLooping Parent
		{
			get
			{
				return UIElement.Looping.UiBaseAs<ViewLooping>();
			}
		}

		// Token: 0x060074A0 RID: 29856 RVA: 0x00365144 File Offset: 0x00363344
		private void Awake()
		{
			this.strategyBtn.ClearAndAddListener(delegate
			{
				bool flag = !this._isSelected;
				if (flag)
				{
					this.Parent.SetStrategy(this.Index);
				}
			});
		}

		// Token: 0x060074A1 RID: 29857 RVA: 0x00365160 File Offset: 0x00363360
		public void Set(QiArtStrategyItem config, bool isConcentrationEnough, bool isNeiliEnough, bool isSelected)
		{
			bool isCostEnough = isConcentrationEnough && isNeiliEnough;
			base.gameObject.SetActive(config != null);
			bool flag = config != null;
			if (flag)
			{
				this.strategyName.text = config.Name;
				this._isSelected = isSelected;
				this.cost.text = config.ConcentrationCost.ToString();
				TooltipInvoker tooltipInvoker = this.mouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				this.mouseTip.RuntimeParam.Set("arg0", config.Name);
				this.mouseTip.RuntimeParam.Set("arg1", config.Desc);
				this.strategyBtn.interactable = ((isCostEnough && this.Parent.HasEmptyStrategySlot) || isSelected);
				this.strategyBtn.enabled = !isSelected;
				this.ConcentrationLack.gameObject.SetActive(!isCostEnough && !isSelected);
				bool activeSelf = this.ConcentrationLack.gameObject.activeSelf;
				if (activeSelf)
				{
					this.ConcentrationLack.GetComponentInChildren<TextMeshProUGUI>().text = (isNeiliEnough ? LanguageKey.LK_Looping_Tip_Strategy_NotEnough.Tr() : LanguageKey.LK_Looping_Tip_Neili_NotEnough.Tr());
				}
				this.costBack.sprite = this.costBackArr[((!isCostEnough || !this.Parent.HasEmptyStrategySlot) && !isSelected) ? 1 : 0];
				this.strategyBg.sprite = this.strategyBgSprites[isSelected ? 1 : 0];
			}
		}

		// Token: 0x04005724 RID: 22308
		[SerializeField]
		private Sprite[] costBackArr;

		// Token: 0x04005725 RID: 22309
		[SerializeField]
		private Sprite[] strategyBgSprites;

		// Token: 0x04005726 RID: 22310
		[SerializeField]
		private CButton strategyBtn;

		// Token: 0x04005727 RID: 22311
		[SerializeField]
		private TooltipInvoker mouseTip;

		// Token: 0x04005728 RID: 22312
		[SerializeField]
		private TextMeshProUGUI strategyName;

		// Token: 0x04005729 RID: 22313
		[SerializeField]
		private TextMeshProUGUI cost;

		// Token: 0x0400572A RID: 22314
		[SerializeField]
		private CImage ConcentrationLack;

		// Token: 0x0400572B RID: 22315
		[SerializeField]
		private CImage costBack;

		// Token: 0x0400572C RID: 22316
		[SerializeField]
		private CImage strategyBg;

		// Token: 0x0400572D RID: 22317
		public int Index;

		// Token: 0x0400572E RID: 22318
		private bool _isSelected;
	}
}
