using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Taiwu;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Item
{
	// Token: 0x02000A10 RID: 2576
	public class ItemAutoOperationGroup : MonoBehaviour
	{
		// Token: 0x06007DEB RID: 32235 RVA: 0x003A6D3C File Offset: 0x003A4F3C
		public void Init(ItemAutoOperationSettingData data, bool isDiscard)
		{
			this._data = data;
			this._isDiscard = isDiscard;
			this._groupData = (isDiscard ? this._data.DiscardGroup : this._data.DisassembleGroup);
			this.textTitle.text = (isDiscard ? LanguageKey.LK_CharacterMenu_Items_AutoOperation_Discard.Tr() : LanguageKey.LK_CharacterMenu_Items_AutoOperation_Disassemble.Tr());
			this.buttonReset.ClearAndAddListener(new Action(this.OnClickButtonReset));
			this.switchToggle.onValueChanged.RemoveAllListeners();
			this.switchToggle.onValueChanged.AddListener(new UnityAction<bool>(this.SwitchToggleOnValueChanged));
			this.switchToggle.SetIsOnWithoutNotify(this._groupData.IsEnabled);
			this.InitItems();
			this.InitToggleGroupFilter();
			bool flag = !isDiscard;
			if (flag)
			{
				this.InitDisassembleSpecial();
			}
		}

		// Token: 0x06007DEC RID: 32236 RVA: 0x003A6E18 File Offset: 0x003A5018
		private void OnClickButtonReset()
		{
			string title = LanguageKey.LK_CharacterMenu_Items_AutoOperation_ResetAll_Title.Tr();
			string content = LanguageKey.LK_CharacterMenu_Items_AutoOperation_ResetAll_Content.Tr();
			CommonUtils.ShowConfirmDialog(title, content, delegate
			{
				bool isDiscard = this._isDiscard;
				if (isDiscard)
				{
					this._data.ResetDiscard();
				}
				else
				{
					this._data.ResetDisassemble();
				}
				this.Init(this._data, this._isDiscard);
			}, null, EDialogType.None);
		}

		// Token: 0x06007DED RID: 32237 RVA: 0x003A6E52 File Offset: 0x003A5052
		private void SwitchToggleOnValueChanged(bool isOn)
		{
			this._groupData.IsEnabled = isOn;
		}

		// Token: 0x06007DEE RID: 32238 RVA: 0x003A6E64 File Offset: 0x003A5064
		private void InitItems()
		{
			List<EItemAutoOperationTargetType> targetTypeList = ItemAutoOperationSettingGroup.GetTargetTypeList(this._groupData.OperationType);
			GameObject template = this.layoutItem.GetChild(0).gameObject;
			for (int i = 0; i < targetTypeList.Count; i++)
			{
				Transform child = (i < this.layoutItem.childCount) ? this.layoutItem.GetChild(i) : Object.Instantiate<GameObject>(template, this.layoutItem).transform;
				child.gameObject.SetActive(true);
				ItemAutoOperationItem item = child.GetComponent<ItemAutoOperationItem>();
				EItemAutoOperationTargetType type = targetTypeList[i];
				ItemAutoOperationSettingItem itemData = this._groupData.TypeDict.GetOrDefault(type);
				item.Init(itemData);
			}
			for (int j = targetTypeList.Count; j < this.layoutItem.childCount; j++)
			{
				this.layoutItem.GetChild(j).gameObject.SetActive(false);
			}
		}

		// Token: 0x06007DEF RID: 32239 RVA: 0x003A6F58 File Offset: 0x003A5158
		private void InitToggleGroupFilter()
		{
			this.toggleGroupSource.Clear();
			int count = EItemAutoOperationSource.Count.ToInt();
			GameObject template = this.toggleGroupSource.transform.GetChild(0).gameObject;
			sbyte index = 0;
			while ((int)index < count)
			{
				Transform child = ((int)index < this.toggleGroupSource.transform.childCount) ? this.toggleGroupSource.transform.GetChild((int)index) : Object.Instantiate<GameObject>(template, this.toggleGroupSource.transform).transform;
				CToggle item = child.GetComponent<CToggle>();
				EItemAutoOperationSource source = (EItemAutoOperationSource)index;
				if (!true)
				{
				}
				string text;
				switch (source)
				{
				case EItemAutoOperationSource.Combat:
					text = LanguageKey.LK_CharacterMenu_Items_AutoOperation_Source_Combat.Tr();
					break;
				case EItemAutoOperationSource.Pick:
					text = LanguageKey.LK_CharacterMenu_Items_AutoOperation_Source_Pick.Tr();
					break;
				case EItemAutoOperationSource.Other:
					text = LanguageKey.LK_CharacterMenu_Items_AutoOperation_Source_Other.Tr();
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				if (!true)
				{
				}
				string sourceName = text;
				TextMeshProUGUI componentInChildren = item.GetComponentInChildren<TextMeshProUGUI>();
				if (componentInChildren != null)
				{
					componentInChildren.SetText(sourceName, true);
				}
				bool isOn = this._groupData.SourceList.Contains(source);
				item.SetIsOnWithoutNotify(isOn);
				this.toggleGroupSource.Add(item);
				index += 1;
			}
			for (int i = count; i < this.toggleGroupSource.transform.childCount; i++)
			{
				this.toggleGroupSource.transform.GetChild(i).gameObject.SetActive(false);
			}
			this.toggleGroupSource.Init();
			this.toggleGroupSource.OnActiveIndexChange -= this.ToggleGroupFilterOnActiveIndexChange;
			this.toggleGroupSource.OnActiveIndexChange += this.ToggleGroupFilterOnActiveIndexChange;
		}

		// Token: 0x06007DF0 RID: 32240 RVA: 0x003A710C File Offset: 0x003A530C
		private void ToggleGroupFilterOnActiveIndexChange(int newIndex, int oldIndex)
		{
			bool flag = newIndex > -1 && !this._groupData.SourceList.Contains((EItemAutoOperationSource)newIndex);
			if (flag)
			{
				this._groupData.SourceList.Add((EItemAutoOperationSource)newIndex);
			}
			else
			{
				bool flag2 = oldIndex > -1 && this._groupData.SourceList.Contains((EItemAutoOperationSource)oldIndex);
				if (flag2)
				{
					this._groupData.SourceList.Remove((EItemAutoOperationSource)oldIndex);
				}
			}
		}

		// Token: 0x06007DF1 RID: 32241 RVA: 0x003A7180 File Offset: 0x003A5380
		private void InitDisassembleSpecial()
		{
			this.dropdownGradeTool.ClearOptions();
			List<string> optionList = new List<string>();
			for (int i = 0; i < 9; i++)
			{
				LanguageKey key = LanguageKey.LK_Grade_0 + i;
				optionList.Add(key.Tr().SetGradeColor(i));
			}
			this.dropdownGradeTool.AddOptions(optionList);
			this.dropdownGradeTool.onValueChanged.RemoveAllListeners();
			this.dropdownGradeTool.onValueChanged.AddListener(new UnityAction<int>(this.DropdownGradeToolOnValueChanged));
			this.dropdownGradeTool.SetValueWithoutNotify((int)this._data.DisassembleToolGrade);
			this.switchToggleDisassembleWhenDiscard.onValueChanged.RemoveAllListeners();
			this.switchToggleDisassembleWhenDiscard.onValueChanged.AddListener(new UnityAction<bool>(this.SwitchToggleDisassembleWhenDiscardOnValueChanged));
			this.switchToggleDisassembleWhenDiscard.SetIsOnWithoutNotify(this._data.DisassembleWhenDiscard);
			this.switchToggleDiscardWhenDisassemble.onValueChanged.RemoveAllListeners();
			this.switchToggleDiscardWhenDisassemble.onValueChanged.AddListener(new UnityAction<bool>(this.SwitchToggleDiscardWhenDisassembleOnValueChanged));
			this.switchToggleDiscardWhenDisassemble.SetIsOnWithoutNotify(this._data.DiscardWhenDisassemble);
		}

		// Token: 0x06007DF2 RID: 32242 RVA: 0x003A72AA File Offset: 0x003A54AA
		private void DropdownGradeToolOnValueChanged(int value)
		{
			this._data.DisassembleToolGrade = (sbyte)value;
		}

		// Token: 0x06007DF3 RID: 32243 RVA: 0x003A72BA File Offset: 0x003A54BA
		private void SwitchToggleDisassembleWhenDiscardOnValueChanged(bool isOn)
		{
			this._data.DisassembleWhenDiscard = isOn;
		}

		// Token: 0x06007DF4 RID: 32244 RVA: 0x003A72C9 File Offset: 0x003A54C9
		private void SwitchToggleDiscardWhenDisassembleOnValueChanged(bool isOn)
		{
			this._data.DiscardWhenDisassemble = isOn;
		}

		// Token: 0x04006011 RID: 24593
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x04006012 RID: 24594
		[SerializeField]
		private CButton buttonReset;

		// Token: 0x04006013 RID: 24595
		[SerializeField]
		private CSwitchToggle switchToggle;

		// Token: 0x04006014 RID: 24596
		[SerializeField]
		private CToggleGroupMultiSelect toggleGroupSource;

		// Token: 0x04006015 RID: 24597
		[SerializeField]
		private Transform layoutItem;

		// Token: 0x04006016 RID: 24598
		[Header("拆解的特殊内容")]
		[SerializeField]
		private CDropdown dropdownGradeTool;

		// Token: 0x04006017 RID: 24599
		[SerializeField]
		private CSwitchToggle switchToggleDisassembleWhenDiscard;

		// Token: 0x04006018 RID: 24600
		[SerializeField]
		private CSwitchToggle switchToggleDiscardWhenDisassemble;

		// Token: 0x04006019 RID: 24601
		private ItemAutoOperationSettingGroup _groupData;

		// Token: 0x0400601A RID: 24602
		private ItemAutoOperationSettingData _data;

		// Token: 0x0400601B RID: 24603
		private bool _isDiscard;
	}
}
