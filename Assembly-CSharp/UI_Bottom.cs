using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using DG.Tweening;
using FrameWork;
using FrameWork.CommandSystem;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.Building;
using Game.Views.CharacterMenu;
using Game.Views.Map;
using Game.Views.VillagerRoleView;
using GameData.Common;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.CombatSkill;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.LifeRecord;
using GameData.Domains.LifeRecord.GeneralRecord;
using GameData.Domains.Map;
using GameData.Domains.Map.TeammateBubble;
using GameData.Domains.Organization;
using GameData.Domains.Organization.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000370 RID: 880
[Obsolete]
public class UI_Bottom : UIBase
{
	// Token: 0x060032FD RID: 13053 RVA: 0x00191BC8 File Offset: 0x0018FDC8
	public void OpenMainOperation()
	{
		UIManager.Instance.MaskUI(UIElement.MainOperation);
	}

	// Token: 0x060032FE RID: 13054 RVA: 0x00191BDB File Offset: 0x0018FDDB
	public void OpenBlockOperation()
	{
		UIManager.Instance.MaskUI(UIElement.BlockOperation);
	}

	// Token: 0x17000593 RID: 1427
	// (get) Token: 0x060032FF RID: 13055 RVA: 0x00191BEE File Offset: 0x0018FDEE
	// (set) Token: 0x06003300 RID: 13056 RVA: 0x00191BF6 File Offset: 0x0018FDF6
	private bool WaitingTimeCollect
	{
		get
		{
			return this._waitingTimeCollect;
		}
		set
		{
			this._waitingTimeCollect = value;
			ViewWorldMap.SetDisableMoving(this._waitingTimeCollect);
		}
	}

