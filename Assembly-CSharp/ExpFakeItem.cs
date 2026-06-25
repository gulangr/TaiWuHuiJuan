using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200022A RID: 554
public class ExpFakeItem : Refers
{
	// Token: 0x06002332 RID: 9010 RVA: 0x001034C9 File Offset: 0x001016C9
	private void Awake()
	{
		this.InitRefers();
	}

	// Token: 0x06002333 RID: 9011 RVA: 0x001034D4 File Offset: 0x001016D4
	public void Refresh(sbyte grade, string numberText = "", UnityAction onClick = null, UnityAction onMouseEnter = null, UnityAction onMouseExit = null, bool isDisabled = false, bool isSelected = false)
	{
		this.InitRefers();
		this.SetNumber(numberText);
		this.SetEnterEvent(onMouseEnter);
		this.SetExitEvent(onMouseExit);
		this.SetSelectState(isSelected);
		this.SetGrade(grade);
		this.SetClickEvent(onClick);
		this.SetDisable(isDisabled);
	}

	// Token: 0x06002334 RID: 9012 RVA: 0x00103525 File Offset: 0x00101725
	public void SetNumber(string numberText)
	{
		this._numberLabel.text = numberText;
	}

	// Token: 0x06002335 RID: 9013 RVA: 0x00103538 File Offset: 0x00101738
	public void SetClickEvent(UnityAction onClick)
	{
		CButtonObsolete button = base.GetComponent<CButtonObsolete>();
		button.onClick.RemoveAllListeners();
		bool flag = onClick != null;
		if (flag)
		{
			button.onClick.AddListener(onClick);
		}
	}

	// Token: 0x06002336 RID: 9014 RVA: 0x00103570 File Offset: 0x00101770
	public void SetEnterEvent(UnityAction onEnter)
	{
		PointerTrigger trigger = base.GetComponent<PointerTrigger>();
		trigger.EnterEvent.RemoveAllListeners();
		trigger.EnterEvent.AddListener(delegate()
		{
			this._enterMark.gameObject.SetActive(true);
		});
		bool flag = onEnter != null;
		if (flag)
		{
			trigger.EnterEvent.AddListener(onEnter);
		}
	}

	// Token: 0x06002337 RID: 9015 RVA: 0x001035C0 File Offset: 0x001017C0
	public void SetExitEvent(UnityAction onExit)
	{
		PointerTrigger trigger = base.GetComponent<PointerTrigger>();
		trigger.ExitEvent.RemoveAllListeners();
		trigger.ExitEvent.AddListener(delegate()
		{
			this._enterMark.gameObject.SetActive(false);
		});
		bool flag = onExit != null;
		if (flag)
		{
			trigger.ExitEvent.AddListener(onExit);
		}
	}

	// Token: 0x06002338 RID: 9016 RVA: 0x0010360E File Offset: 0x0010180E
	public void SetSelectState(bool selected)
	{
		this._checkMark.SetActive(selected);
	}

	// Token: 0x06002339 RID: 9017 RVA: 0x00103620 File Offset: 0x00101820
	public TooltipInvoker GetTip()
	{
		return base.GetComponent<TooltipInvoker>();
	}

	// Token: 0x0600233A RID: 9018 RVA: 0x00103638 File Offset: 0x00101838
	public void SetDisable(bool isDisabled)
	{
		HSVStyleRoot hsv = base.GetComponent<HSVStyleRoot>();
		if (isDisabled)
		{
			hsv.SetDefaultGrayAndBlack();
		}
		else
		{
			hsv.SetDefault();
		}
	}

	// Token: 0x0600233B RID: 9019 RVA: 0x00103662 File Offset: 0x00101862
	public void SetInteractable(bool interactable)
	{
		base.GetComponent<CButtonObsolete>().interactable = interactable;
		base.GetComponent<PointerTrigger>().enabled = interactable;
	}

	// Token: 0x0600233C RID: 9020 RVA: 0x0010367F File Offset: 0x0010187F
	public void SetGrade(sbyte grade)
	{
		this._gradeBack.SetSprite(CommonItemBack.GetGradeBack(grade), false, null);
	}

	// Token: 0x0600233D RID: 9021 RVA: 0x00103698 File Offset: 0x00101898
	private void InitRefers()
	{
		this._numberLabel = base.CGet<TextMeshProUGUI>("NumberLabel");
		this._checkMark = base.CGet<GameObject>("CheckMark");
		this._enterMark = base.CGet<GameObject>("EnterMark");
		this._gradeBack = base.CGet<CImage>("GradeBack");
	}

	// Token: 0x04001AFC RID: 6908
	private TextMeshProUGUI _numberLabel;

	// Token: 0x04001AFD RID: 6909
	private GameObject _checkMark;

	// Token: 0x04001AFE RID: 6910
	private GameObject _enterMark;

	// Token: 0x04001AFF RID: 6911
	private CImage _gradeBack;
}
