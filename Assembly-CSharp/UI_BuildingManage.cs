using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Building;
using Coffee.UIExtensions;
using Config;
using Config.Common;
using Config.ConfigCells.Character;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using Game.Components.Avatar;
using Game.Views.Building;
using Game.Views.Building.BuildingAreaQuickActionMenu;
using Game.Views.CharacterMenu;
using Game.Views.VillagerRoleView;
using GameData.Combat.Math;
using GameData.Common;
using GameData.Domains.Building;
using GameData.Domains.Building.SamsaraPlatformRecord;
using GameData.Domains.Building.ShopEvent;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.LifeRecord;
using GameData.Domains.LifeRecord.GeneralRecord;
using GameData.Domains.Map;
using GameData.Domains.Organization;
using GameData.Domains.Story;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UICommon.Character.Elements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020001B0 RID: 432
[Obsolete]
public class UI_BuildingManage : UIBase
{
	// Token: 0x170002A7 RID: 679
	// (get) Token: 0x060018AA RID: 6314 RVA: 0x000996E6 File Offset: 0x000978E6
	private bool IsBuildingManagementUnlocked
	{
		get
		{
			return SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10);
		}
	}

	// Token: 0x170002A8 RID: 680
	// (get) Token: 0x060018AB RID: 6315 RVA: 0x000996F4 File Offset: 0x000978F4
	private List<UI_BuildingManage.EventBookMonthlyData> _currentEventBookDatas
	{
		get
		{
			return (this._mainToggleGroup.GetActive().Key == 1) ? this._shopLearnEventRenderInfos : this._shopManageEventRenderInfos;
		}
	}

	// Token: 0x060018AC RID: 6316 RVA: 0x00099718 File Offset: 0x00097918
	private void ResetSensitiveWordTip()
	{
		bool flag = this._sensitiveWordTipCoroutine != null;
		if (flag)
		{
			base.StopCoroutine(this._sensitiveWordTipCoroutine);
		}
		CanvasGroup commonWarningCanvasGroup = this._leftInfo.CGet<CanvasGroup>("SensitiveWarningTip");
		commonWarningCanvasGroup.alpha = 0f;
	}

	// Token: 0x060018AD RID: 6317 RVA: 0x00099760 File Offset: 0x00097960
	private void Awake()
	{
		this.InitRefers();
		this._removeCollectInfoPage = base.CGet<Refers>("ExpandRemoveCollectInfoPage");
		this._expandInfoItem = this._removeCollectInfoPage.CGet<Refers>("ExpandInfoItem");
		this._expandTitleContent = this._removeCollectInfoPage.CGet<GameObject>("ExpandTitleContent").GetComponent<RectTransform>();
		this._kuangBg = this._removeCollectInfoPage.CGet<RectTransform>("KuangBg");
		this._eventBook = this._shopInfoPage.CGet<GameObject>("EventBook");
		this._autoArrangeToggle = this._shopInfoPage.CGet<CToggleObsolete>("AutoArrangeToggle");
		this._autoSoldToggle = this._shopInfoPage.CGet<CToggleObsolete>("AutoSoldToggle");
		this._shopQuickSelectBtn = this._shopInfoPage.CGet<CButtonObsolete>("ShopQuickSelectBtn");
		this._shopQuickClearBtn = this._shopInfoPage.CGet<CButtonObsolete>("ShopQuickSelectCancelBtn");
		this._quickAddSoldItemBtn = this._shopInfoPage.CGet<CButtonObsolete>("QuickAddSoldItemBtn");
		this._quickAddSoldItemBtn.ClearAndAddListener(new Action(this.OnClickQuickAddSoldItemBtn));
		this._quickRemoveSoldItemBtn = this._shopInfoPage.CGet<CButtonObsolete>("QuickRemoveSoldItemBtn");
		this._quickRemoveSoldItemBtn.ClearAndAddListener(new Action(this.OnClickQuickRemoveSoldItemBtn));
		this._soldItemSettingBtn = this._shopInfoPage.CGet<CButtonObsolete>("SoldItemSettingBtn");
		this._soldItemSettingBtn.ClearAndAddListener(new Action(this.OnClickSoldItemSettingBtn));
		this._btnHolder = base.CGet<RectTransform>("ButtonHolder");
		this._residentsHolder = base.CGet<GameObject>("ResidentInfo");
		this._residentViewPrefab = this._infoPage.CGet<GameObject>("ResidentView3");
		this._residentCount = this._infoPage.CGet<TextMeshProUGUI>("ResidentTitleText");
		this._mainToggleGroup.InitPreOnToggle(-1);
		this._mainToggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnToggleChange);
		this._settlementId = SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageSettlementId();
		this._buildingModel = SingletonObject.getInstance<BuildingModel>();
		this._villageLocation = SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageBlock();
		this._autoCheckInToggle = this._infoPage.CGet<CToggleObsolete>("AutoCheckInToggle");
		this._autoCheckInMouseTip = this._infoPage.CGet<TooltipInvoker>("AutoCheckInMouseTip");
		this._currentVillagerList = new List<VillagerRoleCharacterDisplayData>[VillagerRole.Instance.Count];
		this._lostVillagerList = new List<VillagerRoleCharacterDisplayData>[VillagerRole.Instance.Count];
	}

	// Token: 0x060018AE RID: 6318 RVA: 0x000999B8 File Offset: 0x00097BB8
	private void InitResourceCollectToggle()
	{
		GameObject resourceOutput = this._shopInfoPage.CGet<GameObject>("ResourceOutput");
		CToggleGroupObsolete resourceOutputInfoHolder = this._shopInfoPage.CGet<CToggleGroupObsolete>("ResourceOutputInfoHolder");
		GameObject resourceOutputInfo = this._shopInfoPage.CGet<GameObject>("ResourceOutputInfo");
		CToggleGroupObsolete resourceStorageToggleGroup = this._shopInfoPage.CGet<CToggleGroupObsolete>("ResourceStorageToggleGroup");
		bool flag = this._shopEventData != null && this._shopEventData.ResourceList.Count > 0;
		if (flag)
		{
			resourceOutput.SetActive(true);
			this.ReuseGameObjectFunc(resourceOutputInfo, this._shopEventData.ResourceList.Count, resourceOutputInfoHolder.transform);
			bool flag2 = this._shopEventData.ResourceList.Count <= 1;
			sbyte curResourceType;
			if (flag2)
			{
				CToggleObsolete toggle = this._reuseDic[resourceOutputInfo.name][0].GetComponent<CToggleObsolete>();
				toggle.isOn = true;
				curResourceType = this._shopEventData.ResourceList.First<sbyte>();
			}
			else
			{
				resourceOutputInfoHolder.Clear();
				for (int i = 0; i < this._shopEventData.ResourceList.Count; i++)
				{
					CToggleObsolete toggle2 = this._reuseDic[resourceOutputInfo.name][i].GetComponent<CToggleObsolete>();
					toggle2.Key = (int)this._shopEventData.ResourceList[i];
					resourceOutputInfoHolder.Add(toggle2);
				}
				resourceOutputInfoHolder.OnActiveToggleChange = null;
				resourceOutputInfoHolder.InitPreOnToggle(-1);
				curResourceType = this._buildingModel.GetCollectBuildingResourceTypeWithToxicology(this._blockKey, this._blockData);
				resourceOutputInfoHolder.Set((int)curResourceType, true, false);
				resourceOutputInfoHolder.OnActiveToggleChange = delegate(CToggleObsolete togNew, CToggleObsolete togOld)
				{
					sbyte resourceType = (sbyte)togNew.Key;
					BuildingDomainMethod.Call.SetCollectBuildingResourceType(this._blockKey, resourceType);
					this.UpdateShopManagersByResourceType(resourceType);
					this.UpdateProductRequireTip();
					this.RefreshShopEventStorageToggleGroup(resourceStorageToggleGroup, resourceType, false, false);
				};
			}
			bool showTog = curResourceType <= 6;
			resourceStorageToggleGroup.gameObject.SetActive(showTog);
			resourceStorageToggleGroup.InitPreOnToggle(-1);
			resourceStorageToggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnCollectStorageToggleChange);
			this.RefreshShopEventStorageToggleGroup(resourceStorageToggleGroup, curResourceType, false, false);
		}
		else
		{
			resourceOutput.SetActive(false);
		}
	}

	// Token: 0x060018AF RID: 6319 RVA: 0x00099BDB File Offset: 0x00097DDB
	private void OnCollectStorageToggleChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
	}

	// Token: 0x060018B0 RID: 6320 RVA: 0x00099BDE File Offset: 0x00097DDE
	private void OnOutputStorageToggleChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
	}

	// Token: 0x060018B1 RID: 6321 RVA: 0x00099BE1 File Offset: 0x00097DE1
	private void OnSoldStorageToggleChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
	}

	// Token: 0x060018B2 RID: 6322 RVA: 0x00099BE4 File Offset: 0x00097DE4
	private TaiwuVillageStorageType GetShopEventStorageType(UI_BuildingManage.ShopEventStorageToggleKey togKey, sbyte resourceType)
	{
		if (!true)
		{
		}
		TaiwuVillageStorageType result;
		switch (togKey)
		{
		case UI_BuildingManage.ShopEventStorageToggleKey.Inventory:
			result = TaiwuVillageStorageType.Inventory;
			break;
		case UI_BuildingManage.ShopEventStorageToggleKey.Warehouse:
			result = TaiwuVillageStorageType.Warehouse;
			break;
		case UI_BuildingManage.ShopEventStorageToggleKey.Treasury:
			result = TaiwuVillageStorageType.Treasury;
			break;
		case UI_BuildingManage.ShopEventStorageToggleKey.Stock:
			result = TaiwuVillageStorageType.Stock;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x060018B3 RID: 6323 RVA: 0x00099C30 File Offset: 0x00097E30
	private UI_BuildingManage.ShopEventStorageToggleKey GetShopEventStorageToggleKey(TaiwuVillageStorageType storageType, bool isMaterial)
	{
		if (!true)
		{
		}
		UI_BuildingManage.ShopEventStorageToggleKey result;
		switch (storageType)
		{
		case TaiwuVillageStorageType.Inventory:
			result = UI_BuildingManage.ShopEventStorageToggleKey.Inventory;
			break;
		case TaiwuVillageStorageType.Warehouse:
			result = UI_BuildingManage.ShopEventStorageToggleKey.Warehouse;
			break;
		case TaiwuVillageStorageType.Treasury:
			result = UI_BuildingManage.ShopEventStorageToggleKey.Treasury;
			break;
		case TaiwuVillageStorageType.Stock:
			result = UI_BuildingManage.ShopEventStorageToggleKey.Stock;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x060018B4 RID: 6324 RVA: 0x00099C7C File Offset: 0x00097E7C
	private void RefreshShopEventStorageToggleGroup(CToggleGroupObsolete toggleGroup, sbyte resourceType, bool isItem, bool isMakeMaterial)
	{
		bool flag = !toggleGroup || !toggleGroup.gameObject.activeSelf;
		if (!flag)
		{
			toggleGroup.Get(UI_BuildingManage.ShopEventStorageToggleKey.Inventory.ToInt()).gameObject.SetActive(!isItem);
			toggleGroup.Get(UI_BuildingManage.ShopEventStorageToggleKey.Warehouse.ToInt()).gameObject.SetActive(isItem);
			toggleGroup.Get(UI_BuildingManage.ShopEventStorageToggleKey.Treasury.ToInt()).gameObject.SetActive(true);
			toggleGroup.Get(UI_BuildingManage.ShopEventStorageToggleKey.Stock.ToInt()).gameObject.SetActive(isItem);
			BuildingResourceOutputSetting setting = this._buildingModel.GetBuildingShopEventSetting((int)this._blockKey.BuildingBlockIndex);
			TaiwuVillageStorageType storageType = isItem ? setting.ItemStorage : setting.ResourceStorage;
			UI_BuildingManage.ShopEventStorageToggleKey togKey = this.GetShopEventStorageToggleKey(storageType, isMakeMaterial);
			toggleGroup.SetWithoutNotify(togKey.ToInt(), true);
			foreach (CToggleObsolete toggle in toggleGroup.GetAll())
			{
				bool flag2 = !toggle.gameObject.activeSelf;
				if (!flag2)
				{
					TaiwuVillageStorageType curStorageType = this.GetShopEventStorageType((UI_BuildingManage.ShopEventStorageToggleKey)toggle.Key, resourceType);
					ItemSourceType itemSourceType = BuildingResourceOutputSetting.GetItemSourceType(curStorageType);
					string sourceTypeName = UI_Warehouse.GetTitle(itemSourceType, true);
					string title = LocalStringManager.Get(LanguageKey.LK_Building_OutputStorageTip_Title);
					string content = LocalStringManager.GetFormat(LanguageKey.LK_Building_OutputStorageTip_Content, sourceTypeName);
					TooltipInvoker tip = toggle.GetComponentInChildren<TooltipInvoker>(true);
					tip.Type = TipType.Simple;
					string[] presetParam = tip.PresetParam;
					bool flag3 = presetParam == null || presetParam.Length != 2;
					if (flag3)
					{
						tip.PresetParam = new string[2];
					}
					tip.PresetParam[0] = title;
					tip.PresetParam[1] = content;
				}
			}
		}
	}

	// Token: 0x060018B5 RID: 6325 RVA: 0x00099E60 File Offset: 0x00098060
	public override void OnInit(ArgumentBox argsBox)
	{
		this.InitRefers();
		bool flag = argsBox == null;
		if (!flag)
		{
			argsBox.Get("AreaId", out this._areaId);
			argsBox.Get("BlockId", out this._blockId);
			argsBox.Get("BlockTemplateId", out this._blockTemplateId);
			argsBox.Get("BuildingBlockIndex", out this._buildingBlockIndex);
			argsBox.Get<BuildingBlockData>("BuildingBlockData", out this._blockData);
			argsBox.Get<BuildingAreaData>("BuildingAreaData", out this._areaData);
			int tabKey;
			argsBox.Get("tabKey", out tabKey);
			bool flag2 = tabKey >= 0;
			if (flag2)
			{
				this.SetInitialTab(tabKey);
			}
			this._blockKey = new BuildingBlockKey(this._areaId, this._blockId, this._blockData.BlockIndex);
			this._configData = BuildingBlock.Instance[this._blockData.TemplateId];
			this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			this._resourceBlockRanking = int.MaxValue;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
			this.PlayOpenSound(this._blockData.TemplateId);
		}
	}

	// Token: 0x060018B6 RID: 6326 RVA: 0x00099FA4 File Offset: 0x000981A4
	private void PlayOpenSound(short templateId)
	{
		string soundName;
		UI_BuildingManage.SoundDict.TryGetValue(templateId, out soundName);
		foreach (KeyValuePair<short, string> pair in UI_BuildingManage.SoundDict)
		{
			AudioManager.Instance.StopAllSound(pair.Value);
		}
		AudioManager.Instance.PlaySound(soundName, false, false);
	}

	// Token: 0x060018B7 RID: 6327 RVA: 0x0009A020 File Offset: 0x00098220
	private void DisplayAllResident()
	{
		for (int i = 0; i < this._residentViews.Count; i++)
		{
			this.DisplayResident(null, this._residentViews[i], true);
		}
	}

	// Token: 0x060018B8 RID: 6328 RVA: 0x0009A060 File Offset: 0x00098260
	private void OnListenerIdReady()
	{
		this.ResetState();
		this.DisplayAllResident();
		this.UpdateBuildingData();
		this.InitResourceCollectToggle();
		this.UpdateProgresInfo();
		this.UpdateToggles();
		TaiwuDomainMethod.Call.CanTransferItemToWarehouse(this.Element.GameDataListenerId);
		BuildingDomainMethod.Call.GetTaiwuVillageResourceBlockEffectInfo(this.Element.GameDataListenerId, this._configData.TemplateId);
		BuildingDomainMethod.Call.GetTaiwuVillageResourceBlockEffect(this.Element.GameDataListenerId, EBuildingScaleEffect.ShopProgressBonus);
		this.GetBuildingCoreInfo();
		bool flag = this._configData.TemplateId == 50;
		if (flag)
		{
			BuildingDomainMethod.Call.GetSamsaraPlatformRecord(this.Element.GameDataListenerId);
		}
		this.ApplyInitialTab();
	}

	// Token: 0x060018B9 RID: 6329 RVA: 0x0009A10C File Offset: 0x0009830C
	private void ResetState()
	{
		CButtonObsolete expandRemoveButton = this._removeCollectInfoPage.CGet<CButtonObsolete>("ExpandQuickSelectBtn");
		expandRemoveButton.interactable = this.IsBuildingManagementUnlocked;
		this._leftInfo.CGet<CanvasGroup>("SensitiveWarningTip").alpha = 0f;
		this._infoPage.CGet<CButtonObsolete>("ResidentViewQuickBtn").interactable = this.IsBuildingManagementUnlocked;
		this._btnTemplate.SetActive(false);
		this._residentViewPrefab.SetActive(false);
		this._buttonHolderLine.SetActive(false);
		this.ArrangementSettingPanel.gameObject.SetActive(false);
		this._shopQuickClearBtn.interactable = true;
		this._shopQuickSelectBtn.interactable = true;
		this._selectingOperatorIndex = -1;
		this._selectingShopManagerIndex = -1;
		this._isTaiwuVillageBuilding = (this._areaId == this._villageLocation.AreaId && this._blockId == this._villageLocation.BlockId);
		for (int i = 0; i < this._operatorListCached.Length; i++)
		{
			this._operatorListCached[i] = -1;
		}
		for (int j = 0; j < this._shopManagerListCached.Length; j++)
		{
			this._shopManagerListCached[j] = -1;
		}
	}

	// Token: 0x060018BA RID: 6330 RVA: 0x0009A244 File Offset: 0x00098444
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)this._taiwuCharId), new uint[]
		{
			59U,
			29U
		}));
		this.MonitorFields.Add(new UIBase.MonitorDataField(3, 1, (ulong)this._settlementId, new uint[]
		{
			10U
		}));
		this.MonitorFields.Add(new UIBase.MonitorDataField(19, 56, ulong.MaxValue, null));
		bool flag = (this._blockData.TemplateId == 48 || this._blockData.TemplateId == 257 || this._blockData.TemplateId == 258) && this._blockData.OperationType != 0;
		if (flag)
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(5, 8, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(5, 7, ulong.MaxValue, null));
		}
		bool isEntertain = this.IsEntertain;
		if (isEntertain)
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 192, ulong.MaxValue, null));
		}
		bool flag2 = this._configData.IsShop || this.IsEntertain;
		if (flag2)
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(9, 17, ulong.MaxValue, null));
		}
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 9, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 10, ulong.MaxValue, null));
	}

	// Token: 0x060018BB RID: 6331 RVA: 0x0009A3B4 File Offset: 0x000985B4
	private void OnEnable()
	{
		this._displayInited = false;
		base.CGet<CButtonObsolete>("ConfirmBtn").interactable = false;
		bool isOn = this._mainToggleGroup.Get(0).isOn;
		if (isOn)
		{
			this.OnToggleChange(this._mainToggleGroup.Get(0), null);
		}
		else
		{
			this._mainToggleGroup.Set(0, true, false);
		}
		GEvent.Add(UiEvents.BuildingOperatorChange, new GEvent.Callback(this.OnBuildingBlockUpdate));
		GEvent.Add(UiEvents.BuildingShopManagerChange, new GEvent.Callback(this.OnBuildingBlockUpdate));
		GEvent.Add(UiEvents.BuildingCustomNameChange, new GEvent.Callback(this.OnBuildingCustomNameChange));
		GEvent.Add(UiEvents.SwitchBuildingManage, new GEvent.Callback(this.OnSwitchBuildingManage));
		GEvent.Add(EEvents.OnTaiwuResourceChange, new GEvent.Callback(this.OnTaiwuResourceChange));
		GEvent.Add(UiEvents.BuildingManageArrangeFocusFinish, new GEvent.Callback(this.BuildingManageArrangeFocusFinish));
		GEvent.Add(UiEvents.BuildingBlockDataChange, new GEvent.Callback(this.OnBuildingBlockDataChange));
		GEvent.Add(UiEvents.SamsaraPlatformRecordDataChange, new GEvent.Callback(this.SamsaraPlatformRecordDataChange));
		GEvent.OnEvent(UiEvents.CloseBuildingManage, EasyPool.Get<ArgumentBox>().Set("isClose", false));
	}

	// Token: 0x060018BC RID: 6332 RVA: 0x0009A4FC File Offset: 0x000986FC
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.BuildingOperatorChange, new GEvent.Callback(this.OnBuildingBlockUpdate));
		GEvent.Remove(UiEvents.BuildingShopManagerChange, new GEvent.Callback(this.OnBuildingBlockUpdate));
		GEvent.Remove(UiEvents.BuildingCustomNameChange, new GEvent.Callback(this.OnBuildingCustomNameChange));
		GEvent.Remove(UiEvents.SwitchBuildingManage, new GEvent.Callback(this.OnSwitchBuildingManage));
		GEvent.Remove(EEvents.OnTaiwuResourceChange, new GEvent.Callback(this.OnTaiwuResourceChange));
		GEvent.Remove(UiEvents.BuildingManageArrangeFocusFinish, new GEvent.Callback(this.BuildingManageArrangeFocusFinish));
		GEvent.Add(UiEvents.BuildingBlockDataChange, new GEvent.Callback(this.OnBuildingBlockDataChange));
		GEvent.Remove(UiEvents.SamsaraPlatformRecordDataChange, new GEvent.Callback(this.SamsaraPlatformRecordDataChange));
		GEvent.OnEvent(UiEvents.CloseBuildingManage, EasyPool.Get<ArgumentBox>().Set("isClose", true));
		GEvent.OnEvent(UiEvents.UpdateAllBlockInfo, null);
		this.ResetData();
		bool isShop = this._configData.IsShop;
		if (isShop)
		{
			GEvent.OnEvent(UiEvents.OnUpdateQuickBtnState, null);
		}
	}

	// Token: 0x060018BD RID: 6333 RVA: 0x0009A628 File Offset: 0x00098828
	private void Update()
	{
		bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) && base.CGet<CButtonObsolete>("ConfirmBtn").interactable && base.CGet<CButtonObsolete>("ConfirmBtn").gameObject.activeSelf;
		if (flag)
		{
			TextMeshProUGUI confirmText = base.CGet<CButtonObsolete>("ConfirmBtn").GetComponent<Refers>().CGet<TextMeshProUGUI>("ConfirmText");
			bool flag2 = confirmText.text == LocalStringManager.Get(LanguageKey.LK_Building_Start_Remove);
			if (flag2)
			{
				this.OnClick(base.CGet<CButtonObsolete>("ConfirmBtn").transform);
			}
		}
	}

	// Token: 0x060018BE RID: 6334 RVA: 0x0009A6CC File Offset: 0x000988CC
	private void OnSwitchBuildingManage(ArgumentBox argsBox)
	{
		this.ResetData();
		argsBox.Get<BuildingBlockData>("BuildingBlockData", out this._blockData);
		argsBox.Get<BuildingAreaData>("BuildingAreaData", out this._areaData);
		this._configData = BuildingBlock.Instance[this._blockData.TemplateId];
		this.PlayOpenSound(this._blockData.TemplateId);
		this._selectingOperatorIndex = -1;
		this._selectingShopManagerIndex = -1;
		base.CGet<GameObject>("ButtonHolderLine").SetActive(false);
		this._blockKey = new BuildingBlockKey(this._areaId, this._blockId, this._blockData.BlockIndex);
		this.DisplayAllResident();
		this.UpdateBuildingData();
		this.InitResourceCollectToggle();
		this.UpdateProgresInfo();
		this._displayInited = false;
		base.CGet<CButtonObsolete>("ConfirmBtn").interactable = false;
		this.UpdateToggles();
		base.RemoveMonitorFieldId(4, 0, (ulong)((long)this._taiwuCharId));
		base.RemoveMonitorFieldId(3, 1, (ulong)this._settlementId);
		base.RemoveMonitorFieldId(19, 192);
		base.RemoveMonitorFieldId(9, 17);
		base.AppendMonitorFieldId(new UIBase.MonitorDataField(4, 0, (ulong)((long)this._taiwuCharId), new uint[]
		{
			59U,
			29U
		}));
		base.AppendMonitorFieldId(new UIBase.MonitorDataField(3, 1, (ulong)this._settlementId, new uint[]
		{
			10U
		}));
		bool flag = this._configData.IsShop || this.IsEntertain;
		if (flag)
		{
			base.AppendMonitorFieldId(new UIBase.MonitorDataField(9, 17, ulong.MaxValue, null));
		}
		BuildingDomainMethod.Call.GetTaiwuVillageResourceBlockEffectInfo(this.Element.GameDataListenerId, this._configData.TemplateId);
		this.ResetScrollBarValue();
		this.ResetSensitiveWordTip();
	}

	// Token: 0x060018BF RID: 6335 RVA: 0x0009A885 File Offset: 0x00098A85
	private void ResetScrollBarValue()
	{
		this._leftInfo.CGet<CScrollbarLegacy>("VerticalScrollbar").value = 0f;
	}

	// Token: 0x060018C0 RID: 6336 RVA: 0x0009A8A3 File Offset: 0x00098AA3
	private void OnTaiwuResourceChange(ArgumentBox argumentBox)
	{
		this.UpdateRepairButton();
	}

	// Token: 0x060018C1 RID: 6337 RVA: 0x0009A8B0 File Offset: 0x00098AB0
	private void ResetData()
	{
		this._shopEventData = null;
		foreach (GameObject btn in this._btnGroup)
		{
			btn.gameObject.SetActive(false);
		}
		foreach (GameObject go in this._animationGoList)
		{
			Object.Destroy(go);
		}
		this._animationGoList.Clear();
	}

	// Token: 0x060018C2 RID: 6338 RVA: 0x0009A968 File Offset: 0x00098B68
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
					bool flag = notification.DomainId == 4;
					if (flag)
					{
						bool flag2 = notification.MethodId == 48;
						if (flag2)
						{
							List<CharacterDisplayData> displayDataList = null;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref displayDataList);
							this._charDisplayDataDict.Clear();
							foreach (CharacterDisplayData data in displayDataList)
							{
								this._charDisplayDataDict.Add(data.CharacterId, data);
							}
							bool flag3 = this._configData.TemplateId == 46;
							if (flag3)
							{
								BuildingDomainMethod.Call.GetCharsInResidence(this.Element.GameDataListenerId, this._blockKey);
							}
							else
							{
								bool flag4 = this._configData.TemplateId == 47;
								if (flag4)
								{
									BuildingDomainMethod.Call.GetCharsInComfortableHouse(this.Element.GameDataListenerId, this._blockKey);
								}
							}
							this.RefreshAllQuickSelectButtons();
							this.Element.ShowAfterRefresh();
						}
						else
						{
							bool flag5 = notification.MethodId == 87 || notification.MethodId == 89;
							if (flag5)
							{
								ValueTuple<int, int> retValue = default(ValueTuple<int, int>);
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref retValue);
								bool flag6 = this._propertyValueDict.ContainsKey(retValue.Item1);
								if (!flag6)
								{
									this._propertyValueDict.Add(retValue.Item1, (short)retValue.Item2);
									bool flag7 = this._propertyValueDict.Count == this._villagerList.Count;
									if (flag7)
									{
										bool flag8 = !this._displayInited;
										if (flag8)
										{
											this.InitDisplay();
										}
										else
										{
											this.UpdateCurrentPage();
										}
									}
								}
							}
							else
							{
								bool flag9 = notification.MethodId == 90;
								if (flag9)
								{
									int[] lifeSkillAttainments = null;
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref lifeSkillAttainments);
									bool flag10 = this._lifeSkillAttainmentDict.Count >= this._villagerList.Count;
									if (flag10)
									{
										this._lifeSkillAttainmentDict.Clear();
									}
									this._lifeSkillAttainmentDict.Add(this._villagerList[this._lifeSkillAttainmentDict.Count], lifeSkillAttainments);
									bool flag11 = this._lifeSkillAttainmentDict.Count == this._villagerList.Count;
									if (flag11)
									{
										this.UpdateShopManagers();
									}
								}
							}
						}
					}
					else
					{
						bool flag12 = notification.DomainId == 9;
						if (flag12)
						{
							bool flag13 = notification.MethodId == 47 || notification.MethodId == 49 || notification.MethodId == 21;
							if (flag13)
							{
								ValueTuple<short, BuildingBlockData> retValue2 = new ValueTuple<short, BuildingBlockData>(0, new BuildingBlockData());
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref retValue2);
								bool flag14 = retValue2.Item1 == this._buildingBlockIndex;
								if (flag14)
								{
									this._blockData = retValue2.Item2;
									this.UpdateBuildingData();
									this.UpdateToggles();
									this.UpdateCurrentPage();
									this.UpdateRepairButton();
									UIElement.BuildingArea.UiBaseAs<ViewBuildingArea>().UpdateBuildingData(this._blockKey, this._blockData, true);
								}
								this.UpdateProgresInfo();
							}
							else
							{
								bool flag15 = notification.MethodId == 48;
								if (flag15)
								{
									ValueTuple<short, BuildingBlockData> retValue3 = new ValueTuple<short, BuildingBlockData>(0, new BuildingBlockData());
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref retValue3);
									bool flag16 = retValue3.Item1 == this._buildingBlockIndex;
									if (flag16)
									{
										this._blockData = retValue3.Item2;
									}
									this.UpdateProgresInfo();
								}
								else
								{
									bool flag17 = notification.MethodId == 51;
									if (flag17)
									{
										ValueTuple<short, BuildingBlockData> retValue4 = new ValueTuple<short, BuildingBlockData>(0, new BuildingBlockData());
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref retValue4);
										bool flag18 = retValue4.Item1 == this._buildingBlockIndex;
										if (flag18)
										{
											this._blockData = retValue4.Item2;
											this.UpdateBuildingData();
											this.UpdateCurrentPage();
											this.UpdateRepairButton();
											UIElement.BuildingArea.UiBaseAs<ViewBuildingArea>().UpdateBuildingData(this._blockKey, this._blockData, true);
										}
									}
									else
									{
										bool flag19 = notification.MethodId == 59 || notification.MethodId == 60;
										if (flag19)
										{
											CharacterList characterList = default(CharacterList);
											Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref characterList);
											this._residents.Clear();
											List<int> collection3 = characterList.GetCollection();
											sbyte level = this._buildingModel.GetBuildingLevel(this._blockKey, this._blockData);
											this._residents.AddRange(collection3);
											this._stringBuilder.Clear();
											this._stringBuilder.Append(LocalStringManager.Get((this._configData.TemplateId == 46) ? LanguageKey.LK_Building_ResidentInfo : LanguageKey.LK_Building_ComfortableInfo2)).Append("  (").Append(collection3.Count).Append("/").Append(BuildingScale.DefValue.ResidenceCapacity.GetLevelEffect((int)level)).Append(")");
											this._residentCount.SetText(this._stringBuilder.ToString(), true);
											int i;
											for (i = 0; i < collection3.Count; i++)
											{
												bool flag20 = this._residentViews.Count <= i;
												if (flag20)
												{
													this.CreateNewResident();
												}
												CharacterDisplayData displayData = this._charDisplayDataDict[collection3[i]];
												this.DisplayResident(displayData, this._residentViews[i], true);
											}
											int capacity2 = BuildingScale.DefValue.ResidenceCapacity.GetLevelEffect((int)level);
											while (i < capacity2)
											{
												bool flag21 = this._residentViews.Count <= i;
												if (flag21)
												{
													this.CreateNewResident();
												}
												this.DisplayResident(null, this._residentViews[i], true);
												i++;
											}
											while (i < this._residentViews.Count)
											{
												this._residentViews[i].gameObject.SetActive(false);
												i++;
											}
											UIElement.BuildingArea.UiBaseAs<ViewBuildingArea>().UpdateResidentHouseInfo(this._blockKey, this._blockData);
										}
										else
										{
											bool flag22 = notification.MethodId == 62 || notification.MethodId == 88;
											if (flag22)
											{
												CharacterList characterList2 = default(CharacterList);
												Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref characterList2);
												sbyte level2 = this._buildingModel.GetBuildingLevel(this._blockKey, this._blockData);
												int capacity = BuildingScale.DefValue.ComfortableHouseCapacity.GetLevelEffect((int)level2);
												this._residents.Clear();
												List<int> collection = characterList2.GetCollection();
												this._residents.AddRange(collection);
												this._stringBuilder.Clear();
												this._stringBuilder.Append(LocalStringManager.Get((this._configData.TemplateId == 46) ? LanguageKey.LK_Building_ResidentInfo : LanguageKey.LK_Building_ComfortableInfo2)).Append("  (").Append(collection.Count).Append("/").Append(capacity).Append(")");
												this._residentCount.SetText(this._stringBuilder.ToString(), true);
												CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(null, collection, delegate(int offset, RawDataPool dataPool)
												{
													List<CharacterDisplayData> data2 = null;
													Serializer.Deserialize(dataPool, offset, ref data2);
													int l;
													for (l = 0; l < collection.Count; l++)
													{
														bool flag54 = this._residentViews.Count <= l;
														if (flag54)
														{
															this.CreateNewResident();
														}
														CharacterDisplayData displayData2 = data2[l];
														this.DisplayResident(displayData2, this._residentViews[l], true);
													}
													while (l < capacity)
													{
														bool flag55 = this._residentViews.Count <= l;
														if (flag55)
														{
															this.CreateNewResident();
														}
														this.DisplayResident(null, this._residentViews[l], true);
														l++;
													}
													while (l < this._residentViews.Count)
													{
														this.DisplayResident(null, this._residentViews[l], false);
														l++;
													}
													UIElement.BuildingArea.UiBaseAs<ViewBuildingArea>().UpdateComfortableHouseInfo(this._blockKey, this._blockData);
												});
											}
											else
											{
												bool flag23 = notification.MethodId == 61;
												if (flag23)
												{
													Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._allResidents);
													this.ShowSelectResidentPanel();
												}
												else
												{
													bool flag24 = notification.MethodId == 68;
													if (flag24)
													{
														Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._blockList);
													}
													else
													{
														bool flag25 = notification.MethodId == 125;
														if (flag25)
														{
															Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._samsaraPlatformRecordCollection);
															this.OnSamsaraCollectionUpdated();
														}
														else
														{
															bool flag26 = notification.MethodId == 160;
															if (flag26)
															{
																int shopProgressBonus = 0;
																Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref shopProgressBonus);
																this._shopProgressBonus = shopProgressBonus;
															}
															else
															{
																bool flag27 = notification.MethodId == 162;
																if (flag27)
																{
																	List<BuildingBlockData> blockList = new List<BuildingBlockData>();
																	Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref blockList);
																	for (int j = 0; j < blockList.Count; j++)
																	{
																		bool flag28 = blockList[j].BlockIndex == this._blockData.BlockIndex;
																		if (flag28)
																		{
																			this._resourceBlockRanking = j;
																			break;
																		}
																	}
																	this.UpdateCalculates();
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
							bool flag29 = notification.DomainId == 5;
							if (flag29)
							{
								bool flag30 = notification.MethodId == 11;
								if (flag30)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._availableWorker);
								}
								else
								{
									bool flag31 = notification.MethodId == 181;
									if (flag31)
									{
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._availableChildren);
									}
									else
									{
										bool flag32 = notification.MethodId == 42;
										if (flag32)
										{
											Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._canTransfer);
											this.FreshCanSoldItemList();
										}
										else
										{
											bool flag33 = notification.MethodId == 55;
											if (flag33)
											{
												Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._canUseBuildingCore);
											}
											else
											{
												bool flag34 = notification.MethodId == 71;
												if (flag34)
												{
													Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._cannotUseInventoryBuildingCore);
												}
												else
												{
													bool flag35 = notification.MethodId == 176;
													if (flag35)
													{
														List<VillagerRoleCharacterDisplayData> villagerRoleCharacterDisplayDataList = new List<VillagerRoleCharacterDisplayData>();
														Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref villagerRoleCharacterDisplayDataList);
														this.HandleVillagerDisplayData(villagerRoleCharacterDisplayDataList);
														this.Element.ShowAfterRefresh();
													}
													else
													{
														bool flag36 = notification.MethodId == 121;
														if (flag36)
														{
															this.UpdateShopManagers();
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
								bool flag37 = notification.DomainId == 19;
								if (flag37)
								{
									bool flag38 = notification.MethodId == 197;
									if (flag38)
									{
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._feast);
										this.RefreshEntertainPage();
										this.RefreshEntertainReward();
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
				bool flag39 = uid.DomainId == 4 && uid.DataId == 0 && (int)uid.SubId0 == this._taiwuCharId;
				if (flag39)
				{
					bool flag40 = uid.SubId1 == 29U;
					if (flag40)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._learnedLifeSkillItems);
						this.GetLockBuildingList();
						this.UpdateLockBuildingList();
					}
					else
					{
						bool flag41 = uid.SubId1 == 59U;
						if (flag41)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._learnedCombatSkillItems);
						}
					}
				}
				else
				{
					bool flag42 = uid.DomainId == 3 && uid.DataId == 1 && (short)uid.SubId0 == this._settlementId && uid.SubId1 == 10U;
					if (flag42)
					{
						OrgMemberCollection collection2 = null;
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref collection2);
						collection2.GetAllMembers(this._villagerList);
						bool flag43 = this._villagerList.Count > 0;
						if (flag43)
						{
							TaiwuDomainMethod.Call.GetAllVillagersAvailableForWork(this.Element.GameDataListenerId);
							TaiwuDomainMethod.Call.GetAllChildAvailableForWork(this.Element.GameDataListenerId);
							CharacterDomainMethod.Call.GetCharacterDisplayDataList(this.Element.GameDataListenerId, this._villagerList);
						}
						else
						{
							this._charDisplayDataDict.Clear();
							bool flag44 = !this._displayInited;
							if (flag44)
							{
								this.InitDisplay();
							}
							else
							{
								this.UpdateCurrentPage();
							}
							this.Element.ShowAfterRefresh();
						}
						this.UpdatePropertyValueData(null);
						bool flag45 = this._configData.TemplateId == 46;
						if (flag45)
						{
							BuildingDomainMethod.Call.GetCharsInResidence(this.Element.GameDataListenerId, this._blockKey);
						}
						else
						{
							bool flag46 = this._configData.TemplateId == 47;
							if (flag46)
							{
								BuildingDomainMethod.Call.GetCharsInComfortableHouse(this.Element.GameDataListenerId, this._blockKey);
							}
							else
							{
								bool isShop = this._configData.IsShop;
								if (isShop)
								{
									this._lifeSkillAttainmentDict.Clear();
									for (int k = 0; k < this._villagerList.Count; k++)
									{
										CharacterDomainMethod.Call.GetAllLifeSkillAttainment(this.Element.GameDataListenerId, this._villagerList[k]);
									}
								}
							}
						}
					}
					else
					{
						bool flag47 = uid.DomainId == 5;
						if (flag47)
						{
							bool flag48 = uid.DataId == 9;
							if (flag48)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._buildingSpaceLimit);
							}
							else
							{
								bool flag49 = uid.DataId == 10;
								if (flag49)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._buildingSpaceCurr);
								}
							}
						}
						else
						{
							bool flag50 = uid.DomainId == 9;
							if (flag50)
							{
								bool flag51 = uid.DataId == 17;
								if (flag51)
								{
									Dictionary<BuildingBlockKey, BuildingEarningsData> earningsDataDict = new Dictionary<BuildingBlockKey, BuildingEarningsData>();
									Serializer.DeserializeModifications<BuildingBlockKey>(wrapper.DataPool, notification.ValueOffset, earningsDataDict);
									this.RefreshEarnings(earningsDataDict);
									bool isEntertain = this.IsEntertain;
									if (isEntertain)
									{
										ExtraDomainMethod.Call.GetFeast(this.Element.GameDataListenerId, this._blockKey);
									}
								}
							}
							else
							{
								bool flag52 = uid.DomainId == 19 && uid.DataId == 56;
								if (flag52)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._unlockWorkingList);
									ResidentView.SetUnlockedWorkingList(this._unlockWorkingList);
									this.RefreshAllQuickSelectButtons();
								}
								else
								{
									bool flag53 = uid.DomainId == 19 && uid.DataId == 192;
									if (flag53)
									{
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._unlockedFeastTypes);
										this._unlockedFeastTypes.Remove(0);
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060018C3 RID: 6339 RVA: 0x0009B874 File Offset: 0x00099A74
	private void RefreshEarnings(Dictionary<BuildingBlockKey, BuildingEarningsData> earningsDataDict)
	{
		bool isRecruit = this._shopEventData != null && this._shopEventData.RecruitPeopleProb.Count > 0;
		this._shopInfoPage.CGet<GameObject>("RecruitPeople").SetActive(isRecruit);
		earningsDataDict.TryGetValue(this._blockKey, out this._earningsData);
		int slotCount = (int)GameData.Domains.Building.SharedMethods.GetBuildingSlotCount(this._configData.TemplateId);
		bool flag = this.UpdateShopOutputInfo(slotCount);
		if (flag)
		{
			CButtonObsolete quickBtn = this._shopInfoPage.CGet<CButtonObsolete>("QuickCollectItemBtn");
			bool flag2 = earningsDataDict.ContainsKey(this._blockKey);
			if (flag2)
			{
				quickBtn.interactable = true;
				int totalCost = 0;
				List<ItemKey> collectItemList = this._earningsData.CollectionItemList;
				List<IntPair> collectResourceList = this._earningsData.CollectionResourceList;
				GameObject itemResourceButton = this._shopInfoPage.CGet<GameObject>("ItemResourceButton");
				for (int i = 0; i < slotCount; i++)
				{
					Refers currentItemRefers = this._reuseDic[itemResourceButton.name][i].GetComponent<Refers>();
					bool isExist = i < collectItemList.Count;
					currentItemRefers.CGet<GameObject>("Stay").SetActive(isExist);
					bool flag3 = isExist;
					if (flag3)
					{
						ItemKey itemKey = collectItemList[i];
						bool flag4 = this._shopEventData.ItemGradeProbList.Count > 0;
						if (flag4)
						{
							totalCost += ItemTemplateHelper.GetBaseValue(itemKey.ItemType, itemKey.TemplateId);
							currentItemRefers.CGet<TextMeshProUGUI>("StayText").text = (GameData.Domains.Building.SharedMethods.GetBuildingEarnPreserveTime(this._blockTemplateId) - (int)collectItemList[i].ModificationState).ToString();
							int index = i;
							Action <>9__1;
							ItemDomainMethod.AsyncCall.GetItemDisplayData(this, itemKey, this._taiwuCharId, delegate(int offset, RawDataPool dataPool)
							{
								ItemDisplayData itemDisplayData3 = null;
								Serializer.Deserialize(dataPool, offset, ref itemDisplayData3);
								bool flag19 = this.IsMoneyEnoughBuy(itemKey);
								if (flag19)
								{
									ItemResourceButton component = currentItemRefers.GetComponent<ItemResourceButton>();
									ItemDisplayData itemDisplayData4 = itemDisplayData3;
									ItemResourceButton.ItemResourceButtonState btnState = ItemResourceButton.ItemResourceButtonState.Reveive;
									Action add = null;
									Action change = null;
									Action receive;
									if ((receive = <>9__1) == null)
									{
										receive = (<>9__1 = delegate()
										{
											BuildingDomainMethod.Call.AcceptBuildingBlockCollectEarning(this.Element.GameDataListenerId, this._blockKey, index, false, true, true);
											this.ShowGetItemAnimation();
										});
									}
									component.SetButtonFunc(itemDisplayData4, btnState, add, change, receive);
								}
								else
								{
									currentItemRefers.GetComponent<ItemResourceButton>().SetButtonFunc(itemDisplayData3, ItemResourceButton.ItemResourceButtonState.LackOfMoney, null, null, null);
								}
							});
						}
						else
						{
							currentItemRefers.CGet<GameObject>("Stay").SetActive(false);
							int index = i;
							Action <>9__3;
							ItemDomainMethod.AsyncCall.GetItemDisplayData(this, itemKey, this._taiwuCharId, delegate(int offset, RawDataPool dataPool)
							{
								ItemDisplayData itemDisplayData3 = null;
								Serializer.Deserialize(dataPool, offset, ref itemDisplayData3);
								ItemResourceButton component = currentItemRefers.GetComponent<ItemResourceButton>();
								ItemDisplayData itemDisplayData4 = itemDisplayData3;
								ItemResourceButton.ItemResourceButtonState btnState = ItemResourceButton.ItemResourceButtonState.Reveive;
								Action add = null;
								Action change = null;
								Action receive;
								if ((receive = <>9__3) == null)
								{
									receive = (<>9__3 = delegate()
									{
										BuildingDomainMethod.Call.AcceptBuildingBlockCollectEarning(this.Element.GameDataListenerId, this._blockKey, index, false);
										this.ShowGetItemAnimation();
									});
								}
								component.SetButtonFunc(itemDisplayData4, btnState, add, change, receive);
							});
						}
					}
					else
					{
						bool flag5 = i < collectItemList.Count + collectResourceList.Count;
						if (flag5)
						{
							int index = i;
							IntPair resInfo = collectResourceList[index - collectItemList.Count];
							currentItemRefers.GetComponent<ItemResourceButton>().SetResourceFunc((sbyte)resInfo.First, resInfo.Second, ItemResourceButton.ItemResourceButtonState.Reveive, null, null, delegate
							{
								BuildingDomainMethod.Call.AcceptBuildingBlockCollectEarning(this.Element.GameDataListenerId, this._blockKey, index, false);
								this.ShowGetItemAnimation();
							});
						}
						else
						{
							ItemDisplayData itemDisplayData = new ItemDisplayData();
							currentItemRefers.GetComponent<ItemResourceButton>().SetButtonFunc(itemDisplayData, ItemResourceButton.ItemResourceButtonState.None, null, null, null);
						}
					}
				}
				bool flag6 = totalCost > 0;
				if (flag6)
				{
					quickBtn.interactable = this.IsMoneyEnoughBuy(totalCost);
					TooltipInvoker quickBtnTip = quickBtn.GetComponent<TooltipInvoker>();
					bool interactable = quickBtn.interactable;
					if (interactable)
					{
						quickBtnTip.enabled = false;
					}
					else
					{
						quickBtnTip.enabled = true;
						quickBtnTip.PresetParam[0] = LocalStringManager.Get("LK_Building_LockOfResource");
					}
				}
				else
				{
					quickBtn.GetComponent<TooltipInvoker>().PresetParam[0] = LocalStringManager.Get("LK_Building_AutoCollectTip");
				}
			}
			else
			{
				quickBtn.interactable = false;
				GameObject itemResourceButton2 = this._shopInfoPage.CGet<GameObject>("ItemResourceButton");
				for (int j = 0; j < slotCount; j++)
				{
					Refers currentItemRefers5 = this._reuseDic[itemResourceButton2.name][j].GetComponent<Refers>();
					ItemDisplayData itemDisplayData2 = new ItemDisplayData();
					currentItemRefers5.CGet<GameObject>("Stay").SetActive(false);
					currentItemRefers5.GetComponent<ItemResourceButton>().SetButtonFunc(itemDisplayData2, ItemResourceButton.ItemResourceButtonState.None, null, null, null);
				}
			}
		}
		GameObject recruitPeopleButton = this._shopInfoPage.CGet<GameObject>("RecruitPeopleButton");
		bool flag7 = this.UpdateShopRecruitInfo(earningsDataDict, slotCount) && isRecruit;
		if (flag7)
		{
			List<IntPair> recruitLevelList = this._earningsData.RecruitLevelList;
			bool flag8 = this._configData.TemplateId == 223;
			if (flag8)
			{
				CButtonObsolete quickBtn2 = this._shopInfoPage.CGet<CButtonObsolete>("QuickRecruitPeopleBtn");
				TooltipInvoker mouseTip = quickBtn2.GetComponent<TooltipInvoker>();
				bool flag9 = this.IsMoneyEnoughRecruit(recruitLevelList.Count);
				if (flag9)
				{
					quickBtn2.interactable = true;
					mouseTip.PresetParam[0] = LocalStringManager.Get("LK_Building_AutoRecruit");
				}
				else
				{
					quickBtn2.interactable = false;
					mouseTip.PresetParam[0] = LocalStringManager.Get("LK_Building_LockOfResource");
				}
			}
			for (int k = 0; k < slotCount; k++)
			{
				Refers currentItemRefers2 = this._reuseDic[recruitPeopleButton.name][k].GetComponent<Refers>();
				bool isExist2 = k < recruitLevelList.Count;
				TooltipInvoker tipsDp = currentItemRefers2.GetComponentInChildren<TooltipInvoker>();
				tipsDp.enabled = false;
				currentItemRefers2.CGet<CImage>("Normal").gameObject.SetActive(isExist2);
				currentItemRefers2.CGet<GameObject>("Stay").SetActive(isExist2);
				bool flag10 = isExist2;
				if (flag10)
				{
					Refers peopleRefers = currentItemRefers2.CGet<CImage>("HasPeopleImg").GetComponent<Refers>();
					Refers buttonRefers = currentItemRefers2.CGet<GameObject>("BtnHolder").GetComponent<Refers>();
					CButtonObsolete rejectButton = buttonRefers.CGet<CButtonObsolete>("RejectOperateBtn");
					int remainTime = GameData.Domains.Building.SharedMethods.GetBuildingEarnPreserveTime(this._blockTemplateId) - recruitLevelList[k].Second;
					currentItemRefers2.CGet<TextMeshProUGUI>("StayText").text = remainTime.ToString();
					ExtraDomainMethod.AsyncCall.RequestRecruitCharacterData(this, this._blockKey, k, delegate(int offset, RawDataPool pool)
					{
						RecruitCharacterData recruitCharacterData = null;
						Serializer.Deserialize(pool, offset, ref recruitCharacterData);
						bool flag19 = recruitCharacterData != null;
						if (flag19)
						{
							tipsDp.enabled = true;
							tipsDp.RuntimeParam = new ArgumentBox();
							tipsDp.RuntimeParam.Set("RemainTime", remainTime);
							tipsDp.RuntimeParam.SetObject("Data", new CharacterDisplayDataForTooltip(recruitCharacterData));
							ValueTuple<string, string> name = recruitCharacterData.FullName.GetName(recruitCharacterData.Gender, SingletonObject.getInstance<BasicGameData>().CustomTexts);
							string surname = name.Item1;
							string givenName = name.Item2;
							peopleRefers.CGet<Game.Components.Avatar.Avatar>("Avatar").Refresh(recruitCharacterData.GenerateAvatarRelatedData());
							peopleRefers.CGet<TextMeshProUGUI>("Name").text = surname + givenName;
						}
					});
					int index = k;
					Action <>9__9;
					rejectButton.ClearAndAddListener(delegate
					{
						UIElement dialog = UIElement.Dialog;
						ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
						string key = "Cmd";
						DialogCmd dialogCmd = new DialogCmd();
						dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_Building_RejectRecruitPeople_Cmd_Title).ColorReplace();
						dialogCmd.Content = LocalStringManager.Get(LanguageKey.LK_Building_RejectRecruitPeople_Cmd_Text).ColorReplace();
						dialogCmd.Type = 1;
						Action yes;
						if ((yes = <>9__9) == null)
						{
							yes = (<>9__9 = delegate()
							{
								BuildingDomainMethod.Call.RejectBuildingBlockRecruitPeople(this._blockKey, index);
							});
						}
						dialogCmd.Yes = yes;
						dialog.SetOnInitArgs(argumentBox.SetObject(key, dialogCmd));
						UIManager.Instance.MaskUI(UIElement.Dialog);
					});
					bool flag11 = this._configData.TemplateId == 223;
					if (flag11)
					{
						bool flag12 = this.IsMoneyEnoughRecruit(1);
						if (flag12)
						{
							currentItemRefers2.GetComponent<ItemResourceButton>().SetRecruitFunc(ItemResourceButton.ItemResourceButtonState.Reveive, null, null, delegate
							{
								BuildingDomainMethod.AsyncCall.AcceptBuildingBlockRecruitPeople(this, this._blockKey, index, delegate(int offset, RawDataPool dataPool)
								{
									int charId = -1;
									Serializer.Deserialize(dataPool, offset, ref charId);
									bool flag19 = charId != -1;
									if (flag19)
									{
										List<int> charIdList = new List<int>
										{
											charId
										};
										this.ShowGetPeopleView(charIdList);
									}
								});
							});
						}
						else
						{
							currentItemRefers2.GetComponent<ItemResourceButton>().SetRecruitFunc(ItemResourceButton.ItemResourceButtonState.LackOfMoney, null, null, null);
						}
					}
					else
					{
						currentItemRefers2.GetComponent<ItemResourceButton>().SetRecruitFunc(ItemResourceButton.ItemResourceButtonState.Reveive, null, null, delegate
						{
							BuildingDomainMethod.AsyncCall.AcceptBuildingBlockRecruitPeople(this, this._blockKey, index, delegate(int offset, RawDataPool dataPool)
							{
								int charId = -1;
								Serializer.Deserialize(dataPool, offset, ref charId);
								bool flag19 = charId != -1;
								if (flag19)
								{
									List<int> charIdList = new List<int>
									{
										charId
									};
									this.ShowGetPeopleView(charIdList);
								}
							});
						});
					}
				}
				else
				{
					currentItemRefers2.GetComponent<ItemResourceButton>().SetRecruitFunc(ItemResourceButton.ItemResourceButtonState.None, null, null, null);
				}
			}
		}
		else
		{
			this._shopInfoPage.CGet<CButtonObsolete>("QuickRecruitPeopleBtn").interactable = false;
			for (int l = 0; l < slotCount; l++)
			{
				Refers currentItemRefers3 = this._reuseDic[recruitPeopleButton.name][l].GetComponent<Refers>();
				TooltipInvoker tipsDp2 = currentItemRefers3.GetComponentInChildren<TooltipInvoker>();
				tipsDp2.enabled = false;
				currentItemRefers3.CGet<CImage>("Normal").gameObject.SetActive(false);
				currentItemRefers3.CGet<GameObject>("Stay").SetActive(false);
				currentItemRefers3.GetComponent<ItemResourceButton>().SetRecruitFunc(ItemResourceButton.ItemResourceButtonState.None, null, null, null);
			}
		}
		bool flag13 = this.UpdateShopSoldItemInfo(earningsDataDict, slotCount);
		if (flag13)
		{
			GameObject soldItemButton = this._shopInfoPage.CGet<GameObject>("SoldItemButton");
			bool flag14 = earningsDataDict.ContainsKey(this._blockKey);
			if (flag14)
			{
				for (int m = 0; m < slotCount; m++)
				{
					Refers currentItemRefers = this._reuseDic[soldItemButton.name][m].GetComponent<Refers>();
					int index = m;
					bool flag15 = m < this._earningsData.ShopSoldItemList.Count && this._earningsData.ShopSoldItemList[m].TemplateId != -1;
					if (flag15)
					{
						ItemDomainMethod.AsyncCall.GetItemDisplayData(this, this._earningsData.ShopSoldItemList[m], this._taiwuCharId, delegate(int offset, RawDataPool dataPool)
						{
							ItemDisplayData displayData = null;
							Serializer.Deserialize(dataPool, offset, ref displayData);
							currentItemRefers.GetComponent<ItemResourceButton>().SetSoldItemFunc(displayData, new IntPair(-1, -1), ItemResourceButton.ItemResourceButtonState.Change, null, delegate
							{
								this.OpenMultiSelectItemWindow();
							}, null);
						});
					}
					else
					{
						bool flag16 = m < this._earningsData.ShopSoldItemList.Count && this._earningsData.ShopSoldItemEarnList[m].First != -1;
						if (flag16)
						{
							currentItemRefers.GetComponent<ItemResourceButton>().SetSoldItemFunc(null, this._earningsData.ShopSoldItemEarnList[m], ItemResourceButton.ItemResourceButtonState.Reveive, null, null, delegate
							{
								BuildingDomainMethod.Call.ShopBuildingSoldItemReceive(this._blockKey, index);
								this.ShowGetItemAnimation();
							});
						}
						else
						{
							bool flag17 = m >= this._earningsData.ShopSoldItemList.Count || (this._earningsData.ShopSoldItemList[m].TemplateId == -1 && this._earningsData.ShopSoldItemEarnList[m].First == -1);
							if (flag17)
							{
								currentItemRefers.GetComponent<ItemResourceButton>().SetSoldItemFunc(null, new IntPair(-1, -1), ItemResourceButton.ItemResourceButtonState.Add, delegate
								{
									this.OpenMultiSelectItemWindow();
								}, null, null);
							}
						}
					}
				}
			}
			else
			{
				for (int n = 0; n < slotCount; n++)
				{
					Refers currentItemRefers4 = this._reuseDic[soldItemButton.name][n].GetComponent<Refers>();
					currentItemRefers4.GetComponent<ItemResourceButton>().SetSoldItemFunc(null, new IntPair(-1, -1), ItemResourceButton.ItemResourceButtonState.Add, delegate
					{
						this.OpenMultiSelectItemWindow();
					}, null, null);
				}
			}
		}
		this.UpdateProductRequireTip();
		bool isFixBookManage = this.IsFixBookManage();
		this._shopInfoPage.CGet<Refers>("FixBookManageInfo").gameObject.SetActive(isFixBookManage);
		this.UpdateShopManageInfoActive();
		bool flag18 = isFixBookManage;
		if (flag18)
		{
			this._bookCollectionData = earningsDataDict;
			this.UpdateBookCollectionRoomInfo(this._bookCollectionData);
		}
		UIElement.BuildingArea.UiBaseAs<ViewBuildingArea>().UpdateShopGetItemInfo(this._blockKey);
	}

	// Token: 0x060018C4 RID: 6340 RVA: 0x0009C2F0 File Offset: 0x0009A4F0
	private bool IsFixBookManage()
	{
		return this._blockData.TemplateId == 105;
	}

	// Token: 0x060018C5 RID: 6341 RVA: 0x0009C314 File Offset: 0x0009A514
	private bool IsNormalManage()
	{
		return GameData.Domains.Building.SharedMethods.BuildingIsShopWithEvent(this._configData);
	}

	// Token: 0x060018C6 RID: 6342 RVA: 0x0009C331 File Offset: 0x0009A531
	private void UpdateShopManageInfoActive()
	{
		this._shopInfoPage.CGet<GameObject>("ShopInfoArea").SetActive(this.IsFixBookManage() || this.IsNormalManage());
	}

	// Token: 0x060018C7 RID: 6343 RVA: 0x0009C35C File Offset: 0x0009A55C
	private void UpdateProductRequireTip()
	{
		bool isNormalManage = this.IsNormalManage();
		bool isFixBookManage = this.IsFixBookManage();
		GameObject normalManageInfo = this._shopInfoPage.CGet<GameObject>("NormalManageInfo");
		normalManageInfo.SetActive(isNormalManage);
		this.UpdateShopManageInfoActive();
		bool flag = !isNormalManage;
		if (!flag)
		{
			CImage requiredIcon = this._shopInfoPage.CGet<CImage>("RequireIcon");
			Refers productRefers = this._shopInfoPage.CGet<Refers>("ProductionInfoArea");
			requiredIcon.gameObject.SetActive(GameData.Domains.Building.SharedMethods.BuildingRequireSafetyOrCulture(this._configData));
			productRefers.gameObject.SetActive(this._configData.IsCollectResourceBuilding || GameData.Domains.Building.SharedMethods.IsBuildingProduceMoneyAuthority(this._configData, this._shopEventData) || GameData.Domains.Building.SharedMethods.IsBuildingSoldItem(this._configData, this._shopEventData));
			GameObject productTextHolder = productRefers.CGet<GameObject>("ProductTextHolder");
			TooltipInvoker produceMouseTip = productRefers.CGet<TooltipInvoker>("MouseTip");
			TooltipInvoker requireMouseTip = requiredIcon.GetComponent<TooltipInvoker>();
			bool mouseTipDisable = this.GetShopManagerCount() == 0;
			produceMouseTip.enabled = !mouseTipDisable;
			requireMouseTip.enabled = !mouseTipDisable;
			productTextHolder.SetActive(!mouseTipDisable);
			requireMouseTip.GetComponent<DisableStyleRoot>().SetStyleEffect(mouseTipDisable, false);
			bool flag2 = mouseTipDisable;
			if (!flag2)
			{
				BuildingDomainMethod.AsyncCall.GetShopManagementYieldTipsData(this, this._blockKey, delegate(int offset, RawDataPool pool)
				{
					BuildingManageYieldTipsData tipsData = default(BuildingManageYieldTipsData);
					Serializer.Deserialize(pool, offset, ref tipsData);
					bool activeSelf = productRefers.gameObject.activeSelf;
					if (activeSelf)
					{
						TextMeshProUGUI productText = productRefers.CGet<TextMeshProUGUI>("ProductText");
						bool flag3 = GameData.Domains.Building.SharedMethods.IsBuildingProduceMoneyAuthority(this._configData, this._shopEventData) || GameData.Domains.Building.SharedMethods.IsBuildingSoldItem(this._configData, this._shopEventData);
						if (flag3)
						{
							produceMouseTip.Type = TipType.BuildingProduce;
							produceMouseTip.RuntimeParam = new ArgumentBox().SetObject("ProduceData", tipsData);
							productText.text = MouseTipBuildingProduce.CalcProduct(tipsData, GameData.Domains.Building.SharedMethods.IsBuildingProduceMoneyAuthority(this._configData, this._shopEventData));
						}
						else
						{
							bool isCollectResourceBuilding = this._configData.IsCollectResourceBuilding;
							if (isCollectResourceBuilding)
							{
								produceMouseTip.Type = TipType.BuildingProduceCollectResource;
								produceMouseTip.RuntimeParam = new ArgumentBox().SetObject("ProduceData", tipsData);
								productText.text = tipsData.ResourceOutputValuation.ToString();
							}
						}
					}
					bool activeSelf2 = requiredIcon.gameObject.activeSelf;
					if (activeSelf2)
					{
						this.UpdateRequiredTip(requiredIcon, tipsData);
					}
				});
			}
		}
	}

	// Token: 0x060018C8 RID: 6344 RVA: 0x0009C4E0 File Offset: 0x0009A6E0
	private void UpdateRequiredTip(CImage requiredIcon, BuildingManageYieldTipsData tipsData)
	{
		TooltipInvoker requiredTips = requiredIcon.GetComponent<TooltipInvoker>();
		string postfix;
		bool flag;
		bool flag2;
		MouseTipBuildingRequireCultureSafety.CalcIconName(this._configData, out postfix, out flag, out flag2);
		requiredIcon.SetSprite("building_effect_" + postfix, false, null);
		bool flag3 = tipsData.SafetyOrCultureFactorSettlementsAndPickValue == null || tipsData.SafetyOrCultureFactorSettlementsAndPickValue.Count == 0;
		if (flag3)
		{
			requiredTips.Type = TipType.SingleDesc;
			requiredTips.RuntimeParam = new ArgumentBox();
			requiredTips.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Building_Effect_Tips_Text_5));
		}
		else
		{
			requiredTips.Type = TipType.BuildingRequireCultureSafety;
			requiredTips.RuntimeParam = new ArgumentBox();
			requiredTips.RuntimeParam.Set("TemplateId", this._configData.TemplateId);
			requiredTips.RuntimeParam.Set<BuildingManageYieldTipsData>("ProduceData", tipsData);
		}
	}

	// Token: 0x060018C9 RID: 6345 RVA: 0x0009C5B0 File Offset: 0x0009A7B0
	private void UpdateBookCollectionRoomInfo(Dictionary<BuildingBlockKey, BuildingEarningsData> earning)
	{
		bool flag = earning == null;
		if (!flag)
		{
			Refers fixBookManageInfo = this._shopInfoPage.CGet<Refers>("FixBookManageInfo");
			ItemResourceButton fixBookBtn = fixBookManageInfo.CGet<ItemResourceButton>("ItemResourceButton");
			GameObject progressBar = fixBookManageInfo.CGet<GameObject>("ProgressBar");
			TextMeshProUGUI progressText = fixBookManageInfo.CGet<TextMeshProUGUI>("ProgressText");
			CImage progressImgFill = fixBookManageInfo.CGet<CImage>("ProgressImgFill");
			Refers bookInfo = fixBookManageInfo.CGet<Refers>("BookInfo");
			GameObject stateRoot = fixBookManageInfo.CGet<GameObject>("StateRoot");
			TextMeshProUGUI stateText = fixBookManageInfo.CGet<TextMeshProUGUI>("StateText");
			BuildingEarningsData data;
			bool flag2 = earning.TryGetValue(this._blockKey, out data) && data != null && data.FixBookInfoList.Count > 0 && data.FixBookInfoList[0].IsValid();
			if (flag2)
			{
				ItemDisplayData itemDisplayData = new ItemDisplayData
				{
					Key = data.FixBookInfoList[0]
				};
				Action <>9__3;
				AsyncMethodCallbackDelegate <>9__2;
				Action <>9__4;
				AsyncMethodCallbackDelegate <>9__1;
				ItemDomainMethod.AsyncCall.GetSkillBookPagesInfo(this, itemDisplayData.Key, delegate(int offset, RawDataPool dataPool)
				{
					SkillBookPageDisplayData displayData = null;
					Serializer.Deserialize(dataPool, offset, ref displayData);
					bool canFix = displayData.CanFix();
					int needProgress = (int)displayData.GetFixProgress().Item2;
					SingletonObject.getInstance<BasicGameData>().ChallengeModeData.ApplyChallengeModeBuildingWorkHard(ref needProgress);
					int curProgress = Math.Min(needProgress, (int)this._blockData.ShopProgress);
					progressBar.gameObject.SetActive(canFix);
					progressImgFill.fillAmount = (float)curProgress / (float)needProgress;
					bookInfo.gameObject.SetActive(canFix);
					stateRoot.SetActive(!canFix);
					bool flag3 = canFix;
					if (flag3)
					{
						List<CImage> oldInfo = bookInfo.CGetList<CImage>("OldPageInfo");
						List<CImage> newInfo = bookInfo.CGetList<CImage>("NewPageInfo");
						oldInfo[oldInfo.Count - 1].transform.parent.gameObject.SetActive(displayData.State.Length == oldInfo.Count);
						newInfo[oldInfo.Count - 1].transform.parent.gameObject.SetActive(displayData.State.Length == newInfo.Count);
						int index = 0;
						for (int i = displayData.State.Length - 1; i >= 0; i--)
						{
							bool flag4 = displayData.State[i] == 0;
							if (flag4)
							{
								oldInfo[i].SetSprite("sp_shuji_icon_0", false, null);
							}
							bool flag5 = displayData.State[i] == 1;
							if (flag5)
							{
								oldInfo[i].SetSprite("sp_shuji_icon_1", false, null);
								index = i;
							}
							bool flag6 = displayData.State[i] == 2;
							if (flag6)
							{
								oldInfo[i].SetSprite("sp_shuji_icon_2", false, null);
								index = i;
							}
						}
						for (int j = displayData.State.Length - 1; j >= 0; j--)
						{
							bool flag7 = displayData.State[j] == 0;
							if (flag7)
							{
								newInfo[j].SetSprite("sp_shuji_icon_0", false, null);
							}
							bool flag8 = displayData.State[j] == 1;
							if (flag8)
							{
								newInfo[j].SetSprite("sp_shuji_icon_1", false, null);
							}
							bool flag9 = displayData.State[j] == 2;
							if (flag9)
							{
								newInfo[j].SetSprite("sp_shuji_icon_2", false, null);
							}
						}
						newInfo[index].SetSprite("sp_shuji_icon_0", false, null);
						progressText.text = UI_BuildingManage.GetPredictProgressText(curProgress, this.ShopManageProgressDelta, needProgress);
						IAsyncMethodRequestHandler <>4__this = this;
						ItemKey key = itemDisplayData.Key;
						int taiwuCharId = this._taiwuCharId;
						AsyncMethodCallbackDelegate callback;
						if ((callback = <>9__2) == null)
						{
							callback = (<>9__2 = delegate(int offset2, RawDataPool dataPool2)
							{
								ItemDisplayData displayData2 = null;
								Serializer.Deserialize(dataPool2, offset2, ref displayData2);
								ItemResourceButton fixBookBtn = fixBookBtn;
								ItemDisplayData itemDisplayData = displayData2;
								ItemResourceButton.ItemResourceButtonState btnState = ItemResourceButton.ItemResourceButtonState.Change;
								Action add = null;
								Action change;
								if ((change = <>9__3) == null)
								{
									change = (<>9__3 = delegate()
									{
										DialogCmd cmd = new DialogCmd
										{
											Title = LocalStringManager.Get(LanguageKey.LK_Building_ChangeBook),
											Content = LocalStringManager.Get(LanguageKey.LK_Building_ChangeBookTip),
											Yes = new Action(this.OpenMultiSelectItemWindow)
										};
										UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
										UIManager.Instance.MaskUI(UIElement.Dialog);
									});
								}
								fixBookBtn.SetFixBookFunc(itemDisplayData, btnState, add, change, null);
							});
						}
						ItemDomainMethod.AsyncCall.GetItemDisplayData(<>4__this, key, taiwuCharId, callback);
					}
					else
					{
						stateText.SetText(LocalStringManager.Get(LanguageKey.LK_Building_AccomplishFix), true);
						IAsyncMethodRequestHandler <>4__this2 = this;
						ItemKey key2 = itemDisplayData.Key;
						int taiwuCharId2 = this._taiwuCharId;
						AsyncMethodCallbackDelegate callback2;
						if ((callback2 = <>9__1) == null)
						{
							callback2 = (<>9__1 = delegate(int offset2, RawDataPool dataPool2)
							{
								ItemDisplayData displayData2 = null;
								Serializer.Deserialize(dataPool2, offset2, ref displayData2);
								ItemResourceButton fixBookBtn = fixBookBtn;
								ItemDisplayData itemDisplayData = displayData2;
								ItemResourceButton.ItemResourceButtonState btnState = ItemResourceButton.ItemResourceButtonState.Reveive;
								Action add = null;
								Action change = null;
								Action receive;
								if ((receive = <>9__4) == null)
								{
									receive = (<>9__4 = delegate()
									{
										bool isTaiwuOnSettlement = SingletonObject.getInstance<WorldMapModel>().IsTaiwuOnSettlement;
										BuildingDomainMethod.Call.ReceiveFixBook(this._blockKey, isTaiwuOnSettlement);
									});
								}
								fixBookBtn.SetFixBookFunc(itemDisplayData, btnState, add, change, receive);
							});
						}
						ItemDomainMethod.AsyncCall.GetItemDisplayData(<>4__this2, key2, taiwuCharId2, callback2);
					}
				});
			}
			else
			{
				progressBar.gameObject.SetActive(false);
				bookInfo.gameObject.SetActive(false);
				stateRoot.SetActive(true);
				stateText.SetText(LocalStringManager.Get(LanguageKey.LK_Building_FixBook_None), true);
				fixBookBtn.SetFixBookFunc(null, ItemResourceButton.ItemResourceButtonState.Add, new Action(this.OpenMultiSelectItemWindow), null, null);
			}
		}
	}

	// Token: 0x060018CA RID: 6346 RVA: 0x0009C758 File Offset: 0x0009A958
	private bool IsMoneyEnoughBuy(ItemKey itemKey)
	{
		return this._buildingModel.GetResourceCount(6) >= ItemTemplateHelper.GetBaseValue(itemKey.ItemType, itemKey.TemplateId);
	}

	// Token: 0x060018CB RID: 6347 RVA: 0x0009C78C File Offset: 0x0009A98C
	private bool IsMoneyEnoughBuy(int cost)
	{
		return this._buildingModel.GetResourceCount(6) >= cost;
	}

	// Token: 0x060018CC RID: 6348 RVA: 0x0009C7B0 File Offset: 0x0009A9B0
	private bool IsMoneyEnoughRecruit(int count = 1)
	{
		return this._buildingModel.GetResourceCount(7) >= (int)GlobalConfig.Instance.RecruitPeopleCost * count;
	}

	// Token: 0x060018CD RID: 6349 RVA: 0x0009C7E0 File Offset: 0x0009A9E0
	private void OpenMultiSelectItemWindow()
	{
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("canTransfer", this._canTransfer);
		argBox.Set("BuildingTemplateId", this._configData.TemplateId);
		argBox.SetObject("buildingBlockKey", this._blockKey);
		argBox.SetObject("buildingEarningsData", this._earningsData);
		argBox.SetObject("warehouseList", this._warehouseCanSoldItemList);
		argBox.SetObject("inventoryList", this._inventoryCanSoldItemList);
		argBox.SetObject("treasuryList", this._treasuryCanSoldItemList);
		argBox.SetObject("callback", new Action(this.<OpenMultiSelectItemWindow>g__FreshAction|141_0));
		UIElement.MultiSelectItem.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.MultiSelectItem, true);
	}

	// Token: 0x060018CE RID: 6350 RVA: 0x0009C8B4 File Offset: 0x0009AAB4
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		string text = btnName;
		string text2 = text;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(text2);
		if (num <= 2839120067U)
		{
			if (num <= 881705813U)
			{
				if (num <= 591905656U)
				{
					if (num != 465096593U)
					{
						if (num == 591905656U)
						{
							if (text2 == "QuickSoldCollectBtn")
							{
								bool tmp = false;
								bool flag = this._earningsData == null;
								if (!flag)
								{
									foreach (IntPair pair in this._earningsData.ShopSoldItemEarnList)
									{
										bool flag2 = pair.First != -1;
										if (flag2)
										{
											tmp = true;
										}
									}
									bool flag3 = !tmp;
									if (!flag3)
									{
										BuildingDomainMethod.Call.ShopBuildingSoldItemReceiveQuick(this._blockKey);
										this.ShowGetItemAnimation();
									}
								}
							}
						}
					}
					else if (text2 == "ResidentViewQuickBtn")
					{
						bool flag4 = this._blockData.TemplateId == 47;
						if (flag4)
						{
							BuildingDomainMethod.Call.QuickFillComfortableHouse(this.Element.GameDataListenerId, this._blockKey);
						}
						else
						{
							bool flag5 = this._blockData.TemplateId == 46;
							if (flag5)
							{
								BuildingDomainMethod.Call.QuickFillResidence(this.Element.GameDataListenerId, this._blockKey);
							}
						}
					}
				}
				else
				{
					if (num != 683528073U)
					{
						if (num != 881705813U)
						{
							return;
						}
						if (!(text2 == "ConfirmBtn"))
						{
							return;
						}
					}
					else if (!(text2 == "CancelBtn"))
					{
						return;
					}
					bool bConfirm = btnName == "ConfirmBtn";
					sbyte operationType = this.TogKeyToOperationType(this._mainToggleGroup.GetActive().Key);
					Action action = bConfirm ? (this._blockData.OperationStopping ? new Action(this.ContinueOperation) : new Action(this.ConfirmOperation)) : new Action(this.CancelOperation);
					bool isUsefulResource = BuildingBlockData.IsUsefulResource(this._configData.Type);
					bool flag6 = isUsefulResource && !bConfirm && operationType == 0;
					if (flag6)
					{
						DialogCmd cmd = new DialogCmd
						{
							Title = LocalStringManager.Get(LanguageKey.LK_Building_Stop_Build),
							Content = LocalStringManager.Get(LanguageKey.LK_Building_Stop_Build_Tip_Resource),
							Type = 1,
							Yes = action
						};
						UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
						UIManager.Instance.MaskUI(UIElement.Dialog);
					}
					else
					{
						DialogCmd dialogCmd = new DialogCmd();
						dialogCmd.Title = LocalStringManager.Get((bConfirm ? (this._blockData.OperationStopping ? this.ContinueOperationTipTitleKey : this.StartOperationTipTitleKey) : this.StopOperationTipTitleKey)[(int)operationType]);
						dialogCmd.Content = LocalStringManager.Get((bConfirm ? (this._blockData.OperationStopping ? this.ContinueOperationTipDescKey : this.ConfirmOperationTipDescKey) : this.StopOperationTipDescKey)[(int)operationType]);
						dialogCmd.Type = 1;
						dialogCmd.Yes = action;
						action();
					}
				}
			}
			else if (num <= 1520676528U)
			{
				if (num != 1293099464U)
				{
					if (num == 1520676528U)
					{
						if (text2 == "ShopQuickSelectBtn")
						{
							bool flag7 = this._configData == null;
							if (!flag7)
							{
								base.StartCoroutine(this.DisableShopQuickSelectButtonForAWhile());
								BuildingDomainMethod.AsyncCall.QuickArrangeShopManager(this, this._blockKey, null);
							}
						}
					}
				}
				else if (text2 == "ExpandQuickSelectBtn")
				{
					bool flag8 = this._configData == null;
					if (!flag8)
					{
						BuildingDomainMethod.AsyncCall.QuickArrangeBuildOperator(this, this._configData.TemplateId, this._blockKey, this.TogKeyToOperationType(this._mainToggleGroup.GetActive().Key), delegate(int offset, RawDataPool dataPool)
						{
							List<int> charIdList = new List<int>();
							Serializer.Deserialize(dataPool, offset, ref charIdList);
							for (int index = 0; index < charIdList.Count; index++)
							{
								this.SelectOperator(charIdList[index], index);
							}
							this.SetOperationLeftTimeString(charIdList);
						});
					}
				}
			}
			else if (num != 1801823230U)
			{
				if (num != 1875036912U)
				{
					if (num == 2839120067U)
					{
						if (text2 == "QuickRecruitPeopleBtn")
						{
							bool flag9 = this._earningsData == null || (this._earningsData.RecruitLevelList.Count == 0 && this._earningsData.RecruitLevelList.Count == 0);
							if (!flag9)
							{
								BuildingDomainMethod.AsyncCall.AcceptBuildingBlockRecruitPeopleQuick(this, this._blockKey, delegate(int offset, RawDataPool dataPool)
								{
									List<int> charIdList = new List<int>();
									Serializer.Deserialize(dataPool, offset, ref charIdList);
									bool flag19 = charIdList != null;
									if (flag19)
									{
										this.ShowGetPeopleView(charIdList);
									}
								});
							}
						}
					}
				}
				else if (text2 == "RemoveShopManager")
				{
					this._selectingOperatorIndex = -1;
					this._selectingShopManagerIndex = btn.transform.parent.GetSiblingIndex();
					this.SelectShopManager(-1);
				}
			}
			else if (text2 == "QuickRecruitRejectPeopleBtn")
			{
				bool flag10 = this._earningsData == null || (this._earningsData.RecruitLevelList.Count == 0 && this._earningsData.RecruitLevelList.Count == 0);
				if (!flag10)
				{
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
					{
						Title = LocalStringManager.Get(LanguageKey.LK_Building_QuickRejectRecruitPeople_Cmd_Title).ColorReplace(),
						Content = LocalStringManager.Get(LanguageKey.LK_Building_QuickRejectRecruitPeople_Cmd_Text).ColorReplace(),
						Type = 1,
						Yes = delegate()
						{
							BuildingDomainMethod.Call.RejectBuildingBlockRecruitPeopleQuick(this._blockKey);
						}
					}));
					UIManager.Instance.MaskUI(UIElement.Dialog);
				}
			}
		}
		else if (num <= 3491078529U)
		{
			if (num <= 3207020220U)
			{
				if (num != 3087260821U)
				{
					if (num == 3207020220U)
					{
						if (text2 == "ResidentViewQuickClearBtn")
						{
							bool flag11 = this._residents.Count == 0;
							if (!flag11)
							{
								bool flag12 = this._blockData.TemplateId == 47;
								if (flag12)
								{
									BuildingDomainMethod.Call.RemoveAllFromComfortableHouse(this._blockKey);
									BuildingDomainMethod.Call.GetCharsInComfortableHouse(this.Element.GameDataListenerId, this._blockKey);
								}
								else
								{
									bool flag13 = this._blockData.TemplateId == 46;
									if (flag13)
									{
										bool flag14 = this._residents.Count >= 6;
										if (flag14)
										{
											ViewBuildingArea.ShowDialog(LocalStringManager.Get(LanguageKey.LK_Building_QuickClear), LocalStringManager.Get(LanguageKey.LK_Building_QuickClearTip), delegate
											{
												BuildingDomainMethod.Call.RemoveAllFormResidence(this._blockKey);
												BuildingDomainMethod.Call.GetCharsInResidence(this.Element.GameDataListenerId, this._blockKey);
											}, null);
										}
										else
										{
											BuildingDomainMethod.Call.RemoveAllFormResidence(this._blockKey);
											BuildingDomainMethod.Call.GetCharsInResidence(this.Element.GameDataListenerId, this._blockKey);
										}
									}
								}
							}
						}
					}
				}
				else if (text2 == "RemoveOperator")
				{
					this._selectingOperatorIndex = btn.transform.parent.GetSiblingIndex();
					this._selectingShopManagerIndex = -1;
					this.SelectOperator(-1);
				}
			}
			else if (num != 3448155331U)
			{
				if (num == 3491078529U)
				{
					if (text2 == "RenameBtn")
					{
						TMP_InputField inputField = this._leftInfo.CGet<TMP_InputField>("CustomNameInput");
						btn.gameObject.SetActive(false);
						this._leftInfo.CGet<TextMeshProUGUI>("BuildingName").gameObject.SetActive(false);
						inputField.gameObject.SetActive(true);
						Renamer.SelectAndSetEscHandler(inputField, true);
						TMP_FontAsset fontAsset = inputField.textComponent.font;
						inputField.onValueChanged.RemoveAllListeners();
						inputField.textComponent.rectTransform.localPosition = Vector3.zero;
						inputField.text = (this._buildingModel.CustomBuildingName.ContainsKey(this._blockKey) ? SingletonObject.getInstance<BasicGameData>().CustomTexts[this._buildingModel.CustomBuildingName[this._blockKey]] : "");
						inputField.onValueChanged.AddListener(delegate(string valueStr)
						{
							inputField.FixAndSetInputFieldText(ref valueStr, fontAsset);
						});
						inputField.onEndEdit.RemoveAllListeners();
						inputField.onEndEdit.AddListener(delegate(string valueStr)
						{
							this.HideCustomNameInput();
							bool shouldCancel = Renamer.ShouldCancel;
							if (!shouldCancel)
							{
								UIManager.Instance.SetEscHandler(null);
								bool hasSensitiveWord = inputField.SensitiveWordHandle(ref valueStr);
								bool flag19 = hasSensitiveWord;
								if (flag19)
								{
									CanvasGroup commonWarningCanvasGroup = this._leftInfo.CGet<CanvasGroup>("SensitiveWarningTip");
									commonWarningCanvasGroup.alpha = 1f;
									bool flag20 = this._sensitiveWordTipCoroutine != null;
									if (flag20)
									{
										this.StopCoroutine(this._sensitiveWordTipCoroutine);
									}
									Tween sensitiveWordTipTween = this._sensitiveWordTipTween;
									if (sensitiveWordTipTween != null)
									{
										sensitiveWordTipTween.Kill(false);
									}
									this._sensitiveWordTipCoroutine = this.DelayCallReturnCoroutine(delegate
									{
										bool activeInHierarchy = commonWarningCanvasGroup.gameObject.activeInHierarchy;
										if (activeInHierarchy)
										{
											this._sensitiveWordTipTween = commonWarningCanvasGroup.DOFade(0f, SensitiveWordsSystem.SensitiveWordAnimationFadeTime);
										}
									}, SensitiveWordsSystem.SensitiveWordAnimationStayTime);
								}
								this.Rename();
								inputField.textComponent.transform.localPosition = Vector2.zero;
								inputField.transform.Find("Text Area/Caret").localPosition = Vector2.zero;
							}
						});
						bool flag15 = inputField.text.IsNullOrEmpty();
						if (flag15)
						{
							inputField.Select();
						}
						inputField.InputOnSelectBindMoveTextEnd();
					}
				}
			}
			else if (text2 == "Close")
			{
				this.QuickHide();
			}
		}
		else if (num <= 3631961807U)
		{
			if (num != 3627579297U)
			{
				if (num == 3631961807U)
				{
					if (text2 == "QuickCollectItemBtn")
					{
						bool isEntertain = this.IsEntertain;
						if (isEntertain)
						{
							this.GetAllEntertainReward();
						}
						else
						{
							bool flag16 = this._earningsData == null || (this._earningsData.CollectionItemList.Count == 0 && this._earningsData.CollectionResourceList.Count == 0);
							if (!flag16)
							{
								bool flag17 = this._configData.TemplateId == 222;
								if (!flag17)
								{
									BuildingDomainMethod.Call.AcceptBuildingBlockCollectEarningQuick(this._blockKey, false);
									this.ShowGetItemAnimation();
								}
							}
						}
					}
				}
			}
			else if (text2 == "FeastDropdownMask")
			{
				this.ShowFeastDropdownMask(false);
			}
		}
		else if (num != 3801697046U)
		{
			if (num != 4135341380U)
			{
				if (num == 4172688598U)
				{
					if (text2 == "ExpandQuickCancelBtn")
					{
						bool flag18 = this._configData == null;
						if (!flag18)
						{
							this._selectingOperatorIndex = 0;
							while (this._selectingOperatorIndex < this._operatorListCached.Length)
							{
								this.SelectOperator(-1);
								this._selectingOperatorIndex++;
							}
							this.UpdateExpandRemoveCollectOperators();
						}
					}
				}
			}
			else if (text2 == "RepairBuildingBtn")
			{
				BuildingDomainMethod.Call.Repair(this.Element.GameDataListenerId, this._blockKey);
				BuildingDomainMethod.Call.GetBuildingBlockList(this.Element.GameDataListenerId, new Location(this._areaId, this._blockId));
			}
		}
		else if (text2 == "ShopQuickSelectCancelBtn")
		{
			this.UnSelectShopManager();
			base.StartCoroutine(this.DisableShopQuickClearButtonForAWhile());
		}
	}

	// Token: 0x060018CF RID: 6351 RVA: 0x0009D3BC File Offset: 0x0009B5BC
	private bool IsDependKungfuPracticeRoom(BuildingBlockItem config)
	{
		return config.DependBuildings.Count > 0 && config.DependBuildings[0] == 52 && config.IsShop;
	}

	// Token: 0x060018D0 RID: 6352 RVA: 0x0009D3F8 File Offset: 0x0009B5F8
	private void ShowGetPeopleView(List<int> charIdList)
	{
		bool flag = charIdList.Count <= 0;
		if (!flag)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.SetObject("CharIdList", charIdList);
			argBox.Set("ObtainType", 15);
			UIElement.GetItem.SetOnInitArgs(argBox);
			UIManager.Instance.MaskUI(UIElement.GetItem);
		}
	}

	// Token: 0x060018D1 RID: 6353 RVA: 0x0009D458 File Offset: 0x0009B658
	private void ShowGetItemAnimation()
	{
		CanvasGroup canvasGroup = base.CGet<CanvasGroup>("GetItemTips");
		base.CGet<TextMeshProUGUI>("GetItemTipsText").SetText(LocalStringManager.Get(LanguageKey.LK_Building_GetResource), true);
		Sequence seq = DOTween.Sequence();
		seq.Append(canvasGroup.DOFade(1f, 0.5f));
		seq.Append(canvasGroup.DOFade(0f, 1f));
	}

	// Token: 0x060018D2 RID: 6354 RVA: 0x0009D4C2 File Offset: 0x0009B6C2
	public void SelectOperator(int charId)
	{
		this.SelectOperator(charId, this._selectingOperatorIndex);
	}

	// Token: 0x060018D3 RID: 6355 RVA: 0x0009D4D4 File Offset: 0x0009B6D4
	public void SelectOperator(int charId, int index)
	{
		bool flag = this._blockData.OperationType == -1;
		if (flag)
		{
			this._operatorListCached[index] = charId;
			this.UpdateExpandRemoveCollectOperators();
		}
		else
		{
			bool flag2 = this._operatorListCached[index] == charId;
			if (flag2)
			{
				return;
			}
			BuildingDomainMethod.Call.SetOperator(this._blockKey, (sbyte)index, charId);
		}
		this.RefreshAllQuickSelectButtons();
	}

	// Token: 0x060018D4 RID: 6356 RVA: 0x0009D534 File Offset: 0x0009B734
	public override void QuickHide()
	{
		AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
		bool activeSelf = this.ArrangementSettingPanel.gameObject.activeSelf;
		if (activeSelf)
		{
			this.ArrangeFocusFinish();
		}
		else
		{
			bool activeSelf2 = this.SoldItemSettingPanel.gameObject.activeSelf;
			if (activeSelf2)
			{
				this.HideSoldItemSetting();
			}
			else
			{
				bool flag = this._feastDropdownMask && this._feastDropdownMask.gameObject.activeSelf;
				if (flag)
				{
					this._feastDropdown.Hide();
					this.ShowFeastDropdownMask(false);
				}
				else
				{
					GEvent.OnEvent(UiEvents.BuildingManageClosed, null);
					base.QuickHide();
				}
			}
		}
	}

	// Token: 0x060018D5 RID: 6357 RVA: 0x0009D5E3 File Offset: 0x0009B7E3
	private void OnGUI()
	{
		this.UpdateFeastDropdown();
		this.UpdateSoldItemSettingDropdown();
	}

	// Token: 0x060018D6 RID: 6358 RVA: 0x0009D5F4 File Offset: 0x0009B7F4
	public void HideCustomNameInput()
	{
		TMP_InputField customNameInput = this._leftInfo.CGet<TMP_InputField>("CustomNameInput");
		bool activeSelf = customNameInput.gameObject.activeSelf;
		if (activeSelf)
		{
			this._leftInfo.CGet<CButtonObsolete>("RenameBtn").gameObject.SetActive(true);
			customNameInput.gameObject.SetActive(false);
			this._leftInfo.CGet<TextMeshProUGUI>("BuildingName").gameObject.SetActive(true);
		}
	}

	// Token: 0x060018D7 RID: 6359 RVA: 0x0009D669 File Offset: 0x0009B869
	private void InitDisplay()
	{
		this.InitMainToggleGroup();
		this.ShowEntranceButtons();
		this.ShowFixedBuildingInfo();
		this.UpdateButtonsShow();
	}

	// Token: 0x060018D8 RID: 6360 RVA: 0x0009D688 File Offset: 0x0009B888
	private void SelectExpandChild()
	{
		Refers refers = this._expandInfoPage.GetComponent<Refers>();
		BuildingExpandResource buildingExpandResource = refers.CGet<BuildingExpandResource>("BuildingExpandResource");
		BuildingExpandTaiwuVillage buildingExpandTaiwuVillage = refers.CGet<BuildingExpandTaiwuVillage>("BuildingExpandTaiwuVillage");
		BuildingExpandTeaHorseCaravan buildingExpandTeaHouseCaravan = refers.CGet<BuildingExpandTeaHorseCaravan>("BuildingExpandTeaHouseCaravan");
		BuildingExpandBuildingSlotWarehouse buildingExpandBuildingSlotWarehouse = refers.CGet<BuildingExpandBuildingSlotWarehouse>("BuildingExpandBuildingSlotWarehouse");
		BuildingExpandBuildingSlotResidence buildingExpandBuildingSlotResidence = refers.CGet<BuildingExpandBuildingSlotResidence>("BuildingExpandBuildingSlotResidence");
		BuildingExpandBuildingSlotComfortableHouse buildingExpandBuildingSlotComfortableHouse = refers.CGet<BuildingExpandBuildingSlotComfortableHouse>("BuildingExpandBuildingSlotComfortableHouse");
		BuildingBlockItem blockConfig = BuildingBlock.Instance[this._blockData.TemplateId];
		bool isResource = blockConfig.Class == EBuildingBlockClass.BornResource;
		buildingExpandResource.gameObject.SetActive(isResource);
		bool isTeaHorse = blockConfig.TemplateId == 51;
		buildingExpandTeaHouseCaravan.gameObject.SetActive(isTeaHorse);
		bool isTaiwuVillage = blockConfig.TemplateId == 44;
		buildingExpandTaiwuVillage.gameObject.SetActive(isTaiwuVillage);
		bool isSlotWarehouse = blockConfig.TemplateId == 48;
		buildingExpandBuildingSlotWarehouse.gameObject.SetActive(isSlotWarehouse);
		bool isSlotResidence = blockConfig.TemplateId == 46;
		buildingExpandBuildingSlotResidence.gameObject.SetActive(isSlotResidence);
		bool isSlotComfortableHouse = blockConfig.TemplateId == 47;
		buildingExpandBuildingSlotComfortableHouse.gameObject.SetActive(isSlotComfortableHouse);
	}

	// Token: 0x060018D9 RID: 6361 RVA: 0x0009D7A8 File Offset: 0x0009B9A8
	private void UpdateBuildingData()
	{
		this._configData = BuildingBlock.Instance[this._blockData.TemplateId];
		bool flag = this._configData.SuccesEvent.Count > 0;
		if (flag)
		{
			this._shopEventData = ShopEvent.Instance[this._configData.SuccesEvent[0]];
		}
		bool flag2 = this._buildingModel.BuildingOperators.ContainsKey(this._blockKey);
		if (flag2)
		{
			List<int> charList = this._buildingModel.BuildingOperators[this._blockKey].GetCollection();
			for (int i = 0; i < this._operatorListCached.Length; i++)
			{
				this._operatorListCached[i] = charList[i];
			}
			this.RefreshAllQuickSelectButtons();
		}
		else
		{
			this.ClearOperators();
		}
		this.FillShopManagerList();
		bool flag3 = this._shopEventData != null && this._shopEventData.ExchangeResourceGoods != -1 && this._blockData.OperationType == -1;
		if (flag3)
		{
			this.FreshCanSoldItemList();
		}
		bool flag4 = this._blockData.TemplateId == 105;
		if (flag4)
		{
			this.FreshCanSoldItemList();
		}
		bool isEntertain = this.IsEntertain;
		if (isEntertain)
		{
			this.FreshCanSoldItemList();
		}
		this._eventBook.SetActive(this.ShowEventBook());
		bool flag5 = this.ShowEventBook();
		if (flag5)
		{
			BuildingDomainMethod.AsyncCall.GetOrCreateShopEventCollection(this, this._blockKey, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._shopEventCollection);
				this.OnShopEventCollectionUpdated();
			});
		}
		this.RefreshAllQuickSelectButtons();
	}

	// Token: 0x060018DA RID: 6362 RVA: 0x0009D934 File Offset: 0x0009BB34
	private void FillShopManagerList()
	{
		List<int> buildingMemberList = this._buildingModel.GetBuildingShopManager(this._blockKey);
		bool flag = buildingMemberList != null;
		if (flag)
		{
			for (int i = 0; i < this._shopManagerListCached.Length; i++)
			{
				bool flag2 = i < buildingMemberList.Count;
				if (flag2)
				{
					this._shopManagerListCached[i] = buildingMemberList[i];
				}
				else
				{
					this._shopManagerListCached[i] = -1;
				}
			}
		}
		else
		{
			for (int j = 0; j < this._shopManagerListCached.Length; j++)
			{
				this._shopManagerListCached[j] = -1;
			}
		}
	}

	// Token: 0x060018DB RID: 6363 RVA: 0x0009D9CC File Offset: 0x0009BBCC
	private void OnShopEventCollectionUpdated()
	{
		this._shopEventRenderInfos.Clear();
		this._shopManageEventRenderInfos.Clear();
		this._shopLearnEventRenderInfos.Clear();
		this._managerArgumentCollection.Clear();
		this._managerRenderArgumentCollection.Clear();
		bool flag = this._shopEventCollection == null || this._shopEventCollection.Count == 0;
		if (flag)
		{
			this.UpdateShopEventBookInfo();
		}
		else
		{
			this.CollectShopEventBooks();
			string key = "UI_BuildingManage";
			RecordArgumentsRequest request = new RecordArgumentsRequest(this._managerArgumentCollection);
			LifeRecordDomainMethod.AsyncCall.GetRecordRenderInfoArguments(this, key, request, delegate(int offset, RawDataPool dataPool)
			{
				ArgumentCollectionRenderArguments dynamicArguments = null;
				Serializer.Deserialize(dataPool, offset, ref dynamicArguments);
				GameMessageUtils.RenderDynamicArguments(dynamicArguments, this._managerArgumentCollection, this._managerRenderArgumentCollection, false, false);
				GameMessageUtils.RenderFixedArguments(this._managerArgumentCollection, this._managerRenderArgumentCollection, false);
				this.UpdateShopEventBookInfo();
			});
		}
	}

	// Token: 0x060018DC RID: 6364 RVA: 0x0009DA6C File Offset: 0x0009BC6C
	private void CollectShopEventBooks()
	{
		this._shopEventRenderInfos.Clear();
		this._shopLearnEventRenderInfos.Clear();
		this._shopManageEventRenderInfos.Clear();
		this._shopEventCollection.GetRenderInfos(this._shopEventRenderInfos, this._managerArgumentCollection);
		UI_BuildingManage.EventBookMonthlyData currentLearnMonth = null;
		UI_BuildingManage.EventBookMonthlyData currentManageMonth = null;
		foreach (ShopEventRenderInfo info in this._shopEventRenderInfos)
		{
			bool flag = CommonUtils.CheckShopLearnEvent(info.RecordType);
			if (flag)
			{
				bool flag2 = currentLearnMonth == null || currentLearnMonth.Date != info.Date;
				if (flag2)
				{
					currentLearnMonth = new UI_BuildingManage.EventBookMonthlyData(info.Date);
					this._shopLearnEventRenderInfos.Add(currentLearnMonth);
				}
				currentLearnMonth.EventInfos.Add(info);
			}
			else
			{
				bool flag3 = currentManageMonth == null || currentManageMonth.Date != info.Date;
				if (flag3)
				{
					currentManageMonth = new UI_BuildingManage.EventBookMonthlyData(info.Date);
					this._shopManageEventRenderInfos.Add(currentManageMonth);
				}
				currentManageMonth.EventInfos.Add(info);
			}
		}
	}

	// Token: 0x060018DD RID: 6365 RVA: 0x0009DBA4 File Offset: 0x0009BDA4
	private void FreshCanSoldItemList()
	{
		this._inventoryItemList.Clear();
		this._inventoryCanSoldItemList.Clear();
		this._warehouseItemList.Clear();
		this._warehouseCanSoldItemList.Clear();
		this._treasuryItemList.Clear();
		this._treasuryCanSoldItemList.Clear();
		bool needContinue = (this._shopEventData != null && this._shopEventData.ExchangeResourceGoods != -1) || this._blockData.TemplateId == 105 || this.IsEntertain;
		bool flag = !needContinue;
		if (!flag)
		{
			bool flag2 = this._blockData.TemplateId == 105;
			if (flag2)
			{
				bool canTransfer = this._canTransfer;
				if (canTransfer)
				{
					BuildingDomainMethod.AsyncCall.GetTaiwuCanFixBookItemDataList(this, ItemSourceType.Inventory, delegate(int offset, RawDataPool dataPool)
					{
						Serializer.Deserialize(dataPool, offset, ref this._inventoryCanSoldItemList);
					});
				}
				BuildingDomainMethod.AsyncCall.GetTaiwuCanFixBookItemDataList(this, ItemSourceType.Warehouse, delegate(int offset, RawDataPool dataPool)
				{
					Serializer.Deserialize(dataPool, offset, ref this._warehouseCanSoldItemList);
				});
				BuildingDomainMethod.AsyncCall.GetTaiwuCanFixBookItemDataList(this, ItemSourceType.Treasury, delegate(int offset, RawDataPool dataPool)
				{
					Serializer.Deserialize(dataPool, offset, ref this._treasuryCanSoldItemList);
				});
				BuildingDomainMethod.AsyncCall.GetBuildingBlockData(this, this._blockKey, delegate(int offset2, RawDataPool pool2)
				{
					Serializer.Deserialize(pool2, offset2, ref this._blockData);
					UIElement.BuildingArea.UiBaseAs<ViewBuildingArea>().UpdateBuildingData(this._blockKey, this._blockData, true);
				});
			}
			else
			{
				bool canTransfer2 = this._canTransfer;
				if (canTransfer2)
				{
					CharacterDomainMethod.AsyncCall.GetAllInventoryItemsExcludeValueZero(this, this._taiwuCharId, delegate(int offset, RawDataPool dataPool)
					{
						Serializer.Deserialize(dataPool, offset, ref this._inventoryItemList);
						this.FilterItemByList(this._inventoryItemList, this._inventoryCanSoldItemList);
					});
				}
				TaiwuDomainMethod.AsyncCall.GetAllWarehouseItemsExcludeValueZero(this, delegate(int offset, RawDataPool dataPool)
				{
					Serializer.Deserialize(dataPool, offset, ref this._warehouseItemList);
					this.FilterItemByList(this._warehouseItemList, this._warehouseCanSoldItemList);
				});
				TaiwuDomainMethod.AsyncCall.GetAllTreasuryItems(this, delegate(int offset, RawDataPool dataPool)
				{
					Serializer.Deserialize(dataPool, offset, ref this._treasuryItemList);
					this._treasuryItemList.RemoveAll((ItemDisplayData d) => d.Value <= 0L);
					this.FilterItemByList(this._treasuryItemList, this._treasuryCanSoldItemList);
				});
			}
		}
	}

	// Token: 0x060018DE RID: 6366 RVA: 0x0009DD00 File Offset: 0x0009BF00
	private void FilterItemByList(List<ItemDisplayData> itemDataList, List<ItemDisplayData> resultList)
	{
		resultList.Clear();
		for (int i = 0; i < itemDataList.Count; i++)
		{
			ItemKey itemKey = itemDataList[i].Key;
			bool result = GameData.Domains.Building.SharedMethods.IsBuildingCanSoldItem(this._configData, itemKey);
			bool flag = result;
			if (flag)
			{
				resultList.Add(itemDataList[i]);
			}
		}
	}

	// Token: 0x060018DF RID: 6367 RVA: 0x0009DD5C File Offset: 0x0009BF5C
	private void UpdatePropertyValueData(BuildingBlockItem configData = null)
	{
		bool flag = configData == null;
		if (flag)
		{
			configData = this.GetConfigData();
		}
		bool flag2 = configData != null && configData.TemplateId != 0;
		if (flag2)
		{
			this._propertyValueDict.Clear();
			foreach (int charId in this._villagerList)
			{
				bool flag3 = configData.RequireLifeSkillType >= 0;
				if (flag3)
				{
					CharacterDomainMethod.Call.GetLifeSkillAttainment(this.Element.GameDataListenerId, charId, configData.RequireLifeSkillType);
				}
				else
				{
					bool flag4 = configData.RequireCombatSkillType >= 0;
					if (!flag4)
					{
						throw new Exception(string.Format("Require skill of building {0} not filled", configData.TemplateId));
					}
					CharacterDomainMethod.Call.GetCombatSkillAttainment(this.Element.GameDataListenerId, charId, configData.RequireCombatSkillType);
				}
			}
		}
	}

	// Token: 0x060018E0 RID: 6368 RVA: 0x0009DE58 File Offset: 0x0009C058
	private void UpdateRepairButton()
	{
		CButtonObsolete btn = base.CGet<CButtonObsolete>("RepairBuildingBtn");
		int togKey = this._mainToggleGroup.GetActive().Key;
		this.isNeedRepair = ((this._configData.Type == EBuildingBlockType.Building || this._configData.Type == EBuildingBlockType.MainBuilding) && this._blockData.NeedMaintenanceCost() && this._isTaiwuVillageBuilding && this._configData.MaxDurability > this._blockData.Durability && togKey == 4);
		btn.gameObject.SetActive(this.isNeedRepair);
		this._damageInfoPage.CGet<GameObject>("NeedResourceHolder").SetActive(this.isNeedRepair);
		this._damageInfoPage.CGet<GameObject>("RepairCompleteTip").SetActive(!this.isNeedRepair);
		bool flag = !btn.gameObject.activeSelf;
		if (!flag)
		{
			TooltipInvoker tipDisplayer = btn.GetComponent<TooltipInvoker>();
			bool flag2 = tipDisplayer.PresetParam == null || tipDisplayer.PresetParam.Length != 2;
			if (flag2)
			{
				tipDisplayer.PresetParam = new string[2];
			}
			bool flag3 = !this.IsBuildingManagementUnlocked;
			if (flag3)
			{
				btn.interactable = false;
				tipDisplayer.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_Building_Unmaintainable_Title);
				tipDisplayer.PresetParam[1] = LocalStringManager.Get(LanguageKey.LK_Functionality_Locked);
				tipDisplayer.Refresh(false, -1);
			}
			else
			{
				bool flag4 = this._blockData.Durability >= this._configData.MaxDurability;
				if (flag4)
				{
					btn.interactable = false;
					tipDisplayer.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_Building_No_Need_For_Maintenance_Title);
					tipDisplayer.PresetParam[1] = LocalStringManager.Get(LanguageKey.LK_Building_No_Need_For_Maintenance_Desc);
					tipDisplayer.Refresh(false, -1);
				}
				else
				{
					bool enough = true;
					StringBuilder builder = EasyPool.Get<StringBuilder>();
					builder.Clear();
					int[] resourceCosts = new int[8];
					sbyte repairCostResourceType = 6;
					int repqirCostCount = GameData.Domains.Building.SharedMethods.CalcRepairBuildingCost(this._blockData, this._configData);
					resourceCosts[(int)repairCostResourceType] = repqirCostCount;
					this._stringBuilder.Clear();
					sbyte resType = 0;
					while ((int)resType < resourceCosts.Length)
					{
						bool flag5 = resourceCosts[(int)resType] <= 0;
						if (!flag5)
						{
							int resourceOwned = this._buildingModel.GetResourceCount(resType);
							int resourceCost = resourceCosts[(int)resType];
							enough = (resourceOwned >= resourceCost);
							builder.AppendFormat("{0}:{1}/{2}\n", Config.ResourceType.Instance[repairCostResourceType].Name, resourceOwned.ToString(), resourceCost);
							this._stringBuilder.AppendFormat("{0}/{1}\n", CommonUtils.GetDisplayStringForNum(resourceOwned, 100000), CommonUtils.GetDisplayStringForNum(resourceCost, 100000));
						}
						resType += 1;
					}
					tipDisplayer.PresetParam[0] = LocalStringManager.Get(enough ? LanguageKey.LK_Building_Click_To_Maintain : LanguageKey.LK_Building_Unmaintainable_Title);
					tipDisplayer.PresetParam[1] = builder.ToString();
					tipDisplayer.Refresh(false, -1);
					Refers repairRefer = this._damageInfoPage.CGet<Refers>("RepairResourceInfo");
					repairRefer.CGet<CImage>("ResourceIcon").SetSprite(Config.ResourceType.Instance[repairCostResourceType].Icon, false, null);
					repairRefer.CGet<TextMeshProUGUI>("ResourceName").SetText(Config.ResourceType.Instance[repairCostResourceType].Name, true);
					Debug.Log(this._stringBuilder);
					repairRefer.CGet<TextMeshProUGUI>("ResourceCount").SetText(this._stringBuilder.ToString(), true);
					EasyPool.Free<StringBuilder>(builder);
					btn.interactable = enough;
				}
			}
		}
	}

	// Token: 0x060018E1 RID: 6369 RVA: 0x0009E1CC File Offset: 0x0009C3CC
	private void ShowEntranceButtons()
	{
		this.btnIndex = 0;
		this._btnHolder.gameObject.SetActive(false);
		bool flag = this._blockData.TemplateId == 44;
		if (flag)
		{
			this.CreateBtnGo("building_icon_cuzhichenlie", delegate
			{
				BuildingActionUtils.ShowCricketCollection();
			}, LanguageKey.LK_Building_Btn_Cricket);
			bool canOperateStoneRoom = SingletonObject.getInstance<BuildingModel>().CanOperateStoneRoom;
			if (canOperateStoneRoom)
			{
				this.CreateBtnGo("building_icon_shiwu", delegate
				{
					BuildingActionUtils.ShowStoneHouse(this._blockKey);
				}, LanguageKey.LK_Building_StoneRoom);
			}
			ExtraDomainMethod.AsyncCall.GetIsJiaoPoolOpen(this, delegate(int offset, RawDataPool dataPool)
			{
				bool isOpen = false;
				Serializer.Deserialize(dataPool, offset, ref isOpen);
				bool flag18 = isOpen;
				if (flag18)
				{
					this.CreateBtnGo("building_icon_jiaochi", delegate
					{
						BuildingActionUtils.ShowJiaoPool(this._areaData);
					}, LanguageKey.LK_Building_Jiaochi);
				}
			});
		}
		else
		{
			bool flag2 = this._blockData.TemplateId == 52;
			if (flag2)
			{
				bool isTaiwuVillageBuilding = this._isTaiwuVillageBuilding;
				if (isTaiwuVillageBuilding)
				{
					Refers btnRefers = this.CreateBtnGo("building_icon_xunlian", delegate
					{
						UIManager.Instance.ShowUI(UIElement.KungfuPracticeRoomPuppet, true);
					}, LanguageKey.LK_Building_KungfuRoomIcon);
					BuildingDomainMethod.AsyncCall.GetXiangshuIdInKungfuRoom(this, delegate(int offset, RawDataPool dataPool)
					{
						List<sbyte> xiangshuIdList = new List<sbyte>();
						Serializer.Deserialize(dataPool, offset, ref xiangshuIdList);
						btnRefers.GetComponent<CButtonObsolete>().interactable = (xiangshuIdList.Count > 0);
						bool flag18 = xiangshuIdList.Count == 0;
						if (flag18)
						{
							TooltipInvoker mouseTip = btnRefers.CGet<TooltipInvoker>("BtnMouseTip");
							mouseTip.enabled = true;
							mouseTip.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_Building_KungfuRoomTips5);
						}
					});
				}
				OrganizationDomainMethod.AsyncCall.GetOrganizationTemplateIdOfTaiwuLocation(this, delegate(int offset, RawDataPool dataPool)
				{
					short currLocationOrganizationTemplateId = -1;
					Serializer.Deserialize(dataPool, offset, ref currLocationOrganizationTemplateId);
					this.CreateBtnGo("building_icon_trainingroom", delegate
					{
						ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
						argBox.Set("CharId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
						argBox.Set("ShowCombatSkill", true);
						argBox.Set("CurrLocationOrganizationTemplateId", currLocationOrganizationTemplateId);
						argBox.Set("CheckEquipRequirePracticeLevel", false);
						argBox.Set("ShowNone", false);
						argBox.Set("AtSettlement", this._canTransfer);
						argBox.Set("IsTaiwuVillageBuilding", this._isTaiwuVillageBuilding);
						argBox.Set("ShowSelectCount", true);
						argBox.Set("PracticeCombatSkillCostActionPoint", UI_BuildingManage.GetPracticeCombatSkillCostActionPoint((int)this._buildingModel.GetBuildingLevel(this._blockKey, this._blockData)));
						argBox.SetObject("UnselectableCombatSkillList", new List<short>());
						argBox.SetObject("CombatSkillIdList", this.GetCurrOrganizationCanPracticeSkills(currLocationOrganizationTemplateId));
						argBox.SetObject("Callback2", new Action<sbyte, short, int>(delegate(sbyte type, short skillId, int count)
						{
							bool flag18 = type < 0 || skillId < 0;
							if (!flag18)
							{
								BuildingDomainMethod.AsyncCall.PracticingCombatSkillInPracticeRoom(this, this._blockKey, skillId, count, UI_BuildingManage.GetPracticeCombatSkillCostActionPoint((int)this._buildingModel.GetBuildingLevel(this._blockKey, this._blockData)) * count, delegate(int offset2, RawDataPool dataPool2)
								{
									int proficiency = 0;
									Serializer.Deserialize(dataPool2, offset2, ref proficiency);
									ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
									argumentBox.Set("Proficiency", proficiency);
									argumentBox.SetObject("CollectInfo", new List<CollectResourceResult>
									{
										new CollectResourceResult
										{
											ResourceType = -1,
											ResourceCount = proficiency,
											ItemDisplayData = null
										}
									});
									argumentBox.Set("CollectType", 3);
									UIElement.CollectResource.SetOnInitArgs(argumentBox);
									UIManager.Instance.ShowUI(UIElement.CollectResource, true);
									EasyPool.Free<ArgumentBox>(argumentBox);
								});
							}
						}));
						argBox.Set("IsShowNeiLiFinish", false);
						UIElement.SelectSkill.SetOnInitArgs(argBox);
						UIManager.Instance.MaskUI(UIElement.SelectSkill);
					}, LanguageKey.LK_PracticeCombatSkill_Name);
				});
			}
			else
			{
				bool flag3 = this._blockData.TemplateId == 50;
				if (flag3)
				{
					this.CreateBtnGo("building_icon_lunhuitai", delegate
					{
						SingletonObject.getInstance<CharacterMonitorModel>().RefreshAllMonitorCharacterAliveState();
						ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("BuildingData", this._blockData).SetObject("BuildingKey", this._blockKey);
						UIElement.SamsaraPlatform.SetOnInitArgs(args);
						UIManager.Instance.ShowUI(UIElement.SamsaraPlatform, true);
					}, LanguageKey.LK_Building_Samsara_Platform);
					OrganizationDomainMethod.AsyncCall.GetSectFunctionStatus(this, 11, SectFunctionStatuses.SectFunctionStatusType.SpecialInteractionUnlocked, delegate(int offset, RawDataPool dataPool)
					{
						bool isOpen = false;
						Serializer.Deserialize(dataPool, offset, ref isOpen);
						bool flag18 = isOpen;
						if (flag18)
						{
							this.CreateBtnGo("building_icon_swapsoul", delegate
							{
								UIManager.Instance.ShowUI(UIElement.SwapSoul, true);
							}, LanguageKey.LK_Building_Btn_SoulSwapCeremony);
						}
					});
					StoryDomainMethod.AsyncCall.JingangMonkSoulBtnShow(this, delegate(int offset, RawDataPool dataPool)
					{
						bool isOpen = false;
						Serializer.Deserialize(dataPool, offset, ref isOpen);
						bool flag18 = isOpen;
						if (flag18)
						{
							this.CreateBtnGo("building_icon_swapsoul", delegate
							{
								BuildingDomainMethod.Call.SectMainStoryJingangClickMonkSoulBtn();
							}, LanguageKey.UI_SectMainStory_Jingang_MonkSoul);
						}
					});
				}
				else
				{
					bool flag4 = this._blockData.TemplateId == 49;
					if (flag4)
					{
						this.CreateBtnGo("building_icon_sicao_0", delegate
						{
							BuildingActionUtils.ShowWarehouse(ItemSourceType.Trough);
						}, LanguageKey.LK_Trough);
						OrganizationDomainMethod.AsyncCall.GetSectFunctionStatus(this, 14, SectFunctionStatuses.SectFunctionStatusType.SpecialInteractionUnlocked, delegate(int offset, RawDataPool dataPool)
						{
							bool isOpen = false;
							Serializer.Deserialize(dataPool, offset, ref isOpen);
							bool flag18 = isOpen;
							if (flag18)
							{
								this.CreateBtnGo("building_icon_assignchicken", delegate
								{
									ArgumentBox argbox = EasyPool.Get<ArgumentBox>();
									argbox.Set("EnterType", ViewVillagerRole.EnterType.Normal);
									argbox.Set("EnterPage", ViewVillagerRole.EVillagerRolePage.ChickenAssign);
									UIElement.VillagerRole.SetOnInitArgs(argbox);
									UIManager.Instance.ShowUI(UIElement.VillagerRole, true);
								}, LanguageKey.LK_AssignChicken_Title);
							}
						});
					}
					else
					{
						bool flag5 = this._blockData.TemplateId == 45;
						if (flag5)
						{
							this.CreateBtnGo("building_icon_taiwucitang", delegate
							{
								ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
								argBox.Set("AreaId", this._areaId);
								argBox.Set("BlockId", this._blockId);
								argBox.Set("BuildingBlockIndex", this._buildingBlockIndex);
								argBox.Set("EnterType", ViewVillagerRole.EnterType.Normal);
								argBox.Set("EnterPage", ViewVillagerRole.EVillagerRolePage.RoleDescription);
								UIElement.VillagerRole.SetOnInitArgs(argBox);
								UIManager.Instance.ShowUI(UIElement.VillagerRole, true);
							}, LanguageKey.LK_Building_TaiwuVillageLineage_Name);
						}
						else
						{
							bool flag6 = this._blockData.TemplateId == 51;
							if (flag6)
							{
								this.CreateBtnGo("building_icon_chamabang", delegate
								{
									UIElement.TeaHorseCaravan.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("BuildingBlockData", this._blockData).SetObject("BuildingBlockKey", this._blockKey));
									UIManager.Instance.ShowUI(UIElement.TeaHorseCaravan, true);
								}, LanguageKey.LK_Building_TeaHouseCaravan);
							}
							else
							{
								bool flag7 = this._blockData.TemplateId == 48;
								if (flag7)
								{
									this.CreateBtnGo("building_icon_cangkurukou", delegate
									{
										BuildingActionUtils.ShowWarehouse(ItemSourceType.Warehouse);
									}, LanguageKey.LK_Warehouse);
									bool showMoreTog = SingletonObject.getInstance<BasicGameData>().CanShowMoreTogOnWarehouse();
									bool flag8 = showMoreTog;
									if (flag8)
									{
										this.CreateBtnGo("building_icon_gongku_0", delegate
										{
											BuildingActionUtils.ShowWarehouse(ItemSourceType.Treasury);
										}, LanguageKey.LK_Treasury);
										this.CreateBtnGo("building_icon_huocang_0", delegate
										{
											BuildingActionUtils.ShowWarehouse(ItemSourceType.Stock);
										}, LanguageKey.LK_StockStorage);
									}
								}
								else
								{
									bool flag9 = this._blockData.TemplateId >= 276 && this._blockData.TemplateId <= 282;
									if (flag9)
									{
										Refers btnRefers2 = this.CreateBtnGo("building_icon_shanghui", delegate
										{
											BuildingActionUtils.ShowMerchant(this._configData, this._areaId);
										}, LanguageKey.LK_Merchant);
									}
									else
									{
										bool flag10 = this._blockData.TemplateId == 283;
										if (flag10)
										{
											this.CreateBtnGo("building_icon_shanghui", delegate
											{
												BuildingActionUtils.ShowSpecialShop(this._configData);
											}, LanguageKey.LK_Merchant);
										}
										else
										{
											short templateId = this._blockData.TemplateId;
											bool flag11 = templateId >= 284 && templateId <= 302;
											if (flag11)
											{
												this.CreateBtnGo("building_icon_gongku_0", delegate
												{
													BuildingActionUtils.ShowTreasuryShop(this._blockData);
												}, LanguageKey.LK_Building_Treasury);
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
		bool canMakeItem = this._configData.CanMakeItem;
		if (canMakeItem)
		{
			this.CreateBtnGo("building_icon_zhizao", delegate
			{
				BuildingActionUtils.ShowMake(this._blockData, this._blockKey, UI_Make.UIMakeTab.Make);
			}, (this._blockData.TemplateId == 257 || this._blockData.TemplateId == 258) ? LanguageKey.LK_Building_FixWood : LanguageKey.LK_Make_Item);
		}
		bool flag12 = this._configData.AddReadingLifeSkillBookEfficiency == 6 || this._configData.AddReadingLifeSkillBookEfficiency == 7 || this._configData.AddReadingLifeSkillBookEfficiency == 10 || this._configData.AddReadingLifeSkillBookEfficiency == 11 || this._configData.AddReadingLifeSkillBookEfficiency == 8 || this._configData.AddReadingLifeSkillBookEfficiency == 9 || this._blockData.TemplateId == 257 || this._blockData.TemplateId == 258;
		if (flag12)
		{
			this.CreateBtnGo("building_icon_xiubu", delegate
			{
				BuildingActionUtils.ShowMake(this._blockData, this._blockKey, UI_Make.UIMakeTab.Repair);
			}, LanguageKey.LK_Building_Btn_Repair);
		}
		bool flag13 = this._configData.AddReadingLifeSkillBookEfficiency == 9;
		if (flag13)
		{
			this.CreateBtnGo("building_icon_cuidu", delegate
			{
				BuildingActionUtils.ShowMake(this._blockData, this._blockKey, UI_Make.UIMakeTab.Poison);
			}, LanguageKey.LK_Poison_Item);
			this.CreateBtnGo("building_icon_jiedu", delegate
			{
				BuildingActionUtils.ShowMake(this._blockData, this._blockKey, UI_Make.UIMakeTab.RemovePoison);
			}, LanguageKey.LK_Remove_Poison);
		}
		bool flag14 = this._configData.AddReadingLifeSkillBookEfficiency == 6 || this._configData.AddReadingLifeSkillBookEfficiency == 7 || this._configData.AddReadingLifeSkillBookEfficiency == 10 || this._configData.AddReadingLifeSkillBookEfficiency == 11 || this._blockData.TemplateId == 257 || this._blockData.TemplateId == 258;
		if (flag14)
		{
			this.CreateBtnGo("building_icon_jingzhi", delegate
			{
				BuildingActionUtils.ShowMake(this._blockData, this._blockKey, UI_Make.UIMakeTab.Refine);
			}, LanguageKey.LK_Strengthen_Item);
		}
		bool flag15 = this._configData.AddReadingLifeSkillBookEfficiency == 10;
		if (flag15)
		{
			this.CreateBtnGo("building_icon_alterations", delegate
			{
				BuildingActionUtils.ShowMake(this._blockData, this._blockKey, UI_Make.UIMakeTab.Weave);
			}, LanguageKey.LK_Weave_Item);
		}
		bool flag16 = this._configData.TemplateId >= 303 && this._configData.TemplateId <= 317;
		if (flag16)
		{
			this.CreateBtnGo("building_icon_jianlao_0", delegate
			{
				BuildingActionUtils.ShowPrison();
			}, LanguageKey.LK_SettlementPrison);
			this.CreateBtnGo("building_icon_xuanshang_0", delegate
			{
				BuildingActionUtils.ShowBounty(this._blockKey);
			}, LanguageKey.LK_BountyAmount_Short);
			this.CreateBtnGo("building_icon_law_0", delegate
			{
				BuildingActionUtils.ShowLaw(this._blockKey);
			}, LanguageKey.LK_Law_Title);
		}
		bool flag17 = this._configData.ArtisanOrderAvailable && this._isTaiwuVillageBuilding;
		if (flag17)
		{
			this.CreateBtnGo("building_icon_jiangren", delegate
			{
				BuildingActionUtils.ShowMake(this._blockData, this._blockKey, UI_Make.UIMakeTab.CraftsmanPanel);
			}, LanguageKey.LK_Craftsman_Entry);
		}
	}

	// Token: 0x060018E2 RID: 6370 RVA: 0x0009E8BC File Offset: 0x0009CABC
	private Refers CreateBtnGo(string btnIcon, Action action, LanguageKey btnNameKey)
	{
		this._btnHolder.gameObject.SetActive(true);
		bool flag = this._btnGroup.Count > this.btnIndex;
		GameObject btnGo;
		if (flag)
		{
			btnGo = this._btnGroup[this.btnIndex];
		}
		else
		{
			btnGo = Object.Instantiate<GameObject>(this._btnTemplate, this._buttonLayoutGroup.transform);
			this._btnGroup.Add(btnGo);
		}
		base.CGet<GameObject>("ButtonHolderLine").SetActive(true);
		this.btnIndex++;
		btnGo.gameObject.SetActive(true);
		Refers btnRefers = btnGo.GetComponent<Refers>();
		btnRefers.CGet<CImage>("BtnIcon").SetSprite(btnIcon, false, null);
		btnRefers.CGet<TextMeshProUGUI>("BtnName").SetText(LocalStringManager.Get(btnNameKey), true);
		CButtonObsolete btn = btnRefers.CGet<CButtonObsolete>("BtnTemplate");
		btn.ClearAndAddListener(delegate
		{
			Action action2 = action;
			if (action2 != null)
			{
				action2();
			}
		});
		btn.interactable = (this._blockData.OperationType != 0);
		TooltipInvoker mouseTip = btnRefers.CGet<TooltipInvoker>("BtnMouseTip");
		mouseTip.enabled = (this._blockData.OperationType == 0);
		mouseTip.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_Building_Constructing);
		bool flag2 = btnNameKey.Equals(LanguageKey.LK_Warehouse);
		if (flag2)
		{
			btn.interactable = (this._blockData.OperationType == -1);
			btn.GetComponent<TooltipInvoker>().enabled = (this._blockData.OperationType == 0 || this._blockData.OperationType == 1);
		}
		btnRefers.CGet<DisableStyleRoot>("DisableStyleRoot").SetStyleEffect(!btn.interactable, false);
		this.RefreshBasicInfoSize();
		return btnRefers;
	}

	// Token: 0x060018E3 RID: 6371 RVA: 0x0009EA90 File Offset: 0x0009CC90
	private void UpdateCurrentPage()
	{
		bool flag = !this._displayInited;
		if (!flag)
		{
			this.UpdateBuildingInfoPage();
			bool flag2 = this._removeCollectInfoPage.gameObject.activeSelf && this._blockData.OperationType != 0;
			if (flag2)
			{
				this.UpdateRemoveInfoPage();
			}
			else
			{
				bool activeSelf = this._shopInfoPage.gameObject.activeSelf;
				if (activeSelf)
				{
					this.UpdateShopInfoPage();
				}
				else
				{
					bool activeSelf2 = this._damageInfoPage.gameObject.activeSelf;
					if (activeSelf2)
					{
						this.UpdateDamageInfoPage();
					}
					else
					{
						bool activeSelf3 = this._expandInfoPage.gameObject.activeSelf;
						if (activeSelf3)
						{
							this.UpdateExpandInfoPage();
						}
					}
				}
			}
			this.UpdateButtonsShow();
			this.UpdateToggleTitle();
			bool flag3 = this._mainToggleGroup.GetActivatedCount() != 0 || this._blockData.OperationType == 0;
			if (flag3)
			{
				this._topInfo.SetActive(true);
				base.CGet<GameObject>("NoContent").SetActive(false);
			}
			else
			{
				this._topInfo.SetActive(false);
				base.CGet<GameObject>("NoContent").SetActive(true);
			}
		}
	}

	// Token: 0x060018E4 RID: 6372 RVA: 0x0009EBBC File Offset: 0x0009CDBC
	private void UpdateDamageInfoPage()
	{
		this.UpdateDamageInfo();
		this.UpdateRepairButton();
		this.UpdateBuildingInfoPage();
	}

	// Token: 0x060018E5 RID: 6373 RVA: 0x0009EBD4 File Offset: 0x0009CDD4
	private void UpdateDamageInfo()
	{
		TextMeshProUGUI damageText = this._damageInfoPage.CGet<TextMeshProUGUI>("DamageText");
		damageText.text = ((this._blockData.OperationType != 0) ? string.Format("{0}%", (int)((this._configData.MaxDurability - this._blockData.Durability) * 100 / this._configData.MaxDurability)) : "0%");
		this._damageInfoPage.CGet<CImage>("DamageCircle").fillAmount = ((this._blockData.OperationType != 0) ? ((float)(this._configData.MaxDurability - this._blockData.Durability) / (float)this._configData.MaxDurability) : 0f);
		damageText.text = damageText.text.SetColor((this._blockData.Durability < this._configData.MaxDurability) ? "brightred" : "pinkyellow");
	}

	// Token: 0x060018E6 RID: 6374 RVA: 0x0009ECC8 File Offset: 0x0009CEC8
	private void UpdateExpandInfoPage()
	{
		Refers refers = this._expandInfoPage.GetComponent<Refers>();
		BuildingExpandResource buildingExpandResource = refers.CGet<BuildingExpandResource>("BuildingExpandResource");
		BuildingExpandTaiwuVillage buildingExpandTaiwuVillage = refers.CGet<BuildingExpandTaiwuVillage>("BuildingExpandTaiwuVillage");
		BuildingExpandTeaHorseCaravan buildingExpandTeaHouseCaravan = refers.CGet<BuildingExpandTeaHorseCaravan>("BuildingExpandTeaHouseCaravan");
		BuildingExpandBuildingSlotWarehouse buildingExpandBuildingSlotWarehouse = refers.CGet<BuildingExpandBuildingSlotWarehouse>("BuildingExpandBuildingSlotWarehouse");
		BuildingExpandBuildingSlotResidence buildingExpandBuildingSlotResidence = refers.CGet<BuildingExpandBuildingSlotResidence>("BuildingExpandBuildingSlotResidence");
		BuildingExpandBuildingSlotComfortableHouse buildingExpandBuildingSlotComfortableHouse = refers.CGet<BuildingExpandBuildingSlotComfortableHouse>("BuildingExpandBuildingSlotComfortableHouse");
		bool activeSelf = buildingExpandTaiwuVillage.gameObject.activeSelf;
		if (activeSelf)
		{
			buildingExpandTaiwuVillage.Refresh(this, this._blockKey, this._blockData);
		}
		bool activeSelf2 = buildingExpandResource.gameObject.activeSelf;
		if (activeSelf2)
		{
			buildingExpandResource.Refresh(this, this._blockKey, this._blockData);
		}
		bool activeSelf3 = buildingExpandTeaHouseCaravan.gameObject.activeSelf;
		if (activeSelf3)
		{
			buildingExpandTeaHouseCaravan.Refresh(this._blockKey);
		}
		bool activeSelf4 = buildingExpandBuildingSlotWarehouse.gameObject.activeSelf;
		if (activeSelf4)
		{
			buildingExpandBuildingSlotWarehouse.Refresh(this, this._blockKey, this._blockData);
		}
		bool activeSelf5 = buildingExpandBuildingSlotResidence.gameObject.activeSelf;
		if (activeSelf5)
		{
			buildingExpandBuildingSlotResidence.Refresh(this, this._blockKey, this._blockData);
		}
		bool activeSelf6 = buildingExpandBuildingSlotComfortableHouse.gameObject.activeSelf;
		if (activeSelf6)
		{
			buildingExpandBuildingSlotComfortableHouse.Refresh(this, this._blockKey, this._blockData);
		}
	}

	// Token: 0x060018E7 RID: 6375 RVA: 0x0009EE0C File Offset: 0x0009D00C
	private void UpdateButtonsShow()
	{
		Refers confirmBtn = base.CGet<CButtonObsolete>("ConfirmBtn").GetComponent<Refers>();
		Refers cancelBtn = base.CGet<CButtonObsolete>("CancelBtn").GetComponent<Refers>();
		CButtonObsolete repairBtn = base.CGet<CButtonObsolete>("RepairBuildingBtn");
		cancelBtn.gameObject.SetActive(false);
		confirmBtn.gameObject.SetActive(false);
		int togKey = this._mainToggleGroup.GetActive().Key;
		repairBtn.gameObject.SetActive(togKey == 4 && this.isNeedRepair);
		bool flag = togKey == 0;
		if (flag)
		{
			bool flag2 = this._blockData.OperationType == 0 && !this._blockData.OperationStopping;
			if (flag2)
			{
				cancelBtn.gameObject.SetActive(true);
				confirmBtn.gameObject.SetActive(false);
				cancelBtn.CGet<TextMeshProUGUI>("CancelText").text = LocalStringManager.Get(LanguageKey.LK_Building_Stop_Build);
			}
			else
			{
				bool flag3 = this._blockData.OperationType == 0 && this._blockData.OperationStopping;
				if (flag3)
				{
					cancelBtn.gameObject.SetActive(false);
					confirmBtn.gameObject.SetActive(true);
					confirmBtn.CGet<TextMeshProUGUI>("ConfirmText").text = LocalStringManager.Get(LanguageKey.LK_Building_Continue_Build);
				}
			}
			this.UpdateExpandRemoveCollectOperators();
		}
		else
		{
			bool flag4 = togKey == 3;
			if (flag4)
			{
				bool flag5 = this._blockData.OperationType == -1;
				if (flag5)
				{
					cancelBtn.gameObject.SetActive(false);
					confirmBtn.gameObject.SetActive(true);
					confirmBtn.CGet<TextMeshProUGUI>("ConfirmText").text = LocalStringManager.Get(LanguageKey.LK_Building_Start_Demolish);
					confirmBtn.CGet<CImage>("ConfirmIcon").SetSprite("building_icon_xiaochaichu", false, null);
				}
				else
				{
					bool flag6 = this._blockData.OperationType == 1 && !this._blockData.OperationStopping;
					if (flag6)
					{
						cancelBtn.gameObject.SetActive(true);
						confirmBtn.gameObject.SetActive(false);
						cancelBtn.CGet<TextMeshProUGUI>("CancelText").text = LocalStringManager.Get(LanguageKey.LK_Building_Stop_Demolish);
					}
					else
					{
						bool flag7 = this._blockData.OperationType == 1 && this._blockData.OperationStopping;
						if (flag7)
						{
							cancelBtn.gameObject.SetActive(false);
							confirmBtn.gameObject.SetActive(true);
							confirmBtn.CGet<TextMeshProUGUI>("ConfirmText").text = LocalStringManager.Get(LanguageKey.LK_Building_Continue_Remove);
						}
					}
				}
			}
		}
	}

	// Token: 0x060018E8 RID: 6376 RVA: 0x0009F08C File Offset: 0x0009D28C
	private void UpdateBuildingInfoPage()
	{
		this.UpdateBuildingNormalInfo();
		bool flag = this._configData.TemplateId == 46 || this._configData.TemplateId == 47;
		if (flag)
		{
			bool flag2 = this._configData.TemplateId == 46;
			if (flag2)
			{
				BuildingDomainMethod.Call.GetCharsInResidence(this.Element.GameDataListenerId, this._blockKey);
			}
			bool flag3 = this._configData.TemplateId == 47;
			if (flag3)
			{
				BuildingDomainMethod.Call.GetCharsInComfortableHouse(this.Element.GameDataListenerId, this._blockKey);
			}
			this.UpdateAutoCheckIn();
		}
		this.UpdateCalculates();
		this._buildingInfoPage.CGet<GameObject>("DependBuildings").SetActive(false);
		this._buildingInfoPage.CGet<GameObject>("ExpandBuildings").SetActive(false);
		bool flag4 = !SingletonObject.getInstance<WorldMapModel>().IsAtSecretVillage() && this._isTaiwuVillageBuilding && this._buildingModel.VillageManagementUnlocked;
		if (flag4)
		{
			this.UpdateDependBuildings();
			this.UpdateExpandBuildings();
		}
	}

	// Token: 0x060018E9 RID: 6377 RVA: 0x0009F190 File Offset: 0x0009D390
	private void OnBuildingBlockDataChange(ArgumentBox _)
	{
		bool flag = this._buildingInfoPage == null;
		if (!flag)
		{
			this.UpdateBuildingInfoPage();
		}
	}

	// Token: 0x060018EA RID: 6378 RVA: 0x0009F1B8 File Offset: 0x0009D3B8
	private void UpdateBuildingNormalInfo()
	{
		this._stringBuilder.Clear();
		this._stringBuilder.Append(this._configData.Desc.SetColor("lightgrey") + "\n\n" + this._configData.FuncDesc.SetColor("pinkyellow"));
		this._buildingInfoPage.CGet<TextMeshProUGUI>("DescText").text = this._stringBuilder.ToString();
		this.UpdateLevelInfo();
		string damageText = (this._blockData.OperationType != 0) ? string.Format("{0}%", (int)((this._configData.MaxDurability - this._blockData.Durability) * 100 / this._configData.MaxDurability)) : "0%";
		TextMeshProUGUI damageContent = this._buildingInfoPage.CGet<TextMeshProUGUI>("DamageContent");
		damageContent.text = damageText.SetColor((this._blockData.Durability < this._configData.MaxDurability) ? "brightred" : "pinkyellow");
		this._buildingInfoPage.CGet<CImage>("DamageCircle").fillAmount = ((this._blockData.OperationType != 0) ? ((float)(this._configData.MaxDurability - this._blockData.Durability) / (float)this._configData.MaxDurability) : 0f);
	}

	// Token: 0x060018EB RID: 6379 RVA: 0x0009F318 File Offset: 0x0009D518
	private void UpdateLevelInfo()
	{
		TextMeshProUGUI levelText = this._buildingInfoPage.CGet<TextMeshProUGUI>("LevelText");
		bool flag = this._configData.MaxLevel <= 1;
		if (flag)
		{
			levelText.transform.parent.gameObject.SetActive(false);
		}
		else
		{
			levelText.transform.parent.gameObject.SetActive(true);
			sbyte buildingLevel = this._buildingModel.GetBuildingLevel(this._blockKey, this._blockData);
			int displayLevel = (this._blockData.OperationType == 0) ? 0 : Mathf.Max((int)buildingLevel, 0);
			levelText.text = string.Format("{0}/{1}", displayLevel, this._configData.MaxLevel);
			TooltipInvoker levelTips = this._buildingInfoPage.CGet<TooltipInvoker>("LevelNameTips");
			levelTips.Type = TipType.BuildingLevel;
			levelTips.RuntimeParam = EasyPool.Get<ArgumentBox>();
			string line1Text = LocalStringManager.GetFormat(LanguageKey.LK_Building_LevelTip_Content1, displayLevel, this._configData.MaxLevel);
			levelTips.RuntimeParam.Set("Line1Text", line1Text.ColorReplace()).Set("BuildingBlockTemplateId", this._configData.TemplateId).Set("BuildingLevel", buildingLevel).Set("IsTaiwuVillageBuilding", this._isTaiwuVillageBuilding).Set("ResourceBlockRanking", this._resourceBlockRanking);
			levelTips.Refresh(false, -1);
		}
	}

	// Token: 0x060018EC RID: 6380 RVA: 0x0009F484 File Offset: 0x0009D684
	private unsafe void UpdateCalculates()
	{
		RectTransform calculate = this._buildingInfoPage.CGet<RectTransform>("Calculate");
		GameObject calculateTitle = this._buildingInfoPage.CGet<GameObject>("CalculateTitle");
		calculate.gameObject.SetActive(this._isTaiwuVillageBuilding && this._blockData.OperationType != 0);
		calculateTitle.gameObject.SetActive(this._isTaiwuVillageBuilding && this._blockData.OperationType != 0);
		bool flag = !this._isTaiwuVillageBuilding || this._blockData.OperationType == 0;
		if (!flag)
		{
			Transform template = calculate.Find("Template");
			Span<IntPair> span2;
			if (this._configData.ExpandInfos != null)
			{
				int count = this._configData.ExpandInfos.Count;
				Span<IntPair> span = new Span<IntPair>(stackalloc byte[checked(unchecked((UIntPtr)count) * (UIntPtr)sizeof(IntPair))], count);
				span2 = span;
			}
			else
			{
				span2 = Span<IntPair>.Empty;
			}
			Span<IntPair> values = span2;
			calculate.parent.Find("CalculateTitle").gameObject.SetActive(values.Length > 0);
			template.gameObject.SetActive(false);
			while (calculate.childCount - 1 > values.Length)
			{
				Object.DestroyImmediate(calculate.GetChild(calculate.childCount - 1).gameObject);
			}
			while (calculate.childCount - 1 < values.Length)
			{
				Transform child2 = Object.Instantiate<Transform>(template, calculate, true);
				child2.localScale = Vector3.one;
				child2.gameObject.SetActive(true);
			}
			List<int> managers = this._buildingModel.GetBuildingShopManager(this._blockKey);
			bool hasLeader = (managers.CheckIndex(0) && managers[0] >= 0) || !this._configData.NeedLeader;
			int i = 0;
			int len = values.Length;
			while (i < len)
			{
				BuildingScaleItem scale = BuildingScale.Instance[this._configData.ExpandInfos[i]];
				Transform child = calculate.GetChild(i + 1);
				TextMeshProUGUI labelName = child.Find("Text").GetComponent<TextMeshProUGUI>();
				TextMeshProUGUI labelValue = child.Find("Value").GetComponent<TextMeshProUGUI>();
				labelName.text = scale.Name;
				sbyte level = this._buildingModel.GetBuildingLevel(this._blockKey, this._blockData);
				bool flag2 = !hasLeader;
				int value;
				if (flag2)
				{
					value = 0;
				}
				else
				{
					bool flag3 = scale.Formula >= 0;
					if (flag3)
					{
						bool flag4 = this._configData.Class == EBuildingBlockClass.BornResource;
						if (flag4)
						{
							bool flag5 = this._resourceBlockRanking < 5;
							if (flag5)
							{
								int percentage = GameData.Domains.Building.SharedMethods.GetResourceBlockEffectPercentage(this._resourceBlockRanking);
								int baseValue = GameData.Domains.Building.SharedMethods.GetResourceBlockEffectPercentageValue((int)level, percentage);
								value = BuildingFormula.Instance[scale.Formula].Calculate(baseValue);
							}
							else
							{
								value = 0;
							}
						}
						else
						{
							bool flag6 = this._formulaContext != null && this._formulaContext.BlockKey.Equals(this._blockKey);
							if (flag6)
							{
								value = BuildingFormula.Instance[scale.Formula].Calculate(this._formulaContext);
							}
							else
							{
								value = 0;
							}
						}
					}
					else
					{
						int levelEffectIndex = (int)(level - 1);
						value = ((scale.LevelEffect != null && scale.LevelEffect.CheckIndex(levelEffectIndex)) ? scale.LevelEffect[levelEffectIndex] : 0);
					}
				}
				bool flag7 = !hasLeader && this._configData.IsShop;
				if (flag7)
				{
					labelValue.text = "-";
				}
				else
				{
					labelValue.text = UI_BuildingManage.GetBuildingScaleFormatString(scale.Type, value);
					bool flag8 = scale.Type == EBuildingScaleType.Maintaince;
					if (flag8)
					{
						labelValue.text = string.Format("{0}", value).SetColor("brightred");
					}
				}
				bool flag9 = scale.TemplateId == 109;
				if (flag9)
				{
					child.gameObject.SetActive(false);
					ExtraDomainMethod.AsyncCall.GetIsJiaoPoolOpen(this, delegate(int offset, RawDataPool dataPool)
					{
						bool isOpen = false;
						Serializer.Deserialize(dataPool, offset, ref isOpen);
						child.gameObject.SetActive(isOpen);
					});
				}
				i++;
			}
		}
	}

	// Token: 0x060018ED RID: 6381 RVA: 0x0009F8C4 File Offset: 0x0009DAC4
	public static string GetBuildingScaleFormatString(EBuildingScaleType type, int value)
	{
		string displayStr = value.ToString();
		bool flag = type == EBuildingScaleType.Percentage;
		if (flag)
		{
			displayStr = value.ToString() + "%";
		}
		else
		{
			bool flag2 = type == EBuildingScaleType.BonusPercentage;
			if (flag2)
			{
				displayStr = ((value < 0) ? string.Format("{0}%", value) : string.Format("+{0}%", value));
			}
			else
			{
				bool flag3 = type == EBuildingScaleType.MovePoint;
				if (flag3)
				{
					displayStr = ((float)value / 10f).ToString("0.0");
				}
				else
				{
					bool flag4 = type == EBuildingScaleType.ReducePercentage;
					if (flag4)
					{
						displayStr = "-" + value.ToString() + "%";
					}
				}
			}
		}
		return displayStr;
	}

	// Token: 0x060018EE RID: 6382 RVA: 0x0009F970 File Offset: 0x0009DB70
	private void UpdateDependBuildings()
	{
		List<short> dependBuildings = this._configData.DependBuildings;
		bool showDependBuildings = dependBuildings != null && dependBuildings.Count > 0;
		this._buildingInfoPage.CGet<GameObject>("DependBuildings").SetActive(showDependBuildings);
		RectTransform content = this._buildingInfoPage.CGet<RectTransform>("DependBuildingsContent");
		BuildingInfoView buildingInfoPrefab = this._buildingInfoPage.CGet<BuildingInfoView>("BuildingInfoPrefab");
		for (int i = 0; i < content.childCount; i++)
		{
			content.GetChild(i).gameObject.SetActive(false);
		}
		bool flag = showDependBuildings;
		if (flag)
		{
			for (int j = 0; j < this._configData.DependBuildings.Count; j++)
			{
				short templateId = this._configData.DependBuildings[j];
				bool flag2 = templateId == -1;
				if (!flag2)
				{
					bool flag3 = j >= content.childCount;
					BuildingInfoView buildingInfoView;
					if (flag3)
					{
						buildingInfoView = Object.Instantiate<BuildingInfoView>(buildingInfoPrefab, content, false);
						buildingInfoView.OnClickBuildingInfoView += this.OnClickBuildingInfoView;
					}
					else
					{
						buildingInfoView = content.GetChild(j).GetComponent<BuildingInfoView>();
					}
					buildingInfoView.UpdateBuildingInfo(templateId, j, this.IsBuildingTemplateIdEnabled(templateId));
					buildingInfoView.gameObject.SetActive(true);
				}
			}
		}
	}

	// Token: 0x060018EF RID: 6383 RVA: 0x0009FACC File Offset: 0x0009DCCC
	private ValueTuple<UI_BuildingManage.EBuildingNotAvailableType, string> IsBuildingTemplateIdEnabled(short templateId)
	{
		BuildingBlockItem config = BuildingBlock.Instance[templateId];
		UI_BuildingManage.EBuildingNotAvailableType buildingBotAvailableType = UI_BuildingManage.EBuildingNotAvailableType.None;
		LanguageKey resultLanguageKey = LanguageKey.LK_BuildingInfoView_Tip;
		ViewBuildingArea buildingArea = UIElement.BuildingArea.UiBaseAs<ViewBuildingArea>();
		bool flag = config.IsUnique && buildingArea.ContainsBuilding(config.TemplateId, false);
		if (flag)
		{
			buildingBotAvailableType = UI_BuildingManage.EBuildingNotAvailableType.BuildConditionNotMet;
		}
		Dictionary<short, bool> dependBuildingDict;
		bool flag2 = !buildingArea.CanBuildAnywhere(config, out dependBuildingDict);
		if (flag2)
		{
			buildingBotAvailableType = UI_BuildingManage.EBuildingNotAvailableType.BuildConditionNotMet;
			foreach (short id in config.DependBuildings)
			{
				bool hasBuilding;
				bool flag3 = dependBuildingDict != null && dependBuildingDict.TryGetValue(id, out hasBuilding) && hasBuilding;
				if (!flag3)
				{
					BuildingBlockItem buildingConfig = BuildingBlock.Instance[id];
					bool flag4 = buildingConfig.Type == EBuildingBlockType.Building;
					if (!flag4)
					{
						EBuildingBlockType type = buildingConfig.Type;
						bool flag5 = type == EBuildingBlockType.NormalResource || type == EBuildingBlockType.SpecialResource;
						if (flag5)
						{
						}
					}
				}
			}
		}
		bool flag6 = !CommonUtils.IsBuildingCostResourcesEnough(config);
		if (flag6)
		{
			buildingBotAvailableType = UI_BuildingManage.EBuildingNotAvailableType.BuildConditionNotMet;
		}
		bool flag7 = ViewBuildingOverview.SpaceMeet(config, this._buildingSpaceLimit - this._buildingSpaceCurr);
		if (flag7)
		{
			buildingBotAvailableType = UI_BuildingManage.EBuildingNotAvailableType.BuildConditionNotMet;
		}
		bool flag8 = config.BuildingCoreItem != -1;
		if (flag8)
		{
			bool hasBuildingCore = GameData.Domains.Building.SharedMethods.HasBuildingCore(config, (this._cannotUseInventoryBuildingCore != null) ? this._canUseBuildingCore.Concat(this._cannotUseInventoryBuildingCore).ToList<ItemDisplayData>() : this._canUseBuildingCore).Item1;
			bool flag9 = !hasBuildingCore;
			if (flag9)
			{
				buildingBotAvailableType = UI_BuildingManage.EBuildingNotAvailableType.BuildConditionNotMet;
			}
		}
		bool isUnlock = this.CanUnlockBuildingByLifeSkill(config) && CommonUtils.CanUnlockBuildingByMainProgress(config);
		bool flag10 = !isUnlock;
		if (flag10)
		{
			buildingBotAvailableType = UI_BuildingManage.EBuildingNotAvailableType.Locked;
		}
		return new ValueTuple<UI_BuildingManage.EBuildingNotAvailableType, string>(buildingBotAvailableType, LocalStringManager.Get(resultLanguageKey));
	}

	// Token: 0x060018F0 RID: 6384 RVA: 0x0009FCA0 File Offset: 0x0009DEA0
	private void OnClickBuildingInfoView(short buildingTemplateId, int index)
	{
		this.QuickHide();
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
		argumentBox.Clear();
		argumentBox.Set("isHaveChickenKing", UI_Bottom._isHaveChickenKing);
		argumentBox.Set("AutoSelectBuildingTemplateId", buildingTemplateId);
		UIElement.BuildingOverview.SetOnInitArgs(argumentBox);
		UIManager.Instance.ShowUI(UIElement.BuildingOverview, true);
	}

	// Token: 0x060018F1 RID: 6385 RVA: 0x0009FD00 File Offset: 0x0009DF00
	private void UpdateExpandBuildings()
	{
		List<short> expandBuildings = this._configData.ExpandBuildings;
		bool showExpandBuildings = expandBuildings != null && expandBuildings.Count > 0;
		this._buildingInfoPage.CGet<GameObject>("ExpandBuildings").SetActive(showExpandBuildings);
		RectTransform content = this._buildingInfoPage.CGet<RectTransform>("ExpandBuildingsContent");
		BuildingInfoView buildingInfoPrefab = this._buildingInfoPage.CGet<BuildingInfoView>("BuildingInfoPrefab");
		for (int i = 0; i < content.childCount; i++)
		{
			content.GetChild(i).gameObject.SetActive(false);
		}
		bool flag = showExpandBuildings;
		if (flag)
		{
			for (int j = 0; j < this._configData.ExpandBuildings.Count; j++)
			{
				short templateId = this._configData.ExpandBuildings[j];
				bool flag2 = templateId == -1;
				if (!flag2)
				{
					bool flag3 = j >= content.childCount;
					BuildingInfoView buildingInfoView;
					if (flag3)
					{
						buildingInfoView = Object.Instantiate<BuildingInfoView>(buildingInfoPrefab, content, false);
						buildingInfoView.OnClickBuildingInfoView += this.OnClickBuildingInfoView;
					}
					else
					{
						buildingInfoView = content.GetChild(j).GetComponent<BuildingInfoView>();
					}
					buildingInfoView.UpdateBuildingInfo(templateId, j, this.IsBuildingTemplateIdEnabled(templateId));
					buildingInfoView.gameObject.SetActive(true);
				}
			}
		}
	}

	// Token: 0x060018F2 RID: 6386 RVA: 0x0009FE5C File Offset: 0x0009E05C
	private void UpdateAutoCheckIn()
	{
		bool isResidence = this._configData.TemplateId == 46;
		TooltipInvoker autoCheckInMouseTip = this._autoCheckInMouseTip;
		if (autoCheckInMouseTip.RuntimeParam == null)
		{
			autoCheckInMouseTip.RuntimeParam = EasyPool.Get<ArgumentBox>();
		}
		this._autoCheckInMouseTip.Type = TipType.SingleDesc;
		this._autoCheckInMouseTip.RuntimeParam.Set("arg0", isResidence ? LocalStringManager.Get("LK_Building_AutoCheckInTip2") : LocalStringManager.Get("LK_Building_AutoCheckInTip1"));
		UnityAction<bool> <>9__1;
		base.AsyncMethodCall<short>(9, isResidence ? 105 : 104, this._buildingBlockIndex, delegate(int offset, RawDataPool dataPool)
		{
			bool isAutoArrange = false;
			Serializer.Deserialize(dataPool, offset, ref isAutoArrange);
			this._autoCheckInToggle.onValueChanged.RemoveAllListeners();
			this._autoCheckInToggle.isOn = isAutoArrange;
			UnityEvent<bool> onValueChanged = this._autoCheckInToggle.onValueChanged;
			UnityAction<bool> call;
			if ((call = <>9__1) == null)
			{
				call = (<>9__1 = delegate(bool isOn)
				{
					bool isResidence = isResidence;
					if (isResidence)
					{
						BuildingDomainMethod.Call.SetResidenceAutoCheckIn(this._buildingBlockIndex, isOn);
					}
					else
					{
						BuildingDomainMethod.Call.SetComfortableAutoCheckIn(this._buildingBlockIndex, isOn);
					}
				});
			}
			onValueChanged.AddListener(call);
		});
	}

	// Token: 0x060018F3 RID: 6387 RVA: 0x0009FF14 File Offset: 0x0009E114
	private void ShowFixedBuildingInfo()
	{
		CImage buildingIcon = this._leftInfo.CGet<CImage>("BuildingIcon");
		ViewBuildingArea.SetBuildingIcon(buildingIcon, this._configData, false, null);
		buildingIcon.GetComponent<ImageAspectKeeper>().Refresh();
		buildingIcon.gameObject.SetActive(!ViewBuildingArea.IsShowAnimation(this._blockData.TemplateId));
		GameObject animationHolder = this._leftInfo.CGet<GameObject>("AnimationHolder");
		bool flag = ViewBuildingArea.IsShowAnimation(this._blockData.TemplateId);
		if (flag)
		{
			ResLoader.Load<GameObject>(ViewBuildingArea.GetBuildingAnimationPrefabPath(this._blockData.TemplateId), delegate(GameObject obj)
			{
				GameObject go = Object.Instantiate<GameObject>(obj, animationHolder.transform, false);
				this._animationGoList.Add(go);
			}, null, false);
		}
		ViewBuildingArea.SetBuildingName(this._leftInfo.CGet<TextMeshProUGUI>("BuildingName"), this._configData, this._blockKey, this._blockTemplateId, true);
		CButtonObsolete renameMouseTip = this._leftInfo.CGet<CButtonObsolete>("RenameBtn");
		bool canRename = SingletonObject.getInstance<WorldMapModel>().IsAtTaiwuVillage(this._areaId, this._blockId) && BuildingBlockData.IsBuilding(this._configData.Type);
		renameMouseTip.interactable = canRename;
		renameMouseTip.GetComponent<PointerTrigger>().enabled = canRename;
		TooltipInvoker component = renameMouseTip.GetComponent<TooltipInvoker>();
		ArgumentBox argumentBox;
		if ((argumentBox = component.RuntimeParam) == null)
		{
			argumentBox = (component.RuntimeParam = new ArgumentBox());
		}
		ArgumentBox runtimeParam = argumentBox;
		runtimeParam.Set("arg0", LocalStringManager.Get(canRename ? LanguageKey.LK_Building_Rename : LanguageKey.LK_Building_CannotRename));
		renameMouseTip.GetComponent<DisableStyleRoot>().SetStyleEffect(!canRename, false);
		this._leftInfo.CGet<TMP_InputField>("CustomNameInput").gameObject.SetActive(false);
		this._leftInfo.CGet<CImage>("LevelIcon").SetSprite(GameData.Domains.Building.SharedMethods.HaveUsefulResourceBlockEffect(this._configData.TemplateId, this._resourceBlockRanking) ? "building_rank_light" : "building_dengji", false, null);
		this._leftInfo.CGet<CImage>("BuildingWidth").SetSprite((this._configData.Width == 2) ? "building_zhange_1" : "building_zhange_0", false, null);
		this._buildingInfoPage.CGet<GameObject>("MaintainInfo").SetActive(this._configData.BaseMaintenanceCost.Count > 0 && this._blockData.OperationType != 0 && this._isTaiwuVillageBuilding);
		bool flag2 = this._configData.BaseMaintenanceCost.Count > 0 && this._blockData.OperationType != 0;
		if (flag2)
		{
			this._buildingInfoPage.CGet<CImage>("MaintainIcon").SetSprite(Config.ResourceType.Instance[this._configData.BaseMaintenanceCost[0].ResourceType].Icon, false, null);
			int[] costArray = GameData.Domains.Building.SharedMethods.GetFinalMaintenanceCost(this._configData);
			int cost = costArray[(int)this._configData.BaseMaintenanceCost[0].ResourceType];
			string text = string.Format("{0}/{1}", cost, LocalStringManager.Get(LanguageKey.LK_Month));
			this._buildingInfoPage.CGet<TextMeshProUGUI>("MaintainName").SetText(text, true);
		}
		this.RefreshBasicInfoSize();
	}

	// Token: 0x060018F4 RID: 6388 RVA: 0x000A023C File Offset: 0x0009E43C
	private void RefreshBasicInfoSize()
	{
		RectTransform basicInfo = this._buildingInfoPage.CGet<RectTransform>("BasicInfo");
		basicInfo.sizeDelta = (this._btnHolder.gameObject.activeSelf ? new Vector2(basicInfo.sizeDelta.x, 534f) : new Vector2(basicInfo.sizeDelta.x, 673f));
		basicInfo.anchoredPosition = (this._btnHolder.gameObject.activeSelf ? new Vector2(basicInfo.anchoredPosition.x, -406f) : new Vector2(basicInfo.anchoredPosition.x, -336f));
	}

	// Token: 0x060018F5 RID: 6389 RVA: 0x000A02E8 File Offset: 0x0009E4E8
	private void UpdateButtonMouseTip()
	{
		int togKey = this._mainToggleGroup.GetActive().Key;
		bool flag = togKey == 1;
		if (!flag)
		{
			CButtonObsolete confirmBtn = base.CGet<CButtonObsolete>("ConfirmBtn");
			Refers confirmBtnRefers = confirmBtn.GetComponent<Refers>();
			CButtonObsolete cancelBtn = base.CGet<CButtonObsolete>("CancelBtn");
			sbyte operationType = this.TogKeyToOperationType(togKey);
			bool flag2 = this._blockData.OperationType != operationType;
			if (flag2)
			{
				string tipDesc;
				bool canOperate = this.CanExecuteOperation(operationType, out tipDesc);
				TooltipInvoker tipDisplayer = confirmBtn.GetComponent<TooltipInvoker>();
				confirmBtn.interactable = canOperate;
				confirmBtnRefers.CGet<DisableStyleRoot>("ConfirmBtn").SetStyleEffect(!confirmBtn.interactable, false);
				CImage confirmIcon = confirmBtnRefers.CGet<CImage>("ConfirmIcon");
				bool flag3 = togKey == 3;
				if (flag3)
				{
					confirmIcon.SetSprite(canOperate ? "building_icon_xiaochaichu" : "building_icon_xiaochaichuhui", false, null);
				}
				tipDisplayer.Type = TipType.Simple;
				tipDisplayer.PresetParam[0] = LocalStringManager.Get(this.StartOperationTipTitleKey[(int)operationType]);
				tipDisplayer.PresetParam[1] = tipDesc;
			}
			else
			{
				confirmBtn.interactable = true;
				confirmBtnRefers.CGet<DisableStyleRoot>("ConfirmBtn").SetStyleEffect(false, false);
				CImage confirmIcon2 = confirmBtnRefers.CGet<CImage>("ConfirmIcon");
				bool flag4 = this._blockData.OperationType == 1;
				if (flag4)
				{
					confirmIcon2.SetSprite("building_icon_xiaochaichu", false, null);
				}
				bool operationStopping = this._blockData.OperationStopping;
				if (operationStopping)
				{
					TooltipInvoker tipDisplayer2 = confirmBtn.GetComponent<TooltipInvoker>();
					tipDisplayer2.Type = TipType.SingleDesc;
					tipDisplayer2.PresetParam[0] = LocalStringManager.Get(this.ContinueOperationTipTitleKey[(int)operationType]);
				}
				else
				{
					TutorialChapterModel tutorialModel = SingletonObject.getInstance<TutorialChapterModel>();
					bool flag5 = !tutorialModel.GetFunctionStatus(1) && operationType == 0;
					if (flag5)
					{
						cancelBtn.gameObject.SetActive(false);
					}
					else
					{
						cancelBtn.gameObject.SetActive(true);
						TooltipInvoker tipDisplayer3 = cancelBtn.GetComponent<TooltipInvoker>();
						tipDisplayer3.Type = TipType.SingleDesc;
						bool isUsefulResource = BuildingBlockData.IsUsefulResource(this._configData.Type);
						bool flag6 = operationType == 1 || (operationType == 0 && isUsefulResource);
						if (flag6)
						{
							tipDisplayer3.PresetParam[0] = LocalStringManager.Get(this.StopOperationTipTitleKey[(int)operationType]);
						}
						else
						{
							tipDisplayer3.PresetParam[0] = LocalStringManager.Get(this.StopOperationTipDescKey[(int)operationType]);
						}
					}
				}
			}
		}
	}

	// Token: 0x060018F6 RID: 6390 RVA: 0x000A0530 File Offset: 0x0009E730
	private void SetResourceInfo(sbyte resourceType, Refers resourceRefers)
	{
		CImage resourceIcon = resourceRefers.CGet<CImage>("ResourceIcon");
		TextMeshProUGUI resourceName = resourceRefers.CGet<TextMeshProUGUI>("ResourceName");
		TextMeshProUGUI resourceCount = resourceRefers.CGet<TextMeshProUGUI>("ResourceCount");
		resourceIcon.SetSprite(Config.ResourceType.Instance[resourceType].Icon, false, null);
		resourceName.SetText(Config.ResourceType.Instance[resourceType].Name, true);
		BuildingDomainMethod.AsyncCall.CalcResourceOutputCount(this, this._blockKey, resourceType, delegate(int offset, RawDataPool dataPool)
		{
			int resourceAdd = 0;
			Serializer.Deserialize(dataPool, offset, ref resourceAdd);
			this._stringBuilder.Clear();
			this._stringBuilder.Append(this._buildingModel.GetResourceCount(resourceType)).Append(" +").Append(resourceAdd.ToString().SetColor("brightblue"));
			resourceCount.text = this._stringBuilder.ToString();
		});
	}

	// Token: 0x060018F7 RID: 6391 RVA: 0x000A05D8 File Offset: 0x0009E7D8
	private void ConfirmOperation()
	{
		sbyte operationType = this.TogKeyToOperationType(this._mainToggleGroup.GetActive().Key);
		sbyte b = operationType;
		sbyte b2 = b;
		if (b2 == 1)
		{
			AudioManager.Instance.PlaySound("ui_industry_dismantle", false, false);
			this._blockData.OperationType = operationType;
			BuildingDomainMethod.Call.Remove(this.Element.GameDataListenerId, this._blockKey, this._operatorListCached);
		}
	}

	// Token: 0x060018F8 RID: 6392 RVA: 0x000A0644 File Offset: 0x0009E844
	private void CancelOperation()
	{
		bool flag = this._blockData.OperationType == 0;
		if (flag)
		{
			BuildingDomainMethod.Call.SetStopOperation(UIElement.BuildingArea.GameDataListenerId, this._blockKey, true);
			this.QuickHide();
		}
		else
		{
			BuildingDomainMethod.Call.SetStopOperation(this.Element.GameDataListenerId, this._blockKey, true);
		}
	}

	// Token: 0x060018F9 RID: 6393 RVA: 0x000A06A0 File Offset: 0x0009E8A0
	private void ContinueOperation()
	{
		BuildingDomainMethod.Call.SetStopOperation(this.Element.GameDataListenerId, this._blockKey, false);
	}

	// Token: 0x060018FA RID: 6394 RVA: 0x000A06BC File Offset: 0x0009E8BC
	private void UpdateProgresInfo()
	{
		bool flag = this._fillBg == null;
		if (flag)
		{
			this._fillBg = base.CGet<GameObject>("ProgressBg");
		}
		bool flag2 = this._fillImg == null;
		if (flag2)
		{
			this._fillImg = base.CGet<CImage>("ProgressFill");
		}
		bool flag3 = this._fillText == null;
		if (flag3)
		{
			this._fillText = base.CGet<TextMeshProUGUI>("ProgressText");
		}
		this._fillBg.gameObject.SetActive(this._blockData.OperationType != -1);
		bool flag4 = this._blockData.OperationType == -1;
		if (!flag4)
		{
			short progress = this._blockData.OperationProgress;
			short totalProgress = this._configData.OperationTotalProgress[(int)this._blockData.OperationType];
			this._fillImg.fillAmount = (float)progress / (float)totalProgress;
			this._stringBuilder.Clear();
			bool flag5 = this._blockData.OperationType == 0;
			if (flag5)
			{
				this._stringBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Building_BuildProgress));
			}
			else
			{
				bool flag6 = this._blockData.OperationType == 1;
				if (flag6)
				{
					this._stringBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Building_RemoveProgress));
				}
			}
			this._stringBuilder.Append(progress).Append("/").Append(totalProgress);
			this._fillText.SetText(this._stringBuilder.ToString(), true);
		}
	}

	// Token: 0x060018FB RID: 6395 RVA: 0x000A0840 File Offset: 0x0009EA40
	private bool CanExecuteOperation(sbyte operationType, out string tipDesc)
	{
		bool result = true;
		StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
		strBuilder.Clear();
		bool flag = !this._operatorListCached.Exist((int id) => id >= 0);
		if (flag)
		{
			result = false;
			strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Cannot_Build_No_Operator).ColorReplace() + "\n");
		}
		bool flag2 = operationType == 0;
		if (flag2)
		{
			BuildingBlockItem configData = this.GetConfigData();
			ViewBuildingArea buildingArea = UIElement.BuildingArea.UiBaseAs<ViewBuildingArea>();
			List<short> dependBuildings = configData.DependBuildings;
			bool hasAllDependBuildings = true;
			sbyte dependBuildingMinLevel = sbyte.MaxValue;
			int dependBuildingMinLevelIndex = -1;
			short[] dependBuildingTemplateId = null;
			bool flag3 = dependBuildings.Count > 0;
			if (flag3)
			{
				List<BuildingBlockData> neighborList = new List<BuildingBlockData>();
				bool[] dependBuildingFound = new bool[dependBuildings.Count];
				sbyte[] dependBuildingLevel = new sbyte[dependBuildings.Count];
				dependBuildingTemplateId = new short[dependBuildings.Count];
				buildingArea.GetNeighborBlocks(this._buildingBlockIndex, ref neighborList, configData.Width, 2, null);
				for (int i = 0; i < neighborList.Count; i++)
				{
					BuildingBlockData neighborBlock = neighborList[i];
					int dependIndex = dependBuildings.IndexOf(neighborBlock.TemplateId);
					bool flag4 = dependIndex >= 0 && neighborBlock.CanUse();
					if (flag4)
					{
						BuildingBlockKey blockKey = new BuildingBlockKey(this._areaId, this._blockId, neighborBlock.BlockIndex);
						sbyte buildingLevel = this._buildingModel.GetBuildingLevel(blockKey, neighborBlock);
						dependBuildingFound[dependIndex] = true;
						dependBuildingLevel[dependIndex] = Math.Max(dependBuildingLevel[dependIndex], buildingLevel);
						dependBuildingTemplateId[dependIndex] = neighborBlock.TemplateId;
					}
				}
				hasAllDependBuildings = !dependBuildingFound.Exist(false);
				dependBuildingMinLevel = dependBuildingLevel.Min<sbyte>();
				dependBuildingMinLevelIndex = dependBuildingLevel.IndexOf(dependBuildingMinLevel);
			}
			bool flag5 = !this._resourceEnough;
			if (flag5)
			{
				result = false;
				strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Cannot_Build_Resource_Not_Enough).ColorReplace() + "\n");
			}
			bool flag6 = operationType == 0;
			if (flag6)
			{
				bool flag7 = !hasAllDependBuildings;
				if (flag7)
				{
					result = false;
					strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Cannot_Build_No_Depend_Building).ColorReplace() + "\n");
				}
				bool flag8 = this._configData.IsUnique && buildingArea.ContainsBuilding(this._configData.TemplateId, false);
				if (flag8)
				{
					result = false;
					strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Cannot_Build_Unique_Building).ColorReplace() + "\n");
				}
			}
			else
			{
				bool flag9 = this._configData.MaxLevel >= dependBuildingMinLevel;
				if (flag9)
				{
					bool flag10 = dependBuildingTemplateId != null;
					if (flag10)
					{
						short templateId = dependBuildingTemplateId[dependBuildingMinLevelIndex];
						BuildingBlockItem config = BuildingBlock.Instance[templateId];
						bool isResource = BuildingBlockData.IsResource(config.Type);
						result = false;
						strBuilder.Append(LocalStringManager.Get((!isResource) ? LanguageKey.LK_Cannot_Build_Reach_Depend_Max_Level_Tip1 : LanguageKey.LK_Cannot_Build_Reach_Depend_Max_Level_Tip2).ColorReplace());
						strBuilder.Append("\n");
					}
				}
			}
		}
		strBuilder.Append(LocalStringManager.Get(this.StartOperationTipDescKey[(int)operationType]));
		tipDesc = strBuilder.ToString();
		EasyPool.Free<StringBuilder>(strBuilder);
		return result;
	}

	// Token: 0x060018FC RID: 6396 RVA: 0x000A0B74 File Offset: 0x0009ED74
	private void SetOperationLeftTimeString(List<int> operators)
	{
		sbyte operationType = this.TogKeyToOperationType(this._mainToggleGroup.GetActive().Key);
		TextMeshProUGUI leftTimeText = this._removeCollectInfoPage.CGet<TextMeshProUGUI>("ExpandCostTimeText");
		BuildingBlockItem configData = this.GetConfigData();
		bool flag = configData == null || configData.TemplateId == 0;
		if (flag)
		{
			leftTimeText.text = "-";
		}
		else
		{
			bool flag2 = this._blockData.OperationType != -1 && this._blockData.OperationStopping;
			if (flag2)
			{
				leftTimeText.text = "1";
			}
			else
			{
				BuildingDomainMethod.AsyncCall.GetOperationLeftTime(this, this._configData.TemplateId, this._blockKey, operationType, operators, delegate(int offset, RawDataPool dataPool)
				{
					int leftTime = 0;
					Serializer.Deserialize(dataPool, offset, ref leftTime);
					leftTimeText.text = ((leftTime > 0) ? leftTime.ToString() : "-");
				});
			}
		}
	}

	// Token: 0x060018FD RID: 6397 RVA: 0x000A0C40 File Offset: 0x0009EE40
	private int OperationTypeToTogKey(sbyte operationType)
	{
		bool flag = operationType == 0;
		int result;
		if (flag)
		{
			result = 0;
		}
		else
		{
			result = (int)(operationType + 2);
		}
		return result;
	}

	// Token: 0x060018FE RID: 6398 RVA: 0x000A0C64 File Offset: 0x0009EE64
	private sbyte TogKeyToOperationType(int togKey)
	{
		bool flag = togKey == 3;
		sbyte result;
		if (flag)
		{
			result = 1;
		}
		else
		{
			result = (sbyte)togKey;
		}
		return result;
	}

	// Token: 0x060018FF RID: 6399 RVA: 0x000A0C84 File Offset: 0x0009EE84
	private BuildingBlockItem GetConfigData()
	{
		return this._configData;
	}

	// Token: 0x06001900 RID: 6400 RVA: 0x000A0C9C File Offset: 0x0009EE9C
	private sbyte GetOperationNeedSkillType()
	{
		bool isCollectResourceBuilding = this._configData.IsCollectResourceBuilding;
		sbyte lifeSkillType;
		if (isCollectResourceBuilding)
		{
			sbyte resourceType = this._buildingModel.GetCollectBuildingResourceTypeWithToxicology(this._blockKey, this._blockData);
			lifeSkillType = this._configData.RequireLifeSkillType;
		}
		else
		{
			lifeSkillType = this._configData.RequireLifeSkillType;
		}
		return lifeSkillType;
	}

	// Token: 0x06001901 RID: 6401 RVA: 0x000A0CF8 File Offset: 0x0009EEF8
	private void OnBuildingBlockUpdate(ArgumentBox argBox)
	{
		BuildingBlockKey blockKey;
		argBox.Get<BuildingBlockKey>("BuildingBlockKey", out blockKey);
		bool flag = blockKey.AreaId == this._areaId && blockKey.BlockId == this._blockId && blockKey.BuildingBlockIndex == this._buildingBlockIndex;
		if (flag)
		{
			base.StartCoroutine(this.DelayRefresh());
		}
	}

	// Token: 0x06001902 RID: 6402 RVA: 0x000A0D54 File Offset: 0x0009EF54
	private IEnumerator DelayRefresh()
	{
		yield return new WaitForSeconds(0.05f);
		this.UpdateBuildingData();
		this.UpdateToggles();
		this.UpdateCurrentPage();
		yield break;
	}

	// Token: 0x06001903 RID: 6403 RVA: 0x000A0D64 File Offset: 0x0009EF64
	private void InitRefers()
	{
		bool refersInitialized = this._refersInitialized;
		if (!refersInitialized)
		{
			this._arrangementSettingPanel = base.CGet<Refers>("ArrangementSettingPanel");
			this._autoCheckInToggleLabel = base.CGet<TextMeshProUGUI>("AutoCheckInToggleLabel");
			this._btnTemplate = base.CGet<GameObject>("BtnTemplate");
			this._buildingInfoPage = base.CGet<Refers>("BuildingInfoPage");
			this._buttonHolder = base.CGet<RectTransform>("ButtonHolder");
			this._buttonHolderLine = base.CGet<GameObject>("ButtonHolderLine");
			this._buttonLayoutGroup = base.CGet<GameObject>("ButtonLayoutGroup");
			this._cancelBtn = base.CGet<CButtonObsolete>("CancelBtn");
			this._confirmBtn = base.CGet<CButtonObsolete>("ConfirmBtn");
			this._damageInfoPage = base.CGet<Refers>("DamageInfoPage");
			this._entertainPage = base.CGet<Refers>("EntertainPage");
			this._expandInfoPage = base.CGet<GameObject>("ExpandInfoPage");
			this._expandManpowerHolder = base.CGet<GameObject>("ExpandManpowerHolder");
			this._expandRemoveCollectInfoPage = base.CGet<Refers>("ExpandRemoveCollectInfoPage");
			this._getItemTips = base.CGet<CanvasGroup>("GetItemTips");
			this._getItemTipsText = base.CGet<TextMeshProUGUI>("GetItemTipsText");
			this._infoPage = base.CGet<Refers>("InfoPage");
			this._leftInfo = base.CGet<Refers>("LeftInfo");
			this._mainToggleGroup = base.CGet<CToggleGroupObsolete>("MainToggleGroup");
			this._needResourceHolder = base.CGet<Refers>("NeedResourceHolder");
			this._noContent = base.CGet<GameObject>("NoContent");
			this._progressBg = base.CGet<GameObject>("ProgressBg");
			this._progressFill = base.CGet<CImage>("ProgressFill");
			this._progressText = base.CGet<TextMeshProUGUI>("ProgressText");
			this._repairBuildingBtn = base.CGet<CButtonObsolete>("RepairBuildingBtn");
			this._residentInfo = base.CGet<GameObject>("ResidentInfo");
			this._residentViewQuickText = base.CGet<TextMeshProUGUI>("ResidentViewQuickText");
			this._residentsLayoutGroup = base.CGet<GameObject>("ResidentsLayoutGroup");
			this._rightInfo = base.CGet<GameObject>("RightInfo");
			this._rightTitle = base.CGet<TextMeshProUGUI>("RightTitle");
			this._shopInfoPage = base.CGet<Refers>("ShopInfoPage");
			this._soldItemSettingPanel = base.CGet<Refers>("SoldItemSettingPanel");
			this._topInfo = base.CGet<GameObject>("TopInfo");
			this._refersInitialized = true;
		}
	}

	// Token: 0x170002A9 RID: 681
	// (get) Token: 0x06001904 RID: 6404 RVA: 0x000A0FBC File Offset: 0x0009F1BC
	private bool IsEntertain
	{
		get
		{
			BuildingBlockItem configData = this._configData;
			return ((configData != null) ? new short?(configData.TemplateId) : null) == 47;
		}
	}

	// Token: 0x06001905 RID: 6405 RVA: 0x000A1000 File Offset: 0x0009F200
	private void RefreshEntertainPage()
	{
		short type = this._feast.GetFeastType();
		bool showTitle = type != 0;
		this._entertainPage.CGet<GameObject>("TitleBack").SetActive(showTitle);
		bool flag = showTitle;
		if (flag)
		{
			FeastItem config = Config.Feast.Instance[type];
			this._entertainPage.CGet<TextMeshProUGUI>("TitleText").text = config.Name;
			TooltipInvoker tip = this._entertainPage.CGet<TooltipInvoker>("TitleTip");
			tip.Type = TipType.BuildingFeast;
			TooltipInvoker tooltipInvoker = tip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			tip.RuntimeParam.Set("type", type);
		}
		CToggleObsolete toggleAuto = this._entertainPage.CGet<CToggleObsolete>("ToggleAuto");
		toggleAuto.onValueChanged.RemoveAllListeners();
		toggleAuto.isOn = this._feast.AutoRefill;
		toggleAuto.onValueChanged.AddListener(delegate(bool isOn)
		{
			ExtraDomainMethod.Call.FeastSetAutoRefill(this._blockKey, isOn);
		});
		CButtonObsolete buttonMenu = this._entertainPage.CGet<CButtonObsolete>("ButtonMenu");
		buttonMenu.ClearAndAddListener(delegate
		{
			ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("unlockedFeastTypes", this._unlockedFeastTypes);
			UIElement.BuildingFeastMenu.SetOnInitArgs(args);
			UIManager.Instance.ShowUI(UIElement.BuildingFeastMenu, true);
		});
		CButtonObsolete buttonQuick = this._entertainPage.CGet<CButtonObsolete>("ButtonQuick");
		buttonQuick.interactable = true;
		buttonQuick.ClearAndAddListener(delegate
		{
			ExtraDomainMethod.Call.FeastQuickRefill(this._blockKey);
			this.RefreshEntertainData();
		});
		this._feastDropdown = this._entertainPage.CGet<CDropdownLegacy>("FeastDropdown");
		this._feastDropdown.onValueChanged.RemoveAllListeners();
		this._feastDropdown.ClearOptions();
		List<string> options = (from id in this._unlockedFeastTypes
		where id != 0
		select Config.Feast.Instance[id].Name).Prepend(LanguageKey.LK_Building_Entertain_Dropdown_Invalid.Tr()).ToList<string>();
		this._feastDropdown.AddOptions(options);
		this._feastDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnMakeDropdownValueChanged));
		this._feastDropdown.value = this._unlockedFeastTypes.IndexOf(this._feast.TargetType) + 1;
		this._feastDropdownMask = this._entertainPage.CGet<GameObject>("FeastDropdownMask");
		this.ShowFeastDropdown(this._unlockedFeastTypes.Count > 0);
		Refers[] refersArray = this._entertainPage.CGet<GameObject>("FoodLayout").GetComponentsInChildren<Refers>();
		for (int i = 0; i < refersArray.Length; i++)
		{
			int index = i;
			ItemKey itemKey = this._feast.GetDish(index);
			Refers refers = refersArray[index];
			bool isValid = itemKey.IsValid();
			string text2;
			if (isValid)
			{
				sbyte itemType = itemKey.ItemType;
				if (!true)
				{
				}
				string text;
				if (itemType != 7)
				{
					if (itemType != 9)
					{
						text = string.Empty;
					}
					else
					{
						text = TeaWine.Instance[itemKey.TemplateId].BigIcon;
					}
				}
				else
				{
					text = Food.Instance[itemKey.TemplateId].BigIcon;
				}
				if (!true)
				{
				}
				text2 = text;
			}
			else
			{
				text2 = "building_villa_Emptydishes_0";
			}
			string bigIcon = text2;
			refers.CGet<CImage>("Image").SetSprite(bigIcon, false, null);
			refers.CGet<GameObject>("Hover").SetActive(false);
			refers.CGet<GameObject>("ValidRoot").SetActive(isValid);
			refers.CGet<GameObject>("InvalidRoot").SetActive(!isValid);
			refers.CGet<CButtonObsolete>("ButtonReplace").ClearAndAddListener(delegate
			{
				this.SelectEntertainTargetItem(index);
			});
			refers.CGet<CButtonObsolete>("ButtonAdd").ClearAndAddListener(delegate
			{
				this.SelectEntertainTargetItem(index);
			});
			bool flag2 = isValid;
			if (flag2)
			{
				string itemName = ItemTemplateHelper.GetName(itemKey.ItemType, itemKey.TemplateId);
				sbyte itemGrade = ItemTemplateHelper.GetGrade(itemKey.ItemType, itemKey.TemplateId);
				refers.CGet<TextMeshProUGUI>("NameText").text = itemName.SetGradeColor((int)itemGrade);
				refers.CGet<TextMeshProUGUI>("CountText").text = LocalStringManager.GetFormat(LanguageKey.LK_Building_Entertain_RemainCount, this._feast.DishDurability[index], GlobalConfig.Instance.FeastDurability);
			}
			UIParticle effect = refers.CGet<UIParticle>("Effect");
			effect.gameObject.SetActive(isValid);
		}
	}

	// Token: 0x06001906 RID: 6406 RVA: 0x000A1498 File Offset: 0x0009F698
	private void PlayEntertainEffectRandom()
	{
		UIParticle[] particles = this._entertainPage.CGet<GameObject>("FoodLayout").GetComponentsInChildren<UIParticle>();
		UIParticle[] array = particles;
		for (int i = 0; i < array.Length; i++)
		{
			UIParticle particle = array[i];
			particle.Stop();
			float random = Random.Range(0f, 0.2f);
			DOVirtual.DelayedCall(random, delegate
			{
				particle.Play();
			}, false);
		}
	}

	// Token: 0x06001907 RID: 6407 RVA: 0x000A1510 File Offset: 0x0009F710
	private void SelectEntertainTargetItem(int index)
	{
		int taiwuId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		List<ItemKey> list = new List<ItemKey>();
		for (int i = 0; i < GlobalConfig.Instance.FeastCount; i++)
		{
			ItemKey dish;
			bool flag = this._feast.Dish.TryGetValue(i, out dish) && dish.IsValid();
			if (flag)
			{
				list.Add(dish);
			}
		}
		ItemDomainMethod.AsyncCall.GetItemDisplayDataListOptional(this, list, taiwuId, delegate(int offset, RawDataPool dataPool)
		{
			List<ItemDisplayData> itemDisplayData = new List<ItemDisplayData>();
			Serializer.Deserialize(dataPool, offset, ref itemDisplayData);
			for (int j = 0; j < GlobalConfig.Instance.FeastCount; j++)
			{
				ItemKey dish2;
				bool flag2 = !this._feast.Dish.TryGetValue(j, out dish2) || !dish2.IsValid();
				if (flag2)
				{
					itemDisplayData.Insert(j, new ItemDisplayData());
				}
			}
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.SetObject("buildingEarningsData", null);
			argBox.Set("canTransfer", this._canTransfer);
			argBox.Set("BuildingTemplateId", this._configData.TemplateId);
			argBox.Set("currentIndex", index);
			argBox.SetObject("buildingBlockKey", this._blockKey);
			argBox.SetObject("warehouseList", this._warehouseCanSoldItemList);
			argBox.SetObject("inventoryList", this._inventoryCanSoldItemList);
			argBox.SetObject("treasuryList", this._treasuryCanSoldItemList);
			argBox.SetObject("feast", this._feast);
			argBox.SetObject("dishList", itemDisplayData);
			argBox.SetObject("callback", new Action(this.RefreshEntertainData));
			UIElement.MultiSelectItem.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.MultiSelectItem, true);
		});
	}

	// Token: 0x06001908 RID: 6408 RVA: 0x000A15A1 File Offset: 0x0009F7A1
	private void RefreshEntertainData()
	{
		this.FreshCanSoldItemList();
		ExtraDomainMethod.Call.GetFeast(this.Element.GameDataListenerId, this._blockKey);
	}

	// Token: 0x06001909 RID: 6409 RVA: 0x000A15C4 File Offset: 0x0009F7C4
	private void RefreshEntertainReward()
	{
		this._shopInfoPage.CGet<GameObject>("ItemOutput").SetActive(true);
		TextMeshProUGUI itemOutputText = this._shopInfoPage.CGet<TextMeshProUGUI>("ItemOutputText");
		int curCount = this._feast.GetInUseGiftSlotCount();
		int maxCount = GlobalConfig.Instance.FeastGiftCount;
		itemOutputText.text = LocalStringManager.GetFormat(LanguageKey.LK_Building_Reward_Content, curCount, maxCount).ColorReplace();
		GameObject itemResourceButton = this._shopInfoPage.CGet<GameObject>("ItemResourceButton");
		GameObject itemResourceButtonHolder = this._shopInfoPage.CGet<GameObject>("ItemResourceButtonHolder");
		this.ReuseGameObjectFunc(itemResourceButton, maxCount, itemResourceButtonHolder.transform);
		CToggleGroupObsolete outputStorageToggleGroup = this._shopInfoPage.CGet<CToggleGroupObsolete>("OutputStorageToggleGroup");
		outputStorageToggleGroup.gameObject.SetActive(true);
		outputStorageToggleGroup.InitPreOnToggle(-1);
		outputStorageToggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnOutputStorageToggleChange);
		this.RefreshShopEventStorageToggleGroup(outputStorageToggleGroup, -1, true, false);
		bool hasGift = false;
		for (int i = 0; i < maxCount; i++)
		{
			int index = i;
			Refers currentItemRefers = this._reuseDic[itemResourceButton.name][index].GetComponent<Refers>();
			currentItemRefers.CGet<GameObject>("Stay").SetActive(false);
			ItemResourceButton button = currentItemRefers.GetComponent<ItemResourceButton>();
			ItemKey itemKey = this._feast.GetGift(index);
			bool hasTemplate = itemKey.HasTemplate;
			if (hasTemplate)
			{
				hasGift = true;
				bool flag = ItemTemplateHelper.IsMiscResource(itemKey.ItemType, itemKey.TemplateId);
				if (flag)
				{
					int amount;
					this._feast.GiftCount.TryGetValue(index, out amount);
					sbyte resourceType = ItemTemplateHelper.GetMiscResourceType(itemKey.ItemType, itemKey.TemplateId);
					button.SetResourceFunc(resourceType, amount, ItemResourceButton.ItemResourceButtonState.Reveive, null, null, delegate
					{
						this.GetEntertainReward(index);
					});
				}
				else
				{
					bool flag2 = itemKey.IsValid();
					if (flag2)
					{
						Action <>9__2;
						ItemDomainMethod.AsyncCall.GetItemDisplayData(this, itemKey, this._taiwuCharId, delegate(int offset, RawDataPool dataPool)
						{
							ItemDisplayData itemDisplayData3 = null;
							Serializer.Deserialize(dataPool, offset, ref itemDisplayData3);
							ItemResourceButton button = button;
							ItemDisplayData itemDisplayData4 = itemDisplayData3;
							ItemResourceButton.ItemResourceButtonState btnState = ItemResourceButton.ItemResourceButtonState.Reveive;
							Action add = null;
							Action change = null;
							Action receive;
							if ((receive = <>9__2) == null)
							{
								receive = (<>9__2 = delegate()
								{
									this.GetEntertainReward(index);
								});
							}
							button.SetButtonFunc(itemDisplayData4, btnState, add, change, receive);
						});
					}
					else
					{
						ItemDisplayData itemDisplayData = new ItemDisplayData(itemKey.ItemType, itemKey.TemplateId);
						button.SetButtonFunc(itemDisplayData, ItemResourceButton.ItemResourceButtonState.Reveive, null, null, delegate
						{
							this.GetEntertainReward(index);
						});
					}
				}
			}
			else
			{
				ItemDisplayData itemDisplayData2 = new ItemDisplayData();
				button.SetButtonFunc(itemDisplayData2, ItemResourceButton.ItemResourceButtonState.None, null, null, null);
			}
		}
		CButtonObsolete quickBtn = this._shopInfoPage.CGet<CButtonObsolete>("QuickCollectItemBtn");
		quickBtn.interactable = hasGift;
	}

	// Token: 0x0600190A RID: 6410 RVA: 0x000A185F File Offset: 0x0009FA5F
	private void GetEntertainReward(int index)
	{
		ExtraDomainMethod.Call.FeastReceiveGift(this._blockKey, index);
		this.ShowGetItemAnimation();
		ExtraDomainMethod.Call.GetFeast(this.Element.GameDataListenerId, this._blockKey);
	}

	// Token: 0x0600190B RID: 6411 RVA: 0x000A1890 File Offset: 0x0009FA90
	private void GetAllEntertainReward()
	{
		bool hasGift = false;
		foreach (KeyValuePair<int, ItemKey> keyValuePair in this._feast.Gift)
		{
			int num;
			ItemKey itemKey2;
			keyValuePair.Deconstruct(out num, out itemKey2);
			int key = num;
			ItemKey itemKey = itemKey2;
			bool hasTemplate = itemKey.HasTemplate;
			if (hasTemplate)
			{
				hasGift = true;
				ExtraDomainMethod.Call.FeastReceiveGift(this._blockKey, key);
			}
		}
		bool flag = hasGift;
		if (flag)
		{
			this.ShowGetItemAnimation();
			ExtraDomainMethod.Call.GetFeast(this.Element.GameDataListenerId, this._blockKey);
		}
	}

	// Token: 0x0600190C RID: 6412 RVA: 0x000A1940 File Offset: 0x0009FB40
	private void OnMakeDropdownValueChanged(int value)
	{
		short index = Convert.ToInt16(value - 1);
		int type = (int)((index >= 0) ? this._unlockedFeastTypes[(int)index] : -1);
		bool flag = (int)this._feast.TargetType != type;
		if (flag)
		{
			ExtraDomainMethod.Call.FeastSetTargetType(this._blockKey, (short)type);
		}
	}

	// Token: 0x0600190D RID: 6413 RVA: 0x000A1990 File Offset: 0x0009FB90
	private void ShowFeastDropdown(bool show)
	{
		bool flag = this._feastDropdown && this._feastDropdown.gameObject.activeSelf != show;
		if (flag)
		{
			this._feastDropdown.gameObject.SetActive(show);
		}
		bool flag2 = !show;
		if (flag2)
		{
			this.ShowFeastDropdownMask(false);
		}
	}

	// Token: 0x0600190E RID: 6414 RVA: 0x000A19EC File Offset: 0x0009FBEC
	private void ShowFeastDropdownMask(bool show)
	{
		bool flag = this._feastDropdownMask && this._feastDropdownMask.gameObject.activeSelf != show;
		if (flag)
		{
			this._feastDropdownMask.gameObject.SetActive(show);
		}
	}

	// Token: 0x0600190F RID: 6415 RVA: 0x000A1A38 File Offset: 0x0009FC38
	private void UpdateFeastDropdown()
	{
		bool flag = !this._feastDropdown || !this._feastDropdown.IsExpanded;
		if (flag)
		{
			this.ShowFeastDropdownMask(false);
		}
		else
		{
			Transform trans = this._feastDropdown.transform.Find("Dropdown List");
			bool flag2 = !trans;
			if (flag2)
			{
				this.ShowFeastDropdownMask(false);
			}
			else
			{
				this.ShowFeastDropdownMask(true);
				CToggleObsolete[] toggles = this._feastDropdown.GetComponentsInChildren<CToggleObsolete>();
				PositionFollower positionFollower = this._feastDropdown.GetComponentInChildren<PositionFollower>();
				foreach (CToggleObsolete togCell in toggles)
				{
					bool flag3 = !togCell.gameObject.activeSelf;
					if (!flag3)
					{
						togCell.transform.Find("Disable").gameObject.SetActive(togCell.isOn);
						bool flag4 = togCell.isOn && positionFollower;
						if (flag4)
						{
							positionFollower.Target = togCell.transform;
						}
					}
				}
				this._feastDropdown.transform.Find("Dropdown List").GetComponent<Canvas>().sortingOrder = 640;
				Transform blocker = base.transform.parent.parent.Find("Blocker");
				bool flag5 = blocker != null;
				if (flag5)
				{
					blocker.GetComponent<Canvas>().sortingOrder = 639;
				}
			}
		}
	}

	// Token: 0x06001910 RID: 6416 RVA: 0x000A1BAC File Offset: 0x0009FDAC
	private void SetEntertainChar(int[] charIdList)
	{
		foreach (ResidentView residentView in this._residentViews)
		{
			bool flag = residentView && residentView.CharId >= 0;
			if (flag)
			{
				BuildingDomainMethod.Call.RemoveFromComfortableHouse(-1, residentView.CharId, this._blockKey);
			}
		}
		foreach (int charId in charIdList)
		{
			bool flag2 = charId >= 0;
			if (flag2)
			{
				BuildingDomainMethod.Call.AddToComfortableHouse(-1, charId, this._blockKey);
			}
		}
		BuildingDomainMethod.Call.GetCharsInComfortableHouse(this.Element.GameDataListenerId, this._blockKey);
	}

	// Token: 0x06001911 RID: 6417 RVA: 0x000A1C78 File Offset: 0x0009FE78
	private void InitMainToggleGroup()
	{
		int togKey = (this._initialTabKey >= 0) ? this._initialTabKey : ((this._blockData.OperationType != -1) ? this.OperationTypeToTogKey(this._blockData.OperationType) : 0);
		this._displayInited = true;
		bool flag = this._blockData.OperationType == 0;
		if (flag)
		{
			this._mainToggleGroup.Set(0, true, false);
			this.UpdateCurrentPage();
			this._removeCollectInfoPage.CGet<TextMeshProUGUI>("ResourceTitleText").text = LocalStringManager.Get(LanguageKey.LK_Building_ExpandConsume);
			this.SetResourceInfo((from x in BuildingBlock.Instance[this._blockData.TemplateId].BaseBuildCost
			select (int)x).ToList<int>());
			this.RefreshInfoTogName();
			this.UpdateResourceContentSize();
			base.CGet<GameObject>("TopInfo").SetActive(true);
			base.CGet<GameObject>("NoContent").SetActive(false);
			bool flag2 = this._initialTabKey >= 0;
			if (flag2)
			{
				this._initialTabKey = -1;
			}
		}
		else
		{
			bool flag3 = this._mainToggleGroup.GetActive().Key == togKey;
			if (flag3)
			{
				this.UpdateCurrentPage();
			}
			else
			{
				this._mainToggleGroup.Set(togKey, true, false);
			}
			bool flag4 = this._initialTabKey >= 0;
			if (flag4)
			{
				this.RefreshInfoTogName();
				bool flag5 = this._mainToggleGroup.GetActivatedCount() != 0;
				if (flag5)
				{
					base.CGet<GameObject>("TopInfo").SetActive(true);
					base.CGet<GameObject>("NoContent").SetActive(false);
				}
				else
				{
					base.CGet<GameObject>("TopInfo").SetActive(false);
					base.CGet<GameObject>("NoContent").SetActive(true);
				}
				this._initialTabKey = -1;
			}
			else
			{
				BuildingBlockItem blockConfig = BuildingBlock.Instance[this._blockData.TemplateId];
				bool flag6 = this._blockData.TemplateId >= 21 && this._blockData.TemplateId <= 23;
				if (flag6)
				{
					this._mainToggleGroup.Set(3, true, false);
				}
				else
				{
					bool flag7 = blockConfig.Class == EBuildingBlockClass.BornResource;
					if (flag7)
					{
						this._mainToggleGroup.Set(2, true, false);
					}
				}
				bool flag8 = this._blockData.TemplateId == 51;
				if (flag8)
				{
					this._mainToggleGroup.Set(2, true, false);
				}
				bool flag9 = this._blockData.TemplateId == 48;
				if (flag9)
				{
					this._mainToggleGroup.Set(2, true, false);
				}
				bool flag10 = this._blockData.TemplateId == 49 && this._isTaiwuVillageBuilding;
				if (flag10)
				{
					this._mainToggleGroup.Set(3, true, false);
				}
				bool flag11 = this._configData.IsShop && this._blockData.OperationType != 0;
				if (flag11)
				{
					this._mainToggleGroup.Set(1, true, false);
				}
				bool flag12 = this._blockData.Durability < this._configData.MaxDurability && this._blockData.OperationType == -1;
				if (flag12)
				{
					this._mainToggleGroup.Set(4, true, false);
				}
				ValueTuple<bool, int> valueTuple = this._mainToggleGroup.IsOnlyTogActive();
				bool onlyOne = valueTuple.Item1;
				int activeIndex = valueTuple.Item2;
				bool flag13 = onlyOne;
				if (flag13)
				{
					this._mainToggleGroup.Set(activeIndex, true, false);
				}
				this.RefreshInfoTogName();
				bool flag14 = this._mainToggleGroup.GetActivatedCount() != 0;
				if (flag14)
				{
					base.CGet<GameObject>("TopInfo").SetActive(true);
					base.CGet<GameObject>("NoContent").SetActive(false);
				}
				else
				{
					base.CGet<GameObject>("TopInfo").SetActive(false);
					base.CGet<GameObject>("NoContent").SetActive(true);
				}
			}
		}
	}

	// Token: 0x06001912 RID: 6418 RVA: 0x000A2060 File Offset: 0x000A0260
	private void RefreshInfoTogName()
	{
		CToggleObsolete toggle = this._mainToggleGroup.Get(0);
		UI_BuildingManage.SetMainToggleText(toggle, this.GetInfoToggleText());
	}

	// Token: 0x06001913 RID: 6419 RVA: 0x000A2088 File Offset: 0x000A0288
	private string GetInfoToggleText()
	{
		short templateId = this._configData.TemplateId;
		bool flag = (templateId == 47 || templateId == 46) && this._blockData.OperationType != 0;
		string result;
		if (flag)
		{
			result = LocalStringManager.Get((this._configData.TemplateId == 46) ? LanguageKey.LK_Building_ResidentInfo : LanguageKey.LK_Building_ComfortableInfo1);
		}
		else
		{
			bool flag2 = this._configData.TemplateId == 50;
			if (flag2)
			{
				result = LocalStringManager.Get(LanguageKey.LK_Building_SamsaraPlatformLog);
			}
			else
			{
				result = LocalStringManager.Get(LanguageKey.LK_Detail);
			}
		}
		return result;
	}

	// Token: 0x06001914 RID: 6420 RVA: 0x000A2118 File Offset: 0x000A0318
	private void OnToggleChange(CToggleObsolete newTog, CToggleObsolete oldTog)
	{
		bool flag = newTog == null;
		if (!flag)
		{
			this._infoPage.gameObject.SetActive(newTog.Key == 0);
			bool flag2 = newTog.Key == 0;
			if (flag2)
			{
				GameObject gameObject = this._infoPage.CGet<GameObject>("ResidentInfo");
				short templateId = this._configData.TemplateId;
				gameObject.SetActive(templateId == 46 || templateId == 47);
				bool flag3 = this._configData.TemplateId == 46;
				if (flag3)
				{
					GridLayoutGroup layoutGroup = base.CGet<GameObject>("ResidentsLayoutGroup").GetComponent<GridLayoutGroup>();
					layoutGroup.padding.left = 85;
					layoutGroup.spacing = this._residenceLayoutSpacing;
				}
				else
				{
					bool flag4 = this._configData.TemplateId == 47;
					if (flag4)
					{
						GridLayoutGroup layoutGroup2 = base.CGet<GameObject>("ResidentsLayoutGroup").GetComponent<GridLayoutGroup>();
						layoutGroup2.padding.left = 210;
						layoutGroup2.spacing = this._comfortableHouseLayoutSpacing;
					}
				}
				bool showSamsara = this._configData.TemplateId == 50;
				bool flag5 = this._blockData.Durability < this._configData.MaxDurability;
				if (flag5)
				{
					showSamsara = false;
				}
				bool flag6 = showSamsara;
				if (flag6)
				{
					this.ShowTaiwuSamsaraLog();
				}
				this._infoPage.CGet<Refers>("TaiwuSamsaraLog").gameObject.SetActive(showSamsara);
			}
			else
			{
				this._infoPage.CGet<Refers>("TaiwuSamsaraLog").gameObject.SetActive(false);
			}
			TutorialChapterModel tutorialModel = SingletonObject.getInstance<TutorialChapterModel>();
			bool flag7 = !tutorialModel.GetFunctionStatus(1) && this._blockData.OperationType == 0;
			if (flag7)
			{
				this._removeCollectInfoPage.gameObject.SetActive(false);
			}
			else
			{
				this._removeCollectInfoPage.gameObject.SetActive(newTog.Key == 3 || this._blockData.OperationType == 0);
			}
			bool showExpand = newTog.Key == 2;
			this._expandInfoPage.gameObject.SetActive(showExpand);
			bool flag8 = showExpand;
			if (flag8)
			{
				this.SelectExpandChild();
			}
			this._damageInfoPage.gameObject.SetActive(newTog.Key == 4);
			bool flag9 = (newTog.Key == 1 || newTog.Key == 5) && this._isTaiwuVillageBuilding;
			if (flag9)
			{
				this._shopInfoPage.gameObject.SetActive(true);
				this._shopInfoPage.CGet<GameObject>("ManagePanel").SetActive(newTog.Key != 5);
				this._shopInfoPage.CGet<GameObject>("ProductionPanel").SetActive(newTog.Key == 5);
			}
			else
			{
				bool flag10 = newTog.Key == 7 && this._isTaiwuVillageBuilding;
				if (flag10)
				{
					this._shopInfoPage.gameObject.SetActive(true);
					this._shopInfoPage.CGet<GameObject>("ManagePanel").SetActive(false);
					this._shopInfoPage.CGet<GameObject>("ProductionPanel").SetActive(true);
				}
				else
				{
					this._shopInfoPage.gameObject.SetActive(false);
				}
			}
			this._residentsHolder.SetActive((this._configData.TemplateId == 46 || this._configData.TemplateId == 47) && newTog.Key == 0 && this._blockData.OperationType != 0);
			for (int i = 0; i < 3; i++)
			{
				Transform child = this._expandManpowerHolder.transform.GetChild(i);
				child.gameObject.SetActive(true);
			}
			this._removeCollectInfoPage.CGet<GameObject>("BottomBtnBg").gameObject.SetActive(newTog.Key == 3);
			this.UpdateCurrentPage();
			bool flag11;
			if (oldTog != null)
			{
				int key = newTog.Key;
				flag11 = (key == 5 || key == 1 || key == 7);
			}
			else
			{
				flag11 = false;
			}
			bool flag12 = flag11;
			if (flag12)
			{
				this.UpdateShopEventBookInfo();
			}
			bool showEntertainPage = newTog.Key == 6 && this._isTaiwuVillageBuilding;
			this._entertainPage.gameObject.SetActive(showEntertainPage);
			this.PlayEntertainEffectRandom();
		}
	}

	// Token: 0x06001915 RID: 6421 RVA: 0x000A2554 File Offset: 0x000A0754
	private void UpdateToggles()
	{
		CToggleObsolete infoTog = this._mainToggleGroup.Get(0);
		GameObject gameObject = infoTog.gameObject;
		short templateId = this._configData.TemplateId;
		gameObject.SetActive(templateId == 46 || templateId == 47 || templateId == 50);
		CToggleObsolete shopTog = this._mainToggleGroup.Get(1);
		shopTog.gameObject.SetActive(this._configData.IsShop && this._blockData.OperationType != 0 && this._isTaiwuVillageBuilding);
		shopTog.interactable = (this.IsBuildingManagementUnlocked && this._isTaiwuVillageBuilding);
		CToggleObsolete productionTog = this._mainToggleGroup.Get(5);
		bool hasResourceIncome = this._shopEventData != null && this._shopEventData.ResourceList.Count > 0;
		bool hasShopManage = this._configData.IsShop && this._blockData.OperationType != 0 && this._isTaiwuVillageBuilding;
		bool hasItemIncome = this._shopEventData != null && (this._shopEventData.ResourceGoods != -1 || this._shopEventData.ItemList.Count > 0 || this._shopEventData.ItemGradeProbList.Count > 0);
		bool hasSoldItem = this._shopEventData != null && this._shopEventData.ExchangeResourceGoods != -1;
		bool hasRecruit = this._shopEventData != null && this._shopEventData.RecruitPeopleProb.Count > 0;
		productionTog.gameObject.SetActive(hasShopManage && (hasResourceIncome || hasItemIncome || hasSoldItem || hasRecruit));
		productionTog.interactable = (this.IsBuildingManagementUnlocked && this._isTaiwuVillageBuilding);
		Refers pageTogGroupRefers = this._mainToggleGroup.GetComponent<Refers>();
		CToggleObsolete expandTog = this._mainToggleGroup.Get(2);
		this.UpdateExpandToggle(expandTog);
		CToggleObsolete removeTog = this._mainToggleGroup.Get(3);
		bool canRemove = UI_BuildingManage.CanRemove(this._configData.TemplateId, this._isTaiwuVillageBuilding);
		removeTog.gameObject.SetActive(canRemove);
		bool flag = canRemove;
		if (flag)
		{
			sbyte operationType = this._blockData.OperationType;
			bool removeTogInteractable = operationType == -1 || operationType == 1;
			removeTog.interactable = (this.IsBuildingManagementUnlocked && removeTogInteractable);
			TooltipInvoker removeTogMouseTip = removeTog.GetComponent<Refers>().CGet<TooltipInvoker>("DisableTip");
			removeTogMouseTip.enabled = !removeTogInteractable;
			bool flag2 = !removeTogInteractable;
			if (flag2)
			{
				TooltipInvoker tooltipInvoker = removeTogMouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				bool flag3 = this._blockData.OperationType == 0;
				if (flag3)
				{
					removeTogMouseTip.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Building_RemoveToggle_Tip1));
				}
			}
		}
		this._mainToggleGroup.Get(4).gameObject.SetActive(this._blockData.Durability < this._configData.MaxDurability && this._blockData.OperationType == -1);
		this._mainToggleGroup.gameObject.SetActive(this._blockData.OperationType != 0);
		CToggleObsolete entertainTog = this._mainToggleGroup.Get(6);
		entertainTog.gameObject.SetActive(this.IsEntertain);
		entertainTog.interactable = (this.IsBuildingManagementUnlocked && this._isTaiwuVillageBuilding);
		CToggleObsolete rewardTog = this._mainToggleGroup.Get(7);
		rewardTog.gameObject.SetActive(this.IsEntertain);
		rewardTog.interactable = (this.IsBuildingManagementUnlocked && this._isTaiwuVillageBuilding);
	}

	// Token: 0x06001916 RID: 6422 RVA: 0x000A28E8 File Offset: 0x000A0AE8
	public static bool CanRemove(short templateId, bool isTaiwuVillageBuilding)
	{
		BuildingBlockItem config = BuildingBlock.Instance[templateId];
		return config.Class != EBuildingBlockClass.Static && config.OperationTotalProgress[1] >= 0 && isTaiwuVillageBuilding;
	}

	// Token: 0x06001917 RID: 6423 RVA: 0x000A2924 File Offset: 0x000A0B24
	private void UpdateExpandToggle(CToggleObsolete expandTog)
	{
		BuildingBlockItem blockConfig = BuildingBlock.Instance[this._blockData.TemplateId];
		bool isResource = blockConfig.Class == EBuildingBlockClass.BornResource;
		bool isUselessResource = isResource && blockConfig.Type == EBuildingBlockType.UselessResource;
		expandTog.gameObject.SetActive(this._configData.MaxLevel > 1 && !isUselessResource && blockConfig.TemplateId != 50);
		bool isTeaHorse = blockConfig.TemplateId == 51;
		string expandToggleText = UI_BuildingManage.GetExpandToggleText(isResource, isTeaHorse);
		UI_BuildingManage.SetMainToggleText(expandTog, expandToggleText);
	}

	// Token: 0x06001918 RID: 6424 RVA: 0x000A29AC File Offset: 0x000A0BAC
	private static string GetExpandToggleText(bool isResource, bool isTeaHorse)
	{
		string expandToggleText;
		if (isResource)
		{
			expandToggleText = LocalStringManager.Get(LanguageKey.LK_Building_Expand_UpgradeResourceBuilding_Tab);
		}
		else if (isTeaHorse)
		{
			expandToggleText = LocalStringManager.Get(LanguageKey.LK_Building_TeaHorse_Awareness_Title);
		}
		else
		{
			expandToggleText = LocalStringManager.Get(LanguageKey.LK_Building_Upgrade);
		}
		return expandToggleText;
	}

	// Token: 0x06001919 RID: 6425 RVA: 0x000A29F4 File Offset: 0x000A0BF4
	private void UpdateToggleTitle()
	{
		TextMeshProUGUI titleInfoText = this._rightTitle;
		CToggleObsolete curTog = this._mainToggleGroup.GetActive();
		bool flag = curTog.Key == 0;
		if (flag)
		{
			bool flag2 = this._blockData.OperationType == 0;
			if (flag2)
			{
				titleInfoText.SetText(LocalStringManager.Get(LanguageKey.LK_Building_New), true);
			}
			else
			{
				short templateId = this._configData.TemplateId;
				bool flag3 = templateId == 47 || templateId == 46;
				if (flag3)
				{
					titleInfoText.SetText(LocalStringManager.Get((this._configData.TemplateId == 46) ? LanguageKey.LK_Building_ResidentInfo : LanguageKey.LK_Building_ComfortableInfo1), true);
					base.CGet<TextMeshProUGUI>("ResidentViewQuickText").SetText(LocalStringManager.Get((this._configData.TemplateId == 46) ? LanguageKey.LK_Building_QuickCheckIn : LanguageKey.LK_Building_ComfortableInfo3), true);
					base.CGet<TextMeshProUGUI>("AutoCheckInToggleLabel").SetText(LocalStringManager.Get((this._configData.TemplateId == 46) ? LanguageKey.LK_Building_AutoCheckIn : LanguageKey.LK_Building_ComfortableInfo4), true);
				}
				else
				{
					BuildingBlockItem template = BuildingBlock.Instance.GetItem(this._configData.TemplateId);
					bool flag4 = template != null;
					if (flag4)
					{
						titleInfoText.SetText(template.Name, true);
					}
				}
			}
		}
		else
		{
			bool flag5 = curTog.Key == 1;
			if (flag5)
			{
				titleInfoText.SetText(LocalStringManager.Get(LanguageKey.LK_Building_ManageInfo), true);
			}
			else
			{
				bool flag6 = curTog.Key == 2;
				if (flag6)
				{
					BuildingBlockItem blockConfig = BuildingBlock.Instance[this._blockData.TemplateId];
					bool isResource = blockConfig.Class == EBuildingBlockClass.BornResource;
					bool isTeaHorse = blockConfig.TemplateId == 51;
					LanguageKey expandTitleKey = UI_BuildingManage.GetExpandTitleKey(isResource, isTeaHorse);
					titleInfoText.SetText(LocalStringManager.Get(expandTitleKey), true);
				}
				else
				{
					bool flag7 = curTog.Key == 3;
					if (flag7)
					{
						titleInfoText.SetText(LocalStringManager.Get(LanguageKey.LK_Building_RemoveBuilding), true);
					}
					else
					{
						bool flag8 = curTog.Key == 4;
						if (flag8)
						{
							titleInfoText.SetText(LocalStringManager.Get(LanguageKey.LK_Building_DamageTitle), true);
						}
						else
						{
							bool flag9 = curTog.Key == 5;
							if (flag9)
							{
								titleInfoText.SetText(LocalStringManager.Get(LanguageKey.LK_Building_ProductionInfo), true);
							}
							else
							{
								bool flag10 = curTog.Key == 7;
								if (flag10)
								{
									titleInfoText.SetText(LocalStringManager.Get(LanguageKey.LK_Building_Reward_Title), true);
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0600191A RID: 6426 RVA: 0x000A2C60 File Offset: 0x000A0E60
	private static LanguageKey GetExpandTitleKey(bool isResource, bool isTeaHorse)
	{
		LanguageKey result;
		if (isResource)
		{
			result = LanguageKey.LK_Building_Expand_UpgradeResourceBuilding_Title;
		}
		else if (isTeaHorse)
		{
			result = LanguageKey.LK_Building_TeaHorse_Awareness_Title;
		}
		else
		{
			result = LanguageKey.LK_Building_Upgrade;
		}
		return result;
	}

	// Token: 0x0600191B RID: 6427 RVA: 0x000A2C94 File Offset: 0x000A0E94
	private static void SetMainToggleText(CToggleObsolete mainToggle, string str)
	{
		foreach (TextMeshProUGUI label in mainToggle.LabelList)
		{
			label.text = str;
		}
	}

	// Token: 0x0600191C RID: 6428 RVA: 0x000A2CF0 File Offset: 0x000A0EF0
	private void UpdateRemoveInfoPage()
	{
		this.UpdateBuildingNormalInfo();
		GameObject expandTitleContent = this._removeCollectInfoPage.CGet<GameObject>("ExpandTitleContent");
		TextMeshProUGUI resourceTitleText = this._removeCollectInfoPage.CGet<TextMeshProUGUI>("ResourceTitleText");
		TooltipInvoker resourceTitleMouseTip = this._removeCollectInfoPage.CGet<TooltipInvoker>("ResourceTitleMouseTip");
		List<int> resourceCountList = EasyPool.Get<List<int>>();
		resourceCountList.Clear();
		int togKey = this._mainToggleGroup.GetActive().Key;
		bool flag = togKey == 3;
		if (flag)
		{
			expandTitleContent.SetActive(false);
			resourceTitleText.text = LocalStringManager.Get(LanguageKey.LK_Building_RemoveGetResource);
			resourceTitleMouseTip.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_Building_RemoveGetResourceTip);
			sbyte level = this._buildingModel.GetBuildingLevel(this._blockKey, this._blockData);
			sbyte resourceType = 0;
			while ((int)resourceType < this._configData.BaseBuildCost.Length)
			{
				resourceCountList.Add(GameData.Domains.Building.SharedMethods.GetResourceReturnOfRemoveBuilding(this._configData, level, resourceType, this._blockData));
				resourceType += 1;
			}
		}
		this.SetResourceInfo(resourceCountList);
		bool flag2 = this._blockData.OperationType == -1;
		if (flag2)
		{
			this.ClearOperators();
		}
		this.UpdateExpandRemoveCollectOperators();
		this.UpdateResourceContentSize();
		EasyPool.Free<List<int>>(resourceCountList);
	}

	// Token: 0x0600191D RID: 6429 RVA: 0x000A2E28 File Offset: 0x000A1028
	private void SetResourceInfo(List<int> resourceCountList)
	{
		int togKey = this._mainToggleGroup.GetActive().Key;
		bool costResource = togKey != 3;
		List<GameObject> resourceObjList = this._needResourceHolder.CGetList<GameObject>("ResourceObj");
		List<TextMeshProUGUI> resourceTextList = this._needResourceHolder.CGetList<TextMeshProUGUI>("ResourceCount");
		this._resourceEnough = true;
		for (sbyte i = 0; i < 8; i += 1)
		{
			bool valid = resourceCountList.CheckIndex((int)i);
			resourceObjList[(int)i].SetActive(valid && resourceCountList[(int)i] > 0);
			bool flag = valid && resourceCountList[(int)i] > 0;
			if (flag)
			{
				bool flag2 = costResource;
				string resourceStr;
				if (flag2)
				{
					string color = (this._buildingModel.GetResourceCount(i) >= resourceCountList[(int)i]) ? "brightblue" : "brightred";
					string ownStr = CommonUtils.GetDisplayStringForNum(this._buildingModel.GetResourceCount(i), 100000).SetColor(color);
					resourceStr = string.Format("{0}/{1}", ownStr, resourceCountList[(int)i]);
					bool flag3 = this._buildingModel.GetResourceCount(i) < resourceCountList[(int)i];
					if (flag3)
					{
						this._resourceEnough = false;
					}
				}
				else
				{
					resourceStr = resourceCountList[(int)i].ToString();
				}
				resourceTextList[(int)i].text = resourceStr;
			}
		}
	}

	// Token: 0x0600191E RID: 6430 RVA: 0x000A2F98 File Offset: 0x000A1198
	private void ClearOperators()
	{
		for (int i = 0; i < this._operatorListCached.Length; i++)
		{
			this._operatorListCached[i] = -1;
		}
		this.RefreshAllQuickSelectButtons();
	}

	// Token: 0x0600191F RID: 6431 RVA: 0x000A2FD0 File Offset: 0x000A11D0
	private void UpdateResourceContentSize()
	{
		RectTransform resourceContent = this._removeCollectInfoPage.CGet<RectTransform>("ResourceContent");
		int activeCount = 0;
		for (int i = 0; i < resourceContent.childCount; i++)
		{
			bool activeSelf = resourceContent.GetChild(i).gameObject.activeSelf;
			if (activeSelf)
			{
				activeCount++;
			}
		}
		int itemCountEachRow = resourceContent.GetComponent<GridLayoutGroup>().constraintCount;
		bool flag = activeCount <= itemCountEachRow;
		if (flag)
		{
			resourceContent.sizeDelta = this.ResourceContentSize;
		}
		else
		{
			bool flag2 = activeCount <= itemCountEachRow * 2;
			if (flag2)
			{
				resourceContent.sizeDelta = this.ResourceContentSize + new Vector2(0f, this.ResourceContentSizeHighAdd);
			}
			else
			{
				resourceContent.sizeDelta = this.ResourceContentSize + new Vector2(0f, this.ResourceContentSizeHighAdd * 2f);
			}
		}
		resourceContent.GetComponent<GridLayoutGroup>().cellSize = this.ItemResourceInfoSize;
	}

	// Token: 0x06001920 RID: 6432 RVA: 0x000A30C0 File Offset: 0x000A12C0
	private void CreateNewResident()
	{
		sbyte index = (sbyte)this._residentViews.Count;
		GameObject obj = Object.Instantiate<GameObject>(this._residentViewPrefab, base.CGet<GameObject>("ResidentsLayoutGroup").transform);
		ResidentView residentView = obj.GetComponent<ResidentView>();
		UnityAction ChangeAvatarAction = delegate()
		{
			bool isEntertain = this.IsEntertain;
			if (isEntertain)
			{
				sbyte level = this._buildingModel.GetBuildingLevel(this._blockKey, this._blockData);
				int charMaxCount = BuildingScale.DefValue.ComfortableHouseCapacity.GetLevelEffect((int)level);
				List<int> selectedCharIdList = (from r in this._residentViews
				where r.CharId >= 0
				select r.CharId).ToList<int>();
				BuildingDomainMethod.AsyncCall.GetFeastTargetCharList(this, this._blockKey, delegate(int offset, RawDataPool pool)
				{
					List<CharacterDisplayData> data = new List<CharacterDisplayData>();
					Serializer.Deserialize(pool, offset, ref data);
					ArgumentBox args = EasyPool.Get<ArgumentBox>().Set("selectCount", charMaxCount).SetObject("charIdList", (from d in data
					select d.CharacterId).ToList<int>()).SetObject("selectedCharIdList", selectedCharIdList).SetObject("filterType", new List<global::CharacterTable.CharacterTableCommonFilterTypes>
					{
						global::CharacterTable.CharacterTableCommonFilterTypes.Feast
					}).SetObject("onMultiSelect", new Action<int[]>(this.SetEntertainChar)).Set("enableMultiSelect", true);
					UIElement.SelectCharLegacy.SetOnInitArgs(args);
					UIManager.Instance.ShowUI(UIElement.SelectCharLegacy, true);
				});
			}
			else
			{
				BuildingDomainMethod.Call.GetAllResidents(this.Element.GameDataListenerId, this._blockKey, this.IsAtResidence());
			}
		};
		residentView.CGet<CButtonObsolete>("ChangeButton").onClick.AddListener(delegate()
		{
			this.replaceResidentIndex = index;
			ChangeAvatarAction();
		});
		residentView.CGet<CButtonObsolete>("SelectCharBtn").onClick.AddListener(delegate()
		{
			this.replaceResidentIndex = -1;
			ChangeAvatarAction();
		});
		this._residentViews.Add(residentView);
	}

	// Token: 0x06001921 RID: 6433 RVA: 0x000A3174 File Offset: 0x000A1374
	private bool IsAtResidence()
	{
		return this._configData.TemplateId == 46;
	}

	// Token: 0x06001922 RID: 6434 RVA: 0x000A3198 File Offset: 0x000A1398
	public void SetCharacterInResidence(int charId)
	{
		bool flag = this._configData.TemplateId == 46;
		if (flag)
		{
			bool flag2 = this.replaceResidentIndex != -1;
			if (flag2)
			{
				BuildingDomainMethod.Call.ReplaceCharacterInResidence(-1, charId, this._blockKey, this.replaceResidentIndex);
				this.replaceResidentIndex = -1;
			}
			else
			{
				bool flag3 = charId < 0;
				if (flag3)
				{
					return;
				}
				BuildingDomainMethod.Call.AddToResidence(charId, this._blockKey);
			}
			BuildingDomainMethod.Call.GetCharsInResidence(this.Element.GameDataListenerId, this._blockKey);
		}
	}

	// Token: 0x06001923 RID: 6435 RVA: 0x000A321C File Offset: 0x000A141C
	private void DisplayResident(CharacterDisplayData displayData, ResidentView residentView, bool modifiable)
	{
		GameObject selectCharBack = residentView.CGet<GameObject>("SelectCharBack");
		bool flag = displayData != null;
		if (flag)
		{
			residentView.RenderResidentCharInfo(displayData.CharacterId);
			selectCharBack.SetActive(false);
			residentView.gameObject.SetActive(true);
			int charId = displayData.CharacterId;
			TextMeshProUGUI workTypeText = residentView.CGet<TextMeshProUGUI>("WorkTypeText");
			bool flag2 = this._buildingModel.VillagerWork.ContainsKey(charId) && (this._buildingModel.VillagerWork[charId].WorkType == 0 || this._buildingModel.VillagerWork[charId].WorkType == 1 || this._buildingModel.VillagerWork[charId].WorkType >= 10);
			if (flag2)
			{
				VillagerWorkData workData = this._buildingModel.VillagerWork[charId];
				Location location = new Location(workData.AreaId, workData.BlockId);
				MapDomainMethod.AsyncCall.IsLocationInBuildingEffectRange(this, this._charDisplayDataDict[charId].Location, location, delegate(int offset, RawDataPool dataPool)
				{
					bool isInBuildingEffectRange = false;
					Serializer.Deserialize(dataPool, offset, ref isInBuildingEffectRange);
					bool flag3 = isInBuildingEffectRange;
					if (flag3)
					{
						bool flag4 = this._buildingModel.VillagerWork[charId].WorkType == 0;
						if (flag4)
						{
							workTypeText.SetText(LocalStringManager.Get(LanguageKey.UI_VillagerWork_Build), true);
						}
						else
						{
							workTypeText.SetText(LocalStringManager.Get(string.Format("LK_WorkType_{0}", workData.WorkType)), true);
						}
					}
					else
					{
						workTypeText.SetText(LocalStringManager.Get(LanguageKey.UI_VillagerWork_Move), true);
					}
				});
			}
			else
			{
				workTypeText.SetText(LocalStringManager.Get(LanguageKey.UI_VillagerWork_Idle), true);
			}
		}
		else
		{
			residentView.RenderResidentCharInfo(-1);
			selectCharBack.SetActive(modifiable);
			selectCharBack.GetComponentInChildren<CButtonObsolete>().interactable = this.IsBuildingManagementUnlocked;
			residentView.gameObject.SetActive(modifiable);
		}
		residentView.CGet<GameObject>("CharInfoHolder").SetActive(displayData != null);
	}

	// Token: 0x06001924 RID: 6436 RVA: 0x000A33F9 File Offset: 0x000A15F9
	private void ShowSelectResidentPanel()
	{
		this.OnShowHomelessOnlyTogValueChange();
	}

	// Token: 0x06001925 RID: 6437 RVA: 0x000A3404 File Offset: 0x000A1604
	private void OnShowHomelessOnlyTogValueChange()
	{
		string[] array2;
		if (this._configData.TemplateId != 46)
		{
			string[] array = new string[3];
			array[0] = LocalStringManager.Get(LanguageKey.LK_ResidentState_Residence);
			array[1] = LocalStringManager.Get(LanguageKey.LK_ResidentState_Homeless);
			array2 = array;
			array[2] = LocalStringManager.Get(LanguageKey.LK_ResidentState_Infected).SetColor(Color.red);
		}
		else
		{
			string[] array3 = new string[3];
			array3[0] = LocalStringManager.Get(LanguageKey.LK_ResidentState_Homeless);
			array3[1] = LocalStringManager.Get(LanguageKey.LK_ResidentState_Residence);
			array2 = array3;
			array3[2] = LocalStringManager.Get(LanguageKey.LK_ResidentState_Infected).SetColor(Color.red);
		}
		string[] livingPlace = array2;
		this._residentsLivingPlace.Clear();
		for (int i = 0; i < this._allResidents.Count; i++)
		{
			using (List<int>.Enumerator enumerator = this._allResidents[i].GetCollection().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int charId = enumerator.Current;
					bool flag = this._residentViews.All((ResidentView r) => r.CharId != charId);
					if (flag)
					{
						this._residentsLivingPlace.Add(new ValueTuple<int, string>(charId, livingPlace[i]));
					}
				}
			}
		}
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Clear();
		argBox.SetObject("charList", this._residentsLivingPlace);
		argBox.SetObject("callback", new Action<int>(this.SetCharacterInResidence));
		argBox.SetObject("filterType", new List<global::CharacterTable.CharacterTableCommonFilterTypes>
		{
			global::CharacterTable.CharacterTableCommonFilterTypes.Resident
		});
		argBox.SetObject("usingPages", new List<ECharacterTableType>
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
		UIElement.SelectCharLegacy.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.SelectCharLegacy, true);
	}

	// Token: 0x06001926 RID: 6438 RVA: 0x000A3634 File Offset: 0x000A1834
	private void ReuseGameObject(GameObject goTemplate, int index, Transform parentTrans)
	{
		bool flag = !this._reuseDic.ContainsKey(goTemplate.name);
		GameObject go;
		if (flag)
		{
			List<GameObject> goList = new List<GameObject>();
			this._reuseDic.Add(goTemplate.name, goList);
			go = Object.Instantiate<GameObject>(goTemplate, parentTrans);
			goList.Add(go);
		}
		else
		{
			List<GameObject> goList = this._reuseDic[goTemplate.name];
			bool flag2 = goList.Count > index;
			if (flag2)
			{
				go = goList[index];
			}
			else
			{
				go = Object.Instantiate<GameObject>(goTemplate, parentTrans);
				goList.Add(go);
			}
		}
		go.SetActive(true);
	}

	// Token: 0x06001927 RID: 6439 RVA: 0x000A36D4 File Offset: 0x000A18D4
	private void ReuseGameObjectFunc(GameObject goTemplate, int count, Transform parentTrans)
	{
		bool flag = count < 0;
		if (!flag)
		{
			for (int i = 0; i < count; i++)
			{
				this.ReuseGameObject(goTemplate, i, parentTrans);
			}
			this.SetIdleReuseGameObjectNotActive(goTemplate, count);
		}
	}

	// Token: 0x06001928 RID: 6440 RVA: 0x000A3714 File Offset: 0x000A1914
	private void SetIdleReuseGameObjectNotActive(GameObject goTemplate, int index)
	{
		bool flag = this._reuseDic.ContainsKey(goTemplate.name);
		if (flag)
		{
			List<GameObject> goList = this._reuseDic[goTemplate.name];
			bool flag2 = goList.Count > index;
			if (flag2)
			{
				for (int i = index; i < goList.Count; i++)
				{
					goList[i].gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x06001929 RID: 6441 RVA: 0x000A3786 File Offset: 0x000A1986
	private void GetBuildingCoreInfo()
	{
		TaiwuDomainMethod.Call.GetCanOperateItemDisplayDataInVillage(this.Element.GameDataListenerId, 1205);
		TaiwuDomainMethod.Call.GetCannotOperateItemDisplayDataInInventory(this.Element.GameDataListenerId, 1205);
	}

	// Token: 0x0600192A RID: 6442 RVA: 0x000A37B8 File Offset: 0x000A19B8
	private void OnBuildingCustomNameChange(ArgumentBox argBox)
	{
		BuildingBlockKey blockKey;
		argBox.Get<BuildingBlockKey>("BuildingBlockKey", out blockKey);
		bool flag = blockKey.AreaId == this._areaId && blockKey.BlockId == this._blockId && blockKey.BuildingBlockIndex == this._buildingBlockIndex;
		if (flag)
		{
			ViewBuildingArea.SetBuildingName(this._leftInfo.CGet<TextMeshProUGUI>("BuildingName"), this._configData, blockKey, this._blockTemplateId, true);
		}
	}

	// Token: 0x0600192B RID: 6443 RVA: 0x000A382C File Offset: 0x000A1A2C
	public void Rename()
	{
		TMP_InputField inputField = this._leftInfo.CGet<TMP_InputField>("CustomNameInput");
		BuildingDomainMethod.Call.SetBuildingCustomName(this._blockKey, inputField.text);
		this._leftInfo.CGet<CButtonObsolete>("RenameBtn").gameObject.SetActive(true);
		inputField.gameObject.SetActive(false);
		this._leftInfo.CGet<TextMeshProUGUI>("BuildingName").gameObject.SetActive(true);
	}

	// Token: 0x0600192C RID: 6444 RVA: 0x000A38A4 File Offset: 0x000A1AA4
	private void UpdateLockBuildingList()
	{
		for (int i = 0; i < this._learnedLifeSkillItems.Count; i++)
		{
			Config.LifeSkillItem unlockItemConfig = LifeSkill.Instance[this._learnedLifeSkillItems[i].SkillTemplateId];
			List<short> idList = new List<short>();
			CommonUtils.GetUnlockBuildingListFromConfig(unlockItemConfig, idList);
			bool flag = this._learnedLifeSkillItems[i].IsPageRead(0);
			if (flag)
			{
				for (int j = 0; j < idList.Count; j++)
				{
					bool flag2 = this._lockBuildingList.Contains(idList[j]);
					if (flag2)
					{
						this._lockBuildingList.Remove(idList[j]);
					}
				}
			}
		}
	}

	// Token: 0x0600192D RID: 6445 RVA: 0x000A3968 File Offset: 0x000A1B68
	private bool CanUnlockBuildingByLifeSkill(BuildingBlockItem blockItem)
	{
		return !this._lockBuildingList.Contains(blockItem.TemplateId);
	}

	// Token: 0x0600192E RID: 6446 RVA: 0x000A3990 File Offset: 0x000A1B90
	private void GetLockBuildingList()
	{
		this._lockBuildingList.Clear();
		List<short> lockBuildingList = new List<short>();
		for (int i = 0; i < LifeSkill.Instance.Count; i++)
		{
			Config.LifeSkillItem lifeSkillItem = LifeSkill.Instance.GetItem((short)i);
			CommonUtils.GetUnlockBuildingListFromConfig(lifeSkillItem, lockBuildingList);
			this._lockBuildingList.AddRange(lockBuildingList);
		}
	}

	// Token: 0x0600192F RID: 6447 RVA: 0x000A39F0 File Offset: 0x000A1BF0
	public static int GetPracticeCombatSkillCostActionPoint(int buildingLevel)
	{
		int cost = GlobalConfig.Instance.CombatSkillPracticeActionPointCost[0] - (buildingLevel - 1) * GlobalConfig.Instance.CombatSkillPracticeActionPointCost[1];
		return cost / 10 * 10;
	}

	// Token: 0x06001930 RID: 6448 RVA: 0x000A3A28 File Offset: 0x000A1C28
	private List<short> GetCurrOrganizationCanPracticeSkills(short organizationTemplateId)
	{
		List<short> canPracticeSkills = new List<short>();
		bool isTaiwuVillageBuilding = this._isTaiwuVillageBuilding;
		if (isTaiwuVillageBuilding)
		{
			canPracticeSkills.AddRange(this._learnedCombatSkillItems);
		}
		else
		{
			for (int i = this._learnedCombatSkillItems.Count - 1; i >= 0; i--)
			{
				CombatSkillItem config = CombatSkill.Instance[this._learnedCombatSkillItems[i]];
				bool flag = (short)config.SectId == organizationTemplateId;
				if (flag)
				{
					canPracticeSkills.Add(config.TemplateId);
				}
			}
		}
		return canPracticeSkills;
	}

	// Token: 0x06001931 RID: 6449 RVA: 0x000A3AB4 File Offset: 0x000A1CB4
	private unsafe void UpdateExpandRemoveCollectOperators()
	{
		int togKey = this._mainToggleGroup.GetActive().Key;
		bool flag = togKey == 1;
		if (!flag)
		{
			for (int i = 0; i < 3; i++)
			{
				UI_BuildingManage.<>c__DisplayClass286_0 CS$<>8__locals1 = new UI_BuildingManage.<>c__DisplayClass286_0();
				CS$<>8__locals1.<>4__this = this;
				CS$<>8__locals1.charId = this._operatorListCached[i];
				Transform child = this._expandManpowerHolder.transform.GetChild(i);
				ResidentView residentView = child.GetComponent<ResidentView>();
				bool flag2 = residentView == null;
				if (!flag2)
				{
					residentView.RenderShopCharInfo(CS$<>8__locals1.charId);
					Refers operatorRefers = child.GetComponent<Refers>();
					bool flag3 = CS$<>8__locals1.charId != -1;
					if (flag3)
					{
						TextMeshProUGUI workTypeText = operatorRefers.CGet<TextMeshProUGUI>("WorkTypeText");
						bool flag4 = SingletonObject.getInstance<TutorialChapterModel>().InGuiding && SingletonObject.getInstance<TutorialChapterModel>().TutorialChapterIndex == 1;
						if (flag4)
						{
							workTypeText.SetText(LocalStringManager.Get(LanguageKey.UI_VillagerWork_Build), true);
							CharacterItem charConfig = Character.Instance.GetItem(908);
							short value = *(ref charConfig.BaseLifeSkillQualifications.Items.FixedElementField + (IntPtr)15 * 2);
							this._propertyValueDict.Clear();
							this._propertyValueDict.Add(SingletonObject.getInstance<BasicGameData>().TaiwuCharId, value);
						}
						else
						{
							bool flag5 = this._buildingModel.VillagerWork.ContainsKey(CS$<>8__locals1.charId) && (this._buildingModel.VillagerWork[CS$<>8__locals1.charId].WorkType == 0 || this._buildingModel.VillagerWork[CS$<>8__locals1.charId].WorkType == 1);
							if (flag5)
							{
								VillagerWorkData workData = this._buildingModel.VillagerWork[CS$<>8__locals1.charId];
								Location location = new Location(workData.AreaId, workData.BlockId);
								MapDomainMethod.AsyncCall.IsLocationInBuildingEffectRange(this, this._charDisplayDataDict[CS$<>8__locals1.charId].Location, location, delegate(int offset, RawDataPool dataPool)
								{
									bool isInBuildingEffectRange = false;
									Serializer.Deserialize(dataPool, offset, ref isInBuildingEffectRange);
									bool flag15 = isInBuildingEffectRange;
									if (flag15)
									{
										bool flag16 = CS$<>8__locals1.<>4__this._buildingModel.VillagerWork.ContainsKey(CS$<>8__locals1.charId) && CS$<>8__locals1.<>4__this._buildingModel.VillagerWork[CS$<>8__locals1.charId].WorkType == 0;
										if (flag16)
										{
											workTypeText.SetText(LocalStringManager.Get(LanguageKey.UI_VillagerWork_Build), true);
										}
										else
										{
											workTypeText.SetText(LocalStringManager.Get(string.Format("LK_WorkType_{0}", workData.WorkType)), true);
										}
									}
									else
									{
										workTypeText.SetText(LocalStringManager.Get(LanguageKey.UI_VillagerWork_Move), true);
									}
								});
							}
							else
							{
								bool flag6 = this._buildingModel.VillagerWork.ContainsKey(CS$<>8__locals1.charId) && this._buildingModel.VillagerWork[CS$<>8__locals1.charId].WorkType >= 10;
								if (flag6)
								{
									VillagerWorkData workData2 = this._buildingModel.VillagerWork[CS$<>8__locals1.charId];
									bool flag7 = workData2.AreaId == this._charDisplayDataDict[CS$<>8__locals1.charId].Location.AreaId && workData2.BlockId == this._charDisplayDataDict[CS$<>8__locals1.charId].Location.BlockId;
									if (flag7)
									{
										bool flag8 = this._buildingModel.VillagerWork[CS$<>8__locals1.charId].WorkType == 0;
										if (flag8)
										{
											workTypeText.SetText(LocalStringManager.Get(LanguageKey.UI_VillagerWork_Build), true);
										}
										else
										{
											workTypeText.SetText(LocalStringManager.Get(string.Format("LK_WorkType_{0}", workData2.WorkType)), true);
										}
									}
									else
									{
										workTypeText.SetText(LocalStringManager.Get(LanguageKey.UI_VillagerWork_Move), true);
									}
								}
								else
								{
									workTypeText.SetText(LocalStringManager.Get(LanguageKey.UI_VillagerWork_Idle), true);
								}
							}
						}
						Refers personalityRefers = operatorRefers.CGet<Refers>("Personality");
						TextMeshProUGUI personalityCount = personalityRefers.CGet<TextMeshProUGUI>("PersonalityCount");
						CharacterDomainMethod.AsyncCall.GetGroupCharDisplayDataList(this, new List<int>
						{
							CS$<>8__locals1.charId
						}, delegate(int offset, RawDataPool pool)
						{
							List<GroupCharDisplayData> dataList = new List<GroupCharDisplayData>();
							Serializer.Deserialize(pool, offset, ref dataList);
							int personality = dataList[0].Personalities.GetSum();
							personalityCount.text = personality.ToString();
						});
					}
					CS$<>8__locals1.index = i;
					CS$<>8__locals1.charIdList = (from id in this._availableWorker
					where !this._operatorListCached.Contains(id)
					select id).ToList<int>();
					CS$<>8__locals1.skillValueDict = new Dictionary<int, short>();
					bool flag9 = this._configData.RequireLifeSkillType != -1;
					if (flag9)
					{
						for (int j = 0; j < CS$<>8__locals1.charIdList.Count; j++)
						{
							bool flag10 = CS$<>8__locals1.charIdList[j] < 0;
							if (!flag10)
							{
								int charId = CS$<>8__locals1.charIdList[j];
								sbyte requireLifeSkillType = this._configData.RequireLifeSkillType;
								AsyncMethodCallbackDelegate callback;
								if ((callback = CS$<>8__locals1.<>9__4) == null)
								{
									callback = (CS$<>8__locals1.<>9__4 = delegate(int offset, RawDataPool dataPool)
									{
										ValueTuple<int, int> skillValue = new ValueTuple<int, int>(-1, -1);
										Serializer.Deserialize(dataPool, offset, ref skillValue);
										bool flag15 = skillValue.Item1 > -1;
										if (flag15)
										{
											bool flag16 = !CS$<>8__locals1.skillValueDict.ContainsKey(skillValue.Item1);
											if (flag16)
											{
												CS$<>8__locals1.skillValueDict.Add(skillValue.Item1, (short)skillValue.Item2);
											}
										}
									});
								}
								CharacterDomainMethod.AsyncCall.GetLifeSkillAttainment(this, charId, requireLifeSkillType, callback);
							}
						}
					}
					else
					{
						for (int k = 0; k < CS$<>8__locals1.charIdList.Count; k++)
						{
							bool flag11 = CS$<>8__locals1.charIdList[k] < 0;
							if (!flag11)
							{
								int charId2 = CS$<>8__locals1.charIdList[k];
								sbyte requireCombatSkillType = this._configData.RequireCombatSkillType;
								AsyncMethodCallbackDelegate callback2;
								if ((callback2 = CS$<>8__locals1.<>9__5) == null)
								{
									callback2 = (CS$<>8__locals1.<>9__5 = delegate(int offset, RawDataPool dataPool)
									{
										ValueTuple<int, int> skillValue = new ValueTuple<int, int>(-1, -1);
										Serializer.Deserialize(dataPool, offset, ref skillValue);
										bool flag15 = skillValue.Item1 > -1;
										if (flag15)
										{
											bool flag16 = !CS$<>8__locals1.skillValueDict.ContainsKey(skillValue.Item1);
											if (flag16)
											{
												CS$<>8__locals1.skillValueDict.Add(skillValue.Item1, (short)skillValue.Item2);
											}
										}
									});
								}
								CharacterDomainMethod.AsyncCall.GetCombatSkillAttainment(this, charId2, requireCombatSkillType, callback2);
							}
						}
					}
					operatorRefers.CGet<CButtonObsolete>("SelectCharBtn").ClearAndAddListener(new Action(CS$<>8__locals1.<UpdateExpandRemoveCollectOperators>g__OpenSelectChar|1));
					operatorRefers.CGet<CButtonObsolete>("ChangeButton").ClearAndAddListener(new Action(CS$<>8__locals1.<UpdateExpandRemoveCollectOperators>g__OpenSelectChar|1));
				}
			}
			int peopleNumber = 0;
			for (int l = 0; l < this._operatorListCached.Length; l++)
			{
				bool flag12 = this._operatorListCached[l] != -1;
				if (flag12)
				{
					peopleNumber++;
				}
			}
			this._stringBuilder.Clear();
			bool flag13 = togKey == 0;
			if (flag13)
			{
				this._stringBuilder.Append(LocalStringManager.Get("LK_Building_ArrangeManpower")).Append("  (").Append(peopleNumber).Append("/").Append(BuildingBlockData.IsUsefulResource(this._configData.Type) ? 1 : 3).Append(")");
			}
			else
			{
				bool flag14 = togKey == 3;
				if (flag14)
				{
					this._stringBuilder.Append(LocalStringManager.Get("LK_Building_RemoveManpower")).Append("  (").Append(peopleNumber).Append("/").Append(3).Append(")");
				}
			}
			this._removeCollectInfoPage.CGet<TextMeshProUGUI>("TitleExpandText").text = this._stringBuilder.ToString();
			this.SetOperationLeftTimeString(this._operatorListCached.ToList<int>());
			this.UpdateButtonMouseTip();
		}
	}

	// Token: 0x170002AA RID: 682
	// (get) Token: 0x06001932 RID: 6450 RVA: 0x000A41C6 File Offset: 0x000A23C6
	private List<int> ShopManagerList
	{
		get
		{
			return this._buildingModel.GetBuildingShopManager(this._blockKey);
		}
	}

	// Token: 0x06001933 RID: 6451 RVA: 0x000A41DC File Offset: 0x000A23DC
	private void UpdateShopManagers()
	{
		sbyte lifeSkillType = this.GetOperationNeedSkillType();
		this.UpdateShopManagers(lifeSkillType);
	}

	// Token: 0x06001934 RID: 6452 RVA: 0x000A41FC File Offset: 0x000A23FC
	private void UpdateShopManagers(sbyte lifeSkillType)
	{
		for (int i = 0; i < 7; i++)
		{
			int charId = this.ShopManagerList[i];
			ResidentView residentView = (i == 0) ? this._shopInfoPage.CGet<ResidentView>("ManagerLeaderView") : this._shopInfoPage.CGet<RectTransform>("MemberHolder").transform.GetChild(i - 1).GetComponent<ResidentView>();
			residentView.gameObject.SetActive(true);
			bool flag = i == 0;
			if (flag)
			{
				residentView.RenderManagerLeader(charId, this._configData, this._blockKey, new Action<int>(this.ActionWhenQuickAssignRole));
			}
			else
			{
				residentView.RenderManagerMemberInfo(charId, this._configData, this._blockKey);
			}
			Refers childRefer = residentView.GetComponent<Refers>();
			this.RefreshSelectCharacterBtn(i, childRefer, 1, lifeSkillType);
		}
		this.UpdateResourceInfo();
		this.UpdateManageEfficiency();
		this.UpdateProductRequireTip();
		TaiwuDomainMethod.Call.GetVillagerRoleCharacterDisplayDataOnPanel(this.Element.GameDataListenerId);
	}

	// Token: 0x06001935 RID: 6453 RVA: 0x000A42F0 File Offset: 0x000A24F0
	private void UpdateResourceInfo()
	{
		GameObject resourceOutput = this._shopInfoPage.CGet<GameObject>("ResourceOutput");
		GameObject resourceOutputInfo = this._shopInfoPage.CGet<GameObject>("ResourceOutputInfo");
		bool flag = this._shopEventData != null && this._shopEventData.ResourceList.Count > 0;
		if (flag)
		{
			resourceOutput.SetActive(true);
			for (int i = 0; i < this._shopEventData.ResourceList.Count; i++)
			{
				Refers resourceRefers = this._reuseDic[resourceOutputInfo.name][i].GetComponent<Refers>();
				sbyte resourceType = this._shopEventData.ResourceList[i];
				this.SetResourceInfo(resourceType, resourceRefers);
			}
		}
		else
		{
			resourceOutput.SetActive(false);
		}
	}

	// Token: 0x06001936 RID: 6454 RVA: 0x000A43B7 File Offset: 0x000A25B7
	private void UpdateManageEfficiency()
	{
		BuildingDomainMethod.AsyncCall.GetBuildingFormulaContextBridge(this, this._blockKey, delegate(int offset, RawDataPool dataPool)
		{
			Serializer.Deserialize(dataPool, offset, ref this._formulaContext);
		});
		BuildingDomainMethod.AsyncCall.GetBuildingAttainment(this, this._blockData, this._blockKey, delegate(int offset, RawDataPool dataPool)
		{
			this._currentAttainment = 0;
			Serializer.Deserialize(dataPool, offset, ref this._currentAttainment);
			bool flag = this._configData.TemplateId == 105;
			if (flag)
			{
				this.UpdateBookCollectionRoomInfo(this._bookCollectionData);
			}
			else
			{
				this._shopInfoPage.CGet<TextMeshProUGUI>("ProduceProgressText").SetText(this.GetPredictProgressText(), true);
			}
			this.UpdateShopProgress();
			this.UpdateCalculates();
		});
	}

	// Token: 0x06001937 RID: 6455 RVA: 0x000A43F4 File Offset: 0x000A25F4
	private void RefreshWorkInfoAndLifeSkill(int charId, sbyte lifeSkillType, Refers childRefer)
	{
		bool flag = charId < 0;
		if (!flag)
		{
			TextMeshProUGUI workTypeText = childRefer.CGet<TextMeshProUGUI>("WorkTypeText");
			bool flag2 = this._buildingModel.VillagerWork.ContainsKey(charId) && (this._buildingModel.VillagerWork[charId].WorkType == 0 || this._buildingModel.VillagerWork[charId].WorkType == 1);
			if (flag2)
			{
				VillagerWorkData workData = this._buildingModel.VillagerWork[charId];
				Location location = new Location(workData.AreaId, workData.BlockId);
				MapDomainMethod.AsyncCall.IsLocationInBuildingEffectRange(this, this._charDisplayDataDict[charId].Location, location, delegate(int offset, RawDataPool dataPool)
				{
					bool isInBuildingEffectRange = false;
					Serializer.Deserialize(dataPool, offset, ref isInBuildingEffectRange);
					bool flag8 = isInBuildingEffectRange;
					if (flag8)
					{
						bool flag9 = this._buildingModel.VillagerWork.ContainsKey(charId) && this._buildingModel.VillagerWork[charId].WorkType == 0;
						if (flag9)
						{
							workTypeText.SetText(LocalStringManager.Get(LanguageKey.UI_VillagerWork_Build), true);
						}
						else
						{
							workTypeText.SetText(LocalStringManager.Get(string.Format("LK_WorkType_{0}", workData.WorkType)), true);
						}
					}
					else
					{
						workTypeText.SetText(LocalStringManager.Get(LanguageKey.UI_VillagerWork_Move), true);
					}
				});
			}
			else
			{
				bool flag3 = this._buildingModel.VillagerWork.ContainsKey(charId) && this._buildingModel.VillagerWork[charId].WorkType >= 10;
				if (flag3)
				{
					VillagerWorkData workData2 = this._buildingModel.VillagerWork[charId];
					bool flag4 = workData2.AreaId == this._charDisplayDataDict[charId].Location.AreaId && workData2.BlockId == this._charDisplayDataDict[charId].Location.BlockId;
					if (flag4)
					{
						bool flag5 = this._buildingModel.VillagerWork[charId].WorkType == 0;
						if (flag5)
						{
							workTypeText.SetText(LocalStringManager.Get(LanguageKey.UI_VillagerWork_Build), true);
						}
						else
						{
							workTypeText.SetText(LocalStringManager.Get(string.Format("LK_WorkType_{0}", workData2.WorkType)), true);
						}
					}
					else
					{
						workTypeText.SetText(LocalStringManager.Get(LanguageKey.UI_VillagerWork_Move), true);
					}
				}
				else
				{
					workTypeText.SetText(LocalStringManager.Get(LanguageKey.UI_VillagerWork_Idle), true);
				}
			}
			Refers lifeSkillRefer = childRefer.CGet<Refers>("LifeSkill");
			bool flag6 = this.IsDependKungfuPracticeRoom(this._configData);
			if (flag6)
			{
				CharacterDomainMethod.AsyncCall.GetCharacterMaxCombatSkillAttainment(this, charId, delegate(int offset, RawDataPool dataPool)
				{
					ValueTuple<sbyte, short> combatSkillAttainment = new ValueTuple<sbyte, short>(0, 0);
					Serializer.Deserialize(dataPool, offset, ref combatSkillAttainment);
					lifeSkillRefer.CGet<CImage>("LifeSkillIcon").SetSprite(CombatSkillType.Instance[combatSkillAttainment.Item1].DisplayIcon, false, null);
					lifeSkillRefer.CGet<TextMeshProUGUI>("LifeSkillName").SetText(CombatSkillType.Instance[combatSkillAttainment.Item1].Name, true);
					lifeSkillRefer.CGet<TextMeshProUGUI>("LifeSkillCount").SetText(combatSkillAttainment.Item2.ToString(), true);
				});
			}
			else
			{
				lifeSkillRefer.CGet<CImage>("LifeSkillIcon").SetSprite(Config.LifeSkillType.Instance[lifeSkillType].DisplayIcon, false, null);
				lifeSkillRefer.CGet<TextMeshProUGUI>("LifeSkillName").SetText(Config.LifeSkillType.Instance[lifeSkillType].Name, true);
				bool flag7 = this._lifeSkillAttainmentDict.ContainsKey(charId);
				if (flag7)
				{
					lifeSkillRefer.CGet<TextMeshProUGUI>("LifeSkillCount").SetText(this._lifeSkillAttainmentDict[charId][(int)lifeSkillType].ToString(), true);
				}
			}
		}
	}

	// Token: 0x06001938 RID: 6456 RVA: 0x000A4750 File Offset: 0x000A2950
	private void RefreshSelectCharacterBtn(int i, Refers childRefer, byte charPrefabType, sbyte lifeSkillType)
	{
		UI_BuildingManage.<>c__DisplayClass294_0 CS$<>8__locals1 = new UI_BuildingManage.<>c__DisplayClass294_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.index = i;
		CS$<>8__locals1.charIdList = (from id in this._availableWorker
		where !CS$<>8__locals1.<>4__this._shopManagerListCached.Contains(id)
		select id).ToList<int>();
		CS$<>8__locals1.skillValueDict = new Dictionary<int, short>();
		bool flag = this.IsDependKungfuPracticeRoom(this._configData) && this._configData.IsShop;
		if (flag)
		{
			for (int j = 0; j < CS$<>8__locals1.charIdList.Count; j++)
			{
				bool flag2 = CS$<>8__locals1.charIdList[j] < 0;
				if (!flag2)
				{
					int characterId = CS$<>8__locals1.charIdList[j];
					CharacterDomainMethod.AsyncCall.GetCharacterMaxCombatSkillAttainment(this, characterId, delegate(int offset, RawDataPool dataPool)
					{
						ValueTuple<sbyte, short> skillValue = new ValueTuple<sbyte, short>(-1, -1);
						Serializer.Deserialize(dataPool, offset, ref skillValue);
						CS$<>8__locals1.skillValueDict.Add(characterId, skillValue.Item2);
					});
				}
			}
		}
		else
		{
			for (int k = 0; k < CS$<>8__locals1.charIdList.Count; k++)
			{
				bool flag3 = CS$<>8__locals1.charIdList[k] < 0;
				if (!flag3)
				{
					int charId = CS$<>8__locals1.charIdList[k];
					AsyncMethodCallbackDelegate callback;
					if ((callback = CS$<>8__locals1.<>9__3) == null)
					{
						callback = (CS$<>8__locals1.<>9__3 = delegate(int offset, RawDataPool dataPool)
						{
							ValueTuple<int, int> skillValue = new ValueTuple<int, int>(-1, -1);
							Serializer.Deserialize(dataPool, offset, ref skillValue);
							bool flag4 = skillValue.Item1 > -1;
							if (flag4)
							{
								CS$<>8__locals1.skillValueDict.Add(skillValue.Item1, (short)skillValue.Item2);
							}
						});
					}
					CharacterDomainMethod.AsyncCall.GetLifeSkillAttainment(this, charId, lifeSkillType, callback);
				}
			}
		}
		childRefer.CGet<CButtonObsolete>("SelectCharBtn").ClearAndAddListener(new Action(CS$<>8__locals1.<RefreshSelectCharacterBtn>g__OpenSelectChar|1));
		childRefer.CGet<CButtonObsolete>("ChangeButton").ClearAndAddListener(new Action(CS$<>8__locals1.<RefreshSelectCharacterBtn>g__OpenSelectChar|1));
	}

	// Token: 0x06001939 RID: 6457 RVA: 0x000A48E8 File Offset: 0x000A2AE8
	private void UpdateShopManagersByResourceType(sbyte selectResourceType)
	{
		sbyte lifeSkillType = this._configData.RequireLifeSkillType;
		this.UpdateShopManagers(lifeSkillType);
	}

	// Token: 0x0600193A RID: 6458 RVA: 0x000A490A File Offset: 0x000A2B0A
	public void SelectShopManager(int charId)
	{
		BuildingDomainMethod.Call.SetShopManager(this._blockKey, (sbyte)this._selectingShopManagerIndex, charId);
	}

	// Token: 0x0600193B RID: 6459 RVA: 0x000A4924 File Offset: 0x000A2B24
	public void UnSelectShopManager()
	{
		for (sbyte i = 0; i < 7; i += 1)
		{
			bool flag = !this.CurrInfluenceLeader && i == 0;
			if (!flag)
			{
				bool flag2 = !this.CurrInfluenceMember && i != 0;
				if (!flag2)
				{
					BuildingDomainMethod.Call.SetShopManager(this._blockKey, i, -1);
				}
			}
		}
	}

	// Token: 0x0600193C RID: 6460 RVA: 0x000A4980 File Offset: 0x000A2B80
	private void ShowSelectCharWithFilter(List<int> charIdList, Action<int> onSelect)
	{
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
		argumentBox.Clear();
		argumentBox.SetObject("callback", onSelect);
		argumentBox.SetObject("charIdList", charIdList);
		argumentBox.SetObject("filterType", new List<global::CharacterTable.CharacterTableCommonFilterTypes>
		{
			global::CharacterTable.CharacterTableCommonFilterTypes.Resident
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
	}

	// Token: 0x0600193D RID: 6461 RVA: 0x000A4A58 File Offset: 0x000A2C58
	private void UpdateShopInfoPage()
	{
		bool isEntertain = this.IsEntertain;
		if (!isEntertain)
		{
			this.UpdateShopTitle();
			this.UpdateShopManagers();
			this.UpdateAutoWorkAndSold();
			this.RefreshAllQuickSelectButtons();
			this.UpdateVillagerManagerBtn();
			this.UpdateArrangementSettingBtn();
			this.InitSoldItemSetting();
		}
	}

	// Token: 0x0600193E RID: 6462 RVA: 0x000A4AA3 File Offset: 0x000A2CA3
	private void UpdateVillagerManagerBtn()
	{
		this._shopInfoPage.CGet<CButtonObsolete>("VillagerManagerBtn").ClearAndAddListener(delegate
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("EnterType", ViewVillagerRole.EnterType.Normal);
			argBox.Set("EnterPage", ViewVillagerRole.EVillagerRolePage.RoleDescription);
			argBox.Set("RoleTemplateId", this._configData.VillagerRoleTemplateIds[0]);
			UIElement.VillagerRole.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.VillagerRole, true);
		});
	}

	// Token: 0x0600193F RID: 6463 RVA: 0x000A4AC8 File Offset: 0x000A2CC8
	private string GetPredictProgressText()
	{
		int delta = this.ShopManageProgressDelta * this._shopProgressBonus;
		int needProgress = (int)this._configData.MaxProduceValue;
		SingletonObject.getInstance<BasicGameData>().ChallengeModeData.ApplyChallengeModeBuildingWorkHard(ref needProgress);
		return UI_BuildingManage.GetPredictProgressText((int)this._blockData.ShopProgress, delta, needProgress);
	}

	// Token: 0x06001940 RID: 6464 RVA: 0x000A4B1C File Offset: 0x000A2D1C
	public static string GetPredictProgressText(int progressDelta, CValuePercentBonus bonus, BuildingBlockItem configData, BuildingBlockData blockData)
	{
		int delta = progressDelta * bonus;
		int needProgress = (int)configData.MaxProduceValue;
		SingletonObject.getInstance<BasicGameData>().ChallengeModeData.ApplyChallengeModeBuildingWorkHard(ref needProgress);
		return UI_BuildingManage.GetPredictProgressText((int)blockData.ShopProgress, delta, needProgress);
	}

	// Token: 0x06001941 RID: 6465 RVA: 0x000A4B5C File Offset: 0x000A2D5C
	public static string GetPredictProgressText(int cur, int delta, int max)
	{
		return string.Format("{0}+{1}{2}{3}", new object[]
		{
			cur.ToString().SetColor("pinkyellow"),
			Math.Min(delta, max),
			"/".SetColor("pinkyellow"),
			max.ToString().SetColor("pinkyellow")
		});
	}

	// Token: 0x170002AB RID: 683
	// (get) Token: 0x06001942 RID: 6466 RVA: 0x000A4BC2 File Offset: 0x000A2DC2
	private int ShopManageProgressDelta
	{
		get
		{
			return (this.ShopManagerList[0] > -1 || !this._configData.NeedLeader) ? GameData.Domains.Building.SharedMethods.GetShopManageProgressDelta(this._blockData.TemplateId, this._currentAttainment) : 0;
		}
	}

	// Token: 0x06001943 RID: 6467 RVA: 0x000A4BFC File Offset: 0x000A2DFC
	public static int GetShopManageProgressDelta(List<int> shopManagerList, BuildingBlockItem configData, BuildingBlockData blockData, int currentAttainment)
	{
		return (shopManagerList[0] > -1 || !configData.NeedLeader) ? GameData.Domains.Building.SharedMethods.GetShopManageProgressDelta(blockData.TemplateId, currentAttainment) : 0;
	}

	// Token: 0x06001944 RID: 6468 RVA: 0x000A4C34 File Offset: 0x000A2E34
	private int ShopManageProgressEffectDelta()
	{
		int baseDelta = this.ShopManageProgressDelta;
		return baseDelta * this._shopProgressBonus - baseDelta;
	}

	// Token: 0x06001945 RID: 6469 RVA: 0x000A4C60 File Offset: 0x000A2E60
	private void UpdateShopProgress()
	{
		GameObject produceProgressBar = this._shopInfoPage.CGet<GameObject>("ProduceProgressBar");
		TooltipInvoker tips = produceProgressBar.GetComponent<TooltipInvoker>();
		produceProgressBar.SetActive(this._configData.MaxProduceValue > 0);
		this._shopInfoPage.CGet<CImage>("ProduceProgressGauge").fillAmount = this._blockData.ShopProgressFill;
		this.SetProduceProgressInfo(tips);
	}

	// Token: 0x06001946 RID: 6470 RVA: 0x000A4CC4 File Offset: 0x000A2EC4
	private void SetProduceProgressInfo(TooltipInvoker tips)
	{
		this._shopInfoPage.CGet<TextMeshProUGUI>("ProduceProgressText").text = this.GetPredictProgressText();
		GeneralLineData desc = new GeneralLineData
		{
			Type = 3,
			Args = new List<string>
			{
				LocalStringManager.Get(LanguageKey.LK_Building_ManageProduceValue_Tips_Text)
			}
		};
		GeneralLineData title = new GeneralLineData
		{
			Type = 3,
			Args = new List<string>
			{
				LocalStringManager.GetFormat(LanguageKey.LK_Brackets_Symbol, LocalStringManager.Get(LanguageKey.LK_Building_ManageProduceValue_Tips_Title))
			}
		};
		GeneralLineData contentProgress = new GeneralLineData
		{
			Type = 5,
			Args = new List<string>
			{
				LocalStringManager.GetFormat(LanguageKey.LK_Building_ManageProduceValue_Tips_ContentNormal, this.ShopManageProgressDelta)
			},
			ExtraArgs = new List<object>
			{
				20
			}
		};
		GeneralLineData contentEffect = new GeneralLineData
		{
			Type = 5,
			Args = new List<string>
			{
				LocalStringManager.GetFormat(LanguageKey.LK_Building_ManageProduceValue_Tips_ContentExp, this.ShopManageProgressEffectDelta())
			},
			ExtraArgs = new List<object>
			{
				20
			}
		};
		int lineCount = 3;
		TooltipInvoker tips2 = tips;
		if (tips2.RuntimeParam == null)
		{
			tips2.RuntimeParam = new ArgumentBox();
		}
		tips.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_Building_ManageProduceValue_Tips_Title)).SetObject("LineData1", desc).SetObject("LineData2", title).SetObject("LineData3", contentProgress);
		bool flag = this._shopProgressBonus > 0;
		if (flag)
		{
			int lineCount3 = lineCount;
			lineCount = lineCount3 + 1;
			tips.RuntimeParam.SetObject(string.Format("LineData{0}", lineCount), contentEffect);
		}
		ShopEventItem shopEvent = null;
		bool flag2 = this._configData.SuccesEvent.CheckIndex(0);
		if (flag2)
		{
			shopEvent = ShopEvent.Instance.GetItem(this._configData.SuccesEvent[0]);
		}
		bool flag3 = shopEvent != null && (GameData.Domains.Building.SharedMethods.IsBuildingProduceMoneyAuthority(this._configData, shopEvent) || GameData.Domains.Building.SharedMethods.IsBuildingSoldItem(this._configData, shopEvent) || shopEvent.RecruitPeopleProb.Count != 0);
		if (flag3)
		{
			BuildingDomainMethod.AsyncCall.CalculateBuildingManageHarvestSuccessRates(this, this._blockKey, delegate(int offset2, RawDataPool pool2)
			{
				int[] rates = new int[]
				{
					0,
					-1
				};
				Serializer.Deserialize(pool2, offset2, ref rates);
				int rate2 = rates[1];
				bool giveRate2 = false;
				LanguageKey languageKey = LanguageKey.LK_Building_ManageProduceValue_Tips_Text_10;
				short templateId = this._configData.TemplateId;
				short num = templateId;
				if (num != 215)
				{
					if (num == 216)
					{
						giveRate2 = true;
						languageKey = LanguageKey.LK_Building_ManageProduceValue_Tips_Text_11;
					}
				}
				else
				{
					giveRate2 = true;
					languageKey = LanguageKey.LK_Building_ManageProduceValue_Tips_Text_10;
				}
				bool flag4 = rate2 >= 0 && giveRate2;
				if (flag4)
				{
					GeneralLineData gamblingHouseOrBrothel = new GeneralLineData
					{
						Type = 3,
						Args = new List<string>
						{
							LocalStringManager.Get(languageKey)
						}
					};
					GeneralLineData gamblingHouseOrBrothelValue = new GeneralLineData
					{
						Type = 5,
						Args = new List<string>
						{
							LocalStringManager.GetFormat(LanguageKey.LK_Building_ManageProduceValue_Tips_Text_9, rate2, (rate2 < 100) ? "pinkyellow" : "lightblue")
						}
					};
					int lineCount2 = lineCount;
					lineCount = lineCount2 + 1;
					tips.RuntimeParam.SetObject(string.Format("LineData{0}", lineCount), gamblingHouseOrBrothel);
					lineCount2 = lineCount;
					lineCount = lineCount2 + 1;
					tips.RuntimeParam.SetObject(string.Format("LineData{0}", lineCount), gamblingHouseOrBrothelValue);
				}
			});
			tips.RuntimeParam.Set("LineCount", lineCount);
		}
		else
		{
			tips.RuntimeParam.Set("LineCount", lineCount);
		}
	}

	// Token: 0x06001947 RID: 6471 RVA: 0x000A4F7C File Offset: 0x000A317C
	private void UpdateAutoWorkAndSold()
	{
		BuildingDomainMethod.AsyncCall.GetBuildingIsAutoWork(this, this._buildingBlockIndex, delegate(int offset, RawDataPool dataPool)
		{
			bool isAutoArrange = false;
			Serializer.Deserialize(dataPool, offset, ref isAutoArrange);
			this._autoArrangeToggle.onValueChanged.RemoveAllListeners();
			this._autoArrangeToggle.isOn = isAutoArrange;
			this._autoArrangeToggle.onValueChanged.AddListener(delegate(bool isOn)
			{
				BuildingDomainMethod.Call.SetBuildingAutoWork(this._buildingBlockIndex, isOn);
			});
		});
		bool flag = this._shopEventData != null && this._shopEventData.ExchangeResourceGoods != -1;
		if (flag)
		{
			BuildingDomainMethod.AsyncCall.GetBuildingIsAutoSold(this, this._buildingBlockIndex, delegate(int offset, RawDataPool dataPool)
			{
				bool isAutoSold = false;
				Serializer.Deserialize(dataPool, offset, ref isAutoSold);
				this._autoSoldToggle.onValueChanged.RemoveAllListeners();
				this._autoSoldToggle.isOn = isAutoSold;
				this._autoSoldToggle.onValueChanged.AddListener(delegate(bool isOn)
				{
					BuildingDomainMethod.Call.SetBuildingAutoSold(this._buildingBlockIndex, isOn);
				});
			});
		}
	}

	// Token: 0x06001948 RID: 6472 RVA: 0x000A4FE0 File Offset: 0x000A31E0
	[return: TupleElementNames(new string[]
	{
		"iconName",
		"requireName"
	})]
	public static ValueTuple<string, string> GetBuildingRequireIconAndName(BuildingBlockItem buildingConfig)
	{
		bool flag = buildingConfig.RequireLifeSkillType >= 0;
		ValueTuple<string, string> result;
		if (flag)
		{
			sbyte type = buildingConfig.RequireLifeSkillType;
			LifeSkillTypeItem config = Config.LifeSkillType.Instance[type];
			result = new ValueTuple<string, string>(config.DisplayIcon, config.Name);
		}
		else
		{
			bool flag2 = buildingConfig.RequireCombatSkillType >= 0;
			if (flag2)
			{
				sbyte type2 = buildingConfig.RequireCombatSkillType;
				CombatSkillTypeItem config2 = CombatSkillType.Instance[type2];
				result = new ValueTuple<string, string>(config2.DisplayIcon, config2.Name);
			}
			else
			{
				result = new ValueTuple<string, string>(string.Empty, string.Empty);
			}
		}
		return result;
	}

	// Token: 0x06001949 RID: 6473 RVA: 0x000A5078 File Offset: 0x000A3278
	private sbyte GetShopManagerCount()
	{
		List<int> managerList = this.ShopManagerList;
		sbyte count = 0;
		for (int i = 0; i < managerList.Count; i++)
		{
			bool flag = managerList[i] != -1;
			if (flag)
			{
				count += 1;
			}
		}
		return count;
	}

	// Token: 0x0600194A RID: 6474 RVA: 0x000A50C8 File Offset: 0x000A32C8
	private int GetLeaderCount()
	{
		List<int> managerList = this.ShopManagerList;
		return (managerList[0] >= 0) ? 1 : 0;
	}

	// Token: 0x0600194B RID: 6475 RVA: 0x000A50F0 File Offset: 0x000A32F0
	private int GetMemberCount()
	{
		List<int> managerList = this.ShopManagerList;
		sbyte count = 0;
		for (int i = 0; i < managerList.Count; i++)
		{
			bool flag = managerList[i] != -1;
			if (flag)
			{
				count += 1;
			}
		}
		return (int)count - this.GetLeaderCount();
	}

	// Token: 0x0600194C RID: 6476 RVA: 0x000A5144 File Offset: 0x000A3344
	private bool UpdateShopOutputInfo(int slotCount)
	{
		bool isShow = this._shopEventData != null && (this._shopEventData.ResourceGoods != -1 || this._shopEventData.ItemList.Count > 0 || this._shopEventData.ItemGradeProbList.Count > 0);
		this._shopInfoPage.CGet<GameObject>("ItemOutput").SetActive(isShow);
		bool flag = !isShow;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			TextMeshProUGUI itemOutputText = this._shopInfoPage.CGet<TextMeshProUGUI>("ItemOutputText");
			itemOutputText.SetText(LocalStringManager.Get(LanguageKey.LK_Building_ShopItemOutput), true);
			GameObject itemResourceButton = this._shopInfoPage.CGet<GameObject>("ItemResourceButton");
			GameObject itemResourceButtonHolder = this._shopInfoPage.CGet<GameObject>("ItemResourceButtonHolder");
			this.ReuseGameObjectFunc(itemResourceButton, slotCount, itemResourceButtonHolder.transform);
			bool isMoney = this._shopEventData.ResourceGoods == 6;
			bool isMultiplyItem = this._shopEventData.ItemGradeProbList.Count > 0;
			bool isItems = this._shopEventData.ItemList.Count > 0 || isMultiplyItem;
			bool showTog = isMoney || isItems;
			CToggleGroupObsolete outputStorageToggleGroup = this._shopInfoPage.CGet<CToggleGroupObsolete>("OutputStorageToggleGroup");
			outputStorageToggleGroup.gameObject.SetActive(showTog);
			bool flag2 = showTog;
			if (flag2)
			{
				outputStorageToggleGroup.InitPreOnToggle(-1);
				outputStorageToggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnOutputStorageToggleChange);
				bool flag3 = isMoney;
				sbyte resourceType;
				bool isItem;
				bool isMakeMaterial;
				if (flag3)
				{
					resourceType = 6;
					isItem = false;
					isMakeMaterial = false;
				}
				else
				{
					resourceType = -1;
					bool flag4 = isMultiplyItem;
					if (flag4)
					{
						isItem = true;
						isMakeMaterial = false;
					}
					else
					{
						PresetInventoryItem presetInventoryItem = this._shopEventData.ItemList.First<PresetInventoryItem>();
						isItem = true;
						bool flag5;
						if (presetInventoryItem.Type == 5)
						{
							List<short> craftableItemTypes = Config.Material.Instance[presetInventoryItem.TemplateId].CraftableItemTypes;
							flag5 = (craftableItemTypes != null && craftableItemTypes.Count > 0);
						}
						else
						{
							flag5 = false;
						}
						isMakeMaterial = flag5;
						bool flag6 = isMakeMaterial;
						if (flag6)
						{
							resourceType = ItemTemplateHelper.GetResourceType(presetInventoryItem.Type, presetInventoryItem.TemplateId);
						}
					}
				}
				this.RefreshShopEventStorageToggleGroup(outputStorageToggleGroup, resourceType, isItem, isMakeMaterial);
			}
			result = true;
		}
		return result;
	}

	// Token: 0x0600194D RID: 6477 RVA: 0x000A534C File Offset: 0x000A354C
	private bool UpdateShopRecruitInfo(Dictionary<BuildingBlockKey, BuildingEarningsData> earning, int slotCount)
	{
		TextMeshProUGUI recruitPeopleText = this._shopInfoPage.CGet<TextMeshProUGUI>("RecruitPeopleText");
		GameObject recruitPeopleButton = this._shopInfoPage.CGet<GameObject>("RecruitPeopleButton");
		GameObject recruitPeopleButtonHolder = this._shopInfoPage.CGet<GameObject>("RecruitPeopleButtonHolder");
		CButtonObsolete quickRecruitPeopleBtn = this._shopInfoPage.CGet<CButtonObsolete>("QuickRecruitPeopleBtn");
		CButtonObsolete quickRecruitRejectPeopleBtn = this._shopInfoPage.CGet<CButtonObsolete>("QuickRecruitRejectPeopleBtn");
		quickRecruitPeopleBtn.interactable = false;
		quickRecruitRejectPeopleBtn.interactable = false;
		TooltipInvoker quickBtnTip = quickRecruitPeopleBtn.GetComponent<TooltipInvoker>();
		this.ReuseGameObjectFunc(recruitPeopleButton, slotCount, recruitPeopleButtonHolder.transform);
		recruitPeopleButton.SetActive(false);
		UI_BuildingManage.<>c__DisplayClass317_0 CS$<>8__locals1 = new UI_BuildingManage.<>c__DisplayClass317_0();
		CS$<>8__locals1.spriteSize = recruitPeopleButton.GetComponent<CImage>().sprite.rect.size;
		CS$<>8__locals1.rt = recruitPeopleButtonHolder.GetComponent<RectTransform>();
		CS$<>8__locals1.rrt = this._shopInfoPage.GetComponent<RectTransform>();
		CS$<>8__locals1.layout = recruitPeopleButtonHolder.GetComponent<GridLayoutGroup>();
		CS$<>8__locals1.layout.cellSize = CS$<>8__locals1.spriteSize;
		LayoutRebuilder.ForceRebuildLayoutImmediate(CS$<>8__locals1.rt);
		CS$<>8__locals1.layout.CalculateLayoutInputHorizontal();
		CS$<>8__locals1.layout.CalculateLayoutInputVertical();
		UI_RecruitPeopleOverview.RefitContentSize(CS$<>8__locals1.rrt, CS$<>8__locals1.rt);
		SingletonObject.getInstance<YieldHelper>().StartCoroutine(CS$<>8__locals1.<UpdateShopRecruitInfo>g__Routine|0());
		bool flag = earning.ContainsKey(this._blockKey);
		bool result;
		if (flag)
		{
			BuildingEarningsData data;
			earning.TryGetValue(this._blockKey, out data);
			bool flag2 = data != null;
			if (flag2)
			{
				this._stringBuilder.Clear();
				this._stringBuilder.Append(LocalStringManager.Get("LK_Building_ShopRecruitPeople")).Append("  (").Append(data.RecruitLevelList.Count).Append("/").Append(GameData.Domains.Building.SharedMethods.GetBuildingSlotCount(this._configData.TemplateId)).Append(")");
				recruitPeopleText.text = this._stringBuilder.ToString();
				bool flag3 = this._configData.TemplateId == 223;
				if (flag3)
				{
					quickRecruitRejectPeopleBtn.interactable = true;
					quickRecruitPeopleBtn.interactable = false;
					bool interactable = quickRecruitPeopleBtn.interactable;
					if (interactable)
					{
						quickBtnTip.enabled = false;
					}
					else
					{
						quickBtnTip.enabled = true;
						quickBtnTip.PresetParam[0] = LocalStringManager.Get("LK_Building_LockOfResource");
					}
				}
				else
				{
					quickBtnTip.PresetParam[0] = LocalStringManager.Get("LK_Building_AutoRecruit");
					quickRecruitPeopleBtn.interactable = (data.RecruitLevelList.Count > 0);
					quickRecruitRejectPeopleBtn.interactable = (data.RecruitLevelList.Count > 0);
				}
			}
			result = true;
		}
		else
		{
			this._stringBuilder.Clear();
			this._stringBuilder.Append(LocalStringManager.Get("LK_Building_ShopRecruitPeople")).Append("  (0").Append("/").Append(GameData.Domains.Building.SharedMethods.GetBuildingSlotCount(this._configData.TemplateId)).Append(")");
			recruitPeopleText.text = this._stringBuilder.ToString();
			result = false;
		}
		return result;
	}

	// Token: 0x0600194E RID: 6478 RVA: 0x000A5660 File Offset: 0x000A3860
	private bool UpdateShopSoldItemInfo(Dictionary<BuildingBlockKey, BuildingEarningsData> earning, int slotCount)
	{
		GameObject soldItem = this._shopInfoPage.CGet<GameObject>("SoldItem");
		ShopEventItem shopEventData = this._shopEventData;
		bool flag = shopEventData == null || shopEventData.ExchangeResourceGoods == -1;
		bool result;
		if (flag)
		{
			soldItem.gameObject.SetActive(false);
			result = false;
		}
		else
		{
			int count = 0;
			bool flag2 = earning.ContainsKey(this._blockKey);
			if (flag2)
			{
				BuildingEarningsData data = earning[this._blockKey];
				bool flag3 = data == null;
				if (flag3)
				{
					return false;
				}
				for (int i = 0; i < data.ShopSoldItemList.Count; i++)
				{
					bool flag4 = data.ShopSoldItemList[i].TemplateId != -1;
					if (flag4)
					{
						count++;
					}
					bool flag5 = data.ShopSoldItemEarnList[i].First != -1;
					if (flag5)
					{
						count++;
					}
				}
			}
			TextMeshProUGUI soldItemText = this._shopInfoPage.CGet<TextMeshProUGUI>("SoldItemText");
			this._stringBuilder.Clear();
			this._stringBuilder.Append(LocalStringManager.Get("LK_Building_ShopSoldItem")).Append("  (").Append(count).Append("/").Append(GameData.Domains.Building.SharedMethods.GetBuildingSlotCount(this._configData.TemplateId)).Append(")");
			soldItemText.SetText(this._stringBuilder.ToString(), true);
			GameObject soldItemButton = this._shopInfoPage.CGet<GameObject>("SoldItemButton");
			GameObject soldItemButtonHolder = this._shopInfoPage.CGet<GameObject>("SoldItemButtonHolder");
			this.ReuseGameObjectFunc(soldItemButton, slotCount, soldItemButtonHolder.transform);
			soldItem.gameObject.SetActive(true);
			bool showTog = this._shopEventData.ExchangeResourceGoods == 6;
			CToggleGroupObsolete soldStorageToggleGroup = this._shopInfoPage.CGet<CToggleGroupObsolete>("SoldStorageToggleGroup");
			soldStorageToggleGroup.gameObject.SetActive(showTog);
			bool flag6 = showTog;
			if (flag6)
			{
				soldStorageToggleGroup.InitPreOnToggle(-1);
				soldStorageToggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnSoldStorageToggleChange);
				this.RefreshShopEventStorageToggleGroup(soldStorageToggleGroup, 6, false, false);
			}
			result = true;
		}
		return result;
	}

	// Token: 0x0600194F RID: 6479 RVA: 0x000A5888 File Offset: 0x000A3A88
	private void UpdateShopEventBookInfo()
	{
		TextMeshProUGUI eventBookText = this._shopInfoPage.CGet<TextMeshProUGUI>("EventBookText");
		this._stringBuilder.Clear();
		this._stringBuilder.Append(this._configData.Name).Append((this._mainToggleGroup.GetActive().Key == 1) ? LocalStringManager.Get("LK_Building_ShopLearnEventBook") : LocalStringManager.Get("LK_Building_ShopEventBook"));
		eventBookText.text = this._stringBuilder.ToString();
		LoopVerticalScrollRect scrollRect = this._shopInfoPage.CGet<LoopVerticalScrollRect>("EventLoopScrollView");
		GameObject lineTemplate = this._shopInfoPage.CGet<Refers>("EventBookLineTemplate").gameObject;
		scrollRect.InitLoop(lineTemplate, this._currentEventBookDatas.Count, new Action<Transform, int>(this.RenderEventBookLine), null);
	}

	// Token: 0x06001950 RID: 6480 RVA: 0x000A5954 File Offset: 0x000A3B54
	private void RenderEventBookLine(Transform item, int index)
	{
		bool flag = !this._currentEventBookDatas.CheckIndex(index);
		if (!flag)
		{
			UI_BuildingManage.EventBookMonthlyData monthlyData = this._currentEventBookDatas[index];
			Refers itemRefers = item.GetComponent<Refers>();
			TimeManager timeManager = SingletonObject.getInstance<TimeManager>();
			itemRefers.CGet<TextMeshProUGUI>("EventGroupTitle").text = LocalStringManager.GetFormat(LanguageKey.LK_Game_Time, new object[]
			{
				timeManager.GetYearByDate(monthlyData.Date),
				(int)(timeManager.GetMonthInYear(monthlyData.Date) + 1),
				LocalStringManager.Get(string.Format("LK_Season_{0}", TimeKit.GetSeason(monthlyData.Date))),
				Month.Instance[timeManager.GetMonthInYear(monthlyData.Date)].Name
			});
			RectTransform eventLayout = itemRefers.CGet<RectTransform>("EventLayout");
			GameObject eventLineTemplate = itemRefers.CGet<GameObject>("EventLine");
			CommonUtils.PrepareEnoughChildren(eventLayout, eventLineTemplate, monthlyData.EventInfos.Count, null);
			for (int i = 0; i < monthlyData.EventInfos.Count; i++)
			{
				Refers eventLineRefers = eventLayout.GetChild(i).GetComponent<Refers>();
				ShopEventRenderInfo renderInfo = monthlyData.EventInfos[i];
				ShopEventItem shopEventCfg = ShopEvent.Instance[renderInfo.RecordType];
				TextMeshProUGUI descTmp = eventLineRefers.CGet<TextMeshProUGUI>("EventDesc");
				descTmp.text = GameMessageUtils.ParseRenderInfoToText(shopEventCfg.Desc, renderInfo, this._managerRenderArgumentCollection);
			}
			RectTransform rectTransform = item.GetComponent<RectTransform>();
			LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
			item.GetComponent<LayoutElement>().preferredHeight = rectTransform.rect.height;
		}
	}

	// Token: 0x06001951 RID: 6481 RVA: 0x000A5B04 File Offset: 0x000A3D04
	private bool ShowEventBook()
	{
		return (this._configData.IsShop || this.IsEntertain) && this._blockData.OperationType == -1;
	}

	// Token: 0x06001952 RID: 6482 RVA: 0x000A5B3C File Offset: 0x000A3D3C
	private IEnumerator DisableShopQuickSelectButtonForAWhile()
	{
		this._shopQuickSelectBtn.interactable = false;
		yield return new WaitForSeconds(0.5f);
		this.RefreshAllQuickSelectButtons();
		yield break;
	}

	// Token: 0x06001953 RID: 6483 RVA: 0x000A5B4C File Offset: 0x000A3D4C
	private void RefreshAllQuickSelectButtons()
	{
		bool flag = this._shopInfoPage.gameObject.activeSelf && this._configData.IsShop;
		if (flag)
		{
			BuildingDomainMethod.AsyncCall.CanQuickArrangeShopManager(this, this._blockKey, delegate(int offset, RawDataPool pool)
			{
				bool canQuickArrangeShopManager = false;
				Serializer.Deserialize(pool, offset, ref canQuickArrangeShopManager);
				this.RefreshQuickSelectButton(this._shopQuickSelectBtn, canQuickArrangeShopManager);
			});
		}
		this.RefreshQuickSelectButton(this._removeCollectInfoPage.CGet<CButtonObsolete>("ExpandQuickSelectBtn"), this.HasAvailableWorkerForExpandRemove());
	}

	// Token: 0x06001954 RID: 6484 RVA: 0x000A5BB6 File Offset: 0x000A3DB6
	private IEnumerator DisableShopQuickClearButtonForAWhile()
	{
		this._shopQuickClearBtn.interactable = false;
		yield return new WaitForSeconds(0.5f);
		this.RefreshAllQuickClearButtons();
		yield break;
	}

	// Token: 0x06001955 RID: 6485 RVA: 0x000A5BC5 File Offset: 0x000A3DC5
	private void RefreshAllQuickClearButtons()
	{
		this._shopQuickClearBtn.interactable = true;
	}

	// Token: 0x06001956 RID: 6486 RVA: 0x000A5BD8 File Offset: 0x000A3DD8
	private void RefreshQuickSelectButton(CButtonObsolete button, bool interactable)
	{
		button.interactable = interactable;
		TooltipInvoker tipDisplayer = button.GetComponent<TooltipInvoker>();
		TooltipInvoker tooltipInvoker = tipDisplayer;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
		}
		tipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(interactable ? LanguageKey.LK_Building_QuickArrangeTip : LanguageKey.LK_Building_QuickArrangeTip_Disable));
		tipDisplayer.Refresh(false, -1);
	}

	// Token: 0x06001957 RID: 6487 RVA: 0x000A5C3C File Offset: 0x000A3E3C
	private bool IsAvailableWorker(int id)
	{
		bool flag = this._operatorListCached.Contains(id);
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			Dictionary<int, VillagerWorkData> villagerWork = this._buildingModel.VillagerWork;
			bool flag2 = villagerWork != null && villagerWork.ContainsKey(id);
			if (flag2)
			{
				result = false;
			}
			else
			{
				CharacterDisplayData displayData;
				bool flag3 = this._charDisplayDataDict.TryGetValue(id, out displayData) && displayData.CompletelyInfected;
				result = !flag3;
			}
		}
		return result;
	}

	// Token: 0x06001958 RID: 6488 RVA: 0x000A5CA8 File Offset: 0x000A3EA8
	private bool HasAvailableWorkerForExpandRemove()
	{
		int count = this._availableWorker.Count((int id) => !this._shopManagerListCached.Contains(id) && this.IsAvailableWorker(id));
		count += this._unlockWorkingList.Count;
		return count > 0;
	}

	// Token: 0x170002AC RID: 684
	// (get) Token: 0x06001959 RID: 6489 RVA: 0x000A5CE4 File Offset: 0x000A3EE4
	private Refers ArrangementSettingPanel
	{
		get
		{
			return base.CGet<Refers>("ArrangementSettingPanel");
		}
	}

	// Token: 0x170002AD RID: 685
	// (get) Token: 0x0600195A RID: 6490 RVA: 0x000A5CF1 File Offset: 0x000A3EF1
	private BuildingOptionAutoGiveMemberPreset CurrArrangementSettingPresetData
	{
		get
		{
			return this._buildingModel.GetBuildingArrangementSetting(this._blockKey, this._blockData);
		}
	}

	// Token: 0x170002AE RID: 686
	// (get) Token: 0x0600195B RID: 6491 RVA: 0x000A5D0A File Offset: 0x000A3F0A
	private bool CurrInfluenceLeader
	{
		get
		{
			return this.CurrArrangementSettingPresetData.GetIsInfluenceLeader();
		}
	}

	// Token: 0x170002AF RID: 687
	// (get) Token: 0x0600195C RID: 6492 RVA: 0x000A5D17 File Offset: 0x000A3F17
	private bool CurrInfluenceMember
	{
		get
		{
			return this.CurrArrangementSettingPresetData.GetIsInfluenceMember();
		}
	}

	// Token: 0x0600195D RID: 6493 RVA: 0x000A5D24 File Offset: 0x000A3F24
	private void UpdateArrangementSettingBtn()
	{
		this.ArrangementSettingPanel.gameObject.SetActive(false);
		CButtonObsolete arrangementSettingBtn = this._shopInfoPage.CGet<CButtonObsolete>("ArrangementSettingBtn");
		arrangementSettingBtn.gameObject.SetActive(true);
		arrangementSettingBtn.ClearAndAddListener(delegate
		{
			this.ArrangeFocusStart();
			this._arrangementSetting = this.CurrArrangementSettingPresetData;
			this.RefreshPresetTogGroup();
		});
		this.ArrangementSettingPanel.CGet<CButtonObsolete>("ArrangementSettingPanelBtn").ClearAndAddListener(new Action(this.ArrangeFocusFinish));
	}

	// Token: 0x0600195E RID: 6494 RVA: 0x000A5D97 File Offset: 0x000A3F97
	private void ArrangeFocusStart()
	{
		this.ArrangementSettingPanel.gameObject.SetActive(true);
		GEvent.OnEvent(UiEvents.BuildingManageArrangeFocusStart, null);
	}

	// Token: 0x0600195F RID: 6495 RVA: 0x000A5DBC File Offset: 0x000A3FBC
	private void ArrangeFocusFinish()
	{
		this.ArrangementSettingPanel.gameObject.SetActive(false);
		GEvent.OnEvent(UiEvents.BuildingManageArrangeFocusFinish, null);
		bool flag = this._arrangementSettingIndex >= 0;
		if (flag)
		{
			CToggleObsolete curSettingTog = this.ArrangementSettingPanel.CGet<CToggleObsolete>("CurSettingTog");
			curSettingTog.isOn = true;
		}
		this._buildingModel.SetBuildingArrangementSetting(this._blockKey, this._arrangementSetting);
		this.RefreshAllQuickSelectButtons();
	}

	// Token: 0x06001960 RID: 6496 RVA: 0x000A5E34 File Offset: 0x000A4034
	private void RefreshPresetTogGroup()
	{
		CButtonObsolete buttonSync = this.ArrangementSettingPanel.CGet<CButtonObsolete>("ButtonSync");
		buttonSync.ClearAndAddListener(delegate
		{
			this._buildingModel.SetBuildingArrangementSetting(this._blockKey, this._arrangementSetting);
		});
		this.InitArrangementSettings();
		this.InitPresetToggleGroup();
	}

	// Token: 0x06001961 RID: 6497 RVA: 0x000A5E74 File Offset: 0x000A4074
	private void InitPresetToggleGroup()
	{
		CToggleObsolete curSettingTog = this.ArrangementSettingPanel.CGet<CToggleObsolete>("CurSettingTog");
		CToggleGroupObsolete presetTogGroup = this.ArrangementSettingPanel.CGet<CToggleGroupObsolete>("PresetTogGroup");
		curSettingTog.onValueChanged.RemoveAllListeners();
		curSettingTog.onValueChanged.AddListener(delegate(bool isOn)
		{
			if (isOn)
			{
				bool flag = presetTogGroup.GetActive();
				if (flag)
				{
					presetTogGroup.Set(presetTogGroup.GetActive(), false);
				}
				this._arrangementSettingIndex = -1;
				this._arrangementSetting = this.CurrArrangementSettingPresetData;
				this.RefreshArrangementSettings();
			}
			else
			{
				this._buildingModel.SetBuildingArrangementSetting(this._blockKey, this._arrangementSetting);
			}
		});
		presetTogGroup.InitPreOnToggle(-1);
		presetTogGroup.OnActiveToggleChange = delegate(CToggleObsolete newToggle, CToggleObsolete oldToggle)
		{
			bool flag = oldToggle;
			if (flag)
			{
				this._buildingModel.SetBuildingArrangementSettingPresetData(oldToggle.Key, this._arrangementSetting);
			}
			bool flag2 = newToggle;
			if (flag2)
			{
				bool flag3 = !oldToggle;
				if (flag3)
				{
					curSettingTog.isOn = false;
				}
				this._arrangementSettingIndex = newToggle.Key;
				this._arrangementSetting = this._buildingModel.GetBuildingArrangementSettingPresetData(this._arrangementSettingIndex);
				this.RefreshArrangementSettings();
			}
		};
		foreach (CToggleObsolete tog in presetTogGroup.GetAll())
		{
			tog.gameObject.SetActive(tog.Key < 3);
		}
		bool isOn2 = curSettingTog.isOn;
		if (isOn2)
		{
			curSettingTog.onValueChanged.Invoke(true);
		}
		else
		{
			curSettingTog.isOn = true;
		}
	}

	// Token: 0x06001962 RID: 6498 RVA: 0x000A5F98 File Offset: 0x000A4198
	private void InitArrangementSettings()
	{
		this.InitArrangementSettings(this.ArrangementSettingPanel.CGet<Refers>("LeaderLayout"), false);
		this.InitArrangementSettings(this.ArrangementSettingPanel.CGet<Refers>("MemberLayout"), true);
	}

	// Token: 0x06001963 RID: 6499 RVA: 0x000A5FCC File Offset: 0x000A41CC
	private void InitArrangementSettings(Refers refers, bool isMember)
	{
		CToggleGroupObsolete enableToggleGroup = refers.CGet<CToggleGroupObsolete>("EnableToggleGroup");
		enableToggleGroup.InitPreOnToggle(-1);
		enableToggleGroup.OnActiveToggleChange = delegate(CToggleObsolete newTog, CToggleObsolete oldTog)
		{
			bool flag2 = !newTog;
			if (!flag2)
			{
				bool enable = newTog.Key == 0;
				bool isMember3 = isMember;
				if (isMember3)
				{
					this._arrangementSetting.SetIsInfluenceMember(enable);
				}
				else
				{
					this._arrangementSetting.SetIsInfluenceLeader(enable);
				}
				this.RefreshArrangementSettings();
			}
		};
		CToggleGroupObsolete pickRuleToggleGroup = refers.CGet<CToggleGroupObsolete>("PickRuleToggleGroup");
		pickRuleToggleGroup.InitPreOnToggle(-1);
		pickRuleToggleGroup.OnActiveToggleChange = delegate(CToggleObsolete newTog, CToggleObsolete oldTog)
		{
			bool flag2 = !newTog;
			if (!flag2)
			{
				sbyte rule = (sbyte)newTog.Key;
				bool isMember3 = isMember;
				if (isMember3)
				{
					this._arrangementSetting.PickRuleForMember = rule;
				}
				else
				{
					this._arrangementSetting.PickRuleForLeader = rule;
				}
			}
		};
		foreach (CToggleObsolete tog in pickRuleToggleGroup.GetAll())
		{
			BuildingOptionAutoGiveMemberPreset.PickRule key = (BuildingOptionAutoGiveMemberPreset.PickRule)tog.Key;
			if (!true)
			{
			}
			LanguageKey languageKey;
			switch (key)
			{
			case BuildingOptionAutoGiveMemberPreset.PickRule.ManageFirst:
				languageKey = LanguageKey.LK_Building_Arrangement_PickRule_Manage_Tip;
				break;
			case BuildingOptionAutoGiveMemberPreset.PickRule.QualificationFirst:
				languageKey = (isMember ? LanguageKey.LK_Building_Arrangement_PickRule_Qualification_Member_Tip : LanguageKey.LK_Building_Arrangement_PickRule_Qualification_Leader_Tip);
				break;
			case BuildingOptionAutoGiveMemberPreset.PickRule.ReadingFirst:
				languageKey = (isMember ? LanguageKey.LK_Building_Arrangement_PickRule_Read_Member_Tip : LanguageKey.LK_Building_Arrangement_PickRule_Read_Leader_Tip);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!true)
			{
			}
			LanguageKey tipKey = languageKey;
			TooltipInvoker tip = tog.GetComponent<TooltipInvoker>();
			string[] presetParam = tip.PresetParam;
			bool flag = presetParam == null || presetParam.Length != 1;
			if (flag)
			{
				tip.PresetParam = new string[1];
			}
			tip.PresetParam[0] = LocalStringManager.Get(tipKey).ColorReplace();
		}
		CToggleGroupObsolete roleRuleToggleGroup = refers.CGet<CToggleGroupObsolete>("RoleRuleToggleGroup");
		roleRuleToggleGroup.InitPreOnToggle(-1);
		roleRuleToggleGroup.OnActiveToggleChange = delegate(CToggleObsolete newTog, CToggleObsolete oldTog)
		{
			int newKey = (newTog != null) ? newTog.Key : -1;
			int oldKey = (oldTog != null) ? oldTog.Key : -1;
			int addRule = (newKey >= 0) ? (1 << newKey) : 0;
			int removeRule = (oldKey >= 0) ? (1 << oldKey) : 0;
			bool isMember3 = isMember;
			if (isMember3)
			{
				BuildingOptionAutoGiveMemberPreset arrangementSetting = this._arrangementSetting;
				arrangementSetting.RoleRuleForMember |= (sbyte)addRule;
				BuildingOptionAutoGiveMemberPreset arrangementSetting2 = this._arrangementSetting;
				arrangementSetting2.RoleRuleForMember ^= (sbyte)removeRule;
			}
			else
			{
				BuildingOptionAutoGiveMemberPreset arrangementSetting3 = this._arrangementSetting;
				arrangementSetting3.RoleRuleForLeader |= (sbyte)addRule;
				BuildingOptionAutoGiveMemberPreset arrangementSetting4 = this._arrangementSetting;
				arrangementSetting4.RoleRuleForLeader ^= (sbyte)removeRule;
			}
		};
		CToggleObsolete lockCharToggle = refers.CGet<CToggleObsolete>("LockCharToggle");
		lockCharToggle.onValueChanged.RemoveAllListeners();
		lockCharToggle.onValueChanged.AddListener(delegate(bool isOn)
		{
			bool isMember3 = isMember;
			if (isMember3)
			{
				this._arrangementSetting.LockCharForMember = isOn;
			}
			else
			{
				this._arrangementSetting.LockCharForLeader = isOn;
			}
		});
		bool isMember2 = isMember;
		if (isMember2)
		{
			CSliderLegacy memberCountSlider = refers.CGet<CSliderLegacy>("MemberCountSlider");
			TMP_InputField input = refers.CGet<TMP_InputField>("MemberCountInputField");
			memberCountSlider.maxValue = 6f;
			memberCountSlider.minValue = 1f;
			memberCountSlider.onValueChanged.RemoveAllListeners();
			memberCountSlider.onValueChanged.AddListener(delegate(float value)
			{
				int amount = (int)value;
				input.text = amount.ToString();
				this._arrangementSetting.Amount = amount;
			});
			input.onEndEdit.RemoveAllListeners();
			input.onEndEdit.AddListener(delegate(string value)
			{
				int amount;
				int.TryParse(value, out amount);
				amount = Math.Clamp(amount, (int)memberCountSlider.minValue, (int)memberCountSlider.maxValue);
				this._arrangementSetting.Amount = amount;
				memberCountSlider.value = (float)amount;
			});
		}
	}

	// Token: 0x06001964 RID: 6500 RVA: 0x000A625C File Offset: 0x000A445C
	private void RefreshArrangementSettings()
	{
		this.RefreshArrangementSetting(this.ArrangementSettingPanel.CGet<Refers>("LeaderLayout"), false);
		this.RefreshArrangementSetting(this.ArrangementSettingPanel.CGet<Refers>("MemberLayout"), true);
		CButtonObsolete buttonSync = this.ArrangementSettingPanel.CGet<CButtonObsolete>("ButtonSync");
		buttonSync.interactable = (this._arrangementSettingIndex >= 0);
	}

	// Token: 0x06001965 RID: 6501 RVA: 0x000A62C0 File Offset: 0x000A44C0
	private void RefreshArrangementSetting(Refers refers, bool isMember)
	{
		bool interactable = isMember ? this._arrangementSetting.GetIsInfluenceMember() : this._arrangementSetting.GetIsInfluenceLeader();
		CToggleGroupObsolete enableToggleGroup = refers.CGet<CToggleGroupObsolete>("EnableToggleGroup");
		enableToggleGroup.Set(interactable ? 0 : 1, true, false);
		sbyte pickRule = isMember ? this._arrangementSetting.PickRuleForMember : this._arrangementSetting.PickRuleForLeader;
		CToggleGroupObsolete pickRuleToggleGroup = refers.CGet<CToggleGroupObsolete>("PickRuleToggleGroup");
		foreach (CToggleObsolete tog in pickRuleToggleGroup.GetAll())
		{
			tog.interactable = interactable;
			tog.GetComponent<DisableStyleRoot>().SetStyleEffect(!tog.interactable, false);
		}
		pickRuleToggleGroup.Set((int)pickRule, true, false);
		sbyte roleRule = isMember ? this._arrangementSetting.RoleRuleForMember : this._arrangementSetting.RoleRuleForLeader;
		CToggleGroupObsolete roleRuleToggleGroup = refers.CGet<CToggleGroupObsolete>("RoleRuleToggleGroup");
		foreach (CToggleObsolete tog2 in roleRuleToggleGroup.GetAll())
		{
			int togRule = 1 << tog2.Key;
			tog2.isOn = (((int)roleRule & togRule) != 0);
			tog2.interactable = interactable;
			tog2.GetComponent<DisableStyleRoot>().SetStyleEffect(!tog2.interactable, false);
		}
		bool lockChar = isMember ? this._arrangementSetting.LockCharForMember : this._arrangementSetting.LockCharForLeader;
		CToggleObsolete lockCharToggle = refers.CGet<CToggleObsolete>("LockCharToggle");
		lockCharToggle.isOn = lockChar;
		lockCharToggle.interactable = interactable;
		lockCharToggle.GetComponent<DisableStyleRoot>().SetStyleEffect(!lockCharToggle.interactable, false);
		if (isMember)
		{
			CSliderLegacy memberCountSlider = refers.CGet<CSliderLegacy>("MemberCountSlider");
			memberCountSlider.value = (float)this._arrangementSetting.Amount;
			memberCountSlider.interactable = interactable;
			memberCountSlider.GetComponent<DisableStyleRoot>().SetStyleEffect(!interactable, false);
			TMP_InputField memberCountInputField = refers.CGet<TMP_InputField>("MemberCountInputField");
			memberCountInputField.text = memberCountSlider.value.ToString();
			memberCountInputField.interactable = interactable;
			memberCountInputField.GetComponent<DisableStyleRoot>().SetStyleEffect(!interactable, false);
		}
	}

	// Token: 0x06001966 RID: 6502 RVA: 0x000A6524 File Offset: 0x000A4724
	private void BuildingManageArrangeFocusFinish(ArgumentBox argbox)
	{
		this.ArrangementSettingPanel.gameObject.SetActive(false);
	}

	// Token: 0x170002B0 RID: 688
	// (get) Token: 0x06001967 RID: 6503 RVA: 0x000A6539 File Offset: 0x000A4739
	private Refers TaiwuShrineLog
	{
		get
		{
			return base.CGet<Refers>("InfoPage").CGet<Refers>("TaiwuSamsaraLog");
		}
	}

	// Token: 0x06001968 RID: 6504 RVA: 0x000A6550 File Offset: 0x000A4750
	private void SamsaraPlatformRecordDataChange(ArgumentBox _)
	{
		BuildingDomainMethod.Call.GetSamsaraPlatformRecord(this.Element.GameDataListenerId);
	}

	// Token: 0x06001969 RID: 6505 RVA: 0x000A6564 File Offset: 0x000A4764
	private void ShowTaiwuSamsaraLog()
	{
		this.TaiwuShrineLog.CGet<Refers>("DetailPanel").gameObject.SetActive(true);
		this.TaiwuShrineLog.CGet<IdSwitch>("IDSwitchController").gameObject.SetActive(false);
		HorizontalPageSwitchController controller = this.TaiwuShrineLog.CGet<HorizontalPageSwitchController>("PageSwitchController");
		controller.PageItemRefreshHandler = new Action<int, Refers>(this.RefreshTopYearItemOne);
		controller.SetItemSelectStateHandler = new Action<Refers, bool>(this.SetTopYearItemSelectState);
		controller.InitPageCount(0, 0, false);
		controller.RegisterOnSelectIndexChangeHandler(new Action<int>(this.OnSelectYearIndexChange));
		this.TaiwuShrineLog.CGet<IdSwitch>("IDSwitchController").OnValueChanged = new Action<int>(this.OnDetailViewPageSwitch);
		Refers detailRefers = this.TaiwuShrineLog.CGet<Refers>("DetailPanel");
		detailRefers.CGet<CanvasGroup>("PagePrefab").gameObject.SetActive(false);
		detailRefers.CGet<Refers>("DateLinePrefab").gameObject.SetActive(false);
		detailRefers.CGet<Refers>("ContentLine").gameObject.SetActive(false);
		detailRefers.CGet<RectTransform>("SplitLinePrefab").gameObject.SetActive(false);
		this._detailPageHeight = detailRefers.CGet<CanvasGroup>("PagePrefab").GetComponent<RectTransform>().rect.height;
		this._pointerRectTrans = this.TaiwuShrineLog.CGet<PositionFollower>("Follower").GetComponent<RectTransform>();
		this._charNameFullBtnPoolItem = new PoolItem(string.Empty, this.TaiwuShrineLog.CGet<GameObject>("FullCharNameButton"));
		this._charNameLeftPartBtnPoolItem = new PoolItem(string.Empty, this.TaiwuShrineLog.CGet<GameObject>("LeftPartCharNameButton"));
		this._charNameRightPartBtnPoolItem = new PoolItem(string.Empty, this.TaiwuShrineLog.CGet<GameObject>("RightPartCharNameButton"));
		this._focusYear = SingletonObject.getInstance<TimeManager>().GetYear();
	}

	// Token: 0x0600196A RID: 6506 RVA: 0x000A6738 File Offset: 0x000A4938
	private void LateUpdate()
	{
		bool activeSelf = this.TaiwuShrineLog.gameObject.activeSelf;
		if (activeSelf)
		{
			this.TaiwuShrineLog.CGet<LayoutElement>("ControlPart").preferredWidth = this._pointerRectTrans.anchoredPosition.x - 27f;
		}
	}

	// Token: 0x0600196B RID: 6507 RVA: 0x000A6788 File Offset: 0x000A4988
	private void ReInitTopPageList()
	{
		int yearCurrent = SingletonObject.getInstance<TimeManager>().GetYear();
		this._topYearDataList.Clear();
		bool isFiveYearMode = true;
		int unit = isFiveYearMode ? 5 : 1;
		int num;
		if (this._yearRecords.Count <= 0)
		{
			num = yearCurrent;
		}
		else
		{
			num = this._yearRecords.Min((UI_BuildingManage.YearRecord y) => y.Year);
		}
		int minYear = num;
		int yearStart = Math.Max(1, minYear / unit * unit);
		int yearRangeA = yearStart;
		int initIndex = -1;
		for (int i = yearStart; i <= yearCurrent; i++)
		{
			bool flag = i == this._focusYear;
			if (flag)
			{
				initIndex = this._topYearDataList.Count;
			}
			bool flag2 = isFiveYearMode;
			if (flag2)
			{
				bool flag3 = (i % unit == 0 && i != yearStart) || i == yearCurrent;
				if (flag3)
				{
					Vector2Int vec = new Vector2Int(yearRangeA, i);
					this._topYearDataList.Add(vec);
					yearRangeA = i + 1;
				}
			}
			else
			{
				Vector2Int vec2 = new Vector2Int(i, i);
				this._topYearDataList.Add(vec2);
			}
		}
		Action<int, Refers> refreshHandler = new Action<int, Refers>(this.RefreshTopYearItemOne);
		bool flag4 = isFiveYearMode;
		if (flag4)
		{
			refreshHandler = new Action<int, Refers>(this.RefreshTopYearItemFive);
		}
		HorizontalPageSwitchController controller = this.TaiwuShrineLog.CGet<HorizontalPageSwitchController>("PageSwitchController");
		controller.PageItemRefreshHandler = refreshHandler;
		controller.SetItemSelectStateHandler = new Action<Refers, bool>(this.SetTopYearItemSelectState);
		controller.InitPageCount(this._topYearDataList.Count, initIndex, false);
	}

	// Token: 0x0600196C RID: 6508 RVA: 0x000A6910 File Offset: 0x000A4B10
	private void RefreshTopYearItemOne(int index, Refers refers)
	{
		TextMeshProUGUI textComponent = refers.CGet<TextMeshProUGUI>("LabelOff");
		textComponent.text = LocalStringManager.GetFormat(LanguageKey.UI_AdvanceMonth_TimeChangeInfo_Year, this._topYearDataList[index].x);
		refers.CGet<TextMeshProUGUI>("LabelOn").text = textComponent.text;
		refers.CGet<CToggleObsolete>("Toggle").onValueChanged.RemoveAllListeners();
		refers.CGet<CToggleObsolete>("Toggle").onValueChanged.AddListener(delegate(bool isOn)
		{
			if (isOn)
			{
				this._focusYear = this._topYearDataList[index].x;
				this.TaiwuShrineLog.CGet<HorizontalPageSwitchController>("PageSwitchController").SetSelect(index, true);
			}
		});
	}

	// Token: 0x0600196D RID: 6509 RVA: 0x000A69BC File Offset: 0x000A4BBC
	private void RefreshTopYearItemFive(int index, Refers refers)
	{
		Vector2Int vec = this._topYearDataList[index];
		int yearBegin = vec.x;
		int yearEnd = vec.y;
		string yearRange = (yearBegin == yearEnd) ? yearBegin.ToString() : string.Format("{0}-{1}", yearBegin, yearEnd);
		string labelString = LocalStringManager.GetFormat(LanguageKey.UI_AdvanceMonth_TimeChangeInfo_Year, yearRange);
		TextMeshProUGUI textComponent = refers.CGet<TextMeshProUGUI>("LabelOff");
		textComponent.text = labelString;
		refers.CGet<TextMeshProUGUI>("LabelOn").text = labelString;
		refers.CGet<CToggleObsolete>("Toggle").onValueChanged.RemoveAllListeners();
		refers.CGet<CToggleObsolete>("Toggle").onValueChanged.AddListener(delegate(bool isOn)
		{
			if (isOn)
			{
				this._focusYear = yearBegin;
				this.TaiwuShrineLog.CGet<HorizontalPageSwitchController>("PageSwitchController").SetSelect(index, true);
			}
		});
	}

	// Token: 0x0600196E RID: 6510 RVA: 0x000A6AA6 File Offset: 0x000A4CA6
	private void SetTopYearItemSelectState(Refers refers, bool selectState)
	{
		refers.CGet<CToggleObsolete>("Toggle").isOn = selectState;
		refers.CGet<CToggleObsolete>("Toggle").interactable = !selectState;
	}

	// Token: 0x0600196F RID: 6511 RVA: 0x000A6AD0 File Offset: 0x000A4CD0
	private void OnSelectYearIndexChange(int index)
	{
		Refers taiwuShrineLog = base.CGet<Refers>("InfoPage").CGet<Refers>("TaiwuSamsaraLog");
		Refers itemRefers = taiwuShrineLog.CGet<HorizontalPageSwitchController>("PageSwitchController").GetPageItem(index);
		bool flag = null != itemRefers;
		if (flag)
		{
			taiwuShrineLog.CGet<PositionFollower>("Follower").Target = itemRefers.transform;
		}
		int startYear = this._topYearDataList[index].x;
		int endYear = this._topYearDataList[index].y;
		this._selectedYearRecords.Clear();
		this._selectedYearRecords.AddRange(from y in this._yearRecords
		where y.Year >= startYear && y.Year <= endYear
		select y);
		this.RefreshAsDetailView(this._selectedYearRecords);
	}

	// Token: 0x06001970 RID: 6512 RVA: 0x000A6BA0 File Offset: 0x000A4DA0
	private void OnDetailViewPageSwitch(int value)
	{
		for (int i = 0; i < this._detailViewPageList.Count; i++)
		{
			CanvasGroup canvasGroup = this._detailViewPageList[i];
			canvasGroup.DOKill(false);
			bool flag = i + 1 == value;
			if (flag)
			{
				canvasGroup.DOFade(1f, 0.3f).SetAutoKill(true);
				canvasGroup.blocksRaycasts = true;
			}
			else
			{
				canvasGroup.DOFade(0f, 0.3f).SetAutoKill(true);
				canvasGroup.blocksRaycasts = false;
			}
		}
	}

	// Token: 0x06001971 RID: 6513 RVA: 0x000A6C30 File Offset: 0x000A4E30
	private void OnSamsaraCollectionUpdated()
	{
		this._samsaraPlatformRecordRenderInfos.Clear();
		this._samsaraLogArgumentCollection.Clear();
		this._samsaraLogRenderedArgumentCollection.Clear();
		bool flag = this._samsaraPlatformRecordCollection == null || this._samsaraPlatformRecordCollection.Count == 0;
		if (flag)
		{
			this._yearRecords.Clear();
			this.ReInitTopPageList();
		}
		else
		{
			this._samsaraPlatformRecordCollection.GetRenderInfos(this._samsaraPlatformRecordRenderInfos, this._samsaraLogArgumentCollection);
			string key = "UI_SamsaraPlatformRecords";
			this._charIdList.Clear();
			this._charIdList.AddRange(this._samsaraLogArgumentCollection.Characters);
			this._charIdList.AddRange(this._samsaraLogArgumentCollection.CharacterRealNames);
			RecordArgumentsRequest argRequest = new RecordArgumentsRequest(this._samsaraLogArgumentCollection)
			{
				Characters = this._charIdList
			};
			LifeRecordDomainMethod.AsyncCall.GetRecordRenderInfoArguments(this, key, argRequest, delegate(int offset, RawDataPool dataPool)
			{
				ArgumentCollectionRenderArguments dynamicArguments = null;
				Serializer.Deserialize(dataPool, offset, ref dynamicArguments);
				GameMessageUtils.RenderDynamicArguments(dynamicArguments, this._samsaraLogArgumentCollection, this._samsaraLogRenderedArgumentCollection, true, true);
				GameMessageUtils.RenderFixedArguments(this._samsaraLogArgumentCollection, this._samsaraLogRenderedArgumentCollection, false);
				TimeManager timeManager = SingletonObject.getInstance<TimeManager>();
				this._yearRecords.Clear();
				foreach (SamsaraPlatformRecordRenderInfo renderInfo in this._samsaraPlatformRecordRenderInfos)
				{
					int year = timeManager.GetYearByDate(renderInfo.Date);
					int yearIndex = this._yearRecords.FindIndex((UI_BuildingManage.YearRecord y) => y.Year == year);
					bool flag2 = yearIndex >= 0;
					UI_BuildingManage.YearRecord yearRecord;
					if (flag2)
					{
						yearRecord = this._yearRecords[yearIndex];
					}
					else
					{
						yearRecord = new UI_BuildingManage.YearRecord();
						yearRecord.Year = year;
						this._yearRecords.Add(yearRecord);
					}
					sbyte month = timeManager.GetMonthInYear(renderInfo.Date);
					int monthIndex = yearRecord.MonthRecords.FindIndex((UI_BuildingManage.MonthRecord y) => y.Month == (int)month);
					bool flag3 = monthIndex >= 0;
					UI_BuildingManage.MonthRecord monthRecord;
					if (flag3)
					{
						monthRecord = yearRecord.MonthRecords[monthIndex];
					}
					else
					{
						monthRecord = new UI_BuildingManage.MonthRecord();
						monthRecord.Month = (int)month;
						monthRecord.Date = renderInfo.Date;
						yearRecord.MonthRecords.Add(monthRecord);
					}
					string desc = renderInfo.GetText(this._samsaraLogRenderedArgumentCollection).ColorReplace();
					monthRecord.Records.Add(desc);
				}
				this.ReInitTopPageList();
			});
		}
	}

	// Token: 0x06001972 RID: 6514 RVA: 0x000A6D24 File Offset: 0x000A4F24
	private void RefreshAsDetailView(List<UI_BuildingManage.YearRecord> yearRecords)
	{
		UI_BuildingManage.<>c__DisplayClass378_0 CS$<>8__locals1 = new UI_BuildingManage.<>c__DisplayClass378_0();
		CS$<>8__locals1.<>4__this = this;
		Refers taiwuShrineLog = base.CGet<Refers>("InfoPage").CGet<Refers>("TaiwuSamsaraLog");
		CS$<>8__locals1.detailRefers = taiwuShrineLog.CGet<Refers>("DetailPanel");
		CanvasGroup canvasGroup = CS$<>8__locals1.detailRefers.CGet<CanvasGroup>("DetailPanel");
		canvasGroup.alpha = 0f;
		CS$<>8__locals1.detailRefers.gameObject.SetActive(true);
		CS$<>8__locals1.pageRoot = CS$<>8__locals1.detailRefers.CGet<RectTransform>("ContentRoot");
		bool flag = this._detailViewPageList == null;
		if (flag)
		{
			this._detailViewPageList = new List<CanvasGroup>();
		}
		CS$<>8__locals1.dateLineCache = new List<Refers>();
		CS$<>8__locals1.contentLineCache = new List<Refers>();
		CS$<>8__locals1.splitLineCache = new List<CImage>();
		CS$<>8__locals1.pageCache = new List<CanvasGroup>(CS$<>8__locals1.pageRoot.GetComponentsInTopChildren(true));
		CS$<>8__locals1.pageCache.ForEach(delegate(CanvasGroup e)
		{
			CS$<>8__locals1.<>4__this.CollectPage(e, CS$<>8__locals1.dateLineCache, CS$<>8__locals1.contentLineCache, CS$<>8__locals1.splitLineCache);
		});
		this._detailViewPageList.Clear();
		CS$<>8__locals1.curPage = CS$<>8__locals1.<RefreshAsDetailView>g__GetPage|1();
		this._detailViewPageList.Add(CS$<>8__locals1.curPage);
		CS$<>8__locals1.pageContentHeight = 0f;
		List<RectTransform> lineCache = new List<RectTransform>();
		bool hasRecord = false;
		for (int i = 0; i < yearRecords.Count; i++)
		{
			List<UI_BuildingManage.MonthRecord> monthDataList = yearRecords[i].MonthRecords;
			bool flag2 = monthDataList == null || monthDataList.Count <= 0;
			if (!flag2)
			{
				for (int j = 0; j < monthDataList.Count; j++)
				{
					List<string> recordList = monthDataList[j].Records;
					bool flag3 = recordList == null || recordList.Count <= 0;
					if (!flag3)
					{
						bool flag4 = i + j > 0;
						if (flag4)
						{
							RectTransform splitLine = CS$<>8__locals1.<RefreshAsDetailView>g__GetSplitLine|4(CS$<>8__locals1.curPage.transform);
							lineCache.Add(splitLine);
						}
						int date = monthDataList[j].Date;
						Refers dateLine = CS$<>8__locals1.<RefreshAsDetailView>g__GetDateLine|2(CS$<>8__locals1.curPage.transform);
						MonthItem monthConfig = Month.Instance.GetItem(SingletonObject.getInstance<TimeManager>().GetMonthInYear(date));
						string dateDesc = SingletonObject.getInstance<TimeManager>().GetDateDisplayContent(date);
						dateLine.CGet<TextMeshProUGUI>("Date").text = dateDesc + " " + monthConfig.Name;
						lineCache.Add(dateLine.GetComponent<RectTransform>());
						for (int k = 0; k < recordList.Count; k++)
						{
							string content = recordList[k];
							Refers contentLine = CS$<>8__locals1.<RefreshAsDetailView>g__GetContentLine|3(CS$<>8__locals1.curPage.transform);
							bool flag5 = contentLine.UserObject == null;
							CharacterNameClickLinkHandler handler;
							if (flag5)
							{
								RectTransform btnRoot = contentLine.CGet<RectTransform>("ButtonRoot");
								handler = new CharacterNameClickLinkHandler(btnRoot, this._charNameFullBtnPoolItem, this._charNameLeftPartBtnPoolItem, this._charNameRightPartBtnPoolItem, new Action<int>(this.OnCharacterNameClicked));
								contentLine.UserObject = handler;
							}
							else
							{
								handler = (contentLine.UserObject as CharacterNameClickLinkHandler);
							}
							TMPTextSpriteHelper spriteHelper = contentLine.CGet<TMPTextSpriteHelper>("SpriteHelper");
							TextMeshProUGUI label = contentLine.CGet<TextMeshProUGUI>("Content");
							label.text = content;
							if (handler != null)
							{
								handler.ProcessLinkInfo(label, true);
							}
							spriteHelper.Parse();
							RectTransform lineRect = contentLine.GetComponent<RectTransform>();
							lineRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, label.GetPreferredValues().y + 7f);
							lineCache.Add(lineRect);
							CS$<>8__locals1.<RefreshAsDetailView>g__AddLines|5(lineCache);
							hasRecord = true;
						}
					}
				}
			}
		}
		CS$<>8__locals1.dateLineCache.ForEach(delegate(Refers e)
		{
			e.gameObject.SetActive(false);
		});
		CS$<>8__locals1.contentLineCache.ForEach(delegate(Refers e)
		{
			e.gameObject.SetActive(false);
		});
		CS$<>8__locals1.splitLineCache.ForEach(delegate(CImage e)
		{
			e.gameObject.SetActive(false);
		});
		CS$<>8__locals1.pageCache.ForEach(delegate(CanvasGroup e)
		{
			e.gameObject.SetActive(false);
		});
		taiwuShrineLog.CGet<IdSwitch>("IDSwitchController").Init(this._detailViewPageList.Count, this._detailViewPageList.Count, 1);
		this.OnDetailViewPageSwitch(this._detailViewPageList.Count);
		taiwuShrineLog.CGet<IdSwitch>("IDSwitchController").gameObject.SetActive(this._detailViewPageList.Count > 1);
		CS$<>8__locals1.detailRefers.CGet<GameObject>("NoContent").SetActive(!hasRecord);
		canvasGroup.DOFade(1f, 0.5f);
	}

	// Token: 0x06001973 RID: 6515 RVA: 0x000A71F8 File Offset: 0x000A53F8
	private void CollectPage(CanvasGroup page, List<Refers> dateLineList, List<Refers> contentLineList, List<CImage> splitLineList)
	{
		CImage[] splitLines = page.transform.GetComponentsInTopChildren(true);
		Refers[] refers = page.transform.GetComponentsInTopChildren(true);
		splitLineList.AddRange(splitLines);
		dateLineList.AddRange(refers.FindAll((Refers e) => e.UserInt == 0));
		contentLineList.AddRange(refers.FindAll((Refers e) => e.UserInt == 1));
	}

	// Token: 0x06001974 RID: 6516 RVA: 0x000A7284 File Offset: 0x000A5484
	private void OnCharacterNameClicked(int charId)
	{
		ArgumentBox args = new ArgumentBox();
		args.SetObject("TargetPageIndex", ECharacterSubToggleBase.StoryBase);
		GEvent.OnEvent(UiEvents.OnNeedOpenCharacterMenuSubPage, args);
		this.HandleCharacterDisplayData(charId);
	}

	// Token: 0x06001975 RID: 6517 RVA: 0x000A72C3 File Offset: 0x000A54C3
	private void HandleCharacterDisplayData(int charId)
	{
		List<int> list = new List<int>();
		list.Add(charId);
		CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataListForRelations(this, list, delegate(int offset, RawDataPool dataPool)
		{
			List<CharacterDisplayDataForRelations> charData = new List<CharacterDisplayDataForRelations>();
			Serializer.Deserialize(dataPool, offset, ref charData);
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharacterId", charData[0].CharacterId);
			argBox.SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.RelationshipBase, ECharacterSubPage.None));
			UIElement.CharacterMenu.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		});
	}

	// Token: 0x06001976 RID: 6518 RVA: 0x000A72FC File Offset: 0x000A54FC
	private void ActionWhenQuickAssignRole(int villagerId)
	{
		short roleKey = this._configData.VillagerRoleTemplateIds[0];
		VillagerRoleUtils.ConfirmAndAssignRole(villagerId, roleKey, null, null, this);
	}

	// Token: 0x06001977 RID: 6519 RVA: 0x000A7324 File Offset: 0x000A5524
	private void HandleVillagerDisplayData(List<VillagerRoleCharacterDisplayData> villagerRoleCharacterDisplayDataList)
	{
		this.ResetVillagerList();
		for (int i = 0; i < villagerRoleCharacterDisplayDataList.Count; i++)
		{
			VillagerRoleCharacterDisplayData data = villagerRoleCharacterDisplayDataList[i];
			bool flag = (data.Flags & 1) > 0;
			if (flag)
			{
				this._currentVillagerList[(int)data.RoleTemplateId].Add(data);
			}
			else
			{
				bool flag2 = (data.Flags & 2) > 0;
				if (flag2)
				{
					this._lostVillagerList[(int)data.RoleTemplateId].Add(data);
				}
			}
		}
	}

	// Token: 0x06001978 RID: 6520 RVA: 0x000A73A6 File Offset: 0x000A55A6
	private void ResetVillagerList()
	{
		UI_BuildingManage.ResetVillagerList(this._currentVillagerList);
		UI_BuildingManage.ResetVillagerList(this._lostVillagerList);
	}

	// Token: 0x06001979 RID: 6521 RVA: 0x000A73C4 File Offset: 0x000A55C4
	private static void ResetVillagerList(List<VillagerRoleCharacterDisplayData>[] villagerList)
	{
		for (int i = 0; i < villagerList.Length; i++)
		{
			List<VillagerRoleCharacterDisplayData> t = villagerList[i];
			bool flag = t == null;
			if (flag)
			{
				t = new List<VillagerRoleCharacterDisplayData>();
				villagerList[i] = t;
			}
			else
			{
				t.Clear();
			}
		}
	}

	// Token: 0x0600197A RID: 6522 RVA: 0x000A7409 File Offset: 0x000A5609
	public void SetInitialTab(int tabKey)
	{
		this._initialTabKey = tabKey;
	}

	// Token: 0x0600197B RID: 6523 RVA: 0x000A7414 File Offset: 0x000A5614
	private void ApplyInitialTab()
	{
		bool flag = this._initialTabKey >= 0;
		if (flag)
		{
			CToggleObsolete toggle = this._mainToggleGroup.Get(this._initialTabKey);
			bool flag2 = toggle != null && toggle.gameObject.activeSelf;
			if (flag2)
			{
				this._mainToggleGroup.Set(this._initialTabKey, true, false);
			}
		}
	}

	// Token: 0x0600197C RID: 6524 RVA: 0x000A7478 File Offset: 0x000A5678
	private void UpdateShopTitle()
	{
		this._stringBuilder.Clear();
		this._stringBuilder.Append(this._configData.LeaderName).Append("(").Append(this.GetLeaderCount()).Append("/").Append(1).Append(")");
		this._shopInfoPage.CGet<TextMeshProUGUI>("ShopLeaderTitle").text = this._stringBuilder.ToString();
		this._stringBuilder.Clear();
		this._stringBuilder.Append(this._configData.MemberName).Append("(").Append(this.GetMemberCount()).Append("/").Append(6).Append(")");
		this._shopInfoPage.CGet<TextMeshProUGUI>("ShopMemberTitle").text = this._stringBuilder.ToString();
		this.UpdateShopManageTitle();
	}

	// Token: 0x0600197D RID: 6525 RVA: 0x000A7574 File Offset: 0x000A5774
	private void UpdateShopManageTitle()
	{
		Refers refers = this._shopInfoPage.CGet<Refers>("ManageTitleRefers");
		ValueTuple<string, string> buildingRequireIconAndName = UI_BuildingManage.GetBuildingRequireIconAndName(this._configData);
		string iconName = buildingRequireIconAndName.Item1;
		string requireName = buildingRequireIconAndName.Item2;
		CImage icon = refers.CGet<CImage>("Icon");
		TextMeshProUGUI value = refers.CGet<TextMeshProUGUI>("Value");
		icon.SetSprite(iconName, false, null);
		value.text = requireName;
	}

	// Token: 0x170002B1 RID: 689
	// (get) Token: 0x0600197E RID: 6526 RVA: 0x000A75D7 File Offset: 0x000A57D7
	private Refers SoldItemSettingPanel
	{
		get
		{
			return base.CGet<Refers>("SoldItemSettingPanel");
		}
	}

	// Token: 0x170002B2 RID: 690
	// (get) Token: 0x0600197F RID: 6527 RVA: 0x000A75E4 File Offset: 0x000A57E4
	private CToggleGroupObsolete ItemTypeToggleGroup
	{
		get
		{
			return this.SoldItemSettingPanel.CGet<CToggleGroupObsolete>("ItemTypeToggleGroup");
		}
	}

	// Token: 0x170002B3 RID: 691
	// (get) Token: 0x06001980 RID: 6528 RVA: 0x000A75F6 File Offset: 0x000A57F6
	private BuildingOptionAutoAddSoldItemPreset CurSoldItemPreset
	{
		get
		{
			return this._buildingModel.GetBuildingSoldItemSetting(this._blockKey, this._blockData);
		}
	}

	// Token: 0x06001981 RID: 6529 RVA: 0x000A760F File Offset: 0x000A580F
	private void OnClickQuickAddSoldItemBtn()
	{
		BuildingDomainMethod.Call.QuickAddShopSoldItem(this._blockKey);
	}

	// Token: 0x06001982 RID: 6530 RVA: 0x000A761E File Offset: 0x000A581E
	private void OnClickQuickRemoveSoldItemBtn()
	{
		BuildingDomainMethod.Call.QuickRemoveShopSoldItem(this._blockKey);
	}

	// Token: 0x06001983 RID: 6531 RVA: 0x000A762D File Offset: 0x000A582D
	private void OnClickSoldItemSettingBtn()
	{
		this.SoldItemSettingPanel.gameObject.SetActive(true);
	}

	// Token: 0x06001984 RID: 6532 RVA: 0x000A7644 File Offset: 0x000A5844
	private void InitSoldItemSetting()
	{
		short itemSubType;
		this._soldItemSettingTypeList = GameData.Domains.Building.SharedMethods.GetBuildingCanSoldItemTypeList(this._configData, out itemSubType);
		CToggleGroupObsolete itemTypeToggleGroup = this.ItemTypeToggleGroup;
		itemTypeToggleGroup.Clear();
		itemTypeToggleGroup.OnActiveToggleChange = null;
		sbyte i = 0;
		while ((int)i < this._soldItemSettingTypeList.Count)
		{
			sbyte itemType = this._soldItemSettingTypeList[(int)i];
			Transform child = ((int)i < itemTypeToggleGroup.transform.childCount) ? itemTypeToggleGroup.transform.GetChild((int)i) : Object.Instantiate<Transform>(itemTypeToggleGroup.transform.GetChild(0), itemTypeToggleGroup.transform);
			child.gameObject.SetActive(true);
			child.GetComponentInChildren<TextMeshProUGUI>(true).text = ((itemSubType == -1) ? LocalStringManager.Get(string.Format("LK_ItemType_{0}", itemType)) : LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", itemSubType)));
			CToggleObsolete tog = child.GetComponent<CToggleObsolete>();
			List<sbyte> itemTypeList = this.CurSoldItemPreset.ItemTypeList;
			bool flag = itemTypeList == null || itemTypeList.Count <= 0 || this.CurSoldItemPreset.ItemTypeList.Contains(itemType);
			if (flag)
			{
				tog.isOn = true;
			}
			else
			{
				tog.isOn = false;
			}
			i += 1;
		}
		for (int j = this._soldItemSettingTypeList.Count; j < itemTypeToggleGroup.transform.childCount; j++)
		{
			itemTypeToggleGroup.transform.GetChild(j).gameObject.SetActive(false);
		}
		itemTypeToggleGroup.AddAllChildToggles();
		itemTypeToggleGroup.InitPreOnToggle(-1);
		itemTypeToggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnSoldItemSettingItemTypeToggleChange);
		this.RefreshItemTypeToggleTip();
		CButtonObsolete maskButton = this.SoldItemSettingPanel.CGet<CButtonObsolete>("MaskButton");
		maskButton.ClearAndAddListener(new Action(this.HideSoldItemSetting));
		this._soldItemSettingGradeMinDropdown = this.SoldItemSettingPanel.CGet<CDropdownLegacy>("GradeMinDropdown");
		this._soldItemSettingGradeMinDropdown.onValueChanged.RemoveAllListeners();
		this._soldItemSettingGradeMinDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnSoldItemSettingGradeMinDropdownValueChanged));
		this._soldItemSettingGradeMaxDropdown = this.SoldItemSettingPanel.CGet<CDropdownLegacy>("GradeMaxDropdown");
		this._soldItemSettingGradeMaxDropdown.onValueChanged.RemoveAllListeners();
		this._soldItemSettingGradeMaxDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnSoldItemSettingGradeMaxDropdownValueChanged));
		this._soldItemSettingDropdownMask = this.SoldItemSettingPanel.CGet<GameObject>("DropdownMask");
		this.RefreshSoldItemSettingDropdown();
		CToggleGroupObsolete gradeOrderToggleGroup = this.SoldItemSettingPanel.CGet<CToggleGroupObsolete>("GradeOrderToggleGroup");
		gradeOrderToggleGroup.InitPreOnToggle(-1);
		gradeOrderToggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnGradeOrderToggleChange);
		int gradeOrderTogKey = (int)(this.CurSoldItemPreset.GradeOrder - 1);
		gradeOrderToggleGroup.Set(gradeOrderTogKey, true, false);
		CToggleGroupObsolete propertyOrderToggleGroup = this.SoldItemSettingPanel.CGet<CToggleGroupObsolete>("PropertyOrderToggleGroup");
		propertyOrderToggleGroup.InitPreOnToggle(-1);
		propertyOrderToggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnPropertyOrderToggleChange);
		BuildingOptionAutoAddSoldItemPreset.EPropertyOrder curPropertyOrder = (BuildingOptionAutoAddSoldItemPreset.EPropertyOrder)this.CurSoldItemPreset.PropertyOrder;
		bool isMaxValue = curPropertyOrder.HasFlag(BuildingOptionAutoAddSoldItemPreset.EPropertyOrder.MaxValue);
		propertyOrderToggleGroup.Set((int)(BuildingOptionAutoAddSoldItemPreset.EPropertyOrder.MaxValue.ToSbyte() - 1), isMaxValue, false);
		bool isMaxAmount = curPropertyOrder.HasFlag(BuildingOptionAutoAddSoldItemPreset.EPropertyOrder.MaxAmount);
		propertyOrderToggleGroup.Set((int)(BuildingOptionAutoAddSoldItemPreset.EPropertyOrder.MaxAmount.ToSbyte() - 1), isMaxAmount, false);
	}

	// Token: 0x06001985 RID: 6533 RVA: 0x000A7998 File Offset: 0x000A5B98
	private void RefreshItemTypeToggleTip()
	{
		StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
		CToggleGroupObsolete itemTypeToggleGroup = this.ItemTypeToggleGroup;
		List<CToggleObsolete> toggles = itemTypeToggleGroup.GetAll();
		for (int index = 0; index < toggles.Count; index++)
		{
			CToggleObsolete tog = toggles[index];
			bool flag = !tog.gameObject.activeSelf;
			if (!flag)
			{
				sbyte itemType = this._soldItemSettingTypeList[index];
				string typeName = LocalStringManager.Get(string.Format("LK_ItemType_{0}", itemType));
				TooltipInvoker tip = tog.GetComponent<TooltipInvoker>();
				TooltipInvoker tooltipInvoker = tip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				string content = LanguageKey.LK_Building_SoldSetting_Item_Type_Tip.TrFormat(typeName);
				stringBuilder.Clear();
				stringBuilder.AppendLine(content);
				bool flag2 = tog.isOn && itemTypeToggleGroup.GetAllActive().Count == 1;
				if (flag2)
				{
					stringBuilder.AppendLine(LanguageKey.LK_Building_SoldSetting_Item_Type_LimitTip.Tr());
				}
				tip.RuntimeParam.Set("arg0", stringBuilder.ToString());
			}
		}
		stringBuilder.Clear();
		EasyPool.Free<StringBuilder>(stringBuilder);
	}

	// Token: 0x06001986 RID: 6534 RVA: 0x000A7AC0 File Offset: 0x000A5CC0
	private void OnGradeOrderToggleChange(CToggleObsolete newTog, CToggleObsolete olgTog)
	{
		BuildingOptionAutoAddSoldItemPreset.EGradeOrder gradeOder = (BuildingOptionAutoAddSoldItemPreset.EGradeOrder)(newTog.Key + 1);
		bool flag = gradeOder.ToSbyte() != this.CurSoldItemPreset.GradeOrder;
		if (flag)
		{
			this.CurSoldItemPreset.GradeOrder = gradeOder.ToSbyte();
			this.SaveSoldItemSetting();
		}
	}

	// Token: 0x06001987 RID: 6535 RVA: 0x000A7B18 File Offset: 0x000A5D18
	private void OnPropertyOrderToggleChange(CToggleObsolete newTog, CToggleObsolete olgTog)
	{
		sbyte result = 0;
		bool flag = newTog;
		if (flag)
		{
			BuildingOptionAutoAddSoldItemPreset.EPropertyOrder newOrder = (BuildingOptionAutoAddSoldItemPreset.EPropertyOrder)(newTog.Key + 1);
			BuildingOptionAutoAddSoldItemPreset.EPropertyOrder curOrder = (BuildingOptionAutoAddSoldItemPreset.EPropertyOrder)this.CurSoldItemPreset.PropertyOrder;
			result = (curOrder | newOrder).ToSbyte();
		}
		bool flag2 = olgTog;
		if (flag2)
		{
			BuildingOptionAutoAddSoldItemPreset.EPropertyOrder oldOrder = (BuildingOptionAutoAddSoldItemPreset.EPropertyOrder)(olgTog.Key + 1);
			BuildingOptionAutoAddSoldItemPreset.EPropertyOrder curOrder2 = (BuildingOptionAutoAddSoldItemPreset.EPropertyOrder)this.CurSoldItemPreset.PropertyOrder;
			result = (curOrder2 ^ oldOrder).ToSbyte();
		}
		bool flag3 = result != this.CurSoldItemPreset.PropertyOrder;
		if (flag3)
		{
			this.CurSoldItemPreset.PropertyOrder = result;
			this.SaveSoldItemSetting();
		}
	}

	// Token: 0x06001988 RID: 6536 RVA: 0x000A7BBC File Offset: 0x000A5DBC
	private void RefreshSoldItemSettingDropdown()
	{
		this._soldItemSettingGradeMinOptionList.Clear();
		this._soldItemSettingGradeMaxOptionList.Clear();
		sbyte minGrade = this.CurSoldItemPreset.MinGrade;
		sbyte maxGrade = this.CurSoldItemPreset.MaxGrade;
		for (sbyte i = 0; i <= 8; i += 1)
		{
			bool flag = i <= maxGrade;
			if (flag)
			{
				this._soldItemSettingGradeMinOptionList.Add(i);
			}
			bool flag2 = i >= minGrade;
			if (flag2)
			{
				this._soldItemSettingGradeMaxOptionList.Add(i);
			}
		}
		this._soldItemSettingGradeMinDropdown.ClearOptions();
		List<string> gradeMinOptionList = (from g in this._soldItemSettingGradeMinOptionList
		select CommonUtils.GetShortGradeText((int)g, true)).ToList<string>();
		this._soldItemSettingGradeMinDropdown.AddOptions(gradeMinOptionList);
		int minIndex = this._soldItemSettingGradeMinOptionList.IndexOf(minGrade);
		this._soldItemSettingGradeMinDropdown.value = minIndex;
		this.RefreshSoldItemSettingGradeDropdownSelection(this._soldItemSettingGradeMinDropdown, minGrade);
		this._soldItemSettingGradeMaxDropdown.ClearOptions();
		List<string> gradeMaxOptionList = (from g in this._soldItemSettingGradeMaxOptionList
		select CommonUtils.GetShortGradeText((int)g, true)).ToList<string>();
		this._soldItemSettingGradeMaxDropdown.AddOptions(gradeMaxOptionList);
		int maxIndex = this._soldItemSettingGradeMaxOptionList.IndexOf(maxGrade);
		this._soldItemSettingGradeMaxDropdown.value = maxIndex;
		this.RefreshSoldItemSettingGradeDropdownSelection(this._soldItemSettingGradeMaxDropdown, maxGrade);
	}

	// Token: 0x06001989 RID: 6537 RVA: 0x000A7D34 File Offset: 0x000A5F34
	private void OnSoldItemSettingGradeMinDropdownValueChanged(int value)
	{
		sbyte grade = this._soldItemSettingGradeMinOptionList[value];
		bool flag = grade != this.CurSoldItemPreset.MinGrade;
		if (flag)
		{
			this.CurSoldItemPreset.MinGrade = grade;
			this.SaveSoldItemSetting();
			this.RefreshSoldItemSettingDropdown();
			this.RefreshSoldItemSettingGradeDropdownSelection(this._soldItemSettingGradeMinDropdown, grade);
		}
	}

	// Token: 0x0600198A RID: 6538 RVA: 0x000A7D90 File Offset: 0x000A5F90
	private void OnSoldItemSettingGradeMaxDropdownValueChanged(int value)
	{
		sbyte grade = this._soldItemSettingGradeMaxOptionList[value];
		bool flag = grade != this.CurSoldItemPreset.MaxGrade;
		if (flag)
		{
			this.CurSoldItemPreset.MaxGrade = grade;
			this.SaveSoldItemSetting();
			this.RefreshSoldItemSettingDropdown();
			this.RefreshSoldItemSettingGradeDropdownSelection(this._soldItemSettingGradeMaxDropdown, grade);
		}
	}

	// Token: 0x0600198B RID: 6539 RVA: 0x000A7DEC File Offset: 0x000A5FEC
	private void RefreshSoldItemSettingGradeDropdownSelection(CDropdownLegacy dropdown, sbyte grade)
	{
		CImage gradeImage = dropdown.transform.Find("Layout/GradeBack").GetComponent<CImage>();
		gradeImage.gameObject.SetActive(true);
		gradeImage.SetSprite(ItemView.GetGradeIcon(grade), false, null);
		TextMeshProUGUI componentInChildren = gradeImage.GetComponentInChildren<TextMeshProUGUI>();
		if (componentInChildren != null)
		{
			componentInChildren.SetText(ItemView.GetGradeText(grade), true);
		}
	}

	// Token: 0x0600198C RID: 6540 RVA: 0x000A7E48 File Offset: 0x000A6048
	private void ShowMakeDropdownMask(bool show)
	{
		bool flag = this._soldItemSettingDropdownMask && this._soldItemSettingDropdownMask.gameObject.activeSelf != show;
		if (flag)
		{
			this._soldItemSettingDropdownMask.gameObject.SetActive(show);
		}
	}

	// Token: 0x0600198D RID: 6541 RVA: 0x000A7E94 File Offset: 0x000A6094
	private void UpdateSoldItemSettingDropdown()
	{
		bool flag = this.UpdateSoldItemSettingDropdown(this._soldItemSettingGradeMinDropdown, this._soldItemSettingGradeMinOptionList);
		if (!flag)
		{
			this.UpdateSoldItemSettingDropdown(this._soldItemSettingGradeMaxDropdown, this._soldItemSettingGradeMaxOptionList);
		}
	}

	// Token: 0x0600198E RID: 6542 RVA: 0x000A7ED0 File Offset: 0x000A60D0
	private bool UpdateSoldItemSettingDropdown(CDropdownLegacy dropdown, List<sbyte> optionList)
	{
		bool flag = !dropdown || !dropdown.IsExpanded;
		bool result;
		if (flag)
		{
			this.ShowMakeDropdownMask(false);
			result = false;
		}
		else
		{
			Transform trans = dropdown.transform.Find("Dropdown List");
			bool flag2 = !trans;
			if (flag2)
			{
				this.ShowMakeDropdownMask(false);
				result = false;
			}
			else
			{
				this.ShowMakeDropdownMask(true);
				CToggleObsolete[] toggles = dropdown.GetComponentsInChildren<CToggleObsolete>();
				PositionFollower positionFollower = dropdown.GetComponentInChildren<PositionFollower>();
				foreach (CToggleObsolete togCell in toggles)
				{
					bool flag3 = !togCell.gameObject.activeSelf;
					if (!flag3)
					{
						togCell.transform.Find("Disable").gameObject.SetActive(togCell.isOn);
						bool flag4 = togCell.isOn && positionFollower;
						if (flag4)
						{
							positionFollower.Target = togCell.transform;
						}
					}
				}
				RectTransform content = trans.GetComponentInChildren<CScrollRectLegacy>().Content;
				int childCount = content.childCount;
				for (int i = 1; i < childCount; i++)
				{
					Transform item = content.GetChild(i);
					CImage gradeBack = item.Find("Layout/GradeBack").GetComponent<CImage>();
					bool flag5 = !gradeBack.gameObject.activeSelf;
					if (flag5)
					{
						gradeBack.gameObject.SetActive(true);
					}
					sbyte grade = optionList[i - 1];
					CImage component = gradeBack.GetComponent<CImage>();
					if (component != null)
					{
						component.SetSprite(ItemView.GetGradeIcon(grade), false, null);
					}
					TextMeshProUGUI componentInChildren = gradeBack.GetComponentInChildren<TextMeshProUGUI>();
					if (componentInChildren != null)
					{
						componentInChildren.SetText(ItemView.GetGradeText(grade), true);
					}
				}
				dropdown.transform.Find("Dropdown List").GetComponent<Canvas>().sortingOrder = 640;
				Transform blocker = base.transform.parent.parent.Find("Blocker");
				bool flag6 = blocker != null;
				if (flag6)
				{
					blocker.GetComponent<Canvas>().sortingOrder = 639;
				}
				result = true;
			}
		}
		return result;
	}

	// Token: 0x0600198F RID: 6543 RVA: 0x000A80EA File Offset: 0x000A62EA
	private void HideSoldItemSetting()
	{
		this.SoldItemSettingPanel.gameObject.SetActive(false);
	}

	// Token: 0x06001990 RID: 6544 RVA: 0x000A8100 File Offset: 0x000A6300
	private void OnSoldItemSettingItemTypeToggleChange(CToggleObsolete newTog, CToggleObsolete oldTog)
	{
		bool flag = newTog;
		if (flag)
		{
			sbyte itemType = this._soldItemSettingTypeList[newTog.Key];
			bool flag2 = !this.CurSoldItemPreset.ItemTypeList.Contains(itemType);
			if (flag2)
			{
				this.CurSoldItemPreset.ItemTypeList.Add(itemType);
			}
			this.RefreshItemTypeToggleTip();
			this.SaveSoldItemSetting();
		}
		else
		{
			bool flag3 = oldTog;
			if (flag3)
			{
				sbyte itemType2 = this._soldItemSettingTypeList[oldTog.Key];
				bool flag4 = this.CurSoldItemPreset.ItemTypeList == null;
				if (flag4)
				{
					this.CurSoldItemPreset.ItemTypeList = new List<sbyte>();
					this.CurSoldItemPreset.ItemTypeList.AddRange(this._soldItemSettingTypeList);
				}
				this.CurSoldItemPreset.ItemTypeList.Remove(itemType2);
				this.RefreshItemTypeToggleTip();
				this.SaveSoldItemSetting();
			}
		}
	}

	// Token: 0x06001991 RID: 6545 RVA: 0x000A81E3 File Offset: 0x000A63E3
	private void SaveSoldItemSetting()
	{
		this._buildingModel.SetBuildingSoldItemSetting(this._blockKey, this.CurSoldItemPreset);
	}

	// Token: 0x06001999 RID: 6553 RVA: 0x000A8693 File Offset: 0x000A6893
	[CompilerGenerated]
	private void <OpenMultiSelectItemWindow>g__FreshAction|141_0()
	{
		this.FreshCanSoldItemList();
	}

	// Token: 0x040013AD RID: 5037
	private readonly string[] StartOperationTipTitleKey = new string[]
	{
		"LK_Building_Start_Build",
		"LK_Building_Start_Remove"
	};

	// Token: 0x040013AE RID: 5038
	private readonly string[] StartOperationTipDescKey = new string[]
	{
		"LK_Building_Start_Build_Tip_Desc",
		"LK_Building_Start_Remove_Tip_Desc"
	};

	// Token: 0x040013AF RID: 5039
	private readonly string[] StopOperationTipTitleKey = new string[]
	{
		"LK_Building_Stop_Build",
		"LK_Building_Stop_Remove"
	};

	// Token: 0x040013B0 RID: 5040
	private readonly string[] ContinueOperationTipTitleKey = new string[]
	{
		"LK_Building_Continue_Build",
		"LK_Building_Continue_Remove"
	};

	// Token: 0x040013B1 RID: 5041
	private readonly string[] ConfirmOperationTipDescKey = new string[]
	{
		"LK_Confirm_Build_Tip_Desc",
		"LK_Confirm_Remove_Tip_Desc"
	};

	// Token: 0x040013B2 RID: 5042
	private readonly string[] StopOperationTipDescKey = new string[]
	{
		"LK_Building_CancelBuild",
		"LK_Building_Stop_Remove_Tip_Desc"
	};

	// Token: 0x040013B3 RID: 5043
	private readonly string[] ContinueOperationTipDescKey = new string[]
	{
		"LK_Building_Continue_Build_Tip_Desc",
		"LK_Building_Continue_Remove_Tip_Desc"
	};

	// Token: 0x040013B4 RID: 5044
	private short _areaId;

	// Token: 0x040013B5 RID: 5045
	private short _blockId;

	// Token: 0x040013B6 RID: 5046
	private short _blockTemplateId;

	// Token: 0x040013B7 RID: 5047
	private short _buildingBlockIndex;

	// Token: 0x040013B8 RID: 5048
	private int _taiwuCharId;

	// Token: 0x040013B9 RID: 5049
	private short _settlementId;

	// Token: 0x040013BA RID: 5050
	private BuildingModel _buildingModel;

	// Token: 0x040013BB RID: 5051
	private BuildingBlockData _blockData;

	// Token: 0x040013BC RID: 5052
	private BuildingBlockItem _configData;

	// Token: 0x040013BD RID: 5053
	private List<BuildingBlockData> _blockList;

	// Token: 0x040013BE RID: 5054
	private bool _displayInited;

	// Token: 0x040013BF RID: 5055
	private readonly int[] _operatorListCached = new int[3];

	// Token: 0x040013C0 RID: 5056
	private readonly int[] _shopManagerListCached = new int[7];

	// Token: 0x040013C1 RID: 5057
	private int _selectingOperatorIndex;

	// Token: 0x040013C2 RID: 5058
	private int _selectingShopManagerIndex;

	// Token: 0x040013C3 RID: 5059
	private bool _resourceEnough;

	// Token: 0x040013C4 RID: 5060
	private bool _isTaiwuVillageBuilding;

	// Token: 0x040013C5 RID: 5061
	private int _resourceBlockRanking;

	// Token: 0x040013C6 RID: 5062
	private Refers _removeCollectInfoPage;

	// Token: 0x040013C7 RID: 5063
	private GameObject _eventBook;

	// Token: 0x040013C8 RID: 5064
	private RectTransform _btnHolder;

	// Token: 0x040013C9 RID: 5065
	private GameObject _residentsHolder;

	// Token: 0x040013CA RID: 5066
	private GameObject _residentViewPrefab;

	// Token: 0x040013CB RID: 5067
	private TextMeshProUGUI _residentCount;

	// Token: 0x040013CC RID: 5068
	private readonly List<ResidentView> _residentViews = new List<ResidentView>();

	// Token: 0x040013CD RID: 5069
	private List<int> _unlockWorkingList = new List<int>();

	// Token: 0x040013CE RID: 5070
	private readonly List<int> _villagerList = new List<int>();

	// Token: 0x040013CF RID: 5071
	private readonly Dictionary<int, CharacterDisplayData> _charDisplayDataDict = new Dictionary<int, CharacterDisplayData>();

	// Token: 0x040013D0 RID: 5072
	private readonly Dictionary<int, short> _propertyValueDict = new Dictionary<int, short>();

	// Token: 0x040013D1 RID: 5073
	private readonly Dictionary<int, int[]> _lifeSkillAttainmentDict = new Dictionary<int, int[]>();

	// Token: 0x040013D2 RID: 5074
	private List<CharacterList> _allResidents = new List<CharacterList>();

	// Token: 0x040013D3 RID: 5075
	private List<int> _residents = new List<int>();

	// Token: 0x040013D4 RID: 5076
	private List<int> _availableWorker = new List<int>();

	// Token: 0x040013D5 RID: 5077
	private List<int> _availableChildren = new List<int>();

	// Token: 0x040013D6 RID: 5078
	private StringBuilder _stringBuilder = new StringBuilder();

	// Token: 0x040013D7 RID: 5079
	private List<GameObject> _btnGroup = new List<GameObject>();

	// Token: 0x040013D8 RID: 5080
	private Dictionary<string, List<GameObject>> _reuseDic = new Dictionary<string, List<GameObject>>();

	// Token: 0x040013D9 RID: 5081
	private int btnIndex = 0;

	// Token: 0x040013DA RID: 5082
	private bool isNeedRepair = false;

	// Token: 0x040013DB RID: 5083
	private BuildingBlockKey _blockKey;

	// Token: 0x040013DC RID: 5084
	private ShopEventItem _shopEventData;

	// Token: 0x040013DD RID: 5085
	private List<ItemDisplayData> _inventoryItemList = new List<ItemDisplayData>();

	// Token: 0x040013DE RID: 5086
	private List<ItemDisplayData> _warehouseItemList = new List<ItemDisplayData>();

	// Token: 0x040013DF RID: 5087
	private List<ItemDisplayData> _treasuryItemList = new List<ItemDisplayData>();

	// Token: 0x040013E0 RID: 5088
	private List<ItemDisplayData> _inventoryCanSoldItemList = new List<ItemDisplayData>();

	// Token: 0x040013E1 RID: 5089
	private List<ItemDisplayData> _warehouseCanSoldItemList = new List<ItemDisplayData>();

	// Token: 0x040013E2 RID: 5090
	private List<ItemDisplayData> _treasuryCanSoldItemList = new List<ItemDisplayData>();

	// Token: 0x040013E3 RID: 5091
	private CButtonObsolete _shopQuickSelectBtn;

	// Token: 0x040013E4 RID: 5092
	private CButtonObsolete _shopQuickClearBtn;

	// Token: 0x040013E5 RID: 5093
	private CButtonObsolete _quickAddSoldItemBtn;

	// Token: 0x040013E6 RID: 5094
	private CButtonObsolete _quickRemoveSoldItemBtn;

	// Token: 0x040013E7 RID: 5095
	private CButtonObsolete _soldItemSettingBtn;

	// Token: 0x040013E8 RID: 5096
	private CToggleObsolete _autoArrangeToggle;

	// Token: 0x040013E9 RID: 5097
	private CToggleObsolete _autoSoldToggle;

	// Token: 0x040013EA RID: 5098
	private CToggleObsolete _autoCheckInToggle;

	// Token: 0x040013EB RID: 5099
	private TooltipInvoker _autoCheckInMouseTip;

	// Token: 0x040013EC RID: 5100
	private sbyte replaceResidentIndex = -1;

	// Token: 0x040013ED RID: 5101
	private List<GameObject> _animationGoList = new List<GameObject>();

	// Token: 0x040013EE RID: 5102
	private BuildingEarningsData _earningsData = null;

	// Token: 0x040013EF RID: 5103
	private bool _canTransfer;

	// Token: 0x040013F0 RID: 5104
	private Location _villageLocation;

	// Token: 0x040013F1 RID: 5105
	private Dictionary<BuildingBlockKey, BuildingEarningsData> _bookCollectionData;

	// Token: 0x040013F2 RID: 5106
	private Vector3 _contentRectTransPos;

	// Token: 0x040013F3 RID: 5107
	private List<int> _neighborDistanceList;

	// Token: 0x040013F4 RID: 5108
	private BuildingAreaData _areaData;

	// Token: 0x040013F5 RID: 5109
	private int _buildingSpaceCurr;

	// Token: 0x040013F6 RID: 5110
	private int _buildingSpaceLimit;

	// Token: 0x040013F7 RID: 5111
	private List<GameData.Domains.Character.LifeSkillItem> _learnedLifeSkillItems = new List<GameData.Domains.Character.LifeSkillItem>();

	// Token: 0x040013F8 RID: 5112
	private List<short> _learnedCombatSkillItems = new List<short>();

	// Token: 0x040013F9 RID: 5113
	private List<short> _lockBuildingList = new List<short>();

	// Token: 0x040013FA RID: 5114
	private Vector2 _residenceLayoutSpacing = new Vector2(55f, 20f);

	// Token: 0x040013FB RID: 5115
	private Vector2 _comfortableHouseLayoutSpacing = new Vector2(80f, 20f);

	// Token: 0x040013FC RID: 5116
	private Vector2 _defaultLayoutSpacing = new Vector2(0f, 20f);

	// Token: 0x040013FD RID: 5117
	private int _currentAttainment;

	// Token: 0x040013FE RID: 5118
	private BuildingFormulaContextBridge _formulaContext;

	// Token: 0x040013FF RID: 5119
	private GameObject _fillBg;

	// Token: 0x04001400 RID: 5120
	private CImage _fillImg;

	// Token: 0x04001401 RID: 5121
	private TextMeshProUGUI _fillText;

	// Token: 0x04001402 RID: 5122
	private Refers _expandInfoItem;

	// Token: 0x04001403 RID: 5123
	private List<Refers> _expandInfoItemList = new List<Refers>();

	// Token: 0x04001404 RID: 5124
	private RectTransform _expandTitleContent;

	// Token: 0x04001405 RID: 5125
	private RectTransform _kuangBg;

	// Token: 0x04001406 RID: 5126
	private const float InitHeight = 86f;

	// Token: 0x04001407 RID: 5127
	private const float ExpandInfoItemHeightOne = 54f;

	// Token: 0x04001408 RID: 5128
	private const float ExpandInfoItemHeightOther = 52f;

	// Token: 0x04001409 RID: 5129
	private ShopEventCollection _shopEventCollection;

	// Token: 0x0400140A RID: 5130
	private ArgumentCollection _managerArgumentCollection = new ArgumentCollection();

	// Token: 0x0400140B RID: 5131
	private RenderedArgumentCollection _managerRenderArgumentCollection = new RenderedArgumentCollection();

	// Token: 0x0400140C RID: 5132
	private List<ShopEventRenderInfo> _shopEventRenderInfos = new List<ShopEventRenderInfo>();

	// Token: 0x0400140D RID: 5133
	private List<UI_BuildingManage.EventBookMonthlyData> _shopManageEventRenderInfos = new List<UI_BuildingManage.EventBookMonthlyData>();

	// Token: 0x0400140E RID: 5134
	private List<UI_BuildingManage.EventBookMonthlyData> _shopLearnEventRenderInfos = new List<UI_BuildingManage.EventBookMonthlyData>();

	// Token: 0x0400140F RID: 5135
	private List<ItemDisplayData> _canUseBuildingCore;

	// Token: 0x04001410 RID: 5136
	private List<ItemDisplayData> _cannotUseInventoryBuildingCore;

	// Token: 0x04001411 RID: 5137
	private Coroutine _sensitiveWordTipCoroutine;

	// Token: 0x04001412 RID: 5138
	private Tween _sensitiveWordTipTween;

	// Token: 0x04001413 RID: 5139
	private static readonly Dictionary<short, string> SoundDict = new Dictionary<short, string>
	{
		{
			44,
			"ui_building_taiwucun"
		},
		{
			129,
			"ui_building_huolianshi"
		},
		{
			139,
			"ui_building_mugongfang"
		},
		{
			179,
			"ui_building_qiaojiangwu"
		},
		{
			203,
			"ui_building_shijiao"
		},
		{
			169,
			"ui_building_xiulou"
		},
		{
			149,
			"ui_building_yaofang"
		},
		{
			159,
			"ui_building_youshi"
		}
	};

	// Token: 0x04001414 RID: 5140
	private Refers _arrangementSettingPanel;

	// Token: 0x04001415 RID: 5141
	private TextMeshProUGUI _autoCheckInToggleLabel;

	// Token: 0x04001416 RID: 5142
	private GameObject _btnTemplate;

	// Token: 0x04001417 RID: 5143
	private Refers _buildingInfoPage;

	// Token: 0x04001418 RID: 5144
	private RectTransform _buttonHolder;

	// Token: 0x04001419 RID: 5145
	private GameObject _buttonHolderLine;

	// Token: 0x0400141A RID: 5146
	private GameObject _buttonLayoutGroup;

	// Token: 0x0400141B RID: 5147
	private CButtonObsolete _cancelBtn;

	// Token: 0x0400141C RID: 5148
	private CButtonObsolete _confirmBtn;

	// Token: 0x0400141D RID: 5149
	private Refers _damageInfoPage;

	// Token: 0x0400141E RID: 5150
	private Refers _entertainPage;

	// Token: 0x0400141F RID: 5151
	private GameObject _expandInfoPage;

	// Token: 0x04001420 RID: 5152
	private GameObject _expandManpowerHolder;

	// Token: 0x04001421 RID: 5153
	private Refers _expandRemoveCollectInfoPage;

	// Token: 0x04001422 RID: 5154
	private CanvasGroup _getItemTips;

	// Token: 0x04001423 RID: 5155
	private TextMeshProUGUI _getItemTipsText;

	// Token: 0x04001424 RID: 5156
	private Refers _infoPage;

	// Token: 0x04001425 RID: 5157
	private Refers _leftInfo;

	// Token: 0x04001426 RID: 5158
	private CToggleGroupObsolete _mainToggleGroup;

	// Token: 0x04001427 RID: 5159
	private Refers _needResourceHolder;

	// Token: 0x04001428 RID: 5160
	private GameObject _noContent;

	// Token: 0x04001429 RID: 5161
	private GameObject _progressBg;

	// Token: 0x0400142A RID: 5162
	private CImage _progressFill;

	// Token: 0x0400142B RID: 5163
	private TextMeshProUGUI _progressText;

	// Token: 0x0400142C RID: 5164
	private CButtonObsolete _repairBuildingBtn;

	// Token: 0x0400142D RID: 5165
	private GameObject _residentInfo;

	// Token: 0x0400142E RID: 5166
	private TextMeshProUGUI _residentViewQuickText;

	// Token: 0x0400142F RID: 5167
	private GameObject _residentsLayoutGroup;

	// Token: 0x04001430 RID: 5168
	private GameObject _rightInfo;

	// Token: 0x04001431 RID: 5169
	private TextMeshProUGUI _rightTitle;

	// Token: 0x04001432 RID: 5170
	private Refers _shopInfoPage;

	// Token: 0x04001433 RID: 5171
	private Refers _soldItemSettingPanel;

	// Token: 0x04001434 RID: 5172
	private GameObject _topInfo;

	// Token: 0x04001435 RID: 5173
	private bool _refersInitialized;

	// Token: 0x04001436 RID: 5174
	private CDropdownLegacy _feastDropdown;

	// Token: 0x04001437 RID: 5175
	private GameObject _feastDropdownMask;

	// Token: 0x04001438 RID: 5176
	private GameData.Domains.Building.Feast _feast;

	// Token: 0x04001439 RID: 5177
	private List<short> _unlockedFeastTypes = new List<short>();

	// Token: 0x0400143A RID: 5178
	private Vector2 ItemResourceInfoSize = new Vector2(262f, 38f);

	// Token: 0x0400143B RID: 5179
	private Vector2 ResourceContentSize = new Vector2(970f, 80f);

	// Token: 0x0400143C RID: 5180
	private float ResourceContentSizeHighAdd = 60f;

	// Token: 0x0400143D RID: 5181
	private List<ValueTuple<int, string>> _residentsLivingPlace = new List<ValueTuple<int, string>>();

	// Token: 0x0400143E RID: 5182
	private CValuePercentBonus _shopProgressBonus;

	// Token: 0x0400143F RID: 5183
	private int _arrangementSettingIndex = -1;

	// Token: 0x04001440 RID: 5184
	private BuildingOptionAutoGiveMemberPreset _arrangementSetting;

	// Token: 0x04001441 RID: 5185
	private List<Vector2Int> _topYearDataList = new List<Vector2Int>();

	// Token: 0x04001442 RID: 5186
	private int _focusYear;

	// Token: 0x04001443 RID: 5187
	private List<CanvasGroup> _detailViewPageList;

	// Token: 0x04001444 RID: 5188
	private float _detailPageHeight;

	// Token: 0x04001445 RID: 5189
	private RectTransform _pointerRectTrans;

	// Token: 0x04001446 RID: 5190
	private PoolItem _charNameFullBtnPoolItem;

	// Token: 0x04001447 RID: 5191
	private PoolItem _charNameLeftPartBtnPoolItem;

	// Token: 0x04001448 RID: 5192
	private PoolItem _charNameRightPartBtnPoolItem;

	// Token: 0x04001449 RID: 5193
	private SamsaraPlatformRecordCollection _samsaraPlatformRecordCollection = new SamsaraPlatformRecordCollection();

	// Token: 0x0400144A RID: 5194
	private ArgumentCollection _samsaraLogArgumentCollection = new ArgumentCollection();

	// Token: 0x0400144B RID: 5195
	private RenderedArgumentCollection _samsaraLogRenderedArgumentCollection = new RenderedArgumentCollection();

	// Token: 0x0400144C RID: 5196
	private List<SamsaraPlatformRecordRenderInfo> _samsaraPlatformRecordRenderInfos = new List<SamsaraPlatformRecordRenderInfo>();

	// Token: 0x0400144D RID: 5197
	private readonly List<UI_BuildingManage.YearRecord> _yearRecords = new List<UI_BuildingManage.YearRecord>(1024);

	// Token: 0x0400144E RID: 5198
	private readonly List<UI_BuildingManage.YearRecord> _selectedYearRecords = new List<UI_BuildingManage.YearRecord>(1024);

	// Token: 0x0400144F RID: 5199
	private readonly List<int> _charIdList = new List<int>();

	// Token: 0x04001450 RID: 5200
	private List<VillagerRoleCharacterDisplayData>[] _currentVillagerList;

	// Token: 0x04001451 RID: 5201
	private List<VillagerRoleCharacterDisplayData>[] _lostVillagerList;

	// Token: 0x04001452 RID: 5202
	private int _initialTabKey = -1;

	// Token: 0x04001453 RID: 5203
	private List<sbyte> _soldItemSettingTypeList;

	// Token: 0x04001454 RID: 5204
	private CDropdownLegacy _soldItemSettingGradeMinDropdown;

	// Token: 0x04001455 RID: 5205
	private CDropdownLegacy _soldItemSettingGradeMaxDropdown;

	// Token: 0x04001456 RID: 5206
	private GameObject _soldItemSettingDropdownMask;

	// Token: 0x04001457 RID: 5207
	private readonly List<sbyte> _soldItemSettingGradeMinOptionList = new List<sbyte>();

	// Token: 0x04001458 RID: 5208
	private readonly List<sbyte> _soldItemSettingGradeMaxOptionList = new List<sbyte>();

	// Token: 0x02001312 RID: 4882
	private enum ShopEventStorageToggleKey
	{
		// Token: 0x04009C8E RID: 40078
		Inventory,
		// Token: 0x04009C8F RID: 40079
		Warehouse,
		// Token: 0x04009C90 RID: 40080
		Treasury,
		// Token: 0x04009C91 RID: 40081
		Stock,
		// Token: 0x04009C92 RID: 40082
		[Obsolete]
		Make,
		// Token: 0x04009C93 RID: 40083
		[Obsolete]
		Craft,
		// Token: 0x04009C94 RID: 40084
		[Obsolete]
		Medicine,
		// Token: 0x04009C95 RID: 40085
		[Obsolete]
		Food
	}

	// Token: 0x02001313 RID: 4883
	private class EventBookMonthlyData
	{
		// Token: 0x0600C7D8 RID: 51160 RVA: 0x00586B66 File Offset: 0x00584D66
		public EventBookMonthlyData(int date)
		{
			this.Date = date;
			this.EventInfos = new List<ShopEventRenderInfo>();
		}

		// Token: 0x04009C96 RID: 40086
		public int Date;

		// Token: 0x04009C97 RID: 40087
		public List<ShopEventRenderInfo> EventInfos;
	}

	// Token: 0x02001314 RID: 4884
	public enum EBuildingNotAvailableType
	{
		// Token: 0x04009C99 RID: 40089
		None,
		// Token: 0x04009C9A RID: 40090
		BuildConditionNotMet,
		// Token: 0x04009C9B RID: 40091
		Locked
	}

	// Token: 0x02001315 RID: 4885
	private class YearRecord
	{
		// Token: 0x04009C9C RID: 40092
		public int Year;

		// Token: 0x04009C9D RID: 40093
		public readonly List<UI_BuildingManage.MonthRecord> MonthRecords = new List<UI_BuildingManage.MonthRecord>();
	}

	// Token: 0x02001316 RID: 4886
	private class MonthRecord
	{
		// Token: 0x04009C9E RID: 40094
		public int Date;

		// Token: 0x04009C9F RID: 40095
		public int Month;

		// Token: 0x04009CA0 RID: 40096
		public readonly List<string> Records = new List<string>();
	}
}
