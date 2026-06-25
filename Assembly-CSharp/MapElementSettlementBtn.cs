using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.CommandSystem;
using FrameWork.UISystem.UIElements;
using Game.Views.Legacy.WorldMap;
using GameData.Domains.Map;
using GameData.Domains.Organization;
using GameData.Domains.Organization.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020003F3 RID: 1011
public class MapElementSettlementBtn : MapElementBase
{
	// Token: 0x06003CCA RID: 15562 RVA: 0x001E96D8 File Offset: 0x001E78D8
	public static bool CheckMaybeExist(Location location)
	{
		bool atPastTaiwuVillage = MapElementBase.MapModel.AtPastTaiwuVillage;
		bool result;
		if (atPastTaiwuVillage)
		{
			result = false;
		}
		else
		{
			bool flag = MapElementBase.MapModel.ViewMode == WorldMapModel.EViewMode.Info && MapElementBase.MapModel.CurrentAreaId == location.AreaId;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = MapElementBase.MapModel.ShowingAreaId != location.AreaId;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = location.AreaId != MapElementBase.MapModel.CurrentAreaId;
					if (flag3)
					{
						result = false;
					}
					else
					{
						MapBlockData blockData = MapElementBase.MapModel.GetBlockData(location);
						bool flag4 = ((blockData != null) ? blockData.GetConfig() : null) == null;
						if (flag4)
						{
							result = false;
						}
						else
						{
							bool flag5 = !blockData.Visible;
							if (flag5)
							{
								result = false;
							}
							else
							{
								bool flag6 = MapElementBase.MapModel.SelectedBlock == null;
								if (flag6)
								{
									result = false;
								}
								else
								{
									bool flag7 = blockData.GetRootBlock() != MapElementBase.MapModel.SelectedBlock.GetRootBlock();
									if (flag7)
									{
										result = false;
									}
									else
									{
										bool unlockBtn = MapElementSettlementBtn.CheckUnlockBtn();
										bool flag8 = !unlockBtn;
										if (flag8)
										{
											result = false;
										}
										else
										{
											SettlementInfo[] settlementInfos = MapElementBase.MapModel.Areas[(int)location.AreaId].SettlementInfos;
											bool flag9 = settlementInfos == null;
											if (flag9)
											{
												result = false;
											}
											else
											{
												for (int i = 0; i < settlementInfos.Length; i++)
												{
													bool flag10 = settlementInfos[i].BlockId == location.BlockId;
													if (flag10)
													{
														return true;
													}
												}
												result = false;
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
		return result;
	}

	// Token: 0x06003CCB RID: 15563 RVA: 0x001E986C File Offset: 0x001E7A6C
	private static bool CheckUnlockBtn()
	{
		TutorialChapterModel tutorialModel = SingletonObject.getInstance<TutorialChapterModel>();
		bool inGuiding = tutorialModel.InGuiding;
		bool result;
		if (inGuiding)
		{
			result = tutorialModel.CanShowEnterIndustry;
		}
		else
		{
			result = (!MapElementBase.MapModel.SecretVillageOnFire && !SingletonObject.getInstance<TaskModel>().IsTaskInProgress(231));
		}
		return result;
	}

	// Token: 0x1700062E RID: 1582
	// (get) Token: 0x06003CCC RID: 15564 RVA: 0x001E98B8 File Offset: 0x001E7AB8
	protected override bool AutoSetActive
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700062F RID: 1583
	// (get) Token: 0x06003CCD RID: 15565 RVA: 0x001E98BB File Offset: 0x001E7ABB
	private bool ShouldActiveInfo
	{
		get
		{
			return this._settlementInfo.OrgTemplateId != 0 && !SingletonObject.getInstance<TutorialChapterModel>().InGuiding && SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(13);
		}
	}

	// Token: 0x17000630 RID: 1584
	// (get) Token: 0x06003CCE RID: 15566 RVA: 0x001E98E5 File Offset: 0x001E7AE5
	public override EMapLayer Layer
	{
		get
		{
			return EMapLayer.SettlementAndStation;
		}
	}

	// Token: 0x06003CCF RID: 15567 RVA: 0x001E98E8 File Offset: 0x001E7AE8
	public override void Scale(float wheel)
	{
		float factor = Mathf.Pow(1f / wheel, 1.5f) * wheel;
		float factor2 = Mathf.Pow(1f / wheel, 2f) * wheel;
		Vector3 reverseScale = Vector3.one * MapElementSettlementBtn.<Scale>g__NewFactor|20_0(factor, 0.5f);
		PointerScaleAnim[] pointerScaleAnims = base.GetComponentsInChildren<PointerScaleAnim>(true);
		foreach (PointerScaleAnim scale in pointerScaleAnims)
		{
			scale.BaseScale = reverseScale;
			scale.TargetScale = reverseScale * 1f;
			scale.ScaleReset();
		}
		this.goUnavailableTips.transform.localScale = reverseScale;
		this.goAlterInfo.transform.localScale = reverseScale;
		base.GetComponentInChildren<HorizontalLayoutGroup>().spacing = 32f * MapElementSettlementBtn.<Scale>g__NewFactor|20_0(factor2, 0.25f);
		base.transform.localScale = Vector3.one * (Mathf.Pow(1f / wheel, 1.375f) * wheel);
	}

	// Token: 0x06003CD0 RID: 15568 RVA: 0x001E99EC File Offset: 0x001E7BEC
	protected override void OnCreate()
	{
		CButton buildingBtn = this.btnEnterBuildingArea;
		CButton infoBtn = this.btnSettlementInformation;
		CButton sectBtn = this.btnSect;
		CButton warehouseBtn = this.btnWareHouse;
		CButton merchant = this.btnMerchant;
		warehouseBtn.ClearAndAddListener(new Action(this.OnClickWarehouse));
		sectBtn.ClearAndAddListener(new Action(this.OnClickSect));
		infoBtn.ClearAndAddListener(new Action(this.OnClickInfo));
		buildingBtn.ClearAndAddListener(new Action(this.OnClickBuilding));
		merchant.ClearAndAddListener(new Action(this.OnClickMerchant));
	}

	// Token: 0x06003CD1 RID: 15569 RVA: 0x001E9A80 File Offset: 0x001E7C80
	protected override void OnRefresh()
	{
		bool ignore = MapElementBase.MapModel.CurrentLocation.Equals(base.BlockLocation);
		this.btnEnterBuildingArea.GetComponentInChildren<CInputEventImage>().IgnoreDrag = ignore;
		this.btnSettlementInformation.GetComponentInChildren<CInputEventImage>().IgnoreDrag = ignore;
		this.btnSect.GetComponent<CInputEventImage>().IgnoreDrag = ignore;
		this.btnWareHouse.GetComponent<CInputEventImage>().IgnoreDrag = ignore;
		this.btnMerchant.GetComponent<CInputEventImage>().IgnoreDrag = ignore;
		this.UpdateSettlementInfo();
		OrganizationDomainMethod.AsyncCall.GetSettlementTreasuryDisplayData(this.Dispatcher, this._settlementInfo.SettlementId, 0, delegate(int offset, RawDataPool dataPool)
		{
			SettlementTreasuryDisplayData settlementTreasuryDisplayData = default(SettlementTreasuryDisplayData);
			Serializer.Deserialize(dataPool, offset, ref settlementTreasuryDisplayData);
			byte alertTime = settlementTreasuryDisplayData.AlertTime;
			this._isOnAlert = (alertTime > 0);
			SettlementBtnPrefabAlterInfo alterInfo = this.goAlterInfo.GetComponent<SettlementBtnPrefabAlterInfo>();
			alterInfo.gameObject.SetActive(this._isOnAlert);
			bool isOnAlert = this._isOnAlert;
			if (isOnAlert)
			{
				alterInfo.txtMeshAlertTime.text = alertTime.ToString();
			}
			this.RefreshMerchant();
		});
	}

	// Token: 0x06003CD2 RID: 15570 RVA: 0x001E9B2C File Offset: 0x001E7D2C
	private void RefreshMerchant()
	{
		CButton merchant = this.btnMerchant;
		MapBlockData selectedBlock = MapElementBase.MapModel.SelectedBlock;
		bool flag = ((selectedBlock != null) ? new EMapBlockType?(selectedBlock.BlockType) : null) == EMapBlockType.City;
		if (flag)
		{
			TaiwuDomainMethod.AsyncCall.GetSelectMapBlockHasMerchantId(this.Dispatcher, base.BlockLocation, new AsyncMethodCallbackDelegate(this.UpdateMerchantEntrance));
		}
		else
		{
			merchant.transform.parent.gameObject.SetActive(false);
			this.UpdateGroup();
		}
	}

	// Token: 0x06003CD3 RID: 15571 RVA: 0x001E9BC4 File Offset: 0x001E7DC4
	private void UpdateMerchantEntrance(int offset, RawDataPool dataPool)
	{
		CButton merchant = this.btnMerchant;
		int selectMapBlockHasMerchantId = -1;
		Serializer.Deserialize(dataPool, offset, ref selectMapBlockHasMerchantId);
		bool flag = selectMapBlockHasMerchantId > -1;
		if (flag)
		{
			this.UpdateSettlementInfo();
			List<short> visitedSettlements = MapElementBase.MapModel.VisitedSettlements;
			bool isVisited = visitedSettlements != null && visitedSettlements.Contains(this._settlementInfo.SettlementId);
			MapBlockData playerAtBlock = MapElementBase.MapModel.PlayerAtBlock;
			short? num;
			if (playerAtBlock == null)
			{
				num = null;
			}
			else
			{
				MapBlockData rootBlock = playerAtBlock.GetRootBlock();
				num = ((rootBlock != null) ? new short?(rootBlock.BlockId) : null);
			}
			short blockId = num ?? -1;
			bool isInBlock = base.BlockLocation.BlockId == blockId;
			merchant.transform.parent.gameObject.SetActive(true);
			merchant.interactable = (isVisited && isInBlock);
			this.SetButtonTextColor(merchant);
		}
		else
		{
			merchant.transform.parent.gameObject.SetActive(false);
		}
		this.UpdateGroup();
	}

	// Token: 0x06003CD4 RID: 15572 RVA: 0x001E9CC8 File Offset: 0x001E7EC8
	private void UpdateSettlementInfo()
	{
		this._settlementInfo = MapElementBase.MapModel.Areas[(int)base.BlockLocation.AreaId].SettlementInfos.First((SettlementInfo info) => info.BlockId == base.BlockLocation.BlockId);
	}

	// Token: 0x06003CD5 RID: 15573 RVA: 0x001E9D00 File Offset: 0x001E7F00
	private void UpdateGroup()
	{
		bool flag = base.BlockData == null;
		if (!flag)
		{
			this.UpdateSettlementInfo();
			MapBlockItem blockConfig = base.BlockData.GetConfig();
			WorldMapModel mapModel = MapElementBase.MapModel;
			bool atArea = base.BlockLocation.AreaId == mapModel.ShowingAreaId;
			int blockId = (int)base.BlockLocation.BlockId;
			MapBlockData playerAtBlock = mapModel.PlayerAtBlock;
			short? num;
			if (playerAtBlock == null)
			{
				num = null;
			}
			else
			{
				MapBlockData rootBlock = playerAtBlock.GetRootBlock();
				num = ((rootBlock != null) ? new short?(rootBlock.BlockId) : null);
			}
			short? num2 = num;
			int? num3 = (num2 != null) ? new int?((int)num2.GetValueOrDefault()) : null;
			bool flag2;
			if (!(blockId == num3.GetValueOrDefault() & num3 != null))
			{
				int blockId2 = (int)base.BlockLocation.BlockId;
				MapBlockData selectedBlock = mapModel.SelectedBlock;
				short? num4;
				if (selectedBlock == null)
				{
					num4 = null;
				}
				else
				{
					MapBlockData rootBlock2 = selectedBlock.GetRootBlock();
					num4 = ((rootBlock2 != null) ? new short?(rootBlock2.BlockId) : null);
				}
				num2 = num4;
				num3 = ((num2 != null) ? new int?((int)num2.GetValueOrDefault()) : null);
				flag2 = (blockId2 == num3.GetValueOrDefault() & num3 != null);
			}
			else
			{
				flag2 = true;
			}
			bool atBlock = flag2;
			bool visible = atArea && atBlock;
			bool flag3 = !visible;
			if (!flag3)
			{
				byte blockSize = blockConfig.Size;
				byte areaSize = MapElementBase.MapModel.GetAreaSize(base.BlockLocation.AreaId);
				base.GetComponent<RectTransform>().anchoredPosition = this.PosGenerator(new Location(base.BlockLocation.AreaId, base.BlockLocation.BlockId + (short)(areaSize * (blockSize - 1))));
				bool canEnterBuildingArea = SingletonObject.getInstance<TutorialChapterModel>().CanShowEnterIndustry;
				List<short> visitedSettlements = MapElementBase.MapModel.VisitedSettlements;
				bool canClick = visitedSettlements != null && visitedSettlements.Contains(this._settlementInfo.SettlementId);
				CButton buildingBtn = this.btnEnterBuildingArea;
				CButton infoBtn = this.btnSettlementInformation;
				CButton sectBtn = this.btnSect;
				CButton warehouseBtn = this.btnWareHouse;
				GameObject unavailableTips = this.goUnavailableTips;
				buildingBtn.transform.parent.gameObject.SetActive(canEnterBuildingArea);
				buildingBtn.interactable = (canClick && !this._isOnAlert);
				infoBtn.transform.parent.gameObject.SetActive(this.ShouldActiveInfo);
				infoBtn.interactable = canClick;
				sectBtn.transform.parent.gameObject.SetActive(this._settlementInfo.OrgTemplateId >= 0 && Organization.Instance[this._settlementInfo.OrgTemplateId].IsSect);
				warehouseBtn.transform.parent.gameObject.SetActive(!SingletonObject.getInstance<TutorialChapterModel>().InGuiding);
				warehouseBtn.interactable = (canClick && !this._isOnAlert);
				unavailableTips.SetActive(!canClick && canEnterBuildingArea);
				base.gameObject.SetActive(true);
				int totalCount = this.layoutButtons.Sum((CButton x) => x.gameObject.activeInHierarchy ? 1 : 0);
				int layoutIndex = 0;
				for (int i = 0; i < this.layoutButtons.Length; i++)
				{
					CButton button = this.layoutButtons[i];
					bool activeInHierarchy = button.gameObject.activeInHierarchy;
					if (activeInHierarchy)
					{
						layoutIndex++;
						bool needUp = layoutIndex != 1 && layoutIndex != totalCount;
						button.GetComponent<RectTransform>().anchoredPosition = (needUp ? Vector2.zero.SetY(50f) : Vector2.zero);
					}
				}
				this.SetButtonTextColor(buildingBtn);
				this.SetButtonTextColor(infoBtn);
				this.SetButtonTextColor(sectBtn);
				this.SetButtonTextColor(warehouseBtn);
			}
		}
	}

	// Token: 0x06003CD6 RID: 15574 RVA: 0x001EA0CC File Offset: 0x001E82CC
	private void SetButtonTextColor(CButton button)
	{
		TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
		Color color = button.interactable ? this.btnNameColor : this.btnNameGrayColor;
		text.color = color;
		button.GetComponent<PointerScaleAnim>().enabled = button.interactable;
	}

	// Token: 0x06003CD7 RID: 15575 RVA: 0x001EA112 File Offset: 0x001E8312
	protected override void OnCollect()
	{
		this._settlementInfo = default(SettlementInfo);
	}

	// Token: 0x06003CD8 RID: 15576 RVA: 0x001EA124 File Offset: 0x001E8324
	private void OnClickSect()
	{
		bool isMapMoving = MapElementBase.IsMapMoving;
		if (!isMapMoving)
		{
			ArgumentBox args = EasyPool.Get<ArgumentBox>().Set("SectTemplateId", this._settlementInfo.OrgTemplateId);
			CommandManager.AddCommand<CommandMaskUIWithArgs, UIElement, ArgumentBox>(EPriority.ShowUINormal, UIElement.CombatSkillTree, args);
		}
	}

	// Token: 0x06003CD9 RID: 15577 RVA: 0x001EA168 File Offset: 0x001E8368
	private void OnClickInfo()
	{
		bool isMapMoving = MapElementBase.IsMapMoving;
		if (!isMapMoving)
		{
			ArgumentBox args = EasyPool.Get<ArgumentBox>().Set("SettlementId", this._settlementInfo.SettlementId);
			CommandManager.AddCommandShowUI(EPriority.ShowUINormal, UIElement.SettlementInformation, args);
		}
	}

	// Token: 0x06003CDA RID: 15578 RVA: 0x001EA1AC File Offset: 0x001E83AC
	private void OnClickWarehouse()
	{
		bool isMapMoving = MapElementBase.IsMapMoving;
		if (!isMapMoving)
		{
			CommandManager.AddCommandShowUI(EPriority.ShowUINormal, UIElement.Warehouse, null);
		}
	}

	// Token: 0x06003CDB RID: 15579 RVA: 0x001EA1D4 File Offset: 0x001E83D4
	private void OnClickMerchant()
	{
		bool isMapMoving = MapElementBase.IsMapMoving;
		if (!isMapMoving)
		{
			TaiwuDomainMethod.Call.EnterMerchant();
			CButton merchant = this.btnMerchant;
			merchant.interactable = false;
		}
	}

	// Token: 0x06003CDC RID: 15580 RVA: 0x001EA204 File Offset: 0x001E8404
	private void OnClickBuilding()
	{
		bool isMapMoving = MapElementBase.IsMapMoving;
		if (!isMapMoving)
		{
			WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
			bool flag = mapModel.GetTaiwuVillageBlock().Equals(base.BlockLocation);
			if (flag)
			{
				BasicGameData basicGameData = SingletonObject.getInstance<BasicGameData>();
				bool flag2 = basicGameData.IsDreamBack && !basicGameData.IsDreamBackStateUnlocked(4);
				if (flag2)
				{
					bool flag3 = !this._isDreamBackUnlockBuilding;
					if (flag3)
					{
						this._isDreamBackUnlockBuilding = true;
						TaiwuEventDomainMethod.Call.TaiwuCrossArchiveFindMemory(4);
					}
					return;
				}
			}
			GEvent.OnEvent(UiEvents.HideMapBlockCharList, null);
			ArgumentBox argsBox = EasyPool.Get<ArgumentBox>();
			argsBox.Set("AreaId", base.BlockLocation.AreaId);
			argsBox.Set("BlockId", base.BlockLocation.BlockId);
			UIElement.BuildingArea.SetOnInitArgs(argsBox);
			CommandManager.AddCommandStackUI(EPriority.StackUINormal, UIElement.StateBuilding, null);
		}
	}

	// Token: 0x06003CDE RID: 15582 RVA: 0x001EA300 File Offset: 0x001E8500
	[CompilerGenerated]
	internal static float <Scale>g__NewFactor|20_0(float factor, float pow)
	{
		return 1f + (factor - 1f) * pow;
	}

	// Token: 0x04002B99 RID: 11161
	private SettlementInfo _settlementInfo;

	// Token: 0x04002B9A RID: 11162
	[SerializeField]
	private CButton[] layoutButtons = new CButton[4];

	// Token: 0x04002B9B RID: 11163
	[SerializeField]
	private GameObject goUnavailableTips;

	// Token: 0x04002B9C RID: 11164
	[SerializeField]
	private CButton btnEnterBuildingArea;

	// Token: 0x04002B9D RID: 11165
	[SerializeField]
	private CButton btnSect;

	// Token: 0x04002B9E RID: 11166
	[SerializeField]
	private CButton btnSettlementInformation;

	// Token: 0x04002B9F RID: 11167
	[SerializeField]
	private CButton btnWareHouse;

	// Token: 0x04002BA0 RID: 11168
	[SerializeField]
	private CButton btnMerchant;

	// Token: 0x04002BA1 RID: 11169
	[SerializeField]
	private GameObject goAlterInfo;

	// Token: 0x04002BA2 RID: 11170
	[SerializeField]
	private Color btnNameColor;

	// Token: 0x04002BA3 RID: 11171
	[SerializeField]
	private Color btnNameGrayColor;

	// Token: 0x04002BA4 RID: 11172
	private bool _isOnAlert;

	// Token: 0x04002BA5 RID: 11173
	private bool _isDreamBackUnlockBuilding = false;
}
