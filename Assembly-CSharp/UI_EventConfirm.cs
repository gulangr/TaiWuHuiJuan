using System;
using FrameWork;
using GameData.Domains.TaiwuEvent;
using TMPro;
using UnityEngine;

// Token: 0x0200037C RID: 892
public class UI_EventConfirm : UIBase
{
	// Token: 0x06003416 RID: 13334 RVA: 0x0019D085 File Offset: 0x0019B285
	private void Awake()
	{
		this._title = base.CGet<TextMeshProUGUI>("Title");
		this._content = base.CGet<TextMeshProUGUI>("MassageText");
		this._buttonNo = base.CGet<CButtonObsolete>("BtnNo");
	}

	// Token: 0x06003417 RID: 13335 RVA: 0x0019D0BB File Offset: 0x0019B2BB
	public override void OnInit(ArgumentBox argsBox)
	{
		argsBox.Get("Title", out this._titleStr);
		argsBox.Get("Content", out this._contentStr);
		argsBox.Get("ShowBtnNo", out this._showBtnNo);
	}

	// Token: 0x06003418 RID: 13336 RVA: 0x0019D0F4 File Offset: 0x0019B2F4
	protected override void OnClick(Transform btn)
	{
		bool flag = btn.name == "BtnYes";
		if (flag)
		{
			this._selectResult = true;
			this.QuickHide();
		}
		bool flag2 = btn.name == "BtnNo";
		if (flag2)
		{
			this._selectResult = false;
			this.QuickHide();
		}
	}

	// Token: 0x06003419 RID: 13337 RVA: 0x0019D14A File Offset: 0x0019B34A
	public override void QuickHide()
	{
		AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
		base.QuickHide();
	}

	// Token: 0x0600341A RID: 13338 RVA: 0x0019D166 File Offset: 0x0019B366
	private void OnEnable()
	{
		this._title.SetText(this._titleStr, true);
		this._content.SetText(this._contentStr, true);
		this._buttonNo.gameObject.SetActive(this._showBtnNo);
	}

	// Token: 0x0600341B RID: 13339 RVA: 0x0019D1A6 File Offset: 0x0019B3A6
	private void OnDisable()
	{
		TaiwuEventDomainMethod.Call.TriggerListener("DialogChoiceMade", this._selectResult);
	}

	// Token: 0x040025F4 RID: 9716
	private TextMeshProUGUI _title;

	// Token: 0x040025F5 RID: 9717
	private TextMeshProUGUI _content;

	// Token: 0x040025F6 RID: 9718
	private CButtonObsolete _buttonNo;

	// Token: 0x040025F7 RID: 9719
	private string _titleStr;

	// Token: 0x040025F8 RID: 9720
	private string _contentStr;

	// Token: 0x040025F9 RID: 9721
	private bool _showBtnNo;

	// Token: 0x040025FA RID: 9722
	private bool _selectResult;

	// Token: 0x040025FB RID: 9723
	public const string Title = "Title";

	// Token: 0x040025FC RID: 9724
	public const string Content = "Content";

	// Token: 0x040025FD RID: 9725
	public const string ShowBtnNo = "ShowBtnNo";
}