	// Token: 0x17000594 RID: 1428
	// (get) Token: 0x06003301 RID: 13057 RVA: 0x00191C0C File Offset: 0x0018FE0C
	private int TaiwuCharId
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		}
	}

	// Token: 0x06003302 RID: 13058 RVA: 0x00191C18 File Offset: 0x0018FE18
	public override void OnInit(ArgumentBox argsBox)
	{
		this.MapPickupWindow.gameObject.SetActive(false);
		this._groupChar = base.CGet<Refers>("GroupChar");
		this._mapBlockInfo = base.CGet<Refers>("MapBlockInfo");
		this._buildingAreaInfo = base.CGet<Refers>("BuildingAreaInfo");
		this._readAndLoop = base.CGet<ReadAndLoop>("ReadAndLoop");
		this.EnsureMapBlockInfoAdventureButtons();
		bool ready = this.Element.Ready;
		if (!ready)
		{
			this._lastCheckLoopingCombatSkill = new CombatSkillKey(-1, -1);
			this._mapModel = SingletonObject.getInstance<WorldMapModel>();
			this._taiwuCharacterModel = SingletonObject.getInstance<TaiwuCharacterModel>();
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				this.SetInteractable(true, true, true);
				this.SetTimeDiskLocked(false);
				this.UpdateDate(this._timeDisk);
				this.UpdateReading();
				this.UpdateAllCombatTeammates();
				MapBlockData blockData = this._blockData ?? this._mapModel.GetBlockData(this._mapModel.CurrentBlockId);
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>().SetObject("block", blockData);
				this.UpdateMapBlockInfo(argBox);
				this.RefreshWorkButton(UI_Bottom.ERefreshWorkButtonMoving.Unknown);
				this.RefreshLifePanelButton();
				BuildingDomainMethod.AsyncCall.IsHaveChickenKing(this, delegate(int offset, RawDataPool dataPool)
				{
					Serializer.Deserialize(dataPool, offset, ref UI_Bottom._isHaveChickenKing);
				});
				TaiwuDomainMethod.Call.CalcResourceChangeByVillageWork(this.Element.GameDataListenerId);
				TaiwuDomainMethod.Call.CalcResourceChangeByBuildingMaintain(this.Element.GameDataListenerId);
			}));
			UIElement element2 = this.Element;
			element2.OnShowed = (Action)Delegate.Combine(element2.OnShowed, new Action(delegate()
			{
				this._extraPostProcess.Clear();
			}));
			UIElement element3 = this.Element;
			element3.OnHide = (Action)Delegate.Combine(element3.OnHide, new Action(delegate()
			{
				this._extraPostProcess.Clear();
				bool flag = this._lastCheckLoopingCombatSkill.SkillTemplateId >= 0;
				if (flag)
				{
					GameDataBridge.AddDataUnMonitor(this.Element.GameDataListenerId, 7, 0, (ulong)this._lastCheckLoopingCombatSkill, 6U);
					this._lastCheckLoopingCombatSkill = new CombatSkillKey(-1, -1);
				}
			}));
			this._uiAnim = base.GetComponent<UIAnim>();
			this._uiAnim.Init(Vector3.zero, Vector3.zero.SetY(-500f));
		}
	}

	// Token: 0x06003303 RID: 13059 RVA: 0x00191D5C File Offset: 0x0018FF5C
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 31, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 58, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 8, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 7, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 55, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)this.TaiwuCharId, UI_Bottom.ListenTaiwuFieldIds));
		this.MonitorFields.Add(new UIBase.MonitorDataField(3, 1, (ulong)SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageSettlementId(), new uint[]
		{
			10U
		}));
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 51, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 14, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 41, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 13, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 40, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 43, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 44, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 9, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 10, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(19, 10, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(19, 56, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(19, 114, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(19, 115, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(19, 122, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 71, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(19, 201, ulong.MaxValue, null));
	}

	// Token: 0x06003304 RID: 13060 RVA: 0x00191F9C File Offset: 0x0019019C
	private void Awake()
	{
		this.MapPickupWindow.Init();
		this._groupChar = base.CGet<Refers>("GroupChar");
		TriggerPanel lifeMenuBg = this._groupChar.CGet<TriggerPanel>("LifeMenuBg");
		this._professionBottomMenu = lifeMenuBg.GetComponent<ProfessionBottomMenu>();
		this._professionBottomMenu.Init(this);
		this._readAndLoop = base.CGet<ReadAndLoop>("ReadAndLoop");
		this._readAndLoop.Init(this);
		this._timeDisk = base.CGet<Refers>("TimeDisk");
		TooltipInvoker tip = this._timeDisk.CGet<TooltipInvoker>("MouseTip");
		tip.Type = TipType.Advance;
		tip.RuntimeParam = new ArgumentBox();
		this._mapBlockInfo = base.CGet<Refers>("MapBlockInfo");
		this._buildingAreaInfo = base.CGet<Refers>("BuildingAreaInfo");
		this._advanceDialogCmd.No = delegate()
		{
			this._advanceActionPointConfirmed = false;
			this._advanceLoopingNeigongConfirmed = false;
			this._advanceInventoryOverflowConfirmed = false;
		};
		this.InitCharPanel();
		this.InitBlockInfoTog();
		GEvent.Add(EEvents.OnGameStateChange, new GEvent.Callback(this.OnGameStateChange));
		GEvent.Add(UiEvents.OnSetBuildingBtnShow, new GEvent.Callback(this.OnSetBuildingBtnShow));
		GEvent.Add(UiEvents.TaskBubbleStart, new GEvent.Callback(this.OnTaskBubbleStart));
		GEvent.Add(UiEvents.TaskBubbleEnded, new GEvent.Callback(this.OnTaskBubbleEnded));
		GEvent.Add(UiEvents.OnEventWindowStart, new GEvent.Callback(this.OnEventWindowStart));
		GEvent.Add(UiEvents.OnEventWindowEnded, new GEvent.Callback(this.OnEventWindowEnded));
		GEvent.Add(UiEvents.OnSetVillagerRole, new GEvent.Callback(this.OnSetVillagerRole));
		GEvent.Add(UiEvents.OnTaiwuReadingBookProgressMayChange, new GEvent.Callback(this.OnTaiwuReadingBookProgressMayChange));
		GEvent.Add(UiEvents.PickupDisplayInfoChange, new GEvent.Callback(this.OnPickUpDisplayInfoChange));
		GEvent.Add(UiEvents.MapPickupDataChanged, new GEvent.Callback(this.OnCurrentPickUpDisplayInfoChange));
		this.InitCustomButtons();
		this.InitAdvanceDaysBtn();
	}

	// Token: 0x06003305 RID: 13061 RVA: 0x001921B0 File Offset: 0x001903B0
	private void LateUpdate()
	{
		bool updateReadingDirty = this._updateReadingDirty;
		if (updateReadingDirty)
		{
			this.UpdateReadingLate();
			this._updateReadingDirty = false;
		}
	}

	// Token: 0x06003306 RID: 13062 RVA: 0x001921D8 File Offset: 0x001903D8
	private void OnDestroy()
	{
		PoolManager.RemoveData("MiniMap_CanPassRoutePrefab");
		PoolManager.RemoveData("MiniMap_CanUnlockRoutePrefab");
		GEvent.Remove(EEvents.OnGameStateChange, new GEvent.Callback(this.OnGameStateChange));
		GEvent.Remove(UiEvents.OnSetBuildingBtnShow, new GEvent.Callback(this.OnSetBuildingBtnShow));
		GEvent.Remove(UiEvents.TaskBubbleStart, new GEvent.Callback(this.OnTaskBubbleStart));
		GEvent.Remove(UiEvents.TaskBubbleEnded, new GEvent.Callback(this.OnTaskBubbleEnded));
		GEvent.Remove(UiEvents.OnEventWindowStart, new GEvent.Callback(this.OnEventWindowStart));
		GEvent.Remove(UiEvents.OnEventWindowEnded, new GEvent.Callback(this.OnEventWindowEnded));
		GEvent.Remove(UiEvents.OnSetVillagerRole, new GEvent.Callback(this.OnSetVillagerRole));
		GEvent.Remove(UiEvents.OnTaiwuReadingBookProgressMayChange, new GEvent.Callback(this.OnTaiwuReadingBookProgressMayChange));
		GEvent.Remove(UiEvents.PickupDisplayInfoChange, new GEvent.Callback(this.OnPickUpDisplayInfoChange));
		GEvent.Remove(UiEvents.MapPickupDataChanged, new GEvent.Callback(this.OnCurrentPickUpDisplayInfoChange));
	}

	// Token: 0x06003307 RID: 13063 RVA: 0x00192310 File Offset: 0x00190510
	private void OnEnable()
	{
		this._groupChar.gameObject.SetActive(false);
		GEvent.Add(EEvents.OnMonthChange, new GEvent.Callback(this.OnMonthChange));
		GEvent.Add(EEvents.RequestAdvanceMonth, new GEvent.Callback(this.RequestAdvanceMonth));
		GEvent.Add(EEvents.OnActionPointChange, new GEvent.Callback(this.OnDaysInMonthChange));
		GEvent.Add(EEvents.OnActionPointInPrevMonthChange, new GEvent.Callback(this.OnDaysInMonthChange));
		GEvent.Add(UiEvents.WorldMapShowInfoBlockChange, new GEvent.Callback(this.UpdateMapBlockInfo));
		GEvent.Add(UiEvents.CombatTeammateChange, new GEvent.Callback(this.UpdateCombatTeammate));
		GEvent.Add(UiEvents.WorldMapBlockDataChange, new GEvent.Callback(this.OnBlockDataChange));
		GEvent.Add(UiEvents.WorldMapVillagerWorkingLocationChange, new GEvent.Callback(this.OnWorldMapMarkLocationChange));
		GEvent.Add(UiEvents.AdventureRemakeEnter, new GEvent.Callback(this.OnAdventureRemakeEnter));
		GEvent.Add(UiEvents.AdventureRemakeExit, new GEvent.Callback(this.OnAdventureRemakeExit));
		GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		GEvent.Add(UiEvents.ShowAdvanceMonthConfirm, new GEvent.Callback(this.OnClickAdvanceMonth));
		GEvent.Add(EEvents.OnFunctionLockStateChange, new GEvent.Callback(this.OnGameFunctionLockStateChange));
		GEvent.Add(EEvents.OnTaiwuCharIdChange, new GEvent.Callback(this.OnTaiwuCharIdChange));
		GEvent.Add(UiEvents.OnTeammateHideStateChange, new GEvent.Callback(this.OnHideTeammatesStateChange));
		GEvent.Add(UiEvents.OnLocationMarkChange, new GEvent.Callback(this.OnLocationMarkChange));
		GEvent.Add(UiEvents.OnMapBlockEnemyTogChange, new GEvent.Callback(this.OnMapBlockEnemyTogChange));
		GEvent.Add(UiEvents.OnMapBlockFriendTogChange, new GEvent.Callback(this.OnMapBlockFriendTogChange));
		GEvent.Add(UiEvents.OnMapBlockMerchantTogChange, new GEvent.Callback(this.OnMapBlockMerchantTogChange));
		GEvent.Add(UiEvents.OnMapBlockSettlementEdgeTogChange, new GEvent.Callback(this.OnMapBlockSettlementEdgeTogChange));
		GEvent.Add(EEvents.TaiwuGroupChange, new GEvent.Callback(this.AddTeammateMonitor));
		GEvent.Add(UiEvents.PlayAnimToHideMainUI, new GEvent.Callback(this.PlayAnimToHideMainUI));
		GEvent.Add(UiEvents.PlayAnimToShowMainUI, new GEvent.Callback(this.PlayAnimToShowMainUI));
		GEvent.Add(UiEvents.WorldMapPlayerBlockChange, new GEvent.Callback(this.OnPlayerBlockChange));
		GEvent.Add(EEvents.OnAdvancingMonthStateChange, new GEvent.Callback(this.OnAdvancingMonthStateChange));
		GEvent.Add(UiEvents.OnSetBottomInteractable, new GEvent.Callback(this.OnSetBottomInteractable));
		GEvent.Add(UiEvents.OnSetBottomRightPartVisible, new GEvent.Callback(this.OnSetBottomRightPartVisible));
		GEvent.Add(UiEvents.OnUpdateQuickBtnState, new GEvent.Callback(this.OnUpdateQuickBtnState));
		GEvent.Add(UiEvents.OnRefreshWorkButton, new GEvent.Callback(this.OnRefreshWorkButton));
		GEvent.Add(UiEvents.OnRefreshWorkPanel, new GEvent.Callback(this.OnRefreshWorkPanel));
		GEvent.Add(UiEvents.OnSetBuildingAreaInfo, new GEvent.Callback(this.OnSetBuildingAreaInfo));
		GEvent.Add(UiEvents.OnSetAreaSpiritualDebt, new GEvent.Callback(this.OnSetAreaSpiritualDebt));
		GEvent.Add(UiEvents.OnRefreshBottomLifePanel, new GEvent.Callback(this.OnRefreshBottomLifePanel));
		GEvent.Add(UiEvents.OnStopVillagerWork, new GEvent.Callback(this.OnStopVillagerWork));
		GEvent.Add(UiEvents.OnMapBlockPastLifeRelationTogChange, new GEvent.Callback(this.OnMapBlockPastLifeRelationTogChange));
		GEvent.Add(UiEvents.OnBottomShowNewNotification, new GEvent.Callback(this.OnBottomShowNewNotification));
		GEvent.Add(UiEvents.CloseBuildingManage, new GEvent.Callback(this.CloseBuildingManage));
		GEvent.Add(UiEvents.OnJieqingMaskCharacterListChanged, new GEvent.Callback(this.OnJieqingMaskCharacterListChanged));
		FunctionLockManager funcLockManager = SingletonObject.getInstance<FunctionLockManager>();
		base.CGet<CButtonObsolete>("WorldMapButton").interactable = funcLockManager.IsFunctionUnlock(4);
		this.ChangeButtonActive(base.CGet<Refers>("MapBlockInfo").CGet<CButtonObsolete>("CollectResource"), funcLockManager.IsFunctionUnlock(5));
		this.ChangeButtonActive(base.CGet<Refers>("MapBlockInfo").CGet<CButtonObsolete>("FindTreasure"), funcLockManager.IsFunctionUnlock(10) && !SingletonObject.getInstance<TutorialChapterModel>().InGuiding);
		this.ChangeButtonActive(base.CGet<Refers>("MapBlockInfo").CGet<CButtonObsolete>("Dispatch"), funcLockManager.IsFunctionUnlock(10));
		this.ChangeButtonActive(base.CGet<Refers>("MapBlockInfo").CGet<CButtonObsolete>("Mark"), funcLockManager.IsFunctionUnlock(10));
		this.ChangeButtonActive(this._mapBlockInfo.CGet<CButtonObsolete>("SettlementInfoOpen"), funcLockManager.IsFunctionUnlock(13));
		this.ChangePanelActive(UI_Bottom.EPanelType.None);
	}

	// Token: 0x06003308 RID: 13064 RVA: 0x001927F4 File Offset: 0x001909F4
	private void OnDisable()
	{
		GEvent.Remove(EEvents.OnMonthChange, new GEvent.Callback(this.OnMonthChange));
		GEvent.Remove(EEvents.RequestAdvanceMonth, new GEvent.Callback(this.RequestAdvanceMonth));
		GEvent.Remove(EEvents.OnActionPointChange, new GEvent.Callback(this.OnDaysInMonthChange));
		GEvent.Remove(EEvents.OnActionPointInPrevMonthChange, new GEvent.Callback(this.OnDaysInMonthChange));
		GEvent.Remove(UiEvents.WorldMapShowInfoBlockChange, new GEvent.Callback(this.UpdateMapBlockInfo));
		GEvent.Remove(UiEvents.CombatTeammateChange, new GEvent.Callback(this.UpdateCombatTeammate));
		GEvent.Remove(UiEvents.WorldMapBlockDataChange, new GEvent.Callback(this.OnBlockDataChange));
		GEvent.Remove(UiEvents.WorldMapVillagerWorkingLocationChange, new GEvent.Callback(this.OnWorldMapMarkLocationChange));
		GEvent.Remove(UiEvents.AdventureRemakeEnter, new GEvent.Callback(this.OnAdventureRemakeEnter));
		GEvent.Remove(UiEvents.AdventureRemakeExit, new GEvent.Callback(this.OnAdventureRemakeExit));
		GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		GEvent.Remove(UiEvents.ShowAdvanceMonthConfirm, new GEvent.Callback(this.OnClickAdvanceMonth));
		GEvent.Remove(EEvents.OnFunctionLockStateChange, new GEvent.Callback(this.OnGameFunctionLockStateChange));
		GEvent.Remove(EEvents.OnTaiwuCharIdChange, new GEvent.Callback(this.OnTaiwuCharIdChange));
		GEvent.Remove(UiEvents.OnTeammateHideStateChange, new GEvent.Callback(this.OnHideTeammatesStateChange));
		GEvent.Remove(UiEvents.OnLocationMarkChange, new GEvent.Callback(this.OnLocationMarkChange));
		GEvent.Remove(UiEvents.OnMapBlockEnemyTogChange, new GEvent.Callback(this.OnMapBlockEnemyTogChange));
		GEvent.Remove(UiEvents.OnMapBlockFriendTogChange, new GEvent.Callback(this.OnMapBlockFriendTogChange));
		GEvent.Remove(UiEvents.OnMapBlockMerchantTogChange, new GEvent.Callback(this.OnMapBlockMerchantTogChange));
		GEvent.Remove(UiEvents.OnMapBlockSettlementEdgeTogChange, new GEvent.Callback(this.OnMapBlockSettlementEdgeTogChange));
		GEvent.Remove(EEvents.TaiwuGroupChange, new GEvent.Callback(this.AddTeammateMonitor));
		GEvent.Remove(UiEvents.PlayAnimToHideMainUI, new GEvent.Callback(this.PlayAnimToHideMainUI));
		GEvent.Remove(UiEvents.PlayAnimToShowMainUI, new GEvent.Callback(this.PlayAnimToShowMainUI));
		GEvent.Remove(UiEvents.WorldMapPlayerBlockChange, new GEvent.Callback(this.OnPlayerBlockChange));
		GEvent.Remove(EEvents.OnAdvancingMonthStateChange, new GEvent.Callback(this.OnAdvancingMonthStateChange));
		GEvent.Remove(UiEvents.OnSetBottomInteractable, new GEvent.Callback(this.OnSetBottomInteractable));
		GEvent.Remove(UiEvents.OnSetBottomRightPartVisible, new GEvent.Callback(this.OnSetBottomRightPartVisible));
		GEvent.Remove(UiEvents.OnUpdateQuickBtnState, new GEvent.Callback(this.OnUpdateQuickBtnState));
		GEvent.Remove(UiEvents.OnRefreshWorkButton, new GEvent.Callback(this.OnRefreshWorkButton));
		GEvent.Remove(UiEvents.OnRefreshWorkPanel, new GEvent.Callback(this.OnRefreshWorkPanel));
		GEvent.Remove(UiEvents.OnSetBuildingAreaInfo, new GEvent.Callback(this.OnSetBuildingAreaInfo));
		GEvent.Remove(UiEvents.OnSetAreaSpiritualDebt, new GEvent.Callback(this.OnSetAreaSpiritualDebt));
		GEvent.Remove(UiEvents.OnRefreshBottomLifePanel, new GEvent.Callback(this.OnRefreshBottomLifePanel));
		GEvent.Remove(UiEvents.OnStopVillagerWork, new GEvent.Callback(this.OnStopVillagerWork));
		GEvent.Remove(UiEvents.OnMapBlockPastLifeRelationTogChange, new GEvent.Callback(this.OnMapBlockPastLifeRelationTogChange));
		GEvent.Remove(UiEvents.OnBottomShowNewNotification, new GEvent.Callback(this.OnBottomShowNewNotification));
		GEvent.Remove(UiEvents.CloseBuildingManage, new GEvent.Callback(this.CloseBuildingManage));
		GEvent.Remove(UiEvents.OnJieqingMaskCharacterListChanged, new GEvent.Callback(this.OnJieqingMaskCharacterListChanged));
	}

	// Token: 0x06003309 RID: 13065 RVA: 0x00192BE0 File Offset: 0x00190DE0
	private void Update()
	{
		bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) && !GameApp.AdvancingMonth && this._timeDisk.gameObject.activeSelf && !UIElement.PartWorld.Exist && !SingletonObject.getInstance<EventModel>().LockInputByEvent && SingletonObject.getInstance<WorldMapModel>().TaiwuMoveState == WorldMapModel.MoveState.Idle;
		if (flag)
		{
			this.DoAdvance();
		}
		bool flag2 = CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false) && base.CGet<Refers>("AdvanceDays").gameObject.activeInHierarchy;
		if (flag2)
		{
			base.CGet<Refers>("AdvanceDays").gameObject.SetActive(false);
		}
		bool flag3 = Input.GetMouseButtonDown(1) || (Input.GetMouseButtonDown(0) && !this._isMouseInPanelRange);
		if (flag3)
		{
			bool flag4 = UIManager.Instance.IsFocusElement(UIElement.StateMainWorld);
			if (flag4)
			{
				CButtonObsolete dispatch = this._mapBlockInfo.CGet<CButtonObsolete>("Dispatch");
				bool isInDispatch = RectTransformUtility.RectangleContainsScreenPoint(dispatch.GetComponent<RectTransform>(), Input.mousePosition, UIManager.Instance.UiCamera);
				bool flag5 = !isInDispatch;
				if (flag5)
				{
					this.RefreshWorkPanel(false);
				}
				CButtonObsolete collectResource = this._mapBlockInfo.CGet<CButtonObsolete>("CollectResource");
				bool isInCollect = RectTransformUtility.RectangleContainsScreenPoint(collectResource.GetComponent<RectTransform>(), Input.mousePosition, UIManager.Instance.UiCamera);
				bool flag6 = !isInCollect;
				if (flag6)
				{
					this.RefreshCollectPanel(false);
				}
				CButtonObsolete findTreasure = this._mapBlockInfo.CGet<CButtonObsolete>("FindTreasure");
				bool isInTreasure = RectTransformUtility.RectangleContainsScreenPoint(findTreasure.GetComponent<RectTransform>(), Input.mousePosition, UIManager.Instance.UiCamera);
				bool flag7 = !isInTreasure;
				if (flag7)
				{
					this.RefreshTreasurePanel(false);
				}
			}
		}
		foreach (Action proc in this._extraPostProcess)
		{
			if (proc != null)
			{
				proc();
			}
		}
		this._readAndLoop.OnUpdate();
	}

	// Token: 0x0600330A RID: 13066 RVA: 0x00192E14 File Offset: 0x00191014
	public void SetIsMouseInPanelRange(bool isInRange)
	{
		this._isMouseInPanelRange = isInRange;
	}

	// Token: 0x0600330B RID: 13067 RVA: 0x00192E20 File Offset: 0x00191020
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
					bool flag = notification.DomainId == 2;
					if (flag)
					{
						bool flag2 = notification.MethodId == 20;
						if (flag2)
						{
							CollectResourceResult result = default(CollectResourceResult);
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref result);
							ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
							argBox.Set("CollectResourceIsMax", this._collectResourceIsMax);
							argBox.Set("CollectType", 0);
							argBox.SetObject("CollectInfo", new List<CollectResourceResult>
							{
								result
							});
							UIElement.CollectResource.SetOnInitArgs(argBox);
							UIElement collectResource = UIElement.CollectResource;
							collectResource.OnHide = (Action)Delegate.Combine(collectResource.OnHide, new Action(delegate()
							{
								WorldDomainMethod.Call.AdvanceDaysInMonth(1);
							}));
							UIManager.Instance.ShowUI(UIElement.CollectResource, true);
						}
						else
						{
							bool flag3 = notification.MethodId == 19;
							if (flag3)
							{
								MapBlockData block = null;
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref block);
								StringBuilder strBuilder = new StringBuilder();
								strBuilder.Clear();
								bool flag4 = block != null;
								if (flag4)
								{
									bool flag5 = block.RootBlockId >= 0;
									if (flag5)
									{
										MapDomainMethod.AsyncCall.GetBlockData(this, block.AreaId, block.RootBlockId, delegate(int offsetR, RawDataPool poolR)
										{
											Serializer.Deserialize(poolR, offsetR, ref block);
											bool flag69 = block.BlockSubType != EMapBlockSubType.SwordTomb;
											if (flag69)
											{
												strBuilder.Append((block.BelongBlockId >= 0) ? this._mapModel.GetBlockName(this._mapModel.CurrentAreaId, block.BelongBlockId, block.TemplateId, -1) : this._mapModel.GetAreaName(this._mapModel.CurrentAreaId));
											}
											strBuilder.Append(LocalStringManager.GetFormat(LanguageKey.LK_Quotation_Marks_Fix, this._mapModel.GetBlockName(this._mapModel.CurrentAreaId, block.BlockId, block.TemplateId, -1)).SetColor("lightbrown"));
										});
									}
									else
									{
										bool flag6 = block.BlockSubType != EMapBlockSubType.SwordTomb;
										if (flag6)
										{
											strBuilder.Append((block.BelongBlockId >= 0) ? this._mapModel.GetBlockName(this._mapModel.CurrentAreaId, block.BelongBlockId, block.TemplateId, -1) : this._mapModel.GetAreaName(this._mapModel.CurrentAreaId));
										}
										strBuilder.Append(LocalStringManager.GetFormat(LanguageKey.LK_Quotation_Marks_Fix, this._mapModel.GetBlockName(this._mapModel.CurrentAreaId, block.BlockId, block.TemplateId, -1)).SetColor("lightbrown"));
									}
								}
								this._mapBlockInfo.CGet<TextMeshProUGUI>("Name").text = strBuilder.ToString();
							}
							else
							{
								bool flag7 = notification.MethodId == 45;
								if (flag7)
								{
									TeammateBubbleCollection collection = null;
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref collection);
									this.OnTeammateBubbleCollectionUpdated(collection);
								}
							}
						}
					}
					else
					{
						bool flag8 = notification.DomainId == 5;
						if (flag8)
						{
							bool flag9 = notification.MethodId == 4 || notification.MethodId == 2 || notification.MethodId == 1 || notification.MethodId == 122 || notification.MethodId == 3 || notification.MethodId == 175;
							if (flag9)
							{
								this.RefreshWorkPanelDelayOneFrame();
							}
							else
							{
								bool flag10 = notification.MethodId == 12;
								if (flag10)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._gatherChange);
								}
								else
								{
									bool flag11 = notification.MethodId == 14;
									if (flag11)
									{
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._maintainChange);
									}
								}
							}
						}
						else
						{
							bool flag12 = notification.DomainId == 4;
							if (flag12)
							{
								bool flag13 = notification.MethodId == 48;
								if (flag13)
								{
									List<CharacterDisplayData> displayDataList = null;
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref displayDataList);
									for (int i = 0; i < displayDataList.Count; i++)
									{
										CharacterDisplayData data = displayDataList[i];
										this._charDisplayDataDict[data.CharacterId] = data;
									}
									bool flag14 = this._needUpdateGroupAvatar && this._charDisplayDataDict.ContainsKey(this.TaiwuCharId);
									if (flag14)
									{
										this._teammateIdList.Remove(this.TaiwuCharId);
										this._groupChar.CGet<Game.Components.Avatar.Avatar>("Avatar").Refresh(this._charDisplayDataDict[this.TaiwuCharId], true);
										this._groupChar.gameObject.SetActive(true);
										this._needUpdateGroupAvatar = false;
									}
									SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, new Action(this.ChangePanelActiveByWorkField));
									this.Element.ShowAfterRefresh();
								}
							}
							else
							{
								bool flag15 = notification.DomainId == 7 && notification.MethodId == 0;
								if (flag15)
								{
									List<CombatSkillDisplayData> dataList = null;
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref dataList);
									bool flag16 = dataList != null;
									if (flag16)
									{
										foreach (CombatSkillDisplayData data2 in dataList)
										{
											this._combatSkillDisplayDataDict[data2.TemplateId] = data2;
										}
									}
									bool flag17 = this._combatSkillDisplayDataDict.Count == this._learnedNeigongList.Count;
									if (flag17)
									{
										this.UpdateObtainedNeili();
									}
								}
								else
								{
									bool flag18 = notification.DomainId == 13 && notification.MethodId == 5;
									if (flag18)
									{
										ArgumentCollectionRenderArguments dynamicArguments = null;
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref dynamicArguments);
										this.HandleTeammateBubbleCollectionResult(dynamicArguments);
									}
									else
									{
										bool flag19 = notification.DomainId == 6;
										if (flag19)
										{
											ushort methodId = notification.MethodId;
											ushort num = methodId;
											if (num == 9)
											{
												Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._pagesInfo);
												this._readAndLoop.RefreshReadingBookPagesInfoBalls(this._currentReadingBookKey, this._pagesInfo);
												this._readAndLoop.RefreshReadingBookProgress(this._currentReadingBookKey, this._pagesInfo);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			else
			{
				DataUid uid = notification.Uid;
				bool flag20 = uid.DomainId == 5;
				if (flag20)
				{
					bool flag21 = uid.DataId == 31;
					if (flag21)
					{
						CharacterSet characterSet = default(CharacterSet);
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref characterSet);
						this._charDisplayDataDict.Remove(this.TaiwuCharId);
						for (int j = 0; j < this._teammateIdList.Count; j++)
						{
							int charId = this._teammateIdList[j];
							bool flag22 = this._charDisplayDataDict.ContainsKey(charId) && !this._villagerIdList.Contains(charId);
							if (flag22)
							{
								this._charDisplayDataDict.Remove(charId);
							}
						}
						this._teammateIdList.Clear();
						this._teammateIdList.AddRange(characterSet.GetCollection());
						this._needUpdateGroupAvatar = true;
						CharacterDomainMethod.Call.GetCharacterDisplayDataList(this.Element.GameDataListenerId, this._teammateIdList);
					}
					else
					{
						bool flag23 = uid.DataId == 8;
						if (flag23)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._warehouseCurrLoad);
						}
						else
						{
							bool flag24 = uid.DataId == 7;
							if (flag24)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._warehouseMaxLoad);
							}
							else
							{
								bool flag25 = uid.DataId == 55;
								if (flag25)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._materialResourceMaxCount);
								}
								else
								{
									bool flag26 = uid.DataId == 51;
									if (flag26)
									{
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._visitedSettlements);
										this.UpdateMapBlockInfo(null);
									}
									else
									{
										bool flag27 = uid.DataId == 14;
										if (flag27)
										{
											Serializer.DeserializeModifications<short>(wrapper.DataPool, notification.ValueOffset, this._lifeSkillData);
											this.UpdateReading();
										}
										else
										{
											bool flag28 = uid.DataId == 41;
											if (flag28)
											{
												Serializer.DeserializeModifications<short>(wrapper.DataPool, notification.ValueOffset, this._notLearnedLifeSkillData);
												this.UpdateReading();
											}
											else
											{
												bool flag29 = uid.DataId == 13;
												if (flag29)
												{
													Serializer.DeserializeModifications<short>(wrapper.DataPool, notification.ValueOffset, this._combatSkillData);
													this.UpdateReading();
												}
												else
												{
													bool flag30 = uid.DataId == 40;
													if (flag30)
													{
														Serializer.DeserializeModifications<short>(wrapper.DataPool, notification.ValueOffset, this._notLearnedCombatSkillData);
														this.UpdateReading();
													}
													else
													{
														bool flag31 = uid.DataId == 43;
														if (flag31)
														{
															Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._currentReadingBookKey);
															this.AsyncRefreshReadingBookPagesInfoBalls();
															this.UpdateReading();
															ArgumentBox argBox2 = EasyPool.Get<ArgumentBox>();
															argBox2.Set<ItemKey>("Key", this._currentReadingBookKey);
															GEvent.OnEvent(UiEvents.OnTaiwuReadingBookKeyMayChange, argBox2);
														}
														else
														{
															bool flag32 = uid.DataId == 44;
															if (flag32)
															{
																Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._referenceBooks);
																this.UpdateReading();
															}
															else
															{
																bool flag33 = uid.DataId == 9;
																if (flag33)
																{
																	Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._buildingSpaceLimit);
																	this.UpdateBuildigSpaceInfo();
																}
																else
																{
																	bool flag34 = uid.DataId == 10;
																	if (flag34)
																	{
																		Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._buildingSpaceCurr);
																		this.UpdateBuildigSpaceInfo();
																	}
																	else
																	{
																		bool flag35 = uid.DataId == 58;
																		if (flag35)
																		{
																			this.RefreshRoleManageButton();
																		}
																		else
																		{
																			bool flag36 = uid.DataId == 71;
																			if (flag36)
																			{
																				this._needUpdateGroupAvatar = true;
																				this._teammateIdList.Add(this.TaiwuCharId);
																				CharacterDomainMethod.Call.GetCharacterDisplayDataList(this.Element.GameDataListenerId, this._teammateIdList);
																				this.UpdateAllCombatTeammates();
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				else
				{
					bool flag37 = uid.DomainId == 4 && uid.DataId == 0 && ((ushort)uid.SubId1 == 26 || (ushort)uid.SubId1 == 44);
					if (flag37)
					{
						bool flag38 = this._injuriesDict == null;
						if (flag38)
						{
							this._injuriesDict = new Dictionary<int, Injuries>();
						}
						bool flag39 = this._poisonIntsDict == null;
						if (flag39)
						{
							this._poisonIntsDict = new Dictionary<int, PoisonInts>();
						}
						int charId2 = (int)uid.SubId0;
						bool flag40 = (ushort)uid.SubId1 == 26;
						if (flag40)
						{
							Injuries injuries = default(Injuries);
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref injuries);
							this._injuriesDict[charId2] = injuries;
						}
						else
						{
							PoisonInts poisons = default(PoisonInts);
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref poisons);
							this._poisonIntsDict[charId2] = poisons;
						}
						this.RefreshAllHealBtn();
					}
					else
					{
						bool flag41 = uid.DomainId == 4 && uid.DataId == 0 && (int)uid.SubId0 == this.TaiwuCharId;
						if (flag41)
						{
							bool flag42 = uid.SubId1 == 34U;
							if (flag42)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._resources);
								this._professionBottomMenu.UpdateResources(this._resources);
							}
							else
							{
								bool flag43 = uid.SubId1 == 66U;
								if (flag43)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._exp);
									this._professionBottomMenu.UpdateExp(this._exp);
								}
								else
								{
									bool flag44 = uid.SubId1 == 56U || uid.SubId1 == 39U || uid.SubId1 == 75U;
									if (flag44)
									{
										bool flag45 = uid.SubId1 == 56U;
										if (flag45)
										{
											ItemKey[] equipments = null;
											Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref equipments);
											this.HandlerDataCharacterEquipments(equipments);
										}
										bool flag46 = this._charDisplayDataDict.ContainsKey(this.TaiwuCharId);
										if (flag46)
										{
											List<int> charIdList = EasyPool.Get<List<int>>();
											charIdList.Clear();
											charIdList.Add(this.TaiwuCharId);
											this._teammateIdList.Remove(this.TaiwuCharId);
											this._charDisplayDataDict.Remove(this.TaiwuCharId);
											this._needUpdateGroupAvatar = true;
											CharacterDomainMethod.Call.GetCharacterDisplayDataList(this.Element.GameDataListenerId, charIdList);
											EasyPool.Free<List<int>>(charIdList);
										}
									}
									else
									{
										bool flag47 = uid.SubId1 == 59U;
										if (flag47)
										{
											Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._learnedNeigongList);
											this._learnedNeigongList.RemoveAll((short id) => CombatSkill.Instance[id].EquipType != 0);
											this._combatSkillDisplayDataDict.Clear();
											bool flag48 = this._learnedNeigongList.Count > 0;
											if (flag48)
											{
												CombatSkillModel.GetCombatSkillDisplayData(this.Element.GameDataListenerId, this.TaiwuCharId, this._learnedNeigongList);
											}
										}
										else
										{
											bool flag49 = uid.SubId1 == 46U;
											if (flag49)
											{
												Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._loopingNeigong);
												this.UpdateObtainedNeili();
												ArgumentBox argBox3 = EasyPool.Get<ArgumentBox>();
												argBox3.Set("Id", this._loopingNeigong);
											}
											else
											{
												bool flag50 = uid.SubId1 == 104U;
												if (flag50)
												{
													Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._inventoryCurrLoad);
												}
												else
												{
													bool flag51 = uid.SubId1 == 103U;
													if (flag51)
													{
														Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._inventoryMaxLoad);
													}
													else
													{
														bool flag52 = uid.SubId1 == 111U;
														if (flag52)
														{
															sbyte neiliType = -1;
															Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref neiliType);
															this._readAndLoop.RefreshNeiliType(neiliType);
														}
														else
														{
															bool flag53 = uid.SubId1 == 43U;
															if (flag53)
															{
																Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._taiwuMainAttributes);
																this._readAndLoop.RefreshMainAttribute(this._taiwuMainAttributes);
															}
															else
															{
																bool flag54 = uid.SubId1 == 72U;
																if (flag54)
																{
																	int[] taiwuExtraNeiliAllocationProgress = null;
																	Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref taiwuExtraNeiliAllocationProgress);
																	this._readAndLoop.RefreshTaiwuExtraNeiliAllocationProgress(taiwuExtraNeiliAllocationProgress);
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
						else
						{
							bool flag55 = uid.DomainId == 3 && uid.DataId == 1 && (short)uid.SubId0 == this._mapModel.GetTaiwuVillageSettlementId() && uid.SubId1 == 10U;
							if (flag55)
							{
								OrgMemberCollection collection2 = null;
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref collection2);
								collection2.GetAllMembers(this._villagerIdList);
								for (int k = 0; k < this._villagerIdList.Count; k++)
								{
									int charId3 = this._villagerIdList[k];
									bool flag56 = this._charDisplayDataDict.ContainsKey(charId3) && !this._teammateIdList.Contains(charId3);
									if (flag56)
									{
										this._charDisplayDataDict.Remove(charId3);
									}
								}
								bool flag57 = this._villagerIdList.Count > 0;
								if (flag57)
								{
									CharacterDomainMethod.Call.GetCharacterDisplayDataList(this.Element.GameDataListenerId, this._villagerIdList);
								}
							}
							else
							{
								bool flag58 = uid.DomainId == 7;
								if (flag58)
								{
									bool flag59 = uid.DataId == 0 && uid.SubId1 == 6U;
									if (flag59)
									{
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._obtainedNeili);
										this.UpdateLoopingNeigong();
									}
								}
								else
								{
									bool flag60 = uid.DomainId == 19;
									if (flag60)
									{
										bool flag61 = uid.DataId == 10;
										if (flag61)
										{
											Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._readingEventBookList);
											this.UpdateReading();
										}
										else
										{
											bool flag62 = uid.DataId == 56;
											if (flag62)
											{
												Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._unlockedWorkingList);
											}
											else
											{
												bool flag63 = uid.DataId == 114;
												if (flag63)
												{
													Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._activeLoopingProgress);
													this.RefreshLoopingProgress();
												}
												else
												{
													bool flag64 = uid.DataId == 115;
													if (flag64)
													{
														Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._activeReadingProgress);
														bool flag65 = this._activeReadingProgress % 10 == 0;
														if (flag65)
														{
															bool flag66 = this._currentReadingBookKey.IsValid();
															if (flag66)
															{
																ItemDomainMethod.Call.GetSkillBookPagesInfo(this.Element.GameDataListenerId, this._currentReadingBookKey);
															}
														}
														this.RefreshReadingProgress();
													}
													else
													{
														bool flag67 = uid.DataId == 122;
														if (flag67)
														{
															Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._loopingEventSkillIdList);
															this.UpdateLoopingNeigong();
														}
														else
														{
															bool flag68 = uid.DataId == 201;
															if (flag68)
															{
																Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._mainUiCustomButtonList);
																this.UpdateMainUiCustomButtons();
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0600330C RID: 13068 RVA: 0x0019402C File Offset: 0x0019222C
	private void UpdateBuildigSpaceInfo()
	{
		string value = (this._buildingSpaceCurr > this._buildingSpaceLimit) ? this._buildingSpaceCurr.ToString().SetColor("red") : this._buildingSpaceCurr.ToString().SetColor("pinkyellow");
		int value2 = this._buildingSpaceLimit;
		string title = LocalStringManager.Get(LanguageKey.LK_Taiwu_BuildingSpace_Title);
		this._buildingAreaInfo.CGet<TextMeshProUGUI>("BuildingSpaceLimitText").SetText(string.Format("{0}/{1}\n{2}", value, value2, title), true);
	}

	// Token: 0x0600330D RID: 13069 RVA: 0x001940B4 File Offset: 0x001922B4
	protected override void OnClick(Transform btn)
	{
		bool buildingButtonBaned = UI_Bottom._buildingButtonBaned;
		if (!buildingButtonBaned)
		{
			string btnName = btn.name;
			bool flag = btnName == "TaiwuChar";
			if (flag)
			{
				this.OpenTeammateCharacterMenu(this.TaiwuCharId, ECharacterSubToggleBase.None, ECharacterSubPage.None);
			}
			else
			{
				bool flag2 = btnName.StartsWith("LoopNeigongSlot");
				if (flag2)
				{
					bool flag3 = !this._forcedCanOpenCharacterMenu;
					if (!flag3)
					{
						UIManager.Instance.ShowUI(UIElement.Looping, true);
					}
				}
				else
				{
					bool flag4 = btnName.StartsWith("Teammate_");
					if (flag4)
					{
						int charId = btn.GetComponent<Refers>().UserInt;
						TaiwuEventDomainMethod.Call.OnCharacterClicked(charId);
					}
					else
					{
						bool flag5 = btnName == "AdvanceSolarTerm";
						if (flag5)
						{
							this.DoAdvance();
						}
						else
						{
							bool flag6 = btnName == "CollectResource";
							if (flag6)
							{
								this.ChangePanelActive(this._collectPanelIsShow ? UI_Bottom.EPanelType.None : UI_Bottom.EPanelType.Collect);
							}
							else
							{
								bool flag7 = btnName == "Dispatch";
								if (flag7)
								{
									this.ChangePanelActive(this._workPanelIsShow ? UI_Bottom.EPanelType.None : UI_Bottom.EPanelType.Work);
								}
								else
								{
									bool flag8 = btnName == "Mark";
									if (flag8)
									{
										this.OnClickMark();
									}
									else
									{
										bool flag9 = btnName == "FindTreasure";
										if (flag9)
										{
											this.ChangePanelActive(this._treasurePanelIsShow ? UI_Bottom.EPanelType.None : UI_Bottom.EPanelType.Treasure);
										}
										else
										{
											bool flag10 = btnName == "FindTreasureOnce";
											if (flag10)
											{
												this.FindTreasureOnce();
											}
											else
											{
												bool flag11 = btnName == "FindTreasureSeries";
												if (flag11)
												{
													this.FindTreasureSeries();
												}
												else
												{
													bool flag12 = btnName == "TimeWorkBtn";
													if (flag12)
													{
														this.TimeCollect();
													}
													else
													{
														bool flag13 = btnName == "MapOption";
														if (flag13)
														{
															UIManager.Instance.ShowUI(UIElement.MapInfoOption, true);
														}
														else
														{
															bool flag14 = btnName == "BtnHeal";
															if (flag14)
															{
																this.OpenHeal();
															}
															else
															{
																bool flag15 = btnName == "BtnInventory";
																if (flag15)
																{
																	this.OpenTeammateCharacterMenu(this.TaiwuCharId, ECharacterSubToggleBase.ItemBase, ECharacterSubPage.None);
																}
																else
																{
																	bool flag16 = btnName == "BtnNeili";
																	if (flag16)
																	{
																		this.OpenTeammateCharacterMenu(this.TaiwuCharId, ECharacterSubToggleBase.EquipCombatSkillBase, ECharacterSubPage.None);
																	}
																	else
																	{
																		bool flag17 = btnName == "BtnPractice";
																		if (flag17)
																		{
																			this.OpenTeammateCharacterMenu(this.TaiwuCharId, ECharacterSubToggleBase.PracticeBase, ECharacterSubPage.None);
																		}
																		else
																		{
																			bool flag18 = btnName == "BtnProfession";
																			if (flag18)
																			{
																				UIManager.Instance.ShowUI(UIElement.Profession, true);
																			}
																			else
																			{
																				bool flag19 = btnName == "BtnLife";
																				if (flag19)
																				{
																					UIManager.Instance.ShowUI(UIElement.Profession, true);
																				}
																				else
																				{
																					bool flag20 = btnName == "ReadingSlot";
																					if (flag20)
																					{
																						this.OnReadingClicked();
																					}
																					else
																					{
																						bool flag21 = btnName == "ResetCamera";
																						if (flag21)
																						{
																							GEvent.OnEvent(UiEvents.WorldMapResetMapCamera, EasyPool.Get<ArgumentBox>().Set("isAnim", true));
																						}
																						else
																						{
																							bool flag22 = btnName == "WorldMapButton";
																							if (flag22)
																							{
																								WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
																								bool flag23 = mapModel.TaiwuMoveState == WorldMapModel.MoveState.Idle;
																								if (flag23)
																								{
																									UIManager.Instance.ShowUI(UIElement.StatePartWorldMap, true);
																								}
																							}
																							else
																							{
																								bool flag24 = btnName.StartsWith("Area");
																								if (flag24)
																								{
																									short areaId = this._mapModel.GetAreaIdByStateIndex(btn.transform.GetSiblingIndex());
																									GEvent.OnEvent(UiEvents.WorldMapSetCameraToArea, EasyPool.Get<ArgumentBox>().Set("areaId", areaId));
																								}
																								else
																								{
																									bool flag25 = btnName.Equals("ReviewButton");
																									if (flag25)
																									{
																										UIElement.MonthNotify.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("NeedSave", false));
																										UIManager.Instance.ShowUI(UIElement.MonthNotify, true);
																									}
																									else
																									{
																										bool flag26 = btnName == "Close";
																										if (flag26)
																										{
																											UIManager.Instance.StackBack(null);
																										}
																										else
																										{
																											bool flag27 = btnName == "ButtonBuildingOverview";
																											if (flag27)
																											{
																												ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
																												argBox.Clear();
																												argBox.Set("isHaveChickenKing", UI_Bottom._isHaveChickenKing);
																												UIElement.BuildingOverview.SetOnInitArgs(argBox);
																												UIManager.Instance.ShowUI(UIElement.BuildingOverview, true);
																											}
																											else
																											{
																												bool flag28 = btnName == "ButtonMultiplyRemoveBuilding";
																												if (!flag28)
																												{
																													bool flag29 = btnName == "Warehouse";
																													if (flag29)
																													{
																														UIManager.Instance.ShowUI(UIElement.Warehouse, true);
																													}
																													else
																													{
																														bool flag30 = btnName == "SectSupportInfoBtn";
																														if (flag30)
																														{
																															bool flag31 = this._currOrganizationId < 0;
																															if (!flag31)
																															{
																																ArgumentBox box = EasyPool.Get<ArgumentBox>();
																																box.Clear();
																																box.Set("SectTemplateId", this._currOrganizationId);
																																UIElement.CombatSkillTree.SetOnInitArgs(box);
																																UIManager.Instance.MaskUI(UIElement.CombatSkillTree);
																															}
																														}
																														else
																														{
																															bool flag32 = btnName == "JieQingSpecialBuild";
																															if (flag32)
																															{
																																GEvent.OnEvent(UiEvents.BuildQiwenxingtai, null);
																															}
																															else
																															{
																																bool flag33 = btnName == "ResourceBtn";
																																if (flag33)
																																{
																																	BuildingDomainMethod.Call.QuickCollectShopSoldItem();
																																	this.UpdateQuickBtnState();
																																}
																																else
																																{
																																	bool flag34 = btnName == "ItemBtn";
																																	if (flag34)
																																	{
																																		BuildingDomainMethod.AsyncCall.QuickCollectShopItem(this, delegate(int offset, RawDataPool dataPool)
																																		{
																																			List<ItemDisplayData> itemDisplayDatas = new List<ItemDisplayData>();
																																			Serializer.Deserialize(dataPool, offset, ref itemDisplayDatas);
																																			ArgumentBox argBox2 = EasyPool.Get<ArgumentBox>();
																																			argBox2.SetObject("ItemList", itemDisplayDatas);
																																			argBox2.Set("InWareHouse", true);
																																			UIElement.GetItem.SetOnInitArgs(argBox2);
																																			UIManager.Instance.MaskUI(UIElement.GetItem);
																																			this.UpdateQuickBtnState();
																																		});
																																	}
																																	else
																																	{
																																		bool flag35 = btnName == "PeopleBtn";
																																		if (flag35)
																																		{
																																			UI_RecruitPeopleOverview.EntryFromBuildingArea();
																																		}
																																		else
																																		{
																																			bool flag36 = btnName == "QuickSelect";
																																			if (flag36)
																																			{
																																				bool activeSelf = this._buildingAreaInfo.CGet<CButtonObsolete>("ResourceBtn").gameObject.activeSelf;
																																				if (activeSelf)
																																				{
																																					BuildingDomainMethod.Call.QuickCollectShopSoldItem();
																																					this.UpdateQuickBtnState();
																																				}
																																				bool flag37 = this._buildingAreaInfo.CGet<CButtonObsolete>("ItemBtn").gameObject.activeSelf && !this._buildingAreaInfo.CGet<CButtonObsolete>("PeopleBtn").gameObject.activeSelf;
																																				if (flag37)
																																				{
																																					BuildingDomainMethod.AsyncCall.QuickCollectShopItem(this, delegate(int offset, RawDataPool dataPool)
																																					{
																																						List<ItemDisplayData> itemDisplayDatas = new List<ItemDisplayData>();
																																						Serializer.Deserialize(dataPool, offset, ref itemDisplayDatas);
																																						ArgumentBox argBox2 = EasyPool.Get<ArgumentBox>();
																																						argBox2.SetObject("ItemList", itemDisplayDatas);
																																						argBox2.Set("InWareHouse", true);
																																						UIElement.GetItem.SetOnInitArgs(argBox2);
																																						UIManager.Instance.MaskUI(UIElement.GetItem);
																																						this.UpdateQuickBtnState();
																																					});
																																				}
																																				else
																																				{
																																					bool flag38 = !this._buildingAreaInfo.CGet<CButtonObsolete>("ItemBtn").gameObject.activeSelf && this._buildingAreaInfo.CGet<CButtonObsolete>("PeopleBtn").gameObject.activeSelf;
																																					if (flag38)
																																					{
																																						UI_RecruitPeopleOverview.EntryFromBuildingArea();
																																					}
																																					else
																																					{
																																						bool flag39 = this._buildingAreaInfo.CGet<CButtonObsolete>("ItemBtn").gameObject.activeSelf && this._buildingAreaInfo.CGet<CButtonObsolete>("PeopleBtn").gameObject.activeSelf;
																																						if (flag39)
																																						{
																																							BuildingDomainMethod.AsyncCall.QuickCollectShopItem(this, delegate(int offset, RawDataPool dataPool)
																																							{
																																								List<ItemDisplayData> itemDisplayDatas = new List<ItemDisplayData>();
																																								Serializer.Deserialize(dataPool, offset, ref itemDisplayDatas);
																																								ArgumentBox argBox2 = EasyPool.Get<ArgumentBox>();
																																								argBox2.SetObject("ItemList", itemDisplayDatas);
																																								argBox2.Set("InWareHouse", true);
																																								argBox2.SetObject("CloseAction", new Action(delegate
																																								{
																																									UI_RecruitPeopleOverview.EntryFromBuildingArea();
																																								}));
																																								UIElement.GetItem.SetOnInitArgs(argBox2);
																																								UIManager.Instance.MaskUI(UIElement.GetItem);
																																							});
																																						}
																																					}
																																				}
																																			}
																																			else
																																			{
																																				bool flag40 = btnName == "ButtonPlanBuilding";
																																				if (flag40)
																																				{
																																					GEvent.OnEvent(UiEvents.StartPlanOrRemoveBuilding, null);
																																				}
																																				else
																																				{
																																					bool flag41 = btnName == "ShowAllLogButton";
																																					if (flag41)
																																					{
																																						GEvent.OnEvent(UiEvents.OnBottomShowAllLogClicked, null);
																																						base.CGet<GameObject>("NewLogNotice").SetActive(false);
																																					}
																																					else
																																					{
																																						bool flag42 = btnName == "FollowButton";
																																						if (flag42)
																																						{
																																							UIManager.Instance.ShowUI(UIElement.Following, true);
																																						}
																																						else
																																						{
																																							bool flag43 = btnName == "RoleManageButton";
																																							if (flag43)
																																							{
																																								ArgumentBox argbox = EasyPool.Get<ArgumentBox>();
																																								argbox.Set("EnterType", ViewVillagerRole.EnterType.Normal);
																																								argbox.Set("EnterPage", ViewVillagerRole.EVillagerRolePage.RoleDescription);
																																								UIElement.VillagerRole.SetOnInitArgs(argbox);
																																								UIManager.Instance.ShowUI(UIElement.VillagerRole, true);
																																							}
																																							else
																																							{
																																								bool flag44 = btnName == "MenuMenu";
																																								if (!flag44)
																																								{
																																									bool flag45 = btnName == "DispatchMenu";
																																									if (flag45)
																																									{
																																									}
																																								}
																																							}
																																						}
																																					}
																																				}
																																			}
																																		}
																																	}
																																}
																															}
																														}
																													}
																												}
																											}
																										}
																									}
																								}
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0600330E RID: 13070 RVA: 0x001948DC File Offset: 0x00192ADC
	private void OpenHeal()
	{
		bool flag = !this._forcedCanOpenCharacterMenu;
		if (!flag)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			HashSet<int> patientList = EasyPool.Get<HashSet<int>>();
			HashSet<int> doctorList = EasyPool.Get<HashSet<int>>();
			patientList.Add(this.TaiwuCharId);
			patientList.UnionWith(this._teammateIdList);
			CharacterMonitorModel monitor = SingletonObject.getInstance<CharacterMonitorModel>();
			doctorList.UnionWith(patientList);
			patientList.UnionWith(monitor.GetTaiwuSpecialGroup());
			doctorList.UnionWith(monitor.GetTaiwuGearMateGroup());
			argBox.SetObject("DoctorList", doctorList.ToList<int>());
			argBox.SetObject("PatientList", patientList.ToList<int>());
			argBox.Set("NeedPay", false);
			CharacterDomainMethod.AsyncCall.GetSomeoneKidnapCharacters(null, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool dataPool)
			{
				KidnappedCharacterList kidnappedCharacterList = null;
				Serializer.Deserialize(dataPool, offset, ref kidnappedCharacterList);
				bool flag2 = kidnappedCharacterList != null;
				if (flag2)
				{
					for (int i = 0; i < kidnappedCharacterList.GetCount(); i++)
					{
						patientList.Add(kidnappedCharacterList.Get(i).CharId);
					}
				}
				UIElement.Heal.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.Heal, true);
			});
		}
	}

	// Token: 0x0600330F RID: 13071 RVA: 0x001949D8 File Offset: 0x00192BD8
	private void OnReadingClicked()
	{
		bool flag = !this._forcedCanOpenCharacterMenu;
		if (!flag)
		{
			bool exist = UIElement.WorldMap.Exist;
			if (exist)
			{
				ViewWorldMap worldMap = UIElement.WorldMap.UiBaseAs<ViewWorldMap>();
				bool flag2 = worldMap != null && (worldMap.IsMoving || worldMap.IsDoingMove);
				if (flag2)
				{
					return;
				}
			}
			UIElement.Reading.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("SlotIndex", 0));
			UIManager.Instance.ShowUI(UIElement.Reading, true);
		}
	}

	// Token: 0x06003310 RID: 13072 RVA: 0x00194A60 File Offset: 0x00192C60
	private void UpdateObtainedNeili()
	{
		bool isContained = this._combatSkillDisplayDataDict.ContainsKey(this._loopingNeigong);
		bool flag = this._loopingNeigong < 0 || isContained;
		if (flag)
		{
			bool flag2 = this._lastCheckLoopingCombatSkill.SkillTemplateId >= 0;
			if (flag2)
			{
				GameDataBridge.AddDataUnMonitor(this.Element.GameDataListenerId, 7, 0, (ulong)this._lastCheckLoopingCombatSkill, 6U);
			}
			this._lastCheckLoopingCombatSkill = new CombatSkillKey(this.TaiwuCharId, this._loopingNeigong);
			bool flag3 = isContained;
			if (flag3)
			{
				GameDataBridge.AddDataMonitor(this.Element.GameDataListenerId, 7, 0, (ulong)this._lastCheckLoopingCombatSkill, 6U);
			}
			else
			{
				this.UpdateLoopingNeigong();
			}
		}
	}

	// Token: 0x06003311 RID: 13073 RVA: 0x00194B0D File Offset: 0x00192D0D
	private void UpdateLoopingNeigong()
	{
		this._readAndLoop.RefreshLoopingNeigong(this._loopingNeigong, this._obtainedNeili, this.TaiwuCharId, this._combatSkillDisplayDataDict, this._loopingEventSkillIdList);
		this._readAndLoop.RefreshActiveLoopButton();
	}

	// Token: 0x06003312 RID: 13074 RVA: 0x00194B46 File Offset: 0x00192D46
	private void OnTaiwuReadingBookProgressMayChange(ArgumentBox _)
	{
		this.UpdateReading();
		this.AsyncRefreshReadingBookPagesInfoBalls();
	}

	// Token: 0x06003313 RID: 13075 RVA: 0x00194B57 File Offset: 0x00192D57
	private void UpdateReading()
	{
		this._updateReadingDirty = true;
	}

	// Token: 0x06003314 RID: 13076 RVA: 0x00194B64 File Offset: 0x00192D64
	private void UpdateReadingLate()
	{
		this._readAndLoop.RefreshReading(this, this._currentReadingBookKey, this._referenceBooks, this._readingEventBookList);
		this._readAndLoop.RefreshActiveReadButton();
		this._readAndLoop.RefreshReadingBookProgress(this._currentReadingBookKey, this._pagesInfo);
	}

	// Token: 0x06003315 RID: 13077 RVA: 0x00194BB8 File Offset: 0x00192DB8
	private void AsyncRefreshReadingBookPagesInfoBalls()
	{
		bool flag = this._currentReadingBookKey.IsValid();
		if (flag)
		{
			ItemDomainMethod.Call.GetSkillBookPagesInfo(this.Element.GameDataListenerId, this._currentReadingBookKey);
		}
		else
		{
			this._readAndLoop.RefreshReadingBookPagesInfoBalls(this._currentReadingBookKey, null);
		}
	}

	// Token: 0x06003316 RID: 13078 RVA: 0x00194C08 File Offset: 0x00192E08
	private void AddTeammateMonitor(ArgumentBox argBox)
	{
		bool flag = this._monitoredInjuryCharIds != null;
		if (flag)
		{
			foreach (int charId in this._monitoredInjuryCharIds)
			{
				bool flag2 = charId != this.TaiwuCharId;
				if (flag2)
				{
					base.RemoveMonitorFieldId(4, 0, (ulong)((long)charId));
				}
			}
		}
		CharacterSet teamCharIds;
		argBox.Get<CharacterSet>("groupCharIds", out teamCharIds);
		bool flag3 = this._monitoredInjuryCharIds == null;
		if (flag3)
		{
			this._monitoredInjuryCharIds = new List<int>();
		}
		this._monitoredInjuryCharIds.Clear();
		foreach (int charId2 in teamCharIds.GetCollection())
		{
			bool flag4 = charId2 != this.TaiwuCharId;
			if (flag4)
			{
				this._monitoredInjuryCharIds.Add(charId2);
				base.AppendMonitorFieldId(new UIBase.MonitorDataField(4, 0, (ulong)((long)charId2), new uint[]
				{
					26U,
					44U
				}));
			}
		}
	}

	// Token: 0x06003317 RID: 13079 RVA: 0x00194D40 File Offset: 0x00192F40
	private void RefreshHealBtn(Refers refers)
	{
		bool isLocked = !this._forcedCanOpenCharacterMenu;
		bool canClick = !isLocked;
		CButtonObsolete btnHeal = refers.CGet<CButtonObsolete>("BtnHeal");
		CButtonObsolete btnInventory = refers.CGet<CButtonObsolete>("BtnInventory");
		btnHeal.interactable = canClick;
		btnInventory.interactable = canClick;
	}

	// Token: 0x06003318 RID: 13080 RVA: 0x00194D85 File Offset: 0x00192F85
	private void RefreshAllHealBtn()
	{
		this.RefreshHealBtn(this._groupChar.CGet<Refers>("BaseLifeBtnLayout").GetComponent<Refers>());
		this.UpdateMainUiCustomButtons();
	}

	// Token: 0x06003319 RID: 13081 RVA: 0x00194DAC File Offset: 0x00192FAC
	private void RefreshLifePanelButton()
	{
		this.UpdateMainUiCustomButtons();
		Refers refers = this._groupChar.CGet<Refers>("BaseLifeBtnLayout");
		CButtonObsolete btnNeili = refers.CGet<CButtonObsolete>("BtnNeili");
		CButtonObsolete btnPractice = refers.CGet<CButtonObsolete>("BtnPractice");
		GameObject btnProcticeLocked = refers.CGet<GameObject>("BtnPracticeLocked");
		GameObject btnNeiliLocked = refers.CGet<GameObject>("BtnNeiliLocked");
		GameObject btnInventoryLocked = refers.CGet<GameObject>("BtnInventoryLocked");
		GameObject btnHealLocked = refers.CGet<GameObject>("BtnHealLocked");
		CButtonObsolete btnHeal = refers.CGet<CButtonObsolete>("BtnHeal");
		CButtonObsolete btnInventory = refers.CGet<CButtonObsolete>("BtnInventory");
		bool canUse = this._forcedCanOpenCharacterMenu;
		bool canUse2 = this._forcedCanOpenCharacterMenu && UI_Bottom.CanOpenCharacterMenuByLegacyAndTutorial();
		bool showLock = !this._forcedCanOpenCharacterMenu;
		TutorialChapterModel tutorialModel = SingletonObject.getInstance<TutorialChapterModel>();
		bool canUse3 = canUse2 && tutorialModel.GetFunctionStatus(7);
		btnNeili.interactable = canUse3;
		btnPractice.interactable = canUse3;
		btnHeal.interactable = canUse;
		btnInventory.interactable = canUse2;
		btnProcticeLocked.SetActive(showLock);
		btnNeiliLocked.SetActive(showLock);
		btnInventoryLocked.SetActive(showLock);
		btnHealLocked.SetActive(showLock);
		btnPractice.GetComponent<PointerTrigger>().enabled = canUse3;
		btnNeili.GetComponent<PointerTrigger>().enabled = canUse3;
		btnHeal.GetComponent<PointerTrigger>().enabled = canUse;
		btnInventory.GetComponent<PointerTrigger>().enabled = canUse2;
	}

	// Token: 0x0600331A RID: 13082 RVA: 0x00194EFC File Offset: 0x001930FC
	private void OnRefreshBottomLifePanel(ArgumentBox argumentBox)
	{
		this.RefreshAllHealBtn();
		this.RefreshLifePanelButton();
		this.UpdateMainUiCustomButtons();
	}

	// Token: 0x0600331B RID: 13083 RVA: 0x00194F14 File Offset: 0x00193114
	private void ReturnBottomTimeDisk(ArgumentBox argBox)
	{
		Transform timeDisk;
		argBox.Get<Transform>("timeDisk", out timeDisk);
		Transform animationRoot = base.transform.Find("AnimationRoot");
		timeDisk.SetParent(animationRoot, true);
		timeDisk.Find("ReviewButton").gameObject.SetActive(true);
		Transform backImg;
		argBox.Get<Transform>("backImg", out backImg);
		Transform layout = base.transform.Find("AnimationRoot/Layout");
		backImg.SetParent(layout, true);
		backImg.SetAsFirstSibling();
		backImg.GetComponent<HSVStyleRoot>().SetDefault();
		Transform readAndLoop;
		argBox.Get<Transform>("readAndLoop", out readAndLoop);
		readAndLoop.SetParent(layout, true);
		readAndLoop.SetSiblingIndex(layout.childCount - 2);
		readAndLoop.GetComponent<HSVStyleRoot>().SetDefault();
	}

	// Token: 0x0600331C RID: 13084 RVA: 0x00194FD1 File Offset: 0x001931D1
	private void OnStopVillagerWork(ArgumentBox _)
	{
		this.RefreshWorkPanelDelayOneFrame();
	}

	// Token: 0x0600331D RID: 13085 RVA: 0x00194FDA File Offset: 0x001931DA
	private void OnHideTeammatesStateChange(ArgumentBox box)
	{
		this.UpdateAllCombatTeammates();
	}

	// Token: 0x0600331E RID: 13086 RVA: 0x00194FE4 File Offset: 0x001931E4
	private void UpdateAllCombatTeammates()
	{
		Refers template = this._groupChar.CGet<Refers>("CombatTeammateTemplate");
		RectTransform rootSlot = this._groupChar.CGet<RectTransform>("CombatCharHolder");
		List<int> data = SingletonObject.getInstance<CharacterMonitorModel>().GetTaiwuCombatTeamCharIds();
		data.RemoveAt(0);
		template.gameObject.SetActive(false);
		bool hideState = SingletonObject.getInstance<BasicGameData>().HideAllTeammates;
		bool flag = hideState;
		if (flag)
		{
			for (int i = 0; i < 3; i++)
			{
				Transform panel = rootSlot.Find(i.ToString());
				bool flag2 = null != panel;
				if (flag2)
				{
					this.UpdateCombatTeammate(i, -1);
				}
			}
		}
		else
		{
			for (int index = 0; index < data.Count; index++)
			{
				int charId = data[index];
				Transform transform = rootSlot.Find(index.ToString());
				Refers panel2 = (transform != null) ? transform.GetComponent<Refers>() : null;
				bool flag3 = panel2 == null;
				if (flag3)
				{
					panel2 = Object.Instantiate<GameObject>(template.gameObject, rootSlot).GetComponent<Refers>();
				}
				panel2.name = index.ToString();
				panel2.gameObject.SetActive(true);
				this.UpdateCombatTeammate(index, charId);
			}
		}
	}

	// Token: 0x0600331F RID: 13087 RVA: 0x0019511C File Offset: 0x0019331C
	private void UpdateCombatTeammate(ArgumentBox argBox)
	{
		int index;
		int charId;
		bool flag = argBox.Get("index", out index) && argBox.Get("characterId", out charId);
		if (flag)
		{
			this.UpdateCombatTeammate(index, charId);
		}
	}

	// Token: 0x06003320 RID: 13088 RVA: 0x00195158 File Offset: 0x00193358
	private void UpdateCombatTeammate(int index, int charId)
	{
		UI_Bottom.<>c__DisplayClass124_0 CS$<>8__locals1 = new UI_Bottom.<>c__DisplayClass124_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.charId = charId;
		CS$<>8__locals1.index = index;
		RectTransform rootSlot = this._groupChar.CGet<RectTransform>("CombatCharHolder");
		Refers panel = rootSlot.Find(CS$<>8__locals1.index.ToString()).GetComponent<Refers>();
		CS$<>8__locals1.isExist = (CS$<>8__locals1.charId >= 0);
		CS$<>8__locals1.avatar = panel.CGet<Game.Components.Avatar.Avatar>("Avatar");
		CS$<>8__locals1.selectButton = panel.CGet<CButtonObsolete>("Select");
		RectTransform stickerHolder = panel.CGet<RectTransform>("StickerHolder");
		CS$<>8__locals1.btnChat = panel.CGet<CButtonObsolete>("BtnChat");
		CS$<>8__locals1.btnExchange = panel.CGet<CButtonObsolete>("BtnExchange");
		TriggerPanel triggerPanel = panel.CGet<TriggerPanel>("MenuBg");
		triggerPanel.Init(new Func<bool>(this.CheckCanEnterPanel), null);
		stickerHolder.gameObject.SetActive(false);
		CS$<>8__locals1.list = new List<int>
		{
			CS$<>8__locals1.charId
		};
		CS$<>8__locals1.list.AddRange(SingletonObject.getInstance<CharacterMonitorModel>().GetTaiwuTeamCharIds());
		CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this, CS$<>8__locals1.list, new AsyncMethodCallbackDelegate(CS$<>8__locals1.<UpdateCombatTeammate>g__OnGetCharacterDisplayDataList|0));
		bool locked = !this._forcedCanOperateInCharacterMenu || (!this._forcedCanExchangeChar && !CS$<>8__locals1.isExist);
		panel.CGet<GameObject>("Locked").SetActive(locked);
	}

	// Token: 0x06003321 RID: 13089 RVA: 0x001952B7 File Offset: 0x001934B7
	private bool CheckCanEnterPanel()
	{
		return !WorldMapModel.Traveling && !UIElement.PartWorld.Exist;
	}

	// Token: 0x06003322 RID: 13090 RVA: 0x001952D0 File Offset: 0x001934D0
	private bool CheckCanEnterLifePanel()
	{
		return this._forcedCanOpenCharacterMenu && SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(27);
	}

	// Token: 0x06003323 RID: 13091 RVA: 0x001952EC File Offset: 0x001934EC
	private void OnMonthChange(ArgumentBox argBox)
	{
		this._needUpdateGroupAvatar = true;
		this._teammateIdList.Add(this.TaiwuCharId);
		CharacterDomainMethod.Call.GetCharacterDisplayDataList(this.Element.GameDataListenerId, this._teammateIdList);
		bool flag = this._villagerIdList.Count > 0;
		if (flag)
		{
			CharacterDomainMethod.Call.GetCharacterDisplayDataList(this.Element.GameDataListenerId, this._villagerIdList);
		}
		this._teammateIdList.Remove(this.TaiwuCharId);
		this._advanceActionPointConfirmed = false;
		this._advanceLoopingNeigongConfirmed = false;
		this._advanceInventoryOverflowConfirmed = false;
		this.AsyncRefreshReadingBookPagesInfoBalls();
	}

	// Token: 0x06003324 RID: 13092 RVA: 0x00195384 File Offset: 0x00193584
	private void OnDaysInMonthChange(ArgumentBox argBox)
	{
		this.SendTeammateBubbleRequest();
		this.UpdateDate(this._timeDisk);
		this.UpdateObtainedNeili();
		this.UpdateReading();
		this.UpdateActiveReadAndLoopButton(-1);
		bool flag = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays() > 0;
		if (flag)
		{
			this._advanceInventoryOverflowConfirmed = false;
		}
		bool flag2 = !UIElement.WorldMap.UiBaseAs<ViewWorldMap>().IsDoingMove;
		if (flag2)
		{
			this.UpdateMapBlockInfo(new ArgumentBox().SetObject("block", this._blockData));
			this.RefreshWorkButton(UI_Bottom.ERefreshWorkButtonMoving.Unknown);
		}
	}

	// Token: 0x06003325 RID: 13093 RVA: 0x00195414 File Offset: 0x00193614
	private void DoAdvance()
	{
		bool flag = !SingletonObject.getInstance<TutorialChapterModel>().CanAdvanceForChapter2() || !SingletonObject.getInstance<TutorialChapterModel>().CanAdvanceForChapter5(this._loopingNeigong) || !SingletonObject.getInstance<TutorialChapterModel>().CanAdvanceForChapter6(this._currentReadingBookKey);
		if (!flag)
		{
			bool flag2 = UIElement.EventWindow.Exist || UIElement.BlackMask.Exist || UIElement.GetItem.Exist;
			if (!flag2)
			{
				bool flag3 = WorldMapModel.Traveling || UIElement.PartWorld.Exist;
				if (!flag3)
				{
					GEvent.OnEvent(UiEvents.AdventureClickAdvanceBtn, null);
					bool activeInHierarchy = base.CGet<Refers>("AdvanceDays").gameObject.activeInHierarchy;
					if (activeInHierarchy)
					{
						base.CGet<Refers>("AdvanceDays").gameObject.SetActive(false);
						this.OnClickAdvanceMonth(null);
					}
					else
					{
						this.RefreshAdvanceDaysInfo();
						base.CGet<Refers>("AdvanceDays").gameObject.SetActive(true);
					}
				}
			}
		}
	}

	// Token: 0x06003326 RID: 13094 RVA: 0x00195514 File Offset: 0x00193714
	private void OpenTeammateCharacterMenu(int charId, ECharacterSubToggleBase subToggle = ECharacterSubToggleBase.None, ECharacterSubPage subPage = ECharacterSubPage.None)
	{
		bool flag = !UI_Bottom.CanOpenCharacterMenuByLegacyAndTutorial();
		if (!flag)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharacterId", charId);
			argBox.Set("CanOperate", this._forcedCanOperateInCharacterMenu);
			argBox.SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(subToggle, subPage));
			UIElement.CharacterMenu.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}
	}

	// Token: 0x06003327 RID: 13095 RVA: 0x0019558C File Offset: 0x0019378C
	public static bool CanOpenCharacterMenuByLegacyAndTutorial()
	{
		bool flag = SingletonObject.getInstance<DisplayTriggerModel>().LegacyPassingState > 0;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = !SingletonObject.getInstance<TutorialChapterModel>().OpenCharacterMenuEnable;
			result = !flag2;
		}
		return result;
	}

	// Token: 0x06003328 RID: 13096 RVA: 0x001955C9 File Offset: 0x001937C9
	private void OnClickCollectResource(sbyte resourceType)
	{
		this._selectedResourceType = resourceType;
		this.TimeCollect();
	}

	// Token: 0x06003329 RID: 13097 RVA: 0x001955DC File Offset: 0x001937DC
	private unsafe void TimeCollect()
	{
		bool flag = SingletonObject.getInstance<TimeManager>().IsActionPointRunningOut();
		if (!flag)
		{
			this.WaitingTimeCollect = true;
			MapBlockData mapBlockData = SingletonObject.getInstance<WorldMapModel>().PlayerAtBlock;
			this._collectResourceIsMax = (*(ref mapBlockData.CurrResources.Items.FixedElementField + (IntPtr)this._selectedResourceType * 2) == *(ref mapBlockData.MaxResources.Items.FixedElementField + (IntPtr)this._selectedResourceType * 2));
			MapDomainMethod.Call.CollectResource(this.Element.GameDataListenerId, this.TaiwuCharId, this._selectedResourceType);
		}
	}

	// Token: 0x0600332A RID: 13098 RVA: 0x00195668 File Offset: 0x00193868
	public static void ShowSelectVillagerWindow(List<int> charIdList, Dictionary<int, CharacterDisplayData> charDisplayDataDict, Action<int> callback, int exchangingWorkVillagerId = -1)
	{
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.SetObject("callback", callback);
		argBox.SetObject("charIdList", charIdList);
		argBox.SetObject("displayDataDict", charDisplayDataDict);
		bool flag = exchangingWorkVillagerId >= 0;
		if (flag)
		{
			argBox.Set("selectedCharId", exchangingWorkVillagerId);
		}
		argBox.Set("ShowNone", true);
		argBox.Set("ShowWorking", true);
		argBox.Set("ShowInfected", true);
		CommandManager.AddCommandShowUI(EPriority.ShowUINormal, UIElement.SelectCharLegacy, argBox);
	}

	// Token: 0x0600332B RID: 13099 RVA: 0x001956F4 File Offset: 0x001938F4
	private void OnSelectWorkingVillageChar(int charId)
	{
		UI_Bottom.<>c__DisplayClass135_0 CS$<>8__locals1 = new UI_Bottom.<>c__DisplayClass135_0();
		CS$<>8__locals1.charId = charId;
		CS$<>8__locals1.<>4__this = this;
		bool flag = CS$<>8__locals1.charId >= 0;
		if (flag)
		{
			Location location = new Location(this._blockData.AreaId, this._blockData.BlockId);
			CharacterDisplayData characterDisplayData;
			bool flag2 = this._charDisplayDataDict.TryGetValue(CS$<>8__locals1.charId, out characterDisplayData);
			if (flag2)
			{
				bool isAutoDispatch = this._isAutoDispatch;
				if (isAutoDispatch)
				{
					CS$<>8__locals1.<OnSelectWorkingVillageChar>g__ConfirmAction|0();
				}
				else
				{
					BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
					bool marked = buildingModel.CheckBlockIsMarked(location);
					DialogCmd cmd = new DialogCmd
					{
						Title = LocalStringManager.Get(LanguageKey.LK_Villager_Collect_Resource_Tip_Title),
						Content = LocalStringManager.GetFormat(marked ? LanguageKey.LK_Villager_Collect_Resource_Confirm : LanguageKey.LK_Villager_Collect_Resource_And_Mark_Confirm, NameCenter.GetCharMonasticTitleOrNameByDisplayData(characterDisplayData, false, false)).ColorReplace(),
						Type = 1,
						Yes = new Action(CS$<>8__locals1.<OnSelectWorkingVillageChar>g__ConfirmAction|0),
						No = null
					};
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
					UIManager.Instance.MaskUI(UIElement.Dialog);
				}
			}
		}
		else
		{
			this._mapModel.RequestStopVillagerWorkOnMap(this._blockData.GetLocation(), true);
			this.RefreshAutoDispatchBtn(false, this._btnAutoDispatch);
		}
	}

	// Token: 0x0600332C RID: 13100 RVA: 0x00195848 File Offset: 0x00193A48
	private void OnBlockDataChange(ArgumentBox argsBox)
	{
		MapBlockData data;
		argsBox.Get<MapBlockData>("Data", out data);
		bool flag = data.AreaId == this._blockData.AreaId && data.BlockId == this._blockData.BlockId;
		if (flag)
		{
			this._blockData = data;
			argsBox.SetObject("block", data);
			this.UpdateMapBlockInfo(argsBox);
			this.RefreshWorkButton(UI_Bottom.ERefreshWorkButtonMoving.Unknown);
		}
	}

	// Token: 0x0600332D RID: 13101 RVA: 0x001958B8 File Offset: 0x00193AB8
	private void UpdateMapBlockInfo(ArgumentBox argBox)
	{
		bool flag = argBox != null;
		if (flag)
		{
			MapBlockData block;
			argBox.Get<MapBlockData>("block", out block);
			bool flag2 = block != null;
			if (!flag2)
			{
				return;
			}
			this._blockData = block;
		}
		MapAreaData areaData = this._mapModel.Areas[(int)this._blockData.AreaId];
		bool isCityTown = this._blockData.IsCityTown();
		StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
		Location location = new Location(this._blockData.AreaId, this._blockData.BlockId);
		bool flag3 = location.IsValid();
		if (flag3)
		{
			MapDomainMethod.Call.GetBlockData(this.Element.GameDataListenerId, this._blockData.AreaId, this._blockData.BlockId);
		}
		TextMeshProUGUI text = this._mapBlockInfo.CGet<TextMeshProUGUI>("Desc");
		MapBlockItem blockConfig = this._blockData.GetConfig();
		strBuilder.Clear();
		bool flag4 = blockConfig != null;
		if (flag4)
		{
			strBuilder.Append(blockConfig.Desc);
		}
		text.text = strBuilder.ToString().ColorReplace();
		this._mapBlockInfo.CGet<CScrollRectLegacy>("DescScroll").ScrollTo(Vector2.zero, 0.3f);
		this.ChangePanelActive(UI_Bottom.EPanelType.None);
		this.RefreshWorkButton(UI_Bottom.ERefreshWorkButtonMoving.Unknown);
		EasyPool.Free<StringBuilder>(strBuilder);
	}

	// Token: 0x0600332E RID: 13102 RVA: 0x00195A04 File Offset: 0x00193C04
	private void RefreshWorkButton(UI_Bottom.ERefreshWorkButtonMoving moving = UI_Bottom.ERefreshWorkButtonMoving.Unknown)
	{
		bool flag = moving == UI_Bottom.ERefreshWorkButtonMoving.Moving;
		if (flag)
		{
			this._isTaiwuMovingOnMap = true;
		}
		else
		{
			bool flag2 = moving == UI_Bottom.ERefreshWorkButtonMoving.StopMoving;
			if (flag2)
			{
				this._isTaiwuMovingOnMap = false;
			}
		}
		CButtonObsolete dispatch = this._mapBlockInfo.CGet<CButtonObsolete>("Dispatch");
		CButtonObsolete mark = this._mapBlockInfo.CGet<CButtonObsolete>("Mark");
		this.ChangeButtonInteractable(dispatch, !this._isTaiwuMovingOnMap, null);
		this.ChangeButtonInteractable(mark, !this._isTaiwuMovingOnMap, null);
		CButtonObsolete collect = this._mapBlockInfo.CGet<CButtonObsolete>("CollectResource");
		CButtonObsolete treasure = this._mapBlockInfo.CGet<CButtonObsolete>("FindTreasure");
		bool isTaiwuMovingOnMap = this._isTaiwuMovingOnMap;
		if (isTaiwuMovingOnMap)
		{
			this.ChangeButtonInteractableWithStyle(collect, false, false, null);
			this.ChangeButtonInteractableWithStyle(treasure, false, false, null);
		}
		else
		{
			MapBlockData blockData = this._blockData;
			MapBlockItem blockConfig = (blockData != null) ? blockData.GetConfig() : null;
			bool flag3 = blockConfig != null;
			if (flag3)
			{
				this.RefreshMarkButton();
				ValueTuple<bool, string> checkCollect = this.CanCollectResource(blockConfig.ResourceCollectionType);
				this.ChangeButtonInteractableWithStyle(collect, checkCollect.Item1, true, checkCollect.Item2);
				ValueTuple<bool, string> checkTreasure = this.CanFindTreasure();
				this.ChangeButtonInteractableWithStyle(treasure, checkTreasure.Item1, true, checkTreasure.Item2);
			}
		}
	}

	// Token: 0x0600332F RID: 13103 RVA: 0x00195B38 File Offset: 0x00193D38
	private void OnRefreshWorkButton(ArgumentBox argumentBox)
	{
		bool isStartMove;
		argumentBox.Get("isStartMove", out isStartMove);
		this.RefreshWorkButton(isStartMove ? UI_Bottom.ERefreshWorkButtonMoving.Moving : UI_Bottom.ERefreshWorkButtonMoving.StopMoving);
	}

	// Token: 0x06003330 RID: 13104 RVA: 0x00195B64 File Offset: 0x00193D64
	private ValueTuple<bool, string> CanCollectResource(sbyte resourceCollectionType)
	{
		bool hasResource = resourceCollectionType > 0;
		bool hasTime = !SingletonObject.getInstance<TimeManager>().IsActionPointRunningOut();
		bool isCurrBlock = this._blockData.BlockId == this._mapModel.CurrentBlockId;
		string tips = (!hasResource) ? LocalStringManager.Get(LanguageKey.LK_Resource_Cannot_Collect_No_Resource_Tips) : ((!hasTime) ? LocalStringManager.Get(LanguageKey.LK_Resource_Cannot_Collect_No_Time_Tips) : ((!isCurrBlock) ? LocalStringManager.Get(LanguageKey.LK_Resource_Cannot_Collect_Not_Reach_Tips) : LocalStringManager.Get(LanguageKey.LK_Collect_Resource_Tip_Desc)));
		bool tipsEnabled = hasResource && hasTime && isCurrBlock;
		bool flag = !tipsEnabled;
		if (flag)
		{
			tips = tips.SetColor("brightred");
		}
		return new ValueTuple<bool, string>(tipsEnabled, tips);
	}

	// Token: 0x06003331 RID: 13105 RVA: 0x00195C04 File Offset: 0x00193E04
	private ValueTuple<bool, string> CanFindTreasure()
	{
		bool hasTime = SingletonObject.getInstance<TimeManager>().IsActionDayEnough(3);
		bool isCurrBlock = this._blockData.BlockId == this._mapModel.CurrentBlockId;
		string tips = (!hasTime) ? LocalStringManager.Get(LanguageKey.LK_Treasure_Cannot_Find_No_Time_Tips) : ((!isCurrBlock) ? LocalStringManager.Get(LanguageKey.LK_Treasure_Cannot_Find_Not_Reach_Tips) : LocalStringManager.Get(LanguageKey.LK_Treasure_Find_Tips_Desc));
		bool tipsEnabled = hasTime && isCurrBlock;
		bool flag = !tipsEnabled;
		if (flag)
		{
			tips = tips.SetColor("brightred");
		}
		return new ValueTuple<bool, string>(tipsEnabled, tips);
	}

	// Token: 0x06003332 RID: 13106 RVA: 0x00195C88 File Offset: 0x00193E88
	private void OnWorldMapMarkLocationChange(ArgumentBox argsBox)
	{
		short areaId;
		argsBox.Get("AreaId", out areaId);
		short blockId;
		argsBox.Get("BlockId", out blockId);
		bool hasWork;
		argsBox.Get("HasWork", out hasWork);
		this.SetMarkButton(this.CurBlockIsMarked, hasWork);
	}

	// Token: 0x06003333 RID: 13107 RVA: 0x00195CD0 File Offset: 0x00193ED0
	private void OnGameStateChange(ArgumentBox argBox)
	{
		Enum state;
		argBox.Get("newState", out state);
		bool flag = (EGameState)state == EGameState.Login;
		if (flag)
		{
			this._blockData = null;
		}
	}

	// Token: 0x06003334 RID: 13108 RVA: 0x00195D00 File Offset: 0x00193F00
	private void OnGameFunctionLockStateChange(ArgumentBox argBox)
	{
		FunctionLockManager funcLockManager = SingletonObject.getInstance<FunctionLockManager>();
		byte functionId;
		argBox.Get("FunctionId", out functionId);
		bool flag = functionId == 4;
		if (flag)
		{
			base.CGet<CButtonObsolete>("WorldMapButton").interactable = funcLockManager.IsFunctionUnlock(4);
		}
		bool flag2 = functionId == 5;
		if (flag2)
		{
			this.ChangeButtonActive(base.CGet<Refers>("MapBlockInfo").CGet<CButtonObsolete>("CollectResource"), funcLockManager.IsFunctionUnlock(5));
			this.RefreshWorkButton(UI_Bottom.ERefreshWorkButtonMoving.Unknown);
		}
		bool flag3 = functionId == 10;
		if (flag3)
		{
			this.ChangeButtonActive(base.CGet<Refers>("MapBlockInfo").CGet<CButtonObsolete>("Dispatch"), funcLockManager.IsFunctionUnlock(10));
			this.ChangeButtonActive(base.CGet<Refers>("MapBlockInfo").CGet<CButtonObsolete>("Mark"), funcLockManager.IsFunctionUnlock(10));
			this.ChangeButtonActive(base.CGet<Refers>("MapBlockInfo").CGet<CButtonObsolete>("FindTreasure"), funcLockManager.IsFunctionUnlock(10) && !SingletonObject.getInstance<TutorialChapterModel>().InGuiding);
			this.RefreshWorkButton(UI_Bottom.ERefreshWorkButtonMoving.Unknown);
		}
		bool flag4 = functionId == 13;
		if (flag4)
		{
			this.ChangeButtonActive(this._mapBlockInfo.CGet<CButtonObsolete>("SettlementInfoOpen"), funcLockManager.IsFunctionUnlock(13));
			this.RefreshWorkButton(UI_Bottom.ERefreshWorkButtonMoving.Unknown);
		}
		bool flag5 = functionId == 27;
		if (flag5)
		{
			this.RefreshProfessionButton(null);
		}
	}

	// Token: 0x06003335 RID: 13109 RVA: 0x00195E58 File Offset: 0x00194058
	private void ChangeButtonActive(CButtonObsolete button, bool active)
	{
		bool flag = button == null;
		if (!flag)
		{
			bool flag2 = this.TrySetAdventureButtonActive(button, active);
			if (!flag2)
			{
				button.gameObject.SetActive(active);
			}
		}
	}

	// Token: 0x06003336 RID: 13110 RVA: 0x00195E90 File Offset: 0x00194090
	private void EnsureMapBlockInfoAdventureButtons()
	{
		bool flag = this._mapBlockInfo == null || this._mapBlockInfoAdventureButtons != null;
		if (!flag)
		{
			this._mapBlockInfoAdventureButtons = new HashSet<CButtonObsolete>
			{
				this._mapBlockInfo.CGet<CButtonObsolete>("Dispatch"),
				this._mapBlockInfo.CGet<CButtonObsolete>("CollectResource"),
				this._mapBlockInfo.CGet<CButtonObsolete>("FindTreasure"),
				this._mapBlockInfo.CGet<CButtonObsolete>("Mark")
			};
			foreach (CButtonObsolete button in this._mapBlockInfoAdventureButtons)
			{
				bool flag2 = button == null;
				if (!flag2)
				{
					this._mapBlockInfoAdventureButtonActiveStates[button] = button.gameObject.activeSelf;
				}
			}
			this.SyncAdventureRemakeState();
		}
	}

	// Token: 0x06003337 RID: 13111 RVA: 0x00195F98 File Offset: 0x00194198
	private void SyncAdventureRemakeState()
	{
		AdventureRemakeModel adventureModel = SingletonObject.getInstance<AdventureRemakeModel>();
		this._isAdventureRemakeRunning = (((adventureModel != null) ? adventureModel.AdventureTaiwu : null) != null && !adventureModel.AdventureTaiwu.NotInAdventure);
		this.RefreshMapBlockInfoAdventureButtons();
	}

	// Token: 0x06003338 RID: 13112 RVA: 0x00195FD8 File Offset: 0x001941D8
	private void RefreshMapBlockInfoAdventureButtons()
	{
		bool flag = this._mapBlockInfoAdventureButtonActiveStates.Count == 0;
		if (!flag)
		{
			foreach (KeyValuePair<CButtonObsolete, bool> pair in this._mapBlockInfoAdventureButtonActiveStates)
			{
				CButtonObsolete button = pair.Key;
				bool flag2 = button == null;
				if (!flag2)
				{
					button.gameObject.SetActive(pair.Value && !this._isAdventureRemakeRunning);
				}
			}
		}
	}

	// Token: 0x06003339 RID: 13113 RVA: 0x00196078 File Offset: 0x00194278
	private bool TrySetAdventureButtonActive(CButtonObsolete button, bool active)
	{
		this.EnsureMapBlockInfoAdventureButtons();
		bool flag = this._mapBlockInfoAdventureButtons == null || !this._mapBlockInfoAdventureButtons.Contains(button);
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			this._mapBlockInfoAdventureButtonActiveStates[button] = active;
			button.gameObject.SetActive(active && !this._isAdventureRemakeRunning);
			result = true;
		}
		return result;
	}

	// Token: 0x0600333A RID: 13114 RVA: 0x001960E0 File Offset: 0x001942E0
	private void ChangeButtonInteractableWithStyle(CButtonObsolete button, bool interactable, bool isGrayWhenNotInteractable, string tips = null)
	{
		button.interactable = interactable;
		button.GetComponent<PointerTrigger>().enabled = interactable;
		bool flag = !tips.IsNullOrEmpty();
		if (flag)
		{
			button.GetComponent<TooltipInvoker>().PresetParam[1] = tips;
		}
		if (isGrayWhenNotInteractable)
		{
			button.transform.Find("Active").gameObject.SetActive(interactable);
			button.transform.Find("InActive").gameObject.SetActive(!interactable);
		}
		else
		{
			button.transform.Find("Active").gameObject.SetActive(true);
			button.transform.Find("InActive").gameObject.SetActive(false);
		}
	}

	// Token: 0x0600333B RID: 13115 RVA: 0x001961A0 File Offset: 0x001943A0
	private void ChangeButtonInteractable(CButtonObsolete button, bool interactable, string tips = null)
	{
		button.interactable = interactable;
		button.GetComponent<PointerTrigger>().enabled = interactable;
		bool flag = !tips.IsNullOrEmpty();
		if (flag)
		{
			button.GetComponent<TooltipInvoker>().PresetParam[1] = tips;
		}
		bool flag2 = button.transition != Selectable.Transition.SpriteSwap;
		if (!flag2)
		{
			Transform transform = button.transform.Find("Active");
			GameObject activeObject = (transform != null) ? transform.gameObject : null;
			Transform transform2 = button.transform.Find("InActive");
			GameObject inactiveObject = (transform2 != null) ? transform2.gameObject : null;
			bool flag3 = activeObject != null;
			if (flag3)
			{
				activeObject.SetActive(interactable);
			}
			bool flag4 = inactiveObject != null;
			if (flag4)
			{
				inactiveObject.SetActive(!interactable);
			}
		}
	}

	// Token: 0x0600333C RID: 13116 RVA: 0x00196257 File Offset: 0x00194457
	private void ChangePanelActive(UI_Bottom.EPanelType panelType)
	{
		this.RefreshWorkPanel(panelType == UI_Bottom.EPanelType.Work);
		this.RefreshCollectPanel(panelType == UI_Bottom.EPanelType.Collect);
		this.RefreshTreasurePanel(panelType == UI_Bottom.EPanelType.Treasure);
	}

	// Token: 0x0600333D RID: 13117 RVA: 0x0019627B File Offset: 0x0019447B
	private void ChangePanelActiveByWorkField()
	{
		this.ChangePanelActive(this._workPanelIsShow ? UI_Bottom.EPanelType.Work : UI_Bottom.EPanelType.None);
	}

	// Token: 0x0600333E RID: 13118 RVA: 0x00196291 File Offset: 0x00194491
	private void RefreshWorkPanelDelayOneFrame()
	{
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, new Action(this.ChangePanelActiveByWorkField));
	}

	// Token: 0x0600333F RID: 13119 RVA: 0x001962AC File Offset: 0x001944AC
	private void RefreshWorkPanel(bool show)
	{
		this._workPanelIsShow = show;
		VillagerWorkPanel panel = this._mapBlockInfo.CGet<VillagerWorkPanel>("WorkPanel");
		this._btnAutoDispatch = panel.CGet<CButtonObsolete>("BtnDispatch");
		this._btnAutoDispatch.gameObject.SetActive(false);
		bool flag = !show && !UIElement.VillagerRoleSelectStorageType.Exist;
		if (flag)
		{
			panel.gameObject.SetActive(false);
		}
		else
		{
			panel.gameObject.SetActive(true);
			panel.Refresh(this.Element.GameDataListenerId, this._blockData, this._charDisplayDataDict, new Action<int>(this.<RefreshWorkPanel>g__OnExchangeCharacterIdChanged|157_0), this._unlockedWorkingList);
			this._btnAutoDispatch.ClearAndAddListener(delegate
			{
				TaiwuDomainMethod.AsyncCall.GetVillagersForWork(this, true, this._blockData.GetConfig().ResourceCollectionType > 0, delegate(int offset, RawDataPool dataPool)
				{
					List<int> charIdList = new List<int>();
					Serializer.Deserialize(dataPool, offset, ref charIdList);
					bool flag2 = charIdList.Count > 0;
					if (flag2)
					{
						this._isAutoDispatch = true;
						this.OnSelectWorkingVillageChar(charIdList.First<int>());
						this._isAutoDispatch = false;
					}
				});
			});
		}
	}

	// Token: 0x06003340 RID: 13120 RVA: 0x00196374 File Offset: 0x00194574
	private void OnRefreshWorkPanel(ArgumentBox argumentBox)
	{
		bool show;
		argumentBox.Get("show", out show);
		this.ChangePanelActive(show ? UI_Bottom.EPanelType.Work : UI_Bottom.EPanelType.None);
	}

	// Token: 0x06003341 RID: 13121 RVA: 0x001963A0 File Offset: 0x001945A0
	private void RefreshCollectPanel(bool show)
	{
		this._collectPanelIsShow = show;
		Refers panel = this._mapBlockInfo.CGet<Refers>("CollectPanel");
		bool flag = !show;
		if (flag)
		{
			panel.gameObject.SetActive(false);
		}
		else
		{
			panel.gameObject.SetActive(true);
			RectTransform selectorResource = panel.CGet<RectTransform>("ResourceSelector");
			MapBlockItem blockConfig = this._blockData.GetConfig();
			Dictionary<sbyte, CButtonObsolete> resourceButtons = EasyPool.Get<Dictionary<sbyte, CButtonObsolete>>();
			resourceButtons.Clear();
			resourceButtons.Add(0, selectorResource.Find("Food").GetComponent<CButtonObsolete>());
			resourceButtons.Add(5, selectorResource.Find("Herbal").GetComponent<CButtonObsolete>());
			resourceButtons.Add(3, selectorResource.Find("Jade").GetComponent<CButtonObsolete>());
			resourceButtons.Add(2, selectorResource.Find("Stone").GetComponent<CButtonObsolete>());
			resourceButtons.Add(4, selectorResource.Find("Silk").GetComponent<CButtonObsolete>());
			resourceButtons.Add(1, selectorResource.Find("Wood").GetComponent<CButtonObsolete>());
			Transform labelTrans = panel.transform.Find("Result/Label");
			labelTrans.GetComponent<TextLanguage>().SetLanguage();
			labelTrans.GetComponent<TMPTextSpriteHelper>().Parse();
			TutorialChapterModel tutorialChapterModel = SingletonObject.getInstance<TutorialChapterModel>();
			bool tutorialCollect = tutorialChapterModel.InGuiding && tutorialChapterModel.TutorialChapterIndex == 2;
			foreach (KeyValuePair<sbyte, CButtonObsolete> pair in resourceButtons)
			{
				sbyte resourceType = pair.Key;
				CButtonObsolete button = pair.Value;
				button.ClearAndAddListener(delegate
				{
					this._selectedResourceType = resourceType;
					this.TimeCollect();
				});
				Transform title = button.transform.Find("Title");
				bool canInteract = blockConfig.ResourceCollectionType > 0;
				bool flag2 = tutorialCollect;
				if (flag2)
				{
					canInteract = (resourceType == 1);
				}
				button.transform.Find("Active").gameObject.SetActive(false);
				bool flag3 = canInteract;
				if (flag3)
				{
					button.interactable = true;
					title.Find("Label").gameObject.SetActive(true);
					title.Find("Disable").gameObject.SetActive(false);
				}
				else
				{
					GameObject disable = title.Find("Disable").gameObject;
					button.interactable = false;
					title.Find("Label").gameObject.SetActive(false);
					disable.SetActive(true);
					disable.GetComponent<TextMeshProUGUI>().text = Config.ResourceType.Instance[resourceType].Name;
				}
			}
			EasyPool.Free<Dictionary<sbyte, CButtonObsolete>>(resourceButtons);
		}
	}

	// Token: 0x06003342 RID: 13122 RVA: 0x0019667C File Offset: 0x0019487C
	private void RefreshTreasurePanel(bool show)
	{
		this._treasurePanelIsShow = show;
		this._mapBlockInfo.CGet<GameObject>("TreasurePanel").SetActive(show);
	}

	// Token: 0x06003343 RID: 13123 RVA: 0x001966A0 File Offset: 0x001948A0
	private void RefreshAutoDispatchBtn(bool curBlockHaveChar, CButtonObsolete btnAutoDispatch)
	{
		btnAutoDispatch.gameObject.SetActive(true);
		TaiwuDomainMethod.AsyncCall.GetAllVillagersAvailableForWork(this, true, delegate(int offset, RawDataPool dataPool)
		{
			bool flag = btnAutoDispatch == null;
			if (!flag)
			{
				List<int> charIdList = new List<int>();
				Serializer.Deserialize(dataPool, offset, ref charIdList);
				bool canAutoDispatch = charIdList.Count > 0 && !curBlockHaveChar;
				btnAutoDispatch.interactable = canAutoDispatch;
				btnAutoDispatch.GetComponent<TooltipInvoker>().enabled = !canAutoDispatch;
				btnAutoDispatch.GetComponent<PointerTrigger>().enabled = canAutoDispatch;
			}
		});
	}

	// Token: 0x06003344 RID: 13124 RVA: 0x001966E8 File Offset: 0x001948E8
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

	// Token: 0x06003345 RID: 13125 RVA: 0x00196700 File Offset: 0x00194900
	private void OnTopUiChanged(ArgumentBox argBox)
	{
		this.DisableTeammateBubble();
		bool flag = this.WaitingTimeCollect && !UIManager.Instance.IsFocusElement(UIElement.CollectResource) && !UIManager.Instance.IsFocusElement(UIElement.GetItem);
		if (flag)
		{
			this.WaitingTimeCollect = false;
		}
		bool flag2 = UIManager.Instance.IsFocusElement(UIElement.Bottom);
		if (flag2)
		{
			bool flag3 = SingletonObject.getInstance<DisplayTriggerModel>().LegacyPassingState == 0;
			if (flag3)
			{
				bool flag4 = this._learnedNeigongList.Count > 0;
				if (flag4)
				{
					CombatSkillModel.GetCombatSkillDisplayData(this.Element.GameDataListenerId, this.TaiwuCharId, this._learnedNeigongList);
				}
				this.UpdateAllCombatTeammates();
			}
			this.RefreshWorkButton(UI_Bottom.ERefreshWorkButtonMoving.Unknown);
		}
		else
		{
			this.HideCharPanel();
		}
		bool flag5 = UIManager.Instance.IsFocusElement(UIElement.EventWindow);
		if (flag5)
		{
			this.ChangeButtonInteractable(this._mapBlockInfo.CGet<CButtonObsolete>("CollectResource"), false, null);
			this.ChangeButtonInteractable(this._mapBlockInfo.CGet<CButtonObsolete>("FindTreasure"), false, null);
			this.ChangeButtonInteractable(this._mapBlockInfo.CGet<CButtonObsolete>("Dispatch"), false, null);
			this.ChangeButtonInteractable(this._mapBlockInfo.CGet<CButtonObsolete>("Mark"), false, null);
		}
		this.RefreshLifePanelButton();
	}

	// Token: 0x06003346 RID: 13126 RVA: 0x00196844 File Offset: 0x00194A44
	private void OnTaiwuCharIdChange(ArgumentBox argumentBox)
	{
		int oldTaiwuCharId;
		argumentBox.Get("OldTaiwuCharId", out oldTaiwuCharId);
		int newTaiwuCharId;
		argumentBox.Get("NewTaiwuCharId", out newTaiwuCharId);
		base.RemoveMonitorFieldId(4, 0, (ulong)oldTaiwuCharId);
		base.AppendMonitorFieldId(new UIBase.MonitorDataField(4, 0, (ulong)newTaiwuCharId, UI_Bottom.ListenTaiwuFieldIds));
		this.UpdateObtainedNeili();
		this.UpdateReading();
	}

	// Token: 0x06003347 RID: 13127 RVA: 0x0019689C File Offset: 0x00194A9C
	private void OnClickAdvanceMonth(ArgumentBox argBox)
	{
		bool flag = WorldMapModel.Traveling || UIElement.PartWorld.Exist;
		if (!flag)
		{
			this._advanceDialogCmd.Title = LocalStringManager.Get(LanguageKey.UI_AdvanceMonth_TipTitle);
			bool flag2 = !this._advanceActionPointConfirmed && !SingletonObject.getInstance<TimeManager>().IsActionPointRunningOut();
			if (flag2)
			{
				this._advanceDialogCmd.Content = LocalStringManager.Get(LanguageKey.LK_AdvanceMonth_LeftActionPoint).ColorReplace() + "\n" + LocalStringManager.Get(LanguageKey.LK_Advance_Month_Confirm);
				this._advanceDialogCmd.Yes = delegate()
				{
					this._advanceActionPointConfirmed = true;
					GEvent.OnEvent(UiEvents.ShowAdvanceMonthConfirm, argBox);
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", this._advanceDialogCmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				bool flag3 = this._loopingNeigong < 0 && !this._advanceLoopingNeigongConfirmed && this._learnedNeigongList.Exists((short id) => !this._combatSkillDisplayDataDict[id].Revoked);
				if (flag3)
				{
					this._advanceDialogCmd.Content = LocalStringManager.Get(LanguageKey.LK_Advance_Month_Warn_No_Loop_Neigong).ColorReplace() + "\n" + LocalStringManager.Get(LanguageKey.LK_Advance_Month_Confirm);
					this._advanceDialogCmd.Yes = delegate()
					{
						this._advanceLoopingNeigongConfirmed = true;
						GEvent.OnEvent(UiEvents.ShowAdvanceMonthConfirm, argBox);
					};
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", this._advanceDialogCmd));
					UIManager.Instance.MaskUI(UIElement.Dialog);
				}
				else
				{
					bool flag4 = !this._advanceInventoryOverflowConfirmed;
					if (flag4)
					{
						StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
						strBuilder.Clear();
						bool flag5 = this._inventoryCurrLoad > this._inventoryMaxLoad;
						if (flag5)
						{
							strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Advance_Month_Warn_Inventory_Overflow).ColorReplace() + "\n");
						}
						bool flag6 = this._warehouseCurrLoad > this._warehouseMaxLoad;
						if (flag6)
						{
							strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Advance_Month_Warn_Warehouse_Overflow).ColorReplace() + "\n");
						}
						bool flag7 = strBuilder.Length > 0;
						if (flag7)
						{
							strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Advance_Month_Warn_Overflow).ColorReplace());
							this._advanceDialogCmd.Content = string.Format("{0}\n{1}", strBuilder, LocalStringManager.Get(LanguageKey.LK_Advance_Month_Confirm));
							this._advanceDialogCmd.Yes = delegate()
							{
								this._advanceInventoryOverflowConfirmed = true;
								GEvent.OnEvent(UiEvents.ShowAdvanceMonthConfirm, argBox);
							};
							UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", this._advanceDialogCmd));
							UIManager.Instance.MaskUI(UIElement.Dialog);
							EasyPool.Free<StringBuilder>(strBuilder);
							return;
						}
						EasyPool.Free<StringBuilder>(strBuilder);
					}
					AudioManager.Instance.PlaySound(string.Format("SFX_tex_month_{0}", SingletonObject.getInstance<TimeManager>().GetMonthInCurrYear()), false, false);
					Action advanceAction;
					bool flag8 = argBox != null && argBox.Get<Action>("callback", out advanceAction) && advanceAction != null;
					if (flag8)
					{
						advanceAction();
					}
					else
					{
						this.AdvanceMonth();
					}
				}
			}
		}
	}

	// Token: 0x06003348 RID: 13128 RVA: 0x00196BC8 File Offset: 0x00194DC8
	private void RequestAdvanceMonth(ArgumentBox argBox)
	{
		this.OnClickAdvanceMonth(null);
	}

	// Token: 0x06003349 RID: 13129 RVA: 0x00196BD3 File Offset: 0x00194DD3
	private void AdvanceMonth()
	{
		GlobalDomainMethod.AsyncCall.CheckDriveSpace(this, delegate(int offset, RawDataPool dataPool)
		{
			bool hasSpace = false;
			Serializer.Deserialize(dataPool, offset, ref hasSpace);
			bool flag = hasSpace;
			if (flag)
			{
				UI_Bottom.<AdvanceMonth>g__Action|167_1();
			}
			else
			{
				string archiveDirPath = GameApp.GetArchiveDirPath();
				string disk = Path.GetPathRoot(archiveDirPath);
				string title = LocalStringManager.Get(LanguageKey.LK_Save_CheckDiskSpace_Title);
				string content = LocalStringManager.GetFormat(LanguageKey.LK_Save_CheckDiskSpace_Content, disk).ColorReplace();
				CommonUtils.ShowConfirmDialog(title, content, new Action(UI_Bottom.<AdvanceMonth>g__Action|167_1), null, EDialogType.None);
			}
		});
	}

	// Token: 0x0600334A RID: 13130 RVA: 0x00196C00 File Offset: 0x00194E00
	private void RefreshAdvanceDaysInfo()
	{
		UI_Bottom.<>c__DisplayClass168_0 CS$<>8__locals1 = new UI_Bottom.<>c__DisplayClass168_0();
		CS$<>8__locals1.advanceDaysRefer = base.CGet<Refers>("AdvanceDays");
		CS$<>8__locals1.slider = CS$<>8__locals1.advanceDaysRefer.CGet<CSliderLegacy>("AdvanceDaysSlider");
		CS$<>8__locals1.leftDays = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
		CS$<>8__locals1.slider.minValue = 0f;
		CS$<>8__locals1.slider.maxValue = (float)CS$<>8__locals1.leftDays;
		CS$<>8__locals1.slider.value = CS$<>8__locals1.slider.minValue;
		CS$<>8__locals1.<RefreshAdvanceDaysInfo>g__UpdateActionPointText|1();
		CS$<>8__locals1.slider.onValueChanged.AddListener(delegate(float value)
		{
			base.<RefreshAdvanceDaysInfo>g__UpdateActionPointText|1();
		});
	}

	// Token: 0x0600334B RID: 13131 RVA: 0x00196CAC File Offset: 0x00194EAC
	private void InitAdvanceDaysBtn()
	{
		base.CGet<Refers>("AdvanceDays").gameObject.SetActive(false);
		Refers advanceDaysRefer = base.CGet<Refers>("AdvanceDays");
		CSliderLegacy slider = advanceDaysRefer.CGet<CSliderLegacy>("AdvanceDaysSlider");
		advanceDaysRefer.CGet<CButton>("AdvanceDaysConfirm").ClearAndAddListener(delegate
		{
			this.CGet<Refers>("AdvanceDays").gameObject.SetActive(false);
			bool flag = slider.value == 0f;
			if (flag)
			{
				this.OnClickAdvanceMonth(null);
			}
			else
			{
				bool notInAdventure = SingletonObject.getInstance<AdventureRemakeModel>().AdventureTaiwu.NotInAdventure;
				if (notInAdventure)
				{
					WorldDomainMethod.Call.AdvanceDaysInMonth((int)slider.value);
				}
				else
				{
					GEvent.OnEvent(UiEvents.AdventureAdvanceDaysSet, EasyPool.Get<ArgumentBox>().Set("AdvanceDays", (int)slider.value));
				}
			}
		});
		advanceDaysRefer.CGet<CButton>("AdvanceDaysCancel").ClearAndAddListener(delegate
		{
			this.CGet<Refers>("AdvanceDays").gameObject.SetActive(false);
		});
	}

	// Token: 0x0600334C RID: 13132 RVA: 0x00196D35 File Offset: 0x00194F35
	private void OnPlayerBlockChange(ArgumentBox argumentBox)
	{
		this.HideCharPanel();
	}

	// Token: 0x0600334D RID: 13133 RVA: 0x00196D3F File Offset: 0x00194F3F
	private void OnAdvancingMonthStateChange(ArgumentBox argbox)
	{
		this.RefreshAllHealBtn();
	}

	// Token: 0x0600334E RID: 13134 RVA: 0x00196D4C File Offset: 0x00194F4C
	public void UpdateDate(Refers timeDisk)
	{
		int actionPointCurrMonth = SingletonObject.getInstance<BasicGameData>().ActionPointCurrMonth;
		timeDisk.CGet<TextMeshProUGUI>("LeftDay").text = string.Format("{0}", actionPointCurrMonth / 10);
		timeDisk.CGet<TextMeshProUGUI>("TotalDay").text = string.Format("{0}", TimeManager.ActionPointMax / 10);
		float fillAmount = (float)actionPointCurrMonth / (float)TimeManager.ActionPointMax;
		timeDisk.CGet<CImage>("ExtraProgress").fillAmount = fillAmount;
		RectTransform extraProgressCurrent = timeDisk.CGet<RectTransform>("ExtraProgressCurrent");
		float totalHeight = extraProgressCurrent.parent.GetComponent<RectTransform>().rect.height;
		extraProgressCurrent.anchoredPosition = new Vector2(0f, totalHeight * fillAmount);
	}

	// Token: 0x0600334F RID: 13135 RVA: 0x00196E07 File Offset: 0x00195007
	private void SetTimeDiskLocked(bool isLocked)
	{
		this._timeDisk.CGet<CImage>("Locked").gameObject.SetActive(isLocked);
	}

	// Token: 0x06003350 RID: 13136 RVA: 0x00196E28 File Offset: 0x00195028
	private void SetRightPartVisible(bool visible, bool timeDiskVisible, bool backendVisible, bool backendForceVisible = false)
	{
		base.CGet<Refers>("TimeDisk").gameObject.SetActive(timeDiskVisible);
		base.CGet<Refers>("MapBlockInfo").gameObject.SetActive(visible);
		base.CGet<CButtonObsolete>("WorldMapButton").gameObject.SetActive(visible);
		base.CGet<GameObject>("BtnLayout").SetActive(visible);
		base.CGet<Refers>("TogLayout").gameObject.SetActive(visible);
	}

	// Token: 0x06003351 RID: 13137 RVA: 0x00196EA4 File Offset: 0x001950A4
	private void OnSetBottomRightPartVisible(ArgumentBox argumentBox)
	{
		bool visible;
		argumentBox.Get("visible", out visible);
		bool timeDiskVisible;
		argumentBox.Get("timeDiskVisible", out timeDiskVisible);
		bool backendVisible;
		argumentBox.Get("backendVisible", out backendVisible);
		bool backendForceVisible;
		argumentBox.Get("backendForceVisible", out backendForceVisible);
		this.SetRightPartVisible(visible, timeDiskVisible, backendVisible, backendForceVisible);
	}

	// Token: 0x06003352 RID: 13138 RVA: 0x00196EF5 File Offset: 0x001950F5
	private void OnAdventureRemakeEnter(ArgumentBox argumentBox)
	{
		this.EnsureMapBlockInfoAdventureButtons();
		this._isAdventureRemakeRunning = true;
		this.ChangePanelActive(UI_Bottom.EPanelType.None);
		this.RefreshMapBlockInfoAdventureButtons();
	}

	// Token: 0x06003353 RID: 13139 RVA: 0x00196F15 File Offset: 0x00195115
	private void OnAdventureRemakeExit(ArgumentBox argumentBox)
	{
		this.EnsureMapBlockInfoAdventureButtons();
		this._isAdventureRemakeRunning = false;
		this.RefreshMapBlockInfoAdventureButtons();
	}

	// Token: 0x06003354 RID: 13140 RVA: 0x00196F30 File Offset: 0x00195130
	private void SetInteractable(bool canOpenCharacterMenu, bool canOperateInCharacterMenu, bool canExchangeChar)
	{
		this._forcedCanOperateInCharacterMenu = canOperateInCharacterMenu;
		this._forcedCanOpenCharacterMenu = canOpenCharacterMenu;
		this._forcedCanExchangeChar = canExchangeChar;
		this.UpdateAllCombatTeammates();
		base.CGet<CButtonObsolete>("TaiwuChar").interactable = this._forcedCanOpenCharacterMenu;
		this._readAndLoop.SetInteractable(this._forcedCanOpenCharacterMenu);
		this.RefreshProfessionButton(null);
		this.RefreshLifePanelButton();
		this.RefreshAllHealBtn();
		this.UpdateMainUiCustomButtons();
	}

	// Token: 0x06003355 RID: 13141 RVA: 0x00196FA0 File Offset: 0x001951A0
	private void OnSetBottomInteractable(ArgumentBox argumentBox)
	{
		bool canOpenCharacterMenu;
		argumentBox.Get("canOpenCharacterMenu", out canOpenCharacterMenu);
		bool canOperateInCharacterMenu;
		argumentBox.Get("canOperateInCharacterMenu", out canOperateInCharacterMenu);
		bool canExchangeChar;
		argumentBox.Get("canExchangeChar", out canExchangeChar);
		this.SetInteractable(canOpenCharacterMenu, canOperateInCharacterMenu, canExchangeChar);
	}

	// Token: 0x06003356 RID: 13142 RVA: 0x00196FE4 File Offset: 0x001951E4
	private void SetBuildingBtnShow(bool b)
	{
		base.CGet<GameObject>("BtnLayout").SetActive(b);
		base.CGet<Refers>("MapBlockInfo").gameObject.SetActive(b);
		base.CGet<CButtonObsolete>("WorldMapButton").gameObject.SetActive(b);
		if (b)
		{
			base.CGet<Refers>("BuildingAreaInfo").gameObject.SetActive(false);
		}
		base.CGet<Refers>("TogLayout").gameObject.SetActive(b);
	}

	// Token: 0x06003357 RID: 13143 RVA: 0x00197068 File Offset: 0x00195268
	private void OnSetBuildingBtnShow(ArgumentBox argumentBox)
	{
		bool show;
		argumentBox.Get("show", out show);
		this.SetBuildingBtnShow(show);
	}

	// Token: 0x06003358 RID: 13144 RVA: 0x0019708C File Offset: 0x0019528C
	private void SetBuildingAreaInfo(bool isTaiwuVillage, bool isBambooHouse, bool isSectLocation, bool isCityLocation, MapBlockData mapBlockData)
	{
		this._isTaiwuVillage = isTaiwuVillage;
		SettlementInfo settlementInfo = SingletonObject.getInstance<WorldMapModel>().GetLocationOrganizationInfo(mapBlockData.GetLocation());
		sbyte templateId = Organization.Instance[settlementInfo.OrgTemplateId].TemplateId;
		this._buildingAreaInfo.CGet<GameObject>("QuickSelectBtn").SetActive(isTaiwuVillage);
		this._buildingAreaInfo.CGet<CButtonObsolete>("ButtonPlanBuilding").gameObject.SetActive(isTaiwuVillage);
		this._buildingAreaInfo.CGet<CButtonObsolete>("ButtonMultiplyRemoveBuilding").gameObject.SetActive(isTaiwuVillage);
		this._buildingAreaInfo.CGet<GameObject>("Warehouse").SetActive(isTaiwuVillage);
		this.RefreshRoleManageButton();
		TutorialChapterModel tutorialChapterModel = SingletonObject.getInstance<TutorialChapterModel>();
		bool isChapter3 = tutorialChapterModel.InGuiding && tutorialChapterModel.TutorialChapterIndex == 2;
		CButtonObsolete overviewButton = this._buildingAreaInfo.CGet<CButtonObsolete>("ButtonBuildingOverview");
		bool showOverviewButton = !isChapter3 && (isTaiwuVillage || tutorialChapterModel.InGuiding);
		overviewButton.gameObject.SetActive(showOverviewButton);
		overviewButton.interactable = (SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10) || tutorialChapterModel.InGuiding);
		this.SetBuildingSpaceMousetip();
		this._buildingAreaInfo.CGet<GameObject>("BambooHouseBg").SetActive(isBambooHouse);
		this._buildingAreaInfo.CGet<GameObject>("SectSupportInfo").SetActive(isSectLocation);
		this.UpdateAreaSpiritualDebt(isSectLocation, isCityLocation, mapBlockData);
		if (isSectLocation)
		{
			this._currOrganizationId = templateId;
			OrganizationDomainMethod.AsyncCall.GetOrganizationCombatSkillsDisplayData(this, templateId, delegate(int offset, RawDataPool dataPool)
			{
				OrganizationCombatSkillsDisplayData data = new OrganizationCombatSkillsDisplayData();
				Serializer.Deserialize(dataPool, offset, ref data);
				double value = Math.Round((double)((float)data.ApprovingRate / 10f), 1);
				this._buildingAreaInfo.CGet<TextMeshProUGUI>("SectSupportText").SetText(string.Format("{0}%\n{1}", value, LocalStringManager.Get(LanguageKey.LK_Building_SectSupport_Title)), true);
			});
		}
		this.UpdateQuickBtnState();
		this.UpdateQiwenxingtaiBtnState(isSectLocation, templateId);
	}

	// Token: 0x06003359 RID: 13145 RVA: 0x0019721C File Offset: 0x0019541C
	private void UpdateQiwenxingtaiBtnState(bool isSectLocation, sbyte templateId)
	{
		Refers jieqingBuildingRefer = this._buildingAreaInfo.CGet<Refers>("JieQingSpecialBuilding");
		jieqingBuildingRefer.gameObject.SetActive(false);
		bool flag = !isSectLocation;
		if (!flag)
		{
			this._currOrganizationId = templateId;
			bool orgAvailable = UI_Bottom.JieQingData.OrgAvailable((short)templateId);
			bool flag2 = !orgAvailable;
			if (flag2)
			{
				jieqingBuildingRefer.gameObject.SetActive(false);
			}
			else
			{
				OrganizationDomainMethod.AsyncCall.GetSectFunctionStatus(null, 13, SectFunctionStatuses.SectFunctionStatusType.SpecialInteractionUnlocked, delegate(int offset, RawDataPool pool)
				{
					bool unlock = false;
					Serializer.Deserialize(pool, offset, ref unlock);
					jieqingBuildingRefer.gameObject.SetActive(unlock);
					bool flag3 = !unlock;
					if (!flag3)
					{
						CButtonObsolete button = jieqingBuildingRefer.CGet<CButtonObsolete>("JieQingSpecialBuild");
						button.interactable = false;
						this.RefreshQiWenXingtaiTips();
					}
				});
			}
		}
	}

	// Token: 0x0600335A RID: 13146 RVA: 0x001972AC File Offset: 0x001954AC
	public void RefreshQiWenXingtaiTips()
	{
		Refers jieqingBuildingRefer = this._buildingAreaInfo.CGet<Refers>("JieQingSpecialBuilding");
		jieqingBuildingRefer.CGet<TextMeshProUGUI>("BuildingSpaceLimitText").text = string.Format("{0}/1", UI_Bottom.JieQingData.HasQiwenXingtai ? 1 : 0);
		this._jieQingData.ApprovingRateInit = false;
		OrganizationDomainMethod.AsyncCall.GetOrganizationCombatSkillsDisplayData(this, this._currOrganizationId, delegate(int offset, RawDataPool dataPool)
		{
			OrganizationCombatSkillsDisplayData data = new OrganizationCombatSkillsDisplayData();
			Serializer.Deserialize(dataPool, offset, ref data);
			this._jieQingData.ApprovingRate = data.ApprovingRate;
			this._jieQingData.ApprovingRateInit = true;
			this._jieQingData.TryRefresh((short)this._currOrganizationId, jieqingBuildingRefer);
		});
		this._jieQingData.CountInit = false;
		ExtraDomainMethod.AsyncCall.GetSectExtraLegacyBuildingCounts(this, delegate(int offset, RawDataPool dataPool)
		{
			ValueTuple<int, int> counts = new ValueTuple<int, int>(0, 0);
			Serializer.Deserialize(dataPool, offset, ref counts);
			this._jieQingData.CurrCount = counts.Item1;
			this._jieQingData.MaxCount = counts.Item2;
			this._jieQingData.CountInit = true;
			this._jieQingData.TryRefresh((short)this._currOrganizationId, jieqingBuildingRefer);
		});
	}

	// Token: 0x0600335B RID: 13147 RVA: 0x00197354 File Offset: 0x00195554
	private void SetBuildingSpaceMousetip()
	{
		CButtonObsolete overviewButton = this._buildingAreaInfo.CGet<CButtonObsolete>("ButtonBuildingOverview");
		bool flag = !overviewButton.gameObject.activeSelf;
		if (!flag)
		{
			TaiwuDomainMethod.AsyncCall.GetTaiwuVillageSpaceLimitInfo(this, delegate(int offset, RawDataPool dataPool)
			{
				ValueTuple<int, int, int, int> info = default(ValueTuple<int, int, int, int>);
				Serializer.Deserialize(dataPool, offset, ref info);
				TooltipInvoker mouseTip = overviewButton.GetComponent<TooltipInvoker>();
				mouseTip.Type = TipType.GeneralLines;
				GeneralLineData desc = new GeneralLineData
				{
					Type = 3,
					Args = new List<string>
					{
						LocalStringManager.Get(LanguageKey.LK_Building_SpaceTip)
					}
				};
				GeneralLineData title = new GeneralLineData
				{
					Type = 3,
					Args = new List<string>
					{
						LocalStringManager.GetFormat(LanguageKey.LK_Brackets_Symbol, LocalStringManager.Get(LanguageKey.LK_Taiwu_BuildingSpace_Title))
					}
				};
				GeneralLineData contentVillage = new GeneralLineData
				{
					Type = 5,
					Args = new List<string>
					{
						LocalStringManager.GetFormat(LanguageKey.LK_Building_Space_VillageProvide, info.Item1)
					},
					ExtraArgs = new List<object>
					{
						20
					}
				};
				GeneralLineData contentSpaceExtraAdd = new GeneralLineData
				{
					Type = 5,
					Args = new List<string>
					{
						LocalStringManager.GetFormat(LanguageKey.LK_Building_Space_EmeiProvide, info.Item2)
					},
					ExtraArgs = new List<object>
					{
						20
					}
				};
				GeneralLineData contentProsperousConstruction = new GeneralLineData
				{
					Type = 5,
					Args = new List<string>
					{
						LocalStringManager.GetFormat(LanguageKey.LK_Building_Space_ProsperousConstructionProvide, info.Item3)
					},
					ExtraArgs = new List<object>
					{
						20
					}
				};
				GeneralLineData contentResourceBlockEffect = new GeneralLineData
				{
					Type = 5,
					Args = new List<string>
					{
						LocalStringManager.GetFormat(LanguageKey.LK_Building_Space_BuildingProvide, info.Item4)
					},
					ExtraArgs = new List<object>
					{
						20
					}
				};
				int lineCount = 3;
				TooltipInvoker tooltipInvoker = mouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				mouseTip.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_Taiwu_BuildingSpace_Title)).SetObject("LineData1", desc).SetObject("LineData2", title).SetObject("LineData3", contentVillage);
				bool flag2 = info.Item2 > 0;
				if (flag2)
				{
					lineCount++;
					mouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", lineCount), contentSpaceExtraAdd);
				}
				bool flag3 = info.Item3 > 0;
				if (flag3)
				{
					lineCount++;
					mouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", lineCount), contentProsperousConstruction);
				}
				bool flag4 = info.Item4 > 0;
				if (flag4)
				{
					lineCount++;
					mouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", lineCount), contentResourceBlockEffect);
				}
				mouseTip.RuntimeParam.Set("LineCount", lineCount);
			});
		}
	}

	// Token: 0x0600335C RID: 13148 RVA: 0x001973AA File Offset: 0x001955AA
	private void CloseBuildingManage(ArgumentBox argBox)
	{
		this.SetBuildingSpaceMousetip();
	}

	// Token: 0x0600335D RID: 13149 RVA: 0x001973B4 File Offset: 0x001955B4
	private void RefreshRoleManageButton()
	{
		GameObject roleManageButtonNode = this._buildingAreaInfo.CGet<GameObject>("RoleManageButtonNode");
		Refers roleManageButton = this._buildingAreaInfo.CGet<Refers>("RoleManageButton");
		TextMeshProUGUI roleSeatLabel = roleManageButton.CGet<TextMeshProUGUI>("RoleSeatLabel");
		roleManageButtonNode.SetActive(this._isTaiwuVillage);
		roleSeatLabel.text = LocalStringManager.Get(LanguageKey.LK_Building_TaiwuVillageLineage_Name);
		bool hasTaiwuShrine = ViewBuildingArea.HasBuilding(45, true);
		roleManageButton.GetComponent<CButtonObsolete>().interactable = hasTaiwuShrine;
		roleManageButton.CGet<DisableStyleRoot>("DisableStyleRoot").SetStyleEffect(!hasTaiwuShrine, false);
	}

	// Token: 0x0600335E RID: 13150 RVA: 0x0019743C File Offset: 0x0019563C
	private void UpdateAreaSpiritualDebt(bool isSectLocation, bool isCityLocation, MapBlockData mapBlockData)
	{
		int value = SingletonObject.getInstance<WorldMapModel>().GetAreaSpiritualDebt(mapBlockData.AreaId);
		string textValue = WorldMapModel.GetFormatSpiritualDebt(value, 0);
		this._buildingAreaInfo.CGet<GameObject>("SpiritualDebtIncrease").SetActive(value > 0);
		this._buildingAreaInfo.CGet<GameObject>("SpiritualDebtDecrease").SetActive(value < 0);
		this._buildingAreaInfo.CGet<GameObject>("SpiritualDebtZero").SetActive(value == 0);
		this._buildingAreaInfo.CGet<GameObject>("AreaSpiritualDebtInfo").SetActive(isCityLocation || isSectLocation);
		this._buildingAreaInfo.gameObject.SetActive(true);
		this._buildingAreaInfo.CGet<TextMeshProUGUI>("AreaSpiritualDebtText").text = textValue;
		this._buildingAreaInfo.CGet<TextMeshProUGUI>("AreaSpiritualDebtText2").SetText(textValue, true);
	}

	// Token: 0x0600335F RID: 13151 RVA: 0x0019750C File Offset: 0x0019570C
	private void OnSetBuildingAreaInfo(ArgumentBox argumentBox)
	{
		bool isTaiwuVillage;
		argumentBox.Get("isTaiwuVillage", out isTaiwuVillage);
		bool isBambooHouse;
		argumentBox.Get("isBambooHouse", out isBambooHouse);
		bool isSectLocation;
		argumentBox.Get("isSectLocation", out isSectLocation);
		bool isCityLocation;
		argumentBox.Get("isCityLocation", out isCityLocation);
		MapBlockData mapBlockData;
		argumentBox.Get<MapBlockData>("mapBlockData", out mapBlockData);
		this.SetBuildingAreaInfo(isTaiwuVillage, isBambooHouse, isSectLocation, isCityLocation, mapBlockData);
	}

	// Token: 0x06003360 RID: 13152 RVA: 0x00197570 File Offset: 0x00195770
	private void OnSetAreaSpiritualDebt(ArgumentBox argumentBox)
	{
		bool isSectLocation;
		argumentBox.Get("isSectLocation", out isSectLocation);
		bool isCityLocation;
		argumentBox.Get("isCityLocation", out isCityLocation);
		MapBlockData mapBlockData;
		argumentBox.Get<MapBlockData>("mapBlockData", out mapBlockData);
		this.UpdateAreaSpiritualDebt(isSectLocation, isCityLocation, mapBlockData);
	}

	// Token: 0x06003361 RID: 13153 RVA: 0x001975B4 File Offset: 0x001957B4
	private void UpdateQuickBtnState()
	{
		BuildingDomainMethod.AsyncCall.QuickCollectShopItemCount(this, delegate(int offset, RawDataPool dataPool)
		{
			int count = 0;
			Serializer.Deserialize(dataPool, offset, ref count);
			this._buildingAreaInfo.CGet<CButtonObsolete>("ItemBtn").gameObject.SetActive(count > 0);
		});
		BuildingDomainMethod.AsyncCall.QuickRecruitPeopleCount(this, delegate(int offset, RawDataPool dataPool)
		{
			int count = 0;
			Serializer.Deserialize(dataPool, offset, ref count);
			this._buildingAreaInfo.CGet<CButtonObsolete>("PeopleBtn").gameObject.SetActive(count > 0);
		});
		BuildingDomainMethod.AsyncCall.QuickCollectBuildingEarnCount(this, delegate(int offset, RawDataPool dataPool)
		{
			int count = 0;
			Serializer.Deserialize(dataPool, offset, ref count);
			this._buildingAreaInfo.CGet<CButtonObsolete>("QuickSelect").interactable = (count > 0);
			TextMeshProUGUI quickSelectText = this._buildingAreaInfo.CGet<TextMeshProUGUI>("QuickSelectText");
			TextMeshProUGUI quickSelectTextDisable = this._buildingAreaInfo.CGet<TextMeshProUGUI>("QuickSelectTextDisable");
			bool enable = count > 0;
			quickSelectText.gameObject.SetActive(enable);
			quickSelectTextDisable.gameObject.SetActive(!enable);
		});
		GEvent.OnEvent(UiEvents.UpdateAllBlockInfo, null);
	}

	// Token: 0x06003362 RID: 13154 RVA: 0x00197609 File Offset: 0x00195809
	private void OnUpdateQuickBtnState(ArgumentBox argumentBox)
	{
		this.UpdateQuickBtnState();
	}

	// Token: 0x17000595 RID: 1429
	// (get) Token: 0x06003363 RID: 13155 RVA: 0x00197614 File Offset: 0x00195814
	private bool CurBlockIsMarked
	{
		get
		{
			bool curBlockHasWork = this.CurBlockHasWork;
			bool result;
			if (curBlockHasWork)
			{
				result = true;
			}
			else
			{
				bool flag = this._blockData == null;
				if (flag)
				{
					result = false;
				}
				else
				{
					Location location = this._blockData.GetLocation();
					result = SingletonObject.getInstance<BuildingModel>().CheckBlockIsMarked(location);
				}
			}
			return result;
		}
	}

	// Token: 0x17000596 RID: 1430
	// (get) Token: 0x06003364 RID: 13156 RVA: 0x0019765C File Offset: 0x0019585C
	private bool CurBlockHasWork
	{
		get
		{
			bool flag = this._blockData == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Location location = this._blockData.GetLocation();
				result = SingletonObject.getInstance<BuildingModel>().CheckBlockHasWork(location, -1);
			}
			return result;
		}
	}

	// Token: 0x06003365 RID: 13157 RVA: 0x00197698 File Offset: 0x00195898
	private void OnClickMark()
	{
		Location location = this._blockData.GetLocation();
		bool curBlockIsMarked = this.CurBlockIsMarked;
		if (curBlockIsMarked)
		{
			bool flag = !this.CurBlockHasWork;
			if (flag)
			{
				this.RemoveLocationMark(location);
				this.SetMarkButton(false, false);
			}
			else
			{
				DialogCmd cmd = new DialogCmd
				{
					Title = LocalStringManager.Get(LanguageKey.LK_Mark_Cancel_Tip_Title),
					Content = LocalStringManager.Get(LanguageKey.LK_Mark_Cancel_Tip_Desc2),
					Type = 1,
					Yes = delegate()
					{
						this.RemoveLocationMark(location);
						this.SetMarkButton(false, false);
						SingletonObject.getInstance<WorldMapModel>().RequestStopVillagerWorkOnMap(location, true);
					},
					No = null
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
		}
		else
		{
			this.AddLocationMark(location);
			this.SetMarkButton(true, false);
		}
	}

	// Token: 0x06003366 RID: 13158 RVA: 0x00197787 File Offset: 0x00195987
	private void FindTreasureOnce()
	{
		CommandManager.AddCommandShowUI(EPriority.ShowUINormal, UIElement.CollectResource, EasyPool.Get<ArgumentBox>().Set("IsDigSeries", false).Set("CollectType", 4));
	}

	// Token: 0x06003367 RID: 13159 RVA: 0x001977B2 File Offset: 0x001959B2
	private void FindTreasureSeries()
	{
		CommandManager.AddCommandShowUI(EPriority.ShowUINormal, UIElement.CollectResource, EasyPool.Get<ArgumentBox>().Set("IsDigSeries", true).Set("CollectType", 4));
	}

	// Token: 0x06003368 RID: 13160 RVA: 0x001977DD File Offset: 0x001959DD
	private void RefreshMarkButton()
	{
		this.SetMarkButton(this.CurBlockIsMarked, this.CurBlockHasWork);
	}

	// Token: 0x06003369 RID: 13161 RVA: 0x001977F4 File Offset: 0x001959F4
	private void SetMarkButton(bool isMarked, bool hasWork)
	{
		this.ChangeButtonInteractable(base.CGet<Refers>("MapBlockInfo").CGet<CButtonObsolete>("Mark"), true, null);
		GameObject markedGo = base.CGet<Refers>("MapBlockInfo").CGet<GameObject>("Marked");
		markedGo.SetActive(isMarked);
		MonoJoint componentInChildren = markedGo.GetComponentInChildren<MonoJoint>();
		if (componentInChildren != null)
		{
			componentInChildren.JointSync();
		}
	}

	// Token: 0x0600336A RID: 13162 RVA: 0x00197850 File Offset: 0x00195A50
	private void AddLocationMark(Location location)
	{
		ExtraDomainMethod.Call.AddLocationMark(location);
	}

	// Token: 0x0600336B RID: 13163 RVA: 0x0019785A File Offset: 0x00195A5A
	private void RemoveLocationMark(Location location)
	{
		ExtraDomainMethod.Call.RemoveLocationMark(location);
	}

	// Token: 0x0600336C RID: 13164 RVA: 0x00197864 File Offset: 0x00195A64
	private void OnLocationMarkChange(ArgumentBox argBox)
	{
		this.RefreshMarkButton();
	}

	// Token: 0x0600336D RID: 13165 RVA: 0x00197870 File Offset: 0x00195A70
	private void InitToggle(CToggleObsolete toggle, Func<bool> getSetting, Action<bool> setSetting, Action onToggleChange)
	{
		toggle.isOn = getSetting();
		toggle.onValueChanged.RemoveAllListeners();
		toggle.onValueChanged.AddListener(delegate(bool isOn)
		{
			setSetting(isOn);
			SingletonObject.getInstance<GlobalSettings>().SaveSettings();
			onToggleChange();
		});
	}

	// Token: 0x0600336E RID: 13166 RVA: 0x001978C4 File Offset: 0x00195AC4
	private void InitBlockInfoTog()
	{
		Refers togLayout = base.CGet<Refers>("TogLayout");
		this.InitToggle(togLayout.CGet<CToggleObsolete>("TogShowEnemy"), () => SingletonObject.getInstance<GlobalSettings>().ShowMapBlockEnemyCount, delegate(bool isOn)
		{
			SingletonObject.getInstance<GlobalSettings>().ShowMapBlockEnemyCount = isOn;
		}, delegate
		{
			GEvent.OnEvent(UiEvents.OnMapBlockEnemyTogChange, null);
		});
		this.InitToggle(togLayout.CGet<CToggleObsolete>("TogShowFriend"), () => SingletonObject.getInstance<GlobalSettings>().ShowMapBlockFriendCount, delegate(bool isOn)
		{
			SingletonObject.getInstance<GlobalSettings>().ShowMapBlockFriendCount = isOn;
		}, delegate
		{
			GEvent.OnEvent(UiEvents.OnMapBlockFriendTogChange, null);
		});
		this.InitToggle(togLayout.CGet<CToggleObsolete>("TogShowMerchant"), () => SingletonObject.getInstance<GlobalSettings>().ShowMapBlockMerchantIcon, delegate(bool isOn)
		{
			SingletonObject.getInstance<GlobalSettings>().ShowMapBlockMerchantIcon = isOn;
		}, delegate
		{
			GEvent.OnEvent(UiEvents.OnMapBlockMerchantTogChange, null);
		});
		this.InitToggle(togLayout.CGet<CToggleObsolete>("TogShowSettlementEdge"), () => SingletonObject.getInstance<GlobalSettings>().ShowMapBlockSettlementEdge, delegate(bool isOn)
		{
			SingletonObject.getInstance<GlobalSettings>().ShowMapBlockSettlementEdge = isOn;
		}, delegate
		{
			GEvent.OnEvent(UiEvents.OnMapBlockSettlementEdgeTogChange, null);
		});
		bool crossArchive = this._crossArchive;
		if (crossArchive)
		{
			togLayout.CGet<GameObject>("DummyParent").SetActive(false);
			togLayout.CGet<CToggleObsolete>("TogShowPastLifeRelation").gameObject.SetActive(true);
			this.InitToggle(togLayout.CGet<CToggleObsolete>("TogShowPastLifeRelation"), () => SingletonObject.getInstance<GlobalSettings>().ShowMapBlockPastLifeRelationIcon, delegate(bool isOn)
			{
				SingletonObject.getInstance<GlobalSettings>().ShowMapBlockPastLifeRelationIcon = isOn;
			}, delegate
			{
				GEvent.OnEvent(UiEvents.OnMapBlockPastLifeRelationTogChange, null);
			});
		}
		else
		{
			togLayout.CGet<GameObject>("DummyParent").SetActive(true);
			togLayout.CGet<CToggleObsolete>("TogShowPastLifeRelation").gameObject.SetActive(false);
		}
	}

	// Token: 0x0600336F RID: 13167 RVA: 0x00197B6E File Offset: 0x00195D6E
	private void OnMapBlockEnemyTogChange(ArgumentBox argumentBox)
	{
		this.InitBlockInfoTog();
	}

	// Token: 0x06003370 RID: 13168 RVA: 0x00197B78 File Offset: 0x00195D78
	private void OnMapBlockFriendTogChange(ArgumentBox argumentBox)
	{
		this.InitBlockInfoTog();
	}

	// Token: 0x06003371 RID: 13169 RVA: 0x00197B82 File Offset: 0x00195D82
	private void OnMapBlockMerchantTogChange(ArgumentBox argumentBox)
	{
		this.InitBlockInfoTog();
	}

	// Token: 0x06003372 RID: 13170 RVA: 0x00197B8C File Offset: 0x00195D8C
	private void OnMapBlockSettlementEdgeTogChange(ArgumentBox argumentBox)
	{
		this.InitBlockInfoTog();
	}

	// Token: 0x06003373 RID: 13171 RVA: 0x00197B96 File Offset: 0x00195D96
	private void OnMapBlockPastLifeRelationTogChange(ArgumentBox argumentBox)
	{
		this.InitBlockInfoTog();
	}

	// Token: 0x06003374 RID: 13172 RVA: 0x00197BA0 File Offset: 0x00195DA0
	private void OnBottomShowNewNotification(ArgumentBox argumentBox)
	{
		base.CGet<GameObject>("NewLogNotice").SetActive(true);
	}

	// Token: 0x06003375 RID: 13173 RVA: 0x00197BB8 File Offset: 0x00195DB8
	private void InitCharPanel()
	{
		TriggerPanel lifeMenuBg = this._groupChar.CGet<TriggerPanel>("LifeMenuBg");
		lifeMenuBg.Init(new Func<bool>(this.CheckCanEnterLifePanel), new Action(this.RefreshProfession));
	}

	// Token: 0x06003376 RID: 13174 RVA: 0x00197BF8 File Offset: 0x00195DF8
	private void HideCharPanel()
	{
		TriggerPanel lifeMenuBg = this._groupChar.CGet<TriggerPanel>("LifeMenuBg");
		lifeMenuBg.gameObject.SetActive(false);
		RectTransform combatCharHolder = this._groupChar.CGet<RectTransform>("CombatCharHolder");
		for (int i = 0; i < combatCharHolder.childCount; i++)
		{
			TriggerPanel triggerPanel = combatCharHolder.GetChild(i).GetComponent<Refers>().CGet<TriggerPanel>("MenuBg");
			triggerPanel.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003377 RID: 13175 RVA: 0x00197C74 File Offset: 0x00195E74
	private void RefreshProfessionButton(ArgumentBox box)
	{
		bool enable = SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(27);
		bool enable2 = this._forcedCanOpenCharacterMenu;
		CButtonObsolete professionButton = this._groupChar.CGet<CButtonObsolete>("BtnLife");
		Refers buttonRefers = professionButton.GetComponent<Refers>();
		bool flag = !enable;
		if (flag)
		{
			professionButton.interactable = false;
			professionButton.ClickAudioKey = null;
			professionButton.GetComponent<PointerTrigger>().enabled = false;
		}
		else
		{
			buttonRefers.CGet<GameObject>("Locked").SetActive(!enable2);
			professionButton.interactable = enable2;
			professionButton.ClickAudioKey = (enable2 ? "SFX_ProfessionSkill_enter" : null);
			professionButton.GetComponent<PointerTrigger>().enabled = enable2;
		}
	}

	// Token: 0x06003378 RID: 13176 RVA: 0x00197D14 File Offset: 0x00195F14
	private void RefreshProfession()
	{
		TriggerPanel lifeMenuBg = this._groupChar.CGet<TriggerPanel>("LifeMenuBg");
		this._professionBottomMenu.Refresh();
		AudioManager.Instance.PlaySound("SFX_ProfessionSkill_hover", false, false);
	}

	// Token: 0x06003379 RID: 13177 RVA: 0x00197D51 File Offset: 0x00195F51
	public void ProfessionTravelerSkillTwoSwitchState(bool start)
	{
		base.CGet<GameObject>("AnimationRoot").SetActive(!start);
		base.CGet<GameObject>("ProfessionSkillTwoTips").SetActive(start);
	}

	// Token: 0x0600337A RID: 13178 RVA: 0x00197D7C File Offset: 0x00195F7C
	private void PlayAnimToHideMainUI(ArgumentBox argumentBox)
	{
		bool keepTimePanel = false;
		if (argumentBox != null)
		{
			argumentBox.Get("KeepTimePanel", out keepTimePanel);
		}
		bool flag = keepTimePanel;
		if (flag)
		{
			base.CGet<RectTransform>("Layout").DOLocalMoveY(-1500f, 0.3f, false);
		}
		else
		{
			this._uiAnim.PlayHideAnimation(null, true);
		}
	}

	// Token: 0x0600337B RID: 13179 RVA: 0x00197DD4 File Offset: 0x00195FD4
	private void PlayAnimToShowMainUI(ArgumentBox argumentBox)
	{
		bool keepTimePanel = false;
		if (argumentBox != null)
		{
			argumentBox.Get("KeepTimePanel", out keepTimePanel);
		}
		bool flag = keepTimePanel;
		if (flag)
		{
			base.CGet<RectTransform>("Layout").DOLocalMoveY(0f, 0.3f, false);
		}
		else
		{
			this._uiAnim.PlayShowAnimation(null, true);
		}
	}

	// Token: 0x0600337C RID: 13180 RVA: 0x00197E2C File Offset: 0x0019602C
	private void RefreshReadingProgress()
	{
		this._readAndLoop.OnReadingProgressUpdate(this._activeReadingProgress);
	}

	// Token: 0x0600337D RID: 13181 RVA: 0x00197E41 File Offset: 0x00196041
	private void RefreshLoopingProgress()
	{
		this._readAndLoop.OnLoopingProgressUpdate(this._activeLoopingProgress);
	}

	// Token: 0x0600337E RID: 13182 RVA: 0x00197E56 File Offset: 0x00196056
	private void UpdateActiveReadAndLoopButton(sbyte _type = -1)
	{
		this._readAndLoop.RefreshActiveReadButton();
		this._readAndLoop.RefreshActiveLoopButton();
	}

	// Token: 0x0600337F RID: 13183 RVA: 0x00197E71 File Offset: 0x00196071
	private void OnTaskBubbleStart(ArgumentBox box)
	{
		this._isTaskBubbleDisplaying = true;
	}

	// Token: 0x06003380 RID: 13184 RVA: 0x00197E7B File Offset: 0x0019607B
	private void OnTaskBubbleEnded(ArgumentBox box)
	{
		this._isTaskBubbleDisplaying = false;
	}

	// Token: 0x06003381 RID: 13185 RVA: 0x00197E85 File Offset: 0x00196085
	private void OnEventWindowStart(ArgumentBox box)
	{
		this._isEventWindowDisplaying = true;
	}

	// Token: 0x06003382 RID: 13186 RVA: 0x00197E8F File Offset: 0x0019608F
	private void OnEventWindowEnded(ArgumentBox box)
	{
		this._isEventWindowDisplaying = false;
	}

	// Token: 0x06003383 RID: 13187 RVA: 0x00197E99 File Offset: 0x00196099
	private void OnSetVillagerRole(ArgumentBox box)
	{
		this.RefreshRoleManageButton();
	}

	// Token: 0x06003384 RID: 13188 RVA: 0x00197EA4 File Offset: 0x001960A4
	private void SendTeammateBubbleRequest()
	{
		bool isAbleToDisplayBubble = this._isAbleToDisplayBubble;
		if (isAbleToDisplayBubble)
		{
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.2f, delegate
			{
				MapDomainMethod.Call.GetTeammateBubbleCollection(this.Element.GameDataListenerId, WorldMapModel.Traveling);
			});
		}
	}

	// Token: 0x06003385 RID: 13189 RVA: 0x00197ED8 File Offset: 0x001960D8
	private void OnTeammateBubbleCollectionUpdated(TeammateBubbleCollection collection)
	{
		bool flag = collection == null || this._isTaskBubbleDisplaying || this._isEventWindowDisplaying;
		if (!flag)
		{
			this._teammateBubbleArgumentCollection.Clear();
			this._teammateBubbleCollection = collection;
			this._renderInfo = this._teammateBubbleCollection.GetRenderInfo(0, this._teammateBubbleArgumentCollection);
			string key = "UI_Bottom";
			RecordArgumentsRequest request = new RecordArgumentsRequest(this._teammateBubbleArgumentCollection);
			LifeRecordDomainMethod.Call.GetRecordRenderInfoArguments(this.Element.GameDataListenerId, key, request);
		}
	}

	// Token: 0x06003386 RID: 13190 RVA: 0x00197F54 File Offset: 0x00196154
	private void HandleTeammateBubbleCollectionResult(ArgumentCollectionRenderArguments dynamicArguments)
	{
		this._isAbleToDisplayBubble = false;
		RenderedArgumentCollection renderedArgumentCollection = new RenderedArgumentCollection();
		int index = this._renderInfo.Index;
		string desc = this._renderInfo.Text;
		Bubble bubbleObject = this._groupChar.CGet<RectTransform>("CombatCharHolder").Find(index.ToString()).GetComponent<Refers>().CGet<Bubble>("TeammateBubble");
		this._bubbleConfig = TeammateBubble.Instance[this._renderInfo.RecordType];
		GameMessageUtils.RenderDynamicArguments(dynamicArguments, this._teammateBubbleArgumentCollection, renderedArgumentCollection, false, false);
		GameMessageUtils.RenderFixedArguments(this._teammateBubbleArgumentCollection, renderedArgumentCollection, false);
		string text = GameMessageUtils.ParseRenderInfoToText(desc, this._renderInfo, renderedArgumentCollection);
		bubbleObject.SetText(text, true);
		bubbleObject.gameObject.SetActive(true);
		SingletonObject.getInstance<YieldHelper>().DelaySecondsDo((float)this._bubbleConfig.Duration / 60f, new Action(this.DisableTeammateBubble));
		SingletonObject.getInstance<YieldHelper>().DelaySecondsDo((float)this._bubbleConfig.Duration / 60f + 3f, new Action(this.EnableTeammateBubble));
	}

	// Token: 0x06003387 RID: 13191 RVA: 0x0019806C File Offset: 0x0019626C
	private void DisableTeammateBubble()
	{
		RectTransform holder;
		bool flag = this._groupChar != null && this._groupChar.CTryGet<RectTransform>("CombatCharHolder", out holder);
		if (flag)
		{
			for (int i = 0; i < holder.childCount; i++)
			{
				holder.GetChild(i).GetComponent<Refers>().CGet<Bubble>("TeammateBubble").gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06003388 RID: 13192 RVA: 0x001980D8 File Offset: 0x001962D8
	private void EnableTeammateBubble()
	{
		this._isAbleToDisplayBubble = true;
	}

	// Token: 0x06003389 RID: 13193 RVA: 0x001980E4 File Offset: 0x001962E4
	private void OnJieqingMaskCharacterListChanged(ArgumentBox argBox)
	{
		bool flag = argBox == null;
		if (flag)
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this, new List<int>
			{
				this.TaiwuCharId
			}, delegate(int offset, RawDataPool pool)
			{
				List<CharacterDisplayData> displayData = null;
				Serializer.Deserialize(pool, offset, ref displayData);
				this._groupChar.CGet<Game.Components.Avatar.Avatar>("Avatar").Refresh(displayData[0], true);
			});
		}
	}

	// Token: 0x0600338A RID: 13194 RVA: 0x00198120 File Offset: 0x00196320
	private void UpdateMainUiCustomButtons()
	{
		global::MainUiCustomButton customButtons = base.CGet<global::MainUiCustomButton>("CustomButtons");
		customButtons.RefreshButtons(this._mainUiCustomButtonList);
	}

	// Token: 0x0600338B RID: 13195 RVA: 0x00198148 File Offset: 0x00196348
	private void InitCustomButtons()
	{
		global::MainUiCustomButton customButtons = base.CGet<global::MainUiCustomButton>("CustomButtons");
		customButtons.Init(delegate
		{
			this.OpenTeammateCharacterMenu(this.TaiwuCharId, ECharacterSubToggleBase.None, ECharacterSubPage.None);
		}, delegate
		{
			this.OpenTeammateCharacterMenu(this.TaiwuCharId, ECharacterSubToggleBase.EquipmentBase, ECharacterSubPage.None);
		}, delegate
		{
			this.OpenTeammateCharacterMenu(this.TaiwuCharId, ECharacterSubToggleBase.ItemBase, ECharacterSubPage.None);
		}, delegate
		{
			this.OpenTeammateCharacterMenu(this.TaiwuCharId, ECharacterSubToggleBase.EquipCombatSkillBase, ECharacterSubPage.None);
		}, delegate
		{
			this.OpenTeammateCharacterMenu(this.TaiwuCharId, ECharacterSubToggleBase.PracticeBase, ECharacterSubPage.None);
		}, new Action(this.OpenHeal), () => this._forcedCanOpenCharacterMenu);
	}

	// Token: 0x0600338C RID: 13196 RVA: 0x001981C0 File Offset: 0x001963C0
	private void HandlerDataCharacterEquipments(ItemKey[] equipments)
	{
		ItemKey carrierKey = equipments[11];
		bool flag = carrierKey.TemplateId >= 0;
		if (flag)
		{
			ItemDomainMethod.AsyncCall.GetItemDisplayData(null, carrierKey, delegate(int offset, RawDataPool pool)
			{
				ItemDisplayData displayData = null;
				Serializer.Deserialize(pool, offset, ref displayData);
				bool flag2 = displayData.Durability > 0;
				if (flag2)
				{
					this.UpdateTaiwuCarrier(carrierKey.TemplateId);
				}
				else
				{
					this.UpdateTaiwuCarrier(-1);
				}
			});
		}
		else
		{
			this.UpdateTaiwuCarrier(-1);
		}
	}

	// Token: 0x0600338D RID: 13197 RVA: 0x00198223 File Offset: 0x00196423
	private void UpdateTaiwuCarrier(short carrierTemplateId)
	{
		this.MapPickupWindow.RefreshCarrierInfo(carrierTemplateId);
	}

	// Token: 0x0600338E RID: 13198 RVA: 0x00198234 File Offset: 0x00196434
	private void OnPickUpDisplayInfoChange(ArgumentBox argBox)
	{
		MapBlockData blockData;
		argBox.Get<MapBlockData>("MapBlockData", out blockData);
		bool refreshOnly;
		argBox.Get("RefreshOnly", out refreshOnly);
		bool flag = refreshOnly;
		if (flag)
		{
			this.MapPickupWindow.RefreshPickupItemInfos();
		}
		else
		{
			this.MapPickupWindow.RefreshPickupItemInfos(blockData);
		}
	}

	// Token: 0x0600338F RID: 13199 RVA: 0x0019827E File Offset: 0x0019647E
	private void OnCurrentPickUpDisplayInfoChange(ArgumentBox argBox)
	{
		this.MapPickupWindow.RefreshPickupItemInfos();
	}

	// Token: 0x06003398 RID: 13208 RVA: 0x0019860D File Offset: 0x0019680D
	[CompilerGenerated]
	private void <RefreshWorkPanel>g__OnExchangeCharacterIdChanged|157_0(int characterId)
	{
		this._exchangingWorkVillagerId = characterId;
		this.OpenSelectWindow();
	}

	// Token: 0x0600339C RID: 13212 RVA: 0x00198781 File Offset: 0x00196981
	[CompilerGenerated]
	internal static void <AdvanceMonth>g__Action|167_1()
	{
		WorldDomainMethod.Call.AdvanceMonth();
		GameApp.AdvancingMonth = true;
	}

	// Token: 0x0400253A RID: 9530
	public MapPickupsWindow MapPickupWindow;

	// Token: 0x0400253B RID: 9531
	private const string CanPassRoutePrefabKey = "MiniMap_CanPassRoutePrefab";

	// Token: 0x0400253C RID: 9532
	private const string CanUnlockRoutePrefabKey = "MiniMap_CanUnlockRoutePrefab";

	// Token: 0x0400253D RID: 9533
	private Refers _groupChar;

	// Token: 0x0400253E RID: 9534
	private ProfessionBottomMenu _professionBottomMenu;

	// Token: 0x0400253F RID: 9535
	private ReadAndLoop _readAndLoop;

	// Token: 0x04002540 RID: 9536
	private Refers _timeDisk;

	// Token: 0x04002541 RID: 9537
	private Refers _mapBlockInfo;

	// Token: 0x04002542 RID: 9538
	private Refers _buildingAreaInfo;

	// Token: 0x04002543 RID: 9539
	private bool _workPanelIsShow;

	// Token: 0x04002544 RID: 9540
	private bool _collectPanelIsShow;

	// Token: 0x04002545 RID: 9541
	private bool _treasurePanelIsShow;

	// Token: 0x04002546 RID: 9542
	private int _buildingSpaceCurr;

	// Token: 0x04002547 RID: 9543
	private int _buildingSpaceLimit;

	// Token: 0x04002548 RID: 9544
	private readonly List<int> _teammateIdList = new List<int>();

	// Token: 0x04002549 RID: 9545
	private bool _needUpdateGroupAvatar;

	// Token: 0x0400254A RID: 9546
	private readonly Dictionary<int, CharacterDisplayData> _charDisplayDataDict = new Dictionary<int, CharacterDisplayData>();

	// Token: 0x0400254B RID: 9547
	private sbyte _currOrganizationId = -1;

	// Token: 0x0400254C RID: 9548
	public static bool _buildingButtonBaned;

	// Token: 0x0400254D RID: 9549
	private List<int> _readingEventBookList = new List<int>();

	// Token: 0x0400254E RID: 9550
	private List<int> _unlockedWorkingList = new List<int>();

	// Token: 0x0400254F RID: 9551
	private bool _collectResourceIsMax;

	// Token: 0x04002550 RID: 9552
	private bool _isAdventureRemakeRunning;

	// Token: 0x04002551 RID: 9553
	private HashSet<CButtonObsolete> _mapBlockInfoAdventureButtons;

	// Token: 0x04002552 RID: 9554
	private readonly Dictionary<CButtonObsolete, bool> _mapBlockInfoAdventureButtonActiveStates = new Dictionary<CButtonObsolete, bool>();

	// Token: 0x04002553 RID: 9555
	private short _activeLoopingProgress = -1;

	// Token: 0x04002554 RID: 9556
	private short _activeReadingProgress = -1;

	// Token: 0x04002555 RID: 9557
	private List<short> _loopingEventSkillIdList;

	// Token: 0x04002556 RID: 9558
	private List<sbyte> _mainUiCustomButtonList;

	// Token: 0x04002557 RID: 9559
	private CombatSkillKey _lastCheckLoopingCombatSkill;

	// Token: 0x04002558 RID: 9560
	private readonly Dictionary<short, TaiwuLifeSkill> _lifeSkillData = new Dictionary<short, TaiwuLifeSkill>();

	// Token: 0x04002559 RID: 9561
	private readonly Dictionary<short, TaiwuLifeSkill> _notLearnedLifeSkillData = new Dictionary<short, TaiwuLifeSkill>();

	// Token: 0x0400255A RID: 9562
	private readonly Dictionary<short, TaiwuCombatSkill> _combatSkillData = new Dictionary<short, TaiwuCombatSkill>();

	// Token: 0x0400255B RID: 9563
	private readonly Dictionary<short, TaiwuCombatSkill> _notLearnedCombatSkillData = new Dictionary<short, TaiwuCombatSkill>();

	// Token: 0x0400255C RID: 9564
	private ItemKey[] _referenceBooks;

	// Token: 0x0400255D RID: 9565
	private ItemKey _currentReadingBookKey = ItemKey.Invalid;

	// Token: 0x0400255E RID: 9566
	private List<short> _learnedNeigongList = new List<short>();

	// Token: 0x0400255F RID: 9567
	private readonly Dictionary<short, CombatSkillDisplayData> _combatSkillDisplayDataDict = new Dictionary<short, CombatSkillDisplayData>();

	// Token: 0x04002560 RID: 9568
	private short _loopingNeigong = -1;

	// Token: 0x04002561 RID: 9569
	private SkillBookPageDisplayData _pagesInfo = null;

	// Token: 0x04002562 RID: 9570
	private short _obtainedNeili;

	// Token: 0x04002563 RID: 9571
	private int _inventoryCurrLoad;

	// Token: 0x04002564 RID: 9572
	private int _inventoryMaxLoad;

	// Token: 0x04002565 RID: 9573
	private int _warehouseCurrLoad;

	// Token: 0x04002566 RID: 9574
	private int _warehouseMaxLoad;

	// Token: 0x04002567 RID: 9575
	private ResourceInts _resources;

	// Token: 0x04002568 RID: 9576
	private int _exp;

	// Token: 0x04002569 RID: 9577
	private int _materialResourceMaxCount;

	// Token: 0x0400256A RID: 9578
	private List<short> _visitedSettlements;

	// Token: 0x0400256B RID: 9579
	private bool _advanceActionPointConfirmed;

	// Token: 0x0400256C RID: 9580
	private bool _advanceLoopingNeigongConfirmed;

	// Token: 0x0400256D RID: 9581
	private bool _advanceInventoryOverflowConfirmed;

	// Token: 0x0400256E RID: 9582
	private readonly DialogCmd _advanceDialogCmd = new DialogCmd();

	// Token: 0x0400256F RID: 9583
	private readonly HashSet<Action> _extraPostProcess = new HashSet<Action>();

	// Token: 0x04002570 RID: 9584
	private TaiwuCharacterModel _taiwuCharacterModel;

	// Token: 0x04002571 RID: 9585
	private WorldMapModel _mapModel;

	// Token: 0x04002572 RID: 9586
	private MapBlockData _blockData;

	// Token: 0x04002573 RID: 9587
	private short _settlementId;

	// Token: 0x04002574 RID: 9588
	private sbyte _selectedResourceType;

	// Token: 0x04002575 RID: 9589
	private int[] _gatherChange = new int[8];

	// Token: 0x04002576 RID: 9590
	private int[] _maintainChange = new int[8];

	// Token: 0x04002577 RID: 9591
	private readonly List<int> _villagerIdList = new List<int>();

	// Token: 0x04002578 RID: 9592
	private bool _waitingTimeCollect;

	// Token: 0x04002579 RID: 9593
	private int _exchangingWorkVillagerId;

	// Token: 0x0400257A RID: 9594
	public static bool _isHaveChickenKing = false;

	// Token: 0x0400257B RID: 9595
	private CButtonObsolete _btnAutoDispatch;

	// Token: 0x0400257C RID: 9596
	private bool _isAutoDispatch;

	// Token: 0x0400257D RID: 9597
	private bool _isMouseInPanelRange;

	// Token: 0x0400257E RID: 9598
	private Dictionary<int, Injuries> _injuriesDict;

	// Token: 0x0400257F RID: 9599
	private Dictionary<int, PoisonInts> _poisonIntsDict;

	// Token: 0x04002580 RID: 9600
	private List<int> _monitoredInjuryCharIds;

	// Token: 0x04002581 RID: 9601
	private bool _updateReadingDirty;

	// Token: 0x04002582 RID: 9602
	private UI_Bottom.JieQingData _jieQingData = new UI_Bottom.JieQingData();

	// Token: 0x04002583 RID: 9603
	private UIAnim _uiAnim;

	// Token: 0x04002584 RID: 9604
	private bool _crossArchive = SingletonObject.getInstance<BasicGameData>().IsDreamBack;

	// Token: 0x04002585 RID: 9605
	private TeammateBubbleCollection _teammateBubbleCollection;

	// Token: 0x04002586 RID: 9606
	private ArgumentCollection _teammateBubbleArgumentCollection = new ArgumentCollection();

	// Token: 0x04002587 RID: 9607
	private TeammateBubbleRenderInfo _renderInfo;

	// Token: 0x04002588 RID: 9608
	private bool _isAbleToDisplayBubble = true;

	// Token: 0x04002589 RID: 9609
	private bool _isTaskBubbleDisplaying = false;

	// Token: 0x0400258A RID: 9610
	private bool _isEventWindowDisplaying = false;

	// Token: 0x0400258B RID: 9611
	private TeammateBubbleItem _bubbleConfig;

	// Token: 0x0400258C RID: 9612
	private bool _isTaiwuVillage;

	// Token: 0x0400258D RID: 9613
	private static readonly uint[] ListenTaiwuFieldIds = new uint[]
	{
		66U,
		34U,
		56U,
		39U,
		75U,
		59U,
		46U,
		104U,
		103U,
		26U,
		44U,
		111U,
		43U,
		72U
	};

	// Token: 0x0400258E RID: 9614
	private MainAttributes _taiwuMainAttributes = default(MainAttributes);

	// Token: 0x0400258F RID: 9615
	private bool _isTaiwuMovingOnMap = false;

	// Token: 0x04002590 RID: 9616
	private bool _forcedCanOperateInCharacterMenu;

	// Token: 0x04002591 RID: 9617
	private bool _forcedCanOpenCharacterMenu;

	// Token: 0x04002592 RID: 9618
	private bool _forcedCanExchangeChar;

	// Token: 0x02001736 RID: 5942
	private enum EPanelType
	{
		// Token: 0x0400AABE RID: 43710
		None,
		// Token: 0x0400AABF RID: 43711
		Work,
		// Token: 0x0400AAC0 RID: 43712
		Collect,
		// Token: 0x0400AAC1 RID: 43713
		Treasure
	}

	// Token: 0x02001737 RID: 5943
	private class JieQingData
	{
		// Token: 0x170016A1 RID: 5793
		// (get) Token: 0x0600D35D RID: 54109 RVA: 0x005B1A82 File Offset: 0x005AFC82
		public static BuildingBlockItem Config
		{
			get
			{
				return BuildingBlock.Instance[275];
			}
		}

		// Token: 0x170016A2 RID: 5794
		// (get) Token: 0x0600D35E RID: 54110 RVA: 0x005B1A93 File Offset: 0x005AFC93
		public static bool HasQiwenXingtai
		{
			get
			{
				return ViewBuildingArea.HasBuilding(275, false);
			}
		}

		// Token: 0x170016A3 RID: 5795
		// (get) Token: 0x0600D35F RID: 54111 RVA: 0x005B1AA0 File Offset: 0x005AFCA0
		public static bool ResourceEngough
		{
			get
			{
				return CommonUtils.IsBuildingCostResourcesEnough(UI_Bottom.JieQingData.Config);
			}
		}

		// Token: 0x0600D360 RID: 54112 RVA: 0x005B1AAC File Offset: 0x005AFCAC
		public static bool OrgAvailable(short templateId)
		{
			return UI_Bottom.JieQingData.Config.AvailableOrganization.Contains(templateId);
		}

		// Token: 0x0600D361 RID: 54113 RVA: 0x005B1AC0 File Offset: 0x005AFCC0
		public void TryRefresh(short templateId, Refers jieqingBuildingRefer)
		{
			bool flag = !this.ApprovingRateInit || !this.CountInit;
			if (!flag)
			{
				bool approvingRateReach = UI_Bottom.JieQingData.Config.ApprovingRate <= this.ApprovingRate;
				List<ESpecialBuildErrorType> errorTypes = new List<ESpecialBuildErrorType>();
				bool flag2 = !approvingRateReach;
				if (flag2)
				{
					errorTypes.Add(ESpecialBuildErrorType.Approve);
				}
				bool flag3 = !UI_Bottom.JieQingData.ResourceEngough;
				if (flag3)
				{
					errorTypes.Add(ESpecialBuildErrorType.Resrouce);
				}
				bool hasQiwenXingtai = UI_Bottom.JieQingData.HasQiwenXingtai;
				if (hasQiwenXingtai)
				{
					errorTypes.Add(ESpecialBuildErrorType.AlreadyBuilt);
				}
				bool flag4 = !UI_Bottom.JieQingData.OrgAvailable(templateId);
				if (flag4)
				{
					errorTypes.Add(ESpecialBuildErrorType.NotAvailable);
				}
				bool flag5 = this.CurrCount >= this.MaxCount;
				if (flag5)
				{
					errorTypes.Add(ESpecialBuildErrorType.ReachMaxCount);
				}
				bool canInteract = errorTypes.Count == 0;
				DisableStyleRoot disableStyleRoot = jieqingBuildingRefer.CGet<DisableStyleRoot>("DisableStyleRoot");
				disableStyleRoot.SetStyleEffect(!canInteract, false);
				jieqingBuildingRefer.CGet<CButtonObsolete>("JieQingSpecialBuild").interactable = canInteract;
				TooltipInvoker tips = jieqingBuildingRefer.CGet<TooltipInvoker>("MouseTipDisplayer");
				tips.Type = TipType.SpecialBuild;
				TooltipInvoker tooltipInvoker = tips;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				tips.RuntimeParam.SetObject("BuildingBlockItem", UI_Bottom.JieQingData.Config);
				tips.RuntimeParam.SetObject("ErrorTypes", errorTypes);
				tips.RuntimeParam.Set("ApproveRate", this.ApprovingRate);
				tips.RuntimeParam.Set("ApproveNeeded", UI_Bottom.JieQingData.Config.ApprovingRate);
				tips.RuntimeParam.Set("CurCount", this.CurrCount);
				tips.RuntimeParam.Set("MaxCount", this.MaxCount);
			}
		}

		// Token: 0x0400AAC2 RID: 43714
		public short ApprovingRate = 0;

		// Token: 0x0400AAC3 RID: 43715
		public bool ApprovingRateInit = false;

		// Token: 0x0400AAC4 RID: 43716
		public int CurrCount = 0;

		// Token: 0x0400AAC5 RID: 43717
		public int MaxCount = 0;

		// Token: 0x0400AAC6 RID: 43718
		public bool CountInit = false;
	}

	// Token: 0x02001738 RID: 5944
	private enum ERefreshWorkButtonMoving
	{
		// Token: 0x0400AAC8 RID: 43720
		Moving,
		// Token: 0x0400AAC9 RID: 43721
		StopMoving,
		// Token: 0x0400AACA RID: 43722
		Unknown
	}
}
