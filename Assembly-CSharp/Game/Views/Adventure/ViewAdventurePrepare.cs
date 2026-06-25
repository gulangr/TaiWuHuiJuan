using System;
using System.Collections.Generic;
using System.Linq;
using AdventureEditor.Beta;
using FrameWork;
using FrameWork.CommandSystem;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UI;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Views.EventWindow;
using Game.Views.Select;
using GameData.Adventure;
using GameData.Common;
using GameData.Domains.Adventure;
using GameData.Domains.Character;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Google.Protobuf.Collections;
using TMPro;
using UnityEngine;

namespace Game.Views.Adventure
{
	// Token: 0x02000C7B RID: 3195
	public class ViewAdventurePrepare : UIBase
	{
		// Token: 0x170010FD RID: 4349
		// (get) Token: 0x0600A229 RID: 41513 RVA: 0x004BC41A File Offset: 0x004BA61A
		private AdventureRemakeModel AdventureRemakeModel
		{
			get
			{
				return SingletonObject.getInstance<AdventureRemakeModel>();
			}
		}

		// Token: 0x170010FE RID: 4350
		// (get) Token: 0x0600A22A RID: 41514 RVA: 0x004BC421 File Offset: 0x004BA621
		private int TaiwuCharId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x170010FF RID: 4351
		// (get) Token: 0x0600A22B RID: 41515 RVA: 0x004BC42D File Offset: 0x004BA62D
		private int CoreId
		{
			get
			{
				return this._isMajorEvent ? this._adventureMajorEventData.Id : this._adventureData.Id;
			}
		}

		// Token: 0x17001100 RID: 4352
		// (get) Token: 0x0600A22C RID: 41516 RVA: 0x004BC44F File Offset: 0x004BA64F
		private EventTextureManager EventTextureManager
		{
			get
			{
				return SingletonObject.getInstance<EventTextureManager>();
			}
		}

