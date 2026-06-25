using System;
using System.Collections.Generic;
using System.Reflection;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Item;
using Game.Components.SortAndFilter.Item.Apply;
using Game.Views.Combat;
using Game.Views.Select;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using UnityEngine;

namespace Game.Components.Common
{
	// Token: 0x02000F87 RID: 3975
	public class CombatRawCreateItemSelect : MonoBehaviour
	{
		// Token: 0x1700149C RID: 5276
		// (get) Token: 0x0600B6B4 RID: 46772 RVA: 0x005348CF File Offset: 0x00532ACF
		public ItemListScroll ItemListScroll
		{
			get
			{
				return this.groupItemList;
			}
		}

		// Token: 0x1700149D RID: 5277
		// (get) Token: 0x0600B6B5 RID: 46773 RVA: 0x005348D7 File Offset: 0x00532AD7
		public ISortAndFilterView SortFilterView
		{
			get
			{
				return this.sortAndFilterView;
			}
		}

		// Token: 0x0600B6B6 RID: 46774 RVA: 0x005348E0 File Offset: 0x00532AE0
		private void Awake()
		{
			bool flag = this.sortAndFilterView == null && this.groupItemList != null;
			if (flag)
			{
				this.sortAndFilterView = this.groupItemList.GetComponentInChildren<SortAndFilter>(true);
			}
		}

		// Token: 0x0600B6B7 RID: 46775 RVA: 0x00534924 File Offset: 0x00532B24
		public void Init(string saveKey)
		{
			this._saveKey = saveKey;
			this.groupItemList.Init(this._saveKey, ESortAndFilterControllerType.Equip, true, new Action<ITradeableContent, RowItemLine>(this.OnRenderRowItem), new Action<ITradeableContent, RowItemLine>(this.OnClickRowItem), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Power, null, null, null);
		}

		// Token: 0x0600B6B8 RID: 46776 RVA: 0x00534970 File Offset: 0x00532B70
		public void Set(List<TemplateKey> sourceData, Action<TemplateKey> onSelect, sbyte equipmentSlotIndex = -1)
		{
			this.SelectedRawCreateItem = new TemplateKey(-1, -1);
			this._equipmentSlotForFilter = equipmentSlotIndex;
			this._sourceData.Clear();
			bool flag = sourceData != null;
			if (flag)
			{
				foreach (TemplateKey i in sourceData)
				{
					this._sourceData.Add(new ItemDisplayData(i.ItemType, i.TemplateId));
				}
			}
			this._onSelect = onSelect;
			this.UpdateSortAndFilter();
		}

