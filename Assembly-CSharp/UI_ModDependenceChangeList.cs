using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.ModSystem;
using GameData.Domains.Mod;
using TMPro;
using UnityEngine;

// Token: 0x02000263 RID: 611
public class UI_ModDependenceChangeList : UIBase
{
	// Token: 0x17000464 RID: 1124
	// (get) Token: 0x060027FF RID: 10239 RVA: 0x00126AB1 File Offset: 0x00124CB1
	private bool IsOnRecordSelect
	{
		get
		{
			return UIManager.Instance.IsElementActive(UIElement.RecordSelect);
		}
	}

	// Token: 0x06002800 RID: 10240 RVA: 0x00126AC4 File Offset: 0x00124CC4
	public override void OnInit(ArgumentBox argsBox)
	{
		this.InitRefers();
		this._dependenciesChangedList.Clear();
		List<ModId> list;
		bool flag = argsBox.Get<List<ModId>>("DependenciesChangedList", out list);
		if (flag)
		{
			this._dependenciesChangedList.AddRange(list);
		}
		this._btnYes.gameObject.SetActive(true);
		this._btnNo.gameObject.SetActive(this.IsOnRecordSelect);
		InfinityScrollLegacy scrollView = this._scrollView;
		if (scrollView.OnItemRender == null)
		{
			scrollView.OnItemRender = delegate(int index, Refers refers)
			{
				ModId modId = this._dependenciesChangedList[index];
				ModInfoWithDisplayData modInfo = ModManager.GetModInfo(modId);
				refers.CGet<TextMeshProUGUI>("Title").text = modInfo.Title;
				bool isRemote = modInfo.ModId.Source == 1;
				refers.CGet<GameObject>("Steam").SetActive(isRemote);
				refers.CGet<GameObject>("External").SetActive(!isRemote);
			};
		}
		this._scrollView.SetDataCount(this._dependenciesChangedList.Count);
		this._contentText.text = LocalStringManager.Get(this.IsOnRecordSelect ? LanguageKey.LK_Mod_DependenceChanged_Content_Record : LanguageKey.LK_Mod_DependenceChanged_Content);
		this._confirmText.text = (this.IsOnRecordSelect ? LocalStringManager.Get(LanguageKey.LK_Mod_DependenceChanged_Confirm_Record) : string.Empty);
	}

	// Token: 0x06002801 RID: 10241 RVA: 0x00126BB0 File Offset: 0x00124DB0
	protected override void OnClick(Transform btn)
	{
		string name = btn.name;
		string a = name;
		if (!(a == "BtnYes"))
		{
			if (a == "BtnNo")
			{
				this.OnCancel();
			}
		}
		else
		{
			this.OnConfirm();
		}
	}

	// Token: 0x06002802 RID: 10242 RVA: 0x00126BF8 File Offset: 0x00124DF8
	private void OnConfirm()
	{
		bool isOnRecordSelect = this.IsOnRecordSelect;
		if (isOnRecordSelect)
		{
			this.QuickHide();
			UIManager.Instance.HideUI(UIElement.RecordSelect);
			UIManager.Instance.ShowUI(UIElement.ModPanelOld, true);
		}
		else
		{
			GEvent.OnEvent(UiEvents.DisableDependenciesChangedMods, null);
			this.QuickHide();
		}
	}

	// Token: 0x06002803 RID: 10243 RVA: 0x00126C53 File Offset: 0x00124E53
	private void OnCancel()
	{
		this.QuickHide();
	}

	// Token: 0x06002804 RID: 10244 RVA: 0x00126C60 File Offset: 0x00124E60
	private void InitRefers()
	{
		this._title = base.CGet<TextMeshProUGUI>("Title");
		this._contentText = base.CGet<TextMeshProUGUI>("ContentText");
		this._confirmText = base.CGet<TextMeshProUGUI>("ConfirmText");
		this._btnYes = base.CGet<CButtonObsolete>("BtnYes");
		this._btnNo = base.CGet<CButtonObsolete>("BtnNo");
		this._scrollView = base.CGet<InfinityScrollLegacy>("ScrollView");
	}

	// Token: 0x04001D44 RID: 7492
	private readonly List<ModId> _dependenciesChangedList = new List<ModId>();

	// Token: 0x04001D45 RID: 7493
	private TextMeshProUGUI _title;

	// Token: 0x04001D46 RID: 7494
	private TextMeshProUGUI _contentText;

	// Token: 0x04001D47 RID: 7495
	private TextMeshProUGUI _confirmText;

	// Token: 0x04001D48 RID: 7496
	private CButtonObsolete _btnYes;

	// Token: 0x04001D49 RID: 7497
	private CButtonObsolete _btnNo;

	// Token: 0x04001D4A RID: 7498
	private InfinityScrollLegacy _scrollView;
}