		// Token: 0x0600A22D RID: 41517 RVA: 0x004BC458 File Offset: 0x004BA658
		public override void OnInit(ArgumentBox argsBox)
		{
			this._isItemsReady = false;
			this._isResourcesReady = false;
			this._isMajorEvent = argsBox.Get("MajorEventId", out this._adventureMajorEventId);
			argsBox.Get("AdventureId", out this._adventureRemakeId);
			bool flag = !this._isMajorEvent;
			if (flag)
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
			short curPlayerBlockId = SingletonObject.getInstance<WorldMapModel>().CurrentBlockId;
			short curPlayerAreaId = SingletonObject.getInstance<WorldMapModel>().CurrentAreaId;
			short enterBlockId;
			short enterAreaId;
			bool flag2 = argsBox.Get("EnterBlockId", out enterBlockId) && argsBox.Get("EnterAreaId", out enterAreaId);
			if (flag2)
			{
				this._atValidPosition = (enterBlockId == curPlayerBlockId && enterAreaId == curPlayerAreaId);
			}
			else
			{
				this._atValidPosition = true;
			}
			this.NeedDataListenerId = true;
			this.UpdateAdventureBasicInfo();
			this.UpdateItemInfo();
			this.UpdateTimeCost();
			this.RefreshStartButton();
			this.UpdateEventTexture();
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				bool flag3 = this._costData.CostItems != null && this._costData.CostItems.Count != 0;
				if (flag3)
				{
					bool flag4 = this.CoreId == 798222380;
					if (flag4)
					{
						WorldDomainMethod.Call.GetJuniorXiangshuLocations(this.Element.GameDataListenerId);
					}
					CharacterDomainMethod.Call.GetAllInventoryItems(this.Element.GameDataListenerId, this.TaiwuCharId);
				}
				else
				{
					this._isItemsReady = true;
					this.TryShow();
				}
			}));
		}

		// Token: 0x0600A22E RID: 41518 RVA: 0x004BC5E8 File Offset: 0x004BA7E8
		private void Update()
		{
			bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) && this.confirmButton.interactable;
			if (flag)
			{
				this.ClickConfirm();
			}
		}

		// Token: 0x0600A22F RID: 41519 RVA: 0x004BC628 File Offset: 0x004BA828
		private void UpdateEventTexture()
		{
			string textureName = this._isMajorEvent ? this._adventureMajorEventData.EventTexture : this._adventureData.EventTexture;
			ViewEventWindow.UpdateTexture(textureName, this.eventTexture, this.EventTextureManager, ViewEventWindow.TextureDirectory, "");
		}

		// Token: 0x0600A230 RID: 41520 RVA: 0x004BC674 File Offset: 0x004BA874
		private void UpdateAdventureBasicInfo()
		{
			string adventureTypeName = LocalStringManager.Get(this._isMajorEvent ? LanguageKey.LK_MajorEvent : LanguageKey.LK_Adventure);
			this.infoTitle.SetText(LocalStringManager.GetFormat(LanguageKey.LK_Adventure_Enter_Info, adventureTypeName), true);
			string nameText = this._isMajorEvent ? this._adventureMajorEventData.Name : this._adventureData.Name;
			this.adventureName.SetText(nameText.ColorReplace(), true);
			this.nameTitle.SetText(LocalStringManager.GetFormat(LanguageKey.LK_Adventure_Enter_Name, adventureTypeName), true);
			this.nameContent.SetText(nameText.ColorReplace(), true);
			string desc = this._isMajorEvent ? this._adventureMajorEventData.Desc : this._adventureData.Desc;
			this.descTitle.SetText(LocalStringManager.GetFormat(LanguageKey.LK_Adventure_Enter_Desc, adventureTypeName), true);
			this.descContent.SetText(desc.ColorReplace(), true);
			bool flag = !this._isMajorEvent;
			if (flag)
			{
				this.targetTitle.SetText(LocalStringManager.Get(LanguageKey.LK_Adventure_Enter_Target), true);
				this.difficultyTitle.SetText(LocalStringManager.Get(LanguageKey.LK_Adventure_Enter_Difficulty), true);
				this.rewardTitle.SetText(LocalStringManager.Get(LanguageKey.LK_Adventure_Enter_Reward), true);
				this.targetContent.SetText(this._adventureData.DescTarget, true);
				this.difficultyContent.SetText(LocalStringManager.Get(string.Format("LK_Adventure_Difficulty_Grade_{0}", this._adventureData.Grade)), true);
				this.rewardContent.SetText(this._adventureData.DescReward, true);
			}
			this.targetTitle.transform.parent.gameObject.SetActive(!this._isMajorEvent);
			this.difficultyTitle.transform.parent.gameObject.SetActive(!this._isMajorEvent);
			this.rewardTitle.transform.parent.gameObject.SetActive(!this._isMajorEvent);
			int remainMonths = this._isMajorEvent ? this._adventureMajorEvent.RemainMonths : this._adventureRemake.RemainMonths;
			this.leftTimeTitle.transform.parent.gameObject.SetActive(remainMonths >= 0);
			bool flag2 = remainMonths >= 0;
			if (flag2)
			{
				this.leftTimeTitle.SetText(LocalStringManager.Get(LanguageKey.LK_Adventure_Enter_LeftTime), true);
				this.leftTimeContent.SetText(remainMonths.ToString(), true);
			}
			this.costTitle.SetText(LocalStringManager.GetFormat(LanguageKey.LK_Adventure_Enter_Cost, adventureTypeName), true);
			this.needItemTitle.SetText(LocalStringManager.GetFormat(LanguageKey.LK_Adventure_Prepare_RequiredItems, Array.Empty<object>()), true);
			this.needResourceTitle.SetText(LocalStringManager.GetFormat(LanguageKey.LK_Adventure_Enter_NeedResource, Array.Empty<object>()), true);
			this.timeCostTitle.SetText(LocalStringManager.GetFormat(LanguageKey.LK_Adventure_Prepare_RequiredTime, Array.Empty<object>()), true);
		}

		// Token: 0x0600A231 RID: 41521 RVA: 0x004BC96C File Offset: 0x004BAB6C
		protected override void OnClick(Transform btn)
		{
			base.OnClick(btn);
			string btnName = btn.name;
			string text = btnName;
			string a = text;
			if (!(a == "Confirm"))
			{
				if (!(a == "Close"))
				{
					if (a == "AutoSelectItem")
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

		// Token: 0x0600A232 RID: 41522 RVA: 0x004BC9D4 File Offset: 0x004BABD4
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
				else
				{
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
		}

		// Token: 0x0600A233 RID: 41523 RVA: 0x004BCA5C File Offset: 0x004BAC5C
		private void ShowTranceLandWarn()
		{
			string nameText = this._isMajorEvent ? this._adventureMajorEventData.Name : this._adventureData.Name;
			DialogCmd cmd = new DialogCmd
			{
				Title = nameText,
				Content = LocalStringManager.Get(LanguageKey.LK_DangerNotice_SpiritLand),
				Yes = new Action(this.ConfirmEnterAdventure)
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x0600A234 RID: 41524 RVA: 0x004BCAE8 File Offset: 0x004BACE8
		private void ConfirmEnterAdventure()
		{
			this._waitingConfirm = false;
			bool flag = this._adventureRemakeId < 0;
			if (!flag)
			{
				this.QuickHide();
				CommandManager.AddCommandMethodCall<int, List<ItemKey>>(EPriority.CallMethodNormal, 10, this._isMajorEvent ? 12 : 3, this._isMajorEvent ? this._adventureMajorEventId : this._adventureRemakeId, this._selectedItems, new CallMethodRespHandler(this.HandlerMethodEnterAdventure), null);
			}
		}

		// Token: 0x0600A235 RID: 41525 RVA: 0x004BCB54 File Offset: 0x004BAD54
		private void HandlerMethodEnterAdventure(int offset, RawDataPool pool)
		{
			bool result = false;
			Serializer.Deserialize(pool, offset, ref result);
			bool flag = !result;
			if (!flag)
			{
				bool activeSelf = base.gameObject.activeSelf;
				if (activeSelf)
				{
					UIMaskManager instance = SingletonObject.getInstance<UIMaskManager>();
					if (instance != null)
					{
						instance.DetachMask(base.transform);
					}
					base.gameObject.SetActive(false);
				}
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

		// Token: 0x0600A236 RID: 41526 RVA: 0x004BCC18 File Offset: 0x004BAE18
		private void UpdateTimeCost()
		{
			TimeManager timeManager = SingletonObject.getInstance<TimeManager>();
			float hadValue = timeManager.GetRemainingFloatActionPointConvertToDays();
			float neededValue = timeManager.ActionPointConvertToDays(this._costData.CostTime);
			TextMeshProUGUI valueText = this.timeCostContent;
			string color = (neededValue <= hadValue) ? "brightblue" : "brightred";
			valueText.SetText(string.Format("{0}/{1:f1}", hadValue.ToString("f1").SetColor(color), neededValue), true);
			this.timeCost.SetActive(neededValue > 0f);
			this.UpdateCostInfoGroup();
		}

		// Token: 0x0600A237 RID: 41527 RVA: 0x004BCCA4 File Offset: 0x004BAEA4
		private void TryShow()
		{
			bool flag = this._isItemsReady && this._isResourcesReady;
			if (flag)
			{
				this.Element.ShowAfterRefresh();
			}
		}

		// Token: 0x0600A238 RID: 41528 RVA: 0x004BCCD4 File Offset: 0x004BAED4
		private void OnClickAutoSelectAll()
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

		// Token: 0x0600A239 RID: 41529 RVA: 0x004BCD3C File Offset: 0x004BAF3C
		private void OnClickChooseItem(int index)
		{
			SelectItemConfig config = SelectItemConfig.CreateSingleSelectConfig(new SelectItemRules
			{
				OnlyFromInventory = true
			}, delegate(List<SelectedItemData> itemSelected)
			{
				bool flag = itemSelected != null && itemSelected.Count > 0;
				if (flag)
				{
					SelectedItemData selectedItem = itemSelected[0];
					bool flag2 = !selectedItem.IsCancelled && selectedItem.ItemData != null;
					if (flag2)
					{
						ItemKey curItemKey = selectedItem.ItemData.Key;
						this.SetItemInfo(index, curItemKey);
					}
				}
			}, "", null);
			config.MaxSelectCount = 1;
			config.ExternalItems = this._selectableItems[index];
			config.InitialSelectedItems = new List<SelectedItemData>();
			config.HideSourceToggles = true;
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("DisplayBg", true);
			argBox.SetObject("SelectItemConfig", config);
			UIElement.SelectItem.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.SelectItem, true);
		}

		// Token: 0x0600A23A RID: 41530 RVA: 0x004BCDFC File Offset: 0x004BAFFC
		private unsafe void RefreshStartButton()
		{
			bool flag = !this._atValidPosition;
			if (flag)
			{
				this.confirmButton.interactable = false;
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
						this.confirmButton.interactable = false;
						return;
					}
				}
				for (int j = 0; j < this._selectedItems.Count; j++)
				{
					bool flag3 = this._selectedItems[j].IsValid();
					if (!flag3)
					{
						this.confirmButton.interactable = false;
						return;
					}
				}
				bool flag4 = !SingletonObject.getInstance<TimeManager>().IsActionPointEnough(this._costData.CostTime);
				if (flag4)
				{
					this.confirmButton.interactable = false;
				}
				else
				{
					this.confirmButton.interactable = true;
				}
			}
		}

		// Token: 0x0600A23B RID: 41531 RVA: 0x004BCF3C File Offset: 0x004BB13C
		private void UpdateCostInfoGroup()
		{
			bool isShow = this.needItemTitle.gameObject.activeSelf || this.needItemsHolder.activeSelf || this.resourceHolder.transform.parent.gameObject.activeSelf;
			this.costInfoGroup.gameObject.SetActive(isShow);
		}

		// Token: 0x0600A23C RID: 41532 RVA: 0x004BCF9C File Offset: 0x004BB19C
		private void UpdateItemInfo()
		{
			this._selectedItems.Clear();
			this._inventoryItems.Clear();
			this._selectableItems.Clear();
			this._selectableItemTypeInfos.Clear();
			RepeatedField<AdventureCostItem> costItems = this._costData.CostItems;
			bool flag = costItems != null && costItems.Count > 0;
			if (flag)
			{
				for (int i = 0; i < 3; i++)
				{
					int index = i;
					ItemSlot item = this.needItems[index];
					item.gameObject.SetActive(this._costData.CostItems.Count > index);
					item.button.ClearAndAddListener(delegate
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
				this.needItemsHolder.SetActive(true);
				this.needItemTitle.gameObject.SetActive(true);
				this.autoSelectItemButton.gameObject.SetActive(true);
			}
			else
			{
				this.needItemsHolder.SetActive(false);
				this.needItemTitle.gameObject.SetActive(false);
				this.autoSelectItemButton.gameObject.SetActive(false);
			}
			this.UpdateCostInfoGroup();
		}

		// Token: 0x0600A23D RID: 41533 RVA: 0x004BD1BC File Offset: 0x004BB3BC
		private void SetItemInfo(int index, ItemKey itemKey)
		{
			ItemSlot itemSlot = this.needItems[index];
			TooltipInvoker tipDisplayer = itemSlot.mouseTip;
			bool flag = tipDisplayer.RuntimeParam == null;
			if (flag)
			{
				tipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			tipDisplayer.RuntimeParam.Clear();
			tipDisplayer.PresetParam = null;
			this._selectedItems[index] = itemKey;
			bool flag2 = !itemKey.IsValid();
			if (flag2)
			{
				itemSlot.SetData(null);
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
				ItemDisplayData displayData = this._selectableItems[index].First((ItemDisplayData item) => item.ContainsItemKey(itemKey));
				itemSlot.SetData(displayData);
				RowItemLine.SetMouseTipDisplayer(true, displayData, tipDisplayer);
			}
			this.RefreshStartButton();
		}

		// Token: 0x0600A23E RID: 41534 RVA: 0x004BD398 File Offset: 0x004BB598
		private void UpdateItems()
		{
			bool flag = this._costData.CostItems == null || this._costData.CostItems.Count == 0;
			if (!flag)
			{
				for (int i = 0; i < this._costData.CostItems.Count; i++)
				{
					ItemSlot needItem = this.needItems[i];
					bool hasValidItem = this._selectableItems[i].Count > 0;
					this._selectedItems[i] = ItemKey.Invalid;
					needItem.button.interactable = hasValidItem;
				}
			}
		}

		// Token: 0x0600A23F RID: 41535 RVA: 0x004BD430 File Offset: 0x004BB630
		private unsafe void UpdateResources()
		{
			bool hasAnyResourceCost = false;
			this.resourceHolder.Rebuild<ResourceSlot>(this._costData.CostResources.Count, delegate(ResourceSlot slot, int index)
			{
				int resourceType = this._costData.CostResources[index].Type;
				int count = this._costData.CostResources[index].Value;
				bool flag = count <= 0;
				if (!flag)
				{
					hasAnyResourceCost = true;
					this.UpdateResourceCost(slot, this._costData.CostResources[resourceType].Value, *(ref this._resources.Items.FixedElementField + (IntPtr)resourceType * 4), resourceType);
				}
			});
			this.resourceHolder.transform.parent.gameObject.SetActive(hasAnyResourceCost);
			this.UpdateCostInfoGroup();
		}

		// Token: 0x0600A240 RID: 41536 RVA: 0x004BD4A4 File Offset: 0x004BB6A4
		private void UpdateResourceCost(ResourceSlot slot, int neededValue, int hadValue, int resourceType)
		{
			TextMeshProUGUI valueText = slot.value;
			slot.icon.SetSprite(CommonUtils.GetResOrExpIcon((sbyte)resourceType, false), false, null);
			string color = (neededValue <= hadValue) ? "brightblue" : "brightred";
			string valueDisplayStr = CommonUtils.GetDisplayStringForNum(hadValue, 100000);
			valueText.SetText(string.Format("{0}/{1}", valueDisplayStr.SetColor(color), neededValue), true);
		}

		// Token: 0x0600A241 RID: 41537 RVA: 0x004BD50C File Offset: 0x004BB70C
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)this.TaiwuCharId), new uint[]
			{
				34U
			}));
		}

		// Token: 0x0600A242 RID: 41538 RVA: 0x004BD534 File Offset: 0x004BB734
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

		// Token: 0x0600A243 RID: 41539 RVA: 0x004BD5DC File Offset: 0x004BB7DC
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
										sbyte bossId;
										bool flag7 = !fragment2BossId.TryGetValue(itemKey.TemplateId, out bossId);
										if (!flag7)
										{
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

		// Token: 0x0600A244 RID: 41540 RVA: 0x004BD830 File Offset: 0x004BBA30
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

		// Token: 0x04007E19 RID: 32281
		[SerializeField]
		private TextMeshProUGUI adventureName;

		// Token: 0x04007E1A RID: 32282
		[SerializeField]
		private TextMeshProUGUI infoTitle;

		// Token: 0x04007E1B RID: 32283
		[SerializeField]
		private TextMeshProUGUI nameTitle;

		// Token: 0x04007E1C RID: 32284
		[SerializeField]
		private TextMeshProUGUI nameContent;

		// Token: 0x04007E1D RID: 32285
		[SerializeField]
		private TextMeshProUGUI descTitle;

		// Token: 0x04007E1E RID: 32286
		[SerializeField]
		private TextMeshProUGUI descContent;

		// Token: 0x04007E1F RID: 32287
		[SerializeField]
		private TextMeshProUGUI targetTitle;

		// Token: 0x04007E20 RID: 32288
		[SerializeField]
		private TextMeshProUGUI targetContent;

		// Token: 0x04007E21 RID: 32289
		[SerializeField]
		private TextMeshProUGUI rewardTitle;

		// Token: 0x04007E22 RID: 32290
		[SerializeField]
		private TextMeshProUGUI rewardContent;

		// Token: 0x04007E23 RID: 32291
		[SerializeField]
		private TextMeshProUGUI difficultyTitle;

		// Token: 0x04007E24 RID: 32292
		[SerializeField]
		private TextMeshProUGUI difficultyContent;

		// Token: 0x04007E25 RID: 32293
		[SerializeField]
		private TextMeshProUGUI leftTimeTitle;

		// Token: 0x04007E26 RID: 32294
		[SerializeField]
		private TextMeshProUGUI leftTimeContent;

		// Token: 0x04007E27 RID: 32295
		[SerializeField]
		private TextMeshProUGUI costTitle;

		// Token: 0x04007E28 RID: 32296
		[SerializeField]
		private TextMeshProUGUI needItemTitle;

		// Token: 0x04007E29 RID: 32297
		[SerializeField]
		private TextMeshProUGUI needResourceTitle;

		// Token: 0x04007E2A RID: 32298
		[SerializeField]
		private TextMeshProUGUI timeCostTitle;

		// Token: 0x04007E2B RID: 32299
		[SerializeField]
		private TextMeshProUGUI timeCostContent;

		// Token: 0x04007E2C RID: 32300
		[SerializeField]
		private CRawImage eventTexture;

		// Token: 0x04007E2D RID: 32301
		[SerializeField]
		private CButton confirmButton;

		// Token: 0x04007E2E RID: 32302
		[SerializeField]
		private CButton autoSelectItemButton;

		// Token: 0x04007E2F RID: 32303
		[SerializeField]
		private ItemSlot[] needItems = new ItemSlot[3];

		// Token: 0x04007E30 RID: 32304
		[SerializeField]
		private GameObject needItemsHolder;

		// Token: 0x04007E31 RID: 32305
		[SerializeField]
		private TemplatedContainerAssemblyNew resourceHolder;

		// Token: 0x04007E32 RID: 32306
		[SerializeField]
		private GameObject costInfoGroup;

		// Token: 0x04007E33 RID: 32307
		[SerializeField]
		private GameObject timeCost;

		// Token: 0x04007E34 RID: 32308
		private bool _isItemsReady;

		// Token: 0x04007E35 RID: 32309
		private bool _isResourcesReady;

		// Token: 0x04007E36 RID: 32310
		private bool _waitingConfirm;

		// Token: 0x04007E37 RID: 32311
		private bool _atValidPosition;

		// Token: 0x04007E38 RID: 32312
		private int _adventureRemakeId;

		// Token: 0x04007E39 RID: 32313
		private int _adventureMajorEventId;

		// Token: 0x04007E3A RID: 32314
		private bool _isMajorEvent;

		// Token: 0x04007E3B RID: 32315
		private AdventureRuntime _adventureRemake;

		// Token: 0x04007E3C RID: 32316
		private AdventureMajorEvent _adventureMajorEvent;

		// Token: 0x04007E3D RID: 32317
		private AdventureData _adventureData;

		// Token: 0x04007E3E RID: 32318
		private AdventureMajorEventData _adventureMajorEventData;

		// Token: 0x04007E3F RID: 32319
		private AdventureCostData _costData;

		// Token: 0x04007E40 RID: 32320
		private List<Location> _juniorXiangshuLocations = new List<Location>();

		// Token: 0x04007E41 RID: 32321
		private const int MaxItemCount = 3;

		// Token: 0x04007E42 RID: 32322
		private List<ItemDisplayData> _inventoryItems = new List<ItemDisplayData>();

		// Token: 0x04007E43 RID: 32323
		private readonly List<ValueTuple<sbyte, short>[]> _selectableItemTypeInfos = new List<ValueTuple<sbyte, short>[]>();

		// Token: 0x04007E44 RID: 32324
		private readonly List<List<ItemDisplayData>> _selectableItems = new List<List<ItemDisplayData>>();

		// Token: 0x04007E45 RID: 32325
		private readonly List<ItemKey> _selectedItems = new List<ItemKey>();

		// Token: 0x04007E46 RID: 32326
		private ResourceInts _resources;
	}
}
