using System;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x020003AB RID: 939
public class UI_ShopConfirm : UIBase
{
	// Token: 0x06003874 RID: 14452 RVA: 0x001C7C44 File Offset: 0x001C5E44
	public override void OnInit(ArgumentBox argsBox)
	{
		int tradeMoney;
		argsBox.Get("tradeMoney", out tradeMoney);
		int selfMoney;
		argsBox.Get("selfMoney", out selfMoney);
		int merchantMoney;
		argsBox.Get("merchantMoney", out merchantMoney);
		sbyte merchantType;
		argsBox.Get("merchantType", out merchantType);
		argsBox.Get<Action>("onConfirm", out this._onConfirm);
		bool isAreaDebtShop;
		argsBox.Get("isAreaDebtShop", out isAreaDebtShop);
		string moneyName = isAreaDebtShop ? LocalStringManager.Get(LanguageKey.LK_Area_Debt_Tip_Title) : LocalStringManager.Get(LanguageKey.LK_Resource_Name_Money);
		string moneyContent = LocalStringManager.Get((tradeMoney >= 0) ? LanguageKey.LK_Shop_DoDeal_Get_Money : LanguageKey.LK_Shop_DoDeal_Cost_Money);
		base.CGet<TextMeshProUGUI>("MoneyContent").SetText(moneyContent, true);
		int realTradeMoney = Mathf.Abs(Mathf.Min(tradeMoney, merchantMoney));
		string moneyChange = (tradeMoney >= 0) ? string.Format("+{0}", realTradeMoney).SetColor("brightblue") : string.Format("-{0}", realTradeMoney).SetColor("brightred");
		base.CGet<TextMeshProUGUI>("MoneyChange").SetText(moneyChange, true);
		base.CGet<TextMeshProUGUI>("ResourceName").text = moneyName;
		base.CGet<CImage>("ResourceIcon").SetSprite(isAreaDebtShop ? "ui9_icon_resource_bar_10" : "ui9_icon_resource_bar_6", false, null);
		this._canTrade = true;
		bool flag = merchantMoney < tradeMoney;
		string confirmContent;
		if (flag)
		{
			confirmContent = LocalStringManager.GetFormat(LanguageKey.LK_Shop_DoDealWarning, moneyName).SetColor("brightred");
		}
		else
		{
			bool flag2 = selfMoney >= -tradeMoney;
			if (flag2)
			{
				confirmContent = LocalStringManager.Get(LanguageKey.LK_Shop_DoDeal_Confirm);
			}
			else
			{
				this._canTrade = false;
				confirmContent = LocalStringManager.GetFormat(LanguageKey.LK_Shop_DoDeal_Money_Not_Enough, moneyName).SetColor("brightred");
			}
		}
		base.CGet<TextMeshProUGUI>("ConfirmContent").SetText(confirmContent, true);
		base.CGet<GameObject>("Cancel").SetActive(this._canTrade);
	}

	// Token: 0x06003875 RID: 14453 RVA: 0x001C7E1E File Offset: 0x001C601E
	private void OnCancelClick()
	{
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x06003876 RID: 14454 RVA: 0x001C7E34 File Offset: 0x001C6034
	private void OnConfirmClick()
	{
		bool canTrade = this._canTrade;
		if (canTrade)
		{
			Action onConfirm = this._onConfirm;
			if (onConfirm != null)
			{
				onConfirm();
			}
		}
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x06003877 RID: 14455 RVA: 0x001C7E6F File Offset: 0x001C606F
	public override void QuickHide()
	{
		base.QuickHide();
		this.OnCancelClick();
	}

	// Token: 0x06003878 RID: 14456 RVA: 0x001C7E80 File Offset: 0x001C6080
	protected override void OnClick(Transform btn)
	{
		string name = btn.name;
		string a = name;
		if (!(a == "Confirm"))
		{
			if (a == "Cancel")
			{
				this.OnCancelClick();
			}
		}
		else
		{
			this.OnConfirmClick();
		}
	}

	// Token: 0x06003879 RID: 14457 RVA: 0x001C7EC8 File Offset: 0x001C60C8
	private void Update()
	{
		bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			this.OnConfirmClick();
		}
		else
		{
			bool flag2 = CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false);
			if (flag2)
			{
				this.QuickHide();
			}
		}
	}

	// Token: 0x040028E6 RID: 10470
	private Action _onConfirm;

	// Token: 0x040028E7 RID: 10471
	private bool _canTrade;
}
