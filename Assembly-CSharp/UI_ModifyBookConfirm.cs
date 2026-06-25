using System;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x0200038E RID: 910
public class UI_ModifyBookConfirm : UIBase
{
	// Token: 0x0600360A RID: 13834 RVA: 0x001B3288 File Offset: 0x001B1488
	public override void OnInit(ArgumentBox argsBox)
	{
		argsBox.Get<Action>("OnConfirm", out this._onConfirm);
		argsBox.Get<Action>("OnCancel", out this._onCancel);
		argsBox.Get("CostExp", out this._expCost);
		argsBox.Get("MonitorExp", out this._monitorExp);
		argsBox.Get("BookName", out this._bookName);
		base.CGet<PopupWindow>("PopupWindowBase").OnConfirmClick = new Action(this.OnConfirmClick);
		base.CGet<PopupWindow>("PopupWindowBase").OnCancelClick = new Action(this.OnCancelClick);
		string content = LocalStringManager.GetFormat(LanguageKey.LK_CombatSkill_ModifyBookConfirm_Tip, this._bookName);
		base.CGet<TextMeshProUGUI>("Content").text = content;
		base.CGet<GameObject>("Time").SetActive(true);
		int remainTime = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
		base.CGet<GameObject>("Time").transform.Find("Current").GetComponentInChildren<TextMeshProUGUI>().text = LocalStringManager.GetFormat(LanguageKey.LK_Make_Resource_Require_Meet, remainTime.ToString().SetColor("brightblue"), this._timeCost);
		base.CGet<GameObject>("Exp").SetActive(true);
		string color = (this._monitorExp >= this._expCost) ? "brightblue" : "brightred";
		base.CGet<GameObject>("Exp").transform.Find("Current").GetComponentInChildren<TextMeshProUGUI>().text = LocalStringManager.GetFormat(LanguageKey.LK_Make_Resource_Require_Meet, CommonUtils.GetDisplayStringForNum(this._monitorExp, 100000).SetColor(color), this._expCost);
		bool canConfirm = this._monitorExp >= this._expCost && remainTime >= this._timeCost;
		base.CGet<CButtonObsolete>("Confirm").interactable = canConfirm;
	}

	// Token: 0x0600360B RID: 13835 RVA: 0x001B3460 File Offset: 0x001B1660
	private void OnCancelClick()
	{
		UIManager.Instance.HideUI(UIElement.ModifyBookConfirm);
		Action onCancel = this._onCancel;
		if (onCancel != null)
		{
			onCancel();
		}
	}

	// Token: 0x0600360C RID: 13836 RVA: 0x001B3485 File Offset: 0x001B1685
	private void OnConfirmClick()
	{
		UIManager.Instance.HideUI(UIElement.ModifyBookConfirm);
		Action onConfirm = this._onConfirm;
		if (onConfirm != null)
		{
			onConfirm();
		}
	}

	// Token: 0x0600360D RID: 13837 RVA: 0x001B34AA File Offset: 0x001B16AA
	public override void QuickHide()
	{
		base.QuickHide();
		this.OnCancelClick();
	}

	// Token: 0x04002733 RID: 10035
	private readonly int _timeCost = 10;

	// Token: 0x04002734 RID: 10036
	private int _expCost;

	// Token: 0x04002735 RID: 10037
	private int _monitorExp;

	// Token: 0x04002736 RID: 10038
	private string _bookName;

	// Token: 0x04002737 RID: 10039
	private Action _onConfirm;

	// Token: 0x04002738 RID: 10040
	private Action _onCancel;
}
