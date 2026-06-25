using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using GameData.Domains.Item;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace AdventureEditor.Beta
{
	// Token: 0x020006AD RID: 1709
	public class UI_ItemTemplateSelector : UIBase
	{
		// Token: 0x170009BC RID: 2492
		// (get) Token: 0x06004FB2 RID: 20402 RVA: 0x00254182 File Offset: 0x00252382
		public bool AllowSelectSubType
		{
			get
			{
				return (this._selectType & UI_ItemTemplateSelector.ESelectType.ItemSubType) > UI_ItemTemplateSelector.ESelectType.None;
			}
		}

		// Token: 0x170009BD RID: 2493
		// (get) Token: 0x06004FB3 RID: 20403 RVA: 0x0025418F File Offset: 0x0025238F
		public bool AllowSelectItemTemplate
		{
			get
			{
				return (this._selectType & UI_ItemTemplateSelector.ESelectType.ItemTemplate) > UI_ItemTemplateSelector.ESelectType.None;
			}
		}

		// Token: 0x170009BE RID: 2494
		// (get) Token: 0x06004FB4 RID: 20404 RVA: 0x0025419C File Offset: 0x0025239C
		public bool AllowSwitching
		{
			get
			{
				return this._selectType == UI_ItemTemplateSelector.ESelectType.TemplateAndSubType;
			}
		}

		// Token: 0x170009BF RID: 2495
		// (get) Token: 0x06004FB5 RID: 20405 RVA: 0x002541A7 File Offset: 0x002523A7
		private sbyte NowItemType
		{
			get
			{
				return this.GetItemType();
			}
		}

		// Token: 0x06004FB6 RID: 20406 RVA: 0x002541B0 File Offset: 0x002523B0
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get("MultipleChoice", out this._multipleChoice);
			argsBox.Get<UI_ItemTemplateSelector.OnConfirmHandler>("OnConfirm", out this._onConfirm);
			Enum selectType;
			if (!argsBox.Get("SelectType", out selectType))
			{
				throw new ArgumentException("Unable to select without select type.");
			}
			this._selectType = (UI_ItemTemplateSelector.ESelectType)selectType;
			EditingAdventureData.ItemCostItem[] items;
			this._itemGroupData = (argsBox.Get<EditingAdventureData.ItemCostItem[]>("InitialSelection", out items) ? new List<EditingAdventureData.ItemCostItem>(items) : new List<EditingAdventureData.ItemCostItem>());
			CToggleObsolete selectSubTypeToggle = base.CGet<CToggleObsolete>("SelectSubTypeToggle");
			selectSubTypeToggle.onValueChanged.RemoveAllListeners();
			bool flag = !this.AllowSelectSubType && selectSubTypeToggle.isOn;
			if (flag)
			{
				selectSubTypeToggle.isOn = false;
			}
			else
			{
				bool flag2 = !this.AllowSelectItemTemplate && !selectSubTypeToggle.isOn;
				if (flag2)
				{
					selectSubTypeToggle.isOn = true;
				}
			}
			selectSubTypeToggle.gameObject.SetActive(this.AllowSwitching);
			selectSubTypeToggle.onValueChanged.AddListener(delegate(bool _)
			{
				this.RefreshItemScroll();
			});
		}

		// Token: 0x06004FB7 RID: 20407 RVA: 0x002542B0 File Offset: 0x002524B0
		private void Awake()
		{
			CDropdownLegacy itemTypeDropdown = base.CGet<CDropdownLegacy>("ItemTypeDropdown");
			itemTypeDropdown.ClearOptions();
			List<string> itemTypeNames = new List<string>();
			for (int i = 0; i < 13; i++)
			{
				itemTypeNames.Add(LocalStringManager.Get(string.Format("LK_ItemType_{0}", i)));
			}
			itemTypeDropdown.AddOptions(itemTypeNames);
			itemTypeDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnItemTypeDropdownValueChanged));
			base.CGet<InfinityScrollLegacy>("ItemScroll").OnItemRender = new Action<int, Refers>(this.OnItemRender);
			TextMeshProUGUI selectedItemsText = base.CGet<TextMeshProUGUI>("SelectedItems");
			base.CGet<CButtonObsolete>("ButtonSelected").ClearAndAddListener(delegate
			{
				int linkIndex = TMP_TextUtilities.FindIntersectingLink(selectedItemsText, Input.mousePosition, UIManager.Instance.UiCamera);
				bool flag = linkIndex != -1;
				if (flag)
				{
					TMP_LinkInfo linkInfo = selectedItemsText.textInfo.linkInfo[linkIndex];
					int index;
					bool flag2 = int.TryParse(linkInfo.GetLinkID(), out index);
					if (flag2)
					{
						sbyte item = this._itemGroupData[index].Item1;
						int item2 = (int)this._itemGroupData[index].Item2;
						sbyte itemType = item;
						int itemId = item2;
						this.RemoveSelectedItem(itemType, (short)itemId);
						bool flag3 = itemType == this.NowItemType;
						if (flag3)
						{
							int dataIdx = this._showItemIdList.IndexOf((short)itemId);
							bool flag4 = -1 != dataIdx;
							if (flag4)
							{
								this.CGet<InfinityScrollLegacy>("ItemScroll").RefreshCell(dataIdx);
							}
						}
					}
				}
			});
			base.CGet<TMP_InputField>("ItemSearchInput").onValueChanged.AddListener(new UnityAction<string>(this.OnItemSearchInputValueChange));
		}

		// Token: 0x06004FB8 RID: 20408 RVA: 0x002543A0 File Offset: 0x002525A0
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string a = name;
			if (a == "BtnClose")
			{
				this.QuickHide();
			}
		}

		// Token: 0x06004FB9 RID: 20409 RVA: 0x002543CF File Offset: 0x002525CF
		private void OnEnable()
		{
			this.RefreshSelectedItems();
			this.RefreshItemScroll();
		}

		// Token: 0x06004FBA RID: 20410 RVA: 0x002543E0 File Offset: 0x002525E0
		private void RefreshSelectedItems()
		{
			List<EditingAdventureData.ItemCostItem> itemGroupData = this._itemGroupData;
			bool flag = itemGroupData == null || itemGroupData.Count <= 0;
			if (flag)
			{
				base.CGet<TextMeshProUGUI>("SelectedItems").text = LocalStringManager.Get(LanguageKey.UI_AdventureEditor_SelectedItems) + LocalStringManager.Get(LanguageKey.LK_None);
			}
			else
			{
				List<string> itemNames = new List<string>();
				for (int i = 0; i < this._itemGroupData.Count; i++)
				{
					sbyte item = this._itemGroupData[i].Item1;
					short item2 = this._itemGroupData[i].Item2;
					sbyte itemType = item;
					short itemId = item2;
					string itemName = (itemId >= 0) ? ItemUtils.GetItemColorName(itemType, itemId) : LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", UI_ItemTemplateSelector.WrapOrUnwrapItemSubType(itemId)));
					itemNames.Add(string.Format("<u><link=\"{0}\">{1}</link></u>", i, itemName));
				}
				base.CGet<TextMeshProUGUI>("SelectedItems").text = LocalStringManager.Get(LanguageKey.UI_AdventureEditor_SelectedItems) + string.Join("、", itemNames);
			}
		}

		// Token: 0x06004FBB RID: 20411 RVA: 0x00254500 File Offset: 0x00252700
		private sbyte GetItemType()
		{
			CDropdownLegacy itemTypeDropdown = base.CGet<CDropdownLegacy>("ItemTypeDropdown");
			return (sbyte)itemTypeDropdown.value;
		}

		// Token: 0x06004FBC RID: 20412 RVA: 0x00254528 File Offset: 0x00252728
		private void SetAllItemIdList()
		{
			bool showSubTypes = base.CGet<CToggleObsolete>("SelectSubTypeToggle").isOn;
			switch (this.NowItemType)
			{
			case 0:
				this._allItemIdList = (showSubTypes ? ItemSubType.Type2SubTypes[0].Select(new Func<short, short>(UI_ItemTemplateSelector.WrapOrUnwrapItemSubType)).ToList<short>() : Weapon.Instance.GetAllKeys());
				break;
			case 1:
				this._allItemIdList = (showSubTypes ? ItemSubType.Type2SubTypes[1].Select(new Func<short, short>(UI_ItemTemplateSelector.WrapOrUnwrapItemSubType)).ToList<short>() : Armor.Instance.GetAllKeys());
				break;
			case 2:
				this._allItemIdList = (showSubTypes ? ItemSubType.Type2SubTypes[2].Select(new Func<short, short>(UI_ItemTemplateSelector.WrapOrUnwrapItemSubType)).ToList<short>() : Accessory.Instance.GetAllKeys());
				break;
			case 3:
				this._allItemIdList = (showSubTypes ? ItemSubType.Type2SubTypes[3].Select(new Func<short, short>(UI_ItemTemplateSelector.WrapOrUnwrapItemSubType)).ToList<short>() : Clothing.Instance.GetAllKeys());
				break;
			case 4:
				this._allItemIdList = (showSubTypes ? ItemSubType.Type2SubTypes[4].Select(new Func<short, short>(UI_ItemTemplateSelector.WrapOrUnwrapItemSubType)).ToList<short>() : Carrier.Instance.GetAllKeys());
				break;
			case 5:
				this._allItemIdList = (showSubTypes ? ItemSubType.Type2SubTypes[5].Select(new Func<short, short>(UI_ItemTemplateSelector.WrapOrUnwrapItemSubType)).ToList<short>() : Config.Material.Instance.GetAllKeys());
				break;
			case 6:
				this._allItemIdList = (showSubTypes ? ItemSubType.Type2SubTypes[6].Select(new Func<short, short>(UI_ItemTemplateSelector.WrapOrUnwrapItemSubType)).ToList<short>() : CraftTool.Instance.GetAllKeys());
				break;
			case 7:
				this._allItemIdList = (showSubTypes ? ItemSubType.Type2SubTypes[7].Select(new Func<short, short>(UI_ItemTemplateSelector.WrapOrUnwrapItemSubType)).ToList<short>() : Food.Instance.GetAllKeys());
				break;
			case 8:
				this._allItemIdList = (showSubTypes ? ItemSubType.Type2SubTypes[8].Select(new Func<short, short>(UI_ItemTemplateSelector.WrapOrUnwrapItemSubType)).ToList<short>() : Medicine.Instance.GetAllKeys());
				break;
			case 9:
				this._allItemIdList = (showSubTypes ? ItemSubType.Type2SubTypes[9].Select(new Func<short, short>(UI_ItemTemplateSelector.WrapOrUnwrapItemSubType)).ToList<short>() : TeaWine.Instance.GetAllKeys());
				break;
			case 10:
				this._allItemIdList = (showSubTypes ? ItemSubType.Type2SubTypes[10].Select(new Func<short, short>(UI_ItemTemplateSelector.WrapOrUnwrapItemSubType)).ToList<short>() : SkillBook.Instance.GetAllKeys());
				break;
			case 11:
				this._allItemIdList = (showSubTypes ? ItemSubType.Type2SubTypes[11].Select(new Func<short, short>(UI_ItemTemplateSelector.WrapOrUnwrapItemSubType)).ToList<short>() : Cricket.Instance.GetAllKeys());
				break;
			case 12:
				this._allItemIdList = (showSubTypes ? ItemSubType.Type2SubTypes[12].Select(new Func<short, short>(UI_ItemTemplateSelector.WrapOrUnwrapItemSubType)).ToList<short>() : Misc.Instance.GetAllKeys());
				break;
			}
			bool flag = showSubTypes;
			if (flag)
			{
			}
		}

		// Token: 0x06004FBD RID: 20413 RVA: 0x0025485C File Offset: 0x00252A5C
		private void RefreshItemScroll()
		{
			this.SetAllItemIdList();
			bool flag = this._showItemIdList == null;
			if (flag)
			{
				this._showItemIdList = new List<short>();
			}
			this._showItemIdList.Clear();
			string searchKey = base.CGet<TMP_InputField>("ItemSearchInput").text;
			bool flag2 = string.IsNullOrEmpty(searchKey);
			if (flag2)
			{
				this._showItemIdList.AddRange(this._allItemIdList);
			}
			else
			{
				short id;
				bool flag3 = short.TryParse(searchKey, out id);
				if (flag3)
				{
					this._allItemIdList.ForEach(delegate(short e)
					{
						bool flag4 = e.ToString().Contains(searchKey);
						if (flag4)
						{
							this._showItemIdList.Add(e);
						}
					});
				}
				else
				{
					this._allItemIdList.ForEach(delegate(short e)
					{
						bool flag4 = e < 0;
						if (!flag4)
						{
							string itemName = ItemTemplateHelper.GetName(this.NowItemType, e);
							bool flag5 = itemName.Contains(searchKey);
							if (flag5)
							{
								this._showItemIdList.Add(e);
							}
						}
					});
				}
			}
			base.CGet<InfinityScrollLegacy>("ItemScroll").UpdateData(this._showItemIdList.Count);
		}

		// Token: 0x06004FBE RID: 20414 RVA: 0x00254944 File Offset: 0x00252B44
		private bool IsItemSelected(short itemId)
		{
			for (int i = 0; i < this._itemGroupData.Count; i++)
			{
				bool flag = this._itemGroupData[i].Item1 == this.NowItemType && this._itemGroupData[i].Item2 == itemId;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004FBF RID: 20415 RVA: 0x002549AC File Offset: 0x00252BAC
		private void RemoveSelectedItem(sbyte itemType, short itemId)
		{
			for (int i = 0; i < this._itemGroupData.Count; i++)
			{
				bool flag = this._itemGroupData[i].Item1 == itemType && this._itemGroupData[i].Item2 == itemId;
				if (flag)
				{
					this._itemGroupData.RemoveAt(i);
					break;
				}
			}
			this.RefreshSelectedItems();
		}

		// Token: 0x06004FC0 RID: 20416 RVA: 0x00254A1C File Offset: 0x00252C1C
		private void AddItemToSelected(short itemId)
		{
			bool flag = !this._multipleChoice;
			if (flag)
			{
				this._itemGroupData.Clear();
				base.CGet<InfinityScrollLegacy>("ItemScroll").UpdateData(this._showItemIdList.Count);
			}
			this._itemGroupData.Add(new ValueTuple<sbyte, short>(this.NowItemType, itemId));
			this.RefreshSelectedItems();
		}

		// Token: 0x06004FC1 RID: 20417 RVA: 0x00254A85 File Offset: 0x00252C85
		public override void QuickHide()
		{
			base.QuickHide();
			UI_ItemTemplateSelector.OnConfirmHandler onConfirm = this._onConfirm;
			if (onConfirm != null)
			{
				onConfirm(this._itemGroupData);
			}
		}

		// Token: 0x06004FC2 RID: 20418 RVA: 0x00254AA8 File Offset: 0x00252CA8
		private bool IsItemSubTypeSelected(short wrappedItemSubType)
		{
			for (int i = 0; i < this._itemGroupData.Count; i++)
			{
				bool flag = this._itemGroupData[i].Item1 == this.NowItemType && this._itemGroupData[i].Item2 == wrappedItemSubType;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004FC3 RID: 20419 RVA: 0x00254B10 File Offset: 0x00252D10
		private void RemoveSelectedItemSubType(sbyte itemType, short wrappedItemSubType)
		{
			for (int i = 0; i < this._itemGroupData.Count; i++)
			{
				bool flag = this._itemGroupData[i].Item1 == itemType && this._itemGroupData[i].Item2 == wrappedItemSubType;
				if (flag)
				{
					this._itemGroupData.RemoveAt(i);
					break;
				}
			}
			this.RefreshSelectedItems();
		}

		// Token: 0x06004FC4 RID: 20420 RVA: 0x00254B80 File Offset: 0x00252D80
		private void AddItemSubTypeToSelected(short wrappedItemSubType)
		{
			bool flag = !this._multipleChoice;
			if (flag)
			{
				this._itemGroupData.Clear();
				base.CGet<InfinityScrollLegacy>("ItemScroll").UpdateData(this._showItemIdList.Count);
			}
			this._itemGroupData.Add(new ValueTuple<sbyte, short>(this.NowItemType, wrappedItemSubType));
			this.RefreshSelectedItems();
		}

		// Token: 0x06004FC5 RID: 20421 RVA: 0x00254BE9 File Offset: 0x00252DE9
		private void OnTypeRender(int index, Refers refers)
		{
		}

		// Token: 0x06004FC6 RID: 20422 RVA: 0x00254BEC File Offset: 0x00252DEC
		public static short WrapOrUnwrapItemSubType(short itemSubType)
		{
			return -itemSubType - 1;
		}

		// Token: 0x06004FC7 RID: 20423 RVA: 0x00254BF4 File Offset: 0x00252DF4
		private void OnItemRender(int index, Refers refers)
		{
			bool selectState = false;
			bool isOn = base.CGet<CToggleObsolete>("SelectSubTypeToggle").isOn;
			if (isOn)
			{
				short wrappedItemSubType = this._showItemIdList[index];
				short itemSubType = UI_ItemTemplateSelector.WrapOrUnwrapItemSubType(wrappedItemSubType);
				refers.CGet<CImage>("IconBack").SetSprite(ItemUtils.GetItemIconBack(this.NowItemType, 0), false, null);
				refers.CGet<CImage>("Icon").SetSprite(ItemTemplateHelper.GetIcon(this.NowItemType, 0), false, null);
				refers.CGet<TextMeshProUGUI>("ItemName").text = LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", itemSubType));
				refers.CGet<TextMeshProUGUI>("ItemTemplateId").text = itemSubType.ToString();
				selectState = this.IsItemSubTypeSelected(wrappedItemSubType);
				refers.CGet<CButtonObsolete>("Button").ClearAndAddListener(delegate
				{
					bool selectState = !selectState;
					selectState = selectState;
					if (selectState)
					{
						this.AddItemSubTypeToSelected(wrappedItemSubType);
					}
					else
					{
						this.RemoveSelectedItemSubType(this.NowItemType, wrappedItemSubType);
					}
					this.OnItemRender(index, refers);
				});
			}
			else
			{
				short itemId = this._showItemIdList[index];
				refers.CGet<CImage>("IconBack").SetSprite(ItemUtils.GetItemIconBack(this.NowItemType, itemId), false, null);
				refers.CGet<CImage>("Icon").SetSprite(ItemTemplateHelper.GetIcon(this.NowItemType, itemId), false, null);
				refers.CGet<TextMeshProUGUI>("ItemName").text = ItemUtils.GetItemColorName(this.NowItemType, itemId);
				refers.CGet<TextMeshProUGUI>("ItemTemplateId").text = itemId.ToString();
				selectState = this.IsItemSelected(itemId);
				refers.CGet<CButtonObsolete>("Button").ClearAndAddListener(delegate
				{
					bool selectState = !selectState;
					selectState = selectState;
					if (selectState)
					{
						this.AddItemToSelected(itemId);
					}
					else
					{
						this.RemoveSelectedItem(this.NowItemType, itemId);
					}
					this.OnItemRender(index, refers);
				});
			}
			refers.CGet<GameObject>("Selected").SetActive(selectState);
		}

		// Token: 0x06004FC8 RID: 20424 RVA: 0x00254E9C File Offset: 0x0025309C
		private void OnItemSearchInputValueChange(string inputString)
		{
			this.RefreshItemScroll();
		}

		// Token: 0x06004FC9 RID: 20425 RVA: 0x00254EA6 File Offset: 0x002530A6
		private void OnItemTypeDropdownValueChanged(int newIndex)
		{
			this.RefreshItemScroll();
		}

		// Token: 0x040036E8 RID: 14056
		private List<EditingAdventureData.ItemCostItem> _itemGroupData;

		// Token: 0x040036E9 RID: 14057
		private List<short> _allItemIdList;

		// Token: 0x040036EA RID: 14058
		private List<short> _showItemIdList;

		// Token: 0x040036EB RID: 14059
		private UI_ItemTemplateSelector.OnConfirmHandler _onConfirm;

		// Token: 0x040036EC RID: 14060
		private bool _multipleChoice;

		// Token: 0x040036ED RID: 14061
		private UI_ItemTemplateSelector.ESelectType _selectType;

		// Token: 0x02001AF0 RID: 6896
		[Flags]
		public enum ESelectType
		{
			// Token: 0x0400B779 RID: 46969
			None = 0,
			// Token: 0x0400B77A RID: 46970
			ItemTemplate = 1,
			// Token: 0x0400B77B RID: 46971
			ItemSubType = 2,
			// Token: 0x0400B77C RID: 46972
			TemplateAndSubType = 3
		}

		// Token: 0x02001AF1 RID: 6897
		// (Invoke) Token: 0x0600DFBF RID: 57279
		public delegate void OnConfirmHandler(List<EditingAdventureData.ItemCostItem> selectedItems);
	}
}
