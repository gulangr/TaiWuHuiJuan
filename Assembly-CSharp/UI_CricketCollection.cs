using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using GameData.Domains.Building;
using GameData.Domains.Building.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;

// Token: 0x020001B5 RID: 437
public class UI_CricketCollection : UIBase
{
	// Token: 0x170002B7 RID: 695
	// (get) Token: 0x060019F9 RID: 6649 RVA: 0x000AA678 File Offset: 0x000A8878
	private ItemSourceType SelectedSourceType
	{
		get
		{
			bool flag = this._selectedSourceToggleIndex != -1;
			ItemSourceType result;
			if (flag)
			{
				result = this._sourceTypeArray[this._selectedSourceToggleIndex];
			}
			else
			{
				result = ItemSourceType.Invalid;
			}
			return result;
		}
	}

	// Token: 0x170002B8 RID: 696
	// (get) Token: 0x060019FA RID: 6650 RVA: 0x000AA6AC File Offset: 0x000A88AC
	private short SelectedSubType
	{
		get
		{
			bool flag = this._selectedTypeToggleIndex != -1;
			short result;
			if (flag)
			{
				result = this._itemSubTypeArray[this._selectedTypeToggleIndex];
			}
			else
			{
				result = -1;
			}
			return result;
		}
	}

	// Token: 0x060019FB RID: 6651 RVA: 0x000AA6E0 File Offset: 0x000A88E0
	public override void OnInit(ArgumentBox argsBox)
	{
		this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		this.NeedDataListenerId = true;
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
		bool inited = this._inited;
		if (inited)
		{
			this.GoToOpMode(UI_CricketCollection.OperationMode.Normal, false);
		}
	}

	// Token: 0x060019FC RID: 6652 RVA: 0x000AA744 File Offset: 0x000A8944
	private void Awake()
	{
		this._slotHolder = base.CGet<RectTransform>("SlotHolder");
		this._batchOperationView = base.CGet<Refers>("BatchOperationView");
		this._batchOperationSwitchButton = base.CGet<CButtonObsolete>("BatchOperationSwitchButton");
		this._removeAllJarButton = base.CGet<CButtonObsolete>("RemoveAllJarButton");
		this._removeAllJarButton.OnInteractableChange.AddListener(delegate(bool interactable)
		{
			this._removeAllJarButton.transform.Find("Icon").gameObject.SetActive(interactable);
			this._removeAllJarButton.transform.Find("IconDisable").gameObject.SetActive(!interactable);
		});
		this._addAllJarButton = base.CGet<CButtonObsolete>("AddAllJarButton");
		this._addAllJarButton.OnInteractableChange.AddListener(delegate(bool interactable)
		{
			this._addAllJarButton.transform.Find("Icon").gameObject.SetActive(interactable);
			this._addAllJarButton.transform.Find("IconDisable").gameObject.SetActive(!interactable);
		});
		this._removeAllCricketButton = base.CGet<CButtonObsolete>("RemoveAllCricketButton");
		this._removeAllCricketButton.OnInteractableChange.AddListener(delegate(bool interactable)
		{
			this._removeAllCricketButton.transform.Find("Icon").gameObject.SetActive(interactable);
			this._removeAllCricketButton.transform.Find("IconDisable").gameObject.SetActive(!interactable);
		});
		this._addAllCricketButton = base.CGet<CButtonObsolete>("AddAllCricketButton");
		this._addAllCricketButton.OnInteractableChange.AddListener(delegate(bool interactable)
		{
			this._addAllCricketButton.transform.Find("Icon").gameObject.SetActive(interactable);
			this._addAllCricketButton.transform.Find("IconDisable").gameObject.SetActive(!interactable);
		});
		this._mainWindow = base.CGet<GameObject>("MainWindow");
		this._batchFilterJarToggle = this._batchOperationView.CGet<CToggleObsolete>("BatchFilterJarToggle");
		this._batchFilterCricketToggle = this._batchOperationView.CGet<CToggleObsolete>("BatchFilterCricketToggle");
		this._inventoryToggle = this._batchOperationView.CGet<CToggleObsolete>("Inventory");
		this._warehouseToggle = this._batchOperationView.CGet<CToggleObsolete>("Warehouse");
		this._itemScrollView = this._batchOperationView.CGet<ItemScrollView>("ItemScrollView");
		this._itemScrollView.Init();
		this._itemScrollView.SimpleViewLineCount = 16;
		this._itemScrollView.DetailViewLineCount = 7;
		this._itemScrollView.SetItemList(ref this._batchModeDisplayDatas, true, null, true, new Action<ItemDisplayData, ItemView>(this.OnRenderBatchModeItem));
		this._closeBatchOperationViewButton = this._batchOperationView.CGet<CButtonObsolete>("CloseBatchOperationViewButton");
		this._sourceTogGroup = this._batchOperationView.CGet<CToggleGroupObsolete>("SourceTogGroup");
		this._sourceTogGroup.OnActiveToggleChange = delegate(CToggleObsolete newToggle, CToggleObsolete oldToggle)
		{
			this._selectedSourceToggleIndex = newToggle.Key;
			bool flag = !this._batchFilterJarToggle.interactable;
			if (flag)
			{
				this._batchFilterJarToggle.interactable = true;
				this._batchFilterJarToggle.GetComponent<PointerTrigger>().enabled = true;
			}
			this.RefreshRemoveAllJarButton();
			bool flag2 = this._operationMode == UI_CricketCollection.OperationMode.Batch;
			if (flag2)
			{
				this.RefreshSlotsWithMode();
				this._needReSelectCricketInBatchMode = true;
				this.GetBatchDisplayData();
			}
		};
		this._sourceTogGroup.InitPreOnToggle(-1);
		this._typeToggleGroup = this._batchOperationView.CGet<CToggleGroupObsolete>("TypeToggleGroup");
		this._typeToggleGroup.OnActiveToggleChange = delegate(CToggleObsolete newToggle, CToggleObsolete oldToggle)
		{
			this._selectedTypeToggleIndex = newToggle.Key;
			bool flag = this._operationMode == UI_CricketCollection.OperationMode.Batch;
			if (flag)
			{
				this.GetBatchDisplayData();
				this._needReSelectCricketInBatchMode = true;
				this.RefreshSlotsWithMode();
			}
		};
		this._typeToggleGroup.InitPreOnToggle(-1);
		this._collectionJarList = new ItemKey[this._slotHolder.childCount];
		this._collectionCricketList = new ItemKey[this._slotHolder.childCount];
		for (int i = 0; i < this._slotHolder.childCount; i++)
		{
			int index = i;
			Refers slotRefers = this._slotHolder.GetChild(i).GetComponent<Refers>();
			CButtonObsolete changeCricketBtn = slotRefers.CGet<CButtonObsolete>("ChangeCricket");
			PointerTrigger cricketPointerTrigger = slotRefers.CGet<CricketView>("Cricket").GetComponent<PointerTrigger>();
			PointerTrigger btnPointerTrigger = changeCricketBtn.GetComponent<PointerTrigger>();
			this._collectionJarList[i] = ItemKey.Invalid;
			this._collectionCricketList[i] = ItemKey.Invalid;
			slotRefers.CGet<CButtonObsolete>("AddJar").onClick.AddListener(delegate()
			{
				this.ShowSelectItemWindow(index, false);
			});
			slotRefers.CGet<CButtonObsolete>("ChangeJar").onClick.AddListener(delegate()
			{
				this.ShowSelectItemWindow(index, false);
			});
			slotRefers.CGet<CButtonObsolete>("AddCricket").onClick.AddListener(delegate()
			{
				this.ShowSelectItemWindow(index, true);
			});
			CButtonObsolete batchModeButton = slotRefers.CGet<CButtonObsolete>("BatchModeButton");
			CToggleObsolete batchModeToggle = slotRefers.CGet<CToggleObsolete>("BatchModeToggle");
			batchModeToggle.isOn = false;
			batchModeButton.GetComponent<PointerTrigger>().enabled = (this._operationMode == UI_CricketCollection.OperationMode.Batch);
			batchModeButton.gameObject.SetActive(this._operationMode == UI_CricketCollection.OperationMode.Batch);
			batchModeButton.ClearAndAddListener(delegate
			{
				bool flag = this._operationMode == UI_CricketCollection.OperationMode.Batch;
				if (flag)
				{
					batchModeToggle.isOn = true;
					this._curSelectedIndex = index;
				}
			});
			changeCricketBtn.onClick.AddListener(delegate()
			{
				this.ShowSelectItemWindow(index, true);
			});
			changeCricketBtn.gameObject.SetActive(false);
			cricketPointerTrigger.EnterEvent.AddListener(delegate()
			{
				changeCricketBtn.gameObject.SetActive(this._operationMode == UI_CricketCollection.OperationMode.Normal);
			});
			cricketPointerTrigger.ExitEvent.AddListener(delegate()
			{
				bool flag = !RectTransformUtility.RectangleContainsScreenPoint(cricketPointerTrigger.GetComponent<RectTransform>(), Input.mousePosition, UIManager.Instance.UiCamera);
				if (flag)
				{
					changeCricketBtn.gameObject.SetActive(false);
				}
			});
			btnPointerTrigger.ExitEvent.AddListener(delegate()
			{
				bool flag = !RectTransformUtility.RectangleContainsScreenPoint(cricketPointerTrigger.GetComponent<RectTransform>(), Input.mousePosition, UIManager.Instance.UiCamera);
				if (flag)
				{
					changeCricketBtn.gameObject.SetActive(false);
				}
			});
		}
		this._inited = true;
	}

