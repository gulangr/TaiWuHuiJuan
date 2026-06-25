using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Steamworks;
using TMPro;
using UnityEngine;

// Token: 0x02000262 RID: 610
public class ModUploadConfirmDialog : MonoBehaviour
{
	// Token: 0x060027FC RID: 10236 RVA: 0x00126980 File Offset: 0x00124B80
	public void Show(bool isCreate, Action<string> onConfirm)
	{
		bool flag = this.inputField;
		if (flag)
		{
			this.inputField.text = string.Empty;
		}
		string title = LocalStringManager.Get(isCreate ? LanguageKey.LK_Upload : LanguageKey.LK_Update);
		string content = LocalStringManager.Get(isCreate ? LanguageKey.LK_Mod_Upload_Create_Ready_Tip : LanguageKey.LK_Mod_Upload_Update_Ready_Tip).ColorReplace();
		this.titleText.text = title;
		this.contentText.text = content;
		string pchURL = "http://steamcommunity.com/sharedfiles/workshoplegalagreement";
		this.termOfService.OnLinkTipClick = delegate(string str)
		{
			SteamFriends.ActivateGameOverlayToWebPage(pchURL, EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default);
		};
		this.termOfService.OnLinkTipActivate = delegate(string str, ArgumentBox argBox)
		{
			argBox.Set("arg0", pchURL);
			return TipType.SingleDesc;
		};
		this.btnYes.ClearAndAddListener(delegate
		{
			this.Hide();
			Action<string> onConfirm2 = onConfirm;
			TMP_InputField tmp_InputField = this.inputField;
			onConfirm2((tmp_InputField != null) ? tmp_InputField.text : null);
		});
		this.btnNo.ClearAndAddListener(new Action(this.Hide));
		this.buttonAgreement.ClearAndAddListener(delegate
		{
			SteamFriends.ActivateGameOverlayToWebPage(pchURL, EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default);
		});
		base.gameObject.SetActive(true);
	}

	// Token: 0x060027FD RID: 10237 RVA: 0x00126A99 File Offset: 0x00124C99
	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04001D3D RID: 7485
	[SerializeField]
	private CButton btnYes;

	// Token: 0x04001D3E RID: 7486
	[SerializeField]
	private CButton btnNo;

	// Token: 0x04001D3F RID: 7487
	[SerializeField]
	private MouseTipLinkTips termOfService;

	// Token: 0x04001D40 RID: 7488
	[SerializeField]
	private CButton buttonAgreement;

	// Token: 0x04001D41 RID: 7489
	[SerializeField]
	private TMP_InputField inputField;

	// Token: 0x04001D42 RID: 7490
	[SerializeField]
	private TextMeshProUGUI titleText;

	// Token: 0x04001D43 RID: 7491
	[SerializeField]
	private TextMeshProUGUI contentText;
}
