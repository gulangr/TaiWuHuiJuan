using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Item;
using Game.Components.SortAndFilter.Item.Apply;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.Combat.Item
{
	// Token: 0x02000B3E RID: 2878
	public class ViewSetCombatQuickUseItemSlot : UIBase
	{
		// Token: 0x17000FA4 RID: 4004
		// (get) Token: 0x06008F26 RID: 36646 RVA: 0x0042BA32 File Offset: 0x00429C32
		private int TotalWisdomCount
		{
			get
			{
				return (int)(this._wisdomCount + this._specialWisdomCount);
			}
		}

		// Token: 0x17000FA5 RID: 4005
		// (get) Token: 0x06008F27 RID: 36647 RVA: 0x0042BA41 File Offset: 0x00429C41
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x17000FA6 RID: 4006
		// (get) Token: 0x06008F28 RID: 36648 RVA: 0x0042BA48 File Offset: 0x00429C48
		private int TotalValidCount
		{
			get
			{
				return this._tempSlotDataList.Count(delegate(CombatQuickUseItemSlotData d)
				{
					Inventory inventory = d.Inventory;
					return inventory != null && inventory.InventoryItemTotalCount > 0;
				});
			}
		}

		// Token: 0x17000FA7 RID: 4007
		// (get) Token: 0x06008F29 RID: 36649 RVA: 0x0042BA74 File Offset: 0x00429C74
		private bool IsMax
		{
			get
			{
				return this.TotalValidCount == 10;
			}
		}

		// Token: 0x06008F2A RID: 36650 RVA: 0x0042BA80 File Offset: 0x00429C80
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get<List<ItemDisplayData>>("ItemList", out this._itemList);
			argsBox.Get<List<CombatQuickUseItemSlotData>>("SlotList", out this._slotDataList);
			argsBox.Get("WisdomIcon", out this._wisdomIcon);
			argsBox.Get("CanEatMore", out this._canEatMore);
			argsBox.Get("CanUseSwordFragment", out this._canUseSwordFragment);
			argsBox.Get<sbyte[]>("WeaponTricks", out this._weaponTricks);
			argsBox.Get("CharId", out this._currentCharId);
			argsBox.Get("WisdomCount", out this._wisdomCount);
			argsBox.Get("SpecialWisdomCount", out this._specialWisdomCount);
			argsBox.Get<CharacterInjuryDisplayData>("InjuryData", out this._injuryData);
			int lineId = EFilterLine.MainFilter.ToInt();
			this.itemListScroll.CustomWisdomDataGenerator = new Func<ITradeableContent, IconAndTextCellData>(this.CustomWisdomDataGenerator);
			this.itemListScroll.Init("CombatUseItemPanelItemListScroll", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnItemRender), new Action<ITradeableContent, RowItemLine>(this.OnItemClick), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability | ItemListScroll.EColumnType.Wisdom, null, null, null);
			this.itemListScroll.SortAndFilterController.SetToggleVisible(lineId, CombatUseItemPanel.DefaultFilterTypes, false);
			this._tempSlotDataList.Clear();
			this._tempSlotDataList.AddRange(this._slotDataList);
			this._tempItemList.Clear();
			this._tempItemList.AddRange(from d in this._itemList
			where !this._slotDataList.Any(delegate(CombatQuickUseItemSlotData s)
			{
				Inventory inventory = s.Inventory;
				return inventory != null && inventory.InventoryItemTotalCount > 0 && d.ContainsItemKey(s.Inventory.Items.First<KeyValuePair<ItemKey, int>>().Key);
			})
			select d);
			this.itemListScroll.SetItemList(this._tempItemList);
			this.RefreshSlots();
			this.buttonClose.ClearAndAddListener(new Action(this.QuickHide));
		}

		// Token: 0x06008F2B RID: 36651 RVA: 0x0042BC30 File Offset: 0x00429E30
		public override void QuickHide()
		{
			CombatDomainMethod.Call.SetCombatQuickUseItemSlotData(this._tempSlotDataList);
			base.QuickHide();
		}

		// Token: 0x06008F2C RID: 36652 RVA: 0x0042BC48 File Offset: 0x00429E48
		private void RefreshSlots()
		{
			int totalValidCount = this.TotalValidCount;
			for (int index = 0; index < this.itemSlotArray.Length; index++)
			{
				ViewSetCombatQuickUseItemSlot.<>c__DisplayClass25_0 CS$<>8__locals1 = new ViewSetCombatQuickUseItemSlot.<>c__DisplayClass25_0();
				CombatQuickUseItemSlot item = this.itemSlotArray[index];
				CombatQuickUseItemSlotData slotData = this._tempSlotDataList[index];
				ViewSetCombatQuickUseItemSlot.<>c__DisplayClass25_0 CS$<>8__locals2 = CS$<>8__locals1;
				Inventory inventory = slotData.Inventory;
				CS$<>8__locals2.firstKey = ((inventory != null && inventory.InventoryItemTotalCount > 0) ? slotData.Inventory.Items.Keys.First<ItemKey>() : ItemKey.Invalid);
				ItemDisplayData itemData = (CS$<>8__locals1.firstKey == ItemKey.Invalid) ? null : this._itemList.Find((ItemDisplayData d) => d.ContainsItemKey(CS$<>8__locals1.firstKey));
				bool isShow = CS$<>8__locals1.firstKey.HasTemplate || index == totalValidCount;
				item.Refresh(index, isShow, true, CS$<>8__locals1.firstKey, itemData, this._wisdomIcon, string.Empty, new Action<int, ItemDisplayData>(this.OnClickButtonUse), null, null, null);
			}
		}

		// Token: 0x06008F2D RID: 36653 RVA: 0x0042BD4C File Offset: 0x00429F4C
		private IconAndTextCellData CustomWisdomDataGenerator(ITradeableContent content)
		{
			return new IconAndTextCellData(this._wisdomIcon, string.Format("X{0}", content.RealKey.GetConsumedFeatureMedals()), true, false, false, false);
		}

		// Token: 0x06008F2E RID: 36654 RVA: 0x0042BD88 File Offset: 0x00429F88
		private void OnItemRender(ITradeableContent content, RowItemLine rowItemLine)
		{
			ItemDisplayData itemData = (ItemDisplayData)content;
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			string itemTip = string.Empty;
			bool interactable = !this.IsMax && !itemData.IsLocked && this.CheckItemCanUse(itemData, out itemTip);
			rowItemLine.SetInteractable(interactable, true);
			rowItemLine.SetDisabled(!interactable);
			rowItemLine.SetSelected(false);
			bool flag = !interactable && !itemTip.IsNullOrEmpty();
			if (flag)
			{
				rowItemMain.SetInteractionStateLockText(itemTip);
			}
		}

		// Token: 0x06008F2F RID: 36655 RVA: 0x0042BE14 File Offset: 0x0042A014
		private bool CheckItemCanUse(ItemDisplayData itemData, out string itemTip)
		{
			itemTip = string.Empty;
			int costWisdom = itemData.RealKey.GetConsumedFeatureMedals();
			CombatSubProcessorCharacter processor;
			bool flag = this.Model.ProcessorCharacters.TryGetValue(this._currentCharId, out processor) && processor.UseItemCostNoWisdom;
			if (flag)
			{
				costWisdom = 0;
			}
			bool costEnough = costWisdom <= this.TotalWisdomCount;
			bool flag2 = !costEnough;
			return !flag2 && CombatUseItemPanel.GetInteractable(itemData, out itemTip, this._canEatMore, this._canUseSwordFragment, this._weaponTricks, this._injuryData);
		}

		// Token: 0x06008F30 RID: 36656 RVA: 0x0042BEA0 File Offset: 0x0042A0A0
		private void OnItemClick(ITradeableContent content, RowItemLine rowItemLine)
		{
			ItemDisplayData itemData = (ItemDisplayData)content;
			Inventory allItemKeysFromPool = itemData.GetAllInventoryFromPool();
			CombatQuickUseItemSlotData slotData = this._tempSlotDataList[this.TotalValidCount];
			CombatQuickUseItemSlotData combatQuickUseItemSlotData = slotData;
			if (combatQuickUseItemSlotData.Inventory == null)
			{
				combatQuickUseItemSlotData.Inventory = new Inventory();
			}
			slotData.Inventory.OfflineAdd(allItemKeysFromPool);
			ItemDisplayData.ReturnInventoryToPool(allItemKeysFromPool);
			this.RefreshSlots();
			this._tempItemList.Remove(itemData);
			this.itemListScroll.SetItemList(this._tempItemList);
		}

		// Token: 0x06008F31 RID: 36657 RVA: 0x0042BF20 File Offset: 0x0042A120
		private void OnClickButtonUse(int index, ItemDisplayData itemData)
		{
			this._tempSlotDataList.RemoveAt(index);
			this._tempSlotDataList.Add(new CombatQuickUseItemSlotData());
			this.RefreshSlots();
			bool flag = itemData != null;
			if (flag)
			{
				this._tempItemList.Add(itemData);
				this.itemListScroll.SetItemList(this._tempItemList);
			}
		}

		// Token: 0x04006D5E RID: 27998
		[SerializeField]
		private ItemListScroll itemListScroll;

		// Token: 0x04006D5F RID: 27999
		[SerializeField]
		private CombatQuickUseItemSlot[] itemSlotArray;

		// Token: 0x04006D60 RID: 28000
		[SerializeField]
		private CButton buttonClose;

		// Token: 0x04006D61 RID: 28001
		private List<ItemDisplayData> _itemList = new List<ItemDisplayData>();

		// Token: 0x04006D62 RID: 28002
		private readonly List<ItemDisplayData> _tempItemList = new List<ItemDisplayData>();

		// Token: 0x04006D63 RID: 28003
		private List<CombatQuickUseItemSlotData> _slotDataList;

		// Token: 0x04006D64 RID: 28004
		private readonly List<CombatQuickUseItemSlotData> _tempSlotDataList = new List<CombatQuickUseItemSlotData>();

		// Token: 0x04006D65 RID: 28005
		private string _wisdomIcon;

		// Token: 0x04006D66 RID: 28006
		private bool _canEatMore;

		// Token: 0x04006D67 RID: 28007
		private bool _canUseSwordFragment;

		// Token: 0x04006D68 RID: 28008
		private sbyte[] _weaponTricks;

		// Token: 0x04006D69 RID: 28009
		private int _currentCharId;

		// Token: 0x04006D6A RID: 28010
		private short _wisdomCount;

		// Token: 0x04006D6B RID: 28011
		private short _specialWisdomCount;

		// Token: 0x04006D6C RID: 28012
		private CharacterInjuryDisplayData _injuryData;
	}
}
