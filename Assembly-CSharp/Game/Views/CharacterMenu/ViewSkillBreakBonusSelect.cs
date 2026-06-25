using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.Common;
using Game.Components.Item;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.BonusSelect;
using Game.Views.Item;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.CombatSkill;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B98 RID: 2968
	public class ViewSkillBreakBonusSelect : UIBase
	{
		// Token: 0x17000FD6 RID: 4054
		// (get) Token: 0x06009267 RID: 37479 RVA: 0x00443184 File Offset: 0x00441384
		private ItemSourceType SelectedFilterItemSourceType
		{
			get
			{
				return this._itemSourceTypeArray.GetOrDefault(this.SelectedItemSourceTypeIndex, ItemSourceType.Inventory);
			}
		}

		// Token: 0x17000FD7 RID: 4055
		// (get) Token: 0x06009268 RID: 37480 RVA: 0x00443198 File Offset: 0x00441398
		private int SelectedItemSourceTypeIndex
		{
			get
			{
				return this.sourceFilterGroup.GetActiveIndex();
			}
		}

		// Token: 0x17000FD8 RID: 4056
		// (get) Token: 0x06009269 RID: 37481 RVA: 0x004431A8 File Offset: 0x004413A8
		private SkillBreakPlateBonus? DisplayingBonus
		{
			get
			{
				return new SkillBreakPlateBonus?(this._hoverGeneratedBonus ?? (this._generatedBonus ?? this._currentBonus));
			}
		}

		// Token: 0x17000FD9 RID: 4057
		// (get) Token: 0x0600926A RID: 37482 RVA: 0x004431F2 File Offset: 0x004413F2
		private ViewSkillBreakBonusSelect.EColumnType ColumnTypeFlags
		{
			get
			{
				return (ViewSkillBreakBonusSelect.EColumnType)2147483647;
			}
		}

		// Token: 0x0600926B RID: 37483 RVA: 0x004431FC File Offset: 0x004413FC
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get<Action<ItemKey, ItemSourceType>>("OnConfirmItem", out this._onConfirmItem);
			argsBox.Get<Action<int>>("OnConfirmExp", out this._onConfirmExp);
			argsBox.Get<Action<int, ushort>>("OnConfirmRelation", out this._onConfirmRelation);
			argsBox.Get<Action>("OnConfirmClear", out this._onConfirmClear);
			argsBox.Get("CurrentExp", out this._currentExp);
			argsBox.Get<LifeSkillShorts>("LifeSkillAttainments", out this._lifeSkillAttainments);
			argsBox.Get<SkillBreakPlateBonus>("CurrentBonus", out this._currentBonus);
			argsBox.Get("SkillId", out this._skillId);
			List<int> powerList;
			argsBox.Get<List<int>>("PossiblePowerList", out powerList);
			this._possiblePowerList.Clear();
			this._possiblePowerList.AddRange(powerList);
			this._generatedBonus = null;
			this._selectedItemKey = ItemKey.Invalid;
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
		}

		// Token: 0x0600926C RID: 37484 RVA: 0x00443308 File Offset: 0x00441508
		private void Awake()
		{
			this.rowTemplate.gameObject.SetActive(false);
			this.rowTemplateForChar.gameObject.SetActive(false);
			this.singleTextCellContainer.gameObject.SetActive(false);
			this.itemIconAndNameCellContainer.gameObject.SetActive(false);
			this.avatarAndNameCellContainer.gameObject.SetActive(false);
			this.expCellContainer.gameObject.SetActive(false);
			this.avatarSingleTextCellContainer.gameObject.SetActive(false);
			this.InitItemSourceFilter();
			this.InitSortAndFilter();
		}

		// Token: 0x0600926D RID: 37485 RVA: 0x004433A4 File Offset: 0x004415A4
		private void InitSortAndFilter()
		{
			bool flag = this.sortAndFilter == null;
			if (!flag)
			{
				this._sortAndFilterController = new BonusSelectSortAndFilterController(this.sortAndFilter);
				this._sortAndFilterController.Init(new Action(this.OnSortAndFilterChanged), "SkillBreakBonusSelect");
				this._sortAndFilterController.SetContext(this._skillId);
				this._sortAndFilterController.SetToggleVisible(new ToggleIdIndex(0, ToggleKey.AllKey), false);
				this.sortAndFilter.SetUpToggleGroupHotKey(this.Element, 0);
			}
		}

		// Token: 0x0600926E RID: 37486 RVA: 0x00443430 File Offset: 0x00441630
		private void OnSortAndFilterChanged()
		{
			ViewSkillBreakBonusSelect.ESelectorMode newMode = this.GetSelectedModeFromSelectedType();
			bool isModeChanged = this._selectorMode != newMode;
			this._selectorMode = newMode;
			this.RefreshSourceFilterEnable();
			this.RefreshList(isModeChanged);
			this.sortGo.SetActive(this.sortAndFilter.Sections.Count > 0);
			this.filterGo.SetActive(this.sortAndFilter.Sections.Count > 0);
		}

		// Token: 0x0600926F RID: 37487 RVA: 0x004434A6 File Offset: 0x004416A6
		private void InitItemSourceFilter()
		{
			this.sourceFilterGroup.Init(-1);
			ToggleGroupHotkeyController.Set(this.Element, this.sourceFilterGroup, 1, null);
			this.sourceFilterGroup.OnActiveIndexChange += this.OnSourceFilterChange;
		}

		// Token: 0x06009270 RID: 37488 RVA: 0x004434E2 File Offset: 0x004416E2
		private void OnListenerIdReady()
		{
			TaiwuDomainMethod.Call.GetSkillBreakBonusSelectDisplayData(this.Element.GameDataListenerId, this._skillId, this._currentBonus.RelationCharId);
		}

		// Token: 0x06009271 RID: 37489 RVA: 0x00443508 File Offset: 0x00441708
		private void Update()
		{
			bool keyUp = Input.GetKeyUp(KeyCode.Space);
			if (keyUp)
			{
				bool interactable = this.confirmButton.interactable;
				if (interactable)
				{
					this.OnClickConfirm();
				}
			}
		}

		// Token: 0x06009272 RID: 37490 RVA: 0x0044353C File Offset: 0x0044173C
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string a = name;
			if (!(a == "ButtonCancel") && !(a == "ButtonCloseView"))
			{
				if (!(a == "ButtonConfirm"))
				{
					if (a == "ButtonSelectNoBonus")
					{
						this.SelectNoBonus();
					}
				}
				else
				{
					this.OnClickConfirm();
				}
			}
			else
			{
				this.QuickHide();
			}
		}

		// Token: 0x06009273 RID: 37491 RVA: 0x004435A8 File Offset: 0x004417A8
		private void OnClickConfirm()
		{
			SkillBreakPlateBonus? displayingBonus = this.DisplayingBonus;
			bool flag;
			if (displayingBonus != null)
			{
				ESkillBreakPlateBonusType type = displayingBonus.GetValueOrDefault().Type;
				if (type - ESkillBreakPlateBonusType.Item <= 3)
				{
					flag = true;
					goto IL_31;
				}
			}
			flag = false;
			IL_31:
			bool needDialog = flag;
			bool flag2 = !needDialog;
			if (flag2)
			{
				this.ConfirmAndClose();
			}
			else
			{
				string content = LocalStringManager.Get(LanguageKey.LK_Skill_Break_Fill_Bonus_Confirm_Content_Item);
				displayingBonus = this.DisplayingBonus;
				bool flag3 = ((displayingBonus != null) ? new ESkillBreakPlateBonusType?(displayingBonus.GetValueOrDefault().Type) : null) == ESkillBreakPlateBonusType.Friend;
				if (flag3)
				{
					content = LocalStringManager.Get(LanguageKey.LK_Skill_Break_Fill_Bonus_Confirm_Content_RelationFriend);
				}
				displayingBonus = this.DisplayingBonus;
				bool flag4 = ((displayingBonus != null) ? new ESkillBreakPlateBonusType?(displayingBonus.GetValueOrDefault().Type) : null) == ESkillBreakPlateBonusType.Relation;
				if (flag4)
				{
					content = LocalStringManager.Get(LanguageKey.LK_Skill_Break_Fill_Bonus_Confirm_Content_RelationEnemyOrAdore);
				}
				DialogCmd cmd = new DialogCmd
				{
					Title = LocalStringManager.Get(LanguageKey.LK_Skill_Break_Fill_Bonus_Confirm_Title),
					Content = content,
					Type = 1,
					Yes = new Action(this.ConfirmAndClose),
					No = null
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
		}

		// Token: 0x06009274 RID: 37492 RVA: 0x0044372E File Offset: 0x0044192E
		private void OnSourceFilterChange(int newIndex, int oldIndex)
		{
			this.RefreshList(true);
		}

		// Token: 0x06009275 RID: 37493 RVA: 0x0044373C File Offset: 0x0044193C
		private void OnClickItem(int index, RowItem rowItem)
		{
			SkillBreakBonusSelectableItem data = this._filteredData[index];
			EBonusItemType type = data.Type;
			EBonusItemType ebonusItemType = type;
			if (ebonusItemType != EBonusItemType.Exp)
			{
				if (ebonusItemType != EBonusItemType.Character)
				{
					this.OnClickBonusItem(index);
				}
				else
				{
					this.OnClickBonusCharacter(index);
				}
			}
			else
			{
				this.OnClickBonusExp(index);
			}
		}

		// Token: 0x06009276 RID: 37494 RVA: 0x0044378C File Offset: 0x0044198C
		private void OnClickBonusItem(int index)
		{
			ItemDisplayData itemDisplayData = this._filteredData[index].ItemDisplayData;
			this._selectedItemSourceType = this.SelectedFilterItemSourceType;
			bool flag = itemDisplayData.Key.IsValid();
			if (flag)
			{
				SkillBreakPlateBonus? generatedBonus = this._generatedBonus;
				bool flag2 = generatedBonus != null && generatedBonus.GetValueOrDefault().Type == ESkillBreakPlateBonusType.Item && this._selectedItemKey == itemDisplayData.Key;
				if (flag2)
				{
					this.SelectNoBonus();
				}
				else
				{
					this.GenerateBonusByItem(itemDisplayData);
					this.RefreshBonusDisplay();
				}
			}
			else
			{
				this.SelectNoBonus();
			}
		}

		// Token: 0x06009277 RID: 37495 RVA: 0x00443830 File Offset: 0x00441A30
		private void OnClickBonusCharacter(int index)
		{
			SkillBreakBonusSelectableItem data = this._filteredData[index];
			bool flag = data.CharacterDisplayData == null || data.CharacterDisplayData.CharacterId < 0;
			if (flag)
			{
				this.SelectNoBonus();
			}
			else
			{
				bool flag2 = this._generatedBonus != null && ViewSkillBreakBonusSelect.IsSameBonus(this._generatedBonus.Value, data.BonusData);
				if (flag2)
				{
					this.SelectNoBonus();
				}
				else
				{
					this.GenerateBonusByCharacter(data.CharacterDisplayData, data.BonusData);
					this.RefreshBonusDisplay();
				}
			}
		}

		// Token: 0x06009278 RID: 37496 RVA: 0x004438C8 File Offset: 0x00441AC8
		private void OnClickBonusExp(int index)
		{
			int expLevel = SkillBreakPlateConstants.ExpLevelValues.Count - 1 - index;
			bool flag = expLevel < 0;
			if (flag)
			{
				this.SelectNoBonus();
			}
			else
			{
				SkillBreakPlateBonus? generatedBonus = this._generatedBonus;
				bool flag2 = generatedBonus != null && generatedBonus.GetValueOrDefault().Type == ESkillBreakPlateBonusType.Exp && this._generatedBonus.Value.ExpLevel == expLevel;
				if (flag2)
				{
					this.SelectNoBonus();
				}
				else
				{
					this.GenerateBonusByExp(expLevel);
					this.RefreshBonusDisplay();
				}
			}
		}

		// Token: 0x06009279 RID: 37497 RVA: 0x00443958 File Offset: 0x00441B58
		private void SelectNoBonus()
		{
			bool flag = this._currentBonus == SkillBreakPlateBonus.Invalid;
			if (flag)
			{
				this.<SelectNoBonus>g__Action|47_0();
			}
			else
			{
				string title = LanguageKey.LK_Skill_Break_BonusSelect_Remove_Title.Tr();
				ESkillBreakPlateBonusType type = this._currentBonus.Type;
				string content = (type == ESkillBreakPlateBonusType.Item || type == ESkillBreakPlateBonusType.Exp) ? LanguageKey.LK_Skill_Break_BonusSelect_Remove_Item.Tr() : LanguageKey.LK_Skill_Break_BonusSelect_Remove_Character.Tr();
				CommonUtils.ShowConfirmDialog(title, content, new Action(this.<SelectNoBonus>g__Action|47_0), null, EDialogType.None);
			}
		}

		// Token: 0x0600927A RID: 37498 RVA: 0x004439D5 File Offset: 0x00441BD5
		private void GenerateBonusByHoverItem(ItemDisplayData itemDisplayData)
		{
			this._hoverGeneratedBonus = new SkillBreakPlateBonus?(SkillBreakPlateBonus.CreateItem(itemDisplayData.Key.ItemType, itemDisplayData.Key.TemplateId));
			this._hoverItemDisplayData = itemDisplayData;
			this.RefreshBonusDisplay();
		}

		// Token: 0x0600927B RID: 37499 RVA: 0x00443A0C File Offset: 0x00441C0C
		private void GenerateBonusByHoverExp(int expLevel)
		{
			bool flag = expLevel < 0;
			if (flag)
			{
				this._hoverGeneratedBonus = null;
			}
			else
			{
				this._hoverGeneratedBonus = new SkillBreakPlateBonus?(SkillBreakPlateBonus.CreateExp(expLevel));
			}
			this.RefreshBonusDisplay();
		}

		// Token: 0x0600927C RID: 37500 RVA: 0x00443A48 File Offset: 0x00441C48
		private void RefreshSourceFilterEnable()
		{
			bool flag = this._skillBreakBonusSelectDisplayData == null;
			if (!flag)
			{
				bool usable = this._selectorMode == ViewSkillBreakBonusSelect.ESelectorMode.Item;
				this.sourceFilterGroup.gameObject.SetActive(usable);
				ItemSourceToggleHelper.RefreshInteractableForInteract(this.sourceFilterGroup, this._skillBreakBonusSelectDisplayData.CanTransferItemToWarehouse, false);
			}
		}

		// Token: 0x0600927D RID: 37501 RVA: 0x00443A9C File Offset: 0x00441C9C
		private ViewSkillBreakBonusSelect.ESelectorMode GetSelectedModeFromSelectedType()
		{
			SortAndFilterState state = this._sortAndFilterController.SortAndFilterState;
			LineState mainFilterState = state.LineStates[0];
			ToggleKey toggleKey = mainFilterState.ToggleGroupState;
			bool isAll = toggleKey.IsAll;
			ViewSkillBreakBonusSelect.ESelectorMode result;
			if (isAll)
			{
				result = ViewSkillBreakBonusSelect.ESelectorMode.Exp;
			}
			else
			{
				int index = toggleKey.Index;
				if (!true)
				{
				}
				ViewSkillBreakBonusSelect.ESelectorMode eselectorMode;
				if (index != 0)
				{
					if (index != 1)
					{
						eselectorMode = ViewSkillBreakBonusSelect.ESelectorMode.Item;
					}
					else
					{
						eselectorMode = ViewSkillBreakBonusSelect.ESelectorMode.Character;
					}
				}
				else
				{
					eselectorMode = ViewSkillBreakBonusSelect.ESelectorMode.Exp;
				}
				if (!true)
				{
				}
				result = eselectorMode;
			}
			return result;
		}

		// Token: 0x0600927E RID: 37502 RVA: 0x00443B10 File Offset: 0x00441D10
		private RowItem GetRowItemTemplate()
		{
			ViewSkillBreakBonusSelect.ESelectorMode mode = this.GetSelectedModeFromSelectedType();
			bool flag = mode == ViewSkillBreakBonusSelect.ESelectorMode.Character;
			RowItem result;
			if (flag)
			{
				result = this.rowTemplateForChar;
			}
			else
			{
				result = this.rowTemplate;
			}
			return result;
		}

		// Token: 0x0600927F RID: 37503 RVA: 0x00443B40 File Offset: 0x00441D40
		private void RefreshNoBonusCombatSkillView()
		{
			this.RefreshCombatSkillInfo();
			CombatSkillDisplayData combatSkillDisplayData = this._skillBreakBonusSelectDisplayData.CombatSkillDisplayData;
			this.combatSkillItem.Set(combatSkillDisplayData);
			this.combatSkillAreaTips.enabled = (combatSkillDisplayData != null);
			bool flag = combatSkillDisplayData != null;
			if (flag)
			{
				this.combatSkillAreaTips.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("CombatSkillId", combatSkillDisplayData.TemplateId).Set("CharId", combatSkillDisplayData.CharId);
				bool showing = this.combatSkillAreaTips.Showing;
				if (showing)
				{
					this.combatSkillAreaTips.Refresh(false, -1);
				}
			}
		}

		// Token: 0x06009280 RID: 37504 RVA: 0x00443BD8 File Offset: 0x00441DD8
		private void RefreshCombatSkillInfo()
		{
			CombatSkillItem config = CombatSkill.Instance.GetItem(this._skillId);
			this.combatSkillNameProperty.Set(LanguageKey.LK_ItemName.Tr(), config.Name.SetColor(Colors.Instance.GradeColors[(int)config.Grade]), new bool?(true));
			this.combatSkillTypeProperty.Set(LanguageKey.LK_ItemType.Tr(), Config.CombatSkillType.Instance[config.Type].Name, new bool?(false));
			this.combatSkillSectProperty.Set(LanguageKey.LK_CombatSkill_Sect_Type.Tr(), Organization.Instance[config.SectId].Name, new bool?(true));
		}

		// Token: 0x06009281 RID: 37505 RVA: 0x00443C95 File Offset: 0x00441E95
		private IEnumerable<ViewSkillBreakBonusSelect.EColumnType> GetColumnTypeList()
		{
			switch (this._selectorMode)
			{
			case ViewSkillBreakBonusSelect.ESelectorMode.Exp:
				yield return ViewSkillBreakBonusSelect.EColumnType.Exp;
				break;
			case ViewSkillBreakBonusSelect.ESelectorMode.Item:
				yield return ViewSkillBreakBonusSelect.EColumnType.IconAndName;
				yield return ViewSkillBreakBonusSelect.EColumnType.Amount;
				yield return ViewSkillBreakBonusSelect.EColumnType.Type;
				yield return ViewSkillBreakBonusSelect.EColumnType.Weight;
				yield return ViewSkillBreakBonusSelect.EColumnType.Value;
				break;
			case ViewSkillBreakBonusSelect.ESelectorMode.Character:
				yield return ViewSkillBreakBonusSelect.EColumnType.AvatarAndName;
				yield return ViewSkillBreakBonusSelect.EColumnType.Grade;
				yield return ViewSkillBreakBonusSelect.EColumnType.Favor;
				yield return ViewSkillBreakBonusSelect.EColumnType.Attainment;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			yield break;
		}

		// Token: 0x06009282 RID: 37506 RVA: 0x00443CA5 File Offset: 0x00441EA5
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitions()
		{
			foreach (ViewSkillBreakBonusSelect.EColumnType flag in this.GetColumnTypeList())
			{
				bool flag2 = (flag & this.ColumnTypeFlags) == (ViewSkillBreakBonusSelect.EColumnType)0;
				if (!flag2)
				{
					ViewSkillBreakBonusSelect.EColumnType ecolumnType = flag;
					ViewSkillBreakBonusSelect.EColumnType ecolumnType2 = ecolumnType;
					ViewSkillBreakBonusSelect.EColumnType ecolumnType3 = ecolumnType2;
					if (ecolumnType3 <= ViewSkillBreakBonusSelect.EColumnType.AvatarAndName)
					{
						if (ecolumnType3 <= ViewSkillBreakBonusSelect.EColumnType.Weight)
						{
							switch (ecolumnType3)
							{
							case ViewSkillBreakBonusSelect.EColumnType.IconAndName:
							{
								ColumnDefinition<SkillBreakBonusSelectableItem, ItemDisplayData> columnDefinition = new ColumnDefinition<SkillBreakBonusSelectableItem, ItemDisplayData>();
								columnDefinition.LayoutOption = new LayoutOption(200f, 1f, 400f, 1);
								columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Item.Tr());
								columnDefinition.CellDataGenerator = ((SkillBreakBonusSelectableItem data) => data.ItemDisplayData);
								columnDefinition.SortId = 0;
								yield return columnDefinition;
								break;
							}
							case ViewSkillBreakBonusSelect.EColumnType.Amount:
							{
								ColumnDefinition<SkillBreakBonusSelectableItem, string> columnDefinition2 = new ColumnDefinition<SkillBreakBonusSelectableItem, string>();
								columnDefinition2.LayoutOption = new LayoutOption(60f, 1f, 100f, 1);
								columnDefinition2.TableHeadLabel = (() => LanguageKey.LK_Count.Tr());
								columnDefinition2.CellDataGenerator = ((SkillBreakBonusSelectableItem data) => data.ItemDisplayData.Amount.ToString());
								columnDefinition2.SortId = 17;
								yield return columnDefinition2;
								break;
							}
							case ViewSkillBreakBonusSelect.EColumnType.IconAndName | ViewSkillBreakBonusSelect.EColumnType.Amount:
								goto IL_6F4;
							case ViewSkillBreakBonusSelect.EColumnType.Type:
							{
								ColumnDefinition<SkillBreakBonusSelectableItem, string> columnDefinition3 = new ColumnDefinition<SkillBreakBonusSelectableItem, string>();
								columnDefinition3.LayoutOption = new LayoutOption(60f, 1f, 100f, 1);
								columnDefinition3.TableHeadLabel = (() => LanguageKey.LK_Type.Tr());
								columnDefinition3.CellDataGenerator = ((SkillBreakBonusSelectableItem data) => LocalStringManager.Get(string.Format("LK_ItemType_{0}", data.ItemDisplayData.Key.ItemType)));
								columnDefinition3.SortId = 56;
								yield return columnDefinition3;
								break;
							}
							default:
							{
								if (ecolumnType3 != ViewSkillBreakBonusSelect.EColumnType.Weight)
								{
									goto IL_6F4;
								}
								ColumnDefinition<SkillBreakBonusSelectableItem, string> columnDefinition4 = new ColumnDefinition<SkillBreakBonusSelectableItem, string>();
								columnDefinition4.LayoutOption = new LayoutOption(60f, 1f, 100f, 1);
								columnDefinition4.TableHeadLabel = (() => LanguageKey.LK_Weight.Tr());
								columnDefinition4.CellDataGenerator = ((SkillBreakBonusSelectableItem data) => NumberFormatUtils.FormatItemWeight(data.ItemDisplayData.Weight));
								columnDefinition4.SortId = 6;
								yield return columnDefinition4;
								break;
							}
							}
						}
						else if (ecolumnType3 != ViewSkillBreakBonusSelect.EColumnType.Value)
						{
							if (ecolumnType3 != ViewSkillBreakBonusSelect.EColumnType.AvatarAndName)
							{
								goto IL_6F4;
							}
							ColumnDefinition<SkillBreakBonusSelectableItem, SkillBreakBonusSelectAvatarAndNameCellData> columnDefinition5 = new ColumnDefinition<SkillBreakBonusSelectableItem, SkillBreakBonusSelectAvatarAndNameCellData>();
							columnDefinition5.LayoutOption = new LayoutOption(60f, 1f, 446f, 1);
							columnDefinition5.TableHeadLabel = (() => LanguageKey.LK_Char_Name.Tr());
							columnDefinition5.CellDataGenerator = new Func<SkillBreakBonusSelectableItem, SkillBreakBonusSelectAvatarAndNameCellData>(this.GetCharacterItemData);
							columnDefinition5.SortId = 0;
							yield return columnDefinition5;
						}
						else
						{
							ColumnDefinition<SkillBreakBonusSelectableItem, string> columnDefinition6 = new ColumnDefinition<SkillBreakBonusSelectableItem, string>();
							columnDefinition6.LayoutOption = new LayoutOption(60f, 1f, 100f, 1);
							columnDefinition6.TableHeadLabel = (() => LanguageKey.LK_ItemValue.Tr());
							columnDefinition6.CellDataGenerator = ((SkillBreakBonusSelectableItem data) => data.ItemDisplayData.Value.ToString());
							columnDefinition6.SortId = 5;
							yield return columnDefinition6;
						}
					}
					else if (ecolumnType3 <= ViewSkillBreakBonusSelect.EColumnType.Favor)
					{
						if (ecolumnType3 != ViewSkillBreakBonusSelect.EColumnType.Grade)
						{
							if (ecolumnType3 != ViewSkillBreakBonusSelect.EColumnType.Favor)
							{
								goto IL_6F4;
							}
							ColumnDefinition<SkillBreakBonusSelectableItem, string> columnDefinition7 = new ColumnDefinition<SkillBreakBonusSelectableItem, string>();
							columnDefinition7.LayoutOption = new LayoutOption(60f, 1f, 400f, 1);
							columnDefinition7.TableHeadLabel = (() => LanguageKey.LK_Favorability.Tr());
							columnDefinition7.CellDataGenerator = ((SkillBreakBonusSelectableItem data) => this.GetCharacterItemData(data).GetFavor());
							columnDefinition7.SortId = 11;
							yield return columnDefinition7;
						}
						else
						{
							ColumnDefinition<SkillBreakBonusSelectableItem, string> columnDefinition8 = new ColumnDefinition<SkillBreakBonusSelectableItem, string>();
							columnDefinition8.LayoutOption = new LayoutOption(60f, 1f, 400f, 1);
							columnDefinition8.TableHeadLabel = (() => LanguageKey.LK_Grade_Title.Tr());
							columnDefinition8.CellDataGenerator = ((SkillBreakBonusSelectableItem data) => this.GetCharacterItemData(data).GetGrade());
							columnDefinition8.SortId = 1;
							yield return columnDefinition8;
						}
					}
					else if (ecolumnType3 != ViewSkillBreakBonusSelect.EColumnType.Attainment)
					{
						if (ecolumnType3 != ViewSkillBreakBonusSelect.EColumnType.Exp)
						{
							goto IL_6F4;
						}
						ColumnDefinition<SkillBreakBonusSelectableItem, int> columnDefinition9 = new ColumnDefinition<SkillBreakBonusSelectableItem, int>();
						columnDefinition9.LayoutOption = new LayoutOption(60f, 1f, 1646f, 1);
						columnDefinition9.TableHeadLabel = (() => LanguageKey.LK_Name.Tr());
						columnDefinition9.CellDataGenerator = ((SkillBreakBonusSelectableItem data) => data.BonusData.ExpLevel);
						columnDefinition9.SortId = 0;
						yield return columnDefinition9;
					}
					else
					{
						ColumnDefinition<SkillBreakBonusSelectableItem, string> columnDefinition10 = new ColumnDefinition<SkillBreakBonusSelectableItem, string>();
						columnDefinition10.LayoutOption = new LayoutOption(60f, 1f, 400f, 1);
						columnDefinition10.TableHeadLabel = (() => LanguageKey.LK_Skill_Break_BonusSelect_FriendAttainment.Tr());
						columnDefinition10.CellDataGenerator = ((SkillBreakBonusSelectableItem data) => this.GetCharacterItemData(data).GetAttainment());
						columnDefinition10.SortId = 121;
						yield return columnDefinition10;
					}
					continue;
					IL_6F4:
					throw new ArgumentOutOfRangeException();
				}
			}
			IEnumerator<ViewSkillBreakBonusSelect.EColumnType> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06009283 RID: 37507 RVA: 0x00443CB8 File Offset: 0x00441EB8
		private SkillBreakBonusSelectAvatarAndNameCellData GetCharacterItemData(SkillBreakBonusSelectableItem selectableItem)
		{
			return new SkillBreakBonusSelectAvatarAndNameCellData
			{
				Bonus = selectableItem.BonusData,
				DisplayData = selectableItem.CharacterDisplayData,
				SkillId = this._skillId
			};
		}

		// Token: 0x06009284 RID: 37508 RVA: 0x00443CF3 File Offset: 0x00441EF3
		private IEnumerable<ViewSkillBreakBonusSelect.EColumnType> GenerateColumnTypeFlags()
		{
			foreach (ViewSkillBreakBonusSelect.EColumnType flag in this.GetColumnTypeList())
			{
				bool flag2 = (flag & this.ColumnTypeFlags) == (ViewSkillBreakBonusSelect.EColumnType)0;
				if (!flag2)
				{
					yield return flag;
				}
			}
			IEnumerator<ViewSkillBreakBonusSelect.EColumnType> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06009285 RID: 37509 RVA: 0x00443D04 File Offset: 0x00441F04
		private RowCellContainer GetCellContainerTemplate(ViewSkillBreakBonusSelect.EColumnType columnType)
		{
			if (!true)
			{
			}
			RowCellContainer result;
			if (columnType >= ViewSkillBreakBonusSelect.EColumnType.Grade)
			{
				if (columnType <= ViewSkillBreakBonusSelect.EColumnType.Attainment)
				{
					result = this.avatarSingleTextCellContainer;
					goto IL_56;
				}
				if (columnType == ViewSkillBreakBonusSelect.EColumnType.Exp)
				{
					result = this.expCellContainer;
					goto IL_56;
				}
			}
			else
			{
				if (columnType == ViewSkillBreakBonusSelect.EColumnType.IconAndName)
				{
					result = this.itemIconAndNameCellContainer;
					goto IL_56;
				}
				if (columnType == ViewSkillBreakBonusSelect.EColumnType.AvatarAndName)
				{
					result = this.avatarAndNameCellContainer;
					goto IL_56;
				}
			}
			result = this.singleTextCellContainer;
			IL_56:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06009286 RID: 37510 RVA: 0x00443D70 File Offset: 0x00441F70
		private void PrepareRowTemplateContainers(RowItem rowItem)
		{
			Transform containerRoot = rowItem.ContainerRoot;
			for (int i = containerRoot.childCount - 1; i >= 0; i--)
			{
				Transform child = containerRoot.GetChild(i);
				bool flag = child.GetComponent<RowCellContainer>() != null;
				if (flag)
				{
					Object.Destroy(child.gameObject);
				}
			}
			foreach (ViewSkillBreakBonusSelect.EColumnType columnType in this.GenerateColumnTypeFlags())
			{
				RowCellContainer containerTemplate = this.GetCellContainerTemplate(columnType);
				RowCellContainer container = Object.Instantiate<RowCellContainer>(containerTemplate, containerRoot);
				container.gameObject.SetActive(true);
			}
		}

		// Token: 0x06009287 RID: 37511 RVA: 0x00443E30 File Offset: 0x00442030
		private void RefreshList(bool isModeChanged = true)
		{
			RowItem template = this.GetRowItemTemplate();
			if (isModeChanged)
			{
				this.PrepareRowTemplateContainers(template);
				this.scroll.ClearInfinityScrollCache();
				this.scroll.Init<SkillBreakBonusSelectDisplayData>(this.GenerateColumnDefinitions(), true, new Action<int, GameObject>(this.OnItemRender), new Action<int, RowItem>(this.OnClickItem));
				this.scroll.SetSortController(this._sortAndFilterController);
			}
			this.ApplySortAndFilter();
			this.scroll.SetData<SkillBreakBonusSelectableItem>(this._filteredData, -1);
			bool flag = this.scroll.InfiniteScroll.srcPrefab != template.gameObject;
			if (flag)
			{
				this.scroll.InfiniteScroll.UpdateStyle(template.gameObject, this._filteredData.Count);
			}
		}

		// Token: 0x06009288 RID: 37512 RVA: 0x00443EF8 File Offset: 0x004420F8
		private void ApplySortAndFilter()
		{
			this._filteredData.Clear();
			bool flag = this._skillBreakBonusSelectDisplayData == null;
			if (!flag)
			{
				List<SkillBreakBonusSelectableItem> list = this._skillBreakBonusSelectDisplayData.GetTotalBonusItemList(this.SelectedFilterItemSourceType);
				List<SkillBreakBonusSelectableItem> sourceData = list;
				bool flag2 = this._sortAndFilterController == null || sourceData == null;
				if (flag2)
				{
					bool flag3 = sourceData != null;
					if (flag3)
					{
						this._filteredData.AddRange(sourceData);
					}
				}
				else
				{
					Func<SkillBreakBonusSelectableItem, bool> filter = this._sortAndFilterController.GenerateFilter();
					foreach (SkillBreakBonusSelectableItem item in sourceData)
					{
						bool flag4 = filter(item);
						if (flag4)
						{
							this._filteredData.Add(item);
						}
					}
					Comparison<SkillBreakBonusSelectableItem> comparer = this._sortAndFilterController.GenerateComparer(this._filteredData);
					this._filteredData.Sort(comparer);
					this._sortAndFilterController.AfterFilter(sourceData);
				}
			}
		}

		// Token: 0x06009289 RID: 37513 RVA: 0x00444000 File Offset: 0x00442200
		private void OnItemRender(int index, GameObject rowObject)
		{
			switch (this._selectorMode)
			{
			case ViewSkillBreakBonusSelect.ESelectorMode.Exp:
				this.OnRenderExp(index, rowObject);
				break;
			case ViewSkillBreakBonusSelect.ESelectorMode.Item:
				this.OnRenderItem(index, rowObject);
				break;
			case ViewSkillBreakBonusSelect.ESelectorMode.Character:
				this.OnRenderCharacter(index, rowObject);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x0600928A RID: 37514 RVA: 0x00444054 File Offset: 0x00442254
		private void OnRenderCharacter(int index, GameObject rowObject)
		{
			RowItem rowItem = rowObject.GetComponent<RowItem>();
			SkillBreakBonusSelectableItem data = this._filteredData[index];
			SkillBreakPlateBonus bonus = data.BonusData;
			rowItem.OnPointerEnterEvent = delegate()
			{
				this.OnEnterChar(bonus);
			};
			rowItem.OnPointerExitEvent = delegate()
			{
				this.OnExitChar(bonus);
			};
			bool isSelected = this._generatedBonus != null && ViewSkillBreakBonusSelect.IsSameBonus(this._generatedBonus.Value, data.BonusData);
			rowItem.SetSelected(isSelected);
		}

		// Token: 0x0600928B RID: 37515 RVA: 0x004440E2 File Offset: 0x004422E2
		private void OnEnterChar(SkillBreakPlateBonus bonus)
		{
			this._hoverGeneratedBonus = new SkillBreakPlateBonus?(bonus);
			this.RefreshBonusDisplay();
		}

		// Token: 0x0600928C RID: 37516 RVA: 0x004440F8 File Offset: 0x004422F8
		private void OnExitChar(SkillBreakPlateBonus bonus)
		{
			bool flag = this._hoverGeneratedBonus != null && ViewSkillBreakBonusSelect.IsSameBonus(bonus, this._hoverGeneratedBonus.Value);
			if (flag)
			{
				this._hoverGeneratedBonus = this._generatedBonus;
				this.RefreshBonusDisplay();
			}
		}

		// Token: 0x0600928D RID: 37517 RVA: 0x00444140 File Offset: 0x00442340
		private void OnRenderItem(int index, GameObject rowObject)
		{
			RowItem rowItem = rowObject.GetComponent<RowItem>();
			SkillBreakBonusSelectableItem data = this._filteredData[index];
			rowItem.OnPointerEnterEvent = delegate()
			{
				this.OnEnterItem(data.ItemDisplayData);
			};
			rowItem.OnPointerExitEvent = delegate()
			{
				this.OnExitItem(data.ItemDisplayData);
			};
			rowItem.TipDisplayer.ShowOnLeft = true;
			RowItemLine.SetMouseTipDisplayer(true, data.ItemDisplayData, rowItem.TipDisplayer);
			bool isSelected = this._generatedBonus != null && ViewSkillBreakBonusSelect.IsSameBonus(this._generatedBonus.Value, data.BonusData) && data.ItemDisplayData.RealKey.Equals(this._selectedItemKey);
			rowItem.SetSelected(isSelected);
			bool interactable = !data.ItemDisplayData.IsLocked;
			rowItem.SetInteractable(interactable, true);
			rowItem.SetDisabled(!interactable);
		}

		// Token: 0x0600928E RID: 37518 RVA: 0x00444239 File Offset: 0x00442439
		private void OnEnterItem(ItemDisplayData itemDisplayData)
		{
			this.GenerateBonusByHoverItem(itemDisplayData);
		}

		// Token: 0x0600928F RID: 37519 RVA: 0x00444244 File Offset: 0x00442444
		private void OnExitItem(ItemDisplayData itemDisplayData)
		{
			bool flag;
			if (this._hoverGeneratedBonus != null)
			{
				ItemDisplayData hoverItemDisplayData = this._hoverItemDisplayData;
				ItemKey? itemKey = (hoverItemDisplayData != null) ? new ItemKey?(hoverItemDisplayData.Key) : null;
				flag = (itemKey == ((itemDisplayData != null) ? new ItemKey?(itemDisplayData.Key) : null));
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this._hoverGeneratedBonus = this._generatedBonus;
				this.RefreshBonusDisplay();
			}
		}

		// Token: 0x06009290 RID: 37520 RVA: 0x004442E8 File Offset: 0x004424E8
		private void OnRenderExp(int index, GameObject rowObject)
		{
			RowItem rowItem = rowObject.GetComponent<RowItem>();
			SkillBreakBonusSelectableItem data = this._filteredData[index];
			int expLevel = data.BonusData.ExpLevel;
			int levelExpValue = SkillBreakPlateConstants.ExpLevelValues[expLevel];
			bool isDisableStyle = this._currentExp < levelExpValue;
			rowItem.SetInteractable(!isDisableStyle, true);
			rowItem.OnPointerEnterEvent = delegate()
			{
				this.OnEnterExpFakeItem(expLevel);
			};
			rowItem.OnPointerExitEvent = delegate()
			{
				this.OnExitExpFakeItem(expLevel);
			};
			this.RefreshExpFakeItemTip(rowItem.TipDisplayer, levelExpValue);
			bool isSelected = this._generatedBonus != null && ViewSkillBreakBonusSelect.IsSameBonus(this._generatedBonus.Value, data.BonusData);
			rowItem.SetSelected(isSelected);
		}

		// Token: 0x06009291 RID: 37521 RVA: 0x004443B4 File Offset: 0x004425B4
		private void OnEnterExpFakeItem(int expLevel)
		{
			this.GenerateBonusByHoverExp(expLevel);
		}

		// Token: 0x06009292 RID: 37522 RVA: 0x004443C0 File Offset: 0x004425C0
		private void OnExitExpFakeItem(int expLevel)
		{
			bool flag = this._hoverGeneratedBonus != null && ViewSkillBreakBonusSelect.IsSameBonus(this._hoverGeneratedBonus.Value, SkillBreakPlateBonus.CreateExp(expLevel));
			if (flag)
			{
				this._hoverGeneratedBonus = this._generatedBonus;
				this.RefreshBonusDisplay();
			}
		}

		// Token: 0x06009293 RID: 37523 RVA: 0x00444410 File Offset: 0x00442610
		private void RefreshExpFakeItemTip(TooltipInvoker tip, int levelExp)
		{
			tip.enabled = (levelExp > 0);
			tip.Type = TipType.ExpCheck;
			if (tip.RuntimeParam == null)
			{
				tip.RuntimeParam = new ArgumentBox();
			}
			tip.RuntimeParam.Set("NeedExp", levelExp);
			tip.RuntimeParam.Set("HasExp", this._currentExp);
			tip.RuntimeParam.Set("Desc", LocalStringManager.Get(LanguageKey.LK_Skill_Break_Tip_ExpNotEnough));
		}

		// Token: 0x06009294 RID: 37524 RVA: 0x00444490 File Offset: 0x00442690
		private void GenerateEmptyBonus()
		{
			this._generatedBonus = new SkillBreakPlateBonus?(SkillBreakPlateBonus.Invalid);
		}

		// Token: 0x06009295 RID: 37525 RVA: 0x004444A3 File Offset: 0x004426A3
		private void GenerateBonusByItem(ItemDisplayData itemDisplayData)
		{
			this._generatedBonus = new SkillBreakPlateBonus?(SkillBreakPlateBonus.CreateItem(itemDisplayData.Key.ItemType, itemDisplayData.Key.TemplateId));
			this._selectedItemKey = itemDisplayData.Key;
		}

		// Token: 0x06009296 RID: 37526 RVA: 0x004444D8 File Offset: 0x004426D8
		private void GenerateBonusByExp(int expLevel)
		{
			this._generatedBonus = new SkillBreakPlateBonus?(SkillBreakPlateBonus.CreateExp(expLevel));
		}

		// Token: 0x06009297 RID: 37527 RVA: 0x004444EC File Offset: 0x004426EC
		private void GenerateBonusByCharacter(CharacterDisplayData characterDisplayData, SkillBreakPlateBonus bonus)
		{
			this._generatedBonus = new SkillBreakPlateBonus?(bonus);
			this._selectedCharDisplayData = characterDisplayData;
		}

		// Token: 0x06009298 RID: 37528 RVA: 0x00444504 File Offset: 0x00442704
		private void RefreshBonusDisplay()
		{
			this.scroll.InfiniteScroll.ReRender();
			bool isSelected = this._generatedBonus != null;
			bool isChanged = isSelected && !ViewSkillBreakBonusSelect.IsSameBonus(this._generatedBonus.Value, this._currentBonus);
			this.confirmButton.interactable = isChanged;
			this.tipConfirmButton.enabled = !this.confirmButton.interactable;
			bool enabled = this.tipConfirmButton.enabled;
			if (enabled)
			{
				string content = (!isSelected) ? LanguageKey.LK_Skill_Break_BonusSelect_Empty_Info.Tr().SetColor("brightred") : ((!isChanged) ? LanguageKey.LK_Skill_Break_BonusSelect_NoChange.Tr().SetColor("brightred") : string.Empty);
				this.tipConfirmButton.PresetParam = new string[]
				{
					content
				};
				this.tipConfirmButton.Type = TipType.SingleDesc;
			}
			GameObject gameObject = this.activeDescLabelRelation.gameObject;
			SkillBreakPlateBonus? displayingBonus = this.DisplayingBonus;
			bool active;
			if (displayingBonus != null)
			{
				ESkillBreakPlateBonusType type = displayingBonus.GetValueOrDefault().Type;
				if (type == ESkillBreakPlateBonusType.Relation || type == ESkillBreakPlateBonusType.Friend)
				{
					active = true;
					goto IL_117;
				}
			}
			active = false;
			IL_117:
			gameObject.SetActive(active);
			bool flag = this.DisplayingBonus == null || this.DisplayingBonus.Value.Type == ESkillBreakPlateBonusType.None;
			if (flag)
			{
				this.ClearBonusEffects();
				this.SelectItemToShowForHasBonus(false);
				this.RefreshPowerLabel();
			}
			else
			{
				this.SelectItemToShowForHasBonus(true);
				this.SwitchSelectedBonusSlot();
				switch (this.DisplayingBonus.Value.Type)
				{
				case ESkillBreakPlateBonusType.Item:
					this.RefreshBonusItemView();
					break;
				case ESkillBreakPlateBonusType.Relation:
				case ESkillBreakPlateBonusType.Friend:
					this.RefreshBonusCharacterHead();
					this.RefreshRelationActiveDesc();
					break;
				case ESkillBreakPlateBonusType.Exp:
					this.RefreshBonusExpItem();
					break;
				}
				this.RefreshBonusInfoLayout();
				int range = this.DisplayingBonus.Value.ImpactRange;
				this.activeDescLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_Skill_Break_BonusSelect_Info_Active_Desc, range).ColorReplace();
				this.RefreshPowerLabel();
				this.RefreshBonusEffects();
			}
		}

		// Token: 0x06009299 RID: 37529 RVA: 0x0044473C File Offset: 0x0044293C
		private void SelectItemToShowForHasBonus(bool hasBonus)
		{
			foreach (GameObject go in this.itemsShowWhenHasBonus)
			{
				go.SetActive(hasBonus);
			}
			foreach (GameObject go2 in this.itemsShowWhenNoBonus)
			{
				go2.SetActive(!hasBonus);
			}
		}

		// Token: 0x0600929A RID: 37530 RVA: 0x0044479C File Offset: 0x0044299C
		private void RefreshRelationActiveDesc()
		{
			bool flag = this.DisplayingBonus == null;
			if (!flag)
			{
				bool flag2 = this.DisplayingBonus.Value.Type == ESkillBreakPlateBonusType.Friend;
				if (flag2)
				{
					this.activeDescLabelRelation.text = LocalStringManager.Get(LanguageKey.LK_Skill_Break_BonusSelect_Info_Active_Desc_Friend);
				}
				else
				{
					TextMeshProUGUI textMeshProUGUI = this.activeDescLabelRelation;
					ushort relationType = this.DisplayingBonus.Value.RelationType;
					if (!true)
					{
					}
					LanguageKey id;
					if (relationType != 16384)
					{
						if (relationType != 32768)
						{
							id = LanguageKey.LK_None;
						}
						else
						{
							id = LanguageKey.LK_Skill_Break_BonusSelect_Info_Active_Desc_Enemy;
						}
					}
					else
					{
						id = LanguageKey.LK_Skill_Break_BonusSelect_Info_Active_Desc_Adore;
					}
					if (!true)
					{
					}
					textMeshProUGUI.text = LocalStringManager.Get(id);
				}
			}
		}

		// Token: 0x0600929B RID: 37531 RVA: 0x00444868 File Offset: 0x00442A68
		private void RefreshBonusExpItem()
		{
			bool flag = this.DisplayingBonus == null;
			if (!flag)
			{
				this.selectedExpFakeItemBack.SetBack(this.DisplayingBonus.Value.Grade);
			}
		}

		// Token: 0x0600929C RID: 37532 RVA: 0x004448B0 File Offset: 0x00442AB0
		private void RefreshBonusEffects()
		{
			bool flag = this.DisplayingBonus == null;
			if (!flag)
			{
				SkillBreakBonusEffectHelper.GenerateBonusEffectDisplays(this._skillId, this.DisplayingBonus.Value, this._lifeSkillAttainments, this._resultList);
				CommonUtils.PrepareEnoughChildren(this.bonusEffectLayout.transform, this.bonusEffectItemTemplate.gameObject, this._resultList.Count, null);
				for (int i = 0; i < this._resultList.Count; i++)
				{
					SkillBreakBonusEffect bonusItem = this.bonusEffectLayout.transform.GetChild(i).GetComponent<SkillBreakBonusEffect>();
					bonusItem.Refresh(this._resultList[i], SkillBreakBonusEffect.EBonusIconSize.Big);
					bonusItem.SetShowBack(i % 2 == 1);
				}
			}
		}

		// Token: 0x0600929D RID: 37533 RVA: 0x00444988 File Offset: 0x00442B88
		private void ClearBonusEffects()
		{
			CommonUtils.PrepareEnoughChildren(this.bonusEffectLayout.transform, this.bonusEffectItemTemplate.gameObject, 0, null);
		}

		// Token: 0x0600929E RID: 37534 RVA: 0x004449BC File Offset: 0x00442BBC
		private void RefreshPowerLabel()
		{
			string title = LanguageKey.LK_Skill_Break_BonusSelect_Info_Power_Desc.Tr();
			bool flag = this.DisplayingBonus == null;
			if (flag)
			{
				this.powerProperty.Set(title, string.Empty, new bool?(false));
			}
			else
			{
				int rangeIndex = this.DisplayingBonus.Value.ImpactRange - 1;
				string[] coloredPowerList = this._possiblePowerList.Select(delegate(int p, int i)
				{
					string color = (i == rangeIndex) ? "brightblue" : "grayscaleblack";
					return string.Format("+{0}", p).SetColor(color);
				}).ToArray<string>();
				string value = LocalStringManager.GetFormat(LanguageKey.LK_Skill_Break_BonusSelect_Info_Power_Value, coloredPowerList[0], coloredPowerList[1], coloredPowerList[2]).ColorReplace();
				this.powerProperty.Set(title, value, new bool?(false));
			}
		}

		// Token: 0x0600929F RID: 37535 RVA: 0x00444A78 File Offset: 0x00442C78
		private void RefreshBonusInfoLayout()
		{
			this.RefreshSelectedBonusInfoName();
			this.RefreshSelectedBonusInfoType();
			this.RefreshSelectedBonusInfoGrade();
			this.RefreshSelectedBonusInfoCharFavor();
			this.RefreshSelectedBonusInfoCharAttainment();
		}

		// Token: 0x060092A0 RID: 37536 RVA: 0x00444AA0 File Offset: 0x00442CA0
		private void RefreshSelectedBonusInfoGrade()
		{
			sbyte grade = this.GetSelectedBonusGrade();
			string title = LanguageKey.LK_ItemGrade.Tr();
			string value = LocalStringManager.Get(string.Format("LK_Grade_{0}", grade)).SetGradeColor((int)grade);
			this.bonusInfoGradeProperty.Set(string.Empty, title, value, new bool?(true), false);
		}

		// Token: 0x060092A1 RID: 37537 RVA: 0x00444AF8 File Offset: 0x00442CF8
		private void RefreshSelectedBonusInfoType()
		{
			bool flag = this.DisplayingBonus == null;
			if (!flag)
			{
				string bonusType = SkillBreakBonusEffect.Instance[this.DisplayingBonus.Value.Effect.TemplateId].Name;
				string title = LanguageKey.LK_ItemType.Tr();
				string value = bonusType.SetColor("normaladventure");
				this.bonusInfoTypeProperty.Set(string.Empty, title, value, new bool?(false), false);
			}
		}

		// Token: 0x060092A2 RID: 37538 RVA: 0x00444B7C File Offset: 0x00442D7C
		private void RefreshSelectedBonusInfoName()
		{
			string title = LanguageKey.LK_Name.Tr();
			string value = this.GetSelectedBonusName().SetColor("pinkyellow");
			this.bonusInfoNameProperty.Set(string.Empty, title, value, new bool?(true), false);
		}

		// Token: 0x060092A3 RID: 37539 RVA: 0x00444BC0 File Offset: 0x00442DC0
		private void RefreshSelectedBonusInfoCharFavor()
		{
			bool flag = this.DisplayingBonus == null;
			if (!flag)
			{
				ESkillBreakPlateBonusType type = this.DisplayingBonus.Value.Type;
				bool show = type == ESkillBreakPlateBonusType.Relation || type == ESkillBreakPlateBonusType.Friend;
				this.bonusInfoFavorProperty.gameObject.SetActive(show);
				bool flag2 = show;
				if (flag2)
				{
					string title = LanguageKey.LK_Favorability.Tr();
					string value = CommonUtils.GetFavorString(this.DisplayingBonus.Value.Favorability);
					this.bonusInfoFavorProperty.Set(string.Empty, title, value, new bool?(true), false);
				}
			}
		}

		// Token: 0x060092A4 RID: 37540 RVA: 0x00444C74 File Offset: 0x00442E74
		private void RefreshSelectedBonusInfoCharAttainment()
		{
			bool flag = this.DisplayingBonus == null;
			if (!flag)
			{
				bool show = this.DisplayingBonus.Value.Type == ESkillBreakPlateBonusType.Friend;
				this.bonusInfoAttainmentProperty.gameObject.SetActive(show);
				bool flag2 = show;
				if (flag2)
				{
					string title = LanguageKey.LK_Skill_Break_BonusSelect_FriendAttainment.Tr();
					string value = this.DisplayingBonus.Value.FriendAttainment.ToString();
					this.bonusInfoAttainmentProperty.Set(string.Empty, title, value, new bool?(true), false);
				}
			}
		}

		// Token: 0x060092A5 RID: 37541 RVA: 0x00444D18 File Offset: 0x00442F18
		private void RefreshBonusCharacterHead()
		{
			bool flag = this.DisplayingBonus == null;
			if (!flag)
			{
				int charId = this.DisplayingBonus.Value.RelationCharId;
				CharacterDisplayData selectedCharacterDisplayData = this._skillBreakBonusSelectDisplayData.SelectedCharacterDisplayData;
				int selectedId = (selectedCharacterDisplayData != null) ? selectedCharacterDisplayData.CharacterId : -1;
				CharacterDisplayData characterDisplayData;
				if (charId != selectedId)
				{
					SkillBreakBonusSelectableItem skillBreakBonusSelectableItem = this._skillBreakBonusSelectDisplayData.CharacterBonusItemList.Find((SkillBreakBonusSelectableItem d) => d.CharacterDisplayData.CharacterId == charId);
					characterDisplayData = ((skillBreakBonusSelectableItem != null) ? skillBreakBonusSelectableItem.CharacterDisplayData : null);
				}
				else
				{
					characterDisplayData = this._skillBreakBonusSelectDisplayData.SelectedCharacterDisplayData;
				}
				CharacterDisplayData displayData = characterDisplayData;
				bool flag2 = displayData != null;
				if (flag2)
				{
					this.selectedBonusCharacterAvatar.Refresh(displayData, true);
					this._selectedCharDisplayData = displayData;
				}
			}
		}

		// Token: 0x060092A6 RID: 37542 RVA: 0x00444DE0 File Offset: 0x00442FE0
		private void RefreshBonusItemView()
		{
			bool flag = this.DisplayingBonus == null;
			if (!flag)
			{
				this._bonusItemDisplayData.Key = new ItemKey
				{
					ItemType = this.DisplayingBonus.Value.ItemType,
					TemplateId = this.DisplayingBonus.Value.ItemTemplateId
				};
				this.selectedBonusItemBack.Set(this._bonusItemDisplayData, false);
			}
		}

		// Token: 0x060092A7 RID: 37543 RVA: 0x00444E68 File Offset: 0x00443068
		private void SwitchSelectedBonusSlot()
		{
			bool flag = this.DisplayingBonus == null;
			if (!flag)
			{
				ESkillBreakPlateBonusType type = this.DisplayingBonus.Value.Type;
				this.selectedBonusItem.SetActive(type == ESkillBreakPlateBonusType.Item);
				this.selectedBonusExp.SetActive(type == ESkillBreakPlateBonusType.Exp);
				this.selectedBonusCharacter.SetActive(type == ESkillBreakPlateBonusType.Relation || type == ESkillBreakPlateBonusType.Friend);
			}
		}

		// Token: 0x060092A8 RID: 37544 RVA: 0x00444EDC File Offset: 0x004430DC
		private void ConfirmAndClose()
		{
			bool flag = this._generatedBonus == null;
			if (!flag)
			{
				switch (this._generatedBonus.Value.Type)
				{
				case ESkillBreakPlateBonusType.None:
					this.ConfirmClear();
					break;
				case ESkillBreakPlateBonusType.Item:
					this.ConfirmItem();
					break;
				case ESkillBreakPlateBonusType.Relation:
				case ESkillBreakPlateBonusType.Friend:
					this.ConfirmCharacter();
					break;
				case ESkillBreakPlateBonusType.Exp:
					this.ConfirmExp();
					break;
				}
				this.QuickHide();
			}
		}

		// Token: 0x060092A9 RID: 37545 RVA: 0x00444F5C File Offset: 0x0044315C
		private void ConfirmCharacter()
		{
			bool flag = this._generatedBonus == null;
			if (!flag)
			{
				int charId = this._generatedBonus.Value.RelationCharId;
				ushort relation = this._generatedBonus.Value.RelationType;
				Action<int, ushort> onConfirmRelation = this._onConfirmRelation;
				if (onConfirmRelation != null)
				{
					onConfirmRelation(charId, relation);
				}
			}
		}

		// Token: 0x060092AA RID: 37546 RVA: 0x00444FBA File Offset: 0x004431BA
		private void ConfirmClear()
		{
			Action onConfirmClear = this._onConfirmClear;
			if (onConfirmClear != null)
			{
				onConfirmClear();
			}
		}

		// Token: 0x060092AB RID: 37547 RVA: 0x00444FD0 File Offset: 0x004431D0
		private void ConfirmExp()
		{
			bool flag = this._generatedBonus == null;
			if (!flag)
			{
				Action<int> onConfirmExp = this._onConfirmExp;
				if (onConfirmExp != null)
				{
					onConfirmExp(this._generatedBonus.Value.ExpLevel);
				}
			}
		}

		// Token: 0x060092AC RID: 37548 RVA: 0x00445018 File Offset: 0x00443218
		private void ConfirmItem()
		{
			bool flag = this._generatedBonus == null;
			if (!flag)
			{
				Action<ItemKey, ItemSourceType> onConfirmItem = this._onConfirmItem;
				if (onConfirmItem != null)
				{
					onConfirmItem(this._selectedItemKey, this._selectedItemSourceType);
				}
			}
		}

		// Token: 0x060092AD RID: 37549 RVA: 0x00445058 File Offset: 0x00443258
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 1)
				{
					this.HandleMethodReturn(notification, wrapper);
				}
			}
		}

		// Token: 0x060092AE RID: 37550 RVA: 0x004450C8 File Offset: 0x004432C8
		private void HandleMethodReturn(Notification notification, NotificationWrapper wrapper)
		{
			ushort domainId = notification.DomainId;
			ushort methodId = notification.MethodId;
			RawDataPool pool = wrapper.DataPool;
			int offset = notification.ValueOffset;
			bool flag = domainId == 5 && methodId == 200;
			if (flag)
			{
				Serializer.Deserialize(pool, offset, ref this._skillBreakBonusSelectDisplayData);
				this.RefreshBonusDisplay();
				this.RefreshNoBonusCombatSkillView();
				this._sortAndFilterController.SetToggleIsOn(0, 0);
				this.Element.ShowAfterRefresh();
				GlobalDomainMethod.Call.InvokeGuidingTrigger(131);
			}
		}

		// Token: 0x060092AF RID: 37551 RVA: 0x0044514C File Offset: 0x0044334C
		private sbyte GetSelectedBonusGrade()
		{
			SkillBreakPlateBonus? skillBreakPlateBonus;
			return (this.DisplayingBonus != null) ? skillBreakPlateBonus.GetValueOrDefault().Grade : -1;
		}

		// Token: 0x060092B0 RID: 37552 RVA: 0x00445180 File Offset: 0x00443380
		private string GetSelectedBonusName()
		{
			bool flag = this.DisplayingBonus == null;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				switch (this.DisplayingBonus.Value.Type)
				{
				case ESkillBreakPlateBonusType.Item:
					result = ItemTemplateHelper.GetName(this.DisplayingBonus.Value.ItemType, this.DisplayingBonus.Value.ItemTemplateId);
					break;
				case ESkillBreakPlateBonusType.Relation:
				case ESkillBreakPlateBonusType.Friend:
					result = ((this._selectedCharDisplayData == null) ? string.Empty : NameCenter.GetMonasticTitleOrDisplayName(this._selectedCharDisplayData, false));
					break;
				case ESkillBreakPlateBonusType.Exp:
				{
					IReadOnlyList<int> levels = SkillBreakPlateConstants.ExpLevelValues;
					int hasExp = this._currentExp;
					int needExp = levels[this.DisplayingBonus.Value.ExpLevel];
					string expColor = (hasExp >= needExp) ? "brightblue" : "brightred";
					string hasExpText = hasExp.ToString().SetColor(expColor);
					result = string.Format("{0}/{1}", hasExpText, needExp);
					break;
				}
				default:
					result = string.Empty;
					break;
				}
			}
			return result;
		}

		// Token: 0x060092B1 RID: 37553 RVA: 0x004452AC File Offset: 0x004434AC
		private static bool IsSameBonus(SkillBreakPlateBonus x, SkillBreakPlateBonus y)
		{
			bool flag = x.Type != y.Type;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				switch (x.Type)
				{
				case ESkillBreakPlateBonusType.None:
					result = true;
					break;
				case ESkillBreakPlateBonusType.Item:
					result = (x.ItemType == y.ItemType && x.ItemTemplateId == y.ItemTemplateId);
					break;
				case ESkillBreakPlateBonusType.Relation:
					result = (x.RelationType == y.RelationType && x.RelationCharId == y.RelationCharId);
					break;
				case ESkillBreakPlateBonusType.Exp:
					result = (x.ExpLevel == y.ExpLevel);
					break;
				case ESkillBreakPlateBonusType.Friend:
					result = (x.RelationCharId == y.RelationCharId);
					break;
				default:
					result = false;
					break;
				}
			}
			return result;
		}

		// Token: 0x060092B2 RID: 37554 RVA: 0x0044537C File Offset: 0x0044357C
		public ViewSkillBreakBonusSelect()
		{
			ItemSourceType[] array = new ItemSourceType[3];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.4636993D3E1DA4E9D6B8F87B79E8F7C6D018580D52661950EABC3845C5897A4D).FieldHandle);
			this._itemSourceTypeArray = array;
			this._possiblePowerList = new List<int>();
			this._itemSelectorWorldCorners = new Vector3[4];
			this._filteredData = new List<SkillBreakBonusSelectableItem>();
			this._selectorMode = ViewSkillBreakBonusSelect.ESelectorMode.Invalid;
			this._resultList = new List<SkillBreakBonusEffectDisplay>();
			this._bonusItemDisplayData = new ItemDisplayData();
			base..ctor();
		}

		// Token: 0x060092B3 RID: 37555 RVA: 0x004453E6 File Offset: 0x004435E6
		[CompilerGenerated]
		private void <SelectNoBonus>g__Action|47_0()
		{
			this.GenerateEmptyBonus();
			this._hoverGeneratedBonus = null;
			this._selectedItemKey = ItemKey.Invalid;
			this.RefreshBonusDisplay();
			this.scroll.InfiniteScroll.ReRender();
		}

		// Token: 0x040070E6 RID: 28902
		private readonly ItemSourceType[] _itemSourceTypeArray;

		// Token: 0x040070E7 RID: 28903
		private Action<ItemKey, ItemSourceType> _onConfirmItem;

		// Token: 0x040070E8 RID: 28904
		private Action<int> _onConfirmExp;

		// Token: 0x040070E9 RID: 28905
		private Action<int, ushort> _onConfirmRelation;

		// Token: 0x040070EA RID: 28906
		private Action _onConfirmClear;

		// Token: 0x040070EB RID: 28907
		private int _currentExp;

		// Token: 0x040070EC RID: 28908
		private LifeSkillShorts _lifeSkillAttainments;

		// Token: 0x040070ED RID: 28909
		private SkillBreakPlateBonus _currentBonus;

		// Token: 0x040070EE RID: 28910
		private short _skillId;

		// Token: 0x040070EF RID: 28911
		private readonly List<int> _possiblePowerList;

		// Token: 0x040070F0 RID: 28912
		private SkillBreakBonusSelectDisplayData _skillBreakBonusSelectDisplayData;

		// Token: 0x040070F1 RID: 28913
		private ItemSourceType _selectedItemSourceType;

		// Token: 0x040070F2 RID: 28914
		private CharacterDisplayData _selectedCharDisplayData;

		// Token: 0x040070F3 RID: 28915
		private SkillBreakPlateBonus? _generatedBonus;

		// Token: 0x040070F4 RID: 28916
		private ItemKey _selectedItemKey;

		// Token: 0x040070F5 RID: 28917
		private SkillBreakPlateBonus? _hoverGeneratedBonus;

		// Token: 0x040070F6 RID: 28918
		private ItemDisplayData _hoverItemDisplayData;

		// Token: 0x040070F7 RID: 28919
		private readonly Vector3[] _itemSelectorWorldCorners;

		// Token: 0x040070F8 RID: 28920
		private BonusSelectSortAndFilterController _sortAndFilterController;

		// Token: 0x040070F9 RID: 28921
		private readonly List<SkillBreakBonusSelectableItem> _filteredData;

		// Token: 0x040070FA RID: 28922
		private ViewSkillBreakBonusSelect.ESelectorMode _selectorMode;

		// Token: 0x040070FB RID: 28923
		private readonly List<SkillBreakBonusEffectDisplay> _resultList;

		// Token: 0x040070FC RID: 28924
		private readonly ItemDisplayData _bonusItemDisplayData;

		// Token: 0x040070FD RID: 28925
		[SerializeField]
		private SkillBreakBonusEffect bonusEffectItemTemplate;

		// Token: 0x040070FE RID: 28926
		[SerializeField]
		private CButton confirmButton;

		// Token: 0x040070FF RID: 28927
		[SerializeField]
		private TooltipInvoker tipConfirmButton;

		// Token: 0x04007100 RID: 28928
		[SerializeField]
		private CToggleGroup sourceFilterGroup;

		// Token: 0x04007101 RID: 28929
		[SerializeField]
		private CharacterMenuCombatSkillItem combatSkillItem;

		// Token: 0x04007102 RID: 28930
		[SerializeField]
		private ItemBack selectedBonusItemBack;

		// Token: 0x04007103 RID: 28931
		[SerializeField]
		private GameObject selectedBonusCharacter;

		// Token: 0x04007104 RID: 28932
		[SerializeField]
		private GameObject selectedBonusExp;

		// Token: 0x04007105 RID: 28933
		[SerializeField]
		private GameObject selectedBonusItem;

		// Token: 0x04007106 RID: 28934
		[SerializeField]
		private RectTransform bonusEffectLayout;

		// Token: 0x04007107 RID: 28935
		[SerializeField]
		private TextMeshProUGUI activeDescLabel;

		// Token: 0x04007108 RID: 28936
		[SerializeField]
		private TextMeshProUGUI activeDescLabelRelation;

		// Token: 0x04007109 RID: 28937
		[SerializeField]
		private PropertyItem bonusInfoGradeProperty;

		// Token: 0x0400710A RID: 28938
		[SerializeField]
		private PropertyItem bonusInfoNameProperty;

		// Token: 0x0400710B RID: 28939
		[SerializeField]
		private PropertyItem bonusInfoTypeProperty;

		// Token: 0x0400710C RID: 28940
		[SerializeField]
		private PropertyItem bonusInfoFavorProperty;

		// Token: 0x0400710D RID: 28941
		[SerializeField]
		private PropertyItem bonusInfoAttainmentProperty;

		// Token: 0x0400710E RID: 28942
		[SerializeField]
		private PropertyItem combatSkillNameProperty;

		// Token: 0x0400710F RID: 28943
		[SerializeField]
		private PropertyItem combatSkillSectProperty;

		// Token: 0x04007110 RID: 28944
		[SerializeField]
		private PropertyItem combatSkillTypeProperty;

		// Token: 0x04007111 RID: 28945
		[SerializeField]
		private PropertyItem powerProperty;

		// Token: 0x04007112 RID: 28946
		[SerializeField]
		private Game.Components.Avatar.Avatar selectedBonusCharacterAvatar;

		// Token: 0x04007113 RID: 28947
		[SerializeField]
		private ItemBack selectedExpFakeItemBack;

		// Token: 0x04007114 RID: 28948
		[SerializeField]
		private TooltipInvoker combatSkillAreaTips;

		// Token: 0x04007115 RID: 28949
		[SerializeField]
		private GameObject[] itemsShowWhenHasBonus;

		// Token: 0x04007116 RID: 28950
		[SerializeField]
		private GameObject[] itemsShowWhenNoBonus;

		// Token: 0x04007117 RID: 28951
		[SerializeField]
		private ListStyleGeneralScroll scroll;

		// Token: 0x04007118 RID: 28952
		[Header("行模板配置")]
		[SerializeField]
		private RowItem rowTemplate;

		// Token: 0x04007119 RID: 28953
		[SerializeField]
		private RowItem rowTemplateForChar;

		// Token: 0x0400711A RID: 28954
		[SerializeField]
		private RowCellContainer singleTextCellContainer;

		// Token: 0x0400711B RID: 28955
		[SerializeField]
		private RowCellContainer itemIconAndNameCellContainer;

		// Token: 0x0400711C RID: 28956
		[SerializeField]
		private RowCellContainer avatarAndNameCellContainer;

		// Token: 0x0400711D RID: 28957
		[SerializeField]
		private RowCellContainer avatarSingleTextCellContainer;

		// Token: 0x0400711E RID: 28958
		[SerializeField]
		private RowCellContainer expCellContainer;

		// Token: 0x0400711F RID: 28959
		[Header("排序筛选")]
		[SerializeField]
		private SortAndFilter sortAndFilter;

		// Token: 0x04007120 RID: 28960
		[SerializeField]
		private GameObject sortGo;

		// Token: 0x04007121 RID: 28961
		[SerializeField]
		private GameObject filterGo;

		// Token: 0x020021AA RID: 8618
		private enum ESelectorMode
		{
			// Token: 0x0400D695 RID: 54933
			Invalid = -1,
			// Token: 0x0400D696 RID: 54934
			Exp,
			// Token: 0x0400D697 RID: 54935
			Item,
			// Token: 0x0400D698 RID: 54936
			Character
		}

		// Token: 0x020021AB RID: 8619
		[Flags]
		public enum EColumnType
		{
			// Token: 0x0400D69A RID: 54938
			IconAndName = 1,
			// Token: 0x0400D69B RID: 54939
			Amount = 2,
			// Token: 0x0400D69C RID: 54940
			Type = 4,
			// Token: 0x0400D69D RID: 54941
			Weight = 8,
			// Token: 0x0400D69E RID: 54942
			Value = 16,
			// Token: 0x0400D69F RID: 54943
			AvatarAndName = 32,
			// Token: 0x0400D6A0 RID: 54944
			Grade = 64,
			// Token: 0x0400D6A1 RID: 54945
			Favor = 128,
			// Token: 0x0400D6A2 RID: 54946
			Attainment = 256,
			// Token: 0x0400D6A3 RID: 54947
			Exp = 512
		}
	}
}
