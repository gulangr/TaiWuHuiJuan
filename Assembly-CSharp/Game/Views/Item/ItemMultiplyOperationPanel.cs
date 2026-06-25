using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Item.Apply;
using Game.Views.CharacterMenu;
using GameData.Domains.Character;
using GameData.Domains.Character.Alertness;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Item
{
	// Token: 0x02000A12 RID: 2578
	public class ItemMultiplyOperationPanel : MonoBehaviour
	{
		// Token: 0x06007E07 RID: 32263 RVA: 0x003A7DB0 File Offset: 0x003A5FB0
		private void Awake()
		{
			this.quickEncyclopedia.gameObject.SetActive(false);
			this.toggleGroup.Init(-1);
			this.toggleGroup.OnActiveIndexChange += this.OnActiveToggleChange;
			List<CToggle> togList = this.toggleGroup.GetAll();
			for (int i = 0; i < togList.Count; i++)
			{
				bool show = i < ItemMultiplyOperationPanel.TogKey.Count.ToInt();
				CToggle tog = this.toggleGroup.Get(i);
				tog.gameObject.SetActive(show);
			}
			this.buttonClose.ClearAndAddListener(new Action(this.TryExitMultiplyMode));
			this.buttonConfirm.ClearAndAddListener(new Action(this.OnClickOperate));
			ItemListScroll.EColumnType toolFlags = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Durability;
			ItemListScroll.EColumnType normalFlags = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Value;
			ItemListScroll.EColumnType carrierFlags = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Durability | ItemListScroll.EColumnType.Tame;
			ItemListScroll.EColumnType gainFlags = ItemListScroll.EColumnType.IconAndName;
			ESortAndFilterControllerType controllerType = ESortAndFilterControllerType.Item;
			Dictionary<ItemListScroll.EColumnType, LayoutOption> toolLayout = new Dictionary<ItemListScroll.EColumnType, LayoutOption>
			{
				{
					ItemListScroll.EColumnType.IconAndName,
					new LayoutOption(300f, 300f, 300f, 1)
				},
				{
					ItemListScroll.EColumnType.Durability,
					new LayoutOption(300f, 300f, 300f, 1)
				}
			};
			Dictionary<ItemListScroll.EColumnType, LayoutOption> normalLayout = new Dictionary<ItemListScroll.EColumnType, LayoutOption>
			{
				{
					ItemListScroll.EColumnType.IconAndName,
					new LayoutOption(300f, 300f, 300f, 1)
				},
				{
					ItemListScroll.EColumnType.Amount,
					new LayoutOption(150f, 150f, 150f, 1)
				},
				{
					ItemListScroll.EColumnType.Value,
					new LayoutOption(150f, 150f, 150f, 1)
				}
			};
			Dictionary<ItemListScroll.EColumnType, LayoutOption> carrierLayout = new Dictionary<ItemListScroll.EColumnType, LayoutOption>
			{
				{
					ItemListScroll.EColumnType.IconAndName,
					new LayoutOption(300f, 300f, 300f, 1)
				},
				{
					ItemListScroll.EColumnType.Durability,
					new LayoutOption(150f, 150f, 150f, 1)
				},
				{
					ItemListScroll.EColumnType.Tame,
					new LayoutOption(150f, 150f, 150f, 1)
				}
			};
			this.toolListScroll.Init("ItemMultiplyOperationTool", controllerType, true, new Action<ITradeableContent, RowItemLine>(this.OnItemRenderTool), null, toolFlags, toolLayout, null, null);
			this.gainListScroll.Init("ItemMultiplyOperationGain", controllerType, true, new Action<ITradeableContent, RowItemLine>(this.OnItemRender), null, gainFlags, null, null, null);
			this.discardListScroll.Init("ItemMultiplyOperationDiscard", controllerType, true, new Action<ITradeableContent, RowItemLine>(this.OnItemRender), null, normalFlags, normalLayout, null, null);
			this.carrierListScroll.Init("ItemMultiplyOperationCarrier", controllerType, true, new Action<ITradeableContent, RowItemLine>(this.OnItemRenderCarrier), new Action<ITradeableContent, RowItemLine>(this.OnItemClickCarrier), carrierFlags, carrierLayout, null, null);
			this.materialListScroll.Init("ItemMultiplyOperationMaterial", controllerType, true, new Action<ITradeableContent, RowItemLine>(this.OnItemRender), null, normalFlags, normalLayout, null, null);
			ItemListScroll.EColumnType cricketFlags = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Value;
			this.cricketListScroll.Init("ItemMultiplyOperationCricket", controllerType, true, new Action<ITradeableContent, RowItemLine>(this.OnItemRenderCricket), new Action<ITradeableContent, RowItemLine>(this.OnItemClickCricket), cricketFlags, normalLayout, null, null);
			this.bloodDewListScroll.Init("ItemMultiplyOperationBloodDew", controllerType, true, new Action<ITradeableContent, RowItemLine>(this.OnItemRender), null, normalFlags, normalLayout, null, null);
			this.transferListScroll.Init("ItemMultiplyOperationTransfer", controllerType, true, new Action<ITradeableContent, RowItemLine>(this.OnItemRender), null, normalFlags, normalLayout, null, null);
			this.ClearTransferPage();
		}

		// Token: 0x06007E08 RID: 32264 RVA: 0x003A80F4 File Offset: 0x003A62F4
		private void OnEnable()
		{
			GEvent.Add(UiEvents.ItemMultiplyOperationContentChange, new GEvent.Callback(this.OnItemMultiplyOperationContentChange));
			GEvent.Add(UiEvents.ExitMultiplyOperation, new GEvent.Callback(this.OnExitMultiplyOperation));
			GEvent.Add(UiEvents.OnSelectTransferItemChar, new GEvent.Callback(this.OnSelectTransferItemChar));
		}

		// Token: 0x06007E09 RID: 32265 RVA: 0x003A8150 File Offset: 0x003A6350
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.ItemMultiplyOperationContentChange, new GEvent.Callback(this.OnItemMultiplyOperationContentChange));
			GEvent.Remove(UiEvents.ExitMultiplyOperation, new GEvent.Callback(this.OnExitMultiplyOperation));
			GEvent.Remove(UiEvents.OnSelectTransferItemChar, new GEvent.Callback(this.OnSelectTransferItemChar));
		}

		// Token: 0x06007E0A RID: 32266 RVA: 0x003A81AC File Offset: 0x003A63AC
		public void Show(ArgumentBox argsBox)
		{
			this._charId = -1;
			this._hasTeammate = false;
			bool exist = UIElement.CharacterMenu.Exist;
			if (exist)
			{
				ViewCharacterMenu menu = UIElement.CharacterMenu.UiBaseAs<ViewCharacterMenu>();
				IReadOnlyList<CharacterDisplayData> characters = (menu != null) ? menu.DisplayCharacters : null;
				bool flag = characters != null;
				if (flag)
				{
					int count = 0;
					for (int i = 0; i < characters.Count; i++)
					{
						CharacterDisplayData item = characters[i];
						bool flag2 = item == null;
						if (!flag2)
						{
							bool flag3 = menu.IsTaiwuBeastTeammate(item.CharacterId);
							if (!flag3)
							{
								count++;
							}
						}
					}
					this._hasTeammate = (count > 1);
				}
			}
			this.RefreshData(argsBox);
			this.RefreshToggleInteractable();
			ItemMultiplyOperationPanel.TogKey togKey = this.GetTogKey(this._curOperationType);
			this.quickEncyclopedia.gameObject.SetActive(togKey == ItemMultiplyOperationPanel.TogKey.Disassemble);
			this.toggleGroup.SetWithoutNotify((int)togKey);
			this.RefreshPage();
			base.gameObject.SetActive(true);
		}

		// Token: 0x06007E0B RID: 32267 RVA: 0x003A82B0 File Offset: 0x003A64B0
		public void Hide()
		{
			base.gameObject.SetActive(false);
			this.ClearTransferPage();
			bool flag = this._clothingChangeCharId >= 0;
			if (flag)
			{
				int id = this._clothingChangeCharId;
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
				{
					GEvent.OnEvent(UiEvents.OnChangeCharacterClothing, EasyPool.Get<ArgumentBox>().Set("CharacterId", id));
				});
				this._clothingChangeCharId = -1;
			}
		}

		// Token: 0x06007E0C RID: 32268 RVA: 0x003A8318 File Offset: 0x003A6518
		private void OnItemRender(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			rowItemLine.SetInteractable(false, false);
			string charName = NameCenter.GetMonasticTitleOrDisplayName(this._taiwuCharacterDisplayData, true);
			RowItemLine.SetResourceTip(rowItemLine.Data, rowItemLine.TipDisplayer, charName, false, false);
		}

		// Token: 0x06007E0D RID: 32269 RVA: 0x003A836C File Offset: 0x003A656C
		private void OnItemRenderTool(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			this.OnItemRender(itemData, rowItemLine);
			string content = itemData.DurabilityChange.IsNullOrEmpty() ? CommonUtils.GetDurabilityString(itemData.Durability, itemData.MaxDurability) : itemData.DurabilityChange;
			rowItemLine.RowItemMain.ItemBack.SetCountInfo(content, "ui9_icon_item_info_durability");
		}

		// Token: 0x06007E0E RID: 32270 RVA: 0x003A83C4 File Offset: 0x003A65C4
		private void OnActiveToggleChange(int newKey, int oldKey)
		{
			bool processingToggleChange = this._processingToggleChange;
			if (!processingToggleChange)
			{
				this._processingToggleChange = true;
				ItemOperationType.EItemOperationType operationType = this.GetItemOperationType((ItemMultiplyOperationPanel.TogKey)newKey);
				bool flag = this._curOperationType != operationType || (operationType == ItemOperationType.EItemOperationType.Feeding && newKey == 5 != this._isFeedingCricket);
				if (flag)
				{
					ArgumentBox argsBox = EasyPool.Get<ArgumentBox>().Set("ItemOperationType", operationType);
					bool flag2 = operationType == ItemOperationType.EItemOperationType.Feeding;
					if (flag2)
					{
						argsBox.Set("IsCricketFeeding", newKey == 5);
					}
					GEvent.OnEvent(UiEvents.ItemMultiplyOperationTypeChange, argsBox);
				}
				this._isFeedingCricket = (newKey == 5);
				this._curOperationType = operationType;
				this.quickEncyclopedia.gameObject.SetActive(newKey == 1);
				this.RefreshPage();
				this._processingToggleChange = false;
			}
		}

		// Token: 0x06007E0F RID: 32271 RVA: 0x003A8490 File Offset: 0x003A6690
		private ItemMultiplyOperationPanel.TogKey GetTogKey(ItemOperationType.EItemOperationType itemOperationType)
		{
			if (!true)
			{
			}
			ItemMultiplyOperationPanel.TogKey result;
			switch (itemOperationType)
			{
			case ItemOperationType.EItemOperationType.Repair:
				result = ItemMultiplyOperationPanel.TogKey.Repair;
				goto IL_4C;
			case ItemOperationType.EItemOperationType.Disassemble:
				result = ItemMultiplyOperationPanel.TogKey.Disassemble;
				goto IL_4C;
			case ItemOperationType.EItemOperationType.Transfer:
				result = ItemMultiplyOperationPanel.TogKey.Transfer;
				goto IL_4C;
			case ItemOperationType.EItemOperationType.Discard:
				result = ItemMultiplyOperationPanel.TogKey.Discard;
				goto IL_4C;
			case ItemOperationType.EItemOperationType.Feeding:
				result = (this._isFeedingCricket ? ItemMultiplyOperationPanel.TogKey.CricketFeeding : ItemMultiplyOperationPanel.TogKey.Feeding);
				goto IL_4C;
			}
			result = ItemMultiplyOperationPanel.TogKey.Disassemble;
			IL_4C:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007E10 RID: 32272 RVA: 0x003A84F4 File Offset: 0x003A66F4
		private ItemOperationType.EItemOperationType GetItemOperationType(ItemMultiplyOperationPanel.TogKey togKey)
		{
			if (!true)
			{
			}
			ItemOperationType.EItemOperationType result;
			switch (togKey)
			{
			case ItemMultiplyOperationPanel.TogKey.Repair:
				result = ItemOperationType.EItemOperationType.Repair;
				break;
			case ItemMultiplyOperationPanel.TogKey.Disassemble:
				result = ItemOperationType.EItemOperationType.Disassemble;
				break;
			case ItemMultiplyOperationPanel.TogKey.Discard:
				result = ItemOperationType.EItemOperationType.Discard;
				break;
			case ItemMultiplyOperationPanel.TogKey.Feeding:
				result = ItemOperationType.EItemOperationType.Feeding;
				break;
			case ItemMultiplyOperationPanel.TogKey.Transfer:
				result = ItemOperationType.EItemOperationType.Transfer;
				break;
			case ItemMultiplyOperationPanel.TogKey.CricketFeeding:
				result = ItemOperationType.EItemOperationType.Feeding;
				break;
			default:
				result = ItemOperationType.EItemOperationType.Disassemble;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007E11 RID: 32273 RVA: 0x003A854C File Offset: 0x003A674C
		private string GetItemOperationTypeName(ItemOperationType.EItemOperationType operationType)
		{
			if (!true)
			{
			}
			LanguageKey languageKey;
			switch (operationType)
			{
			case ItemOperationType.EItemOperationType.Repair:
				languageKey = LanguageKey.LK_Repair_Item;
				goto IL_6B;
			case ItemOperationType.EItemOperationType.Disassemble:
				languageKey = LanguageKey.LK_Disassemble_Item;
				goto IL_6B;
			case ItemOperationType.EItemOperationType.Transfer:
				languageKey = LanguageKey.LK_Transfer_Item;
				goto IL_6B;
			case ItemOperationType.EItemOperationType.Take:
				languageKey = LanguageKey.LK_TakeFrom_Item;
				goto IL_6B;
			case ItemOperationType.EItemOperationType.Discard:
				languageKey = LanguageKey.LK_Discard_Item;
				goto IL_6B;
			case ItemOperationType.EItemOperationType.Feeding:
				languageKey = LanguageKey.LK_Feeding_Item;
				goto IL_6B;
			}
			throw new ArgumentOutOfRangeException("operationType", operationType, null);
			IL_6B:
			if (!true)
			{
			}
			LanguageKey key = languageKey;
			return LocalStringManager.Get(key);
		}

		// Token: 0x06007E12 RID: 32274 RVA: 0x003A85D4 File Offset: 0x003A67D4
		private void OnItemMultiplyOperationContentChange(ArgumentBox argsBox)
		{
			this.RefreshData(argsBox);
			ItemOperationType.EItemOperationType curOperationType = this._curOperationType;
			bool flag = curOperationType == ItemOperationType.EItemOperationType.Transfer || curOperationType == ItemOperationType.EItemOperationType.Take;
			if (flag)
			{
				this.RefreshCharDebt();
			}
			else
			{
				this.RefreshPage();
			}
		}

		// Token: 0x06007E13 RID: 32275 RVA: 0x003A8616 File Offset: 0x003A6816
		private void OnExitMultiplyOperation(ArgumentBox argsBox)
		{
			this.TryExitMultiplyMode();
		}

		// Token: 0x06007E14 RID: 32276 RVA: 0x003A8620 File Offset: 0x003A6820
		private void RefreshData(ArgumentBox argsBox)
		{
			argsBox.Get<ItemKey>("EmptyToolKey", out this._emptyToolKey);
			argsBox.Get<CharacterDisplayData>("CharData", out this._taiwuCharacterDisplayData);
			argsBox.Get<Dictionary<ItemDisplayData, int>>("ItemDict", out this._itemDict);
			argsBox.Get<List<ItemDisplayData>>("ItemList", out this._itemOrderedList);
			argsBox.Get<Dictionary<ItemDisplayData, short>>("ToolDict", out this._toolDict);
			argsBox.Get("UseEmptyTool", out this._useEmptyTool);
			Enum curOperationType;
			argsBox.Get("ItemOperationType", out curOperationType);
			Enum itemSourceType;
			argsBox.Get("ItemSourceType", out itemSourceType);
			this._curOperationType = (ItemOperationType.EItemOperationType)curOperationType;
			this._itemSourceType = (ItemSourceType)itemSourceType;
			bool flag = !argsBox.Get<ResourceInts>("NeedResource", out this._needResource);
			if (flag)
			{
				this._needResource.Initialize();
			}
			bool flag2 = !argsBox.Get<ResourceInts>("Resource", out this._resource);
			if (flag2)
			{
				this._resource.Initialize();
			}
			bool flag3 = !argsBox.Get<ResourceInts>("GetResource", out this._getResource);
			if (flag3)
			{
				this._getResource.Initialize();
			}
			argsBox.Get<List<short>>("ChanceMaterial", out this._chanceMaterialIdList);
			argsBox.Get<ITradeableContent>("FeedingTarget", out this._feedingTarget);
			argsBox.Get<List<ItemDisplayData>>("FeedingTargetList", out this._feedingTargetList);
			this._isFeedingCricket = (this._feedingTarget != null && this._feedingTarget.Key.ItemType == 11);
			bool flag4 = !argsBox.Get("HasCarrierFeeding", out this._hasCarrierFeeding);
			if (flag4)
			{
				this._hasCarrierFeeding = false;
			}
			bool flag5 = !argsBox.Get("HasCricketFeeding", out this._hasCricketFeeding);
			if (flag5)
			{
				this._hasCricketFeeding = false;
			}
		}

		// Token: 0x06007E15 RID: 32277 RVA: 0x003A87DC File Offset: 0x003A69DC
		private void HideAllToggle()
		{
			foreach (object obj in Enum.GetValues(typeof(ItemMultiplyOperationPanel.TogKey)))
			{
				int toggleKey = (int)obj;
				this.toggleGroup.Get(toggleKey).gameObject.SetActive(false);
			}
		}

		// Token: 0x06007E16 RID: 32278 RVA: 0x003A8854 File Offset: 0x003A6A54
		private void ShowToggle(ItemMultiplyOperationPanel.TogKey togKey, bool visibility, bool interactable = true)
		{
			CToggle tog = this.toggleGroup.Get((int)togKey);
			tog.gameObject.SetActive(visibility);
			tog.interactable = interactable;
			TooltipInvoker tip = tog.GetComponentInChildren<TooltipInvoker>();
			tip.Type = TipType.SingleDesc;
			ItemOperationType.EItemOperationType operationType = this.GetItemOperationType(togKey);
			string operationName = this.GetItemOperationTypeName(operationType);
			tip.PresetParam = new string[]
			{
				operationName
			};
		}

		// Token: 0x06007E17 RID: 32279 RVA: 0x003A88B4 File Offset: 0x003A6AB4
		private void RefreshToggleInteractable()
		{
			ItemOperationType.EItemOperationType curOperationType = this._curOperationType;
			bool isNormalOperation = curOperationType == ItemOperationType.EItemOperationType.Repair || curOperationType == ItemOperationType.EItemOperationType.Disassemble || curOperationType == ItemOperationType.EItemOperationType.Discard || curOperationType == ItemOperationType.EItemOperationType.Transfer || curOperationType == ItemOperationType.EItemOperationType.Feeding;
			bool isTroughSource = this._itemSourceType == ItemSourceType.Trough;
			bool isInCharacterMenuItems = UIElement.CharacterMenuItems.Exist && UIElement.CharacterMenuItems.IsShowing;
			this.ShowToggle(ItemMultiplyOperationPanel.TogKey.Repair, true, isNormalOperation && !isTroughSource);
			this.ShowToggle(ItemMultiplyOperationPanel.TogKey.Disassemble, true, isNormalOperation);
			this.ShowToggle(ItemMultiplyOperationPanel.TogKey.Discard, true, isNormalOperation);
			this.ShowToggle(ItemMultiplyOperationPanel.TogKey.Feeding, true, isNormalOperation && this._hasCarrierFeeding);
			this.ShowToggle(ItemMultiplyOperationPanel.TogKey.CricketFeeding, true, isNormalOperation && this._hasCricketFeeding);
			this.ShowToggle(ItemMultiplyOperationPanel.TogKey.Transfer, isInCharacterMenuItems, isNormalOperation && this._hasTeammate);
		}

		// Token: 0x06007E18 RID: 32280 RVA: 0x003A8970 File Offset: 0x003A6B70
		private void RefreshPage()
		{
			this.textOperationTypeTitle.text = this.GetItemOperationTypeName(this._curOperationType);
			this.ShowPage(this.discardPage.gameObject, ItemOperationType.EItemOperationType.Discard);
			this.feedingPage.SetActive(this._curOperationType == ItemOperationType.EItemOperationType.Feeding && !this._isFeedingCricket);
			this.cricketFeedingPage.SetActive(this._curOperationType == ItemOperationType.EItemOperationType.Feeding && this._isFeedingCricket);
			this.ShowPage(this.transferPage.gameObject, ItemOperationType.EItemOperationType.Transfer);
			this.RefreshResourcePage(false, true);
			this.RefreshToolPage(false);
			this.RefreshGainPage(false, null);
			switch (this._curOperationType)
			{
			case ItemOperationType.EItemOperationType.Repair:
				this.RefreshRepairPage();
				this.CheckNeedEmpty(ItemOperationType.EItemOperationType.Repair);
				goto IL_133;
			case ItemOperationType.EItemOperationType.Disassemble:
				this.RefreshDisassemblePage();
				this.CheckNeedEmpty(ItemOperationType.EItemOperationType.Disassemble);
				goto IL_133;
			case ItemOperationType.EItemOperationType.Transfer:
				this.RefreshTransferPage();
				this.CheckNeedEmpty(ItemOperationType.EItemOperationType.Transfer);
				goto IL_133;
			case ItemOperationType.EItemOperationType.Discard:
				this.RefreshDiscardPage();
				this.CheckNeedEmpty(ItemOperationType.EItemOperationType.Discard);
				goto IL_133;
			case ItemOperationType.EItemOperationType.Feeding:
			{
				bool isFeedingCricket = this._isFeedingCricket;
				if (isFeedingCricket)
				{
					this.RefreshCricketFeedingPage();
				}
				else
				{
					this.RefreshFeedingPage();
				}
				this.CheckNeedEmpty(ItemOperationType.EItemOperationType.Feeding);
				goto IL_133;
			}
			}
			throw new ArgumentOutOfRangeException();
			IL_133:
			ItemOperationType.EItemOperationType curOperationType = this._curOperationType;
			if (!true)
			{
			}
			int num;
			if (curOperationType != ItemOperationType.EItemOperationType.Repair)
			{
				if (curOperationType != ItemOperationType.EItemOperationType.Disassemble)
				{
					num = -1;
				}
				else
				{
					num = 107;
				}
			}
			else
			{
				num = 106;
			}
			if (!true)
			{
			}
			int triggerKey = num;
			bool flag = triggerKey >= 0;
			if (flag)
			{
				GlobalDomainMethod.Call.InvokeGuidingTrigger((short)triggerKey);
			}
		}

		// Token: 0x06007E19 RID: 32281 RVA: 0x003A8AF6 File Offset: 0x003A6CF6
		private void ShowPage(GameObject page, ItemOperationType.EItemOperationType operationType)
		{
			page.SetActive(this._curOperationType == operationType);
		}

		// Token: 0x06007E1A RID: 32282 RVA: 0x003A8B0C File Offset: 0x003A6D0C
		private void RefreshBtn(bool isDisabled = false)
		{
			string operationName = this.GetItemOperationTypeName(this._curOperationType);
			this.buttonConfirm.GetComponentInChildren<TextMeshProUGUI>().text = operationName;
			Selectable selectable = this.buttonConfirm;
			Dictionary<ItemDisplayData, int> itemDict = this._itemDict;
			selectable.interactable = (itemDict != null && itemDict.Count > 0 && !isDisabled);
			TooltipInvoker tip = this.buttonConfirm.GetComponent<TooltipInvoker>();
			tip.enabled = false;
			tip.Type = TipType.SingleDesc;
			string[] presetParam = tip.PresetParam;
			bool flag = presetParam == null || presetParam.Length != 1;
			if (flag)
			{
				tip.PresetParam = new string[1];
			}
			bool flag2 = this._curOperationType == ItemOperationType.EItemOperationType.Feeding && this._feedingTarget != null;
			if (flag2)
			{
				bool isFeedingCricket = this._isFeedingCricket;
				if (isFeedingCricket)
				{
					int spiritMax = GlobalConfig.Instance.CricketSpiritMax;
					bool needFeed = this._feedingTarget.CricketData != null && this._feedingTarget.CricketData.Spirit < spiritMax;
					List<ItemDisplayData> itemOrderedList = this._itemOrderedList;
					bool hasCost = itemOrderedList != null && itemOrderedList.Count > 0;
					this.buttonConfirm.interactable = (needFeed && hasCost);
					tip.enabled = !this.buttonConfirm.interactable;
					tip.PresetParam[0] = (needFeed ? LanguageKey.LK_Cricket_Feeding_Tip_NoBloodDew.Tr() : LanguageKey.LK_Cricket_Feeding_Tip_NoNeed.Tr());
				}
				else
				{
					bool needFeed2 = this._feedingTarget.Durability != this._feedingTarget.MaxDurability || this._feedingTarget.CarrierTamePoint != GlobalConfig.Instance.MaxCarrierTamePoint;
					List<ItemDisplayData> itemOrderedList = this._itemOrderedList;
					bool hasCost2 = itemOrderedList != null && itemOrderedList.Count > 0;
					this.buttonConfirm.interactable = (needFeed2 && hasCost2);
					tip.enabled = !this.buttonConfirm.interactable;
					tip.PresetParam[0] = (needFeed2 ? LanguageKey.LK_Feeding_Item_Tip_MaterialNotMeet.Tr() : LanguageKey.LK_Feeding_Item_Tip_NoNeed.Tr());
				}
			}
		}

		// Token: 0x06007E1B RID: 32283 RVA: 0x003A8D10 File Offset: 0x003A6F10
		private void RefreshDisassemblePage()
		{
			this.RefreshResourcePage(true, true);
			this.RefreshToolPage(true);
			this._chanceMaterialList.Clear();
			bool flag = this._chanceMaterialIdList != null;
			if (flag)
			{
				var materialGroupList = this._chanceMaterialIdList.GroupBy((short item) => item, (short key, IEnumerable<short> ids) => new
				{
					id = key,
					count = ids.Count<short>()
				}).ToList();
				IEnumerable<ItemDisplayData> materialList = materialGroupList.Select(delegate(item)
				{
					ItemKey key = new ItemKey(5, 0, item.id, -1);
					return new ItemDisplayData
					{
						Key = key
					};
				});
				this._chanceMaterialList.AddRange(materialList);
			}
			this.RefreshGainPage(true, this._chanceMaterialList);
			this.RefreshBtn(false);
		}

		// Token: 0x06007E1C RID: 32284 RVA: 0x003A8DE8 File Offset: 0x003A6FE8
		private void RefreshDiscardPage()
		{
			List<ItemDisplayData> itemList = (from p in this._itemOrderedList
			select p.Clone(this._itemDict[p])).ToList<ItemDisplayData>();
			this.discardListScroll.SetItemList(itemList);
			this.RefreshBtn(false);
		}

		// Token: 0x06007E1D RID: 32285 RVA: 0x003A8E28 File Offset: 0x003A7028
		private void RefreshRepairPage()
		{
			this.RefreshResourcePage(true, false);
			this.RefreshToolPage(true);
			bool isMeet = this._resource.CheckIsMeet(ref this._needResource);
			this.RefreshBtn(!isMeet);
		}

		// Token: 0x06007E1E RID: 32286 RVA: 0x003A8E64 File Offset: 0x003A7064
		private void OnItemRenderCarrier(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			bool isSelected = this._feedingTarget == itemData;
			rowItemLine.SetSelected(isSelected);
			string content = itemData.DurabilityChange.IsNullOrEmpty() ? CommonUtils.GetDurabilityString(itemData.Durability, itemData.MaxDurability) : itemData.DurabilityChange;
			rowItemLine.RowItemMain.ItemBack.SetCountInfo(content, "ui9_icon_item_info_durability");
		}

		// Token: 0x06007E1F RID: 32287 RVA: 0x003A8EDA File Offset: 0x003A70DA
		private void OnItemClickCarrier(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			this._feedingTarget = itemData;
			this.RefreshFeedingPage();
		}

		// Token: 0x06007E20 RID: 32288 RVA: 0x003A8EEC File Offset: 0x003A70EC
		private void RefreshFeedingPage()
		{
			this.carrierListScroll.SetItemList(this._feedingTargetList);
			bool flag = this._feedingTarget != null && this._feedingTarget.Key.ItemType != 11;
			if (flag)
			{
				int targetIndex = this.carrierListScroll.FindItemIndex(this._feedingTarget);
				this.carrierListScroll.InfiniteScroll.ScrollTo(targetIndex, 0.3f);
			}
			List<ItemDisplayData> itemList = (from p in this._itemOrderedList
			select p.Clone(this._itemDict[p])).ToList<ItemDisplayData>();
			this.materialListScroll.SetItemList(itemList);
			int addDurability = 0;
			bool flag2 = this._feedingTarget != null;
			if (flag2)
			{
				Dictionary<ItemDisplayData, int> itemDict = this._itemDict;
				bool flag3 = itemDict != null && itemDict.Count > 0;
				if (flag3)
				{
					foreach (KeyValuePair<ItemDisplayData, int> keyValuePair in this._itemDict)
					{
						ItemDisplayData itemDisplayData;
						int num;
						keyValuePair.Deconstruct(out itemDisplayData, out num);
						ItemDisplayData itemData = itemDisplayData;
						int count = num;
						addDurability += GameData.Domains.Extra.SharedMethods.GetFoodAddCarrierDurability(this._feedingTarget.Key.TemplateId, itemData.Key.TemplateId, count);
					}
				}
				addDurability = Mathf.Min(addDurability, this._feedingTarget.MaxDurability - this._feedingTarget.Durability);
			}
			ITradeableContent feedingTarget = this._feedingTarget;
			int curDurability = (feedingTarget != null) ? feedingTarget.Durability : 0;
			ITradeableContent feedingTarget2 = this._feedingTarget;
			int curMaxDurability = (feedingTarget2 != null) ? feedingTarget2.MaxDurability : 0;
			bool isMax = curDurability == curMaxDurability;
			string curDurabilityColor = (addDurability == 0) ? "pinkyellow" : (isMax ? "brightblue" : "brightred");
			string curDurabilityStr = curDurability.ToString().SetColor(curDurabilityColor);
			string addDurabilityStr = (addDurability == 0) ? string.Empty : string.Format("+{0}", addDurability).SetColor("brightblue");
			string durabilityContent = string.Format("{0}{1}/{2}", curDurabilityStr, addDurabilityStr, curMaxDurability);
			this.propertyDurability.Set(string.Empty, LanguageKey.LK_Feeding_Durability.Tr(), durabilityContent, null, false);
			bool flag4 = this._feedingTarget != null && ItemTemplateHelper.HasCarrierTame(this._feedingTarget.Key.ItemType, this._feedingTarget.Key.TemplateId);
			if (flag4)
			{
				int curTame = this._feedingTarget.CarrierTamePoint;
				string curTameColor = (curTame >= 100) ? "brightblue" : "brightred";
				string curTameText = curTame.ToString().SetColor(curTameColor);
				int addTame = 0;
				Dictionary<ItemDisplayData, int> itemDict = this._itemDict;
				bool flag5 = itemDict != null && itemDict.Count > 0;
				if (flag5)
				{
					foreach (KeyValuePair<ItemDisplayData, int> keyValuePair in this._itemDict)
					{
						ItemDisplayData itemDisplayData;
						int num;
						keyValuePair.Deconstruct(out itemDisplayData, out num);
						ItemDisplayData itemData2 = itemDisplayData;
						int count2 = num;
						addTame += GameData.Domains.Extra.SharedMethods.GetFoodAddCarrierTamePoint(this._feedingTarget.Key.TemplateId, itemData2.Key.TemplateId, count2);
					}
				}
				addTame = Mathf.Min(addTame, GlobalConfig.Instance.MaxCarrierTamePoint - curTame);
				string addTameText = (addTame == 0) ? string.Empty : string.Format("+{0}", addTame).SetColor("brightblue");
				this.propertyTame.gameObject.SetActive(true);
				string tameContent = string.Format("{0}{1}/{2}", curTameText, addTameText, GlobalConfig.Instance.MaxCarrierTamePoint);
				this.propertyTame.Set(string.Empty, LanguageKey.LK_Feeding_TamePoint.Tr(), tameContent, null, false);
			}
			else
			{
				this.propertyTame.gameObject.SetActive(false);
			}
			this.RefreshBtn(false);
			bool flag6 = this._feedingTarget != null;
			if (flag6)
			{
				ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("FeedingTarget", this._feedingTarget);
				GEvent.OnEvent(UiEvents.ItemMultiplyOperationTargetChange, args);
			}
		}

		// Token: 0x06007E21 RID: 32289 RVA: 0x003A9310 File Offset: 0x003A7510
		private void OnItemRenderCricket(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			bool isSelected = this._feedingTarget == itemData;
			rowItemLine.SetSelected(isSelected);
			ItemDisplayData cd = itemData as ItemDisplayData;
			bool flag = cd != null && cd.CricketData != null;
			if (flag)
			{
				int spiritMax = GlobalConfig.Instance.CricketSpiritMax;
				rowItemLine.RowItemMain.ItemBack.SetCountInfo(string.Format("{0}/{1}", cd.CricketData.Spirit, spiritMax), "ui9_icon_item_info_durability");
			}
		}

		// Token: 0x06007E22 RID: 32290 RVA: 0x003A93A7 File Offset: 0x003A75A7
		private void OnItemClickCricket(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			this._feedingTarget = itemData;
			this.RefreshCricketFeedingPage();
		}

		// Token: 0x06007E23 RID: 32291 RVA: 0x003A93B8 File Offset: 0x003A75B8
		private void RefreshCricketFeedingPage()
		{
			this.cricketListScroll.SetItemList(this._feedingTargetList);
			bool flag = this._feedingTarget != null && this._feedingTarget.Key.ItemType == 11;
			if (flag)
			{
				int targetIndex = this.cricketListScroll.FindItemIndex(this._feedingTarget);
				this.cricketListScroll.InfiniteScroll.ScrollTo(targetIndex, 0.3f);
			}
			List<ItemDisplayData> itemList = (from p in this._itemOrderedList
			select p.Clone(this._itemDict[p])).ToList<ItemDisplayData>();
			this.bloodDewListScroll.SetItemList(itemList);
			int curSpirit = 0;
			int spiritMax = GlobalConfig.Instance.CricketSpiritMax;
			int addSpirit = 0;
			ItemDisplayData target = this._feedingTarget as ItemDisplayData;
			bool flag2 = target != null && target.CricketData != null;
			if (flag2)
			{
				curSpirit = target.CricketData.Spirit;
				List<ItemDisplayData> itemOrderedList = this._itemOrderedList;
				bool flag3 = itemOrderedList != null && itemOrderedList.Count > 0;
				if (flag3)
				{
					foreach (ItemDisplayData itemData in this._itemOrderedList)
					{
						int count;
						this._itemDict.TryGetValue(itemData, out count);
						sbyte grade = Misc.Instance[itemData.Key.TemplateId].Grade;
						addSpirit += GlobalConfig.Instance.CricketBloodDewAddSpirit[(int)grade] * count;
					}
				}
				addSpirit = Mathf.Min(addSpirit, spiritMax - curSpirit);
			}
			string curSpiritColor = (curSpirit >= spiritMax) ? "brightblue" : "brightred";
			string curSpiritStr = curSpirit.ToString().SetColor(curSpiritColor);
			string addSpiritStr = (addSpirit == 0) ? string.Empty : string.Format("+{0}", addSpirit).SetColor("brightblue");
			string spiritContent = string.Format("{0}{1}/{2}", curSpiritStr, addSpiritStr, spiritMax);
			this.propertySpirit.Set(string.Empty, LanguageKey.LK_Cricket_Feeding_SpiritAdd.Tr(), spiritContent, null, false);
			this.RefreshBtn(false);
			bool flag4 = this._feedingTarget != null;
			if (flag4)
			{
				ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("FeedingTarget", this._feedingTarget);
				GEvent.OnEvent(UiEvents.ItemMultiplyOperationTargetChange, args);
			}
		}

		// Token: 0x06007E24 RID: 32292 RVA: 0x003A9610 File Offset: 0x003A7810
		private void RefreshTransferPage()
		{
			bool flag = this._targetCharacterDisplayData == null;
			if (!flag)
			{
				List<ItemDisplayData> itemList = (from p in this._itemOrderedList
				select p.Clone(this._itemDict[p])).ToList<ItemDisplayData>();
				this.transferListScroll.SetItemList(itemList);
				this.RefreshBtn(false);
				bool charTypeIsMeet = this._targetCharacterDisplayData.CreatingType == 1;
				bool showChange = charTypeIsMeet && this._itemDict.Count > 0;
				bool flag2 = showChange;
				if (flag2)
				{
					Inventory items = new Inventory();
					foreach (KeyValuePair<ItemDisplayData, int> keyValuePair in this._itemDict)
					{
						ItemDisplayData itemDisplayData;
						int num;
						keyValuePair.Deconstruct(out itemDisplayData, out num);
						ItemDisplayData itemData = itemDisplayData;
						int amount = num;
						bool isResource = itemData.IsResource;
						if (isResource)
						{
							items.OfflineAdd(itemData.Key, amount);
						}
						else
						{
							Inventory list = itemData.GetOperationInventoryFromPool(amount, true);
							items.OfflineAdd(list);
							ItemDisplayData.ReturnInventoryToPool(list);
						}
					}
					CharacterDomainMethod.AsyncCall.GetTransferItemPreviewDisplayData(null, this._charId, items, true, delegate(int offset, RawDataPool pool)
					{
						TransferItemPreviewDisplayData transferItemPreviewDisplayData = new TransferItemPreviewDisplayData();
						Serializer.Deserialize(pool, offset, ref transferItemPreviewDisplayData);
						ItemMultiplyOperationPanel.RefreshTransferPreview(transferItemPreviewDisplayData, this.propertyChangeAlertness, this.propertyChangeFavor, this.propertyChangeHappiness, this.propertyChangeAlertnessTitle, this.propertyChangeFavorTitle, this.propertyChangeHappinessTitle);
					});
				}
				else
				{
					this.propertyChangeFavor.gameObject.SetActive(false);
					this.propertyChangeFavorTitle.SetActive(false);
					this.propertyChangeAlertness.gameObject.SetActive(false);
					this.propertyChangeAlertnessTitle.SetActive(false);
					this.propertyChangeHappiness.gameObject.SetActive(false);
					this.propertyChangeHappinessTitle.SetActive(false);
				}
			}
		}

		// Token: 0x06007E25 RID: 32293 RVA: 0x003A97B4 File Offset: 0x003A79B4
		public static void RefreshTransferPreview(TransferItemPreviewDisplayData data, PropertyChange propertyChangeAlertness, PropertyChange propertyChangeFavor, PropertyChange propertyChangeHappiness, GameObject propertyChangeAlertnessTitle = null, GameObject propertyChangeFavorTitle = null, GameObject propertyChangeHappinessTitle = null)
		{
			sbyte originalAlertnessLevel = CharacterAlertnessData.GetLevel(data.OriginalAlertness);
			sbyte finalAlertnessLevel = CharacterAlertnessData.GetLevel(data.FinalAlertness);
			bool showAlertness = originalAlertnessLevel != finalAlertnessLevel;
			propertyChangeAlertness.gameObject.SetActive(showAlertness);
			if (propertyChangeAlertnessTitle != null)
			{
				propertyChangeAlertnessTitle.SetActive(showAlertness);
			}
			bool flag = showAlertness;
			if (flag)
			{
				string icon = CommonUtils.GetAlertnessIcon(0);
				string title = LanguageKey.LK_Alertness.Tr();
				string iconCurrent = CommonUtils.GetAlertnessIcon((int)originalAlertnessLevel);
				string valueCurrent = CommonUtils.GetAlertnessName((int)originalAlertnessLevel);
				string iconPreview = CommonUtils.GetAlertnessIcon((int)finalAlertnessLevel);
				string valuePreview = CommonUtils.GetAlertnessName((int)finalAlertnessLevel);
				propertyChangeAlertness.Set(icon, title, iconCurrent, valueCurrent, iconPreview, valuePreview);
			}
			short originalFavor = data.OriginalFavor;
			sbyte originalFavorLevel = FavorabilityType.GetFavorabilityType(originalFavor);
			short finalFavor = data.FinalFavor;
			sbyte finalFavorLevel = FavorabilityType.GetFavorabilityType(finalFavor);
			bool showFavor = originalFavorLevel != finalFavorLevel;
			propertyChangeFavor.gameObject.SetActive(showFavor);
			if (propertyChangeFavorTitle != null)
			{
				propertyChangeFavorTitle.SetActive(showFavor);
			}
			bool flag2 = showFavor;
			if (flag2)
			{
				string icon2 = CommonUtils.GetFavorabilityLevelIconName(0, false);
				string title2 = LanguageKey.LK_Favorability.Tr();
				string iconCurrent2 = CommonUtils.GetFavorabilityIconName(originalFavor, true);
				string valueCurrent2 = CommonUtils.GetFavorStringByLevel(originalFavorLevel);
				string iconPreview2 = CommonUtils.GetFavorabilityIconName(finalFavor, true);
				string valuePreview2 = CommonUtils.GetFavorStringByLevel(finalFavorLevel);
				propertyChangeFavor.Set(icon2, title2, iconCurrent2, valueCurrent2, iconPreview2, valuePreview2);
			}
			sbyte originalHappiness = data.OriginalHappiness;
			sbyte originalHappinessLevel = HappinessType.GetHappinessType(originalHappiness);
			sbyte finalHappiness = data.FinalHappiness;
			sbyte finalHappinessLevel = HappinessType.GetHappinessType(finalHappiness);
			bool showHappiness = originalHappinessLevel != finalHappinessLevel;
			propertyChangeHappiness.gameObject.SetActive(showHappiness);
			if (propertyChangeHappinessTitle != null)
			{
				propertyChangeHappinessTitle.SetActive(showHappiness);
			}
			bool flag3 = showHappiness;
			if (flag3)
			{
				string icon3 = CommonUtils.GetHappinessIconName(0);
				string title3 = LanguageKey.LK_Main_SummaryInfo_Happiness.Tr();
				string iconCurrent3 = CommonUtils.GetHappinessIconName(originalHappinessLevel);
				string valueCurrent3 = CommonUtils.GetHappinessString(originalHappinessLevel);
				string iconPreview3 = CommonUtils.GetHappinessIconName(finalHappinessLevel);
				string valuePreview3 = CommonUtils.GetHappinessString(finalHappinessLevel);
				propertyChangeHappiness.Set(icon3, title3, iconCurrent3, valueCurrent3, iconPreview3, valuePreview3);
			}
		}

		// Token: 0x06007E26 RID: 32294 RVA: 0x003A9990 File Offset: 0x003A7B90
		private void ClearTransferPage()
		{
			this.propertyChangeAlertness.gameObject.SetActive(false);
			this.propertyChangeFavor.gameObject.SetActive(false);
			this.propertyChangeHappiness.gameObject.SetActive(false);
			this.propertyChangeAlertnessTitle.SetActive(false);
			this.propertyChangeFavorTitle.SetActive(false);
			this.propertyChangeHappinessTitle.SetActive(false);
		}

		// Token: 0x06007E27 RID: 32295 RVA: 0x003A99FB File Offset: 0x003A7BFB
		private void OnSelectTransferItemChar(ArgumentBox argsBox)
		{
			argsBox.Get("CharacterId", out this._charId);
			this.RefreshCharDebt();
		}

		// Token: 0x06007E28 RID: 32296 RVA: 0x003A9A17 File Offset: 0x003A7C17
		private void RefreshCharDebt()
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, this._charId, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._targetCharacterDisplayData);
				this.RefreshTransferPage();
			});
		}

		// Token: 0x06007E29 RID: 32297 RVA: 0x003A9A34 File Offset: 0x003A7C34
		private void RefreshResourcePage(bool show, bool isGet)
		{
			this.resourcePage.gameObject.SetActive(show);
			bool flag = !show;
			if (!flag)
			{
				LanguageKey titleKey = isGet ? LanguageKey.LK_Item_Disassemble_Tip_Resource_Header : LanguageKey.LK_Item_Repair_Tip_Resource_Title;
				this.resourcePageTitle.text = LocalStringManager.Get(titleKey);
				for (int i = 0; i < this.resourcePropertyItems.Length; i++)
				{
					PropertyItem resourcePropertyItem = this.resourcePropertyItems[i];
					ResourceTypeItem config = Config.ResourceType.Instance[i];
					resourcePropertyItem.Set(config.Icon, config.Name, string.Empty, null, false);
					if (isGet)
					{
						int getValue = this._getResource.Get(i);
						resourcePropertyItem.gameObject.SetActive(getValue > 0);
						string value = CommonUtils.GetDisplayStringForNum(getValue, 100000).SetColor("pinkyellow");
						resourcePropertyItem.SetValue(value);
					}
					else
					{
						int needValue = this._needResource.Get(i);
						resourcePropertyItem.gameObject.SetActive(needValue > 0);
						int curValue = this._resource.Get(i);
						string curText = CommonUtils.GetDisplayStringForNum(curValue, 100000).SetColor("pinkyellow");
						string needColor = (curValue >= needValue) ? "brightblue" : "brightred";
						string needText = CommonUtils.GetDisplayStringForNum(needValue, 100000).SetColor(needColor);
						string value2 = LocalStringManager.GetFormat(LanguageKey.LK_Make_Resource_Require_Meet, curText, needText);
						resourcePropertyItem.SetValue(value2);
					}
				}
			}
		}

		// Token: 0x06007E2A RID: 32298 RVA: 0x003A9BC0 File Offset: 0x003A7DC0
		private void RefreshToolPage(bool show)
		{
			this.toolPage.gameObject.SetActive(show);
			bool flag = !show;
			if (!flag)
			{
				this._costToolList.Clear();
				Dictionary<ItemDisplayData, short> toolDict = this._toolDict;
				bool flag2 = toolDict != null && toolDict.Count > 0;
				if (flag2)
				{
					this._costToolList.AddRange(this._toolDict.Keys);
				}
				bool useEmptyTool = this._useEmptyTool;
				if (useEmptyTool)
				{
					this._costToolList.Insert(0, new ItemDisplayData
					{
						Key = this._emptyToolKey
					});
				}
				foreach (KeyValuePair<ItemDisplayData, short> keyValuePair in this._toolDict)
				{
					ItemDisplayData itemDisplayData;
					short num;
					keyValuePair.Deconstruct(out itemDisplayData, out num);
					ItemDisplayData itemData = itemDisplayData;
					short cost;
					bool flag3 = this._toolDict.TryGetValue(itemData, out cost);
					if (flag3)
					{
						string curText = itemData.Durability.ToString().SetColor("pinkyellow");
						string costText = string.Format("-{0}", cost).SetColor("brightred");
						string maxText = itemData.MaxDurability.ToString().SetColor("pinkyellow");
						itemData.DurabilityChange = curText + costText + "/" + maxText;
					}
					else
					{
						itemData.DurabilityChange = "-";
					}
				}
				this.toolListScroll.SetItemList(this._costToolList);
			}
		}

		// Token: 0x06007E2B RID: 32299 RVA: 0x003A9D4C File Offset: 0x003A7F4C
		private void RefreshGainPage(bool show, List<ItemDisplayData> itemList)
		{
			this.gainPage.gameObject.SetActive(show);
			bool flag = !show;
			if (!flag)
			{
				this.gainListScroll.SetItemList(itemList);
			}
		}

		// Token: 0x06007E2C RID: 32300 RVA: 0x003A9D84 File Offset: 0x003A7F84
		private void OnClickOperate()
		{
			switch (this._curOperationType)
			{
			case ItemOperationType.EItemOperationType.Repair:
				GEvent.OnEvent(UiEvents.ItemMultiplyOperationConfirm, null);
				return;
			case ItemOperationType.EItemOperationType.Disassemble:
				GEvent.OnEvent(UiEvents.ItemMultiplyOperationConfirm, null);
				return;
			case ItemOperationType.EItemOperationType.Transfer:
			{
				this._clothingChangeCharId = -1;
				bool flag = this._itemDict != null;
				if (flag)
				{
					foreach (ItemDisplayData itemData in this._itemDict.Keys)
					{
						bool flag2 = itemData.UsingType == ItemDisplayData.ItemUsingType.Equiped && itemData.RealKey.ItemType == 3;
						if (flag2)
						{
							this._clothingChangeCharId = itemData.OwnerCharId;
							break;
						}
					}
				}
				string title = LocalStringManager.Get(LanguageKey.LK_ItemMultiplyOperation_Transfer_Tip_Title);
				string content = LocalStringManager.Get(LanguageKey.LK_ItemMultiplyOperation_Transfer_Tip_Content);
				CommonUtils.ShowConfirmDialog(title, content, delegate
				{
					GEvent.OnEvent(UiEvents.ItemMultiplyOperationConfirm, null);
				}, null, EDialogType.None);
				return;
			}
			case ItemOperationType.EItemOperationType.Take:
			{
				string title2 = LocalStringManager.Get(LanguageKey.LK_ItemMultiplyOperation_Transfer_Tip_Title);
				string content2 = LocalStringManager.Get(LanguageKey.LK_ItemMultiplyOperation_Transfer_Tip_Content);
				CommonUtils.ShowConfirmDialog(title2, content2, delegate
				{
					GEvent.OnEvent(UiEvents.ItemMultiplyOperationConfirm, null);
				}, null, EDialogType.None);
				return;
			}
			case ItemOperationType.EItemOperationType.Discard:
				GEvent.OnEvent(UiEvents.ItemMultiplyOperationConfirm, null);
				return;
			case ItemOperationType.EItemOperationType.Feeding:
				GEvent.OnEvent(UiEvents.ItemMultiplyOperationConfirm, null);
				return;
			}
			throw new ArgumentOutOfRangeException();
		}

		// Token: 0x06007E2D RID: 32301 RVA: 0x003A9F30 File Offset: 0x003A8130
		private void TryExitMultiplyMode()
		{
			bool flag = this._itemDict.Count == 0;
			if (flag)
			{
				this.ExitMultiplyMode();
			}
			else
			{
				DialogCmd dialogCmd = new DialogCmd();
				dialogCmd.Type = 1;
				dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_Common_Attention);
				dialogCmd.Content = LocalStringManager.GetFormat(LanguageKey.LK_Item_Exit_Multiply, Array.Empty<object>());
				dialogCmd.Yes = new Action(this.ExitMultiplyMode);
				dialogCmd.No = null;
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
		}

		// Token: 0x06007E2E RID: 32302 RVA: 0x003A9FD0 File Offset: 0x003A81D0
		private void ExitMultiplyMode()
		{
			base.gameObject.SetActive(false);
			GEvent.OnEvent(UiEvents.ExitMultiplyOperation, null);
			bool flag = this._clothingChangeCharId >= 0;
			if (flag)
			{
				GEvent.OnEvent(UiEvents.OnChangeCharacterClothing, EasyPool.Get<ArgumentBox>().Set("CharacterId", this._clothingChangeCharId));
				this._clothingChangeCharId = -1;
			}
		}

		// Token: 0x06007E2F RID: 32303 RVA: 0x003AA038 File Offset: 0x003A8238
		private void CheckNeedEmpty(ItemOperationType.EItemOperationType curType)
		{
			if (curType > ItemOperationType.EItemOperationType.Discard)
			{
				if (curType != ItemOperationType.EItemOperationType.Feeding)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.scrollView.SetActive(this._feedingTargetList.Count > 0);
				this.emptyIcon.SetActive(this._feedingTargetList.Count <= 0);
			}
			else
			{
				this.scrollView.SetActive(this._currentSelectedItemCount > 0);
				this.emptyIcon.SetActive(this._currentSelectedItemCount <= 0);
			}
		}

		// Token: 0x06007E30 RID: 32304 RVA: 0x003AA0C6 File Offset: 0x003A82C6
		public void RefreshSelectedItem(int totalSelectedCount)
		{
			this._currentSelectedItemCount = totalSelectedCount;
			this.CheckNeedEmpty(this._curOperationType);
		}

		// Token: 0x04006021 RID: 24609
		[SerializeField]
		private CToggleGroup toggleGroup;

		// Token: 0x04006022 RID: 24610
		[SerializeField]
		private TextMeshProUGUI textOperationTypeTitle;

		// Token: 0x04006023 RID: 24611
		[SerializeField]
		private CButton buttonClose;

		// Token: 0x04006024 RID: 24612
		[SerializeField]
		private CButton buttonConfirm;

		// Token: 0x04006025 RID: 24613
		[SerializeField]
		private GameObject scrollView;

		// Token: 0x04006026 RID: 24614
		[SerializeField]
		private GameObject emptyIcon;

		// Token: 0x04006027 RID: 24615
		[Header("资源")]
		[SerializeField]
		private GameObject resourcePage;

		// Token: 0x04006028 RID: 24616
		[SerializeField]
		private TextMeshProUGUI resourcePageTitle;

		// Token: 0x04006029 RID: 24617
		[SerializeField]
		private PropertyItem[] resourcePropertyItems;

		// Token: 0x0400602A RID: 24618
		[Header("工具")]
		[SerializeField]
		private GameObject toolPage;

		// Token: 0x0400602B RID: 24619
		[SerializeField]
		private ItemListScroll toolListScroll;

		// Token: 0x0400602C RID: 24620
		[Header("获得")]
		[SerializeField]
		private GameObject gainPage;

		// Token: 0x0400602D RID: 24621
		[SerializeField]
		private ItemListScroll gainListScroll;

		// Token: 0x0400602E RID: 24622
		[Header("丢弃")]
		[SerializeField]
		private GameObject discardPage;

		// Token: 0x0400602F RID: 24623
		[SerializeField]
		private ItemListScroll discardListScroll;

		// Token: 0x04006030 RID: 24624
		[Header("投喂")]
		[SerializeField]
		private GameObject feedingPage;

		// Token: 0x04006031 RID: 24625
		[SerializeField]
		private PropertyItem propertyTame;

		// Token: 0x04006032 RID: 24626
		[SerializeField]
		private PropertyItem propertyDurability;

		// Token: 0x04006033 RID: 24627
		[SerializeField]
		private ItemListScroll carrierListScroll;

		// Token: 0x04006034 RID: 24628
		[SerializeField]
		private ItemListScroll materialListScroll;

		// Token: 0x04006035 RID: 24629
		[Header("投喂促织")]
		[SerializeField]
		private GameObject cricketFeedingPage;

		// Token: 0x04006036 RID: 24630
		[SerializeField]
		private PropertyItem propertySpirit;

		// Token: 0x04006037 RID: 24631
		[SerializeField]
		private ItemListScroll cricketListScroll;

		// Token: 0x04006038 RID: 24632
		[SerializeField]
		private ItemListScroll bloodDewListScroll;

		// Token: 0x04006039 RID: 24633
		[Header("转赠")]
		[SerializeField]
		private GameObject transferPage;

		// Token: 0x0400603A RID: 24634
		[SerializeField]
		private GameObject propertyChangeAlertnessTitle;

		// Token: 0x0400603B RID: 24635
		[SerializeField]
		private GameObject propertyChangeFavorTitle;

		// Token: 0x0400603C RID: 24636
		[SerializeField]
		private GameObject propertyChangeHappinessTitle;

		// Token: 0x0400603D RID: 24637
		[SerializeField]
		private PropertyChange propertyChangeAlertness;

		// Token: 0x0400603E RID: 24638
		[SerializeField]
		private PropertyChange propertyChangeFavor;

		// Token: 0x0400603F RID: 24639
		[SerializeField]
		private PropertyChange propertyChangeHappiness;

		// Token: 0x04006040 RID: 24640
		[SerializeField]
		private ItemListScroll transferListScroll;

		// Token: 0x04006041 RID: 24641
		[Header("百晓册入口控件")]
		[SerializeField]
		private QuickEncyclopedia quickEncyclopedia;

		// Token: 0x04006042 RID: 24642
		private ItemOperationType.EItemOperationType _curOperationType;

		// Token: 0x04006043 RID: 24643
		private ItemSourceType _itemSourceType;

		// Token: 0x04006044 RID: 24644
		private Dictionary<ItemDisplayData, short> _toolDict;

		// Token: 0x04006045 RID: 24645
		private bool _useEmptyTool;

		// Token: 0x04006046 RID: 24646
		private List<ItemDisplayData> _costToolList = new List<ItemDisplayData>();

		// Token: 0x04006047 RID: 24647
		private Dictionary<ItemDisplayData, int> _itemDict;

		// Token: 0x04006048 RID: 24648
		private List<ItemDisplayData> _itemOrderedList;

		// Token: 0x04006049 RID: 24649
		private ResourceInts _getResource;

		// Token: 0x0400604A RID: 24650
		private List<short> _chanceMaterialIdList;

		// Token: 0x0400604B RID: 24651
		private List<ItemDisplayData> _chanceMaterialList = new List<ItemDisplayData>();

		// Token: 0x0400604C RID: 24652
		private ResourceInts _needResource;

		// Token: 0x0400604D RID: 24653
		private ResourceInts _resource;

		// Token: 0x0400604E RID: 24654
		private List<ItemDisplayData> _feedingTargetList;

		// Token: 0x0400604F RID: 24655
		private ITradeableContent _feedingTarget;

		// Token: 0x04006050 RID: 24656
		private int _charId;

		// Token: 0x04006051 RID: 24657
		private bool _hasTeammate;

		// Token: 0x04006052 RID: 24658
		private bool _isFeedingCricket;

		// Token: 0x04006053 RID: 24659
		private bool _hasCarrierFeeding;

		// Token: 0x04006054 RID: 24660
		private bool _hasCricketFeeding;

		// Token: 0x04006055 RID: 24661
		private bool _processingToggleChange;

		// Token: 0x04006056 RID: 24662
		private ItemKey _emptyToolKey;

		// Token: 0x04006057 RID: 24663
		private CharacterDisplayData _taiwuCharacterDisplayData;

		// Token: 0x04006058 RID: 24664
		private CharacterDisplayData _targetCharacterDisplayData;

		// Token: 0x04006059 RID: 24665
		private int _currentSelectedItemCount;

		// Token: 0x0400605A RID: 24666
		private int _clothingChangeCharId = -1;

		// Token: 0x02001F93 RID: 8083
		private enum TogKey
		{
			// Token: 0x0400CDF7 RID: 52727
			Repair,
			// Token: 0x0400CDF8 RID: 52728
			Disassemble,
			// Token: 0x0400CDF9 RID: 52729
			Discard,
			// Token: 0x0400CDFA RID: 52730
			Feeding,
			// Token: 0x0400CDFB RID: 52731
			Transfer,
			// Token: 0x0400CDFC RID: 52732
			CricketFeeding,
			// Token: 0x0400CDFD RID: 52733
			Count
		}
	}
}
