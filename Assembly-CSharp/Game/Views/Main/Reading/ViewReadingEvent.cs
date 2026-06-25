using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Common;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Main.Reading
{
	// Token: 0x02000974 RID: 2420
	public class ViewReadingEvent : UIBase
	{
		// Token: 0x17000D26 RID: 3366
		// (get) Token: 0x0600740C RID: 29708 RVA: 0x0035FDAE File Offset: 0x0035DFAE
		private SkillBookPageDisplayData CurPageDisplayData
		{
			get
			{
				return this._pageDisplayData[this._curReadingBook.Id];
			}
		}

		// Token: 0x0600740D RID: 29709 RVA: 0x0035FDC8 File Offset: 0x0035DFC8
		public override void OnInit(ArgumentBox argsBox)
		{
			GlobalDomainMethod.Call.InvokeGuidingTrigger(72);
			this._usedStrategies.Clear();
			this._curReadingBook = ItemKey.Invalid;
			this.NeedDataListenerId = true;
			this._modified = false;
			this._inited = false;
			this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			this.strategyHolder.Init(-1);
			this.readingPages.fivePages.gameObject.SetActive(false);
			this.readingPages.sixPages.gameObject.SetActive(false);
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				BuildingDomainMethod.Call.GetSutraReadingRoomBuffValue(this.Element.GameDataListenerId);
			}));
			UIElement element2 = this.Element;
			element2.OnShowed = (Action)Delegate.Combine(element2.OnShowed, new Action(delegate()
			{
				bool inGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
				if (inGuiding)
				{
					TaiwuEventDomainMethod.Call.TriggerListener(EventActionKey.DefValue.TutorialEnterReadingEvent, false);
				}
				this.UpdateCurReadingBookPages(false);
			}));
		}

		// Token: 0x0600740E RID: 29710 RVA: 0x0035FEA4 File Offset: 0x0035E0A4
		private void Awake()
		{
			for (int i = 0; i < 12; i++)
			{
				int index = i;
				CToggle tog = this.strategyToggleList[index];
				ReadingEventStrategyToggle refers = tog.GetComponent<ReadingEventStrategyToggle>();
				PointerTrigger trigger = tog.GetComponent<PointerTrigger>();
				trigger.EnterEvent.RemoveAllListeners();
				trigger.EnterEvent.AddListener(delegate()
				{
					bool flag = tog.interactable && !tog.isOn;
					if (flag)
					{
						refers.highlight.SetActive(true);
					}
					refers.tips.SetActive(!tog.interactable);
					this.PreviewStrategy(index);
				});
				trigger.ExitEvent.RemoveAllListeners();
				trigger.ExitEvent.AddListener(delegate()
				{
					refers.highlight.SetActive(false);
					refers.tips.SetActive(false);
					this.ClearStrategyPreview();
				});
			}
		}

		// Token: 0x0600740F RID: 29711 RVA: 0x0035FF5C File Offset: 0x0035E15C
		private void OnDisable()
		{
			this.ClearStrategyPreview();
			bool modified = this._modified;
			if (modified)
			{
				ExtraDomainMethod.Call.RemoveReadingEventBookId(this._curReadingBook.Id, this._curReadingBook.TemplateId);
			}
			GEvent.OnEvent(UiEvents.CloseReadingEvent, null);
		}

		// Token: 0x06007410 RID: 29712 RVA: 0x0035FFA8 File Offset: 0x0035E1A8
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(5, 43, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(5, 44, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)this._taiwuCharId, new uint[]
			{
				43U,
				79U
			}));
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 119, ulong.MaxValue, null));
		}

		// Token: 0x06007411 RID: 29713 RVA: 0x00360028 File Offset: 0x0035E228
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
						this.HandleMethodReturn(notification.DomainId, notification.MethodId, notification.ValueOffset, wrapper.DataPool);
					}
				}
				else
				{
					this.HandleDataModification(notification.Uid, notification.ValueOffset, wrapper.DataPool);
				}
			}
		}

		// Token: 0x06007412 RID: 29714 RVA: 0x003600D0 File Offset: 0x0035E2D0
		private unsafe void HandleDataModification(DataUid uid, int offset, RawDataPool dataPool)
		{
			ushort domainId = uid.DomainId;
			ushort num = domainId;
			if (num != 4)
			{
				if (num != 5)
				{
					if (num == 19)
					{
						ushort dataId = uid.DataId;
						ushort num2 = dataId;
						if (num2 == 119)
						{
							TaiwuDomainMethod.Call.GetCurrentBookAvailableReadingStrategies(this.Element.GameDataListenerId);
						}
					}
				}
				else
				{
					ushort dataId2 = uid.DataId;
					ushort num3 = dataId2;
					if (num3 != 43)
					{
						if (num3 == 44)
						{
							Serializer.Deserialize(dataPool, offset, ref this._referenceBooks);
							CharacterDomainMethod.Call.GetInventoryItems(this.Element.GameDataListenerId, this._taiwuCharId, 1001);
							CharacterDomainMethod.Call.GetInventoryItems(this.Element.GameDataListenerId, this._taiwuCharId, 1000);
						}
					}
					else
					{
						Serializer.Deserialize(dataPool, offset, ref this._curReadingBook);
						bool flag = this._curReadingBook.IsValid();
						if (!flag)
						{
							throw new Exception(string.Format("{0} can't be invalid in this case.", this._curReadingBook));
						}
						ItemDomainMethod.Call.GetItemDisplayData(this.Element.GameDataListenerId, this._curReadingBook);
						base.RemoveMonitorFieldId(5, 43);
					}
				}
			}
			else
			{
				bool flag2 = uid.DataId == 0;
				if (flag2)
				{
					uint subId = uid.SubId1;
					uint num4 = subId;
					if (num4 != 43U)
					{
						if (num4 == 79U)
						{
							MainAttributes mainAttributes;
							Serializer.Deserialize(dataPool, offset, ref mainAttributes);
							this._maxIntelligence = (int)(*(ref mainAttributes.Items.FixedElementField + (IntPtr)5 * 2));
							this.intelligenceCost.SetText(LocalStringManager.GetFormat(LanguageKey.LK_Reading_CostIntelligence, this._curIntelligence, this._maxIntelligence), true);
						}
					}
					else
					{
						MainAttributes mainAttributes2;
						Serializer.Deserialize(dataPool, offset, ref mainAttributes2);
						this._curIntelligence = (int)(*(ref mainAttributes2.Items.FixedElementField + (IntPtr)5 * 2));
						this.intelligenceCost.SetText(LocalStringManager.GetFormat(LanguageKey.LK_Reading_CostIntelligence, this._curIntelligence, this._maxIntelligence), true);
						this.UpdateStrategiesSelectable();
					}
				}
			}
		}

		// Token: 0x06007413 RID: 29715 RVA: 0x003602D8 File Offset: 0x0035E4D8
		private void HandleMethodReturn(ushort domainId, ushort methodId, int offset, RawDataPool dataPool)
		{
			switch (domainId)
			{
			case 4:
				if (methodId <= 48)
				{
					if (methodId != 26)
					{
						if (methodId == 48)
						{
							List<CharacterDisplayData> charDataList = null;
							Serializer.Deserialize(dataPool, offset, ref charDataList);
							this.Element.ShowAfterRefresh();
						}
					}
					else
					{
						this._getInventoryItemsCount++;
						List<ItemDisplayData> displayDataList = null;
						Serializer.Deserialize(dataPool, offset, ref displayDataList);
						bool flag = displayDataList != null && displayDataList.Count > 0;
						if (flag)
						{
							foreach (ItemDisplayData data in displayDataList)
							{
								ItemDomainMethod.Call.GetSkillBookPagesInfo(this.Element.GameDataListenerId, data.Key);
							}
						}
						bool flag2 = this._getInventoryItemsCount >= 2;
						if (flag2)
						{
							this._getInventoryItemsCount = 0;
							bool flag3 = this._curBookDisplayData.Durability > 0;
							if (flag3)
							{
								TaiwuDomainMethod.Call.GetCurReadingStrategies(this.Element.GameDataListenerId);
							}
						}
					}
				}
				else if (methodId != 88)
				{
					if (methodId != 90)
					{
					}
				}
				break;
			case 5:
				if (methodId != 30)
				{
					if (methodId == 92)
					{
						Serializer.Deserialize(dataPool, offset, ref this._selectableStrategies);
						this.RenderSelectableStrategies();
					}
				}
				else
				{
					Serializer.Deserialize(dataPool, offset, ref this._curStrategies);
					ExtraDomainMethod.Call.GetBookStrategiesExpireTime(this.Element.GameDataListenerId, this._curReadingBook);
					ReadingDisplayHelper.UpdateReadingBookInfo(this.bookIntro, this.readingInspireHolder, this._curReadingBook, this._curBookDisplayData, this._referenceBooks, false, null, false);
				}
				break;
			case 6:
			{
				bool flag4 = methodId == 7;
				if (flag4)
				{
					Serializer.Deserialize(dataPool, offset, ref this._curBookDisplayData);
					ReadingDisplayHelper.UpdateReadingBookInfo(this.bookIntro, this.readingInspireHolder, this._curReadingBook, this._curBookDisplayData, this._referenceBooks, false, null, false);
				}
				else
				{
					bool flag5 = methodId == 9;
					if (flag5)
					{
						SkillBookPageDisplayData displayData = null;
						Serializer.Deserialize(dataPool, offset, ref displayData);
						this._pageDisplayData[displayData.ItemKey.Id] = displayData;
					}
				}
				break;
			}
			case 7:
			case 8:
				break;
			case 9:
			{
				bool flag6 = methodId == 92;
				if (flag6)
				{
					Serializer.Deserialize(dataPool, offset, ref this._sutraReadingRoomBuffValue);
				}
				break;
			}
			default:
				if (domainId == 19)
				{
					bool flag7 = methodId == 121;
					if (flag7)
					{
						Serializer.Deserialize(dataPool, offset, ref this._curReadingBookExpireTime);
						this.UpdateCurReadingBookPages(false);
						bool flag8 = !this._inited;
						if (flag8)
						{
							this._inited = true;
							CharacterDomainMethod.Call.GetCharacterDisplayDataList(this.Element.GameDataListenerId, new List<int>
							{
								SingletonObject.getInstance<BasicGameData>().TaiwuCharId
							});
						}
						else
						{
							this.UpdateStrategiesSelectable();
						}
						this.AutoSelectPage();
						this.RefreshCloseButton();
					}
				}
				break;
			}
		}

		// Token: 0x06007414 RID: 29716 RVA: 0x003605E4 File Offset: 0x0035E7E4
		private void RenderSelectableStrategies()
		{
			this.ClearStrategyPreview();
			short intCostChange = this._curStrategies.GetPageIntCostChange(this._curPage);
			for (int i = 0; i < 12; i++)
			{
				int targetSlotStrategy = (int)((this._curStrategyIndex != -1) ? this._curStrategies.GetPageStrategy(this._curPage, this._curStrategyIndex) : -1);
				CToggle curTog = this.strategyToggleList[i];
				CToggle tempCurTog = curTog;
				int index = i;
				bool flag = this._selectableStrategies.Count <= index;
				if (flag)
				{
					curTog.gameObject.SetActive(false);
				}
				else
				{
					curTog.gameObject.SetActive(true);
					ReadingEventStrategyToggle refers = curTog.GetComponent<ReadingEventStrategyToggle>();
					byte strategyId = this._selectableStrategies[index];
					bool isUsed = this._usedStrategies.Contains(index);
					bool flag2 = isUsed;
					if (flag2)
					{
						curTog.gameObject.SetActive(false);
					}
					else
					{
						ReadingStrategyItem configData = ReadingStrategy.Instance[(int)strategyId];
						int intCost = Math.Max(0, (int)(intCostChange + configData.IntelligenceCost)) * this._sutraReadingRoomBuffValue / 100;
						refers.txtName.text = configData.Name;
						refers.cost.text = intCost.ToString();
						refers.selected.SetActive(isUsed);
						string notInteractableReason = this.GetNotInteractableReason(configData, isUsed, targetSlotStrategy, intCost);
						TooltipInvoker mouseTipDisplayer = refers.GetComponent<TooltipInvoker>();
						TooltipInvoker tooltipInvoker = mouseTipDisplayer;
						if (tooltipInvoker.RuntimeParam == null)
						{
							tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
						}
						mouseTipDisplayer.RuntimeParam.Set("arg0", configData.Name).Set("arg1", configData.Desc + notInteractableReason);
						refers.intelligence.SetActive(this._curIntelligence < intCost);
						curTog.interactable = (targetSlotStrategy == -1 && !isUsed && this._curStrategyIndex >= 0 && this._curIntelligence >= intCost && this._curBookDisplayData.Durability > 0 && this._curBookDisplayData.Durability >= configData.DurabilityCost);
						curTog.isOn = false;
						curTog.onValueChanged.RemoveAllListeners();
						Action <>9__2;
						curTog.onValueChanged.AddListener(delegate(bool isOn)
						{
							bool flag3 = !isOn;
							if (!flag3)
							{
								bool flag4 = configData.DurabilityCost >= this._curBookDisplayData.Durability;
								if (flag4)
								{
									DialogCmd dialogCmd = new DialogCmd();
									dialogCmd.Type = 1;
									dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_ReadingEvent_Cost_Durability_Title);
									dialogCmd.Content = LocalStringManager.Get(LanguageKey.LK_ReadingEvent_Cost_Durability_Content).ColorReplace();
									dialogCmd.Yes = new Action(base.<RenderSelectableStrategies>g__OnSelect|1);
									Action no;
									if ((no = <>9__2) == null)
									{
										no = (<>9__2 = delegate()
										{
											tempCurTog.isOn = false;
										});
									}
									dialogCmd.No = no;
									DialogCmd cmd = dialogCmd;
									UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
									UIManager.Instance.MaskUI(UIElement.Dialog);
								}
								else
								{
									base.<RenderSelectableStrategies>g__OnSelect|1();
								}
							}
						});
						refers.toggleDisableStyleRoot.SetStyleEffect(!curTog.interactable || curTog.isOn, false);
					}
				}
			}
			this.extraStrategy.ArrangeChildren();
		}

		// Token: 0x06007415 RID: 29717 RVA: 0x003608BC File Offset: 0x0035EABC
		private void UpdateStrategiesSelectable()
		{
			bool flag = this._selectableStrategies == null || this._selectableStrategies.Count == 0;
			if (!flag)
			{
				short intCostChange = this._curStrategies.GetPageIntCostChange(this._curPage);
				for (int i = 0; i < 12; i++)
				{
					sbyte targetSlotStrategy = this._curStrategies.GetPageStrategy(this._curPage, i);
					CToggle curTog = this.strategyToggleList[i];
					int index = i;
					bool flag2 = this._selectableStrategies.Count <= index;
					if (!flag2)
					{
						byte strategyId = this._selectableStrategies[index];
						bool isUsed = this._usedStrategies.Contains(index);
						bool flag3 = isUsed;
						if (flag3)
						{
							curTog.gameObject.SetActive(false);
						}
						else
						{
							curTog.gameObject.SetActive(true);
							ReadingStrategyItem configData = ReadingStrategy.Instance[(int)strategyId];
							int intCost = Math.Max(0, (int)(intCostChange + configData.IntelligenceCost)) * this._sutraReadingRoomBuffValue / 100;
							ReadingEventStrategyToggle refers = curTog.GetComponent<ReadingEventStrategyToggle>();
							refers.cost.text = intCost.ToString();
							curTog.interactable = (targetSlotStrategy == -1 && !isUsed && this._curStrategyIndex >= 0 && this._curIntelligence >= intCost && this._curBookDisplayData.Durability > 0 && this._curBookDisplayData.Durability >= configData.DurabilityCost);
							bool flag4 = !curTog.interactable && !isUsed;
							if (flag4)
							{
								string notInteractableReason = this.GetNotInteractableReason(configData, isUsed, (int)targetSlotStrategy, intCost);
								TooltipInvoker mouseTipDisplayer = refers.GetComponent<TooltipInvoker>();
								bool flag5 = mouseTipDisplayer.RuntimeParam != null;
								if (flag5)
								{
									mouseTipDisplayer.RuntimeParam.Set("arg1", configData.Desc + notInteractableReason);
								}
							}
							refers.toggleDisableStyleRoot.SetStyleEffect(!curTog.interactable || curTog.isOn, false);
						}
					}
				}
			}
		}

		// Token: 0x06007416 RID: 29718 RVA: 0x00360AB8 File Offset: 0x0035ECB8
		private string GetNotInteractableReason(ReadingStrategyItem configData, bool isUsed, int targetSlotStrategy, int actualIntCost)
		{
			string result;
			if (isUsed)
			{
				result = string.Empty;
			}
			else
			{
				bool flag = this._curBookDisplayData.Durability <= 0;
				if (flag)
				{
					result = "\n" + LocalStringManager.Get(LanguageKey.LK_Tool_Durability_Not_Enough).SetColor("brightred");
				}
				else
				{
					bool flag2 = configData.DurabilityCost > 0 && this._curBookDisplayData.Durability < configData.DurabilityCost;
					if (flag2)
					{
						result = "\n" + LocalStringManager.Get(LanguageKey.LK_Tool_Durability_Not_Enough).SetColor("brightred");
					}
					else
					{
						bool flag3 = this._curIntelligence < actualIntCost;
						if (flag3)
						{
							result = "\n" + LocalStringManager.Get(LanguageKey.LK_ActiveRead_Tip_NotEnough1).ColorReplace();
						}
						else
						{
							bool flag4 = targetSlotStrategy >= 0;
							if (flag4)
							{
								result = string.Empty;
							}
							else
							{
								bool flag5 = this._curStrategyIndex < 0;
								if (flag5)
								{
									result = string.Empty;
								}
								else
								{
									result = string.Empty;
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06007417 RID: 29719 RVA: 0x00360BB4 File Offset: 0x0035EDB4
		private void UpdateCurReadingBookPages(bool clearPage = true)
		{
			this.ClearStrategyPreview();
			List<SkillBookPageDisplayData> refDataList = new List<SkillBookPageDisplayData>(this._referenceBooks.Length);
			foreach (ItemKey refBook in this._referenceBooks)
			{
				SkillBookPageDisplayData pageData;
				bool flag = refBook.IsValid() && this._pageDisplayData.TryGetValue(refBook.Id, out pageData);
				if (flag)
				{
					refDataList.Add(pageData);
				}
			}
			SkillBookPageDisplayData pageDisplayData = null;
			bool flag2 = this._curReadingBook.IsValid();
			if (flag2)
			{
				pageDisplayData = this._pageDisplayData[this._curReadingBook.Id];
			}
			this._curPagesDisplay = ReadingDisplayHelper.UpdatePages(this.readingPages, this._curReadingBook, pageDisplayData, this._curStrategies, this._curReadingBookExpireTime, null, this._curBookDisplayData.IsReadingFinished, !this._inited, clearPage, refDataList, new Action<int>(this.OnClickPage), this.IsReadingEventOfTianShuXuanJiByAdoptiveFather());
		}

		// Token: 0x06007418 RID: 29720 RVA: 0x00360C9C File Offset: 0x0035EE9C
		private void SwitchPage(int pageIndex)
		{
			this._curPage = (byte)pageIndex;
			this._curStrategyIndex = -1;
			int startIndex = this.IsReadingEventOfTianShuXuanJiByAdoptiveFather() ? 2 : 0;
			for (int strategyIndex = startIndex; strategyIndex < 3; strategyIndex++)
			{
				sbyte strategy = this._curStrategies.GetPageStrategy(this._curPage, strategyIndex);
				bool flag = strategy >= 0;
				if (!flag)
				{
					this._curStrategyIndex = strategyIndex;
					break;
				}
			}
			this._curPagesDisplay.FocusStrategySlot(this._curPage, this._curStrategyIndex);
			this.RenderSelectableStrategies();
		}

		// Token: 0x06007419 RID: 29721 RVA: 0x00360D21 File Offset: 0x0035EF21
		private void OnClickPage(int pageIndex)
		{
			this.SwitchPage(pageIndex);
		}

		// Token: 0x0600741A RID: 29722 RVA: 0x00360D2C File Offset: 0x0035EF2C
		private void AutoSelectPage()
		{
			short bookSubType = ItemTemplateHelper.GetItemSubType(this._curReadingBook.ItemType, this._curReadingBook.TemplateId);
			int pageCount = (bookSubType == 1001) ? 6 : 5;
			byte startPageIndex = this._curPage;
			for (int p = (int)startPageIndex; p < pageCount * 2; p++)
			{
				int searchingPageIndex = p % pageCount;
				int startIndex = this.IsReadingEventOfTianShuXuanJiByAdoptiveFather() ? 2 : 0;
				for (int i = startIndex; i < 3; i++)
				{
					bool flag = this._curStrategies.GetPageStrategy((byte)searchingPageIndex, i) == -1;
					if (flag)
					{
						this.SwitchPage(searchingPageIndex);
						return;
					}
				}
			}
		}

		// Token: 0x0600741B RID: 29723 RVA: 0x00360DD4 File Offset: 0x0035EFD4
		private void SelectStrategy(int strategyTemplateId, int availableStrategyIndex)
		{
			this.ClearStrategyPreview();
			bool flag = strategyTemplateId >= 0;
			if (flag)
			{
				this._modified = true;
				TaiwuDomainMethod.Call.SetReadingStrategy(this._curPage, this._curStrategyIndex, (sbyte)strategyTemplateId);
				this._usedStrategies.Add(availableStrategyIndex);
				short durabilityCost = ReadingStrategy.Instance[strategyTemplateId].DurabilityCost;
				bool flag2 = this._curBookDisplayData.Durability - durabilityCost > 0;
				if (flag2)
				{
					ItemDomainMethod.Call.GetSkillBookPagesInfo(this.Element.GameDataListenerId, this._curBookDisplayData.Key);
					TaiwuDomainMethod.Call.GetCurReadingStrategies(this.Element.GameDataListenerId);
				}
				else
				{
					GEvent.OnEvent(UiEvents.RefreshBookList, EasyPool.Get<ArgumentBox>().Set("CurReadingBookId", this._curBookDisplayData.Key.Id));
					this.QuickHide();
				}
				bool flag3 = durabilityCost != 0;
				if (flag3)
				{
					ItemDisplayData curBookDisplayData = this._curBookDisplayData;
					curBookDisplayData.Durability -= durabilityCost;
				}
				ReadingDisplayHelper.UpdateReadingBookInfo(this.bookIntro, this.readingInspireHolder, this._curReadingBook, this._curBookDisplayData, this._referenceBooks, false, null, false);
			}
			int thisTimeSelectedIndex = this._curStrategyIndex;
			bool clearPageStrategies = ReadingStrategy.Instance[strategyTemplateId].ClearPageStrategies;
			if (clearPageStrategies)
			{
				for (int strategyIndex = this.IsReadingEventOfTianShuXuanJiByAdoptiveFather() ? 2 : 0; strategyIndex < 3; strategyIndex++)
				{
					bool flag4 = strategyIndex <= this._curStrategyIndex;
					if (!flag4)
					{
						this._curStrategyIndex = strategyIndex;
						break;
					}
				}
			}
			else
			{
				int strategyIndex2 = this.IsReadingEventOfTianShuXuanJiByAdoptiveFather() ? 2 : 0;
				this._curStrategyIndex = -1;
				while (strategyIndex2 < 3)
				{
					sbyte strategy = this._curStrategies.GetPageStrategy(this._curPage, strategyIndex2);
					bool flag5 = strategy >= 0;
					if (!flag5)
					{
						bool flag6 = strategyIndex2 <= thisTimeSelectedIndex;
						if (!flag6)
						{
							this._curStrategyIndex = strategyIndex2;
							break;
						}
					}
					strategyIndex2++;
				}
			}
			this._curPagesDisplay.FocusStrategySlot(this._curPage, this._curStrategyIndex);
			this.UpdateStrategiesSelectable();
		}

		// Token: 0x0600741C RID: 29724 RVA: 0x00360FDC File Offset: 0x0035F1DC
		private void PreviewStrategy(int availableStrategyIndex)
		{
			bool flag = this._curPagesDisplay == null;
			if (!flag)
			{
				bool flag2 = this._curStrategyIndex < 0;
				if (!flag2)
				{
					bool flag3 = availableStrategyIndex < 0 || availableStrategyIndex >= this._selectableStrategies.Count;
					if (!flag3)
					{
						bool flag4 = this._previewStrategyIndex == availableStrategyIndex;
						if (!flag4)
						{
							this._previewStrategyIndex = availableStrategyIndex;
							sbyte strategyId = (sbyte)this._selectableStrategies[availableStrategyIndex];
							bool flag5 = strategyId < 0;
							if (!flag5)
							{
								SkillBookPageDisplayData displayData;
								bool flag6 = !this._pageDisplayData.TryGetValue(this._curReadingBook.Id, out displayData);
								if (!flag6)
								{
									ReadingStrategyItem configData = ReadingStrategy.Instance[strategyId];
									bool slotPreviewApplied = this.ApplyStrategySlotPreview(configData, strategyId, displayData);
									bool efficiencyPreviewApplied = this.ApplyEfficiencyPreview(configData, strategyId);
									bool flag7 = !slotPreviewApplied && !efficiencyPreviewApplied;
									if (flag7)
									{
										this.ClearStrategyPreview();
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600741D RID: 29725 RVA: 0x003610C5 File Offset: 0x0035F2C5
		private void ClearStrategyPreview()
		{
			this._previewStrategyIndex = -1;
			ReadingSkillBookPagesDisplay curPagesDisplay = this._curPagesDisplay;
			if (curPagesDisplay != null)
			{
				curPagesDisplay.ClearStrategyPreview();
			}
			ReadingBookIntro readingBookIntro = this.bookIntro;
			if (readingBookIntro != null)
			{
				readingBookIntro.ClearEfficiencyPreview();
			}
		}

		// Token: 0x0600741E RID: 29726 RVA: 0x003610F4 File Offset: 0x0035F2F4
		private bool ApplyStrategySlotPreview(ReadingStrategyItem configData, sbyte strategyId, SkillBookPageDisplayData displayData)
		{
			bool flag = this._curStrategyIndex < 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = displayData == null;
				if (flag2)
				{
					result = false;
				}
				else
				{
					int pageCount = displayData.ReadingProgress.Length;
					this.EnsurePreviewBuffer(pageCount);
					for (int i = 0; i < pageCount; i++)
					{
						this._previewProgressBuffer[i] = (int)displayData.ReadingProgress[i];
					}
					Array.Clear(this._previewSlotMarkers, 0, this._previewSlotMarkers.Length);
					this._previewProgressList.Clear();
					bool previewSkipPage = configData.SkipPage;
					bool flag3;
					this.ApplyProgressPreview(configData, displayData, out flag3);
					bool flag4 = strategyId == 4;
					if (flag4)
					{
						for (int j = 0; j < 3; j++)
						{
							sbyte currStrategyId = this._curStrategies.GetPageStrategy(this._curPage, j);
							bool flag5 = currStrategyId < 0;
							if (!flag5)
							{
								ReadingStrategyItem currConfig = ReadingStrategy.Instance[currStrategyId];
								bool stopRepeatTrigger;
								bool hasTriggeredImmediateEffect = this.ApplyProgressPreview(currConfig, displayData, out stopRepeatTrigger);
								this._previewSlotMarkers[j] = (hasTriggeredImmediateEffect ? ReadingSkillBookPagesDisplay.PreviewMarkerType.Repeat : ReadingSkillBookPagesDisplay.PreviewMarkerType.None);
								bool flag6 = stopRepeatTrigger;
								if (flag6)
								{
									break;
								}
							}
						}
					}
					bool clearPageStrategies = configData.ClearPageStrategies;
					if (clearPageStrategies)
					{
						for (int k = 0; k < 3; k++)
						{
							sbyte currStrategyId2 = this._curStrategies.GetPageStrategy(this._curPage, k);
							this._previewSlotMarkers[k] = ((currStrategyId2 >= 0) ? ReadingSkillBookPagesDisplay.PreviewMarkerType.Clear : ReadingSkillBookPagesDisplay.PreviewMarkerType.None);
						}
					}
					int effectMultiplier = 1;
					bool flag7 = strategyId == 5;
					if (flag7)
					{
						int doubleCount = 0;
						for (int l = 0; l < 3; l++)
						{
							sbyte currStrategyId3 = this._curStrategies.GetPageStrategy(this._curPage, l);
							bool flag8 = currStrategyId3 == 5;
							if (flag8)
							{
								doubleCount++;
							}
						}
						effectMultiplier = 1 << doubleCount + 1;
						for (int m = 0; m < 3; m++)
						{
							sbyte currStrategyId4 = this._curStrategies.GetPageStrategy(this._curPage, m);
							this._previewSlotMarkers[m] = ((currStrategyId4 >= 0) ? ReadingSkillBookPagesDisplay.PreviewMarkerType.EffectMultiplier : ReadingSkillBookPagesDisplay.PreviewMarkerType.None);
						}
					}
					byte n = 0;
					while ((int)n < pageCount)
					{
						bool flag9 = this._previewProgressBuffer[(int)n] == (int)displayData.ReadingProgress[(int)n];
						if (!flag9)
						{
							this._previewProgressList.Add(new ReadingSkillBookPagesDisplay.PreviewPageProgress
							{
								PageIndex = n,
								ProgressValue = this._previewProgressBuffer[(int)n]
							});
						}
						n += 1;
					}
					bool hasSlotPreview = previewSkipPage || this._previewProgressList.Count > 0 || ViewReadingEvent.HasAnyPreviewMarker(this._previewSlotMarkers);
					bool flag10 = !hasSlotPreview;
					if (flag10)
					{
						this._curPagesDisplay.ClearStrategyPreview();
						result = false;
					}
					else
					{
						ReadingSkillBookPagesDisplay.ReadingStrategyPreview preview = new ReadingSkillBookPagesDisplay.ReadingStrategyPreview
						{
							PageIndex = this._curPage,
							PreviewSkipPage = previewSkipPage,
							EffectMultiplier = effectMultiplier,
							SlotMarkers = this._previewSlotMarkers,
							PreviewProgressList = this._previewProgressList
						};
						this._curPagesDisplay.ApplyStrategyPreview(preview);
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x0600741F RID: 29727 RVA: 0x003613F4 File Offset: 0x0035F5F4
		private bool ApplyProgressPreview(ReadingStrategyItem configData, SkillBookPageDisplayData displayData, out bool stopRepeatTrigger)
		{
			stopRepeatTrigger = false;
			bool hasTriggeredEffect = false;
			bool flag = configData.MaxProgressAddValue != 0;
			if (flag)
			{
				hasTriggeredEffect = true;
				int addValue = ViewReadingEvent.GetMinProgressAddValue(configData);
				this.ApplyProgressToPage(this._curPage, addValue);
				stopRepeatTrigger = (this._previewProgressBuffer[(int)this._curPage] >= 100);
			}
			bool flag2 = configData.NextPageProgressAddValue != 0 && this._previewProgressBuffer[(int)this._curPage] >= 100 && this.ApplyProgressToNextPage(displayData, (int)configData.NextPageProgressAddValue);
			if (flag2)
			{
				hasTriggeredEffect = true;
			}
			return hasTriggeredEffect;
		}

		// Token: 0x06007420 RID: 29728 RVA: 0x0036147B File Offset: 0x0035F67B
		private void ApplyProgressToPage(byte pageIndex, int addValue)
		{
			this._previewProgressBuffer[(int)pageIndex] = Math.Clamp(this._previewProgressBuffer[(int)pageIndex] + addValue, 0, 100);
		}

		// Token: 0x06007421 RID: 29729 RVA: 0x00361498 File Offset: 0x0035F698
		private bool ApplyProgressToNextPage(SkillBookPageDisplayData displayData, int addValue)
		{
			byte nextPage = this.FindNextProgressPage(displayData, this._curPage);
			bool flag = nextPage == byte.MaxValue;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.ApplyProgressToPage(nextPage, addValue);
				result = true;
			}
			return result;
		}

		// Token: 0x06007422 RID: 29730 RVA: 0x003614D4 File Offset: 0x0035F6D4
		private byte FindNextProgressPage(SkillBookPageDisplayData displayData, byte startPageIndex)
		{
			byte i = startPageIndex + 1;
			while ((int)i < displayData.ReadingProgress.Length)
			{
				bool flag = this.GetPreviewPageIncompleteState(displayData, i) == 2;
				if (!flag)
				{
					bool flag2 = this._previewProgressBuffer[(int)i] >= 100;
					if (!flag2)
					{
						return i;
					}
				}
				i += 1;
			}
			return byte.MaxValue;
		}

		// Token: 0x06007423 RID: 29731 RVA: 0x00361534 File Offset: 0x0035F734
		private sbyte GetPreviewPageIncompleteState(SkillBookPageDisplayData displayData, byte pageIndex)
		{
			sbyte pageState = displayData.State[(int)pageIndex];
			bool flag = this._referenceBooks == null;
			sbyte result;
			if (flag)
			{
				result = pageState;
			}
			else
			{
				foreach (ItemKey refBook in this._referenceBooks)
				{
					bool flag2 = !refBook.IsValid();
					if (!flag2)
					{
						SkillBookPageDisplayData refPageDisplayData;
						bool flag3 = !this._pageDisplayData.TryGetValue(refBook.Id, out refPageDisplayData);
						if (!flag3)
						{
							bool flag4 = refPageDisplayData.ItemKey.TemplateId != displayData.ItemKey.TemplateId;
							if (!flag4)
							{
								bool flag5 = !refPageDisplayData.State.CheckIndex((int)pageIndex);
								if (!flag5)
								{
									sbyte refState = refPageDisplayData.State[(int)pageIndex];
									bool hasSupply = refState < pageState;
									bool flag6 = displayData.IsCombatBook && refPageDisplayData.Type.CheckIndex((int)pageIndex) && displayData.Type.CheckIndex((int)pageIndex) && refPageDisplayData.Type[(int)pageIndex] != displayData.Type[(int)pageIndex];
									if (flag6)
									{
										hasSupply = false;
									}
									bool flag7 = hasSupply;
									if (flag7)
									{
										pageState = refState;
									}
								}
							}
						}
					}
				}
				result = pageState;
			}
			return result;
		}

		// Token: 0x06007424 RID: 29732 RVA: 0x0036166C File Offset: 0x0035F86C
		private bool ApplyEfficiencyPreview(ReadingStrategyItem configData, sbyte strategyId)
		{
			bool flag = this._curStrategyIndex < 0 || this.bookIntro == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				int currentEfficiency = this.bookIntro.CurrentReadingEfficiency;
				bool flag2 = currentEfficiency <= 0;
				if (flag2)
				{
					this.bookIntro.ClearEfficiencyPreview();
					result = false;
				}
				else
				{
					int currentBonus = this._curStrategies.GetPageReadingEfficiencyBonus(this._curPage);
					ReadingBookStrategies previewStrategies = this._curStrategies;
					bool clearPageStrategies = configData.ClearPageStrategies;
					if (clearPageStrategies)
					{
						previewStrategies.ClearPageStrategies(this._curPage);
					}
					bool flag3 = strategyId == 5;
					if (flag3)
					{
						ViewReadingEvent.ApplyDoubleEfficiencyPreview(ref previewStrategies, this._curPage);
					}
					sbyte efficiencyBonus = ViewReadingEvent.GetMinEfficiencyBonus(configData);
					previewStrategies.SetPageStrategy(this._curPage, this._curStrategyIndex, strategyId, efficiencyBonus);
					int previewBonus = previewStrategies.GetPageReadingEfficiencyBonus(this._curPage);
					bool flag4 = previewBonus == currentBonus;
					if (flag4)
					{
						this.bookIntro.ClearEfficiencyPreview();
						result = false;
					}
					else
					{
						int previewEfficiency = (int)Math.Round((double)(currentEfficiency * (100 + previewBonus)) / (double)Math.Max(1, 100 + currentBonus));
						previewEfficiency = Math.Max(0, previewEfficiency);
						SkillBookPageDisplayData pageDisplayData;
						bool flag5 = this._pageDisplayData.TryGetValue(this._curReadingBook.Id, out pageDisplayData) && ((pageDisplayData != null) ? pageDisplayData.ReadingProgress : null) != null;
						if (flag5)
						{
							previewEfficiency = Math.Min(previewEfficiency, pageDisplayData.ReadingProgress.Length * 100);
						}
						this.bookIntro.SetEfficiencyPreview(previewEfficiency);
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06007425 RID: 29733 RVA: 0x003617EC File Offset: 0x0035F9EC
		private void EnsurePreviewBuffer(int pageCount)
		{
			bool flag = this._previewProgressBuffer == null || this._previewProgressBuffer.Length != pageCount;
			if (flag)
			{
				this._previewProgressBuffer = new int[pageCount];
			}
		}

		// Token: 0x06007426 RID: 29734 RVA: 0x00361824 File Offset: 0x0035FA24
		private static int GetMinProgressAddValue(ReadingStrategyItem configData)
		{
			return (int)Math.Min(configData.MinProgressAddValue, configData.MaxProgressAddValue);
		}

		// Token: 0x06007427 RID: 29735 RVA: 0x00361848 File Offset: 0x0035FA48
		private static sbyte GetMinEfficiencyBonus(ReadingStrategyItem configData)
		{
			return Math.Min(configData.MinCurrPageEfficiencyChange, configData.MaxCurrPageEfficiencyChange);
		}

		// Token: 0x06007428 RID: 29736 RVA: 0x0036186C File Offset: 0x0035FA6C
		private static bool HasAnyPreviewMarker(ReadingSkillBookPagesDisplay.PreviewMarkerType[] markers)
		{
			bool flag = markers == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < markers.Length; i++)
				{
					bool flag2 = markers[i] > ReadingSkillBookPagesDisplay.PreviewMarkerType.None;
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06007429 RID: 29737 RVA: 0x003618B0 File Offset: 0x0035FAB0
		private unsafe static void ApplyDoubleEfficiencyPreview(ref ReadingBookStrategies strategies, byte pageIndex)
		{
			int startIndex = (int)(pageIndex * 3);
			for (int i = 0; i < 3; i++)
			{
				int index = startIndex + i;
				int value = (int)(*(ref strategies.Bonus.FixedElementField + index) * 2);
				*(ref strategies.Bonus.FixedElementField + index) = (sbyte)Math.Clamp(value, 0, 100);
			}
		}

		// Token: 0x0600742A RID: 29738 RVA: 0x00361904 File Offset: 0x0035FB04
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = btnName == "CloseBtn";
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x0600742B RID: 29739 RVA: 0x00361934 File Offset: 0x0035FB34
		public override void QuickHide()
		{
			bool flag = this.IsReadingEventOfTianShuXuanJiByAdoptiveFather();
			if (flag)
			{
				short bookSubType = ItemTemplateHelper.GetItemSubType(this._curReadingBook.ItemType, this._curReadingBook.TemplateId);
				int pageCount = (bookSubType == 1001) ? 6 : 5;
				bool hasNoFinishedPage = this.HasNoFinishedPage(pageCount);
				bool flag2 = hasNoFinishedPage;
				if (flag2)
				{
					return;
				}
				TaiwuEventDomainMethod.Call.TriggerListener("FinishReadingTianShuXuanJi", true);
			}
			AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			base.QuickHide();
		}

		// Token: 0x0600742C RID: 29740 RVA: 0x003619B0 File Offset: 0x0035FBB0
		private bool HasNoFinishedPage(int pageCount)
		{
			bool hasNoFinishedPage = false;
			for (int pageIndex = 0; pageIndex < pageCount; pageIndex++)
			{
				bool flag = this.CurPageDisplayData.ReadingProgress[pageIndex] < 100;
				if (flag)
				{
					hasNoFinishedPage = true;
					break;
				}
			}
			return hasNoFinishedPage;
		}

		// Token: 0x0600742D RID: 29741 RVA: 0x003619F4 File Offset: 0x0035FBF4
		private void RefreshCloseButton()
		{
			bool flag = !this.IsReadingEventOfTianShuXuanJiByAdoptiveFather();
			if (flag)
			{
				this.closeBtn.interactable = true;
			}
			else
			{
				short bookSubType = ItemTemplateHelper.GetItemSubType(this._curReadingBook.ItemType, this._curReadingBook.TemplateId);
				int pageCount = (bookSubType == 1001) ? 6 : 5;
				this.closeBtn.interactable = !this.HasNoFinishedPage(pageCount);
			}
		}

		// Token: 0x0600742E RID: 29742 RVA: 0x00361A60 File Offset: 0x0035FC60
		private bool CanNextPageInTutorial()
		{
			bool flag = this.IsReadingEventOfTianShuXuanJiByAdoptiveFather();
			return !flag || this.CurPageDisplayData.ReadingProgress[(int)this._curPage] >= 100;
		}

		// Token: 0x0600742F RID: 29743 RVA: 0x00361A9C File Offset: 0x0035FC9C
		private bool IsReadingEventOfTianShuXuanJiByAdoptiveFather()
		{
			TutorialChapterModel model = SingletonObject.getInstance<TutorialChapterModel>();
			bool flag = model.InGuiding && model.TutorialChapterIndex == 5;
			return flag && this.CurPageDisplayData.ItemKey.TemplateId == 878;
		}

		// Token: 0x04005683 RID: 22147
		[SerializeField]
		private CButton closeBtn;

		// Token: 0x04005684 RID: 22148
		[SerializeField]
		private CircularLayout extraStrategy;

		// Token: 0x04005685 RID: 22149
		[SerializeField]
		private ReadingPages readingPages;

		// Token: 0x04005686 RID: 22150
		[SerializeField]
		private TextMeshProUGUI intelligenceCost;

		// Token: 0x04005687 RID: 22151
		[SerializeField]
		private ReadingBookIntro bookIntro;

		// Token: 0x04005688 RID: 22152
		[SerializeField]
		private ReadingInspireHolder readingInspireHolder;

		// Token: 0x04005689 RID: 22153
		[SerializeField]
		private CToggleGroup strategyHolder;

		// Token: 0x0400568A RID: 22154
		[SerializeField]
		private List<CToggle> strategyToggleList;

		// Token: 0x0400568B RID: 22155
		private static readonly WaitForSeconds _waitForSeconds4 = new WaitForSeconds(4f);

		// Token: 0x0400568C RID: 22156
		private ItemKey _curReadingBook;

		// Token: 0x0400568D RID: 22157
		private ItemKey[] _referenceBooks;

		// Token: 0x0400568E RID: 22158
		private ReadingBookStrategies _curStrategies;

		// Token: 0x0400568F RID: 22159
		private IntList _curReadingBookExpireTime;

		// Token: 0x04005690 RID: 22160
		private ItemDisplayData _curBookDisplayData = new ItemDisplayData();

		// Token: 0x04005691 RID: 22161
		private readonly Dictionary<int, SkillBookPageDisplayData> _pageDisplayData = new Dictionary<int, SkillBookPageDisplayData>();

		// Token: 0x04005692 RID: 22162
		private int _taiwuCharId;

		// Token: 0x04005693 RID: 22163
		private List<byte> _selectableStrategies = new List<byte>();

		// Token: 0x04005694 RID: 22164
		private byte _curPage;

		// Token: 0x04005695 RID: 22165
		private int _curStrategyIndex;

		// Token: 0x04005696 RID: 22166
		private ReadingSkillBookPagesDisplay _curPagesDisplay;

		// Token: 0x04005697 RID: 22167
		private bool _inited;

		// Token: 0x04005698 RID: 22168
		private int _curIntelligence;

		// Token: 0x04005699 RID: 22169
		private int _maxIntelligence;

		// Token: 0x0400569A RID: 22170
		private const int NormalSelectableStrategyCount = 9;

		// Token: 0x0400569B RID: 22171
		private const int ExtraMaxSelectableStrategyCount = 3;

		// Token: 0x0400569C RID: 22172
		private int _sutraReadingRoomBuffValue;

		// Token: 0x0400569D RID: 22173
		private readonly HashSet<int> _usedStrategies = new HashSet<int>();

		// Token: 0x0400569E RID: 22174
		private int[] _previewProgressBuffer;

		// Token: 0x0400569F RID: 22175
		private readonly List<ReadingSkillBookPagesDisplay.PreviewPageProgress> _previewProgressList = new List<ReadingSkillBookPagesDisplay.PreviewPageProgress>();

		// Token: 0x040056A0 RID: 22176
		private readonly ReadingSkillBookPagesDisplay.PreviewMarkerType[] _previewSlotMarkers = new ReadingSkillBookPagesDisplay.PreviewMarkerType[3];

		// Token: 0x040056A1 RID: 22177
		private int _previewStrategyIndex = -1;

		// Token: 0x040056A2 RID: 22178
		private bool _modified = false;

		// Token: 0x040056A3 RID: 22179
		private int _getInventoryItemsCount = 0;
	}
}
