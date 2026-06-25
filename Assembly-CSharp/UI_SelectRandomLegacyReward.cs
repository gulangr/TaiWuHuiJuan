using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;

// Token: 0x0200023A RID: 570
public class UI_SelectRandomLegacyReward : UIBase
{
	// Token: 0x06002512 RID: 9490 RVA: 0x00110DCC File Offset: 0x0010EFCC
	public override void OnInit(ArgumentBox argsBox)
	{
		int groupId;
		argsBox.Get("GroupId", out groupId);
		argsBox.Get<Action<short>>("OnSelectLegacy", out this._onSelectReward);
		this._window = base.CGet<PopupWindow>("Window");
		this._window.OnCancelClick = new Action(this.CancelSelectLegacy);
		this._window.OnConfirmClick = new Action(this.ConfirmSelectLegacy);
		this._toggleGroup = base.CGet<CToggleGroupObsolete>("Legacies");
		this._toggleGroup.InitPreOnToggle(-1);
		CToggleGroupObsolete toggleGroup = this._toggleGroup;
		if (toggleGroup.OnActiveToggleChange == null)
		{
			toggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnActiveToggleChanged);
		}
		bool flag = this._toggleGroup.AnyTogglesOn();
		if (flag)
		{
			this._toggleGroup.Set(this._toggleGroup.GetActive(), false);
		}
		this.UpdateConfirmButton();
		if (this._legacies == null)
		{
			this._legacies = new List<short>();
		}
		TaiwuDomainMethod.AsyncCall.GetRandomLegaciesInGroup(this, (sbyte)groupId, 3, new AsyncMethodCallbackDelegate(this.GetRandomLegaciesInGroup));
	}

	// Token: 0x06002513 RID: 9491 RVA: 0x00110ECF File Offset: 0x0010F0CF
	private void OnActiveToggleChanged(CToggleObsolete curr, CToggleObsolete prev)
	{
		this.UpdateConfirmButton();
	}

	// Token: 0x06002514 RID: 9492 RVA: 0x00110ED9 File Offset: 0x0010F0D9
	private void UpdateConfirmButton()
	{
		this._window.ConfirmButton.interactable = this._toggleGroup.AnyTogglesOn();
	}

	// Token: 0x06002515 RID: 9493 RVA: 0x00110EF8 File Offset: 0x0010F0F8
	private void GetRandomLegaciesInGroup(int offset, RawDataPool pool)
	{
		Serializer.Deserialize(pool, offset, ref this._legacies);
		this.ShowLegacies(this._legacies);
	}

	// Token: 0x06002516 RID: 9494 RVA: 0x00110F18 File Offset: 0x0010F118
	private void ConfirmSelectLegacy()
	{
		int index = this._toggleGroup.GetActive().Key;
		short legacyId = this._legacies[index];
		UIManager.Instance.HideUI(this.Element);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.SetObject("LegacyList", new List<short>
		{
			legacyId
		});
		argBox.SetObject("CloseAction", new Action(delegate
		{
			this._onSelectReward(legacyId);
		}));
		UIElement.GetItem.SetOnInitArgs(argBox);
		UIManager.Instance.MaskUI(UIElement.GetItem);
	}

	// Token: 0x06002517 RID: 9495 RVA: 0x00110FC0 File Offset: 0x0010F1C0
	private void CancelSelectLegacy()
	{
		string title = LocalStringManager.Get(LanguageKey.LK_Legacy_Reward_GiveUp_Title);
		string content = LocalStringManager.Get(LanguageKey.LK_Legacy_Reward_GiveUp_Content);
		CommonUtils.ShowConfirmDialog(title, content, delegate
		{
			UIManager.Instance.HideUI(this.Element);
		}, null, EDialogType.None);
	}

	// Token: 0x06002518 RID: 9496 RVA: 0x00110FFC File Offset: 0x0010F1FC
	private void ShowLegacies(List<short> legacies)
	{
		for (int i = 0; i < legacies.Count; i++)
		{
			CToggleObsolete toggle = this._toggleGroup.Get(i);
			LegacyView legacyView = toggle.GetComponentInParent<LegacyView>();
			LegacyItem configData = Legacy.Instance[legacies[i]];
			legacyView.RefreshBasicInfo(configData);
			legacyView.RefreshCostInfo(configData, false, false, true, true, false);
			legacyView.RefreshMouseTip(configData, configData.Desc);
			legacyView.RefreshHighlight(true, true, false);
			legacyView.RefreshInteraction(true, false, false);
			legacyView.SetOnToggleValueChanged(delegate(bool isOn)
			{
				legacyView.RefreshInteraction(true, isOn, false);
			});
		}
	}

	// Token: 0x06002519 RID: 9497 RVA: 0x001110C2 File Offset: 0x0010F2C2
	public override void QuickHide()
	{
		this.CancelSelectLegacy();
	}

	// Token: 0x04001BA8 RID: 7080
	private PopupWindow _window;

	// Token: 0x04001BA9 RID: 7081
	private CToggleGroupObsolete _toggleGroup;

	// Token: 0x04001BAA RID: 7082
	private List<short> _legacies;

	// Token: 0x04001BAB RID: 7083
	private Action<short> _onSelectReward;

	// Token: 0x04001BAC RID: 7084
	private const int LegacyCount = 3;
}
