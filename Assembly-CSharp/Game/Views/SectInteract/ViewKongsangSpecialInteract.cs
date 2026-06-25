using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Item.Apply;
using Game.Views.Select;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Story;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x020009B2 RID: 2482
	public class ViewKongsangSpecialInteract : UIBase
	{
		// Token: 0x17000D64 RID: 3428
		// (get) Token: 0x06007827 RID: 30759 RVA: 0x0037E543 File Offset: 0x0037C743
		private int SelectWugKingCount
		{
			get
			{
				return this._selectedWugKingDict.Sum((KeyValuePair<ItemKey, int> v) => v.Value);
			}
		}

		// Token: 0x17000D65 RID: 3429
		// (get) Token: 0x06007828 RID: 30760 RVA: 0x0037E56F File Offset: 0x0037C76F
		private int TaiwuCharId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x06007829 RID: 30761 RVA: 0x0037E57C File Offset: 0x0037C77C
		private void Awake()
		{
			this.characterScroll.OnItemRender += this.OnItemRender;
			ItemListScroll.EColumnType columnType = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount;
			this.itemScroll.Init("kongsang_special_interact_item_scroll_view", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnRenderItem), new Action<ITradeableContent, RowItemLine>(this.OnClickItem), columnType, null, null, null);
			this.itemScroll.CustomAmountDataGenerator = new Func<ITradeableContent, string>(this.AmountCellDataGenerator);
			PoolManager.SetSrcObject("KongsangSpecialInteractWugKingPrefab", this.dropItemPrefab.gameObject);
			this.wugKingFilterDropdown.Setup(new FilterDropdownConfig
			{
				MenuBarLabel = LanguageKey.LK_Resource_Choosy_ItemMenu,
				ItemConfigs = new List<FilterDropdownItemConfig>
				{
					new FilterDropdownItemConfig(LanguageKey.LK_KongsangSpecialInteract_WugKing_0),
					new FilterDropdownItemConfig(LanguageKey.LK_KongsangSpecialInteract_WugKing_1),
					new FilterDropdownItemConfig(LanguageKey.LK_KongsangSpecialInteract_WugKing_2),
					new FilterDropdownItemConfig(LanguageKey.LK_KongsangSpecialInteract_WugKing_3),
					new FilterDropdownItemConfig(LanguageKey.LK_KongsangSpecialInteract_WugKing_4),
					new FilterDropdownItemConfig(LanguageKey.LK_KongsangSpecialInteract_WugKing_5),
					new FilterDropdownItemConfig(LanguageKey.LK_KongsangSpecialInteract_WugKing_6),
					new FilterDropdownItemConfig(LanguageKey.LK_KongsangSpecialInteract_WugKing_7)
				},
				DefaultSelectedIndices = new HashSet<int>(),
				IsMultiSelect = true
			});
			this.wugKingFilterDropdown.OnSelectionChanged = delegate()
			{
				this.SetFilter(this.wugKingFilterDropdown.GetSelectedIndices());
			};
		}

		// Token: 0x0600782A RID: 30762 RVA: 0x0037E710 File Offset: 0x0037C910
		private string AmountCellDataGenerator(ITradeableContent itemData)
		{
			return this._selectedWugKingDict.Any((KeyValuePair<ItemKey, int> t) => t.Key.Equals(itemData.RealKey)) ? string.Format("{0}/{1}", this._selectedWugKingDict[itemData.RealKey], itemData.Amount) : itemData.Amount.ToString();
		}

		// Token: 0x0600782B RID: 30763 RVA: 0x0037E794 File Offset: 0x0037C994
		private void OnDestroy()
		{
			PoolManager.RemoveData("KongsangSpecialInteractWugKingPrefab");
		}

		// Token: 0x0600782C RID: 30764 RVA: 0x0037E7A2 File Offset: 0x0037C9A2
		public override void OnInit(ArgumentBox argsBox)
		{
			this.NeedDataListenerId = true;
			this.NeedWaitData = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
		}

		// Token: 0x0600782D RID: 30765 RVA: 0x0037E7DA File Offset: 0x0037C9DA
		private void OnListenerIdReady()
		{
			this.RequestData();
		}

		// Token: 0x0600782E RID: 30766 RVA: 0x0037E7E4 File Offset: 0x0037C9E4
		private void RequestData()
		{
			this._selectedWugKingDict.Clear();
			this.ClearDropWugKingItems();
			this.mask.gameObject.SetActive(false);
			this.wugKingFilterDropdown.UnSelectAll(true);
			this._selectedCharacterIds.Clear();
			this.eff_kongsang_luhuodanci.gameObject.SetActive(false);
			this.eff_kongsang_kuosanjinghua.gameObject.SetActive(false);
			this.stoveSkeleton.AnimationState.SetAnimation(0, ViewKongsangSpecialInteract.stoveSkeletonAnims[1], true);
			this.GetInventoryWugKing();
			this.GetCurAreaValidCharacters();
		}

		// Token: 0x0600782F RID: 30767 RVA: 0x0037E87C File Offset: 0x0037CA7C
		private void GetInventoryWugKing()
		{
			CharacterDomainMethod.AsyncCall.GetAllInventoryItems(null, this.TaiwuCharId, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._inventoryWugkingList);
				this._filteredWugkingList.Clear();
				for (int i = this._inventoryWugkingList.Count - 1; i >= 0; i--)
				{
					bool flag = !EatingItems.IsWugKing(this._inventoryWugkingList[i].Key);
					if (flag)
					{
						this._inventoryWugkingList.RemoveAt(i);
					}
					else
					{
						this._filteredWugkingList.Add(this._inventoryWugkingList[i]);
					}
				}
				this.itemScroll.SetItemList(this._filteredWugkingList);
			});
		}

		// Token: 0x06007830 RID: 30768 RVA: 0x0037E898 File Offset: 0x0037CA98
		private void GetCurAreaValidCharacters()
		{
			StoryDomainMethod.AsyncCall.GetCurAreaValidCharactersForTripodVessel(null, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._targetCharacterDisplayDatas);
				this._characterDisplayDataDict.Clear();
				using (List<CharacterDisplayData>.Enumerator enumerator = this._targetCharacterDisplayDatas.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						CharacterDisplayData data = enumerator.Current;
						this._characterDisplayDataDict[data.CharacterId] = data;
						CharacterDomainMethod.AsyncCall.GetCharacterTemporaryFeaturesExpireDate(null, new IntPair(data.CharacterId, 738), delegate(int offset, RawDataPool dataPool)
						{
							int expireDate = -1;
							Serializer.Deserialize(dataPool, offset, ref expireDate);
							this._characterFeatureRemainTimeDict[data.CharacterId] = new ValueTuple<int, int>(data.DarkAshCounter.Total, (expireDate > -1) ? (expireDate - SingletonObject.getInstance<BasicGameData>().CurrDate) : 0);
						});
					}
				}
				this.characterScroll.SetDataCount(this._selectedCharacterIds.Count);
				this._chacacterItems.Clear();
				this.UpdateSelectionUI();
				bool flag = !this.Element.Ready;
				if (flag)
				{
					this.Element.ShowAfterRefresh();
				}
			});
		}

		// Token: 0x06007831 RID: 30769 RVA: 0x0037E8B0 File Offset: 0x0037CAB0
		private void SetFilter(IReadOnlyCollection<int> selectedIndices)
		{
			this._filteredWugkingList.Clear();
			foreach (ItemDisplayData item in this._inventoryWugkingList)
			{
				int itemIndex = (int)(item.Key.TemplateId - 423 - 1);
				bool flag = selectedIndices.Contains(itemIndex);
				if (flag)
				{
					this._filteredWugkingList.Add(item);
				}
			}
			this.itemScroll.SetItemList(this._filteredWugkingList);
		}

		// Token: 0x06007832 RID: 30770 RVA: 0x0037E950 File Offset: 0x0037CB50
		public void OnRenderItem(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, false);
			bool isSelected = this._selectedWugKingDict.Any((KeyValuePair<ItemKey, int> t) => t.Key.Equals(itemData.RealKey));
			rowItemLine.SetSelected(isSelected);
			int selectedCount = this.SelectWugKingCount;
			int requiredCount = this._selectedCharacterIds.Count;
			int remainingCount = requiredCount - selectedCount;
			bool isDisabled = remainingCount <= 0 && !isSelected && !itemData.IsLocked;
			rowItemLine.SetInteractable(!isDisabled, true);
			rowItemLine.SetDisabled(isDisabled);
			bool flag = isSelected;
			if (flag)
			{
				rowItemLine.SetDataForContainerDirect(1, string.Format("{0}/{1}", this._selectedWugKingDict[itemData.RealKey], itemData.Amount));
			}
			else
			{
				rowItemLine.SetDataForContainerDirect(1, itemData.Amount.ToString());
			}
		}

		// Token: 0x06007833 RID: 30771 RVA: 0x0037EA5C File Offset: 0x0037CC5C
		public void OnClickItem(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			ViewKongsangSpecialInteract.<>c__DisplayClass43_0 CS$<>8__locals1 = new ViewKongsangSpecialInteract.<>c__DisplayClass43_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.itemData = itemData;
			bool flag = this._selectedWugKingDict.ContainsKey(CS$<>8__locals1.itemData.Key);
			if (flag)
			{
				ValueTuple<ItemKey, int> pair = new ValueTuple<ItemKey, int>(CS$<>8__locals1.itemData.Key, -this._selectedWugKingDict[CS$<>8__locals1.itemData.Key]);
				this._selectedWugKingDict.Remove(CS$<>8__locals1.itemData.Key);
				CS$<>8__locals1.<OnClickItem>g__OnRefresh|0(pair);
			}
			else
			{
				sbyte itemType = CS$<>8__locals1.itemData.RealKey.ItemType;
				sbyte b = itemType;
				if (b != 8)
				{
					throw new Exception(string.Format("UnHandled ItemType: {0}", CS$<>8__locals1.itemData.RealKey.ItemType));
				}
				int selectedCount = this.SelectWugKingCount;
				int requiredCount = this._selectedCharacterIds.Count;
				int maxCount = requiredCount - selectedCount;
				this.itemScroll.SetItemToSelectCountMode(rowItemLine, delegate(int count)
				{
					ValueTuple<ItemKey, int> pair2 = CS$<>8__locals1.<>4__this.SetSelectedItem(CS$<>8__locals1.itemData.Clone(count));
					base.<OnClickItem>g__OnRefresh|0(pair2);
				}, delegate
				{
					ValueTuple<ItemKey, int> pair2 = CS$<>8__locals1.<>4__this.SetSelectedItem(null);
					base.<OnClickItem>g__OnRefresh|0(pair2);
				}, 0, maxCount, 1, null, false, null, false);
			}
		}

		// Token: 0x06007834 RID: 30772 RVA: 0x0037EB74 File Offset: 0x0037CD74
		[return: TupleElementNames(new string[]
		{
			"data",
			"count"
		})]
		private ValueTuple<ItemKey, int> SetSelectedItem(ITradeableContent itemData)
		{
			this.itemScroll.SelectedItem = itemData;
			bool flag = itemData == null;
			ValueTuple<ItemKey, int> result;
			if (flag)
			{
				result = new ValueTuple<ItemKey, int>(ItemKey.Invalid, -1);
			}
			else
			{
				int originalAmount = 0;
				bool flag2 = this._selectedWugKingDict.ContainsKey(itemData.Key);
				if (flag2)
				{
					originalAmount = this._selectedWugKingDict[itemData.Key];
				}
				this._selectedWugKingDict[itemData.Key] = itemData.Amount;
				result = new ValueTuple<ItemKey, int>(itemData.Key, itemData.Amount - originalAmount);
			}
			return result;
		}

		// Token: 0x06007835 RID: 30773 RVA: 0x0037EBFF File Offset: 0x0037CDFF
		private void OnContentChange([TupleElementNames(new string[]
		{
			"data",
			"count"
		})] List<ValueTuple<ItemKey, int>> changeList = null)
		{
			this.RefreshDropItem(changeList);
			this.UpdateSelectionUI();
		}

		// Token: 0x06007836 RID: 30774 RVA: 0x0037EC14 File Offset: 0x0037CE14
		private void RefreshDropItem([TupleElementNames(new string[]
		{
			"data",
			"count"
		})] List<ValueTuple<ItemKey, int>> changeList = null)
		{
			bool flag = changeList != null;
			if (flag)
			{
				foreach (ValueTuple<ItemKey, int> valueTuple in changeList)
				{
					ItemKey itemKey = valueTuple.Item1;
					int amount = valueTuple.Item2;
					Queue<GameObject> queue;
					bool flag2 = !this._dropItemDict.TryGetValue(itemKey, out queue);
					if (flag2)
					{
						queue = new Queue<GameObject>();
						this._dropItemDict[itemKey] = queue;
					}
					int count = Mathf.Clamp(amount, 0, 10);
					bool flag3 = count > 0;
					if (flag3)
					{
						int totalCount = this._dropItemDict.Sum((KeyValuePair<ItemKey, Queue<GameObject>> p) => p.Value.Count);
						int times = count;
						while (times > 0)
						{
							times--;
							bool flag4 = totalCount >= 50;
							if (flag4)
							{
								break;
							}
							GameObject item = PoolManager.GetObject("KongsangSpecialInteractWugKingPrefab");
							queue.Enqueue(item);
							RectTransform rect = item.transform as RectTransform;
							rect.SetParent(this.dropItemRoot);
							rect.localScale = new Vector3(1f, 1f, 1f);
							CImage image = item.GetComponent<CImage>();
							string icon = ItemTemplateHelper.GetIcon(itemKey.ItemType, itemKey.TemplateId);
							image.SetSprite(icon, false, null);
							bool flag5 = totalCount == 0;
							if (flag5)
							{
								rect.anchoredPosition = Vector3.zero;
							}
							else
							{
								float radius = Random.value * 150f;
								float angle = Random.value * 2f * 3.1415927f;
								Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
								rect.anchoredPosition = offset;
							}
							totalCount++;
						}
					}
					else
					{
						bool flag6 = count < 0;
						if (flag6)
						{
							int times2 = -count;
							while (times2 > 0)
							{
								times2--;
								GameObject item2 = queue.Dequeue();
								ViewKongsangSpecialInteract.ReturnItem(item2);
							}
						}
					}
				}
			}
			foreach (KeyValuePair<ItemKey, Queue<GameObject>> keyValuePair in this._dropItemDict)
			{
				ItemKey itemKey3;
				Queue<GameObject> queue3;
				keyValuePair.Deconstruct(out itemKey3, out queue3);
				ItemKey itemKey2 = itemKey3;
				Queue<GameObject> queue2 = queue3;
				bool needHide = !this._selectedWugKingDict.ContainsKey(itemKey2);
				bool flag7 = needHide;
				if (flag7)
				{
					while (queue2.Count > 0)
					{
						GameObject item3 = queue2.Dequeue();
						ViewKongsangSpecialInteract.ReturnItem(item3);
					}
				}
			}
		}

		// Token: 0x06007837 RID: 30775 RVA: 0x0037EF00 File Offset: 0x0037D100
		private static void ReturnItem(GameObject item)
		{
			item.GetComponent<DropWugKingItem>().eff_kongsang_guchong.gameObject.SetActive(false);
			item.transform.SetParent(null);
			PoolManager.Destroy("KongsangSpecialInteractWugKingPrefab", item);
		}

		// Token: 0x06007838 RID: 30776 RVA: 0x0037EF34 File Offset: 0x0037D134
		private void ClearDropWugKingItems()
		{
			foreach (KeyValuePair<ItemKey, Queue<GameObject>> keyValuePair in this._dropItemDict)
			{
				ItemKey itemKey;
				Queue<GameObject> queue;
				keyValuePair.Deconstruct(out itemKey, out queue);
				Queue<GameObject> v = queue;
				while (v.Count > 0)
				{
					GameObject item = v.Dequeue();
					ViewKongsangSpecialInteract.ReturnItem(item);
				}
			}
		}

		// Token: 0x06007839 RID: 30777 RVA: 0x0037EFBC File Offset: 0x0037D1BC
		protected override void OnClick(Transform btn)
		{
			bool flag = btn.name == "CloseBtn";
			if (flag)
			{
				this.QuickHide();
			}
			else
			{
				bool flag2 = btn.name == "SelectCharBtn";
				if (flag2)
				{
					this.ShowSelectCharacterUI();
				}
				else
				{
					bool flag3 = btn.name == "ConfirmBtn";
					if (flag3)
					{
						DialogCmd dialogCmd2 = new DialogCmd();
						dialogCmd2.Type = 1;
						dialogCmd2.Title = LocalStringManager.Get(LanguageKey.LK_KongsangSpecialInteract_Confirm_Title);
						dialogCmd2.Content = LanguageKey.LK_KongsangSpecialInteract_Confirm_Content.TrFormat(this.SelectWugKingCount);
						dialogCmd2.Yes = delegate()
						{
							List<ItemKeyAndCount> wugKingList = new List<ItemKeyAndCount>();
							foreach (KeyValuePair<ItemKey, int> keyValuePair in this._selectedWugKingDict)
							{
								ItemKey itemKey;
								int num;
								keyValuePair.Deconstruct(out itemKey, out num);
								ItemKey i = itemKey;
								int v = num;
								wugKingList.Add(new ItemKeyAndCount
								{
									ItemKey = i,
									Count = v
								});
							}
							StoryDomainMethod.Call.ApplyKongsangSpecialInteract(this._selectedCharacterIds, wugKingList);
							base.StartCoroutine(this.PlayFireEff());
							base.DelayCall(delegate
							{
								this.RequestData();
							}, 1.6f);
						};
						dialogCmd2.No = delegate()
						{
						};
						DialogCmd dialogCmd = dialogCmd2;
						UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
						UIManager.Instance.MaskUI(UIElement.Dialog);
					}
				}
			}
		}

		// Token: 0x0600783A RID: 30778 RVA: 0x0037F0C4 File Offset: 0x0037D2C4
		public override void QuickHide()
		{
			bool activeSelf = this.mask.gameObject.activeSelf;
			if (!activeSelf)
			{
				TaiwuEventDomainMethod.Call.TriggerListener("ShowSectMainStorySpecialInteract", true);
				base.QuickHide();
			}
		}

		// Token: 0x0600783B RID: 30779 RVA: 0x0037F0FC File Offset: 0x0037D2FC
		private void UpdateSelectionUI()
		{
			int selectedCount = this.SelectWugKingCount;
			this.wugKingCountText.text = ((selectedCount < this._selectedCharacterIds.Count) ? string.Format("{0}/{1}", selectedCount.ToString().SetColor("brightred"), this._selectedCharacterIds.Count).ColorReplace() : string.Format("<color=#brightyellow>{0}</color>/{1}", selectedCount, this._selectedCharacterIds.Count).ColorReplace());
			this.characterCountText.text = LanguageKey.LK_KongsanSpecialInteract_CharacterCount.TrFormat(string.Format("<color=#brightyellow>{0}</color>/{1}", this._selectedCharacterIds.Count, this._targetCharacterDisplayDatas.Count)).ColorReplace();
			bool enabled = selectedCount >= this._selectedCharacterIds.Count && this._selectedCharacterIds.Count > 0;
			this.confirmBtn.interactable = enabled;
			TooltipInvoker mouseTip = this.confirmBtn.GetComponent<TooltipInvoker>();
			mouseTip.enabled = !enabled;
			bool flag = !enabled;
			if (flag)
			{
				bool flag2 = this._selectedCharacterIds.Count <= 0;
				if (flag2)
				{
					mouseTip.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_KongsangSpecialInteract_Confirm_NoCharacter);
				}
				else
				{
					mouseTip.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_KongsangSpecialInteract_Confirm_WugKingNotEnough);
				}
			}
			this.itemScroll.gameObject.SetActive(this._selectedCharacterIds.Count > 0);
			this.empty.gameObject.SetActive(this._selectedCharacterIds.Count <= 0);
		}

		// Token: 0x0600783C RID: 30780 RVA: 0x0037F298 File Offset: 0x0037D498
		public void ShowSelectCharacterUI()
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForGeneralScrollListBatch(this, (from v in this._targetCharacterDisplayDatas
			select v.CharacterId).ToList<int>(), delegate(int offset, RawDataPool pool)
			{
				List<CharacterDisplayDataForGeneralScrollList> displayData = new List<CharacterDisplayDataForGeneralScrollList>();
				Serializer.Deserialize(pool, offset, ref displayData);
				List<ISelectCharacterData> selectList = new List<ISelectCharacterData>();
				foreach (CharacterDisplayDataForGeneralScrollList item in displayData)
				{
					ValueTuple<int, int> remainTime;
					bool flag = this._characterFeatureRemainTimeDict.TryGetValue(item.CharacterId, out remainTime) && remainTime.Item2 > 0;
					if (flag)
					{
						item.TripodVesselProtectRemainTime = remainTime.Item2;
					}
					selectList.Add(new BasicSelectCharacterDataAdapter(item));
				}
				CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.DarkAah);
				config.CustomColumnGenerator = new Dictionary<ESelectCharacterSubPage, Func<IEnumerable<ColumnDefinition>>>();
				config.CustomColumnGenerator[ESelectCharacterSubPage.DarkAah] = new Func<IEnumerable<ColumnDefinition>>(this.GenerateKongsangColumns);
				config.InteractionMode = ESelectCharacterInteractionMode.Instant;
				config.SelectionMode = ESelectCharacterSelectionMode.Multiple;
				config.TargetCount = this._targetCharacterDisplayDatas.Count;
				config.InitialSelectedCharacterIds = this._selectedCharacterIds;
				config.CustomTextGenerator = new Func<IReadOnlyList<int>, string>(this.GenerateWugkingHintText);
				ArgumentBox args = EasyPool.Get<ArgumentBox>();
				args.SetObject("SelectCharacterConfig", config);
				args.SetObject("SelectCharacterDataList", selectList);
				args.SetObject("SelectCharacterCallback", new SelectCharacterCallback(this.OnSelectCharacter));
				UIElement.SelectChar.SetOnInitArgs(args);
				UIManager.Instance.MaskUI(UIElement.SelectChar);
			});
		}

		// Token: 0x0600783D RID: 30781 RVA: 0x0037F2E8 File Offset: 0x0037D4E8
		private string GenerateWugkingHintText(IReadOnlyList<int> list)
		{
			LanguageKey id = LanguageKey.LK_Kongsang_Select_WugkingAmount;
			List<ItemDisplayData> inventoryWugkingList = this._inventoryWugkingList;
			int num;
			if (inventoryWugkingList == null)
			{
				num = 0;
			}
			else
			{
				num = inventoryWugkingList.Sum((ItemDisplayData v) => v.Amount);
			}
			return LocalStringManager.GetFormat(id, num);
		}

		// Token: 0x0600783E RID: 30782 RVA: 0x0037F33A File Offset: 0x0037D53A
		private IEnumerable<ColumnDefinition> GenerateKongsangColumns()
		{
			ColumnDefinition<ISelectCharacterData, SingleTextWithTipData> columnDefinition = new ColumnDefinition<ISelectCharacterData, SingleTextWithTipData>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => CharacterFeature.Instance[216].Name);
			columnDefinition.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				CharacterDisplayDataForGeneralScrollList generalData = data.GetGeneralScrollListData();
				ValueTuple<int, int> remainTime;
				bool flag = this._characterFeatureRemainTimeDict.TryGetValue(generalData.CharacterId, out remainTime) && remainTime.Item1 > 0;
				SingleTextWithTipData result;
				if (flag)
				{
					result = new SingleTextWithTipData
					{
						content = remainTime.Item1.ToString() + "<SpName=ui_sp_icon_month_0>",
						tipAction = delegate(TooltipInvoker tip)
						{
							tip.Type = TipType.Feature;
							if (tip.RuntimeParam == null)
							{
								tip.RuntimeParam = new ArgumentBox();
							}
							tip.RuntimeParam.Set("FeatureId", 216);
							tip.RuntimeParam.Set("CharacterId", generalData.CharacterId);
						}
					};
				}
				else
				{
					bool flag2 = this._characterDisplayDataDict[generalData.CharacterId].FeatureIds.Contains(680);
					if (flag2)
					{
						result = new SingleTextWithTipData
						{
							content = CharacterFeature.Instance[680].Name
						};
					}
					else
					{
						bool flag3 = this._characterDisplayDataDict[generalData.CharacterId].FeatureIds.Contains(215) || this._characterDisplayDataDict[generalData.CharacterId].FeatureIds.Contains(211);
						if (flag3)
						{
							result = new SingleTextWithTipData
							{
								content = LanguageKey.LK_KongsanSpecialInteract_Infection.Tr()
							};
						}
						else
						{
							bool flag4 = this._characterDisplayDataDict[generalData.CharacterId].FeatureIds.Contains(726);
							if (flag4)
							{
								result = new SingleTextWithTipData
								{
									content = CharacterFeature.Instance[726].Name
								};
							}
							else
							{
								result = new SingleTextWithTipData
								{
									content = "-"
								};
							}
						}
					}
				}
				return result;
			};
			columnDefinition.SortId = 201;
			yield return columnDefinition;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition2 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition2.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition2.TableHeadLabel = (() => LanguageKey.LK_KongsangSpecialInteract_Title.Tr());
			columnDefinition2.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				CharacterDisplayDataForGeneralScrollList generalData = data.GetGeneralScrollListData();
				ValueTuple<int, int> remainTime;
				bool flag = this._characterFeatureRemainTimeDict.TryGetValue(generalData.CharacterId, out remainTime) && remainTime.Item2 > 0;
				string result;
				if (flag)
				{
					result = remainTime.Item2.ToString() + "<SpName=ui_sp_icon_month_0>";
				}
				else
				{
					result = "-";
				}
				return result;
			};
			columnDefinition2.SortId = 202;
			yield return columnDefinition2;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition3 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition3.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition3.TableHeadLabel = (() => LanguageKey.LK_Main_SummaryInfo_Organization.Tr());
			columnDefinition3.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				CharacterDisplayDataForGeneralScrollList generalData = data.GetGeneralScrollListData();
				string text;
				return CommonUtils.TryGetCharacterSpecialGradeName((int)generalData.CharacterTemplateId, out text) ? "-" : SingletonObject.getInstance<WorldMapModel>().GetSettlementName(generalData.OrgInfo);
			};
			columnDefinition3.SortId = 203;
			yield return columnDefinition3;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition4 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition4.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition4.TableHeadLabel = (() => LanguageKey.LK_Main_SummaryInfo_Identity.Tr());
			columnDefinition4.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				CharacterDisplayDataForGeneralScrollList generalData = data.GetGeneralScrollListData();
				return CommonUtils.GetIdentityStringWithSpecialCharacterConfig((int)generalData.CharacterTemplateId, generalData.OrgInfo, generalData.Gender, generalData.PhysiologicalAge, false);
			};
			columnDefinition4.SortId = 1;
			yield return columnDefinition4;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition5 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition5.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition5.TableHeadLabel = (() => LanguageKey.LK_RelationShip.Tr());
			columnDefinition5.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				CharacterDisplayDataForGeneralScrollList generalData = data.GetGeneralScrollListData();
				bool flag = generalData == null;
				string result;
				if (flag)
				{
					result = "-";
				}
				else
				{
					bool flag2 = generalData.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
					if (flag2)
					{
						result = "-";
					}
					else
					{
						result = CommonUtils.GetHighestPriorityRelationText(generalData.RelationToTaiwu, generalData.IsSameFactionWithTaiwu);
					}
				}
				return result;
			};
			columnDefinition5.SortId = 136;
			yield return columnDefinition5;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition6 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition6.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition6.TableHeadLabel = (() => LanguageKey.LK_Favorability.Tr());
			columnDefinition6.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				CharacterDisplayDataForGeneralScrollList generalData = data.GetGeneralScrollListData();
				return CommonUtils.GetFavorStringByInteracted(generalData.FavorabilityToTaiwu, generalData.IsInteractedWithTaiwu);
			};
			columnDefinition6.SortId = 11;
			yield return columnDefinition6;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition7 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition7.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition7.TableHeadLabel = (() => LanguageKey.LK_Main_SummaryInfo_Fame.Tr());
			columnDefinition7.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				CharacterDisplayDataForGeneralScrollList generalData = data.GetGeneralScrollListData();
				return CommonUtils.GetFameString(FameType.GetFameType(generalData.Fame));
			};
			columnDefinition7.SortId = 59;
			yield return columnDefinition7;
			yield break;
		}

		// Token: 0x0600783F RID: 30783 RVA: 0x0037F34C File Offset: 0x0037D54C
		private void OnSelectCharacter(List<int> selectedCharacterIds)
		{
			this._selectedCharacterIds.Clear();
			this._selectedCharacterIds.AddRange(selectedCharacterIds);
			this.characterScroll.SetDataCount(this._selectedCharacterIds.Count);
			this._chacacterItems.Clear();
			this.UpdateSelectionUI();
			this.itemScroll.ReRender();
		}

		// Token: 0x06007840 RID: 30784 RVA: 0x0037F3AC File Offset: 0x0037D5AC
		private void OnItemRender(int index, GameObject gameObject)
		{
			KongsangChacacterItem chacacterItem = gameObject.GetComponent<KongsangChacacterItem>();
			chacacterItem.Set(this._characterDisplayDataDict[this._selectedCharacterIds[index]], this._characterFeatureRemainTimeDict[this._selectedCharacterIds[index]].Item2);
		}

		// Token: 0x06007841 RID: 30785 RVA: 0x0037F3FC File Offset: 0x0037D5FC
		public void OnCharacterDeleteBtnClicked(int characterId)
		{
			this._selectedCharacterIds.Remove(characterId);
			this.characterScroll.SetDataCount(this._selectedCharacterIds.Count);
			this._chacacterItems.Clear();
			this.UpdateSelectionUI();
			this.itemScroll.ReRender();
		}

		// Token: 0x06007842 RID: 30786 RVA: 0x0037F44D File Offset: 0x0037D64D
		public void AppendCharacterItem(KongsangChacacterItem chacacterItem)
		{
			this._chacacterItems.Add(chacacterItem);
		}

		// Token: 0x06007843 RID: 30787 RVA: 0x0037F45D File Offset: 0x0037D65D
		private IEnumerator PlayFireEff()
		{
			this.mask.gameObject.SetActive(true);
			this.stoveSkeleton.AnimationState.SetAnimation(0, ViewKongsangSpecialInteract.stoveSkeletonAnims[3], true);
			this.eff_kongsang_luhuodanci.gameObject.SetActive(false);
			this.eff_kongsang_luhuodanci.gameObject.SetActive(true);
			this.eff_kongsang_kuosanjinghua.gameObject.SetActive(false);
			this.eff_kongsang_kuosanjinghua.gameObject.SetActive(true);
			this.effHolderFrontForAlphalControl.DOFade(0f, 0.8f);
			foreach (KeyValuePair<ItemKey, Queue<GameObject>> keyValuePair in this._dropItemDict)
			{
				ItemKey itemKey;
				Queue<GameObject> queue;
				keyValuePair.Deconstruct(out itemKey, out queue);
				Queue<GameObject> v = queue;
				foreach (GameObject item in v)
				{
					ParticleSystem eff_kongsang_guchong = item.GetComponent<DropWugKingItem>().eff_kongsang_guchong;
					eff_kongsang_guchong.gameObject.SetActive(true);
					item.GetComponent<CImage>().enabled = false;
					eff_kongsang_guchong = null;
					item = null;
				}
				Queue<GameObject>.Enumerator enumerator2 = default(Queue<GameObject>.Enumerator);
				ItemKey i = default(ItemKey);
				v = null;
			}
			Dictionary<ItemKey, Queue<GameObject>>.Enumerator enumerator = default(Dictionary<ItemKey, Queue<GameObject>>.Enumerator);
			yield return new WaitForSeconds(0.2f);
			foreach (KongsangChacacterItem item2 in this._chacacterItems)
			{
				base.StartCoroutine(item2.PlayClearEff());
				item2 = null;
			}
			List<KongsangChacacterItem>.Enumerator enumerator3 = default(List<KongsangChacacterItem>.Enumerator);
			yield return new WaitForSeconds(0.6f);
			this.effHolderFrontForAlphalControl.DOFade(1f, 0.8f);
			yield return null;
			yield break;
		}

		// Token: 0x06007844 RID: 30788 RVA: 0x0037F46C File Offset: 0x0037D66C
		public int GetDarkAshRemainTime(int charId)
		{
			ValueTuple<int, int> remainTime;
			bool flag = this._characterFeatureRemainTimeDict.TryGetValue(charId, out remainTime) && remainTime.Item1 > 0;
			int result;
			if (flag)
			{
				result = remainTime.Item1;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x06007845 RID: 30789 RVA: 0x0037F4AC File Offset: 0x0037D6AC
		public int GetTripodVesselProtectRemainTime(int charId)
		{
			ValueTuple<int, int> remainTime;
			bool flag = this._characterFeatureRemainTimeDict.TryGetValue(charId, out remainTime) && remainTime.Item2 > 0;
			int result;
			if (flag)
			{
				result = remainTime.Item2;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x04005AD8 RID: 23256
		[SerializeField]
		private InfinityScroll characterScroll;

		// Token: 0x04005AD9 RID: 23257
		[SerializeField]
		private ItemListScroll itemScroll;

		// Token: 0x04005ADA RID: 23258
		[SerializeField]
		private TextMeshProUGUI wugKingCountText;

		// Token: 0x04005ADB RID: 23259
		[SerializeField]
		private TextMeshProUGUI characterCountText;

		// Token: 0x04005ADC RID: 23260
		[SerializeField]
		private CButton confirmBtn;

		// Token: 0x04005ADD RID: 23261
		[SerializeField]
		private DropWugKingItem dropItemPrefab;

		// Token: 0x04005ADE RID: 23262
		[SerializeField]
		private RectTransform dropItemRoot;

		// Token: 0x04005ADF RID: 23263
		[SerializeField]
		private FilterDropdown wugKingFilterDropdown;

		// Token: 0x04005AE0 RID: 23264
		[SerializeField]
		private RectTransform mask;

		// Token: 0x04005AE1 RID: 23265
		[SerializeField]
		private RectTransform empty;

		// Token: 0x04005AE2 RID: 23266
		[SerializeField]
		private SkeletonGraphic stoveSkeleton;

		// Token: 0x04005AE3 RID: 23267
		[SerializeField]
		private ParticleSystem eff_kongsang_luhuodanci;

		// Token: 0x04005AE4 RID: 23268
		[SerializeField]
		private ParticleSystem eff_kongsang_kuosanjinghua;

		// Token: 0x04005AE5 RID: 23269
		[SerializeField]
		private CanvasGroup effHolderFrontForAlphalControl;

		// Token: 0x04005AE6 RID: 23270
		public const float EffTime = 0.8f;

		// Token: 0x04005AE7 RID: 23271
		private const int DropItemSingleMaxCount = 10;

		// Token: 0x04005AE8 RID: 23272
		private const int DropItemTotalMaxCount = 50;

		// Token: 0x04005AE9 RID: 23273
		private const float DropItemCircleRadius = 150f;

		// Token: 0x04005AEA RID: 23274
		private const string ItemPrefabKey = "KongsangSpecialInteractWugKingPrefab";

		// Token: 0x04005AEB RID: 23275
		private static readonly string[] stoveSkeletonAnims = new string[]
		{
			"idle_1",
			"idle_2",
			"fire_1",
			"fire_2"
		};

		// Token: 0x04005AEC RID: 23276
		private readonly List<int> _selectedCharacterIds = new List<int>();

		// Token: 0x04005AED RID: 23277
		private List<CharacterDisplayData> _targetCharacterDisplayDatas = new List<CharacterDisplayData>();

		// Token: 0x04005AEE RID: 23278
		private readonly Dictionary<int, CharacterDisplayData> _characterDisplayDataDict = new Dictionary<int, CharacterDisplayData>();

		// Token: 0x04005AEF RID: 23279
		private readonly Dictionary<int, ValueTuple<int, int>> _characterFeatureRemainTimeDict = new Dictionary<int, ValueTuple<int, int>>();

		// Token: 0x04005AF0 RID: 23280
		private List<ItemDisplayData> _inventoryWugkingList = new List<ItemDisplayData>();

		// Token: 0x04005AF1 RID: 23281
		private readonly List<ItemDisplayData> _filteredWugkingList = new List<ItemDisplayData>();

		// Token: 0x04005AF2 RID: 23282
		private readonly Dictionary<ItemKey, int> _selectedWugKingDict = new Dictionary<ItemKey, int>();

		// Token: 0x04005AF3 RID: 23283
		private readonly Dictionary<ItemKey, Queue<GameObject>> _dropItemDict = new Dictionary<ItemKey, Queue<GameObject>>();

		// Token: 0x04005AF4 RID: 23284
		private readonly List<KongsangChacacterItem> _chacacterItems = new List<KongsangChacacterItem>();
	}
}
