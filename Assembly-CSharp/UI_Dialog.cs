using System;
using FrameWork;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000377 RID: 887
public class UI_Dialog : UIBase
{
	// Token: 0x17000597 RID: 1431
	// (get) Token: 0x060033F8 RID: 13304 RVA: 0x0019BD0F File Offset: 0x00199F0F
	private CButtonObsolete _btnConfirm
	{
		get
		{
			return base.CGet<CButtonObsolete>("BtnYes");
		}
	}

	// Token: 0x060033F9 RID: 13305 RVA: 0x0019BD1C File Offset: 0x00199F1C
	public override void OnInit(ArgumentBox argsBox)
	{
		if (argsBox != null)
		{
			argsBox.Get<DialogCmd>("Cmd", out this._showingCmd);
		}
		GlobalSettings settingData = SingletonObject.getInstance<GlobalSettings>();
		bool checkDonotShow = settingData.CheckDialogDoNotShow(this._showingCmd.DialogType);
		CommonSwitch switchDoNotShow = base.CGet<CommonSwitch>("CommonSwitch");
		switchDoNotShow.onValueChanged.RemoveAllListeners();
		switchDoNotShow.isOn = checkDonotShow;
		switchDoNotShow.onValueChanged.AddListener(new UnityAction<bool>(this.SetDonotShow));
		base.CGet<GameObject>("DonotShow").gameObject.SetActive(this._showingCmd.DialogType > EDialogType.None);
		base.CGet<TextMeshProUGUI>("Title").text = this._showingCmd.Title.ColorReplace();
		base.CGet<TextMeshProUGUI>("MassageText").text = this._showingCmd.Content.ColorReplace();
		TMPTextSpriteHelper tmpTextSpriteHelper = base.CGet<TextMeshProUGUI>("MassageText").GetComponent<TMPTextSpriteHelper>();
		tmpTextSpriteHelper.SpriteSizeFitType = this._showingCmd.SpriteHelperFitType;
		tmpTextSpriteHelper.StaticSpriteSize = this._showingCmd.SpriteHelperSize;
		tmpTextSpriteHelper.Parse();
		bool triButton = this._showingCmd.Type >= 3;
		base.CGet<GameObject>("ThreeButtonGroup").SetActive(triButton);
		this._btnConfirm.gameObject.SetActive(false);
		base.CGet<GameObject>("BtnNo").SetActive(false);
		base.CGet<CButtonObsolete>("BtnClose").gameObject.SetActive(false);
		bool flag = triButton;
		if (flag)
		{
			base.CGet<TextMeshProUGUI>("GroupYesText").text = this._showingCmd.GroupYesText;
			base.CGet<TextMeshProUGUI>("GroupNoText").text = this._showingCmd.GroupNoText;
			base.CGet<GameObject>("GroupNo").SetActive(this._showingCmd.Type == 3 || this._showingCmd.Type == 4);
			base.CGet<GameObject>("GroupCancel").SetActive(this._showingCmd.Type == 3);
		}
		else
		{
			this._btnConfirm.gameObject.SetActive(this._showingCmd.Type == 1);
			base.CGet<GameObject>("BtnNo").SetActive(this._showingCmd.Type == 1);
			base.CGet<CButtonObsolete>("BtnClose").gameObject.SetActive(this._showingCmd.Type == 2);
		}
		base.CGet<CButtonObsolete>("BtnClose").gameObject.SetActive(this._showingCmd.Type == 2);
	}

	// Token: 0x060033FA RID: 13306 RVA: 0x0019BFB0 File Offset: 0x0019A1B0
	private void SetDonotShow(bool donotShow)
	{
		GlobalSettings settingData = SingletonObject.getInstance<GlobalSettings>();
		if (donotShow)
		{
			settingData.AddDialogDoNotShow(this._showingCmd.DialogType);
		}
		else
		{
			settingData.RemoveDialogDoNotShow(this._showingCmd.DialogType);
		}
		settingData.SaveSettings();
	}

	// Token: 0x060033FB RID: 13307 RVA: 0x0019BFF8 File Offset: 0x0019A1F8
	public DialogCmd GetDiaLogCmd()
	{
		return this._showingCmd;
	}

	// Token: 0x060033FC RID: 13308 RVA: 0x0019C010 File Offset: 0x0019A210
	protected override void OnClick(Transform btn)
	{
		UIManager.Instance.HideUI(this.Element);
		string btnName = btn.name;
		bool flag = "BtnYes" == btnName || "GroupYes" == btnName || "BtnClose" == btnName;
		if (flag)
		{
			Action yes = this._showingCmd.Yes;
			if (yes != null)
			{
				yes();
			}
		}
		else
		{
			bool flag2 = "BtnNo" == btnName || "GroupNo" == btnName;
			if (flag2)
			{
				Action no = this._showingCmd.No;
				if (no != null)
				{
					no();
				}
			}
		}
	}

	// Token: 0x060033FD RID: 13309 RVA: 0x0019C0B4 File Offset: 0x0019A2B4
	private void Update()
	{
		bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) || CommonCommandKit.Enter.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			UIManager.Instance.HideUI(this.Element);
			Action yes = this._showingCmd.Yes;
			if (yes != null)
			{
				yes();
			}
		}
		else
		{
			bool flag2 = CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false) || CommonCommandKit.RightMouse.Check(this.Element, false, false, false, true, false);
			if (flag2)
			{
				AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
				UIManager.Instance.HideUI(this.Element);
				Action no = this._showingCmd.No;
				if (no != null)
				{
					no();
				}
			}
		}
	}

	// Token: 0x040025D9 RID: 9689
	private DialogCmd _showingCmd;

	// Token: 0x040025DA RID: 9690
	private TMPLinkClickBridge _linkClickBridge;
}
