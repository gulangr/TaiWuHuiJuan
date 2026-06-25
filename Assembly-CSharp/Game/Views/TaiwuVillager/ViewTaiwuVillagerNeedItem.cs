using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Item;
using Game.Components.SortAndFilter.Item.Apply;
using Game.Components.SortAndFilter.SelectCharacter;
using Game.Views.Select;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;

namespace Game.Views.TaiwuVillager
{
	// Token: 0x02000759 RID: 1881
	public class ViewTaiwuVillagerNeedItem : UIBase
	{
		// Token: 0x06005B0C RID: 23308 RVA: 0x002A42B0 File Offset: 0x002A24B0
		private void Awake()
		{
			this.btnClose.ClearAndAddListener(delegate
			{
				this.QuickHide();
			});
			this.btnSwitchCharacterMode.Init(0);
			this.btnSwitchCharacterMode.OnActiveIndexChange += this.SwitchCharacterListMode;
			this.itemScroll.Init("EquipSortAndFilterController", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnItemRender), new Action<ITradeableContent, RowItemLine>(this.OnItemRowClicked), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.RequirVillagerAmount, null, null, null);
			ItemSortAndFilterController itemSortController = this.itemScroll.SortAndFilterController as ItemSortAndFilterController;
			bool flag = itemSortController != null;
			if (flag)
			{
				itemSortController.ItemSortController.RequireCharacterAmountGetter = new Func<ITradeableContent, int>(this.GetRequireCharacterAmount);
			}
			this.characterScrollFlat.OnItemRender += this.CharacterScrollFlat_OnItemRender;
			this.RefreshCharacterListStructure();
			this.InitSortAndFilter();
		}

		// Token: 0x06005B0D RID: 23309 RVA: 0x002A4386 File Offset: 0x002A2586
		private void SwitchCharacterListMode(int currentIndex, int previousIndex)
		{
			this._characterListMode = (currentIndex == 0);
			this.characterScroll.gameObject.SetActive(this._characterListMode);
			this.characterScrollFlat.gameObject.SetActive(!this._characterListMode);
		}

