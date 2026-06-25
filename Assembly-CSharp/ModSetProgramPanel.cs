using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using FrameWork;
using FrameWork.ModSystem;
using FrameWork.UISystem.UIElements;
using Game.Views.Migrate.Mod;
using UnityEngine;

// Token: 0x02000260 RID: 608
public class ModSetProgramPanel : MonoBehaviour
{
	// Token: 0x17000463 RID: 1123
	// (get) Token: 0x060027E2 RID: 10210 RVA: 0x00125F63 File Offset: 0x00124163
	private string PluginDirectory
	{
		get
		{
			return this._modInfo.DirectoryName.IsNullOrEmpty() ? string.Empty : Path.Combine(this._modInfo.DirectoryName, "Plugins");
		}
	}

	// Token: 0x060027E3 RID: 10211 RVA: 0x00125F94 File Offset: 0x00124194
	public void Init()
	{
		this.btnYes.ClearAndAddListener(new Action(this.OnClickConfirm));
		this.btnNo.ClearAndAddListener(new Action(this.OnClickCancel));
		this.buttonLegacy.ClearAndAddListener(new Action(this.OnClickButtonLegacy));
		this.duplicatedTip.SetActive(false);
		this._orderDropdownOptionList.Clear();
	}

	// Token: 0x060027E4 RID: 10212 RVA: 0x00126003 File Offset: 0x00124203
	private void OnClickConfirm()
	{
		this._onConfirm();
		this.Hide();
	}

	// Token: 0x060027E5 RID: 10213 RVA: 0x00126019 File Offset: 0x00124219
	private void OnClickCancel()
	{
		this.Hide();
	}

	// Token: 0x060027E6 RID: 10214 RVA: 0x00126024 File Offset: 0x00124224
	public void Show(ModInfoWithDisplayData modInfo, List<string> tempFrontPlugins, List<string> tempBackendPlugins, Action onSave)
	{
		ModSetProgramPanel.<>c__DisplayClass22_0 CS$<>8__locals1 = new ModSetProgramPanel.<>c__DisplayClass22_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.tempFrontPlugins = tempFrontPlugins;
		CS$<>8__locals1.tempBackendPlugins = tempBackendPlugins;
		this._onSave = onSave;
		this._modInfo = modInfo;
		this._tempFrontendPlugins.Clear();
		this._tempFrontendPlugins.AddRange(CS$<>8__locals1.tempFrontPlugins);
		this._tempBackendPlugins.Clear();
		this._tempBackendPlugins.AddRange(CS$<>8__locals1.tempBackendPlugins);
		this.Show();
		this._onConfirm = new Action(CS$<>8__locals1.<Show>g__OnConfirm|0);
	}

	// Token: 0x060027E7 RID: 10215 RVA: 0x001260B4 File Offset: 0x001242B4
	private void Show()
	{
		bool flag = this._tempFrontendPlugins.Count == 0;
		if (flag)
		{
			this._tempFrontendPlugins.Add(string.Empty);
		}
		this.RefreshProgramList(this.frontend, this._tempFrontendPlugins);
		this.frontend.buttonMore.ClearAndAddListener(delegate
		{
			this._tempFrontendPlugins.Add(string.Empty);
			this.RefreshProgramList(this.frontend, this._tempFrontendPlugins);
		});
		bool flag2 = this._tempBackendPlugins.Count == 0;
		if (flag2)
		{
			this._tempBackendPlugins.Add(string.Empty);
		}
		this.RefreshProgramList(this.backend, this._tempBackendPlugins);
		this.backend.buttonMore.ClearAndAddListener(delegate
		{
			this._tempBackendPlugins.Add(string.Empty);
			this.RefreshProgramList(this.backend, this._tempBackendPlugins);
		});
		base.gameObject.SetActive(true);
	}