		// Token: 0x0600B6B9 RID: 46777 RVA: 0x00534A10 File Offset: 0x00532C10
		public void Clear()
		{
			this.SelectedRawCreateItem = new TemplateKey(-1, -1);
			this._equipmentSlotForFilter = -1;
			this._sourceData.Clear();
			this._appliedRawCreateColumnFlags = null;
			this._accessoryTreasureLayoutActive = false;
			bool flag = this.groupItemList == null || this.sortAndFilterView == null;
			if (!flag)
			{
				this.ReplaceItemListScrollSortFilter(new EmptyItemController(this.sortAndFilterView, LanguageKey.EventEditor_Error_DuplicateGroupKey));
				this.groupItemList.SetItemList(new List<ITradeableContent>(), -1);
				this.groupItemList.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600B6BA RID: 46778 RVA: 0x00534AAC File Offset: 0x00532CAC
		public bool Contains(TemplateKey key)
		{
			foreach (ITradeableContent data in this.groupItemList.FilteredData)
			{
				TemplateKey i = CombatRawCreateItemSelect.ToTemplateKey(data);
				bool flag = i.ItemType == key.ItemType && i.TemplateId == key.TemplateId;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600B6BB RID: 46779 RVA: 0x00534B34 File Offset: 0x00532D34
		public void Select(TemplateKey key)
		{
			this.OnSelect(key, true);
		}

		// Token: 0x0600B6BC RID: 46780 RVA: 0x00534B3F File Offset: 0x00532D3F
		public void SelectWithoutNotify(TemplateKey key)
		{
			this.OnSelect(key, false);
		}

		// Token: 0x0600B6BD RID: 46781 RVA: 0x00534B4C File Offset: 0x00532D4C
		public void TrySelectFirstMatchingGrade(ItemDisplayData equipment)
		{
			bool flag = equipment == null || this.groupItemList.FilteredData.Count == 0;
			if (!flag)
			{
				ItemKey itemKey = equipment.Key;
				sbyte targetGrade = ItemTemplateHelper.GetGrade(itemKey.ItemType, itemKey.TemplateId);
				for (int i = 0; i < this.groupItemList.FilteredData.Count; i++)
				{
					TemplateKey tk = CombatRawCreateItemSelect.ToTemplateKey(this.groupItemList.FilteredData[i]);
					bool flag2 = ItemTemplateHelper.GetGrade(tk.ItemType, tk.TemplateId) != targetGrade;
					if (!flag2)
					{
						int flat = this.groupItemList.FindItemIndex(this.groupItemList.FilteredData[i].Key);
						bool flag3 = flat >= 0;
						if (flag3)
						{
							this.groupItemList.InfiniteScroll.ScrollTo(flat, 0.3f);
						}
						this.OnSelect(tk, true);
						return;
					}
				}
				this.OnSelect(CombatRawCreateItemSelect.ToTemplateKey(this.groupItemList.FilteredData[0]), true);
			}
		}

		// Token: 0x0600B6BE RID: 46782 RVA: 0x00534C6C File Offset: 0x00532E6C
		private void EnsureSortAndFilterController()
		{
			ISortAndFilterView view = this.SortFilterView;
			bool flag = view == null;
			if (!flag)
			{
				EEquipSubFilterKeys? equipSub = this.ResolveEquipSubFilterKey();
				bool flag2 = equipSub != null;
				if (flag2)
				{
					CombatRawCreateDestinationItemSortAndFilterController raw = this.groupItemList.SortAndFilterController as CombatRawCreateDestinationItemSortAndFilterController;
					bool flag3 = raw != null && raw.EquipSubFilterKey == equipSub.Value;
					if (!flag3)
					{
						this.ReplaceItemListScrollSortFilter(new CombatRawCreateDestinationItemSortAndFilterController(view, equipSub.Value, this.ResolveRawCreateFilterPanelTitleKey()));
					}
				}
				else
				{
					bool flag4 = this.groupItemList.SortAndFilterController is SortOnlyItemController;
					if (!flag4)
					{
						this.ReplaceItemListScrollSortFilter(new SortOnlyItemController(view, this.ResolveRawCreateFilterPanelTitleKey()));
					}
				}
			}
		}

		// Token: 0x0600B6BF RID: 46783 RVA: 0x00534D1C File Offset: 0x00532F1C
		private LanguageKey ResolveRawCreateFilterPanelTitleKey()
		{
			EEquipSubFilterKeys? sub = this.ResolveEquipSubFilterKey();
			EEquipSubFilterKeys? eequipSubFilterKeys = sub;
			EEquipSubFilterKeys eequipSubFilterKeys2 = EEquipSubFilterKeys.Accessory;
			bool flag = eequipSubFilterKeys.GetValueOrDefault() == eequipSubFilterKeys2 & eequipSubFilterKeys != null;
			LanguageKey result;
			if (flag)
			{
				result = LanguageKey.LK_SelectItem_Column_AccessoryEffect;
			}
			else
			{
				bool flag2 = sub != null;
				if (flag2)
				{
					result = LanguageKey.LK_CommonSortAndFilter_Filter_Item_Main_2;
				}
				else
				{
					bool flag3 = this._sourceData.Count > 0 && this._sourceData[0].Key.ItemType == 2;
					if (flag3)
					{
						result = LanguageKey.LK_SelectItem_Column_AccessoryEffect;
					}
					else
					{
						result = LanguageKey.LK_CommonSortAndFilter_Filter_Item_Main_2;
					}
				}
			}
			return result;
		}

		// Token: 0x0600B6C0 RID: 46784 RVA: 0x00534DB0 File Offset: 0x00532FB0
		private ItemListScroll.EColumnType ResolveRawCreateListColumnFlags()
		{
			return ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Power;
		}

		// Token: 0x0600B6C1 RID: 46785 RVA: 0x00534DC8 File Offset: 0x00532FC8
		private bool ShouldUseAccessoryTreasureColumnLayout()
		{
			EEquipSubFilterKeys? sub = this.ResolveEquipSubFilterKey();
			EEquipSubFilterKeys? eequipSubFilterKeys = sub;
			EEquipSubFilterKeys eequipSubFilterKeys2 = EEquipSubFilterKeys.Accessory;
			bool flag = eequipSubFilterKeys.GetValueOrDefault() == eequipSubFilterKeys2 & eequipSubFilterKeys != null;
			return flag || (this._sourceData.Count > 0 && this._sourceData[0].Key.ItemType == 2);
		}

		// Token: 0x0600B6C2 RID: 46786 RVA: 0x00534E2C File Offset: 0x0053302C
		private void EnsureRawCreateListColumnLayout()
		{
			bool flag = this.ShouldUseAccessoryTreasureColumnLayout();
			if (flag)
			{
				bool accessoryTreasureLayoutActive = this._accessoryTreasureLayoutActive;
				if (!accessoryTreasureLayoutActive)
				{
					this._accessoryTreasureLayoutActive = true;
					this._appliedRawCreateColumnFlags = null;
					List<ColumnDefinition> defs = new List<ColumnDefinition>
					{
						SelectItemColumnHelper.CreateIconAndNameColumn(),
						SelectItemColumnHelper.CreateAccessoryEffectColumn()
					};
					this.groupItemList.SetColumnDefinitions(defs, new Action<RowItem>(this.PrepareCombatRawCreateAccessoryRowTemplate));
				}
			}
			else
			{
				bool accessoryTreasureLayoutActive2 = this._accessoryTreasureLayoutActive;
				if (accessoryTreasureLayoutActive2)
				{
					this._accessoryTreasureLayoutActive = false;
					this.groupItemList.SetColumnDefinitions(null, null);
				}
				ItemListScroll.EColumnType flags = this.ResolveRawCreateListColumnFlags();
				ItemListScroll.EColumnType? appliedRawCreateColumnFlags = this._appliedRawCreateColumnFlags;
				ItemListScroll.EColumnType ecolumnType = flags;
				bool flag2 = appliedRawCreateColumnFlags.GetValueOrDefault() == ecolumnType & appliedRawCreateColumnFlags != null;
				if (!flag2)
				{
					this._appliedRawCreateColumnFlags = new ItemListScroll.EColumnType?(flags);
					this.groupItemList.SetColumnTypeFlags(flags);
				}
			}
		}

		// Token: 0x0600B6C3 RID: 46787 RVA: 0x00534F09 File Offset: 0x00533109
		private void PrepareCombatRawCreateAccessoryRowTemplate(RowItem rowTemplate)
		{
			this.groupItemList.PrepareRowTemplateContainers(ItemListScroll.EColumnType.IconAndName);
			this.groupItemList.PrepareRowTemplateContainers(ItemListScroll.EColumnType.Power);
		}

		// Token: 0x0600B6C4 RID: 46788 RVA: 0x00534F2C File Offset: 0x0053312C
		private void ApplyRawCreateFilterPanelTitle()
		{
			SortAndFilter sortAndFilter = this.sortAndFilterView;
			if (sortAndFilter != null)
			{
				sortAndFilter.SetPanelTitleKey(this.ResolveRawCreateFilterPanelTitleKey());
			}
		}

		// Token: 0x0600B6C5 RID: 46789 RVA: 0x00534F48 File Offset: 0x00533148
		private void ReplaceItemListScrollSortFilter(ItemSortAndFilterController newController)
		{
			Type listType = typeof(ItemListScroll);
			FieldInfo field = listType.GetField("_sortAndFilterController", BindingFlags.Instance | BindingFlags.NonPublic);
			SortAndFilterController<ITradeableContent> oldCtrl = ((field != null) ? field.GetValue(this.groupItemList) : null) as SortAndFilterController<ITradeableContent>;
			if (oldCtrl != null)
			{
				oldCtrl.UninitForReplace();
			}
			newController.Init(new Action(this.ItemListHostOnSortAndFilterChanged), this._saveKey);
			FieldInfo field2 = listType.GetField("_sortAndFilterController", BindingFlags.Instance | BindingFlags.NonPublic);
			if (field2 != null)
			{
				field2.SetValue(this.groupItemList, newController);
			}
			FieldInfo field3 = listType.GetField("scroll", BindingFlags.Instance | BindingFlags.NonPublic);
			ListStyleGeneralScroll rowScroll = ((field3 != null) ? field3.GetValue(this.groupItemList) : null) as ListStyleGeneralScroll;
			bool flag = rowScroll != null;
			if (flag)
			{
				rowScroll.SetSortController(newController);
			}
			FieldInfo field4 = listType.GetField("cardScroll", BindingFlags.Instance | BindingFlags.NonPublic);
			CardStyleGeneralScroll cardScroll = ((field4 != null) ? field4.GetValue(this.groupItemList) : null) as CardStyleGeneralScroll;
			bool flag2 = cardScroll != null;
			if (flag2)
			{
				cardScroll.SetSortController(newController);
			}
		}

		// Token: 0x0600B6C6 RID: 46790 RVA: 0x00535039 File Offset: 0x00533239
		private void ItemListHostOnSortAndFilterChanged()
		{
			this.groupItemList.SetItemList(this._sourceData, -1);
			Action onSortAndFilterChangedCallback = this.groupItemList.OnSortAndFilterChangedCallback;
			if (onSortAndFilterChangedCallback != null)
			{
				onSortAndFilterChangedCallback();
			}
		}

		// Token: 0x0600B6C7 RID: 46791 RVA: 0x00535068 File Offset: 0x00533268
		private EEquipSubFilterKeys? ResolveEquipSubFilterKey()
		{
			EEquipSubFilterKeys fromSlot;
			bool flag = CombatRawCreateDestinationItemSortAndFilterController.TryMapEquipmentSlotToEquipSubFilter(this._equipmentSlotForFilter, out fromSlot);
			EEquipSubFilterKeys? result;
			if (flag)
			{
				result = new EEquipSubFilterKeys?(fromSlot);
			}
			else
			{
				bool flag2 = this._sourceData.Count == 0;
				if (flag2)
				{
					result = null;
				}
				else
				{
					sbyte itemType = this._sourceData[0].Key.ItemType;
					if (!true)
					{
					}
					EEquipSubFilterKeys? eequipSubFilterKeys;
					switch (itemType)
					{
					case 0:
						eequipSubFilterKeys = new EEquipSubFilterKeys?(EEquipSubFilterKeys.Weapon);
						break;
					case 1:
						eequipSubFilterKeys = new EEquipSubFilterKeys?(EEquipSubFilterKeys.Helm);
						break;
					case 2:
						eequipSubFilterKeys = new EEquipSubFilterKeys?(EEquipSubFilterKeys.Accessory);
						break;
					default:
						eequipSubFilterKeys = null;
						break;
					}
					if (!true)
					{
					}
					result = eequipSubFilterKeys;
				}
			}
			return result;
		}

		// Token: 0x0600B6C8 RID: 46792 RVA: 0x0053511C File Offset: 0x0053331C
		private void UpdateSortAndFilter()
		{
			bool prevWasValid = CombatRawCreateItemSelect.IsValidSelection(this.SelectedRawCreateItem);
			this.EnsureSortAndFilterController();
			this.EnsureRawCreateListColumnLayout();
			this.ApplyRawCreateFilterPanelTitle();
			SortAndFilterController<ITradeableContent> sortAndFilterController = this.groupItemList.SortAndFilterController;
			if (sortAndFilterController != null)
			{
				sortAndFilterController.NotifyDataChanged(this._sourceData);
			}
			this.groupItemList.SetItemList(this._sourceData);
			int selIdx = -1;
			bool flag = this.SelectedRawCreateItem.TemplateId >= 0;
			if (flag)
			{
				for (int i = 0; i < this.groupItemList.FilteredData.Count; i++)
				{
					ItemKey j = this.groupItemList.FilteredData[i].Key;
					bool flag2 = j.ItemType == this.SelectedRawCreateItem.ItemType && j.TemplateId == this.SelectedRawCreateItem.TemplateId;
					if (flag2)
					{
						selIdx = i;
						break;
					}
				}
			}
			this.groupItemList.SetItemList(this._sourceData, selIdx);
			bool flag3 = this.groupItemList.FilteredData.Count == 0;
			if (flag3)
			{
				this.SelectedRawCreateItem = new TemplateKey(-1, -1);
				bool hasSource = this._sourceData.Count > 0;
				this.groupItemList.gameObject.SetActive(hasSource);
			}
			else
			{
				this.groupItemList.gameObject.SetActive(true);
				int idx = this.FindFilteredIndex(this.SelectedRawCreateItem);
				bool flag4 = idx >= 0;
				if (flag4)
				{
					int flat = this.groupItemList.FindItemIndex(this.groupItemList.FilteredData[idx].Key);
					bool flag5 = flat >= 0;
					if (flag5)
					{
						this.groupItemList.InfiniteScroll.ScrollTo(flat, 0.3f);
					}
				}
				else
				{
					bool flag6 = prevWasValid;
					if (flag6)
					{
						this.OnSelect(CombatRawCreateItemSelect.ToTemplateKey(this.groupItemList.FilteredData[0]), true);
					}
				}
			}
		}

		// Token: 0x0600B6C9 RID: 46793 RVA: 0x0053530C File Offset: 0x0053350C
		private void OnRenderRowItem(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			TemplateKey tk = CombatRawCreateItemSelect.ToTemplateKey(itemData);
			rowItemLine.SetSelected(tk.ItemType == this.SelectedRawCreateItem.ItemType && tk.TemplateId == this.SelectedRawCreateItem.TemplateId);
		}

		// Token: 0x0600B6CA RID: 46794 RVA: 0x00535369 File Offset: 0x00533569
		private void OnClickRowItem(ITradeableContent content, RowItemLine _)
		{
			this.OnSelect(CombatRawCreateItemSelect.ToTemplateKey(content), true);
		}

		// Token: 0x0600B6CB RID: 46795 RVA: 0x0053537C File Offset: 0x0053357C
		private void OnSelect(TemplateKey key, bool call = true)
		{
			this.SelectedRawCreateItem = key;
			this.groupItemList.ReRender();
			if (call)
			{
				Action<TemplateKey> onSelect = this._onSelect;
				if (onSelect != null)
				{
					onSelect(key);
				}
			}
		}

		// Token: 0x0600B6CC RID: 46796 RVA: 0x005353B8 File Offset: 0x005335B8
		private int FindFilteredIndex(TemplateKey key)
		{
			bool flag = !CombatRawCreateItemSelect.IsValidSelection(key);
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				for (int i = 0; i < this.groupItemList.FilteredData.Count; i++)
				{
					TemplateKey tk = CombatRawCreateItemSelect.ToTemplateKey(this.groupItemList.FilteredData[i]);
					bool flag2 = tk.ItemType == key.ItemType && tk.TemplateId == key.TemplateId;
					if (flag2)
					{
						return i;
					}
				}
				result = -1;
			}
			return result;
		}

		// Token: 0x0600B6CD RID: 46797 RVA: 0x0053543F File Offset: 0x0053363F
		private static bool IsValidSelection(TemplateKey key)
		{
			return key.TemplateId >= 0;
		}

		// Token: 0x0600B6CE RID: 46798 RVA: 0x00535450 File Offset: 0x00533650
		private static TemplateKey ToTemplateKey(ITradeableContent data)
		{
			ItemKey key = data.Key;
			return new TemplateKey(key.ItemType, key.TemplateId);
		}

		// Token: 0x04008DF1 RID: 36337
		[SerializeField]
		private ItemListScroll groupItemList;

		// Token: 0x04008DF2 RID: 36338
		[SerializeField]
		private SortAndFilter sortAndFilterView;

		// Token: 0x04008DF3 RID: 36339
		private string _saveKey;

		// Token: 0x04008DF4 RID: 36340
		private sbyte _equipmentSlotForFilter = -1;

		// Token: 0x04008DF5 RID: 36341
		private readonly List<ITradeableContent> _sourceData = new List<ITradeableContent>();

		// Token: 0x04008DF6 RID: 36342
		private Action<TemplateKey> _onSelect;

		// Token: 0x04008DF7 RID: 36343
		[NonSerialized]
		public TemplateKey SelectedRawCreateItem;

		// Token: 0x04008DF8 RID: 36344
		private ItemListScroll.EColumnType? _appliedRawCreateColumnFlags;

		// Token: 0x04008DF9 RID: 36345
		private bool _accessoryTreasureLayoutActive;

		// Token: 0x04008DFA RID: 36346
		private const BindingFlags ItemListNonPublic = BindingFlags.Instance | BindingFlags.NonPublic;
	}
}
