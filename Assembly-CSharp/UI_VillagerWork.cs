using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020003B7 RID: 951
public class UI_VillagerWork : UIBase
{
	// Token: 0x170005CE RID: 1486
	// (get) Token: 0x06003948 RID: 14664 RVA: 0x001D0E16 File Offset: 0x001CF016
	private WorldMapModel MapModel
	{
		get
		{
			return SingletonObject.getInstance<WorldMapModel>();
		}
	}

	// Token: 0x06003949 RID: 14665 RVA: 0x001D0E20 File Offset: 0x001CF020
	public override void OnInit(ArgumentBox argsBox)
	{
		this.NeedDataListenerId = true;
		this._characterDisplayDatas.Clear();
		this._exchangingWorkVillagerId = -1;
		this._popupWindow = base.CGet<PopupWindow>("PopupWindowBase");
		this._popupWindow.TitleLabel.text = LocalStringManager.Get(LanguageKey.LK_Mark_Location_Manage);
		this._popupWindow.OnConfirmClick = new Action(this.CloseUi);
		this._popupWindow.ConfirmButton.interactable = false;
		this.Element.OnListenerIdReady = new Action(this.RefreshData);
	}

	// Token: 0x0600394A RID: 14666 RVA: 0x001D0EB6 File Offset: 0x001CF0B6
	public void Awake()
	{
		this._scroll = base.CGet<InfinityScrollLegacy>("VerticalScrollView");
		this._scroll.OnItemRender = new Action<int, Refers>(this.OnItemRender);
	}

	// Token: 0x0600394B RID: 14667 RVA: 0x001D0EE4 File Offset: 0x001CF0E4
	private void OnDestroy()
	{
		foreach (MapElementInfo element in base.transform.GetComponentsInTopChildren(false))
		{
			bool userBool = element.UserBool;
			if (userBool)
			{
				element.UserBool = false;
				element.Collect();
			}
		}
	}

	// Token: 0x0600394C RID: 14668 RVA: 0x001D0F30 File Offset: 0x001CF130
	private void OnEnable()
	{
		GEvent.Add(UiEvents.VillagerWorkDataChange, new GEvent.Callback(this.OnVillagerWorkDataChange));
		GEvent.Add(UiEvents.OnStopVillagerWork, new GEvent.Callback(this.OnStopVillagerWork));
		GEvent.Add(UiEvents.OnSetVillagerRole, new GEvent.Callback(this.OnSetVillagerRole));
	}

