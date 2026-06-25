using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CharacterDataMonitor;
using Config;
using FrameWork;
using Game.Views.Map;
using Game.Views.World;
using GameData.Common;
using GameData.DLC.FiveLoong;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Merchant;
using GameData.Domains.Organization.Display;
using GameData.Domains.Story;
using GameData.Domains.Story.MainStory;
using GameData.Domains.Story.SectMainStory;
using GameData.Domains.Taiwu;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000149 RID: 329
public class WorldMapModel : ISingletonInit, IDisposable
{
	// Token: 0x170001F8 RID: 504
	// (get) Token: 0x060011B6 RID: 4534 RVA: 0x0006B276 File Offset: 0x00069476
	[TupleElementNames(new string[]
	{
		"BlockId",
		"SettlementId",
		"AreaTemplateId"
	})]
	public ValueTuple<short, short, short> BrokenPerformAreaSettlementData
	{
		[return: TupleElementNames(new string[]
		{
			"BlockId",
			"SettlementId",
			"AreaTemplateId"
		})]
		get
		{
			return this._brokenPerformAreaSettlementData;
		}
	}

	// Token: 0x170001F9 RID: 505
	// (get) Token: 0x060011B7 RID: 4535 RVA: 0x0006B27E File Offset: 0x0006947E
	public MapBlockData CurrentBlockData
	{
		get
		{
			return this.GetBlockData(this.CurrentBlockId);
		}
	}

	// Token: 0x060011B8 RID: 4536 RVA: 0x0006B28C File Offset: 0x0006948C
	public bool IsHideCharacterSet()
	{
		return SingletonObject.getInstance<EventModel>().HideAllMapBlockCharacters;
	}

	// Token: 0x170001FA RID: 506
	// (get) Token: 0x060011B9 RID: 4537 RVA: 0x0006B298 File Offset: 0x00069498
	public bool IsTravellingEventPerforming
	{
		get
		{
			return this._travellingEventPerforming;
		}
	}

	// Token: 0x170001FB RID: 507
	// (get) Token: 0x060011BA RID: 4538 RVA: 0x0006B2A0 File Offset: 0x000694A0
	public bool IsMoveBanned
	{
		get
		{
			return this.MoveBanned > 0 || SingletonObject.getInstance<BasicGameData>().AdvancingMonthState != 0;
		}
	}

	// Token: 0x170001FC RID: 508
	// (get) Token: 0x060011BB RID: 4539 RVA: 0x0006B2BB File Offset: 0x000694BB
	public Location CurrentLocation
	{
		get
		{
			return new Location(this.CurrentAreaId, this.CurrentBlockId);
		}
	}

	// Token: 0x170001FD RID: 509
	// (get) Token: 0x060011BC RID: 4540 RVA: 0x0006B2CE File Offset: 0x000694CE
	public Location LastAtLocation
	{
		get
		{
			return this._lastUpdatedLocation;
		}
	}

	// Token: 0x170001FE RID: 510
	// (get) Token: 0x060011BD RID: 4541 RVA: 0x0006B2D8 File Offset: 0x000694D8
	public MapBlockData PlayerAtBlock
	{
		get
		{
			MapBlockData block;
			return (this.CurrentAreaId >= 0 && this.CurrentBlockId >= 0 && this.TryGetBlockData(this.CurrentLocation, out block)) ? block : null;
		}
	}

	// Token: 0x170001FF RID: 511
	// (get) Token: 0x060011BE RID: 4542 RVA: 0x0006B30B File Offset: 0x0006950B
	public MapBlockData SelectedBlock
	{
		get
		{
			return (this.SelectedBlockId >= 0) ? this.GetBlockData(this.SelectedBlockId) : null;
		}
	}

	// Token: 0x17000200 RID: 512
	// (get) Token: 0x060011BF RID: 4543 RVA: 0x0006B325 File Offset: 0x00069525
	public IReadOnlyList<CaravanDisplayData> CaravanData
	{
		get
		{
			return this._caravanData;
		}
	}

	// Token: 0x17000201 RID: 513
	// (get) Token: 0x060011C0 RID: 4544 RVA: 0x0006B32D File Offset: 0x0006952D
	public IReadOnlyList<Location> MovePath
	{
		get
		{
			return this._movePath;
		}
	}

	// Token: 0x17000202 RID: 514
	// (get) Token: 0x060011C1 RID: 4545 RVA: 0x0006B335 File Offset: 0x00069535
	public IReadOnlyDictionary<Location, HashSet<short>> BlockGroupDict
	{
		get
		{
			return this._blockGroupDict;
		}
	}

	// Token: 0x17000203 RID: 515
	// (get) Token: 0x060011C2 RID: 4546 RVA: 0x0006B33D File Offset: 0x0006953D
	public sbyte ConsummateLevel
	{
		get
		{
			return this._consummateLevel;
		}
	}

	// Token: 0x17000204 RID: 516
	// (get) Token: 0x060011C3 RID: 4547 RVA: 0x0006B345 File Offset: 0x00069545
	public bool AtPastTaiwuVillage
	{
		get
		{
			return this.CurrentAreaId == 139;
		}
	}

	// Token: 0x17000205 RID: 517
	// (get) Token: 0x060011C4 RID: 4548 RVA: 0x0006B354 File Offset: 0x00069554
	public bool AtSecretVillageAreaId
	{
		get
		{
			return this.CurrentAreaId == 137;
		}
	}

	// Token: 0x17000206 RID: 518
	// (get) Token: 0x060011C5 RID: 4549 RVA: 0x0006B363 File Offset: 0x00069563
	public bool AtChaishan
	{
		get
		{
			return this.CurrentAreaId == 140;
		}
	}

	// Token: 0x17000207 RID: 519
	// (get) Token: 0x060011C6 RID: 4550 RVA: 0x0006B372 File Offset: 0x00069572
	// (set) Token: 0x060011C7 RID: 4551 RVA: 0x0006B37A File Offset: 0x0006957A
	public WorldMapModel.MoveState TaiwuMoveState { get; private set; } = WorldMapModel.MoveState.Idle;

	// Token: 0x17000208 RID: 520
	// (get) Token: 0x060011C8 RID: 4552 RVA: 0x0006B383 File Offset: 0x00069583
	// (set) Token: 0x060011C9 RID: 4553 RVA: 0x0006B38B File Offset: 0x0006958B
	public WorldMapModel.EViewMode ViewMode { get; private set; } = WorldMapModel.EViewMode.Normal;

	// Token: 0x17000209 RID: 521
	// (get) Token: 0x060011CA RID: 4554 RVA: 0x0006B394 File Offset: 0x00069594
	// (set) Token: 0x060011CB RID: 4555 RVA: 0x0006B39C File Offset: 0x0006959C
	public Location TemporaryMarkLocation { get; private set; } = Location.Invalid;

	// Token: 0x1700020A RID: 522
	// (get) Token: 0x060011CC RID: 4556 RVA: 0x0006B3A5 File Offset: 0x000695A5
	// (set) Token: 0x060011CD RID: 4557 RVA: 0x0006B3AD File Offset: 0x000695AD
	public List<Location> TaskPanelMainMarkLocationList { get; private set; } = new List<Location>();

	// Token: 0x1700020B RID: 523
	// (get) Token: 0x060011CE RID: 4558 RVA: 0x0006B3B6 File Offset: 0x000695B6
	// (set) Token: 0x060011CF RID: 4559 RVA: 0x0006B3BE File Offset: 0x000695BE
	public List<Location> FindMapBlockMarkLocationList { get; private set; } = new List<Location>();

	// Token: 0x1700020C RID: 524
	// (get) Token: 0x060011D0 RID: 4560 RVA: 0x0006B3C7 File Offset: 0x000695C7
	// (set) Token: 0x060011D1 RID: 4561 RVA: 0x0006B3CF File Offset: 0x000695CF
	public HashSet<Location> FindBlockSet { get; private set; } = new HashSet<Location>();

