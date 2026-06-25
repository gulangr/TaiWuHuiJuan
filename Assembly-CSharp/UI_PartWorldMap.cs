using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using DG.Tweening;
using FrameWork;
using FrameWork.CommandSystem;
using FrameWork.UISystem.Components;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000394 RID: 916
public class UI_PartWorldMap : UIBase
{
	// Token: 0x060036A8 RID: 13992 RVA: 0x001B7E73 File Offset: 0x001B6073
	public static TravelSkeletonItem GetSkeleton(short carrierId)
	{
		return (carrierId < 0) ? Config.TravelSkeleton.Instance[55] : Config.TravelSkeleton.Instance[Carrier.Instance[carrierId].TravelSkeleton];
	}

	// Token: 0x170005B6 RID: 1462
	// (get) Token: 0x060036A9 RID: 13993 RVA: 0x001B7EA1 File Offset: 0x001B60A1
	private float StepAnimationDurationWithoutLimit
	{
		get
		{
			return 0.15f * (float)this._nextAreaCostDays;
		}
	}

	// Token: 0x170005B7 RID: 1463
	// (get) Token: 0x060036AA RID: 13994 RVA: 0x001B7EB0 File Offset: 0x001B60B0
	private float StepAnimationDuration
	{
		get
		{
			return Math.Max(this.StepAnimationDurationWithoutLimit, 1.5f);
		}
	}

