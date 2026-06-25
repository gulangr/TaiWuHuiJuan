using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using GameData.Common;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;

// Token: 0x02000103 RID: 259
public class BuildingModel : ISingletonInit, IDisposable
{
	// Token: 0x170000EB RID: 235
	// (get) Token: 0x060008D5 RID: 2261 RVA: 0x0003CEEF File Offset: 0x0003B0EF
	private FunctionLockManager FunctionLockManager
	{
		get
		{
			return SingletonObject.getInstance<FunctionLockManager>();
		}
	}

	// Token: 0x170000EC RID: 236
	// (get) Token: 0x060008D6 RID: 2262 RVA: 0x0003CEF6 File Offset: 0x0003B0F6
	public bool VillageManagementUnlocked
	{
		get
		{
			return this.FunctionLockManager.IsFunctionUnlock(10);
		}
	}

	// Token: 0x170000ED RID: 237
	// (get) Token: 0x060008D7 RID: 2263 RVA: 0x0003CF05 File Offset: 0x0003B105
	public bool CanOperateStoneRoom
	{
		get
		{
			return this.FunctionLockManager.IsFunctionUnlock(4);
		}
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x0003CF14 File Offset: 0x0003B114
	public void Init()
	{
		this._gameDataListenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyBuildingData));
		for (int i = 0; i < 15; i++)
		{
			GameDataBridge.AddDataMonitor(this._gameDataListenerId, 20, 0, (ulong)((long)i), uint.MaxValue);
		}
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 9, 4, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 9, 3, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 9, 5, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 9, 18, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 9, 21, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 5, 53, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 5, 65, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 141, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 9, 1, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 177, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 189, ulong.MaxValue, uint.MaxValue);
		GEvent.Add(EEvents.OnFunctionLockStateChange, new GEvent.Callback(this.OnGameFunctionLockStateChange));
		GEvent.Add(EEvents.OnTaiwuCharIdChange, new GEvent.Callback(this.OnTaiwuCharIdChange));
		GEvent.Add(UiEvents.CancelPlanOrRemoveBuilding, new GEvent.Callback(this.CancelBuildingPlan));
	}

	// Token: 0x060008D9 RID: 2265 RVA: 0x0003D07B File Offset: 0x0003B27B
	private void CancelBuildingPlan(ArgumentBox argbox)
	{
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 9, 5, ulong.MaxValue, uint.MaxValue);
	}

	// Token: 0x060008DA RID: 2266 RVA: 0x0003D090 File Offset: 0x0003B290
	private void OnGameFunctionLockStateChange(ArgumentBox argBox)
	{
		FunctionLockManager funcLockManager = SingletonObject.getInstance<FunctionLockManager>();
		byte functionId;
		argBox.Get("FunctionId", out functionId);
		bool flag = functionId == 11;
		if (flag)
		{
			bool flag2 = funcLockManager.IsFunctionUnlock(11);
			if (flag2)
			{
				BuildingDomainMethod.Call.InitMapBlockChicken();
			}
		}
	}

	// Token: 0x060008DB RID: 2267 RVA: 0x0003D0D8 File Offset: 0x0003B2D8
	private void OnNotifyBuildingData(List<NotificationWrapper> notifications)
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
						bool flag2 = notification.MethodId == 126;
						if (flag2)
						{
							ValueTuple<ItemSourceType, ResourceInts> tuple = default(ValueTuple<ItemSourceType, ResourceInts>);
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref tuple);
							this.ResourceDict[tuple.Item1] = tuple.Item2;
							ItemSourceType item = tuple.Item1;
							List<ItemSourceType> needItemSourceTypeList = this.NeedItemSourceTypeList;
							bool flag3 = item == needItemSourceTypeList[needItemSourceTypeList.Count - 1];
							if (flag3)
							{
								GEvent.OnEvent(EEvents.OnTaiwuResourceChange, null);
							}
						}
					}
				}
			}
			else
			{
				DataUid uid = notification.Uid;
				bool flag4 = uid.DomainId == 20;
				if (flag4)
				{
					bool flag5 = uid.DataId == 0;
					if (flag5)
					{
						sbyte status = 0;
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref status);
						this._stateTaskStatuses[(int)(checked((IntPtr)notification.Uid.SubId0))] = status;
					}
				}
				bool flag6 = uid.DomainId == 9;
				if (flag6)
				{
					UiEvents uiEvent = UiEvents.Invalid;
					HashSet<BuildingBlockKey> modifiedKeys = EasyPool.Get<HashSet<BuildingBlockKey>>();
					bool flag7 = uid.DataId == 21;
					if (flag7)
					{
						Serializer.DeserializeModifications<Location>(wrapper.DataPool, notification.ValueOffset, this.LocationMarkHashSet);
						HashSet<Location> modifiedKeys2 = EasyPool.Get<HashSet<Location>>();
						CommonUtils.GetModifiedSingleValueCollectionKeyOfStruct<Location, VoidValue>(wrapper.DataPool, notification.ValueOffset, modifiedKeys2);
						foreach (Location location in modifiedKeys2)
						{
							bool marked = this.LocationMarkHashSet.Contains(location);
							ArgumentBox argsBox = EasyPool.Get<ArgumentBox>();
							argsBox.Set("AreaId", location.AreaId);
							argsBox.Set("BlockId", location.BlockId);
							argsBox.Set("Marked", marked);
							GEvent.OnEvent(UiEvents.OnLocationMarkChange, argsBox);
						}
						EasyPool.Free<HashSet<Location>>(modifiedKeys2);
					}
					else
					{
						bool flag8 = uid.DataId == 4;
						if (flag8)
						{
							uiEvent = UiEvents.BuildingOperatorChange;
							Serializer.DeserializeModifications<BuildingBlockKey>(wrapper.DataPool, notification.ValueOffset, this.BuildingOperators);
							CommonUtils.GetModifiedSingleValueCollectionKeyOfStruct<BuildingBlockKey, CharacterList>(wrapper.DataPool, notification.ValueOffset, modifiedKeys);
						}
						else
						{
							bool flag9 = uid.DataId == 3;
							if (flag9)
							{
								Serializer.DeserializeModifications<BuildingBlockKey>(wrapper.DataPool, notification.ValueOffset, this.CollectBuildingResourceType);
							}
							else
							{
								bool flag10 = uid.DataId == 5;
								if (flag10)
								{
									uiEvent = UiEvents.BuildingCustomNameChange;
									Serializer.DeserializeModifications<BuildingBlockKey>(wrapper.DataPool, notification.ValueOffset, this.CustomBuildingName);
									CommonUtils.GetModifiedSingleValueCollectionKeyOfPrimitive<BuildingBlockKey, int>(wrapper.DataPool, notification.ValueOffset, modifiedKeys);
								}
								else
								{
									bool flag11 = uid.DataId == 18;
									if (flag11)
									{
										uiEvent = UiEvents.BuildingShopManagerChange;
										Serializer.DeserializeModifications<BuildingBlockKey>(wrapper.DataPool, notification.ValueOffset, this._shopManagerDict);
										CommonUtils.GetModifiedSingleValueCollectionKeyOfStruct<BuildingBlockKey, CharacterList>(wrapper.DataPool, notification.ValueOffset, modifiedKeys);
									}
									else
									{
										bool flag12 = uid.DataId == 1;
										if (flag12)
										{
											Serializer.DeserializeModifications<BuildingBlockKey>(wrapper.DataPool, notification.ValueOffset, this._buildingBlockDataDict);
											GEvent.OnEvent(UiEvents.BuildingBlockDataChange, null);
										}
									}
								}
							}
						}
					}
					bool flag13 = uiEvent != UiEvents.Invalid;
					if (flag13)
					{
						bool flag14 = uiEvent == UiEvents.BuildingCustomNameChange;
						if (flag14)
						{
							YieldHelper yieldHelper = SingletonObject.getInstance<YieldHelper>();
							using (HashSet<BuildingBlockKey>.Enumerator enumerator3 = modifiedKeys.GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									BuildingBlockKey blockKey = enumerator3.Current;
									yieldHelper.DelayFrameDo(1U, delegate
									{
										GEvent.OnEvent(uiEvent, EasyPool.Get<ArgumentBox>().SetObject("BuildingBlockKey", blockKey));
									});
								}
							}
						}
						else
						{
							foreach (BuildingBlockKey blockKey2 in modifiedKeys)
							{
								GEvent.OnEvent(uiEvent, EasyPool.Get<ArgumentBox>().SetObject("BuildingBlockKey", blockKey2));
							}
						}
					}
					EasyPool.Free<HashSet<BuildingBlockKey>>(modifiedKeys);
				}
				else
				{
					bool flag15 = uid.DomainId == 5;
					if (flag15)
					{
						bool flag16 = uid.DataId == 53;
						if (flag16)
						{
							Serializer.DeserializeModifications<int>(wrapper.DataPool, notification.ValueOffset, this.VillagerWork);
							GEvent.OnEvent(UiEvents.VillagerWorkDataChange, null);
						}
						else
						{
							bool flag17 = uid.DataId == 65;
							if (flag17)
							{
								this.RefreshResources();
							}
						}
					}
					else
					{
						bool flag18 = uid.DomainId == 19;
						if (flag18)
						{
							bool flag19 = uid.DataId == 141;
							if (flag19)
							{
								Serializer.DeserializeModifications<int>(wrapper.DataPool, notification.ValueOffset, this.BuildingResourceOutputSettings);
								GEvent.OnEvent(UiEvents.BuildingResourceOutputSettingsChanged, null);
							}
							else
							{
								bool flag20 = uid.DataId == 177;
								if (flag20)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._buildingArrangementSettingPresetData);
								}
								else
								{
									bool flag21 = uid.DataId == 189;
									if (flag21)
									{
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._sectYuanshanThreeVitals);
										ExtraDomainMethod.AsyncCall.AreVitalsDemon(null, delegate(int offset, RawDataPool pool)
										{
											Serializer.Deserialize(pool, offset, ref this._areVitalsDemon);
										});
										ExtraDomainMethod.AsyncCall.GetCanSelectThreeVitalsDisplayData(null, delegate(int offset, RawDataPool pool)
										{
											Serializer.Deserialize(pool, offset, ref this.YuanshanThreeVitalsDisplayData);
										});
										ExtraDomainMethod.AsyncCall.GetOppositeThreeVitalsCharDataList(null, delegate(int offset, RawDataPool pool)
										{
											Serializer.Deserialize(pool, offset, ref this.YuanshanOppositeThreeVitalsDisplayData);
										});
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060008DC RID: 2268 RVA: 0x0003D724 File Offset: 0x0003B924
	public sbyte GetCollectBuildingResourceTypeWithToxicology(BuildingBlockKey blockKey, BuildingBlockData block)
	{
		BuildingBlockItem configData = BuildingBlock.Instance[block.TemplateId];
		bool flag = !configData.IsCollectResourceBuilding;
		if (flag)
		{
			throw new Exception(string.Format("Building {0} is not a collect resource building", block.TemplateId));
		}
		bool flag2 = this.CollectBuildingResourceType.ContainsKey(blockKey);
		if (!flag2)
		{
			BuildingBlockItem resourceConfig = BuildingBlock.Instance[configData.DependBuildings[0]];
			for (int i = 0; i < resourceConfig.CollectResourcePercent.Length; i++)
			{
				bool flag3 = resourceConfig.CollectResourcePercent[i] > 0;
				if (flag3)
				{
					return (sbyte)i;
				}
			}
			throw new Exception(string.Format("Building {0} has no collectable resource type", block.TemplateId));
		}
		return this.CollectBuildingResourceType[blockKey];
	}

	// Token: 0x060008DD RID: 2269 RVA: 0x0003D7F8 File Offset: 0x0003B9F8
	public sbyte GetCollectBuildingResourceTypeReal(BuildingBlockKey blockKey, BuildingBlockData block)
	{
		sbyte resourceType = this.GetCollectBuildingResourceTypeWithToxicology(blockKey, block);
		bool flag = resourceType >= 6;
		if (flag)
		{
			resourceType = 5;
		}
		return resourceType;
	}

	// Token: 0x060008DE RID: 2270 RVA: 0x0003D824 File Offset: 0x0003BA24
	public void Dispose()
	{
		GEvent.Remove(EEvents.OnFunctionLockStateChange, new GEvent.Callback(this.OnGameFunctionLockStateChange));
		GEvent.Remove(UiEvents.CancelPlanOrRemoveBuilding, new GEvent.Callback(this.CancelBuildingPlan));
		for (int i = 0; i < 15; i++)
		{
			GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 20, 0, (ulong)((long)i), uint.MaxValue);
		}
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 9, 4, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 9, 3, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 9, 5, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 9, 18, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 5, 53, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 5, 65, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 9, 21, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 141, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 9, 1, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 177, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 189, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.UnregisterListener(this._gameDataListenerId);
		this.BuildingOperators.Clear();
	}

	// Token: 0x060008DF RID: 2271 RVA: 0x0003D974 File Offset: 0x0003BB74
	public bool CheckBlockIsMarked(Location location)
	{
		if (location.IsValid())
		{
			HashSetAsDictionary<Location> locationMarkHashSet = this.LocationMarkHashSet;
			if (locationMarkHashSet != null && locationMarkHashSet.Count > 0)
			{
				VoidValue voidValue;
				return this.LocationMarkHashSet.TryGetValue(location, out voidValue);
			}
		}
		return false;
	}

	// Token: 0x060008E0 RID: 2272 RVA: 0x0003D9B0 File Offset: 0x0003BBB0
	public bool CheckBlockHasWork(Location location, sbyte type = -1)
	{
		bool flag = !location.IsValid();
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			VillagerWorkData workData = this.VillagerWork.Values.FirstOrDefault((VillagerWorkData d) => d.AreaId == location.AreaId && d.BlockId == location.BlockId);
			bool hasWork = workData != null && VillagerWorkType.IsWorkOnMapAndNeedMark(workData.WorkType);
			bool flag2 = hasWork && type > -1 && type != workData.WorkType;
			if (flag2)
			{
				hasWork = false;
			}
			result = hasWork;
		}
		return result;
	}

	// Token: 0x060008E1 RID: 2273 RVA: 0x0003DA36 File Offset: 0x0003BC36
	public void SetBuildingResourceOutputSetting(int blockIndex, BuildingResourceOutputSetting settings)
	{
		BuildingDomainMethod.Call.SetBuildingResourceOutputSetting(blockIndex, settings);
	}

	// Token: 0x060008E2 RID: 2274 RVA: 0x0003DA44 File Offset: 0x0003BC44
	public BuildingResourceOutputSetting GetBuildingShopEventSetting(int blockIndex)
	{
		BuildingResourceOutputSetting setting;
		bool flag = this.BuildingResourceOutputSettings.TryGetValue(blockIndex, out setting);
		BuildingResourceOutputSetting result;
		if (flag)
		{
			result = setting;
		}
		else
		{
			setting = new BuildingResourceOutputSetting();
			setting.Init();
			result = setting;
		}
		return result;
	}

	// Token: 0x060008E3 RID: 2275 RVA: 0x0003DA7C File Offset: 0x0003BC7C
	public int GetResourceCount(sbyte type)
	{
		bool flag = type < 0;
		int result;
		if (flag)
		{
			result = 0;
		}
		else
		{
			int count = 0;
			foreach (KeyValuePair<ItemSourceType, ResourceInts> keyValuePair in this.ResourceDict)
			{
				ItemSourceType itemSourceType;
				ResourceInts resourceInts;
				keyValuePair.Deconstruct(out itemSourceType, out resourceInts);
				ResourceInts resource = resourceInts;
				count += resource.Get((int)type);
			}
			result = count;
		}
		return result;
	}

	// Token: 0x060008E4 RID: 2276 RVA: 0x0003DB00 File Offset: 0x0003BD00
	public int GetResourceCount(ItemSourceType itemSourceType, sbyte type)
	{
		int count = 0;
		foreach (KeyValuePair<ItemSourceType, ResourceInts> keyValuePair in this.ResourceDict)
		{
			ItemSourceType itemSourceType2;
			ResourceInts resourceInts;
			keyValuePair.Deconstruct(out itemSourceType2, out resourceInts);
			ItemSourceType sourceType = itemSourceType2;
			ResourceInts resource = resourceInts;
			bool flag = itemSourceType == sourceType;
			if (flag)
			{
				count += resource.Get((int)type);
			}
		}
		return count;
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x0003DB84 File Offset: 0x0003BD84
	public void RefreshResources()
	{
		foreach (ItemSourceType itemSourceType in this.NeedItemSourceTypeList)
		{
			TaiwuDomainMethod.Call.GetAllResources(this._gameDataListenerId, itemSourceType);
		}
	}

	// Token: 0x060008E6 RID: 2278 RVA: 0x0003DBE0 File Offset: 0x0003BDE0
	private void OnTaiwuCharIdChange(ArgumentBox argumentBox)
	{
		this.RefreshResources();
		GEvent.OnEvent(UiEvents.OnForceRefreshAllMapBlock, null);
	}

	// Token: 0x060008E7 RID: 2279 RVA: 0x0003DBF8 File Offset: 0x0003BDF8
	public int GetTaiwuSpecialBuildingLevel(BuildingBlockKey blockKey)
	{
		BuildingBlockData dataEx;
		bool flag = this._buildingBlockDataDict.TryGetValue(blockKey, out dataEx);
		int result;
		if (flag)
		{
			result = (int)dataEx.CalcUnlockedLevelCount();
		}
		else
		{
			result = -1;
		}
		return result;
	}

	// Token: 0x060008E8 RID: 2280 RVA: 0x0003DC28 File Offset: 0x0003BE28
	public BuildingBlockData GetTaiwuBuildingData(BuildingBlockKey blockKey)
	{
		return this._buildingBlockDataDict.GetValueOrDefault(blockKey);
	}

	// Token: 0x060008E9 RID: 2281 RVA: 0x0003DC48 File Offset: 0x0003BE48
	public bool GetBuilding(short templateId, out BuildingBlockData buildingBlockData)
	{
		Location taiwuLocation = SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageBlock();
		foreach (KeyValuePair<BuildingBlockKey, BuildingBlockData> keyValuePair in this._buildingBlockDataDict)
		{
			BuildingBlockKey buildingBlockKey;
			BuildingBlockData buildingBlockData2;
			keyValuePair.Deconstruct(out buildingBlockKey, out buildingBlockData2);
			BuildingBlockKey i = buildingBlockKey;
			BuildingBlockData v = buildingBlockData2;
			short areaId = taiwuLocation.AreaId;
			short blockId = taiwuLocation.BlockId;
			short areaId2 = i.AreaId;
			short blockId2 = i.BlockId;
			bool flag;
			if (areaId == areaId2 && blockId == blockId2 && v.TemplateId == templateId)
			{
				sbyte operationType = v.OperationType;
				flag = (operationType != 0 && operationType != 1);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				buildingBlockData = v;
				return true;
			}
		}
		buildingBlockData = null;
		return false;
	}

	// Token: 0x060008EA RID: 2282 RVA: 0x0003DD1C File Offset: 0x0003BF1C
	public bool GetBuilding(short templateId, out BuildingBlockKey key, out BuildingBlockData data)
	{
		foreach (KeyValuePair<BuildingBlockKey, BuildingBlockData> keyValuePair in this._buildingBlockDataDict)
		{
			BuildingBlockKey buildingBlockKey;
			BuildingBlockData buildingBlockData;
			keyValuePair.Deconstruct(out buildingBlockKey, out buildingBlockData);
			BuildingBlockKey i = buildingBlockKey;
			BuildingBlockData v = buildingBlockData;
			bool flag;
			if (v.TemplateId == templateId)
			{
				sbyte operationType = v.OperationType;
				flag = (operationType != 0 && operationType != 1);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				key = i;
				data = v;
				return true;
			}
		}
		key = default(BuildingBlockKey);
		data = null;
		return false;
	}

	// Token: 0x060008EB RID: 2283 RVA: 0x0003DDC8 File Offset: 0x0003BFC8
	public sbyte GetBuildingLevel(BuildingBlockKey blockKey, BuildingBlockData blockData)
	{
		Location taiwuVillageLocation = SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageBlock();
		BuildingBlockItem config = BuildingBlock.Instance[blockData.TemplateId];
		bool flag = taiwuVillageLocation.AreaId != blockKey.AreaId || taiwuVillageLocation.BlockId != blockKey.BlockId;
		sbyte result;
		if (flag)
		{
			result = Math.Min(blockData.Level, config.MaxLevel);
		}
		else
		{
			bool flag2 = config.MaxLevel > 1;
			if (flag2)
			{
				bool flag3 = config.Type == EBuildingBlockType.UselessResource;
				if (flag3)
				{
					result = blockData.Level;
				}
				else
				{
					BuildingBlockData dataEx = this._buildingBlockDataDict.GetValueOrDefault(blockKey);
					result = dataEx.CalcUnlockedLevelCount();
				}
			}
			else
			{
				result = config.MaxLevel;
			}
		}
		return result;
	}

	// Token: 0x060008EC RID: 2284 RVA: 0x0003DE78 File Offset: 0x0003C078
	public sbyte GetTaiwuBuildingLevel(BuildingBlockKey blockKey)
	{
		BuildingBlockData blockData = this.GetTaiwuBuildingData(blockKey);
		return this.GetBuildingLevel(blockKey, blockData);
	}

	// Token: 0x060008ED RID: 2285 RVA: 0x0003DE9C File Offset: 0x0003C09C
	public List<int> GetBuildingShopManager(BuildingBlockKey blockKey)
	{
		CharacterList manager;
		bool flag = this._shopManagerDict.TryGetValue(blockKey, out manager);
		List<int> result;
		if (flag)
		{
			result = manager.GetCollection();
		}
		else
		{
			List<int> value = new List<int>();
			for (int i = 0; i < 7; i++)
			{
				value.Add(-1);
			}
			result = value;
		}
		return result;
	}

	// Token: 0x060008EE RID: 2286 RVA: 0x0003DEF4 File Offset: 0x0003C0F4
	public int GetBuildingShopManagerCount(BuildingBlockKey blockKey)
	{
		CharacterList manager;
		bool flag = this._shopManagerDict.TryGetValue(blockKey, out manager);
		int result;
		if (flag)
		{
			result = manager.GetRealCount();
		}
		else
		{
			result = 0;
		}
		return result;
	}

	// Token: 0x060008EF RID: 2287 RVA: 0x0003DF24 File Offset: 0x0003C124
	public void SetBuildingArrangementSettingPresetData(int index, BuildingOptionAutoGiveMemberPreset setting)
	{
		ExtraDomainMethod.Call.ChangeBuildingArrangementSettingPresetData(index, setting);
	}

	// Token: 0x060008F0 RID: 2288 RVA: 0x0003DF30 File Offset: 0x0003C130
	public BuildingOptionAutoGiveMemberPreset GetBuildingArrangementSettingPresetData(int index)
	{
		BuildingOptionAutoGiveMemberPreset value = this._buildingArrangementSettingPresetData[index];
		bool flag = value == null;
		if (flag)
		{
			value = new BuildingOptionAutoGiveMemberPreset();
		}
		return value;
	}

	// Token: 0x060008F1 RID: 2289 RVA: 0x0003DF5C File Offset: 0x0003C15C
	public void SetBuildingArrangementSetting(BuildingBlockKey blockKey, BuildingOptionAutoGiveMemberPreset setting)
	{
		BuildingBlockData dataEx = this._buildingBlockDataDict.GetValueOrDefault(blockKey);
		dataEx.ArrangementSetting = setting;
		BuildingDomainMethod.Call.SetBuildingArrangementSetting(blockKey, setting);
	}

	// Token: 0x060008F2 RID: 2290 RVA: 0x0003DF88 File Offset: 0x0003C188
	public BuildingOptionAutoGiveMemberPreset GetBuildingArrangementSetting(BuildingBlockKey blockKey, BuildingBlockData blockData)
	{
		BuildingBlockData dataEx = this._buildingBlockDataDict.GetValueOrDefault(blockKey);
		BuildingBlockData buildingBlockData = dataEx;
		if (buildingBlockData.ArrangementSetting == null)
		{
			buildingBlockData.ArrangementSetting = new BuildingOptionAutoGiveMemberPreset();
		}
		return dataEx.ArrangementSetting;
	}

	// Token: 0x060008F3 RID: 2291 RVA: 0x0003DFC4 File Offset: 0x0003C1C4
	public void SetBuildingSoldItemSetting(BuildingBlockKey blockKey, BuildingOptionAutoAddSoldItemPreset setting)
	{
		BuildingBlockData dataEx = this._buildingBlockDataDict.GetValueOrDefault(blockKey);
		dataEx.SoldItemSetting = setting;
		BuildingDomainMethod.Call.SetBuildingSoldItemSetting(blockKey, setting);
	}

	// Token: 0x060008F4 RID: 2292 RVA: 0x0003DFF0 File Offset: 0x0003C1F0
	public BuildingOptionAutoAddSoldItemPreset GetBuildingSoldItemSetting(BuildingBlockKey blockKey, BuildingBlockData blockData)
	{
		BuildingBlockData dataEx = this._buildingBlockDataDict.GetValueOrDefault(blockKey);
		BuildingBlockData buildingBlockData = dataEx;
		if (buildingBlockData.SoldItemSetting == null)
		{
			buildingBlockData.SoldItemSetting = new BuildingOptionAutoAddSoldItemPreset();
		}
		return dataEx.SoldItemSetting;
	}

	// Token: 0x060008F5 RID: 2293 RVA: 0x0003E02C File Offset: 0x0003C22C
	public sbyte GetStateTemplateIdByOrgTemplateId(sbyte orgTemplateId)
	{
		foreach (MapStateItem state in ((IEnumerable<MapStateItem>)MapState.Instance))
		{
			bool flag = state.SectID == orgTemplateId;
			if (flag)
			{
				return state.TemplateId;
			}
		}
		return -1;
	}

	// Token: 0x060008F6 RID: 2294 RVA: 0x0003E090 File Offset: 0x0003C290
	public sbyte GetOrgTemplateIdByStateTemplateId(sbyte stateTemplateId)
	{
		bool flag = stateTemplateId < 0 || (int)stateTemplateId >= MapState.Instance.Count;
		sbyte result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			result = MapState.Instance[stateTemplateId].SectID;
		}
		return result;
	}

	// Token: 0x060008F7 RID: 2295 RVA: 0x0003E0D4 File Offset: 0x0003C2D4
	public bool AreVitalsDemon()
	{
		return this._areVitalsDemon;
	}

	// Token: 0x060008F8 RID: 2296 RVA: 0x0003E0EC File Offset: 0x0003C2EC
	public bool YuanshanThreeVitalsExist()
	{
		List<SectStoryThreeVitalsCharacter> sectYuanshanThreeVitals = this._sectYuanshanThreeVitals;
		return sectYuanshanThreeVitals != null && sectYuanshanThreeVitals.Count > 0;
	}

	// Token: 0x060008F9 RID: 2297 RVA: 0x0003E114 File Offset: 0x0003C314
	public bool YuanshanThreeVitalsAllInPrison()
	{
		Tester.Assert(this.YuanshanThreeVitalsExist(), "");
		bool allInPrison = true;
		foreach (SectStoryThreeVitalsCharacter data in this._sectYuanshanThreeVitals)
		{
			bool flag = !data.IsInPrison;
			if (flag)
			{
				allInPrison = false;
				break;
			}
		}
		return allInPrison;
	}

	// Token: 0x060008FA RID: 2298 RVA: 0x0003E194 File Offset: 0x0003C394
	public bool YuanshanThreeVitalsAllNotAllowAsTeammate()
	{
		Tester.Assert(this.YuanshanThreeVitalsExist(), "");
		bool allowAsTeammate = false;
		foreach (SectStoryThreeVitalsCharacter data in this._sectYuanshanThreeVitals)
		{
			bool isInPrison = data.IsInPrison;
			if (!isInPrison)
			{
				bool flag = data.AllowAsTeammate(this._areVitalsDemon);
				if (flag)
				{
					allowAsTeammate = true;
					break;
				}
			}
		}
		return allowAsTeammate;
	}

	// Token: 0x060008FB RID: 2299 RVA: 0x0003E224 File Offset: 0x0003C424
	public SectStoryThreeVitalsCharacter GetThreeVitalsCharacterByType(SectStoryThreeVitalsCharacterType type)
	{
		foreach (SectStoryThreeVitalsCharacter data in this._sectYuanshanThreeVitals)
		{
			bool flag = data.VitalType == type;
			if (flag)
			{
				return data;
			}
		}
		return null;
	}

	// Token: 0x060008FC RID: 2300 RVA: 0x0003E28C File Offset: 0x0003C48C
	public List<int> GetYuanshanThreeVitalsIdList()
	{
		bool flag = this.YuanshanThreeVitalsDisplayData == null;
		List<int> result;
		if (flag)
		{
			result = new List<int>();
		}
		else
		{
			List<CharacterDisplayData> yuanshanThreeVitalsDisplayData = this.YuanshanThreeVitalsDisplayData;
			List<int> list;
			if (yuanshanThreeVitalsDisplayData == null)
			{
				list = null;
			}
			else
			{
				list = (from a in yuanshanThreeVitalsDisplayData
				select a.CharacterId).ToList<int>();
			}
			result = list;
		}
		return result;
	}

	// Token: 0x060008FD RID: 2301 RVA: 0x0003E2EC File Offset: 0x0003C4EC
	public List<int> GetAvailableYuanshanThreeVitalsIdList()
	{
		List<int> list = new List<int>();
		bool flag = this._sectYuanshanThreeVitals == null || this.YuanshanThreeVitalsDisplayData == null;
		List<int> result;
		if (flag)
		{
			result = list;
		}
		else
		{
			for (int index = 0; index < this._sectYuanshanThreeVitals.Count; index++)
			{
				SectStoryThreeVitalsCharacter data = this._sectYuanshanThreeVitals[index];
				bool flag2 = !data.AllowAsTeammate(this._areVitalsDemon);
				if (!flag2)
				{
					bool flag3 = !this.YuanshanThreeVitalsDisplayData.CheckIndex(index);
					if (!flag3)
					{
						list.Add(this.YuanshanThreeVitalsDisplayData[index].CharacterId);
					}
				}
			}
			result = list;
		}
		return result;
	}

	// Token: 0x060008FE RID: 2302 RVA: 0x0003E394 File Offset: 0x0003C594
	public List<int> GetYuanshanOppositeThreeVitalsIdList()
	{
		bool flag = this.YuanshanOppositeThreeVitalsDisplayData == null;
		List<int> result;
		if (flag)
		{
			result = new List<int>();
		}
		else
		{
			List<CharacterDisplayData> yuanshanOppositeThreeVitalsDisplayData = this.YuanshanOppositeThreeVitalsDisplayData;
			List<int> list;
			if (yuanshanOppositeThreeVitalsDisplayData == null)
			{
				list = null;
			}
			else
			{
				list = (from a in yuanshanOppositeThreeVitalsDisplayData
				select a.CharacterId).ToList<int>();
			}
			result = list;
		}
		return result;
	}

	// Token: 0x060008FF RID: 2303 RVA: 0x0003E3F4 File Offset: 0x0003C5F4
	public bool GetSectStoryThreeVitalsCharacterTypeById(int characterId, ref SectStoryThreeVitalsCharacterType characterType)
	{
		for (int i = 0; i < this.YuanshanThreeVitalsDisplayData.Count; i++)
		{
			CharacterDisplayData displayData = this.YuanshanThreeVitalsDisplayData[i];
			bool flag = characterId == displayData.CharacterId;
			if (flag)
			{
				foreach (object type in Enum.GetValues(typeof(SectStoryThreeVitalsCharacterType)))
				{
					short templateId = ((SectStoryThreeVitalsCharacterType)type).GetVitalTemplateId(this._areVitalsDemon);
					bool flag2 = templateId == displayData.TemplateId;
					if (flag2)
					{
						characterType = (SectStoryThreeVitalsCharacterType)type;
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06000900 RID: 2304 RVA: 0x0003E4D0 File Offset: 0x0003C6D0
	public bool ThreeVitalsAllowAsTeammate(int characterId)
	{
		SectStoryThreeVitalsCharacterType type = SectStoryThreeVitalsCharacterType.Earth;
		bool sectStoryThreeVitalsCharacterTypeById = this.GetSectStoryThreeVitalsCharacterTypeById(characterId, ref type);
		bool result;
		if (sectStoryThreeVitalsCharacterTypeById)
		{
			SectStoryThreeVitalsCharacter data = this.GetThreeVitalsCharacterByType(type);
			result = data.AllowAsTeammate(this._areVitalsDemon);
		}
		else
		{
			result = false;
		}
		return result;
	}

	// Token: 0x06000901 RID: 2305 RVA: 0x0003E50C File Offset: 0x0003C70C
	public short GetYuanshanThreeVitalsTemplateId(int id)
	{
		foreach (CharacterDisplayData data in this.YuanshanThreeVitalsDisplayData)
		{
			bool flag = data.CharacterId == id;
			if (flag)
			{
				return data.TemplateId;
			}
		}
		return -1;
	}

	// Token: 0x06000902 RID: 2306 RVA: 0x0003E578 File Offset: 0x0003C778
	public sbyte GetStateTaskStatus(int stateTemplateId)
	{
		int stateIndex = stateTemplateId - 1;
		return this._stateTaskStatuses.GetOrDefault(stateIndex, 0);
	}

	// Token: 0x06000903 RID: 2307 RVA: 0x0003E59C File Offset: 0x0003C79C
	public sbyte GetAreaTaskStatus(int areaTemplateId)
	{
		return this.GetStateTaskStatus((int)MapArea.Instance[areaTemplateId].StateID);
	}

	// Token: 0x06000904 RID: 2308 RVA: 0x0003E5C4 File Offset: 0x0003C7C4
	public sbyte GetOrgTaskStatus(sbyte orgTemplateId)
	{
		sbyte stateId = this.GetStateTemplateIdByOrgTemplateId(orgTemplateId);
		return this.GetStateTaskStatus((int)stateId);
	}

	// Token: 0x06000905 RID: 2309 RVA: 0x0003E5E8 File Offset: 0x0003C7E8
	public bool AreaIsTaskStatus(int areaTemplateId, sbyte expectStatus)
	{
		return this.GetAreaTaskStatus(areaTemplateId) == expectStatus;
	}

	// Token: 0x06000906 RID: 2310 RVA: 0x0003E604 File Offset: 0x0003C804
	public bool OrgIsTaskStatus(sbyte orgTemplateId, sbyte expectStatus)
	{
		return this.GetOrgTaskStatus(orgTemplateId) == expectStatus;
	}

	// Token: 0x04000BF7 RID: 3063
	private int _gameDataListenerId = -1;

	// Token: 0x04000BF8 RID: 3064
	public readonly Dictionary<ItemSourceType, ResourceInts> ResourceDict = new Dictionary<ItemSourceType, ResourceInts>();

	// Token: 0x04000BF9 RID: 3065
	private Dictionary<BuildingBlockKey, CharacterList> _shopManagerDict = new Dictionary<BuildingBlockKey, CharacterList>();

	// Token: 0x04000BFA RID: 3066
	public Dictionary<BuildingBlockKey, CharacterList> BuildingOperators = new Dictionary<BuildingBlockKey, CharacterList>();

	// Token: 0x04000BFB RID: 3067
	public Dictionary<BuildingBlockKey, sbyte> CollectBuildingResourceType = new Dictionary<BuildingBlockKey, sbyte>();

	// Token: 0x04000BFC RID: 3068
	public Dictionary<BuildingBlockKey, int> CustomBuildingName = new Dictionary<BuildingBlockKey, int>();

	// Token: 0x04000BFD RID: 3069
	public Dictionary<int, VillagerWorkData> VillagerWork = new Dictionary<int, VillagerWorkData>();

	// Token: 0x04000BFE RID: 3070
	public HashSetAsDictionary<Location> LocationMarkHashSet = new HashSetAsDictionary<Location>();

	// Token: 0x04000BFF RID: 3071
	public Dictionary<int, BuildingResourceOutputSetting> BuildingResourceOutputSettings = new Dictionary<int, BuildingResourceOutputSetting>();

	// Token: 0x04000C00 RID: 3072
	private Dictionary<BuildingBlockKey, BuildingBlockData> _buildingBlockDataDict = new Dictionary<BuildingBlockKey, BuildingBlockData>();

	// Token: 0x04000C01 RID: 3073
	private BuildingOptionAutoGiveMemberPreset[] _buildingArrangementSettingPresetData;

	// Token: 0x04000C02 RID: 3074
	private readonly sbyte[] _stateTaskStatuses = new sbyte[15];

	// Token: 0x04000C03 RID: 3075
	public readonly List<ItemSourceType> NeedItemSourceTypeList = new List<ItemSourceType>
	{
		ItemSourceType.Resources,
		ItemSourceType.Treasury
	};

	// Token: 0x04000C04 RID: 3076
	private List<SectStoryThreeVitalsCharacter> _sectYuanshanThreeVitals = new List<SectStoryThreeVitalsCharacter>();

	// Token: 0x04000C05 RID: 3077
	private bool _areVitalsDemon;

	// Token: 0x04000C06 RID: 3078
	public List<CharacterDisplayData> YuanshanThreeVitalsDisplayData = new List<CharacterDisplayData>();

	// Token: 0x04000C07 RID: 3079
	public List<CharacterDisplayData> YuanshanOppositeThreeVitalsDisplayData = new List<CharacterDisplayData>();
}
