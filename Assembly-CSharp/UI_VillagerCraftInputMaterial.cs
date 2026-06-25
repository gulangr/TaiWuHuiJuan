using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;

// Token: 0x02000413 RID: 1043
public class UI_VillagerCraftInputMaterial : UIBase
{
	// Token: 0x17000654 RID: 1620
	// (get) Token: 0x06003E26 RID: 15910 RVA: 0x001F2D0C File Offset: 0x001F0F0C
	private VillagerCraftPreviewPanel VillagerCraftPreviewPanel
	{
		get
		{
			return base.CGet<VillagerCraftPreviewPanel>("VillagerCraftPreviewPanel");
		}
	}

	// Token: 0x17000655 RID: 1621
	// (get) Token: 0x06003E27 RID: 15911 RVA: 0x001F2D19 File Offset: 0x001F0F19
	private ItemScrollView MaterialItemScrollView
	{
		get
		{
			return base.CGet<ItemScrollView>("ItemScrollView");
		}
	}

	// Token: 0x17000656 RID: 1622
	// (get) Token: 0x06003E28 RID: 15912 RVA: 0x001F2D26 File Offset: 0x001F0F26
	private CToggleGroupObsolete ItemSourceToggleGroup
	{
		get
		{
			return base.CGet<CToggleGroupObsolete>("ItemSourceToggleGroup");
		}
	}

	// Token: 0x17000657 RID: 1623
	// (get) Token: 0x06003E29 RID: 15913 RVA: 0x001F2D33 File Offset: 0x001F0F33
	private CButtonObsolete ButtonConfirm
	{
		get
		{
			return base.CGet<CButtonObsolete>("Confirm");
		}
	}

	// Token: 0x17000658 RID: 1624
	// (get) Token: 0x06003E2A RID: 15914 RVA: 0x001F2D40 File Offset: 0x001F0F40
	private CButtonObsolete ButtonCancel
	{
		get
		{
			return base.CGet<CButtonObsolete>("Cancel");
		}
	}

	// Token: 0x17000659 RID: 1625
	// (get) Token: 0x06003E2B RID: 15915 RVA: 0x001F2D4D File Offset: 0x001F0F4D
	private UI_VillagerCraftInputMaterial.ItemSourceTogKey CurItemSourceTogKey
	{
		get
		{
			return (UI_VillagerCraftInputMaterial.ItemSourceTogKey)this.ItemSourceToggleGroup.GetActive().Key;
		}
	}

	// Token: 0x1700065A RID: 1626
	// (get) Token: 0x06003E2C RID: 15916 RVA: 0x001F2D60 File Offset: 0x001F0F60
	private ItemSourceType CurItemSourceType
	{
		get
		{
			UI_VillagerCraftInputMaterial.ItemSourceTogKey curItemSourceTogKey = this.CurItemSourceTogKey;
			if (!true)
			{
			}
			ItemSourceType result;
			switch (curItemSourceTogKey)
			{
			case UI_VillagerCraftInputMaterial.ItemSourceTogKey.Inventory:
				result = ItemSourceType.Inventory;
				break;
			case UI_VillagerCraftInputMaterial.ItemSourceTogKey.Warehouse:
				result = ItemSourceType.Warehouse;
				break;
			case UI_VillagerCraftInputMaterial.ItemSourceTogKey.Treasury:
				result = ItemSourceType.Treasury;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!true)
			{
			}
			return result;
		}
	}

	// Token: 0x1700065B RID: 1627
	// (get) Token: 0x06003E2D RID: 15917 RVA: 0x001F2DA4 File Offset: 0x001F0FA4
	private List<ItemDisplayData> CurItems
	{
		get
		{
			UI_VillagerCraftInputMaterial.ItemSourceTogKey curItemSourceTogKey = this.CurItemSourceTogKey;
			if (!true)
			{
			}
			List<ItemDisplayData> result;
			switch (curItemSourceTogKey)
			{
			case UI_VillagerCraftInputMaterial.ItemSourceTogKey.Inventory:
				result = this._inventoryItems;
				break;
			case UI_VillagerCraftInputMaterial.ItemSourceTogKey.Warehouse:
				result = this._warehouseItems;
				break;
			case UI_VillagerCraftInputMaterial.ItemSourceTogKey.Treasury:
				result = this._treasuryItems;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!true)
			{
			}
			return result;
		}
	}