	// Token: 0x170005B8 RID: 1464
	// (get) Token: 0x060036AB RID: 13995 RVA: 0x001B7EC2 File Offset: 0x001B60C2
	private int TaiwuCharId
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		}
	}

	// Token: 0x170005B9 RID: 1465
	// (get) Token: 0x060036AC RID: 13996 RVA: 0x001B7ECE File Offset: 0x001B60CE
	private bool IsWorldTravelUnlocked
	{
		get
		{
			return SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(4);
		}
	}

	// Token: 0x170005BA RID: 1466
	// (get) Token: 0x060036AD RID: 13997 RVA: 0x001B7EDC File Offset: 0x001B60DC
	private bool CanTravelInteractable
	{
		get
		{
			CrossAreaMoveInfo moveInfo = this._moveInfo;
			return moveInfo != null && moveInfo.Traveling && this._moveInfo.CurrentAreaId != this._moveInfo.ToAreaId;
		}
	}

	// Token: 0x170005BB RID: 1467
	// (get) Token: 0x060036AE RID: 13998 RVA: 0x001B7F19 File Offset: 0x001B6119
	private Graphic TravelPathGraphic
	{
		get
		{
			return this._partWorldView.PathHolder.GetComponent<CRawImage>();
		}
	}

	// Token: 0x170005BC RID: 1468
	// (get) Token: 0x060036AF RID: 13999 RVA: 0x001B7F2B File Offset: 0x001B612B
	private bool TaiwuIsKid
	{
		get
		{
			return this._taiwuDisplayData.AvatarRelatedData.DisplayAge < 16;
		}
	}

	// Token: 0x170005BD RID: 1469
	// (get) Token: 0x060036B0 RID: 14000 RVA: 0x001B7F41 File Offset: 0x001B6141
	private TravelSkeletonItem TravelSkeleton
	{
		get
		{
			return this._kidnappedTravelData.Valid ? Config.TravelSkeleton.Instance[56] : UI_PartWorldMap.GetSkeleton(this._mapModel.TaiwuCarrier);
		}
	}

	// Token: 0x060036B1 RID: 14001 RVA: 0x001B7F70 File Offset: 0x001B6170
	public override void OnInit(ArgumentBox argsBox)
	{
		this._mapModel = SingletonObject.getInstance<WorldMapModel>();
		this._moveInfo = new CrossAreaMoveInfo();
		this._newMoveInfo = new CrossAreaMoveInfo();
		this._travelIsFinish = false;
		this._travelDestAreaId = -1;
		this._travelAnimLastIdleState = null;
		this.SetTravelButtonsVisibility(false);
		this._partWorldView = base.CGet<PartWorldView>("PartWorldView");
		this._partWorldView.ScaleAndMoveRoot.GetComponent<PointerTrigger>().SetBindElement(this.Element);
		base.CGet<UISwitcher>("DirectReturn").Switch(this._mapModel.CurrentAreaId != this._mapModel.GetTaiwuVillageAreaId());
		base.CGet<SkeletonGraphic>("CharacterSkeleton").gameObject.SetActive(false);
		base.CGet<SkeletonGraphic>("SubCharacterSkeleton").gameObject.SetActive(false);
		base.CGet<SkeletonGraphic>("CarrierSkeleton").gameObject.SetActive(false);
		base.CGet<SkeletonGraphic>("TravelEventAnimation").gameObject.SetActive(false);
		this.UpdatePlayerPos(-1, false);
		base.CGet<RectTransform>("SimpleTravel").gameObject.SetActive(false);
		base.CGet<RectTransform>("ComplexTravel").gameObject.SetActive(false);
		base.CGet<CImage>("Arrow").gameObject.SetActive(true);
		base.GetComponent<CanvasGroup>().alpha = 0f;
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
	}

	// Token: 0x060036B2 RID: 14002 RVA: 0x001B80FC File Offset: 0x001B62FC
	private void OnHide()
	{
		bool flag = new Location(this._mapModel.CurrentAreaId, this._mapModel.CurrentBlockId).IsValid();
		if (flag)
		{
			bool isVisited;
			this._taiwuVisitedAreas.TryGetValue(this._mapModel.CurrentAreaId, out isVisited);
			UIElement.NewAreaNotify.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("AreaTemplateId", SingletonObject.getInstance<WorldMapModel>().Areas[(int)this._mapModel.CurrentAreaId].GetConfig().TemplateId).Set("IsVisited", isVisited));
			UIElement.NewAreaNotify.Show();
		}
	}

	// Token: 0x060036B3 RID: 14003 RVA: 0x001B819C File Offset: 0x001B639C
	private void Awake()
	{
		this._partWorldView = base.CGet<PartWorldView>("PartWorldView");
		this._partWorldView.OnClickArea = new Action<short>(this.OnClickArea);
		this._partWorldView.OnMouseEnterArea = new Action<short>(this.OnMouseEnterArea);
		this._partWorldView.OnMouseExitArea = new Action<short>(this.OnMouseExitArea);
		this._partWorldView.ScaleAndMoveRoot.OnScale = new Action<Vector3>(this.OnScale);
		this._partWorldView.PathHolder.GetComponent<Line2DGenerator>().OverrideVertices = this._travelPathVertices;
		GameObject gains = base.CGet<GameObject>("Gains");
		this._gainsScroll = gains.GetComponentInChildren<InfinityScrollLegacy>();
		this._gainsScroll.OnItemRender = new Action<int, Refers>(this.OnGainsRender);
		this._gainsScroll.SetDataCount(0);
		this._focusTaiwuVillage = base.CGet<RectTransform>("FocusTaiwuVillage");
		PoolManager.SetSrcObject("BigMap_CanPassRoutePrefab", base.CGet<GameObject>("CanPassRoutePrefab"));
		PoolManager.SetSrcObject("BigMap_CanUnlockRoutePrefab", base.CGet<GameObject>("CanUnlockRoutePrefab"));
		PoolManager.SetSrcObject("UI_PartWorldMap_BigMap_AreaEffect", base.CGet<AreaEffect>("AreaEffectPrefab").gameObject);
		RectTransform stateNameHolder = base.CGet<RectTransform>("StateNameHolder");
		sbyte stateId = 0;
		while ((int)stateId < stateNameHolder.childCount)
		{
			stateNameHolder.GetChild((int)stateId).GetComponent<TextMeshProUGUI>().text = MapState.Instance[(int)(stateId + 1)].Name;
			stateId += 1;
		}
		this.InitAreas();
		GEvent.Add(EEvents.OnGameStateChange, new GEvent.Callback(this.OnGameStateChange));
	}

	// Token: 0x060036B4 RID: 14004 RVA: 0x001B8330 File Offset: 0x001B6530
	private void OnDestroy()
	{
		PoolManager.RemoveData("UI_PartWorldMap_BigMap_AreaEffect");
		PoolManager.RemoveData("BigMap_CanPassRoutePrefab");
		PoolManager.RemoveData("BigMap_CanUnlockRoutePrefab");
		GEvent.Remove(EEvents.OnGameStateChange, new GEvent.Callback(this.OnGameStateChange));
	}

	// Token: 0x060036B5 RID: 14005 RVA: 0x001B836C File Offset: 0x001B656C
	private void OnEnable()
	{
		this.EventOnSetBottomRightPartVisible(false);
		GEvent.Add(EEvents.OnConfirmQuitGameState, new GEvent.Callback(this.OnConfirmQuitGameState));
		GEvent.Add(UiEvents.MonthNotifyProcessComplete, new GEvent.Callback(this.OnMonthNotifyComplete));
		GEvent.Add(EEvents.OnTaiwuCharIdChange, new GEvent.Callback(this.OnTaiwuCharIdChange));
		GEvent.Add(UiEvents.OnTravelPathUnlocked, new GEvent.Callback(this.OnTravelPathUnlocked));
		GEvent.Add(UiEvents.OnCharacterTaiwuCarrierChanged, new GEvent.Callback(this.OnCharacterTaiwuCarrierChanged));
		GEvent.Add(UiEvents.HidePartWorldMap, new GEvent.Callback(this.HidePartWorldMap));
		GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		this._partWorldView.ScaleAndMoveRoot.enabled = false;
		this._isQuickHiding = false;
		CToggleObsolete pauseToggle = base.CGet<CToggleObsolete>("PauseToggle");
		pauseToggle.isOn = false;
		ConchShipCursor.Instance.AddWheelProgress(-1f);
		ConchShipCursor.Instance.AddMouseWheelProgressFullHandler(this.Element.Name, new Action(this.OnMouseWheelProgressFull));
	}

	// Token: 0x060036B6 RID: 14006 RVA: 0x001B8498 File Offset: 0x001B6698
	private void OnDisable()
	{
		this.EventOnSetBottomRightPartVisible(true);
		this.SetTravelButtonsVisibility(false);
		GEvent.Remove(EEvents.OnConfirmQuitGameState, new GEvent.Callback(this.OnConfirmQuitGameState));
		GEvent.Remove(UiEvents.MonthNotifyProcessComplete, new GEvent.Callback(this.OnMonthNotifyComplete));
		GEvent.Remove(EEvents.OnTaiwuCharIdChange, new GEvent.Callback(this.OnTaiwuCharIdChange));
		GEvent.Remove(UiEvents.OnTravelPathUnlocked, new GEvent.Callback(this.OnTravelPathUnlocked));
		GEvent.Remove(UiEvents.OnCharacterTaiwuCarrierChanged, new GEvent.Callback(this.OnCharacterTaiwuCarrierChanged));
		GEvent.Remove(UiEvents.HidePartWorldMap, new GEvent.Callback(this.HidePartWorldMap));
		GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		this._partWorldView.ScaleAndMoveRoot.enabled = false;
		ConchShipCursor.Instance.RemoveMouseWheelProgressFullHandler(new Action(this.OnMouseWheelProgressFull));
		bool flag = !this._isQuickHiding;
		if (flag)
		{
			AudioManager.Instance.PlaySound(base.CGet<AudioClip>("SeEnter"), false, 100);
		}
		CommandKitBase.SetDisable(false);
		this.ProcessCachedGEvents();
		this.ClearMouseInState();
		this.ClearGainsGauge();
		this.ClearTravelPath();
	}

	// Token: 0x060036B7 RID: 14007 RVA: 0x001B85DC File Offset: 0x001B67DC
	private void Update()
	{
		this.UpdateTaiwuVillage();
		bool flag = this.UpdateCheckPause();
		if (!flag)
		{
			this.UpdateTravelAnimIdleState();
			this.UpdateCheckContinue();
			bool flag2 = CommandKitBase.GetDisable() || !UIManager.Instance.IsFocusElement(this.Element) || this._moveInfo.Traveling;
			if (!flag2)
			{
				bool disableCheckClose = this._disableCheckClose;
				if (disableCheckClose)
				{
					this._disableCheckClose = false;
				}
				else
				{
					this.UpdateCheckHide();
				}
			}
		}
	}

	// Token: 0x060036B8 RID: 14008 RVA: 0x001B8654 File Offset: 0x001B6854
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(19, 163, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(19, 164, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(2, 56, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(2, 51, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(19, 15, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(19, 204, ulong.MaxValue, null));
	}

	// Token: 0x060036B9 RID: 14009 RVA: 0x001B86FC File Offset: 0x001B68FC
	private void OnListenerIdReady()
	{
		CharacterDomainMethod.Call.GetCharacterDisplayDataList(this.Element.GameDataListenerId, new List<int>
		{
			this.TaiwuCharId
		});
		MapDomainMethod.Call.GetAllAreaDisplayData(this.Element.GameDataListenerId);
		this._partWorldView.ScaleAndMoveRoot.transform.localScale = Vector3.one;
		this._partWorldView.ScaleAndMoveRoot.Reset();
		this._taiwuVillagePos = this._partWorldView.AreaHolder.GetChild((int)this._mapModel.GetTaiwuVillageBlock().AreaId).GetComponent<RectTransform>().anchoredPosition - this._partWorldView.AreaHolder.sizeDelta / 2f;
		this.SetAreaStatusActive(true);
		bool flag = this._mapModel.CurrentAreaId < 135;
		if (flag)
		{
			this.FocusArea(this._mapModel.CurrentAreaId, 0.1f);
		}
		AudioManager.Instance.PlaySound(base.CGet<AudioClip>("SeEnter"), false, 100);
		base.CGet<CImage>("Arrow").SetSprite(CommonUtils.GetTaiwuSpriteName(), true, null);
	}

	// Token: 0x060036BA RID: 14010 RVA: 0x001B8820 File Offset: 0x001B6A20
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
					this.HandlerMethodReturn(notification, wrapper);
				}
			}
			else
			{
				this.HandlerDataModification(notification, wrapper);
			}
		}
	}

	// Token: 0x060036BB RID: 14011 RVA: 0x001B88A0 File Offset: 0x001B6AA0
	protected override void OnClick(Transform btn)
	{
		string name = btn.name;
		string a = name;
		if (!(a == "FocusTaiwuVillage"))
		{
			if (!(a == "StopTravel"))
			{
				if (!(a == "DirectReturn"))
				{
					if (a == "CloseBtn")
					{
						this._isQuickHiding = true;
						this.QuickHide();
					}
				}
				else
				{
					this._travelDestAreaId = this._mapModel.GetTaiwuVillageBlock().AreaId;
					CommandManager.AddCommandMethodCall<short, short, short>(EPriority.CallMethodNormal, 2, 11, this._mapModel.CurrentAreaId, this._mapModel.CurrentBlockId, this._mapModel.GetTaiwuVillageAreaId(), delegate(int offset, RawDataPool pool)
					{
						CrossAreaMoveInfo moveInfo = new CrossAreaMoveInfo();
						Serializer.Deserialize(pool, offset, ref moveInfo);
						moveInfo.AuthorityCost = 0;
						TravelUtils.ShowDirectTravelConfirmDialog(moveInfo, new Action(MapDomainMethod.Call.DirectTravelToTaiwuVillage), delegate
						{
							this._travelDestAreaId = -1;
						});
					}, null);
				}
			}
			else if (SingletonObject.getInstance<BasicGameData>().AdvancingMonthState == 0)
			{
				if (!this._isDirectTraveling)
				{
					DialogCmd dialogCmd = new DialogCmd
					{
						Type = 1,
						Yes = new Action(this.StopTravel),
						No = delegate()
						{
							this.EnableStateMask(true, true);
						},
						Title = LocalStringManager.Get(LanguageKey.UI_Stop_Travel),
						Content = LocalStringManager.Get(LanguageKey.UI_Stop_Travel_Confirm)
					};
					ArgumentBox argBox = EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd);
					CommandManager.AddCommandShowUI(EPriority.ShowUINormal, UIElement.Dialog, argBox);
				}
			}
		}
		else if (!WorldMapModel.Traveling)
		{
			this.FocusArea(this._mapModel.GetTaiwuVillageBlock().AreaId, 0.1f);
		}
	}

	// Token: 0x060036BC RID: 14012 RVA: 0x001B8A1C File Offset: 0x001B6C1C
	public override void QuickHide()
	{
		AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
		this.OnMouseExitState((int)this._currSelectedStateId);
		bool flag = this._travelDestAreaId < 0;
		if (flag)
		{
			this.HideSelf();
		}
	}

	// Token: 0x060036BD RID: 14013 RVA: 0x001B8A60 File Offset: 0x001B6C60
	private void OnGameStateChange(ArgumentBox argBox)
	{
		Enum state;
		argBox.Get("newState", out state);
		bool flag = (EGameState)state != EGameState.Loading;
		if (!flag)
		{
			this.Element.OnHide = null;
			GEvent.ClearEvent(UiEvents.OnTravelCheckPointProcessed);
		}
	}

	// Token: 0x060036BE RID: 14014 RVA: 0x001B8AA8 File Offset: 0x001B6CA8
	private void OnConfirmQuitGameState(ArgumentBox argBox)
	{
		bool traveling = WorldMapModel.Traveling;
		if (traveling)
		{
			bool show;
			argBox.Get("ShowState", out show);
			Time.timeScale = (float)(show ? 0 : 1);
			CommandKitBase.SetDisable(!show);
		}
	}

	// Token: 0x060036BF RID: 14015 RVA: 0x001B8AE8 File Offset: 0x001B6CE8
	private void OnWorldMapInit(ArgumentBox argbox)
	{
		bool travelIsFinish = this._travelIsFinish;
		if (travelIsFinish)
		{
			this.FinishAndHideSelf();
		}
	}

	// Token: 0x060036C0 RID: 14016 RVA: 0x001B8B07 File Offset: 0x001B6D07
	private void OnMonthNotifyComplete(ArgumentBox argBox)
	{
		MapDomainMethod.Call.GetAllAreaDisplayData(this.Element.GameDataListenerId);
	}

	// Token: 0x060036C1 RID: 14017 RVA: 0x001B8B1C File Offset: 0x001B6D1C
	private void OnTaiwuCharIdChange(ArgumentBox argBox)
	{
		base.CGet<CImage>("Arrow").SetSprite(CommonUtils.GetTaiwuSpriteName(), true, null);
		this.ProcessCachedGEvents();
		this.FinishAndHideSelf();
		GEvent.OnEvent(UiEvents.WorldMapResetMapCamera, EasyPool.Get<ArgumentBox>().Set("isAnim", false));
	}

	// Token: 0x060036C2 RID: 14018 RVA: 0x001B8B6D File Offset: 0x001B6D6D
	private void OnTravelPathUnlocked(ArgumentBox argbox)
	{
		CommandManager.AddCommandMethodCall(EPriority.CallGetAreaDisplayData, 2, 35, delegate(int offset, RawDataPool pool)
		{
			Serializer.Deserialize(pool, offset, ref this._areaDisplayData);
			this.HandlerMethodGetAllAreaDisplayData();
			this.UpdateAreaRoute();
		}, null);
	}

	// Token: 0x060036C3 RID: 14019 RVA: 0x001B8B87 File Offset: 0x001B6D87
	private void HidePartWorldMap(ArgumentBox argbox)
	{
		this.HideSelf();
	}

	// Token: 0x060036C4 RID: 14020 RVA: 0x001B8B91 File Offset: 0x001B6D91
	private void OnTopUiChanged(ArgumentBox argumentBox)
	{
		this._disableCheckClose = true;
	}

	// Token: 0x060036C5 RID: 14021 RVA: 0x001B8B9C File Offset: 0x001B6D9C
	private void OnCharacterTaiwuCarrierChanged(ArgumentBox argbox)
	{
		bool traveling = this._moveInfo.Traveling;
		if (traveling)
		{
			this.PlayCarrierAnim();
		}
	}

	// Token: 0x060036C6 RID: 14022 RVA: 0x001B8BC0 File Offset: 0x001B6DC0
	private void EventOnSetBottomRightPartVisible(bool visible)
	{
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("visible", visible).Set("timeDiskVisible", true).Set("backendVisible", true).Set("backendForceVisible", true);
		GEvent.OnEvent(UiEvents.OnSetBottomRightPartVisible, argumentBox);
	}

	// Token: 0x060036C7 RID: 14023 RVA: 0x001B8C14 File Offset: 0x001B6E14
	private void UpdateTaiwuVillage()
	{
		RectTransform rectTrans = this._partWorldView.ScaleAndMoveRoot.GetComponent<RectTransform>();
		Rect parentRect = rectTrans.parent.GetComponent<RectTransform>().rect;
		float diffW = this._focusTaiwuVillage.rect.width;
		float diffH = this._focusTaiwuVillage.rect.height;
		parentRect.xMin += diffW * 0.2f;
		parentRect.xMax -= diffW * 0.2f;
		parentRect.yMin += diffH * 1.5f;
		parentRect.yMax -= diffH * 2f;
		float rectWidth = rectTrans.sizeDelta.x * rectTrans.localScale.x;
		float rectHeight = rectTrans.sizeDelta.y * rectTrans.localScale.y;
		Vector2 currCenterPos = new Vector2(-(rectTrans.anchoredPosition.x - rectWidth * (rectTrans.pivot.x - 0.5f)) / rectTrans.localScale.x, -(rectTrans.anchoredPosition.y - rectHeight * (rectTrans.pivot.y - 0.5f)) / rectTrans.localScale.y);
		float viewportX = (this._taiwuVillagePos.x - currCenterPos.x) * rectTrans.localScale.x;
		float viewportY = (this._taiwuVillagePos.y - currCenterPos.y) * rectTrans.localScale.y;
		bool taiwuVillageVisible = -parentRect.width / 2f < viewportX && viewportX < parentRect.width / 2f && -parentRect.height / 2f < viewportY && viewportY < parentRect.height / 2f;
		this._focusTaiwuVillage.gameObject.SetActive(!taiwuVillageVisible);
		bool flag = !taiwuVillageVisible;
		if (flag)
		{
			float slope = Mathf.Abs(viewportY / viewportX);
			float btnX = 0f;
			float btnY = 0f;
			float intersectX = parentRect.height / 2f / slope * (float)((viewportX > 0f) ? 1 : -1);
			float intersectY = parentRect.width / 2f * slope * (float)((viewportY > 0f) ? 1 : -1);
			bool flag2 = -parentRect.width / 2f <= intersectX && intersectX <= parentRect.width / 2f;
			if (flag2)
			{
				btnX = Mathf.Clamp(intersectX, -parentRect.width / 2f + 70f, parentRect.width / 2f - 70f);
				btnY = (parentRect.height / 2f - 70f) * (float)((viewportY > 0f) ? 1 : -1) + this._focusTaiwuVillage.rect.height * 0.5f;
			}
			else
			{
				bool flag3 = -parentRect.height / 2f <= intersectY && intersectY <= parentRect.height / 2f;
				if (flag3)
				{
					btnX = (parentRect.width / 2f - 70f) * (float)((viewportX > 0f) ? 1 : -1);
					btnY = Mathf.Clamp(intersectY, -parentRect.height / 2f + 70f, parentRect.height / 2f - 70f);
				}
			}
			this._focusTaiwuVillage.anchoredPosition = new Vector2(btnX, btnY);
			this._focusTaiwuVillage.GetChild(1).localRotation = Quaternion.Euler(new Vector3(0f, 0f, Vector2.SignedAngle(Vector2.right, new Vector2(viewportX, viewportY))));
		}
	}

	// Token: 0x060036C8 RID: 14024 RVA: 0x001B8FE4 File Offset: 0x001B71E4
	private bool UpdateCheckPause()
	{
		CToggleObsolete pauseToggle = base.CGet<CToggleObsolete>("PauseToggle");
		bool interactable = pauseToggle.interactable;
		if (interactable)
		{
			bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) && WorldMapModel.Traveling;
			if (flag)
			{
				CToggleObsolete ctoggleObsolete = pauseToggle;
				ctoggleObsolete.isOn = !ctoggleObsolete.isOn;
				return true;
			}
			bool isFocus = UIManager.Instance.IsFocusElement(this.Element);
			bool isPause = pauseToggle.isOn;
			bool flag2 = !isFocus && !isPause;
			if (flag2)
			{
				pauseToggle.isOn = true;
				this._isTravelAutoPaused = true;
				return true;
			}
			bool flag3 = isFocus && isPause && this._isTravelAutoPaused;
			if (flag3)
			{
				pauseToggle.isOn = false;
				this._isTravelAutoPaused = false;
				return true;
			}
		}
		return false;
	}

	// Token: 0x060036C9 RID: 14025 RVA: 0x001B90B8 File Offset: 0x001B72B8
	private void UpdateTravelAnimIdleState()
	{
		CrossAreaMoveInfo moveInfo = this._moveInfo;
		bool flag = moveInfo == null || !moveInfo.Traveling;
		if (!flag)
		{
			CToggleObsolete pauseToggle = base.CGet<CToggleObsolete>("PauseToggle");
			bool paused = this._isTravelAutoPaused || pauseToggle.isOn;
			bool flag2 = this._travelAnimLastIdleState == null || this._travelAnimLastIdleState.Value != paused;
			if (flag2)
			{
				this.PlayCarrierAnim();
			}
			this._travelAnimLastIdleState = new bool?(paused);
		}
	}

	// Token: 0x060036CA RID: 14026 RVA: 0x001B913C File Offset: 0x001B733C
	private void UpdateCheckContinue()
	{
		bool flag = this._continueWaitingSeconds < 0f;
		if (!flag)
		{
			bool flag2 = !this._moveInfo.Traveling;
			if (flag2)
			{
				this.ClearWait();
			}
			else
			{
				this._continueWaitingSeconds -= Time.deltaTime;
				bool flag3 = this._continueWaitingSeconds < 0f;
				if (flag3)
				{
					this.ContinueTravelWithCheck();
				}
			}
		}
	}

	// Token: 0x060036CB RID: 14027 RVA: 0x001B91A4 File Offset: 0x001B73A4
	private void UpdateCheckHide()
	{
		this._partWorldView.ScaleAndMoveRoot.enabled = this.Element.Ready;
		bool flag = this._travelDestAreaId < 0 && this.Element.Ready;
		if (flag)
		{
			this._partWorldView.DragRoot.enabled = true;
			this._partWorldView.ScaleAndMoveRoot.enabled = true;
			float currScale = this._partWorldView.ScaleAndMoveRoot.transform.localScale.x;
			float scrollValue = Input.GetAxis("Mouse ScrollWheel");
			bool flag2 = currScale >= this._partWorldView.ScaleAndMoveRoot.Max.x && scrollValue > 0f;
			if (flag2)
			{
				ConchShipCursor.Instance.AddWheelProgress(scrollValue * 2f);
			}
			else
			{
				bool flag3 = scrollValue < 0f;
				if (flag3)
				{
					ConchShipCursor.Instance.AddWheelProgress(-1f);
				}
			}
			bool flag4 = CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false) || CommonCommandKit.RightMouse.Check(this.Element, false, false, false, true, false);
			if (flag4)
			{
				this.QuickHide();
			}
		}
		else
		{
			this._partWorldView.DragRoot.enabled = false;
			this._partWorldView.ScaleAndMoveRoot.enabled = false;
		}
		bool flag5 = this._travelIsFinish && UIManager.Instance.IsFocusElement(this.Element) && WorldMapModel.MapBlockLoadFinish && WorldMapModel.MapBlockUiLoadFinish;
		if (flag5)
		{
			this.FinishAndHideSelf();
		}
	}

	// Token: 0x060036CC RID: 14028 RVA: 0x001B9330 File Offset: 0x001B7530
	private void HandlerDataModification(Notification notification, NotificationWrapper wrapper)
	{
		DataUid uid = notification.Uid;
		ushort domainId = uid.DomainId;
		ushort num = domainId;
		if (num != 2)
		{
			if (num == 19)
			{
				if (uid.DataId != 15)
				{
					if (uid.DataId != 163)
					{
						if (uid.DataId != 164)
						{
							if (uid.DataId == 204)
							{
								Serializer.DeserializeModifications<short>(wrapper.DataPool, notification.ValueOffset, this._taiwuVisitedAreas);
								base.RemoveMonitorFieldId(19, 204);
							}
						}
						else
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._kidnappedTravelData);
						}
					}
					else
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._isDirectTraveling);
						this.ChangeTravelInteractable();
					}
				}
				else
				{
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._gainsInTravel);
					this.HandlerDataGainsInTravel();
				}
			}
		}
		else if (uid.DataId != 56)
		{
			if (uid.DataId == 51)
			{
				Serializer.DeserializeModifications<TravelRouteKey>(wrapper.DataPool, notification.ValueOffset, this._travelRoutes);
				this.HandlerDataTravelRoute();
			}
		}
		else
		{
			Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._newMoveInfo);
			this.HandlerDataTravelInfo();
		}
	}

	// Token: 0x060036CD RID: 14029 RVA: 0x001B9484 File Offset: 0x001B7684
	private void HandlerMethodReturn(Notification notification, NotificationWrapper wrapper)
	{
		ushort domainId = notification.DomainId;
		ushort num = domainId;
		if (num != 2)
		{
			if (num == 4)
			{
				if (notification.MethodId == 48)
				{
					List<CharacterDisplayData> displayDataList = null;
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref displayDataList);
					this.HandlerMethodGetCharacterDisplayDataList(displayDataList);
				}
			}
		}
		else if (notification.MethodId != 11)
		{
			if (notification.MethodId == 35)
			{
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._areaDisplayData);
				this.HandlerMethodGetAllAreaDisplayData();
			}
		}
		else
		{
			CrossAreaMoveInfo travelInfo = new CrossAreaMoveInfo();
			Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref travelInfo);
			this.HandlerMethodGetTravelCost(travelInfo);
		}
	}

	// Token: 0x060036CE RID: 14030 RVA: 0x001B9538 File Offset: 0x001B7738
	private void HandlerMethodContinueTravelWithDetectTravelingEvent(int offset, RawDataPool pool)
	{
		short travelingEventId = -1;
		Serializer.Deserialize(pool, offset, ref travelingEventId);
		this.HandlerMethodContinueTravelWithDetectTravelingEvent(travelingEventId);
	}

	// Token: 0x060036CF RID: 14031 RVA: 0x001B955A File Offset: 0x001B775A
	private void HandlerMethodContinueTravel(int offset, RawDataPool pool)
	{
		this.HandlerMethodContinueTravelWithDetectTravelingEvent(-1);
	}

	// Token: 0x060036D0 RID: 14032 RVA: 0x001B9568 File Offset: 0x001B7768
	private void HandlerDataTravelInfo()
	{
		AdaptableLog.Info(string.Format("HandlerDataTravelInfo {0} {1} {2}", this._newMoveInfo.Traveling, this._newMoveInfo.FromAreaId, this._newMoveInfo.ToAreaId));
		bool flag = !this._moveInfo.Traveling && this._newMoveInfo.Traveling;
		if (flag)
		{
			this.StartTravel(this._newMoveInfo);
		}
		bool flag2 = this._moveInfo.Traveling && !this._newMoveInfo.Traveling;
		if (flag2)
		{
			this._travelIsFinish = true;
		}
		this.SyncRouteCostDays();
		this._nextAreaCostDays = this._moveInfo.NextCostDays;
		short srcArea = this._moveInfo.CurrentAreaId;
		short dstArea = this._newMoveInfo.CurrentAreaId;
		bool flag3 = srcArea != dstArea && this._moveInfo.Traveling && this._newMoveInfo.Traveling;
		if (flag3)
		{
			this.PlayTravelAnim(srcArea, dstArea);
		}
		this.ResetWait();
		CrossAreaMoveInfo newMoveInfo = this._newMoveInfo;
		CrossAreaMoveInfo moveInfo = this._moveInfo;
		this._moveInfo = newMoveInfo;
		this._newMoveInfo = moveInfo;
	}

	// Token: 0x060036D1 RID: 14033 RVA: 0x001B9690 File Offset: 0x001B7890
	private void HandlerDataTravelRoute()
	{
		this.UpdateAreaRoute();
	}

	// Token: 0x060036D2 RID: 14034 RVA: 0x001B969C File Offset: 0x001B789C
	private void HandlerDataGainsInTravel()
	{
		bool flag = this._gainsInTravel == null;
		if (flag)
		{
			this._gainsScroll.UpdateData(0);
		}
		else
		{
			this._gainsScroll.UpdateData(this._gainsInTravel.Count);
		}
	}

	// Token: 0x060036D3 RID: 14035 RVA: 0x001B96E0 File Offset: 0x001B78E0
	private void HandlerMethodGetTravelCost(CrossAreaMoveInfo travelInfo)
	{
		bool flag = travelInfo.ToAreaId != this._currSelectedAreaId;
		if (!flag)
		{
			this._currSelectedAreaMoveInfo = travelInfo;
			this.UpdateTravelPath(travelInfo);
		}
	}

	// Token: 0x060036D4 RID: 14036 RVA: 0x001B9714 File Offset: 0x001B7914
	private void HandlerMethodContinueTravelWithDetectTravelingEvent(short travelingEventId)
	{
		this.ChangeTravelInteractable();
		bool flag = travelingEventId < 0;
		if (!flag)
		{
			TravelingEventItem config = TravelingEvent.Instance[travelingEventId];
			string aniPath = string.Format("RemakeResources/SpineAnimations/Travel/TravelingEvent_{0}_SkeletonData", (int)config.DisplayType);
			ResLoader.Load<SkeletonDataAsset>(aniPath, delegate(SkeletonDataAsset aniData)
			{
				SkeletonGraphic travelEventSkeleton = this.CGet<SkeletonGraphic>("TravelEventAnimation");
				travelEventSkeleton.skeletonDataAsset = aniData;
				travelEventSkeleton.Initialize(true);
				travelEventSkeleton.AnimationState.SetAnimation(0, "animation", false);
				travelEventSkeleton.timeScale = this.StepAnimationDuration / 0.5f;
				travelEventSkeleton.gameObject.SetActive(true);
			}, null, false);
			bool flag2 = string.IsNullOrEmpty(config.Event);
			if (!flag2)
			{
				AdaptableLog.Info(string.Format("HandlerMethodContinueTravelWithDetectTravelingEvent wait for {0}", travelingEventId));
				this.ChangeTravelInteractable(false, true);
				short destAreaId = this._travelDestAreaId;
				GEvent.AddOneShot(UiEvents.OnTravelCheckPointProcessed, delegate(ArgumentBox _)
				{
					AdaptableLog.Info(string.Format("HandlerMethodContinueTravelWithDetectTravelingEvent waited {0}", travelingEventId));
					this._travelDestAreaId = destAreaId;
					this.ResetWait(0.33f);
					this.ChangeTravelInteractable(this.CanTravelInteractable, false);
				});
			}
		}
	}

	// Token: 0x060036D5 RID: 14037 RVA: 0x001B97E7 File Offset: 0x001B79E7
	private void HandlerMethodGetAllAreaDisplayData()
	{
		this.UpdateAreaInfo();
	}

	// Token: 0x060036D6 RID: 14038 RVA: 0x001B97F4 File Offset: 0x001B79F4
	private void HandlerMethodGetCharacterDisplayDataList(List<CharacterDisplayData> displayDataList)
	{
		this._taiwuDisplayData = displayDataList[0];
		bool traveling = this._moveInfo.Traveling;
		if (traveling)
		{
			this.PlayCarrierAnim();
			this.PlayCarrierSound();
		}
	}

	// Token: 0x060036D7 RID: 14039 RVA: 0x001B982E File Offset: 0x001B7A2E
	private void OnMouseWheelProgressFull()
	{
		ConchShipCursor.Instance.AddWheelProgress(-1f);
		this.QuickHide();
	}

	// Token: 0x060036D8 RID: 14040 RVA: 0x001B9848 File Offset: 0x001B7A48
	private void OnMouseEnterArea(short areaId)
	{
		this.OnMouseEnterState((int)this._mapModel.GetStateId(areaId));
		bool traveling = this._moveInfo.Traveling;
		if (!traveling)
		{
			this._currSelectedAreaId = areaId;
			this._currSelectedAreaMoveInfo = null;
			bool flag = areaId != this._mapModel.CurrentAreaId;
			if (flag)
			{
				MapDomainMethod.Call.GetTravelCost(this.Element.GameDataListenerId, this._mapModel.CurrentAreaId, this._mapModel.CurrentBlockId, areaId);
			}
		}
	}

	// Token: 0x060036D9 RID: 14041 RVA: 0x001B98C8 File Offset: 0x001B7AC8
	private void OnMouseExitArea(short areaId)
	{
		this.OnMouseExitState((int)this._mapModel.GetStateId(areaId));
		bool traveling = this._moveInfo.Traveling;
		if (!traveling)
		{
			bool flag = this._currSelectedAreaMoveInfo != null;
			if (flag)
			{
				this.ClearTravelPath();
			}
			this._currSelectedAreaId = -1;
			this._currSelectedAreaMoveInfo = null;
		}
	}

	// Token: 0x060036DA RID: 14042 RVA: 0x001B991C File Offset: 0x001B7B1C
	private void OnScale(Vector3 scale)
	{
		bool inActive = !Mathf.Approximately(0.5f, scale.x) && !Mathf.Approximately(0.5f, scale.y);
		bool flag = this._isAreaStatusActive != inActive;
		if (flag)
		{
			this.SetAreaStatusActive(inActive);
		}
	}

	// Token: 0x060036DB RID: 14043 RVA: 0x001B996C File Offset: 0x001B7B6C
	private void EnableStateMask(bool enable, bool callMouseExit = true)
	{
		if (callMouseExit)
		{
			this.OnMouseExitState((int)this._currSelectedStateId);
		}
		for (int i = 0; i < this._partWorldView.StateMaskHolder.childCount; i++)
		{
			this._partWorldView.StateMaskHolder.GetChild(i).GetComponent<IrregularClickableImage>().enabled = enable;
		}
	}

	// Token: 0x060036DC RID: 14044 RVA: 0x001B99C8 File Offset: 0x001B7BC8
	private void OnClickArea(short areaId)
	{
		bool flag = areaId == this._mapModel.CurrentAreaId || this._travelDestAreaId >= 0 || this._moveInfo.Traveling || this._travelIsFinish;
		if (!flag)
		{
			bool flag2 = this._currSelectedAreaId != areaId || this._currSelectedAreaMoveInfo == null;
			if (!flag2)
			{
				this._travelDestAreaId = areaId;
				this.ShowTravelTipsDialog(this._currSelectedAreaMoveInfo);
			}
		}
	}

	// Token: 0x060036DD RID: 14045 RVA: 0x001B9A38 File Offset: 0x001B7C38
	private void ChangeTravelInteractable()
	{
		this.ChangeTravelInteractable(this.CanTravelInteractable);
	}

	// Token: 0x060036DE RID: 14046 RVA: 0x001B9A47 File Offset: 0x001B7C47
	private void ChangeTravelInteractable(bool interactable)
	{
		base.CGet<CToggleObsolete>("PauseToggle").interactable = interactable;
		base.CGet<UISwitcher>("StopTravel").Switch(interactable && !this._isDirectTraveling);
	}

	// Token: 0x060036DF RID: 14047 RVA: 0x001B9A7C File Offset: 0x001B7C7C
	private void ChangeTravelInteractable(bool interactable, bool isPause)
	{
		base.CGet<CToggleObsolete>("PauseToggle").isOn = isPause;
		this.ChangeTravelInteractable(interactable);
	}

	// Token: 0x060036E0 RID: 14048 RVA: 0x001B9A99 File Offset: 0x001B7C99
	private void ShowTravelTipsDialog(CrossAreaMoveInfo moveInfo)
	{
		TravelUtils.ShowTravelConfirmDialog(moveInfo, new Action(this.ConfirmTravel), new Action(this.CancelTravel));
		this.EnableStateMask(false, false);
	}

	// Token: 0x060036E1 RID: 14049 RVA: 0x001B9AC4 File Offset: 0x001B7CC4
	private void StartTravel(CrossAreaMoveInfo moveInfo)
	{
		this.InvokeFirstEvent();
		short area = moveInfo.CurrentAreaId;
		this.FocusArea(area, 0.1f);
		this.UpdatePlayerPos(area, false);
		this.UpdateAreaTravelInfo(moveInfo);
		this.ShowTravelInfo(moveInfo);
		this.SetTravelButtonsVisibility(true);
		this.ChangeTravelInteractable(true);
		this.PlayCarrierAnim();
		this.PlayCarrierSound();
		this.TravelTweenCallback(moveInfo);
	}

	// Token: 0x060036E2 RID: 14050 RVA: 0x001B9B2C File Offset: 0x001B7D2C
	private void ConfirmTravel()
	{
		this.EnableStateMask(true, true);
		MapDomainMethod.Call.StartTravel(this._travelDestAreaId);
	}

	// Token: 0x060036E3 RID: 14051 RVA: 0x001B9B44 File Offset: 0x001B7D44
	private void CancelTravel()
	{
		this._travelDestAreaId = -1;
		this.ClearTravelPath();
		this.EnableStateMask(true, true);
	}

	// Token: 0x060036E4 RID: 14052 RVA: 0x001B9B60 File Offset: 0x001B7D60
	private void StopTravel()
	{
		this.ClearWait();
		MapDomainMethod.Call.StopTravel();
		bool flag = this._moveInfo.CurrentAreaId == this._moveInfo.FromAreaId;
		if (flag)
		{
			this.HideSelf();
		}
		this.ProcessCachedGEvents();
		this.EnableStateMask(true, true);
		this.ChangeTravelInteractable(false);
	}

	// Token: 0x060036E5 RID: 14053 RVA: 0x001B9BB8 File Offset: 0x001B7DB8
	private void ContinueTravel()
	{
		AudioManager.Instance.PlayAmbience(AudioManager.DummyAudioName, 1f, 100);
		WorldMapModel.MapBlockLoadFinish = false;
		WorldMapModel.MapBlockUiLoadFinish = false;
		bool flag = GMFunc.AvoidTravelEvent || this._kidnappedTravelData.Valid;
		if (flag)
		{
			CommandManager.AddCommandMethodCall(EPriority.CallContinueTravel, 2, 13, new CallMethodRespHandler(this.HandlerMethodContinueTravel), new CallMethodSkipHandler(this.ResetWait));
		}
		else
		{
			CommandManager.AddCommandMethodCall(EPriority.CallContinueTravel, 2, 31, new CallMethodRespHandler(this.HandlerMethodContinueTravelWithDetectTravelingEvent), new CallMethodSkipHandler(this.ResetWait));
		}
	}

	// Token: 0x060036E6 RID: 14054 RVA: 0x001B9C48 File Offset: 0x001B7E48
	private void ContinueTravelWithCheck()
	{
		UIElement conflictElement = null;
		bool exist = UIElement.Loading.Exist;
		if (exist)
		{
			conflictElement = UIElement.Loading;
		}
		bool exist2 = UIElement.MonthNotify.Exist;
		if (exist2)
		{
			conflictElement = UIElement.MonthNotify;
		}
		bool exist3 = UIElement.CombatBegin.Exist;
		if (exist3)
		{
			conflictElement = UIElement.CombatBegin;
		}
		bool exist4 = UIElement.Combat.Exist;
		if (exist4)
		{
			conflictElement = UIElement.Combat;
		}
		bool exist5 = UIElement.EventWindow.Exist;
		if (exist5)
		{
			conflictElement = UIElement.EventWindow;
		}
		bool inConflict = conflictElement != null || SingletonObject.getInstance<EventModel>().HasListeningEvent;
		BasicGameData basic = SingletonObject.getInstance<BasicGameData>();
		bool inAdvance = basic.AdvancingMonthState != 0 || basic.SavingWorld;
		CToggleObsolete pauseToggle = base.CGet<CToggleObsolete>("PauseToggle");
		bool inPause = pauseToggle.isOn;
		bool flag = inConflict || inAdvance || inPause || CommandManager.IsRunning;
		if (flag)
		{
			this.ResetWait();
		}
		else
		{
			this.ContinueTravel();
		}
	}

	// Token: 0x060036E7 RID: 14055 RVA: 0x001B9D2F File Offset: 0x001B7F2F
	private void InvokeFirstEvent()
	{
		GEvent.OnEvent(UiEvents.OnTravelStart, null);
		GEvent.AddOneShot(UiEvents.WorldMapInited, new GEvent.Callback(this.OnWorldMapInit));
	}

	// Token: 0x060036E8 RID: 14056 RVA: 0x001B9D59 File Offset: 0x001B7F59
	private void ResetWait()
	{
		this._continueWaitingSeconds = this.StepAnimationDuration;
	}

	// Token: 0x060036E9 RID: 14057 RVA: 0x001B9D68 File Offset: 0x001B7F68
	private void ResetWait(float ratio)
	{
		this._continueWaitingSeconds = this.StepAnimationDuration * ratio;
	}

	// Token: 0x060036EA RID: 14058 RVA: 0x001B9D79 File Offset: 0x001B7F79
	private void ClearWait()
	{
		this._continueWaitingSeconds = -1f;
	}

	// Token: 0x060036EB RID: 14059 RVA: 0x001B9D88 File Offset: 0x001B7F88
	private void SyncRouteCostDays()
	{
		bool flag = this._moveInfo.Route.CostList.Count != this._newMoveInfo.Route.CostList.Count;
		if (!flag)
		{
			this._moveInfo.Route.CostList.Clear();
			this._moveInfo.Route.CostList.AddRange(this._newMoveInfo.Route.CostList);
		}
	}

	// Token: 0x060036EC RID: 14060 RVA: 0x001B9E07 File Offset: 0x001B8007
	private void PlayTravelAnim(short srcArea, short dstArea)
	{
		this.UpdateTravelPath(this._newMoveInfo);
		this.TravelTweenCallback(this._newMoveInfo);
		this.FocusArea(dstArea, this.StepAnimationDurationWithoutLimit);
	}

	// Token: 0x060036ED RID: 14061 RVA: 0x001B9E34 File Offset: 0x001B8034
	private void PlayCarrierAnim()
	{
		bool isMove = this._moveInfo.Traveling && !this._isTravelAutoPaused && !base.CGet<CToggleObsolete>("PauseToggle").isOn;
		this.PlayCarrierAnimCharacter(isMove);
		this.PlayCarrierAnimCarrier(isMove);
		SkeletonGraphic simple = base.CGet<RectTransform>("SimpleTravel").GetComponentInChildren<SkeletonGraphic>();
		this.PlayCarrierAnimOther(simple, this.TravelSkeleton.SimpleAnimation, isMove);
		SkeletonGraphic complex = base.CGet<RectTransform>("ComplexTravel").GetComponent<Refers>().CGet<SkeletonGraphic>("Muheren");
		this.PlayCarrierAnimOther(complex, this.TravelSkeleton.ComplexAnimation, isMove);
	}

	// Token: 0x060036EE RID: 14062 RVA: 0x001B9ED4 File Offset: 0x001B80D4
	private void PlayCarrierAnimCharacter(bool isMove)
	{
		SkeletonGraphic charSkeleton = base.CGet<SkeletonGraphic>("CharacterSkeleton");
		SkeletonGraphic subCharSkeleton = base.CGet<SkeletonGraphic>("SubCharacterSkeleton");
		this.PlayCarrierAnimCharacterEquipments(charSkeleton, this.TaiwuCharId);
		string animName = isMove ? this.TravelSkeleton.Animation : this.TravelSkeleton.AnimationIdle;
		this.PlayCarrierAnimCharacterAnimation(charSkeleton, animName);
		bool valid = this._kidnappedTravelData.Valid;
		if (valid)
		{
			this.PlayCarrierAnimCharacterEquipments(subCharSkeleton, this._kidnappedTravelData.HunterCharId);
			string subAnimName = isMove ? this.TravelSkeleton.SubAnimation : this.TravelSkeleton.SubAnimationIdle;
			this.PlayCarrierAnimCharacterAnimation(subCharSkeleton, subAnimName);
		}
		else
		{
			subCharSkeleton.gameObject.SetActive(false);
		}
	}

	// Token: 0x060036EF RID: 14063 RVA: 0x001B9F88 File Offset: 0x001B8188
	private void PlayCarrierAnimCharacterAnimation(SkeletonGraphic charSkeleton, string animName)
	{
		string animPath = "RemakeResources/SpineAnimations/Character/TravelAnimations/" + animName;
		ResLoader.Load<RawAnimationAsset>(animPath, delegate(RawAnimationAsset rawAnimation)
		{
			Spine.Animation anim = rawAnimation.GetAnimation(charSkeleton.skeletonDataAsset);
			charSkeleton.AnimationState.SetAnimation(0, anim, true);
			charSkeleton.gameObject.SetActive(true);
		}, null, false);
	}

	// Token: 0x060036F0 RID: 14064 RVA: 0x001B9FC4 File Offset: 0x001B81C4
	private void PlayCarrierAnimCharacterEquipments(SkeletonGraphic skeleton, int charId)
	{
		CharacterDisplayData displayData = (charId == this.TaiwuCharId) ? this._taiwuDisplayData : null;
		bool flag = displayData == null;
		if (flag)
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(this, charId, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref displayData);
			});
		}
		CharacterDomainMethod.AsyncCall.GetAllEquipmentItems(this, charId, delegate(int offset, RawDataPool pool)
		{
			List<ItemDisplayData> equipments = EasyPool.Get<List<ItemDisplayData>>();
			Serializer.Deserialize(pool, offset, ref equipments);
			CombatAnimationUtils.UpdateSkeleton(skeleton, displayData, equipments);
			EasyPool.Free<List<ItemDisplayData>>(equipments);
		});
	}

	// Token: 0x060036F1 RID: 14065 RVA: 0x001BA02C File Offset: 0x001B822C
	private void PlayCarrierAnimCarrier(bool isMove)
	{
		SkeletonGraphic carrierSkeleton = base.CGet<SkeletonGraphic>("CarrierSkeleton");
		carrierSkeleton.gameObject.SetActive(this.TravelSkeleton.AnyCarrier);
		bool flag = !this.TravelSkeleton.AnyCarrier;
		if (!flag)
		{
			string carrierAnim = isMove ? this.TravelSkeleton.CarrierAnimation : this.TravelSkeleton.CarrierAnimationIdle;
			string carrierAnimPath = "RemakeResources/SpineAnimations/Carrier/" + this.TravelSkeleton.CarrierAnimationPath;
			ResLoader.Load<SkeletonDataAsset>(carrierAnimPath, delegate(SkeletonDataAsset animData)
			{
				carrierSkeleton.skeletonDataAsset = animData;
				carrierSkeleton.initialSkinName = this.TravelSkeleton.CarrierAnimationSkin;
				carrierSkeleton.Initialize(true);
				carrierSkeleton.transform.localScale = Vector3.one * (this.TaiwuIsKid ? 0.85f : 1f);
				carrierSkeleton.AnimationState.SetAnimation(0, carrierAnim, true);
				carrierSkeleton.gameObject.SetActive(true);
			}, null, false);
		}
	}

	// Token: 0x060036F2 RID: 14066 RVA: 0x001BA0D4 File Offset: 0x001B82D4
	private void PlayCarrierAnimOther(SkeletonGraphic skeletonGraphic, string animName, bool isMove)
	{
		skeletonGraphic.timeScale = (float)(isMove ? 1 : 0);
		Spine.AnimationState state = skeletonGraphic.AnimationState;
		TrackEntry current = state.GetCurrent(0);
		bool flag = current == null || current.Animation.Name != animName;
		if (flag)
		{
			state.SetAnimation(0, animName, true);
		}
	}

	// Token: 0x060036F3 RID: 14067 RVA: 0x001BA124 File Offset: 0x001B8324
	private void PlayCarrierSound()
	{
		string travelSound = this.TravelSkeleton.Sound;
		bool flag = !string.IsNullOrEmpty(travelSound);
		if (flag)
		{
			AudioManager.Instance.PlaySound(travelSound, false, false);
		}
	}

	// Token: 0x060036F4 RID: 14068 RVA: 0x001BA15C File Offset: 0x001B835C
	private void TravelTweenCallback(CrossAreaMoveInfo moveInfo)
	{
		UI_PartWorldMap.<>c__DisplayClass117_0 CS$<>8__locals1 = new UI_PartWorldMap.<>c__DisplayClass117_0();
		CS$<>8__locals1.<>4__this = this;
		Refers root = base.CGet<RectTransform>("ComplexTravel").GetComponent<Refers>();
		List<short> areaList = moveInfo.Route.AreaList;
		int currIndex = moveInfo.RouteIndex;
		Refers route = root.CGet<Refers>("TravelRoute");
		float duration = this.StepAnimationDurationWithoutLimit;
		CS$<>8__locals1.routeTransform = route.GetComponent<RectTransform>();
		CS$<>8__locals1.stable = route.CGet<CImage>("Stable");
		CS$<>8__locals1.progressBack = route.CGet<CImage>("ProgressBack");
		CS$<>8__locals1.progress = route.CGet<CImage>("Progress");
		Refers prev = route.CGet<Refers>("Prev");
		Refers current = route.CGet<Refers>("Current");
		Refers next = route.CGet<Refers>("Next");
		Refers nextNext = route.CGet<Refers>("NextNext");
		prev.gameObject.SetActive(true);
		current.gameObject.SetActive(true);
		next.gameObject.SetActive(true);
		nextNext.gameObject.SetActive(true);
		CS$<>8__locals1.stable.enabled = true;
		bool flag = currIndex == moveInfo.Route.AreaList.Count - 1;
		if (flag)
		{
			UI_PartWorldMap.<>c__DisplayClass117_1 CS$<>8__locals2 = new UI_PartWorldMap.<>c__DisplayClass117_1();
			CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			CS$<>8__locals2.startRouteAngle = 15f;
			CS$<>8__locals2.startProgressBackAngle = 30f;
			CS$<>8__locals2.startProgressAngle = 30f;
			CS$<>8__locals2.startProgressBackFill = 0.125f;
			CS$<>8__locals2.startProgressFill = 0.085f;
			CS$<>8__locals2.targetRouteAngle = 30f;
			CS$<>8__locals2.targetProgressBackAngle = 30f;
			CS$<>8__locals2.targetProgressAngle = 30f;
			CS$<>8__locals2.targetProgressBackFill = 0.085f;
			CS$<>8__locals2.targetProgressFill = 0.085f;
			CS$<>8__locals2.CS$<>8__locals1.stable.enabled = false;
			CS$<>8__locals2.CS$<>8__locals1.<TravelTweenCallback>g__SetAreaName|0(next, moveInfo.LastAreaId, false);
			CS$<>8__locals2.CS$<>8__locals1.<TravelTweenCallback>g__SetAreaName|0(nextNext, moveInfo.CurrentAreaId, true);
			CS$<>8__locals2.<TravelTweenCallback>g__Step|1(0f);
			DOVirtual.Float(0f, 1f, duration, new TweenCallback<float>(CS$<>8__locals2.<TravelTweenCallback>g__Step|1)).OnComplete(delegate
			{
				CS$<>8__locals2.CS$<>8__locals1.stable.enabled = true;
				CS$<>8__locals2.CS$<>8__locals1.<>4__this.ProcessCachedGEvents();
			}).SetAutoKill(true);
		}
		else
		{
			bool flag2 = moveInfo.CurrentAreaId == moveInfo.LastAreaId;
			if (flag2)
			{
				CS$<>8__locals1.routeTransform.rotation = (CS$<>8__locals1.progressBack.rectTransform.rotation = (CS$<>8__locals1.progress.rectTransform.rotation = Quaternion.Euler(0f, 0f, 0f)));
				CS$<>8__locals1.progressBack.fillAmount = 0.1675f;
				CS$<>8__locals1.progress.fillAmount = 0f;
				CS$<>8__locals1.stable.enabled = true;
				prev.gameObject.SetActive(false);
				CS$<>8__locals1.<TravelTweenCallback>g__SetAreaName|0(current, moveInfo.CurrentAreaId, true);
				CS$<>8__locals1.<TravelTweenCallback>g__SetAreaName|0(next, moveInfo.NextAreaId, false);
			}
			else
			{
				UI_PartWorldMap.<>c__DisplayClass117_2 CS$<>8__locals3 = new UI_PartWorldMap.<>c__DisplayClass117_2();
				CS$<>8__locals3.CS$<>8__locals2 = CS$<>8__locals1;
				CS$<>8__locals3.startRouteAngle = 0;
				CS$<>8__locals3.startProgressBackAngle = 30f;
				CS$<>8__locals3.startProgressAngle = 30f;
				CS$<>8__locals3.startProgressBackFill = 0.1675f;
				CS$<>8__locals3.startProgressFill = 0.085f;
				CS$<>8__locals3.targetRouteAngle = 15f;
				CS$<>8__locals3.targetProgressBackAngle = 30f;
				CS$<>8__locals3.targetProgressAngle = 30f;
				CS$<>8__locals3.targetProgressBackFill = 0.1675f;
				CS$<>8__locals3.targetProgressFill = 0.085f;
				int prevDisplayIndex = moveInfo.RouteIndex - 2;
				bool flag3 = currIndex == 0;
				if (flag3)
				{
					CS$<>8__locals3.startProgressBackAngle = 0f;
					CS$<>8__locals3.startProgressAngle = 0f;
					CS$<>8__locals3.startProgressFill = 0f;
					CS$<>8__locals3.targetProgressBackAngle = 15f;
					CS$<>8__locals3.targetProgressAngle = 15f;
					CS$<>8__locals3.targetProgressFill = 0.0425f;
				}
				else
				{
					bool flag4 = currIndex == 1;
					if (flag4)
					{
						prevDisplayIndex = moveInfo.RouteIndex - 1;
						CS$<>8__locals3.startProgressBackAngle = 15f;
						CS$<>8__locals3.startProgressAngle = 15f;
						CS$<>8__locals3.startProgressFill = 0.0425f;
					}
					else
					{
						bool flag5 = currIndex == moveInfo.Route.AreaList.Count - 2;
						if (flag5)
						{
							CS$<>8__locals3.targetProgressBackFill = 0.125f;
						}
					}
				}
				CS$<>8__locals3.CS$<>8__locals2.stable.enabled = false;
				prev.gameObject.SetActive(moveInfo.Route.AreaList.CheckIndex(prevDisplayIndex));
				bool activeSelf = prev.gameObject.activeSelf;
				if (activeSelf)
				{
					CS$<>8__locals3.CS$<>8__locals2.<TravelTweenCallback>g__SetAreaName|0(prev, moveInfo.Route.AreaList[prevDisplayIndex], false);
				}
				CS$<>8__locals3.CS$<>8__locals2.<TravelTweenCallback>g__SetAreaName|0(current, moveInfo.LastAreaId, false);
				CS$<>8__locals3.CS$<>8__locals2.<TravelTweenCallback>g__SetAreaName|0(next, moveInfo.CurrentAreaId, true);
				CS$<>8__locals3.CS$<>8__locals2.<TravelTweenCallback>g__SetAreaName|0(nextNext, moveInfo.NextAreaId, false);
				CS$<>8__locals3.<TravelTweenCallback>g__Step|3(0f);
				DOVirtual.Float(0f, 1f, duration, new TweenCallback<float>(CS$<>8__locals3.<TravelTweenCallback>g__Step|3)).OnComplete(delegate
				{
					CS$<>8__locals3.CS$<>8__locals2.stable.enabled = true;
				}).SetAutoKill(true);
			}
		}
	}

	// Token: 0x060036F5 RID: 14069 RVA: 0x001BA6AC File Offset: 0x001B88AC
	private void SetTravelButtonsVisibility(bool isTravelDoing)
	{
		base.CGet<UISwitcher>("DirectReturn").gameObject.SetActive(!isTravelDoing);
		base.CGet<CButtonObsolete>("CloseBtn").gameObject.SetActive(!isTravelDoing);
		base.CGet<UISwitcher>("StopTravel").gameObject.SetActive(isTravelDoing);
		base.CGet<CToggleObsolete>("PauseToggle").gameObject.SetActive(isTravelDoing);
	}

	// Token: 0x060036F6 RID: 14070 RVA: 0x001BA71C File Offset: 0x001B891C
	private void OnGainsRender(int index, Refers refers)
	{
		ItemKey unit = this._gainsInTravel[index];
		string itemName = ItemUtils.GetItemColorName(unit.ItemType, unit.TemplateId);
		string itemIcon = ItemTemplateHelper.GetIcon(unit.ItemType, unit.TemplateId);
		refers.CGet<CImage>("Icon").SetSprite(itemIcon, false, null);
		refers.CGet<TextMeshProUGUI>("Name").text = itemName;
		refers.CGet<TextMeshProUGUI>("Value").text = "+1";
	}

	// Token: 0x060036F7 RID: 14071 RVA: 0x001BA798 File Offset: 0x001B8998
	private void UpdatePlayerPos(short areaId = -1, bool isTween = false)
	{
		bool flag = areaId < 0;
		if (flag)
		{
			areaId = this._mapModel.CurrentAreaId;
		}
		MapAreaItem areaConfig = this._mapModel.Areas[(int)areaId].GetConfig();
		RectTransform bottom = this._partWorldView.CurrPosBottom;
		RectTransform pointer = this._partWorldView.CurrPosPointer;
		bool flag2 = areaConfig.StateID <= 0;
		if (flag2)
		{
			bottom.gameObject.SetActive(false);
			pointer.gameObject.SetActive(false);
		}
		else if (isTween)
		{
			float duration = this.StepAnimationDurationWithoutLimit;
			bottom.DOKill(false);
			bottom.DOAnchorPos(this._partWorldView.GetAnchoredPos((int)areaConfig.WorldMapPos[0], (int)areaConfig.WorldMapPos[1]) + new Vector2(2222f, 1083f), duration, false);
			bottom.gameObject.SetActive(true);
			pointer.DOKill(false);
			pointer.DOAnchorPos(this._partWorldView.GetAnchoredPos((int)areaConfig.WorldMapPos[0], (int)areaConfig.WorldMapPos[1]) + new Vector2(2222f, 1175f), duration, false);
			pointer.gameObject.SetActive(true);
		}
		else
		{
			bottom.anchoredPosition = this._partWorldView.GetAnchoredPos((int)areaConfig.WorldMapPos[0], (int)areaConfig.WorldMapPos[1]) + new Vector2(2222f, 1083f);
			bottom.gameObject.SetActive(true);
			pointer.anchoredPosition = this._partWorldView.GetAnchoredPos((int)areaConfig.WorldMapPos[0], (int)areaConfig.WorldMapPos[1]) + new Vector2(2222f, 1175f);
			pointer.gameObject.SetActive(true);
		}
	}

	// Token: 0x060036F8 RID: 14072 RVA: 0x001BA958 File Offset: 0x001B8B58
	private void FocusArea(short areaId, float tweenDuration = 0.1f)
	{
		bool flag = this._partWorldView.AreaHolder.childCount <= (int)areaId || areaId < 0;
		if (!flag)
		{
			Vector2 areaMapPos = this._partWorldView.AreaHolder.GetChild((int)areaId).GetComponent<RectTransform>().anchoredPosition - this._partWorldView.AreaHolder.sizeDelta / 2f;
			RectTransform rectTrans = this._partWorldView.ScaleAndMoveRoot.GetComponent<RectTransform>();
			Rect parentRect = rectTrans.parent.GetComponent<RectTransform>().rect;
			float rectWidth = rectTrans.sizeDelta.x * rectTrans.localScale.x;
			float rectHeight = rectTrans.sizeDelta.y * rectTrans.localScale.y;
			float posX = -areaMapPos.x * rectTrans.localScale.x + rectWidth * (rectTrans.pivot.x - 0.5f);
			float posY = -areaMapPos.y * rectTrans.localScale.y + rectHeight * (rectTrans.pivot.y - 0.5f);
			float minX = -(rectWidth - parentRect.width) / 2f + rectWidth * (rectTrans.pivot.x - 0.5f);
			float maxX = minX + (rectWidth - parentRect.width);
			float minY = -(rectHeight - parentRect.height) / 2f + rectHeight * (rectTrans.pivot.y - 0.5f);
			float maxY = minY + (rectHeight - parentRect.height);
			this._partWorldView.ScaleAndMoveRoot.GetComponent<RectTransform>().DOAnchorPos(new Vector2(Mathf.Clamp(posX, minX, maxX), Mathf.Clamp(posY, minY, maxY)), tweenDuration, false);
		}
	}

	// Token: 0x060036F9 RID: 14073 RVA: 0x001BAB0C File Offset: 0x001B8D0C
	private void ShowTravelInfo(CrossAreaMoveInfo moveInfo)
	{
		bool isSimple = false;
		RectTransform animationNode = base.CGet<RectTransform>("TravelAnimation");
		animationNode.SetParent(base.CGet<RectTransform>(isSimple ? "SimpleAnimationRoot" : "ComplexAnimationRoot"));
		animationNode.localPosition = Vector3.zero;
		animationNode.localScale = Vector3.one * 0.1f;
		base.CGet<RectTransform>(isSimple ? "SimpleTravel" : "ComplexTravel").gameObject.SetActive(true);
		this.TravelTweenCallback(moveInfo);
	}

	// Token: 0x060036FA RID: 14074 RVA: 0x001BAB8F File Offset: 0x001B8D8F
	private void InitAreas()
	{
		this._partWorldView.InitAreas();
	}

	// Token: 0x060036FB RID: 14075 RVA: 0x001BABA0 File Offset: 0x001B8DA0
	private void UpdateAreaRoute()
	{
		RectTransform routeHolder = base.CGet<RectTransform>("RouteHolder");
		Dictionary<ValueTuple<ByteCoordinate, ByteCoordinate>, GameObject> canPassRouteDict = EasyPool.Get<Dictionary<ValueTuple<ByteCoordinate, ByteCoordinate>, GameObject>>();
		Dictionary<ValueTuple<ByteCoordinate, ByteCoordinate>, GameObject> canUnlockRouteDict = EasyPool.Get<Dictionary<ValueTuple<ByteCoordinate, ByteCoordinate>, GameObject>>();
		List<ByteCoordinate> posList = EasyPool.Get<List<ByteCoordinate>>();
		for (int i = routeHolder.childCount - 1; i >= 2; i--)
		{
			GameObject routeObj = routeHolder.GetChild(i).gameObject;
			PoolManager.Destroy((routeObj.name == "CanPassRoutePrefab") ? "BigMap_CanPassRoutePrefab" : "BigMap_CanUnlockRoutePrefab", routeObj);
		}
		for (short areaId = 0; areaId < 135; areaId += 1)
		{
			MapAreaData areaData = this._mapModel.Areas[(int)areaId];
			bool flag = !areaData.StationUnlocked || (!this.IsWorldTravelUnlocked && this._mapModel.CurrentStateId >= 0 && this._mapModel.GetStateId(areaId) != this._mapModel.CurrentStateId);
			if (!flag)
			{
				HashSet<short> neighborAreas = areaData.NeighborAreas;
				foreach (short neighborAreaId in neighborAreas)
				{
					bool flag2 = !this.IsWorldTravelUnlocked && this._mapModel.CurrentStateId >= 0 && this._mapModel.GetStateId(neighborAreaId) != this._mapModel.CurrentStateId;
					if (!flag2)
					{
						bool stationUnlocked = this._mapModel.Areas[(int)neighborAreaId].StationUnlocked;
						sbyte[] worldPos = areaData.GetConfig().WorldMapPos;
						sbyte[] neighborWorldPos = this._mapModel.Areas[(int)neighborAreaId].GetConfig().WorldMapPos;
						TravelRouteKey routeKey = new TravelRouteKey(areaId, neighborAreaId);
						bool reverse = routeKey.FromAreaId > routeKey.ToAreaId;
						bool flag3 = reverse;
						if (flag3)
						{
							routeKey.Reverse();
						}
						posList.Clear();
						posList.Add(new ByteCoordinate((byte)((!reverse) ? worldPos : neighborWorldPos)[0], (byte)((!reverse) ? worldPos : neighborWorldPos)[1]));
						posList.AddRange(this._travelRoutes[routeKey].PosList);
						posList.Add(new ByteCoordinate((byte)((!reverse) ? neighborWorldPos : worldPos)[0], (byte)((!reverse) ? neighborWorldPos : worldPos)[1]));
						for (int j = 0; j < posList.Count - 1; j++)
						{
							ByteCoordinate fromPos = posList[j];
							ByteCoordinate toPos = posList[j + 1];
							bool flag4 = fromPos.X > toPos.X || (fromPos.X == toPos.X && fromPos.Y > toPos.Y);
							if (flag4)
							{
								ByteCoordinate byteCoordinate = toPos;
								ByteCoordinate byteCoordinate2 = fromPos;
								fromPos = byteCoordinate;
								toPos = byteCoordinate2;
							}
							ValueTuple<ByteCoordinate, ByteCoordinate> key = new ValueTuple<ByteCoordinate, ByteCoordinate>(fromPos, toPos);
							bool flag5 = canPassRouteDict.ContainsKey(key) || (canUnlockRouteDict.ContainsKey(key) && !stationUnlocked);
							if (!flag5)
							{
								RectTransform routeTransform = PoolManager.GetObject<RectTransform>(stationUnlocked ? "BigMap_CanPassRoutePrefab" : "BigMap_CanUnlockRoutePrefab");
								float rotate = (float)((fromPos.X == toPos.X) ? ((fromPos.Y < toPos.Y) ? 90 : -90) : ((fromPos.X < toPos.X) ? 0 : 180));
								bool flag6 = stationUnlocked && canUnlockRouteDict.ContainsKey(key);
								if (flag6)
								{
									GameObject routeObj2 = canUnlockRouteDict[key];
									PoolManager.Destroy((routeObj2.name == "CanPassRoutePrefab") ? "BigMap_CanPassRoutePrefab" : "BigMap_CanUnlockRoutePrefab", routeObj2);
									canUnlockRouteDict.Remove(key);
								}
								routeTransform.GetComponent<CImage>().SetSprite(string.Format("largemap_part_1_road_{0}", ((int)areaId + j) % 3), false, null);
								routeTransform.SetParent(routeHolder, false);
								routeTransform.localScale = Vector3.one;
								routeTransform.anchoredPosition = this._partWorldView.GetAnchoredPos((int)fromPos.X, (int)fromPos.Y);
								routeTransform.localEulerAngles = Vector3.one.SetZ(rotate);
								routeTransform.sizeDelta = new Vector2(111f, 34f);
								routeTransform.gameObject.SetActive(true);
								(stationUnlocked ? canPassRouteDict : canUnlockRouteDict).Add(key, routeTransform.gameObject);
							}
						}
					}
				}
			}
		}
		routeHolder.gameObject.SetActive(false);
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
		{
			routeHolder.gameObject.SetActive(true);
		});
		EasyPool.Free<Dictionary<ValueTuple<ByteCoordinate, ByteCoordinate>, GameObject>>(canPassRouteDict);
		EasyPool.Free<Dictionary<ValueTuple<ByteCoordinate, ByteCoordinate>, GameObject>>(canUnlockRouteDict);
		EasyPool.Free<List<ByteCoordinate>>(posList);
	}

	// Token: 0x060036FC RID: 14076 RVA: 0x001BB084 File Offset: 0x001B9284
	private void UpdateAreaInfo()
	{
		for (short areaId = 0; areaId < 135; areaId += 1)
		{
			MapAreaData areaData = this._mapModel.Areas[(int)areaId];
			MapAreaItem areaConfig = this._mapModel.Areas[(int)areaId].GetConfig();
			AreaDisplayData displayData = this._areaDisplayData[(int)areaId];
			Refers areaRefers = this._partWorldView.AreaHolder.GetChild((int)areaId).GetComponent<Refers>();
			CrossAreaMoveInfo newMoveInfo = this._newMoveInfo;
			bool inTravel = newMoveInfo != null && newMoveInfo.Traveling;
			CrossAreaMoveInfo newMoveInfo2 = this._newMoveInfo;
			bool? flag;
			if (newMoveInfo2 == null)
			{
				flag = null;
			}
			else
			{
				TravelRoute route = newMoveInfo2.Route;
				if (route == null)
				{
					flag = null;
				}
				else
				{
					List<short> areaList = route.AreaList;
					flag = ((areaList != null) ? new bool?(areaList.Contains(areaId)) : null);
				}
			}
			bool? flag2 = flag;
			bool inRoute = flag2.GetValueOrDefault();
			bool stationUnlocked = areaData.StationUnlocked || (inTravel && inRoute);
			areaRefers.gameObject.SetActive(this.IsWorldTravelUnlocked || this._mapModel.CurrentStateId < 0 || this._mapModel.GetStateId(areaId) == this._mapModel.CurrentStateId);
			areaRefers.GetComponent<CButtonObsolete>().interactable = true;
			areaRefers.CGet<CImage>("AreaIcon").SetSprite(areaConfig.BigMapIcon, false, null);
			areaRefers.CGet<AreaStatus>("AreaStatus").SetStatus(displayData);
			areaRefers.CGet<AreaName>("AreaName").SetBroken(displayData.IsBroken);
			areaRefers.CGet<AreaName>("AreaName").ShowFiveLoongIcon(displayData.AnyLoong);
			areaRefers.CGet<AreaBroken>("AreaBroken").SetBroken(displayData.IsBroken, displayData.BrokenLevel);
			areaRefers.CGet<DisableStyleRoot>("DisableStyleRoot").SetStyleEffect(stationUnlocked, false);
			areaRefers.CGet<DisableStyleRoot>("DisableStyleRoot").SetStyleEffect(!stationUnlocked, false);
			TooltipInvoker tip = areaRefers.GetComponent<TooltipInvoker>();
			TooltipInvoker tooltipInvoker = tip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			tip.RuntimeParam.Clear();
			tip.RuntimeParam.Set("areaId", areaId);
			tip.RuntimeParam.SetObject("displayData", displayData);
			bool flag3 = SingletonObject.getInstance<DlcManager>().IsDlcInstalled(DlcManager.DlcIdFiveLoong);
			if (flag3)
			{
				AreaEffect areaEffect = areaRefers.GetComponentInChildren<AreaEffect>(true);
				bool flag4 = areaEffect == null && displayData.AnyLoong;
				if (flag4)
				{
					areaEffect = PoolManager.GetObject<AreaEffect>("UI_PartWorldMap_BigMap_AreaEffect");
					RectTransform rectTrans = areaEffect.transform as RectTransform;
					rectTrans.SetParent(areaRefers.transform);
					rectTrans.SetAsFirstSibling();
					rectTrans.localPosition = Vector3.zero;
					rectTrans.anchoredPosition = Vector2.zero;
					areaEffect.Refresh(displayData.LoongStatus);
				}
				else
				{
					bool flag5 = null != areaEffect && !displayData.AnyLoong;
					if (flag5)
					{
						areaEffect.transform.SetParent(null);
						PoolManager.Destroy("UI_PartWorldMap_BigMap_AreaEffect", areaEffect.gameObject);
					}
				}
			}
		}
		base.GetComponent<CanvasGroup>().alpha = 1f;
		this.Element.ShowAfterRefresh();
	}

	// Token: 0x060036FD RID: 14077 RVA: 0x001BB3BC File Offset: 0x001B95BC
	private void UpdateAreaTravelInfo(CrossAreaMoveInfo moveInfo)
	{
		bool flag = this._areaDisplayData == null;
		if (!flag)
		{
			this.UpdateAreaInfo();
		}
	}

	// Token: 0x060036FE RID: 14078 RVA: 0x001BB3E0 File Offset: 0x001B95E0
	private void SetAreaStatusActive(bool inActive)
	{
		this._isAreaStatusActive = inActive;
		for (short areaId = 0; areaId < 135; areaId += 1)
		{
			Refers areaRefers = this._partWorldView.AreaHolder.GetChild((int)areaId).GetComponent<Refers>();
			areaRefers.CGet<AreaStatus>("AreaStatus").gameObject.SetActive(inActive);
		}
	}

	// Token: 0x060036FF RID: 14079 RVA: 0x001BB43C File Offset: 0x001B963C
	private void UpdateTravelPath(CrossAreaMoveInfo travelInfo)
	{
		List<ByteCoordinate> posList = travelInfo.Route.PosList;
		short currAreaId = travelInfo.CurrentAreaId;
		this._travelPathVertices.Clear();
		ByteCoordinate currAreaPos = this.<UpdateTravelPath>g__ConvertPos|128_1(currAreaId);
		bool flag = currAreaId == travelInfo.FromAreaId;
		int startIndex;
		if (flag)
		{
			this.<UpdateTravelPath>g__InsertPos|128_0(currAreaPos);
			startIndex = ((posList.Count > 0) ? 0 : -1);
		}
		else
		{
			startIndex = posList.IndexOf(currAreaPos);
		}
		bool flag2 = startIndex >= 0;
		if (flag2)
		{
			for (int i = startIndex; i < posList.Count; i++)
			{
				this.<UpdateTravelPath>g__InsertPos|128_0(posList[i]);
			}
		}
		ByteCoordinate toAreaPos = this.<UpdateTravelPath>g__ConvertPos|128_1(travelInfo.ToAreaId);
		this.<UpdateTravelPath>g__InsertPos|128_0(toAreaPos);
		bool traveling = travelInfo.Traveling;
		if (traveling)
		{
			this.UpdatePlayerPos(currAreaId, true);
			AudioManager.Instance.PlaySound(base.CGet<AudioClip>("SeCursor").name, false, false);
		}
		this.TravelPathGraphic.SetVerticesDirty();
		sbyte currStateId = this._mapModel.GetStateId(currAreaId);
		string bgmName = (currStateId >= 0 && currAreaId != this._mapModel.GetTaiwuVillageBlock().AreaId) ? MapState.Instance[(int)(currStateId + 1)].Bgm : "main_fushixun";
		MusicPlayerModel musicPlayerModel = SingletonObject.getInstance<MusicPlayerModel>();
		bool flag3 = AudioManager.Instance.GetPlayingMusic() != bgmName && musicPlayerModel.IsEnabled && !musicPlayerModel.IsPlaying;
		if (flag3)
		{
			AudioManager.Instance.PlayMusic(bgmName, 3f, 100, null);
		}
	}

	// Token: 0x06003700 RID: 14080 RVA: 0x001BB5C2 File Offset: 0x001B97C2
	private void ClearTravelPath()
	{
		this._travelPathVertices.Clear();
		this.TravelPathGraphic.SetVerticesDirty();
	}

	// Token: 0x06003701 RID: 14081 RVA: 0x001BB5E0 File Offset: 0x001B97E0
	private void ProcessCachedGEvents()
	{
		foreach (ValueTuple<Enum, ArgumentBox> eventPair in this.GEventsOnExit)
		{
			GEvent.OnEvent(eventPair.Item1, eventPair.Item2);
		}
		this.GEventsOnExit.Clear();
	}

	// Token: 0x06003702 RID: 14082 RVA: 0x001BB650 File Offset: 0x001B9850
	public void Preload()
	{
		base.CGet<SkeletonGraphic>("CharacterSkeleton").Initialize(false);
		base.CGet<SkeletonGraphic>("SubCharacterSkeleton").Initialize(false);
	}

	// Token: 0x06003703 RID: 14083 RVA: 0x001BB678 File Offset: 0x001B9878
	public void FinishAndHideSelf()
	{
		UIElement element = this.Element;
		element.OnHide = (Action)Delegate.Combine(element.OnHide, new Action(this.OnHide));
		GEvent.ClearEvent(UiEvents.OnTravelCheckPointProcessed);
		GEvent.OnEvent(UiEvents.OnTravelFinish, null);
		this.HideSelf();
		this._isTravelAutoPaused = false;
	}

	// Token: 0x06003704 RID: 14084 RVA: 0x001BB6D6 File Offset: 0x001B98D6
	public void HideSelf()
	{
		CommandKitBase.SetDisable(false);
		UIManager.Instance.HideUI(UIElement.StatePartWorldMap);
	}

	// Token: 0x06003705 RID: 14085 RVA: 0x001BB6F0 File Offset: 0x001B98F0
	public void OnMouseEnterState(int stateId)
	{
		bool flag = (int)this._currSelectedStateId == stateId || !UIManager.Instance.IsFocusElement(this.Element);
		if (!flag)
		{
			this._currSelectedStateId = (sbyte)stateId;
			Transform stateMask = this._partWorldView.StateMaskHolder.GetChild(stateId);
			bool enabled = stateMask.GetComponent<IrregularClickableImage>().enabled;
			if (enabled)
			{
				stateMask.GetChild(0).gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06003706 RID: 14086 RVA: 0x001BB760 File Offset: 0x001B9960
	public void OnMouseExitState(int stateId)
	{
		bool flag = this._currSelectedStateId < 0 || (int)this._currSelectedStateId != stateId || !UIManager.Instance.IsFocusElement(this.Element);
		if (!flag)
		{
			this._currSelectedStateId = -1;
			this._partWorldView.StateMaskHolder.GetChild(stateId).GetChild(0).gameObject.SetActive(false);
		}
	}

	// Token: 0x06003707 RID: 14087 RVA: 0x001BB7C8 File Offset: 0x001B99C8
	public void ClearMouseInState()
	{
		foreach (object obj in this._partWorldView.StateMaskHolder)
		{
			Transform state = (Transform)obj;
			state.GetChild(0).gameObject.SetActive(false);
		}
		this._currSelectedStateId = -1;
	}

	// Token: 0x06003708 RID: 14088 RVA: 0x001BB840 File Offset: 0x001B9A40
	public void ClearGainsGauge()
	{
		this._gainsScroll.UpdateData(0);
	}

	// Token: 0x0600370F RID: 14095 RVA: 0x001BB944 File Offset: 0x001B9B44
	[CompilerGenerated]
	private void <UpdateTravelPath>g__InsertPos|128_0(ByteCoordinate pos)
	{
		Vector2 anchoredPos = this._partWorldView.GetAnchoredPos((int)pos.X, (int)pos.Y);
		anchoredPos += this._partWorldView.PathHolder.offsetMin;
		this._travelPathVertices.Add(anchoredPos);
	}

	// Token: 0x06003710 RID: 14096 RVA: 0x001BB990 File Offset: 0x001B9B90
	[CompilerGenerated]
	private ByteCoordinate <UpdateTravelPath>g__ConvertPos|128_1(short areaId)
	{
		IReadOnlyList<sbyte> configPos = this._mapModel.Areas[(int)areaId].GetConfig().WorldMapPos;
		return new ByteCoordinate((byte)configPos[0], (byte)configPos[1]);
	}

	// Token: 0x040027A6 RID: 10150
	private const float TravelProgressAniSpeed = 0.15f;

	// Token: 0x040027A7 RID: 10151
	private const string CanPassRoutePrefabKey = "BigMap_CanPassRoutePrefab";

	// Token: 0x040027A8 RID: 10152
	private const string CanUnlockRoutePrefabKey = "BigMap_CanUnlockRoutePrefab";

	// Token: 0x040027A9 RID: 10153
	private const string AreaEffectPrefabKey = "UI_PartWorldMap_BigMap_AreaEffect";

	// Token: 0x040027AA RID: 10154
	public static readonly string[] BrokenIcons = new string[]
	{
		"largemap_part_2_icon_posui_0",
		"largemap_part_2_icon_posui_1",
		"largemap_part_2_icon_posui_2",
		"largemap_part_2_icon_posui_3"
	};

	// Token: 0x040027AB RID: 10155
	private WorldMapModel _mapModel;

	// Token: 0x040027AC RID: 10156
	private bool _isDirectTraveling;

	// Token: 0x040027AD RID: 10157
	private KidnappedTravelData _kidnappedTravelData;

	// Token: 0x040027AE RID: 10158
	private CrossAreaMoveInfo _moveInfo;

	// Token: 0x040027AF RID: 10159
	private CrossAreaMoveInfo _newMoveInfo;

	// Token: 0x040027B0 RID: 10160
	private CharacterDisplayData _taiwuDisplayData;

	// Token: 0x040027B1 RID: 10161
	private List<ItemKey> _gainsInTravel;

	// Token: 0x040027B2 RID: 10162
	private readonly Dictionary<TravelRouteKey, TravelRoute> _travelRoutes = new Dictionary<TravelRouteKey, TravelRoute>();

	// Token: 0x040027B3 RID: 10163
	private readonly Dictionary<short, bool> _taiwuVisitedAreas = new Dictionary<short, bool>();

	// Token: 0x040027B4 RID: 10164
	private AreaDisplayData[] _areaDisplayData;

	// Token: 0x040027B5 RID: 10165
	private CrossAreaMoveInfo _currSelectedAreaMoveInfo;

	// Token: 0x040027B6 RID: 10166
	[NonSerialized]
	public readonly List<ValueTuple<Enum, ArgumentBox>> GEventsOnExit = new List<ValueTuple<Enum, ArgumentBox>>();

	// Token: 0x040027B7 RID: 10167
	private bool _travelIsFinish;

	// Token: 0x040027B8 RID: 10168
	private float _continueWaitingSeconds;

	// Token: 0x040027B9 RID: 10169
	private short _travelDestAreaId;

	// Token: 0x040027BA RID: 10170
	private int _nextAreaCostDays;

	// Token: 0x040027BB RID: 10171
	private sbyte _currSelectedStateId = -1;

	// Token: 0x040027BC RID: 10172
	private short _currSelectedAreaId = -1;

	// Token: 0x040027BD RID: 10173
	private PartWorldView _partWorldView;

	// Token: 0x040027BE RID: 10174
	private InfinityScrollLegacy _gainsScroll;

	// Token: 0x040027BF RID: 10175
	private RectTransform _focusTaiwuVillage;

	// Token: 0x040027C0 RID: 10176
	private Vector2 _taiwuVillagePos;

	// Token: 0x040027C1 RID: 10177
	private bool _isTravelAutoPaused;

	// Token: 0x040027C2 RID: 10178
	private bool _isQuickHiding;

	// Token: 0x040027C3 RID: 10179
	private bool _isAreaStatusActive;

	// Token: 0x040027C4 RID: 10180
	private bool? _travelAnimLastIdleState;

	// Token: 0x040027C5 RID: 10181
	private readonly List<Vector2> _travelPathVertices = new List<Vector2>();

	// Token: 0x040027C6 RID: 10182
	private bool _disableCheckClose;
}
