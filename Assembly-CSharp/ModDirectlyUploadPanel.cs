using System;
using System.IO;
using FrameWork;
using FrameWork.ModSystem;
using FrameWork.UISystem.UIElements;
using Game.Views.Migrate.Mod;
using GameData.Domains.Mod;
using Steamworks;
using TMPro;
using UnityEngine;

// Token: 0x0200025B RID: 603
public class ModDirectlyUploadPanel : MonoBehaviour
{
	// Token: 0x060027AA RID: 10154 RVA: 0x001244F0 File Offset: 0x001226F0
	public void Show(Action<ModInfoWithDisplayData, string> onConfirm)
	{
		this._onConfirm = onConfirm;
		this._modInfo = null;
		this._modPath = string.Empty;
		this._modChangeNote = string.Empty;
		this.pathInputField.text = string.Empty;
		this.visibilityDropdown.ClearOptions();
		this.visibilityDropdown.AddOptions(SteamManager.VisibilityOptionList);
		this.visibilityDropdown.value = 0;
		this.changeNoteInputField.text = string.Empty;
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
		this.buttonAgreement.ClearAndAddListener(delegate
		{
			SteamFriends.ActivateGameOverlayToWebPage(pchURL, EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default);
		});
		this.buttonDirectory.ClearAndAddListener(new Action(this.OnClickDirectory));
		this.btnYes.ClearAndAddListener(new Action(this.OnClickConfirm));
		this.btnNo.ClearAndAddListener(new Action(this.Hide));
		base.gameObject.SetActive(true);
	}

	// Token: 0x060027AB RID: 10155 RVA: 0x0012461A File Offset: 0x0012281A
	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x060027AC RID: 10156 RVA: 0x0012462C File Offset: 0x0012282C
	private void OnClickConfirm()
	{
		this._modPath = this.pathInputField.text;
		bool flag = string.IsNullOrEmpty(this._modPath) || !Directory.Exists(this._modPath);
		if (flag)
		{
			string title = LocalStringManager.Get(LanguageKey.LK_Mod_Upload_Invalid_Dir_Title);
			string content = LocalStringManager.Get(LanguageKey.LK_Mod_Upload_Invalid_Dir_Content);
			CommonUtils.ShowDialog(title, content, null, EDialogType.None);
		}
		else
		{
			string configPath = Path.Combine(this._modPath, "Config.Lua");
			bool flag2 = File.Exists(configPath);
			if (flag2)
			{
				this._modInfo = ModManager.ReadModInfo(configPath, string.Empty, true, false);
				this.Hide();
				this._modChangeNote = this.changeNoteInputField.text;
				this._modInfo.DirectoryName = this._modPath;
				this._modInfo.Visibility = (EModVisibility)this.visibilityDropdown.value;
				this._onConfirm(this._modInfo, this._modChangeNote);
			}
			else
			{
				string title2 = LocalStringManager.Get(LanguageKey.LK_Mod_Upload_No_Config_Title);
				string content2 = LocalStringManager.Get(LanguageKey.LK_Mod_Upload_No_Config_Content);
				CommonUtils.ShowDialog(title2, content2, null, EDialogType.None);
			}
		}
	}

	// Token: 0x060027AD RID: 10157 RVA: 0x00124744 File Offset: 0x00122944
	private void OnClickDirectory()
	{
		this._modPath = LocalDialog.GetUnitySaveDir("Select your Mod directory", ModManager.GetModRootFolder());
		this.pathInputField.SetTextWithoutNotify(this._modPath);
		bool flag = string.IsNullOrEmpty(this._modPath) || !Directory.Exists(this._modPath);
		if (flag)
		{
			string title = LocalStringManager.Get(LanguageKey.LK_Mod_Upload_Invalid_Dir_Title);
			string content = LocalStringManager.Get(LanguageKey.LK_Mod_Upload_Invalid_Dir_Content);
			CommonUtils.ShowDialog(title, content, null, EDialogType.None);
		}
	}

	// Token: 0x060027AE RID: 10158 RVA: 0x001247BD File Offset: 0x001229BD
	private void ShowMakeDropdownMask(bool show)
	{
	}

	// Token: 0x060027AF RID: 10159 RVA: 0x001247C0 File Offset: 0x001229C0
	private void OnGUI()
	{
		ModDropdownUtils.HandleDropdown(this.visibilityDropdown, null);
	}

	// Token: 0x04001CF1 RID: 7409
	[SerializeField]
	private TMP_InputField pathInputField;

	// Token: 0x04001CF2 RID: 7410
	[SerializeField]
	private CDropdown visibilityDropdown;

	// Token: 0x04001CF3 RID: 7411
	[SerializeField]
	private TMP_InputField changeNoteInputField;

	// Token: 0x04001CF4 RID: 7412
	[SerializeField]
	private MouseTipLinkTips termOfService;

	// Token: 0x04001CF5 RID: 7413
	[SerializeField]
	private CButton btnYes;

	// Token: 0x04001CF6 RID: 7414
	[SerializeField]
	private CButton btnNo;

	// Token: 0x04001CF7 RID: 7415
	[SerializeField]
	private CButton buttonDirectory;

	// Token: 0x04001CF8 RID: 7416
	[SerializeField]
	private CButton buttonAgreement;

	// Token: 0x04001CF9 RID: 7417
	private ModInfoWithDisplayData _modInfo;

	// Token: 0x04001CFA RID: 7418
	private string _modPath;

	// Token: 0x04001CFB RID: 7419
	private string _modChangeNote;

	// Token: 0x04001CFC RID: 7420
	private Action<ModInfoWithDisplayData, string> _onConfirm;
}