	// Token: 0x06003E2E RID: 15918 RVA: 0x001F2DF8 File Offset: 0x001F0FF8
	public override void OnInit(ArgumentBox argsBox)
	{
		this._inventoryItems.Clear();
		this._warehouseItems.Clear();
		this._treasuryItems.Clear();
		argsBox.Get<ArtisanOrder>("ArtisanOrder", out this._artisanOrder);
		argsBox.Get<ProductionPool>("ProductionPool", out this._originProductionPool);
		ECraftType currentCraftType;
		argsBox.Get<ECraftType>("CurrentCraftType", out currentCraftType);
		argsBox.Get("ItemSubType", out this._itemSubType);
		this._resourceType = UI_CraftsmanPanel.GetResourceTypeByCraftType(currentCraftType);
		this.VillagerCraftPreviewPanel.SetProductionPool(this._originProductionPool, null, this._itemSubType);
		this.MaterialItemScrollView.Init();
		this.MaterialItemScrollView.SetItemList(ref this._inventoryItems, true, "UI_VillagerCraftInputMaterial_Material", true, new Action<ItemDisplayData, ItemView>(this.OnRenderItem));
		int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		Dictionary<ItemSourceType, List<ItemDisplayData>> itemDict = new Dictionary<ItemSourceType, List<ItemDisplayData>>
		{
			{
				ItemSourceType.Inventory,
				this._inventoryItems
			},
			{
				ItemSourceType.Warehouse,
				this._warehouseItems
			},
			{
				ItemSourceType.Treasury,
				this._treasuryItems
			}
		};
		this._multiplyItemScrollView = base.CGet<MultiplyItemScrollView>("MultiplyItemScrollView");
		this._multiplyItemScrollView.Init(taiwuCharId, itemDict, null, null, new Action<List<ValueTuple<ItemDisplayData, int>>>(this.OnContentChange), true);
		this._multiplyItemScrollView.SetFilter(new Func<ItemDisplayData, bool>(this.MaterialFilter));
		this._multiplyItemScrollView.CurMultiplyScrollView.SortAndFilter.SetItemInteraction = new Func<ItemDisplayData, bool>(this.SetItemInteraction);
		this.ItemSourceToggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnItemSourceToggleChange);
		this.ItemSourceToggleGroup.InitPreOnToggle(-1);
		bool isSettlement = SingletonObject.getInstance<WorldMapModel>().GetTaiwuCharOnSettlement() > 0;
		this.ItemSourceToggleGroup.SetInteractable(true, UI_VillagerCraftInputMaterial.ItemSourceTogKey.Inventory.ToInt());
		this.ItemSourceToggleGroup.SetInteractable(isSettlement, UI_VillagerCraftInputMaterial.ItemSourceTogKey.Warehouse.ToInt());
		this.ItemSourceToggleGroup.SetInteractable(isSettlement, UI_VillagerCraftInputMaterial.ItemSourceTogKey.Treasury.ToInt());
		this.ItemSourceToggleGroup.SetToFirstInteractable(false);
		this.ItemSourceToggleGroup.GetAll().ForEach(delegate(CToggleObsolete t)
		{
			MonoJoint[] componentsInChildren = t.GetComponentsInChildren<MonoJoint>(true);
			if (componentsInChildren != null)
			{
				componentsInChildren.ForEach(delegate(int i, MonoJoint joint)
				{
					joint.JointSync();
					return false;
				});
			}
		});
		this.ButtonConfirm.ClearAndAddListener(new Action(this.OnClickConfirm));
		this.ButtonCancel.ClearAndAddListener(new Action(this.OnClickCancel));
		this.RefreshButtonConfirm();
		this.NeedDataListenerId = true;
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
		{
			CharacterDomainMethod.Call.GetAllInventoryItems(this.Element.GameDataListenerId, taiwuCharId);
			TaiwuDomainMethod.Call.GetAllWarehouseItems(this.Element.GameDataListenerId);
			TaiwuDomainMethod.Call.GetAllTreasuryItems(this.Element.GameDataListenerId);
		}));
	}

	// Token: 0x06003E2F RID: 15919 RVA: 0x001F309C File Offset: 0x001F129C
	private bool SetItemInteraction(ItemDisplayData itemData)
	{
		return this._originProductionPool.CanMaterialBeAdded(itemData.Key);
	}

	// Token: 0x06003E30 RID: 15920 RVA: 0x001F30C4 File Offset: 0x001F12C4
	private bool MaterialFilter(ItemDisplayData itemData)
	{
		bool flag = itemData.Key.ItemType != 5;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			MaterialItem materialConfig = Material.Instance[itemData.Key.TemplateId];
			bool flag2 = materialConfig.CraftableItemTypes.Count <= 0;
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool isMeetItemType = this._itemSubType < 0 || materialConfig.CraftableItemTypes.Any((short id) => MakeItemType.Instance[id].ItemSubType == this._itemSubType);
				bool flag3 = !isMeetItemType;
				if (flag3)
				{
					result = false;
				}
				else
				{
					bool isMeetResourceType = this._resourceType < 0 || materialConfig.ResourceType == this._resourceType;
					bool flag4 = !isMeetResourceType;
					result = !flag4;
				}
			}
		}
		return result;
	}

	// Token: 0x06003E31 RID: 15921 RVA: 0x001F3188 File Offset: 0x001F1388
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 1)
			{
				bool flag = notification.DomainId == 4;
				if (flag)
				{
					bool flag2 = notification.MethodId == 27;
					if (flag2)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._inventoryItems);
					}
				}
				else
				{
					bool flag3 = notification.DomainId == 5;
					if (flag3)
					{
						bool flag4 = notification.MethodId == 15;
						if (flag4)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._warehouseItems);
						}
						else
						{
							bool flag5 = notification.MethodId == 64;
							if (flag5)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._treasuryItems);
								this._multiplyItemScrollView.EnterVillagerCraftMode();
								this.Element.ShowAfterRefresh();
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06003E32 RID: 15922 RVA: 0x001F32B4 File Offset: 0x001F14B4
	private void OnItemSourceToggleChange(CToggleObsolete newTog, CToggleObsolete oldTog)
	{
		this._multiplyItemScrollView.InventoryItemSourceType = this.CurItemSourceType;
		this._multiplyItemScrollView.RefreshItems();
	}

	// Token: 0x06003E33 RID: 15923 RVA: 0x001F32D4 File Offset: 0x001F14D4
	private void OnRenderItem(ItemDisplayData itemData, ItemView itemView)
	{
		this._multiplyItemScrollView.OnRenderItemMultiply(itemData, itemView);
		itemView.SetLocked(!itemData.Interactable);
	}

	// Token: 0x06003E34 RID: 15924 RVA: 0x001F32F8 File Offset: 0x001F14F8
	private void OnContentChange([TupleElementNames(new string[]
	{
		"data",
		"count"
	})] List<ValueTuple<ItemDisplayData, int>> changeList = null)
	{
		this.RefreshButtonConfirm();
		bool flag = changeList == null;
		if (!flag)
		{
			bool flag2 = this._multiplyItemScrollView.SelectedMultiplyItemDict.Count == 0;
			if (flag2)
			{
				this.VillagerCraftPreviewPanel.SetProductionPool(this._originProductionPool, null, this._itemSubType);
			}
			else
			{
				this._previewAddMaterials.Items.Clear();
				foreach (KeyValuePair<ItemDisplayData, int> keyValuePair in this._multiplyItemScrollView.SelectedMultiplyItemDict)
				{
					ItemDisplayData itemDisplayData;
					int num;
					keyValuePair.Deconstruct(out itemDisplayData, out num);
					ItemDisplayData itemData = itemDisplayData;
					int amount = num;
					Inventory keyList = itemData.GetOperationInventoryFromPool(amount, false);
					this._previewAddMaterials.OfflineAdd(keyList);
					ItemDisplayData.ReturnInventoryToPool(keyList);
				}
			}
		}
	}

	// Token: 0x06003E35 RID: 15925 RVA: 0x001F33DC File Offset: 0x001F15DC
	private void RefreshButtonConfirm()
	{
		this.ButtonConfirm.interactable = (this._multiplyItemScrollView.SelectedMultiplyItemDict.Count > 0);
	}

	// Token: 0x06003E36 RID: 15926 RVA: 0x001F3400 File Offset: 0x001F1600
	private void OnClickConfirm()
	{
		string title = LocalStringManager.Get(LanguageKey.LK_VillagerCraftInputMaterial_ConfirmDialog_Title);
		string content = LocalStringManager.Get(LanguageKey.LK_VillagerCraftInputMaterial_ConfirmDialog_Content);
		CommonUtils.ShowConfirmDialog(title, content, delegate
		{
			this._multiplyItemScrollView.OnItemMultiplyOperationConfirm(null);
			this.QuickHide();
		}, null, EDialogType.None);
	}

	// Token: 0x06003E37 RID: 15927 RVA: 0x001F343A File Offset: 0x001F163A
	private void OnClickCancel()
	{
		this.QuickHide();
	}

	// Token: 0x06003E38 RID: 15928 RVA: 0x001F3444 File Offset: 0x001F1644
	private void OnDisable()
	{
		this._multiplyItemScrollView.ExitMultiplyMode();
	}

	// Token: 0x04002CD8 RID: 11480
	private MultiplyItemScrollView _multiplyItemScrollView;

	// Token: 0x04002CD9 RID: 11481
	private List<ItemDisplayData> _inventoryItems = new List<ItemDisplayData>();

	// Token: 0x04002CDA RID: 11482
	private List<ItemDisplayData> _warehouseItems = new List<ItemDisplayData>();

	// Token: 0x04002CDB RID: 11483
	private List<ItemDisplayData> _treasuryItems = new List<ItemDisplayData>();

	// Token: 0x04002CDC RID: 11484
	private ArtisanOrder _artisanOrder;

	// Token: 0x04002CDD RID: 11485
	private short _itemSubType;

	// Token: 0x04002CDE RID: 11486
	private readonly Inventory _previewAddMaterials = new Inventory();

	// Token: 0x04002CDF RID: 11487
	private ProductionPool _previewProductionPool = new ProductionPool();

	// Token: 0x04002CE0 RID: 11488
	private ProductionPool _originProductionPool;

	// Token: 0x04002CE1 RID: 11489
	private sbyte _resourceType;

	// Token: 0x020018B3 RID: 6323
	private enum ItemSourceTogKey
	{
		// Token: 0x0400AFCF RID: 45007
		Inventory,
		// Token: 0x0400AFD0 RID: 45008
		Warehouse,
		// Token: 0x0400AFD1 RID: 45009
		Treasury
	}
}
