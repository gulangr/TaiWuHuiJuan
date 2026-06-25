using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.BuildingOverview;
using Game.Views.Building.BuildingManage;
using Game.Views.Buildings.Migrate;
using GameData.Common;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Organization;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Building
{
	// Token: 0x02000BE1 RID: 3041
	public class ViewBuildingOverview : UIBase
	{
		// Token: 0x060099AE RID: 39342 RVA: 0x0047C980 File Offset: 0x0047AB80
		public override void OnInit(ArgumentBox argsBox)
		{
			this.shrineBuild = ViewBuildingArea.HasBuilding(45, true);
			argsBox.Get("isHaveChickenKing", out this._isHaveChickenKing);
			this._autoSelectBuildingAfterRefresh = argsBox.Get("AutoSelectBuildingTemplateId", out this._autoSelectBuildingTemplateId);
			bool needClear;
			this._needClearSortAndFilterOnInit = (argsBox.Get("NeedClearBuildingOverviewFilter", out needClear) && needClear);
			this.confirmLabel.text = LocalStringManager.Get(LanguageKey.LK_Building_Start_Build);
			this.challengeOpen = SingletonObject.getInstance<BasicGameData>().ChallengeModeData.IsEnabled(EChallengeModeImplement.BuildingAreaLimit);
			this.InitSet();
			bool needClearSortAndFilterOnInit = this._needClearSortAndFilterOnInit;
			if (needClearSortAndFilterOnInit)
			{
				BuildingOverviewSortAndFilterController buildingOverviewSortAndFilterController = this._buildingOverviewSortAndFilterController;
				if (buildingOverviewSortAndFilterController != null)
				{
					buildingOverviewSortAndFilterController.ClearAllFilter();
				}
			}
			this.SpecialSetting();
			this.InitData();
			this.ResetState();
			this.SetToggleState(false);
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
		}

		// Token: 0x060099AF RID: 39343 RVA: 0x0047CA70 File Offset: 0x0047AC70
		private void SpecialSetting()
		{
			TutorialChapterModel tutorialInstance = SingletonObject.getInstance<TutorialChapterModel>();
			bool flag = tutorialInstance.InGuiding && tutorialInstance.TutorialChapterIndex == 1;
			if (flag)
			{
				this.leftTypeToggleGroup.Get(1).GetComponent<BuildingOverviewToggle>().txtName.SetText(LocalStringManager.Get(LanguageKey.LK_Tutorial_TutorialAreaName), true);
			}
			else
			{
				this.leftTypeToggleGroup.Get(1).GetComponent<BuildingOverviewToggle>().txtName.SetText(LocalStringManager.Get(LanguageKey.LK_Building_Overview_Filter_Village), true);
			}
		}

		// Token: 0x060099B0 RID: 39344 RVA: 0x0047CAF4 File Offset: 0x0047ACF4
		private void InitSet()
		{
			this._buildingArea = UIElement.BuildingArea.UiBaseAs<ViewBuildingArea>();
			this._buildingModel = SingletonObject.getInstance<BuildingModel>();
			bool isPlacingBuildingNow = this._buildingArea.isPlacingBuildingNow;
			if (isPlacingBuildingNow)
			{
				this._buildingArea.CancelPlaceBuilding(false);
			}
			this._dependNormalBuildingsLack = false;
			this._dependResourcesBuildingsLack = false;
			bool flag = !PoolManager.HasData("UI_BuildingOverview_BuildingTypeObject");
			if (flag)
			{
				PoolManager.SetSrcObject("UI_BuildingOverview_BuildingTypeObject", this.typeBuildingPrefab.gameObject);
			}
			bool flag2 = !PoolManager.HasData("UI_BuildingOverview_BuildingItemObject");
			if (flag2)
			{
				PoolManager.SetSrcObject("UI_BuildingOverview_BuildingItemObject", this.buildingPrefabTemplate.gameObject);
			}
			this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			this._settlementId = SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageSettlementId();
			for (int i = 0; i < 16; i++)
			{
				int index = i + 5;
				CToggle tog = this.leftTypeToggleGroup.Get(index);
				BuildingOverviewToggle refer = tog.GetComponent<BuildingOverviewToggle>();
				LifeSkillTypeItem config = Config.LifeSkillType.Instance[i];
				refer.txtName.text = config.Name;
				string sp = CommonUtils.GetFilterLifeSkillTypeIcon(i);
				refer.icon.SetSprite(sp, false, null);
			}
			this.title.text = LocalStringManager.Get(LanguageKey.LK_BuildingClassName_Overview);
			Vector3 position = this.content.anchoredPosition3D;
			this.content.anchoredPosition3D = new Vector3(position.x, 0f, position.z);
		}

		// Token: 0x060099B1 RID: 39345 RVA: 0x0047CC6E File Offset: 0x0047AE6E
		private void OnListenerIdReady()
		{
			TaiwuDomainMethod.Call.GetCanOperateItemDisplayDataInVillage(this.Element.GameDataListenerId, 1205);
			TaiwuDomainMethod.Call.GetCannotOperateItemDisplayDataInInventory(this.Element.GameDataListenerId, 1205);
			GlobalDomainMethod.Call.InvokeGuidingTrigger(119);
		}

		// Token: 0x060099B2 RID: 39346 RVA: 0x0047CCA8 File Offset: 0x0047AEA8
		private void Awake()
		{
			this.mask.gameObject.SetActive(true);
			this.GetLockBuildingList();
			this._toggleGroupMultiSelect = this.buildingScroll.GetComponent<CToggleGroupMultiSelect>();
			this._toggleGroupMultiSelect.Init();
			this._toggleGroupMultiSelect.OnActiveIndexChange += this.ToggleGroupIndexChange;
			this.leftTypeToggleGroup.Init(-1);
			this.leftTypeToggleGroup.OnActiveIndexChange += this.LeftTypeToggleIndexChange;
			this.buildingScroll.OnScrollEvent += this.OnBuildingScrollChanged;
			this.buildingScroll.OnListenerDimensionsChangeEvent += this.OnBuildingScrollContentDimensionsChanged;
			this.InitSortAndFilter();
		}

		// Token: 0x060099B3 RID: 39347 RVA: 0x0047CD60 File Offset: 0x0047AF60
		private void InitSortAndFilter()
		{
			this._buildingOverviewSortAndFilterController = new BuildingOverviewSortAndFilterController(this.sortAndFilter);
			this._buildingOverviewSortAndFilterController.Init(new Action(this.OnFilterChange), "BuildingOverview");
		}

		// Token: 0x060099B4 RID: 39348 RVA: 0x0047CD91 File Offset: 0x0047AF91
		private void OnFilterChange()
		{
			this.ApplySortAndFilter();
		}

		// Token: 0x060099B5 RID: 39349 RVA: 0x0047CD9B File Offset: 0x0047AF9B
		private void OnDisable()
		{
			this.mask.gameObject.SetActive(true);
		}

		// Token: 0x060099B6 RID: 39350 RVA: 0x0047CDB0 File Offset: 0x0047AFB0
		private void ToggleGroupIndexChange(int toggleNew, int toggleOld)
		{
			bool flag = toggleNew < 0;
			if (flag)
			{
				this.ResetState();
			}
		}

		// Token: 0x060099B7 RID: 39351 RVA: 0x0047CDD0 File Offset: 0x0047AFD0
		private void LeftTypeToggleIndexChange(int toggleNew, int toggleOld)
		{
			bool isSyncingLeftTypeToggle = this._isSyncingLeftTypeToggle;
			if (!isSyncingLeftTypeToggle)
			{
				bool flag = toggleNew == 2;
				if (flag)
				{
					GlobalDomainMethod.Call.InvokeGuidingTrigger(120);
				}
				bool flag2 = !base.gameObject.activeSelf;
				if (!flag2)
				{
					bool flag3 = toggleNew == 0;
					if (flag3)
					{
						base.StartCoroutine(this.SetBuildingScrollToTop());
					}
					else
					{
						BuildingOverviewTypeBuildingPrefab typePanel = this.FindTypePanelForLeftToggle(toggleNew);
						bool flag4 = typePanel != null;
						if (flag4)
						{
							base.StartCoroutine(this.SetBuildingScrollPosition(typePanel.selfRect));
						}
						else
						{
							RectTransform buildingItem = this.SelectFirstBuildingItem(toggleNew);
							bool flag5 = buildingItem != null;
							if (flag5)
							{
								base.StartCoroutine(this.SetBuildingScrollPosition(buildingItem));
							}
						}
					}
				}
			}
		}

		// Token: 0x060099B8 RID: 39352 RVA: 0x0047CE7C File Offset: 0x0047B07C
		private void ApplySortAndFilter()
		{
			List<BuildingOverviewSortData> sourceList = this.CollectSortAndFilterData();
			BuildingOverviewSortAndFilterController buildingOverviewSortAndFilterController = this._buildingOverviewSortAndFilterController;
			if (buildingOverviewSortAndFilterController != null)
			{
				buildingOverviewSortAndFilterController.NotifyDataChanged(sourceList);
			}
			BuildingOverviewSortAndFilterController buildingOverviewSortAndFilterController2 = this._buildingOverviewSortAndFilterController;
			Func<BuildingOverviewSortData, bool> func;
			if ((func = ((buildingOverviewSortAndFilterController2 != null) ? buildingOverviewSortAndFilterController2.GenerateFilter() : null)) == null && (func = ViewBuildingOverview.<>c.<>9__82_0) == null)
			{
				func = (ViewBuildingOverview.<>c.<>9__82_0 = ((BuildingOverviewSortData _) => true));
			}
			Func<BuildingOverviewSortData, bool> filter = func;
			BuildingOverviewSortAndFilterController buildingOverviewSortAndFilterController3 = this._buildingOverviewSortAndFilterController;
			Comparison<BuildingOverviewSortData> comparer = (buildingOverviewSortAndFilterController3 != null) ? buildingOverviewSortAndFilterController3.GenerateComparer(sourceList) : null;
			List<BuildingOverviewSortData> filteredList = new List<BuildingOverviewSortData>(sourceList.Count);
			for (int i = 0; i < sourceList.Count; i++)
			{
				BuildingOverviewSortData item = sourceList[i];
				bool flag = filter(item);
				if (flag)
				{
					filteredList.Add(item);
				}
			}
			bool flag2 = comparer != null;
			if (flag2)
			{
				filteredList.Sort(comparer);
			}
			HashSet<short> visibleTemplateIds = new HashSet<short>(filteredList.Count);
			for (int j = 0; j < filteredList.Count; j++)
			{
				visibleTemplateIds.Add(filteredList[j].Building.TemplateId);
			}
			foreach (KeyValuePair<short, BuildingOverviewBuildingPrefab> pair in this._buildingPrefabMap)
			{
				pair.Value.gameObject.SetActive(visibleTemplateIds.Contains(pair.Key));
			}
			Dictionary<Transform, int> parentSiblingIndexMap = new Dictionary<Transform, int>();
			for (int k = 0; k < filteredList.Count; k++)
			{
				short templateId = filteredList[k].Building.TemplateId;
				BuildingOverviewBuildingPrefab prefab;
				bool flag3 = !this._buildingPrefabMap.TryGetValue(templateId, out prefab);
				if (!flag3)
				{
					Transform parent = prefab.transform.parent;
					int siblingIndex;
					bool flag4 = !parentSiblingIndexMap.TryGetValue(parent, out siblingIndex);
					if (flag4)
					{
						siblingIndex = 0;
					}
					prefab.transform.SetSiblingIndex(siblingIndex);
					parentSiblingIndexMap[parent] = siblingIndex + 1;
				}
			}
			BuildingOverviewSortAndFilterController buildingOverviewSortAndFilterController4 = this._buildingOverviewSortAndFilterController;
			if (buildingOverviewSortAndFilterController4 != null)
			{
				buildingOverviewSortAndFilterController4.AfterFilter(sourceList);
			}
			this.HideNoneItemPanelAfterFilter();
			this.UpdateEmptyResultMark();
			this.AdjustPanelSizeAfterFilter();
			this.EnsureBuildingListLayoutUpdated();
			this.SelectFirstBuildingItem();
			this.SyncLeftTypeToggleFromScroll();
		}

		// Token: 0x060099B9 RID: 39353 RVA: 0x0047D0CC File Offset: 0x0047B2CC
		private List<BuildingOverviewSortData> CollectSortAndFilterData()
		{
			List<BuildingOverviewSortData> result = new List<BuildingOverviewSortData>(this._buildingPrefabMap.Count);
			int nullConfigCount = 0;
			foreach (KeyValuePair<short, BuildingOverviewBuildingPrefab> pair in this._buildingPrefabMap)
			{
				BuildingBlockItem buildingConfig = BuildingBlock.Instance[pair.Key];
				bool flag = buildingConfig == null;
				if (flag)
				{
					nullConfigCount++;
				}
				result.Add(new BuildingOverviewSortData
				{
					Building = buildingConfig,
					Status = pair.Value.Status
				});
			}
			return result;
		}

		// Token: 0x060099BA RID: 39354 RVA: 0x0047D184 File Offset: 0x0047B384
		private void UpdateEmptyResultMark()
		{
			bool flag = this.emptyGo == null;
			if (!flag)
			{
				CToggleGroupMultiSelect toggleGroupMultiSelect = this._toggleGroupMultiSelect;
				List<CToggle> toggles = (toggleGroupMultiSelect != null) ? toggleGroupMultiSelect.GetAll() : null;
				bool hasVisibleItem = false;
				bool flag2 = toggles != null;
				if (flag2)
				{
					for (int i = 0; i < toggles.Count; i++)
					{
						bool activeSelf = toggles[i].gameObject.activeSelf;
						if (activeSelf)
						{
							hasVisibleItem = true;
							break;
						}
					}
				}
				this.emptyGo.SetActive(!hasVisibleItem);
			}
		}

		// Token: 0x060099BB RID: 39355 RVA: 0x0047D210 File Offset: 0x0047B410
		private void SelectFirstBuildingItem()
		{
			for (int i = 0; i < this.content.childCount; i++)
			{
				Transform child = this.content.GetChild(i);
				BuildingOverviewTypeBuildingPrefab typeBuilding = child.GetComponent<BuildingOverviewTypeBuildingPrefab>();
				bool activeSelf = typeBuilding.gameObject.activeSelf;
				if (activeSelf)
				{
					for (int j = 0; j < typeBuilding.buildingRoot.childCount; j++)
					{
						Transform buildingItem = typeBuilding.buildingRoot.GetChild(j);
						BuildingOverviewBuildingPrefab building = buildingItem.GetComponent<BuildingOverviewBuildingPrefab>();
						bool activeSelf2 = building.gameObject.activeSelf;
						if (activeSelf2)
						{
							building.toggle.isOn = true;
							this._toggleGroupMultiSelect.NotifyToggle(building.toggle, true, true);
							return;
						}
					}
				}
			}
		}

		// Token: 0x060099BC RID: 39356 RVA: 0x0047D2DC File Offset: 0x0047B4DC
		private EBuildingBlockClass? GetBuildingBlockClassForLeftToggle(int toggleIndex)
		{
			EBuildingBlockClass? result;
			switch (toggleIndex)
			{
			case 0:
				result = null;
				break;
			case 1:
				result = new EBuildingBlockClass?(EBuildingBlockClass.Villiage);
				break;
			case 2:
				result = new EBuildingBlockClass?(EBuildingBlockClass.BornResource);
				break;
			case 3:
				result = new EBuildingBlockClass?(EBuildingBlockClass.Resource);
				break;
			case 4:
				result = new EBuildingBlockClass?(EBuildingBlockClass.Kungfu);
				break;
			default:
			{
				int lifeSkillIndex = toggleIndex - 5;
				bool flag = lifeSkillIndex < 0 || lifeSkillIndex >= 16;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = new EBuildingBlockClass?(EBuildingBlockClass.Music + lifeSkillIndex);
				}
				break;
			}
			}
			return result;
		}

		// Token: 0x060099BD RID: 39357 RVA: 0x0047D370 File Offset: 0x0047B570
		private BuildingOverviewTypeBuildingPrefab FindTypePanelForLeftToggle(int toggleIndex)
		{
			EBuildingBlockClass? targetClass = this.GetBuildingBlockClassForLeftToggle(toggleIndex);
			bool flag = targetClass == null;
			BuildingOverviewTypeBuildingPrefab result;
			if (flag)
			{
				for (int i = 0; i < this._typeBuildingItems.Count; i++)
				{
					bool activeSelf = this._typeBuildingItems[i].Panel.gameObject.activeSelf;
					if (activeSelf)
					{
						return this._typeBuildingItems[i].Panel;
					}
				}
				result = null;
			}
			else
			{
				for (int j = 0; j < this._typeBuildingItems.Count; j++)
				{
					bool flag2 = this._typeBuildingItems[j].BlockClass == targetClass.Value;
					if (flag2)
					{
						return this._typeBuildingItems[j].Panel;
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x060099BE RID: 39358 RVA: 0x0047D44C File Offset: 0x0047B64C
		private RectTransform SelectFirstVisibleBuildingInTypePanel(BuildingOverviewTypeBuildingPrefab panel)
		{
			RectTransform[] roots = new RectTransform[]
			{
				panel.buildingRoot,
				panel.buildingRootNormalResource,
				panel.buildingRootLegacy
			};
			foreach (RectTransform root in roots)
			{
				bool flag = !root.gameObject.activeSelf;
				if (!flag)
				{
					for (int i = 0; i < root.childCount; i++)
					{
						Transform buildingItem = root.GetChild(i);
						BuildingOverviewBuildingPrefab building = buildingItem.GetComponent<BuildingOverviewBuildingPrefab>();
						bool flag2 = building == null || !building.gameObject.activeSelf;
						if (!flag2)
						{
							building.toggle.isOn = true;
							this._toggleGroupMultiSelect.NotifyToggle(building.toggle, true, true);
							return buildingItem.GetComponent<RectTransform>();
						}
					}
				}
			}
			return null;
		}

		// Token: 0x060099BF RID: 39359 RVA: 0x0047D53C File Offset: 0x0047B73C
		private RectTransform SelectFirstBuildingItem(int toggleNew)
		{
			BuildingOverviewTypeBuildingPrefab typePanel = this.FindTypePanelForLeftToggle(toggleNew);
			return (typePanel == null) ? null : this.SelectFirstVisibleBuildingInTypePanel(typePanel);
		}

		// Token: 0x060099C0 RID: 39360 RVA: 0x0047D56C File Offset: 0x0047B76C
		private int GetLeftToggleIndexForBlockClass(EBuildingBlockClass blockClass)
		{
			switch (blockClass)
			{
			case EBuildingBlockClass.BornResource:
				return 2;
			case EBuildingBlockClass.Villiage:
				return 1;
			case EBuildingBlockClass.Resource:
				return 3;
			case EBuildingBlockClass.Kungfu:
				return 4;
			}
			int lifeSkillIndex = blockClass - EBuildingBlockClass.Music;
			bool flag = lifeSkillIndex >= 0 && lifeSkillIndex < 16;
			int result;
			if (flag)
			{
				result = 5 + lifeSkillIndex;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x060099C1 RID: 39361 RVA: 0x0047D5D0 File Offset: 0x0047B7D0
		private EBuildingBlockClass? FindBlockClassForPanel(BuildingOverviewTypeBuildingPrefab panel)
		{
			for (int i = 0; i < this._typeBuildingItems.Count; i++)
			{
				bool flag = this._typeBuildingItems[i].Panel == panel;
				if (flag)
				{
					return new EBuildingBlockClass?(this._typeBuildingItems[i].BlockClass);
				}
			}
			return null;
		}

		// Token: 0x060099C2 RID: 39362 RVA: 0x0047D63C File Offset: 0x0047B83C
		private void EnsureBuildingListLayoutUpdated()
		{
			Canvas.ForceUpdateCanvases();
			bool flag = this.content != null;
			if (flag)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(this.content);
			}
		}

		// Token: 0x060099C3 RID: 39363 RVA: 0x0047D66C File Offset: 0x0047B86C
		private int GetLeftToggleIndexForScrollPosition()
		{
			bool flag;
			if (!(this.content == null))
			{
				CScrollRect cscrollRect = this.buildingScroll;
				flag = (((cscrollRect != null) ? cscrollRect.Viewport : null) == null);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			int result;
			if (flag2)
			{
				result = 0;
			}
			else
			{
				RectTransform viewport = this.buildingScroll.Viewport;
				float referenceWorldY = viewport.TransformPoint(new Vector3(0f, viewport.rect.yMax - 5f, 0f)).y;
				EBuildingBlockClass? activeBlockClass = null;
				for (int i = 0; i < this.content.childCount; i++)
				{
					RectTransform child = this.content.GetChild(i) as RectTransform;
					bool flag3 = child == null || !child.gameObject.activeSelf;
					if (!flag3)
					{
						BuildingOverviewTypeBuildingPrefab typeBuilding = child.GetComponent<BuildingOverviewTypeBuildingPrefab>();
						bool flag4 = typeBuilding == null;
						if (!flag4)
						{
							child.GetWorldCorners(ViewBuildingOverview.ScrollSyncCornersBuffer);
							float panelBottomWorldY = ViewBuildingOverview.ScrollSyncCornersBuffer[0].y;
							bool flag5 = panelBottomWorldY > referenceWorldY;
							if (!flag5)
							{
								EBuildingBlockClass? currentClass = this.FindBlockClassForPanel(typeBuilding);
								bool flag6 = currentClass != null;
								if (flag6)
								{
									activeBlockClass = currentClass;
								}
								break;
							}
							EBuildingBlockClass? passedClass = this.FindBlockClassForPanel(typeBuilding);
							bool flag7 = passedClass != null;
							if (flag7)
							{
								activeBlockClass = passedClass;
							}
						}
					}
				}
				bool flag8 = activeBlockClass == null;
				if (flag8)
				{
					result = 0;
				}
				else
				{
					result = this.GetLeftToggleIndexForBlockClass(activeBlockClass.Value);
				}
			}
			return result;
		}

		// Token: 0x060099C4 RID: 39364 RVA: 0x0047D7F8 File Offset: 0x0047B9F8
		private void SyncLeftTypeToggleFromScroll()
		{
			bool flag = this._isSyncingLeftTypeToggle || this.leftTypeToggleGroup == null;
			if (!flag)
			{
				int targetToggle = this.GetLeftToggleIndexForScrollPosition();
				bool flag2 = this.leftTypeToggleGroup.GetActiveIndex() == targetToggle;
				if (!flag2)
				{
					this._isSyncingLeftTypeToggle = true;
					this.leftTypeToggleGroup.SetWithoutNotify(targetToggle);
					this._isSyncingLeftTypeToggle = false;
				}
			}
		}

		// Token: 0x060099C5 RID: 39365 RVA: 0x0047D859 File Offset: 0x0047BA59
		private void OnBuildingScrollChanged()
		{
			this.SyncLeftTypeToggleFromScroll();
		}

		// Token: 0x060099C6 RID: 39366 RVA: 0x0047D863 File Offset: 0x0047BA63
		private void OnBuildingScrollContentDimensionsChanged()
		{
			this.EnsureBuildingListLayoutUpdated();
			this.SyncLeftTypeToggleFromScroll();
		}

		// Token: 0x060099C7 RID: 39367 RVA: 0x0047D874 File Offset: 0x0047BA74
		private void BeginSyncLeftTypeToggleFromProgramScroll()
		{
			this._isSyncingLeftTypeToggle = true;
			CScrollRect cscrollRect = this.buildingScroll;
			cscrollRect.OnScrollEnd = (Action)Delegate.Remove(cscrollRect.OnScrollEnd, new Action(this.EndSyncLeftTypeToggleFromProgramScroll));
			CScrollRect cscrollRect2 = this.buildingScroll;
			cscrollRect2.OnScrollEnd = (Action)Delegate.Combine(cscrollRect2.OnScrollEnd, new Action(this.EndSyncLeftTypeToggleFromProgramScroll));
		}

		// Token: 0x060099C8 RID: 39368 RVA: 0x0047D8D7 File Offset: 0x0047BAD7
		private void EndSyncLeftTypeToggleFromProgramScroll()
		{
			CScrollRect cscrollRect = this.buildingScroll;
			cscrollRect.OnScrollEnd = (Action)Delegate.Remove(cscrollRect.OnScrollEnd, new Action(this.EndSyncLeftTypeToggleFromProgramScroll));
			this._isSyncingLeftTypeToggle = false;
			this.SyncLeftTypeToggleFromScroll();
		}

		// Token: 0x060099C9 RID: 39369 RVA: 0x0047D90F File Offset: 0x0047BB0F
		private void CancelSyncLeftTypeToggleFromProgramScroll()
		{
			CScrollRect cscrollRect = this.buildingScroll;
			cscrollRect.OnScrollEnd = (Action)Delegate.Remove(cscrollRect.OnScrollEnd, new Action(this.EndSyncLeftTypeToggleFromProgramScroll));
			this._isSyncingLeftTypeToggle = false;
		}

		// Token: 0x060099CA RID: 39370 RVA: 0x0047D940 File Offset: 0x0047BB40
		private void AdjustGridRootHeightAfterFilter(RectTransform root, float lastLineAdjustPixel)
		{
			bool flag = root == null || !root.gameObject.activeSelf;
			if (!flag)
			{
				int visibleChildrenCount = 0;
				for (int i = 0; i < root.childCount; i++)
				{
					bool activeSelf = root.GetChild(i).gameObject.activeSelf;
					if (activeSelf)
					{
						visibleChildrenCount++;
					}
				}
				GridLayoutGroup gridLayout = root.GetComponent<GridLayoutGroup>();
				bool flag2 = gridLayout == null;
				if (!flag2)
				{
					int line = Mathf.CeilToInt((float)visibleChildrenCount / (float)Mathf.Max(1, gridLayout.constraintCount));
					float height = (float)line * this.buildingPrefabSize.y + (float)gridLayout.padding.vertical + (float)(line - 1) * gridLayout.spacing.y + lastLineAdjustPixel;
					root.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
				}
			}
		}

		// Token: 0x060099CB RID: 39371 RVA: 0x0047DA14 File Offset: 0x0047BC14
		private void AdjustPanelSizeAfterFilter()
		{
			foreach (ViewBuildingOverview.BuildingTypeState item in this._typeBuildingItems)
			{
				this.AdjustGridRootHeightAfterFilter(item.Panel.buildingRoot, 64f);
				this.AdjustGridRootHeightAfterFilter(item.Panel.buildingRootLegacy, 64f);
				this.AdjustGridRootHeightAfterFilter(item.Panel.buildingRootNormalResource, 64f);
				RectTransform panelRect = item.Panel.transform as RectTransform;
				bool flag = panelRect != null;
				if (flag)
				{
					LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);
				}
			}
		}

		// Token: 0x060099CC RID: 39372 RVA: 0x0047DACC File Offset: 0x0047BCCC
		private void HideNoneItemPanelAfterFilter()
		{
			foreach (ViewBuildingOverview.BuildingTypeState item in this._typeBuildingItems)
			{
				bool allChildrenInvisibleInBuildingRoot = true;
				for (int i = 0; i < item.Panel.buildingRoot.childCount; i++)
				{
					bool activeSelf = item.Panel.buildingRoot.GetChild(i).gameObject.activeSelf;
					if (activeSelf)
					{
						allChildrenInvisibleInBuildingRoot = false;
						break;
					}
				}
				item.Panel.buildingRoot.gameObject.SetActive(!allChildrenInvisibleInBuildingRoot);
				item.Panel.typeNameRect.gameObject.SetActive(!allChildrenInvisibleInBuildingRoot);
				bool allChildrenInvisibleInBuildingRootLegacy = true;
				for (int j = 0; j < item.Panel.buildingRootLegacy.childCount; j++)
				{
					bool activeSelf2 = item.Panel.buildingRootLegacy.GetChild(j).gameObject.activeSelf;
					if (activeSelf2)
					{
						allChildrenInvisibleInBuildingRootLegacy = false;
						break;
					}
				}
				item.Panel.buildingRootLegacy.gameObject.SetActive(!allChildrenInvisibleInBuildingRootLegacy);
				item.Panel.typeNameRectLegacy.gameObject.SetActive(!allChildrenInvisibleInBuildingRootLegacy);
				bool allChildrenInvisibleInBuildingRootNormalResource = true;
				for (int k = 0; k < item.Panel.buildingRootNormalResource.childCount; k++)
				{
					bool activeSelf3 = item.Panel.buildingRootNormalResource.GetChild(k).gameObject.activeSelf;
					if (activeSelf3)
					{
						allChildrenInvisibleInBuildingRootNormalResource = false;
						break;
					}
				}
				item.Panel.buildingRootNormalResource.gameObject.SetActive(!allChildrenInvisibleInBuildingRootNormalResource);
				item.Panel.typeNameRectNormalResource.gameObject.SetActive(!allChildrenInvisibleInBuildingRootNormalResource);
				bool panelActive = !allChildrenInvisibleInBuildingRoot || !allChildrenInvisibleInBuildingRootLegacy || !allChildrenInvisibleInBuildingRootNormalResource;
				item.Panel.gameObject.SetActive(item.ShouldState && panelActive);
			}
		}

		// Token: 0x060099CD RID: 39373 RVA: 0x0047DCE4 File Offset: 0x0047BEE4
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
			this.autoArrangeBtn.gameObject.SetActive(!SingletonObject.getInstance<TutorialChapterModel>().InGuiding);
			this.quickClearOperatorBtn.SetActive(!SingletonObject.getInstance<TutorialChapterModel>().InGuiding);
		}

		// Token: 0x060099CE RID: 39374 RVA: 0x0047DD78 File Offset: 0x0047BF78
		private void SetToggleState(bool notifySelectionChange = false)
		{
			List<CToggle> toggleList = this.leftTypeToggleGroup.GetAll();
			bool flag = this.shrineBuild;
			if (flag)
			{
				for (int i = 0; i < this.leftTypeToggleGroup.Count(); i++)
				{
					toggleList[i].interactable = true;
					toggleList[i].GetComponent<DisableStyleRoot>().SetStyleEffect(false, false);
				}
				if (notifySelectionChange)
				{
					this.leftTypeToggleGroup.Set(0, false);
				}
				else
				{
					this.leftTypeToggleGroup.SetWithoutNotify(0);
				}
			}
			else
			{
				for (int j = 0; j < this.leftTypeToggleGroup.Count(); j++)
				{
					toggleList[j].interactable = false;
					toggleList[j].GetComponent<DisableStyleRoot>().SetStyleEffect(true, false);
				}
				toggleList[1].GetComponent<DisableStyleRoot>().SetStyleEffect(false, false);
				if (notifySelectionChange)
				{
					this.leftTypeToggleGroup.Set(1, true);
				}
				else
				{
					this.leftTypeToggleGroup.SetWithoutNotify(1);
				}
			}
		}

		// Token: 0x060099CF RID: 39375 RVA: 0x0047DE88 File Offset: 0x0047C088
		private void OnDestroy()
		{
			PoolManager.RemoveData("UI_BuildingOverview_BuildingTypeObject");
			PoolManager.RemoveData("UI_BuildingOverview_BuildingItemObject");
			this.leftTypeToggleGroup.OnActiveIndexChange -= this.LeftTypeToggleIndexChange;
			this._toggleGroupMultiSelect.OnActiveIndexChange -= this.ToggleGroupIndexChange;
			this.buildingScroll.OnScrollEvent -= this.OnBuildingScrollChanged;
			this.buildingScroll.OnListenerDimensionsChangeEvent -= this.OnBuildingScrollContentDimensionsChanged;
			CScrollRect cscrollRect = this.buildingScroll;
			cscrollRect.OnScrollEnd = (Action)Delegate.Remove(cscrollRect.OnScrollEnd, new Action(this.EndSyncLeftTypeToggleFromProgramScroll));
		}

		// Token: 0x060099D0 RID: 39376 RVA: 0x0047DF34 File Offset: 0x0047C134
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)this._taiwuCharId), new uint[]
			{
				29U
			}));
			this.MonitorFields.Add(new UIBase.MonitorDataField(3, 1, (ulong)this._settlementId, new uint[]
			{
				10U
			}));
			this.MonitorFields.Add(new UIBase.MonitorDataField(5, 9, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(5, 10, ulong.MaxValue, null));
		}

		// Token: 0x060099D1 RID: 39377 RVA: 0x0047DFBC File Offset: 0x0047C1BC
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
								this.RefreshAutoArrangeBtn();
								this.Element.ShowAfterRefresh();
							}
						}
						else
						{
							bool flag3 = notification.DomainId == 5;
							if (flag3)
							{
								bool flag4 = notification.MethodId == 11;
								if (flag4)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._availableWorkers);
									this.UpdateOperatorInfo();
								}
								bool flag5 = notification.MethodId == 55;
								if (flag5)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._canUseBuildingCore);
								}
								bool flag6 = notification.MethodId == 71;
								if (flag6)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._cannotUseInventoryBuildingCore);
									this.UpdateCostInfo();
									this.UpdateConfirmButton();
								}
							}
						}
					}
				}
				else
				{
					DataUid uid = notification.Uid;
					bool flag7 = uid.DomainId == 4 && uid.DataId == 0 && (int)uid.SubId0 == this._taiwuCharId && uid.SubId1 == 29U;
					if (flag7)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._learnedLifeSkillItems);
						this.UpdateLockBuildingList();
						this.SetToggleState(false);
					}
					else
					{
						bool flag8 = uid.DomainId == 3 && uid.DataId == 1 && (int)uid.SubId0 == (int)this._settlementId && uid.SubId1 == 10U;
						if (flag8)
						{
							OrgMemberCollection collection = null;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref collection);
							collection.GetAllMembers(this._villagerList);
							bool flag9 = this._villagerList.Count > 0;
							if (flag9)
							{
								TaiwuDomainMethod.Call.GetAllVillagersAvailableForWork(this.Element.GameDataListenerId);
								CharacterDomainMethod.Call.GetCharacterDisplayDataList(this.Element.GameDataListenerId, this._villagerList);
							}
							else
							{
								this._charDisplayDataDict.Clear();
							}
						}
						else
						{
							bool flag10 = uid.DomainId == 5;
							if (flag10)
							{
								bool flag11 = uid.DataId == 9;
								if (flag11)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._buildingSpaceLimit);
									this.UpdateBuildingSpaceInfo();
								}
								else
								{
									bool flag12 = uid.DataId == 10;
									if (flag12)
									{
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._buildingSpaceCurr);
										this.UpdateBuildingSpaceInfo();
										this.Refresh();
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060099D2 RID: 39378 RVA: 0x0047E33C File Offset: 0x0047C53C
		private void UpdateBuildingSpaceInfo()
		{
			bool flag = this._configData == null;
			if (!flag)
			{
				int needSpace = (int)(BuildingBlockData.IsUsefulResource(this._configData.Type) ? 0 : this._configData.Width);
				string color = (this._buildingSpaceLimit - this._buildingSpaceCurr >= needSpace) ? "F8E0CA" : "brightred";
				this.buildingSpaceInfoText.SetText(LocalStringManager.GetFormat(LanguageKey.LK_Taiwu_BuildingSpaceLimit, string.Format("<color=#{0}>{1}</color>", color, this._buildingSpaceLimit - this._buildingSpaceCurr).ColorReplace(), needSpace), true);
			}
		}

		// Token: 0x060099D3 RID: 39379 RVA: 0x0047E3E0 File Offset: 0x0047C5E0
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			string text = btnName;
			string a = text;
			if (!(a == "Close"))
			{
				if (!(a == "AutoArrangeBtn"))
				{
					if (!(a == "RemoveOperator"))
					{
						if (!(a == "QuickClearOperatorBtn"))
						{
							if (a == "ConfirmOperation")
							{
								GEvent.OnEvent(UiEvents.StartPlacingBuilding, null);
								this._buildingArea.StartPlacingBuildingWithName(this._configData, this._operatorListCached, this._customName);
								this.QuickHide();
							}
						}
						else
						{
							this._selectingOperatorIndex = 0;
							while (this._selectingOperatorIndex < this._operatorListCached.Length)
							{
								this._operatorListCached[this._selectingOperatorIndex] = -1;
								this._selectingOperatorIndex++;
							}
							this.UpdateOperatorInfo();
							this.UpdateConfirmButton();
							this.RefreshAutoArrangeBtn();
						}
					}
					else
					{
						this._selectingOperatorIndex = btn.transform.parent.GetSiblingIndex();
						bool flag = this._operatorListCached[this._selectingOperatorIndex] >= 0;
						if (flag)
						{
							this._operatorListCached[this._selectingOperatorIndex] = -1;
							this.UpdateOperatorInfo();
							this.UpdateConfirmButton();
							this.RefreshAutoArrangeBtn();
						}
					}
				}
				else
				{
					bool flag2 = this._configData == null;
					if (!flag2)
					{
						this.AutoArrangeWorkers();
					}
				}
			}
			else
			{
				this.QuickHide();
			}
		}

		// Token: 0x060099D4 RID: 39380 RVA: 0x0047E554 File Offset: 0x0047C754
		private void RefreshAutoArrangeBtn()
		{
			bool interactable = this.CanAutoArrange();
			this.autoArrangeBtn.interactable = interactable;
			this.autoArrangeBtn.transform.GetComponentInChildren<TextMeshProUGUI>().color = Colors.Instance[interactable ? "yellow" : "lightgrey"];
			TooltipInvoker tipDisplayer = this.autoArrangeBtn.GetComponent<TooltipInvoker>();
			TooltipInvoker tooltipInvoker = tipDisplayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			tipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(interactable ? LanguageKey.LK_Building_QuickArrangeTip : LanguageKey.LK_Building_QuickArrangeTip_Disable));
			tipDisplayer.Refresh(false, -1);
		}

		// Token: 0x060099D5 RID: 39381 RVA: 0x0047E5F8 File Offset: 0x0047C7F8
		private bool CanAutoArrange()
		{
			BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
			int count = this._availableWorkers.Count((int id) => !this._operatorListCached.Contains(id) && !this._charDisplayDataDict[id].CompletelyInfected && !buildingModel.VillagerWork.ContainsKey(id));
			return count > 0;
		}

		// Token: 0x060099D6 RID: 39382 RVA: 0x0047E63E File Offset: 0x0047C83E
		private void AutoArrangeWorkers()
		{
			BuildingDomainMethod.AsyncCall.QuickArrangeBuildOperator(this, this._configData.TemplateId, BuildingBlockKey.Invalid, 0, delegate(int offset, RawDataPool dataPool)
			{
				List<int> charIdList = new List<int>();
				Serializer.Deserialize(dataPool, offset, ref charIdList);
				for (int index = 0; index < charIdList.Count; index++)
				{
					this._selectingOperatorIndex = index;
					this.SetOperator(charIdList[index]);
				}
			});
		}

		// Token: 0x060099D7 RID: 39383 RVA: 0x0047E668 File Offset: 0x0047C868
		private void Update()
		{
			bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) && this.confirmOperation.interactable;
			if (flag)
			{
				this._buildingArea.StartPlacingBuildingWithName(this._configData, this._operatorListCached, this._customName);
				this.QuickHide();
			}
		}

		// Token: 0x060099D8 RID: 39384 RVA: 0x0047E6C8 File Offset: 0x0047C8C8
		private void InitData()
		{
			this._buildingMap = new Dictionary<EBuildingBlockClass, List<BuildingBlockItem>>();
			bool inGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			if (inGuiding)
			{
				bool addBambooHouse = SingletonObject.getInstance<TutorialChapterModel>().TutorialChapterIndex == 1;
				for (int i = 2; i < 22; i++)
				{
					bool flag = i == 4;
					if (!flag)
					{
						List<BuildingBlockItem> list = new List<BuildingBlockItem>();
						bool flag2 = addBambooHouse;
						if (flag2)
						{
							bool flag3 = i == 2;
							if (flag3)
							{
								list.Add(BuildingBlock.Instance.GetItem(258));
							}
						}
						this._buildingMap.Add((EBuildingBlockClass)i, list);
					}
				}
			}
			else
			{
				bool flag4 = this.shrineBuild;
				if (flag4)
				{
					BuildingBlock.Instance.Iterate(delegate(BuildingBlockItem item)
					{
						bool flag7 = item.Class >= EBuildingBlockClass.BornResource && item.Type != EBuildingBlockType.UselessResource;
						if (flag7)
						{
							List<BuildingBlockItem> list3;
							bool flag8 = !this._buildingMap.TryGetValue(item.Class, out list3);
							if (flag8)
							{
								list3 = new List<BuildingBlockItem>();
								this._buildingMap.Add(item.Class, list3);
							}
							bool flag9 = (item.TemplateId != 45 && item.TemplateId != 49) || (item.TemplateId == 49 && this._isHaveChickenKing);
							if (flag9)
							{
								list3.Add(item);
							}
						}
						return true;
					});
					foreach (KeyValuePair<EBuildingBlockClass, List<BuildingBlockItem>> pair in this._buildingMap)
					{
						pair.Value.Sort((BuildingBlockItem left, BuildingBlockItem right) => (int)(left.TemplateId - right.TemplateId));
					}
				}
				else
				{
					for (int j = 2; j < 22; j++)
					{
						bool flag5 = j == 4;
						if (!flag5)
						{
							List<BuildingBlockItem> list2 = new List<BuildingBlockItem>();
							bool flag6 = j == 2;
							if (flag6)
							{
								list2.Add(BuildingBlock.Instance.GetItem(45));
							}
							this._buildingMap.Add((EBuildingBlockClass)j, list2);
						}
					}
				}
			}
		}

		// Token: 0x060099D9 RID: 39385 RVA: 0x0047E858 File Offset: 0x0047CA58
		public void ResetItems()
		{
			this._toggleGroupMultiSelect.Clear();
			this._typeBuildingItems.Clear();
			this._buildingToggleMap.Clear();
			this._buildingPrefabMap.Clear();
			foreach (object obj in this.content)
			{
				Transform child = (Transform)obj;
				BuildingOverviewTypeBuildingPrefab childRefers = child.GetComponent<BuildingOverviewTypeBuildingPrefab>();
				RectTransform itemRoot = childRefers.buildingRoot;
				foreach (object obj2 in itemRoot)
				{
					Transform buildingItem = (Transform)obj2;
					PoolManager.Destroy("UI_BuildingOverview_BuildingItemObject", buildingItem.gameObject);
				}
				RectTransform legacyRoot = childRefers.buildingRootLegacy;
				foreach (object obj3 in legacyRoot)
				{
					Transform buildingItem2 = (Transform)obj3;
					PoolManager.Destroy("UI_BuildingOverview_BuildingItemObject", buildingItem2.gameObject);
				}
				RectTransform normalResourceRoot = childRefers.buildingRootNormalResource;
				foreach (object obj4 in normalResourceRoot)
				{
					Transform buildingItem3 = (Transform)obj4;
					PoolManager.Destroy("UI_BuildingOverview_BuildingItemObject", buildingItem3.gameObject);
				}
				PoolManager.Destroy("UI_BuildingOverview_BuildingTypeObject", child.gameObject);
			}
		}

		// Token: 0x060099DA RID: 39386 RVA: 0x0047EA58 File Offset: 0x0047CC58
		private void Refresh()
		{
			bool flag = ViewBuildingArea.BlockList == null;
			if (flag)
			{
				bool isWaitingBlockList = this._isWaitingBlockList;
				if (!isWaitingBlockList)
				{
					this._isWaitingBlockList = true;
					Location villageBlock = SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageBlock();
					Location location = new Location(villageBlock.AreaId, villageBlock.BlockId);
					BuildingDomainMethod.AsyncCall.GetBuildingBlockList(this, location, delegate(int offset, RawDataPool pool)
					{
						List<BuildingBlockData> blockList = null;
						Serializer.Deserialize(pool, offset, ref blockList);
						ViewBuildingArea.BlockList = blockList;
						bool shrineBuildAfterBlockList = ViewBuildingArea.HasBuilding(45, true);
						bool flag2 = this.shrineBuild != shrineBuildAfterBlockList;
						if (flag2)
						{
							this.shrineBuild = shrineBuildAfterBlockList;
							this.InitData();
						}
						this._isWaitingBlockList = false;
						this.Refresh();
					});
				}
			}
			else
			{
				this.ResetItems();
				base.StartCoroutine(this.StartLoadRefreshType());
				this.SetToggleState(false);
			}
		}

		// Token: 0x060099DB RID: 39387 RVA: 0x0047EAD7 File Offset: 0x0047CCD7
		private IEnumerable<EBuildingBlockClass> EnumerateTypePanelsInLeftToggleOrder()
		{
			yield return EBuildingBlockClass.Villiage;
			yield return EBuildingBlockClass.BornResource;
			yield return EBuildingBlockClass.Resource;
			yield return EBuildingBlockClass.Kungfu;
			int num;
			for (int i = 0; i < 16; i = num + 1)
			{
				yield return EBuildingBlockClass.Music + i;
				num = i;
			}
			HashSet<EBuildingBlockClass> included = new HashSet<EBuildingBlockClass>
			{
				EBuildingBlockClass.Villiage,
				EBuildingBlockClass.BornResource,
				EBuildingBlockClass.Resource,
				EBuildingBlockClass.Kungfu
			};
			for (int j = 0; j < 16; j = num + 1)
			{
				included.Add(EBuildingBlockClass.Music + j);
				num = j;
			}
			EBuildingBlockClass ebuildingBlockClass;
			for (EBuildingBlockClass blockClass = EBuildingBlockClass.BornResource; blockClass <= EBuildingBlockClass.Eclectic; blockClass = ebuildingBlockClass)
			{
				bool flag = blockClass == EBuildingBlockClass.Function || included.Contains(blockClass);
				if (!flag)
				{
					yield return blockClass;
				}
				ebuildingBlockClass = blockClass + 1;
			}
			yield break;
		}

		// Token: 0x060099DC RID: 39388 RVA: 0x0047EAE7 File Offset: 0x0047CCE7
		private IEnumerator StartLoadRefreshType()
		{
			foreach (EBuildingBlockClass blockClass in this.EnumerateTypePanelsInLeftToggleOrder())
			{
				List<BuildingBlockItem> list;
				bool flag = this._buildingMap.TryGetValue(blockClass, out list);
				if (flag)
				{
					bool flag2 = list.Count == 0;
					if (flag2)
					{
						continue;
					}
					BuildingOverviewTypeBuildingPrefab refers = this.RefreshType(list, blockClass);
					refers.buildingRoot.GetComponent<GridLayoutGroup>().cellSize = this._cellSizeVector2;
					refers.buildingRootLegacy.GetComponent<GridLayoutGroup>().cellSize = this._cellSizeVector2;
					refers.typeName.text = LocalStringManager.Get(string.Format("LK_BuildingClassName_{0}", (int)blockClass));
					refers.transform.SetParent(this.content, false);
					refers.transform.SetAsLastSibling();
					this._typeBuildingItems.Add(new ViewBuildingOverview.BuildingTypeState
					{
						Panel = refers,
						ShouldState = true,
						BlockClass = blockClass
					});
					bool flag3 = blockClass == EBuildingBlockClass.Villiage;
					if (flag3)
					{
						this._toggleGroupMultiSelect.Select(0, true);
					}
					refers = null;
				}
				yield return null;
				list = null;
			}
			IEnumerator<EBuildingBlockClass> enumerator = null;
			this.ApplySortAndFilter();
			this.AutoSelectBuildingToggle(this._autoSelectBuildingTemplateId);
			this.UpdateEmptyResultMark();
			yield break;
			yield break;
		}

		// Token: 0x060099DD RID: 39389 RVA: 0x0047EAF8 File Offset: 0x0047CCF8
		private BuildingOverviewTypeBuildingPrefab RefreshType(List<BuildingBlockItem> list, EBuildingBlockClass blockClass)
		{
			BuildingOverviewTypeBuildingPrefab refers = this.RefreshResourceType(list, blockClass);
			RectTransform rootRect = refers.buildingRoot;
			for (int i = 0; i < list.Count; i++)
			{
				BuildingOverviewBuildingPrefab itemRefers = this.RefreshBuildingItem(list[i]);
				itemRefers.transform.SetParent(rootRect, false);
				itemRefers.transform.SetAsLastSibling();
				CToggle tog = itemRefers.toggle;
				this._toggleGroupMultiSelect.Add(tog);
			}
			return refers;
		}

		// Token: 0x060099DE RID: 39390 RVA: 0x0047EB74 File Offset: 0x0047CD74
		private BuildingOverviewTypeBuildingPrefab RefreshResourceType(List<BuildingBlockItem> list, EBuildingBlockClass blockClass)
		{
			BuildingOverviewTypeBuildingPrefab refers = PoolManager.GetObject<BuildingOverviewTypeBuildingPrefab>("UI_BuildingOverview_BuildingTypeObject");
			RectTransform rootRect = refers.buildingRoot;
			RectTransform typeNameRect = refers.typeNameRect;
			RectTransform typeNameNormalResource = refers.typeNameRectNormalResource;
			RectTransform buildingRootNormalResource = refers.buildingRootNormalResource;
			typeNameNormalResource.gameObject.SetActive(false);
			buildingRootNormalResource.gameObject.SetActive(false);
			RectTransform selfRect = refers.selfRect;
			GridLayoutGroup gridLayout = rootRect.GetComponent<GridLayoutGroup>();
			gridLayout.cellSize = this._cellSizeVector2;
			int line = Mathf.CeilToInt((float)list.Count / (float)Mathf.Max(1, gridLayout.constraintCount));
			float height = (float)line * this.buildingPrefabSize.y + (float)gridLayout.padding.vertical + gridLayout.spacing.y * (float)(line - 1) + 64f;
			selfRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
			refers.buildingRoot.gameObject.SetActive(true);
			return refers;
		}

		// Token: 0x060099DF RID: 39391 RVA: 0x0047EC5C File Offset: 0x0047CE5C
		private BuildingOverviewBuildingPrefab RefreshBuildingItem(BuildingBlockItem item)
		{
			BuildingOverviewBuildingPrefab buildingPrefab = PoolManager.GetObject<BuildingOverviewBuildingPrefab>("UI_BuildingOverview_BuildingItemObject");
			this._buildingToggleMap[item.TemplateId] = buildingPrefab.toggle;
			this._buildingPrefabMap[item.TemplateId] = buildingPrefab;
			ViewBuildingArea.SetBuildingIcon(buildingPrefab.icon, item, false, null);
			buildingPrefab.buildingName.text = item.Name;
			buildingPrefab.TemplateId = item.TemplateId;
			RectTransform rectTransform = buildingPrefab.buildingName.gameObject.GetComponent<RectTransform>();
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)(24 * item.Name.Length) + 0.01f);
			TooltipInvoker mouseTips = buildingPrefab.mouseTip;
			mouseTips.PresetParam = new string[]
			{
				item.Name,
				item.Desc + "\n\n<size=20><color=#8E8E8E>" + item.FuncDesc + "</color></size>"
			};
			Dictionary<short, bool> dependBuildingDict;
			bool canBuild = this._buildingArea.CanBuildAnywhere(item, out dependBuildingDict);
			bool lackNormalBuilding = false;
			bool lackResourcesBuilding = false;
			foreach (short id in item.DependBuildings)
			{
				bool hasBuilding;
				bool flag = dependBuildingDict != null && dependBuildingDict.TryGetValue(id, out hasBuilding) && hasBuilding;
				if (!flag)
				{
					BuildingBlockItem buildingConfig = BuildingBlock.Instance[id];
					bool flag2 = buildingConfig.Type == EBuildingBlockType.Building;
					if (flag2)
					{
						lackNormalBuilding = true;
					}
					else
					{
						EBuildingBlockType type = buildingConfig.Type;
						bool flag3 = type == EBuildingBlockType.NormalResource || type == EBuildingBlockType.SpecialResource;
						if (flag3)
						{
							lackResourcesBuilding = true;
						}
					}
				}
			}
			bool isUnlock = this.CanUnlockBuildingByLifeSkill(item) && this.CanUnlockBuildingByMainProgress(item);
			buildingPrefab.Status |= (isUnlock ? BuildingOverviewBuildingPrefab.EBuildingStatus.Unlocked : BuildingOverviewBuildingPrefab.EBuildingStatus.Locked);
			CToggle toggle = buildingPrefab.toggle;
			toggle.interactable = true;
			toggle.onValueChanged.RemoveAllListeners();
			toggle.isOn = false;
			toggle.onValueChanged.AddListener(delegate(bool isOn)
			{
				bool flag8 = !isOn;
				if (!flag8)
				{
					bool isUnlock = isUnlock;
					if (isUnlock)
					{
						this.operationInfo.SetActive(true);
						this.buildInfoMask.padding = Vector4.zero;
						this.timeCostEstimate.SetActive(true);
					}
					else
					{
						this.operationInfo.SetActive(false);
						this.buildInfoMask.padding = new Vector4(0f, -500f, 0f, 0f);
						this.timeCostEstimate.SetActive(false);
					}
					this.input.SetActive(true);
					for (int i = 0; i < this._operatorListCached.Length; i++)
					{
						this._operatorListCached[i] = -1;
					}
					this._configData = item;
					this._dependNormalBuildingsLack = (!canBuild & lackNormalBuilding);
					this._dependResourcesBuildingsLack = (!canBuild & lackResourcesBuilding);
					this.ClearOperatorListCache();
					this.UpdateCostInfo();
					this.UpdateOperatorInfo();
					this.UpdateConfirmButton();
					this.UpdateDesc();
					this.RefreshAutoArrangeBtn();
				}
			});
			bool countCanBuildCheck = false;
			bool showChallengeWarning = false;
			buildingPrefab.mask.SetActive(!isUnlock);
			buildingPrefab.infoArea.SetActive(isUnlock);
			buildingPrefab.buildingCountInArea.SetActive(isUnlock);
			buildingPrefab.ResetLockDisplay();
			bool flag4 = !isUnlock;
			if (flag4)
			{
				this.RefreshBuildingLockDisplay(item, buildingPrefab);
			}
			else
			{
				int count = ViewBuildingArea.GetBuildingCount(item.TemplateId, false);
				bool challengeUnique = item.Class != EBuildingBlockClass.Villiage && item.Class != EBuildingBlockClass.BornResource && this.challengeOpen;
				showChallengeWarning = (challengeUnique && count > 0);
				TextMeshProUGUI countTip = buildingPrefab.countTip;
				bool isBuilt = count > 0;
				countCanBuildCheck = ((item.IsUnique && !isBuilt) || !item.IsUnique);
				buildingPrefab.Status &= ~(BuildingOverviewBuildingPrefab.EBuildingStatus.NotBuild | BuildingOverviewBuildingPrefab.EBuildingStatus.AlreadyBuild);
				bool flag5 = isBuilt;
				if (flag5)
				{
					buildingPrefab.Status |= BuildingOverviewBuildingPrefab.EBuildingStatus.AlreadyBuild;
				}
				else
				{
					buildingPrefab.Status |= BuildingOverviewBuildingPrefab.EBuildingStatus.NotBuild;
				}
				bool flag6 = !isBuilt;
				if (flag6)
				{
					bool isUnique = item.IsUnique;
					if (isUnique)
					{
						countTip.text = LocalStringManager.GetFormat(LanguageKey.LK_BuildingOverview_BuildingCount_Tip2, "1".SetColor("b9b6b1"));
					}
					else
					{
						countTip.text = LocalStringManager.Get(LanguageKey.LK_Building_NotBuild);
					}
				}
				else
				{
					int countToShow = item.IsUnique ? 1 : count;
					countTip.text = LocalStringManager.GetFormat(LanguageKey.LK_BuildingOverview_BuildingCount_Tip1, countToShow).SetColor("f7f7f7");
				}
			}
			bool coreWarning = this.CheckShowCoreItemInfo(item);
			buildingPrefab.warningInfo.SetActive(coreWarning || showChallengeWarning);
			bool activeSelf = buildingPrefab.warningInfo.gameObject.activeSelf;
			if (activeSelf)
			{
				buildingPrefab.warningTip.SetText(LocalStringManager.Get(showChallengeWarning ? LanguageKey.LK_Building_Overview_Limit : LanguageKey.LK_Building_CoreItem_NotEnough).SetColor(showChallengeWarning ? "b9b6b1" : "ec5f68"), true);
			}
			bool flag7 = !coreWarning && !showChallengeWarning && countCanBuildCheck && !lackNormalBuilding && !lackResourcesBuilding;
			if (flag7)
			{
				buildingPrefab.Status |= BuildingOverviewBuildingPrefab.EBuildingStatus.CanBuild;
			}
			return buildingPrefab;
		}

		// Token: 0x060099E0 RID: 39392 RVA: 0x0047F12C File Offset: 0x0047D32C
		private void RefreshBuildingLockDisplay(BuildingBlockItem item, BuildingOverviewBuildingPrefab buildingPrefab)
		{
			bool flag = !this.CanUnlockBuildingByMainProgress(item);
			if (flag)
			{
				string mainLockTips = ViewBuildingOverview.GetMainProgressLockTips(item);
				buildingPrefab.ApplyMainProgressLock(LocalStringManager.Get(LanguageKey.LK_Building_DefaultUnLockTips));
				this.SetupMainProgressLockMouseTip(buildingPrefab, item, mainLockTips);
			}
			else
			{
				Config.LifeSkillItem lifeSkillItem;
				bool flag2 = !this.CanUnlockBuildingByLifeSkill(item) && this.TryGetLifeSkillBookLockTips(item, buildingPrefab.lockTips, out lifeSkillItem);
				if (flag2)
				{
					sbyte[] readingProgress = this.BuildLifeSkillReadingProgress(lifeSkillItem);
					SkillBookItem skillBookConfig = SkillBook.Instance[lifeSkillItem.SkillBookId];
					buildingPrefab.ApplyLifeSkillBookLock(lifeSkillItem, skillBookConfig, readingProgress);
				}
				else
				{
					buildingPrefab.ResetLockDisplay();
					bool flag3 = buildingPrefab.lockTips != null;
					if (flag3)
					{
						buildingPrefab.lockTips.text = LocalStringManager.Get(LanguageKey.LK_Building_DefaultUnLockTips);
					}
				}
			}
		}

		// Token: 0x060099E1 RID: 39393 RVA: 0x0047F1E8 File Offset: 0x0047D3E8
		private static string GetMainProgressLockTips(BuildingBlockItem item)
		{
			bool flag = item.TemplateId == 51;
			string result;
			if (flag)
			{
				result = NewFunctionUnlock.Instance[20].Desc;
			}
			else
			{
				bool flag2 = item.TemplateId == 50;
				if (flag2)
				{
					result = NewFunctionUnlock.Instance[21].Desc;
				}
				else
				{
					result = LocalStringManager.Get(LanguageKey.LK_Building_DefaultUnLockTips);
				}
			}
			return result;
		}

		// Token: 0x060099E2 RID: 39394 RVA: 0x0047F248 File Offset: 0x0047D448
		private void SetupMainProgressLockMouseTip(BuildingOverviewBuildingPrefab buildingPrefab, BuildingBlockItem item, string lockTipsText)
		{
			TooltipInvoker mouseTip = buildingPrefab.mouseTip;
			bool flag = mouseTip == null;
			if (!flag)
			{
				mouseTip.RuntimeParam = null;
				mouseTip.PresetParam = new string[]
				{
					item.Name,
					string.Concat(new string[]
					{
						lockTipsText,
						"\n\n<size=20><color=#8E8E8E>",
						item.Desc,
						"\n\n",
						item.FuncDesc,
						"</color></size>"
					})
				};
				bool showing = mouseTip.Showing;
				if (showing)
				{
					mouseTip.Refresh(false, -1);
				}
			}
		}

		// Token: 0x060099E3 RID: 39395 RVA: 0x0047F2D8 File Offset: 0x0047D4D8
		private sbyte[] BuildLifeSkillReadingProgress(Config.LifeSkillItem lifeSkillConfig)
		{
			GameData.Domains.Character.LifeSkillItem? learnedData = null;
			for (int i = 0; i < this._learnedLifeSkillItems.Count; i++)
			{
				bool flag = this._learnedLifeSkillItems[i].SkillTemplateId == lifeSkillConfig.TemplateId;
				if (flag)
				{
					learnedData = new GameData.Domains.Character.LifeSkillItem?(this._learnedLifeSkillItems[i]);
					break;
				}
			}
			sbyte[] progress = new sbyte[5];
			for (int j = 0; j < progress.Length; j++)
			{
				progress[j] = ((learnedData != null && learnedData.Value.IsPageRead((byte)j)) ? 100 : 0);
			}
			return progress;
		}

		// Token: 0x060099E4 RID: 39396 RVA: 0x0047F390 File Offset: 0x0047D590
		private bool TryGetLifeSkillBookLockTips(BuildingBlockItem item, TextMeshProUGUI tips, out Config.LifeSkillItem lifeSkillItem)
		{
			lifeSkillItem = null;
			tips.text = LocalStringManager.Get(LanguageKey.LK_Building_DefaultUnLockTips);
			bool flag = item.RequireLifeSkillType < 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < 9; i++)
				{
					List<short> idList = new List<short>();
					lifeSkillItem = LifeSkill.Instance[(int)(item.RequireLifeSkillType * 9) + i];
					CommonUtils.GetUnlockBuildingListFromConfig(lifeSkillItem, idList);
					bool flag2 = idList == null;
					if (!flag2)
					{
						bool flag3 = !idList.Contains(item.TemplateId);
						if (!flag3)
						{
							tips.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)(160 + 24 * lifeSkillItem.Name.Length));
							tips.text = (LocalStringManager.GetFormat(LanguageKey.LK_Building_LockTips, lifeSkillItem.Grade, lifeSkillItem.Name) ?? "").ColorReplace();
							return true;
						}
					}
				}
				lifeSkillItem = null;
				result = false;
			}
			return result;
		}

		// Token: 0x060099E5 RID: 39397 RVA: 0x0047F48C File Offset: 0x0047D68C
		private bool CheckShowCoreItemInfo(BuildingBlockItem item)
		{
			bool flag = false;
			bool flag2 = item.BuildingCoreItem != -1;
			if (flag2)
			{
				flag = true;
				ItemKey itemKey = new ItemKey(12, 0, item.BuildingCoreItem, -1);
				bool flag3 = this._canUseBuildingCore != null;
				if (flag3)
				{
					foreach (ItemDisplayData canUseItem in this._canUseBuildingCore)
					{
						bool flag4 = canUseItem.Key.ItemType == itemKey.ItemType && canUseItem.Key.TemplateId == itemKey.TemplateId && canUseItem.Amount > 0;
						if (flag4)
						{
							flag = false;
							break;
						}
					}
				}
				bool flag5 = this._cannotUseInventoryBuildingCore != null;
				if (flag5)
				{
					foreach (ItemDisplayData canUseItem2 in this._cannotUseInventoryBuildingCore)
					{
						bool flag6 = canUseItem2.Key.ItemType == itemKey.ItemType && canUseItem2.Key.TemplateId == itemKey.TemplateId && canUseItem2.Amount > 0;
						if (flag6)
						{
							flag = false;
							break;
						}
					}
				}
			}
			return flag;
		}

		// Token: 0x060099E6 RID: 39398 RVA: 0x0047F5F4 File Offset: 0x0047D7F4
		private bool CanUnlockBuildingByLifeSkill(BuildingBlockItem blockItem)
		{
			return !this._lockBuildingList.Contains(blockItem.TemplateId);
		}

		// Token: 0x060099E7 RID: 39399 RVA: 0x0047F61C File Offset: 0x0047D81C
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

		// Token: 0x060099E8 RID: 39400 RVA: 0x0047F6E0 File Offset: 0x0047D8E0
		private bool CanUnlockBuildingByMainProgress(BuildingBlockItem configData)
		{
			bool flag = configData.TemplateId == 51;
			bool result;
			if (flag)
			{
				result = SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(23);
			}
			else
			{
				bool flag2 = configData.TemplateId == 50;
				result = (!flag2 || SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(12));
			}
			return result;
		}

		// Token: 0x060099E9 RID: 39401 RVA: 0x0047F72F File Offset: 0x0047D92F
		private void ResetState()
		{
			this.ClearOperatorListCache();
			this._configData = null;
			this.UpdateCostInfo();
			this.UpdateOperatorInfo();
			this.UpdateConfirmButton();
			this.RefreshAutoArrangeBtn();
		}

		// Token: 0x060099EA RID: 39402 RVA: 0x0047F75C File Offset: 0x0047D95C
		private void ClearOperatorListCache()
		{
			for (int i = 0; i < this._operatorListCached.Length; i++)
			{
				this._operatorListCached[i] = -1;
			}
		}

		// Token: 0x060099EB RID: 39403 RVA: 0x0047F78A File Offset: 0x0047D98A
		private void SetOperator(int charId)
		{
			this._operatorListCached[this._selectingOperatorIndex] = charId;
			this.UpdateOperatorInfo();
			this.UpdateConfirmButton();
			this.RefreshAutoArrangeBtn();
		}

		// Token: 0x060099EC RID: 39404 RVA: 0x0047F7B0 File Offset: 0x0047D9B0
		private void UpdateCostInfo()
		{
			this.resourcesCostTitle.SetActive(true);
			this.resourceObjList[0].transform.parent.gameObject.SetActive(true);
			bool flag = this._configData == null;
			if (flag)
			{
				for (int i = 0; i < 8; i++)
				{
					this.resourceObjList[i].SetActive(true);
					this.resourceTextList[i].text = "- / -".SetColor("b9b6b1");
				}
			}
			else
			{
				List<int> resourceCountList = EasyPool.Get<List<int>>();
				resourceCountList.Clear();
				bool flag2 = this._configData.Class == EBuildingBlockClass.BornResource;
				if (flag2)
				{
					for (int j = 0; j < this._configData.BaseBuildCost.Length; j++)
					{
						resourceCountList.Add(0);
					}
				}
				else
				{
					for (int k = 0; k < this._configData.BaseBuildCost.Length; k++)
					{
						resourceCountList.Add((int)this._configData.BaseBuildCost[k]);
					}
				}
				this._hasEnoughResources = true;
				for (sbyte l = 0; l < 8; l += 1)
				{
					this.resourceObjList[(int)l].SetActive(resourceCountList[(int)l] > 0);
					bool flag3 = resourceCountList[(int)l] > 0;
					if (flag3)
					{
						string color = (this._buildingModel.GetResourceCount(l) >= resourceCountList[(int)l]) ? "8dc3c3" : "ec5f68";
						string resourceStr = CommonUtils.GetDisplayStringForNum(this._buildingModel.GetResourceCount(l), 100000).SetColor(color) + "/" + CommonUtils.GetDisplayStringForNum(resourceCountList[(int)l], 100000);
						bool flag4 = this._buildingModel.GetResourceCount(l) < resourceCountList[(int)l];
						if (flag4)
						{
							this._hasEnoughResources = false;
						}
						this.resourceTextList[(int)l].text = resourceStr;
					}
				}
				this._hasEnoughCore = false;
				bool flag5 = this._configData != null && this._configData.BuildingCoreItem != -1;
				if (flag5)
				{
					this.SetBuildingCoreActive(true);
					this.coreItemHolder.itemName.SetText(Misc.Instance.GetItem(this._configData.BuildingCoreItem).Name, true);
					this.coreItemHolder.itemDesc.SetText(Misc.Instance.GetItem(this._configData.BuildingCoreItem).Desc, true);
					ValueTuple<bool, int> result = GameData.Domains.Building.SharedMethods.HasBuildingCore(this._configData, (this._cannotUseInventoryBuildingCore != null) ? this._canUseBuildingCore.Concat(this._cannotUseInventoryBuildingCore).ToList<ItemDisplayData>() : this._canUseBuildingCore);
					this._hasEnoughCore = result.Item1;
					int count = result.Item2;
					string curCount = string.Format("{0}", count);
					bool flag6 = count < 1;
					if (flag6)
					{
						curCount = ("<color=#brightred>" + curCount + "</color>").ColorReplace();
					}
					else
					{
						curCount = ("<color=#brightblue>" + curCount + "</color>").ColorReplace();
					}
					ItemDisplayData coreItem = new ItemDisplayData(12, this._configData.BuildingCoreItem);
					this.coreItemHolder.coreItemView.Set(coreItem, false);
					this.coreItemHolder.cur.text = curCount + "/1";
					this.coreItemHolder.mouseTip.Type = TipType.Misc;
					TooltipInvoker mouseTip = this.coreItemHolder.mouseTip;
					if (mouseTip.RuntimeParam == null)
					{
						mouseTip.RuntimeParam = EasyPool.Get<ArgumentBox>();
					}
					this.coreItemHolder.mouseTip.RuntimeParam.SetObject("ItemData", coreItem).Set("TemplateDataOnly", true);
					this.UpdateConfirmButton();
				}
				else
				{
					this._hasEnoughCore = true;
					this.SetBuildingCoreActive(false);
				}
				this.UpdateBuildingSpaceInfo();
				EasyPool.Free<List<int>>(resourceCountList);
			}
		}

		// Token: 0x060099ED RID: 39405 RVA: 0x0047FBB6 File Offset: 0x0047DDB6
		private void SetBuildingCoreActive(bool show)
		{
			this.coreItemHolder.gameObject.SetActive(show);
			this.coreItemHolder.coreItemView.gameObject.SetActive(show);
		}

		// Token: 0x060099EE RID: 39406 RVA: 0x0047FBE4 File Offset: 0x0047DDE4
		private void UpdateOperatorInfo()
		{
			this.quickClearOperatorBtn.GetComponent<CButton>().interactable = !this._operatorListCached.All((int charId) => charId < 0);
			int maxOps = this._operatorListCached.Length;
			List<int> charIds = new List<int>();
			for (int i = 0; i < maxOps; i++)
			{
				bool flag = this._operatorListCached[i] >= 0;
				if (flag)
				{
					charIds.Add(this._operatorListCached[i]);
				}
			}
			this.SetOperationLeftTime(this._operatorListCached.ToList<int>());
			this.SetOperatorCount(charIds);
			bool flag2 = charIds.Count == 0;
			if (flag2)
			{
				for (int j = 0; j < this.operatorList.Count; j++)
				{
					this.operatorList[j].gameObject.SetActive(j < maxOps);
					bool flag3 = j < maxOps;
					if (flag3)
					{
						this.operatorList[j].SetForOperator(null, j, new Action<int>(this.OnSelectOperator), new Action<int>(this.OnCancelOperator), this, false, null);
					}
				}
			}
			else
			{
				CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this, charIds, delegate(int offset, RawDataPool pool)
				{
					List<CharacterDisplayData> dataList = null;
					Serializer.Deserialize(pool, offset, ref dataList);
					Dictionary<int, CharacterDisplayData> dataDict = new Dictionary<int, CharacterDisplayData>();
					bool flag4 = dataList != null;
					if (flag4)
					{
						foreach (CharacterDisplayData data in dataList)
						{
							dataDict[data.CharacterId] = data;
						}
					}
					for (int k = 0; k < this.operatorList.Count; k++)
					{
						this.operatorList[k].gameObject.SetActive(k < maxOps);
						bool flag5 = k >= maxOps;
						if (!flag5)
						{
							int charId = this._operatorListCached[k];
							CharacterDisplayData d;
							CharacterDisplayData charData = (charId >= 0 && dataDict.TryGetValue(charId, out d)) ? d : null;
							bool flag6 = charId >= 0;
							if (flag6)
							{
								BuildingDomainMethod.Call.SetUnlockedWorkingVillagers(charId, false);
							}
							this.operatorList[k].SetForOperator(charData, k, new Action<int>(this.OnSelectOperator), new Action<int>(this.OnCancelOperator), this, false, new Action<int, bool>(this.SetUnlockedWorkingVillager));
						}
					}
				});
			}
		}

		// Token: 0x060099EF RID: 39407 RVA: 0x0047FD53 File Offset: 0x0047DF53
		private void SetUnlockedWorkingVillager(int charId, bool isUnlock)
		{
			BuildingDomainMethod.Call.SetUnlockedWorkingVillagers(charId, isUnlock);
		}

		// Token: 0x060099F0 RID: 39408 RVA: 0x0047FD60 File Offset: 0x0047DF60
		private void SetOperatorCount(List<int> charIds)
		{
			StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
			stringBuilder.Append(LocalStringManager.Get("LK_Building_BuildManpower")).Append("  (").Append(charIds.Count).Append("/").Append(3).Append(")");
			this.titleExpandText.text = stringBuilder.ToString();
			EasyPool.Free<StringBuilder>(stringBuilder);
		}

		// Token: 0x060099F1 RID: 39409 RVA: 0x0047FDD0 File Offset: 0x0047DFD0
		private void OnSelectOperator(int index)
		{
			ViewBuildingOverview.<>c__DisplayClass146_0 CS$<>8__locals1 = new ViewBuildingOverview.<>c__DisplayClass146_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.index = index;
			this._selectingOperatorIndex = CS$<>8__locals1.index;
			List<int> availableWorker = (from id in this._availableWorkers
			where !CS$<>8__locals1.<>4__this._operatorListCached.Contains(id)
			select id).ToList<int>();
			bool flag = availableWorker != null && availableWorker.Count > 0;
			if (flag)
			{
				CS$<>8__locals1.<OnSelectOperator>g__OpenSelectCharPanel|2(availableWorker);
			}
			else
			{
				TaiwuDomainMethod.AsyncCall.GetAllVillagersAvailableForWork(this, delegate(int offset, RawDataPool pool)
				{
					List<int> workerList = new List<int>();
					Serializer.Deserialize(pool, offset, ref workerList);
					base.<OnSelectOperator>g__OpenSelectCharPanel|2(workerList);
				});
			}
		}

		// Token: 0x060099F2 RID: 39410 RVA: 0x0047FE50 File Offset: 0x0047E050
		private void OnCharSelected(int charId)
		{
			int index = this._selectingOperatorIndex;
			bool flag = this._operatorListCached[index] == charId;
			if (!flag)
			{
				this._operatorListCached[index] = charId;
				this.UpdateOperatorInfo();
				this.UpdateConfirmButton();
				this.RefreshAutoArrangeBtn();
			}
		}

		// Token: 0x060099F3 RID: 39411 RVA: 0x0047FE94 File Offset: 0x0047E094
		private void OnCancelOperator(int index)
		{
			bool flag = this._operatorListCached[index] == -1;
			if (!flag)
			{
				this._operatorListCached[index] = -1;
				this.UpdateOperatorInfo();
				this.UpdateConfirmButton();
				this.RefreshAutoArrangeBtn();
			}
		}

		// Token: 0x060099F4 RID: 39412 RVA: 0x0047FED4 File Offset: 0x0047E0D4
		private void UpdateConfirmButton()
		{
			bool flag = this._configData != null;
			if (flag)
			{
				string tipDesc = "";
				bool flag2 = this.CanExecuteBuild(out tipDesc);
				if (flag2)
				{
					this.confirmOperation.gameObject.SetActive(true);
					this.confirmOperation.interactable = true;
					this.timeCostEstimate.gameObject.SetActive(true);
					this.unlockTips.gameObject.SetActive(false);
					TooltipInvoker tipDisplayer = this.confirmOperation.GetComponent<TooltipInvoker>();
					tipDisplayer.PresetParam[0] = LocalStringManager.Get("LK_Building_Start_Build");
					tipDisplayer.PresetParam[1] = tipDesc;
				}
				else
				{
					string tipsDesc2 = "";
					bool flag3 = this.IsSeletdUnlockBylife(out tipsDesc2);
					if (flag3)
					{
						this.confirmOperation.gameObject.SetActive(true);
						this.confirmOperation.interactable = false;
						this.timeCostEstimate.gameObject.SetActive(true);
						this.unlockTips.gameObject.SetActive(false);
						TooltipInvoker tipDisplayer2 = this.confirmOperation.GetComponent<TooltipInvoker>();
						tipDisplayer2.PresetParam[0] = LocalStringManager.Get("LK_Building_Start_Build");
						tipDisplayer2.PresetParam[1] = tipDesc;
					}
					else
					{
						this.confirmOperation.gameObject.SetActive(false);
						this.timeCostEstimate.gameObject.SetActive(false);
						this.unlockTips.gameObject.SetActive(true);
						this.unlockTipsText.text = tipsDesc2;
					}
				}
			}
			else
			{
				this.confirmOperation.gameObject.SetActive(false);
				this.timeCostEstimate.gameObject.SetActive(false);
			}
		}

		// Token: 0x060099F5 RID: 39413 RVA: 0x00480070 File Offset: 0x0047E270
		private void UpdateDesc()
		{
			List<CToggle> cToggles = this._toggleGroupMultiSelect.GetAll();
			for (int i = 0; i < cToggles.Count; i++)
			{
				bool isOn = cToggles[i].isOn;
				if (isOn)
				{
					BuildingOverviewBuildingPrefab refers = cToggles[i].gameObject.GetComponent<BuildingOverviewBuildingPrefab>();
					refers.desc1.text = this._configData.Desc.ColorReplace();
					refers.desc2.text = this._configData.FuncDesc.ColorReplace();
					refers.needBlock.text = ((this._configData.Width == 2) ? "4/4" : "1/4");
					refers.needBlockImage.SetSprite((this._configData.Width == 2) ? "building_zhange_1" : "building_zhange_0", false, null);
					refers.needBlockImage.gameObject.SetActive(true);
					this.buildingName.text = (this._configData.Name ?? "");
					this._customName = "";
					break;
				}
			}
		}

		// Token: 0x060099F6 RID: 39414 RVA: 0x00480194 File Offset: 0x0047E394
		public override void QuickHide()
		{
			AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			base.QuickHide();
		}

		// Token: 0x060099F7 RID: 39415 RVA: 0x004801B0 File Offset: 0x0047E3B0
		private unsafe void SetOperationLeftTime(List<int> operators)
		{
			string timeStr = "-";
			TextMeshProUGUI leftTimeText = this.timeCost;
			bool flag = this._configData == null;
			if (flag)
			{
				leftTimeText.SetText(timeStr, true);
			}
			else
			{
				int totalProgress = (int)this._configData.OperationTotalProgress[0];
				bool flag2 = SingletonObject.getInstance<TutorialChapterModel>().InGuiding && SingletonObject.getInstance<TutorialChapterModel>().TutorialChapterIndex == 1 && this._operatorListCached[0] == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				if (flag2)
				{
					CharacterItem charConfig = Character.Instance.GetItem(908);
					int speed = (int)(*(ref charConfig.BaseLifeSkillQualifications.Items.FixedElementField + (IntPtr)15 * 2));
					bool flag3 = speed > 0;
					if (flag3)
					{
						timeStr = Math.Ceiling((double)((float)totalProgress / (float)speed)).ToString();
					}
					leftTimeText.SetText(timeStr, true);
				}
				else
				{
					BuildingDomainMethod.AsyncCall.GetOperationLeftTime(this, this._configData.TemplateId, BuildingBlockKey.Invalid, 0, operators, delegate(int offset, RawDataPool dataPool)
					{
						int leftTime = 0;
						Serializer.Deserialize(dataPool, offset, ref leftTime);
						leftTimeText.text = ((leftTime > 0) ? leftTime.ToString() : "-");
					});
				}
			}
		}

		// Token: 0x060099F8 RID: 39416 RVA: 0x004802C0 File Offset: 0x0047E4C0
		private bool IsSeletdUnlockBylife(out string tipDesc)
		{
			tipDesc = string.Empty;
			bool flag = this._configData == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !this.CanUnlockBuildingByLifeSkill(this._configData);
				if (flag2)
				{
					tipDesc = LocalStringManager.Get(LanguageKey.LK_Building_DefaultUnLockTips);
					bool flag3 = this._configData.RequireLifeSkillType >= 0;
					if (flag3)
					{
						for (int i = 0; i < 9; i++)
						{
							List<short> idList = new List<short>();
							Config.LifeSkillItem lifeSkillItem = LifeSkill.Instance[(int)(this._configData.RequireLifeSkillType * 9) + i];
							CommonUtils.GetUnlockBuildingListFromConfig(lifeSkillItem, idList);
							bool flag4 = idList == null;
							if (!flag4)
							{
								bool flag5 = idList.Contains(this._configData.TemplateId);
								if (flag5)
								{
									tipDesc = (LocalStringManager.GetFormat(LanguageKey.LK_Building_LockTips, lifeSkillItem.Grade, lifeSkillItem.Name) ?? "").ColorReplace();
									break;
								}
							}
						}
					}
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060099F9 RID: 39417 RVA: 0x004803D4 File Offset: 0x0047E5D4
		private bool CanExecuteBuild(out string tipDesc)
		{
			bool flag = this._configData == null;
			bool result2;
			if (flag)
			{
				tipDesc = string.Empty;
				result2 = false;
			}
			else
			{
				bool result = true;
				StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
				strBuilder.Clear();
				bool flag2 = !this._operatorListCached.Exist((int id) => id >= 0);
				if (flag2)
				{
					result = false;
					strBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Cannot_Build_No_Operator) ?? "");
				}
				bool flag3 = !this._hasEnoughResources;
				if (flag3)
				{
					result = false;
					strBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Cannot_Build_Resource_Not_Enough) ?? "");
				}
				bool flag4 = this._configData != null && this._configData.BuildingCoreItem != -1;
				if (flag4)
				{
					bool flag5 = !this._hasEnoughCore;
					if (flag5)
					{
						result = false;
						strBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Cannot_Build_CoreItem_Not_Enough) ?? "");
					}
				}
				BuildingBlockItem configData = this._configData;
				bool flag6 = configData != null && configData.IsUnique && this._buildingArea.ContainsBuilding(this._configData.TemplateId, false);
				if (flag6)
				{
					result = false;
					strBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Cannot_Build_Unique_Building) ?? "");
				}
				bool challengeUnique = this._configData.Class != EBuildingBlockClass.Villiage && this._configData.Class != EBuildingBlockClass.BornResource && this.challengeOpen;
				int count = ViewBuildingArea.GetBuildingCount(this._configData.TemplateId, false);
				bool flag7 = challengeUnique && count > 0;
				if (flag7)
				{
					result = false;
					strBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Building_Overview_Limit).SetColor("brightred").ColorReplace() ?? "");
				}
				bool flag8 = ViewBuildingOverview.SpaceMeet(this._configData, this._buildingSpaceLimit - this._buildingSpaceCurr);
				if (flag8)
				{
					result = false;
					strBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Building_BuildingSpaceLack) ?? "");
				}
				bool flag9 = this._dependNormalBuildingsLack || this._dependResourcesBuildingsLack;
				if (flag9)
				{
					result = false;
					bool dependNormalBuildingsLack = this._dependNormalBuildingsLack;
					if (dependNormalBuildingsLack)
					{
						strBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Building_DependNormalBuildingsLack) ?? "");
					}
					bool dependResourcesBuildingsLack = this._dependResourcesBuildingsLack;
					if (dependResourcesBuildingsLack)
					{
						strBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Building_DependResourcesBuildingsLack) ?? "");
					}
				}
				bool isUnlockByLiftSkill = this.CanUnlockBuildingByLifeSkill(this._configData);
				bool isUnlockByMainProgress = this.CanUnlockBuildingByMainProgress(this._configData);
				bool isUnlock = isUnlockByLiftSkill && isUnlockByMainProgress;
				bool flag10 = !isUnlock;
				if (flag10)
				{
					result = false;
					strBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_BuildingOverview_NotUnlock) ?? "");
				}
				strBuilder.Append(LocalStringManager.Get("LK_Building_Start_Build_Tip_Desc"));
				tipDesc = strBuilder.ToString();
				EasyPool.Free<StringBuilder>(strBuilder);
				result2 = result;
			}
			return result2;
		}

		// Token: 0x060099FA RID: 39418 RVA: 0x004806C0 File Offset: 0x0047E8C0
		public static bool SpaceMeet(BuildingBlockItem config, int leftSpace)
		{
			int needSpace = (int)(BuildingBlockData.IsUsefulResource(config.Type) ? 0 : config.Width);
			return needSpace > 0 && leftSpace < needSpace;
		}

		// Token: 0x060099FB RID: 39419 RVA: 0x004806F4 File Offset: 0x0047E8F4
		private void AutoSelectBuildingToggle(short selectTemplateId)
		{
			CToggle toggle;
			bool flag = this._autoSelectBuildingAfterRefresh && this._buildingToggleMap.TryGetValue(selectTemplateId, out toggle);
			if (flag)
			{
				RectTransform rectTransform = toggle.GetComponent<RectTransform>();
				base.StartCoroutine(this.SetBuildingScrollPosition(rectTransform));
				toggle.isOn = true;
				this._toggleGroupMultiSelect.NotifyToggle(toggle, true, true);
			}
			else
			{
				this._toggleGroupMultiSelect.Select(0, true);
			}
			this.mask.gameObject.SetActive(false);
		}

		// Token: 0x060099FC RID: 39420 RVA: 0x00480771 File Offset: 0x0047E971
		private IEnumerator SetBuildingScrollPosition(RectTransform rectTransform)
		{
			yield return new WaitForEndOfFrame();
			this.EnsureBuildingListLayoutUpdated();
			this.BeginSyncLeftTypeToggleFromProgramScroll();
			bool flag = !this.buildingScroll.ScrollTo(rectTransform, 0.3f);
			if (flag)
			{
				this.CancelSyncLeftTypeToggleFromProgramScroll();
			}
			yield break;
		}

		// Token: 0x060099FD RID: 39421 RVA: 0x00480787 File Offset: 0x0047E987
		private IEnumerator SetBuildingScrollToTop()
		{
			yield return new WaitForSeconds(0.1f);
			this.EnsureBuildingListLayoutUpdated();
			this.BeginSyncLeftTypeToggleFromProgramScroll();
			this.buildingScroll.ScrollTo(Vector2.zero, 0.3f);
			yield break;
		}

		// Token: 0x0400766C RID: 30316
		[SerializeField]
		private BuildingOverviewBuildingPrefab buildingPrefabTemplate;

		// Token: 0x0400766D RID: 30317
		[SerializeField]
		private BuildingOverviewTypeBuildingPrefab typeBuildingPrefab;

		// Token: 0x0400766E RID: 30318
		[SerializeField]
		private RectTransform content;

		// Token: 0x0400766F RID: 30319
		[SerializeField]
		private CScrollRect buildingScroll;

		// Token: 0x04007670 RID: 30320
		[SerializeField]
		private List<BuildingManagerMemberView> operatorList;

		// Token: 0x04007671 RID: 30321
		[SerializeField]
		private TextMeshProUGUI timeCost;

		// Token: 0x04007672 RID: 30322
		[SerializeField]
		private List<GameObject> resourceObjList;

		// Token: 0x04007673 RID: 30323
		[SerializeField]
		private List<TextMeshProUGUI> resourceTextList;

		// Token: 0x04007674 RID: 30324
		[SerializeField]
		private CButton confirmOperation;

		// Token: 0x04007675 RID: 30325
		[SerializeField]
		private CToggleGroup leftTypeToggleGroup;

		// Token: 0x04007676 RID: 30326
		[SerializeField]
		private TextMeshProUGUI titleExpandText;

		// Token: 0x04007677 RID: 30327
		[SerializeField]
		private TextMeshProUGUI buildingName;

		// Token: 0x04007678 RID: 30328
		[SerializeField]
		private GameObject input;

		// Token: 0x04007679 RID: 30329
		[SerializeField]
		private TextMeshProUGUI desc1;

		// Token: 0x0400767A RID: 30330
		[SerializeField]
		private TextMeshProUGUI desc2;

		// Token: 0x0400767B RID: 30331
		[SerializeField]
		private TextMeshProUGUI needBlock;

		// Token: 0x0400767C RID: 30332
		[SerializeField]
		private CImage needBlockImage;

		// Token: 0x0400767D RID: 30333
		[SerializeField]
		private TextMeshProUGUI buildingSpaceInfoText;

		// Token: 0x0400767E RID: 30334
		[SerializeField]
		private GameObject operationInfo;

		// Token: 0x0400767F RID: 30335
		[SerializeField]
		private GameObject timeCostEstimate;

		// Token: 0x04007680 RID: 30336
		[SerializeField]
		private BuildingOverViewCoreItemHolder coreItemHolder;

		// Token: 0x04007681 RID: 30337
		[SerializeField]
		private CButton autoArrangeBtn;

		// Token: 0x04007682 RID: 30338
		[SerializeField]
		private GameObject resourcesCostTitle;

		// Token: 0x04007683 RID: 30339
		[SerializeField]
		private GameObject mask;

		// Token: 0x04007684 RID: 30340
		[SerializeField]
		private TextMeshProUGUI title;

		// Token: 0x04007685 RID: 30341
		[SerializeField]
		private GameObject quickClearOperatorBtn;

		// Token: 0x04007686 RID: 30342
		[SerializeField]
		private TextMeshProUGUI confirmLabel;

		// Token: 0x04007687 RID: 30343
		[SerializeField]
		private GameObject unlockTips;

		// Token: 0x04007688 RID: 30344
		[SerializeField]
		private TextMeshProUGUI unlockTipsText;

		// Token: 0x04007689 RID: 30345
		[SerializeField]
		private GameObject emptyGo;

		// Token: 0x0400768A RID: 30346
		[SerializeField]
		private SortAndFilter sortAndFilter;

		// Token: 0x0400768B RID: 30347
		[SerializeField]
		private RectMask2D buildInfoMask;

		// Token: 0x0400768C RID: 30348
		public Vector2 buildingPrefabSize;

		// Token: 0x0400768D RID: 30349
		private ViewBuildingArea _buildingArea;

		// Token: 0x0400768E RID: 30350
		private BuildingBlockItem _configData;

		// Token: 0x0400768F RID: 30351
		private CToggleGroupMultiSelect _toggleGroupMultiSelect;

		// Token: 0x04007690 RID: 30352
		private readonly Dictionary<short, CToggle> _buildingToggleMap = new Dictionary<short, CToggle>();

		// Token: 0x04007691 RID: 30353
		private readonly Dictionary<short, BuildingOverviewBuildingPrefab> _buildingPrefabMap = new Dictionary<short, BuildingOverviewBuildingPrefab>();

		// Token: 0x04007692 RID: 30354
		private int _taiwuCharId;

		// Token: 0x04007693 RID: 30355
		private short _settlementId;

		// Token: 0x04007694 RID: 30356
		private string _customName;

		// Token: 0x04007695 RID: 30357
		private int _buildingSpaceCurr;

		// Token: 0x04007696 RID: 30358
		private int _buildingSpaceLimit;

		// Token: 0x04007697 RID: 30359
		private BuildingModel _buildingModel;

		// Token: 0x04007698 RID: 30360
		private List<GameData.Domains.Character.LifeSkillItem> _learnedLifeSkillItems = new List<GameData.Domains.Character.LifeSkillItem>();

		// Token: 0x04007699 RID: 30361
		private BuildingOverviewSortAndFilterController _buildingOverviewSortAndFilterController;

		// Token: 0x0400769A RID: 30362
		private const int LeftSpecialToggleCount = 5;

		// Token: 0x0400769B RID: 30363
		private const float LeftTypeScrollTopPadding = 5f;

		// Token: 0x0400769C RID: 30364
		private static readonly Vector3[] ScrollSyncCornersBuffer = new Vector3[4];

		// Token: 0x0400769D RID: 30365
		private readonly List<ViewBuildingOverview.BuildingTypeState> _typeBuildingItems = new List<ViewBuildingOverview.BuildingTypeState>();

		// Token: 0x0400769E RID: 30366
		private bool _isSyncingLeftTypeToggle;

		// Token: 0x0400769F RID: 30367
		private readonly List<short> _lockBuildingList = new List<short>();

		// Token: 0x040076A0 RID: 30368
		private const string Blue = "8dc3c3";

		// Token: 0x040076A1 RID: 30369
		private const string Red = "ec5f68";

		// Token: 0x040076A2 RID: 30370
		private const string White = "b9b6b1";

		// Token: 0x040076A3 RID: 30371
		private const string White_Light = "f7f7f7";

		// Token: 0x040076A4 RID: 30372
		private bool _dependNormalBuildingsLack;

		// Token: 0x040076A5 RID: 30373
		private bool _dependResourcesBuildingsLack;

		// Token: 0x040076A6 RID: 30374
		private bool _isHaveChickenKing;

		// Token: 0x040076A7 RID: 30375
		private List<ItemDisplayData> _canUseBuildingCore = new List<ItemDisplayData>();

		// Token: 0x040076A8 RID: 30376
		private List<ItemDisplayData> _cannotUseInventoryBuildingCore = new List<ItemDisplayData>();

		// Token: 0x040076A9 RID: 30377
		private Dictionary<EBuildingBlockClass, List<BuildingBlockItem>> _buildingMap;

		// Token: 0x040076AA RID: 30378
		private const string BuildingTypePrefabKey = "UI_BuildingOverview_BuildingTypeObject";

		// Token: 0x040076AB RID: 30379
		private const string BuildingItemPrefabKey = "UI_BuildingOverview_BuildingItemObject";

		// Token: 0x040076AC RID: 30380
		private short _autoSelectBuildingTemplateId;

		// Token: 0x040076AD RID: 30381
		private bool _autoSelectBuildingAfterRefresh;

		// Token: 0x040076AE RID: 30382
		private readonly Vector2 _cellSizeVector2 = new Vector2(308f, 418f);

		// Token: 0x040076AF RID: 30383
		private bool challengeOpen;

		// Token: 0x040076B0 RID: 30384
		private bool shrineBuild;

		// Token: 0x040076B1 RID: 30385
		private bool _needClearSortAndFilterOnInit;

		// Token: 0x040076B2 RID: 30386
		private bool _isWaitingBlockList;

		// Token: 0x040076B3 RID: 30387
		private readonly int[] _operatorListCached = new int[3];

		// Token: 0x040076B4 RID: 30388
		private readonly List<int> _villagerList = new List<int>();

		// Token: 0x040076B5 RID: 30389
		private List<int> _availableWorkers = new List<int>();

		// Token: 0x040076B6 RID: 30390
		private readonly Dictionary<int, CharacterDisplayData> _charDisplayDataDict = new Dictionary<int, CharacterDisplayData>();

		// Token: 0x040076B7 RID: 30391
		private bool _hasEnoughResources;

		// Token: 0x040076B8 RID: 30392
		private bool _hasEnoughCore;

		// Token: 0x040076B9 RID: 30393
		private int _selectingOperatorIndex;

		// Token: 0x020022C1 RID: 8897
		private class BuildingTypeState
		{
			// Token: 0x0400DC0D RID: 56333
			public BuildingOverviewTypeBuildingPrefab Panel;

			// Token: 0x0400DC0E RID: 56334
			public bool ShouldState;

			// Token: 0x0400DC0F RID: 56335
			public EBuildingBlockClass BlockClass;
		}
	}
}
