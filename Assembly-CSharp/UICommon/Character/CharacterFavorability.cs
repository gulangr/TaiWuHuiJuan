using System;
using CharacterDataMonitor;
using TMPro;
using UICommon.Character.Elements;

namespace UICommon.Character
{
	// Token: 0x020005D0 RID: 1488
	public class CharacterFavorability : CharacterUIElement
	{
		// Token: 0x170008E1 RID: 2273
		// (get) Token: 0x06004678 RID: 18040 RVA: 0x00210C18 File Offset: 0x0020EE18
		private BasicInfoMonitor Item
		{
			get
			{
				return this.MonitorDataItem as BasicInfoMonitor;
			}
		}

		// Token: 0x06004679 RID: 18041 RVA: 0x00210C28 File Offset: 0x0020EE28
		public CharacterFavorability(CImage icon, TextMeshProUGUI label, TooltipInvoker mouseTip = null, TMPTextFadeAnimation animation = null, CImage favorDebtIcon = null)
		{
			bool flag = null == icon && null == label;
			if (flag)
			{
				throw new Exception("can not handle CharacterFavorability for null elements");
			}
			this._infoItem = new InfoItem(icon, null, label, animation, mouseTip);
			this._infoItem.SetInfoName(LocalStringManager.Get(LanguageKey.LK_Favorability));
			this._infoItem.SetIcon(string.Empty);
			this._infoItem.SetInfoValue(string.Empty);
			this._favorDebtIcon = favorDebtIcon;
			bool flag2 = null != favorDebtIcon;
			if (flag2)
			{
				favorDebtIcon.gameObject.SetActive(false);
			}
			bool flag3 = null != mouseTip;
			if (flag3)
			{
				mouseTip.enabled = true;
				mouseTip.Type = TipType.Simple;
				mouseTip.IsLanguageKey = false;
				mouseTip.PresetParam = new string[]
				{
					LocalStringManager.Get(LanguageKey.LK_Favorability),
					LocalStringManager.Get(LanguageKey.LK_Favorability_TipContent)
				};
			}
		}

		// Token: 0x0600467A RID: 18042 RVA: 0x00210D1C File Offset: 0x0020EF1C
		public CharacterFavorability(Refers refers, bool tipType = false)
		{
			bool flag = refers == null;
			if (flag)
			{
				throw new Exception("refers can not be null to create CharacterCharm element!");
			}
			this._tipType = tipType;
			this._infoItem = new InfoItem(refers);
			this._infoItem.SetInfoName(LocalStringManager.Get(LanguageKey.LK_Favorability));
			this._infoItem.SetIcon(string.Empty);
			this._infoItem.SetInfoValue(string.Empty);
			TooltipInvoker mouseTip = this._infoItem.GetMouseTip();
			bool flag2 = null != mouseTip;
			if (flag2)
			{
				mouseTip.enabled = true;
				mouseTip.Type = TipType.Simple;
				mouseTip.IsLanguageKey = false;
				mouseTip.PresetParam = new string[]
				{
					LocalStringManager.Get(LanguageKey.LK_Favorability),
					LocalStringManager.Get(LanguageKey.LK_Favorability_TipContent)
				};
			}
			bool flag3 = refers.Names.Contains("FavorDebt");
			if (flag3)
			{
				this._favorDebtIcon = refers.CGet<CImage>("FavorDebt");
			}
		}

		// Token: 0x0600467B RID: 18043 RVA: 0x00210E13 File Offset: 0x0020F013
		internal override void BindEvent()
		{
			this.Item.AddDebtsOfTaiwuListener(new Action(this.FillElement));
		}

		// Token: 0x0600467C RID: 18044 RVA: 0x00210E2F File Offset: 0x0020F02F
		public override void UnbindEvent()
		{
			this.Item.RemoveDebtsOfTaiwuListener(new Action(this.FillElement));
		}

		// Token: 0x0600467D RID: 18045 RVA: 0x00210E4C File Offset: 0x0020F04C
		public override void FillElement()
		{
			bool flag = this.MonitorDataItem == null;
			if (!flag)
			{
				bool flag2 = !this._infoItem.HasValidElement();
				if (!flag2)
				{
					bool isTaiwu = base.IsTaiwu;
					if (isTaiwu)
					{
						this._infoItem.SetIcon("sp_icon_favorability_loveself");
						this._infoItem.SetInfoValue("-");
						bool flag3 = null != this._favorDebtIcon;
						if (flag3)
						{
							this._favorDebtIcon.gameObject.SetActive(false);
						}
					}
					else
					{
						this.FillElement(this.Item.FavorabilityToTaiwu, this.Item.IsInteractedCharacter, this.Item.HasAlertness, (long)this.Item.Alertness);
					}
				}
			}
		}

		// Token: 0x0600467E RID: 18046 RVA: 0x00210F0C File Offset: 0x0020F10C
		internal void FillElement(short favorabilityToTaiwu, bool isInteractedCharacter = false, bool hasDebt = false, long taiwuDebtWorth = 0L)
		{
			bool flag = !isInteractedCharacter;
			if (flag)
			{
				favorabilityToTaiwu = short.MinValue;
			}
			this._infoItem.SetIcon(CommonUtils.GetFavorIconByInteractedLegacy(favorabilityToTaiwu, isInteractedCharacter));
			this._infoItem.SetInfoValue(CommonUtils.GetFavorString(favorabilityToTaiwu));
			bool flag2 = null != this._favorDebtIcon;
			if (flag2)
			{
				this._favorDebtIcon.gameObject.SetActive(hasDebt);
				string debtIcon = CommonUtils.GetDebtIcon(taiwuDebtWorth);
				this._favorDebtIcon.SetSprite(debtIcon, true, null);
			}
			bool tipType = this._tipType;
			if (tipType)
			{
				this._infoItem.GetMouseTip().PresetParam[1] = CommonUtils.GetFavorString(favorabilityToTaiwu);
			}
		}

		// Token: 0x0600467F RID: 18047 RVA: 0x00210FAC File Offset: 0x0020F1AC
		public override void ResetToEmpty()
		{
			bool flag = null != this._favorDebtIcon;
			if (flag)
			{
				this._favorDebtIcon.gameObject.SetActive(false);
			}
			bool flag2 = !this._infoItem.HasValidElement();
			if (flag2)
			{
				bool flag3 = this.MonitorDataItem != null;
				if (flag3)
				{
					this.UnbindEvent();
					this.MonitorDataItem = null;
				}
			}
			else
			{
				this._infoItem.SetIcon(string.Empty);
				this._infoItem.SetInfoValue("-");
			}
		}

		// Token: 0x06004680 RID: 18048 RVA: 0x00211030 File Offset: 0x0020F230
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<BasicInfoMonitor>(charId, this.IsDead);
		}

		// Token: 0x06004681 RID: 18049 RVA: 0x00211053 File Offset: 0x0020F253
		public void SetNoData()
		{
			this._infoItem.SetIcon(CommonUtils.GetFavorIconByInteractedLegacy(0, false));
			this._infoItem.SetInfoValue("-".SetColor("white"));
		}

		// Token: 0x040030D6 RID: 12502
		private readonly InfoItem _infoItem;

		// Token: 0x040030D7 RID: 12503
		private CImage _favorDebtIcon;

		// Token: 0x040030D8 RID: 12504
		private bool _tipType = false;
	}
}