	// Token: 0x060027E8 RID: 10216 RVA: 0x00126175 File Offset: 0x00124375
	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x060027E9 RID: 10217 RVA: 0x00126184 File Offset: 0x00124384
	private void RefreshProgramList(ModProgramSetting refers, List<string> programList)
	{
		RectTransform rectTrans = refers.layout;
		this.RefreshLegacyButton();
		this.duplicatedTip.SetActive(false);
		for (int i = this._orderDropdownOptionList.Count; i < programList.Count; i++)
		{
			string str = string.Format("{0}.", i + 1);
			this._orderDropdownOptionList.Add(str);
		}
		this._dropdownList.Clear();
		for (int j = 0; j < programList.Count; j++)
		{
			Transform obj = (j < rectTrans.childCount) ? rectTrans.GetChild(j) : Object.Instantiate<Transform>(rectTrans.GetChild(0), rectTrans);
			obj.gameObject.SetActive(true);
			ModProgramSettingItem refer = obj.GetComponent<ModProgramSettingItem>();
			int index = j;
			string program = programList[index];
			CButton buttonDirectory = refer.buttonDirectory;
			UI_ModPanel.RefreshButtonPath(buttonDirectory, program);
			buttonDirectory.ClearAndAddListener(delegate
			{
				string path = LocalDialog.GetUnitySelectFileName("DLL Files(*.DLL)\0*.DLL\0", this.PluginDirectory);
				bool flag2 = File.Exists(path);
				if (flag2)
				{
					string fileName = Path.GetFileName(path);
					bool isContained = this._tempFrontendPlugins.Contains(fileName) || this._tempBackendPlugins.Contains(fileName);
					bool flag3 = isContained && programList[index] != fileName;
					if (flag3)
					{
						this.duplicatedTip.SetActive(true);
					}
					else
					{
						program = fileName;
						programList[index] = program;
						ModSetProgramPanel.TempFileNameToPathDict[program] = path;
						UI_ModPanel.RefreshButtonPath(buttonDirectory, program);
						this.RefreshProgramList(refers, programList);
					}
				}
			});
			CButton buttonLess = refer.buttonLess;
			buttonLess.ClearAndAddListener(delegate
			{
				programList.Remove(program);
				this.RefreshProgramList(refers, programList);
			});
			CDropdown dropdown = refer.orderDropdown;
			dropdown.onValueChanged.RemoveAllListeners();
			dropdown.ClearOptions();
			List<string> list = new List<string>();
			for (int k = 0; k < programList.Count; k++)
			{
				bool flag = !programList[k].IsNullOrEmpty() || index == programList.Count - 1;
				if (flag)
				{
					list.Add(this._orderDropdownOptionList[k]);
				}
			}
			dropdown.AddOptions(list);
			dropdown.value = index;
			dropdown.onValueChanged.AddListener(delegate(int value)
			{
				programList.Remove(program);
				programList.Insert(value, program);
				this.RefreshProgramList(refers, programList);
			});
			this._dropdownList.Add(dropdown);
			dropdown.interactable = !program.IsNullOrEmpty();
		}
		for (int l = programList.Count; l < rectTrans.childCount; l++)
		{
			rectTrans.GetChild(l).gameObject.SetActive(false);
		}
		CButton buttonMore = refers.buttonMore;
		buttonMore.interactable = !programList.Contains(string.Empty);
	}

	// Token: 0x060027EA RID: 10218 RVA: 0x00126470 File Offset: 0x00124670
	private void RefreshLegacyButton()
	{
		bool hasOrigin = this._modInfo.FrontendPlugins.Count > 0 || this._modInfo.BackendPlugins.Count > 0;
		this._compareFrontendPlugins.Clear();
		this._compareFrontendPlugins.AddRange(from p in this._tempFrontendPlugins
		where !p.IsNullOrEmpty()
		select p);
		this._compareBackendPlugins.Clear();
		this._compareBackendPlugins.AddRange(from p in this._tempBackendPlugins
		where !p.IsNullOrEmpty()
		select p);
		bool hasChange = ModSetProgramPanel.HasChange(this._modInfo.FrontendPlugins, this._compareFrontendPlugins) || ModSetProgramPanel.HasChange(this._modInfo.BackendPlugins, this._compareBackendPlugins);
		this.buttonLegacy.interactable = (hasOrigin && !hasChange && !ModSetProgramPanel.NeedBackup);
		TooltipInvoker tip = this.buttonLegacy.GetComponent<TooltipInvoker>();
		tip.Type = TipType.Simple;
		string title = LocalStringManager.Get(LanguageKey.LK_Mod_SetLegacy_Tip_Title);
		string content = hasChange ? LocalStringManager.Get(LanguageKey.LK_Mod_SetLegacy_Tip_HasChange).SetColor("brightred") : LocalStringManager.Get(LanguageKey.LK_Mod_SetLegacy_Tip_Content);
		tip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("arg0", title).Set("arg1", content);
	}

	// Token: 0x060027EB RID: 10219 RVA: 0x001265E1 File Offset: 0x001247E1
	private void ShowMakeDropdownMask(bool show)
	{
	}