		// Token: 0x06005B0E RID: 23310 RVA: 0x002A43C4 File Offset: 0x002A25C4
		private void CharacterScrollFlat_OnItemRender(int index, GameObject arg2)
		{
			VillagerNeedCharacterView charView = arg2.GetComponent<VillagerNeedCharacterView>();
			ItemNeedCharacterDisplayData data = this._characterDisplayDataList[index];
			charView.Set(data.AvatarRelatedData, data.NameData, data.TakeTime);
			bool flag = charView.toolTip != null;
			if (flag)
			{
				charView.toolTip.Type = TipType.CharacterOnMapBlock;
				TooltipInvoker toolTip = charView.toolTip;
				if (toolTip.RuntimeParam == null)
				{
					toolTip.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				charView.toolTip.RuntimeParam.Set("CharId", data.CharacterId);
			}
		}

		// Token: 0x06005B0F RID: 23311 RVA: 0x002A4458 File Offset: 0x002A2658
		private void RefreshCharacterListStructure()
		{
			IEnumerable<ColumnDefinition> columnDefinitions = this.GenerateCharacterColumnDefinitions();
			this.PrepareCharacterRowTemplateContainers();
			this.characterScroll.ClearInfinityScrollCache();
			this.characterScroll.Init<ItemNeedCharacterDisplayData>(columnDefinitions, true, null, null);
			this._listStructureReady = true;
		}

		// Token: 0x06005B10 RID: 23312 RVA: 0x002A4498 File Offset: 0x002A2698
		private void PrepareCharacterRowTemplateContainers()
		{
			RowItem currentTemplate = this.CreateRowTemplateForSubPage();
			this.characterScroll.SetRowTemplate(currentTemplate);
		}

		// Token: 0x06005B11 RID: 23313 RVA: 0x002A44BC File Offset: 0x002A26BC
		private RowItem CreateRowTemplateForSubPage()
		{
			RowItem newTemplate = Object.Instantiate<RowItem>(this.rowTemplate, this.rowTemplate.transform.parent);
			newTemplate.gameObject.SetActive(false);
			Transform containerRoot = newTemplate.ContainerRoot;
			RowCellContainer avatarContainer = Object.Instantiate<RowCellContainer>(this.avatarAndNameCellContainer, containerRoot);
			avatarContainer.gameObject.SetActive(true);
			IEnumerable<ColumnDefinition> columnDefinitions = ViewTaiwuVillagerNeedItem.GenerateStateColumns();
			foreach (ColumnDefinition columnDef in columnDefinitions)
			{
				Type[] genericArgs = columnDef.GetType().GetGenericArguments();
				bool flag = genericArgs.Length >= 2 && genericArgs[1] == typeof(IconAndTextCellData);
				RowCellContainer container;
				if (flag)
				{
					container = Object.Instantiate<RowCellContainer>(this.iconAndTextCellContainer, containerRoot);
				}
				else
				{
					container = Object.Instantiate<RowCellContainer>(this.singleTextCellContainer, containerRoot);
				}
				container.gameObject.SetActive(true);
			}
			newTemplate.ResetSibling();
			return newTemplate;
		}

		// Token: 0x06005B12 RID: 23314 RVA: 0x002A45C8 File Offset: 0x002A27C8
		private void OnItemRowClicked(ITradeableContent content, RowItemLine line)
		{
			ItemDisplayData itemData = content as ItemDisplayData;
			this._selectedItemkey = content.Key;
			foreach (ItemNeedDisplayData item in this._cachedData)
			{
				bool flag = item.ItemDisplayData.Key == itemData.Key;
				if (flag)
				{
					this.RefreshCharacterContent(item.CharacterDisplayDataList);
				}
			}
		}

		// Token: 0x06005B13 RID: 23315 RVA: 0x002A4658 File Offset: 0x002A2858
		private void RefreshCharacterContent(List<ItemNeedCharacterDisplayData> characterDisplayDataList)
		{
			this._characterDisplayDataList = characterDisplayDataList;
			bool flag = this._characterDisplayDataList == null;
			if (flag)
			{
				this.characterScroll.SetData<ItemNeedCharacterDisplayData>(new List<ItemNeedCharacterDisplayData>(), -1);
				this.characterScrollFlat.SetDataCount(0);
				this.emptyObject.gameObject.SetActive(true);
				SelectCharacterSortAndFilterController characterSortAndFilterController = this._characterSortAndFilterController;
				if (characterSortAndFilterController != null)
				{
					characterSortAndFilterController.SetFilteredCount(0);
				}
				this.SetNeedVillagerAmountText(0);
			}
			else
			{
				List<ISelectCharacterData> filteredDataList = this._characterDisplayDataList.Cast<ISelectCharacterData>().ToList<ISelectCharacterData>();
				SelectCharacterSortAndFilterController characterSortAndFilterController2 = this._characterSortAndFilterController;
				Func<ISelectCharacterData, bool> func;
				if ((func = ((characterSortAndFilterController2 != null) ? characterSortAndFilterController2.GenerateFilter() : null)) == null && (func = ViewTaiwuVillagerNeedItem.<>c.<>9__28_0) == null)
				{
					func = (ViewTaiwuVillagerNeedItem.<>c.<>9__28_0 = ((ISelectCharacterData _) => true));
				}
				Func<ISelectCharacterData, bool> filter = func;
				filteredDataList = filteredDataList.Where(filter).ToList<ISelectCharacterData>();
				SelectCharacterSortAndFilterController characterSortAndFilterController3 = this._characterSortAndFilterController;
				Comparison<ISelectCharacterData> comparer = (characterSortAndFilterController3 != null) ? characterSortAndFilterController3.GenerateComparer(filteredDataList) : null;
				bool flag2 = comparer != null;
				if (flag2)
				{
					filteredDataList.Sort(comparer);
				}
				SelectCharacterSortAndFilterController characterSortAndFilterController4 = this._characterSortAndFilterController;
				if (characterSortAndFilterController4 != null)
				{
					characterSortAndFilterController4.AfterFilter(this._characterDisplayDataList);
				}
				List<ItemNeedCharacterDisplayData> displayDataList = filteredDataList.Cast<ItemNeedCharacterDisplayData>().ToList<ItemNeedCharacterDisplayData>();
				this.characterScroll.SetData<ItemNeedCharacterDisplayData>(displayDataList, -1);
				this.characterScrollFlat.SetDataCount(displayDataList.Count);
				this.emptyObject.gameObject.SetActive(displayDataList.Count == 0);
				this.SetNeedVillagerAmountText(displayDataList.Count);
			}
		}

		// Token: 0x06005B14 RID: 23316 RVA: 0x002A47B5 File Offset: 0x002A29B5
		private void SetNeedVillagerAmountText(int value)
		{
			this.neededVillagerTitle.text = LocalStringManager.GetFormat(LanguageKey.LK_VillagerNeed_TakeCharacterTitle, value);
		}

		// Token: 0x06005B15 RID: 23317 RVA: 0x002A47D4 File Offset: 0x002A29D4
		private void OnItemRender(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			rowItemLine.SetInteractable(true, true);
			rowItemLine.SetDisabled(false);
			bool flag = itemData.Key == this._selectedItemkey;
			if (flag)
			{
				this.OnItemRowClicked(itemData, rowItemLine);
			}
			rowItemLine.SetSelected(itemData.Key == this._selectedItemkey);
			int takeCount = 1;
			foreach (ItemNeedDisplayData item in this._cachedData)
			{
				bool flag2 = item.ItemDisplayData.Key == itemData.Key;
				if (flag2)
				{
					takeCount = item.CharacterDisplayDataList.Count;
				}
			}
			bool flag3 = !this.itemScroll.IsCardMode;
			if (flag3)
			{
				rowItemLine.ContainerRoot.GetChild(rowItemLine.ContainerRoot.childCount - 1).GetComponentInChildren<TextMeshProUGUI>().text = takeCount.ToString();
			}
		}

		// Token: 0x06005B16 RID: 23318 RVA: 0x002A48F8 File Offset: 0x002A2AF8
		private void InitSortAndFilter()
		{
			bool flag = this.characterSortAndFilter == null;
			if (!flag)
			{
				this._characterSortAndFilterController = new SelectCharacterSortAndFilterController(this.characterSortAndFilter, new List<ESelectCharacterFilterMenuId>
				{
					ESelectCharacterFilterMenuId.RoleArrangement
				}, null, false);
				this._characterSortAndFilterController.Init(new Action(this.OnCharacterSortAndFilterChanged), "SelectCharacter");
				this.characterScroll.SetSortController(this._characterSortAndFilterController);
				List<SortItemState> newItemStates = new List<SortItemState>();
				newItemStates.Add(new SortItemState
				{
					SortId = 134,
					SortDirection = ESortDirection.Ascending
				});
				this._characterSortAndFilterController.SetSortState(new SortStateData
				{
					ItemStates = newItemStates
				});
			}
		}

		// Token: 0x06005B17 RID: 23319 RVA: 0x002A49AD File Offset: 0x002A2BAD
		private void OnCharacterSortAndFilterChanged()
		{
			this.RefreshCharacterContent(this._characterDisplayDataList);
		}

		// Token: 0x06005B18 RID: 23320 RVA: 0x002A49BD File Offset: 0x002A2BBD
		private IEnumerable<ColumnDefinition> GenerateItemColumnDefinitions()
		{
			yield return this.CreateAvatarWithNameColumn();
			yield break;
		}

		// Token: 0x06005B19 RID: 23321 RVA: 0x002A49CD File Offset: 0x002A2BCD
		private IEnumerable<ColumnDefinition> GenerateCharacterColumnDefinitions()
		{
			yield return this.CreateAvatarWithNameColumn();
			foreach (ColumnDefinition col in ViewTaiwuVillagerNeedItem.GenerateStateColumns())
			{
				yield return col;
				col = null;
			}
			IEnumerator<ColumnDefinition> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06005B1A RID: 23322 RVA: 0x002A49E0 File Offset: 0x002A2BE0
		private ColumnDefinition CreateAvatarWithNameColumn()
		{
			ColumnDefinition<ItemNeedCharacterDisplayData, AvatarWithNameCellData> columnDefinition = new ColumnDefinition<ItemNeedCharacterDisplayData, AvatarWithNameCellData>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 337f,
				FlexibleWidth = 0f,
				PreferredWidth = 337f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Char_Name.Tr());
			columnDefinition.CellDataGenerator = ((ItemNeedCharacterDisplayData data) => AvatarWithNameCellData.FromItemNeedCharacterDisplayData(data, false, null, null));
			columnDefinition.SortId = 0;
			return columnDefinition;
		}

		// Token: 0x06005B1B RID: 23323 RVA: 0x002A4A83 File Offset: 0x002A2C83
		private static IEnumerable<ColumnDefinition> GenerateStateColumns()
		{
			yield return ViewTaiwuVillagerNeedItem.CreateIconAndTextColumn(() => LanguageKey.LK_VillagerInfo_TakeDate.Tr(), (ItemNeedCharacterDisplayData data) => ViewTaiwuVillagerNeedItem.CreateRemainMonthData(data.TakeTime), 134, 150f, 155f);
			yield return ViewTaiwuVillagerNeedItem.CreateTextColumn(() => LanguageKey.LK_VillagerInfo_TakeAmount.Tr(), (ItemNeedCharacterDisplayData data) => data.TakeAmount.ToString(), 135, 120f, 120f);
			yield return ViewTaiwuVillagerNeedItem.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Identity.Tr(), (ItemNeedCharacterDisplayData data) => ViewTaiwuVillagerNeedItem.GetRoleName(data.RoleTemplateId), 1, 120f, 120f);
			yield return ViewTaiwuVillagerNeedItem.CreateTextColumn(() => LanguageKey.LK_ItemDeltaFeeling.Tr(), (ItemNeedCharacterDisplayData data) => ViewTaiwuVillagerNeedItem.GetFavorDisplayString(data.FavorabilityToTaiwu, data.IsInteractedWithTaiwu), 11, 120f, 120f);
			yield return ViewTaiwuVillagerNeedItem.CreateTextColumn(() => LanguageKey.LK_RelationShip.Tr(), (ItemNeedCharacterDisplayData data) => ViewTaiwuVillagerNeedItem.GetHighestPriorityRelationText(data.RelationToTaiwu), 136, 120f, 120f);
			yield return ViewTaiwuVillagerNeedItem.CreateTextColumn(() => LanguageKey.LK_VillagerInfo_InfluencePower.Tr(), (ItemNeedCharacterDisplayData data) => data.PowerLevel.ToString(), 137, 120f, 120f);
			yield break;
		}

