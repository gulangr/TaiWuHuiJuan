using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Item.Apply;
using Game.Components.SortAndFilter.SelectCharacter;
using Game.Views.Select;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.TaiwuEvent.DisplayEvent;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Cricket
{
	// Token: 0x02000ABB RID: 2747
	public class ViewCricketBetting : UIBase
	{
		// Token: 0x060086D7 RID: 34519 RVA: 0x003EB460 File Offset: 0x003E9660
		public override void OnInit(ArgumentBox argsBox)
		{
			this.NeedDataListenerId = true;
			this._cricketBettingData = null;
			argsBox.Get<EventCricketBettingData>("CricketBettingData", out this._cricketBettingData);
			this.RefreshView();
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
		}

		// Token: 0x060086D8 RID: 34520 RVA: 0x003EB4BC File Offset: 0x003E96BC
		private void Awake()
		{
			this.wagerTypeTogGroup.OnActiveIndexChange += this.OnChipTogChanged;
			this.wagerTypeTogGroup.Init(0);
			this.autoBetToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnAutoBetToggleChanged));
			this.characterViewModeBtn.Init(1);
			this.characterViewModeBtn.OnActiveIndexChange += this.OnCharacterViewModeBtnClicked;
			this.characterScrollView.OnItemRender += this.OnRenderCharList;
			this.InitCharacterListSortAndFilter();
			this.characterListScroll.RowSelectedProvider = new Func<int, object, bool>(this.IsCharacterListRowSelected);
			this.characterListScroll.RowDisabledProvider = new Func<int, object, bool>(this.IsCharacterListRowDisabled);
			this.characterListScroll.Init<ViewCricketBetting.CharacterListRowData>(ViewCricketBetting.GenerateCharacterListColumns(), true, new Action<int, GameObject>(this.OnCharacterListRowRender), new Action<int, RowItem>(this.OnCharacterListRowClicked));
			this.itemListScroll.Init("CricketBettingItems", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnRenderItemList), null, ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Value, ViewCricketBetting.BettingItemColumnLayoutOptions, null, null);
			this.confirmBtn.ClearAndAddListener(new Action(this.OnConfirmBtnClicked));
			this.cancelBtn.ClearAndAddListener(new Action(this.OnCancelBtnClicked));
			this.rewardGroup.OnActiveIndexChange += this.OnRewardBtnClicked;
		}

		// Token: 0x060086D9 RID: 34521 RVA: 0x003EB61C File Offset: 0x003E981C
		private void OnDestroy()
		{
			this.wagerTypeTogGroup.OnActiveIndexChange -= this.OnChipTogChanged;
			this.autoBetToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnAutoBetToggleChanged));
			this.characterScrollView.OnItemRender -= this.OnRenderCharList;
			this.characterListScroll.OnRowClicked -= this.OnCharacterListRowClicked;
			this.rewardGroup.OnActiveIndexChange -= this.OnRewardBtnClicked;
		}

		// Token: 0x060086DA RID: 34522 RVA: 0x003EB6A8 File Offset: 0x003E98A8
		private void RefreshView()
		{
			this.RefreshBackground();
			this.RefreshCharacterViews();
			this._suppressAutoBetToggleCallback = true;
			this.autoBetToggle.isOn = this._cricketBettingData.AutoBet;
			this._suppressAutoBetToggleCallback = false;
			this.wagerTypeTogGroup.Set(0, false);
			this.SetChipToggleState(0);
			this.characterScrollView.UpdateData(0);
			InfinityScroll infinityScroll = this.characterScrollView;
			List<CharacterDisplayData> betCharacters = this._cricketBettingData.BetCharacters;
			infinityScroll.UpdateData((betCharacters != null) ? betCharacters.Count : 0);
			this.RefreshCharacterListRows();
			this.itemListScroll.SetItemList((from item in this._cricketBettingData.BetItems
			where item != null && item.Key != ItemKey.Invalid
			select item).ToList<ItemDisplayData>());
			this.itemListScroll.ReRender();
			this.confirmBtn.interactable = false;
			this.targetWager.SetData(Wager.Invalid, 0L);
			this.RefreshRewardList();
			bool flag = this._cricketBettingData.BetRewards.Count > 0;
			if (flag)
			{
				this.ApplyRewardSelection(0);
			}
			this.UpdateWager();
		}

		// Token: 0x060086DB RID: 34523 RVA: 0x003EB7CF File Offset: 0x003E99CF
		private void OnListenerIdReady()
		{
			base.DelayFrameCall(delegate
			{
				this.Element.ShowAfterRefresh();
			}, 1U);
		}

		// Token: 0x060086DC RID: 34524 RVA: 0x003EB7E8 File Offset: 0x003E99E8
		private void OnDisable()
		{
			this.ResetCharList();
			this._selectedToggle = -1;
			this._selectedReward = -1;
			this._selectedRewardMinValue = long.MaxValue;
			this._selectedWager = Wager.Invalid;
			this._selectedChipContent = null;
			this._selectedChipItem = null;
			this._selectingChipItem = null;
			this._characterListRows.Clear();
			this._filteredCharacterListRows.Clear();
			this._sortedCardCharacterList.Clear();
			this.rewardGroup.Clear();
		}

		// Token: 0x060086DD RID: 34525 RVA: 0x003EB86C File Offset: 0x003E9A6C
		private void Update()
		{
			bool flag = !CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
			if (!flag)
			{
				bool flag2 = !this.confirmBtn.interactable;
				if (!flag2)
				{
					this.confirmBtn.onClick.Invoke();
					this.confirmBtn.interactable = false;
				}
			}
		}

		// Token: 0x060086DE RID: 34526 RVA: 0x003EB8CA File Offset: 0x003E9ACA
		public override void QuickHide()
		{
			this.OnCancelBtnClicked();
		}

		// Token: 0x060086DF RID: 34527 RVA: 0x003EB8D4 File Offset: 0x003E9AD4
		private void RefreshBackground()
		{
			int backgroundType = (int)this.AnalysisBackgroundType();
			this.backgroundMain.SetTexture("ui9_tex_ccricketcombat_betting_bg_" + backgroundType.ToString());
			this.backgroundSub.SetTexture("ui9_tex_ccricketcombat_betting_table_" + backgroundType.ToString());
		}

		// Token: 0x060086E0 RID: 34528 RVA: 0x003EB923 File Offset: 0x003E9B23
		private void RefreshCharacterViews()
		{
			this.SetCharacterView(this.selfCharacterAvatar, this.selfCharacterName, this._cricketBettingData.SelfCharacter);
			this.SetCharacterView(this.targetCharacterAvatar, this.targetCharacterName, this._cricketBettingData.TargetCharacter);
		}

		// Token: 0x060086E1 RID: 34529 RVA: 0x003EB964 File Offset: 0x003E9B64
		private void SetCharacterView(Game.Components.Avatar.Avatar avatar, TextMeshProUGUI nameLabel, CharacterDisplayData displayData)
		{
			bool flag = displayData == null;
			if (flag)
			{
				avatar.ResetToBlank(false);
				nameLabel.text = string.Empty;
			}
			else
			{
				bool isTaiwu = displayData.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				avatar.Refresh(displayData.AvatarRelatedData, displayData.TemplateId);
				nameLabel.text = NameCenter.GetMonasticTitleOrDisplayName(displayData, isTaiwu);
			}
		}

		// Token: 0x060086E2 RID: 34530 RVA: 0x003EB9C8 File Offset: 0x003E9BC8
		private ViewCricketBetting.ECricketBettingBackgroundType AnalysisBackgroundType()
		{
			MapBlockData blockData = SingletonObject.getInstance<WorldMapModel>().PlayerAtBlock;
			EMapBlockType blockType = blockData.BlockType;
			if (!true)
			{
			}
			ViewCricketBetting.ECricketBettingBackgroundType result;
			if (blockType != EMapBlockType.City)
			{
				if (blockType != EMapBlockType.Sect)
				{
					EMapBlockSubType blockSubType = blockData.BlockSubType;
					if (!true)
					{
					}
					ViewCricketBetting.ECricketBettingBackgroundType ecricketBettingBackgroundType;
					if (blockSubType != EMapBlockSubType.TaiwuCun)
					{
						switch (blockSubType)
						{
						case EMapBlockSubType.Village:
							break;
						case EMapBlockSubType.Town:
							ecricketBettingBackgroundType = ViewCricketBetting.ECricketBettingBackgroundType.Town;
							goto IL_64;
						case EMapBlockSubType.WalledTown:
							ecricketBettingBackgroundType = ViewCricketBetting.ECricketBettingBackgroundType.WalledTown;
							goto IL_64;
						default:
							ecricketBettingBackgroundType = ViewCricketBetting.ECricketBettingBackgroundType.Wild;
							goto IL_64;
						}
					}
					ecricketBettingBackgroundType = ViewCricketBetting.ECricketBettingBackgroundType.Village;
					IL_64:
					if (!true)
					{
					}
					result = ecricketBettingBackgroundType;
				}
				else
				{
					result = ViewCricketBetting.ECricketBettingBackgroundType.Sect;
				}
			}
			else
			{
				result = ViewCricketBetting.ECricketBettingBackgroundType.City;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x060086E3 RID: 34531 RVA: 0x003EBA4C File Offset: 0x003E9C4C
		private void OnChipTogChanged(int togNew, int togOld)
		{
			bool flag = togNew == -1;
			if (!flag)
			{
				this.SetChipToggleState(togNew);
			}
		}

		// Token: 0x060086E4 RID: 34532 RVA: 0x003EBA6C File Offset: 0x003E9C6C
		private void OnAutoBetToggleChanged(bool isOn)
		{
			bool suppressAutoBetToggleCallback = this._suppressAutoBetToggleCallback;
			if (!suppressAutoBetToggleCallback)
			{
				TaiwuDomainMethod.Call.SetCricketBettingAutoBet(isOn);
				if (isOn)
				{
					this.TryAutoBetSelection();
				}
			}
		}

		// Token: 0x060086E5 RID: 34533 RVA: 0x003EBA99 File Offset: 0x003E9C99
		private void OnCharacterViewModeBtnClicked(int previousIndex, int currentIndex)
		{
			this._characterListViewMode = (ViewCricketBetting.ECharacterListViewMode)currentIndex;
			this.ApplyCharacterListViewMode();
		}

		// Token: 0x060086E6 RID: 34534 RVA: 0x003EBAAC File Offset: 0x003E9CAC
		private void OnChipItemConfirmed(int count)
		{
			bool flag = count > 0;
			if (flag)
			{
				this.UpdateChipItem(count);
			}
			else
			{
				this.OnChipItemCanceled();
			}
		}

		// Token: 0x060086E7 RID: 34535 RVA: 0x003EBAD3 File Offset: 0x003E9CD3
		private void OnChipItemCanceled()
		{
			this._selectingChipItem = null;
		}

		// Token: 0x060086E8 RID: 34536 RVA: 0x003EBAE0 File Offset: 0x003E9CE0
		private static string GetUsingOperationConfirmTip(ITradeableContent displayData)
		{
			ItemDisplayData itemDisplayData = displayData as ItemDisplayData;
			bool flag = itemDisplayData != null;
			string result;
			if (flag)
			{
				result = itemDisplayData.GetUsingOperationConfirmTip(ItemDisplayData.ItemUsingOperationType.Bet);
			}
			else
			{
				string usingTypeName = displayData.UsingType.ToString();
				string itemTypeName = (displayData.Key.ItemType == 10) ? LocalStringManager.Get(LanguageKey.LK_ItemType_10) : LocalStringManager.Get(LanguageKey.LK_Item);
				string operationName = LocalStringManager.Get(LanguageKey.LK_ItemUsingOperationType_Bet);
				result = LocalStringManager.GetFormat(LanguageKey.LK_ItemUsing_ConfirmTip, usingTypeName, itemTypeName, operationName).ColorReplace();
			}
			return result;
		}

		// Token: 0x060086E9 RID: 34537 RVA: 0x003EBB6C File Offset: 0x003E9D6C
		private void OnChipItemClicked(ITradeableContent content, RowItemLine rowItemLine)
		{
			ViewCricketBetting.<>c__DisplayClass59_0 CS$<>8__locals1 = new ViewCricketBetting.<>c__DisplayClass59_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.content = content;
			CS$<>8__locals1.rowItemLine = rowItemLine;
			bool flag = CS$<>8__locals1.content.UsingType != ItemDisplayData.ItemUsingType.Invalid;
			if (flag)
			{
				DialogCmd cmd = new DialogCmd
				{
					Title = LocalStringManager.Get(LanguageKey.LK_Common_Attention),
					Content = ViewCricketBetting.GetUsingOperationConfirmTip(CS$<>8__locals1.content),
					Type = 1,
					Yes = delegate()
					{
						CS$<>8__locals1.<>4__this.itemListScroll.HandleClickItem(CS$<>8__locals1.content, CS$<>8__locals1.rowItemLine, new Action<RowItemLine>(base.<OnChipItemClicked>g__HandleSelectItem|0));
					}
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				this.itemListScroll.HandleClickItem(CS$<>8__locals1.content, CS$<>8__locals1.rowItemLine, new Action<RowItemLine>(CS$<>8__locals1.<OnChipItemClicked>g__HandleSelectItem|0));
			}
		}

		// Token: 0x060086EA RID: 34538 RVA: 0x003EBC40 File Offset: 0x003E9E40
		private void OnRewardBtnClicked(int togNew, int togOld)
		{
			bool flag = togNew == -1;
			if (!flag)
			{
				this.ApplyRewardSelection(togNew);
			}
		}

		// Token: 0x060086EB RID: 34539 RVA: 0x003EBC60 File Offset: 0x003E9E60
		private void OnConfirmBtnClicked()
		{
			CricketWagerData enemyWagerData = this._cricketBettingData.BetRewards[this._selectedReward];
			long selfWagerValue = this.GetWagerValue(true);
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("EnemyId", this._cricketBettingData.TargetCharacter.CharacterId);
			argBox.Set<Wager>("SelfWager", this._selectedWager);
			argBox.Set<CricketWagerData>("EnemyWagerData", enemyWagerData);
			argBox.Set("SelfWagerValue", selfWagerValue);
			argBox.Set("EnemyWagerValue", enemyWagerData.MinWagerValue);
			argBox.Set("BettingSelectedReward", this._selectedReward);
			argBox.Set("FromBetting", true);
			argBox.Set("DoubleDamage", this._cricketBettingData.DoubleDamage);
			argBox.Set("OnlyNoInjuryCricket", this._cricketBettingData.OnlyNoInjuryCricket);
			argBox.Set("MinGrade", this._cricketBettingData.MinGrade);
			argBox.Set("MaxGrade", this._cricketBettingData.MaxGrade);
			UIElement.CricketCombat.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.CricketCombat, true);
		}

		// Token: 0x060086EC RID: 34540 RVA: 0x003EBD85 File Offset: 0x003E9F85
		private void OnCancelBtnClicked()
		{
			TaiwuEventDomainMethod.Call.SetCricketBettingResult(false, Wager.Invalid, this._selectedReward);
			UIManager.Instance.HideUI(this.Element);
		}

		// Token: 0x060086ED RID: 34541 RVA: 0x003EBDAC File Offset: 0x003E9FAC
		private Wager GetWagerByItemDisplayData(ITradeableContent data, int count = 1)
		{
			ItemKey key = data.Key;
			bool flag = key == ItemKey.Invalid;
			Wager result;
			if (flag)
			{
				result = Wager.Invalid;
			}
			else
			{
				result = (data.IsResource ? Wager.CreateResource(ItemTemplateHelper.GetMiscResourceType(key.ItemType, key.TemplateId), count) : Wager.CreateItem(key, count));
			}
			return result;
		}

		// Token: 0x060086EE RID: 34542 RVA: 0x003EBE04 File Offset: 0x003EA004
		private int GetMinAmountByMinValue()
		{
			long unitValue = this.GetWagerValue(false);
			bool flag = unitValue <= 0L;
			int result;
			if (flag)
			{
				result = 1;
			}
			else
			{
				result = (int)Math.Min(2147483647L, (this._selectedRewardMinValue + unitValue - 1L) / unitValue);
			}
			return result;
		}

		// Token: 0x060086EF RID: 34543 RVA: 0x003EBE48 File Offset: 0x003EA048
		private long GetWagerValue(bool isSelected)
		{
			ITradeableContent tradeableContent;
			if (!isSelected)
			{
				RowItemLine selectingChipItem = this._selectingChipItem;
				tradeableContent = ((selectingChipItem != null) ? selectingChipItem.Data : null);
			}
			else
			{
				tradeableContent = this._selectedChipContent;
			}
			ITradeableContent chipContent = tradeableContent;
			Wager wager = isSelected ? this._selectedWager : ((chipContent == null) ? Wager.Invalid : this.GetWagerByItemDisplayData(chipContent, 1));
			sbyte type = wager.Type;
			if (!true)
			{
			}
			long result;
			switch (type)
			{
			case -1:
				result = 0L;
				goto IL_F9;
			case 1:
			{
				long? num = (chipContent != null) ? new long?(chipContent.Value) : null;
				result = wager.CalcWagerValue(((num != null) ? new int?((int)num.GetValueOrDefault()) : null).GetValueOrDefault(), 0, 0, 0, -1, 0);
				goto IL_F9;
			}
			case 2:
				result = ((wager.CharId >= 0) ? this._cricketBettingData.BetCharacterValueMap[wager.CharId] : 0L);
				goto IL_F9;
			}
			result = wager.CalcWagerValue(0, 0, 0, 0, -1, 0);
			IL_F9:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x060086F0 RID: 34544 RVA: 0x003EBF5C File Offset: 0x003EA15C
		private int GetCharIdByIndex(int index)
		{
			bool flag = this._sortedCardCharacterList.Count > 0;
			int result;
			if (flag)
			{
				result = ((index >= 0 && index < this._sortedCardCharacterList.Count) ? this._sortedCardCharacterList[index].CharacterId : -1);
			}
			else
			{
				int num;
				if (index >= 0)
				{
					List<CharacterDisplayData> betCharacters = this._cricketBettingData.BetCharacters;
					if (index < ((betCharacters != null) ? betCharacters.Count : 0))
					{
						num = this._cricketBettingData.BetCharacters[index].CharacterId;
						goto IL_73;
					}
				}
				num = -1;
				IL_73:
				result = num;
			}
			return result;
		}

		// Token: 0x060086F1 RID: 34545 RVA: 0x003EBFE0 File Offset: 0x003EA1E0
		private void UpdateChipItem(int count)
		{
			this.ResetCharList();
			this._selectedChipItem = this._selectingChipItem;
			this._selectedChipContent = this._selectedChipItem.Data;
			this._selectedWager = this.GetWagerByItemDisplayData(this._selectedChipContent, count);
			this._selectingChipItem = null;
			this.UpdateWager();
			this.RefreshItemSelectionState();
			this.RefreshCharacterListSelectionState();
		}

		// Token: 0x060086F2 RID: 34546 RVA: 0x003EC044 File Offset: 0x003EA244
		private void AutoSelectChipItem(ITradeableContent content, int count)
		{
			this.wagerTypeTogGroup.Set(0, false);
			this.ResetCharList();
			this._selectedChipItem = null;
			this._selectedChipContent = content;
			this._selectedWager = this.GetWagerByItemDisplayData(content, count);
			this.UpdateWager();
			this.RefreshItemSelectionState();
			this.RefreshCharacterListSelectionState();
		}

		// Token: 0x060086F3 RID: 34547 RVA: 0x003EC098 File Offset: 0x003EA298
		private void TryAutoBetSelection()
		{
			bool flag = !this.autoBetToggle.isOn;
			if (!flag)
			{
				bool flag2 = this._selectedRewardMinValue == long.MaxValue;
				if (!flag2)
				{
					ITradeableContent content;
					int count;
					bool flag3 = !this.TryGetAutoBetCandidate(out content, out count);
					if (!flag3)
					{
						ITradeableContent selectedChipContent = this._selectedChipContent;
						ItemKey? itemKey = (selectedChipContent != null) ? new ItemKey?(selectedChipContent.Key) : null;
						ItemKey key = content.Key;
						bool flag4 = itemKey != null && (itemKey == null || itemKey.GetValueOrDefault() == key) && (this._selectedWager.Type != 0 || this._selectedWager.Count == count) && this.GetWagerValue(true) >= this._selectedRewardMinValue;
						if (!flag4)
						{
							this.AutoSelectChipItem(content, count);
						}
					}
				}
			}
		}

		// Token: 0x060086F4 RID: 34548 RVA: 0x003EC184 File Offset: 0x003EA384
		private bool TryGetAutoBetCandidate(out ITradeableContent content, out int count)
		{
			content = null;
			count = 0;
			ItemDisplayData bestResource = null;
			int bestResourceCount = int.MaxValue;
			long bestResourceTotalValue = long.MinValue;
			foreach (ItemDisplayData itemData in this._cricketBettingData.BetItems)
			{
				bool flag = itemData == null || itemData.Key == ItemKey.Invalid || !itemData.IsResource;
				if (!flag)
				{
					long unitValue = this.GetWagerByItemDisplayData(itemData, 1).CalcWagerValue(0, 0, 0, 0, -1, 0);
					bool flag2 = unitValue <= 0L;
					if (!flag2)
					{
						int requiredCount = (int)Math.Min(2147483647L, (this._selectedRewardMinValue + unitValue - 1L) / unitValue);
						bool flag3 = requiredCount > itemData.Amount;
						if (!flag3)
						{
							long totalValue = this.GetWagerByItemDisplayData(itemData, itemData.Amount).CalcWagerValue(0, 0, 0, 0, -1, 0);
							bool flag4 = bestResource == null || requiredCount < bestResourceCount || (requiredCount == bestResourceCount && totalValue > bestResourceTotalValue);
							if (flag4)
							{
								bestResource = itemData;
								bestResourceCount = requiredCount;
								bestResourceTotalValue = totalValue;
							}
						}
					}
				}
			}
			bool flag5 = bestResource != null;
			bool result;
			if (flag5)
			{
				content = bestResource;
				count = bestResourceCount;
				result = true;
			}
			else
			{
				ItemDisplayData bestItem = null;
				int bestItemValue = int.MaxValue;
				foreach (ItemDisplayData itemData2 in this._cricketBettingData.BetItems)
				{
					bool flag6 = itemData2 == null || itemData2.Key == ItemKey.Invalid || itemData2.IsResource;
					if (!flag6)
					{
						bool flag7 = itemData2.Value < this._selectedRewardMinValue;
						if (!flag7)
						{
							bool flag8 = bestItem == null || itemData2.Value < (long)bestItemValue;
							if (flag8)
							{
								bestItem = itemData2;
								bestItemValue = (int)itemData2.Value;
							}
						}
					}
				}
				bool flag9 = bestItem == null;
				if (flag9)
				{
					result = false;
				}
				else
				{
					content = bestItem;
					count = 1;
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060086F5 RID: 34549 RVA: 0x003EC3B8 File Offset: 0x003EA5B8
		private void RefreshRewardList()
		{
			this.rewardGroup.Clear();
			int rewardCount = this._cricketBettingData.BetRewards.Count;
			CommonUtils.PrepareEnoughChildren(this.rewardGroup.transform, this.rewardTemplate.gameObject, rewardCount, null);
			for (int index = 0; index < rewardCount; index++)
			{
				CricketBettingRewardItemView rewardView = this.rewardGroup.transform.GetChild(index).GetComponent<CricketBettingRewardItemView>();
				rewardView.SetData(index, this._cricketBettingData.BetRewards[index], new Action<int>(this.OnRewardItemSelected));
				this.rewardGroup.Add(rewardView.Toggle);
			}
			bool flag = rewardCount > 0;
			if (flag)
			{
				this.rewardGroup.Set(0, true);
			}
		}

		// Token: 0x060086F6 RID: 34550 RVA: 0x003EC484 File Offset: 0x003EA684
		private void ApplyRewardSelection(int index)
		{
			CricketWagerData reward = this._cricketBettingData.BetRewards[index];
			this._selectedReward = index;
			this._selectedRewardMinValue = reward.MinWagerValue;
			this.targetWager.gameObject.SetActive(true);
			this.targetWager.SetData(reward.Wager, reward.MinWagerValue);
			this.RefreshCharacterCardValues();
			this.RefreshCharacterListRows();
			this.itemListScroll.ReRender();
			this.TryAutoBetSelection();
			this.UpdateWager();
		}

		// Token: 0x060086F7 RID: 34551 RVA: 0x003EC509 File Offset: 0x003EA709
		private void OnRewardItemSelected(int index)
		{
			this.rewardGroup.Set(index, false);
		}

		// Token: 0x060086F8 RID: 34552 RVA: 0x003EC51C File Offset: 0x003EA71C
		private void UpdateWager()
		{
			long wagerValue = this.GetWagerValue(true);
			bool isValueEnough = this._selectedRewardMinValue <= wagerValue;
			bool hasSelectedWager = this._selectedWager.Type != -1;
			this.confirmBtn.interactable = isValueEnough;
			TooltipInvoker tipDisplayer = this.confirmBtn.GetComponent<TooltipInvoker>();
			bool flag = tipDisplayer != null;
			if (flag)
			{
				tipDisplayer.enabled = (!hasSelectedWager || !isValueEnough);
				bool flag2 = !hasSelectedWager;
				if (flag2)
				{
					tipDisplayer.Type = TipType.SingleDesc;
					TooltipInvoker tooltipInvoker = tipDisplayer;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					tipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_CricketBetting_Empty));
				}
				else
				{
					bool flag3 = !isValueEnough;
					if (flag3)
					{
						tipDisplayer.Type = TipType.SingleDesc;
						TooltipInvoker tooltipInvoker = tipDisplayer;
						if (tooltipInvoker.RuntimeParam == null)
						{
							tooltipInvoker.RuntimeParam = new ArgumentBox();
						}
						tipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_CricketBetting_Tip_ValueInsufficient));
					}
				}
			}
			this.selfWager.gameObject.SetActive(true);
			this.selfWager.SetData(this._selectedWager, wagerValue);
		}

		// Token: 0x060086F9 RID: 34553 RVA: 0x003EC648 File Offset: 0x003EA848
		private void SetChipToggleState(int togIndex)
		{
			bool isItemMode = togIndex == 0;
			this.itemSelectHolder.SetActive(isItemMode);
			this.characterSelectHolder.SetActive(!isItemMode);
			this._selectedToggle = togIndex;
			this.ApplyCharacterListViewMode();
		}

		// Token: 0x060086FA RID: 34554 RVA: 0x003EC686 File Offset: 0x003EA886
		private void ResetCharList()
		{
			this._selectedWager = Wager.Invalid;
			this.RefreshCharacterCardSelectionState();
			this.RefreshCharacterListSelectionState();
		}

		// Token: 0x060086FB RID: 34555 RVA: 0x003EC6A4 File Offset: 0x003EA8A4
		private void SelectCharacterWagerByIndex(int index)
		{
			bool flag = this._selectedToggle == 1;
			if (flag)
			{
				this._selectedChipItem = null;
				this._selectedChipContent = null;
			}
			this._selectedWager = Wager.CreateChar(this.GetCharIdByIndex(index));
			this.UpdateWager();
			this.RefreshItemSelectionState();
			this.RefreshCharacterCardSelectionState();
			this.RefreshCharacterListSelectionState();
		}

		// Token: 0x060086FC RID: 34556 RVA: 0x003EC700 File Offset: 0x003EA900
		private void ApplyCharacterListViewMode()
		{
			bool isCharacterMode = this._selectedToggle == 1;
			bool isListMode = this._characterListViewMode == ViewCricketBetting.ECharacterListViewMode.List;
			this.characterCardModeHolder.SetActive(isCharacterMode && !isListMode);
			this.characterListModeHolder.SetActive(isCharacterMode && isListMode);
		}

		// Token: 0x060086FD RID: 34557 RVA: 0x003EC748 File Offset: 0x003EA948
		private void OnCharacterListRowRender(int index, GameObject rowObject)
		{
			bool flag = index < 0 || index >= this._filteredCharacterListRows.Count;
			if (!flag)
			{
				RowItem rowItem = rowObject.GetComponent<RowItem>();
				TooltipInvoker tipDisplayer = rowItem.TipDisplayer;
				bool flag2 = tipDisplayer == null;
				if (!flag2)
				{
					ViewCricketBetting.CharacterListRowData rowData = this._filteredCharacterListRows[index];
					bool isDisabled = !this.CanSelectCharacter(rowData.CharacterId);
					bool flag3 = isDisabled;
					if (flag3)
					{
						tipDisplayer.Type = TipType.SingleDesc;
						tipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>();
						tipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_CricketBetting_ValueInsufficient));
						tipDisplayer.enabled = true;
					}
					else
					{
						bool flag4 = rowData.CharacterId >= 0;
						if (flag4)
						{
							tipDisplayer.Type = TipType.CharacterOnMapBlock;
							tipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("CharId", rowData.CharacterId);
							tipDisplayer.enabled = true;
						}
						else
						{
							tipDisplayer.enabled = false;
						}
					}
				}
			}
		}

		// Token: 0x060086FE RID: 34558 RVA: 0x003EC844 File Offset: 0x003EAA44
		private void OnCharacterListRowClicked(int index, RowItem rowItem)
		{
			bool flag = index < 0 || index >= this._filteredCharacterListRows.Count;
			if (!flag)
			{
				bool flag2 = !this.CanSelectCharacter(this._filteredCharacterListRows[index].CharacterId);
				if (!flag2)
				{
					this.SelectCharacterWagerByCharacterId(this._filteredCharacterListRows[index].CharacterId);
					bool isOn = this.autoBetToggle.isOn;
					if (isOn)
					{
						this._suppressAutoBetToggleCallback = true;
						this.autoBetToggle.isOn = false;
						this._suppressAutoBetToggleCallback = false;
						TaiwuDomainMethod.Call.SetCricketBettingAutoBet(false);
					}
				}
			}
		}

		// Token: 0x060086FF RID: 34559 RVA: 0x003EC8DC File Offset: 0x003EAADC
		private bool IsCharacterListRowSelected(int index, object rowData)
		{
			ViewCricketBetting.CharacterListRowData data = rowData as ViewCricketBetting.CharacterListRowData;
			bool flag = data == null;
			return !flag && this._selectedWager.Type == 2 && this._selectedWager.CharId == data.CharacterId;
		}

		// Token: 0x06008700 RID: 34560 RVA: 0x003EC928 File Offset: 0x003EAB28
		private bool IsCharacterListRowDisabled(int index, object rowData)
		{
			ViewCricketBetting.CharacterListRowData data = rowData as ViewCricketBetting.CharacterListRowData;
			return data == null || !this.CanSelectCharacter(data.CharacterId);
		}

		// Token: 0x06008701 RID: 34561 RVA: 0x003EC958 File Offset: 0x003EAB58
		private void RefreshCharacterListSelectionState()
		{
			bool flag = this._filteredCharacterListRows.Count > 0;
			if (flag)
			{
				this.characterListScroll.InfiniteScroll.ReRender();
			}
		}

		// Token: 0x06008702 RID: 34562 RVA: 0x003EC989 File Offset: 0x003EAB89
		private void RefreshItemSelectionState()
		{
			this.itemListScroll.ReRender();
		}

		// Token: 0x06008703 RID: 34563 RVA: 0x003EC998 File Offset: 0x003EAB98
		private void RefreshCharacterCardSelectionState()
		{
			this.characterScrollView.ReRender();
		}

		// Token: 0x06008704 RID: 34564 RVA: 0x003EC9A7 File Offset: 0x003EABA7
		private void RefreshCharacterCardValues()
		{
			this.characterScrollView.ReRender();
		}

		// Token: 0x06008705 RID: 34565 RVA: 0x003EC9B8 File Offset: 0x003EABB8
		private void RefreshCharacterListRows()
		{
			this._characterListRows.Clear();
			bool flag = this._cricketBettingData.BetCharacters != null;
			if (flag)
			{
				foreach (CharacterDisplayData displayData in this._cricketBettingData.BetCharacters)
				{
					long charValue = this._cricketBettingData.BetCharacterValueMap[displayData.CharacterId];
					this._characterListRows.Add(new ViewCricketBetting.CharacterListRowData
					{
						GeneralData = ViewCricketBetting.ConvertToGeneralScrollListData(displayData),
						Relation = ViewCricketBetting.GetHighestPriorityRelationText(displayData.RelationToTaiwu, displayData.IsSameFactionWithTaiwu),
						Identity = CommonUtils.GetOrganizationGradeString(displayData.OrgInfo, displayData.Gender, displayData.PhysiologicalAge, (int)displayData.TemplateId),
						WagerValue = charValue,
						Value = charValue.ToString().SetColor((charValue >= this._selectedRewardMinValue) ? "A7988A" : "9D3A20")
					});
				}
			}
			this.ApplyCharacterListSortAndFilter();
			this.RefreshCharacterListSelectionState();
		}

		// Token: 0x06008706 RID: 34566 RVA: 0x003ECAE4 File Offset: 0x003EACE4
		private static string GetHighestPriorityRelationText(ushort relationToTaiwu, bool isSameFaction)
		{
			bool flag = RelationType.ContainParentRelations(relationToTaiwu);
			string result;
			if (flag)
			{
				result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Parent);
			}
			else
			{
				bool flag2 = RelationType.ContainChildRelations(relationToTaiwu);
				if (flag2)
				{
					result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Child);
				}
				else
				{
					bool flag3 = RelationType.ContainBrotherOrSisterRelations(relationToTaiwu);
					if (flag3)
					{
						result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Bro);
					}
					else
					{
						bool flag4 = RelationType.HasRelation(relationToTaiwu, 1024);
						if (flag4)
						{
							result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Wife);
						}
						else
						{
							bool flag5 = RelationType.HasRelation(relationToTaiwu, 32768);
							if (flag5)
							{
								result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Enemy);
							}
							else
							{
								bool flag6 = RelationType.HasRelation(relationToTaiwu, 16384);
								if (flag6)
								{
									result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Adored);
								}
								else
								{
									bool flag7 = RelationType.HasRelation(relationToTaiwu, 2048) || RelationType.HasRelation(relationToTaiwu, 4096);
									if (flag7)
									{
										result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Mentor);
									}
									else
									{
										bool flag8 = RelationType.HasRelation(relationToTaiwu, 512);
										if (flag8)
										{
											result = LocalStringManager.Get(LanguageKey.LK_RelationShip_SwornBro);
										}
										else
										{
											bool flag9 = RelationType.HasRelation(relationToTaiwu, 8192);
											if (flag9)
											{
												result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Friend);
											}
											else if (isSameFaction)
											{
												result = LocalStringManager.Get(LanguageKey.LK_Faction);
											}
											else
											{
												result = "-";
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06008707 RID: 34567 RVA: 0x003ECC2A File Offset: 0x003EAE2A
		private static IEnumerable<ColumnDefinition> GenerateCharacterListColumns()
		{
			yield return ViewCricketBetting.CreateCharacterAvatarWithNameColumn();
			yield return ViewCricketBetting.CreateCharacterTextColumn(() => LanguageKey.LK_Favorability.Tr(), (ViewCricketBetting.CharacterListRowData data) => CommonUtils.GetFavorString(data.GeneralData.FavorabilityToTaiwu), 100f, 130f, 11);
			yield return ViewCricketBetting.CreateCharacterTextColumn(() => LanguageKey.LK_RelationShip.Tr(), (ViewCricketBetting.CharacterListRowData data) => data.Relation, 100f, 130f, 136);
			yield return ViewCricketBetting.CreateCharacterTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Identity.Tr(), (ViewCricketBetting.CharacterListRowData data) => data.Identity, 120f, 150f, 1);
			yield return ViewCricketBetting.CreateCharacterTextColumn(() => LanguageKey.LK_ItemValue.Tr(), (ViewCricketBetting.CharacterListRowData data) => data.Value, 80f, 110f, 5);
			yield break;
		}

		// Token: 0x06008708 RID: 34568 RVA: 0x003ECC34 File Offset: 0x003EAE34
		private static ColumnDefinition CreateCharacterAvatarWithNameColumn()
		{
			ColumnDefinition<ViewCricketBetting.CharacterListRowData, AvatarWithNameCellData> columnDefinition = new ColumnDefinition<ViewCricketBetting.CharacterListRowData, AvatarWithNameCellData>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 220f,
				FlexibleWidth = 0f,
				PreferredWidth = 260f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Char_Name.Tr());
			columnDefinition.CellDataGenerator = ((ViewCricketBetting.CharacterListRowData data) => AvatarWithNameCellData.FromCharacterDisplayDataForGeneralScrollList(data.GeneralData, data.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId, null, null));
			columnDefinition.SortId = 0;
			return columnDefinition;
		}

		// Token: 0x06008709 RID: 34569 RVA: 0x003ECCD8 File Offset: 0x003EAED8
		private static ColumnDefinition CreateCharacterTextColumn(Func<string> headerLabel, Func<ViewCricketBetting.CharacterListRowData, string> valueGetter, float minWidth, float preferredWidth, short sortId = -1)
		{
			return new ColumnDefinition<ViewCricketBetting.CharacterListRowData, string>
			{
				LayoutOption = new LayoutOption
				{
					MinWidth = minWidth,
					FlexibleWidth = 0f,
					PreferredWidth = preferredWidth,
					Priority = 1
				},
				TableHeadLabel = headerLabel,
				CellDataGenerator = valueGetter,
				SortId = sortId
			};
		}

		// Token: 0x0600870A RID: 34570 RVA: 0x003ECD38 File Offset: 0x003EAF38
		private void InitCharacterListSortAndFilter()
		{
			bool flag = this.characterSortAndFilter == null;
			if (!flag)
			{
				this._characterSortAndFilterController = new SelectCharacterSortAndFilterController(this.characterSortAndFilter, new List<ESelectCharacterFilterMenuId>
				{
					ESelectCharacterFilterMenuId.Gender,
					ESelectCharacterFilterMenuId.BehaviorType,
					ESelectCharacterFilterMenuId.Relation,
					ESelectCharacterFilterMenuId.AdoreRelation,
					ESelectCharacterFilterMenuId.EnemyRelation,
					ESelectCharacterFilterMenuId.Organization,
					ESelectCharacterFilterMenuId.Sect
				}, new List<short>
				{
					0,
					11,
					136,
					1,
					5
				}, false);
				this.characterSortAndFilter.SetUpToggleGroupHotKey(this.Element, 0);
				this._characterSortAndFilterController.Init(new Action(this.OnCharacterSortAndFilterChanged), "SelectCharacter");
				this.characterListScroll.SetSortController(this._characterSortAndFilterController);
			}
		}

		// Token: 0x0600870B RID: 34571 RVA: 0x003ECE1E File Offset: 0x003EB01E
		private void OnCharacterSortAndFilterChanged()
		{
			this.ApplyCharacterListSortAndFilter();
		}

		// Token: 0x0600870C RID: 34572 RVA: 0x003ECE28 File Offset: 0x003EB028
		private void ApplyCharacterListSortAndFilter()
		{
			ViewCricketBetting.<>c__DisplayClass94_0 CS$<>8__locals1 = new ViewCricketBetting.<>c__DisplayClass94_0();
			IEnumerable<ViewCricketBetting.CharacterListRowData> rows = this._characterListRows;
			ViewCricketBetting.<>c__DisplayClass94_0 CS$<>8__locals2 = CS$<>8__locals1;
			SelectCharacterSortAndFilterController characterSortAndFilterController = this._characterSortAndFilterController;
			CS$<>8__locals2.filter = ((characterSortAndFilterController != null) ? characterSortAndFilterController.GenerateFilter() : null);
			bool flag = CS$<>8__locals1.filter != null;
			if (flag)
			{
				rows = from row in rows
				where CS$<>8__locals1.filter(row)
				select row;
			}
			this._filteredCharacterListRows.Clear();
			this._filteredCharacterListRows.AddRange(rows);
			ViewCricketBetting.<>c__DisplayClass94_0 CS$<>8__locals3 = CS$<>8__locals1;
			SelectCharacterSortAndFilterController characterSortAndFilterController2 = this._characterSortAndFilterController;
			CS$<>8__locals3.comparer = ((characterSortAndFilterController2 != null) ? characterSortAndFilterController2.GenerateComparer(this._filteredCharacterListRows.Cast<ISelectCharacterData>().ToList<ISelectCharacterData>()) : null);
			bool flag2 = CS$<>8__locals1.comparer != null;
			if (flag2)
			{
				this._filteredCharacterListRows.Sort((ViewCricketBetting.CharacterListRowData x, ViewCricketBetting.CharacterListRowData y) => CS$<>8__locals1.comparer(x, y));
			}
			SelectCharacterSortAndFilterController characterSortAndFilterController3 = this._characterSortAndFilterController;
			if (characterSortAndFilterController3 != null)
			{
				characterSortAndFilterController3.AfterFilter(this._characterListRows);
			}
			this.characterListScroll.SetData<ViewCricketBetting.CharacterListRowData>(this._filteredCharacterListRows, -1);
			this._sortedCardCharacterList.Clear();
			bool flag3 = this._cricketBettingData.BetCharacters != null;
			if (flag3)
			{
				Dictionary<int, CharacterDisplayData> charDict = new Dictionary<int, CharacterDisplayData>();
				foreach (CharacterDisplayData c in this._cricketBettingData.BetCharacters)
				{
					charDict[c.CharacterId] = c;
				}
				foreach (ViewCricketBetting.CharacterListRowData row2 in this._filteredCharacterListRows)
				{
					CharacterDisplayData data;
					bool flag4 = charDict.TryGetValue(row2.CharacterId, out data);
					if (flag4)
					{
						this._sortedCardCharacterList.Add(data);
					}
				}
			}
			this.characterScrollView.UpdateData(this._sortedCardCharacterList.Count);
		}

		// Token: 0x0600870D RID: 34573 RVA: 0x003ED010 File Offset: 0x003EB210
		private void SelectCharacterWagerByCharacterId(int characterId)
		{
			bool flag = this._selectedToggle == 1;
			if (flag)
			{
				this._selectedChipItem = null;
				this._selectedChipContent = null;
			}
			this._selectedWager = Wager.CreateChar(characterId);
			this.UpdateWager();
			this.RefreshItemSelectionState();
			this.RefreshCharacterListSelectionState();
		}

		// Token: 0x0600870E RID: 34574 RVA: 0x003ED05C File Offset: 0x003EB25C
		private bool IsItemSelected(ITradeableContent content)
		{
			bool flag = this._selectedChipContent == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				sbyte type = this._selectedWager.Type;
				result = ((type == 1 || type == 0) && content.Key == this._selectedChipContent.Key && content.CharacterId == this._selectedChipContent.CharacterId);
			}
			return result;
		}

		// Token: 0x0600870F RID: 34575 RVA: 0x003ED0C4 File Offset: 0x003EB2C4
		private static CharacterDisplayDataForGeneralScrollList ConvertToGeneralScrollListData(CharacterDisplayData data)
		{
			return new CharacterDisplayDataForGeneralScrollList
			{
				CharacterId = data.CharacterId,
				CharacterTemplateId = data.TemplateId,
				NameData = new NameRelatedData
				{
					CharTemplateId = data.TemplateId,
					Gender = data.Gender,
					MonkType = data.MonkType,
					FullName = data.FullName,
					OrgTemplateId = data.OrgInfo.OrgTemplateId,
					OrgGrade = data.OrgInfo.Grade,
					MonasticTitle = data.MonasticTitle,
					CustomDisplayNameId = data.CustomDisplayNameId,
					NickNameId = data.NickNameId,
					ExtraNameTextTemplateId = data.ExtraNameTextTemplateId
				},
				Health = data.Health,
				MaxLeftHealth = data.LeftMaxHealth,
				Charm = data.Charm,
				BehaviorType = data.BehaviorType,
				Fame = data.FameType,
				Happiness = data.Happiness,
				FavorabilityToTaiwu = data.FavorabilityToTaiwu,
				Personalities = data.Personalities,
				AttackMedal = data.AttackMedal,
				DefenceMedal = data.DefenceMedal,
				WisdomMedal = data.WisdomMedal,
				ConsummateLevel = data.ConsummateLevel,
				Gender = data.Gender,
				PhysiologicalAge = data.PhysiologicalAge,
				CreatingType = data.CreatingType,
				AvatarRelatedData = data.AvatarRelatedData,
				OrgInfo = data.OrgInfo,
				RelationToTaiwu = data.RelationToTaiwu,
				RelationFromTaiwu = data.RelationFromTaiwu,
				IsSameFactionWithTaiwu = data.IsSameFactionWithTaiwu
			};
		}

		// Token: 0x06008710 RID: 34576 RVA: 0x003ED280 File Offset: 0x003EB480
		private void OnRenderItemList(ITradeableContent content, RowItemLine rowItemLine)
		{
			ItemDisplayData itemData = (ItemDisplayData)content;
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			rowItemLine.SetSelected(this.IsItemSelected(itemData));
			bool canSelect = (ViewCricketBetting.IsItemWagerable(itemData) && itemData.Value >= this._selectedRewardMinValue) || (itemData.IsResource && this.GetWagerByItemDisplayData(itemData, itemData.Amount).CalcWagerValue(0, 0, 0, 0, -1, 0) >= this._selectedRewardMinValue);
			canSelect = (canSelect && !itemData.IsLocked);
			bool isWagerable = ViewCricketBetting.IsItemWagerable(itemData) || itemData.IsResource;
			bool hasValue = !itemData.IsResource || itemData.Amount > 0;
			long itemValue = itemData.IsResource ? this.GetWagerByItemDisplayData(itemData, itemData.Amount).CalcWagerValue(0, 0, 0, 0, -1, 0) : itemData.Value;
			bool valueInsufficient = isWagerable && hasValue && itemValue < this._selectedRewardMinValue;
			bool flag = canSelect;
			if (flag)
			{
				rowItemLine.SetLocked(false);
				rowItemLine.SetInteractable(true, true);
				rowItemLine.SetDisabled(false);
				rowItemLine.SetClickEvent(delegate
				{
					this.OnChipItemClicked(content, rowItemLine);
				});
			}
			else
			{
				rowItemLine.SetLocked(false);
				rowItemLine.SetInteractable(false, true);
				rowItemLine.SetDisabled(true);
				bool flag2 = valueInsufficient;
				if (flag2)
				{
					rowItemLine.TipDisplayer.Type = TipType.SingleDesc;
					rowItemLine.TipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>();
					rowItemLine.TipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_CricketBetting_ValueInsufficient));
					rowItemLine.TipDisplayer.enabled = true;
				}
			}
			bool flag3 = canSelect && itemData.IsResource;
			if (flag3)
			{
				string taiwuName = (this._cricketBettingData.SelfCharacter == null) ? string.Empty : NameCenter.GetNameByDisplayData(this._cricketBettingData.SelfCharacter, true, false);
				RowItemLine.SetResourceTip(itemData, rowItemLine.TipDisplayer, taiwuName, true, false);
			}
		}

		// Token: 0x06008711 RID: 34577 RVA: 0x003ED4E8 File Offset: 0x003EB6E8
		private void OnRenderCharList(int index, GameObject charObj)
		{
			CricketBettingCharacterCard characterCard = charObj.GetComponent<CricketBettingCharacterCard>();
			characterCard.BindClick(delegate
			{
				this.OnCharacterCardClicked(index);
			});
			int charId = this.GetCharIdByIndex(index);
			long value;
			this._cricketBettingData.BetCharacterValueMap.TryGetValue(charId, out value);
			bool isTeammate = SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuTeamCharacter(charId);
			CharacterDisplayData charDisplayData = this.GetCharacterDisplayDataByIndex(index);
			characterCard.SetCharacter(charDisplayData, value, this._selectedRewardMinValue, isTeammate);
			characterCard.SetSelected(this.IsCharacterCardSelected(index));
		}

		// Token: 0x06008712 RID: 34578 RVA: 0x003ED588 File Offset: 0x003EB788
		private void OnCharacterCardClicked(int index)
		{
			int charId = this.GetCharIdByIndex(index);
			bool flag = !this.CanSelectCharacter(charId);
			if (!flag)
			{
				this.SelectCharacterWagerByIndex(index);
				bool flag2 = this.autoBetToggle.isOn && this._selectedWager.Type != -1 && this.GetWagerValue(true) < this._selectedRewardMinValue;
				if (flag2)
				{
					this.TryAutoBetSelection();
				}
			}
		}

		// Token: 0x06008713 RID: 34579 RVA: 0x003ED5F0 File Offset: 0x003EB7F0
		private bool IsCharacterCardSelected(int index)
		{
			return this._selectedWager.Type == 2 && this._selectedWager.CharId == this.GetCharIdByIndex(index);
		}

		// Token: 0x06008714 RID: 34580 RVA: 0x003ED628 File Offset: 0x003EB828
		private CharacterDisplayData GetCharacterDisplayDataByIndex(int index)
		{
			bool flag = this._sortedCardCharacterList.Count > 0;
			CharacterDisplayData result;
			if (flag)
			{
				result = ((index >= 0 && index < this._sortedCardCharacterList.Count) ? this._sortedCardCharacterList[index] : null);
			}
			else
			{
				bool flag2 = index < 0 || this._cricketBettingData.BetCharacters == null || index >= this._cricketBettingData.BetCharacters.Count;
				if (flag2)
				{
					result = null;
				}
				else
				{
					result = this._cricketBettingData.BetCharacters[index];
				}
			}
			return result;
		}

		// Token: 0x06008715 RID: 34581 RVA: 0x003ED6B4 File Offset: 0x003EB8B4
		private bool CanSelectCharacter(int characterId)
		{
			long value;
			return characterId >= 0 && this._cricketBettingData.BetCharacterValueMap.TryGetValue(characterId, out value) && value >= this._selectedRewardMinValue;
		}

		// Token: 0x06008716 RID: 34582 RVA: 0x003ED6F0 File Offset: 0x003EB8F0
		private static bool IsItemWagerable(ItemDisplayData itemData)
		{
			return ItemTemplateHelper.IsWagerable(itemData.Key.ItemType, itemData.Key.TemplateId);
		}

		// Token: 0x0400679A RID: 26522
		[SerializeField]
		private Game.Components.Avatar.Avatar selfCharacterAvatar;

		// Token: 0x0400679B RID: 26523
		[SerializeField]
		private Game.Components.Avatar.Avatar targetCharacterAvatar;

		// Token: 0x0400679C RID: 26524
		[SerializeField]
		private TextMeshProUGUI selfCharacterName;

		// Token: 0x0400679D RID: 26525
		[SerializeField]
		private TextMeshProUGUI targetCharacterName;

		// Token: 0x0400679E RID: 26526
		[SerializeField]
		private CRawImage backgroundMain;

		// Token: 0x0400679F RID: 26527
		[SerializeField]
		private CRawImage backgroundSub;

		// Token: 0x040067A0 RID: 26528
		[SerializeField]
		private CToggle autoBetToggle;

		// Token: 0x040067A1 RID: 26529
		[SerializeField]
		private InfinityScroll characterScrollView;

		// Token: 0x040067A2 RID: 26530
		[SerializeField]
		private ItemListScroll itemListScroll;

		// Token: 0x040067A3 RID: 26531
		[SerializeField]
		private CToggleGroup wagerTypeTogGroup;

		// Token: 0x040067A4 RID: 26532
		[SerializeField]
		private GameObject itemSelectHolder;

		// Token: 0x040067A5 RID: 26533
		[SerializeField]
		private GameObject characterSelectHolder;

		// Token: 0x040067A6 RID: 26534
		[SerializeField]
		private CToggleGroup characterViewModeBtn;

		// Token: 0x040067A7 RID: 26535
		[SerializeField]
		private GameObject characterCardModeHolder;

		// Token: 0x040067A8 RID: 26536
		[SerializeField]
		private GameObject characterListModeHolder;

		// Token: 0x040067A9 RID: 26537
		[SerializeField]
		private ListStyleGeneralScroll characterListScroll;

		// Token: 0x040067AA RID: 26538
		[SerializeField]
		private SortAndFilter characterSortAndFilter;

		// Token: 0x040067AB RID: 26539
		[SerializeField]
		private CButton cancelBtn;

		// Token: 0x040067AC RID: 26540
		[SerializeField]
		private CButton confirmBtn;

		// Token: 0x040067AD RID: 26541
		[SerializeField]
		private CricketBettingWagerView selfWager;

		// Token: 0x040067AE RID: 26542
		[SerializeField]
		private CricketBettingWagerView targetWager;

		// Token: 0x040067AF RID: 26543
		[SerializeField]
		private CToggleGroup rewardGroup;

		// Token: 0x040067B0 RID: 26544
		[SerializeField]
		private CricketBettingRewardItemView rewardTemplate;

		// Token: 0x040067B1 RID: 26545
		private EventCricketBettingData _cricketBettingData;

		// Token: 0x040067B2 RID: 26546
		private int _selectedToggle = -1;

		// Token: 0x040067B3 RID: 26547
		private int _selectedReward = -1;

		// Token: 0x040067B4 RID: 26548
		private long _selectedRewardMinValue = long.MaxValue;

		// Token: 0x040067B5 RID: 26549
		private Wager _selectedWager = Wager.Invalid;

		// Token: 0x040067B6 RID: 26550
		private ITradeableContent _selectedChipContent;

		// Token: 0x040067B7 RID: 26551
		private RowItemLine _selectedChipItem;

		// Token: 0x040067B8 RID: 26552
		private RowItemLine _selectingChipItem;

		// Token: 0x040067B9 RID: 26553
		private readonly List<ViewCricketBetting.CharacterListRowData> _characterListRows = new List<ViewCricketBetting.CharacterListRowData>();

		// Token: 0x040067BA RID: 26554
		private readonly List<ViewCricketBetting.CharacterListRowData> _filteredCharacterListRows = new List<ViewCricketBetting.CharacterListRowData>();

		// Token: 0x040067BB RID: 26555
		private readonly List<CharacterDisplayData> _sortedCardCharacterList = new List<CharacterDisplayData>();

		// Token: 0x040067BC RID: 26556
		private bool _suppressAutoBetToggleCallback;

		// Token: 0x040067BD RID: 26557
		private ViewCricketBetting.ECharacterListViewMode _characterListViewMode = ViewCricketBetting.ECharacterListViewMode.Card;

		// Token: 0x040067BE RID: 26558
		private SelectCharacterSortAndFilterController _characterSortAndFilterController;

		// Token: 0x040067BF RID: 26559
		private static readonly Dictionary<ItemListScroll.EColumnType, LayoutOption> BettingItemColumnLayoutOptions = new Dictionary<ItemListScroll.EColumnType, LayoutOption>
		{
			{
				ItemListScroll.EColumnType.IconAndName,
				new LayoutOption(200f, 1f, 500f, 1)
			},
			{
				ItemListScroll.EColumnType.Amount,
				new LayoutOption(100f, 1f, 156f, 1)
			},
			{
				ItemListScroll.EColumnType.Type,
				new LayoutOption(100f, 1f, 156f, 1)
			},
			{
				ItemListScroll.EColumnType.Value,
				new LayoutOption(100f, 1f, 156f, 1)
			}
		};

		// Token: 0x0200207D RID: 8317
		private enum ECharacterListViewMode
		{
			// Token: 0x0400D13C RID: 53564
			Card,
			// Token: 0x0400D13D RID: 53565
			List
		}

		// Token: 0x0200207E RID: 8318
		private class CharacterListRowData : ISelectCharacterData, ISelectCharacterWagerValueRow
		{
			// Token: 0x17001962 RID: 6498
			// (get) Token: 0x0600F75C RID: 63324 RVA: 0x0062946C File Offset: 0x0062766C
			// (set) Token: 0x0600F75D RID: 63325 RVA: 0x00629474 File Offset: 0x00627674
			public long WagerValue { get; set; }

			// Token: 0x17001963 RID: 6499
			// (get) Token: 0x0600F75E RID: 63326 RVA: 0x0062947D File Offset: 0x0062767D
			public int CharacterId
			{
				get
				{
					CharacterDisplayDataForGeneralScrollList generalData = this.GeneralData;
					return (generalData != null) ? generalData.CharacterId : -1;
				}
			}

			// Token: 0x0600F75F RID: 63327 RVA: 0x00629494 File Offset: 0x00627694
			public CharacterDisplayDataForGeneralScrollList GetGeneralScrollListData()
			{
				return this.GeneralData;
			}

			// Token: 0x0400D13E RID: 53566
			public CharacterDisplayDataForGeneralScrollList GeneralData;

			// Token: 0x0400D13F RID: 53567
			public string Relation;

			// Token: 0x0400D140 RID: 53568
			public string Identity;

			// Token: 0x0400D141 RID: 53569
			public string Value;
		}

		// Token: 0x0200207F RID: 8319
		private enum ECricketBettingBackgroundType
		{
			// Token: 0x0400D144 RID: 53572
			Wild,
			// Token: 0x0400D145 RID: 53573
			City,
			// Token: 0x0400D146 RID: 53574
			Sect,
			// Token: 0x0400D147 RID: 53575
			WalledTown,
			// Token: 0x0400D148 RID: 53576
			Town,
			// Token: 0x0400D149 RID: 53577
			Village
		}
	}
}