	// Token: 0x060027EC RID: 10220 RVA: 0x001265E4 File Offset: 0x001247E4
	private void OnGUI()
	{
		foreach (CDropdown dropdown in this._dropdownList)
		{
			ModDropdownUtils.HandleDropdown(dropdown, null);
		}
	}

	// Token: 0x060027ED RID: 10221 RVA: 0x0012663C File Offset: 0x0012483C
	private void OnClickButtonLegacy()
	{
		bool flag = this._modInfo.FrontendPluginsLegacy.Count > 0 || this._modInfo.BackendPluginsLegacy.Count > 0;
		if (flag)
		{
			string title = LocalStringManager.Get(LanguageKey.LK_Mod_SetLegacy_Dialog_Title);
			string content = LocalStringManager.Get(LanguageKey.LK_Mod_SetLegacy_Dialog_Content);
			CommonUtils.ShowConfirmDialog(title, content, new Action(this.<OnClickButtonLegacy>g__OnConfirm|29_0), null, EDialogType.None);
		}
		else
		{
			this.<OnClickButtonLegacy>g__OnConfirm|29_0();
		}
	}

	// Token: 0x060027EE RID: 10222 RVA: 0x001266B2 File Offset: 0x001248B2
	public static void Clear()
	{
		ModSetProgramPanel.TempFileNameToPathDict.Clear();
		ModSetProgramPanel.NeedBackup = false;
	}

	// Token: 0x060027EF RID: 10223 RVA: 0x001266C8 File Offset: 0x001248C8
	private static bool HasChange(IReadOnlyList<string> a, IReadOnlyList<string> b)
	{
		bool flag = a.Count != b.Count;
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			for (int i = 0; i < a.Count; i++)
			{
				bool flag2 = a[i] != b[i];
				if (flag2)
				{
					return true;
				}
			}
			result = false;
		}
		return result;
	}

	// Token: 0x060027F4 RID: 10228 RVA: 0x001267E0 File Offset: 0x001249E0
	[CompilerGenerated]
	private void <OnClickButtonLegacy>g__OnConfirm|29_0()
	{
		ModSetProgramPanel.NeedBackup = true;
		this.RefreshLegacyButton();
		string title = LocalStringManager.Get(LanguageKey.LK_Mod_Program_Backup_Success_Tip_Title);
		string content = LocalStringManager.Get(LanguageKey.LK_Mod_Program_Backup_Success_Tip_Content);
		CommonUtils.ShowConfirmDialog(title, content, this._onSave, null, EDialogType.None);
	}

	// Token: 0x04001D25 RID: 7461
	[SerializeField]
	private ModProgramSetting frontend;

	// Token: 0x04001D26 RID: 7462
	[SerializeField]
	private ModProgramSetting backend;

	// Token: 0x04001D27 RID: 7463
	[SerializeField]
	private CButton btnYes;

	// Token: 0x04001D28 RID: 7464
	[SerializeField]
	private CButton btnNo;

	// Token: 0x04001D29 RID: 7465
	[SerializeField]
	private GameObject duplicatedTip;

	// Token: 0x04001D2A RID: 7466
	[SerializeField]
	private CButton buttonLegacy;

	// Token: 0x04001D2B RID: 7467
	private ModInfoWithDisplayData _modInfo;

	// Token: 0x04001D2C RID: 7468
	private readonly List<string> _tempFrontendPlugins = new List<string>();

	// Token: 0x04001D2D RID: 7469
	private readonly List<string> _tempBackendPlugins = new List<string>();

	// Token: 0x04001D2E RID: 7470
	private readonly List<string> _compareFrontendPlugins = new List<string>();

	// Token: 0x04001D2F RID: 7471
	private readonly List<string> _compareBackendPlugins = new List<string>();

	// Token: 0x04001D30 RID: 7472
	public static readonly Dictionary<string, string> TempFileNameToPathDict = new Dictionary<string, string>();

	// Token: 0x04001D31 RID: 7473
	public static bool NeedBackup = false;

	// Token: 0x04001D32 RID: 7474
	private readonly List<string> _orderDropdownOptionList = new List<string>();

	// Token: 0x04001D33 RID: 7475
	private readonly List<CDropdown> _dropdownList = new List<CDropdown>();

	// Token: 0x04001D34 RID: 7476
	private Action _onConfirm;

	// Token: 0x04001D35 RID: 7477
	private Action _onSave;
}