		// Token: 0x06005B1C RID: 23324 RVA: 0x002A4A8C File Offset: 0x002A2C8C
		private static string GetRoleName(short roleTemplateId)
		{
			bool flag = roleTemplateId < 0;
			string result;
			if (flag)
			{
				result = LocalStringManager.Get(LanguageKey.LK_Villager);
			}
			else
			{
				short memeberId = VillagerRole.Instance[roleTemplateId].OrganizationMember;
				result = OrganizationMember.Instance[memeberId].GradeName;
			}
			return result;
		}

		// Token: 0x06005B1D RID: 23325 RVA: 0x002A4AD8 File Offset: 0x002A2CD8
		private static ColumnDefinition CreateTextColumn(Func<string> headerKey, Func<ItemNeedCharacterDisplayData, string> valueGetter, short sortId = -1, float minWidth = 30f, float preferredWidth = 90f)
		{
			return new ColumnDefinition<ItemNeedCharacterDisplayData, string>
			{
				LayoutOption = new LayoutOption
				{
					MinWidth = minWidth,
					FlexibleWidth = 1f,
					PreferredWidth = preferredWidth,
					Priority = 1
				},
				TableHeadLabel = headerKey,
				CellDataGenerator = ((ItemNeedCharacterDisplayData data) => valueGetter(data)),
				SortId = sortId
			};
		}