	// Token: 0x0600394D RID: 14669 RVA: 0x001D0F90 File Offset: 0x001CF190
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.VillagerWorkDataChange, new GEvent.Callback(this.OnVillagerWorkDataChange));
		GEvent.Remove(UiEvents.OnStopVillagerWork, new GEvent.Callback(this.OnStopVillagerWork));
		GEvent.Remove(UiEvents.OnSetVillagerRole, new GEvent.Callback(this.OnSetVillagerRole));
	}

	// Token: 0x0600394E RID: 14670 RVA: 0x001D0FF0 File Offset: 0x001CF1F0
	public override void QuickHide()
	{
		bool interactable = this._popupWindow.ConfirmButton.interactable;
		if (interactable)
		{
			AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			this.CloseUi();
		}
	}

	// Token: 0x0600394F RID: 14671 RVA: 0x001D102D File Offset: 0x001CF22D
	private void CloseUi()
	{
		this._popupWindow.ConfirmButton.interactable = false;
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x06003950 RID: 14672 RVA: 0x001D1054 File Offset: 0x001CF254
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
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
					bool flag2 = notification.MethodId == 11;
					if (flag2)
					{
						List<int> charIdList = new List<int>();
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref charIdList);
						bool flag3 = this._characterDisplayDatas.Count > 0;
						if (flag3)
						{
							this.RefreshView();
						}
						else
						{
							charIdList.Add(taiwuCharId);
							CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this, charIdList, delegate(int offset, RawDataPool pool)
							{
								List<CharacterDisplayData> displayDataList = new List<CharacterDisplayData>();
								Serializer.Deserialize(pool, offset, ref displayDataList);
								foreach (CharacterDisplayData displayData in displayDataList)
								{
									this._characterDisplayDatas[displayData.CharacterId] = displayData;
								}
								this.RefreshView();
								this.Element.ShowAfterRefresh();
							});
						}
					}
					else
					{
						bool flag4 = notification.MethodId == 4 || notification.MethodId == 2 || notification.MethodId == 1 || notification.MethodId == 3 || notification.MethodId == 122 || notification.MethodId == 175;
						if (flag4)
						{
							this.RefreshView();
						}
					}
				}
			}
		}
	}

	// Token: 0x06003951 RID: 14673 RVA: 0x001D11AC File Offset: 0x001CF3AC
	private void OnSelectWorkingVillageChar(int charId)
	{
		UI_VillagerWork.<>c__DisplayClass18_0 CS$<>8__locals1 = new UI_VillagerWork.<>c__DisplayClass18_0();
		CS$<>8__locals1.charId = charId;
		CS$<>8__locals1.<>4__this = this;
		WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
		CS$<>8__locals1.location = this._selectingWorkLocation;
		bool flag = CS$<>8__locals1.charId < 0;
		if (flag)
		{
			SingletonObject.getInstance<WorldMapModel>().RequestStopVillagerWorkOnMap(CS$<>8__locals1.location, true);
		}
		else
		{
			bool isAutoDispatch = this._isAutoDispatch;
			if (isAutoDispatch)
			{
				CS$<>8__locals1.<OnSelectWorkingVillageChar>g__ConfirmAction|0();
			}
			else
			{
				BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
				bool marked = buildingModel.CheckBlockIsMarked(CS$<>8__locals1.location);
				DialogCmd cmd = new DialogCmd
				{
					Title = LocalStringManager.Get(LanguageKey.LK_Villager_Collect_Resource_Tip_Title),
					Content = LocalStringManager.GetFormat(marked ? LanguageKey.LK_Villager_Collect_Resource_Confirm : LanguageKey.LK_Villager_Collect_Resource_And_Mark_Confirm, NameCenter.GetCharMonasticTitleOrNameByDisplayData(this._characterDisplayDatas[CS$<>8__locals1.charId], false, false)).ColorReplace(),
					Type = 1,
					Yes = new Action(CS$<>8__locals1.<OnSelectWorkingVillageChar>g__ConfirmAction|0),
					No = null
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
		}
	}

	// Token: 0x06003952 RID: 14674 RVA: 0x001D12D0 File Offset: 0x001CF4D0
	private void RefreshData()
	{
		BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
		HashSetAsDictionary<Location> locationMarkHashSet = buildingModel.LocationMarkHashSet;
		this._locationMarkList.Clear();
		this._locationMarkList.AddRange(locationMarkHashSet.Collection);
		this.MapModel.RequestExtraMapBlockData(this._locationMarkList, true, true);
		TaiwuDomainMethod.Call.GetAllVillagersAvailableForWork(this.Element.GameDataListenerId);
	}

	// Token: 0x06003953 RID: 14675 RVA: 0x001D132E File Offset: 0x001CF52E
	private void RefreshView()
	{
		this._scroll.SetDataCount(this._locationMarkList.Count);
		this._popupWindow.ConfirmButton.interactable = true;
		this.RefreshCollectResult();
	}

	// Token: 0x06003954 RID: 14676 RVA: 0x001D1364 File Offset: 0x001CF564
	private void OnItemRender(int index, Refers refers)
	{
		Location location = this._locationMarkList[index];
		MapBlockData blockData = this.MapModel.GetBlockData(location);
		MapBlockView mapBlockView = refers.CGet<MapBlockView>("MapBlockView");
		MapBlockData rootBlockData = (blockData.RootBlockId > -1) ? this.MapModel.GetBlockData(new Location(blockData.AreaId, blockData.RootBlockId)) : null;
		mapBlockView.Refresh(blockData, rootBlockData);
		this.RefreshSingleView(refers, location);
		TooltipInvoker tips = refers.CGet<TooltipInvoker>("Tips");
		tips.RuntimeParam = new ArgumentBox().SetObject("MapBlockData", blockData);
		tips.NeedRefresh = true;
		refers.CGet<GameObject>("Line").SetActive(index > 0);
	}

	// Token: 0x06003955 RID: 14677 RVA: 0x001D1418 File Offset: 0x001CF618
	private bool IsShouldTotal(VillagerWorkData workData, int characterId)
	{
		bool flag = workData == null;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = workData.WorkType == 10;
			if (flag2)
			{
				Location location = new Location(workData.AreaId, workData.BlockId);
				MapBlockData blockData;
				bool flag3 = !this.MapModel.TryGetBlockData(location, out blockData);
				if (flag3)
				{
					result = false;
				}
				else
				{
					bool? flag4;
					if (blockData == null)
					{
						flag4 = null;
					}
					else
					{
						HashSet<int> characterSet = blockData.CharacterSet;
						flag4 = ((characterSet != null) ? new bool?(characterSet.Contains(characterId)) : null);
					}
					bool? inPlace = flag4;
					result = (inPlace != null && inPlace.Value);
				}
			}
			else
			{
				result = false;
			}
		}
		return result;
	}

	// Token: 0x06003956 RID: 14678 RVA: 0x001D14C4 File Offset: 0x001CF6C4
	private void RefreshCollectResult()
	{
		byte worldResourceAmountType = SingletonObject.getInstance<BasicGameData>().WorldResourceAmountType;
		BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
		Dictionary<sbyte, int> resourceCollectData = EasyPool.Get<Dictionary<sbyte, int>>();
		resourceCollectData.Clear();
		for (sbyte resourceType = 0; resourceType < 6; resourceType += 1)
		{
			resourceCollectData.Add(resourceType, 0);
		}
		using (List<Location>.Enumerator enumerator = this._locationMarkList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Location location = enumerator.Current;
				IEnumerable<KeyValuePair<int, VillagerWorkData>> villagerWork = buildingModel.VillagerWork;
				Func<KeyValuePair<int, VillagerWorkData>, bool> predicate;
				Func<KeyValuePair<int, VillagerWorkData>, bool> <>9__0;
				if ((predicate = <>9__0) == null)
				{
					predicate = (<>9__0 = ((KeyValuePair<int, VillagerWorkData> pair) => pair.Value.AreaId == location.AreaId && pair.Value.BlockId == location.BlockId));
				}
				using (IEnumerator<KeyValuePair<int, VillagerWorkData>> enumerator2 = villagerWork.Where(predicate).GetEnumerator())
				{
					if (enumerator2.MoveNext())
					{
						KeyValuePair<int, VillagerWorkData> pair3 = enumerator2.Current;
						int characterId = pair3.Key;
						VillagerWorkData workData = pair3.Value;
						bool flag = this.IsShouldTotal(workData, characterId);
						if (flag)
						{
							int resource;
							this._modifiedCollectAmount.TryGetValue(characterId, out resource);
							Dictionary<sbyte, int> dictionary = resourceCollectData;
							sbyte resourceType2 = workData.ResourceType;
							dictionary[resourceType2] += resource;
						}
					}
				}
			}
		}
		Refers list = base.CGet<Refers>("ResultList");
		foreach (KeyValuePair<sbyte, int> pair2 in resourceCollectData)
		{
			ResourceTypeItem config = Config.ResourceType.Instance[pair2.Key];
			Refers unit = list.CGet<Refers>(UI_CollectResource.ResourceTypeNames[(int)pair2.Key]);
			bool flag2 = pair2.Value > 0;
			if (flag2)
			{
				unit.gameObject.SetActive(true);
				unit.CGet<TextMeshProUGUI>("Type").text = config.Name;
				unit.CGet<TextMeshProUGUI>("Value").text = string.Format("  +{0}/{1}  ", pair2.Value, LocalStringManager.Get(LanguageKey.LK_Term));
			}
			else
			{
				unit.gameObject.SetActive(false);
			}
		}
		EasyPool.Free<Dictionary<sbyte, int>>(resourceCollectData);
	}

	// Token: 0x06003957 RID: 14679 RVA: 0x001D1724 File Offset: 0x001CF924
	private void RefreshSingleView(Refers panel, Location location)
	{
		UI_VillagerWork.<>c__DisplayClass24_0 CS$<>8__locals1 = new UI_VillagerWork.<>c__DisplayClass24_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.panel = panel;
		CS$<>8__locals1.location = location;
		BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
		CS$<>8__locals1.characterId = -1;
		CS$<>8__locals1.blockData = this.MapModel.GetBlockData(CS$<>8__locals1.location);
		VillagerWorkData workData = null;
		IEnumerable<KeyValuePair<int, VillagerWorkData>> villagerWork = buildingModel.VillagerWork;
		Func<KeyValuePair<int, VillagerWorkData>, bool> predicate;
		if ((predicate = CS$<>8__locals1.<>9__4) == null)
		{
			predicate = (CS$<>8__locals1.<>9__4 = ((KeyValuePair<int, VillagerWorkData> pair) => pair.Value.AreaId == CS$<>8__locals1.blockData.AreaId && pair.Value.BlockId == CS$<>8__locals1.blockData.BlockId));
		}
		using (IEnumerator<KeyValuePair<int, VillagerWorkData>> enumerator = villagerWork.Where(predicate).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				KeyValuePair<int, VillagerWorkData> pair2 = enumerator.Current;
				CS$<>8__locals1.characterId = pair2.Key;
				workData = pair2.Value;
			}
		}
		bool flag = this.IsShouldTotal(workData, CS$<>8__locals1.characterId);
		if (flag)
		{
			ResourceTypeItem resourceConfig = Config.ResourceType.Instance[workData.ResourceType];
			CS$<>8__locals1.panel.CGet<TextMeshProUGUI>("ResourceType").text = resourceConfig.Name;
			CS$<>8__locals1.panel.CGet<CImage>("ResourceIcon").SetSprite(resourceConfig.Icon, false, null);
			TaiwuDomainMethod.AsyncCall.GetVillagerRoleCharacterDisplayData(null, CS$<>8__locals1.characterId, delegate(int offset, RawDataPool dataPool)
			{
				VillagerRoleCharacterDisplayData displayData = new VillagerRoleCharacterDisplayData();
				Serializer.Deserialize(dataPool, offset, ref displayData);
				bool flag4 = displayData.RoleTemplateId == 0;
				if (flag4)
				{
					CS$<>8__locals1.<>4__this._modifiedCollectAmount[CS$<>8__locals1.characterId] = displayData.CollectResourceAmount;
					CS$<>8__locals1.<>4__this.RefreshCollectResult();
				}
				CS$<>8__locals1.panel.CGet<TextMeshProUGUI>("ResourceValue").text = string.Format(" +{0}/{1} ", displayData.CollectResourceAmount, LocalStringManager.Get(LanguageKey.LK_Term));
			});
			CS$<>8__locals1.panel.CGet<GameObject>("Resource").SetActive(true);
		}
		else
		{
			CS$<>8__locals1.panel.CGet<GameObject>("Resource").SetActive(false);
		}
		MapDomainMethod.AsyncCall.GetBlockFullName(this, CS$<>8__locals1.location, delegate(int offsetData, RawDataPool poolData)
		{
			FullBlockName fullBlockName = default(FullBlockName);
			Serializer.Deserialize(poolData, offsetData, ref fullBlockName);
			string blockName = SingletonObject.getInstance<WorldMapModel>().GetFullBlockName(fullBlockName, true, true, true, true);
			CS$<>8__locals1.panel.CGet<TextMeshProUGUI>("LocationName").text = blockName;
		});
		MapElementInfo elementInfo = CS$<>8__locals1.panel.CGet<MapElementInfo>("BlockInfoPrefab");
		bool flag2 = !elementInfo.UserBool;
		if (flag2)
		{
			elementInfo.UserBool = true;
			elementInfo.BindAsyncMethod();
		}
		bool activeInfo = MapElementInfo.CheckMaybeExistForVillager(CS$<>8__locals1.blockData);
		elementInfo.gameObject.SetActive(activeInfo);
		bool flag3 = activeInfo;
		if (flag3)
		{
			elementInfo.SetNotPlayEnterAnim(true);
			elementInfo.Refresh(CS$<>8__locals1.location);
		}
		CButtonObsolete btnRemove = CS$<>8__locals1.panel.CGet<CButtonObsolete>("BtnRemove");
		btnRemove.ClearAndAddListener(delegate
		{
			bool flag4 = CS$<>8__locals1.blockData == null;
			if (!flag4)
			{
				DialogCmd dialogCmd = new DialogCmd();
				dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_Mark_Cancel_Tip_Title);
				dialogCmd.Content = LocalStringManager.Get(LanguageKey.LK_Mark_Cancel_Tip_Desc);
				dialogCmd.Type = 1;
				Action yes;
				if ((yes = CS$<>8__locals1.<>9__6) == null)
				{
					yes = (CS$<>8__locals1.<>9__6 = delegate()
					{
						CS$<>8__locals1.<>4__this._locationMarkList.Remove(CS$<>8__locals1.location);
						ExtraDomainMethod.Call.RemoveLocationMark(CS$<>8__locals1.location);
						SingletonObject.getInstance<WorldMapModel>().RequestStopVillagerWorkOnMap(CS$<>8__locals1.blockData.GetLocation(), true);
					});
				}
				dialogCmd.Yes = yes;
				dialogCmd.No = null;
				DialogCmd cmd = dialogCmd;
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
		});
		VillagerWorkPanel villagerWorkPanel = CS$<>8__locals1.panel.CGet<VillagerWorkPanel>("VillagerWorkPanel");
		villagerWorkPanel.Refresh(this.Element.GameDataListenerId, CS$<>8__locals1.blockData, this._characterDisplayDatas, new Action<int>(CS$<>8__locals1.<RefreshSingleView>g__OnExchangeCharacterIdChanged|2), null);
		CButtonObsolete btnAutoDispatch = villagerWorkPanel.CGet<CButtonObsolete>("BtnDispatch");
		btnAutoDispatch.ClearAndAddListener(delegate
		{
			CS$<>8__locals1.<>4__this._selectingWorkLocation = CS$<>8__locals1.location;
			IAsyncMethodRequestHandler <>4__this = CS$<>8__locals1.<>4__this;
			bool includeUnlockedWorkingVillagers = true;
			bool farmerFirst = CS$<>8__locals1.blockData.GetConfig().ResourceCollectionType > 0;
			AsyncMethodCallbackDelegate callback;
			if ((callback = CS$<>8__locals1.<>9__7) == null)
			{
				callback = (CS$<>8__locals1.<>9__7 = delegate(int offset, RawDataPool dataPool)
				{
					List<int> charIdList = new List<int>();
					Serializer.Deserialize(dataPool, offset, ref charIdList);
					bool flag4 = charIdList.Count > 0;
					if (flag4)
					{
						CS$<>8__locals1.<>4__this._isAutoDispatch = true;
						CS$<>8__locals1.<>4__this.OnSelectWorkingVillageChar(charIdList.First<int>());
						CS$<>8__locals1.<>4__this._isAutoDispatch = false;
					}
				});
			}
			TaiwuDomainMethod.AsyncCall.GetVillagersForWork(<>4__this, includeUnlockedWorkingVillagers, farmerFirst, callback);
		});
	}

	// Token: 0x06003958 RID: 14680 RVA: 0x001D19B0 File Offset: 0x001CFBB0
	private void OpenSelectWindow()
	{
		TaiwuDomainMethod.AsyncCall.GetAllVillagersAvailableForWork(this, delegate(int offset, RawDataPool dataPool)
		{
			List<int> list = new List<int>();
			Serializer.Deserialize(dataPool, offset, ref list);
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
			argumentBox.SetObject("callback", new Action<int>(this.OnSelectWorkingVillageChar));
			argumentBox.SetObject("charIdList", list);
			argumentBox.SetObject("filterType", new List<global::CharacterTable.CharacterTableCommonFilterTypes>
			{
				global::CharacterTable.CharacterTableCommonFilterTypes.Villager
			});
			argumentBox.SetObject("usingPages", new List<ECharacterTableType>
			{
				ECharacterTableType.Villager,
				ECharacterTableType.GeneralProperty,
				ECharacterTableType.MainAndAttackProperty,
				ECharacterTableType.HitProperty,
				ECharacterTableType.LifeSkill,
				ECharacterTableType.CombatSkill,
				ECharacterTableType.Personality,
				ECharacterTableType.ItemAndResource,
				ECharacterTableType.Command,
				ECharacterTableType.LegendBookCompetitors,
				ECharacterTableType.LegendBookFallen
			});
			UIElement.SelectCharLegacy.SetOnInitArgs(argumentBox);
			UIManager.Instance.ShowUI(UIElement.SelectCharLegacy, true);
			this._exchangingWorkVillagerId = -1;
		});
	}

	// Token: 0x06003959 RID: 14681 RVA: 0x001D19C6 File Offset: 0x001CFBC6
	private void OnVillagerWorkDataChange(ArgumentBox argumentBox)
	{
		this.RefreshCollectResult();
	}

	// Token: 0x0600395A RID: 14682 RVA: 0x001D19D0 File Offset: 0x001CFBD0
	private void OnStopVillagerWork(ArgumentBox _)
	{
		this.RefreshView();
	}

	// Token: 0x0600395B RID: 14683 RVA: 0x001D19D9 File Offset: 0x001CFBD9
	private void OnSetVillagerRole(ArgumentBox _)
	{
		this.RefreshView();
	}

	// Token: 0x0400297A RID: 10618
	private PopupWindow _popupWindow;

	// Token: 0x0400297B RID: 10619
	private readonly Dictionary<int, CharacterDisplayData> _characterDisplayDatas = new Dictionary<int, CharacterDisplayData>();

	// Token: 0x0400297C RID: 10620
	private int _exchangingWorkVillagerId;

	// Token: 0x0400297D RID: 10621
	private Location _selectingWorkLocation;

	// Token: 0x0400297E RID: 10622
	private readonly List<Location> _locationMarkList = new List<Location>();

	// Token: 0x0400297F RID: 10623
	private InfinityScrollLegacy _scroll;

	// Token: 0x04002980 RID: 10624
	private bool _isAutoDispatch;

	// Token: 0x04002981 RID: 10625
	private readonly Dictionary<int, int> _modifiedCollectAmount = new Dictionary<int, int>();
}
