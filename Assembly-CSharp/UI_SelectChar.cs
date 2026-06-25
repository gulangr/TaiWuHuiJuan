using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.TaiwuEvent;
using UnityEngine;

// Token: 0x020003A0 RID: 928
public class UI_SelectChar : UIBase
{
	// Token: 0x060037D9 RID: 14297 RVA: 0x001C160C File Offset: 0x001BF80C
	public override void OnInit(ArgumentBox argsBox)
	{
		argsBox.Get<Action<int>>("callback", out this._onSelected);
		List<int> charIdList;
		argsBox.Get<List<int>>("charIdList", out charIdList);
		List<CharacterTable.CharacterTableCommonFilterTypes> filterTypes;
		bool flag = !argsBox.Get<List<CharacterTable.CharacterTableCommonFilterTypes>>("filterType", out filterTypes);
		if (flag)
		{
			filterTypes = new List<CharacterTable.CharacterTableCommonFilterTypes>
			{
				CharacterTable.CharacterTableCommonFilterTypes.Character
			};
		}
		List<int> selectedCharIdList;
		bool flag2 = !argsBox.Get<List<int>>("selectedCharIdList", out selectedCharIdList);
		if (flag2)
		{
			selectedCharIdList = new List<int>();
		}
		HashSet<int> bannedList;
		bool flag3 = !argsBox.Get<HashSet<int>>("bannedCharIdList", out bannedList);
		if (flag3)
		{
			bannedList = new HashSet<int>();
		}
		Func<HashSet<int>, int> getCurrValue;
		bool flag4 = !argsBox.Get<Func<HashSet<int>, int>>("getCurrValue", out getCurrValue);
		if (flag4)
		{
			getCurrValue = null;
		}
		bool flag5 = argsBox.Get("enableMultiSelect", out this._isMultiSelect) && this._isMultiSelect;
		if (flag5)
		{
			argsBox.Get<Action<int[]>>("onMultiSelect", out this._onMultiSelected);
		}
		else
		{
			this._selectCount = 1;
		}
		bool flag6 = argsBox.Get("selectCount", out this._selectCount);
		if (flag6)
		{
			argsBox.Get<Action<int[]>>("onMultiSelect", out this._onMultiSelected);
		}
		else
		{
			this._selectCount = 1;
		}
		bool value;
		bool? canSwitchSelect = argsBox.Get("canSwitchSelect", out value) ? new bool?(value) : null;
		bool canSelectSpecialChar;
		bool flag7 = !argsBox.Get("canSelectSpecialChar", out canSelectSpecialChar);
		if (flag7)
		{
			canSelectSpecialChar = true;
		}
		List<ECharacterTableType> usingPages;
		bool flag8 = !argsBox.Get<List<ECharacterTableType>>("usingPages", out usingPages);
		if (flag8)
		{
			usingPages = new List<ECharacterTableType>
			{
				ECharacterTableType.GeneralProperty,
				ECharacterTableType.MainAndAttackProperty,
				ECharacterTableType.HitProperty,
				ECharacterTableType.LifeSkill,
				ECharacterTableType.CombatSkill,
				ECharacterTableType.Personality,
				ECharacterTableType.ItemAndResource,
				ECharacterTableType.Command,
				ECharacterTableType.LegendBookCompetitors,
				ECharacterTableType.LegendBookFallen
			};
		}
		CharacterTable table = base.CGet<CharacterTable>("CharacterTable");
		table.canSwitchSelection = true;
		table.multiSelection = this._isMultiSelect;
		table.canSelectAll = false;
		table.maxSelectCount = (this._isMultiSelect ? this._selectCount : -1);
		CharacterTable characterTable = table;
		List<int> charList = charIdList;
		List<ItemKey> itemKeys = null;
		List<CharacterTable.CharacterTableFilterData> filters = null;
		HashSet<int> bannedChars = bannedList;
		HashSet<int> selectedChars = selectedCharIdList.ToHashSet<int>();
		Action<int> onAvatarBtnClicked = null;
		List<ECharacterTableType> usingPages2 = usingPages;
		List<CharacterTable.CharacterTableCommonFilterTypes> commonFilterTypesList = filterTypes;
		short charValueCharacterTableElementTemplateId = -1;
		bool canSelectSpecialChar2 = canSelectSpecialChar;
		characterTable.Init(charList, itemKeys, filters, bannedChars, selectedChars, onAvatarBtnClicked, usingPages2, commonFilterTypesList, charValueCharacterTableElementTemplateId, getCurrValue, canSelectSpecialChar2, canSwitchSelect);
		base.CGet<CButtonObsolete>("ButtonConfirm").ClearAndAddListener(new Action(this.OnConfirm));
		base.CGet<CButtonObsolete>("ButtonCancel").ClearAndAddListener(new Action(this.OnCancel));
		base.CGet<CButtonObsolete>("ButtonClose").ClearAndAddListener(new Action(this.OnCancel));
	}

	// Token: 0x060037DA RID: 14298 RVA: 0x001C1884 File Offset: 0x001BFA84
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = btnName == "ButtonClose" || btnName == "ButtonCancel";
		if (flag)
		{
			UIManager.Instance.HideUI(this.Element);
		}
		else
		{
			bool flag2 = btnName == "ButtonConfirm";
			if (flag2)
			{
				this.OnConfirm();
			}
		}
	}

	// Token: 0x060037DB RID: 14299 RVA: 0x001C18E1 File Offset: 0x001BFAE1
	private void OnCancel()
	{
		this.QuickHide();
	}

	// Token: 0x060037DC RID: 14300 RVA: 0x001C18EC File Offset: 0x001BFAEC
	private void OnConfirm()
	{
		List<int> list = base.CGet<CharacterTable>("CharacterTable").GetSelectedCharIdList();
		bool isMultiSelect = this._isMultiSelect;
		if (isMultiSelect)
		{
			this._onMultiSelected(list.ToArray());
		}
		else
		{
			this._onSelected(list[0]);
		}
		UIManager.Instance.HideUI(this.Element);
		this.TriggerEvent(true);
	}

	// Token: 0x060037DD RID: 14301 RVA: 0x001C1955 File Offset: 0x001BFB55
	public override void QuickHide()
	{
		AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
		base.QuickHide();
		this.TriggerEvent(false);
	}

	// Token: 0x060037DE RID: 14302 RVA: 0x001C197C File Offset: 0x001BFB7C
	private void TriggerEvent(bool confirm)
	{
		List<int> list = base.CGet<CharacterTable>("CharacterTable").GetSelectedCharIdList();
		bool flag = list.Count == 0;
		if (!flag)
		{
			bool isMultiSelect = this._isMultiSelect;
			if (isMultiSelect)
			{
				CharacterSet characterSet = default(CharacterSet);
				foreach (int charId in list)
				{
					characterSet.Add(charId);
				}
				TaiwuEventDomainMethod.Call.SetCharacterSetSelectResult("SelectCharOver", "SelectedCharId", characterSet);
			}
			else
			{
				TaiwuEventDomainMethod.Call.SetListenerEventActionIntArg("SelectCharOver", "SelectedCharId", list.First<int>());
			}
			TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg("SelectCharOver", "SelectedCharConfirm", confirm);
			TaiwuEventDomainMethod.Call.TriggerListener("SelectCharOver", true);
		}
	}

	// Token: 0x04002864 RID: 10340
	private Action<int> _onSelected;

	// Token: 0x04002865 RID: 10341
	private Action<int[]> _onMultiSelected;

	// Token: 0x04002866 RID: 10342
	private int _selectCount;

	// Token: 0x04002867 RID: 10343
	private bool _isMultiSelect;
}