		// Token: 0x06005B1E RID: 23326 RVA: 0x002A4B50 File Offset: 0x002A2D50
		private static ColumnDefinition CreateIconAndTextColumn(Func<string> headerKey, Func<ItemNeedCharacterDisplayData, IconAndTextCellData> valueGetter, short sortId = -1, float minWidth = 30f, float preferredWidth = 90f)
		{
			return new ColumnDefinition<ItemNeedCharacterDisplayData, IconAndTextCellData>
			{
				LayoutOption = new LayoutOption
				{
					MinWidth = minWidth,
					PreferredWidth = preferredWidth,
					FlexibleWidth = 1f
				},
				TableHeadLabel = headerKey,
				CellDataGenerator = valueGetter,
				SortId = sortId
			};
		}

		// Token: 0x06005B1F RID: 23327 RVA: 0x002A4BA8 File Offset: 0x002A2DA8
		private static IconAndTextCellData CreateRemainMonthData(int remainMonth)
		{
			string iconName = "ui9_icon_month";
			string text = LocalStringManager.GetFormat(LanguageKey.LK_VillagerNeed_TakeAfterMonth, remainMonth);
			return new IconAndTextCellData(iconName, text, true, false, false, false);
		}

		// Token: 0x06005B20 RID: 23328 RVA: 0x002A4BDC File Offset: 0x002A2DDC
		private static string GetFavorDisplayString(short favorabilityToTaiwu, bool isInteractedWithTaiwu)
		{
			return CommonUtils.GetFavorStringByInteracted(favorabilityToTaiwu, isInteractedWithTaiwu);
		}

