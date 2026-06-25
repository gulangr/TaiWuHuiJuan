using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Building;
using Game.Views.Building.BuildingManage;
using Game.Views.Map;
using GameData.Common;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Organization;
using GameData.Domains.Organization.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Building
{
	// Token: 0x02000BE0 RID: 3040
	public class ViewBuildingArea : UIBase
	{
		// Token: 0x1700105E RID: 4190
		// (get) Token: 0x060098E7 RID: 39143 RVA: 0x00472F31 File Offset: 0x00471131
		public Location CurrentLocation
		{
			get
			{
				return new Location(this._areaId, this._blockId);
			}
		}

		// Token: 0x1700105F RID: 4191
		// (get) Token: 0x060098E8 RID: 39144 RVA: 0x00472F44 File Offset: 0x00471144
		private BuildingModel BuildingModel
		{
			get
			{
				return SingletonObject.getInstance<BuildingModel>();
			}
		}

		// Token: 0x17001060 RID: 4192
		// (get) Token: 0x060098E9 RID: 39145 RVA: 0x00472F4B File Offset: 0x0047114B
		private WorldMapModel WorldMapModel
		{
			get
			{
				return SingletonObject.getInstance<WorldMapModel>();
			}
		}

		// Token: 0x060098EA RID: 39146 RVA: 0x00472F54 File Offset: 0x00471154
		private void Awake()
		{
			this._buildingDict = new Dictionary<short, BuildingBlockData>();
			this._buildingPlacementData = new ViewBuildingArea.BuildingPlacementData();
			BuildingAreaBlock[] blockRefers = this.buildingBlockHolder.GetComponentsInChildren<BuildingAreaBlock>();
			for (int i = 0; i < blockRefers.Length; i++)
			{
				bool flag = blockRefers[i].gameObject.name.Contains("SizeOne");
				if (flag)
				{
					this._blockListSize1.Enqueue(blockRefers[i]);
				}
				else
				{
					this._blockListSize2.Enqueue(blockRefers[i]);
				}
			}
			this._roadPool = new PoolItem("ui_BuildingArea_RoadPrefab", this.road);
			this.settlementRoadAnimation.ChickenObjectPool = new PoolItem("ui_BuildingArea_ChickenPrefab", this.settlementRoadAnimation.chickenTemplate);
			this.settlementRoadAnimation.VillagerObjectPool = new PoolItem("ui_BuildingArea_VillagerPrefab", this.settlementRoadAnimation.villagerTemplate);
			this._getItemPool = new PoolItem("GetItemPrefab", this.buildingInfoHolder.getEarnHolder);
			this._getPeoplePool = new PoolItem("GetPeoplePrefab", this.buildingInfoHolder.getEarnHolder);
			this._getResourcePool = new PoolItem("GetResourcePrefab", this.buildingInfoHolder.getEarnHolder);
			this._shopTipPool = new PoolItem("ShopTipPrefab", this.buildingInfoHolder.shopTip);
			this._exceptionPool = new PoolItem("ExceptionPrefab", this.buildingInfoHolder.exception);
			this._damageInfoPool = new PoolItem("DamageInfoPrefab", this.buildingInfoHolder.damageInfo);
			this._residentsInfoPool = new PoolItem("ResidentsInfoPrefab", this.buildingInfoHolder.residentsInfo);
			this._samsaraInfoPool = new PoolItem("SamsaraInfoPrefab", this.buildingInfoHolder.samsaraInfo);
			this._buildingOperatePool = new PoolItem("BuildingOperatePrefab", this.buildingInfoHolder.buildingOperate);
			this._levelAndNamePool = new PoolItem("LevelAndNamePrefab", this.buildingInfoHolder.levelAndName);
			this._treasuryResourceInfoPool = new PoolItem("TreasuryResourceInfoPrefab", this.buildingInfoHolder.treasuryResourceInfo);
			this._coreProducingCooldownPool = new PoolItem("CoreProducingCooldownPrefab", this.buildingInfoHolder.coreCooldownInfo);
			this._teaHorseCaravanPool = new PoolItem("TeaHorseCaravanPrefab", this.buildingInfoHolder.teaHorseCaravanInfo);
			this._getItemDict = new Dictionary<short, BuildingBlockInfo>();
			this._getPeopleDict = new Dictionary<short, BuildingBlockInfo>();
			this._getResourceDict = new Dictionary<short, BuildingBlockInfo>();
			this._shopTipDict = new Dictionary<short, BuildingBlockInfo>();
			this._exceptionDict = new Dictionary<short, BuildingBlockInfo>();
			this._damageInfoDict = new Dictionary<short, BuildingBlockInfo>();
			this._residentsInfoDict = new Dictionary<short, BuildingBlockInfo>();
			this._samsaraInfoDict = new Dictionary<short, BuildingBlockInfo>();
			this._buildingOperateDict = new Dictionary<short, BuildingBlockInfo>();
			this._levelAndNameDict = new Dictionary<short, BuildingBlockInfo>();
			this._treasuryResourceInfoDict = new Dictionary<short, BuildingBlockInfo>();
			this._coreProducingCooldownDict = new Dictionary<short, BuildingBlockInfo>();
			this._teaHorseCaravanDict = new Dictionary<short, BuildingBlockInfo>();
			this._moveRoot = this.moveAndScaleRoot;
			this.buildingAreaInfo.SetBuildingArea(this);
			this.settlementRoadAnimation.SetBuildingArea(this);
			this.buildingExceptionInfo.SetBuildingArea(this);
			this.buildingAreaInfo.gameObject.SetActive(true);
			this.planBuilding.gameObject.SetActive(false);
			this.multiplyRemove.gameObject.SetActive(false);
			this.buildingAreaResourceChange.gameObject.SetActive(false);
			PoolManager.SetSrcObject("CreateBuildingEffectSmallKey", this.createBuildingEffectSmall);
			PoolManager.SetSrcObject("CreateBuildingDoneEffectSmallKey", this.createBuildingDoneEffectSmall);
			PoolManager.SetSrcObject("CreateBuildingEffectLargeKey", this.createBuildingEffectLarge);
			PoolManager.SetSrcObject("CreateBuildingDoneEffectLargeKey", this.createBuildingDoneEffectLarge);
		}

		// Token: 0x060098EB RID: 39147 RVA: 0x004732DC File Offset: 0x004714DC
		public override void OnInit(ArgumentBox argsBox)
		{
			bool flag = argsBox != null;
			if (flag)
			{
				argsBox.Get("AreaId", out this._areaId);
				argsBox.Get("BlockId", out this._blockId);
				argsBox.Get("OpenBuildingOverview", out this._defaultOpenBuildingOverview);
				argsBox.Get("AutoSelectBuildingTemplateId", out this._autoSelectBuildingTemplateId);
				argsBox.Get("NeedClearBuildingOverviewFilter", out this._needClearBuildingOverviewFilter);
			}
			this.isSecretVilliage = SingletonObject.getInstance<WorldMapModel>().IsAtSecretVillage();
			this.mapWeather.SetArea(this._areaId);
			this.ResetState();
			this._isTaiwuVillage = this.WorldMapModel.IsAtTaiwuVillage(this._areaId, this._blockId);
			this.buildingExceptionInfo.gameObject.SetActive(this._isTaiwuVillage);
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				this.RefreshNewlyCreatedBuilding();
				BuildingDomainMethod.Call.GetBuildingAreaData(this.Element.GameDataListenerId, new Location(this._areaId, this._blockId));
				MapDomainMethod.Call.GetBlockData(this.Element.GameDataListenerId, this._areaId, this._blockId);
				BuildingDomainMethod.AsyncCall.IsHaveChickenKing(this, delegate(int offset, RawDataPool dataPool)
				{
					Serializer.Deserialize(dataPool, offset, ref this.settlementRoadAnimation.HaveChickenKing);
				});
				OrganizationDomainMethod.Call.GetSettlementIdByAreaIdAndBlockId(this.Element.GameDataListenerId, this._areaId, this._blockId);
				AudioManager.Instance.PlaySound("ui_industry_open", false, false);
				SingletonObject.getInstance<WorldMapModel>().UpdateBgm();
			}));
			UIElement element2 = this.Element;
			element2.OnShowed = (Action)Delegate.Combine(element2.OnShowed, new Action(delegate()
			{
				TaiwuEventDomainMethod.Call.TriggerListener("TutorialChaptersBuildingAreaShowed", true);
				TaiwuEventDomainMethod.Call.OnEnterBuildingArea(this._rootLocation);
				this.moveAndScaleRoot.GetComponent<MouseWheelScale>().OnPointerExit();
			}));
			this._stringBuilder = EasyPool.Get<StringBuilder>();
			this._stringBuilder2 = EasyPool.Get<StringBuilder>();
			this._rootLocation = new Location(this._areaId, this._blockId);
			this.SetCanUseBuilding();
			GlobalDomainMethod.Call.InvokeGuidingTrigger(143);
			bool isTaiwuVillage = this._isTaiwuVillage;
			if (isTaiwuVillage)
			{
				GlobalDomainMethod.Call.InvokeGuidingTrigger(142);
			}
			bool defaultOpenBuildingOverview = this._defaultOpenBuildingOverview;
			if (defaultOpenBuildingOverview)
			{
				this.OpenBuildingOverview(this._autoSelectBuildingTemplateId);
			}
		}

		// Token: 0x060098EC RID: 39148 RVA: 0x00473466 File Offset: 0x00471666
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(5, 9, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(5, 10, ulong.MaxValue, null));
		}

		// Token: 0x060098ED RID: 39149 RVA: 0x00473497 File Offset: 0x00471697
		private void ResetState()
		{
			this.settlementRoadAnimation.ChickenInit = false;
			this.NeedDataListenerId = true;
			this.moveAndScaleRoot.GetComponent<CanvasGroup>().blocksRaycasts = true;
		}

		// Token: 0x060098EE RID: 39150 RVA: 0x004734C0 File Offset: 0x004716C0
		private void OpenBuildingOverview(short autoSelectBuildingTemplateId)
		{
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
			argumentBox.Clear();
			argumentBox.Set("AutoSelectBuildingTemplateId", autoSelectBuildingTemplateId);
			argumentBox.Set("NeedClearBuildingOverviewFilter", this._needClearBuildingOverviewFilter);
			UIElement.BuildingOverview.SetOnInitArgs(argumentBox);
			UIManager.Instance.ShowUI(UIElement.BuildingOverview, true);
		}

		// Token: 0x060098EF RID: 39151 RVA: 0x00473518 File Offset: 0x00471718
		private void SetCanUseBuilding()
		{
			bool flag = this.WorldMapModel.BlockGroupDict.ContainsKey(this._rootLocation);
			if (flag)
			{
				HashSet<short> blockList = this.WorldMapModel.BlockGroupDict[this._rootLocation];
				this.canUseBuilding = (blockList.Contains(this.WorldMapModel.CurrentBlockId) || (this.WorldMapModel.CurrentAreaId == this._rootLocation.AreaId && this.WorldMapModel.CurrentBlockId == this._rootLocation.BlockId));
			}
			else
			{
				this.canUseBuilding = (this.WorldMapModel.CurrentAreaId == this._rootLocation.AreaId && this.WorldMapModel.CurrentBlockId == this._rootLocation.BlockId);
			}
		}

		// Token: 0x060098F0 RID: 39152 RVA: 0x004735E8 File Offset: 0x004717E8
		private void OnEnable()
		{
			GEvent.Add(UiEvents.BuildingOperatorChange, new GEvent.Callback(this.OnBuildingBlockUpdate));
			GEvent.Add(UiEvents.BuildingShopManagerChange, new GEvent.Callback(this.OnBuildingBlockUpdate));
			GEvent.Add(UiEvents.BuildingCustomNameChange, new GEvent.Callback(this.OnBuildingCustomNameChange));
			GEvent.Add(EEvents.OnMonthChange, new GEvent.Callback(this.OnMonthChange));
			GEvent.Add(UiEvents.ShowAdvanceMonthConfirm, new GEvent.Callback(this.ShowAdvanceMonthConfirm));
			GEvent.Add(UiEvents.UpdateAllBlockInfo, new GEvent.Callback(this.OnUpdateAllBlockInfo));
			GEvent.Add(UiEvents.UpdateRoad, delegate(ArgumentBox _)
			{
				this.UpdateRoad();
			});
			GEvent.Add(UiEvents.HideBuildingArea, new GEvent.Callback(this.HideBuildingArea));
			GEvent.Add(UiEvents.WorldMapPlayerBlockChange, new GEvent.Callback(this.WorldMapPlayerBlockChange));
			GEvent.Add(UiEvents.SettlementTreasuryEffect, new GEvent.Callback(this.GetSettlementTreasuryDisplayData));
			GEvent.Add(EEvents.OnAreaSpiritualDebtChange, new GEvent.Callback(this.SetBottomInfoWithArg));
			GEvent.Add(UiEvents.BuildQiwenxingtai, new GEvent.Callback(this.OnTryBuildQiwenxingtai));
			GEvent.Add(UiEvents.RefreshExceptionInfo, new GEvent.Callback(this.RefreshExceptionInfo));
			GEvent.Add(UiEvents.BuildingManageClosed, new GEvent.Callback(this.OnBuildingManageClosed));
			GEvent.Add(UiEvents.NotifySwitchBuildingManage, new GEvent.Callback(this.NotifySwitchBuildingManage));
			GEvent.Add(UiEvents.RequestOpenBuildingManage, new GEvent.Callback(this.OnRequestOpenBuildingManage));
			GEvent.Add(UiEvents.QuickActionMenuBackgroundClicked, new GEvent.Callback(this.OnQuickActionMenuBackgroundClicked));
			GEvent.Add(UiEvents.BuildingBlockDataChange, new GEvent.Callback(this.BuildingBlockDataChange));
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
			GEvent.Add(UiEvents.RepairAllBuilding, new GEvent.Callback(this.RepairAllBuilding));
			GEvent.Add(UiEvents.RepairBuilding, new GEvent.Callback(this.RepairBuilding));
			GEvent.Add(UiEvents.BuildingAreaHide, new GEvent.Callback(this.HideBuildingArea));
			GEvent.Add(UiEvents.MonthNotifyProcessComplete, new GEvent.Callback(this.OnMonthNotifyProcessComplete));
			GEvent.Add(UiEvents.WorldMapResetMapCamera, new GEvent.Callback(this.WorldMapResetMapCamera));
			this.showMask.DOKill(false);
			this.showMask.color = Color.black;
			this.showMask.gameObject.SetActive(true);
			this.showMask.DOFade(0f, 0.6666667f).SetDelay(0.18f).SetEase(Ease.Linear).OnComplete(delegate
			{
				this.showMask.gameObject.SetActive(false);
			});
			GEvent.OnEvent(UiEvents.OnSetBuildingBtnShow, EasyPool.Get<ArgumentBox>().Set("show", false));
			GEvent.OnEvent(UiEvents.OnShowBuildingArea, null);
		}

		// Token: 0x060098F1 RID: 39153 RVA: 0x004738F8 File Offset: 0x00471AF8
		private void WorldMapPlayerBlockChange(ArgumentBox argbox)
		{
			bool isTaiwuVillage = this._isTaiwuVillage;
			if (!isTaiwuVillage)
			{
				this._worldMapPlayerBlockChangeAction = (Action)Delegate.Combine(this._worldMapPlayerBlockChangeAction, new Action(this.WorldMapPlayerBlockChange));
			}
		}

		// Token: 0x060098F2 RID: 39154 RVA: 0x00473934 File Offset: 0x00471B34
		public void WorldMapPlayerBlockChange()
		{
			this.SetCanUseBuilding();
			this.EscapeUpdateBlockInteractable();
		}

		// Token: 0x060098F3 RID: 39155 RVA: 0x00473945 File Offset: 0x00471B45
		private void HideBuildingArea(ArgumentBox argbox)
		{
			UIManager.Instance.StackBack(null);
		}

		// Token: 0x060098F4 RID: 39156 RVA: 0x00473954 File Offset: 0x00471B54
		private void ShowAdvanceMonthConfirm(ArgumentBox argbox)
		{
			bool flag = this._buildingPlacementData.CurBuildingIcon != null;
			if (flag)
			{
				this.CancelPlaceBuilding(false);
			}
		}

		// Token: 0x060098F5 RID: 39157 RVA: 0x00473984 File Offset: 0x00471B84
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.BuildingOperatorChange, new GEvent.Callback(this.OnBuildingBlockUpdate));
			GEvent.Remove(UiEvents.BuildingShopManagerChange, new GEvent.Callback(this.OnBuildingBlockUpdate));
			GEvent.Remove(UiEvents.BuildingCustomNameChange, new GEvent.Callback(this.OnBuildingCustomNameChange));
			GEvent.Remove(EEvents.OnMonthChange, new GEvent.Callback(this.OnMonthChange));
			GEvent.Remove(UiEvents.ShowAdvanceMonthConfirm, new GEvent.Callback(this.ShowAdvanceMonthConfirm));
			GEvent.Remove(UiEvents.UpdateAllBlockInfo, new GEvent.Callback(this.OnUpdateAllBlockInfo));
			GEvent.Remove(UiEvents.UpdateRoad, delegate(ArgumentBox _)
			{
				this.UpdateRoad();
			});
			GEvent.Remove(UiEvents.HideBuildingArea, new GEvent.Callback(this.HideBuildingArea));
			GEvent.Remove(UiEvents.WorldMapPlayerBlockChange, new GEvent.Callback(this.WorldMapPlayerBlockChange));
			GEvent.Remove(UiEvents.SettlementTreasuryEffect, new GEvent.Callback(this.GetSettlementTreasuryDisplayData));
			GEvent.Remove(EEvents.OnAreaSpiritualDebtChange, new GEvent.Callback(this.SetBottomInfoWithArg));
			GEvent.Remove(UiEvents.BuildQiwenxingtai, new GEvent.Callback(this.OnTryBuildQiwenxingtai));
			GEvent.Remove(UiEvents.RefreshExceptionInfo, new GEvent.Callback(this.RefreshExceptionInfo));
			GEvent.Remove(UiEvents.BuildingManageClosed, new GEvent.Callback(this.OnBuildingManageClosed));
			GEvent.Remove(UiEvents.NotifySwitchBuildingManage, new GEvent.Callback(this.NotifySwitchBuildingManage));
			GEvent.Remove(UiEvents.RequestOpenBuildingManage, new GEvent.Callback(this.OnRequestOpenBuildingManage));
			GEvent.Remove(UiEvents.QuickActionMenuBackgroundClicked, new GEvent.Callback(this.OnQuickActionMenuBackgroundClicked));
			GEvent.Remove(UiEvents.BuildingBlockDataChange, new GEvent.Callback(this.BuildingBlockDataChange));
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
			GEvent.Remove(UiEvents.RepairAllBuilding, new GEvent.Callback(this.RepairAllBuilding));
			GEvent.Remove(UiEvents.RepairBuilding, new GEvent.Callback(this.RepairBuilding));
			GEvent.Remove(UiEvents.MonthNotifyProcessComplete, new GEvent.Callback(this.OnMonthNotifyProcessComplete));
			GEvent.Remove(UiEvents.WorldMapResetMapCamera, new GEvent.Callback(this.WorldMapResetMapCamera));
			EasyPool.Free<StringBuilder>(this._stringBuilder);
			EasyPool.Free<StringBuilder>(this._stringBuilder2);
			GEvent.OnEvent(UiEvents.OnSetBuildingBtnShow, EasyPool.Get<ArgumentBox>().Set("show", true));
			this.ClearAnimation();
			bool flag = this.isPlacingBuildingNow;
			if (flag)
			{
				this.CancelPlaceBuilding(false);
			}
			this.ClearBlockRefersDict();
			for (int i = this.border.childCount - 1; i >= 0; i--)
			{
				Object.Destroy(this.border.GetChild(i).gameObject);
			}
			GEvent.OnEvent(UiEvents.OnHideBuildingArea, null);
			SingletonObject.getInstance<WorldMapModel>().UpdateBgm();
			BuildingDomainMethod.Call.TryShowNotifications();
		}

		// Token: 0x060098F6 RID: 39158 RVA: 0x00473C94 File Offset: 0x00471E94
		private void RepairAllBuilding(ArgumentBox argBox)
		{
			for (int i = 0; i < ViewBuildingArea.BlockList.Count; i++)
			{
				BuildingBlockData block = ViewBuildingArea.BlockList[i];
				bool flag = block.TemplateId <= 0;
				if (!flag)
				{
					BuildingBlockItem config = BuildingBlock.Instance[block.TemplateId];
					block.Durability = config.MaxDurability;
				}
			}
			this.UpdateAllBlockInfo();
			this.RefreshExceptionInfo(null);
		}

		// Token: 0x060098F7 RID: 39159 RVA: 0x00473D0C File Offset: 0x00471F0C
		private void RepairBuilding(ArgumentBox argBox)
		{
			short blockIndex;
			argBox.Get("BuildingBlockIndex", out blockIndex);
			BuildingBlockData block = ViewBuildingArea.BlockList[(int)blockIndex];
			bool flag = block.TemplateId <= 0;
			if (!flag)
			{
				BuildingBlockItem config = BuildingBlock.Instance[block.TemplateId];
				block.Durability = config.MaxDurability;
				BuildingAreaBlock blockRefers = this._blockRefersDict[blockIndex];
				BuildingBlockKey buildingBlockKey = new BuildingBlockKey(this._areaId, this._blockId, blockIndex);
				this.UpdateBlockInfo(block, buildingBlockKey, blockRefers, config);
			}
		}

		// Token: 0x060098F8 RID: 39160 RVA: 0x00473D92 File Offset: 0x00471F92
		private void TopUiChanged(ArgumentBox argBox)
		{
			BuildingDomainMethod.Call.GetCanPluckFeatherChickenIds(this.Element.GameDataListenerId);
		}

		// Token: 0x060098F9 RID: 39161 RVA: 0x00473DA8 File Offset: 0x00471FA8
		private void NotifySwitchBuildingManage(ArgumentBox argBox)
		{
			short blockIndex;
			argBox.Get("BuildingBlockIndex", out blockIndex);
			BuildingBlockData buildingBlockData = ViewBuildingArea.BlockList[(int)blockIndex];
			bool flag = UIManager.Instance.IsElementActive(UIElement.BuildingManage);
			if (flag)
			{
				ArgumentBox argsBox = EasyPool.Get<ArgumentBox>();
				argsBox.SetObject("BuildingBlockData", buildingBlockData);
				argsBox.Set<BuildingAreaData>("BuildingAreaData", this._areaData);
				GEvent.OnEvent(UiEvents.SwitchBuildingManage, argsBox);
			}
			else
			{
				this.OpenBuildingManage(blockIndex, -1, Game.Views.Building.BuildingManage.BuildingManageTogKey.Invalid);
			}
		}

		// Token: 0x060098FA RID: 39162 RVA: 0x00473E28 File Offset: 0x00472028
		private void OnDestroy()
		{
			PoolItem roadPool = this._roadPool;
			if (roadPool != null)
			{
				roadPool.Destroy();
			}
			this._roadPool = null;
			PoolItem chickenObjectPool = this.settlementRoadAnimation.ChickenObjectPool;
			if (chickenObjectPool != null)
			{
				chickenObjectPool.Destroy();
			}
			this.settlementRoadAnimation.ChickenObjectPool = null;
			PoolItem villagerObjectPool = this.settlementRoadAnimation.VillagerObjectPool;
			if (villagerObjectPool != null)
			{
				villagerObjectPool.Destroy();
			}
			this.settlementRoadAnimation.VillagerObjectPool = null;
			PoolManager.RemoveData("CreateBuildingEffectSmallKey");
			PoolManager.RemoveData("CreateBuildingDoneEffectSmallKey");
			PoolManager.RemoveData("CreateBuildingEffectLargeKey");
			PoolManager.RemoveData("CreateBuildingDoneEffectLargeKey");
		}

		// Token: 0x060098FB RID: 39163 RVA: 0x00473EC4 File Offset: 0x004720C4
		private void ClearAnimation()
		{
			foreach (GameObject go in this._animationGoList)
			{
				Object.DestroyImmediate(go);
			}
			this._animationGoList.Clear();
		}

		// Token: 0x060098FC RID: 39164 RVA: 0x00473F28 File Offset: 0x00472128
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
						bool flag = notification.DomainId == 9;
						if (flag)
						{
							ushort methodId = notification.MethodId;
							ushort num = methodId;
							if (num <= 67)
							{
								if (num <= 47)
								{
									if (num == 36 || num == 47)
									{
										ValueTuple<short, BuildingBlockData> retValue = new ValueTuple<short, BuildingBlockData>(0, new BuildingBlockData());
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref retValue);
										this.UpdateBuildingData(new BuildingBlockKey(this._areaId, this._blockId, retValue.Item1), retValue.Item2, true);
										this.RefreshSingleBuildingBuildingOperateState(retValue.Item2);
										this.UpdateRoad();
										this.SetNotScaleElement();
									}
								}
								else if (num != 49)
								{
									if (num == 67)
									{
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._areaData);
										bool flag2 = this.testRoad;
										if (flag2)
										{
											this._areaData.LandFormType = this.testLandFormType;
										}
										BuildingDomainMethod.Call.GetBuildingBlockList(this.Element.GameDataListenerId, this.CurrentLocation);
									}
								}
								else
								{
									ValueTuple<short, BuildingBlockData> building = new ValueTuple<short, BuildingBlockData>(0, new BuildingBlockData());
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref building);
									this.UpdateBuildingData(new BuildingBlockKey(this._areaId, this._blockId, building.Item1), building.Item2, true);
									this.UpdateRoad();
								}
							}
							else if (num <= 102)
							{
								if (num != 68)
								{
									if (num == 102)
									{
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.settlementRoadAnimation.ChickenList);
										this.UpdateRoad();
										bool seekFeather = this.TaskModel.IsTaskInProgress(303);
										bool flag3 = seekFeather;
										if (flag3)
										{
											base.AppendMonitorFieldId(new UIBase.MonitorDataField(19, 136, ulong.MaxValue, null));
										}
										else
										{
											this.TryInitSettlementRoadAnimation();
										}
									}
								}
								else
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref ViewBuildingArea.BlockList);
									this.InitBuildingArea();
									this.RefreshBuildingOperateState();
									this.RefreshExceptionInfo(null);
								}
							}
							else if (num != 147)
							{
								if (num != 217)
								{
									if (num == 222)
									{
										this.settlementRoadAnimation.CanPluckFeatherChickenIds.Clear();
										List<int> temp = new List<int>();
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref temp);
										bool flag4 = temp != null;
										if (flag4)
										{
											for (int i = 0; i < temp.Count; i++)
											{
												this.settlementRoadAnimation.CanPluckFeatherChickenIds.Add(temp[i]);
											}
										}
										this.settlementRoadAnimation.PlayChickenLoseFeatureEffect();
										this.settlementRoadAnimation.UpdateChickenFeather();
									}
								}
								else
								{
									List<ItemDisplayData> itemDisplayDataList = new List<ItemDisplayData>();
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref itemDisplayDataList);
									ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
									argBox.SetObject("ItemList", itemDisplayDataList);
									UIElement.GetItem.SetOnInitArgs(argBox);
									UIManager.Instance.MaskUI(UIElement.GetItem);
									GEvent.OnEvent(UiEvents.OnUpdateQuickBtnState, null);
								}
							}
							else
							{
								BuildingExceptionData buildingExceptionData = new BuildingExceptionData();
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref buildingExceptionData);
								this.RefreshAllException(buildingExceptionData);
							}
						}
						else
						{
							bool flag5 = notification.DomainId == 2 && notification.MethodId == 19;
							if (flag5)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._mapBlockData);
								this.RefreshExceptionInfo(null);
							}
							else
							{
								bool flag6 = notification.DomainId == 3;
								if (flag6)
								{
									bool flag7 = notification.MethodId == 8;
									if (flag7)
									{
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._settlementId);
										this.GetSettlementTreasuryDisplayData(null);
									}
									else
									{
										bool flag8 = notification.MethodId == 11;
										if (flag8)
										{
											Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._settlementTreasuryDisplayData);
											this.RefreshAllBlockTreasuryInfo();
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
					bool flag9 = uid.DomainId == 19 && uid.DataId == 136;
					if (flag9)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this.settlementRoadAnimation.SectFulongLoseFeatherChickensNew);
						this.settlementRoadAnimation.CalcChickenChange();
						this.settlementRoadAnimation.SectFulongLoseFeatherChickensOld = this.settlementRoadAnimation.SectFulongLoseFeatherChickensNew.ToList<int>();
						bool flag10 = !this.settlementRoadAnimation.ChickenInit;
						if (flag10)
						{
							this.TryInitSettlementRoadAnimation();
						}
					}
					else
					{
						bool flag11 = notification.Uid.DomainId == 5;
						if (flag11)
						{
							bool flag12 = uid.DataId == 9;
							if (flag12)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._buildingSpaceLimit);
								this.UpdateBuildingSpaceInfo();
							}
							else
							{
								bool flag13 = uid.DataId == 10;
								if (flag13)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._buildingSpaceCurr);
									this.UpdateBuildingSpaceInfo();
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060098FD RID: 39165 RVA: 0x004744D0 File Offset: 0x004726D0
		private void EscapeUpdateBlockInteractable()
		{
			for (short index = 0; index < (short)(this._areaData.Width * this._areaData.Width); index += 1)
			{
				BuildingBlockData blockData = ViewBuildingArea.BlockList[(int)index];
				BuildingBlockItem configData = BuildingBlock.Instance[blockData.TemplateId];
				bool flag = configData == null;
				if (!flag)
				{
					BuildingAreaBlock blockRefers = this._blockRefersDict[blockData.BlockIndex];
					CImage icon = blockRefers.buildingIcon;
					bool interactable = this.BuildingBlockCanInteractable(blockData, configData);
					icon.GetComponent<CButton>().interactable = interactable;
				}
			}
		}

		// Token: 0x060098FE RID: 39166 RVA: 0x00474568 File Offset: 0x00472768
		private bool BuildingBlockCanInteractable(BuildingBlockData blockData, BuildingBlockItem configData)
		{
			bool flag = blockData.OperationType == 0 && SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !this._isStartPlanning;
				if (flag2)
				{
					bool flag3;
					if (!this._isTaiwuVillage && (!configData.CanOpenManageOutTaiwu || !this.canUseBuilding))
					{
						short templateId = configData.TemplateId;
						flag3 = (templateId == 48 || templateId == 49);
					}
					else
					{
						flag3 = true;
					}
					result = flag3;
				}
				else
				{
					bool isStartPlanning = this._isStartPlanning;
					if (isStartPlanning)
					{
						bool isTaiwuVillage = blockData.TemplateId == 44;
						result = !isTaiwuVillage;
					}
					else
					{
						bool isStartMultiplyRemove = this._isStartMultiplyRemove;
						result = (isStartMultiplyRemove && ViewBuildingArea.CanRemove(configData.TemplateId, true));
					}
				}
			}
			return result;
		}

		// Token: 0x060098FF RID: 39167 RVA: 0x00474620 File Offset: 0x00472820
		private void InitBuildingArea()
		{
			ViewBuildingArea.<>c__DisplayClass162_0 CS$<>8__locals1 = new ViewBuildingArea.<>c__DisplayClass162_0();
			CS$<>8__locals1.<>4__this = this;
			this.ClearAnimation();
			this._buildingDict.Clear();
			this._notScaleElements.Clear();
			this._customScaleElements.Clear();
			this.ClearBlockRefersDict();
			this._notScaleElements.Add(this.mouseRightCancel1);
			this._notScaleElements.Add(this.mouseRightCancel2);
			int areaWidth = (int)this._areaData.Width * 380 + (int)((this._areaData.Width - 1) * 50) + 360;
			this.buildingBlockHolder.SetSize(new Vector2((float)areaWidth, (float)areaWidth));
			this.SetRoadHolderSize(new Vector2((float)areaWidth, (float)areaWidth));
			bool flag = !this._isKeepMouseWheelScale;
			if (flag)
			{
				this._moveRoot.localScale = this.initialScale;
				this._moveRoot.pivot = new Vector2(0.5f, 0.5f);
				this._moveRoot.anchoredPosition = Vector2.zero;
			}
			else
			{
				this._isKeepMouseWheelScale = false;
			}
			this._moveRoot.GetComponent<PointerTrigger>().SetBindElement(this.Element);
			for (short index = 0; index < (short)(this._areaData.Width * this._areaData.Width); index += 1)
			{
				bool flag2 = ViewBuildingArea.BlockList[(int)index].TemplateId < 0 || (int)ViewBuildingArea.BlockList[(int)index].TemplateId > BuildingBlock.Instance.Count;
				int blockWidth;
				if (flag2)
				{
					blockWidth = 1;
				}
				else
				{
					blockWidth = (int)BuildingBlock.Instance[ViewBuildingArea.BlockList[(int)index].TemplateId].Width;
				}
				this.CreateBlock(index, blockWidth);
				this.UpdateBlock(index, true);
			}
			CS$<>8__locals1.borderRoot = this.border;
			MouseWheelScale scaler = this._moveRoot.GetComponent<MouseWheelScale>();
			UIRectDragMove dragger = this._moveRoot.GetComponent<UIRectDragMove>();
			dragger.BElastic = false;
			dragger.ElasticDuration = 0f;
			dragger.ClampType = UIRectDragMove.DragClampType.Corner;
			dragger.BeginDragCallback = delegate()
			{
				UIManager.Instance.SetEscHandler(new Action(base.<InitBuildingArea>g__CancelDrag|0));
			};
			dragger.EndDragCallback = new Action(this.SetEscHandler);
			sbyte land = this._areaData.LandFormType;
			string borderPath = string.Format("RemakeResources/Prefab/Legacy/Core/Building/Border/{0}_{1}", this._areaData.Width, land);
			ResLoader.Load<GameObject>(borderPath, delegate(GameObject prefab)
			{
				RectTransform border = Object.Instantiate<GameObject>(prefab, CS$<>8__locals1.borderRoot, false).GetComponent<RectTransform>();
				border.gameObject.SetActive(true);
				Vector2 borderSize = border.rect.size;
				CS$<>8__locals1.<>4__this._moveRoot.sizeDelta = borderSize;
				Vector2Int viewSize = AspectRatioController.ViewSize;
				float scale = Mathf.Max((float)viewSize.x / borderSize.x, (float)viewSize.y / borderSize.y);
				scaler.Min = new Vector3(scale, scale, scale);
				Vector3 currentScale = CS$<>8__locals1.<>4__this._moveRoot.localScale;
				CS$<>8__locals1.<>4__this._moveRoot.localScale = new Vector3(Mathf.Clamp(currentScale.x, scale, scaler.Max.x), Mathf.Clamp(currentScale.y, scale, scaler.Max.y), 1f);
				scaler.Reset();
			}, delegate(string path)
			{
				AdaptableLog.Warning("Cannot load block effect: (" + path + ")", false);
			}, false);
			this.SetBackGround();
			this.SetNotScaleElement();
			BuildingDomainMethod.Call.GetCanPluckFeatherChickenIds(this.Element.GameDataListenerId);
			BuildingDomainMethod.Call.GetSettlementChickenDataList(this.Element.GameDataListenerId, new Location(this._areaId, this._blockId), false);
			this.GetCurrentLocation();
			this.SetBottomInfo();
			bool flag3 = this._worldMapPlayerBlockChangeAction != null;
			if (flag3)
			{
				this._worldMapPlayerBlockChangeAction();
				this._worldMapPlayerBlockChangeAction = null;
			}
		}

		// Token: 0x06009900 RID: 39168 RVA: 0x00474974 File Offset: 0x00472B74
		private void SetEscHandler()
		{
			bool flag = this._isStartPlanning && this._isResetBuilding;
			if (flag)
			{
				UIManager.Instance.SetEscHandler(new Action(this.ExitMoveBuilding));
			}
			else
			{
				bool flag2 = this._isStartPlanning && !this._isResetBuilding;
				if (flag2)
				{
					UIManager.Instance.SetEscHandler(new Action(this.ExitBuildingPlan));
				}
				else
				{
					bool isStartMultiplyRemove = this._isStartMultiplyRemove;
					if (isStartMultiplyRemove)
					{
						UIManager.Instance.SetEscHandler(new Action(this.CancelMultiplyRemove));
					}
					else
					{
						UIManager.Instance.SetEscHandler(null);
					}
				}
			}
		}

		// Token: 0x06009901 RID: 39169 RVA: 0x00474A18 File Offset: 0x00472C18
		private void ClearBlockRefers(BuildingAreaBlock block)
		{
			bool flag = block.gameObject.name.Contains("SizeOne");
			if (flag)
			{
				this._blockListSize1.Enqueue(block);
			}
			else
			{
				this._blockListSize2.Enqueue(block);
			}
			block.transform.localPosition = this._blockListPos;
		}

		// Token: 0x06009902 RID: 39170 RVA: 0x00474A74 File Offset: 0x00472C74
		private void ClearBlockRefersDict()
		{
			this.ClearObjectPoolItems();
			foreach (BuildingAreaBlock block in this._blockRefersDict.Values)
			{
				this.ClearBlockRefers(block);
			}
			this._blockRefersDict.Clear();
		}

		// Token: 0x06009903 RID: 39171 RVA: 0x00474AE8 File Offset: 0x00472CE8
		private void ClearObjectPoolItems()
		{
			this.ClearObjectPoolItem(this._getItemPool, this._getItemDict);
			this.ClearObjectPoolItem(this._getPeoplePool, this._getPeopleDict);
			this.ClearObjectPoolItem(this._getResourcePool, this._getResourceDict);
			this.ClearObjectPoolItem(this._shopTipPool, this._shopTipDict);
			this.ClearObjectPoolItem(this._exceptionPool, this._exceptionDict);
			this.ClearObjectPoolItem(this._buildingOperatePool, this._buildingOperateDict);
			this.ClearObjectPoolItem(this._damageInfoPool, this._damageInfoDict);
			this.ClearObjectPoolItem(this._residentsInfoPool, this._residentsInfoDict);
			this.ClearObjectPoolItem(this._samsaraInfoPool, this._samsaraInfoDict);
			this.ClearObjectPoolItem(this._levelAndNamePool, this._levelAndNameDict);
			this.ClearObjectPoolItem(this._treasuryResourceInfoPool, this._treasuryResourceInfoDict);
			this.ClearObjectPoolItem(this._coreProducingCooldownPool, this._coreProducingCooldownDict);
			this.ClearObjectPoolItem(this._teaHorseCaravanPool, this._teaHorseCaravanDict);
		}

		// Token: 0x06009904 RID: 39172 RVA: 0x00474BF0 File Offset: 0x00472DF0
		private void ClearObjectPoolItem(PoolItem getResourcePool, Dictionary<short, BuildingBlockInfo> getResourceDict)
		{
			foreach (KeyValuePair<short, BuildingBlockInfo> pair in getResourceDict)
			{
				bool flag = pair.Value != null;
				if (flag)
				{
					getResourcePool.DestroyObject(pair.Value.gameObject);
				}
			}
			getResourceDict.Clear();
		}

		// Token: 0x06009905 RID: 39173 RVA: 0x00474C68 File Offset: 0x00472E68
		private void SetBottomInfo()
		{
			this.buildingAreaInfo.RefreshBuildAreaInfo(this._isTaiwuVillage, this._isSectLocation, this._isCityLocation, this._isBambooHouse, this._mapBlockData, this._areaData, ViewBuildingArea.BlockList, this._settlementId, this.settlementRoadAnimation.HaveChickenKing);
		}

		// Token: 0x06009906 RID: 39174 RVA: 0x00474CBC File Offset: 0x00472EBC
		private void SetBottomInfoWithArg(ArgumentBox argbox)
		{
			ArgumentBox args = EasyPool.Get<ArgumentBox>().Set("isTaiwuVillage", this._isTaiwuVillage).Set("isSectLocation", this._isSectLocation).Set("isCityLocation", this._isCityLocation).Set<MapBlockData>("mapBlockData", this._mapBlockData);
			GEvent.OnEvent(UiEvents.OnSetAreaSpiritualDebt, args);
		}

		// Token: 0x06009907 RID: 39175 RVA: 0x00474D24 File Offset: 0x00472F24
		private void GetCurrentLocation()
		{
			this._isSectLocation = false;
			this._isCityLocation = false;
			this.haveWarehouse = false;
			for (int i = 0; i < ViewBuildingArea.BlockList.Count; i++)
			{
				short templateId = ViewBuildingArea.BlockList[i].TemplateId;
				bool flag = templateId < 0;
				if (!flag)
				{
					bool flag2 = templateId == 257 || templateId == 258;
					if (flag2)
					{
						this._isBambooHouse = true;
					}
					bool flag3 = ViewBuildingArea.SectBuildingIdList.Contains(templateId);
					if (flag3)
					{
						this._isSectLocation = true;
					}
					bool flag4 = ViewBuildingArea.CityBuildingIdList.Contains(templateId);
					if (flag4)
					{
						this._isCityLocation = true;
					}
					bool flag5 = templateId == 48;
					if (flag5)
					{
						this.haveWarehouse = true;
					}
				}
			}
			bool flag6 = this.haveWarehouse;
			if (flag6)
			{
				GlobalDomainMethod.Call.InvokeGuidingTrigger(71);
			}
		}

		// Token: 0x06009908 RID: 39176 RVA: 0x00474E00 File Offset: 0x00473000
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string a = name;
			if (!(a == "ButtonConfirmPlanBuilding"))
			{
				if (!(a == "ButtonCancelPlanBuilding"))
				{
					if (!(a == "ButtonConfirmMultiplyRemove"))
					{
						if (a == "ButtonCancelMultiplyRemove")
						{
							this.CancelMultiplyRemove();
						}
					}
					else
					{
						this.ConfirmMultiplyRemove();
					}
				}
				else
				{
					bool flag = this._buildingPlacementData.CurBuildingIcon != null;
					if (flag)
					{
						this.CancelResetBuilding(false);
					}
					this.SwitchPlanBuildingState(false);
				}
			}
			else
			{
				bool flag2 = this._buildingPlacementData.CurBuildingIcon != null;
				if (flag2)
				{
					this.CancelResetBuilding(false);
				}
				this.SwitchPlanBuildingState(true);
			}
		}

		// Token: 0x06009909 RID: 39177 RVA: 0x00474EB4 File Offset: 0x004730B4
		private void CreateBlock(short index, int blockWidth)
		{
			BuildingAreaBlock block = null;
			bool flag = blockWidth == 1;
			if (flag)
			{
				block = this._blockListSize1.Dequeue();
			}
			else
			{
				block = this._blockListSize2.Dequeue();
			}
			block.buildingBlockIndex = index;
			CButton iconBtn = block.buildingIcon.GetComponent<CButton>();
			iconBtn.ClearAndAddListener(delegate
			{
				this.BuildingIconClick(block);
			});
			block.transform.SetSiblingIndex((int)index);
			this._blockRefersDict[index] = block;
			int xIndex = (int)(index % (short)this._areaData.Width);
			int yIndex = (int)(index / (short)this._areaData.Width);
			block.GetComponent<RectTransform>().anchoredPosition = new Vector2((float)(370 + 430 * xIndex), (float)(-(float)(370 + 430 * yIndex)));
			this._customScaleElements.Add(new ValueTuple<RectTransform, Vector2>(block.leftTopHolder, this.scaleMinMaxLeftTop));
			this._customScaleElements.Add(new ValueTuple<RectTransform, Vector2>(block.levelAndNameHolder, this.scaleMinMaxNameLabel));
			bool flag2 = blockWidth == 1;
			if (flag2)
			{
				this._notScaleElements.Add(block.getEarnHolder);
			}
		}

		// Token: 0x0600990A RID: 39178 RVA: 0x00475014 File Offset: 0x00473214
		private void BuildingIconClick(BuildingAreaBlock block)
		{
			bool isStartPlanning = this._isStartPlanning;
			if (isStartPlanning)
			{
				this._isResetBuilding = true;
				UIManager.Instance.SetEscHandler(new Action(this.ExitMoveBuilding));
				this.SetBlockToEmpty(block.buildingBlockIndex);
			}
			else
			{
				bool isStartMultiplyRemove = this._isStartMultiplyRemove;
				if (isStartMultiplyRemove)
				{
					this.ChangeMultiplyRemoveBuilding(block);
				}
				else
				{
					this.OpenBuildingManage(block.buildingBlockIndex, -1, Game.Views.Building.BuildingManage.BuildingManageTogKey.Invalid);
				}
			}
		}

		// Token: 0x0600990B RID: 39179 RVA: 0x00475080 File Offset: 0x00473280
		private void SetBuildingDescMouseTip(BuildingAreaBlock block, BuildingBlockItem config, BuildingBlockKey blockKey, BuildingBlockData blockData)
		{
			TooltipInvoker buildingDescMouseTip = block.buildingDescMouseTip;
			bool isStartPlanning = this._isStartPlanning;
			if (isStartPlanning)
			{
				buildingDescMouseTip.enabled = false;
			}
			else
			{
				sbyte level = this.BuildingModel.GetBuildingLevel(blockKey, blockData);
				TooltipInvoker tooltipInvoker = buildingDescMouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				buildingDescMouseTip.RuntimeParam.Set("IsTaiwuVillage", this._isTaiwuVillage);
				buildingDescMouseTip.RuntimeParam.Set("BuildingLevel", level);
				buildingDescMouseTip.RuntimeParam.SetObject("BuildingBlockKey", blockKey);
				buildingDescMouseTip.RuntimeParam.SetObject("BuildingBlockData", blockData);
				buildingDescMouseTip.Type = TipType.BuildingBlock;
				buildingDescMouseTip.enabled = true;
			}
		}

		// Token: 0x0600990C RID: 39180 RVA: 0x0047513C File Offset: 0x0047333C
		private void UpdateBlock(short blockIndex, bool isUpdateInfo = true)
		{
			ViewBuildingArea.<>c__DisplayClass175_0 CS$<>8__locals1 = new ViewBuildingArea.<>c__DisplayClass175_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.blockIndex = blockIndex;
			CS$<>8__locals1.blockData = ViewBuildingArea.BlockList[(int)CS$<>8__locals1.blockIndex];
			CS$<>8__locals1.buildingBlockKey = new BuildingBlockKey(this._areaId, this._blockId, CS$<>8__locals1.blockIndex);
			bool flag = CS$<>8__locals1.blockData.TemplateId > 0;
			if (flag)
			{
				this._buildingDict[CS$<>8__locals1.blockIndex] = CS$<>8__locals1.blockData;
			}
			else
			{
				this._buildingDict.Remove(CS$<>8__locals1.blockIndex);
			}
			BuildingAreaBlock blockRefers = this._blockRefersDict[CS$<>8__locals1.blockData.BlockIndex];
			CRawImage backImg = blockRefers.back;
			CImage buildImg = blockRefers.canBuild;
			GameObject holder = blockRefers.holder;
			GameObject levelNameHolder = blockRefers.levelAndNameHolder.gameObject;
			this.ManagePoolItem(this._levelAndNamePool, levelNameHolder, this._levelAndNameDict, CS$<>8__locals1.blockIndex, CS$<>8__locals1.blockData.TemplateId > 0);
			bool flag2 = CS$<>8__locals1.blockData.TemplateId > 0;
			if (flag2)
			{
				ViewBuildingArea.<>c__DisplayClass175_1 CS$<>8__locals2 = new ViewBuildingArea.<>c__DisplayClass175_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals2.config = BuildingBlock.Instance[CS$<>8__locals2.CS$<>8__locals1.blockData.TemplateId];
				SingletonObject.getInstance<YieldHelper>().StartYield(CS$<>8__locals2.<UpdateBlock>g__LevelNameRoutine|0());
			}
			bool virtualBlock = CS$<>8__locals1.blockData.RootBlockIndex >= 0;
			backImg.gameObject.SetActive(!virtualBlock);
			buildImg.gameObject.SetActive(false);
			holder.SetActive(!virtualBlock);
			bool flag3 = virtualBlock;
			if (!flag3)
			{
				BuildingBlockItem configData = BuildingBlock.Instance[CS$<>8__locals1.blockData.TemplateId];
				sbyte level = this.BuildingModel.GetBuildingLevel(CS$<>8__locals1.buildingBlockKey, CS$<>8__locals1.blockData);
				short templateId = CS$<>8__locals1.blockData.TemplateId;
				bool flag4 = templateId == 257 || templateId == 258 || this.isSecretVilliage;
				if (flag4)
				{
				}
				this.SetBackImgRandom(backImg, configData, CS$<>8__locals1.blockIndex, 1);
				backImg.GetComponent<RectTransform>().localPosition = ((configData.Width == 2) ? ViewBuildingArea.SizeTwoV2 : Vector3.zero);
				bool emptyBlock = CS$<>8__locals1.blockData.TemplateId == 0;
				if (isUpdateInfo)
				{
					holder.SetActive(!emptyBlock);
				}
				bool flag5 = emptyBlock;
				if (!flag5)
				{
					CImage icon = blockRefers.buildingIcon;
					bool interactable = this.BuildingBlockCanInteractable(CS$<>8__locals1.blockData, configData);
					icon.GetComponent<CButton>().interactable = interactable;
					ViewBuildingArea.SetBuildingIcon(icon, configData, false, null);
					this.SetBuildingAnimation(CS$<>8__locals1.blockData, icon);
					CRawImage operateImg = blockRefers.operateImg;
					operateImg.gameObject.SetActive(CS$<>8__locals1.blockData.OperationType != -1);
					icon.SetAlpha((float)((this.GetIconActive(CS$<>8__locals1.blockData, configData) && !ViewBuildingArea.IsShowAnimation(CS$<>8__locals1.blockData.TemplateId)) ? 1 : 0));
					operateImg.SetTexture(this.GetOperateImg(CS$<>8__locals1.blockData, configData));
					if (isUpdateInfo)
					{
						this.UpdateBlockInfo(CS$<>8__locals1.blockData, CS$<>8__locals1.buildingBlockKey, blockRefers, configData);
					}
					else
					{
						blockRefers.leftTopHolder.gameObject.SetActive(false);
						blockRefers.getEarnHolder.gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x0600990D RID: 39181 RVA: 0x00475498 File Offset: 0x00473698
		private string GetOperateImg(BuildingBlockData blockData, BuildingBlockItem configData)
		{
			bool flag = blockData.OperationType == -1;
			string result;
			if (flag)
			{
				result = "";
			}
			else
			{
				bool isBuilding = BuildingBlockData.IsBuilding(configData.Type);
				this.sb.Clear();
				this.sb.Append("building_block");
				string operateStr = (blockData.OperationType == 0) ? "_create" : "_remove";
				this.sb.Append(operateStr);
				bool flag2 = isBuilding;
				if (flag2)
				{
					this.sb.Append((configData.Width > 1) ? "_big" : "_small");
					result = this.sb.ToString();
				}
				else
				{
					this.sb.Append("_resource");
					result = this.sb.ToString();
				}
			}
			return result;
		}

		// Token: 0x0600990E RID: 39182 RVA: 0x00475564 File Offset: 0x00473764
		private bool GetIconActive(BuildingBlockData blockData, BuildingBlockItem configData)
		{
			bool flag = blockData.OperationType == -1;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool isBuilding = BuildingBlockData.IsBuilding(configData.Type);
				bool flag2 = isBuilding && blockData.OperationType == 1;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = !isBuilding && blockData.OperationType == 0;
					result = !flag3;
				}
			}
			return result;
		}

		// Token: 0x0600990F RID: 39183 RVA: 0x004755C4 File Offset: 0x004737C4
		private void StopOperate(BuildingBlockKey blockKey)
		{
			AudioManager.Instance.PlaySound("ui_industry_dismantle", false, false);
			BuildingDomainMethod.Call.SetStopOperation(this.Element.GameDataListenerId, blockKey, true);
		}

		// Token: 0x06009910 RID: 39184 RVA: 0x004755EC File Offset: 0x004737EC
		private void UpdateBlockInfo(BuildingBlockData blockData, BuildingBlockKey blockKey, BuildingAreaBlock block, BuildingBlockItem configData)
		{
			this.SetBuildingDescMouseTip(block, configData, blockKey, blockData);
			block.getEarnHolder.gameObject.SetActive(true);
			GameObject levelNameHolder = block.levelAndNameHolder.gameObject;
			this.ManagePoolItem(this._levelAndNamePool, levelNameHolder, this._levelAndNameDict, blockKey.BuildingBlockIndex, blockData.TemplateId > 0);
			sbyte buildingLevel = this.BuildingModel.GetBuildingLevel(blockKey, blockData);
			bool flag = blockData.TemplateId > 0 && buildingLevel > 0;
			if (flag)
			{
				BuildingBlockInfo levelAndName = this._levelAndNameDict[blockKey.BuildingBlockIndex];
				this.SetBuildingLevelText(levelAndName.levelText, blockData);
			}
			bool damageGoActive = blockData.OperationType != 0 && configData.MaxDurability > blockData.Durability;
			this.ManagePoolItem(this._damageInfoPool, block.leftTopHolder.gameObject, this._damageInfoDict, blockKey.BuildingBlockIndex, damageGoActive);
			bool flag2 = damageGoActive;
			if (flag2)
			{
				this._stringBuilder.Clear();
				this._stringBuilder.Append((int)((configData.MaxDurability - blockData.Durability) * 100 / configData.MaxDurability)).Append("%");
				BuildingBlockInfo blockInfo = this._damageInfoDict[blockKey.BuildingBlockIndex];
				blockInfo.damageText.text = this._stringBuilder.ToString();
				blockInfo.damageTip.Type = TipType.SingleDesc;
				string[] presetParam = blockInfo.damageTip.PresetParam;
				int num = 0;
				string str = LocalStringManager.Get(LanguageKey.LK_Building_Damage);
				StringBuilder stringBuilder = this._stringBuilder;
				presetParam[num] = str + ((stringBuilder != null) ? stringBuilder.ToString() : null);
			}
			bool residentsActive = (configData.TemplateId == 47 || configData.TemplateId == 46) && this._isTaiwuVillage && blockData.OperationType != 0;
			this.ManagePoolItem(this._residentsInfoPool, block.leftTopHolder.gameObject, this._residentsInfoDict, blockKey.BuildingBlockIndex, residentsActive);
			bool flag3 = configData.TemplateId == 46 && this._isTaiwuVillage;
			if (flag3)
			{
				this.UpdateResidentHouseInfo(blockKey, blockData);
			}
			bool flag4 = configData.TemplateId == 47 && this._isTaiwuVillage;
			if (flag4)
			{
				this.UpdateComfortableHouseInfo(blockKey, blockData);
			}
			bool flag5 = configData.Width == 1;
			if (flag5)
			{
				RectTransform earnHolder = block.getEarnHolder;
				bool flag6 = earnHolder != null;
				if (flag6)
				{
					earnHolder.gameObject.SetActive(configData.IsShop);
				}
			}
			bool flag7 = GameData.Domains.Building.SharedMethods.BuildingCanGetEarningData(configData);
			if (flag7)
			{
				this.UpdateShopGetItemInfo(blockKey);
			}
			GameObject parent = block.leftTopHolder.gameObject;
			bool showShopTip = configData.IsShop && this._isTaiwuVillage && blockData.CanUse();
			this.ManagePoolItem(this._shopTipPool, parent, this._shopTipDict, blockKey.BuildingBlockIndex, showShopTip);
			bool flag8 = showShopTip;
			if (flag8)
			{
				this.UpdateShopProgressInfo(blockData, blockKey);
			}
			bool flag9 = blockData.OperationType == -1;
			if (flag9)
			{
				this.ManagePoolItem(this._buildingOperatePool, parent, this._buildingOperateDict, blockKey.BuildingBlockIndex, false);
			}
			else
			{
				sbyte operationType = blockData.OperationType;
				BuildingDomainMethod.AsyncCall.GetBuildingOperationLeftTime(this, blockKey, operationType, delegate(int offset, RawDataPool dataPool)
				{
					int leftTime = 0;
					Serializer.Deserialize(dataPool, offset, ref leftTime);
					bool isMeetWorker = leftTime >= 0;
					ViewBuildingArea.<>c__DisplayClass179_2 CS$<>8__locals3;
					CS$<>8__locals3.time = "";
					bool flag13 = !isMeetWorker;
					if (flag13)
					{
						CS$<>8__locals3.time = "∞";
					}
					else
					{
						CS$<>8__locals3.time = leftTime.ToString();
					}
					base.<UpdateBlockInfo>g__UpdateBuildingOperationInfo|4(ref CS$<>8__locals3);
				});
			}
			bool flag10 = blockData.TemplateId == 50;
			if (flag10)
			{
				BuildingDomainMethod.AsyncCall.GetSamsaraPlatformCharList(this, delegate(int offset, RawDataPool dataPool)
				{
					List<SamsaraPlatformCharDisplayData> samsaraCharList = new List<SamsaraPlatformCharDisplayData>();
					Serializer.Deserialize(dataPool, offset, ref samsaraCharList);
					this.ManagePoolItem(this._samsaraInfoPool, block.leftTopHolder.gameObject, this._samsaraInfoDict, blockKey.BuildingBlockIndex, samsaraCharList.Count > 0);
					bool flag13 = samsaraCharList.Count > 0 && this._samsaraInfoDict.ContainsKey(blockKey.BuildingBlockIndex);
					if (flag13)
					{
						BuildingBlockInfo blockInfo2 = this._samsaraInfoDict[blockKey.BuildingBlockIndex];
						blockInfo2.samsaraText.SetText(samsaraCharList.Count.ToString(), true);
						this._stringBuilder.Clear();
						this._stringBuilder.AppendFormat(LocalStringManager.Get(LanguageKey.LK_Building_WaitSamsara), samsaraCharList.Count);
						blockInfo2.samsaraMouseTip.PresetParam[0] = this._stringBuilder.ToString();
					}
				});
			}
			EBuildingBlockType type = configData.Type;
			bool flag11 = type == EBuildingBlockType.NormalResource || type == EBuildingBlockType.SpecialResource;
			if (flag11)
			{
				ExtraDomainMethod.AsyncCall.GetResourceBlockProducingCoreCooldown(this, blockKey, delegate(int offset, RawDataPool pool)
				{
					int cooldown = 0;
					Serializer.Deserialize(pool, offset, ref cooldown);
					bool flag13 = cooldown > 0;
					if (flag13)
					{
						this.ManagePoolItem(this._coreProducingCooldownPool, block.leftTopHolder.gameObject, this._coreProducingCooldownDict, blockKey.BuildingBlockIndex, true);
						this._coreProducingCooldownDict[blockKey.BuildingBlockIndex].cooldownText.text = LanguageKey.LK_Building_CoreProducingCooldown.TrFormat(cooldown);
						int lineCount = 0;
						TooltipInvoker mouseTip = this._coreProducingCooldownDict[blockKey.BuildingBlockIndex].cooldownTip;
						mouseTip.Type = TipType.GeneralLines;
						TooltipInvoker tooltipInvoker = mouseTip;
						if (tooltipInvoker.RuntimeParam == null)
						{
							tooltipInvoker.RuntimeParam = new ArgumentBox();
						}
						mouseTip.RuntimeParam.Set("Title", LanguageKey.LK_MouseTip_CoreProducingCooldown_Title.Tr());
						mouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
						{
							Type = 11,
							Args = new List<string>
							{
								LanguageKey.LK_MouseTip_CoreProducingCooldown_Desc1.Tr()
							}
						});
						mouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
						{
							Type = 5,
							Args = new List<string>
							{
								LanguageKey.LK_MouseTip_CoreProducingCooldown_Desc2.TrFormat(cooldown)
							},
							ExtraArgs = new List<object>
							{
								5
							}
						});
						mouseTip.RuntimeParam.Set("LineCount", lineCount);
					}
					else
					{
						this.ManagePoolItem(this._coreProducingCooldownPool, block.leftTopHolder.gameObject, this._coreProducingCooldownDict, blockKey.BuildingBlockIndex, false);
					}
				});
			}
			else
			{
				this.ManagePoolItem(this._coreProducingCooldownPool, block.leftTopHolder.gameObject, this._coreProducingCooldownDict, blockKey.BuildingBlockIndex, false);
			}
			bool flag12 = blockData.TemplateId == 51;
			if (flag12)
			{
				BuildingDomainMethod.AsyncCall.GetTeaHorseCaravanData(this, delegate(int offset, RawDataPool pool)
				{
					TeaHorseCaravanData data = new TeaHorseCaravanData();
					Serializer.Deserialize(pool, offset, ref data);
					this.ManagePoolItem(this._teaHorseCaravanPool, block.leftTopHolder.gameObject, this._teaHorseCaravanDict, blockKey.BuildingBlockIndex, true);
					BuildingBlockInfo blockInfo2 = this._teaHorseCaravanDict[blockKey.BuildingBlockIndex];
					int cost = data.GetReplenishmentCost();
					string costStr = string.Format(" (-{0})", cost).SetColor("darkred");
					string info = data.CaravanReplenishment.ToString().SetColor("secondary") + costStr + "/" + 100.ToString().SetColor("lightgrey");
					blockInfo2.teaHorseCaravanText.SetText(info, true);
					blockInfo2.teaHorseCaravanTip.PresetParam[0] = LocalStringManager.GetFormat(LanguageKey.LK_Building_TeaHorse_Title, data.CaravanReplenishment, 100, cost);
				});
			}
			else
			{
				this.ManagePoolItem(this._teaHorseCaravanPool, block.leftTopHolder.gameObject, this._teaHorseCaravanDict, blockKey.BuildingBlockIndex, false);
			}
		}

		// Token: 0x06009911 RID: 39185 RVA: 0x00475AA8 File Offset: 0x00473CA8
		private void UpdateAllBlockInfo()
		{
			for (short index = 0; index < (short)(this._areaData.Width * this._areaData.Width); index += 1)
			{
				BuildingBlockData blockData = ViewBuildingArea.BlockList[(int)index];
				bool flag = blockData.RootBlockIndex > -1;
				if (!flag)
				{
					BuildingAreaBlock blockRefers = this._blockRefersDict[blockData.BlockIndex];
					BuildingBlockItem configData = BuildingBlock.Instance[blockData.TemplateId];
					BuildingBlockKey buildingBlockKey = new BuildingBlockKey(this._areaId, this._blockId, index);
					this.UpdateBlockInfo(blockData, buildingBlockKey, blockRefers, configData);
				}
			}
		}

		// Token: 0x06009912 RID: 39186 RVA: 0x00475B44 File Offset: 0x00473D44
		private void OnUpdateAllBlockInfo(ArgumentBox argumentBox)
		{
			bool flag = !this._isMonthChangeUpdate;
			if (flag)
			{
				this.UpdateAllBlockInfo();
			}
		}

		// Token: 0x06009913 RID: 39187 RVA: 0x00475B68 File Offset: 0x00473D68
		private void UpdateShopProgressInfo(BuildingBlockData blockData, BuildingBlockKey blockKey)
		{
			BuildingBlockInfo blockInfo = this._shopTipDict[blockKey.BuildingBlockIndex];
			BuildingBlockItem configData = BuildingBlock.Instance[blockData.TemplateId];
			bool flag = blockData.TemplateId == 105;
			if (flag)
			{
				BuildingDomainMethod.AsyncCall.GetFixBookProgress(this, blockKey, delegate(int offset, RawDataPool dataPool)
				{
					int progress = 0;
					Serializer.Deserialize(dataPool, offset, ref progress);
					progress = Math.Min(progress, 100);
					this._stringBuilder.Clear();
					this._stringBuilder.Append(progress).Append("%");
					blockInfo.shopProgressCircle.fillAmount = (float)progress / 100f;
					this._stringBuilder2.Clear();
					this._stringBuilder2.AppendFormat(LocalStringManager.Get(LanguageKey.LK_Building_ManageProgress), this._stringBuilder);
					blockInfo.shopProgressMouseTip.PresetParam[0] = this._stringBuilder2.ToString();
				});
			}
			else
			{
				this._stringBuilder.Clear();
				this._stringBuilder.Append(blockData.ShopProgressPercentage).Append("%");
				blockInfo.shopProgressCircle.fillAmount = blockData.ShopProgressFill;
				this._stringBuilder2.Clear();
				this._stringBuilder2.AppendFormat(LocalStringManager.Get(LanguageKey.LK_Building_ManageProgress), this._stringBuilder);
				blockInfo.shopProgressMouseTip.PresetParam[0] = this._stringBuilder2.ToString();
			}
			int count = this.BuildingModel.GetBuildingShopManagerCount(blockKey);
			this._stringBuilder.Clear();
			this._stringBuilder.Append(count).Append("/").Append(7);
			blockInfo.shopCountValue.SetText(this._stringBuilder);
			blockInfo.shopCountMouseTip.PresetParam[0] = LocalStringManager.GetFormat(LanguageKey.LK_Building_ShopManagerPeople, this._stringBuilder.ToString());
		}

		// Token: 0x06009914 RID: 39188 RVA: 0x00475CD0 File Offset: 0x00473ED0
		private void SetBuildingAnimation(BuildingBlockData blockData, CImage icon)
		{
			bool flag = ViewBuildingArea.IsShowAnimation(blockData.TemplateId);
			if (flag)
			{
				bool flag2 = icon.transform.childCount == 0;
				if (flag2)
				{
					ResLoader.Load<GameObject>(ViewBuildingArea.GetBuildingAnimationPrefabPath(blockData.TemplateId), delegate(GameObject obj)
					{
						GameObject go = Object.Instantiate<GameObject>(obj, icon.transform, false);
						this._animationGoList.Add(go);
					}, null, false);
				}
			}
		}

		// Token: 0x06009915 RID: 39189 RVA: 0x00475D3C File Offset: 0x00473F3C
		public void UpdateComfortableHouseInfo(BuildingBlockKey blockKey, BuildingBlockData blockData)
		{
			BuildingDomainMethod.AsyncCall.GetCharsInComfortableHouse(this, blockKey, delegate(int offset, RawDataPool dataPool)
			{
				CharacterList residentList = default(CharacterList);
				Serializer.Deserialize(dataPool, offset, ref residentList);
				List<int> list = residentList.GetCollection();
				int sum = 0;
				for (int i = 0; i < list.Count; i++)
				{
					bool flag = list[i] != -1;
					if (flag)
					{
						sum++;
					}
				}
				bool flag2 = this._residentsInfoDict.ContainsKey(blockKey.BuildingBlockIndex);
				if (flag2)
				{
					sbyte level = this.BuildingModel.GetBuildingLevel(blockKey, blockData);
					int capacity = BuildingScale.DefValue.ComfortableHouseCapacity.GetLevelEffect((int)level);
					this._stringBuilder.Clear();
					this._stringBuilder.Append(sum).Append("/").Append(capacity);
					BuildingBlockInfo blockInfo = this._residentsInfoDict[blockKey.BuildingBlockIndex];
					blockInfo.residentsText.text = this._stringBuilder.ToString();
					this._stringBuilder2.Clear();
					this._stringBuilder2.AppendFormat(LocalStringManager.Get(LanguageKey.LK_Building_ComfortableHouseNum), this._stringBuilder);
					blockInfo.residentsMouseTip.PresetParam[0] = this._stringBuilder2.ToString();
				}
			});
		}

		// Token: 0x06009916 RID: 39190 RVA: 0x00475D80 File Offset: 0x00473F80
		public void UpdateResidentHouseInfo(BuildingBlockKey blockKey, BuildingBlockData blockData)
		{
			BuildingDomainMethod.AsyncCall.GetCharsInResidence(this, blockKey, delegate(int offset, RawDataPool dataPool)
			{
				CharacterList residentList = default(CharacterList);
				Serializer.Deserialize(dataPool, offset, ref residentList);
				List<int> list = residentList.GetCollection();
				int sum = 0;
				for (int i = 0; i < list.Count; i++)
				{
					bool flag = list[i] != -1;
					if (flag)
					{
						sum++;
					}
				}
				bool flag2 = this._residentsInfoDict.ContainsKey(blockKey.BuildingBlockIndex);
				if (flag2)
				{
					sbyte buildingLevel = this.BuildingModel.GetBuildingLevel(blockKey, blockData);
					this._stringBuilder.Clear();
					this._stringBuilder.Append(sum).Append("/").Append(BuildingScale.DefValue.ResidenceCapacity.GetLevelEffect((int)buildingLevel));
					BuildingBlockInfo blockInfo = this._residentsInfoDict[blockKey.BuildingBlockIndex];
					blockInfo.residentsText.text = this._stringBuilder.ToString();
					this._stringBuilder2.Clear();
					this._stringBuilder2.AppendFormat(LocalStringManager.Get(LanguageKey.LK_Building_ResidentNum), this._stringBuilder);
					blockInfo.residentsMouseTip.PresetParam[0] = this._stringBuilder2.ToString();
				}
			});
		}

		// Token: 0x06009917 RID: 39191 RVA: 0x00475DC4 File Offset: 0x00473FC4
		public void UpdateShopGetItemInfo(BuildingBlockKey blockKey)
		{
			BuildingAreaBlock blockRefers = this._blockRefersDict[blockKey.BuildingBlockIndex];
			BuildingBlockData blockData = this._buildingDict[blockKey.BuildingBlockIndex];
			BuildingBlockItem configData = BuildingBlock.Instance[blockData.TemplateId];
			sbyte slot = GameData.Domains.Building.SharedMethods.GetBuildingSlotCount(blockData.TemplateId);
			GameObject getEarnHolder = blockRefers.getEarnHolder.gameObject;
			AsyncMethodCallbackDelegate <>9__2;
			Action <>9__1;
			Action <>9__3;
			Action <>9__4;
			Action <>9__5;
			BuildingDomainMethod.AsyncCall.GetBuildingEarningData(this, blockKey, delegate(int offset, RawDataPool dataPool)
			{
				BuildingEarningsData earningData = new BuildingEarningsData();
				Serializer.Deserialize(dataPool, offset, ref earningData);
				int count = 0;
				bool flag = earningData != null && earningData.CollectionItemList != null;
				if (flag)
				{
					count += earningData.CollectionItemList.Count;
					this.ManagePoolItem(this._getItemPool, getEarnHolder, this._getItemDict, blockKey.BuildingBlockIndex, earningData.CollectionItemList.Count > 0 && configData.TemplateId != 222);
					bool flag2 = earningData.CollectionItemList.Count > 0 && configData.TemplateId != 222;
					if (flag2)
					{
						BuildingBlockInfo itemRefers = this._getItemDict[blockKey.BuildingBlockIndex];
						CButton getItemBtn = itemRefers.getItemBtn;
						itemRefers.getItemIcon.SetSprite("ui9_icon_buildign_get_item", false, null);
						itemRefers.getItemCount.SetText(earningData.CollectionItemList.Count.ToString(), true);
						TooltipInvoker mouseTipDisplayer = getItemBtn.GetComponent<TooltipInvoker>();
						bool flag3 = mouseTipDisplayer.RuntimeParam == null;
						if (flag3)
						{
							mouseTipDisplayer.RuntimeParam = new ArgumentBox();
						}
						mouseTipDisplayer.Type = TipType.BuildingShowItem;
						mouseTipDisplayer.RuntimeParam.SetObject("itemKeys", earningData.CollectionItemList);
						mouseTipDisplayer.RuntimeParam.Set("maxCount", slot);
						CButton cbutton = getItemBtn;
						Action action;
						if ((action = <>9__1) == null)
						{
							action = (<>9__1 = delegate()
							{
								IAsyncMethodRequestHandler <>4__this = this;
								BuildingBlockKey blockKey2 = blockKey;
								AsyncMethodCallbackDelegate callback;
								if ((callback = <>9__2) == null)
								{
									callback = (<>9__2 = delegate(int offset2, RawDataPool dataPool2)
									{
										List<ItemDisplayData> itemDisplayDatas = new List<ItemDisplayData>();
										Serializer.Deserialize(dataPool2, offset2, ref itemDisplayDatas);
										ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
										argBox.SetObject("ItemList", itemDisplayDatas);
										UIElement.GetItem.SetOnInitArgs(argBox);
										UIManager.Instance.MaskUI(UIElement.GetItem);
										this.ManagePoolItem(this._getItemPool, getEarnHolder, this._getItemDict, blockKey.BuildingBlockIndex, false);
										this.UpdateAllBlockInfo();
										GEvent.OnEvent(UiEvents.OnUpdateQuickBtnState, null);
									});
								}
								BuildingDomainMethod.AsyncCall.QuickCollectSingleShopItem(<>4__this, blockKey2, callback);
							});
						}
						cbutton.ClearAndAddListener(action);
					}
				}
				else
				{
					this.ManagePoolItem(this._getItemPool, getEarnHolder, this._getItemDict, blockKey.BuildingBlockIndex, false);
				}
				bool flag4 = earningData != null && earningData.RecruitLevelList != null;
				if (flag4)
				{
					count += earningData.RecruitLevelList.Count;
					this.ManagePoolItem(this._getPeoplePool, getEarnHolder, this._getPeopleDict, blockKey.BuildingBlockIndex, earningData.RecruitLevelList.Count > 0);
					bool flag5 = earningData.RecruitLevelList.Count > 0;
					if (flag5)
					{
						BuildingBlockInfo itemRefer = this._getPeopleDict[blockKey.BuildingBlockIndex];
						CButton getPeopleBtn = itemRefer.getItemBtn;
						itemRefer.getItemIcon.SetSprite("ui9_icon_buildign_get_people", false, null);
						itemRefer.getItemCount.SetText(earningData.RecruitLevelList.Count.ToString(), true);
						TooltipInvoker mouseTipDisplayer2 = getPeopleBtn.GetComponent<TooltipInvoker>();
						bool flag6 = mouseTipDisplayer2.RuntimeParam == null;
						if (flag6)
						{
							mouseTipDisplayer2.RuntimeParam = new ArgumentBox();
						}
						mouseTipDisplayer2.Type = TipType.BuildingShowRecruitPeople;
						mouseTipDisplayer2.RuntimeParam.SetObject("charList", earningData.RecruitLevelList);
						mouseTipDisplayer2.RuntimeParam.SetObject("blockKey", blockKey);
						mouseTipDisplayer2.RuntimeParam.Set("maxCount", slot);
						CButton cbutton2 = getPeopleBtn;
						Action action2;
						if ((action2 = <>9__3) == null)
						{
							action2 = (<>9__3 = delegate()
							{
								UI_RecruitPeopleOverview.EntryFromBuildingArea(blockKey);
							});
						}
						cbutton2.ClearAndAddListener(action2);
					}
				}
				else
				{
					this.ManagePoolItem(this._getPeoplePool, getEarnHolder, this._getPeopleDict, blockKey.BuildingBlockIndex, false);
				}
				int shopSoldItemCount = 0;
				int shopSoldItemSum = 0;
				bool flag7 = earningData != null && earningData.ShopSoldItemEarnList != null;
				if (flag7)
				{
					for (int i = 0; i < earningData.ShopSoldItemEarnList.Count; i++)
					{
						bool flag8 = earningData.ShopSoldItemEarnList[i].First != -1;
						if (flag8)
						{
							count++;
							shopSoldItemCount++;
						}
					}
				}
				this.ManagePoolItem(this._getResourcePool, getEarnHolder, this._getResourceDict, blockKey.BuildingBlockIndex, shopSoldItemCount > 0);
				bool flag9 = shopSoldItemCount > 0;
				if (flag9)
				{
					BuildingBlockInfo blockInfo = this._getResourceDict[blockKey.BuildingBlockIndex];
					sbyte resourceType = 0;
					for (int j = 0; j < earningData.ShopSoldItemEarnList.Count; j++)
					{
						bool flag10 = earningData.ShopSoldItemEarnList[j].First >= 0;
						if (flag10)
						{
							shopSoldItemSum += earningData.ShopSoldItemEarnList[j].Second;
							resourceType = (sbyte)earningData.ShopSoldItemEarnList[j].First;
						}
					}
					TooltipInvoker mouseTip = blockInfo.getItemBtn.GetComponent<TooltipInvoker>();
					mouseTip.Type = TipType.SingleDesc;
					TooltipInvoker tooltipInvoker = mouseTip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					bool flag11 = resourceType == 6;
					if (flag11)
					{
						blockInfo.getItemIcon.SetSprite("ui9_icon_buildign_get_money", false, null);
						blockInfo.getItemCount.SetText(shopSoldItemSum.ToString(), true);
						mouseTip.RuntimeParam.Set("arg0", LocalStringManager.GetFormat(LanguageKey.LK_Building_ShopGetMoney, shopSoldItemSum));
					}
					else
					{
						bool flag12 = resourceType == 7;
						if (flag12)
						{
							blockInfo.getItemIcon.SetSprite("ui9_icon_buildign_get_authority", false, null);
							blockInfo.getItemCount.SetText(shopSoldItemSum.ToString(), true);
							mouseTip.RuntimeParam.Set("arg0", LocalStringManager.GetFormat(LanguageKey.LK_Building_ShopGetAuthority, shopSoldItemSum));
						}
					}
					CButton getItemBtn2 = blockInfo.getItemBtn;
					Action action3;
					if ((action3 = <>9__4) == null)
					{
						action3 = (<>9__4 = delegate()
						{
							BuildingDomainMethod.Call.QuickCollectSingleShopSoldItem(blockKey);
							this.ManagePoolItem(this._getResourcePool, getEarnHolder, this._getResourceDict, blockKey.BuildingBlockIndex, false);
							this.UpdateAllBlockInfo();
							GEvent.OnEvent(UiEvents.OnUpdateQuickBtnState, null);
						});
					}
					getItemBtn2.ClearAndAddListener(action3);
				}
				bool flag13 = earningData != null && earningData.CollectionResourceList != null;
				if (flag13)
				{
					count += earningData.CollectionResourceList.Count;
					this.ManagePoolItem(this._getResourcePool, getEarnHolder, this._getResourceDict, blockKey.BuildingBlockIndex, earningData.CollectionResourceList.Count > 0 || shopSoldItemCount > 0);
					int resourceSum = 0;
					bool flag14 = earningData.CollectionResourceList.Count > 0;
					if (flag14)
					{
						BuildingBlockInfo resourceRefers = this._getResourceDict[blockKey.BuildingBlockIndex];
						for (int k = 0; k < earningData.CollectionResourceList.Count; k++)
						{
							resourceSum += earningData.CollectionResourceList[k].Second;
						}
						TooltipInvoker mouseTip2 = resourceRefers.getItemBtn.GetComponent<TooltipInvoker>();
						mouseTip2.Type = TipType.SingleDesc;
						TooltipInvoker tooltipInvoker = mouseTip2;
						if (tooltipInvoker.RuntimeParam == null)
						{
							tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
						}
						bool flag15 = earningData.CollectionResourceList[0].First == 6;
						if (flag15)
						{
							resourceRefers.getItemIcon.SetSprite("ui9_icon_buildign_get_money", false, null);
							resourceRefers.getItemCount.SetText(resourceSum.ToString(), true);
							mouseTip2.RuntimeParam.Set("arg0", LocalStringManager.GetFormat(LanguageKey.LK_Building_ShopGetMoney, resourceSum));
						}
						else
						{
							bool flag16 = earningData.CollectionResourceList[0].First == 7;
							if (flag16)
							{
								resourceRefers.getItemIcon.SetSprite("ui9_icon_buildign_get_authority", false, null);
								resourceRefers.getItemCount.SetText(resourceSum.ToString(), true);
								mouseTip2.RuntimeParam.Set("arg0", LocalStringManager.GetFormat(LanguageKey.LK_Building_ShopGetAuthority, resourceSum));
							}
						}
						CButton getItemBtn3 = resourceRefers.getItemBtn;
						Action action4;
						if ((action4 = <>9__5) == null)
						{
							action4 = (<>9__5 = delegate()
							{
								BuildingDomainMethod.Call.QuickCollectSingleShopSoldItem(blockKey);
								this.ManagePoolItem(this._getResourcePool, getEarnHolder, this._getResourceDict, blockKey.BuildingBlockIndex, false);
								this.UpdateAllBlockInfo();
								GEvent.OnEvent(UiEvents.OnUpdateQuickBtnState, null);
							});
						}
						getItemBtn3.ClearAndAddListener(action4);
					}
				}
			});
		}

		// Token: 0x06009918 RID: 39192 RVA: 0x00475E68 File Offset: 0x00474068
		private void ManagePoolItem(PoolItem poolItem, GameObject parentObj, Dictionary<short, BuildingBlockInfo> objDict, short blockIndex, bool getOrDestroy)
		{
			if (getOrDestroy)
			{
				bool flag = !objDict.ContainsKey(blockIndex);
				if (flag)
				{
					GameObject obj = poolItem.GetObject();
					obj.transform.SetParent(parentObj.transform, false);
					obj.GetComponent<RectTransform>().localPosition = Vector3.zero;
					objDict[blockIndex] = obj.GetComponent<BuildingBlockInfo>();
				}
				else
				{
					BuildingBlockInfo refers = objDict[blockIndex];
					bool flag2 = refers != null;
					if (flag2)
					{
						refers.transform.SetParent(parentObj.transform, false);
					}
				}
			}
			else
			{
				bool flag3 = objDict.ContainsKey(blockIndex) && objDict[blockIndex] != null;
				if (flag3)
				{
					objDict[blockIndex].gameObject.SetActive(false);
					poolItem.DestroyObject(objDict[blockIndex].gameObject);
					objDict.Remove(blockIndex);
				}
			}
		}

		// Token: 0x06009919 RID: 39193 RVA: 0x00475F50 File Offset: 0x00474150
		private bool IsSectSpecialBuilding(short templateId)
		{
			return ViewBuildingArea.SectSpecialBuildingIdList.Contains(templateId);
		}

		// Token: 0x0600991A RID: 39194 RVA: 0x00475F70 File Offset: 0x00474170
		private bool IsSettlementTreasuryBuilding(short templateId)
		{
			return templateId >= 284 && templateId <= 302;
		}

		// Token: 0x0600991B RID: 39195 RVA: 0x00475F98 File Offset: 0x00474198
		private bool IsSettlementPrisonBuilding(short templateId)
		{
			return templateId >= 303 && templateId <= 317;
		}

		// Token: 0x0600991C RID: 39196 RVA: 0x00475FC0 File Offset: 0x004741C0
		private bool RenderCanBuild(short rootIndex, int areaWidth, int templateId)
		{
			BuildingBlockItem configData = BuildingBlock.Instance[templateId];
			int buildingWidth = (int)configData.Width;
			Dictionary<short, bool> dictionary;
			bool nearDependBuildings = this.NearDependBuildings(rootIndex, configData, areaWidth, out dictionary);
			int blockX = (int)rootIndex % areaWidth;
			int blockY = (int)rootIndex / areaWidth;
			int edge = areaWidth - buildingWidth + 1;
			bool flag = blockX == edge || blockY == edge;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool canBuild = nearDependBuildings;
				for (int i = blockX; i < Math.Min(blockX + buildingWidth, areaWidth); i++)
				{
					for (int j = blockY; j < Math.Min(blockY + buildingWidth, areaWidth); j++)
					{
						short index = (short)(j * areaWidth + i);
						bool flag2 = index < 0 || (int)index >= ViewBuildingArea.BlockList.Count;
						if (!flag2)
						{
							bool flag3 = !nearDependBuildings;
							if (flag3)
							{
								bool flag4 = ViewBuildingArea.BlockList[(int)index].RootBlockIndex >= 0;
								if (flag4)
								{
									this._blockRefersDict[ViewBuildingArea.BlockList[(int)index].RootBlockIndex].canBuild.color = this._invalidColor;
								}
								else
								{
									this._blockRefersDict[index].canBuild.color = this._invalidColor;
								}
							}
							else
							{
								bool flag5 = ViewBuildingArea.BlockList[(int)index].TemplateId > 0;
								if (flag5)
								{
									this._blockRefersDict[index].canBuild.color = this._invalidColor;
									canBuild = false;
								}
								else
								{
									bool flag6 = ViewBuildingArea.BlockList[(int)index].RootBlockIndex >= 0;
									if (flag6)
									{
										this._blockRefersDict[ViewBuildingArea.BlockList[(int)index].RootBlockIndex].canBuild.color = this._invalidColor;
										canBuild = false;
									}
									else
									{
										TutorialChapterModel tutorialInstance = SingletonObject.getInstance<TutorialChapterModel>();
										bool flag7 = tutorialInstance.InGuiding && tutorialInstance.TutorialChapterIndex == 1;
										if (flag7)
										{
											canBuild = (blockX == areaWidth / 2 - 1 && blockY == areaWidth / 2 - 1);
										}
									}
								}
							}
						}
					}
				}
				bool flag8 = canBuild;
				if (flag8)
				{
					bool flag9 = buildingWidth == 1;
					if (flag9)
					{
						for (int k = blockX; k < blockX + buildingWidth; k++)
						{
							for (int l = blockY; l < blockY + buildingWidth; l++)
							{
								short index2 = (short)(l * areaWidth + k);
								this._blockRefersDict[index2].canBuild.color = this._canBuildColor;
							}
						}
					}
					else
					{
						bool flag10 = buildingWidth == 2;
						if (flag10)
						{
							bool tmp = true;
							for (int m = blockX; m < blockX + buildingWidth; m++)
							{
								for (int n = blockY; n < blockY + buildingWidth; n++)
								{
									short index3 = (short)(n * areaWidth + m);
									this._blockRefersDict[index3].canBuild.color = this._canBuildColor;
									bool flag11 = tmp;
									if (flag11)
									{
										CRawImage backImg = this._blockRefersDict[index3].back;
										this.SetBackImgRandom(backImg, configData, index3, 1);
										backImg.GetComponent<RectTransform>().localPosition = new Vector3(210f, -210f, 0f);
										RectTransform buildRectTransform = this._blockRefersDict[index3].canBuild.GetComponent<RectTransform>();
										buildRectTransform.localPosition = Vector3.zero;
										buildRectTransform.sizeDelta = new Vector2(900f, 900f);
										tmp = false;
									}
									else
									{
										RectTransform rectTransform = this._blockRefersDict[index3].back.GetComponent<RectTransform>();
										rectTransform.gameObject.SetActive(false);
									}
								}
							}
						}
					}
				}
				result = canBuild;
			}
			return result;
		}

		// Token: 0x0600991D RID: 39197 RVA: 0x004763A0 File Offset: 0x004745A0
		private void UnRenderCanBuild(int rootIndex, int areaWidth, int buildingWidth)
		{
			int blockX = rootIndex % areaWidth;
			int blockY = rootIndex / areaWidth;
			for (int i = blockX; i < Math.Min(blockX + buildingWidth, areaWidth); i++)
			{
				for (int j = blockY; j < Math.Min(blockY + buildingWidth, areaWidth); j++)
				{
					short index = (short)(j * areaWidth + i);
					bool flag = !this._blockRefersDict.ContainsKey(index);
					if (!flag)
					{
						BuildingBlockData blockData = ViewBuildingArea.BlockList[(int)index];
						bool flag2 = blockData.TemplateId > 0 || blockData.RootBlockIndex >= 0;
						if (!flag2)
						{
							CImage buildImg = this._blockRefersDict[index].canBuild;
							buildImg.color = this._canBuildColor;
							buildImg.GetComponent<RectTransform>().sizeDelta = new Vector2(370f, 370f);
							CRawImage cImg = this._blockRefersDict[index].back;
							cImg.transform.localPosition = Vector3.zero;
							BuildingBlockItem config = BuildingBlock.Instance[blockData.TemplateId];
							this.SetBackImgRandom(cImg, config, index, 1);
							cImg.SetNativeSize();
							cImg.gameObject.SetActive(true);
						}
					}
				}
			}
		}

		// Token: 0x0600991E RID: 39198 RVA: 0x004764EC File Offset: 0x004746EC
		private bool BlockCanBuild(short rootIndex, int areaWidth, int buildingWidth, BuildingBlockItem configData, out Dictionary<short, bool> dependBuildingDict)
		{
			dependBuildingDict = null;
			bool flag = ViewBuildingArea.BlockList[(int)rootIndex].TemplateId != 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				int blockX = (int)rootIndex % areaWidth;
				int blockY = (int)rootIndex / areaWidth;
				int edge = areaWidth - buildingWidth + 1;
				bool flag2 = blockX == edge || blockY == edge;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool canBuild = this.NearDependBuildings(rootIndex, configData, areaWidth, out dependBuildingDict);
					bool flag3 = !canBuild;
					if (flag3)
					{
						result = false;
					}
					else
					{
						for (int i = blockX; i < Math.Min(blockX + buildingWidth, areaWidth); i++)
						{
							for (int j = blockY; j < Math.Min(blockY + buildingWidth, areaWidth); j++)
							{
								int index = j * areaWidth + i;
								bool flag4 = index < 0 || index >= ViewBuildingArea.BlockList.Count;
								if (flag4)
								{
									return false;
								}
								bool flag5 = ViewBuildingArea.BlockList[index].TemplateId > 0;
								if (flag5)
								{
									return false;
								}
								bool flag6 = ViewBuildingArea.BlockList[index].RootBlockIndex >= 0;
								if (flag6)
								{
									return false;
								}
							}
						}
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x0600991F RID: 39199 RVA: 0x00476628 File Offset: 0x00474828
		public bool CanBuildAnywhere(BuildingBlockItem configData, out Dictionary<short, bool> dependBuildingDict)
		{
			dependBuildingDict = null;
			bool flag = ViewBuildingArea.BlockList == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = configData.IsUnique && this.ContainsBuilding(configData.TemplateId, false) && configData.TemplateId != 45;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = configData.DependBuildings.Count == 0;
					if (flag3)
					{
						foreach (BuildingBlockData block in ViewBuildingArea.BlockList)
						{
							bool flag4 = this.BlockCanBuild(block.BlockIndex, (int)this._areaData.Width, (int)configData.Width, configData, out dependBuildingDict);
							if (flag4)
							{
								return true;
							}
						}
						result = false;
					}
					else
					{
						List<BuildingBlockData> neighborBlockList = EasyPool.Get<List<BuildingBlockData>>();
						foreach (BuildingBlockData building in this._buildingDict.Values)
						{
							bool flag5 = configData.DependBuildings.Contains(building.TemplateId);
							if (flag5)
							{
								this.GetNeighborBlocks(building.BlockIndex, ref neighborBlockList, BuildingBlock.Instance[building.TemplateId].Width, 2, null);
								foreach (BuildingBlockData neighborBlock in neighborBlockList)
								{
									bool flag6 = neighborBlock.TemplateId != 0 || neighborBlock.RootBlockIndex >= 0;
									if (!flag6)
									{
										bool flag7 = this.BlockCanBuild(neighborBlock.BlockIndex, (int)this._areaData.Width, (int)configData.Width, configData, out dependBuildingDict);
										if (flag7)
										{
											EasyPool.Free<List<BuildingBlockData>>(neighborBlockList);
											return true;
										}
									}
								}
							}
						}
						EasyPool.Free<List<BuildingBlockData>>(neighborBlockList);
						result = false;
					}
				}
			}
			return result;
		}

		// Token: 0x06009920 RID: 39200 RVA: 0x00476840 File Offset: 0x00474A40
		private void ResetCanBuild()
		{
			foreach (BuildingAreaBlock refers in this._blockRefersDict.Values)
			{
				refers.canBuild.gameObject.SetActive(false);
			}
		}

		// Token: 0x06009921 RID: 39201 RVA: 0x004768A8 File Offset: 0x00474AA8
		private void HighlightCanBuild(BuildingBlockItem configData)
		{
			int areaWidth = (int)this._areaData.Width;
			int buildingWidth = (int)configData.Width;
			TutorialChapterModel tutorialInstance = SingletonObject.getInstance<TutorialChapterModel>();
			bool flag = tutorialInstance.InGuiding && tutorialInstance.TutorialChapterIndex == 1;
			if (flag)
			{
				foreach (BuildingBlockData block in ViewBuildingArea.BlockList)
				{
					int blockX = (int)block.BlockIndex % areaWidth;
					int blockY = (int)block.BlockIndex / areaWidth;
					bool flag2 = (blockX == areaWidth / 2 || blockX == areaWidth / 2 - 1) && (blockY == areaWidth / 2 || blockY == areaWidth / 2 - 1);
					if (flag2)
					{
						CImage canBuildImg = this._blockRefersDict[block.BlockIndex].canBuild;
						canBuildImg.gameObject.SetActive(true);
						canBuildImg.color = this._canBuildColor;
					}
				}
			}
			else
			{
				foreach (BuildingBlockData block2 in ViewBuildingArea.BlockList)
				{
					Dictionary<short, bool> dictionary;
					bool flag3 = this.BlockCanBuild(block2.BlockIndex, areaWidth, (int)configData.Width, configData, out dictionary);
					if (flag3)
					{
						int blockX2 = (int)block2.BlockIndex % areaWidth;
						int blockY2 = (int)block2.BlockIndex / areaWidth;
						for (int i = blockX2; i < blockX2 + buildingWidth; i++)
						{
							for (int j = blockY2; j < blockY2 + buildingWidth; j++)
							{
								short index = (short)(j * areaWidth + i);
								CImage canBuildImg2 = this._blockRefersDict[index].canBuild;
								canBuildImg2.gameObject.SetActive(true);
								canBuildImg2.color = this._canBuildColor;
							}
						}
					}
				}
			}
		}

		// Token: 0x06009922 RID: 39202 RVA: 0x00476AA8 File Offset: 0x00474CA8
		private bool NearDependBuildings(short rootIndex, BuildingBlockItem configData, int areaWidth, out Dictionary<short, bool> dependBuildingDict)
		{
			dependBuildingDict = null;
			List<short> dependBuildings = configData.DependBuildings;
			bool flag = dependBuildings.Count == 0;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				List<BuildingBlockData> neighborList = new List<BuildingBlockData>();
				List<int> neighborDistanceList = EasyPool.Get<List<int>>();
				dependBuildingDict = new Dictionary<short, bool>();
				foreach (short id in dependBuildings)
				{
					dependBuildingDict[id] = false;
				}
				this.GetNeighborBlocks(rootIndex, ref neighborList, configData.Width, 2, neighborDistanceList);
				this._blockDependencyDistanceDict[(int)rootIndex] = int.MaxValue;
				for (int i = 0; i < neighborList.Count; i++)
				{
					BuildingBlockData neighborBlock = neighborList[i];
					bool flag2 = neighborBlock.RootBlockIndex >= 0;
					if (flag2)
					{
						neighborBlock = ViewBuildingArea.BlockList[(int)neighborBlock.RootBlockIndex];
					}
					bool flag3 = neighborBlock.TemplateId != 0 && neighborBlock.CanUse();
					if (flag3)
					{
						int dependIndex = dependBuildings.IndexOf(neighborBlock.TemplateId);
						bool flag4 = dependIndex >= 0;
						if (flag4)
						{
							dependBuildingDict[neighborBlock.TemplateId] = true;
							BuildingBlockItem neighborConfigData = BuildingBlock.Instance[neighborBlock.TemplateId];
							int distance = this.GetDistanceInTwoBlock(rootIndex, (int)neighborBlock.BlockIndex, (int)configData.Width, areaWidth, (int)neighborConfigData.Width);
							int currentDistance = int.MaxValue;
							this._blockDependencyDistanceDict.TryGetValue((int)rootIndex, out currentDistance);
							this._blockDependencyDistanceDict[(int)rootIndex] = Mathf.Min(distance, currentDistance);
						}
					}
				}
				EasyPool.Free<List<int>>(neighborDistanceList);
				result = dependBuildingDict.Values.All((bool v) => v);
			}
			return result;
		}

		// Token: 0x06009923 RID: 39203 RVA: 0x00476C90 File Offset: 0x00474E90
		private int GetDistanceInTwoBlock(short rootIndex, int neighborIndex, int blockWidth, int areaWidth, int neighborWidth)
		{
			return CommonUtils.GetDistanceInTwoBlock(rootIndex, neighborIndex, blockWidth, areaWidth, neighborWidth);
		}

		// Token: 0x06009924 RID: 39204 RVA: 0x00476CB0 File Offset: 0x00474EB0
		public void StartPlacingBuilding(BuildingBlockItem item, int[] operatorList, sbyte level = 1, bool instantBuild = false)
		{
			this.isPlacingBuildingNow = true;
			this._buildingPlacementData.CurBuildingBlockItem = item;
			this._buildingPlacementData.CurBuildingIcon = ((item.Width == 1) ? this.buildingIconPrefabSize1 : this.buildingIconPrefabSize2);
			CImage icon = this._buildingPlacementData.CurBuildingIcon;
			icon.transform.Find("MouseRightCancel").gameObject.SetActive(false);
			ViewBuildingArea.SetBuildingIcon(icon, item, false, null);
			icon.gameObject.SetActive(true);
			Vector2 sizeDelta = icon.rectTransform.sizeDelta;
			float offset = sizeDelta.x / 2f - sizeDelta.x / (float)(item.Width * 2);
			this._buildingPlacementData.Offset = new Vector2(-offset, offset);
			this._buildingPlacementData.OperatorList = operatorList;
			this._buildingPlacementData.Level = level;
			this._buildingPlacementData.InstantBuild = instantBuild;
			this.HighlightCanBuild(item);
			this.ForbiddenSelectTip(true);
		}

		// Token: 0x06009925 RID: 39205 RVA: 0x00476DA4 File Offset: 0x00474FA4
		private void ForbiddenSelectTip(bool isForbidden)
		{
			foreach (KeyValuePair<short, BuildingAreaBlock> pair in this._blockRefersDict)
			{
				pair.Value.pointerTrigger.enabled = !isForbidden;
			}
		}

		// Token: 0x06009926 RID: 39206 RVA: 0x00476E0C File Offset: 0x0047500C
		private void OnTryBuildQiwenxingtai(ArgumentBox argbox)
		{
			BuildingBlockItem config = BuildingBlock.Instance[275];
			this.StartPlacingBuilding(config, null, 1, true);
		}

		// Token: 0x06009927 RID: 39207 RVA: 0x00476E35 File Offset: 0x00475035
		public void StartPlacingBuildingWithName(BuildingBlockItem item, int[] operatorList, string customName)
		{
			this._customName = customName;
			this.StartPlacingBuilding(item, operatorList, 1, false);
		}

		// Token: 0x06009928 RID: 39208 RVA: 0x00476E4C File Offset: 0x0047504C
		private void UpdatePlacingBuilding()
		{
			Transform iconTrans = this._buildingPlacementData.CurBuildingIcon.transform;
			Vector3 pos = UIManager.Instance.UiCamera.ScreenToWorldPoint(Input.mousePosition);
			iconTrans.position = new Vector3(pos.x, pos.y);
			short index = this.GetPositionIndex();
			bool flag = index == this._buildingPlacementData.CurIndex;
			if (!flag)
			{
				this.UnRenderCanBuild((int)this._buildingPlacementData.CurIndex, (int)this._areaData.Width, (int)this._buildingPlacementData.CurBuildingBlockItem.Width);
				this._buildingPlacementData.CurIndex = index;
				bool flag2 = index < 0 || (int)index >= ViewBuildingArea.BlockList.Count;
				if (flag2)
				{
					this._buildingPlacementData.CanBuild = false;
				}
				else
				{
					this._buildingPlacementData.CanBuild = this.RenderCanBuild(index, (int)this._areaData.Width, (int)this._buildingPlacementData.CurBuildingBlockItem.TemplateId);
				}
			}
		}

		// Token: 0x06009929 RID: 39209 RVA: 0x00476F48 File Offset: 0x00475148
		private void PlaceBuildingInputHandle()
		{
			bool flag = Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape);
			if (flag)
			{
				this.CancelPlaceBuilding(true);
			}
			else
			{
				bool flag2 = this._buildingPlacementData.CurIndex < 0;
				if (!flag2)
				{
					bool flag3 = Input.GetMouseButtonUp(0) && !this._dialogIsOpen;
					if (flag3)
					{
						this.HandlePlaceBuildingConfirmDialog();
					}
					else
					{
						bool mouseButtonDown = Input.GetMouseButtonDown(0);
						if (mouseButtonDown)
						{
							this._buildingPlacementData.MouseClickPos = Input.mousePosition;
						}
					}
				}
			}
		}

		// Token: 0x0600992A RID: 39210 RVA: 0x00476FCC File Offset: 0x004751CC
		private void HandlePlaceBuildingConfirmDialog()
		{
			BuildingBlockItem config = BuildingBlock.Instance[this._buildingPlacementData.CurBuildingBlockItem.TemplateId];
			this.HandlePlaceBuildingConfirm(false);
		}

		// Token: 0x0600992B RID: 39211 RVA: 0x00477000 File Offset: 0x00475200
		private bool PlaceUnusefulCheck()
		{
			return (Input.mousePosition - this._buildingPlacementData.MouseClickPos).magnitude >= 5f;
		}

		// Token: 0x0600992C RID: 39212 RVA: 0x0047703C File Offset: 0x0047523C
		private void HandlePlaceBuildingConfirm(bool showConfirmDialog = false)
		{
			bool flag = !showConfirmDialog;
			if (flag)
			{
				bool flag2 = this.PlaceUnusefulCheck();
				if (flag2)
				{
					return;
				}
			}
			bool flag3 = !this._buildingPlacementData.CanBuild;
			if (!flag3)
			{
				short currentIndex = this._buildingPlacementData.CurIndex;
				bool flag4 = this.ShowDependencyTips(this._buildingPlacementData.CurBuildingBlockItem, currentIndex);
				if (flag4)
				{
					DialogCmd cmd = new DialogCmd
					{
						Title = LocalStringManager.Get(LanguageKey.LK_Building_Start_Build),
						Content = LocalStringManager.Get(LanguageKey.LK_Confirm_Build_Tip_Distance).ColorReplace(),
						Yes = delegate()
						{
							this.ConfirmBuildPlace();
						}
					};
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
					UIManager.Instance.MaskUI(UIElement.Dialog);
				}
				else
				{
					this.ConfirmBuildPlace();
				}
			}
		}

		// Token: 0x0600992D RID: 39213 RVA: 0x00477118 File Offset: 0x00475318
		private bool ShowDependencyTips(BuildingBlockItem buildingBlockItem, short curIndex)
		{
			ShopEventItem shopEvent = buildingBlockItem.SuccesEvent.CheckIndex(0) ? ShopEvent.Instance.GetItem(buildingBlockItem.SuccesEvent[0]) : null;
			bool isAcceptedSubEffectByDistance = buildingBlockItem.IsCollectResourceBuilding || GameData.Domains.Building.SharedMethods.IsBuildingProduceMoneyAuthority(buildingBlockItem, shopEvent) || GameData.Domains.Building.SharedMethods.IsBuildingSoldItem(buildingBlockItem, shopEvent);
			return isAcceptedSubEffectByDistance && buildingBlockItem.DependBuildings.Count > 0 && this._blockDependencyDistanceDict.ContainsKey((int)curIndex) && this._blockDependencyDistanceDict[(int)curIndex] >= 2;
		}

		// Token: 0x0600992E RID: 39214 RVA: 0x004771A4 File Offset: 0x004753A4
		private void ConfirmBuildPlace()
		{
			TutorialChapterModel tutorialInstance = SingletonObject.getInstance<TutorialChapterModel>();
			bool flag = tutorialInstance.InGuiding && tutorialInstance.TutorialChapterIndex == 1;
			if (flag)
			{
				int areaWidth = (int)this._areaData.Width;
				int blockX = (int)this._buildingPlacementData.CurIndex % areaWidth;
				int blockY = (int)this._buildingPlacementData.CurIndex / areaWidth;
				bool flag2 = blockX != areaWidth / 2 - 1 || blockY != areaWidth / 2 - 1;
				if (flag2)
				{
					return;
				}
			}
			this.UnRenderCanBuild((int)this._buildingPlacementData.CurIndex, (int)this._areaData.Width, (int)this._buildingPlacementData.CurBuildingBlockItem.Width);
			this._buildingPlacementData.CurBuildingIcon.gameObject.SetActive(false);
			this.ConfirmBuild();
			this.isPlacingBuildingNow = false;
			this.ForbiddenSelectTip(false);
		}

		// Token: 0x0600992F RID: 39215 RVA: 0x00477274 File Offset: 0x00475474
		public void CancelPlaceBuilding(bool isMouseCancel)
		{
			this.UnRenderCanBuild((int)this._buildingPlacementData.CurIndex, (int)this._areaData.Width, (int)this._buildingPlacementData.CurBuildingBlockItem.Width);
			this.ResetCanBuild();
			this.isPlacingBuildingNow = false;
			bool flag = isMouseCancel && !this._buildingPlacementData.InstantBuild;
			if (flag)
			{
				UIElement.BuildingOverview.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("isHaveChickenKing", this.settlementRoadAnimation.HaveChickenKing));
				UIManager.Instance.ShowUI(UIElement.BuildingOverview, true);
			}
			this._buildingPlacementData.Clear();
			this.ForbiddenSelectTip(false);
		}

		// Token: 0x06009930 RID: 39216 RVA: 0x00477321 File Offset: 0x00475521
		public void Rename()
		{
			BuildingDomainMethod.Call.SetBuildingCustomName(this._placeBlockKey, this._customName);
		}

		// Token: 0x06009931 RID: 39217 RVA: 0x00477338 File Offset: 0x00475538
		private void ConfirmBuild()
		{
			AudioManager.Instance.PlaySound("ui_industry_put", false, false);
			BuildingBlockKey buildingBlockKey = new BuildingBlockKey(this._areaId, this._blockId, this._buildingPlacementData.CurIndex);
			this._placeBlockKey = buildingBlockKey;
			bool flag = this._buildingPlacementData.CurBuildingBlockItem.TemplateId == 275;
			if (flag)
			{
				ExtraDomainMethod.Call.BuildExtraLegacyBuilding(buildingBlockKey);
				BuildingDomainMethod.Call.GetBuildingBlockList(this.Element.GameDataListenerId, new Location(this._areaId, this._blockId));
				this.UpdateAllBlockInfo();
			}
			else
			{
				bool instantBuild = this._buildingPlacementData.InstantBuild;
				if (instantBuild)
				{
					BuildingDomainMethod.Call.GmCmd_BuildImmediately(this.Element.GameDataListenerId, this._buildingPlacementData.CurBuildingBlockItem.TemplateId, buildingBlockKey, this._buildingPlacementData.Level);
				}
				else
				{
					BuildingDomainMethod.Call.Build(this.Element.GameDataListenerId, buildingBlockKey, this._buildingPlacementData.CurBuildingBlockItem.TemplateId, this._buildingPlacementData.OperatorList);
					bool inGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
					if (inGuiding)
					{
						TaiwuEventDomainMethod.Call.TriggerListener(EventActionKey.DefValue.TutorialPlaceBuilding, false);
						bool flag2 = this._buildingPlacementData.CurBuildingBlockItem.TemplateId == 258;
						if (flag2)
						{
							SingletonObject.getInstance<TutorialChapterModel>().BambooHouseStartBuildFlag = true;
						}
					}
				}
			}
			this.ResetCanBuild();
			this._buildingPlacementData.Clear();
			this.Rename();
		}

		// Token: 0x06009932 RID: 39218 RVA: 0x004774A4 File Offset: 0x004756A4
		private short GetPositionIndex()
		{
			Vector3 initPos = this._blockRefersDict[0].transform.localPosition;
			Vector3 curPos = this._buildingPlacementData.CurBuildingIcon.rectTransform.anchoredPosition + this._buildingPlacementData.Offset;
			int col = Convert.ToInt32((curPos.x - initPos.x) / 430f);
			int row = Convert.ToInt32((initPos.y - curPos.y) / 430f);
			bool flag = col < 0 || col >= (int)this._areaData.Width || row < 0 || row >= (int)this._areaData.Width;
			short result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				result = (short)(row * (int)this._areaData.Width + col);
			}
			return result;
		}

		// Token: 0x06009933 RID: 39219 RVA: 0x00477574 File Offset: 0x00475774
		private void OpenBuildingManage(short blockIndex, short templateId = -1, Game.Views.Building.BuildingManage.BuildingManageTogKey tabKey = Game.Views.Building.BuildingManage.BuildingManageTogKey.Invalid)
		{
			bool blockHotKey = UIManager.Instance.BlockHotKey;
			if (!blockHotKey)
			{
				bool flag = templateId == -1;
				if (flag)
				{
					templateId = ViewBuildingArea.BlockList[(int)blockIndex].TemplateId;
				}
				bool needOpenWarehouse = !this._isTaiwuVillage && templateId == 48;
				bool flag2 = needOpenWarehouse;
				if (flag2)
				{
					UIManager.Instance.ShowUI(UIElement.Warehouse, true);
				}
				else
				{
					bool flag3 = this.IsSectSpecialBuilding(templateId);
					if (flag3)
					{
						TaiwuEventDomainMethod.Call.OnSectSpecialBuildingClicked(templateId);
					}
					else
					{
						bool flag4 = this.IsSettlementTreasuryBuilding(templateId);
						if (flag4)
						{
							TaiwuEventDomainMethod.Call.SettlementTreasuryBuildingClicked(templateId, 0, 0);
						}
						else
						{
							bool flag5 = templateId == 275;
							if (flag5)
							{
								bool flag6 = ViewBuildingArea.HasBuilding(251, false);
								if (flag6)
								{
									OrganizationDomainMethod.AsyncCall.GetSectFunctionStatus(this, 13, SectFunctionStatuses.SectFunctionStatusType.SpecialInteractionUnlocked, delegate(int offset, RawDataPool pool)
									{
										bool unlocked = false;
										Serializer.Deserialize(pool, offset, ref unlocked);
										bool flag12 = !unlocked;
										if (!flag12)
										{
											UIElement dialog = UIElement.Dialog;
											ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
											string key = "Cmd";
											DialogCmd dialogCmd = new DialogCmd();
											dialogCmd.Type = 2;
											dialogCmd.Title = LanguageKey.LK_Building_RemoveQiWenXingtai_Title_Jieqing.Tr();
											dialogCmd.Content = LanguageKey.LK_Building_RemoveQiWenXingtai_Desc_Jieqing.Tr();
											dialogCmd.Yes = delegate()
											{
											};
											dialog.SetOnInitArgs(argumentBox.SetObject(key, dialogCmd));
											UIManager.Instance.MaskUI(UIElement.Dialog);
										}
									});
								}
								else
								{
									bool flag7 = this._rootLocation.IsValid();
									if (flag7)
									{
										SettlementInfo settlement = SingletonObject.getInstance<WorldMapModel>().GetLocationOrganizationInfo(this._rootLocation);
										sbyte orgTemplateId = settlement.OrgTemplateId;
										bool flag8 = orgTemplateId <= 15 && orgTemplateId >= 1;
										if (flag8)
										{
											BuildingBlockItem configData = BuildingBlock.Instance[templateId];
											BuildingBlockKey blockKey;
											BuildingBlockData blockData;
											bool isGet = SingletonObject.getInstance<BuildingModel>().GetBuilding(templateId, out blockKey, out blockData);
											bool flag9 = isGet;
											if (flag9)
											{
												sbyte level = SingletonObject.getInstance<BuildingModel>().GetBuildingLevel(blockKey, blockData);
												List<GainInfo> gainList = new List<GainInfo>();
												sbyte resType = 0;
												while ((int)resType < configData.BaseBuildCost.Length)
												{
													int amount = GameData.Domains.Building.SharedMethods.GetResourceReturnOfRemoveBuilding(configData, level, resType, blockData);
													bool flag10 = amount <= 0;
													if (!flag10)
													{
														gainList.Add(new GainInfo
														{
															Type = (EConfirmDialogCostType)resType,
															Value = amount
														});
													}
													resType += 1;
												}
												ConfirmDialogCmd cmd = new ConfirmDialogCmd
												{
													ValueStyle = 2,
													Title = LocalStringManager.Get(LanguageKey.LK_Building_RemoveQiWenXingtai_Title),
													ContentUpper = LocalStringManager.Get(LanguageKey.LK_Building_GetResourceReturn),
													ContentLower = LocalStringManager.Get(LanguageKey.LK_Building_RemoveQiWenXingtai_Desc),
													GainInfos = gainList,
													Yes = delegate()
													{
														ExtraDomainMethod.Call.RemoveSectExtraLegacyBuilding(settlement.OrgTemplateId);
														ViewBuildingArea.BlockList[(int)blockIndex] = new BuildingBlockData(blockIndex, 0, -1, -1);
														this.UpdateBlock(blockIndex, true);
														this.UpdateRoad();
													}
												};
												UIElement.ConfirmDialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
												UIManager.Instance.MaskUI(UIElement.ConfirmDialog);
											}
										}
									}
								}
							}
							else
							{
								bool flag11 = this.IsSettlementPrisonBuilding(templateId);
								if (flag11)
								{
									BuildingBlockItem config = BuildingBlock.Instance[templateId];
									OrganizationDomainMethod.AsyncCall.IsTaiwuSectFugitive(this, config.BelongOrganization, delegate(int offset, RawDataPool dataPool)
									{
										bool isPrisoner = false;
										Serializer.Deserialize(dataPool, offset, ref isPrisoner);
										bool flag12 = isPrisoner;
										if (flag12)
										{
											TaiwuEventDomainMethod.Call.OnSectBuildingClicked(templateId);
										}
										else
										{
											this.ShowBuildingManage(blockIndex, templateId, tabKey);
										}
									});
								}
								else
								{
									this.ShowBuildingManage(blockIndex, templateId, tabKey);
									base.DelayCall(delegate
									{
										TaiwuEventDomainMethod.Call.OnSectBuildingClicked(templateId);
									}, 1f);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06009934 RID: 39220 RVA: 0x004778D8 File Offset: 0x00475AD8
		private void OpenBuildingManage(ArgumentBox argsBox)
		{
			BuildingBlockKey buildingKey;
			argsBox.Get<BuildingBlockKey>("buildingKey", out buildingKey);
			Enum enumValue;
			Game.Views.Building.BuildingManage.BuildingManageTogKey tabKey = argsBox.Get("tabKey", out enumValue) ? ((Game.Views.Building.BuildingManage.BuildingManageTogKey)enumValue) : Game.Views.Building.BuildingManage.BuildingManageTogKey.Invalid;
			short blockIndex = buildingKey.BuildingBlockIndex;
			BuildingBlockData blockData = ViewBuildingArea.BlockList[(int)blockIndex];
			short templateId = blockData.TemplateId;
			this.ShowBuildingManage(blockIndex, templateId, tabKey);
		}

		// Token: 0x06009935 RID: 39221 RVA: 0x00477938 File Offset: 0x00475B38
		private void ShowBuildingQuickActionMenu(short blockIndex)
		{
			BuildingBlockData blockData = ViewBuildingArea.BlockList[(int)blockIndex];
			bool isTaiwuVillageBuilding = this._areaId == SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageAreaId();
			this.CheckAndAdjustCameraForQuickActionMenu(blockIndex);
			RectTransform backRectTransform = null;
			BuildingAreaBlock blockRefers;
			bool flag = this._blockRefersDict.TryGetValue(blockIndex, out blockRefers);
			if (flag)
			{
				CRawImage backImage = blockRefers.back;
				bool flag2 = backImage != null;
				if (flag2)
				{
					backRectTransform = backImage.GetComponent<RectTransform>();
				}
			}
			MouseWheelScale scaler = this._moveRoot.GetComponent<MouseWheelScale>();
			float currentScale = (scaler != null) ? scaler.transform.localScale.x : 1f;
			UIElement.BuildingQuickActionMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("AreaId", this._areaId).Set("BlockId", this._blockId).Set("BuildingBlockIndex", blockIndex).SetObject("BuildingBlockData", blockData).Set("IsTaiwuVillageBuilding", isTaiwuVillageBuilding).SetObject("BackRectTransform", backRectTransform).Set("CameraMoveDuration", this.quickActionMenuCameraMoveDuration).Set("CurrentScale", currentScale).Set<BuildingAreaData>("BuildingAreaData", this._areaData));
			UIManager.Instance.ShowUI(UIElement.BuildingQuickActionMenu, true);
		}

		// Token: 0x06009936 RID: 39222 RVA: 0x00477A70 File Offset: 0x00475C70
		private void ShowBuildingManage(short blockIndex, short templateId, Game.Views.Building.BuildingManage.BuildingManageTogKey tabKey = Game.Views.Building.BuildingManage.BuildingManageTogKey.Invalid)
		{
			ArgumentBox argsBox = EasyPool.Get<ArgumentBox>();
			argsBox.SetObject("MapBlockData", this._mapBlockData);
			argsBox.SetObject("BuildingBlockData", ViewBuildingArea.BlockList[(int)blockIndex]);
			argsBox.Set<BuildingAreaData>("BuildingAreaData", this._areaData);
			argsBox.Set("tabKey", tabKey);
			UIElement.BuildingManage.SetOnInitArgs(argsBox);
			UIManager.Instance.ShowUI(UIElement.BuildingManage, true);
			bool key = Input.GetKey(KeyCode.LeftControl);
			if (key)
			{
				argsBox = EasyPool.Get<ArgumentBox>();
				argsBox.Set("AreaId", this._areaId);
				argsBox.Set("BlockId", this._blockId);
				argsBox.Set("BuildingBlockIndex", blockIndex);
				argsBox.SetObject("BuildingBlockData", ViewBuildingArea.BlockList[(int)blockIndex]);
				argsBox.Set<BuildingAreaData>("BuildingAreaData", this._areaData);
				argsBox.Set("tabKey", tabKey);
				UIElement.BuildingManageOld.SetOnInitArgs(argsBox);
				UIManager.Instance.ShowUI(UIElement.BuildingManageOld, true);
			}
		}

		// Token: 0x06009937 RID: 39223 RVA: 0x00477B94 File Offset: 0x00475D94
		public void MoveCameraToBuilding(short blockIndex, Action onComplete = null)
		{
			BuildingAreaBlock blockRefers;
			bool flag = this._blockRefersDict.TryGetValue(blockIndex, out blockRefers);
			if (flag)
			{
				RectTransform buildingRectTransform = blockRefers.GetComponent<RectTransform>();
				Rect holderRect = this.buildingBlockHolder.rect;
				Vector2 buildingPosFromHolderTopLeft = buildingRectTransform.anchoredPosition;
				Vector2 holderCenterFromTopLeft = new Vector2(holderRect.width * 0.5f, -holderRect.height * 0.5f);
				BuildingBlockData blockData = ViewBuildingArea.BlockList[(int)blockIndex];
				BuildingBlockItem configData = BuildingBlock.Instance[blockData.TemplateId];
				sbyte buildingWidth = (configData != null) ? configData.Width : 1;
				Vector2 buildingVisualCenterOffset = Vector2.zero;
				bool flag2 = buildingWidth == 2;
				if (flag2)
				{
					buildingVisualCenterOffset = new Vector2(215f, -215f);
				}
				Vector2 buildingPosFromHolderCenter = buildingPosFromHolderTopLeft + buildingVisualCenterOffset - holderCenterFromTopLeft;
				float zoomScale = (buildingWidth == 2) ? this.cameraZoomScaleForSize2 : this.cameraZoomScale;
				Vector3 targetScale = new Vector3(zoomScale, zoomScale, 1f);
				Vector2 targetPos = new Vector2((-buildingPosFromHolderCenter.x + this.cameraOffsetX / zoomScale) * zoomScale, (-buildingPosFromHolderCenter.y + this.cameraOffsetY / zoomScale) * zoomScale);
				this._savedMoveRootScale = this._moveRoot.localScale;
				this._savedMoveRootAnchoredPos = this._moveRoot.anchoredPosition;
				UIRectDragMove dragger = this._moveRoot.GetComponent<UIRectDragMove>();
				MouseWheelScale scaler = this._moveRoot.GetComponent<MouseWheelScale>();
				UIRectMouseMove mouseMove = this._moveRoot.GetComponent<UIRectMouseMove>();
				this._savedDragEnabled = (dragger != null && dragger.enabled);
				this._savedMouseWheelEnabled = (scaler != null && scaler.enabled);
				this._savedMouseMoveEnabled = (mouseMove != null && mouseMove.enabled);
				this._hasSavedCameraState = true;
				bool flag3 = dragger != null;
				if (flag3)
				{
					dragger.enabled = false;
				}
				bool flag4 = scaler != null;
				if (flag4)
				{
					scaler.enabled = false;
				}
				bool flag5 = mouseMove != null;
				if (flag5)
				{
					mouseMove.enabled = false;
				}
				bool flag6 = this.cameraMoveDuration <= 0.01f;
				if (flag6)
				{
					this._moveRoot.localScale = targetScale;
					this._moveRoot.anchoredPosition = targetPos;
					this._isCameraMoving = false;
					Action onComplete2 = onComplete;
					if (onComplete2 != null)
					{
						onComplete2();
					}
				}
				else
				{
					this._isCameraMoving = true;
					this._moveRoot.DOKill(false);
					this._moveRoot.DOScale(targetScale, this.cameraMoveDuration).SetEase(Ease.OutQuad);
					this._moveRoot.DOAnchorPos(targetPos, this.cameraMoveDuration, false).SetEase(Ease.OutQuad).OnComplete(delegate
					{
						this._isCameraMoving = false;
						Action onComplete4 = onComplete;
						if (onComplete4 != null)
						{
							onComplete4();
						}
					});
				}
			}
			else
			{
				Action onComplete3 = onComplete;
				if (onComplete3 != null)
				{
					onComplete3();
				}
			}
		}

		// Token: 0x06009938 RID: 39224 RVA: 0x00477E6C File Offset: 0x0047606C
		private void OnBuildingManageClosed(ArgumentBox argBox)
		{
			this.OnBuildingManageClosed();
		}

		// Token: 0x06009939 RID: 39225 RVA: 0x00477E78 File Offset: 0x00476078
		public void OnBuildingManageClosed()
		{
			bool hasSavedCameraState = this._hasSavedCameraState;
			if (hasSavedCameraState)
			{
				UIRectDragMove dragger = this._moveRoot.GetComponent<UIRectDragMove>();
				MouseWheelScale scaler = this._moveRoot.GetComponent<MouseWheelScale>();
				UIRectMouseMove mouseMove = this._moveRoot.GetComponent<UIRectMouseMove>();
				bool flag = this.cameraMoveDuration <= 0.01f;
				if (flag)
				{
					this._moveRoot.localScale = this._savedMoveRootScale;
					this._moveRoot.anchoredPosition = this._savedMoveRootAnchoredPos;
					bool flag2 = dragger != null;
					if (flag2)
					{
						dragger.enabled = this._savedDragEnabled;
					}
					bool flag3 = scaler != null;
					if (flag3)
					{
						scaler.enabled = this._savedMouseWheelEnabled;
					}
					bool flag4 = mouseMove != null;
					if (flag4)
					{
						mouseMove.enabled = this._savedMouseMoveEnabled;
					}
					bool flag5 = dragger != null;
					if (flag5)
					{
						dragger.SetDirty();
					}
					this._hasSavedCameraState = false;
					this._isCameraMoving = false;
				}
				else
				{
					this._isCameraMoving = true;
					this._moveRoot.DOKill(false);
					this._moveRoot.DOScale(this._savedMoveRootScale, this.cameraMoveDuration).SetEase(Ease.OutQuad);
					this._moveRoot.DOAnchorPos(this._savedMoveRootAnchoredPos, this.cameraMoveDuration, false).SetEase(Ease.OutQuad).OnComplete(delegate
					{
						bool flag7 = dragger != null;
						if (flag7)
						{
							dragger.enabled = this._savedDragEnabled;
						}
						bool flag8 = scaler != null;
						if (flag8)
						{
							scaler.enabled = this._savedMouseWheelEnabled;
						}
						bool flag9 = mouseMove != null;
						if (flag9)
						{
							mouseMove.enabled = this._savedMouseMoveEnabled;
						}
						bool flag10 = dragger != null;
						if (flag10)
						{
							dragger.SetDirty();
						}
						this._hasSavedCameraState = false;
						this._isCameraMoving = false;
					});
				}
			}
			else
			{
				UIRectDragMove fallbackDragger = this._moveRoot.GetComponent<UIRectDragMove>();
				bool flag6 = fallbackDragger != null;
				if (flag6)
				{
					fallbackDragger.SetDirty();
				}
			}
		}

		// Token: 0x0600993A RID: 39226 RVA: 0x00478034 File Offset: 0x00476234
		public void EnableRectDragMove()
		{
			UIRectDragMove dragger = this._moveRoot.GetComponent<UIRectDragMove>();
			MouseWheelScale scaler = this._moveRoot.GetComponent<MouseWheelScale>();
			UIRectMouseMove mouseMove = this._moveRoot.GetComponent<UIRectMouseMove>();
			bool flag = dragger;
			if (flag)
			{
				dragger.enabled = true;
			}
			bool flag2 = scaler;
			if (flag2)
			{
				scaler.enabled = true;
			}
			bool flag3 = mouseMove;
			if (flag3)
			{
				mouseMove.enabled = true;
			}
		}

		// Token: 0x0600993B RID: 39227 RVA: 0x004780A0 File Offset: 0x004762A0
		public void MoveCameraCenterToBuilding(short blockIndex)
		{
			BuildingAreaBlock blockRefers;
			bool flag = !this._blockRefersDict.TryGetValue(blockIndex, out blockRefers);
			if (!flag)
			{
				RectTransform buildingRectTransform = blockRefers.GetComponent<RectTransform>();
				RectTransform moveAndScaleRootRect = this._moveRoot.GetComponent<RectTransform>();
				Vector3 localPosition = moveAndScaleRootRect.InverseTransformPoint(buildingRectTransform.position);
				Vector2 startPos = moveAndScaleRootRect.anchoredPosition;
				Vector2 endPos = new Vector2(-localPosition.x, -localPosition.y);
				DOVirtual.Float(0f, 1f, this.cameraMoveDuration, delegate(float stepVal)
				{
					moveAndScaleRootRect.anchoredPosition = Vector2.Lerp(startPos, moveAndScaleRootRect.localScale.x * endPos, stepVal);
				});
			}
		}

		// Token: 0x0600993C RID: 39228 RVA: 0x00478144 File Offset: 0x00476344
		private void OnBuildingBlockUpdate(ArgumentBox argBox)
		{
			BuildingBlockKey blockKey;
			argBox.Get<BuildingBlockKey>("BuildingBlockKey", out blockKey);
			bool isMonthChangeUpdate = this._isMonthChangeUpdate;
			if (!isMonthChangeUpdate)
			{
				bool flag = blockKey.AreaId == this._areaId && blockKey.BlockId == this._blockId;
				if (flag)
				{
					this.UpdateBlock(blockKey.BuildingBlockIndex, true);
				}
			}
		}

		// Token: 0x0600993D RID: 39229 RVA: 0x004781A0 File Offset: 0x004763A0
		private void OnBuildingCustomNameChange(ArgumentBox argBox)
		{
			BuildingBlockKey blockKey;
			argBox.Get<BuildingBlockKey>("BuildingBlockKey", out blockKey);
			bool flag = blockKey.AreaId == this._areaId && blockKey.BlockId == this._blockId;
			if (flag)
			{
				BuildingBlockData blockData = ViewBuildingArea.BlockList[(int)blockKey.BuildingBlockIndex];
				BuildingAreaBlock blockRefers = this._blockRefersDict[blockData.BlockIndex];
				RectTransform levelNameHolder = blockRefers.levelAndNameHolder;
				this.ManagePoolItem(this._levelAndNamePool, levelNameHolder.gameObject, this._levelAndNameDict, blockKey.BuildingBlockIndex, true);
				BuildingBlockInfo levelNameRefers = this._levelAndNameDict[blockKey.BuildingBlockIndex];
				TextMeshProUGUI nameText = levelNameRefers.buildingName;
				ViewBuildingArea.SetBuildingName(nameText, BuildingBlock.Instance[blockData.TemplateId], blockKey, this._mapBlockData.TemplateId, false);
			}
		}

		// Token: 0x0600993E RID: 39230 RVA: 0x00478274 File Offset: 0x00476474
		private void OnMonthChange(ArgumentBox argBox)
		{
			bool flag = !this._isMonthChangeUpdate;
			if (flag)
			{
				this._isMonthChangeUpdate = true;
				this.ClearAnimation();
				this.UpdateRoad();
				this._waitMonthNotifyProcessComplete = true;
				this.RefreshNewlyCreatedBuilding();
				BuildingDomainMethod.Call.GetBuildingBlockList(this.Element.GameDataListenerId, new Location(this._areaId, this._blockId));
				base.StartCoroutine(this.ResetMonthChangeUpdate());
			}
		}

		// Token: 0x0600993F RID: 39231 RVA: 0x004782E3 File Offset: 0x004764E3
		private IEnumerator ResetMonthChangeUpdate()
		{
			while (SingletonObject.getInstance<BasicGameData>().AdvancingMonthState != 14)
			{
				yield return ViewBuildingArea.WaitForSeconds;
			}
			this.GetSettlementTreasuryDisplayData(null);
			this._isMonthChangeUpdate = false;
			yield return null;
			yield break;
		}

		// Token: 0x06009940 RID: 39232 RVA: 0x004782F4 File Offset: 0x004764F4
		public override void QuickHide()
		{
			bool leftMouseDown = CommonCommandKit.LeftMouse.Check(UIElement.BuildingArea, true, false, false, true, false);
			bool flag = leftMouseDown;
			if (!flag)
			{
				base.QuickHide();
			}
		}

		// Token: 0x06009941 RID: 39233 RVA: 0x00478328 File Offset: 0x00476528
		private void Update()
		{
			bool flag = this._buildingPlacementData.CurBuildingIcon != null && this._buildingPlacementData.CurBuildingIcon.gameObject.activeSelf && UIManager.Instance.IsFocusElement(UIElement.BuildingArea);
			if (flag)
			{
				UIRectDragMove dragMove = this._moveRoot.GetComponent<UIRectDragMove>();
				bool shouldEnableDragMove = false;
				this.UpdatePlacingBuilding();
				bool flag2 = this.isPlacingBuildingNow;
				if (flag2)
				{
					this.PlaceBuildingInputHandle();
					shouldEnableDragMove = true;
				}
				bool isResetBuilding = this._isResetBuilding;
				if (isResetBuilding)
				{
					this.ResetBuildingInputHandle();
					shouldEnableDragMove = true;
				}
				bool flag3 = shouldEnableDragMove != dragMove.enabled;
				if (flag3)
				{
					dragMove.enabled = shouldEnableDragMove;
				}
				this._moveRoot.GetComponent<UIRectMouseMove>().SetEnabled(true);
			}
			else
			{
				bool flag4 = !this._isCameraMoving && !this._hasSavedCameraState;
				if (flag4)
				{
					this._moveRoot.GetComponent<UIRectDragMove>().enabled = true;
					this._moveRoot.GetComponent<UIRectDragMove>().SetDirty();
				}
				this._moveRoot.GetComponent<UIRectMouseMove>().SetEnabled(false);
			}
			bool flag5 = CommonCommandKit.Space.Check(UIElement.StateBuilding, false, false, false, true, false);
			if (flag5)
			{
				bool flag6 = this._isStartPlanning && this.buttonConfirmPlanBuilding.interactable;
				if (flag6)
				{
					bool flag7 = this._isResetBuilding && this._buildingPlacementData.CurBuildingIcon != null;
					if (flag7)
					{
						this.CancelResetBuilding(false);
					}
					else
					{
						this.SwitchPlanBuildingState(true);
					}
				}
				else
				{
					bool flag8 = this._isStartMultiplyRemove && this.buttonConfirmMultiplyRemove.interactable;
					if (flag8)
					{
						this.ConfirmMultiplyRemove();
					}
					else
					{
						bool flag9 = this.isPlacingBuildingNow;
						if (flag9)
						{
							this.CancelPlaceBuilding(false);
						}
					}
				}
			}
		}

		// Token: 0x06009942 RID: 39234 RVA: 0x004784EC File Offset: 0x004766EC
		private void LateUpdate()
		{
			float scrollValue = Input.GetAxis("Mouse ScrollWheel");
			bool scaled = Math.Abs(scrollValue) > 0f;
			bool flag = scaled;
			if (flag)
			{
				this.SetNotScaleElement();
			}
		}

		// Token: 0x06009943 RID: 39235 RVA: 0x00478521 File Offset: 0x00476721
		private void ExitBuildingPlan()
		{
			this.SwitchPlanBuildingState(false);
			AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
		}

		// Token: 0x06009944 RID: 39236 RVA: 0x0047853E File Offset: 0x0047673E
		private void ExitMoveBuilding()
		{
			this.CancelResetBuilding(true);
		}

		// Token: 0x06009945 RID: 39237 RVA: 0x0047854C File Offset: 0x0047674C
		private void SetNotScaleElement()
		{
			bool flag = !this._moveRoot.GetComponent<CanvasGroup>().interactable;
			if (!flag)
			{
				float rootScale = this._moveRoot.localScale.x;
				float localScale = 1f / rootScale;
				localScale = Mathf.Clamp(localScale, 1f, this.localScaleMax);
				foreach (RectTransform ele in this._notScaleElements)
				{
					bool flag2 = ele != null;
					if (flag2)
					{
						ele.localScale = new Vector3(localScale, localScale, 1f);
					}
				}
				Vector2 rootScaleScaleMinMax = new Vector2(0.25f, 1f);
				foreach (ValueTuple<RectTransform, Vector2> ele2 in this._customScaleElements)
				{
					bool flag3 = ele2.Item1 != null;
					if (flag3)
					{
						float scale = TargetScaleAlign.GetAlignScale(rootScale, rootScaleScaleMinMax, ele2.Item2, true);
						ele2.Item1.localScale = new Vector3(scale, scale, 1f);
					}
				}
			}
		}

		// Token: 0x06009946 RID: 39238 RVA: 0x004786A0 File Offset: 0x004768A0
		public void GetSettlementTreasuryDisplayData(ArgumentBox box = null)
		{
			OrganizationDomainMethod.Call.GetSettlementTreasuryDisplayData(this.Element.GameDataListenerId, this._settlementId, 0);
			this.SetBottomInfoWithArg(box);
		}

		// Token: 0x06009947 RID: 39239 RVA: 0x004786C4 File Offset: 0x004768C4
		private void RefreshAllBlockTreasuryInfo()
		{
			for (short index = 0; index < (short)(this._areaData.Width * this._areaData.Width); index += 1)
			{
				BuildingBlockData blockData = ViewBuildingArea.BlockList[(int)index];
				bool flag = blockData.RootBlockIndex > -1;
				if (!flag)
				{
					BuildingAreaBlock block = this._blockRefersDict[blockData.BlockIndex];
					BuildingBlockKey key = this.MakeKey(blockData.BlockIndex);
					short templateId = blockData.TemplateId;
					bool isTreasury = templateId >= 284 && templateId <= 302;
					SettlementTreasury settlementTreasury = this._settlementTreasuryDisplayData.SettlementTreasury;
					bool showResourceStatus = isTreasury && settlementTreasury != null && this._settlementTreasuryDisplayData.ResourceStatus != 1;
					CImage parent = block.canBuild;
					this.ManagePoolItem(this._treasuryResourceInfoPool, parent.gameObject, this._treasuryResourceInfoDict, blockData.BlockIndex, showResourceStatus);
					BuildingBlockInfo blockInfo;
					bool flag2 = showResourceStatus && this._treasuryResourceInfoDict.TryGetValue(blockData.BlockIndex, out blockInfo);
					if (flag2)
					{
						blockInfo.name = string.Format("TreasuryResourceInfo_{0}", blockData.BlockIndex);
						CImage resourceStatus = blockInfo.resourceStatus;
						bool isPoor = this._settlementTreasuryDisplayData.ResourceStatus == 0;
						string resourceStatusSprite = isPoor ? "ui9_buildingarea_specialicon_4_0" : "ui9_buildingarea_specialicon_4_1";
						resourceStatus.SetSprite(resourceStatusSprite, false, null);
						templateId = blockData.TemplateId;
						bool isSect = templateId >= 288 && templateId <= 302;
						TooltipInvoker tip = blockInfo.resourceStatusTip;
						tip.Type = TipType.Simple;
						string[] presetParam = tip.PresetParam;
						bool flag3 = presetParam == null || presetParam.Length != 2;
						if (flag3)
						{
							tip.PresetParam = new string[2];
						}
						LanguageKey titleKey = isPoor ? LanguageKey.LK_Building_SettlementTreasury_ResourceTip_Title_Pool : LanguageKey.LK_Building_SettlementTreasury_ResourceTip_Title_Rich;
						blockInfo.resourceStatusText.SetText(LocalStringManager.Get(titleKey), true);
						LanguageKey contentKey = isSect ? (isPoor ? LanguageKey.LK_Building_SettlementTreasury_ResourceTip_Content_Pool_Sect : LanguageKey.LK_Building_SettlementTreasury_ResourceTip_Content_Rich_Sect) : (isPoor ? LanguageKey.LK_Building_SettlementTreasury_ResourceTip_Content_Pool : LanguageKey.LK_Building_SettlementTreasury_ResourceTip_Content_Rich);
						tip.PresetParam[0] = LocalStringManager.Get(titleKey);
						tip.PresetParam[1] = LocalStringManager.Get(contentKey).ColorReplace();
					}
				}
			}
		}

		// Token: 0x06009948 RID: 39240 RVA: 0x00478906 File Offset: 0x00476B06
		public void GetNeighborBlocks(short blockIndex, ref List<BuildingBlockData> neighborBlockList, sbyte blockWidth = 1, int range = 2, List<int> neighborDistanceList = null)
		{
			CommonUtils.GetNeighborBlocks(this._areaData, ViewBuildingArea.BlockList, blockIndex, ref neighborBlockList, blockWidth, range, neighborDistanceList);
		}

		// Token: 0x06009949 RID: 39241 RVA: 0x00478924 File Offset: 0x00476B24
		public bool ContainsBuilding(short blockTemplateId, bool requireCanUse = false)
		{
			bool flag = ViewBuildingArea.BlockList == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				for (short blockIndex = 0; blockIndex < (short)(this._areaData.Width * this._areaData.Width); blockIndex += 1)
				{
					bool flag2 = ViewBuildingArea.BlockList[(int)blockIndex].TemplateId != blockTemplateId;
					if (!flag2)
					{
						bool flag3 = !requireCanUse || ViewBuildingArea.BlockList[(int)blockIndex].CanUse();
						if (flag3)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x0600994A RID: 39242 RVA: 0x004789B0 File Offset: 0x00476BB0
		public void UpdateBuildingData(BuildingBlockKey blockKey, BuildingBlockData blockData, bool isUpdateInfo = true)
		{
			bool flag = blockKey.AreaId != this._areaId || blockKey.BlockId != this._blockId;
			if (!flag)
			{
				int buildingWidth = 1;
				int templateId = (int)ViewBuildingArea.BlockList[(int)blockKey.BuildingBlockIndex].TemplateId;
				bool flag2 = templateId >= 0;
				if (flag2)
				{
					buildingWidth = (int)BuildingBlock.Instance[templateId].Width;
				}
				BuildingAreaBlock block = this._blockRefersDict[blockKey.BuildingBlockIndex];
				int targetWidth = 1;
				bool flag3 = blockData.TemplateId >= 0;
				if (flag3)
				{
					targetWidth = (int)BuildingBlock.Instance[blockData.TemplateId].Width;
				}
				string widthStr = string.Empty;
				bool flag4 = targetWidth == 1;
				if (flag4)
				{
					widthStr = "SizeOne";
				}
				else
				{
					widthStr = "SizeTwo";
				}
				bool flag5 = !block.gameObject.name.Contains(widthStr);
				if (flag5)
				{
					this.ClearBlockRefers(block);
					this.CreateBlock(blockKey.BuildingBlockIndex, targetWidth);
				}
				bool flag6 = targetWidth > buildingWidth;
				if (flag6)
				{
					buildingWidth = targetWidth;
				}
				int blockX = (int)(blockKey.BuildingBlockIndex % (short)this._areaData.Width);
				int blockY = (int)(blockKey.BuildingBlockIndex / (short)this._areaData.Width);
				for (int i = blockX; i < Math.Min(blockX + buildingWidth, (int)this._areaData.Width); i++)
				{
					for (int j = blockY; j < Math.Min(blockY + buildingWidth, (int)this._areaData.Width); j++)
					{
						short index = (short)(j * (int)this._areaData.Width + i);
						bool flag7 = index < 0 || (int)index >= ViewBuildingArea.BlockList.Count;
						if (!flag7)
						{
							bool flag8 = index == blockKey.BuildingBlockIndex;
							if (flag8)
							{
								ViewBuildingArea.BlockList[(int)index] = blockData;
							}
							else
							{
								bool flag9 = blockData.TemplateId > 0;
								if (flag9)
								{
									ViewBuildingArea.BlockList[(int)index] = new BuildingBlockData(index, -1, -1, blockData.BlockIndex);
								}
								else
								{
									sbyte buildingLevel = this.BuildingModel.GetBuildingLevel(blockKey, blockData);
									ViewBuildingArea.BlockList[(int)index].ResetData(blockData.TemplateId, buildingLevel, -1);
								}
							}
							this.UpdateBlock(index, isUpdateInfo);
						}
					}
				}
			}
		}

		// Token: 0x0600994B RID: 39243 RVA: 0x00478C0C File Offset: 0x00476E0C
		private BuildingBlockKey MakeKey(short index)
		{
			return new BuildingBlockKey(this._areaId, this._blockId, index);
		}

		// Token: 0x0600994C RID: 39244 RVA: 0x00478C30 File Offset: 0x00476E30
		public static bool HasBuilding(short templateId, bool finishBuild = false)
		{
			bool flag = ViewBuildingArea.BlockList == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				foreach (BuildingBlockData blockData in ViewBuildingArea.BlockList)
				{
					bool flag2 = blockData.TemplateId == templateId;
					if (flag2)
					{
						bool flag3 = !finishBuild;
						if (flag3)
						{
							return true;
						}
						bool flag4 = blockData.OperationType != 0;
						if (flag4)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x0600994D RID: 39245 RVA: 0x00478CC8 File Offset: 0x00476EC8
		public static int GetBuildingCount(short templateId, bool finishBuild = false)
		{
			int count = 0;
			bool flag = ViewBuildingArea.BlockList == null;
			int result;
			if (flag)
			{
				result = count;
			}
			else
			{
				foreach (BuildingBlockData blockData in ViewBuildingArea.BlockList)
				{
					bool flag2 = blockData.TemplateId == templateId;
					if (flag2)
					{
						bool flag3 = !finishBuild || blockData.OperationType != 0;
						if (flag3)
						{
							count++;
						}
					}
				}
				result = count;
			}
			return result;
		}

		// Token: 0x0600994E RID: 39246 RVA: 0x00478D60 File Offset: 0x00476F60
		private void OnRequestOpenBuildingManage(ArgumentBox argsBox)
		{
			bool flag = argsBox == null;
			if (!flag)
			{
				this.OpenBuildingManage(argsBox);
			}
		}

		// Token: 0x0600994F RID: 39247 RVA: 0x00478D80 File Offset: 0x00476F80
		private void OnQuickActionMenuBackgroundClicked(ArgumentBox argsBox)
		{
			bool flag = argsBox == null;
			if (!flag)
			{
				BuildingBlockKey buildingKey;
				argsBox.Get<BuildingBlockKey>("buildingKey", out buildingKey);
				short buildingIndex = buildingKey.BuildingBlockIndex;
				Vector3 mousePos = Input.mousePosition;
				BuildingAreaBlock block;
				bool flag2 = this._blockRefersDict.TryGetValue(buildingIndex, out block);
				if (flag2)
				{
					RectTransform buildingIconRect = block.back.GetComponent<RectTransform>();
					Vector2 localPoint;
					bool flag3 = RectTransformUtility.ScreenPointToLocalPointInRectangle(buildingIconRect, mousePos, UIManager.Instance.UiCamera, out localPoint);
					if (flag3)
					{
						bool flag4 = buildingIconRect.rect.Contains(localPoint);
						if (flag4)
						{
							ArgumentBox args = EasyPool.Get<ArgumentBox>();
							args.SetObject("buildingKey", buildingKey).Set("tabKey", -1);
							this.OpenBuildingManage(args);
						}
					}
				}
			}
		}

		// Token: 0x06009950 RID: 39248 RVA: 0x00478E44 File Offset: 0x00477044
		private bool CheckAndAdjustCameraForQuickActionMenu(short blockIndex)
		{
			BuildingAreaBlock blockRefers;
			bool flag = !this._blockRefersDict.TryGetValue(blockIndex, out blockRefers);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				CRawImage backImage = blockRefers.back;
				bool flag2 = backImage == null;
				if (flag2)
				{
					result = false;
				}
				else
				{
					RectTransform backRectTransform = backImage.GetComponent<RectTransform>();
					RectTransform buildingRectTransform = blockRefers.GetComponent<RectTransform>();
					BuildingBlockData blockData = ViewBuildingArea.BlockList[(int)blockIndex];
					BuildingBlockItem configData = BuildingBlock.Instance[blockData.TemplateId];
					sbyte buildingWidth = (configData != null) ? configData.Width : 1;
					Vector2 buildingVisualCenterOffset = Vector2.zero;
					bool flag3 = buildingWidth == 2;
					if (flag3)
					{
						buildingVisualCenterOffset = new Vector2(215f, -215f);
					}
					Vector3[] backWorldCorners = new Vector3[4];
					backRectTransform.GetWorldCorners(backWorldCorners);
					Vector3 backWorldCenter = (backWorldCorners[0] + backWorldCorners[2]) * 0.5f;
					Canvas canvas = base.GetComponentInParent<Canvas>();
					bool flag4 = canvas == null;
					if (flag4)
					{
						result = false;
					}
					else
					{
						Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, backWorldCenter);
						int screenWidth = Screen.width;
						int screenHeight = Screen.height;
						float screenCenterX = (float)screenWidth * 0.5f;
						float screenCenterY = (float)screenHeight * 0.5f;
						Vector2 buildingCenterRelativeToScreenCenter = new Vector2(screenPoint.x - screenCenterX, screenPoint.y - screenCenterY);
						float distanceFromLeft = screenPoint.x;
						float distanceFromRight = (float)screenWidth - screenPoint.x;
						float distanceFromTop = (float)screenHeight - screenPoint.y;
						float distanceFromBottom = screenPoint.y;
						float edgeThreshold = Mathf.Max(200f, (float)screenWidth * 0.15f);
						bool needAdjust = false;
						Vector2 adjustOffset = Vector2.zero;
						bool flag5 = distanceFromLeft < edgeThreshold;
						if (flag5)
						{
							adjustOffset.x = edgeThreshold - distanceFromLeft;
							needAdjust = true;
						}
						else
						{
							bool flag6 = distanceFromRight < edgeThreshold;
							if (flag6)
							{
								adjustOffset.x = -(edgeThreshold - distanceFromRight);
								needAdjust = true;
							}
						}
						bool flag7 = distanceFromTop < edgeThreshold;
						if (flag7)
						{
							adjustOffset.y = -(edgeThreshold - distanceFromTop);
							needAdjust = true;
						}
						else
						{
							bool flag8 = distanceFromBottom < edgeThreshold;
							if (flag8)
							{
								adjustOffset.y = edgeThreshold - distanceFromBottom;
								needAdjust = true;
							}
						}
						bool flag9 = needAdjust;
						if (flag9)
						{
							Vector2 currentCameraPos = this._moveRoot.anchoredPosition;
							this._savedQuickActionCameraPos = currentCameraPos;
							this._hasSavedQuickActionCameraState = true;
							Vector2 targetPos = currentCameraPos + adjustOffset;
							this._moveRoot.DOAnchorPos(targetPos, this.quickActionMenuCameraMoveDuration, false).SetEase(Ease.OutQuad);
							result = true;
						}
						else
						{
							result = false;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06009951 RID: 39249 RVA: 0x004790AC File Offset: 0x004772AC
		public void RestoreCameraFromQuickActionMenu()
		{
			bool hasSavedQuickActionCameraState = this._hasSavedQuickActionCameraState;
			if (hasSavedQuickActionCameraState)
			{
				this._moveRoot.DOAnchorPos(this._savedQuickActionCameraPos, this.quickActionMenuCameraMoveDuration, false).SetEase(Ease.OutQuad);
				this._hasSavedQuickActionCameraState = false;
			}
		}

		// Token: 0x06009952 RID: 39250 RVA: 0x004790EC File Offset: 0x004772EC
		private void RefreshNewlyCreatedBuilding()
		{
			bool isTaiwuVillage = this._isTaiwuVillage;
			if (isTaiwuVillage)
			{
				BuildingDomainMethod.AsyncCall.GetNewlyCreatedBuildingIndex(this, delegate(int offset, RawDataPool dataPool)
				{
					Serializer.Deserialize(dataPool, offset, ref this._newlyCreatedBuildingIndexList);
				});
			}
			else
			{
				this._newlyCreatedBuildingIndexList.Clear();
			}
		}

		// Token: 0x06009953 RID: 39251 RVA: 0x0047912C File Offset: 0x0047732C
		private void OnMonthNotifyProcessComplete(ArgumentBox argbox)
		{
			bool waitMonthNotifyProcessComplete = this._waitMonthNotifyProcessComplete;
			if (waitMonthNotifyProcessComplete)
			{
				this._waitMonthNotifyProcessComplete = false;
				this.RefreshBuildingOperateState();
			}
		}

		// Token: 0x06009954 RID: 39252 RVA: 0x00479154 File Offset: 0x00477354
		private void WorldMapResetMapCamera(ArgumentBox _)
		{
			this.moveAndScaleRoot.localPosition = new Vector3(0f, 0f, 0f);
		}

		// Token: 0x06009955 RID: 39253 RVA: 0x00479178 File Offset: 0x00477378
		private void RefreshBuildingOperateState()
		{
			bool waitMonthNotifyProcessComplete = this._waitMonthNotifyProcessComplete;
			if (!waitMonthNotifyProcessComplete)
			{
				for (short index = 0; index < (short)(this._areaData.Width * this._areaData.Width); index += 1)
				{
					BuildingBlockData blockData = ViewBuildingArea.BlockList[(int)index];
					this.RefreshSingleBuildingBuildingOperateState(blockData);
				}
				BuildingDomainMethod.Call.ClearNewlyCreatedBuildingIndex();
			}
		}

		// Token: 0x06009956 RID: 39254 RVA: 0x004791D8 File Offset: 0x004773D8
		private void RefreshSingleBuildingBuildingOperateState(BuildingBlockData blockData)
		{
			BuildingAreaBlock blockRefers = this._blockRefersDict[blockData.BlockIndex];
			bool isNewlyCreatedBuilding = this._newlyCreatedBuildingIndexList.Contains(blockRefers.buildingBlockIndex);
			bool flag = isNewlyCreatedBuilding;
			if (flag)
			{
				blockRefers.CreateBuildingDone();
			}
			else
			{
				blockRefers.SetBuildingOperateState(blockData.OperationType, blockData.OperationStopping);
			}
		}

		// Token: 0x06009957 RID: 39255 RVA: 0x0047922B File Offset: 0x0047742B
		public static void Hide()
		{
			GEvent.OnEvent(UiEvents.BuildingAreaHide, EasyPool.Get<ArgumentBox>().Set("AreaId", SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageAreaId()));
		}

		// Token: 0x06009958 RID: 39256 RVA: 0x00479254 File Offset: 0x00477454
		private int GridRandomSuffix(int blockIndex)
		{
			return ((blockIndex ^ (int)this._settlementId ^ 15625) + (blockIndex ^ (int)this._areaId ^ 2401) + (blockIndex * (int)(this._settlementId ^ 6561) ^ blockIndex * (int)(this._areaId ^ 19937))) % 3 + 1;
		}

		// Token: 0x06009959 RID: 39257 RVA: 0x004792A0 File Offset: 0x004774A0
		private void SetBackImgRandom(CRawImage image, BuildingBlockItem config, short blockIndex, int level = 1)
		{
			this.sb.Clear();
			this.sb.Append("building_block");
			bool isBuilding = BuildingBlockData.IsBuilding(config.Type);
			bool isResource = BuildingBlockData.IsResource(config.Type);
			bool flag = isBuilding;
			if (flag)
			{
				this.sb.Append("_building");
			}
			else
			{
				bool flag2 = isResource;
				if (flag2)
				{
					this.sb.Append("_resource");
				}
				else
				{
					this.sb.Append("_empty");
				}
			}
			sbyte landFormType = this._areaData.LandFormType;
			this.sb.Append(string.Format("_{0}", landFormType));
			bool flag3 = isBuilding;
			if (flag3)
			{
				this.sb.Append((config.Width > 1) ? "_big" : "_small");
				string levelStr = this.GetLevelStr(config, level);
				this.sb.Append(levelStr);
			}
			else
			{
				this.sb.Append(string.Format("_{0}", this.GridRandomSuffix((int)blockIndex)));
			}
			string textureName = this.sb.ToString();
			image.SetTexture(textureName);
			image.SetNativeSize();
		}

		// Token: 0x0600995A RID: 39258 RVA: 0x004793D4 File Offset: 0x004775D4
		private string GetLevelStr(BuildingBlockItem config, int level)
		{
			int mapLevel = this.GetBuildingImgMapLevel(config, level);
			return string.Format("_{0}", mapLevel);
		}

		// Token: 0x0600995B RID: 39259 RVA: 0x00479400 File Offset: 0x00477600
		private int GetBuildingImgMapLevel(BuildingBlockItem config, int level)
		{
			bool flag = level >= (int)config.MaxLevel;
			int result2;
			if (flag)
			{
				result2 = 4;
			}
			else
			{
				bool flag2 = level < 0;
				if (flag2)
				{
					result2 = 1;
				}
				else
				{
					float ratio = Mathf.Clamp((float)level / (float)config.MaxLevel, 0f, 1f);
					int result = Mathf.FloorToInt(ratio * 4f) + 1;
					result2 = result;
				}
			}
			return result2;
		}

		// Token: 0x0600995C RID: 39260 RVA: 0x00479460 File Offset: 0x00477660
		private void SetBackGround()
		{
			this.backgroundBack.SetColor(this.GetBackgroundBackColor());
			GridLayoutGroup layoutGroup = this.backgroundLayout;
			bool flag = this._areaData.Width == 16;
			if (flag)
			{
				layoutGroup.cellSize = ViewBuildingArea.Width1;
			}
			else
			{
				bool flag2 = this._areaData.Width == 12;
				if (flag2)
				{
					layoutGroup.cellSize = ViewBuildingArea.Width2;
				}
				else
				{
					layoutGroup.cellSize = ViewBuildingArea.Width3;
				}
			}
			sbyte index = this._areaData.LandFormType;
			for (int i = 0; i < this.backgroundImgList.Length; i++)
			{
				bool flag3 = i == 0 || i == 2 || i == 6 || i == 8;
				int index2;
				if (flag3)
				{
					index2 = 0;
				}
				else
				{
					bool flag4 = i == 1 || i == 3 || i == 5 || i == 7;
					if (flag4)
					{
						index2 = 1;
					}
					else
					{
						index2 = 2;
					}
				}
				string bgName = string.Format("buildingarea_bg_{0}_{1}", index, index2);
				ViewBuildingArea.SetTexturePicture(this.backgroundImgList[i], "RemakeResources/Textures/Building/Border/" + bgName, false);
			}
		}

		// Token: 0x0600995D RID: 39261 RVA: 0x00479584 File Offset: 0x00477784
		private Color GetBackgroundBackColor()
		{
			return this.backgroundBackColor[(int)this._areaData.LandFormType];
		}

		// Token: 0x17001061 RID: 4193
		// (get) Token: 0x0600995E RID: 39262 RVA: 0x004795AC File Offset: 0x004777AC
		private TaskModel TaskModel
		{
			get
			{
				return SingletonObject.getInstance<TaskModel>();
			}
		}

		// Token: 0x0600995F RID: 39263 RVA: 0x004795B3 File Offset: 0x004777B3
		private void TryInitSettlementRoadAnimation()
		{
			this.settlementRoadAnimation.TryInit(this._settlementId, this._roadCrossPos);
			this.settlementRoadAnimation.animationHolder.gameObject.SetActive(true);
		}

		// Token: 0x06009960 RID: 39264 RVA: 0x004795E8 File Offset: 0x004777E8
		public void OnChickenClick(int id)
		{
			bool flag = !this.canUseBuilding;
			if (!flag)
			{
				bool flag2 = this.TaskModel.IsTaskInProgress(303);
				if (flag2)
				{
					BuildingDomainMethod.Call.ClickChickenSign(id);
				}
				else
				{
					bool flag3 = this.settlementRoadAnimation.CanPluckFeatherChickenIds.Contains(id);
					if (flag3)
					{
						BuildingDomainMethod.Call.PluckChickenFeather(this.Element.GameDataListenerId, id);
						this.settlementRoadAnimation.PlayChickenLoseFeatureEffect();
						this.settlementRoadAnimation.UpdateChickenFeather();
					}
					else
					{
						TaiwuEventDomainMethod.Call.OnClickChickenCoop();
					}
				}
			}
		}

		// Token: 0x06009961 RID: 39265 RVA: 0x0047966D File Offset: 0x0047786D
		public void ShowAfterRefresh()
		{
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x06009962 RID: 39266 RVA: 0x0047967C File Offset: 0x0047787C
		private void UpdateBuildingSpaceInfo()
		{
			this.buildingAreaInfo.UpdateBuildingSpaceInfo(this._buildingSpaceCurr, this._buildingSpaceLimit);
		}

		// Token: 0x06009963 RID: 39267 RVA: 0x00479698 File Offset: 0x00477898
		private void BuildingBlockDataChange(ArgumentBox argBox)
		{
			this.buildingAreaInfo.RefreshBuildAreaInfo(this._isTaiwuVillage, this._isSectLocation, this._isCityLocation, this._isBambooHouse, this._mapBlockData, this._areaData, ViewBuildingArea.BlockList, this._settlementId, this.settlementRoadAnimation.HaveChickenKing);
		}

		// Token: 0x06009964 RID: 39268 RVA: 0x004796EC File Offset: 0x004778EC
		private void RefreshAllException(BuildingExceptionData buildingExceptionData)
		{
			foreach (KeyValuePair<short, BuildingBlockData> pair in this._buildingDict)
			{
				BuildingBlockData blockData = pair.Value;
				BuildingBlockKey buildingBlockKey = this.MakeKey(blockData.BlockIndex);
				BuildingAreaBlock block = this._blockRefersDict[pair.Key];
				this.RefreshException(buildingBlockKey, block, buildingExceptionData);
			}
			this.buildingExceptionInfo.RefreshInfo(buildingExceptionData, this._mapBlockData);
		}

		// Token: 0x06009965 RID: 39269 RVA: 0x00479784 File Offset: 0x00477984
		private void RefreshException(BuildingBlockKey blockKey, BuildingAreaBlock block, BuildingExceptionData buildingExceptionData)
		{
			BuildingExceptionItem exceptionItem;
			bool flag;
			if (buildingExceptionData.BuildingExceptionDict.TryGetValue(blockKey, out exceptionItem) && exceptionItem != null)
			{
				List<sbyte> exceptionTypeList = exceptionItem.ExceptionTypeList;
				int? num = (exceptionTypeList != null) ? new int?(exceptionTypeList.Count) : null;
				int num2 = 0;
				if ((num.GetValueOrDefault() > num2 & num != null) && !this._isStartPlanning && !this._isStartMultiplyRemove)
				{
					flag = this._isTaiwuVillage;
					goto IL_67;
				}
			}
			flag = false;
			IL_67:
			bool showException = flag;
			RectTransform parent = block.leftTopHolder;
			this.ManagePoolItem(this._exceptionPool, parent.gameObject, this._exceptionDict, blockKey.BuildingBlockIndex, showException);
			bool flag2 = !showException;
			if (!flag2)
			{
				StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
				stringBuilder.Clear();
				BuildingBlockInfo exceptionRefers = this._exceptionDict[blockKey.BuildingBlockIndex];
				TooltipInvoker tip = exceptionRefers.exceptionTip;
				foreach (sbyte exceptionType in exceptionItem.ExceptionTypeList)
				{
					string strKey = ViewBuildingArea.GetBuildingExceptionString((BuildingExceptionType)exceptionType);
					stringBuilder.AppendLine(strKey.SetColor("darkred"));
				}
				string tipContent = stringBuilder.ToString();
				TooltipInvoker tooltipInvoker = tip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				tip.RuntimeParam.Set("arg0", tipContent);
				exceptionRefers.exceptionText.text = exceptionItem.ExceptionTypeList.Count.ToString();
				EasyPool.Free<StringBuilder>(stringBuilder);
			}
		}

		// Token: 0x06009966 RID: 39270 RVA: 0x0047991C File Offset: 0x00477B1C
		private void RefreshExceptionInfo(ArgumentBox argBox)
		{
			BuildingDomainMethod.Call.GetBuildingExceptionData(this.Element.GameDataListenerId);
		}

		// Token: 0x06009967 RID: 39271 RVA: 0x00479930 File Offset: 0x00477B30
		public void UpdateRoad()
		{
			sbyte areaWidth = this._areaData.Width;
			this.ClearRoad();
			HashSet<Vector2Int> crossPoints = EasyPool.Get<HashSet<Vector2Int>>();
			this.CollectCrossPoints(crossPoints, (int)areaWidth);
			int padding = 180;
			int cellSize = 380;
			int roadWidth = 50;
			foreach (Vector2Int crossKey in crossPoints)
			{
				bool hasUp = crossPoints.Contains(crossKey + Vector2Int.down);
				bool hasDown = crossPoints.Contains(crossKey + Vector2Int.up);
				bool hasLeft = crossPoints.Contains(crossKey + Vector2Int.left);
				bool hasRight = crossPoints.Contains(crossKey + Vector2Int.right);
				int x = padding + (cellSize + roadWidth) * crossKey.x - roadWidth / 2;
				int y = -(padding + (cellSize + roadWidth) * crossKey.y - roadWidth / 2);
				int roadX = padding + (cellSize + roadWidth) * crossKey.x + cellSize / 2;
				int roadY = -(padding + (cellSize + roadWidth) * crossKey.y + cellSize / 2);
				int index = crossKey.y * (int)areaWidth + crossKey.x;
				BuildingBlockKey upperLeftBlockKey = new BuildingBlockKey(this._areaId, this._blockId, (short)(index - (int)areaWidth - 1));
				BuildingBlockKey upperRightBlockKey = new BuildingBlockKey(this._areaId, this._blockId, (short)(index - (int)areaWidth));
				BuildingBlockKey lowerLeftBlockKey = new BuildingBlockKey(this._areaId, this._blockId, (short)(index - 1));
				BuildingBlockKey lowerRightBlockKey = new BuildingBlockKey(this._areaId, this._blockId, (short)index);
				bool flag = crossKey.x > 0 && crossKey.y > 0 && this.IsBuilding((int)upperLeftBlockKey.BuildingBlockIndex);
				bool hasUpperRightBuilding = crossKey.x < (int)areaWidth && crossKey.y > 0 && this.IsBuilding((int)upperRightBlockKey.BuildingBlockIndex);
				bool hasLowerLeftBuilding = crossKey.x > 0 && crossKey.y < (int)areaWidth && this.IsBuilding((int)lowerLeftBlockKey.BuildingBlockIndex);
				bool hasLowerRightBuilding = crossKey.x < (int)areaWidth && crossKey.y < (int)areaWidth && this.IsBuilding((int)lowerRightBlockKey.BuildingBlockIndex);
				ValueTuple<int, int> upperLeftInfo = this.GetBuildingMapLevelAndWidth(upperLeftBlockKey);
				ValueTuple<int, int> upperRightInfo = this.GetBuildingMapLevelAndWidth(upperRightBlockKey);
				ValueTuple<int, int> lowerLeftInfo = this.GetBuildingMapLevelAndWidth(lowerLeftBlockKey);
				ValueTuple<int, int> lowerRightInfo = this.GetBuildingMapLevelAndWidth(lowerRightBlockKey);
				this.AddRoadCrossPos((float)x, (float)y, crossKey);
				ValueTuple<int, int> buildingMapLevelAndWidth = this.GetBuildingMapLevelAndWidth(upperLeftInfo, upperRightInfo, lowerLeftInfo, lowerRightInfo);
				int mapLevel = buildingMapLevelAndWidth.Item1;
				int width = buildingMapLevelAndWidth.Item2;
				GameObject cornerObj = this.GenerateRoadCorner((float)x, (float)y, crossKey);
				this.SetRoadCornerImageRandom(cornerObj, mapLevel, width);
				bool flag2 = (hasLowerRightBuilding || hasLowerLeftBuilding) && hasDown;
				if (flag2)
				{
					ValueTuple<int, int> buildingMapLevelAndWidth2 = this.GetBuildingMapLevelAndWidth(lowerLeftInfo, lowerRightInfo);
					int mapLevel2 = buildingMapLevelAndWidth2.Item1;
					int width2 = buildingMapLevelAndWidth2.Item2;
					GameObject roadObj = this.GenerateRoad((float)x, (float)roadY, 90f, crossKey);
					this.SetRoadImageRandom(roadObj, mapLevel2, width2);
				}
				bool flag3 = (hasLowerRightBuilding || hasUpperRightBuilding) && hasRight;
				if (flag3)
				{
					ValueTuple<int, int> buildingMapLevelAndWidth3 = this.GetBuildingMapLevelAndWidth(upperRightInfo, lowerRightInfo);
					int mapLevel3 = buildingMapLevelAndWidth3.Item1;
					int width3 = buildingMapLevelAndWidth3.Item2;
					GameObject roadObj2 = this.GenerateRoad((float)roadX, (float)y, 0f, crossKey);
					this.SetRoadImageRandom(roadObj2, mapLevel3, width3);
				}
			}
			EasyPool.Free<HashSet<Vector2Int>>(crossPoints);
			this.roadHolder.gameObject.SetActive(true);
			this.roadCornerHolder.gameObject.SetActive(true);
		}

		// Token: 0x06009968 RID: 39272 RVA: 0x00479CA8 File Offset: 0x00477EA8
		private void ClearRoad()
		{
			foreach (GameObject roadObj in this._roadObjList)
			{
				this._roadPool.DestroyObject(roadObj);
			}
			this._roadObjList.Clear();
			this._roadCrossPos.Clear();
		}

		// Token: 0x06009969 RID: 39273 RVA: 0x00479D1C File Offset: 0x00477F1C
		private void CollectCrossPoints(HashSet<Vector2Int> points, int areaWidth)
		{
			foreach (KeyValuePair<short, BuildingBlockData> pair in this._buildingDict)
			{
				bool flag = !this.IsBuilding((int)pair.Key);
				if (!flag)
				{
					BuildingBlockItem template = BuildingBlock.Instance[pair.Value.TemplateId];
					bool flag2 = template == null;
					if (!flag2)
					{
						int width = (int)template.Width;
						int y = (int)pair.Key / areaWidth;
						int x = (int)pair.Key % areaWidth;
						for (int i = 0; i <= width; i++)
						{
							points.Add(new Vector2Int(x + i, y));
							points.Add(new Vector2Int(x + i, y + width));
							points.Add(new Vector2Int(x, y + i));
							points.Add(new Vector2Int(x + width, y + i));
						}
					}
				}
			}
		}

		// Token: 0x0600996A RID: 39274 RVA: 0x00479E3C File Offset: 0x0047803C
		private void AddRoadCrossPos(float x, float y, Vector2Int crossKey)
		{
			bool flag = !this._roadCrossPos.ContainsKey(crossKey);
			if (flag)
			{
				this._roadCrossPos.Add(crossKey, new Vector2Int((int)x, (int)y));
			}
		}

		// Token: 0x0600996B RID: 39275 RVA: 0x00479E74 File Offset: 0x00478074
		private GameObject GenerateRoad(float x, float y, float rotateAngle, Vector2Int crossKey)
		{
			GameObject roadObj = this._roadPool.GetObject();
			RectTransform roadTransform = roadObj.GetComponent<RectTransform>();
			roadObj.name = crossKey.ToString();
			roadTransform.SetParent(this.roadHolder, false);
			roadTransform.anchoredPosition = new Vector3(x, y);
			roadTransform.localRotation = Quaternion.Euler(0f, 0f, rotateAngle);
			this._roadObjList.Add(roadObj);
			return roadObj;
		}

		// Token: 0x0600996C RID: 39276 RVA: 0x00479EF4 File Offset: 0x004780F4
		private GameObject GenerateRoadCorner(float x, float y, Vector2Int crossKey)
		{
			GameObject roadObj = this._roadPool.GetObject();
			RectTransform roadTransform = roadObj.GetComponent<RectTransform>();
			roadObj.name = crossKey.ToString();
			roadTransform.SetParent(this.roadCornerHolder, false);
			roadTransform.anchoredPosition = new Vector3(x, y);
			this._roadObjList.Add(roadObj);
			return roadObj;
		}

		// Token: 0x0600996D RID: 39277 RVA: 0x00479F5C File Offset: 0x0047815C
		private void SetRoadImageRandom(GameObject roadObj, int mapLevel, int width)
		{
			CRawImage roadImg = roadObj.GetComponent<CRawImage>();
			sbyte landFormType = this._areaData.LandFormType;
			string widthStr = (width > 1) ? "big" : "small";
			string textureName = string.Format("building_road_{0}_{1}_{2}", landFormType, widthStr, mapLevel);
			roadImg.SetTexture(textureName);
			roadImg.SetNativeSize();
		}

		// Token: 0x0600996E RID: 39278 RVA: 0x00479FB8 File Offset: 0x004781B8
		private void SetRoadCornerImageRandom(GameObject roadObj, int mapLevel, int width)
		{
			CRawImage roadImg = roadObj.GetComponent<CRawImage>();
			sbyte landFormType = this._areaData.LandFormType;
			string widthStr = (width > 1) ? "big" : "small";
			string textureName = string.Format("building_road_{0}_{1}_{2}_corner", landFormType, widthStr, mapLevel);
			roadImg.SetTexture(textureName);
			roadImg.SetNativeSize();
		}

		// Token: 0x0600996F RID: 39279 RVA: 0x0047A014 File Offset: 0x00478214
		public void GetNeighborCross(Vector2Int crossKey, List<Vector2Int> neighborCross)
		{
			neighborCross.Clear();
			Dictionary<Vector2Int, Vector2Int>.KeyCollection crossKeys = this._roadCrossPos.Keys;
			bool flag = crossKeys.Contains(crossKey + Vector2Int.left);
			if (flag)
			{
				neighborCross.Add(crossKey + Vector2Int.left);
			}
			bool flag2 = crossKeys.Contains(crossKey + Vector2Int.right);
			if (flag2)
			{
				neighborCross.Add(crossKey + Vector2Int.right);
			}
			bool flag3 = crossKeys.Contains(crossKey + Vector2Int.up);
			if (flag3)
			{
				neighborCross.Add(crossKey + Vector2Int.up);
			}
			bool flag4 = crossKeys.Contains(crossKey + Vector2Int.down);
			if (flag4)
			{
				neighborCross.Add(crossKey + Vector2Int.down);
			}
		}

		// Token: 0x06009970 RID: 39280 RVA: 0x0047A0D3 File Offset: 0x004782D3
		private void SetRoadHolderSize(Vector2 size)
		{
			this.roadHolder.SetSize(size);
			this.roadCornerHolder.SetSize(size);
			this.settlementRoadAnimation.animationHolder.SetSize(size);
		}

		// Token: 0x06009971 RID: 39281 RVA: 0x0047A102 File Offset: 0x00478302
		private void SetRoadActive(bool active)
		{
			this.roadHolder.gameObject.SetActive(active);
			this.roadCornerHolder.gameObject.SetActive(active);
			this.settlementRoadAnimation.animationHolder.gameObject.SetActive(active);
		}

		// Token: 0x06009972 RID: 39282 RVA: 0x0047A140 File Offset: 0x00478340
		private bool IsBuilding(int blockIndex)
		{
			bool flag = blockIndex < 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				BuildingBlockData blockData = ViewBuildingArea.BlockList[blockIndex];
				bool flag2 = blockData.RootBlockIndex >= 0;
				if (flag2)
				{
					result = true;
				}
				else
				{
					EBuildingBlockType type = BuildingBlock.Instance[blockData.TemplateId].Type;
					result = (type == EBuildingBlockType.Building || type == EBuildingBlockType.MainBuilding);
				}
			}
			return result;
		}

		// Token: 0x06009973 RID: 39283 RVA: 0x0047A1A4 File Offset: 0x004783A4
		[return: TupleElementNames(new string[]
		{
			"mapLevel",
			"width"
		})]
		private ValueTuple<int, int> GetBuildingMapLevelAndWidth(BuildingBlockKey blockKey)
		{
			bool flag = blockKey.BuildingBlockIndex < 0 || blockKey.BuildingBlockIndex >= (short)(this._areaData.Width * this._areaData.Width);
			ValueTuple<int, int> result;
			if (flag)
			{
				result = new ValueTuple<int, int>(-1, -1);
			}
			else
			{
				BuildingBlockData blockData = ViewBuildingArea.BlockList[(int)blockKey.BuildingBlockIndex];
				bool flag2 = blockData.TemplateId == 0;
				if (flag2)
				{
					result = new ValueTuple<int, int>(-1, -1);
				}
				else
				{
					bool flag3 = blockData.RootBlockIndex >= 0;
					if (flag3)
					{
						short rootIndex = blockData.RootBlockIndex;
						blockData = ViewBuildingArea.BlockList[(int)rootIndex];
						blockKey = new BuildingBlockKey(blockKey.AreaId, blockKey.BlockId, rootIndex);
					}
					BuildingBlockItem config = BuildingBlock.Instance[blockData.TemplateId];
					short templateId = blockData.TemplateId;
					bool flag4 = templateId == 257 || templateId == 258 || this.isSecretVilliage || !this.IsBuilding((int)blockData.BlockIndex);
					if (flag4)
					{
						result = new ValueTuple<int, int>(1, (int)config.Width);
					}
					else
					{
						sbyte level = this.BuildingModel.GetBuildingLevel(blockKey, blockData);
						result = new ValueTuple<int, int>(this.GetBuildingImgMapLevel(config, (int)level), (int)config.Width);
					}
				}
			}
			return result;
		}

		// Token: 0x06009974 RID: 39284 RVA: 0x0047A2E0 File Offset: 0x004784E0
		[return: TupleElementNames(new string[]
		{
			"mapLevel",
			"width"
		})]
		private ValueTuple<int, int> GetBuildingMapLevelAndWidth([TupleElementNames(new string[]
		{
			"mapLevel",
			"width"
		})] ValueTuple<int, int> info1, [TupleElementNames(new string[]
		{
			"mapLevel",
			"width"
		})] ValueTuple<int, int> info2)
		{
			int mapLevel = info1.Item1;
			int width = info1.Item2;
			bool flag = info2.Item1 > mapLevel;
			if (flag)
			{
				mapLevel = info2.Item1;
				width = info2.Item2;
			}
			else
			{
				bool flag2 = info2.Item1 == mapLevel;
				if (flag2)
				{
					width = Math.Max(info1.Item2, info2.Item2);
				}
			}
			return new ValueTuple<int, int>(mapLevel, width);
		}

		// Token: 0x06009975 RID: 39285 RVA: 0x0047A34C File Offset: 0x0047854C
		[return: TupleElementNames(new string[]
		{
			"mapLevel",
			"width"
		})]
		private ValueTuple<int, int> GetBuildingMapLevelAndWidth([TupleElementNames(new string[]
		{
			"mapLevel",
			"width"
		})] ValueTuple<int, int> info1, [TupleElementNames(new string[]
		{
			"mapLevel",
			"width"
		})] ValueTuple<int, int> info2, [TupleElementNames(new string[]
		{
			"mapLevel",
			"width"
		})] ValueTuple<int, int> info3, [TupleElementNames(new string[]
		{
			"mapLevel",
			"width"
		})] ValueTuple<int, int> info4)
		{
			ValueTuple<int, int> infoTmp = this.GetBuildingMapLevelAndWidth(info1, info2);
			ValueTuple<int, int> infoTmp2 = this.GetBuildingMapLevelAndWidth(infoTmp, info3);
			return this.GetBuildingMapLevelAndWidth(infoTmp2, info4);
		}

		// Token: 0x06009976 RID: 39286 RVA: 0x0047A37B File Offset: 0x0047857B
		public void StartPlanBuilding()
		{
			this.SetElementStateForPlan(true);
			this.SetBlockCannotMove();
			this.CopyBlockListData(true);
			this.SetBuildingDescMouseTipEnable(false);
			GlobalDomainMethod.Call.InvokeGuidingTrigger(144);
		}

		// Token: 0x06009977 RID: 39287 RVA: 0x0047A3A8 File Offset: 0x004785A8
		private void FinishPlanBuilding()
		{
			this.SetElementStateForPlan(false);
			this._operateRecord.Clear();
			this.CancelBlockCannotMove();
			this.UpdateAllBlockInfo();
			this.SetBuildingDescMouseTipEnable(true);
			bool flag = this.PlanBuildingIsSameAll();
			if (flag)
			{
				this.SetRoadActive(true);
			}
		}

		// Token: 0x06009978 RID: 39288 RVA: 0x0047A3F4 File Offset: 0x004785F4
		private void SetBuildingDescMouseTipEnable(bool enable)
		{
			foreach (BuildingAreaBlock refer in this._blockRefersDict.Values)
			{
				TooltipInvoker mouseTip = refer.buildingDescMouseTip;
				mouseTip.enabled = enable;
			}
		}

		// Token: 0x06009979 RID: 39289 RVA: 0x0047A45C File Offset: 0x0047865C
		private void SetBlockToEmpty(short blockIndex)
		{
			BuildingBlockData blockData = ViewBuildingArea.BlockList[(int)blockIndex];
			BuildingBlockItem blockConfig = BuildingBlock.Instance.GetItem(blockData.TemplateId);
			bool flag = blockConfig.Width == 1;
			if (flag)
			{
				ViewBuildingArea.BlockList[(int)blockIndex] = new BuildingBlockData(blockIndex, 0, -1, -1);
				this.UpdateBlock(blockIndex, true);
			}
			else
			{
				bool flag2 = blockConfig.Width == 2;
				if (flag2)
				{
					ViewBuildingArea.BlockList[(int)blockIndex] = new BuildingBlockData(blockIndex, 0, -1, -1);
					this.UpdateBlock(blockIndex, true);
					this._blockRefersDict[blockIndex].back.SetNativeSize();
					this._blockRefersDict[blockIndex].back.rectTransform.localPosition = Vector2.zero;
					ViewBuildingArea.BlockList[(int)(blockIndex + 1)] = new BuildingBlockData(blockIndex + 1, 0, -1, -1);
					this.UpdateBlock(blockIndex + 1, true);
					ViewBuildingArea.BlockList[(int)(blockIndex + (short)this._areaData.Width)] = new BuildingBlockData(blockIndex + (short)this._areaData.Width, 0, -1, -1);
					this.UpdateBlock(blockIndex + (short)this._areaData.Width, true);
					ViewBuildingArea.BlockList[(int)(blockIndex + (short)this._areaData.Width + 1)] = new BuildingBlockData(blockIndex + (short)this._areaData.Width + 1, 0, -1, -1);
					this.UpdateBlock(blockIndex + (short)this._areaData.Width + 1, true);
				}
			}
			this._nowResetBuildingData.Item1 = blockIndex;
			this._nowResetBuildingData.Item2 = blockData;
			this.StartResetBuildingPlace(blockConfig);
		}

		// Token: 0x0600997A RID: 39290 RVA: 0x0047A5FC File Offset: 0x004787FC
		private void SetNewBlockData(short indexNow, short indexOri)
		{
			this._blockRefersDict[indexNow].leftTopHolder.gameObject.SetActive(false);
			this._nowResetBuildingData.Item2.BlockIndex = indexNow;
			ViewBuildingArea.BlockList[(int)indexNow] = this._nowResetBuildingData.Item2;
			this._buildingDict.Remove(indexOri);
			this._buildingDict.Add(indexNow, this._nowResetBuildingData.Item2);
			ConchShipCursor.Instance.SetCursorImage("sp_cursor_base", 0.5f, 0.5f);
			this.SetBuildingIconPointTrigger(true);
			this.UpDateCustomBuildingName(indexOri, indexNow);
			BuildingBlockItem blockConfig = BuildingBlock.Instance.GetItem(this._nowResetBuildingData.Item2.TemplateId);
			bool flag = blockConfig.Width == 1;
			if (flag)
			{
				this.UpdateBuildingData(new BuildingBlockKey(this._areaId, this._blockId, indexNow), this._nowResetBuildingData.Item2, false);
			}
			else
			{
				bool flag2 = blockConfig.Width == 2;
				if (flag2)
				{
					this.UpdateBuildingData(new BuildingBlockKey(this._areaId, this._blockId, indexNow), this._nowResetBuildingData.Item2, false);
					this._blockRefersDict[indexNow].back.rectTransform.localPosition = new Vector3(210f, -210f);
				}
			}
		}

		// Token: 0x0600997B RID: 39291 RVA: 0x0047A750 File Offset: 0x00478950
		private void UpDateCustomBuildingName(short indexOri, short indexNow)
		{
			BuildingBlockKey blockKeyOri = new BuildingBlockKey(this._areaId, this._blockId, indexOri);
			BuildingBlockKey blockKeyNow = new BuildingBlockKey(this._areaId, this._blockId, indexNow);
			BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
			bool flag = buildingModel.CustomBuildingName.ContainsKey(blockKeyOri);
			if (flag)
			{
				int nameData = buildingModel.CustomBuildingName[blockKeyOri];
				buildingModel.CustomBuildingName.Remove(blockKeyOri);
				buildingModel.CustomBuildingName.Add(blockKeyNow, nameData);
			}
		}

		// Token: 0x0600997C RID: 39292 RVA: 0x0047A7C8 File Offset: 0x004789C8
		private void SetBlockCannotMove()
		{
			foreach (KeyValuePair<short, BuildingBlockData> pair in this._buildingDict)
			{
				BuildingBlockData blockData = pair.Value;
				BuildingBlockItem blockConfig = BuildingBlock.Instance.GetItem(blockData.TemplateId);
				BuildingAreaBlock blockRefers = this._blockRefersDict[pair.Key];
				blockRefers.leftTopHolder.gameObject.SetActive(false);
				blockRefers.getEarnHolder.gameObject.SetActive(false);
				bool canMove = blockConfig.MoveBuildCostResourceRate > 0;
				bool flag = !canMove;
				if (flag)
				{
					this.SetBuildingBlockInteractable(blockRefers, false);
					blockRefers.selectTip.enabled = false;
				}
				else
				{
					bool flag2 = blockData.OperationType != -1;
					if (flag2)
					{
						this.SetBuildingBlockInteractable(blockRefers, false);
						blockRefers.selectTip.enabled = false;
					}
				}
			}
			this.SetRoadActive(false);
		}

		// Token: 0x0600997D RID: 39293 RVA: 0x0047A8D8 File Offset: 0x00478AD8
		private void SetBuildingBlockInteractable(BuildingAreaBlock block, bool interactable)
		{
			block.buildingIcon.GetComponent<CButton>().interactable = interactable;
			block.styleRoot.SetInteractable(interactable);
		}

		// Token: 0x0600997E RID: 39294 RVA: 0x0047A8FC File Offset: 0x00478AFC
		private void CancelBlockCannotMove()
		{
			foreach (KeyValuePair<short, BuildingAreaBlock> keyValuePair in this._blockRefersDict)
			{
				short num;
				BuildingAreaBlock buildingAreaBlock;
				keyValuePair.Deconstruct(out num, out buildingAreaBlock);
				short key = num;
				BuildingAreaBlock blockRefers = buildingAreaBlock;
				BuildingBlockData blockData;
				bool flag = !this._buildingDict.TryGetValue(key, out blockData);
				if (!flag)
				{
					BuildingBlockItem blockConfig = BuildingBlock.Instance.GetItem(blockData.TemplateId);
					bool interactable = this.BuildingBlockCanInteractable(blockData, blockConfig);
					this.SetBuildingBlockInteractable(blockRefers, interactable);
					blockRefers.selectTip.enabled = true;
					blockRefers.leftTopHolder.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x0600997F RID: 39295 RVA: 0x0047A9C4 File Offset: 0x00478BC4
		private void RecoverBlockData(short blockIndex)
		{
			ViewBuildingArea.BlockList[(int)blockIndex] = this._nowResetBuildingData.Item2;
			BuildingBlockData blockData = ViewBuildingArea.BlockList[(int)blockIndex];
			BuildingBlockItem blockConfig = BuildingBlock.Instance.GetItem(blockData.TemplateId);
			bool flag = blockConfig.Width == 1;
			if (flag)
			{
				this.UpdateBlock(blockIndex, false);
			}
			else
			{
				bool flag2 = blockConfig.Width == 2;
				if (flag2)
				{
					this.UpdateBlock(blockIndex, false);
					ViewBuildingArea.BlockList[(int)(blockIndex + 1)] = new BuildingBlockData(blockIndex + 1, -1, -1, blockIndex);
					this.UpdateBlock(blockIndex + 1, true);
					ViewBuildingArea.BlockList[(int)(blockIndex + (short)this._areaData.Width)] = new BuildingBlockData(blockIndex + (short)this._areaData.Width, -1, -1, blockIndex);
					this.UpdateBlock(blockIndex + (short)this._areaData.Width, true);
					ViewBuildingArea.BlockList[(int)(blockIndex + (short)this._areaData.Width + 1)] = new BuildingBlockData(blockIndex + (short)this._areaData.Width + 1, -1, -1, blockIndex);
					this.UpdateBlock(blockIndex + (short)this._areaData.Width + 1, true);
					this._blockRefersDict[blockIndex].back.SetNativeSize();
					this._blockRefersDict[blockIndex].back.rectTransform.sizeDelta = new Vector2(740f, 740f);
					this._blockRefersDict[this._nowResetBuildingData.Item1].back.rectTransform.localPosition = new Vector2(210f, -210f);
				}
			}
			this.SetBuildingIconPointTrigger(true);
			this._buildingPlacementData.MouseClickPos = Vector3.zero;
		}

		// Token: 0x06009980 RID: 39296 RVA: 0x0047AB88 File Offset: 0x00478D88
		private void CopyBlockListData(bool state)
		{
			this._blockListCopy.Clear();
			if (state)
			{
				foreach (BuildingBlockData element in ViewBuildingArea.BlockList)
				{
					this._blockListCopy.Add(element.Clone());
				}
			}
		}

		// Token: 0x06009981 RID: 39297 RVA: 0x0047AC00 File Offset: 0x00478E00
		private bool PlanBuildingIsSameAll()
		{
			bool isSameAll = true;
			for (int i = 0; i < this._blockListCopy.Count; i++)
			{
				bool flag = !this._blockListCopy[i].PlanEquals(ViewBuildingArea.BlockList[i]);
				if (flag)
				{
					isSameAll = false;
					break;
				}
			}
			return isSameAll;
		}

		// Token: 0x06009982 RID: 39298 RVA: 0x0047AC5C File Offset: 0x00478E5C
		public void SwitchPlanBuildingState(bool confirm)
		{
			bool isSameAll = this.PlanBuildingIsSameAll();
			bool flag = confirm && this._isStartPlanning;
			if (flag)
			{
				bool flag2 = !isSameAll;
				if (flag2)
				{
					HashSet<int> sameSet = new HashSet<int>();
					HashSet<int> realMovedBuildingIndexSet = (from p in this._operateRecord
					where ViewBuildingArea.BlockList[p.Second].TemplateId != 0
					select p.Second).Distinct<int>().ToHashSet<int>();
					foreach (int index in realMovedBuildingIndexSet)
					{
						BuildingBlockData blockData = ViewBuildingArea.BlockList[index];
						BuildingBlockData originBlockData = this._blockListCopy[index];
						bool flag3 = originBlockData.PlanEquals(blockData);
						if (flag3)
						{
							sameSet.Add(index);
						}
					}
					BuildingDomainMethod.Call.ConfirmPlanBuilding(this._operateRecord, new Location(this._areaId, this._blockId), sameSet.ToList<int>());
					BuildingDomainMethod.Call.GetBuildingBlockList(this.Element.GameDataListenerId, new Location(this._areaId, this._blockId));
					this._isKeepMouseWheelScale = true;
				}
				this.FinishPlanBuilding();
			}
			else
			{
				bool flag4 = !confirm && this._isStartPlanning;
				if (flag4)
				{
					bool flag5 = !isSameAll;
					if (flag5)
					{
						ViewBuildingArea.ShowDialog(LocalStringManager.Get(LanguageKey.LK_Building_BuildingPlanCancel), LocalStringManager.Get(LanguageKey.LK_Building_BuildingPlanCancelTip), delegate
						{
							BuildingDomainMethod.Call.GetBuildingBlockList(this.Element.GameDataListenerId, this.CurrentLocation);
							this._isKeepMouseWheelScale = true;
							for (int i = this._operateRecord.Count - 1; i >= 0; i--)
							{
								this.UpDateCustomBuildingName((short)this._operateRecord[i].Second, (short)this._operateRecord[i].First);
							}
							GEvent.OnEvent(UiEvents.CancelPlanOrRemoveBuilding, null);
							this.FinishPlanBuilding();
						}, new Action(this.CancelDialog));
					}
					else
					{
						this.FinishPlanBuilding();
					}
				}
			}
		}

		// Token: 0x06009983 RID: 39299 RVA: 0x0047AE1C File Offset: 0x0047901C
		public void CancelDialog()
		{
			this.SetEscHandler();
		}

		// Token: 0x06009984 RID: 39300 RVA: 0x0047AE28 File Offset: 0x00479028
		public void SetElementStateForPlan(bool startPlan)
		{
			this._isStartPlanning = startPlan;
			this.planBuilding.gameObject.SetActive(startPlan);
			this.buildingAreaInfo.gameObject.SetActive(!startPlan);
			this.settlementRoadAnimation.chickenUiHolder.gameObject.SetActive(!startPlan);
			this.ShowResourceChange(startPlan);
			if (startPlan)
			{
				UIManager.Instance.SetEscHandler(new Action(this.ExitBuildingPlan));
				this.buildingAreaResourceChange.RefreshResourceChangeOnPlan(null);
				GEvent.OnEvent(UiEvents.StartPlanOrRemoveBuilding, null);
			}
			else
			{
				GEvent.OnEvent(UiEvents.CancelPlanOrRemoveBuilding, null);
				UIManager.Instance.SetEscHandler(null);
			}
		}

		// Token: 0x06009985 RID: 39301 RVA: 0x0047AEE0 File Offset: 0x004790E0
		public void StartResetBuildingPlace(BuildingBlockItem item)
		{
			this._buildingPlacementData.CurBuildingBlockItem = item;
			this._buildingPlacementData.CurBuildingIcon = ((item.Width == 1) ? this.buildingIconPrefabSize1 : this.buildingIconPrefabSize2);
			CImage icon = this._buildingPlacementData.CurBuildingIcon;
			icon.transform.Find("MouseRightCancel").gameObject.SetActive(true);
			ViewBuildingArea.SetBuildingIcon(icon, item, false, null);
			icon.gameObject.SetActive(true);
			Vector2 sizeDelta = icon.rectTransform.sizeDelta;
			float offset = sizeDelta.x / 2f - sizeDelta.x / (float)(item.Width * 2);
			this._buildingPlacementData.Offset = new Vector2(-offset, offset);
			this.HighlightCanBuild(item);
			ConchShipCursor.Instance.SetCursorImage("ui9_icon_building_block_operate_2", 0.5f, 0.5f);
			this.SetBuildingIconPointTrigger(false);
		}

		// Token: 0x06009986 RID: 39302 RVA: 0x0047AFC4 File Offset: 0x004791C4
		private void SetBuildingIconPointTrigger(bool value)
		{
			foreach (short key in this._buildingDict.Keys)
			{
				BuildingAreaBlock refers = this._blockRefersDict[key];
				PointerTrigger cursorTrigger = refers.buildingIcon.GetComponent<PointerTrigger>();
				cursorTrigger.enabled = value;
			}
		}

		// Token: 0x06009987 RID: 39303 RVA: 0x0047B03C File Offset: 0x0047923C
		private void ResetBuildingInputHandle()
		{
			bool flag = this._buildingPlacementData.CurIndex < 0;
			if (!flag)
			{
				bool mouseButtonUp = Input.GetMouseButtonUp(0);
				if (mouseButtonUp)
				{
					bool flag2 = (Input.mousePosition - this._buildingPlacementData.MouseClickPos).magnitude >= 5f;
					if (!flag2)
					{
						bool flag3 = !this._buildingPlacementData.CanBuild;
						if (!flag3)
						{
							bool flag4 = this._nowResetBuildingData.Item1 == this._buildingPlacementData.CurIndex;
							if (flag4)
							{
								this.CancelResetBuilding(true);
							}
							else
							{
								this.UnRenderCanBuild((int)this._buildingPlacementData.CurIndex, (int)this._areaData.Width, (int)this._buildingPlacementData.CurBuildingBlockItem.Width);
								this._buildingPlacementData.CurBuildingIcon.gameObject.SetActive(false);
								this.ConfirmResetBuild();
							}
						}
					}
				}
				else
				{
					bool mouseButtonDown = Input.GetMouseButtonDown(0);
					if (mouseButtonDown)
					{
						this._buildingPlacementData.MouseClickPos = Input.mousePosition;
					}
				}
			}
		}

		// Token: 0x06009988 RID: 39304 RVA: 0x0047B150 File Offset: 0x00479350
		public void CancelResetBuilding(bool isMouseCancel)
		{
			this.UnRenderCanBuild((int)this._buildingPlacementData.CurIndex, (int)this._areaData.Width, (int)this._buildingPlacementData.CurBuildingBlockItem.Width);
			this.ResetCanBuild();
			this._buildingPlacementData.Clear();
			this.RecoverBlockData(this._nowResetBuildingData.Item1);
			ConchShipCursor.Instance.SetDefaultCursor();
			base.StartCoroutine(this.CancelResetBuilding());
		}

		// Token: 0x06009989 RID: 39305 RVA: 0x0047B1C8 File Offset: 0x004793C8
		private IEnumerator CancelResetBuilding()
		{
			yield return ViewBuildingArea.WaitForSeconds;
			this._isResetBuilding = false;
			UIManager.Instance.SetEscHandler(new Action(this.ExitBuildingPlan));
			yield break;
		}

		// Token: 0x0600998A RID: 39306 RVA: 0x0047B1D7 File Offset: 0x004793D7
		private IEnumerator DelayAction(Action action)
		{
			yield return ViewBuildingArea.WaitForSeconds;
			if (action != null)
			{
				action();
			}
			yield break;
		}

		// Token: 0x0600998B RID: 39307 RVA: 0x0047B1F0 File Offset: 0x004793F0
		private void ConfirmResetBuild()
		{
			AudioManager.Instance.PlaySound("ui_industry_put", false, false);
			ConchShipCursor.Instance.SetDefaultCursor();
			BuildingBlockKey buildingBlockKey = new BuildingBlockKey(this._areaId, this._blockId, this._buildingPlacementData.CurIndex);
			this._operateRecord.Add(new IntPair((int)this._nowResetBuildingData.Item1, (int)this._buildingPlacementData.CurIndex));
			this.SetNewBlockData(buildingBlockKey.BuildingBlockIndex, this._nowResetBuildingData.Item1);
			this.ResetCanBuild();
			this._isResetBuilding = false;
			UIManager.Instance.SetEscHandler(new Action(this.ExitBuildingPlan));
			this._buildingPlacementData.Clear();
			int[] costArray = new int[8];
			HashSet<int> realMovedBuildingIndexSet = (from p in this._operateRecord
			where ViewBuildingArea.BlockList[p.Second].TemplateId != 0
			select p.Second).Distinct<int>().ToHashSet<int>();
			foreach (int index in realMovedBuildingIndexSet)
			{
				BuildingBlockData blockData = ViewBuildingArea.BlockList[index];
				BuildingBlockData originBlockData = this._blockListCopy[index];
				bool flag = originBlockData.PlanEquals(blockData);
				if (!flag)
				{
					bool flag2 = blockData == null || blockData.ConfigData == null;
					if (!flag2)
					{
						sbyte costRate = blockData.ConfigData.MoveBuildCostResourceRate;
						for (int i = 0; i < blockData.ConfigData.BaseBuildCost.Length; i++)
						{
							int amount = (int)(blockData.ConfigData.BaseBuildCost[i] * (ushort)costRate / 100);
							costArray[i] += amount;
						}
					}
				}
			}
			this._isPlanBuildingResourceMeet = this.buildingAreaResourceChange.RefreshResourceChangeOnPlan(costArray);
			bool isSameAll = this.PlanBuildingIsSameAll();
			this.buttonConfirmPlanBuilding.interactable = (!isSameAll && this._isPlanBuildingResourceMeet);
		}

		// Token: 0x0600998C RID: 39308 RVA: 0x0047B420 File Offset: 0x00479620
		public short GetOriginalIndex(short currentIndex)
		{
			List<IntPair> record = this._operateRecord;
			bool flag = record == null || record.Count == 0;
			short result2;
			if (flag)
			{
				result2 = currentIndex;
			}
			else
			{
				short result = currentIndex;
				for (int i = record.Count - 1; i >= 0; i--)
				{
					int num;
					int num2;
					record[i].Deconstruct(out num, out num2);
					int beforeMove = num;
					int afterMove = num2;
					bool flag2 = afterMove == (int)result;
					if (flag2)
					{
						result = (short)beforeMove;
					}
				}
				result2 = result;
			}
			return result2;
		}

		// Token: 0x0600998D RID: 39309 RVA: 0x0047B4A4 File Offset: 0x004796A4
		public void SetBuildingLevelText(TextMeshProUGUI text, BuildingBlockData blockData)
		{
			BuildingBlockKey buildingBlockKey = this.GetBuildingBlockKeyAtPlanning(blockData);
			text.text = this.BuildingModel.GetBuildingLevel(buildingBlockKey, blockData).ToString();
		}

		// Token: 0x0600998E RID: 39310 RVA: 0x0047B4D8 File Offset: 0x004796D8
		public BuildingBlockKey GetBuildingBlockKeyAtPlanning(BuildingBlockData blockData)
		{
			BuildingBlockKey buildingBlockKey = new BuildingBlockKey(this._areaId, this._blockId, blockData.BlockIndex);
			bool isStartPlanning = this._isStartPlanning;
			if (isStartPlanning)
			{
				short originalIndex = this.GetOriginalIndex(blockData.BlockIndex);
				buildingBlockKey = new BuildingBlockKey(this._areaId, this._blockId, originalIndex);
			}
			return buildingBlockKey;
		}

		// Token: 0x0600998F RID: 39311 RVA: 0x0047B534 File Offset: 0x00479734
		public void StartMultiplyRemoveBuilding()
		{
			this._multiplyRemoveBuildingSelectionSet.Clear();
			this._multiplyRemoveBuildingWorkerDict.Clear();
			this.RefreshMultiplyRemove(new Action<bool>(this.SetBlockCannotRemove));
			this.ShowResourceChange(true);
			this.SetElementStateForRemove(true);
			this.multiplyRemove.gameObject.SetActive(true);
		}

		// Token: 0x06009990 RID: 39312 RVA: 0x0047B590 File Offset: 0x00479790
		private void StopMultiplyRemoveBuilding()
		{
			foreach (BuildingAreaBlock refers in this._multiplyRemoveBuildingSelectionSet)
			{
				this.UpdateMultiplyRemoveBuildingOperationInfo(refers, false);
			}
			this._multiplyRemoveBuildingSelectionSet.Clear();
			this._multiplyRemoveBuildingWorkerDict.Clear();
			this.ShowResourceChange(false);
			this.SetElementStateForRemove(false);
			this.CancelBlockCannotRemove();
			this.SetRoadActive(true);
			this.multiplyRemove.gameObject.SetActive(false);
		}

		// Token: 0x06009991 RID: 39313 RVA: 0x0047B634 File Offset: 0x00479834
		public void SetElementStateForRemove(bool state)
		{
			this._isStartMultiplyRemove = state;
			this.buildingAreaInfo.gameObject.SetActive(!state);
			if (state)
			{
				GEvent.OnEvent(UiEvents.StartPlanOrRemoveBuilding, null);
				UIManager.Instance.SetEscHandler(new Action(this.CancelMultiplyRemove));
			}
			else
			{
				GEvent.OnEvent(UiEvents.CancelPlanOrRemoveBuilding, null);
				UIManager.Instance.SetEscHandler(null);
			}
		}

		// Token: 0x06009992 RID: 39314 RVA: 0x0047B6A8 File Offset: 0x004798A8
		private void SetBlockCannotRemove(bool workerAvailable)
		{
			foreach (KeyValuePair<short, BuildingBlockData> pair in this._buildingDict)
			{
				BuildingBlockData blockData = pair.Value;
				BuildingBlockItem blockConfig = BuildingBlock.Instance.GetItem(blockData.TemplateId);
				BuildingAreaBlock block = this._blockRefersDict[pair.Key];
				block.leftTopHolder.gameObject.SetActive(false);
				block.getEarnHolder.gameObject.SetActive(false);
				bool canRemove = ViewBuildingArea.CanRemove(blockConfig.TemplateId, true);
				List<short> blockIndexList = (from b in this._multiplyRemoveBuildingSelectionSet
				select b.buildingBlockIndex).ToList<short>();
				bool flag = blockIndexList.Contains(pair.Key);
				if (flag)
				{
					this.SetBuildingBlockInteractable(block, true);
				}
				else
				{
					bool flag2 = !canRemove || !workerAvailable;
					if (flag2)
					{
						this.SetBuildingBlockInteractable(block, false);
						block.selectTip.enabled = false;
					}
					else
					{
						bool flag3 = blockData.OperationType != -1;
						if (flag3)
						{
							this.SetBuildingBlockInteractable(block, false);
							block.selectTip.enabled = false;
						}
						else
						{
							this.SetBuildingBlockInteractable(block, true);
							block.selectTip.enabled = true;
						}
					}
				}
			}
			this.SetRoadActive(false);
		}

		// Token: 0x06009993 RID: 39315 RVA: 0x0047B83C File Offset: 0x00479A3C
		private void CancelBlockCannotRemove()
		{
			foreach (KeyValuePair<short, BuildingAreaBlock> keyValuePair in this._blockRefersDict)
			{
				short num;
				BuildingAreaBlock buildingAreaBlock;
				keyValuePair.Deconstruct(out num, out buildingAreaBlock);
				short key = num;
				BuildingAreaBlock blockRefers = buildingAreaBlock;
				BuildingBlockData blockData;
				bool flag = !this._buildingDict.TryGetValue(key, out blockData);
				if (!flag)
				{
					BuildingBlockItem blockConfig = BuildingBlock.Instance.GetItem(blockData.TemplateId);
					bool interactable = this.BuildingBlockCanInteractable(blockData, blockConfig);
					this.SetBuildingBlockInteractable(blockRefers, interactable);
					blockRefers.selectTip.enabled = true;
					blockRefers.leftTopHolder.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x06009994 RID: 39316 RVA: 0x0047B904 File Offset: 0x00479B04
		private void ConfirmMultiplyRemove()
		{
			AudioManager.Instance.PlaySound("ui_industry_dismantle", false, false);
			foreach (BuildingAreaBlock refers in this._multiplyRemoveBuildingSelectionSet)
			{
				short blockIndex = refers.buildingBlockIndex;
				BuildingBlockKey blockKey = this.MakeKey(blockIndex);
				BuildingBlockData blockData = ViewBuildingArea.BlockList[(int)blockIndex];
				blockData.OperationType = 1;
				BuildingDomainMethod.Call.Remove(this.Element.GameDataListenerId, blockKey, this._multiplyRemoveBuildingWorkerDict[blockIndex].ToArray());
			}
			this.StopMultiplyRemoveBuilding();
		}

		// Token: 0x06009995 RID: 39317 RVA: 0x0047B9B8 File Offset: 0x00479BB8
		private void CancelMultiplyRemove()
		{
			AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			this.StopMultiplyRemoveBuilding();
		}

		// Token: 0x06009996 RID: 39318 RVA: 0x0047B9D4 File Offset: 0x00479BD4
		private void RefreshMultiplyRemove(Action<bool> action = null)
		{
			int tempWorkerCount = this._multiplyRemoveBuildingWorkerDict.Sum(delegate(KeyValuePair<short, List<int>> p)
			{
				List<int> value = p.Value;
				int result;
				if (value == null)
				{
					result = 0;
				}
				else
				{
					result = value.Count((int id) => id >= 0);
				}
				return result;
			});
			int[] list = new int[8];
			BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
			foreach (BuildingAreaBlock refers in this._multiplyRemoveBuildingSelectionSet)
			{
				short blockIndex = refers.buildingBlockIndex;
				BuildingBlockKey blockKey = this.MakeKey(blockIndex);
				BuildingBlockData blockData = ViewBuildingArea.BlockList[(int)blockIndex];
				sbyte level = buildingModel.GetBuildingLevel(blockKey, blockData);
				sbyte resourceType = 0;
				while ((int)resourceType < blockData.ConfigData.BaseBuildCost.Length)
				{
					int count = GameData.Domains.Building.SharedMethods.GetResourceReturnOfRemoveBuilding(blockData.ConfigData, level, resourceType, blockData);
					list[(int)resourceType] += count;
					resourceType += 1;
				}
			}
			TaiwuDomainMethod.AsyncCall.GetVillagersForWork(this, delegate(int offset, RawDataPool pool)
			{
				List<int> charList = new List<int>();
				Serializer.Deserialize(pool, offset, ref charList);
				int remainCount = charList.Count - tempWorkerCount;
				this.buildingAreaResourceChange.RefreshResourceChangeOnRemove(remainCount, list);
				Action<bool> action2 = action;
				if (action2 != null)
				{
					action2(remainCount > 0);
				}
			});
		}

		// Token: 0x06009997 RID: 39319 RVA: 0x0047BB10 File Offset: 0x00479D10
		private void ChangeMultiplyRemoveBuilding(BuildingAreaBlock block)
		{
			List<int> curAllWorkList = this._multiplyRemoveBuildingWorkerDict.Values.SelectMany((List<int> l) => l).ToList<int>();
			BuildingBlockData blockData = ViewBuildingArea.BlockList[(int)block.buildingBlockIndex];
			BuildingBlockKey blockKey = this.MakeKey(blockData.BlockIndex);
			bool flag = this._multiplyRemoveBuildingSelectionSet.Add(block);
			if (flag)
			{
				this.UpdateMultiplyRemoveBuildingOperationInfo(block, true);
				List<int> works;
				bool flag2 = !this._multiplyRemoveBuildingWorkerDict.TryGetValue(blockData.BlockIndex, out works);
				if (flag2)
				{
					works = new List<int>();
					this._multiplyRemoveBuildingWorkerDict.Add(blockData.BlockIndex, works);
				}
				works.Clear();
				BuildingDomainMethod.AsyncCall.QuickArrangeBuildOperator(this, blockData.TemplateId, blockKey, 1, curAllWorkList, delegate(int offset, RawDataPool pool)
				{
					Serializer.Deserialize(pool, offset, ref works);
					this.RefreshMultiplyRemove(new Action<bool>(this.SetBlockCannotRemove));
				});
			}
			else
			{
				this._multiplyRemoveBuildingSelectionSet.Remove(block);
				this._multiplyRemoveBuildingWorkerDict.Remove(blockData.BlockIndex);
				this.RefreshMultiplyRemove(new Action<bool>(this.SetBlockCannotRemove));
				this.UpdateMultiplyRemoveBuildingOperationInfo(block, false);
			}
			this.buttonConfirmMultiplyRemove.interactable = (this._multiplyRemoveBuildingSelectionSet.Count > 0);
		}

		// Token: 0x06009998 RID: 39320 RVA: 0x0047BC69 File Offset: 0x00479E69
		private void UpdateMultiplyRemoveBuildingOperationInfo(BuildingAreaBlock block, bool isSelected)
		{
			block.multiplyRemoveSelected.gameObject.SetActive(isSelected);
		}

		// Token: 0x06009999 RID: 39321 RVA: 0x0047BC80 File Offset: 0x00479E80
		private static bool CanRemove(short templateId, bool isTaiwuVillageBuilding)
		{
			BuildingBlockItem config = BuildingBlock.Instance[templateId];
			return config.Class != EBuildingBlockClass.Static && config.OperationTotalProgress[1] >= 0 && isTaiwuVillageBuilding;
		}

		// Token: 0x0600999A RID: 39322 RVA: 0x0047BCB9 File Offset: 0x00479EB9
		private void ShowResourceChange(bool show)
		{
			this.buildingAreaResourceChange.gameObject.SetActive(show);
		}

		// Token: 0x0600999B RID: 39323 RVA: 0x0047BCD0 File Offset: 0x00479ED0
		public static void SetBuildingIcon(CImage image, BuildingBlockItem configData, bool setNativeSize = false, Action onSet = null)
		{
			bool flag = configData.TemplateId == 0;
			if (!flag)
			{
				string path = "Building/" + configData.Icon;
				ResLoader.LoadModOrGameResource<Sprite>(path, delegate(Sprite sprite)
				{
					image.sprite = sprite;
					image.enabled = true;
					bool setNativeSize2 = setNativeSize;
					if (setNativeSize2)
					{
						image.SetNativeSize();
					}
					Action onSet2 = onSet;
					if (onSet2 != null)
					{
						onSet2();
					}
				}, null);
			}
		}

		// Token: 0x0600999C RID: 39324 RVA: 0x0047BD30 File Offset: 0x00479F30
		public static void SetTexturePicture(CImage image, string path, bool setNativeSize = false)
		{
			ResLoader.Load<Sprite>(path, delegate(Sprite sprite)
			{
				image.sprite = sprite;
				image.enabled = true;
				bool setNativeSize2 = setNativeSize;
				if (setNativeSize2)
				{
					image.SetNativeSize();
				}
			}, null, false);
		}

		// Token: 0x0600999D RID: 39325 RVA: 0x0047BD68 File Offset: 0x00479F68
		public static void SetBuildingName(TextMeshProUGUI text, BuildingBlockItem configData, BuildingBlockKey blockKey, short mapBlockTemplateId, bool showOriginName = false)
		{
			bool flag = configData == null;
			if (!flag)
			{
				WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
				BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
				bool isTaiwuVillage = mapModel.IsAtTaiwuVillage(blockKey.AreaId, blockKey.BlockId);
				string name = (configData.Type == EBuildingBlockType.MainBuilding) ? mapModel.GetBlockName(blockKey.AreaId, blockKey.BlockId, mapBlockTemplateId, -1) : configData.Name;
				bool flag2 = isTaiwuVillage && buildingModel.CustomBuildingName.ContainsKey(blockKey);
				if (flag2)
				{
					Dictionary<int, string> customTexts = SingletonObject.getInstance<BasicGameData>().CustomTexts;
					bool flag3 = customTexts.ContainsKey(buildingModel.CustomBuildingName[blockKey]);
					if (flag3)
					{
						string customName = SingletonObject.getInstance<BasicGameData>().CustomTexts[buildingModel.CustomBuildingName[blockKey]];
						name = (showOriginName ? LocalStringManager.GetFormat(LanguageKey.LK_Custom_Building_Name_Format, customName, name) : customName);
					}
				}
				bool flag4 = configData.Type == EBuildingBlockType.MainBuilding;
				string nameColor;
				if (flag4)
				{
					nameColor = "C16927";
				}
				else
				{
					bool flag5 = configData.Type == EBuildingBlockType.NormalResource;
					if (flag5)
					{
						nameColor = "B5DEDE";
					}
					else
					{
						bool flag6 = configData.Type == EBuildingBlockType.SpecialResource;
						if (flag6)
						{
							nameColor = "ffe78f";
						}
						else
						{
							bool flag7 = configData.Type == EBuildingBlockType.UselessResource;
							if (flag7)
							{
								nameColor = "939393";
							}
							else
							{
								bool flag8 = ViewBuildingArea.SectSpecialBuildingIdList.Contains(configData.TemplateId) || configData.TemplateId == 45 || configData.TemplateId == 51 || configData.TemplateId == 50 || configData.TemplateId == 49 || (configData.TemplateId >= 276 && configData.TemplateId <= 282) || configData.TemplateId == 283 || (configData.TemplateId >= 284 && configData.TemplateId <= 302) || (configData.TemplateId >= 303 && configData.TemplateId <= 317);
								if (flag8)
								{
									nameColor = "B975FF";
								}
								else
								{
									bool flag9 = configData.Type == EBuildingBlockType.Building;
									if (flag9)
									{
										nameColor = "F8E0CA";
									}
									else
									{
										nameColor = "lightbrown";
									}
								}
							}
						}
					}
				}
				text.text = name.SetColor(nameColor);
				text.SetAllDirty();
			}
		}

		// Token: 0x0600999E RID: 39326 RVA: 0x0047BF94 File Offset: 0x0047A194
		public static string GetBuildingName(BuildingBlockItem configData, BuildingBlockKey blockKey, short mapBlockTemplateId, bool showOriginName = false)
		{
			WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
			BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
			bool isTaiwuVillage = mapModel.IsAtTaiwuVillage(blockKey.AreaId, blockKey.BlockId);
			string name = (configData.Type == EBuildingBlockType.MainBuilding) ? mapModel.GetBlockName(blockKey.AreaId, blockKey.BlockId, mapBlockTemplateId, -1) : configData.Name;
			bool flag = isTaiwuVillage && buildingModel.CustomBuildingName.ContainsKey(blockKey);
			if (flag)
			{
				Dictionary<int, string> customTexts = SingletonObject.getInstance<BasicGameData>().CustomTexts;
				bool flag2 = customTexts.ContainsKey(buildingModel.CustomBuildingName[blockKey]);
				if (flag2)
				{
					string customName = SingletonObject.getInstance<BasicGameData>().CustomTexts[buildingModel.CustomBuildingName[blockKey]];
					name = (showOriginName ? LocalStringManager.GetFormat(LanguageKey.LK_Custom_Building_Name_Format, customName, name) : customName);
				}
			}
			return name;
		}

		// Token: 0x0600999F RID: 39327 RVA: 0x0047C064 File Offset: 0x0047A264
		public static void ShowDialog(string title, string content, Action yesAction, Action noAction)
		{
			DialogCmd cmd = new DialogCmd
			{
				Title = title,
				Content = content,
				Yes = delegate()
				{
					Action yesAction2 = yesAction;
					if (yesAction2 != null)
					{
						yesAction2();
					}
				},
				No = delegate()
				{
					Action noAction2 = noAction;
					if (noAction2 != null)
					{
						noAction2();
					}
				},
				GroupYesText = LocalStringManager.Get(LanguageKey.LK_HotKeyGroup_Common_Confirm),
				GroupNoText = LocalStringManager.Get(LanguageKey.LK_HotKeyGroup_Common_Cancel)
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x060099A0 RID: 39328 RVA: 0x0047C10C File Offset: 0x0047A30C
		public static string GetBuildingAnimationPrefabPath(short templateId)
		{
			string iconPath = BuildingBlock.Instance[templateId].Icon;
			string iconName = iconPath.Contains('/') ? iconPath.Substring(iconPath.LastIndexOf('/') + 1) : iconPath;
			return "RemakeResources/SpineAnimations/Building/AnimationPrefab/" + iconName + "_AnimationPrefab";
		}

		// Token: 0x060099A1 RID: 39329 RVA: 0x0047C160 File Offset: 0x0047A360
		public static bool IsShowAnimation(short templateId)
		{
			return ViewBuildingArea.SectBuildingIdList.Contains(templateId) || ViewBuildingArea.CityBuildingIdList.Contains(templateId) || ViewBuildingArea.TaiwuBuildingIdList.Contains(templateId);
		}

		// Token: 0x060099A2 RID: 39330 RVA: 0x0047C19C File Offset: 0x0047A39C
		public static string GetBuildingExceptionString(BuildingExceptionType exceptionType)
		{
			if (!true)
			{
			}
			LanguageKey languageKey;
			switch (exceptionType)
			{
			case BuildingExceptionType.ManageStoppedForDependency:
				languageKey = LanguageKey.LK_Building_ExceptionTip_ManageStoppedForDependency;
				break;
			case BuildingExceptionType.ManageStoppedForNoLeader:
				languageKey = LanguageKey.LK_Building_ExceptionTip_ManageStoppedForNoLeader;
				break;
			case BuildingExceptionType.ComfortableHouseEntertainNoFood:
				languageKey = LanguageKey.LK_Building_ExceptionTip_ComfortableHouseEntertainNoFood_Long;
				break;
			case BuildingExceptionType.LearnException:
				languageKey = LanguageKey.Lk_Building_Exception_Learn;
				break;
			case BuildingExceptionType.BuildStoppedForWorkerShortage:
				languageKey = LanguageKey.LK_Building_ExceptionTip_BuildStoppedForWorkerShortage;
				break;
			case BuildingExceptionType.DemolishStoppedForWorkerShortage:
				languageKey = LanguageKey.LK_Building_ExceptionTip_DemolishStoppedForWorkerShortage;
				break;
			case BuildingExceptionType.EffectStoppedForDependency:
				languageKey = LanguageKey.LK_Building_ExceptionTip_EffectStoppedForDependency;
				break;
			case BuildingExceptionType.Damaged:
				languageKey = LanguageKey.LK_Building_ExceptionTip_Damaged;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!true)
			{
			}
			LanguageKey strKey = languageKey;
			return strKey.Tr();
		}

		// Token: 0x040075C5 RID: 30149
		[SerializeField]
		private RectTransform buildingBlockHolder;

		// Token: 0x040075C6 RID: 30150
		[SerializeField]
		private BuildingInfoHolder buildingInfoHolder;

		// Token: 0x040075C7 RID: 30151
		[SerializeField]
		private RectTransform moveAndScaleRoot;

		// Token: 0x040075C8 RID: 30152
		[SerializeField]
		private CImage showMask;

		// Token: 0x040075C9 RID: 30153
		[SerializeField]
		private RectTransform border;

		// Token: 0x040075CA RID: 30154
		[SerializeField]
		private CImage buildingIconPrefabSize1;

		// Token: 0x040075CB RID: 30155
		[SerializeField]
		private CImage buildingIconPrefabSize2;

		// Token: 0x040075CC RID: 30156
		[SerializeField]
		private RectTransform mouseRightCancel1;

		// Token: 0x040075CD RID: 30157
		[SerializeField]
		private RectTransform mouseRightCancel2;

		// Token: 0x040075CE RID: 30158
		[Header("天气")]
		[SerializeField]
		private MapWeather mapWeather;

		// Token: 0x040075CF RID: 30159
		[Header("建筑建造特效")]
		[SerializeField]
		private GameObject createBuildingEffectSmall;

		// Token: 0x040075D0 RID: 30160
		[SerializeField]
		private GameObject createBuildingDoneEffectSmall;

		// Token: 0x040075D1 RID: 30161
		[SerializeField]
		private GameObject createBuildingEffectLarge;

		// Token: 0x040075D2 RID: 30162
		[SerializeField]
		private GameObject createBuildingDoneEffectLarge;

		// Token: 0x040075D3 RID: 30163
		private const string RoadPrefabKey = "ui_BuildingArea_RoadPrefab";

		// Token: 0x040075D4 RID: 30164
		private const string ChickenPrefabKey = "ui_BuildingArea_ChickenPrefab";

		// Token: 0x040075D5 RID: 30165
		private const string VillagerPrefabKey = "ui_BuildingArea_VillagerPrefab";

		// Token: 0x040075D6 RID: 30166
		private const string GetItemPrefabKey = "GetItemPrefab";

		// Token: 0x040075D7 RID: 30167
		private const string GetPeoplePrefabKey = "GetPeoplePrefab";

		// Token: 0x040075D8 RID: 30168
		private const string GetResourcePrefabKey = "GetResourcePrefab";

		// Token: 0x040075D9 RID: 30169
		private const string ShopTipPrefabKey = "ShopTipPrefab";

		// Token: 0x040075DA RID: 30170
		private const string ExceptionPrefabKey = "ExceptionPrefab";

		// Token: 0x040075DB RID: 30171
		private const string DamageInfoPrefabKey = "DamageInfoPrefab";

		// Token: 0x040075DC RID: 30172
		private const string ResidentsInfoPrefabKey = "ResidentsInfoPrefab";

		// Token: 0x040075DD RID: 30173
		private const string SamsaraInfoPrefabKey = "SamsaraInfoPrefab";

		// Token: 0x040075DE RID: 30174
		private const string BuildingOperatePrefabKey = "BuildingOperatePrefab";

		// Token: 0x040075DF RID: 30175
		private const string LevelAndNamePrefabKey = "LevelAndNamePrefab";

		// Token: 0x040075E0 RID: 30176
		private const string TreasuryResourceInfoPrefabKey = "TreasuryResourceInfoPrefab";

		// Token: 0x040075E1 RID: 30177
		private const string CoreProducingCooldownPrefabKey = "CoreProducingCooldownPrefab";

		// Token: 0x040075E2 RID: 30178
		private const string TeaHorseCaravanPrefabKey = "TeaHorseCaravanPrefab";

		// Token: 0x040075E3 RID: 30179
		private const string BuildingAnimationPrefabPath = "RemakeResources/SpineAnimations/Building/AnimationPrefab/";

		// Token: 0x040075E4 RID: 30180
		private readonly List<GameObject> _animationGoList = new List<GameObject>();

		// Token: 0x040075E5 RID: 30181
		private static readonly List<short> SectSpecialBuildingIdList = new List<short>
		{
			239,
			244,
			240,
			241,
			242,
			243,
			245,
			246,
			247,
			248,
			249,
			250,
			251,
			252,
			253,
			259,
			260,
			261,
			262,
			263,
			264,
			265,
			266,
			267,
			268,
			269,
			270,
			271,
			272,
			273
		};

		// Token: 0x040075E6 RID: 30182
		private static readonly List<short> CityBuildingIdList = new List<short>
		{
			224,
			225,
			226,
			227,
			228,
			229,
			230,
			231,
			232,
			233,
			234,
			235,
			236,
			237,
			238,
			254,
			255,
			256
		};

		// Token: 0x040075E7 RID: 30183
		private static readonly List<short> TaiwuBuildingIdList = new List<short>
		{
			44,
			257,
			258
		};

		// Token: 0x040075E8 RID: 30184
		private static readonly List<short> SectBuildingIdList = new List<short>
		{
			239,
			240,
			241,
			242,
			243,
			244,
			245,
			246,
			247,
			248,
			249,
			250,
			251,
			252,
			253
		};

		// Token: 0x040075E9 RID: 30185
		private bool haveWarehouse;

		// Token: 0x040075EA RID: 30186
		private readonly Vector2 initialScale = new Vector2(0.65f, 0.65f);

		// Token: 0x040075EB RID: 30187
		private const int AreaSizePadding = 180;

		// Token: 0x040075EC RID: 30188
		private const int BuildingBlockSize = 380;

		// Token: 0x040075ED RID: 30189
		private const int RoadWidth = 50;

		// Token: 0x040075EE RID: 30190
		private static readonly Vector2 SizeTwoV2 = new Vector2(210f, -210f);

		// Token: 0x040075EF RID: 30191
		public float localScaleMax = 1.2f;

		// Token: 0x040075F0 RID: 30192
		private const int SizeOnePrefabMaxCount = 324;

		// Token: 0x040075F1 RID: 30193
		private const int SizeTwoPrefabMaxCount = 81;

		// Token: 0x040075F2 RID: 30194
		public Vector2 scaleMinMaxNameLabel = new Vector2(1.65f, 1f);

		// Token: 0x040075F3 RID: 30195
		public Vector2 scaleMinMaxLeftTop = new Vector2(2f, 1f);

		// Token: 0x040075F4 RID: 30196
		private bool isSecretVilliage;

		// Token: 0x040075F5 RID: 30197
		[SerializeField]
		private float cameraZoomScale = 1f;

		// Token: 0x040075F6 RID: 30198
		[SerializeField]
		private float cameraZoomScaleForSize2 = 1f;

		// Token: 0x040075F7 RID: 30199
		[SerializeField]
		private float cameraOffsetX = -850f;

		// Token: 0x040075F8 RID: 30200
		[SerializeField]
		private float cameraOffsetY = 250f;

		// Token: 0x040075F9 RID: 30201
		[SerializeField]
		private float cameraMoveDuration = 0.2f;

		// Token: 0x040075FA RID: 30202
		[Header("快速操作菜单镜头调整")]
		[Tooltip("保证轮盘在屏幕内的参数")]
		[SerializeField]
		private float quickActionMenuEdgeDistanceThreshold = 1200f;

		// Token: 0x040075FB RID: 30203
		[Tooltip("镜头调整持续时间")]
		[SerializeField]
		private float quickActionMenuCameraMoveDuration = 0.15f;

		// Token: 0x040075FC RID: 30204
		[Tooltip("快速操作菜单基准缩放（将被当前缩放覆盖）")]
		[SerializeField]
		private float quickActionMenuBaseScale = 1f;

		// Token: 0x040075FD RID: 30205
		private short _areaId;

		// Token: 0x040075FE RID: 30206
		private short _blockId;

		// Token: 0x040075FF RID: 30207
		private bool _defaultOpenBuildingOverview;

		// Token: 0x04007600 RID: 30208
		private short _autoSelectBuildingTemplateId;

		// Token: 0x04007601 RID: 30209
		private bool _needClearBuildingOverviewFilter;

		// Token: 0x04007602 RID: 30210
		private short _settlementId;

		// Token: 0x04007603 RID: 30211
		private Location _rootLocation;

		// Token: 0x04007604 RID: 30212
		private bool _isTaiwuVillage;

		// Token: 0x04007605 RID: 30213
		private bool _isBambooHouse;

		// Token: 0x04007606 RID: 30214
		private bool _isSectLocation;

		// Token: 0x04007607 RID: 30215
		private bool _isCityLocation;

		// Token: 0x04007608 RID: 30216
		public bool canUseBuilding;

		// Token: 0x04007609 RID: 30217
		private string _customName;

		// Token: 0x0400760A RID: 30218
		private BuildingBlockKey _placeBlockKey;

		// Token: 0x0400760B RID: 30219
		private BuildingAreaData _areaData = new BuildingAreaData();

		// Token: 0x0400760C RID: 30220
		public static List<BuildingBlockData> BlockList;

		// Token: 0x0400760D RID: 30221
		private Dictionary<short, BuildingBlockData> _buildingDict;

		// Token: 0x0400760E RID: 30222
		public bool isPlacingBuildingNow;

		// Token: 0x0400760F RID: 30223
		private readonly bool _dialogIsOpen = false;

		// Token: 0x04007610 RID: 30224
		private bool _isCameraMoving = false;

		// Token: 0x04007611 RID: 30225
		private Vector3 _savedMoveRootScale;

		// Token: 0x04007612 RID: 30226
		private Vector2 _savedMoveRootAnchoredPos;

		// Token: 0x04007613 RID: 30227
		private bool _savedDragEnabled;

		// Token: 0x04007614 RID: 30228
		private bool _savedMouseWheelEnabled;

		// Token: 0x04007615 RID: 30229
		private bool _savedMouseMoveEnabled;

		// Token: 0x04007616 RID: 30230
		private bool _hasSavedCameraState;

		// Token: 0x04007617 RID: 30231
		private Vector2 _savedQuickActionCameraPos;

		// Token: 0x04007618 RID: 30232
		private bool _hasSavedQuickActionCameraState = false;

		// Token: 0x04007619 RID: 30233
		private readonly Dictionary<short, BuildingAreaBlock> _blockRefersDict = new Dictionary<short, BuildingAreaBlock>(324);

		// Token: 0x0400761A RID: 30234
		private readonly List<GameObject> _roadObjList = new List<GameObject>();

		// Token: 0x0400761B RID: 30235
		private readonly Dictionary<Vector2Int, Vector2Int> _roadCrossPos = new Dictionary<Vector2Int, Vector2Int>();

		// Token: 0x0400761C RID: 30236
		private RectTransform _moveRoot;

		// Token: 0x0400761D RID: 30237
		private MapBlockData _mapBlockData;

		// Token: 0x0400761E RID: 30238
		private StringBuilder _stringBuilder;

		// Token: 0x0400761F RID: 30239
		private StringBuilder _stringBuilder2;

		// Token: 0x04007620 RID: 30240
		private readonly Color _canBuildColor = new Color(1f, 1f, 1f, 1f);

		// Token: 0x04007621 RID: 30241
		private readonly Color _invalidColor = new Color(0f, 0f, 0f, 0f);

		// Token: 0x04007622 RID: 30242
		private readonly List<RectTransform> _notScaleElements = new List<RectTransform>();

		// Token: 0x04007623 RID: 30243
		[TupleElementNames(new string[]
		{
			"t",
			"scaleMinMax"
		})]
		private readonly List<ValueTuple<RectTransform, Vector2>> _customScaleElements = new List<ValueTuple<RectTransform, Vector2>>();

		// Token: 0x04007624 RID: 30244
		public const short DependMaxDistance = 2;

		// Token: 0x04007625 RID: 30245
		private bool _isKeepMouseWheelScale;

		// Token: 0x04007626 RID: 30246
		private Action _worldMapPlayerBlockChangeAction;

		// Token: 0x04007627 RID: 30247
		private readonly Dictionary<int, int> _blockDependencyDistanceDict = new Dictionary<int, int>();

		// Token: 0x04007628 RID: 30248
		private ViewBuildingArea.BuildingPlacementData _buildingPlacementData;

		// Token: 0x04007629 RID: 30249
		private readonly Queue<BuildingAreaBlock> _blockListSize1 = new Queue<BuildingAreaBlock>(324);

		// Token: 0x0400762A RID: 30250
		private readonly Queue<BuildingAreaBlock> _blockListSize2 = new Queue<BuildingAreaBlock>(81);

		// Token: 0x0400762B RID: 30251
		private readonly Vector3 _blockListPos = new Vector3(1274f, 8726f, 0f);

		// Token: 0x0400762C RID: 30252
		private bool _isMonthChangeUpdate = false;

		// Token: 0x0400762D RID: 30253
		private static readonly WaitForSeconds WaitForSeconds = new WaitForSeconds(0.1f);

		// Token: 0x0400762E RID: 30254
		private PoolItem _getItemPool;

		// Token: 0x0400762F RID: 30255
		private PoolItem _getPeoplePool;

		// Token: 0x04007630 RID: 30256
		private PoolItem _getResourcePool;

		// Token: 0x04007631 RID: 30257
		private PoolItem _shopTipPool;

		// Token: 0x04007632 RID: 30258
		private PoolItem _levelAndNamePool;

		// Token: 0x04007633 RID: 30259
		private PoolItem _exceptionPool;

		// Token: 0x04007634 RID: 30260
		private PoolItem _buildingOperatePool;

		// Token: 0x04007635 RID: 30261
		private PoolItem _damageInfoPool;

		// Token: 0x04007636 RID: 30262
		private PoolItem _residentsInfoPool;

		// Token: 0x04007637 RID: 30263
		private PoolItem _samsaraInfoPool;

		// Token: 0x04007638 RID: 30264
		private PoolItem _treasuryResourceInfoPool;

		// Token: 0x04007639 RID: 30265
		private PoolItem _coreProducingCooldownPool;

		// Token: 0x0400763A RID: 30266
		private PoolItem _teaHorseCaravanPool;

		// Token: 0x0400763B RID: 30267
		private Dictionary<short, BuildingBlockInfo> _getItemDict;

		// Token: 0x0400763C RID: 30268
		private Dictionary<short, BuildingBlockInfo> _getPeopleDict;

		// Token: 0x0400763D RID: 30269
		private Dictionary<short, BuildingBlockInfo> _getResourceDict;

		// Token: 0x0400763E RID: 30270
		private Dictionary<short, BuildingBlockInfo> _shopTipDict;

		// Token: 0x0400763F RID: 30271
		private Dictionary<short, BuildingBlockInfo> _levelAndNameDict;

		// Token: 0x04007640 RID: 30272
		private Dictionary<short, BuildingBlockInfo> _exceptionDict;

		// Token: 0x04007641 RID: 30273
		private Dictionary<short, BuildingBlockInfo> _buildingOperateDict;

		// Token: 0x04007642 RID: 30274
		private Dictionary<short, BuildingBlockInfo> _damageInfoDict;

		// Token: 0x04007643 RID: 30275
		private Dictionary<short, BuildingBlockInfo> _residentsInfoDict;

		// Token: 0x04007644 RID: 30276
		private Dictionary<short, BuildingBlockInfo> _samsaraInfoDict;

		// Token: 0x04007645 RID: 30277
		private Dictionary<short, BuildingBlockInfo> _treasuryResourceInfoDict;

		// Token: 0x04007646 RID: 30278
		private Dictionary<short, BuildingBlockInfo> _coreProducingCooldownDict;

		// Token: 0x04007647 RID: 30279
		private Dictionary<short, BuildingBlockInfo> _teaHorseCaravanDict;

		// Token: 0x04007648 RID: 30280
		private SettlementTreasuryDisplayData _settlementTreasuryDisplayData;

		// Token: 0x04007649 RID: 30281
		private List<short> _newlyCreatedBuildingIndexList = new List<short>();

		// Token: 0x0400764A RID: 30282
		private bool _waitMonthNotifyProcessComplete;

		// Token: 0x0400764B RID: 30283
		[SerializeField]
		private CImage[] backgroundImgList;

		// Token: 0x0400764C RID: 30284
		[SerializeField]
		private GridLayoutGroup backgroundLayout;

		// Token: 0x0400764D RID: 30285
		[SerializeField]
		private CImage backgroundBack;

		// Token: 0x0400764E RID: 30286
		[SerializeField]
		private Color[] backgroundBackColor = new Color[6];

		// Token: 0x0400764F RID: 30287
		private StringBuilder sb = new StringBuilder();

		// Token: 0x04007650 RID: 30288
		private static readonly Vector2 Width1 = new Vector2(2700f, 2700f);

		// Token: 0x04007651 RID: 30289
		private static readonly Vector2 Width2 = new Vector2(2150f, 2150f);

		// Token: 0x04007652 RID: 30290
		private static readonly Vector2 Width3 = new Vector2(1840f, 1840f);

		// Token: 0x04007653 RID: 30291
		[SerializeField]
		private SettlementRoadAnimation settlementRoadAnimation;

		// Token: 0x04007654 RID: 30292
		[SerializeField]
		private BuildingAreaInfo buildingAreaInfo;

		// Token: 0x04007655 RID: 30293
		private int _buildingSpaceLimit;

		// Token: 0x04007656 RID: 30294
		private int _buildingSpaceCurr;

		// Token: 0x04007657 RID: 30295
		[SerializeField]
		private BuildingExceptionInfo buildingExceptionInfo;

		// Token: 0x04007658 RID: 30296
		[Header("道路")]
		[SerializeField]
		private RectTransform roadHolder;

		// Token: 0x04007659 RID: 30297
		[SerializeField]
		private GameObject road;

		// Token: 0x0400765A RID: 30298
		[SerializeField]
		private RectTransform roadCornerHolder;

		// Token: 0x0400765B RID: 30299
		[SerializeField]
		private bool testRoad;

		// Token: 0x0400765C RID: 30300
		[SerializeField]
		private sbyte testLandFormType;

		// Token: 0x0400765D RID: 30301
		private PoolItem _roadPool;

		// Token: 0x0400765E RID: 30302
		[Header("建筑规划")]
		[SerializeField]
		private GameObject planBuilding;

		// Token: 0x0400765F RID: 30303
		[SerializeField]
		private CButton buttonConfirmPlanBuilding;

		// Token: 0x04007660 RID: 30304
		private bool _isPlanBuildingResourceMeet;

		// Token: 0x04007661 RID: 30305
		private bool _isStartPlanning;

		// Token: 0x04007662 RID: 30306
		private bool _isResetBuilding;

		// Token: 0x04007663 RID: 30307
		private ValueTuple<short, BuildingBlockData> _nowResetBuildingData;

		// Token: 0x04007664 RID: 30308
		private readonly List<BuildingBlockData> _blockListCopy = new List<BuildingBlockData>();

		// Token: 0x04007665 RID: 30309
		private readonly List<IntPair> _operateRecord = new List<IntPair>();

		// Token: 0x04007666 RID: 30310
		private bool _isStartMultiplyRemove;

		// Token: 0x04007667 RID: 30311
		private readonly HashSet<BuildingAreaBlock> _multiplyRemoveBuildingSelectionSet = new HashSet<BuildingAreaBlock>();

		// Token: 0x04007668 RID: 30312
		private readonly Dictionary<short, List<int>> _multiplyRemoveBuildingWorkerDict = new Dictionary<short, List<int>>();

		// Token: 0x04007669 RID: 30313
		[Header("批量撤除")]
		[SerializeField]
		private GameObject multiplyRemove;

		// Token: 0x0400766A RID: 30314
		[SerializeField]
		private CButton buttonConfirmMultiplyRemove;

		// Token: 0x0400766B RID: 30315
		[SerializeField]
		private BuildingAreaResourceChange buildingAreaResourceChange;

		// Token: 0x020022A5 RID: 8869
		private class BuildingPlacementData
		{
			// Token: 0x06010016 RID: 65558 RVA: 0x00649574 File Offset: 0x00647774
			public BuildingPlacementData()
			{
				this.CurBuildingIcon = null;
				this.CurBuildingBlockItem = null;
				this.CurIndex = -1;
				this.CanBuild = false;
				this.Offset = Vector2.zero;
				this.OperatorList = null;
				this.Level = 1;
				this.InstantBuild = false;
			}

			// Token: 0x06010017 RID: 65559 RVA: 0x006495C8 File Offset: 0x006477C8
			public void Clear()
			{
				bool flag = this.CurBuildingIcon != null;
				if (flag)
				{
					this.CurBuildingIcon.gameObject.SetActive(false);
				}
				this.CurBuildingIcon = null;
				this.CurBuildingBlockItem = null;
				this.CurIndex = -1;
				this.CanBuild = false;
				this.Offset = Vector2.zero;
				this.OperatorList = null;
				this.Level = 1;
				this.InstantBuild = false;
				this.MouseClickPos = Vector3.zero;
			}

			// Token: 0x0400DBA9 RID: 56233
			public CImage CurBuildingIcon;

			// Token: 0x0400DBAA RID: 56234
			public BuildingBlockItem CurBuildingBlockItem;

			// Token: 0x0400DBAB RID: 56235
			public short CurIndex;

			// Token: 0x0400DBAC RID: 56236
			public bool CanBuild;

			// Token: 0x0400DBAD RID: 56237
			public Vector2 Offset;

			// Token: 0x0400DBAE RID: 56238
			public int[] OperatorList;

			// Token: 0x0400DBAF RID: 56239
			public sbyte Level;

			// Token: 0x0400DBB0 RID: 56240
			public bool InstantBuild;

			// Token: 0x0400DBB1 RID: 56241
			public Vector3 MouseClickPos;
		}
	}
}
