using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Building.RecordBase;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Item.Apply;
using Game.Views.Migrate;
using GameData.Common;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.LifeRecord;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Buildings
{
	// Token: 0x02000BC5 RID: 3013
	public class ViewTeaHorseCaravan : UIBase
	{
		// Token: 0x17001043 RID: 4163
		// (get) Token: 0x060097D0 RID: 38864 RVA: 0x0046B674 File Offset: 0x00469874
		private BuildingModel BuildingModel
		{
			get
			{
				return SingletonObject.getInstance<BuildingModel>();
			}
		}

		// Token: 0x060097D1 RID: 38865 RVA: 0x0046B67C File Offset: 0x0046987C
		public override void OnInit(ArgumentBox argsBox)
		{
			this._blockSelf = (this._blockExchange = false);
			this.carryingItems.Init("TeaHorseCarryingItem", ESortAndFilterControllerType.Empty, true, new Action<ITradeableContent, RowItemLine>(this.OnRenderCarryGoods), null, ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability, null, null, null);
			this.goodsItems.Init("TeaHorseGoodsItem", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnRenderGoodsItem), null, ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability, null, null, null);
			this.selfItems.Init("TeaHorseSelfItem", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnRenderSelfItem), null, ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability, null, null, null);
			this.carryingItems.SetTableHeadSortEnabled(false);
			this.exchangeGoodsItems.Init("TeaHorseReceivedItem", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnRenderReceivedItem), null, ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability, null, null, null);
			this.carryItemToExchangingItems.Init("TeaHorseExchangingItem", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnRenderExchangingItem), null, ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability, null, null, null);
			bool flag = !this.carryItemToExchangingItems.IsCardMode;
			if (flag)
			{
				this.carryItemToExchangingItems.SwitchCardMode();
			}
			this.OnLanguageChange(LocalStringManager.CurLanguageType);
			this.diaryBtn.gameObject.SetActive(true);
			this.diaryInfoPanel.SetActive(false);
			this.exchangeItemPanel.SetActive(false);
			argsBox.Get<BuildingBlockData>("BuildingBlockData", out this._buildingBlockData);
			argsBox.Get<BuildingBlockKey>("BuildingBlockKey", out this._buildingBlocKey);
			sbyte buildingLevel = this.BuildingModel.GetBuildingLevel(this._buildingBlocKey, this._buildingBlockData);
			BuildingScaleItem config = BuildingScale.Instance[116];
			this._carryItemCount = config.LevelEffect[(int)(buildingLevel - 1)];
			List<int> levelEffect = config.LevelEffect;
			this._maxCarryItemCount = levelEffect[levelEffect.Count - 1];
			this._inventoryItems.Clear();
			this._equipItems.Clear();
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
			bool flag2 = !this._firstEnter;
			if (flag2)
			{
				this._firstEnter = true;
				GlobalDomainMethod.Call.InvokeGuidingTrigger(129);
			}
		}

		// Token: 0x060097D2 RID: 38866 RVA: 0x0046B894 File Offset: 0x00469A94
		private void Awake()
		{
			this.selfTogGroup.OnActiveIndexChange += this.OnToggleChange;
			this.selfTogGroup.Init(-1);
			this.goodsTogGroup.OnActiveIndexChange += this.OnGoodsToggleChange;
			this.goodsTogGroup.Init(-1);
		}

		// Token: 0x060097D3 RID: 38867 RVA: 0x0046B8EC File Offset: 0x00469AEC
		private void OnDisable()
		{
			this.ClearExchangePanel(false);
			this._isFirstOpen = true;
			GEvent.OnEvent(UiEvents.UpdateAllBlockInfo, null);
			this.ClearAudio();
		}

		// Token: 0x060097D4 RID: 38868 RVA: 0x0046B914 File Offset: 0x00469B14
		private void ClearAudio()
		{
			foreach (string audioName in this._caravanAudioList)
			{
				AudioManager.Instance.StopAllSound(audioName);
			}
			foreach (string audioName2 in this._weatherAudioList)
			{
				AudioManager.Instance.StopAllSound(audioName2);
			}
			foreach (string audioName3 in this._weatherThunderAudioList)
			{
				AudioManager.Instance.StopAllSound(audioName3);
			}
			AudioManager.Instance.StopAmbienceWithFade(1f);
		}

		// Token: 0x060097D5 RID: 38869 RVA: 0x0046BA1C File Offset: 0x00469C1C
		public override void InitMonitorFieldIds()
		{
			int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)taiwuCharId, new uint[]
			{
				104U,
				103U
			}));
			this.MonitorFields.Add(new UIBase.MonitorDataField(5, 8, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(5, 7, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(9, 19, ulong.MaxValue, null));
		}

		// Token: 0x060097D6 RID: 38870 RVA: 0x0046BAA0 File Offset: 0x00469CA0
		private void OnListenerIdReady()
		{
			this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			CharacterDomainMethod.Call.GetCharacterDisplayDataList(this.Element.GameDataListenerId, new List<int>
			{
				SingletonObject.getInstance<BasicGameData>().TaiwuCharId
			});
			TaiwuDomainMethod.Call.CanTransferItemToWarehouse(this.Element.GameDataListenerId);
			this.RefreshTaiwuItems();
		}

		// Token: 0x060097D7 RID: 38871 RVA: 0x0046BB00 File Offset: 0x00469D00
		private void RefreshTaiwuItems()
		{
			TaiwuDomainMethod.Call.GetAllWarehouseItems(this.Element.GameDataListenerId);
			TaiwuDomainMethod.Call.GetAllTreasuryItems(this.Element.GameDataListenerId);
			TaiwuDomainMethod.Call.GetAllItems(this.Element.GameDataListenerId, ItemSourceType.Stock);
			CharacterDomainMethod.Call.GetAllInventoryItems(this.Element.GameDataListenerId, this._taiwuCharId);
			CharacterDomainMethod.Call.GetAllEquipmentItems(this.Element.GameDataListenerId, this._taiwuCharId);
		}

		// Token: 0x060097D8 RID: 38872 RVA: 0x0046BB70 File Offset: 0x00469D70
		public override void QuickHide()
		{
			AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			UIManager.Instance.HideUI(this.Element);
		}

		// Token: 0x060097D9 RID: 38873 RVA: 0x0046BB98 File Offset: 0x00469D98
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b != 0)
				{
					if (b == 1)
					{
						bool flag = notification.DomainId == 5;
						if (flag)
						{
							bool flag2 = notification.MethodId == 42;
							if (flag2)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._canTransfer);
							}
							else
							{
								bool flag3 = notification.MethodId == 15;
								if (flag3)
								{
									List<ItemDisplayData> dataList = new List<ItemDisplayData>();
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref dataList);
									this.stockDic[1] = (dataList ?? new List<ItemDisplayData>());
									int index = 0;
									foreach (ItemDisplayData data in this.stockDic[1])
									{
										data.ItemShopLevel = index++;
									}
									this.UpdateSelfItem(false);
								}
								else
								{
									bool flag4 = notification.MethodId == 64;
									if (flag4)
									{
										List<ItemDisplayData> dataList2 = new List<ItemDisplayData>();
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref dataList2);
										this.stockDic[2] = (dataList2 ?? new List<ItemDisplayData>());
										int index2 = 0;
										foreach (ItemDisplayData data2 in this.stockDic[2])
										{
											data2.ItemShopLevel = index2++;
										}
									}
									else
									{
										bool flag5 = notification.MethodId == 118;
										if (flag5)
										{
											ValueTuple<ItemSourceType, List<ItemDisplayData>> tuple = default(ValueTuple<ItemSourceType, List<ItemDisplayData>>);
											Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref tuple);
											this.stockDic[3] = (tuple.Item2 ?? new List<ItemDisplayData>());
											int index3 = 0;
											foreach (ItemDisplayData data3 in this.stockDic[3])
											{
												data3.ItemShopLevel = index3++;
											}
										}
									}
								}
							}
						}
						else
						{
							bool flag6 = notification.DomainId == 4;
							if (flag6)
							{
								bool flag7 = notification.MethodId == 48;
								if (flag7)
								{
									List<CharacterDisplayData> displayDataList = null;
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref displayDataList);
									this._taiwuDisplayData = displayDataList[0];
								}
								else
								{
									bool flag8 = notification.MethodId == 27;
									if (flag8)
									{
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._inventoryItems);
										if (this._inventoryItems == null)
										{
											this._inventoryItems = new List<ItemDisplayData>();
										}
									}
									else
									{
										bool flag9 = notification.MethodId == 29;
										if (flag9)
										{
											Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._equipItems);
											foreach (ItemDisplayData item in this._equipItems)
											{
												bool flag10 = item.Key.IsValid();
												if (flag10)
												{
													this._inventoryItems.Add(item.Clone(-1));
												}
											}
											this.stockDic[0] = this._inventoryItems;
											int index4 = 0;
											foreach (ItemDisplayData data4 in this.stockDic[0])
											{
												data4.ItemShopLevel = index4++;
											}
											List<ItemDisplayData> items = this.stockDic[this.selfTogGroup.GetActiveIndex()];
											this.selfItems.SetItemList(items);
											this._blockSelf = false;
											TMP_Text tmp_Text = this.awarenessText;
											LanguageKey languageKey = LanguageKey.LK_TeaHorse_Awareness;
											TeaHorseCaravanData teaHorseCaravanData = this._teaHorseCaravanData;
											tmp_Text.text = languageKey.TrFormat((teaHorseCaravanData != null) ? teaHorseCaravanData.CaravanAwareness : 100);
											this.Element.ShowAfterRefresh();
										}
									}
								}
							}
						}
					}
				}
				else
				{
					DataUid uid = notification.Uid;
					bool flag11 = uid.DomainId == 4;
					if (flag11)
					{
						bool flag12 = uid.SubId1 == 104U;
						if (flag12)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._inventoryCurrLoad);
						}
						else
						{
							bool flag13 = uid.SubId1 == 103U;
							if (flag13)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._inventoryMaxLoad);
							}
						}
					}
					else
					{
						bool flag14 = uid.DomainId == 5;
						if (flag14)
						{
							bool flag15 = uid.DataId == 8 || uid.DataId == 7;
							if (flag15)
							{
								bool flag16 = uid.DataId == 8;
								if (flag16)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._wareHouseCurrLoad);
								}
								else
								{
									bool flag17 = uid.DataId == 7;
									if (flag17)
									{
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._wareHouseMaxLoad);
									}
								}
								string currLoad = ((float)this._wareHouseCurrLoad / 100f).ToString("f1").SetColor(CommonUtils.GetLoadWeightValueColor(this._wareHouseCurrLoad, this._wareHouseMaxLoad));
								this.warehouseLoad.text = string.Format("{0}/{1:f1}", currLoad, (float)this._wareHouseMaxLoad / 100f);
							}
						}
						else
						{
							bool flag18 = uid.DomainId == 9 && uid.DataId == 19;
							if (flag18)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._teaHorseCaravanData);
								bool flag19;
								if (this._teaHorseCaravanData.CaravanState == 4)
								{
									List<ItemKey> exchangeGoodsList = this._teaHorseCaravanData.ExchangeGoodsList;
									flag19 = (exchangeGoodsList == null || exchangeGoodsList.Count <= 0);
								}
								else
								{
									flag19 = false;
								}
								bool flag20 = flag19;
								if (flag20)
								{
									this._teaHorseCaravanData.CaravanState = 1;
									BuildingDomainMethod.Call.QuickGetExchangeItem();
								}
								this.UpdateExchangeItem();
								this.SetWeatherVFX();
								this.SetAudio();
							}
						}
					}
				}
			}
		}

		// Token: 0x060097DA RID: 38874 RVA: 0x0046C280 File Offset: 0x0046A480
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = btnName == "Close";
			if (flag)
			{
				base.QuickHide();
			}
			else
			{
				bool flag2 = btnName == "StartBtn";
				if (flag2)
				{
					this.quickCollectCarryingItem.gameObject.SetActive(false);
					bool flag3 = this._teaHorseCaravanData.CaravanState < 1;
					if (flag3)
					{
						bool flag4 = this._teaHorseCaravanData.CarryGoodsList.Count > 0;
						if (flag4)
						{
							BuildingDomainMethod.Call.SetTeaHorseCaravanState(1);
						}
					}
					else
					{
						BuildingDomainMethod.Call.SetTeaHorseCaravanState(2);
					}
				}
				else
				{
					bool flag5 = btnName == "ReturnBtn";
					if (flag5)
					{
						BuildingDomainMethod.Call.SetTeaHorseCaravanState(3);
					}
					else
					{
						bool flag6 = btnName == "SearchBtn";
						if (flag6)
						{
							BuildingDomainMethod.Call.StartSearchReplenishment();
						}
						else
						{
							bool flag7 = btnName == "ExchangeBtn";
							if (flag7)
							{
								this._exchangingSupplies = true;
								this.exchangeItemPanel.SetActive(true);
								this.UpdateExchangeItem();
								this.UpdateSelfItem(true);
								this.UpdateItemAction();
							}
							else
							{
								bool flag8 = btnName == "ExchangeConfirmBtn";
								if (flag8)
								{
									bool flag9 = this._carryItemToExchangeList.Count == 0 && this._gainItemToExchangeList.Count == 0;
									if (flag9)
									{
										return;
									}
									this._exchangingSupplies = false;
									BuildingDomainMethod.Call.ExchangeItemToReplenishment((from e in this._carryItemToExchangeList
									select e.Item1).ToList<ItemKey>(), this._gainItemToExchangeList);
									this._carryItemToExchangeList.Clear();
									this._gainItemToExchangeList.Clear();
									this.exchangeItemPanel.SetActive(false);
								}
								else
								{
									bool flag10 = btnName == "ExchangeClearBtn" || btnName == "ExchangeCloseBtn";
									if (flag10)
									{
										this.ClearExchangePanel(btnName == "ExchangeClearBtn");
									}
									else
									{
										bool flag11 = btnName == "DiaryBtn";
										if (flag11)
										{
											this.OpenDiaryInfoPanel();
										}
										else
										{
											bool flag12 = btnName == "DiaryInfoCloseBtn";
											if (flag12)
											{
												this.CloseDiaryInfoPanel();
											}
											else
											{
												bool flag13 = btnName == "BtnCollectAllItem" || btnName == "ResetData";
												if (flag13)
												{
													UIElement.GetItem.SetOnInitArgs(new ArgumentBox().Set("Title", LanguageKey.LK_Get_Item_Get.Tr()).Set("InWareHouse", true).SetObject("ItemList", (from key in this._teaHorseCaravanData.ExchangeGoodsList
													select new ItemDisplayData(key.ItemType, key.TemplateId)).ToList<ItemDisplayData>()));
													UIManager.Instance.MaskUI(UIElement.GetItem);
													BuildingDomainMethod.Call.QuickGetExchangeItem(this.Source);
												}
												else
												{
													bool flag14 = btnName == "BtnDiscardAllItem";
													if (flag14)
													{
														UIElement dialog = UIElement.Dialog;
														ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
														string key2 = "Cmd";
														DialogCmd dialogCmd = new DialogCmd();
														dialogCmd.Type = 1;
														dialogCmd.Title = LanguageKey.LK_TeaHorse_GoodsAbandonAllItem.Tr();
														dialogCmd.Content = LanguageKey.LK_TeaHorse_GoodsAbandonAllItem_Confirm.Tr();
														dialogCmd.Yes = new Action(BuildingDomainMethod.Call.QuickDiscardExchangeItem);
														dialogCmd.No = delegate()
														{
														};
														dialog.SetOnInitArgs(argumentBox.SetObject(key2, dialogCmd));
														UIManager.Instance.MaskUI(UIElement.Dialog);
													}
													else
													{
														bool flag15 = btnName == "QuickCollectCarryingItem";
														if (flag15)
														{
															int i = this._teaHorseCaravanData.CarryGoodsList.Count;
															while (i-- > 0)
															{
																BuildingDomainMethod.Call.GetBackTeaHorseCarryItem(this._teaHorseCaravanData.CarryGoodsList[i].Item1, this._teaHorseCaravanData.CarryGoodsList[i].Item2);
															}
															this._teaHorseCaravanData.CarryGoodsList.Clear();
															this.UpdateSelfItem(true);
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			this.RefreshTaiwuItems();
		}

		// Token: 0x060097DB RID: 38875 RVA: 0x0046C6AC File Offset: 0x0046A8AC
		private void OnRenderReceivedItem(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			ItemDisplayData displayData = itemData as ItemDisplayData;
			bool flag = displayData == null;
			if (!flag)
			{
				RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
				rowItemMain.SetData(itemData);
				rowItemLine.Set(rowItemMain, true);
				bool exchangingSupplies = this._exchangingSupplies;
				if (exchangingSupplies)
				{
					this.OnRenderGetItemWhileExchanging(displayData, rowItemLine);
				}
				else
				{
					rowItemLine.SetInteractable(false, true);
				}
			}
		}

		// Token: 0x060097DC RID: 38876 RVA: 0x0046C704 File Offset: 0x0046A904
		private void OnRenderSelfItem(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			ItemDisplayData displayData = itemData as ItemDisplayData;
			bool flag = displayData == null;
			if (!flag)
			{
				RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
				rowItemMain.SetData(itemData);
				rowItemLine.Set(rowItemMain, true);
				sbyte itemSourceType = displayData.ItemSourceType;
				if (itemSourceType <= 4)
				{
					if (itemSourceType < 2)
					{
						goto IL_6E;
					}
				}
				else if (itemSourceType != 5)
				{
					goto IL_6E;
				}
				bool flag2 = true;
				goto IL_71;
				IL_6E:
				flag2 = false;
				IL_71:
				bool flag3 = flag2;
				if (flag3)
				{
					this.OnRenderWarehouseItem(displayData, rowItemLine);
				}
				else
				{
					this.OnRenderInventoryItem(displayData, rowItemLine);
				}
				rowItemLine.OnPointerEnterEvent = (rowItemLine.Interactable ? delegate()
				{
					this.OnHoverSelfItem(rowItemLine);
				} : null);
				rowItemLine.OnPointerExitEvent = new Action(this.OnHoverItemEnd);
			}
		}

		// Token: 0x060097DD RID: 38877 RVA: 0x0046C7E8 File Offset: 0x0046A9E8
		private void OnRenderWarehouseItem(ItemDisplayData itemData, RowItemLine rowItemLine)
		{
			bool lockStatus = !ItemTemplateHelper.IsTransferable(itemData.Key.ItemType, itemData.Key.TemplateId) || this._teaHorseCaravanData.CaravanState >= 1 || this._teaHorseCaravanData.CarryGoodsList.Count >= this._carryItemCount;
			rowItemLine.SetDisabled(lockStatus);
			rowItemLine.SetInteractable(!lockStatus && !itemData.IsLocked, true);
			rowItemLine.SetClickEvent(delegate
			{
				bool blockSelf = this._blockSelf;
				if (!blockSelf)
				{
					this._blockSelf = true;
					this.WarehouseItemToCarry(rowItemLine.Data);
				}
			});
		}

		// Token: 0x060097DE RID: 38878 RVA: 0x0046C89C File Offset: 0x0046AA9C
		private void OnRenderInventoryItem(ItemDisplayData itemData, RowItemLine rowItemLine)
		{
			bool lockStatus = !this._canTransfer || !ItemTemplateHelper.IsTransferable(itemData.Key.ItemType, itemData.Key.TemplateId) || this._teaHorseCaravanData.CaravanState >= 1 || this._teaHorseCaravanData.CarryGoodsList.Count >= this._carryItemCount;
			rowItemLine.SetDisabled(lockStatus);
			rowItemLine.SetInteractable(!lockStatus && !itemData.IsLocked, true);
			rowItemLine.SetClickEvent(delegate
			{
				bool blockSelf = this._blockSelf;
				if (!blockSelf)
				{
					this._blockSelf = true;
					this.InventoryItemToCarryWithTip(rowItemLine.Data);
				}
			});
		}

		// Token: 0x17001044 RID: 4164
		// (get) Token: 0x060097DF RID: 38879 RVA: 0x0046C95C File Offset: 0x0046AB5C
		public ItemSourceType Source
		{
			get
			{
				int activeIndex = this.selfTogGroup.GetActiveIndex();
				if (!true)
				{
				}
				ItemSourceType result;
				switch (activeIndex)
				{
				case 0:
					result = ItemSourceType.Inventory;
					break;
				case 1:
					result = ItemSourceType.Warehouse;
					break;
				case 2:
					result = ItemSourceType.Treasury;
					break;
				default:
					result = ItemSourceType.Stock;
					break;
				}
				if (!true)
				{
				}
				return result;
			}
		}

		// Token: 0x060097E0 RID: 38880 RVA: 0x0046C9A4 File Offset: 0x0046ABA4
		private void OnRenderGetItem(ItemDisplayData itemData, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			rowItemLine.SetDisabled(false);
			rowItemLine.SetInteractable(true, true);
			rowItemLine.SetClickEvent(delegate
			{
				bool blockSelf = this._blockSelf;
				if (!blockSelf)
				{
					this._blockSelf = true;
					int index = itemData.ItemShopLevel;
					bool flag = this._teaHorseCaravanData.ExchangeGoodsList.Count == 1;
					if (flag)
					{
						BuildingDomainMethod.Call.QuickGetExchangeItem(this.Source);
					}
					else
					{
						bool flag2 = this._teaHorseCaravanData.ExchangeGoodsList.CheckIndex(index) && this._teaHorseCaravanData.ExchangeGoodsList[index] == itemData.Key;
						if (flag2)
						{
							ItemKey itemKey = this._teaHorseCaravanData.ExchangeGoodsList[index];
							this._teaHorseCaravanData.ExchangeGoodsList.RemoveAt(index);
							BuildingDomainMethod.Call.QuickGetSpecificExchangeItem(itemKey, this.Source);
							bool flag3 = this._teaHorseCaravanData.ExchangeGoodsList.Count > 0;
							if (flag3)
							{
								List<ItemDisplayData> items = this._teaHorseCaravanData.ExchangeGoodsList.Select((ItemKey key, int newIndex) => new ItemDisplayData(key.ItemType, key.TemplateId)
								{
									ItemShopLevel = newIndex
								}).ToList<ItemDisplayData>();
								this.goodsItems.SetItemList(items);
							}
							else
							{
								BuildingDomainMethod.Call.QuickGetExchangeItem(this.Source);
							}
						}
						else
						{
							Debug.LogWarning(string.Format("itemData {0} has index {1} but CheckIndex(index) failed or key mismatch", itemData, index));
						}
					}
					this.RefreshTaiwuItems();
				}
			});
		}

		// Token: 0x060097E1 RID: 38881 RVA: 0x0046CA08 File Offset: 0x0046AC08
		private void OnRenderExchangingItem(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			ItemDisplayData displayData = itemData as ItemDisplayData;
			bool flag = displayData == null;
			if (!flag)
			{
				RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
				rowItemMain.SetData(itemData);
				rowItemLine.Set(rowItemMain, true);
				rowItemLine.OnPointerEnterEvent = (rowItemLine.Interactable ? delegate()
				{
					this.OnHoverItem(rowItemMain.ItemBack.transform as RectTransform, this.carryItemToExchangingItems.IsCardMode);
				} : null);
				rowItemLine.OnPointerExitEvent = new Action(this.OnHoverItemEnd);
				rowItemLine.SetClickEvent(delegate
				{
					bool blockExchange = this._blockExchange;
					if (!blockExchange)
					{
						this._blockExchange = true;
						bool flag2 = displayData.ItemShopLevel < 0;
						if (flag2)
						{
							int index = ~displayData.ItemShopLevel;
							bool flag3 = this._gainItemToExchangeList.CheckIndex(index) && this._gainItemToExchangeList[index] == itemData.Key;
							if (flag3)
							{
								this._teaHorseCaravanData.ExchangeGoodsList.Add(this._gainItemToExchangeList[index]);
								this._gainItemToExchangeList.RemoveAt(index);
							}
							else
							{
								Debug.LogWarning(string.Format("itemData {0} {1} has index {2} but CheckIndex(index) failed or key mismatch", itemData.Key, itemData, index));
							}
						}
						else
						{
							int index2 = displayData.ItemShopLevel;
							bool flag4 = this._carryItemToExchangeList.CheckIndex(index2) && this._carryItemToExchangeList[index2].Item1 == itemData.Key;
							if (flag4)
							{
								this._teaHorseCaravanData.CarryGoodsList.Add(this._carryItemToExchangeList[index2]);
								this._carryItemToExchangeList.RemoveAt(index2);
							}
							else
							{
								Debug.LogWarning(string.Format("itemData {0} {1} has index {2} but CheckIndex(index) failed or key mismatch", itemData.Key, itemData, index2));
							}
						}
						this.UpdateItemAction();
					}
				});
			}
		}

		// Token: 0x060097E2 RID: 38882 RVA: 0x0046CABC File Offset: 0x0046ACBC
		private void OnRenderGetItemWhileExchanging(ItemDisplayData itemData, RowItemLine rowItemLine)
		{
			bool flag = this.CanAddToExchange();
			if (flag)
			{
				rowItemLine.SetInteractable(true, true);
				rowItemLine.SetClickEvent(delegate
				{
					bool blockExchange = this._blockExchange;
					if (!blockExchange)
					{
						this._blockExchange = true;
						bool flag2 = this._teaHorseCaravanData.ExchangeGoodsList.CheckIndex(itemData.ItemShopLevel) && this._teaHorseCaravanData.ExchangeGoodsList[itemData.ItemShopLevel] == itemData.Key;
						if (flag2)
						{
							this._gainItemToExchangeList.Add(this._teaHorseCaravanData.ExchangeGoodsList[itemData.ItemShopLevel]);
							this._teaHorseCaravanData.ExchangeGoodsList.RemoveAt(itemData.ItemShopLevel);
						}
						else
						{
							Debug.LogWarning(string.Format("itemData {0} {1} has index {2} but CheckIndex(index) failed or key mismatch", itemData.Key, itemData, itemData.ItemShopLevel));
						}
						this.UpdateItemAction();
					}
				});
				rowItemLine.OnPointerEnterEvent = (rowItemLine.Interactable ? delegate()
				{
					this.OnHoverItem(rowItemLine.GetComponentInChildren<RowItemMain>().ItemBack.transform as RectTransform, this.exchangeGoodsItems.IsCardMode);
				} : null);
				rowItemLine.OnPointerExitEvent = new Action(this.OnHoverItemEnd);
			}
			else
			{
				rowItemLine.SetInteractable(false, true);
			}
		}

		// Token: 0x060097E3 RID: 38883 RVA: 0x0046CB68 File Offset: 0x0046AD68
		private void OnRenderGoodsItem(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			ItemDisplayData displayData = itemData as ItemDisplayData;
			bool flag = displayData == null;
			if (!flag)
			{
				int goodsTogState = this._goodsTogState;
				int num = goodsTogState;
				if (num != 0)
				{
					if (num == 1)
					{
						this.OnRenderCarryGoods(displayData, rowItemLine);
					}
				}
				else
				{
					this.OnRenderGetItem(displayData, rowItemLine);
				}
				rowItemLine.OnPointerEnterEvent = (rowItemLine.Interactable ? delegate()
				{
					this.OnHoverGoodsItem(rowItemLine);
				} : null);
				rowItemLine.OnPointerExitEvent = new Action(this.OnHoverItemEnd);
			}
		}

		// Token: 0x060097E4 RID: 38884 RVA: 0x0046CC14 File Offset: 0x0046AE14
		private void OnRenderCarryGoods(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			ViewTeaHorseCaravan.<>c__DisplayClass104_0 CS$<>8__locals1 = new ViewTeaHorseCaravan.<>c__DisplayClass104_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.rowItemLine = rowItemLine;
			CS$<>8__locals1.displayData = (itemData as ItemDisplayData);
			bool flag = CS$<>8__locals1.displayData == null;
			if (!flag)
			{
				RowItemMain rowItemMain = CS$<>8__locals1.rowItemLine.GetComponentInChildren<RowItemMain>();
				rowItemMain.SetData(itemData);
				CS$<>8__locals1.rowItemLine.Set(rowItemMain, true);
				CS$<>8__locals1.currSelfTogItemSource = this.selfTogGroup.GetActiveIndex() + 1;
				bool normalInteractable = itemData.Key.HasTemplate && (this._exchangingSupplies ? this.CanAddToExchange() : CS$<>8__locals1.<OnRenderCarryGoods>g__CanTransfer|3(CS$<>8__locals1.currSelfTogItemSource));
				normalInteractable = (normalInteractable && !itemData.IsLocked);
				CS$<>8__locals1.rowItemLine.SetInteractable(normalInteractable, true);
				RowItem rowItemLine2 = CS$<>8__locals1.rowItemLine;
				bool disabled;
				if (!normalInteractable)
				{
					if (itemData.Key.HasTemplate)
					{
						sbyte caravanState = this._teaHorseCaravanData.CaravanState;
						disabled = (caravanState != 2 && caravanState != 3);
					}
					else
					{
						disabled = true;
					}
				}
				else
				{
					disabled = false;
				}
				rowItemLine2.SetDisabled(disabled);
				CS$<>8__locals1.rowItemLine.OnPointerEnterEvent = (normalInteractable ? delegate()
				{
					CS$<>8__locals1.<>4__this.OnHoverCarryingItem(CS$<>8__locals1.rowItemLine);
				} : null);
				CS$<>8__locals1.rowItemLine.OnPointerExitEvent = new Action(this.OnHoverItemEnd);
				bool flag2 = normalInteractable;
				if (flag2)
				{
					CS$<>8__locals1.rowItemLine.SetClickEvent(this._exchangingSupplies ? delegate()
					{
						bool blockExchange = CS$<>8__locals1.<>4__this._blockExchange;
						if (!blockExchange)
						{
							CS$<>8__locals1.<>4__this._blockExchange = true;
							bool flag3 = CS$<>8__locals1.<>4__this._teaHorseCaravanData.CarryGoodsList.CheckIndex(CS$<>8__locals1.displayData.ItemShopLevel) && CS$<>8__locals1.<>4__this._teaHorseCaravanData.CarryGoodsList[CS$<>8__locals1.displayData.ItemShopLevel].Item1 == CS$<>8__locals1.displayData.Key;
							if (flag3)
							{
								CS$<>8__locals1.<>4__this._carryItemToExchangeList.Add(CS$<>8__locals1.<>4__this._teaHorseCaravanData.CarryGoodsList[CS$<>8__locals1.displayData.ItemShopLevel]);
								CS$<>8__locals1.<>4__this._teaHorseCaravanData.CarryGoodsList.RemoveAt(CS$<>8__locals1.displayData.ItemShopLevel);
							}
							else
							{
								Debug.LogWarning(string.Format("itemData {0} {1} CheckIndex(index) failed or key mismatch", CS$<>8__locals1.displayData.Key, CS$<>8__locals1.displayData));
							}
							CS$<>8__locals1.<>4__this.UpdateItemAction();
						}
					} : delegate()
					{
						bool blockExchange = CS$<>8__locals1.<>4__this._blockExchange;
						if (!blockExchange)
						{
							CS$<>8__locals1.<>4__this._blockExchange = true;
							switch (CS$<>8__locals1.currSelfTogItemSource)
							{
							case 1:
								if (CS$<>8__locals1.<>4__this._canTransfer)
								{
									CS$<>8__locals1.<>4__this.CarryItemToInventory(CS$<>8__locals1.displayData);
								}
								break;
							case 2:
								CS$<>8__locals1.<>4__this.CarryItemToWarehouse(CS$<>8__locals1.displayData);
								break;
							case 3:
								CS$<>8__locals1.<>4__this.CarryItemToWarehouse(CS$<>8__locals1.displayData);
								break;
							case 4:
								CS$<>8__locals1.<>4__this.CarryItemToWarehouse(CS$<>8__locals1.displayData);
								break;
							}
						}
					});
				}
			}
		}

		// Token: 0x060097E5 RID: 38885 RVA: 0x0046CD8C File Offset: 0x0046AF8C
		private void UpdateSelfItem(bool refreshCarry = true)
		{
			List<ItemDisplayData> list2;
			if (!refreshCarry)
			{
				List<ItemDisplayData> list;
				list2 = (this.stockDic.TryGetValue(this.selfTogGroup.GetActiveIndex(), out list) ? list : new List<ItemDisplayData>());
			}
			else
			{
				list2 = this._teaHorseCaravanData.ExchangeGoodsList.Select((ItemKey itemKey, int index) => new ItemDisplayData(itemKey.ItemType, itemKey.TemplateId)
				{
					ItemShopLevel = index
				}).ToList<ItemDisplayData>();
			}
			List<ItemDisplayData> items = list2;
			if (refreshCarry)
			{
				this.exchangeGoodsItems.SetItemList(items);
			}
			else
			{
				this.selfItems.SetItemList(items);
			}
		}

		// Token: 0x060097E6 RID: 38886 RVA: 0x0046CE18 File Offset: 0x0046B018
		private void UpdateExchangeItem()
		{
			sbyte caravanState = this._teaHorseCaravanData.CaravanState;
			bool canChooseItem = caravanState == 0 || caravanState == 4;
			this.chooseItem.SetActive(canChooseItem);
			this.exchangeGoodsItem.SetActive(!canChooseItem);
			this.UpdateSelfItem(!canChooseItem);
			bool isReadyGetItem = this._teaHorseCaravanData.CaravanState == 4;
			this.carryingItem.SetActive(!isReadyGetItem);
			this.goodsItem.SetActive(isReadyGetItem);
			Transform transform = this.arrow.transform;
			caravanState = this._teaHorseCaravanData.CaravanState;
			transform.localScale = new Vector3((float)((caravanState == 1 || caravanState == 4 || caravanState == 2 || caravanState == 3) ? -1 : 1), 1f, 1f);
			bool flag = this._currCaravanState != this._teaHorseCaravanData.CaravanState;
			if (flag)
			{
				bool flag2 = this._teaHorseCaravanData.CaravanState == 4;
				if (flag2)
				{
					this.goodsTogGroup.Set(0, true);
					bool flag3 = !this._canTransfer;
					if (flag3)
					{
						this.selfTogGroup.Set(1, true);
					}
				}
			}
			this._currCaravanState = this._teaHorseCaravanData.CaravanState;
			ItemDomainMethod.AsyncCall.GetItemDisplayDataListOptional(this, (from x in this._teaHorseCaravanData.CarryGoodsList
			select x.Item1).ToList<ItemKey>(), -1, -1, false, delegate(int offset, RawDataPool pool)
			{
				List<ItemDisplayData> dataList = null;
				Serializer.Deserialize(pool, offset, ref dataList);
				if (dataList == null)
				{
					dataList = new List<ItemDisplayData>();
				}
				int idx = Math.Max(dataList.Count, this._teaHorseCaravanData.CarryGoodsList.Count);
				while (idx-- > 0)
				{
					bool flag4 = dataList[idx].Key == this._teaHorseCaravanData.CarryGoodsList[idx].Item1;
					if (flag4)
					{
						dataList[idx].ItemSourceType = this._teaHorseCaravanData.CarryGoodsList[idx].Item2;
						dataList[idx].ItemShopLevel = idx;
					}
					else
					{
						Debug.LogWarning(string.Format("DataList item {0} and CarryGoodsList item {1} have different key, skip calculating.", dataList[idx].Key, this._teaHorseCaravanData.CarryGoodsList[idx].Item1));
					}
				}
				this._carryingItems = dataList.ToList<ItemDisplayData>();
				dataList.AddRange(from _ in Enumerable.Range(dataList.Count, this._carryItemCount - dataList.Count)
				select new ItemDisplayData());
				bool isReadyGetItem = isReadyGetItem;
				if (isReadyGetItem)
				{
					this.RefreshGoodsItem();
				}
				else
				{
					this.carryingItems.SetItemList(dataList);
				}
				this._blockExchange = false;
			});
			this.RefreshBg();
			this.RefreshText();
			this.RefreshAnim();
			this.RefreshBtns();
			this.RefreshDiary();
		}

		// Token: 0x060097E7 RID: 38887 RVA: 0x0046CFD0 File Offset: 0x0046B1D0
		private void RefreshBg()
		{
			int idx = this.bgChildScrolls.Count;
			while (idx-- > 0)
			{
				this.bgChildScrolls[idx].gameObject.SetActive(idx == (int)this._teaHorseCaravanData.Terrain);
			}
			UIChildScroll uichildScroll = this.bgChildScrolls[(int)this._teaHorseCaravanData.Terrain];
			sbyte caravanState = this._teaHorseCaravanData.CaravanState;
			if (!true)
			{
			}
			int num;
			if (!this._teaHorseCaravanData.IsStartSearch)
			{
				if (caravanState != 2)
				{
					if (caravanState != 3)
					{
						num = 0;
					}
					else
					{
						num = -40;
					}
				}
				else
				{
					num = 40;
				}
			}
			else
			{
				num = 0;
			}
			if (!true)
			{
			}
			uichildScroll.ScrollXSpeed = (float)num;
		}

		// Token: 0x060097E8 RID: 38888 RVA: 0x0046D084 File Offset: 0x0046B284
		private void RefreshText()
		{
			this.carryingText.SetText(string.Format("({0}/{1})", this._teaHorseCaravanData.CarryGoodsList.Count, this._carryItemCount), true);
			TMP_Text tmp_Text = this.distanceText;
			sbyte caravanState = this._teaHorseCaravanData.CaravanState;
			if (!true)
			{
			}
			string text;
			if (!this._teaHorseCaravanData.IsStartSearch)
			{
				switch (caravanState)
				{
				case 0:
					text = LocalStringManager.Get(LanguageKey.LK_TeaHorse_Idle);
					goto IL_DF;
				case 1:
					text = LocalStringManager.Get(LanguageKey.LK_TeaHorse_ReadyGo);
					goto IL_DF;
				case 4:
					text = LocalStringManager.Get(LanguageKey.LK_TeaHorse_WaitGetItem);
					goto IL_DF;
				}
				text = LocalStringManager.GetFormat(LanguageKey.LK_TeaHorse_Distance, this._teaHorseCaravanData.StartMonth, this._teaHorseCaravanData.DistanceToTaiwuVillage);
			}
			else
			{
				text = LanguageKey.LK_TeaHorse_GetReplenishment.Tr();
			}
			IL_DF:
			if (!true)
			{
			}
			tmp_Text.text = text;
			this.distanceText.GetComponent<TMPTextSpriteHelper>().Parse();
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.distanceText.transform.parent.GetComponent<RectTransform>());
			sbyte caravanState2 = this._teaHorseCaravanData.CaravanState;
			bool flag = caravanState2 == 1 || caravanState2 == 2 || caravanState2 == 3;
			if (flag)
			{
				this.weatherBg.gameObject.SetActive(true);
				this.direction.gameObject.SetActive(true);
				this.weatherIcon.SetSprite(TeaHorseCaravanWeather.Instance[this._teaHorseCaravanData.Weather].Icon, true, null);
				this.awarenessText.text = LanguageKey.LK_TeaHorse_Awareness.TrFormat(this._teaHorseCaravanData.CaravanAwareness);
				sbyte replenishmentChange = TeaHorseCaravanWeather.Instance[this._teaHorseCaravanData.Weather].ReplenishmentChange;
				this.weatherChangeReplenishmentText.text = ((replenishmentChange == 0) ? replenishmentChange.ToString() : ("-" + replenishmentChange.ToString()));
				this.replenishmentText.text = LanguageKey.LK_TeaHorse_Replenishment.TrFormat(this._teaHorseCaravanData.CaravanReplenishment, 100, 5);
				this.replenishmentText.GetComponent<TMPTextSpriteHelper>().Parse();
				int lostProb = (this._teaHorseCaravanData.LackReplenishmentTurn > 10) ? 100 : ((this._teaHorseCaravanData.LackReplenishmentTurn > 0) ? ((int)Math.Min(100f, MathF.Pow(2f, (float)(this._teaHorseCaravanData.LackReplenishmentTurn - 1)))) : 0);
				TMP_Text tmp_Text2 = this.forwardDescText;
				sbyte caravanState3 = this._teaHorseCaravanData.CaravanState;
				if (!true)
				{
				}
				switch (caravanState3)
				{
				case 1:
					text = LanguageKey.LK_TeaHorse_ReadyGo.Tr();
					goto IL_31A;
				case 2:
					text = LanguageKey.LK_TeaHorse_ForwardWest.Tr();
					goto IL_31A;
				case 4:
					text = LanguageKey.LK_TeaHorse_WaitGetItem.Tr();
					goto IL_31A;
				}
				text = LanguageKey.LK_TeaHorse_ForwardEast.Tr();
				IL_31A:
				if (!true)
				{
				}
				tmp_Text2.SetText(text, true);
				bool isStartSearch = this._teaHorseCaravanData.IsStartSearch;
				if (isStartSearch)
				{
					this.forwardDescText.SetText(LanguageKey.LK_TeaHorse_GetReplenishment2.Tr(), true);
				}
				GameObject gameObject = this.forwardDescText.gameObject;
				caravanState2 = this._teaHorseCaravanData.CaravanState;
				gameObject.SetActive(caravanState2 > 1 && caravanState2 <= 4);
				this.goodLostText.text = LanguageKey.LK_TeaHorse_LoseProbVal.TrFormat(lostProb);
				this.goodLostText.GetComponent<TMPTextSpriteHelper>().Parse();
			}
			else
			{
				this.weatherBg.gameObject.SetActive(false);
				this.direction.gameObject.SetActive(false);
			}
		}

		// Token: 0x060097E9 RID: 38889 RVA: 0x0046D43C File Offset: 0x0046B63C
		private void RefreshAnim()
		{
			BuildingBlockItem config = BuildingBlock.Instance[51];
			sbyte buildingLevel = this.BuildingModel.GetBuildingLevel(this._buildingBlocKey, this._buildingBlockData);
			bool flag = buildingLevel >= config.MaxLevel;
			if (flag)
			{
				buildingLevel = config.MaxLevel;
			}
			this.teaHorseAnimation.transform.localScale = new Vector3((this._teaHorseCaravanData.CaravanState == 3) ? -1f : 1f, 1f, 1f);
			Spine.AnimationState animationState = this.teaHorseAnimation.AnimationState;
			int trackIndex = 0;
			sbyte caravanState = this._teaHorseCaravanData.CaravanState;
			animationState.SetAnimation(trackIndex, ((caravanState == 2 || caravanState == 3) && !this._teaHorseCaravanData.IsStartSearch) ? string.Format("run_lv{0}", (int)(buildingLevel / 2)) : string.Format("idle_lv{0}", (int)(buildingLevel / 2)), true);
		}

		// Token: 0x060097EA RID: 38890 RVA: 0x0046D51C File Offset: 0x0046B71C
		private void RefreshBtns()
		{
			GameObject gameObject = this.startBtn.gameObject;
			sbyte caravanState = this._teaHorseCaravanData.CaravanState;
			gameObject.SetActive((caravanState == 3 || caravanState == 0 || caravanState == 1 || caravanState == 4) && (this._teaHorseCaravanData.CarryGoodsList.Count > 0 || this._teaHorseCaravanData.CaravanState == 0));
			this.startBtn.interactable = (this._teaHorseCaravanData.CaravanState == 3 || (this._teaHorseCaravanData.CaravanState == 0 && this._teaHorseCaravanData.CarryGoodsList.Count > 0));
			TooltipInvoker displayer = this.startBtn.GetComponent<TooltipInvoker>();
			bool flag = displayer.enabled = (this._teaHorseCaravanData.CaravanState == 4 || (this._teaHorseCaravanData.CaravanState == 0 && this._teaHorseCaravanData.CarryGoodsList.Count == 0));
			if (flag)
			{
				string[] presetParam = displayer.PresetParam;
				bool flag2 = presetParam == null || presetParam.Length <= 0;
				if (flag2)
				{
					displayer.PresetParam = new string[]
					{
						""
					};
				}
				displayer.PresetParam[0] = ((this._teaHorseCaravanData.CaravanState == 4) ? LanguageKey.LK_Building_TeaHorse_Start_Btn_Tips_Returned.Tr() : LanguageKey.LK_Building_TeaHorse_Start_Btn_Tips_None.Tr());
			}
			this.returnBtn.gameObject.SetActive(this._teaHorseCaravanData.CaravanState == 2);
			this.exchangeBtn.gameObject.SetActive(this._teaHorseCaravanData.IsShowExchangeReplenishment);
			this.exchangeBtn.interactable = ((this._teaHorseCaravanData.CarryGoodsList.Count > 0 || this._teaHorseCaravanData.ExchangeGoodsList.Count > 0) && this._teaHorseCaravanData.ExchangeReplenishmentRemainAmount > 0);
			this.exchangeText.text = string.Format("{0}<color=#lightgrey>/{1}</color>", this._teaHorseCaravanData.ExchangeReplenishmentRemainAmount, this._teaHorseCaravanData.ExchangeReplenishmentAmountMax).ColorReplace();
			this.searchBtn.gameObject.SetActive(this._teaHorseCaravanData.IsShowSeachReplenishment);
			this.searchBtn.interactable = !this._teaHorseCaravanData.IsStartSearch;
			this.searchText.text = string.Format("{0}<color=#lightgrey>/{1}</color>", this._teaHorseCaravanData.SearchReplenishmentAmount, this._teaHorseCaravanData.SearchReplenishmentMax).ColorReplace();
		}

		// Token: 0x060097EB RID: 38891 RVA: 0x0046D797 File Offset: 0x0046B997
		private void RefreshDiary()
		{
			this.diaryInfoScroll.DataRender = new Func<TransferableRecord, TransferableRecordDataBase, ValueTuple<string, string>>(ViewTeaHorseCaravan.DataRender);
			BuildingDomainMethod.AsyncCall.GetTeaHorseCaravanEvent(this, delegate(int x, RawDataPool y)
			{
				this.diaryInfoScroll.Read(x, y);
				TMP_Text tmp_Text = this.diaryInfoText;
				List<TransferableRecord> record = this.diaryInfoScroll.Data.Record;
				tmp_Text.SetText((record != null && record.Count > 0) ? LanguageKey.LK_TeaHorse_Diary.Tr() : LanguageKey.LK_TeaHorse_DiaryLack.Tr(), true);
				Selectable selectable = this.diaryBtn;
				List<TransferableRecord> record2 = this.diaryInfoScroll.Data.Record;
				selectable.interactable = (record2 != null && record2.Count > 0);
			});
		}

		// Token: 0x060097EC RID: 38892 RVA: 0x0046D7C4 File Offset: 0x0046B9C4
		[return: TupleElementNames(new string[]
		{
			"main",
			"sub"
		})]
		private static ValueTuple<string, string> DataRender(TransferableRecord record, TransferableRecordDataBase data)
		{
			TeaHorseCaravanEventItem config = TeaHorseCaravanEvent.Instance[record.RecordType];
			bool flag = config != null;
			ValueTuple<string, string> result;
			if (flag)
			{
				result = new ValueTuple<string, string>(string.Format(config.Desc, (from x in record.Arguments
				select GameMessageUtils.ReadArguments(x.Item1, x.Item2, data)).ToArray<object>()).ColorReplace(), "");
			}
			else
			{
				Debug.LogWarning(string.Format("Invalid record type: {0}", record.RecordType));
				result = new ValueTuple<string, string>("", "");
			}
			return result;
		}

		// Token: 0x060097ED RID: 38893 RVA: 0x0046D860 File Offset: 0x0046BA60
		private void SetWeatherVFX()
		{
			List<GameObject> weathers = this.weatherVfx.weatherList;
			for (int i = 0; i < weathers.Count; i++)
			{
				weathers[i].SetActive(false);
			}
			bool flag = this._teaHorseCaravanData.Weather == 0 || this._teaHorseCaravanData.Weather == 3;
			if (!flag)
			{
				GameObject go = this.weatherVfx.weatherList[(int)(this._teaHorseCaravanData.Weather - 1)];
				bool flag2 = go != null;
				if (flag2)
				{
					go.SetActive(true);
					ParticleSystem particle = go.GetComponent<ParticleSystem>();
					bool flag3 = particle != null;
					if (flag3)
					{
						particle.Play();
					}
				}
			}
		}

		// Token: 0x060097EE RID: 38894 RVA: 0x0046D918 File Offset: 0x0046BB18
		private void SetAudio()
		{
			bool flag = this._teaHorseCaravanData.DistanceToTaiwuVillage == 0;
			if (flag)
			{
				AudioManager.Instance.PlaySoundNoRepeat("caravan_map_taiwu", 100, true, false);
				bool flag2 = this._teaHorseCaravanData.CaravanState == 0;
				if (flag2)
				{
					AudioManager.Instance.StopAllSound("caravan_carriage_idle");
					AudioManager.Instance.PlaySoundNoRepeat("caravan_carriage_rest", 100, true, false);
				}
				else
				{
					bool flag3 = this._teaHorseCaravanData.CaravanState == 1;
					if (flag3)
					{
						AudioManager.Instance.StopAllSound("caravan_carriage_rest");
						AudioManager.Instance.PlaySoundNoRepeat("caravan_carriage_idle", 100, true, false);
					}
				}
			}
			else
			{
				AudioManager.Instance.PlaySoundNoRepeat("caravan_carriage_run", 100, true, false);
			}
			switch (this._teaHorseCaravanData.Weather)
			{
			case 1:
				AudioManager.Instance.PlaySoundNoRepeat("weather_hailstone", 100, true, false);
				break;
			case 2:
				AudioManager.Instance.PlaySoundNoRepeat("weather_sandstorm", 100, true, false);
				break;
			case 4:
			{
				AudioManager.Instance.PlaySoundNoRepeat("weather_heavyrain", 100, true, false);
				string thunderName = this._weatherThunderAudioList.GetRandom<string>();
				int randomVolume = GameApp.RandomRange(20, 80);
				AudioManager.Instance.PlaySoundNoRepeat(thunderName, randomVolume, true, false);
				break;
			}
			case 5:
				AudioManager.Instance.PlaySoundNoRepeat("weather_lightrain", 100, true, false);
				break;
			case 6:
				AudioManager.Instance.PlaySoundNoRepeat("weather_snow", 100, true, false);
				break;
			case 7:
				AudioManager.Instance.PlaySoundNoRepeat("weather_snowstorm", 100, true, false);
				break;
			case 8:
				AudioManager.Instance.PlaySoundNoRepeat("weather_heavywind", 100, true, false);
				break;
			}
		}

		// Token: 0x060097EF RID: 38895 RVA: 0x0046DAF8 File Offset: 0x0046BCF8
		private void UpdateItemAction()
		{
			this.exchangeBtn.gameObject.SetActive(this._carryItemToExchangeList.Count + this._gainItemToExchangeList.Count == 0);
			ItemDomainMethod.AsyncCall.GetItemDisplayDataListOptional(this, (from x in this._carryItemToExchangeList
			select x.Item1).ToList<ItemKey>(), -1, -1, false, delegate(int offset, RawDataPool pool)
			{
				List<ItemDisplayData> dataList = null;
				Serializer.Deserialize(pool, offset, ref dataList);
				if (dataList == null)
				{
					dataList = new List<ItemDisplayData>();
				}
				int idx2 = Math.Max(dataList.Count, this._carryItemToExchangeList.Count);
				while (idx2-- > 0)
				{
					bool flag = dataList[idx2].Key == this._carryItemToExchangeList[idx2].Item1;
					if (flag)
					{
						dataList[idx2].ItemShopLevel = idx2;
					}
					else
					{
						Debug.LogWarning(string.Format("DataList item {0} and CarryGoodsList item {1} have different key, skip calculating.", dataList[idx2].Key, this._teaHorseCaravanData.CarryGoodsList[idx2].Item1));
					}
				}
				dataList.AddRange(this._gainItemToExchangeList.Select((ItemKey key, int idx) => new ItemDisplayData(key.ItemType, key.TemplateId)
				{
					ItemShopLevel = ~idx
				}));
				this.carryItemToExchangingItems.SetItemList(dataList);
				this.exchangeConfirmBtn.interactable = (dataList.Count > 0);
				this.exchangeDetailText.text = LanguageKey.LK_TeaHorse_ExchangeReplenishment_Detail.TrFormat(this.CalcExchanageReplenishmentNumReal(), this.CaravanReplenishmentMaxValue);
				this._blockExchange = false;
			});
			List<ItemDisplayData> items = this._teaHorseCaravanData.ExchangeGoodsList.Select((ItemKey itemKey, int index) => new ItemDisplayData(itemKey.ItemType, itemKey.TemplateId)
			{
				ItemShopLevel = index
			}).ToList<ItemDisplayData>();
			this.exchangeGoodsItems.SetItemList(items);
			ItemDomainMethod.AsyncCall.GetItemDisplayDataListOptional(this, (from x in this._teaHorseCaravanData.CarryGoodsList
			select x.Item1).ToList<ItemKey>(), -1, -1, false, delegate(int offset, RawDataPool pool)
			{
				List<ItemDisplayData> dataList = null;
				Serializer.Deserialize(pool, offset, ref dataList);
				if (dataList == null)
				{
					dataList = new List<ItemDisplayData>();
				}
				int idx = Math.Max(dataList.Count, this._teaHorseCaravanData.CarryGoodsList.Count);
				while (idx-- > 0)
				{
					bool flag = dataList[idx].Key == this._teaHorseCaravanData.CarryGoodsList[idx].Item1;
					if (flag)
					{
						dataList[idx].ItemSourceType = this._teaHorseCaravanData.CarryGoodsList[idx].Item2;
						dataList[idx].ItemShopLevel = idx;
					}
					else
					{
						Debug.LogWarning(string.Format("DataList item {0} and CarryGoodsList item {1} have different key, skip calculating.", dataList[idx].Key, this._teaHorseCaravanData.CarryGoodsList[idx].Item1));
					}
				}
				dataList.AddRange(from _ in Enumerable.Range(dataList.Count, this._carryItemCount - dataList.Count)
				select new ItemDisplayData());
				this.carryingItems.SetItemList(dataList);
			});
		}

		// Token: 0x060097F0 RID: 38896 RVA: 0x0046DC04 File Offset: 0x0046BE04
		private bool CanAddToExchange()
		{
			return this._teaHorseCaravanData.ExchangeReplenishmentRemainAmount > this.CalcExchanageReplenishmentNum() && (int)this.CalcExchanageReplenishmentNum() < this.CaravanReplenishmentMaxValue;
		}

		// Token: 0x17001045 RID: 4165
		// (get) Token: 0x060097F1 RID: 38897 RVA: 0x0046DC3A File Offset: 0x0046BE3A
		public int CaravanReplenishmentMaxValue
		{
			get
			{
				return Math.Min((int)this._teaHorseCaravanData.ExchangeReplenishmentRemainAmount, (int)(100 - this._teaHorseCaravanData.CaravanReplenishment));
			}
		}

		// Token: 0x060097F2 RID: 38898 RVA: 0x0046DC5C File Offset: 0x0046BE5C
		private short CalcExchanageReplenishmentNum()
		{
			short getReplenishmentNum = 0;
			foreach (ValueTuple<ItemKey, sbyte> item in this._carryItemToExchangeList)
			{
				getReplenishmentNum += this.GradeToReplenishment(ItemTemplateHelper.GetGrade(item.Item1.ItemType, item.Item1.TemplateId));
			}
			foreach (ItemKey item2 in this._gainItemToExchangeList)
			{
				getReplenishmentNum += this.GradeToReplenishment(ItemTemplateHelper.GetGrade(item2.ItemType, item2.TemplateId));
			}
			return getReplenishmentNum;
		}

		// Token: 0x060097F3 RID: 38899 RVA: 0x0046DD38 File Offset: 0x0046BF38
		private short CalcExchanageReplenishmentNumReal()
		{
			return (short)Math.Min((int)this.CalcExchanageReplenishmentNum(), this.CaravanReplenishmentMaxValue);
		}

		// Token: 0x060097F4 RID: 38900 RVA: 0x0046DD4C File Offset: 0x0046BF4C
		private short GradeToReplenishment(sbyte grade)
		{
			return (short)((grade + 1) * 5 + 5);
		}

		// Token: 0x060097F5 RID: 38901 RVA: 0x0046DD68 File Offset: 0x0046BF68
		private void OnToggleChange(int newTog, int oldTog)
		{
			List<ItemDisplayData> displayDataList;
			this.stockDic.TryGetValue(newTog, out displayDataList);
			if (displayDataList == null)
			{
				displayDataList = new List<ItemDisplayData>();
			}
			this.selfItems.SetItemList(displayDataList);
			this.carryingItems.ReRender();
			this.goodsItems.ReRender();
		}

		// Token: 0x060097F6 RID: 38902 RVA: 0x0046DDB3 File Offset: 0x0046BFB3
		private void OnGoodsToggleChange(int newTog, int oldTog)
		{
			this._goodsTogState = newTog;
			this.RefreshGoodsItem();
		}

		// Token: 0x060097F7 RID: 38903 RVA: 0x0046DDC4 File Offset: 0x0046BFC4
		private void RefreshGoodsItem()
		{
			bool flag = this._teaHorseCaravanData.CaravanState != 4;
			if (!flag)
			{
				this.goodsBottom.SetActive(this._goodsTogState == 0);
				RectTransform rect = this.goodsItems.GetComponent<RectTransform>();
				rect.offsetMin = new Vector2(rect.offsetMin.x, (this._goodsTogState == 0) ? this._goodsItemsInReceivedBottom : this._goodsItemsInCarryBottom);
				List<ItemDisplayData> list;
				if (this._goodsTogState != 0)
				{
					list = this._carryingItems;
				}
				else
				{
					list = this._teaHorseCaravanData.ExchangeGoodsList.Select((ItemKey itemKey, int index) => new ItemDisplayData(itemKey.ItemType, itemKey.TemplateId)
					{
						ItemShopLevel = index
					}).ToList<ItemDisplayData>();
				}
				List<ItemDisplayData> items = list;
				this.goodsItems.SetItemList(items);
			}
		}

		// Token: 0x060097F8 RID: 38904 RVA: 0x0046DE8C File Offset: 0x0046C08C
		private void CarryItemToWarehouse(ItemDisplayData item)
		{
			int activeTog = this.selfTogGroup.GetActiveIndex();
			Inventory inventory = item.GetOperationInventoryFromPool(1, false);
			foreach (KeyValuePair<ItemKey, int> keyValuePair in inventory.Items)
			{
				ItemKey itemKey2;
				int num;
				keyValuePair.Deconstruct(out itemKey2, out num);
				ItemKey itemKey = itemKey2;
				BuildingDomainMethod.Call.GetBackTeaHorseCarryItem(itemKey, (sbyte)(activeTog + 1));
			}
			ItemDisplayData.ReturnInventoryToPool(inventory);
			this.RefreshTaiwuItems();
		}

		// Token: 0x060097F9 RID: 38905 RVA: 0x0046DF1C File Offset: 0x0046C11C
		private void CarryItemToInventory(ItemDisplayData item)
		{
			Inventory inventory = item.GetOperationInventoryFromPool(1, false);
			foreach (KeyValuePair<ItemKey, int> keyValuePair in inventory.Items)
			{
				ItemKey itemKey2;
				int num;
				keyValuePair.Deconstruct(out itemKey2, out num);
				ItemKey itemKey = itemKey2;
				BuildingDomainMethod.Call.GetBackTeaHorseCarryItem(itemKey, 1);
			}
			ItemDisplayData.ReturnInventoryToPool(inventory);
			this.RefreshTaiwuItems();
		}

		// Token: 0x060097FA RID: 38906 RVA: 0x0046DF9C File Offset: 0x0046C19C
		private void InventoryItemToCarryWithTip(ITradeableContent itemData)
		{
			ItemDisplayData displayData = itemData as ItemDisplayData;
			bool flag = displayData == null;
			if (flag)
			{
				this._blockSelf = false;
			}
			else
			{
				bool flag2 = itemData.UsingType != ItemDisplayData.ItemUsingType.Invalid;
				if (flag2)
				{
					DialogCmd cmd = new DialogCmd
					{
						Title = LocalStringManager.Get(LanguageKey.LK_Common_Attention),
						Content = displayData.GetUsingOperationConfirmTip(ItemDisplayData.ItemUsingOperationType.Default),
						Type = 1,
						Yes = delegate()
						{
							this.InventoryItemToCarry(displayData);
						},
						No = delegate()
						{
							this._blockSelf = false;
						}
					};
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
					UIManager.Instance.MaskUI(UIElement.Dialog);
				}
				else
				{
					this.InventoryItemToCarry(displayData);
				}
			}
		}

		// Token: 0x060097FB RID: 38907 RVA: 0x0046E084 File Offset: 0x0046C284
		private void InventoryItemToCarry(ItemDisplayData item)
		{
			bool flag = this._teaHorseCaravanData.CarryGoodsList.Count >= this._carryItemCount;
			if (!flag)
			{
				ItemDisplayData.ClearItemUsingState(item, this._inventoryItems);
				Inventory inventory = item.GetOperationInventoryFromPool(1, false);
				item.ChangeAmount(inventory, false);
				bool flag2 = item.Amount == 0;
				if (flag2)
				{
					this._inventoryItems.Remove(item);
				}
				int index = 0;
				foreach (ItemDisplayData data in this._inventoryItems)
				{
					data.ItemShopLevel = index++;
				}
				this.selfItems.SetItemList(this._inventoryItems);
				foreach (KeyValuePair<ItemKey, int> keyValuePair in inventory.Items)
				{
					ItemKey itemKey2;
					int num;
					keyValuePair.Deconstruct(out itemKey2, out num);
					ItemKey itemKey = itemKey2;
					BuildingDomainMethod.Call.AddItemToTeaHorseCarryItem(itemKey, 1);
				}
				ItemDisplayData.ReturnInventoryToPool(inventory);
				this.RefreshTaiwuItems();
			}
		}

		// Token: 0x060097FC RID: 38908 RVA: 0x0046E1BC File Offset: 0x0046C3BC
		private void WarehouseItemToCarry(ITradeableContent itemData)
		{
			ItemDisplayData displayData = itemData as ItemDisplayData;
			bool flag = displayData == null;
			if (!flag)
			{
				bool flag2 = this._teaHorseCaravanData.CarryGoodsList.Count >= this._carryItemCount;
				if (!flag2)
				{
					int activeTog = this.selfTogGroup.GetActiveIndex();
					List<ItemDisplayData> dataList;
					this.stockDic.TryGetValue(activeTog, out dataList);
					Inventory inventory = itemData.GetOperationInventoryFromPool(1, false);
					itemData.ChangeAmount(inventory, false);
					bool flag3 = itemData.Amount == 0;
					if (flag3)
					{
						dataList.Remove(displayData);
					}
					int index = 0;
					foreach (ItemDisplayData data in dataList)
					{
						data.ItemShopLevel = index++;
					}
					this.selfItems.SetItemList(dataList);
					foreach (KeyValuePair<ItemKey, int> keyValuePair in inventory.Items)
					{
						ItemKey itemKey2;
						int num;
						keyValuePair.Deconstruct(out itemKey2, out num);
						ItemKey itemKey = itemKey2;
						BuildingDomainMethod.Call.AddItemToTeaHorseCarryItem(itemKey, (sbyte)(activeTog + 1));
					}
					ItemDisplayData.ReturnInventoryToPool(inventory);
					this.RefreshTaiwuItems();
				}
			}
		}

		// Token: 0x060097FD RID: 38909 RVA: 0x0046E314 File Offset: 0x0046C514
		private void ClearExchangePanel(bool keepActive = false)
		{
			GameObject gameObject = this.exchangeItemPanel;
			this._exchangingSupplies = keepActive;
			gameObject.SetActive(keepActive);
			BuildingDomainMethod.Call.SetTeaHorseCaravanState(this._teaHorseCaravanData.CaravanState);
			this._carryItemToExchangeList.Clear();
			this._gainItemToExchangeList.Clear();
			if (keepActive)
			{
				this.UpdateItemAction();
			}
		}

		// Token: 0x060097FE RID: 38910 RVA: 0x0046E370 File Offset: 0x0046C570
		public override void OnLanguageChange(LocalStringManager.LanguageType languageType)
		{
			base.OnLanguageChange(languageType);
			RectTransform rectTransform = this.statusTips;
			LocalStringManager.LanguageType curLanguageType = LocalStringManager.CurLanguageType;
			if (!true)
			{
			}
			Vector2 sizeDelta;
			if (curLanguageType != LocalStringManager.LanguageType.CN)
			{
				sizeDelta = this.statusTipsEnSize;
			}
			else
			{
				sizeDelta = this.statusTipsCnSize;
			}
			if (!true)
			{
			}
			rectTransform.sizeDelta = sizeDelta;
		}

		// Token: 0x060097FF RID: 38911 RVA: 0x0046E3BA File Offset: 0x0046C5BA
		private void OpenDiaryInfoPanel()
		{
			this.diaryInfoPanel.SetActive(true);
			this.diaryInfoScroll.RefreshScrollToEnd();
		}

		// Token: 0x06009800 RID: 38912 RVA: 0x0046E3D6 File Offset: 0x0046C5D6
		private void CloseDiaryInfoPanel()
		{
			this.diaryInfoPanel.SetActive(false);
		}

		// Token: 0x06009801 RID: 38913 RVA: 0x0046E3E8 File Offset: 0x0046C5E8
		private void OnHoverCarryingItem(RowItemLine item)
		{
			bool flag = this._teaHorseCaravanData.CaravanState != 0 && !this._exchangingSupplies;
			if (!flag)
			{
				RowItemMain rowItemMain = item.GetComponentInChildren<RowItemMain>();
				RectTransform rect = rowItemMain.ItemBack.GetComponent<RectTransform>();
				this.OnHoverItem(rect, this.carryingItems.IsCardMode);
			}
		}

		// Token: 0x06009802 RID: 38914 RVA: 0x0046E43C File Offset: 0x0046C63C
		private void OnHoverGoodsItem(RowItemLine item)
		{
			bool flag = this._teaHorseCaravanData.CaravanState != 4 && !this._exchangingSupplies;
			if (!flag)
			{
				RowItemMain rowItemMain = item.GetComponentInChildren<RowItemMain>();
				RectTransform rect = rowItemMain.ItemBack.GetComponent<RectTransform>();
				this.OnHoverItem(rect, this.goodsItems.IsCardMode);
			}
		}

		// Token: 0x06009803 RID: 38915 RVA: 0x0046E490 File Offset: 0x0046C690
		private void OnHoverSelfItem(RowItemLine item)
		{
			bool flag = this._teaHorseCaravanData.CaravanState != 0 && !this._exchangingSupplies;
			if (!flag)
			{
				RowItemMain rowItemMain = item.GetComponentInChildren<RowItemMain>();
				RectTransform rect = rowItemMain.ItemBack.GetComponent<RectTransform>();
				this.OnHoverItem(rect, this.selfItems.IsCardMode);
			}
		}

		// Token: 0x06009804 RID: 38916 RVA: 0x0046E4E4 File Offset: 0x0046C6E4
		private void OnHoverItem(RectTransform trans, bool isCardMode)
		{
			this.hoverFollower.Target = trans;
			Vector2 offset = trans.sizeDelta;
			float x = Mathf.Lerp(offset.x, -offset.x, trans.pivot.x);
			this.hoverFollower.Offset = new Vector2(x, -trans.sizeDelta.y) * 0.5f;
			this.hoverImg.gameObject.SetActive(true);
			this.hoverImg.sprite = (isCardMode ? this.hoverLarge : this.hoverSmall);
			this.hoverImg.SetNativeSize();
		}

		// Token: 0x06009805 RID: 38917 RVA: 0x0046E58A File Offset: 0x0046C78A
		private void OnHoverItemEnd()
		{
			this.hoverImg.gameObject.SetActive(false);
		}

		// Token: 0x04007485 RID: 29829
		[SerializeField]
		private TextMeshProUGUI warehouseLoad;

		// Token: 0x04007486 RID: 29830
		[SerializeField]
		private GeneralRecord diaryInfoScroll;

		// Token: 0x04007487 RID: 29831
		[SerializeField]
		private CImage weatherIcon;

		// Token: 0x04007488 RID: 29832
		[SerializeField]
		private GameObject arrow;

		// Token: 0x04007489 RID: 29833
		[SerializeField]
		private GameObject weatherBg;

		// Token: 0x0400748A RID: 29834
		[SerializeField]
		private GameObject distanceBg;

		// Token: 0x0400748B RID: 29835
		[SerializeField]
		private GameObject direction;

		// Token: 0x0400748C RID: 29836
		[SerializeField]
		private TeaHorseCaravanWeatherVfx weatherVfx;

		// Token: 0x0400748D RID: 29837
		[SerializeField]
		private SkeletonGraphic teaHorseAnimation;

		// Token: 0x0400748E RID: 29838
		[SerializeField]
		private ItemListScroll carryItemToExchangingItems;

		// Token: 0x0400748F RID: 29839
		[SerializeField]
		private ItemListScroll carryingItems;

		// Token: 0x04007490 RID: 29840
		[SerializeField]
		private ItemListScroll selfItems;

		// Token: 0x04007491 RID: 29841
		[SerializeField]
		private ItemListScroll exchangeGoodsItems;

		// Token: 0x04007492 RID: 29842
		[SerializeField]
		private ItemListScroll goodsItems;

		// Token: 0x04007493 RID: 29843
		[SerializeField]
		private CToggleGroup selfTogGroup;

		// Token: 0x04007494 RID: 29844
		[SerializeField]
		private CToggleGroup goodsTogGroup;

		// Token: 0x04007495 RID: 29845
		[SerializeField]
		private RectTransform statusTips;

		// Token: 0x04007496 RID: 29846
		[SerializeField]
		private GameObject exchangeItemPanel;

		// Token: 0x04007497 RID: 29847
		[SerializeField]
		private GameObject diaryInfoPanel;

		// Token: 0x04007498 RID: 29848
		[SerializeField]
		private GameObject chooseItem;

		// Token: 0x04007499 RID: 29849
		[SerializeField]
		private GameObject exchangeGoodsItem;

		// Token: 0x0400749A RID: 29850
		[SerializeField]
		private GameObject carryingItem;

		// Token: 0x0400749B RID: 29851
		[SerializeField]
		private GameObject goodsItem;

		// Token: 0x0400749C RID: 29852
		[SerializeField]
		private GameObject goodsBottom;

		// Token: 0x0400749D RID: 29853
		[SerializeField]
		private CButton startBtn;

		// Token: 0x0400749E RID: 29854
		[SerializeField]
		private CButton returnBtn;

		// Token: 0x0400749F RID: 29855
		[SerializeField]
		private CButton exchangeBtn;

		// Token: 0x040074A0 RID: 29856
		[SerializeField]
		private CButton exchangeConfirmBtn;

		// Token: 0x040074A1 RID: 29857
		[SerializeField]
		private CButton searchBtn;

		// Token: 0x040074A2 RID: 29858
		[SerializeField]
		private CButton quickCollectCarryingItem;

		// Token: 0x040074A3 RID: 29859
		[SerializeField]
		private CButton diaryBtn;

		// Token: 0x040074A4 RID: 29860
		[SerializeField]
		private Vector2 statusTipsCnSize = new Vector2(656f, 48f);

		// Token: 0x040074A5 RID: 29861
		[SerializeField]
		private Vector2 statusTipsEnSize = new Vector2(670f, 88f);

		// Token: 0x040074A6 RID: 29862
		[SerializeField]
		private TMP_Text awarenessText;

		// Token: 0x040074A7 RID: 29863
		[SerializeField]
		private TMP_Text weatherChangeReplenishmentText;

		// Token: 0x040074A8 RID: 29864
		[SerializeField]
		private TMP_Text distanceText;

		// Token: 0x040074A9 RID: 29865
		[SerializeField]
		private TMP_Text replenishmentText;

		// Token: 0x040074AA RID: 29866
		[SerializeField]
		private TMP_Text goodLostText;

		// Token: 0x040074AB RID: 29867
		[SerializeField]
		private TMP_Text searchText;

		// Token: 0x040074AC RID: 29868
		[SerializeField]
		private TMP_Text exchangeText;

		// Token: 0x040074AD RID: 29869
		[SerializeField]
		private TMP_Text exchangeDetailText;

		// Token: 0x040074AE RID: 29870
		[SerializeField]
		private TMP_Text diaryInfoText;

		// Token: 0x040074AF RID: 29871
		[SerializeField]
		private TMP_Text forwardDescText;

		// Token: 0x040074B0 RID: 29872
		[SerializeField]
		private TMP_Text carryingText;

		// Token: 0x040074B1 RID: 29873
		[SerializeField]
		private List<UIChildScroll> bgChildScrolls;

		// Token: 0x040074B2 RID: 29874
		private List<ItemDisplayData> _inventoryItems = new List<ItemDisplayData>();

		// Token: 0x040074B3 RID: 29875
		private List<ItemDisplayData> _equipItems = new List<ItemDisplayData>();

		// Token: 0x040074B4 RID: 29876
		private List<ItemDisplayData> _carryingItems = new List<ItemDisplayData>();

		// Token: 0x040074B5 RID: 29877
		private Dictionary<int, List<ItemDisplayData>> stockDic = new Dictionary<int, List<ItemDisplayData>>();

		// Token: 0x040074B6 RID: 29878
		private bool _canTransfer;

		// Token: 0x040074B7 RID: 29879
		private CharacterDisplayData _taiwuDisplayData;

		// Token: 0x040074B8 RID: 29880
		private int _wareHouseCurrLoad;

		// Token: 0x040074B9 RID: 29881
		private int _wareHouseMaxLoad;

		// Token: 0x040074BA RID: 29882
		private int _inventoryCurrLoad;

		// Token: 0x040074BB RID: 29883
		private int _inventoryMaxLoad;

		// Token: 0x040074BC RID: 29884
		private TeaHorseCaravanData _teaHorseCaravanData;

		// Token: 0x040074BD RID: 29885
		private BuildingBlockData _buildingBlockData;

		// Token: 0x040074BE RID: 29886
		private BuildingBlockKey _buildingBlocKey;

		// Token: 0x040074BF RID: 29887
		private List<GameObject> _carryItemList = new List<GameObject>();

		// Token: 0x040074C0 RID: 29888
		private List<GameObject> _gainItemList = new List<GameObject>();

		// Token: 0x040074C1 RID: 29889
		private List<ValueTuple<ItemKey, sbyte>> _carryItemToExchangeList = new List<ValueTuple<ItemKey, sbyte>>();

		// Token: 0x040074C2 RID: 29890
		private List<ItemKey> _gainItemToExchangeList = new List<ItemKey>();

		// Token: 0x040074C3 RID: 29891
		private int _carryItemCount;

		// Token: 0x040074C4 RID: 29892
		private int _maxCarryItemCount;

		// Token: 0x040074C5 RID: 29893
		private string _diaryInfoItem = "DiaryInfoItem";

		// Token: 0x040074C6 RID: 29894
		private bool _isStartAddGoods = false;

		// Token: 0x040074C7 RID: 29895
		private bool _isFirstOpen = true;

		// Token: 0x040074C8 RID: 29896
		private bool _isShowDiary;

		// Token: 0x040074C9 RID: 29897
		private GameObject _diaryInfoScrollView;

		// Token: 0x040074CA RID: 29898
		private GameObject _exchangeItemPanel;

		// Token: 0x040074CB RID: 29899
		private const float TeaHorseAnimationScale = 1f;

		// Token: 0x040074CC RID: 29900
		private int _goodsTogState;

		// Token: 0x040074CD RID: 29901
		private sbyte _currCaravanState = 0;

		// Token: 0x040074CE RID: 29902
		private readonly float _goodsItemsInReceivedBottom = 100f;

		// Token: 0x040074CF RID: 29903
		private readonly float _goodsItemsInCarryBottom = 20f;

		// Token: 0x040074D0 RID: 29904
		private readonly List<string> _caravanAudioList = new List<string>
		{
			"caravan_carriage_idle",
			"caravan_carriage_rest",
			"caravan_carriage_run",
			"caravan_map_taiwu"
		};

		// Token: 0x040074D1 RID: 29905
		private readonly List<string> _weatherAudioList = new List<string>
		{
			"weather_hailstone",
			"weather_heavyrain",
			"weather_heavywind",
			"weather_lightrain",
			"weather_sandstorm",
			"weather_snow",
			"weather_snowstorm"
		};

		// Token: 0x040074D2 RID: 29906
		private readonly List<string> _weatherThunderAudioList = new List<string>
		{
			"weather_thunder1#20_80",
			"weather_thunder2#20_80",
			"weather_thunder3#20_80",
			"weather_thunder4#20_80"
		};

		// Token: 0x040074D3 RID: 29907
		private bool _firstEnter;

		// Token: 0x040074D4 RID: 29908
		private int _taiwuCharId;

		// Token: 0x040074D5 RID: 29909
		private bool _blockSelf;

		// Token: 0x040074D6 RID: 29910
		private bool _blockExchange;

		// Token: 0x040074D7 RID: 29911
		private bool _exchangingSupplies = false;

		// Token: 0x040074D8 RID: 29912
		[SerializeField]
		private CImage hoverImg;

		// Token: 0x040074D9 RID: 29913
		[SerializeField]
		private PositionFollower hoverFollower;

		// Token: 0x040074DA RID: 29914
		[SerializeField]
		private Sprite hoverSmall;

		// Token: 0x040074DB RID: 29915
		[SerializeField]
		private Sprite hoverLarge;

		// Token: 0x0200228A RID: 8842
		public class TeaHorseCaravanState
		{
			// Token: 0x0400DB4B RID: 56139
			public const sbyte None = 0;

			// Token: 0x0400DB4C RID: 56140
			public const sbyte Ready = 1;

			// Token: 0x0400DB4D RID: 56141
			public const sbyte Forward = 2;

			// Token: 0x0400DB4E RID: 56142
			public const sbyte Return = 3;

			// Token: 0x0400DB4F RID: 56143
			public const sbyte ReadyGetItem = 4;
		}

		// Token: 0x0200228B RID: 8843
		public class TeaHorseCaravanWeatherState
		{
			// Token: 0x0400DB50 RID: 56144
			public const sbyte ClearDay = 0;

			// Token: 0x0400DB51 RID: 56145
			public const sbyte Hailstone = 1;

			// Token: 0x0400DB52 RID: 56146
			public const sbyte Sandstorm = 2;

			// Token: 0x0400DB53 RID: 56147
			public const sbyte BurningSun = 3;

			// Token: 0x0400DB54 RID: 56148
			public const sbyte HeavyRain = 4;

			// Token: 0x0400DB55 RID: 56149
			public const sbyte LightRain = 5;

			// Token: 0x0400DB56 RID: 56150
			public const sbyte LightSnow = 6;

			// Token: 0x0400DB57 RID: 56151
			public const sbyte HeavySnow = 7;

			// Token: 0x0400DB58 RID: 56152
			public const sbyte HeavyWind = 8;

			// Token: 0x0400DB59 RID: 56153
			public const sbyte Fog = 9;
		}

		// Token: 0x0200228C RID: 8844
		private class TeaHorseCaravanGoodsTogState
		{
			// Token: 0x0400DB5A RID: 56154
			public const int Received = 0;

			// Token: 0x0400DB5B RID: 56155
			public const int Carry = 1;
		}
	}
}