	// Token: 0x060019FD RID: 6653 RVA: 0x000AABB0 File Offset: 0x000A8DB0
	private void OnDisable()
	{
		this._cricketRefreshMap.Clear();
		this._selectedSourceToggleIndex = 0;
		this._batchModeDisplayDatas.Clear();
		this._itemScrollView.SetItemList(ref this._batchModeDisplayDatas, false, null, false, null);
	}

	// Token: 0x060019FE RID: 6654 RVA: 0x000AABE8 File Offset: 0x000A8DE8
	public void OnListenerIdReady()
	{
		this.GetAllInfo();
	}

	// Token: 0x060019FF RID: 6655 RVA: 0x000AABF4 File Offset: 0x000A8DF4
	private void GetAllInfo()
	{
		TaiwuDomainMethod.Call.GetCanOperateItemDisplayDataInVillage(this.Element.GameDataListenerId, 1100);
		TaiwuDomainMethod.Call.GetCanOperateItemDisplayDataInVillage(this.Element.GameDataListenerId, 1201);
		TaiwuDomainMethod.Call.CanTransferItemToWarehouse(this.Element.GameDataListenerId);
		BuildingDomainMethod.Call.GetCollectionJars(this.Element.GameDataListenerId);
		BuildingDomainMethod.Call.GetCollectionCrickets(this.Element.GameDataListenerId);
		BuildingDomainMethod.Call.GetCollectionCricketRegen(this.Element.GameDataListenerId);
		BuildingDomainMethod.Call.GetAuthorityGain(this.Element.GameDataListenerId);
		BuildingDomainMethod.Call.GetBatchButtonEnableState(this.Element.GameDataListenerId);
	}

