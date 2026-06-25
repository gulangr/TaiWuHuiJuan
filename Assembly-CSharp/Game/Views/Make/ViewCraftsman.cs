using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Coffee.UIExtensions;
using Config;
using Config.ConfigCells;
using FrameWork;
using FrameWork.UISystem.Components.EffectPlayer;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Item;
using Game.Components.SortAndFilter.Item.Apply;
using Game.Views.Building.BuildingManage;
using Game.Views.Item;
using Game.Views.Select;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Views.Make
{
	// Token: 0x02000960 RID: 2400
	public class ViewCraftsman : UIBase
	{
		// Token: 0x17000D08 RID: 3336
		// (get) Token: 0x06007291 RID: 29329 RVA: 0x00353725 File Offset: 0x00351925
		private int AddProgress
		{
			get
			{
				return this._selectedItemDict.Sum((KeyValuePair<ItemDisplayData, int> p) => p.Value * this.GetItemProgress(p.Key));
			}
		}

		// Token: 0x17000D09 RID: 3337
		// (get) Token: 0x06007292 RID: 29330 RVA: 0x0035373E File Offset: 0x0035193E
		private int RemainProgress
		{
			get
			{
				return this._needProgress - this.AddProgress;
			}
		}

		// Token: 0x17000D0A RID: 3338
		// (get) Token: 0x06007293 RID: 29331 RVA: 0x0035374D File Offset: 0x0035194D
		private ViewMake.ItemSourceTogKey CurResourceTogKey
		{
			get
			{
				return (ViewMake.ItemSourceTogKey)this.resourceToggleGroup.GetActiveIndex();
			}
		}

		// Token: 0x17000D0B RID: 3339
		// (get) Token: 0x06007294 RID: 29332 RVA: 0x0035375A File Offset: 0x0035195A
		private ItemSourceType CurResourceItemSource
		{
			get
			{
				return ViewMake.GetItemSourceType(this.CurResourceTogKey);
			}
		}

		// Token: 0x17000D0C RID: 3340
		// (get) Token: 0x06007295 RID: 29333 RVA: 0x00353767 File Offset: 0x00351967
		private ViewMake.ItemSourceTogKey CurMaterialTogKey
		{
			get
			{
				return (ViewMake.ItemSourceTogKey)this.materialToggleGroup.GetActiveIndex();
			}
		}

		// Token: 0x17000D0D RID: 3341
		// (get) Token: 0x06007296 RID: 29334 RVA: 0x00353774 File Offset: 0x00351974
		private ItemSourceType CurMaterialItemSource
		{
			get
			{
				return ViewMake.GetItemSourceType(this.CurMaterialTogKey);
			}
		}

		// Token: 0x17000D0E RID: 3342
		// (get) Token: 0x06007297 RID: 29335 RVA: 0x00353781 File Offset: 0x00351981
		private short CurItemSubType
		{
			get
			{
				return this.GetItemSubType(this._currentCraftType);
			}
		}

		// Token: 0x17000D0F RID: 3343
		// (get) Token: 0x06007298 RID: 29336 RVA: 0x0035378F File Offset: 0x0035198F
		private bool IsBuilding
		{
			get
			{
				return this._panelMode == ViewCraftsman.EPanelMode.Building;
			}
		}

		// Token: 0x17000D10 RID: 3344
		// (get) Token: 0x06007299 RID: 29337 RVA: 0x0035379A File Offset: 0x0035199A
		private sbyte LiftSkillType
		{
			get
			{
				return this.IsBuilding ? this._buildingConfig.RequireLifeSkillType : ViewCraftsman.GetOperationNeedSkillType(this._currentCraftType);
			}
		}

		// Token: 0x0600729A RID: 29338 RVA: 0x003537BC File Offset: 0x003519BC
		private void OnEnable()
		{
			SingletonObject.getInstance<LifeSkillCombatModel>().EndEvent += this.OnLifeSkillCombatEnd;
		}

		// Token: 0x0600729B RID: 29339 RVA: 0x003537D6 File Offset: 0x003519D6
		private void OnDisable()
		{
			SingletonObject.getInstance<LifeSkillCombatModel>().EndEvent -= this.OnLifeSkillCombatEnd;
		}

		// Token: 0x0600729C RID: 29340 RVA: 0x003537F0 File Offset: 0x003519F0
		public override void OnInit(ArgumentBox argsBox)
		{
			this.canvasGroup.alpha = 0f;
			bool flag = argsBox.Get("charId", out this._artisanId);
			if (flag)
			{
				argsBox.Get("craftsmanPanelType", out this._craftsmanType);
				this._panelMode = ViewCraftsman.EPanelMode.Character;
				this.InitCraftTypeList();
				this.InitCraftTypeToggleGroup();
			}
			else
			{
				argsBox.Get<BuildingBlockKey>("blockKey", out this._blockKey);
				argsBox.Get<BuildingBlockData>("blockData", out this._blockData);
				this._panelMode = ViewCraftsman.EPanelMode.Building;
				this._artisanId = -1;
				this._buildingConfig = BuildingBlock.Instance[this._blockData.TemplateId];
				this.InitSwitchBuilding();
				this._currentCraftType = this.GetCraftTypeForBuilding();
			}
			this.SetProductListScroll();
			this.buildingPanel.gameObject.SetActive(this.IsBuilding);
			this.characterPanel.gameObject.SetActive(!this.IsBuilding);
			this.NeedWaitData = true;
			this.RequestData();
		}

		// Token: 0x0600729D RID: 29341 RVA: 0x003538F8 File Offset: 0x00351AF8
		private void RequestData()
		{
			bool isBuilding = this.IsBuilding;
			if (isBuilding)
			{
				BuildingDomainMethod.AsyncCall.GetCraftManDisplayDataForBuilding(this, this._blockKey, delegate(int offset, RawDataPool pool)
				{
					Serializer.Deserialize(pool, offset, ref this._displayData);
					this.Refresh();
				});
			}
			else
			{
				BuildingDomainMethod.AsyncCall.GetCraftManDisplayDataForCharacter(this, this._artisanId, delegate(int offset, RawDataPool pool)
				{
					Serializer.Deserialize(pool, offset, ref this._displayData);
					bool flag = this._displayData.ArtisanOrder == null;
					if (flag)
					{
						ExtraDomainMethod.AsyncCall.GetProductionPoolPreview(this, this._artisanId, this.LiftSkillType, delegate(int offset, RawDataPool pool)
						{
							Serializer.Deserialize(pool, offset, ref this._displayData.ProductionPool);
							this.Refresh();
						});
					}
					else
					{
						this.Refresh();
					}
				});
			}
		}

		// Token: 0x0600729E RID: 29342 RVA: 0x00353944 File Offset: 0x00351B44
		private void Awake()
		{
			this.buttonClose.ClearAndAddListener(new Action(this.QuickHide));
			this.buttonShopQuickSelect.ClearAndAddListener(new Action(this.OnClickButtonShopQuickSelect));
			this.buttonShopQuickCancel.ClearAndAddListener(new Action(this.OnClickButtonShopQuickCancel));
			this.toggleGroupSwitch.OnActiveIndexChange += this.ToggleGroupSwitchOnActiveIndexChange;
			this.craftTypeToggleGroup.OnActiveIndexChange += this.CraftTypeToggleGroupOnActiveIndexChange;
			this.buttonOrder.ClearAndAddListener(new Action(this.OnClickButtonOrder));
			this.buttonInterceptOrder.ClearAndAddListener(new Action(this.OnClickButtonOrderIntercept));
			this.buttonNegotiate.ClearAndAddListener(new Action(this.OnClickButtonNegotiate));
			this.productTypeDropdown.onValueChanged.RemoveAllListeners();
			this.productTypeDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnProductTypeDropdownValueChanged));
			this.resourceListScroll.CustomAmountDataGenerator = new Func<ITradeableContent, string>(this.AmountCellDataGenerator);
			this.resourceListScroll.Init("ViewCraftManResourceListScroll", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnItemRenderResource), new Action<ITradeableContent, RowItemLine>(this.OnItemClickResource), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount, null, null, null);
			this.buttonAddResourceFill.ClearAndAddListener(new Action(this.OnClickButtonAddResource));
			this.buttonConfirmAddResource.ClearAndAddListener(new Action(this.OnClickButtonConfirmAddResource));
			this.buttonCancelAddResource.ClearAndAddListener(new Action(this.OnClickButtonCancelAddResource));
			this.buttonAddResourceAll.ClearAndAddListener(new Action(this.OnClickButtonAddResourceAll));
			this.buttonRemoveResourceAll.ClearAndAddListener(new Action(this.OnClickButtonRemoveResourceAll));
			this.resourceToggleGroup.Init(-1);
			this.resourceToggleGroup.OnActiveIndexChange += this.ResourceToggleGroupOnActiveIndexChange;
			this.toggleShowSelectResourceList.onValueChanged.RemoveAllListeners();
			this.toggleShowSelectResourceList.onValueChanged.AddListener(new UnityAction<bool>(this.ShowSelectResourceList));
			this.toggleShowSelectResourceList.SetIsOnWithoutNotify(false);
			this.goSelectResourceArea.SetActive(false);
			this.txtSelectResourceCount.text = "0";
			this.selectResourceListScroll.Init("ViewCraftManSelectResourceListScroll", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnItemRenderSelectResource), new Action<ITradeableContent, RowItemLine>(this.OnItemClickSelectResource), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount, null, null, null);
			this.materialListScroll.CustomAmountDataGenerator = new Func<ITradeableContent, string>(this.AmountCellDataGenerator);
			this.materialListScroll.Init("ViewCraftManMaterialListScroll", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnItemRenderMaterial), new Action<ITradeableContent, RowItemLine>(this.OnItemClickMaterial), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount, null, null, null);
			this.buttonAddMaterialFill.ClearAndAddListener(new Action(this.OnClickButtonAddMaterial));
			this.buttonConfirmAddMaterial.ClearAndAddListener(new Action(this.OnClickButtonConfirmAddMaterial));
			this.buttonCancelAddMaterial.ClearAndAddListener(new Action(this.OnClickButtonCancelAddMaterial));
			this.materialToggleGroup.Init(-1);
			this.materialToggleGroup.OnActiveIndexChange += this.MaterialToggleGroupOnActiveIndexChange;
			this.buttonRemoveMaterialAll.ClearAndAddListener(new Action(this.OnClickButtonRemoveMaterialAll));
			this.buttonAddMaterialAll.ClearAndAddListener(new Action(this.OnClickButtonAddMaterialAll));
			this.toggleShowSelectMaterialList.onValueChanged.RemoveAllListeners();
			this.toggleShowSelectMaterialList.onValueChanged.AddListener(new UnityAction<bool>(this.ShowSelectMaterialList));
			this.toggleShowSelectMaterialList.SetIsOnWithoutNotify(false);
			this.goSelectMaterialArea.SetActive(false);
			this.txtSelectMaterialCount.text = "0";
			this.selectMaterialListScroll.Init("ViewCraftManSelectMaterialListScroll", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnItemRenderSelectMaterial), new Action<ITradeableContent, RowItemLine>(this.OnItemClickSelectMaterial), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount, null, null, null);
			this.InitParticle();
			this.InitStorageDropdown();
		}

		// Token: 0x0600729F RID: 29343 RVA: 0x00353D2C File Offset: 0x00351F2C
		private void OnDestroy()
		{
			this.resourceToggleGroup.OnActiveIndexChange -= this.ResourceToggleGroupOnActiveIndexChange;
			this.materialToggleGroup.OnActiveIndexChange -= this.MaterialToggleGroupOnActiveIndexChange;
			this.craftTypeToggleGroup.OnActiveIndexChange -= this.CraftTypeToggleGroupOnActiveIndexChange;
			this.toggleGroupSwitch.OnActiveIndexChange -= this.ToggleGroupSwitchOnActiveIndexChange;
		}

		// Token: 0x060072A0 RID: 29344 RVA: 0x00353D9C File Offset: 0x00351F9C
		private void Refresh()
		{
			bool isLeaderRoleMatch;
			if (this.IsBuilding)
			{
				VillagerRoleCharacterDisplayData villagerRoleCharacterDisplayData = this._displayData.VillagerRoleDataList.FirstOrDefault<VillagerRoleCharacterDisplayData>();
				isLeaderRoleMatch = (villagerRoleCharacterDisplayData != null && villagerRoleCharacterDisplayData.MatchVillagerRole);
			}
			else
			{
				isLeaderRoleMatch = true;
			}
			this._isLeaderRoleMatch = isLeaderRoleMatch;
			this.InitArtisanOrderProductionSet();
			bool isBuilding = this.IsBuilding;
			if (isBuilding)
			{
				this.RefreshBuildingPanel();
			}
			else
			{
				this.RefreshCharacterPanel();
			}
			this.RefreshProduct();
			this.RefreshStorageDropdown();
			this.RefreshSourceToggleInteractable(this.resourceToggleGroup);
			bool activeSelf = this.resourcePanel.activeSelf;
			if (activeSelf)
			{
				this.RefreshResourceList();
			}
			this.RefreshSourceToggleInteractable(this.materialToggleGroup);
			bool activeSelf2 = this.materialPanel.activeSelf;
			if (activeSelf2)
			{
				this.RefreshMaterialList();
			}
			this.CheckPutMaterial();
			this.CheckPutResource();
			this.Element.ShowAfterRefresh();
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
			{
				this.canvasGroup.alpha = 1f;
			});
		}

		// Token: 0x060072A1 RID: 29345 RVA: 0x00353E80 File Offset: 0x00352080
		public override void QuickHide()
		{
			bool isAddResource = this.IsAddResource;
			if (isAddResource)
			{
				this.CancelAddResource();
			}
			else
			{
				bool isAddMaterial = this.IsAddMaterial;
				if (isAddMaterial)
				{
					this.CancelAddMaterial();
				}
				else
				{
					bool activeSelf = this.focusPanel.gameObject.activeSelf;
					if (activeSelf)
					{
						this.ExitFocus();
						AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
					}
					else
					{
						base.QuickHide();
						TaiwuEventDomainMethod.Call.TriggerListener("FinishCraftsmanPanelUI", true);
					}
				}
			}
		}

		// Token: 0x060072A2 RID: 29346 RVA: 0x00353EFC File Offset: 0x003520FC
		private void SetProductListScroll()
		{
			this.productListScroll.CustomProductRateDataGenerator = new Func<ITradeableContent, string>(this.CustomProductRateDataGenerator);
			this.productListScroll.Init("ViewCraftManProductListScroll", ESortAndFilterControllerType.CraftsmanProduct, false, new Action<ITradeableContent, RowItemLine>(this.OnItemRenderProduct), null, ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.ProductRate, null, null, null);
			bool flag = this._blockData == null;
			if (flag)
			{
				this.productListScroll.SortAndFilterController.SetToggleVisible(EFilterLine.MainFilter.ToInt(), new List<int>
				{
					EMainFilterKeys.Food.ToInt(),
					EMainFilterKeys.Medicine.ToInt(),
					EMainFilterKeys.Equip.ToInt(),
					EMainFilterKeys.Misc.ToInt()
				}, false);
			}
			else
			{
				short templateId = this._blockData.TemplateId;
				bool flag2 = templateId == 203;
				if (flag2)
				{
					this.productListScroll.SortAndFilterController.SetToggleVisible(EFilterLine.MainFilter.ToInt(), EMainFilterKeys.Food.ToInt());
					this.productListScroll.SortAndFilterController.SetToggleIsOnWithoutNotify(EFilterLine.MainFilter.ToInt(), EMainFilterKeys.Food.ToInt());
				}
				else
				{
					bool flag3 = templateId == 169;
					if (flag3)
					{
						this.productListScroll.SortAndFilterController.SetToggleVisible(EFilterLine.MainFilter.ToInt(), new List<int>
						{
							EMainFilterKeys.Equip.ToInt(),
							EMainFilterKeys.Misc.ToInt()
						}, false);
						this.productListScroll.SortAndFilterController.SetToggleIsOnWithoutNotify(EFilterLine.MainFilter.ToInt(), EMainFilterKeys.Equip.ToInt());
					}
					else
					{
						bool flag4 = templateId == 149 || templateId == 159;
						if (flag4)
						{
							this.productListScroll.SortAndFilterController.SetToggleVisible(EFilterLine.MainFilter.ToInt(), new List<int>
							{
								EMainFilterKeys.Medicine.ToInt(),
								EMainFilterKeys.Equip.ToInt()
							}, false);
							this.productListScroll.SortAndFilterController.SetToggleIsOnWithoutNotify(EFilterLine.MainFilter.ToInt(), EMainFilterKeys.Medicine.ToInt());
						}
						else
						{
							this.productListScroll.SortAndFilterController.SetToggleVisible(EFilterLine.MainFilter.ToInt(), EMainFilterKeys.Equip.ToInt());
							this.productListScroll.SortAndFilterController.SetToggleIsOnWithoutNotify(EFilterLine.MainFilter.ToInt(), EMainFilterKeys.Equip.ToInt());
						}
					}
				}
			}
		}

		// Token: 0x060072A3 RID: 29347 RVA: 0x00354190 File Offset: 0x00352390
		private void RefreshBuildingPanel()
		{
			this.textTitle.text = this._buildingConfig.Name;
			this.RefreshManagerTitle();
			this.RefreshManagerView();
			string tipContent = string.Empty;
			int managerId = this.GetCurManagerId();
			bool flag = managerId < 0;
			if (flag)
			{
				tipContent = LanguageKey.LK_Building_Craft_Tip_NoManager.Tr().ColorReplace();
			}
			else
			{
				bool flag2 = !this._isLeaderRoleMatch;
				if (flag2)
				{
					tipContent = LanguageKey.LK_Building_Craft_Tip_ManagerRoleNotMatch.Tr().ColorReplace();
				}
			}
			this.managerLeaderView.SetWarningTip(tipContent);
			bool flag3 = this._isLeaderRoleMatch && managerId > 0;
			if (flag3)
			{
				this.effect.Play("eff_make_huawen_ui_001");
			}
			else
			{
				this.effect.Play("eff_make_huawen_ui_002");
			}
			this.RefreshSwitchBuilding();
			GlobalDomainMethod.Call.InvokeGuidingTrigger(105);
		}

		// Token: 0x060072A4 RID: 29348 RVA: 0x0035425C File Offset: 0x0035245C
		private int GetCurManagerId()
		{
			int managerIndex = 0;
			return this._displayData.ShopManagerList.GetOrDefault(managerIndex, -1);
		}

		// Token: 0x060072A5 RID: 29349 RVA: 0x00354284 File Offset: 0x00352484
		private void RefreshManagerTitle()
		{
			this._stringBuilder.Clear();
			this._stringBuilder.Append(this._buildingConfig.LeaderName).Append("(").Append(this.GetLeaderCount()).Append("/").Append(1).Append(")");
			this.textManagerLeaderTitle.text = this._stringBuilder.ToString();
			this._stringBuilder.Clear();
			this._stringBuilder.Append(this._buildingConfig.MemberName).Append("(").Append(this.GetMemberCount()).Append("/").Append(6).Append(")");
			this.textManagerMemberTitle.text = this._stringBuilder.ToString();
		}

		// Token: 0x060072A6 RID: 29350 RVA: 0x00354364 File Offset: 0x00352564
		private void RefreshManagerView()
		{
			for (int i = 0; i < 7; i++)
			{
				int charId = this._displayData.ShopManagerList[i];
				bool flag = i == 0;
				if (flag)
				{
					List<VillagerRoleCharacterDisplayData> villagerRoleDataList = this._displayData.VillagerRoleDataList;
					VillagerRoleCharacterDisplayData villagerRoleData = (villagerRoleDataList != null) ? villagerRoleDataList.GetOrDefault(i) : null;
					List<CharacterDisplayData> characterDataList = this._displayData.CharacterDataList;
					CharacterDisplayData charData = (characterDataList != null) ? characterDataList.GetOrDefault(i) : null;
					List<int> villagerEfficiencyList = this._displayData.VillagerEfficiencyList;
					int efficiency = (villagerEfficiencyList != null) ? villagerEfficiencyList.GetOrDefault(i) : 0;
					List<int> unlockedWorkingVillagerList = this._displayData.UnlockedWorkingVillagerList;
					bool isUnlocked = unlockedWorkingVillagerList != null && unlockedWorkingVillagerList.Contains(charId);
					this.managerLeaderView.Refresh(i, charId, this._blockData, villagerRoleData, charData, efficiency, true, isUnlocked, new Action<int>(this.OpenSelectChar), new Action<int>(this.CancelShopManager), new Action<int, bool>(this.SetUnlockedWorkingVillager), new Action<bool>(this.OnAssignRole));
				}
				else
				{
					int memberIndex = i - 1;
					List<VillagerRoleCharacterDisplayData> villagerRoleDataList2 = this._displayData.VillagerRoleDataList;
					VillagerRoleCharacterDisplayData villagerRoleData2 = (villagerRoleDataList2 != null) ? villagerRoleDataList2.GetOrDefault(i) : null;
					List<CharacterDisplayData> characterDataList2 = this._displayData.CharacterDataList;
					CharacterDisplayData charData2 = (characterDataList2 != null) ? characterDataList2.GetOrDefault(i) : null;
					List<ShopBuildingTeachBookData> teachBookDataList = this._displayData.TeachBookDataList;
					ShopBuildingTeachBookData teachData = (teachBookDataList != null) ? teachBookDataList.GetOrDefault(i) : null;
					List<int> villagerEfficiencyList2 = this._displayData.VillagerEfficiencyList;
					int efficiency2 = (villagerEfficiencyList2 != null) ? villagerEfficiencyList2.GetOrDefault(i) : 0;
					List<int> unlockedWorkingVillagerList2 = this._displayData.UnlockedWorkingVillagerList;
					bool isUnlocked2 = unlockedWorkingVillagerList2 != null && unlockedWorkingVillagerList2.Contains(charId);
					Dictionary<int, int> shopManagerUpgradeQualificationDict = this._displayData.ShopManagerUpgradeQualificationDict;
					int upgradeQualification = (shopManagerUpgradeQualificationDict != null) ? shopManagerUpgradeQualificationDict.GetOrDefault(charId) : 0;
					this.managerMemberViewArray[memberIndex].Refresh(i, charId, this._blockData, villagerRoleData2, charData2, teachData, upgradeQualification, efficiency2, isUnlocked2, new Action<int>(this.OpenSelectChar), new Action<int>(this.CancelShopManager), new Action<int, bool>(this.SetUnlockedWorkingVillager));
				}
			}
		}

		// Token: 0x060072A7 RID: 29351 RVA: 0x0035454D File Offset: 0x0035274D
		private void OnClickButtonShopQuickSelect()
		{
			BuildingDomainMethod.AsyncCall.QuickArrangeShopManager(null, this._blockKey, null);
			this.RequestData();
		}

		// Token: 0x060072A8 RID: 29352 RVA: 0x00354568 File Offset: 0x00352768
		private void OnClickButtonShopQuickCancel()
		{
			for (sbyte i = 0; i < 7; i += 1)
			{
				BuildingDomainMethod.Call.SetShopManager(this._blockKey, i, -1);
			}
			this.RequestData();
		}

		// Token: 0x060072A9 RID: 29353 RVA: 0x0035459C File Offset: 0x0035279C
		private int GetLeaderCount()
		{
			List<int> managerList = this._displayData.ShopManagerList;
			return (managerList[0] >= 0) ? 1 : 0;
		}

		// Token: 0x060072AA RID: 29354 RVA: 0x003545C8 File Offset: 0x003527C8
		private int GetMemberCount()
		{
			List<int> managerList = this._displayData.ShopManagerList;
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

		// Token: 0x060072AB RID: 29355 RVA: 0x00354624 File Offset: 0x00352824
		private void InitSwitchBuilding()
		{
			this.toggleGroupSwitch.Init(-1);
			this.toggleGroupSwitch.gameObject.SetActive(false);
			this.toggleGroupSwitch.OnActiveIndexChange -= this.ToggleGroupSwitchOnActiveIndexChange;
			this.toggleGroupSwitch.OnActiveIndexChange += this.ToggleGroupSwitchOnActiveIndexChange;
			this.toggleSwitch.onValueChanged.RemoveAllListeners();
			this.toggleSwitch.onValueChanged.AddListener(new UnityAction<bool>(this.ToggleSwitchOnValueChanged));
			this.toggleSwitch.SetIsOnWithoutNotify(false);
		}

		// Token: 0x060072AC RID: 29356 RVA: 0x003546BC File Offset: 0x003528BC
		private void RefreshSwitchBuilding()
		{
			List<CToggle> all = this.toggleGroupSwitch.GetAll();
			for (int index = 0; index < all.Count; index++)
			{
				CToggle toggle = all[index];
				MakeBuildingItem buildingItem = toggle.GetComponent<MakeBuildingItem>();
				buildingItem.Init(index);
				short buildingId = MakeBuildingItem.BuildingIdList[index];
				List<BuildingBlockData> blockList = this._displayData.BlockList;
				bool hasBuilding = blockList != null && blockList.Any((BuildingBlockData b) => b.TemplateId == buildingId);
				toggle.gameObject.SetActive(hasBuilding);
			}
		}

		// Token: 0x060072AD RID: 29357 RVA: 0x00354754 File Offset: 0x00352954
		private void ToggleGroupSwitchOnActiveIndexChange(int nweIndex, int oldIndex)
		{
			short buildingId = MakeBuildingItem.BuildingIdList[nweIndex];
			BuildingBlockData blockData = this._displayData.BlockList.Find((BuildingBlockData b) => b.TemplateId == buildingId);
			BuildingBlockKey blockKey = new BuildingBlockKey(this._blockKey.AreaId, this._blockKey.BlockId, blockData.BlockIndex);
			ArgumentBox args = EasyPool.Get<ArgumentBox>().Set<BuildingBlockKey>("blockKey", blockKey).Set<BuildingBlockData>("blockData", blockData);
			this.OnInit(args);
			this.ExitFocus();
		}

		// Token: 0x060072AE RID: 29358 RVA: 0x003547E4 File Offset: 0x003529E4
		private void ToggleSwitchOnValueChanged(bool isOn)
		{
			this.ShowSwitchBuilding(isOn);
		}

		// Token: 0x060072AF RID: 29359 RVA: 0x003547F0 File Offset: 0x003529F0
		private void ShowSwitchBuilding(bool isShow)
		{
			if (isShow)
			{
				this.EnterFocus(new List<GameObject>
				{
					this.rootSwitch
				}, delegate
				{
					this.toggleSwitch.SetIsOnWithoutNotify(false);
					this.toggleGroupSwitch.gameObject.SetActive(false);
				});
				int index = MakeBuildingItem.BuildingIdList.IndexOf(this._blockData.TemplateId);
				this.toggleGroupSwitch.SetWithoutNotify(index);
				this.toggleGroupSwitch.gameObject.SetActive(true);
			}
			else
			{
				this.ExitFocus();
			}
		}

		// Token: 0x060072B0 RID: 29360 RVA: 0x00354868 File Offset: 0x00352A68
		private void OpenSelectChar(int index)
		{
			this._selectingShopManagerIndex = index;
			List<int> charIdListTemp = new List<int>();
			List<int> availableWorker2 = this._displayData.AvailableWorker;
			List<int> availableWorker = (availableWorker2 != null) ? (from id in availableWorker2
			where !this._shopManagerListCached.Contains(id)
			select id).ToList<int>() : null;
			bool flag = availableWorker != null && availableWorker.Count > 0;
			if (flag)
			{
				charIdListTemp.AddRange(availableWorker);
			}
			bool flag2 = this._selectingShopManagerIndex != 0;
			if (flag2)
			{
				List<int> availableChildren2 = this._displayData.AvailableChildren;
				List<int> availableChildren = (availableChildren2 != null) ? (from id in availableChildren2
				where !this._shopManagerListCached.Contains(id)
				select id).ToList<int>() : null;
				bool flag3 = availableChildren != null && availableChildren.Count > 0;
				if (flag3)
				{
					charIdListTemp.AddRange(availableChildren);
				}
			}
			bool isTutorial = SingletonObject.getInstance<TutorialChapterModel>().InGuiding && SingletonObject.getInstance<TutorialChapterModel>().TutorialChapterIndex == 1;
			bool flag4 = isTutorial;
			if (flag4)
			{
				charIdListTemp.Clear();
				charIdListTemp.Add(SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
			}
			this.ShowSelectCharWithFilter(charIdListTemp, new SelectCharacterCallback(this.SelectShopManager));
		}

		// Token: 0x060072B1 RID: 29361 RVA: 0x00354974 File Offset: 0x00352B74
		private void ShowSelectCharWithFilter(List<int> charIdList, SelectCharacterCallback callback)
		{
			int curId = this._displayData.ShopManagerList[this._selectingShopManagerIndex];
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForGeneralScrollListBatch(this, charIdList, delegate(int offset, RawDataPool pool)
			{
				List<CharacterDisplayDataForGeneralScrollList> displayData = new List<CharacterDisplayDataForGeneralScrollList>();
				Serializer.Deserialize(pool, offset, ref displayData);
				List<ISelectCharacterData> selectList = (from item in displayData
				select new BasicSelectCharacterDataAdapter(item)).ToList<ISelectCharacterData>();
				CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.None);
				config.InteractionMode = ESelectCharacterInteractionMode.Instant;
				config.SelectionMode = ESelectCharacterSelectionMode.Single;
				CommonSelectCharacterConfig commonSelectCharacterConfig = config;
				object initialSelectedCharacterIds;
				if (curId < 0)
				{
					initialSelectedCharacterIds = null;
				}
				else
				{
					(initialSelectedCharacterIds = new List<int>()).Add(curId);
				}
				commonSelectCharacterConfig.InitialSelectedCharacterIds = initialSelectedCharacterIds;
				config.BannedCharacterIds = (from id in this._displayData.ShopManagerList
				where id >= 0
				select id).ToHashSet<int>();
				UIElement.SelectChar.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("SelectCharacterConfig", config).SetObject("SelectCharacterDataList", selectList).SetObject("SelectCharacterCallback", callback));
				UIManager.Instance.MaskUI(UIElement.SelectChar);
			});
		}

		// Token: 0x060072B2 RID: 29362 RVA: 0x003549C8 File Offset: 0x00352BC8
		private void SelectShopManager(List<int> list)
		{
			int oldId = this._displayData.ShopManagerList[this._selectingShopManagerIndex];
			int newId = (list.Count == 0) ? -1 : list.Single<int>();
			bool flag = oldId == newId;
			if (!flag)
			{
				BuildingDomainMethod.Call.SetShopManager(this._blockKey, (sbyte)this._selectingShopManagerIndex, newId);
				this.RequestData();
			}
		}

		// Token: 0x060072B3 RID: 29363 RVA: 0x00354A24 File Offset: 0x00352C24
		private void CancelShopManager(int index)
		{
			BuildingDomainMethod.Call.SetShopManager(this._blockKey, (sbyte)index, -1);
			this.RequestData();
		}

		// Token: 0x060072B4 RID: 29364 RVA: 0x00354A3D File Offset: 0x00352C3D
		private void SetUnlockedWorkingVillager(int charId, bool isUnlock)
		{
			BuildingDomainMethod.Call.SetUnlockedWorkingVillagers(charId, isUnlock);
			this.RequestData();
		}

		// Token: 0x060072B5 RID: 29365 RVA: 0x00354A4F File Offset: 0x00352C4F
		private void OnAssignRole(bool _)
		{
			this.RequestData();
			GEvent.OnEvent(UiEvents.OnSetVillagerRole, null);
		}

		// Token: 0x060072B6 RID: 29366 RVA: 0x00354A6C File Offset: 0x00352C6C
		private void RefreshCharacterPanel()
		{
			this.RefreshCharacterData();
			this.RefreshCraftTypeToggleGroup();
			this.textTitle.text = ViewCraftsman.GetCraftsmanTitleByCraftType(this._craftsmanType);
			this.textArtisan.text = ViewCraftsman.GetLeaderNameByCraftType(this._currentCraftType);
			sbyte lifeSkillType = this.LiftSkillType;
			short attainment = this._displayData.ArtisanLifeSkillAttainments.Get((int)lifeSkillType);
			short qualifications = this._displayData.ArtisanLifeSkillQualifications.Get((int)lifeSkillType);
			int delta = this.GetProgressPercent(this._displayData.ArtisanOrderProgressDeltas.Get((int)lifeSkillType));
			this.artisanView.Refresh(this._displayData.ArtisanCharData, lifeSkillType, (int)attainment, (int)qualifications, delta);
			ArtisanOrder artisanOrder = this._displayData.ArtisanOrder;
			bool hasSubscriber = artisanOrder != null && artisanOrder.ArtisanId > 0;
			this.subscriberView.gameObject.SetActive(hasSubscriber);
			this.objectNoneSubscriber.gameObject.SetActive(!hasSubscriber);
			bool flag = hasSubscriber;
			if (flag)
			{
				this.subscriberView.Refresh(this._displayData.SubscriberCharData);
			}
			ArtisanOrder artisanOrder2 = this._displayData.ArtisanOrder;
			int debateCount = (artisanOrder2 != null) ? artisanOrder2.DebateCount : 0;
			bool isIntercept = this._panelMode.HasFlag(ViewCraftsman.EPanelMode.Intercept);
			bool isNoOrder = this._panelMode.HasFlag(ViewCraftsman.EPanelMode.NoOrder);
			bool isTaiwuOrder = this._panelMode.HasFlag(ViewCraftsman.EPanelMode.TaiwuOrdered);
			this.buttonOrder.gameObject.SetActive(!isIntercept);
			this.buttonInterceptOrder.gameObject.SetActive(isIntercept);
			ArtisanOrder artisanOrder3 = this._displayData.ArtisanOrder;
			sbyte orderLifeSkillType = (artisanOrder3 != null) ? artisanOrder3.LifeSkillType : -1;
			string lifeSkillTypeName = (orderLifeSkillType >= 0) ? Config.LifeSkillType.Instance[orderLifeSkillType].Name : string.Empty;
			bool isSameResourceType = this.LiftSkillType == orderLifeSkillType || orderLifeSkillType < 0;
			int price = this._displayData.GetFinalPrice(isIntercept);
			bool isTypeMeet = isSameResourceType && price > 0;
			int money = SingletonObject.getInstance<BuildingModel>().GetResourceCount(6);
			bool isResourceMeet = money >= price;
			string curStr = CommonUtils.GetDisplayStringForNum(money, 100000).SetColor(isResourceMeet ? "brightblue" : "brightred");
			string costStr = CommonUtils.GetDisplayStringForNum(price, 100000);
			this.textOrderCost.text = curStr + "/" + costStr;
			this.buttonOrder.interactable = (isTypeMeet && isResourceMeet && isNoOrder);
			this.buttonInterceptOrder.interactable = (isTypeMeet && isResourceMeet && isIntercept);
			TooltipInvoker tipButtonOrder = this.buttonOrder.gameObject.GetOrAddComponent<TooltipInvoker>();
			tipButtonOrder.enabled = !this.buttonOrder.interactable;
			tipButtonOrder.Type = TipType.SingleDesc;
			bool enabled = tipButtonOrder.enabled;
			if (enabled)
			{
				LanguageKey key = (!isNoOrder) ? LanguageKey.LK_Craftsman_Confirm_Ordered : ((!isTypeMeet) ? LanguageKey.LK_Craftsman_Confirm_Not_Enough_Attainment : LanguageKey.LK_Craftsman_Confirm_Not_Enough_Money);
				tipButtonOrder.PresetParam = new string[]
				{
					key.Tr().ColorReplace()
				};
			}
			string content = isNoOrder ? LanguageKey.LK_Craftsman_OrderProduct_Tip.Tr().ColorReplace() : (isIntercept ? LanguageKey.LK_Craftsman_InterceptOrder_Tip.Tr() : ((!isTaiwuOrder) ? LanguageKey.LK_Craftsman_Ordered_Tip.TrFormat(lifeSkillTypeName) : string.Empty));
			this.textOrderState.text = content.ColorReplace();
			this.textOrderState.gameObject.SetActive(!content.IsNullOrEmpty());
			this.effect.Play("eff_make_huawen_ui_001");
			LifeSkillTypeItem skillConfig = Config.LifeSkillType.Instance[this.LiftSkillType];
			this.textNegotiateNeed.text = LanguageKey.LK_Craftsman_NegotiateNeed.TrFormat(skillConfig.Icon, skillConfig.Name);
			this.helperNegotiateNeed.Parse();
			this.buttonNegotiate.gameObject.SetActive(isIntercept && debateCount == 0 && isSameResourceType);
			GlobalDomainMethod.Call.InvokeGuidingTrigger(76);
		}

		// Token: 0x060072B7 RID: 29367 RVA: 0x00354E5C File Offset: 0x0035305C
		private void InitCraftTypeToggleGroup()
		{
			CToggleGroup toggleGroup = this.craftTypeToggleGroup;
			toggleGroup.Clear();
			CommonUtils.PrepareEnoughChildren(toggleGroup.transform, toggleGroup.transform.GetChild(0).gameObject, this._craftTypes.Count, null);
			for (int i = 0; i < this._craftTypes.Count; i++)
			{
				Transform temp = toggleGroup.transform.GetChild(i);
				temp.gameObject.SetActive(true);
				temp.GetComponentInChildren<TextMeshProUGUI>().text = ViewCraftsman.GetCraftTypesNameByEnum(this._craftTypes[i]);
				CToggle tog = temp.GetComponent<CToggle>();
				toggleGroup.Add(tog);
			}
			toggleGroup.Init(-1);
		}

		// Token: 0x060072B8 RID: 29368 RVA: 0x00354F18 File Offset: 0x00353118
		private void RefreshCraftTypeToggleGroup()
		{
			CToggleGroup toggleGroup = this.craftTypeToggleGroup;
			CraftManDisplayData displayData = this._displayData;
			ArtisanOrder artisanOrder = (displayData != null) ? displayData.ArtisanOrder : null;
			bool flag = artisanOrder != null && artisanOrder.SubscriberId > 0 && artisanOrder.LifeSkillType >= 0;
			if (flag)
			{
				for (int i = 0; i < this._craftTypes.Count; i++)
				{
					ViewCraftsman.ECraftType craftType = this._craftTypes[i];
					short itemSubType = this.GetItemSubType(craftType);
					CToggle tog = toggleGroup.Get(i);
					sbyte needSkillType = ViewCraftsman.GetOperationNeedSkillType(craftType);
					int num = (int)needSkillType;
					CraftManDisplayData displayData2 = this._displayData;
					sbyte? b = (displayData2 != null) ? new sbyte?(displayData2.ArtisanOrder.LifeSkillType) : null;
					int? num2 = (b != null) ? new int?((int)b.GetValueOrDefault()) : null;
					bool isSkillMeet = num == num2.GetValueOrDefault() & num2 != null;
					bool flag2 = isSkillMeet;
					if (flag2)
					{
						Selectable selectable = tog;
						CraftManDisplayData displayData3 = this._displayData;
						short? num3 = (displayData3 != null) ? new short?(displayData3.ArtisanOrder.ItemSubType) : null;
						num2 = ((num3 != null) ? new int?((int)num3.GetValueOrDefault()) : null);
						int num4 = (int)itemSubType;
						selectable.interactable = (num2.GetValueOrDefault() == num4 & num2 != null);
						bool interactable = tog.interactable;
						if (interactable)
						{
							this._currentCraftType = craftType;
						}
					}
					else
					{
						tog.interactable = false;
					}
				}
			}
			else
			{
				for (int j = 0; j < this._craftTypes.Count; j++)
				{
					CToggle tog2 = toggleGroup.Get(j);
					tog2.interactable = true;
				}
			}
			toggleGroup.SetWithoutNotify(this._craftTypes.IndexOf(this._currentCraftType));
		}

		// Token: 0x060072B9 RID: 29369 RVA: 0x003550FA File Offset: 0x003532FA
		private void CraftTypeToggleGroupOnActiveIndexChange(int newIndex, int oldIndex)
		{
			this._currentCraftType = this._craftTypes[newIndex];
			ExtraDomainMethod.AsyncCall.GetProductionPoolPreview(this, this._artisanId, this.LiftSkillType, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._displayData.ProductionPool);
				this.Refresh();
			});
		}

		// Token: 0x060072BA RID: 29370 RVA: 0x00355130 File Offset: 0x00353330
		private void RefreshCharacterData()
		{
			bool flag = !this._panelMode.HasFlag(ViewCraftsman.EPanelMode.Character);
			if (!flag)
			{
				this._panelMode = ViewCraftsman.EPanelMode.Character;
				ArtisanOrder artisanOrder = this._displayData.ArtisanOrder;
				int subscriberId = (artisanOrder != null) ? artisanOrder.SubscriberId : -1;
				bool flag2 = subscriberId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				if (flag2)
				{
					this._panelMode |= ViewCraftsman.EPanelMode.TaiwuOrdered;
				}
				else
				{
					bool flag3 = subscriberId > 0;
					if (flag3)
					{
						this._panelMode |= ViewCraftsman.EPanelMode.Intercept;
					}
					else
					{
						this._panelMode |= ViewCraftsman.EPanelMode.NoOrder;
					}
				}
			}
		}

		// Token: 0x060072BB RID: 29371 RVA: 0x003551CC File Offset: 0x003533CC
		private void InitCraftTypeList()
		{
			this._craftTypes.Clear();
			bool flag = this._craftsmanType == 63;
			if (flag)
			{
				this._craftTypes.Add(ViewCraftsman.ECraftType.Forging);
				this._craftTypes.Add(ViewCraftsman.ECraftType.Woodworking);
				this._craftTypes.Add(ViewCraftsman.ECraftType.Weaving);
				this._craftTypes.Add(ViewCraftsman.ECraftType.Jade);
			}
			else
			{
				bool flag2 = this._craftsmanType == 64;
				if (flag2)
				{
					this._craftTypes.Add(ViewCraftsman.ECraftType.Medicine);
					this._craftTypes.Add(ViewCraftsman.ECraftType.Toxicology);
				}
				else
				{
					bool flag3 = this._craftsmanType == 65;
					if (flag3)
					{
						this._craftTypes.Add(ViewCraftsman.ECraftType.Cooking);
					}
					else
					{
						bool flag4 = this._craftsmanType == 66;
						if (!flag4)
						{
							throw new NotImplementedException(string.Format("CraftsmanType {0} is not implemented.", this._craftsmanType));
						}
						this._craftTypes.Add(ViewCraftsman.ECraftType.Tea);
						this._craftTypes.Add(ViewCraftsman.ECraftType.Wine);
					}
				}
			}
			this._currentCraftType = this._craftTypes[0];
		}

		// Token: 0x060072BC RID: 29372 RVA: 0x003552D5 File Offset: 0x003534D5
		private void OnClickButtonNegotiate()
		{
			this._startLifeSkillCombat = true;
			SingletonObject.getInstance<LifeSkillCombatModel>().StartLifeSkillCombat(this._displayData.ArtisanOrder.ArtisanId, this.LiftSkillType, false);
		}

		// Token: 0x060072BD RID: 29373 RVA: 0x00355304 File Offset: 0x00353504
		private void OnClickButtonOrder()
		{
			int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			ExtraDomainMethod.Call.CreateArtisanOrder(this._artisanId, taiwuCharId, this.LiftSkillType, this.CurItemSubType);
			this.RequestData();
		}

		// Token: 0x060072BE RID: 29374 RVA: 0x00355340 File Offset: 0x00353540
		private void OnClickButtonOrderIntercept()
		{
			int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			ExtraDomainMethod.Call.InterceptArtisanOrder(this._displayData.ArtisanOrder.ArtisanId, taiwuCharId, this._displayData.ArtisanOrder.IsDebateWon);
			this.RequestData();
		}

		// Token: 0x060072BF RID: 29375 RVA: 0x00355388 File Offset: 0x00353588
		private void OnLifeSkillCombatEnd(bool isTaiwuWin)
		{
			bool flag = !this._startLifeSkillCombat;
			if (!flag)
			{
				this._startLifeSkillCombat = false;
				this._displayData.ArtisanOrder.IsDebateWon = isTaiwuWin;
				ExtraDomainMethod.Call.ArtisanOrderDebate(this._displayData.ArtisanOrder.ArtisanId, isTaiwuWin);
				this.RequestData();
			}
		}

		// Token: 0x060072C0 RID: 29376 RVA: 0x003553DC File Offset: 0x003535DC
		private void RefreshProduct()
		{
			short itemSubType = this.CurItemSubType;
			string icon;
			string text;
			ViewCraftsman.GetMakeItemTypeInfo(itemSubType, out icon, out text);
			this.imageProductType.SetSprite(icon, false, null);
			ArtisanOrder artisanOrder = this._displayData.ArtisanOrder;
			int curValue = (artisanOrder != null) ? artisanOrder.Progress : 0;
			int showCurValue = this.GetProgressPercent(curValue);
			this.imageProductProgress.fillAmount = (float)showCurValue / 100f;
			this.imageProductProgressChange.fillAmount = 0f;
			int maxValue = GameData.Domains.Building.SharedMethods.MaxProductionProgress(true);
			this._needProgress = maxValue - curValue;
			ArtisanOrder artisanOrder2 = this._displayData.ArtisanOrder;
			int addValue = (artisanOrder2 != null) ? artisanOrder2.ProgressDelta : 0;
			ArtisanOrder artisanOrder3 = this._displayData.ArtisanOrder;
			int addBaseValue = (artisanOrder3 != null) ? artisanOrder3.ProgressBaseDelta : 0;
			int showAddValue = this.GetProgressPercent(addValue);
			int showAddBaseValue = this.GetProgressPercent(addBaseValue);
			string curStr = string.Format("{0}%", showCurValue);
			string addStr = string.Format("{0}%", showAddValue).SetColor("brightblue");
			this.textProductProgress.text = LanguageKey.LK_Craftsman_CraftProgress_Content.TrFormat(curStr, addStr);
			ArtisanOrder artisanOrder4 = this._displayData.ArtisanOrder;
			int charId = (artisanOrder4 != null) ? artisanOrder4.ArtisanId : 0;
			bool interactable = charId > 0 && this._isLeaderRoleMatch;
			this.backProductEmpty.SetActive(!interactable);
			this.backProductFill.SetActive(interactable);
			this.buttonAddMaterialEmpty.gameObject.SetActive(!interactable);
			this.buttonAddMaterialFill.gameObject.SetActive(interactable);
			this.buttonAddResourceEmpty.gameObject.SetActive(!interactable);
			this.buttonAddResourceFill.gameObject.SetActive(interactable);
			short curItemSubType = this.CurItemSubType;
			bool isTeaWine = curItemSubType == 900 || curItemSubType == 901;
			this.productTypeDropdown.interactable = (interactable && !isTeaWine);
			this.storageDropdown.interactable = interactable;
			this.RefreshTipProductProgress(showAddValue, showAddBaseValue);
			this.RefreshDropdownProductType();
			this.RefreshProductPool();
		}

		// Token: 0x060072C1 RID: 29377 RVA: 0x003555F0 File Offset: 0x003537F0
		private void RefreshTipProductProgress(int showAddValue, int showAddBaseValue)
		{
			TooltipInvoker mouseTip = this.tipProductProgress;
			mouseTip.Type = TipType.GeneralLines;
			int extraValue = showAddValue - showAddBaseValue;
			GeneralLineData desc = new GeneralLineData
			{
				Type = 3,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_VillagerCraftPreviewPanel_Tip_Normal)
				}
			};
			GeneralLineData title = new GeneralLineData
			{
				Type = 3,
				Args = new List<string>
				{
					LocalStringManager.GetFormat(LanguageKey.LK_Brackets_Symbol, LocalStringManager.Get(LanguageKey.LK_Craftsman_CraftProgress))
				}
			};
			GeneralLineData contentNormal = new GeneralLineData
			{
				Type = 5,
				Args = new List<string>
				{
					LocalStringManager.GetFormat(LanguageKey.LK_Craftsman_CraftProgress_Tip, showAddBaseValue.ToString() + "%")
				},
				ExtraArgs = new List<object>
				{
					20
				}
			};
			GeneralLineData contentExtra = new GeneralLineData
			{
				Type = 5,
				Args = new List<string>
				{
					LocalStringManager.GetFormat(LanguageKey.LK_Craftsman_CraftProgress_ExtraTip, extraValue.ToString() + "%")
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
			mouseTip.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_Craftsman_CraftProgress)).SetObject("LineData1", desc).SetObject("LineData2", title).SetObject("LineData3", contentNormal);
			bool flag = extraValue > 0;
			if (flag)
			{
				lineCount++;
				mouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", lineCount), contentExtra);
			}
			mouseTip.RuntimeParam.Set("LineCount", lineCount);
		}

		// Token: 0x060072C2 RID: 29378 RVA: 0x003557B8 File Offset: 0x003539B8
		private void RefreshDropdownProductType()
		{
			short itemSubType = this.CurItemSubType;
			int index = this._displayData.CanProduceItemSubType.IndexOf(itemSubType);
			List<string> optionList = this._displayData.CanProduceItemSubType.Select(delegate(short subType)
			{
				string text;
				string typeName;
				ViewCraftsman.GetMakeItemTypeInfo(subType, out text, out typeName);
				return typeName;
			}).ToList<string>();
			this.productTypeDropdown.ClearOptions();
			this.productTypeDropdown.AddOptions(optionList);
			this.productTypeDropdown.SetValueWithoutNotify(index);
		}

		// Token: 0x060072C3 RID: 29379 RVA: 0x0035583C File Offset: 0x00353A3C
		private void OnProductTypeDropdownValueChanged(int value)
		{
			short subType = this._displayData.CanProduceItemSubType[value];
			ExtraDomainMethod.Call.SetArtisanOrderProductionType(this._displayData.ArtisanOrder, subType);
			this.RequestData();
		}

		// Token: 0x060072C4 RID: 29380 RVA: 0x00355878 File Offset: 0x00353A78
		private void RefreshProductPool()
		{
			short itemSubType = this.CurItemSubType;
			this._displayData.ProductionPool = this._displayData.ProductionPool;
			this._totalWeight = 0;
			this._previewTotalWeight = 0;
			this._productItemList.Clear();
			bool flag = this._displayData.ProductionPool != null;
			if (flag)
			{
				foreach (KeyValuePair<Production, ProductionData> keyValuePair in this._displayData.ProductionPool.Productions)
				{
					Production production3;
					ProductionData productionData3;
					keyValuePair.Deconstruct(out production3, out productionData3);
					Production production = production3;
					ProductionData productionData = productionData3;
					short subType = ItemTemplateHelper.GetItemSubType(production.ItemType, production.TemplateId);
					bool flag2 = itemSubType >= 0 && itemSubType != subType;
					if (!flag2)
					{
						bool canProduce = productionData.CanProduce;
						if (canProduce)
						{
							this._totalWeight += productionData.Weight;
						}
						ItemDisplayData itemDisplayData = new ItemDisplayData(production.ItemType, production.TemplateId);
						ProductionPool previewProductionPool = this._previewProductionPool;
						itemDisplayData.ProductionData = ((previewProductionPool != null) ? previewProductionPool.Productions[production] : this._displayData.ProductionPool.Productions[production]);
						ItemDisplayData itemData = itemDisplayData;
						this._productItemList.Add(itemData);
					}
				}
			}
			bool flag3 = this._previewProductionPool != null;
			if (flag3)
			{
				foreach (KeyValuePair<Production, ProductionData> keyValuePair in this._previewProductionPool.Productions)
				{
					Production production3;
					ProductionData productionData3;
					keyValuePair.Deconstruct(out production3, out productionData3);
					Production production2 = production3;
					ProductionData productionData2 = productionData3;
					short subType2 = ItemTemplateHelper.GetItemSubType(production2.ItemType, production2.TemplateId);
					bool flag4 = itemSubType >= 0 && itemSubType != subType2;
					if (!flag4)
					{
						bool canProduce2 = productionData2.CanProduce;
						if (canProduce2)
						{
							this._previewTotalWeight += productionData2.Weight;
						}
					}
				}
			}
			this.productListScroll.SetItemList(this._productItemList);
		}

		// Token: 0x060072C5 RID: 29381 RVA: 0x00355AB0 File Offset: 0x00353CB0
		private string CustomProductRateDataGenerator(ITradeableContent content)
		{
			ItemDisplayData itemData = content as ItemDisplayData;
			int previewChance;
			bool flag;
			bool isUp;
			bool isDown;
			int chance = this.GetChance(itemData, out previewChance, out flag, out isUp, out isDown);
			bool flag2 = previewChance != chance;
			string chanceText;
			if (flag2)
			{
				string previewChanceColor = isUp ? "brightblue" : (isDown ? "brightred" : "pinkyellow");
				int delta = previewChance - chance;
				chanceText = string.Format("{0}% ", chance) + string.Format("{0:+0;-0;0}%", delta).SetColor(previewChanceColor);
			}
			else
			{
				chanceText = string.Format("{0}%", chance);
			}
			return chanceText;
		}

		// Token: 0x060072C6 RID: 29382 RVA: 0x00355B54 File Offset: 0x00353D54
		private int GetChance(ItemDisplayData itemData, out int previewChance, out bool canProduce, out bool isUp, out bool isDown)
		{
			Production production = new Production(itemData.RealKey.ItemType, itemData.RealKey.TemplateId);
			ProductionData productionData = this._displayData.ProductionPool.Productions[production];
			int weight = productionData.Weight;
			int totalWeight = this._totalWeight;
			canProduce = productionData.CanProduce;
			float realChance;
			int chance = this.CalcChance(weight, totalWeight, out realChance);
			bool flag = this._previewProductionPool != null;
			if (flag)
			{
				ProductionData previewProductionData = this._previewProductionPool.Productions[production];
				int previewWeight = previewProductionData.Weight;
				float previewRealChance;
				previewChance = this.CalcChance(previewWeight, this._previewTotalWeight, out previewRealChance);
				isUp = (previewRealChance > realChance);
				isDown = (previewRealChance < realChance);
			}
			else
			{
				previewChance = chance;
				isUp = false;
				isDown = false;
			}
			return chance;
		}

		// Token: 0x060072C7 RID: 29383 RVA: 0x00355C24 File Offset: 0x00353E24
		private void OnItemRenderProduct(ITradeableContent content, RowItemLine rowItemLine)
		{
			ItemDisplayData itemData = content as ItemDisplayData;
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(content);
			rowItemLine.Set(rowItemMain, true);
			int previewChance;
			bool canProduce;
			bool flag;
			bool flag2;
			this.GetChance(itemData, out previewChance, out canProduce, out flag, out flag2);
			bool notMeet = previewChance == 0 || !canProduce;
			bool flag3 = notMeet;
			if (flag3)
			{
				rowItemMain.SetItemNotCanSelectReason(LocalStringManager.Get(LanguageKey.LK_VillagerCraftPreviewPanel_CannotProduce).SetColor("brightred"));
			}
			else
			{
				rowItemMain.HideInteractionState();
			}
			rowItemLine.SetDisabled(notMeet);
			rowItemLine.SetInteractable(false, true);
		}

		// Token: 0x060072C8 RID: 29384 RVA: 0x00355CB0 File Offset: 0x00353EB0
		private int CalcChance(int weight, int totalWeight, out float realChance)
		{
			realChance = ((totalWeight == 0) ? 0f : ((float)weight / (float)totalWeight));
			int chance = (totalWeight == 0) ? 0 : (weight * 100 / totalWeight);
			float num = realChance;
			bool flag = num > 0f && num < 0.01f;
			if (flag)
			{
				chance = 1;
			}
			return chance;
		}

		// Token: 0x060072C9 RID: 29385 RVA: 0x00355D00 File Offset: 0x00353F00
		private int GetProgressPercent(int value)
		{
			return 100 * value / GameData.Domains.Building.SharedMethods.MaxProductionProgress(true);
		}

		// Token: 0x060072CA RID: 29386 RVA: 0x00355D20 File Offset: 0x00353F20
		public static void GetMakeItemTypeInfo(short itemSubType, out string icon, out string name)
		{
			MakeItemTypeItem config = (from m in MakeItemType.Instance
			where !m.TypeBigIcon.IsNullOrEmpty()
			select m).FirstOrDefault((MakeItemTypeItem m) => m.ItemSubType == itemSubType);
			name = (((config != null) ? config.TypeName : null) ?? LocalStringManager.Get(LanguageKey.LK_Common_All));
			icon = (((config != null) ? config.TypeBigIcon : null) ?? "ui9_icon_make_item_type_all");
		}

		// Token: 0x060072CB RID: 29387 RVA: 0x00355DAC File Offset: 0x00353FAC
		private void OnClickButtonAddResource()
		{
			bool isAddResource = this.IsAddResource;
			if (isAddResource)
			{
				this.CancelAddResource();
			}
			else
			{
				this.EnterAddResource();
			}
		}

		// Token: 0x060072CC RID: 29388 RVA: 0x00355DD4 File Offset: 0x00353FD4
		private void OnClickButtonCancelAddResource()
		{
			this.CancelAddResource();
		}

		// Token: 0x060072CD RID: 29389 RVA: 0x00355DE0 File Offset: 0x00353FE0
		private void OnClickButtonConfirmAddResource()
		{
			List<ItemDisplayData> itemList = (from p in this._selectedItemDict
			select p.Key.Clone(p.Value)).ToList<ItemDisplayData>();
			ExtraDomainMethod.Call.AddResourceItemToArtisanOrder(this._displayData.ArtisanOrder, itemList);
			this._selectedItemDict.Clear();
			this.txtSelectResourceCount.text = "0";
			this.ShowAddResourceParticle();
			this.RequestData();
		}

		// Token: 0x060072CE RID: 29390 RVA: 0x00355E5C File Offset: 0x0035405C
		private void EnterAddResource()
		{
			this.EnterFocus(new List<GameObject>
			{
				this.resourcePanel,
				this.buttonAddResourceFill.gameObject,
				this.imageProductType.transform.parent.gameObject,
				this.rootProductProgress
			}, null);
			this.RefreshResourceList();
			this.RefreshSelectResourceList();
			this.toggleShowSelectResourceList.SetIsOnWithoutNotify(false);
			this.goSelectResourceArea.SetActive(false);
			this.resourcePanel.gameObject.SetActive(true);
		}

		// Token: 0x060072CF RID: 29391 RVA: 0x00355EFC File Offset: 0x003540FC
		private void CancelAddResource()
		{
			this.resourcePanel.gameObject.SetActive(false);
			this.ExitFocus();
			this._selectedItemDict.Clear();
			this.txtSelectResourceCount.text = "0";
			this.CheckPutResource();
			AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
		}

		// Token: 0x17000D11 RID: 3345
		// (get) Token: 0x060072D0 RID: 29392 RVA: 0x00355F59 File Offset: 0x00354159
		private bool IsAddResource
		{
			get
			{
				return this.resourcePanel.activeSelf;
			}
		}

		// Token: 0x060072D1 RID: 29393 RVA: 0x00355F68 File Offset: 0x00354168
		private void RefreshResourceList()
		{
			this._resourceItemList.Clear();
			List<ItemDisplayData> itemList = this.GetItemList(this.CurResourceItemSource);
			sbyte resourceType = ViewCraftsman.GetResourceTypeByCraftType(this._currentCraftType);
			IEnumerable<ItemDisplayData> availableItemList = (itemList != null) ? itemList.Where(delegate(ItemDisplayData t)
			{
				bool isResource = t.IsResource;
				bool result;
				if (isResource)
				{
					result = (t.ResourceType == resourceType);
				}
				else
				{
					bool flag2 = ItemType.IsEquipmentItemType(t.RealKey.ItemType);
					result = (flag2 && ItemTemplateHelper.GetResourceType(t.RealKey.ItemType, t.RealKey.TemplateId) == resourceType);
				}
				return result;
			}) : null;
			bool flag = availableItemList != null;
			if (flag)
			{
				this._resourceItemList.AddRange(availableItemList);
			}
			this.resourceListScroll.SetItemList(this._resourceItemList);
		}

		// Token: 0x060072D2 RID: 29394 RVA: 0x00355FE8 File Offset: 0x003541E8
		private void OnItemClickResource(ITradeableContent content, RowItemLine rowItemLine)
		{
			ItemDisplayData itemData = content as ItemDisplayData;
			bool flag = this._selectedItemDict.Remove(itemData);
			if (flag)
			{
				this.CheckPutResource();
				this.txtSelectResourceCount.text = string.Format("{0}", this._selectedItemDict.Count);
				this.RefreshSelectResourceList();
			}
			else
			{
				bool flag2 = content.Amount == 1;
				if (flag2)
				{
					this.SetItemSelectCount(content, 1);
					this.CheckPutResource();
				}
				else
				{
					int remainProgress = this.RemainProgress;
					int limitCount = (remainProgress <= 0) ? 0 : Math.Max(1, remainProgress / this.GetItemProgress(itemData));
					this.resourceListScroll.SetItemToSelectCountMode(rowItemLine, delegate(int count)
					{
						this.SetItemSelectCount(content, count);
						this.CheckPutResource();
					}, delegate
					{
						bool flag3 = this._selectedItemDict.Remove(itemData);
						if (flag3)
						{
							this.CheckPutResource();
						}
					}, 0, limitCount, 1, null, false, null, false);
				}
			}
		}

		// Token: 0x060072D3 RID: 29395 RVA: 0x003560E4 File Offset: 0x003542E4
		private void OnItemRenderResource(ITradeableContent content, RowItemLine rowItemLine)
		{
			ItemDisplayData itemData = content as ItemDisplayData;
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			bool isSelected = this._selectedItemDict.ContainsKey(itemData);
			rowItemLine.SetSelected(isSelected);
			int remainProgress = this.RemainProgress;
			int itemProgress = this.GetItemProgress(itemData);
			int limitCount = (remainProgress <= 0 || itemProgress <= 0) ? 0 : Math.Max(1, remainProgress / itemProgress);
			bool interactable = limitCount > 0 || isSelected;
			rowItemLine.SetInteractable(interactable, true);
			rowItemLine.SetDisabled(!interactable);
		}

		// Token: 0x060072D4 RID: 29396 RVA: 0x00356170 File Offset: 0x00354370
		private void OnItemRenderSelectResource(ITradeableContent content, RowItemLine rowItemLine)
		{
			ItemDisplayData itemData = content as ItemDisplayData;
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			rowItemLine.SetSelected(false);
		}

		// Token: 0x060072D5 RID: 29397 RVA: 0x003561A5 File Offset: 0x003543A5
		private void OnItemClickSelectResource(ITradeableContent content, RowItemLine rowItemLine)
		{
			this.SetItemSelectCount(content, 0);
			this.CheckPutResource();
		}

		// Token: 0x060072D6 RID: 29398 RVA: 0x003561B8 File Offset: 0x003543B8
		private void RefreshSelectResourceList()
		{
			List<ItemDisplayData> list = new List<ItemDisplayData>();
			foreach (KeyValuePair<ItemDisplayData, int> info in this._selectedItemDict)
			{
				list.Add(info.Key);
			}
			this.selectResourceListScroll.SetItemList(list);
			this.txtSelectResourceCount.text = string.Format("{0}", list.Count);
		}

		// Token: 0x060072D7 RID: 29399 RVA: 0x0035624C File Offset: 0x0035444C
		private int GetItemProgress(ItemDisplayData itemData)
		{
			return GameData.Domains.Building.SharedMethods.GetItemProductionProgress(itemData);
		}

		// Token: 0x060072D8 RID: 29400 RVA: 0x00356254 File Offset: 0x00354454
		private void OnClickButtonAddResourceAll()
		{
			int remainProgress = this.RemainProgress;
			IReadOnlyList<ITradeableContent> candidates = this.resourceListScroll.FilteredData;
			bool needSelect = false;
			bool flag = remainProgress > 0;
			if (flag)
			{
				foreach (ITradeableContent candidate in candidates)
				{
					int count;
					bool flag2 = !this._selectedItemDict.TryGetValue(candidate as ItemDisplayData, out count) || count < candidate.Amount;
					if (flag2)
					{
						needSelect = true;
						break;
					}
				}
			}
			bool flag3 = !needSelect;
			if (flag3)
			{
				this.CheckPutResource();
			}
			else
			{
				this._selectedItemDict.Clear();
				this.txtSelectResourceCount.text = "0";
				int needProgress = this._needProgress;
				for (int i = 0; i < candidates.Count; i++)
				{
					ItemDisplayData itemData = candidates[i] as ItemDisplayData;
					int itemProgress = this.GetItemProgress(itemData);
					int tempAmount = itemData.Amount;
					while (tempAmount > 0 && needProgress > 0)
					{
						tempAmount--;
						needProgress -= itemProgress;
					}
					bool flag4 = needProgress < 0 && this.TrySetCandidateSmaller(candidates, itemProgress + needProgress, i + 1, itemData.Grade);
					if (flag4)
					{
						tempAmount++;
					}
					this.SetItemSelectCount(itemData, itemData.Amount - tempAmount);
					bool flag5 = needProgress <= 0;
					if (flag5)
					{
						break;
					}
				}
				this.CheckPutResource();
			}
		}

		// Token: 0x060072D9 RID: 29401 RVA: 0x003563E8 File Offset: 0x003545E8
		private void OnClickButtonRemoveResourceAll()
		{
			this._selectedItemDict.Clear();
			this.CheckPutResource();
			this.RefreshResourceList();
			this.RefreshSelectResourceList();
			this.txtSelectResourceCount.text = "0";
		}

		// Token: 0x060072DA RID: 29402 RVA: 0x0035641D File Offset: 0x0035461D
		private void ShowSelectResourceList(bool isOn)
		{
			this.goSelectResourceArea.SetActive(isOn);
			this.RefreshSelectResourceList();
		}

		// Token: 0x060072DB RID: 29403 RVA: 0x00356434 File Offset: 0x00354634
		private bool TrySetCandidateSmaller(IReadOnlyList<ITradeableContent> candidates, int targetValue, int startIndex, sbyte grade)
		{
			int currentValue = int.MaxValue;
			int targetIndex = -1;
			int currentGrade = (int)grade;
			for (int i = startIndex; i < candidates.Count; i++)
			{
				ItemDisplayData itemData = candidates[i] as ItemDisplayData;
				int itemProgress = this.GetItemProgress(itemData);
				bool flag = itemProgress >= targetValue && itemProgress <= currentValue && (int)itemData.Grade < currentGrade;
				if (flag)
				{
					targetIndex = i;
					currentGrade = (int)itemData.Grade;
					currentValue = itemProgress;
				}
			}
			bool flag2 = targetIndex >= 0;
			if (flag2)
			{
				this.SetItemSelectCount(candidates[targetIndex], 1);
			}
			return targetIndex >= 0;
		}

		// Token: 0x060072DC RID: 29404 RVA: 0x003564D4 File Offset: 0x003546D4
		private void CheckPutResource()
		{
			ArtisanOrder artisanOrder = this._displayData.ArtisanOrder;
			int curValue = (artisanOrder != null) ? artisanOrder.Progress : 0;
			int showCurValue = this.GetProgressPercent(curValue + this.AddProgress);
			this.buttonConfirmAddResource.interactable = (this._selectedItemDict.Count > 0);
			this.imageProductProgressChange.fillAmount = (float)showCurValue / 100f;
			bool activeInHierarchy = this.resourceListScroll.gameObject.activeInHierarchy;
			if (activeInHierarchy)
			{
				this.resourceListScroll.ReRender();
			}
			ArtisanOrder artisanOrder2 = this._displayData.ArtisanOrder;
			int addValue = (artisanOrder2 != null) ? artisanOrder2.ProgressDelta : 0;
			int showAddValue = this.GetProgressPercent(addValue);
			string curStr = string.Format("{0}%", showCurValue);
			string addStr = string.Format("{0}%", showAddValue).SetColor("brightblue");
			this.textProductProgress.text = LanguageKey.LK_Craftsman_CraftProgress_Content.TrFormat(curStr, addStr);
		}

		// Token: 0x060072DD RID: 29405 RVA: 0x003565C1 File Offset: 0x003547C1
		private void ResourceToggleGroupOnActiveIndexChange(int newIndex, int oldIndex)
		{
			this.RefreshResourceList();
		}

		// Token: 0x060072DE RID: 29406 RVA: 0x003565CC File Offset: 0x003547CC
		private void OnClickButtonAddMaterial()
		{
			bool isAddMaterial = this.IsAddMaterial;
			if (isAddMaterial)
			{
				this.CancelAddMaterial();
			}
			else
			{
				this.EnterAddMaterial();
			}
		}

		// Token: 0x060072DF RID: 29407 RVA: 0x003565F4 File Offset: 0x003547F4
		private void OnClickButtonCancelAddMaterial()
		{
			this.CancelAddMaterial();
		}

		// Token: 0x060072E0 RID: 29408 RVA: 0x00356600 File Offset: 0x00354800
		private void OnClickButtonConfirmAddMaterial()
		{
			string title = LocalStringManager.Get(LanguageKey.LK_VillagerCraftInputMaterial_ConfirmDialog_Title);
			string content = LocalStringManager.Get(LanguageKey.LK_VillagerCraftInputMaterial_ConfirmDialog_Content);
			CommonUtils.ShowConfirmDialog(title, content, delegate
			{
				this._previewProductionPool = null;
				List<ItemDisplayData> itemList = (from p in this._selectedItemDict
				select p.Key.Clone(p.Value)).ToList<ItemDisplayData>();
				ExtraDomainMethod.Call.AddMaterialToArtisanOrder(this._displayData.ArtisanOrder, itemList);
				this._selectedItemDict.Clear();
				this.ShowAddMaterailParticle();
				this.RequestData();
				this.CancelAddMaterial();
			}, null, EDialogType.None);
		}

		// Token: 0x060072E1 RID: 29409 RVA: 0x0035663C File Offset: 0x0035483C
		private void EnterAddMaterial()
		{
			this.EnterFocus(new List<GameObject>
			{
				this.materialPanel,
				this.buttonAddMaterialFill.gameObject,
				this.imageProductType.transform.parent.gameObject,
				this.productPanel
			}, null);
			this.RefreshMaterialList();
			this.RefreshSelectMaterialList();
			this.goSelectMaterialArea.SetActive(false);
			this.toggleShowSelectMaterialList.SetIsOnWithoutNotify(false);
			this.materialPanel.gameObject.SetActive(true);
		}

		// Token: 0x060072E2 RID: 29410 RVA: 0x003566DC File Offset: 0x003548DC
		private void CancelAddMaterial()
		{
			this.materialPanel.gameObject.SetActive(false);
			this.ExitFocus();
			this._selectedItemDict.Clear();
			this._previewProductionPool = null;
			this.CheckPutMaterial();
			AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
		}

		// Token: 0x17000D12 RID: 3346
		// (get) Token: 0x060072E3 RID: 29411 RVA: 0x0035672F File Offset: 0x0035492F
		private bool IsAddMaterial
		{
			get
			{
				return this.materialPanel.activeSelf;
			}
		}

		// Token: 0x060072E4 RID: 29412 RVA: 0x0035673C File Offset: 0x0035493C
		private void RefreshMaterialList()
		{
			List<ItemDisplayData> list = this.GetItemList(this.CurMaterialItemSource);
			short itemSubType = this.CurItemSubType;
			sbyte resourceType = ViewCraftsman.GetResourceTypeByCraftType(this._currentCraftType);
			Func<short, bool> <>9__1;
			IEnumerable<ItemDisplayData> availableItemList = list.Where(delegate(ItemDisplayData itemData)
			{
				bool flag = itemData.Key.ItemType != 5;
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					MaterialItem materialConfig = Config.Material.Instance[itemData.Key.TemplateId];
					bool flag2 = materialConfig.CraftableItemTypes.Count <= 0;
					if (flag2)
					{
						result = false;
					}
					else
					{
						bool flag3;
						if (itemSubType >= 0)
						{
							IEnumerable<short> craftableItemTypes = materialConfig.CraftableItemTypes;
							Func<short, bool> predicate;
							if ((predicate = <>9__1) == null)
							{
								predicate = (<>9__1 = ((short id) => MakeItemType.Instance[id].ItemSubType == itemSubType));
							}
							flag3 = craftableItemTypes.Any(predicate);
						}
						else
						{
							flag3 = true;
						}
						bool isMeetItemType = flag3;
						bool flag4 = !isMeetItemType;
						if (flag4)
						{
							result = false;
						}
						else
						{
							bool isMeetResourceType = resourceType < 0 || materialConfig.ResourceType == resourceType;
							bool flag5 = !isMeetResourceType;
							result = !flag5;
						}
					}
				}
				return result;
			});
			this._materialItemList.Clear();
			this._materialItemList.AddRange(availableItemList);
			this.materialListScroll.SetItemList(this._materialItemList);
		}

		// Token: 0x060072E5 RID: 29413 RVA: 0x003567B8 File Offset: 0x003549B8
		private void OnItemClickMaterial(ITradeableContent content, RowItemLine rowItemLine)
		{
			ItemDisplayData itemData = content as ItemDisplayData;
			bool flag = this._selectedItemDict.Remove(itemData);
			if (flag)
			{
				this.CheckPutMaterial();
			}
			else
			{
				bool flag2 = content.Amount == 1;
				if (flag2)
				{
					this.SetItemSelectCount(content, 1);
					this.CheckPutMaterial();
				}
				else
				{
					this.materialListScroll.SetItemToSelectCountMode(rowItemLine, delegate(int count)
					{
						this.SetItemSelectCount(content, count);
						this.CheckPutMaterial();
					}, delegate
					{
						bool flag3 = this._selectedItemDict.Remove(itemData);
						if (flag3)
						{
							this.CheckPutMaterial();
						}
					}, 0, 0, 1, null, false, null, false);
				}
			}
		}

		// Token: 0x060072E6 RID: 29414 RVA: 0x00356860 File Offset: 0x00354A60
		private void OnItemRenderMaterial(ITradeableContent content, RowItemLine rowItemLine)
		{
			ItemDisplayData itemData = content as ItemDisplayData;
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			bool isSelected = this._selectedItemDict.ContainsKey(itemData);
			rowItemLine.SetSelected(isSelected);
			rowItemLine.SetInteractable(true, true);
		}

		// Token: 0x060072E7 RID: 29415 RVA: 0x003568AB File Offset: 0x00354AAB
		private void MaterialToggleGroupOnActiveIndexChange(int newIndex, int oldIndex)
		{
			this.RefreshMaterialList();
		}

		// Token: 0x060072E8 RID: 29416 RVA: 0x003568B8 File Offset: 0x00354AB8
		private void CheckPutMaterial()
		{
			this.buttonConfirmAddMaterial.interactable = (this._selectedItemDict.Count > 0);
			List<ItemDisplayData> itemList = (from p in this._selectedItemDict
			select p.Key.Clone(p.Value)).ToList<ItemDisplayData>();
			ExtraDomainMethod.AsyncCall.GetArtisanOrderMaterialPreview(this, this._displayData.ArtisanOrder, itemList, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._previewProductionPool);
				this.RefreshProductPool();
				this.materialListScroll.ReRender();
			});
		}

		// Token: 0x060072E9 RID: 29417 RVA: 0x0035692F File Offset: 0x00354B2F
		private void OnClickButtonRemoveMaterialAll()
		{
			this._selectedItemDict.Clear();
			this._previewProductionPool = null;
			this.CheckPutMaterial();
			this.RefreshSelectMaterialList();
		}

		// Token: 0x060072EA RID: 29418 RVA: 0x00356954 File Offset: 0x00354B54
		private void OnClickButtonAddMaterialAll()
		{
			this._selectedItemDict.Clear();
			IReadOnlyList<ITradeableContent> filterData = this.materialListScroll.FilteredData;
			foreach (ITradeableContent data in filterData)
			{
				ItemDisplayData itemData = data as ItemDisplayData;
				bool flag = itemData != null;
				if (flag)
				{
					this._selectedItemDict.Add(itemData, itemData.Amount);
				}
			}
			this.RefreshMaterialList();
			this.CheckPutMaterial();
			this.RefreshSelectMaterialList();
		}

		// Token: 0x060072EB RID: 29419 RVA: 0x003569F0 File Offset: 0x00354BF0
		private void ShowSelectMaterialList(bool isOn)
		{
			this.goSelectMaterialArea.SetActive(isOn);
			this.RefreshSelectMaterialList();
		}

		// Token: 0x060072EC RID: 29420 RVA: 0x00356A08 File Offset: 0x00354C08
		private void OnItemRenderSelectMaterial(ITradeableContent content, RowItemLine rowItemLine)
		{
			ItemDisplayData itemData = content as ItemDisplayData;
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			rowItemLine.SetSelected(false);
			rowItemLine.SetInteractable(true, true);
		}

		// Token: 0x060072ED RID: 29421 RVA: 0x00356A46 File Offset: 0x00354C46
		private void OnItemClickSelectMaterial(ITradeableContent content, RowItemLine rowItemLine)
		{
			this.SetItemSelectCount(content, 0);
			this.CheckPutMaterial();
		}

		// Token: 0x060072EE RID: 29422 RVA: 0x00356A5C File Offset: 0x00354C5C
		private void RefreshSelectMaterialList()
		{
			List<ItemDisplayData> list = new List<ItemDisplayData>();
			foreach (KeyValuePair<ItemDisplayData, int> info in this._selectedItemDict)
			{
				list.Add(info.Key);
			}
			this.selectMaterialListScroll.SetItemList(list);
			this.txtSelectMaterialCount.text = string.Format("{0}", list.Count);
		}

		// Token: 0x060072EF RID: 29423 RVA: 0x00356AF0 File Offset: 0x00354CF0
		private void InitParticle()
		{
			this._addMaterailParticle = this.buttonAddMaterialFill.GetComponentInChildren<UIParticle>(true);
			this._addResourceParticle = this.buttonAddResourceFill.GetComponentInChildren<UIParticle>(true);
			this.HideAddMaterailParticle();
			this.HideAddResourceParticle();
		}

		// Token: 0x060072F0 RID: 29424 RVA: 0x00356B28 File Offset: 0x00354D28
		private void ShowAddMaterailParticle()
		{
			bool flag = this._addMaterailParticle == null;
			if (!flag)
			{
				this._particlePlayHelper.PlayOnceParticle(this._addMaterailParticle, 1f, null);
			}
		}

		// Token: 0x060072F1 RID: 29425 RVA: 0x00356B60 File Offset: 0x00354D60
		private void HideAddMaterailParticle()
		{
			bool flag = this._addMaterailParticle == null;
			if (!flag)
			{
				this._addMaterailParticle.gameObject.SetActive(false);
			}
		}

		// Token: 0x060072F2 RID: 29426 RVA: 0x00356B94 File Offset: 0x00354D94
		private void ShowAddResourceParticle()
		{
			bool flag = this._addResourceParticle == null;
			if (!flag)
			{
				this._particlePlayHelper.PlayOnceParticle(this._addResourceParticle, 1f, null);
			}
		}

		// Token: 0x060072F3 RID: 29427 RVA: 0x00356BCC File Offset: 0x00354DCC
		private void HideAddResourceParticle()
		{
			bool flag = this._addResourceParticle == null;
			if (!flag)
			{
				this._addResourceParticle.gameObject.SetActive(false);
			}
		}

		// Token: 0x060072F4 RID: 29428 RVA: 0x00356C00 File Offset: 0x00354E00
		private void InitStorageDropdown()
		{
			this.storageDropdown.onValueChanged.RemoveAllListeners();
			this.storageDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnStorageDropdownValueChanged));
			this.storageDropdown.OnItemAdded = new Action<int, RectTransform>(this.OnStorageDropdownItemAdded);
			this.storageDropdown.ClearOptions();
			List<string> list = (from k in MakeSubPageMake.StorageNameKeys
			select k.Tr()).ToList<string>();
			this.storageDropdown.AddOptions(list);
		}

		// Token: 0x060072F5 RID: 29429 RVA: 0x00356C9B File Offset: 0x00354E9B
		private void OnStorageDropdownItemAdded(int index, RectTransform rectTransform)
		{
		}

		// Token: 0x060072F6 RID: 29430 RVA: 0x00356CA0 File Offset: 0x00354EA0
		private void OnStorageDropdownValueChanged(int value)
		{
			if (!true)
			{
			}
			ItemSourceType itemSourceType;
			switch (value)
			{
			case 0:
				itemSourceType = ItemSourceType.Inventory;
				break;
			case 1:
				itemSourceType = ItemSourceType.Warehouse;
				break;
			case 2:
				itemSourceType = ItemSourceType.Treasury;
				break;
			case 3:
				itemSourceType = ItemSourceType.Stock;
				break;
			default:
				throw new ArgumentOutOfRangeException("value", value, null);
			}
			if (!true)
			{
			}
			ItemSourceType type = itemSourceType;
			ExtraDomainMethod.Call.SetArtisanOrderStorageType(this._displayData.ArtisanOrder, type);
			this.RequestData();
		}

		// Token: 0x060072F7 RID: 29431 RVA: 0x00356D0C File Offset: 0x00354F0C
		private void RefreshStorageDropdown()
		{
			bool flag = this._displayData.ArtisanOrder == null;
			if (flag)
			{
				this.storageDropdown.gameObject.SetActive(false);
			}
			else
			{
				ItemSourceType storageType = (ItemSourceType)this._displayData.ArtisanOrder.StorageType;
				if (!true)
				{
				}
				int num;
				switch (storageType)
				{
				case ItemSourceType.Inventory:
					num = 0;
					goto IL_71;
				case ItemSourceType.Warehouse:
					num = 1;
					goto IL_71;
				case ItemSourceType.Treasury:
					num = 2;
					goto IL_71;
				case ItemSourceType.Stock:
					num = 3;
					goto IL_71;
				}
				throw new ArgumentOutOfRangeException();
				IL_71:
				if (!true)
				{
				}
				int value = num;
				this.storageDropdown.SetValueWithoutNotify(value);
				this.storageDropdown.gameObject.SetActive(true);
			}
		}

		// Token: 0x17000D13 RID: 3347
		// (get) Token: 0x060072F8 RID: 29432 RVA: 0x00356DAF File Offset: 0x00354FAF
		private bool IsFocus
		{
			get
			{
				return this.focusPanel.activeSelf;
			}
		}

		// Token: 0x060072F9 RID: 29433 RVA: 0x00356DBC File Offset: 0x00354FBC
		private void EnterFocus(List<GameObject> targetList, Action onExit = null)
		{
			bool isFocus = this.IsFocus;
			if (!isFocus)
			{
				this.focusPanel.SetActive(true);
				this._focusList.Clear();
				foreach (GameObject target in targetList)
				{
					this._focusList.Add(new ValueTuple<Transform, Transform, int>(target.transform, target.transform.parent, target.transform.GetSiblingIndex()));
					target.transform.SetParent(this.focusPanel.transform);
				}
				this._onExitFocus = onExit;
			}
		}

		// Token: 0x060072FA RID: 29434 RVA: 0x00356E7C File Offset: 0x0035507C
		private void ExitFocus()
		{
			bool flag = !this.IsFocus;
			if (!flag)
			{
				this.focusPanel.SetActive(false);
				this._focusList.Reverse();
				foreach (ValueTuple<Transform, Transform, int> valueTuple in this._focusList)
				{
					Transform self = valueTuple.Item1;
					Transform parent = valueTuple.Item2;
					int index = valueTuple.Item3;
					self.SetParent(parent);
					self.SetSiblingIndex(index);
				}
				this._focusList.Clear();
				Action onExitFocus = this._onExitFocus;
				if (onExitFocus != null)
				{
					onExitFocus();
				}
			}
		}

		// Token: 0x060072FB RID: 29435 RVA: 0x00356F3C File Offset: 0x0035513C
		public void InitArtisanOrderProductionSet()
		{
			this._productionPoolDic.Clear();
			bool flag = this._displayData.ArtisanOrder == null;
			if (!flag)
			{
				ArtisanOrder order = this._displayData.ArtisanOrder;
				sbyte lifeSkillType = order.LifeSkillType;
				sbyte b = lifeSkillType;
				sbyte b2 = b;
				if (b2 != 5)
				{
					if (b2 != 14)
					{
						foreach (MaterialItem materialConfig in ((IEnumerable<MaterialItem>)Config.Material.Instance))
						{
							bool flag2 = materialConfig.RequiredLifeSkillType != lifeSkillType || materialConfig.IsSpecial;
							if (!flag2)
							{
								for (int index = 0; index < materialConfig.CraftableItemTypes.Count; index++)
								{
									short makeItemType = materialConfig.CraftableItemTypes[index];
									foreach (short makeItemSubType in MakeItemType.Instance[makeItemType].MakeItemSubTypes)
									{
										MakeItemResult result = MakeItemSubType.Instance[makeItemSubType].Result;
										bool flag3 = order.ItemSubType < 0 || ItemTemplateHelper.GetItemSubType(result.ItemType, result.TemplateId) == order.ItemSubType;
										if (flag3)
										{
											this.<InitArtisanOrderProductionSet>g__AddToDic|214_0(result.ItemType, this.GetProductionSet(result.ItemType, result.TemplateId, materialConfig.Grade, 0));
										}
									}
								}
							}
						}
					}
					else
					{
						foreach (MaterialItem materialConfig2 in ((IEnumerable<MaterialItem>)Config.Material.Instance))
						{
							bool flag4 = materialConfig2.RequiredLifeSkillType != lifeSkillType || materialConfig2.IsSpecial;
							if (!flag4)
							{
								foreach (short makeItemType2 in materialConfig2.CraftableItemTypes)
								{
									foreach (short makeItemSubType2 in MakeItemType.Instance[makeItemType2].MakeItemSubTypes)
									{
										MakeItemResult result2 = MakeItemSubType.Instance[makeItemSubType2].Result;
										bool flag5 = order.ItemSubType < 0 || ItemTemplateHelper.GetItemSubType(result2.ItemType, result2.TemplateId) == order.ItemSubType;
										if (flag5)
										{
											this.<InitArtisanOrderProductionSet>g__AddToDic|214_0(result2.ItemType, this.GetProductionSet(result2.ItemType, result2.TemplateId, materialConfig2.Grade, 8));
										}
									}
								}
							}
						}
					}
				}
				else
				{
					foreach (TeaWineItem teaWineConfig in ((IEnumerable<TeaWineItem>)TeaWine.Instance))
					{
						bool flag6 = teaWineConfig.ItemSubType == order.ItemSubType || (order.IsArtisanOrder() && order.ItemSubType < 0);
						if (flag6)
						{
							this.<InitArtisanOrderProductionSet>g__AddToDic|214_0(teaWineConfig.ItemType, this.GetProductionSet(teaWineConfig.ItemType, teaWineConfig.TemplateId, teaWineConfig.Grade, 0));
						}
					}
				}
			}
		}

		// Token: 0x060072FC RID: 29436 RVA: 0x00357344 File Offset: 0x00355544
		private HashSet<short> GetProductionSet(sbyte itemType, short baseTemplateId, sbyte baseGrade, int addOn = 8)
		{
			HashSet<short> result = new HashSet<short>();
			int upperGrade = 2 + addOn;
			for (int i = -1; i < upperGrade; i++)
			{
				short templateId;
				bool flag = !ProductionPool.TryGetProductionTemplateId((sbyte)((int)baseGrade + i), itemType, baseTemplateId, out templateId);
				if (!flag)
				{
					result.Add(templateId);
				}
			}
			return result;
		}

		// Token: 0x060072FD RID: 29437 RVA: 0x00357398 File Offset: 0x00355598
		public void RefreshSourceToggleInteractable(CToggleGroup toggleGroup)
		{
			bool isBuilding = this.IsBuilding;
			if (isBuilding)
			{
				ItemSourceToggleHelper.RefreshInteractableForBuilding(toggleGroup, this._displayData.CanTransferItemToWarehouse);
			}
			else
			{
				ItemSourceToggleHelper.RefreshInteractableForInteract(toggleGroup, this._displayData.CanTransferItemToWarehouse, false);
			}
		}

		// Token: 0x060072FE RID: 29438 RVA: 0x003573D8 File Offset: 0x003555D8
		private void SetItemSelectCount(ITradeableContent content, int count)
		{
			ItemDisplayData itemData = content as ItemDisplayData;
			int lastCount;
			bool flag = this._selectedItemDict.TryGetValue(itemData, out lastCount) && count == lastCount;
			if (!flag)
			{
				bool flag2 = this._selectedItemDict.ContainsKey(itemData);
				if (flag2)
				{
					this._selectedItemDict[itemData] = count;
					bool flag3 = count <= 0;
					if (flag3)
					{
						this._selectedItemDict.Remove(itemData);
					}
				}
				else
				{
					bool flag4 = count <= 0;
					if (flag4)
					{
						return;
					}
					this._selectedItemDict.Add(itemData, count);
				}
				bool isAddResource = this.IsAddResource;
				if (isAddResource)
				{
					this.RefreshSelectResourceList();
				}
				bool isAddMaterial = this.IsAddMaterial;
				if (isAddMaterial)
				{
					this.RefreshSelectMaterialList();
				}
			}
		}

		// Token: 0x060072FF RID: 29439 RVA: 0x00357490 File Offset: 0x00355690
		private List<ItemDisplayData> GetItemList(ItemSourceType sourceType)
		{
			if (!true)
			{
			}
			List<ItemDisplayData> result;
			switch (sourceType)
			{
			case ItemSourceType.Inventory:
				result = (this._displayData.InventoryItemList ?? new List<ItemDisplayData>());
				break;
			case ItemSourceType.Warehouse:
				result = (this._displayData.WarehouseItemList ?? new List<ItemDisplayData>());
				break;
			case ItemSourceType.Treasury:
				result = (this._displayData.TreasuryItemList ?? new List<ItemDisplayData>());
				break;
			default:
				throw new ArgumentOutOfRangeException("sourceType", sourceType, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007300 RID: 29440 RVA: 0x00357514 File Offset: 0x00355714
		public static sbyte GetResourceTypeByCraftType(ViewCraftsman.ECraftType craftType)
		{
			if (!true)
			{
			}
			sbyte result;
			switch (craftType)
			{
			case ViewCraftsman.ECraftType.None:
				result = -1;
				break;
			case ViewCraftsman.ECraftType.Tea:
				result = 6;
				break;
			case ViewCraftsman.ECraftType.Wine:
				result = 6;
				break;
			case ViewCraftsman.ECraftType.Forging:
				result = 2;
				break;
			case ViewCraftsman.ECraftType.Woodworking:
				result = 1;
				break;
			case ViewCraftsman.ECraftType.Weaving:
				result = 4;
				break;
			case ViewCraftsman.ECraftType.Medicine:
				result = 5;
				break;
			case ViewCraftsman.ECraftType.Toxicology:
				result = 5;
				break;
			case ViewCraftsman.ECraftType.Cooking:
				result = 0;
				break;
			case ViewCraftsman.ECraftType.Jade:
				result = 3;
				break;
			default:
				throw new ArgumentOutOfRangeException("craftType", craftType, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007301 RID: 29441 RVA: 0x00357594 File Offset: 0x00355794
		public static sbyte GetOperationNeedSkillType(ViewCraftsman.ECraftType craftType)
		{
			if (!true)
			{
			}
			sbyte result;
			switch (craftType)
			{
			case ViewCraftsman.ECraftType.Tea:
				result = 5;
				break;
			case ViewCraftsman.ECraftType.Wine:
				result = 5;
				break;
			case ViewCraftsman.ECraftType.Forging:
				result = 6;
				break;
			case ViewCraftsman.ECraftType.Woodworking:
				result = 7;
				break;
			case ViewCraftsman.ECraftType.Weaving:
				result = 10;
				break;
			case ViewCraftsman.ECraftType.Medicine:
				result = 8;
				break;
			case ViewCraftsman.ECraftType.Toxicology:
				result = 9;
				break;
			case ViewCraftsman.ECraftType.Cooking:
				result = 14;
				break;
			case ViewCraftsman.ECraftType.Jade:
				result = 11;
				break;
			default:
				result = -1;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007302 RID: 29442 RVA: 0x0035760C File Offset: 0x0035580C
		public static string GetCraftTypesNameByEnum(ViewCraftsman.ECraftType craftType)
		{
			string result;
			switch (craftType)
			{
			case ViewCraftsman.ECraftType.Tea:
				result = LocalStringManager.Get(LanguageKey.LK_Craftsman_Tea);
				break;
			case ViewCraftsman.ECraftType.Wine:
				result = LocalStringManager.Get(LanguageKey.LK_Craftsman_Alcohol);
				break;
			case ViewCraftsman.ECraftType.Forging:
				result = LocalStringManager.Get(LanguageKey.LK_LifeSkillType_6);
				break;
			case ViewCraftsman.ECraftType.Woodworking:
				result = LocalStringManager.Get(LanguageKey.LK_LifeSkillType_7);
				break;
			case ViewCraftsman.ECraftType.Weaving:
				result = LocalStringManager.Get(LanguageKey.LK_LifeSkillType_10);
				break;
			case ViewCraftsman.ECraftType.Medicine:
				result = LocalStringManager.Get(LanguageKey.LK_MedicineStorageMaterial);
				break;
			case ViewCraftsman.ECraftType.Toxicology:
				result = LocalStringManager.Get(LanguageKey.LK_Make_Poison);
				break;
			case ViewCraftsman.ECraftType.Cooking:
				result = LocalStringManager.Get(LanguageKey.LK_Cooking);
				break;
			case ViewCraftsman.ECraftType.Jade:
				result = LocalStringManager.Get(LanguageKey.LK_LifeSkillType_11);
				break;
			default:
				result = "";
				break;
			}
			return result;
		}

		// Token: 0x06007303 RID: 29443 RVA: 0x003576CC File Offset: 0x003558CC
		private static string GetCraftsmanTitleByCraftType(sbyte craftsmanType)
		{
			if (!true)
			{
			}
			LanguageKey languageKey;
			switch (craftsmanType)
			{
			case 63:
				languageKey = LanguageKey.LK_Craftsman_CraftsmanTitle;
				break;
			case 64:
				languageKey = LanguageKey.LK_Craftsman_DoctorTitle;
				break;
			case 65:
				languageKey = LanguageKey.LK_Craftsman_PeasantTitle;
				break;
			case 66:
				languageKey = LanguageKey.LK_Craftsman_LiteratiTitle;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!true)
			{
			}
			LanguageKey lStringKey = languageKey;
			return LocalStringManager.Get(lStringKey);
		}

		// Token: 0x06007304 RID: 29444 RVA: 0x00357730 File Offset: 0x00355930
		public static string GetLeaderNameByCraftType(ViewCraftsman.ECraftType craftType)
		{
			if (!true)
			{
			}
			short num;
			switch (craftType)
			{
			case ViewCraftsman.ECraftType.None:
				num = -1;
				break;
			case ViewCraftsman.ECraftType.Tea:
				num = 127;
				break;
			case ViewCraftsman.ECraftType.Wine:
				num = 128;
				break;
			case ViewCraftsman.ECraftType.Forging:
				num = 129;
				break;
			case ViewCraftsman.ECraftType.Woodworking:
				num = 139;
				break;
			case ViewCraftsman.ECraftType.Weaving:
				num = 169;
				break;
			case ViewCraftsman.ECraftType.Medicine:
				num = 149;
				break;
			case ViewCraftsman.ECraftType.Toxicology:
				num = 159;
				break;
			case ViewCraftsman.ECraftType.Cooking:
				num = 203;
				break;
			case ViewCraftsman.ECraftType.Jade:
				num = 179;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!true)
			{
			}
			short buildingBlockTemplateId = num;
			BuildingBlockItem config = BuildingBlock.Instance.GetItem(buildingBlockTemplateId);
			return (config != null) ? config.LeaderName : null;
		}

		// Token: 0x06007305 RID: 29445 RVA: 0x003577E4 File Offset: 0x003559E4
		private ViewCraftsman.ECraftType GetCraftTypeForBuilding()
		{
			short templateId = this._buildingConfig.TemplateId;
			if (!true)
			{
			}
			ViewCraftsman.ECraftType result;
			if (templateId <= 149)
			{
				switch (templateId)
				{
				case 127:
					result = ViewCraftsman.ECraftType.Tea;
					goto IL_9F;
				case 128:
					result = ViewCraftsman.ECraftType.Wine;
					goto IL_9F;
				case 129:
					result = ViewCraftsman.ECraftType.Forging;
					goto IL_9F;
				default:
					if (templateId == 139)
					{
						result = ViewCraftsman.ECraftType.Woodworking;
						goto IL_9F;
					}
					if (templateId == 149)
					{
						result = ViewCraftsman.ECraftType.Medicine;
						goto IL_9F;
					}
					break;
				}
			}
			else if (templateId <= 169)
			{
				if (templateId == 159)
				{
					result = ViewCraftsman.ECraftType.Toxicology;
					goto IL_9F;
				}
				if (templateId == 169)
				{
					result = ViewCraftsman.ECraftType.Weaving;
					goto IL_9F;
				}
			}
			else
			{
				if (templateId == 179)
				{
					result = ViewCraftsman.ECraftType.Jade;
					goto IL_9F;
				}
				if (templateId == 203)
				{
					result = ViewCraftsman.ECraftType.Cooking;
					goto IL_9F;
				}
			}
			throw new ArgumentOutOfRangeException();
			IL_9F:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007306 RID: 29446 RVA: 0x0035789C File Offset: 0x00355A9C
		private short GetItemSubType(ViewCraftsman.ECraftType craftType)
		{
			if (!true)
			{
			}
			short result;
			if (craftType != ViewCraftsman.ECraftType.Tea)
			{
				if (craftType != ViewCraftsman.ECraftType.Wine)
				{
					ArtisanOrder artisanOrder = this._displayData.ArtisanOrder;
					result = ((artisanOrder != null) ? artisanOrder.ItemSubType : -1);
				}
				else
				{
					result = 901;
				}
			}
			else
			{
				result = 900;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007307 RID: 29447 RVA: 0x003578E8 File Offset: 0x00355AE8
		private string AmountCellDataGenerator(ITradeableContent content)
		{
			ItemDisplayData itemData = content as ItemDisplayData;
			bool flag = itemData == null;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				int selectedCount;
				this._selectedItemDict.TryGetValue(itemData, out selectedCount);
				bool isSelected = selectedCount > 0;
				string maxAmountStr = CommonUtils.GetDisplayStringForNum(itemData.Amount, 100000);
				bool flag2 = !isSelected;
				if (flag2)
				{
					result = maxAmountStr;
				}
				else
				{
					string selectedAmountStr = CommonUtils.GetDisplayStringForNum(selectedCount, 100000);
					result = selectedAmountStr + "/" + maxAmountStr;
				}
			}
			return result;
		}

		// Token: 0x06007314 RID: 29460 RVA: 0x00357BA4 File Offset: 0x00355DA4
		[CompilerGenerated]
		private void <InitArtisanOrderProductionSet>g__AddToDic|214_0(sbyte itemType, HashSet<short> hashSet)
		{
			bool flag = !this._productionPoolDic.ContainsKey(itemType);
			if (flag)
			{
				this._productionPoolDic[itemType] = new HashSet<short>();
			}
			foreach (short item in hashSet)
			{
				this._productionPoolDic[itemType].Add(item);
			}
		}

		// Token: 0x04005503 RID: 21763
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x04005504 RID: 21764
		[SerializeField]
		private CButton buttonClose;

		// Token: 0x04005505 RID: 21765
		[SerializeField]
		private GameObject focusPanel;

		// Token: 0x04005506 RID: 21766
		[SerializeField]
		private CanvasGroup canvasGroup;

		// Token: 0x04005507 RID: 21767
		[SerializeField]
		private Animation effect;

		// Token: 0x04005508 RID: 21768
		[Header("建筑代制")]
		[SerializeField]
		private GameObject buildingPanel;

		// Token: 0x04005509 RID: 21769
		[SerializeField]
		private TextMeshProUGUI textManagerLeaderTitle;

		// Token: 0x0400550A RID: 21770
		[SerializeField]
		private BuildingManagerLeaderView managerLeaderView;

		// Token: 0x0400550B RID: 21771
		[SerializeField]
		private TextMeshProUGUI textManagerMemberTitle;

		// Token: 0x0400550C RID: 21772
		[SerializeField]
		private BuildingManagerMemberView[] managerMemberViewArray;

		// Token: 0x0400550D RID: 21773
		[SerializeField]
		private CButton buttonShopQuickSelect;

		// Token: 0x0400550E RID: 21774
		[SerializeField]
		private CButton buttonShopQuickCancel;

		// Token: 0x0400550F RID: 21775
		[Header("切换建筑")]
		[SerializeField]
		private GameObject rootSwitch;

		// Token: 0x04005510 RID: 21776
		[SerializeField]
		private CToggle toggleSwitch;

		// Token: 0x04005511 RID: 21777
		[SerializeField]
		private CToggleGroup toggleGroupSwitch;

		// Token: 0x04005512 RID: 21778
		[Header("人物代制")]
		[SerializeField]
		private GameObject characterPanel;

		// Token: 0x04005513 RID: 21779
		[SerializeField]
		private TextMeshProUGUI textArtisan;

		// Token: 0x04005514 RID: 21780
		[SerializeField]
		private BuildingManagerLeaderView artisanView;

		// Token: 0x04005515 RID: 21781
		[SerializeField]
		private BuildingManagerMemberView subscriberView;

		// Token: 0x04005516 RID: 21782
		[SerializeField]
		private GameObject objectNoneSubscriber;

		// Token: 0x04005517 RID: 21783
		[SerializeField]
		private TextMeshProUGUI textOrderState;

		// Token: 0x04005518 RID: 21784
		[SerializeField]
		private TextMeshProUGUI textOrderCost;

		// Token: 0x04005519 RID: 21785
		[SerializeField]
		private CButton buttonNegotiate;

		// Token: 0x0400551A RID: 21786
		[SerializeField]
		private TextMeshProUGUI textNegotiateNeed;

		// Token: 0x0400551B RID: 21787
		[SerializeField]
		private TMPTextSpriteHelper helperNegotiateNeed;

		// Token: 0x0400551C RID: 21788
		[SerializeField]
		private CButton buttonOrder;

		// Token: 0x0400551D RID: 21789
		[SerializeField]
		private CButton buttonInterceptOrder;

		// Token: 0x0400551E RID: 21790
		[SerializeField]
		private CToggleGroup craftTypeToggleGroup;

		// Token: 0x0400551F RID: 21791
		[Header("产物")]
		[SerializeField]
		private GameObject backProductEmpty;

		// Token: 0x04005520 RID: 21792
		[SerializeField]
		private GameObject backProductFill;

		// Token: 0x04005521 RID: 21793
		[SerializeField]
		private CButton buttonAddMaterialEmpty;

		// Token: 0x04005522 RID: 21794
		[SerializeField]
		private CButton buttonAddResourceEmpty;

		// Token: 0x04005523 RID: 21795
		[SerializeField]
		private CButton buttonAddMaterialFill;

		// Token: 0x04005524 RID: 21796
		[SerializeField]
		private CButton buttonAddResourceFill;

		// Token: 0x04005525 RID: 21797
		[SerializeField]
		private CImage imageProductType;

		// Token: 0x04005526 RID: 21798
		[SerializeField]
		private CDropdown productTypeDropdown;

		// Token: 0x04005527 RID: 21799
		[SerializeField]
		private GameObject rootProductProgress;

		// Token: 0x04005528 RID: 21800
		[SerializeField]
		private CImage imageProductProgress;

		// Token: 0x04005529 RID: 21801
		[SerializeField]
		private CImage imageProductProgressChange;

		// Token: 0x0400552A RID: 21802
		[SerializeField]
		private TextMeshProUGUI textProductProgress;

		// Token: 0x0400552B RID: 21803
		[SerializeField]
		private TooltipInvoker tipProductProgress;

		// Token: 0x0400552C RID: 21804
		[SerializeField]
		private CDropdown storageDropdown;

		// Token: 0x0400552D RID: 21805
		[SerializeField]
		private GameObject productPanel;

		// Token: 0x0400552E RID: 21806
		[SerializeField]
		private ItemListScroll productListScroll;

		// Token: 0x0400552F RID: 21807
		[Header("投入引子")]
		[SerializeField]
		private GameObject materialPanel;

		// Token: 0x04005530 RID: 21808
		[SerializeField]
		private ItemListScroll materialListScroll;

		// Token: 0x04005531 RID: 21809
		[SerializeField]
		private CToggleGroup materialToggleGroup;

		// Token: 0x04005532 RID: 21810
		[SerializeField]
		private CButton buttonConfirmAddMaterial;

		// Token: 0x04005533 RID: 21811
		[SerializeField]
		private CButton buttonCancelAddMaterial;

		// Token: 0x04005534 RID: 21812
		[SerializeField]
		private CButton buttonAddMaterialAll;

		// Token: 0x04005535 RID: 21813
		[SerializeField]
		private CButton buttonRemoveMaterialAll;

		// Token: 0x04005536 RID: 21814
		[SerializeField]
		private CToggle toggleShowSelectMaterialList;

		// Token: 0x04005537 RID: 21815
		[SerializeField]
		private ItemListScroll selectMaterialListScroll;

		// Token: 0x04005538 RID: 21816
		[SerializeField]
		private GameObject goSelectMaterialArea;

		// Token: 0x04005539 RID: 21817
		[SerializeField]
		private TextMeshProUGUI txtSelectMaterialCount;

		// Token: 0x0400553A RID: 21818
		[Header("投入资源")]
		[SerializeField]
		private GameObject resourcePanel;

		// Token: 0x0400553B RID: 21819
		[SerializeField]
		private ItemListScroll resourceListScroll;

		// Token: 0x0400553C RID: 21820
		[SerializeField]
		private CToggleGroup resourceToggleGroup;

		// Token: 0x0400553D RID: 21821
		[SerializeField]
		private CButton buttonConfirmAddResource;

		// Token: 0x0400553E RID: 21822
		[SerializeField]
		private CButton buttonCancelAddResource;

		// Token: 0x0400553F RID: 21823
		[SerializeField]
		private CButton buttonAddResourceAll;

		// Token: 0x04005540 RID: 21824
		[SerializeField]
		private CButton buttonRemoveResourceAll;

		// Token: 0x04005541 RID: 21825
		[SerializeField]
		private CToggle toggleShowSelectResourceList;

		// Token: 0x04005542 RID: 21826
		[SerializeField]
		private ItemListScroll selectResourceListScroll;

		// Token: 0x04005543 RID: 21827
		[SerializeField]
		private GameObject goSelectResourceArea;

		// Token: 0x04005544 RID: 21828
		[SerializeField]
		private TextMeshProUGUI txtSelectResourceCount;

		// Token: 0x04005545 RID: 21829
		private BuildingBlockKey _blockKey;

		// Token: 0x04005546 RID: 21830
		private BuildingBlockData _blockData;

		// Token: 0x04005547 RID: 21831
		private BuildingBlockItem _buildingConfig;

		// Token: 0x04005548 RID: 21832
		private readonly StringBuilder _stringBuilder = new StringBuilder();

		// Token: 0x04005549 RID: 21833
		private bool _isLeaderRoleMatch;

		// Token: 0x0400554A RID: 21834
		private ProductionPool _previewProductionPool;

		// Token: 0x0400554B RID: 21835
		private int _totalWeight;

		// Token: 0x0400554C RID: 21836
		private int _previewTotalWeight;

		// Token: 0x0400554D RID: 21837
		private readonly List<ItemDisplayData> _productItemList = new List<ItemDisplayData>();

		// Token: 0x0400554E RID: 21838
		private readonly Dictionary<ItemDisplayData, int> _selectedItemDict = new Dictionary<ItemDisplayData, int>();

		// Token: 0x0400554F RID: 21839
		private int _needProgress;

		// Token: 0x04005550 RID: 21840
		private readonly List<ItemDisplayData> _resourceItemList = new List<ItemDisplayData>();

		// Token: 0x04005551 RID: 21841
		private readonly List<ItemDisplayData> _materialItemList = new List<ItemDisplayData>();

		// Token: 0x04005552 RID: 21842
		private readonly Dictionary<sbyte, HashSet<short>> _productionPoolDic = new Dictionary<sbyte, HashSet<short>>();

		// Token: 0x04005553 RID: 21843
		private ViewCraftsman.EPanelMode _panelMode;

		// Token: 0x04005554 RID: 21844
		private UIParticle _addMaterailParticle;

		// Token: 0x04005555 RID: 21845
		private UIParticle _addResourceParticle;

		// Token: 0x04005556 RID: 21846
		private readonly UIParticlePlayHelper _particlePlayHelper = new UIParticlePlayHelper();

		// Token: 0x04005557 RID: 21847
		private ViewCraftsman.ECraftType _currentCraftType;

		// Token: 0x04005558 RID: 21848
		private readonly List<ViewCraftsman.ECraftType> _craftTypes = new List<ViewCraftsman.ECraftType>();

		// Token: 0x04005559 RID: 21849
		private bool _startLifeSkillCombat;

		// Token: 0x0400555A RID: 21850
		private int _artisanId;

		// Token: 0x0400555B RID: 21851
		private sbyte _craftsmanType;

		// Token: 0x0400555C RID: 21852
		private CraftManDisplayData _displayData;

		// Token: 0x0400555D RID: 21853
		private readonly int[] _shopManagerListCached = new int[7];

		// Token: 0x0400555E RID: 21854
		private int _selectingShopManagerIndex;

		// Token: 0x0400555F RID: 21855
		[TupleElementNames(new string[]
		{
			"self",
			"parent",
			"index"
		})]
		private readonly List<ValueTuple<Transform, Transform, int>> _focusList = new List<ValueTuple<Transform, Transform, int>>();

		// Token: 0x04005560 RID: 21856
		private Action _onExitFocus;

		// Token: 0x02001E68 RID: 7784
		[Flags]
		public enum EPanelMode
		{
			// Token: 0x0400C9A7 RID: 51623
			None = 0,
			// Token: 0x0400C9A8 RID: 51624
			Building = 1,
			// Token: 0x0400C9A9 RID: 51625
			Character = 2,
			// Token: 0x0400C9AA RID: 51626
			Intercept = 4,
			// Token: 0x0400C9AB RID: 51627
			TaiwuOrdered = 8,
			// Token: 0x0400C9AC RID: 51628
			NoOrder = 16
		}

		// Token: 0x02001E69 RID: 7785
		public enum ECraftType
		{
			// Token: 0x0400C9AE RID: 51630
			None,
			// Token: 0x0400C9AF RID: 51631
			Tea,
			// Token: 0x0400C9B0 RID: 51632
			Wine,
			// Token: 0x0400C9B1 RID: 51633
			Forging,
			// Token: 0x0400C9B2 RID: 51634
			Woodworking,
			// Token: 0x0400C9B3 RID: 51635
			Weaving,
			// Token: 0x0400C9B4 RID: 51636
			Medicine,
			// Token: 0x0400C9B5 RID: 51637
			Toxicology,
			// Token: 0x0400C9B6 RID: 51638
			Cooking,
			// Token: 0x0400C9B7 RID: 51639
			Jade
		}
	}
}
