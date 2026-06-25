using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Item;
using Game.Components.SortAndFilter.Item.Apply;
using Game.Views.Item;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Profession;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Make
{
	// Token: 0x02000961 RID: 2401
	public class ViewMake : UIBase
	{
		// Token: 0x17000D14 RID: 3348
		// (get) Token: 0x06007315 RID: 29461 RVA: 0x00357C2C File Offset: 0x00355E2C
		public CToggleGroup ToolSourceToggleGroup
		{
			get
			{
				return this.toolSourceToggleGroup;
			}
		}

		// Token: 0x17000D15 RID: 3349
		// (get) Token: 0x06007316 RID: 29462 RVA: 0x00357C34 File Offset: 0x00355E34
		private MakeTogKey _curTab
		{
			get
			{
				return (MakeTogKey)this.subPageToggleGroup.GetActiveIndex();
			}
		}

		// Token: 0x17000D16 RID: 3350
		// (get) Token: 0x06007317 RID: 29463 RVA: 0x00357C41 File Offset: 0x00355E41
		private ViewMake.ItemSourceTogKey CurToolTogKey
		{
			get
			{
				return (ViewMake.ItemSourceTogKey)this.toolSourceToggleGroup.GetActiveIndex();
			}
		}

		// Token: 0x17000D17 RID: 3351
		// (get) Token: 0x06007318 RID: 29464 RVA: 0x00357C4E File Offset: 0x00355E4E
		private ItemSourceType CurToolItemSource
		{
			get
			{
				return ViewMake.GetItemSourceType(this.CurToolTogKey);
			}
		}

		// Token: 0x17000D18 RID: 3352
		// (get) Token: 0x06007319 RID: 29465 RVA: 0x00357C5B File Offset: 0x00355E5B
		public List<ItemDisplayData> AllToolList
		{
			get
			{
				return this._allToolList;
			}
		}

		// Token: 0x17000D19 RID: 3353
		// (get) Token: 0x0600731A RID: 29466 RVA: 0x00357C63 File Offset: 0x00355E63
		public ItemDisplayData EmptyTool
		{
			get
			{
				return this._emptyTool;
			}
		}

		// Token: 0x17000D1A RID: 3354
		// (get) Token: 0x0600731B RID: 29467 RVA: 0x00357C6B File Offset: 0x00355E6B
		public ItemDisplayData SelectedTool
		{
			get
			{
				return this._selectedTool;
			}
		}

		// Token: 0x17000D1B RID: 3355
		// (get) Token: 0x0600731C RID: 29468 RVA: 0x00357C73 File Offset: 0x00355E73
		public int TaiwuCharId
		{
			get
			{
				return this._taiwuCharId;
			}
		}

		// Token: 0x17000D1C RID: 3356
		// (get) Token: 0x0600731D RID: 29469 RVA: 0x00357C7B File Offset: 0x00355E7B
		public BuildingModel BuildingModel
		{
			get
			{
				return this._buildingModel;
			}
		}

		// Token: 0x17000D1D RID: 3357
		// (get) Token: 0x0600731E RID: 29470 RVA: 0x00357C83 File Offset: 0x00355E83
		public sbyte CurLifeSkillType
		{
			get
			{
				return this._curLifeSkillType;
			}
		}

		// Token: 0x17000D1E RID: 3358
		// (get) Token: 0x0600731F RID: 29471 RVA: 0x00357C8B File Offset: 0x00355E8B
		public BuildingBlockKey BuildingBlockKey
		{
			get
			{
				return this._buildingBlockKey;
			}
		}

		// Token: 0x17000D1F RID: 3359
		// (get) Token: 0x06007320 RID: 29472 RVA: 0x00357C93 File Offset: 0x00355E93
		public BuildingBlockData BlockData
		{
			get
			{
				return this._blockData;
			}
		}

		// Token: 0x17000D20 RID: 3360
		// (get) Token: 0x06007321 RID: 29473 RVA: 0x00357C9B File Offset: 0x00355E9B
		private bool CanTransfer
		{
			get
			{
				return this._displayData.CanTransferItemToWarehouse;
			}
		}

		// Token: 0x06007322 RID: 29474 RVA: 0x00357CA8 File Offset: 0x00355EA8
		public override void OnInit(ArgumentBox argsBox)
		{
			this._buildingModel = SingletonObject.getInstance<BuildingModel>();
			this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			argsBox.Get<List<MakeTogKey>>("AllTab", out this._allTabList);
			bool flag = !argsBox.Get<MakeTogKey>("Tab", out this._initTab);
			if (flag)
			{
				this._initTab = this._allTabList.First<MakeTogKey>();
			}
			argsBox.Get<BuildingBlockKey>("BuildingBlockKey", out this._buildingBlockKey);
			argsBox.Get<BuildingBlockData>("BuildingBlockData", out this._blockData);
			argsBox.Get("LifeSkillType", out this._curLifeSkillType);
			foreach (KeyValuePair<int, int> data in this._spineMap)
			{
				bool flag2 = data.Key == (int)this._blockData.TemplateId;
				if (flag2)
				{
					this.spineArrys[data.Value].SetActive(true);
				}
				else
				{
					this.spineArrys[data.Value].SetActive(false);
				}
			}
			bool flag3 = this._blockData.TemplateId == 159;
			if (flag3)
			{
				this.toolListScroll.SetColumnDefinitions(this.GenerateColumnDefinitions(), new Action<RowItem>(this.GenerateRowTemplateContainers));
			}
			else
			{
				this.toolListScroll.SetColumnDefinitions(null, null);
			}
			this.textTitle.text = BuildingBlock.Instance[this._blockData.TemplateId].Name;
			this.subPageToggleGroup.Init(-1);
			for (MakeTogKey togKey = MakeTogKey.Make; togKey < MakeTogKey.Count; togKey++)
			{
				int index = togKey.ToInt();
				bool showTog = this._allTabList.Contains(togKey);
				CToggle tog = this.subPageToggleGroup.Get(index);
				tog.gameObject.SetActive(showTog);
				tog.GetComponentInChildren<TextMeshProUGUI>(true).text = ViewMake.GetMakeTogName(togKey);
				MakeSubPage subPage = this.subPages[index];
				bool flag4 = showTog;
				if (flag4)
				{
					subPage.Init(this);
				}
			}
			this.ClearToolList("");
			this.InitSwitchBuilding();
			bool inGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			if (inGuiding)
			{
				TaiwuEventDomainMethod.Call.TriggerListener(EventActionKey.DefValue.TutorialOpenViewMake, false);
				this.toolSourceToggleGroup.Set(ViewMake.ItemSourceTogKey.Warehouse.ToInt(), false);
			}
			this.NeedWaitData = true;
			this.RequestData();
		}

		// Token: 0x06007323 RID: 29475 RVA: 0x00357F28 File Offset: 0x00356128
		public void RequestData()
		{
			List<CToggle> allToggleList = this.subPageToggleGroup.GetAll();
			for (int i = 0; i < allToggleList.Count; i++)
			{
				CToggle toggle = allToggleList[i];
				bool activeSelf = toggle.gameObject.activeSelf;
				if (activeSelf)
				{
					this.subPages[i].RequestData();
				}
			}
			BuildingDomainMethod.AsyncCall.GetBuildingMakeDisplayData(this, this._buildingBlockKey, this._curLifeSkillType, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._displayData);
				bool flag = this._initTab != MakeTogKey.Invalid;
				if (flag)
				{
					this.subPageToggleGroup.SetWithoutNotify(this._initTab.ToInt());
					this.RefreshEncyclopedia(this._initTab.ToInt());
					CanvasGroup cg = this.subPages[(int)this._initTab].GetComponent<CanvasGroup>();
					bool flag2 = cg != null;
					if (flag2)
					{
						cg.alpha = 1f;
					}
					this._initTab = MakeTogKey.Invalid;
				}
				this.Refresh();
				this.Element.ShowAfterRefresh();
			});
		}

		// Token: 0x06007324 RID: 29476 RVA: 0x00357FA0 File Offset: 0x003561A0
		public static ArgumentBox GetMakeBuildingInfo(BuildingBlockData blockData, BuildingBlockKey buildingBlockKey, MakeTogKey initTab = MakeTogKey.Invalid)
		{
			short templateId = blockData.TemplateId;
			bool isBambooHouse = templateId == 257 || templateId == 258;
			BuildingBlockItem configData = BuildingBlock.Instance[blockData.TemplateId];
			List<MakeTogKey> allTabList = new List<MakeTogKey>();
			bool canMakeItem = configData.CanMakeItem;
			if (canMakeItem)
			{
				allTabList.Add(MakeTogKey.Make);
			}
			bool flag = configData.RequireLifeSkillType == 9;
			if (flag)
			{
				allTabList.Add(MakeTogKey.AddPoison);
				allTabList.Add(MakeTogKey.RemovePoison);
			}
			sbyte requireLifeSkillType = configData.RequireLifeSkillType;
			bool flag2 = requireLifeSkillType == 6 || requireLifeSkillType == 7 || requireLifeSkillType == 10 || requireLifeSkillType == 11 || isBambooHouse;
			if (flag2)
			{
				allTabList.Add(MakeTogKey.Refine);
			}
			requireLifeSkillType = configData.RequireLifeSkillType;
			bool flag3 = requireLifeSkillType == 6 || requireLifeSkillType == 7 || requireLifeSkillType == 10 || requireLifeSkillType == 11 || requireLifeSkillType == 8 || requireLifeSkillType == 9 || isBambooHouse;
			if (flag3)
			{
				allTabList.Add(MakeTogKey.Repair);
			}
			bool flag4 = configData.RequireLifeSkillType == 10;
			if (flag4)
			{
				allTabList.Add(MakeTogKey.Weave);
			}
			Location villageLocation = SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageBlock();
			bool flag5 = buildingBlockKey.AreaId == villageLocation.AreaId && buildingBlockKey.BlockId == villageLocation.BlockId;
			allTabList = allTabList.Distinct<MakeTogKey>().ToList<MakeTogKey>();
			sbyte lifeSkillType = isBambooHouse ? 7 : configData.RequireLifeSkillType;
			bool flag6 = initTab > MakeTogKey.Invalid && !allTabList.Contains(initTab);
			if (flag6)
			{
				initTab = allTabList.First<MakeTogKey>();
			}
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
			argumentBox.SetObject("Tab", initTab);
			argumentBox.SetObject("AllTab", allTabList);
			argumentBox.SetObject("BuildingBlockData", blockData);
			argumentBox.SetObject("BuildingBlockKey", buildingBlockKey);
			argumentBox.Set("LifeSkillType", lifeSkillType);
			return argumentBox;
		}

		// Token: 0x06007325 RID: 29477 RVA: 0x0035817C File Offset: 0x0035637C
		private void Awake()
		{
			this.subPageToggleGroup.OnActiveIndexChange += this.SubPageToggleGroupOnActiveIndexChange;
			this.buttonClose.ClearAndAddListener(new Action(this.QuickHide));
			this.toolListScroll.Init("ViewMakeTool", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnItemRender), new Action<ITradeableContent, RowItemLine>(this.OnItemClick), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Durability | ItemListScroll.EColumnType.ToolAttainment, null, null, null);
			this.toolListScroll.SetCustomBuildGroup(new Action(this.CustomBuildGroup), false);
			this.toolListScroll.SortAndFilterController.SetToggleVisible(EFilterLine.MainFilter.ToInt(), EMainFilterKeys.CraftTool.ToInt());
			this.toolListScroll.SortAndFilterController.SetToggleIsOnWithoutNotify(EFilterLine.MainFilter.ToInt(), EMainFilterKeys.CraftTool.ToInt());
			this.toolSourceToggleGroup.Init(-1);
			this.toolSourceToggleGroup.OnActiveIndexChange += this.ToolSourceToggleGroupOnActiveIndexChange;
		}

		// Token: 0x06007326 RID: 29478 RVA: 0x0035827B File Offset: 0x0035647B
		private void OnDestroy()
		{
			this.subPageToggleGroup.OnActiveIndexChange -= this.SubPageToggleGroupOnActiveIndexChange;
			this.toolSourceToggleGroup.OnActiveIndexChange -= this.ToolSourceToggleGroupOnActiveIndexChange;
		}

		// Token: 0x06007327 RID: 29479 RVA: 0x003582AE File Offset: 0x003564AE
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitions()
		{
			ColumnDefinition<ITradeableContent, ITradeableContent> columnDefinition = new ColumnDefinition<ITradeableContent, ITradeableContent>();
			columnDefinition.LayoutOption = new LayoutOption(200f, 1f, 622f, 1);
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Item.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => data);
			columnDefinition.SortId = 0;
			yield return columnDefinition;
			ColumnDefinition<ITradeableContent, string> columnDefinition2 = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition2.LayoutOption = new LayoutOption(120f, 1f, 120f, 1);
			columnDefinition2.TableHeadLabel = (() => LanguageKey.LK_Durability.Tr());
			columnDefinition2.CellDataGenerator = ((ITradeableContent data) => data.DurabilityChange.IsNullOrEmpty() ? CommonUtils.GetDurabilityString(data.Durability, data.MaxDurability) : data.DurabilityChange);
			columnDefinition2.SortId = 18;
			yield return columnDefinition2;
			ColumnDefinition<ITradeableContent, IconAndTextCellData> columnDefinition3 = new ColumnDefinition<ITradeableContent, IconAndTextCellData>();
			columnDefinition3.LayoutOption = new LayoutOption(100f, 1f, 120f, 1);
			columnDefinition3.TableHeadLabel = (() => SortItem.Instance[211].Names[0]);
			columnDefinition3.CellDataGenerator = delegate(ITradeableContent d)
			{
				CraftToolItem toolConfig = CraftTool.Instance[d.RealKey.TemplateId];
				LifeSkillTypeItem skillConfig = Config.LifeSkillType.Instance[toolConfig.RequiredLifeSkillTypes.Last<sbyte>()];
				return new IconAndTextCellData(skillConfig.Icon, string.Format("+{0}", toolConfig.AttainmentBonus), true, false, false, false);
			};
			columnDefinition3.SortId = 211;
			yield return columnDefinition3;
			yield break;
		}

		// Token: 0x06007328 RID: 29480 RVA: 0x003582BE File Offset: 0x003564BE
		private void GenerateRowTemplateContainers(RowItem rowItem)
		{
			ViewMake.<GenerateRowTemplateContainers>g__CreateCellContainers|73_0(rowItem.ContainerRoot, this.itemIconAndNameCellContainer);
			ViewMake.<GenerateRowTemplateContainers>g__CreateCellContainers|73_0(rowItem.ContainerRoot, this.cellContainerSingleText);
			ViewMake.<GenerateRowTemplateContainers>g__CreateCellContainers|73_0(rowItem.ContainerRoot, this.iconAndTextCellContainer);
		}

		// Token: 0x06007329 RID: 29481 RVA: 0x003582F9 File Offset: 0x003564F9
		private void SubPageToggleGroupOnActiveIndexChange(int newIndex, int oldIndex)
		{
			this.Refresh();
			this.RefreshEncyclopedia(newIndex);
		}

		// Token: 0x0600732A RID: 29482 RVA: 0x0035830C File Offset: 0x0035650C
		private void RefreshEncyclopedia(int index)
		{
			switch (index)
			{
			case 0:
				this.quickEncyclopedia.encyclopediaLink = EEncyclopediaTipLinkType.Craft;
				break;
			case 1:
				this.quickEncyclopedia.encyclopediaLink = EEncyclopediaTipLinkType.Repair;
				break;
			case 2:
				this.quickEncyclopedia.encyclopediaLink = EEncyclopediaTipLinkType.Envenom;
				break;
			case 3:
				this.quickEncyclopedia.encyclopediaLink = EEncyclopediaTipLinkType.Detoxify;
				break;
			case 4:
				this.quickEncyclopedia.encyclopediaLink = EEncyclopediaTipLinkType.Refinement;
				break;
			case 5:
				this.quickEncyclopedia.encyclopediaLink = EEncyclopediaTipLinkType.Alteration;
				break;
			}
		}

		// Token: 0x0600732B RID: 29483 RVA: 0x00358398 File Offset: 0x00356598
		private void Refresh()
		{
			if (this._emptyTool == null)
			{
				this._emptyTool = new ItemDisplayData(this._displayData.EmptyToolKey, 1);
			}
			this.RefreshAllToolList();
			this.ClearToolList("");
			this.RefreshSourceToggleInteractable(this.toolSourceToggleGroup);
			this.RefreshSwitchBuilding();
			int index = this.subPageToggleGroup.GetActiveIndex();
			for (int i = 0; i < this.subPages.Length; i++)
			{
				MakeSubPage subPage = this.subPages[i];
				bool isShow = index == i;
				subPage.gameObject.SetActive(isShow);
				bool flag = isShow;
				if (flag)
				{
					subPage.Refresh(this._displayData);
				}
			}
		}

		// Token: 0x0600732C RID: 29484 RVA: 0x00358444 File Offset: 0x00356644
		public override void QuickHide()
		{
			bool activeSelf = this.toggleGroupSwitch.gameObject.activeSelf;
			if (activeSelf)
			{
				this.ShowSwitchBuilding(false);
				AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			}
			else
			{
				bool activeSelf2 = this.focusRoot.gameObject.activeSelf;
				if (activeSelf2)
				{
					this.ExitFocusMode();
					AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
				}
				else
				{
					foreach (MakeSubPage subPage in this.subPages)
					{
						bool flag = subPage.gameObject.activeSelf && subPage.QuickHide();
						if (flag)
						{
							return;
						}
					}
					base.QuickHide();
				}
			}
		}

		// Token: 0x0600732D RID: 29485 RVA: 0x003584F7 File Offset: 0x003566F7
		public void RefreshSourceToggleInteractable(CToggleGroup toggleGroup)
		{
			ItemSourceToggleHelper.RefreshInteractableForBuilding(toggleGroup, this._displayData.CanTransferItemToWarehouse);
		}

		// Token: 0x0600732E RID: 29486 RVA: 0x0035850C File Offset: 0x0035670C
		public static ItemSourceType GetItemSourceType(ViewMake.ItemSourceTogKey togKey)
		{
			if (!true)
			{
			}
			ItemSourceType result;
			switch (togKey)
			{
			case ViewMake.ItemSourceTogKey.Inventory:
				result = ItemSourceType.Inventory;
				break;
			case ViewMake.ItemSourceTogKey.Warehouse:
				result = ItemSourceType.Warehouse;
				break;
			case ViewMake.ItemSourceTogKey.Treasury:
				result = ItemSourceType.Treasury;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600732F RID: 29487 RVA: 0x00358548 File Offset: 0x00356748
		public static string GetMakeTogName(MakeTogKey makeTogKey)
		{
			if (!true)
			{
			}
			string result;
			switch (makeTogKey)
			{
			case MakeTogKey.Make:
				result = LanguageKey.LK_Make_Item.Tr();
				break;
			case MakeTogKey.Repair:
				result = LanguageKey.LK_Repair_Item.Tr();
				break;
			case MakeTogKey.AddPoison:
				result = LanguageKey.LK_Poison_Item.Tr();
				break;
			case MakeTogKey.RemovePoison:
				result = LanguageKey.LK_Remove_Poison.Tr();
				break;
			case MakeTogKey.Refine:
				result = LanguageKey.LK_Strengthen_Item.Tr();
				break;
			case MakeTogKey.Weave:
				result = LanguageKey.LK_Weave_Item.Tr();
				break;
			default:
				throw new ArgumentOutOfRangeException("makeTogKey", makeTogKey, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007330 RID: 29488 RVA: 0x003585E0 File Offset: 0x003567E0
		public List<ItemDisplayData> GetItemList(ItemSourceType itemSourceType)
		{
			if (!true)
			{
			}
			List<ItemDisplayData> result;
			switch (itemSourceType)
			{
			case ItemSourceType.Equipment:
				result = (this._displayData.EquippedItemList ?? new List<ItemDisplayData>());
				break;
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
				throw new ArgumentOutOfRangeException("itemSourceType", itemSourceType, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007331 RID: 29489 RVA: 0x0035867C File Offset: 0x0035687C
		public static bool IsEmptyTool(ITradeableContent data)
		{
			return ViewMake.IsEmptyTool(data.RealKey);
		}

		// Token: 0x06007332 RID: 29490 RVA: 0x00358689 File Offset: 0x00356889
		public static bool IsEmptyTool(ItemKey itemKey)
		{
			return ItemTemplateHelper.IsEmptyTool(itemKey.ItemType, itemKey.TemplateId);
		}

		// Token: 0x06007333 RID: 29491 RVA: 0x0035869C File Offset: 0x0035689C
		public static short GetToolDurabilityCost(ITradeableContent tool, sbyte targetGrade)
		{
			bool flag = tool == null || !tool.RealKey.IsValid() || ViewMake.IsEmptyTool(tool) || targetGrade < 0;
			short result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				result = CraftTool.Instance[tool.RealKey.TemplateId].DurabilityCost[(int)targetGrade];
			}
			return result;
		}

		// Token: 0x06007334 RID: 29492 RVA: 0x003586F4 File Offset: 0x003568F4
		private static short GetToolDurabilityCost(ITradeableContent tool, List<sbyte> targetGradeList)
		{
			short cost = 0;
			foreach (sbyte targetGrade in targetGradeList)
			{
				cost += ViewMake.GetToolDurabilityCost(tool, targetGrade);
			}
			return cost;
		}

		// Token: 0x06007335 RID: 29493 RVA: 0x00358754 File Offset: 0x00356954
		public static short GetRefineToolDurabilityCostExpectLast(ITradeableContent tool, List<sbyte> targetGradeList)
		{
			short cost = 0;
			for (int i = 0; i < targetGradeList.Count - 1; i++)
			{
				sbyte targetGrade = targetGradeList[i];
				cost += ViewMake.GetToolDurabilityCost(tool, targetGrade);
			}
			return cost;
		}

		// Token: 0x06007336 RID: 29494 RVA: 0x00358798 File Offset: 0x00356998
		public static int GetEmptyToolAttainmentBonus(sbyte lifeSkillType)
		{
			ProfessionModel professionModel = SingletonObject.getInstance<ProfessionModel>();
			bool skillTypeIsMeet = lifeSkillType == 7 || lifeSkillType == 6 || lifeSkillType == 11 || lifeSkillType == 10;
			bool flag = skillTypeIsMeet && professionModel.IsProfessionalSkillUnlockedAndEquipped(8);
			int result;
			if (flag)
			{
				ProfessionData professionData = professionModel.GetProfessionData(2);
				int bonus = professionData.GetSeniorityEmptyToolAttainmentBonus();
				result = bonus;
			}
			else
			{
				result = ProfessionData.SeniorityToEmptyToolAttainmentBonus(0);
			}
			return result;
		}

		// Token: 0x06007337 RID: 29495 RVA: 0x003587F8 File Offset: 0x003569F8
		public static short GetToolAttainment(short templateId, sbyte lifeSkillType)
		{
			bool flag = templateId < 0;
			short result2;
			if (flag)
			{
				result2 = 0;
			}
			else
			{
				CraftToolItem toolConfig = CraftTool.Instance[templateId];
				bool skillTypeIsMeet = lifeSkillType == 7 || lifeSkillType == 6 || lifeSkillType == 11 || lifeSkillType == 10;
				bool flag2 = !skillTypeIsMeet;
				if (flag2)
				{
					result2 = toolConfig.AttainmentBonus;
				}
				else
				{
					ProfessionModel professionModel = SingletonObject.getInstance<ProfessionModel>();
					bool equipped = professionModel.IsProfessionalSkillUnlockedAndEquipped(9);
					short attainment = toolConfig.AttainmentBonus;
					bool flag3 = equipped;
					if (flag3)
					{
						ProfessionData professionData = professionModel.GetProfessionData(2);
						int result = (int)attainment * (100 + professionData.GetSeniorityAttainmentBonus()) / 100;
						attainment = (short)result;
					}
					result2 = attainment;
				}
			}
			return result2;
		}

		// Token: 0x06007338 RID: 29496 RVA: 0x00358894 File Offset: 0x00356A94
		public static short GetFinalAttainment(short toolTemplateId, short skillAttainment, sbyte lifeSkillType)
		{
			short toolAttainment = ViewMake.GetToolAttainment(toolTemplateId, skillAttainment, lifeSkillType);
			return Convert.ToInt16((int)(skillAttainment + toolAttainment));
		}

		// Token: 0x06007339 RID: 29497 RVA: 0x003588BC File Offset: 0x00356ABC
		public static short GetToolAttainment(short toolTemplateId, short skillAttainment, sbyte lifeSkillType)
		{
			bool flag = ItemTemplateHelper.IsEmptyTool(6, toolTemplateId);
			short result;
			if (flag)
			{
				int bonus = ViewMake.GetEmptyToolAttainmentBonus(lifeSkillType);
				int toolAttainment = (int)skillAttainment * bonus / 100;
				result = (short)toolAttainment;
			}
			else
			{
				short toolAttainment2 = ViewMake.GetToolAttainment(toolTemplateId, lifeSkillType);
				result = toolAttainment2;
			}
			return result;
		}

		// Token: 0x0600733A RID: 29498 RVA: 0x003588FC File Offset: 0x00356AFC
		public void EnterFocusMode(Transform target, Action onExit = null)
		{
			bool activeSelf = this.focusRoot.gameObject.activeSelf;
			if (!activeSelf)
			{
				this._focusTargetOriginParent = target.parent;
				this._focusTarget = target;
				this.focusRoot.gameObject.SetActive(true);
				this._focusTarget.gameObject.SetActive(true);
				this._focusTarget.SetParent(this.focusRoot.transform);
				this._onExitFocus = onExit;
			}
		}

		// Token: 0x0600733B RID: 29499 RVA: 0x00358978 File Offset: 0x00356B78
		public void ExitFocusMode()
		{
			bool flag = !this.focusRoot.gameObject.activeSelf;
			if (!flag)
			{
				this.focusRoot.gameObject.SetActive(false);
				this._focusTarget.gameObject.SetActive(false);
				this._focusTarget.SetParent(this._focusTargetOriginParent);
				Action onExitFocus = this._onExitFocus;
				if (onExitFocus != null)
				{
					onExitFocus();
				}
			}
		}

		// Token: 0x0600733C RID: 29500 RVA: 0x003589E7 File Offset: 0x00356BE7
		public void ShowToolPanel(bool isShow)
		{
			this.toolPanel.gameObject.SetActive(isShow);
		}

		// Token: 0x0600733D RID: 29501 RVA: 0x003589FC File Offset: 0x00356BFC
		public void RefreshToolList(int needAttainment, List<sbyte> lifeSkillTypeList, List<List<sbyte>> targetGradeList, ItemDisplayData selectedTool, Action<ItemDisplayData> onSelectTool, bool isAutoSelect, int actionCount = 1)
		{
			this._toolNeedAttainment = needAttainment;
			this._toolLifeSkillTypeList = lifeSkillTypeList;
			this._toolTargetGradeList = targetGradeList;
			this._selectedTool = selectedTool;
			this._onSelectTool = onSelectTool;
			this._toolActionCount = actionCount;
			this._toolList.Clear();
			bool flag = this._toolLifeSkillTypeList.Count < 0 || this._toolNeedAttainment < 0;
			if (flag)
			{
				this.toolListScroll.SetItemList(this._toolList);
			}
			else
			{
				bool flag2;
				bool flag3;
				bool flag4;
				this._emptyTool.Interactable = this.CheckTool(this._emptyTool, out flag2, out flag3, out flag4);
				this._toolList.Add(this._emptyTool);
				foreach (ItemDisplayData itemData in this._allToolList)
				{
					bool flag5 = itemData.RealKey.ItemType != 6;
					if (!flag5)
					{
						CraftToolItem config = CraftTool.Instance[itemData.RealKey.TemplateId];
						bool flag6 = !config.RequiredLifeSkillTypes.Intersect(this._toolLifeSkillTypeList).Any<sbyte>();
						if (!flag6)
						{
							bool flag7 = itemData.ItemSourceTypeEnum == this.CurToolItemSource;
							if (flag7)
							{
								this._toolList.Add(itemData);
							}
							List<sbyte> targetGrade = this.GetTargetGradeList(config);
							itemData.Interactable = this.CheckTool(itemData, out flag4, out flag3, out flag2);
							string curText = itemData.Durability.ToString().SetColor("pinkyellow");
							int durabilityCost = (int)ViewMake.GetToolDurabilityCost(itemData, targetGrade) * this._toolActionCount;
							string costText = string.Format("-{0}", durabilityCost).SetColor("brightred");
							string maxText = itemData.MaxDurability.ToString().SetColor("pinkyellow");
							itemData.DurabilityChange = curText + costText + "/" + maxText;
						}
					}
				}
				this.toolListScroll.SetItemList(this._toolList);
				if (isAutoSelect)
				{
					this.AutoSelectTool();
				}
			}
		}

		// Token: 0x0600733E RID: 29502 RVA: 0x00358C34 File Offset: 0x00356E34
		public void AutoSelectTool()
		{
			this._selectedTool = this.GetAutoSelectTool();
			this._onSelectTool(this._selectedTool);
		}

		// Token: 0x0600733F RID: 29503 RVA: 0x00358C58 File Offset: 0x00356E58
		public ItemDisplayData GetAutoSelectTool()
		{
			bool interactable = this._emptyTool.Interactable;
			if (interactable)
			{
				bool isMeetAttainment;
				bool flag;
				bool flag2;
				this.CheckTool(this._emptyTool, out isMeetAttainment, out flag, out flag2);
				bool flag3 = isMeetAttainment;
				if (flag3)
				{
					return this._emptyTool;
				}
			}
			IOrderedEnumerable<ItemDisplayData> orderList = (from d in this._allToolList
			where d.Interactable
			orderby ViewMake.GetToolDurabilityCost(d, this._toolTargetGradeList.FirstOrDefault<List<sbyte>>()) == 0 descending
			select d).ThenBy(delegate(ItemDisplayData d)
			{
				sbyte grade = d.Grade;
				short onceCost = ViewMake.GetToolDurabilityCost(d, this._toolTargetGradeList.FirstOrDefault<List<sbyte>>());
				int gradeScore = GlobalConfig.Instance.MakeAutoSelectToolGradeScore[(int)grade] * (int)onceCost;
				int totalCost = (int)onceCost * this._toolActionCount;
				int destroyScore = (totalCost >= (int)d.Durability) ? GlobalConfig.Instance.MakeAutoSelectToolDestroyScore[(int)grade] : 0;
				return gradeScore + destroyScore;
			}).ThenByDescending((ItemDisplayData d) => d.Grade);
			return orderList.FirstOrDefault<ItemDisplayData>();
		}

		// Token: 0x06007340 RID: 29504 RVA: 0x00358D18 File Offset: 0x00356F18
		public void SetRightMask(bool isShow)
		{
			this.rightMask.SetActive(isShow);
		}

		// Token: 0x06007341 RID: 29505 RVA: 0x00358D28 File Offset: 0x00356F28
		private List<sbyte> GetTargetGradeList(CraftToolItem config)
		{
			List<sbyte> targetGradeList = this._toolTargetGradeList.FirstOrDefault<List<sbyte>>();
			bool flag = this._curTab == MakeTogKey.Refine;
			if (flag)
			{
				sbyte requiredLifeSkillType = config.RequiredLifeSkillTypes.FirstOrDefault<sbyte>();
				int index = GameData.Domains.Character.LifeSkillType.RefineTypes.IndexOf(requiredLifeSkillType);
				bool flag2 = index >= 0 && index < this._toolTargetGradeList.Count;
				if (flag2)
				{
					targetGradeList = this._toolTargetGradeList[index];
				}
			}
			return targetGradeList;
		}

		// Token: 0x06007342 RID: 29506 RVA: 0x00358D98 File Offset: 0x00356F98
		public void RerenderToolList(ItemDisplayData selectedTool)
		{
			this._selectedTool = selectedTool;
			this.toolListScroll.ReRender();
		}

		// Token: 0x06007343 RID: 29507 RVA: 0x00358DB0 File Offset: 0x00356FB0
		public void ClearToolList(string emptyContent = "")
		{
			this._toolNeedAttainment = -1;
			this._toolTargetGradeList.Clear();
			this._toolLifeSkillTypeList.Clear();
			this._toolActionCount = 1;
			this._selectedTool = null;
			this.toolListScroll.SetEmptyContent(emptyContent);
			this._toolList.Clear();
			this.toolListScroll.SetItemList(this._toolList);
		}

		// Token: 0x06007344 RID: 29508 RVA: 0x00358E18 File Offset: 0x00357018
		private bool CheckTool(ITradeableContent itemData, out bool isMeetAttainment, out bool isMeetDurability, out bool lifeSkillTypeMatch)
		{
			short charAttainment = this._displayData.LifeSkillAttainments.Get((int)this._curLifeSkillType);
			short finalAttainment = ViewMake.GetFinalAttainment(itemData.RealKey.TemplateId, charAttainment, this._curLifeSkillType);
			isMeetAttainment = ((int)finalAttainment >= this._toolNeedAttainment);
			CraftToolItem toolConfig = CraftTool.Instance[itemData.RealKey.TemplateId];
			List<sbyte> targetGradeList = this.GetTargetGradeList(toolConfig);
			short oneCost = ViewMake.GetToolDurabilityCost(itemData, targetGradeList);
			lifeSkillTypeMatch = true;
			bool flag = itemData.RealKey.Equals(this._displayData.EmptyToolKey);
			if (flag)
			{
				isMeetDurability = true;
			}
			else
			{
				int curDurability = itemData.Durability;
				bool flag2 = this._curTab == MakeTogKey.Refine;
				if (flag2)
				{
					short costExpectLast = ViewMake.GetRefineToolDurabilityCostExpectLast(itemData, targetGradeList);
					isMeetDurability = (curDurability - (int)costExpectLast >= 1);
					List<sbyte> canSelectLifeSkillType = new List<sbyte>();
					for (int i = 0; i < this._toolTargetGradeList.Count; i++)
					{
						bool flag3 = this._toolTargetGradeList[i].Count > 0;
						if (flag3)
						{
							canSelectLifeSkillType.Add(GameData.Domains.Character.LifeSkillType.RefineTypes[i]);
						}
					}
					lifeSkillTypeMatch = toolConfig.RequiredLifeSkillTypes.Intersect(canSelectLifeSkillType).Any<sbyte>();
				}
				else
				{
					short totalCost = Convert.ToInt16((int)oneCost * this._toolActionCount);
					isMeetDurability = (curDurability >= (int)totalCost || curDurability + (int)oneCost > (int)totalCost);
				}
			}
			return isMeetDurability & isMeetAttainment & lifeSkillTypeMatch;
		}

		// Token: 0x06007345 RID: 29509 RVA: 0x00358F90 File Offset: 0x00357190
		private void CustomBuildGroup()
		{
			string title = LanguageKey.LK_Make_Tool_Group_CanUse.Tr();
			this._availableToolList.Clear();
			this._availableToolList.AddRange(from d in this.toolListScroll.FilteredData
			where d.Interactable
			select d);
			this.toolListScroll.AddGroup(0, title, this._availableToolList, null, true);
			string title2 = LanguageKey.LK_Make_Tool_Group_CanNotUse.Tr();
			this._notAvailableToolList.Clear();
			this._notAvailableToolList.AddRange(from d in this.toolListScroll.FilteredData
			where !d.Interactable
			select d);
			this.toolListScroll.AddGroup(1, title2, this._notAvailableToolList, null, true);
		}

		// Token: 0x06007346 RID: 29510 RVA: 0x00359074 File Offset: 0x00357274
		private void OnItemRender(ITradeableContent content, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(content);
			rowItemLine.Set(rowItemMain, true);
			bool isSelected = this._selectedTool == content;
			rowItemLine.SetSelected(isSelected);
			bool isMeetAttainment;
			bool isMeetDurability;
			bool lifeSkillTypeMatch;
			bool interactable = this.CheckTool(content, out isMeetAttainment, out isMeetDurability, out lifeSkillTypeMatch);
			rowItemLine.SetInteractable(interactable, true);
			string lockText = string.Empty;
			bool flag = !interactable;
			if (flag)
			{
				bool flag2 = !isMeetAttainment;
				if (flag2)
				{
					lockText = LanguageKey.LK_Item_Operation_AttainmentNotMeet.Tr();
				}
				else
				{
					bool flag3 = !isMeetDurability;
					if (flag3)
					{
						lockText = LanguageKey.LK_Tool_Durability_Not_Enough.Tr();
					}
					else
					{
						bool flag4 = !lifeSkillTypeMatch;
						if (flag4)
						{
							lockText = LanguageKey.LK_Tool_LifeSkill_Not_Enough.Tr();
						}
					}
				}
				rowItemMain.SetInteractionStateLockText(lockText);
			}
			else
			{
				rowItemMain.HideInteractionState();
			}
			rowItemLine.SetDisabled(!interactable);
		}

		// Token: 0x06007347 RID: 29511 RVA: 0x00359140 File Offset: 0x00357340
		private void OnItemClick(ITradeableContent content, RowItemLine rowItemLine)
		{
			bool flag = this._selectedTool == content;
			if (flag)
			{
				this._selectedTool = null;
			}
			else
			{
				this._selectedTool = (content as ItemDisplayData);
			}
			this._onSelectTool(this._selectedTool);
			this.toolListScroll.ReRender();
		}

		// Token: 0x06007348 RID: 29512 RVA: 0x00359192 File Offset: 0x00357392
		private void ToolSourceToggleGroupOnActiveIndexChange(int newIndex, int oldIndex)
		{
			this.RefreshToolList(this._toolNeedAttainment, this._toolLifeSkillTypeList, this._toolTargetGradeList, this._selectedTool, this._onSelectTool, false, this._toolActionCount);
		}

		// Token: 0x06007349 RID: 29513 RVA: 0x003591C4 File Offset: 0x003573C4
		private void RefreshAllToolList()
		{
			this._allToolList.Clear();
			bool canTransferItemToWarehouse = this._displayData.CanTransferItemToWarehouse;
			if (canTransferItemToWarehouse)
			{
				this.<RefreshAllToolList>g__Add|106_0(this._displayData.InventoryItemList);
			}
			this.<RefreshAllToolList>g__Add|106_0(this._displayData.WarehouseItemList);
			this.<RefreshAllToolList>g__Add|106_0(this._displayData.TreasuryItemList);
		}

		// Token: 0x0600734A RID: 29514 RVA: 0x00359228 File Offset: 0x00357428
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

		// Token: 0x0600734B RID: 29515 RVA: 0x003592C0 File Offset: 0x003574C0
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

		// Token: 0x0600734C RID: 29516 RVA: 0x00359358 File Offset: 0x00357558
		private void ToggleGroupSwitchOnActiveIndexChange(int nweIndex, int oldIndex)
		{
			short buildingId = MakeBuildingItem.BuildingIdList[nweIndex];
			BuildingBlockData blockData = this._displayData.BlockList.Find((BuildingBlockData b) => b.TemplateId == buildingId);
			BuildingBlockKey blockKey = new BuildingBlockKey(this._buildingBlockKey.AreaId, this._buildingBlockKey.BlockId, blockData.BlockIndex);
			MakeTogKey initTab = (MakeTogKey)this.subPages.ToList<MakeSubPage>().FindIndex((MakeSubPage p) => p.gameObject.activeSelf);
			ArgumentBox args = ViewMake.GetMakeBuildingInfo(blockData, blockKey, initTab);
			this.OnInit(args);
			this.ExitFocusMode();
		}

		// Token: 0x0600734D RID: 29517 RVA: 0x00359407 File Offset: 0x00357607
		private void ToggleSwitchOnValueChanged(bool isOn)
		{
			this.ShowSwitchBuilding(isOn);
		}

		// Token: 0x0600734E RID: 29518 RVA: 0x00359414 File Offset: 0x00357614
		private void ShowSwitchBuilding(bool isShow)
		{
			this.toggleGroupSwitch.gameObject.SetActive(isShow);
			if (isShow)
			{
				this.EnterFocusMode(this.rootSwitch.transform, delegate
				{
					this.toggleSwitch.SetIsOnWithoutNotify(false);
					this.rootSwitch.gameObject.SetActive(true);
				});
				int index = MakeBuildingItem.BuildingIdList.IndexOf(this._blockData.TemplateId);
				this.toggleGroupSwitch.SetWithoutNotify(index);
			}
			else
			{
				this.ExitFocusMode();
			}
		}

		// Token: 0x0600734F RID: 29519 RVA: 0x00359488 File Offset: 0x00357688
		public short GetAttainmentByBuildingEffect(sbyte lifeSkillType, short requiredAttainment)
		{
			int attainmentEffect = (lifeSkillType == this.CurLifeSkillType) ? this._displayData.BuildingAttainmentEffect : 0;
			return GameData.Domains.Building.SharedMethods.GetRequiredLifeSkillAttainmentByBuildingEffect((int)requiredAttainment, attainmentEffect);
		}

		// Token: 0x06007350 RID: 29520 RVA: 0x003594BC File Offset: 0x003576BC
		public short GetPoisonRequiredAttainment(short templateId, bool condense)
		{
			short requiredAttainment = ItemTemplateHelper.GetPoisonRequiredAttainment(8, templateId);
			if (condense)
			{
				int bonus = GlobalConfig.Instance.CondensePoisonRequiredAttainmentBonus;
				requiredAttainment = Convert.ToInt16((int)requiredAttainment * (100 + bonus) / 100);
			}
			return this.GetAttainmentByBuildingEffect(8, requiredAttainment);
		}

		// Token: 0x06007353 RID: 29523 RVA: 0x00359678 File Offset: 0x00357878
		[CompilerGenerated]
		internal static void <GenerateRowTemplateContainers>g__CreateCellContainers|73_0(Transform containerRoot, RowCellContainer prefab)
		{
			RowCellContainer container = Object.Instantiate<RowCellContainer>(prefab, containerRoot);
			container.gameObject.SetActive(true);
		}

		// Token: 0x06007356 RID: 29526 RVA: 0x00359718 File Offset: 0x00357918
		[CompilerGenerated]
		private void <RefreshAllToolList>g__Add|106_0(List<ItemDisplayData> list)
		{
			bool flag = list == null;
			if (!flag)
			{
				foreach (ItemDisplayData itemData in list)
				{
					sbyte itemType = itemData.RealKey.ItemType;
					sbyte b = itemType;
					if (b == 6)
					{
						CraftToolItem config = CraftTool.Instance[itemData.RealKey.TemplateId];
						bool flag2 = this._curTab == MakeTogKey.Refine;
						if (flag2)
						{
							bool flag3 = config.RequiredLifeSkillTypes.Intersect(GameData.Domains.Character.LifeSkillType.RefineTypes).Any<sbyte>();
							if (flag3)
							{
								this._allToolList.Add(itemData);
							}
						}
						else
						{
							bool flag4 = config.RequiredLifeSkillTypes.Contains(this._curLifeSkillType);
							if (flag4)
							{
								this._allToolList.Add(itemData);
							}
						}
					}
				}
			}
		}

		// Token: 0x04005561 RID: 21857
		[SerializeField]
		private MakeSubPage[] subPages;

		// Token: 0x04005562 RID: 21858
		[SerializeField]
		private CToggleGroup subPageToggleGroup;

		// Token: 0x04005563 RID: 21859
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x04005564 RID: 21860
		[SerializeField]
		private CButton buttonClose;

		// Token: 0x04005565 RID: 21861
		[SerializeField]
		private GameObject focusRoot;

		// Token: 0x04005566 RID: 21862
		[SerializeField]
		private GameObject rightMask;

		// Token: 0x04005567 RID: 21863
		[Header("工具列表")]
		[SerializeField]
		private CToggleGroup toolSourceToggleGroup;

		// Token: 0x04005568 RID: 21864
		[SerializeField]
		private ItemListScroll toolListScroll;

		// Token: 0x04005569 RID: 21865
		[SerializeField]
		private GameObject toolPanel;

		// Token: 0x0400556A RID: 21866
		[Header("切换建筑")]
		[SerializeField]
		private GameObject rootSwitch;

		// Token: 0x0400556B RID: 21867
		[SerializeField]
		private CToggle toggleSwitch;

		// Token: 0x0400556C RID: 21868
		[SerializeField]
		private CToggleGroup toggleGroupSwitch;

		// Token: 0x0400556D RID: 21869
		[Header("百晓册入口控件")]
		[SerializeField]
		private QuickEncyclopedia quickEncyclopedia;

		// Token: 0x0400556E RID: 21870
		[Header("Spine")]
		[SerializeField]
		private GameObject[] spineArrys;

		// Token: 0x0400556F RID: 21871
		[SerializeField]
		private RowCellContainer itemIconAndNameCellContainer;

		// Token: 0x04005570 RID: 21872
		[SerializeField]
		private RowCellContainer cellContainerSingleText;

		// Token: 0x04005571 RID: 21873
		[SerializeField]
		private RowCellContainer iconAndTextCellContainer;

		// Token: 0x04005572 RID: 21874
		private BuildingMakeDisplayData _displayData;

		// Token: 0x04005573 RID: 21875
		private int _taiwuCharId;

		// Token: 0x04005574 RID: 21876
		private BuildingModel _buildingModel;

		// Token: 0x04005575 RID: 21877
		private List<MakeTogKey> _allTabList;

		// Token: 0x04005576 RID: 21878
		private sbyte _curLifeSkillType;

		// Token: 0x04005577 RID: 21879
		private MakeTogKey _initTab;

		// Token: 0x04005578 RID: 21880
		private BuildingBlockKey _buildingBlockKey;

		// Token: 0x04005579 RID: 21881
		private BuildingBlockData _blockData;

		// Token: 0x0400557A RID: 21882
		private Transform _focusTargetOriginParent;

		// Token: 0x0400557B RID: 21883
		private Transform _focusTarget;

		// Token: 0x0400557C RID: 21884
		private Action _onExitFocus;

		// Token: 0x0400557D RID: 21885
		private readonly List<ITradeableContent> _toolList = new List<ITradeableContent>();

		// Token: 0x0400557E RID: 21886
		private readonly List<ITradeableContent> _availableToolList = new List<ITradeableContent>();

		// Token: 0x0400557F RID: 21887
		private readonly List<ITradeableContent> _notAvailableToolList = new List<ITradeableContent>();

		// Token: 0x04005580 RID: 21888
		private Action<ItemDisplayData> _onSelectTool;

		// Token: 0x04005581 RID: 21889
		private ItemDisplayData _selectedTool;

		// Token: 0x04005582 RID: 21890
		private ItemDisplayData _emptyTool;

		// Token: 0x04005583 RID: 21891
		private int _toolNeedAttainment = -1;

		// Token: 0x04005584 RID: 21892
		private List<List<sbyte>> _toolTargetGradeList = new List<List<sbyte>>();

		// Token: 0x04005585 RID: 21893
		private List<sbyte> _toolLifeSkillTypeList = new List<sbyte>();

		// Token: 0x04005586 RID: 21894
		private int _toolActionCount = 1;

		// Token: 0x04005587 RID: 21895
		private readonly List<ItemDisplayData> _allToolList = new List<ItemDisplayData>();

		// Token: 0x04005588 RID: 21896
		private Dictionary<int, int> _spineMap = new Dictionary<int, int>
		{
			{
				129,
				0
			},
			{
				139,
				1
			},
			{
				179,
				2
			},
			{
				203,
				3
			},
			{
				169,
				4
			},
			{
				149,
				5
			},
			{
				159,
				6
			}
		};

		// Token: 0x02001E73 RID: 7795
		public enum ItemSourceTogKey
		{
			// Token: 0x0400C9D2 RID: 51666
			Inventory,
			// Token: 0x0400C9D3 RID: 51667
			Warehouse,
			// Token: 0x0400C9D4 RID: 51668
			Treasury
		}
	}
}