		// Token: 0x06005B21 RID: 23329 RVA: 0x002A4BF8 File Offset: 0x002A2DF8
		private static string GetHighestPriorityRelationText(ushort relationToTaiwu)
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

		// Token: 0x06005B22 RID: 23330 RVA: 0x002A4D28 File Offset: 0x002A2F28
		public override void OnInit(ArgumentBox argsBox)
		{
			this._selectedItemkey = ItemKey.Invalid;
			ItemKey itemKey;
			bool flag = argsBox.Get<ItemKey>("ItemKey", out itemKey);
			if (flag)
			{
				this._selectedItemkey = itemKey;
			}
			bool flag2 = this._characterSortAndFilterController != null;
			if (flag2)
			{
				List<SortItemState> newItemStates = new List<SortItemState>();
				newItemStates.Add(new SortItemState
				{
					SortId = 134,
					SortDirection = ESortDirection.Ascending
				});
				this._characterSortAndFilterController.SetSortState(new SortStateData
				{
					ItemStates = newItemStates
				});
			}
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				this._characterDisplayDataList.Clear();
				TaiwuDomainMethod.Call.GetTreasuryNeededItemDataList(this.Element.GameDataListenerId);
				SelectCharacterSortAndFilterController characterSortAndFilterController = this._characterSortAndFilterController;
				if (characterSortAndFilterController != null)
				{
					characterSortAndFilterController.AfterFilter(this._characterDisplayDataList);
				}
			}));
		}

		// Token: 0x06005B23 RID: 23331 RVA: 0x002A4DDC File Offset: 0x002A2FDC
		private int GetCurrValue(HashSet<int> selectedCharIdList)
		{
			return base.CGet<global::CharacterTable>("CharacterTable").GetCharIdList().Count;
		}

		// Token: 0x06005B24 RID: 23332 RVA: 0x002A4E04 File Offset: 0x002A3004
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			base.OnNotifyGameData(notifications);
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
						bool flag2 = notification.MethodId == 217;
						if (flag2)
						{
							this._cachedData.Clear();
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._cachedData);
							this.Element.ShowAfterRefresh();
							this.RefreshListData();
						}
					}
				}
			}
		}

		// Token: 0x06005B25 RID: 23333 RVA: 0x002A4ED8 File Offset: 0x002A30D8
		private void OnDisable()
		{
			this.characterScroll.SetData<ItemNeedCharacterDisplayData>(new List<ItemNeedCharacterDisplayData>(), -1);
			this.characterScrollFlat.SetDataCount(0);
			this.SetNeedVillagerAmountText(0);
			this.emptyObject.gameObject.SetActive(true);
		}

		// Token: 0x06005B26 RID: 23334 RVA: 0x002A4F14 File Offset: 0x002A3114
		private int GetRequireCharacterAmount(ITradeableContent content)
		{
			return this._requireCharacterAmountMap.GetValueOrDefault(content.Key);
		}

		// Token: 0x06005B27 RID: 23335 RVA: 0x002A4F28 File Offset: 0x002A3128
		private void RefreshRequireCharacterAmountMap()
		{
			this._requireCharacterAmountMap.Clear();
			foreach (ItemNeedDisplayData item in this._cachedData)
			{
				Dictionary<ItemKey, int> requireCharacterAmountMap = this._requireCharacterAmountMap;
				ItemKey key = item.ItemDisplayData.Key;
				List<ItemNeedCharacterDisplayData> characterDisplayDataList = item.CharacterDisplayDataList;
				requireCharacterAmountMap[key] = ((characterDisplayDataList != null) ? characterDisplayDataList.Count : 0);
			}
		}

		// Token: 0x06005B28 RID: 23336 RVA: 0x002A4FAC File Offset: 0x002A31AC
		private void RefreshListData()
		{
			this.RefreshRequireCharacterAmountMap();
			List<ItemDisplayData> list = (from t in this._cachedData
			select t.ItemDisplayData).ToList<ItemDisplayData>();
			this.itemScroll.SetItemList(list, 0);
			bool flag = this._selectedItemkey == ItemKey.Invalid && this.itemScroll.FilteredData.Count > 0;
			if (flag)
			{
				this._selectedItemkey = this.itemScroll.FilteredData[0].Key;
			}
			this.itemScroll.ReRender();
		}

		// Token: 0x04003ECB RID: 16075
		[SerializeField]
		private CButton btnClose;

		// Token: 0x04003ECC RID: 16076
		[SerializeField]
		private CToggleGroup btnSwitchCharacterMode;

		// Token: 0x04003ECD RID: 16077
		[Header("物品列表")]
		[SerializeField]
		private ItemListScroll itemScroll;

		// Token: 0x04003ECE RID: 16078
		[SerializeField]
		private TextMeshProUGUI neededVillagerTitle;

		// Token: 0x04003ECF RID: 16079
		[Header("角色列表")]
		[SerializeField]
		private VillagerNeedCharacterView flatModeTemplate;

		// Token: 0x04003ED0 RID: 16080
		[SerializeField]
		private InfinityScroll characterScrollFlat;

		// Token: 0x04003ED1 RID: 16081
		[SerializeField]
		private ListStyleGeneralScroll characterScroll;

		// Token: 0x04003ED2 RID: 16082
		[SerializeField]
		private SortAndFilter characterSortAndFilter;

		// Token: 0x04003ED3 RID: 16083
		[SerializeField]
		private GameObject emptyObject;

		// Token: 0x04003ED4 RID: 16084
		[Header("行模板配置")]
		[SerializeField]
		private RowItem rowTemplate;

		// Token: 0x04003ED5 RID: 16085
		[SerializeField]
		private RowCellContainer avatarAndNameCellContainer;

		// Token: 0x04003ED6 RID: 16086
		[SerializeField]
		private RowCellContainer singleTextCellContainer;

		// Token: 0x04003ED7 RID: 16087
		[SerializeField]
		private RowCellContainer iconAndTextCellContainer;

		// Token: 0x04003ED8 RID: 16088
		private List<ItemNeedDisplayData> _cachedData = new List<ItemNeedDisplayData>();

		// Token: 0x04003ED9 RID: 16089
		private List<ItemNeedCharacterDisplayData> _characterDisplayDataList = new List<ItemNeedCharacterDisplayData>();

		// Token: 0x04003EDA RID: 16090
		private SelectCharacterSortAndFilterController _characterSortAndFilterController;

		// Token: 0x04003EDB RID: 16091
		private List<ItemKey> _villagerNeededItemSet = new List<ItemKey>();

		// Token: 0x04003EDC RID: 16092
		private readonly Dictionary<ItemKey, int> _requireCharacterAmountMap = new Dictionary<ItemKey, int>();

		// Token: 0x04003EDD RID: 16093
		private bool _listStructureReady;

		// Token: 0x04003EDE RID: 16094
		private bool _characterListMode = true;

		// Token: 0x04003EDF RID: 16095
		private ItemKey _selectedItemkey = ItemKey.Invalid;
	}
}
