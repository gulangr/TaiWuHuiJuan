using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Select;
using GameData.Domains.Building;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000BF9 RID: 3065
	public class BuildingManageSubPageEntertain : BuildingManageSubPage
	{
		// Token: 0x17001073 RID: 4211
		// (get) Token: 0x06009BD5 RID: 39893 RVA: 0x0049022D File Offset: 0x0048E42D
		public List<short> UnlockedFeastTypes
		{
			get
			{
				return this._unlockedFeastTypes;
			}
		}

		// Token: 0x17001074 RID: 4212
		// (get) Token: 0x06009BD6 RID: 39894 RVA: 0x00490235 File Offset: 0x0048E435
		public BuildingBlockKey BlockKey
		{
			get
			{
				return this.ParentView.BlockKey;
			}
		}

		// Token: 0x17001075 RID: 4213
		// (get) Token: 0x06009BD7 RID: 39895 RVA: 0x00490242 File Offset: 0x0048E442
		public CDropdown DishDropdown
		{
			get
			{
				return this.dishDropdown;
			}
		}

		// Token: 0x06009BD8 RID: 39896 RVA: 0x0049024C File Offset: 0x0048E44C
		private bool IsSelectedTableDish(ITradeableContent selected, int slotIndex)
		{
			ITradeableContent tableDish;
			bool flag = selected == null || !this._dishes.TryGetValue(slotIndex, out tableDish);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = selected == tableDish;
				if (flag2)
				{
					result = true;
				}
				else
				{
					bool flag3 = selected.ItemSourceType >= 0;
					result = (!flag3 && selected.RealKey.Equals(this._feast.GetDish(slotIndex)));
				}
			}
			return result;
		}

		// Token: 0x06009BD9 RID: 39897 RVA: 0x004902BC File Offset: 0x0048E4BC
		private bool IsDishDurabilityConsumed(int dishIndex)
		{
			int durability = this._feast.DishDurability.GetValueOrDefault(dishIndex, 0);
			return durability > 0 && durability != GlobalConfig.Instance.FeastDurability;
		}

		// Token: 0x06009BDA RID: 39898 RVA: 0x004902F8 File Offset: 0x0048E4F8
		private void Awake()
		{
			for (int i = 0; i < this.dishes.childCount; i++)
			{
				this.dishes.GetChild(i).GetComponent<FeastDish>().Init(new Action(this.OnClickDish));
			}
			this.btnMenu.ClearAndAddListener(new Action(this.OnClickMenu));
			this.btnQuick.ClearAndAddListener(new Action(this.OnClickQuick));
			this.toggleAuto.onValueChanged.RemoveAllListeners();
			this.toggleAuto.onValueChanged.AddListener(new UnityAction<bool>(this.OnClickAuto));
			this.dishDropdown.onValueChanged.RemoveAllListeners();
			this.dishDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnChangeDropdown));
			this.btnClear.ClearAndAddListener(new Action(this.OnClickClear));
		}

		// Token: 0x06009BDB RID: 39899 RVA: 0x004903E7 File Offset: 0x0048E5E7
		public override void Refresh(BuildingManageDisplayData displayData)
		{
			this.DisplayData = displayData;
			this._dishSource.Clear();
			this.RequestFeastData();
		}

		// Token: 0x06009BDC RID: 39900 RVA: 0x00490404 File Offset: 0x0048E604
		private void RequestFeastData()
		{
			BuildingDomainMethod.AsyncCall.GetUnlockedFeastTypeList(null, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._unlockedFeastTypes);
				this.dishDropdown.ClearOptions();
				List<string> options = new List<string>
				{
					LanguageKey.LK_Building_Entertain_Dropdown_Invalid.Tr()
				};
				this.DropdownValueToDishType[0] = 0;
				this.DishTypeToDropdownValue[0] = 0;
				bool flag = this._unlockedFeastTypes != null;
				if (flag)
				{
					foreach (short id in this._unlockedFeastTypes)
					{
						bool flag2 = id != 0;
						if (flag2)
						{
							this.DishTypeToDropdownValue[id] = options.Count;
							this.DropdownValueToDishType[options.Count] = id;
							options.Add(Config.Feast.Instance[id].Name);
						}
					}
				}
				this.dishDropdown.AddOptions(options);
			});
			ExtraDomainMethod.AsyncCall.GetFeast(null, this.BlockKey, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._feast);
				this.RefreshAll();
			});
		}

		// Token: 0x06009BDD RID: 39901 RVA: 0x00490434 File Offset: 0x0048E634
		private void RefreshAll()
		{
			short type = (this._feast.TargetType < 0) ? 0 : this._feast.TargetType;
			this.dishDropdown.SetValueWithoutNotify(this.DishTypeToDropdownValue[type]);
			this.toggleAuto.SetIsOnWithoutNotify(this._feast.AutoRefill);
			for (int i = 0; i < this.dishes.childCount; i++)
			{
				this.dishes.GetChild(i).GetComponent<FeastDish>().Set(this._feast.GetDish(i), this._feast.DishDurability.GetValueOrDefault(i, 0));
			}
			FeastItem config = Config.Feast.Instance[this._feast.GetFeastType()];
			bool showTitle = config.TemplateId > 0;
			bool flag = showTitle;
			if (flag)
			{
				this.titleLabel.text = Config.Feast.Instance[this._feast.GetFeastType()].Name;
				TooltipInvoker tip = this.titleBack.GetComponent<TooltipInvoker>();
				TooltipInvoker tooltipInvoker = tip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				tip.RuntimeParam.Set("type", config.TemplateId);
			}
			this.titleBack.SetActive(showTitle);
			this.btnClear.gameObject.SetActive(type > 0);
		}

		// Token: 0x06009BDE RID: 39902 RVA: 0x00490592 File Offset: 0x0048E792
		private void OnClickMenu()
		{
			UIElement.BuildingFeastMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Parent", this));
			UIManager.Instance.MaskUI(UIElement.BuildingFeastMenu);
		}

		// Token: 0x06009BDF RID: 39903 RVA: 0x004905C0 File Offset: 0x0048E7C0
		private void OnClickQuick()
		{
			ExtraDomainMethod.Call.FeastQuickRefill(this.BlockKey);
			this.RequestFeastData();
		}

		// Token: 0x06009BE0 RID: 39904 RVA: 0x004905D6 File Offset: 0x0048E7D6
		private void OnClickAuto(bool isOn)
		{
			ExtraDomainMethod.Call.FeastSetAutoRefill(this.BlockKey, isOn);
		}

		// Token: 0x06009BE1 RID: 39905 RVA: 0x004905E6 File Offset: 0x0048E7E6
		private void OnClickClear()
		{
			ExtraDomainMethod.Call.FeastSetTargetType(this.BlockKey, -1);
			this.RequestFeastData();
		}

		// Token: 0x06009BE2 RID: 39906 RVA: 0x00490600 File Offset: 0x0048E800
		private void OnChangeDropdown(int value)
		{
			short type = this.DropdownValueToDishType[value];
			bool flag = type == 0;
			if (flag)
			{
				type = -1;
			}
			ExtraDomainMethod.Call.FeastSetTargetType(this.BlockKey, type);
			this.RequestFeastData();
		}

		// Token: 0x06009BE3 RID: 39907 RVA: 0x00490639 File Offset: 0x0048E839
		private void OnClickDish()
		{
			TaiwuDomainMethod.AsyncCall.GetAllDishes(null, delegate(int offset, RawDataPool pool)
			{
				SelectItemDisplayData data = null;
				Serializer.Deserialize(pool, offset, ref data);
				List<ItemDisplayData> itemList = new List<ItemDisplayData>();
				List<SelectedItemData> selected = new List<SelectedItemData>();
				this._dishes.Clear();
				for (int i = 0; i < GlobalConfig.Instance.FeastCount; i++)
				{
					ItemKey dish = this._feast.GetDish(i);
					bool flag = dish.IsValid();
					if (flag)
					{
						ItemDisplayData itemData = new ItemDisplayData(dish, 1)
						{
							SpecialArg = this._feast.DishDurability[i],
							ItemSourceType = -1
						};
						this._dishes[i] = itemData;
						selected.Add(new SelectedItemData(itemData, 1));
						itemList.Add(itemData);
					}
				}
				List<ItemDisplayData> itemListInventory = new List<ItemDisplayData>(itemList);
				List<ItemDisplayData> itemListWarehouse = new List<ItemDisplayData>(itemList);
				List<ItemDisplayData> itemListTreasury = new List<ItemDisplayData>(itemList);
				bool flag2 = data.InventoryItems != null;
				if (flag2)
				{
					foreach (ItemDisplayData item in data.InventoryItems)
					{
						item.SpecialArg = GlobalConfig.Instance.FeastDurability;
						itemListInventory.Add(item);
					}
				}
				bool flag3 = data.TreasuryItems != null;
				if (flag3)
				{
					foreach (ItemDisplayData item2 in data.TreasuryItems)
					{
						item2.SpecialArg = GlobalConfig.Instance.FeastDurability;
						itemListTreasury.Add(item2);
					}
				}
				bool flag4 = data.WarehouseItems != null;
				if (flag4)
				{
					foreach (ItemDisplayData item3 in data.WarehouseItems)
					{
						item3.SpecialArg = GlobalConfig.Instance.FeastDurability;
						itemListWarehouse.Add(item3);
					}
				}
				SelectItemConfig config = SelectItemConfig.CreateMultipleSelectConfig(new SelectItemRules(), new SelectItemsCallback(this.CheckSelected), "", 0, -1, new ESelectItemColumnType?(ESelectItemColumnType.IconAndName | ESelectItemColumnType.Amount | ESelectItemColumnType.Type | ESelectItemColumnType.Value | ESelectItemColumnType.Weight | ESelectItemColumnType.DishDurability));
				config.MaxSelectCount = GlobalConfig.Instance.FeastCount;
				config.InitialSelectedItems = selected;
				config.AllowEmpty = true;
				config.ShowSelectedArea = true;
				config.OperationMode = ESelectItemOperationMode.Slot;
				config.CustomTextGenerator = new Func<IReadOnlyList<SelectedItemData>, string>(this.OnSelectionChange);
				config.CustomTextToolTipSetter = new Action<IReadOnlyList<SelectedItemData>, TooltipInvoker>(this.OnSelectionChangeToolTip);
				config.ExternalItems = itemListInventory;
				config.ExternalWarehouseItems = itemListWarehouse;
				config.ExternalTreasuryItems = itemListTreasury;
				config.SplitSelectedAmountIntoSingleEntries = true;
				config.DisableWhenMaxSelected = true;
				UIElement.SelectItem.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("SelectItemConfig", config));
				UIManager.Instance.MaskUI(UIElement.SelectItem);
			});
		}

		// Token: 0x06009BE4 RID: 39908 RVA: 0x00490650 File Offset: 0x0048E850
		private static short GetFeastTypeFromSelected(IReadOnlyList<SelectedItemData> selected)
		{
			GameData.Domains.Building.Feast tempFeast = new GameData.Domains.Building.Feast
			{
				Dish = new Dictionary<int, ItemKey>()
			};
			int index = 0;
			bool flag = selected != null;
			if (flag)
			{
				foreach (SelectedItemData data in selected)
				{
					tempFeast.Dish.Add(index++, data.ItemData.Key);
				}
			}
			return tempFeast.GetFeastType();
		}

		// Token: 0x06009BE5 RID: 39909 RVA: 0x004906DC File Offset: 0x0048E8DC
		private string OnSelectionChange(IReadOnlyList<SelectedItemData> selected)
		{
			short type = BuildingManageSubPageEntertain.GetFeastTypeFromSelected(selected);
			return (type > 0) ? LocalStringManager.GetFormat(LanguageKey.LK_Building_Entertain_FeastSelecting, LanguageKey.LK_Building_Entertain_Selecting.Tr(), Config.Feast.Instance[type].Name) : LanguageKey.LK_Entertain_Default_Tip.Tr();
		}

		// Token: 0x06009BE6 RID: 39910 RVA: 0x0049072C File Offset: 0x0048E92C
		private void OnSelectionChangeToolTip(IReadOnlyList<SelectedItemData> selected, TooltipInvoker tip)
		{
			short type = BuildingManageSubPageEntertain.GetFeastTypeFromSelected(selected);
			bool flag = type > 0;
			if (flag)
			{
				tip.enabled = true;
				tip.Type = TipType.BuildingFeast;
				if (tip.RuntimeParam == null)
				{
					tip.RuntimeParam = new ArgumentBox();
				}
				tip.RuntimeParam.Set("type", type);
				bool showing = tip.Showing;
				if (showing)
				{
					tip.Refresh(false, -1);
				}
			}
			else
			{
				bool showing2 = tip.Showing;
				if (showing2)
				{
					tip.HideTips();
				}
				tip.enabled = false;
			}
		}

		// Token: 0x06009BE7 RID: 39911 RVA: 0x004907BC File Offset: 0x0048E9BC
		private void CheckSelected(List<SelectedItemData> selected)
		{
			List<SelectedItemData> selectedForCheck = (selected == null) ? new List<SelectedItemData>() : new List<SelectedItemData>(selected);
			Action <>9__0;
			for (int i = 0; i < GlobalConfig.Instance.FeastCount; i++)
			{
				bool flag = this._feast.GetDish(i).IsValid();
				if (flag)
				{
					bool found = false;
					foreach (SelectedItemData data in selectedForCheck)
					{
						bool flag2 = this.IsSelectedTableDish(data.ItemData, i);
						if (flag2)
						{
							found = true;
							selectedForCheck.Remove(data);
							break;
						}
					}
					bool flag3 = !found && this.IsDishDurabilityConsumed(i);
					if (flag3)
					{
						DialogCmd dialogCmd = new DialogCmd();
						dialogCmd.Title = LanguageKey.LK_Building_Entertain_Warn_RemoveDish_Title.Tr();
						dialogCmd.Content = LanguageKey.LK_Building_Entertain_Warn_RemoveDish_Content.Tr();
						Action yes;
						if ((yes = <>9__0) == null)
						{
							yes = (<>9__0 = delegate()
							{
								this.OnConfirmDish(selected);
							});
						}
						dialogCmd.Yes = yes;
						DialogCmd dialog = dialogCmd;
						ArgumentBox argBox = EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialog);
						UIElement.Dialog.SetOnInitArgs(argBox);
						UIManager.Instance.MaskUI(UIElement.Dialog);
						return;
					}
				}
			}
			this.OnConfirmDish(selected);
		}

		// Token: 0x06009BE8 RID: 39912 RVA: 0x00490948 File Offset: 0x0048EB48
		private void OnConfirmDish(List<SelectedItemData> selected)
		{
			for (int i = 0; i < GlobalConfig.Instance.FeastCount; i++)
			{
				bool flag = this._feast.GetDish(i).IsValid();
				if (flag)
				{
					SelectedItemData matched = null;
					foreach (SelectedItemData data in selected)
					{
						bool flag2 = this.IsSelectedTableDish(data.ItemData, i);
						if (flag2)
						{
							matched = data;
							break;
						}
					}
					bool flag3 = matched != null;
					if (flag3)
					{
						selected.Remove(matched);
					}
					else
					{
						ViewSelectItem selectMenu = UIElement.CharacterMenu.UiBaseAs<ViewSelectItem>();
						ItemSourceType source = this._dishSource.GetValueOrDefault(i, (selectMenu != null) ? selectMenu.SelectedSourceType : ItemSourceType.Invalid);
						ExtraDomainMethod.Call.FeastRemoveDish(this.BlockKey, i, source);
						this._feast.Dish[i] = ItemKey.Invalid;
					}
				}
			}
			foreach (SelectedItemData data2 in selected)
			{
				bool flag4 = data2.ItemData.ItemSourceType < 0;
				if (!flag4)
				{
					for (int j = 0; j < data2.SelectedAmount; j++)
					{
						for (int k = 0; k < GlobalConfig.Instance.FeastCount; k++)
						{
							bool flag5 = !this._feast.GetDish(k).IsValid();
							if (flag5)
							{
								ItemSourceType source2 = (ItemSourceType)data2.ItemData.ItemSourceType;
								ExtraDomainMethod.Call.FeastAddDish(this.BlockKey, k, data2.ItemData.RealKey, source2);
								this._dishSource[k] = source2;
								this._feast.Dish[k] = data2.ItemData.RealKey;
								break;
							}
						}
					}
				}
			}
			this.RequestFeastData();
		}

		// Token: 0x040078BB RID: 30907
		[SerializeField]
		private GameObject titleBack;

		// Token: 0x040078BC RID: 30908
		[SerializeField]
		private TextMeshProUGUI titleLabel;

		// Token: 0x040078BD RID: 30909
		[SerializeField]
		private Transform dishes;

		// Token: 0x040078BE RID: 30910
		[SerializeField]
		private CButton btnMenu;

		// Token: 0x040078BF RID: 30911
		[SerializeField]
		private CButton btnQuick;

		// Token: 0x040078C0 RID: 30912
		[SerializeField]
		private CToggle toggleAuto;

		// Token: 0x040078C1 RID: 30913
		[SerializeField]
		private CDropdown dishDropdown;

		// Token: 0x040078C2 RID: 30914
		[SerializeField]
		private CButton btnClear;

		// Token: 0x040078C3 RID: 30915
		private List<short> _unlockedFeastTypes = new List<short>();

		// Token: 0x040078C4 RID: 30916
		private GameData.Domains.Building.Feast _feast;

		// Token: 0x040078C5 RID: 30917
		private Dictionary<int, ITradeableContent> _dishes = new Dictionary<int, ITradeableContent>();

		// Token: 0x040078C6 RID: 30918
		public Dictionary<short, int> DishTypeToDropdownValue = new Dictionary<short, int>();

		// Token: 0x040078C7 RID: 30919
		public Dictionary<int, short> DropdownValueToDishType = new Dictionary<int, short>();

		// Token: 0x040078C8 RID: 30920
		private Dictionary<int, ItemSourceType> _dishSource = new Dictionary<int, ItemSourceType>();
	}
}
