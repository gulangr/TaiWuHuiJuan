using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Merchant;
using GameData.Domains.Taiwu.ExchangeSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Exchange
{
	// Token: 0x02000A2F RID: 2607
	[RequireComponent(typeof(CToggle))]
	public class ShopToggle : MonoBehaviour
	{
		// Token: 0x17000DE3 RID: 3555
		// (get) Token: 0x06007FD2 RID: 32722 RVA: 0x003B8271 File Offset: 0x003B6471
		public bool IsUnlock
		{
			get
			{
				return this._isUnlock && base.gameObject.activeSelf;
			}
		}

		// Token: 0x06007FD3 RID: 32723 RVA: 0x003B828C File Offset: 0x003B648C
		public void Set(bool isUnlock)
		{
			bool flag = this._isUnlock == isUnlock;
			if (!flag)
			{
				SpriteState state = this.activeToggle.spriteState;
				if (isUnlock)
				{
					this.selected.sprite = (state.selectedSprite = (state.pressedSprite = this.activeSelected));
					this.hover.sprite = (state.highlightedSprite = this.activeHover);
					state.disabledSprite = (this.back.sprite = this.activeBase);
				}
				else
				{
					this.selected.sprite = (state.selectedSprite = (state.pressedSprite = this.inactiveSelected));
					this.hover.sprite = (state.highlightedSprite = this.inactiveHover);
					state.disabledSprite = (this.back.sprite = this.inactiveBase);
				}
				this.activeToggle.spriteState = state;
				Behaviour behaviour = this.tip;
				this._isUnlock = isUnlock;
				behaviour.enabled = !isUnlock;
			}
		}

		// Token: 0x06007FD4 RID: 32724 RVA: 0x003B83B4 File Offset: 0x003B65B4
		public void Refresh(ShopExchange exchange)
		{
			bool isActive = exchange.IsPageShow(this.level);
			base.gameObject.SetActive(isActive);
			bool flag = !isActive;
			if (!flag)
			{
				short maxBuyCount = ShopExchange.GetMaxBuyCount(this.level);
				TMP_Text tmp_Text = this.buyCount;
				string text;
				if (exchange.MinDebtLevel < this.level)
				{
					MerchantOverFavorLevelData data = exchange.TradeArguments.OverFavorData.MerchantOverFavorLevelDataArray[this.level];
					if (data != null && data.BuyCount != 32767)
					{
						text = string.Format("{0}/{1}", (data.BuyCount < 0) ? data.BuyCount.ToString().SetColor("darkred") : ((data.BuyCount < maxBuyCount) ? data.BuyCount.ToString().SetColor("orange") : maxBuyCount.ToString().SetColor("brightblue")), maxBuyCount);
						goto IL_DA;
					}
				}
				text = "";
				IL_DA:
				tmp_Text.text = text;
				bool flag2;
				if (exchange.TradeArguments.OverFavorData.MerchantOverFavorLevelDataArray.CheckIndex(this.level))
				{
					MerchantOverFavorLevelData merchantOverFavorLevelData = exchange.TradeArguments.OverFavorData.MerchantOverFavorLevelDataArray[this.level];
					flag2 = (((merchantOverFavorLevelData != null) ? new short?(merchantOverFavorLevelData.BuyCount) : null) <= 0);
				}
				else
				{
					flag2 = false;
				}
				bool lack = flag2;
				TooltipInvoker tooltipInvoker = this.tip;
				ArgumentBox argumentBox;
				if ((argumentBox = tooltipInvoker.RuntimeParam) == null)
				{
					argumentBox = (tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>());
				}
				argumentBox.Set("arg0", lack ? LanguageKey.LK_Exchange_Shop_Lack_Debt.Tr() : LanguageKey.LK_Shop_Favor_Tip.Tr()).Set("arg1", LanguageKey.LK_Exchange_Shop_Lack_Debt_Desc.Tr());
				this.tip.Type = (lack ? TipType.Simple : TipType.SingleDesc);
				this.tip.IsLanguageKey = false;
			}
		}

		// Token: 0x0400620D RID: 25101
		[SerializeField]
		internal RectTransform rectTransform;

		// Token: 0x0400620E RID: 25102
		[SerializeField]
		private CImage back;

		// Token: 0x0400620F RID: 25103
		[SerializeField]
		private CImage hover;

		// Token: 0x04006210 RID: 25104
		[SerializeField]
		private CImage selected;

		// Token: 0x04006211 RID: 25105
		[SerializeField]
		private Sprite activeBase;

		// Token: 0x04006212 RID: 25106
		[SerializeField]
		private Sprite inactiveBase;

		// Token: 0x04006213 RID: 25107
		[SerializeField]
		private Sprite activeHover;

		// Token: 0x04006214 RID: 25108
		[SerializeField]
		private Sprite inactiveHover;

		// Token: 0x04006215 RID: 25109
		[SerializeField]
		private Sprite activeSelected;

		// Token: 0x04006216 RID: 25110
		[SerializeField]
		private Sprite inactiveSelected;

		// Token: 0x04006217 RID: 25111
		[SerializeField]
		private CToggle activeToggle;

		// Token: 0x04006218 RID: 25112
		[SerializeField]
		private TMP_Text buyCount;

		// Token: 0x04006219 RID: 25113
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x0400621A RID: 25114
		[SerializeField]
		private int level;

		// Token: 0x0400621B RID: 25115
		private bool _isUnlock;
	}
}