	// Token: 0x06001A00 RID: 6656 RVA: 0x000AAC94 File Offset: 0x000A8E94
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 1)
			{
				bool flag = notification.DomainId == 5;
				if (flag)
				{
					bool flag2 = notification.MethodId == 55;
					if (flag2)
					{
						List<ItemDisplayData> itemDataList = new List<ItemDisplayData>();
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref itemDataList);
						bool flag3 = itemDataList.Count == 0;
						if (!flag3)
						{
							List<ItemKey> itemList = (itemDataList[0].Key.ItemType == 11) ? this._canOperateCricketList : this._canOperateJarList;
							for (int i = 0; i < itemDataList.Count; i++)
							{
								ItemDisplayData itemData = itemDataList[i];
								itemList.Add(itemData.Key);
								this._itemDisplayDataDict[itemData.Key] = itemData;
							}
						}
					}
					else
					{
						bool flag4 = notification.MethodId == 42;
						if (flag4)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._canTransferItemToWarehouse);
							this.RefreshBatchView();
						}
					}
				}
				else
				{
					bool flag5 = notification.DomainId == 9;
					if (flag5)
					{
						bool flag6 = notification.MethodId == 33;
						if (flag6)
						{
							ItemDisplayData[] collectionJars = null;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref collectionJars);
							for (int j = 0; j < collectionJars.Length; j++)
							{
								this._collectionJarList[j] = collectionJars[j].Key;
								this._itemDisplayDataDict[collectionJars[j].Key] = collectionJars[j];
								this.UpdateJarDisplay(j);
							}
						}
						else
						{
							bool flag7 = notification.MethodId == 32;
							if (flag7)
							{
								ItemDisplayData[] collectionCrickets = null;
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref collectionCrickets);
								for (int k = 0; k < collectionCrickets.Length; k++)
								{
									this._collectionCricketList[k] = collectionCrickets[k].Key;
									this._itemDisplayDataDict[collectionCrickets[k].Key] = collectionCrickets[k];
									this.UpdateCricketDisplay(k);
								}
								this.UpdateTitle();
								this.RefreshSlotsWithMode();
								ItemDomainMethod.Call.GetCricketsAliveState(this.Element.GameDataListenerId, this._collectionCricketList.ToList<ItemKey>());
							}
							else
							{
								bool flag8 = notification.MethodId == 34;
								if (flag8)
								{
									int[] collectionCricketRegen = null;
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref collectionCricketRegen);
									for (int l = 0; l < collectionCricketRegen.Length; l++)
									{
										ItemKey jarKey = this._collectionJarList[l];
										Refers refers = this._slotHolder.GetChild(l).GetComponent<Refers>();
										CImage progressImg = refers.CGet<CImage>("HealProgress");
										float healProgress = jarKey.IsValid() ? ((float)collectionCricketRegen[l] / (float)GameData.Domains.Building.SharedMethods.CalcCricketRegenTime(Misc.Instance[jarKey.TemplateId].Grade)) : 0f;
										progressImg.fillAmount = healProgress;
									}
								}
								else
								{
									bool flag9 = notification.MethodId == 35;
									if (flag9)
									{
										int addAuthority = 0;
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref addAuthority);
										base.CGet<TextMeshProUGUI>("AddAuthority").text = string.Format("+{0}", addAuthority);
										this.Element.ShowAfterRefresh();
									}
									else
									{
										bool flag10 = notification.MethodId == 131;
										if (flag10)
										{
											Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._batchModeDisplayDatas);
											this._batchModeDisplayDatas.Insert(0, new ItemDisplayData());
											this.RefreshBatchModeItemList();
										}
										else
										{
											bool flag11 = notification.MethodId == 133;
											if (flag11)
											{
												Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._batchModeButtonStateData);
												this.RefreshBatchButtons();
											}
										}
									}
								}
							}
						}
					}
					else
					{
						bool flag12 = notification.DomainId == 6;
						if (flag12)
						{
							bool flag13 = notification.MethodId == 23;
							if (flag13)
							{
								bool[] aliveStateArray = null;
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref aliveStateArray);
								for (int m = 0; m < aliveStateArray.Length; m++)
								{
									bool alive = aliveStateArray[m];
									Refers refers2 = this._slotHolder.GetChild(m).GetComponent<Refers>();
									CImage iconImage = refers2.CGet<CImage>("Icon");
									string iconName = alive ? "building_icon_huifu" : "building_icon_cricketdead";
									iconImage.SetSprite(iconName, true, null);
									CImage progressImg2 = refers2.CGet<CImage>("HealProgress");
									progressImg2.enabled = alive;
									TooltipInvoker tip = refers2.CGet<TooltipInvoker>("DurabilityLayout");
									bool flag14 = tip.PresetParam == null || tip.PresetParam.Length == 0;
									if (flag14)
									{
										tip.PresetParam = new string[1];
									}
									LanguageKey key = alive ? LanguageKey.LK_CricketCollection_Durability_Alive : LanguageKey.LK_CricketCollection_Durability_Dead;
									tip.PresetParam[0] = LocalStringManager.Get(key);
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06001A01 RID: 6657 RVA: 0x000AB210 File Offset: 0x000A9410
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		string text = btnName;
		string text2 = text;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(text2);
		if (num <= 1798226640U)
		{
			if (num != 220832923U)
			{
				if (num != 511380535U)
				{
					if (num == 1798226640U)
					{
						if (text2 == "BatchOperationSwitchButton")
						{
							this.SwitchOpMode();
						}
					}
				}
				else if (text2 == "CloseBatchOperationViewButton")
				{
					this.GoToOpMode(UI_CricketCollection.OperationMode.Normal, true);
				}
			}
			else if (text2 == "RemoveAllCricketButton")
			{
				this.RemoveAllCricket();
			}
		}
		else if (num <= 2473055052U)
		{
			if (num != 2379669205U)
			{
				if (num == 2473055052U)
				{
					if (text2 == "AddAllJarButton")
					{
						this.AddAllJar();
					}
				}
			}
			else if (text2 == "RemoveAllJarButton")
			{
				this.RemoveAllJar();
			}
		}
		else if (num != 3113711670U)
		{
			if (num == 3448155331U)
			{
				if (text2 == "Close")
				{
					this.GoToOpMode(UI_CricketCollection.OperationMode.Normal, false);
					this.QuickHide();
				}
			}
		}
		else if (text2 == "AddAllCricketButton")
		{
			this.AddAllCricket();
		}
	}

	// Token: 0x06001A02 RID: 6658 RVA: 0x000AB350 File Offset: 0x000A9550
	public override void QuickHide()
	{
		base.QuickHide();
		AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
		for (int i = 0; i < this._slotHolder.childCount; i++)
		{
			this._slotHolder.GetChild(i).GetComponent<Refers>().CGet<CButtonObsolete>("ChangeCricket").gameObject.SetActive(false);
		}
	}

	// Token: 0x06001A03 RID: 6659 RVA: 0x000AB3B8 File Offset: 0x000A95B8
	public void ShowSelectItemWindow(int index, bool isCricket)
	{
		this._curSelectedIndex = index;
		ArgumentBox argumentBox = new ArgumentBox();
		argumentBox.Set("showNone", true);
		argumentBox.Set("fromWhereSelect", 1);
		argumentBox.Set("itemSubType", isCricket ? 1100 : 1201);
		argumentBox.SetObject("filterType", ItemSortAndFilter.ItemFilterType.Other);
		if (isCricket)
		{
			argumentBox.SetObject("callback", new Action<ItemKey>(this.OnSelectCricket));
			argumentBox.SetObject("initItemKey", this._collectionCricketList[index]);
		}
		else
		{
			argumentBox.SetObject("callback", new Action<ItemKey>(this.OnSelectCricketJar));
			argumentBox.SetObject("initItemKey", this._collectionJarList[index]);
		}
		if (isCricket)
		{
			this._slotHolder.GetChild(index).GetComponent<Refers>().CGet<CButtonObsolete>("ChangeCricket").gameObject.SetActive(false);
		}
		UIElement.SelectItemLegacy.SetOnInitArgs(argumentBox);
		UIManager.Instance.ShowUI(UIElement.SelectItemLegacy, true);
	}

	// Token: 0x06001A04 RID: 6660 RVA: 0x000AB4D8 File Offset: 0x000A96D8
	public void OnSelectCricketJar(ItemKey jarKey)
	{
		ItemKey lastJarKey = this._collectionJarList[this._curSelectedIndex];
		bool flag = jarKey.Equals(lastJarKey);
		if (!flag)
		{
			bool flag2 = lastJarKey.IsValid();
			if (flag2)
			{
				this._canOperateJarList.Add(lastJarKey);
				BuildingDomainMethod.Call.CricketCollectionRemove(this._curSelectedIndex, false);
			}
			bool flag3 = jarKey.IsValid();
			if (flag3)
			{
				this._canOperateJarList.Remove(jarKey);
				BuildingDomainMethod.Call.CricketCollectionAdd(this._curSelectedIndex, false, jarKey);
			}
			else
			{
				bool flag4 = this._collectionCricketList[this._curSelectedIndex].IsValid();
				if (flag4)
				{
					this.OnSelectCricket(ItemKey.Invalid);
				}
			}
			this._collectionJarList[this._curSelectedIndex] = jarKey;
			this.UpdateJarDisplay(this._curSelectedIndex);
			this.ClearHealProgress(this._curSelectedIndex);
			BuildingDomainMethod.Call.GetBatchButtonEnableState(this.Element.GameDataListenerId);
		}
	}

	// Token: 0x06001A05 RID: 6661 RVA: 0x000AB5C4 File Offset: 0x000A97C4
	public void OnSelectCricket(ItemKey cricketKey)
	{
		ItemKey lastCricketKey = this._collectionCricketList[this._curSelectedIndex];
		bool flag = cricketKey.Equals(lastCricketKey);
		if (!flag)
		{
			bool flag2 = lastCricketKey.IsValid();
			if (flag2)
			{
				this._canOperateCricketList.Add(lastCricketKey);
				BuildingDomainMethod.Call.CricketCollectionRemove(this._curSelectedIndex, true);
			}
			bool flag3 = cricketKey.IsValid();
			if (flag3)
			{
				this._canOperateCricketList.Remove(cricketKey);
				BuildingDomainMethod.Call.CricketCollectionAdd(this._curSelectedIndex, true, cricketKey);
			}
			this._collectionCricketList[this._curSelectedIndex] = cricketKey;
			this.UpdateCricketDisplay(this._curSelectedIndex);
			this.UpdateTitle();
			this.ClearHealProgress(this._curSelectedIndex);
			BuildingDomainMethod.Call.GetAuthorityGain(this.Element.GameDataListenerId);
			ItemDomainMethod.Call.GetCricketsAliveState(this.Element.GameDataListenerId, this._collectionCricketList.ToList<ItemKey>());
			BuildingDomainMethod.Call.GetBatchButtonEnableState(this.Element.GameDataListenerId);
		}
	}

	// Token: 0x06001A06 RID: 6662 RVA: 0x000AB6B8 File Offset: 0x000A98B8
	private void UpdateTitle()
	{
		int cricketCount = 0;
		for (int i = 0; i < this._collectionCricketList.Length; i++)
		{
			bool flag = this._collectionCricketList[i].IsValid();
			if (flag)
			{
				cricketCount++;
			}
		}
		base.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.GetFormat(LanguageKey.LK_CricketCollection_Title, cricketCount, this._collectionCricketList.Length);
	}

	// Token: 0x06001A07 RID: 6663 RVA: 0x000AB728 File Offset: 0x000A9928
	private void UpdateJarDisplay(int index)
	{
		Refers slotRefers = this._slotHolder.GetChild(index).GetComponent<Refers>();
		ItemKey jarKey = this._collectionJarList[index];
		bool hasJar = jarKey.IsValid();
		slotRefers.CGet<CImage>("Jar").SetSprite(hasJar ? "building_ququguan_1" : "building_ququguan_0", false, null);
		bool isBatchMode = this._operationMode == UI_CricketCollection.OperationMode.Batch;
		bool isNormalMode = this._operationMode == UI_CricketCollection.OperationMode.Normal;
		slotRefers.CGet<CButtonObsolete>("AddJar").gameObject.SetActive(!hasJar && isNormalMode);
		slotRefers.CGet<CButtonObsolete>("ChangeJar").gameObject.SetActive(hasJar && isNormalMode);
		slotRefers.CGet<CButtonObsolete>("AddCricket").gameObject.SetActive(hasJar && !this._collectionCricketList[index].IsValid() && isNormalMode);
		slotRefers.CGet<GameObject>("JarInfo").SetActive(hasJar);
		bool isInBatchModeCricketTab = isBatchMode && this._selectedTypeToggleIndex == this._batchFilterCricketToggle.Key;
		CButtonObsolete batchModeButton = slotRefers.CGet<CButtonObsolete>("BatchModeButton");
		CToggleObsolete batchModeToggle = slotRefers.CGet<CToggleObsolete>("BatchModeToggle");
		bool hasCricket = this._collectionCricketList[index].IsValid();
		this.RefreshCricketTips(batchModeButton.GetComponent<TooltipInvoker>(), isBatchMode && hasCricket, this._itemDisplayDataDict.GetValueOrDefault(this._collectionCricketList[index]), false);
		batchModeToggle.isOn = (isBatchMode && this._curSelectedIndex == index);
		bool canOperationThisSlot = isBatchMode && (!isInBatchModeCricketTab || hasJar);
		batchModeButton.gameObject.SetActive(isBatchMode);
		batchModeButton.interactable = canOperationThisSlot;
		batchModeButton.GetComponent<PointerTrigger>().enabled = canOperationThisSlot;
		bool flag = hasJar;
		if (flag)
		{
			MiscItem jarConfig = Misc.Instance[jarKey.TemplateId];
			slotRefers.CGet<CImage>("GradeBack").SetSprite(ItemView.GetGradeIcon(jarConfig.Grade), false, null);
			slotRefers.CGet<TextMeshProUGUI>("JarGrade").text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", jarConfig.Grade));
			slotRefers.CGet<TextMeshProUGUI>("JarName").text = jarConfig.Name;
		}
	}

	// Token: 0x06001A08 RID: 6664 RVA: 0x000AB950 File Offset: 0x000A9B50
	private void UpdateCricketDisplay(int index)
	{
		Refers slotRefers = this._slotHolder.GetChild(index).GetComponent<Refers>();
		ItemKey cricketKey = this._collectionCricketList[index];
		bool hasCricket = cricketKey.IsValid();
		CricketView cricketView = slotRefers.CGet<CricketView>("Cricket");
		cricketView.gameObject.SetActive(hasCricket);
		slotRefers.CGet<CButtonObsolete>("AddCricket").gameObject.SetActive(this._collectionJarList[index].IsValid() && !hasCricket && this._operationMode == UI_CricketCollection.OperationMode.Normal);
		slotRefers.CGet<GameObject>("CricketInfo").SetActive(hasCricket);
		bool isCricketChanged = false;
		UI_CricketCollection.CricketAnimationState state;
		bool flag = !this._cricketRefreshMap.TryGetValue(index, out state);
		if (flag)
		{
			isCricketChanged = true;
		}
		else
		{
			bool flag2 = state.Cricket != cricketKey;
			if (flag2)
			{
				isCricketChanged = true;
			}
		}
		bool flag3 = hasCricket && isCricketChanged;
		if (flag3)
		{
			ItemDisplayData displayData = this._itemDisplayDataDict[cricketKey];
			cricketView.SetCricketData(displayData.CricketColorId, displayData.CricketPartId, true, displayData, false);
			bool isUiVisible = base.gameObject.activeInHierarchy;
			bool flag4 = displayData.Durability > 0;
			if (flag4)
			{
				bool flag5 = isUiVisible;
				if (flag5)
				{
					Coroutine coroutine = base.StartCoroutine(this.RandomDelay(delegate
					{
						cricketView.PlayAnimation(ECricketAnim.Idle, true, false);
						cricketView.Sing(true, true, true, 1f, null, 0f);
					}));
					this._cricketRefreshMap[index] = new UI_CricketCollection.CricketAnimationState
					{
						Cricket = cricketKey,
						AnimationCoroutine = coroutine
					};
				}
			}
			else
			{
				bool flag6 = isUiVisible;
				if (flag6)
				{
					UI_CricketCollection.CricketAnimationState animationState;
					bool flag7 = this._cricketRefreshMap.TryGetValue(index, out animationState) && animationState.AnimationCoroutine != null;
					if (flag7)
					{
						base.StopCoroutine(animationState.AnimationCoroutine);
					}
					this._cricketRefreshMap.Remove(index);
				}
				cricketView.StopAnimation();
				cricketView.StopLoopSing();
			}
			slotRefers.CGet<TextMeshProUGUI>("CricketName").text = cricketView.Name;
			slotRefers.CGet<TextMeshProUGUI>("Durability").text = string.Format("{0}/{1}", displayData.Durability, displayData.MaxDurability);
			CImage cimage = slotRefers.CGet<CImage>("CricketGradeBack");
			if (cimage != null)
			{
				cimage.SetSprite(ItemView.GetGradeIcon(ItemTemplateHelper.GetCricketGrade(cricketView.ColorId, cricketView.PartId)), false, null);
			}
			TextMeshProUGUI textMeshProUGUI = slotRefers.CGet<TextMeshProUGUI>("CricketGrade");
			if (textMeshProUGUI != null)
			{
				textMeshProUGUI.SetText(LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", ItemTemplateHelper.GetCricketGrade(cricketView.ColorId, cricketView.PartId))), true);
			}
		}
		bool flag8 = !hasCricket;
		if (flag8)
		{
			UI_CricketCollection.CricketAnimationState animationState2;
			bool flag9 = this._cricketRefreshMap.TryGetValue(index, out animationState2) && animationState2.AnimationCoroutine != null;
			if (flag9)
			{
				base.StopCoroutine(animationState2.AnimationCoroutine);
			}
			this._cricketRefreshMap.Remove(index);
		}
		cricketView.GetComponent<CEmptyGraphic>().enabled = (this._operationMode == UI_CricketCollection.OperationMode.Normal);
	}

	// Token: 0x06001A09 RID: 6665 RVA: 0x000ABC6B File Offset: 0x000A9E6B
	private IEnumerator RandomDelay(Action action)
	{
		float delayTime = Random.Range(0f, 2f);
		yield return new WaitForSeconds(delayTime);
		if (action != null)
		{
			action();
		}
		yield break;
	}

	// Token: 0x06001A0A RID: 6666 RVA: 0x000ABC81 File Offset: 0x000A9E81
	private void ClearHealProgress(int index)
	{
		this._slotHolder.GetChild(index).GetComponent<Refers>().CGet<CImage>("HealProgress").fillAmount = 0f;
	}

	// Token: 0x06001A0B RID: 6667 RVA: 0x000ABCAC File Offset: 0x000A9EAC
	private void SwitchOpMode()
	{
		UI_CricketCollection.OperationMode operationMode2 = this._operationMode;
		if (!true)
		{
		}
		UI_CricketCollection.OperationMode operationMode3;
		if (operationMode2 != UI_CricketCollection.OperationMode.Normal)
		{
			if (operationMode2 != UI_CricketCollection.OperationMode.Batch)
			{
				throw new ArgumentOutOfRangeException();
			}
			operationMode3 = UI_CricketCollection.OperationMode.Normal;
		}
		else
		{
			operationMode3 = UI_CricketCollection.OperationMode.Batch;
		}
		if (!true)
		{
		}
		UI_CricketCollection.OperationMode operationMode = operationMode3;
		this.GoToOpMode(operationMode, true);
	}

	// Token: 0x06001A0C RID: 6668 RVA: 0x000ABCF0 File Offset: 0x000A9EF0
	private void GoToOpMode(UI_CricketCollection.OperationMode operationMode, bool playAnimation = true)
	{
		if (operationMode != UI_CricketCollection.OperationMode.Normal)
		{
			if (operationMode == UI_CricketCollection.OperationMode.Batch)
			{
				if (playAnimation)
				{
					this._mainWindow.GetComponent<RectTransform>().DOLocalMoveY(300f, 0.5f, false).SetEase(Ease.OutExpo);
				}
				else
				{
					this._mainWindow.GetComponent<RectTransform>().localPosition = Vector2.zero;
				}
				this._operationMode = UI_CricketCollection.OperationMode.Batch;
				this._batchOperationView.gameObject.SetActive(true);
				this._isSwitchingMode = true;
				TaiwuDomainMethod.Call.CanTransferItemToWarehouse(this.Element.GameDataListenerId);
			}
		}
		else
		{
			if (playAnimation)
			{
				this._mainWindow.GetComponent<RectTransform>().DOLocalMoveY(0f, 0.5f, false).SetEase(Ease.OutExpo);
			}
			this._operationMode = UI_CricketCollection.OperationMode.Normal;
			this._batchOperationView.gameObject.SetActive(false);
		}
		this._needReSelectCricketInBatchMode = true;
		this.RefreshSlotsWithMode();
	}

	// Token: 0x06001A0D RID: 6669 RVA: 0x000ABDDC File Offset: 0x000A9FDC
	private void RefreshBatchView()
	{
		this._inventoryToggle.interactable = this._canTransferItemToWarehouse;
		bool flag = this._operationMode == UI_CricketCollection.OperationMode.Batch;
		if (flag)
		{
			bool isSwitchingMode = this._isSwitchingMode;
			if (isSwitchingMode)
			{
				this.AutoSelectSource();
				this.AutoSelectItemSubType();
			}
			this.GetBatchDisplayData();
		}
		this._isSwitchingMode = false;
	}

	// Token: 0x06001A0E RID: 6670 RVA: 0x000ABE34 File Offset: 0x000AA034
	private void GetBatchDisplayData()
	{
		bool flag = this.SelectedSubType == -1 || this.SelectedSourceType == ItemSourceType.Invalid;
		if (flag)
		{
			Debug.LogWarning("Invalid subType or sourceType");
		}
		else
		{
			bool flag2 = !this._canTransferItemToWarehouse && this.SelectedSourceType == ItemSourceType.Inventory;
			if (flag2)
			{
				Debug.LogWarning("Can't transfer item to warehouse");
			}
			else
			{
				BuildingDomainMethod.Call.GetCricketOrJarFromSourceStorage(this.Element.GameDataListenerId, this.SelectedSubType, this.SelectedSourceType);
			}
		}
	}

	// Token: 0x06001A0F RID: 6671 RVA: 0x000ABEAC File Offset: 0x000AA0AC
	private void RefreshBatchModeItemList()
	{
		this._itemScrollView.SetItemList(ref this._batchModeDisplayDatas, false, null, false, null);
	}

	// Token: 0x06001A10 RID: 6672 RVA: 0x000ABEC5 File Offset: 0x000AA0C5
	private void RefreshBatchButtons()
	{
		this.RefreshRemoveAllCricketButton();
		this.RefreshRemoveAllJarButton();
		this.RefreshAddAllCricketButton();
		this.RefreshAddAllCricketJarButton();
	}

	// Token: 0x06001A11 RID: 6673 RVA: 0x000ABEE4 File Offset: 0x000AA0E4
	private void RefreshRemoveAllJarButton()
	{
		this._removeAllJarButton.interactable = this._batchModeButtonStateData.HasJarInCollection;
		TooltipInvoker tipDisplayer = this._removeAllJarButton.GetComponent<TooltipInvoker>();
		TooltipInvoker tooltipInvoker = tipDisplayer;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		tipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_RemoveAllJar));
		bool interactable = this._removeAllJarButton.interactable;
		if (interactable)
		{
			tipDisplayer.RuntimeParam.Set("arg1", LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_RemoveAllJar_Tips));
		}
		else
		{
			tipDisplayer.RuntimeParam.Set("arg1", LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_RemoveAllJar_Tips2));
		}
		tipDisplayer.Refresh(false, -1);
	}

	// Token: 0x06001A12 RID: 6674 RVA: 0x000ABF98 File Offset: 0x000AA198
	private void RefreshRemoveAllCricketButton()
	{
		this._removeAllCricketButton.interactable = this._batchModeButtonStateData.HasCricketInCollection;
		TooltipInvoker tipDisplayer = this._removeAllCricketButton.GetComponent<TooltipInvoker>();
		TooltipInvoker tooltipInvoker = tipDisplayer;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		tipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_RemoveAllCricket));
		bool interactable = this._removeAllCricketButton.interactable;
		if (interactable)
		{
			tipDisplayer.RuntimeParam.Set("arg1", LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_RemoveAllCricket_Tips));
		}
		else
		{
			tipDisplayer.RuntimeParam.Set("arg1", LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_RemoveAllCricket_Tips2));
		}
		tipDisplayer.Refresh(false, -1);
	}

	// Token: 0x06001A13 RID: 6675 RVA: 0x000AC04C File Offset: 0x000AA24C
	private void RefreshAddAllCricketJarButton()
	{
		this._addAllJarButton.interactable = (this._batchModeButtonStateData.HasJarInSources && this._batchModeButtonStateData.HasEmptyPositionInCollection);
		TooltipInvoker tipDisplayer = this._addAllJarButton.GetComponent<TooltipInvoker>();
		TooltipInvoker tooltipInvoker = tipDisplayer;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		tipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_AddAllJar));
		bool interactable = this._addAllJarButton.interactable;
		if (interactable)
		{
			tipDisplayer.RuntimeParam.Set("arg1", LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_AddAllJar_Tips));
		}
		else
		{
			bool flag = !this._batchModeButtonStateData.HasJarInSources;
			if (flag)
			{
				tipDisplayer.RuntimeParam.Set("arg1", LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_AddAllJar_Tips2));
			}
			else
			{
				bool flag2 = !this._batchModeButtonStateData.HasEmptyPositionInCollection;
				if (flag2)
				{
					tipDisplayer.RuntimeParam.Set("arg1", LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_AddAllJar_Tips3));
				}
			}
		}
		tipDisplayer.Refresh(false, -1);
	}

	// Token: 0x06001A14 RID: 6676 RVA: 0x000AC158 File Offset: 0x000AA358
	private void RefreshAddAllCricketButton()
	{
		this._addAllCricketButton.interactable = (this._batchModeButtonStateData.HasCricketInSources && this._batchModeButtonStateData.HasEmptyJarInCollection);
		TooltipInvoker tipDisplayer = this._addAllCricketButton.GetComponent<TooltipInvoker>();
		TooltipInvoker tooltipInvoker = tipDisplayer;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		tipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_AddAllCricket));
		bool interactable = this._addAllCricketButton.interactable;
		if (interactable)
		{
			tipDisplayer.RuntimeParam.Set("arg1", LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_AddAllCricket_Tips));
		}
		else
		{
			bool flag = !this._batchModeButtonStateData.HasCricketInSources;
			if (flag)
			{
				tipDisplayer.RuntimeParam.Set("arg1", LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_AddAllCricket_Tips2));
			}
			else
			{
				bool flag2 = !this._batchModeButtonStateData.HasEmptyJarInCollection;
				if (flag2)
				{
					tipDisplayer.RuntimeParam.Set("arg1", LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_AddAllCricket_Tips3));
				}
			}
		}
		tipDisplayer.Refresh(false, -1);
	}

	// Token: 0x06001A15 RID: 6677 RVA: 0x000AC264 File Offset: 0x000AA464
	private void OnRenderBatchModeItem(ItemDisplayData itemData, ItemView itemView)
	{
		itemView.SetClickEvent(delegate
		{
			this.OnClickItem(itemView);
		});
	}

	// Token: 0x06001A16 RID: 6678 RVA: 0x000AC2A0 File Offset: 0x000AA4A0
	private void OnClickItem(ItemView itemView)
	{
		bool flag = this._curSelectedIndex == -1;
		if (!flag)
		{
			BuildingDomainMethod.Call.SmartOperateCricketOrJarCollection(this._curSelectedIndex, this.SelectedSubType, this.SelectedSourceType, itemView.Data.Key);
			this.GetAllInfo();
		}
	}

	// Token: 0x06001A17 RID: 6679 RVA: 0x000AC2E7 File Offset: 0x000AA4E7
	private void AutoSelectItemSubType()
	{
		this._typeToggleGroup.Set(this._batchFilterJarToggle.Key, true, false);
		this._selectedTypeToggleIndex = 0;
	}

	// Token: 0x06001A18 RID: 6680 RVA: 0x000AC30C File Offset: 0x000AA50C
	private void AutoSelectSource()
	{
		bool canTransferItemToWarehouse = this._canTransferItemToWarehouse;
		if (canTransferItemToWarehouse)
		{
			this._sourceTogGroup.Set(this._inventoryToggle.Key, true, false);
			this._selectedSourceToggleIndex = 0;
		}
		else
		{
			this._sourceTogGroup.Set(this._warehouseToggle.Key, true, false);
			this._selectedSourceToggleIndex = 1;
		}
	}

	// Token: 0x06001A19 RID: 6681 RVA: 0x000AC36A File Offset: 0x000AA56A
	private void AddAllCricket()
	{
		BuildingDomainMethod.Call.CricketCollectionBatchAddCricket();
		this._needReSelectCricketInBatchMode = true;
		this.GetAllInfo();
	}

	// Token: 0x06001A1A RID: 6682 RVA: 0x000AC381 File Offset: 0x000AA581
	private void RemoveAllCricket()
	{
		BuildingDomainMethod.Call.CricketCollectionBatchRemoveCricket(this.SelectedSourceType);
		this._needReSelectCricketInBatchMode = true;
		this.GetAllInfo();
	}

	// Token: 0x06001A1B RID: 6683 RVA: 0x000AC39E File Offset: 0x000AA59E
	private void AddAllJar()
	{
		BuildingDomainMethod.Call.CricketCollectionBatchAddCricketJar();
		this._needReSelectCricketInBatchMode = true;
		this.GetAllInfo();
	}

	// Token: 0x06001A1C RID: 6684 RVA: 0x000AC3B5 File Offset: 0x000AA5B5
	private void RemoveAllJar()
	{
		BuildingDomainMethod.Call.CricketCollectionBatchRemoveJar(this.SelectedSourceType);
		this._needReSelectCricketInBatchMode = true;
		this.GetAllInfo();
	}

	// Token: 0x06001A1D RID: 6685 RVA: 0x000AC3D4 File Offset: 0x000AA5D4
	private void RefreshSlotsWithMode()
	{
		bool isBatchMode = this._operationMode == UI_CricketCollection.OperationMode.Batch;
		bool needReSelect = this._needReSelectCricketInBatchMode && isBatchMode;
		bool flag = needReSelect;
		if (flag)
		{
			this._curSelectedIndex = -1;
		}
		for (int i = 0; i < this._slotHolder.childCount; i++)
		{
			ItemKey jarKey = this._collectionJarList[i];
			bool hasJar = jarKey.IsValid();
			bool flag2 = this._curSelectedIndex == -1 && needReSelect && hasJar;
			if (flag2)
			{
				this._curSelectedIndex = i;
			}
			this.UpdateJarDisplay(i);
			this.UpdateCricketDisplay(i);
		}
		this._needReSelectCricketInBatchMode = false;
	}

	// Token: 0x06001A1E RID: 6686 RVA: 0x000AC468 File Offset: 0x000AA668
	private void RefreshCricketTips(TooltipInvoker tipDisplayer, bool showMouseTips = false, ItemDisplayData itemData = null, bool isTripleTail = false)
	{
		tipDisplayer.enabled = showMouseTips;
		if (showMouseTips)
		{
			tipDisplayer.Type = (isTripleTail ? TipType.SingleDesc : TipType.Cricket);
			bool flag = tipDisplayer.RuntimeParam == null;
			if (flag)
			{
				tipDisplayer.RuntimeParam = new ArgumentBox();
			}
			if (isTripleTail)
			{
				tipDisplayer.RuntimeParam.SetObject("arg0", Misc.Instance[34].Name);
			}
			else
			{
				tipDisplayer.RuntimeParam.SetObject("ItemData", itemData);
			}
			tipDisplayer.Refresh(false, -1);
		}
	}

	// Token: 0x04001485 RID: 5253
	private ItemKey[] _collectionJarList;

	// Token: 0x04001486 RID: 5254
	private ItemKey[] _collectionCricketList;

	// Token: 0x04001487 RID: 5255
	private readonly List<ItemKey> _canOperateJarList = new List<ItemKey>();

	// Token: 0x04001488 RID: 5256
	private readonly List<ItemKey> _canOperateCricketList = new List<ItemKey>();

	// Token: 0x04001489 RID: 5257
	private readonly Dictionary<ItemKey, ItemDisplayData> _itemDisplayDataDict = new Dictionary<ItemKey, ItemDisplayData>();

	// Token: 0x0400148A RID: 5258
	private readonly Dictionary<int, UI_CricketCollection.CricketAnimationState> _cricketRefreshMap = new Dictionary<int, UI_CricketCollection.CricketAnimationState>();

	// Token: 0x0400148B RID: 5259
	private int _taiwuCharId;

	// Token: 0x0400148C RID: 5260
	private int _curSelectedIndex = -1;

	// Token: 0x0400148D RID: 5261
	private bool _canTransferItemToWarehouse;

	// Token: 0x0400148E RID: 5262
	private UI_CricketCollection.OperationMode _operationMode;

	// Token: 0x0400148F RID: 5263
	private bool _inited = false;

	// Token: 0x04001490 RID: 5264
	private RectTransform _slotHolder;

	// Token: 0x04001491 RID: 5265
	private Refers _batchOperationView;

	// Token: 0x04001492 RID: 5266
	private CButtonObsolete _batchOperationSwitchButton;

	// Token: 0x04001493 RID: 5267
	private CButtonObsolete _removeAllJarButton;

	// Token: 0x04001494 RID: 5268
	private CButtonObsolete _addAllJarButton;

	// Token: 0x04001495 RID: 5269
	private CButtonObsolete _removeAllCricketButton;

	// Token: 0x04001496 RID: 5270
	private CButtonObsolete _addAllCricketButton;

	// Token: 0x04001497 RID: 5271
	private GameObject _mainWindow;

	// Token: 0x04001498 RID: 5272
	private CToggleGroupObsolete _sourceTogGroup;

	// Token: 0x04001499 RID: 5273
	private CToggleGroupObsolete _typeToggleGroup;

	// Token: 0x0400149A RID: 5274
	private CToggleObsolete _batchFilterJarToggle;

	// Token: 0x0400149B RID: 5275
	private CToggleObsolete _batchFilterCricketToggle;

	// Token: 0x0400149C RID: 5276
	private CToggleObsolete _inventoryToggle;

	// Token: 0x0400149D RID: 5277
	private CToggleObsolete _warehouseToggle;

	// Token: 0x0400149E RID: 5278
	private ItemScrollView _itemScrollView;

	// Token: 0x0400149F RID: 5279
	private CButtonObsolete _closeBatchOperationViewButton;

	// Token: 0x040014A0 RID: 5280
	private int _selectedSourceToggleIndex = -1;

	// Token: 0x040014A1 RID: 5281
	private int _selectedTypeToggleIndex = -1;

	// Token: 0x040014A2 RID: 5282
	private ItemSourceType[] _sourceTypeArray = new ItemSourceType[]
	{
		ItemSourceType.Inventory,
		ItemSourceType.Warehouse
	};

	// Token: 0x040014A3 RID: 5283
	private short[] _itemSubTypeArray = new short[]
	{
		1201,
		1100
	};

	// Token: 0x040014A4 RID: 5284
	private bool _isSwitchingMode = false;

	// Token: 0x040014A5 RID: 5285
	private List<ItemDisplayData> _batchModeDisplayDatas = new List<ItemDisplayData>();

	// Token: 0x040014A6 RID: 5286
	private CricketCollectionBatchButtonStateDisplayData _batchModeButtonStateData = new CricketCollectionBatchButtonStateDisplayData();

	// Token: 0x040014A7 RID: 5287
	private bool _needReSelectCricketInBatchMode = false;

	// Token: 0x0200134D RID: 4941
	private class CricketAnimationState
	{
		// Token: 0x04009D58 RID: 40280
		public ItemKey Cricket;

		// Token: 0x04009D59 RID: 40281
		public Coroutine AnimationCoroutine;
	}

	// Token: 0x0200134E RID: 4942
	private enum OperationMode
	{
		// Token: 0x04009D5B RID: 40283
		Normal,
		// Token: 0x04009D5C RID: 40284
		Batch
	}
}
