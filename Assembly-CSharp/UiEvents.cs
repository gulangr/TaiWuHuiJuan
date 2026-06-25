using System;

// Token: 0x020000E3 RID: 227
public enum UiEvents
{
	// Token: 0x040008F7 RID: 2295
	Invalid = -1,
	// Token: 0x040008F8 RID: 2296
	OnUIBasePlaced,
	// Token: 0x040008F9 RID: 2297
	UnloadPackers,
	// Token: 0x040008FA RID: 2298
	TopUiChanged,
	// Token: 0x040008FB RID: 2299
	OnUIElementHide,
	// Token: 0x040008FC RID: 2300
	OnUIElementShow,
	// Token: 0x040008FD RID: 2301
	OnUIElementHideStart,
	// Token: 0x040008FE RID: 2302
	OnUIElementShowFinish,
	// Token: 0x040008FF RID: 2303
	OnLoadingElementHide,
	// Token: 0x04000900 RID: 2304
	WorldMapEnterNewArea,
	// Token: 0x04000901 RID: 2305
	WorldMapPlayerAreaChange,
	// Token: 0x04000902 RID: 2306
	WorldMapPlayerBlockChange,
	// Token: 0x04000903 RID: 2307
	WorldMapAreaDataInit,
	// Token: 0x04000904 RID: 2308
	WorldMapAreaDataChange,
	// Token: 0x04000905 RID: 2309
	WorldMapBlockDataChange,
	// Token: 0x04000906 RID: 2310
	WorldMapShowingAreaChange,
	// Token: 0x04000907 RID: 2311
	WorldMapShowInfoBlockChange,
	// Token: 0x04000908 RID: 2312
	WorldMapShowInfoStatusChange,
	// Token: 0x04000909 RID: 2313
	WorldMapVillagerWorkingLocationChange,
	// Token: 0x0400090A RID: 2314
	WorldMapInited,
	// Token: 0x0400090B RID: 2315
	WorldMapResetMapCamera,
	// Token: 0x0400090C RID: 2316
	WorldMapSetCameraToArea,
	// Token: 0x0400090D RID: 2317
	WorldMapSetCameraToLocation,
	// Token: 0x0400090E RID: 2318
	WorldMapRefreshTradeCaravanPath,
	// Token: 0x0400090F RID: 2319
	WorldMapShowPath,
	// Token: 0x04000910 RID: 2320
	WorldMapHidePath,
	// Token: 0x04000911 RID: 2321
	WorldMapUpdateAreaOffset,
	// Token: 0x04000912 RID: 2322
	PartWorldMapLookAt,
	// Token: 0x04000913 RID: 2323
	PartWorldMapDirectReturn,
	// Token: 0x04000914 RID: 2324
	PartWorldMapHide,
	// Token: 0x04000915 RID: 2325
	PartWorldMapDataChanged,
	// Token: 0x04000916 RID: 2326
	PartWorldMapHighlight,
	// Token: 0x04000917 RID: 2327
	WeatherChanged,
	// Token: 0x04000918 RID: 2328
	AnimalPlaceDataChange,
	// Token: 0x04000919 RID: 2329
	CricketPlaceDataChange,
	// Token: 0x0400091A RID: 2330
	OnMapBlockCharCustomInfoChanged,
	// Token: 0x0400091B RID: 2331
	OnMapBlockCharCustomButtonChanged,
	// Token: 0x0400091C RID: 2332
	BuildingOperatorChange,
	// Token: 0x0400091D RID: 2333
	BuildingShopManagerChange,
	// Token: 0x0400091E RID: 2334
	BuildingCustomNameChange,
	// Token: 0x0400091F RID: 2335
	BuildingResourceOutputSettingsChanged,
	// Token: 0x04000920 RID: 2336
	CloseBuildingManage,
	// Token: 0x04000921 RID: 2337
	OpenBuildingCraftsmanPanel,
	// Token: 0x04000922 RID: 2338
	SwitchBuildingManage,
	// Token: 0x04000923 RID: 2339
	NotifySwitchBuildingManage,
	// Token: 0x04000924 RID: 2340
	SwitchBuildingMake,
	// Token: 0x04000925 RID: 2341
	UpdateAllBlockInfo,
	// Token: 0x04000926 RID: 2342
	UpdateRoad,
	// Token: 0x04000927 RID: 2343
	RepairAllBuilding,
	// Token: 0x04000928 RID: 2344
	RepairBuilding,
	// Token: 0x04000929 RID: 2345
	StartPlacingBuilding,
	// Token: 0x0400092A RID: 2346
	HideMapBlockCharList,
	// Token: 0x0400092B RID: 2347
	CloseReadingEvent,
	// Token: 0x0400092C RID: 2348
	RefreshBookList,
	// Token: 0x0400092D RID: 2349
	StartPlanOrRemoveBuilding,
	// Token: 0x0400092E RID: 2350
	CancelPlanOrRemoveBuilding,
	// Token: 0x0400092F RID: 2351
	SettlementTreasuryEffect,
	// Token: 0x04000930 RID: 2352
	BuildingBlockDataChange,
	// Token: 0x04000931 RID: 2353
	BuildingVowLevelChanged,
	// Token: 0x04000932 RID: 2354
	BuildingManageArrangeFocusStart,
	// Token: 0x04000933 RID: 2355
	BuildingManageArrangeFocusFinish,
	// Token: 0x04000934 RID: 2356
	BuildingAreaHide,
	// Token: 0x04000935 RID: 2357
	SamsaraPlatformRecordDataChange,
	// Token: 0x04000936 RID: 2358
	TaiwuVillageShowShrineChange,
	// Token: 0x04000937 RID: 2359
	CloseMakePage,
	// Token: 0x04000938 RID: 2360
	RefreshVideo,
	// Token: 0x04000939 RID: 2361
	TutorialVideoSwitchToMiniSize,
	// Token: 0x0400093A RID: 2362
	[Obsolete]
	StopAutoStartCombat,
	// Token: 0x0400093B RID: 2363
	UpdateMapSettlementBtn,
	// Token: 0x0400093C RID: 2364
	EventWindowRefreshComplete,
	// Token: 0x0400093D RID: 2365
	EventWindowAppearResult,
	// Token: 0x0400093E RID: 2366
	EventWindowSelectOption,
	// Token: 0x0400093F RID: 2367
	EventWindowAutoChoose,
	// Token: 0x04000940 RID: 2368
	EventWindowSelectNormalInformation,
	// Token: 0x04000941 RID: 2369
	EventWindowSelectSecretInformation,
	// Token: 0x04000942 RID: 2370
	OnNewEventComingToShow,
	// Token: 0x04000943 RID: 2371
	VillagerWorkDataChange,
	// Token: 0x04000944 RID: 2372
	ShowAdvanceMonthConfirm,
	// Token: 0x04000945 RID: 2373
	OnAdvanceMonthAnimationComplete,
	// Token: 0x04000946 RID: 2374
	MonthNotifyProcessComplete,
	// Token: 0x04000947 RID: 2375
	AutoProcessMonthNotify,
	// Token: 0x04000948 RID: 2376
	OnMonthNotifySortingGroupChanged,
	// Token: 0x04000949 RID: 2377
	CombatTeammateChange,
	// Token: 0x0400094A RID: 2378
	TeammateCommandChanged,
	// Token: 0x0400094B RID: 2379
	UsingMedicineItemSwitch,
	// Token: 0x0400094C RID: 2380
	EnterTransferItem,
	// Token: 0x0400094D RID: 2381
	OnSelectTransferItemChar,
	// Token: 0x0400094E RID: 2382
	MultiplyTransferItemResult,
	// Token: 0x0400094F RID: 2383
	OnNewInstantNotification,
	// Token: 0x04000950 RID: 2384
	OnNewTravelNotification,
	// Token: 0x04000951 RID: 2385
	OnHealingOuterRestrictionUpdate,
	// Token: 0x04000952 RID: 2386
	OnHealingInnerRestrictionUpdate,
	// Token: 0x04000953 RID: 2387
	OnNeiliAllocationTypeRestrictionUpdate,
	// Token: 0x04000954 RID: 2388
	OnHandlingMonthlyEventBlockChange,
	// Token: 0x04000955 RID: 2389
	OnCharacterLifeRecordYearReady,
	// Token: 0x04000956 RID: 2390
	OnGetCharBirthDateByLifeRecordModel,
	// Token: 0x04000957 RID: 2391
	OnLifeRecordNameButtonClicked,
	// Token: 0x04000958 RID: 2392
	OnTeammateHideStateChange,
	// Token: 0x04000959 RID: 2393
	OnLocationMarkChange,
	// Token: 0x0400095A RID: 2394
	ItemMultiplyOperationTypeChange,
	// Token: 0x0400095B RID: 2395
	ItemMultiplyOperationContentChange,
	// Token: 0x0400095C RID: 2396
	ItemMultiplyOperationTargetChange,
	// Token: 0x0400095D RID: 2397
	ExitMultiplyOperation,
	// Token: 0x0400095E RID: 2398
	ItemMultiplyOperationConfirm,
	// Token: 0x0400095F RID: 2399
	ItemMultiplyOperationFinish,
	// Token: 0x04000960 RID: 2400
	ItemMultiplyOperationCancelSelection,
	// Token: 0x04000961 RID: 2401
	ItemGradeFilterSettingChange,
	// Token: 0x04000962 RID: 2402
	HideEventStyleFeatureSelectPage,
	// Token: 0x04000963 RID: 2403
	CharacterMenuHide,
	// Token: 0x04000964 RID: 2404
	OnMapBlockEnemyTogChange,
	// Token: 0x04000965 RID: 2405
	OnMapBlockFriendTogChange,
	// Token: 0x04000966 RID: 2406
	OnMapBlockMerchantTogChange,
	// Token: 0x04000967 RID: 2407
	OnMapBlockSettlementEdgeTogChange,
	// Token: 0x04000968 RID: 2408
	OnMapBlockPastLifeRelationTogChange,
	// Token: 0x04000969 RID: 2409
	OnMapBlockVisibilityNeedRefresh,
	// Token: 0x0400096A RID: 2410
	OnMapBlockMoveFinished,
	// Token: 0x0400096B RID: 2411
	OnForceRefreshAllMapBlock,
	// Token: 0x0400096C RID: 2412
	OnClickMapElement,
	// Token: 0x0400096D RID: 2413
	OnHoverMapElement,
	// Token: 0x0400096E RID: 2414
	OnHoverExitMapElement,
	// Token: 0x0400096F RID: 2415
	OnTravelStart,
	// Token: 0x04000970 RID: 2416
	OnTravelPauseStatusChanged,
	// Token: 0x04000971 RID: 2417
	OnTravelFinish,
	// Token: 0x04000972 RID: 2418
	HidePartWorldMap,
	// Token: 0x04000973 RID: 2419
	OnTravelCheckPointProcessed,
	// Token: 0x04000974 RID: 2420
	OnEventWindowDisplayDataChanged,
	// Token: 0x04000975 RID: 2421
	OnMarriageCharacterListChanged,
	// Token: 0x04000976 RID: 2422
	OnJieqingMaskCharacterListChanged,
	// Token: 0x04000977 RID: 2423
	OnEventHandleComplete,
	// Token: 0x04000978 RID: 2424
	OnLifeSkillCombatForceSilentResult,
	// Token: 0x04000979 RID: 2425
	OnLifeSkillCombatEnterCardFocusMode,
	// Token: 0x0400097A RID: 2426
	OnLifeSkillCombatExitCardFocusMode,
	// Token: 0x0400097B RID: 2427
	OnNeedCombatPause,
	// Token: 0x0400097C RID: 2428
	OnNeedCombatResume,
	// Token: 0x0400097D RID: 2429
	PlayCombatSoundOnce,
	// Token: 0x0400097E RID: 2430
	ShowCombatTeammateCommand,
	// Token: 0x0400097F RID: 2431
	ShowCombatSpecialEffectTips,
	// Token: 0x04000980 RID: 2432
	ShowCombatSurrenderMark,
	// Token: 0x04000981 RID: 2433
	ShowFleeAnimation,
	// Token: 0x04000982 RID: 2434
	ShowCombatTianSuiBaoLu,
	// Token: 0x04000983 RID: 2435
	ShowAbsorbNeiliAllocation,
	// Token: 0x04000984 RID: 2436
	CombatTutorialSettingChanged,
	// Token: 0x04000985 RID: 2437
	CombatPureMode,
	// Token: 0x04000986 RID: 2438
	EventTriggerCricketCatch,
	// Token: 0x04000987 RID: 2439
	CricketCombatUpdateProperty,
	// Token: 0x04000988 RID: 2440
	TaskAdd,
	// Token: 0x04000989 RID: 2441
	TopTask,
	// Token: 0x0400098A RID: 2442
	ClearTask,
	// Token: 0x0400098B RID: 2443
	TaskRemove,
	// Token: 0x0400098C RID: 2444
	TaskGroupDataUpdated,
	// Token: 0x0400098D RID: 2445
	TaskBubbleStart,
	// Token: 0x0400098E RID: 2446
	TaskBubbleEnded,
	// Token: 0x0400098F RID: 2447
	AdventurePlayStateChange,
	// Token: 0x04000990 RID: 2448
	AdventurePlayStateIsLockedChange,
	// Token: 0x04000991 RID: 2449
	AdventureMoveBtnInteractable,
	// Token: 0x04000992 RID: 2450
	OnOpenAdventureResult,
	// Token: 0x04000993 RID: 2451
	OnProfessionDataChange,
	// Token: 0x04000994 RID: 2452
	OnProfessionSlotsChange,
	// Token: 0x04000995 RID: 2453
	CloseProfessionMask,
	// Token: 0x04000996 RID: 2454
	UpdateProfessionIllustrationAndSkill,
	// Token: 0x04000997 RID: 2455
	ShowProfessionPropertyChange,
	// Token: 0x04000998 RID: 2456
	ShowProfessionTeammateUI,
	// Token: 0x04000999 RID: 2457
	PlayProfessionAudioSound,
	// Token: 0x0400099A RID: 2458
	PlayProfessionSkillAnimAndShowEffect,
	// Token: 0x0400099B RID: 2459
	SpecialProfessionSkillUpdate,
	// Token: 0x0400099C RID: 2460
	InvokeProfessionEvent,
	// Token: 0x0400099D RID: 2461
	ProfessionTravelerSkillTwoStart,
	// Token: 0x0400099E RID: 2462
	ProfessionTravelerSkillTwoStop,
	// Token: 0x0400099F RID: 2463
	ProfessionTravelerSkillTwoMoveStart,
	// Token: 0x040009A0 RID: 2464
	ProfessionTravelerSkillTwoMoveEnd,
	// Token: 0x040009A1 RID: 2465
	ProfessionTravelerSkillThreeMove,
	// Token: 0x040009A2 RID: 2466
	ProfessionSkillConfirmSelectCancel,
	// Token: 0x040009A3 RID: 2467
	ProfessionSkillUnlock,
	// Token: 0x040009A4 RID: 2468
	PlayAnimToHideMainUI,
	// Token: 0x040009A5 RID: 2469
	PlayAnimToShowMainUI,
	// Token: 0x040009A6 RID: 2470
	OnSetBottomInteractable,
	// Token: 0x040009A7 RID: 2471
	OnSetBottomRightPartVisible,
	// Token: 0x040009A8 RID: 2472
	OnUpdateQuickBtnState,
	// Token: 0x040009A9 RID: 2473
	OnRefreshWorkButton,
	// Token: 0x040009AA RID: 2474
	OnRefreshWorkPanel,
	// Token: 0x040009AB RID: 2475
	OnSetBuildingBtnShow,
	// Token: 0x040009AC RID: 2476
	[Obsolete]
	OnSetBuildingAreaInfo,
	// Token: 0x040009AD RID: 2477
	OnSetAreaSpiritualDebt,
	// Token: 0x040009AE RID: 2478
	OnRefreshBottomLifePanel,
	// Token: 0x040009AF RID: 2479
	OnRefreshCharacterHealUIBottom,
	// Token: 0x040009B0 RID: 2480
	OnRefreshCharacterMenuItem,
	// Token: 0x040009B1 RID: 2481
	WareHouseOnDisable,
	// Token: 0x040009B2 RID: 2482
	RefreshExceptionInfo,
	// Token: 0x040009B3 RID: 2483
	MouseTipBaseOnEnable,
	// Token: 0x040009B4 RID: 2484
	MouseTipBaseOnDisable,
	// Token: 0x040009B5 RID: 2485
	OnUpdateCaravanBlockCharData,
	// Token: 0x040009B6 RID: 2486
	OnInstantNotificationEventStopReceive,
	// Token: 0x040009B7 RID: 2487
	OnInstantNotificationEventStartReceive,
	// Token: 0x040009B8 RID: 2488
	OnMoveTimeCostPercentChanged,
	// Token: 0x040009B9 RID: 2489
	OnVisitedSettlementsChanged,
	// Token: 0x040009BA RID: 2490
	OnCharacterTaiwuGenderChanged,
	// Token: 0x040009BB RID: 2491
	OnCharacterTaiwuInventoryChanged,
	// Token: 0x040009BC RID: 2492
	OnCharacterTaiwuCarrierChanged,
	// Token: 0x040009BD RID: 2493
	OnStateAdventureDataReceived,
	// Token: 0x040009BE RID: 2494
	OnAdventureTaiwuChanged,
	// Token: 0x040009BF RID: 2495
	AdventureRemakeEnter,
	// Token: 0x040009C0 RID: 2496
	AdventureRemakeExit,
	// Token: 0x040009C1 RID: 2497
	AdventureUnitMicroClick,
	// Token: 0x040009C2 RID: 2498
	AdventureUnitMicroPointerEnter,
	// Token: 0x040009C3 RID: 2499
	AdventureUnitMicroPointerExit,
	// Token: 0x040009C4 RID: 2500
	AdventureExitClick,
	// Token: 0x040009C5 RID: 2501
	AdventureRemakeIconClick,
	// Token: 0x040009C6 RID: 2502
	AdventureRemakeOpenPartOne,
	// Token: 0x040009C7 RID: 2503
	MajorEventSkipCompleteAnim,
	// Token: 0x040009C8 RID: 2504
	AdventureRemakeOpenPartTwo,
	// Token: 0x040009C9 RID: 2505
	AdventureRemakeDictChange,
	// Token: 0x040009CA RID: 2506
	AdventureMajorEventChange,
	// Token: 0x040009CB RID: 2507
	AdventureRemakeEditorStatusReverted,
	// Token: 0x040009CC RID: 2508
	AdventureRemakeEditorStatusEvolved,
	// Token: 0x040009CD RID: 2509
	AdventureRemakeEditorStatusLoaded,
	// Token: 0x040009CE RID: 2510
	AdventureRemakeEditorStatusSaved,
	// Token: 0x040009CF RID: 2511
	AdventureEditorToggleGroupPanel,
	// Token: 0x040009D0 RID: 2512
	AdventureRemakeFinish,
	// Token: 0x040009D1 RID: 2513
	AdventureElementAlertAnim,
	// Token: 0x040009D2 RID: 2514
	AdventureBlockChangeIcon,
	// Token: 0x040009D3 RID: 2515
	AdventureElementShowHideEffect,
	// Token: 0x040009D4 RID: 2516
	AdventureRefreshBlockEffect,
	// Token: 0x040009D5 RID: 2517
	AdventureRefreshGlobalEffect,
	// Token: 0x040009D6 RID: 2518
	AdventureTaiwuShowDialog,
	// Token: 0x040009D7 RID: 2519
	AdventureEventHandled,
	// Token: 0x040009D8 RID: 2520
	AdventureElementShowDialog,
	// Token: 0x040009D9 RID: 2521
	AdventureElementDeleteAnim,
	// Token: 0x040009DA RID: 2522
	AdventureCameraMoveToBlock,
	// Token: 0x040009DB RID: 2523
	AdventureDelayAction,
	// Token: 0x040009DC RID: 2524
	AdventureStartSelectElement,
	// Token: 0x040009DD RID: 2525
	AdventureAdvanceDaysSet,
	// Token: 0x040009DE RID: 2526
	[Obsolete]
	AdventureClickAdvanceBtn,
	// Token: 0x040009DF RID: 2527
	AdventureResetCamera,
	// Token: 0x040009E0 RID: 2528
	AdventureBanTimeBall,
	// Token: 0x040009E1 RID: 2529
	AdventureMajorEventIconClick,
	// Token: 0x040009E2 RID: 2530
	OnAdventureMajorEventTaiwuChanged,
	// Token: 0x040009E3 RID: 2531
	AdventureEditorRemakeRefreshPropertyPanel,
	// Token: 0x040009E4 RID: 2532
	AdventureEditorPureModeSwitch,
	// Token: 0x040009E5 RID: 2533
	EnterCharacterMenuPractice,
	// Token: 0x040009E6 RID: 2534
	OnNeedOpenCharacterMenuSubPage,
	// Token: 0x040009E7 RID: 2535
	OnTravelPathUnlocked,
	// Token: 0x040009E8 RID: 2536
	OnSweepNetAmountTipsChanged,
	// Token: 0x040009E9 RID: 2537
	OnDreamBackStatusChanged,
	// Token: 0x040009EA RID: 2538
	OnTextureShowChanged,
	// Token: 0x040009EB RID: 2539
	OnMapAlterSettlementChanged,
	// Token: 0x040009EC RID: 2540
	OnMapCharacterChanged,
	// Token: 0x040009ED RID: 2541
	OnBloodLightLocationsChanged,
	// Token: 0x040009EE RID: 2542
	OnFairylandDataChanged,
	// Token: 0x040009EF RID: 2543
	OnHeavenlyTreeDataChanged,
	// Token: 0x040009F0 RID: 2544
	OnLingBaoLightLocationsChanged,
	// Token: 0x040009F1 RID: 2545
	OnLingBaoDarkLocationsChanged,
	// Token: 0x040009F2 RID: 2546
	OnBloodLocationsChanged,
	// Token: 0x040009F3 RID: 2547
	OnFulongInFlameAreasChanged,
	// Token: 0x040009F4 RID: 2548
	OnWorldMapCharacterImpactRangeChanged,
	// Token: 0x040009F5 RID: 2549
	OnZhujianThiefDataChanged,
	// Token: 0x040009F6 RID: 2550
	OnDreamBackLocationsChanged,
	// Token: 0x040009F7 RID: 2551
	OnFiveLoongDictChanged,
	// Token: 0x040009F8 RID: 2552
	OnBrokenMaterialDataChange,
	// Token: 0x040009F9 RID: 2553
	OnBrokenMaterialEventInvoked,
	// Token: 0x040009FA RID: 2554
	RequestBottomTimeDisk,
	// Token: 0x040009FB RID: 2555
	ResponseBottomTimeDisk,
	// Token: 0x040009FC RID: 2556
	ReturnBottomTimeDisk,
	// Token: 0x040009FD RID: 2557
	OnMapBlockDisplayDataChanged,
	// Token: 0x040009FE RID: 2558
	OnExtraMapBlockDataRequested,
	// Token: 0x040009FF RID: 2559
	OnStopVillagerWork,
	// Token: 0x04000A00 RID: 2560
	OnBottomShowAllLogClicked,
	// Token: 0x04000A01 RID: 2561
	OnBottomShowNewNotification,
	// Token: 0x04000A02 RID: 2562
	OnSetMapBlockCharListTog,
	// Token: 0x04000A03 RID: 2563
	MapFocusLocationGrave,
	// Token: 0x04000A04 RID: 2564
	MapClearLocationTemporaryMark,
	// Token: 0x04000A05 RID: 2565
	MapAddLocationsToTemporaryMarkList,
	// Token: 0x04000A06 RID: 2566
	MapAddLocationsToTemporaryMarkListForTask,
	// Token: 0x04000A07 RID: 2567
	MapClearAllTemporaryMarkListForTask,
	// Token: 0x04000A08 RID: 2568
	MapClearAllTemporaryMarkList,
	// Token: 0x04000A09 RID: 2569
	MapBlockChange,
	// Token: 0x04000A0A RID: 2570
	UpdateMapBlockData,
	// Token: 0x04000A0B RID: 2571
	MapCurrentLocationFixedCharacterDataReceived,
	// Token: 0x04000A0C RID: 2572
	UpdateCombatSkillDisplayDataMastered,
	// Token: 0x04000A0D RID: 2573
	ReceivedCombatSkillDisplayData,
	// Token: 0x04000A0E RID: 2574
	OnShowBuildingArea,
	// Token: 0x04000A0F RID: 2575
	OnHideBuildingArea,
	// Token: 0x04000A10 RID: 2576
	HideBuildingArea,
	// Token: 0x04000A11 RID: 2577
	WorldMapDoShakeByEvent,
	// Token: 0x04000A12 RID: 2578
	WorldMapDoFocusByEvent,
	// Token: 0x04000A13 RID: 2579
	DefeatSwordTomb,
	// Token: 0x04000A14 RID: 2580
	OnMusicPlayerEnabledStateChange,
	// Token: 0x04000A15 RID: 2581
	OnMusicPlayerPlayStateChange,
	// Token: 0x04000A16 RID: 2582
	OnConfirmSectEmeiConsumBookToGetExp,
	// Token: 0x04000A17 RID: 2583
	[Obsolete]
	OnChangeTopUiInAdventure,
	// Token: 0x04000A18 RID: 2584
	OnRelationButtonClick,
	// Token: 0x04000A19 RID: 2585
	OnSkillCasting,
	// Token: 0x04000A1A RID: 2586
	OnTryInterruptSkillCasting,
	// Token: 0x04000A1B RID: 2587
	OnSkillCasted,
	// Token: 0x04000A1C RID: 2588
	OnLegendaryBookGiveUpStart,
	// Token: 0x04000A1D RID: 2589
	ClearIncludedInscriptionChar,
	// Token: 0x04000A1E RID: 2590
	ConfirmIncludedInscriptionChar,
	// Token: 0x04000A1F RID: 2591
	ConfirmIncludedInscriptionCharMultipleChoice,
	// Token: 0x04000A20 RID: 2592
	OnEventWindowStart,
	// Token: 0x04000A21 RID: 2593
	OnEventWindowEnded,
	// Token: 0x04000A22 RID: 2594
	RefreshFollowing,
	// Token: 0x04000A23 RID: 2595
	NickNameChanged,
	// Token: 0x04000A24 RID: 2596
	OnTaiwuOriginalSurnameSettingChanged,
	// Token: 0x04000A25 RID: 2597
	OnVillagerDispatched,
	// Token: 0x04000A26 RID: 2598
	RefreshVillagerRoleDispatch,
	// Token: 0x04000A27 RID: 2599
	VillagerRoleNickNameChanged,
	// Token: 0x04000A28 RID: 2600
	OnSetVillagerRole,
	// Token: 0x04000A29 RID: 2601
	OnAssignChickenToVillagerRole,
	// Token: 0x04000A2A RID: 2602
	RealConfirmExecuteProfessionSkill,
	// Token: 0x04000A2B RID: 2603
	SentChangeCharacterMasteredCombatSkill,
	// Token: 0x04000A2C RID: 2604
	MapPickupDataChanged,
	// Token: 0x04000A2D RID: 2605
	MapPickupEffectChanged,
	// Token: 0x04000A2E RID: 2606
	CricketWishEffectChanged,
	// Token: 0x04000A2F RID: 2607
	ShowCombatLifeSkillHiddenInfo,
	// Token: 0x04000A30 RID: 2608
	ShowCombatLifeSkillTalk,
	// Token: 0x04000A31 RID: 2609
	CombatLifeSkillClickUnit,
	// Token: 0x04000A32 RID: 2610
	CombatLifeSkillHoverUnit,
	// Token: 0x04000A33 RID: 2611
	CombatLifeSkillClickBlock,
	// Token: 0x04000A34 RID: 2612
	CombatLifeSkillHoverStrategy,
	// Token: 0x04000A35 RID: 2613
	CombatLifeSkillUnlockStrategy,
	// Token: 0x04000A36 RID: 2614
	ChangeLifeSkillCombatData,
	// Token: 0x04000A37 RID: 2615
	CombatLifeSkillClickChar,
	// Token: 0x04000A38 RID: 2616
	OnTaiwuReadingBookProgressMayChange,
	// Token: 0x04000A39 RID: 2617
	OnTaiwuLoopingNeigongMayChange,
	// Token: 0x04000A3A RID: 2618
	OnCombatSkillMasteryChanged,
	// Token: 0x04000A3B RID: 2619
	OnTaiwuReadingBookKeyMayChange,
	// Token: 0x04000A3C RID: 2620
	OnEnableAutoTriggerNormalMapPickupChanged,
	// Token: 0x04000A3D RID: 2621
	AdventureLightingSettingChanged,
	// Token: 0x04000A3E RID: 2622
	OnConfirmVillagerCraftInputMaterial,
	// Token: 0x04000A3F RID: 2623
	HideCharacterMenuAndSubPages,
	// Token: 0x04000A40 RID: 2624
	RestoreCharacterMenuAndSubPages,
	// Token: 0x04000A41 RID: 2625
	HealUiClosed,
	// Token: 0x04000A42 RID: 2626
	DoWorldMapScaleTween,
	// Token: 0x04000A43 RID: 2627
	OnClickWorldMapBlock,
	// Token: 0x04000A44 RID: 2628
	OnReshowBlockList,
	// Token: 0x04000A45 RID: 2629
	OnLanguageChange,
	// Token: 0x04000A46 RID: 2630
	ReleaseCoverObject,
	// Token: 0x04000A47 RID: 2631
	[Obsolete]
	NotifyOpenBuildingManage,
	// Token: 0x04000A48 RID: 2632
	BuildingManageClosed,
	// Token: 0x04000A49 RID: 2633
	PlaySkeletonParticle,
	// Token: 0x04000A4A RID: 2634
	PlayVitalAnim,
	// Token: 0x04000A4B RID: 2635
	BuildQiwenxingtai,
	// Token: 0x04000A4C RID: 2636
	OnJieqingSignStateRefresh,
	// Token: 0x04000A4D RID: 2637
	OnAddCraftsmanResource,
	// Token: 0x04000A4E RID: 2638
	OnPutResourcePreview,
	// Token: 0x04000A4F RID: 2639
	OnChangeCharacterClothing,
	// Token: 0x04000A50 RID: 2640
	InviteSelectBlockStart,
	// Token: 0x04000A51 RID: 2641
	InviteSelectBlockStop,
	// Token: 0x04000A52 RID: 2642
	DivineFlameSelectBlockStart,
	// Token: 0x04000A53 RID: 2643
	DivineFlameSelectBlockStop,
	// Token: 0x04000A54 RID: 2644
	RequestOpenBuildingManage,
	// Token: 0x04000A55 RID: 2645
	QuickActionMenuBackgroundClicked,
	// Token: 0x04000A56 RID: 2646
	PickupDisplayInfoChange,
	// Token: 0x04000A57 RID: 2647
	GmReloadCombatScene,
	// Token: 0x04000A58 RID: 2648
	GmUpdateWeather,
	// Token: 0x04000A59 RID: 2649
	NewFeatureUnlockHint,
	// Token: 0x04000A5A RID: 2650
	ViewDefendHeavenlyTreeRefresh,
	// Token: 0x04000A5B RID: 2651
	ExchangeResource,
	// Token: 0x04000A5C RID: 2652
	OnTaiwuFeatureDeleted,
	// Token: 0x04000A5D RID: 2653
	OnPointEnterItemMenuTake,
	// Token: 0x04000A5E RID: 2654
	OnPointExitItemMenuTake,
	// Token: 0x04000A5F RID: 2655
	OnConfirmSetSelectCount,
	// Token: 0x04000A60 RID: 2656
	NotifyBottomCustomButtonChange,
	// Token: 0x04000A61 RID: 2657
	NotifyBottomUnlockScrollListChange,
	// Token: 0x04000A62 RID: 2658
	DisableDependenciesChangedMods,
	// Token: 0x04000A63 RID: 2659
	WorkshopModPreviewImageHasDownloaded,
	// Token: 0x04000A64 RID: 2660
	ModEditSettings,
	// Token: 0x04000A65 RID: 2661
	OnModViewShowMask,
	// Token: 0x04000A66 RID: 2662
	OnModViewHideMask,
	// Token: 0x04000A67 RID: 2663
	OnModViewRefresh,
	// Token: 0x04000A68 RID: 2664
	OnModViewDownload,
	// Token: 0x04000A69 RID: 2665
	OnModViewUnSubscribe,
	// Token: 0x04000A6A RID: 2666
	RefreshCharacterMenuStack,
	// Token: 0x04000A6B RID: 2667
	OnEventWindowSelectItems,
	// Token: 0x04000A6C RID: 2668
	RefreshAllAvatar,
	// Token: 0x04000A6D RID: 2669
	ShowUnlockScrollBtnAnim,
	// Token: 0x04000A6E RID: 2670
	RequestLegacyItemRefresh,
	// Token: 0x04000A6F RID: 2671
	OnIronPlateDataChanged,
	// Token: 0x04000A70 RID: 2672
	OnEmeiGuidanceDataChanged,
	// Token: 0x04000A71 RID: 2673
	OnForceUpdateTime,
	// Token: 0x04000A72 RID: 2674
	OnShowUsingMedicine,
	// Token: 0x04000A73 RID: 2675
	OnShowEatDetail,
	// Token: 0x04000A74 RID: 2676
	CombatSpeedChanged
}
