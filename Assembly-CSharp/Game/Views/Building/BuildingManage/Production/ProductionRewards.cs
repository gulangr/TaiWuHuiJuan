using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FrameWork;
using Game.Views.Select;
using GameData.Domains.Building;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Building.BuildingManage.Production
{
	// Token: 0x02000C1D RID: 3101
	public class ProductionRewards : MonoBehaviour, IProductionComponent
	{
		// Token: 0x06009D89 RID: 40329 RVA: 0x0049C5A0 File Offset: 0x0049A7A0
		public void Setup(IProductionHandler handler)
		{
			this._handler = handler;
		}

		// Token: 0x06009D8A RID: 40330 RVA: 0x0049C5AC File Offset: 0x0049A7AC
		public void Refresh()
		{
			this.layout.cellSize = this.layout.cellSize.SetY(this._handler.NeedResource ? this.cellHeightWithCost : this.cellHeightNormal);
			int slotCount = (this._handler.TemplateId == 47) ? GlobalConfig.Instance.FeastGiftCount : ((int)GameData.Domains.Building.SharedMethods.GetBuildingSlotCount(this._handler.TemplateId));
			for (int i = 0; i < slotCount; i++)
			{
				bool flag = i == this.productionRoot.childCount;
				if (flag)
				{
					Object.Instantiate<Transform>(this.productionRoot.GetChild(0), this.productionRoot);
				}
				ItemResourceButton itemResourceButton = this.productionRoot.GetChild(i).GetComponent<ItemResourceButton>();
				this.RefreshItem(i, itemResourceButton);
				bool flag2 = !itemResourceButton.gameObject.activeSelf;
				if (flag2)
				{
					itemResourceButton.gameObject.SetActive(true);
				}
			}
			for (int j = slotCount; j < this.productionRoot.childCount; j++)
			{
				GameObject child = this.productionRoot.GetChild(j).gameObject;
				bool activeSelf = child.activeSelf;
				if (activeSelf)
				{
					child.SetActive(false);
				}
			}
		}

		// Token: 0x06009D8B RID: 40331 RVA: 0x0049C6E4 File Offset: 0x0049A8E4
		private void RefreshItem(int i, ItemResourceButton itemResourceButton)
		{
			ProductionRewards.<>c__DisplayClass7_0 CS$<>8__locals1 = new ProductionRewards.<>c__DisplayClass7_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.i = i;
			CS$<>8__locals1.itemResourceButton = itemResourceButton;
			this.SetAsEmpty(CS$<>8__locals1.itemResourceButton);
			BuildingEarningsData earningsData = this._handler.Data.EarningsData;
			bool flag = this._handler.Data.Feast != null;
			if (flag)
			{
				ItemKey key2;
				bool flag2 = this._handler.Data.Feast.Gift.TryGetValue(CS$<>8__locals1.i, out key2) && key2.HasTemplate;
				if (flag2)
				{
					ProductionRewards.<>c__DisplayClass7_1 CS$<>8__locals2 = new ProductionRewards.<>c__DisplayClass7_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals2.data = null;
					bool flag3 = ItemTemplateHelper.IsMiscResource(key2.ItemType, key2.TemplateId);
					if (flag3)
					{
						CS$<>8__locals2.data = ItemDisplayData.CreateResource(ItemTemplateHelper.GetMiscResourceType(key2.ItemType, key2.TemplateId), this._handler.Data.Feast.GiftCount[CS$<>8__locals2.CS$<>8__locals1.i], SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
						CS$<>8__locals2.CS$<>8__locals1.itemResourceButton.SetResourceFunc(CS$<>8__locals2.data.ResourceType, CS$<>8__locals2.data.Amount, ItemResourceButton.ItemResourceButtonState.Reveive, null, null, new Action(CS$<>8__locals2.<RefreshItem>g__ReceiveFeast|3));
					}
					else
					{
						ItemDomainMethod.AsyncCall.GetItemDisplayData(this._handler.Async, key2, delegate(int offset, RawDataPool pool)
						{
							Serializer.Deserialize(pool, offset, ref CS$<>8__locals2.data);
							bool flag9 = CS$<>8__locals2.data != null;
							if (flag9)
							{
								CS$<>8__locals2.data.Amount = CS$<>8__locals2.CS$<>8__locals1.<>4__this._handler.Data.Feast.GiftCount[CS$<>8__locals2.CS$<>8__locals1.i];
								CS$<>8__locals2.CS$<>8__locals1.itemResourceButton.SetButtonFunc(CS$<>8__locals2.data, ItemResourceButton.ItemResourceButtonState.Reveive, null, null, new Action(base.<RefreshItem>g__ReceiveFeast|3));
							}
						});
					}
				}
			}
			else
			{
				List<ItemKey> list = (earningsData != null) ? earningsData.CollectionItemList : null;
				bool flag4 = list != null && list.Count > 0 && earningsData.CollectionItemList.CheckIndex(CS$<>8__locals1.i) && earningsData.CollectionItemList[CS$<>8__locals1.i].HasTemplate;
				if (flag4)
				{
					ItemKey key = earningsData.CollectionItemList[CS$<>8__locals1.i];
					ItemDomainMethod.AsyncCall.GetItemDisplayData(this._handler.Async, key, delegate(int offset, RawDataPool pool)
					{
						ItemDisplayData data = null;
						Serializer.Deserialize(pool, offset, ref data);
						bool flag9 = data != null;
						if (flag9)
						{
							ItemResourceButton itemResourceButton2 = CS$<>8__locals1.itemResourceButton;
							ItemDisplayData itemDisplayData = data;
							ItemResourceButton.ItemResourceButtonState btnState = (CS$<>8__locals1.<>4__this._handler.Data.BlockData.TemplateId != 222 || CS$<>8__locals1.<>4__this._handler.IsMoneyEnoughToBuy(data)) ? ItemResourceButton.ItemResourceButtonState.Reveive : ItemResourceButton.ItemResourceButtonState.LackOfMoney;
							Action add = null;
							Action change = null;
							Action receive;
							if ((receive = CS$<>8__locals1.<>9__5) == null)
							{
								receive = (CS$<>8__locals1.<>9__5 = delegate()
								{
									CS$<>8__locals1.<>4__this.ReceiveItem(CS$<>8__locals1.i, null);
								});
							}
							itemResourceButton2.SetButtonFunc(itemDisplayData, btnState, add, change, receive);
							bool flag10 = CS$<>8__locals1.<>4__this._handler.TemplateId == 222;
							if (flag10)
							{
								CS$<>8__locals1.itemResourceButton.AppendRemainTime(CS$<>8__locals1.<>4__this._handler.PawnShopRemainMonth((int)key.ModificationState));
								CS$<>8__locals1.itemResourceButton.AppendResourceCount(6, CS$<>8__locals1.<>4__this._handler.PawnShopCostMoney(data));
							}
						}
					});
				}
				else
				{
					List<IntPair> list2 = (earningsData != null) ? earningsData.CollectionResourceList : null;
					bool flag5 = list2 != null && list2.Count > 0 && earningsData.CollectionResourceList.CheckIndex(CS$<>8__locals1.i) && earningsData.CollectionResourceList[CS$<>8__locals1.i].First != -1;
					if (flag5)
					{
						int num;
						int num2;
						earningsData.CollectionResourceList[CS$<>8__locals1.i].Deconstruct(out num, out num2);
						int typeInt = num;
						int value = num2;
						sbyte type = (sbyte)typeInt;
						CS$<>8__locals1.itemResourceButton.SetResourceFunc(type, value, ItemResourceButton.ItemResourceButtonState.Reveive, null, null, delegate
						{
							CS$<>8__locals1.<>4__this.ReceiveResource(CS$<>8__locals1.i, null);
						});
					}
					else
					{
						list2 = ((earningsData != null) ? earningsData.RecruitLevelList : null);
						bool flag6 = list2 != null && list2.Count > 0 && earningsData.RecruitLevelList.CheckIndex(CS$<>8__locals1.i) && earningsData.RecruitLevelList[CS$<>8__locals1.i].First != -1;
						if (flag6)
						{
							int num;
							int num2;
							earningsData.RecruitLevelList[CS$<>8__locals1.i].Deconstruct(out num2, out num);
							int outdate = num;
							ExtraDomainMethod.AsyncCall.RequestRecruitCharacterData(this._handler.Async, this._handler.Key, CS$<>8__locals1.i, delegate(int offset, RawDataPool pool)
							{
								RecruitCharacterData data = new RecruitCharacterData();
								Serializer.Deserialize(pool, offset, ref data);
								ItemResourceButton itemResourceButton2 = CS$<>8__locals1.itemResourceButton;
								RecruitCharacterData data2 = data;
								ItemResourceButton.ItemResourceButtonState btnState = (CS$<>8__locals1.<>4__this._handler.TemplateId != 223 || CS$<>8__locals1.<>4__this._handler.IsAuthorityEnoughToRecruit(1)) ? ItemResourceButton.ItemResourceButtonState.ReveiveRecruit : ItemResourceButton.ItemResourceButtonState.LackOfMoneyRecruit;
								Action add = null;
								Action change = null;
								Action receive;
								if ((receive = CS$<>8__locals1.<>9__8) == null)
								{
									receive = (CS$<>8__locals1.<>9__8 = delegate()
									{
										CS$<>8__locals1.<>4__this.ReceiveRecruit(CS$<>8__locals1.i);
									});
								}
								Action reject;
								if ((reject = CS$<>8__locals1.<>9__9) == null)
								{
									reject = (CS$<>8__locals1.<>9__9 = delegate()
									{
										CS$<>8__locals1.<>4__this.RejectRecruit(CS$<>8__locals1.i);
									});
								}
								itemResourceButton2.SetRecruitFunc(data2, btnState, add, change, receive, reject);
								CS$<>8__locals1.itemResourceButton.AppendRemainTime(CS$<>8__locals1.<>4__this._handler.RecruitRemainMonth(outdate));
								bool flag9 = CS$<>8__locals1.<>4__this._handler.TemplateId == 223;
								if (flag9)
								{
									CS$<>8__locals1.itemResourceButton.AppendResourceCount(7, CS$<>8__locals1.<>4__this._handler.AuthorityToRecruit(1));
								}
							});
						}
						else
						{
							list = ((earningsData != null) ? earningsData.ShopSoldItemList : null);
							bool flag7 = list != null && list.Count > 0 && earningsData.ShopSoldItemList.CheckIndex(CS$<>8__locals1.i) && earningsData.ShopSoldItemList[CS$<>8__locals1.i].HasTemplate;
							if (flag7)
							{
								ItemDomainMethod.AsyncCall.GetItemDisplayData(this._handler.Async, earningsData.ShopSoldItemList[CS$<>8__locals1.i], delegate(int offset, RawDataPool pool)
								{
									ItemDisplayData data = null;
									Serializer.Deserialize(pool, offset, ref data);
									bool flag9 = data != null;
									if (flag9)
									{
										CS$<>8__locals1.itemResourceButton.SetSoldItemFunc(data, default(IntPair), ItemResourceButton.ItemResourceButtonState.Change, null, new Action(CS$<>8__locals1.<>4__this.OpenMultiSelectItemWindow), null);
									}
								});
							}
							else
							{
								list2 = ((earningsData != null) ? earningsData.ShopSoldItemEarnList : null);
								bool flag8 = list2 != null && list2.Count > 0 && earningsData.ShopSoldItemEarnList.CheckIndex(CS$<>8__locals1.i) && earningsData.ShopSoldItemEarnList[CS$<>8__locals1.i].First != -1;
								if (flag8)
								{
									CS$<>8__locals1.itemResourceButton.SetResourceFunc((sbyte)earningsData.ShopSoldItemEarnList[CS$<>8__locals1.i].First, earningsData.ShopSoldItemEarnList[CS$<>8__locals1.i].Second, ItemResourceButton.ItemResourceButtonState.Reveive, null, null, delegate
									{
										CS$<>8__locals1.<>4__this.ReceiveSold(CS$<>8__locals1.i, null);
									});
								}
								else
								{
									this.SetAsEmpty(CS$<>8__locals1.itemResourceButton);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06009D8C RID: 40332 RVA: 0x0049CBB4 File Offset: 0x0049ADB4
		private void SetAsEmpty(ItemResourceButton itemResourceButton)
		{
			itemResourceButton.SetAsEmpty();
			bool isSold = this._handler.IsSold;
			if (isSold)
			{
				itemResourceButton.SetButtonFunc(null, ItemResourceButton.ItemResourceButtonState.Add, new Action(this.OpenMultiSelectItemWindow), null, null);
			}
		}

		// Token: 0x06009D8D RID: 40333 RVA: 0x0049CBF0 File Offset: 0x0049ADF0
		private void OpenMultiSelectItemWindow()
		{
			IAsyncMethodRequestHandler async = this._handler.Async;
			BuildingEarningsData earningsData = this._handler.Data.EarningsData;
			IEnumerable<ItemKey> enumerable = (earningsData != null) ? earningsData.ShopSoldItemList : null;
			ItemDomainMethod.AsyncCall.GetItemDisplayDataListOptional(async, (from x in enumerable ?? Enumerable.Empty<ItemKey>()
			where x.HasTemplate
			select x).ToList<ItemKey>(), -1, -1, false, delegate(int offset, RawDataPool pool)
			{
				ProductionRewards.<>c__DisplayClass9_0 CS$<>8__locals1 = new ProductionRewards.<>c__DisplayClass9_0();
				CS$<>8__locals1.<>4__this = this;
				CS$<>8__locals1.data = new List<ItemDisplayData>();
				Serializer.Deserialize(pool, offset, ref CS$<>8__locals1.data);
				foreach (ItemDisplayData item in CS$<>8__locals1.data)
				{
					item.ItemSourceType = 10;
				}
				List<SelectedItemData> initialSelectedItems = (from x in CS$<>8__locals1.data
				select new SelectedItemData(x, 1)).ToList<SelectedItemData>();
				List<ItemDisplayData> itemList = new List<ItemDisplayData>(from x in initialSelectedItems
				select x.ItemData as ItemDisplayData into x
				where x != null
				select x);
				List<ItemDisplayData> itemListInventory = new List<ItemDisplayData>(itemList);
				List<ItemDisplayData> itemListWarehouse = new List<ItemDisplayData>(itemList);
				List<ItemDisplayData> itemListTreasury = new List<ItemDisplayData>(itemList);
				bool flag = this._handler.Data.WarehouseCanSoldItemList != null;
				if (flag)
				{
					itemListWarehouse.AddRange(this._handler.Data.WarehouseCanSoldItemList);
				}
				bool flag2 = this._handler.Data.InventoryCanSoldItemList != null;
				if (flag2)
				{
					itemListInventory.AddRange(this._handler.Data.InventoryCanSoldItemList);
				}
				bool flag3 = this._handler.Data.TreasuryCanSoldItemList != null;
				if (flag3)
				{
					itemListTreasury.AddRange(this._handler.Data.TreasuryCanSoldItemList);
				}
				SelectItemRules rules = new SelectItemRules();
				SelectItemsCallback callback = new SelectItemsCallback(CS$<>8__locals1.<OpenMultiSelectItemWindow>g__OnShopSoldItemSelected|5);
				string title = "";
				ESelectItemColumnType? columnFlags = new ESelectItemColumnType?(ESelectItemColumnType.IconAndName | ESelectItemColumnType.Amount | ESelectItemColumnType.Type | ESelectItemColumnType.Value | ESelectItemColumnType.Weight);
				int buildingSlotCount = (int)GameData.Domains.Building.SharedMethods.GetBuildingSlotCount(this._handler.TemplateId);
				BuildingEarningsData earningsData2 = this._handler.Data.EarningsData;
				int? num;
				if (earningsData2 == null)
				{
					num = null;
				}
				else
				{
					List<IntPair> shopSoldItemEarnList = earningsData2.ShopSoldItemEarnList;
					if (shopSoldItemEarnList == null)
					{
						num = null;
					}
					else
					{
						num = new int?(shopSoldItemEarnList.Count((IntPair x) => x.First != -1));
					}
				}
				int? num2 = num;
				SelectItemConfig config = SelectItemConfig.CreateMultipleSelectConfig(rules, callback, title, buildingSlotCount - num2.GetValueOrDefault(), -1, columnFlags);
				config.InitialSelectedItems = initialSelectedItems;
				config.AllowEmpty = true;
				config.ShowSelectedArea = true;
				config.OperationMode = ESelectItemOperationMode.Slot;
				config.CustomTextGenerator = null;
				config.ExternalItems = itemListInventory;
				config.ExternalTreasuryItems = itemListTreasury;
				config.ExternalWarehouseItems = itemListWarehouse;
				config.SplitSelectedAmountIntoSingleEntries = true;
				config.Rules.OnlyFromInventory = false;
				config.CheckSameByReferenceOnly = true;
				UIElement.SelectItem.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("SelectItemConfig", config));
				UIManager.Instance.MaskUI(UIElement.SelectItem);
			});
		}

		// Token: 0x06009D8E RID: 40334 RVA: 0x0049CC70 File Offset: 0x0049AE70
		private void ReceiveItem(int index, ItemDisplayData receiveItem = null)
		{
			IProductionHandler handler = this._handler;
			BuildingEarningsData earningsData = (handler != null) ? handler.Data.EarningsData : null;
			List<ItemKey> list = (earningsData != null) ? earningsData.CollectionItemList : null;
			bool flag = list == null || list.Count <= 0;
			if (!flag)
			{
				bool flag2 = !earningsData.CollectionItemList.CheckIndex(index);
				if (!flag2)
				{
					BuildingDomainMethod.AsyncCall.AcceptBuildingBlockCollectEarning(this._handler.Async, this._handler.Key, index, false, true, this._handler.TemplateId == 222, null);
					bool flag3 = receiveItem != null;
					if (flag3)
					{
						this._handler.ShowGetItemView(new List<ItemDisplayData>
						{
							receiveItem
						});
					}
					this._handler.Reload();
					this._handler.DataChanged = true;
				}
			}
		}

		// Token: 0x06009D8F RID: 40335 RVA: 0x0049CD44 File Offset: 0x0049AF44
		private void ReceiveResource(int index, ItemDisplayData receiveItem = null)
		{
			IProductionHandler handler = this._handler;
			BuildingEarningsData earningsData = (handler != null) ? handler.Data.EarningsData : null;
			List<IntPair> list = (earningsData != null) ? earningsData.CollectionResourceList : null;
			bool flag = list == null || list.Count <= 0;
			if (!flag)
			{
				bool flag2 = !earningsData.CollectionResourceList.CheckIndex(index);
				if (!flag2)
				{
					BuildingDomainMethod.AsyncCall.AcceptBuildingBlockCollectEarning(this._handler.Async, this._handler.Key, index, false, null);
					bool flag3 = receiveItem != null;
					if (flag3)
					{
						this._handler.ShowGetItemView(new List<ItemDisplayData>
						{
							receiveItem
						});
					}
					this._handler.Reload();
					this._handler.DataChanged = true;
				}
			}
		}

		// Token: 0x06009D90 RID: 40336 RVA: 0x0049CE00 File Offset: 0x0049B000
		private void ReceiveSold(int index, ItemDisplayData receiveItem = null)
		{
			IProductionHandler handler = this._handler;
			BuildingEarningsData earningsData = (handler != null) ? handler.Data.EarningsData : null;
			List<IntPair> list = (earningsData != null) ? earningsData.ShopSoldItemEarnList : null;
			bool flag = list == null || list.Count <= 0;
			if (!flag)
			{
				bool flag2 = !earningsData.ShopSoldItemEarnList.CheckIndex(index);
				if (!flag2)
				{
					BuildingDomainMethod.Call.ShopBuildingSoldItemReceive(this._handler.Key, index);
					bool flag3 = receiveItem != null;
					if (flag3)
					{
						this._handler.ShowGetItemView(new List<ItemDisplayData>
						{
							receiveItem
						});
					}
					this._handler.Reload();
					this._handler.DataChanged = true;
				}
			}
		}

		// Token: 0x06009D91 RID: 40337 RVA: 0x0049CEB0 File Offset: 0x0049B0B0
		private void ReceiveRecruit(int index)
		{
			IProductionHandler handler = this._handler;
			BuildingEarningsData earningsData = (handler != null) ? handler.Data.EarningsData : null;
			List<IntPair> list = (earningsData != null) ? earningsData.RecruitLevelList : null;
			bool flag = list == null || list.Count <= 0;
			if (!flag)
			{
				bool flag2 = !earningsData.RecruitLevelList.CheckIndex(index);
				if (!flag2)
				{
					BuildingDomainMethod.AsyncCall.AcceptBuildingBlockRecruitPeople(this._handler.Async, this._handler.Key, index, new AsyncMethodCallbackDelegate(this.ReceiveRecruitHandler));
				}
			}
		}

		// Token: 0x06009D92 RID: 40338 RVA: 0x0049CF3C File Offset: 0x0049B13C
		private void RejectRecruit(int index)
		{
			IProductionHandler handler = this._handler;
			BuildingEarningsData earningsData = (handler != null) ? handler.Data.EarningsData : null;
			List<IntPair> list = (earningsData != null) ? earningsData.RecruitLevelList : null;
			bool flag = list == null || list.Count <= 0;
			if (!flag)
			{
				bool flag2 = !earningsData.RecruitLevelList.CheckIndex(index);
				if (!flag2)
				{
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
					{
						Title = LanguageKey.LK_Building_RejectRecruitPeople_Cmd_Title.Tr(),
						Content = LanguageKey.LK_Building_RejectRecruitPeople_Cmd_Text.Tr(),
						Type = 1,
						Yes = delegate()
						{
							BuildingDomainMethod.Call.RejectBuildingBlockRecruitPeople(this._handler.Key, index);
							this._handler.Reload();
							this._handler.DataChanged = true;
						}
					}));
					UIManager.Instance.MaskUI(UIElement.Dialog);
				}
			}
		}

		// Token: 0x06009D93 RID: 40339 RVA: 0x0049D024 File Offset: 0x0049B224
		private void ReceiveRecruitHandler(int offset, RawDataPool pool)
		{
			int charId = -1;
			Serializer.Deserialize(pool, offset, ref charId);
			bool flag = charId < 0;
			if (!flag)
			{
				List<int> charIdList = new List<int>
				{
					charId
				};
				this._handler.ShowGetPeopleView(charIdList);
				this._handler.Reload();
				this._handler.DataChanged = true;
			}
		}

		// Token: 0x06009D96 RID: 40342 RVA: 0x0049D370 File Offset: 0x0049B570
		[CompilerGenerated]
		internal static void <OpenMultiSelectItemWindow>g__AddToChangeList|9_8(ItemKey item, sbyte itemSourceType, int amount, ref ProductionRewards.<>c__DisplayClass9_1 A_3)
		{
			if (!true)
			{
			}
			int num;
			switch (itemSourceType)
			{
			case 1:
				num = 1;
				goto IL_42;
			case 2:
				num = 3;
				goto IL_42;
			case 3:
				num = 5;
				goto IL_42;
			case 4:
				break;
			case 5:
				num = 7;
				goto IL_42;
			default:
				if (itemSourceType == 10)
				{
					num = 0;
					goto IL_42;
				}
				break;
			}
			num = -1;
			IL_42:
			if (!true)
			{
			}
			int operateType = num;
			bool flag = operateType == -1;
			if (!flag)
			{
				for (int i = 0; i < amount; i++)
				{
					A_3.changeKeyList.Add(item);
					A_3.changeOperateTypeList.Add(operateType);
				}
			}
		}

		// Token: 0x040079FB RID: 31227
		[SerializeField]
		private Transform productionRoot;

		// Token: 0x040079FC RID: 31228
		[SerializeField]
		private GridLayoutGroup layout;

		// Token: 0x040079FD RID: 31229
		[SerializeField]
		private float cellHeightNormal;

		// Token: 0x040079FE RID: 31230
		[SerializeField]
		private float cellHeightWithCost;

		// Token: 0x040079FF RID: 31231
		private IProductionHandler _handler;
	}
}