	// Token: 0x060011D2 RID: 4562 RVA: 0x0006B3D8 File Offset: 0x000695D8
	public void Init()
	{
		GameApp.ClockAndLogInfo("Call WorldMapModel.Init", false);
		this._gameDataListenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyMapData));
		this._dispatcher = DispatcherUtils.RegisterDispatcher();
		this._lastUpdatedLocation = WorldMapModel.UninitializedLocation;
		this._isLocationUpdated = false;
		this.UpdateTaiwuCharId();
		for (int i = 0; i < this._blockDataIds.Length; i++)
		{
			this._blockDataIds[i] = ushort.MaxValue;
		}
		this.CurrentStateId = -1;
		this.CurrentAreaId = WorldMapModel.UninitializedLocation.AreaId;
		this.CurrentBlockId = WorldMapModel.UninitializedLocation.BlockId;
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 4, 0, (ulong)this._taiwuCharId, WorldMapModel.TaiwuObjectsFields);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 2, 46, ulong.MaxValue, uint.MaxValue);
		for (uint areaId = 0U; areaId < 141U; areaId += 1U)
		{
			GameDataBridge.AddDataMonitor(this._gameDataListenerId, 2, 0, (ulong)areaId, uint.MaxValue);
			GameDataBridge.AddDataMonitor(this._gameDataListenerId, 2, 53, (ulong)areaId, uint.MaxValue);
		}
		for (uint brokenIndex = 0U; brokenIndex < 90U; brokenIndex += 1U)
		{
			GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 44, (ulong)brokenIndex, uint.MaxValue);
		}
		for (int j = 0; j < 8; j++)
		{
			GameDataBridge.AddDataMonitor(this._gameDataListenerId, 2, 55, (ulong)((long)j), uint.MaxValue);
		}
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 2, 57, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 2, 60, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 2, 59, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 2, 64, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 2, 58, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 2, 61, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 2, 62, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 2, 63, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 2, 66, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 2, 65, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 5, 22, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 5, 51, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 5, 57, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 5, 54, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 5, 64, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 12, 6, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 12, 8, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 12, 7, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 43, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 45, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 61, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 63, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 62, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 71, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 133, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 156, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 78, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 95, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 111, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 145, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 148, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 202, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 203, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 20, 9, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 20, 14, ulong.MaxValue, uint.MaxValue);
		GEvent.Add(UiEvents.WorldMapShowInfoStatusChange, new GEvent.Callback(this.OnWorldMapShowInfoStatusChange));
		GEvent.Add(UiEvents.OnProfessionDataChange, new GEvent.Callback(this.OnProfessionChanged));
		GEvent.Add(UiEvents.OnProfessionSlotsChange, new GEvent.Callback(this.OnProfessionChanged));
		GEvent.Add(UiEvents.OnEventHandleComplete, new GEvent.Callback(this.OnHandleEventComplete));
		SingletonObject.getInstance<BuildingModel>();
	}

	// Token: 0x060011D3 RID: 4563 RVA: 0x0006B840 File Offset: 0x00069A40
	public void Dispose()
	{
		GEvent.Remove(UiEvents.WorldMapShowInfoStatusChange, new GEvent.Callback(this.OnWorldMapShowInfoStatusChange));
		GEvent.Remove(UiEvents.OnProfessionDataChange, new GEvent.Callback(this.OnProfessionChanged));
		GEvent.Remove(UiEvents.OnProfessionSlotsChange, new GEvent.Callback(this.OnProfessionChanged));
		GEvent.Remove(UiEvents.OnEventHandleComplete, new GEvent.Callback(this.OnHandleEventComplete));
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 12, 8, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 12, 7, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 12, 6, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 4, 0, (ulong)this._taiwuCharId, WorldMapModel.TaiwuObjectsFields);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 46, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 47, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 48, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 49, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 50, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 70, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 72, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 56, ulong.MaxValue, uint.MaxValue);
		for (uint areaId = 0U; areaId < 141U; areaId += 1U)
		{
			GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 0, (ulong)areaId, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 53, (ulong)areaId, uint.MaxValue);
		}
		for (uint brokenIndex = 0U; brokenIndex < 90U; brokenIndex += 1U)
		{
			GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 44, (ulong)brokenIndex, uint.MaxValue);
		}
		for (int i = 0; i < 8; i++)
		{
			GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 55, (ulong)((long)i), uint.MaxValue);
		}
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 57, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 60, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 59, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 64, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 58, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 61, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 62, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 63, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 66, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 65, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 5, 22, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 5, 51, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 5, 57, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 5, 54, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 5, 64, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 43, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 45, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 61, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 63, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 62, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 71, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 133, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 156, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 78, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 95, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 111, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 145, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 148, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 202, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 203, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 20, 9, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 20, 14, ulong.MaxValue, uint.MaxValue);
		for (int j = 0; j < this._blockDataIds.Length; j++)
		{
			bool flag = this._blockDataIds[j] != ushort.MaxValue;
			if (flag)
			{
				GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, this._blockDataIds[j], ulong.MaxValue, uint.MaxValue);
			}
		}
		GameDataBridge.UnregisterListener(this._gameDataListenerId);
		DispatcherUtils.UnregisterDispatcher(this._dispatcher);
		this._lastUpdatedLocation = Location.Invalid;
		this._isLocationUpdated = false;
		this._gameDataListenerId = -1;
		this._normalBlocks = null;
		this._dispatcher = null;
	}

	// Token: 0x060011D4 RID: 4564 RVA: 0x0006BD18 File Offset: 0x00069F18
	private void OnNotifyMapData(List<NotificationWrapper> notifications)
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
					ushort domainId = notification.DomainId;
					ushort num = domainId;
					if (num != 2)
					{
						if (num == 5)
						{
							this.HandlerMethodTaiwuDomain(wrapper, notification);
						}
					}
					else
					{
						this.HandlerMethodMapDomain(wrapper, notification);
					}
				}
			}
			else
			{
				DataUid uid = notification.Uid;
				ushort domainId2 = uid.DomainId;
				ushort num2 = domainId2;
				if (num2 <= 12)
				{
					switch (num2)
					{
					case 2:
						this.HandlerDataMapDomain(uid, wrapper, notification);
						break;
					case 3:
						break;
					case 4:
						this.HandlerDataCharacterDomain(uid, wrapper, notification);
						break;
					case 5:
						this.HandlerDataTaiwuDomain(uid, wrapper, notification);
						break;
					default:
						if (num2 == 12)
						{
							this.HandlerDataTaiwuEventDomain(uid, wrapper, notification);
						}
						break;
					}
				}
				else if (num2 != 19)
				{
					if (num2 == 20)
					{
						this.HandlerDataStoryDomain(uid, wrapper, notification);
					}
				}
				else
				{
					this.HandlerDataExtraDomain(uid, wrapper, notification);
				}
			}
		}
	}

	// Token: 0x060011D5 RID: 4565 RVA: 0x0006BE6C File Offset: 0x0006A06C
	private void OnTravelCheckableEvent(Enum em, ArgumentBox argsBox = null)
	{
		bool flag = UIElement.PartWorld.Exist && WorldMapModel.Traveling;
		if (flag)
		{
			ViewPartWorldMap partWorldMap = UIElement.PartWorld.UiBaseAs<ViewPartWorldMap>();
			partWorldMap.GEventsOnExit.RemoveAll((ValueTuple<Enum, ArgumentBox> p) => object.Equals(em, p.Item1));
			partWorldMap.GEventsOnExit.Add(new ValueTuple<Enum, ArgumentBox>(em, argsBox));
		}
		else
		{
			GEvent.OnEvent(em, argsBox);
		}
	}

	// Token: 0x060011D6 RID: 4566 RVA: 0x0006BEF0 File Offset: 0x0006A0F0
	private void OnWorldMapShowInfoStatusChange(ArgumentBox argsBox)
	{
		ViewWorldMap worldMap = UIElement.WorldMap.UiBaseAs<ViewWorldMap>();
		bool flag = worldMap != null;
		if (flag)
		{
			worldMap.StartCoroutine(worldMap.RefreshMapUi(true));
		}
		GEvent.OnEvent(UiEvents.WorldMapShowInfoBlockChange, EasyPool.Get<ArgumentBox>().SetObject("block", this.PlayerAtBlock));
	}

	// Token: 0x060011D7 RID: 4567 RVA: 0x0006BF44 File Offset: 0x0006A144
	private void OnProfessionChanged(ArgumentBox argbox)
	{
		int addingRange = 0;
		ProfessionModel professionModel = SingletonObject.getInstance<ProfessionModel>();
		bool flag = professionModel.IsProfessionalSkillUnlockedAndEquipped(45);
		if (flag)
		{
			addingRange += professionModel.GetProfessionData(11).GetSeniorityVisionRangeBonus();
		}
		bool flag2 = addingRange == this._professionAddingViewRange;
		if (!flag2)
		{
			this._professionAddingViewRange = addingRange;
			GEvent.OnEvent(UiEvents.OnMapBlockVisibilityNeedRefresh, null);
		}
	}

	// Token: 0x060011D8 RID: 4568 RVA: 0x0006BF9C File Offset: 0x0006A19C
	private void OnHandleEventComplete(ArgumentBox argsBox)
	{
		bool flag = this.TaiwuMoveState == WorldMapModel.MoveState.WaitEventShow;
		if (flag)
		{
			this.ChangeTaiwuMoveState(WorldMapModel.MoveState.Idle);
		}
	}

	// Token: 0x060011D9 RID: 4569 RVA: 0x0006BFC0 File Offset: 0x0006A1C0
	private void HandlerDataMapDomain(DataUid uid, NotificationWrapper wrapper, Notification notification)
	{
		int blockIndex = -1;
		ushort dataId = uid.DataId;
		ushort num = dataId;
		if (num != 0)
		{
			switch (num)
			{
			case 46:
			{
				bool flag = CommonUtils.GetSingleValueCollectionModificationsType(wrapper.DataPool, notification.ValueOffset) == 0;
				if (flag)
				{
					this._brokenBlocks = new AreaBlockCollection();
					Serializer.DeserializeModifications<short>(wrapper.DataPool, notification.ValueOffset, this._brokenBlocks);
					this._brokenBlocks.ConvertToRegularCollection();
					short blockId = 0;
					while ((int)blockId < this._brokenBlocks.Count)
					{
						this.UpdateBlockGroup(this._brokenBlocks[blockId]);
						blockId += 1;
					}
				}
				else
				{
					Serializer.DeserializeModifications<short>(wrapper.DataPool, notification.ValueOffset, this._brokenBlocks);
					HashSet<short> modifiedKeys = EasyPool.Get<HashSet<short>>();
					CommonUtils.GetModifiedSingleValueCollectionKeyOfClass<short, MapBlockData>(wrapper.DataPool, notification.ValueOffset, modifiedKeys);
					foreach (short modifiedKey in modifiedKeys)
					{
						this.UpdateBlockGroup(this._brokenBlocks[modifiedKey]);
					}
					bool flag2 = !WorldMapModel.Traveling;
					if (flag2)
					{
						this.HandlerDataMapBrokenAreaBlocks(modifiedKeys);
					}
					EasyPool.Free<HashSet<short>>(modifiedKeys);
				}
				goto IL_489;
			}
			case 47:
			case 48:
			case 49:
			case 50:
			case 70:
			case 72:
				blockIndex = 0;
				goto IL_489;
			case 53:
			{
				short areaId = (short)uid.SubId0;
				CricketPlaceData cache = null;
				bool flag3 = areaId == this.CurrentAreaId && this.CricketPlaceData[(int)areaId] != null;
				if (flag3)
				{
					cache = new CricketPlaceData(this.CricketPlaceData[(int)areaId]);
				}
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.CricketPlaceData[(int)areaId]);
				this.HandlerDataMapCricketChanged(areaId, cache);
				goto IL_489;
			}
			case 55:
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.SwordTombLocations[(int)(checked((IntPtr)uid.SubId0))]);
				goto IL_489;
			case 56:
			{
				bool flag4 = GameApp.Instance.GetCurrentGameStateName() == EGameState.Loading && SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(3);
				if (flag4)
				{
					CrossAreaMoveInfo travelInfo = new CrossAreaMoveInfo();
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref travelInfo);
					this.HandlerDataMapTravelLoaded(travelInfo);
				}
				goto IL_489;
			}
			case 57:
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._travellingEventPerforming);
				this.HandlerDataMapTravelEventChanged();
				goto IL_489;
			case 58:
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.HunterAnimals);
				this.HandlerDataMapCharacterChanged();
				goto IL_489;
			case 59:
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.MoveBanned);
				goto IL_489;
			case 60:
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.CrossArchiveLockMoveTime);
				this.HandlerDataMapCrossArchiveLockMoveTime();
				goto IL_489;
			case 61:
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.FleeBeasts);
				this.HandlerDataMapCharacterChanged();
				goto IL_489;
			case 62:
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.FleeLoongs);
				this.HandlerDataMapCharacterChanged();
				goto IL_489;
			case 63:
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.LoongLocations);
				this.HandlerDataMapCharacterChanged();
				goto IL_489;
			case 64:
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.AlterSettlementLocations);
				this.HandlerDataMapAlterSettlementLocationsChanged();
				goto IL_489;
			case 65:
			{
				bool result = false;
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref result);
				this.HandlerDataIsTaiwuInFulongFlameArea(result);
				goto IL_489;
			}
			case 66:
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._visibleMapPickups);
				this.HandlerVisiblePickupChanged();
				goto IL_489;
			}
			bool flag5 = this._blockDataIds.Exist(uid.DataId);
			if (flag5)
			{
				blockIndex = this._blockDataIds.IndexOf(uid.DataId);
			}
		}
		else
		{
			short areaId2 = (short)uid.SubId0;
			MapAreaData areaData = this.Areas[(int)areaId2];
			bool initAreaData = areaData == null;
			bool flag6 = initAreaData;
			if (flag6)
			{
				areaData = (this.Areas[(int)areaId2] = new MapAreaData());
			}
			Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref areaData);
			bool flag7 = initAreaData;
			if (flag7)
			{
				this.HandlerDataMapAreasInit(areaData, areaId2);
			}
			else
			{
				this.HandlerDataMapAreasChanged(areaData, areaId2);
			}
		}
		IL_489:
		bool flag8 = blockIndex >= 0;
		if (flag8)
		{
			this.HandlerDataMapDomainBlocks(wrapper, notification, blockIndex);
		}
	}

	// Token: 0x060011DA RID: 4570 RVA: 0x0006C480 File Offset: 0x0006A680
	private void HandlerDataMapDomainBlocks(NotificationWrapper wrapper, Notification notification, int blockIndex)
	{
		bool flag = CommonUtils.GetSingleValueCollectionModificationsType(wrapper.DataPool, notification.ValueOffset) == 0;
		if (flag)
		{
			this._normalBlocks[blockIndex] = new AreaBlockCollection();
			Serializer.DeserializeModifications<short>(wrapper.DataPool, notification.ValueOffset, this._normalBlocks[blockIndex]);
			this.HandlerDataMapBlocksInit(blockIndex);
		}
		else
		{
			Serializer.DeserializeModifications<short>(wrapper.DataPool, notification.ValueOffset, this._normalBlocks[blockIndex]);
			bool flag2 = !WorldMapModel.Traveling;
			if (flag2)
			{
				HashSet<short> modifiedKeys = EasyPool.Get<HashSet<short>>();
				CommonUtils.GetModifiedSingleValueCollectionKeyOfClass<short, MapBlockData>(wrapper.DataPool, notification.ValueOffset, modifiedKeys);
				this.HandlerDataMapBlocksModified(blockIndex, modifiedKeys);
				EasyPool.Free<HashSet<short>>(modifiedKeys);
			}
		}
	}

	// Token: 0x060011DB RID: 4571 RVA: 0x0006C52C File Offset: 0x0006A72C
	private void HandlerMethodMapDomain(NotificationWrapper wrapper, Notification notification)
	{
		ushort methodId = notification.MethodId;
		ushort num = methodId;
		if (num != 29)
		{
			if (num == 37)
			{
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.CurrentAreaBlockDisplayData);
				this.HandlerMethodGetBlockDisplayDataInArea();
			}
		}
		else
		{
			List<MapBlockData> blockDataList = null;
			Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref blockDataList);
			this.HandlerMethodGetMapBlockDataListOptional(blockDataList);
		}
	}

	// Token: 0x060011DC RID: 4572 RVA: 0x0006C594 File Offset: 0x0006A794
	private void HandlerDataCharacterDomain(DataUid uid, NotificationWrapper wrapper, Notification notification)
	{
		bool flag = uid.DataId == 0 && (int)uid.SubId0 == this._taiwuCharId;
		if (flag)
		{
			this.HandlerDataCharacterDomainTaiwu(uid, wrapper, notification);
		}
	}

	// Token: 0x060011DD RID: 4573 RVA: 0x0006C5CC File Offset: 0x0006A7CC
	private void HandlerDataCharacterDomainTaiwu(DataUid uid, NotificationWrapper wrapper, Notification notification)
	{
		uint subId = uid.SubId1;
		uint num = subId;
		if (num != 3U)
		{
			if (num != 28U)
			{
				switch (num)
				{
				case 55U:
				{
					Location location = default(Location);
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref location);
					this.HandlerDataCharacterLocation(location);
					break;
				}
				case 56U:
				{
					ItemKey[] equipments = null;
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref equipments);
					this.HandlerDataCharacterEquipments(equipments);
					break;
				}
				case 57U:
					this.HandlerDataCharacterInventory();
					break;
				}
			}
			else
			{
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._consummateLevel);
				Action<sbyte> consummateChanged = this.ConsummateChanged;
				if (consummateChanged != null)
				{
					consummateChanged(this._consummateLevel);
				}
			}
		}
		else
		{
			Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.TaiwuGender);
			this.HandlerDataCharacterGender();
		}
	}

	// Token: 0x060011DE RID: 4574 RVA: 0x0006C6B4 File Offset: 0x0006A8B4
	private void HandlerDataTaiwuDomain(DataUid uid, NotificationWrapper wrapper, Notification notification)
	{
		ushort dataId = uid.DataId;
		ushort num = dataId;
		if (num <= 51)
		{
			if (num != 22)
			{
				if (num == 51)
				{
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.VisitedSettlements);
					this.HandlerDataTaiwuVisitedSettlements();
				}
			}
			else
			{
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.MoveTimeCostPercent);
				this.HandlerDataTaiwuMoveTimeCostPercent();
			}
		}
		else if (num != 54)
		{
			if (num != 57)
			{
				if (num == 64)
				{
					Injuries injuries = default(Injuries);
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref injuries);
					this.HandlerDataTaiwuGroupWorstInjuries(injuries);
				}
			}
			else
			{
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.MarkLocationMaxCount);
			}
		}
		else
		{
			Serializer.DeserializeModifications<Location>(wrapper.DataPool, notification.ValueOffset, this.VillagerWorkLocations);
			HashSet<Location> modifiedKeys = EasyPool.Get<HashSet<Location>>();
			CommonUtils.GetModifiedSingleValueCollectionKeyOfStruct<Location, VoidValue>(wrapper.DataPool, notification.ValueOffset, modifiedKeys);
			this.HandlerDataTaiwuVillagerWorkLocations(modifiedKeys);
			EasyPool.Free<HashSet<Location>>(modifiedKeys);
		}
	}

	// Token: 0x060011DF RID: 4575 RVA: 0x0006C7C8 File Offset: 0x0006A9C8
	private void HandlerMethodTaiwuDomain(NotificationWrapper wrapper, Notification notification)
	{
		ushort methodId = notification.MethodId;
		ushort num = methodId;
		if (num == 79)
		{
			this.HandlerMethodTaiwuStopVillagerWork();
		}
	}

	// Token: 0x060011E0 RID: 4576 RVA: 0x0006C7F0 File Offset: 0x0006A9F0
	private void HandlerDataTaiwuEventDomain(DataUid uid, NotificationWrapper wrapper, Notification notification)
	{
		switch (uid.DataId)
		{
		case 6:
			this.HandlerDataTaiwuEventLocationChanged();
			break;
		case 7:
			Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.SecretVillageOnFire);
			this.HandlerDataTaiwuEventBlockChanged();
			break;
		case 8:
			Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.TaiwuVillageShowShrine);
			GEvent.OnEvent(UiEvents.TaiwuVillageShowShrineChange, null);
			this.HandlerDataTaiwuEventBlockChanged();
			break;
		}
	}

	// Token: 0x060011E1 RID: 4577 RVA: 0x0006C878 File Offset: 0x0006AA78
	private void HandlerDataExtraDomain(DataUid uid, NotificationWrapper wrapper, Notification notification)
	{
		ushort dataId = uid.DataId;
		ushort num = dataId;
		if (num <= 111)
		{
			if (num <= 71)
			{
				switch (num)
				{
				case 43:
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.SectXuehouBloodLightLocations);
					this.HandlerDataExtraSectXuehouBloodLightLocations();
					break;
				case 44:
				{
					int brokenIndex = (int)uid.SubId0;
					TreasureMaterialData temp = new TreasureMaterialData();
					TreasureMaterialData current = this.BrokenAreaMaterials[brokenIndex];
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref temp);
					this.HandlerDataExtraBrokenMaterial(brokenIndex, temp, current);
					break;
				}
				case 45:
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.SectWudangFairylandData);
					this.HandlerDataExtraSectWudangFairylandData();
					break;
				default:
					switch (num)
					{
					case 61:
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.SectWudangHeavenlyTreeList);
						this.HandlerDataExtraSectWudangHeavenlyTreeData();
						break;
					case 62:
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.SectWudangLingBaoDark);
						this.HandlerDataExtraSectWudangLingBaoDark();
						break;
					case 63:
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.SectWudangLingBaoLight);
						this.HandlerDataExtraSectWudangLingBaoLight();
						break;
					default:
						if (num == 71)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.SectEmeiBloodLocations);
							this.HandlerDataExtraSectEmeiBloodLocations();
						}
						break;
					}
					break;
				}
			}
			else if (num != 78)
			{
				if (num != 95)
				{
					if (num == 111)
					{
						Serializer.DeserializeModifications<int>(wrapper.DataPool, notification.ValueOffset, this.Animals);
						this.HandlerDataExtraAnimals();
					}
				}
				else
				{
					Serializer.DeserializeModifications<short>(wrapper.DataPool, notification.ValueOffset, this.FiveLoongDict);
					this.HandlerDataExtraFiveLoongDict();
				}
			}
			else
			{
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.DreamBackLocationData);
				this.HandlerDataExtraDreamBackLocationData();
			}
		}
		else if (num <= 148)
		{
			if (num != 133)
			{
				if (num != 145)
				{
					if (num == 148)
					{
						Serializer.DeserializeModifications<short>(wrapper.DataPool, notification.ValueOffset, this.CricketPlaceExtraData);
						this.HandlerDataExtraCricketPlaceExtraData();
					}
				}
				else
				{
					Serializer.DeserializeModifications<short>(wrapper.DataPool, notification.ValueOffset, this.AreaSpiritualDebt);
					this.HandlerDataExtraAreaSpiritualDebt();
				}
			}
			else
			{
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.SectFulongInFlameAreas);
				this.HandlerDataExtraSectFulongInFlameAreas();
			}
		}
		else if (num != 156)
		{
			if (num != 202)
			{
				if (num == 203)
				{
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.CustomMapBlockCharButtonList);
					this.HandlerCustomMapBlockCharButtonList();
				}
			}
			else
			{
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.CustomMapBlockCharInfoList);
				this.HandlerCustomMapBlockCharInfoList();
			}
		}
		else
		{
			Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.SectZhujianThiefList);
			this.HandlerDataExtraSectZhujianThiefList();
		}
	}

	// Token: 0x060011E2 RID: 4578 RVA: 0x0006CBA0 File Offset: 0x0006ADA0
	private void HandlerDataStoryDomain(DataUid uid, NotificationWrapper wrapper, Notification notification)
	{
		ushort dataId = uid.DataId;
		ushort num = dataId;
		if (num != 9)
		{
			if (num == 14)
			{
				List<SectEmeiGuidanceMapData> emeiData = null;
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref emeiData);
				this.HandlerEmeiGuidanceData(emeiData);
			}
		}
		else
		{
			IronPlateData ironPlateData = this.IronPlateData;
			bool isUnlocked = ironPlateData == null || ironPlateData.IsUnlocked;
			Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.IronPlateData);
			bool isNewUnlock = !isUnlocked && this.IronPlateData.IsUnlocked;
			this.HandlerDataIronPlateDataChanged(isNewUnlock);
			bool flag = this.IronPlateData.IsUnlocked && this.IronPlateData.FollowingCharId >= 0;
			if (flag)
			{
				StoryDomainMethod.AsyncCall.GetIronPlateCombatCharId(null, delegate(int offset, RawDataPool pool)
				{
					int combatCharId = 0;
					Serializer.Deserialize(pool, offset, ref combatCharId);
					CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, combatCharId, delegate(int offset, RawDataPool pool)
					{
						Serializer.Deserialize(pool, offset, ref this.IronPlateCombatCharData);
					});
				});
			}
		}
	}

	// Token: 0x060011E3 RID: 4579 RVA: 0x0006CC70 File Offset: 0x0006AE70
	private ItemDisplayData AnalysisMethodGetItemDisplayData(int offset, RawDataPool pool)
	{
		ItemDisplayData displayData = null;
		Serializer.Deserialize(pool, offset, ref displayData);
		return displayData;
	}

	// Token: 0x060011E4 RID: 4580 RVA: 0x0006CC90 File Offset: 0x0006AE90
	private void HandlerDataMapAreasInit(MapAreaData areaData, short areaId)
	{
		for (int i = 0; i < areaData.SettlementInfos.Length; i++)
		{
			SettlementInfo info = areaData.SettlementInfos[i];
			bool flag = info.RandomNameId >= 0 && !this.SettlementRandNameDict.ContainsKey(info.SettlementId);
			if (flag)
			{
				this.SettlementRandNameDict.Add(info.SettlementId, info.RandomNameId);
			}
		}
		bool flag2 = areaId == 138;
		if (flag2)
		{
			foreach (SettlementInfo info2 in areaData.SettlementInfos)
			{
				bool flag3 = info2.OrgTemplateId == 38;
				if (flag3)
				{
					this._brokenPerformAreaSettlementData = new ValueTuple<short, short, short>(info2.BlockId, info2.SettlementId, areaData.GetTemplateId());
				}
			}
		}
		bool flag4 = areaId != 140;
		if (!flag4)
		{
			this.MonitorTravelInfo();
			GEvent.OnEvent(UiEvents.WorldMapAreaDataInit, null);
			for (short j = 0; j < 141; j += 1)
			{
				this.AreaMapSize[j] = this.GetAreaSize(j);
			}
		}
	}

	// Token: 0x060011E5 RID: 4581 RVA: 0x0006CDC4 File Offset: 0x0006AFC4
	private void HandlerDataMapAreasChanged(MapAreaData areaData, short areaId)
	{
		bool discovered = areaData.Discovered;
		bool stationUnlocked = areaData.StationUnlocked;
		this.OnTravelCheckableEvent(UiEvents.WorldMapAreaDataChange, EasyPool.Get<ArgumentBox>().Set("AreaId", areaId).Set("DiscoveredChanged", !discovered && areaData.Discovered).Set("StationUnlockChanged", !stationUnlocked && areaData.StationUnlocked));
	}

	// Token: 0x060011E6 RID: 4582 RVA: 0x0006CE2C File Offset: 0x0006B02C
	private void HandlerDataMapBrokenAreaBlocks(HashSet<short> modifiedKeys)
	{
		foreach (short blockId in modifiedKeys)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.SetObject("Data", this._brokenBlocks[blockId]);
			this.LastBlockDataChangedLocation = new Location(this.CurrentAreaId, blockId);
			this.OnTravelCheckableEvent(UiEvents.WorldMapBlockDataChange, argBox);
		}
	}

	// Token: 0x060011E7 RID: 4583 RVA: 0x0006CEB8 File Offset: 0x0006B0B8
	private void HandlerDataMapBlocksInit(int index)
	{
		this.InitAreaBlocks(index);
	}

	// Token: 0x060011E8 RID: 4584 RVA: 0x0006CEC4 File Offset: 0x0006B0C4
	private void HandlerDataMapBlocksModified(int index, HashSet<short> modifiedKeys)
	{
		foreach (short blockId in modifiedKeys)
		{
			MapBlockData blockData = this._normalBlocks[index][blockId];
			this.UpdateBlockGroup(blockData);
		}
		foreach (short blockId2 in modifiedKeys)
		{
			MapBlockData blockData2 = this._normalBlocks[index][blockId2];
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.SetObject("Data", blockData2);
			this.LastBlockDataChangedLocation = new Location(this.CurrentAreaId, blockId2);
			this.OnTravelCheckableEvent(UiEvents.WorldMapBlockDataChange, argBox);
		}
	}

	// Token: 0x060011E9 RID: 4585 RVA: 0x0006CFAC File Offset: 0x0006B1AC
	private void HandlerDataMapCricketChanged(short areaId, CricketPlaceData cache)
	{
		bool flag = areaId != this.CurrentAreaId;
		if (!flag)
		{
			CricketPlaceData current = this.CricketPlaceData[(int)areaId];
			bool flag2 = cache != null && current != null && cache.CricketBlocks.Length == current.CricketBlocks.Length;
			if (flag2)
			{
				for (int i = 0; i < current.CricketBlocks.Length; i++)
				{
					this.CricketChangedInArea[current.CricketBlocks[i]] = cache.CricketBlocks[i];
				}
			}
			GEvent.OnEvent(UiEvents.CricketPlaceDataChange, null);
			this.CricketChangedInArea.Clear();
		}
	}

	// Token: 0x060011EA RID: 4586 RVA: 0x0006D044 File Offset: 0x0006B244
	private void HandlerDataExtraBrokenMaterial(int brokenIndex, TreasureMaterialData temp, TreasureMaterialData current)
	{
		this.BrokenAreaMaterials[brokenIndex] = temp;
		short areaId = (short)(brokenIndex + 45);
		bool flag = areaId == this.CurrentAreaId;
		if (flag)
		{
			List<Location> changedLocations = EasyPool.Get<List<Location>>();
			List<short> emptyTemplateId = EasyPool.Get<List<short>>();
			List<short> emptyTemplateId2 = EasyPool.Get<List<short>>();
			changedLocations.Clear();
			emptyTemplateId.Clear();
			emptyTemplateId2.Clear();
			bool flag2 = temp != null && temp.BlockMaterialTemplateIds != null;
			if (flag2)
			{
				emptyTemplateId.AddRange(temp.BlockMaterialTemplateIds.Keys);
			}
			bool flag3 = current != null && current.BlockMaterialTemplateIds != null;
			if (flag3)
			{
				emptyTemplateId2.AddRange(current.BlockMaterialTemplateIds.Keys);
			}
			foreach (short blockId in emptyTemplateId)
			{
				bool flag4 = !emptyTemplateId2.Contains(blockId);
				if (flag4)
				{
					changedLocations.Add(new Location(areaId, blockId));
				}
			}
			foreach (short blockId2 in emptyTemplateId2)
			{
				bool flag5 = !emptyTemplateId.Contains(blockId2);
				if (flag5)
				{
					changedLocations.Add(new Location(areaId, blockId2));
				}
			}
			GEvent.OnEvent(UiEvents.OnBrokenMaterialDataChange, EasyPool.Get<ArgumentBox>().SetObject("changedLocations", changedLocations));
			EasyPool.Free<List<Location>>(changedLocations);
			EasyPool.Free<List<short>>(emptyTemplateId);
			EasyPool.Free<List<short>>(emptyTemplateId2);
		}
	}

	// Token: 0x060011EB RID: 4587 RVA: 0x0006D1DC File Offset: 0x0006B3DC
	private void HandlerDataExtraSectXuehouBloodLightLocations()
	{
		GEvent.OnEvent(UiEvents.OnBloodLightLocationsChanged, null);
	}

	// Token: 0x060011EC RID: 4588 RVA: 0x0006D1F0 File Offset: 0x0006B3F0
	private void HandlerDataExtraSectWudangFairylandData()
	{
		GEvent.OnEvent(UiEvents.OnFairylandDataChanged, null);
	}

	// Token: 0x060011ED RID: 4589 RVA: 0x0006D204 File Offset: 0x0006B404
	private void HandlerDataExtraSectWudangHeavenlyTreeData()
	{
		GEvent.OnEvent(UiEvents.OnHeavenlyTreeDataChanged, null);
	}

	// Token: 0x060011EE RID: 4590 RVA: 0x0006D218 File Offset: 0x0006B418
	private void HandlerDataExtraSectWudangLingBaoLight()
	{
		GEvent.OnEvent(UiEvents.OnLingBaoLightLocationsChanged, null);
	}

	// Token: 0x060011EF RID: 4591 RVA: 0x0006D22C File Offset: 0x0006B42C
	private void HandlerDataExtraSectWudangLingBaoDark()
	{
		GEvent.OnEvent(UiEvents.OnLingBaoDarkLocationsChanged, null);
	}

	// Token: 0x060011F0 RID: 4592 RVA: 0x0006D240 File Offset: 0x0006B440
	private void HandlerDataExtraSectEmeiBloodLocations()
	{
		GEvent.OnEvent(UiEvents.OnBloodLocationsChanged, null);
	}

	// Token: 0x060011F1 RID: 4593 RVA: 0x0006D254 File Offset: 0x0006B454
	private void HandlerDataExtraSectFulongInFlameAreas()
	{
		GEvent.OnEvent(UiEvents.OnFulongInFlameAreasChanged, null);
	}

	// Token: 0x060011F2 RID: 4594 RVA: 0x0006D268 File Offset: 0x0006B468
	private void HandlerDataExtraSectZhujianThiefList()
	{
		using (List<SectStoryThiefData>.Enumerator enumerator = this.SectZhujianThiefList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				SectStoryThiefData thiefData = enumerator.Current;
				bool flag = thiefData.AreaId != this.CurrentAreaId;
				if (!flag)
				{
					SectStoryThiefData lastData = this._lastThiefData.FirstOrDefault((SectStoryThiefData x) => x.AreaId == thiefData.AreaId && x.CatchThiefTimes == thiefData.CatchThiefTimes);
					bool flag2 = lastData == null;
					if (!flag2)
					{
						for (int i = 0; i < thiefData.ThiefBlockIds.Count; i++)
						{
							this.ThiefChangedInArea[thiefData.ThiefBlockIds[i]] = lastData.ThiefBlockIds[i];
						}
					}
				}
			}
		}
		this._lastThiefData.Clear();
		this._lastThiefData.AddRange(from x in this.SectZhujianThiefList
		select new SectStoryThiefData(x));
		GEvent.OnEvent(UiEvents.OnZhujianThiefDataChanged, null);
		this.ThiefChangedInArea.Clear();
	}

	// Token: 0x060011F3 RID: 4595 RVA: 0x0006D3BC File Offset: 0x0006B5BC
	private void HandlerDataExtraDreamBackLocationData()
	{
		GEvent.OnEvent(UiEvents.OnDreamBackLocationsChanged, null);
	}

	// Token: 0x060011F4 RID: 4596 RVA: 0x0006D3D0 File Offset: 0x0006B5D0
	private void HandlerDataExtraFiveLoongDict()
	{
		GEvent.OnEvent(UiEvents.OnFiveLoongDictChanged, null);
	}

	// Token: 0x060011F5 RID: 4597 RVA: 0x0006D3E4 File Offset: 0x0006B5E4
	private void HandlerDataExtraAnimals()
	{
		bool hasChanged = false;
		Dictionary<int, short> prevData = new Dictionary<int, short>();
		Dictionary<short, List<int>> currentAreaAnimalData;
		bool flag = this.LocationAnimalMap.TryGetValue(this.CurrentAreaId, out currentAreaAnimalData);
		if (flag)
		{
			foreach (KeyValuePair<short, List<int>> keyValuePair in currentAreaAnimalData)
			{
				short num;
				List<int> list;
				keyValuePair.Deconstruct(out num, out list);
				short blockId = num;
				List<int> animalIds = list;
				foreach (int animalId in animalIds)
				{
					prevData[animalId] = -blockId;
				}
			}
		}
		this.LocationAnimalMap.Clear();
		foreach (KeyValuePair<int, GameData.Domains.Character.Animal> keyValuePair2 in this.Animals)
		{
			int num2;
			GameData.Domains.Character.Animal animal2;
			keyValuePair2.Deconstruct(out num2, out animal2);
			int animalId2 = num2;
			GameData.Domains.Character.Animal animal = animal2;
			bool flag2 = !this.LocationAnimalMap.ContainsKey(animal.Location.AreaId);
			if (flag2)
			{
				this.LocationAnimalMap[animal.Location.AreaId] = new Dictionary<short, List<int>>();
			}
			bool flag3 = !this.LocationAnimalMap[animal.Location.AreaId].ContainsKey(animal.Location.BlockId);
			if (flag3)
			{
				this.LocationAnimalMap[animal.Location.AreaId][animal.Location.BlockId] = new List<int>();
			}
			this.LocationAnimalMap[animal.Location.AreaId][animal.Location.BlockId].Add(animal.Id);
			bool flag4 = animal.Location.AreaId == this.CurrentAreaId;
			if (flag4)
			{
				short blockId2;
				bool flag5 = !prevData.TryGetValue(animalId2, out blockId2) || -blockId2 != animal.Location.BlockId;
				if (flag5)
				{
					hasChanged = true;
					this.LastAnimalDataChangedLocation = new Location(this.CurrentAreaId, animal.Location.BlockId);
				}
				prevData[animalId2] = animal.Location.BlockId;
			}
		}
		bool flag6 = !hasChanged;
		if (flag6)
		{
			foreach (short blockId3 in prevData.Values)
			{
				bool flag7 = blockId3 < 0;
				if (flag7)
				{
					hasChanged = true;
					break;
				}
			}
		}
		bool flag8 = hasChanged;
		if (flag8)
		{
			GEvent.OnEvent(UiEvents.AnimalPlaceDataChange, null);
		}
	}

	// Token: 0x060011F6 RID: 4598 RVA: 0x0006D708 File Offset: 0x0006B908
	private void HandlerDataExtraAreaSpiritualDebt()
	{
		GEvent.OnEvent(EEvents.OnAreaSpiritualDebtChange, null);
	}

	// Token: 0x060011F7 RID: 4599 RVA: 0x0006D719 File Offset: 0x0006B919
	private void HandlerDataExtraCricketPlaceExtraData()
	{
		GEvent.OnEvent(UiEvents.CricketPlaceDataChange, null);
	}

	// Token: 0x060011F8 RID: 4600 RVA: 0x0006D72A File Offset: 0x0006B92A
	private void HandlerCustomMapBlockCharInfoList()
	{
		GEvent.OnEvent(UiEvents.OnMapBlockCharCustomInfoChanged, null);
	}

	// Token: 0x060011F9 RID: 4601 RVA: 0x0006D73B File Offset: 0x0006B93B
	private void HandlerCustomMapBlockCharButtonList()
	{
		GEvent.OnEvent(UiEvents.OnMapBlockCharCustomButtonChanged, null);
	}

	// Token: 0x060011FA RID: 4602 RVA: 0x0006D74C File Offset: 0x0006B94C
	private void HandlerDataMapTravelLoaded(CrossAreaMoveInfo travelInfo)
	{
		Location fromLocation = new Location(travelInfo.FromAreaId, travelInfo.FromBlockId);
		bool flag = travelInfo.Traveling && this.CurrentLocation != fromLocation;
		if (flag)
		{
			this.UpdateCurrLocation(fromLocation);
			UIGroup stateMainWorld = UIElement.StateMainWorld;
			stateMainWorld.OnShowed = (Action)Delegate.Combine(stateMainWorld.OnShowed, new Action(delegate()
			{
				UIManager.Instance.ShowUI(UIElement.StatePartWorldMap, true);
			}));
			GEvent.OnEvent(EEvents.LoadingProgress, EasyPool.Get<ArgumentBox>().Set("Progress", 100));
		}
	}

	// Token: 0x060011FB RID: 4603 RVA: 0x0006D7E8 File Offset: 0x0006B9E8
	private void HandlerDataMapTravelEventChanged()
	{
		bool flag = !this._travellingEventPerforming;
		if (flag)
		{
			GEvent.OnEvent(UiEvents.OnTravelCheckPointProcessed, null);
		}
	}

	// Token: 0x060011FC RID: 4604 RVA: 0x0006D811 File Offset: 0x0006BA11
	private void HandlerDataMapCrossArchiveLockMoveTime()
	{
		GEvent.OnEvent(UiEvents.OnDreamBackStatusChanged, null);
	}

	// Token: 0x060011FD RID: 4605 RVA: 0x0006D825 File Offset: 0x0006BA25
	private void HandlerDataMapAlterSettlementLocationsChanged()
	{
		GEvent.OnEvent(UiEvents.OnMapAlterSettlementChanged, null);
	}

	// Token: 0x060011FE RID: 4606 RVA: 0x0006D839 File Offset: 0x0006BA39
	private void HandlerDataMapCharacterChanged()
	{
		GEvent.OnEvent(UiEvents.OnMapCharacterChanged, null);
	}

	// Token: 0x060011FF RID: 4607 RVA: 0x0006D850 File Offset: 0x0006BA50
	private void HandlerDataIsTaiwuInFulongFlameArea(bool res)
	{
		bool flag = !res;
		if (flag)
		{
			AudioManager.Instance.StopSound("SFX_fulongfire_loop");
		}
	}

	// Token: 0x06001200 RID: 4608 RVA: 0x0006D878 File Offset: 0x0006BA78
	private void HandlerMethodGetBlockDisplayDataInArea()
	{
		this.IndexedAreaBlockDisplayData.Clear();
		for (int index = 0; index < this.CurrentAreaBlockDisplayData.Count; index++)
		{
			MapBlockDisplayData displayData = this.CurrentAreaBlockDisplayData[index];
			this.IndexedAreaBlockDisplayData[displayData.TreasureExpect.Location] = index;
		}
		GEvent.OnEvent(UiEvents.OnMapBlockDisplayDataChanged, null);
	}

	// Token: 0x06001201 RID: 4609 RVA: 0x0006D8E4 File Offset: 0x0006BAE4
	private void HandlerMethodGetMapBlockDataListOptional(List<MapBlockData> blockDataList)
	{
		bool flag = blockDataList != null;
		if (flag)
		{
			foreach (MapBlockData blockData in blockDataList)
			{
				this.ExtraRequestedBlockData[blockData.GetLocation()] = blockData;
			}
		}
		GEvent.OnEvent(UiEvents.OnExtraMapBlockDataRequested, null);
	}

	// Token: 0x06001202 RID: 4610 RVA: 0x0006D95C File Offset: 0x0006BB5C
	private void HandlerDataCharacterLocation(Location location)
	{
		bool traveling = location.AreaId < 0 || location.BlockId < 0;
		bool flag = WorldMapModel.Traveling != traveling;
		if (flag)
		{
			WorldMapModel.Traveling = traveling;
			GEvent.OnEvent(UiEvents.OnRefreshBottomLifePanel, null);
		}
		bool flag2 = !WorldMapModel.Traveling;
		if (flag2)
		{
			this.UpdateCurrLocation(location);
		}
	}

	// Token: 0x06001203 RID: 4611 RVA: 0x0006D9BC File Offset: 0x0006BBBC
	private void HandlerDataCharacterGender()
	{
		GEvent.OnEvent(UiEvents.OnCharacterTaiwuGenderChanged, null);
	}

	// Token: 0x06001204 RID: 4612 RVA: 0x0006D9D0 File Offset: 0x0006BBD0
	private void HandlerDataCharacterInventory()
	{
		GEvent.OnEvent(UiEvents.OnCharacterTaiwuInventoryChanged, null);
	}

	// Token: 0x1700020D RID: 525
	// (get) Token: 0x06001205 RID: 4613 RVA: 0x0006D9E4 File Offset: 0x0006BBE4
	// (set) Token: 0x06001206 RID: 4614 RVA: 0x0006D9F0 File Offset: 0x0006BBF0
	public static bool UsingCarrierFirst
	{
		get
		{
			return SingletonObject.getInstance<GlobalSettings>().UsingCarrierFirst;
		}
		set
		{
			SingletonObject.getInstance<GlobalSettings>().UsingCarrierFirst = value;
		}
	}

	// Token: 0x06001207 RID: 4615 RVA: 0x0006DA00 File Offset: 0x0006BC00
	public void HandlerDataCharacterEquipments(ItemKey[] equipments)
	{
		sbyte[] carrierSlots = WorldMapModel.UsingCarrierFirst ? WorldMapModel.UsingCarrierFirstOrder : WorldMapModel.UsingCarrierLastOrder;
		sbyte[] bonusSlots = carrierSlots.Concat(new sbyte[]
		{
			8,
			9,
			10
		}).ToArray<sbyte>();
		ItemDomainMethod.AsyncCall.GetItemDisplayDataListOptional(this._dispatcher, (from idx in bonusSlots
		select equipments[(int)idx] into x
		where x.HasTemplate
		select x).ToList<ItemKey>(), delegate(int offset, RawDataPool pool)
		{
			List<ItemDisplayData> displayData = new List<ItemDisplayData>();
			Serializer.Deserialize(pool, offset, ref displayData);
			WorldMapModel <>4__this = this;
			ItemDisplayData itemDisplayData = displayData.FirstOrDefault((ItemDisplayData item) => item.Durability > 0 && item.Key.ItemType == 4);
			<>4__this.UpdateTaiwuCarrier((itemDisplayData != null) ? itemDisplayData.Key.TemplateId : -1);
			int totalBonusRate = 0;
			foreach (ItemDisplayData item2 in displayData)
			{
				bool flag = item2.Durability <= 0;
				if (!flag)
				{
					bool flag2 = item2.Key.ItemType == 4;
					if (flag2)
					{
						totalBonusRate += (int)Carrier.Instance[item2.Key.TemplateId].BaseExploreBonusRate;
					}
					else
					{
						bool flag3 = item2.Key.ItemType == 2;
						if (flag3)
						{
							totalBonusRate += (int)Accessory.Instance[item2.Key.TemplateId].BaseExploreBonusRate;
						}
					}
				}
			}
			this.TaiwuExploreBonusRate = totalBonusRate;
		});
	}

	// Token: 0x06001208 RID: 4616 RVA: 0x0006DAA6 File Offset: 0x0006BCA6
	private void HandlerDataTaiwuMoveTimeCostPercent()
	{
		GEvent.OnEvent(UiEvents.OnMoveTimeCostPercentChanged, null);
	}

	// Token: 0x06001209 RID: 4617 RVA: 0x0006DABA File Offset: 0x0006BCBA
	private void HandlerDataTaiwuVisitedSettlements()
	{
		GEvent.OnEvent(UiEvents.OnVisitedSettlementsChanged, null);
	}

	// Token: 0x0600120A RID: 4618 RVA: 0x0006DAD0 File Offset: 0x0006BCD0
	private void HandlerDataTaiwuVillagerWorkLocations(HashSet<Location> modifiedKeys)
	{
		foreach (Location location in modifiedKeys)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>().Set("AreaId", location.AreaId).Set("BlockId", location.BlockId).Set("HasWork", this.VillagerWorkLocations.Contains(location));
			GEvent.OnEvent(UiEvents.WorldMapVillagerWorkingLocationChange, argBox);
		}
	}

	// Token: 0x0600120B RID: 4619 RVA: 0x0006DB68 File Offset: 0x0006BD68
	private void HandlerDataTaiwuGroupWorstInjuries(Injuries injuries)
	{
		this.IsTaiwuGroupGetMaxLevelInjuries = false;
		for (sbyte bodyPart = 0; bodyPart < 7; bodyPart += 1)
		{
			ValueTuple<sbyte, sbyte> valueTuple = injuries.Get(bodyPart);
			sbyte outer = valueTuple.Item1;
			sbyte inner = valueTuple.Item2;
			bool flag = outer == 6 || inner == 6;
			if (flag)
			{
				this.IsTaiwuGroupGetMaxLevelInjuries = true;
				break;
			}
		}
	}

	// Token: 0x0600120C RID: 4620 RVA: 0x0006DBC0 File Offset: 0x0006BDC0
	private void HandlerDataIronPlateDataChanged(bool isNewUnlock)
	{
		ArgumentBox args = EasyPool.Get<ArgumentBox>().Set("isNewUnlock", isNewUnlock);
		GEvent.OnEvent(UiEvents.OnIronPlateDataChanged, args);
	}

	// Token: 0x0600120D RID: 4621 RVA: 0x0006DBF0 File Offset: 0x0006BDF0
	private void HandlerEmeiGuidanceData(List<SectEmeiGuidanceMapData> data)
	{
		this.SectEmeiGuidanceData.Clear();
		bool flag = data != null;
		if (flag)
		{
			foreach (SectEmeiGuidanceMapData d in data)
			{
				this.SectEmeiGuidanceData.TryAdd(d.Location, new List<SectEmeiGuidanceMapData>());
				this.SectEmeiGuidanceData[d.Location].Add(d);
			}
		}
		GEvent.OnEvent(UiEvents.OnEmeiGuidanceDataChanged, null);
	}

	// Token: 0x0600120E RID: 4622 RVA: 0x0006DC94 File Offset: 0x0006BE94
	private void HandlerMethodTaiwuStopVillagerWork()
	{
		GEvent.OnEvent(UiEvents.OnStopVillagerWork, null);
	}

	// Token: 0x0600120F RID: 4623 RVA: 0x0006DCA8 File Offset: 0x0006BEA8
	private void HandlerDataTaiwuEventLocationChanged()
	{
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
		{
			Location location = new Location(this.CurrentAreaId, this.CurrentBlockId);
			this.UpdateCurrLocation(location);
		});
	}

	// Token: 0x06001210 RID: 4624 RVA: 0x0006DCC4 File Offset: 0x0006BEC4
	private void HandlerDataTaiwuEventBlockChanged()
	{
		bool flag = this.ShowingAreaId < 0;
		if (!flag)
		{
			List<MapBlockData> blocks = EasyPool.Get<List<MapBlockData>>();
			this.GetAreaBlocks(this.ShowingAreaId, blocks);
			foreach (MapBlockData block in blocks)
			{
				bool flag2 = !block.GetConfig().TaiwuEventChangedBlock;
				if (!flag2)
				{
					ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
					argBox.SetObject("Data", block);
					this.LastBlockDataChangedLocation = new Location(this.CurrentAreaId, block.BlockId);
					this.OnTravelCheckableEvent(UiEvents.WorldMapBlockDataChange, argBox);
				}
			}
			EasyPool.Free<List<MapBlockData>>(blocks);
		}
	}

	// Token: 0x06001211 RID: 4625 RVA: 0x0006DD90 File Offset: 0x0006BF90
	private void HandlerVisiblePickupChanged()
	{
		this.VisibleMapPickupDict.Clear();
		bool flag = this._visibleMapPickups != null;
		if (flag)
		{
			foreach (MapElementPickupDisplayData pickupDisplayData in this._visibleMapPickups)
			{
				List<MapElementPickupDisplayData> group;
				bool flag2 = !this.VisibleMapPickupDict.TryGetValue(pickupDisplayData.Pickup.Location, out group);
				if (flag2)
				{
					group = new List<MapElementPickupDisplayData>();
					this.VisibleMapPickupDict[pickupDisplayData.Pickup.Location] = group;
				}
				group.Add(pickupDisplayData);
			}
		}
		GEvent.OnEvent(UiEvents.MapPickupDataChanged, null);
	}

	// Token: 0x06001212 RID: 4626 RVA: 0x0006DE58 File Offset: 0x0006C058
	private void UpdateTaiwuCarrier(short carrierTemplateId)
	{
		this.TaiwuCarrier = carrierTemplateId;
		GEvent.OnEvent(UiEvents.OnCharacterTaiwuCarrierChanged, null);
	}

	// Token: 0x06001213 RID: 4627 RVA: 0x0006DE73 File Offset: 0x0006C073
	private void MonitorTravelInfo()
	{
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 2, 56, ulong.MaxValue, uint.MaxValue);
	}

	// Token: 0x06001214 RID: 4628 RVA: 0x0006DE88 File Offset: 0x0006C088
	private void UpdateTaiwuCharId()
	{
		this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		this.TaiwuResources = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<ResourceMonitor>(this._taiwuCharId, false);
	}

	// Token: 0x06001215 RID: 4629 RVA: 0x0006DEB2 File Offset: 0x0006C0B2
	private void RequestTaiwuObjectsData()
	{
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 4, 0, (ulong)this._taiwuCharId, WorldMapModel.TaiwuObjectsFields);
		this.UpdateTaiwuCharId();
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 4, 0, (ulong)this._taiwuCharId, WorldMapModel.TaiwuObjectsFields);
	}

	// Token: 0x06001216 RID: 4630 RVA: 0x0006DEF0 File Offset: 0x0006C0F0
	private void RequestAreaBlockData(sbyte stateId, short areaId)
	{
		this.CurrentStateId = stateId;
		this._blockGroupDict.Clear();
		this._blockRootDict.Clear();
		for (int i = 0; i < this._blockDataIds.Length; i++)
		{
			bool flag = this._blockDataIds[i] != ushort.MaxValue;
			if (flag)
			{
				GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, this._blockDataIds[i], ulong.MaxValue, uint.MaxValue);
				this._blockDataIds[i] = ushort.MaxValue;
				this._normalBlocks[i] = null;
			}
		}
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 47, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 48, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 49, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 50, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 70, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 2, 72, ulong.MaxValue, uint.MaxValue);
		bool flag2 = stateId >= 0;
		if (flag2)
		{
			for (int j = 0; j < this._blockDataIds.Length; j++)
			{
				this._blockDataIds[j] = (ushort)((int)(1 + stateId * 3) + j);
				GameDataBridge.AddDataMonitor(this._gameDataListenerId, 2, this._blockDataIds[j], ulong.MaxValue, uint.MaxValue);
			}
		}
		else
		{
			GameDataBridge.AddDataMonitor(this._gameDataListenerId, 2, this.SpecialAreaIdToDataId(areaId), ulong.MaxValue, uint.MaxValue);
		}
	}

	// Token: 0x06001217 RID: 4631 RVA: 0x0006E05C File Offset: 0x0006C25C
	private ushort SpecialAreaIdToDataId(short areaId)
	{
		if (!true)
		{
		}
		ushort result;
		switch (areaId)
		{
		case 135:
			result = 47;
			break;
		case 136:
			result = 48;
			break;
		case 137:
			result = 49;
			break;
		case 138:
			result = 50;
			break;
		case 139:
			result = 70;
			break;
		case 140:
			result = 72;
			break;
		default:
			throw new ArgumentOutOfRangeException("areaId", areaId, null);
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06001218 RID: 4632 RVA: 0x0006E0D0 File Offset: 0x0006C2D0
	private void RequestMapBlockDisplayData()
	{
		bool flag = this.CurrentAreaId < 0;
		if (!flag)
		{
			MapDomainMethod.Call.GetBlockDisplayDataInArea(this._gameDataListenerId, this.CurrentAreaId);
		}
	}

	// Token: 0x06001219 RID: 4633 RVA: 0x0006E100 File Offset: 0x0006C300
	private void UpdateCurrLocation(Location location)
	{
		this._lastUpdatedLocation = new Location(this.CurrentAreaId, this.CurrentBlockId);
		bool flag = this._lastUpdatedLocation != WorldMapModel.UninitializedLocation;
		if (flag)
		{
			this._isLocationUpdated = true;
		}
		bool flag2 = location.AreaId != this.CurrentAreaId;
		if (flag2)
		{
			sbyte stateId = this.GetStateId(location.AreaId);
			this.CurrentAreaId = location.AreaId;
			this.CurrentBlockId = location.BlockId;
			this.ShowingAreaId = this.CurrentAreaId;
			this.OnTravelCheckableEvent(UiEvents.WorldMapPlayerAreaChange, null);
			this.OnTravelCheckableEvent(UiEvents.WorldMapPlayerBlockChange, null);
			bool flag3 = this.CurrentStateId != stateId || stateId < 0;
			if (flag3)
			{
				this.RequestAreaBlockData(stateId, location.AreaId);
			}
			else
			{
				this.OnTravelCheckableEvent(UiEvents.WorldMapEnterNewArea, EasyPool.Get<ArgumentBox>().Set("SameState", true));
			}
		}
		else
		{
			bool flag4 = location.BlockId != this.CurrentBlockId;
			if (flag4)
			{
				this.CurrentBlockId = location.BlockId;
				this.OnTravelCheckableEvent(UiEvents.WorldMapPlayerBlockChange, null);
			}
		}
	}

	// Token: 0x0600121A RID: 4634 RVA: 0x0006E224 File Offset: 0x0006C424
	private void InitAreaBlocks(int index)
	{
		this._normalBlocks[index].ConvertToRegularCollection();
		short blockId = 0;
		while ((int)blockId < this._normalBlocks[index].Count)
		{
			MapBlockData blockData = this._normalBlocks[index][blockId];
			this.UpdateBlockGroup(blockData);
			blockId += 1;
		}
		bool flag;
		if (this.CurrentStateId >= 0)
		{
			flag = !this._normalBlocks.Exist((AreaBlockCollection collection) => collection == null);
		}
		else
		{
			flag = true;
		}
		bool flag2 = flag;
		if (flag2)
		{
			this.OnTravelCheckableEvent(UiEvents.WorldMapEnterNewArea, EasyPool.Get<ArgumentBox>().Set("SameState", false));
			bool flag3 = GameApp.Instance.GetCurrentGameStateName() == EGameState.Loading;
			if (flag3)
			{
				UIManager.Instance.ChangeToUI(this.AtPastTaiwuVillage ? UIElement.PastTaiwuVillage : UIElement.StateMainWorld);
			}
		}
	}

	// Token: 0x0600121B RID: 4635 RVA: 0x0006E304 File Offset: 0x0006C504
	private void UpdateBlockGroup(MapBlockData blockData)
	{
		Location blockLocation = blockData.GetLocation();
		bool flag = blockData.RootBlockId >= 0;
		if (flag)
		{
			Location rootLocation = new Location(blockLocation.AreaId, blockData.RootBlockId);
			this._blockGroupDict.GetOrNew(rootLocation).Add(blockLocation.BlockId);
			this._blockRootDict[blockLocation] = rootLocation;
		}
		else
		{
			Location rootLocation2;
			bool flag2 = this._blockRootDict.TryGetValue(blockLocation, out rootLocation2);
			if (flag2)
			{
				this._blockGroupDict.GetOrNew(rootLocation2).Remove(blockLocation.BlockId);
				this._blockRootDict.Remove(blockLocation);
			}
		}
	}

	// Token: 0x0600121C RID: 4636 RVA: 0x0006E3A4 File Offset: 0x0006C5A4
	public void RequestStopVillagerWorkOnMap(Location location, bool stopOnly)
	{
		WorldMapModel.<>c__DisplayClass226_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.location = location;
		CS$<>8__locals1.stopOnly = stopOnly;
		this.<RequestStopVillagerWorkOnMap>g__Stop|226_0(10, ref CS$<>8__locals1);
		this.<RequestStopVillagerWorkOnMap>g__Stop|226_0(11, ref CS$<>8__locals1);
		this.<RequestStopVillagerWorkOnMap>g__Stop|226_0(12, ref CS$<>8__locals1);
		this.<RequestStopVillagerWorkOnMap>g__Stop|226_0(13, ref CS$<>8__locals1);
		this.<RequestStopVillagerWorkOnMap>g__Stop|226_0(14, ref CS$<>8__locals1);
	}

	// Token: 0x0600121D RID: 4637 RVA: 0x0006E402 File Offset: 0x0006C602
	public void RequestExtraMapBlockData(List<Location> locations, bool includeRoot = false, bool includeBelong = false)
	{
		MapDomainMethod.Call.GetMapBlockDataListOptional(this._gameDataListenerId, locations, includeRoot, includeBelong);
	}

	// Token: 0x0600121E RID: 4638 RVA: 0x0006E414 File Offset: 0x0006C614
	public void JumpToTemporaryMark(Location location, int charListTogKey)
	{
		bool flag = location.IsValid() && this.GetBlockData(location) != null && this.Areas[(int)location.AreaId].Discovered;
		if (flag)
		{
			this.TemporaryMarkLocation = location;
			GEvent.OnEvent(UiEvents.MapFocusLocationGrave, EasyPool.Get<ArgumentBox>().Set<Location>("location", location).Set("togKey", charListTogKey));
		}
	}

	// Token: 0x0600121F RID: 4639 RVA: 0x0006E484 File Offset: 0x0006C684
	public void ClearTemporaryMark()
	{
		bool flag = this.TemporaryMarkLocation.IsValid();
		if (flag)
		{
			Location location = this.TemporaryMarkLocation;
			this.TemporaryMarkLocation = Location.Invalid;
			GEvent.OnEvent(UiEvents.MapClearLocationTemporaryMark, EasyPool.Get<ArgumentBox>().Set<Location>("location", location));
		}
	}

	// Token: 0x06001220 RID: 4640 RVA: 0x0006E4DC File Offset: 0x0006C6DC
	public void AddLocationsToTemporaryMarkList(List<Location> locations)
	{
		List<Location> addedLocations = new List<Location>();
		addedLocations.Clear();
		foreach (Location location in locations)
		{
			bool flag = location.IsValid() && this.GetBlockData(location) != null && this.Areas[(int)location.AreaId].Discovered && !this.FindMapBlockMarkLocationList.Contains(location);
			if (flag)
			{
				this.FindMapBlockMarkLocationList.Add(location);
				addedLocations.Add(location);
			}
		}
		bool flag2 = addedLocations.Count > 0;
		if (flag2)
		{
			GEvent.OnEvent(UiEvents.MapAddLocationsToTemporaryMarkList, EasyPool.Get<ArgumentBox>().SetObject("locations", addedLocations));
		}
	}

	// Token: 0x06001221 RID: 4641 RVA: 0x0006E5BC File Offset: 0x0006C7BC
	public void ClearAllTemporaryMarkList()
	{
		bool flag = this.FindMapBlockMarkLocationList.Count > 0;
		if (flag)
		{
			this.FindMapBlockMarkLocationList.Clear();
			GEvent.OnEvent(UiEvents.MapClearAllTemporaryMarkList, null);
		}
	}

	// Token: 0x06001222 RID: 4642 RVA: 0x0006E5FC File Offset: 0x0006C7FC
	public void AddLocationsToTemporaryMarkListForTask(List<Location> locations)
	{
		List<Location> addedLocations = new List<Location>();
		addedLocations.Clear();
		foreach (Location location in locations)
		{
			bool flag = location.IsValid() && this.GetBlockData(location) != null && this.Areas[(int)location.AreaId].Discovered && !this.TaskPanelMainMarkLocationList.Contains(location);
			if (flag)
			{
				this.TaskPanelMainMarkLocationList.Add(location);
				addedLocations.Add(location);
			}
		}
		bool flag2 = addedLocations.Count > 0;
		if (flag2)
		{
			GEvent.OnEvent(UiEvents.MapAddLocationsToTemporaryMarkListForTask, EasyPool.Get<ArgumentBox>().SetObject("locations", addedLocations));
		}
	}

	// Token: 0x06001223 RID: 4643 RVA: 0x0006E6DC File Offset: 0x0006C8DC
	public void ClearAllTemporaryMarkListForTask()
	{
		bool flag = this.TaskPanelMainMarkLocationList.Count > 0;
		if (flag)
		{
			this.TaskPanelMainMarkLocationList.Clear();
			GEvent.OnEvent(UiEvents.MapClearAllTemporaryMarkListForTask, null);
		}
	}

	// Token: 0x06001224 RID: 4644 RVA: 0x0006E71C File Offset: 0x0006C91C
	public bool IsCurrentBlockMeetDistanceForHeavenTree(int distance)
	{
		MapBlockData blockData = this.CurrentBlockData;
		EMapBlockType type = MapBlock.Instance[blockData.TemplateId].Type;
		bool flag = type == EMapBlockType.Developed || type == EMapBlockType.City || type == EMapBlockType.Sect || type == EMapBlockType.Town || type == EMapBlockType.Station;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = this.SectWudangHeavenlyTreeList == null;
			if (flag2)
			{
				result = true;
			}
			else
			{
				List<MapBlockData> neighborBlocks = new List<MapBlockData>();
				this.GetNeighborList(blockData.AreaId, blockData.BlockId, neighborBlocks, distance, false);
				foreach (SectStoryHeavenlyTreeExtendable tree in this.SectWudangHeavenlyTreeList)
				{
					bool flag3 = tree.Location.AreaId != blockData.GetLocation().AreaId;
					if (!flag3)
					{
						MapBlockData block = this.GetBlockData(tree.Location);
						ByteCoordinate pos = block.GetBlockPos();
						bool flag4 = (int)blockData.GetManhattanDistanceToPosWithoutRoot(pos.X, pos.Y) < distance;
						if (flag4)
						{
							return false;
						}
					}
				}
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06001225 RID: 4645 RVA: 0x0006E84C File Offset: 0x0006CA4C
	public bool IsCurrentBlockMeetTypeForHeavenlyTree()
	{
		return this.CurrentBlockData.BlockSubType != EMapBlockSubType.Ruin && !this.CurrentBlockData.IsCityTown() && this.CurrentBlockData.BlockSubType != EMapBlockSubType.SwordTomb && this.CurrentBlockData.BlockType != EMapBlockType.Developed && this.CurrentBlockData.BlockSubType != EMapBlockSubType.Station && this.CurrentAreaId != 140 && !this.CurrentBlockIsLoongMapBlock();
	}

	// Token: 0x06001226 RID: 4646 RVA: 0x0006E8C0 File Offset: 0x0006CAC0
	public bool CurrentBlockIsLoongMapBlock()
	{
		return this.CurrentBlockData.BlockSubType == EMapBlockSubType.DLCLoong;
	}

	// Token: 0x06001227 RID: 4647 RVA: 0x0006E8E4 File Offset: 0x0006CAE4
	public int GetMoveCost(Location location)
	{
		bool flag = location.AreaId != this.CurrentAreaId;
		int result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			result = this._aStarMap.GetMoveCostActionPoint(location.BlockId);
		}
		return result;
	}

	// Token: 0x06001228 RID: 4648 RVA: 0x0006E920 File Offset: 0x0006CB20
	public void FindWay(Location location)
	{
		bool flag = location.AreaId != this.CurrentAreaId;
		if (flag)
		{
			throw new Exception(string.Format("FindWay to {0} must in current area {1}.", location, this.CurrentAreaId));
		}
		this.PathClear();
		this._aStarMap.FindWay(this._movePath, location.BlockId);
		this.PathDequeue();
	}

	// Token: 0x06001229 RID: 4649 RVA: 0x0006E98C File Offset: 0x0006CB8C
	public bool PathContains(Location location)
	{
		return this._movePath.Contains(location);
	}

	// Token: 0x0600122A RID: 4650 RVA: 0x0006E9AC File Offset: 0x0006CBAC
	public void PathDequeue()
	{
		bool flag = this._movePath.Count <= 0;
		if (!flag)
		{
			bool flag2 = this._movePath[0] != this.CurrentLocation;
			if (flag2)
			{
				PredefinedLog.Show(11, "Path dequeue in " + this.CurrentLocation.ToString() + ", path[0]=" + this._movePath[0].ToString());
			}
			else
			{
				this._movePath.RemoveAt(0);
			}
		}
	}

	// Token: 0x0600122B RID: 4651 RVA: 0x0006EA41 File Offset: 0x0006CC41
	public void PathClear()
	{
		this._movePath.Clear();
	}

	// Token: 0x0600122C RID: 4652 RVA: 0x0006EA50 File Offset: 0x0006CC50
	public int GetPathBlockMoveCost(Location location)
	{
		bool flag = !this.PathContains(location) || location == this.CurrentLocation;
		int result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			bool crossArchiveLockMoveTime = this.CrossArchiveLockMoveTime;
			if (crossArchiveLockMoveTime)
			{
				result = 0;
			}
			else
			{
				int moveCost = 0;
				for (int i = 0; i < this._movePath.Count; i++)
				{
					Location node = this._movePath[i];
					MapBlockData block = this.GetBlockData(node);
					moveCost += block.MoveCostActionPoint;
					bool flag2 = node == location;
					if (flag2)
					{
						break;
					}
				}
				result = moveCost;
			}
		}
		return result;
	}

	// Token: 0x0600122D RID: 4653 RVA: 0x0006EAE8 File Offset: 0x0006CCE8
	public int GetAnimalCount(Location location)
	{
		Dictionary<short, List<int>> animalAreaData;
		List<int> animalIds;
		return (this.LocationAnimalMap.TryGetValue(location.AreaId, out animalAreaData) && animalAreaData.TryGetValue(location.BlockId, out animalIds)) ? animalIds.Count : 0;
	}

	// Token: 0x0600122E RID: 4654 RVA: 0x0006EB28 File Offset: 0x0006CD28
	public string GetSettlementName(OrganizationInfo orgInfo)
	{
		short settlementId = orgInfo.SettlementId;
		bool flag = GameData.Domains.World.SharedMethods.SmallVillageXiangshu((short)orgInfo.OrgTemplateId, true);
		string result;
		if (flag)
		{
			result = LocalStringManager.Get(LanguageKey.LK_SmallVillage_InfectedOrgAnonymous);
		}
		else
		{
			bool flag2 = settlementId == this._brokenPerformAreaSettlementData.Item2;
			if (flag2)
			{
				result = LocalStringManager.Get(LanguageKey.LK_Stockade_InStory);
			}
			else
			{
				short randomNameIndex;
				bool flag3 = this.SettlementRandNameDict.TryGetValue(settlementId, out randomNameIndex);
				if (flag3)
				{
					result = LocalTownNames.Instance.TownNameCore[(int)randomNameIndex].Name;
				}
				else
				{
					bool flag4 = orgInfo.OrgTemplateId >= 0;
					if (flag4)
					{
						result = Organization.Instance[orgInfo.OrgTemplateId].Name;
					}
					else
					{
						result = string.Empty;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x0600122F RID: 4655 RVA: 0x0006EBD8 File Offset: 0x0006CDD8
	public string GetSettlementName(SettlementDisplayData settlementDisplayData)
	{
		short settlementId = (short)settlementDisplayData.SettlementId;
		bool flag = settlementId == this._brokenPerformAreaSettlementData.Item2 && settlementDisplayData.AreaTemplateId == this._brokenPerformAreaSettlementData.Item3;
		string result;
		if (flag)
		{
			result = LocalStringManager.Get(LanguageKey.LK_Stockade_InStory);
		}
		else
		{
			short randomNameIndex;
			bool flag2 = this.SettlementRandNameDict.TryGetValue(settlementId, out randomNameIndex);
			if (flag2)
			{
				result = LocalTownNames.Instance.TownNameCore[(int)randomNameIndex].Name;
			}
			else
			{
				bool flag3 = settlementDisplayData.OrgTemplateId >= 0;
				if (flag3)
				{
					result = Organization.Instance[settlementDisplayData.OrgTemplateId].Name;
				}
				else
				{
					result = string.Empty;
				}
			}
		}
		return result;
	}

	// Token: 0x06001230 RID: 4656 RVA: 0x0006EC80 File Offset: 0x0006CE80
	public int GetAreaSpiritualDebt(short areaId)
	{
		return this.AreaSpiritualDebt.GetValueOrDefault(areaId, 0);
	}

	// Token: 0x06001231 RID: 4657 RVA: 0x0006ECA0 File Offset: 0x0006CEA0
	public int GetCurrAreaSpiritualDebt()
	{
		return this.AreaSpiritualDebt.GetValueOrDefault(SingletonObject.getInstance<WorldMapModel>().CurrentBlockData.AreaId, 0);
	}

	// Token: 0x06001232 RID: 4658 RVA: 0x0006ECD0 File Offset: 0x0006CED0
	public static string GetFormatSpiritualDebt(int value, int targetValue = 0)
	{
		return CommonUtils.GetDisplayStringForNum(value, 100000).SetColor((value == 0) ? "pinkyellow" : ((value > targetValue) ? "brightblue" : "brightred"));
	}

	// Token: 0x06001233 RID: 4659 RVA: 0x0006ED0C File Offset: 0x0006CF0C
	public void OnMapInitFinish()
	{
		bool flag = GameApp.Instance.GetCurrentGameStateName() == EGameState.Loading;
		if (flag)
		{
			GEvent.OnEvent(EEvents.LoadingProgress, EasyPool.Get<ArgumentBox>().Set("Progress", 100));
		}
		GameApp.ClockAndLogInfo("Call WorldMapModel.OnMapInitFinish", false);
		GEvent.OnEvent(UiEvents.WorldMapInited, null);
		GEvent.OnEvent(EEvents.WorldDataReady, null);
		bool flag2 = this._isLocationUpdated && this._lastUpdatedLocation.AreaId != this.CurrentAreaId && this._lastUpdatedLocation.BlockId != this.CurrentBlockId;
		if (flag2)
		{
			this._lastUpdatedLocation.AreaId = this.CurrentAreaId;
			this._lastUpdatedLocation.BlockId = this.CurrentBlockId;
			MapDomainMethod.Call.MoveFinish(this._lastUpdatedLocation, new Location(this.CurrentAreaId, this.CurrentBlockId));
		}
	}

	// Token: 0x06001234 RID: 4660 RVA: 0x0006EDEC File Offset: 0x0006CFEC
	public int GetViewRange(Location taiwuLocation)
	{
		MapBlockData blockData;
		bool flag = !this.TryGetBlockData(taiwuLocation, out blockData);
		int result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			result = (int)blockData.GetConfig().ViewRange + this._professionAddingViewRange;
		}
		return result;
	}

	// Token: 0x06001235 RID: 4661 RVA: 0x0006EE24 File Offset: 0x0006D024
	public bool IsLocationShouldInSight(Location location)
	{
		bool flag = location.AreaId != this.CurrentAreaId;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			MapBlockData blockData;
			bool flag2 = !this.TryGetBlockData(location, out blockData);
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool flag3 = !blockData.Visible;
				if (flag3)
				{
					result = false;
				}
				else
				{
					bool flag4 = this.IsAnyWorkerOnMap(blockData);
					if (flag4)
					{
						result = true;
					}
					else
					{
						int viewRange = this.GetViewRange(this.CurrentLocation);
						bool flag5 = viewRange < 0;
						if (flag5)
						{
							result = false;
						}
						else
						{
							byte areaSize = this.GetAreaSize(this.CurrentAreaId);
							ByteCoordinate coordinate = WorldMapModel.IndexToCoordinate(this.CurrentBlockId, areaSize);
							byte distance = blockData.GetManhattanDistanceToPos(coordinate.X, coordinate.Y);
							result = ((int)distance <= viewRange);
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06001236 RID: 4662 RVA: 0x0006EEE8 File Offset: 0x0006D0E8
	public int GetAreaIndexInState(short areaId)
	{
		bool flag = areaId >= 135;
		int result;
		if (flag)
		{
			result = 0;
		}
		else
		{
			bool flag2 = areaId < 45;
			if (flag2)
			{
				result = (int)(areaId % 3);
			}
			else
			{
				result = (int)(3 + (areaId - 45) % 6);
			}
		}
		return result;
	}

	// Token: 0x06001237 RID: 4663 RVA: 0x0006EF24 File Offset: 0x0006D124
	public int GetStateAdventureCount(int stateId)
	{
		return (stateId >= 0) ? 9 : 1;
	}

	// Token: 0x06001238 RID: 4664 RVA: 0x0006EF40 File Offset: 0x0006D140
	public short GetAreaIdByStateIndex(int indexInState)
	{
		bool flag = this.CurrentStateId < 0;
		short result;
		if (flag)
		{
			result = this.CurrentAreaId;
		}
		else
		{
			bool flag2 = indexInState < 3;
			if (flag2)
			{
				result = (short)((int)(this.CurrentStateId * 3) + indexInState);
			}
			else
			{
				result = (short)((int)(45 + 6 * this.CurrentStateId) + indexInState - 3);
			}
		}
		return result;
	}

	// Token: 0x06001239 RID: 4665 RVA: 0x0006EF90 File Offset: 0x0006D190
	public short GetAreaIdByAreaTemplateId(short areaTemplateId)
	{
		foreach (MapAreaData area in this.Areas)
		{
			bool flag = area.GetTemplateId() == areaTemplateId;
			if (flag)
			{
				return area.GetId();
			}
		}
		return -1;
	}

	// Token: 0x0600123A RID: 4666 RVA: 0x0006EFD8 File Offset: 0x0006D1D8
	public byte GetAreaSize(short areaId)
	{
		bool flag = areaId == this.GetTaiwuVillageAreaId() || areaId == 139;
		byte result;
		if (flag)
		{
			result = (byte)GlobalConfig.Instance.TaiwuVillageForceAreaSize;
		}
		else
		{
			result = ((areaId < 45 || areaId >= 135) ? this.Areas[(int)areaId].GetConfig().Size : 5);
		}
		return result;
	}

	// Token: 0x0600123B RID: 4667 RVA: 0x0006F034 File Offset: 0x0006D234
	public void GetAreaBlocks(short areaId, List<MapBlockData> blocks)
	{
		blocks.Clear();
		bool flag = areaId < 45 || areaId >= 135;
		if (flag)
		{
			int index = (int)((areaId < 45) ? (areaId % 3) : 0);
			bool flag2 = this._normalBlocks[index] != null;
			if (flag2)
			{
				blocks.AddRange(this._normalBlocks[index].GetArray());
			}
		}
		else
		{
			int blockPerArea = 25;
			int blockIdBegin = (int)((short)(blockPerArea * (int)(areaId - 45)));
			for (int i = 0; i < blockPerArea; i++)
			{
				short index2 = (short)(blockIdBegin + i);
				bool flag3 = this._brokenBlocks[index2] != null;
				if (flag3)
				{
					blocks.Add(this._brokenBlocks[index2]);
				}
			}
		}
	}

	// Token: 0x0600123C RID: 4668 RVA: 0x0006F0EC File Offset: 0x0006D2EC
	public sbyte GetCurrentStateTemplateId()
	{
		short areaId = this.CurrentAreaId;
		bool flag = areaId < 0;
		sbyte result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			MapAreaData areaData = this.Areas[(int)areaId];
			result = areaData.GetConfig().StateID;
		}
		return result;
	}

	// Token: 0x0600123D RID: 4669 RVA: 0x0006F128 File Offset: 0x0006D328
	public short GetCurrentAreaTemplateId()
	{
		short areaId = this.CurrentAreaId;
		bool flag = areaId < 0;
		short result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			MapAreaData areaData = this.Areas[(int)areaId];
			result = areaData.GetTemplateId();
		}
		return result;
	}

	// Token: 0x0600123E RID: 4670 RVA: 0x0006F15C File Offset: 0x0006D35C
	public MapBlockData GetBlockData(short blockId)
	{
		return this.GetBlockData(new Location(this.CurrentAreaId, blockId));
	}

	// Token: 0x0600123F RID: 4671 RVA: 0x0006F180 File Offset: 0x0006D380
	public bool TryGetBlockData(Location location, out MapBlockData block)
	{
		block = this.GetBlockData(location);
		return block != null;
	}

	// Token: 0x06001240 RID: 4672 RVA: 0x0006F1A0 File Offset: 0x0006D3A0
	private bool IsAnyWorkerOnMap(MapBlockData blockData)
	{
		HashSet<int> characterSet = blockData.CharacterSet;
		bool flag = characterSet == null || characterSet.Count <= 0;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
			Dictionary<int, VillagerWorkData> villagerWork = buildingModel.VillagerWork;
			bool flag2 = villagerWork == null || villagerWork.Count <= 0;
			if (flag2)
			{
				result = false;
			}
			else
			{
				foreach (VillagerWorkData workData in buildingModel.VillagerWork.Values)
				{
					bool flag3 = !VillagerWorkType.IsWorkOnMap(workData.WorkType);
					if (!flag3)
					{
						bool flag4 = workData.AreaId != blockData.AreaId || workData.BlockId != blockData.BlockId;
						if (!flag4)
						{
							bool flag5 = blockData.CharacterSet.Contains(workData.CharacterId);
							if (flag5)
							{
								return true;
							}
						}
					}
				}
				result = false;
			}
		}
		return result;
	}

	// Token: 0x06001241 RID: 4673 RVA: 0x0006F2B0 File Offset: 0x0006D4B0
	public bool IsAreaHasDangerTips(short areaId)
	{
		return this.GetAreaSize(areaId) == 5;
	}

	// Token: 0x06001242 RID: 4674 RVA: 0x0006F2BC File Offset: 0x0006D4BC
	public bool IsAreaInCurrentState(short areaId)
	{
		return this.GetStateId(areaId) == this.CurrentStateId;
	}

	// Token: 0x06001243 RID: 4675 RVA: 0x0006F2D0 File Offset: 0x0006D4D0
	public bool IsLocationContainsMaterial(Location location)
	{
		bool flag = !MapAreaData.IsBrokenArea(location.AreaId);
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			TreasureMaterialData areaMaterials = this.BrokenAreaMaterials[(int)(location.AreaId - 45)];
			List<short> materials;
			bool flag2 = !areaMaterials.BlockMaterialTemplateIds.TryGetValue(location.BlockId, out materials);
			result = (!flag2 && materials.Count > 0);
		}
		return result;
	}

	// Token: 0x06001244 RID: 4676 RVA: 0x0006F334 File Offset: 0x0006D534
	public MapBlockData GetBlockData(Location location)
	{
		bool flag = this.IsAreaInCurrentState(location.AreaId) && MapAreaData.IsNormalArea(location.AreaId);
		if (flag)
		{
			int areaIndex = (int)((location.AreaId < 45) ? (location.AreaId % 3) : 0);
			MapBlockData block;
			bool flag2 = this._normalBlocks != null && this._normalBlocks.CheckIndex(areaIndex) && this._normalBlocks[areaIndex] != null && this._normalBlocks[areaIndex].TryGetValue(location.BlockId, out block);
			if (flag2)
			{
				return block;
			}
		}
		else
		{
			bool flag3 = MapAreaData.IsBrokenArea(location.AreaId);
			if (flag3)
			{
				int blockPerArea = 25;
				int blockIdBegin = (int)((short)(blockPerArea * (int)(location.AreaId - 45)));
				MapBlockData block2;
				bool flag4 = this._brokenBlocks != null && this._brokenBlocks.TryGetValue((short)(blockIdBegin + (int)location.BlockId), out block2);
				if (flag4)
				{
					return block2;
				}
			}
		}
		MapBlockData extraBlockData;
		return this.ExtraRequestedBlockData.TryGetValue(location, out extraBlockData) ? extraBlockData : null;
	}

	// Token: 0x06001245 RID: 4677 RVA: 0x0006F42D File Offset: 0x0006D62D
	public int GetBlockNameIndex(MapBlockData block, byte areaSize)
	{
		return block.GetBlockIndexInBigBlock(areaSize);
	}

	// Token: 0x06001246 RID: 4678 RVA: 0x0006F438 File Offset: 0x0006D638
	public string GetAreaName(short areaId)
	{
		MapAreaData areaData = this.Areas[(int)areaId];
		return areaData.GetConfig().Name;
	}

	// Token: 0x06001247 RID: 4679 RVA: 0x0006F460 File Offset: 0x0006D660
	public bool IsAtSecretVillage()
	{
		return this.CurrentAreaId == 137;
	}

	// Token: 0x06001248 RID: 4680 RVA: 0x0006F480 File Offset: 0x0006D680
	public string GetAreaIcon(short areaId)
	{
		MapAreaData areaData = this.Areas[(int)areaId];
		return areaData.GetConfig().BigMapIcon;
	}

	// Token: 0x06001249 RID: 4681 RVA: 0x0006F4A8 File Offset: 0x0006D6A8
	public string GetBlockName(short areaId, short blockId, short blockTemplateId, int nameIndex = -1)
	{
		MapAreaData areaData = this.Areas[(int)areaId];
		short settlementId = -1;
		int i = 0;
		int count = areaData.SettlementInfos.Length;
		while (i < count)
		{
			bool flag = blockId == areaData.SettlementInfos[i].BlockId;
			if (flag)
			{
				settlementId = areaData.SettlementInfos[i].SettlementId;
				break;
			}
			i++;
		}
		bool flag2 = areaId == 138 && blockTemplateId == 36;
		string result;
		if (flag2)
		{
			result = LocalStringManager.Get(LanguageKey.LK_Stockade_InStory);
		}
		else
		{
			bool flag3 = blockTemplateId < 0;
			if (flag3)
			{
				result = areaData.GetConfig().Name;
			}
			else
			{
				MapBlockItem configData = MapBlock.Instance[blockTemplateId];
				bool flag4 = configData.SubType != EMapBlockSubType.SwordTomb;
				if (flag4)
				{
					bool flag5 = settlementId >= 0 && this.SettlementRandNameDict.ContainsKey(settlementId);
					if (flag5)
					{
						return LocalTownNames.Instance.TownNameCore[(int)this.SettlementRandNameDict[settlementId]].Name;
					}
				}
				result = ((nameIndex < 0 || nameIndex >= configData.BlockNames.Length) ? configData.Name : configData.BlockNames[nameIndex]);
			}
		}
		return result;
	}

	// Token: 0x0600124A RID: 4682 RVA: 0x0006F5D8 File Offset: 0x0006D7D8
	public string GetBlockName(Location location)
	{
		MapBlockData mapBlockData = this.GetBlockData(location);
		int blockIndex = this.GetBlockNameIndex(mapBlockData, this.GetAreaSize(location.AreaId));
		return this.GetBlockName(mapBlockData.AreaId, mapBlockData.BlockId, mapBlockData.GetConfig().TemplateId, blockIndex);
	}

	// Token: 0x0600124B RID: 4683 RVA: 0x0006F628 File Offset: 0x0006D828
	public MapBlockData GetNeighbor(MapBlockData block, MoveDirection direction, bool needPassable = true)
	{
		byte mapSize = this.GetAreaSize(block.AreaId);
		ByteCoordinate pos = block.GetBlockPos();
		switch (direction)
		{
		case MoveDirection.Left:
		{
			bool flag = pos.X > 0;
			if (!flag)
			{
				return null;
			}
			pos.X -= 1;
			break;
		}
		case MoveDirection.Up:
		{
			bool flag2 = pos.Y < mapSize - 1;
			if (!flag2)
			{
				return null;
			}
			pos.Y += 1;
			break;
		}
		case MoveDirection.Right:
		{
			bool flag3 = pos.X < mapSize - 1;
			if (!flag3)
			{
				return null;
			}
			pos.X += 1;
			break;
		}
		case MoveDirection.Down:
		{
			bool flag4 = pos.Y > 0;
			if (!flag4)
			{
				return null;
			}
			pos.Y -= 1;
			break;
		}
		default:
			throw new Exception(string.Format("Invalid direction: {0}", direction));
		}
		MapBlockData neighborBlock = this.GetBlockData(new Location(block.AreaId, WorldMapModel.CoordinateToIndex(pos, mapSize)));
		return (!needPassable || neighborBlock.IsPassable()) ? neighborBlock : null;
	}

	// Token: 0x0600124C RID: 4684 RVA: 0x0006F750 File Offset: 0x0006D950
	public void GetNeighborList(short areaId, short blockId, List<MapBlockData> neighborBlocks, int maxSteps = 1, bool includeCenter = false)
	{
		byte areaSize = this.GetAreaSize(areaId);
		List<MapBlockData> areaBlocks = EasyPool.Get<List<MapBlockData>>();
		this.GetAreaBlocks(areaId, areaBlocks);
		MapBlockData centerBlock = areaBlocks[(int)blockId].GetRootBlock();
		ByteCoordinate blockPos = WorldMapModel.IndexToCoordinate(centerBlock.BlockId, areaSize);
		byte blockSize = centerBlock.GetConfig().Size;
		neighborBlocks.Clear();
		byte x = (byte)Math.Max((int)blockPos.X - maxSteps, 0);
		while ((int)x < Math.Min((int)(blockPos.X + blockSize) + maxSteps, (int)areaSize))
		{
			byte y = (byte)Math.Max((int)blockPos.Y - maxSteps, 0);
			while ((int)y < Math.Min((int)(blockPos.Y + blockSize) + maxSteps, (int)areaSize))
			{
				MapBlockData neighborBlock = areaBlocks[(int)WorldMapModel.CoordinateToIndex(new ByteCoordinate(x, y), areaSize)];
				bool flag = (includeCenter || neighborBlock.BlockId != centerBlock.BlockId) && (int)centerBlock.GetManhattanDistanceToPos(x, y) <= maxSteps && neighborBlock.TemplateId != 126 && !neighborBlocks.Contains(neighborBlock);
				if (flag)
				{
					neighborBlocks.Add(neighborBlock);
				}
				y += 1;
			}
			x += 1;
		}
		EasyPool.Free<List<MapBlockData>>(areaBlocks);
	}

	// Token: 0x0600124D RID: 4685 RVA: 0x0006F888 File Offset: 0x0006DA88
	public void GetNeighborListRaw(short areaId, short blockId, List<MapBlockData> neighborBlocks, int maxSteps = 1, bool includeCenter = false)
	{
		byte areaSize = this.GetAreaSize(areaId);
		List<MapBlockData> areaBlocks = EasyPool.Get<List<MapBlockData>>();
		this.GetAreaBlocks(areaId, areaBlocks);
		MapBlockData centerBlock = areaBlocks[(int)blockId];
		ByteCoordinate blockPos = WorldMapModel.IndexToCoordinate(centerBlock.BlockId, areaSize);
		byte blockSize = 0;
		neighborBlocks.Clear();
		byte x = (byte)Math.Max((int)blockPos.X - maxSteps, 0);
		while ((int)x < Math.Min((int)(blockPos.X + blockSize) + maxSteps, (int)areaSize))
		{
			byte y = (byte)Math.Max((int)blockPos.Y - maxSteps, 0);
			while ((int)y < Math.Min((int)(blockPos.Y + blockSize) + maxSteps, (int)areaSize))
			{
				MapBlockData neighborBlock = areaBlocks[(int)WorldMapModel.CoordinateToIndex(new ByteCoordinate(x, y), areaSize)];
				bool flag = Mathf.Abs((int)(x - blockPos.X)) + Mathf.Abs((int)(y - blockPos.Y)) > maxSteps;
				if (!flag)
				{
					bool flag2 = (includeCenter || neighborBlock.BlockId != centerBlock.BlockId) && neighborBlock.TemplateId != 126 && !neighborBlocks.Contains(neighborBlock);
					if (flag2)
					{
						neighborBlocks.Add(neighborBlock);
					}
				}
				y += 1;
			}
			x += 1;
		}
		EasyPool.Free<List<MapBlockData>>(areaBlocks);
	}

	// Token: 0x0600124E RID: 4686 RVA: 0x0006F9CC File Offset: 0x0006DBCC
	public bool IsEdgeBlock(Location location)
	{
		MapBlockData block = this.GetBlockData(location).GetRootBlock();
		MapBlockItem blockConfig = (block != null) ? block.GetConfig() : null;
		bool flag;
		if (block != null && block.TemplateId != 126)
		{
			byte? b = (blockConfig != null) ? new byte?(blockConfig.Size) : null;
			int? num = (b != null) ? new int?((int)b.GetValueOrDefault()) : null;
			int num2 = 1;
			flag = (num.GetValueOrDefault() > num2 & num != null);
		}
		else
		{
			flag = true;
		}
		bool flag2 = flag;
		bool result;
		if (flag2)
		{
			result = false;
		}
		else
		{
			byte areaSize = this.GetAreaSize(block.AreaId);
			ByteCoordinate pos = WorldMapModel.IndexToCoordinate(block.BlockId, areaSize);
			result = (pos.X == 0 || pos.X == areaSize - 1 || pos.Y == 0 || pos.Y == areaSize - 1 || !this.GetBlockData(new Location(block.AreaId, block.BlockId - 1)).IsPassable() || !this.GetBlockData(new Location(block.AreaId, block.BlockId + 1)).IsPassable() || !this.GetBlockData(new Location(block.AreaId, block.BlockId - (short)areaSize)).IsPassable() || !this.GetBlockData(new Location(block.AreaId, block.BlockId + (short)areaSize)).IsPassable());
		}
		return result;
	}

	// Token: 0x0600124F RID: 4687 RVA: 0x0006FB44 File Offset: 0x0006DD44
	public static short CoordinateToIndex(ByteCoordinate coordinateInArea, byte mapSize)
	{
		return (short)(coordinateInArea.X + coordinateInArea.Y * mapSize);
	}

	// Token: 0x06001250 RID: 4688 RVA: 0x0006FB68 File Offset: 0x0006DD68
	public static short PositionToIndex(byte x, byte y, byte mapSize)
	{
		return (short)(x + y * mapSize);
	}

	// Token: 0x06001251 RID: 4689 RVA: 0x0006FB80 File Offset: 0x0006DD80
	public static ByteCoordinate IndexToCoordinate(short blockIndex, byte mapSize)
	{
		return new ByteCoordinate((byte)(blockIndex % (short)mapSize), (byte)(blockIndex / (short)mapSize));
	}

	// Token: 0x06001252 RID: 4690 RVA: 0x0006FB9F File Offset: 0x0006DD9F
	public ByteCoordinate LocationToCoordinate(Location location)
	{
		return WorldMapModel.IndexToCoordinate(location.BlockId, this.GetAreaSize(location.AreaId));
	}

	// Token: 0x06001253 RID: 4691 RVA: 0x0006FBB8 File Offset: 0x0006DDB8
	public Location CoordinateToLocation(short areaId, byte x, byte y)
	{
		return new Location(areaId, WorldMapModel.PositionToIndex(x, y, this.GetAreaSize(areaId)));
	}

	// Token: 0x06001254 RID: 4692 RVA: 0x0006FBD0 File Offset: 0x0006DDD0
	public uint GetManhattanDistanceToPos(Location lhs, Location rhs)
	{
		bool flag = lhs.AreaId != rhs.AreaId;
		uint result;
		if (flag)
		{
			result = uint.MaxValue;
		}
		else
		{
			byte size = this.GetAreaSize(lhs.AreaId);
			ByteCoordinate posL = WorldMapModel.IndexToCoordinate(lhs.BlockId, size);
			ByteCoordinate posR = WorldMapModel.IndexToCoordinate(rhs.BlockId, size);
			result = (uint)posL.GetManhattanDistance(posR);
		}
		return result;
	}

	// Token: 0x06001255 RID: 4693 RVA: 0x0006FC30 File Offset: 0x0006DE30
	public sbyte GetStateId(short areaId)
	{
		return (sbyte)((areaId < 45) ? (areaId / 3) : ((areaId < 135) ? ((areaId - 45) / 6) : -1));
	}

	// Token: 0x06001256 RID: 4694 RVA: 0x0006FC60 File Offset: 0x0006DE60
	public sbyte GetStateTemplateIdByAreaId(short areaId)
	{
		MapAreaData areaData = this.Areas[(int)areaId];
		return areaData.GetConfig().StateID;
	}

	// Token: 0x06001257 RID: 4695 RVA: 0x0006FC88 File Offset: 0x0006DE88
	public short GetTaiwuVillageAreaId()
	{
		return (short)((SingletonObject.getInstance<BasicGameData>().TaiwuVillageStateTemplateId - 1) * 3 + 2);
	}

	// Token: 0x06001258 RID: 4696 RVA: 0x0006FCAC File Offset: 0x0006DEAC
	public Location GetTaiwuVillageBlock()
	{
		short areaId = this.GetTaiwuVillageAreaId();
		short blockId = this.GetTaiwuVillageSettlementInfo().BlockId;
		return new Location(areaId, blockId);
	}

	// Token: 0x06001259 RID: 4697 RVA: 0x0006FCD8 File Offset: 0x0006DED8
	private SettlementInfo GetTaiwuVillageSettlementInfo()
	{
		short areaId = this.GetTaiwuVillageAreaId();
		SettlementInfo[] settlementInfos = this.Areas[(int)areaId].SettlementInfos;
		foreach (SettlementInfo info in settlementInfos)
		{
			bool flag = info.OrgTemplateId == 16;
			if (flag)
			{
				return info;
			}
		}
		PredefinedLog.Show(11, string.Format("Could not find Taiwu Village by OrgTemplateId in area {0}, falling back to index 1.", areaId));
		return this.Areas[(int)areaId].SettlementInfos[1];
	}

	// Token: 0x0600125A RID: 4698 RVA: 0x0006FD60 File Offset: 0x0006DF60
	public SettlementInfo GetLocationOrganizationInfo(Location location)
	{
		MapAreaData mapAreaData = this.Areas[(int)location.AreaId];
		SettlementInfo settlementInfo = default(SettlementInfo);
		for (int i = 0; i < mapAreaData.SettlementInfos.Length; i++)
		{
			bool flag = mapAreaData.SettlementInfos[i].BlockId == location.BlockId;
			if (flag)
			{
				settlementInfo = mapAreaData.SettlementInfos[i];
				break;
			}
		}
		return settlementInfo;
	}

	// Token: 0x0600125B RID: 4699 RVA: 0x0006FDD4 File Offset: 0x0006DFD4
	public bool IsAtTaiwuVillage(short areaId = -1, short blockId = -1)
	{
		Location blockKey = this.GetTaiwuVillageBlock();
		return blockKey.AreaId == ((areaId >= 0) ? areaId : this.CurrentAreaId) && blockKey.BlockId == ((blockId >= 0) ? blockId : this.CurrentBlockId);
	}

	// Token: 0x0600125C RID: 4700 RVA: 0x0006FE1C File Offset: 0x0006E01C
	public short GetTaiwuVillageSettlementId()
	{
		return this.GetTaiwuVillageSettlementInfo().SettlementId;
	}

	// Token: 0x0600125D RID: 4701 RVA: 0x0006FE39 File Offset: 0x0006E039
	public void UpdateMainStoryBgm(string bgnName)
	{
		this._mainStoryBgmName = bgnName;
		this.UpdateBgm();
	}

	// Token: 0x0600125E RID: 4702 RVA: 0x0006FE4C File Offset: 0x0006E04C
	public void UpdateBgm()
	{
		bool flag = !string.IsNullOrEmpty(this._mainStoryBgmName);
		if (flag)
		{
			SingletonObject.getInstance<MusicPlayerModel>().SetIsMainStoryControlBgm(true);
			AudioManager.Instance.PlayMusic(this._mainStoryBgmName, 1f, 100, null);
		}
		else
		{
			SingletonObject.getInstance<MusicPlayerModel>().SetIsMainStoryControlBgm(false);
			MapBlockData block = this.GetBlockData(this.CurrentBlockId);
			string[] bgsNames = MapBlock.Instance[block.TemplateId].Bgs;
			bool isBuildingAreaOpen = UIManager.Instance.IsFocusElement(UIElement.BuildingArea);
			int blockIndex = this.GetBlockNameIndex(block, this.GetAreaSize(this.CurrentAreaId));
			string bgsName = AudioManager.DummyAudioName;
			bool flag2 = isBuildingAreaOpen;
			if (flag2)
			{
				bgsName = AudioManager.DummyAudioName;
			}
			else
			{
				bool flag3 = blockIndex >= 0 && blockIndex < bgsNames.Length;
				if (flag3)
				{
					bgsName = bgsNames[blockIndex];
				}
				else
				{
					bool flag4 = blockIndex < 0 && bgsNames.Length != 0;
					if (flag4)
					{
						bgsName = bgsNames.GetRandom<string>();
					}
				}
			}
			bool flag5 = AudioManager.Instance.GetPlayingAmbience() != bgsName;
			if (flag5)
			{
				bool flag6 = bgsName.IsNullOrEmpty();
				if (flag6)
				{
					AudioManager.Instance.PlayAmbience(AudioManager.DummyAudioName, 1f, 0);
				}
				else
				{
					AudioManager.Instance.PlayAmbience(bgsName, 1f, 100);
				}
			}
			MusicPlayerModel musicPlayerModel = SingletonObject.getInstance<MusicPlayerModel>();
			bool flag7 = musicPlayerModel.IsEnabled && musicPlayerModel.IsPlaying;
			if (!flag7)
			{
				bool flag8 = musicPlayerModel.IsEnabled && musicPlayerModel.IsPaused;
				if (flag8)
				{
					musicPlayerModel.ResumeMusic();
				}
				else
				{
					this.UpdateCurrBlockMusic(block);
				}
			}
		}
	}

	// Token: 0x0600125F RID: 4703 RVA: 0x0006FFDC File Offset: 0x0006E1DC
	private void UpdateCurrBlockMusic(MapBlockData blockData)
	{
		bool flag = blockData.BelongBlockId >= 0;
		if (flag)
		{
			blockData = this.GetBlockData(blockData.BelongBlockId);
		}
		MapBlockItem configData = MapBlock.Instance[blockData.TemplateId];
		string bgmName = configData.Bgm;
		bool flag2 = bgmName.IsNullOrEmpty();
		if (flag2)
		{
			bgmName = ((this.CurrentStateId >= 0 && this.CurrentAreaId != this.GetTaiwuVillageBlock().AreaId) ? MapState.Instance[(int)(this.CurrentStateId + 1)].Bgm : "main_fushixun");
		}
		bool flag3 = AudioManager.Instance.GetPlayingMusic() != bgmName;
		if (flag3)
		{
			AudioManager.Instance.PlayMusic(bgmName, 1f, 100, null);
		}
	}

	// Token: 0x06001260 RID: 4704 RVA: 0x0007008F File Offset: 0x0006E28F
	public void ResetTaiwuCharId()
	{
		this.RequestTaiwuObjectsData();
	}

	// Token: 0x06001261 RID: 4705 RVA: 0x0007009C File Offset: 0x0006E29C
	public bool IsCaravanExist(Location location)
	{
		for (int i = 0; i < this.CaravanData.Count; i++)
		{
			bool flag = this.CaravanData[i].PathInArea.GetCurrLocation() == location;
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001262 RID: 4706 RVA: 0x000700F0 File Offset: 0x0006E2F0
	public CaravanDisplayData GetCaravanData(int caravanId)
	{
		foreach (CaravanDisplayData displayData in this._caravanData)
		{
			bool flag = displayData.CaravanId == caravanId;
			if (flag)
			{
				return displayData;
			}
		}
		return null;
	}

	// Token: 0x06001263 RID: 4707 RVA: 0x00070158 File Offset: 0x0006E358
	public void SetCaravanData(List<CaravanDisplayData> data)
	{
		this._caravanData.Clear();
		bool flag = data != null;
		if (flag)
		{
			this._caravanData.AddRange(data);
		}
		GEvent.OnEvent(UiEvents.OnUpdateCaravanBlockCharData, EasyPool.Get<ArgumentBox>().SetObject("caravanList", data));
	}

	// Token: 0x06001264 RID: 4708 RVA: 0x000701A8 File Offset: 0x0006E3A8
	public bool IsHunterAnimalExist(Location location)
	{
		bool flag = this.HunterAnimals == null;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			foreach (HunterAnimalKey animalKey in this.HunterAnimals)
			{
				bool flag2 = animalKey.AreaId == location.AreaId && animalKey.BlockId == location.BlockId;
				if (flag2)
				{
					return true;
				}
			}
			result = false;
		}
		return result;
	}

	// Token: 0x06001265 RID: 4709 RVA: 0x00070238 File Offset: 0x0006E438
	public bool IsFleeAnimalExist(Location location)
	{
		return this.IsFleeBeastExist(location) || this.IsFleeLoongExist(location);
	}

	// Token: 0x06001266 RID: 4710 RVA: 0x00070260 File Offset: 0x0006E460
	public bool IsFleeBeastExist(Location location)
	{
		List<Location> fleeBeasts = this.FleeBeasts;
		return fleeBeasts != null && fleeBeasts.Any((Location x) => x == location);
	}

	// Token: 0x06001267 RID: 4711 RVA: 0x000702A0 File Offset: 0x0006E4A0
	public bool IsFleeLoongExist(Location location)
	{
		List<Location> fleeLoongs = this.FleeLoongs;
		return fleeLoongs != null && fleeLoongs.Any((Location x) => x == location);
	}

	// Token: 0x06001268 RID: 4712 RVA: 0x000702E0 File Offset: 0x0006E4E0
	public bool IsLoongExist(Location location)
	{
		List<LoongLocationData> loongLocations = this.LoongLocations;
		return loongLocations != null && loongLocations.Any((LoongLocationData x) => x.Location == location);
	}

	// Token: 0x06001269 RID: 4713 RVA: 0x0007031D File Offset: 0x0006E51D
	public IEnumerable<LoongLocationData> GetLoongLocationData(Location location)
	{
		bool flag = this.LoongLocations == null;
		if (flag)
		{
			yield break;
		}
		foreach (LoongLocationData locationData in this.LoongLocations)
		{
			bool flag2 = locationData.Location == location;
			if (flag2)
			{
				yield return locationData;
			}
			locationData = default(LoongLocationData);
		}
		List<LoongLocationData>.Enumerator enumerator = default(List<LoongLocationData>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x0600126A RID: 4714 RVA: 0x00070334 File Offset: 0x0006E534
	public bool IsHeavenlyTreeExist(Location location)
	{
		bool flag = this.SectWudangHeavenlyTreeList == null;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			foreach (SectStoryHeavenlyTreeExtendable heavenlyTree in this.SectWudangHeavenlyTreeList)
			{
				bool flag2 = heavenlyTree.Location == location;
				if (flag2)
				{
					return true;
				}
			}
			result = false;
		}
		return result;
	}

	// Token: 0x0600126B RID: 4715 RVA: 0x000703B0 File Offset: 0x0006E5B0
	public SectStoryHeavenlyTreeExtendable TryGetHeavenlyTreeData(Location location)
	{
		bool flag = this.SectWudangHeavenlyTreeList == null;
		SectStoryHeavenlyTreeExtendable result;
		if (flag)
		{
			result = null;
		}
		else
		{
			foreach (SectStoryHeavenlyTreeExtendable heavenlyTree in this.SectWudangHeavenlyTreeList)
			{
				bool flag2 = heavenlyTree.Location == location;
				if (flag2)
				{
					return heavenlyTree;
				}
			}
			result = null;
		}
		return result;
	}

	// Token: 0x0600126C RID: 4716 RVA: 0x0007042C File Offset: 0x0006E62C
	public bool IsLocationInFlameRender(Location location)
	{
		WorldMapModel.<>c__DisplayClass306_0 CS$<>8__locals1 = new WorldMapModel.<>c__DisplayClass306_0();
		CS$<>8__locals1.location = location;
		CS$<>8__locals1.<>4__this = this;
		List<FulongInFlameArea> sectFulongInFlameAreas = this.SectFulongInFlameAreas;
		bool flag = sectFulongInFlameAreas == null || sectFulongInFlameAreas.Count <= 0;
		return !flag && this.SectFulongInFlameAreas.Any(new Func<FulongInFlameArea, bool>(CS$<>8__locals1.<IsLocationInFlameRender>g__InFlame|0)) && this.GetManhattanDistanceToPos(CS$<>8__locals1.location, this.CurrentLocation) <= 1U;
	}

	// Token: 0x0600126D RID: 4717 RVA: 0x000704A8 File Offset: 0x0006E6A8
	public short GetTaiwuCharOnSettlement()
	{
		bool flag = this.CurrentAreaId >= 0 && this.CurrentBlockId >= 0;
		if (flag)
		{
			MapBlockData currentRootBlock = this.GetBlockData(this.CurrentBlockId).GetRootBlock();
			MapAreaData area = this.Areas[(int)this.CurrentAreaId];
			foreach (SettlementInfo settlementInfo in area.SettlementInfos)
			{
				bool flag2 = settlementInfo.BlockId == currentRootBlock.BlockId;
				if (flag2)
				{
					return settlementInfo.SettlementId;
				}
			}
		}
		return -1;
	}

	// Token: 0x1700020E RID: 526
	// (get) Token: 0x0600126E RID: 4718 RVA: 0x00070542 File Offset: 0x0006E742
	public bool IsTaiwuOnSettlement
	{
		get
		{
			return this.GetTaiwuCharOnSettlement() > 0;
		}
	}

	// Token: 0x0600126F RID: 4719 RVA: 0x00070550 File Offset: 0x0006E750
	public string GetFullBlockName(FullBlockName fullBlockName, bool includeState = true, bool includeArea = true, bool includeBelongBlock = true, bool includeBlock = true)
	{
		bool flag = fullBlockName.stateTemplateId < 0 || fullBlockName.areaTemplateId < 0 || fullBlockName.BlockData == null;
		string result;
		if (flag)
		{
			result = LocalStringManager.Get(LanguageKey.LK_Character_Location_Format_Invalid_3);
		}
		else
		{
			string finalStateName = string.Empty;
			string finalAreaName = string.Empty;
			string finalBelongBlockName = string.Empty;
			string finalBlockName = string.Empty;
			if (includeState)
			{
				MapStateItem stateConfig = MapState.Instance[fullBlockName.stateTemplateId];
				finalStateName = stateConfig.Name;
			}
			if (includeArea)
			{
				MapAreaItem areaConfig = MapArea.Instance[fullBlockName.areaTemplateId];
				finalAreaName = ((!finalStateName.IsNullOrEmpty()) ? ("-" + areaConfig.Name) : areaConfig.Name);
			}
			if (includeBelongBlock)
			{
				MapBlockData belongBlockData = fullBlockName.BelongBlockData;
				bool flag2 = belongBlockData != null;
				if (flag2)
				{
					finalBelongBlockName = ((!finalStateName.IsNullOrEmpty() || !finalAreaName.IsNullOrEmpty()) ? ("-" + this.GetBlockName(belongBlockData.AreaId, belongBlockData.BlockId, belongBlockData.TemplateId, -1)) : this.GetBlockName(belongBlockData.AreaId, belongBlockData.BlockId, belongBlockData.TemplateId, -1));
				}
			}
			if (includeBlock)
			{
				MapBlockData blockData = fullBlockName.BlockData;
				int nameIndex = this.GetBlockNameIndex(blockData, this.GetAreaSize(blockData.AreaId));
				string blockName = this.GetBlockName(blockData.AreaId, blockData.BlockId, blockData.TemplateId, nameIndex);
				string destroyedStr = blockData.Destroyed ? LocalStringManager.Get(LanguageKey.LK_Map_Block_Destroyed_Tip).SetColor("lightgrey") : string.Empty;
				finalBlockName = ((!finalStateName.IsNullOrEmpty() || !finalAreaName.IsNullOrEmpty() || !finalBelongBlockName.IsNullOrEmpty()) ? ("-" + blockName + destroyedStr) : (blockName + destroyedStr));
			}
			string title = finalStateName + finalAreaName + finalBelongBlockName + finalBlockName;
			result = (title.IsNullOrEmpty() ? LocalStringManager.Get(LanguageKey.LK_Character_Location_Format_Invalid_3) : title);
		}
		return result;
	}

	// Token: 0x06001270 RID: 4720 RVA: 0x0007074C File Offset: 0x0006E94C
	public string GetCurBlockName()
	{
		MapBlockData blockData = this.CurrentBlockData;
		int nameIndex = this.GetBlockNameIndex(blockData, this.GetAreaSize(blockData.AreaId));
		return this.GetBlockName(blockData.AreaId, blockData.BlockId, blockData.TemplateId, nameIndex);
	}

	// Token: 0x06001271 RID: 4721 RVA: 0x00070794 File Offset: 0x0006E994
	public bool ChangeTaiwuMoveState(WorldMapModel.MoveState newState)
	{
		switch (this.TaiwuMoveState)
		{
		case WorldMapModel.MoveState.Idle:
		{
			bool flag = newState == WorldMapModel.MoveState.SendMoveMessage || newState == WorldMapModel.MoveState.WaitEventShow || newState == WorldMapModel.MoveState.EscapeToAdjacentBlock || newState == WorldMapModel.MoveState.ProfessionTravelerSkill;
			if (flag)
			{
				this.TaiwuMoveState = newState;
				return true;
			}
			break;
		}
		case WorldMapModel.MoveState.SendMoveMessage:
		{
			bool flag2 = newState == WorldMapModel.MoveState.PerformMove;
			if (flag2)
			{
				this.TaiwuMoveState = newState;
				return true;
			}
			break;
		}
		case WorldMapModel.MoveState.PerformMove:
		{
			bool flag3 = newState == WorldMapModel.MoveState.Idle || newState == WorldMapModel.MoveState.WaitEventShow;
			if (flag3)
			{
				this.TaiwuMoveState = newState;
				return true;
			}
			break;
		}
		case WorldMapModel.MoveState.WaitEventShow:
		{
			bool flag4 = newState == WorldMapModel.MoveState.Idle;
			if (flag4)
			{
				this.TaiwuMoveState = newState;
				return true;
			}
			break;
		}
		case WorldMapModel.MoveState.EscapeToAdjacentBlock:
		{
			bool flag5 = newState == WorldMapModel.MoveState.Idle || newState == WorldMapModel.MoveState.PerformMove;
			if (flag5)
			{
				this.TaiwuMoveState = newState;
				return true;
			}
			break;
		}
		case WorldMapModel.MoveState.ProfessionTravelerSkill:
		{
			bool flag6 = newState == WorldMapModel.MoveState.Idle;
			if (flag6)
			{
				this.TaiwuMoveState = newState;
				return true;
			}
			break;
		}
		}
		return false;
	}

	// Token: 0x06001272 RID: 4722 RVA: 0x00070894 File Offset: 0x0006EA94
	public void ChangeViewMode(WorldMapModel.EViewMode mode)
	{
		this.ViewMode = mode;
		this.UpdateViewModeData();
	}

	// Token: 0x06001273 RID: 4723 RVA: 0x000708A8 File Offset: 0x0006EAA8
	public void UpdateViewModeData()
	{
		bool flag = this.ViewMode == WorldMapModel.EViewMode.Info;
		if (flag)
		{
			this.RequestMapBlockDisplayData();
		}
		else
		{
			this.CurrentAreaBlockDisplayData.Clear();
			this.IndexedAreaBlockDisplayData.Clear();
		}
	}

	// Token: 0x06001274 RID: 4724 RVA: 0x000708E8 File Offset: 0x0006EAE8
	public MapBlockDisplayData? TryGetViewModeData(Location location)
	{
		bool flag = !location.IsValid() || location.AreaId != this.CurrentAreaId;
		MapBlockDisplayData? result;
		if (flag)
		{
			result = null;
		}
		else
		{
			bool flag2 = this.IndexedAreaBlockDisplayData == null || this.CurrentAreaBlockDisplayData == null;
			if (flag2)
			{
				result = null;
			}
			else
			{
				int index;
				bool flag3 = !this.IndexedAreaBlockDisplayData.TryGetValue(location, out index);
				if (flag3)
				{
					result = null;
				}
				else
				{
					result = new MapBlockDisplayData?(this.CurrentAreaBlockDisplayData[index]);
				}
			}
		}
		return result;
	}

	// Token: 0x06001275 RID: 4725 RVA: 0x00070984 File Offset: 0x0006EB84
	public static bool IsSettlementBlock(MapBlockItem config)
	{
		return config.Type == EMapBlockType.City || config.Type == EMapBlockType.Sect || config.Type == EMapBlockType.Town;
	}

	// Token: 0x06001276 RID: 4726 RVA: 0x000709B4 File Offset: 0x0006EBB4
	public bool CurrentLocationHasCricket()
	{
		CricketPlaceExtraData cricketPlaceExtraData;
		short num;
		bool flag = this.CricketPlaceExtraData.TryGetValue(this.CurrentAreaId, out cricketPlaceExtraData) && cricketPlaceExtraData != null && cricketPlaceExtraData.ExtraMapUnits != null && cricketPlaceExtraData.ExtraMapUnits.TryGetValue(this.CurrentBlockId, out num);
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			CricketPlaceData cricketData = this.CricketPlaceData[(int)this.CurrentAreaId];
			bool flag2 = cricketData == null;
			if (flag2)
			{
				result = false;
			}
			else
			{
				int index = cricketData.CricketBlocks.IndexOf(this.CurrentBlockId);
				result = (index >= 0 && !cricketData.CricketTriggered[index]);
			}
		}
		return result;
	}

	// Token: 0x0600127C RID: 4732 RVA: 0x00070D11 File Offset: 0x0006EF11
	[CompilerGenerated]
	private void <RequestStopVillagerWorkOnMap>g__Stop|226_0(sbyte type, ref WorldMapModel.<>c__DisplayClass226_0 A_2)
	{
		TaiwuDomainMethod.Call.StopVillagerWorkOptional(this._gameDataListenerId, A_2.location.AreaId, A_2.location.BlockId, type, A_2.stopOnly);
	}

	// Token: 0x04000F35 RID: 3893
	public const int SwordTombLocationCount = 8;

	// Token: 0x04000F36 RID: 3894
	public const int MapLoadingStartProgress = 50;

	// Token: 0x04000F37 RID: 3895
	public const int MaxLoadMapBlockPerFrame = 30;

	// Token: 0x04000F38 RID: 3896
	public const string DefaultBgm = "main_fushixun";

	// Token: 0x04000F39 RID: 3897
	private static readonly Location UninitializedLocation = new Location(-22, -22);

	// Token: 0x04000F3A RID: 3898
	private static readonly uint[] TaiwuObjectsFields = new uint[]
	{
		55U,
		3U,
		57U,
		56U,
		28U
	};

	// Token: 0x04000F3B RID: 3899
	public Dictionary<short, Dictionary<short, List<int>>> LocationAnimalMap = new Dictionary<short, Dictionary<short, List<int>>>();

	// Token: 0x04000F3C RID: 3900
	private sbyte _consummateLevel;

	// Token: 0x04000F3D RID: 3901
	internal Action<sbyte> ConsummateChanged;

	// Token: 0x04000F3E RID: 3902
	public bool TaiwuVillageShowShrine;

	// Token: 0x04000F3F RID: 3903
	public bool SecretVillageOnFire;

	// Token: 0x04000F40 RID: 3904
	public bool CrossArchiveLockMoveTime;

	// Token: 0x04000F41 RID: 3905
	public int MoveBanned;

	// Token: 0x04000F42 RID: 3906
	public sbyte TaiwuGender;

	// Token: 0x04000F43 RID: 3907
	public short TaiwuCarrier;

	// Token: 0x04000F44 RID: 3908
	public int TaiwuExploreBonusRate;

	// Token: 0x04000F45 RID: 3909
	public int MoveTimeCostPercent;

	// Token: 0x04000F46 RID: 3910
	public bool IsTaiwuGroupGetMaxLevelInjuries;

	// Token: 0x04000F47 RID: 3911
	public short MarkLocationMaxCount;

	// Token: 0x04000F48 RID: 3912
	public HashSetAsDictionary<Location> VillagerWorkLocations = new HashSetAsDictionary<Location>();

	// Token: 0x04000F49 RID: 3913
	public List<short> VisitedSettlements;

	// Token: 0x04000F4A RID: 3914
	public List<Location> AlterSettlementLocations;

	// Token: 0x04000F4B RID: 3915
	public List<HunterAnimalKey> HunterAnimals;

	// Token: 0x04000F4C RID: 3916
	public List<Location> FleeBeasts;

	// Token: 0x04000F4D RID: 3917
	public List<Location> FleeLoongs;

	// Token: 0x04000F4E RID: 3918
	public List<LoongLocationData> LoongLocations;

	// Token: 0x04000F4F RID: 3919
	private List<MapElementPickupDisplayData> _visibleMapPickups;

	// Token: 0x04000F50 RID: 3920
	public readonly Dictionary<Location, List<MapElementPickupDisplayData>> VisibleMapPickupDict = new Dictionary<Location, List<MapElementPickupDisplayData>>();

	// Token: 0x04000F51 RID: 3921
	public readonly Dictionary<Location, Queue<MapElementPickupDisplayData>> MapPickupEffectQueueDict = new Dictionary<Location, Queue<MapElementPickupDisplayData>>();

	// Token: 0x04000F52 RID: 3922
	public readonly HashSetAsDictionary<Location> CricketWishEffectLocations = new HashSetAsDictionary<Location>();

	// Token: 0x04000F53 RID: 3923
	public List<Location> SectXuehouBloodLightLocations;

	// Token: 0x04000F54 RID: 3924
	public List<SectStoryFairyland> SectWudangFairylandData;

	// Token: 0x04000F55 RID: 3925
	public List<SectStoryHeavenlyTreeExtendable> SectWudangHeavenlyTreeList;

	// Token: 0x04000F56 RID: 3926
	public List<Location> SectWudangLingBaoLight;

	// Token: 0x04000F57 RID: 3927
	public List<Location> SectWudangLingBaoDark;

	// Token: 0x04000F58 RID: 3928
	public List<Location> SectEmeiBloodLocations;

	// Token: 0x04000F59 RID: 3929
	public List<DreamBackLocationData> DreamBackLocationData;

	// Token: 0x04000F5A RID: 3930
	public List<FulongInFlameArea> SectFulongInFlameAreas = new List<FulongInFlameArea>();

	// Token: 0x04000F5B RID: 3931
	public Dictionary<Location, List<SectEmeiGuidanceMapData>> SectEmeiGuidanceData = new Dictionary<Location, List<SectEmeiGuidanceMapData>>();

	// Token: 0x04000F5C RID: 3932
	public List<SectStoryThiefData> SectZhujianThiefList = new List<SectStoryThiefData>();

	// Token: 0x04000F5D RID: 3933
	public readonly Location[] SwordTombLocations = new Location[8];

	// Token: 0x04000F5E RID: 3934
	public readonly MapAreaData[] Areas = new MapAreaData[141];

	// Token: 0x04000F5F RID: 3935
	public readonly CricketPlaceData[] CricketPlaceData = new CricketPlaceData[141];

	// Token: 0x04000F60 RID: 3936
	public readonly Dictionary<short, CricketPlaceExtraData> CricketPlaceExtraData = new Dictionary<short, CricketPlaceExtraData>();

	// Token: 0x04000F61 RID: 3937
	public List<short> CustomMapBlockCharInfoList = new List<short>();

	// Token: 0x04000F62 RID: 3938
	public List<short> CustomMapBlockCharButtonList = new List<short>();

	// Token: 0x04000F63 RID: 3939
	public readonly TreasureMaterialData[] BrokenAreaMaterials = new TreasureMaterialData[90];

	// Token: 0x04000F64 RID: 3940
	public readonly Dictionary<short, LoongInfo> FiveLoongDict = new Dictionary<short, LoongInfo>();

	// Token: 0x04000F65 RID: 3941
	public readonly Dictionary<int, GameData.Domains.Character.Animal> Animals = new Dictionary<int, GameData.Domains.Character.Animal>();

	// Token: 0x04000F66 RID: 3942
	public readonly Dictionary<short, int> AreaSpiritualDebt = new Dictionary<short, int>();

	// Token: 0x04000F67 RID: 3943
	private AreaBlockCollection[] _normalBlocks = new AreaBlockCollection[3];

	// Token: 0x04000F68 RID: 3944
	private AreaBlockCollection _brokenBlocks;

	// Token: 0x04000F69 RID: 3945
	private bool _travellingEventPerforming = false;

	// Token: 0x04000F6A RID: 3946
	public IronPlateData IronPlateData;

	// Token: 0x04000F6B RID: 3947
	public CharacterDisplayData IronPlateCombatCharData;

	// Token: 0x04000F6C RID: 3948
	public ResourceMonitor TaiwuResources;

	// Token: 0x04000F6D RID: 3949
	public Location LastBlockDataChangedLocation = Location.Invalid;

	// Token: 0x04000F6E RID: 3950
	public Location LastAnimalDataChangedLocation = Location.Invalid;

	// Token: 0x04000F6F RID: 3951
	public Dictionary<short, short> SettlementRandNameDict = new Dictionary<short, short>();

	// Token: 0x04000F70 RID: 3952
	public Dictionary<short, short> CricketChangedInArea = new Dictionary<short, short>();

	// Token: 0x04000F71 RID: 3953
	public Dictionary<short, short> ThiefChangedInArea = new Dictionary<short, short>();

	// Token: 0x04000F72 RID: 3954
	public List<short> FuyuHiltMovePath;

	// Token: 0x04000F73 RID: 3955
	public List<MapBlockDisplayData> CurrentAreaBlockDisplayData = new List<MapBlockDisplayData>();

	// Token: 0x04000F74 RID: 3956
	public Dictionary<Location, MapBlockData> ExtraRequestedBlockData = new Dictionary<Location, MapBlockData>();

	// Token: 0x04000F75 RID: 3957
	public Dictionary<Location, int> IndexedAreaBlockDisplayData = new Dictionary<Location, int>();

	// Token: 0x04000F76 RID: 3958
	public Dictionary<short, byte> AreaMapSize = new Dictionary<short, byte>();

	// Token: 0x04000F77 RID: 3959
	public sbyte CurrentStateId;

	// Token: 0x04000F78 RID: 3960
	public short CurrentAreaId;

	// Token: 0x04000F79 RID: 3961
	public short CurrentBlockId;

	// Token: 0x04000F7A RID: 3962
	public short ShowingAreaId;

	// Token: 0x04000F7B RID: 3963
	public short SelectedBlockId;

	// Token: 0x04000F7C RID: 3964
	private int _taiwuCharId;

	// Token: 0x04000F7D RID: 3965
	private byte _mapSize;

	// Token: 0x04000F7E RID: 3966
	private readonly ushort[] _blockDataIds = new ushort[3];

	// Token: 0x04000F7F RID: 3967
	private Location _lastUpdatedLocation;

	// Token: 0x04000F80 RID: 3968
	private bool _isLocationUpdated;

	// Token: 0x04000F81 RID: 3969
	[TupleElementNames(new string[]
	{
		"BlockId",
		"SettlementId",
		"AreaTemplateId"
	})]
	private ValueTuple<short, short, short> _brokenPerformAreaSettlementData;

	// Token: 0x04000F82 RID: 3970
	private readonly List<CaravanDisplayData> _caravanData = new List<CaravanDisplayData>();

	// Token: 0x04000F83 RID: 3971
	private readonly List<Location> _movePath = new List<Location>();

	// Token: 0x04000F84 RID: 3972
	private Dictionary<Location, HashSet<short>> _blockGroupDict = new Dictionary<Location, HashSet<short>>();

	// Token: 0x04000F85 RID: 3973
	private Dictionary<Location, Location> _blockRootDict = new Dictionary<Location, Location>();

	// Token: 0x04000F86 RID: 3974
	private readonly List<SectStoryThiefData> _lastThiefData = new List<SectStoryThiefData>();

	// Token: 0x04000F87 RID: 3975
	private int _professionAddingViewRange;

	// Token: 0x04000F8D RID: 3981
	public static bool MapBlockLoadFinish = false;

	// Token: 0x04000F8E RID: 3982
	public static bool MapBlockUiLoadFinish = false;

	// Token: 0x04000F8F RID: 3983
	public static bool MapBlockRenderFinish = false;

	// Token: 0x04000F90 RID: 3984
	public static bool Traveling;

	// Token: 0x04000F91 RID: 3985
	public MapBlockData RemoveSwordTombRootMapBlock;

	// Token: 0x04000F92 RID: 3986
	private int _gameDataListenerId = -1;

	// Token: 0x04000F93 RID: 3987
	private DispatcherInstance _dispatcher;

	// Token: 0x04000F94 RID: 3988
	private readonly AStarMap _aStarMap = new AStarMap();

	// Token: 0x04000F95 RID: 3989
	private string _mainStoryBgmName;

	// Token: 0x04000F97 RID: 3991
	public static readonly sbyte[] UsingCarrierFirstOrder = new sbyte[]
	{
		11,
		12
	};

	// Token: 0x04000F98 RID: 3992
	public static readonly sbyte[] UsingCarrierLastOrder = new sbyte[]
	{
		12,
		11
	};

	// Token: 0x0200121E RID: 4638
	public enum MoveState
	{
		// Token: 0x04009997 RID: 39319
		Idle,
		// Token: 0x04009998 RID: 39320
		SendMoveMessage,
		// Token: 0x04009999 RID: 39321
		PerformMove,
		// Token: 0x0400999A RID: 39322
		WaitEventShow,
		// Token: 0x0400999B RID: 39323
		EscapeToAdjacentBlock,
		// Token: 0x0400999C RID: 39324
		ProfessionTravelerSkill
	}

	// Token: 0x0200121F RID: 4639
	public enum EViewMode
	{
		// Token: 0x0400999E RID: 39326
		Normal,
		// Token: 0x0400999F RID: 39327
		Info
	}
}
