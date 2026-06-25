using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using EasyButtons;
using FrameWork;
using FrameWork.CommandSystem;
using FrameWork.UISystem.Components;
using Game.Views.CharacterMenu;
using Game.Views.Legacy.WorldMap;
using Game.Views.MapBlockCharList;
using Game.Views.Migrate;
using Game.Views.Select;
using GameData.Combat.Math;
using GameData.DLC.FiveLoong;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.SortFilter;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.Map;
using GameData.Domains.Merchant;
using GameData.Domains.Story;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Profession;
using GameData.Domains.Taiwu.Profession.SkillsData;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Map.RenderSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views.Map
{
	// Token: 0x02000945 RID: 2373
	public class ViewWorldMap : UIBase, IMapRoot
	{
		// Token: 0x17000CCA RID: 3274
		// (get) Token: 0x06006EB2 RID: 28338 RVA: 0x00333256 File Offset: 0x00331456
		public MapBlockData PlayerAtBlock
		{
			get
			{
				return this._mapModel.PlayerAtBlock;
			}
		}

		// Token: 0x17000CCB RID: 3275
		// (get) Token: 0x06006EB3 RID: 28339 RVA: 0x00333263 File Offset: 0x00331463
		public MapBlockData SelectedBlock
		{
			get
			{
				return this._mapModel.SelectedBlock;
			}
		}

		// Token: 0x17000CCC RID: 3276
		// (get) Token: 0x06006EB4 RID: 28340 RVA: 0x00333270 File Offset: 0x00331470
		public bool IsDoingMove
		{
			get
			{
				return this._doingMove;
			}
		}

		// Token: 0x17000CCD RID: 3277
		// (get) Token: 0x06006EB5 RID: 28341 RVA: 0x00333278 File Offset: 0x00331478
		public bool IsMoving
		{
			get
			{
				return this._pathMoving || this._teleportMoving;
			}
		}

		// Token: 0x17000CCE RID: 3278
		// (get) Token: 0x06006EB6 RID: 28342 RVA: 0x0033328C File Offset: 0x0033148C
		private bool CatchCricketShouldActive
		{
			get
			{
				return this._mapModel.CurrentLocationHasCricket() && this._mapModel.ViewMode != WorldMapModel.EViewMode.Info && this._mapModel.ShowingAreaId == this._mapModel.CurrentAreaId && SingletonObject.getInstance<GlobalSettings>().GetMapElementDisplayRuleItemState(36, true);
			}
		}

		// Token: 0x17000CCF RID: 3279
		// (get) Token: 0x06006EB7 RID: 28343 RVA: 0x003332DC File Offset: 0x003314DC
		private bool IsDivineFlameActive
		{
			get
			{
				return this._xiangshuAvatarId >= 0;
			}
		}

		// Token: 0x17000CD0 RID: 3280
		// (get) Token: 0x06006EB8 RID: 28344 RVA: 0x003332EA File Offset: 0x003314EA
		// (set) Token: 0x06006EB9 RID: 28345 RVA: 0x003332F1 File Offset: 0x003314F1
		public static bool InAdventureRemake { get; set; }

		// Token: 0x17000CD1 RID: 3281
		// (get) Token: 0x06006EBA RID: 28346 RVA: 0x003332F9 File Offset: 0x003314F9
		public Vector2 LastCameraMoveTarget
		{
			get
			{
				return this._lastCameraMoveTarget;
			}
		}

		// Token: 0x06006EBB RID: 28347 RVA: 0x00333304 File Offset: 0x00331504
		public override void OnInit(ArgumentBox argsBox)
		{
			this._mapModel = SingletonObject.getInstance<WorldMapModel>();
			this._hotkeyMoveDirection = MoveDirection.None;
			this._hotkeyMoveReady = true;
			this._pathMoving = false;
			ViewWorldMap.SetDisableMoving(false);
			this.UpdateBlockId();
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				bool flag = !this._playerAtBlockInitialized;
				if (flag)
				{
					MapRenderSystem.UpdateAreaOffset();
					GEvent.OnEvent(UiEvents.WorldMapUpdateAreaOffset, null);
					this.InitMap();
					base.StartCoroutine(this.RefreshMapBlock());
					base.StartCoroutine(this.RefreshMapUi(true));
					this.RefreshFulongInFlameAreas();
					this.moveRoot.localScale = Vector3.one;
					this.OnMapScaled(1f, false, true);
					this.UpdateBlockId();
					this._playerAtBlockInitialized = true;
				}
				this.Element.ShowAfterRefresh();
			}));
		}

		// Token: 0x06006EBC RID: 28348 RVA: 0x00333370 File Offset: 0x00331570
		private void Awake()
		{
			this._onConfirmInviteCharacter = new Action<int>(this.OnConfirmSelectInviteCharacter);
			this._onCancelInviteCharacter = new Action(this.OnCancelSelectInviteCharacter);
			this._onQuickHideBySystem = new Action(this.OnCancelSelectInviteCharacter);
			this._openCharacterMenu = new Action<int>(this.OpenCharacterMenu);
			this._disableCondition = new Predicate<CharacterDisplayDataForUltimateSelect>(this.CharacterDisableCondition);
			this._player = this.player;
			this._select = this.select;
			this._selectVirtual = this.selectVirtual;
			this.InitializePlayerIcon();
			this.InitializeAllSectStoryEffectContainers();
			PoolManager.SetSrcObject(MapElementContainer.InfoPrefabKey, this.infoPrefab);
			PoolManager.SetSrcObject(MapElementContainer.CostPrefabKey, this.costPrefab);
			PoolManager.SetSrcObject(MapElementContainer.CricketPrefabKey, this.cricketPlacePrefab.gameObject);
			PoolManager.SetSrcObject("ui_WorldmapInfo_TradeCaravanPathNodePrefab", this.caravanPathNodePrefab.gameObject);
			PoolManager.SetSrcObject(MapElementContainer.AdventureRemakePrefabKey, this.adventureRemakePrefab.gameObject);
			PoolManager.SetSrcObject(MapElementContainer.SpecialCharacterPrefabKey, this.characterPrefab);
			PoolManager.SetSrcObject(MapElementContainer.SettlementBtnPrefabKey, this.settlementBtnPrefab);
			PoolManager.SetSrcObject(MapElementContainer.StationBtnPrefabKey, this.stationBtnPrefab);
			PoolManager.SetSrcObject(MapElementContainer.MerchantPrefabKey, this.merchantPrefab);
			PoolManager.SetSrcObject(MapElementContainer.ExpectPromptPrefabKey, this.expectPromptPrefab);
			PoolManager.SetSrcObject(MapElementContainer.TemporaryMarkPrefabKey, this.temporaryMarkPrefab);
			PoolManager.SetSrcObject(MapElementContainer.DreamBackPrefabKey, this.dreamBackPrefab);
			PoolManager.SetSrcObject(MapElementContainer.FulongFlamePrefabKey, this.fulongFlamePrefab);
			PoolManager.SetSrcObject(MapElementContainer.EmeiGuidancePrefabKey, this.emeiGuidancePrefab);
			PoolManager.SetSrcObject(MapElementContainer.ZhujianThiefPrefabKey, this.zhujianThiefPrefab);
			PoolManager.SetSrcObject(MapElementContainer.PickupPrefabKey, this.mapElementPickup);
			PoolManager.SetSrcObject(MapElementContainer.PickupEffectPrefabKey, this.mapElementPickupEffect);
			PoolManager.SetSrcObject(MapElementContainer.CricketWishEffectPrefabKey, this.mapElementCricketWishEffect);
			GEvent.Add(EEvents.OnMonthChange, new GEvent.Callback(this.OnMonthChange));
			GEvent.Add(UiEvents.WorldMapInited, new GEvent.Callback(this.OnMapInited));
			GEvent.Add(UiEvents.AnimalPlaceDataChange, new GEvent.Callback(this.OnAnimalPlaceDataChange));
			GEvent.Add(UiEvents.WorldMapBlockDataChange, new GEvent.Callback(this.OnBlockDataChange));
			GEvent.Add(UiEvents.OnBrokenMaterialDataChange, new GEvent.Callback(this.OnBrokenMaterialDataChange));
			GEvent.Add(UiEvents.OnMapBlockVisibilityNeedRefresh, new GEvent.Callback(this.OnMapBlockVisibilityNeedRefresh));
			GEvent.Add(UiEvents.WorldMapResetMapCamera, new GEvent.Callback(this.OnResetMapCamera));
			GEvent.Add(UiEvents.WorldMapSetCameraToArea, new GEvent.Callback(this.OnSetCameraToArea));
			GEvent.Add(UiEvents.WorldMapSetCameraToLocation, new GEvent.Callback(this.OnSetCameraToLocation));
			GEvent.Add(UiEvents.OnMoveTimeCostPercentChanged, new GEvent.Callback(this.OnMoveTimeCostPercentChanged));
			GEvent.Add(UiEvents.OnVisitedSettlementsChanged, new GEvent.Callback(this.OnVisitedSettlementsChanged));
			GEvent.Add(UiEvents.OnCharacterTaiwuGenderChanged, new GEvent.Callback(this.OnTaiwuGenderChanged));
			GEvent.Add(UiEvents.OnCharacterTaiwuInventoryChanged, new GEvent.Callback(this.OnTaiwuInventoryChanged));
			GEvent.Add(UiEvents.OnCharacterTaiwuCarrierChanged, new GEvent.Callback(this.OnTaiwuCarrierChanged));
			GEvent.Add(UiEvents.OnStateAdventureDataReceived, new GEvent.Callback(this.OnStateAdventureDataReceived));
			GEvent.Add(UiEvents.MapCurrentLocationFixedCharacterDataReceived, new GEvent.Callback(this.MapCurrentLocationFixedCharacterDataReceived));
			GEvent.Add(UiEvents.OnEventHandleComplete, new GEvent.Callback(this.OnEventHandleComplete));
			GEvent.Add(UiEvents.WorldMapPlayerBlockChange, new GEvent.Callback(this.OnPlayerLocationChangedInHide));
			GEvent.Add(UiEvents.CricketPlaceDataChange, new GEvent.Callback(this.OnCricketPlaceDataChange));
			GEvent.Add(UiEvents.OnZhujianThiefDataChanged, new GEvent.Callback(this.OnZhujianThiefDataChanged));
			GEvent.Add(UiEvents.WorldMapVillagerWorkingLocationChange, new GEvent.Callback(this.OnWorldMapVillagerWorkingLocationChange));
			GEvent.Add(UiEvents.OnLocationMarkChange, new GEvent.Callback(this.OnLocationMarkChange));
			GEvent.Add(UiEvents.OnMapBlockMerchantTogChange, new GEvent.Callback(this.OnMapBlockMerchantTogChange));
			GEvent.Add(UiEvents.OnMapAlterSettlementChanged, new GEvent.Callback(this.OnMapAlterSettlementChanged));
			GEvent.Add(UiEvents.OnMapCharacterChanged, new GEvent.Callback(this.OnMapCharacterChanged));
			GEvent.Add(UiEvents.OnBloodLightLocationsChanged, new GEvent.Callback(this.OnBloodLightLocationsChanged));
			GEvent.Add(UiEvents.OnFairylandDataChanged, new GEvent.Callback(this.OnFairylandDataChanged));
			GEvent.Add(UiEvents.OnHeavenlyTreeDataChanged, new GEvent.Callback(this.OnHeavenlyTreeDataChanged));
			GEvent.Add(UiEvents.OnLingBaoLightLocationsChanged, new GEvent.Callback(this.OnLingBaoLightLocationsChanged));
			GEvent.Add(UiEvents.OnLingBaoDarkLocationsChanged, new GEvent.Callback(this.OnLingBaoDarkLocationsChanged));
			GEvent.Add(UiEvents.OnBloodLocationsChanged, new GEvent.Callback(this.OnBloodLocationsChanged));
			GEvent.Add(UiEvents.OnFulongInFlameAreasChanged, new GEvent.Callback(this.OnFulongFlameLocationsChanged));
			GEvent.Add(UiEvents.OnEmeiGuidanceDataChanged, new GEvent.Callback(this.OnEmeiGuidanceDataChanged));
			GEvent.Add(UiEvents.OnDreamBackLocationsChanged, new GEvent.Callback(this.OnDreamBackLocationsChanged));
			GEvent.Add(UiEvents.OnUpdateCaravanBlockCharData, new GEvent.Callback(this.OnUpdateCaravanBlockCharData));
			GEvent.Add(UiEvents.OnFiveLoongDictChanged, new GEvent.Callback(this.OnFiveLoongDictChanged));
			GEvent.Add(UiEvents.OnProfessionDataChange, new GEvent.Callback(this.OnProfessionDataChange));
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.HandleOpenWindow));
			GEvent.Add(EEvents.AchievementUnlocked, new GEvent.Callback(this.HandleOpenWindow));
			GEvent.Add(UiEvents.ProfessionSkillUnlock, new GEvent.Callback(this.HandleOpenWindow));
			GEvent.Add(UiEvents.CombatLifeSkillUnlockStrategy, new GEvent.Callback(this.HandleOpenWindow));
			GEvent.Add(UiEvents.NewFeatureUnlockHint, new GEvent.Callback(this.HandleNewFeatureUnlockHint));
			GEvent.Add(UiEvents.MapPickupDataChanged, new GEvent.Callback(this.RefreshMapPickups));
			GEvent.Add(UiEvents.MapPickupEffectChanged, new GEvent.Callback(this.RefreshMapPickupEffect));
			GEvent.Add(UiEvents.CricketWishEffectChanged, new GEvent.Callback(this.RefreshCricketWishEffect));
			GEvent.Add(UiEvents.DoWorldMapScaleTween, new GEvent.Callback(this.OnDoWorldMapScaleTween));
			GEvent.Add(UiEvents.OnClickWorldMapBlock, new GEvent.Callback(this.OnClickWorldMapBlock));
			GEvent.Add(UiEvents.OnReshowBlockList, new GEvent.Callback(this.OnReshowBlockList));
			GEvent.Add(UiEvents.OnShowBuildingArea, new GEvent.Callback(this.OnShowBuildingArea));
			GEvent.Add(UiEvents.OnHideBuildingArea, new GEvent.Callback(this.OnHideBuildingArea));
			GEvent.Add(UiEvents.AdventureRemakeIconClick, new GEvent.Callback(this.AdventureRemakeIconClick));
			GEvent.Add(UiEvents.AdventureRemakeOpenPartOne, new GEvent.Callback(this.AdventureRemakeOpenPartOne));
			GEvent.Add(UiEvents.AdventureRemakeOpenPartTwo, new GEvent.Callback(this.AdventureRemakeOpenPartTwo));
			GEvent.Add(UiEvents.AdventureRemakeExit, new GEvent.Callback(this.AdventureExitAnim));
			GEvent.Add(UiEvents.OnJieqingSignStateRefresh, new GEvent.Callback(this.OnJieqingSignStateRefresh));
			GEvent.Add(UiEvents.AdventureRemakeDictChange, new GEvent.Callback(this.AdventureRemakeDictChange));
			GEvent.Add(UiEvents.AdventureMajorEventChange, new GEvent.Callback(this.AdventureMajorEventChange));
			GEvent.Add(UiEvents.EventTriggerCricketCatch, new GEvent.Callback(this.EventTriggerCricketCatch));
			ConchShipCursor.Instance.AddMouseWheelProgressFullHandler(this.Element.Name, new Action(this.OpenStatePartWorldMap));
			this.UpdateBlockId();
			this.UpdateAreaSettlementBlockMap();
			this._playerAtBlockInitialized = false;
			this._mapModel.SelectedBlockId = -1;
			this.paths.GetComponent<LineRenderer2D>().VerticesProvider = (() => this._indicateLineValidVertices);
			this.invalidPaths.VerticesProvider = (() => this._indicateLineInvalidVertices);
			this.caravanPathRoot.GetComponent<LineRenderer2D>().VerticesProvider = (() => this._caravanLineVertices);
			MapRenderSystem.BlockEffectRoot = this.mapEffectRoot;
			MapRenderSystem.GetAdditionEffectInfo = (() => this.AdditionalBlockEffects);
			MapRenderSystem.BlockAppearAnimCurve = this.BlockAppearAnimationCurve;
			TooltipManager instance = SingletonObject.getInstance<TooltipManager>();
			instance.NeedShowNewTipsOnMouseExitTips = (TooltipManager.NeedShowNewTipsOnMouseExitTipsDelegate)Delegate.Combine(instance.NeedShowNewTipsOnMouseExitTips, new TooltipManager.NeedShowNewTipsOnMouseExitTipsDelegate(this.NeedShowTipsOnMouseExitTips));
		}

		// Token: 0x06006EBD RID: 28349 RVA: 0x00333C50 File Offset: 0x00331E50
		private void OnDestroy()
		{
			PoolManager.RemoveData(MapElementContainer.InfoPrefabKey);
			PoolManager.RemoveData(MapElementContainer.CostPrefabKey);
			PoolManager.RemoveData(MapElementContainer.CricketPrefabKey);
			PoolManager.RemoveData("ui_WorldmapInfo_TradeCaravanPathNodePrefab");
			PoolManager.RemoveData(MapElementContainer.AdventureRemakePrefabKey);
			PoolManager.RemoveData(MapElementContainer.SpecialCharacterPrefabKey);
			PoolManager.RemoveData(MapElementContainer.SettlementBtnPrefabKey);
			PoolManager.RemoveData(MapElementContainer.StationBtnPrefabKey);
			PoolManager.RemoveData(MapElementContainer.MerchantPrefabKey);
			PoolManager.RemoveData(MapElementContainer.ExpectPromptPrefabKey);
			PoolManager.RemoveData(MapElementContainer.TemporaryMarkPrefabKey);
			PoolManager.RemoveData(MapElementContainer.DreamBackPrefabKey);
			PoolManager.RemoveData(MapElementContainer.FulongFlamePrefabKey);
			PoolManager.RemoveData(MapElementContainer.ZhujianThiefPrefabKey);
			PoolManager.RemoveData(MapElementContainer.PickupPrefabKey);
			PoolManager.RemoveData(MapElementContainer.PickupEffectPrefabKey);
			PoolManager.RemoveData(MapElementContainer.CricketWishEffectPrefabKey);
			GEvent.Remove(EEvents.OnMonthChange, new GEvent.Callback(this.OnMonthChange));
			GEvent.Remove(UiEvents.WorldMapInited, new GEvent.Callback(this.OnMapInited));
			GEvent.Remove(UiEvents.AnimalPlaceDataChange, new GEvent.Callback(this.OnAnimalPlaceDataChange));
			GEvent.Remove(UiEvents.WorldMapBlockDataChange, new GEvent.Callback(this.OnBlockDataChange));
			GEvent.Remove(UiEvents.OnBrokenMaterialDataChange, new GEvent.Callback(this.OnBrokenMaterialDataChange));
			GEvent.Remove(UiEvents.OnMapBlockVisibilityNeedRefresh, new GEvent.Callback(this.OnMapBlockVisibilityNeedRefresh));
			GEvent.Remove(UiEvents.WorldMapResetMapCamera, new GEvent.Callback(this.OnResetMapCamera));
			GEvent.Remove(UiEvents.WorldMapSetCameraToArea, new GEvent.Callback(this.OnSetCameraToArea));
			GEvent.Remove(UiEvents.WorldMapSetCameraToLocation, new GEvent.Callback(this.OnSetCameraToLocation));
			GEvent.Remove(UiEvents.OnMoveTimeCostPercentChanged, new GEvent.Callback(this.OnMoveTimeCostPercentChanged));
			GEvent.Remove(UiEvents.OnVisitedSettlementsChanged, new GEvent.Callback(this.OnVisitedSettlementsChanged));
			GEvent.Remove(UiEvents.OnCharacterTaiwuGenderChanged, new GEvent.Callback(this.OnTaiwuGenderChanged));
			GEvent.Remove(UiEvents.OnCharacterTaiwuInventoryChanged, new GEvent.Callback(this.OnTaiwuInventoryChanged));
			GEvent.Remove(UiEvents.OnCharacterTaiwuCarrierChanged, new GEvent.Callback(this.OnTaiwuCarrierChanged));
			GEvent.Remove(UiEvents.OnStateAdventureDataReceived, new GEvent.Callback(this.OnStateAdventureDataReceived));
			GEvent.Remove(UiEvents.MapCurrentLocationFixedCharacterDataReceived, new GEvent.Callback(this.MapCurrentLocationFixedCharacterDataReceived));
			GEvent.Remove(UiEvents.OnEventHandleComplete, new GEvent.Callback(this.OnEventHandleComplete));
			GEvent.Remove(UiEvents.WorldMapPlayerBlockChange, new GEvent.Callback(this.OnPlayerLocationChangedInHide));
			GEvent.Remove(UiEvents.CricketPlaceDataChange, new GEvent.Callback(this.OnCricketPlaceDataChange));
			GEvent.Remove(UiEvents.OnZhujianThiefDataChanged, new GEvent.Callback(this.OnZhujianThiefDataChanged));
			GEvent.Remove(UiEvents.WorldMapVillagerWorkingLocationChange, new GEvent.Callback(this.OnWorldMapVillagerWorkingLocationChange));
			GEvent.Remove(UiEvents.OnLocationMarkChange, new GEvent.Callback(this.OnLocationMarkChange));
			GEvent.Remove(UiEvents.OnMapBlockMerchantTogChange, new GEvent.Callback(this.OnMapBlockMerchantTogChange));
			GEvent.Remove(UiEvents.OnMapAlterSettlementChanged, new GEvent.Callback(this.OnMapAlterSettlementChanged));
			GEvent.Remove(UiEvents.OnMapCharacterChanged, new GEvent.Callback(this.OnMapCharacterChanged));
			GEvent.Remove(UiEvents.OnBloodLightLocationsChanged, new GEvent.Callback(this.OnBloodLightLocationsChanged));
			GEvent.Remove(UiEvents.OnFairylandDataChanged, new GEvent.Callback(this.OnFairylandDataChanged));
			GEvent.Remove(UiEvents.OnHeavenlyTreeDataChanged, new GEvent.Callback(this.OnHeavenlyTreeDataChanged));
			GEvent.Remove(UiEvents.OnLingBaoLightLocationsChanged, new GEvent.Callback(this.OnLingBaoLightLocationsChanged));
			GEvent.Remove(UiEvents.OnLingBaoDarkLocationsChanged, new GEvent.Callback(this.OnLingBaoDarkLocationsChanged));
			GEvent.Remove(UiEvents.OnBloodLocationsChanged, new GEvent.Callback(this.OnBloodLocationsChanged));
			GEvent.Remove(UiEvents.OnFulongInFlameAreasChanged, new GEvent.Callback(this.OnFulongFlameLocationsChanged));
			GEvent.Remove(UiEvents.OnEmeiGuidanceDataChanged, new GEvent.Callback(this.OnEmeiGuidanceDataChanged));
			GEvent.Remove(UiEvents.OnDreamBackLocationsChanged, new GEvent.Callback(this.OnDreamBackLocationsChanged));
			GEvent.Remove(UiEvents.OnUpdateCaravanBlockCharData, new GEvent.Callback(this.OnUpdateCaravanBlockCharData));
			GEvent.Remove(UiEvents.OnFiveLoongDictChanged, new GEvent.Callback(this.OnFiveLoongDictChanged));
			GEvent.Remove(UiEvents.OnProfessionDataChange, new GEvent.Callback(this.OnProfessionDataChange));
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.HandleOpenWindow));
			GEvent.Remove(UiEvents.ProfessionSkillUnlock, new GEvent.Callback(this.HandleOpenWindow));
			GEvent.Remove(UiEvents.CombatLifeSkillUnlockStrategy, new GEvent.Callback(this.HandleOpenWindow));
			GEvent.Remove(UiEvents.NewFeatureUnlockHint, new GEvent.Callback(this.HandleNewFeatureUnlockHint));
			GEvent.Remove(UiEvents.MapPickupDataChanged, new GEvent.Callback(this.RefreshMapPickups));
			GEvent.Remove(UiEvents.MapPickupEffectChanged, new GEvent.Callback(this.RefreshMapPickupEffect));
			GEvent.Remove(UiEvents.CricketWishEffectChanged, new GEvent.Callback(this.RefreshCricketWishEffect));
			GEvent.Remove(UiEvents.DoWorldMapScaleTween, new GEvent.Callback(this.OnDoWorldMapScaleTween));
			GEvent.Remove(UiEvents.OnClickWorldMapBlock, new GEvent.Callback(this.OnClickWorldMapBlock));
			GEvent.Remove(UiEvents.OnReshowBlockList, new GEvent.Callback(this.OnReshowBlockList));
			GEvent.Remove(UiEvents.OnShowBuildingArea, new GEvent.Callback(this.OnShowBuildingArea));
			GEvent.Remove(UiEvents.OnHideBuildingArea, new GEvent.Callback(this.OnHideBuildingArea));
			GEvent.Remove(UiEvents.AdventureRemakeIconClick, new GEvent.Callback(this.AdventureRemakeIconClick));
			GEvent.Remove(UiEvents.AdventureRemakeOpenPartOne, new GEvent.Callback(this.AdventureRemakeOpenPartOne));
			GEvent.Remove(UiEvents.AdventureRemakeOpenPartTwo, new GEvent.Callback(this.AdventureRemakeOpenPartTwo));
			GEvent.Remove(UiEvents.AdventureRemakeExit, new GEvent.Callback(this.AdventureExitAnim));
			GEvent.Remove(UiEvents.OnJieqingSignStateRefresh, new GEvent.Callback(this.OnJieqingSignStateRefresh));
			GEvent.Remove(UiEvents.AdventureRemakeDictChange, new GEvent.Callback(this.AdventureRemakeDictChange));
			GEvent.Remove(UiEvents.AdventureMajorEventChange, new GEvent.Callback(this.AdventureMajorEventChange));
			GEvent.Remove(UiEvents.EventTriggerCricketCatch, new GEvent.Callback(this.EventTriggerCricketCatch));
			this.CollectAllContainers();
			ConchShipCursor.Instance.RemoveMouseWheelProgressFullHandler(new Action(this.OpenStatePartWorldMap));
			AudioManager.Instance.StopLoopSoundWithAmbience("SFX_professionskill_lvren_loop");
			TooltipManager instance = SingletonObject.getInstance<TooltipManager>();
			instance.NeedShowNewTipsOnMouseExitTips = (TooltipManager.NeedShowNewTipsOnMouseExitTipsDelegate)Delegate.Remove(instance.NeedShowNewTipsOnMouseExitTips, new TooltipManager.NeedShowNewTipsOnMouseExitTipsDelegate(this.NeedShowTipsOnMouseExitTips));
		}

		// Token: 0x06006EBE RID: 28350 RVA: 0x0033437C File Offset: 0x0033257C
		private void OnEnable()
		{
			GEvent.Add(UiEvents.WorldMapPlayerAreaChange, new GEvent.Callback(this.OnPlayerAreaChange));
			GEvent.Add(UiEvents.WorldMapPlayerBlockChange, new GEvent.Callback(this.OnPlayerBlockChange));
			GEvent.Add(UiEvents.WorldMapEnterNewArea, new GEvent.Callback(this.OnEnterNewArea));
			GEvent.Add(UiEvents.WorldMapAreaDataChange, new GEvent.Callback(this.OnAreaDataChange));
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChange));
			GEvent.Add(EEvents.OnActionPointChange, new GEvent.Callback(this.OnDaysInMonthChange));
			GEvent.Add(EEvents.OnScreenResolutionChange, new GEvent.Callback(this.OnResolutionChange));
			GEvent.Add(EEvents.OnFunctionLockStateChange, new GEvent.Callback(this.OnGameFunctionLockStateChange));
			GEvent.Add(UiEvents.OnMapBlockEnemyTogChange, new GEvent.Callback(this.OnMapBlockEnemyTogChange));
			GEvent.Add(UiEvents.OnForceRefreshAllMapBlock, new GEvent.Callback(this.OnForceRefreshAllMapBlock));
			GEvent.Add(UiEvents.OnMapBlockFriendTogChange, new GEvent.Callback(this.OnMapBlockFriendTogChange));
			GEvent.Add(UiEvents.WorldMapRefreshTradeCaravanPath, new GEvent.Callback(this.OnRefreshTradeCaravanPath));
			GEvent.Add(UiEvents.WorldMapShowPath, new GEvent.Callback(this.OnShowPath));
			GEvent.Add(UiEvents.WorldMapHidePath, new GEvent.Callback(this.OnHidePath));
			GEvent.Add(UiEvents.UpdateMapSettlementBtn, new GEvent.Callback(this.OnTutorialChapterIndexUpdated));
			GEvent.Add(UiEvents.OnMapBlockDisplayDataChanged, new GEvent.Callback(this.OnMapBlockDisplayDataChanged));
			GEvent.Add(UiEvents.MapFocusLocationGrave, new GEvent.Callback(this.MapFocusLocationGrave));
			GEvent.Add(UiEvents.MapClearLocationTemporaryMark, new GEvent.Callback(this.MapClearLocationTemporaryMark));
			GEvent.Add(UiEvents.MapAddLocationsToTemporaryMarkList, new GEvent.Callback(this.MapAddLocationsToTemporaryMarkList));
			GEvent.Add(UiEvents.MapClearAllTemporaryMarkList, new GEvent.Callback(this.MapClearAllTemporaryMarkList));
			GEvent.Add(UiEvents.MapAddLocationsToTemporaryMarkListForTask, new GEvent.Callback(this.MapAddLocationsToTemporaryMarkListForTask));
			GEvent.Add(UiEvents.MapClearAllTemporaryMarkListForTask, new GEvent.Callback(this.MapClearAllTemporaryMarkListForTask));
			GEvent.Add(UiEvents.VillagerWorkDataChange, new GEvent.Callback(this.VillagerWorkDataChange));
			GEvent.Add(UiEvents.WorldMapDoShakeByEvent, new GEvent.Callback(this.WorldMapDoShakeByEvent));
			GEvent.Add(UiEvents.WorldMapDoFocusByEvent, new GEvent.Callback(this.WorldMapDoFocusByEvent));
			GEvent.Add(UiEvents.OnClickMapElement, new GEvent.Callback(this.OnClickMapElement));
			GEvent.Add(UiEvents.OnHoverMapElement, new GEvent.Callback(this.OnHoverMapElement));
			GEvent.Add(UiEvents.OnHoverExitMapElement, new GEvent.Callback(this.OnHoverExitMapElement));
			GEvent.Add(UiEvents.OnMapBlockPastLifeRelationTogChange, new GEvent.Callback(this.OnMapBlockPastLifeRelationTogChange));
			GEvent.Add(UiEvents.ProfessionTravelerSkillTwoStart, new GEvent.Callback(this.ProfessionTravelerSkillTwoStart));
			GEvent.Add(UiEvents.ProfessionTravelerSkillTwoStop, new GEvent.Callback(this.ProfessionTravelerSkillTwoStop));
			GEvent.Add(UiEvents.ProfessionSkillConfirmSelectCancel, new GEvent.Callback(this.ProfessionSkillConfirmSelectCancel));
			GEvent.Add(UiEvents.ProfessionTravelerSkillThreeMove, new GEvent.Callback(this.ProfessionTravelerSkillThreeMove));
			GEvent.Add(UiEvents.ProfessionTravelerSkillTwoMoveStart, new GEvent.Callback(this.ProfessionTravelerSkillTwoMoveStart));
			GEvent.Add(UiEvents.ProfessionTravelerSkillTwoMoveEnd, new GEvent.Callback(this.ProfessionTravelerSkillTwoMoveEnd));
			GEvent.Add(UiEvents.InviteSelectBlockStart, new GEvent.Callback(this.InviteSelectBlockStart));
			GEvent.Add(UiEvents.InviteSelectBlockStop, new GEvent.Callback(this.InviteSelectBlockStop));
			GEvent.Add(UiEvents.DivineFlameSelectBlockStart, new GEvent.Callback(this.DivineFlameSelectBlockStart));
			GEvent.Add(UiEvents.DivineFlameSelectBlockStop, new GEvent.Callback(this.DivineFlameSelectBlockStop));
			GEvent.Add(UiEvents.DefeatSwordTomb, new GEvent.Callback(this.DefeatSwordTomb));
			this.MapClickReceiver.ExtraScaleListening = (() => !this._pathMoving && !this._teleportMoving && !this.IsDoingMove && !this.IsMoving);
			this.MapClickReceiver.OnMapBlockClick = delegate(int x, int y)
			{
				MapBlockData blockData = this.FindBlockByLogicalPosition(x, y);
				bool flag2 = blockData != null && blockData.AreaId == this._mapModel.CurrentAreaId;
				if (flag2)
				{
					this.OnBlockClick(blockData);
				}
			};
			this.MapClickReceiver.OnMapBlockPointStay = delegate(int x, int y)
			{
				TooltipInvoker mouseTips = this.MapClickReceiver.TipDisplayer;
				bool flag2 = this.<OnEnable>g__CheckNeedShowTip|151_4(x, y);
				if (flag2)
				{
					mouseTips.enabled = true;
					MouseTipBase mouseTipBase = SingletonObject.getInstance<TooltipManager>().GetTipsUi(mouseTips.Type);
					bool flag3 = mouseTipBase && !mouseTipBase.gameObject.activeSelf;
					if (flag3)
					{
						mouseTips.ShowTips();
					}
				}
				else
				{
					mouseTips.enabled = false;
				}
			};
			this.MapClickReceiver.OnMapBlockPointEnter = delegate(int x, int y)
			{
				MapBlockData blockData = this.FindBlockByLogicalPosition(x, y);
				TooltipInvoker mouseTips = this.MapClickReceiver.TipDisplayer;
				bool flag2 = blockData != null && blockData.AreaId == this._mapModel.CurrentAreaId;
				if (flag2)
				{
					Location location = new Location(blockData.AreaId, blockData.BlockId);
					bool flag3 = blockData.Visible || this._professionTravelerSkillTwoActive || this._inviteSelectBlockActive;
					if (flag3)
					{
						this._selectVirtual.anchoredPosition = MapRenderSystem.GetBlockLocalPos(location);
						this._selectVirtual.gameObject.SetActive(true);
						ConchShipCursor.Instance.TrySetClickableCursor();
						mouseTips.enabled = true;
					}
					else
					{
						this._selectVirtual.gameObject.SetActive(false);
						ConchShipCursor.Instance.TrySetDefaultCursor();
						mouseTips.enabled = false;
					}
					bool flag4 = blockData.IsCityTown() && SingletonObject.getInstance<GlobalSettings>().ShowMapBlockSettlementEdge;
					if (flag4)
					{
						bool flag5 = !this._areaSettlementBlockMap.Keys.Contains(this._displayingSettlementBlockId);
						if (flag5)
						{
							this._displayingSettlementBlockId = blockData.GetRootBlock().BlockId;
							this.SettlementBlockConatiner.TurnOffAll();
							HashSet<short> blockIds;
							bool flag6 = this._areaSettlementBlockMap.TryGetValue(this._displayingSettlementBlockId, out blockIds);
							if (flag6)
							{
								foreach (short blockId in blockIds)
								{
									Location otherLocation = new Location(blockData.AreaId, blockId);
									MapBlockData data = this._mapModel.GetBlockData(otherLocation);
									this.SettlementBlockConatiner.TryRefresh(otherLocation, data.Visible, this._displayingSettlementBlockId);
								}
							}
						}
					}
					else
					{
						this.SettlementBlockConatiner.TurnOffAll();
						this._displayingSettlementBlockId = -1;
					}
					bool flag7 = !this.<OnEnable>g__CheckNeedShowTip|151_4(x, y);
					if (!flag7)
					{
						List<CaravanDisplayData> caravanList = (from d in this._mapModel.CaravanData
						where d.PathInArea.GetCurrLocation() == new Location(blockData.AreaId, blockData.BlockId)
						select d).ToList<CaravanDisplayData>();
						BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
						VillagerWorkData villagerWorkData = buildingModel.VillagerWork.Values.ToList<VillagerWorkData>().Find((VillagerWorkData d) => d.AreaId == blockData.AreaId && d.BlockId == blockData.BlockId && VillagerWorkType.IsWorkOnMap(d.WorkType));
						ArgumentBox argsBox = new ArgumentBox();
						argsBox.SetObject("MapBlockData", blockData);
						argsBox.Set("IsUseFullName", false);
						argsBox.SetObject("CaravanList", caravanList);
						argsBox.SetObject("VillagerWorkData", villagerWorkData);
						mouseTips.RuntimeParam = argsBox;
						mouseTips.Type = TipType.MapBlock;
						TooltipManager manager = SingletonObject.getInstance<TooltipManager>();
						bool isHitting = manager.IsHittingShowingTips();
						this._currentCoord = new Vector2((float)x, (float)y);
						this._hoverBlockX = x;
						this._hoverBlockY = y;
						bool flag8 = !isHitting;
						if (flag8)
						{
							this._lastShowTipsCoord = this._currentCoord;
							manager.Tick(true);
						}
					}
				}
				else
				{
					ConchShipCursor.Instance.TrySetDefaultCursor();
					this.<OnEnable>g__FailedToHandleMapBlock|151_0();
					this.SettlementBlockConatiner.TurnOffAll();
				}
			};
			this.MapClickReceiver.OnMapBlockPointExit = delegate(int x, int y)
			{
				ConchShipCursor.Instance.TrySetDefaultCursor();
				this.<OnEnable>g__FailedToHandleMapBlock|151_0();
				this._hoverBlockX = -1;
				this._hoverBlockY = -1;
			};
			this.BackGroundPointerTrigger.EnterEvent.RemoveAllListeners();
			this.BackGroundPointerTrigger.EnterEvent.AddListener(delegate()
			{
				this.MapClickReceiver.ScaleListening = true;
			});
			this.BackGroundPointerTrigger.ExitEvent.RemoveAllListeners();
			this.BackGroundPointerTrigger.ExitEvent.AddListener(delegate()
			{
				this.MapClickReceiver.ScaleListening = false;
			});
			this.MapClickReceiver.ExtraScaleListening = (() => !this._pathMoving && !this._teleportMoving && !this.IsDoingMove && !this.IsMoving);
			this.MapClickReceiver.OnScaleEvent = delegate(float scale)
			{
				bool flag2 = this._pathMoving || this._teleportMoving || this.IsDoingMove || this.IsMoving;
				if (!flag2)
				{
					Vector2 mousePosition = UIManager.Instance.MousePosToLocalPos(this.moveRoot);
					Vector3 scaleOld = this.moveRoot.localScale;
					this.moveRoot.localScale = scale * Vector3.one;
					this.MoveCameraTo(this._lastCameraMoveTarget + (this._lastCameraMoveTarget - mousePosition) * (scaleOld.x / this.moveRoot.localScale.x - 1f), false, null, 0.2f, Ease.Unset);
					this.OnMapScaled(scale, false, true);
				}
			};
			this.MapClickReceiver.OnDragEvent = delegate(Vector2 offset)
			{
				bool flag2 = this._pathMoving || this._teleportMoving;
				if (!flag2)
				{
					this.MoveCameraTo(this._lastCameraMoveTarget - offset, false, null, 0.2f, Ease.Unset);
				}
			};
			this.caravanRoot.gameObject.SetActive(SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(9));
			this.RefreshSettlementAndStationBtn();
			this.UpdateStationBtnVisible();
			bool flag = this._inputOffsetCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._inputOffsetCoroutine);
				this._inputOffsetCoroutine = null;
			}
			base.StartCoroutine(this._inputOffsetCoroutine = this.<OnEnable>g__InputOffsetProcess|151_12());
			this.OnMapScaled(this.MapClickReceiver.CurrentScale, true, true);
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.5f, delegate
			{
				this.MapClickReceiver.RefreshScale();
			});
			MerchantDomainMethod.Call.PullTradeCaravanLocations();
		}

		// Token: 0x06006EBF RID: 28351 RVA: 0x00334964 File Offset: 0x00332B64
		private void OnDisable()
		{
			bool flag = this._inputOffsetCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._inputOffsetCoroutine);
				this._inputOffsetCoroutine = null;
			}
			GEvent.Remove(UiEvents.WorldMapPlayerAreaChange, new GEvent.Callback(this.OnPlayerAreaChange));
			GEvent.Remove(UiEvents.WorldMapPlayerBlockChange, new GEvent.Callback(this.OnPlayerBlockChange));
			GEvent.Remove(UiEvents.WorldMapEnterNewArea, new GEvent.Callback(this.OnEnterNewArea));
			GEvent.Remove(UiEvents.WorldMapAreaDataChange, new GEvent.Callback(this.OnAreaDataChange));
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChange));
			GEvent.Remove(EEvents.OnActionPointChange, new GEvent.Callback(this.OnDaysInMonthChange));
			GEvent.Remove(EEvents.OnScreenResolutionChange, new GEvent.Callback(this.OnResolutionChange));
			GEvent.Remove(EEvents.OnFunctionLockStateChange, new GEvent.Callback(this.OnGameFunctionLockStateChange));
			GEvent.Remove(UiEvents.OnMapBlockEnemyTogChange, new GEvent.Callback(this.OnMapBlockEnemyTogChange));
			GEvent.Remove(UiEvents.OnForceRefreshAllMapBlock, new GEvent.Callback(this.OnForceRefreshAllMapBlock));
			GEvent.Remove(UiEvents.OnMapBlockFriendTogChange, new GEvent.Callback(this.OnMapBlockFriendTogChange));
			GEvent.Remove(UiEvents.WorldMapRefreshTradeCaravanPath, new GEvent.Callback(this.OnRefreshTradeCaravanPath));
			GEvent.Remove(UiEvents.WorldMapShowPath, new GEvent.Callback(this.OnShowPath));
			GEvent.Remove(UiEvents.WorldMapHidePath, new GEvent.Callback(this.OnHidePath));
			GEvent.Remove(UiEvents.UpdateMapSettlementBtn, new GEvent.Callback(this.OnTutorialChapterIndexUpdated));
			GEvent.Remove(UiEvents.OnMapBlockDisplayDataChanged, new GEvent.Callback(this.OnMapBlockDisplayDataChanged));
			GEvent.Remove(UiEvents.MapFocusLocationGrave, new GEvent.Callback(this.MapFocusLocationGrave));
			GEvent.Remove(UiEvents.MapClearLocationTemporaryMark, new GEvent.Callback(this.MapClearLocationTemporaryMark));
			GEvent.Remove(UiEvents.MapAddLocationsToTemporaryMarkList, new GEvent.Callback(this.MapAddLocationsToTemporaryMarkList));
			GEvent.Remove(UiEvents.MapClearAllTemporaryMarkList, new GEvent.Callback(this.MapClearAllTemporaryMarkList));
			GEvent.Remove(UiEvents.MapAddLocationsToTemporaryMarkListForTask, new GEvent.Callback(this.MapAddLocationsToTemporaryMarkListForTask));
			GEvent.Remove(UiEvents.MapClearAllTemporaryMarkListForTask, new GEvent.Callback(this.MapClearAllTemporaryMarkListForTask));
			GEvent.Remove(UiEvents.VillagerWorkDataChange, new GEvent.Callback(this.VillagerWorkDataChange));
			GEvent.Remove(UiEvents.WorldMapDoShakeByEvent, new GEvent.Callback(this.WorldMapDoShakeByEvent));
			GEvent.Remove(UiEvents.WorldMapDoFocusByEvent, new GEvent.Callback(this.WorldMapDoFocusByEvent));
			GEvent.Remove(UiEvents.OnClickMapElement, new GEvent.Callback(this.OnClickMapElement));
			GEvent.Remove(UiEvents.OnHoverMapElement, new GEvent.Callback(this.OnHoverMapElement));
			GEvent.Remove(UiEvents.OnHoverExitMapElement, new GEvent.Callback(this.OnHoverExitMapElement));
			GEvent.Remove(UiEvents.OnMapBlockPastLifeRelationTogChange, new GEvent.Callback(this.OnMapBlockPastLifeRelationTogChange));
			GEvent.Remove(UiEvents.ProfessionTravelerSkillTwoStart, new GEvent.Callback(this.ProfessionTravelerSkillTwoStart));
			GEvent.Remove(UiEvents.ProfessionTravelerSkillTwoStop, new GEvent.Callback(this.ProfessionTravelerSkillTwoStop));
			GEvent.Remove(UiEvents.ProfessionSkillConfirmSelectCancel, new GEvent.Callback(this.ProfessionSkillConfirmSelectCancel));
			GEvent.Remove(UiEvents.ProfessionTravelerSkillThreeMove, new GEvent.Callback(this.ProfessionTravelerSkillThreeMove));
			GEvent.Remove(UiEvents.ProfessionTravelerSkillTwoMoveStart, new GEvent.Callback(this.ProfessionTravelerSkillTwoMoveStart));
			GEvent.Remove(UiEvents.ProfessionTravelerSkillTwoMoveEnd, new GEvent.Callback(this.ProfessionTravelerSkillTwoMoveEnd));
			GEvent.Remove(UiEvents.InviteSelectBlockStart, new GEvent.Callback(this.InviteSelectBlockStart));
			GEvent.Remove(UiEvents.InviteSelectBlockStop, new GEvent.Callback(this.InviteSelectBlockStop));
			GEvent.Remove(UiEvents.DivineFlameSelectBlockStart, new GEvent.Callback(this.DivineFlameSelectBlockStart));
			GEvent.Remove(UiEvents.DivineFlameSelectBlockStop, new GEvent.Callback(this.DivineFlameSelectBlockStop));
			GEvent.Remove(UiEvents.DefeatSwordTomb, new GEvent.Callback(this.DefeatSwordTomb));
			this.MapClickReceiver.OnMapBlockClick = null;
			this.MapClickReceiver.OnMapBlockPointEnter = null;
			this.MapClickReceiver.OnMapBlockPointExit = null;
			this.MapClickReceiver.OnScaleEvent = null;
			this.MapClickReceiver.OnDragEvent = null;
			this._hotkeyMoveDirection = MoveDirection.None;
			this._pathMoving = false;
			ViewWorldMap.SetDisableMoving(false);
			ConchShipCursor.Instance.SetCursor(-1);
		}

		// Token: 0x06006EC0 RID: 28352 RVA: 0x00334E14 File Offset: 0x00333014
		private MapBlockData FindBlockByLogicalPosition(int x, int y)
		{
			WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
			short areaId = worldMapModel.ShowingAreaId;
			foreach (Location blockPair in this._mapBlockSet)
			{
				Location location = blockPair;
				bool flag = location.AreaId != areaId;
				if (!flag)
				{
					MapBlockData blockData = this._mapModel.GetBlockData(location);
					ByteCoordinate position = blockData.GetBlockPos();
					bool flag2 = x == (int)position.X && y == (int)position.Y;
					if (flag2)
					{
						return blockData;
					}
				}
			}
			return null;
		}

		// Token: 0x06006EC1 RID: 28353 RVA: 0x00334ED0 File Offset: 0x003330D0
		private void HandleMarkCurBlock()
		{
			MapBlockData blockData = this.FindBlockByLogicalPosition(this._hoverBlockX, this._hoverBlockY);
			bool flag = blockData == null || blockData.AreaId != this._mapModel.CurrentAreaId;
			if (!flag)
			{
				Location location = blockData.GetLocation();
				BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
				bool hasWorker = buildingModel.CheckBlockHasWork(location, -1);
				bool flag2 = hasWorker || buildingModel.CheckBlockIsMarked(location);
				if (flag2)
				{
					bool flag3 = hasWorker;
					if (flag3)
					{
						UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
						{
							Title = LanguageKey.LK_Mark_Cancel_Tip_Title.Tr(),
							Content = LanguageKey.LK_Mark_Cancel_Tip_Desc2.Tr(),
							Type = 1,
							Yes = delegate()
							{
								ExtraDomainMethod.Call.RemoveLocationMark(location);
								SingletonObject.getInstance<WorldMapModel>().RequestStopVillagerWorkOnMap(location, true);
							},
							No = null
						}));
						UIManager.Instance.MaskUI(UIElement.Dialog);
					}
					else
					{
						ExtraDomainMethod.Call.RemoveLocationMark(location);
					}
				}
				else
				{
					ExtraDomainMethod.Call.AddLocationMark(location);
				}
			}
		}

		// Token: 0x06006EC2 RID: 28354 RVA: 0x00334FF4 File Offset: 0x003331F4
		private void Update()
		{
			bool inAdventureRemake = ViewWorldMap.InAdventureRemake;
			if (!inAdventureRemake)
			{
				bool flag = this._pathMoving && (Input.GetMouseButtonDown(1) || MapCommandKit.MoveUp.Check(this.Element, true, false, true, true, false) || MapCommandKit.MoveDown.Check(this.Element, true, false, true, true, false) || MapCommandKit.MoveLeft.Check(this.Element, true, false, true, true, false) || MapCommandKit.MoveRight.Check(this.Element, true, false, true, true, false));
				if (flag)
				{
					this._pathMoving = false;
					bool flag2 = Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
					if (flag2)
					{
						this.StopMove();
					}
				}
				bool flag3 = CommandKitBase.GetDisable() || !this._playerAtBlockInitialized || !this._hotkeyMoveReady || this._mapModel.TaiwuMoveState != WorldMapModel.MoveState.Idle || this.MapClickReceiver.IsDragProcessing() || SingletonObject.getInstance<EventModel>().LockInputByEvent || UIElement.CollectResource.Exist;
				if (flag3)
				{
					this._hotkeyMoveDirection = MoveDirection.None;
				}
				else
				{
					bool inputBanned = UIElement.FullScreenMask.Exist || ViewMapBlockCharList.IsFocusOnSearchInputField;
					bool flag4 = this._hotkeyMoveDirection == MoveDirection.None && !inputBanned && !this._professionTravelerSkillTwoActive && !this._inviteSelectBlockActive;
					if (flag4)
					{
						bool flag5 = MapCommandKit.MoveUp.Check(this.Element, true, false, false, true, false);
						if (flag5)
						{
							this._hotkeyMoveDirection = MoveDirection.Up;
						}
						else
						{
							bool flag6 = MapCommandKit.MoveLeft.Check(this.Element, true, false, false, true, false);
							if (flag6)
							{
								this._hotkeyMoveDirection = MoveDirection.Left;
							}
							else
							{
								bool flag7 = MapCommandKit.MoveDown.Check(this.Element, true, false, false, true, false);
								if (flag7)
								{
									this._hotkeyMoveDirection = MoveDirection.Down;
								}
								else
								{
									bool flag8 = MapCommandKit.MoveRight.Check(this.Element, true, false, false, true, false);
									if (flag8)
									{
										this._hotkeyMoveDirection = MoveDirection.Right;
									}
								}
							}
						}
					}
					else
					{
						bool flag9 = inputBanned || (this._hotkeyMoveDirection == MoveDirection.Up && !MapCommandKit.MoveUp.Check(this.Element, true, false, false, true, false)) || (this._hotkeyMoveDirection == MoveDirection.Left && !MapCommandKit.MoveLeft.Check(this.Element, true, false, false, true, false)) || (this._hotkeyMoveDirection == MoveDirection.Down && !MapCommandKit.MoveDown.Check(this.Element, true, false, false, true, false)) || (this._hotkeyMoveDirection == MoveDirection.Right && !MapCommandKit.MoveRight.Check(this.Element, true, false, false, true, false));
						if (flag9)
						{
							this._hotkeyMoveDirection = MoveDirection.None;
						}
					}
					bool flag10 = this._hotkeyMoveDirection != MoveDirection.None;
					if (flag10)
					{
						MapBlockData block = this._mapModel.GetNeighbor(this.PlayerAtBlock, this._hotkeyMoveDirection, true);
						bool flag11 = block != null && block.AreaId == this._mapModel.ShowingAreaId && !ViewWorldMap._disableMoving;
						if (flag11)
						{
							this._pathMoving = false;
							this._hotkeyMoveReady = false;
							this.SelectBlock(block);
							this.RefreshTradeCaravanPath(-1);
							this.FindWay(block.GetLocation());
							bool flag12 = this.CheckGuidBlockMove();
							if (flag12)
							{
								this._hotkeyMoveReady = true;
							}
							else
							{
								this.MoveToNext();
							}
						}
						else
						{
							this._hotkeyMoveDirection = MoveDirection.None;
						}
					}
				}
			}
		}

		// Token: 0x06006EC3 RID: 28355 RVA: 0x00335328 File Offset: 0x00333528
		public static bool CheckRightMouseClickHandled()
		{
			ViewWorldMap self = UIElement.WorldMap.UiBaseAs<ViewWorldMap>();
			bool flag = !self._pathMoving && !UIElement.VillagerRoleSelectStorageType.Exist;
			if (flag)
			{
				bool flag2 = self._professionTravelerSkillTwoActive && !UIManager.Instance.IsFocusElement(UIElement.ProfessionSkillConfirm);
				if (flag2)
				{
					self.ProfessionTravelerSkillTwoCancel();
					return true;
				}
				bool inviteSelectBlockActive = self._inviteSelectBlockActive;
				if (inviteSelectBlockActive)
				{
					self.InviteSelectBlockCancel();
					return true;
				}
				bool isDivineFlameActive = self.IsDivineFlameActive;
				if (isDivineFlameActive)
				{
					self.DivineFlameSelectBlockCancel();
					return true;
				}
			}
			return false;
		}

		// Token: 0x06006EC4 RID: 28356 RVA: 0x003353C4 File Offset: 0x003335C4
		private void LateUpdate()
		{
			bool flag = !this._sortingDirty;
			if (!flag)
			{
				this._sortingDirty = false;
				this._positions.Clear();
				foreach (KeyValuePair<Location, MapElementContainer> keyValuePair in this._activateMapElements)
				{
					Location location3;
					MapElementContainer mapElementContainer;
					keyValuePair.Deconstruct(out location3, out mapElementContainer);
					Location location = location3;
					MapElementContainer container = mapElementContainer;
					bool flag2 = container.Count > 0;
					if (flag2)
					{
						this._positions.Add(location);
					}
				}
				CollectionUtils.Sort<Location>(this._positions, new Func<Location, Location, int>(this.Comparison));
				Location taiwuPosition = this._mapModel.CurrentLocation;
				this.GetElementContainer(taiwuPosition).SyncSorting();
				this._positions.Remove(taiwuPosition);
				int siblingIndex = 0;
				foreach (Location location2 in this._positions)
				{
					this.GetElementContainer(location2).SyncSorting(ref siblingIndex);
				}
			}
		}

		// Token: 0x06006EC5 RID: 28357 RVA: 0x003354FC File Offset: 0x003336FC
		private bool NeedShowTipsOnMouseExitTips()
		{
			return this._lastShowTipsCoord != this._currentCoord;
		}

		// Token: 0x06006EC6 RID: 28358 RVA: 0x00335520 File Offset: 0x00333720
		private int Comparison(Location a, Location b)
		{
			bool flag = a.AreaId != b.AreaId;
			int result;
			if (flag)
			{
				result = a.AreaId.CompareTo(b.AreaId);
			}
			else
			{
				bool flag2 = a.BlockId == b.BlockId;
				if (flag2)
				{
					result = 0;
				}
				else
				{
					byte size = this._mapModel.AreaMapSize[a.AreaId];
					ByteCoordinate aPos = WorldMapModel.IndexToCoordinate(a.BlockId, size);
					ByteCoordinate bPos = WorldMapModel.IndexToCoordinate(b.BlockId, size);
					int ay = (int)(aPos.Y - aPos.X);
					int by = (int)(bPos.Y - bPos.X);
					bool flag3 = ay != by;
					if (flag3)
					{
						result = by.CompareTo(ay);
					}
					else
					{
						int ax = (int)(aPos.X + aPos.Y);
						int bx = (int)(bPos.X + bPos.Y);
						result = ax.CompareTo(bx);
					}
				}
			}
			return result;
		}

		// Token: 0x06006EC7 RID: 28359 RVA: 0x00335610 File Offset: 0x00333810
		private void OnApplicationFocus(bool hasFocus)
		{
			if (hasFocus)
			{
				bool flag = null != this.MapClickReceiver;
				if (flag)
				{
					CanvasGroup canvasGroup = this.MapClickReceiver.GetComponent<CanvasGroup>();
					bool flag2 = canvasGroup != null;
					if (flag2)
					{
						canvasGroup.blocksRaycasts = true;
					}
				}
			}
		}

		// Token: 0x06006EC8 RID: 28360 RVA: 0x00335658 File Offset: 0x00333858
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 1)
				{
					this.HandlerMethodReturn(wrapper, notification);
				}
			}
		}

		// Token: 0x06006EC9 RID: 28361 RVA: 0x003356C8 File Offset: 0x003338C8
		public void OnPointerEnterMapIcon()
		{
			ViewWorldMap.PointerOnAnyMapBlock = true;
		}

		// Token: 0x06006ECA RID: 28362 RVA: 0x003356D1 File Offset: 0x003338D1
		public void OnPointerExitMapIcon()
		{
			ViewWorldMap.PointerOnAnyMapBlock = false;
		}

		// Token: 0x06006ECB RID: 28363 RVA: 0x003356DC File Offset: 0x003338DC
		public void OnCatchCricketClicked()
		{
			bool flag = this._mapModel.TaiwuMoveState > WorldMapModel.MoveState.Idle;
			if (!flag)
			{
				this._wishingCricketId = -1;
				CricketPlaceExtraData cricketPlaceExtraData;
				short num;
				bool flag2 = this._mapModel.CricketPlaceExtraData.TryGetValue(this._mapModel.CurrentAreaId, out cricketPlaceExtraData) && cricketPlaceExtraData != null && cricketPlaceExtraData.ExtraMapUnits != null && cricketPlaceExtraData.ExtraMapUnits.TryGetValue(this._mapModel.CurrentBlockId, out num);
				if (flag2)
				{
					bool flag3 = cricketPlaceExtraData.WishingCrickets != null;
					if (flag3)
					{
						this._wishingCricketId = cricketPlaceExtraData.WishingCrickets.GetOrDefault(this._mapModel.CurrentBlockId, -1);
					}
				}
				else
				{
					CricketPlaceData cricketData = this._mapModel.CricketPlaceData[(int)this._mapModel.CurrentAreaId];
					int index = Array.IndexOf<short>(cricketData.CricketBlocks, this._mapModel.CurrentBlockId);
					int groupIndex = index / 3;
					bool flag4 = (int)cricketData.RealCircketIdx[groupIndex] != index % 3;
					if (flag4)
					{
						bool flag5 = SingletonObject.getInstance<WorldMapModel>().CurrentAreaId != 135;
						if (flag5)
						{
							DialogCmd cmd = new DialogCmd
							{
								Title = LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", 1100)),
								Content = LocalStringManager.Get(LanguageKey.LK_Cricket_FakeCricket).ColorReplace(),
								Type = 2
							};
							UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
							UIManager.Instance.MaskUI(UIElement.Dialog);
						}
						else
						{
							this._mapModel.ChangeTaiwuMoveState(WorldMapModel.MoveState.WaitEventShow);
							TaiwuEventDomainMethod.Call.MainStoryFinishCatchCricket(false);
						}
					}
				}
				MapDomainMethod.Call.TryTriggerCricketCatch(this.Element.GameDataListenerId);
				ViewWorldMap.SetDisableMoving(true);
			}
		}

		// Token: 0x06006ECC RID: 28364 RVA: 0x00335896 File Offset: 0x00333A96
		private void OnShowPath(ArgumentBox _)
		{
			this.ShowPath();
		}

		// Token: 0x06006ECD RID: 28365 RVA: 0x0033589F File Offset: 0x00333A9F
		private void OnHidePath(ArgumentBox _)
		{
			this.HidePath();
		}

		// Token: 0x06006ECE RID: 28366 RVA: 0x003358A8 File Offset: 0x00333AA8
		private void OnTutorialChapterIndexUpdated(ArgumentBox _)
		{
			this.UpdateSettlementAndStationBtnUsable();
		}

		// Token: 0x06006ECF RID: 28367 RVA: 0x003358B4 File Offset: 0x00333AB4
		private void OnUpdateCaravanBlockCharData(ArgumentBox _)
		{
			bool flag = this._playerAtBlockInitialized && this._mapModel.CurrentLocation.IsValid();
			if (flag)
			{
				this.RefreshCaravans();
			}
		}

		// Token: 0x06006ED0 RID: 28368 RVA: 0x003358EC File Offset: 0x00333AEC
		private void OnRefreshTradeCaravanPath(ArgumentBox argumentBox)
		{
			int merchantId;
			argumentBox.Get("merchantId", out merchantId);
			this.RefreshTradeCaravanPath(merchantId);
		}

		// Token: 0x06006ED1 RID: 28369 RVA: 0x00335910 File Offset: 0x00333B10
		private void OnResetMapCamera(ArgumentBox argumentBox)
		{
			bool isAnim;
			argumentBox.Get("isAnim", out isAnim);
			Ease easeMode;
			argumentBox.Get<Ease>("easeMode", out easeMode);
			bool flag = easeMode == Ease.Unset;
			if (flag)
			{
				this.ResetMapCamera(isAnim);
			}
			else
			{
				this.ResetMapCamera(easeMode, isAnim);
			}
		}

		// Token: 0x06006ED2 RID: 28370 RVA: 0x00335958 File Offset: 0x00333B58
		private void OnSetCameraToArea(ArgumentBox argumentBox)
		{
			short areaId;
			argumentBox.Get("areaId", out areaId);
			this.SetCameraToArea(areaId);
		}

		// Token: 0x06006ED3 RID: 28371 RVA: 0x0033597C File Offset: 0x00333B7C
		private void OnSetCameraToLocation(ArgumentBox argumentBox)
		{
			Location location;
			argumentBox.Get<Location>("location", out location);
			bool doTween;
			argumentBox.Get("doTween", out doTween);
			TweenCallback tweenCallback;
			argumentBox.Get<TweenCallback>("tweenCallBack", out tweenCallback);
			float tweenTime;
			argumentBox.Get("tweenTime", out tweenTime);
			Ease ease;
			argumentBox.Get<Ease>("ease", out ease);
			this.MoveCameraTo(location, doTween, tweenCallback, tweenTime, ease);
		}

		// Token: 0x06006ED4 RID: 28372 RVA: 0x003359E0 File Offset: 0x00333BE0
		private void OnMoveTimeCostPercentChanged(ArgumentBox _)
		{
			bool flag = this._mapModel.MovePath.Count > 0;
			if (flag)
			{
				this.RerenderMovePath();
			}
		}

		// Token: 0x06006ED5 RID: 28373 RVA: 0x00335A0C File Offset: 0x00333C0C
		private void OnVisitedSettlementsChanged(ArgumentBox _)
		{
			bool playerAtBlockInitialized = this._playerAtBlockInitialized;
			if (playerAtBlockInitialized)
			{
				this.UpdateSettlementAndStationBtnUsable();
			}
		}

		// Token: 0x06006ED6 RID: 28374 RVA: 0x00335A2B File Offset: 0x00333C2B
		private void OnTaiwuGenderChanged(ArgumentBox _)
		{
			this.UpdatePlayerIconOnMap(false);
		}

		// Token: 0x06006ED7 RID: 28375 RVA: 0x00335A35 File Offset: 0x00333C35
		private void OnTaiwuInventoryChanged(ArgumentBox _)
		{
			this.RefreshCricketPlace(false);
		}

		// Token: 0x06006ED8 RID: 28376 RVA: 0x00335A3F File Offset: 0x00333C3F
		private void OnTaiwuCarrierChanged(ArgumentBox _)
		{
			this.UpdatePlayerIconOnMap(false);
		}

		// Token: 0x06006ED9 RID: 28377 RVA: 0x00335A49 File Offset: 0x00333C49
		private void AdventureRemakeDictChange(ArgumentBox _)
		{
			this.UpdateAdventureIcons();
		}

		// Token: 0x06006EDA RID: 28378 RVA: 0x00335A52 File Offset: 0x00333C52
		private void AdventureMajorEventChange(ArgumentBox _)
		{
			this.UpdateAdventureIcons();
		}

		// Token: 0x06006EDB RID: 28379 RVA: 0x00335A5B File Offset: 0x00333C5B
		private void OnStateAdventureDataReceived(ArgumentBox _)
		{
			this.UpdateAdventureIcons();
		}

		// Token: 0x06006EDC RID: 28380 RVA: 0x00335A64 File Offset: 0x00333C64
		private void OnMapBlockEnemyTogChange(ArgumentBox _)
		{
			this.RefreshAllRefreshMapBlockInfo();
		}

		// Token: 0x06006EDD RID: 28381 RVA: 0x00335A6D File Offset: 0x00333C6D
		private void OnForceRefreshAllMapBlock(ArgumentBox _)
		{
			this.RefreshAllRefreshMapBlockInfo();
		}

		// Token: 0x06006EDE RID: 28382 RVA: 0x00335A76 File Offset: 0x00333C76
		private void OnMapBlockFriendTogChange(ArgumentBox _)
		{
			this.RefreshAllRefreshMapBlockInfo();
		}

		// Token: 0x06006EDF RID: 28383 RVA: 0x00335A7F File Offset: 0x00333C7F
		private void OnMapBlockMerchantTogChange(ArgumentBox _)
		{
			this.UpdateAllMerchants();
		}

		// Token: 0x06006EE0 RID: 28384 RVA: 0x00335A88 File Offset: 0x00333C88
		private void OnMapBlockPastLifeRelationTogChange(ArgumentBox _)
		{
			this.RefreshAllRefreshMapBlockInfo();
		}

		// Token: 0x06006EE1 RID: 28385 RVA: 0x00335A91 File Offset: 0x00333C91
		private void OnMapAlterSettlementChanged(ArgumentBox argbox)
		{
			this.UpdateAlterEffects();
		}

		// Token: 0x06006EE2 RID: 28386 RVA: 0x00335A9A File Offset: 0x00333C9A
		private void OnMapCharacterChanged(ArgumentBox _)
		{
			this.UpdateMapCharacters();
		}

		// Token: 0x06006EE3 RID: 28387 RVA: 0x00335AA3 File Offset: 0x00333CA3
		private void OnBloodLightLocationsChanged(ArgumentBox _)
		{
			this.RefreshBloodLights();
		}

		// Token: 0x06006EE4 RID: 28388 RVA: 0x00335AAC File Offset: 0x00333CAC
		private void OnFairylandDataChanged(ArgumentBox _)
		{
			this.RefreshFairylands();
		}

		// Token: 0x06006EE5 RID: 28389 RVA: 0x00335AB5 File Offset: 0x00333CB5
		private void OnHeavenlyTreeDataChanged(ArgumentBox _)
		{
			this.RefreshHeavenTree();
		}

		// Token: 0x06006EE6 RID: 28390 RVA: 0x00335ABE File Offset: 0x00333CBE
		private void OnLingBaoLightLocationsChanged(ArgumentBox _)
		{
			this.RefreshLingbaoLight();
		}

		// Token: 0x06006EE7 RID: 28391 RVA: 0x00335AC7 File Offset: 0x00333CC7
		private void OnLingBaoDarkLocationsChanged(ArgumentBox _)
		{
			this.RefreshLingbaoDark();
		}

		// Token: 0x06006EE8 RID: 28392 RVA: 0x00335AD0 File Offset: 0x00333CD0
		private void OnBloodLocationsChanged(ArgumentBox _)
		{
			this.RefreshBloods();
		}

		// Token: 0x06006EE9 RID: 28393 RVA: 0x00335AD9 File Offset: 0x00333CD9
		private void OnFulongFlameLocationsChanged(ArgumentBox _)
		{
			this.RefreshFulongInFlameAreas();
		}

		// Token: 0x06006EEA RID: 28394 RVA: 0x00335AE2 File Offset: 0x00333CE2
		private void OnEmeiGuidanceDataChanged(ArgumentBox _)
		{
			this.RefreshEmeiDuidance();
		}

		// Token: 0x06006EEB RID: 28395 RVA: 0x00335AEC File Offset: 0x00333CEC
		private void OnDreamBackLocationsChanged(ArgumentBox _)
		{
			foreach (MapElementContainer container in this._activateMapElements.Values)
			{
				container.UpdateDreamBack();
			}
		}

		// Token: 0x06006EEC RID: 28396 RVA: 0x00335B48 File Offset: 0x00333D48
		private void OnFiveLoongDictChanged(ArgumentBox _)
		{
			this.UpdatePlayerIconOnMap(false);
		}

		// Token: 0x06006EED RID: 28397 RVA: 0x00335B54 File Offset: 0x00333D54
		private void OnProfessionDataChange(ArgumentBox argumentBox)
		{
			int professionId;
			argumentBox.Get("ProfessionId", out professionId);
			bool flag = professionId == 11;
			if (flag)
			{
				this.RefreshProfessionTravelerStationExistEffect();
				this.RefreshProfessionTravelerStationUpEffect();
			}
		}

		// Token: 0x06006EEE RID: 28398 RVA: 0x00335B8C File Offset: 0x00333D8C
		private void HandleNewFeatureUnlockHint(ArgumentBox argumentBox)
		{
			bool flag = GameApp.Instance.GetCurrentGameStateName() != EGameState.InGame;
			if (!flag)
			{
				bool flag2 = ViewNewFunctionUnlock.Queue.Count == 0;
				if (!flag2)
				{
					bool flag3 = UIManager.Instance.IsFocusElement(this.Element) && this.HandleNewfunction(argumentBox);
					if (!flag3)
					{
						ViewNewFunctionUnlock.DrainQueueAndNotifyHintShowed(false);
					}
				}
			}
		}

		// Token: 0x06006EEF RID: 28399 RVA: 0x00335BEC File Offset: 0x00333DEC
		private void HandleOpenWindow(ArgumentBox argumentBox)
		{
			bool disableMoving = ViewWorldMap._disableMoving;
			if (!disableMoving)
			{
				bool flag = SingletonObject.getInstance<WorldMapModel>().TaiwuMoveState == WorldMapModel.MoveState.WaitEventShow;
				if (!flag)
				{
					bool flag2 = this.HandleNewArea(argumentBox);
					if (!flag2)
					{
						bool flag3 = this.HandleProfessionSkillUnlock(argumentBox);
						if (!flag3)
						{
							bool flag4 = this.HandleAchievementPopup(argumentBox);
							if (!flag4)
							{
								bool flag5 = this.HandleNewfunction(argumentBox);
								if (!flag5)
								{
									bool flag6 = this.HandleLifeSkillCombatStrategyUnlock(argumentBox);
									if (flag6)
									{
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06006EF0 RID: 28400 RVA: 0x00335C60 File Offset: 0x00333E60
		private bool HandleProfessionSkillUnlock(ArgumentBox argumentBox)
		{
			bool flag = !UIManager.Instance.IsFocusElement(this.Element);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool ignoreProfessionSkillUnlockAnimation = GMFunc.IgnoreProfessionSkillUnlockAnimation;
				if (ignoreProfessionSkillUnlockAnimation)
				{
					result = false;
				}
				else
				{
					bool flag2 = !UI_ProfessionSkillUnlocked.HasProfessionSkillUnlockedToShow();
					if (flag2)
					{
						result = false;
					}
					else
					{
						bool flag3 = SingletonObject.getInstance<WorldMapModel>().TaiwuMoveState == WorldMapModel.MoveState.WaitEventShow;
						if (flag3)
						{
							result = false;
						}
						else
						{
							ViewWorldMap.SetDisableMoving(true);
							this.StopMove();
							SingletonObject.getInstance<YieldHelper>().StartCoroutine(this.DelayTryShowProfessionSkillUnlocked());
							result = true;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06006EF1 RID: 28401 RVA: 0x00335CE1 File Offset: 0x00333EE1
		private IEnumerator DelayTryShowProfessionSkillUnlocked()
		{
			yield return new WaitForSeconds(0.5f);
			this.StopMove();
			bool flag = GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame;
			if (flag)
			{
				ExtraDomainMethod.AsyncCall.CanShowProfessionSkillUnlocked(this, delegate(int offset, RawDataPool dataPool)
				{
					bool flag2 = false;
					Serializer.Deserialize(dataPool, offset, ref flag2);
					bool flag3 = flag2 && UIManager.Instance.IsFocusElement(this.Element);
					if (flag3)
					{
						UIManager.Instance.MaskUI(UIElement.ProfessionSkillUnlocked);
					}
					ViewWorldMap.SetDisableMoving(false);
				});
			}
			else
			{
				ViewWorldMap.SetDisableMoving(false);
			}
			yield break;
		}

		// Token: 0x06006EF2 RID: 28402 RVA: 0x00335CF0 File Offset: 0x00333EF0
		private IEnumerator DelayTryShowNewFunctionUnlocked()
		{
			yield return new WaitForSeconds(0.5f);
			this.StopMove();
			ViewWorldMap.SetDisableMoving(false);
			bool advancingMonth = GameApp.AdvancingMonth;
			if (advancingMonth)
			{
				yield break;
			}
			bool flag = !UIManager.Instance.IsFocusElement(this.Element);
			if (flag)
			{
				bool flag2 = !UIElement.NewFunctionUnlock.Exist;
				if (flag2)
				{
					ViewNewFunctionUnlock.DrainQueueAndNotifyHintShowed(false);
				}
				yield break;
			}
			UIManager.Instance.ShowUI(UIElement.NewFunctionUnlock, true);
			yield break;
		}

		// Token: 0x06006EF3 RID: 28403 RVA: 0x00335CFF File Offset: 0x00333EFF
		private IEnumerator DelayTryShowArriveArea()
		{
			yield return new WaitForSeconds(0.1f);
			bool flag = !UIManager.Instance.IsFocusElement(this.Element);
			if (flag)
			{
				yield break;
			}
			UIManager.Instance.ShowUI(UIElement.NewAreaNotify, true);
			yield break;
		}

		// Token: 0x06006EF4 RID: 28404 RVA: 0x00335D10 File Offset: 0x00333F10
		private bool HandleLifeSkillCombatStrategyUnlock(ArgumentBox argumentBox)
		{
			bool flag = !this.<HandleLifeSkillCombatStrategyUnlock>g__Check|206_1();
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				ViewWorldMap.SetDisableMoving(true);
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(10U, delegate
				{
					bool flag2 = !this.<HandleLifeSkillCombatStrategyUnlock>g__Check|206_1();
					if (flag2)
					{
						ViewWorldMap.SetDisableMoving(false);
					}
					else
					{
						ExtraDomainMethod.AsyncCall.CanShowProfessionSkillUnlocked(this, delegate(int offset, RawDataPool dataPool)
						{
							bool flag3 = false;
							Serializer.Deserialize(dataPool, offset, ref flag3);
							bool flag4 = flag3 && this.<HandleLifeSkillCombatStrategyUnlock>g__Check|206_1();
							if (flag4)
							{
								TaiwuDomainMethod.AsyncCall.GetNewUnlockedDebateStrategyList(this, delegate(int offset, RawDataPool dataPool)
								{
									List<short> list = null;
									Serializer.Deserialize(dataPool, offset, ref list);
									bool flag5 = list != null && list.Count > 0 && list.ContentIsDifferent(this._unlockedDebateStrategyList);
									if (flag5)
									{
										this._unlockedDebateStrategyList.Clear();
										this._unlockedDebateStrategyList.AddRange(list);
										ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
										argBox.SetObject("UnlockedDebateStrategyList", list);
										UIElement.GetItem.SetOnInitArgs(argBox);
										UIManager.Instance.MaskUI(UIElement.GetItem);
										GlobalDomainMethod.Call.InvokeGuidingTrigger(296);
									}
									ViewWorldMap.SetDisableMoving(false);
								});
							}
							else
							{
								ViewWorldMap.SetDisableMoving(false);
							}
						});
					}
				});
				result = true;
			}
			return result;
		}

		// Token: 0x06006EF5 RID: 28405 RVA: 0x00335D54 File Offset: 0x00333F54
		private bool HandleNewfunction(ArgumentBox argumentBox)
		{
			bool flag = GameApp.Instance.GetCurrentGameStateName() != EGameState.InGame;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !UIManager.Instance.IsFocusElement(this.Element);
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = ViewNewFunctionUnlock.Queue.Count == 0;
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool advancingMonth = GameApp.AdvancingMonth;
						if (advancingMonth)
						{
							result = false;
						}
						else
						{
							ViewWorldMap.SetDisableMoving(true);
							this.StopMove();
							SingletonObject.getInstance<YieldHelper>().StartCoroutine(this.DelayTryShowNewFunctionUnlocked());
							result = true;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06006EF6 RID: 28406 RVA: 0x00335DE0 File Offset: 0x00333FE0
		private bool HandleNewArea(ArgumentBox argumentBox)
		{
			bool flag = !ViewNewAreaNotify.CheckNeedShow();
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = GameApp.Instance.GetCurrentGameStateName() != EGameState.InGame;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = !UIManager.Instance.IsFocusElement(this.Element);
					if (flag3)
					{
						result = false;
					}
					else
					{
						SingletonObject.getInstance<YieldHelper>().StartCoroutine(this.DelayTryShowArriveArea());
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06006EF7 RID: 28407 RVA: 0x00335E48 File Offset: 0x00334048
		private bool HandleAchievementPopup(ArgumentBox argumentBox)
		{
			bool flag = GameApp.Instance.GetCurrentGameStateName() != EGameState.InGame;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = SingletonObject.getInstance<BasicGameData>().AdvancingMonthState != 0;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = SingletonObject.getInstance<BasicGameData>().ToPopupAchievements.Count <= 0;
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = UIManager.Instance.IsElementActive(UIElement.GetItem);
						if (flag4)
						{
							result = false;
						}
						else
						{
							UIElement.AchievementPopUp.Show();
							result = true;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06006EF8 RID: 28408 RVA: 0x00335ECC File Offset: 0x003340CC
		private void RefreshProfessionTravelerEffect()
		{
			this.RefreshProfessionTravelerStationExistEffect();
			this.RefreshProfessionTravelerStationUpEffect();
			bool flag = this._lastBlockId != this._mapModel.CurrentBlockId;
			if (flag)
			{
				this.RefreshProfessionTravelerStationDownEffect();
			}
		}

		// Token: 0x06006EF9 RID: 28409 RVA: 0x00335F0C File Offset: 0x0033410C
		private void OnMapBlockDisplayDataChanged(ArgumentBox _)
		{
			bool flag = this._mapModel.ViewMode == WorldMapModel.EViewMode.Info;
			if (flag)
			{
				this.ShowNegativeFilm();
			}
			this.UpdateAllContainers();
		}

		// Token: 0x06006EFA RID: 28410 RVA: 0x00335F3C File Offset: 0x0033413C
		private void MapFocusLocationGrave(ArgumentBox argBox)
		{
			Location location;
			argBox.Get<Location>("location", out location);
			int togKey;
			argBox.Get("togKey", out togKey);
			this.MapFocusLocationGrave(location, togKey);
			this.GetElementContainer(location).UpdateTemporaryMark();
		}

		// Token: 0x06006EFB RID: 28411 RVA: 0x00335F7C File Offset: 0x0033417C
		private void MapClearLocationTemporaryMark(ArgumentBox argBox)
		{
			Location location;
			argBox.Get<Location>("location", out location);
			this.GetElementContainer(location).UpdateTemporaryMark();
		}

		// Token: 0x06006EFC RID: 28412 RVA: 0x00335FA8 File Offset: 0x003341A8
		private void MapAddLocationsToTemporaryMarkList(ArgumentBox argBox)
		{
			List<Location> locations;
			argBox.Get<List<Location>>("locations", out locations);
			foreach (Location location in locations)
			{
				MapElementContainer container = this.GetElementContainer(location);
				bool flag = container != null;
				if (flag)
				{
					container.UpdateTemporaryMark();
				}
			}
		}

		// Token: 0x06006EFD RID: 28413 RVA: 0x0033601C File Offset: 0x0033421C
		private void MapClearAllTemporaryMarkList(ArgumentBox _)
		{
			foreach (MapElementContainer container in this._activateMapElements.Values)
			{
				container.UpdateTemporaryMark();
			}
		}

		// Token: 0x06006EFE RID: 28414 RVA: 0x00336078 File Offset: 0x00334278
		private void MapAddLocationsToTemporaryMarkListForTask(ArgumentBox argBox)
		{
			List<Location> locations;
			argBox.Get<List<Location>>("locations", out locations);
			foreach (Location location in locations)
			{
				MapElementContainer container = this.GetElementContainer(location);
				bool flag = container != null;
				if (flag)
				{
					container.UpdateTemporaryMark();
				}
			}
		}

		// Token: 0x06006EFF RID: 28415 RVA: 0x003360EC File Offset: 0x003342EC
		private void MapClearAllTemporaryMarkListForTask(ArgumentBox _)
		{
			foreach (MapElementContainer container in this._activateMapElements.Values)
			{
				container.UpdateTemporaryMark();
			}
		}

		// Token: 0x06006F00 RID: 28416 RVA: 0x00336148 File Offset: 0x00334348
		private void MapCurrentLocationFixedCharacterDataReceived(ArgumentBox _)
		{
			this.RefreshPlayerVisible();
		}

		// Token: 0x06006F01 RID: 28417 RVA: 0x00336154 File Offset: 0x00334354
		private void OnEventHandleComplete(ArgumentBox argbox)
		{
			bool flag = !this._hotkeyMoveReady;
			if (flag)
			{
				this._hotkeyMoveReady = true;
			}
			bool flag2 = this._pathMoving && this._mapModel.MovePath.Count == 0;
			if (flag2)
			{
				this._pathMoving = false;
			}
		}

		// Token: 0x06006F02 RID: 28418 RVA: 0x003361A0 File Offset: 0x003343A0
		private void VillagerWorkDataChange(ArgumentBox _)
		{
			foreach (MapElementContainer container in this._activateMapElements.Values)
			{
				container.UpdateAdventure();
			}
		}

		// Token: 0x06006F03 RID: 28419 RVA: 0x003361FC File Offset: 0x003343FC
		private void WorldMapDoShakeByEvent(ArgumentBox argBox)
		{
			base.StartCoroutine(this.CoShakeMapByEvent(argBox));
		}

		// Token: 0x06006F04 RID: 28420 RVA: 0x0033620C File Offset: 0x0033440C
		private void WorldMapDoFocusByEvent(ArgumentBox argBox)
		{
			base.StartCoroutine(this.CoFocusMapByEvent(argBox));
		}

		// Token: 0x06006F05 RID: 28421 RVA: 0x0033621C File Offset: 0x0033441C
		private void OnClickMapElement(ArgumentBox argBox)
		{
			bool flag = this._pathMoving || !UIManager.Instance.IsFocusElement(this.Element);
			if (!flag)
			{
				Location location;
				MapBlockData blockData;
				bool flag2 = !argBox.Get<Location>("Location", out location) || !this._mapModel.TryGetBlockData(location, out blockData);
				if (!flag2)
				{
					this.OnBlockClick(blockData);
				}
			}
		}

		// Token: 0x06006F06 RID: 28422 RVA: 0x00336280 File Offset: 0x00334480
		private void OnHoverMapElement(ArgumentBox argBox)
		{
			bool flag = !UIManager.Instance.IsFocusElement(this.Element);
			if (!flag)
			{
				Location location;
				MapBlockData blockData;
				bool flag2 = !argBox.Get<Location>("Location", out location) || !this._mapModel.TryGetBlockData(location, out blockData);
				if (!flag2)
				{
					bool flag3 = blockData.AreaId != this._mapModel.CurrentAreaId;
					if (!flag3)
					{
						this._selectVirtual.anchoredPosition = MapRenderSystem.GetBlockLocalPos(location);
						this._selectVirtual.gameObject.SetActive(true);
						ConchShipCursor.Instance.TrySetClickableCursor();
					}
				}
			}
		}

		// Token: 0x06006F07 RID: 28423 RVA: 0x0033631D File Offset: 0x0033451D
		private void OnHoverExitMapElement(ArgumentBox argBox)
		{
			this._selectVirtual.gameObject.SetActive(false);
			ConchShipCursor.Instance.TrySetDefaultCursor();
		}

		// Token: 0x06006F08 RID: 28424 RVA: 0x0033633D File Offset: 0x0033453D
		private void RefreshMapPickups(ArgumentBox _)
		{
			this.RefreshMapPickups();
		}

		// Token: 0x06006F09 RID: 28425 RVA: 0x00336348 File Offset: 0x00334548
		private void RefreshMapPickups()
		{
			foreach (MapElementContainer container in this._activateMapElements.Values)
			{
				container.UpdatePickup();
			}
		}

		// Token: 0x06006F0A RID: 28426 RVA: 0x003363A4 File Offset: 0x003345A4
		private void RefreshMapPickupEffect(ArgumentBox _)
		{
			foreach (MapElementContainer container in this._activateMapElements.Values)
			{
				container.UpdatePickupEffect();
			}
		}

		// Token: 0x06006F0B RID: 28427 RVA: 0x00336400 File Offset: 0x00334600
		private void RefreshCricketWishEffect(ArgumentBox _)
		{
			foreach (MapElementContainer container in this._activateMapElements.Values)
			{
				container.UpdateCricketWishEffect();
			}
		}

		// Token: 0x06006F0C RID: 28428 RVA: 0x0033645C File Offset: 0x0033465C
		private void OnClickWorldMapBlock(ArgumentBox argBox)
		{
			MapBlockData mapBlockData;
			argBox.Get<MapBlockData>("mapBlockData", out mapBlockData);
			this.OnBlockClick(mapBlockData);
		}

		// Token: 0x06006F0D RID: 28429 RVA: 0x00336480 File Offset: 0x00334680
		private void OnReshowBlockList(ArgumentBox argBox)
		{
			bool isPlayerAtBlock;
			argBox.Get("isPlayerAtBlock", out isPlayerAtBlock);
			this.ReshowBlockList(isPlayerAtBlock);
		}

		// Token: 0x06006F0E RID: 28430 RVA: 0x003364A4 File Offset: 0x003346A4
		private void HandlerMethodReturn(NotificationWrapper wrapper, Notification notification)
		{
			ushort domainId = notification.DomainId;
			ushort num = domainId;
			if (num != 2)
			{
				if (num == 4)
				{
					if (notification.MethodId == 28)
					{
						int sweepNetAmount = 0;
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref sweepNetAmount);
						this.HandlerMethodGetInventoryItemAmount(sweepNetAmount);
					}
				}
			}
			else if (notification.MethodId != 9)
			{
				if (notification.MethodId == 18)
				{
					bool catchRealCricket = false;
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref catchRealCricket);
					this.HandlerMethodTryTriggerCricketCatch(catchRealCricket);
				}
			}
			else
			{
				bool breakingMoving = false;
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref breakingMoving);
				this.HandlerMethodIsContinuousMovingBreak(breakingMoving);
			}
		}

		// Token: 0x06006F0F RID: 28431 RVA: 0x00336554 File Offset: 0x00334754
		private void HandlerMethodGetInventoryItemAmount(int sweepNetAmount)
		{
			string[] param = new string[]
			{
				LocalStringManager.GetFormat(LanguageKey.LK_WorldMap_SweepNet_Amount, sweepNetAmount)
			};
			GEvent.OnEvent(UiEvents.OnSweepNetAmountTipsChanged, EasyPool.Get<ArgumentBox>().SetObject("tips", param));
			RectTransform catchCricket = this.rectTsCatchCricket;
			catchCricket.gameObject.SetActive(false);
			bool catchCricketShouldActive = this.CatchCricketShouldActive;
			if (catchCricketShouldActive)
			{
				catchCricket.anchoredPosition = MapRenderSystem.GetBlockLocalPos(this._mapModel.CurrentLocation);
				catchCricket.gameObject.SetActive(true);
				Button button = catchCricket.GetChild(0).GetComponent<Button>();
				button.interactable = (sweepNetAmount > 0);
				string shouldActiveName = button.interactable ? "Active" : "InActive";
				CricketPlaceExtraData cricketPlaceExtraData;
				short num;
				bool flag = this._mapModel.CricketPlaceExtraData.TryGetValue(this._mapModel.CurrentAreaId, out cricketPlaceExtraData) && cricketPlaceExtraData != null && cricketPlaceExtraData.ExtraMapUnits != null && cricketPlaceExtraData.ExtraMapUnits.TryGetValue(this._mapModel.CurrentBlockId, out num) && !cricketPlaceExtraData.IsRegularCricket(this._mapModel.CurrentBlockId);
				if (flag)
				{
					bool isWishing = cricketPlaceExtraData.WishingCrickets != null && cricketPlaceExtraData.WishingCrickets.ContainsKey(this._mapModel.CurrentBlockId);
					shouldActiveName += (isWishing ? "Wishing" : "Duke");
				}
				foreach (object obj in button.transform)
				{
					Transform bt = (Transform)obj;
					bt.gameObject.SetActive(bt.name == shouldActiveName);
				}
				button.GetComponent<TooltipInvoker>().PresetParam = param;
				List<MapBlockData> areaBlocks = EasyPool.Get<List<MapBlockData>>();
				this._mapModel.GetAreaBlocks(this._mapModel.CurrentAreaId, areaBlocks);
				this.UpdateSingleCricket(this._mapModel.CurrentLocation);
				EasyPool.Free<List<MapBlockData>>(areaBlocks);
			}
		}

		// Token: 0x06006F10 RID: 28432 RVA: 0x0033676C File Offset: 0x0033496C
		private void HandlerMethodIsContinuousMovingBreak(bool breakingMoving)
		{
			this._mapModel.ChangeTaiwuMoveState(breakingMoving ? WorldMapModel.MoveState.WaitEventShow : WorldMapModel.MoveState.Idle);
			bool flag = this._continuousMovingCallback != null;
			if (flag)
			{
				bool breakingMoving2 = breakingMoving;
				if (breakingMoving2)
				{
					this.OnMoveFinished();
					this.ResetSelectedBlock();
				}
				else
				{
					this._continuousMovingCallback();
				}
				this._continuousMovingCallback = null;
			}
			else
			{
				this.OnMoveFinished();
				this.RerenderMovePath();
			}
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
			{
				bool flag2 = !breakingMoving;
				if (flag2)
				{
					this._hotkeyMoveReady = true;
				}
			});
		}

		// Token: 0x06006F11 RID: 28433 RVA: 0x00336810 File Offset: 0x00334A10
		private void HandlerMethodTryTriggerCricketCatch(bool catchRealCricket)
		{
			if (catchRealCricket)
			{
				SingletonObject.getInstance<WorldMapModel>().ChangeTaiwuMoveState(WorldMapModel.MoveState.WaitEventShow);
				UIElement catchCricket = UIElement.CatchCricket;
				catchCricket.OnHide = (Action)Delegate.Combine(catchCricket.OnHide, new Action(delegate()
				{
					ViewWorldMap.SetDisableMoving(false);
				}));
				float originScale = this.moveRoot.localScale.x;
				float targetScale = ViewWorldMap.GetCricketCatchTransitionTargetScale(originScale);
				WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
				EMapAreaAreaDirection areaDirection = mapModel.Areas[(int)mapModel.CurrentAreaId].GetConfig().AreaDirection;
				this.PlayAdventureOpenPartOneTransition(originScale, targetScale, delegate
				{
					AudioManager.Instance.PlaySound("CCricket_netwhoosh", false, false);
					ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
					argBox.Set("AreaType", (sbyte)areaDirection);
					bool flag = this._wishingCricketId >= 0;
					if (flag)
					{
						argBox.Set("WishingCricketId", this._wishingCricketId);
					}
					UIElement.CatchCricket.SetOnInitArgs(argBox);
					UIManager.Instance.ShowUI(UIElement.CatchCricket, true);
					this.RestoreAdventurePartOneTransitionImmediately(originScale);
				});
			}
			else
			{
				ViewWorldMap.SetDisableMoving(false);
			}
		}

		// Token: 0x06006F12 RID: 28434 RVA: 0x003368E4 File Offset: 0x00334AE4
		private void EventTriggerCricketCatch(ArgumentBox argBox)
		{
			float originScale = this.moveRoot.localScale.x;
			float targetScale = ViewWorldMap.GetCricketCatchTransitionTargetScale(originScale);
			EMapAreaAreaDirection areaDirection = this._mapModel.Areas[(int)this._mapModel.CurrentAreaId].GetConfig().AreaDirection;
			this.PlayAdventureOpenPartOneTransition(originScale, targetScale, delegate
			{
				ArgumentBox args = EasyPool.Get<ArgumentBox>();
				args.Set("AreaType", (sbyte)areaDirection);
				UIElement.CatchCricket.SetOnInitArgs(args);
				UIManager.Instance.ShowUI(UIElement.CatchCricket, true);
				this.RestoreAdventurePartOneTransitionImmediately(originScale);
			});
		}

		// Token: 0x06006F13 RID: 28435 RVA: 0x00336964 File Offset: 0x00334B64
		private void InitMap()
		{
			this._mapModel.SelectedBlockId = -1;
			this._lastBlockId = -1;
			this.ResetClickReceiverOffset();
			RectTransform catchCricket = this.rectTsCatchCricket;
			catchCricket.gameObject.SetActive(false);
			base.StartCoroutine(this.ResetMapRenderer());
		}

		// Token: 0x06006F14 RID: 28436 RVA: 0x003369B0 File Offset: 0x00334BB0
		private void ResetClickReceiverOffset()
		{
			short showingAreaId = this._mapModel.ShowingAreaId;
			byte showingAreaSize = this._mapModel.GetAreaSize(showingAreaId);
			this.MapClickReceiver.SetMapSize((int)showingAreaSize);
			for (int i = 0; i < 9; i++)
			{
				short areaId = this._mapModel.GetAreaIdByStateIndex(i);
				bool flag = areaId == 139 || areaId == 140;
				if (flag)
				{
					this.MapClickReceiver.SetOffset(MapRenderSystem.AreaOffset[0]);
				}
				else
				{
					bool flag2 = this._mapModel.ShowingAreaId == areaId;
					if (flag2)
					{
						this.MapClickReceiver.SetOffset(MapRenderSystem.AreaOffset[i]);
					}
				}
			}
		}

		// Token: 0x06006F15 RID: 28437 RVA: 0x00336A67 File Offset: 0x00334C67
		private IEnumerator ResetMapRenderer()
		{
			short showingAreaId = this._mapModel.ShowingAreaId;
			byte showingAreaSize = this._mapModel.GetAreaSize(showingAreaId);
			List<MapBlockData> areaBlocks = EasyPool.Get<List<MapBlockData>>();
			this._mapModel.GetAreaBlocks(showingAreaId, areaBlocks);
			this.RefreshAllSectStoryEffects();
			MapRenderSystem mapRenderController = SingletonObject.getInstance<MapRenderSystem>();
			Dictionary<MapRenderSystem.EMapLayer, MapRawImage> mapLayers = new Dictionary<MapRenderSystem.EMapLayer, MapRawImage>();
			mapLayers.Add(MapRenderSystem.EMapLayer.MapBlock, this.MapRawImage);
			mapLayers.Add(MapRenderSystem.EMapLayer.MapBlockShadow, this.MapRawImageShadow);
			mapLayers.Add(MapRenderSystem.EMapLayer.BlockBaseShadow, this.mapRawImageBaseShadow);
			mapLayers.Add(MapRenderSystem.EMapLayer.BlockBaseAndBorder, this.mapRawImageBlockBaseAndBorder);
			mapLayers.Add(MapRenderSystem.EMapLayer.BlockPreparation, this.mapRawImagePreparation);
			WorldMapModel.MapBlockRenderFinish = false;
			this.background.SetTexture(this.GetPreferBackgroundName());
			Coroutine renderCoroutine = base.StartCoroutine(mapRenderController.InitAndRenderMap((int)showingAreaSize, areaBlocks, mapLayers));
			this.MapRawImage.transform.localPosition = MapRenderSystem.CalcDefaultMapLayerPosition(showingAreaId);
			yield return renderCoroutine;
			MapRenderSystem mapRenderSystem = SingletonObject.getInstance<MapRenderSystem>();
			mapRenderSystem.RefreshAllMapBlocks(true);
			this.RefreshAllBlockVisibility();
			WorldMapModel.MapBlockRenderFinish = true;
			yield break;
		}

		// Token: 0x06006F16 RID: 28438 RVA: 0x00336A78 File Offset: 0x00334C78
		private string GetPreferBackgroundName()
		{
			short showingAreaId = this._mapModel.ShowingAreaId;
			MapAreaData areaData = this._mapModel.Areas[(int)showingAreaId];
			EMapAreaAreaDirection direction = areaData.GetConfig().AreaDirection;
			return ViewWorldMap.AreaBackgrounds[direction];
		}

		// Token: 0x06006F17 RID: 28439 RVA: 0x00336ABB File Offset: 0x00334CBB
		private IEnumerator RefreshMapBlock()
		{
			WorldMapModel.MapBlockLoadFinish = false;
			this._mapBlockSet.Clear();
			this.ClearSectStoryEffects();
			int totalBlockCount = 0;
			int blockCounter = 0;
			int totalProgress = 40;
			int currProgress = 50;
			bool flag = GameApp.Instance.GetCurrentGameStateName() == EGameState.Loading;
			int num;
			if (flag)
			{
				for (int i = 0; i < 9; i = num + 1)
				{
					bool flag2 = this._mapModel.CurrentStateId < 0 && i > 0;
					if (flag2)
					{
						break;
					}
					short areaId = this._mapModel.GetAreaIdByStateIndex(i);
					int areaSize = (int)this._mapModel.GetAreaSize(areaId);
					totalBlockCount += areaSize * areaSize;
					num = i;
				}
				bool flag3 = this._mapModel.CurrentStateId >= 0;
				if (flag3)
				{
					totalBlockCount += 150;
				}
			}
			List<MapBlockData> blockDataList = EasyPool.Get<List<MapBlockData>>();
			for (int j = 0; j < 9; j = num + 1)
			{
				bool flag4 = this._mapModel.CurrentStateId < 0 && j > 0;
				if (flag4)
				{
					break;
				}
				short areaId2 = this._mapModel.GetAreaIdByStateIndex(j);
				this._mapModel.GetAreaBlocks(areaId2, blockDataList);
				blockDataList.Sort(delegate(MapBlockData blockL, MapBlockData blockR)
				{
					ByteCoordinate posL = blockL.GetBlockPos();
					ByteCoordinate posR = blockR.GetBlockPos();
					MapBlockItem configL = blockL.GetConfig();
					MapBlockItem configR = blockR.GetConfig();
					byte sizeL = (blockL.RootBlockId < 0 && configL != null) ? configL.Size : 1;
					byte sizeR = (blockR.RootBlockId < 0 && configR != null) ? configR.Size : 1;
					bool flag12 = sizeL > 1;
					if (flag12)
					{
						posL.X = posL.X + sizeL - 1;
					}
					bool flag13 = sizeR > 1;
					if (flag13)
					{
						posR.X = posR.X + sizeR - 1;
					}
					int mapYl = (int)(posL.Y - posL.X);
					int mapYr = (int)(posR.Y - posR.X);
					bool flag14 = sizeL > 1;
					if (flag14)
					{
						mapYl += (int)sizeL;
					}
					bool flag15 = sizeR > 1;
					if (flag15)
					{
						mapYr += (int)sizeR;
					}
					bool flag16 = mapYl != mapYr;
					int result;
					if (flag16)
					{
						result = mapYr - mapYl;
					}
					else
					{
						result = (int)(sizeL - sizeR);
					}
					return result;
				});
				foreach (MapBlockData blockData in blockDataList)
				{
					MapBlockItem blockConfig = blockData.GetConfig();
					int blockSize = (int)((blockData.RootBlockId < 0 && blockConfig != null) ? blockConfig.Size : 1);
					string poolKey = this._mapBlockPrefabKey[blockSize - 1];
					ByteCoordinate blockPos = blockData.GetBlockPos();
					Vector2 refersPos = new Vector2((float)(blockPos.X + blockPos.Y) * MapRenderSystem.MapBlockPosInterval.x, (float)(blockPos.Y - blockPos.X) * MapRenderSystem.MapBlockPosInterval.y);
					bool flag5 = blockSize > 1;
					if (flag5)
					{
						refersPos.x += (float)(blockSize - 1) * MapRenderSystem.MapBlockPosInterval.x;
						refersPos.y -= (float)(blockSize - 1) * MapRenderSystem.MapBlockPosInterval.y;
					}
					bool flag6 = blockData.TemplateId != 126;
					if (flag6)
					{
						Location location = new Location(areaId2, blockData.BlockId);
						this._mapBlockSet.Add(location);
						location = default(Location);
					}
					num = blockCounter;
					blockCounter = num + 1;
					bool flag7 = GameApp.Instance.GetCurrentGameStateName() == EGameState.Loading;
					if (flag7)
					{
						int progress = 50 + totalProgress * CValuePercent.Parse(blockCounter, totalBlockCount);
						bool flag8 = currProgress != progress;
						if (flag8)
						{
							currProgress = progress;
							GEvent.OnEvent(EEvents.LoadingProgress, EasyPool.Get<ArgumentBox>().Set("Progress", progress));
						}
					}
					bool flag9 = blockCounter % 30 == 0;
					if (flag9)
					{
						yield return null;
					}
					blockConfig = null;
					blockPos = default(ByteCoordinate);
					refersPos = default(Vector2);
					blockData = null;
				}
				List<MapBlockData>.Enumerator enumerator = default(List<MapBlockData>.Enumerator);
				num = j;
			}
			short currentAreaId = this._mapModel.CurrentAreaId;
			short num2 = currentAreaId;
			short num3 = num2;
			if (num3 - 135 > 5)
			{
				bool flag10 = this._mapModel.CurrentStateId >= 0;
				if (flag10)
				{
					for (int k = 0; k < 9; k = num + 1)
					{
						ViewWorldMap.<>c__DisplayClass241_0 CS$<>8__locals1 = new ViewWorldMap.<>c__DisplayClass241_0();
						CS$<>8__locals1.<>4__this = this;
						CS$<>8__locals1.areaId = this._mapModel.GetAreaIdByStateIndex(k);
						this._mapModel.GetAreaBlocks(CS$<>8__locals1.areaId, blockDataList);
						blockDataList.RemoveAll((MapBlockData block) => !CS$<>8__locals1.<>4__this._mapModel.IsEdgeBlock(new Location(CS$<>8__locals1.areaId, block.BlockId)));
						bool flag11 = this._mapModel.ShowingAreaId == CS$<>8__locals1.areaId;
						if (flag11)
						{
							this.MapClickReceiver.SetOffset(MapRenderSystem.AreaOffset[k]);
						}
						CS$<>8__locals1 = null;
						num = k;
					}
				}
				else
				{
					this.MapClickReceiver.SetOffset(MapRenderSystem.AreaOffset[0]);
				}
			}
			else
			{
				this._mapModel.GetAreaBlocks(this._mapModel.CurrentAreaId, blockDataList);
				blockDataList.RemoveAll((MapBlockData block) => !this._mapModel.IsEdgeBlock(new Location(this._mapModel.CurrentAreaId, block.BlockId)));
				this.MapClickReceiver.SetOffset(MapRenderSystem.AreaOffset[0]);
			}
			EasyPool.Free<List<MapBlockData>>(blockDataList);
			WorldMapModel.MapBlockLoadFinish = true;
			bool mapBlockUiLoadFinish = WorldMapModel.MapBlockUiLoadFinish;
			if (mapBlockUiLoadFinish)
			{
				this._mapModel.OnMapInitFinish();
			}
			this.ResetClickReceiverOffset();
			base.StartCoroutine(this.ResetMapRenderer());
			yield break;
			yield break;
		}

		// Token: 0x06006F18 RID: 28440 RVA: 0x00336ACA File Offset: 0x00334CCA
		private IEnumerator RefreshAllBlock()
		{
			this.ResetClickReceiverOffset();
			yield return this.ResetMapRenderer();
			yield break;
		}

		// Token: 0x06006F19 RID: 28441 RVA: 0x00336ADC File Offset: 0x00334CDC
		private void MapFocusLocationGrave(Location location, int togKey)
		{
			Location taiwuLocation = this._mapModel.CurrentLocation;
			bool flag = this._mapModel.ShowingAreaId != location.AreaId;
			if (flag)
			{
				this.SetShowingArea(location.AreaId);
				this.ScaleView(0.25f);
				this.MoveCameraTo(location, false, null, 0.2f, Ease.Unset);
				DOVirtual.Float(0.25f, 0.625f, 1f, new TweenCallback<float>(this.ScaleView)).SetEase(Ease.OutQuad);
			}
			else
			{
				bool flag2 = taiwuLocation.IsValid() && !taiwuLocation.Equals(location);
				if (flag2)
				{
					this.MoveCameraTo(taiwuLocation, false, null, 0.2f, Ease.Unset);
					this.MoveCameraTo(location, true, null, 1f, Ease.OutQuad);
				}
				else
				{
					this.MoveCameraTo(location, true, null, 0.2f, Ease.OutQuad);
				}
			}
			this.SelectBlock(this._mapModel.GetBlockData(location));
			GEvent.OnEvent(UiEvents.OnSetMapBlockCharListTog, EasyPool.Get<ArgumentBox>().Set("TogKey", togKey));
		}

		// Token: 0x06006F1A RID: 28442 RVA: 0x00336BE8 File Offset: 0x00334DE8
		public void PlayCricketWishPerformance(Location location, Action onComplete = null)
		{
			MapRenderSystem renderSystem = SingletonObject.getInstance<MapRenderSystem>();
			renderSystem.SetMapBlockLightAtLocation(location);
			TweenCallback <>9__1;
			this.FocusLocationForCricketWish(location, delegate
			{
				this.ShowCricketWishEffect(location);
				AudioManager.Instance.PlaySound("CCricket_Wish", false, false);
				float delay = 1.5f;
				TweenCallback callback;
				if ((callback = <>9__1) == null)
				{
					callback = (<>9__1 = delegate()
					{
						renderSystem.ClearMapBlockAdditionalLightState(false);
						Action onComplete2 = onComplete;
						if (onComplete2 != null)
						{
							onComplete2();
						}
					});
				}
				DOVirtual.DelayedCall(delay, callback, true).SetUpdate(true).SetAutoKill(true);
			});
		}

		// Token: 0x06006F1B RID: 28443 RVA: 0x00336C48 File Offset: 0x00334E48
		private void FocusLocationForCricketWish(Location location, TweenCallback onComplete)
		{
			Location taiwuLocation = this._mapModel.CurrentLocation;
			bool flag = this._mapModel.ShowingAreaId != location.AreaId;
			if (flag)
			{
				this.SetShowingArea(location.AreaId);
				this.ScaleView(0.25f);
				this.MoveCameraTo(location, false, null, 0.2f, Ease.Unset);
				DOVirtual.Float(0.25f, 0.625f, 1f, new TweenCallback<float>(this.ScaleView)).SetEase(Ease.OutQuad).SetAutoKill(true).OnComplete(onComplete);
			}
			else
			{
				bool flag2 = taiwuLocation.IsValid() && !taiwuLocation.Equals(location);
				if (flag2)
				{
					this.MoveCameraTo(taiwuLocation, false, null, 0.2f, Ease.Unset);
					this.MoveCameraTo(location, true, onComplete, 1f, Ease.OutQuad);
				}
				else
				{
					this.MoveCameraTo(location, true, onComplete, 0.2f, Ease.OutQuad);
				}
			}
			this.SelectBlock(this._mapModel.GetBlockData(location));
		}

		// Token: 0x06006F1C RID: 28444 RVA: 0x00336D40 File Offset: 0x00334F40
		private void ShowCricketWishEffect(Location location)
		{
			bool flag = this._mapModel.CricketWishEffectLocations.Contains(location);
			if (!flag)
			{
				this._mapModel.CricketWishEffectLocations[location] = default(VoidValue);
				GEvent.OnEvent(UiEvents.CricketWishEffectChanged, null);
			}
		}

		// Token: 0x06006F1D RID: 28445 RVA: 0x00336D94 File Offset: 0x00334F94
		private void MoveCameraTo(Vector2 position, bool doTween = false, TweenCallback onComplete = null, float tweenTime = 0.2f, Ease ease = Ease.Unset)
		{
			RectTransform root = this.moveRoot;
			bool flag = this._cameraMovingTweener != null;
			if (flag)
			{
				CommonUtils.TryKillTween(this._cameraMovingTweener, true);
				this._cameraMovingTweener = null;
			}
			bool flag2 = doTween && Vector2.Distance(position, this._lastCameraMoveTarget) > 0.1f;
			if (flag2)
			{
				Vector2 currentPosition = this._lastCameraMoveTarget;
				Vector2 deltaPosition = position - currentPosition;
				this._cameraMovingTweener = DOVirtual.Float(0f, 1f, tweenTime, delegate(float progress)
				{
					root.anchoredPosition = -(currentPosition + deltaPosition * progress) * root.localScale.x;
				}).SetEase(ease).SetAutoKill(true).OnComplete(onComplete);
			}
			else
			{
				root.anchoredPosition = -position * root.localScale.x;
				if (onComplete != null)
				{
					onComplete();
				}
			}
			this._lastCameraMoveTarget = position;
		}

		// Token: 0x06006F1E RID: 28446 RVA: 0x00336E98 File Offset: 0x00335098
		private void OnMapScaled(float newScale, bool forceUpdateVolume = false, bool centering = true)
		{
			float oldScale = MapBlockEffect.LossyScale;
			MapBlockEffect.LossyScale = newScale;
			this._player.GetComponent<RectTransform>().localScale = Vector3.one / newScale;
			this.ScaleAllContainers(newScale);
			float cricketScale = Mathf.Pow(1f / newScale, 1.375f) * newScale;
			this.rectTsCatchCricket.localScale = Vector3.one * cricketScale;
			bool flag = Math.Abs(oldScale - newScale) >= 0.01f || forceUpdateVolume;
			if (flag)
			{
				Vector2 pointA = new Vector2(0.25f, 0f);
				Vector2 pointB = new Vector2(1f, 1f);
				float i = (pointB.y - pointA.y) / (pointB.x - pointA.x);
				float b = pointA.y - i * pointA.x;
				float y = i * newScale + b;
				AudioManager.Instance.SetMapAmbienceVolumeRate(y);
				AudioManager.Instance.SetMusicVolumeWithFade(1f, 0f);
				Vector2 pointA2 = new Vector2(0.25f, 1f);
				Vector2 pointB2 = new Vector2(1f, 0.5f);
				float j = (pointB2.y - pointA2.y) / (pointB2.x - pointA2.x);
				float b2 = pointA2.y - j * pointA2.x;
				float y2 = j * newScale + b2;
				AudioManager.Instance.SetMapMusicVolumeRate(y2);
				AudioManager.Instance.SetMusicVolumeWithFade(1f, 0f);
			}
		}

		// Token: 0x06006F1F RID: 28447 RVA: 0x0033702A File Offset: 0x0033522A
		private void ResetMapUi()
		{
			this.ClearMovePath();
		}

		// Token: 0x06006F20 RID: 28448 RVA: 0x00337034 File Offset: 0x00335234
		private void RefreshPlayerVisible()
		{
			this.RefreshPlayerVisible(this.PlayerAtBlock);
		}

		// Token: 0x06006F21 RID: 28449 RVA: 0x00337044 File Offset: 0x00335244
		private void RefreshPlayerVisible(MapBlockData block)
		{
			bool flag = block == null;
			if (!flag)
			{
				bool flag2 = block.BlockId == this._mapModel.CurrentBlockId && block.AreaId == this._mapModel.CurrentAreaId;
				if (flag2)
				{
					bool visible = !this.GetElementContainer(block.GetLocation()).CheckHasCharacter();
					GameObject playerIcon = this._player.GetComponent<WorldMapPlayerInfo>().goPlayerIcon;
					bool flag3 = playerIcon.activeSelf != visible;
					if (flag3)
					{
						playerIcon.SetActive(visible);
						bool flag4 = visible;
						if (flag4)
						{
							this.UpdatePlayerIconOnMap(true);
						}
					}
				}
			}
		}

		// Token: 0x06006F22 RID: 28450 RVA: 0x003370E0 File Offset: 0x003352E0
		private void RefreshMapBlockInfo(MapBlockData block)
		{
			Location location = block.GetLocation();
			this.UpdateSingleElement(location);
			this.RefreshPlayerVisible(block);
		}

		// Token: 0x06006F23 RID: 28451 RVA: 0x00337105 File Offset: 0x00335305
		private void ClearMovePath()
		{
			this.ClearMovePathLine();
			this.CollectAllCosts();
			this._mapModel.PathClear();
			this._plannedMoveCosts.Clear();
			this._hasConsumedFirstMove = false;
		}

		// Token: 0x06006F24 RID: 28452 RVA: 0x00337135 File Offset: 0x00335335
		private void ClearMovePathLine()
		{
			this._indicateLineValidVertices.Clear();
			this._indicateLineInvalidVertices.Clear();
			this.paths.SetVerticesDirty();
			this.invalidPaths.GetComponent<CImage>().SetVerticesDirty();
			this._movingByController = false;
		}

		// Token: 0x06006F25 RID: 28453 RVA: 0x00337174 File Offset: 0x00335374
		private void RerenderMovePath()
		{
			bool isMovingByController = this._movingByController;
			this.ClearMovePathLine();
			bool flag = this._mapModel.MovePath.Count <= 0;
			if (!flag)
			{
				this._movingByController = isMovingByController;
				Vector2 playerPos = MapRenderSystem.GetBlockLocalPos(new Location(this.PlayerAtBlock.AreaId, this.PlayerAtBlock.BlockId));
				playerPos -= this.BlockUICenterOffset;
				BasicGameData basicGameData = SingletonObject.getInstance<BasicGameData>();
				for (int i = 0; i < this._mapModel.MovePath.Count; i++)
				{
					Location location = this._mapModel.MovePath[i];
					Vector2 localPos = MapRenderSystem.GetBlockLocalPos(location);
					localPos -= this.BlockUICenterOffset;
					this.UpdateSingleCost(location);
					int moveCost = this._mapModel.GetPathBlockMoveCost(this._mapModel.MovePath[i]);
					bool flag2 = SingletonObject.getInstance<TimeManager>().IsActionPointEnough(moveCost);
					if (flag2)
					{
						bool flag3 = i == 0;
						if (flag3)
						{
							this._indicateLineValidVertices.Add(playerPos);
						}
						this._indicateLineValidVertices.Add(localPos);
					}
					else
					{
						bool flag4 = i == 0;
						if (flag4)
						{
							this._indicateLineInvalidVertices.Add(playerPos);
						}
						else
						{
							bool flag5 = this._indicateLineInvalidVertices.Count <= 0;
							if (flag5)
							{
								List<Vector2> indicateLineInvalidVertices = this._indicateLineInvalidVertices;
								List<Vector2> indicateLineValidVertices = this._indicateLineValidVertices;
								indicateLineInvalidVertices.Add(indicateLineValidVertices[indicateLineValidVertices.Count - 1]);
							}
						}
						this._indicateLineInvalidVertices.Add(localPos);
					}
				}
				this.paths.SetVerticesDirty();
				this.invalidPaths.GetComponent<CImage>().SetVerticesDirty();
			}
		}

		// Token: 0x06006F26 RID: 28454 RVA: 0x00337328 File Offset: 0x00335528
		private void UpdatePlayerIconOnMap(bool forceRefreshEffect = false)
		{
			GameObject playerIcon = this._player.GetComponent<WorldMapPlayerInfo>().goPlayerIcon;
			CImage playerIconImage = playerIcon.GetComponent<CImage>();
			string playerSpriteName = CommonUtils.GetTaiwuSpriteName();
			playerIconImage.SetSpriteOnly(playerSpriteName, true, null);
			playerIconImage.enabled = true;
			bool flag = SingletonObject.getInstance<DlcManager>().IsDlcInstalled(DlcManager.DlcIdFiveLoong);
			if (flag)
			{
				ViewWorldMap.<>c__DisplayClass256_0 CS$<>8__locals1 = new ViewWorldMap.<>c__DisplayClass256_0();
				int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				Transform playerIconTransform = playerIcon.transform;
				string enabledEffectIconName = null;
				CS$<>8__locals1.fiveBallEffect = playerIconTransform.Find("LoongInfo");
				bool flag2 = CS$<>8__locals1.fiveBallEffect == null;
				if (!flag2)
				{
					CS$<>8__locals1.srcLayer = -1;
					foreach (short loongId in this._mapModel.FiveLoongDict.Keys)
					{
						LoongInfo loongInfo = this._mapModel.FiveLoongDict[loongId];
						if (!true)
						{
						}
						Transform transform;
						switch (loongId)
						{
						case 246:
							transform = CS$<>8__locals1.fiveBallEffect.Find("jin");
							break;
						case 247:
							transform = CS$<>8__locals1.fiveBallEffect.Find("shui");
							break;
						case 248:
							transform = CS$<>8__locals1.fiveBallEffect.Find("mu");
							break;
						case 249:
							transform = CS$<>8__locals1.fiveBallEffect.Find("huo");
							break;
						case 250:
							transform = CS$<>8__locals1.fiveBallEffect.Find("tu");
							break;
						default:
							transform = null;
							break;
						}
						if (!true)
						{
						}
						Transform ball = transform;
						bool ballShouldVisible = false;
						ushort debuffCount;
						bool flag3 = loongInfo.CharacterDebuffCounts != null && loongInfo.CharacterDebuffCounts.TryGetValue(taiwuCharId, out debuffCount) && debuffCount > 0;
						if (flag3)
						{
							enabledEffectIconName = playerSpriteName;
							playerIconImage.enabled = false;
							ballShouldVisible = true;
						}
						bool flag4 = ball != null && ball.gameObject.activeSelf != ballShouldVisible;
						if (flag4)
						{
							ball.gameObject.SetActive(ballShouldVisible);
						}
					}
					for (int i = 0; i < playerIconTransform.childCount; i++)
					{
						Transform effect = playerIconTransform.GetChild(i);
						bool flag5 = effect == CS$<>8__locals1.fiveBallEffect;
						if (!flag5)
						{
							bool effectShouldBeActive = effect.name == enabledEffectIconName;
							bool flag6 = effectShouldBeActive != effect.gameObject.activeSelf || forceRefreshEffect;
							if (flag6)
							{
								effect.gameObject.SetActive(effectShouldBeActive);
							}
							bool flag7 = effectShouldBeActive;
							if (flag7)
							{
								CS$<>8__locals1.srcLayer = effect.GetChild(0).GetComponent<ParticleSystemRenderer>().sortingOrder;
							}
						}
					}
					bool effectShouldBeActive2 = !string.IsNullOrEmpty(enabledEffectIconName);
					bool flag8 = effectShouldBeActive2 != CS$<>8__locals1.fiveBallEffect.gameObject.activeSelf || forceRefreshEffect;
					if (flag8)
					{
						CS$<>8__locals1.fiveBallEffect.gameObject.SetActive(effectShouldBeActive2);
						bool flag9 = effectShouldBeActive2 && CS$<>8__locals1.fiveBallEffect.gameObject.activeInHierarchy;
						if (flag9)
						{
							MonoJoint behaviour = CS$<>8__locals1.fiveBallEffect.gameObject.GetOrAddComponent<MonoJoint>();
							behaviour.StopAllCoroutines();
							behaviour.StartCoroutine(CS$<>8__locals1.<UpdatePlayerIconOnMap>g__FiveBallUpdate|0());
						}
					}
				}
			}
		}

		// Token: 0x06006F27 RID: 28455 RVA: 0x00337684 File Offset: 0x00335884
		private bool CheckCatchCricketRefresh()
		{
			RectTransform catchCricket = this.rectTsCatchCricket;
			return catchCricket.gameObject.activeSelf != this.CatchCricketShouldActive;
		}

		// Token: 0x06006F28 RID: 28456 RVA: 0x003376B4 File Offset: 0x003358B4
		private void RefreshCatchCricketBtn()
		{
			RectTransform catchCricket = this.rectTsCatchCricket;
			Button button = catchCricket.GetChild(0).GetComponent<Button>();
			button.interactable = false;
			bool catchCricketShouldActive = this.CatchCricketShouldActive;
			if (catchCricketShouldActive)
			{
				catchCricket.anchoredPosition = MapRenderSystem.GetBlockLocalPos(this.PlayerAtBlock.GetLocation());
				catchCricket.gameObject.SetActive(true);
			}
			else
			{
				catchCricket.gameObject.SetActive(false);
			}
		}

		// Token: 0x06006F29 RID: 28457 RVA: 0x00337720 File Offset: 0x00335920
		private void SelectBlock(MapBlockData block)
		{
			int selectedBlockId = (int)this._mapModel.SelectedBlockId;
			short? num = (block != null) ? new short?(block.BlockId) : null;
			int? num2 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
			bool flag = selectedBlockId == num2.GetValueOrDefault() & num2 != null;
			if (!flag)
			{
				this._mapModel.SelectedBlockId = ((block != null) ? block.BlockId : -1);
				GEvent.OnEvent(UiEvents.PickupDisplayInfoChange, EasyPool.Get<ArgumentBox>().SetObject("MapBlockData", block));
				bool flag2 = block == null;
				if (!flag2)
				{
					bool flag3 = this._mapModel.TaiwuMoveState == WorldMapModel.MoveState.Idle;
					if (flag3)
					{
						GEvent.OnEvent(UiEvents.WorldMapShowInfoBlockChange, EasyPool.Get<ArgumentBox>().SetObject("block", block));
					}
					else
					{
						GEvent.OnEvent(UiEvents.WorldMapBlockDataChange, EasyPool.Get<ArgumentBox>().SetObject("Data", block));
					}
					this._select.anchoredPosition = MapRenderSystem.GetBlockLocalPos(new Location(block.AreaId, block.BlockId));
					this._selectVirtual.anchoredPosition = this._select.anchoredPosition;
					this.UpdateSettlementAndStationBtnUsable();
				}
			}
		}

		// Token: 0x06006F2A RID: 28458 RVA: 0x00337868 File Offset: 0x00335A68
		private void RefreshPath(MapBlockData block)
		{
			bool flag = block != this.PlayerAtBlock;
			if (flag)
			{
				this.FindWay(block.GetLocation());
			}
			else
			{
				this.ClearMovePath();
			}
		}

		// Token: 0x06006F2B RID: 28459 RVA: 0x0033789C File Offset: 0x00335A9C
		private void FindWay(Location location)
		{
			this.CollectAllCosts();
			this._mapModel.FindWay(location);
			this.RecordPathCosts();
			this.RerenderMovePath();
		}

		// Token: 0x06006F2C RID: 28460 RVA: 0x003378C4 File Offset: 0x00335AC4
		private void RecordPathCosts()
		{
			this._plannedMoveCosts.Clear();
			this._hasConsumedFirstMove = false;
			for (int i = 0; i < this._mapModel.MovePath.Count; i++)
			{
				MapBlockData block = this._mapModel.GetBlockData(this._mapModel.MovePath[i]);
				this._plannedMoveCosts.Add(block.MoveCostActionPoint);
			}
		}

		// Token: 0x06006F2D RID: 28461 RVA: 0x00337938 File Offset: 0x00335B38
		private void OnTopUiChange(ArgumentBox argsBox = null)
		{
			bool flag = this._pathMoving && !UIManager.Instance.IsFocusElement(this.Element);
			if (flag)
			{
				this._hotkeyMoveDirection = MoveDirection.None;
				this._pathMoving = false;
			}
			bool flag2 = !UIElement.AdvanceConfirm.Exist && !UIElement.Advance.Exist && !UIElement.MonthNotify.Exist;
			if (flag2)
			{
				this.ClearMovePath();
			}
			bool flag3 = UIManager.Instance.IsFocusElement(this.Element);
			if (flag3)
			{
				this._mapModel.UpdateViewModeData();
			}
			bool flag4 = !UIManager.Instance.IsFocusElement(this.Element);
			if (flag4)
			{
				this._mapModel.ClearTemporaryMark();
			}
			bool flag5 = UIManager.Instance.IsFocusElement(this.Element) && UIElement.MonthNotify.Exist && !UIManager.Instance.IsFocusElement(UIElement.MonthNotify);
			if (flag5)
			{
				this.ReshowBlockList(false);
			}
			this._hotkeyMoveReady = true;
			this._movingByController = false;
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.5f, delegate
			{
				this.MapClickReceiver.RefreshScale();
			});
			this.RefreshSettlementAndStationBtn();
			this.UpdateStationBtnVisible();
			bool flag6 = UIManager.Instance.IsFocusElement(this.Element);
			if (flag6)
			{
				this.PastTaiwuVillageSetNegativeFilmInArea();
			}
		}

		// Token: 0x06006F2E RID: 28462 RVA: 0x00337A86 File Offset: 0x00335C86
		private void OnMapInited(ArgumentBox argsBox)
		{
			this.RefreshCricketPlace(true);
			this.RefreshCaravans();
			this.UpdateAdventureIcons();
			this.RefreshSettlementAndStationBtn();
			this.UpdateSettlementAndStationBtnUsable();
			this.UpdateStationBtnVisible();
			this.RefreshProfessionTravelerEffect();
			this.TryStartProfessionTravelerStationSound();
		}

		// Token: 0x06006F2F RID: 28463 RVA: 0x00337AC4 File Offset: 0x00335CC4
		private void OnPlayerLocationChangedInHide(ArgumentBox _)
		{
			bool activeInHierarchy = base.gameObject.activeInHierarchy;
			if (!activeInHierarchy)
			{
				this.SetDirty();
				bool flag = this._lastBlockId != this._mapModel.CurrentBlockId;
				if (flag)
				{
					this.ResetSelectedBlock();
					this.SetPlayerLocationWithRefreshAllBlockVisibility(this._mapModel.CurrentLocation, default(Location), -1f, true);
					this._mapModel.ChangeTaiwuMoveState(WorldMapModel.MoveState.Idle);
					SingletonObject.getInstance<MapRenderSystem>().RefreshAllMapBlocks(true);
				}
			}
		}

		// Token: 0x06006F30 RID: 28464 RVA: 0x00337B47 File Offset: 0x00335D47
		private void OnPlayerAreaChange(ArgumentBox argsBox)
		{
			this._playerAtBlockInitialized = false;
			this.UpdateBlockId();
			this.UpdateAreaSettlementBlockMap();
			CommandKitBase.SetDisable(false);
			this.ClearMovePath();
		}

		// Token: 0x06006F31 RID: 28465 RVA: 0x00337B70 File Offset: 0x00335D70
		private void OnPlayerBlockChange(ArgumentBox argsBox)
		{
			bool flag = !this._playerAtBlockInitialized;
			if (!flag)
			{
				this.SetDirty();
				bool flag2 = !this._pathMoving;
				if (flag2)
				{
					this.ResetSelectedBlock();
				}
				bool flag3 = this._mapModel.LastAtLocation.IsValid();
				if (flag3)
				{
					this.UpdateSingleCharacter(this._mapModel.LastAtLocation);
				}
				this.UpdateSingleCharacter(this._mapModel.CurrentLocation);
				this.RefreshPlayerVisible();
				this.RefreshCricketPlace(false);
				this.PlayMoveAni(this._lastBlockId, this._mapModel.CurrentBlockId);
				this.RefreshProfessionTravelerStationUpEffect();
				this.RefreshProfessionTravelerStationDownEffect();
				this.RefreshProfessionTravelerStationSound(this._mapModel.LastAtLocation, this._mapModel.CurrentLocation);
				this.UpdateBlockId();
				this._mapModel.UpdateBgm();
				this.UpdateSettlementAndStationBtnUsable();
				GEvent.OnEvent(UiEvents.WorldMapShowInfoBlockChange, EasyPool.Get<ArgumentBox>().SetObject("block", this.PlayerAtBlock));
				this.OnMapScaled(this.MapClickReceiver.CurrentScale, false, true);
				GEvent.OnEvent(UiEvents.OnRefreshWorkButton, EasyPool.Get<ArgumentBox>().Set("isStartMove", true));
				this.RefreshMapPickups(argsBox);
				SingletonObject.getInstance<MapRenderSystem>().RefreshAllMapBlocks(true);
			}
		}

		// Token: 0x06006F32 RID: 28466 RVA: 0x00337CBC File Offset: 0x00335EBC
		private void UpdateBlockId()
		{
			this._lastBlockId = this._mapModel.CurrentBlockId;
		}

		// Token: 0x06006F33 RID: 28467 RVA: 0x00337CD0 File Offset: 0x00335ED0
		private void UpdateAreaSettlementBlockMap()
		{
			bool flag = this._mapModel.CurrentAreaId < 0;
			if (flag)
			{
				this._areaSettlementBlockMap.Clear();
				this.SettlementBlockConatiner.Clear();
			}
			else
			{
				short areaId = this._mapModel.CurrentAreaId;
				SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1f, delegate
				{
					bool flag2 = areaId != this._mapModel.CurrentAreaId;
					if (!flag2)
					{
						List<MapBlockData> blocks = new List<MapBlockData>();
						this._mapModel.GetAreaBlocks(this._mapModel.CurrentAreaId, blocks);
						Dictionary<MapBlockData, ByteCoordinate> settlementsWithPositions = EasyPool.Get<Dictionary<MapBlockData, ByteCoordinate>>();
						foreach (MapBlockData block in blocks)
						{
							ByteCoordinate position = block.GetBlockPos();
							bool flag3 = block.IsCityTown();
							if (flag3)
							{
								settlementsWithPositions.Add(block, block.GetBlockPos());
							}
						}
						this._areaSettlementBlockMap.Clear();
						foreach (MapBlockData block2 in blocks)
						{
							bool flag4 = ((block2.RootBlockId == -1) ? block2.BelongBlockId : block2.GetRootBlock().BelongBlockId) == -1;
							if (!flag4)
							{
								ByteCoordinate blockPos = block2.GetBlockPos();
								foreach (KeyValuePair<MapBlockData, ByteCoordinate> keyValuePair in settlementsWithPositions)
								{
									MapBlockData mapBlockData;
									ByteCoordinate byteCoordinate;
									keyValuePair.Deconstruct(out mapBlockData, out byteCoordinate);
									MapBlockData settlement = mapBlockData;
									ByteCoordinate settlementPos = byteCoordinate;
									Location taiwuVillageBlock = SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageBlock();
									int distance = blockPos.GetManhattanDistance(settlementPos);
									Location settlementRootLocation = settlement.GetRootBlock().GetLocation();
									bool flag5 = distance <= (int)(GlobalConfig.Instance.SettlementInfluenceRange - ((settlementRootLocation == taiwuVillageBlock) ? 1 : 0));
									if (flag5)
									{
										bool flag6 = !this._areaSettlementBlockMap.ContainsKey(settlementRootLocation.BlockId);
										if (flag6)
										{
											this._areaSettlementBlockMap[settlementRootLocation.BlockId] = new HashSet<short>();
										}
										this._areaSettlementBlockMap[settlementRootLocation.BlockId].Add(block2.BlockId);
									}
								}
							}
						}
						EasyPool.Free<Dictionary<MapBlockData, ByteCoordinate>>(settlementsWithPositions);
						this.SettlementBlockConatiner.RefreshAll(areaId, this._areaSettlementBlockMap);
						this.SettlementBlockConatiner.TurnOffAll();
					}
				});
			}
		}

		// Token: 0x06006F34 RID: 28468 RVA: 0x00337D48 File Offset: 0x00335F48
		private void OnEnterNewArea(ArgumentBox argsBox)
		{
			bool sameState;
			argsBox.Get("SameState", out sameState);
			this._playerAtBlockInitialized = true;
			this.InitMap();
			this.UpdateBlockId();
			SingletonObject.getInstance<CharacterMonitorModel>().RemoveUnusedCharacter();
			WorldMapModel.MapBlockLoadFinish = false;
			WorldMapModel.MapBlockUiLoadFinish = false;
			bool flag = sameState;
			if (flag)
			{
				this.ResetClickReceiverOffset();
			}
			else
			{
				base.StartCoroutine(this.RefreshMapBlock());
			}
			base.StartCoroutine(this.RefreshMapUi(true));
			base.StartCoroutine(this.AdjustMapObjectScale());
			this.MapClickReceiver.StopDrag();
			this.MoveCameraTo(MapRenderSystem.GetBlockLocalPos(new Location(this.PlayerAtBlock.AreaId, this.PlayerAtBlock.BlockId)), false, null, 0.2f, Ease.Unset);
			this.OnMapScaled(this.MapClickReceiver.CurrentScale, false, true);
			this.RefreshPlayerVisible();
			this.RefreshFulongInFlameAreas();
			this.RefreshAllZhujianThief();
		}

		// Token: 0x06006F35 RID: 28469 RVA: 0x00337E2A File Offset: 0x0033602A
		private IEnumerator AdjustMapObjectScale()
		{
			yield return new WaitForSeconds(0.5f);
			this.OnMapScaled(this.MapClickReceiver.CurrentScale, false, true);
			yield break;
		}

		// Token: 0x06006F36 RID: 28470 RVA: 0x00337E39 File Offset: 0x00336039
		private IEnumerator CoShakeMapByEvent(ArgumentBox argBox)
		{
			UIElement.BlockInteract.Show();
			yield return base.StartCoroutine(this.CoShakeMap());
			UIElement eventWindow = UIElement.EventWindow;
			eventWindow.OnShowed = (Action)Delegate.Combine(eventWindow.OnShowed, new Action(delegate()
			{
				UIElement.BlockInteract.Hide(false);
			}));
			TaiwuEventDomainMethod.Call.TriggerListener("WorldMapShaken", true);
			yield break;
		}

		// Token: 0x06006F37 RID: 28471 RVA: 0x00337E4F File Offset: 0x0033604F
		private IEnumerator CoFocusMapByEvent(ArgumentBox argBox)
		{
			UIElement.BlockInteract.Show();
			Location location;
			argBox.Get<Location>("location", out location);
			this.MoveCameraTo(location, false, null, 0.2f, Ease.Unset);
			float duration;
			argBox.Get("duration", out duration);
			yield return new WaitForSeconds(duration);
			UIElement eventWindow = UIElement.EventWindow;
			eventWindow.OnShowed = (Action)Delegate.Combine(eventWindow.OnShowed, new Action(delegate()
			{
				UIElement.BlockInteract.Hide(false);
			}));
			TaiwuEventDomainMethod.Call.TriggerListener("WorldMapFocused", true);
			yield break;
		}

		// Token: 0x06006F38 RID: 28472 RVA: 0x00337E65 File Offset: 0x00336065
		private IEnumerator CoShakeMap()
		{
			float quakeValue = 50f;
			AudioManager.Instance.PlayAmbience(AudioManager.DummyAudioName, 0.2f, 100);
			AudioManager.Instance.PlaySound("ui_earthquake", false, false);
			yield return this.moveRoot.DOShakeAnchorPos(3f, quakeValue, 10, 90f, false, true, ShakeRandomnessMode.Full).WaitForCompletion();
			yield break;
		}

		// Token: 0x06006F39 RID: 28473 RVA: 0x00337E74 File Offset: 0x00336074
		private void OnAreaDataChange(ArgumentBox argsBox)
		{
			short areaId;
			argsBox.Get("AreaId", out areaId);
			bool stationUnlockChange;
			argsBox.Get("StationUnlockChanged", out stationUnlockChange);
			bool flag = stationUnlockChange && this._mapModel.GetStateId(areaId) == this._mapModel.CurrentStateId;
			if (flag)
			{
				this.UpdateStationBtnVisible();
			}
		}

		// Token: 0x06006F3A RID: 28474 RVA: 0x00337EC8 File Offset: 0x003360C8
		private void OnAnimalPlaceDataChange(ArgumentBox argBox)
		{
			ArgumentBox aBox = new ArgumentBox();
			List<MapBlockData> blocks = EasyPool.Get<List<MapBlockData>>();
			this._mapModel.GetAreaBlocks(this._mapModel.ShowingAreaId, blocks);
			foreach (MapBlockData block in blocks)
			{
				aBox.SetObject("Data", block);
				this.OnBlockDataChange(aBox);
			}
			EasyPool.Free<List<MapBlockData>>(blocks);
		}

		// Token: 0x06006F3B RID: 28475 RVA: 0x00337F54 File Offset: 0x00336154
		private void UpdateAlterEffects()
		{
			bool equal = (this._mapModel.AlterSettlementLocations == null) ? (this._lastAlterSettlementLocations.Count == 0) : this._lastAlterSettlementLocations.SequenceEqual(this._mapModel.AlterSettlementLocations);
			bool flag = equal;
			if (!flag)
			{
				IEnumerable<Location> settlementLocations = this._lastAlterSettlementLocations;
				bool flag2 = this._mapModel.AlterSettlementLocations != null;
				if (flag2)
				{
					settlementLocations = settlementLocations.Union(this._mapModel.AlterSettlementLocations);
				}
				MapRenderSystem renderSystem = SingletonObject.getInstance<MapRenderSystem>();
				foreach (Location location in settlementLocations)
				{
					bool flag3 = location.AreaId == this._mapModel.ShowingAreaId;
					if (flag3)
					{
						renderSystem.UpdateBlockEffect(this._mapModel.GetBlockData(location));
					}
				}
				this._lastAlterSettlementLocations.Clear();
				bool flag4 = this._mapModel.AlterSettlementLocations != null;
				if (flag4)
				{
					this._lastAlterSettlementLocations.AddRange(this._mapModel.AlterSettlementLocations);
				}
			}
		}

		// Token: 0x06006F3C RID: 28476 RVA: 0x00338074 File Offset: 0x00336274
		private void ProcessBlock(MapBlockData block)
		{
			Location blockLocation = new Location(block.AreaId, block.BlockId);
			bool isVisible = block.Visible;
			this.TryRefreshSectStoryEffects(blockLocation, isVisible);
			MapRenderSystem renderSystem = SingletonObject.getInstance<MapRenderSystem>();
			renderSystem.UpdateBlockState(block);
			renderSystem.UpdateBlockEffect(block);
			renderSystem.UpdateBlockSprite(block);
		}

		// Token: 0x06006F3D RID: 28477 RVA: 0x003380C4 File Offset: 0x003362C4
		private void OnBlockDataChange(ArgumentBox argsBox)
		{
			MapBlockData block;
			argsBox.Get<MapBlockData>("Data", out block);
			bool flag = this._mapModel.GetStateId(block.AreaId) == this._mapModel.CurrentStateId && block.TemplateId != 126 && !UIManager.Instance.IsFocusElement(UIElement.PartWorld);
			if (flag)
			{
				bool flag2 = block.AreaId == this._mapModel.CurrentAreaId;
				if (flag2)
				{
					this.RefreshMapBlockInfo(block);
				}
				bool flag3 = block.AreaId == this._mapModel.ShowingAreaId;
				if (flag3)
				{
					this.ProcessBlock(block);
				}
				StoryDomainMethod.Call.UpdateSectEmeiGuidanceData(block.GetLocation());
			}
		}

		// Token: 0x06006F3E RID: 28478 RVA: 0x00338170 File Offset: 0x00336370
		private void OnBrokenMaterialDataChange(ArgumentBox argBox)
		{
			List<Location> changedLocations;
			argBox.Get<List<Location>>("changedLocations", out changedLocations);
			foreach (Location location in changedLocations)
			{
				MapBlockData blockData;
				bool flag = this._mapModel.TryGetBlockData(location, out blockData);
				if (flag)
				{
					this.ProcessBlock(blockData);
				}
			}
		}

		// Token: 0x06006F3F RID: 28479 RVA: 0x003381E4 File Offset: 0x003363E4
		private void OnMapBlockVisibilityNeedRefresh(ArgumentBox argsBox)
		{
			this.RefreshAllBlockVisibility();
			SingletonObject.getInstance<MapRenderSystem>().RefreshAllMapBlocks(false);
		}

		// Token: 0x06006F40 RID: 28480 RVA: 0x003381FC File Offset: 0x003363FC
		private void OnCricketPlaceDataChange(ArgumentBox argsBox)
		{
			bool mapBlockLoadFinish = WorldMapModel.MapBlockLoadFinish;
			if (mapBlockLoadFinish)
			{
				this.RefreshCricketPlace(false);
				this.OnMapScaled(this.MapClickReceiver.CurrentScale, false, true);
			}
		}

		// Token: 0x06006F41 RID: 28481 RVA: 0x00338231 File Offset: 0x00336431
		private void OnZhujianThiefDataChanged(ArgumentBox argBox)
		{
			this.RefreshAllZhujianThief();
		}

		// Token: 0x06006F42 RID: 28482 RVA: 0x0033823C File Offset: 0x0033643C
		private void OnWorldMapVillagerWorkingLocationChange(ArgumentBox argsBox)
		{
			short areaId;
			argsBox.Get("AreaId", out areaId);
			short blockId;
			argsBox.Get("BlockId", out blockId);
			bool flag = areaId == this._mapModel.CurrentAreaId;
			if (flag)
			{
				this.UpdateSingleInfo(new Location(areaId, blockId));
			}
		}

		// Token: 0x06006F43 RID: 28483 RVA: 0x00338288 File Offset: 0x00336488
		private void OnLocationMarkChange(ArgumentBox argsBox)
		{
			short areaId;
			argsBox.Get("AreaId", out areaId);
			short blockId;
			argsBox.Get("BlockId", out blockId);
			bool flag = areaId == this._mapModel.CurrentAreaId;
			if (flag)
			{
				this.UpdateSingleInfo(new Location(areaId, blockId));
			}
		}

		// Token: 0x06006F44 RID: 28484 RVA: 0x003382D4 File Offset: 0x003364D4
		private bool CheckGuidBlockMove()
		{
			bool inGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			if (inGuiding)
			{
				bool flag = !SingletonObject.getInstance<TutorialChapterModel>().MoveEnable;
				if (flag)
				{
					TaiwuEventDomainMethod.Call.TryMoveWhenMoveDisable();
					this.ClearMovePath();
					return true;
				}
				Location forceLocation = SingletonObject.getInstance<TutorialChapterModel>().ForceNextLocation;
				bool flag2 = forceLocation != Location.Invalid && this._mapModel.MovePath.Count > 0 && forceLocation != this._mapModel.MovePath[0];
				if (flag2)
				{
					TaiwuEventDomainMethod.Call.TryMoveToInvalidLocationInTutorial();
					this.ClearMovePath();
					return true;
				}
			}
			return false;
		}

		// Token: 0x06006F45 RID: 28485 RVA: 0x0033837A File Offset: 0x0033657A
		private void MoveToNext()
		{
			CommandManager.AddCommandAuto(EPriority.TaiwuMove, delegate
			{
				bool isMoveBanned = this._mapModel.IsMoveBanned;
				bool result;
				if (isMoveBanned)
				{
					result = false;
				}
				else
				{
					bool flag = this._mapModel.TaiwuMoveState > WorldMapModel.MoveState.Idle;
					if (flag)
					{
						result = false;
					}
					else
					{
						bool flag2 = this._mapModel.TaiwuMoveState == WorldMapModel.MoveState.Idle && this._mapModel.MovePath.Count > 0;
						if (flag2)
						{
							this.MoveToNextTruly();
						}
						result = (this._mapModel.TaiwuMoveState > WorldMapModel.MoveState.Idle);
					}
				}
				return result;
			}, () => this._mapModel.TaiwuMoveState == WorldMapModel.MoveState.Idle);
		}

		// Token: 0x06006F46 RID: 28486 RVA: 0x0033839C File Offset: 0x0033659C
		private void MoveToNextTruly()
		{
			bool waitAdvanceMonth = this._waitAdvanceMonth;
			if (!waitAdvanceMonth)
			{
				bool flag = this._pathMoving && this._plannedMoveCosts.Count > 0;
				if (flag)
				{
					int currentCost = this._mapModel.GetBlockData(this._mapModel.MovePath[0]).MoveCostActionPoint;
					bool flag2 = currentCost != this._plannedMoveCosts[0];
					if (flag2)
					{
						bool hasConsumedFirstMove = this._hasConsumedFirstMove;
						if (hasConsumedFirstMove)
						{
							this._pathMoving = false;
							this.OnMoveFinished();
							this.RerenderMovePath();
							return;
						}
						int minCount = Math.Min(this._mapModel.MovePath.Count, this._plannedMoveCosts.Count);
						for (int i = 0; i < minCount; i++)
						{
							MapBlockData block = this._mapModel.GetBlockData(this._mapModel.MovePath[i]);
							this._plannedMoveCosts[i] = block.MoveCostActionPoint;
						}
					}
				}
				int moveCost = this._mapModel.GetPathBlockMoveCost(this._mapModel.MovePath[0]);
				bool flag3 = SingletonObject.getInstance<TimeManager>().IsActionPointEnough(moveCost);
				if (flag3)
				{
					AdaptableLog.Info(string.Format("Move to {0}, path=[{1}]", this._mapModel.MovePath[0].BlockId, string.Join<short>(",", from x in this._mapModel.MovePath
					select x.BlockId)));
					MapDomainMethod.Call.Move(this._mapModel.MovePath[0].BlockId);
					this._mapModel.ChangeTaiwuMoveState(WorldMapModel.MoveState.SendMoveMessage);
					UIElement.FullScreenMask.Show();
					this._doingMove = true;
					bool flag4 = this._plannedMoveCosts.Count > 0;
					if (flag4)
					{
						this._plannedMoveCosts.RemoveAt(0);
						this._hasConsumedFirstMove = true;
					}
					this._needUpdatePath = false;
					bool flag5 = this._pathMoving && this._mapModel.MovePath.Count > 2;
					if (flag5)
					{
						List<MapBlockData> neighborBlocks = EasyPool.Get<List<MapBlockData>>();
						this._mapModel.GetNeighborList(this.PlayerAtBlock.AreaId, this._mapModel.MovePath[0].BlockId, neighborBlocks, 1, false);
						for (int j = 0; j < neighborBlocks.Count; j++)
						{
							bool flag6 = !neighborBlocks[j].Visible;
							if (flag6)
							{
								break;
							}
						}
						EasyPool.Free<List<MapBlockData>>(neighborBlocks);
					}
				}
				else
				{
					bool flag7 = TimeManager.ActionPointMax >= moveCost;
					if (flag7)
					{
						bool flag8 = !SingletonObject.getInstance<TutorialChapterModel>().AdvanceMonthEnable;
						if (flag8)
						{
							this._pathMoving = false;
							this.OnMoveFinished();
						}
						else
						{
							bool atPastTaiwuVillage = SingletonObject.getInstance<WorldMapModel>().AtPastTaiwuVillage;
							if (atPastTaiwuVillage)
							{
								GEvent.OnEvent(EEvents.RequestAdvanceMonth, null);
								this.OnMoveFinished();
							}
							else
							{
								DialogCmd cmd = new DialogCmd
								{
									Type = 1,
									Title = LocalStringManager.Get(LanguageKey.UI_AdvanceMonth_TipTitle),
									Content = LocalStringManager.Get(LanguageKey.LK_Left_Time_Not_Enough_Tip).ColorReplace() + "\n" + LocalStringManager.Get(LanguageKey.LK_Advance_Month_Confirm),
									Yes = delegate()
									{
										GEvent.OnEvent(UiEvents.ShowAdvanceMonthConfirm, EasyPool.Get<ArgumentBox>().SetObject("callback", new Action(this.AdvanceMonth)));
									}
								};
								UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
								UIManager.Instance.MaskUI(UIElement.Dialog);
								this.OnMoveFinished();
							}
						}
					}
					else
					{
						DialogCmd cmd2 = new DialogCmd
						{
							Type = 2,
							Title = LocalStringManager.Get(LanguageKey.LK_Cannot_Move_Tip_Title),
							Content = LocalStringManager.Get(LanguageKey.LK_Left_Time_Not_Enough_Tip).ColorReplace()
						};
						UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd2));
						UIManager.Instance.MaskUI(UIElement.Dialog);
						this.OnMoveFinished();
					}
				}
			}
		}

		// Token: 0x06006F47 RID: 28487 RVA: 0x003387B6 File Offset: 0x003369B6
		private void AdvanceMonth()
		{
			GlobalDomainMethod.AsyncCall.CheckDriveSpace(this, delegate(int offset, RawDataPool dataPool)
			{
				bool hasSpace = false;
				Serializer.Deserialize(dataPool, offset, ref hasSpace);
				bool flag = hasSpace;
				if (flag)
				{
					this.<AdvanceMonth>g__Action|289_1();
				}
				else
				{
					string archiveDirPath = GameApp.GetArchiveDirPath();
					string disk = Path.GetPathRoot(archiveDirPath);
					string title = LocalStringManager.Get(LanguageKey.LK_Save_CheckDiskSpace_Title);
					string content = LocalStringManager.GetFormat(LanguageKey.LK_Save_CheckDiskSpace_Content, disk).ColorReplace();
					CommonUtils.ShowConfirmDialog(title, content, new Action(this.<AdvanceMonth>g__Action|289_1), null, EDialogType.None);
				}
			});
		}

		// Token: 0x06006F48 RID: 28488 RVA: 0x003387D0 File Offset: 0x003369D0
		private void PlayMoveAni(short fromBlockId, short toBlockId)
		{
			bool teleportMoving = this._teleportMoving;
			if (teleportMoving)
			{
				this._teleportMoving = false;
				this.SetPlayerLocationWithRefreshAllBlockVisibility(new Location(this._mapModel.CurrentAreaId, toBlockId), default(Location), -1f, true);
				this.OnMoveAniComplete(fromBlockId);
				this.RefreshAllBlockVisibility();
			}
			else
			{
				base.StartCoroutine(this.CoPlayMoveAnim(fromBlockId, toBlockId));
			}
		}

		// Token: 0x06006F49 RID: 28489 RVA: 0x0033883A File Offset: 0x00336A3A
		private IEnumerator CoPlayMoveAnim(short fromBlockId, short toBlockId)
		{
			this._mapModel.ChangeTaiwuMoveState(WorldMapModel.MoveState.PerformMove);
			Location fromLocation = new Location(this.PlayerAtBlock.AreaId, fromBlockId);
			Location toLocation = new Location(this.PlayerAtBlock.AreaId, toBlockId);
			bool eventPerforming = UIElement.EventWindow.Exist;
			bool flag = this._mapModel.ShowingAreaId != this.PlayerAtBlock.AreaId;
			if (flag)
			{
				this.SetShowingArea(this.PlayerAtBlock.AreaId);
			}
			DOVirtual.Float(0f, 1f, ViewWorldMap.MoveStepTime, delegate(float stepVal)
			{
				this.SetPlayerLocationWithRefreshAllBlockVisibility(fromLocation, toLocation, stepVal, false);
			});
			yield return new WaitForSeconds(ViewWorldMap.MoveStepTime + ViewWorldMap.MoveInterval);
			bool flag2 = eventPerforming || !this._movingByController;
			if (flag2)
			{
				MapDomainMethod.Call.MoveFinish(fromLocation, toLocation);
				this.UpdateSingleElement(fromLocation);
				this.UpdateSingleElement(toLocation);
				this.ClearMovePath();
			}
			this.OnMoveAniComplete(fromBlockId);
			yield break;
		}

		// Token: 0x06006F4A RID: 28490 RVA: 0x00338858 File Offset: 0x00336A58
		private void OnMoveAniComplete(short fromBlockId)
		{
			Location current = new Location(this._mapModel.CurrentAreaId, this._mapModel.CurrentBlockId);
			bool flag = this._mapModel.MovePath.Count > 0;
			if (flag)
			{
				Location previous = new Location(this._mapModel.CurrentAreaId, fromBlockId);
				MapDomainMethod.Call.MoveFinish(previous, current);
				this.UpdateSingleElement(previous);
				this.UpdateSingleElement(current);
				bool inGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
				if (inGuiding)
				{
					SingletonObject.getInstance<TutorialChapterModel>().UpdateForceNextLocation();
				}
				this.CollectSingleCost(this._mapModel.MovePath[0]);
				this._mapModel.PathDequeue();
			}
			MapDomainMethod.Call.IsContinuousMovingBreak(this.Element.GameDataListenerId);
			bool flag2 = this._mapModel.MovePath.Count == 0;
			if (flag2)
			{
				this._pathMoving = false;
			}
			bool pathMoving = this._pathMoving;
			if (pathMoving)
			{
				this._continuousMovingCallback = delegate()
				{
					bool flag3 = this.CheckGuidBlockMove();
					if (flag3)
					{
						this._pathMoving = false;
						this.OnMoveFinished();
						this.RerenderMovePath();
						this.OnBlockClick(this._mapModel.GetBlockData(current));
					}
					else
					{
						this.MoveToNext();
						this.RerenderMovePath();
					}
				};
			}
		}

		// Token: 0x06006F4B RID: 28491 RVA: 0x00338970 File Offset: 0x00336B70
		private void OnMoveFinished()
		{
			UIElement.FullScreenMask.Hide(false);
			GLog.TagLog("UI_Worldmap", string.Format("OnMoveFinished at {0}", Time.frameCount), Array.Empty<object>());
			this._doingMove = false;
			this.RefreshCricketPlace(false);
			this.RefreshPlayerVisible();
			this.UpdateBlockId();
			GEvent.OnEvent(UiEvents.OnRefreshWorkButton, EasyPool.Get<ArgumentBox>().Set("isStartMove", false));
			GEvent.OnEvent(UiEvents.OnMapBlockMoveFinished, EasyPool.Get<ArgumentBox>());
		}

		// Token: 0x06006F4C RID: 28492 RVA: 0x00338A00 File Offset: 0x00336C00
		private void OnDaysInMonthChange(ArgumentBox argBox)
		{
			bool flag = this._waitAdvanceMonth && SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays() == 0;
			if (flag)
			{
				WorldDomainMethod.Call.AdvanceMonth();
				GameApp.AdvancingMonth = true;
			}
			bool flag2 = !this._doingMove && this._mapModel.MovePath.Count > 0 && !WorldMapModel.Traveling;
			if (flag2)
			{
				this.RerenderMovePath();
			}
		}

		// Token: 0x06006F4D RID: 28493 RVA: 0x00338A6C File Offset: 0x00336C6C
		private void OnMonthChange(ArgumentBox argBox)
		{
			int[] data;
			argBox.Get<int[]>("Data", out data);
			bool flag = data == null || data.Length != 2 || data[0] == data[1];
			if (!flag)
			{
				bool activeInHierarchy = base.gameObject.activeInHierarchy;
				if (activeInHierarchy)
				{
					this._waitAdvanceMonth = false;
					this._hotkeyMoveReady = true;
					base.StartCoroutine(this.RefreshAllBlock());
				}
				else
				{
					this._playerAtBlockInitialized = false;
				}
			}
		}

		// Token: 0x06006F4E RID: 28494 RVA: 0x00338AD7 File Offset: 0x00336CD7
		private void OnResolutionChange(ArgumentBox argBox)
		{
			this.UpdateAllInfo();
			this.RerenderMovePath();
			this.RefreshCricketPlace(true);
			this.RefreshCaravans();
			this.UpdateAdventureIcons();
		}

		// Token: 0x06006F4F RID: 28495 RVA: 0x00338B00 File Offset: 0x00336D00
		private void OnGameFunctionLockStateChange(ArgumentBox argBox)
		{
			byte functionId;
			argBox.Get("FunctionId", out functionId);
			bool flag = functionId == 9;
			if (flag)
			{
				this.caravanRoot.gameObject.SetActive(SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(9));
			}
			bool flag2 = functionId == 13;
			if (flag2)
			{
				SettlementInfo[] settlementInfos = this._mapModel.Areas[(int)this._mapModel.CurrentAreaId].SettlementInfos;
				for (int i = 0; i < settlementInfos.Length; i++)
				{
					short blockId = settlementInfos[i].BlockId;
					bool flag3 = blockId >= 0;
					if (flag3)
					{
						this.GetElementContainer(new Location(this._mapModel.CurrentAreaId, blockId)).UpdateSettlementBtn();
					}
				}
			}
		}

		// Token: 0x06006F50 RID: 28496 RVA: 0x00338BC0 File Offset: 0x00336DC0
		private void SetShowingArea(short areaId)
		{
			this._mapModel.ShowingAreaId = areaId;
			this._player.gameObject.SetActive(areaId == this._mapModel.CurrentAreaId);
			this._select.gameObject.SetActive(areaId == this._mapModel.CurrentAreaId);
			this._selectVirtual.gameObject.SetActive(areaId == this._mapModel.CurrentAreaId);
			this.RefreshTradeCaravanPath(-1);
			this.ResetClickReceiverOffset();
			base.StartCoroutine(this.ResetMapRenderer());
			this.UpdateAllContainers();
			this.RefreshCatchCricketBtn();
			this.OnMapScaled(this.MapClickReceiver.CurrentScale, false, true);
			GEvent.OnEvent(UiEvents.WorldMapShowingAreaChange, null);
		}

		// Token: 0x06006F51 RID: 28497 RVA: 0x00338C84 File Offset: 0x00336E84
		private void RefreshCaravans()
		{
			this.RefreshTradeCaravanPath(this._showingPathCaravanId);
			foreach (CaravanDisplayData caravanData in this._mapModel.CaravanData)
			{
				Location location = caravanData.PathInArea.GetCurrLocation();
				this.UpdateSingleCharacter(location);
				Location last;
				bool flag = this._lastCaravanLocationDict.TryGetValue(caravanData.CaravanId, out last) && last != location;
				if (flag)
				{
					this.UpdateSingleCharacter(last);
				}
				this._lastCaravanLocationDict[caravanData.CaravanId] = location;
			}
			this.RefreshPlayerVisible();
		}

		// Token: 0x06006F52 RID: 28498 RVA: 0x00338D3C File Offset: 0x00336F3C
		private void InitializePlayerIcon()
		{
			bool flag = SingletonObject.getInstance<DlcManager>().IsDlcInstalled(DlcManager.DlcIdFiveLoong);
			if (flag)
			{
				ViewWorldMap.<>c__DisplayClass300_0 CS$<>8__locals1 = new ViewWorldMap.<>c__DisplayClass300_0();
				CS$<>8__locals1.<>4__this = this;
				CS$<>8__locals1.playerIcon = this._player.GetComponent<WorldMapPlayerInfo>().goPlayerIcon;
				CS$<>8__locals1.effectRoot = "RemakeResources/Particle/MapBlockEffect/WorldUI";
				CS$<>8__locals1.<InitializePlayerIcon>g__AddPlayerIconEffect|0(CommonUtils.GetTaiwuSpriteName(1), "eff_worlduiui_debuff_nan");
				CS$<>8__locals1.<InitializePlayerIcon>g__AddPlayerIconEffect|0(CommonUtils.GetTaiwuSpriteName(0), "eff_worlduiui_debuff_nv");
				CS$<>8__locals1.<InitializePlayerIcon>g__AddPlayerIconEffect|0("LoongInfo", "eff_worldui_wuqiu");
			}
			this.UpdatePlayerIconOnMap(false);
		}

		// Token: 0x06006F53 RID: 28499 RVA: 0x00338DCC File Offset: 0x00336FCC
		private void InitializeAllSectStoryEffectContainers()
		{
			this._sectStoryEffectContainers.Clear();
			this._sectStoryEffectContainers.Add(this.BloodLightContainer);
			this._sectStoryEffectContainers.Add(this.FairylandContainer);
			this._sectStoryEffectContainers.Add(this.LingbaoLightContainer);
			this._sectStoryEffectContainers.Add(this.LingbaoDarkContainer);
			this._sectStoryEffectContainers.Add(this.BloodContainer);
			this._sectStoryEffectContainers.Add(this.ProfessionTravelerStationExistContainer);
			this._sectStoryEffectContainers.Add(this.ProfessionTravelerStationUpContainer);
			this._sectStoryEffectContainers.Add(this.ProfessionTravelerStationDownContainer);
			this._sectStoryEffectContainers.Add(this.ProfessionTravelerSkillTwoContainer);
			this._sectStoryEffectContainers.Add(this.ProfessionTravelerSkillThreeMoveStartMaleContainer);
			this._sectStoryEffectContainers.Add(this.ProfessionTravelerSkillThreeMoveStartFemaleContainer);
			this._sectStoryEffectContainers.Add(this.ProfessionTravelerSkillThreeMoveEndMaleContainer);
			this._sectStoryEffectContainers.Add(this.ProfessionTravelerSkillThreeMoveEndFemaleContainer);
			this._sectStoryEffectContainers.Add(this.swordTombAppearContainer);
			this._sectStoryEffectContainers.Add(this.swordTombDisappearContainer);
			this._sectStoryEffectContainers.Add(this.taiwuVillageStationContainer);
			this._sectStoryEffectContainers.Add(this.divineFlameJiuhanBadContainer);
			foreach (SectStoryEffectContainer container in this._sectStoryEffectContainers)
			{
				container.PosGenerator = new MapElementPosGenerator(MapRenderSystem.GetBlockLocalPos);
			}
			this.InFlameAreaContainer.PosGenerator = new MapElementPosGenerator(MapRenderSystem.GetBlockLocalPos);
			this.InFlameAreaContainer.GetPrefabIndexByLocation = new Func<Location, short, int>(this.GetFulongFlamePrefabByLocation);
			this.SettlementBlockConatiner.PosGenerator = new MapElementPosGenerator(MapRenderSystem.GetBlockLocalPos);
			this.SettlementBlockConatiner.GetPrefabIndexByLocation = new Func<Location, short, int>(this.GetSettlementBlockPrefabByLocation);
			this.SettlementBlockConatiner.GetSinglePrefabIndexByLocation = new Func<Location, short, int>(this.GetSettlementBlockSinglePrefabByLocation);
			bool flag = this.CharacterImpactRangeContainer != null;
			if (flag)
			{
				this.CharacterImpactRangeContainer.PosGenerator = new MapElementPosGenerator(MapRenderSystem.GetBlockLocalPos);
				this.CharacterImpactRangeContainer.GetPrefabIndexByLocation = new Func<Location, short, int>(this.CharacterImpactRangeContainer.GetPrefabIndex);
				this.CharacterImpactRangeContainer.UseSettlementEffectPrefabs(this.SettlementBlockConatiner);
			}
		}

		// Token: 0x06006F54 RID: 28500 RVA: 0x00339040 File Offset: 0x00337240
		private void TryRefreshSectStoryEffects(Location location, bool visible)
		{
			foreach (SectStoryEffectContainer container in this._sectStoryEffectContainers)
			{
				container.TryRefresh(location, visible);
			}
			this.InFlameAreaContainer.TryRefresh(location, visible, 0);
			MapBlockCharacterImpactRangeEffectContainer characterImpactRangeContainer = this.CharacterImpactRangeContainer;
			if (characterImpactRangeContainer != null)
			{
				characterImpactRangeContainer.TryRefresh(location, visible, 0);
			}
		}

		// Token: 0x06006F55 RID: 28501 RVA: 0x003390BC File Offset: 0x003372BC
		private void ClearSectStoryEffects()
		{
			foreach (SectStoryEffectContainer container in this._sectStoryEffectContainers)
			{
				container.Clear();
			}
			this.InFlameAreaContainer.Clear();
			MapBlockCharacterImpactRangeEffectContainer characterImpactRangeContainer = this.CharacterImpactRangeContainer;
			if (characterImpactRangeContainer != null)
			{
				characterImpactRangeContainer.Hide();
			}
		}

		// Token: 0x06006F56 RID: 28502 RVA: 0x00339130 File Offset: 0x00337330
		private void RefreshAllSectStoryEffects()
		{
			this.RefreshBloodLights();
			this.RefreshFairylands();
			this.RefreshLingbaoLight();
			this.RefreshLingbaoDark();
			this.RefreshBloods();
			this.RefreshEmeiDuidance();
			this.RefreshHeavenTree();
		}

		// Token: 0x06006F57 RID: 28503 RVA: 0x00339164 File Offset: 0x00337364
		private void RefreshBloodLights()
		{
			bool flag = this._mapModel.SectXuehouBloodLightLocations == null || this._mapModel.SectXuehouBloodLightLocations.Count == 0;
			if (flag)
			{
				this.BloodLightContainer.Clear();
			}
			else
			{
				this.BloodLightContainer.RefreshAll(this._mapModel.SectXuehouBloodLightLocations);
			}
		}

		// Token: 0x06006F58 RID: 28504 RVA: 0x003391C0 File Offset: 0x003373C0
		private void RefreshFairylands()
		{
			bool flag = this._mapModel.SectWudangFairylandData == null || this._mapModel.SectWudangFairylandData.Count == 0;
			if (flag)
			{
				this.FairylandContainer.Clear();
			}
			else
			{
				List<Location> fairylandLocations = EasyPool.Get<List<Location>>();
				fairylandLocations.Clear();
				fairylandLocations.AddRange(from x in this._mapModel.SectWudangFairylandData
				where !x.Destroyed && !x.Visited
				select x.Location);
				this.FairylandContainer.RefreshAll(fairylandLocations);
				EasyPool.Free<List<Location>>(fairylandLocations);
			}
		}

		// Token: 0x06006F59 RID: 28505 RVA: 0x00339280 File Offset: 0x00337480
		private void RefreshHeavenTree()
		{
			bool flag = this._mapModel.SectWudangHeavenlyTreeList == null;
			if (!flag)
			{
				List<Location> temp = EasyPool.Get<List<Location>>();
				foreach (SectStoryHeavenlyTreeExtendable heavenlyTree in this._mapModel.SectWudangHeavenlyTreeList)
				{
					temp.Add(heavenlyTree.Location);
					this.UpdateSingleCharacter(heavenlyTree.Location);
				}
				foreach (Location lastHeavenTreeLocation in this._lastHeavenTreeLocations)
				{
					bool flag2 = !temp.Contains(lastHeavenTreeLocation);
					if (flag2)
					{
						this.UpdateSingleCharacter(lastHeavenTreeLocation);
					}
				}
				this._lastHeavenTreeLocations.Clear();
				this._lastHeavenTreeLocations.AddRange(temp);
				EasyPool.Free<List<Location>>(temp);
			}
		}

		// Token: 0x06006F5A RID: 28506 RVA: 0x00339388 File Offset: 0x00337588
		private void RefreshLingbaoLight()
		{
			bool flag = this._mapModel.SectWudangLingBaoLight == null || this._mapModel.SectWudangLingBaoLight.Count == 0;
			if (flag)
			{
				this.LingbaoLightContainer.Clear();
			}
			else
			{
				this.LingbaoLightContainer.RefreshAll(this._mapModel.SectWudangLingBaoLight);
			}
		}

		// Token: 0x06006F5B RID: 28507 RVA: 0x003393E4 File Offset: 0x003375E4
		private void RefreshLingbaoDark()
		{
			bool flag = this._mapModel.SectWudangLingBaoDark == null || this._mapModel.SectWudangLingBaoDark.Count == 0;
			if (flag)
			{
				this.LingbaoDarkContainer.Clear();
			}
			else
			{
				this.LingbaoDarkContainer.RefreshAll(this._mapModel.SectWudangLingBaoDark);
			}
		}

		// Token: 0x06006F5C RID: 28508 RVA: 0x00339440 File Offset: 0x00337640
		private void RefreshBloods()
		{
			bool flag = this._mapModel.SectEmeiBloodLocations == null || this._mapModel.SectEmeiBloodLocations.Count == 0;
			if (flag)
			{
				this.BloodContainer.Clear();
			}
			else
			{
				this.BloodContainer.RefreshAll(this._mapModel.SectEmeiBloodLocations);
			}
		}

		// Token: 0x06006F5D RID: 28509 RVA: 0x0033949A File Offset: 0x0033769A
		private void RefreshEmeiDuidance()
		{
			this.UpdateAllContainers();
		}

		// Token: 0x06006F5E RID: 28510 RVA: 0x003394A4 File Offset: 0x003376A4
		private void RefreshFulongInFlameAreas()
		{
			bool flag = this._mapModel.SectFulongInFlameAreas == null || this._mapModel.SectFulongInFlameAreas.Count == 0;
			if (flag)
			{
				this.InFlameAreaContainer.Clear();
			}
			else
			{
				List<Location> locations = EasyPool.Get<List<Location>>();
				foreach (FulongInFlameArea area in this._mapModel.SectFulongInFlameAreas)
				{
					foreach (short blockId in area.EdgeBlocks.Keys)
					{
						locations.Add(new Location(area.AreaId, blockId));
					}
				}
				this.InFlameAreaContainer.RefreshAll(locations);
				EasyPool.Free<List<Location>>(locations);
			}
			foreach (MapElementContainer container in this._activateMapElements.Values)
			{
				container.UpdateFulongFlame();
			}
			List<MapBlockData> neighbors = EasyPool.Get<List<MapBlockData>>();
			this._mapModel.GetNeighborList(this._mapModel.CurrentAreaId, this._mapModel.CurrentBlockId, neighbors, 1, false);
			neighbors.Add(this._mapModel.PlayerAtBlock);
			foreach (MapBlockData neighbor in neighbors)
			{
				this.ProcessBlock(neighbor);
			}
			EasyPool.Free<List<MapBlockData>>(neighbors);
		}

		// Token: 0x06006F5F RID: 28511 RVA: 0x0033967C File Offset: 0x0033787C
		private int GetFulongFlamePrefabByLocation(Location location, short settlementBlockId)
		{
			foreach (FulongInFlameArea area in this._mapModel.SectFulongInFlameAreas)
			{
				sbyte neighborBit;
				bool flag = area.EdgeBlocks.TryGetValue(location.BlockId, out neighborBit);
				if (flag)
				{
					if (!true)
					{
					}
					int result;
					switch (neighborBit)
					{
					case 1:
						result = 7;
						goto IL_A4;
					case 2:
						result = 5;
						goto IL_A4;
					case 3:
						result = 6;
						goto IL_A4;
					case 4:
						result = 2;
						goto IL_A4;
					case 5:
						result = 4;
						goto IL_A4;
					case 8:
						result = 0;
						goto IL_A4;
					case 10:
						result = 3;
						goto IL_A4;
					case 12:
						result = 1;
						goto IL_A4;
					}
					result = -1;
					IL_A4:
					if (!true)
					{
					}
					return result;
				}
			}
			return -1;
		}

		// Token: 0x06006F60 RID: 28512 RVA: 0x0033976C File Offset: 0x0033796C
		private void RefreshAllZhujianThief()
		{
			foreach (MapElementContainer container in this._activateMapElements.Values)
			{
				container.UpdateZhujianThief();
			}
		}

		// Token: 0x06006F61 RID: 28513 RVA: 0x003397C8 File Offset: 0x003379C8
		private void RefreshProfessionTravelerStationExistEffect()
		{
			List<Location> locations = EasyPool.Get<List<Location>>();
			ViewWorldMap.GetProfessionTravelerStationLocations(locations);
			bool flag = locations.Count == 0;
			if (flag)
			{
				this.ProfessionTravelerStationExistContainer.Clear();
			}
			else
			{
				this.ProfessionTravelerStationExistContainer.RefreshAll(locations);
				RectTransform containerMapBlockRoot = this.ProfessionTravelerStationExistContainer.mapBlockRoot;
				ParticleSystem[] particleSystems = containerMapBlockRoot.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem particle in particleSystems)
				{
					particle.Play();
				}
			}
			EasyPool.Free<List<Location>>(locations);
		}

		// Token: 0x06006F62 RID: 28514 RVA: 0x00339850 File Offset: 0x00337A50
		private static void GetProfessionTravelerStationLocations(List<Location> locations)
		{
			ProfessionModel model = SingletonObject.getInstance<ProfessionModel>();
			ProfessionData professionData;
			bool flag = model.TaiwuProfessions.TryGetValue(11, out professionData);
			if (flag)
			{
				TravelerSkillsData skillData = professionData.GetSkillsData<TravelerSkillsData>();
				for (int i = 0; i < skillData.PalaceCount; i++)
				{
					TravelerPalaceData palaceData = skillData.TryGetPalaceData(i);
					bool flag2 = palaceData != null;
					if (flag2)
					{
						locations.Add(palaceData.Location);
					}
				}
			}
		}

		// Token: 0x06006F63 RID: 28515 RVA: 0x003398C4 File Offset: 0x00337AC4
		private void RefreshProfessionTravelerStationDownEffect()
		{
			ProfessionModel model = SingletonObject.getInstance<ProfessionModel>();
			List<Location> locations = EasyPool.Get<List<Location>>();
			ProfessionData professionData;
			bool flag = model.TaiwuProfessions.TryGetValue(11, out professionData);
			if (flag)
			{
				TravelerSkillsData skillData = professionData.GetSkillsData<TravelerSkillsData>();
				for (int i = 0; i < skillData.PalaceCount; i++)
				{
					TravelerPalaceData palaceData = skillData.TryGetPalaceData(i);
					bool flag2 = palaceData != null && palaceData.Location.AreaId == this._mapModel.CurrentAreaId && palaceData.Location.BlockId == this._lastBlockId;
					if (flag2)
					{
						locations.Add(palaceData.Location);
					}
				}
			}
			bool flag3 = locations.Count == 0;
			if (flag3)
			{
				this.ProfessionTravelerStationDownContainer.Clear();
			}
			else
			{
				this.ProfessionTravelerStationDownContainer.RefreshAll(locations);
				RectTransform containerMapBlockRoot = this.ProfessionTravelerStationDownContainer.mapBlockRoot;
				ParticleSystem[] particleSystems = containerMapBlockRoot.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem particle in particleSystems)
				{
					particle.Play();
				}
			}
			EasyPool.Free<List<Location>>(locations);
		}

		// Token: 0x06006F64 RID: 28516 RVA: 0x003399E0 File Offset: 0x00337BE0
		private void RefreshProfessionTravelerStationSound(Location lastLocation, Location newLocation)
		{
			List<Location> locations = EasyPool.Get<List<Location>>();
			ViewWorldMap.GetProfessionTravelerStationLocations(locations);
			bool isBuildingAreaOpen = UIManager.Instance.IsFocusElement(UIElement.BuildingArea);
			bool lastIn = locations.Contains(lastLocation);
			bool newIn = locations.Contains(newLocation);
			bool flag = newIn;
			if (flag)
			{
				AudioManager.Instance.PlaySound("SFX_professionskill_lvren_play", false, false);
			}
			bool flag2 = lastIn;
			if (flag2)
			{
				AudioManager.Instance.PlaySound("SFX_professionskill_lvren_stop", false, false);
			}
			bool flag3 = isBuildingAreaOpen;
			if (flag3)
			{
				AudioManager.Instance.StopLoopSoundWithAmbience("SFX_professionskill_lvren_loop");
			}
			else
			{
				bool flag4 = !lastIn && newIn;
				if (flag4)
				{
					AudioManager.Instance.StopLoopSoundWithAmbience("SFX_professionskill_lvren_loop");
					AudioManager.Instance.PlayLoopSoundWithAmbience("SFX_professionskill_lvren_loop");
				}
				bool flag5 = lastIn && !newIn;
				if (flag5)
				{
					AudioManager.Instance.StopLoopSoundWithAmbience("SFX_professionskill_lvren_loop");
				}
			}
			EasyPool.Free<List<Location>>(locations);
		}

		// Token: 0x06006F65 RID: 28517 RVA: 0x00339AC8 File Offset: 0x00337CC8
		private void TryStartProfessionTravelerStationSound()
		{
			Location currentLoation = this._mapModel.CurrentLocation;
			List<Location> locations = EasyPool.Get<List<Location>>();
			ViewWorldMap.GetProfessionTravelerStationLocations(locations);
			bool isBuildingAreaOpen = UIManager.Instance.IsFocusElement(UIElement.BuildingArea);
			bool flag = locations.Count > 0 && locations.Contains(currentLoation) && !isBuildingAreaOpen;
			if (flag)
			{
				AudioManager.Instance.StopLoopSoundWithAmbience("SFX_professionskill_lvren_loop");
				AudioManager.Instance.PlayLoopSoundWithAmbience("SFX_professionskill_lvren_loop");
			}
			EasyPool.Free<List<Location>>(locations);
		}

		// Token: 0x06006F66 RID: 28518 RVA: 0x00339B48 File Offset: 0x00337D48
		private void RefreshProfessionTravelerStationUpEffect()
		{
			ProfessionModel model = SingletonObject.getInstance<ProfessionModel>();
			List<Location> locations = EasyPool.Get<List<Location>>();
			ProfessionData professionData;
			bool flag = model.TaiwuProfessions.TryGetValue(11, out professionData);
			if (flag)
			{
				TravelerSkillsData skillData = professionData.GetSkillsData<TravelerSkillsData>();
				for (int i = 0; i < skillData.PalaceCount; i++)
				{
					TravelerPalaceData palaceData = skillData.TryGetPalaceData(i);
					bool flag2 = palaceData != null && palaceData.Location.AreaId == this._mapModel.CurrentAreaId && palaceData.Location.BlockId == this._mapModel.CurrentBlockId;
					if (flag2)
					{
						locations.Add(palaceData.Location);
					}
				}
			}
			bool flag3 = locations.Count == 0;
			if (flag3)
			{
				this.ProfessionTravelerStationUpContainer.Clear();
			}
			else
			{
				this.ProfessionTravelerStationUpContainer.RefreshAll(locations);
				RectTransform containerMapBlockRoot = this.ProfessionTravelerStationUpContainer.mapBlockRoot;
				ParticleSystem[] particleSystems = containerMapBlockRoot.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem particle in particleSystems)
				{
					particle.Play();
				}
			}
			EasyPool.Free<List<Location>>(locations);
		}

		// Token: 0x06006F67 RID: 28519 RVA: 0x00339C6C File Offset: 0x00337E6C
		private void ProfessionTravelerSkillThreeMove(ArgumentBox argumentBox)
		{
			int index;
			argumentBox.Get("Index", out index);
			short targetAreaId;
			argumentBox.Get("TargetAreaId", out targetAreaId);
			base.StartCoroutine(this.ProfessionTravelerSkillThreeMove(index, targetAreaId != this._mapModel.CurrentAreaId));
		}

		// Token: 0x06006F68 RID: 28520 RVA: 0x00339CB5 File Offset: 0x00337EB5
		private IEnumerator ProfessionTravelerSkillThreeMove(int index, bool showBlackMask)
		{
			yield return new WaitForSeconds(0.5f);
			List<Location> locations = EasyPool.Get<List<Location>>();
			locations.Clear();
			locations.Add(this._mapModel.CurrentLocation);
			this._player.gameObject.SetActive(false);
			this.GetProfessionTravelerSkillThreeMoveStartContainer().RefreshAll(locations);
			AudioManager.Instance.PlaySound("SFX_professionskill_lvren_enter", false, false);
			if (showBlackMask)
			{
				yield return new WaitForSeconds(0.4f);
			}
			if (showBlackMask)
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Set("AnimToShowMask", true);
				argBox.Set("AnimTime", 0.5f);
				argBox.Set("HideAfterShow", false);
				UIElement.BlackMask.SetOnInitArgs(argBox);
				UIElement.BlackMask.Show();
				argBox = null;
			}
			bool flag = !showBlackMask;
			if (flag)
			{
				yield return new WaitForSeconds(0.5f);
			}
			MapDomainMethod.Call.TeleportOnTravelerPalace(this.Element.GameDataListenerId, index);
			yield return new WaitForSeconds(0.2f);
			yield return new WaitUntil(() => WorldMapModel.MapBlockLoadFinish && WorldMapModel.MapBlockUiLoadFinish && WorldMapModel.MapBlockRenderFinish);
			if (showBlackMask)
			{
				ArgumentBox argBox2 = EasyPool.Get<ArgumentBox>();
				argBox2.Set("AnimToShowMask", false);
				argBox2.Set("AnimTime", 0.5f);
				argBox2.Set("HideAfterShow", true);
				UIElement.BlackMask.SetOnInitArgs(argBox2);
				UIElement.BlackMask.UiBaseAs<UI_BlackMask>().Refresh(argBox2, true);
				argBox2 = null;
			}
			this._player.gameObject.SetActive(false);
			yield return new WaitForSeconds(0.5f);
			locations.Clear();
			locations.Add(this._mapModel.CurrentLocation);
			this.GetProfessionTravelerSkillThreeMoveEndContainer().RefreshAll(locations);
			AudioManager.Instance.PlaySound("SFX_professionskill_lvren_exit", false, false);
			yield return new WaitForSeconds(0.1f);
			this._player.gameObject.SetActive(true);
			yield return new WaitForSeconds(1f);
			this.GetProfessionTravelerSkillThreeMoveStartContainer().Clear();
			this.GetProfessionTravelerSkillThreeMoveEndContainer().Clear();
			EasyPool.Free<List<Location>>(locations);
			yield break;
		}

		// Token: 0x06006F69 RID: 28521 RVA: 0x00339CD2 File Offset: 0x00337ED2
		private void ProfessionTravelerSkillTwoMoveStart(ArgumentBox argumentBox)
		{
			base.StartCoroutine(this.ProfessionTravelerSkillTwoMoveStart());
		}

		// Token: 0x06006F6A RID: 28522 RVA: 0x00339CE2 File Offset: 0x00337EE2
		private IEnumerator ProfessionTravelerSkillTwoMoveStart()
		{
			List<Location> locations = EasyPool.Get<List<Location>>();
			locations.Clear();
			locations.Add(this._mapModel.CurrentLocation);
			this._player.gameObject.SetActive(false);
			this.GetProfessionTravelerSkillThreeMoveStartContainer().RefreshAll(locations);
			yield return new WaitForSeconds(0.5f);
			this.GetProfessionTravelerSkillThreeMoveStartContainer().Clear();
			EasyPool.Free<List<Location>>(locations);
			yield break;
		}

		// Token: 0x06006F6B RID: 28523 RVA: 0x00339CF1 File Offset: 0x00337EF1
		private void ProfessionTravelerSkillTwoMoveEnd(ArgumentBox argumentBox)
		{
			base.StartCoroutine(this.ProfessionTravelerSkillTwoMoveEnd());
		}

		// Token: 0x06006F6C RID: 28524 RVA: 0x00339D01 File Offset: 0x00337F01
		private void OnHideBuildingArea(ArgumentBox argBox)
		{
			this.RefreshProfessionTravelerStationSound(this._mapModel.LastAtLocation, this._mapModel.CurrentLocation);
		}

		// Token: 0x06006F6D RID: 28525 RVA: 0x00339D21 File Offset: 0x00337F21
		private void OnJieqingSignStateRefresh(ArgumentBox argBox)
		{
			this.UpdateAllContainers();
		}

		// Token: 0x06006F6E RID: 28526 RVA: 0x00339D2B File Offset: 0x00337F2B
		private void OnShowBuildingArea(ArgumentBox argBox)
		{
			this.RefreshProfessionTravelerStationSound(this._mapModel.LastAtLocation, this._mapModel.CurrentLocation);
		}

		// Token: 0x06006F6F RID: 28527 RVA: 0x00339D4B File Offset: 0x00337F4B
		private IEnumerator ProfessionTravelerSkillTwoMoveEnd()
		{
			List<Location> locations = EasyPool.Get<List<Location>>();
			locations.Clear();
			locations.Add(this._mapModel.CurrentLocation);
			this._player.gameObject.SetActive(true);
			this.GetProfessionTravelerSkillThreeMoveEndContainer().RefreshAll(locations);
			yield return new WaitForSeconds(0.5f);
			this.GetProfessionTravelerSkillThreeMoveEndContainer().Clear();
			EasyPool.Free<List<Location>>(locations);
			yield break;
		}

		// Token: 0x06006F70 RID: 28528 RVA: 0x00339D5C File Offset: 0x00337F5C
		private SectStoryEffectContainer GetProfessionTravelerSkillThreeMoveStartContainer()
		{
			bool flag = this._mapModel.TaiwuGender == 1;
			SectStoryEffectContainer result;
			if (flag)
			{
				result = this.ProfessionTravelerSkillThreeMoveStartMaleContainer;
			}
			else
			{
				result = this.ProfessionTravelerSkillThreeMoveStartFemaleContainer;
			}
			return result;
		}

		// Token: 0x06006F71 RID: 28529 RVA: 0x00339D90 File Offset: 0x00337F90
		private SectStoryEffectContainer GetProfessionTravelerSkillThreeMoveEndContainer()
		{
			bool flag = this._mapModel.TaiwuGender == 1;
			SectStoryEffectContainer result;
			if (flag)
			{
				result = this.ProfessionTravelerSkillThreeMoveEndMaleContainer;
			}
			else
			{
				result = this.ProfessionTravelerSkillThreeMoveEndFemaleContainer;
			}
			return result;
		}

		// Token: 0x06006F72 RID: 28530 RVA: 0x00339DC4 File Offset: 0x00337FC4
		private void OpenStatePartWorldMap()
		{
			bool atPastTaiwuVillage = this._mapModel.AtPastTaiwuVillage;
			if (!atPastTaiwuVillage)
			{
				CommandManager.AddCommandShowUI(EPriority.ShowUINormal, UIElement.StatePartWorldMap, null);
			}
		}

		// Token: 0x06006F73 RID: 28531 RVA: 0x00339DF4 File Offset: 0x00337FF4
		private void ProfessionTravelerSkillTwoStart(ArgumentBox argumentBox)
		{
			this._professionTravelerSkillTwoActive = true;
			this._mapModel.ChangeTaiwuMoveState(WorldMapModel.MoveState.ProfessionTravelerSkill);
			UIManager.Instance.SetEscHandler(new Action(this.ProfessionTravelerSkillTwoCancel));
			this.ClearMovePath();
			ConchShipCursor.Instance.RemoveMouseWheelProgressFullHandler(new Action(this.OpenStatePartWorldMap));
			GEvent.OnEvent(UiEvents.PlayAnimToHideMainUI, null);
			ProfessionModel model = SingletonObject.getInstance<ProfessionModel>();
			List<MapBlockData> mapBlockDatas = EasyPool.Get<List<MapBlockData>>();
			this._professionTravelerSkillTwoCanSelectLocations.Clear();
			ProfessionData professionData;
			bool flag = model.TaiwuProfessions.TryGetValue(11, out professionData);
			if (flag)
			{
				int distance = professionData.SeniorityToTeleportDistance();
				this._mapModel.GetNeighborList(this._mapModel.CurrentAreaId, this._mapModel.CurrentBlockId, mapBlockDatas, distance, true);
				foreach (MapBlockData mapBlockData in mapBlockDatas)
				{
					this._professionTravelerSkillTwoCanSelectLocations.Add(mapBlockData.GetLocation());
				}
				this._professionTravelerSkillTwoCanSelectLocations.Add(this._mapModel.CurrentLocation);
				float duration = 0.5f;
				bool flag2 = this._professionTravelerSkillTwoCanSelectLocations.Count == 0;
				if (flag2)
				{
					this.HideBlockAndLine(duration);
				}
				else
				{
					this.ShowBlockAndLine(duration, this._professionTravelerSkillTwoCanSelectLocations);
				}
				SingletonObject.getInstance<MapRenderSystem>().AdjustMapBlockAdditionalLightState(distance, false);
			}
			EasyPool.Free<List<MapBlockData>>(mapBlockDatas);
		}

		// Token: 0x06006F74 RID: 28532 RVA: 0x00339F70 File Offset: 0x00338170
		private void ProfessionTravelerSkillTwoStop(ArgumentBox argumentBox)
		{
			this._professionTravelerSkillTwoActive = false;
			this._mapModel.ChangeTaiwuMoveState(WorldMapModel.MoveState.Idle);
			this.HideBlockAndLine(0.5f);
			SingletonObject.getInstance<MapRenderSystem>().ClearMapBlockAdditionalLightState(false);
			ConchShipCursor.Instance.AddMouseWheelProgressFullHandler(this.Element.Name, new Action(this.OpenStatePartWorldMap));
			this._selectVirtual.gameObject.SetActive(false);
			GEvent.OnEvent(UiEvents.PlayAnimToShowMainUI, null);
		}

		// Token: 0x06006F75 RID: 28533 RVA: 0x00339FF0 File Offset: 0x003381F0
		private void ShowBlockAndLine(float duration, List<Location> inviteSelectBlockLocations)
		{
			CanvasGroup lineRootCanvasGroup = this.ProfessionTravelerSkillTwoContainer.professionTravelerSkillTwoLine2dHolder.GetComponent<CanvasGroup>();
			lineRootCanvasGroup.alpha = 0f;
			lineRootCanvasGroup.DOFade(1f, duration).SetEase(Ease.InOutQuad).SetAutoKill(true);
			this.DrawBlockLines(this._mapModel.CurrentLocation, inviteSelectBlockLocations);
			RectTransform blockRoot = this.ProfessionTravelerSkillTwoContainer.mapBlockRoot;
			this.ProfessionTravelerSkillTwoContainer.RefreshAll(inviteSelectBlockLocations);
			for (int i = 0; i < blockRoot.childCount; i++)
			{
				Transform child = blockRoot.GetChild(i);
				ParticleSystemAlphaController controller = child.GetComponent<ParticleSystemAlphaController>();
				controller.SetActiveNoTween(false);
				controller.SetActiveWithTween(true, duration, Ease.InOutQuad);
			}
		}

		// Token: 0x06006F76 RID: 28534 RVA: 0x0033A0A0 File Offset: 0x003382A0
		private void HideBlockAndLine(float duration)
		{
			CanvasGroup lineRootCanvasGroup = this.ProfessionTravelerSkillTwoContainer.professionTravelerSkillTwoLine2dHolder.GetComponent<CanvasGroup>();
			lineRootCanvasGroup.DOFade(0f, duration).SetEase(Ease.InOutQuad).SetAutoKill(true).OnComplete(delegate
			{
				this.ProfessionTravelerSkillTwoContainer.Clear();
				this.ClearLines();
			});
			RectTransform blockRoot = this.ProfessionTravelerSkillTwoContainer.mapBlockRoot;
			for (int i = 0; i < blockRoot.childCount; i++)
			{
				Transform child = blockRoot.GetChild(i);
				ParticleSystemAlphaController controller = child.GetComponent<ParticleSystemAlphaController>();
				controller.SetActiveWithTween(false, duration, Ease.InOutQuad);
			}
		}

		// Token: 0x06006F77 RID: 28535 RVA: 0x0033A12C File Offset: 0x0033832C
		private void ProfessionSkillConfirmSelectCancel(ArgumentBox argumentBox)
		{
			bool professionTravelerSkillTwoActive = this._professionTravelerSkillTwoActive;
			if (professionTravelerSkillTwoActive)
			{
				UIManager.Instance.SetEscHandler(new Action(this.ProfessionTravelerSkillTwoCancel));
			}
		}

		// Token: 0x06006F78 RID: 28536 RVA: 0x0033A15D File Offset: 0x0033835D
		private void ProfessionTravelerSkillTwoCancel()
		{
			GEvent.OnEvent(UiEvents.ProfessionTravelerSkillTwoStop, null);
		}

		// Token: 0x06006F79 RID: 28537 RVA: 0x0033A174 File Offset: 0x00338374
		private void DrawBlockLines(Location centerLocation, List<Location> locations)
		{
			byte areaSize = this._mapModel.GetAreaSize(centerLocation.AreaId);
			RectTransform line2DHolder = this.ProfessionTravelerSkillTwoContainer.professionTravelerSkillTwoLine2dHolder;
			Line2DGenerator line2DTemplate = this.ProfessionTravelerSkillTwoContainer.professionTravelerSkillTwoLine2dTemp;
			for (int i = 0; i < locations.Count; i++)
			{
				Location currLocation = locations[i];
				ByteCoordinate currBlockPos = WorldMapModel.IndexToCoordinate(currLocation.BlockId, areaSize);
				bool hasRightNeighbor = false;
				bool hasLeftNeighbor = false;
				bool hasUpNeighbor = false;
				bool hasDownNeighbor = false;
				for (int j = 0; j < locations.Count; j++)
				{
					bool flag = j == i;
					if (!flag)
					{
						Location otherLocation = locations[j];
						ByteCoordinate otherBlockPos = WorldMapModel.IndexToCoordinate(otherLocation.BlockId, areaSize);
						bool flag2 = this.IsRightNeighbor(currBlockPos, otherBlockPos);
						if (flag2)
						{
							hasRightNeighbor = true;
						}
						bool flag3 = this.IsLeftNeighbor(currBlockPos, otherBlockPos);
						if (flag3)
						{
							hasLeftNeighbor = true;
						}
						bool flag4 = this.IsUpNeighbor(currBlockPos, otherBlockPos);
						if (flag4)
						{
							hasUpNeighbor = true;
						}
						bool flag5 = this.IsDownNeighbor(currBlockPos, otherBlockPos);
						if (flag5)
						{
							hasDownNeighbor = true;
						}
					}
				}
				this.DrawBlockLine(currLocation, line2DHolder, line2DTemplate, hasLeftNeighbor, hasRightNeighbor, hasUpNeighbor, hasDownNeighbor);
			}
		}

		// Token: 0x06006F7A RID: 28538 RVA: 0x0033A294 File Offset: 0x00338494
		private void DrawBlockLine(Location currLocation, RectTransform lineHolder, Line2DGenerator line2DTemplate, bool hasLeftNeighbor, bool hasRightNeighbor, bool hasUpNeighbor, bool hasDownNeighbor)
		{
			bool flag = !hasUpNeighbor;
			if (flag)
			{
				Vector2 rightTop = MapRenderSystem.GetBlockLocalPos(currLocation) + new Vector2(MapRenderSystem.MapBlockPosInterval.x, 0f);
				Vector2 leftTop = MapRenderSystem.GetBlockLocalPos(currLocation) + new Vector2(0f, MapRenderSystem.MapBlockPosInterval.y);
				this.DrawLine(lineHolder, line2DTemplate, rightTop, leftTop);
			}
			bool flag2 = !hasDownNeighbor;
			if (flag2)
			{
				Vector2 leftDown = MapRenderSystem.GetBlockLocalPos(currLocation) - new Vector2(MapRenderSystem.MapBlockPosInterval.x, 0f);
				Vector2 rightDown = MapRenderSystem.GetBlockLocalPos(currLocation) - new Vector2(0f, MapRenderSystem.MapBlockPosInterval.y);
				this.DrawLine(lineHolder, line2DTemplate, leftDown, rightDown);
			}
			bool flag3 = !hasLeftNeighbor;
			if (flag3)
			{
				Vector2 leftDown2 = MapRenderSystem.GetBlockLocalPos(currLocation) - new Vector2(MapRenderSystem.MapBlockPosInterval.x, 0f);
				Vector2 leftTop2 = MapRenderSystem.GetBlockLocalPos(currLocation) + new Vector2(0f, MapRenderSystem.MapBlockPosInterval.y);
				this.DrawLine(lineHolder, line2DTemplate, leftDown2, leftTop2);
			}
			bool flag4 = !hasRightNeighbor;
			if (flag4)
			{
				Vector2 rightTop2 = MapRenderSystem.GetBlockLocalPos(currLocation) + new Vector2(MapRenderSystem.MapBlockPosInterval.x, 0f);
				Vector2 rightDown2 = MapRenderSystem.GetBlockLocalPos(currLocation) - new Vector2(0f, MapRenderSystem.MapBlockPosInterval.y);
				this.DrawLine(lineHolder, line2DTemplate, rightTop2, rightDown2);
			}
		}

		// Token: 0x06006F7B RID: 28539 RVA: 0x0033A40C File Offset: 0x0033860C
		private void DrawLine(RectTransform lineHolder, Line2DGenerator line2DTemplate, Vector2 point1, Vector2 point2)
		{
			Line2DGenerator line2DGenerator = this.GetAvailableLine2DGenerator(lineHolder, line2DTemplate);
			line2DGenerator.gameObject.SetActive(true);
			line2DGenerator.Vertices = new List<Vector2>
			{
				point1,
				point2
			}.ToArray();
		}

		// Token: 0x06006F7C RID: 28540 RVA: 0x0033A454 File Offset: 0x00338654
		private Line2DGenerator GetAvailableLine2DGenerator(RectTransform lineHolder, Line2DGenerator line2DTemplate)
		{
			foreach (Line2DGenerator line in this._professionTravelerSkillTwoLineList)
			{
				bool flag = !line.gameObject.activeSelf;
				if (flag)
				{
					return line;
				}
			}
			Line2DGenerator line2DGenerator = Object.Instantiate<Line2DGenerator>(line2DTemplate, lineHolder);
			this._professionTravelerSkillTwoLineList.Add(line2DGenerator);
			return line2DGenerator;
		}

		// Token: 0x06006F7D RID: 28541 RVA: 0x0033A4D8 File Offset: 0x003386D8
		private void ClearLines()
		{
			foreach (Line2DGenerator line in this._professionTravelerSkillTwoLineList)
			{
				line.gameObject.SetActive(false);
			}
		}

		// Token: 0x06006F7E RID: 28542 RVA: 0x0033A538 File Offset: 0x00338738
		private bool IsRightNeighbor(ByteCoordinate curr, ByteCoordinate other)
		{
			return curr.X == other.X - 1 && curr.Y == other.Y;
		}

		// Token: 0x06006F7F RID: 28543 RVA: 0x0033A56C File Offset: 0x0033876C
		private bool IsLeftNeighbor(ByteCoordinate curr, ByteCoordinate other)
		{
			return curr.X == other.X + 1 && curr.Y == other.Y;
		}

		// Token: 0x06006F80 RID: 28544 RVA: 0x0033A5A0 File Offset: 0x003387A0
		private bool IsUpNeighbor(ByteCoordinate curr, ByteCoordinate other)
		{
			return curr.X == other.X && curr.Y == other.Y - 1;
		}

		// Token: 0x06006F81 RID: 28545 RVA: 0x0033A5D4 File Offset: 0x003387D4
		private bool IsDownNeighbor(ByteCoordinate curr, ByteCoordinate other)
		{
			return curr.X == other.X && curr.Y == other.Y + 1;
		}

		// Token: 0x06006F82 RID: 28546 RVA: 0x0033A608 File Offset: 0x00338808
		private void CollectAllElements()
		{
			foreach (MapElementContainer container in this._activateMapElements.Values)
			{
				container.CollectAll();
			}
		}

		// Token: 0x06006F83 RID: 28547 RVA: 0x0033A664 File Offset: 0x00338864
		private void CollectAllContainers()
		{
			foreach (MapElementContainer container in this._activateMapElements.Values)
			{
				container.CollectAll();
				container.UnbindLocation();
				EasyPool.Free<MapElementContainer>(container);
			}
			this._activateMapElements.Clear();
		}

		// Token: 0x06006F84 RID: 28548 RVA: 0x0033A6DC File Offset: 0x003388DC
		private void ScaleAllContainers(float wheel)
		{
			foreach (MapElementContainer container in this._activateMapElements.Values)
			{
				container.ScaleAll(wheel);
			}
		}

		// Token: 0x06006F85 RID: 28549 RVA: 0x0033A73C File Offset: 0x0033893C
		private void UpdateAllContainers()
		{
			foreach (MapElementContainer container in this._activateMapElements.Values)
			{
				container.UpdateAll();
			}
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>().Set("RefreshOnly", true);
			GEvent.OnEvent(UiEvents.PickupDisplayInfoChange, argBox);
		}

		// Token: 0x06006F86 RID: 28550 RVA: 0x0033A7BC File Offset: 0x003389BC
		private MapElementContainer GetElementContainer(Location location)
		{
			MapElementContainer container;
			bool flag = this._activateMapElements.TryGetValue(location, out container);
			MapElementContainer result;
			if (flag)
			{
				result = container;
			}
			else
			{
				container = EasyPool.Get<MapElementContainer>();
				container.BindLocation(this, location);
				this._activateMapElements.Add(location, container);
				result = container;
			}
			return result;
		}

		// Token: 0x06006F87 RID: 28551 RVA: 0x0033A802 File Offset: 0x00338A02
		private void UpdateSingleElement(Location location)
		{
			this.GetElementContainer(location).UpdateAll();
		}

		// Token: 0x06006F88 RID: 28552 RVA: 0x0033A814 File Offset: 0x00338A14
		private void UpdateAllMerchants()
		{
			foreach (MapElementContainer container in this._activateMapElements.Values)
			{
				container.UpdateMerchant();
			}
		}

		// Token: 0x06006F89 RID: 28553 RVA: 0x0033A870 File Offset: 0x00338A70
		private void UpdateAdventureIcons()
		{
			foreach (MapElementContainer container in this._activateMapElements.Values)
			{
				container.UpdateAdventure();
			}
		}

		// Token: 0x06006F8A RID: 28554 RVA: 0x0033A8CC File Offset: 0x00338ACC
		private void CollectAllCrickets()
		{
			foreach (MapElementContainer container in this._activateMapElements.Values)
			{
				container.CollectCricket();
			}
		}

		// Token: 0x06006F8B RID: 28555 RVA: 0x0033A928 File Offset: 0x00338B28
		private void RefreshCricketPlace(bool doClear = false)
		{
			if (doClear)
			{
				this.CollectAllCrickets();
			}
			CharacterDomainMethod.Call.GetInventoryItemAmount(this.Element.GameDataListenerId, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, 12, 18);
			bool flag = this.CheckCatchCricketRefresh();
			if (flag)
			{
				this.RefreshCatchCricketBtn();
			}
			foreach (MapElementContainer container in this._activateMapElements.Values)
			{
				container.UpdateCricket();
			}
			short areaId = this._mapModel.CurrentAreaId;
			CricketPlaceExtraData cricketPlaceExtraData;
			bool flag2 = this._mapModel.CricketPlaceExtraData.TryGetValue(areaId, out cricketPlaceExtraData) && cricketPlaceExtraData != null && cricketPlaceExtraData.ExtraMapUnits != null;
			if (flag2)
			{
				foreach (short blockId in cricketPlaceExtraData.ExtraMapUnits.Keys)
				{
					Location location = new Location(areaId, blockId);
					bool flag3 = !this._activateMapElements.ContainsKey(location);
					if (flag3)
					{
						this.UpdateSingleCricket(location);
					}
				}
			}
			CricketPlaceData cricketPlaceData = this._mapModel.CricketPlaceData[(int)areaId];
			bool flag4 = cricketPlaceData == null;
			if (!flag4)
			{
				foreach (short blockId2 in cricketPlaceData.CricketBlocks)
				{
					Location location2 = new Location(areaId, blockId2);
					bool flag5 = !this._activateMapElements.ContainsKey(location2);
					if (flag5)
					{
						this.UpdateSingleCricket(location2);
					}
				}
			}
		}

		// Token: 0x06006F8C RID: 28556 RVA: 0x0033AAD8 File Offset: 0x00338CD8
		private void UpdateSingleCricket(Location location)
		{
			this.GetElementContainer(location).UpdateCricket();
		}

		// Token: 0x06006F8D RID: 28557 RVA: 0x0033AAE8 File Offset: 0x00338CE8
		private void RefreshSettlementAndStationBtn()
		{
			foreach (MapElementContainer container in this._activateMapElements.Values)
			{
				container.UpdateSettlementAndStationBtn();
			}
			SettlementInfo[] settlementInfos = this._mapModel.Areas[(int)this._mapModel.CurrentAreaId].SettlementInfos;
			for (int i = 0; i < settlementInfos.Length; i++)
			{
				Location location = new Location(this._mapModel.CurrentAreaId, settlementInfos[i].BlockId);
				bool flag = !this._activateMapElements.ContainsKey(location);
				if (flag)
				{
					this.GetElementContainer(location).UpdateSettlementBtn();
				}
			}
			for (int j = 0; j < 9; j++)
			{
				short areaId = this._mapModel.GetAreaIdByStateIndex(j);
				short blockId = this._mapModel.Areas[(int)areaId].StationBlockId;
				Location location2 = new Location(areaId, blockId);
				bool flag2 = !this._activateMapElements.ContainsKey(location2);
				if (flag2)
				{
					this.GetElementContainer(location2).UpdateStationBtn();
				}
			}
		}

		// Token: 0x06006F8E RID: 28558 RVA: 0x0033AC24 File Offset: 0x00338E24
		private void UpdateSettlementAndStationBtnUsable()
		{
			foreach (MapElementContainer container in this._activateMapElements.Values)
			{
				container.UpdateSettlementBtn();
			}
		}

		// Token: 0x06006F8F RID: 28559 RVA: 0x0033AC80 File Offset: 0x00338E80
		private void UpdateStationBtnVisible()
		{
			foreach (MapElementContainer container in this._activateMapElements.Values)
			{
				container.UpdateStationBtn();
			}
		}

		// Token: 0x06006F90 RID: 28560 RVA: 0x0033ACDC File Offset: 0x00338EDC
		private void UpdateSingleCharacter(Location location)
		{
			this.GetElementContainer(location).UpdateCharacter();
		}

		// Token: 0x06006F91 RID: 28561 RVA: 0x0033ACEC File Offset: 0x00338EEC
		private void UpdateMapCharacters()
		{
			foreach (MapElementContainer container in this._activateMapElements.Values)
			{
				container.UpdateCharacter();
			}
			this.RefreshPlayerVisible();
		}

		// Token: 0x06006F92 RID: 28562 RVA: 0x0033AD50 File Offset: 0x00338F50
		private void UpdateAllInfo()
		{
			foreach (MapElementContainer container in this._activateMapElements.Values)
			{
				container.UpdateInfo();
			}
		}

		// Token: 0x06006F93 RID: 28563 RVA: 0x0033ADAC File Offset: 0x00338FAC
		private void UpdateSingleInfo(Location location)
		{
			this.GetElementContainer(location).UpdateInfo();
		}

		// Token: 0x06006F94 RID: 28564 RVA: 0x0033ADBC File Offset: 0x00338FBC
		private int GetSettlementBlockPrefabByLocation(Location location, short settlementId)
		{
			sbyte type = MapBlockEightDirectionType.GetDirectionType(this._areaSettlementBlockMap[settlementId], this._mapModel.GetAreaSize(this._mapModel.CurrentAreaId), location.BlockId);
			if (!true)
			{
			}
			int result;
			switch (type)
			{
			case 1:
				result = 7;
				goto IL_91;
			case 2:
				result = 5;
				goto IL_91;
			case 3:
				result = 6;
				goto IL_91;
			case 4:
				result = 2;
				goto IL_91;
			case 5:
				result = 4;
				goto IL_91;
			case 8:
				result = 0;
				goto IL_91;
			case 10:
				result = 3;
				goto IL_91;
			case 12:
				result = 1;
				goto IL_91;
			}
			result = -1;
			IL_91:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06006F95 RID: 28565 RVA: 0x0033AE64 File Offset: 0x00339064
		private int GetSettlementBlockSinglePrefabByLocation(Location location, short settlementId)
		{
			sbyte type = MapBlockEightDirectionType.GetDirectionType(this._areaSettlementBlockMap[settlementId], this._mapModel.GetAreaSize(this._mapModel.CurrentAreaId), location.BlockId);
			if (!true)
			{
			}
			int num;
			if (type != 7)
			{
				switch (type)
				{
				case 11:
					num = 4;
					goto IL_68;
				case 13:
					num = 2;
					goto IL_68;
				case 14:
					num = 1;
					goto IL_68;
				}
				num = -1;
			}
			else
			{
				num = 8;
			}
			IL_68:
			if (!true)
			{
			}
			int singleType = num;
			return MapBlockEightDirectionType.GetSingleIndex(singleType);
		}

		// Token: 0x06006F96 RID: 28566 RVA: 0x0033AEF0 File Offset: 0x003390F0
		private void CollectAllCosts()
		{
			foreach (MapElementContainer container in this._activateMapElements.Values)
			{
				container.CollectCost();
			}
		}

		// Token: 0x06006F97 RID: 28567 RVA: 0x0033AF4C File Offset: 0x0033914C
		private void CollectSingleCost(Location location)
		{
			this.GetElementContainer(location).CollectCost();
		}

		// Token: 0x06006F98 RID: 28568 RVA: 0x0033AF5C File Offset: 0x0033915C
		private void UpdateSingleCost(Location location)
		{
			this.GetElementContainer(location).UpdateCost();
		}

		// Token: 0x06006F99 RID: 28569 RVA: 0x0033AF6C File Offset: 0x0033916C
		private void RefreshAllRefreshMapBlockInfo()
		{
			List<MapBlockData> areaBlocks = EasyPool.Get<List<MapBlockData>>();
			this._mapModel.GetAreaBlocks(this._mapModel.CurrentAreaId, areaBlocks);
			short blockId = 0;
			while ((int)blockId < areaBlocks.Count)
			{
				MapBlockData blockData = areaBlocks[(int)blockId];
				bool flag = blockData.TemplateId != 126 && this._mapBlockSet.Contains(new Location(this.PlayerAtBlock.AreaId, blockId));
				if (flag)
				{
					this.RefreshMapBlockInfo(blockData);
				}
				blockId += 1;
			}
			this.RefreshCatchCricketBtn();
		}

		// Token: 0x06006F9A RID: 28570 RVA: 0x0033AFF8 File Offset: 0x003391F8
		private void SwitchMapBlockViewMode()
		{
			WorldMapModel.EViewMode newMode = (this._mapModel.ViewMode == WorldMapModel.EViewMode.Normal) ? WorldMapModel.EViewMode.Info : WorldMapModel.EViewMode.Normal;
			this._mapModel.ChangeViewMode(newMode);
			bool flag = newMode == WorldMapModel.EViewMode.Normal;
			if (flag)
			{
				this.ClearNegativeFilm();
				this.UpdateAllContainers();
			}
		}

		// Token: 0x06006F9B RID: 28571 RVA: 0x0033B040 File Offset: 0x00339240
		[Button("更新当前负片效果")]
		private void ShowNegativeFilm()
		{
			List<Location> expectLocations = EasyPool.Get<List<Location>>();
			expectLocations.Clear();
			foreach (Location location in this._mapModel.IndexedAreaBlockDisplayData.Keys)
			{
				bool flag = MapElementExpectPrompt.CheckMaybeExist(location);
				if (flag)
				{
					expectLocations.Add(location);
				}
			}
			SingletonObject.getInstance<MapRenderSystem>().SetNegativeFilmInArea(expectLocations, 0.3f, true);
			EasyPool.Free<List<Location>>(expectLocations);
		}

		// Token: 0x06006F9C RID: 28572 RVA: 0x0033B0D4 File Offset: 0x003392D4
		[Button("清除所有负片效果")]
		private void ClearNegativeFilm()
		{
			SingletonObject.getInstance<MapRenderSystem>().ReverseClearNegativeFilm(0.3f);
		}

		// Token: 0x06006F9D RID: 28573 RVA: 0x0033B0E8 File Offset: 0x003392E8
		private void InviteSelectBlockStart(ArgumentBox argumentBox)
		{
			ViewWorldMap.<>c__DisplayClass375_0 CS$<>8__locals1 = new ViewWorldMap.<>c__DisplayClass375_0();
			CS$<>8__locals1.<>4__this = this;
			this._inviteSelectBlockActive = true;
			this._mapModel.ChangeTaiwuMoveState(WorldMapModel.MoveState.ProfessionTravelerSkill);
			UIManager.Instance.SetEscHandler(new Action(this.InviteSelectBlockCancel));
			this.ClearMovePath();
			ConchShipCursor.Instance.RemoveMouseWheelProgressFullHandler(new Action(this.OpenStatePartWorldMap));
			GEvent.OnEvent(UiEvents.PlayAnimToHideMainUI, null);
			CS$<>8__locals1.mapBlockDatas = EasyPool.Get<List<MapBlockData>>();
			this._inviteSelectBlockLocations.Clear();
			List<Location> tempData = new List<Location>();
			CS$<>8__locals1.distance = 1;
			this._mapModel.GetNeighborList(this._mapModel.CurrentAreaId, this._mapModel.CurrentBlockId, CS$<>8__locals1.mapBlockDatas, CS$<>8__locals1.distance, true);
			foreach (MapBlockData mapBlockData in CS$<>8__locals1.mapBlockDatas)
			{
				tempData.Add(mapBlockData.GetLocation());
			}
			tempData.Add(this._mapModel.CurrentLocation);
			CS$<>8__locals1.duration = 0.5f;
			bool flag = tempData.Count == 0;
			if (flag)
			{
				this.HideBlockAndLine(CS$<>8__locals1.duration);
				CS$<>8__locals1.<InviteSelectBlockStart>g__PostProcess|0();
			}
			else
			{
				for (int i = 0; i < tempData.Count; i++)
				{
					Location tempLocationData = tempData[i];
					bool endFlag = i == tempData.Count - 1;
					ExtraDomainMethod.AsyncCall.CheckLocationHasBeggerSkill1(null, tempLocationData, delegate(int offset, RawDataPool dataPool)
					{
						bool flag2 = false;
						Serializer.Deserialize(dataPool, offset, ref flag2);
						bool flag3 = !flag2;
						if (flag3)
						{
							CS$<>8__locals1.<>4__this._inviteSelectBlockLocations.Add(tempLocationData);
						}
						bool endFlag = endFlag;
						if (endFlag)
						{
							CS$<>8__locals1.<>4__this.ShowBlockAndLine(CS$<>8__locals1.duration, CS$<>8__locals1.<>4__this._inviteSelectBlockLocations);
							CS$<>8__locals1.<InviteSelectBlockStart>g__PostProcess|0();
						}
					});
				}
			}
		}

		// Token: 0x06006F9E RID: 28574 RVA: 0x0033B2B0 File Offset: 0x003394B0
		private void InviteSelectBlockStop(ArgumentBox argumentBox)
		{
			this._inviteSelectBlockActive = false;
			this._mapModel.ChangeTaiwuMoveState(WorldMapModel.MoveState.Idle);
			this.HideBlockAndLine(0.5f);
			SingletonObject.getInstance<MapRenderSystem>().ClearMapBlockAdditionalLightState(false);
			ConchShipCursor.Instance.AddMouseWheelProgressFullHandler(this.Element.Name, new Action(this.OpenStatePartWorldMap));
			this._selectVirtual.gameObject.SetActive(false);
			GEvent.OnEvent(UiEvents.PlayAnimToShowMainUI, null);
		}

		// Token: 0x06006F9F RID: 28575 RVA: 0x0033B330 File Offset: 0x00339530
		private void InviteSelectBlockCancel()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharacterId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
			argBox.Set("CanOperate", true);
			argBox.SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.ItemBase, ECharacterSubPage.None));
			UIElement.CharacterMenu.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
			GEvent.OnEvent(UiEvents.InviteSelectBlockStop, null);
		}

		// Token: 0x06006FA0 RID: 28576 RVA: 0x0033B3AD File Offset: 0x003395AD
		private void OnConfirmSelectInviteCharacter(int characterId)
		{
			TaiwuEventDomainMethod.Call.OnTaiwuTryInvite(characterId, this._inviteBlockLocation);
			GEvent.OnEvent(UiEvents.InviteSelectBlockStop, null);
		}

		// Token: 0x06006FA1 RID: 28577 RVA: 0x0033B3CE File Offset: 0x003395CE
		private void OnCancelSelectInviteCharacter()
		{
			UIManager.Instance.SetEscHandler(new Action(this.InviteSelectBlockCancel));
		}

		// Token: 0x06006FA2 RID: 28578 RVA: 0x0033B3E8 File Offset: 0x003395E8
		private void OpenCharacterMenu(int charId)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharacterId", charId);
			argBox.SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.CharacterBase, ECharacterSubPage.Character));
			UIElement.CharacterMenu.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x06006FA3 RID: 28579 RVA: 0x0033B440 File Offset: 0x00339640
		private bool CharacterDisableCondition(CharacterDisplayDataForUltimateSelect character)
		{
			return character.Invited || character.Trapped || character.PrioritizeActionOverInvite;
		}

		// Token: 0x06006FA4 RID: 28580 RVA: 0x0033B46C File Offset: 0x0033966C
		private void DivineFlameSelectBlockStart(ArgumentBox argumentBox)
		{
			argumentBox.Get("xiangshuAvatarId", out this._xiangshuAvatarId);
			argumentBox.Get<List<Location>>("locationList", out this._divineFlameLocationList);
			this._mapModel.ChangeTaiwuMoveState(WorldMapModel.MoveState.ProfessionTravelerSkill);
			UIManager.Instance.SetEscHandler(new Action(this.DivineFlameSelectBlockCancel));
			this.ClearMovePath();
			ConchShipCursor.Instance.RemoveMouseWheelProgressFullHandler(new Action(this.OpenStatePartWorldMap));
			GEvent.OnEvent(UiEvents.PlayAnimToHideMainUI, null);
			float duration = 0.5f;
			this.ShowBlockAndLine(duration, this._divineFlameLocationList);
			SingletonObject.getInstance<MapRenderSystem>().AdjustMapBlockAdditionalLightState(1, false);
		}

		// Token: 0x06006FA5 RID: 28581 RVA: 0x0033B514 File Offset: 0x00339714
		private void DivineFlameSelectBlockStop(ArgumentBox argumentBox)
		{
			this._xiangshuAvatarId = -1;
			this._mapModel.ChangeTaiwuMoveState(WorldMapModel.MoveState.Idle);
			UIManager.Instance.SetEscHandler(null);
			this.HideBlockAndLine(0.5f);
			SingletonObject.getInstance<MapRenderSystem>().ClearMapBlockAdditionalLightState(false);
			ConchShipCursor.Instance.AddMouseWheelProgressFullHandler(this.Element.Name, new Action(this.OpenStatePartWorldMap));
			this._selectVirtual.gameObject.SetActive(false);
			GEvent.OnEvent(UiEvents.PlayAnimToShowMainUI, null);
		}

		// Token: 0x06006FA6 RID: 28582 RVA: 0x0033B5A0 File Offset: 0x003397A0
		private void DivineFlameSelectBlockCancel()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharacterId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
			argBox.Set("CanOperate", true);
			argBox.SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.ItemBase, ECharacterSubPage.None));
			UIElement.CharacterMenu.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
			this.DivineFlameSelectBlockStop(null);
		}

		// Token: 0x06006FA7 RID: 28583 RVA: 0x0033B614 File Offset: 0x00339814
		private void PastTaiwuVillageSetNegativeFilmInArea()
		{
			bool atPastTaiwuVillage = this._mapModel.AtPastTaiwuVillage;
			if (atPastTaiwuVillage)
			{
				bool flag = SingletonObject.getInstance<MapRenderSystem>().RenderState != MapRenderSystem.EMapState.Negative;
				if (flag)
				{
					base.StartCoroutine(this.PastTaiwuVillageRefreshNegativeFilmInArea());
				}
			}
		}

		// Token: 0x06006FA8 RID: 28584 RVA: 0x0033B654 File Offset: 0x00339854
		private IEnumerator PastTaiwuVillageRefreshNegativeFilmInArea()
		{
			yield return new WaitUntil(() => WorldMapModel.MapBlockLoadFinish && WorldMapModel.MapBlockUiLoadFinish && WorldMapModel.MapBlockRenderFinish);
			SingletonObject.getInstance<MapRenderSystem>().SetNegativeFilmInArea(null, 0.3f, true);
			yield break;
		}

		// Token: 0x06006FA9 RID: 28585 RVA: 0x0033B663 File Offset: 0x00339863
		public void SetDirty()
		{
			this._sortingDirty = true;
		}

		// Token: 0x06006FAA RID: 28586 RVA: 0x0033B66C File Offset: 0x0033986C
		Vector2 IMapRoot.ToPos(Location location)
		{
			return MapRenderSystem.GetBlockLocalPos(location);
		}

		// Token: 0x06006FAB RID: 28587 RVA: 0x0033B674 File Offset: 0x00339874
		RectTransform IMapRoot.Layer2Root(EMapLayer layer, Location location)
		{
			bool flag = layer == EMapLayer.SettlementAndStation;
			RectTransform result;
			if (flag)
			{
				result = this.settlementAndStationBtnRoot;
			}
			else
			{
				bool flag2 = location == SingletonObject.getInstance<WorldMapModel>().CurrentLocation;
				if (flag2)
				{
					result = this.taiwuRoot;
				}
				else
				{
					result = this.commonRoot;
				}
			}
			return result;
		}

		// Token: 0x06006FAC RID: 28588 RVA: 0x0033B6BA File Offset: 0x003398BA
		public void MoveToBlock(short blockId)
		{
			this.MoveToBlock(new Location(this._mapModel.CurrentAreaId, blockId));
		}

		// Token: 0x06006FAD RID: 28589 RVA: 0x0033B6D8 File Offset: 0x003398D8
		public void MoveToBlock(Location location)
		{
			Tester.Assert(location.IsValid() && location.AreaId == this._mapModel.CurrentAreaId, "");
			MapBlockData block = this._mapModel.GetBlockData(location);
			Tester.Assert(block != null, "");
			this.SelectBlock(block);
			this.FindWay(block.GetLocation());
			this._pathMoving = true;
			this._movingByController = true;
			this.MoveToNext();
		}

		// Token: 0x06006FAE RID: 28590 RVA: 0x0033B758 File Offset: 0x00339958
		public void SetCameraToArea(short areaId)
		{
			MapAreaData areaData = this._mapModel.Areas[(int)areaId];
			Location taiwuVillageBlock = this._mapModel.GetTaiwuVillageBlock();
			bool flag = this._mapModel.ShowingAreaId != areaId;
			if (flag)
			{
				this.SetShowingArea(areaId);
			}
			this.MoveCameraTo((areaId == taiwuVillageBlock.AreaId) ? taiwuVillageBlock : new Location(areaId, (areaData.SettlementInfos[0].SettlementId >= 0) ? areaData.SettlementInfos[0].BlockId : areaData.StationBlockId), false, null, 0.2f, Ease.Unset);
		}

		// Token: 0x06006FAF RID: 28591 RVA: 0x0033B7EC File Offset: 0x003399EC
		public void ResetMapCamera(bool isAnim = false)
		{
			bool flag = this._mapModel.ShowingAreaId != this.PlayerAtBlock.AreaId;
			if (flag)
			{
				this.SetShowingArea(this.PlayerAtBlock.AreaId);
			}
			this.MoveCameraTo(new Location(this.PlayerAtBlock.AreaId, this.PlayerAtBlock.BlockId), isAnim, null, 0.2f, Ease.Unset);
		}

		// Token: 0x06006FB0 RID: 28592 RVA: 0x0033B858 File Offset: 0x00339A58
		public void RefreshTradeCaravanPath(int merchantId)
		{
			RectTransform pathRoot = this.caravanPathRoot;
			bool flag = pathRoot == null;
			if (!flag)
			{
				foreach (object obj in pathRoot)
				{
					Transform child = (Transform)obj;
					PoolManager.Destroy("ui_WorldmapInfo_TradeCaravanPathNodePrefab", child.gameObject);
				}
				this._showingPathCaravanId = merchantId;
				bool flag2 = this._showingPathCaravanId < 0;
				if (flag2)
				{
					this._caravanLineVertices.Clear();
					pathRoot.GetComponent<CImage>().SetVerticesDirty();
				}
				else
				{
					CaravanPath path = this._mapModel.CaravanData.First((CaravanDisplayData data) => data.CaravanId == merchantId).PathInArea;
					int nodeIndex = 0;
					while (nodeIndex < path.MoveNodes.Count && path.MoveNodes[nodeIndex] <= 0)
					{
						nodeIndex++;
					}
					bool flag3 = path.MoveNodes[0] >= 0;
					Location lastLocation;
					if (flag3)
					{
						this._caravanLineVertices.Clear();
						int startIndex = path.MoveNodes[0];
						lastLocation = path.FullPath[startIndex];
						Vector2 srcPos = MapRenderSystem.GetBlockLocalPos(lastLocation) - this.BlockUICenterOffset;
						this._caravanLineVertices.Add(srcPos);
						for (int i = startIndex + 1; i < path.FullPath.Count; i++)
						{
							Location location = path.FullPath[i];
							Vector2 destPos = MapRenderSystem.GetBlockLocalPos(location) - this.BlockUICenterOffset;
							this._caravanLineVertices.Add(destPos);
						}
					}
					pathRoot.GetComponent<CImage>().SetVerticesDirty();
					lastLocation = default(Location);
					for (int j = nodeIndex; j < path.MoveNodes.Count; j++)
					{
						int index = path.MoveNodes[j];
						bool flag4 = index < 0;
						if (flag4)
						{
							break;
						}
						Location location2 = path.FullPath[index];
						bool flag5 = location2.Equals(lastLocation);
						if (!flag5)
						{
							RectTransform node = PoolManager.GetObject<RectTransform>("ui_WorldmapInfo_TradeCaravanPathNodePrefab");
							node.name = string.Format("node-{0}", location2.BlockId);
							node.SetParent(pathRoot, false);
							node.localScale = Vector3.one;
							node.localPosition = MapRenderSystem.GetBlockLocalPos(location2) - this.BlockUICenterOffset;
							node.gameObject.SetActive(true);
							lastLocation = location2;
						}
					}
				}
			}
		}

		// Token: 0x06006FB1 RID: 28593 RVA: 0x0033BB20 File Offset: 0x00339D20
		public void ShowPath()
		{
			bool flag = this.SelectedBlock == null;
			if (!flag)
			{
				this.RefreshPath(this.SelectedBlock);
			}
		}

		// Token: 0x06006FB2 RID: 28594 RVA: 0x0033BB4A File Offset: 0x00339D4A
		public void HidePath()
		{
			this.RefreshPath(this.PlayerAtBlock);
		}

		// Token: 0x06006FB3 RID: 28595 RVA: 0x0033BB5A File Offset: 0x00339D5A
		public static void SetDisableMoving(bool isDisableMoving)
		{
			ViewWorldMap._disableMoving = isDisableMoving;
		}

		// Token: 0x06006FB4 RID: 28596 RVA: 0x0033BB63 File Offset: 0x00339D63
		public void StopMove()
		{
			this._pathMoving = false;
			this._ignoreClick = true;
			this.MapClickReceiver.StopDrag();
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.2f, delegate
			{
				this._ignoreClick = false;
			});
		}

		// Token: 0x06006FB5 RID: 28597 RVA: 0x0033BB9C File Offset: 0x00339D9C
		public void RefreshAllBlockVisibility()
		{
			WorldMapModel.MapBlockLoadFinish = true;
			this.OnMapScaled(this.MapClickReceiver.CurrentScale, false, true);
			this.RefreshMapPickups();
		}

		// Token: 0x06006FB6 RID: 28598 RVA: 0x0033BBC0 File Offset: 0x00339DC0
		public void SetPlayerLocationWithRefreshAllBlockVisibility(Location location, Location location2 = default(Location), float lerpValue = -1f, bool refreshAllBlockVisibility = true)
		{
			Vector2 pos = MapRenderSystem.GetBlockLocalPos(location);
			Vector2 playerPos = (lerpValue >= 0f) ? Vector2.Lerp(pos, MapRenderSystem.GetBlockLocalPos(location2), lerpValue) : pos;
			bool flag = lerpValue >= 0f;
			if (flag)
			{
				bool flag2 = this._indicateLineValidVertices.Count > 0;
				if (flag2)
				{
					this._indicateLineValidVertices[0] = playerPos - this.BlockUICenterOffset;
				}
				this.paths.SetVerticesDirty();
			}
			this._player.GetComponent<RectTransform>().anchoredPosition = playerPos;
			this.MoveCameraTo(playerPos, false, null, 0.2f, Ease.Unset);
			bool flag3 = (lerpValue < 0f || Mathf.Approximately(lerpValue, 1f)) && refreshAllBlockVisibility;
			if (flag3)
			{
				this.RefreshAllBlockVisibility();
			}
		}

		// Token: 0x06006FB7 RID: 28599 RVA: 0x0033BC80 File Offset: 0x00339E80
		public IEnumerator QuickTravel(short targetAreaId, Action<int> onProgressChanged = null)
		{
			if (onProgressChanged != null)
			{
				onProgressChanged(0);
			}
			MapDomainMethod.Call.QuickTravel(targetAreaId);
			yield return new WaitUntil(() => WorldMapModel.MapBlockLoadFinish && WorldMapModel.MapBlockUiLoadFinish && WorldMapModel.MapBlockRenderFinish);
			this.RefreshAllBlockVisibility();
			yield return new WaitForEndOfFrame();
			if (onProgressChanged != null)
			{
				onProgressChanged(100);
			}
			yield break;
		}

		// Token: 0x06006FB8 RID: 28600 RVA: 0x0033BC9D File Offset: 0x00339E9D
		public IEnumerator ShowSwordTombAppear(List<Location> locationList)
		{
			bool flag = locationList == null || locationList.Count <= 0;
			if (flag)
			{
				yield break;
			}
			UIElement.BlockInteract.Show();
			yield return DOVirtual.Float(1f, 0.25f, 1.5f, new TweenCallback<float>(this.ScaleView)).WaitForCompletion();
			yield return base.StartCoroutine(this.CoShakeMap());
			WaitForSeconds moveWait = new WaitForSeconds(0.75f);
			WaitForSeconds appearWait = new WaitForSeconds(1f);
			WaitForSeconds changeWait = new WaitForSeconds(1f);
			int[] xiangShuAvatarOrderAdjust = new int[]
			{
				1,
				2,
				3,
				4,
				5,
				6,
				7,
				0
			};
			int num;
			for (int i = 0; i < xiangShuAvatarOrderAdjust.Length; i = num)
			{
				Location cellLocation = locationList[i];
				this.MoveCameraTo(cellLocation, true, null, 0.75f, Ease.Unset);
				yield return moveWait;
				sbyte xiangShuAvatarId = SingletonObject.getInstance<BasicGameData>().GetXiangshuAvatarTaskInOrderIndex(xiangShuAvatarOrderAdjust[i]);
				short blockTemplateId;
				bool flag2 = GameData.Domains.Map.SharedConstValue.XiangshuId2BlockConfigId.TryGetValue(xiangShuAvatarId, out blockTemplateId);
				if (flag2)
				{
					AudioManager.Instance.PlaySound("ui_swordtomb", false, false);
					yield return 0.1f;
					MapDomainMethod.Call.SetBlockAndViewRangeVisibleByBlockTemplateId(cellLocation, blockTemplateId);
					AudioManager.Instance.PlaySound("Ui_JianZhong_RedLight", false, false);
					this.swordTombAppearContainer.Append(cellLocation);
					yield return changeWait;
					MapDomainMethod.Call.ChangeBlockTemplate(this.Element.GameDataListenerId, cellLocation, blockTemplateId, false);
					yield return appearWait;
				}
				cellLocation = default(Location);
				num = i + 1;
			}
			this.MoveCameraTo(locationList[locationList.Count - 1], true, null, 0.75f, Ease.Unset);
			yield return moveWait;
			TaiwuEventDomainMethod.Call.TriggerListener("MainStorySwordTombAppearComplete", true);
			this.swordTombAppearContainer.Clear();
			UIElement.BlockInteract.Hide(false);
			yield break;
		}

		// Token: 0x06006FB9 RID: 28601 RVA: 0x0033BCB3 File Offset: 0x00339EB3
		private void DefeatSwordTomb(ArgumentBox argumentBox)
		{
			base.StartCoroutine(this.DefeatSwordTomb());
		}

		// Token: 0x06006FBA RID: 28602 RVA: 0x0033BCC3 File Offset: 0x00339EC3
		private IEnumerator DefeatSwordTomb()
		{
			UIElement.BlockInteract.Show();
			MapBlockData rootMapBlock = SingletonObject.getInstance<WorldMapModel>().RemoveSwordTombRootMapBlock;
			MapDomainMethod.Call.RemoveSwordTombFromLocation(rootMapBlock.GetLocation());
			short blockTemplateId = rootMapBlock.GetConfig().TemplateId;
			sbyte xiangshuAvatarId = GameData.Domains.Map.SharedConstValue.SwordTombId2XiangshuId[blockTemplateId];
			AudioManager.Instance.PlaySound("Ui_JianZhong_Disappear", false, false);
			this.swordTombDisappearContainer.Append(rootMapBlock.GetLocation(), (int)xiangshuAvatarId);
			yield return new WaitForSeconds(1f);
			yield return new WaitForSeconds(1f);
			UIElement.BlockInteract.Hide(false);
			this.swordTombDisappearContainer.Clear();
			SingletonObject.getInstance<WorldMapModel>().RemoveSwordTombRootMapBlock = null;
			yield break;
		}

		// Token: 0x06006FBB RID: 28603 RVA: 0x0033BCD2 File Offset: 0x00339ED2
		public IEnumerator ShowUnlockTaiwuPostLocation(Location location)
		{
			UIElement.BlockInteract.Show();
			this.taiwuVillageStationContainer.Append(location);
			yield return new WaitForSeconds(1f);
			MapDomainMethod.Call.ChangeBlockTemplate(this.Element.GameDataListenerId, location, 37, true);
			yield return new WaitForSeconds(0.5f);
			TaiwuEventDomainMethod.Call.TriggerListener("UnlockTaiwuStationComplete", true);
			this.taiwuVillageStationContainer.Clear();
			UIElement.BlockInteract.Hide(false);
			yield break;
		}

		// Token: 0x06006FBC RID: 28604 RVA: 0x0033BCE8 File Offset: 0x00339EE8
		public void MoveCameraTo(Location location, bool doTween = false, TweenCallback onComplete = null, float tweenTime = 0.2f, Ease ease = Ease.Unset)
		{
			this.MoveCameraTo(MapRenderSystem.GetBlockLocalPos(location), doTween, onComplete, tweenTime, ease);
		}

		// Token: 0x06006FBD RID: 28605 RVA: 0x0033BD00 File Offset: 0x00339F00
		public void ScaleView(float scale)
		{
			this.moveRoot.localScale = scale * Vector3.one;
			this.MapClickReceiver.SetCurrentScale(scale);
			this.MoveCameraTo(this._lastCameraMoveTarget, false, null, 0.2f, Ease.Unset);
			this.OnMapScaled(scale, false, true);
		}

		// Token: 0x06006FBE RID: 28606 RVA: 0x0033BD54 File Offset: 0x00339F54
		private void OnDoWorldMapScaleTween(ArgumentBox argBox)
		{
			this._scaleTween = ViewWorldMap.ResetScaleTween(this._scaleTween);
			EWorldmapScaleTweenType tweenType;
			argBox.Get<EWorldmapScaleTweenType>("tweenType", out tweenType);
			bool flag = tweenType == EWorldmapScaleTweenType.IntervalPingPong;
			if (flag)
			{
				this.SetPingPongScale(argBox);
			}
			else
			{
				bool flag2 = tweenType == EWorldmapScaleTweenType.SimpleFloat;
				if (flag2)
				{
					this.SetSimpleFloatScale(argBox);
				}
				else
				{
					bool flag3 = tweenType == EWorldmapScaleTweenType.RecordScale;
					if (flag3)
					{
						this._recordedScale = this.moveRoot.localScale.x;
					}
					else
					{
						bool flag4 = tweenType == EWorldmapScaleTweenType.RestoreScale;
						if (flag4)
						{
							this.SetRestoreScale(argBox);
						}
					}
				}
			}
			this._scaleTween.Play<Sequence>();
		}

		// Token: 0x06006FBF RID: 28607 RVA: 0x0033BDE4 File Offset: 0x00339FE4
		public static Sequence ResetScaleTween(Sequence sequence)
		{
			bool flag = sequence != null && sequence.IsActive() && !sequence.IsComplete();
			if (flag)
			{
				sequence.Kill(true);
			}
			sequence = DOTween.Sequence();
			sequence.Pause<Sequence>();
			return sequence;
		}

		// Token: 0x06006FC0 RID: 28608 RVA: 0x0033BE28 File Offset: 0x0033A028
		private void SetPingPongScale(ArgumentBox argBox)
		{
			float targetScale;
			argBox.Get("targetScale", out targetScale);
			float duration;
			argBox.Get("duration", out duration);
			Ease easeMode;
			argBox.Get<Ease>("easeMode", out easeMode);
			float interval;
			argBox.Get("interval", out interval);
			float originScale = this.moveRoot.localScale.x;
			Tweener inTween = DOVirtual.Float(originScale, targetScale, duration, new TweenCallback<float>(this.ScaleView)).SetEase(easeMode);
			this._scaleTween.AppendInterval(interval);
			this._scaleTween.Join(inTween);
			Tweener outTween = DOVirtual.Float(targetScale, originScale, duration, new TweenCallback<float>(this.ScaleView)).SetEase(easeMode);
			this._scaleTween.Append(outTween);
			this._scaleTween.OnComplete(delegate
			{
				this.ScaleView(originScale);
			});
		}

		// Token: 0x06006FC1 RID: 28609 RVA: 0x0033BF18 File Offset: 0x0033A118
		private void SetSimpleFloatScale(ArgumentBox argBox)
		{
			float targetScale;
			argBox.Get("targetScale", out targetScale);
			float duration;
			argBox.Get("duration", out duration);
			Ease easeMode;
			argBox.Get<Ease>("easeMode", out easeMode);
			float originScale = this.moveRoot.localScale.x;
			Tweener inTween = DOVirtual.Float(originScale, targetScale, duration, new TweenCallback<float>(this.ScaleView)).SetEase(easeMode);
			this._scaleTween.Join(inTween);
			this._scaleTween.OnComplete(delegate
			{
				this.ScaleView(targetScale);
			});
		}

		// Token: 0x06006FC2 RID: 28610 RVA: 0x0033BFBC File Offset: 0x0033A1BC
		private void SetRestoreScale(ArgumentBox argBox)
		{
			float duration;
			argBox.Get("duration", out duration);
			Ease easeMode;
			argBox.Get<Ease>("easeMode", out easeMode);
			float originScale = this.moveRoot.localScale.x;
			Tweener inTween = DOVirtual.Float(originScale, this._recordedScale, duration, new TweenCallback<float>(this.ScaleView)).SetEase(easeMode);
			this._scaleTween.Join(inTween);
			this._scaleTween.OnComplete(delegate
			{
				this.ScaleView(this._recordedScale);
			});
		}

		// Token: 0x06006FC3 RID: 28611 RVA: 0x0033C03C File Offset: 0x0033A23C
		public IEnumerator RefreshMapUi(bool waitingForBlocksReady = false)
		{
			WorldMapModel.MapBlockUiLoadFinish = false;
			this.ResetMapUi();
			this._mapModel.UpdateBgm();
			yield return new WaitForEndOfFrame();
			Location playerLocation = new Location(this.PlayerAtBlock.AreaId, this.PlayerAtBlock.BlockId);
			List<MapBlockData> areaBlocks = EasyPool.Get<List<MapBlockData>>();
			this._mapModel.GetAreaBlocks(this._mapModel.CurrentAreaId, areaBlocks);
			this.CollectAllElements();
			short blockId = 0;
			while ((int)blockId < areaBlocks.Count)
			{
				MapBlockData blockData = areaBlocks[(int)blockId];
				bool flag = blockData.TemplateId != 126;
				if (flag)
				{
					while (!this._mapBlockSet.Contains(new Location(this.PlayerAtBlock.AreaId, blockId)))
					{
						yield return null;
					}
					this.RefreshMapBlockInfo(blockData);
				}
				blockData = null;
				short num = blockId;
				blockId = num + 1;
			}
			EasyPool.Free<List<MapBlockData>>(areaBlocks);
			this._player.gameObject.SetActive(true);
			this._select.gameObject.SetActive(true);
			this._selectVirtual.gameObject.SetActive(true);
			this.ResetSelectedBlock();
			this.SetPlayerLocationWithRefreshAllBlockVisibility(playerLocation, default(Location), -1f, true);
			WorldMapModel.MapBlockUiLoadFinish = true;
			if (waitingForBlocksReady)
			{
				while (!WorldMapModel.MapBlockLoadFinish)
				{
					yield return null;
				}
				this._mapModel.OnMapInitFinish();
			}
			else
			{
				bool mapBlockLoadFinish = WorldMapModel.MapBlockLoadFinish;
				if (mapBlockLoadFinish)
				{
					this._mapModel.OnMapInitFinish();
				}
			}
			yield break;
		}

		// Token: 0x06006FC4 RID: 28612 RVA: 0x0033C054 File Offset: 0x0033A254
		public void OnBlockClick(MapBlockData block)
		{
			bool flag = (!this.IsDivineFlameActive && !this._inviteSelectBlockActive && !this._professionTravelerSkillTwoActive && !block.Visible) || this._hotkeyMoveDirection != MoveDirection.None || this._ignoreClick || this._doingMove || UIManager.Instance.BlockHotKey;
			if (!flag)
			{
				bool flag2 = block.AreaId != this._mapModel.CurrentAreaId;
				if (!flag2)
				{
					bool inviteSelectBlockActive = this._inviteSelectBlockActive;
					if (inviteSelectBlockActive)
					{
						bool flag3 = !this._inviteSelectBlockLocations.Contains(block.GetLocation());
						if (flag3)
						{
							return;
						}
						this._inviteBlockLocation = block.GetLocation();
						this.SelectCharacterPageForInvite();
					}
					List<Location> divineFlameLocationList = this._divineFlameLocationList;
					bool contains = divineFlameLocationList != null && divineFlameLocationList.Contains(block.GetLocation());
					bool flag4 = this.IsDivineFlameActive && contains;
					if (flag4)
					{
						bool isGood = SingletonObject.getInstance<BasicGameData>().IsXiangshuAvatarTaskStatusGood((int)this._xiangshuAvatarId);
						string title = ViewCharacterMenuItems.GetDivineFlameTitle(this._xiangshuAvatarId, isGood);
						string content = ViewCharacterMenuItems.GetDivineFlameSelectTargetTip(this._xiangshuAvatarId, isGood, true);
						CommonUtils.ShowConfirmDialog(title, content, delegate
						{
							StoryDomainMethod.Call.UseDivineFlame(this._xiangshuAvatarId, -1, block.GetLocation());
							bool flag11 = !isGood;
							if (flag11)
							{
								this.divineFlameJiuhanBadContainer.Append(block.GetLocation());
							}
							GEvent.OnEvent(UiEvents.DivineFlameSelectBlockStop, null);
						}, null, EDialogType.None);
					}
					bool professionTravelerSkillTwoActive = this._professionTravelerSkillTwoActive;
					if (professionTravelerSkillTwoActive)
					{
						bool flag5 = !this._professionTravelerSkillTwoCanSelectLocations.Contains(block.GetLocation());
						if (!flag5)
						{
							ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
							argumentBox.Clear();
							ProfessionSkillArg professionSkillArg = new ProfessionSkillArg
							{
								ProfessionId = 11,
								SkillId = 46,
								IsSuccess = true,
								ProfessionTravelerTargetLocation = block.GetLocation()
							};
							argumentBox.SetObject("ProfessionSkillArg", professionSkillArg);
							argumentBox.SetObject("OnConfirm", new Action(this.ProfessionTravelerSkillTwoCancel));
							UIElement.ProfessionSkillConfirm.SetOnInitArgs(argumentBox);
							UIManager.Instance.MaskUI(UIElement.ProfessionSkillConfirm);
							UIManager.Instance.SetEscHandler(null);
						}
					}
					else
					{
						this.RefreshTradeCaravanPath(-1);
						bool flag6 = GMFunc.TeleportMove && block != this.PlayerAtBlock && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));
						if (flag6)
						{
							Vector2 uiPos = MapRenderSystem.GetBlockLocalPos(new Location(block.AreaId, block.BlockId));
							this._teleportMoving = true;
							this._player.GetComponent<RectTransform>().anchoredPosition = uiPos;
							this.MoveCameraTo(uiPos, false, null, 0.2f, Ease.Unset);
							MapDomainMethod.Call.Move(block.BlockId);
							this.SelectBlock(block);
							this.ClearMovePath();
						}
						else
						{
							AudioManager.Instance.PlaySound("ui_default_click_left", false, false);
							bool flag7 = block == this.SelectedBlock;
							if (flag7)
							{
								bool flag8 = (block.AreaId != this._mapModel.CurrentAreaId || block.BlockId != this._mapModel.CurrentBlockId) && this.CheckGuidBlockMove();
								if (!flag8)
								{
									bool flag9 = !this._pathMoving && this._mapModel.MovePath.Count > 0 && !ViewWorldMap._disableMoving && !SingletonObject.getInstance<EventModel>().LockInputByEvent;
									if (flag9)
									{
										this._pathMoving = true;
										this._movingByController = true;
										this.MoveToNext();
									}
									bool flag10 = block != this.PlayerAtBlock && this._mapModel.MovePath.Count <= 0;
									if (flag10)
									{
										this.FindWay(block.GetLocation());
									}
								}
							}
							else
							{
								this.SelectBlock(block);
								this.RefreshPath(block);
							}
						}
					}
				}
			}
		}

		// Token: 0x06006FC5 RID: 28613 RVA: 0x0033C468 File Offset: 0x0033A668
		private void SelectCharacterPageForInvite()
		{
			CharacterSortFilterSettings filterSetting = new CharacterSortFilterSettings
			{
				FilterType = 5,
				FilterSubType = 3
			};
			filterSetting.FilterSubId = -1;
			filterSetting.SortOrder.Clear();
			CharacterDomainMethod.AsyncCall.InitializeCharacterSortFilter(this, filterSetting, delegate(int offset, RawDataPool dataPool)
			{
				CharacterList charIds;
				Serializer.Deserialize(dataPool, offset, ref charIds);
				CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForGeneralScrollListBatch(this, charIds.GetCollection(), delegate(int offset, RawDataPool dataPool)
				{
					List<CharacterDisplayDataForGeneralScrollList> dataList = new List<CharacterDisplayDataForGeneralScrollList>();
					Serializer.Deserialize(dataPool, offset, ref dataList);
					this.OpenSelectCharacterUIForInvite(dataList);
				});
			});
		}

		// Token: 0x06006FC6 RID: 28614 RVA: 0x0033C4B4 File Offset: 0x0033A6B4
		private void OpenSelectCharacterUIForInvite(List<CharacterDisplayDataForGeneralScrollList> dataList)
		{
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
			CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.None);
			config.InteractionMode = ESelectCharacterInteractionMode.Instant;
			config.SelectionMode = ESelectCharacterSelectionMode.Single;
			config.Title = LocalStringManager.Get(LanguageKey.LK_Item_Invite_Title);
			UIElement.SelectChar.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("SelectCharacterConfig", config).SetObject("SelectCharacterDataList", dataList).SetObject("SelectCharacterCallback", new SelectCharacterCallback(delegate(List<int> selectedIds)
			{
				bool flag = selectedIds != null && selectedIds.Count > 0;
				if (flag)
				{
					Action<int> onConfirmInviteCharacter = this._onConfirmInviteCharacter;
					if (onConfirmInviteCharacter != null)
					{
						onConfirmInviteCharacter(selectedIds[0]);
					}
				}
				else
				{
					Action onCancelInviteCharacter = this._onCancelInviteCharacter;
					if (onCancelInviteCharacter != null)
					{
						onCancelInviteCharacter();
					}
				}
			})));
			UIManager.Instance.MaskUI(UIElement.SelectChar);
		}

		// Token: 0x06006FC7 RID: 28615 RVA: 0x0033C53C File Offset: 0x0033A73C
		public void ResetSelectedBlock()
		{
			this.SelectBlock(this.PlayerAtBlock);
			this.ClearMovePath();
		}

		// Token: 0x06006FC8 RID: 28616 RVA: 0x0033C554 File Offset: 0x0033A754
		private void ReshowBlockList(bool isPlayerAtBlock = true)
		{
			bool flag = this.MapClickReceiver != null;
			if (flag)
			{
				this.OnMapScaled(this.MapClickReceiver.CurrentScale, false, true);
			}
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
			{
				bool flag2 = this == null;
				if (flag2)
				{
					PredefinedLog.Show(11, "ReshowBlockList empty");
				}
				else
				{
					bool flag3 = !this.gameObject.activeSelf;
					if (!flag3)
					{
						GEvent.OnEvent(UiEvents.WorldMapShowInfoBlockChange, new ArgumentBox().SetObject("block", (isPlayerAtBlock || this.SelectedBlock == null) ? this.PlayerAtBlock : this.SelectedBlock));
					}
				}
			});
		}

		// Token: 0x06006FC9 RID: 28617 RVA: 0x0033C5B4 File Offset: 0x0033A7B4
		public void ResetMapCamera(Ease ease, bool isAnim = false)
		{
			bool flag = this._mapModel.ShowingAreaId != this.PlayerAtBlock.AreaId;
			if (flag)
			{
				this.SetShowingArea(this.PlayerAtBlock.AreaId);
			}
			this.MoveCameraTo(new Location(this.PlayerAtBlock.AreaId, this.PlayerAtBlock.BlockId), isAnim, null, 1f, ease);
		}

		// Token: 0x06006FCA RID: 28618 RVA: 0x0033C620 File Offset: 0x0033A820
		private static float GetCricketCatchTransitionTargetScale(float originScale)
		{
			return Mathf.Min(Mathf.Max(originScale + 0.1f, 0.5f), 1.1f);
		}

		// Token: 0x06006FCB RID: 28619 RVA: 0x0033C650 File Offset: 0x0033A850
		private void PlayAdventureOpenPartOneTransition(float startScale, float targetScale, TweenCallback onComplete)
		{
			this._adventureScaleSequence = ViewWorldMap.ResetScaleTween(this._adventureScaleSequence);
			this._blendMaskSequence = ViewWorldMap.ResetScaleTween(this._blendMaskSequence);
			Tweener scaleTween = DOVirtual.Float(startScale, targetScale, 0.25f, new TweenCallback<float>(this.ScaleView)).SetEase(Ease.Linear);
			this._adventureScaleSequence.Append(scaleTween);
			CImage blendImage = this.adventureBlend;
			blendImage.SetAlpha(0f);
			Tweener blendTweenPart = DOVirtual.Float(0f, 0.3f, 0.05f, delegate(float alpha)
			{
				blendImage.SetAlpha(alpha);
			}).SetEase(Ease.Linear);
			this._blendMaskSequence.Append(blendTweenPart);
			CImage mask = this.adventureMask;
			mask.SetAlpha(0f);
			mask.gameObject.SetActive(true);
			Tweener maskTweenPart = DOVirtual.Float(0f, 1f, 0.15f, delegate(float alpha)
			{
				mask.SetAlpha(alpha);
			}).SetEase(Ease.Linear);
			this._blendMaskSequence.Append(maskTweenPart);
			maskTweenPart.OnComplete(onComplete);
			this._blendMaskSequence.Play<Sequence>();
			this._adventureScaleSequence.Play<Sequence>();
		}

		// Token: 0x06006FCC RID: 28620 RVA: 0x0033C788 File Offset: 0x0033A988
		private void RestoreAdventurePartOneTransitionImmediately(float restoreScale)
		{
			this._adventureScaleSequence = ViewWorldMap.ResetScaleTween(this._adventureScaleSequence);
			this._blendMaskSequence = ViewWorldMap.ResetScaleTween(this._blendMaskSequence);
			this.ScaleView(restoreScale);
			CImage mask = this.adventureMask;
			CImage blendImage = this.adventureBlend;
			mask.SetAlpha(0f);
			mask.gameObject.SetActive(false);
			blendImage.SetAlpha(0f);
		}

		// Token: 0x06006FCD RID: 28621 RVA: 0x0033C7F4 File Offset: 0x0033A9F4
		private void AdventureRemakeIconClick(ArgumentBox argBox)
		{
			AudioManager.Instance.PlaySound("ui_adventure_uiwhoosh", false, false);
			int adventureId;
			bool isAdventure = argBox.Get("AdventureId", out adventureId);
			int majorEventId;
			bool isMajorEvent = argBox.Get("MajorEventId", out majorEventId);
			this._scaleTween = ViewWorldMap.ResetScaleTween(this._scaleTween);
			float originScale = this.moveRoot.localScale.x;
			float duration = Mathf.Abs(originScale - 0.4f) * 1f;
			Tweener inTween = DOVirtual.Float(originScale, 0.4f, duration, new TweenCallback<float>(this.ScaleView)).SetEase(Ease.OutQuad);
			this._scaleTween.Join(inTween);
			this._scaleTween.OnComplete(delegate
			{
				this.ScaleView(0.4f);
				ArgumentBox box = EasyPool.Get<ArgumentBox>();
				bool isAdventure = isAdventure;
				if (isAdventure)
				{
					box.Set("AdventureId", adventureId);
				}
				bool isMajorEvent = isMajorEvent;
				if (isMajorEvent)
				{
					box.Set("MajorEventId", majorEventId);
				}
				UIElement.AdventurePrepareRemake.SetOnInitArgs(box.Set("EnterAreaId", this._mapModel.CurrentLocation.AreaId).Set("EnterBlockId", this._mapModel.CurrentLocation.BlockId));
				UIManager.Instance.MaskUI(UIElement.AdventurePrepareRemake);
				CommandKitBase.SetDisable(false);
			});
			CommandKitBase.SetDisable(true);
			this._scaleTween.Play<Sequence>();
		}

		// Token: 0x06006FCE RID: 28622 RVA: 0x0033C8DC File Offset: 0x0033AADC
		private void AdventureRemakeOpenPartOne(ArgumentBox argBox)
		{
			int adventureId;
			bool isAdventure = argBox.Get("AdventureId", out adventureId);
			int majorEventId;
			bool isMajorEvent = argBox.Get("MajorEventId", out majorEventId);
			this.PlayAdventureOpenPartOneTransition(0.4f, 0.5f, delegate
			{
				ArgumentBox box = EasyPool.Get<ArgumentBox>();
				box.Set("AdventureId", adventureId);
				box.Set("MajorEventId", majorEventId);
				bool exist = UIElement.TextureShow.Exist;
				if (exist)
				{
					UIManager.Instance.HideUI(UIElement.TextureShow);
				}
				bool isAdventure = isAdventure;
				if (isAdventure)
				{
					UIElement.AdventureRemake.SetOnInitArgs(box);
					UIManager.Instance.StackToUI(SingletonObject.getInstance<AdventureRemakeModel>().SpecialBottomAdventure(adventureId) ? UIElement.StateAdventureRemakeSpecialBottom : UIElement.StateAdventureRemake);
				}
				else
				{
					UIElement.AdventureMajorEvent.SetOnInitArgs(box);
					UIManager.Instance.StackToUI(UIElement.StateMajorEvent);
				}
			});
		}

		// Token: 0x06006FCF RID: 28623 RVA: 0x0033C938 File Offset: 0x0033AB38
		private void AdventureRemakeOpenPartTwo(ArgumentBox argBox)
		{
			this._blendMaskSequence = ViewWorldMap.ResetScaleTween(this._blendMaskSequence);
			CImage mask = this.adventureMask;
			CImage blendImage = this.adventureBlend;
			mask.gameObject.SetActive(false);
			blendImage.SetAlpha(0f);
		}

		// Token: 0x06006FD0 RID: 28624 RVA: 0x0033C980 File Offset: 0x0033AB80
		private void AdventureExitAnim(ArgumentBox argBox)
		{
			this._adventureScaleSequence = ViewWorldMap.ResetScaleTween(this._adventureScaleSequence);
			this._blendMaskSequence = ViewWorldMap.ResetScaleTween(this._blendMaskSequence);
			float duration = 0.25f;
			Tweener scaleTween = DOVirtual.Float(0.5f, 0.4f, duration * 2f, new TweenCallback<float>(this.ScaleView)).SetEase(Ease.Linear);
			this._adventureScaleSequence.Append(scaleTween);
			this._adventureScaleSequence.Play<Sequence>();
		}

		// Token: 0x06006FD8 RID: 28632 RVA: 0x0033CC20 File Offset: 0x0033AE20
		[CompilerGenerated]
		private void <OnEnable>g__FailedToHandleMapBlock|151_0()
		{
			TooltipInvoker mouseTips = this.MapClickReceiver.TipDisplayer;
			mouseTips.enabled = false;
			mouseTips.HideTips();
			this._selectVirtual.gameObject.SetActive(false);
		}

		// Token: 0x06006FDC RID: 28636 RVA: 0x0033CD34 File Offset: 0x0033AF34
		[CompilerGenerated]
		private bool <OnEnable>g__CheckNeedShowTip|151_4(int x, int y)
		{
			WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
			bool crossArchiveLockMoveTime = worldMapModel.CrossArchiveLockMoveTime;
			bool result;
			if (crossArchiveLockMoveTime)
			{
				result = false;
			}
			else
			{
				this.MapClickReceiver.ScaleListening = true;
				MapBlockData blockData = this.FindBlockByLogicalPosition(x, y);
				bool flag = blockData != null && blockData.AreaId == this._mapModel.CurrentAreaId;
				if (flag)
				{
					MapBlockItem blockConfig = blockData.GetConfig();
					bool flag2 = (!blockConfig.ShowTips && !blockData.Destroyed) || !blockData.Visible;
					result = !flag2;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06006FE4 RID: 28644 RVA: 0x0033D278 File Offset: 0x0033B478
		[CompilerGenerated]
		private IEnumerator <OnEnable>g__InputOffsetProcess|151_12()
		{
			WaitForSeconds wait = new WaitForSeconds(0.5f);
			while (base.isActiveAndEnabled)
			{
				this.ResetClickReceiverOffset();
				yield return wait;
			}
			this._inputOffsetCoroutine = null;
			yield break;
		}

		// Token: 0x06006FEA RID: 28650 RVA: 0x0033D410 File Offset: 0x0033B610
		[CompilerGenerated]
		private bool <HandleLifeSkillCombatStrategyUnlock>g__Check|206_1()
		{
			bool flag = GameApp.Instance.GetCurrentGameStateName() != EGameState.InGame;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = SingletonObject.getInstance<BasicGameData>().AdvancingMonthState != 0;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool elementIsMeet = UIManager.Instance.IsFocusElement(this.Element) || UIManager.Instance.IsFocusElement(UIElement.Reading);
					bool flag3 = !elementIsMeet;
					result = !flag3;
				}
			}
			return result;
		}

		// Token: 0x06006FF1 RID: 28657 RVA: 0x0033D5DC File Offset: 0x0033B7DC
		[CompilerGenerated]
		private void <AdvanceMonth>g__Action|289_1()
		{
			int leftDays = SingletonObject.getInstance<BasicGameData>().ActionPointCurrMonth / 10;
			bool flag = leftDays == 0;
			if (flag)
			{
				WorldDomainMethod.Call.AdvanceMonth();
				GameApp.AdvancingMonth = true;
			}
			else
			{
				this._waitAdvanceMonth = true;
				WorldDomainMethod.Call.AdvanceDaysInMonth(leftDays);
			}
		}

		// Token: 0x04005260 RID: 21088
		public static readonly float MoveStepTime = 0.4f;

		// Token: 0x04005261 RID: 21089
		public static readonly float MoveInterval = 0.1f;

		// Token: 0x04005262 RID: 21090
		private readonly string[] _mapBlockPrefabKey = new string[]
		{
			"map_block_size_1",
			"map_block_size_2",
			"map_block_size_3"
		};

		// Token: 0x04005263 RID: 21091
		private const string CaravanPathNodePrefabKey = "ui_WorldmapInfo_TradeCaravanPathNodePrefab";

		// Token: 0x04005264 RID: 21092
		private static readonly Dictionary<EMapAreaAreaDirection, string> AreaBackgrounds = new Dictionary<EMapAreaAreaDirection, string>
		{
			{
				EMapAreaAreaDirection.South,
				"map_bg_south"
			},
			{
				EMapAreaAreaDirection.North,
				"map_bg_north"
			},
			{
				EMapAreaAreaDirection.West,
				"map_bg_west"
			}
		};

		// Token: 0x04005265 RID: 21093
		private WorldMapModel _mapModel;

		// Token: 0x04005266 RID: 21094
		public MapRawImage MapRawImage;

		// Token: 0x04005267 RID: 21095
		public MapRawImage MapRawImageShadow;

		// Token: 0x04005268 RID: 21096
		public MapRawImage mapRawImageBaseShadow;

		// Token: 0x04005269 RID: 21097
		public MapRawImage mapRawImageBlockBaseAndBorder;

		// Token: 0x0400526A RID: 21098
		public MapRawImage mapRawImagePreparation;

		// Token: 0x0400526B RID: 21099
		public MapClickReceiver MapClickReceiver;

		// Token: 0x0400526C RID: 21100
		public PointerTrigger BackGroundPointerTrigger;

		// Token: 0x0400526D RID: 21101
		[SerializeField]
		private Vector2 BlockUICenterOffset = new Vector2(0f, 120f);

		// Token: 0x0400526E RID: 21102
		public WorldMapAdditionalEffectsInfo AdditionalBlockEffects;

		// Token: 0x0400526F RID: 21103
		public AnimationCurve BlockAppearAnimationCurve;

		// Token: 0x04005270 RID: 21104
		public SectStoryEffectContainer BloodLightContainer;

		// Token: 0x04005271 RID: 21105
		public SectStoryEffectContainer FairylandContainer;

		// Token: 0x04005272 RID: 21106
		public SectStoryEffectContainer LingbaoLightContainer;

		// Token: 0x04005273 RID: 21107
		public SectStoryEffectContainer LingbaoDarkContainer;

		// Token: 0x04005274 RID: 21108
		public SectStoryEffectContainer BloodContainer;

		// Token: 0x04005275 RID: 21109
		public MapBlockEightDirectionEffectContainer InFlameAreaContainer;

		// Token: 0x04005276 RID: 21110
		public MapBlockEightDirectionEffectContainer SettlementBlockConatiner;

		// Token: 0x04005277 RID: 21111
		public MapBlockCharacterImpactRangeEffectContainer CharacterImpactRangeContainer;

		// Token: 0x04005278 RID: 21112
		public SectStoryEffectContainer ProfessionTravelerStationExistContainer;

		// Token: 0x04005279 RID: 21113
		public SectStoryEffectContainer ProfessionTravelerStationUpContainer;

		// Token: 0x0400527A RID: 21114
		public SectStoryEffectContainer ProfessionTravelerStationDownContainer;

		// Token: 0x0400527B RID: 21115
		public WorldMapProfessionTravelerSkillTwoInfo ProfessionTravelerSkillTwoContainer;

		// Token: 0x0400527C RID: 21116
		public SectStoryEffectContainer ProfessionTravelerSkillThreeMoveStartMaleContainer;

		// Token: 0x0400527D RID: 21117
		public SectStoryEffectContainer ProfessionTravelerSkillThreeMoveStartFemaleContainer;

		// Token: 0x0400527E RID: 21118
		public SectStoryEffectContainer ProfessionTravelerSkillThreeMoveEndMaleContainer;

		// Token: 0x0400527F RID: 21119
		public SectStoryEffectContainer ProfessionTravelerSkillThreeMoveEndFemaleContainer;

		// Token: 0x04005280 RID: 21120
		public SectStoryEffectContainer swordTombAppearContainer;

		// Token: 0x04005281 RID: 21121
		public WorldMapEffectContainer swordTombDisappearContainer;

		// Token: 0x04005282 RID: 21122
		public SectStoryEffectContainer taiwuVillageStationContainer;

		// Token: 0x04005283 RID: 21123
		public SectStoryEffectContainer divineFlameJiuhanBadContainer;

		// Token: 0x04005284 RID: 21124
		public static bool PointerOnAnyMapBlock;

		// Token: 0x04005285 RID: 21125
		[Header("SerializeField Private")]
		[Space(10f)]
		[SerializeField]
		private CRawImage background;

		// Token: 0x04005286 RID: 21126
		[SerializeField]
		private GameObject infoPrefab;

		// Token: 0x04005287 RID: 21127
		[SerializeField]
		private RectTransform moveRoot;

		// Token: 0x04005288 RID: 21128
		[SerializeField]
		private CImage paths;

		// Token: 0x04005289 RID: 21129
		[SerializeField]
		private RectTransform cricketPlacePrefab;

		// Token: 0x0400528A RID: 21130
		[SerializeField]
		private RectTransform taiwuRoot;

		// Token: 0x0400528B RID: 21131
		[SerializeField]
		private RectTransform rectTsCatchCricket;

		// Token: 0x0400528C RID: 21132
		[SerializeField]
		private GameObject settlementBtnPrefab;

		// Token: 0x0400528D RID: 21133
		[SerializeField]
		private RectTransform settlementAndStationBtnRoot;

		// Token: 0x0400528E RID: 21134
		[SerializeField]
		private RectTransform caravanRoot;

		// Token: 0x0400528F RID: 21135
		[SerializeField]
		private RectTransform caravanPathRoot;

		// Token: 0x04005290 RID: 21136
		[SerializeField]
		private RectTransform caravanPathNodePrefab;

		// Token: 0x04005291 RID: 21137
		[SerializeField]
		private GameObject stationBtnPrefab;

		// Token: 0x04005292 RID: 21138
		[SerializeField]
		private RectTransform borderLineRoot;

		// Token: 0x04005293 RID: 21139
		[SerializeField]
		private RectTransform mapBlockRoot;

		// Token: 0x04005294 RID: 21140
		[SerializeField]
		private RectTransform shadowRoot;

		// Token: 0x04005295 RID: 21141
		[SerializeField]
		private RectTransform player;

		// Token: 0x04005296 RID: 21142
		[SerializeField]
		private RectTransform select;

		// Token: 0x04005297 RID: 21143
		[SerializeField]
		private RectTransform selectVirtual;

		// Token: 0x04005298 RID: 21144
		[SerializeField]
		private GameObject merchantPrefab;

		// Token: 0x04005299 RID: 21145
		[SerializeField]
		private GameObject characterPrefab;

		// Token: 0x0400529A RID: 21146
		[SerializeField]
		private RectTransform commonRoot;

		// Token: 0x0400529B RID: 21147
		[SerializeField]
		private GameObject costPrefab;

		// Token: 0x0400529C RID: 21148
		[SerializeField]
		private GameObject expectPromptPrefab;

		// Token: 0x0400529D RID: 21149
		[SerializeField]
		private GameObject temporaryMarkPrefab;

		// Token: 0x0400529E RID: 21150
		[SerializeField]
		private LineRenderer2D invalidPaths;

		// Token: 0x0400529F RID: 21151
		[SerializeField]
		private RectTransform mapEffectRoot;

		// Token: 0x040052A0 RID: 21152
		[SerializeField]
		private GameObject dreamBackPrefab;

		// Token: 0x040052A1 RID: 21153
		[SerializeField]
		private GameObject fulongFlamePrefab;

		// Token: 0x040052A2 RID: 21154
		[SerializeField]
		private GameObject emeiGuidancePrefab;

		// Token: 0x040052A3 RID: 21155
		[SerializeField]
		private GameObject zhujianThiefPrefab;

		// Token: 0x040052A4 RID: 21156
		[SerializeField]
		private GameObject mapElementPickup;

		// Token: 0x040052A5 RID: 21157
		[SerializeField]
		private GameObject mapElementPickupEffect;

		// Token: 0x040052A6 RID: 21158
		[SerializeField]
		private GameObject mapElementCricketWishEffect;

		// Token: 0x040052A7 RID: 21159
		[SerializeField]
		private MapElementAdventureRemake adventureRemakePrefab;

		// Token: 0x040052A8 RID: 21160
		[SerializeField]
		private CImage adventureBlend;

		// Token: 0x040052A9 RID: 21161
		[SerializeField]
		private CImage adventureMask;

		// Token: 0x040052AA RID: 21162
		private static bool _disableMoving;

		// Token: 0x040052AB RID: 21163
		private readonly HashSet<Location> _mapBlockSet = new HashSet<Location>();

		// Token: 0x040052AC RID: 21164
		private bool _sortingDirty;

		// Token: 0x040052AD RID: 21165
		private RectTransform _player;

		// Token: 0x040052AE RID: 21166
		private RectTransform _select;

		// Token: 0x040052AF RID: 21167
		private RectTransform _selectVirtual;

		// Token: 0x040052B0 RID: 21168
		private short _lastBlockId;

		// Token: 0x040052B1 RID: 21169
		private bool _playerAtBlockInitialized;

		// Token: 0x040052B2 RID: 21170
		private bool _ignoreClick;

		// Token: 0x040052B3 RID: 21171
		private short _wishingCricketId;

		// Token: 0x040052B4 RID: 21172
		private bool _professionTravelerSkillTwoActive;

		// Token: 0x040052B5 RID: 21173
		private bool _inviteSelectBlockActive;

		// Token: 0x040052B6 RID: 21174
		private sbyte _xiangshuAvatarId = -1;

		// Token: 0x040052B7 RID: 21175
		private List<Location> _divineFlameLocationList;

		// Token: 0x040052B8 RID: 21176
		private bool _waitAdvanceMonth;

		// Token: 0x040052B9 RID: 21177
		private bool _teleportMoving;

		// Token: 0x040052BA RID: 21178
		private IEnumerator _inputOffsetCoroutine;

		// Token: 0x040052BB RID: 21179
		private Action _continuousMovingCallback;

		// Token: 0x040052BC RID: 21180
		private Tweener _cameraMovingTweener;

		// Token: 0x040052BD RID: 21181
		private readonly Dictionary<Location, MapElementContainer> _activateMapElements = new Dictionary<Location, MapElementContainer>();

		// Token: 0x040052BE RID: 21182
		private readonly List<SectStoryEffectContainer> _sectStoryEffectContainers = new List<SectStoryEffectContainer>();

		// Token: 0x040052BF RID: 21183
		private int _showingPathCaravanId = -1;

		// Token: 0x040052C0 RID: 21184
		private MoveDirection _hotkeyMoveDirection;

		// Token: 0x040052C1 RID: 21185
		private bool _hotkeyMoveReady;

		// Token: 0x040052C2 RID: 21186
		private bool _pathMoving;

		// Token: 0x040052C3 RID: 21187
		private bool _doingMove;

		// Token: 0x040052C4 RID: 21188
		private readonly List<int> _plannedMoveCosts = new List<int>();

		// Token: 0x040052C5 RID: 21189
		private bool _hasConsumedFirstMove;

		// Token: 0x040052C6 RID: 21190
		private bool _needUpdatePath;

		// Token: 0x040052C7 RID: 21191
		private bool _movingByController;

		// Token: 0x040052C9 RID: 21193
		private readonly List<Vector2> _indicateLineValidVertices = new List<Vector2>();

		// Token: 0x040052CA RID: 21194
		private readonly List<Vector2> _indicateLineInvalidVertices = new List<Vector2>();

		// Token: 0x040052CB RID: 21195
		private readonly List<Vector2> _caravanLineVertices = new List<Vector2>();

		// Token: 0x040052CC RID: 21196
		private readonly Dictionary<int, Location> _lastCaravanLocationDict = new Dictionary<int, Location>();

		// Token: 0x040052CD RID: 21197
		private Dictionary<short, HashSet<short>> _areaSettlementBlockMap = new Dictionary<short, HashSet<short>>();

		// Token: 0x040052CE RID: 21198
		private short _displayingSettlementBlockId = -1;

		// Token: 0x040052CF RID: 21199
		private readonly List<Location> _lastHeavenTreeLocations = new List<Location>();

		// Token: 0x040052D0 RID: 21200
		private readonly List<Location> _lastAlterSettlementLocations = new List<Location>();

		// Token: 0x040052D1 RID: 21201
		private Vector2 _clickReceiverOriginPosition;

		// Token: 0x040052D2 RID: 21202
		private Vector2 _lastCameraMoveTarget;

		// Token: 0x040052D3 RID: 21203
		private Vector2 _lastShowTipsCoord;

		// Token: 0x040052D4 RID: 21204
		private Vector2 _currentCoord;

		// Token: 0x040052D5 RID: 21205
		private int _hoverBlockX = -1;

		// Token: 0x040052D6 RID: 21206
		private int _hoverBlockY = -1;

		// Token: 0x040052D7 RID: 21207
		private readonly List<Location> _professionTravelerSkillTwoCanSelectLocations = new List<Location>();

		// Token: 0x040052D8 RID: 21208
		private readonly List<Location> _inviteSelectBlockLocations = new List<Location>();

		// Token: 0x040052D9 RID: 21209
		private List<Line2DGenerator> _professionTravelerSkillTwoLineList = new List<Line2DGenerator>();

		// Token: 0x040052DA RID: 21210
		private Sequence _scaleTween;

		// Token: 0x040052DB RID: 21211
		private float _recordedScale;

		// Token: 0x040052DC RID: 21212
		private Action<int> _onConfirmInviteCharacter;

		// Token: 0x040052DD RID: 21213
		private Action _onCancelInviteCharacter;

		// Token: 0x040052DE RID: 21214
		private Action _onQuickHideBySystem;

		// Token: 0x040052DF RID: 21215
		private Action<int> _openCharacterMenu;

		// Token: 0x040052E0 RID: 21216
		private Predicate<CharacterDisplayDataForUltimateSelect> _disableCondition;

		// Token: 0x040052E1 RID: 21217
		private Location _inviteBlockLocation;

		// Token: 0x040052E2 RID: 21218
		private List<short> _unlockedDebateStrategyList = new List<short>();

		// Token: 0x040052E3 RID: 21219
		private readonly List<Location> _positions = new List<Location>();

		// Token: 0x040052E4 RID: 21220
		private const float ResetScale = 0.4f;

		// Token: 0x040052E5 RID: 21221
		public const float AdventureFirstScale = 0.5f;

		// Token: 0x040052E6 RID: 21222
		private const float BlendImagePartOneDuration = 0.05f;

		// Token: 0x040052E7 RID: 21223
		private const float MaskImagePartOneDuration = 0.15f;

		// Token: 0x040052E8 RID: 21224
		public const float MaskImagePartTwoDuration = 0.45f;

		// Token: 0x040052E9 RID: 21225
		public const float BlueBlendEndValue = 0.3f;

		// Token: 0x040052EA RID: 21226
		private Sequence _adventureScaleSequence;

		// Token: 0x040052EB RID: 21227
		private Sequence _blendMaskSequence;
	}
}
