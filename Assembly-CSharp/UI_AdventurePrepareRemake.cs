using System;
using System.Collections.Generic;
using System.Linq;
using AdventureEditor.Beta;
using DG.Tweening;
using FrameWork;
using FrameWork.CommandSystem;
using GameData.Adventure;
using GameData.Common;
using GameData.Domains.Adventure;
using GameData.Domains.Character;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Google.Protobuf.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000166 RID: 358
public class UI_AdventurePrepareRemake : UIBase
{
	// Token: 0x17000238 RID: 568
	// (get) Token: 0x060013EA RID: 5098 RVA: 0x0007BDE6 File Offset: 0x00079FE6
	private AdventureRemakeModel AdventureRemakeModel
	{
		get
		{
			return SingletonObject.getInstance<AdventureRemakeModel>();
		}
	}

	// Token: 0x17000239 RID: 569
	// (get) Token: 0x060013EB RID: 5099 RVA: 0x0007BDED File Offset: 0x00079FED
	private int CoreId
	{
		get
		{
			return this._isMajorEvent ? this._adventureMajorEventData.Id : this._adventureData.Id;
		}
	}

	// Token: 0x060013EC RID: 5100 RVA: 0x0007BE10 File Offset: 0x0007A010
	public override void OnInit(ArgumentBox argsBox)
	{
		this._isItemsReady = false;
		this._isResourcesReady = false;
		this._isAnimReady = false;
		this.InitAnimation();
		this._nameText = base.CGet<TextMeshProUGUI>("AdventureName");
		this._descText = base.CGet<TextMeshProUGUI>("Desc");
		this._timeRefers = base.CGet<Refers>("Time");
		this._itemCostHolder = base.CGet<GameObject>("ItemCost");
		this._resourceCostHolder = base.CGet<GameObject>("ResourceCost");
		this._startAdventureBtn = base.CGet<CButtonObsolete>("ButtonConfirm");
		this._resourceCostHolder.SetActive(false);
		short curPlayerBlockId = SingletonObject.getInstance<WorldMapModel>().CurrentBlockId;
		short curPlayerAreaId = SingletonObject.getInstance<WorldMapModel>().CurrentAreaId;
		short enterBlockId;
		short enterAreaId;
		bool flag = argsBox.Get("EnterBlockId", out enterBlockId) && argsBox.Get("EnterAreaId", out enterAreaId);
		if (flag)
		{
			this._atValidPosition = (enterBlockId == curPlayerBlockId && enterAreaId == curPlayerAreaId);
		}
		else
		{
			this._atValidPosition = true;
		}
		this.NeedDataListenerId = true;
		this._isMajorEvent = argsBox.Get("MajorEventId", out this._adventureMajorEventId);
		argsBox.Get("AdventureId", out this._adventureRemakeId);
		bool flag2 = !this._isMajorEvent;
		if (flag2)
		{
			this._adventureRemake = this.AdventureRemakeModel.AdventureRemakeDict[this._adventureRemakeId];
			this._adventureData = AdventureRemakeModel.Core.GetAdventureData(this._adventureRemake.CoreId);
			this._costData = this._adventureData.Cost;
		}
		else
		{
			this._adventureMajorEvent = this.AdventureRemakeModel.AdventureMajorEventDict[this._adventureMajorEventId];
			this._adventureMajorEventData = AdventureRemakeModel.Core.GetAdventureMajorEventData(this._adventureMajorEvent.CoreId);
			this._costData = this._adventureMajorEventData.Cost;
		}
		this.UpdateAdventureBasicInfo();
		this._selectedItems.Clear();
		this._inventoryItems.Clear();
		this._selectableItems.Clear();
		this._selectableItemTypeInfos.Clear();
		RepeatedField<AdventureCostItem> costItems = this._costData.CostItems;
		bool flag3 = costItems != null && costItems.Count > 0;
		if (flag3)
		{
			for (int i = 0; i < 3; i++)
			{
				int index = i;
				Refers refers = this._itemsRefers[index];
				refers.gameObject.SetActive(this._costData.CostItems.Count > index);
				refers.GetComponent<CButtonObsolete>().ClearAndAddListener(delegate
				{
					this.OnClickChooseItem(index);
				});
			}
			List<ValueTuple<sbyte, short>> items = new List<ValueTuple<sbyte, short>>();
			for (int j = 0; j < this._costData.CostItems.Count; j++)
			{
				items.Clear();
				foreach (AdventureItemReference adventureItemReference in this._costData.CostItems[j].AvailableItems)
				{
					items.Add(new ValueTuple<sbyte, short>((sbyte)adventureItemReference.Type, (short)adventureItemReference.TemplateId));
				}
				this._selectedItems.Add(ItemKey.Invalid);
				this._selectableItemTypeInfos.Add(items.ToArray());
				this.SetItemInfo(j, ItemKey.Invalid);
			}
			this._itemCostHolder.SetActive(true);
			base.CGet<CButtonObsolete>("AutoSelectItemsBtn").gameObject.SetActive(true);
		}
		else
		{
			this._itemCostHolder.SetActive(false);
			base.CGet<CButtonObsolete>("AutoSelectItemsBtn").gameObject.SetActive(false);
		}
		this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		int remainMonths = this._isMajorEvent ? this._adventureMajorEvent.RemainMonths : this._adventureRemake.RemainMonths;
		base.CGet<TextMeshProUGUI>("RemainingMonthValue").text = remainMonths.ToString();
		base.CGet<GameObject>("RemainingMonth").SetActive(remainMonths >= 0);
		UI_AdventurePrepareRemake.UpdateTimeCost(this._timeRefers, this._costData.CostTime);
		this.RefreshStartButton();
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
		{
			this.PlayAnimation();
			bool flag4 = this._costData.CostItems != null && this._costData.CostItems.Count != 0;
			if (flag4)
			{
				bool flag5 = this.CoreId == 798222380;
				if (flag5)
				{
					WorldDomainMethod.Call.GetJuniorXiangshuLocations(this.Element.GameDataListenerId);
				}
				CharacterDomainMethod.Call.GetAllInventoryItems(this.Element.GameDataListenerId, this._taiwuCharId);
			}
			else
			{
				this._isItemsReady = true;
				this.TryShow();
			}
		}));
	}

	// Token: 0x060013ED RID: 5101 RVA: 0x0007C288 File Offset: 0x0007A488
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)this._taiwuCharId), new uint[]
		{
			34U
		}));
	}

	// Token: 0x060013EE RID: 5102 RVA: 0x0007C2B0 File Offset: 0x0007A4B0
	private void SetItemInfo(int index, ItemKey itemKey)
	{
		Refers itemRefers = this._itemsRefers[index];
		Refers selectedRefers = itemRefers.CGet<Refers>("Selected");
		GameObject valid = itemRefers.CGet<GameObject>("Valid");
		GameObject invalid = itemRefers.CGet<GameObject>("Invalid");
		TooltipInvoker tipDisplayer = itemRefers.GetComponent<TooltipInvoker>();
		bool flag = tipDisplayer.RuntimeParam == null;
		if (flag)
		{
			tipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>();
		}
		tipDisplayer.RuntimeParam.Clear();
		tipDisplayer.PresetParam = null;
		ItemView itemView = selectedRefers as ItemView;
		this._selectedItems[index] = itemKey;
		bool flag2 = !itemKey.IsValid();
		if (flag2)
		{
			selectedRefers.gameObject.SetActive(false);
			bool flag3 = this._selectableItems.Count > index;
			if (flag3)
			{
				valid.SetActive(this._selectableItems[index].Count > 0);
				invalid.SetActive(this._selectableItems[index].Count == 0);
			}
			tipDisplayer.Type = TipType.Simple;
			sbyte itemType = this._selectableItemTypeInfos[index][0].Item1;
			short templateId = this._selectableItemTypeInfos[index][0].Item2;
			string info = (templateId >= 0) ? ItemUtils.GetItemColorName(itemType, templateId) : LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", UI_ItemTemplateSelector.WrapOrUnwrapItemSubType(templateId)));
			for (int i = 1; i < this._selectableItemTypeInfos[index].Length; i++)
			{
				ValueTuple<sbyte, short> valueTuple = this._selectableItemTypeInfos[index][i];
				itemType = valueTuple.Item1;
				templateId = valueTuple.Item2;
				info = info + "\n" + ((templateId >= 0) ? ItemUtils.GetItemColorName(itemType, templateId) : LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", UI_ItemTemplateSelector.WrapOrUnwrapItemSubType(templateId))));
			}
			tipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Adventure_Prepare_RequiredItems));
			tipDisplayer.RuntimeParam.Set("arg1", info);
		}
		else
		{
			selectedRefers.gameObject.SetActive(true);
			valid.SetActive(true);
			invalid.SetActive(false);
			ItemDisplayData displayData = this._selectableItems[index].First((ItemDisplayData item) => item.ContainsItemKey(itemKey));
			itemView.SetData(displayData, false, 1, false, true, null, false, true);
			TooltipInvoker displayer = itemView.GetComponent<TooltipInvoker>();
			tipDisplayer.Type = displayer.Type;
			tipDisplayer.RuntimeParam = displayer.RuntimeParam;
		}
		this.RefreshStartButton();
	}

	// Token: 0x060013EF RID: 5103 RVA: 0x0007C558 File Offset: 0x0007A758
	private void InitAnimation()
	{
		this._butterflyTrans = base.CGet<RectTransform>("Butterfly");
		this._butterflyShadowTrans = base.CGet<RectTransform>("ButterflyShadow");
		this._woodPadLeftTrans = base.CGet<RectTransform>("WoodPadLeft");
		this._woodPadRightTrans = base.CGet<RectTransform>("WoodPadRight");
		this._mainWindow = base.CGet<RectTransform>("MainWindow");
		this._butterflyTrans.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -32f));
		this._butterflyTrans.localScale = new Vector3(0.64f, 0.64f, 1f);
		this._butterflyTrans.anchoredPosition = new Vector2(-380f, -20f);
		this._butterflyShadowTrans.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -32f));
		this._butterflyShadowTrans.localScale = new Vector3(0.64f, 0.64f, 1f);
		this._butterflyShadowTrans.anchoredPosition = new Vector2(-378f, -19f);
		this._woodPadLeftTrans.anchoredPosition = new Vector2(-694f, 40.5f);
		this._woodPadRightTrans.anchoredPosition = new Vector2(695f, 20f);
		this._mainWindow.GetComponent<CanvasGroup>().alpha = 0f;
	}

	// Token: 0x060013F0 RID: 5104 RVA: 0x0007C6C8 File Offset: 0x0007A8C8
	private void PlayAnimation()
	{
		this._butterflyTrans.DOScale(Vector3.one, 0.5f);
		this._butterflyTrans.DORotate(Vector3.one, 0.5f, RotateMode.Fast);
		this._butterflyTrans.DOAnchorPos(new Vector2(-485f, -110f), 0.5f, false);
		this._butterflyShadowTrans.DOScale(Vector3.one, 0.5f);
		this._butterflyShadowTrans.DORotate(Vector3.one, 0.5f, RotateMode.Fast);
		this._butterflyShadowTrans.DOAnchorPos(new Vector2(-478f, -129f), 0.5f, false);
		this._woodPadLeftTrans.DOAnchorPos(new Vector2(-498.5f, 40.5f), 0.5f, false);
		this._woodPadRightTrans.DOAnchorPos(new Vector2(595f, 20f), 0.5f, false);
		this._mainWindow.GetComponent<CanvasGroup>().DOFade(1f, 0.5f);
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(base.CGet<CScrollRectLegacy>("VerticalScrollView").Content);
			this._isAnimReady = true;
			this.TryShow();
		});
	}

	// Token: 0x060013F1 RID: 5105 RVA: 0x0007C7E8 File Offset: 0x0007A9E8
	private void TryShow()
	{
		bool flag = this._isAnimReady && this._isItemsReady && this._isResourcesReady;
		if (flag)
		{
			this.Element.ShowAfterRefresh();
		}
	}

	// Token: 0x060013F2 RID: 5106 RVA: 0x0007C820 File Offset: 0x0007AA20
	private void ShowTranceLandWarn()
	{
		DialogCmd cmd = new DialogCmd
		{
			Title = this.GetAdventureName(),
			Content = LocalStringManager.Get(LanguageKey.LK_DangerNotice_SpiritLand),
			Yes = new Action(this.ConfirmEnterAdventure)
		};
		UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
		UIManager.Instance.MaskUI(UIElement.Dialog);
	}

	// Token: 0x060013F3 RID: 5107 RVA: 0x0007C890 File Offset: 0x0007AA90
	private void ConfirmEnterAdventure()
	{
		this._waitingConfirm = false;
		bool flag = this._adventureRemakeId < 0;
		if (!flag)
		{
			CommandManager.AddCommandMethodCall<int, List<ItemKey>>(EPriority.CallMethodNormal, 10, this._isMajorEvent ? 12 : 3, this._isMajorEvent ? this._adventureMajorEventId : this._adventureRemakeId, this._selectedItems, new CallMethodRespHandler(this.HandlerMethodEnterAdventure), null);
		}
	}

	// Token: 0x060013F4 RID: 5108 RVA: 0x0007C8F4 File Offset: 0x0007AAF4
	private void HandlerMethodEnterAdventure(int offset, RawDataPool pool)
	{
		bool result = false;
		Serializer.Deserialize(pool, offset, ref result);
		bool exist = this.Element.Exist;
		if (exist)
		{
			UIManager.Instance.HideUI(this.Element);
		}
		bool flag = !result;
		if (!flag)
		{
			bool isMajorEvent = this._isMajorEvent;
			if (isMajorEvent)
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Set("MajorEventId", this._adventureMajorEventId);
				GEvent.OnEvent(UiEvents.AdventureRemakeOpenPartOne, argBox);
			}
			else
			{
				ArgumentBox argBox2 = EasyPool.Get<ArgumentBox>();
				argBox2.Set("AdventureId", this._adventureRemakeId);
				GEvent.OnEvent(UiEvents.AdventureRemakeOpenPartOne, argBox2);
			}
		}
	}

	// Token: 0x060013F5 RID: 5109 RVA: 0x0007C9A0 File Offset: 0x0007ABA0
	private unsafe void RefreshStartButton()
	{
		bool flag = !this._atValidPosition;
		if (flag)
		{
			this._startAdventureBtn.interactable = false;
		}
		else
		{
			for (int i = 0; i < this._costData.CostResources.Count; i++)
			{
				int resourceType = this._costData.CostResources[i].Type;
				int count = this._costData.CostResources[i].Value;
				bool flag2 = *(ref this._resources.Items.FixedElementField + (IntPtr)resourceType * 4) >= count;
				if (!flag2)
				{
					this._startAdventureBtn.interactable = false;
					return;
				}
			}
			for (int j = 0; j < this._selectedItems.Count; j++)
			{
				bool flag3 = this._selectedItems[j].IsValid();
				if (!flag3)
				{
					this._startAdventureBtn.interactable = false;
					return;
				}
			}
			bool flag4 = !SingletonObject.getInstance<TimeManager>().IsActionPointEnough(this._costData.CostTime);
			if (flag4)
			{
				this._startAdventureBtn.interactable = false;
			}
			else
			{
				this._startAdventureBtn.interactable = true;
			}
		}
	}

	// Token: 0x060013F6 RID: 5110 RVA: 0x0007CAE0 File Offset: 0x0007ACE0
	private void UpdateAdventureBasicInfo()
	{
		this._nameText.text = this.GetAdventureName();
		this._descText.text = this.GetAdventureDesc();
	}

	// Token: 0x060013F7 RID: 5111 RVA: 0x0007CB08 File Offset: 0x0007AD08
	private string GetAdventureName()
	{
		string name = this._isMajorEvent ? this._adventureMajorEventData.Name : this._adventureData.Name;
		return name.ColorReplace();
	}

	// Token: 0x060013F8 RID: 5112 RVA: 0x0007CB44 File Offset: 0x0007AD44
	private string GetAdventureDesc()
	{
		string desc = this._isMajorEvent ? this._adventureMajorEventData.Desc : this._adventureData.Desc;
		return desc.ColorReplace();
	}

	// Token: 0x060013F9 RID: 5113 RVA: 0x0007CB80 File Offset: 0x0007AD80
	private void UpdateItems()
	{
		bool flag = this._costData.CostItems == null || this._costData.CostItems.Count == 0;
		if (!flag)
		{
			for (int i = 0; i < this._costData.CostItems.Count; i++)
			{
				Refers refers = this._itemsRefers[i];
				bool hasValidItem = this._selectableItems[i].Count > 0;
				this._selectedItems[i] = ItemKey.Invalid;
				refers.CGet<GameObject>("Invalid").SetActive(!hasValidItem);
				refers.CGet<GameObject>("Valid").SetActive(hasValidItem);
				refers.CGet<Refers>("Selected").gameObject.SetActive(false);
				refers.GetComponent<CButtonObsolete>().interactable = hasValidItem;
			}
		}
	}

	// Token: 0x060013FA RID: 5114 RVA: 0x0007CC60 File Offset: 0x0007AE60
	private unsafe void UpdateResources()
	{
		bool hasAnyResourceCost = false;
		for (sbyte resourceType = 0; resourceType < 8; resourceType += 1)
		{
			Refers refers = this._resourcesRefers[(int)resourceType];
			refers.gameObject.SetActive(false);
		}
		for (int i = 0; i < this._costData.CostResources.Count; i++)
		{
			int resourceType2 = this._costData.CostResources[i].Type;
			int count = this._costData.CostResources[i].Value;
			bool flag = count <= 0;
			if (!flag)
			{
				Refers refers2 = this._resourcesRefers[resourceType2];
				hasAnyResourceCost = true;
				refers2.gameObject.SetActive(true);
				UI_AdventurePrepareRemake.UpdateResourceCost(refers2, this._costData.CostResources[resourceType2].Value, *(ref this._resources.Items.FixedElementField + (IntPtr)resourceType2 * 4), string.Empty);
			}
		}
		this._resourceCostHolder.SetActive(hasAnyResourceCost);
		LayoutRebuilder.MarkLayoutForRebuild(this._resourceCostHolder.GetComponent<RectTransform>());
		LayoutRebuilder.ForceRebuildLayoutImmediate(base.CGet<RectTransform>("Content"));
	}

	// Token: 0x060013FB RID: 5115 RVA: 0x0007CD90 File Offset: 0x0007AF90
	private static void UpdateResourceCost(Refers refers, int neededValue, int hadValue, string unit)
	{
		TextMeshProUGUI valueText = refers.CGet<TextMeshProUGUI>("Value");
		TextMeshProUGUI notEnoughValueText = refers.CGet<TextMeshProUGUI>("NotEnoughValue");
		notEnoughValueText.gameObject.SetActive(false);
		string color = (neededValue <= hadValue) ? "brightblue" : "brightred";
		string valueDisplayStr = CommonUtils.GetDisplayStringForNum(hadValue, 100000);
		valueText.SetText(string.Format("{0}/{1}{2}", (valueDisplayStr + unit).SetColor(color), neededValue, unit), true);
	}

	// Token: 0x060013FC RID: 5116 RVA: 0x0007CE08 File Offset: 0x0007B008
	private static void UpdateTimeCost(Refers refers, int actionPoint)
	{
		TimeManager timeManager = SingletonObject.getInstance<TimeManager>();
		float hadValue = timeManager.GetRemainingFloatActionPointConvertToDays();
		float neededValue = timeManager.ActionPointConvertToDays(actionPoint);
		TextMeshProUGUI valueText = refers.CGet<TextMeshProUGUI>("Value");
		TextMeshProUGUI notEnoughValueText = refers.CGet<TextMeshProUGUI>("NotEnoughValue");
		notEnoughValueText.gameObject.SetActive(false);
		string color = (neededValue <= hadValue) ? "brightblue" : "brightred";
		valueText.SetText(string.Format("{0}/{1:f1}", hadValue.ToString("f1").SetColor(color), neededValue), true);
	}

	// Token: 0x060013FD RID: 5117 RVA: 0x0007CE90 File Offset: 0x0007B090
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
					this.FetchMethodReturnValue(notification.DomainId, notification.MethodId, notification.ValueOffset, wrapper.DataPool);
				}
			}
			else
			{
				this.HandData(notification.Uid, notification.ValueOffset, wrapper.DataPool);
			}
		}
	}

	// Token: 0x060013FE RID: 5118 RVA: 0x0007CF38 File Offset: 0x0007B138
	private void FetchMethodReturnValue(ushort domainId, ushort methodId, int returnValueOffset, RawDataPool dataPool)
	{
		bool flag = domainId == 10 && methodId == 0;
		if (flag)
		{
			bool anyEvent = false;
			Serializer.Deserialize(dataPool, returnValueOffset, ref anyEvent);
			bool flag2 = anyEvent;
			if (flag2)
			{
				UIElement eventWindow = UIElement.EventWindow;
				eventWindow.OnHide = (Action)Delegate.Combine(eventWindow.OnHide, new Action(this.ConfirmEnterAdventure));
			}
			else
			{
				this.ConfirmEnterAdventure();
			}
		}
		else
		{
			bool flag3 = domainId == 1 && methodId == 3;
			if (flag3)
			{
				Serializer.Deserialize(dataPool, returnValueOffset, ref this._juniorXiangshuLocations);
			}
			else
			{
				bool flag4 = domainId == 4 && methodId == 27;
				if (flag4)
				{
					Serializer.Deserialize(dataPool, returnValueOffset, ref this._inventoryItems);
					for (int j = 0; j < this._selectableItems.Count; j++)
					{
						this._selectableItems[j].Clear();
					}
					this._selectableItems.Clear();
					int i2;
					int i;
					for (i = 0; i < this._costData.CostItems.Count; i = i2 + 1)
					{
						List<ItemDisplayData> selectableItems = (from item in this._inventoryItems
						where this._selectableItemTypeInfos[i].Exist((ValueTuple<sbyte, short> itemInfo) => itemInfo.Item1 == item.Key.ItemType && (itemInfo.Item2 == item.Key.TemplateId || -itemInfo.Item2 - 1 == ItemTemplateHelper.GetItemSubType(item.Key.ItemType, item.Key.TemplateId)))
						select item).ToList<ItemDisplayData>();
						this._selectableItems.Add(selectableItems);
						bool flag5 = this.CoreId == 798222380 && this._juniorXiangshuLocations.Count > 0;
						if (flag5)
						{
							Dictionary<short, sbyte> fragment2BossId = GameData.Domains.Combat.SharedConstValue.SwordFragment2BossId;
							for (int index = selectableItems.Count - 1; index >= 0; index--)
							{
								ItemKey itemKey = selectableItems[index].Key;
								bool flag6 = itemKey.ItemType != 12;
								if (!flag6)
								{
									bool flag7 = !fragment2BossId.ContainsKey(itemKey.TemplateId);
									if (!flag7)
									{
										sbyte bossId = fragment2BossId[itemKey.TemplateId];
										bool flag8 = this._juniorXiangshuLocations[(int)bossId].IsValid();
										if (flag8)
										{
											selectableItems.RemoveAt(index);
										}
									}
								}
							}
						}
						i2 = i;
					}
					this.UpdateItems();
					bool flag9 = !this._isItemsReady;
					if (flag9)
					{
						this._isItemsReady = true;
						this.TryShow();
					}
				}
			}
		}
	}

	// Token: 0x060013FF RID: 5119 RVA: 0x0007D19C File Offset: 0x0007B39C
	private void HandData(DataUid uid, int valueOffset, RawDataPool dataPool)
	{
		bool flag = uid.DomainId == 4 && uid.DataId == 0 && uid.SubId1 == 34U;
		if (flag)
		{
			Serializer.Deserialize(dataPool, valueOffset, ref this._resources);
			this.UpdateResources();
			this.RefreshStartButton();
			bool flag2 = !this._isResourcesReady;
			if (flag2)
			{
				this._isResourcesReady = true;
				this.TryShow();
			}
		}
	}

	// Token: 0x06001400 RID: 5120 RVA: 0x0007D208 File Offset: 0x0007B408
	protected override void OnClick(Transform btn)
	{
		base.OnClick(btn);
		string btnName = btn.name;
		string text = btnName;
		string a = text;
		if (!(a == "ButtonConfirm"))
		{
			if (!(a == "ButtonCancel"))
			{
				if (a == "AutoSelectItemsBtn")
				{
					this.OnClickAutoSelectAll();
				}
			}
			else
			{
				this.QuickHide();
			}
		}
		else
		{
			this.ClickConfirm();
		}
	}

	// Token: 0x06001401 RID: 5121 RVA: 0x0007D270 File Offset: 0x0007B470
	private void ClickConfirm()
	{
		bool waitingConfirm = this._waitingConfirm;
		if (!waitingConfirm)
		{
			bool flag = this._isMajorEvent && this._adventureMajorEvent.CoreId == 1092089202;
			if (flag)
			{
				this.ShowTranceLandWarn();
			}
			bool flag2 = !this._isMajorEvent && XiangshuAvatarIds.IsSwordTombAdventure(this._adventureRemake.CoreId);
			if (flag2)
			{
				this._waitingConfirm = true;
				AdventureDomainMethod.Call.TryInvokeConfirmEnterEvent(this.Element.GameDataListenerId);
			}
			else
			{
				this.ConfirmEnterAdventure();
			}
		}
	}

	// Token: 0x06001402 RID: 5122 RVA: 0x0007D2F4 File Offset: 0x0007B4F4
	private void Update()
	{
		bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) && this._startAdventureBtn.interactable;
		if (flag)
		{
			this.ClickConfirm();
		}
	}

	// Token: 0x06001403 RID: 5123 RVA: 0x0007D334 File Offset: 0x0007B534
	public override void QuickHide()
	{
		AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
		base.QuickHide();
	}

	// Token: 0x06001404 RID: 5124 RVA: 0x0007D350 File Offset: 0x0007B550
	public void OnClickChooseItem(int index)
	{
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.SetObject("DisplayData", this._selectableItems[index]);
		argBox.SetObject("callback", new Action<ItemKey>(delegate(ItemKey itemKey)
		{
			this.SetItemInfo(index, itemKey);
		}));
		ItemSortAndFilter.ItemFilterType filterType = ItemSortAndFilter.GetFilterType(this._selectableItemTypeInfos[index][0].Item1);
		bool flag = !this._selectableItemTypeInfos[index].Exist((ValueTuple<sbyte, short> item) => filterType != ItemSortAndFilter.GetFilterType(item.Item1));
		if (flag)
		{
			argBox.SetObject("filterType", filterType);
		}
		UIElement.SelectItemLegacy.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.SelectItemLegacy, true);
	}

	// Token: 0x06001405 RID: 5125 RVA: 0x0007D434 File Offset: 0x0007B634
	public void OnClickAutoSelectAll()
	{
		for (int index = 0; index < this._costData.CostItems.Count; index++)
		{
			bool flag = this._selectableItems[index].Count > 0;
			if (flag)
			{
				this.SetItemInfo(index, this._selectableItems[index][0].Key);
			}
		}
	}

	// Token: 0x040010B5 RID: 4277
	private RectTransform _butterflyTrans;

	// Token: 0x040010B6 RID: 4278
	private RectTransform _butterflyShadowTrans;

	// Token: 0x040010B7 RID: 4279
	private RectTransform _woodPadLeftTrans;

	// Token: 0x040010B8 RID: 4280
	private RectTransform _woodPadRightTrans;

	// Token: 0x040010B9 RID: 4281
	private RectTransform _mainWindow;

	// Token: 0x040010BA RID: 4282
	private int _adventureRemakeId;

	// Token: 0x040010BB RID: 4283
	private int _adventureMajorEventId;

	// Token: 0x040010BC RID: 4284
	private bool _isMajorEvent;

	// Token: 0x040010BD RID: 4285
	private AdventureRuntime _adventureRemake;

	// Token: 0x040010BE RID: 4286
	private AdventureMajorEvent _adventureMajorEvent;

	// Token: 0x040010BF RID: 4287
	private AdventureData _adventureData;

	// Token: 0x040010C0 RID: 4288
	private AdventureMajorEventData _adventureMajorEventData;

	// Token: 0x040010C1 RID: 4289
	private AdventureCostData _costData;

	// Token: 0x040010C2 RID: 4290
	private TextMeshProUGUI _nameText;

	// Token: 0x040010C3 RID: 4291
	private TextMeshProUGUI _descText;

	// Token: 0x040010C4 RID: 4292
	private Refers _timeRefers;

	// Token: 0x040010C5 RID: 4293
	[SerializeField]
	private Refers[] _resourcesRefers;

	// Token: 0x040010C6 RID: 4294
	[SerializeField]
	private Refers[] _itemsRefers;

	// Token: 0x040010C7 RID: 4295
	private GameObject _itemCostHolder;

	// Token: 0x040010C8 RID: 4296
	private GameObject _resourceCostHolder;

	// Token: 0x040010C9 RID: 4297
	private CButtonObsolete _startAdventureBtn;

	// Token: 0x040010CA RID: 4298
	private const int MaxItemCount = 3;

	// Token: 0x040010CB RID: 4299
	private List<ItemDisplayData> _inventoryItems = new List<ItemDisplayData>();

	// Token: 0x040010CC RID: 4300
	private readonly List<ValueTuple<sbyte, short>[]> _selectableItemTypeInfos = new List<ValueTuple<sbyte, short>[]>();

	// Token: 0x040010CD RID: 4301
	private readonly List<List<ItemDisplayData>> _selectableItems = new List<List<ItemDisplayData>>();

	// Token: 0x040010CE RID: 4302
	private readonly List<ItemKey> _selectedItems = new List<ItemKey>();

	// Token: 0x040010CF RID: 4303
	private List<Location> _juniorXiangshuLocations = new List<Location>();

	// Token: 0x040010D0 RID: 4304
	private int _remainTime;

	// Token: 0x040010D1 RID: 4305
	private bool _atValidPosition;

	// Token: 0x040010D2 RID: 4306
	private int _taiwuCharId;

	// Token: 0x040010D3 RID: 4307
	private ResourceInts _resources;

	// Token: 0x040010D4 RID: 4308
	private bool _waitingConfirm;

	// Token: 0x040010D5 RID: 4309
	private bool _isAnimReady;

	// Token: 0x040010D6 RID: 4310
	private bool _isItemsReady;

	// Token: 0x040010D7 RID: 4311
	private bool _isResourcesReady;
}
